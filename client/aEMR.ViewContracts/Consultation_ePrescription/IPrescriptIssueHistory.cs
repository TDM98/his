namespace aEMR.ViewContracts
{
    public interface IPrescriptIssueHistory
    {
        long PrescriptID { get; set; }
        void GetPrescriptionIssueHistory();
    }
}
