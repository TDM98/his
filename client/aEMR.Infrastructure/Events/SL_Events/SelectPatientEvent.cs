using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.ComponentModel.DataAnnotations;
using aEMR.DataContracts;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Printing;
using eHCMS.Services.Core.Base;
using FluentValidation.Results;
//using Ax.ViewContracts.SL;
/*
 * 20170627 #001 CMN: Added event for ConsultingDiagnosys
 * 20170927 #002 CMN: Added DeadReason
 * 20181005 #003 TNHX: [BM0000034] Add Event print PhieuChiDinh at PatientRegistration History
 * 20181119 #004 TTM:   BM 0005257: Tạo mới hàm lấy dữ liệu cho Out Standing Task bệnh nhân nội trú nằm tại khoa.
 * 20181129 #005 TNHX: [BM0005312]: Add Event print PhieuMienGiam at PatientRegistration
 * 20181225 #006 TNHX: [BM0005462] Re-make report PhieuChiDinh
 * 20190520 #007 TNHX: [BM0006874] Add event Print at ConfirmHIView For PrintingReceiptWithDrugBill
 * 20191102 #008 TNHX: [BM 0017411] Add event After Choose MedPrice + PCLPrice
 * 20020908 #009 TNHX: [BM] Add event CallNextTicketQMSEvent
 * 20220321 #010 QTD:  Thêm 1 danh sách BN nhập viện bên OutstandingTask màn hình Tạm ứng
 * 20221020 #011 QTD:  Thêm sự kiện load thông tin BN cho màn hình Tính lại bill viện phí
*/
namespace aEMR.Infrastructure.Events
{

    public class CreateNewPatientEvent
    {
        public string FullName { get; set; }
        public long OrderNumber { get; set; }
        public DateTime ServiceStartedAt { get; set; }
    }

    public  class ValidateFailedEvent
    {
        public ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> ValidationResults { get; set; }
    }
    public class ShowMessageEvent
    {
        public string Message { get; set; }
    }
    /// <summary>
    /// Sự kiện thông báo chưa có thẻ bảo hiểm nào được activate.
    /// </summary>
    public  class ActiveHiMissingEvent
    {
        public string Message { get; set; }
    }

    /// <summary>
    /// Sự kiện thông báo chưa có bệnh nhân, hoặc chưa tìm bệnh nhân để bắt đầu làm việc.
    /// </summary>
    public class PatientMissingEvent
    {
        public string Message { get; set; }
    }

    /// <summary>
    /// Sự kiện thông báo vừa xác nhận một thẻ bảo hiểm hợp lệ (hoặc bỏ một thẻ bảo hiểm đã confirm) xong.
    /// </summary>
    public class HiCardConfirmedEvent
    {
        public HealthInsurance HiProfile;
        public PaperReferal PaperReferal;
        public object Source;
        public bool IsEmergency = false;
        public bool IsEmergInPtReExamination = false;
        public bool IsHICard_FiveYearsCont = false;
        public bool IsChildUnder6YearsOld = false;
        public bool IsAllowCrossRegion = false;
        public bool IsHICard_FiveYearsCont_NoPaid = false;
        public DateTime? FiveYearsAppliedDate = null;
        public DateTime? FiveYearsARowDate = null;
        public long V_ReceiveMethod;
    }
    public class HiCardConfirmCompleted
    {
        public string RegistrationCode;
        public string HICardNo;
        public string HIPCode;
        public long KVCode;
    }

    public class HiCardReloadEvent
    {
        //public HealthInsurance HiProfile;
        //public PaperReferal PaperReferal;
        //public object Source;
    }

    public class PaperReferalDeleteEvent
    {
        
    }

