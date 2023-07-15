using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;


namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptPhieuCongKhaiDV : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPhieuCongKhaiDV()
        {
            InitializeComponent();
        }

        private void XRptPhieuCongKhaiDV_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsXRptPhieuCongKhaiDV1.EnforceConstraints = false;

            spXRptPhieuCongKhaiDVTableAdapter.Fill(dsXRptPhieuCongKhaiDV1.spXRptPhieuCongKhaiDV, Convert.ToInt64(PtRegistrationID.Value),
                Convert.ToDateTime(FromDate.Value), Convert.ToDateTime(ToDate.Value), Convert.ToInt64(DeptID.Value));
            spXRptPhieuCongKhaiDV_PatientInfoTableAdapter.Fill(dsXRptPhieuCongKhaiDV1.spXRptPhieuCongKhaiDV_PatientInfo, Convert.ToInt64(PtRegistrationID.Value),
                Convert.ToInt64(DeptID.Value),Convert.ToInt64(StaffID.Value));
        }
    }
}
