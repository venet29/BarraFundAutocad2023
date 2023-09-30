
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.ApplicationServices
Imports System.Math
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.EditorInput
Imports VARIOS


Imports System.IO
Imports ESTRIBOS_MUROS
Imports AUX_FUNDACIONES

Public Class M1
    Public espac_pata_caja As Integer = 2
    Public espac_pata_fundacion As Integer = 5
    Public espac_borde As Single = 3.5
    'Public cots As String = "\\SERVER-CDV\Dibujo\Proyectos\BIBLIOTECA\"
    Public cots As String = "C:\\"
    Public Shared grupo_referencia As String = "*A2867"
    'Public cots As String = "C:\"
    Dim db As New Database(False, True)
    Dim _ManejadorUsuarios As New ManejadorDatos()

    Public Sub aux__barra(ByVal cbx_dia_princiapl As Integer, ByVal ckbx_barra_refuerzo As Boolean, ByVal rbt_inferior As Boolean, ByVal cbx_dia_principal As String, ByVal cbx_sepa_princiapl As String, ByVal ckbx_traslapo As Boolean, ByVal casos_dibujar_ As String, ByVal grupo_referencia_ As String, ByVal txt_recub As String)
        ' Obtener el documento y la base de datos actuales
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        Dim acDocEd As Editor = Application.DocumentManager.MdiActiveDocument.Editor
        Dim utiles_aux As New utiles
        Using acDoc.LockDocument()
            Application.SetSystemVariable("clayer", "0")

            'Dim _acSSet_LINEA As SelectionSet

            Dim acObjIdColl_borra_v2 As ObjectIdCollection = New ObjectIdCollection()
            Dim aux_x As Single = -1100
            Dim aux_y As Single = -100

            Dim pPtRes As PromptPointResult
            Dim pPtOpts As PromptPointOptions = New PromptPointOptions("")

            ' Solicitar el punto final
            pPtOpts.Message = String.Format("{0}1)Precise el punto Inicial de Barra: ", ControlChars.Lf)
            'pPtOpts.UseBasePoint = True

            pPtRes = acDoc.Editor.GetPoint(pPtOpts)
            If pPtRes.Status = PromptStatus.Cancel Then Exit Sub
            Dim pt As Point3d = pPtRes.Value



            pPtOpts.Message = String.Format("{0}2)Precise el punto final Barra: ", ControlChars.Lf)
            pPtOpts.UseBasePoint = True
            pPtOpts.BasePoint = pt
            pPtRes = acDoc.Editor.GetPoint(pPtOpts)
            If pPtRes.Status = PromptStatus.Cancel Then Exit Sub
            Dim pt2 As Point3d = pPtRes.Value


            'acDoc.Window.WindowState = Window.State.Maximized
            'acDoc.Window.WindowState = Window.State.Normal
            'Dim ptDoc As System.Windows.Point = New System.Windows.Point(0, 0)
            'acDoc.Window.DeviceIndependentLocation = ptDoc


            Dim patalargo

            Select Case cbx_dia_princiapl

                Case 8
                    patalargo = 20
                Case 10
                    patalargo = CStr(20)
                Case 12
                    patalargo = CStr(20)
                Case 16
                    patalargo = CStr(20)
                Case 18
                    patalargo = CStr(25)
                Case 22
                    patalargo = CStr(30)
                Case 25
                    patalargo = CStr(40)
                Case 28
                    patalargo = CStr(40)
                Case 32
                    patalargo = CStr(50)
                Case Else
                    patalargo = CStr(50)
            End Select


            acObjIdColl_borra_v2.Clear()
            'UpdateTransientGraphics(pt)

            Dim _currentPoint As Point3d = pt
            ' Iniciar una transaccion


            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'comprobar bloque

            Dim flecha As String = "_SCAD_CUANTIA_FUND_NH2"
            Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()

                ' Abrir la tabla para bloques en modo lectura
                Dim acBlkTbl As BlockTable = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead)

                If Not acBlkTbl.Has(flecha) Then
                    'Else
                    Dim VARIOS_ As New atributos()
                    If File.Exists("C:\Program Files\AutocadNh\_SCAD_CUANTIA_FUND_NH.dwg") Then
                        VARIOS_.InsertBlock("C:\Program Files\AutocadNh\_SCAD_CUANTIA_FUND_NH.dwg", flecha)
                    End If

                    If Not acBlkTbl.Has("_SCAD_CUANTIA_FUND_NH2") Then
                        MsgBox("Insertar bloque de texto '_SCAD_CUANTIA_FUND_NH2'", vbCritical)
                        GoTo final
                    End If


                End If

                '' Comprobar si el bloque existe
                'If acBlkTbl.Has(flecha) Then

                'Else


                'End If
                acTrans.Commit()
            End Using

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

            Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()


                Dim coordenada_PTO_(1) As Point3d
                'coordenada_PTO_ = coordenada_modificar_fun(Nothing, pt, pt2)    ' OBTENER P1 Y P2 ORDENADOS
                'pt = coordenada_PTO_(0)
                'pt2 = coordenada_PTO_(1)
                Dim ANGLE As Double = utiles_aux.coordenada__angulo_p1_p2_fun(pt, pt2, acDocEd, Nothing)
                Dim acPoly As Polyline = New Polyline()

                ' Abrir la tabla de bloques en modo lectura
                Dim acBlkTbl As BlockTable = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead)
                ' Abrir el registro del bloque de Espacio Modelo en modo escritura
                Dim acBlkTblRec As BlockTableRecord
                acBlkTblRec = acTrans.GetObject(acBlkTbl(BlockTableRecord.ModelSpace), OpenMode.ForWrite)


                Dim acPolyObj As ObjectId = acPoly.ObjectId
                acPoly.SetDatabaseDefaults()
                acPoly.AddVertexAt(0, New Point2d(pt.X, pt.Y), 0, 0, 0)
                acPoly.AddVertexAt(1, New Point2d(pt2.X, pt2.Y), 0, 0, 0)
                acPoly.Layer = "BARRAS"

                ' Añadir el nuevo objeto al registro de la tabla para bloques y a la
                ' transaccion
                acBlkTblRec.AppendEntity(acPoly)
                acTrans.AddNewlyCreatedDBObject(acPoly, True)



                '' Abrir la tabla para bloques en modo lectura
                Dim ents As ObjectIdCollection = New ObjectIdCollection()
                acObjIdColl_borra_v2.Add(acPoly.ObjectId)
                'dibujar linea
                'Dim acLine As Line = PLANTA_.dibujar_linea_pl(Nothing, pt, pt2, "0")
                'Dim acline_obj As Object = ultimo_elemto()

                Dim pMin As Point3d
                Dim pMax As Point3d
                Using acView As ViewTableRecord = acDoc.Editor.GetCurrentView()
                    pMin = New Point3d(Application.GetSystemVariable("viewctr").X - (acView.Width / 2), Application.GetSystemVariable("viewctr").Y - (acView.Height / 2), 0)
                    pMax = New Point3d((acView.Width / 2) + Application.GetSystemVariable("viewctr").X, (acView.Height / 2) + Application.GetSystemVariable("viewctr").Y, 0)
                End Using


                'crear entidad
                Dim ent As Entity = DirectCast(acTrans.GetObject(acPoly.ObjectId, OpenMode.ForRead), Entity)
                If ent Is Nothing Then Return
                Dim cur As Curve = TryCast(ent, Curve)

                Dim values_sup(1) As Point3d
                values_sup(0) = New Point3d(pt.X, pt.Y, 0)
                values_sup(1) = New Point3d(pt2.X, pt2.Y, 0)
                ' Dim Fence_sup As New Point3dCollection(values_sup)


                Dim acTypValAr(4) As TypedValue
                acTypValAr.SetValue(New TypedValue(DxfCode.Operator, "<or"), 0)
                acTypValAr.SetValue(New TypedValue(DxfCode.Start, "LWPOLYLINE"), 1)
                acTypValAr.SetValue(New TypedValue(DxfCode.Start, "LINE"), 2)
                acTypValAr.SetValue(New TypedValue(DxfCode.Operator, "or>"), 3)
                acTypValAr.SetValue(New TypedValue(DxfCode.LayerName, "FUNDACIONES"), 4)
                Dim acSelFtr As SelectionFilter = New SelectionFilter(acTypValAr)

                ' Crear un área de selección definida por (2,2,0) y (10,8,0)
                Dim acSSPrompt2 As PromptSelectionResult = acDocEd.SelectCrossingWindow(values_sup(0), values_sup(1), acSelFtr)

                Dim recubrimeinto As Single = txt_recub


                utiles_aux.Zoom(pMin, pMax, New Point3d(), 1)

                Dim ds_tabla As New DataSet
                Dim dt_tabla As New System.Data.DataTable

                'Dim tabla As New System.Data.DataTable
                dt_tabla.Columns.Add("xx", GetType(Single))
                dt_tabla.Columns.Add("yy", GetType(Single))
                dt_tabla.Columns.Add("largo_menor", GetType(Single))
                dt_tabla.Columns.Add("espesor", GetType(Single))
                Dim values_inter(3) As Point3d

                Dim _pto_barra As Point3d
                Dim fundacion_ As New FUNDACIONES
                Dim direccion As String

                Dim i As Integer = 0
                Dim texto_cuantia As String

                Dim aux_refeurzo_losa As String = ""
                If ckbx_barra_refuerzo = True Then
                    aux_refeurzo_losa = "+"
                End If


                If rbt_inferior = True Then
                    texto_cuantia = "F=" & "%%c" & cbx_dia_principal & "a" & cbx_sepa_princiapl
                Else
                    texto_cuantia = "F'=" & "%%c" & cbx_dia_principal & "a" & cbx_sepa_princiapl
                End If

                utiles_aux.Create_ALayer("BARRAS")
                utiles_aux.Create_ALayer("TEXTO")


                ' Si el estado de la solicitud es OK, es que se han seleccionado objetos
                If acSSPrompt2.Status = PromptStatus.OK Then
                    Dim acSSet As SelectionSet = acSSPrompt2.Value



                    ' Recorrer los objetos del conjunto de selección
                    For Each acSSObj As SelectedObject In acSSet
                        ' Comprobar que se ha devuelto un objeto valido
                        If Not IsDBNull(acSSObj) Then
                            ' Abrir el objeto para escritura
                            Dim excur As Entity = acTrans.GetObject(acSSObj.ObjectId, OpenMode.ForWrite)

                            If Not IsDBNull(excur) Then

                                Dim pts As New Point3dCollection()
                                cur.IntersectWith(excur, Intersect.OnBothOperands, pts, 0, 0)

                                Dim datos(1) As String
                                For k = 0 To pts.Count - 1


                                    dt_tabla.Rows.Add(pts(k).X, pts(k).Y, datos(0), datos(1))
                                Next

                            End If
                        End If
                    Next


                    Dim pto1_barra As Point2d
                    Dim pto2_barra As Point2d
                    Dim pto1_RANGO As Point2d
                    Dim pto2_RANGO As Point2d

                    If dt_tabla.Rows.Count > 2 Then

                        Dim AUX_pto1_ As Point3d
                        Dim AUX_pto2_ As Point3d

                        Dim largoInicial As Single = 100000
                        Dim largoFinal As Single = 100000

                        For Each linea In dt_tabla.Rows

                            If pt.DistanceTo(New Point3d(Convert.ToSingle(linea(0)), Convert.ToSingle(linea(1)), 0)) < largoInicial Then
                                AUX_pto1_ = New Point3d(Convert.ToSingle(linea(0)), Convert.ToSingle(linea(1)), 0)
                                largoInicial = pt.DistanceTo(New Point3d(Convert.ToSingle(linea(0)), Convert.ToSingle(linea(1)), 0))
                            End If


                            If pt2.DistanceTo(New Point3d(Convert.ToSingle(linea(0)), Convert.ToSingle(linea(1)), 0)) < largoFinal Then
                                AUX_pto2_ = New Point3d(Convert.ToSingle(linea(0)), Convert.ToSingle(linea(1)), 0)
                                largoFinal = pt2.DistanceTo(New Point3d(Convert.ToSingle(linea(0)), Convert.ToSingle(linea(1)), 0))
                            End If

                        Next

                        dt_tabla.Rows.Clear()
                        dt_tabla.Rows.Add(AUX_pto1_.X, AUX_pto1_.Y, 0, 0)
                        dt_tabla.Rows.Add(AUX_pto2_.X, AUX_pto2_.Y, 0, 0)

                    End If


                    If dt_tabla.Rows.Count = 2 Then
                        Dim caso_tipo As String = ""
                        If rbt_inferior = True Then
                            caso_tipo = "f10"
                        Else
                            caso_tipo = "f9a"
                        End If
                        If Abs(pt.X - pt2.X) < 0.2 Then ' vertical
                            direccion = "vertical_b"

                        Else ' horizontal
                            direccion = "horizontal_i"

                        End If

                        'If rbt_inferior = True Then texto_cuantia = "F=" & texto_cuantia

                        texto_cuantia = aux_refeurzo_losa & texto_cuantia

                        coordenada_PTO_ = utiles_aux.coordenada_modificar_fun(Nothing, New Point3d(dt_tabla(0)(0), dt_tabla(0)(1), 0), New Point3d(dt_tabla(1)(0), dt_tabla(1)(1), 0))    ' OBTENER P1 Y P2 ORDENADOS
                        pt = coordenada_PTO_(0)
                        pt2 = coordenada_PTO_(1)


                        pto1_barra = New Point2d(pt.X + Cos(ANGLE) * recubrimeinto, pt.Y + Sin(ANGLE) * recubrimeinto)
                        pto2_barra = New Point2d(pt2.X - Cos(ANGLE) * recubrimeinto, pt2.Y - Sin(ANGLE) * recubrimeinto)
                        fundacion_.modificar_barra_FUNDA(ents, ent, pto1_barra, pto2_barra, caso_tipo, direccion, 18, patalargo)

                        fundacion_.dibujar_texto_bloque_fund(pt.X + Cos(ANGLE) * 20, pt.Y + Sin(ANGLE) * 20, ANGLE, ents, "L=" & fundacion_.largo_barra_real_fun, texto_cuantia, fundacion_.largo_barra_parcial_fun)
                        pto1_RANGO = fundacion_.punto_dimension1
                        pto2_RANGO = fundacion_.punto_dimension2
                        'If My1Commands.myPalette.ckbx_traslapo.Checked = True Then
                        '    fundacion_.dibujar_dimesion_funda(ents, Nothing, Nothing, ANGLE)
                        '    _pto_barra = fundacion_.intesrcion_barra_dimension(New Point2d(pt.X + Cos(ANGLE) * recubrimeinto, pt.Y + Sin(ANGLE) * recubrimeinto), New Point2d(pt2.X - Cos(ANGLE) * recubrimeinto, pt2.Y - Sin(ANGLE) * recubrimeinto), fundacion_.punto_dimension1, fundacion_.punto_dimension2)
                        '    fundacion_.dibujar_circulo_fund(ents, New Point3d(_pto_barra.X - Cos(ANGLE) * 10, _pto_barra.Y - Sin(ANGLE) * 10, 0), My1Commands.casos_dibujar, ANGLE)
                        'End If
                    ElseIf dt_tabla.Rows.Count = 1 Then


                        Dim caso_tipo As String = ""
                        'pt = New Point3d(dt_tabla(0)(0), dt_tabla(0)(1), 0)
                        'pt2 = pt2
                        If rbt_inferior = True Then
                            caso_tipo = "f11"
                        Else
                            caso_tipo = "f9a_V"
                        End If

                        If Abs(pt.X - pt2.X) < 0.2 Then ' vertical

                            If pt.Y < pt2.Y Then
                                direccion = "vertical_b"
                                coordenada_PTO_ = utiles_aux.coordenada_modificar_fun(Nothing, New Point3d(dt_tabla(0)(0), dt_tabla(0)(1), 0), pt2)
                            Else
                                direccion = "vertical_a"
                                coordenada_PTO_ = utiles_aux.coordenada_modificar_fun(Nothing, New Point3d(dt_tabla(0)(0), dt_tabla(0)(1), 0), pt2)
                            End If

                        Else ' horizontal
                            If pt.X < pt2.X Then
                                direccion = "horizontal_i"
                                coordenada_PTO_ = utiles_aux.coordenada_modificar_fun(Nothing, New Point3d(dt_tabla(0)(0), dt_tabla(0)(1), 0), pt2)
                            Else
                                direccion = "horizontal_d"
                                coordenada_PTO_ = utiles_aux.coordenada_modificar_fun(Nothing, New Point3d(dt_tabla(0)(0), dt_tabla(0)(1), 0), pt2)
                            End If
                        End If

                        texto_cuantia = aux_refeurzo_losa & texto_cuantia
                        ' OBTENER P1 Y P2 ORDENADOS
                        pt = coordenada_PTO_(0)
                        pt2 = coordenada_PTO_(1)

                        fundacion_.modificar_barra_FUNDA(ents, ent, New Point2d(pt.X + Cos(ANGLE) * recubrimeinto, pt.Y + Sin(ANGLE) * recubrimeinto), New Point2d(pt2.X - Cos(ANGLE) * recubrimeinto, pt2.Y - Sin(ANGLE) * recubrimeinto), caso_tipo, direccion, 18, patalargo)



                        If direccion = "horizontal_i" Or direccion = "vertical_b" Then
                            fundacion_.dibujar_texto_bloque_fund(pt.X + Cos(ANGLE) * 20, pt.Y + Sin(ANGLE) * 20, ANGLE, ents, "L=" & fundacion_.largo_barra_real_fun, texto_cuantia, fundacion_.largo_barra_parcial_fun)
                        Else
                            fundacion_.dibujar_texto_bloque_fund(pt2.X - Cos(ANGLE) * 20, pt2.Y - Sin(ANGLE) * 20, ANGLE, ents, "L=" & fundacion_.largo_barra_real_fun, texto_cuantia, fundacion_.largo_barra_parcial_fun)
                        End If




                    ElseIf dt_tabla.Rows.Count = 0 Then
                        GoTo salto2
                        'Dim caso_tipo As String = ""
                        'caso_tipo = "f3"
                        'If Abs(pt.X - pt2.X) < 0.2 Then ' vertical
                        '    direccion = "vertical_b"
                        'Else ' horizontal
                        '    direccion = "horizontal_i"
                        'End If

                        'coordenada_PTO_ = coordenada_modificar_fun(Nothing, pt, pt2)
                        'pt = coordenada_PTO_(0)
                        'pt2 = coordenada_PTO_(1)
                        'texto_cuantia = aux_refeurzo_losa & texto_cuantia
                        'fundacion_.dibujar_texto_bloque_fund(pt.X + Cos(ANGLE) * 20, pt.Y + Sin(ANGLE) * 20, ANGLE, ents, "L=" & fundacion_.largo_barra_real_fun, texto_cuantia, fundacion_.largo_barra_parcial_fun)

                    ElseIf dt_tabla.Rows.Count <> 0 Then
                        acPoly.Erase()
                        GoTo salto1
                    End If
                    If ckbx_traslapo = True Then
                        fundacion_.dibujar_dimesion_funda(ents, Nothing, Nothing, ANGLE)
                        _pto_barra = fundacion_.intesrcion_barra_dimension(New Point2d(pt.X + Cos(ANGLE) * recubrimeinto, pt.Y + Sin(ANGLE) * recubrimeinto), New Point2d(pt2.X - Cos(ANGLE) * recubrimeinto, pt2.Y - Sin(ANGLE) * recubrimeinto), fundacion_.punto_dimension1, fundacion_.punto_dimension2)
                        fundacion_.dibujar_circulo_fund(ents, New Point3d(_pto_barra.X - Cos(ANGLE) * 10, _pto_barra.Y - Sin(ANGLE) * 10, 0), casos_dibujar_, ANGLE)
                    End If

                    Dim split_ As String() = Replace(LCase("%%c" & cbx_dia_principal & "a" & cbx_sepa_princiapl), "%%c", "").Split(New [Char]() {"a"c, CChar(vbTab)})
                    Dim VARIOS_ As New CODIGOS_DATOS()
                    VARIOS_.addData_PROG_LOSA(ent.ObjectId, fundacion_.tipo, fundacion_.punto_final, fundacion_.punto_inicial, split_(0), fundacion_.largo_barra_real_fun, 0, split_(1), fundacion_.orientacion_, fundacion_.punto_e_losa_real, fundacion_.punto_e_losa)

                    Dim GRUPO_ As New CODIGOS_GRUPOS()
                    'crear grupo
                    GRUPO_.creacion_grupo(ents, grupo_referencia_)

