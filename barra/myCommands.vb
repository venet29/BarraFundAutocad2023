' (C) Copyright 2014 by Microsoft 
'
Imports System.Math
Imports System.Runtime
Imports System
Imports System.IO
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Windows
Imports Autodesk.AutoCAD.Colors
Imports VARIOS
Imports System.Data.SqlClient
Imports System.Data.OleDb



Imports System.Collections.Generic
Imports System.Diagnostics
Imports Autodesk.AutoCAD.Customization







' This line is not mandatory, but improves loading performances
'<Assembly: CommandClass(GetType(barra.MyCommands))> 

'Namespace barravlisp

' This class is instantiated by AutoCAD for each document when
' a command is called by the user the first time in the context
' of a given document. In other words, non static data in this class
' is implicitly per-document!
Public Class My1Commands
    Public Shared datos_barra(40) As String
    Public Shared casos_dibujar As String
    Public Shared acObjIdColl_GENERAL As ObjectIdCollection = Nothing
    Public Shared puntos_finalas(1) As Point3d
    ' Public Shared _doc As Document
    Public Shared grupo_referencia As String = "*A2867"
    Public Shared AUX_UNDO As Boolean = False
    Public Shared _ps_funda As Autodesk.AutoCAD.Windows.PaletteSet '= Nothing

    Public Shared myPalette As FUNDA

    ' Friend Shared _ps As Autodesk.AutoCAD.Windows.PaletteSet = Nothing
    ' Public Shared _contenerdorIDOBJ As Dictionary(Of ObjectId, ObjectId)
    Public Shared acObjIdColl_borra_ As ObjectIdCollection = New ObjectIdCollection()
    Public Shared pts1_v As New Point2d
    Public Shared pts2_v As New Point2d
    Public Shared _contenerdorIDOBJ As ObjectIdCollection = Nothing
    Public Shared _acSSet_LINEA2 As SelectionSet

    Private Shared _ManejadorUsuarios As ManejadorDatos
    'Private _mapAnchoredToHost As Dictionary(O| ObjectId, ObjectId)
    Shared _commandNames As New List(Of String)(New String() {"MOVE", "GRIP_STRETCH", "STRETCH", "EXTEND", "EXTEND", "EATTEDIT"}) '"COPY", , "TRIM""FILLET", "OFFSET", "MIRROR",
    Shared _commandNames_despues As New List(Of String)(New String() {"+DSETTINGS"})  '"MIRROR",, "TRIM","FILLET",
    Shared _commandNames_UNDO As New List(Of String)(New String() {"U", "UNDO"})
    Public Shared aux_enfierra_automatico As Boolean = False  ' si variable es falsa entonces se esta corriendo el programa enfierarra automatico
    Public Shared abrir_nuevo_archivo As Boolean = False


    Public Sub New()
        If Not Nothing = datos_barra(3) AndAlso datos_barra(3) = "si" Then GoTo salto

        Application.SetSystemVariable("PICKFIRST", 1)
        Application.SetSystemVariable("PICKADD", 1)


        Dim _ManejadorUsuarios As New ManejadorDatos()
        Dim resultadoConexion As Boolean = _ManejadorUsuarios.PostBitacora("CARGAR ManejadorEstriboCOnfinamiento")

        If Not resultadoConexion Then
            utiles.ErrorMsg(utiles.ERROR_CONEXION)
            Return
        ElseIf Not _ManejadorUsuarios.ObteneResultado() Then
            utiles.ErrorMsg(utiles.ERROR_OBTENERDATOS)
            Return
        End If

        _contenerdorIDOBJ = New ObjectIdCollection()


        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        AddHandler acDoc.CommandWillStart, AddressOf doc_CommandWillStart
        AddHandler Application.DocumentManager.DocumentCreated, AddressOf docBeginDocOpen_f



        Dim acSSPrompt As PromptSelectionResult
        Dim acDocEd As Editor = Application.DocumentManager.MdiActiveDocument.Editor
        acSSPrompt = acDocEd.SelectLast
        If acSSPrompt.Status = PromptStatus.OK Then
            _acSSet_LINEA2 = acSSPrompt.Value
            _acSSet_LINEA2.Dispose()
        End If
