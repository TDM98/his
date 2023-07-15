using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
/*
 * 20170927 #001 CMN: Added DeadReason
 * 20220316 #002 DatTB: Thêm xác nhận loại bệnh nhân trước khi nhập viện, xác nhận hoãn tạm ứng. lý do, xác nhận hoãn tạm ứng. lý do
 * 20220526 #003 DatTB: Lấy thêm các biến xác nhận tạm hoãn tạm ứng
 * 20220622 #004 QTD:   Thêm đánh dấu bệnh nặng y lệnh nội trú
 * 20221019 #005 DatTB: Thêm biến kiểm tra hồ sơ đang được trả về để kiểm tra điều kiện Ràng buộc mở/khóa các nút Lưu TT XV, Hủy XV, Gửi hồ sơ theo quy trình
 * 20230322 #006 BLQ: Thêm phương pháp điều trị 
 * 20230412 #007 QTD: Thêm các trường cho XML 7
*/
namespace DataEntities
{
    public partial class InPatientAdmDisDetails : NotifyChangedBase, IEditableObject
    {
        public InPatientAdmDisDetails()
            : base()
        {

        }
        public override bool Equals(object obj)
        {
            InPatientAdmDisDetails info = obj as InPatientAdmDisDetails;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.InPatientAdmDisDetailID > 0 && this.InPatientAdmDisDetailID == info.InPatientAdmDisDetailID;
        }

        public override int GetHashCode()
        {
            return this.InPatientAdmDisDetailID.GetHashCode();
        }

