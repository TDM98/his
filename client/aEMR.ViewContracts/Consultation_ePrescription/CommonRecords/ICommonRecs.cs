namespace aEMR.ViewContracts
{
    public interface ICommonRecs
    {
        object UCPatientPInfo { get; set; }
        bool IsShowSummaryContent { get; set; }
        void InitPatientInfo();

        // TxD 22/09/2018 Added the following to ALLOW child VM of each Tab to get ACCESS to the CENTRAL Data stored in the MainKB (Main KHAM BENH) Screen
        ICS_DataStorage CS_DS { get; set; }
        void GetRefImmunization(long MedServiceID);
        IRegistration_DataStorage Registration_DataStorage { get; set; }
    }
}