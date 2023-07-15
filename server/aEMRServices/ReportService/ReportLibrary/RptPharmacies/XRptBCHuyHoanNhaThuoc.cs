using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class XRptBCHuyHoanNhaThuoc : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptBCHuyHoanNhaThuoc()
        {
            InitializeComponent();
        }
        public void FillData()
        {
            dsXRptBCHuyHoanNhaThuoc1.EnforceConstraints = false;
            spRptHuy_HoanThuocTableAdapter.Fill(dsXRptBCHuyHoanNhaThuoc1.spRptHuy_HoanThuoc,Convert.ToDateTime(FromDate.Value), Convert.ToDateTime(ToDate.Value), Convert.ToInt32(StoreID.Value));
        }

        private void XRptBCHuyHoanNhaThuoc_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
