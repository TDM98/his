namespace aEMR.ViewContracts
{
    public interface IConsultationList
    {
        //bool IsChildWindow { get; set; }        
        void GetDiagTrmtsByPtID_Ext();
        void InitPatientInfo();
        IRegistration_DataStorage Registration_DataStorage { get; set; }
    }
}