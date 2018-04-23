Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Windows.Forms
Imports DevExpress.DataAccess.ConnectionParameters
Imports DevExpress.DataAccess.Sql
Imports DevExpress.XtraReports.UI
Imports DevExpress.XtraReports.UserDesigner
Imports DevExpress.XtraReports.UserDesigner.Native

Namespace WindowsFormsApplication1
	Partial Public Class Form1
		Inherits Form

		Public Sub New()
			InitializeComponent()
		End Sub

		Private Sub button1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button1.Click
			Dim dt As New ReportDesignTool(New XtraReport1())
			dt.DesignForm.DesignMdiController.AddCommandHandler(New MyCommandHandler(dt.DesignForm.DesignMdiController))
			dt.ShowDesignerDialog()
		End Sub
	End Class

	Public Class MyCommandHandler
		Implements ICommandHandler

		Private mdiController As XRDesignMdiController

		Public Sub New(ByVal controller As XRDesignMdiController)
			mdiController = controller
		End Sub

		#Region "ICommandHandler Members"

		Public Function CanHandleCommand(ByVal command As ReportCommand, ByRef useNextHandler As Boolean) As Boolean Implements ICommandHandler.CanHandleCommand
			useNextHandler = command <> ReportCommand.OpenSubreport
			Return Not useNextHandler
		End Function

		Public Sub HandleCommand(ByVal command As ReportCommand, ByVal args() As Object) Implements ICommandHandler.HandleCommand
			Dim panel As XRDesignPanel = mdiController.ActiveDesignPanel
			Dim windowsSvc As IWindowsService = TryCast(panel.GetService(GetType(IWindowsService)), IWindowsService)
			windowsSvc.EditSubreport(DirectCast(args(0), XRSubreport))
			panel = mdiController.ActiveDesignPanel
			Dim host As IDesignerHost = TryCast(panel.GetService(GetType(IDesignerHost)), IDesignerHost)
			If host.Container.Components.OfType(Of SqlDataSource)().FirstOrDefault() Is Nothing Then

				Dim parameters As New Access97ConnectionParameters("|DataDirectory|\nwind.mdb", "", "")
				Dim ds As New SqlDataSource(parameters)

				Dim query As New CustomSqlQuery()
				query.Name = "customQuery1"
				query.Sql = "SELECT * FROM Products"
				ds.Queries.Add(query)
				ds.RebuildResultSchema()

				host.Container.Add(ds)

				panel.Report.DataSource = ds
				panel.Report.DataMember = "customQuery1"


				Dim colCount As Integer = 3
				Dim colWidth As Integer = (panel.Report.PageWidth - (panel.Report.Margins.Left + panel.Report.Margins.Right)) \ colCount

				Dim tableDetail As New XRTable()
				tableDetail.Height = 20
				tableDetail.Width = (panel.Report.PageWidth - (panel.Report.Margins.Left + panel.Report.Margins.Right))

				tableDetail.BeginInit()

				Dim detailRow As New XRTableRow()
				detailRow.Width = tableDetail.Width
				tableDetail.Rows.Add(detailRow)
				host.Container.Add(detailRow)

				Dim detailCell As New XRTableCell()
				detailCell.Width = colWidth
				detailCell.DataBindings.Add("Text", Nothing, "customQuery1.ProductName")
				detailRow.Cells.Add(detailCell)

				detailCell = New XRTableCell()
				detailCell.Width = colWidth
				detailCell.DataBindings.Add("Text", Nothing, "customQuery1.QuantityPerUnit")
				detailRow.Cells.Add(detailCell)

				detailCell = New XRTableCell()
				detailCell.Width = colWidth
				detailCell.DataBindings.Add("Text", Nothing, "customQuery1.UnitPrice")
				detailRow.Cells.Add(detailCell)

				tableDetail.EndInit()

				panel.Report.Bands(BandKind.Detail).Controls.Add(tableDetail)

				host.Container.Add(tableDetail)
			End If
		End Sub

		#End Region
	End Class
End Namespace
