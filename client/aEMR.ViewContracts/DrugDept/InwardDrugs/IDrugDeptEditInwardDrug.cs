using DataEntities;
using System.Collections.ObjectModel;
namespace aEMR.ViewContracts
{
    public interface IDrugDeptEditInwardDrug
    {
        InwardDrugMedDept CurrentInwardDrugMedDeptCopy { get; set; }

        ObservableCollection<Lookup> CbxGoodsTypes { get; set; }
        void SetValueForProperty();
        bool IsVND { get; set; }
        long V_MedProductType { get; set; }
    }
    public interface IVTYTTHEditInward
    {
        InwardDrugMedDept CurrentInwardDrugMedDeptCopy { get; set; }

        ObservableCollection<Lookup> CbxGoodsTypes { get; set; }
        void SetValueForProperty();
        bool IsVND { get; set; }
    }
}