salto1:
                Else
salto2:
                    Dim caso_tipo As String = ""
                    caso_tipo = "f3"
                    If Abs(pt.X - pt2.X) < 0.2 Then ' vertical
                        direccion = "vertical_b"
                    Else ' horizontal
                        direccion = "horizontal_i"
                    End If
                    coordenada_PTO_ = utiles_aux.coordenada_modificar_fun(Nothing, pt, pt2)    ' OBTENER P1 Y P2 ORDENADOS
                    pt = coordenada_PTO_(0)
                    pt2 = coordenada_PTO_(1)

                    texto_cuantia = aux_refeurzo_losa & texto_cuantia
                    fundacion_.dibujar_texto_bloque_fund(pt.X + Cos(ANGLE) * 20, pt.Y + Sin(ANGLE) * 20, ANGLE, ents, "L=" & Round(pt.DistanceTo(pt2), 0), texto_cuantia, "")

                    If ckbx_traslapo = True Then
                        fundacion_.dibujar_dimesion_funda(ents, Nothing, Nothing, ANGLE)
                        _pto_barra = fundacion_.intesrcion_barra_dimension(New Point2d(pt.X + Cos(ANGLE) * recubrimeinto, pt.Y + Sin(ANGLE) * recubrimeinto), New Point2d(pt2.X - Cos(ANGLE) * recubrimeinto, pt2.Y - Sin(ANGLE) * recubrimeinto), fundacion_.punto_dimension1, fundacion_.punto_dimension2)
                        fundacion_.dibujar_circulo_fund(ents, New Point3d(_pto_barra.X - Cos(ANGLE) * 10, _pto_barra.Y - Sin(ANGLE) * 10, 0), casos_dibujar_, ANGLE)
                    End If

                    ents.Add(ent.ObjectId)

                    Dim split_ As String() = Replace(LCase("%%c" & cbx_dia_principal & "a" & cbx_sepa_princiapl), "%%c", "").Split(New [Char]() {"a"c, CChar(vbTab)})
                    Dim VARIOS_ As New CODIGOS_DATOS()
                    '  VARIOS_.addData_PROG_LOSA(ent.ObjectId, fundacion_.tipo, fundacion_.punto_final, fundacion_.punto_inicial, split_(0), fundacion_.largo_barra_real_fun, 0, split_(1), fundacion_.orientacion_, fundacion_.punto_e_losa_real, fundacion_.punto_e_losa)
                    VARIOS_.addData_PROG_LOSA(ent.ObjectId, caso_tipo, New Point2d(pt2.X, pt2.Y), New Point2d(pt.X, pt.Y), split_(0), pt.DistanceTo(pt2), 0, split_(1), direccion, 0, 0)
                    Dim GRUPO_ As New CODIGOS_GRUPOS()
                    'crear grupo
                    GRUPO_.creacion_grupo(ents, grupo_referencia_)

                End If

                acTrans.Commit()
                Application.DocumentManager.MdiActiveDocument.Editor.UpdateScreen()
                Application.DocumentManager.MdiActiveDocument.Editor.Regen()
            End Using
final:
        End Using
    End Sub


    'busca y dibuja segun la intersecciones q encuentre
    Public Sub aux__barra_manual(ByRef pt As Point3d, ByRef pt2 As Point3d, ByRef pt_rango As Point3d, ByRef pt2_rango As Point3d, ByVal CUANTIA As String, ByVal direccion As String, ByVal caso_tipo As String, ByVal recubrimeinto As String, ByVal ckbx_traslapo As Boolean, ByVal grupo_referencia_ As String, ByVal casos_dibujar_ As String)
        ' Obtener el documento y la base de datos actuales
        Dim utiles_aux As New utiles
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        Dim acDocEd As Editor = Application.DocumentManager.MdiActiveDocument.Editor
        ' Dim acadObject As Object = Application.AcadApplication
        Using acDoc.LockDocument()


            'Dim _acSSet_LINEA As SelectionSet

            Dim acObjIdColl_borra_v2 As ObjectIdCollection = New ObjectIdCollection()
            Dim aux_x As Single = -1100
            Dim aux_y As Single = -100

            acObjIdColl_borra_v2.Clear()
            'UpdateTransientGraphics(pt)



            Dim _currentPoint As Point3d = pt
            ' Iniciar una transaccion
            Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()


                Dim coordenada_PTO_(1) As Point3d
                'coordenada_PTO_ = coordenada_modificar_fun(Nothing, pt, pt2)    ' OBTENER P1 Y P2 ORDENADOS
                'pt = coordenada_PTO_(0)
                'pt2 = coordenada_PTO_(1)
                Dim ANGLE As Double = utiles_aux.coordenada__angulo_p1_p2_fun(pt, pt2, acDocEd, Nothing)
                Dim acPoly As Polyline = New Polyline()

                ' Abrir la tabla de bloques en modo lectura
                Dim acBlkTbl As BlockTable = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead)
                ' Abrir el registro del bloque de Espacio Modelo en modo escritura
                Dim acBlkTblRec As BlockTableRecord
                acBlkTblRec = acTrans.GetObject(acBlkTbl(BlockTableRecord.ModelSpace), OpenMode.ForWrite)


                Dim acPolyObj As ObjectId = acPoly.ObjectId
                acPoly.SetDatabaseDefaults()
                acPoly.AddVertexAt(0, New Point2d(pt.X, pt.Y), 0, 0, 0)
                acPoly.AddVertexAt(1, New Point2d(pt2.X, pt2.Y), 0, 0, 0)
                acPoly.Layer = "BARRAS"

                ' Añadir el nuevo objeto al registro de la tabla para bloques y a la
                ' transaccion
                acBlkTblRec.AppendEntity(acPoly)
                acTrans.AddNewlyCreatedDBObject(acPoly, True)



                '' Abrir la tabla para bloques en modo lectura
                Dim ents As ObjectIdCollection = New ObjectIdCollection()
                acObjIdColl_borra_v2.Add(acPoly.ObjectId)
                'dibujar linea
                'Dim acLine As Line = PLANTA_.dibujar_linea_pl(Nothing, pt, pt2, "0")
                'Dim acline_obj As Object = ultimo_elemto()



                'crear entidad
                Dim ent As Entity = DirectCast(acTrans.GetObject(acPoly.ObjectId, OpenMode.ForRead), Entity)
                If ent Is Nothing Then Return
                Dim cur As Curve = TryCast(ent, Curve)


                '**********************
                ' acadObject.ZoomExtents()
                Dim values_sup(1) As Point3d
                values_sup(0) = New Point3d(pt.X, pt.Y, 0)
                values_sup(1) = New Point3d(pt2.X, pt2.Y, 0)
                ' Dim Fence_sup As New PoinacTypValAr.SetValue(New TypedValue(DxfCode.LayerLinetype, "CONTINUOUS"), 5)t3dCollection(values_sup)

                Dim acTypValAr(4) As TypedValue

                acTypValAr.SetValue(New TypedValue(DxfCode.Operator, "<or"), 0)
                acTypValAr.SetValue(New TypedValue(DxfCode.Start, "LWPOLYLINE"), 1)
                acTypValAr.SetValue(New TypedValue(DxfCode.Start, "LINE"), 2)
                acTypValAr.SetValue(New TypedValue(DxfCode.Operator, "or>"), 3)
                acTypValAr.SetValue(New TypedValue(DxfCode.LayerName, "FUNDACIONES"), 4)

                Dim acSelFtr As SelectionFilter = New SelectionFilter(acTypValAr)

                ' Crear un área de selección definida por (2,2,0) y (10,8,0)
                Dim acSSPrompt2 As PromptSelectionResult = acDocEd.SelectCrossingWindow(values_sup(0), values_sup(1), acSelFtr)

                '***************
                Dim ds_tabla As New DataSet
                Dim dt_tabla As New System.Data.DataTable

                'Dim tabla As New System.Data.DataTable
                dt_tabla.Columns.Add("xx", GetType(Single))
                dt_tabla.Columns.Add("yy", GetType(Single))
                dt_tabla.Columns.Add("largo_menor", GetType(Single))
                dt_tabla.Columns.Add("espesor", GetType(Single))
                Dim values_inter(3) As Point3d
                Dim i As Integer = 0
                Dim fundacion_ As New FUNDACIONES
                Dim _pto_barra As Point3d
                ' Si el estado de la solicitud es OK, es que se han seleccionado objetos
                If acSSPrompt2.Status = PromptStatus.OK Then
                    Dim acSSet As SelectionSet = acSSPrompt2.Value


                    ' Recorrer los objetos del conjunto de selección
                    For Each acSSObj As SelectedObject In acSSet
                        ' Comprobar que se ha devuelto un objeto valido
                        If Not IsDBNull(acSSObj) Then
                            ' Abrir el objeto para escritura
                            Dim excur As Entity = acTrans.GetObject(acSSObj.ObjectId, OpenMode.ForWrite)

                            If Not IsDBNull(excur) Then

                                Dim pts As New Point3dCollection()
                                cur.IntersectWith(excur, Intersect.OnBothOperands, pts, 0, 0)

                                Dim datos(1) As String
                                For k = 0 To pts.Count - 1


                                    Dim salir As Boolean = False
                                    Dim row As DataRow
                                    For Each row In dt_tabla.Rows
                                        If Math.Abs(row.ItemArray(0) - pts(k).X) < 0.1 And Math.Abs(row.ItemArray(1) - pts(k).Y) < 0.1 Then
                                            salir = True
                                        End If
                                    Next


                                    If salir = True Then Continue For
                                    dt_tabla.Rows.Add(pts(k).X, pts(k).Y, datos(0), datos(1))
                                Next

                            End If
                        End If
                    Next



                    Dim pto1_barra As Point2d
                    Dim pto2_barra As Point2d
                    Dim pto1_RANGO As Point2d
                    Dim pto2_RANGO As Point2d



                    If dt_tabla.Rows.Count = 2 Then
                        'Dim caso_tipo As String = ""
                        'If My1Commands.myPalette.rbt_inferior.Checked = True Then
                        '    caso_tipo = "f10"
                        'Else
                        '    caso_tipo = "f9a"
                        'End If
                        coordenada_PTO_ = utiles_aux.coordenada_modificar_fun(Nothing, New Point3d(dt_tabla(0)(0), dt_tabla(0)(1), 0), New Point3d(dt_tabla(1)(0), dt_tabla(1)(1), 0))    ' OBTENER P1 Y P2 ORDENADOS
                        pt = coordenada_PTO_(0)
                        pt2 = coordenada_PTO_(1)


                        pto1_barra = New Point2d(pt.X + Cos(ANGLE) * recubrimeinto, pt.Y + Sin(ANGLE) * recubrimeinto)
                        pto2_barra = New Point2d(pt2.X - Cos(ANGLE) * recubrimeinto, pt2.Y - Sin(ANGLE) * recubrimeinto)
                        fundacion_.modificar_barra_FUNDA(ents, ent, pto1_barra, pto2_barra, caso_tipo, "", 18, 15)


                        fundacion_.dibujar_texto_bloque_fund(pt.X + Cos(ANGLE) * 20, pt.Y + Sin(ANGLE) * 20, ANGLE, ents, "L=" & fundacion_.largo_barra_real_fun, CUANTIA, fundacion_.largo_barra_parcial_fun)
                        pto1_RANGO = fundacion_.punto_dimension1
                        pto2_RANGO = fundacion_.punto_dimension2
                        'If My1Commands.myPalette.ckbx_traslapo.Checked = True Then
                        '    fundacion_.dibujar_dimesion_funda(ents, Nothing, Nothing, ANGLE)
                        '    _pto_barra = fundacion_.intesrcion_barra_dimension(New Point2d(pt.X + Cos(ANGLE) * recubrimeinto, pt.Y + Sin(ANGLE) * recubrimeinto), New Point2d(pt2.X - Cos(ANGLE) * recubrimeinto, pt2.Y - Sin(ANGLE) * recubrimeinto), fundacion_.punto_dimension1, fundacion_.punto_dimension2)
                        '    fundacion_.dibujar_circulo_fund(ents, New Point3d(_pto_barra.X - Cos(ANGLE) * 10, _pto_barra.Y - Sin(ANGLE) * 10, 0), My1Commands.casos_dibujar, ANGLE)
                        'End If
                    ElseIf dt_tabla.Rows.Count = 1 Then


                        'Dim caso_tipo As String = ""
                        ''pt = New Point3d(dt_tabla(0)(0), dt_tabla(0)(1), 0)
                        ''pt2 = pt2
                        'If tipo_barra <> "f9a_V" Then
                        '    caso_tipo = "f11"
                        'Else
                        '    caso_tipo = "f9a_V"
                        'End If


                        coordenada_PTO_ = utiles_aux.coordenada_modificar_fun(Nothing, New Point3d(dt_tabla(0)(0), dt_tabla(0)(1), 0), pt2)

                        ' OBTENER P1 Y P2 ORDENADOS
                        pt = coordenada_PTO_(0)
                        pt2 = coordenada_PTO_(1)

                        fundacion_.modificar_barra_FUNDA(ents, ent, New Point2d(pt.X + Cos(ANGLE) * recubrimeinto, pt.Y + Sin(ANGLE) * recubrimeinto), New Point2d(pt2.X - Cos(ANGLE) * recubrimeinto, pt2.Y - Sin(ANGLE) * recubrimeinto), caso_tipo, direccion, 18, 15)


                        If direccion = "horizontal_i" Or direccion = "vertical_b" Then
                            fundacion_.dibujar_texto_bloque_fund(pt.X + Cos(ANGLE) * 20, pt.Y + Sin(ANGLE) * 20, ANGLE, ents, "L=" & fundacion_.largo_barra_real_fun, CUANTIA, fundacion_.largo_barra_parcial_fun)
                        Else
                            fundacion_.dibujar_texto_bloque_fund(pt2.X - Cos(ANGLE) * 20, pt2.Y - Sin(ANGLE) * 20, ANGLE, ents, "L=" & fundacion_.largo_barra_real_fun, CUANTIA, fundacion_.largo_barra_parcial_fun)
                        End If




                    ElseIf dt_tabla.Rows.Count = 0 Then
                        GoTo salto2
                        'caso_tipo = "f3"
                        'coordenada_PTO_ = coordenada_modificar_fun(Nothing, New Point3d(dt_tabla(0)(0), dt_tabla(0)(1), 0), New Point3d(dt_tabla(1)(0), dt_tabla(1)(1), 0))    ' OBTENER P1 Y P2 ORDENADOS
                        'pt = coordenada_PTO_(0)
                        'pt2 = coordenada_PTO_(1)

                        'fundacion_.dibujar_texto_bloque_fund(pt.X + Cos(ANGLE) * 20, pt.Y + Sin(ANGLE) * 20, ANGLE, ents, "L=" & fundacion_.largo_barra_real_fun, CUANTIA, fundacion_.largo_barra_parcial_fun)
                        'fundacion_.object_poly = ent.ObjectId
                    ElseIf dt_tabla.Rows.Count <> 0 Then
                        acPoly.Erase()
                        GoTo salto1
                    End If
                    If ckbx_traslapo = True Then
                        fundacion_.dibujar_dimesion_funda(ents, pt_rango, pt2_rango, ANGLE)
                        _pto_barra = fundacion_.intesrcion_barra_dimension(New Point2d(pt.X + Cos(ANGLE) * recubrimeinto, pt.Y + Sin(ANGLE) * recubrimeinto), New Point2d(pt2.X - Cos(ANGLE) * recubrimeinto, pt2.Y - Sin(ANGLE) * recubrimeinto), fundacion_.punto_dimension1, fundacion_.punto_dimension2)
                        fundacion_.dibujar_circulo_fund(ents, New Point3d(_pto_barra.X - Cos(ANGLE) * 10, _pto_barra.Y - Sin(ANGLE) * 10, 0), casos_dibujar_, ANGLE)
                    End If

                    Dim split_ As String() = Replace(Replace(Replace(LCase(CUANTIA), "f=", ""), "f'=", ""), "%%c", "").Split(New [Char]() {"a"c, CChar(vbTab)})
                    Dim VARIOS_ As New CODIGOS_DATOS()
                    VARIOS_.addData_PROG_LOSA(fundacion_.object_poly, fundacion_.tipo, fundacion_.punto_final, fundacion_.punto_inicial, split_(0), fundacion_.largo_barra_real_fun, 0, split_(1), fundacion_.orientacion_, fundacion_.punto_e_losa_real, fundacion_.punto_e_losa)

                    Dim GRUPO_ As New CODIGOS_GRUPOS()
                    'crear grupo
                    GRUPO_.creacion_grupo(ents, grupo_referencia_)

salto1:
                Else
