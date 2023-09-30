
Imports Autodesk.AutoCAD.DatabaseServices
Imports System
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.ApplicationServices
Imports System.Math
Imports System.IO
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Windows
Imports Autodesk.AutoCAD.Colors
Imports VARIOS

Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.Window

Public Class utiles
    Public Const ERROR_CONEXION As String = "Error CX" ' "Error al validad credencial (No conexion)"
    Public Const ERROR_OBTENERDATOS As String = "Error datos" ' "Error al validad credencial (No conexion)"
    Public Shared Sub ErrorMsg(ByVal msg As String)

        System.Windows.Forms.MessageBox.Show(msg, "Mensaje", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error)
    End Sub

    Function NOMBRE_PLANO_F() As String



        NOMBRE_PLANO_F = ""

        Dim ed As Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
        Dim acDoc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        Dim acEd As Editor = acDoc.Editor

        ' Iniciar la interacion
        ' Iniciar una transaccion
        Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()

            ' Crear una matriz de objetos TypedValue para definir el criterio de seleccion
            Dim acTypValAr(2) As TypedValue
            acTypValAr.SetValue(New TypedValue(DxfCode.Start, "INSERT"), 0)
            acTypValAr.SetValue(New TypedValue(DxfCode.LayerName, "0"), 1)
            acTypValAr.SetValue(New TypedValue(DxfCode.BlockName, "VIÑETA"), 2)

            ' Asignar el criterio de seleccion al objeto SelectionFilter
            Dim acSelFtr As SelectionFilter = New SelectionFilter(acTypValAr)
            ' Solicitar al usuario que seleccione objetos en el área de dibujo
            Dim acSSPrompt As PromptSelectionResult
            acSSPrompt = acEd.SelectAll(acSelFtr)

            ' Si el estado de la solicitud es OK, es que se han seleccionado objetos
            If acSSPrompt.Status = PromptStatus.OK Then
                Dim acSSet As SelectionSet = acSSPrompt.Value
                For Each acObjId As ObjectId In acSSet.GetObjectIds()
                    If acObjId.ObjectClass.DxfName.ToString = "INSERT" Then
                        Dim blkRef As BlockReference = DirectCast(acTrans.GetObject(acObjId, OpenMode.ForWrite), BlockReference)
                        Dim btr As BlockTableRecord = DirectCast(acTrans.GetObject(blkRef.BlockTableRecord, OpenMode.ForWrite), BlockTableRecord)
                        btr.Dispose()
                        Dim attCol As AttributeCollection = blkRef.AttributeCollection

                        For Each attId As ObjectId In attCol
                            Dim attRef As AttributeReference = DirectCast(acTrans.GetObject(attId, OpenMode.ForWrite), AttributeReference)
                            If attRef.Tag = "TITULO" Then
                                NOMBRE_PLANO_F = NOMBRE_PLANO_F & " " & attRef.TextString
                            ElseIf attRef.Tag = "TITULO1" Then
                                NOMBRE_PLANO_F = NOMBRE_PLANO_F & " " & attRef.TextString
                            ElseIf attRef.Tag = "TITULO2" Then
                                NOMBRE_PLANO_F = NOMBRE_PLANO_F & " " & attRef.TextString
                            End If
                        Next
                    End If
                Next



            Else
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Cantidad de objetos seleccionados: 0")

            End If
            acTrans.Commit()
        End Using

        ' Libera recursos y vuelve a mostrar el cuadro de dialogo


        Return NOMBRE_PLANO_F
    End Function

    Public Sub marca_error_barra_f(ByRef ent_obj As ObjectId, ByVal msj As String, ByRef aux_mje As Boolean, ByRef aux_grp As Boolean)
        ' se puede usar enticada o objeto  ej        Dim acPoly As Polyline
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        Dim ed As Editor = Application.DocumentManager.MdiActiveDocument.Editor
        Dim CODIGOS_GRUPOS_ As New CODIGOS_GRUPOS()
        Dim CODIGOS_DATOS_ As New CODIGOS_DATOS()
        Create_ALayer("NHREVISION", 2)


        Dim PTOS2 As Extents3d
        Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
            Dim enty As Entity = TryCast(acTrans.GetObject(ent_obj, OpenMode.ForRead), Entity)
            PTOS2 = enty.Bounds()
            acTrans.Commit()
        End Using

        Dim ext As System.Nullable(Of Extents3d) = PTOS2
        If ext.HasValue Then
            Dim maxPt As Point3d = ext.Value.MaxPoint
            Dim minPt As Point3d = ext.Value.MinPoint
            Dim ents As ObjectIdCollection = New ObjectIdCollection()
            Dim nombre_grupo As String = CODIGOS_GRUPOS_.buscar_nombre_grupo(ent_obj)
            Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
                ' Abrir la tabla para bloques en modo lectura
                Dim acBlkTbl As BlockTable
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead)
                ' Abrir el registro del bloque de Espacio Modelo para escritura
                Dim acBlkTblRec As BlockTableRecord
                acBlkTblRec = acTrans.GetObject(acBlkTbl(BlockTableRecord.ModelSpace), OpenMode.ForWrite)
                ' Crear la polilinea con dos segmentos (3 puntos)
                Dim acPoly2 As Autodesk.AutoCAD.DatabaseServices.Polyline = New Autodesk.AutoCAD.DatabaseServices.Polyline()
                acPoly2.SetDatabaseDefaults()
                Dim DELTA As Integer = 40
                acPoly2.AddVertexAt(0, New Point2d(minPt.X - DELTA, minPt.Y - DELTA), 0, 0, 0)
                acPoly2.AddVertexAt(1, New Point2d(minPt.X - DELTA, maxPt.Y + DELTA), 0, 0, 0)
                acPoly2.AddVertexAt(2, New Point2d(maxPt.X + DELTA, maxPt.Y + DELTA), 0, 0, 0)
                acPoly2.AddVertexAt(3, New Point2d(maxPt.X + DELTA, minPt.Y - DELTA), 0, 0, 0)
                acPoly2.AddVertexAt(4, New Point2d(minPt.X - DELTA, minPt.Y - DELTA), 0, 0, 0)
                acPoly2.Layer = "NHREVISION"
                ' Añadir el nuevo objeto al registro de la tabla para bloques
                ' y a la transaccion
                acBlkTblRec.AppendEntity(acPoly2)
                acTrans.AddNewlyCreatedDBObject(acPoly2, True)
                ents.Add(acPoly2.ObjectId)
                ' Guardar el nuevo objeto
                acTrans.Commit()
            End Using


            If aux_grp Then CODIGOS_GRUPOS_.agregar_borrar_grupo(ents, nombre_grupo, "A")
            If aux_mje Then
                MsgBox(msj, vbCritical)
            Else
                ed.WriteMessage(msj)
            End If

        End If
    End Sub


    Function largo_poly(ByRef ent As Polyline, ByVal tipo_barra As String, ByVal direccion As String) As String()

        Dim largo(2) As String
        Dim largo_text As String = "0"

        Dim largo_real As Integer
        Dim pto_0, pto_1, pto_2, pto_3, pto_4 As Point2d

        If ent.NumberOfVertices > 2 Then

            If tipo_barra = "110" Or tipo_barra = "210" Then
                If direccion = "AZ" Then
                    pto_0 = ent.GetPoint2dAt(0)
                    pto_1 = ent.GetPoint2dAt(1)
                    pto_2 = ent.GetPoint2dAt(2)
                    pto_3 = ent.GetPoint2dAt(3)
                Else
                    pto_0 = ent.GetPoint2dAt(3)
                    pto_1 = ent.GetPoint2dAt(2)
                    pto_2 = ent.GetPoint2dAt(1)
                    pto_3 = ent.GetPoint2dAt(0)
                End If

                largo_real = CSng((Abs(pto_0.X - pto_1.X) ^ 2 + Abs(pto_0.Y - pto_1.Y) ^ 2) ^ 0.5) + CSng((Abs(pto_2.X - pto_3.X) ^ 2 + Abs(pto_2.Y - pto_3.Y) ^ 2) ^ 0.5) + 5

                largo_real = Math.Round(largo_real, MidpointRounding.AwayFromZero)

            ElseIf tipo_barra = "111" Or tipo_barra = "112" Or tipo_barra = "211" Or tipo_barra = "212" Then

                If direccion = "AZ" Then
                    pto_0 = ent.GetPoint2dAt(0)
                    pto_1 = ent.GetPoint2dAt(1)
                    pto_2 = ent.GetPoint2dAt(2)
                    pto_3 = ent.GetPoint2dAt(3)
                    pto_4 = ent.GetPoint2dAt(4)

                Else
                    pto_0 = ent.GetPoint2dAt(4)
                    pto_1 = ent.GetPoint2dAt(3)
                    pto_2 = ent.GetPoint2dAt(2)
                    pto_3 = ent.GetPoint2dAt(1)
                    pto_4 = ent.GetPoint2dAt(0)

                End If

                largo_real = CSng((Abs(pto_0.X - pto_1.X) ^ 2 + Abs(pto_0.Y - pto_1.Y) ^ 2) ^ 0.5) _
                           + CSng((Abs(pto_1.X - pto_2.X) ^ 2 + Abs(pto_1.Y - pto_2.Y) ^ 2) ^ 0.5) _
                           + CSng((Abs(pto_3.X - pto_4.X) ^ 2 + Abs(pto_3.Y - pto_4.Y) ^ 2) ^ 0.5) + 5
                largo_real = Math.Round(largo_real, MidpointRounding.AwayFromZero)

                largo_text = "(" & Math.Round((CSng((Abs(pto_1.X - pto_2.X) ^ 2 + Abs(pto_1.Y - pto_2.Y) ^ 2) ^ 0.5)) _
                                      + CSng((Abs(pto_3.X - pto_4.X) ^ 2 + Abs(pto_3.Y - pto_4.Y) ^ 2) ^ 0.5) + 5, MidpointRounding.AwayFromZero) _
                            & "+" & Math.Round(CSng((Abs(pto_0.X - pto_1.X) ^ 2 + Abs(pto_0.Y - pto_1.Y) ^ 2) ^ 0.5), MidpointRounding.AwayFromZero) & ")"


            ElseIf tipo_barra = "101" Or tipo_barra = "102" Or tipo_barra = "103" Or tipo_barra = "104" Or tipo_barra = "201" Or tipo_barra = "202" Or tipo_barra = "203" Or tipo_barra = "204" Then
                If direccion = "AZ" Then
                    pto_0 = ent.GetPoint2dAt(0)
                    pto_1 = ent.GetPoint2dAt(1)
                    pto_2 = ent.GetPoint2dAt(2)
                Else
                    pto_0 = ent.GetPoint2dAt(2)
                    pto_1 = ent.GetPoint2dAt(1)
                    pto_2 = ent.GetPoint2dAt(0)
                End If



                'Application.ShowAlertDialog("x1 : " & pto_0.X & "  y1: " & pto_0.Y &
                '                                "x2 : " & pto_1.X & "  y2: " & pto_1.Y &
                '                                "x3 : " & pto_2.X & "  y3: " & pto_2.Y)
                largo_real = CSng((Abs(pto_0.X - pto_1.X) ^ 2 + Abs(pto_0.Y - pto_1.Y) ^ 2) ^ 0.5 _
                                + (Abs(pto_1.X - pto_2.X) ^ 2 + Abs(pto_1.Y - pto_2.Y) ^ 2) ^ 0.5)
                largo_real = Math.Round(largo_real, MidpointRounding.AwayFromZero)

                largo_text = "(" & Math.Round(CSng((Abs(pto_1.X - pto_2.X) ^ 2 + Abs(pto_1.Y - pto_2.Y) ^ 2) ^ 0.5), MidpointRounding.AwayFromZero) _
                           & "+" & Math.Round(CSng((Abs(pto_0.X - pto_1.X) ^ 2 + Abs(pto_0.Y - pto_1.Y) ^ 2) ^ 0.5), MidpointRounding.AwayFromZero) & ")"

            ElseIf tipo_barra = "105" Or tipo_barra = "106" Or tipo_barra = "107" Or tipo_barra = "108" Or tipo_barra = "205" Or tipo_barra = "206" Or tipo_barra = "207" Or tipo_barra = "208" Then
                If direccion = "AZ" Then
                    pto_0 = ent.GetPoint2dAt(0)
                    pto_1 = ent.GetPoint2dAt(1)
                    pto_2 = ent.GetPoint2dAt(2)
                    pto_3 = ent.GetPoint2dAt(3)
                Else
                    pto_0 = ent.GetPoint2dAt(3)
                    pto_1 = ent.GetPoint2dAt(2)
                    pto_2 = ent.GetPoint2dAt(1)
                    pto_3 = ent.GetPoint2dAt(0)
                End If


                largo_real = CSng(Abs(pto_0.X - pto_1.X) ^ 2 + Abs(pto_0.Y - pto_1.Y) ^ 2) ^ 0.5 _
                           + CSng(Abs(pto_1.X - pto_2.X) ^ 2 + Abs(pto_1.Y - pto_2.Y) ^ 2) ^ 0.5 _
                           + CSng(Abs(pto_2.X - pto_3.X) ^ 2 + Abs(pto_2.Y - pto_3.Y) ^ 2) ^ 0.5
                largo_real = Math.Round(largo_real, MidpointRounding.AwayFromZero)

                largo_text = "(" & Math.Round(CSng((Abs(pto_2.X - pto_3.X) ^ 2 + Abs(pto_2.Y - pto_3.Y) ^ 2) ^ 0.5), MidpointRounding.AwayFromZero) _
                           & "+" & Math.Round(CSng((Abs(pto_1.X - pto_2.X) ^ 2 + Abs(pto_1.Y - pto_2.Y) ^ 2) ^ 0.5), MidpointRounding.AwayFromZero) _
                           & "+" & Math.Round(CSng((Abs(pto_0.X - pto_1.X) ^ 2 + Abs(pto_0.Y - pto_1.Y) ^ 2) ^ 0.5), MidpointRounding.AwayFromZero) & ")"
            End If



        Else
            '  Application.ShowAlertDialog("tipo_barra : " & tipo_barra)
            largo_real = CInt(ent.Length)
        End If
        largo(0) = largo_real
        largo(1) = largo_text
        Return largo
    End Function

    Function aproximar(ByRef valor As Double, ByRef referen As Double, ByRef diamtro_ As Integer) As Double
        aproximar = 0

        Dim AUX_String As String = valor
        If (diamtro_ <= 36) Then

            Dim split_ As String() = AUX_String.Split(New [Char]() {"."c, CChar(vbTab)})
            Dim parte_entera As Integer = Right(split_(0), 1)

            If split_.Length = 2 Then
                valor = parte_entera + CSng("0." & split_(1))
            Else
                valor = parte_entera
            End If

            Select Case valor
                Case Is < 0.01
                    aproximar = 0
                Case Is < 5
                    aproximar = 5 - valor
                Case 5
                    aproximar = 0
                Case Is < 5.00000001
                    aproximar = 0
                Case Is < 10
                    aproximar = 10 - valor
                Case Else
                    aproximar = 0
            End Select

        ElseIf diamtro_ = 16 Or diamtro_ > 16 Then  ' el diamtro_ > 16 esta solo para considerar como maxima aproximacion de 25 a 25

            valor = Right(CStr(valor), 2)

            If valor = 0 Then
                aproximar = 0
            ElseIf valor <= 25 Then

                aproximar = (25 - valor)
            ElseIf valor <= 50 Then
                aproximar = (50 - valor)
            ElseIf valor <= 75 Then
                aproximar = (75 - valor)
            ElseIf valor <= 100 Then
                aproximar = (100 - valor)
            End If
        ElseIf diamtro_ > 16 Then

            valor = Right(CStr(valor), 2)

            If valor <= 50 Then
                aproximar = (50 - valor)

            ElseIf valor <= 100 Then
                aproximar = (100 - valor)
            End If


        End If

        If referen <> 0 Then

            If referen < 0 Then
                aproximar = -aproximar
            End If
        End If


    End Function
    Function aproximar2(ByRef valor As Double, ByRef referen As Single) As Double
        aproximar2 = 0



        Dim AUX_String As String = valor
        Dim split_ As String() = AUX_String.Split(New [Char]() {"."c, CChar(vbTab)})
        '   Dim parte_entera As Integer = Right(CStr(Fix(valor)), 1)

        If split_.Length = 2 Then
            valor = CSng("0." & split_(1))
        Else
            valor = 0
        End If

        Select Case valor
            'Case Is < 0.01
            '    aproximar2 = 0
            Case Is < 0.01
                aproximar2 = -valor
            Case Is < 1
                aproximar2 = 1 - valor
            Case Else
                aproximar2 = 0
        End Select



        If referen <> 0 Then

            If referen < 0 Then
                aproximar2 = -aproximar2
            End If
        End If


    End Function

    Public Sub Create_ALayer(ByVal sLayerName As String)
        ' Obtener el documento y la base de datos actuales
        Dim acDoc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database

        ' Iniciar una transaccion
        Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()

            ' Abrir la tabla para capas en modo lectura
            Dim acLyrTbl As LayerTable = acTrans.GetObject(acCurDb.LayerTableId, OpenMode.ForRead)



            ' Comprobar si la capa 'Center' existe
            If acLyrTbl.Has(sLayerName) = False Then
                ' Crear una nueva capa
                Dim acLyrTblRec As LayerTableRecord = New LayerTableRecord()

                ' Asignar a la capa el color ACI 1 y un nombre
                If sLayerName = "CESTRIBO_E" Or sLayerName = "CESTRIBO_L" Then
                    acLyrTblRec.Color = Color.FromColorIndex(ColorMethod.ByAci, 137)
                ElseIf sLayerName = "CMALLA_V" Or sLayerName = "CMALLA_V" Then
                    acLyrTblRec.Color = Color.FromColorIndex(ColorMethod.ByAci, 0)
                ElseIf sLayerName = "BARRAS" Then
                    acLyrTblRec.Color = Color.FromColorIndex(ColorMethod.ByAci, 6)
                ElseIf sLayerName = "TEXTO" Then
                    acLyrTblRec.Color = Color.FromColorIndex(ColorMethod.ByAci, 3)
                End If

                acLyrTblRec.Name = sLayerName

                ' Actualizar la tabla para capas, para escribir
                acLyrTbl.UpgradeOpen()

                ' Añadir la nueva capa a la tabla y a la transaccion
                acLyrTbl.Add(acLyrTblRec)
                acTrans.AddNewlyCreatedDBObject(acLyrTblRec, True)
            End If
            ' Guardar los cambios
            acTrans.Commit()
        End Using
    End Sub

    Public Sub Create_ALayer(ByVal sLayerName As String, ByVal idexcolor As Integer)
        ' Obtener el documento y la base de datos actuales
        Dim acDoc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database

        ' Iniciar una transaccion
        Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()

            ' Abrir la tabla para capas en modo lectura
            Dim acLyrTbl As LayerTable = acTrans.GetObject(acCurDb.LayerTableId, OpenMode.ForRead)



            ' Comprobar si la capa 'Center' existe
            If acLyrTbl.Has(sLayerName) = False Then
                ' Crear una nueva capa
                Dim acLyrTblRec As LayerTableRecord = New LayerTableRecord()


                acLyrTblRec.Color = Color.FromColorIndex(ColorMethod.ByAci, idexcolor)
                acLyrTblRec.Name = sLayerName

                ' Actualizar la tabla para capas, para escribir
                acLyrTbl.UpgradeOpen()

                ' Añadir la nueva capa a la tabla y a la transaccion
                acLyrTbl.Add(acLyrTblRec)
                acTrans.AddNewlyCreatedDBObject(acLyrTblRec, True)
            End If
            ' Guardar los cambios
            acTrans.Commit()
        End Using
    End Sub

    Function per_hacth(ByVal prRes As ObjectId) As Point3dCollection
        Dim doc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
        Dim ed As Editor = doc.Editor
        Dim p3ds As New Point3dCollection()
        ' Dim prOps As New PromptEntityOptions(vbLf & "Select Hatch: ")
        ' prOps.SetRejectMessage(vbLf & "Not a Hatch")
        ' prOps.AddAllowedClass(GetType(Hatch), False)
        ' Dim prRes As PromptEntityResult = ed.GetEntity(prOps)
        ' If prRes.Status <> PromptStatus.OK Then
        'Return
        ' End If
        Using tr As Transaction = doc.TransactionManager.StartTransaction()
            Dim hatch As Hatch = TryCast(tr.GetObject(prRes, OpenMode.ForRead), Hatch)
            If hatch IsNot Nothing Then
                ' Dim btr As BlockTableRecord = TryCast(tr.GetObject(hatch.OwnerId, OpenMode.ForWrite), BlockTableRecord)
                ' If btr Is Nothing Then
                'Return
                'End If
                Dim plane As Plane = hatch.GetPlane()
                Dim nLoops As Integer = hatch.NumberOfLoops
                For i As Integer = 0 To nLoops - 1
                    Dim [loop] As HatchLoop = hatch.GetLoopAt(i)
                    If [loop].IsPolyline Then
                        Using poly As New Polyline()
                            Dim iVertex As Integer = 0
                            For Each bv As BulgeVertex In [loop].Polyline
                                poly.AddVertexAt(System.Math.Max(System.Threading.Interlocked.Increment(iVertex), iVertex - 1), bv.Vertex, bv.Bulge, 0.0, 0.0)
                                p3ds.Add(New Point3d(bv.Vertex.X, bv.Vertex.Y, 0))
                            Next
                            ' btr.AppendEntity(poly)
                            '  tr.AddNewlyCreatedDBObject(poly, True)
                        End Using
                    Else
                        For Each cv As Curve2d In [loop].Curves
                            Dim line2d As LineSegment2d = TryCast(cv, LineSegment2d)
                            Dim arc2d As CircularArc2d = TryCast(cv, CircularArc2d)
                            Dim ellipse2d As EllipticalArc2d = TryCast(cv, EllipticalArc2d)
                            Dim spline2d As NurbCurve2d = TryCast(cv, NurbCurve2d)
                            If line2d IsNot Nothing Then

                                Using ent As New Line()
                                    p3ds.Add(New Point3d(plane, line2d.StartPoint))
                                    'ent.EndPoint = New Point3d(plane, line2d.EndPoint)
                                    ' btr.AppendEntity(ent)
                                    ' tr.AddNewlyCreatedDBObject(ent, True)
                                End Using
                            ElseIf arc2d IsNot Nothing Then
                                If Math.Abs(arc2d.StartAngle - arc2d.EndAngle) < 0.00001 Then
                                    'Using ent As New Circle(New Point3d(plane, arc2d.Center), plane.Normal, arc2d.Radius)
                                    '    ' btr.AppendEntity(ent)
                                    '    ' tr.AddNewlyCreatedDBObject(ent, True)
                                    'End Using
                                Else
                                    Dim angle As Double = New Vector3d(plane, arc2d.ReferenceVector).AngleOnPlane(plane)
                                    'Using ent As New Arc(New Point3d(plane, arc2d.Center), arc2d.Radius, arc2d.StartAngle + angle, arc2d.EndAngle + angle)
                                    '    ' btr.AppendEntity(ent)
                                    '    ' tr.AddNewlyCreatedDBObject(ent, True)
                                    'End Using
                                End If
                            ElseIf ellipse2d IsNot Nothing Then
                                '-------------------------------------------------------------------------------------------
                                ' Bug: Can not assign StartParam and EndParam of Ellipse:
                                ' Ellipse ent = new Ellipse(new Point3d(plane, e2d.Center), plane.Normal, 
                                '      new Vector3d(plane,e2d.MajorAxis) * e2d.MajorRadius,
                                '      e2d.MinorRadius / e2d.MajorRadius, e2d.StartAngle, e2d.EndAngle);
                                ' ent.StartParam = e2d.StartAngle; 
                                ' ent.EndParam = e2d.EndAngle;
                                ' error CS0200: Property or indexer 'Autodesk.AutoCAD.DatabaseServices.Curve.StartParam' cannot be assigned to -- it is read only
                                ' error CS0200: Property or indexer 'Autodesk.AutoCAD.DatabaseServices.Curve.EndParam' cannot be assigned to -- it is read only
                                '---------------------------------------------------------------------------------------------
                                ' Workaround is using Reflection
                                ' 
                                Using ent As New Ellipse(New Point3d(plane, ellipse2d.Center), plane.Normal, New Vector3d(plane, ellipse2d.MajorAxis) * ellipse2d.MajorRadius, ellipse2d.MinorRadius / ellipse2d.MajorRadius, ellipse2d.StartAngle, ellipse2d.EndAngle)
                                    'ent.[GetType]().InvokeMember("StartParam", BindingFlags.SetProperty, Nothing, ent, New Object() {ellipse2d.StartAngle})
                                    ' ent.[GetType]().InvokeMember("EndParam", BindingFlags.SetProperty, Nothing, ent, New Object() {ellipse2d.EndAngle})


                                    'btr.AppendEntity(ent)
                                    'tr.AddNewlyCreatedDBObject(ent, True)

                                End Using
                            ElseIf spline2d IsNot Nothing Then
                                If spline2d.HasFitData Then
                                    Dim n2fd As NurbCurve2dFitData = spline2d.FitData

                                    For Each p As Point2d In n2fd.FitPoints
                                        p3ds.Add(New Point3d(plane, p))
                                    Next
                                    ' n2fd.KnotParam, 
                                    'Using ent As New Spline(p3ds, New Vector3d(plane, n2fd.StartTangent), New Vector3d(plane, n2fd.EndTangent), n2fd.Degree, n2fd.FitTolerance.EqualPoint)
                                    'btr.AppendEntity(ent)
                                    ' tr.AddNewlyCreatedDBObject(ent, True)
                                    'End Using

                                Else
                                    Dim n2fd As NurbCurve2dData = spline2d.DefinitionData

                                    Dim knots As New DoubleCollection(n2fd.Knots.Count)
                                    For Each p As Point2d In n2fd.ControlPoints
                                        p3ds.Add(New Point3d(plane, p))
                                    Next
                                    For Each k As Double In n2fd.Knots
                                        knots.Add(k)
                                    Next
                                    Dim period As Double = 0
                                    'Using ent As New Spline(n2fd.Degree, n2fd.Rational, spline2d.IsClosed(), spline2d.IsPeriodic(period), p3ds, knots, _
                                    ' n2fd.Weights, n2fd.Knots.Tolerance, n2fd.Knots.Tolerance)
                                    '    btr.AppendEntity(ent)
                                    '    tr.AddNewlyCreatedDBObject(ent, True)
                                    'End Using

                                End If
                            End If
                        Next
                    End If
                Next
            End If
            tr.Commit()
            Return p3ds
        End Using

    End Function

    Public Sub Zoom(ByVal pMin As Point3d,
                    ByVal pMax As Point3d,
                    ByVal pCenter As Point3d,
                    ByVal dFactor As Double)

        ' Obtener el documento y la base de datos del dibujo activos
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        Dim nCurVport As Integer = System.Convert.ToInt32(Application.GetSystemVariable("CVPORT"))

        ' Obtener la extensión del espacio activo si no se han especificado
        ' puntos o solo se ha especificado un punto central.
        ' Comprobar si el espacio Modelo es el espacio activo
        If acCurDb.TileMode = True Then
            If pMin.Equals(New Point3d()) = True And pMax.Equals(New Point3d()) = True Then
                pMin = acCurDb.Extmin
                pMax = acCurDb.Extmax

            End If

        Else

            ' Comprobar si el espacio Papel es el espacio activo
            If nCurVport = 1 Then

                If pMin.Equals(New Point3d()) = True And pMax.Equals(New Point3d()) = True Then
                    pMin = acCurDb.Pextmin
                    pMax = acCurDb.Pextmax

                End If

            Else
                ' Obtener la extension del espacio modelo
                If pMin.Equals(New Point3d()) = True And pMax.Equals(New Point3d()) = True Then
                    pMin = acCurDb.Extmin
                    pMax = acCurDb.Extmax
                End If
            End If
        End If

        ' Iniciar la transaccion
        Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()

            ' Obtener la vista actual
            Using acView As ViewTableRecord = acDoc.Editor.GetCurrentView()

                Dim eExtents As Extents3d
                ' Trasladar las coordenadas SCU a SCV 
                Dim matWCS2DCS As Matrix3d
                matWCS2DCS = Matrix3d.PlaneToWorld(acView.ViewDirection)
                matWCS2DCS = Matrix3d.Displacement(acView.Target - Point3d.Origin) * matWCS2DCS
                matWCS2DCS = Matrix3d.Rotation(-acView.ViewTwist, acView.ViewDirection, acView.Target) * matWCS2DCS

                ' Si se ha especificado un punto central:
                ' Definir los puntos mínimos y máximos de la extension
                ' para los modos Centro y escala
                If pCenter.DistanceTo(Point3d.Origin) <> 0 Then
                    pMin = New Point3d(pCenter.X - (acView.Width / 2), pCenter.Y - (acView.Height / 2), 0)
                    pMax = New Point3d((acView.Width / 2) + pCenter.X, (acView.Height / 2) + pCenter.Y, 0)

                End If

                ' Crear un objeto
                Using acLine As Line = New Line(pMin, pMax)
                    eExtents = New Extents3d(acLine.Bounds.Value.MinPoint, acLine.Bounds.Value.MaxPoint)
                End Using

                ' Calcular el ratio entre el ancho y la altura de la vista actual
                Dim dViewRatio As Double

                dViewRatio = (acView.Width / acView.Height)

                ' Transformar la extension de la vista
                matWCS2DCS = matWCS2DCS.Inverse()

                eExtents.TransformBy(matWCS2DCS)

                Dim dWidth As Double

                Dim dHeight As Double

                Dim pNewCentPt As Point2d

                ' Comprobar si ha especificado un punto central (modos Centro y Escala)
                If pCenter.DistanceTo(Point3d.Origin) <> 0 Then

                    dWidth = acView.Width

                    dHeight = acView.Height

                    If dFactor = 0 Then

                        pCenter = pCenter.TransformBy(matWCS2DCS)

                    End If

                    pNewCentPt = New Point2d(pCenter.X, pCenter.Y)

                Else ' Modos Ventana, Extension y Limites

                    ' Calcular el nuevo ancho y altura de la vista actual
                    dWidth = eExtents.MaxPoint.X - eExtents.MinPoint.X

                    dHeight = eExtents.MaxPoint.Y - eExtents.MinPoint.Y

                    ' Obtener el centro de la vista
                    pNewCentPt = New Point2d(((eExtents.MaxPoint.X + eExtents.MinPoint.X) * 0.5),
                                             ((eExtents.MaxPoint.Y + eExtents.MinPoint.Y) * 0.5))

                End If

                ' Comprobar si el nuevo ancho encaja en la vista
                If dWidth > (dHeight * dViewRatio) Then dHeight = dWidth / dViewRatio

                ' Modificar el tamaño y escala de la vista
                If dFactor <> 0 Then
                    acView.Height = dHeight * dFactor
                    acView.Width = dWidth * dFactor

                End If
                ' Establecer el centro de la vista
                acView.CenterPoint = pNewCentPt
                ' Establecer la vista actual
                acDoc.Editor.SetCurrentView(acView)

            End Using

            ' Confirmar los cambios
            acTrans.Commit()

        End Using

    End Sub
    Public Sub ZoomLimits()
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database

        Zoom(New Point3d(acCurDb.Limmin.X, acCurDb.Limmin.Y, 0),
             New Point3d(acCurDb.Limmax.X, acCurDb.Limmax.Y, 0),
             New Point3d(), 1)
    End Sub

    Public Sub ZOOM_ENTITY_FUND(ByRef ent_obj As ObjectId, ByRef DELTA As Single)
        ' se puede usar enticada o objeto  ej        Dim acPoly As Polyline
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        Dim ed As Editor = Application.DocumentManager.MdiActiveDocument.Editor

        Dim PTOS2 As Extents3d
        Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
            Dim enty As Entity = TryCast(acTrans.GetObject(ent_obj, OpenMode.ForRead), Entity)
            PTOS2 = enty.Bounds()
            acTrans.Commit()
        End Using

        Dim ext As System.Nullable(Of Extents3d) = PTOS2
        If ext.HasValue Then
            Dim maxPt As Point3d = New Point3d(ext.Value.MaxPoint.X + DELTA, ext.Value.MaxPoint.Y + DELTA, 0)
            Dim minPt As Point3d = New Point3d(ext.Value.MinPoint.X - DELTA, ext.Value.MinPoint.Y - DELTA, 0)

            Zoom(minPt, maxPt, New Point3d(), 1)
            Application.DocumentManager.MdiActiveDocument.Editor.UpdateScreen()
            Application.DocumentManager.MdiActiveDocument.Editor.Regen()
        End If
    End Sub




    Function largo_traslapo(ByRef diam As Integer) As Integer

        largo_traslapo = Nothing
        Select Case diam
            Case 6
                largo_traslapo = 35
            Case 8
                largo_traslapo = 50
            Case 10
                largo_traslapo = 60
            Case 12
                largo_traslapo = 70
            Case 16
                largo_traslapo = 95
            Case 18
                largo_traslapo = 105
            Case 22
                largo_traslapo = 160
            Case 25
                largo_traslapo = 180
            Case 28
                largo_traslapo = 205
            Case 32
                largo_traslapo = 230
            Case 36
                largo_traslapo = 260
        End Select

        Return largo_traslapo
    End Function

    ' crea estilo de linea 
    Public Sub crea_estilo_mlinea()
        Dim doc As Document = Application.DocumentManager.MdiActiveDocument
        Dim editor As Editor = doc.Editor
        Dim db As Database = doc.Database

        Using Tx As Transaction = db.TransactionManager.StartTransaction()
            Dim mlineDic As DBDictionary = DirectCast(Tx.GetObject(db.MLStyleDictionaryId, OpenMode.ForRead), DBDictionary)

            If Not mlineDic.Contains("NHESTRIBO") Then


                mlineDic.UpgradeOpen()

                Dim mlineStyle As New MlineStyle()

                mlineDic.SetAt("NHESTRIBO", mlineStyle)

                Tx.AddNewlyCreatedDBObject(mlineStyle, True)

                mlineStyle.EndAngle = 3.14159 * 0.5
                mlineStyle.StartAngle = 3.14159 * 0.5
                mlineStyle.Name = "NHESTRIBO"


                Dim Color As Autodesk.AutoCAD.Colors.Color


                'Color = Autodesk.AutoCAD.Colors.Color.FromNames("ByLayer", "ByLayer")
                'Color = Autodesk.AutoCAD.Colors.Color.FromNames("Red", "Red")

                Dim element As New MlineStyleElement(0, Color, db.Celtype)
                mlineStyle.Elements.Add(element, True)

                element = New MlineStyleElement(-15, Color, db.Celtype)
                mlineStyle.Elements.Add(element, False)
                element = New MlineStyleElement(-30, Color, db.Celtype)
                mlineStyle.Elements.Add(element, True)
                element = New MlineStyleElement(-45, Color, db.Celtype)
                mlineStyle.Elements.Add(element, False)
                element = New MlineStyleElement(-60, Color, db.Celtype)
                mlineStyle.Elements.Add(element, False)

                mlineStyle.StartInnerArcs = False
                mlineStyle.EndInnerArcs = False

            End If



            Tx.Commit()
        End Using

    End Sub


    Function datos_lines(ByVal x1 As Single, ByVal y1 As Single, ByVal x2 As Single, ByVal y2 As Single, ByVal xref As Single, ByVal yref As Single) As Single


        Dim mm As Single = 0.0
        Dim yy As Single = 0.0

        If x2 = x1 Then

            yy = yref
        Else
            mm = (y2 - y1) / (x2 - x1)
            yy = mm * (xref - x1) + y1
        End If


        Return yy
    End Function

    'funcion en vb.net que busque la interseccion perpendicular del punto1 en la linea formada por los punto2 y punto3.
    'y devuelva el punto intersectado
    Public Function InterseccionPerpendicular(punto1 As Point3d, punto2 As Point3d, punto3 As Point3d) As Point3d
        ' Si punto2 y punto3 son el mismo punto
        If punto2.X = punto3.X AndAlso punto2.Y = punto3.Y Then
            Return punto2 ' O punto3, son el mismo punto
        End If

        ' Si la línea es vertical
        If punto2.X = punto3.X Then
            Return New Point3d(punto2.X, punto1.Y, punto1.Z)
        End If

        ' Si la línea es horizontal
        If punto2.Y = punto3.Y Then
            Return New Point3d(punto1.X, punto2.Y, punto1.Z)
        End If

        ' Calcula la pendiente de la línea punto2-punto3
        Dim m As Double = (punto3.Y - punto2.Y) / (punto3.X - punto2.X)

        ' Pendiente de la línea perpendicular es el negativo inverso
        Dim mPerpendicular As Double = -1 / m

        ' Usando y = mx + b, calculamos b para la línea perpendicular
        Dim bPerpendicular As Double = punto1.Y - mPerpendicular * punto1.X

        ' Usando y = mx + b, calculamos b para la línea punto2-punto3
        Dim b As Double = punto2.Y - m * punto2.X

        ' Encuentra el punto de intersección resolviendo las ecuaciones de ambas líneas
        Dim xIntersect As Double = (bPerpendicular - b) / (m - mPerpendicular)
        Dim yIntersect As Double = m * xIntersect + b

        Return New Point3d(xIntersect, yIntersect, punto1.Z)
    End Function


    'La función InterseccionPerpendicular calcula y devuelve el punto de intersección de una línea perpendicular trazada desde punto1 a la línea formada por punto2 y punto3.
    Function largo_ptos(ByRef Pt1 As Point2d, ByRef Pt2 As Point2d) As Single

        Dim largo As Single = (Abs(Pt1.X - Pt2.X) ^ 2 + Abs(Pt1.Y - Pt2.Y) ^ 2) ^ 0.5
        Return largo
    End Function
    Function TIPO_caso_BARRA_LOSA_ind_fund(ByRef lista As Polyline, ByVal tipo_referencia As String, ByVal datos_barra As String()) As String()

        ' si barra es completamente horiozntas estonces son casos 200  ( coordenadas iniciales a la izq)
        ' si tiene alguna inclinacion  es caso 100  , donde "Y" mayor es el pto cero o inicial ( coordenadas iniciales con coordenada y mas altai)


        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        Dim ed As Editor = Application.DocumentManager.MdiActiveDocument.Editor

        Dim cont As Integer = lista.NumberOfVertices
        Dim cont2 As Integer = 0
        Dim largo(10) As String
        Dim factor_doble As Integer = 0

        Using acTrans2 As Transaction = acCurDb.TransactionManager.StartTransaction()

            ' ENTREGA UN COLECCION DE LOS ELEMTOS QUE PERTENECEN A UN DETERMINADO GRUPO, BUSCA EL NOMBRE DEL GRUPO CON ObjectID
            Dim GRUPO_ As New CODIGOS_GRUPOS()
            Dim acObjId_grup As ObjectId() = GRUPO_.buscar_grupo(lista.ObjectId)

            If Not IsNothing(acObjId_grup) Then



                For Each idObj_ As ObjectId In acObjId_grup
                    ' Application.ShowAlertDialog("idObj_.ObjectClass.DxfName.ToString() : " & idObj_.ObjectClass.DxfName.ToString())
                    If idObj_.ObjectClass.DxfName.ToString = "LWPOLYLINE" Then
                        factor_doble = factor_doble + 1
                        If lista.ObjectId <> idObj_ Then
                            Dim ent1 As Polyline = TryCast(acTrans2.GetObject(idObj_, OpenMode.ForWrite), Polyline)
                            cont2 = lista.NumberOfVertices
                        End If
                    End If
                Next
            End If
        End Using



        ' largo(4) :  coordenada 2,3 _  4,5
        Select Case cont
            Case 2
                Dim pto1 As Point2d = lista.GetPoint2dAt(0)
                Dim pto2 As Point2d = lista.GetPoint2dAt(1)
                largo(0) = "f3"
                If factor_doble = 1 Then
                    largo(0) = "f3"
                ElseIf cont = 2 And cont2 = 2 Then
                    largo(0) = "f16"
                ElseIf (cont = 2 And cont2 = 3) Or (cont = 3 And cont2 = 2) Then
                    largo(0) = "f17"
                End If

                If Abs(pto1.X - pto2.X) < 0.1 Then 'caso F3  vertical
                    If pto1.Y > pto2.Y Then
                        largo(1) = "AZ"
                        largo(4) = pto1.X & "," & pto1.Y & "_" & pto2.X & "," & pto2.Y
                    Else
                        largo(1) = "ZA"
                        largo(4) = pto2.X & "," & pto2.Y & "_" & pto1.X & "," & pto1.Y
                    End If

                    If largo(0) = "f16" Or largo(0) = "f17" Then
                        largo(2) = datos_barra(9)
                    Else
                        largo(2) = "vertical_b"
                    End If
                Else 'f3  horizontal o inclinada


                    If pto1.X > pto2.X Then

                        largo(1) = "AZ"
                        largo(4) = pto1.X & "," & pto1.Y & "_" & pto2.X & "," & pto2.Y
                    Else
                        largo(1) = "ZA"
                        largo(4) = pto2.X & "," & pto2.Y & "_" & pto1.X & "," & pto1.Y

                    End If
                    If largo(0) = "f16" Or largo(0) = "f17" Then
                        largo(2) = datos_barra(9)
                    Else
                        largo(2) = "horizontal_i"
                    End If

                End If
                largo(3) = (pto1.X + pto2.X) / 2 & "," & (pto1.Y + pto2.Y) / 2
                largo(5) = Round(Round(pto1.GetDistanceTo(pto2), 0))
                largo(6) = ""
                largo(7) = 0
                largo(8) = 0
                largo(9) = 0


            Case 3 ' f11  y f18

                Dim aux_horizontal As Boolean = False
                Dim pto0 As Point2d = lista.GetPoint2dAt(0)
                Dim pto1 As Point2d = lista.GetPoint2dAt(1)
                Dim pto2 As Point2d = lista.GetPoint2dAt(2)

                largo(0) = "f11"

                If pto0.GetDistanceTo(pto1) < pto2.GetDistanceTo(pto1) Then


                    If Abs(pto1.X - pto2.X) < 0.1 Then
                        ' VERTICAL
                        If pto1.Y > pto2.Y Then
                            largo(4) = pto1.X & "," & pto1.Y & "_" & pto2.X & "," & pto2.Y
                            largo(2) = "vertical_a"
                            largo(1) = "AZ"
                            largo(6) = "(" & Round(pto2.GetDistanceTo(pto1), 0) & "+" & Round(pto0.GetDistanceTo(pto1)) & ")"
                            largo(7) = 0
                            largo(8) = 0
                            largo(9) = 0




                        Else
                            largo(4) = pto2.X & "," & pto2.Y & "_" & pto1.X & "," & pto1.Y
                            largo(2) = "vertical_b"
                            largo(1) = "ZA"
                            largo(6) = "(" & Round(pto0.GetDistanceTo(pto1)) & "+" & Round(pto2.GetDistanceTo(pto1), 0) & ")"
                            largo(7) = 0
                            largo(8) = 0
                            largo(9) = 0


                        End If

                        If pto0.X > pto1.X And largo(0) = "f11" Then
                            largo(0) = "f9a_V"
                        End If

                    Else
                        ' HORIZONTAL O INCLINADA

                        If pto1.X > pto2.X Then
                            largo(4) = pto1.X & "," & pto1.Y & "_" & pto2.X & "," & pto2.Y
                            largo(2) = "horizontal_d"
                            largo(1) = "AZ"
                            largo(6) = "(" & Round(pto2.GetDistanceTo(pto1), 0) & "+" & Round(pto0.GetDistanceTo(pto1)) & ")"
                            largo(7) = 0
                            largo(8) = 0
                            largo(9) = 0

                        Else
                            largo(4) = pto2.X & "," & pto2.Y & "_" & pto1.X & "," & pto1.Y
                            largo(2) = "horizontal_i"
                            largo(1) = "ZA"
                            largo(6) = "(" & Round(pto0.GetDistanceTo(pto1)) & "+" & Round(pto2.GetDistanceTo(pto1), 0) & ")"
                            largo(7) = 0
                            largo(8) = 0
                            largo(9) = 0
                        End If

                        If pto0.Y < pto1.Y And largo(0) = "f11" Then
                            largo(0) = "f9a_V"
                        End If

                    End If


                Else

                    If Abs(pto1.X - pto0.X) < 0.1 Then
                        ' VERTICAL
                        If pto0.Y > pto1.Y Then
                            largo(4) = pto0.X & "," & pto0.Y & "_" & pto1.X & "," & pto1.Y
                            largo(2) = "vertical_b"
                            largo(1) = "AZ"
                            largo(6) = "(" & Round(pto2.GetDistanceTo(pto1), 0) & "+" & Round(pto0.GetDistanceTo(pto1)) & ")"
                            largo(7) = 0
                            largo(8) = 0
                            largo(9) = 0
                        Else
                            largo(4) = pto1.X & "," & pto1.Y & "_" & pto0.X & "," & pto0.Y
                            largo(2) = "vertical_a"
                            largo(1) = "ZA"
                            largo(6) = "(" & Round(pto0.GetDistanceTo(pto1)) & "+" & Round(pto2.GetDistanceTo(pto1), 0) & ")"
                            largo(7) = 0
                            largo(8) = 0
                            largo(9) = 0
                        End If

                        If pto2.X > pto1.X And largo(0) = "f11" Then
                            largo(0) = "f9a_V"
                        End If
                    Else
                        ' HORIZONTAL O INCLINADA

                        If pto0.X > pto1.X Then
                            largo(4) = pto0.X & "," & pto0.Y & "_" & pto1.X & "," & pto1.Y
                            largo(2) = "horizontal_i"
                            largo(1) = "AZ"
                            largo(6) = "(" & Round(pto2.GetDistanceTo(pto1)) & "+" & Round(pto1.GetDistanceTo(pto2), 0) & ")"
                            largo(7) = 0
                            largo(8) = 0
                            largo(9) = 0
                        Else
                            largo(4) = pto1.X & "," & pto1.Y & "_" & pto0.X & "," & pto0.Y
                            largo(2) = "horizontal_d"
                            largo(1) = "ZA"
                            largo(6) = "(" & Round(pto0.GetDistanceTo(pto1), 0) & "+" & Round(pto1.GetDistanceTo(pto2)) & ")"
                            largo(7) = 0
                            largo(8) = 0
                            largo(9) = 0
                        End If

                        If pto2.Y < pto1.Y And largo(0) = "f11" Then
                            largo(0) = "f9a_V"
                        End If

                    End If

                End If




                largo(3) = (pto1.X + pto2.X) / 2 & "," & (pto1.Y + pto2.Y) / 2

                largo(5) = Round(pto0.GetDistanceTo(pto1) + pto2.GetDistanceTo(pto1), 0)


            Case 4 ' f1  , f10  , f19  y f9a
                Dim pto0 As Point2d = lista.GetPoint2dAt(0)
                Dim pto1 As Point2d = lista.GetPoint2dAt(1)
                Dim pto2 As Point2d = lista.GetPoint2dAt(2)
                Dim pto3 As Point2d = lista.GetPoint2dAt(3)
                Dim aux_horizontal As Boolean = False



                If (pto0.Y > pto1.Y Or pto0.X < pto1.X) Then
                    largo(0) = "f10"
                Else
                    largo(0) = "f9a"
                End If


                If Abs(pto1.X - pto2.X) < 0.1 Then
                    ' VERTICAL
                    If pto1.Y > pto2.Y Then
                        largo(4) = pto1.X & "," & pto1.Y & "_" & pto2.X & "," & pto2.Y
                        largo(2) = "vertical_a"
                        largo(1) = "ZA"
                        largo(6) = "(" & Round(pto3.GetDistanceTo(pto2), 0) & "+" & Round(pto2.GetDistanceTo(pto1), 0) & "+" & Round(pto0.GetDistanceTo(pto1)) & ")"
                        largo(7) = 0
                        largo(8) = 0
                        largo(9) = 0
                    Else
                        largo(4) = pto2.X & "," & pto2.Y & "_" & pto1.X & "," & pto1.Y
                        largo(2) = "vertical_a"
                        largo(1) = "AZ"
                        largo(6) = "(" & Round(pto0.GetDistanceTo(pto1)) & "+" & Round(pto2.GetDistanceTo(pto1), 0) & "+" & Round(pto3.GetDistanceTo(pto2), 0) & ")"
                        largo(7) = 0
                        largo(8) = 0
                        largo(9) = 0
                    End If
                Else
                    ' HORIZONTAL O INCLINADA

                    If pto1.X > pto2.X Then
                        largo(4) = pto1.X & "," & pto1.Y & "_" & pto2.X & "," & pto2.Y
                        largo(2) = "horizontal_i"
                        largo(1) = "AZ"
                        largo(6) = "(" & Round(pto3.GetDistanceTo(pto2), 0) & "+" & Round(pto2.GetDistanceTo(pto1), 0) & "+" & Round(pto0.GetDistanceTo(pto1)) & ")"
                        largo(7) = 0
                        largo(8) = 0
                        largo(9) = 0
                    Else
                        largo(4) = pto2.X & "," & pto2.Y & "_" & pto1.X & "," & pto1.Y
                        largo(2) = "horizontal_i"
                        largo(1) = "ZA"
                        largo(6) = "(" & Round(pto0.GetDistanceTo(pto1)) & "+" & Round(pto2.GetDistanceTo(pto1), 0) & "+" & Round(pto3.GetDistanceTo(pto2), 0) & ")"
                        largo(7) = 0
                        largo(8) = 0
                        largo(9) = 0
                    End If


                End If
                largo(3) = (pto1.X + pto2.X) / 2 & "," & (pto1.Y + pto2.Y) / 2



                largo(5) = Round(pto0.GetDistanceTo(pto1) + pto2.GetDistanceTo(pto1) + pto2.GetDistanceTo(pto3), 0)




            Case Else
                Debug.WriteLine("Not between 1 and 10, inclusive")
        End Select
        Return largo
    End Function

    Function TIPO_caso_BARRA_LOSA_ind_1barra(ByRef lista As Polyline, ByVal tipo_referencia As String, ByVal datos_barra As String()) As String()

        ' si barra es completamente horiozntas estonces son casos 200  ( coordenadas iniciales a la izq)
        ' si tiene alguna inclinacion  es caso 100  , donde "Y" mayor es el pto cero o inicial ( coordenadas iniciales con coordenada y mas altai)


        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        Dim ed As Editor = Application.DocumentManager.MdiActiveDocument.Editor

        Dim cont As Integer = lista.NumberOfVertices
        Dim cont2 As Integer = 0
        Dim largo(10) As String
        Dim factor_doble As Integer = 0

        'Using acTrans2 As Transaction = acCurDb.TransactionManager.StartTransaction()

        '    ' ENTREGA UN COLECCION DE LOS ELEMTOS QUE PERTENECEN A UN DETERMINADO GRUPO, BUSCA EL NOMBRE DEL GRUPO CON ObjectID
        '    Dim GRUPO_ As New CODIGOS_GRUPOS()
        '    Dim acObjId_grup As ObjectId() = GRUPO_.buscar_grupo(lista.ObjectId)
        '    Dim ent As Polyline
        '    If Not IsNothing(acObjId_grup) Then



        '        For Each idObj_ As ObjectId In acObjId_grup
        '            ' Application.ShowAlertDialog("idObj_.ObjectClass.DxfName.ToString() : " & idObj_.ObjectClass.DxfName.ToString())
        '            If idObj_.ObjectClass.DxfName.ToString = "LWPOLYLINE" Then
        '                factor_doble = factor_doble + 1
        '                If lista.ObjectId <> idObj_ Then
        '                    Dim ent1 As Polyline = TryCast(acTrans2.GetObject(idObj_, OpenMode.ForWrite), Polyline)
        '                    cont2 = lista.NumberOfVertices
        '                End If
        '            End If
        '        Next
        '    End If
        'End Using

        factor_doble = 1

        ' largo(4) :  coordenada 2,3 _  4,5
        Select Case cont
            Case 2
                Dim pto1 As Point2d = lista.GetPoint2dAt(0)
                Dim pto2 As Point2d = lista.GetPoint2dAt(1)

                If factor_doble = 1 Then
                    largo(0) = "f3"
                ElseIf cont = 2 And cont2 = 2 Then
                    largo(0) = "f16"
                ElseIf (cont = 2 And cont2 = 3) Or (cont = 3 And cont2 = 2) Then
                    largo(0) = "f17"
                End If

                If Abs(pto1.X - pto2.X) < 0.1 Then 'caso F3  vertical
                    If pto1.Y > pto2.Y Then
                        largo(1) = "AZ"
                        largo(4) = pto1.X & "," & pto1.Y & "_" & pto2.X & "," & pto2.Y
                    Else
                        largo(1) = "ZA"
                        largo(4) = pto2.X & "," & pto2.Y & "_" & pto1.X & "," & pto1.Y
                    End If

                    If largo(0) = "f16" Or largo(0) = "f17" Then
                        largo(2) = datos_barra(9)
                    Else
                        largo(2) = "vertical_b"
                    End If
                Else 'f3  horizontal o inclinada


                    If pto1.X > pto2.X Then

                        largo(1) = "AZ"
                        largo(4) = pto1.X & "," & pto1.Y & "_" & pto2.X & "," & pto2.Y
                    Else
                        largo(1) = "ZA"
                        largo(4) = pto2.X & "," & pto2.Y & "_" & pto1.X & "," & pto1.Y

                    End If
                    If largo(0) = "f16" Or largo(0) = "f17" Then
                        largo(2) = datos_barra(9)
                    Else
                        largo(2) = "horizontal_i"
                    End If

                End If
                largo(3) = (pto1.X + pto2.X) / 2 & "," & (pto1.Y + pto2.Y) / 2
                largo(5) = pto1.GetDistanceTo(pto2)
                largo(6) = 0
                largo(7) = 0
                largo(8) = 0
                largo(9) = 0


            Case 3 ' f11  y f18

                Dim aux_horizontal As Boolean = False
                Dim pto0 As Point2d = lista.GetPoint2dAt(0)
                Dim pto1 As Point2d = lista.GetPoint2dAt(1)
                Dim pto2 As Point2d = lista.GetPoint2dAt(2)
                If factor_doble = 1 Then
                    largo(0) = "f11"
                ElseIf cont = 2 And cont2 = 2 Then
                    largo(0) = "f16"
                ElseIf (cont = 2 And cont2 = 3) Or (cont = 3 And cont2 = 2) Then
                    largo(0) = "f17"
                ElseIf (cont = 3 And cont2 = 3) Or (cont = 3 And cont2 = 3) Then
                    largo(0) = "f18"
                End If


                If pto0.GetDistanceTo(pto1) < pto2.GetDistanceTo(pto1) Then


                    If Abs(pto1.X - pto2.X) < 0.1 Then
                        ' VERTICAL
                        If pto1.Y > pto2.Y Then
                            largo(4) = pto1.X & "," & pto1.Y & "_" & pto2.X & "," & pto2.Y
                            largo(2) = "vertical_a"
                            largo(1) = "AZ"
                            largo(6) = 0
                            largo(7) = 0
                            largo(8) = 0
                            largo(9) = pto0.GetDistanceTo(pto1)




                        Else
                            largo(4) = pto2.X & "," & pto2.Y & "_" & pto1.X & "," & pto1.Y
                            largo(2) = "vertical_b"
                            largo(1) = "ZA"
                            largo(6) = 0
                            largo(7) = 0
                            largo(8) = pto0.GetDistanceTo(pto1)
                            largo(9) = 0


                        End If

                        If pto0.X > pto1.X And largo(0) = "f11" Then
                            largo(0) = "f9a_V"
                        End If

                    Else
                        ' HORIZONTAL O INCLINADA

                        If pto1.X > pto2.X Then
                            largo(4) = pto1.X & "," & pto1.Y & "_" & pto2.X & "," & pto2.Y
                            largo(2) = "horizontal_d"
                            largo(1) = "AZ"
                            largo(6) = 0
                            largo(7) = 0
                            largo(8) = 0
                            largo(9) = pto0.GetDistanceTo(pto1)

                        Else
                            largo(4) = pto2.X & "," & pto2.Y & "_" & pto1.X & "," & pto1.Y
                            largo(2) = "horizontal_i"
                            largo(1) = "ZA"
                            largo(6) = 0
                            largo(7) = 0
                            largo(8) = pto0.GetDistanceTo(pto1)
                            largo(9) = 0
                        End If

                        If pto0.Y < pto1.Y And largo(0) = "f11" Then
                            largo(0) = "f9a_V"
                        End If

                    End If
                    largo(5) = pto1.GetDistanceTo(pto2)

                Else

                    If Abs(pto1.X - pto0.X) < 0.1 Then
                        ' VERTICAL
                        If pto0.Y > pto1.Y Then
                            largo(4) = pto0.X & "," & pto0.Y & "_" & pto1.X & "," & pto1.Y
                            largo(2) = "vertical_b"
                            largo(1) = "AZ"
                            largo(6) = 0
                            largo(7) = 0
                            largo(8) = pto1.GetDistanceTo(pto2)
                            largo(9) = 0
                        Else
                            largo(4) = pto1.X & "," & pto1.Y & "_" & pto0.X & "," & pto0.Y
                            largo(2) = "vertical_a"
                            largo(1) = "ZA"
                            largo(6) = 0
                            largo(7) = 0
                            largo(8) = 0
                            largo(9) = pto1.GetDistanceTo(pto2)
                        End If

                        If pto2.X > pto1.X And largo(0) = "f11" Then
                            largo(0) = "f9a_V"
                        End If
                    Else
                        ' HORIZONTAL O INCLINADA

                        If pto0.X > pto1.X Then
                            largo(4) = pto0.X & "," & pto0.Y & "_" & pto1.X & "," & pto1.Y
                            largo(2) = "horizontal_i"
                            largo(1) = "AZ"
                            largo(6) = 0
                            largo(7) = 0
                            largo(8) = pto1.GetDistanceTo(pto2)
                            largo(9) = 0
                        Else
                            largo(4) = pto1.X & "," & pto1.Y & "_" & pto0.X & "," & pto0.Y
                            largo(2) = "horizontal_d"
                            largo(1) = "ZA"
                            largo(6) = 0
                            largo(7) = 0
                            largo(8) = 0
                            largo(9) = pto1.GetDistanceTo(pto2)
                        End If

                        If pto2.Y < pto1.Y And largo(0) = "f11" Then
                            largo(0) = "f9a_V"
                        End If

                    End If

                    largo(5) = pto0.GetDistanceTo(pto1)
                End If

                If largo(0) = "f16" Or largo(0) = "f17" Or largo(0) = "f18" Or largo(0) = "f9a_V" Then
                    largo(2) = datos_barra(9)
                End If


                largo(3) = (pto1.X + pto2.X) / 2 & "," & (pto1.Y + pto2.Y) / 2



            Case 4 ' f1  , f10  , f19  y f9a
                Dim pto0 As Point2d = lista.GetPoint2dAt(0)
                Dim pto1 As Point2d = lista.GetPoint2dAt(1)
                Dim pto2 As Point2d = lista.GetPoint2dAt(2)
                Dim pto3 As Point2d = lista.GetPoint2dAt(3)
                Dim aux_horizontal As Boolean = False



                If (pto0.GetDistanceTo(pto1) < pto2.GetDistanceTo(pto3) And pto2.GetDistanceTo(pto1) < pto2.GetDistanceTo(pto3)) Then 'f1  f19

                    If factor_doble = 1 Then
                        largo(0) = "f1"
                    Else
                        largo(0) = "f19"
                    End If

                    If Abs(pto3.X - pto2.X) < 0.1 Then
                        ' VERTICAL
                        If pto2.Y > pto3.Y Then
                            largo(4) = pto2.X & "," & pto2.Y & "_" & pto3.X & "," & pto3.Y
                            largo(2) = "vertical_a"
                            largo(1) = "AZ"
                            largo(6) = 0
                            largo(7) = pto0.GetDistanceTo(pto1)
                            largo(8) = 0
                            largo(9) = pto1.GetDistanceTo(pto2)
                        Else
                            largo(4) = pto3.X & "," & pto3.Y & "_" & pto2.X & "," & pto2.Y
                            largo(2) = "vertical_b"
                            largo(1) = "ZA"
                            largo(6) = pto0.GetDistanceTo(pto1)
                            largo(7) = 0
                            largo(8) = pto1.GetDistanceTo(pto2)
                            largo(9) = 0
                        End If
                    Else
                        ' HORIZONTAL O INCLINADA

                        If pto2.X > pto3.X Then
                            largo(4) = pto2.X & "," & pto2.Y & "_" & pto3.X & "," & pto3.Y
                            largo(2) = "horizontal_d"
                            largo(1) = "AZ"
                            largo(6) = 0
                            largo(7) = pto0.GetDistanceTo(pto1)
                            largo(8) = 0
                            largo(9) = pto1.GetDistanceTo(pto2)
                        Else
                            largo(4) = pto3.X & "," & pto3.Y & "_" & pto2.X & "," & pto2.Y
                            largo(2) = "horizontal_i"
                            largo(1) = "ZA"
                            largo(6) = pto0.GetDistanceTo(pto1)
                            largo(7) = 0
                            largo(8) = pto1.GetDistanceTo(pto2)
                            largo(9) = 0
                        End If


                    End If
                    largo(3) = (pto3.X + pto2.X) / 2 & "," & (pto3.Y + pto2.Y) / 2

                    largo(5) = pto3.GetDistanceTo(pto2)

                ElseIf (pto3.GetDistanceTo(pto2) < pto0.GetDistanceTo(pto1) And pto2.GetDistanceTo(pto1) < pto0.GetDistanceTo(pto1)) Then 'f1  f19
                    If factor_doble = 1 Then
                        largo(0) = "f1"
                    Else
                        largo(0) = "f19"
                    End If

                    If Abs(pto1.X - pto0.X) < 0.1 Then
                        ' VERTICAL
                        If pto0.Y > pto1.Y Then
                            largo(4) = pto0.X & "," & pto0.Y & "_" & pto1.X & "," & pto1.Y
                            largo(2) = "vertical_b"
                            largo(1) = "AZ"
                            largo(6) = pto2.GetDistanceTo(pto3)
                            largo(7) = 0
                            largo(8) = pto1.GetDistanceTo(pto2)
                            largo(9) = 0
                        Else
                            largo(4) = pto1.X & "," & pto1.Y & "_" & pto0.X & "," & pto0.Y
                            largo(2) = "vertical_a"
                            largo(1) = "ZA"
                            largo(6) = 0
                            largo(7) = pto2.GetDistanceTo(pto3)
                            largo(8) = 0
                            largo(9) = pto1.GetDistanceTo(pto2)
                        End If
                    Else
                        ' HORIZONTAL O INCLINADA

                        If pto0.X > pto1.X Then
                            largo(4) = pto0.X & "," & pto0.Y & "_" & pto1.X & "," & pto1.Y
                            largo(2) = "horizontal_i"
                            largo(1) = "AZ"
                            largo(6) = pto2.GetDistanceTo(pto3)
                            largo(7) = 0
                            largo(8) = pto1.GetDistanceTo(pto2)
                            largo(9) = 0
                        Else
                            largo(4) = pto1.X & "," & pto1.Y & "_" & pto0.X & "," & pto0.Y
                            largo(2) = "horizontal_d"
                            largo(1) = "ZA"
                            largo(6) = 0
                            largo(7) = pto2.GetDistanceTo(pto3)
                            largo(8) = 0
                            largo(9) = pto1.GetDistanceTo(pto2)
                        End If
                    End If
                    largo(3) = (pto1.X + pto0.X) / 2 & "," & (pto1.Y + pto0.Y) / 2
                    largo(5) = pto1.GetDistanceTo(pto0)

                ElseIf (pto3.GetDistanceTo(pto2) < pto1.GetDistanceTo(pto2) And pto0.GetDistanceTo(pto1) < pto1.GetDistanceTo(pto2)) Then ' f10

                    If (pto0.Y > pto1.Y Or pto0.X < pto1.X) Then
                        largo(0) = "f10"
                    Else
                        largo(0) = "f9a"
                    End If


                    If Abs(pto1.X - pto2.X) < 0.1 Then
                        ' VERTICAL
                        If pto1.Y > pto2.Y Then
                            largo(4) = pto2.X & "," & pto2.Y & "_" & pto1.X & "," & pto1.Y
                            largo(2) = "vertical_a"
                            largo(1) = "ZA"
                            largo(6) = 0
                            largo(7) = 0
                            largo(8) = pto0.GetDistanceTo(pto1)
                            largo(9) = pto2.GetDistanceTo(pto3)
                        Else
                            largo(4) = pto1.X & "," & pto1.Y & "_" & pto2.X & "," & pto2.Y
                            largo(2) = "vertical_a"
                            largo(1) = "AZ"
                            largo(6) = 0
                            largo(7) = 0
                            largo(8) = pto2.GetDistanceTo(pto3)
                            largo(9) = pto0.GetDistanceTo(pto1)
                        End If
                    Else
                        ' HORIZONTAL O INCLINADA

                        If pto1.X > pto2.X Then
                            largo(4) = pto1.X & "," & pto1.Y & "_" & pto2.X & "," & pto2.Y
                            largo(2) = "horizontal_i"
                            largo(1) = "AZ"
                            largo(6) = 0
                            largo(7) = 0
                            largo(8) = pto2.GetDistanceTo(pto3)
                            largo(9) = pto0.GetDistanceTo(pto1)
                        Else
                            largo(4) = pto2.X & "," & pto2.Y & "_" & pto1.X & "," & pto1.Y
                            largo(2) = "horizontal_i"
                            largo(1) = "ZA"
                            largo(6) = 0
                            largo(7) = 0
                            largo(8) = pto0.GetDistanceTo(pto1)
                            largo(9) = pto2.GetDistanceTo(pto3)
                        End If


                    End If
                    largo(3) = (pto1.X + pto2.X) / 2 & "," & (pto1.Y + pto2.Y) / 2
                    largo(5) = pto1.GetDistanceTo(pto2)

                End If





            Case 5
                Dim pto0 As Point2d = lista.GetPoint2dAt(0)
                Dim pto1 As Point2d = lista.GetPoint2dAt(1)
                Dim pto2 As Point2d = lista.GetPoint2dAt(2)
                Dim pto3 As Point2d = lista.GetPoint2dAt(3)
                Dim pto4 As Point2d = lista.GetPoint2dAt(4)



                If (pto0.GetDistanceTo(pto1) < pto2.GetDistanceTo(pto3) And pto2.GetDistanceTo(pto1) < pto2.GetDistanceTo(pto3)) Then 'f7 , s3


                    If pto1.GetDistanceTo(pto2) <> pto3.GetDistanceTo(pto4) Then
                        largo(0) = "f3"
                    Else
                        largo(0) = "s7"
                    End If



                    If Abs(pto3.X - pto2.X) < 0.1 Then
                        ' VERTICAL
                        If pto2.Y > pto3.Y Then
                            largo(4) = pto2.X & "," & pto2.Y & "_" & pto3.X & "," & pto3.Y
                            largo(2) = "vertical_a"
                            largo(1) = "ZA"
                            If largo(0) = "s3" Then largo(0) = "s3b"
                        Else
                            largo(4) = pto3.X & "," & pto3.Y & "_" & pto2.X & "," & pto2.Y
                            largo(2) = "vertical_b"
                            largo(1) = "AZ"
                            If largo(0) = "s3" Then largo(0) = "s3a"
                        End If
                    Else
                        ' HORIZONTAL O INCLINADA

                        If pto2.X > pto3.X Then
                            largo(4) = pto2.X & "," & pto2.Y & "_" & pto3.X & "," & pto3.Y
                            largo(2) = "horizontal_d"
                            largo(1) = "ZA"
                            If largo(0) = "s3" Then largo(0) = "s3b"
                        Else
                            largo(4) = pto3.X & "," & pto3.Y & "_" & pto2.X & "," & pto2.Y
                            largo(2) = "horizontal_i"
                            largo(1) = "AZ"
                            If largo(0) = "s3" Then largo(0) = "s3a"
                        End If


                    End If
                    largo(6) = pto0.GetDistanceTo(pto1)
                    largo(7) = 0
                    largo(8) = pto1.GetDistanceTo(pto2)
                    largo(9) = pto3.GetDistanceTo(pto4)

                    largo(3) = (pto3.X + pto2.X) / 2 & "," & (pto3.Y + pto2.Y) / 2
                    largo(5) = pto3.GetDistanceTo(pto2)

                ElseIf (pto4.GetDistanceTo(pto3) < pto2.GetDistanceTo(pto1) And pto3.GetDistanceTo(pto2) < pto2.GetDistanceTo(pto1)) Then 'f7 , s3
                    If Abs(pto0.GetDistanceTo(pto1) - pto3.GetDistanceTo(pto2)) < 0.5 Then
                        largo(0) = "f7"
                    ElseIf InStr(1, tipo_referencia, "s") <> "0" Then
                        largo(0) = "s3"
                    Else
                        MsgBox("Posible error, asignando barra s3 sin referencia inicial 's'.", vbCritical)
                        largo(0) = "s3"
                    End If



                    If Abs(pto1.X - pto2.X) < 0.1 Then
                        ' VERTICAL
                        If pto1.Y > pto2.Y Then
                            largo(4) = pto1.X & "," & pto1.Y & "_" & pto2.X & "," & pto2.Y
                            largo(2) = "vertical_b"
                            largo(1) = "AZ"
                            If largo(0) = "s3" Then largo(0) = "s3a"
                        Else
                            largo(4) = pto2.X & "," & pto1.Y & "_" & pto1.X & "," & pto1.Y
                            largo(2) = "vertical_a"
                            largo(1) = "ZA"
                            If largo(0) = "s3" Then largo(0) = "s3b"
                        End If
                    Else
                        ' HORIZONTAL O INCLINADA

                        If pto1.X > pto2.X Then
                            largo(4) = pto1.X & "," & pto1.Y & "_" & pto2.X & "," & pto2.Y
                            largo(2) = "horizontal_i"
                            largo(1) = "AZ"
                            If largo(0) = "s3" Then largo(0) = "s3a"
                        Else
                            largo(4) = pto2.X & "," & pto2.Y & "_" & pto1.X & "," & pto1.Y
                            largo(2) = "horizontal_d"
                            largo(1) = "ZA"
                            If largo(0) = "s3" Then largo(0) = "s3b"
                        End If


                    End If
                    largo(6) = pto3.GetDistanceTo(pto4)
                    largo(7) = 0
                    largo(8) = pto2.GetDistanceTo(pto3)
                    largo(9) = pto0.GetDistanceTo(pto1)

                    largo(3) = (pto1.X + pto2.X) / 2 & "," & (pto1.Y + pto2.Y) / 2
                    largo(5) = pto1.GetDistanceTo(pto2)
                End If
            Case 6  'f9 f4
                Dim pto0 As Point2d = lista.GetPoint2dAt(0)
                Dim pto1 As Point2d = lista.GetPoint2dAt(1)
                Dim pto2 As Point2d = lista.GetPoint2dAt(2)
                Dim pto3 As Point2d = lista.GetPoint2dAt(3)
                Dim pto4 As Point2d = lista.GetPoint2dAt(4)
                Dim pto5 As Point2d = lista.GetPoint2dAt(5)



                If (pto1.Y > pto2.Y Or pto1.X < pto2.X) Then
                    largo(0) = "f4"
                Else
                    If InStr(1, tipo_referencia, "s") <> "0" Then
                        largo(0) = "s1"
                    Else
                        largo(0) = "f9"
                    End If

                End If


                If Abs(pto2.X - pto3.X) < 0.1 Then
                    ' VERTICAL
                    If pto2.Y > pto3.Y Then
                        largo(4) = pto2.X & "," & pto2.Y & "_" & pto3.X & "," & pto3.Y
                        largo(2) = "vertical_a"
                        largo(1) = "ZA"

                        largo(6) = pto4.GetDistanceTo(pto5)
                        largo(7) = pto0.GetDistanceTo(pto1)
                        largo(8) = pto4.GetDistanceTo(pto3)
                        largo(9) = pto1.GetDistanceTo(pto2)
                    Else
                        largo(4) = pto3.X & "," & pto3.Y & "_" & pto2.X & "," & pto2.Y
                        largo(2) = "vertical_a"
                        largo(1) = "AZ"


                        largo(6) = pto0.GetDistanceTo(pto1)
                        largo(7) = pto4.GetDistanceTo(pto5)
                        largo(8) = pto1.GetDistanceTo(pto2)
                        largo(9) = pto4.GetDistanceTo(pto3)
                    End If
                Else
                    ' HORIZONTAL O INCLINADA

                    If pto2.X > pto3.X Then
                        largo(4) = pto2.X & "," & pto2.Y & "_" & pto3.X & "," & pto3.Y
                        largo(2) = "horizontal_i"
                        largo(1) = "AZ"

                        largo(6) = pto4.GetDistanceTo(pto5)
                        largo(7) = pto0.GetDistanceTo(pto1)
                        largo(8) = pto4.GetDistanceTo(pto3)
                        largo(9) = pto1.GetDistanceTo(pto2)
                    Else
                        largo(4) = pto3.X & "," & pto3.Y & "_" & pto2.X & "," & pto2.Y
                        largo(2) = "horizontal_i"
                        largo(1) = "ZA"


                        largo(6) = pto0.GetDistanceTo(pto1)
                        largo(7) = pto4.GetDistanceTo(pto5)
                        largo(8) = pto1.GetDistanceTo(pto2)
                        largo(9) = pto4.GetDistanceTo(pto3)
                    End If


                End If

                largo(3) = (pto3.X + pto2.X) / 2 & "," & (pto3.Y + pto2.Y) / 2
                largo(5) = pto3.GetDistanceTo(pto2)

            Case Else
                Debug.WriteLine("Not between 1 and 10, inclusive")
        End Select
        Return largo
    End Function



    Function AUX_DIRECION_BARRA(ByRef POLY As Polyline, ByRef posible_caso As String, ByRef coord As String) As String

        Dim pt_orient As Point3d
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database



        Dim ptos As String() = coord.Split(New [Char]() {"_"c, CChar(vbTab)})
        Dim ptos_23 As String() = ptos(0).Split(New [Char]() {","c, CChar(vbTab)})
        Dim ptos_45 As String() = ptos(1).Split(New [Char]() {","c, CChar(vbTab)})

        Dim x_prom As Single = (ptos_23(0) + 0 + ptos_45(0)) / 2
        Dim y_prom As Single = (ptos_23(1) + 0 + ptos_45(1)) / 2




        Using acTrans2 As Transaction = acCurDb.TransactionManager.StartTransaction()

            Dim GRUPO_ As New CODIGOS_GRUPOS()
            ' ENTREGA UN COLECCION DE LOS ELEMTOS QUE PERTENECEN A UN DETERMINADO GRUPO, BUSCA EL NOMBRE DEL GRUPO CON ObjectID
            Dim acObjId_grup As ObjectId() = GRUPO_.buscar_grupo(POLY.ObjectId)
            If Not (acObjId_grup Is Nothing) Then
                For Each idObj_ As ObjectId In acObjId_grup
                    ' Application.ShowAlertDialog("idObj_.ObjectClass.DxfName.ToString() : " & idObj_.ObjectClass.DxfName.ToString())
                    If idObj_.ObjectClass.DxfName.ToString = "TEXT" Then
                        Dim acEnt_TEXTO_ As DBText = TryCast(acTrans2.GetObject(idObj_, OpenMode.ForWrite), DBText)
                        acEnt_TEXTO_.Erase()
                        pt_orient = acEnt_TEXTO_.Position
                        GoTo ir1
                    End If
                Next
            Else
                AUX_DIRECION_BARRA = Nothing
                GoTo ir2
            End If
        End Using

