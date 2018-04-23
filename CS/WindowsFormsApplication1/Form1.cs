using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraReports.UserDesigner.Native;

namespace WindowsFormsApplication1 {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            ReportDesignTool dt = new ReportDesignTool(new XtraReport1());
            dt.DesignForm.DesignMdiController.AddCommandHandler(new MyCommandHandler(dt.DesignForm.DesignMdiController));
            dt.ShowDesignerDialog();
        }
    }

    public class MyCommandHandler : ICommandHandler {
        XRDesignMdiController mdiController;

        public MyCommandHandler(XRDesignMdiController controller) {
            mdiController = controller;
        }

        #region ICommandHandler Members

        public bool CanHandleCommand(ReportCommand command, ref bool useNextHandler) {
            useNextHandler = command != ReportCommand.OpenSubreport;
            return !useNextHandler;
        }

        public void HandleCommand(ReportCommand command, object[] args) {
            XRDesignPanel panel = mdiController.ActiveDesignPanel;
            IWindowsService windowsSvc = panel.GetService(typeof(IWindowsService)) as IWindowsService;
            windowsSvc.EditSubreport((XRSubreport)args[0]);
            panel = mdiController.ActiveDesignPanel;
            IDesignerHost host = panel.GetService(typeof(IDesignerHost)) as IDesignerHost;
            if(host.Container.Components.OfType<SqlDataSource>().FirstOrDefault() == null) {

                Access97ConnectionParameters parameters = new Access97ConnectionParameters(@"|DataDirectory|\nwind.mdb", "", "");
                SqlDataSource ds = new SqlDataSource(parameters);

                CustomSqlQuery query = new CustomSqlQuery();
                query.Name = "customQuery1";
                query.Sql = "SELECT * FROM Products";
                ds.Queries.Add(query);
                ds.RebuildResultSchema();

                host.Container.Add(ds);

                panel.Report.DataSource = ds;
                panel.Report.DataMember = "customQuery1";


                int colCount = 3;
                int colWidth = (panel.Report.PageWidth - (panel.Report.Margins.Left + panel.Report.Margins.Right)) / colCount;

                XRTable tableDetail = new XRTable();
                tableDetail.Height = 20;
                tableDetail.Width = (panel.Report.PageWidth - (panel.Report.Margins.Left + panel.Report.Margins.Right));
                
                tableDetail.BeginInit();

                XRTableRow detailRow = new XRTableRow();
                detailRow.Width = tableDetail.Width;
                tableDetail.Rows.Add(detailRow);
                host.Container.Add(detailRow);

                XRTableCell detailCell = new XRTableCell();
                detailCell.Width = colWidth;
                detailCell.DataBindings.Add("Text", null, "customQuery1.ProductName");
                detailRow.Cells.Add(detailCell);

                detailCell = new XRTableCell();
                detailCell.Width = colWidth;
                detailCell.DataBindings.Add("Text", null, "customQuery1.QuantityPerUnit");
                detailRow.Cells.Add(detailCell);

                detailCell = new XRTableCell();
                detailCell.Width = colWidth;
                detailCell.DataBindings.Add("Text", null, "customQuery1.UnitPrice");
                detailRow.Cells.Add(detailCell);

                tableDetail.EndInit();

                panel.Report.Bands[BandKind.Detail].Controls.Add(tableDetail);

                host.Container.Add(tableDetail);
            }
        }

        #endregion
    }
}
