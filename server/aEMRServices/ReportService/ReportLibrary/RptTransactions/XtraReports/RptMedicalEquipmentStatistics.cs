using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class RptMedicalEquipmentStatistics : DevExpress.XtraReports.UI.XtraReport
    {
        public RptMedicalEquipmentStatistics()
        {
            InitializeComponent();
        }
        private void RptMedicalEquipmentStatistics_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            lbSubTitle.Text = string.Format("(Kỳ hạn báo cáo năm {0})", this.Year.Value);
        }
    }
}
