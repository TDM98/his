using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
/*
 * 20221128 #001 TNHX: 1978 Thêm biên ResultID để lấy kết quả đi kèm phiếu chỉ định CLS
 * 20230626 #002 QTD:  Thêm các trường cho màn hình quản lý danh sách hẹn
 */
namespace DataEntities
{
    //[DataContract]
    [DataContract(IsReference = true)]
    public partial class PatientPCLRequest : MedRegItemBase, IInvoiceItem
    {
        public PatientPCLRequest()
        {
            //ExamRegStatus = AllLookupValues.ExamRegStatus.DANG_KY_KHAM;
            V_PCLRequestStatus = AllLookupValues.V_PCLRequestStatus.OPEN;
        }
        #region Factory Method


        /// Create a new PatientPCLRequest object.

        /// <param name="patientPCLReqID">Initial value of the PatientPCLReqID property.</param>
        /// <param name="diagnosis">Initial value of the Diagnosis property.</param>
        public static PatientPCLRequest CreatePatientPCLRequest(long patientPCLReqID, String diagnosis)
        {
            PatientPCLRequest patientPCLRequest = new PatientPCLRequest();
            patientPCLRequest.PatientPCLReqID = patientPCLReqID;
            patientPCLRequest.Diagnosis = diagnosis;
            return patientPCLRequest;
        }

        #endregion
        #region Primitive Properties


        [DataMemberAttribute()]
        public long PatientPCLReqID
        {
            get
            {
                return _PatientPCLReqID;
            }
            set
            {
                if (_PatientPCLReqID != value)
                {
                    OnPatientPCLReqIDChanging(value);
                    _PatientPCLReqID = value;
                    RaisePropertyChanged("PatientPCLReqID");
                    OnPatientPCLReqIDChanged();
                }
            }
        }
        private long _PatientPCLReqID;
        partial void OnPatientPCLReqIDChanging(long value);
        partial void OnPatientPCLReqIDChanged();


        [DataMemberAttribute()]
        public Nullable<long> PtRegDetailID
        {
            get
            {
                return _PtRegDetailID;
            }
            set
            {
                OnPtRegDetailIDChanging(value);
                _PtRegDetailID = value;
                RaisePropertyChanged("PtRegDetailID");
                OnPtRegDetailIDChanged();
            }
        }
        private Nullable<long> _PtRegDetailID;
        partial void OnPtRegDetailIDChanging(Nullable<long> value);
        partial void OnPtRegDetailIDChanged();


        [DataMemberAttribute()]
        public Nullable<long> ServiceRecID
        {
            get
            {
                return _ServiceRecID;
            }
            set
            {
                OnServiceRecIDChanging(value);
                _ServiceRecID = value;
                RaisePropertyChanged("ServiceRecID");
                OnServiceRecIDChanged();
            }
        }
        private Nullable<long> _ServiceRecID;
        partial void OnServiceRecIDChanging(Nullable<long> value);
        partial void OnServiceRecIDChanged();


        [DataMemberAttribute()]
        public Nullable<long> ReqFromDeptLocID
        {
            get
            {
                return _ReqFromDeptLocID;
            }
            set
            {
                OnReqFromDeptLocIDChanging(value);
                _ReqFromDeptLocID = value;
                RaisePropertyChanged("ReqFromDeptLocID");
                OnReqFromDeptLocIDChanged();
            }
        }
        private Nullable<long> _ReqFromDeptLocID;
        partial void OnReqFromDeptLocIDChanging(Nullable<long> value);
        partial void OnReqFromDeptLocIDChanged();


        [DataMemberAttribute()]
        public String ReqFromDeptLocIDName
        {
            get
            {
                return _ReqFromDeptLocIDName;
            }
            set
            {
                OnReqFromDeptLocIDNameChanging(value);
                _ReqFromDeptLocIDName = value;
                RaisePropertyChanged("ReqFromDeptLocIDName");
                OnReqFromDeptLocIDNameChanged();
            }
        }
        private String _ReqFromDeptLocIDName;
        partial void OnReqFromDeptLocIDNameChanging(String value);
        partial void OnReqFromDeptLocIDNameChanged();


