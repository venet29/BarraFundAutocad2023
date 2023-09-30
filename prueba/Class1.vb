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



Public Class Class1


    <CommandMethod("nhn")> _
    Public Sub nhn()

        '1) lista nivel
        Dim ds_lista_nivel As New DataSet
        Dim dt_lista_nivel As New System.Data.DataTable

        dt_lista_nivel.Columns.Add("coor_z", GetType(Double))
        dt_lista_nivel.Columns.Add("nivel", GetType(String))
        ds_lista_nivel.Tables.Add(dt_lista_nivel)

        '2)  nueva lista -- con coordenadas de los niveles
        Dim ds_nueva_lista As New DataSet
        Dim dt_nueva_lista As New System.Data.DataTable

        dt_nueva_lista.Columns.Add("cero", GetType(String))
        dt_nueva_lista.Columns.Add("coor_z_nivel", GetType(Double))
        ds_nueva_lista.Tables.Add(dt_nueva_lista)

        '3)  nueva final -- con coordenadas de los niveles
        Dim ds_lista_final As New DataSet
        Dim dt_lista_final As New System.Data.DataTable

        dt_lista_final.Columns.Add("nivel", GetType(String))
        dt_lista_final.Columns.Add("coor_z", GetType(Double))
        ds_lista_final.Tables.Add(dt_lista_final)


        '4)  nueva final -- con coordenadas de los niveles
        Dim ds_lista_final2 As New DataSet
        Dim dt_lista_final2 As New System.Data.DataTable

        dt_lista_final2.Columns.Add("nivel", GetType(String))
        dt_lista_final2.Columns.Add("coor_z", GetType(Double))
        ds_lista_final2.Tables.Add(dt_lista_final2)


        '' Obtener el editor del documento actual
        'Dim acDocEd As Editor = Application.DocumentManager.MdiActiveDocument.Editor

        '' Crear una matriz de objetos TypedValue para definir el criterio de seleccion
        'Dim acTypValAr(1) As TypedValue
        'acTypValAr.SetValue(New TypedValue(DxfCode.Start, "TEXT"), 0)
        'acTypValAr.SetValue(New TypedValue(DxfCode.LayerName, "0"), 1)

        '' Asignar el criterio de seleccion al objeto SelectionFilter
        'Dim acSelFtr As SelectionFilter = New SelectionFilter(acTypValAr)

        '' Solicitar al usuario que seleccione objetos en el área de dibujo
        'Dim acSSPrompt As PromptSelectionResult
        'acSSPrompt = acDocEd.GetSelection(acSelFtr)

        '' Si el estado de la solicitud es OK, es que se han seleccionado objetos
        'If acSSPrompt.Status = PromptStatus.OK Then
        '    Dim acSSet As SelectionSet = acSSPrompt.Value

        '    Application.ShowAlertDialog("Cantidad de objetos seleccionados: " & _
        '                                acSSet.Count.ToString())
        'Else
        '    Application.ShowAlertDialog("Cantidad de objetos seleccionados: 0")
        'End If


        Dim ed As Editor = Application.DocumentManager.MdiActiveDocument.Editor
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        Dim acEd As Editor = acDoc.Editor

        ' Iniciar la interacion
        ' Iniciar una transaccion
        Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()



            ' Crear una matriz de objetos TypedValue para definir el criterio de seleccion

            Dim acTypValAr(9) As TypedValue


            acTypValAr.SetValue(New TypedValue(DxfCode.Operator, "<or"), 0)

            acTypValAr.SetValue(New TypedValue(DxfCode.Operator, "<and"), 1)
            acTypValAr.SetValue(New TypedValue(DxfCode.Start, "TEXT"), 2)
            acTypValAr.SetValue(New TypedValue(DxfCode.LayerName, "0"), 3)
            acTypValAr.SetValue(New TypedValue(DxfCode.Operator, "and>"), 4)

            acTypValAr.SetValue(New TypedValue(DxfCode.Operator, "<and"), 5)
            acTypValAr.SetValue(New TypedValue(DxfCode.Start, "INSERT"), 6)
            acTypValAr.SetValue(New TypedValue(DxfCode.LayerName, "0"), 7)
            acTypValAr.SetValue(New TypedValue(DxfCode.Operator, "and>"), 8)

            acTypValAr.SetValue(New TypedValue(DxfCode.Operator, "or>"), 9)



            ' Asignar el criterio de seleccion al objeto SelectionFilter
            Dim acSelFtr As SelectionFilter = New SelectionFilter(acTypValAr)
            ' Solicitar al usuario que seleccione objetos en el área de dibujo
            Dim acSSPrompt As PromptSelectionResult
            acSSPrompt = acEd.GetSelection(acSelFtr)


            ' Si el estado de la solicitud es OK, es que se han seleccionado objetos
            If acSSPrompt.Status = PromptStatus.OK Then
                Dim acSSet As SelectionSet = acSSPrompt.Value
                'Application.ShowAlertDialog("Cantidad de objetos seleccionados: " & acSSet.Count)
                ' Recorrer el segundo conjunto de seleccion
                Dim contador As Integer = 1
                For Each acObjId As ObjectId In acSSet.GetObjectIds()

                    If acObjId.ObjectClass.DxfName.ToString = "TEXT" Then
                        Dim acEnt As DBText = acTrans.GetObject(acObjId, OpenMode.ForWrite)
                        Dim posi_z As Point3d = acEnt.Position

                        If acEnt.TextString = "-" Then

                            dt_lista_nivel.Rows.Add(posi_z.Y / 100, "aux" & contador)
                            contador += 1
                        Else
                            dt_lista_nivel.Rows.Add(posi_z.Y / 100, acEnt.TextString)
                        End If



                    End If

                    If acObjId.ObjectClass.DxfName.ToString = "INSERT" Then

                        Dim blkRef As BlockReference = DirectCast(acTrans.GetObject(acObjId, OpenMode.ForWrite), BlockReference)
                        Dim btr As BlockTableRecord = DirectCast(acTrans.GetObject(blkRef.BlockTableRecord, OpenMode.ForWrite), BlockTableRecord)
                        btr.Dispose()

                        Dim attCol As AttributeCollection = blkRef.AttributeCollection


                        For Each attId As ObjectId In attCol
                            Dim attRef As AttributeReference = DirectCast(acTrans.GetObject(attId, OpenMode.ForWrite), AttributeReference)
                            If attRef.Tag = "NIVEL" Then
                                If IsNumeric(attRef.TextString) Then
                                    dt_nueva_lista.Rows.Add(0, attRef.TextString)

                                End If
                                '  ed.WriteMessage(vbLf & "dt_nueva_lista: " & attRef.TextString)
                            End If
                        Next
                    End If
                Next

                Dim rows_lista_nivel As DataRow()
                rows_lista_nivel = dt_lista_nivel.Select("nivel <> '-'", "coor_z ASC")
                Dim rows_nueva_lista As DataRow()
                rows_nueva_lista = dt_nueva_lista.Select("cero <> '-'", "coor_z_nivel ASC")

                'For j = 0 To rows_lista_nivel.Length - 1
                '    ed.WriteMessage(vbLf & "rows_lista_nivel: " & rows_lista_nivel(j)(0) & " , " & rows_lista_nivel(j)(1))
                'Next

                'For j = 0 To rows_nueva_lista.Length - 1
                '    ed.WriteMessage(vbLf & "rows_nueva_lista: " & rows_nueva_lista(j)(0) & " , " & rows_nueva_lista(j)(1))
                'Next
                Dim aux_comparar As String = ""
                ' crear lista final
                For i = 0 To rows_nueva_lista.Count - 1
                    Dim aux_ As Boolean = True
                    Dim cont As Integer = 0
                    While (cont < rows_lista_nivel.Length And aux_ = True)
                        If rows_nueva_lista(i)(1) < rows_lista_nivel(cont)(0) Then
                            aux_ = False

                            If aux_comparar <> rows_lista_nivel(cont)(1) Then
                                'ed.WriteMessage(vbLf & "Block: " & Replace(Replace(rows_lista_nivel(cont)(1), "%%d", "°"), "%%D", "°") & " , " & rows_nueva_lista(i)(1))
                                dt_lista_final.Rows.Add(Replace(Replace(rows_lista_nivel(cont)(1), "%%d", "°"), "%%D", "°"), rows_nueva_lista(i)(1))
                                If rows_lista_nivel.Length - 1 = cont Then ' se llego al final, se agrega una final
                                    dt_lista_final.Rows.Add("Coron", rows_nueva_lista(i + 1)(1))

                                End If
                            End If
                            aux_comparar = rows_lista_nivel(cont)(1)
                        End If
                        cont = cont + 1
                    End While
                Next

                '4 generar .txt
                Dim dir As String = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\nivel.txt"
                ' Application.ShowAlertDialog(dir)
                Dim strStreamW As Stream = Nothing
                If System.IO.File.Exists(dir) = True Then File.Delete(dir)

                Dim aux As String = Path.GetTempPath()
                strStreamW = File.Create(dir)
                strStreamW.Close()

                Dim sw As New System.IO.StreamWriter(dir)
                Dim texto As String = ""
                For i = 0 To dt_lista_final.Rows.Count - 1
                    texto = dt_lista_final(i)(0) & "," & dt_lista_final(i)(1)
                    sw.WriteLine(texto)
                Next
                sw.Close()
                ' Dim utiles_aux As New utiles
                '5 crear tabla
                dt_lista_final2 = crear_tabla_niveles(dir)

                For j = 0 To dt_lista_final2.Rows.Count - 1
                    ed.WriteMessage(vbLf & "rows_nueva_lista: " & dt_lista_final2(j)(0) & " , " & dt_lista_final2(j)(1))
                Next

            Else
                Application.ShowAlertDialog("Cantidad de objetos seleccionados: 0")

            End If
            acTrans.Commit()
        End Using

        ' Libera recursos y vuelve a mostrar el cuadro de dialogo

    End Sub

    <CommandMethod("AddLine")> _
    Public Sub AddLine()
        ' Obtener el documento y la base de datos actuales
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
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

            ' Crear una linea que comienza en (5,5,0) y termina en (12,3,0)
            Dim acLine As Line = New Line()
            acLine.StartPoint = New Point3d(5, 5, 0)


            acLine.SetDatabaseDefaults()

            ' Añadir el nuevo objeto al registro de la tabla para bloques
            ' y a la transaccion
            acBlkTblRec.AppendEntity(acLine)
            acTrans.AddNewlyCreatedDBObject(acLine, True)

            ' Guardar el nuevo objeto
            acTrans.Commit()
        End Using
    End Sub
    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    'prueba
    <CommandMethod("SendACommandToAutoCAD")> _
    Public Sub SendACommandToAutoCAD()
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument

        ' Dibujar un circulo y hacer Zoom a al extension o limites del dibujo
        acDoc.SendStringToExecute("._circle 2,2,0 4 ", True, False, False)
        acDoc.SendStringToExecute("._zoom _all ", True, False, False)
    End Sub


    <CommandMethod("nhselQW")> _
    Public Sub nhselQW()
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument

        ' Dibujar un circulo y hacer Zoom a al extension o limites del dibujo
        acDoc.SendStringToExecute("._circle 2,2,0 4 ", True, False, False)
        acDoc.SendStringToExecute("._zoom _all ", True, False, False)
    End Sub


    <CommandMethod("ListLayerStates")> _
    Public Sub ListLayerStates()
        ' Obtener el documento y la base de datos actuales
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database

        ' Iniciar una transaccion
        Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()

            Dim acLyrStMan As LayerStateManager
            acLyrStMan = acCurDb.LayerStateManager

            Dim acDbDict As DBDictionary
            acDbDict = acTrans.GetObject(acLyrStMan.LayerStatesDictionaryId(True), _
                                         OpenMode.ForRead)

            Dim sLayerStateNames As String = ""

            For Each acDbDictEnt As DBDictionaryEntry In acDbDict
                sLayerStateNames = sLayerStateNames & vbLf & acDbDictEnt.Key
            Next

            Application.ShowAlertDialog("Los parametros de capa guardados son:" & _
                                        sLayerStateNames)

        End Using
    End Sub



    <CommandMethod("GetKeywordFromUser")> _
    Public Sub GetKeywordFromUser()
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument

        Dim pKeyOpts As PromptKeywordOptions = New PromptKeywordOptions("")
        pKeyOpts.Message = ControlChars.Lf & "Introduzca una opción "
        pKeyOpts.Keywords.Add("linea")
        pKeyOpts.Keywords.Add("circulo")
        pKeyOpts.Keywords.Add("arco")
        pKeyOpts.AllowNone = False

        Dim pKeyRes As PromptResult = acDoc.Editor.GetKeywords(pKeyOpts)

        Application.ShowAlertDialog("Palabra clave introducida: " & _
                                    pKeyRes.StringResult)
    End Sub

    <CommandMethod("MoveObject")> _
    Public Sub MoveObject()
        ' Obtener el documento y la base de datos actuales
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database

        ' Iniciar una transaccion
        Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()

            ' Abrir la tabla de bloques en modo lectura
            Dim acBlkTbl As BlockTable
            acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, _
                                         OpenMode.ForRead)

            ' Abrir el registro del bloque de Espacio Modelo en modo escritura
            Dim acBlkTblRec As BlockTableRecord
            acBlkTblRec = acTrans.GetObject(acBlkTbl(BlockTableRecord.ModelSpace), _
                                            OpenMode.ForWrite)

            ' Crear un circulo en 2,2 con radio 0.5
            Dim acCirc As Circle = New Circle()
            acCirc.SetDatabaseDefaults()
            acCirc.Center = New Point3d(2, 2, 0)
            acCirc.Radius = 0.5

            ' Crear una matriz y mover el circulo usando un vector de (0,0,0) a (2,0,0)
            Dim acPt3d As Point3d = New Point3d(0, 0, 0)
            Dim acVec3d As Vector3d = acPt3d.GetVectorTo(New Point3d(2, 0, 0))

            acCirc.TransformBy(Matrix3d.Displacement(acVec3d))

            ' Añadir el nuevo objeto al registro de la tabla para bloques y a la
            ' transaccion
            acBlkTblRec.AppendEntity(acCirc)
            acTrans.AddNewlyCreatedDBObject(acCirc, True)

            ' Guardar el nuevo objeto
            acTrans.Commit()
        End Using
    End Sub



    ' CREAR MULTILINEAS
    <CommandMethod("AddMline")> _
    Public Shared Sub AddMline()


        Dim doc As Document = Application.DocumentManager.MdiActiveDocument

        Dim db As Database = doc.Database

        Dim ed As Editor = doc.Editor



        Using Tx As Transaction = db.TransactionManager.StartTransaction()


            'get the mline style

            Dim line As New Mline()

            'get the current mline style

            line.Style = db.CmlstyleID

            line.Normal = Vector3d.ZAxis



            line.AppendSegment(New Point3d(0, 0, 0))

            line.AppendSegment(New Point3d(10, 10, 0))

            line.AppendSegment(New Point3d(20, 10, 0))



            'open modelpace


            Dim ModelSpaceId As ObjectId = SymbolUtilityServices.GetBlockModelSpaceId(db)




            Dim model As BlockTableRecord = TryCast(Tx.GetObject(ModelSpaceId, OpenMode.ForWrite), BlockTableRecord)

            model.AppendEntity(line)

            Tx.AddNewlyCreatedDBObject(line, True)


            Tx.Commit()
        End Using

    End Sub

    'crea estilo de multilinea
    <CommandMethod("createmlinestyle")> _
    Public Sub createmlinestyle()
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


                Color = Autodesk.AutoCAD.Colors.Color.FromRgb(255, 255, 255)

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
            End If



            Tx.Commit()
        End Using

    End Sub

    '=======================================================
    'Service provided by Telerik (www.telerik.com)
    'Conversion powered by NRefactory.
    'Twitter: @telerik
    'Facebook: facebook.com/telerik
    '=======================================================


    <CommandMethod("AddBlockRef2")> _
    Public Sub AddBlockRef2()
        ' Obtener el documento y la base de datos activos
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database

        ' Obyener el nombre del bloque
        Dim acPr As PromptResult
        acPr = acDoc.Editor.GetString(ControlChars.Lf & "Indique el nombre del bloque: ")

        If acPr.Status <> PromptStatus.OK Then
            Return
        End If

        ' Iniciar una transaccion
        Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()

            ' Abrir la tabla para bloques en modo lectura
            Dim acBlkTbl As BlockTable
            acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead)


            ' Comprobar si el bloque existe
            If acBlkTbl.Has(acPr.StringResult) Then
                ' Abrir el registro del bloque de Espacio Modelo para escritura
                Dim acBlkTblRec As BlockTableRecord
                acBlkTblRec = acTrans.GetObject(acBlkTbl(BlockTableRecord.ModelSpace), _
                                                OpenMode.ForWrite)

                ' Obtener una referencia del bloque
                Dim acBlk As BlockTableRecord = acTrans.GetObject( _
                acBlkTbl.Item(acPr.StringResult), OpenMode.ForRead, False)

                ' Crear una referencia a bloque
                Dim acBlkRef As New BlockReference(New Point3d(50, 50, 0), acBlk.ObjectId)

                ' Rotar 45º
                ' 45 *(Math.PI/180)
                acBlkRef.Rotation = 0.785
                ' Factor de escala
                acBlkRef.ScaleFactors = New Scale3d(50)

                ' Añadir el nuevo objeto al registro de la tabla para bloques
                ' y a la transaccion
                acBlkTblRec.AppendEntity(acBlkRef)
                acTrans.AddNewlyCreatedDBObject(acBlkRef, True)

                ' Si el bloque contiene atributos, añadirlos a la referencia a bloque
                If acBlk.HasAttributeDefinitions Then
                    For Each acObjId As ObjectId In acBlk
                        Dim acObj As DBObject = acTrans.GetObject(acObjId, OpenMode.ForRead, False)
                        Dim acAttDef As AttributeDefinition = TryCast(acObj, AttributeDefinition)

                        If acAttDef IsNot Nothing Then
                            If Not acAttDef.Constant Then
                                ' Crear una referencia de atributo
                                Dim acAttRef As AttributeReference = New AttributeReference

                                acAttRef.SetAttributeFromBlock(acAttDef, acBlkRef.BlockTransform)

                                ' Esto solo para AutoCAD 2008 o superior
                                ' Si el valor es texto de mutiples lineas
                                If acAttDef.IsMTextAttributeDefinition Then
                                    acAttRef.UpdateMTextAttribute()
                                End If

                                acAttRef.TextString = acAttDef.TextString

                                ' Agregar la referencia de atributo a la 
                                ' referencia a bloque y a la transaccion
                                acBlkRef.AttributeCollection.AppendAttribute(acAttRef)
                                acTrans.AddNewlyCreatedDBObject(acAttRef, True)

                            End If
                        End If
                    Next
                End If

                ' Confirmar los cambios
                acTrans.Commit()
            End If

        End Using
    End Sub



    <CommandMethod("AddBlockRef1")> _
    Public Sub AddBlockRef1()
        ' Obtener el documento y la base de datos activos
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database

        ' Obyener el nombre del bloque
        Dim acPr As PromptResult
        acPr = acDoc.Editor.GetString(ControlChars.Lf & "Indique el nombre del bloque: ")

        If acPr.Status <> PromptStatus.OK Then
            Return
        End If

        ' Iniciar una transaccion
        Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()

            ' Abrir la tabla para bloques en modo lectura
            Dim acBlkTbl As BlockTable
            acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead)


            ' Comprobar si el bloque existe
            If acBlkTbl.Has(acPr.StringResult) Then
                ' Abrir el registro del bloque de Espacio Modelo para escritura
                Dim acBlkTblRec As BlockTableRecord
                acBlkTblRec = acTrans.GetObject(acBlkTbl(BlockTableRecord.ModelSpace), _
                                                OpenMode.ForWrite)

                ' Obtener una referencia del bloque
                Dim acBlk As BlockTableRecord = acTrans.GetObject( _
                acBlkTbl.Item(acPr.StringResult), OpenMode.ForRead, False)

                ' Crear una referencia a bloque
                Dim acBlkRef As New BlockReference(New Point3d(50, 50, 0), acBlk.ObjectId)

                ' Rotar 45º
                ' 45 *(Math.PI/180)
                acBlkRef.Rotation = 0.785
                ' Factor de escala
                acBlkRef.ScaleFactors = New Scale3d(50)

                ' Añadir el nuevo objeto al registro de la tabla para bloques
                ' y a la transaccion
                acBlkTblRec.AppendEntity(acBlkRef)
                acTrans.AddNewlyCreatedDBObject(acBlkRef, True)

                ' Confirmar los cambios
                acTrans.Commit()
            End If

        End Using
    End Sub


    Function crear_tabla_niveles(ByVal ruta As String) As System.Data.DataTable


        Dim tabla As New System.Data.DataTable
        tabla.Columns.Add("nivel", GetType(String))
        tabla.Columns.Add("coor_z", GetType(Double))
        ' Leer el contenido y asignar cada línea a los controles
        ' Comprobar que existe

        ' Borrar el contenido previo de los controles
        ' Leer el fichero usando la codificación de Windows
        ' pero pudiendo usar la marca de tipo de fichero (BOM)

        If System.IO.File.Exists(ruta) = False Then
            GoTo ir1
        End If

        Dim sr As New System.IO.StreamReader(ruta, System.Text.Encoding.Default, True)

        ' Leer el contenido mientras no se llegue al final

        While sr.Peek() <> -1


            ' Leer una líena del fichero
            Dim s As String = sr.ReadLine()

            If String.IsNullOrEmpty(s) Then
                Continue While
            End If

            Dim split As String() = s.Split(New [Char]() {","c, CChar(vbTab)})


            tabla.Rows.Add(Replace(split(0), "Â", ""), split(1))
            ' CODIGO PARA DIVIDIR LOS ELEMENTOS VERTICALES DE LAS ELEVACIONES
            ' DE MODOD QUE QUEDENE ELEMENTOS MAS PEQUEÑOS, DE LARGO MAXIMO  DE UN PISO
            ' PERO NO ES CONSIDERA POR ESO SE CONSIDERA   "CSng(split(0)) = CSng(split(1))"
            ' PARA SER CONSIDERA DEBE SER "CSng(split(0)) = CSng(split(2))" , ESTO  x1=X2




        End While


        sr.Close()

        Return tabla
ir1:
    End Function
End Class
