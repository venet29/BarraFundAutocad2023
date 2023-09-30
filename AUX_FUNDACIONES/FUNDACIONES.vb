Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.ApplicationServices
Imports System.Math
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.EditorInput
'Imports Autodesk.AutoCAD.GraphicsInterface

Imports VARIOS






Public Class FUNDACIONES

    Private punto_ini As Point2d
    Private punto_fin As Point2d
    Private punto_dim1 As Point2d
    Private punto_dim2 As Point2d
    Private punto_angle As Single = 0
    Private planta_desfase0_2 As Single = 0
    Private planta_desfase0 As Single = 0
    Private planta_desfase1 As Single = 0
    Private planta_desfase2 As Single = 0
    Private planta_aux1 As Single = 0
    Private planta_aux2 As Single = 0
    Private punto_e As Single = 0
    Private punto_e_real As Single = 0
    Private largo_min_losa As Single = 0
    Private largo_barra_fund As Single = 0
    Private largo_barra_parcial As String = 0
    Private espesor_muro1 As Single = 0
    Private espesor_muro2 As Single = 0
    Private tipo_losa, tipo_losa2, tipo_orientacion As String
    Private diamtro_ As Integer
    Private ref_obj As Object
    Private ref_obj2 As Object = Nothing
    Private punto_cua_e As String
    Private punto_uso As Point2d
    Public Property punto_uso_() As Point2d
        Get
            Return punto_uso
        End Get
        Set(ByVal Value As Point2d)
            punto_uso = Value
        End Set
    End Property
    Public Property orientacion_() As String
        Get
            Return tipo_orientacion
        End Get
        Set(ByVal Value As String)
            tipo_orientacion = Value
        End Set
    End Property

    Public Property tipo() As String
        Get
            Return tipo_losa
        End Get
        Set(ByVal Value As String)
            tipo_losa = Value
        End Set
    End Property
    Public Property tipo2() As String
        Get
            Return tipo_losa2
        End Get
        Set(ByVal Value As String)
            tipo_losa2 = Value
        End Set
    End Property
    Public Property diamet() As Integer
        Get
            Return diamtro_
        End Get
        Set(ByVal Value As Integer)
            diamtro_ = Value
        End Set
    End Property
    Public Property largo_barra_real_fun() As Single
        Get
            Return largo_barra_fund
        End Get
        Set(ByVal Value As Single)
            largo_barra_fund = Value
        End Set
    End Property
    Public Property largo_barra_parcial_fun() As String
        Get
            Return largo_barra_parcial
        End Get
        Set(ByVal Value As String)
            largo_barra_parcial = Value
        End Set
    End Property
    Public Property punto_inicial() As Point2d
        Get
            Return punto_ini
        End Get
        Set(ByVal Value As Point2d)
            punto_ini = Value
        End Set
    End Property
    Public Property e_muro1() As Single
        Get
            Return espesor_muro1
        End Get
        Set(ByVal Value As Single)
            espesor_muro1 = Value
        End Set
    End Property
    Public Property e_muro2() As Single
        Get
            Return espesor_muro2
        End Get
        Set(ByVal Value As Single)
            espesor_muro2 = Value
        End Set
    End Property
    Public Property largo_minimo_losa() As Single
        Get
            Return largo_min_losa
        End Get
        Set(ByVal Value As Single)
            largo_min_losa = Value
        End Set
    End Property
    Public Property object_poly() As Object
        Get
            Return ref_obj
        End Get
        Set(ByVal Value As Object)
            ref_obj = Value
        End Set
    End Property
    Public Property object_poly2() As Object
        Get
            Return ref_obj2
        End Get
        Set(ByVal Value As Object)
            ref_obj2 = Value
        End Set
    End Property
    Public Property punto_e_losa() As Single
        Get
            Return punto_e
        End Get
        Set(ByVal Value As Single)
            punto_e = Value
        End Set
    End Property
    Public Property punto_e_losa_real() As Single
        Get
            Return punto_e_real
        End Get
        Set(ByVal Value As Single)
            punto_e_real = Value
        End Set
    End Property


    Public Property punto_cua_losa() As String
        Get
            Return punto_cua_e
        End Get
        Set(ByVal Value As String)
            punto_cua_e = Value
        End Set
    End Property
    Public Property punto_final() As Point2d
        Get
            Return punto_fin
        End Get
        Set(ByVal Value As Point2d)
            punto_fin = Value
        End Set
    End Property
    Public Property punto_dimension1() As Point2d
        Get
            Return punto_dim1
        End Get
        Set(ByVal Value As Point2d)
            punto_dim1 = Value
        End Set
    End Property
    Public Property punto_dimension2() As Point2d
        Get
            Return punto_dim2
        End Get
        Set(ByVal Value As Point2d)
            punto_dim2 = Value
        End Set
    End Property
    Public Property punto_angulo() As Single
        Get
            Return punto_angle
        End Get
        Set(ByVal Value As Single)
            punto_angle = Value
        End Set
    End Property
    Public Property punto_DESFASE0_2() As Single
        Get
            Return planta_desfase0_2
        End Get
        Set(ByVal Value As Single)
            planta_desfase0_2 = Value
        End Set
    End Property
    Public Property punto_DESFASE0() As Single
        Get
            Return planta_desfase0
        End Get
        Set(ByVal Value As Single)
            planta_desfase0 = Value
        End Set
    End Property
    Public Property punto_DESFASE1() As Single
        Get
            Return planta_desfase1
        End Get
        Set(ByVal Value As Single)
            planta_desfase1 = Value
        End Set
    End Property
    Public Property punto_DESFASE2() As Single
        Get
            Return planta_desfase2
        End Get
        Set(ByVal Value As Single)
            planta_desfase2 = Value
        End Set
    End Property
    Public Property punto_aux1() As Single
        Get
            Return planta_aux1
        End Get
        Set(ByVal Value As Single)
            planta_aux1 = Value
        End Set
    End Property
    Public Property punto_aux2() As Single
        Get
            Return planta_aux2
        End Get
        Set(ByVal Value As Single)
            planta_aux2 = Value
        End Set
    End Property

    ' dibuja barra losa inferii
    Public Sub modificar_barra_FUNDA(ByRef ents As ObjectIdCollection, ByRef ent As Entity, ByVal pt_ini As Point2d, ByVal pt_fin As Point2d, ByVal casos As String, ByRef direcci As String, ByRef diamtro_pl As Single, ByRef e1 As Single)
        Dim Doc As Document = Application.DocumentManager.MdiActiveDocument
        Dim DB As Database = Doc.Database
        Dim Ed As Editor = Doc.Editor


        Using acTrans As Transaction = DB.TransactionManager.StartTransaction()


            Dim acPolyObj As ObjectId = Nothing
            Dim acEnt_barra_aux As Polyline = TryCast(acTrans.GetObject(ent.ObjectId, OpenMode.ForWrite), Polyline)

            'acEnt_barra_aux.SetPointAt(0, New Point2d(pt_ini.X, pt_ini.Y))
            'acEnt_barra_aux.SetPointAt(1, New Point2d(pt_fin.X, pt_fin.Y))

            Dim pts1 As New Point3d
            Dim pts2 As New Point3d
            Dim pts1_v As New Point2d
            Dim pts2_v As New Point2d
            Dim coordenada_PTO_(1) As Point3d
            ' Dim e1 As Single = 13
            Dim acPoly As Polyline = New Polyline()
            coordenada_PTO_ = coordenada_modificar_fun(Nothing, New Point3d(pt_ini.X, pt_ini.Y, 0), New Point3d(pt_fin.X, pt_fin.Y, 0))    ' OBTENER P1 Y P2 ORDENADOS
            pts1 = coordenada_PTO_(0)
            pts2 = coordenada_PTO_(1)
            Dim ANGLE As Double = coordenada__angulo_p1_p2_fun(pts1, pts2, Ed, Nothing)
            Dim desplaza As Single = 2

            coordenada_PTO_ = coordenada_modificar_fun(Nothing, New Point3d(pt_ini.X, pt_ini.Y, 0), New Point3d(pt_fin.X, pt_fin.Y, 0))

            pts2_v = New Point2d(coordenada_PTO_(1).X, coordenada_PTO_(1).Y)
            pts1_v = New Point2d(coordenada_PTO_(0).X, coordenada_PTO_(0).Y)
            Dim p0, p1, p2, p3, p4, p5, p6 As New Point2d()
            Dim delta0, delta0_2, delta1, delta2 As Single

            punto_ini = pts1_v
            punto_fin = pts2_v
            punto_angle = ANGLE

            If casos = "f3" Then
                p0 = New Point2d(pts1_v.X, pts1_v.Y)
                p1 = New Point2d(pts2_v.X, pts2_v.Y)
                delta1 = aproximar(p0.GetDistanceTo(p1), 1, diamtro_pl)

                acEnt_barra_aux.SetPointAt(0, New Point2d(pts1_v.X - Cos(ANGLE) * delta1 / 2, pts1_v.Y - Sin(ANGLE) * delta1 / 2))
                acEnt_barra_aux.SetPointAt(1, New Point2d(pts2_v.X + Cos(ANGLE) * delta1 / 2, pts2_v.Y + Sin(ANGLE) * delta1 / 2))
                'If direcci = "horizontal_i" Or direcci = "vertical_b" Then


                largo_barra_fund = Round(acEnt_barra_aux.GetPoint3dAt(0).DistanceTo(acEnt_barra_aux.GetPoint3dAt(1)), 0)
                largo_barra_parcial = ""
                'ElseIf direcci = "horizontal_d" Or direcci = "vertical_a" Then
                punto_uso_ = acEnt_barra_aux.GetPoint2dAt(1)
                '    acEnt_barra_aux.SetPointAt(0, New Point2d(pts2_v.X, pts2_v.Y))
                '    acEnt_barra_aux.SetPointAt(1, New Point2d(pts1_v.X, pts1_v.Y))
                'End If




            ElseIf casos = "f9a" Then

                p0 = New Point2d(pts1_v.X + Sin(ANGLE) * e1, pts1_v.Y - Cos(ANGLE) * e1)
                p1 = New Point2d(pts1_v.X, pts1_v.Y)
                p2 = New Point2d(pts2_v.X, pts2_v.Y)
                p3 = New Point2d(pts2_v.X + Sin(ANGLE) * e1, pts2_v.Y - Cos(ANGLE) * e1)

                Dim delta_aux2 As Single = 0
                Dim delta_aux1 As Single = 0

                delta0 = aproximar2_fund(p1.GetDistanceTo(p2), 1)
                delta1 = aproximar(p0.GetDistanceTo(p1) + p1.GetDistanceTo(p2) + p2.GetDistanceTo(p3) + delta0, 1, diamtro_pl)


                If (delta1 Mod 2) <> 0 Then
                    delta_aux1 = delta1 / 2 - 0.5
                    delta_aux2 = delta1 / 2 + 0.5
                Else
                    delta_aux1 = delta1 / 2
                    delta_aux2 = delta1 / 2
                End If

                delta1 = 0

                acEnt_barra_aux.SetPointAt(0, New Point2d(pts1_v.X - Cos(ANGLE) * (delta0) + Sin(ANGLE) * (e1 + delta_aux1), pts1_v.Y - Sin(ANGLE) * (delta0) - Cos(ANGLE) * (e1 + delta_aux1)))
                acEnt_barra_aux.SetPointAt(1, New Point2d(pts1_v.X - Cos(ANGLE) * delta0, pts1_v.Y - Sin(ANGLE) * delta0))
                acEnt_barra_aux.AddVertexAt(2, New Point2d(pts2_v.X, pts2_v.Y), 0, 0, 0)
                acEnt_barra_aux.AddVertexAt(3, New Point2d(pts2_v.X + Sin(ANGLE) * (e1 + delta_aux2), pts2_v.Y - Cos(ANGLE) * (e1 + delta_aux2)), 0, 0, 0)


                largo_barra_fund = Round(acEnt_barra_aux.GetPoint3dAt(0).DistanceTo(acEnt_barra_aux.GetPoint3dAt(1)) + acEnt_barra_aux.GetPoint3dAt(1).DistanceTo(acEnt_barra_aux.GetPoint3dAt(2)) + acEnt_barra_aux.GetPoint3dAt(2).DistanceTo(acEnt_barra_aux.GetPoint3dAt(3)), 0)
                largo_barra_parcial = "(" & Round(acEnt_barra_aux.GetPoint3dAt(0).DistanceTo(acEnt_barra_aux.GetPoint3dAt(1)), 0) & "+" & Round(acEnt_barra_aux.GetPoint3dAt(1).DistanceTo(acEnt_barra_aux.GetPoint3dAt(2)), 0) & "+" & Round(acEnt_barra_aux.GetPoint3dAt(2).DistanceTo(acEnt_barra_aux.GetPoint3dAt(3)), 0) & ")"
                punto_aux1 = delta_aux1
                punto_aux2 = delta_aux2

            ElseIf casos = "f9a_V" Then

                If direcci = "horizontal_i" Or direcci = "vertical_b" Then

                    p0 = New Point2d(pts1_v.X + Sin(ANGLE) * e1, pts1_v.Y - Cos(ANGLE) * e1)
                    p1 = New Point2d(pts1_v.X, pts1_v.Y)
                    p2 = New Point2d(pts2_v.X, pts2_v.Y)

                    delta0 = aproximar2_fund(p1.GetDistanceTo(p2), 1)
                    delta1 = aproximar(p0.GetDistanceTo(p1) + p1.GetDistanceTo(p2) + delta0, 1, diamtro_pl)

                    acEnt_barra_aux.SetPointAt(0, New Point2d(pts1_v.X - Cos(ANGLE) * delta0 + Sin(ANGLE) * (e1 + delta1), pts1_v.Y - Sin(ANGLE) * delta0 - Cos(ANGLE) * (e1 + delta1)))
                    acEnt_barra_aux.SetPointAt(1, New Point2d(pts1_v.X - Cos(ANGLE) * delta0, pts1_v.Y - Sin(ANGLE) * delta0))
                    acEnt_barra_aux.AddVertexAt(2, New Point2d(pts2_v.X, pts2_v.Y), 0, 0, 0)

                    punto_uso_ = acEnt_barra_aux.GetPoint2dAt(2)

                    largo_barra_parcial = "(" & Round(acEnt_barra_aux.GetPoint3dAt(0).DistanceTo(acEnt_barra_aux.GetPoint3dAt(1)), 0) & "+" & Round(acEnt_barra_aux.GetPoint3dAt(1).DistanceTo(acEnt_barra_aux.GetPoint3dAt(2)), 0) & ")"
                    punto_aux1 = 0
                    punto_aux2 = 0

                ElseIf direcci = "horizontal_d" Or direcci = "vertical_a" Then

                    p0 = New Point2d(pts1_v.X, pts1_v.Y)
                    p1 = New Point2d(pts2_v.X, pts2_v.Y)
                    p2 = New Point2d(pts2_v.X + Sin(ANGLE) * e1, pts2_v.Y - Cos(ANGLE) * e1)

                    delta0 = aproximar2_fund(p0.GetDistanceTo(p1), 1)
                    delta1 = aproximar(p0.GetDistanceTo(p1) + p1.GetDistanceTo(p2) + delta0, 1, diamtro_pl)


                    acEnt_barra_aux.SetPointAt(0, New Point2d(pts1_v.X, pts1_v.Y))
                    acEnt_barra_aux.SetPointAt(1, New Point2d(pts2_v.X + Cos(ANGLE) * delta0, pts2_v.Y + Sin(ANGLE) * delta0))
                    acEnt_barra_aux.AddVertexAt(2, New Point2d(pts2_v.X + Cos(ANGLE) * delta0 + Sin(ANGLE) * (e1 + delta1), pts2_v.Y + Sin(ANGLE) * delta0 - Cos(ANGLE) * (e1 + delta1)), 0, 0, 0)
                    punto_uso_ = acEnt_barra_aux.GetPoint2dAt(1)
                    largo_barra_parcial = "(" & Round(acEnt_barra_aux.GetPoint3dAt(0).DistanceTo(acEnt_barra_aux.GetPoint3dAt(1)), 0) & "+" & Round(acEnt_barra_aux.GetPoint3dAt(1).DistanceTo(acEnt_barra_aux.GetPoint3dAt(2)), 0) & ")"
                End If
                largo_barra_fund = Round(acEnt_barra_aux.GetPoint3dAt(0).DistanceTo(acEnt_barra_aux.GetPoint3dAt(1)) + acEnt_barra_aux.GetPoint3dAt(1).DistanceTo(acEnt_barra_aux.GetPoint3dAt(2)), 0)


            ElseIf casos = "f10" Then

                p0 = New Point2d(pts2_v.X - Sin(ANGLE) * e1, pts2_v.Y + Cos(ANGLE) * e1)
                p1 = New Point2d(pts2_v.X, pts2_v.Y)
                p2 = New Point2d(pts1_v.X, pts1_v.Y)
                p3 = New Point2d(pts1_v.X - Sin(ANGLE) * e1, pts1_v.Y + Cos(ANGLE) * e1)

                Dim delta_aux2 As Single = 0
                Dim delta_aux1 As Single = 0
                delta0 = aproximar2_fund(p1.GetDistanceTo(p2), 1)
                delta1 = aproximar(p0.GetDistanceTo(p1) + p1.GetDistanceTo(p2) + p2.GetDistanceTo(p3) + delta0, 1, diamtro_pl)

                If (delta1 Mod 2) <> 0 Then
                    delta_aux1 = delta1 / 2 - 0.5
                    delta_aux2 = delta1 / 2 + 0.5
                Else
                    delta_aux1 = delta1 / 2
                    delta_aux2 = delta1 / 2
                End If
                delta1 = 0

                acEnt_barra_aux.SetPointAt(0, New Point2d(pts1_v.X - Cos(ANGLE) * (delta0) - Sin(ANGLE) * (e1 + delta_aux1), pts1_v.Y - Sin(ANGLE) * (delta0) + Cos(ANGLE) * (e1 + delta_aux1)))
                acEnt_barra_aux.SetPointAt(1, New Point2d(pts1_v.X - Cos(ANGLE) * delta0, pts1_v.Y - Sin(ANGLE) * delta0))
                acEnt_barra_aux.AddVertexAt(2, New Point2d(pts2_v.X, pts2_v.Y), 0, 0, 0)
                acEnt_barra_aux.AddVertexAt(3, New Point2d(pts2_v.X - Sin(ANGLE) * (e1 + delta_aux2), pts2_v.Y + Cos(ANGLE) * (e1 + delta_aux2)), 0, 0, 0)



                'acEnt_barra_aux.SetPointAt(0, New Point2d(pts2_v.X - Cos(ANGLE) * (delta0 + delta1 / 2) - Sin(ANGLE) * e1, pts2_v.Y - Sin(ANGLE) * (delta0 + delta1 / 2) + Cos(ANGLE) * e1))
                'acEnt_barra_aux.SetPointAt(1, New Point2d(pts2_v.X - Cos(ANGLE) * (delta0 + delta1 / 2), pts2_v.Y - Sin(ANGLE) * (delta0 + delta1 / 2)))
                'acEnt_barra_aux.AddVertexAt(2, New Point2d(pts1_v.X + Cos(ANGLE) * delta1 / 2, pts1_v.Y + Sin(ANGLE) * delta1 / 2), 0, 0, 0)
                'acEnt_barra_aux.AddVertexAt(3, New Point2d(pts1_v.X + Cos(ANGLE) * delta1 / 2, pts1_v.Y + Sin(ANGLE) * delta1 / 2 + Cos(ANGLE) * e1), 0, 0, 0)


                largo_barra_fund = Round(acEnt_barra_aux.GetPoint3dAt(0).DistanceTo(acEnt_barra_aux.GetPoint3dAt(1)) + acEnt_barra_aux.GetPoint3dAt(1).DistanceTo(acEnt_barra_aux.GetPoint3dAt(2)) + acEnt_barra_aux.GetPoint3dAt(2).DistanceTo(acEnt_barra_aux.GetPoint3dAt(3)), 0)
                largo_barra_parcial = "(" & Round(acEnt_barra_aux.GetPoint3dAt(0).DistanceTo(acEnt_barra_aux.GetPoint3dAt(1)), 0) & "+" & Round(acEnt_barra_aux.GetPoint3dAt(1).DistanceTo(acEnt_barra_aux.GetPoint3dAt(2)), 0) & "+" & Round(acEnt_barra_aux.GetPoint3dAt(2).DistanceTo(acEnt_barra_aux.GetPoint3dAt(3)), 0) & ")"
                punto_aux1 = delta_aux1
                punto_aux2 = delta_aux2

            ElseIf casos = "f11" Then




                If direcci = "horizontal_i" Or direcci = "vertical_b" Then

                    p0 = New Point2d(pts1_v.X - Sin(ANGLE) * e1, pts1_v.Y + Cos(ANGLE) * e1)
                    p1 = New Point2d(pts1_v.X, pts1_v.Y)
                    p2 = New Point2d(pts2_v.X, pts2_v.Y)

                    delta0 = aproximar2_fund(p1.GetDistanceTo(p2), 1)
                    delta1 = aproximar(p0.GetDistanceTo(p1) + p1.GetDistanceTo(p2) + delta0, 1, diamtro_pl)

                    acEnt_barra_aux.SetPointAt(0, New Point2d(pts1_v.X - Cos(ANGLE) * delta0 - Sin(ANGLE) * (e1 + delta1), pts1_v.Y - Sin(ANGLE) * delta0 + Cos(ANGLE) * (e1 + delta1)))
                    acEnt_barra_aux.SetPointAt(1, New Point2d(pts1_v.X - Cos(ANGLE) * delta0, pts1_v.Y - Sin(ANGLE) * delta0))
                    acEnt_barra_aux.AddVertexAt(2, New Point2d(pts2_v.X, pts2_v.Y), 0, 0, 0)

                    punto_uso_ = acEnt_barra_aux.GetPoint2dAt(2)

                    largo_barra_fund = Round(acEnt_barra_aux.GetPoint3dAt(0).DistanceTo(acEnt_barra_aux.GetPoint3dAt(1)) + acEnt_barra_aux.GetPoint3dAt(1).DistanceTo(acEnt_barra_aux.GetPoint3dAt(2)), 0)
                    largo_barra_parcial = "(" & Round(acEnt_barra_aux.GetPoint3dAt(0).DistanceTo(acEnt_barra_aux.GetPoint3dAt(1)), 0) & "+" & Round(acEnt_barra_aux.GetPoint3dAt(1).DistanceTo(acEnt_barra_aux.GetPoint3dAt(2)), 0) & ")"
                    punto_aux1 = 0
                    punto_aux2 = 0

                ElseIf direcci = "horizontal_d" Or direcci = "vertical_a" Then

                    p0 = New Point2d(pts1_v.X, pts1_v.Y)
                    p1 = New Point2d(pts2_v.X, pts2_v.Y)
                    p2 = New Point2d(pts2_v.X - Sin(ANGLE) * e1, pts2_v.Y + Cos(ANGLE) * e1)

                    delta0 = aproximar2_fund(p0.GetDistanceTo(p1), 1)
                    delta1 = aproximar(p0.GetDistanceTo(p1) + p1.GetDistanceTo(p2) + delta0, 1, diamtro_pl)


                    acEnt_barra_aux.SetPointAt(0, New Point2d(pts1_v.X, pts1_v.Y))
                    acEnt_barra_aux.SetPointAt(1, New Point2d(pts2_v.X + Cos(ANGLE) * delta0, pts2_v.Y + Sin(ANGLE) * delta0))
                    acEnt_barra_aux.AddVertexAt(2, New Point2d(pts2_v.X + Cos(ANGLE) * delta0 - Sin(ANGLE) * (e1 + delta1), pts2_v.Y + Sin(ANGLE) * delta0 + Cos(ANGLE) * (e1 + delta1)), 0, 0, 0)
                    punto_uso_ = acEnt_barra_aux.GetPoint2dAt(1)
                    largo_barra_fund = Round(acEnt_barra_aux.GetPoint3dAt(0).DistanceTo(acEnt_barra_aux.GetPoint3dAt(1)) + acEnt_barra_aux.GetPoint3dAt(1).DistanceTo(acEnt_barra_aux.GetPoint3dAt(2)), 0)
                    largo_barra_parcial = "(" & Round(acEnt_barra_aux.GetPoint3dAt(0).DistanceTo(acEnt_barra_aux.GetPoint3dAt(1)), 0) & "+" & Round(acEnt_barra_aux.GetPoint3dAt(1).DistanceTo(acEnt_barra_aux.GetPoint3dAt(2)), 0) & ")"
                End If

            End If

            orientacion_ = direcci
            tipo = casos
            object_poly = acEnt_barra_aux.ObjectId

            ents.Add(object_poly)

            punto_DESFASE0_2 = delta0_2
            punto_DESFASE0 = delta0
            punto_DESFASE1 = delta1
            punto_DESFASE2 = delta2


            acTrans.Commit()
        End Using


    End Sub

    Public Sub dibujar_dimesion_funda(ByRef ents As ObjectIdCollection, ByVal pt1 As Point3d, ByVal pt2 As Point3d, ByVal ANGLE As Single)

        Dim Doc As Document = Application.DocumentManager.MdiActiveDocument
        Dim DB As Database = Doc.Database
        Dim Ed As Editor = Doc.Editor
        Dim dimension As New dimensiones()

        Dim pPtRes As PromptPointResult
        Dim pPtOpts As PromptPointOptions = New PromptPointOptions("")

        If pt1 = Nothing Or pt2 = Nothing Then


            ' Solicitar el punto final
            pPtOpts.Message = String.Format("{0}3)Precise el punto Inicial de Rango: ", ControlChars.Lf)
            'pPtOpts.UseBasePoint = True

            pPtRes = Doc.Editor.GetPoint(pPtOpts)
            If pPtRes.Status = PromptStatus.Cancel Then Exit Sub
            pt1 = pPtRes.Value



            pPtOpts.Message = String.Format("{0}4)Precise el punto final de Rango: ", ControlChars.Lf)
            pPtOpts.UseBasePoint = True
            pPtOpts.BasePoint = pt1
            pPtRes = Doc.Editor.GetPoint(pPtOpts)
            If pPtRes.Status = PromptStatus.Cancel Then Exit Sub
            pt2 = pPtRes.Value
        End If

        punto_dim1 = New Point2d(pt1.X, pt1.Y)
        punto_dim2 = New Point2d(pt2.X, pt2.Y)


        'Dim pMin As Point3d
        'Dim pMax As Point3d
        'Using acView As ViewTableRecord = Doc.Editor.GetCurrentView()
        '    pMin = New Point3d(Application.GetSystemVariable("viewctr").X - (acView.Width / 2), Application.GetSystemVariable("viewctr").Y - (acView.Height / 2), 0)
        '    pMax = New Point3d((acView.Width / 2) + Application.GetSystemVariable("viewctr").X, (acView.Height / 2) + Application.GetSystemVariable("viewctr").Y, 0)
        'End Using


        'Zoom_fund(New Point3d(Min(pt1.X, pt2.X) - 200, Min(pt1.Y, pt2.Y) - 200, 0), New Point3d(Min(pt1.X, pt2.X) + 200, Min(pt1.Y, pt2.Y) + 200, 0), New Point3d(), 1)

        Using tr As Transaction = DB.TransactionManager.StartTransaction()
            dimension.DrawRotDimension(ents, DB, tr, pt1, pt2, 10, "RANGOS", ANGLE)
        End Using

        'Zoom_fund(pMin, pMax, New Point3d(), 1)

    End Sub

    Public Sub dibujar_circulo_fund(ByRef ents As ObjectIdCollection, ByVal pt_ref As Point3d, ByRef tipo As String, ByRef angulo As Single)
        Dim Doc As Document = Application.DocumentManager.MdiActiveDocument
        Dim DB As Database = Doc.Database
        Dim Ed As Editor = Doc.Editor
        Using tr As Transaction = DB.TransactionManager.StartTransaction()
            ' Abrir la tabla de bloques en modo lectura
            Dim acBlkTbl As BlockTable = tr.GetObject(DB.BlockTableId, OpenMode.ForRead)

            ' Abrir el registro del bloque de Espacio Modelo en modo escritura
            Dim acBlkTblRec As BlockTableRecord
            acBlkTblRec = tr.GetObject(acBlkTbl(BlockTableRecord.ModelSpace), OpenMode.ForWrite)

            Dim espacio As Single = 0


            ' Crear un circulo en 2,2 con radio 0.5
            Dim acCirc As Circle = New Circle()
            acCirc.SetDatabaseDefaults()
            acCirc.Center = New Point3d(pt_ref.X - Sin(angulo) * espacio, pt_ref.Y + Cos(angulo) * espacio, 0)
            acCirc.Radius = 5
            acCirc.Layer = "0"




            ' Añadir el nuevo objeto al registro de la tabla para bloques y a la
            ' transaccion
            acBlkTblRec.AppendEntity(acCirc)
            tr.AddNewlyCreatedDBObject(acCirc, True)
            ents.Add(acCirc.ObjectId)
            tr.Commit()
        End Using
    End Sub


    Public Sub dibujar_texto_bloque_fund(ByVal x2_flecha As Single, ByVal y2_flecha As Single, ByVal angle As String, ByRef ents_dt As ObjectIdCollection, ByVal AUX_LARGO As String, ByVal aux_cuatia As String, ByVal AUX_PARCIAL As String)
        ' Obtener el documento y la base de datos activos
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        Dim acEd As Editor = acDoc.Editor
        ' Obyener el nombre del bloque

        Dim acBlkRef As BlockReference = Nothing

        Dim aux_texto As Boolean = False

        'If Not Directory.Exists(cots) Then
        '    Global.System.Windows.Forms.Application.Exit()
        'End If
        Dim flecha As String = "_SCAD_CUANTIA_FUND_NH"
        ' Iniciar una transaccion




        Using acDoc.LockDocument()


            Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()

                ' Abrir la tabla para bloques en modo lectura
                Dim acBlkTbl As BlockTable = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead)

                If acBlkTbl.Has("_SCAD_CUANTIA_FUND_NH2") Then
                    flecha = "_SCAD_CUANTIA_FUND_NH2"
                Else
                    flecha = "_SCAD_CUANTIA_FUND_NH"
                End If

