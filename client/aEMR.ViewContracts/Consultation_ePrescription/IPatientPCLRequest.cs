namespace aEMR.ViewContracts
{
    public interface IPatientPCLRequest
    {
        bool IsShowSummaryContent { get; set; }
        void InitPatientInfo();
        bool btSaveIsEnabled { get; }

        // TxD 22/09/2018 Added the following to ALLOW child VM of each Tab to get ACCESS to the CENTRAL Data stored in the MainKB (Main KHAM BENH) Screen
        ICS_DataStorage CS_DS { get; set; }
        bool IsShowCheckBoxPayAfter { get; set; }
        IRegistration_DataStorage Registration_DataStorage { get; set; }
        long V_RegistrationType { get; set; }
        void CheckDeptLocation();
    }
}