ir1:
        If posible_caso = "izq" Or posible_caso = "dere" Then

            If x_prom > pt_orient.X Then
                AUX_DIRECION_BARRA = "izq"
            Else
                AUX_DIRECION_BARRA = "dere"
            End If
        Else
            If y_prom > pt_orient.Y Then
                AUX_DIRECION_BARRA = "bajo"
            Else
                AUX_DIRECION_BARRA = "arriba"
            End If
        End If
ir2:
        Return AUX_DIRECION_BARRA
    End Function

    Function ultimo_elemto() As Object
        Dim ed As Editor = Application.DocumentManager.MdiActiveDocument.Editor
        Dim acSSPrompt As PromptSelectionResult
        Dim _acSSet_LINEA As SelectionSet

        acSSPrompt = ed.SelectLast

        ' Si el estado de la solicitud es OK, es que se han seleccionado objetos
        If acSSPrompt.Status = PromptStatus.OK Then
            _acSSet_LINEA = acSSPrompt.Value
            Dim acObjIdColl As ObjectIdCollection = New ObjectIdCollection(_acSSet_LINEA.GetObjectIds())

            ultimo_elemto = acObjIdColl.Item(0)
        Else
            ultimo_elemto = Nothing
        End If

        Return ultimo_elemto
    End Function

    Function coordenada__angulo_p1_p2_fun(ByVal pt1 As Point3d, ByVal pt2 As Point3d, ByVal ed As Editor, ByVal angle As Single) As Double

        Dim ang As Double
        If IsNothing(ed) Then
            ang = angle
            GoTo salto
        End If

        Dim ucsmtx As Matrix3d = ed.CurrentUserCoordinateSystem

        Dim ucs As CoordinateSystem3d = ucsmtx.CoordinateSystem3d

        Dim ucsplane As New Plane(ucs.Origin, ucs.Xaxis, ucs.Yaxis)

        Dim vec As Vector3d = pt2 - pt1

        ang = vec.AngleOnPlane(ucsplane)
