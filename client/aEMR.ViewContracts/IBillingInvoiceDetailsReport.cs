using DevExpress.ReportServer.Printing;


namespace aEMR.ViewContracts
{
    /// <summary>
    /// Hiển thị cửa sổ PatientDetails để Thêm BN, Quản lý BH của BN.
    /// </summary>
    public interface IBillingInvoiceDetailsReport
    {
        RemoteDocumentSource ReportModel { get; set; }
        long InPatientBillingInvID { get; set; }
    }
}
