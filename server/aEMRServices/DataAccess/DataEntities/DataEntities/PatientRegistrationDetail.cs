using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
/*
 * 20210701 #001 TNHX: 260 Thêm cột user bsi mượn UserOfficialAccountID
 * 20210915 #002 TNHX: 436 Thêm thông tin đặt giường cho DV tiền giường
 * 20220321 #003 QTD:  Thêm kiểm tra thông tin tạm ứng
 * 20230510 #004 DatTB: Thêm điều kiện lấy bệnh nhân đã hoàn thành CLS
 */
namespace DataEntities
{
    public partial class PatientRegistrationDetail : MedRegItemBase, IEditableObject
    {
        public PatientRegistrationDetail() : base()
        {
            ServiceQty = 1;
            ExamRegStatus = AllLookupValues.ExamRegStatus.DANG_KY_KHAM;
            MedProductType = AllLookupValues.MedProductType.KCB;
        }
        #region Factory Method


        /// Create a new PatientRegistrationDetail object.

        /// <param name="ptRegDetailID">Initial value of the PtRegDetailID property.</param>
        /// <param name="v_ExamRegStatus">Initial value of the V_ExamRegStatus property.</param>
        public static PatientRegistrationDetail CreatePatientRegistrationDetail(Int64 ptRegDetailID, Int64 v_ExamRegStatus)
        {
            PatientRegistrationDetail patientRegistrationDetail = new PatientRegistrationDetail();
            patientRegistrationDetail.PtRegDetailID = ptRegDetailID;
            patientRegistrationDetail.V_ExamRegStatus = v_ExamRegStatus;
            return patientRegistrationDetail;
        }

        #endregion
        #region Primitive Properties
        public bool SequenceNoReassigned { get; set; }
        [DataMemberAttribute]
        public override long ID
        {
            get { return PtRegDetailID; }
        }

        [DataMemberAttribute()]
        public Int64 PtRegDetailID
        {
            get
            {
                return _PtRegDetailID;
            }
            set
            {
                if (_PtRegDetailID != value)
                {
                    OnPtRegDetailIDChanging(value);
                    ////ReportPropertyChanging("PtRegDetailID");
                    _PtRegDetailID = value;
                    RaisePropertyChanged("PtRegDetailID");
                    OnPtRegDetailIDChanged();
                }
            }
        }
        private Int64 _PtRegDetailID;
        partial void OnPtRegDetailIDChanging(Int64 value);
        partial void OnPtRegDetailIDChanged();

        private int _RegStatus;
        [DataMemberAttribute]
        public int RegStatus
        {
            get
            {
                return _RegStatus;
            }
            set
            {
                if (_RegStatus != value)
                {
                    _RegStatus = value;
                    RaisePropertyChanged("RegStatus");
                }
            }
        }

        public override decimal? MaskedHIAllowedPrice
        {
            get
            {
                if (TotalHIPayment == 0 && TotalCoPayment == 0)
                {
                    return 0;
                }
                return HIAllowedPrice;
            }
        }
        private int _serviceSeqNum;
        [DataMemberAttribute()]
        public int ServiceSeqNum
        {
            get
            {
                return _serviceSeqNum;
            }
            set
            {
                _serviceSeqNum = value;
                RaisePropertyChanged("ServiceSeqNum");
            }
        }
        private int _serviceSeqNum_Old;
        [DataMemberAttribute()]
        public int ServiceSeqNum_Old
        {
            get
            {
                return _serviceSeqNum_Old;
            }
            set
            {
                _serviceSeqNum_Old = value;
                RaisePropertyChanged("ServiceSeqNum_Old");
            }
        }
        private byte _serviceSeqNumType;
        [DataMemberAttribute()]
        public byte ServiceSeqNumType
        {
            get
            {
                return _serviceSeqNumType;
            }
            set
            {
                _serviceSeqNumType = value;
                RaisePropertyChanged("ServiceSeqNumType");
            }
        }

        private string _serviceSeqNumString;
        [DataMemberAttribute]
        public string ServiceSeqNumString
        {
            get
            {
                return _serviceSeqNumString;
            }
            set
            {
                _serviceSeqNumString = value;
                RaisePropertyChanged("ServiceSeqNumString");
            }
        }