salto:
        If ang > PI * 6 Then
            ang = ang - PI * 6
            If ang > PI / 2 * 1.02 Then
                ang = ang - PI
            End If
        ElseIf ang > PI * 5 Then
            ang = ang - PI * 5
            If ang > PI / 2 * 1.02 Then
                ang = ang - PI
            End If
        ElseIf ang > PI * 4 Then
            ang = ang - PI * 4
            If ang > PI / 2 * 1.02 Then
                ang = ang - PI
            End If
        ElseIf ang > PI * 3 Then
            ang = ang - PI * 3
            If ang > PI / 2 * 1.02 Then
                ang = ang - PI
            End If
        ElseIf ang > PI * 2 Then
            ang = ang - PI * 2
            If ang > PI / 2 * 1.02 Then
                ang = ang - PI
            End If
        ElseIf ang > PI Then
            ang = ang - PI
            If ang > PI / 2 * 1.02 Then
                ang = ang - PI
            End If
        ElseIf ang > PI / 2 * 1.02 Then
            ang = ang - PI
        End If


        Return ang

    End Function

    Function coordenada_modificar_fun(ByRef entidad As Entity, ByVal pt1 As Point3d, ByVal pt2 As Point3d) As Point3d()
        ' modifica lso ptos de uan linea
        ' si la linea tiene alguna inclinacion pt2 pto x mas grande 
        ' si linea complatamente vertival  pt2 es con  y mas grande
        ' resultado coordenada_modificar_ (PT1_ (MENOR) PTO_2(MAYOR) ))
        Dim coordenada_modificar_(1) As Point3d
        If Not IsNothing(entidad) Then
            If entidad.ObjectId.ObjectClass.DxfName.ToString = "LINE" Then


            Else


            End If
        End If

        If Abs(pt1.X - pt2.X) < 0.1 Then 'lineas COMPLETAMENTE  verticales , p2 al y mas alto   (lado_corto = ent  p1 p2)
            If pt2.Y < pt1.Y Then
                coordenada_modificar_(0) = pt2
                coordenada_modificar_(1) = pt1
            Else
                coordenada_modificar_(0) = pt1
                coordenada_modificar_(1) = pt2
            End If

        Else  ' solo lineas comletamente horizontales  con cualquier inclinacion.. p2 mayor x       (lado_corto = ent  p1 p2)
            If pt2.X < pt1.X Then
                coordenada_modificar_(0) = pt2
                coordenada_modificar_(1) = pt1
            Else
                coordenada_modificar_(0) = pt1
                coordenada_modificar_(1) = pt2
            End If

        End If

        Return coordenada_modificar_


    End Function



    Public Function NormalizeVector(ByVal inputVector As Point3d) As Point3d
        ' Calcular la magnitud del vector
        Dim magnitude As Double = Math.Sqrt(inputVector.X ^ 2 + inputVector.Y ^ 2 + inputVector.Z ^ 2)

        ' Si la magnitud es cero, no es posible normalizar
        If magnitude = 0 Then
            Throw New InvalidOperationException("El vector no puede tener magnitud cero.")
        End If

        ' Devolver el vector normalizado
        Return New Point3d(inputVector.X / magnitude, inputVector.Y / magnitude, inputVector.Z / magnitude)
    End Function



    Public Function NormalizeDifference(ByVal P1 As Point3d, ByVal P2 As Point3d) As Point3d
        ' Calcula el vector diferencia
        Dim diffVector As New Point3d(P2.X - P1.X, P2.Y - P1.Y, P2.Z - P1.Z)

        ' Calcular la magnitud del vector diferencia
        Dim magnitude As Double = Math.Sqrt(diffVector.X ^ 2 + diffVector.Y ^ 2 + diffVector.Z ^ 2)

        ' Si la magnitud es cero, no es posible normalizar
        If magnitude = 0 Then
            Throw New InvalidOperationException("El vector resultante no puede tener magnitud cero.")
        End If

        ' Devolver el vector normalizado
        Return New Point3d(diffVector.X / magnitude, diffVector.Y / magnitude, diffVector.Z / magnitude)
    End Function

    Public Function NormalizeVector(ByVal inputVector As Point2d) As Point2d
        ' Calcular la magnitud del vector
        Dim magnitude As Double = Math.Sqrt(inputVector.X ^ 2 + inputVector.Y ^ 2)

        ' Si la magnitud es cero, no es posible normalizar
        If magnitude = 0 Then
            Throw New InvalidOperationException("El vector no puede tener magnitud cero.")
        End If

        ' Devolver el vector normalizado
        Return New Point2d(inputVector.X / magnitude, inputVector.Y / magnitude)
    End Function
    Public Function NormalizeDifference(ByVal P1 As Point2d, ByVal P2 As Point2d) As Point2d
        ' Calcula el vector diferencia
        Dim diffVector As New Point2d(P2.X - P1.X, P2.Y - P1.Y)

        ' Calcular la magnitud del vector diferencia
        Dim magnitude As Double = Math.Sqrt(diffVector.X ^ 2 + diffVector.Y ^ 2)

        ' Si la magnitud es cero, no es posible normalizar
        If magnitude = 0 Then
            Throw New InvalidOperationException("El vector resultante no puede tener magnitud cero.")
        End If

        ' Devolver el vector normalizado
        Return New Point2d(diffVector.X / magnitude, diffVector.Y / magnitude)
    End Function

    Public Function Point2dToPoint3d(ByVal pt2d As Point2d, Optional zValue As Double = 0.0) As Point3d
        Return New Point3d(pt2d.X, pt2d.Y, zValue)
    End Function

    Public Function Point3dToPoint2d(ByVal pt3d As Point3d) As Point2d
        Return New Point2d(pt3d.X, pt3d.Y)
    End Function
End Class
