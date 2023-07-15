using System;

namespace eHCMS.ReportLib.PCLDepartment
{
    public partial class TinhHinhHoatDongCLS : DevExpress.XtraReports.UI.XtraReport
    {
        public TinhHinhHoatDongCLS()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsTinhHinhHoatDongCLS1.EnforceConstraints = false;
            sp_Rpt_TinhHinhHoatDongCLSTableAdapter.Fill(dsTinhHinhHoatDongCLS1.sp_Rpt_TinhHinhHoatDongCLS, Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value), Convert.ToInt64(parSectionID.Value));
        }

        private void TinhHinhHoatDongCLS_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
