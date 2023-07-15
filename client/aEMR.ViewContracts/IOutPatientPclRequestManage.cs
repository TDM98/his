using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    /// <summary>
    /// Quản lý danh sách CLS của bệnh nhân ngoại trú (thêm, xóa)
    /// </summary>
    public interface IOutPatientPclRequestManage
    {
        //ObservableCollection<PatientPCLRequestDetail> PclServiceDetails { get; set; }
        ObservableCollection<PatientPCLRequestDetail> PtPclReqDetailItems { get; set; }
        ObservableCollection<PatientPCLRequest> PCLRequests { get; set; }
        bool HiServiceBeingUsed { get; set; }
        bool ShowCheckBoxColumn { get; set; }

        bool IsOldList { get; set; }
        PatientRegistration RegistrationObj { get; set; }
        void NotifyOfCanApplyIsOnPriceDiscount();
        RegistrationViewCase ViewCase { get; set; }
    }
}