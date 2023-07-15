using DataEntities;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    public delegate void TreatmentCollectionOnChanged(IList<DiagnosisTreatment> aDiagnosisTreatmentCollection);
    public interface IDiagnosisTreatmentTree
    {
        void LoadData(long PtRegistrationID);
        IRegistration_DataStorage Registration_DataStorage { get; set; }
        void ApplyDiagnosisTreatmentCollection(IList<DiagnosisTreatment> aDiagnosisTreatmentCollection);
        TreatmentCollectionOnChanged gTreatmentCollectionOnChanged { get; set; }
        byte CurrentViewCase { get; set; }
        bool FromInPatientAdmView { get; set; }
        ObservableCollection<DiagnosisTreatment> DiagnosisTreatmentCollection { get; set; }
    }
}