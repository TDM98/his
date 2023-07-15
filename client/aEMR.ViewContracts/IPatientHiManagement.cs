using System.Collections.ObjectModel;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using eHCMS.Services.Core;
/*
 * 20180926 #001 TTM:   Thêm cờ nếu là kiểm tra online thì sẽ không kiểm tra giấy CV.
 * 20181015 #002 TTM:   Lấy EditingHiItem ra để có thể kiểm tra thẻ online trước khi lưu
 */
namespace aEMR.ViewContracts
{
    /// <summary>
    /// Hiển thị cửa sổ PatientDetails để Thêm BN, Quản lý BH của BN.
    /// </summary>
    public interface IPatientHiManagement
    {
        IHospitalAutoCompleteListing HospitalAutoCompleteContent { get; set; }
        IPaperReferral PaperReferalContent { get; set; }
        //▼====== #001
        bool IsCheckOnline { get; set; }
        //▲====== #001
        //▼====== #002
        HealthInsurance EditingHiItem { get; set; }
        //▲====== #002
        int PatientFindBy { get; set; }
        void CancelEditing();
        bool IsChildWindow { get; set; }


        /// <summary>
        /// //////////////////////////////////////////////////
        /// </summary>
        Patient CurrentPatient { get; set; }

        /// <summary>
        /// Detect Thông tin thẻ bảo hiểm đã thay đổi hay chưa
        /// </summary>
        bool InfoHasChanged { get; set; }

        /// <summary>
        /// Đang lưu thông tin thẻ bảo hiểm
        /// </summary>
        bool IsSaving { get; set; }

        /// <summary>
        /// Đang lấy danh sách thẻ bảo hiểm
        /// </summary>
        bool IsLoading { get; set; }
        /// <summary>
        /// Lấy luôn những thẻ bảo hiểm đã đánh dấu xóa.
        /// </summary>
        bool IncludeDeletedItems { get; set; }

        /// <summary>
        /// Danh sách thẻ bảo hiểm của bệnh nhân hiện tại.
        /// </summary>
        ObservableCollection<HealthInsurance> HealthInsurances { get; set; }

        /// <summary>
        /// Thông tin thẻ bảo hiểm đã được xác nhận
        /// </summary>
        HealthInsurance ConfirmedItem { get; set; }

        /// <summary>
        /// Danh sách các loại thẻ bảo hiểm
        /// </summary>
        ObservableCollection<Lookup> HiCardTypes { get; set; }

        /// <summary>
        /// Co muon xac nhan the bao hiem hay khong
        /// </summary>
        bool ConfirmHealthInsuranceSelected { get; set; }
        /// <summary>
        /// Co quyen chinh sua hay khong
        /// </summary>
        bool CanEdit { get; set; }


        bool CheckValidationAndGetConfirmedItem(bool IsEmergency = false);

        bool mNhanBenhBH_TheBH_ThemMoi { get; set; }
        bool mNhanBenhBH_TheBH_XacNhan { get; set; }
        bool mNhanBenhBH_TheBH_Sua { get; set; }
        bool mNhanBenhBH_TheBH_SuaSauKhiDangKy { get; set; }

        /// <summary>
        /// Co quyen chinh sua hay khong
        /// </summary>
        bool IsMarkAsDeleted
        { get; }

        AllLookupValues.RegistrationType RegistrationType { get; set; }
        HIQRCode QRCode { get; set; }
        bool ShowSaveHIAndConfirmCmd { get; set; }
        long V_ReceiveMethod { get; set; }
    }
}
