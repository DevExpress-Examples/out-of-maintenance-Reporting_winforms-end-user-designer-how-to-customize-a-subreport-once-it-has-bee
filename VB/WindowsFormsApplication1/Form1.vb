Imports System
Imports System.Windows.Forms
Imports DevExpress.XtraReports.UI

Namespace WindowsFormsApplication1
	Partial Public Class Form1
		Inherits Form

		Public Sub New()
			InitializeComponent()
		End Sub

		Private Sub button1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button1.Click
			If Not Me.button1.IsHandleCreated Then Return

			Dim dt As New ReportDesignTool(New XtraReport1())
			dt.DesignRibbonForm.DesignMdiController.AddCommandHandler(New OpenSubreportCommandHandler(dt.DesignRibbonForm.DesignMdiController))
			dt.ShowRibbonDesignerDialog()
		End Sub
	End Class


End Namespace
