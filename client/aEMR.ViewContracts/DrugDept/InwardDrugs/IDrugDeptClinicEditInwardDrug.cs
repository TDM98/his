using DataEntities;
using System.Collections.ObjectModel;
namespace aEMR.ViewContracts
{
    public interface IDrugDeptClinicEditInwardDrug
    {
        InwardDrugClinicDept CurrentInwardDrugClinicDeptCopy { get; set; }

        ObservableCollection<Lookup> CbxGoodsTypes { get; set; }
    }
}