    public class RemoveConfirmedHiCard
    {
        public long HiId;
        public object Source;
    }
    ///<sumary>
    /// Thay đổi thông tin bảo hiểm dịch vụ đã trả tiền rồi
    /// </sumary>
    public class ChangeHIStatus<T> where T : NotifyChangedBase
    {
        public T Item { get; set; }
    }
    ///<sumary>
    /// Thay đổi thông tin bảo hiểm cận lâm sàng đã trả tiền rồi
    /// </sumary>
    public class ChangePCLHIStatus<T> where T : NotifyChangedBase
    {
        public T Item { get; set; }
    }
    ///<sumary>
    /// Thay đổi thông tin miễn giảm dịch vụ đã trả tiền rồi
    /// </sumary>
    public class ChangeDiscountStatus<T> where T : NotifyChangedBase
    {
        public T Item { get; set; }
    }
    /// <summary>
    /// Thông báo đóng view của danh sách bảo hiểm.
    /// </summary>
    public class CloseHiManagementView
    {}
    /// <summary>
    /// Su kien xay ra khi bat dau remove item
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RemoveItem<T> where T:NotifyChangedBase
    {
        public T Item { get; set; }
    }

    
    public class StateChanged<T> where T : NotifyChangedBase
    {
        public T Item { get; set; }
    }


    public class RemoveItem<T,S> where T : NotifyChangedBase
    {
        public T Item { get; set; }
        public S Source { get; set; }
    }

    /// <summary>
    /// Tra lai 1 item nao do.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="S"></typeparam>
    public class ReturnItem<T, S> where T : NotifyChangedBase
    {
        public T Item { get; set; }
        public S Source { get; set; }
    }
    public enum ActiveXPrintType
    {
        ByteArray = 0,
        Base64String = 1,
        FileName = 2
    }
    /// <summary>
    /// Sự kiện gửi đi khi có yêu cầu in.
    /// </summary>
    /// <typeparam name="S"></typeparam>
    public class ActiveXPrintEvt
    {
        public ActiveXPrintEvt()
        {}
        public ActiveXPrintEvt(object source,PrinterType printerType, object data, ActiveXPrintType dataType, string paperName = null)
        {
            Source = source;
            PrinterType = printerType;
            Data = data;
            DataType = dataType;
            PaperName = paperName;
    }

        public ActiveXPrintEvt(object source, PrinterType printerType, object data, ActiveXPrintType dataType, int numOfCopies, string paperName = null)
        {
            Source = source;
            PrinterType = printerType;
            Data = data;
            DataType = dataType;
            NumberOfCopies = numOfCopies;
            PaperName = paperName;
        }

        public object Source { get; set; }
        /// <summary>
        /// Dữ liệu in.
        /// Tùy theo biến Type
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Loại dữ liệu in. VD: Tên file, Base64String, hay là byte array.
        /// </summary>
        public ActiveXPrintType DataType;

        /// <summary>
        /// Loại máy in. VD: Máy in hóa đơn, máy in phiếu dịch vụ ...
        /// </summary>
        public PrinterType PrinterType;

        /// <summary>
        /// Số lần in.
        /// </summary>
        public int NumberOfCopies;

        /// <summary>
        /// Loai Giay cua Report (A4, A5, v.v)
        /// </summary>
        public string PaperName;
    }

    public class LocationSelected
    {
        public RefDepartment RefDepartment
        {
            get;
            set;
        }
        public DeptLocation DeptLocation
        {
            get;
            set;
        }

        public object ItemActivated { get; set; }
    }

    public class LocationSelected_New
    {
        public RefDepartment RefDepartment
        {
            get;
            set;
        }
        public DeptLocation DeptLocation
        {
            get;
            set;
        }

        public object ItemActivated { get; set; }
    }

    public class AuthorizationEvent
    {
        
    }

    public class PayForRegistrationCompleted
    {
        public PatientTransactionPayment Payment { get; set; }
        public List<PaymentAndReceipt> PaymentList { get; set; }
        public PatientRegistration Registration { get; set; }

        /// <summary>
        /// Gui them thong tin phu.
        /// </summary>
        public object ObjectState { get; set; }
        public bool IsInPatientRegistration { get; set; }
        public List<PatientRegistrationDetail> RegDetailsList { get; set; }
        public List<PatientPCLRequest> PCLRequestList { get; set; }

        public bool RefreshItemFromReturnedObj { get; set; } = false;

    }