salto2:
                    caso_tipo = "f3"
                    coordenada_PTO_ = utiles_aux.coordenada_modificar_fun(Nothing, pt, pt2)    ' OBTENER P1 Y P2 ORDENADOS
                    pt = coordenada_PTO_(0)
                    pt2 = coordenada_PTO_(1)

                    fundacion_.dibujar_texto_bloque_fund(pt.X + Cos(ANGLE) * 20, pt.Y + Sin(ANGLE) * 20, ANGLE, ents, "L=" & Round(pt.DistanceTo(pt2), 0), CUANTIA, "")

                    If ckbx_traslapo = True Then
                        fundacion_.dibujar_dimesion_funda(ents, pt_rango, pt2_rango, ANGLE)
                        _pto_barra = fundacion_.intesrcion_barra_dimension(New Point2d(pt.X + Cos(ANGLE) * recubrimeinto, pt.Y + Sin(ANGLE) * recubrimeinto), New Point2d(pt2.X - Cos(ANGLE) * recubrimeinto, pt2.Y - Sin(ANGLE) * recubrimeinto), fundacion_.punto_dimension1, fundacion_.punto_dimension2)
                        fundacion_.dibujar_circulo_fund(ents, New Point3d(_pto_barra.X - Cos(ANGLE) * 10, _pto_barra.Y - Sin(ANGLE) * 10, 0), casos_dibujar_, ANGLE)
                    End If
                    ents.Add(ent.ObjectId)
                    Dim split_ As String() = Replace(Replace(Replace(LCase(CUANTIA), "f=", ""), "f'=", ""), "%%c", "").Split(New [Char]() {"a"c, CChar(vbTab)})
                    Dim VARIOS_ As New CODIGOS_DATOS()
                    VARIOS_.addData_PROG_LOSA(ent.ObjectId, caso_tipo, New Point2d(pt2.X, pt2.Y), New Point2d(pt.X, pt.Y), split_(0), pt.DistanceTo(pt2), 0, split_(1), direccion, 0, 0)

                    Dim GRUPO_ As New CODIGOS_GRUPOS()
                    'crear grupo
                    GRUPO_.creacion_grupo(ents, grupo_referencia_)
                End If

                acTrans.Commit()
                Application.DocumentManager.MdiActiveDocument.Editor.UpdateScreen()
                Application.DocumentManager.MdiActiveDocument.Editor.Regen()
            End Using

        End Using
    End Sub



    Public Sub aux__barra_manual2(ByRef pt As Point3d, ByRef pt2 As Point3d, ByRef pt_rango As Point3d, ByRef pt2_rango As Point3d, ByVal CUANTIA As String, ByVal direccion As String, ByVal tipo As String, ByVal txt_recub_ As String, ByVal rbt_inferior_ As Boolean, ByVal ckbx_traslapo_ As Boolean, ByVal casos_dibujar_ As String, ByVal grupo_referencia_ As String, ByVal recubrimeinto As String)
        ' Obtener el documento y la base de datos actuales
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        Dim acDocEd As Editor = Application.DocumentManager.MdiActiveDocument.Editor
        Dim utiles_aux As New utiles
        Using acDoc.LockDocument()


            'Dim _acSSet_LINEA As SelectionSet

            Dim acObjIdColl_borra_v2 As ObjectIdCollection = New ObjectIdCollection()
            Dim aux_x As Single = -1100
            Dim aux_y As Single = -100

            acObjIdColl_borra_v2.Clear()
            'UpdateTransientGraphics(pt)

            Dim _currentPoint As Point3d = pt
            ' Iniciar una transaccion
            Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()


                Dim coordenada_PTO_(1) As Point3d
                'coordenada_PTO_ = coordenada_modificar_fun(Nothing, pt, pt2)    ' OBTENER P1 Y P2 ORDENADOS
                'pt = coordenada_PTO_(0)
                'pt2 = coordenada_PTO_(1)
                Dim ANGLE As Double = utiles_aux.coordenada__angulo_p1_p2_fun(pt, pt2, acDocEd, Nothing)
                Dim acPoly As Polyline = New Polyline()

                ' Abrir la tabla de bloques en modo lectura
                Dim acBlkTbl As BlockTable = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead)
                ' Abrir el registro del bloque de Espacio Modelo en modo escritura
                Dim acBlkTblRec As BlockTableRecord
                acBlkTblRec = acTrans.GetObject(acBlkTbl(BlockTableRecord.ModelSpace), OpenMode.ForWrite)


                Dim acPolyObj As ObjectId = acPoly.ObjectId
                acPoly.SetDatabaseDefaults()
                acPoly.AddVertexAt(0, New Point2d(pt.X, pt.Y), 0, 0, 0)
                acPoly.AddVertexAt(1, New Point2d(pt2.X, pt2.Y), 0, 0, 0)
                acPoly.Layer = "BARRAS"

                ' Añadir el nuevo objeto al registro de la tabla para bloques y a la
                ' transaccion
                acBlkTblRec.AppendEntity(acPoly)
                acTrans.AddNewlyCreatedDBObject(acPoly, True)



                '' Abrir la tabla para bloques en modo lectura
                Dim ents As ObjectIdCollection = New ObjectIdCollection()
                acObjIdColl_borra_v2.Add(acPoly.ObjectId)
                'dibujar linea
                'Dim acLine As Line = PLANTA_.dibujar_linea_pl(Nothing, pt, pt2, "0")
                'Dim acline_obj As Object = ultimo_elemto()



                'crear entidad
                Dim ent As Entity = DirectCast(acTrans.GetObject(acPoly.ObjectId, OpenMode.ForRead), Entity)
                If ent Is Nothing Then Return
                Dim cur As Curve = TryCast(ent, Curve)





                Dim ds_tabla As New DataSet
                Dim dt_tabla As New System.Data.DataTable

                'Dim tabla As New System.Data.DataTable

                Dim values_inter(3) As Point3d
                Dim i As Integer = 0
                Dim fundacion_ As New FUNDACIONES
                Dim _pto_barra As Point3d
                ' Si el estado de la solicitud es OK, es que se han seleccionado objetos
                If tipo <> "f3" Then




                    ' Recorrer los objetos del conjunto de selección




                    Dim caso_tipo As String = ""
                    Dim pto1_barra As Point2d
                    Dim pto2_barra As Point2d
                    Dim pto1_RANGO As Point2d
                    Dim pto2_RANGO As Point2d

                    If tipo = "f10" Or tipo = "f9a" Then

                        coordenada_PTO_ = utiles_aux.coordenada_modificar_fun(Nothing, pt, pt2)    ' OBTENER P1 Y P2 ORDENADOS
                        pt = coordenada_PTO_(0)
                        pt2 = coordenada_PTO_(1)


                        pto1_barra = New Point2d(pt.X + Cos(ANGLE) * recubrimeinto, pt.Y + Sin(ANGLE) * recubrimeinto)
                        pto2_barra = New Point2d(pt2.X - Cos(ANGLE) * recubrimeinto, pt2.Y - Sin(ANGLE) * recubrimeinto)
                        fundacion_.modificar_barra_FUNDA(ents, ent, pto1_barra, pto2_barra, tipo, "", 18, 15)


                        fundacion_.dibujar_texto_bloque_fund(pt.X + Cos(ANGLE) * 20, pt.Y + Sin(ANGLE) * 20, ANGLE, ents, "L=" & fundacion_.largo_barra_real_fun, CUANTIA, fundacion_.largo_barra_parcial_fun)
                        pto1_RANGO = fundacion_.punto_dimension1
                        pto2_RANGO = fundacion_.punto_dimension2
                        'If My1Commands.myPalette.ckbx_traslapo.Checked = True Then
                        '    fundacion_.dibujar_dimesion_funda(ents, Nothing, Nothing, ANGLE)
                        '    _pto_barra = fundacion_.intesrcion_barra_dimension(New Point2d(pt.X + Cos(ANGLE) * recubrimeinto, pt.Y + Sin(ANGLE) * recubrimeinto), New Point2d(pt2.X - Cos(ANGLE) * recubrimeinto, pt2.Y - Sin(ANGLE) * recubrimeinto), fundacion_.punto_dimension1, fundacion_.punto_dimension2)
                        '    fundacion_.dibujar_circulo_fund(ents, New Point3d(_pto_barra.X - Cos(ANGLE) * 10, _pto_barra.Y - Sin(ANGLE) * 10, 0), My1Commands.casos_dibujar, ANGLE)
                        'End If
                    ElseIf tipo = "f11" Or tipo = "f9a_V" Then


                        'pt = New Point3d(dt_tabla(0)(0), dt_tabla(0)(1), 0)
                        'pt2 = pt2
                        If rbt_inferior_ = True Then
                            caso_tipo = "f11"
                        Else
                            caso_tipo = "f9a_V"
                        End If

                        'End If
                        coordenada_PTO_ = utiles_aux.coordenada_modificar_fun(Nothing, pt, pt2)

                        ' OBTENER P1 Y P2 ORDENADOS
                        pt = coordenada_PTO_(0)
                        pt2 = coordenada_PTO_(1)

                        fundacion_.modificar_barra_FUNDA(ents, ent, New Point2d(pt.X + Cos(ANGLE) * recubrimeinto, pt.Y + Sin(ANGLE) * recubrimeinto), New Point2d(pt2.X - Cos(ANGLE) * recubrimeinto, pt2.Y - Sin(ANGLE) * recubrimeinto), tipo, direccion, 18, 15)


                        If direccion = "horizontal_i" Or direccion = "vertical_b" Then
                            fundacion_.dibujar_texto_bloque_fund(pt.X + Cos(ANGLE) * 20, pt.Y + Sin(ANGLE) * 20, ANGLE, ents, "L=" & fundacion_.largo_barra_real_fun, CUANTIA, fundacion_.largo_barra_parcial_fun)
                        Else
                            fundacion_.dibujar_texto_bloque_fund(pt2.X - Cos(ANGLE) * 20, pt2.Y - Sin(ANGLE) * 20, ANGLE, ents, "L=" & fundacion_.largo_barra_real_fun, CUANTIA, fundacion_.largo_barra_parcial_fun)
                        End If


                    ElseIf dt_tabla.Rows.Count <> 0 Then
                        acPoly.Erase()
                        GoTo salto1
                    End If
                    If ckbx_traslapo_ = True Then
                        fundacion_.dibujar_dimesion_funda(ents, pt_rango, pt2_rango, ANGLE)
                        _pto_barra = fundacion_.intesrcion_barra_dimension(New Point2d(pt.X + Cos(ANGLE) * recubrimeinto, pt.Y + Sin(ANGLE) * recubrimeinto), New Point2d(pt2.X - Cos(ANGLE) * recubrimeinto, pt2.Y - Sin(ANGLE) * recubrimeinto), fundacion_.punto_dimension1, fundacion_.punto_dimension2)
                        fundacion_.dibujar_circulo_fund(ents, New Point3d(_pto_barra.X - Cos(ANGLE) * 10, _pto_barra.Y - Sin(ANGLE) * 10, 0), casos_dibujar_, ANGLE)
                    End If

                    Dim split_ As String() = Replace(LCase(CUANTIA), "%%c", "").Split(New [Char]() {"a"c, CChar(vbTab)})
                    Dim VARIOS_ As New CODIGOS_DATOS()
                    VARIOS_.addData_PROG_LOSA(fundacion_.object_poly, fundacion_.tipo, fundacion_.punto_final, fundacion_.punto_inicial, split_(0), fundacion_.largo_barra_real_fun, 0, split_(1), fundacion_.orientacion_, fundacion_.punto_e_losa_real, fundacion_.punto_e_losa)

                    Dim GRUPO_ As New CODIGOS_GRUPOS()
                    'crear grupo
                    GRUPO_.creacion_grupo(ents, grupo_referencia_)

