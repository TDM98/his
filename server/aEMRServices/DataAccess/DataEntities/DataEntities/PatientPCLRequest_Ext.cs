using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
     //[DataContract]
     [DataContract(IsReference = true)]
    public partial class PatientPCLRequest_Ext : PatientPCLRequest
    {
         public PatientPCLRequest_Ext()
         {
             //ExamRegStatus = AllLookupValues.ExamRegStatus.DANG_KY_KHAM;
             V_PCLRequestStatus = AllLookupValues.V_PCLRequestStatus.OPEN;
         }
        #region Factory Method


         /// Create a new PatientPCLRequest_Ext object.

        /// <param name="patientPCLReqID">Initial value of the PatientPCLReqID property.</param>
        /// <param name="diagnosis">Initial value of the Diagnosis property.</param>
         public static PatientPCLRequest_Ext CreatePatientPCLRequest_Ext(long patientPCLReqID, String diagnosis)
        {
            PatientPCLRequest_Ext PatientPCLRequest_Ext = new PatientPCLRequest_Ext();
            PatientPCLRequest_Ext.PatientPCLReqID = patientPCLReqID;
            PatientPCLRequest_Ext.Diagnosis = diagnosis;
            return PatientPCLRequest_Ext;
        }

         [DataMemberAttribute()]
         public new long PtRegistrationID
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
         private long _PtRegistrationID;
         partial void OnPtRegistrationIDChanging(long value);
         partial void OnPtRegistrationIDChanged();

        #endregion
        #region Primitive Properties


        [DataMemberAttribute()]
        public new long PatientPCLReqID
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
        public new Nullable<long> PtRegDetailID
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
        public new Nullable<long> ServiceRecID
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
        public new Nullable<long> ReqFromDeptLocID
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
        public new String ReqFromDeptLocIDName
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
        public new Nullable<long> PCLDeptLocID
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
        public new String PCLDeptLocIDName
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
        public new String Diagnosis
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
        private String _Diagnosis="";
        partial void OnDiagnosisChanging(String value);
        partial void OnDiagnosisChanged();

        [DataMemberAttribute()]
        public new String DoctorComments
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
        public new Nullable<Boolean> IsExternalExam
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
        public new Nullable<Boolean> IsImported
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
        public new Nullable<Boolean> IsCaseOfEmergency
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
        public new Int64 V_PCLMainCategory
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
        public new Lookup ObjV_PCLMainCategory
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
        public new Nullable<Int64> PCLResultParamImpID
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
        public new PCLResultParamImplementations ObjPCLResultParamImpID
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
        public new string PCLStaffFullName
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
        public new PatientServiceRecord PatientServiceRecord
        {
            get;
            set;
        }

        private ObservableCollection<PatientPCLRequestDetail_Ext> _PatientPCLRequestIndicatorsExt;
        [DataMemberAttribute()]
        public ObservableCollection<PatientPCLRequestDetail_Ext> PatientPCLRequestIndicatorsExt
        {
            get
            {
                return _PatientPCLRequestIndicatorsExt;
            }
            set
            {
                if (_PatientPCLRequestIndicatorsExt != value)
                {
                    _PatientPCLRequestIndicatorsExt = value;
                    RaisePropertyChanged("PatientPCLRequestIndicatorsExt");
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
        public new bool MarkedAsDeleted
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

         public new long ID { get; set; }

         public new decimal InvoicePrice
         {
             get; set; }

         public new decimal? HIAllowedPrice
         {
             get; set; }
         public new decimal? MaskedHIAllowedPrice
         {
             get
             {
                 return HIAllowedPrice;
             }
         }
         public new decimal PriceDifference
         {
             get; set; }

         public new decimal HIPayment
         {
             get; set; }

         public new decimal PatientCoPayment
         {
             get; set; }

         public new decimal PatientPayment
         {
             get; set; }

         public new decimal Qty
         {
             get; set; }

         public new IChargeableItemPrice ChargeableItem { get; set; }

         public new double? HIBenefit
         {
             get; set; }

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

        

        public new virtual AllLookupValues.ExamRegStatus ExamRegStatus
        {
            get
            {
                //return _examRegStatus;
                //Cho tuong thich code cu:
                if(V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.CANCEL)
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
             get; set; }

         public new decimal TotalCoPayment
         {
             get; set; }

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
         public new bool hasNonPriceDetail
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

         public new void CalTotal()
         {
             hasNonPriceDetail=false;
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
                         if (item.TotalInvoicePrice==0)
                         {
                             hasNonPriceDetail = true;
                         }
                     }
                 }
             }
         }
         private DateTime _createdDate=DateTime.Now;
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
         public new Nullable<Int64> AgencyID
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
         public new String PCLRequestNumID
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
         public new long? InPatientBillingInvID
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

         private long _PatientPCLReqExtID;
         [DataMemberAttribute]
         public virtual long PatientPCLReqExtID
         {
             get
             {
                 return _PatientPCLReqExtID;
             }
             set
             {
                 _PatientPCLReqExtID = value;
                 RaisePropertyChanged("PatientPCLReqExtID");
             }
         }

         private string _HosName;
         [DataMemberAttribute]
         public new virtual string HosName
         {
             get
             {
                 return _HosName;
             }
             set
             {
                 _HosName = value;
                 RaisePropertyChanged("HosName");
             }
         }

         public override string ToString()
         {
             return string.Format("PCL Request Ext {0}", this.PatientPCLReqExtID);
         }
      
    }
}
