using DataEntities;
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    public interface IOutwardDrugClinicDeptTemplateSelection
    {
        ObservableCollection<OutwardDrugClinicDeptTemplate> RequestTemplateCollection { get; set; }
        OutwardDrugClinicDeptTemplate CurrentRequestTemplate { get; set; }
        void LoadDataGrid();
    }
}