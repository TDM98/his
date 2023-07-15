using aEMR.ReportModel.BaseModels;

namespace aEMR.ReportModel.ReportModels
{
    public class DiagnosisTreatmentByDoctorStaffID : ReportModelBase
    {
        public DiagnosisTreatmentByDoctorStaffID()
            : base("eHCMS.ReportLib.RptConsultations.XRptDiagnosisTreatmentByDoctorStaffID")
        {
        }
    }

    public class AllDiagnosisGroupByDoctorStaffIDDeptLocationID : ReportModelBase
    {
        public AllDiagnosisGroupByDoctorStaffIDDeptLocationID()
            : base("eHCMS.ReportLib.RptConsultations.XRptAllDiagnosisGroupByDoctorStaffIDDeptLocationID")
        {
        }
    }

    public class RptPatientPCLRequestDetailsByPatientPCLReqID : ReportModelBase
    {
        public RptPatientPCLRequestDetailsByPatientPCLReqID()
            : base("eHCMS.ReportLib.RptConsultations.XRptPatientPCLRequestDetailsByPatientPCLReqID")
        {
        }
    }
    // 20181023 TNHX: [BM0003221] Create Report RptPatientPCLRequestDetailsByPatientPCLReqID_TV which replace for RptPatientPCLRequestDetailsByPatientPCLReqID
    public class RptPatientPCLRequestDetailsByPatientPCLReqID_TV : ReportModelBase
    {
        public RptPatientPCLRequestDetailsByPatientPCLReqID_TV()
            :base("eHCMS.ReportLib.RptConsultations.XRptPatientPCLRequestDetailsByPatientPCLReqID_TV")
        {}
    }
    // 20181205 TNHX: [BM0005300] Create Report RptPatientServiceRequestDetailsByPatientServiceReqID_XML_TV
    public class RptPatientServiceRequestDetailsByPatientServiceReqID_XML_TV : ReportModelBase
    {
        public RptPatientServiceRequestDetailsByPatientServiceReqID_XML_TV()
            : base("eHCMS.ReportLib.RptConsultations.XRptPatientServiceDetailsByPatientServiceReqID_XML_TV")
        { }
    }
    public class RptPatientPCLRequestDetailsByPatientPCLReqID_TV_XML : ReportModelBase
    {
        public RptPatientPCLRequestDetailsByPatientPCLReqID_TV_XML()
            : base("eHCMS.ReportLib.RptConsultations.XRptPatientPCLRequestDetailsByPatientPCLReqID_TV_XML")
        { }
    }
}