    public class PayForRegistrationCompletedAtConfirmHIView
    {
        public PatientTransactionPayment Payment { get; set; }
        public List<PaymentAndReceipt> PaymentList { get; set; }
        public PatientRegistration Registration { get; set; }
        public object ObjectState { get; set; }
        public bool IsInPatientRegistration { get; set; }
        public List<PatientRegistrationDetail> RegDetailsList { get; set; }
        public List<PatientPCLRequest> PCLRequestList { get; set; }
        public bool RefreshItemFromReturnedObj { get; set; } = false;
    }

    public class SaveAndPayForRegistrationCompleted
    {
        public PatientTransactionPayment Payment { get; set; }
        public List<PaymentAndReceipt> PaymentList { get; set; }
        public PatientRegistration RegistrationInfo { get; set; }
    }

    public class SaveRegisFromSimplePayCompleted
    {
        public PatientRegistration RegistrationInfo { get; set; }
    }

    public class LoadInPatientBillingInvoice
    {
    }

    /// <summary>
    /// Sự kiện đánh dấu load được 1 item kiểu T, có ID kiểu I
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="I"></typeparam>
    public class ItemLoaded<T,I>
    {
        public T Item { get; set; }
        public I ID { get; set; }
    }
    public class ResultFound<T>
    {
        public T Result { get; set; }
        public object SearchCriteria { get; set; }
    }

    public class ResultNotFound<T>
    {
        public string Message { get; set; }
        public object SearchCriteria { get; set; }
        public long OrderNumber { get; set; }
    }

    public class ResultNotFoundAppt<T>
    {
        public string Message { get; set; }
        public object SearchCriteria { get; set; }
    }
    /// <summary>
    /// Cập nhật một đối tượng xong.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UpdateCompleted<T>
    {
        public T Item { get; set; }
        public bool bUpdate_CurrentPatient_Info_Only = false;
    }

    public class SavePatientDetailsAndHI_OKEvent
    {
        public Patient theSavedPatient { get; set; }
        public bool IsChildUnder6YearsOld { get; set; }
        public bool IsHICard_FiveYearsCont { get; set; }
        public bool EmergInPtReExamination { get; set;}
        public double CalculatedHiBenefit { get; set; }
    }

    public class AddCompleted<T>
    {
        public T Item { get; set; }
        private bool _RefreshItemFromReturnedObj = false;
        public bool RefreshItemFromReturnedObj
        {
            get { return _RefreshItemFromReturnedObj; }
            set { _RefreshItemFromReturnedObj = value; }
        }
    }

    public class PaperReferalAttached
    {
        public PaperReferal Item { get; set; }
        public long PatientID { get; set; }
    }
    public class PaperReferalEdited
    {
        public PaperReferalEdited()
        {}
        public PaperReferalEdited(PaperReferal source)
        {
            _source = source;
        }

        private PaperReferal _source;
        public PaperReferal SourceObject
        {
            get { return _source; }
        }
        public PaperReferal Item { get; set; }
        public long PatientID { get; set; }
    }
    public class ViewModelClosing<T>
    {
        public T ViewModel;
    }
    //public class PaperReferalConfirmedEvent
    //{
    //    public PaperReferal Item { get; set; }
    //}

    public class DoubleClick
    {
        public object Source { get; set; }
        public EventArgs<object> EventArgs { get; set; }
    }


    public class DoubleClickAddReqLAB
    {
        public object Source { get; set; }
        public EventArgs<object> EventArgs { get; set; }
    }

    public class ItemEdited<T>
    {
        public object Source { get; set; }
        public T Item { get; set; }
        public bool Cancel { get; set; }
    }

    public class ItemSelected<T>
    {
        public object Source { get; set; }
        public T Item { get; set; }
        public bool Cancel { get; set; }
    }

    public class RegisObjAndPrescriptionLst
    {
        public PatientRegistration RegisObj { get; set; }
        public IList<Prescription> PrescriptLst { get; set; }
    }
    public class ItemPatient<T>
    {
        public object Source { get; set; }
        public T Item { get; set; }
        public bool Cancel { get; set; }
    }
    public class ItemPatient1<T>
    { 
        public object Source { get; set; }
        public T Item { get; set; }
        public bool Cancel { get; set; }
    }
    /*▼====: #001*/
    public class ConsultingDiagnosys<T>
    {
        public T Item { get; set; }
    }
    /*▲====: #001*/

