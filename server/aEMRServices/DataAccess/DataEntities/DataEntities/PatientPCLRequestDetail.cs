using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
/*
 * 20221027 #001 QTD:  Thêm trường đánh dấu ưu tiên khi cấp số QMS
 */
namespace DataEntities
{
    [DataContract(IsReference = true)]
    public partial class PatientPCLRequestDetail : MedRegItemBase
    {
        public PatientPCLRequestDetail():base()
        {
            MedProductType = AllLookupValues.MedProductType.CAN_LAM_SANG;
            ExamRegStatus = AllLookupValues.ExamRegStatus.DANG_KY_KHAM;
        }
        #region Factory Method


        /// Create a new PatientPCLRequestDetails object.

        /// <param name="PCLReqItemID">Initial value of the PCLReqItemID property.</param>
        /// <param name="pCLItemID">Initial value of the PCLItemID property.</param>
        /// <param name="patientPCLAppID">Initial value of the PatientPCLAppID property.</param>
        public static PatientPCLRequestDetail CreatePatientPCLRequestDetails(long pCLReqItemID, long pclExamTypeID, long patientPCLReqID)
        {
            PatientPCLRequestDetail patientPCLRequestDetails = new PatientPCLRequestDetail();
            patientPCLRequestDetails.PCLReqItemID = pCLReqItemID;
            patientPCLRequestDetails.PCLExamTypeID = pclExamTypeID;
            patientPCLRequestDetails.PatientPCLReqID= patientPCLReqID;
            return patientPCLRequestDetails;
        }

        #endregion
        public bool SequenceNoReassigned { get; set; }
        #region Primitive Properties
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
        [DataMemberAttribute]
        public override long ID
        {
            get { return PCLReqItemID; }
        }
        [DataMemberAttribute()]
        public long PCLReqItemID
        {
            get
            {
                return _PCLReqItemID;
            }
            set
            {
                if (_PCLReqItemID != value)
                {
                    OnPCLReqItemIDChanging(value);
                    ////ReportPropertyChanging("PCLReqItemID");
                    _PCLReqItemID = value;
                    RaisePropertyChanged("PCLReqItemID");
                    OnPCLReqItemIDChanged();
                }
            }
        }
        private long _PCLReqItemID;
        partial void OnPCLReqItemIDChanging(long value);
        partial void OnPCLReqItemIDChanged();

        [DataMemberAttribute()]
        public long PCLExamTypeID
        {
            get
            {
                return _PCLExamTypeID;
            }
            set
            {
                _PCLExamTypeID = value;
                RaisePropertyChanged("PCLExamTypeID");
            }
        }
        private long _PCLExamTypeID;

        [DataMemberAttribute()]
        public long PatientPCLReqID
        {
            get
            {
                return _PatientPCLReqID;
            }
            set
            {
                OnPatientPCLReqIDChanging(value);
                ////ReportPropertyChanging("PatientPCLReqID");
                _PatientPCLReqID = value;
                RaisePropertyChanged("PatientPCLReqID");
                OnPatientPCLReqIDChanged();
            }
        }
        private long _PatientPCLReqID;
        partial void OnPatientPCLReqIDChanging(long value);
        partial void OnPatientPCLReqIDChanged();
     
        [DataMemberAttribute()]
        public Nullable<Byte> NumberOfTest
        {
            get
            {
                return _NumberOfTest;
            }
            set
            {
                OnNumberOfTestChanging(value);
                _NumberOfTest = value;
                RaisePropertyChanged("NumberOfTest");
                OnNumberOfTestChanged();
            }
        }
        private Nullable<Byte> _NumberOfTest;
        partial void OnNumberOfTestChanging(Nullable<Byte> value);
        partial void OnNumberOfTestChanged();

        #endregion

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

        #region Navigation Properties