salto1:
                Else
                    Dim caso_tipo As String = ""
                    caso_tipo = "f3"
                    coordenada_PTO_ = utiles_aux.coordenada_modificar_fun(Nothing, pt, pt2)    ' OBTENER P1 Y P2 ORDENADOS
                    pt = coordenada_PTO_(0)
                    pt2 = coordenada_PTO_(1)

                    fundacion_.dibujar_texto_bloque_fund(pt.X + Cos(ANGLE) * 20, pt.Y + Sin(ANGLE) * 20, ANGLE, ents, "L=" & Round(pt.DistanceTo(pt2), 0), CUANTIA, "")

                    If ckbx_traslapo_ = True Then
                        fundacion_.dibujar_dimesion_funda(ents, pt_rango, pt2_rango, ANGLE)
                        _pto_barra = fundacion_.intesrcion_barra_dimension(New Point2d(pt.X + Cos(ANGLE) * recubrimeinto, pt.Y + Sin(ANGLE) * recubrimeinto), New Point2d(pt2.X - Cos(ANGLE) * recubrimeinto, pt2.Y - Sin(ANGLE) * recubrimeinto), fundacion_.punto_dimension1, fundacion_.punto_dimension2)
                        fundacion_.dibujar_circulo_fund(ents, New Point3d(_pto_barra.X - Cos(ANGLE) * 10, _pto_barra.Y - Sin(ANGLE) * 10, 0), casos_dibujar_, ANGLE)
                    End If
                    ents.Add(ent.ObjectId)
                    Dim split_ As String() = Replace(LCase(CUANTIA), "%%c", "").Split(New [Char]() {"a"c, CChar(vbTab)})
                    Dim VARIOS_ As New CODIGOS_DATOS()
                    VARIOS_.addData_PROG_LOSA(ent.ObjectId, caso_tipo, New Point2d(pt2.X, pt2.Y), New Point2d(pt.X, pt.Y), split_(0), pt.DistanceTo(pt2), 0, split_(1), direccion, 0, 0)

                    Dim GRUPO_ As New CODIGOS_GRUPOS()
                    'crear grupo
                    GRUPO_.creacion_grupo(ents, grupo_referencia_)
                End If

                acTrans.Commit()
                Application.DocumentManager.MdiActiveDocument.Editor.UpdateScreen()
                Application.DocumentManager.MdiActiveDocument.Editor.Regen()
            End Using

        End Using
    End Sub

    Public Sub aux_dtras_fund(ByVal txt_recub As String, ByVal ckbx_traslapo As Boolean, ByVal grupo_referencia As String, ByVal casos_dibujar As String, ByRef _contenerdorIDOBJ As ObjectIdCollection)

        Dim utiles_aux As New utiles
        Dim doc As Document = Application.DocumentManager.MdiActiveDocument
        Dim db As Database = doc.Database
        Dim ed As Editor = doc.Editor
        Using doc.LockDocument()
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'comprobar bloque
            Dim flecha As String = "_SCAD_CUANTIA_FUND_NH2"
            Using acTrans As Transaction = db.TransactionManager.StartTransaction()

                ' Abrir la tabla para bloques en modo lectura
                Dim acBlkTbl As BlockTable = acTrans.GetObject(db.BlockTableId, OpenMode.ForRead)

                If Not acBlkTbl.Has(flecha) Then
                    'Else
                    Dim VARIOS_ As New atributos()
                    If File.Exists("C:\Program Files\AutocadNh\_SCAD_CUANTIA_FUND_NH.dwg") Then
                        VARIOS_.InsertBlock("C:\Program Files\AutocadNh\_SCAD_CUANTIA_FUND_NH.dwg", flecha)
                    End If

                    If Not acBlkTbl.Has("_SCAD_CUANTIA_FUND_NH2") Then
                        MsgBox("Insertar bloque de texto '_SCAD_CUANTIA_FUND_NH2' ", vbCritical)
                        GoTo final2
                    End If


                End If
                acTrans.Commit()
            End Using

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            Dim FUNDACION_ As New FUNDACIONES()
            Dim acObjIdColl_borra_ss As ObjectIdCollection = New ObjectIdCollection()

            Dim opt1 As New PromptEntityOptions(vbLf & "n1) Seleccionar ubicacion de traslapo:")
            opt1.SetRejectMessage(vbLf & "error!")
            opt1.AddAllowedClass(GetType(Polyline), True)
            Dim res1 As PromptEntityResult = ed.GetEntity(opt1)
            Dim pto1_Interseccion_Rango_barra As Point3d


            Dim lista_obj As New List(Of ObjectId)()

            If res1.Status = PromptStatus.OK Then

                Using tr As Transaction = db.TransactionManager.StartTransaction()

                    Dim l As Polyline = DirectCast(tr.GetObject(res1.ObjectId, OpenMode.ForRead), Polyline)
                    ' Dim ent As Entity = DirectCast(tr.GetObject(l.ObjectId, OpenMode.ForWrite), Entity)
                    ' Dim cur As Curve = TryCast(ent, Curve)

                    Dim pto_selec_mouse As Point3d = l.GetClosestPointTo(res1.PickedPoint, False)

                    Dim REAC_PLANTA As New CODIGOS_DATOS()

                    Dim ss As Polyline = TryCast(tr.GetObject(res1.ObjectId, OpenMode.ForWrite), Polyline)

                    Dim tipo_barra(9) As String
                    REAC_PLANTA.getData_PROG2(ss, tipo_barra, False)
                    Dim split_2 As String() = tipo_barra(7).Split(New [Char]() {"@"c, "_"c, CChar(vbTab)})


                    Dim datos_losa2() As String
                    datos_losa2 = utiles_aux.TIPO_caso_BARRA_LOSA_ind_fund(ss, "f16", tipo_barra)



                    Dim ptoini_rangoOriginal As New Point3d
                    Dim ptoFin_rangoOriginal As New Point3d
                    'borrar
                    Dim _planta_agrupa As New CODIGOS_GRUPOS()
                    Dim ents As ObjectIdCollection = New ObjectIdCollection()
                    Dim acObjId_grup__ As ObjectId()
                    acObjId_grup__ = _planta_agrupa.buscar_grupo(res1.ObjectId)

                    If IsNothing(acObjId_grup__) Then
                        MsgBox("Elemento no agrupado", vbCritical)
                        GoTo final
                    End If


                    'Dim atributo As New atributos()
                    'Dim tipo_FUND(9) As String
                    For Each idObj As ObjectId In acObjId_grup__

                        If idObj.ObjectClass.DxfName.ToString = "DIMENSION" Then
                            'Dim excur As Entity = tr.GetObject(idObj, OpenMode.ForWrite)
                            Dim acEnt_barra_aux As Dimension = TryCast(tr.GetObject(idObj, OpenMode.ForWrite), Dimension)
                            Dim acEnt_barra_aux2 As RotatedDimension = TryCast(tr.GetObject(idObj, OpenMode.ForWrite), RotatedDimension)
                            ptoini_rangoOriginal = acEnt_barra_aux2.XLine1Point
                            ptoFin_rangoOriginal = acEnt_barra_aux2.XLine2Point

                            Dim acPoly3 As Polyline = FUNDACION_.dibujar_barra_fund(acObjIdColl_borra_ss, New Point2d(ptoini_rangoOriginal.X, ptoini_rangoOriginal.Y), New Point2d(ptoFin_rangoOriginal.X, ptoFin_rangoOriginal.Y), "BARRAS")

                            Dim excur As Entity = tr.GetObject(acPoly3.ObjectId, OpenMode.ForWrite)
                            Dim pts As New Point3dCollection()

                            Dim ent3 As Entity = DirectCast(tr.GetObject(l.ObjectId, OpenMode.ForWrite), Entity)
                            Dim cur As Curve = TryCast(ent3, Curve)

                            cur.IntersectWith(excur, Intersect.OnBothOperands, pts, 0, 0)
                            acPoly3.Erase()

                            If pts.Count <> 0 Then
                                pto1_Interseccion_Rango_barra = New Point3d(pts(0).X, pts(0).Y, 0)
                                If pts.Count = 2 Then Dim pto2_Interseccion_Rango_barra As Point3d = New Point3d(pts(1).X, pts(1).Y, 0)
                            Else
                                MsgBox("No se pudo intersectar barra con reccorido. Deben estar intersectadas para generar traslapo", vbCritical)
                                Exit Sub
                            End If
                        End If

                    Next

                    Dim tipo_losa_f(9) As String

                    For Each idObj As ObjectId In acObjId_grup__
                        If idObj.ObjectClass.DxfName.ToString = "TEXT" Then
                            Dim acEnt_barra_aux As DBText = TryCast(tr.GetObject(idObj, OpenMode.ForWrite), DBText)
                            acEnt_barra_aux.Erase()
                        ElseIf idObj.ObjectClass.DxfName.ToString = "LWPOLYLINE" Then
                            _contenerdorIDOBJ.Remove(idObj)
                            Dim acEnt_barra_aux As Polyline = TryCast(tr.GetObject(idObj, OpenMode.ForWrite), Polyline)
                            acEnt_barra_aux.Erase()
                        ElseIf idObj.ObjectClass.DxfName.ToString = "DIMENSION" Then
                            'Dim excur As Entity = tr.GetObject(idObj, OpenMode.ForWrite)
                            Dim acEnt_barra_aux As Dimension = TryCast(tr.GetObject(idObj, OpenMode.ForWrite), Dimension)
                            Dim acEnt_barra_aux2 As RotatedDimension = TryCast(tr.GetObject(idObj, OpenMode.ForWrite), RotatedDimension)

                            ptoini_rangoOriginal = acEnt_barra_aux2.XLine1Point
                            ptoFin_rangoOriginal = acEnt_barra_aux2.XLine2Point

                            acEnt_barra_aux.Erase()
                        ElseIf idObj.ObjectClass.DxfName.ToString = "CIRCLE" Then
                            Dim acEnt_barra_aux As Circle = TryCast(tr.GetObject(idObj, OpenMode.ForWrite), Circle)
                            acEnt_barra_aux.Erase()
                        ElseIf idObj.ObjectClass.DxfName.ToString = "INSERT" Then
                            Dim atributo As New atributos()
                            atributo.obtener_atributos_funda(idObj, tipo_losa_f)
                            Dim acEnt_barra_aux As BlockReference = tr.GetObject(idObj, OpenMode.ForWrite)
                            acEnt_barra_aux.Erase()

                            If tipo_losa_f(0) <> "" Then
                                Dim split_fund As String() = tipo_losa_f(0).ToLower().Split(New [Char]() {"c"c, "a"c, CChar(vbTab)})
                                If IsNumeric(split_fund(1)) Then
                                    tipo_barra(1) = split_fund(1)
                                Else
                                    MsgBox("Error al buscar diamtro de cuantia, se utiliza diamtro Interno de barra")
                                End If
                            Else
                                MsgBox("Error al buscar diamtro de cuantia, se utiliza diamtro Interno de barra")

                            End If
                        End If
                    Next

                    Dim split_3 As String() = datos_losa2(4).Split(New [Char]() {","c, "_"c, CChar(vbTab)})
                    Dim ptoini_barraOriginal As Point3d = New Point3d(split_3(0), split_3(1), 0)
                    Dim ptofin_barraOriginal As Point3d = New Point3d(split_3(2), split_3(3), 0)

                    Dim coordenada_PTO_(1) As Point3d

                    coordenada_PTO_ = utiles_aux.coordenada_modificar_fun(Nothing, ptoini_barraOriginal, ptofin_barraOriginal)
                    ptoini_barraOriginal = coordenada_PTO_(0)
                    ptofin_barraOriginal = coordenada_PTO_(1)

                    Dim angulobarra3 As Single = utiles_aux.coordenada__angulo_p1_p2_fun(New Point3d(ptoini_barraOriginal.X, ptoini_barraOriginal.Y, 0), New Point3d(ptofin_barraOriginal.X, ptofin_barraOriginal.Y, 0), ed, Nothing)
                    Dim direcion_pfin_Ini_BARRA As Point3d = utiles_aux.NormalizeDifference(ptoini_barraOriginal, ptofin_barraOriginal)
                    Dim direcion_pfin_ini_RANGO As Point3d = utiles_aux.NormalizeDifference(ptoini_rangoOriginal, ptoFin_rangoOriginal)


                    Dim delta_nose As Single = 10 ' este delta se imcopora pq tenai agerga 5 cm a cada lado al crea traslapo, no se pq¡¡¡¡, entonces se agrega delta=0


                    pto_selec_mouse = utiles_aux.InterseccionPerpendicular(pto_selec_mouse, ptoini_barraOriginal, ptofin_barraOriginal)
                    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                    '' Dim delta_x, delta_y As Single
                    'If datos_losa2(2) = "horizontal_i" Or datos_losa2(2) = "horizontal_d" Then
                    '    pto_selec_mouse = New Point3d(pto_selec_mouse.X, utiles_aux.datos_lines(ptoini_barraOriginal.X, ptoini_barraOriginal.Y, ptofin_barraOriginal.X, ptofin_barraOriginal.Y, pto_selec_mouse.X, pto_selec_mouse.Y), 0)
                    '    ' delta_x = 2
                    '    '   delta_y = 0
                    'Else
                    '    pto_selec_mouse = New Point3d(ptoini_barraOriginal.X, pto_selec_mouse.Y, 0)
                    '    '  delta_x = 0
                    '    '  delta_y = 2
                    'End If

                    Dim pMin As Point3d
                    Dim pMax As Point3d
                    Using acView As ViewTableRecord = doc.Editor.GetCurrentView()
                        pMin = New Point3d(Application.GetSystemVariable("viewctr").X - (acView.Width / 2), Application.GetSystemVariable("viewctr").Y - (acView.Height / 2), 0)
                        pMax = New Point3d((acView.Width / 2) + Application.GetSystemVariable("viewctr").X, (acView.Height / 2) + Application.GetSystemVariable("viewctr").Y, 0)
                    End Using


                    utiles_aux.Zoom(New Point3d(Min(ptoini_barraOriginal.X, ptofin_barraOriginal.X) - 200, Min(ptoini_barraOriginal.Y, ptofin_barraOriginal.Y) - 200, 0), New Point3d(Max(ptoini_barraOriginal.X, ptofin_barraOriginal.X) + 200, Max(ptoini_barraOriginal.Y, ptofin_barraOriginal.Y) + 200, 0), New Point3d(), 1)

                    utiles_aux.Create_ALayer("BARRAS")
                    utiles_aux.Create_ALayer("TEXTO")
                    'FUNDACION_.punto_e_losa_real = split_2(1)
                    'FUNDACION_.punto_e_losa = split_2(2)

                    Dim _currentPoint_aux As Point3d

                    Dim tipo_losa As String = ""
                    Dim tipo_direccion As String = ""
                    If datos_losa2(0) = "f10" Then
                        tipo_losa = "f11"

                        If datos_losa2(2) = "horizontal_i" Then
                            tipo_direccion = "horizontal_i"
                        ElseIf datos_losa2(2) = "horizontal_d" Then
                            tipo_direccion = "horizontal_i"
                        ElseIf datos_losa2(2) = "vertical_b" Then
                            tipo_direccion = "vertical_b"
                        ElseIf datos_losa2(2) = "vertical_a" Then
                            tipo_direccion = "vertical_b"
                        End If
                    ElseIf datos_losa2(0) = "f11" Then
                        If datos_losa2(2) = "horizontal_i" Then
                            tipo_losa = "f11"
                            tipo_direccion = "horizontal_i"

                        ElseIf datos_losa2(2) = "horizontal_d" Then
                            tipo_losa = "f3"
                            tipo_direccion = "horizontal_i"

                        ElseIf datos_losa2(2) = "vertical_a" Then
                            tipo_losa = "f11"
                            tipo_direccion = "vertical_b"

                        ElseIf datos_losa2(2) = "vertical_b" Then
                            tipo_losa = "f11"
                            tipo_direccion = "vertical_b"

                        End If

                    ElseIf datos_losa2(0) = "f3" Then
                        If datos_losa2(2) = "horizontal_i" Then
                            tipo_direccion = "horizontal_i"
                            tipo_losa = "f3"
                        ElseIf datos_losa2(2) = "horizontal_d" Then
                            tipo_direccion = "horizontal_i"
                            tipo_losa = "f3"
                        ElseIf datos_losa2(2) = "vertical_b" Then
                            tipo_losa = "f3"
                            tipo_direccion = "vertical_b"
                        ElseIf datos_losa2(2) = "vertical_a" Then
                            tipo_losa = "f3"
                            tipo_direccion = "vertical_b"
                        End If
                    ElseIf datos_losa2(0) = "f9a" Then
                        If datos_losa2(2) = "horizontal_i" Then
                            tipo_direccion = "horizontal_i"
                            tipo_losa = "f9a_V"
                        ElseIf datos_losa2(2) = "horizontal_d" Then
                            tipo_direccion = "horizontal_i"
                            tipo_losa = "f9a_V"
                        ElseIf datos_losa2(2) = "vertical_b" Then
                            tipo_losa = "f9a_V"
                            tipo_direccion = "vertical_b"
                        ElseIf datos_losa2(2) = "vertical_a" Then
                            tipo_losa = "f9a_V"
                            tipo_direccion = "vertical_b"
                        End If
                    ElseIf datos_losa2(0) = "f9a_V" Then
                        If datos_losa2(2) = "horizontal_i" Then
                            tipo_direccion = "horizontal_i"
                            tipo_losa = "f9a_V"
                        ElseIf datos_losa2(2) = "horizontal_d" Then
                            tipo_direccion = "horizontal_i"
                            tipo_losa = "f3"
                        ElseIf datos_losa2(2) = "vertical_b" Then
                            tipo_losa = "f9a_V"
                            tipo_direccion = "vertical_b"
                        ElseIf datos_losa2(2) = "vertical_a" Then
                            tipo_losa = "f3"
                            tipo_direccion = "vertical_b"
                        End If

                    Else

                        MsgBox("Tipo de barra distinta a la base de datos ", vbCritical)
                        Exit Sub
                    End If


                    'If tipo_losa = "f3" Then
                    '    FUNDACION_.punto_cua_losa = "%%c" & tipo_barra(1) & "a" & tipo_barra(8)
                    'Else
                    '    FUNDACION_.punto_cua_losa = tipo_losa_f(0)
                    'End If

                    FUNDACION_.punto_cua_losa = tipo_losa_f(0)


                    ' largo de la busqueda desde el pto
                    Dim largoBusqueda As Integer = 4
                    ' barra inferior o mas ala izq   (pto fin pt con y menor o x menor)


                    Dim LArgo As Double = utiles_aux.largo_traslapo(tipo_barra(1)) / 2.0 + delta_nose
                    Dim ptoini3_NuevaBarra1 As Point3d = pto_selec_mouse.Subtract(direcion_pfin_Ini_BARRA.GetAsVector() * LArgo)  ' New Point3d(pt1.X + Cos(angulobarra3) * (utiles_aux.largo_traslapo(tipo_barra(1)) / 2 + delta_nose), pt1.Y + Sin(angulobarra3) * (utiles_aux.largo_traslapo(tipo_barra(1)) / 2 + delta_nose), 0)
                    Dim ptofin3_NuevaBarra1 As Point3d = ptofin_barraOriginal.Add(direcion_pfin_Ini_BARRA.GetAsVector() * LArgo) '   New Point3d(ptofin3.X - Cos(angulobarra3) * largoBusqueda, ptofin3.Y - Sin(angulobarra3) * largoBusqueda, 0)


                    _currentPoint_aux = New Point3d((ptofin_barraOriginal.X + ptoini3_NuevaBarra1.X) / 2, (ptofin_barraOriginal.Y + ptoini3_NuevaBarra1.Y) / 2, 0)


                    Dim ptoInicial_Rango1 As New Point3d
                    Dim ptoFinal_Rango1 As New Point3d
                    Dim dista_d1 As Single = _currentPoint_aux.DistanceTo(pto1_Interseccion_Rango_barra)

                    If Abs(ptofin_barraOriginal.X - ptoini3_NuevaBarra1.X) < 0.1 Then   ' vertical
                        If pto1_Interseccion_Rango_barra.Y > _currentPoint_aux.Y Then
                            ptoInicial_Rango1 = New Point3d(ptoini_rangoOriginal.X - dista_d1 * Cos(angulobarra3), ptoini_rangoOriginal.Y - dista_d1 * Sin(angulobarra3), 0)
                            ptoFinal_Rango1 = New Point3d(ptoFin_rangoOriginal.X - dista_d1 * Cos(angulobarra3), ptoFin_rangoOriginal.Y - dista_d1 * Sin(angulobarra3), 0)
                        Else
                            ptoInicial_Rango1 = New Point3d(ptoini_rangoOriginal.X + dista_d1 * Cos(angulobarra3), ptoini_rangoOriginal.Y + dista_d1 * Sin(angulobarra3), 0)
                            ptoFinal_Rango1 = New Point3d(ptoFin_rangoOriginal.X + dista_d1 * Cos(angulobarra3), ptoFin_rangoOriginal.Y + dista_d1 * Sin(angulobarra3), 0)
                        End If

                        'xline1_tp_n1 = New Point3d(xline1_tp.X, xline1_tp.Y, 0)
                        'xline2_tp_n1 = New Point3d(xline2_tp.X, xline2_tp.Y, 0)
                    Else ' horizontal o inclinado

                        If pto1_Interseccion_Rango_barra.X > _currentPoint_aux.X Then
                            ptoInicial_Rango1 = New Point3d(ptoini_rangoOriginal.X - dista_d1 * Cos(angulobarra3), ptoini_rangoOriginal.Y - dista_d1 * Sin(angulobarra3), 0)
                            ptoFinal_Rango1 = New Point3d(ptoFin_rangoOriginal.X - dista_d1 * Cos(angulobarra3), ptoFin_rangoOriginal.Y - dista_d1 * Sin(angulobarra3), 0)
                        Else
                            ptoInicial_Rango1 = New Point3d(ptoini_rangoOriginal.X + dista_d1 * Cos(angulobarra3), ptoini_rangoOriginal.Y + dista_d1 * Sin(angulobarra3), 0)
                            ptoFinal_Rango1 = New Point3d(ptoFin_rangoOriginal.X + dista_d1 * Cos(angulobarra3), ptoFin_rangoOriginal.Y + dista_d1 * Sin(angulobarra3), 0)
                        End If

                    End If

                    aux__barra_manual(ptofin3_NuevaBarra1, ptoini3_NuevaBarra1, ptoInicial_Rango1, ptoFinal_Rango1, FUNDACION_.punto_cua_losa,
                                      tipo_direccion, tipo_losa, txt_recub, ckbx_traslapo, grupo_referencia, casos_dibujar)

                    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                    acObjIdColl_borra_ss.Clear()

                    If datos_losa2(0) = "f10" Then
                        tipo_losa = "f11"
                        If datos_losa2(2) = "horizontal_i" Then
                            tipo_direccion = "horizontal_d"
                        ElseIf datos_losa2(2) = "horizontal_d" Then
                            tipo_direccion = "horizontal_d"
                        ElseIf datos_losa2(2) = "vertical_b" Then
                            tipo_direccion = "vertical_a"
                        ElseIf datos_losa2(2) = "vertical_a" Then
                            tipo_direccion = "vertical_a"
                        End If

                    ElseIf datos_losa2(0) = "f11" Then
                        If datos_losa2(2) = "horizontal_i" Then
                            tipo_direccion = "horizontal_d"
                            tipo_losa = "f3"
                        ElseIf datos_losa2(2) = "horizontal_d" Then
                            tipo_direccion = "horizontal_d"
                            tipo_losa = "f11"
                        ElseIf datos_losa2(2) = "vertical_a" Then
                            tipo_losa = "f11"
                            tipo_direccion = "vertical_a"
                        ElseIf datos_losa2(2) = "vertical_b" Then
                            tipo_losa = "f3"
                            tipo_direccion = "vertical_a"
                        End If
                    ElseIf datos_losa2(0) = "f3" Then
                        If datos_losa2(2) = "horizontal_i" Then
                            tipo_direccion = "horizontal_d"
                            tipo_losa = "f3"
                        ElseIf datos_losa2(2) = "horizontal_d" Then
                            tipo_direccion = "horizontal_d"
                            tipo_losa = "f3"
                        ElseIf datos_losa2(2) = "vertical_b" Then
                            tipo_losa = "f3"
                            tipo_direccion = "vertical_a"
                        ElseIf datos_losa2(2) = "vertical_a" Then
                            tipo_losa = "f3"
                            tipo_direccion = "vertical_a"
                        End If
                    ElseIf datos_losa2(0) = "f9a" Then
                        If datos_losa2(2) = "horizontal_i" Then
                            tipo_direccion = "horizontal_d"
                            tipo_losa = "f9a_V"
                        ElseIf datos_losa2(2) = "horizontal_d" Then
                            tipo_direccion = "horizontal_d"
                            tipo_losa = "f9a_V"
                        ElseIf datos_losa2(2) = "vertical_b" Then
                            tipo_losa = "f9a_V"
                            tipo_direccion = "vertical_a"
                        ElseIf datos_losa2(2) = "vertical_a" Then
                            tipo_losa = "f9a_V"
                            tipo_direccion = "vertical_a"
                        End If

                    ElseIf datos_losa2(0) = "f9a_V" Then
                        If datos_losa2(2) = "horizontal_i" Then
                            tipo_direccion = "horizontal_d"
                            tipo_losa = "f3"
                        ElseIf datos_losa2(2) = "horizontal_d" Then
                            tipo_direccion = "horizontal_d"
                            tipo_losa = "f9a_V"
                        ElseIf datos_losa2(2) = "vertical_b" Then
                            tipo_losa = "f3"
                            tipo_direccion = "vertical_a"
                        ElseIf datos_losa2(2) = "vertical_a" Then
                            tipo_losa = "f9a_V"
                            tipo_direccion = "vertical_a"
                        End If

                    Else
                        MsgBox("Tipo de barra distinta a la base de datos ", vbCritical)
                        Exit Sub
                        tipo_losa = datos_losa2(0)
                    End If


                    'If tipo_losa = "f3" Then
                    '    FUNDACION_.punto_cua_losa = "%%c" & tipo_barra(1) & "a" & tipo_barra(8)
                    'Else
                    '    FUNDACION_.punto_cua_losa = tipo_losa_f(0)
                    'End If
                    FUNDACION_.punto_cua_losa = tipo_losa_f(0)


                    ' barra superior o mas a la derecha   (pto fin pt con y menor o x menor)
                    'ptoini3_aux = New Point3d(ptoini3.X + Cos(angulobarra3) * largoBusqueda + Sin(angulobarra3) * 5, ptoini3.Y + Sin(angulobarra3) * largoBusqueda - Cos(angulobarra3) * 5, 0)
                    'ptofin3_aux = New Point3d(pt1.X - Cos(angulobarra3) * (utiles_aux.largo_traslapo(tipo_barra(1)) / 2 + delta_nose) + Sin(angulobarra3) * 5, pt1.Y - Sin(angulobarra3) * (utiles_aux.largo_traslapo(tipo_barra(1)) / 2 + delta_nose) - Cos(angulobarra3) * 5, 0)


                    ptoini3_NuevaBarra1 = ptoini_barraOriginal.Subtract(direcion_pfin_Ini_BARRA.GetAsVector() * LArgo)  ' New Point3d(pt1.X + Cos(angulobarra3) * (utiles_aux.largo_traslapo(tipo_barra(1)) / 2 + delta_nose), pt1.Y + Sin(angulobarra3) * (utiles_aux.largo_traslapo(tipo_barra(1)) / 2 + delta_nose), 0)
                    ptofin3_NuevaBarra1 = pto_selec_mouse.Add(direcion_pfin_Ini_BARRA.GetAsVector() * LArgo) '   New Point3d(ptofin3.X - Cos(angulobarra3) * largoBusqueda, ptofin3.Y - Sin(angulobarra3) * largoBusqueda, 0)

                    ptoini3_NuevaBarra1 = New Point3d(ptoini3_NuevaBarra1.X + Sin(angulobarra3) * 5, ptoini3_NuevaBarra1.Y - Cos(angulobarra3) * 5, 0)
                    ptofin3_NuevaBarra1 = New Point3d(ptofin3_NuevaBarra1.X + Sin(angulobarra3) * 5, ptofin3_NuevaBarra1.Y - Cos(angulobarra3) * 5, 0)




                    _currentPoint_aux = New Point3d((ptofin3_NuevaBarra1.X + ptoini3_NuevaBarra1.X) / 2, (ptofin3_NuevaBarra1.Y + ptoini3_NuevaBarra1.Y) / 2, 0)

                    dista_d1 = _currentPoint_aux.DistanceTo(ptoini3_NuevaBarra1)
                    Dim xline1_tp_n2 As New Point3d
                    Dim xline2_tp_n2 As New Point3d

                    If Abs(ptofin_barraOriginal.X - ptoini3_NuevaBarra1.X) < 0.1 Then   ' vertical

                        If ptoini3_NuevaBarra1.Y > _currentPoint_aux.Y Then
                            xline1_tp_n2 = New Point3d(ptoini_rangoOriginal.X - dista_d1 * Cos(angulobarra3), ptoini_rangoOriginal.Y - dista_d1 * Sin(angulobarra3), 0)
                            xline2_tp_n2 = New Point3d(ptoFin_rangoOriginal.X - dista_d1 * Cos(angulobarra3), ptoFin_rangoOriginal.Y - dista_d1 * Sin(angulobarra3), 0)
                        Else
                            xline1_tp_n2 = New Point3d(ptoini_rangoOriginal.X + dista_d1 * Cos(angulobarra3), ptoini_rangoOriginal.Y + dista_d1 * Sin(angulobarra3), 0)
                            xline2_tp_n2 = New Point3d(ptoFin_rangoOriginal.X + dista_d1 * Cos(angulobarra3), ptoFin_rangoOriginal.Y + dista_d1 * Sin(angulobarra3), 0)
                        End If

                        'xline1_tp_n2 = New Point3d(xline1_tp.X, xline1_tp.Y, 0)
                        'xline2_tp_n2 = New Point3d(xline2_tp.X, xline2_tp.Y, 0)
                    Else ' horizontal o inclinado

                        If ptoini3_NuevaBarra1.X > _currentPoint_aux.X Then
                            xline1_tp_n2 = New Point3d(ptoini_rangoOriginal.X + dista_d1 * Cos(angulobarra3), ptoini_rangoOriginal.Y + dista_d1 * Sin(angulobarra3), 0)
                            xline2_tp_n2 = New Point3d(ptoFin_rangoOriginal.X + dista_d1 * Cos(angulobarra3), ptoFin_rangoOriginal.Y + dista_d1 * Sin(angulobarra3), 0)
                        Else
                            xline1_tp_n2 = New Point3d(ptoini_rangoOriginal.X - dista_d1 * Cos(angulobarra3), ptoini_rangoOriginal.Y - dista_d1 * Sin(angulobarra3), 0)
                            xline2_tp_n2 = New Point3d(ptoFin_rangoOriginal.X - dista_d1 * Cos(angulobarra3), ptoFin_rangoOriginal.Y - dista_d1 * Sin(angulobarra3), 0)
                        End If

                    End If
                    'FUNDACION_.punto_cua_losa = tipo_barra(1) & "a" & tipo_barra(8)

                    aux__barra_manual(ptoini3_NuevaBarra1, ptofin3_NuevaBarra1, xline1_tp_n2, xline2_tp_n2, FUNDACION_.punto_cua_losa, tipo_direccion, tipo_losa, txt_recub, ckbx_traslapo, grupo_referencia, casos_dibujar)

                    ' aux__barra_manual2(New Point3d(ptoini3_aux.X, ptoini3_aux.Y, 0), New Point3d(ptofin3.X, ptofin3.Y, 0), xline1_tp_n2, xline2_tp_n2, "%%c" & tipo_barra(1) & "a" & tipo_barra(8), tipo_direccion, tipo_losa)

                    utiles_aux.Zoom(pMin, pMax, New Point3d(), 1)
                    Application.DocumentManager.MdiActiveDocument.Editor.UpdateScreen()
                    Application.DocumentManager.MdiActiveDocument.Editor.Regen()
