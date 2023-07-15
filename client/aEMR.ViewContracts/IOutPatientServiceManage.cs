using System.Collections.ObjectModel;
using System.Windows.Data;
using DataEntities;
using aEMR.Common.PagedCollectionView;
using System.Collections.Generic;
using System.Windows;

namespace aEMR.ViewContracts
{
    /// <summary>
    /// Quản lý danh sách dịch vụ của bệnh nhân ngoại trú (thêm, xóa)
    /// </summary>
    public interface IOutPatientServiceManage
    {
        //PagedCollectionView RegistrationDetails { get; set; }
        ObservableCollection<PatientRegistrationDetail> RegistrationDetails { get; set; }
        CollectionView CV_RegDetailItems { get; set; }
        bool HiServiceBeingUsed { get; set; }
        bool ShowCheckBoxColumn { get; set; }
        bool IsOldList { get; set; }
        void UpdateServiceItemList(List<PatientRegistrationDetail> updatedList) ;
        PatientRegistration RegistrationObj { get; set; }
        bool CanDelete { get; set; }
        void NotifyOfCanApplyIsOnPriceDiscount();
    }
}
