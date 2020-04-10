using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraReports.UserDesigner.Native;
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
            IWindowsService windowsSvc = panel.GetService(typeof(IWindowsService)) as IWindowsService;
            windowsSvc.EditSubreport((XRSubreport)args[0]);
            panel = mdiController.ActiveDesignPanel;
            IDesignerHost host = panel.GetService(typeof(IDesignerHost)) as IDesignerHost;
            if (host.Container.Components.OfType<SqlDataSource>().FirstOrDefault() == null) {
                CreateReport(panel, host);
                panel.Report.Site.Name = "MyReport";
            }
        }
        #endregion

        private void CreateReport(XRDesignPanel panel, IDesignerHost host) {
            Access97ConnectionParameters parameters = new Access97ConnectionParameters(@"|DataDirectory|\nwind.mdb", "", "");
            SqlDataSource ds = new SqlDataSource(parameters);

            SelectQuery query = SelectQueryFluentBuilder.AddTable("Products").SelectAllColumns().Build("Products");
            ds.Queries.Add(query);
            ds.RebuildResultSchema();

            host.Container.Add(ds);

            panel.Report.DataSource = ds;
            panel.Report.DataMember = query.Name;


            int colCount = 3;
            float colWidth = (panel.Report.PageWidth - (panel.Report.Margins.Left + panel.Report.Margins.Right)) / (float)colCount;

            XRTable tableDetail = new XRTable();
            tableDetail.HeightF = 25f;
            tableDetail.WidthF = (panel.Report.PageWidth - (panel.Report.Margins.Left + panel.Report.Margins.Right));

            tableDetail.BeginInit();

            XRTableRow detailRow = new XRTableRow();
            detailRow.WidthF = tableDetail.WidthF;
            tableDetail.Rows.Add(detailRow);
            host.Container.Add(detailRow);

            XRTableCell detailCell = new XRTableCell();
            detailCell.WidthF = colWidth;
            detailCell.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[ProductName]"));
            detailRow.Cells.Add(detailCell);
            host.Container.Add(detailCell);

            detailCell = new XRTableCell();
            detailCell.WidthF = colWidth;
            detailCell.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[QuantityPerUnit]"));
            detailRow.Cells.Add(detailCell);
            host.Container.Add(detailCell);

            detailCell = new XRTableCell();
            detailCell.WidthF = colWidth;
            detailCell.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[UnitPrice]"));
            detailRow.Cells.Add(detailCell);
            host.Container.Add(detailCell);

            tableDetail.EndInit();

            panel.Report.Bands[BandKind.Detail].HeightF = 25f;
            panel.Report.Bands[BandKind.Detail].Controls.Add(tableDetail);

            host.Container.Add(tableDetail);
        }



    }
}
