using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class SubTemp38NgoaiTruCheck : DevExpress.XtraReports.UI.XtraReport
    {
        public SubTemp38NgoaiTruCheck()
        {
            InitializeComponent();
        }

        private void SubTemp38NgoaiTruCheck_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            spRpt_CreateTemp38CheckTableAdapter.Fill(dsTemp38New1.spRpt_CreateTemp38Check, Convert.ToInt64(this.parTransactionID.Value),Convert.ToInt64(this.parPtRegistrationID.Value));
        }

    }
}
