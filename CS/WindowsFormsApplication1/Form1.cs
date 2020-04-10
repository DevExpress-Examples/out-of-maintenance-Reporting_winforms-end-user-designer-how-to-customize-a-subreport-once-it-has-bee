using System;
using System.Windows.Forms;
using DevExpress.XtraReports.UI;

namespace WindowsFormsApplication1 {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            ReportDesignTool dt = new ReportDesignTool(new XtraReport1());
            dt.DesignRibbonForm.DesignMdiController.AddCommandHandler(new OpenSubreportCommandHandler(dt.DesignRibbonForm.DesignMdiController));
            dt.ShowRibbonDesignerDialog();
        }
    }

    
}
