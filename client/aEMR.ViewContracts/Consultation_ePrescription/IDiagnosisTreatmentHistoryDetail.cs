using DataEntities;
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    public interface IDiagnosisTreatmentHistoryDetail
    {
        Prescription CurrentPrescription { get; set; }
        DiagnosisTreatment CurrentDiagnosisTreatment { get; set; }
        ObservableCollection<DiagnosisIcd10Items> CurrentIcd10Collection { get; set; }
        ObservableCollection<DiagnosisICD9Items> CurrentIcd9Collection { get; set; }
    }
}