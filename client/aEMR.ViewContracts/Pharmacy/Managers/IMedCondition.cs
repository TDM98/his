using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IMedCondition
    {
        EntitiesEdit<RefMedContraIndicationTypes> RefMedicalConditionType_Edit { get; set; }
        ObservableCollection<string> allContrainName { get; set; }
        RefGenericDrugDetail NewDrug { get; set; }
    }
}