        [DataMemberAttribute()]
        public Nullable<long> PCLDeptLocID
        {
            get
            {
                return _PCLDeptLocID;
            }
            set
            {
                OnPCLDeptLocIDChanging(value);
                _PCLDeptLocID = value;
                RaisePropertyChanged("PCLDeptLocID");
                OnPCLDeptLocIDChanged();
            }
        }
        private Nullable<long> _PCLDeptLocID;
        partial void OnPCLDeptLocIDChanging(Nullable<long> value);
        partial void OnPCLDeptLocIDChanged();

        [DataMemberAttribute()]
        public String PCLDeptLocIDName
        {
            get
            {
                return _PCLDeptLocIDName;
            }
            set
            {
                OnPCLDeptLocIDNameChanging(value);
                _PCLDeptLocIDName = value;
                RaisePropertyChanged("PCLDeptLocIDName");
                OnPCLDeptLocIDNameChanged();
            }
        }
        private String _PCLDeptLocIDName;
        partial void OnPCLDeptLocIDNameChanging(String value);
        partial void OnPCLDeptLocIDNameChanged();

        [DataMemberAttribute()]
        public String Diagnosis
        {
            get
            {
                return _Diagnosis;
            }
            set
            {
                OnDiagnosisChanging(value);
                _Diagnosis = value;
                RaisePropertyChanged("Diagnosis");
                OnDiagnosisChanged();
            }
        }
        private String _Diagnosis = "";
        partial void OnDiagnosisChanging(String value);
        partial void OnDiagnosisChanged();

        [DataMemberAttribute()]
        public String DoctorComments
        {
            get
            {
                return _DoctorComments;
            }
            set
            {
                OnDoctorCommentsChanging(value);
                _DoctorComments = value;
                RaisePropertyChanged("DoctorComments");
                OnDoctorCommentsChanged();
            }
        }
        private String _DoctorComments;
        partial void OnDoctorCommentsChanging(String value);
        partial void OnDoctorCommentsChanged();

        [DataMemberAttribute()]
        public Nullable<Boolean> IsExternalExam
        {
            get
            {
                return _IsExternalExam;
            }
            set
            {
                OnIsExternalExamChanging(value);
                _IsExternalExam = value;
                RaisePropertyChanged("IsExternalExam");
                OnIsExternalExamChanged();
            }
        }
        private Nullable<Boolean> _IsExternalExam;
        partial void OnIsExternalExamChanging(Nullable<Boolean> value);
        partial void OnIsExternalExamChanged();

        [DataMemberAttribute()]
        public Nullable<Boolean> IsImported
        {
            get
            {
                return _IsImported;
            }
            set
            {
                OnIsImportedChanging(value);
                _IsImported = value;
                RaisePropertyChanged("IsImported");
                OnIsImportedChanged();
            }
        }
        private Nullable<Boolean> _IsImported;
        partial void OnIsImportedChanging(Nullable<Boolean> value);
        partial void OnIsImportedChanged();

        [DataMemberAttribute()]
        public Nullable<Boolean> IsCaseOfEmergency
        {
            get
            {
                return _IsCaseOfEmergency;
            }
            set
            {
                OnIsCaseOfEmergencyChanging(value);
                _IsCaseOfEmergency = value;
                RaisePropertyChanged("IsCaseOfEmergency");
                OnIsCaseOfEmergencyChanged();
            }
        }
        private Nullable<Boolean> _IsCaseOfEmergency;
        partial void OnIsCaseOfEmergencyChanging(Nullable<Boolean> value);
        partial void OnIsCaseOfEmergencyChanged();