    public class RegDetailSelectedForConsultation
    {
        public PatientRegistrationDetail Source { get; set; }
    }

    public class RegDetailSelectedForProcess
    {
        public PatientRegistrationDetail RegDetail { get; set; }
    }

    public class RegDetailFromOutStandingTask
    {
        public PatientRegistrationDetail Source { get; set; }
    }

    public class ItemSelected<S,T>
    {
        public S Sender { get; set; }
        public T Item { get; set; }
    }


    public class PatientSelectedGoToKhamBenh_InPt<TPReg>
    {
        public TPReg Item { get; set; }
    }

    public class RegistrationSelectedToTransfer
    {
        public PatientRegistration PtRegistration { get; set; }
        public AllLookupValues.PatientFindBy PatientFindBy { get; set; }

    }

    public class RegistrationSelectedToHoiChan
    {
        public PatientRegistration PtRegistration { get; set; }
    }

    public class ItemSelecting<S, T>
    {
        public S Sender { get; set; }
        public T Item { get; set; }
        public bool Cancel { get; set; }
    }

    #region Sau khi tìm được đăng ký để khám bệnh thì bắn event này
    //KMx: Nếu tìm thấy 1 ĐK thì bắn event này (07/10/2014 15:22).
    public class RegistrationSelectedForConsultation_K1
    {
        public PatientRegistration Source { get; set; }
        private bool _IsSearchByRegistrationDetails = false;
        public bool IsSearchByRegistrationDetails { get => _IsSearchByRegistrationDetails; set => _IsSearchByRegistrationDetails = value; }
    }

    //KMx: Nếu tìm thấy nhiều hơn 1 ĐK và double click vào 1 đăng ký thì bắn event này (07/10/2014 15:22).
    public class RegistrationSelectedForConsultation_K2
    {
        public PatientRegistration Source { get; set; }
        private bool _IsSearchByRegistrationDetails = false;
        public bool IsSearchByRegistrationDetails { get => _IsSearchByRegistrationDetails; set => _IsSearchByRegistrationDetails = value; }
    }
    #endregion
    //▼====== #004
    #region Tạo sự kiện để bắn từ out standing task tìm bệnh nhân nội trú nằm tại khoa vào các màn hình tương ứng
    //Sự kiện load lại thông tin bệnh nhân chung ở khám bệnh (ConsultationModule)
    public class InPatientRegistrationSelectedForConsultation
    {
        public PatientRegistration Source { get; set; }
    }

    //Sự kiện load lại thông tin bệnh nhân cho màn hình chẩn đoán
    public class SetInPatientInfoAndRegistrationForConsultations_InPt
    {
        public long MedServiceID { get; set; }
    }

    //Sự kiện load lại thông tin bệnh nhân cho màn hình ra toa xuất viện
    public class SetInPatientInfoAndRegistrationForePresciption_InPt { }

    //Sự kiện load lại thông tin bệnh nhân cho màn hình chỉ định xét nghiệm
    public class SetInPatientInfoAndRegistrationForPatientPCLRequest { }

    //Sự kiện load lại thông tin bệnh nhân cho màn hình chỉ định cls hình ảnh
    public class SetInPatientInfoAndRegistrationForPatientPCLRequestImage { }

    //Sự kiện load lại thông tin bệnh nhân cho màn hình xuất viện (Bác sĩ)
    public class SetInPatientInfoAndRegistrationForInPatientDischarge { }

    //Sự kiện load lại thông tin bệnh nhân cho màn hình Quản lý bệnh nhân nội trú
    public class InPatientRegistrationSelectedForInPatientAdmission
    {
        public PatientRegistration Source { get; set; }
    }
    public class InPatientSelectedForInPatientAdmission
    {
        public PatientRegistration Source { get; set; }
    }

    //Sự kiện load lại thông tin bệnh nhân cho màn hình Tạo bill viện phí
    public class InPatientRegistrationSelectedForInPatientRegistration
    {
        public PatientRegistration Source { get; set; }
    }

    //Sự kiện load lại thông tin bệnh nhân cho màn hình Xuất thuốc cho bệnh nhân (y cụ, hóa chất khoa nội trú)
    public class InPatientRegistrationSelectedForOutwardToPatient_V2
    {
        public PatientRegistration Source { get; set; }
    }

