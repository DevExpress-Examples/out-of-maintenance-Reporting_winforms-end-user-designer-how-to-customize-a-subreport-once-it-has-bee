using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraReports.UserDesigner.Native;
using DevExpress.XtraRichEdit.Layout;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

namespace WindowsFormsApplication1 {
    public class OpenSubreportCommandHandler : ICommandHandler {
        XRDesignMdiController mdiController;

        public OpenSubreportCommandHandler(XRDesignMdiController controller) {
            mdiController = controller;
        }

        #region ICommandHandler Members

        bool ICommandHandler.CanHandleCommand(ReportCommand command, ref bool useNextHandler) {
            useNextHandler = command != ReportCommand.OpenSubreport;
            return !useNextHandler;
        }

        void ICommandHandler.HandleCommand(ReportCommand command, object[] args) {
            XRDesignPanel panel = mdiController.ActiveDesignPanel;

            XRSubreport subreport = (XRSubreport)args[0];
            if (subreport.ReportSource == null && String.IsNullOrEmpty(subreport.ReportSourceUrl)) {
                subreport.ReportSource = CreateReport();
            }

            IWindowsService windowsSvc = panel.GetService(typeof(IWindowsService)) as IWindowsService;
            windowsSvc.EditSubreport(subreport);
            subreport.ReportSource = null;
        }
        #endregion

        private XtraReport CreateReport() {            
            
            Access97ConnectionParameters parameters = new Access97ConnectionParameters(@"|DataDirectory|\nwind.mdb", "", "");
            SqlDataSource ds = new SqlDataSource(parameters);

            SelectQuery query = SelectQueryFluentBuilder.AddTable("Products").SelectAllColumns().Build("Products");
            ds.Queries.Add(query);
            ds.RebuildResultSchema();


            XtraReport report = new XtraReport() {
                DataSource = ds,
                DataMember = query.Name
            };


            float actualPageWidth = report.PageWidth - (report.Margins.Left + report.Margins.Right);
            int colCount = 3;
            float colWidth = actualPageWidth / colCount;

            XRTable tableDetail = new XRTable() {
                HeightF = 25f,
                WidthF = actualPageWidth
            };
            
            tableDetail.BeginInit();

            XRTableRow detailRow = new XRTableRow();
            detailRow.WidthF = tableDetail.WidthF;
            tableDetail.Rows.Add(detailRow);

            XRTableCell detailCell = new XRTableCell() { WidthF = colWidth };
            detailCell.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[ProductName]"));
            detailRow.Cells.Add(detailCell);

            detailCell = new XRTableCell() { WidthF = colWidth };
            detailCell.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[QuantityPerUnit]"));
            detailRow.Cells.Add(detailCell);

            detailCell = new XRTableCell() { WidthF = colWidth };
            detailCell.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[UnitPrice]"));
            detailRow.Cells.Add(detailCell);

            tableDetail.EndInit();

            report.Bands.Add(new DetailBand() {
                HeightF = 25f
            });
            report.Bands[BandKind.Detail].Controls.Add(tableDetail);

            return report;
        }



    }
}
