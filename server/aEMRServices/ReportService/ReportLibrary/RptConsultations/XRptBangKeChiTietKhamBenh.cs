using System;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptBangKeChiTietKhamBenh : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptBangKeChiTietKhamBenh()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsBangKeChiTietKhamBenh2.EnforceConstraints = false;
            spPatientRegistrationDetailsByRoomTableAdapter.Fill(dsBangKeChiTietKhamBenh2.spPatientRegistrationDetailsByRoom, Convert.ToInt32(DeptLocID.Value), Convert.ToDateTime(BeginDate.Value), Convert.ToDateTime(EndDate.Value),Convert.ToInt64(StaffID.Value));
        }

        void XRptBangKeChiTietKhamBenh_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