final:

                    tr.Commit()

                End Using
            End If
final2:
        End Using
    End Sub



    Public Sub aux__barra_rango(ByVal txt_recub As String, ByVal casos_dibujar As String)
        ' Obtener el documento y la base de datos actuales
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        Dim acDocEd As Editor = Application.DocumentManager.MdiActiveDocument.Editor
        Dim utiles_aux As New utiles
        Using acDoc.LockDocument()



            Dim ents As ObjectIdCollection = New ObjectIdCollection()
            Dim pt As Point3d
            Dim pt2 As Point3d



            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'comprobar bloque

            Dim flecha As String = "_SCAD_CUANTIA_FUND_NH2"
            Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
                ' Abrir la tabla para bloques en modo lectura
                Dim acBlkTbl As BlockTable = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead)

                If Not acBlkTbl.Has(flecha) Then
                    'Else
                    Dim VARIOS_ As New atributos()
                    If File.Exists("C:\Program Files\AutocadNh\_SCAD_CUANTIA_FUND_NH.dwg") Then
                        VARIOS_.InsertBlock("C:\Program Files\AutocadNh\_SCAD_CUANTIA_FUND_NH.dwg", flecha)
                    End If



                    If Not acBlkTbl.Has("_SCAD_CUANTIA_FUND_NH2") Then
                        MsgBox("Insertar bloque de texto '_SCAD_CUANTIA_FUND_NH2' ", vbCritical)
                        GoTo final
                    End If


                End If
                acTrans.Commit()
            End Using

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx







            ' Iniciar una transaccion
            Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()


                Dim recubrimeinto As Integer = My1Commands.myPalette.txt_recub.Text
                Dim _pto_barra As Point3d
                Dim fundacion_ As New FUNDACIONES




                Dim opt1 As New PromptEntityOptions(vbLf & "n1) Seleccionar barra:")
                opt1.SetRejectMessage(vbLf & "error!")
                opt1.AddAllowedClass(GetType(Polyline), True)
                Dim res1 As PromptEntityResult = acDocEd.GetEntity(opt1)


                If res1.Status = PromptStatus.OK Then

                    Dim poly As Polyline = TryCast(acTrans.GetObject(res1.ObjectId, OpenMode.ForRead), Polyline)





                    Dim REAC_PLANTA As New CODIGOS_DATOS()
                    Dim tipo_caso_barra() As String

                    Dim tipo_barra(9) As String
                    REAC_PLANTA.getData_PROG2(poly, tipo_barra, False)

                    tipo_caso_barra = utiles_aux.TIPO_caso_BARRA_LOSA_ind_fund(poly, tipo_barra(0), tipo_barra)
                    Dim split_ As String() = tipo_caso_barra(4).Split(New [Char]() {","c, "_"c, CChar(vbTab)})
                    Dim ptoini3 As Point3d = New Point3d(split_(0), split_(1), 0)
                    Dim ptofin3 As Point3d = New Point3d(split_(2), split_(3), 0)
                    Dim ANGLE As Single = utiles_aux.coordenada__angulo_p1_p2_fun(New Point3d(ptoini3.X, ptoini3.Y, 0), New Point3d(ptofin3.X, ptofin3.Y, 0), acDocEd, Nothing)

                    Dim coordenada_PTO_(1) As Point3d

                    coordenada_PTO_ = utiles_aux.coordenada_modificar_fun(Nothing, ptoini3, ptofin3)    ' OBTENER P1 Y P2 ORDENADOS
                    pt = coordenada_PTO_(0)
                    pt2 = coordenada_PTO_(1)

                    fundacion_.dibujar_dimesion_funda(ents, Nothing, Nothing, ANGLE)
                    _pto_barra = fundacion_.intesrcion_barra_dimension(New Point2d(pt.X + Cos(ANGLE) * recubrimeinto, pt.Y + Sin(ANGLE) * recubrimeinto), New Point2d(pt2.X - Cos(ANGLE) * recubrimeinto, pt2.Y - Sin(ANGLE) * recubrimeinto), fundacion_.punto_dimension1, fundacion_.punto_dimension2)
                    fundacion_.dibujar_circulo_fund(ents, New Point3d(_pto_barra.X - Cos(ANGLE) * 10, _pto_barra.Y - Sin(ANGLE) * 10, 0), My1Commands.casos_dibujar, ANGLE)



                    Dim GRUPO_ As New CODIGOS_GRUPOS()
                    Dim nombre_grupo As String = GRUPO_.buscar_nombre_grupo(poly.ObjectId)
                    GRUPO_.agregar_borrar_grupo(ents, nombre_grupo, "A")

                End If
salto1:

                acTrans.Commit()
                Application.DocumentManager.MdiActiveDocument.Editor.UpdateScreen()
                Application.DocumentManager.MdiActiveDocument.Editor.Regen()
            End Using
final:
        End Using
    End Sub





    'unir barra
    Public Sub aux_cambiar_texto_mirror()
        If My1Commands.acObjIdColl_GENERAL <> Nothing Then My1Commands.acObjIdColl_GENERAL.Clear()
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acEd As Editor = acDoc.Editor



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

                cambiar_texto_mirror(acObjId, My1Commands.aux_enfierra_automatico, acDoc)

            Next


        End If


        ' Libera recursos y vuelve a mostrar el cuadro de dialogo

    End Sub

    Public Sub cambiar_texto_mirror(ByVal id As ObjectId, ByVal aux_enfierra_automatico As Boolean, ByRef _doc As Document) '_db_ObjectOpenedForModify

        '  Dim id As ObjectId = e.DBObject.Id
        'Application.ShowAlertDialog("NOmbre bloque es  " &   ' acBlkRef.ToString() & " es: ")
        If My1Commands.aux_enfierra_automatico = True Then GoTo final
        ' Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim ed As Editor = Application.DocumentManager.MdiActiveDocument.Editor

        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        '  ed.WriteMessage(ControlChars.Lf & acBlkRef.ToString() & ControlChars.Lf)
        '  ed.WriteMessage(String.Format("{0}Propiedades de ""{{0}}""{0}", _
        '                                      ControlChars.Lf), acBlkRef.Name)
        Dim varios_grupo As New CODIGOS_GRUPOS()
        If Not Directory.Exists(cots) Then
            ''Global.System.Windows.Forms.Application.Exit()
        End If

        '*************************
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

        Using acDoc.LockDocument()


            Using acTrans As Transaction = acDoc.TransactionManager.StartTransaction()


                Try


                    Dim pMin As Point3d
                    Dim pMax As Point3d
                    Using acView As ViewTableRecord = acDoc.Editor.GetCurrentView()
                        pMin = New Point3d(Application.GetSystemVariable("viewctr").X - (acView.Width / 2), Application.GetSystemVariable("viewctr").Y - (acView.Height / 2), 0)
                        pMax = New Point3d((acView.Width / 2) + Application.GetSystemVariable("viewctr").X, (acView.Height / 2) + Application.GetSystemVariable("viewctr").Y, 0)
                    End Using



                    Dim valore_actual As String = ""
                    '  Dim acBlkRef2 As BlockReference = TryCast(acTrans.GetObject(senderObj.ObjectId, OpenMode.ForWrite), BlockReference)
                    Dim acEnt_barra As Polyline = TryCast(acTrans.GetObject(id, OpenMode.ForWrite), Polyline)
                    acEnt_barra.SetDatabaseDefaults()

                    ' Dim retCoord As Object = acEnt_barra.ObjectId.CoordinateSystem2d()

                    Dim numero_pts As Integer = acEnt_barra.NumberOfVertices
                    'Dim pto_ini As Double = acEnt_barra.GetEndWidthAt(2)
                    ' acDoc.Editor.WriteMessage(ControlChars.Lf & "Identificador: " & pto_ini.ToString())


                    Dim resulta_barra As String = "" ' "sup:si,inf:si"
                    Dim resulta_barra_sobre As String = "" ' "sup:si,inf:si"
                    Dim _largo(2) As String

                    'XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                    ' BUSCAR EL TIPO DE BARRA, DE ELEMENTO DESPLAZADO

                    Dim REAC_PLANTA As New CODIGOS_DATOS()
                    Dim _fundacion As New FUNDACIONES()
                    Dim GRUPO_ As New CODIGOS_GRUPOS()

                    Dim tipo_losa_f(9) As String

                    Dim tipo_barra(9) As String
                    REAC_PLANTA.getData_PROG2(acEnt_barra, tipo_barra, False)

                    Select Case tipo_barra(0)
                        Case "f3", "f9", "f9a", "f9a_V", "f10", "f11"

                            Dim acObjId_grup As ObjectId() = GRUPO_.buscar_grupo(acEnt_barra.ObjectId)
                            If Not (acObjId_grup Is Nothing) Then

                                Using acTrans2 As Transaction = acCurDb.TransactionManager.StartTransaction()
                                    'SE BUSCA DENTRO DEL GRUPO SELECCIONADO ALGUNO QUE SE PARESCA AL DE REFERECIA
                                    ' SI ENCUERTA BORRA TODO 
                                    For Each idObj_ As ObjectId In acObjId_grup
                                        If idObj_.ObjectClass.DxfName.ToString = "INSERT" And id <> idObj_ Then
                                            Dim atributo As New atributos()
                                            atributo.obtener_atributos_funda(idObj_, tipo_losa_f)
                                            Dim acEnt_barra_aux As BlockReference = acTrans2.GetObject(idObj_, OpenMode.ForWrite)
                                            Dim pinser As Point3d = acEnt_barra_aux.Position
                                            Dim ents As ObjectIdCollection = New ObjectIdCollection()

                                            _fundacion.dibujar_texto_bloque_fund(pinser.X, pinser.Y, acEnt_barra_aux.Rotation, ents, tipo_losa_f(1), tipo_losa_f(0), tipo_losa_f(2))


                                            Dim nombre_grupo As String = GRUPO_.buscar_nombre_grupo(idObj_)
                                            GRUPO_.agregar_borrar_grupo(ents, nombre_grupo, "A")
                                            acEnt_barra_aux.Erase()

                                        End If
                                    Next
                                    acTrans2.Commit()
                                End Using
                            End If

                        Case Else
                            Debug.WriteLine("NO se encontro tipo de barra en clase 100 o 110")
                    End Select
                    'XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX


                    ' Guardar los cambios
                    acTrans.Commit()
                    ' Application.DocumentManager.MdiActiveDocument.Editor.UpdateScreen()
                    ' Application.DocumentManager.MdiActiveDocument.Editor.Regen()
                Catch Ex As Autodesk.AutoCAD.Runtime.Exception
                    acTrans.Dispose()
                    Application.ShowAlertDialog("Error: 'reac_modif_barra_final'" & vbLf & Ex.Message)
                Finally
                    ' Se mostrara este mensaje aunque se produzca un error

                    ' Application.ShowAlertDialog("error reac_modif_barra_final")
                End Try
            End Using
        End Using