        private InPatientAdmDisDetails _tempInPatientAdmDisDetails;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempInPatientAdmDisDetails = (InPatientAdmDisDetails)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempInPatientAdmDisDetails)
                CopyFrom(_tempInPatientAdmDisDetails);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(InPatientAdmDisDetails p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new InPatientAdmDisDetails object.

        /// <param name="bedLocNumber">Initial value of the BedLocNumber property.</param>
        /// <param name="allocationID">Initial value of the AllocationID property.</param>
        public static InPatientAdmDisDetails CreateInPatientAdmDisDetails(long InPatientAdmDisDetailID)
        {
            InPatientAdmDisDetails InPatientAdmDisDetails = new InPatientAdmDisDetails();
            
            InPatientAdmDisDetails.InPatientAdmDisDetailID = InPatientAdmDisDetailID;
            return InPatientAdmDisDetails;
        }

        #endregion
        #region Primitive Properties

        private DeceasedInfo _deceasedInfo;
        [DataMemberAttribute()]
        public DeceasedInfo DeceasedInfo
        {
            get { return _deceasedInfo; }
            set
            {
                _deceasedInfo = value;
                RaisePropertyChanged("DeceasedInfo");
            }
        }

        [DataMemberAttribute()]
        public long InPatientAdmDisDetailID
        {
            get
            {
                return _InPatientAdmDisDetailID;
            }
            set
            {
                OnInPatientAdmDisDetailIDChanging(value);
                _InPatientAdmDisDetailID = value;
                RaisePropertyChanged("InPatientAdmDisDetailID");
                OnInPatientAdmDisDetailIDChanged();
            }
        }
        private long _InPatientAdmDisDetailID;
        partial void OnInPatientAdmDisDetailIDChanging(long value);
        partial void OnInPatientAdmDisDetailIDChanged();




        [DataMemberAttribute()]
        public long PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                OnPtRegistrationIDChanging(value);
                _PtRegistrationID = value;
                RaisePropertyChanged("PtRegistrationID");
                OnPtRegistrationIDChanged();
            }
        }
        private long _PtRegistrationID;
        partial void OnPtRegistrationIDChanging(long value);
        partial void OnPtRegistrationIDChanged();

        [DataMemberAttribute()]
        public DeptLocation DeptLocationObj
        {
            get
            {
                return _DeptLocObj;
            }
            set
            {
                _DeptLocObj = value;
                RaisePropertyChanged("DeptLocationObj");                
            }
        }
        private DeptLocation _DeptLocObj;

        [DataMemberAttribute()]
        public long DeptLocationID
        {
            get
            {
                return _DeptLocationID;
            }
            set
            {
                OnDeptLocationIDChanging(value);
                _DeptLocationID = value;
                RaisePropertyChanged("DeptLocationID");
                OnDeptLocationIDChanged();
            }
        }
        private long _DeptLocationID;
        partial void OnDeptLocationIDChanging(long value);
        partial void OnDeptLocationIDChanged();
        
        [DataMemberAttribute()]
        public long DeptID
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
        private long _DeptID;
        partial void OnDeptIDChanging(long value);
        partial void OnDeptIDChanged();


        [DataMemberAttribute()]
        public Nullable<DateTime> AdmissionDate
        {
            get
            {
                return _AdmissionDate;
            }
            set
            {
                OnAdmissionDateChanging(value);
                _AdmissionDate = value;
                RaisePropertyChanged("AdmissionDate");
                OnAdmissionDateChanged();
            }
        }
        private Nullable<DateTime> _AdmissionDate;
        partial void OnAdmissionDateChanging(Nullable<DateTime> value);
        partial void OnAdmissionDateChanged();


        [DataMemberAttribute()]
        public long V_AdmissionType
        {
            get
            {
                return _V_AdmissionType;
            }
            set
            {
                OnV_AdmissionTypeChanging(value);
                _V_AdmissionType = value;
                RaisePropertyChanged("V_AdmissionType");
                OnV_AdmissionTypeChanged();
            }
        }
        private long _V_AdmissionType = -1;
        partial void OnV_AdmissionTypeChanging(long value);
        partial void OnV_AdmissionTypeChanged();

        [DataMemberAttribute()]
        public string AdmissionNote
        {
            get
            {
                return _AdmissionNote;
            }
            set
            {
                OnAdmissionNoteChanging(value);
                _AdmissionNote = value;
                RaisePropertyChanged("AdmissionNote");
                OnAdmissionNoteChanged();
            }
        }
        private string _AdmissionNote;
        partial void OnAdmissionNoteChanging(string value);
        partial void OnAdmissionNoteChanged();


        private long? _dsNumber;
        [DataMemberAttribute()]
        public long? DSNumber
        {
            get { return _dsNumber; }
            set
            {
                _dsNumber = value;
                RaisePropertyChanged("DSNumber");
            }
        }

        [DataMemberAttribute()]
        public Nullable<DateTime> DischargeDate
        {
            get
            {
                return _DischargeDate;
            }
            set
            {
                OnDischargeDateChanging(value);
                _DischargeDate = value;
                RaisePropertyChanged("DischargeDate");
                OnDischargeDateChanged();
            }
        }
        private Nullable<DateTime> _DischargeDate;
        partial void OnDischargeDateChanging(Nullable<DateTime> value);
        partial void OnDischargeDateChanged();

        
        [DataMemberAttribute()]
        public Nullable<DateTime> DischargeDetailRecCreatedDate
        {
            get
            {
                return _DischargeDetailRecCreatedDate;
            }
            set
            {
                OnDischargeDetailRecCreatedDateChanging(value);
                _DischargeDetailRecCreatedDate = value;
                RaisePropertyChanged("DischargeDetailRecCreatedDate");
                OnDischargeDetailRecCreatedDateChanged();
            }
        }
        private Nullable<DateTime> _DischargeDetailRecCreatedDate;
        partial void OnDischargeDetailRecCreatedDateChanging(Nullable<DateTime> value);
        partial void OnDischargeDetailRecCreatedDateChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> TempDischargeDate
        {
            get
            {
                return _TempDischargeDate;
            }
            set
            {
                OnTempDischargeDateChanging(value);
                _TempDischargeDate = value;
                RaisePropertyChanged("TempDischargeDate");
                OnTempDischargeDateChanged();
            }
        }
        private Nullable<DateTime> _TempDischargeDate;
        partial void OnTempDischargeDateChanging(Nullable<DateTime> value);
        partial void OnTempDischargeDateChanged();


        [DataMemberAttribute()]
        public AllLookupValues.V_DischargeType? V_DischargeType
        {
            get
            {
                return _V_DischargeType;
            }
            set
            {
                _V_DischargeType = value;
                RaisePropertyChanged("V_DischargeType");
            }
        }
        private AllLookupValues.V_DischargeType? _V_DischargeType;

        private string _dischargeCode;
        [DataMemberAttribute()]
        public string DischargeCode
        {
            get { return _dischargeCode; }
            set
            {
                _dischargeCode = value;
                RaisePropertyChanged("DischargeCode");
            }
        }

        private string _dischargeCode2;
        [DataMemberAttribute()]
        public string DischargeCode2
        {
            get { return _dischargeCode2; }
            set
            {
                _dischargeCode2 = value;
                RaisePropertyChanged("DischargeCode2");
            }
        }

        private AllLookupValues.DischargeCondition? _vDischargeCondition;

        public AllLookupValues.DischargeCondition? VDischargeCondition
        {
            get { return _vDischargeCondition; }
            set
            {
                _vDischargeCondition = value;
                RaisePropertyChanged("VDischargeCondition");
            }
        }

        [DataMemberAttribute()]
        public string DischargeNote2
        {
            get
            {
                return _DischargeNote2;
            }
            set
            {
                _DischargeNote2 = value;
                RaisePropertyChanged("DischargeNote2");
            }
        }
        private string _DischargeNote2;

        [DataMemberAttribute()]
        public string DischargeNote
        {
            get
            {
                return _DischargeNote;
            }
            set
            {
                OnDischargeNoteChanging(value);
                _DischargeNote = value;
                RaisePropertyChanged("DischargeNote");
                OnDischargeNoteChanged();
            }
        }
        private string _DischargeNote;
        partial void OnDischargeNoteChanging(string value);
        partial void OnDischargeNoteChanged();


        [DataMemberAttribute()]
        public DiagnosisTreatment DiagnosisTreatmentInfo
        {
            get
            {
                return _DiagnosisTreatmentInfo;
            }
            set
            {
                _DiagnosisTreatmentInfo = value;
                RaisePropertyChanged("DiagnosisTreatmentInfo");
            }
        }
        private DiagnosisTreatment _DiagnosisTreatmentInfo;

        [DataMemberAttribute()]
        public string Comment
        {
            get
            {
                return _Comment;
            }
            set
            {
                _Comment = value;
                RaisePropertyChanged("Comment");
            }
        }
        private string _Comment;

        [DataMemberAttribute()]
        public string Surgeon
        {
            get
            {
                return _Surgeon;
            }
            set
            {
                _Surgeon = value;
                RaisePropertyChanged("Surgeon");
            }
        }
        private string _Surgeon;

        [DataMemberAttribute()]
        public string Therapist
        {
            get
            {
                return _Therapist;
            }
            set
            {
                _Therapist = value;
                RaisePropertyChanged("Therapist");
            }
        }
        private string _Therapist;

        [DataMemberAttribute()]
        public RefDepartment DischargeDepartment
        {
            get
            {
                return _DischargeDepartment;
            }
            set
            {
                _DischargeDepartment = value;
                RaisePropertyChanged("DischargeDepartment");
            }
        }
        private RefDepartment _DischargeDepartment;

        [DataMemberAttribute()]
        public bool ConfirmNotTreatedAsInPt
        {
            get
            {
                return _ConfirmNotTreatedAsInPt;
            }
            set
            {
                _ConfirmNotTreatedAsInPt = value;
                RaisePropertyChanged("ConfirmNotTreatedAsInPt");
            }
        }
        private bool _ConfirmNotTreatedAsInPt;

        [DataMemberAttribute()]
        public string HuongDieuTri
        {
            get
            {
                return _HuongDieuTri;
            }
            set
            {
                _HuongDieuTri = value;
                RaisePropertyChanged("HuongDieuTri");
            }
        }
        private string _HuongDieuTri;

        [DataMemberAttribute()]
        public string HosTransferIn
        {
            get
            {
                return _HosTransferIn;
            }
            set
            {
                _HosTransferIn = value;
                RaisePropertyChanged("HosTransferIn");
            }
        }
        private string _HosTransferIn;

        [DataMemberAttribute()]
        public long HosTransferInID
        {
            get
            {
                return _HosTransferInID;
            }
            set
            {
                _HosTransferInID = value;
                RaisePropertyChanged("HosTransferInID");
            }
        }
        private long _HosTransferInID;

        [DataMemberAttribute()]
        public string HosTransferOut
        {
            get
            {
                return _HosTransferOut;
            }
            set
            {
                _HosTransferOut = value;
                RaisePropertyChanged("HosTransferOut");
            }
        }
        private string _HosTransferOut;

        [DataMemberAttribute()]
        public long HosTransferOutID
        {
            get
            {
                return _HosTransferOutID;
            }
            set
            {
                _HosTransferOutID = value;
                RaisePropertyChanged("HosTransferOutID");
            }
        }
        private long _HosTransferOutID;

        [DataMemberAttribute()]
        public string OperationDoctor
        {
            get
            {
                return _OperationDoctor;
            }
            set
            {
                _OperationDoctor = value;
                RaisePropertyChanged("OperationDoctor");
            }
        }
        private string _OperationDoctor;

        [DataMemberAttribute()]
        public string ReferralDiagnosis
        {
            get
            {
                return _ReferralDiagnosis;
            }
            set
            {
                _ReferralDiagnosis = value;
                RaisePropertyChanged("ReferralDiagnosis");
            }
        }
        private string _ReferralDiagnosis;


        [DataMemberAttribute()]
        public bool IsDoctorCreatedDischargePaper
        {
            get
            {
                return _IsDoctorCreatedDischargePaper;
            }
            set
            {
                _IsDoctorCreatedDischargePaper = value;
                RaisePropertyChanged("IsDoctorCreatedDischargePaper");
            }
        }
        private bool _IsDoctorCreatedDischargePaper;

        [DataMemberAttribute()]
        public long V_AccidentCode
        {
            get
            {
                return _V_AccidentCode;
            }
            set
            {
                _V_AccidentCode = value;
                RaisePropertyChanged("V_AccidentCode");
            }
        }
        private long _V_AccidentCode = -1;

        [DataMemberAttribute()]
        public DateTime? SurgeryDate
        {
            get
            {
                return _SurgeryDate;
            }
            set
            {
                _SurgeryDate = value;
                RaisePropertyChanged("SurgeryDate");
            }
        }
        private DateTime? _SurgeryDate;


        [DataMemberAttribute()]
        public DateTime? CardiacCatheterDate
        {
            get
            {
                return _CardiacCatheterDate;
            }
            set
            {
                _CardiacCatheterDate = value;
                RaisePropertyChanged("CardiacCatheterDate");
            }
        }
        private DateTime? _CardiacCatheterDate;

        /*▼====: #001*/
        [DataMemberAttribute()]
        public AllLookupValues.DeadReason? V_DeadReason
        {
            get
            {
                return _V_DeadReason;
            }
            set
            {
                _V_DeadReason = value;
                RaisePropertyChanged("V_DeadReason");
            }
        }
        private AllLookupValues.DeadReason? _V_DeadReason;
        /*▲====: #001*/
        [DataMemberAttribute()]
        public bool? IsConfirmEmergencyTreatment
        {
            get
            {
                return _IsConfirmEmergencyTreatment;
            }
            set
            {
                _IsConfirmEmergencyTreatment = value;
                RaisePropertyChanged("IsConfirmEmergencyTreatment");
            }
        }
        private bool? _IsConfirmEmergencyTreatment;

        private string _MedicalHistory;
        [DataMemberAttribute()]
        public string MedicalHistory
        {
            get { return _MedicalHistory; }
            set
            {
                _MedicalHistory = value;
                RaisePropertyChanged("MedicalHistory");
            }
        }
        private string _DischargeStatus;
        [DataMemberAttribute()]
        public string DischargeStatus
        {
            get { return _DischargeStatus; }
            set
            {
                _DischargeStatus = value;
                RaisePropertyChanged("DischargeStatus");
            }
        }
        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public Allocation Allocation
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public RefMedicalServiceItem RefMedicalServiceItem
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<HospitalizationHistoryDetail> HospitalizationHistoryDetails
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public RefDepartment Department
        {
            get { return _department; }
            set
            {
                if (_department != value)
                {
                    _department = value;
                    RaisePropertyChanged("Department");
                }
            }
        }
        private RefDepartment _department;

        [DataMemberAttribute()]
        public Lookup VAdmissionType
        {
            get
            {
                return _VAdmissionType;
            }
            set
            {
                OnVAdmissionTypeChanging(value);
                _VAdmissionType = value;
                RaisePropertyChanged("VAdmissionType");
                OnVAdmissionTypeChanged();
            }
        }
        private Lookup _VAdmissionType;
        partial void OnVAdmissionTypeChanging(Lookup value);
        partial void OnVAdmissionTypeChanged();


        [DataMemberAttribute()]
        public Lookup VDischargeType
        {
            get
            {
                return _VDischargeType;
            }
            set
            {
                OnVDischargeTypeChanging(value);
                _VDischargeType = value;
                RaisePropertyChanged("VDischargeType");
                OnVDischargeTypeChanged();
            }
        }
        private Lookup _VDischargeType;
        partial void OnVDischargeTypeChanging(Lookup value);
        partial void OnVDischargeTypeChanged();

        [DataMemberAttribute()]
        public Lookup V_DischargeCondition
        {
            get
            {
                return _V_DischargeCondition;
            }
            set
            {
                OnV_DischargeConditionChanging(value);
                _V_DischargeCondition = value;
                RaisePropertyChanged("V_DischargeCondition");
                OnV_DischargeConditionChanged();
            }
        }
        private Lookup _V_DischargeCondition;
        partial void OnV_DischargeConditionChanging(Lookup value);
        partial void OnV_DischargeConditionChanged();

        [DataMemberAttribute()]
        public int TotalDaysOfTreatment 
        {
            get
            {
                return _TotalDaysOfTreatment;
            }
            set
            {
                OnTotalDaysOfTreatmentChanging(value);
                _TotalDaysOfTreatment = value;
                RaisePropertyChanged("TotalDaysOfTreatment");
                OnTotalDaysOfTreatmentChanged();
            }
        }
        private int _TotalDaysOfTreatment;
        partial void OnTotalDaysOfTreatmentChanging(int value);
        partial void OnTotalDaysOfTreatmentChanged();

        private PatientRegistration _patientRegistration;
        [DataMemberAttribute()]
        public PatientRegistration PatientRegistration
        {
            get
            {
                return _patientRegistration;
            }
            set
            {
                _patientRegistration = value;
                RaisePropertyChanged("PatientRegistration");
            }
        }

        private ObservableCollection<InPatientDeptDetail> _inPatientDeptDetails;
        [DataMemberAttribute()]
        public ObservableCollection<InPatientDeptDetail> InPatientDeptDetails
        {
            get
            {
                return _inPatientDeptDetails;
            }
            set
            {
                _inPatientDeptDetails = value;
                RaisePropertyChanged("InPatientDeptDetails");
            }
        }

        #endregion

        private long? _MedServiceItemPriceListID;
        private long? _PCLExamTypePriceListID;
        private long? _DrugDeptPriceGroupID;
        private bool _IsGuestEmergencyAdmission;
        [DataMemberAttribute]
        public long? MedServiceItemPriceListID
        {
            get
            {
                return _MedServiceItemPriceListID;
            }
            set
            {
                _MedServiceItemPriceListID = value;
                RaisePropertyChanged("MedServiceItemPriceListID");
            }
        }
        [DataMemberAttribute]
        public long? PCLExamTypePriceListID
        {
            get
            {
                return _PCLExamTypePriceListID;
            }
            set
            {
                _PCLExamTypePriceListID = value;
                RaisePropertyChanged("PCLExamTypePriceListID");
            }
        }
        [DataMemberAttribute]
        public long? DrugDeptPriceGroupID
        {
            get
            {
                return _DrugDeptPriceGroupID;
            }
            set
            {
                _DrugDeptPriceGroupID = value;
                RaisePropertyChanged("DrugDeptPriceGroupID");
            }
        }
        [DataMemberAttribute]
        public bool IsGuestEmergencyAdmission
        {
            get
            {
                return _IsGuestEmergencyAdmission;
            }
            set
            {
                if (_IsGuestEmergencyAdmission == value)
                {
                    return;
                }
                _IsGuestEmergencyAdmission = value;
                RaisePropertyChanged("IsGuestEmergencyAdmission");
            }
        }
        public long TransferFormID { get; set; }

        private bool _IsTreatmentCOVID;
        [DataMemberAttribute]
        public bool IsTreatmentCOVID
        {
            get
            {
                return _IsTreatmentCOVID;
            }
            set
            {
                if (_IsTreatmentCOVID == value)
                {
                    return;
                }
                _IsTreatmentCOVID = value;
                RaisePropertyChanged("IsTreatmentCOVID");
            }
        }

        //▼====: #002
        private long _V_ObjectType;
        [DataMemberAttribute]
        public long V_ObjectType
        {
            get
            {
                return _V_ObjectType;
            }
            set
            {
                if (_V_ObjectType == value)
                {
                    return;
                }
                _V_ObjectType = value;
                RaisePropertyChanged("V_ObjectType");
            }
        }

        private bool _IsPostponementAdvancePayment;
        [DataMemberAttribute]
        public bool IsPostponementAdvancePayment
        {
            get
            {
                return _IsPostponementAdvancePayment;
            }
            set
            {
                if (_IsPostponementAdvancePayment == value)
                {
                    return;
                }
                _IsPostponementAdvancePayment = value;
                RaisePropertyChanged("IsPostponementAdvancePayment");
            }
        }

        private string _PostponementAdvancePaymentNote;
        [DataMemberAttribute]
        public string PostponementAdvancePaymentNote
        {
            get
            {
                return _PostponementAdvancePaymentNote;
            }
            set
            {
                _PostponementAdvancePaymentNote = value;
                RaisePropertyChanged("PostponementAdvancePaymentNote");
            }
        }
        //▲====: #002

        //▼====: #003
        private bool _EnableSearchRegistration_InPt;
        [DataMemberAttribute]
        public bool EnableSearchRegistration_InPt
        {
            get
            {
                return _EnableSearchRegistration_InPt;
            }
            set
            {
                if (_EnableSearchRegistration_InPt == value)
                {
                    return;
                }
                _EnableSearchRegistration_InPt = value;
                RaisePropertyChanged("EnableSearchRegistration_InPt");
            }
        }

        private bool _IsConfirmedPostponement;
        [DataMemberAttribute]
        public bool IsConfirmedPostponement
        {
            get
            {
                return _IsConfirmedPostponement;
            }
            set
            {
                if (_IsConfirmedPostponement == value)
                {
                    return;
                }
                _IsConfirmedPostponement = value;
                RaisePropertyChanged("IsConfirmedPostponement");
            }
        }

        private long _PostponementStaff;
        [DataMemberAttribute]
        public long PostponementStaff
        {
            get
            {
                return _PostponementStaff;
            }
            set
            {
                if (_PostponementStaff == value)
                {
                    return;
                }
                _PostponementStaff = value;
                RaisePropertyChanged("PostponementStaff");
            }
        }
        
        private DateTime? _PostponementDate;
        [DataMemberAttribute()]
        public DateTime? PostponementDate
        {
            get
            {
                return _PostponementDate;
            }
            set
            {
                _PostponementDate = value;
                RaisePropertyChanged("PostponementDate");
            }
        }
        //▲====: #003

        //▼====: #004
        private bool _IsSevereIllness;
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
        //▲====: #004

        //▼====: #005
        private bool _IsReturning;
        public bool IsReturning
        {
            get
            {
                return _IsReturning;
            }
            set
            {
                if (_IsReturning == value)
                {
                    return;
                }
                _IsReturning = value;
                RaisePropertyChanged("IsReturning");
            }
        }
        //▲====: #005
        //▼====: #006
        private string _TreatmentDischarge;
        public string TreatmentDischarge
        {
            get
            {
                return _TreatmentDischarge;
            }
            set
            {
                if (_TreatmentDischarge == value)
                {
                    return;
                }
                _TreatmentDischarge = value;
                RaisePropertyChanged("TreatmentDischarge");
            }
        }
        //▲====: #006

        //▼====: #007
        private DischargePapersInfo _DischargePapersInfo;
        [DataMemberAttribute()]
        public DischargePapersInfo DischargePapersInfo
        {
            get { return _DischargePapersInfo; }
            set
            {
                _DischargePapersInfo = value;
                RaisePropertyChanged("DischargePapersInfo");
            }
        }
        //▲====: #007

        private string _PathologicalProcess;
        public string PathologicalProcess
        {
            get
            {
                return _PathologicalProcess;
            }
            set
            {
                if (_PathologicalProcess == value)
                {
                    return;
                }
                _PathologicalProcess = value;
                RaisePropertyChanged("PathologicalProcess");
            }
        }

        private bool _IsNeedTreatmentSummary;
        public bool IsNeedTreatmentSummary
        {
            get
            {
                return _IsNeedTreatmentSummary;
            }
            set
            {
                if (_IsNeedTreatmentSummary == value)
                {
                    return;
                }
                _IsNeedTreatmentSummary = value;
                RaisePropertyChanged("IsNeedTreatmentSummary");
            }
        }

        private string _DiagnosisTreatmentSummary;
        public string DiagnosisTreatmentSummary
        {
            get
            {
                return _DiagnosisTreatmentSummary;
            }
            set
            {
                if (_DiagnosisTreatmentSummary == value)
                {
                    return;
                }
                _DiagnosisTreatmentSummary = value;
                RaisePropertyChanged("DiagnosisTreatmentSummary");
            }
        }
    }
    public partial class InPatientAdmDisDetailSearchCriteria : InPatientAdmDisDetails
    {
        public InPatientAdmDisDetailSearchCriteria():base()
        {
            
        }
        [DataMemberAttribute()]
        public DateTime FromDate
        {
            get
            {
                return _FromDate;
            }
            set
            {
                OnFromDateChanging(value);
                _FromDate = value;
                RaisePropertyChanged("FromDate");
                OnFromDateChanged();
            }
        }
        private DateTime _FromDate=DateTime.Now.Date;
        partial void OnFromDateChanging(DateTime value);
        partial void OnFromDateChanged();

        [DataMemberAttribute()]
        public DateTime ToDate
        {
            get
            {
                return _ToDate;
            }
            set
            {
                OnToDateChanging(value);
                _ToDate = value;
                RaisePropertyChanged("ToDate");
                OnToDateChanged();
            }
        }
        private DateTime _ToDate=DateTime.Now.Date;
        partial void OnToDateChanging(DateTime value);
        partial void OnToDateChanged();

        [DataMemberAttribute()]
        public new long DeptID
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
        private long _DeptID;
        partial void OnDeptIDChanging(long value);
        partial void OnDeptIDChanged();

        [DataMemberAttribute()]
        public string FullName
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
        private string _FullName;
        partial void OnFullNameChanging(string value);
        partial void OnFullNameChanged();

        [DataMemberAttribute()]
        public string PatientCode
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
        private string _PatientCode;
        partial void OnPatientCodeChanging(string value);
        partial void OnPatientCodeChanged();
    }
}