salto:
    End Sub


    Public Sub docBeginDocOpen_f(ByVal senderObj As Object, ByVal docColDocActEvtArgs As DocumentCollectionEventArgs)

        If abrir_nuevo_archivo = True Then
            carga_archivo_nueva_f()
            abrir_nuevo_archivo = False
        End If
    End Sub

    Public Sub carga_archivo_nueva_f()

        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        AddHandler acDoc.CommandWillStart, AddressOf doc_CommandWillStart
        Using acDoc.LockDocument()

            Dim aux_cambiar As Boolean = False
            Dim acLayoutMgr As LayoutManager

            acLayoutMgr = LayoutManager.Current
            If acLayoutMgr.CurrentLayout <> "Model" Then
                acLayoutMgr.CurrentLayout = "Model"
                aux_cambiar = True
            End If

            Dim utiles_aux As New utiles
            utiles_aux.Create_ALayer("CESTRIBO_E")
            utiles_aux.Create_ALayer("CESTRIBO_L")
            utiles_aux.Create_ALayer("ESTRIBO")
            utiles_aux.Create_ALayer("BARRAS")
            utiles_aux.Create_ALayer("TEXTO")
            Dim caso_m1 As New M1
            caso_m1.aux_reescribir_barras(My1Commands.acObjIdColl_GENERAL, "all_rev")


            If aux_cambiar Then
                acLayoutMgr = LayoutManager.Current
                acLayoutMgr.CurrentLayout = "Layout1"
            End If

            acDoc.Editor.WriteMessage("Fin revision de barra")

        End Using



    End Sub
    ' The CommandMethod attribute can be applied to any public  member 
    ' function of any public class.
    ' The function should take no arguments and return nothing.
    ' If the method is an instance member then the enclosing class is 
    ' instantiated for each document. If the member is a static member then
    ' the enclosing class is NOT instantiated.
    '
    ' NOTE: CommandMethod has overloads where you can provide helpid and
    ' context menu.

    ' Modal Command with localized name
    ' AutoCAD will search for a resource string with Id "MyCommandLocal" in the 
    ' same namespace as this command class. 
    ' If a resource string is not found, then the string "MyLocalCommand" is used 
    ' as the localized command name.
    ' To view/edit the resx file defining the resource strings for this command, 
    ' * click the 'Show All Files' button in the Solution Explorer;
    ' * expand the tree node for myCommands.vb;
    ' * and double click on myCommands.resx
    <CommandMethod("MyGroup", "MyCommand", "MyCommandLocal", CommandFlags.Modal)>
    Public Sub MyCommand() ' This method can have any name
        ' Put your command code here
    End Sub

    ' Modal Command with pickfirst selection
    <CommandMethod("MyGroup", "MyPickFirst", "MyPickFirstLocal", CommandFlags.Modal + CommandFlags.UsePickSet)>
    Public Sub MyPickFirst() ' This method can have any name
        Dim result As PromptSelectionResult = Application.DocumentManager.MdiActiveDocument.Editor.GetSelection()
        If (result.Status = PromptStatus.OK) Then
            ' There are selected entities
            ' Put your command using pickfirst set code here
        Else
            ' There are no selected entities
            ' Put your command code here
        End If
    End Sub

    ' Application Session Command with localized name
    <CommandMethod("MyGroup", "MySessionCmd", "MySessionCmdLocal", CommandFlags.Modal + CommandFlags.Session)>
    Public Sub MySessionCmd() ' This method can have any name
        ' Put your command code here
    End Sub
    <CommandMethod("Ba_add")>
    Public Sub Ba_add()
        'crear_barra_total()

    End Sub


    Public Sub doc_CommandWillStart(ByVal sender As Object, ByVal e As CommandEventArgs)
        Dim _doc As Document = Application.DocumentManager.MdiActiveDocument
        If (My1Commands.myPalette Is Nothing) Then Return

        If My1Commands.myPalette.chbx_desac_reac.Checked = False Then



            If _commandNames.Contains(e.GlobalCommandName) Then
                'Application.ShowAlertDialog("comando :" & e.GlobalCommandName)
                datos_barra(3) = e.GlobalCommandName
                My1Commands._contenerdorIDOBJ.Clear()
                '_doc.Database.ObjectOpenedForModify += New ObjectEventHandler(AddressOf _db_ObjectOpenedForModify)
                AddHandler _doc.Database.ObjectOpenedForModify, AddressOf buscar_contenedor
                ' _doc.CommandCancelled += New CommandEventHandler(AddressOf _doc_CommandEnded)
                AddHandler _doc.CommandCancelled, AddressOf _doc_CommandEnded
                '_doc.CommandEnded += New CommandEventHandler(AddressOf _doc_CommandEnded)
                AddHandler _doc.CommandEnded, AddressOf _doc_CommandEnded
                '  _doc.CommandFailed += New CommandEventHandler(AddressOf _doc_CommandEnded)
                AddHandler _doc.CommandFailed, AddressOf _doc_CommandEnded
            ElseIf _commandNames_UNDO.Contains(e.GlobalCommandName) Then
                datos_barra(3) = e.GlobalCommandName
                AUX_UNDO = True
                AddHandler _doc.CommandCancelled, AddressOf _doc_CommandEnded
                '_doc.CommandEnded += New CommandEventHandler(AddressOf _doc_CommandEnded)
                AddHandler _doc.CommandEnded, AddressOf _doc_CommandEnded
                '_doc.CommandFailed += New CommandEventHandler(AddressOf _doc_CommandEnded)
                AddHandler _doc.CommandFailed, AddressOf _doc_CommandEnded

            ElseIf _commandNames_despues.Contains(e.GlobalCommandName) Then
                'Application.ShowAlertDialog("comando :" & e.GlobalCommandName)
                datos_barra(3) = e.GlobalCommandName
                datos_barra(14) = e.GlobalCommandName
                My1Commands._contenerdorIDOBJ.Clear()

                Application.DocumentManager.MdiActiveDocument.SendStringToExecute(ChrW(3) & ChrW(3), False, True, False)


            End If
        End If


        If e.GlobalCommandName = "QUIT" Then

            RemoveHandler _doc.CommandWillStart, AddressOf doc_CommandWillStart
            datos_barra(3) = "si"
        ElseIf e.GlobalCommandName = "OPEN" Then
            abrir_nuevo_archivo = True

        End If
    End Sub
    Private Sub _doc_CommandEnded(ByVal sender As Object, ByVal e As CommandEventArgs)
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim ed As Editor = Application.DocumentManager.MdiActiveDocument.Editor
        removeEventHandlers()
        Dim caso_m1 As New M1
        ' Remove database reactor before restoring positions
        Try

            If My1Commands._contenerdorIDOBJ <> Nothing Then


                Dim aux_acObjIdColl_GENERAL As ObjectIdCollection = New ObjectIdCollection() 'My1Commands._contenerdorIDOBJ
                For Each acObjId As ObjectId In My1Commands._contenerdorIDOBJ
                    aux_acObjIdColl_GENERAL.Add(acObjId)
                Next

                Dim pMin As Point3d
                Dim pMax As Point3d
                Using acView As ViewTableRecord = acDoc.Editor.GetCurrentView()
                    pMin = New Point3d(Application.GetSystemVariable("viewctr").X - (acView.Width / 2), Application.GetSystemVariable("viewctr").Y - (acView.Height / 2), 0)
                    pMax = New Point3d((acView.Width / 2) + Application.GetSystemVariable("viewctr").X, (acView.Height / 2) + Application.GetSystemVariable("viewctr").Y, 0)
                End Using



                For Each acObjId As ObjectId In aux_acObjIdColl_GENERAL
                    If datos_barra(3) = "STRETCH" Or "GRIP_STRETCH" = datos_barra(3) Or datos_barra(3) = "MOVE" Then
                        caso_m1.reac_modif_barra_losa_final_texto(acObjId, My1Commands.aux_enfierra_automatico, acDoc)
                    ElseIf "EATTEDIT" = datos_barra(3) Then
                        caso_m1.reac_modif_atribut(acObjId, My1Commands.aux_enfierra_automatico, acDoc, My1Commands._contenerdorIDOBJ)
                    End If

                Next
                Dim utiles_aux As New utiles
                utiles_aux.Zoom(pMin, pMax, New Point3d(), 1)

                Application.DocumentManager.MdiActiveDocument.Editor.UpdateScreen()
                Application.DocumentManager.MdiActiveDocument.Editor.Regen()


            End If
        Catch Ex As Autodesk.AutoCAD.Runtime.Exception
            Application.ShowAlertDialog("Error:" & vbLf & Ex.Message)
            Application.ShowAlertDialog("Error _doc_CommandEnded")
            'Finally
            ' Se mostrara este mensaje aunque se produzca un error
            'Application.ShowAlertDialog("Error _doc_CommandEnded")
        End Try
        My1Commands.datos_barra(3) = ""
        My1Commands._contenerdorIDOBJ.Clear()
        AUX_UNDO = False
        ed.WriteMessage("reactor _doc_CommandEnded ok")
    End Sub
    Private Sub removeEventHandlers()
        Dim _doc As Document = Application.DocumentManager.MdiActiveDocument

        RemoveHandler _doc.CommandCancelled, AddressOf _doc_CommandEnded
        '_doc.CommandCancelled -= New CommandEventHandler(AddressOf _doc_CommandEnded)
        RemoveHandler _doc.CommandEnded, AddressOf _doc_CommandEnded
        '  _doc.CommandEnded -= New CommandEventHandler(AddressOf _doc_CommandEnded)
        RemoveHandler _doc.CommandFailed, AddressOf _doc_CommandEnded
        '_doc.CommandFailed -= New CommandEventHandler(AddressOf _doc_CommandEnded)
        RemoveHandler _doc.Database.ObjectOpenedForModify, AddressOf buscar_contenedor
        '  _doc.Database.ObjectOpenedForModify -= New ObjectEventHandler(AddressOf _db_ObjectOpenedForModify)

    End Sub




    Private Sub buscar_contenedor(ByVal sender As Object, ByVal e As ObjectEventArgs)
        Dim id As ObjectId = e.DBObject.Id
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        '  ed.WriteMessage(ControlChars.Lf & acBlkRef.ToString() & ControlChars.Lf)
        '  ed.WriteMessage(String.Format("{0}Propiedades de ""{{0}}""{0}", _
        '                                      ControlChars.Lf), acBlkRef.Name)
        Dim ed As Editor = acDoc.Editor

        Using acDoc.LockDocument()

            Using acTrans As Transaction = acDoc.TransactionManager.StartTransaction()
                Dim GRUPO_ As New CODIGOS_GRUPOS()
                Try

                    If id.ObjectClass.DxfName.ToString = "LWPOLYLINE" Then

                        Dim acEnt_barra As Polyline = TryCast(acTrans.GetObject(id, OpenMode.ForRead), Polyline)
                        'Application.ShowAlertDialog("layer :" & acEnt_barra.Layer)
                        Dim mtext As String = acEnt_barra.Layer
                        If mtext = "BARRAS" And My1Commands._contenerdorIDOBJ.Count = 0 Then


                            Dim acObjId_grup As ObjectId() = GRUPO_.buscar_grupo(acEnt_barra.ObjectId)
                            If Not (acObjId_grup Is Nothing) Then

                                If Not My1Commands._contenerdorIDOBJ.Contains(id) Then My1Commands._contenerdorIDOBJ.Add(id)
                                Using acTrans2 As Transaction = acCurDb.TransactionManager.StartTransaction()
                                    'SE BUSCA DENTRO DEL GRUPO SELECCIONADO ALGUNO QUE SE PARESCA AL DE REFERECIA
                                    ' SI ENCUERTA BORRA TODO 
                                    For Each idObj_ As ObjectId In acObjId_grup
                                        If idObj_.ObjectClass.DxfName.ToString = "LWPOLYLINE" And id <> idObj_ Then
                                            Dim acEnt_barra_aux As Polyline = TryCast(acTrans2.GetObject(idObj_, OpenMode.ForWrite), Polyline)
                                            If acEnt_barra_aux.Layer = "BARRAS" And Not My1Commands._contenerdorIDOBJ.Contains(idObj_) Then
                                                My1Commands._contenerdorIDOBJ.Add(idObj_)

                                                'reac_modif_barra_final(idObj_)
                                            End If

                                        End If
                                    Next

                                End Using
                            End If





                        ElseIf mtext = "BARRAS" And (datos_barra(3) = "STRETCH" Or "GRIP_STRETCH" = datos_barra(3) Or "COPY" = datos_barra(3) Or "MOVE" = datos_barra(3) Or "OFFSET" = datos_barra(3)) Then

                            Dim acObjId_grup As ObjectId() = GRUPO_.buscar_grupo(acEnt_barra.ObjectId)
                            If Not (acObjId_grup Is Nothing) Then
                                If Not My1Commands._contenerdorIDOBJ.Contains(id) Then My1Commands._contenerdorIDOBJ.Add(id)
                                Using acTrans2 As Transaction = acCurDb.TransactionManager.StartTransaction()
                                    'SE BUSCA DENTRO DEL GRUPO SELECCIONADO ALGUNO QUE SE PARESCA AL DE REFERECIA
                                    ' SI ENCUERTA BORRA TODO 
                                    For Each idObj_ As ObjectId In acObjId_grup
                                        If idObj_.ObjectClass.DxfName.ToString = "LWPOLYLINE" And id <> idObj_ Then
                                            Dim acEnt_barra_aux As Polyline = TryCast(acTrans2.GetObject(idObj_, OpenMode.ForWrite), Polyline)
                                            If acEnt_barra_aux.Layer = "BARRAS" And Not My1Commands._contenerdorIDOBJ.Contains(idObj_) Then
                                                My1Commands._contenerdorIDOBJ.Add(idObj_)

                                                'reac_modif_barra_final(idObj_)
                                            End If

                                        End If
                                    Next

                                End Using
                            End If


                        End If

                    ElseIf id.ObjectClass.DxfName.ToString = "ATTRIB" And datos_barra(3) = "EATTEDIT" Then


                        If My1Commands._contenerdorIDOBJ.Count = 0 Then




                            My1Commands._contenerdorIDOBJ.Add(id)


                            'Using acTrans2 As Transaction = acCurDb.TransactionManager.StartTransaction()
                            '    'SE BUSCA DENTRO DEL GRUPO SELECCIONADO ALGUNO QUE SE PARESCA AL DE REFERECIA
                            '    ' SI ENCUERTA BORRA TODO 
                            '    Dim acBlkTbl As BlockTable
                            '    acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead)


                            '    ' Comprobar si el bloque existe
                            '    If acBlkTbl.Has(flecha) Then
                            '        '' Abrir el registro del bloque de Espacio Modelo para escritura



                            '        Dim acBlkRef As BlockReference = acTrans.GetObject(id, OpenMode.ForRead, False, True)
                            '        Dim acAttCol As AttributeCollection = acBlkRef.AttributeCollection

                            '        For Each acObjId In acAttCol
                            '            Dim acAttRef As AttributeReference = acTrans.GetObject(acObjId, OpenMode.ForWrite)
                            '            'Dim sStr As String = ControlChars.Lf & "Etiqueta: " & _
                            '            'acAttRef.Tag & _
                            '            'ControlChars.Lf & "Valor: " & _
                            '            'acAttRef.TextString

                            '            If acAttRef.Tag = "LARGO_MIN" Then
                            '                datos_barra(13) = acAttRef.TextString

                            '            End If
                            '        Next


                            '    End If
                            'End Using

                        End If

                    End If
                    ' acTrans.Commit()
                    acTrans.Commit()
                Catch Ex As Autodesk.AutoCAD.Runtime.Exception
                    ed.WriteMessage("Error buscar_contenedor:" & vbLf & Ex.Message)
                    acTrans.Dispose()

                Finally

                    '  Application.ShowAlertDialog("Error buscar_contenedor")
                End Try
            End Using
        End Using
    End Sub




    'XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX


    <CommandMethod("FF1")>
    Public Sub FF1()
        If _ps_funda Is Nothing Then Exit Sub

        'validar
        If (_ManejadorUsuarios Is Nothing) Then
            _ManejadorUsuarios = New ManejadorDatos()
        End If
        Dim resultadoConexion As Boolean = _ManejadorUsuarios.PostBitacora("CARGAR ManejadorEstriboCOnfinamiento")

        If Not resultadoConexion Then
            utiles.ErrorMsg(utiles.ERROR_CONEXION)
            Return
        ElseIf Not _ManejadorUsuarios.ObteneResultado() Then
            utiles.ErrorMsg(utiles.ERROR_OBTENERDATOS)
            Return
        End If
        '*************************

        Dim caso_m1 As New M1
        caso_m1.aux__barra(My1Commands.myPalette, My1Commands.casos_dibujar, My1Commands.grupo_referencia)
    End Sub
    <CommandMethod("FF2")>
    Public Sub FF2()
        If _ps_funda Is Nothing Then Exit Sub

        'validar
        If (_ManejadorUsuarios Is Nothing) Then
            _ManejadorUsuarios = New ManejadorDatos()
        End If
        Dim resultadoConexion As Boolean = _ManejadorUsuarios.PostBitacora("CARGAR ManejadorEstriboCOnfinamiento")

        If Not resultadoConexion Then
            utiles.ErrorMsg(utiles.ERROR_CONEXION)
            Return
        ElseIf Not _ManejadorUsuarios.ObteneResultado() Then
            utiles.ErrorMsg(utiles.ERROR_OBTENERDATOS)
            Return
        End If
        '*************************

        Dim caso_m1 As New M1
        caso_m1.aux__barra_rango(My1Commands.myPalette.txt_recub.Text, My1Commands.casos_dibujar)
    End Sub
    <CommandMethod("FF3")>
    Public Sub FF3()
        If _ps_funda Is Nothing Then Exit Sub

        'validar
        If (_ManejadorUsuarios Is Nothing) Then
            _ManejadorUsuarios = New ManejadorDatos()
        End If
        Dim resultadoConexion As Boolean = _ManejadorUsuarios.PostBitacora("CARGAR ManejadorEstriboCOnfinamiento")

        If Not resultadoConexion Then
            utiles.ErrorMsg(utiles.ERROR_CONEXION)
            Return
        ElseIf Not _ManejadorUsuarios.ObteneResultado() Then
            utiles.ErrorMsg(utiles.ERROR_OBTENERDATOS)
            Return
        End If
        '*************************

        Dim caso_m1 As New M1
        caso_m1.aux_dtras_fund(My1Commands.myPalette.txt_recub.Text, My1Commands.myPalette.ckbx_traslapo.Checked, My1Commands.grupo_referencia, My1Commands.casos_dibujar, My1Commands._contenerdorIDOBJ)
    End Sub
    'desagruper barras


    'E MEPLO DE PromptKeywordOptions




    ' busca dentro del plano en la viñeta , el  tag con nombre titulo y que lo cambie por caca, pero no lo cambio si recorrer todos los elemtos dentro de viñeta

    <CommandMethod("EBFU")> _
    Public Shared Sub EBFU()
        If _ps_funda IsNot Nothing Then
            'validar
            If (_ManejadorUsuarios Is Nothing) Then
                _ManejadorUsuarios = New ManejadorDatos()
            End If
            Dim resultadoConexion As Boolean = _ManejadorUsuarios.PostBitacora("CARGAR ManejadorEstriboCOnfinamiento")

            If Not resultadoConexion Then
                utiles.ErrorMsg(utiles.ERROR_CONEXION)
                Return
            ElseIf Not _ManejadorUsuarios.ObteneResultado() Then
                utiles.ErrorMsg(utiles.ERROR_OBTENERDATOS)
                Return
            End If
            '*************************
            Dim caso_fund As New M1
            caso_fund.ESTRIBO_FUNDACIONES()

            '' Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable("CMDNAMES", "ESLO")
        End If

    End Sub



    Private Function UpdateAttributesInDatabase(ByVal db As Database, ByVal blockName As String, ByVal attbName As String, ByVal attbValue As String) As Integer

        Dim psId As ObjectId
        Dim tr As Transaction = db.TransactionManager.StartTransaction()
        Using tr

            Dim bt As BlockTable = DirectCast(tr.GetObject(db.BlockTableId, OpenMode.ForRead), BlockTable)
            psId = bt(BlockTableRecord.PaperSpace)
            tr.Commit()
        End Using

        Dim psCount As Integer = UpdateAttributesInBlock(db, psId, blockName, attbName, attbValue)
        Return psCount

    End Function
    Function UpdateAttributesInBlock(ByVal db As Database, ByVal btrId As ObjectId, ByVal blockName As String, ByVal attbName As String, ByVal attbValue As String) As Integer
        'Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument

        Dim changedCount As Integer = 0
        Dim tr As Transaction = db.TransactionManager.StartTransaction()
        Using tr
            Dim btr As BlockTableRecord = DirectCast(tr.GetObject(btrId, OpenMode.ForRead), BlockTableRecord)

            For Each entId As ObjectId In btr
                Dim ent As Entity = TryCast(tr.GetObject(entId, OpenMode.ForRead), Entity)
                If ent IsNot Nothing Then
                    Dim br As BlockReference = TryCast(ent, BlockReference)
                    If br IsNot Nothing Then
                        Dim bd As BlockTableRecord = DirectCast(tr.GetObject(br.BlockTableRecord, OpenMode.ForRead), BlockTableRecord)

                        If bd.Name.ToUpper() = blockName Then

                            For Each arId As ObjectId In br.AttributeCollection
                                Dim obj As DBObject = tr.GetObject(arId, OpenMode.ForRead)
                                Dim ar As AttributeReference = TryCast(obj, AttributeReference)
                                If ar IsNot Nothing Then

                                    If ar.Tag.ToUpper() = attbName Then
                                        ar.UpgradeOpen()
                                        ' Application.ShowAlertDialog("valor tag : " & ar.TextString)
                                        ar.TextString = attbValue
                                        ar.DowngradeOpen()
                                        changedCount += 1
                                    End If
                                End If
                            Next
                        End If

                        ' changedCount += UpdateAttributesInBlock(db, br.BlockTableRecord, blockName, attbName, attbValue)
                    End If
                End If
            Next
            tr.Commit()
        End Using
        Return changedCount
    End Function



    <CommandMethod("Bad")> _
    Public Sub Bad()
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acEd As Editor = acDoc.Editor

        ' Iniciar la interacion


        ' Crear una matriz de objetos TypedValue para definir el criterio de seleccion

        Dim acTypValAr(1) As TypedValue
        acTypValAr.SetValue(New TypedValue(DxfCode.Start, "LWPOLYLINE"), 0)
        acTypValAr.SetValue(New TypedValue(DxfCode.LayerName, "BARRAS"), 1)




        ' Asignar el criterio de seleccion al objeto SelectionFilter
        Dim acSelFtr As SelectionFilter = New SelectionFilter(acTypValAr)
        ' Solicitar al usuario que seleccione objetos en el área de dibujo
        Dim acSSPrompt As PromptSelectionResult
        acSSPrompt = acEd.GetSelection(acSelFtr)


        ' Si el estado de la solicitud es OK, es que se han seleccionado objetos
        If acSSPrompt.Status = PromptStatus.OK Then
            Dim acSSet As SelectionSet = acSSPrompt.Value

            ' Recorrer el segundo conjunto de seleccion
            For Each acObjId As ObjectId In acSSet.GetObjectIds()

                If My1Commands.acObjIdColl_GENERAL Is Nothing Then
                    My1Commands.acObjIdColl_GENERAL = New ObjectIdCollection(acSSet.GetObjectIds())
                ElseIf Not My1Commands.acObjIdColl_GENERAL.Contains(acObjId) Then
                    My1Commands.acObjIdColl_GENERAL.Add(acObjId)
                End If

            Next

            ' desagrupar_barras()

            'Application.ShowAlertDialog("Cantidad de objetos seleccionados: " & acSSet.Count)
            'Tsl_seleccion_malla.Text = "Elementos Seleccionados:" & acSSet.Count
        Else
            Application.ShowAlertDialog("Cantidad de objetos seleccionados: 0")
            'Tsl_seleccion_malla.Text = "Elementos Seleccionados:0"
        End If


        ' Libera recursos y vuelve a mostrar el cuadro de dialogo

    End Sub


    <CommandMethod("nfund")> _
    Public Sub nfund()

        '***************
        'validar
        If (_ManejadorUsuarios Is Nothing) Then
            _ManejadorUsuarios = New ManejadorDatos()
        End If
        Dim resultadoConexion As Boolean = _ManejadorUsuarios.PostBitacora("CARGAR ManejadorEstriboCOnfinamiento")

        If Not resultadoConexion Then
            utiles.ErrorMsg(utiles.ERROR_CONEXION)
            Return
        ElseIf Not _ManejadorUsuarios.ObteneResultado() Then
            utiles.ErrorMsg(utiles.ERROR_OBTENERDATOS)
            Return
        End If
        '*************************

        If _ps_funda Is Nothing Then
            ' Crear el objeto
            _ps_funda = New Autodesk.AutoCAD.Windows.PaletteSet("Armadura Fundaciones")
            ' El contructor tambien admite este formato:vlis
            '_ps = New Autodesk.AutoCAD.Windows.PaletteSet("Paleta de prueba", _
            '                          New System.Guid("EA098374-4136-4aa4-BE3D-00295774C80E"))
            ' Con este ultimo formato es posible guardar leer datos de usuario

            ' Lados en los que puede anclarse
            _ps_funda.DockEnabled = Autodesk.AutoCAD.Windows.DockSides.Left Or
            Autodesk.AutoCAD.Windows.DockSides.Right
            ' Tamaño minimo
            _ps_funda.MinimumSize = New System.Drawing.Size(300, 300)
            myPalette = New FUNDA
            ' Añadir el control
            _ps_funda.Add("Armadura Fundaciones", myPalette)

        End If
        ' carga_archivo_nueva_f()
        ' Mostrar la ventana anclable
        _ps_funda.Visible = True
    End Sub




    ' Define command
    '<CommandMethod("TestPalette")> _
    'Public Sub DoIt()

    '    Dim ps As Autodesk.AutoCAD.Windows.PaletteSet = Nothing
    '    ps = New Autodesk.AutoCAD.Windows.PaletteSet("My First Palette")
    '    Dim myPalette As Container1 = New Container1()
    '    ps.Add("My First Palette", myPalette)
    '    ps.Visible = True

    'End Sub


    ' LispFunction is similar to CommandMethod but it creates a lisp 
    ' callable function. Many return types are supported not just string
    ' or integer.
    <LispFunction("MyLispFunction", "MyLispFunctionLocal")> _
    Public Function MyLispFunction(ByVal args As ResultBuffer) ' This method can have any name
        ' Put your command code here

        ' Return a value to the AutoCAD Lisp Interpreter
        Return 1
    End Function
    <CommandMethod("CB")> _
    Public Sub CreateBlock()


        Dim doc As Document = Application.DocumentManager.MdiActiveDocument
        Dim db As Database = doc.Database
        Dim ed As Editor = doc.Editor

        Dim tr As Transaction = db.TransactionManager.StartTransaction()
        Using tr
            ' Get the block table from the drawing
            Dim bt As BlockTable = DirectCast(tr.GetObject(db.BlockTableId, OpenMode.ForRead), BlockTable)

            ' Check the block name, to see whether it's
            ' already in use
            Dim pso As New PromptStringOptions(vbLf & "Enter new block name: ")
            pso.AllowSpaces = True

            ' A variable for the block's name
            Dim blkName As String = ""

            Do
                Dim pr As PromptResult = ed.GetString(pso)
                ' Just return if the user cancelled
                ' (will abort the transaction as we drop out of the using
                ' statement's scope)
                If pr.Status <> PromptStatus.OK Then
                    Return
                End If
                Try
                    ' Validate the provided symbol table name
                    SymbolUtilityServices.ValidateSymbolName(pr.StringResult, False)
                    ' Only set the block name if it isn't in use
                    If bt.Has(pr.StringResult) Then
                        ed.WriteMessage(vbLf & "A block with this name already exists.")
                    Else
                        blkName = pr.StringResult

                    End If
                Catch
                    ' An exception has been thrown, indicating the
                    ' name is invalid
                    ed.WriteMessage(vbLf & "Invalid block name.")
                End Try
            Loop While blkName = ""

            ' Create our new block table record...
            Dim btr As New BlockTableRecord()
            ' ... and set its properties
            btr.Name = blkName

            ' Add the new block to the block table
            bt.UpgradeOpen()

            Dim btrId As ObjectId = bt.Add(btr)

            tr.AddNewlyCreatedDBObject(btr, True)

            ' Add some lines to the block to form a square
            ' (the entities belong directly to the block)

            Dim ents As DBObjectCollection = SquareOfLines(5)
            For Each ent As Entity In ents
                btr.AppendEntity(ent)
                tr.AddNewlyCreatedDBObject(ent, True)
            Next

            ' Add a block reference to the model space
            Dim ms As BlockTableRecord = DirectCast(tr.GetObject(bt(BlockTableRecord.ModelSpace), OpenMode.ForWrite), BlockTableRecord)
            Dim br As New BlockReference(Point3d.Origin, btrId)

            ms.AppendEntity(br)
            tr.AddNewlyCreatedDBObject(br, True)
            ' Commit the transaction
            tr.Commit()
            ' Report what we've done
            ed.WriteMessage(vbLf & "Created block named ""{0}"" containing {1} entities.", blkName, ents.Count)
        End Using

    End Sub
   



    <CommandMethod("com2")> _
    Public Sub com2()
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument

        ' Dibujar un circulo y hacer Zoom a al extension o limites del dibujo
        acDoc.SendStringToExecute("._ht", False, False, False)

    End Sub



    <CommandMethod("donut")> _
    Public Sub Createdonut()

        ' Obtener el documento y la base de datos actuales
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim ed As Editor = Application.DocumentManager.MdiActiveDocument.Editor
        Dim acCurDb As Database = acDoc.Database



        ' Iniciar una transaccion
        Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()

            ' Abrir la tabla para bloques en modo lectura
            Dim acBlkTbl As BlockTable
            acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead)

            ' Abrir el registro del bloque de Espacio Modelo para escritura
            Dim acBlkTblRec As BlockTableRecord
            acBlkTblRec = acTrans.GetObject(acBlkTbl(BlockTableRecord.ModelSpace), _
                                            OpenMode.ForWrite)



            Dim pdo1 As PromptDoubleOptions = New PromptDoubleOptions("\nSpecify inside diameter of donut: ")
            pdo1.AllowNegative = False

            pdo1.AllowNone = False

            pdo1.AllowZero = False

            Dim dres1 As PromptDoubleResult = ed.GetDouble(pdo1)

            If dres1.Status <> PromptStatus.OK Then
                Return
            End If
            Dim pdo2 As PromptDoubleOptions = New PromptDoubleOptions("\nSpecify outside diameter of donut: ")
            pdo2.AllowNegative = False

            pdo2.AllowNone = False

            pdo2.AllowZero = False

            Dim dres2 As PromptDoubleResult = ed.GetDouble(pdo2)

            If dres2.Status <> PromptStatus.OK Then
                Return
            End If
            Dim pto As PromptPointOptions = New PromptPointOptions("\nSpecify center point of donut: ")
            pto.AllowNone = False

            Dim ptres As PromptPointResult = ed.GetPoint(pto)

            Dim center As Point3d = ptres.Value

            If ptres.Status <> PromptStatus.OK Then
                Return
            End If

            Dim rad1 As Double = 0 'dres1.Value / 2

            Dim rad2 As Double = 6 'dres2.Value / 2

            Dim lwt As Double = rad2 - rad1





            Dim pt1 As Point2d = New Point2d(center.X - (rad1 + lwt / 2), center.Y)
            Dim pt2 As Point2d = New Point2d(center.X + (rad1 + lwt / 2), center.Y)
            Dim pline As Polyline = New Polyline()

            pline.AddVertexAt(0, pt1, 1.0, lwt, lwt)
            pline.AddVertexAt(1, pt2, 1.0, lwt, lwt)
            pline.AddVertexAt(2, pt1, 0.0, lwt, lwt)
            pline.Closed = True
            'acDoc.Editor.WriteMessage(ControlChars.Lf & "Identificador: " & objId.ToString())
            'acDoc.Editor.WriteMessage(ControlChars.Lf & "Controlador: " & objId.Handle.ToString())
            acTrans.AddNewlyCreatedDBObject(pline, True)

            ' Guardar el nuevo objeto
            acTrans.Commit()

        End Using

    End Sub



    Private Function SquareOfLines(ByVal size As Double) As DBObjectCollection

        ' A function to generate a set of entities for our block

        Dim ents As New DBObjectCollection()

        Dim pts As Point3d() = {New Point3d(-size, -size, 0), New Point3d(size, -size, 0), New Point3d(size, size, 0), New Point3d(-size, size, 0)}
        Dim max As Integer = pts.GetUpperBound(0)
        For i As Integer = 0 To max
            Dim j As Integer = (If(i = max, 0, i + 1))
            Dim ln As New Line(pts(i), pts(j))
            ents.Add(ln)
        Next
        Return ents

    End Function

   



    <CommandMethod("addext")> _
    Public Sub AddExtensionDictionary()


        Dim doc As Document = Application.DocumentManager.MdiActiveDocument
        Dim db As Database = doc.Database
        Dim ed As Editor = doc.Editor



        Dim ers As PromptEntityResult = ed.GetEntity("Select entity to add" + " extension dictionary ")

        If ers.Status <> PromptStatus.OK Then
            Return
        End If



        Using tr As Transaction = db.TransactionManager.StartTransaction()

            Dim dbObj As DBObject = tr.GetObject(ers.ObjectId, OpenMode.ForRead)
            Dim extId As ObjectId = dbObj.ExtensionDictionary

            If extId = ObjectId.Null Then
                dbObj.UpgradeOpen()
                dbObj.CreateExtensionDictionary()
                extId = dbObj.ExtensionDictionary
            End If



            'now we will have extId...


            Dim dbExt As DBDictionary = DirectCast(tr.GetObject(extId, OpenMode.ForRead), DBDictionary)



            'if not present add the data

            If Not dbExt.Contains("TEST") Then
                dbExt.UpgradeOpen()
                Dim xRec As New Xrecord()
                Dim rb As New ResultBuffer()
                rb.Add(New TypedValue(CInt(DxfCode.ExtendedDataAsciiString), "Data"))
                rb.Add(New TypedValue(CInt(DxfCode.ExtendedDataReal), 10.2))
                'set the data
                xRec.Data = rb
                dbExt.SetAt("TEST", xRec)
                tr.AddNewlyCreatedDBObject(xRec, True)
            Else
                ed.WriteMessage("entity contains the TEST data" & vbLf)
            End If






            tr.Commit()
        End Using



    End Sub


    ''crea barras 
    '<CommandMethod("REBA")> _
    'Public Sub REBA()
    '    If _ps_funda IsNot Nothing Then
    '        Dim caso_m1 As New M1
    '        caso_m1.aux_reescribir_barras(My1Commands.acObjIdColl_GENERAL, "all")
    '    End If
    'End Sub

    <CommandMethod("REFE_F")> _
    Public Sub REFE_F()
        If _ps_funda IsNot Nothing Then
            'validar
            If (_ManejadorUsuarios Is Nothing) Then
                _ManejadorUsuarios = New ManejadorDatos()
            End If
            Dim resultadoConexion As Boolean = _ManejadorUsuarios.PostBitacora("CARGAR ManejadorEstriboCOnfinamiento")

            If Not resultadoConexion Then
                utiles.ErrorMsg(utiles.ERROR_CONEXION)
                Return
            ElseIf Not _ManejadorUsuarios.ObteneResultado() Then
                utiles.ErrorMsg(utiles.ERROR_OBTENERDATOS)
                Return
            End If
            '*************************

            Dim caso_m1 As New M1
            caso_m1.aux_reescribir_barras(My1Commands.acObjIdColl_GENERAL, "sel")
        End If
    End Sub
    <CommandMethod("REV_F")> _
    Public Sub REV_F()
        If _ps_funda IsNot Nothing Then

            'validar
            If (_ManejadorUsuarios Is Nothing) Then
                _ManejadorUsuarios = New ManejadorDatos()
            End If
            Dim resultadoConexion As Boolean = _ManejadorUsuarios.PostBitacora("CARGAR ManejadorEstriboCOnfinamiento")

            If Not resultadoConexion Then
                utiles.ErrorMsg(utiles.ERROR_CONEXION)
                Return
            ElseIf Not _ManejadorUsuarios.ObteneResultado() Then
                utiles.ErrorMsg(utiles.ERROR_OBTENERDATOS)
                Return
            End If
            '*************************

            Dim caso_m1 As New M1
            caso_m1.aux_reescribir_barras(My1Commands.acObjIdColl_GENERAL, "sel_rev")
        End If
    End Sub

    <CommandMethod("REV_F_all")> _
    Public Sub REV_F_all()
        If _ps_funda IsNot Nothing Then
            'validar
            If (_ManejadorUsuarios Is Nothing) Then
                _ManejadorUsuarios = New ManejadorDatos()
            End If
            Dim resultadoConexion As Boolean = _ManejadorUsuarios.PostBitacora("CARGAR ManejadorEstriboCOnfinamiento")

            If Not resultadoConexion Then
                utiles.ErrorMsg(utiles.ERROR_CONEXION)
                Return
            ElseIf Not _ManejadorUsuarios.ObteneResultado() Then
                utiles.ErrorMsg(utiles.ERROR_OBTENERDATOS)
                Return
            End If
            '*************************

            Dim caso_m1 As New M1
            caso_m1.aux_reescribir_barras(My1Commands.acObjIdColl_GENERAL, "all_rev")
        End If
    End Sub


    <CommandMethod("REFE_F2")> _
    Public Sub REFE_F2()
        If _ps_funda IsNot Nothing Then
            'validar
            If (_ManejadorUsuarios Is Nothing) Then
                _ManejadorUsuarios = New ManejadorDatos()
            End If
            Dim resultadoConexion As Boolean = _ManejadorUsuarios.PostBitacora("CARGAR ManejadorEstriboCOnfinamiento")

            If Not resultadoConexion Then
                utiles.ErrorMsg(utiles.ERROR_CONEXION)
                Return
            ElseIf Not _ManejadorUsuarios.ObteneResultado() Then
                utiles.ErrorMsg(utiles.ERROR_OBTENERDATOS)
                Return
            End If
            '*************************

            Dim caso_m1 As New M1
            caso_m1.aux_cambiar_texto_mirror()
        End If
    End Sub


    ' obtiene datos de entidad

    <CommandMethod("getDataref")> _
    Public Shared Sub getDataref()
        Dim GRUPO_DATOS As New CODIGOS_DATOS()
        GRUPO_DATOS.getData()
    End Sub
    <CommandMethod("getDataref2")>
    Public Shared Sub getDataref2()
        Dim GRUPO_DATOS As New CODIGOS_DATOS()
        GRUPO_DATOS.getData_gene()
    End Sub
    <CommandMethod("PP0")>
    Public Shared Sub PP0()
        Dim osmode_ini As Integer = Application.GetSystemVariable("PICKSTYLE")
        Application.SetSystemVariable("PICKSTYLE", 0)

    End Sub
    <CommandMethod("PP1")>
    Public Shared Sub PP1()
        Dim osmode_ini As Integer = Application.GetSystemVariable("PICKSTYLE")
        Application.SetSystemVariable("PICKSTYLE", 1)
    End Sub


    <CommandMethod("InfoGenerStatus")>
    Public Shared Sub InfoGenerStatus()
        Dim _InfoSystema As New InfoSystema_validar()
        _InfoSystema.M1_EjecutarInfoSistem()
        _InfoSystema.M3_getMacAddress3()
        _InfoSystema.mac.Replace(".", "_")
        utiles.ErrorMsg($" User valid \n N°: { _InfoSystema.mac.Replace(".", "_")} \n user:{ _InfoSystema.usuario}")
    End Sub








    'XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
    ' BARRA ESTADOO
    'Añadir un objeto Pane a la barra de estado
    'Añadir un objeto Pane a la barra de estadorivate Shared _PCount As Integer
    Private Shared _PCount As Integer
    <CommandMethod("ADDSBRPANE")> _
    Public Shared Sub AddStatusBarPane()

        ' Crear un nuevo objeto Pane
        Dim acPane As Pane = New Pane

        ' Texto del panel
        acPane.Text = "Hola " & _PCount.ToString
        ' Estilo
        acPane.Style = PaneStyles.Normal
        ' Icono
        'acPane.Icon = Application.MainWindow.Icon

        ' Añadir un controlador de evento
        AddHandler acPane.MouseDown, AddressOf callbackPaneMouseDown

        ' Añadir el objeto a la coleccion
        Application.StatusBar.Panes.Add(acPane)

        ' Actualizar la barra de estado
        Application.StatusBar.Update()

        _PCount += 1

    End Sub

    Private Shared Sub callbackPaneMouseDown(ByVal sender As Object, _
                                             ByVal e As StatusBarMouseDownEventArgs)

        Dim acPane As Pane = DirectCast(sender, Pane)
        acPane.Style = If(acPane.Style = PaneStyles.PopOut, PaneStyles.Normal, PaneStyles.PopOut)

        Application.StatusBar.Update()

    End Sub

    'Mostrar una ventana de notificacion en el area de notificacion de la barra de estado
    <CommandMethod("SNBB")> _
    Public Sub statusBarBalloon()

        ' Crear un nuevo elemento del area de notificacion de la barra estado 
        Dim acTItm As New TrayItem()

        acTItm.ToolTipText = "Nuevo elemento" & ControlChars.Cr & "del area de notificación"
        ' acTItm.Icon = Application.MainWindow.Icon
        ' Añadir el elemento al area de notificacion
        Application.StatusBar.TrayItems.Add(acTItm)

        ' Crear una ventana de notificacion para el 
        ' elemento del area de notificacion de la barra de estado
        Dim acBbw As New TrayItemBubbleWindow()

        With acBbw
            .Title = "Notificacion de prueba"
            .Text = "Para obtener mas información:"
            .HyperText = "Enlace a mi pagina"
            .HyperLink = "www.mipagina.eu"
            .Text2 = "blablablabla"
            .IconType = IconType.Warning
        End With

        ' Mostar la ventana de notificacion
        acTItm.ShowBubbleWindow(acBbw)

        Application.StatusBar.Update()

        AddHandler acBbw.Closed, Function(o, args) AnonymousMethod(o, args, acTItm)

    End Sub

    Private Function AnonymousMethod(ByVal o As Object, _
                                     ByVal args As TrayItemBubbleWindowClosedEventArgs, _
                                     ByVal ti As TrayItem) As Object

        Try
            Application.StatusBar.TrayItems.Remove(ti)
            Application.StatusBar.Update()

        Catch
        End Try

        Return Nothing
    End Function

    'XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX






End Class





