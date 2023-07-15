using DataEntities;
namespace aEMR.ViewContracts
{
    public interface ISupplierProduct_Edit
    {
        DrugDeptSupplier SelectedSupplier { get; set; }
        void LoadDSHangCC();
    }
}
