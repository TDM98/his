using aEMR.Infrastructure.Events;
using DataEntities;

/*
 * 20180913 TTM #001: Thêm mới hàm để nhận dữ liệu từ ViewModel cha và thêm cờ để xác định xem nó có phải là Dialog hay không
 */

namespace aEMR.ViewContracts
{
    public interface IPhieuTomTat_QuaTrinhDieuTri
    {
        //        int V_TransferFormType { get; set; }
        TreatmentProcess CurrentTreatmentProcess { get; set; }
        void SetCurrentTreatmentProcess(TreatmentProcess item);
        ///*TMA*/
        HealthInsurance CurrentHiItem { get; set; }
        //        PaperReferal ConfirmedPaperReferal { get; set; }
        Patient CurrentPatient { get; set; }
        PatientRegistration PtRegistration { get; set; }

        //        bool V_GetPaperReferalFullFromOtherView { get; set; }
        //        //IPatientDetails PatientDetailsContent { get; set; }
        //        /*TMA*/
        //        //▼====== #001
        bool IsThisViewDialog { get; set; }
        void SetCurrentInformation(TreatmentProcessEvent item);
        //        //▲====== #001
    }
}