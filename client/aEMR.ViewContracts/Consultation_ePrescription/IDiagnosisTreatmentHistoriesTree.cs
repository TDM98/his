using System;

namespace aEMR.ViewContracts
{
    public interface IDiagnosisTreatmentHistoriesTree
    {
        IRegistration_DataStorage Registration_DataStorage { get; set; }
        void GetPatientServicesTreeView(long aPatientID);
        void NotifyViewDataChanged();
        bool IsOutPtTreatmentProgram { get; set; }
        byte CurrentViewCase { get; set; }
        void GetPatientServicesTreeView_ByPtRegistrationID(long PtRegistrationID);
        DateTime? ToDate { get; set; }
        DateTime? FromDate { get; set; }
    }
}