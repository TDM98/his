using aEMR.ReportModel.BaseModels;

namespace aEMR.ReportModel.ReportModels
{
    public class ConsultationEPrescriptionReportModel : ReportModelBase
    {
        //public ConsultationEPrescriptionReportModel()
        //    : base("eHCMS.ReportLib.RptConsultations.XRptEPrescriptionNew")
        public ConsultationEPrescriptionReportModel()
            : base("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescription_V2_SubReport")
        {
        }
    }
    public class ConsultationEPrescription_InPtReportModel : ReportModelBase
    {
        //public ConsultationEPrescription_InPtReportModel()
        //    : base("eHCMS.ReportLib.RptConsultations.XRptEPrescriptionNew_InPt")
        public ConsultationEPrescription_InPtReportModel()
            : base("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescriptionInpt_V2_SubReport")
        {
        }
    }
    /*TMA 22/11/2017 THỬ MẪU BÁO CÁO MỚI*/
    public class ConsultationEPrescriptionReportModelNew: ReportModelBase
    {
        // 20181112 TNHX: [BM0005222] Apply new report for TV
        public ConsultationEPrescriptionReportModelNew()
            : base("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescription_V2_SubReport_TV")
        {
        }
    }
    public class ConsultationEPrescription_InPtReportModelNew : ReportModelBase
    {
        public ConsultationEPrescription_InPtReportModelNew()
            : base("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescriptionInpt_V2_SubReport_TV")
        {
        }
    }
    /*TMA 22/11/2017 THỬ MẪU BÁO CÁO MỚI*/

    public class ConsultationEPrescriptionPrivateReportModel : ReportModelBase
    {
        public ConsultationEPrescriptionPrivateReportModel()
            : base("eHCMS.ReportLib.RptConsultations.XRptEPrescriptionNewPrivate")
        {
        }
    }
    // 20200519 TNHX: Thêm giao dien mới cho TV3
    public class ConsultationEPrescriptionReportModel_TV3 : ReportModelBase
    {
        public ConsultationEPrescriptionReportModel_TV3()
            : base("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescription_V2_SubReport_TV3")
        {
        }
    }
    public class ConsultationEPrescription_InPtReportModel_TV3 : ReportModelBase
    {
        public ConsultationEPrescription_InPtReportModel_TV3()
            : base("eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescriptionInpt_V2_SubReport_TV3")
        {
        }
    }
}
