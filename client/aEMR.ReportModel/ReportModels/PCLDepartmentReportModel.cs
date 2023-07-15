using aEMR.ReportModel.BaseModels;

namespace aEMR.ReportModel.ReportModels
{
    public class PCLDepartmentLaboratoryResultReportModel : ReportModelBase
    {
        public PCLDepartmentLaboratoryResultReportModel()
            : base("eHCMS.ReportLib.PCLDepartment.XRptPatientPCLLaboratoryResults")
        {

        }
    }

    public class PCLDeptImageResult_HeartUltraSoundType1ReportModel : ReportModelBase
    {
        public PCLDeptImageResult_HeartUltraSoundType1ReportModel()
            : base("eHCMS.ReportLib.PCLDepartment.XRptEchoCardiographyType1Results")
        {

        }
    }
    // 20181024 TNHX: [BM0003200] Create new report XRptPatientPCLLaboratoryResults_TV for Thanh Vu Hospital
    public class PCLDepartmentLaboratoryResultReportModel_TV : ReportModelBase
    {
        public PCLDepartmentLaboratoryResultReportModel_TV()
            : base("eHCMS.ReportLib.PCLDepartment.XRptPatientPCLLaboratoryResults_TV_New")
        {
        }
    }
    // 20200528 TNHX: [] Create new report XRptPatientPCLLaboratoryResults_TV3 for Thanh Vu Hospital 3
    public class PCLDepartmentLaboratoryResultReportModel_TV3 : ReportModelBase
    {
        public PCLDepartmentLaboratoryResultReportModel_TV3()
            : base("eHCMS.ReportLib.PCLDepartment.XRptPatientPCLLaboratoryResults_TV3")
        {
        }
    }
}
