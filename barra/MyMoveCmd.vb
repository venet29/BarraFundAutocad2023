
Imports System.Collections.Generic


' rutina similar a move 
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.GraphicsInterface




Imports System
Imports Autodesk.AutoCAD.Runtime

Imports System.Math

Imports Autodesk.AutoCAD.Windows
Imports Autodesk.AutoCAD.Colors
'Imports Autodesk.AutoCAD.GraphicsInterface


Imports System.Runtime




Imports System.IO
Imports System.Security
Imports System.Security.Cryptography
Imports System.Text
Imports System.Data
Imports System.Data.SqlClient
Imports System.Drawing


Public Class MyMoveCmd
    Private _dwg As Document
    Private _editor As Editor
    Private _selectedIds As ObjectIdCollection
    Private _basePoint As Point3d
    Private _currentPoint As Point3d
    Private _moveToPoint As Point3d
    Private _tgDrawables As List(Of Drawable)
    Private _pointMonitored As Boolean = False

    Public Sub New(ByVal dwg As Document)
        _dwg = dwg
        _editor = _dwg.Editor
    End Sub

    Public Sub DoMove()
        'Select object and highlight selected
        Dim ids As New ObjectIdCollection()

        Using tran As Transaction = _
            _dwg.Database.TransactionManager.StartOpenCloseTransaction()

            If Not GetSelectedEntities(tran, ids) Then
                _editor.WriteMessage(vbLf & "*Cancel*")
                ids = New ObjectIdCollection()

            End If
        End Using

        If ids.Count = 0 Then
            Return
        End If

        _selectedIds = ids

        'Calculate base point (lower-left conner of 
        'bounding box that encloses all selected entities)
        _basePoint = GetDefaultBasePoint()

        'current dragging point
        _currentPoint = _basePoint

        Dim trans As Transaction = _dwg.Database.TransactionManager.StartTransaction()

        CreateTransientGraphics(trans)

        Try

            AddHandler _editor.PointMonitor, AddressOf Editor_PointMonitor
            _pointMonitored = True

            While True
                Dim opt As New PromptPointOptions(vbLf & "Select point to move to: ")
                opt.UseBasePoint = True
                opt.BasePoint = _basePoint
                opt.Keywords.Add("Base point")
                opt.AppendKeywordsToMessage = True
                Dim res As PromptPointResult = _editor.GetPoint(opt)
                If res.Status = PromptStatus.OK Then
                    'Get "Move To" point
                    If IsOrthModeOn() Then
                        _moveToPoint = GetOrthoPoint(res.Value)
                    Else
                        _moveToPoint = res.Value
                    End If

                    ' con esto se mueve el objet MoveEntities(trans)
                    _editor.WriteMessage(vbLf & "{0} moved", _selectedIds.Count)
                    Exit While
                ElseIf res.Status = PromptStatus.Keyword Then
                    'If user choose to pick BasePoint,
                    'Stop habdling PointMonitor
                    ClearTransientGraphics()

                    RemoveHandler _editor.PointMonitor, AddressOf Editor_PointMonitor
                    _pointMonitored = False

                    Dim p As Point3d
                    If Not PickBasePoint(p) Then
                        _editor.WriteMessage(vbLf & "*Cancel*")
                        Exit While
                    Else
                        'Reset base point and current dragging point
                        _basePoint = p
                        _currentPoint = _basePoint

                        'Re-create transient graphics
                        CreateTransientGraphics(trans)

                        AddHandler _editor.PointMonitor, AddressOf Editor_PointMonitor
                        _pointMonitored = True
                    End If
                Else
                    _editor.WriteMessage(vbLf & "*Cancel*")
                    Exit While
                End If
            End While
        Catch
            Throw
        Finally
            ClearHighlight(trans)
            ClearTransientGraphics()

            If _pointMonitored Then
                RemoveHandler _editor.PointMonitor, AddressOf Editor_PointMonitor
            End If

            trans.Commit()
            trans.Dispose()
        End Try

    End Sub