        [DataMemberAttribute()]
        public Nullable<Int64> PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                OnPtRegistrationIDChanging(value);
                ////ReportPropertyChanging("PtRegistrationID");
                _PtRegistrationID = value;
                RaisePropertyChanged("PtRegistrationID");
                OnPtRegistrationIDChanged();
            }
        }
        private Nullable<Int64> _PtRegistrationID;
        partial void OnPtRegistrationIDChanging(Nullable<Int64> value);
        partial void OnPtRegistrationIDChanged();


        [DataMemberAttribute()]
        public Nullable<long> DeptLocID
        {
            get
            {
                return _deptLocID;
            }
            set
            {
                ValidateProperty("DeptLocID", value);
                _deptLocID = value;
                RaisePropertyChanged("DeptLocID");
            }
        }
        private Nullable<long> _deptLocID;

        [DataMemberAttribute()]
        public Nullable<long> MedServiceID
        {
            get
            {
                return _MedServiceID;
            }
            set
            {
                OnMedServiceIDChanging(value);
                ////ReportPropertyChanging("MedServiceID");
                _MedServiceID = value;
                RaisePropertyChanged("MedServiceID");
                OnMedServiceIDChanged();
            }
        }
        private Nullable<long> _MedServiceID;
        partial void OnMedServiceIDChanging(Nullable<long> value);
        partial void OnMedServiceIDChanged();

        private RefMedicalServiceType _MedServiceType;
        public RefMedicalServiceType MedServiceType
        {
            get
            {
                return _MedServiceType;
            }
            set
            {
                _MedServiceType = value;
                RaisePropertyChanged("MedServiceType");
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
        [DataMemberAttribute()]
        [Required(ErrorMessage = "Quantity is required")]
        [Range(Double.Epsilon, Double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public Nullable<double> ServiceQty
        {
            get
            {
                return _ServiceQty;
            }
            set
            {
                ValidateProperty("ServiceQty", value);
                _ServiceQty = value;
                RaisePropertyChanged("ServiceQty");
            }
        }
        private Nullable<double> _ServiceQty;
        partial void OnServiceQtyChanging(Nullable<Byte> value);
        partial void OnServiceQtyChanged();

        private Lookup _ExamRegStatusObj;
        [DataMemberAttribute()]
        public Lookup ExamRegStatusObj
        {
            get
            {
                return _ExamRegStatusObj;
            }
            set
            {
                if (_ExamRegStatusObj != value)
                {
                    _ExamRegStatusObj = value;
                    RaisePropertyChanged("ExamRegStatusObj");
                }
            }
        }


        [DataMemberAttribute()]
        private Lookup _ObjV_ExamRegStatus;
        public Lookup ObjV_ExamRegStatus
        {
            get { return _ObjV_ExamRegStatus; }
            set
            {
                OnObjV_ExamRegStatusChanging(value);
                _ObjV_ExamRegStatus = value;
                RaisePropertyChanged("ObjV_ExamRegStatus");
                OnObjV_ExamRegStatusChanged();
            }
        }
        partial void OnObjV_ExamRegStatusChanging(Lookup value);
        partial void OnObjV_ExamRegStatusChanged();

        private BedAllocation _BedAllocation;
        [DataMemberAttribute()]
        public BedAllocation BedAllocation
        {
            get
            {
                return _BedAllocation;
            }
            set
            {
                _BedAllocation = value;
                RaisePropertyChanged("BedAllocation");
            }
        }

        private BedPatientRegDetail _bedPatientRegDetail;
        [DataMemberAttribute()]
        public BedPatientRegDetail BedPatientRegDetail
        {
            get
            {
                return _bedPatientRegDetail;
            }
            set
            {
                _bedPatientRegDetail = value;
                RaisePropertyChanged("BedPatientRegDetail");
            }
        }

        [DataMemberAttribute()]
        public Int64 V_ExamRegStatus
        {
            get
            {
                return _V_ExamRegStatus;
            }
            set
            {
                if (_V_ExamRegStatus != value)
                {
                    _V_ExamRegStatus = value;
                    RaisePropertyChanged("V_ExamRegStatus");
                }
            }
        }
        private Int64 _V_ExamRegStatus;

        [DataMemberAttribute()]
        public Int64 ExamRegStatusObject
        {
            get
            {
                return _ExamRegStatusObject;
            }
            set
            {
                if (_ExamRegStatusObject != value)
                {
                    _ExamRegStatusObject = value;
                    RaisePropertyChanged("ExamRegStatusObject");
                }
            }
        }
        private Int64 _ExamRegStatusObject;

        [DataMemberAttribute()]
        public string RegStaffFullName
        {
            get { return _RegStaffFullName; }
            set
            {
                OnRegStaffFullNameChanging(value);
                _RegStaffFullName = value;
                RaisePropertyChanged("RegStaffFullName");
                OnRegStaffFullNameChanged();
            }
        }
        private string _RegStaffFullName;
        partial void OnRegStaffFullNameChanging(string value);
        partial void OnRegStaffFullNameChanged();


        [DataMemberAttribute()]
        public long ServiceRecID
        {
            get { return _ServiceRecID; }
            set
            {
                OnServiceRecIDChanging(value);
                _ServiceRecID = value;
                RaisePropertyChanged("ServiceRecID");
                OnServiceRecIDChanged();
            }
        }
        private long _ServiceRecID;
        partial void OnServiceRecIDChanging(long value);
        partial void OnServiceRecIDChanged();


        [DataMemberAttribute()]
        public string DiagDeptLocationName
        {
            get { return _DiagDeptLocationName; }
            set
            {
                OnDiagDeptLocationNameChanging(value);
                _DiagDeptLocationName = value;
                RaisePropertyChanged("DiagDeptLocationName");
                OnDiagDeptLocationNameChanged();
            }
        }
        private string _DiagDeptLocationName;
        partial void OnDiagDeptLocationNameChanging(string value);
        partial void OnDiagDeptLocationNameChanged();

        [DataMemberAttribute()]
        public string DiagDoctorName
        {
            get { return _DiagDoctorName; }
            set
            {
                OnDiagDoctorNameChanging(value);
                _DiagDoctorName = value;
                RaisePropertyChanged("DiagDoctorName");
                OnDiagDoctorNameChanged();
            }
        }
        private string _DiagDoctorName;
        partial void OnDiagDoctorNameChanging(string value);
        partial void OnDiagDoctorNameChanged();
        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_COMMONME_REL_PCMD2_PATIENTR", "CommonMedicalRecords")]
        public ObservableCollection<CommonMedicalRecord> CommonMedicalRecords
        {
            get;
            set;
        }


        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_INCURRED_REL_AEREC_PATIENTR", "IncurredMedicalExpenses")]
        public ObservableCollection<IncurredMedicalExpens> IncurredMedicalExpenses
        {
            get;
            set;
        }

        private DeptLocation _deptLocation;
        [DataMemberAttribute()]
        //[Required(ErrorMessage="DeptLocation is required")]
        public DeptLocation DeptLocation
        {
            get
            {
                return _deptLocation;
            }
            set
            {
                ValidateProperty("DeptLocation", value);
                _deptLocation = value;
                RaisePropertyChanged("DeptLocation");
            }
        }


        [DataMemberAttribute()]
        public PatientRegistration PatientRegistration
        {
            get
            {
                return _PatientRegistration;
            }
            set
            {
                OnPatientRegistrationChanging(value);
                _PatientRegistration = value;
                //if (_PatientRegistration!=null
                //    && _PatientRegistration.StaffID!=null)
                //{
                //    PaidStaffID = _PatientRegistration.StaffID.Value;
                //}
                RaisePropertyChanged("PatientRegistration");
                OnPatientRegistrationChanged();
            }
        }
        private PatientRegistration _PatientRegistration;
        partial void OnPatientRegistrationChanging(PatientRegistration PatientRegistration);
        partial void OnPatientRegistrationChanged();


        [DataMemberAttribute()]
        public PrescriptionIssueHistory prescriptionIssueHistory
        {
            get
            {
                return _prescriptionIssueHistory;
            }
            set
            {
                OnprescriptionIssueHistoryChanging(value);
                _prescriptionIssueHistory = value;
                RaisePropertyChanged("prescriptionIssueHistory");
                OnprescriptionIssueHistoryChanged();
            }
        }
        private PrescriptionIssueHistory _prescriptionIssueHistory;
        partial void OnprescriptionIssueHistoryChanging(PrescriptionIssueHistory prescriptionIssueHistory);
        partial void OnprescriptionIssueHistoryChanged();


        private DateTime _RegDetailCancelDate;
        [DataMemberAttribute()]
        public DateTime RegDetailCancelDate
        {
            get
            {
                return _RegDetailCancelDate;
            }
            set
            {
                _RegDetailCancelDate = value;
                RaisePropertyChanged("RegDetailCancelDate");
            }
        }

        private long _RegDetailCancelStaffID;
        [DataMemberAttribute()]
        public long RegDetailCancelStaffID
        {
            get
            {
                return _RegDetailCancelStaffID;
            }
            set
            {
                _RegDetailCancelStaffID = value;
                RaisePropertyChanged("RegDetailCancelStaffID");
            }
        }


        private RefMedicalServiceItem _RefMedicalServiceItem;
        [DataMemberAttribute()]
        [Required(ErrorMessage = "Please select a service")]
        public RefMedicalServiceItem RefMedicalServiceItem
        {
            get
            {
                return _RefMedicalServiceItem;
            }
            set
            {
                ValidateProperty("RefMedicalServiceItem", value);
                _RefMedicalServiceItem = value;
                if (_RefMedicalServiceItem != null)
                {
                    MedServiceName = _RefMedicalServiceItem.MedServiceName;
                    if (!string.IsNullOrEmpty(SpecialNote))
                    {
                        MedServiceName += " - " + SpecialNote;
                    }
                }
                RaisePropertyChanged("RefMedicalServiceItem");
            }
        }

        [DataMemberAttribute()]
        public ObservableCollection<PatientServiceRecord> PatientServiceRecords
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PCLREQUE_REL_REQPC_PATIENTR", "PCLRequestForms")]
        public ObservableCollection<PCLForExternalRef> PCLRequestForms
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PTREGD_REL_PTINF_STAFFS", "Staffs")]
        public Staff Staff
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PTTRANSD_REL_HOSFM_PTREGD", "PatientTransactionDetails")]
        public ObservableCollection<PatientTransactionDetail> PatientTransactionDetails
        {
            get;
            set;
        }

        #endregion
        private long? _inPatientBillingInvID;
        [DataMemberAttribute()]
        public long? InPatientBillingInvID
        {
            get
            {
                return _inPatientBillingInvID;
            }
            set
            {
                if (_inPatientBillingInvID != value)
                {
                    _inPatientBillingInvID = value;
                    RaisePropertyChanged("InPatientBillingInvID");
                }
            }
        }
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }
        private bool _isSelected = false;


        private PatientRegistrationDetail _tempRegDetails;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempRegDetails = (PatientRegistrationDetail)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempRegDetails)
            {
                CopyFrom(_tempRegDetails);

            }
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(PatientRegistrationDetail p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion

        private ServicePrice _ServicePrice = new ServicePrice();
        public ServicePrice ServicePrice
        {
            get
            {
                return _ServicePrice;
            }
            set
            {
                _ServicePrice = value;
                RaisePropertyChanged("ServicePrice");
            }
        }
        private PaymentInfo _PaymentInfo = new PaymentInfo();
        public PaymentInfo PaymentInfo
        {
            get
            {
                return _PaymentInfo;
            }
            set
            {
                _PaymentInfo = value;
                RaisePropertyChanged("PaymentInfo");
            }
        }


        private bool _isSetSeqNumManually;
        public bool IsSetSeqNumManually
        {
            get
            {
                return _isSetSeqNumManually;
            }
            set
            {
                _isSetSeqNumManually = value;
                RaisePropertyChanged("IsSetSeqNumManually");
            }
        }

        #region IInvoiceItem Members

        public override IGenericService GenericServiceItem
        {
            get
            {
                return _RefMedicalServiceItem;
            }
        }

        public override IChargeableItemPrice ChargeableItem
        {
            get
            {
                return _RefMedicalServiceItem;
            }
        }

        #endregion

        [DataMemberAttribute]
        public bool FromAppointment { get; set; }

        [DataMemberAttribute()]
        public DateTime? AppointmentDate
        {
            get
            {
                return _AppointmentDate;
            }
            set
            {
                OnAppointmentDateChanging(value);
                _AppointmentDate = value;
                RaisePropertyChanged("AppointmentDate");
                OnAppointmentDateChanged();
            }
        }
        private DateTime? _AppointmentDate;
        partial void OnAppointmentDateChanging(DateTime? value);
        partial void OnAppointmentDateChanged();


        [DataMemberAttribute()]
        public DateTime? RecCreatedDate
        {
            get
            {
                return _RecCreatedDate;
            }
            set
            {
                OnRecCreatedDateChanging(value);
                _RecCreatedDate = value;
                RaisePropertyChanged("RecCreatedDate");
                OnRecCreatedDateChanged();
            }
        }
        private DateTime? _RecCreatedDate;
        partial void OnRecCreatedDateChanging(DateTime? value);
        partial void OnRecCreatedDateChanged();


        [DataMemberAttribute()]
        public PatientAppointment patientAppointment
        {
            get
            {
                return _patientAppointment;
            }
            set
            {
                OnpatientAppointmentChanging(value);
                _patientAppointment = value;
                RaisePropertyChanged("patientAppointment");
                OnpatientAppointmentChanged();
            }
        }
        private PatientAppointment _patientAppointment;
        partial void OnpatientAppointmentChanging(PatientAppointment value);
        partial void OnpatientAppointmentChanged();


        private string _MedServiceName;
        [DataMemberAttribute()]
        public string MedServiceName
        {
            get
            {
                return _MedServiceName;
            }
            set
            {
                _MedServiceName = value;
                RaisePropertyChanged("MedServiceName");
            }
        }

        [DataMemberAttribute()]
        public string ReceiptNumber
        {
            get
            {
                return _ReceiptNumber;
            }
            set
            {
                if (_ReceiptNumber != value)
                {
                    _ReceiptNumber = value;
                    RaisePropertyChanged("ReceiptNumber");
                }
            }
        }
        private string _ReceiptNumber;

        private string _InvoiceID;
        [DataMemberAttribute()]
        public string InvoiceID
        {
            get
            {
                return _InvoiceID;
            }
            set
            {
                if (_InvoiceID != value)
                {
                    _InvoiceID = value;
                    RaisePropertyChanged("InvoiceID");
                }
            }
        }

        public override bool Equals(object obj)
        {
            PatientRegistrationDetail info = obj as PatientRegistrationDetail;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PtRegDetailID > 0 && this.PtRegDetailID == info.PtRegDetailID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private long? _IntPtDiagDrInstructionID;
        public long? IntPtDiagDrInstructionID
        {
            get
            {
                return _IntPtDiagDrInstructionID;
            }
            set
            {
                _IntPtDiagDrInstructionID = value;
                RaisePropertyChanged("IntPtDiagDrInstructionID");
            }
        }

        private long _DoctorStaffID;
        [DataMemberAttribute()]
        public long DoctorStaffID
        {
            get
            {
                return _DoctorStaffID;
            }
            set
            {
                _DoctorStaffID = value;
                RaisePropertyChanged("DoctorStaffID");
            }
        }

        private DateTime? _MedInstructionDate;
        [DataMemberAttribute()]
        public DateTime? MedInstructionDate
        {
            get
            {
                return _MedInstructionDate;
            }
            set
            {
                _MedInstructionDate = value;
                RaisePropertyChanged("MedInstructionDate");
            }
        }

        private double _HIPaymentPercent = 1;
        [DataMemberAttribute]
        public double HIPaymentPercent
        {
            get
            {
                return _HIPaymentPercent;
            }
            set
            {
                _HIPaymentPercent = value;
                RaisePropertyChanged("HIPaymentPercent");
            }
        }
        [DataMemberAttribute()]
        public long ReqDeptID
        {
            get
            {
                return _ReqDeptID;
            }
            set
            {
                if (_ReqDeptID != value)
                {
                    _ReqDeptID = value;
                    RaisePropertyChanged("ReqDeptID");
                }
            }
        }
        private long _ReqDeptID;

        [DataMemberAttribute()]
        public string Treatment
        {
            get
            {
                return _Treatment;
            }
            set
            {
                if (_Treatment != value)
                {
                    _Treatment = value;
                    RaisePropertyChanged("Treatment");
                }
            }
        }
        private string _Treatment;

        private double _PaymentPercent = 1;
        [DataMemberAttribute]
        public double PaymentPercent
        {
            get => _PaymentPercent; set
            {
                _PaymentPercent = value;
                RaisePropertyChanged("PaymentPercent");
            }
        }

        private string _ReasonChangeStatus;
        [DataMemberAttribute()]
        public string ReasonChangeStatus
        {
            get
            {
                return _ReasonChangeStatus;
            }
            set
            {
                _ReasonChangeStatus = value;
                RaisePropertyChanged("ReasonChangeStatus");
            }
        }

        private long _StaffChangeStatus;
        [DataMemberAttribute()]
        public long StaffChangeStatus
        {
            get
            {
                return _StaffChangeStatus;
            }
            set
            {
                _StaffChangeStatus = value;
                RaisePropertyChanged("StaffChangeStatus");
            }
        }

        private DateTime _DateChangeStatus;
        [DataMemberAttribute()]
        public DateTime DateChangeStatus
        {
            get
            {
                return _DateChangeStatus;
            }
            set
            {
                _DateChangeStatus = value;
                RaisePropertyChanged("DateChangeStatus");
            }
        }

        private string _Diagnosis;
        [DataMemberAttribute()]
        public string Diagnosis
        {
            get
            {
                return _Diagnosis;
            }
            set
            {
                _Diagnosis = value;
                RaisePropertyChanged("Diagnosis");
            }
        }

        private Lookup _V_Ekip;
        [DataMemberAttribute()]
        public Lookup V_Ekip
        {
            get
            {
                return _V_Ekip;
            }
            set
            {
                _V_Ekip = value;
                RaisePropertyChanged("V_Ekip");
            }
        }

        private Lookup _V_EkipIndex;
        public Lookup V_EkipIndex
        {
            get
            {
                return _V_EkipIndex;
            }
            set
            {
                _V_EkipIndex = value;
                RaisePropertyChanged("V_EkipIndex");
            }
        }


        private decimal? _VATRate;
        [DataMemberAttribute]
        public decimal? VATRate
        {
            get
            {
                return _VATRate;
            }
            set
            {
                _VATRate = value;
                RaisePropertyChanged("VATRate");
            }
        }

        private DateTime? _DiagnosisDate;
        [DataMemberAttribute]
        public DateTime? DiagnosisDate
        {
            get
            {
                return _DiagnosisDate;
            }
            set
            {
                _DiagnosisDate = value;
                RaisePropertyChanged("DiagnosisDate");
            }
        }

        private long? _AppointmentID;
        [DataMemberAttribute]
        public long? AppointmentID
        {
            get
            {
                return _AppointmentID;
            }
            set
            {
                if (_AppointmentID == value)
                {
                    return;
                }
                _AppointmentID = value;
                RaisePropertyChanged("AppointmentID");
            }
        }

        private DateTime? _DateStarted;
        [DataMemberAttribute]
        public DateTime? DateStarted
        {
            get
            {
                return _DateStarted;
            }
            set
            {
                if (_DateStarted == value)
                {
                    return;
                }
                _DateStarted = value;
                RaisePropertyChanged("DateStarted");
            }
        }

        private DateTime? _DateEnded;
        [DataMemberAttribute]
        public DateTime? DateEnded
        {
            get
            {
                return _DateEnded;
            }
            set
            {
                if (_DateEnded == value)
                {
                    return;
                }
                _DateEnded = value;
                RaisePropertyChanged("DateEnded");
            }
        }

        private long _HosClientID;
        [DataMemberAttribute()]
        public long HosClientID
        {
            get
            {
                return _HosClientID;
            }
            set
            {
                if (_HosClientID != value)
                {
                    _HosClientID = value;
                    RaisePropertyChanged("HosClientID");
                }
            }
        }
        private long _HosClientContractID;
        [DataMemberAttribute()]
        public long HosClientContractID
        {
            get
            {
                return _HosClientContractID;
            }
            set
            {
                if (_HosClientContractID != value)
                {
                    _HosClientContractID = value;
                    RaisePropertyChanged("HosClientContractID");
                }
            }
        }

        private string _ContractNo;
        [DataMemberAttribute()]
        public string ContractNo
        {
            get
            {
                return _ContractNo;
            }
            set
            {
                if (_ContractNo != value)
                {
                    _ContractNo = value;
                    RaisePropertyChanged("ContractNo");
                }
            }
        }
        private string _ContractName;
        [DataMemberAttribute()]
        public string ContractName
        {
            get
            {
                return _ContractName;
            }
            set
            {
                if (_ContractName != value)
                {
                    _ContractName = value;
                    RaisePropertyChanged("ContractName");
                }
            }
        }
        private string _CompanyName;
        [DataMemberAttribute()]
        public string CompanyName
        {
            get
            {
                return _CompanyName;
            }
            set
            {
                if (_CompanyName != value)
                {
                    _CompanyName = value;
                    RaisePropertyChanged("CompanyName");
                }
            }
        }

        private long? _ApptSvcDetailID;
        [DataMemberAttribute]
        public long? ApptSvcDetailID
        {
            get
            {
                return _ApptSvcDetailID;
            }
            set
            {
                if (_ApptSvcDetailID == value)
                {
                    return;
                }
                _ApptSvcDetailID = value;
                RaisePropertyChanged("ApptSvcDetailID");
            }
        }

        private long? _ConsultationRoomStaffAllocID;
        [DataMemberAttribute]
        public long? ConsultationRoomStaffAllocID
        {
            get
            {
                return _ConsultationRoomStaffAllocID;
            }
            set
            {
                if (_ConsultationRoomStaffAllocID == value)
                {
                    return;
                }
                _ConsultationRoomStaffAllocID = value;
                RaisePropertyChanged("ConsultationRoomStaffAllocID");
            }
        }

        private DateTime? _ApptStartDate;
        [DataMemberAttribute]
        public DateTime? ApptStartDate
        {
            get
            {
                return _ApptStartDate;
            }
            set
            {
                if (_ApptStartDate == value)
                {
                    return;
                }
                _ApptStartDate = value;
                RaisePropertyChanged("ApptStartDate");
            }
        }

        private DateTime? _ApptEndDate;
        [DataMemberAttribute]
        public DateTime? ApptEndDate
        {
            get
            {
                return _ApptEndDate;
            }
            set
            {
                if (_ApptEndDate == value)
                {
                    return;
                }
                _ApptEndDate = value;
                RaisePropertyChanged("ApptEndDate");
            }
        }

        private bool _ConfimedForPreAndDischarge;
        [DataMemberAttribute]
        public bool ConfimedForPreAndDischarge
        {
            get
            {
                return _ConfimedForPreAndDischarge;
            }
            set
            {
                if (_ConfimedForPreAndDischarge == value)
                {
                    return;
                }
                _ConfimedForPreAndDischarge = value;
                RaisePropertyChanged("ConfimedForPreAndDischarge");
            }
        }

        private long? _ClientContractSvcPtID;
        [DataMemberAttribute]
        public long? ClientContractSvcPtID
        {
            get
            {
                return _ClientContractSvcPtID;
            }
            set
            {
                if (_ClientContractSvcPtID == value)
                {
                    return;
                }
                _ClientContractSvcPtID = value;
                RaisePropertyChanged("ClientContractSvcPtID");
            }
        }
        [DataMemberAttribute()]
        public long PackServDetailID
        {
            get
            {
                return _PackServDetailID;
            }
            set
            {
                if (_PackServDetailID != value)
                {
                    _PackServDetailID = value;
                    RaisePropertyChanged("PackServDetailID");
                }
            }
        }
        private int _PatientWaitStatus;

        [DataMemberAttribute()]
        public int PatientWaitStatus
        {
            get
            {
                return _PatientWaitStatus;
            }
            set
            {
                if (_PatientWaitStatus != value)
                {
                    _PatientWaitStatus = value;
                    RaisePropertyChanged("PatientWaitStatus");
                }
            }
        }
        private long _PackServDetailID;

        [DataMemberAttribute()]
        public long ReqFromDeptID
        {
            get
            {
                return _ReqFromDeptID;
            }
            set
            {
                if (_ReqFromDeptID != value)
                {
                    _ReqFromDeptID = value;
                    RaisePropertyChanged("ReqFromDeptID");
                }
            }
        }
        private long _ReqFromDeptID;

        [DataMemberAttribute()]
        public RefDepartments ReqFromDepartment
        {
            get
            {
                return _ReqFromDepartment;
            }
            set
            {
                if (_ReqFromDepartment != value)
                {
                    _ReqFromDepartment = value;
                    RaisePropertyChanged("ReqFromDepartment");
                }
            }
        }
        private RefDepartments _ReqFromDepartment;

        [DataMemberAttribute()]
        public long STT
        {
            get
            {
                return _STT;
            }
            set
            {
                if (_STT != value)
                {
                    _STT = value;
                    RaisePropertyChanged("STT");
                }
            }
        }
        private long _STT;

        private bool _IsPriority;
        public bool IsPriority
        {
            get
            {
                return _IsPriority;
            }
            set
            {
                _IsPriority = value;
                RaisePropertyChanged("IsPriority");
            }
        }
        //▼====: #001
        [DataMemberAttribute()]
        public long? UserOfficialAccountID
        {
            get
            {
                return _UserOfficialAccountID;
            }
            set
            {
                if (_UserOfficialAccountID != value)
                {
                    _UserOfficialAccountID = value;
                    RaisePropertyChanged("UserOfficialAccountID");
                }
            }
        }

        private long? _UserOfficialAccountID;
        //▲====: #001
        //▼====: #002
        [DataMemberAttribute()]
        public long? BedPatientID
        {
            get
            {
                return _BedPatientID;
            }
            set
            {
                if (_BedPatientID != value)
                {
                    _BedPatientID = value;
                    RaisePropertyChanged("BedPatientID");
                }
            }
        }

        private long? _BedPatientID;
        [DataMemberAttribute()]
        public bool IsNotBedService
        {
            get
            {
                return _IsNotBedService;
            }
            set
            {
                if (_IsNotBedService != value)
                {
                    _IsNotBedService = value;
                    RaisePropertyChanged("IsNotBedService");
                }
            }
        }

        private bool _IsNotBedService = true;
        //▲====: #002

        //▼====: #003
        private bool _IsNeedCashAdvance = false;
        [DataMemberAttribute()]
        public bool IsNeedCashAdvance
        {
            get
            {
                return _IsNeedCashAdvance;
            }
            set
            {
                if (_IsNeedCashAdvance == value)
                {
                    return;
                }
                _IsNeedCashAdvance = value;
                RaisePropertyChanged("IsNeedCashAdvance");
            }
        }
        //▲====: #003
        //▼====: #004
        private bool _IsSevereIllness;
        [DataMemberAttribute]
        public bool IsSevereIllness
        {
            get
            {
                return _IsSevereIllness;
            }
            set
            {
                if (_IsSevereIllness == value)
                {
                    return;
                }
                _IsSevereIllness = value;
                RaisePropertyChanged("IsSevereIllness");
            }
        }
        //▲====: #003
        private int _IsDigitalSigned;
        [DataMemberAttribute]
        public int IsDigitalSigned
        {
            get
            {
                return _IsDigitalSigned;
            }
            set
            {
                if (_IsDigitalSigned == value)
                {
                    return;
                }
                _IsDigitalSigned = value;
                RaisePropertyChanged("IsDigitalSigned");
            }
        }

        //▼==== #004
        private bool _IsPCLFinished;

        [DataMemberAttribute]
        public bool IsPCLFinished
        {
            get
            {
                return _IsPCLFinished;
            }
            set
            {
                if (_IsPCLFinished == value)
                {
                    return;
                }
                _IsPCLFinished = value;
                RaisePropertyChanged("IsPCLFinished");
            }
        }
        //▲==== #004
    }

    public static class PatientRegistrationDetailBase
    {
        public static void ApplyHIPaymentPercent(this PatientRegistrationDetail aRegistrationDetail, double aHIPaymentPercent = 0, PatientRegistration aRegistration = null, DateTime? aCurrentServerDateTime = null, bool aOnlyRoundResultForOutward = false, bool aDetectHiApplied = false)
        {
            if (aRegistrationDetail.HIPaymentPercent == aHIPaymentPercent) return;
            var totalHiPayment = aRegistrationDetail.TotalHIPayment;
            var totalCoPayment = aRegistrationDetail.TotalCoPayment;
            var mHIPercent = aRegistrationDetail.HIPaymentPercent;
            if (aHIPaymentPercent == 0)
            {
                aRegistrationDetail.HIPaymentPercent = 0;
                aRegistrationDetail.TotalCoPayment = 0;
                aRegistrationDetail.TotalHIPayment = 0;

                aRegistrationDetail.TotalPatientPayment = aRegistrationDetail.TotalInvoicePrice;
                aRegistrationDetail.TotalPriceDifference = aRegistrationDetail.TotalPatientPayment;
            }
            else
            {
                if (aRegistration == null) throw new Exception("Thiếu thông tin đăng ký");
                aRegistrationDetail.HIPaymentPercent = aHIPaymentPercent;
                aRegistrationDetail.GetItemPrice(aRegistration, aRegistrationDetail.HIBenefit.GetValueOrDefault(0), aCurrentServerDateTime, aDetectHiApplied);
                aRegistrationDetail.GetItemTotalPrice(aOnlyRoundResultForOutward);
            }
            if (aRegistrationDetail.TotalCoPayment != totalCoPayment
                || aRegistrationDetail.TotalHIPayment != totalHiPayment
                || mHIPercent != aRegistrationDetail.HIPaymentPercent)
            {
                if (aRegistrationDetail.RecordState == RecordState.UNCHANGED)
                {
                    aRegistrationDetail.RecordState = RecordState.MODIFIED;
                }
            }
        }
    }

    public class ServicePrice : NotifyChangedBase
    {
        private decimal _Price;
        public decimal Price
        {
            get
            {
                return _Price;
            }
            set
            {
                _Price = value;
                RaisePropertyChanged("Price");
            }
        }

        public decimal _PriceDifference;
        public decimal PriceDifference
        {
            get
            {
                return _PriceDifference;
            }
            set
            {
                _PriceDifference = value;
                RaisePropertyChanged("PriceDifference");
            }
        }
        private decimal _CoPayment;
        public decimal CoPayment
        {
            get
            {
                return _CoPayment;
            }
            set
            {
                _CoPayment = value;
                RaisePropertyChanged("CoPayment");
            }
        }

        private decimal _HIPayment;
        public decimal HIPayment
        {
            get
            {
                return _HIPayment;
            }
            set
            {
                _HIPayment = value;
                RaisePropertyChanged("HIPayment");
            }
        }

        private decimal _PatientPayment;
        public decimal PatientPayment
        {
            get
            {
                return _PatientPayment;
            }
            set
            {
                _PatientPayment = value;
                RaisePropertyChanged("PatientPayment");
            }
        }
    }

    public class PaymentInfo : NotifyChangedBase
    {
        private decimal _TotalHIPayment;
        public decimal TotalHIPayment
        {
            get
            {
                return _TotalHIPayment;
            }
            set
            {
                _TotalHIPayment = value;
                RaisePropertyChanged("TotalHIPayment");
            }
        }

        public decimal _TotalPatientPayment;
        public decimal TotalPatientPayment
        {
            get
            {
                return _TotalPatientPayment;
            }
            set
            {
                _TotalPatientPayment = value;
                RaisePropertyChanged("TotalPatientPayment");
            }
        }
        private decimal _TotalCoPayment;
        public decimal TotalCoPayment
        {
            get
            {
                return _TotalCoPayment;
            }
            set
            {
                _TotalCoPayment = value;
                RaisePropertyChanged("TotalCoPayment");
            }
        }

        private decimal _TotalPriceDifference;
        public decimal TotalPriceDifference
        {
            get
            {
                return _TotalPriceDifference;
            }
            set
            {
                _TotalPriceDifference = value;
                RaisePropertyChanged("TotalPriceDifference");
            }
        }

        /// <summary>
        /// 31-08-2012 Dinh
        /// Thêm trạng thái để phân biệt nội trú và ngoại trú
        /// </summary>
        private RegistrationType _RegistrationType;
        [DataMemberAttribute()]
        public RegistrationType RegistrationType
        {
            get
            {
                return _RegistrationType;
            }
            set
            {
                _RegistrationType = value;
                RaisePropertyChanged("RegistrationType");
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
                RaisePropertyChanged("V_RegistrationType");
            }
        }        
    }
}