using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_TinhHinhHoatDongCLS : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_TinhHinhHoatDongCLS()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsTinhHinhHoatDongCLS1.EnforceConstraints = false;
            sp_Rpt_KT_TinhHinhHoatDongCLSTableAdapter.Fill(dsTinhHinhHoatDongCLS1.sp_Rpt_KT_TinhHinhHoatDongCLS, Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value), Convert.ToInt64(parSectionID.Value));
            xrTable2.Visible = (bool)parIsDetail.Value;
        }

        private void XRpt_TinhHinhHoatDongCLS_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