    //Sự kiện load lại thông tin bệnh nhân cho màn hình Thanh toán
    public class InPatientRegistrationSelectedForInPatientPayment
    {
        public PatientRegistration Source { get; set; }
    }

    //Sự kiện load lại thông tin bệnh nhân cho màn hình điều trị nhiễm khuẩn
    public class InPatientRegistrationSelectedForInfectionCase
    {
        public PatientRegistration Source { get; set; }
    }
    //Sự kiện load lại thông tin bệnh nhân cho màn hình xác nhận lại BHYT
    public class InPatientRegistrationSelectedForConfirmHI
    {
        public PatientRegistration Source { get; set; }
    }
    #endregion
    //▲====== #004

    public class SetBasicDiagTreatmentForRegistrationSummaryV2 { }
    public class SetRefByStaffForRegistrationSummaryV2 { }
    public class SetCurRegistrationForPatientSummaryInfoV3 { }

    #region InPatientInstruction

    public class RegistrationSelectedForInPtInstruction_1
    {
        public PatientRegistration PtRegistration { get; set; }
    }

    public class RegistrationSelectedForInPtInstruction_2
    {
        public PatientRegistration PtRegistration { get; set; }
    }

    #endregion

    /// <summary>
    /// Su kien xay ra khi mot item da duoc remove
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ItemRemoved<T>
    {
        public T Item { get; set; }
    }

    public class ItemRemoved<T,S>
    {
        public T Item { get; set; }
        public S Source { get; set; }
    }

    public class ItemChanged<T, S>
    {
        public T Item { get; set; }
        public S Source { get; set; }
    }

    public class ErrorNotification
    {
        public string Message { get; set; }
    }
    public class ErrorOccurred
    {
        public AxErrorEventArgs CurrentError{ get; set; }
    }

    public class ErrorBoldOccurred
    {
        public string message {get; set;}
        public string checkBoxContent { get; set; }
    }

    public class  ItemSaved<T>
    {
        public T Item { get; set; }
    }
    
    
    /// <summary>
    /// Chon mot item de bat dau edit.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EditItem<T>
    {
        public T Item { get; set; }
    }
    /// <summary>
    /// Tim dang ky la noi tru hay ngoai tru
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PatientFindByChange
    {
        public AllLookupValues.PatientFindBy patientFindBy { get; set; }
    }

    public class InPatientReturnMedProduct
    {
    }

    public class ConfirmHiBenefit
    {
        public float? HiBenefit
        {
            get;
            set;
        }
        public bool? IsCrossRegion
        {
            get;
            set;
        }
        public long PatientId { get; set; }
        public long HiId { get; set; }
        public string HIComment { get; set; }
    }

    public class ConfirmHiBenefitEvent
    {
        public ConfirmHiBenefitEnum confirmHiBenefitEnum { get; set; }
        public float? HiBenefit
        {
            get;
            set;
        }
        public bool? IsCrossRegion
        {
            get;
            set;
        }
        public long PatientId { get; set; }
        public long HiId { get; set; }
        public string HIComment { get; set; }

        public int Selected_OJR_Option { get; set; }       // Selected O-J-R Option
                                                    // 1 => IsConfirmReplaceWithAnotherCard 
                                                    // 2 => IsConfirmJoiningWithNewCard 
                                                    // 3 => IsConfirmRemoveLastAddedCard 

    }

    public class NoChangeConfirmHiBenefit
    {
        public float? HiBenefit
        {
            get;
            set;
        }
        public bool? IsCrossRegion
        {
            get;
            set;
        }
        public long PatientId { get; set; }
        public long HiId { get; set; }
        public string HIComment { get; set; }
    }

    public class PatientChange
    {
        
    }

    public class PatientReloadEvent
    {
        public Patient curPatient { get; set; }
    }

    public class WarningConfirmMsgBoxClose
    {
        public bool IsConfirmed
        {
            get;
            set;
        }
    }

    public class BookingBedForAcceptChangeDeptEvent
    {
        public BedPatientAllocs SelectedBedPatientAlloc { get; set; }
    }