        [DataMemberAttribute()]
        public Int64 V_PCLMainCategory
        {
            get { return _V_PCLMainCategory; }
            set
            {
                if (_V_PCLMainCategory != value)
                {
                    OnV_PCLMainCategoryChanging(value);
                    _V_PCLMainCategory = value;
                    RaisePropertyChanged("V_PCLMainCategory");
                    OnV_PCLMainCategoryChanged();
                }
            }
        }
        private Int64 _V_PCLMainCategory;
        partial void OnV_PCLMainCategoryChanging(Int64 value);
        partial void OnV_PCLMainCategoryChanged();

        [DataMemberAttribute()]
        public Lookup ObjV_PCLMainCategory
        {
            get { return _ObjV_PCLMainCategory; }
            set
            {
                if (_ObjV_PCLMainCategory != value)
                {
                    OnObjV_PCLMainCategoryChanging(value);
                    _ObjV_PCLMainCategory = value;
                    RaisePropertyChanged("ObjV_PCLMainCategory");
                    OnObjV_PCLMainCategoryChanged();
                }
            }
        }
        private Lookup _ObjV_PCLMainCategory;
        partial void OnObjV_PCLMainCategoryChanging(Lookup value);
        partial void OnObjV_PCLMainCategoryChanged();


        [DataMemberAttribute]
        public Nullable<Int64> PCLResultParamImpID
        {
            get { return _PCLResultParamImpID; }
            set
            {
                if (_PCLResultParamImpID != value)
                {
                    OnPCLResultParamImpIDChanging(value);
                    _PCLResultParamImpID = value;
                    RaisePropertyChanged("PCLResultParamImpID");
                    OnPCLResultParamImpIDChanged();
                }
            }
        }
        private Nullable<Int64> _PCLResultParamImpID;
        partial void OnPCLResultParamImpIDChanging(Nullable<Int64> value);
        partial void OnPCLResultParamImpIDChanged();



        [DataMemberAttribute]
        public PCLResultParamImplementations ObjPCLResultParamImpID
        {
            get { return _ObjPCLResultParamImpID; }
            set
            {
                if (_ObjPCLResultParamImpID != value)
                {
                    OnObjPCLResultParamImpIDChanging(value);
                    _ObjPCLResultParamImpID = value;
                    RaisePropertyChanged("ObjPCLResultParamImpID");
                    OnObjPCLResultParamImpIDChanged();
                }
            }
        }
        private PCLResultParamImplementations _ObjPCLResultParamImpID;
        partial void OnObjPCLResultParamImpIDChanging(PCLResultParamImplementations value);
        partial void OnObjPCLResultParamImpIDChanged();

        [DataMemberAttribute()]
        public string PCLStaffFullName
        {
            get { return _PCLStaffFullName; }
            set
            {
                OnPCLStaffFullNameChanging(value);
                _PCLStaffFullName = value;
                RaisePropertyChanged("PCLStaffFullName");
                OnPCLStaffFullNameChanged();
            }
        }
        private string _PCLStaffFullName;
        partial void OnPCLStaffFullNameChanging(string value);
        partial void OnPCLStaffFullNameChanged();
        #endregion

        #region Navigation Properties
        [DataMemberAttribute()]
        public PatientServiceRecord PatientServiceRecord
        {
            get;
            set;
        }

        private ObservableCollection<PatientPCLRequestDetail> _PatientPCLRequestIndicators;
        [DataMemberAttribute()]
        public ObservableCollection<PatientPCLRequestDetail> PatientPCLRequestIndicators
        {
            get
            {
                return _PatientPCLRequestIndicators;
            }
            set
            {
                if (_PatientPCLRequestIndicators != value)
                {
                    _PatientPCLRequestIndicators = value;
                    RaisePropertyChanged("PatientPCLRequestIndicators");
                }
            }
        }

        //[DataMemberAttribute()]
        //// [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PTPCLRSL_REL_REQPC_PTAPP", "PatientPCLExamResults")]
        //public ObservableCollection<PatientPCLExamResult> PatientPCLExamResults
        //{
        //    get;
        //    set;
        //}

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

