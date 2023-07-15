using aEMR.ReportModel.BaseModels;

namespace aEMR.ReportModel.ReportModels
{
    public class OutPatientReceiptReportModel : ReportModelBase
    {
        public OutPatientReceiptReportModel()
            : base("eHCMS.ReportLib.RptPatientRegistration.XRptOutPatientReceipt")
        {
        }
      
    }
    public class OutPatientReceiptReportXMLModel : ReportModelBase
    {
        public OutPatientReceiptReportXMLModel()
            : base("eHCMS.ReportLib.RptPatientRegistration.XRptOutPatientReceiptXML")
        {
        }

    }
}
