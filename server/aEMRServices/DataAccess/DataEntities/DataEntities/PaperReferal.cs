using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using Service.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class PaperReferal : EntityBase, IEditableObject
    {
        public PaperReferal()
            : base()
        {
            _acceptedDate = DateTime.Now;
            
            CanEdit = true;
            CanDelete = true;
        }

        public void HospitalToValue()
        {
            if (Hospital != null)
            {
                IssuerLocation = Hospital.HosName;
                IssuerCode = Hospital.HICode;
                CityProvinceName = Hospital.CityProvinceName;
            }
        }
        private PaperReferal _tempObject;

        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempObject = (PaperReferal)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempObject)
                CopyFrom(_tempObject);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(PaperReferal p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        private long _RefID;
        [DataMemberAttribute()]
        public long RefID
        {
            get
            {
                return _RefID;
            }
            set
            {
                _RefID = value;
                RaisePropertyChanged("RefID");
            }
        }

        private long _hiId;
        [DataMemberAttribute()]
        public long HiId
        {
            get
            {
                return _hiId;
            }
            set
            {
                if (_hiId != value)
                {
                    _hiId = value;
                    RaisePropertyChanged("HiId");
                }
            }
        }

        private HealthInsurance _healthInsurance;
        [Required(ErrorMessage = "Chọn thẻ bảo hiểm")]
        [DataMemberAttribute()]
        public HealthInsurance HealthInsurance
        {
            get
            {
                return _healthInsurance;
            }
            set
            {
                if (_healthInsurance != value)
                {
                    ValidateProperty("HealthInsurance", value);
                    _healthInsurance = value;
                    RaisePropertyChanged("HealthInsurance");
                }
            }
        }

        /*TMA*/
        private long _TransferFormID;
        [DataMemberAttribute()]
        public long TransferFormID
        {
            get
            {
                return _TransferFormID;
            }
            set
            {
                if (_TransferFormID != value)
                {
                    _TransferFormID = value;
                    RaisePropertyChanged("TransferFormID"); 
                }
            }
        }

        private string _TransferNum;
        [DataMemberAttribute()]
        public string TransferNum
        {
            get
            {
                return _TransferNum;
            }
            set
            {
                if (_TransferNum != value)
                {
                    _TransferNum = value;
                    RaisePropertyChanged("TransferNum");
                }
            }
        }
        /*TMA*/

        private long _hospitalID;
        [DataMemberAttribute()]
        public long HospitalID
        {
            get
            {
                return _hospitalID;
            }
            set
            {
                if (_hospitalID != value)
                {
                    _hospitalID = value;
                    RaisePropertyChanged("HospitalID");
                }
            }
        }

        private Hospital _hospital;
        [Required(ErrorMessage = "Chọn bệnh viện")]
        [DataMemberAttribute()]
        public Hospital Hospital
        {
            get
            {
                return _hospital;
            }
            set
            {
                if (_hospital != value)
                {
                    ValidateProperty("Hospital", value);
                    _hospital = value;                    
                    RaisePropertyChanged("Hospital");
                }
            }
        }

        private DateTime? _refCreatedDate;
        [Required(ErrorMessage = "Nhập ngày ký giấy")]
        [CustomValidation(typeof(PaperReferal), "ValidateCreatedDate")]
        [DataMemberAttribute()]
        public DateTime? RefCreatedDate
        {
            get
            {
                return _refCreatedDate;
            }
            set
            {
                if (_refCreatedDate != value)
                {
                    ValidateProperty("RefCreatedDate", value);
                    _refCreatedDate = value;
                    RaisePropertyChanged("RefCreatedDate");  
                }
            }
        }

        private string _treatmentFaculty;
        [DataMemberAttribute()]
        public string TreatmentFaculty
        {
            get
            {
                return _treatmentFaculty;
            }
            set
            {
                if (_treatmentFaculty != value)
                {
                    _treatmentFaculty = value;
                    RaisePropertyChanged("TreatmentFaculty"); 
                }
            }
        }

        private string _generalDiagnoses;
        [DataMemberAttribute()]
        public string GeneralDiagnoses
        {
            get
            {
                return _generalDiagnoses;
            }
            set
            {
                if (_generalDiagnoses !=value)
                {
                    _generalDiagnoses = value;
                    RaisePropertyChanged("GeneralDiagnoses"); 
                }
            }
        }

        private string _currentStatusOfPt;
        [DataMemberAttribute()]
        public string CurrentStatusOfPt
        {
            get
            {
                return _currentStatusOfPt;
            }
            set
            {
                if (_currentStatusOfPt != value)
                {
                    _currentStatusOfPt = value;
                    RaisePropertyChanged("CurrentStatusOfPt"); 
                }
            }
        }

        private string _paperState = "GM";
        [DataMemberAttribute()]
        public string paperState
        {
            get
            {
                return _paperState;
            }
            set
            {
                if (_paperState != value)
                {
                    _paperState = value;
                    RaisePropertyChanged("paperState");
                }
            }
        }

        private DateTime? _acceptedDate;
        [Required(ErrorMessage = "Nhập ngày nhận giấy")]
        [CustomValidation(typeof(PaperReferal), "ValidateAcceptedDate")]
        [DataMemberAttribute()]
        public DateTime? AcceptedDate
        {
            get
            {
                return _acceptedDate;
            }
            set
            {
                if (_acceptedDate != value)
                {
                    ValidateProperty("AcceptedDate", value);
                    _acceptedDate = value;
                    RaisePropertyChanged("AcceptedDate");
                }
            }
        }

        private String _notes;
        [DataMemberAttribute()]
        public String Notes
        {
            get
            {
                return _notes;
            }
            set
            {
                if (_notes != value)
                {
                    _notes = value;
                    RaisePropertyChanged("Notes"); 
                }
            }
        }

        private bool _markAsDeleted;
        [DataMemberAttribute()]
        public bool MarkAsDeleted
        {
            get
            {
                return _markAsDeleted;
            }
            set
            {
                if (_markAsDeleted != value)
                {
                    _markAsDeleted = value;
                    RaisePropertyChanged("MarkAsDeleted");
                }
            }
        }

        private bool _isActive;
        [DataMemberAttribute()]
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    RaisePropertyChanged("IsActive");
                }
            }
        }

        private bool _isBrandNew=false;
        [DataMemberAttribute()]
        public bool isBrandNew
        {
            get
            {
                return _isBrandNew;
            }
            set
            {
                if (_isBrandNew != value)
                {
                    _isBrandNew = value;
                    RaisePropertyChanged("isBrandNew");
                }
            }
        }

        private bool _IsChronicDisease;
        [DataMemberAttribute()]
        public bool IsChronicDisease
        {
            get
            {
                return _IsChronicDisease;
            }
            set
            {
                if (_IsChronicDisease != value)
                {
                    _IsChronicDisease = value;
                    RaisePropertyChanged("IsChronicDisease");
                }
            }
        }
        /*TMA*/
        private bool _IsReUse;
        [DataMemberAttribute()]
        public bool IsReUse
        {
            get
            {
                return _IsReUse;
            }
            set
            {
                if (_IsReUse != value)
                {
                    _IsReUse = value;
                    RaisePropertyChanged("IsReUse");
                }
            }
        }
        /*TMA*/

        private long? _PtRegistrationID;
        [DataMemberAttribute()]
        public long? PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                if (_PtRegistrationID != value)
                {
                    _PtRegistrationID = value;
                    if ( PtRegistrationID!=null
                            && PtRegistrationID>0)
                    {
                        paperState = PtRegistrationID.ToString();
                    }
                    RaisePropertyChanged("PtRegistrationID");
                }
            }
        }

        private string _CityProvinceName;
        [DataMemberAttribute()]
        public string CityProvinceName
        {
            get
            {
                return _CityProvinceName;
            }
            set
            {
                if (_CityProvinceName != value)
                {
                    _CityProvinceName = value;                    
                    RaisePropertyChanged("CityProvinceName");
                }
            }
        }

        private string _IssuerLocation;
        [DataMemberAttribute()]
        public string IssuerLocation
        {
            get
            {
                return _IssuerLocation;
            }
            set
            {
                if (_IssuerLocation != value)
                {
                    _IssuerLocation = value;
                    RaisePropertyChanged("IssuerLocation");
                }
            }
        }

        private string _IssuerCode;
        [DataMemberAttribute()]
        public string IssuerCode
        {
            get
            {
                return _IssuerCode;
            }
            set
            {
                if (_IssuerCode != value)
                {
                    _IssuerCode = value;                    
                    RaisePropertyChanged("IssuerCode");
                }
            }
        }

        public bool IsValid
        {
            get
            {
                return true;
            }
        }

        private EntityState _entityState = EntityState.NEW;
        [DataMemberAttribute()]
        public override EntityState EntityState
        {
            get
            {
                return _entityState;
            }
            set
            {
                if (_entityState != value)
                {
                    _entityState = value;
                    RaisePropertyChanged("EntityState");
                }
            }
        }

        public static ValidationResult ValidateCreatedDate(DateTime? date, ValidationContext context)
        {
            PaperReferal obj = context.ObjectInstance as PaperReferal;
            if (date.HasValue && obj.RefCreatedDate.HasValue)
            {
                if (date.Value.Date > DateTime.Now.Date)
                {
                    return new ValidationResult("Ngày ký giấy không hợp lệ", new string[] { "RefCreatedDate" });
                }
            }
            return ValidationResult.Success;
        }

        public static ValidationResult ValidateAcceptedDate(DateTime? acceptedDate, ValidationContext context)
        {
            PaperReferal obj = context.ObjectInstance as PaperReferal;

            if (acceptedDate.HasValue && obj.RefCreatedDate.HasValue)
            {
                if (acceptedDate.Value.Date > DateTime.Now.Date)
                {
                    return new ValidationResult("Ngày nộp giấy không được chọn ở tương lai", new string[] { "AcceptedDate" });
                }
                if (acceptedDate.Value.Date < obj.RefCreatedDate.Value.Date)
                {
                    return new ValidationResult("Ngày nộp giấy không hợp lệ", new string[] { "AcceptedDate" });
                }
            }
            return ValidationResult.Success;
        }

        public override bool Equals(object obj)
        {
            var info = obj as PaperReferal;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return RefID > 0 && RefID == info.RefID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        private bool _used;
        [DataMemberAttribute()]
        public bool Used
        {
            get { return _used; }
            set
            {
                _used = value;
                RaisePropertyChanged("Used");
            }
        }

        private DateTime? _AdmissionDate;
        [DataMemberAttribute()]
        public DateTime? AdmissionDate
        {
            get
            {
                return _AdmissionDate;
            }
            set
            {
                _AdmissionDate = value;
                RaisePropertyChanged("AdmissionDate");
            }
        }
        private DateTime? _ValidDateFrom;
        [DataMemberAttribute()]
        public DateTime? ValidDateFrom
        {
            get
            {
                return _ValidDateFrom;
            }
            set
            {
                _ValidDateFrom = value;
                RaisePropertyChanged("ValidDateFrom");
            }
        }
        private DateTime? _ValidDateTo;
        [DataMemberAttribute()]
        public DateTime? ValidDateTo
        {
            get
            {
                return _ValidDateTo;
            }
            set
            {
                _ValidDateTo = value;
                RaisePropertyChanged("ValidDateTo");
            }
        }

        private string _FromHospital;
        [DataMemberAttribute()]
        public string FromHospital
        {
            get
            {
                return _FromHospital;
            }
            set
            {
                _FromHospital = value;
                RaisePropertyChanged("FromHospital");
            }
        }
        private string _DischargeReason;
        public string DischargeReason
        {
            get
            {
                return _DischargeReason;
            }
            set
            {
                _DischargeReason = value;
                RaisePropertyChanged("DischargeReason");
            }
        }
    }

    public partial class TransferForm : EntityBase
    {
        private HealthInsurance _UsingHealthInsurance;
        [DataMemberAttribute()]
        public HealthInsurance UsingHealthInsurance
        {
            get
            {
                return _UsingHealthInsurance;
            }
            set
            {
                _UsingHealthInsurance = value;
                RaisePropertyChanged("UsingHealthInsurance");
            }
        }

        private long _TransferFormID;
        [DataMemberAttribute()]
        public long TransferFormID
        {
            get
            {
                return _TransferFormID;
            }
            set
            {
                _TransferFormID = value;
                RaisePropertyChanged("TransferFormID");
            }
        }

        private string _TransferNum;
        [DataMemberAttribute()]
        public string TransferNum
        {
            get
            {
                return _TransferNum;
            }
            set
            {
                _TransferNum = value;
                RaisePropertyChanged("TransferNum");
            }
        }

        private string _SavingNumber;
        [DataMemberAttribute()]
        public string SavingNumber
        {
            get
            {
                return _SavingNumber;
            }
            set
            {
                _SavingNumber = value;
                RaisePropertyChanged("SavingNumber");
            }
        }

        private PatientRegistration _CurPatientRegistration;
        [DataMemberAttribute()]
        public PatientRegistration CurPatientRegistration
        {
            get
            {
                return _CurPatientRegistration;
            }
            set
            {
                _CurPatientRegistration = value;
                RaisePropertyChanged("CurPatientRegistration");
            }
        }

        private Hospital _TransferFromHos;
        [DataMemberAttribute()]
        public Hospital TransferFromHos
        {
            get
            {
                return _TransferFromHos;
            }
            set
            {
                _TransferFromHos = value;
                RaisePropertyChanged("TransferFromHos");
            }
        }

        private Hospital _TransferToHos;
        [DataMemberAttribute()]
        public Hospital TransferToHos
        {
            get
            {
                return _TransferToHos;
            }
            set
            {
                _TransferToHos = value;
                RaisePropertyChanged("TransferToHos");
            }
        }

        private string _HospitalTransferName;
        [DataMemberAttribute()]
        public string HospitalTransferName
        {
            get
            {
                return _HospitalTransferName;
            }
            set
            {
                _HospitalTransferName = value;
                RaisePropertyChanged("HospitalTransferName");
            }
        }

        private DateTime _TransferDate;
        [DataMemberAttribute()]
        public DateTime TransferDate
        {
            get
            {
                return _TransferDate;
            }
            set
            {
                _TransferDate = value;
                RaisePropertyChanged("TransferDate");
            }
        }

        private DateTime _FromDate;
        [DataMemberAttribute()]
        public DateTime FromDate
        {
            get
            {
                return _FromDate;
            }
            set
            {
                _FromDate = value;
                RaisePropertyChanged("FromDate");
            }
        }

        private DateTime _ToDate;
        [DataMemberAttribute()]
        public DateTime ToDate
        {
            get
            {
                return _ToDate;
            }
            set
            {
                _ToDate = value;
                RaisePropertyChanged("ToDate");
            }
        }

        private RefDepartment _TransferFromDept;
        [DataMemberAttribute()]
        public RefDepartment TransferFromDept
        {
            get
            {
                return _TransferFromDept;
            }
            set
            {
                _TransferFromDept = value;
                RaisePropertyChanged("TransferFromDept");
            }
        }

        private string _ClinicalSign;
        [DataMemberAttribute()]
        public string ClinicalSign
        {
            get
            {
                return _ClinicalSign;
            }
            set
            {
                _ClinicalSign = value;
                RaisePropertyChanged("ClinicalSign");
            }
        }

        private string _PCLResult;
        [DataMemberAttribute()]
        public string PCLResult
        {
            get
            {
                return _PCLResult;
            }
            set
            {
                _PCLResult = value;
                RaisePropertyChanged("PCLResult");
            }
        }

        private string _UsedServicesAndItems;
        [DataMemberAttribute()]
        public string UsedServicesAndItems
        {
            get
            {
                return _UsedServicesAndItems;
            }
            set
            {
                _UsedServicesAndItems = value;
                RaisePropertyChanged("UsedServicesAndItems");
            }   
        }

        private string _DiagnosisTreatment;
        [DataMemberAttribute()]
        public string DiagnosisTreatment
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

        private string _ICD10;
        [DataMemberAttribute()]
        public string ICD10
        {
            get
            {
                return _ICD10;
            }
            set
            {
                _ICD10 = value;
                RaisePropertyChanged("ICD10");
            }
        }

        private string _DiagnosisTreatment_Final;
        [DataMemberAttribute()]
        public string DiagnosisTreatment_Final
        {
            get
            {
                return _DiagnosisTreatment_Final;
            }
            set
            {
                _DiagnosisTreatment_Final = value;
                RaisePropertyChanged("DiagnosisTreatment_Final");
            }
        }

        private string _ICD10Final;
        [DataMemberAttribute()]
        public string ICD10Final
        {
            get
            {
                return _ICD10Final;
            }
            set
            {
                _ICD10Final = value;
                RaisePropertyChanged("ICD10Final");
            }
        }

        private string _TransferType;
        [DataMemberAttribute()]
        public string TransferType
        {
            get
            {
                return _TransferType;
            }
            set
            {
                _TransferType = value;
                RaisePropertyChanged("TransferType");
            }
        }

        private long _V_TransferTypeID;
        [DataMemberAttribute()]
        public long V_TransferTypeID
        {
            get
            {
                return _V_TransferTypeID;
            }
            set
            {
                _V_TransferTypeID = value;
                RaisePropertyChanged("V_TransferTypeID");
            }
        }

        private string _TransferReason;
        [DataMemberAttribute()]
        public string TransferReason
        {
            get
            {
                return _TransferReason;
            }
            set
            {
                _TransferReason = value;
                RaisePropertyChanged("TransferReason");
            }
        }

        private long _V_TransferReasonID;
        [DataMemberAttribute()]
        public long V_TransferReasonID
        {
            get
            {
                return _V_TransferReasonID;
            }
            set
            {
                _V_TransferReasonID = value;
                RaisePropertyChanged("V_TransferReasonID");
            }
        }
        /*TMA*/
        private long _PtRegistrationID;
        [DataMemberAttribute()]
        public long PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                _PtRegistrationID = value;
                RaisePropertyChanged("PtRegistrationID");
            }
        }
        /*TMA*/

        private string _TreatmentPlan;
        [DataMemberAttribute()]
        public string TreatmentPlan
        {
            get
            {
                return _TreatmentPlan;
            }
            set
            {
                _TreatmentPlan = value;
                RaisePropertyChanged("TreatmentPlan");
            }
        }

        private string _TreatmentResult;
        [DataMemberAttribute()]
        public string TreatmentResult
        {
            get
            {
                return _TreatmentResult;
            }
            set
            {
                _TreatmentResult = value;
                RaisePropertyChanged("TreatmentResult");
            }
        }

        private long _V_TreatmentResultID;
        [DataMemberAttribute()]
        public long V_TreatmentResultID
        {
            get
            {
                return _V_TreatmentResultID;
            }
            set
            {
                _V_TreatmentResultID = value;
                RaisePropertyChanged("V_TreatmentResultID");
            }
        }

        private string _CMKT;
        [DataMemberAttribute()]
        public string CMKT
        {
            get
            {
                return _CMKT;
            }
            set
            {
                _CMKT = value;
                RaisePropertyChanged("CMKT");
            }
        }

        private long _V_CMKTID;
        [DataMemberAttribute()]
        public long V_CMKTID
        {
            get
            {
                return _V_CMKTID;
            }
            set
            {
                _V_CMKTID = value;
                RaisePropertyChanged("V_CMKTID");
            }
        }

        private string _TransVehicle;
        [DataMemberAttribute()]
        public string TransVehicle
        {
            get
            {
                return _TransVehicle;
            }
            set
            {
                _TransVehicle = value;
                RaisePropertyChanged("TransVehicle");
            }
        }

        private string _TransferBy;
        [DataMemberAttribute()]
        public string TransferBy
        {
            get
            {
                return _TransferBy;
            }
            set
            {
                _TransferBy = value;
                RaisePropertyChanged("TransferBy");
            }
        }

        private long _V_PatientStatusID;
        [DataMemberAttribute()]
        public long V_PatientStatusID
        {
            get
            {
                return _V_PatientStatusID;
            }
            set
            {
                _V_PatientStatusID = value;
                RaisePropertyChanged("V_PatientStatusID");
            }
        }

        private string _PatientStatus;
        [DataMemberAttribute()]
        public string PatientStatus
        {
            get
            {
                return _PatientStatus;
            }
            set
            {
                _PatientStatus = value;
                RaisePropertyChanged("PatientStatus");
            }
        }

        private int _V_TransferFormType;
        [DataMemberAttribute()]
        public int V_TransferFormType
        {
            get
            {
                return _V_TransferFormType;
            }
            set
            {
                _V_TransferFormType = value;
                RaisePropertyChanged("V_TransferFormType");
            }
        }
        
        private bool _IsExistsError;
        [DataMemberAttribute()]
        public bool IsExistsError
        {
            get
            {
                return _IsExistsError;
            }
            set
            {
                _IsExistsError = value;
                RaisePropertyChanged("IsExistsError");
            }
        }

        private bool _IsDiffDiagnosis;
        [DataMemberAttribute()]
        public bool IsDiffDiagnosis
        {
            get
            {
                return _IsDiffDiagnosis;
            }
            set
            {
                _IsDiffDiagnosis = value;
                RaisePropertyChanged("IsDiffDiagnosis");
            }
        }

        private bool _IsActive;
        [DataMemberAttribute()]
        public bool IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                _IsActive = value;
                RaisePropertyChanged("IsActive");
            }
        }

        private bool _MarkAsDeleted;
        [DataMemberAttribute()]
        public bool MarkAsDeleted
        {
            get
            {
                return _MarkAsDeleted;
            }
            set
            {
                _MarkAsDeleted = value;
                RaisePropertyChanged("MarkAsDeleted");
            }
        }

        private string _Note;
        [DataMemberAttribute()]
        public string Note
        {
            get
            {
                return _Note;
            }
            set
            {
                _Note = value;
                RaisePropertyChanged("Note");
            }
        }

        private int _PatientFindBy;
        [DataMemberAttribute()]
        public int PatientFindBy
        {
            get
            {
                return _PatientFindBy;
            }
            set
            {
                _PatientFindBy = value;
                RaisePropertyChanged("PatientFindBy");
            }
        }

        private DiagnosisTreatment _PrevDiagnosisTreatment;
        [DataMemberAttribute()]
        public DiagnosisTreatment PrevDiagnosisTreatment
        {
            get
            {
                return _PrevDiagnosisTreatment;
            }
            set
            {
                _PrevDiagnosisTreatment = value;
                RaisePropertyChanged("PrevDiagnosisTreatment");
            }
        }

        private DiagnosisTreatment _LastDiagnosisTreatment;
        [DataMemberAttribute()]
        public DiagnosisTreatment LastDiagnosisTreatment
        {
            get
            {
                return _LastDiagnosisTreatment;
            }
            set
            {
                _LastDiagnosisTreatment = value;
                RaisePropertyChanged("LastDiagnosisTreatment");
            }
        }

        private DiagnosisIcd10Items _ICD10Main;
        [DataMemberAttribute()]
        public DiagnosisIcd10Items ICD10Main
        {
            get
            {
                return _ICD10Main;
            }
            set
            {
                _ICD10Main = value;
                RaisePropertyChanged("ICD10Main");
            }
        }
        /*TMA*/
        private PaperReferal _PaperReferal;
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
        /*TMA*/
        private DateTime? _TuNgay;
        [DataMemberAttribute()]
        public DateTime? TuNgay
        {
            get
            {
                return _TuNgay;
            }
            set
            {
                _TuNgay = value;
                RaisePropertyChanged("TuNgay");
            }
        }

        private DateTime? _DenNgay;
        [DataMemberAttribute()]
        public DateTime? DenNgay
        {
            get
            {
                return _DenNgay;
            }
            set
            {
                _DenNgay = value;
                RaisePropertyChanged("DenNgay");
            }
        }
    }
}
