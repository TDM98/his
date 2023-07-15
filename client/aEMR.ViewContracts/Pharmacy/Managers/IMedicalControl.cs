using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IMedicalControl
    {
        ObservableCollection<RefMedContraIndicationTypes> allRefMedicalConditionType { get; set; }
    }
}
