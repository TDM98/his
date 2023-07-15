using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations; //validation data
using System.Text.RegularExpressions;
namespace DataEntities
{
    public partial class HospitalizationHistory : EntityBase, IEditableObject
    {
        public HospitalizationHistory()
            : base()
        {

        }

        private HospitalizationHistory _tempHospitalizationHistory;
        #region IEditableObject Members
        public void BeginEdit()
        {
            _tempHospitalizationHistory = (HospitalizationHistory)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempHospitalizationHistory)
                CopyFrom(_tempHospitalizationHistory);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(HospitalizationHistory p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method

        /// Create a new HospitalizationHistory object.

        /// <param name="hHID">Initial value of the HHID property.</param>
        /// <param name="fromDate">Initial value of the FromDate property.</param>
        /// <param name="v_AdmissionType">Initial value of the V_AdmissionType property.</param>
        /// <param name="v_AdmissionReason">Initial value of the V_AdmissionReason property.</param>
        /// <param name="v_ReferralType">Initial value of the V_ReferralType property.</param>
        /// <param name="generalDiagnoses">Initial value of the GeneralDiagnoses property.</param>
        public static HospitalizationHistory CreateHospitalizationHistory(long hHID, DateTime fromDate, Int64 v_AdmissionType, Int64 v_AdmissionReason, Int64 v_ReferralType, String generalDiagnoses)
        {
            HospitalizationHistory hospitalizationHistory = new HospitalizationHistory();
            hospitalizationHistory.HHID = hHID;
            hospitalizationHistory.HDate = fromDate.ToString();
            hospitalizationHistory.FromDate = fromDate;
            hospitalizationHistory.V_AdmissionType = v_AdmissionType;
            hospitalizationHistory.V_AdmissionReason = v_AdmissionReason;
            hospitalizationHistory.V_ReferralType = v_ReferralType;
            hospitalizationHistory.GeneralDiagnoses = generalDiagnoses;
            return hospitalizationHistory;
        }

        #endregion
        #region Primitive Properties
        [DataMemberAttribute()]
        public long HHID
        {
            get
            {
                return _HHID;
            }
            set
            {
                if (_HHID != value)
                {
                    OnHHIDChanging(value);
                    _HHID = value;
                    RaisePropertyChanged("HHID");
                    OnHHIDChanged();
                }
            }
        }
        private long _HHID;
        partial void OnHHIDChanging(long value);
        partial void OnHHIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> CommonMedRecID
        {
            get
            {
                return _CommonMedRecID;
            }
            set
            {
                OnCommonMedRecIDChanging(value);
                _CommonMedRecID = value;
                RaisePropertyChanged("CommonMedRecID");
                OnCommonMedRecIDChanged();
            }
        }
        private Nullable<long> _CommonMedRecID;
        partial void OnCommonMedRecIDChanging(Nullable<long> value);
        partial void OnCommonMedRecIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> IDCode
        {
            get
            {
                return _IDCode;
            }
            set
            {
                OnIDCodeChanging(value);
                _IDCode = value;
                RaisePropertyChanged("IDCode");
                OnIDCodeChanged();
            }
        }
        private Nullable<Int64> _IDCode;
        partial void OnIDCodeChanging(Nullable<Int64> value);
        partial void OnIDCodeChanged();

        //[CustomValidation(typeof(HospitalizationHistory), "validateDay")]
        [DataMemberAttribute()]
        public string HDate
        {
            get
            {
                return _HDate;
            }
            set
            {
                OnHDateChanging(value);
                _HDate=value;
                ValidateProperty("HDate",value);
                RaisePropertyChanged("HDate");
                OnHDateChanged();
            }
        }

        private string _HDate;
        partial void OnHDateChanging(string value);
        partial void OnHDateChanged();

        public static ValidationResult validateDay(string sDate, ValidationContext context)
        {
            bool flag = false;
            if (!string.IsNullOrWhiteSpace(sDate))
            {
                Regex regStr1 = new Regex(@"\d{2}/\d{2}/\d{4}");
                Regex regStr2 = new Regex(@"\d{2}/\d{4}");
                Regex regStr3 = new Regex(@"\d{4}");
                Regex regStr4 = new Regex(@"\d{2}-\d{2}-\d{4}");
                Regex regStr5 = new Regex(@"\d{2}-\d{4}");

                if (regStr1.IsMatch(sDate))
                {
                    flag= true;
                }
                else
                    if (regStr2.IsMatch(sDate))
                    {
                        flag = true;
                    }
                    else
                        if (regStr3.IsMatch(sDate))
                        {
                            flag = true;
                        }
                        else
                            if (regStr4.IsMatch(sDate))
                            {
                                flag = true;
                            }
                            else
                                if (regStr5.IsMatch(sDate))
                                {
                                    flag = true;
                                }
                
            }
            if (flag == false)
            {
                return new ValidationResult("Ngày Không Hợp Lệ!", new string[] { "HDate" });
            }
            return ValidationResult.Success;
        }
        
        [DataMemberAttribute()]
        public Nullable<DateTime> FromDate
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
        //private bool validateDay(string stNew) 
        //{
        //    Regex regStr = new Regex(@"\d{2}/\d{2}/\d{4}");
        //    if (!regStr.IsMatch(stNew))
        //    {
        //        return false;    
        //    }  
        //    return true;
        //}
        private Nullable<DateTime> _FromDate;
        partial void OnFromDateChanging(Nullable<DateTime> value);
        partial void OnFromDateChanged();

        [DataMemberAttribute()]
        public Nullable<long> FromHosID
        {
            get
            {
                return _FromHosID;
            }
            set
            {
                OnFromHosIDChanging(value);
                _FromHosID = value;
                RaisePropertyChanged("FromHosID");
                OnFromHosIDChanged();
            }
        }
        private Nullable<long> _FromHosID;
        partial void OnFromHosIDChanging(Nullable<long> value);
        partial void OnFromHosIDChanged();
        
        //[Required(ErrorMessage = "The Admission Type is required")]
        [DataMemberAttribute()]
        public Nullable<Int64> V_AdmissionType
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
        private Nullable<Int64> _V_AdmissionType;
        partial void OnV_AdmissionTypeChanging(Nullable<Int64> value);
        partial void OnV_AdmissionTypeChanged();

        //[Required(ErrorMessage = "The Admission Reason is required")]
        [DataMemberAttribute()]
        public Nullable<Int64> V_AdmissionReason
        {
            get
            {
                return _V_AdmissionReason;
            }
            set
            {
                OnV_AdmissionReasonChanging(value);
                _V_AdmissionReason = value;
                RaisePropertyChanged("V_AdmissionReason");
                OnV_AdmissionReasonChanged();
            }
        }
        private Nullable<Int64> _V_AdmissionReason;
        partial void OnV_AdmissionReasonChanging(Nullable<Int64> value);
        partial void OnV_AdmissionReasonChanged();

        //[Required(ErrorMessage = "The Referral Type is required")]
        [DataMemberAttribute()]
        public Nullable<Int64> V_ReferralType
        {
            get
            {
                return _V_ReferralType;
            }
            set
            {
                OnV_ReferralTypeChanging(value);
                _V_ReferralType = value;
                RaisePropertyChanged("V_ReferralType");
                OnV_ReferralTypeChanged();
            }
        }
        private Nullable<Int64> _V_ReferralType;
        partial void OnV_ReferralTypeChanging(Nullable<Int64> value);
        partial void OnV_ReferralTypeChanged();

        private String _GeneralDiagnoses;
        //[Required(ErrorMessage = "The General Diagnoses is required")]
        [DataMemberAttribute()]
        public String GeneralDiagnoses
        {
            get
            {
                return _GeneralDiagnoses;
            }
            set
            {
                OnGeneralDiagnosesChanging(value);
                _GeneralDiagnoses = value;
                RaisePropertyChanged("GeneralDiagnoses");
                OnGeneralDiagnosesChanged();
            }
            
        }
        
        partial void OnGeneralDiagnosesChanging(String value);
        partial void OnGeneralDiagnosesChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> ToDate
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
        private Nullable<DateTime> _ToDate;
        partial void OnToDateChanging(Nullable<DateTime> value);
        partial void OnToDateChanged();

        [DataMemberAttribute()]
        public Nullable<long> ToHosID
        {
            get
            {
                return _ToHosID;
            }
            set
            {
                OnToHosIDChanging(value);
                _ToHosID = value;
                RaisePropertyChanged("ToHosID");
                OnToHosIDChanged();
            }
        }
        private Nullable<long> _ToHosID;
        partial void OnToHosIDChanging(Nullable<long> value);
        partial void OnToHosIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> V_TreatmentResult
        {
            get
            {
                return _V_TreatmentResult;
            }
            set
            {
                OnV_TreatmentResultChanging(value);
                _V_TreatmentResult = value;
                RaisePropertyChanged("V_TreatmentResult");
                OnV_TreatmentResultChanged();
            }
        }
        private Nullable<Int64> _V_TreatmentResult;
        partial void OnV_TreatmentResultChanging(Nullable<Int64> value);
        partial void OnV_TreatmentResultChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> V_DischargeReason
        {
            get
            {
                return _V_DischargeReason;
            }
            set
            {
                OnV_DischargeReasonChanging(value);
                _V_DischargeReason = value;
                RaisePropertyChanged("V_DischargeReason");
                OnV_DischargeReasonChanged();
            }
        }
        private Nullable<Int64> _V_DischargeReason;
        partial void OnV_DischargeReasonChanging(Nullable<Int64> value);
        partial void OnV_DischargeReasonChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> V_HospitalType
        {
            get
            {
                return _V_HospitalType;
            }
            set
            {
                OnV_HospitalTypeChanging(value);
                _V_HospitalType = value;
                RaisePropertyChanged("V_HospitalType");
                OnV_HospitalTypeChanged();
            }
        }
        private Nullable<Int64> _V_HospitalType;
        partial void OnV_HospitalTypeChanging(Nullable<Int64> value);
        partial void OnV_HospitalTypeChanged();

        [DataMemberAttribute()]
        public String HHNotes
        {
            get
            {
                return _HHNotes;
            }
            set
            {
                OnHHNotesChanging(value);
                _HHNotes = value;
                RaisePropertyChanged("HHNotes");
                OnHHNotesChanged();
            }
        }
        private String _HHNotes;
        partial void OnHHNotesChanging(String value);
        partial void OnHHNotesChanged();

        [DataMemberAttribute()]
        private bool _isDeleted = true;
        public bool isDeleted
        {
            get
            {
                return _isDeleted;
            }
            set
            {
                if (_isDeleted == value)
                    return;
                _isDeleted = value;
                RaisePropertyChanged("isDeleted");
            }
        }

        [DataMemberAttribute()]
        private bool _isEdit = true;
        public bool isEdit
        {
            get
            {
                return _isEdit;
            }
            set
            {
                if (_isEdit == value)
                    return;
                _isEdit = value;
                RaisePropertyChanged("isEdit");
                if (isEdit == false)
                {
                    isSave = true;
                    isCancel = true;
                }
                else
                {
                    isSave = false;
                    isCancel = false;
                }

            }
        }

        [DataMemberAttribute()]
        private bool _isCancel = false;
        public bool isCancel
        {
            get
            {
                return _isCancel;
            }
            set
            {
                if (_isCancel == value)
                    return;
                _isCancel = value;
                RaisePropertyChanged("isCancel");
            }
        }

        [DataMemberAttribute()]
        private bool _isSave = false;
        public bool isSave
        {
            get
            {
                return _isSave;
            }
            set
            {
                if (_isSave == value)
                    return;
                _isSave = value;
                RaisePropertyChanged("isSave");
            }
        }
        #endregion

        #region Navigation Properties
        private CommonMedicalRecord _CommonMedicalRecord;
        [DataMemberAttribute()]
        public CommonMedicalRecord CommonMedicalRecord
        {
            get
            {
                return _CommonMedicalRecord;
            }
            set
            {
                if (_CommonMedicalRecord != value)
                {
                    _CommonMedicalRecord = value;
                    RaisePropertyChanged("CommonMedicalRecord");
                }
            }
        }

        private DiseasesReference _DiseasesReference;
        [DataMemberAttribute()]
        public DiseasesReference DiseasesReference
        {
            get
            {
                return _DiseasesReference;
            }
            set
            {
                if (_DiseasesReference != value)
                {
                    _DiseasesReference = value;
                    RaisePropertyChanged("DiseasesReference");
                }
            }
        }

        private Hospital _FromHospital;
        [DataMemberAttribute()]
        public Hospital FromHospital
        {
            get
            {
                return _FromHospital;
            }
            set
            {
                if (_FromHospital != value)
                {
                    _FromHospital = value;
                    RaisePropertyChanged("FromHospital");
                }
            }
        }

        private Hospital _ToHospital;
        [DataMemberAttribute()]
        public Hospital ToHospital
        {
            get
            {
                return _ToHospital;
            }
            set
            {
                if (_ToHospital != value)
                {
                    _ToHospital = value;
                    RaisePropertyChanged("ToHospital");
                }
            }
        }

        ObservableCollection<HospitalizationHistoryDetail> _HospitalizationHistoryDetails;
        [DataMemberAttribute()]
        public ObservableCollection<HospitalizationHistoryDetail> HospitalizationHistoryDetails
        {
            get
            {
                return _HospitalizationHistoryDetails;
            }
            set
            {
                if (_HospitalizationHistoryDetails != value)
                {
                    _HospitalizationHistoryDetails = value;
                    RaisePropertyChanged("HospitalizationHistoryDetails");
                }
            }
        }

     
        
     
        private Lookup _LookupAdmissionType;
        //[Required(ErrorMessage = "The Admission Type is required")]
        [DataMemberAttribute()]
        public Lookup LookupAdmissionType
        {
            get
            {
                return _LookupAdmissionType;
            }
            set
            {
                if (_LookupAdmissionType != value)
                {
                    _LookupAdmissionType = value;
                    RaisePropertyChanged("LookupAdmissionType");
                }
            }
        }

     
        
     
        private Lookup _LookupAdmissionReason;
        //[Required(ErrorMessage = "The Admission Reason is required")]
        [DataMemberAttribute()]
        public Lookup LookupAdmissionReason
        {
            get
            {
                return _LookupAdmissionReason;
            }
            set
            {
                if (_LookupAdmissionReason != value)
                {
                    _LookupAdmissionReason = value;
                    RaisePropertyChanged("LookupAdmissionReason");
                }
            }
        }

     
        
     
        private Lookup _LookupReferralType;
        //[Required(ErrorMessage = "The Referral Type is required")]
        [DataMemberAttribute()]
        public Lookup LookupReferralType
        {
            get
            {
                return _LookupReferralType;
            }
            set
            {
                if (_LookupReferralType != value)
                {
                    _LookupReferralType = value;
                    RaisePropertyChanged("LookupReferralType");
                }
            }
        }

     
        
     
        private Lookup _LookupTreatmentResult;
        [DataMemberAttribute()]
        public Lookup LookupTreatmentResult
        {
            get
            {
                return _LookupTreatmentResult;
            }
            set
            {
                if (_LookupTreatmentResult != value)
                {
                    _LookupTreatmentResult = value;
                    RaisePropertyChanged("LookupTreatmentResult");
                }
            }
        }

     
        
     
        private Lookup _LookupDischargeReason;
        [DataMemberAttribute()]
        public Lookup LookupDischargeReason
        {
            get
            {
                return _LookupDischargeReason;
            }
            set
            {
                if (_LookupDischargeReason != value)
                {
                    _LookupDischargeReason = value;
                    RaisePropertyChanged("LookupDischargeReason");
                }
            }
        }

     
        
     
        private Lookup _LookupHospitalType;
        [DataMemberAttribute()]
        public Lookup LookupHospitalType
        {
            get
            {
                return _LookupHospitalType;
            }
            set
            {
                if (_LookupHospitalType != value)
                {
                    _LookupHospitalType = value;
                    RaisePropertyChanged("LookupHospitalType");
                }
            }
        }

        #endregion

    }
}