    public class CloseBedPatientAllocViewEvent
    {
    }

    public class OutDepartmentSuccessEvent
    {
    }

    public class ReloadRegisAfterRecalcBillInvoiceEvent
    {
        public InPatientBillingInvoice Item { get; set; }
    }

    //▼====: #008
    public class SelectPriceListForRecalcBillInvoiceEvent
    {
        public long MedServiceItemPriceListID { get; set; }
        public long PCLExamTypePriceListID { get; set; }
        public bool DoRecalcHiWithPriceList { get; set; }
    }
    //▲====: #008

    public class SelectedRegistrationForTemp02_1
    {
        public PatientRegistration Item { get; set; }
    }

    public class SelectedRegistrationForTemp02_2
    {
        public PatientRegistration Item { get; set; }
    }

    public class LoadBillingDetailsCompleted
    { }
    //▼====: #002
    public class DischargeConditionChange
    {
        public AllLookupValues.DischargeCondition Item { get; set; }
    }
    //▲====: #002
    //▼====: #006
    //▼====: #003
    public class PhieuChiDinhForRegistrationCompleted
    {
        public PatientRegistration Registration { get; set; }
        public List<PatientRegistrationDetail> RegDetailsList { get; set; }
        public List<PatientPCLRequest> PCLRequestList { get; set; }
    }
    //▲====: #003
    //▲====: #006
    //▼====: #005
    public class PhieuMienGiamForRegistrationCompleted
    {
        public PatientTransactionPayment Payment { get; set; }
        public List<PaymentAndReceipt> PaymentList { get; set; }
        public PatientRegistration Registration { get; set; }
        public object ObjectState { get; set; }
        public bool IsInPatientRegistration { get; set; }
        public List<PatientRegistrationDetail> RegDetailsList { get; set; }
        public List<PatientPCLRequest> PCLRequestList { get; set; }
    }
    //▲====: #005

    public class SaveHIAndConfirm
    {

    }
    public class SaveHIAndInPtConfirmHICmd
    {

    }
    public class ReloadPtRegistrationCode
    {
        public string PtRegistrationCode { get; set; }
    }
    public class LoadDataForHtmlEditor
    {
        public SmallProcedure SmallProcedure{ get; set; }
    }
    public class SetEkipForServiceSuccess
    {
        public PatientRegistration RegistrationInfo { get; set; }
    }
    public class SelectPresenter
    {
        public Patient PatientInfo { get; set; }
    }
    public class RegDetailSelectedForAppointmentTMV
    {
        public PatientRegistrationDetail Source { get; set; }
    }
    #region Ticket

    public class SetTicketNumnerTextForPatientRegistrationView
    {
        public TicketIssue Item { get; set; }
    }
    public class ResetViewAfterCallNextTicket
    {
    }
    public class SetTicketIssueForPatientRegistrationView
    {
        public TicketIssue Item { get; set; }
        public bool IsLoadPatientInfo { get; set; }
    }
    public class NotifyAddCurHealthInsuranceToQMS
    {
        public HealthInsurance Item { get; set; }
    }

    //▼===== #009
    public class CallNextTicketQMSEvent
    {
    }
    //▲===== #009

    #endregion
    public class SetTicketForNewRegistrationAgain
    {
        public TicketIssue Item { get; set; }
    }

    public class AddAllPackageTechnicalService
    {
        public ObservableCollection<PackageTechnicalServiceDetail> ListpclCombo { get; set; }
    }

    public class SaveExecuteDrugCompleted
    {
    }
    public class CreateNewMedicalServiceGroup
    {
    }

    public class InPatientRegistrationSelectedForInPatientCashAdvance
    {
        public PatientRegistration Source { get; set; }
    }
    //▼===== #011
    public class InPatientRegistrationSelectedForInPatientBillingInvoiceListing
    {
        public PatientRegistration Source { get; set; }
    }
    //▲===== #011
    public class BasicDiagTreatmentChanged
    {
        public DiseasesReference aICD10Code { get; set; }
    }

    public class CalcPaymentCeilingForTechServiceEvent
    {
        public decimal PaymentCeilingForTechService { get; set; }
    }
}