        private DateTime _PCLReqCancelDate;
        [DataMemberAttribute()]
        public DateTime PCLReqCancelDate
        {
            get
            {
                return _PCLReqCancelDate;
            }
            set
            {
                if (_PCLReqCancelDate != value)
                {
                    _PCLReqCancelDate = value;
                    RaisePropertyChanged("PCLReqCancelDate");
                }
            }
        }

        private long _PCLReqCancelStaffID;
        [DataMemberAttribute()]
        public long PCLReqCancelStaffID
        {
            get
            {
                return _PCLReqCancelStaffID;
            }
            set
            {
                if (_PCLReqCancelStaffID != value)
                {
                    _PCLReqCancelStaffID = value;
                    RaisePropertyChanged("PCLReqCancelStaffID");
                }
            }
        }

        private bool _hiApplied = true;
        [DataMemberAttribute()]
        public new bool HiApplied
        {
            get { return _hiApplied; }
            set
            {
                _hiApplied = value;
                RaisePropertyChanged("HiApplied");
            }
        }

        public override long ID { get; set; }

        public new decimal InvoicePrice
        {
            get; set;
        }

        public new decimal? HIAllowedPrice
        {
            get; set;
        }
        public override decimal? MaskedHIAllowedPrice
        {
            get
            {
                return HIAllowedPrice;
            }
        }
        public new decimal PriceDifference
        {
            get; set;
        }

        public new decimal HIPayment
        {
            get; set;
        }

        public new decimal PatientCoPayment
        {
            get; set;
        }

        public new decimal PatientPayment
        {
            get; set;
        }

        public new decimal Qty
        {
            get; set;
        }

        public new IChargeableItemPrice ChargeableItem { get; set; }

        public new double? HIBenefit
        {
            get; set;
        }

        private DateTime? _paidTime;
        /// <summary>
        /// Ngay tra tien. Neu co gia tri => item nay da duoc tra tien roi.
        /// </summary>
        [DataMemberAttribute()]
        public new DateTime? PaidTime
        {
            get
            {
                return _paidTime;
            }
            set
            {
                _paidTime = value;
                RaisePropertyChanged("PaidTime");
                RaisePropertyChanged("IsPaid");
            }
        }

        private DateTime? _refundTime;
        /// <summary>
        /// Ngay hoan tien. Neu co gia tri => item nay da duoc tra tien roi.
        /// </summary>
        [DataMemberAttribute()]
        public new DateTime? RefundTime
        {
            get
            {
                return _refundTime;
            }
            set
            {
                _refundTime = value;
                RaisePropertyChanged("RefundTime");
            }
        }
        [DataMemberAttribute()]
        public DateTime ReceptionTime
        {
            get
            {
                return _ReceptionTime;
            }
            set
            {
                OnReceptionTimeChanging(value);
                _ReceptionTime = value;
                RaisePropertyChanged("ReceptionTime");
                OnReceptionTimeChanged();
            }
        }
        private DateTime _ReceptionTime;
        partial void OnReceptionTimeChanging(DateTime value);
        partial void OnReceptionTimeChanged();


        public new AllLookupValues.ExamRegStatus ExamRegStatus
        {
            get
            {
                //return _examRegStatus;
                //Cho tuong thich code cu:
                if (V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.CANCEL)
                {
                    return AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI;
                }
                else
                {
                    return AllLookupValues.ExamRegStatus.KHONG_XAC_DINH;
                }
            }
            set
            {
                if (_examRegStatus != value)
                {
                    _examRegStatus = value;
                    RaisePropertyChanged("ExamRegStatus");
                }
            }
        }
        private AllLookupValues.ExamRegStatus _examRegStatus;

        public new decimal TotalInvoicePrice
        {
            get; set;
        }

        public new decimal TotalPriceDifference
        {
            get; set;
        }

