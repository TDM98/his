using aEMR.ReportModel.BaseModels;

namespace aEMR.ReportModel.ReportModels
{
    public class InPatientBillingInvoiceDetailsReportModel : ReportModelBase
    {
        //KMx:Đổi sang report khác (giống mẫu 02 nội trú) (04/12/2014 15:14).
        //public InPatientBillingInvoiceDetailsReportModel()
        //    : base("eHCMS.ReportLib.RptPatientRegistration.XRptInPatientBillingInvoiceDetails")
        //{
        //}

        public InPatientBillingInvoiceDetailsReportModel()
            //: base("eHCMS.ReportLib.RptPatientRegistration.XRpt_InPatientBillingInvoice")
            : base("eHCMS.ReportLib.RptPatientRegistration.XRpt_InPatientBillingInvoiceNew")
        {
        }
    }
}
