using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface ISelectLocation
    {
        /// <summary>
        /// Item duoc kich hoat sau khi login.
        /// </summary>
        object ItemActivated { get; set; }
        //RefDepartments SelectedDepartment { get; set; }
        //ObservableCollection<RefDepartments> Departments { get; set; }
        DeptLocation SelectedLocation { get; set; }
        ObservableCollection<DeptLocation> Locations { get; set; }
        V_DeptTypeOperation V_DeptTypeOperation { get; set; }
        bool mCancel { get; set; }
    }
}