final:
    End Sub




    Public Sub reac_modif_barra_losa_final_texto(ByVal id As ObjectId, ByVal aux_enfierra_automatico As Boolean, ByRef _doc As Document) '_db_ObjectOpenedForModify

        '  Dim id As ObjectId = e.DBObject.Id
        'Application.ShowAlertDialog("NOmbre bloque es  " &   ' acBlkRef.ToString() & " es: ")
        If aux_enfierra_automatico = True Then GoTo final
        ' Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim ed As Editor = Application.DocumentManager.MdiActiveDocument.Editor
        Dim acCurDb As Database = _doc.Database
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        '  ed.WriteMessage(ControlChars.Lf & acBlkRef.ToString() & ControlChars.Lf)
        '  ed.WriteMessage(String.Format("{0}Propiedades de ""{{0}}""{0}", _
        '                                      ControlChars.Lf), acBlkRef.Name)
        Dim varios_grupo As New CODIGOS_GRUPOS()
        If Not Directory.Exists(cots) Then
            ''Global.System.Windows.Forms.Application.Exit()
        End If
        '*************************
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

        Dim utiles_aux As New utiles
        Using acDoc.LockDocument()


            Using acTrans As Transaction = _doc.TransactionManager.StartTransaction()


                Try


                    Dim pMin As Point3d
                    Dim pMax As Point3d
                    Using acView As ViewTableRecord = acDoc.Editor.GetCurrentView()
                        pMin = New Point3d(Application.GetSystemVariable("viewctr").X - (acView.Width / 2), Application.GetSystemVariable("viewctr").Y - (acView.Height / 2), 0)
                        pMax = New Point3d((acView.Width / 2) + Application.GetSystemVariable("viewctr").X, (acView.Height / 2) + Application.GetSystemVariable("viewctr").Y, 0)
                    End Using



                    Dim valore_actual As String = ""
                    '  Dim acBlkRef2 As BlockReference = TryCast(acTrans.GetObject(senderObj.ObjectId, OpenMode.ForWrite), BlockReference)
                    Dim acEnt_barra As Polyline = TryCast(acTrans.GetObject(id, OpenMode.ForWrite), Polyline)
                    acEnt_barra.SetDatabaseDefaults()

                    ' Dim retCoord As Object = acEnt_barra.ObjectId.CoordinateSystem2d()

                    Dim numero_pts As Integer = acEnt_barra.NumberOfVertices
                    'Dim pto_ini As Double = acEnt_barra.GetEndWidthAt(2)
                    ' acDoc.Editor.WriteMessage(ControlChars.Lf & "Identificador: " & pto_ini.ToString())


                    Dim resulta_barra As String = "" ' "sup:si,inf:si"
                    Dim resulta_barra_sobre As String = "" ' "sup:si,inf:si"
                    Dim _largo(2) As String

                    'XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                    ' BUSCAR EL TIPO DE BARRA, DE ELEMENTO DESPLAZADO
                    Dim tipo_caso_barra() As String
                    Dim REAC_PLANTA As New CODIGOS_DATOS()
                    Dim _fundacion As New FUNDACIONES()
                    Dim GRUPO_ As New CODIGOS_GRUPOS()

                    Dim tipo_losa_f(9) As String

                    Dim tipo_barra(9) As String
                    REAC_PLANTA.getData_PROG2(acEnt_barra, tipo_barra, False)
                    ' codigo para solucuona singularidad
                    If acEnt_barra.NumberOfVertices = 2 And tipo_barra(0) = "f16" Then
                        tipo_barra(0) = "f3"
                    End If

                    Select Case tipo_barra(0)
                        Case "f3", "f9", "f9a", "f9a_V", "f10", "f11"

                            Dim acObjId_grup As ObjectId() = GRUPO_.buscar_grupo(acEnt_barra.ObjectId)
                            If Not (acObjId_grup Is Nothing) Then

                                Using acTrans2 As Transaction = acCurDb.TransactionManager.StartTransaction()
                                    'SE BUSCA DENTRO DEL GRUPO SELECCIONADO ALGUNO QUE SE PARESCA AL DE REFERECIA
                                    ' SI ENCUERTA BORRA TODO 
                                    For Each idObj_ As ObjectId In acObjId_grup
                                        If idObj_.ObjectClass.DxfName.ToString = "INSERT" And id <> idObj_ Then
                                            Dim atributo As New atributos()
                                            atributo.obtener_atributos_funda(idObj_, tipo_losa_f)

                                        End If
                                    Next

                                End Using
                            End If

                            tipo_caso_barra = utiles_aux.TIPO_caso_BARRA_LOSA_ind_fund(acEnt_barra, tipo_barra(0), tipo_barra)

                            If Not (tipo_caso_barra(3) Is Nothing) Then

                                Dim split_2 As String() = tipo_caso_barra(3).Split(New [Char]() {","c, CChar(vbTab)})
                                dibujar_barra_reac_fund(id, tipo_caso_barra, tipo_barra(0), "STRECH", tipo_barra, tipo_losa_f(0))
                            Else
                                MsgBox("Barra con datos internos o configuracion incorrecto. No se modifica el texto largos", vbInformation)
                            End If
                        Case Else
                            Debug.WriteLine("NO se encontro tipo de barra en clase 100 o 110")
                    End Select
                    'XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX


                    ' Guardar los cambios
                    acTrans.Commit()
                    ' Application.DocumentManager.MdiActiveDocument.Editor.UpdateScreen()
                    ' Application.DocumentManager.MdiActiveDocument.Editor.Regen()
                Catch Ex As Autodesk.AutoCAD.Runtime.Exception
                    acTrans.Dispose()
                    Application.ShowAlertDialog("Error: 'reac_modif_barra_final'" & vbLf & Ex.Message)
                Finally
                    ' Se mostrara este mensaje aunque se produzca un error

                    ' Application.ShowAlertDialog("error reac_modif_barra_final")
                End Try
            End Using
        End Using
final:
    End Sub


    Public Sub reac_modif_atribut(ByVal id As ObjectId, ByVal aux_enfierra_automatico As Boolean, ByRef _doc As Document, ByRef _contenerdorIDOBJ As ObjectIdCollection) '_db_ObjectOpenedForModify
        Dim utiles_aux As New utiles
        '  Dim id As ObjectId = e.DBObject.Id
        'Application.ShowAlertDialog("NOmbre bloque es  " &   ' acBlkRef.ToString() & " es: ")
        If My1Commands.aux_enfierra_automatico = True Then GoTo final
        ' Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim ed As Editor = Application.DocumentManager.MdiActiveDocument.Editor

        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        '  ed.WriteMessage(ControlChars.Lf & acBlkRef.ToString() & ControlChars.Lf)
        '  ed.WriteMessage(String.Format("{0}Propiedades de ""{{0}}""{0}", _
        '                                      ControlChars.Lf), acBlkRef.Name)
        Dim varios_grupo As New CODIGOS_GRUPOS()
        If Not Directory.Exists(cots) Then
            ''Global.System.Windows.Forms.Application.Exit()
        End If

        '*************************
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

        Dim CODIGOS_DATOS_ As New CODIGOS_DATOS()
        Dim GRUPO_ As New CODIGOS_GRUPOS()
        Using acDoc.LockDocument()


            Using acTrans As Transaction = acDoc.TransactionManager.StartTransaction()


                Dim acObjId_grup As ObjectId() = GRUPO_.buscar_grupo(id)

                If Not (acObjId_grup Is Nothing) Then


                    Using acTrans2 As Transaction = acCurDb.TransactionManager.StartTransaction()
                        'SE BUSCA DENTRO DEL GRUPO SELECCIONADO ALGUNO QUE SE PARESCA AL DE REFERECIA
                        ' SI ENCUERTA BORRA TODO 
                        For Each idObj_ As ObjectId In acObjId_grup
                            If idObj_.ObjectClass.DxfName.ToString = "LWPOLYLINE" And id <> idObj_ Then
                                Dim acEnt_barra As Polyline = TryCast(acTrans2.GetObject(idObj_, OpenMode.ForWrite), Polyline)
                                If acEnt_barra.Layer = "BARRAS" And Not My1Commands._contenerdorIDOBJ.Contains(idObj_) Then


                                    Dim REAC_PLANTA As New CODIGOS_DATOS()

                                    Dim tipo_barra(9) As String
                                    REAC_PLANTA.getData_PROG2(acEnt_barra, tipo_barra, False)




                                    Dim tipo_caso_barra() As String = utiles_aux.TIPO_caso_BARRA_LOSA_ind_fund(acEnt_barra, tipo_barra(0), tipo_barra)

                                    ' TIPO_caso_BARRA_LOSA_ind_fund(acPoly, tipo_barra(0), tipo_caso_barra)

                                    If IsNothing(tipo_caso_barra(0)) Then
                                        acEnt_barra.Color = Autodesk.AutoCAD.Colors.Color.FromRgb(255, 255, 0)
                                        MsgBox("error   TIPO_caso_BARRA_ind, revisar barra", vbInformation)
                                        GoTo salto2
                                    End If




                                    Dim ptos As String() = tipo_caso_barra(4).Split(New [Char]() {"_"c, CChar(vbTab)})
                                    Dim ptos_23 As String() = ptos(0).Split(New [Char]() {","c, CChar(vbTab)})
                                    Dim ptos_45 As String() = ptos(1).Split(New [Char]() {","c, CChar(vbTab)})

                                    Dim x_prom As Single = (ptos_23(0) + 0 + ptos_45(0)) / 2
                                    Dim y_prom As Single = (ptos_23(1) + 0 + ptos_45(1)) / 2
                                    Dim tipo_losa(9) As String



                                    Dim _largo(2) As String
                                    _largo(1) = "0@0@0"

                                    If Not IsNothing(tipo_caso_barra(6)) AndAlso tipo_caso_barra(6) <> "" Then

                                        Dim ptos2 As String() = Replace(Replace(tipo_caso_barra(6), ")", ""), "(", "").Split(New [Char]() {"+"c, CChar(vbTab)})


                                        If ptos2.Length = 2 Then
                                            _largo(0) = ptos2(0) + ptos2(1)
                                        ElseIf ptos2.Length = 3 Then
                                            _largo(0) = ptos2(0) + 0 + ptos2(1) + 0 + ptos2(2)
                                        ElseIf ptos2.Length = 4 Then
                                            _largo(0) = ptos2(0) + 0 + ptos2(1) + 0 + ptos2(2) + 0 + ptos2(3)
                                        End If
                                    Else
                                        _largo(0) = ""
                                    End If



                                    Dim split_ As String() = Nothing

                                    For Each idObj As ObjectId In acObjId_grup
                                        ' Dim acEnt_aux As Entity = acTrans.GetObject(idObj, OpenMode.ForWrite)

                                        Dim atributo As New atributos()
                                        If idObj.ObjectClass.DxfName.ToString = "INSERT" And IsNothing(tipo_losa(0)) And IsNothing(tipo_losa(1)) And IsNothing(tipo_losa(2)) Then
                                            atributo.obtener_atributos_funda(idObj, tipo_losa)


                                            If Not (IsNothing(tipo_losa(0)) And IsNothing(tipo_losa(1)) And IsNothing(tipo_losa(2))) AndAlso
                                                InStr(1, LCase(tipo_losa(0)), "%%c") <> 0 Or InStr(1, LCase(tipo_losa(1)), "%%c") <> 0 And InStr(1, LCase(tipo_losa(2)), "%%c") <> 0 Then

                                                Dim aux_etxt As String = ""

                                                If InStr(1, LCase(tipo_losa(0)), "%%c") <> 0 Then
                                                    aux_etxt = Replace(LCase(Replace(LCase(tipo_losa(0)), "f'=%%c", "")), "%%c", "")
                                                ElseIf InStr(1, LCase(tipo_losa(1)), "%%c") <> 0 Then
                                                    aux_etxt = Replace(LCase(Replace(LCase(tipo_losa(1)), "f'=%%c", "")), "%%c", "")
                                                ElseIf InStr(1, LCase(tipo_losa(2)), "%%c") <> 0 Then
                                                    aux_etxt = Replace(LCase(Replace(LCase(tipo_losa(2)), "f'=%%c", "")), "%%c", "")
                                                End If


                                                split_ = aux_etxt.Split(New [Char]() {"a"c, CChar(vbTab)})
                                            End If


                                        End If

                                    Next


                                    If Not IsNothing(split_) AndAlso Not (split_(1) = "" And split_(0) = "") Then


                                        Dim espaciamiento, diametro As Integer
                                        If split_.Length = 2 Then
                                            diametro = split_(0)
                                            espaciamiento = split_(1)
                                        End If



                                        CODIGOS_DATOS_.addData_PROG2(id, tipo_caso_barra(0), New Point2d(CSng(ptos_23(0)), CSng(ptos_23(1))), New Point2d(CSng(ptos_45(0)), CSng(ptos_45(1))), diametro, _largo(0), _largo(1), espaciamiento, tipo_caso_barra(2))
                                    Else


                                    End If


                                End If

                            End If

salto2:
                        Next

                    End Using
                End If




            End Using
        End Using
final:
    End Sub





    Function dibujar_barra_reac_fund(ByVal id_ As ObjectId, ByVal datos_losa As String(), ByVal casos_dibujar As String, ByVal comando As String, ByVal datos_barra As String(), ByVal CUANTIA As String)
        ' Obtener el documento y la base de datos actuales
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        Dim acDocEd As Editor = Application.DocumentManager.MdiActiveDocument.Editor
        'Dim _acSSet_LINEA As SelectionSet


        Dim utiles_aux As New utiles

        Dim _direccion_LINEA As String = datos_losa(2)

        'If casos_dibujar = "f16" Or casos_dibujar = "f17" Or casos_dibujar = "f18" Or casos_dibujar = "f19" Then
        '    Dim split_ As String() = datos_losa(4).Split(New [Char]() {","c, "_"c, CChar(vbTab)})
        '    _currentPoint = New Point3d((split_(0) + 0.0 + split_(2)) / 2, (split_(1) + 0.0 + split_(3)) / 2, 0)
        'End If

        ' Iniciar una transaccion
        Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()

            Try





                ' Abrir la tabla para bloques en modo lectura
                Dim acBlkTbl As BlockTable
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead)

                ' Abrir el registro del bloque de Espacio Modelo para escritura
                Dim acBlkTblRec As BlockTableRecord
                acBlkTblRec = acTrans.GetObject(acBlkTbl(BlockTableRecord.ModelSpace),
                                                OpenMode.ForWrite)
                Dim AUX As Single = Rnd() * 50
                ' Crear una linea que comienza en (5,5,0) y termina en (12,3,0)
                Dim delta_x, delta_y As Single


                If _direccion_LINEA = "horizontal_i" Or _direccion_LINEA = "horizontal_d" Then
                    delta_x = 2
                    delta_y = 0
                Else
                    delta_x = 0
                    delta_y = 2
                End If
                Dim acObjIdColl_grupo As ObjectIdCollection = New ObjectIdCollection()


                Dim tipo_losa(9) As String

                If comando = "COPY,MOVE" Then


                ElseIf comando = "STRECH" Then

                    Dim acObjId_grup As ObjectId()
                    Dim GRUPO_ As New CODIGOS_GRUPOS()
                    acObjId_grup = GRUPO_.buscar_grupo(id_)
                    'acObjIdColl_grupo.Add(id_)
                    If Not (acObjId_grup Is Nothing) Then

                        For Each idObj As ObjectId In acObjId_grup
                            If idObj.ObjectClass.DxfName.ToString = "TEXT" Then
                                Dim acEnt_barra_aux As DBText = TryCast(acTrans.GetObject(idObj, OpenMode.ForWrite), DBText)
                                acEnt_barra_aux.Erase()
                            End If
                        Next


                        Dim atributo As New atributos()
                        If IsNothing(acObjId_grup) Then
                            MsgBox("Losa no agrupada, revisar", vbCritical)
                            GoTo salir
                        End If



                        'SE BUSCA DENTRO DEL GRUPO SELECCIONADO ALGUNO QUE SE PARESCA AL DE REFERECIA
                        ' SI ENCUERTA BORRA TODO 
                        'For Each idObj_ As ObjectId In acObjId_grup
                        '    If idObj_.ObjectClass.DxfName.ToString = "INSERT" Then
                        '        atributo.obtener_atributos(idObj_, tipo_losa)
                        '        datos = largo_min(idObj_)
                        '        largo_minn = datos(0)
                        '    End If
                        'Next
                        Dim codigo_varios As New CODIGOS_DATOS()

                        Dim _fundacion As New FUNDACIONES
                        Dim split_2 As String() = datos_losa(4).Split(New [Char]() {","c, "_"c, CChar(vbTab)})
                        Dim ptoini As Point2d = New Point2d(split_2(0), split_2(1))
                        Dim ptofin As Point2d = New Point2d(split_2(2), split_2(3))
                        Dim angulobarra As Single = utiles_aux.coordenada__angulo_p1_p2_fun(New Point3d(datos_barra(4), datos_barra(5), 0), New Point3d(datos_barra(2), datos_barra(3), 0), acDocEd, Nothing)
                        Dim split_ As String() = datos_barra(7).Split(New [Char]() {"@"c, CChar(vbTab)})
                        Dim largo_minn As Single = split_(0)
                        Dim espesoor As Single = split_(1)
                        Dim espesoor_real As Single = split_(2)

                        'Dim ent As Polyline = TryCast(acTrans.GetObject(id_, OpenMode.ForWrite), Polyline)
                        'Dim lista As String()
                        'lista = largo_poly_losa(ent, casos_dibujar)

                        'PLANTA_.dibujar_texto_pl(acObjIdColl_grupo, ptoini, ptofin, angulobarra, "F'=%%C" & datos_barra(1) & "a" & datos_barra(8), casos_dibujar, _direccion_LINEA, 0.0, 0.0, 0.0, 0.0, espesoor, largo_minn, datos_losa(5), 0, espesoor_real, PLANTA_.punto_aux1, PLANTA_.punto_aux2)


                        _fundacion.dibujar_texto_pl_reactor(acObjIdColl_grupo, id_, CUANTIA, datos_losa(5), datos_losa(6).Replace("+0+", "+"))



                        Dim split_23 As String() = Replace(Replace(Replace(LCase(CUANTIA), "f=", ""), "f'=", ""), "%%c", "").Split(New [Char]() {"a"c, CChar(vbTab)})

                        If IsNumeric(split_23(0)) And IsNumeric(split_23(1)) Then
                            codigo_varios.addData_PROG_LOSA(id_, datos_barra(0), ptofin, ptoini, split_23(0), datos_losa(5), "0", split_23(1), datos_barra(9), 0.0, 0.0) ', largo, My1Commands.datos_barra(0))
                        Else
                            MsgBox("Error en formato cuantia de barra, no se guardo informacion en barra (dibujar_barra_reac_fund)", vbInformation)
                        End If
                        Dim nombre_grupo As String = GRUPO_.buscar_nombre_grupo(id_)
                        If acObjIdColl_grupo.Count <> 0 Then GRUPO_.agregar_borrar_grupo(acObjIdColl_grupo, nombre_grupo, "A")

                    Else
                        MsgBox("Elemento  no agrupado(dibujar_barra_reac_fund)", vbInformation)
                    End If
                    GoTo salir
                End If