VOLVER:
                ' Comprobar si el bloque existe
                If acBlkTbl.Has(flecha) Then
                    ' Abrir el registro del bloque de Espacio Modelo para escritura
                    Dim acBlkTblRec As BlockTableRecord
                    acBlkTblRec = acTrans.GetObject(acBlkTbl(BlockTableRecord.ModelSpace), OpenMode.ForWrite)

                    ' Obtener una referencia del bloque
                    Dim acBlk As BlockTableRecord = acTrans.GetObject(acBlkTbl.Item(flecha), OpenMode.ForRead, False)

                    ' Crear una referencia a bloque
                    ' Dim acBlkRef As New BlockReference(New Point3d(x2_flecha, y2_flecha, 0), acBlk.ObjectId)
                    acBlkRef = New BlockReference(New Point3d(x2_flecha, y2_flecha, 0), acBlk.ObjectId)
                    ' Rotar 45º
                    ' 45 *(Math.PI/180)
                    acBlkRef.Rotation = angle

                    ' Factor de escala
                    'acBlkRef.ScaleFactors = New Scale3d(50)
                    '  acBlkRef.
                    ' Añadir el nuevo objeto al registro de la tabla para bloques
                    ' y a la transaccion

                    Dim objId As ObjectId = acBlkTblRec.AppendEntity(acBlkRef)
                    ents_dt.Add(objId)

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


                                    Dim suma_plus As Integer = 0
                                    Dim suma_largo As Integer = 0
                                    If acAttRef.Tag = "CUANTIA" Then
                                        acAttRef.TextString = aux_cuatia
                                        Dim POTI3 As Point2d = New Point2d(acAttRef.AlignmentPoint.X, acAttRef.AlignmentPoint.Y)

                                        If InStr(1, LCase(aux_cuatia), "f") <> "0" Then suma_plus = 5
                                        If InStr(1, aux_cuatia, "+") <> "0" Then suma_plus = 8
                                        If Len(AUX_LARGO) > 5 Then suma_largo = 13
                                        'acAttRef.AlignmentPoint = New Point3d(acAttRef.AlignmentPoint.X - suma_plus * COS(angle) - suma_largo, acAttRef.AlignmentPoint.Y - suma_plus * SIN(angle) - suma_largo, 0)
                                        acAttRef.AlignmentPoint = New Point3d(acAttRef.AlignmentPoint.X + (-suma_plus - suma_largo) * Cos(angle), acAttRef.AlignmentPoint.Y + (-suma_plus - suma_largo) * Sin(angle), 0)

                                    ElseIf acAttRef.Tag = "LARGO" Then
                                        acAttRef.TextString = AUX_LARGO
                                    ElseIf acAttRef.Tag = "PARCIAL" Then
                                        acAttRef.TextString = AUX_PARCIAL
                                    End If

                                    ' Agregar la referencia de atributo a la 
                                    ' referencia a bloque y a la transaccion
                                    acBlkRef.AttributeCollection.AppendAttribute(acAttRef)
                                    acTrans.AddNewlyCreatedDBObject(acAttRef, True)

                                End If
                            End If
                        Next
                    End If
                    '--------------------------------------------------------
                    ' Registrar el evento
                    'AddHandler acBlkRef.Modified, AddressOf reac_modif_triangulo_diam
                    '--------------------------------------------------------




                    'Dim acDesBlkRef As BlockReference = DirectCast(acTrans.GetObject(objId, OpenMode.ForWrite), BlockReference)

                    '' acEd.WriteMessage(String.Format("{0}Propiedades de ""{{0}}""{0}", ControlChars.Lf), acBlkRef.Name)

                    '' Obtener la coleccion de propiedades del blpque dinamico
                    'Dim DynPropsCol As DynamicBlockReferencePropertyCollection = acDesBlkRef.DynamicBlockReferencePropertyCollection

                    '' Obtener la informacion de cada propiedad
                    'For Each prop As DynamicBlockReferenceProperty In DynPropsCol
                    '    prop.Value = texto_diam
                    '    'acEd.WriteMessage(ControlChars.Lf & "  Valor actual: ""{0}""" & ControlChars.Lf, prop.Value)
                    'Next






                    ' Confirmar los cambios
                    acTrans.Commit()
                Else
                    MsgBox("NO se entro bloque de texto", vbCritical)
                    Exit Sub
                    aux_texto = True

                End If

            End Using
        End Using
    End Sub
    Function dibujar_barra_fund(ByRef ents As ObjectIdCollection, ByVal pt_ini As Point2d, ByVal pt_fin As Point2d, ByRef st_layer As String) As Polyline
        Dim Doc As Document = Application.DocumentManager.MdiActiveDocument
        Dim DB As Database = Doc.Database
        Dim Ed As Editor = Doc.Editor
        Dim acPoly As Polyline = New Polyline()



        Using tr As Transaction = DB.TransactionManager.StartTransaction()
            ' Abrir la tabla de bloques en modo lectura
            Dim acBlkTbl As BlockTable = tr.GetObject(DB.BlockTableId, OpenMode.ForRead)

            ' Abrir el registro del bloque de Espacio Modelo en modo escritura
            Dim acBlkTblRec As BlockTableRecord
            acBlkTblRec = tr.GetObject(acBlkTbl(BlockTableRecord.ModelSpace), OpenMode.ForWrite)

            ' Crear un circulo en 2,2 con radio 0.5
            Dim dista1 As Single = 20


            Dim acPolyObj As ObjectId = acPoly.ObjectId


            acPoly.SetDatabaseDefaults()

            acPoly.AddVertexAt(0, pt_ini, 0, 0, 0)
            acPoly.AddVertexAt(1, pt_fin, 0, 0, 0)
            acPoly.Layer = st_layer

            ' Añadir el nuevo objeto al registro de la tabla para bloques y a la
            ' transaccion
            acBlkTblRec.AppendEntity(acPoly)
            tr.AddNewlyCreatedDBObject(acPoly, True)
            If Not IsNothing(ents) Then ents.Add(acPoly.ObjectId)
            tr.Commit()
        End Using
        Return acPoly
    End Function

    Function intesrcion_barra_dimension(ByVal pto1_barra As Point2d, ByVal pto2_barra As Point2d, ByVal pto1_dim As Point2d, ByVal pto2_dim As Point2d) As Point3d

        Dim pto_inter As New Point3d

        Dim pt1 As New Point3dCollection()


        Dim acPoly1 As Polyline = dibujar_barra_fund(Nothing, pto1_barra, pto2_barra, "BARRAS")
        Dim acPoly2 As Polyline = dibujar_barra_fund(Nothing, pto1_dim, pto2_dim, "BARRAS")
        acPoly1.IntersectWith(acPoly2, Intersect.OnBothOperands, pt1, 0, 0)
        acPoly1.Erase()
        acPoly2.Erase()
        If pt1.Count > 0 Then
            pto_inter = pt1(0)
        Else
            MsgBox("No existe interseccion entre barra y dimension", vbCritical)
        End If


        Return pto_inter
    End Function


    ' Public Sub reac_modif_barra(ByVal senderObj As Object, ByVal evtArgs As EventArgs)


    Public Sub dibujar_texto_pl_reactor(ByRef ents As ObjectIdCollection, ByVal obj_ As ObjectId, ByRef aux_cuatia As String, ByVal AUX_LARGO As String, ByRef AUX_PARCIAL As String)
        Dim Doc As Document = Application.DocumentManager.MdiActiveDocument
        Dim DB As Database = Doc.Database
        Dim Ed As Editor = Doc.Editor
        Using tr As Transaction = DB.TransactionManager.StartTransaction()
            ' Abrir la tabla de bloques en modo lectura
            ' Dim e1 As Single = 13
            Try


                Dim acObjId_grup As ObjectId()
                Dim nombre_grupo As String
                ' ENTREGA UN COLECCION DE LOS ELEMTOS QUE PERTENECEN A UN DETERMINADO GRUPO, BUSCA EL NOMBRE DEL GRUPO CON ObjectID
                Dim GRUPO_ As New CODIGOS_GRUPOS()
                acObjId_grup = GRUPO_.buscar_grupo(obj_)
                Dim nombre_grupo_ As String = GRUPO_.buscar_nombre_grupo(obj_)


                For Each idObj As ObjectId In acObjId_grup
                    ' Dim acEnt_aux As Entity = acTrans.GetObject(idObj, OpenMode.ForWrite)



                    If idObj.ObjectClass.DxfName.ToString = "INSERT" Then

                        ' Abrir la tabla de bloques en modo lectura
                        ' Dim acBlkRef As BlockReference = TryCast(acTrans.GetObject(idObj, OpenMode.ForRead), BlockReference)

                        Dim acEnt As BlockReference = tr.GetObject(idObj, OpenMode.ForWrite)

                        Dim datos_estribo_valores(4) As String

                        '  Application.ShowAlertDialog("acEnt.BlockName : " & acEnt.Name & "     acEnt.BlockId : " & acEnt.Layer & "    acEnt.BlockName : " & acEnt.Layer)

                        ' If acEnt.Name = "_SCAD_CUANTIA_FUND_NH" Then



                        modificar_bloque_texto_fund(aux_cuatia, AUX_LARGO, AUX_PARCIAL, idObj, ents)

                        'If split_H_(1) = split_V_(1) Then
                        '    acEnt.Erase()
                        '    My1Commands.acObjIdColl_GENERAL.Remove(idObj)
                        '    texto_hatch(texto_coorde, texto_espesor, split_H(0), "", split_H(1), ents, 0)

                        'Else

                        '    acEnt.Erase()
                        '    My1Commands.acObjIdColl_GENERAL.Remove(idObj)
                        '    texto_hatch(texto_coorde, texto_espesor, split_V(0), split_V(1), split_H(1), ents, 1)

                        'End If
                        'End If



                    End If

                Next






                tr.Commit()

            Catch Ex As Autodesk.AutoCAD.Runtime.Exception
                tr.Dispose()
                Application.ShowAlertDialog("Error: 'reac_modif_barra_final'" & vbLf & Ex.Message)
            Finally
            End Try





        End Using
    End Sub
    Dim acBlkRef As BlockReference = Nothing
    Public Sub modificar_bloque_texto_fund(ByVal aux_cuatia As String, ByVal AUX_LARGO As String, ByVal AUX_PARCIAL As String, ByVal obj_ As Object, ByRef ents_dt As ObjectIdCollection)
        Dim flecha As String = ""
        ' Obtener el documento y la base de datos activos
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        ' Iniciar una transaccion
        Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()

            ' Abrir la tabla para bloques en modo lectura
            Dim acBlkTbl As BlockTable
            acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead)


            If acBlkTbl.Has("_SCAD_CUANTIA_FUND_NH2") Then
                flecha = "_SCAD_CUANTIA_FUND_NH2"
            Else
                flecha = "_SCAD_CUANTIA_FUND_NH"
            End If

            ' Comprobar si el bloque existe
            If acBlkTbl.Has(flecha) Then
                '' Abrir el registro del bloque de Espacio Modelo para escritura



                Dim acBlkRef As BlockReference = acTrans.GetObject(obj_, OpenMode.ForRead, False, True)
                Dim acAttCol As AttributeCollection = acBlkRef.AttributeCollection

                For Each acObjId In acAttCol
                    Dim acAttRef As AttributeReference = acTrans.GetObject(acObjId, OpenMode.ForWrite)
                    'Dim sStr As String = ControlChars.Lf & "Etiqueta: " & _
                    'acAttRef.Tag & _
                    'ControlChars.Lf & "Valor: " & _
                    'acAttRef.TextString

                    Dim suma_plus As Integer = 0
                    Dim suma_largo As Integer = 8
                    If acAttRef.Tag = "CUANTIA" Then
                        acAttRef.TextString = aux_cuatia
                        'Dim POTI3 As Point2d = New Point2d(acAttRef.AlignmentPoint.X, acAttRef.AlignmentPoint.Y)

                        'If InStr(1, aux_cuatia, "+") <> "0" Then suma_plus = 8
                        'If Len(AUX_LARGO) > 5 Then suma_largo = 13
                        'acAttRef.AlignmentPoint = New Point3d(acAttRef.AlignmentPoint.X - suma_plus - suma_largo, acAttRef.AlignmentPoint.Y, 0)


                    ElseIf acAttRef.Tag = "LARGO" Then
                        acAttRef.TextString = "L=" & AUX_LARGO
                    ElseIf acAttRef.Tag = "PARCIAL" Then
                        acAttRef.TextString = AUX_PARCIAL
                    End If
                Next

                '--------------------------------------------------------
                ' Registrar el evento
                'AddHandler acBlkRef.Modified, AddressOf reac_modif_triangulo_diam
                '--------------------------------------------------------

                ' Confirmar los cambios
                acTrans.Commit()
            End If

        End Using
    End Sub


    ''''
    Function aproximar(ByRef valor As Single, ByRef referen As Single, ByRef diamtro_ As Integer) As Single
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
    Function aproximar2_fund(ByRef valor As Double, ByRef referen As Single) As Double
        aproximar2_fund = 0

        Dim split_ As String() = valor.ToString.Split(New [Char]() {"."c, CChar(vbTab)})
        ' Dim parte_entera As Integer = Right(CStr(Fix(valor)), 1)

        If split_.Length = 2 Then
            valor = CSng("0." & split_(1))
        Else
            valor = 0
        End If

        Select Case valor
            Case Is < 0.01
                aproximar2_fund = 0

            Case Is < 1
                aproximar2_fund = 1 - valor
            Case Else
                aproximar2_fund = 0
        End Select



        If referen <> 0 Then

            If referen < 0 Then
                aproximar2_fund = -aproximar2_fund
            End If
        End If


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
    Public Sub Zoom_fund(ByVal pMin As Point3d, _
                    ByVal pMax As Point3d, _
                    ByVal pCenter As Point3d, _
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
                    pNewCentPt = New Point2d(((eExtents.MaxPoint.X + eExtents.MinPoint.X) * 0.5), _
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
End Class
