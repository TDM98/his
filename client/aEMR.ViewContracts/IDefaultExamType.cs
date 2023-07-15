using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    /// <summary>
    /// Danh sach cac pclexamtype cua 1 goi dich vu.
    /// </summary>
    public interface IDefaultExamType
    {
        long MedServiceID { get; set; }
        ObservableCollection<PCLExamType> PCLExamTypes { get; set; }
        bool IsLoading { get; set; }
    }
}
