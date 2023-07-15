using System;
using System.Collections.ObjectModel;
using DataEntities;


namespace aEMR.ViewContracts
{
    public interface IPatientDetailsView
    {
        void UpdateGeneralInfoToSource();
        void FocusOnFirstItem();
        void SetActiveTab(PatientInfoTabs activeTab);
        DateTime? DateBecamePatient { get; }
        
    }

    public interface IPatientDetailsAndHIView
    {
        void FocusOnFirstItem();
        DateTime? DateBecamePatient { get; }
    }
}