#Region "private metods"

    Private Function GetSelectedEntities(ByVal tran As Transaction, _
                                         ByRef ids As ObjectIdCollection) As Boolean

        ids = New ObjectIdCollection()

        Dim res As PromptSelectionResult = _editor.SelectImplied()
        If res.Status = PromptStatus.OK Then
            For Each id As ObjectId In res.Value.GetObjectIds()
                ids.Add(id)
                HighlightEntity(tran, id, True)
            Next
        Else
            While True
                Dim msg As String = If(ids.Count > 0, Convert.ToString(ids.Count) & " selected. Pick entity: ", "Pick entity: ")
                Dim opt As New PromptEntityOptions(vbLf & msg)
                opt.AllowNone = True
                Dim entRes As PromptEntityResult = _editor.GetEntity(opt)
                If entRes.Status = PromptStatus.OK Then
                    Dim exists As Boolean = False
                    For Each id As ObjectId In ids
                        If id = entRes.ObjectId Then
                            exists = True
                            Exit For
                        End If
                    Next

                    If Not exists Then
                        ids.Add(entRes.ObjectId)
                        HighlightEntity(tran, entRes.ObjectId, True)
                    End If
                ElseIf entRes.Status = PromptStatus.None Then
                    Exit While
                Else
                    Return False
                End If
            End While
        End If

        Return True
    End Function

    Private Sub HighlightEntity(ByVal tran As Transaction, _
                                ByVal id As ObjectId, ByVal highlight As Boolean)

        Dim ent As Entity = DirectCast(tran.GetObject(id, OpenMode.ForWrite), Entity)
        If highlight Then
            ent.Highlight()
        Else
            ent.Unhighlight()
        End If

    End Sub

    Private Function GetDefaultBasePoint() As Point3d

        Dim exts As New Extents3d(New Point3d(0.0, 0.0, 0.0), New Point3d(0.0, 0.0, 0.0))

        Using tran As Transaction = _dwg.Database.TransactionManager.StartTransaction()
            For i As Integer = 0 To _selectedIds.Count - 1
                Dim id As ObjectId = _selectedIds(i)
                Dim ent As Entity = DirectCast(tran.GetObject(id, OpenMode.ForRead), Entity)

                If i = 0 Then
                    exts = ent.GeometricExtents

                Else
                    Dim ext As Extents3d = ent.GeometricExtents

                    exts.AddExtents(ext)
                End If
            Next

            tran.Commit()
        End Using

        Return exts.MinPoint

    End Function

    Private Function PickBasePoint(ByRef pt As Point3d) As Boolean

        pt = New Point3d()

        Dim opt As New PromptPointOptions("Pick base point: ")
        Dim res As PromptPointResult = _editor.GetPoint(opt)
        If res.Status = PromptStatus.OK Then
            pt = res.Value
            Return True
        Else
            Return False
        End If

    End Function

    Private Function IsOrthModeOn() As Boolean

        Dim orth As Object = Autodesk.AutoCAD.ApplicationServices. _
            Application.GetSystemVariable("ORTHOMODE")

        Return Convert.ToInt32(orth) > 0

    End Function

    Private Function GetOrthoPoint(ByVal pt As Point3d) As Point3d

        Dim x As Double = pt.X
        Dim y As Double = pt.Y

        Dim vec As Vector3d = _basePoint.GetVectorTo(pt)
        If Math.Abs(vec.X) >= Math.Abs(vec.Y) Then
            y = _basePoint.Y
        Else
            x = _basePoint.X
        End If

        Return New Point3d(x, y, 0.0)

    End Function

    Private Sub ClearHighlight(ByVal trans As Transaction)
        For Each id As ObjectId In _selectedIds
            HighlightEntity(trans, id, False)
        Next
    End Sub

    Private Sub MoveEntities(ByVal tran As Transaction)

        Dim mat As Matrix3d = Matrix3d.Displacement(_basePoint.GetVectorTo(_moveToPoint))
        For Each id As ObjectId In _selectedIds
            Dim ent As Entity = DirectCast(tran.GetObject(id, OpenMode.ForWrite), Entity)
            ent.TransformBy(mat)
        Next

    End Sub

#End Region

#Region "private method: handling PointMonitor and transient graphics"

    Private Sub Editor_PointMonitor(ByVal sender As Object, ByVal e As PointMonitorEventArgs)
        Dim pt As Point3d = e.Context.RawPoint
        If IsOrthModeOn() Then
            pt = GetOrthoPoint(pt)
        End If

        UpdateTransientGraphics(pt)

        _currentPoint = pt
    End Sub

    Private Sub CreateTransientGraphics(ByVal tran As Transaction)

        _tgDrawables = New List(Of Drawable)()

        For Each id As ObjectId In _selectedIds
            Dim drawable As Entity = DirectCast(tran.GetObject(id, OpenMode.ForWrite), Entity)

            'Dim drawable As Entity = TryCast(ent.Clone(), Entity)
            drawable.ColorIndex = 1
            _tgDrawables.Add(drawable)
        Next

        For Each d As Drawable In _tgDrawables
            TransientManager.CurrentTransientManager.AddTransient(d, TransientDrawingMode.DirectShortTerm, 128, New IntegerCollection())
        Next
    End Sub

    Private Sub UpdateTransientGraphics(ByVal moveToPoint As Point3d)

        Dim mat As Matrix3d = Matrix3d.Displacement(_currentPoint.GetVectorTo(moveToPoint))

        For Each d As Drawable In _tgDrawables
            Dim e As Entity = TryCast(d, Entity)
            e.TransformBy(mat)

            TransientManager.CurrentTransientManager.UpdateTransient(d, New IntegerCollection())
        Next

    End Sub

    Private Sub ClearTransientGraphics()

        TransientManager.CurrentTransientManager.EraseTransients(TransientDrawingMode.DirectShortTerm, 128, New IntegerCollection())

        For Each d As Drawable In _tgDrawables
            d.Dispose()
        Next

        _tgDrawables.Clear()

    End Sub

#End Region
End Class