        public new decimal TotalHIPayment
        {
            get; set;
        }

        public new decimal TotalCoPayment
        {
            get; set;
        }

        public new decimal TotalPatientPayment
        {
            get; set;
        }
        private long? _hisID;
        [DataMemberAttribute()]
        public new long? HisID
        {
            get
            {
                return _hisID;
            }
            set
            {
                _hisID = value;
            }
        }

        private bool _hasNonPriceDetail;
        [DataMemberAttribute()]
        public bool hasNonPriceDetail
        {
            get
            {
                return _hasNonPriceDetail;
            }
            set
            {
                _hasNonPriceDetail = value;
            }
        }

        public void CalTotal()
        {
            hasNonPriceDetail = false;
            TotalInvoicePrice = 0;
            TotalPriceDifference = 0;
            TotalHIPayment = 0;
            TotalCoPayment = 0;
            TotalPatientPayment = 0;

            if (this.PatientPCLRequestIndicators != null && this.PatientPCLRequestIndicators.Count > 0)
            {
                foreach (var item in PatientPCLRequestIndicators)
                {
                    if (!item.MarkedAsDeleted && item.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                    {
                        TotalInvoicePrice += item.TotalInvoicePrice;
                        TotalPriceDifference += item.TotalPriceDifference;
                        TotalHIPayment += item.TotalHIPayment;
                        TotalCoPayment += item.TotalCoPayment;
                        TotalPatientPayment += item.TotalPatientPayment;
                        if (item.TotalInvoicePrice == 0)
                        {
                            hasNonPriceDetail = true;
                        }
                    }
                }
            }
        }
        private DateTime _createdDate = DateTime.Now;
        [DataMemberAttribute]
        public new virtual DateTime CreatedDate
        {
            get
            {
                return _createdDate;
            }
            set
            {
                _createdDate = value;
                RaisePropertyChanged("CreatedDate");
            }
        }


        [DataMemberAttribute]
        public Nullable<Int64> AgencyID
        {
            get { return _AgencyID; }
            set
            {
                if (_AgencyID != value)
                {
                    OnAgencyIDChanging(value);
                    _AgencyID = value;
                    RaisePropertyChanged("AgencyID");
                    OnAgencyIDChanged();
                }
            }
        }
        private Nullable<Int64> _AgencyID;
        partial void OnAgencyIDChanging(Nullable<Int64> value);
        partial void OnAgencyIDChanged();


        [DataMemberAttribute]
        public String PCLRequestNumID
        {
            get { return _PCLRequestNumID; }
            set
            {
                if (_PCLRequestNumID != value)
                {
                    OnPCLRequestNumIDChanging(value);
                    _PCLRequestNumID = value;
                    RaisePropertyChanged("PCLRequestNumID");
                    OnPCLRequestNumIDChanged();
                }
            }
        }
        private String _PCLRequestNumID;
        partial void OnPCLRequestNumIDChanging(String value);
        partial void OnPCLRequestNumIDChanged();
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

        private string _ReceiptNumber;
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

        private long _ReqFromDeptID;
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

        private RefDepartment _RequestedDepartment;
        [DataMemberAttribute]
        public RefDepartment RequestedDepartment
        {
            get
            {
                return _RequestedDepartment;
            }
            set
            {
                if (_RequestedDepartment != value)
                {
                    _RequestedDepartment = value;
                    RaisePropertyChanged("RequestedDepartment");
                }
            }
        }

        private int _RequestCreatedFrom;
        [DataMemberAttribute()]
        public int RequestCreatedFrom
        {
            get
            {
                return _RequestCreatedFrom;
            }
            set
            {
                if (_RequestCreatedFrom != value)
                {
                    _RequestCreatedFrom = value;
                    RaisePropertyChanged("RequestCreatedFrom");
                }
            }
        }

        private int _IsAllowToPayAfter;
        [DataMemberAttribute()]
        public int IsAllowToPayAfter
        {
            get
            {
                return _IsAllowToPayAfter;
            }
            set
            {
                if (_IsAllowToPayAfter != value)
                {
                    _IsAllowToPayAfter = value;
                    RaisePropertyChanged("IsAllowToPayAfter");
                }
            }
        }

        private Patient _Patient;
        [DataMemberAttribute]
        public Patient Patient
        {
            get => _Patient; set
            {
                _Patient = value;
                RaisePropertyChanged("Patient");
            }
        }

        private string _TemplateFileName;
        [DataMemberAttribute]
        public string TemplateFileName
        {
            get => _TemplateFileName; set
            {
                _TemplateFileName = value;
                RaisePropertyChanged("TemplateFileName");
            }
        }

        private Int16 _DefaultNumFilmsRecv;
        [DataMemberAttribute()]
        public Int16 DefaultNumFilmsRecv
        {
            get
            {
                return _DefaultNumFilmsRecv;
            }
            set
            {
                if (_DefaultNumFilmsRecv != value)
                {
                    _DefaultNumFilmsRecv = value;
                    RaisePropertyChanged("DefaultNumFilmsRecv");
                }
            }
        }

        private bool _AllowToPayAfter = false;
        [DataMemberAttribute()]
        public bool AllowToPayAfter
        {
            get
            {
                return _AllowToPayAfter;
            }
            set
            {
                _AllowToPayAfter = value;
                RaisePropertyChanged("AllowToPayAfter");
            }
        }

        public bool IsPaid
        {
            get { return !(!PaidTime.HasValue || PaidTime == null); }
        }

        private long _PatientClassID;
        [DataMemberAttribute()]
        public long PatientClassID
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

        private bool _IsTransferredToRIS;
        [DataMemberAttribute]
        public bool IsTransferredToRIS
        {
            get
            {
                return _IsTransferredToRIS;
            }
            set
            {
                _IsTransferredToRIS = value;
                RaisePropertyChanged("IsTransferredToRIS");
            }
        }
        private bool _IsCancelTransferredToRIS;
        [DataMemberAttribute]
        public bool IsCancelTransferredToRIS
        {
            get
            {
                return _IsCancelTransferredToRIS;
            }
            set
            {
                _IsCancelTransferredToRIS = value;
                RaisePropertyChanged("IsCancelTransferredToRIS");
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

        private long _PerformStaffID;
        [DataMemberAttribute()]
        public long PerformStaffID
        {
            get
            {
                return _PerformStaffID;
            }
            set
            {
                if (_PerformStaffID != value)
                {
                    _PerformStaffID = value;
                    RaisePropertyChanged("PerformStaffID");
                }
            }
        }

        private string _PerformStaffName;
        [DataMemberAttribute()]
        public string PerformStaffName
        {
            get
            {
                return _PerformStaffName;
            }
            set
            {
                if (_PerformStaffName != value)
                {
                    _PerformStaffName = value;
                    RaisePropertyChanged("PerformStaffName");
                }
            }
        }
        private long _V_ReportForm;
        [DataMemberAttribute()]
        public long V_ReportForm
        {
            get
            {
                return _V_ReportForm;
            }
            set
            {
                if (_V_ReportForm != value)
                {
                    _V_ReportForm = value;
                    RaisePropertyChanged("V_ReportForm");
                }
            }
        }

        private bool _IsWaitResult;
        [DataMemberAttribute]
        public bool IsWaitResult
        {
            get { return _IsWaitResult; }
            set
            {
                if (_IsWaitResult != value)
                {
                    _IsWaitResult = value;
                    RaisePropertyChanged("IsWaitResult");
                }
             
            }
        }
        private bool _IsHaveWaitResult;
        [DataMemberAttribute]
        public bool IsHaveWaitResult
        {
            get { return _IsHaveWaitResult; }
            set
            {
                if (_IsHaveWaitResult != value)
                {
                    _IsHaveWaitResult = value;
                    RaisePropertyChanged("IsHaveWaitResult");
                }
             
            }
        }
        private bool _IsDone;
        [DataMemberAttribute]
        public bool IsDone
        {
            get { return _IsDone; }
            set
            {
                if (_IsDone != value)
                {
                    _IsDone = value;
                    RaisePropertyChanged("IsDone");
                }
             
            }
        }
        private long _V_TransactionStatus;
        [DataMemberAttribute]
        public long V_TransactionStatus
        {
            get { return _V_TransactionStatus; }
            set
            {
                if (_V_TransactionStatus != value)
                {
                    _V_TransactionStatus = value;
                    RaisePropertyChanged("V_TransactionStatus");
                }
            }
        }
        private string _TransactionStatus;
        [DataMemberAttribute]
        public string TransactionStatus
        {
            get { return _TransactionStatus; }
            set
            {
                if (_TransactionStatus != value)
                {
                    _TransactionStatus = value;
                    RaisePropertyChanged("TransactionStatus");
                }
            }
        }
        private DateTime _ResponseDate;
        [DataMemberAttribute]
        public DateTime ResponseDate
        {
            get { return _ResponseDate; }
            set
            {
                if (_ResponseDate != value)
                {
                    _ResponseDate = value;
                    RaisePropertyChanged("ResponseDate");
                }
            }
        }

        private long _ResultID;
        [DataMemberAttribute]
        public long ResultID
        {
            get { return _ResultID; }
            set
            {
                if (_ResultID != value)
                {
                    _ResultID = value;
                    RaisePropertyChanged("ResultID");
                }
            }
        }

        private DateTime? _SendDate;
        [DataMemberAttribute]
        public DateTime? SendDate
        {
            get { return _SendDate; }
            set
            {
                if (_SendDate != value)
                {
                    _SendDate = value;
                    RaisePropertyChanged("SendDate");
                }
            }
        }

    }
    [DataContract]
    public class APIPatientPCLRequest
    {
        [DataMemberAttribute]
        public string Diagnosis { get; set; }
        [DataMemberAttribute]
        public string ChargeableItemName { get; set; }
        [DataMemberAttribute]
        public string V_ExamRegStatusName { get; set; }
        [DataMemberAttribute]
        public string V_PCLRequestStatusName { get; set; }
        [DataMemberAttribute]
        public string TemplateFileName { get; set; }
        [DataMemberAttribute]
        public string ResourceCodeArray { get; set; }
        [DataMemberAttribute]
        public byte[] DefaultTemplateFile { get; set; }
        [DataMemberAttribute]
        public string ServerPublicAddress { get; set; }
        [DataMemberAttribute]
        public long V_PCLRequestType { get; set; }
        [DataMemberAttribute]
        public long PatientPCLReqID { get; set; }
        [DataMemberAttribute]
        public long PCLExamTypeID { get; set; }
        [DataMemberAttribute]
        public string RequestedDoctorName { get; set; }
        [DataMemberAttribute]
        public string ReqFromDeptLocIDName { get; set; }
        [DataMemberAttribute]
        public DateTime? MedicalInstructionDate { get; set; }
        [DataMemberAttribute]
        public DateTime? ResultDate { get; set; }
        [DataMemberAttribute]
        public string PCLRequestNumID { get; set; }
        [DataMemberAttribute]
        public string PCLExamTypeName { get; set; }
        [DataMemberAttribute]
        public APIPatientPCLRequestResult PCLRequestResult { get; set; }
        [DataMemberAttribute]
        public APIPatient Patient { get; set; }
    }
    [DataContract]
    public class APIPatientPCLRequestResult
    {
        [DataMemberAttribute]
        public string Suggest { get; set; }
        [DataMemberAttribute]
        public string PtRegistrationCode { get; set; }
        [DataMemberAttribute]
        public string PerformStaffFullName { get; set; }
    }
}