salir:
                acTrans.Commit()
            Catch Ex As Autodesk.AutoCAD.Runtime.Exception
                acTrans.Dispose()
                Application.ShowAlertDialog("Error: 'reac_modif_barra_final'" & vbLf & Ex.Message)
            Finally
            End Try

            Application.DocumentManager.MdiActiveDocument.Editor.UpdateScreen()
            Application.DocumentManager.MdiActiveDocument.Editor.Regen()
        End Using
        Return True
    End Function



    'RE ESCRIBE DATOS DE BARRA SELECCONAODS
    Public Sub aux_reescribir_barras(ByRef acObjIdColl_GENERAL As ObjectIdCollection, ByVal tipo_selec As String)
        If acObjIdColl_GENERAL <> Nothing Then acObjIdColl_GENERAL.Clear()
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acEd As Editor = acDoc.Editor

        Dim clases_util As New utiles
        Dim titulo_plano As String = clases_util.NOMBRE_PLANO_F().ToLower

        If (InStr(1, titulo_plano, "planta") <> "0" Or InStr(1, titulo_plano, "eleva") <> "0") And InStr(1, titulo_plano, "fund") = "0" Then
            acEd.WriteMessage("Plano no es de fundaciones, no se analiza" & vbLf)
            Exit Sub
        End If
        ' Crear una matriz de objetos TypedValue para definir el criterio de seleccion
        Dim osmode_ini As Integer = Application.GetSystemVariable("PICKSTYLE")
        Application.SetSystemVariable("PICKSTYLE", 1)


        Dim acTypValAr(1) As TypedValue
        acTypValAr.SetValue(New TypedValue(DxfCode.Start, "LWPOLYLINE"), 0)
        acTypValAr.SetValue(New TypedValue(DxfCode.LayerName, "BARRAS"), 1)



        ' Asignar el criterio de seleccion al objeto SelectionFilter
        Dim acSelFtr As SelectionFilter = New SelectionFilter(acTypValAr)
        ' Solicitar al usuario que seleccione objetos en el área de dibujo
        Dim acSSPrompt As PromptSelectionResult

        If tipo_selec = "all" Then
            acSSPrompt = acEd.SelectAll(acSelFtr)
        ElseIf tipo_selec = "all_rev" Then
            acSSPrompt = acEd.SelectAll(acSelFtr)
        ElseIf tipo_selec = "sel_rev" Then
            acSSPrompt = acEd.GetSelection(acSelFtr)
        Else
            acSSPrompt = acEd.GetSelection(acSelFtr)
        End If




        ' Si el estado de la solicitud es OK, es que se han seleccionado objetos
        If acSSPrompt.Status = PromptStatus.OK Then
            Dim acSSet As SelectionSet = acSSPrompt.Value

            ' Recorrer el segundo conjunto de seleccion
            For Each acObjId As ObjectId In acSSet.GetObjectIds()

                If acObjIdColl_GENERAL Is Nothing Then
                    acObjIdColl_GENERAL = New ObjectIdCollection(acSSet.GetObjectIds())
                ElseIf Not acObjIdColl_GENERAL.Contains(acObjId) Then
                    acObjIdColl_GENERAL.Add(acObjId)
                End If

            Next
            'ANTES ZOOOM
            Dim pMin As Point3d
            Dim pMax As Point3d
            Using acView As ViewTableRecord = acDoc.Editor.GetCurrentView()
                pMin = New Point3d(Application.GetSystemVariable("viewctr").X - (acView.Width / 2), Application.GetSystemVariable("viewctr").Y - (acView.Height / 2), 0)
                pMax = New Point3d((acView.Width / 2) + Application.GetSystemVariable("viewctr").X, (acView.Height / 2) + Application.GetSystemVariable("viewctr").Y, 0)
            End Using


            If tipo_selec = "all" Then
                reescribir_barras(True, My1Commands.acObjIdColl_GENERAL)
            ElseIf tipo_selec = "all_rev" Or tipo_selec = "sel_rev" Then
                revisar_barras(True, My1Commands.acObjIdColl_GENERAL)
            Else
                reescribir_barras(False, My1Commands.acObjIdColl_GENERAL)
            End If


            Application.SetSystemVariable("PICKSTYLE", osmode_ini)
            'DESPUES ZOOOM
            clases_util.Zoom(pMin, pMax, New Point3d(), 1)
            Application.DocumentManager.MdiActiveDocument.Editor.UpdateScreen()
            Application.DocumentManager.MdiActiveDocument.Editor.Regen()
        End If


    End Sub



    Public Sub reescribir_barras(ByVal recopiar_datos_inter As Boolean, ByRef acObjIdColl_GENERAL_ As ObjectIdCollection)

        If Not Directory.Exists(cots) Then
            ''Global.System.Windows.Forms.Application.Exit()
        End If

        '*************************
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


        Dim utiles_aux As New utiles
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        Dim direccion_linea_texto As String = ""
        Dim ed As Editor = Application.DocumentManager.MdiActiveDocument.Editor
        Dim aux_inicia_pregunta As Boolean = True
asdasd:
        Dim CODIGOS_GRUPOS_ As New CODIGOS_GRUPOS()
        Dim CODIGOS_DATOS_ As New CODIGOS_DATOS()

        Using acDoc.LockDocument()

            If acObjIdColl_GENERAL_ <> Nothing Then
                ' Iniciar una transaccion
                Using acTrans2 As Transaction = acCurDb.TransactionManager.StartTransaction()
reiniciar:
                    '  Application.ShowAlertDialog("My1Commands.acObjIdColl_GENERAL: " & My1Commands.acObjIdColl_GENERAL.Count)
                    ' Iterar en la colección 

                    Using acPm As New ProgressMeter()
                        '  Establecer el valor maximo del intervalo
                        acPm.SetLimit(acObjIdColl_GENERAL_.Count)
                        ' Mostrar la barra de progreso
                        acPm.Start("Analizando..")


                        For Each acObjId As ObjectId In acObjIdColl_GENERAL_

                            Dim acObjId_grup As ObjectId() = CODIGOS_GRUPOS_.buscar_grupo(acObjId)

                            System.Threading.Thread.Sleep(acObjIdColl_GENERAL_.Count / 100)
                            ' Incrementar la posicion
                            acPm.MeterProgress()
                            ' Permitir que la ventana de AutoCAD se redibuje correctamente
                            System.Windows.Forms.Application.DoEvents()


                            If Not (acObjId_grup Is Nothing) Then


                                Dim aux_text = False
                                For Each idObj As ObjectId In acObjId_grup
                                    ' Dim acEnt_aux As Entity = acTrans.GetObject(idObj, OpenMode.ForWrite)
                                    If idObj.ObjectClass.DxfName.ToString = "INSERT" Then aux_text = True

                                Next

                                If aux_text = False Then GoTo salto2


                                Dim acPoly As Polyline = TryCast(acTrans2.GetObject(acObjId, OpenMode.ForWrite), Polyline)
                                Dim tipo_barra(9) As String

                                CODIGOS_DATOS_.getData_PROG2(acPoly, tipo_barra, True)
                                If acPoly.NumberOfVertices = 2 And tipo_barra(0) = "f16" Then
                                    tipo_barra(0) = "f3"
                                End If

                                If (Not (Not IsNothing(tipo_barra(0)) AndAlso (tipo_barra(0) = "f3" Or tipo_barra(0) = "f9" Or tipo_barra(0) = "f9a" Or tipo_barra(0) = "f9a_V" Or tipo_barra(0) = "f10" Or tipo_barra(0) = "f11")) AndAlso
                                    (IsNumeric(tipo_barra(1)) AndAlso CInt(tipo_barra(1)) < 36) AndAlso
                                    (IsNumeric(tipo_barra(8)) AndAlso CInt(tipo_barra(8)) < 100) AndAlso
                                    (Not IsNothing(tipo_barra(7)) AndAlso tipo_barra(7) = "0@0@0")) Then
                                    GoTo salto
                                End If

                                'If Not (tipo_barra(0) = "f3" Or tipo_barra(0) = "f9" Or tipo_barra(0) = "f9a" Or tipo_barra(0) = "f9a_V" Or tipo_barra(0) = "f10" Or tipo_barra(0) = "f11") Then
                                '    GoTo salto
                                'End If


                                Dim tipo_caso_barra() As String = utiles_aux.TIPO_caso_BARRA_LOSA_ind_fund(acPoly, tipo_barra(0), tipo_barra)

                                ' TIPO_caso_BARRA_LOSA_ind_fund(acPoly, tipo_barra(0), tipo_caso_barra)

                                If IsNothing(tipo_caso_barra(0)) Then
                                    utiles_aux.marca_error_barra_f(acObjId, "", False, True)
                                    If recopiar_datos_inter = False Then MsgBox("error   TIPO_caso_BARRA_ind, revisar barra", vbInformation)
                                    GoTo salto2
                                End If


                                Dim ptos As String() = tipo_caso_barra(4).Split(New [Char]() {"_"c, CChar(vbTab)})
                                Dim ptos_23 As String() = ptos(0).Split(New [Char]() {","c, CChar(vbTab)})
                                Dim ptos_45 As String() = ptos(1).Split(New [Char]() {","c, CChar(vbTab)})

                                Dim x_prom As Single = (ptos_23(0) + 0 + ptos_45(0)) / 2
                                Dim y_prom As Single = (ptos_23(1) + 0 + ptos_45(1)) / 2
                                Dim tipo_losa(9) As String




                                Dim _largo(2) As String
                                _largo(1) = "0@0@0"

                                If Not IsNothing(tipo_caso_barra(6)) AndAlso tipo_caso_barra(6) <> "" Then

                                    Dim ptos2 As String() = Replace(Replace(tipo_caso_barra(6), ")", ""), "(", "").Split(New [Char]() {"+"c, CChar(vbTab)})


                                    If ptos2.Length = 2 Then
                                        _largo(0) = CDbl(ptos2(0)) + CDbl(ptos2(1))
                                    ElseIf ptos2.Length = 3 Then
                                        _largo(0) = CDbl(ptos2(0)) + CDbl(ptos2(1)) + CDbl(ptos2(2))
                                    ElseIf ptos2.Length = 4 Then
                                        _largo(0) = CDbl(ptos2(0)) + CDbl(ptos2(1)) + CDbl(ptos2(2)) + CDbl(ptos2(3))
                                    End If
                                Else
                                    _largo(0) = tipo_caso_barra(5)
                                End If

                                Dim split_ As String() = Nothing

                                Dim valore_actual As String = ""
                                For Each idObj As ObjectId In acObjId_grup
                                    ' Dim acEnt_aux As Entity = acTrans.GetObject(idObj, OpenMode.ForWrite)

                                    Dim atributo As New atributos()
                                    If idObj.ObjectClass.DxfName.ToString = "INSERT" And IsNothing(tipo_losa(0)) And IsNothing(tipo_losa(1)) And IsNothing(tipo_losa(2)) Then
                                        atributo.obtener_atributos_funda(idObj, tipo_losa)


                                        If Not (IsNothing(tipo_losa(0)) And IsNothing(tipo_losa(1)) And IsNothing(tipo_losa(2))) AndAlso
                                           (InStr(1, LCase(tipo_losa(0)), "%%c") <> 0 Or InStr(1, LCase(tipo_losa(1)), "%%c") <> 0 Or InStr(1, LCase(tipo_losa(2)), "%%c") <> 0) Then

                                            Dim aux_etxt As String = ""

                                            If InStr(1, LCase(tipo_losa(0)), "%%c") <> 0 Then
                                                aux_etxt = Replace(LCase(Replace(Replace(LCase(tipo_losa(0)), "'", ""), "f=%%c", "")), "%%c", "")
                                            ElseIf InStr(1, LCase(tipo_losa(1)), "%%c") <> 0 Then
                                                aux_etxt = Replace(LCase(Replace(Replace(LCase(tipo_losa(1)), "'", ""), "f=%%c", "")), "%%c", "")
                                            ElseIf InStr(1, LCase(tipo_losa(2)), "%%c") <> 0 Then
                                                aux_etxt = Replace(LCase(Replace(Replace(LCase(tipo_losa(2)), "'", ""), "f=%%c", "")), "%%c", "")
                                            End If

                                            split_ = aux_etxt.Split(New [Char]() {"a"c, CChar(vbTab)})

                                        ElseIf Not (IsNothing(tipo_losa(0)) And IsNothing(tipo_losa(1)) And IsNothing(tipo_losa(2))) AndAlso
                                            (InStr(1, LCase(tipo_losa(0)), "l=") <> 0 Or InStr(1, LCase(tipo_losa(1)), "l=") <> 0 Or InStr(1, LCase(tipo_losa(2)), "l=") <> 0) Then
                                        ElseIf Not (IsNothing(tipo_losa(0)) And IsNothing(tipo_losa(1)) And IsNothing(tipo_losa(2))) AndAlso
                                            (InStr(1, LCase(tipo_losa(0)), "(") <> 0 Or InStr(1, LCase(tipo_losa(1)), "(") <> 0 Or InStr(1, LCase(tipo_losa(2)), ")") <> 0) Then
                                        End If


                                        Dim blkRef As BlockReference = DirectCast(acTrans2.GetObject(idObj, OpenMode.ForWrite), BlockReference)
                                        Dim btr As BlockTableRecord = DirectCast(acTrans2.GetObject(blkRef.BlockTableRecord, OpenMode.ForWrite), BlockTableRecord)

                                        btr.Dispose()
                                        Dim attCol As AttributeCollection = blkRef.AttributeCollection

                                        For Each attId As ObjectId In attCol
                                            Dim attRef As AttributeReference = DirectCast(acTrans2.GetObject(attId, OpenMode.ForWrite), AttributeReference)
                                            ' Dim str As String = (vbLf & "  Attribute Tag: " + attRef.Tag + vbLf & "    Attribute String: " + attRef.TextString)


                                            If attRef.Tag = "CUANTIA" Then
                                                '       attRef.TextString = "cacaV"
                                            ElseIf attRef.Tag = "LARGO" Then
                                                attRef.TextString = "L=" & _largo(0)
                                            ElseIf attRef.Tag = "PARCIAL" Then
                                                attRef.TextString = tipo_caso_barra(6)
                                            End If

                                        Next


                                    End If

                                Next

                                'borrar
                                For Each idObj_ As ObjectId In acObjId_grup

                                    If idObj_.ObjectClass.DxfName.ToString = "LWPOLYLINE" Then
                                        Dim acPoly2 As Polyline = TryCast(acTrans2.GetObject(idObj_, OpenMode.ForRead), Polyline)
                                        If Not acPoly2.IsErased And acPoly2.Layer = "NHREVISION" Then
                                            Using acTrans3 As Transaction = acCurDb.TransactionManager.StartTransaction()

                                                acPoly2 = TryCast(acTrans3.GetObject(idObj_, OpenMode.ForWrite), Polyline)
                                                acPoly2.Erase()

                                                acTrans3.Commit()
                                            End Using
                                        End If
                                    ElseIf idObj_.ObjectClass.DxfName.ToString = "MTEXT" Then
                                        Dim acEnt_MTEXT As MText = TryCast(acTrans2.GetObject(idObj_, OpenMode.ForRead), MText)
                                        If Not acEnt_MTEXT.IsErased And acEnt_MTEXT.Layer = "NHREVISION" Then
                                            Using acTrans3 As Transaction = acCurDb.TransactionManager.StartTransaction()

                                                acEnt_MTEXT = TryCast(acTrans3.GetObject(idObj_, OpenMode.ForWrite), MText)
                                                acEnt_MTEXT.Erase()

                                                acTrans3.Commit()
                                            End Using
                                        End If

                                    End If
                                Next



                                If Not IsNothing(split_) AndAlso Not (split_(1) = "" And split_(0) = "") AndAlso IsNumeric(split_(0)) AndAlso IsNumeric(split_(1)) Then
                                    Dim espaciamiento, diametro As Integer
                                    If split_.Length = 2 Then
                                        diametro = split_(0)
                                        espaciamiento = split_(1)
                                        CODIGOS_DATOS_.addData_PROG2(acPoly.ObjectId, tipo_caso_barra(0), New Point2d(CSng(ptos_23(0)), CSng(ptos_23(1))), New Point2d(CSng(ptos_45(0)), CSng(ptos_45(1))), diametro, _largo(0), _largo(1), espaciamiento, tipo_caso_barra(2))
                                    End If
                                End If
salto:
                            End If
salto2:
                        Next

                        ' Ocultar la barra
                        acPm.Stop()
                    End Using

                    acTrans2.Commit()
                End Using
            Else
                Application.ShowAlertDialog("No se encontro elementos seleccionados")
            End If
        End Using
        Application.DocumentManager.MdiActiveDocument.Editor.UpdateScreen()
        Application.DocumentManager.MdiActiveDocument.Editor.Regen()
    End Sub

    Public Sub revisar_barras(ByVal recopiar_datos_inter As Boolean, ByRef acObjIdColl_GENERAL_ As ObjectIdCollection)

        If Not Directory.Exists(cots) Then
            ''Global.System.Windows.Forms.Application.Exit()
        End If

        '*************************
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

        Dim utiles_aux As New utiles

        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        Dim direccion_linea_texto As String = ""
        Dim ed As Editor = Application.DocumentManager.MdiActiveDocument.Editor
        Dim aux_inicia_pregunta As Boolean = True
asdasd:

        Dim CODIGOS_GRUPOS_ As New CODIGOS_GRUPOS()
        Dim CODIGOS_DATOS_ As New CODIGOS_DATOS()
        Dim CODIGOS_VARIOS_ As New varios_general()
        Using acDoc.LockDocument()

            Dim acObjIdColl_GENERAL_SALTAR As ObjectIdCollection = New ObjectIdCollection()

            If acObjIdColl_GENERAL_ <> Nothing Then
                ' Iniciar una transaccion
                Using acTrans2 As Transaction = acCurDb.TransactionManager.StartTransaction()
