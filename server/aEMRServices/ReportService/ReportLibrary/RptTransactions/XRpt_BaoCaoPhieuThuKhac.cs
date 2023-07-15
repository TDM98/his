using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_BaoCaoPhieuThuKhac : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BaoCaoPhieuThuKhac()
        {
            InitializeComponent();            
        }

        public void FillData()
        {
            spGenericPayment_GetAllByCreatedDateTableAdapter.Fill((this.DataSource as DataSchema.dsBaoCaoPhieuThuKhac).spGenericPayment_GetAllByCreatedDate, 
                Convert.ToDateTime(this.FromDate.Value).Date, Convert.ToDateTime(this.ToDate.Value).Date,
                null, Convert.ToInt32(this.parStaffID.Value));
        }

        private void XRpt_BaoCaoPhieuThuKhac_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
