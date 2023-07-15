using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRptBCCongTacKCB : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptBCCongTacKCB()
        {
            InitializeComponent();
        }
        private void FillData()
        {
           // (this.DataSource as DataSchema.dsBCCongTacKCB).spBCCongTacKCB
            this.dsBCCongTacKCB2.EnforceConstraints = false;
            spBCCongTacKCBTableAdapter1.Fill(dsBCCongTacKCB2.spBCCongTacKCB, Convert.ToDateTime(FromDate.Value), Convert.ToDateTime(ToDate.Value));
            spBCCongTacKCB_CLSNgoaiTruTableAdapter1.Fill(dsBCCongTacKCB2.spBCCongTacKCB_CLSNgoaiTru,Convert.ToDateTime(FromDate.Value),Convert.ToDateTime(ToDate.Value));
            spBCCongTacKCB_CLSNoiTruTableAdapter1.Fill(dsBCCongTacKCB2.spBCCongTacKCB_CLSNoiTru, Convert.ToDateTime(FromDate.Value), Convert.ToDateTime(ToDate.Value));
            spBCCongTacKCB_ThuThuatNgoaiTruTableAdapter1.Fill(dsBCCongTacKCB2.spBCCongTacKCB_ThuThuatNgoaiTru, Convert.ToDateTime(FromDate.Value), Convert.ToDateTime(ToDate.Value));
        }
        private void XRptBCCongTacKCB_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
