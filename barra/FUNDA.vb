Imports System.Drawing.Color

Imports Autodesk.AutoCAD.DatabaseServices
Imports System
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.ApplicationServices
Imports System.Math
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Windows

Imports aux_planta
Imports ESTRIBOS_MUROS
Imports System.IO
Imports System.Security
Imports System.Security.Cryptography
Imports System.Text
Imports System.Data
Imports System.Data.SqlClient
Imports System.Drawing
Imports System.Windows.Forms


Public Class FUNDA

    Private Sub Button3_Click(sender As System.Object, e As System.EventArgs) Handles btm_barra.Click
        Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()
        ' Dim caso_m1 As New M1
        '   caso_m1.aux__barra(My1Commands.myPalette.cbx_dia_princiapl.Text, My1Commands.myPalette.ckbx_barra_refuerzo.Checked, My1Commands.myPalette.rbt_inferior.Checked, My1Commands.myPalette.cbx_dia_princiapl.Text, My1Commands.myPalette.cbx_sepa_princiapl.Text, My1Commands.myPalette.ckbx_traslapo.Checked, My1Commands.casos_dibujar, My1Commands.grupo_referencia, My1Commands.myPalette.txt_recub.Text)

        Dim acDoc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
        acDoc.SendStringToExecute("._FF1 ", True, False, False)

    End Sub

    Private Sub btm_rango_Click(sender As System.Object, e As System.EventArgs) Handles btm_rango.Click
        Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()
        ' Dim caso_m1 As New M1
        ' caso_m1.aux__barra_rango(My1Commands.myPalette.txt_recub.Text, My1Commands.casos_dibujar)

        Dim acDoc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
        acDoc.SendStringToExecute("._FF2 ", True, False, False)
    End Sub

    Private Sub Button10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_traslapo.Click
        Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()
        'Dim caso_m1 As New M1
        'caso_m1.aux_dtras_fund(My1Commands.myPalette.txt_recub.Text, My1Commands.myPalette.ckbx_traslapo.Checked, My1Commands.grupo_referencia, My1Commands.casos_dibujar, My1Commands._contenerdorIDOBJ)

        Dim acDoc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
        acDoc.SendStringToExecute("._FF3 ", True, False, False)
    End Sub

    Private Sub btn_aceptar_estribo_Click(sender As System.Object, e As System.EventArgs) Handles btn_aceptar_estribo.Click



        '   ESTRIBO_FUNDACIONES()
        Dim acDoc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
        acDoc.SendStringToExecute("._EBFU ", True, False, False)
    End Sub



    Private Sub FUNDA_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim caso_m1 As New M1
        '  caso_m1.aux_reescribir_barras(My1Commands.acObjIdColl_GENERAL, "all_rev")


        Dim utiles_aux As New utiles
        utiles_aux.Create_ALayer("CESTRIBO_E")
        utiles_aux.Create_ALayer("CESTRIBO_L")
        utiles_aux.Create_ALayer("ESTRIBO")
        utiles_aux.Create_ALayer("BARRAS")
        utiles_aux.Create_ALayer("TEXTO")


        'DataGridView_largoPata.Rows.Add("8", "20")
        'DataGridView_largoPata.Rows.Add("10", "20")
        'DataGridView_largoPata.Rows.Add("12", "20")
        'DataGridView_largoPata.Rows.Add("16", "20")
        'DataGridView_largoPata.Rows.Add("18", "25")
        'DataGridView_largoPata.Rows.Add("22", "30")
        'DataGridView_largoPata.Rows.Add("25", "40")
        'DataGridView_largoPata.Rows.Add("28", "40")
        'DataGridView_largoPata.Rows.Add("32", "50")
        'DataGridView_largoPata.Rows.Add("36", "50")

        DataGridView_largoPata.Rows.Add("20", "8")
        DataGridView_largoPata.Rows.Add("20", "10")
        DataGridView_largoPata.Rows.Add("20", "12")
        DataGridView_largoPata.Rows.Add("20", "16")
        DataGridView_largoPata.Rows.Add("25", "18")
        DataGridView_largoPata.Rows.Add("30", "22")
        DataGridView_largoPata.Rows.Add("40", "25")
        DataGridView_largoPata.Rows.Add("40", "28")
        DataGridView_largoPata.Rows.Add("50", "32")
        DataGridView_largoPata.Rows.Add("50", "36")

        If True Then caso_m1.aux_reescribir_barras(My1Commands.acObjIdColl_GENERAL, "all_rev")



        Me.ToolTip1.SetToolTip(btn_revisar, "REV_F")
        Me.ToolTip1.SetToolTip(btn_corregir, "REFE_F")

        Me.ToolTip1.SetToolTip(btn_revisar, "Revisa y corrige datos en barras(REBA)")
    End Sub

    Private Sub btn_recalcular_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_revisar.Click

        Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()
        Dim acDoc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
        acDoc.SendStringToExecute(".REV_F ", True, False, False)

    End Sub

    Private Sub btn_corregir_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_corregir.Click

        Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()
        Dim acDoc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
        acDoc.SendStringToExecute(".REFE_F ", True, False, False)

    End Sub

    Private Sub chbx_tr_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chbx_tr.CheckedChanged
        If chbx_tr.Checked = True Then
            cbx_tr.Enabled = True
            cbx_diam_tr.Enabled = True
            cbx_espa_tr.Enabled = True
        Else
            cbx_tr.Enabled = False
            cbx_diam_tr.Enabled = False
            cbx_espa_tr.Enabled = False
        End If
    End Sub
End Class
