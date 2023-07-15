namespace aEMR.ViewContracts
{
    public interface IConsultationList_InPt
    {
        //bool IsChildWindow { get; set; }        
        void GetDiagTrmtsByPtID_Ext();
        IRegistration_DataStorage Registration_DataStorage { get; set; }
    }
}