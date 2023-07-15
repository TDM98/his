using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using eHCMS.Configurations;
/*
* 20170522 #001 CMN: Added variable to check InPt 5 year HI without paid enough
* 20180102 #002 CMN: Added properties for 4210 file
* 20180815 #003 TBL: Added BNTKSauXV
* 20181212 #004 TTM: Added BasicDiagreatment
* 20200111 #005 TTM: BM 0021792: Thay đổi thông tin miễn giảm => Chỉ được miễn giảm tối đa tiền bệnh nhân chi trả không được phép miễn giảm tiền bệnh nhân cùng chi trả.
* 20200212 #006 TTM: BM 0023912: Fix lỗi khi lưu (Chưa trả tiền) tích BH thay đổi => giá trị của dòng bị sai (Dịch vụ).
* 20200928 #007 TNHX: BM : Thêm điều kiện bỏ qua tiền BN cùng chi trả
* 20210323 #008 BLQ:  243  Lấy V_CheckMedicalFilesStatus theo đăng ký
* 20210528 #009 BLQ:  Thêm trường đã trả tiền thuốc ngoại trú chưa 
* 20220331 #010 DatTB:  Lấy thêm thông tin DLS trả hồ sơ.
* 20220604 #011 QTD:    Kiểm tra đăng ký có chuyển nội trú chưa
* 20220620 #012 QTD:    Kiểm tra đăng ký khám sức khoẻ
* 20220801 #004 DatTB: Lấy thêm ID đề nghị chuyển khoa để chặn tạo y lệnh khi có đề nghị
* 20221210 #014 TNHX: 994 Thêm trường DTDTReportID 
* 20230213 #015 QTD:  Thêm trường trạng thái DTDT Nhà thuốc
* 20230410 #016 TNHX:  3034 Thêm trường trạng thái bảng tự động đẩy cổng BHYT
*/
namespace DataEntities
{
    public class PatientHI_SummaryInfo
    {
        public bool IsCrossRegion { get; set; }

        public HealthInsurance ConfirmedHiItem { get; set; }
        public HealthInsurance ConfirmedHiItem_2 { get; set; }
        public HealthInsurance ConfirmedHiItem_3 { get; set; }

        public PaperReferal ConfirmedPaperReferal { get; set; }
        public PaperReferal ConfirmedPaperReferral_2 { get; set; }
        public PaperReferal ConfirmedPaperReferral_3 { get; set; }

        public double? HiBenefit { get; set; }
        public double? HiBenefit_2 { get; set; }
        public double? HiBenefit_3 { get; set; }
    }

    [DataContract]
    public partial class PatientRegistration : NotifyChangedBase
    {
        public PatientRegistration() : base()
        {
            _PatientRegistrationDetails = new ObservableCollection<PatientRegistrationDetail>();
            //ExamDate = DateTime.Now;
            _RegistrationStatus = AllLookupValues.RegistrationStatus.OPENED;
            _RegistrationPaymentStatus = AllLookupValues.RegistrationPaymentStatus.DEBIT;
        }
        #region Factory Method


        /// Create a new PatientRegistration object.

        /// <param name="ptRegistrationID">Initial value of the PtRegistrationID property.</param>
        /// <param name="examDate">Initial value of the ExamDate property.</param>
        public static PatientRegistration CreatePatientRegistration(Int64 ptRegistrationID, DateTime examDate)
        {
            PatientRegistration patientRegistration = new PatientRegistration();
            patientRegistration.PtRegistrationID = ptRegistrationID;
            patientRegistration.ExamDate = examDate;
            patientRegistration.PatientRegistrationDetails = new ObservableCollection<PatientRegistrationDetail>();
            return patientRegistration;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                if (_PtRegistrationID != value)
                {
                    OnPtRegistrationIDChanging(value);
                    _PtRegistrationID = value;
                    RaisePropertyChanged("PtRegistrationID");
                    OnPtRegistrationIDChanged();
                }
            }
        }
        private Int64 _PtRegistrationID;
        partial void OnPtRegistrationIDChanging(Int64 value);
        partial void OnPtRegistrationIDChanged();

        [DataMemberAttribute()]
        public string PtRegistrationCode
        {
            get
            {
                return _PtRegistrationCode;
            }
            set
            {
                if (_PtRegistrationCode != value)
                {
                    _PtRegistrationCode = value;
                    RaisePropertyChanged("PtRegistrationCode");
                }
            }
        }
        private string _PtRegistrationCode;

