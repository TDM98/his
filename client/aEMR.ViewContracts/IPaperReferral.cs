using System.Collections.ObjectModel;
using DataEntities;
using aEMR.Common;

namespace aEMR.ViewContracts
{
    public interface IPaperReferral
    {
        IHospitalAutoCompleteListing HospitalAutoCompleteContent { get; set; }
        HealthInsurance CurrentHiItem { get; set; }
        bool IsLoading { get; set; }
        bool IsSaving { get; set; }
        bool IsChildWindow { get; set; }
        int PatientFindBy { get; set; }
        ObservableCollection<PaperReferal> PaperReferals { get; set; }
        bool InfoHasChanged { get; set; }
        void CancelEditing();
        /// <summary>
        /// Giấy chuyển viện được sử dụng để confirm
        /// </summary>
        PaperReferal PaperReferalInUse { get; set; }
/*TMA*/
        // Thông tin bệnh nhân
        PatientRegistration PtRegistration { get; set; }
        Patient CurrentPatient { get; set; }
        IPatientDetails PatientDetailsContent { get; set; }
/*TMA*/
        //FormOperation Operation { get; set; }

        /// <summary>
        /// Dùng biến này để lock form Giay Chuyen Vien từ bên ngoài.
        /// </summary>
        bool FormLocked { get; set; }
        bool CanEdit { get; set; }
    }
}
