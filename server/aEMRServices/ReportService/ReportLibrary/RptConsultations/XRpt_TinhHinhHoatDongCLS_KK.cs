using System;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRpt_TinhHinhHoatDongCLS_KK : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_TinhHinhHoatDongCLS_KK()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsTinhHinhHoatDongCLS_KK1.EnforceConstraints = false;
            sp_Rpt_KT_TinhHinhHoatDongCLS_KKTableAdapter.Fill(dsTinhHinhHoatDongCLS_KK1.sp_Rpt_KT_TinhHinhHoatDongCLS_KK, Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value), Convert.ToInt64(parSectionID.Value));
            xrTable2.Visible = (bool)parIsDetail.Value;
        }

        private void XRpt_TinhHinhHoatDongCLS_KK_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
