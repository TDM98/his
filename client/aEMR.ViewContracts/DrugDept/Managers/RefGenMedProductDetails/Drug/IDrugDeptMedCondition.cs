using System.Collections.ObjectModel;
using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IDrugDeptMedCondition
    {
        EntitiesEdit<RefMedContraIndicationTypes> RefMedicalConditionType_Edit { get; set; }
        ObservableCollection<string> allContrainName { get; set; }
        RefGenMedProductDetails NewDrug { get; set; }
    }
}