        //NY cung de tach phong
        [DataMemberAttribute()]
        public ObservableCollection<DeptLocation> ObjDeptLocIDList
        {
            get
            {
                return _ObjDeptLocIDList;
            }
            set
            {
                if (_ObjDeptLocIDList != value)
                {
                    _ObjDeptLocIDList = value;
                    RaisePropertyChanged("ObjDeptLocIDList");
                }
            }
        }
        private ObservableCollection<DeptLocation> _ObjDeptLocIDList;



        [DataMemberAttribute()]
        public PatientPCLRequest PatientPCLRequest
        {
            get { return _PatientPCLRequest; }
            set
            {
                if (_PatientPCLRequest != value)
                {
                    OnPatientPCLRequestChanging(value);
                    _PatientPCLRequest = value;
                    // Txd 03/11/2013
                    //if (_PatientPCLRequest != null && _PatientPCLRequest.StaffID!=null)
                    //{
                    //    PaidStaffID = _PatientPCLRequest.StaffID.Value;
                    //}
                    RaisePropertyChanged("PatientPCLRequest");
                    OnPatientPCLRequestChanged();
                }
            }
        }
        private PatientPCLRequest _PatientPCLRequest;
        partial void OnPatientPCLRequestChanging(PatientPCLRequest value);
        partial void OnPatientPCLRequestChanged();


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

       
        private PCLExamType _pclExamType;
        [DataMemberAttribute()]
        public PCLExamType PCLExamType
        {
            get
            {
                return _pclExamType;
            }
            set
            {
                if (_pclExamType != value)
                {
                    _pclExamType = value;
                    RaisePropertyChanged("PCLExamType");
                }
            }
        }

        private DeptLocation _deptLocation;
        [DataMemberAttribute()]
        public DeptLocation DeptLocation
        {
            get
            {
                return _deptLocation;
            }
            set
            {
                _deptLocation = value;
                RaisePropertyChanged("DeptLocation");
            }
        }

        #endregion

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

        public override IGenericService GenericServiceItem
        {
            get
            {
                return _pclExamType;
            }
        }

        public override IChargeableItemPrice ChargeableItem
        {
            get
            {
                return _pclExamType;
            }
        }

