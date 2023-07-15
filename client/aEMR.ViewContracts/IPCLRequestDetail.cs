using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    /// <summary>
    /// Cap nhat thong tin cua 1 PCLRequestDetail.
    /// </summary>
    public interface IPCLRequestDetail
    {
        PatientPCLRequestDetail PCLRequestDetail { get; set; }
        ObservableCollection<DeptLocation> DeptLocations { get; set; }
        bool IsLoadingPclExamTypeLocations { get; set; }
        PCLExamTypeLocation SelectedExamTypeLocation { get; set; }
        DeptLocation SelectedDeptLocation { get; set; }
    }
}
