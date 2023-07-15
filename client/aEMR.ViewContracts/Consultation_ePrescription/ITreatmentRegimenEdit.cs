using DataEntities;
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    public interface ITreatmentRegimenEdit
    {
        RefTreatmentRegimen gRefTreatmentRegimen { get; set; }
        ObservableCollection<DrugClass> GenericClasses { get; set; }
    }
}