        public override bool Equals(object obj)
        {
            PatientPCLRequestDetail info = obj as PatientPCLRequestDetail;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PCLReqItemID > 0 && this.PCLReqItemID == info.PCLReqItemID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        //EXT
        [DataMemberAttribute()]
        public Nullable<bool> HasResult
        {
            get { return _HasResult; }
            set
            {
                if (_HasResult != value)
                {
                    OnHasResultChanging(value);
                    _HasResult = value;
                    RaisePropertyChanged("HasResult");
                    OnHasResultChanged();
                }
            }
        }
        private Nullable<bool> _HasResult;
        partial void OnHasResultChanging(Nullable<bool> value);
        partial void OnHasResultChanged();
        //EXT


        [DataMemberAttribute()]
        public String V_ExamRegStatusName
        {
            get
            {
                return _V_ExamRegStatusName;
            }
            set
            {
                OnV_ExamRegStatusNameChanging(value);
                _V_ExamRegStatusName = value;
                RaisePropertyChanged("V_ExamRegStatusName");
                OnV_ExamRegStatusNameChanged();
            }
        }
        private String _V_ExamRegStatusName;
        partial void OnV_ExamRegStatusNameChanging(String value);
        partial void OnV_ExamRegStatusNameChanged();


        [DataMemberAttribute()]
        public String PCLSectionName
        {
            get
            {
                return _PCLSectionName;
            }
            set
            {
                OnPCLSectionNameChanging(value);
                _PCLSectionName = value;
                RaisePropertyChanged("PCLSectionName");
                OnPCLSectionNameChanged();
            }
        }
        private String _PCLSectionName;
        partial void OnPCLSectionNameChanging(String value);
        partial void OnPCLSectionNameChanged();

        public bool LocationChanged { get; set; }

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

        private DateTime _PCLReqDetailCancelDate;
        [DataMemberAttribute()]
        public DateTime PCLReqDetailCancelDate
        {
            get
            {
                return _PCLReqDetailCancelDate;
            }
            set
            {
                if (_PCLReqDetailCancelDate != value)
                {
                    _PCLReqDetailCancelDate = value;
                    RaisePropertyChanged("PCLReqDetailCancelDate");
                }
            }
        }

        private long _PCLReqDetailCancelStaffID;
        [DataMemberAttribute()]
        public long PCLReqDetailCancelStaffID
        {
            get
            {
                return _PCLReqDetailCancelStaffID;
            }
            set
            {
                if (_PCLReqDetailCancelStaffID != value)
                {
                    _PCLReqDetailCancelStaffID = value;
                    RaisePropertyChanged("PCLReqDetailCancelStaffID");
                }
            }
        }

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

        private decimal _Qty = 1;
        [Range(0, int.MaxValue, ErrorMessage = "Giá trị số lượng không hợp lệ")]
        [DataMemberAttribute()]
        public override decimal Qty
        {
            get
            {
                return _Qty;
            }
            set
            {
                if (_Qty != value)
                {
                    _Qty = value;
                    RaisePropertyChanged("Qty");
                }
            }
        }

        // 20181212 TNHX [BM0005404] Doesn't print PhieuChiDinh if Doctor already printed it 
        private int _RequestedByDoctor = 0;
        [Range(0, 1, ErrorMessage = "Giá trị RequestedByDoctor không hợp lệ")]
        [DataMemberAttribute()]
        public int RequestedByDoctor
        {
            get
            {
                return _RequestedByDoctor;
            }
            set
            {
                if (_RequestedByDoctor != value)
                {
                    _RequestedByDoctor = value;
                    RaisePropertyChanged("RequestedByDoctor");
                }
            }
        }

        private bool? _IsDoctorChooseHI;
        [DataMemberAttribute()]
        public bool? IsDoctorChooseHI
        {
            get { return _IsDoctorChooseHI; }
            set
            {
                if (_IsDoctorChooseHI != value)
                {
                    _IsDoctorChooseHI = value;
                    RaisePropertyChanged("IsDoctorChooseHI");
                }
            }
        }
        private string _DiagnosisForCheck;
        [DataMemberAttribute()]
        public string DiagnosisForCheck
        {
            get { return _DiagnosisForCheck; }
            set
            {
                if (_DiagnosisForCheck != value)
                {
                    _DiagnosisForCheck = value;
                    RaisePropertyChanged("DiagnosisForCheck");
                }
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
        public String Result
        {
            get
            {
                return _Result;
            }
            set
            {
                OnResultChanging(value);
                _Result = value;
                RaisePropertyChanged("Result");
                OnResultChanged();
            }
        }
        private String _Result;
        partial void OnResultChanging(String value);
        partial void OnResultChanged();
        // Bsi doc ket qua
        [DataMemberAttribute()]
        public Staff ResultDoctorStaff
        {
            get
            {
                return _ResultDoctorStaff;
            }
            set
            {
                _ResultDoctorStaff = value;
                RaisePropertyChanged("ResultDoctorStaff");
            }
        }
        private Staff _ResultDoctorStaff;
        // BsiThuc hien
        [DataMemberAttribute()]
        public Staff PerformDoctorStaff
        {
            get
            {
                return _PerformDoctorStaff;
            }
            set
            {
                _PerformDoctorStaff = value;
                RaisePropertyChanged("PerformDoctorStaff");
            }
        }
        private Staff _PerformDoctorStaff;

        [DataMemberAttribute()]
        public long? UserOfficialAccountID
        {
            get
            {
                return _UserOfficialAccountID;
            }
            set
            {
                _UserOfficialAccountID = value;
                RaisePropertyChanged("UserOfficialAccountID");
            }
        }
        private long? _UserOfficialAccountID;

        [DataMemberAttribute()]
        public bool IsPriority
        {
            get
            {
                return _IsPriority;
            }
            set
            {
                if (_IsPriority != value)
                {
                    _IsPriority = value;
                    RaisePropertyChanged("IsPriority");
                }
            }
        }
        private bool _IsPriority;
    }
}