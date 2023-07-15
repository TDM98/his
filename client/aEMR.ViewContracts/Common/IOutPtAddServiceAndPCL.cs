using DataEntities;

namespace aEMR.ViewContracts
{
    public delegate void OnItemAdded(MedRegItemBase Item);
    public interface IOutPtAddServiceAndPCL
    {
        bool CanAddService { get; set; }
        RefMedicalServiceItem SelectedMedServiceItem { get; set; }
        Lookup SelectedEkip { get; set; }
        DeptLocation SelectedDeptLocation { get; set; }
        RefMedicalServiceType SelectedMedServiceType { get; set; }
        OnItemAdded OnItemAddedCallback { get; set; }
    }
}