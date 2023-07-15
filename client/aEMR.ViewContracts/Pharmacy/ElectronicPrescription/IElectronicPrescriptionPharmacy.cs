namespace aEMR.ViewContracts
{
    public interface IElectronicPrescriptionPharmacyDelete
    {
        long PtRegistrationID { get; set; }
        long DTDTReportID { get; set; }
        long DQGReportID { get; set; }

        long V_RegistrationType { get; set; }
        long IssueID { get; set; }
    }
}