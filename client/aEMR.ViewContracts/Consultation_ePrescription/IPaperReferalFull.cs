using aEMR.Infrastructure.Events;
using DataEntities;

/*
 * 20180913 TTM #001: Thêm mới hàm để nhận dữ liệu từ ViewModel cha và thêm cờ để xác định xem nó có phải là Dialog hay không
 * 20230516 DatTB #002: Thêm biến để Khóa/mở các chức năng lưu xóa giấy chuyển tuyến
 */

namespace aEMR.ViewContracts
{
    public interface IPaperReferalFull
    {
        int V_TransferFormType { get; set; }
        TransferForm CurrentTransferForm { get; set; }
        void SetCurrentTransferForm(TransferForm item);
/*TMA*/
        HealthInsurance CurrentHiItem { get; set; }
        PaperReferal ConfirmedPaperReferal { get; set; }
        Patient CurrentPatient { get; set; }
        PatientRegistration PtRegistration { get; set; }

        bool V_GetPaperReferalFullFromOtherView { get; set; }
        //IPatientDetails PatientDetailsContent { get; set; }
        /*TMA*/
        //▼====== #001
        bool IsThisViewDialog { get; set; }
        void SetCurrentInformation(TransferFormEvent item);
        //▲====== #001
        //▼==== #002
        bool CanEditTransferForm { get; set; }
        bool IsHiReportOrDischarge { get; set; }
        //▲==== #002
    }
}