using DevExpress.ReportServer.Printing;


namespace aEMR.ViewContracts
{
    public interface IPaymentReport
    {
        RemoteDocumentSource ReportModel { get; set; }
        decimal PaymentID { set; }
        int FindPatient { get; set; }
        long CashAdvanceID { get; set; }
    }
}
