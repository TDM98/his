using aEMR.ReportModel.BaseModels;

namespace aEMR.ReportModel.ReportModels
{
    using DevExpress.Xpf.Printing;
    public class InwardDrugClinicDeptReportModel : ReportModelBase
    {
        public InwardDrugClinicDeptReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.XRptInwardDrugClinicDept")
        {

        }
    }
    public class RptDanhSachXuatKhoPhongModel : ReportModelBase
    {
        public RptDanhSachXuatKhoPhongModel()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Outward.XRptDanhSachXuatKhoPhong")
        {

        }
    }

    public class RptOutwardFromClinicDeptToDrugDeptModel : ReportModelBase
    {
        public RptOutwardFromClinicDeptToDrugDeptModel()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Outward.XRptFromClinicDeptToDrugDept")
        {

        }
    }
    public class RptOutwardFromClinicDeptToPatientModel : ReportModelBase
    {
        public RptOutwardFromClinicDeptToPatientModel()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Outward.XRptFromClinicDeptToPatient")
        {

        }
    }

    public class XRptFromClinicDeptToPatientDetails : ReportModelBase
    {
        public XRptFromClinicDeptToPatientDetails()
            : base("eHCMS.ReportLib.RptDrugDepts.XtraReports.Outward.XRptFromClinicDeptToPatientDetails")
        {

        }
    }
}
