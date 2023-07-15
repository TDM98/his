using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IEditInwardDrug
    {
        InwardDrug CurrentInwardDrugCopy { get; set; }
        void SetValueForProperty();
    }
}
