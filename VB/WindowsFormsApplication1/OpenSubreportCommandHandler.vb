Imports DevExpress.DataAccess.ConnectionParameters
Imports DevExpress.DataAccess.Sql
Imports DevExpress.XtraReports.UI
Imports DevExpress.XtraReports.UserDesigner
Imports DevExpress.XtraReports.UserDesigner.Native
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel.Design
Imports System.Linq

Namespace WindowsFormsApplication1
	Public Class OpenSubreportCommandHandler
		Implements ICommandHandler

		Private mdiController As XRDesignMdiController

		Public Sub New(ByVal controller As XRDesignMdiController)
			mdiController = controller
		End Sub

		#Region "ICommandHandler Members"

		Private Function ICommandHandler_CanHandleCommand(ByVal command As ReportCommand, ByRef useNextHandler As Boolean) As Boolean Implements ICommandHandler.CanHandleCommand
			useNextHandler = command <> ReportCommand.OpenSubreport
			Return Not useNextHandler
		End Function

		Private Sub ICommandHandler_HandleCommand(ByVal command As ReportCommand, ByVal args() As Object) Implements ICommandHandler.HandleCommand
			Dim panel As XRDesignPanel = mdiController.ActiveDesignPanel
			Dim windowsSvc As IWindowsService = TryCast(panel.GetService(GetType(IWindowsService)), IWindowsService)
			windowsSvc.EditSubreport(DirectCast(args(0), XRSubreport))
			panel = mdiController.ActiveDesignPanel
			Dim host As IDesignerHost = TryCast(panel.GetService(GetType(IDesignerHost)), IDesignerHost)
			If host.Container.Components.OfType(Of SqlDataSource)().FirstOrDefault() Is Nothing Then
				CreateReport(panel, host)
				panel.Report.Site.Name = "MyReport"
			End If
		End Sub
		#End Region

		Private Sub CreateReport(ByVal panel As XRDesignPanel, ByVal host As IDesignerHost)
			Dim parameters As New Access97ConnectionParameters("|DataDirectory|\nwind.mdb", "", "")
			Dim ds As New SqlDataSource(parameters)

			Dim query As SelectQuery = SelectQueryFluentBuilder.AddTable("Products").SelectAllColumns().Build("Products")
			ds.Queries.Add(query)
			ds.RebuildResultSchema()

			host.Container.Add(ds)

			panel.Report.DataSource = ds
			panel.Report.DataMember = query.Name


			Dim colCount As Integer = 3
'INSTANT VB WARNING: Instant VB cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Dim colWidth As Single = (panel.Report.PageWidth - (panel.Report.Margins.Left + panel.Report.Margins.Right)) / CSng(colCount)

			Dim tableDetail As New XRTable()
			tableDetail.HeightF = 25F
			tableDetail.WidthF = (panel.Report.PageWidth - (panel.Report.Margins.Left + panel.Report.Margins.Right))

			tableDetail.BeginInit()

			Dim detailRow As New XRTableRow()
			detailRow.WidthF = tableDetail.WidthF
			tableDetail.Rows.Add(detailRow)
			host.Container.Add(detailRow)

			Dim detailCell As New XRTableCell()
			detailCell.WidthF = colWidth
			detailCell.ExpressionBindings.Add(New ExpressionBinding("BeforePrint", "Text", "[ProductName]"))
			detailRow.Cells.Add(detailCell)
			host.Container.Add(detailCell)

			detailCell = New XRTableCell()
			detailCell.WidthF = colWidth
			detailCell.ExpressionBindings.Add(New ExpressionBinding("BeforePrint", "Text", "[QuantityPerUnit]"))
			detailRow.Cells.Add(detailCell)
			host.Container.Add(detailCell)

			detailCell = New XRTableCell()
			detailCell.WidthF = colWidth
			detailCell.ExpressionBindings.Add(New ExpressionBinding("BeforePrint", "Text", "[UnitPrice]"))
			detailRow.Cells.Add(detailCell)
			host.Container.Add(detailCell)

			tableDetail.EndInit()

			panel.Report.Bands(BandKind.Detail).HeightF = 25F
			panel.Report.Bands(BandKind.Detail).Controls.Add(tableDetail)

			host.Container.Add(tableDetail)
		End Sub



	End Class
End Namespace