        [DataMemberAttribute()]
        public Nullable<Byte> RegTypeID
        {
            get
            {
                return _RegTypeID;
            }
            set
            {
                OnRegTypeIDChanging(value);
                _RegTypeID = value;
                RaisePropertyChanged("RegTypeID");
                OnRegTypeIDChanged();
            }
        }
        private Nullable<Byte> _RegTypeID;
        partial void OnRegTypeIDChanging(Nullable<Byte> value);
        partial void OnRegTypeIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> DeptID
        {
            get
            {
                return _DeptID;
            }
            set
            {
                OnDeptIDChanging(value);
                _DeptID = value;
                RaisePropertyChanged("DeptID");
                OnDeptIDChanged();
            }
        }
        private Nullable<long> _DeptID;
        partial void OnDeptIDChanging(Nullable<long> value);
        partial void OnDeptIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                OnPatientIDChanging(value);
                _PatientID = value;
                RaisePropertyChanged("PatientID");
                OnPatientIDChanged();
            }
        }
        private Nullable<long> _PatientID;
        partial void OnPatientIDChanging(Nullable<long> value);
        partial void OnPatientIDChanged();

        [DataMemberAttribute()]
        public int CheckTransation
        {
            get
            {
                return _CheckTransation;
            }
            set
            {
                OnCheckTransationChanging(value);
                _CheckTransation = value;
                RaisePropertyChanged("CheckTransation");
                OnCheckTransationChanged();
            }
        }
        private int _CheckTransation = 0;
        partial void OnCheckTransationChanging(int value);
        partial void OnCheckTransationChanged();
        #region properties add on

        [DataMemberAttribute()]
        public String FullName
        {
            get
            {
                return _FullName;
            }
            set
            {
                OnFullNameChanging(value);
                _FullName = value;
                RaisePropertyChanged("FullName");
                OnFullNameChanged();
            }
        }
        private String _FullName;
        partial void OnFullNameChanging(String value);
        partial void OnFullNameChanged();

        [DataMemberAttribute()]
        public String PatientCode
        {
            get
            {
                return _PatientCode;
            }
            set
            {
                OnPatientCodeChanging(value);
                _PatientCode = value;
                RaisePropertyChanged("PatientCode");
                OnPatientCodeChanged();
            }
        }
        private String _PatientCode;
        partial void OnPatientCodeChanging(String value);
        partial void OnPatientCodeChanged();

        #endregion

        [DataMemberAttribute()]
        public Nullable<long> EmergRecID
        {
            get
            {
                return _EmergRecID;
            }
            set
            {
                OnEmergRecIDChanging(value);
                _EmergRecID = value;
                RaisePropertyChanged("EmergRecID");
                OnEmergRecIDChanged();
            }
        }
        private Nullable<long> _EmergRecID;
        partial void OnEmergRecIDChanging(Nullable<long> value);
        partial void OnEmergRecIDChanged();


        [DataMemberAttribute()]
        public Nullable<bool> IsForeigner
        {
            get
            {
                return _IsForeigner;
            }
            set
            {
                OnIsForeignerChanging(value);
                _IsForeigner = value;
                RaisePropertyChanged("IsForeigner");
                OnIsForeignerChanged();
            }
        }
        private Nullable<bool> _IsForeigner;
        partial void OnIsForeignerChanging(Nullable<bool> value);
        partial void OnIsForeignerChanged();


        [DataMemberAttribute()]
        public Nullable<bool> EmergInPtReExamination
        {
            get
            {
                return _EmergInPtReExamination;
            }
            set
            {
                OnEmergInPtReExaminationChanging(value);
                _EmergInPtReExamination = value;
                RaisePropertyChanged("EmergInPtReExamination");
                OnEmergInPtReExaminationChanged();
            }
        }
        private Nullable<bool> _EmergInPtReExamination;
        partial void OnEmergInPtReExaminationChanging(Nullable<bool> value);
        partial void OnEmergInPtReExaminationChanged();


        [DataMemberAttribute()]
        public Nullable<Int64> StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                OnStaffIDChanging(value);
                _StaffID = value;
                RaisePropertyChanged("StaffID");
                OnStaffIDChanged();
            }
        }
        private Nullable<Int64> _StaffID;
        partial void OnStaffIDChanging(Nullable<Int64> value);
        partial void OnStaffIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> HIApprovedStaffID
        {
            get
            {
                return _HIApprovedStaffID;
            }
            set
            {
                OnHIApprovedStaffIDChanging(value);
                _HIApprovedStaffID = value;
                RaisePropertyChanged("HIApprovedStaffID");
                OnHIApprovedStaffIDChanged();
            }
        }
        private Nullable<Int64> _HIApprovedStaffID;
        partial void OnHIApprovedStaffIDChanging(Nullable<Int64> value);
        partial void OnHIApprovedStaffIDChanged();

        [DataMemberAttribute()]
        public DateTime ExamDate
        {
            get
            {
                return _ExamDate;
            }
            set
            {
                OnExamDateChanging(value);
                _ExamDate = value;
                RaisePropertyChanged("ExamDate");
                OnExamDateChanged();
            }
        }
        private DateTime _ExamDate;
        partial void OnExamDateChanging(DateTime value);
        partial void OnExamDateChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> V_DocumentTypeOnHold
        {
            get
            {
                return _V_DocumentTypeOnHold;
            }
            set
            {
                OnV_DocumentTypeOnHoldChanging(value);
                _V_DocumentTypeOnHold = value;
                RaisePropertyChanged("V_DocumentTypeOnHold");
                OnV_DocumentTypeOnHoldChanged();
            }
        }
        private Nullable<Int64> _V_DocumentTypeOnHold;
        partial void OnV_DocumentTypeOnHoldChanging(Nullable<Int64> value);
        partial void OnV_DocumentTypeOnHoldChanged();

        #endregion

        private long? _PatientClassID;
        //Dang ky cho loai benh nhan nao (thong thuong, bao hiem)
        [DataMemberAttribute()]
        public long? PatientClassID
        {
            get
            {
                return _PatientClassID;
            }
            set
            {
                _PatientClassID = value;
                RaisePropertyChanged("PatientClassID");
            }
        }

        private PatientClassification _PatientClassification;
        [DataMemberAttribute()]
        public PatientClassification PatientClassification
        {
            get
            {
                return _PatientClassification;
            }
            set
            {
                _PatientClassification = value;
                RaisePropertyChanged("PatientClassification");
            }
        }
        //HPT 19/08/2015 Begin: Thêm thuộc tính kiểm tra thẻ bảo hiểm hưởng quyền lợi dành cho bệnh nhân có thẻ bảo hiểm 5 năm liên tiếp
        private bool _IsHICard_FiveYearsCont;
        [DataMemberAttribute()]
        public bool IsHICard_FiveYearsCont
        {
            get
            {
                return _IsHICard_FiveYearsCont;
            }
            set
            {
                _IsHICard_FiveYearsCont = value;
                RaisePropertyChanged("IsHICard_FiveYearsCont");
            }
        }

        //▼====: #001
        private bool _IsHICard_FiveYearsCont_NoPaid = false;
        [DataMemberAttribute()]
        public bool IsHICard_FiveYearsCont_NoPaid
        {
            get
            {
                return _IsHICard_FiveYearsCont_NoPaid;
            }
            set
            {
                _IsHICard_FiveYearsCont_NoPaid = value;
                RaisePropertyChanged("IsHICard_FiveYearsCont_NoPaid");
            }
        }
        //▲====: #001

        //▼====: #003
        private DateTime? _FiveYearsAppliedDate;
        [DataMemberAttribute()]
        public DateTime? FiveYearsAppliedDate
        {
            get
            {
                return _FiveYearsAppliedDate;
            }
            set
            {
                _FiveYearsAppliedDate = value;
                RaisePropertyChanged("FiveYearsAppliedDate");
            }
        }
        //▲====: #003
        private DateTime? _FiveYearsARowDate;
        [DataMemberAttribute()]
        public DateTime? FiveYearsARowDate
        {
            get
            {
                return _FiveYearsARowDate;
            }
            set
            {
                _FiveYearsARowDate = value;
                RaisePropertyChanged("FiveYearsARowDate");
            }
        }

        //HPT 04/12/2015 Begin: Thêm thuộc tính kiểm tra thẻ bảo hiểm hưởng quyền lợi dành cho trẻ em thành phố dưới 6 tuổi có thẻ BHYT
        private bool _IsChildUnder6YearsOld = false;
        [DataMemberAttribute()]
        public bool IsChildUnder6YearsOld
        {
            get
            {
                return _IsChildUnder6YearsOld;
            }
            set
            {
                _IsChildUnder6YearsOld = value;
                RaisePropertyChanged("IsChildUnder6YearsOld");
            }
        }

        private bool _IsAllowCrossRegion = false;
        [DataMemberAttribute()]
        public bool IsAllowCrossRegion
        {
            get
            {
                return _IsAllowCrossRegion;
            }
            set
            {
                _IsAllowCrossRegion = value;
                RaisePropertyChanged("IsAllowCrossRegion");
            }
        }

        private double? _PtInsuranceBenefit;
        [DataMemberAttribute()]
        public double? PtInsuranceBenefit
        {
            get
            {
                return _PtInsuranceBenefit;
            }
            set
            {
                _PtInsuranceBenefit = value;
                RaisePropertyChanged("PtInsuranceBenefit");
            }
        }

        private bool? _isCrossRegion;
        [DataMemberAttribute()]
        public bool? IsCrossRegion
        {
            get
            {
                return _isCrossRegion;
            }
            set
            {
                _isCrossRegion = value;
                RaisePropertyChanged("IsCrossRegion");
            }
        }

        private long? _HisID;
        [DataMemberAttribute()]
        public long? HisID
        {
            get
            {
                return _HisID;
            }
            set
            {
                _HisID = value;
                RaisePropertyChanged("HisID");
            }
        }

        /*▼====: #002*/
        private long? _HisID_2;
        [DataMemberAttribute()]
        public long? HisID_2
        {
            get
            {
                return _HisID_2;
            }
            set
            {
                _HisID_2 = value;
                RaisePropertyChanged("HisID_2");
            }
        }

        private double? _PtInsuranceBenefit_2;
        [DataMemberAttribute()]
        public double? PtInsuranceBenefit_2
        {
            get
            {
                return _PtInsuranceBenefit_2;
            }
            set
            {
                _PtInsuranceBenefit_2 = value;
                RaisePropertyChanged("PtInsuranceBenefit_2");
            }
        }

        private HealthInsurance _HealthInsurance_2;
        [DataMemberAttribute()]
        public HealthInsurance HealthInsurance_2
        {
            get
            {
                return _HealthInsurance_2;
            }
            set
            {
                _HealthInsurance_2 = value;
                RaisePropertyChanged("HealthInsurance_2");
            }
        }

        private long? _HisID_3;
        [DataMemberAttribute()]
        public long? HisID_3
        {
            get
            {
                return _HisID_3;
            }
            set
            {
                _HisID_3 = value;
                RaisePropertyChanged("HisID_3");
            }
        }

        private double? _PtInsuranceBenefit_3;
        [DataMemberAttribute()]
        public double? PtInsuranceBenefit_3
        {
            get
            {
                return _PtInsuranceBenefit_3;
            }
            set
            {
                _PtInsuranceBenefit_3 = value;
                RaisePropertyChanged("PtInsuranceBenefit_3");
            }
        }

        private HealthInsurance _HealthInsurance_3;
        [DataMemberAttribute()]
        public HealthInsurance HealthInsurance_3
        {
            get
            {
                return _HealthInsurance_3;
            }
            set
            {
                _HealthInsurance_3 = value;
                RaisePropertyChanged("HealthInsurance_3");
            }
        }
        /*▲====: #002*/

        private string _hiCardNo;
        [DataMemberAttribute()]
        public string HiCardNo
        {
            get
            {
                return _hiCardNo;
            }
            set
            {
                _hiCardNo = value;
                RaisePropertyChanged("HiCardNo");
            }
        }
        private long? _PaperReferalID;
        //Thông tin chuyển viện
        [DataMemberAttribute()]
        public long? PaperReferalID
        {
            get
            {
                return _PaperReferalID;
            }
            set
            {
                _PaperReferalID = value;
                RaisePropertyChanged("PaperReferalID");
            }
        }

        private long? _PaperReferralID_2;
        [DataMemberAttribute()]
        public long? PaperReferralID_2
        {
            get
            {
                return _PaperReferralID_2;
            }
            set
            {
                _PaperReferralID_2 = value;
                RaisePropertyChanged("PaperReferralID_2");
            }
        }
        private long? _PaperReferralID_3;
        [DataMemberAttribute()]
        public long? PaperReferralID_3
        {
            get
            {
                return _PaperReferralID_3;
            }
            set
            {
                _PaperReferralID_3 = value;
                RaisePropertyChanged("PaperReferralID_3");
            }
        }
        #region Navigation Properties

        private PaperReferal _PaperReferal;
        //Thông tin chuyển viện
        [DataMemberAttribute()]
        public PaperReferal PaperReferal
        {
            get
            {
                return _PaperReferal;
            }
            set
            {
                _PaperReferal = value;
                RaisePropertyChanged("PaperReferal");
            }
        }

        private PaperReferal _PaperReferral_2 = null;
        [DataMemberAttribute()]
        public PaperReferal PaperReferral_2
        {
            get
            {
                return _PaperReferral_2;
            }
            set
            {
                _PaperReferral_2 = value;
                RaisePropertyChanged("PaperReferral_2");
            }
        }
        private PaperReferal _PaperReferral_3 = null;
        [DataMemberAttribute()]
        public PaperReferal PaperReferral_3
        {
            get
            {
                return _PaperReferral_3;
            }
            set
            {
                _PaperReferral_3 = value;
                RaisePropertyChanged("PaperReferral_3");
            }
        }

        private PatientHI_SummaryInfo _PtHISumInfo;
        public PatientHI_SummaryInfo PtHISumInfo
        {
            get
            {
                if (_PtHISumInfo == null)
                {
                    _PtHISumInfo = new PatientHI_SummaryInfo();
                }
                if (IsCrossRegion.HasValue)
                    _PtHISumInfo.IsCrossRegion = IsCrossRegion.Value;
                else
                    _PtHISumInfo.IsCrossRegion = false;
                _PtHISumInfo.ConfirmedHiItem = HealthInsurance;
                _PtHISumInfo.ConfirmedHiItem_2 = HealthInsurance_2;
                _PtHISumInfo.ConfirmedHiItem_3 = HealthInsurance_3;
                _PtHISumInfo.ConfirmedPaperReferal = PaperReferal;
                _PtHISumInfo.ConfirmedPaperReferral_2 = PaperReferral_2;
                _PtHISumInfo.ConfirmedPaperReferral_3 = PaperReferral_3;
                _PtHISumInfo.HiBenefit = PtInsuranceBenefit;
                _PtHISumInfo.HiBenefit_2 = PtInsuranceBenefit_2;
                _PtHISumInfo.HiBenefit_3 = PtInsuranceBenefit_3;
                return _PtHISumInfo;
            }
        }

        private EmergencyRecord _EmergencyRecord;
        //Thông tin cấp cứu.
        [DataMemberAttribute()]
        public EmergencyRecord EmergencyRecord
        {
            get
            {
                return _EmergencyRecord;
            }
            set
            {
                _EmergencyRecord = value;
                RaisePropertyChanged("EmergencyRecord");
            }
        }
        private ObservableCollection<PatientRegistrationDetail> _PatientRegistrationDetails;

        [DataMemberAttribute()]
        //[CustomValidation(typeof(PatientRegistration), "ValidateRegistrationDetails")]
        [Required(ErrorMessage = "Please select a service.")]
        public ObservableCollection<PatientRegistrationDetail> PatientRegistrationDetails
        {
            get
            {
                return _PatientRegistrationDetails;
            }
            set
            {
                //ValidateProperty("PatientRegistrationDetails", value);
                _PatientRegistrationDetails = value;
                RaisePropertyChanged("PatientRegistrationDetails");

            }
        }

        private int _SequenceNo;
        [DataMemberAttribute()]
        public int SequenceNo
        {
            get
            {
                return _SequenceNo;
            }
            set
            {
                _SequenceNo = value;
                RaisePropertyChanged("SequenceNo");
            }
        }

        //public static ValidationResult ValidateRegistrationDetails(ObservableCollection<PatientRegistrationDetail> detailsList, ValidationContext context)
        //{
        //    if (detailsList == null)
        //    {
        //        return new ValidationResult("Please select a service.");
        //    }
        //    bool isValid = true;
        //    foreach (PatientRegistrationDetail d in detailsList)
        //    {
        //        if (isValid)
        //        {
        //            isValid = d.Validate();
        //        }
        //        else
        //        {
        //            d.Validate();
        //        }
        //    }
        //    if(isValid)
        //    {
        //        return ValidationResult.Success;
        //    }
        //    else
        //    {
        //        return new ValidationResult("Please validate details list.",new string[1]{"PatientRegistrationDetails"});
        //    }
        //}


        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PATIENTR_REL_PTINF_PATIENTS", "Patients")]
        public Patient Patient
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PATIENTR_REL_PTINF_REFDEPAR", "RefDepartments")]
        public RefDepartment RefDepartment
        {
            get;
            set;

        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PATIENTR_REL_PTINF_REGISTRA", "RegistrationType")]
        public RegistrationType RegistrationType
        {
            get;
            set;
        }



        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PATIENTR_REL_PTINF_STAFFS", "Staffs")]
        public Staff Staff
        {
            get;
            set;
        }

        private PatientTransaction _PatientTransaction;
        [DataMemberAttribute()]
        public PatientTransaction PatientTransaction
        {
            get
            {
                return _PatientTransaction;
            }
            set
            {
                _PatientTransaction = value;
                RaisePropertyChanged("PatientTransaction");
            }
        }

        #endregion

        private AllLookupValues.RegistrationStatus _RegistrationStatus;
        [DataMemberAttribute()]
        public AllLookupValues.RegistrationStatus RegistrationStatus
        {
            get
            {
                return _RegistrationStatus;
            }
            set
            {
                _RegistrationStatus = value;
                RaisePropertyChanged("RegistrationStatus");
            }
        }

        private AllLookupValues.RegistrationPaymentStatus _RegistrationPaymentStatus;
        [DataMemberAttribute()]
        public AllLookupValues.RegistrationPaymentStatus RegistrationPaymentStatus
        {
            get
            {
                return _RegistrationPaymentStatus;
            }
            set
            {
                if (_RegistrationPaymentStatus == value)
                    return;
                _RegistrationPaymentStatus = value;
            }
        }


        private string _MedServiceNames;

        /// Chứa danh sách tên các dịch vụ trong lần đăng ký này.
        /// Cách nhau bởi dấu ','

        [DataMemberAttribute()]
        public string MedServiceNames
        {
            get
            {
                return _MedServiceNames;
            }
            set
            {
                _MedServiceNames = value;
                RaisePropertyChanged("MedServiceNames");
            }
        }
        private HealthInsurance _HealthInsurance;
        [DataMemberAttribute()]
        public HealthInsurance HealthInsurance
        {
            get
            {
                return _HealthInsurance;
            }
            set
            {
                _HealthInsurance = value;
                RaisePropertyChanged("HealthInsurance");
            }
        }
        private RecordState _RecordState = RecordState.DETACHED;
        [DataMemberAttribute()]
        public RecordState RecordState
        {
            get
            {
                return _RecordState;
            }
            set
            {
                _RecordState = value;
                RaisePropertyChanged("RecordState");
            }
        }
        private PayableSum _PayableSum;

        /// Thông tin tổng số tiền phải trả cho đăng ký này.

        [DataMemberAttribute()]
        public PayableSum PayableSum
        {
            get
            {
                return _PayableSum;
            }
            set
            {
                _PayableSum = value;
                RaisePropertyChanged("PayableSum");
            }
        }
        // Hpt 24/09/2015: Thêm thuộc tính để kiểm tra đăng ký quá hạn (dùng khi xuất cho bệnh nhân Vãng Lai - Tiền Giải Phẫu_Module Khoa nội trú).
        // Khi một đăng ký được mở lên (trong các màn hình xuất cho bệnh nhân Vãng Lai-Tiền Giải Phẫu), biến IsOutOfDate sẽ được set giá trị
        // Giá trị IsOutOfDate có ảnh hưởng trực tiếp đến giá trị biến CanSave của đối tượng OutwardDrugClinicDeptInvoice (Có được xuất không)
        private bool _IsOutOfDate = false;
        [DataMemberAttribute()]
        public bool IsOutOfDate
        {
            get
            {
                return _IsOutOfDate;
            }
            set
            {
                _IsOutOfDate = value;
                RaisePropertyChanged("IsOutOfDate");
            }
        }

        private long _V_RegistrationStatus;
        [DataMemberAttribute()]
        public long V_RegistrationStatus
        {
            get
            {
                return _V_RegistrationStatus;
            }
            set
            {
                _V_RegistrationStatus = value;
                RaisePropertyChanged("V_RegistrationStatus");
            }
        }

        private string _StaffName;
        [DataMemberAttribute()]
        public string StaffName
        {
            get
            {
                if (Staff != null)
                {
                    return Staff.FullName;
                }
                return _StaffName;
            }
            set
            {
                if (_StaffName != value)
                {
                    _StaffName = value;
                    RaisePropertyChanged("StaffName");
                }
            }
        }

        private bool _MarkedAsDeleted;
        [DataMemberAttribute()]
        public bool MarkedAsDeleted
        {
            get
            {
                return _MarkedAsDeleted;
            }
            set
            {
                if (_MarkedAsDeleted != value)
                {
                    _MarkedAsDeleted = value;
                    RaisePropertyChanged("MarkedAsDeleted");
                }
            }
        }
        private bool _SomeRegDetailsRemoved;
        [DataMemberAttribute()]
        public bool SomeRegDetailsRemoved
        {
            get
            {
                return _SomeRegDetailsRemoved;
            }
            set
            {
                if (_SomeRegDetailsRemoved != value)
                {
                    _SomeRegDetailsRemoved = value;
                    RaisePropertyChanged("SomeRegDetailsRemoved");
                }
            }
        }

        private int _FindPatient;
        [DataMemberAttribute()]
        public int FindPatient
        {
            get
            {
                return _FindPatient;
            }
            set
            {
                if (_FindPatient != value)
                {
                    _FindPatient = value;
                    RaisePropertyChanged("FindPatient");
                }
            }
        }

        private AllLookupValues.RegistrationType _V_RegistrationType = AllLookupValues.RegistrationType.NGOAI_TRU;
        [DataMemberAttribute()]
        public AllLookupValues.RegistrationType V_RegistrationType
        {
            get
            {
                return _V_RegistrationType;
            }
            set
            {
                _V_RegistrationType = value;
                if (_V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
                {
                    FindPatient = 0;
                }
                else
                {
                    if (_V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
                    {
                        FindPatient = 1;
                    }
                }
                RaisePropertyChanged("V_RegistrationType");
            }
        }

        private AllLookupValues.V_RegForPatientOfType _V_RegForPatientOfType = AllLookupValues.V_RegForPatientOfType.Unknown;
        [DataMemberAttribute()]
        public AllLookupValues.V_RegForPatientOfType V_RegForPatientOfType
        {
            get
            {
                return _V_RegForPatientOfType;
            }
            set
            {
                _V_RegForPatientOfType = value;
                RaisePropertyChanged("V_RegForPatientOfType");
            }
        }

        private decimal? _progSumMinusMinHI;
        [DataMemberAttribute()]
        public decimal? ProgSumMinusMinHI
        {
            get { return _progSumMinusMinHI; }
            set
            {
                if (_progSumMinusMinHI != value)
                {
                    _progSumMinusMinHI = value;
                    RaisePropertyChanged("ProgSumMinusMinHI");
                }
            }
        }
        private ObservableCollection<InPatientBillingInvoice> _inPatientBillingInvoices;
        [DataMemberAttribute()]
        public ObservableCollection<InPatientBillingInvoice> InPatientBillingInvoices
        {
            get
            {
                return _inPatientBillingInvoices;
            }
            set
            {
                _inPatientBillingInvoices = value;
                RaisePropertyChanged("InPatientBillingInvoices");
            }
        }

        private InPatientAdmDisDetails _admissionInfo;
        /// <summary>
        /// Thông tin nhập viện của đăng ký ( Đang ky - Nhap Vien quan hệ 1-1 )
        /// </summary>
        [DataMemberAttribute()]
        public InPatientAdmDisDetails AdmissionInfo
        {
            get { return _admissionInfo; }
            set
            {
                _admissionInfo = value;
                RaisePropertyChanged("AdmissionInfo");
            }
        }

        private BedPatientAllocs _activeBedAllocation;
        [DataMemberAttribute()]
        public BedPatientAllocs ActiveBedAllocation
        {
            get { return _activeBedAllocation; }
            set
            {
                _activeBedAllocation = value;
                RaisePropertyChanged("ActiveBedAllocation");
            }
        }

        private string _DeptLocationName;
        [DataMemberAttribute()]
        public string DeptLocationName
        {
            get { return _DeptLocationName; }
            set
            {
                _DeptLocationName = value;
                RaisePropertyChanged("DeptLocationName");
            }
        }

        private ObservableCollection<BedPatientAllocs> _bedAllocations;
        [DataMemberAttribute()]
        public ObservableCollection<BedPatientAllocs> BedAllocations
        {
            get { return _bedAllocations; }
            set
            {
                _bedAllocations = value;
                RaisePropertyChanged("BedAllocations");
            }
        }

        private bool _isEmergency;
        [DataMemberAttribute()]
        public bool IsEmergency
        {
            get
            {
                return _isEmergency;
            }
            set
            {
                _isEmergency = value;
                RaisePropertyChanged("IsEmergency");
            }
        }

        [DataMemberAttribute()]
        public String HIComment
        {
            get
            {
                return _HIComment;
            }
            set
            {
                _HIComment = value;
                RaisePropertyChanged("HIComment");
            }
        }
        private String _HIComment;

        [DataMemberAttribute()]
        public String DiagDeptLocationName
        {
            get
            {
                return _DiagDeptLocationName;
            }
            set
            {
                _DiagDeptLocationName = value;
                RaisePropertyChanged("DiagDeptLocationName");
            }
        }
        private String _DiagDeptLocationName;


        /// <summary>
        /// Loại đăng ký
        /// 0 : Bình thường
        /// 1 : Đúng Tuyến
        /// 2 : Trái Tuyến
        /// </summary>
        private int _RegisType;
        [DataMemberAttribute()]
        public int RegisType
        {
            get { return _RegisType; }
            set
            {
                if (_RegisType != value)
                {
                    _RegisType = value;
                    RaisePropertyChanged("RegisType");
                }
            }
        }

        /// <summary>
        /// Trạng thái xuất viện
        /// True: Đã xuất viện
        /// False: Chưa xuất viện
        /// </summary>
        private bool _isDischarge;
        [DataMemberAttribute()]
        public bool IsDischarge
        {
            get { return _isDischarge; }
            set
            {
                if (_isDischarge != value)
                {
                    _isDischarge = value;
                    RaisePropertyChanged("IsDischarge");
                }
            }
        }

        private string _inDeptLocation;
        [DataMemberAttribute()]
        public string InDeptLocation
        {
            get { return _inDeptLocation; }
            set
            {
                if (_inDeptLocation != value)
                {
                    _inDeptLocation = value;
                    RaisePropertyChanged("InDeptLocation");
                }
            }
        }

        private DateTime? _AdmissionDate;
        [DataMemberAttribute()]
        public DateTime? AdmissionDate
        {
            get { return _AdmissionDate; }
            set
            {
                if (_AdmissionDate != value)
                {
                    _AdmissionDate = value;
                    RaisePropertyChanged("AdmissionDate");
                }
            }
        }

        private bool _IsChecked;
        [DataMemberAttribute()]
        public bool IsChecked
        {
            get { return _IsChecked; }
            set
            {
                if (_IsChecked != value)
                {
                    _IsChecked = value;
                    RaisePropertyChanged("IsChecked");
                }
            }
        }
        // Biến lưu trạng thái Lock
        // Tạm thời mặc định là return 1
        // Khi Kiên đưa thêm cột IsReported vào thì sẽ return theo giá trị của IsReported
        private int _RegLockFlag = 0;
        [DataMemberAttribute()]
        public int RegLockFlag
        {
            get { return _RegLockFlag; }
            set
            {
                _RegLockFlag = value;
            }
        }

        private long _AdmDeptID = 0;
        [DataMemberAttribute()]
        public long AdmDeptID
        {
            get { return _AdmDeptID; }
            set
            {
                _AdmDeptID = value;
            }
        }
        private long _HIReportID = 0;
        [DataMemberAttribute()]
        public long HIReportID
        {
            get { return _HIReportID; }
            set
            {
                _HIReportID = value;
                RaisePropertyChanged("HIReportID");
            }
        }

        private Int16 _Applied02Version1 = 0;
        [DataMemberAttribute()]
        public Int16 Applied02Version1
        {
            get { return _Applied02Version1; }
            set
            {
                _Applied02Version1 = value;
                RaisePropertyChanged("Applied02Version1");
            }
        }

        private InPatientInstruction _inPatientInstruction;
        [DataMemberAttribute()]
        public InPatientInstruction InPatientInstruction
        {
            get { return _inPatientInstruction; }
            set
            {
                if (_inPatientInstruction != value)
                {
                    _inPatientInstruction = value;
                    RaisePropertyChanged("InPatientInstruction");
                }
            }
        }
        /*TMA*/
        private long _TransferForm = 0;
        [DataMemberAttribute()]
        public long TransferForm
        {
            get { return _TransferForm; }
            set
            {
                _TransferForm = value;
            }
        }
        private string _TransferNum;
        [DataMemberAttribute()]
        public string TransferNum
        {
            get { return _TransferNum; }
            set
            {
                _TransferNum = value;
            }
        }
        /*TMA*/

        //*** TxD 09/01/2018 Begin: Added the following fields to allow adding another Health Insurance card to an existing Registration
        private bool _IsConfirmReplaceWithAnotherCard = false;
        public bool IsConfirmReplaceWithAnotherCard
        {
            get
            {
                return _IsConfirmReplaceWithAnotherCard;
            }
            set
            {
                _IsConfirmReplaceWithAnotherCard = value;
            }
        }

        private bool _IsConfirmJoiningWithNewCard = false;
        public bool IsConfirmJoiningWithNewCard
        {
            get
            {
                return _IsConfirmJoiningWithNewCard;
            }
            set
            {
                _IsConfirmReplaceWithAnotherCard = value;
            }
        }
        //*** TxD 09/01/2018 End

        private bool? _IsHIUnder15Percent;
        [DataMemberAttribute()]
        public bool? IsHIUnder15Percent
        {
            get { return _IsHIUnder15Percent; }
            set
            {
                _IsHIUnder15Percent = value;
            }
        }

        /*▼====: #003*/
        private bool? _BNTKSauXV;
        [DataMemberAttribute()]
        public bool? BNTKSauXV
        {
            get { return _BNTKSauXV; }
            set
            {
                _BNTKSauXV = value;
            }
        }
        /*▲====: #003*/

        private DiagnosisTreatment _DiagnosisTreatment;
        [DataMemberAttribute()]
        public DiagnosisTreatment DiagnosisTreatment
        {
            get
            {
                return _DiagnosisTreatment;
            }
            set
            {
                _DiagnosisTreatment = value;
                RaisePropertyChanged("DiagnosisTreatment");
            }
        }

        private bool _IsSelected = false;
        [DataMemberAttribute()]
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                _IsSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        private string _ReportAppliedCode;
        [DataMemberAttribute()]
        public string ReportAppliedCode
        {
            get
            {
                return _ReportAppliedCode;
            }
            set
            {
                _ReportAppliedCode = value;
                RaisePropertyChanged("ReportAppliedCode");
            }
        }

        private Lookup _V_ReportStatus;
        [DataMemberAttribute()]
        public Lookup V_ReportStatus
        {
            get
            {
                return _V_ReportStatus;
            }
            set
            {
                _V_ReportStatus = value;
                RaisePropertyChanged("V_ReportStatus");
            }
        }

        private long? _ConfirmHIStaffID;
        [DataMemberAttribute]
        public long? ConfirmHIStaffID
        {
            get
            {
                return _ConfirmHIStaffID;
            }
            set
            {
                _ConfirmHIStaffID = value;
                RaisePropertyChanged("ConfirmHIStaffID");
            }
        }

        private long? _PtRegistrationTransferID;
        [DataMemberAttribute]
        public long? PtRegistrationTransferID
        {
            get
            {
                return _PtRegistrationTransferID;
            }
            set
            {
                _PtRegistrationTransferID = value;
                RaisePropertyChanged("PtRegistrationTransferID");
            }
        }
        //▼====== #004
        private string _BasicDiagTreatment;
        [DataMemberAttribute()]
        public string BasicDiagTreatment
        {
            get
            {
                return _BasicDiagTreatment;
            }
            set
            {
                if (_BasicDiagTreatment != value)
                {
                    _BasicDiagTreatment = value;
                }
                RaisePropertyChanged("BasicDiagTreatment");
            }
        }
        //▲====== #004

        //▼==== #013
        private long _InPatientTransferDeptReqID;
        [DataMemberAttribute()]
        public long InPatientTransferDeptReqID
        {
            get
            {
                return _InPatientTransferDeptReqID;
            }
            set
            {
                _InPatientTransferDeptReqID = value;
                RaisePropertyChanged("InPatientTransferDeptReqID");
            }
        }

        private InPatientTransferDeptReq _InPatientTransferDeptReq;
        [DataMemberAttribute()]
        public InPatientTransferDeptReq InPatientTransferDeptReq
        {
            get
            {
                return _InPatientTransferDeptReq;
            }
            set
            {
                _InPatientTransferDeptReq = value;
                RaisePropertyChanged("InPatientTransferDeptReq");
            }
        }
        //▲==== #013
        //▼====: #014
        private long _DTDTReportID = 0;
        [DataMemberAttribute()]
        public long DTDTReportID
        {
            get { return _DTDTReportID; }
            set
            {
                _DTDTReportID = value;
                RaisePropertyChanged("DTDTReportID");
            }
        }
        //▲====: #014
        //▼====: #016
        private Lookup _V_HIReportWaitingStatus;
        [DataMemberAttribute()]
        public Lookup V_HIReportWaitingStatus
        {
            get
            {
                return _V_HIReportWaitingStatus;
            }
            set
            {
                _V_HIReportWaitingStatus = value;
                RaisePropertyChanged("V_HIReportWaitingStatus");
            }
        }
        //▲====: #016

        public IList<PatientRegistrationDetail> AllSaveRegistrationDetails
        {
            get
            {
                return this.PatientRegistrationDetails == null ? null : this.PatientRegistrationDetails.Where(x => !x.MarkedAsDeleted && x.V_ExamRegStatus != (long)V_ExamRegStatus.mNgungTraTienLai && x.RecordState != RecordState.DELETED).ToList();
            }
        }
        public IList<PatientPCLRequestDetail> AllSavePCLRequestDetails
        {
            get
            {
                return this.PCLRequests == null ? null : this.PCLRequests.Where(x => !x.MarkedAsDeleted && x.V_PCLRequestStatus != AllLookupValues.V_PCLRequestStatus.CANCEL && x.RecordState != RecordState.DELETED).SelectMany(x => x.PatientPCLRequestIndicators).Where(x => !x.MarkedAsDeleted && x.V_ExamRegStatus != (long)V_ExamRegStatus.mNgungTraTienLai && x.RecordState != RecordState.DELETED).ToList();
            }
        }
        public IList<PatientRegistrationDetail> AllCanceledRegistrationDetails
        {
            get
            {
                return this.PatientRegistrationDetails == null ? null : this.PatientRegistrationDetails.Where(x => !x.MarkedAsDeleted && x.V_ExamRegStatus == (long)V_ExamRegStatus.mNgungTraTienLai).ToList();
            }
        }
        public IList<PatientPCLRequestDetail> AllCanceledPCLRequestDetails
        {
            get
            {
                return this.PCLRequests == null ? null : this.PCLRequests.Where(x => !x.MarkedAsDeleted).SelectMany(x => x.PatientPCLRequestIndicators).Where(x => x.V_ExamRegStatus == (long)V_ExamRegStatus.mNgungTraTienLai).ToList();
            }
        }
        public IList<OutwardDrug> AllSaveOutwardDrugs
        {
            get
            {
                return this.DrugInvoices == null ? null : this.DrugInvoices.Where(x => x.V_OutDrugInvStatus != (long)AllLookupValues.V_OutDrugInvStatus.CANCELED && x.RecordState != RecordState.DELETED).SelectMany(x => x.OutwardDrugs).ToList();
            }
        }

        private PromoDiscountProgram _PromoDiscountProgramObj;
        [DataMemberAttribute]
        public PromoDiscountProgram PromoDiscountProgramObj
        {
            get => _PromoDiscountProgramObj; set
            {
                _PromoDiscountProgramObj = value;
                RaisePropertyChanged("PromoDiscountProgramObj");
            }
        }

        private List<PromoDiscountProgram> _DiscountProgramCollection;
        [DataMemberAttribute]
        public List<PromoDiscountProgram> DiscountProgramCollection
        {
            get
            {
                return _DiscountProgramCollection;
            }
            set
            {
                _DiscountProgramCollection = value;
                RaisePropertyChanged("DiscountProgramCollection");
            }
        }

        private string _ConfirmStaffFullName;
        [DataMemberAttribute]
        public string ConfirmStaffFullName
        {
            get => _ConfirmStaffFullName; set
            {
                _ConfirmStaffFullName = value;
                RaisePropertyChanged("ConfirmStaffFullName");
            }
        }

        public void ApplyDiscount(MedRegItemBase aInvoiceItem, bool aOnlyRoundResultForOutward, bool IsRemove)
        {
            var mPromoObj = this.PromoDiscountProgramObj;
            if (aInvoiceItem.PromoDiscProgID.GetValueOrDefault(0) > 0 && this.DiscountProgramCollection != null && this.DiscountProgramCollection.Any(x => x.PromoDiscProgID == aInvoiceItem.PromoDiscProgID.Value))
            {
                mPromoObj = this.DiscountProgramCollection.First(x => x.PromoDiscProgID == aInvoiceItem.PromoDiscProgID.Value);
            }
            //if (aInvoiceItem == null || mPromoObj == null || (aInvoiceItem.PaidTime.HasValue && aInvoiceItem.PaidTime != null))
            if (aInvoiceItem == null || mPromoObj == null)
            {
                aInvoiceItem.DiscountAmt = 0;
                return;
            }

            ////▼===== #005: Kiểm tra xem dịch vụ có bảo hiểm chi trả không nếu không thì đây là dịch vụ không tính BH => Xét giá BH về 0 để tính toán miễn giảm cho chính xác.
            //decimal HiPrice = (decimal)aInvoiceItem.HIAllowedPrice;
            //decimal PaymentPercent = 1;
            //if (aInvoiceItem.TotalHIPayment == 0)
            //{
            //    HiPrice = 0;
            //}
            //if(aInvoiceItem is PatientRegistrationDetail)
            //{
            //    PaymentPercent = (aInvoiceItem as PatientRegistrationDetail).HIPaymentPercent < 1 ? (decimal)(aInvoiceItem as PatientRegistrationDetail).HIPaymentPercent : PaymentPercent;
            //}
            //decimal mDiscountAmt = aInvoiceItem.DiscountAmt * (IsRemove ? 0 : 1);
            //if (!mPromoObj.IsOnPriceDiscount)
            //{
            //    //mDiscountAmt = Math.Round((aInvoiceItem.TotalPatientPayment + aInvoiceItem.DiscountAmt) * mPromoObj.DiscountPercent * (IsRemove ? 0 : 1), 0);
            //    mDiscountAmt = Math.Round(((aInvoiceItem.TotalInvoicePrice - (HiPrice * PaymentPercent)) + aInvoiceItem.DiscountAmt) * mPromoObj.DiscountPercent * (IsRemove ? 0 : 1), 0);
            //}
            //if (aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - mDiscountAmt <= 0)
            //{
            //    //mDiscountAmt = aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment;
            //    mDiscountAmt = aInvoiceItem.TotalInvoicePrice - (HiPrice * PaymentPercent);
            //}
            ////▲===== #005
            decimal mDiscountAmt = aInvoiceItem.DiscountAmt * (IsRemove ? 0 : 1);
            //▼====: #007
            switch (mPromoObj.V_DiscountTypeCount.LookupID)
            {
                case (long)AllLookupValues.V_DiscountTypeCount.All:
                    if (!mPromoObj.IsOnPriceDiscount)
                    {
                        //mDiscountAmt = Math.Round((aInvoiceItem.TotalPatientPayment + aInvoiceItem.DiscountAmt) * mPromoObj.DiscountPercent * (IsRemove ? 0 : 1), 0);
                        mDiscountAmt = Math.Round((aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - aInvoiceItem.OtherAmt) * mPromoObj.DiscountPercent * (IsRemove ? 0 : 1), 0);
                    }
                    if (aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - aInvoiceItem.OtherAmt - mDiscountAmt < 0)
                    {
                        mDiscountAmt = aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - aInvoiceItem.OtherAmt;
                    }
                    aInvoiceItem.TotalPatientPayment = aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - aInvoiceItem.OtherAmt - mDiscountAmt;
                    aInvoiceItem.DiscountAmt = mDiscountAmt;
                    break;
                case (long)AllLookupValues.V_DiscountTypeCount.PriceDifference:
                    if (!mPromoObj.IsOnPriceDiscount)
                    {
                        mDiscountAmt = Math.Round((aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - aInvoiceItem.TotalCoPayment - aInvoiceItem.OtherAmt) * mPromoObj.DiscountPercent * (IsRemove ? 0 : 1), 0);
                    }
                    if (aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - aInvoiceItem.TotalCoPayment - aInvoiceItem.OtherAmt - mDiscountAmt < 0)
                    {
                        mDiscountAmt = aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - aInvoiceItem.TotalCoPayment - aInvoiceItem.OtherAmt;
                    }
                    aInvoiceItem.TotalPatientPayment = aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - aInvoiceItem.OtherAmt - mDiscountAmt;
                    aInvoiceItem.DiscountAmt = mDiscountAmt;
                    break;
                case (long)AllLookupValues.V_DiscountTypeCount.AmountCoPay:
                    decimal mPriceDifference = aInvoiceItem.TotalHIPayment == 0 ? aInvoiceItem.TotalInvoicePrice : aInvoiceItem.TotalPriceDifference;
                    if (!mPromoObj.IsOnPriceDiscount)
                    {
                        mDiscountAmt = Math.Round((aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - mPriceDifference - aInvoiceItem.OtherAmt) * mPromoObj.DiscountPercent * (IsRemove ? 0 : 1), 0);
                    }
                    if (aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - mPriceDifference - aInvoiceItem.OtherAmt - mDiscountAmt < 0)
                    {
                        mDiscountAmt = aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - mPriceDifference - aInvoiceItem.OtherAmt;
                    }
                    aInvoiceItem.TotalPatientPayment = aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - aInvoiceItem.OtherAmt - mDiscountAmt;
                    aInvoiceItem.DiscountAmt = mDiscountAmt;
                    break;
                default:
                    if (!mPromoObj.IsOnPriceDiscount)
                    {
                        //mDiscountAmt = Math.Round((aInvoiceItem.TotalPatientPayment + aInvoiceItem.DiscountAmt) * mPromoObj.DiscountPercent * (IsRemove ? 0 : 1), 0);
                        mDiscountAmt = Math.Round((aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - aInvoiceItem.OtherAmt) * mPromoObj.DiscountPercent * (IsRemove ? 0 : 1), 0);
                    }
                    if (aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - aInvoiceItem.OtherAmt - mDiscountAmt < 0)
                    {
                        mDiscountAmt = aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - aInvoiceItem.OtherAmt;
                    }
                    aInvoiceItem.TotalPatientPayment = aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - aInvoiceItem.OtherAmt - mDiscountAmt;
                    aInvoiceItem.DiscountAmt = mDiscountAmt;
                    break;
            }
            //▲====: #007
        }
        public void ChangeHIBenefit(IInvoiceItem aInvoiceItem, IInvoiceItem aUpdateInvoiceItem, bool aFullHIBenefitForConfirm, long HiPolicyMinSalary, bool FullHIOfServicesForConfirm)
        {
            //if (aInvoiceItem == null || (aInvoiceItem.PaidTime.HasValue && aInvoiceItem.PaidTime != null))
            if (aInvoiceItem == null)
            {
                return;
            }
            aInvoiceItem.IsCountHI = aUpdateInvoiceItem.IsCountHI;
            aInvoiceItem.HIBenefit = FullHIOfServicesForConfirm && this != null && aInvoiceItem is PatientRegistrationDetail && (aInvoiceItem as PatientRegistrationDetail).RefMedicalServiceItem != null
                    && (aInvoiceItem as PatientRegistrationDetail).RefMedicalServiceItem.RefMedicalServiceType != null
                    && (aInvoiceItem as PatientRegistrationDetail).RefMedicalServiceItem.RefMedicalServiceType.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH && this.IsValid15PercentHIBenefitCase(HiPolicyMinSalary) ? 1.0 : aUpdateInvoiceItem.HIBenefit;
            aInvoiceItem.HisID = aUpdateInvoiceItem.HisID;
            aInvoiceItem.HIAllowedPrice = aUpdateInvoiceItem.HIAllowedPrice;
            aInvoiceItem.GetItemPrice(this, null, true, aFullHIBenefitForConfirm, HiPolicyMinSalary);
            aInvoiceItem.GetItemTotalPrice();
        }

        private decimal _TotalAmount = 0;
        private decimal _TotalHIRebate = 0;
        private decimal _TotalAmountCoPay = 0;
        private DateTime? _ReportedDate;
        [DataMemberAttribute]
        public decimal TotalAmount
        {
            get => _TotalAmount; set
            {
                _TotalAmount = value;
                RaisePropertyChanged("TotalAmount");
            }
        }
        [DataMemberAttribute]
        public decimal TotalHIRebate
        {
            get => _TotalHIRebate; set
            {
                _TotalHIRebate = value;
                RaisePropertyChanged("TotalHIRebate");
            }
        }
        [DataMemberAttribute]
        public decimal TotalAmountCoPay
        {
            get => _TotalAmountCoPay; set
            {
                _TotalAmountCoPay = value;
                RaisePropertyChanged("TotalAmountCoPay");
            }
        }
        [DataMemberAttribute]
        public DateTime? ReportedDate
        {
            get => _ReportedDate; set
            {
                _ReportedDate = value;
                RaisePropertyChanged("ReportedDate");
            }
        }

        public PatientRegistrationDetail LastestPatientRegistrationDetail
        {
            get
            {
                return PatientRegistrationDetails == null || PatientRegistrationDetails.Count == 0 ? null : PatientRegistrationDetails.LastOrDefault();
            }
        }

        private long? _InPtRegistrationID;
        [DataMemberAttribute()]
        public long? InPtRegistrationID
        {
            get
            {
                return _InPtRegistrationID;
            }
            set
            {
                _InPtRegistrationID = value;
                RaisePropertyChanged("InPtRegistrationID");
            }
        }
        private long? _PtRegistrationID_Child;
        [DataMemberAttribute()]
        public long? PtRegistrationID_Child
        {
            get
            {
                return _PtRegistrationID_Child;
            }
            set
            {
                _PtRegistrationID_Child = value;
                RaisePropertyChanged("PtRegistrationID_Child");
            }
        }
        private long _TranFinalizationID;
        [DataMemberAttribute]
        public long TranFinalizationID
        {
            get
            {
                return _TranFinalizationID;
            }
            set
            {
                _TranFinalizationID = value;
                RaisePropertyChanged("TranFinalizationID");
            }
        }
        public IList<MedRegItemBase> AllSavedInvoiceItem
        {
            get
            {
                List<MedRegItemBase> mValidInvoiceItem = new List<MedRegItemBase>();
                if (AllSaveRegistrationDetails != null)
                {
                    mValidInvoiceItem.AddRange(AllSaveRegistrationDetails);
                }
                if (AllSavePCLRequestDetails != null)
                {
                    mValidInvoiceItem.AddRange(AllSavePCLRequestDetails);
                }
                if (AllSaveOutwardDrugs != null)
                {
                    mValidInvoiceItem.AddRange(AllSaveOutwardDrugs);
                }
                return mValidInvoiceItem;
            }
        }
        private PatientAppointment _Appointment;
        [DataMemberAttribute]
        public PatientAppointment Appointment
        {
            get
            {
                return _Appointment;
            }
            set
            {
                if (_Appointment == value)
                {
                    return;
                }
                _Appointment = value;
                RaisePropertyChanged("Appointment");
            }
        }
        private DiseasesReference _AdmissionICD10;
        [DataMemberAttribute]
        public DiseasesReference AdmissionICD10
        {
            get
            {
                return _AdmissionICD10;
            }
            set
            {
                _AdmissionICD10 = value;
                RaisePropertyChanged("AdmissionICD10");
            }
        }
        private Staff _OutHosDiagStaff;
        [DataMemberAttribute]
        public Staff OutHosDiagStaff
        {
            get
            {
                return _OutHosDiagStaff;
            }
            set
            {
                _OutHosDiagStaff = value;
                RaisePropertyChanged("OutHosDiagStaff");
            }
        }
        private DiagnosysConsultationSummary _DiagnosysConsultation;
        [DataMemberAttribute]
        public DiagnosysConsultationSummary DiagnosysConsultation
        {
            get
            {
                return _DiagnosysConsultation;
            }
            set
            {
                _DiagnosysConsultation = value;
                RaisePropertyChanged("DiagnosysConsultation");
            }
        }

        //20191023 TTM: Danh sách các lần hội chẩn của bệnh nhân.
        private ObservableCollection<DiagnosysConsultationSummary> _DiagnosysConsultationCollection;
        [DataMemberAttribute]
        public ObservableCollection<DiagnosysConsultationSummary> DiagnosysConsultationCollection
        {
            get
            {
                return _DiagnosysConsultationCollection;
            }
            set
            {
                _DiagnosysConsultationCollection = value;
                RaisePropertyChanged("DiagnosysConsultationCollection");
            }
        }
        private string _MedServiceNameCancel;

        /// Chứa danh sách tên các dịch vụ đã hủy trong lần đăng ký này.
        /// Cách nhau bởi dấu ','

        [DataMemberAttribute()]
        public string MedServiceNameCancel
        {
            get
            {
                return _MedServiceNameCancel;
            }
            set
            {
                _MedServiceNameCancel = value;
                RaisePropertyChanged("MedServiceNameCancel");
            }
        }
        private bool _IsCancelRegistration;
        [DataMemberAttribute()]
        public bool IsCancelRegistration
        {
            get
            {
                return _IsCancelRegistration;
            }
            set
            {
                _IsCancelRegistration = value;
                RaisePropertyChanged("IsCancelRegistration");
            }
        }
        private long? _OutPtTreatmentProgramID;
        [DataMemberAttribute]
        public long? OutPtTreatmentProgramID
        {
            get
            {
                return _OutPtTreatmentProgramID;
            }
            set
            {
                if (_OutPtTreatmentProgramID != value)
                {
                    _OutPtTreatmentProgramID = value;
                    RaisePropertyChanged("OutPtTreatmentProgramID");
                    RaisePropertyChanged("IsInTreatmentProgram");
                }
            }
        }
        public bool IsInTreatmentProgram
        {
            get
            {
                return OutPtTreatmentProgramID.HasValue && OutPtTreatmentProgramID > 0;
            }
        }
        private string _ChangedLog;
        [DataMemberAttribute()]
        public string ChangedLog
        {
            get
            {
                return _ChangedLog;
            }
            set
            {
                _ChangedLog = value;
                RaisePropertyChanged("ChangedLog");
            }
        }

        private ObservableCollection<PrescriptionIssueHistory> _ListOfPrescriptionIssueHistory;
        [DataMemberAttribute()]
        public ObservableCollection<PrescriptionIssueHistory> ListOfPrescriptionIssueHistory
        {
            get
            {
                return _ListOfPrescriptionIssueHistory;
            }
            set
            {
                if (_ListOfPrescriptionIssueHistory != value)
                {
                    _ListOfPrescriptionIssueHistory = value;
                    RaisePropertyChanged("ListOfPrescriptionIssueHistory");
                }
            }
        }

        private Patient _RefByPatient;
        [DataMemberAttribute()]
        public Patient RefByPatient
        {
            get
            {
                return _RefByPatient;
            }
            set
            {
                _RefByPatient = value;
                RaisePropertyChanged("RefByPatient");
            }
        }

        private Staff _RefByStaff;
        [DataMemberAttribute()]
        public Staff RefByStaff
        {
            get
            {
                return _RefByStaff;
            }
            set
            {
                _RefByStaff = value;
                RaisePropertyChanged("RefByStaff");
            }
        }
        private decimal _TreatmentTotalMinute;
        [DataMemberAttribute]
        public decimal TreatmentTotalMinute
        {
            get
            {
                return _TreatmentTotalMinute;
            }
            set
            {
                if (_TreatmentTotalMinute == value)
                {
                    return;
                }
                _TreatmentTotalMinute = value;
                RaisePropertyChanged("TreatmentTotalMinute");
            }
        }
        private DateTime? _CurrentInPatientDeptDetailFromDate;
        [DataMemberAttribute]
        public DateTime? CurrentInPatientDeptDetailFromDate
        {
            get
            {
                return _CurrentInPatientDeptDetailFromDate;
            }
            set
            {
                if (_CurrentInPatientDeptDetailFromDate == value)
                {
                    return;
                }
                _CurrentInPatientDeptDetailFromDate = value;
                RaisePropertyChanged("CurrentInPatientDeptDetailFromDate");
            }
        }
        private DateTime? _CurrentInPatientDeptDetailToDate;
        [DataMemberAttribute]
        public DateTime? CurrentInPatientDeptDetailToDate
        {
            get
            {
                return _CurrentInPatientDeptDetailToDate;
            }
            set
            {
                if (_CurrentInPatientDeptDetailToDate == value)
                {
                    return;
                }
                _CurrentInPatientDeptDetailToDate = value;
                RaisePropertyChanged("CurrentInPatientDeptDetailToDate");
            }
        }
        private TicketIssue _TicketIssue;
        [DataMemberAttribute]
        public TicketIssue TicketIssue
        {
            get
            {
                return _TicketIssue;
            }
            set
            {
                _TicketIssue = value;
                RaisePropertyChanged("TicketIssue");
            }
        }

        private long _OutPtRegistrationID;
        [DataMemberAttribute]
        public long OutPtRegistrationID
        {
            get
            {
                return _OutPtRegistrationID;
            }
            set
            {
                if (_OutPtRegistrationID == value)
                {
                    return;
                }
                _OutPtRegistrationID = value;
                RaisePropertyChanged("OutPtRegistrationID");
            }
        }
        private bool _IsDiffBetweenRegistrationAndTicket;
        [DataMemberAttribute]
        public bool IsDiffBetweenRegistrationAndTicket
        {
            get
            {
                return _IsDiffBetweenRegistrationAndTicket;
            }
            set
            {
                if (_IsDiffBetweenRegistrationAndTicket != value)
                {
                    _IsDiffBetweenRegistrationAndTicket = value;
                    RaisePropertyChanged("IsDiffBetweenRegistrationAndTicket");
                }
            }
        }

        private SmallProcedure _SmallProcedureForAutoPerform;
        [DataMemberAttribute]
        public SmallProcedure SmallProcedureForAutoPerform
        {
            get
            {
                return _SmallProcedureForAutoPerform;
            }
            set
            {
                if (_SmallProcedureForAutoPerform != value)
                {
                    _SmallProcedureForAutoPerform = value;
                    RaisePropertyChanged("SmallProcedureForAutoPerform");
                }
            }
        }

        private bool _IsSpecialHIRegistration;
        [DataMemberAttribute]
        public bool IsSpecialHIRegistration
        {
            get
            {
                return _IsSpecialHIRegistration;
            }
            set
            {
                if (_IsSpecialHIRegistration != value)
                {
                    _IsSpecialHIRegistration = value;
                    RaisePropertyChanged("IsSpecialHIRegistration");
                }
            }
        }
        //▼====: #008
        private long? _V_CheckMedicalFilesStatus;
        [DataMemberAttribute]
        public long? V_CheckMedicalFilesStatus
        {
            get
            {
                return _V_CheckMedicalFilesStatus;
            }
            set
            {
                if (_V_CheckMedicalFilesStatus != value)
                {
                    _V_CheckMedicalFilesStatus = value;
                    RaisePropertyChanged("V_CheckMedicalFilesStatus");
                }
            }
        }

        //▼====: #010
        private bool _DLSReject = false;
        [DataMemberAttribute]
        public bool DLSReject
        {
            get
            {
                return _DLSReject;
            }
            set
            {
                if (_DLSReject != value)
                {
                    _DLSReject = value;
                    RaisePropertyChanged("DLSReject");
                }
            }
        }
        //▲====: #010

        private bool _IsDLSChecked;
        [DataMemberAttribute]
        public bool IsDLSChecked
        {
            get
            {
                return _IsDLSChecked;
            }
            set
            {
                if (_IsDLSChecked != value)
                {
                    _IsDLSChecked = value;
                    RaisePropertyChanged("IsDLSChecked");
                }
            }
        }

        private CheckMedicalFiles _CheckMedicalFiles;
        [DataMemberAttribute]
        public CheckMedicalFiles CheckMedicalFiles
        {
            get
            {
                return _CheckMedicalFiles;
            }
            set
            {
                if (_CheckMedicalFiles != value)
                {
                    _CheckMedicalFiles = value;
                    RaisePropertyChanged("CheckMedicalFiles");
                }
            }
        }
        //▲====: #008
        //▼====: #008
        private bool _HasPayOutPtDrugBill = false;
        [DataMemberAttribute]
        public bool HasPayOutPtDrugBill
        {
            get
            {
                return _HasPayOutPtDrugBill;
            }
            set
            {
                if (_HasPayOutPtDrugBill != value)
                {
                    _HasPayOutPtDrugBill = value;
                    RaisePropertyChanged("HasPayOutPtDrugBill");
                }
            }
        }
        //▲====: #008

        //▼====: #011
        private bool _IsAdmission = false;
        [DataMemberAttribute()]
        public bool IsAdmission
        {
            get
            {
                return _IsAdmission;
            }
            set
            {
                if (_IsAdmission == value)
                {
                    return;
                }
                _IsAdmission = value;
                RaisePropertyChanged("IsAdmission");
            }
        }
        //▲====: #011
        private bool _IsSumTreatmentProgram = false;
        [DataMemberAttribute]
        public bool IsSumTreatmentProgram
        {
            get
            {
                return _IsSumTreatmentProgram;
            }
            set
            {
                if (_IsSumTreatmentProgram != value)
                {
                    _IsSumTreatmentProgram = value;
                    RaisePropertyChanged("IsSumTreatmentProgram");
                }
            }
        }
        private long? _OutpatientTreatmentTypeID = null;
        [DataMemberAttribute]
        public long? OutpatientTreatmentTypeID
        {
            get
            {
                return _OutpatientTreatmentTypeID;
            }
            set
            {
                if (_OutpatientTreatmentTypeID != value)
                {
                    _OutpatientTreatmentTypeID = value;
                    RaisePropertyChanged("OutpatientTreatmentTypeID");
                }
            }
        }
        private bool _IsChronic = false;
        [DataMemberAttribute]
        public bool IsChronic
        {
            get
            {
                return _IsChronic;
            }
            set
            {
                if (_IsChronic != value)
                {
                    _IsChronic = value;
                    RaisePropertyChanged("IsChronic");
                }
            }
        }
        private int _PrescriptionsAmount = 0;
        [DataMemberAttribute]
        public int PrescriptionsAmount
        {
            get
            {
                return _PrescriptionsAmount;
            }
            set
            {
                if (_PrescriptionsAmount != value)
                {
                    _PrescriptionsAmount = value;
                    RaisePropertyChanged("PrescriptionsAmount");
                }
            }
        }
        private Lookup _V_OutDischargeCondition;
        [DataMemberAttribute]
        public Lookup V_OutDischargeCondition
        {
            get
            {
                return _V_OutDischargeCondition;
            }
            set
            {
                if (_V_OutDischargeCondition != value)
                {
                    _V_OutDischargeCondition = value;
                    RaisePropertyChanged("V_OutDischargeCondition");
                }
            }
        }
        private Lookup _V_OutDischargeType;
        [DataMemberAttribute]
        public Lookup V_OutDischargeType
        {
            get
            {
                return _V_OutDischargeType;
            }
            set
            {
                if (_V_OutDischargeType != value)
                {
                    _V_OutDischargeType = value;
                    RaisePropertyChanged("V_OutDischargeType");
                }
            }
        }
        private long _OutPtTreatmentProgramStaffID;
        [DataMemberAttribute]
        public long OutPtTreatmentProgramStaffID
        {
            get
            {
                return _OutPtTreatmentProgramStaffID;
            }
            set
            {
                if (_OutPtTreatmentProgramStaffID != value)
                {
                    _OutPtTreatmentProgramStaffID = value;
                    RaisePropertyChanged("OutPtTreatmentProgramStaffID");
                }
            }
        }
        //▼===== #015
        private Lookup _V_ReportStatusPrescription;
        [DataMemberAttribute()]
        public Lookup V_ReportStatusPrescription
        {
            get
            {
                return _V_ReportStatusPrescription;
            }
            set
            {
                _V_ReportStatusPrescription = value;
                RaisePropertyChanged("V_ReportStatusPrescription");
            }
        }
        //▲===== #015
        private Lookup _V_MedicalExaminationType;
        [DataMemberAttribute()]
        public Lookup V_MedicalExaminationType
        {
            get
            {
                return _V_MedicalExaminationType;
            }
            set
            {
                _V_MedicalExaminationType = value;
                RaisePropertyChanged("V_MedicalExaminationType");
            }
        }
        private Lookup _V_ReasonHospitalStay;
        [DataMemberAttribute()]
        public Lookup V_ReasonHospitalStay
        {
            get
            {
                return _V_ReasonHospitalStay;
            }
            set
            {
                _V_ReasonHospitalStay = value;
                RaisePropertyChanged("V_ReasonHospitalStay");
            }
        }


        private string _DisChargePapersDoctorName;
        [DataMemberAttribute]
        public string DisChargePapersDoctorName
        {
            get
            {
                return _DisChargePapersDoctorName;
            }
            set
            {
                if (_DisChargePapersDoctorName != value)
                {
                    _DisChargePapersDoctorName = value;
                    RaisePropertyChanged("DisChargePapersDoctorName");
                }
            }
        }
        private long _V_ReceiveMethod;
        [DataMemberAttribute()]
        public long V_ReceiveMethod
        {
            get
            {
                return _V_ReceiveMethod;
            }
            set
            {
                _V_ReceiveMethod = value;
                RaisePropertyChanged("V_ReceiveMethod");
            }
        }
        private Lookup _V_ObjectMedicalExamination;
        [DataMemberAttribute()]
        public Lookup V_ObjectMedicalExamination
        {
            get
            {
                return _V_ObjectMedicalExamination;
            }
            set
            {
                _V_ObjectMedicalExamination = value;
                RaisePropertyChanged("V_ObjectMedicalExamination");
            }
        }
    }
    public static class PatientRegistrationBase
    {
        public static IList<IInvoiceItem> GetSaveInvoiceItem(this PatientRegistration aRegistration)
        {
            List<IInvoiceItem> mValidInvoiceItem = new List<IInvoiceItem>();
            if (aRegistration.AllSaveRegistrationDetails != null)
            {
                mValidInvoiceItem.AddRange(aRegistration.AllSaveRegistrationDetails);
            }
            if (aRegistration.AllSavePCLRequestDetails != null)
            {
                mValidInvoiceItem.AddRange(aRegistration.AllSavePCLRequestDetails);
            }
            if (aRegistration.AllSaveOutwardDrugs != null)
            {
                mValidInvoiceItem.AddRange(aRegistration.AllSaveOutwardDrugs);
            }
            return mValidInvoiceItem;
        }
        public static void CorrectRegistrationDetails(this PatientRegistration aRegistration, bool aSpecialRuleForHIConsultationApplied, DateTime? aCurrentServerDateTime, float[] aHIPercentOnDifDept, decimal aHIPolicyMinSalary, bool aOnlyRoundResultForOutward, bool aFullHIBenefitForConfirm, bool aDetectHiApplied, decimal AddingServicesPercent, bool aFullHIOfServicesForConfirm
            , decimal AddingHIServicesPercent)
        {
            //Có áp dụng luật Bảo hiểm đối với dịch vụ KCB hay không (Đối với tất cả các dịch vụ KCB, bảo hiểm chỉ tính 1 dịch vụ thôi, còn lại là không có BH)
            //if (!Convert.ToBoolean(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.SpecialRuleForHIConsultationApplied]))

            // Txd 25/05/2014 Replaced ConfigList
            if (!aSpecialRuleForHIConsultationApplied)
            {
                return;
            }
            if (aRegistration.PatientRegistrationDetails == null)
            {
                return;
            }
            if (aRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                bool CountedFull = false;
                //▼===== #006
                foreach (var item in aRegistration.AllSaveRegistrationDetails.Where(x => (x.RefMedicalServiceItem.RefMedicalServiceType != null && x.RefMedicalServiceItem.RefMedicalServiceType.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH)
                    && x.RecordState != RecordState.DELETED
                    && !x.MarkedAsDeleted
                    && x.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI).OrderByDescending(x => x.HisID).OrderBy(x => x.PtRegDetailID > 0 ? x.PtRegDetailID : int.MaxValue).ToList())

                //foreach (var item in aRegistration.AllSaveRegistrationDetails.Where(x => x.RefMedicalServiceItem.RefMedicalServiceType.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH
                //&& x.RecordState != RecordState.DELETED
                //&& !x.MarkedAsDeleted
                //&& x.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI).OrderByDescending(x => x.HIPaymentPercent).ToList())
                //▲===== #006
                {
                    if (CountedFull && (aRegistration.Appointment == null || aRegistration.Appointment.V_AppointmentType != (long)AllLookupValues.AppointmentType.HEN_KHAM_SUC_KHOE))
                    {
                        if (item.PaidTime != null)
                        {
                            continue;
                        }

                        item.GetItemPrice(aRegistration, item.HIBenefit.GetValueOrDefault(0), aCurrentServerDateTime, aDetectHiApplied, false, (long)aHIPolicyMinSalary);
                        if (item.HIAllowedPrice > 0 && aRegistration.PtInsuranceBenefit.GetValueOrDefault(0) > 0)
                        {
                            item.PaymentPercent = (double)AddingHIServicesPercent;
                        }
                        else
                        {
                            item.PaymentPercent = (double)AddingServicesPercent;
                        }
                        //item.DiscountAmt = item.ChargeableItem.NormalPrice - item.ChargeableItem.NormalPrice * SecondServicesPercent;
                        item.GetItemPrice(aRegistration, item.HIBenefit.GetValueOrDefault(0), aCurrentServerDateTime, aDetectHiApplied, false, (long)aHIPolicyMinSalary);
                        item.GetItemTotalPrice(aOnlyRoundResultForOutward);
                    }
                    else if (!CountedFull && item.PaymentPercent != 1)
                    {
                        item.PaymentPercent = 1;
                        item.GetItemPrice(aRegistration, item.HIBenefit.GetValueOrDefault(0), aCurrentServerDateTime, aDetectHiApplied, false, (long)aHIPolicyMinSalary);
                        item.GetItemTotalPrice(aOnlyRoundResultForOutward);
                    }
                    CountedFull = true;
                }

                //▼===== #006
                IList<PatientRegistrationDetail> hiRegDetails = aRegistration.PatientRegistrationDetails.Where(x =>
                    (x.RefMedicalServiceItem.RefMedicalServiceType != null && x.RefMedicalServiceItem.RefMedicalServiceType.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH)
                    && x.RecordState != RecordState.DELETED
                    && !x.MarkedAsDeleted
                    && x.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI
                    && x.HisID.HasValue && x.HisID.Value > 0
                    && x.HIAllowedPrice.HasValue
                    && x.HIAllowedPrice.Value > 0).OrderByDescending(x => x.HisID).OrderBy(x => x.PtRegDetailID > 0 ? x.PtRegDetailID : int.MaxValue).ToList();

                //IList<PatientRegistrationDetail> hiRegDetails = aRegistration.PatientRegistrationDetails.Where(x =>
                //    x.RefMedicalServiceItem.RefMedicalServiceType.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH
                //    && x.RecordState != RecordState.DELETED
                //    && !x.MarkedAsDeleted
                //    && x.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI
                //    && x.HisID.HasValue && x.HisID.Value > 0
                //    && x.HIAllowedPrice.HasValue
                //    && x.HIAllowedPrice.Value > 0).OrderByDescending(x => x.HIPaymentPercent).ToList();
                //▲===== #006
                if (hiRegDetails == null || hiRegDetails.Count < 1)
                {
                    return; // Khong co dich vu BH nao thi return
                }

                //Tinh tong so nhung dich vu bao hiem chap nhan tinh
                //BH chi tinh cho 1 dich vu KCB thoi
                var total = hiRegDetails.Where(x => x.HIAllowedPrice > 0).Count();

                if (total == 0)
                {
                    var registrationDetail = hiRegDetails.FirstOrDefault();

                    if (registrationDetail.ID > 0 && registrationDetail.HisID.HasValue
                        && registrationDetail.HisID.Value > 0
                        && registrationDetail.HIAllowedPrice.HasValue
                        && registrationDetail.HIAllowedPrice.Value > 0)//Co su dung the bh
                    {
                        registrationDetail.HIBenefit = aRegistration.PtInsuranceBenefit;
                    }

                    registrationDetail.ChangeHIBenefit(registrationDetail.HIBenefit.GetValueOrDefault(0), aRegistration, aCurrentServerDateTime, aOnlyRoundResultForOutward, aDetectHiApplied);
                }
                if (total > 0)
                {
                    var firstItem = hiRegDetails.First(x => x.HIAllowedPrice > 0);
                    if (firstItem.PaidTime == null)
                    {
                        firstItem.ChangeHIBenefit(firstItem.HIBenefit.GetValueOrDefault(0), aRegistration, aCurrentServerDateTime, aOnlyRoundResultForOutward, aDetectHiApplied);
                        firstItem.ApplyHIPaymentPercent(1.0, aRegistration, aCurrentServerDateTime, aOnlyRoundResultForOutward, aDetectHiApplied);
                    }
                }
                if (total > 1)
                {
                    var firstItem = hiRegDetails.First(x => x.HIAllowedPrice > 0);
                    if (aHIPercentOnDifDept != null)
                    {
                        List<long> SpecialCollection = new List<long>();
                        foreach (var aItem in hiRegDetails)
                        {
                            if ((aItem.RefMedicalServiceItem != null && aItem.RefMedicalServiceItem.V_SpecialistType > 0 && SpecialCollection.Contains(aItem.RefMedicalServiceItem.V_SpecialistType))
                                || (aItem.RefMedicalServiceItem == null && aItem.RefMedicalServiceItem.V_SpecialistType == 0))
                            {
                                //Không tính quyền lợi BHYT cho các dịch vụ không có chuyên khoa hoặc bị trùng chuyên khoa với các dịch vụ trước
                                if (aItem.PaidTime == null)
                                {
                                    aItem.ApplyHIPaymentPercent(0, aRegistration, aCurrentServerDateTime, aOnlyRoundResultForOutward, aDetectHiApplied);
                                }
                            }
                            else
                            {
                                if (SpecialCollection.Count > 0)
                                {
                                    if (aHIPercentOnDifDept.Length > SpecialCollection.Count - 1)
                                    {
                                        //Tính quyền lợi BHYT theo tỉ lệ cấu hình cho dịch vụ khác chuyên khoa theo thứ tự
                                        if (aItem.PaidTime == null)
                                        {
                                            aItem.ApplyHIPaymentPercent(Math.Round(aHIPercentOnDifDept[SpecialCollection.Count - 1], 4), aRegistration, aCurrentServerDateTime, aOnlyRoundResultForOutward, aDetectHiApplied);
                                        }
                                    }
                                    else if (aItem.PaidTime == null)
                                    {
                                        //Không tính quyền lợi BHYT cho Dịch vụ vượt quá số lượng chuyên khoa cho phép một lần khám
                                        aItem.ApplyHIPaymentPercent(0, aRegistration, aCurrentServerDateTime, aOnlyRoundResultForOutward, aDetectHiApplied);
                                    }
                                }
                                SpecialCollection.Add(aItem.RefMedicalServiceItem.V_SpecialistType);
                            }
                        }
                        //▲====:
                    }
                    else
                    {
                        //Thang dau tien khong tinh.
                        foreach (var registrationDetail in hiRegDetails.Where(item => item != firstItem))
                        {
                            registrationDetail.ChangeHIBenefit();
                        }
                    }
                }
                //Total = 1 la OK
                if (aFullHIOfServicesForConfirm)
                {
                    foreach (var item in aRegistration.AllSaveRegistrationDetails.Where(x => x.HIAllowedPrice > 0
                            && x.HIBenefit != 1
                            && x.PaidTime == null
                            && x.RefMedicalServiceItem != null
                            && x.RefMedicalServiceItem.RefMedicalServiceType != null
                            && x.RefMedicalServiceItem.RefMedicalServiceType.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH))
                    {
                        item.HIBenefit = 1;
                        item.ChangeHIBenefit(item.HIBenefit.GetValueOrDefault(0), aRegistration, aCurrentServerDateTime, aOnlyRoundResultForOutward);
                    }
                }
                else if (aFullHIBenefitForConfirm)
                {
                    if (!aRegistration.IsValid15PercentHIBenefitCase(aHIPolicyMinSalary))
                    {
                        foreach (var item in aRegistration.GetSaveInvoiceItem().Where(x => x.HIAllowedPrice > 0 && x.HIBenefit != aRegistration.PtInsuranceBenefit && x.PaidTime == null))
                        {
                            item.HIBenefit = aRegistration.PtInsuranceBenefit;
                            item.ChangeHIBenefit(item.HIBenefit.GetValueOrDefault(0), aRegistration, aCurrentServerDateTime, aOnlyRoundResultForOutward);
                            aRegistration.BalanceTransactionDetailByInvoiceItem(item);
                        }
                    }
                    else
                    {
                        foreach (var item in aRegistration.GetSaveInvoiceItem().Where(x => x.HIAllowedPrice > 0 && x.HIBenefit != 1 && x.PaidTime == null))
                        {
                            item.HIBenefit = 1;
                            item.ChangeHIBenefit(item.HIBenefit.GetValueOrDefault(0), aRegistration, aCurrentServerDateTime, aOnlyRoundResultForOutward);
                            aRegistration.BalanceTransactionDetailByInvoiceItem(item);
                        }
                    }
                }
            }
            //Tinh tong so dich vu KCB duoc bao hiem thanh toan, neu tong so nay > 1 thi phai tinh lai
            if (aRegistration.AllSavedInvoiceItem != null && aRegistration.AllSavedInvoiceItem.Count > 0)
            {
                foreach (var item in aRegistration.AllSavedInvoiceItem.Where(x => x.PaidTime == null && x.IsDiscounted))
                {
                    if (aRegistration.PromoDiscountProgramObj != null && item.DiscountAmt != Math.Round(item.TotalPatientPayment * aRegistration.PromoDiscountProgramObj.DiscountPercent, 0))
                    {
                        aRegistration.ApplyDiscount(item, aOnlyRoundResultForOutward, false);
                    }
                }
            }
        }
        public static bool IsValid15PercentHIBenefitCase(this PatientRegistration aRegistration, decimal aHIPolicyMinSalary)
        {
            decimal mTotalHIPayment = 0;
            mTotalHIPayment += aRegistration.AllSaveRegistrationDetails == null ? 0 : aRegistration.AllSaveRegistrationDetails.Where(x => x.HIAllowedPrice > 0).Sum(x => (decimal)x.ServiceQty.GetValueOrDefault(1) * x.HIAllowedPrice.GetValueOrDefault(0));
            mTotalHIPayment += aRegistration.AllSavePCLRequestDetails == null ? 0 : aRegistration.AllSavePCLRequestDetails.Where(x => x.HIAllowedPrice > 0).Sum(x => (decimal)x.NumberOfTest.GetValueOrDefault(1) * x.HIAllowedPrice.GetValueOrDefault(0));
            mTotalHIPayment += aRegistration.AllSaveOutwardDrugs == null ? 0 : aRegistration.AllSaveOutwardDrugs.Where(x => x.HIAllowedPrice > 0).Sum(x => x.OutQuantity * x.HIAllowedPrice.GetValueOrDefault(0));
            mTotalHIPayment += aRegistration.InPatientBillingInvoices == null ? 0 : aRegistration.InPatientBillingInvoices.Sum(x => x.TotalHIPayment + x.TotalCoPayment);
            if (mTotalHIPayment < aHIPolicyMinSalary * 15 / 100)
            {
                return true;
            }
            return false;
        }
        public static void RecalHIBenefit(this PatientRegistration aRegistration, bool aEnableOutPtCashAdvance, decimal aHIPolicyMinSalary, bool aOnlyRoundResultForOutward, bool IsCheckErrorPaidTime = true)
        {
            if (!aEnableOutPtCashAdvance) return;
            if (aRegistration.V_RegistrationStatus == (long)AllLookupValues.RegistrationStatus.COMPLETED)
            {
                throw new Exception(eHCMSLanguage.eHCMSResources.K2860_G1_DKDaHTat);
            }
            if (aRegistration.AllSaveRegistrationDetails != null && aRegistration.AllSaveRegistrationDetails.Any(x => x.RefMedicalServiceItem == null || (x.V_ExamRegStatus == (long)V_ExamRegStatus.mDangKyKham && x.RefMedicalServiceItem != null && x.RefMedicalServiceItem.MedicalServiceTypeID.Equals(1))))
            {
                throw new Exception(eHCMSLanguage.eHCMSResources.Z2322_G1_DichVuKBChuaHoanTat);
            }
            if (aRegistration.AllSavePCLRequestDetails != null && aRegistration.PatientClassID != (long)ePatientClassification.PayAfter && aRegistration.PatientClassID != (long)ePatientClassification.CompanyHealthRecord && aRegistration.AllSavePCLRequestDetails.Any(x => x.PaidTime == null) && IsCheckErrorPaidTime)
            {
                throw new Exception(eHCMSLanguage.eHCMSResources.Z2323_G1_ChuaHoanTatThanhToan);
            }
            if (aRegistration.IsValid15PercentHIBenefitCase(aHIPolicyMinSalary) && aRegistration.PtInsuranceBenefit.GetValueOrDefault(0) > 0)
            {
                aRegistration.PtInsuranceBenefit = 1;
            }
            foreach (var item in aRegistration.GetSaveInvoiceItem())
            {
                if (item.HIBenefit != aRegistration.PtInsuranceBenefit && item.HIAllowedPrice.GetValueOrDefault(0) > 0)
                {
                    item.HIBenefit = aRegistration.PtInsuranceBenefit;
                    item.ChangeHIBenefit(item.HIBenefit.GetValueOrDefault(0), aRegistration, null, aOnlyRoundResultForOutward);
                    if (item.PaidTime != null)
                    {
                        aRegistration.BalanceTransactionDetailByInvoiceItem(item);
                    }
                }
                item.PaidTime = null;
            }
        }
        public static void BalanceTransactionDetailByInvoiceItem(this PatientRegistration aRegistration, IInvoiceItem aIInvoiceItem)
        {
            if (aRegistration.PatientTransaction == null || aRegistration.PatientTransaction.PatientTransactionDetails == null || aIInvoiceItem == null || aIInvoiceItem.PaidTime == null) return;
            List<PatientTransactionDetail> mTransactionDetail = null;
            if (aIInvoiceItem is PatientRegistrationDetail)
            {
                mTransactionDetail = aRegistration.PatientTransaction.PatientTransactionDetails.Where(x => x.PtRegDetailID == aIInvoiceItem.ID && x.Amount > 0).ToList();
            }
            if (aIInvoiceItem is PatientPCLRequest)
            {
                mTransactionDetail = aRegistration.PatientTransaction.PatientTransactionDetails.Where(x => x.PCLRequestID == aIInvoiceItem.ID && x.Amount > 0).ToList();
            }
            if (aIInvoiceItem is PatientPCLRequestDetail)
            {
                mTransactionDetail = aRegistration.PatientTransaction.PatientTransactionDetails.Where(x => x.PCLRequestID == (aIInvoiceItem as PatientPCLRequestDetail).PatientPCLReqID && x.Amount > 0).ToList();
            }
            if (aIInvoiceItem is OutwardDrug)
            {
                mTransactionDetail = aRegistration.PatientTransaction.PatientTransactionDetails.Where(x => x.outiID == (aIInvoiceItem as OutwardDrug).outiID && x.Amount > 0).ToList();
            }
            if (mTransactionDetail == null || mTransactionDetail.Count == 0)
            {
                return;
            }
            decimal BalanceAmountCoPay = 0;
            decimal BalancePriceDifference = 0;
            decimal BalanceInsuranceRebate = 0;
            if (aIInvoiceItem is PatientPCLRequestDetail)
            {
                BalanceAmountCoPay = aRegistration.PCLRequests.SelectMany(x => x.PatientPCLRequestIndicators).Where(x => x.PatientPCLReqID == (aIInvoiceItem as PatientPCLRequestDetail).PatientPCLReqID).Sum(x => x.TotalCoPayment) - mTransactionDetail.Sum(x => x.AmountCoPay.GetValueOrDefault(0));
                BalancePriceDifference = aRegistration.PCLRequests.SelectMany(x => x.PatientPCLRequestIndicators).Where(x => x.PatientPCLReqID == (aIInvoiceItem as PatientPCLRequestDetail).PatientPCLReqID).Sum(x => x.TotalPriceDifference) - mTransactionDetail.Sum(x => x.PriceDifference.GetValueOrDefault(0));
                BalanceInsuranceRebate = aRegistration.PCLRequests.SelectMany(x => x.PatientPCLRequestIndicators).Where(x => x.PatientPCLReqID == (aIInvoiceItem as PatientPCLRequestDetail).PatientPCLReqID).Sum(x => x.TotalHIPayment) - mTransactionDetail.Sum(x => x.HealthInsuranceRebate.GetValueOrDefault(0));
            }
            else if (aIInvoiceItem is OutwardDrug)
            {
                BalanceAmountCoPay = aRegistration.DrugInvoices.SelectMany(x => x.OutwardDrugs).Where(x => x.outiID == (aIInvoiceItem as OutwardDrug).outiID).Sum(x => x.TotalCoPayment) - mTransactionDetail.Sum(x => x.AmountCoPay.GetValueOrDefault(0));
                BalancePriceDifference = aRegistration.DrugInvoices.SelectMany(x => x.OutwardDrugs).Where(x => x.outiID == (aIInvoiceItem as OutwardDrug).outiID).Sum(x => x.TotalPriceDifference) - mTransactionDetail.Sum(x => x.PriceDifference.GetValueOrDefault(0));
                BalanceInsuranceRebate = aRegistration.DrugInvoices.SelectMany(x => x.OutwardDrugs).Where(x => x.outiID == (aIInvoiceItem as OutwardDrug).outiID).Sum(x => x.TotalHIPayment) - mTransactionDetail.Sum(x => x.HealthInsuranceRebate.GetValueOrDefault(0));
            }
            else
            {
                BalanceAmountCoPay = aIInvoiceItem.TotalCoPayment - mTransactionDetail.Sum(x => x.AmountCoPay.GetValueOrDefault(0));
                BalancePriceDifference = aIInvoiceItem.TotalPriceDifference - mTransactionDetail.Sum(x => x.PriceDifference.GetValueOrDefault(0));
                BalanceInsuranceRebate = aIInvoiceItem.TotalHIPayment - mTransactionDetail.Sum(x => x.HealthInsuranceRebate.GetValueOrDefault(0));
            }
            if (BalanceAmountCoPay != 0 || BalancePriceDifference != 0 || BalanceInsuranceRebate != 0)
            {
                var BalanceTransactionDetail = mTransactionDetail.LastOrDefault().EntityDeepCopy();
                BalanceTransactionDetail.IsPaided = false;
                BalanceTransactionDetail.TransItemID = 0;
                BalanceTransactionDetail.AmountCoPay = BalanceAmountCoPay;
                BalanceTransactionDetail.PriceDifference = BalancePriceDifference;
                BalanceTransactionDetail.HealthInsuranceRebate = BalanceInsuranceRebate;
                BalanceTransactionDetail.Amount = 0;
                aRegistration.PatientTransaction.PatientTransactionDetails.Add(BalanceTransactionDetail);
            }
        }
        public static IList<IInvoiceItem> GetAllPayableInvoiceItems(this PatientRegistration aRegistration, bool aOnlyRoundResultForOutward)
        {
            List<IInvoiceItem> mAllInvoiceItems = new List<IInvoiceItem>();
            if (aRegistration.PatientRegistrationDetails != null)
            {
                mAllInvoiceItems.AddRange(aRegistration.PatientRegistrationDetails.Where(item => item.RecordState != RecordState.DELETED));
            }
            if (aRegistration.PCLRequests != null)
            {
                foreach (var pcldetails in aRegistration.PCLRequests)
                {
                    if (pcldetails.PatientPCLRequestIndicators != null)
                    {
                        mAllInvoiceItems.AddRange(pcldetails.PatientPCLRequestIndicators.Where(item => item.RecordState != RecordState.DELETED));
                    }
                }
            }
            if (aRegistration.DrugInvoices != null)
            {
                if (!aOnlyRoundResultForOutward)
                {
                    foreach (var invoice in aRegistration.DrugInvoices)
                    {
                        if (invoice.OutwardDrugs != null)
                        {
                            mAllInvoiceItems.AddRange(invoice.OutwardDrugs);
                        }
                        if (invoice.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED)
                        {
                            foreach (var item in invoice.OutwardDrugs)
                            {
                                item.TotalInvoicePrice = -Math.Abs(item.TotalInvoicePrice);
                                item.TotalPriceDifference = -Math.Abs(item.TotalPriceDifference);
                                item.TotalHIPayment = -Math.Abs(item.TotalHIPayment);
                                item.TotalCoPayment = -Math.Abs(item.TotalCoPayment);
                                item.TotalPatientPayment = -Math.Abs(item.TotalPatientPayment);
                                item.InvoicePrice = -Math.Abs(item.InvoicePrice);
                                if (item.HIAllowedPrice.HasValue)
                                {
                                    item.HIAllowedPrice = -Math.Abs(item.HIAllowedPrice.Value);
                                }
                            }
                        }
                        if (invoice.ReturnedInvoices == null || invoice.ReturnedInvoices.Count <= 0) continue;
                        foreach (var returnedInv in invoice.ReturnedInvoices)
                        {
                            if (returnedInv.OutwardDrugs == null || returnedInv.OutwardDrugs.Count <= 0) continue;
                            foreach (var item in returnedInv.OutwardDrugs)
                            {
                                item.TotalInvoicePrice = -Math.Abs(item.TotalInvoicePrice);
                                item.TotalPriceDifference = -Math.Abs(item.TotalPriceDifference);
                                item.TotalHIPayment = -Math.Abs(item.TotalHIPayment);
                                item.TotalCoPayment = -Math.Abs(item.TotalCoPayment);
                                item.TotalPatientPayment = -Math.Abs(item.TotalPatientPayment);
                                item.InvoicePrice = -Math.Abs(item.InvoicePrice);
                                if (item.HIAllowedPrice.HasValue)
                                {
                                    item.HIAllowedPrice = -Math.Abs(item.HIAllowedPrice.Value);
                                }
                            }
                            mAllInvoiceItems.AddRange(returnedInv.OutwardDrugs);
                        }
                    }
                }
                else
                {
                    foreach (var invoice in aRegistration.DrugInvoices)
                    {
                        if (invoice.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED)
                        {
                            foreach (var item in invoice.OutwardDrugs)
                            {
                                item.TotalInvoicePrice = -Math.Abs(item.TotalInvoicePrice);
                                item.TotalPriceDifference = -Math.Abs(item.TotalPriceDifference);
                                item.TotalHIPayment = -Math.Abs(item.TotalHIPayment);
                                item.TotalCoPayment = -Math.Abs(item.TotalCoPayment);
                                item.TotalPatientPayment = -Math.Abs(item.TotalPatientPayment);
                                item.InvoicePrice = -Math.Abs(item.InvoicePrice);
                                if (item.HIAllowedPrice.HasValue)
                                {
                                    item.HIAllowedPrice = -Math.Abs(item.HIAllowedPrice.Value);
                                }
                            }
                        }
                        decimal TotalHIAllowedPrice = 0;
                        if (invoice.OutwardDrugs != null)
                        {
                            OutwardDrug sumOutwardDrug = new OutwardDrug
                            {
                                ExamRegStatus = invoice.ExamRegStatus
                            };
                            sumOutwardDrug.TotalInvoicePrice = Math.Round(invoice.OutwardDrugs.Sum(x => x.TotalInvoicePrice), MidpointRounding.AwayFromZero);
                            sumOutwardDrug.TotalPriceDifference = invoice.OutwardDrugs.Sum(x => x.TotalPriceDifference);
                            sumOutwardDrug.TotalHIPayment = Math.Round(invoice.OutwardDrugs.Sum(x => x.TotalHIPayment), MidpointRounding.AwayFromZero);
                            TotalHIAllowedPrice = invoice.OutwardDrugs.Sum(x => x.HIAllowedPrice.GetValueOrDefault() * x.OutQuantity);
                            sumOutwardDrug.TotalCoPayment = TotalHIAllowedPrice - sumOutwardDrug.TotalHIPayment;
                            sumOutwardDrug.TotalPatientPayment = sumOutwardDrug.TotalInvoicePrice - sumOutwardDrug.TotalHIPayment;
                            mAllInvoiceItems.Add(sumOutwardDrug);
                        }
                        if (invoice.ReturnedInvoices == null || invoice.ReturnedInvoices.Count <= 0) continue;
                        foreach (var returnedInv in invoice.ReturnedInvoices)
                        {
                            if (returnedInv.OutwardDrugs == null || returnedInv.OutwardDrugs.Count <= 0) continue;
                            TotalHIAllowedPrice = 0;
                            OutwardDrug sumReturnOutwardDrug = new OutwardDrug();
                            sumReturnOutwardDrug.TotalInvoicePrice = Math.Round(returnedInv.OutwardDrugs.Sum(x => x.TotalInvoicePrice), MidpointRounding.AwayFromZero);
                            sumReturnOutwardDrug.TotalPriceDifference = returnedInv.OutwardDrugs.Sum(x => x.TotalPriceDifference);
                            sumReturnOutwardDrug.TotalHIPayment = Math.Round(returnedInv.OutwardDrugs.Sum(x => x.TotalHIPayment), MidpointRounding.AwayFromZero);
                            TotalHIAllowedPrice = returnedInv.OutwardDrugs.Sum(x => x.HIAllowedPrice.GetValueOrDefault() * x.OutQuantity);
                            sumReturnOutwardDrug.TotalCoPayment = TotalHIAllowedPrice - sumReturnOutwardDrug.TotalHIPayment;
                            sumReturnOutwardDrug.TotalPatientPayment = sumReturnOutwardDrug.TotalInvoicePrice - sumReturnOutwardDrug.TotalHIPayment;
                            sumReturnOutwardDrug.TotalInvoicePrice = -Math.Abs(sumReturnOutwardDrug.TotalInvoicePrice);
                            sumReturnOutwardDrug.TotalPriceDifference = -Math.Abs(sumReturnOutwardDrug.TotalPriceDifference);
                            sumReturnOutwardDrug.TotalHIPayment = -Math.Abs(sumReturnOutwardDrug.TotalHIPayment);
                            sumReturnOutwardDrug.TotalCoPayment = -Math.Abs(sumReturnOutwardDrug.TotalCoPayment);
                            sumReturnOutwardDrug.TotalPatientPayment = -Math.Abs(sumReturnOutwardDrug.TotalPatientPayment);
                            mAllInvoiceItems.Add(sumReturnOutwardDrug);
                        }
                    }
                }
            }
            if (aRegistration.InPatientInvoices != null)
            {
                foreach (var invoice in aRegistration.InPatientInvoices)
                {
                    if (invoice.OutwardDrugClinicDepts != null)
                    {
                        mAllInvoiceItems.AddRange(invoice.OutwardDrugClinicDepts.Where(item => item.RecordState != RecordState.DELETED));
                    }
                }
            }
            return mAllInvoiceItems;
        }
        public static void CalcPayableSum(this PatientRegistration aRegistration, bool aEnableOutPtCashAdvance, bool aOnlyRoundResultForOutward)
        {
            aRegistration.PayableSum = new PayableSum();
            foreach (var item in aRegistration.GetAllPayableInvoiceItems(aOnlyRoundResultForOutward))
            {
                if (item.RecordState == RecordState.DELETED || item.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                {
                    continue;
                }
                aRegistration.PayableSum.TotalCoPayment += item.TotalCoPayment;
                aRegistration.PayableSum.TotalHIPayment += item.TotalHIPayment;
                aRegistration.PayableSum.TotalPrice += item.TotalInvoicePrice;
                aRegistration.PayableSum.TotalPatientPayment += item.TotalPatientPayment;
                aRegistration.PayableSum.TotalPriceDifference += item.TotalPriceDifference;
                aRegistration.PayableSum.TotalDiscountAmount += item.DiscountAmt;
                aRegistration.PayableSum.TotalDiscountedAmount += item.PaidTime.HasValue && item.PaidTime != null ? item.DiscountAmt : 0;
            }
            if (aRegistration.PatientTransaction != null && aRegistration.PatientTransaction.PatientTransactionDetails != null && aRegistration.PatientTransaction.PatientTransactionPayments != null)
            {
                aRegistration.PayableSum.TotalPatientPaid = aRegistration.PatientTransaction.PatientTransactionPayments.Where(x => !x.IsDeleted.GetValueOrDefault()).Sum(x => x.PayAmount * x.CreditOrDebit);
                aRegistration.PayableSum.TotalPaymentForTransaction = aRegistration.PatientTransaction.PatientTransactionDetails.Sum(x => x.Amount - (x.HealthInsuranceRebate == null ? 0 : x.HealthInsuranceRebate)).GetValueOrDefault();
            }
            else
            {
                aRegistration.PayableSum.TotalPatientPaid = 0;
                aRegistration.PayableSum.TotalPaymentForTransaction = 0;
            }
            if (aEnableOutPtCashAdvance && aRegistration.PatientTransaction != null && aRegistration.PatientTransaction.PatientCashAdvances != null && aRegistration.PatientTransaction.PatientCashAdvances.Count > 0)
            {
                aRegistration.PayableSum.TotalPatientCashAdvance = aRegistration.PatientTransaction.PatientCashAdvances.Where(x => !x.IsDeleted && (aRegistration.ConfirmHIStaffID.GetValueOrDefault(0) > 0 || x.BalanceAmount > 0)).Sum(x => x.BalanceAmount);
                if (!aRegistration.PatientTransaction.PatientCashAdvances.Any(x => !x.IsDeleted))
                {
                    aRegistration.PayableSum.TotalCashAdvanceAmount = 0;
                }
                else
                {
                    aRegistration.PayableSum.TotalCashAdvanceAmount = aRegistration.PatientTransaction.PatientCashAdvances.Where(x => !x.IsDeleted).Sum(x => x.PaymentAmount);
                }
            }
            aRegistration.PayableSum.TotalPatientRemainingOwed = aRegistration.PayableSum.TotalPatientPayment - aRegistration.PayableSum.TotalDiscountAmount - aRegistration.PayableSum.TotalPatientPaid;
            aRegistration.PayableSum.TotalAmtOutwardDrugInvoices = 0;
            aRegistration.PayableSum.TotalPaidForOutwardDrugInvoices = 0;
            aRegistration.PayableSum.TotalAmtRegDetailServices = 0;
            aRegistration.PayableSum.TotalPaidForRegDetailServices = 0;
            aRegistration.PayableSum.TotalAmtRegPCLRequests = 0;
            aRegistration.PayableSum.TotalPaidForRegPCLRequests = 0;
            if (aRegistration.PatientTransaction != null && aRegistration.PatientTransaction.PatientTransactionDetails != null && aRegistration.PatientTransaction.PatientTransactionDetails.Count() > 0)
            {
                //var mTransactionDetails = aRegistration.PatientTransaction.PatientTransactionDetails.Where(x => !aEnableOutPtCashAdvance || x.Amount > 0 || aRegistration.ConfirmHIStaffID.GetValueOrDefault(0) > 0);
                var mTransactionDetails = aRegistration.PatientTransaction.PatientTransactionDetails;
                var all_ODI_Trans = mTransactionDetails.Where(x => x.outiID > 0).ToList();
                foreach (var odiTran in all_ODI_Trans)
                {
                    if (odiTran.Amount < 0)
                    {
                        bool bIs_Return_ODI_Tran = true;
                        foreach (var tmpTran in all_ODI_Trans)
                        {
                            if (odiTran.TransItemID != tmpTran.TransItemID && odiTran.outiID == tmpTran.outiID)
                            {
                                bIs_Return_ODI_Tran = false;
                                break;
                            }
                        }
                        if (bIs_Return_ODI_Tran && odiTran.IsPaided == true)
                        {
                            continue;
                        }
                    }
                    aRegistration.PayableSum.TotalAmtOutwardDrugInvoices += odiTran.Amount - (odiTran.HealthInsuranceRebate.HasValue ? odiTran.HealthInsuranceRebate.Value : 0);
                    if (aRegistration.PatientTransaction.PatientTransactionPayments != null && aRegistration.PatientTransaction.PatientTransactionPayments.Count() > 0)
                    {
                        foreach (var odiTranPaymt in aRegistration.PatientTransaction.PatientTransactionPayments)
                        {
                            if (odiTranPaymt.TransItemID == odiTran.TransItemID && odiTranPaymt.PtPmtAccID != 2 && (odiTranPaymt.IsDeleted == null || odiTranPaymt.IsDeleted == false))
                            {
                                aRegistration.PayableSum.TotalPaidForOutwardDrugInvoices += odiTranPaymt.PayAmount * odiTranPaymt.CreditOrDebit;
                            }
                        }
                    }
                    if (aRegistration.PatientTransaction.PatientCashAdvances != null && aRegistration.PatientTransaction.PatientCashAdvances.Count() > 0 && aRegistration.PatientTransaction.PatientCashAdvances.Any(x => x.OutPatientCashAdvanceLinks != null && x.OutPatientCashAdvanceLinks.Any(y => y.TransItemID == odiTran.TransItemID)))
                    {
                        aRegistration.PayableSum.TotalPaidForOutwardDrugInvoices += (odiTran.Amount - odiTran.HealthInsuranceRebate.GetValueOrDefault(0) - odiTran.DiscountAmt.GetValueOrDefault(0) - odiTran.OtherAmt.GetValueOrDefault(0));
                    }
                }
                var all_RegDetails_Trans = mTransactionDetails.Where(x => x.PtRegDetailID > 0).ToList();
                foreach (var regDetailTran in all_RegDetails_Trans)
                {
                    aRegistration.PayableSum.TotalAmtRegDetailServices += regDetailTran.Amount - (regDetailTran.HealthInsuranceRebate.HasValue ? regDetailTran.HealthInsuranceRebate.Value : 0);
                    foreach (var regDetTranPaymt in aRegistration.PatientTransaction.PatientTransactionPayments)
                    {
                        if (regDetTranPaymt.TransItemID == regDetailTran.TransItemID && regDetTranPaymt.PtPmtAccID != 2 && (regDetTranPaymt.IsDeleted == null || regDetTranPaymt.IsDeleted == false))
                        {
                            aRegistration.PayableSum.TotalPaidForRegDetailServices += regDetTranPaymt.PayAmount;
                        }
                    }
                    if (aRegistration.PatientTransaction.PatientCashAdvances != null && aRegistration.PatientTransaction.PatientCashAdvances.Count() > 0 && aRegistration.PatientTransaction.PatientCashAdvances.Any(x => x.OutPatientCashAdvanceLinks != null && x.OutPatientCashAdvanceLinks.Any(y => y.TransItemID == regDetailTran.TransItemID)))
                    {
                        aRegistration.PayableSum.TotalPaidForRegDetailServices += (regDetailTran.Amount - regDetailTran.HealthInsuranceRebate.GetValueOrDefault(0) - regDetailTran.DiscountAmt.GetValueOrDefault(0) - regDetailTran.OtherAmt.GetValueOrDefault(0));
                    }
                }
                var all_PCL_Trans = mTransactionDetails.Where(x => x.PCLRequestID > 0).ToList();
                foreach (var pclTran in all_PCL_Trans)
                {
                    aRegistration.PayableSum.TotalAmtRegPCLRequests += pclTran.Amount - (pclTran.HealthInsuranceRebate.HasValue ? pclTran.HealthInsuranceRebate.Value : 0);
                    foreach (var pclTranPaymt in aRegistration.PatientTransaction.PatientTransactionPayments)
                    {
                        if (pclTranPaymt.TransItemID == pclTran.TransItemID && pclTranPaymt.PtPmtAccID != 2 && (pclTranPaymt.IsDeleted == null || pclTranPaymt.IsDeleted == false))
                        {
                            aRegistration.PayableSum.TotalPaidForRegPCLRequests += pclTranPaymt.PayAmount;
                        }
                    }
                    if (aRegistration.PatientTransaction.PatientCashAdvances != null && aRegistration.PatientTransaction.PatientCashAdvances.Count() > 0 && aRegistration.PatientTransaction.PatientCashAdvances.Any(x => x.OutPatientCashAdvanceLinks != null && x.OutPatientCashAdvanceLinks.Any(y => y.TransItemID == pclTran.TransItemID)))
                    {
                        aRegistration.PayableSum.TotalPaidForRegPCLRequests += (pclTran.Amount - pclTran.HealthInsuranceRebate.GetValueOrDefault(0) - pclTran.DiscountAmt.GetValueOrDefault(0) - pclTran.OtherAmt.GetValueOrDefault(0));
                    }
                }
            }
        }
        public static void CorrectRegistrationDetails_V2(this PatientRegistration CurrentRegistration, bool SpecialRuleForHIConsultationApplied, DateTime? CurrentServerDateTime,
            float[] HIPercentOnDifDept, long HiPolicyMinSalary, bool OnlyRoundResultForOutward, decimal AddingServicesPercent, decimal AddingHIServicesPercent, bool FullHIOfServicesForConfirm,
            float PercentForEkip, float PercentForOtherEkip)
        {
            if (CurrentRegistration != null && CurrentRegistration.AllSaveRegistrationDetails != null && CurrentRegistration.AllSaveRegistrationDetails.Count() > 0)
            {
                CalcSpecialist(CurrentRegistration, CurrentServerDateTime, HIPercentOnDifDept, OnlyRoundResultForOutward);
                CalcPaymentPercent(CurrentRegistration, CurrentServerDateTime, HIPercentOnDifDept, OnlyRoundResultForOutward, HiPolicyMinSalary, FullHIOfServicesForConfirm, AddingServicesPercent, AddingHIServicesPercent);
                CalcEkip(CurrentRegistration, PercentForEkip, PercentForOtherEkip);
                foreach (PatientRegistrationDetail item in CurrentRegistration.AllSaveRegistrationDetails)
                {
                    if (item.IsDiscounted)
                    {
                        CalcDiscount(CurrentRegistration, item as MedRegItemBase, OnlyRoundResultForOutward, false);
                    }
                    else
                    {
                        CalcDiscount(CurrentRegistration, item as MedRegItemBase, OnlyRoundResultForOutward, true);
                    }
                }
            }
        }
        //Tính chuyên khoa
        private static void CalcSpecialist(PatientRegistration CurrentRegistration, DateTime? aCurrentServerDateTime, float[] aHIPercentOnDifDept, bool aOnlyRoundResultForOutward, bool aDetectHiApplied = false)
        {
            IList<PatientRegistrationDetail> hiRegDetails = CurrentRegistration.PatientRegistrationDetails.Where(x =>
                    (x.RefMedicalServiceItem.RefMedicalServiceType != null && x.RefMedicalServiceItem.RefMedicalServiceType.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH)
                    && x.RecordState != RecordState.DELETED && !x.MarkedAsDeleted && x.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI
                    && x.HisID.HasValue && x.HisID.Value > 0 && x.HIAllowedPrice.HasValue && x.HIAllowedPrice.Value > 0).ToList();
            if (hiRegDetails == null || hiRegDetails.Count == 0)
            {
                return;
            }
            List<List<PatientRegistrationDetail>> SpecialList = new List<List<PatientRegistrationDetail>>();
            bool bFindCK = true;
            do
            {
                if (bFindCK) //Thêm vào danh sách chuyên khoa của dịch vụ có giá BH lớn nhất
                {
                    PatientRegistrationDetail MaxDetail = new PatientRegistrationDetail();
                    foreach (PatientRegistrationDetail item in hiRegDetails)
                    {
                        if (MaxDetail.HIAllowedPrice.GetValueOrDefault() < item.HIAllowedPrice)
                        {
                            MaxDetail = item;
                        }
                    }
                    SpecialList.Add(new List<PatientRegistrationDetail> { MaxDetail });
                    hiRegDetails.Remove(MaxDetail);
                    bFindCK = false;
                }
                else //Thêm vào danh sách dịch vụ của chuyên khoa
                {
                    foreach (PatientRegistrationDetail item in hiRegDetails.ToList())
                    {
                        foreach (List<PatientRegistrationDetail> detail in SpecialList)
                        {
                            foreach (PatientRegistrationDetail PtRegDetail in detail)
                            {
                                if (item.RefMedicalServiceItem != null && PtRegDetail.RefMedicalServiceItem != null && item.RefMedicalServiceItem.V_SpecialistType == PtRegDetail.RefMedicalServiceItem.V_SpecialistType)
                                {
                                    detail.Add(item);
                                    hiRegDetails.Remove(item);
                                }
                                break;
                            }
                        }
                        if (hiRegDetails.Count == 0)
                        {
                            break;
                        }
                    }
                    bFindCK = true;
                }
            } while (hiRegDetails.Count > 0);
            if (aHIPercentOnDifDept != null)
            {
                List<long> SpecialCollection = new List<long>();
                //foreach (PatientRegistrationDetail aItem in hiRegDetails.OrderByDescending(x => x.HIAllowedPrice))
                foreach (List<PatientRegistrationDetail> ListPtRegDetail in SpecialList)
                {
                    foreach (PatientRegistrationDetail aItem in ListPtRegDetail)
                    {
                        if ((aItem.RefMedicalServiceItem != null && aItem.RefMedicalServiceItem.V_SpecialistType > 0 && SpecialCollection.Contains(aItem.RefMedicalServiceItem.V_SpecialistType))
                            || (aItem.RefMedicalServiceItem == null && aItem.RefMedicalServiceItem.V_SpecialistType == 0))
                        {
                            //Không tính quyền lợi BHYT cho các dịch vụ không có chuyên khoa hoặc bị trùng chuyên khoa với các dịch vụ trước
                            aItem.ApplyHIPaymentPercent(0, CurrentRegistration, aCurrentServerDateTime, aOnlyRoundResultForOutward, aDetectHiApplied);
                        }
                        else
                        {
                            if (SpecialCollection.Count > 0)
                            {
                                if (aHIPercentOnDifDept.Length > SpecialCollection.Count - 1)
                                {
                                    //Tính quyền lợi BHYT theo tỉ lệ cấu hình cho dịch vụ khác chuyên khoa theo thứ tự
                                    aItem.ApplyHIPaymentPercent(Math.Round(aHIPercentOnDifDept[SpecialCollection.Count - 1], 4), CurrentRegistration, aCurrentServerDateTime, aOnlyRoundResultForOutward, aDetectHiApplied);
                                }
                                else
                                {
                                    //Không tính quyền lợi BHYT cho Dịch vụ vượt quá số lượng chuyên khoa cho phép một lần khám
                                    aItem.ApplyHIPaymentPercent(0, CurrentRegistration, aCurrentServerDateTime, aOnlyRoundResultForOutward, aDetectHiApplied);
                                }
                            }
                            else
                            {
                                aItem.ChangeHIBenefit(aItem.HIBenefit.GetValueOrDefault(0), CurrentRegistration, aCurrentServerDateTime, aOnlyRoundResultForOutward, aDetectHiApplied);
                                aItem.ApplyHIPaymentPercent(1.0, CurrentRegistration, aCurrentServerDateTime, aOnlyRoundResultForOutward, aDetectHiApplied);
                            }
                            SpecialCollection.Add(aItem.RefMedicalServiceItem.V_SpecialistType);
                        }
                    }
                }
            }
            else
            {
                foreach (PatientRegistrationDetail registrationDetail in hiRegDetails)
                {
                    registrationDetail.ChangeHIBenefit();
                }
            }
        }
        //Tính tỷ lệ dịch vụ
        private static void CalcPaymentPercent(PatientRegistration CurrentRegistration, DateTime? aCurrentServerDateTime, float[] aHIPercentOnDifDept, bool aOnlyRoundResultForOutward,
            long aHIPolicyMinSalary, bool FullHIOfServicesForConfirm, decimal AddingServicesPercent, decimal AddingHIServicesPercent, bool aDetectHiApplied = false)
        {
            //Nếu cả 2 cấu hình đều = 1 thì không cần tính
            if (AddingHIServicesPercent == 1 && AddingServicesPercent == 1)
            {
                return;
            }
            PatientRegistrationDetail maxItem = new PatientRegistrationDetail();
            //Danh sách dịch vụ có dịch vụ BH
            IList<PatientRegistrationDetail> hiRegDetails = CurrentRegistration.AllSaveRegistrationDetails.Where(x => x.HIPayment > 0 && x.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI
                                                            && x.RecordState != RecordState.DELETED && !x.MarkedAsDeleted && x.RefMedicalServiceItem.RefMedicalServiceType != null
                                                            && x.RefMedicalServiceItem.RefMedicalServiceType.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH).ToList();
            //Danh sách dịch vụ không có dịch vụ BH
            IList<PatientRegistrationDetail> RegDetails = CurrentRegistration.AllSaveRegistrationDetails.Where(x => x.HIPayment == 0 && x.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI
                                                && x.RecordState != RecordState.DELETED && !x.MarkedAsDeleted && x.RefMedicalServiceItem.RefMedicalServiceType != null
                                                && x.RefMedicalServiceItem.RefMedicalServiceType.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH).ToList();
            if (hiRegDetails.Count > 0)
            {
                foreach (PatientRegistrationDetail item in hiRegDetails)
                {
                    if (maxItem.HIAllowedPrice.GetValueOrDefault() < item.HIAllowedPrice)
                    {
                        maxItem = item;
                    }
                }
            }
            else if (RegDetails.Count > 0)
            {
                foreach (PatientRegistrationDetail item in RegDetails)
                {
                    if (maxItem.RefMedicalServiceItem == null)
                    {
                        maxItem = item;
                    }
                    else if (item.ChargeableItem.NormalPrice > 0 && maxItem.ChargeableItem.NormalPrice < item.ChargeableItem.NormalPrice)
                    {
                        maxItem = item;
                    }
                }
            }
            foreach (PatientRegistrationDetail item in CurrentRegistration.AllSaveRegistrationDetails.Where(x => x.RefMedicalServiceItem.RefMedicalServiceType != null
                    && x.RefMedicalServiceItem.RefMedicalServiceType.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH
                    && x.RecordState != RecordState.DELETED && !x.MarkedAsDeleted && x.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI).ToList())
            {
                //▼===== 20200729 TTM: Lưu trữ lại thông tin PaymentPercent của dịch vụ, nếu có thay đổi thì set lại RecordState
                double OldPaymentPercent = item.PaymentPercent;
                //▲=====
                if (item != maxItem && (CurrentRegistration.Appointment == null || CurrentRegistration.Appointment.V_AppointmentType != (long)AllLookupValues.AppointmentType.HEN_KHAM_SUC_KHOE)
                    && (CurrentRegistration.PatientClassification == null || CurrentRegistration.PatientClassification.PatientClassID != (long)AllLookupValues.PatientClassification.KHAM_SUC_KHOE)) //====: #012
                {
                    if (item.HIAllowedPrice > 0 && CurrentRegistration.PtInsuranceBenefit.GetValueOrDefault(0) > 0 && item.HIPaymentPercent > 0)
                    {
                        item.PaymentPercent = (double)AddingHIServicesPercent;
                    }
                    else
                    {
                        item.PaymentPercent = (double)AddingServicesPercent;
                    }
                    item.GetItemPrice(CurrentRegistration, item.HIBenefit.GetValueOrDefault(0), aCurrentServerDateTime, aDetectHiApplied, false, aHIPolicyMinSalary);
                    item.GetItemTotalPrice(aOnlyRoundResultForOutward);
                }
                //else if (item.PaymentPercent != 1 && item == maxItem)
                else if (item == maxItem)
                {
                    item.PaymentPercent = 1;
                    item.GetItemPrice(CurrentRegistration, item.HIBenefit.GetValueOrDefault(0), aCurrentServerDateTime, aDetectHiApplied, false, aHIPolicyMinSalary);
                    item.GetItemTotalPrice(aOnlyRoundResultForOutward);
                }
                //▼===== 20200729 TTM: Set lại recordstate khi thay đổi thông tin PaymentPercent và tình trạng dịch vụ là unchanged
                if (item.RecordState == RecordState.UNCHANGED && OldPaymentPercent != item.PaymentPercent)
                {
                    item.RecordState = RecordState.MODIFIED;
                }
                //▲=====
            }
            if (FullHIOfServicesForConfirm)
            {
                foreach (var item in CurrentRegistration.AllSaveRegistrationDetails.Where(x => x.HIAllowedPrice > 0
                        && x.HIBenefit != 1
                        && x.RefMedicalServiceItem != null
                        && x.RefMedicalServiceItem.RefMedicalServiceType != null
                        && x.RefMedicalServiceItem.RefMedicalServiceType.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH
                        && x.RecordState != RecordState.UNCHANGED))
                {
                    if (item.HisID.GetValueOrDefault() > 0)
                    {
                        item.HIBenefit = 1;
                    }
                    item.ChangeHIBenefit(item.HIBenefit.GetValueOrDefault(0), CurrentRegistration, aCurrentServerDateTime, aOnlyRoundResultForOutward);
                }
            }
        }
        //Tính Ekip
        private static void CalcEkip(PatientRegistration CurrentRegistration, float PercentForEkip, float PercentForOtherEkip)
        {
            if (CurrentRegistration != null && CurrentRegistration.PatientRegistrationDetails != null)
            {
                foreach (PatientRegistrationDetail detail in CurrentRegistration.PatientRegistrationDetails.Where(x => x.RefMedicalServiceItem.RefMedicalServiceType != null && x.RefMedicalServiceItem.RefMedicalServiceType.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.THUTHUAT))
                {
                    if (detail.V_EkipIndex != null && detail.V_EkipIndex.LookupID == (long)AllLookupValues.V_EkipIndex.CungEkip)
                    {
                        detail.TotalHIPayment = detail.HIAllowedPrice.GetValueOrDefault() * detail.Qty * (decimal)detail.HIBenefit.GetValueOrDefault() * (decimal)Math.Round(PercentForEkip, 2);
                        detail.HIPaymentPercent = Math.Round(PercentForEkip, 2);
                        detail.TotalPatientPayment = detail.InvoicePrice - detail.TotalHIPayment;
                    }
                    else if (detail.V_EkipIndex != null && detail.V_EkipIndex.LookupID == (long)AllLookupValues.V_EkipIndex.KhacEkip)
                    {
                        detail.TotalHIPayment = detail.HIAllowedPrice.GetValueOrDefault() * detail.Qty * (decimal)detail.HIBenefit.GetValueOrDefault() * (decimal)Math.Round(PercentForOtherEkip, 2);
                        detail.HIPaymentPercent = Math.Round(PercentForOtherEkip, 2);
                        detail.TotalPatientPayment = detail.InvoicePrice - detail.TotalHIPayment;
                    }
                    else
                    {
                        detail.TotalHIPayment = detail.HIAllowedPrice.GetValueOrDefault() * detail.Qty * (decimal)detail.HIBenefit.GetValueOrDefault();
                        detail.HIPaymentPercent = 1;
                        detail.TotalPatientPayment = detail.InvoicePrice - detail.TotalHIPayment;
                    }
                }
            }
        }
        //Tính miễn giảm
        public static void CalcDiscount(PatientRegistration CurrentRegistration, MedRegItemBase aInvoiceItem, bool aOnlyRoundResultForOutward, bool IsRemove)
        {
            var mPromoObj = CurrentRegistration.PromoDiscountProgramObj;
            if (aInvoiceItem.PromoDiscProgID.GetValueOrDefault(0) > 0 && CurrentRegistration.DiscountProgramCollection != null && CurrentRegistration.DiscountProgramCollection.Any(x => x.PromoDiscProgID == aInvoiceItem.PromoDiscProgID.Value))
            {
                mPromoObj = CurrentRegistration.DiscountProgramCollection.First(x => x.PromoDiscProgID == aInvoiceItem.PromoDiscProgID.Value);
            }
            if (aInvoiceItem == null || mPromoObj == null)
            {
                aInvoiceItem.DiscountAmt = 0;
                return;
            }
            decimal mDiscountAmt = aInvoiceItem.DiscountAmt * (IsRemove ? 0 : 1);
            switch (mPromoObj.V_DiscountTypeCount.LookupID)
            {
                case (long)AllLookupValues.V_DiscountTypeCount.All:
                    if (!mPromoObj.IsOnPriceDiscount)
                    {
                        //mDiscountAmt = Math.Round((aInvoiceItem.TotalPatientPayment + aInvoiceItem.DiscountAmt) * mPromoObj.DiscountPercent * (IsRemove ? 0 : 1), 0);
                        mDiscountAmt = Math.Round((aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - aInvoiceItem.OtherAmt) * mPromoObj.DiscountPercent * (IsRemove ? 0 : 1), 0);
                    }
                    if (aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - aInvoiceItem.OtherAmt - mDiscountAmt < 0)
                    {
                        mDiscountAmt = aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - aInvoiceItem.OtherAmt;
                    }
                    aInvoiceItem.TotalPatientPayment = aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - aInvoiceItem.OtherAmt - mDiscountAmt;
                    aInvoiceItem.DiscountAmt = mDiscountAmt;
                    break;
                case (long)AllLookupValues.V_DiscountTypeCount.PriceDifference:
                    if (!mPromoObj.IsOnPriceDiscount)
                    {
                        mDiscountAmt = Math.Round((aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - aInvoiceItem.TotalCoPayment - aInvoiceItem.OtherAmt) * mPromoObj.DiscountPercent * (IsRemove ? 0 : 1), 0);
                    }
                    if (aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - aInvoiceItem.TotalCoPayment - aInvoiceItem.OtherAmt - mDiscountAmt < 0)
                    {
                        mDiscountAmt = aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - aInvoiceItem.TotalCoPayment - aInvoiceItem.OtherAmt;
                    }
                    aInvoiceItem.TotalPatientPayment = aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - aInvoiceItem.OtherAmt - mDiscountAmt;
                    aInvoiceItem.DiscountAmt = mDiscountAmt;
                    break;
                case (long)AllLookupValues.V_DiscountTypeCount.AmountCoPay:
                    decimal mPriceDifference = aInvoiceItem.TotalHIPayment == 0 ? aInvoiceItem.TotalInvoicePrice : aInvoiceItem.TotalPriceDifference;
                    if (!mPromoObj.IsOnPriceDiscount)
                    {
                        mDiscountAmt = Math.Round((aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - mPriceDifference - aInvoiceItem.OtherAmt) * mPromoObj.DiscountPercent * (IsRemove ? 0 : 1), 0);
                    }
                    if (aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - mPriceDifference - aInvoiceItem.OtherAmt - mDiscountAmt < 0)
                    {
                        mDiscountAmt = aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - mPriceDifference - aInvoiceItem.OtherAmt;
                    }
                    aInvoiceItem.TotalPatientPayment = aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - aInvoiceItem.OtherAmt - mDiscountAmt;
                    aInvoiceItem.DiscountAmt = mDiscountAmt;
                    break;
                default:
                    if (!mPromoObj.IsOnPriceDiscount)
                    {
                        //mDiscountAmt = Math.Round((aInvoiceItem.TotalPatientPayment + aInvoiceItem.DiscountAmt) * mPromoObj.DiscountPercent * (IsRemove ? 0 : 1), 0);
                        mDiscountAmt = Math.Round((aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - aInvoiceItem.OtherAmt) * mPromoObj.DiscountPercent * (IsRemove ? 0 : 1), 0);
                    }
                    if (aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - aInvoiceItem.OtherAmt - mDiscountAmt < 0)
                    {
                        mDiscountAmt = aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - aInvoiceItem.OtherAmt;
                    }
                    aInvoiceItem.TotalPatientPayment = aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - aInvoiceItem.OtherAmt - mDiscountAmt;
                    aInvoiceItem.DiscountAmt = mDiscountAmt;
                    break;
            }
            //if (!mPromoObj.IsOnPriceDiscount)
            //{
            //    mDiscountAmt = Math.Round((aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment) * mPromoObj.DiscountPercent * (IsRemove ? 0 : 1), 0);
            //}
            //if (aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - mDiscountAmt < 0)
            //{
            //    mDiscountAmt = aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment;
            //}
            //aInvoiceItem.TotalPatientPayment = aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - aInvoiceItem.OtherAmt - mDiscountAmt;
            //aInvoiceItem.DiscountAmt = mDiscountAmt;
        }
    }
    [DataContract]
    public class PatientRegistration_V2 : PatientRegistration
    {
        private OutPtTransactionFinalization _PtTranFinalization;
        [DataMemberAttribute]
        public OutPtTransactionFinalization PtTranFinalization
        {
            get
            {
                return _PtTranFinalization;
            }
            set
            {
                _PtTranFinalization = value;
                RaisePropertyChanged("PtTranFinalization");
            }
        }
    }
}