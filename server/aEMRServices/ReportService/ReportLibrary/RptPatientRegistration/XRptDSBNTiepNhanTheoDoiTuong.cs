using System;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptDSBNTiepNhanTheoDoiTuong : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptDSBNTiepNhanTheoDoiTuong()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            dsDSachBNTiepNhanTheoDT.EnforceConstraints = false;
            spRptAdmissionStatisticsTableAdapter.Fill(dsDSachBNTiepNhanTheoDT.spRptAdmissionStatistics
                                                                , Convert.ToDateTime(FromDate.Value)
                                                                , Convert.ToDateTime(ToDate.Value));
        }

        private void XRptDSBNTiepNhanTheoDoiTuong_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
