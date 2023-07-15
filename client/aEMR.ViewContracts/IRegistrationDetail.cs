using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    /// <summary>
    /// Cap nhat thong tin cua 1 PCLRequestDetail.
    /// </summary>
    public interface IRegistrationDetail
    {
        PatientRegistrationDetail RegistrationDetail { get; set; }
        ObservableCollection<DeptLocation> DeptLocations { get; set; }
        bool IsLoadingLocations { get; set; }
        DeptLocation SelectedDeptLocation { get; set; }
    }
}