reiniciar:
                    '  Application.ShowAlertDialog("My1Commands.acObjIdColl_GENERAL: " & My1Commands.acObjIdColl_GENERAL.Count)
                    ' Iterar en la colección 

                    Using acPm As New ProgressMeter()
                        '  Establecer el valor maximo del intervalo
                        acPm.SetLimit(acObjIdColl_GENERAL_.Count)
                        ' Mostrar la barra de progreso
                        acPm.Start("Analizando..")


                        For Each acObjId As ObjectId In acObjIdColl_GENERAL_
                            Dim mje_error As String = ""


                            System.Threading.Thread.Sleep(acObjIdColl_GENERAL_.Count / 100)
                            ' Incrementar la posicion
                            acPm.MeterProgress()
                            ' Permitir que la ventana de AutoCAD se redibuje correctamente
                            System.Windows.Forms.Application.DoEvents()
                            If acObjIdColl_GENERAL_SALTAR.Contains(acObjId) Then GoTo salto2

                            Dim acObjId_grup As ObjectId() = CODIGOS_GRUPOS_.buscar_grupo(acObjId)

                            If Not (acObjId_grup Is Nothing) Then

                                Dim aux_error As Boolean = False
                                Dim aux_text = False
                                Dim aux_recorrido = False
                                For Each idObj As ObjectId In acObjId_grup
                                    ' Dim acEnt_aux As Entity = acTrans.GetObject(idObj, OpenMode.ForWrite)
                                    If idObj.ObjectClass.DxfName.ToString = "INSERT" Then aux_text = True
                                    If idObj.ObjectClass.DxfName.ToString = "DIMENSION" Then aux_recorrido = True
                                    If idObj.ObjectClass.DxfName.ToString = "LWPOLYLINE" Then acObjIdColl_GENERAL_SALTAR.Add(idObj)

                                    If CODIGOS_GRUPOS_.buscar_varios_grupo(idObj, False) Then
                                        ' mje_error = "a)Elemento con varios grupos"
                                        ' aux_error = True
                                        CODIGOS_GRUPOS_.recrear_grupo(idObj, grupo_referencia)
                                    End If

                                Next

                                If aux_text = False Then GoTo salto2





                                If aux_recorrido = False Then
                                    mje_error = mje_error & "\Pb)Barra sin recorrido"
                                    aux_error = True
                                End If

                                'utiles_aux.ZOOM_ENTITY_FUND(acObjId, 150)
                                'Application.DocumentManager.MdiActiveDocument.Editor.UpdateScreen()
                                'Application.DocumentManager.MdiActiveDocument.Editor.Regen()
                                Dim acPoly As Polyline = TryCast(acTrans2.GetObject(acObjId, OpenMode.ForRead), Polyline)
                                Dim tipo_barra(9) As String

                                CODIGOS_DATOS_.getData_PROG2(acPoly, tipo_barra, True)

                                If (Not (Not IsNothing(tipo_barra(0)) AndAlso (tipo_barra(0) = "f3" Or tipo_barra(0) = "f9" Or tipo_barra(0) = "f9a" Or tipo_barra(0) = "f9a_V" Or tipo_barra(0) = "f10" Or tipo_barra(0) = "f11")) AndAlso
                                    (IsNumeric(tipo_barra(1)) AndAlso CInt(tipo_barra(1)) < 36) AndAlso
                                    (IsNumeric(tipo_barra(8)) AndAlso CInt(tipo_barra(8)) < 100) AndAlso
                                    (Not IsNothing(tipo_barra(7)) AndAlso tipo_barra(7) = "0@0@0")) Then
                                    GoTo salto
                                End If

                                'If Not (tipo_barra(0) = "f3" Or tipo_barra(0) = "f9" Or tipo_barra(0) = "f9a" Or tipo_barra(0) = "f9a_V" Or tipo_barra(0) = "f10" Or tipo_barra(0) = "f11") Then
                                '    GoTo salto
                                'End If


                                Dim tipo_caso_barra() As String = utiles_aux.TIPO_caso_BARRA_LOSA_ind_fund(acPoly, tipo_barra(0), tipo_barra)

                                ' TIPO_caso_BARRA_LOSA_ind_fund(acPoly, tipo_barra(0), tipo_caso_barra)

                                If IsNothing(tipo_caso_barra(0)) Then
                                    ' acPoly.Color = Autodesk.AutoCAD.Colors.Color.FromRgb(255, 255, 0)
                                    utiles_aux.marca_error_barra_f(acObjId, "", False, True)
                                    If recopiar_datos_inter = False Then MsgBox("error   TIPO_caso_BARRA_ind, revisar barra", vbInformation)
                                    GoTo salto2
                                End If


                                Dim ptos As String() = tipo_caso_barra(4).Split(New [Char]() {"_"c, CChar(vbTab)})
                                Dim ptos_23 As String() = ptos(0).Split(New [Char]() {","c, CChar(vbTab)})
                                Dim ptos_45 As String() = ptos(1).Split(New [Char]() {","c, CChar(vbTab)})

                                Dim x_prom As Single = (ptos_23(0) + 0 + ptos_45(0)) / 2
                                Dim y_prom As Single = (ptos_23(1) + 0 + ptos_45(1)) / 2
                                Dim tipo_losa(9) As String




                                Dim _largo(2) As String
                                _largo(1) = "0@0@0"

                                Dim aux_existe_suma_parcial As Boolean = False
                                If Not IsNothing(tipo_caso_barra(6)) AndAlso tipo_caso_barra(6) <> "" Then

                                    Dim ptos2 As String() = Replace(Replace(tipo_caso_barra(6), ")", ""), "(", "").Split(New [Char]() {"+"c, CChar(vbTab)})
                                    aux_existe_suma_parcial = True

                                    If ptos2.Length = 2 Then
                                        _largo(0) = CDbl(ptos2(0)) + CDbl(ptos2(1))
                                    ElseIf ptos2.Length = 3 Then
                                        _largo(0) = CDbl(ptos2(0)) + CDbl(ptos2(1)) + CDbl(ptos2(2))
                                    ElseIf ptos2.Length = 4 Then
                                        _largo(0) = CDbl(ptos2(0)) + CDbl(ptos2(1)) + CDbl(ptos2(2)) + CDbl(ptos2(3))
                                    End If
                                Else
                                    _largo(0) = tipo_caso_barra(5)
                                End If

                                Dim split_ As String() = Nothing

                                Dim valore_actual As String = ""
                                For Each idObj As ObjectId In acObjId_grup
                                    ' Dim acEnt_aux As Entity = acTrans.GetObject(idObj, OpenMode.ForWrite)

                                    Dim atributo As New atributos()
                                    If idObj.ObjectClass.DxfName.ToString = "INSERT" And IsNothing(tipo_losa(0)) And IsNothing(tipo_losa(1)) And IsNothing(tipo_losa(2)) Then
                                        atributo.obtener_atributos_funda(idObj, tipo_losa)




                                        ' a)
                                        If Not (IsNothing(tipo_losa(0)) And IsNothing(tipo_losa(1)) And IsNothing(tipo_losa(2))) AndAlso
                                           (InStr(1, LCase(tipo_losa(0)), "%%c") <> 0 Or InStr(1, LCase(tipo_losa(1)), "%%c") <> 0 Or InStr(1, LCase(tipo_losa(2)), "%%c") <> 0) Then

                                            Dim aux_etxt As String = ""

                                            If InStr(1, LCase(tipo_losa(0)), "%%c") <> 0 Then
                                                aux_etxt = Replace(LCase(Replace(Replace(Replace(Replace(Replace(Replace(LCase(tipo_losa(0)), "=", ""), "f", ""), "f", ""), "+", ""), "'", ""), "=%%c", "")), "%%c", "")
                                            ElseIf InStr(1, LCase(tipo_losa(1)), "%%c") <> 0 Then
                                                aux_etxt = Replace(LCase(Replace(Replace(Replace(Replace(Replace(Replace(LCase(tipo_losa(0)), "=", ""), "f", ""), "f", ""), "+", ""), "'", ""), "=%%c", "")), "%%c", "")
                                            ElseIf InStr(1, LCase(tipo_losa(2)), "%%c") <> 0 Then
                                                aux_etxt = Replace(LCase(Replace(Replace(Replace(Replace(Replace(Replace(LCase(tipo_losa(0)), "=", ""), "f", ""), "f", ""), "+", ""), "'", ""), "=%%c", "")), "%%c", "")
                                            End If


                                            split_ = aux_etxt.Split(New [Char]() {"a"c, CChar(vbTab)})

                                            If Not (split_.Length = 2 AndAlso (IsNumeric(split_(0)) And IsNumeric(split_(1)))) Then
                                                aux_error = True
                                                mje_error = mje_error & "\Pc)Error en cuantia barra"
                                            End If
                                        Else
                                            mje_error = mje_error & "\P0)Barra sin datos internos"

                                            CODIGOS_VARIOS_.marca_error_barra_f(acObjId, mje_error, True, True)
                                            Continue For
                                        End If

                                        If Not IsNothing(tipo_losa(1)) AndAlso tipo_losa(1).ToLower().Contains("var") Then Continue For

                                        'b)
                                        If Not (IsNothing(tipo_losa(0)) And IsNothing(tipo_losa(1)) And IsNothing(tipo_losa(2))) AndAlso
                                        (InStr(1, LCase(tipo_losa(0)), "l=") <> 0 Or InStr(1, LCase(tipo_losa(1)), "l=") <> 0 Or InStr(1, LCase(tipo_losa(2)), "l=") <> 0) Then

                                            Dim aux_largo As String() = {"0"}
                                            If InStr(1, LCase(tipo_losa(0)), "l=") <> 0 Then
                                                aux_largo = tipo_losa(0).Split(New [Char]() {"="c})
                                            ElseIf InStr(1, LCase(tipo_losa(1)), "l=") <> 0 Then
                                                aux_largo = tipo_losa(1).Split(New [Char]() {"="c})
                                            ElseIf InStr(1, LCase(tipo_losa(2)), "l=") <> 0 Then
                                                aux_largo = tipo_losa(2).Split(New [Char]() {"="c})
                                            End If


                                            If aux_largo.Length = 2 AndAlso CSng(aux_largo(1)) <> CSng(_largo(0)) Then
                                                aux_error = True
                                                mje_error = mje_error & "\Pd)Largo Total texto(L=" & aux_largo(1) & ")  <>  Largo Total Real (" & _largo(0) & ")"
                                            ElseIf Not (aux_largo.Length = 2 AndAlso (aux_largo(0).ToLower = "l" And IsNumeric(aux_largo(1)))) Then
                                                aux_error = True
                                                mje_error = mje_error & "\Pe)Error en Largo Total texto(L=" & aux_largo(1) & ")"
                                            End If

                                            If CSng(_largo(0)) > 1200 Or CSng(aux_largo(1)) > 1200 Then
                                                mje_error = mje_error & "\Pc) Largo de barra mayor a 12mt "
                                                aux_error = True
                                            End If


                                        Else
                                            mje_error = mje_error & "\P0)Barra sin datos internos"
                                            CODIGOS_VARIOS_.marca_error_barra_f(acObjId, mje_error, True, True)
                                            Continue For
                                        End If


                                        'c)
                                        If (Not (IsNothing(tipo_losa(0)) And IsNothing(tipo_losa(1)) And IsNothing(tipo_losa(2))) And aux_existe_suma_parcial) AndAlso
                                        (InStr(1, LCase(tipo_losa(0)), "(") <> 0 Or InStr(1, LCase(tipo_losa(1)), "(") <> 0 Or InStr(1, LCase(tipo_losa(2)), ")") <> 0) Then

                                            Dim aux_largo As String() = {"0"}
                                            If InStr(1, LCase(tipo_losa(0)), "(") <> 0 Then
                                                aux_largo = Replace(Replace(tipo_losa(0), ")", ""), "(", "").Split(New [Char]() {"+"c, CChar(vbTab)})
                                            ElseIf InStr(1, LCase(tipo_losa(1)), "(") <> 0 Then
                                                aux_largo = Replace(Replace(tipo_losa(1), ")", ""), "(", "").Split(New [Char]() {"+"c, CChar(vbTab)})
                                            ElseIf InStr(1, LCase(tipo_losa(2)), "(") <> 0 Then
                                                aux_largo = Replace(Replace(tipo_losa(2), ")", ""), "(", "").Split(New [Char]() {"+"c, CChar(vbTab)})
                                            End If


                                            Dim largo_texto As Single = 0
                                            If aux_largo.Length = 2 AndAlso (IsNumeric(aux_largo(0)) And IsNumeric(aux_largo(1))) Then
                                                largo_texto = CDbl(aux_largo(0)) + CDbl(aux_largo(1))
                                            ElseIf aux_largo.Length = 3 AndAlso (IsNumeric(aux_largo(0)) And IsNumeric(aux_largo(1)) And IsNumeric(aux_largo(2))) Then
                                                largo_texto = CDbl(aux_largo(0)) + CDbl(aux_largo(1)) + CDbl(aux_largo(2))
                                            ElseIf aux_largo.Length = 4 AndAlso (IsNumeric(aux_largo(0)) And IsNumeric(aux_largo(1)) And IsNumeric(aux_largo(2)) And IsNumeric(aux_largo(3))) Then
                                                largo_texto = CDbl(aux_largo(0)) + CDbl(aux_largo(1)) + CDbl(aux_largo(2)) + CDbl(aux_largo(3))
                                            End If

                                            If largo_texto <> CSng(_largo(0)) Then
                                                aux_error = True
                                                mje_error = mje_error & "\Pf)Largo suma parcial texto(a+b+c)= (" & largo_texto & ")  <>  Largo Total Real(" & _largo(0) & ")"
                                            End If


                                        ElseIf aux_existe_suma_parcial = False Then
                                            ' no hace nada
                                        Else
                                            aux_error = True
                                            mje_error = mje_error & "\Pg)Error Largo suma parcial texto(a+b+c)"
                                        End If


                                        If aux_error Then
                                            ' acPoly.Color = Autodesk.AutoCAD.Colors.Color.FromRgb(255, 255, 0)
                                            CODIGOS_VARIOS_.marca_error_barra_f(acObjId, mje_error, True, True)
                                            If recopiar_datos_inter = False Then MsgBox("error   TIPO_caso_BARRA_ind, revisar barra", vbInformation)
                                            GoTo salto2
                                        End If

                                        If Not IsNothing(split_) AndAlso Not (split_(1) = "" And split_(0) = "") AndAlso IsNumeric(split_(0)) AndAlso IsNumeric(split_(1)) Then
                                            Dim espaciamiento, diametro As Integer
                                            If split_.Length = 2 Then
                                                diametro = split_(0)
                                                espaciamiento = split_(1)
                                                CODIGOS_DATOS_.addData_PROG2(acPoly.ObjectId, tipo_caso_barra(0), New Point2d(CSng(ptos_23(0)), CSng(ptos_23(1))), New Point2d(CSng(ptos_45(0)), CSng(ptos_45(1))), diametro, _largo(0), _largo(1), espaciamiento, tipo_caso_barra(2))
                                            End If
                                        End If



                                    End If

                                Next


salto:
                            End If
salto2:
                        Next

                        ' Ocultar la barra
                        acPm.Stop()
                    End Using

                    acTrans2.Commit()
                End Using
            Else
                Application.ShowAlertDialog("No se encontro elementos seleccionados")
            End If
        End Using
        Application.DocumentManager.MdiActiveDocument.Editor.UpdateScreen()
        Application.DocumentManager.MdiActiveDocument.Editor.Regen()
    End Sub



    Public Sub ESTRIBO_FUNDACIONES()

        Dim utiles_aux As New utiles

        Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()


        Dim acDoc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
        Dim acEd As Editor = acDoc.Editor

        If Not My1Commands.acObjIdColl_GENERAL Is Nothing Then
            My1Commands.acObjIdColl_GENERAL.Clear()
        End If


        If My1Commands.myPalette.rdb_dibujar_estribo.Checked = False Then

            Dim acTypValAr(4) As TypedValue
            acTypValAr.SetValue(New TypedValue(DxfCode.Start, "HATCH"), 0)
            acTypValAr.SetValue(New TypedValue(DxfCode.Operator, "<or"), 1)
            acTypValAr.SetValue(New TypedValue(DxfCode.LayerName, "CESTRIBO_E"), 2)
            acTypValAr.SetValue(New TypedValue(DxfCode.LayerName, "CESTRIBO_L"), 3)
            acTypValAr.SetValue(New TypedValue(DxfCode.Operator, "or>"), 4)



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

                My1Commands.myPalette.Tsl_seleccion.Text = "Elementos Seleccionados:" & acSSet.Count
            Else
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Cantidad de objetos seleccionados: 0")
                My1Commands.myPalette.Tsl_seleccion.Text = "Elementos Seleccionados:0"
            End If

        End If







        Dim texto_estribo As String = ""
        Dim texto_traba As String = ""
        Dim texto_lateral As String = ""
        Dim texto_espesor As String = ""

        ' texto_estribo = cbx_estribo.Text & " %%c" & cbx_diam_estr.Text & "a" & cbx_espa_estr.Text
        If My1Commands.myPalette.rbn_modif_estribo.Checked = True Then
            texto_estribo = My1Commands.myPalette.cbx_estribo.Text & " %%c" & My1Commands.myPalette.cbx_diam_estr.Text & "a" & My1Commands.myPalette.cbx_espa_estr.Text
        Else
            texto_estribo = My1Commands.myPalette.cbx_estribo.Text & ",%%c" & My1Commands.myPalette.cbx_diam_estr.Text & "a" & My1Commands.myPalette.cbx_espa_estr.Text
        End If

        texto_espesor = "h=" & My1Commands.myPalette.txtbx_espesor.Text

        If My1Commands.myPalette.chbx_tr.Checked = True Then
            texto_traba = "+" & My1Commands.myPalette.cbx_tr.Text & "TR.%%c" & My1Commands.myPalette.cbx_diam_tr.Text & "a" & My1Commands.myPalette.cbx_espa_tr.Text
        Else
            texto_traba = ""
        End If





        If texto_traba = "" And texto_estribo = "" And texto_lateral = "" Then
            Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Debe seleccionar alguna opcion ")
            GoTo salto_final
        End If

        Dim ESTRIBO_ As New ESTRIBOS()

        utiles_aux.Create_ALayer("CESTRIBO_E")
        utiles_aux.Create_ALayer("CESTRIBO_L")
        utiles_aux.Create_ALayer("ESTRIBO")

        If My1Commands.myPalette.rbn_modif_estribo.Checked = True Then

            If (My1Commands.acObjIdColl_GENERAL = Nothing) Or (Not My1Commands.acObjIdColl_GENERAL = Nothing AndAlso My1Commands.acObjIdColl_GENERAL.Count = 0) Then
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("NO existen elementos seleccionados")
                GoTo salto_final
            End If

            ESTRIBO_.botom_aceptar_modificar(texto_estribo, texto_lateral, texto_traba, texto_espesor, My1Commands.acObjIdColl_GENERAL)

        ElseIf My1Commands.myPalette.rdb_dibujar_estribo.Checked = True Then

            Dim ptStart As Point3d
            Dim ptfin As Point3d
            Dim angulo As Single
            Dim angulo_malla As Single
            Dim aux_orientacio As String



            If My1Commands.myPalette.rbm_vertical.Checked = True Then

                angulo = 0
                angulo_malla = Math.PI / 2
                aux_orientacio = "V"
            ElseIf My1Commands.myPalette.rbm_horizontal.Checked = True Then
                angulo = Math.PI / 2
                angulo_malla = 0
                aux_orientacio = "H"
            Else
                Dim pPtRes As PromptPointResult
                Dim pPtOpts As PromptPointOptions = New PromptPointOptions("")
                pPtOpts.AllowNone = True
                ' Solicitar el punto inicial
                pPtOpts.Message = String.Format("{0}Precise el punto inicial para dar direccion: ", ControlChars.Lf)
                pPtRes = acDoc.Editor.GetPoint(pPtOpts)
                If pPtRes.Status = PromptStatus.None Or pPtRes.Status = PromptStatus.Cancel Then
                    Exit Sub
                End If
                ptStart = pPtRes.Value


                Dim pPtRes2 As PromptPointResult
                Dim pPtOpts2 As PromptPointOptions = New PromptPointOptions("")
                pPtOpts2.AllowNone = True
                ' Solicitar el punto inicial
                pPtOpts2.Message = String.Format("{0}Precise el punto final para dar direccion: ", ControlChars.Lf)
                pPtRes2 = acDoc.Editor.GetPoint(pPtOpts2)
                If pPtRes2.Status = PromptStatus.None Or pPtRes2.Status = PromptStatus.Cancel Then
                    Exit Sub
                End If
                ptfin = pPtRes2.Value

                angulo = utiles_aux.coordenada__angulo_p1_p2_fun(ptStart, ptfin, acEd, Nothing)
                If angulo >= 0 Then
                    angulo_malla = angulo - Math.PI / 2
                ElseIf angulo < 0 Then
                    angulo_malla = angulo + Math.PI / 2

                End If
                aux_orientacio = "O"
            End If


            'botom_aceptar_dibujar(texto_estribo, texto_lateral, texto_traba, texto_espesor)
            'ESTRIBO_.botom_aceptar_dibujar(texto_estribo, "", texto_traba, "0", angulo, ptStart, ptfin, My1Commands.grupo_referencia, angulo_malla, aux_orientacio)
            ESTRIBO_.botom_aceptar_dibujar_estribo(texto_estribo, "", texto_traba, texto_espesor, angulo, ptStart, ptfin, My1Commands.grupo_referencia, angulo_malla, aux_orientacio, True)


        End If


salto_final:
    End Sub



End Class
