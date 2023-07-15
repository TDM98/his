using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using eHCMS.Services.Core;
using System.ComponentModel;
using System.Collections.Generic;
using eHCMSLanguage;
/*
 * 20171214 #001 CMN: Added AgeString for display on form
 */
namespace DataEntities
{
    public partial class Patient : IEditableObject
    {
        private Patient _tempPatient;
        private static long VietNamID = 229;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempPatient = (Patient)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempPatient)
                CopyFrom(_tempPatient);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(Patient p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        private const string GENDER_REQUIRED = "Hãy nhập vào Giới Tính.";
        private const string FULLNAME_REQUIRED = "Hãy nhập vào Họ Tên.";

        partial void OnGenderChanging(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                AddError("Gender", GENDER_REQUIRED, false);
            }
            else
                RemoveError("Gender", GENDER_REQUIRED);
        }

        partial void OnFullNameChanging(string value)
        {
            //if (string.IsNullOrWhiteSpace(value))
            //{
            //    AddError("FullName", FULLNAME_REQUIRED, false);
            //}
            //else
            //    RemoveError("FullName", FULLNAME_REQUIRED);
        }
        [Required(ErrorMessage = "Hãy nhập vào Giới Tính")]
        [DataMemberAttribute()]
        public Gender GenderObj
        {
            get
            {
                return _GenderObj;
            }
            set
            {
                if (_GenderObj != value)
                {
                    ValidateProperty("GenderObj", value);
                    _GenderObj = value;
                    RaisePropertyChanged("GenderObj");
                }
            }
        }
        private Gender _GenderObj;

        private int? _age;
        [Range(0, 120, ErrorMessage = "Ngày Sinh Không Hợp Lệ")]
        [DataMemberAttribute()]
        public int? Age
        {
            get
            {
                return _age;
            }
            set
            {
                if (_age != value)
                {
                    _age = value;
                    //AgeOnly = true;
                    RaisePropertyChanged("Age");
                    RaisePropertyChanged("AgeString");

                    //Reset patient's birthday.
                    //Just assign the new value to private member.
                    //DO NOT set the value of the public property (=> loop)

                    //int yearOfBirth = DateTime.Now.Year;
                    //if (_age.HasValue)
                    //    yearOfBirth -= _age.Value;
                    //_DOB = new DateTime(yearOfBirth, 1, 1);
                    //MonthsOld = 0;
                    //RaisePropertyChanged("DOB");

                    //_MaskedDOB = null;
                    //RaisePropertyChanged("MaskedDOB");
                }
            }
        }

        /*▼====: #001*/
        [DataMemberAttribute()]
        public string AgeString
        {
            get
            {
                if (_AgeString == null)
                    return this.Age.GetValueOrDefault(0).ToString();
                return _AgeString;
            }
            set
            {
                if (_AgeString != value)
                {
                    _AgeString = value;
                    RaisePropertyChanged("AgeString");
                }
            }
        }
        private string _AgeString = null;
        public void GenerateAgeString(DateTime IssuedDate)
        {
            if (DOB.HasValue && IssuedDate != null && IssuedDate > DateTime.MinValue)
            {
                int mMonth = (IssuedDate.Year - DOB.Value.Year) * 12 + IssuedDate.Month - DOB.Value.Month;
                if (mMonth > 72) //20190327 TBL: Duoi 6 tuoi tinh theo thang
                    AgeString = (IssuedDate.Year - DOB.Value.Year).ToString();
                else if (mMonth == 0)
                {
                    int mDay = IssuedDate.Day - DOB.Value.Day;
                    AgeString = string.Format("{0} {1}", mDay, "ngày");
                }
                else AgeString = string.Format("{0} {1}", mMonth, eHCMSResources.Z2633_G1_Thang);
            }
            else
                AgeString = null;
        }
        /*▲====: #001*/

        private long _StaffID;
        [DataMemberAttribute()]
        public long StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                if (_StaffID != value)
                {
                    _StaffID = value;
                    RaisePropertyChanged("StaffID");
                }
            }
        }

        private string _StaffName;
        [DataMemberAttribute()]
        public string StaffName
        {
            get
            {
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

        private DateTime? _MaxExamDateHI;
        [DataMemberAttribute()]
        public DateTime? MaxExamDateHI
        {
            get
            {
                return _MaxExamDateHI;
            }
            set
            {
                if (_MaxExamDateHI != value)
                {
                    _MaxExamDateHI = value;
                    RaisePropertyChanged("MaxExamDateHI");
                }
            }
        }

        private decimal? _MaxDayRptsHI;
        [DataMemberAttribute()]
        public decimal? MaxDayRptsHI
        {
            get
            {
                return _MaxDayRptsHI;
            }
            set
            {
                if (_MaxDayRptsHI != value)
                {
                    _MaxDayRptsHI = value;
                    RaisePropertyChanged("MaxDayRptsHI");
                }
            }
        }

        private int? _yob;
        //[StringLength(4, MinimumLength = 4, ErrorMessage = "Năm Sinh Không Hợp Lệ!")]
        //[Range( 1900, Convert.ToInt32( DateTime.Now.Year.ToString()),ErrorMessage = "Năm Sinh Không Hợp Lệ!")]        
        //[Range(1900, Convert.ToDouble( DateTime.Now.Year ))                  ]
        [DataMemberAttribute()]
        public int? YOB
        {
            get
            {
                return _yob;
            }
            set
            {
                if (_yob != value)
                {
                    _yob = value;
                    RaisePropertyChanged("YOB");
                    if (AgeOnly != null && AgeOnly.Value)
                    {
                        YOBToDOB();
                    }

                }
            }
        }

        private PatientClassification _currentClassification;

        //[CustomValidation(typeof(Patient), "ValidateClassification")]
        //[Required(ErrorMessage="Classification required")]
        [DataMemberAttribute()]
        public PatientClassification CurrentClassification
        {
            get
            {
                return _currentClassification;
            }
            set
            {
                OnCurrentClassificationChanging(value);
                ValidateProperty("CurrentClassification", value);
                _currentClassification = value;
                RaisePropertyChanged("CurrentClassification");
                OnCurrentClassificationChanged();
            }
        }
        partial void OnCurrentClassificationChanging(PatientClassification classification);
        partial void OnCurrentClassificationChanged();

        private bool _classificationHistoryIsLoading;
        public bool ClassificationHistoryIsLoading
        {
            get
            {
                return _classificationHistoryIsLoading;
            }
            set
            {
                _classificationHistoryIsLoading = value;
                RaisePropertyChanged("ClassificationHistoryIsLoading");
            }
        }

        private bool _classificationHistoryLoaded = false;
        public bool ClassificationHistoryLoaded
        {
            get
            {
                return _classificationHistoryLoaded;
            }
            set
            {
                _classificationHistoryLoaded = value;
                RaisePropertyChanged("ClassificationHistoryLoaded");
            }
        }

        private bool _healthInsuranceIsLoading;
        public bool HealthInsuranceIsLoading
        {
            get
            {
                return _healthInsuranceIsLoading;
            }
            set
            {
                _healthInsuranceIsLoading = value;
                RaisePropertyChanged("HealthInsuranceIsLoading");
            }
        }

        private bool _healthInsuranceLoaded = false;
        public bool HealthInsuranceLoaded
        {
            get
            {
                return _healthInsuranceLoaded;
            }
            set
            {
                _healthInsuranceLoaded = value;
                RaisePropertyChanged("HealthInsuranceLoaded");
            }
        }

        private HealthInsurance _currentHealthInsurance;
        [DataMemberAttribute()]
        public HealthInsurance CurrentHealthInsurance
        {
            get
            {
                return _currentHealthInsurance;
            }
            set
            {
                if (_currentHealthInsurance != value)
                {
                    OnCurrentHealthInsuranceChanging(value);
                    _currentHealthInsurance = value;
                    RaisePropertyChanged("CurrentHealthInsurance");
                    OnCurrentHealthInsuranceChanged();

                    HealthInsuranceString = GetHealthInsuranceString();
                    RaisePropertyChanged("HealthInsuranceString");
                    //Xu ly truong hop update the bao hiem

                    if (_tempPatient != null)
                    {
                        _tempPatient.CurrentHealthInsurance = value;
                    }
                }
            }
        }

        partial void OnCurrentHealthInsuranceChanging(HealthInsurance classification);
        partial void OnCurrentHealthInsuranceChanged();

        private PatientRegistration _latestRegistration;
        public PatientRegistration LatestRegistration
        {
            get
            {
                return _latestRegistration;
            }
            set
            {
                _latestRegistration = value;
                RaisePropertyChanged("LatestRegistration");
            }
        }

        private PatientRegistration _latestHIRegistration;
        public PatientRegistration latestHIRegistration
        {
            get
            {
                return _latestHIRegistration;
            }
            set
            {
                _latestHIRegistration = value;
                RaisePropertyChanged("latestHIRegistration");
            }
        }

        private PatientRegistration _latestRegistration_InPt;
        public PatientRegistration LatestRegistration_InPt
        {
            get
            {
                return _latestRegistration_InPt;
            }
            set
            {
                _latestRegistration_InPt = value;
                RaisePropertyChanged("LatestRegistration_InPt");
            }
        }

        private List<PatientAppointment> _appointmentList;
        public List<PatientAppointment> AppointmentList
        {
            get
            {
                return _appointmentList;
            }
            set
            {
                _appointmentList = value;
                RaisePropertyChanged("AppointmentList");
            }
        }

        #region Ext for PMR - NhanLe

        [DataMemberAttribute()]
        public string PatientCodeAndName
        {
            get
            {
                return " ( " + this.PatientCode + " ) " + this.FullName;
            }
            set
            {
            }
        }
        public string GenderString
        {
            get
            {
                if (this.Gender != null)
                {
                    if (this.Gender.CompareTo("F") == 0)
                        //return "Female";
                        return "Nữ";
                    if (this.Gender.CompareTo("M") == 0)
                        return "Nam";
                    else
                        return "";
                }
                else
                    return "";
            }
            set
            {
            }
        }

        public string DemographicString
        {
            get
            {
                StringBuilder st = new StringBuilder();
                st.AppendLine("Đường: " + this.PatientStreetAddress);
                if (this.PatientSurburb != null && this.PatientSurburb.Trim().Length > 0)
                    st.AppendLine("Quận/Huyện: " + this.PatientSurburb);
                if (this.CitiesProvince != null)
                    st.AppendLine("Tỉnh/Thành Phố:" + this.CitiesProvince.CityProvinceName);
                if (this.RefCountry != null)
                    st.AppendLine("Quốc gia:" + this.RefCountry.CountryName);

                if (this.PatientPhoneNumber != null && this.PatientPhoneNumber.Length > 0 && this.PatientCellPhoneNumber != null && this.PatientCellPhoneNumber.Length > 0)
                    st.AppendLine("Điện thoại:" + this.PatientPhoneNumber + " - " + this.PatientCellPhoneNumber);
                else
                    if (this.PatientPhoneNumber != null && this.PatientPhoneNumber.Length > 0)
                    st.AppendLine("Điện thoại:" + this.PatientPhoneNumber);
                else
                        if (this.PatientCellPhoneNumber != null && this.PatientCellPhoneNumber.Length > 0)
                    st.AppendLine("Điện thoại:" + this.PatientCellPhoneNumber);
                else
                    st.AppendLine("Điện thoại:");

                st.AppendLine("Email: " + this.PatientEmailAddress);
                return st.ToString();
            }
            set
            {
            }
        }

        private string _GeneralInfoString;
        [DataMemberAttribute()]
        public string GeneralInfoString
        {
            get
            {
                if (_GeneralInfoString == null || _GeneralInfoString == "")
                {
                    string st = "";
                    st += this.FullName;
                    if (this.GenderString != null)
                        st += " - " + this.GenderString;
                    if (this.DOB != null)
                        //st += " - " + this.DOB.Value.Year.ToString();
                        st += " - " + AgeString + " Tuổi"; //20190102 TBL: BS yeu cau doi thanh tuoi
                    if (this.VBloodType != null && this.VBloodType.BloodTypeName != "")
                        st += " - " + this.VBloodType.BloodTypeName;
                    if (this.PatientCode != null)
                        st += " - " + this.PatientCode;
                    //if (this.NationalMedicalCode != null && this.NationalMedicalCode != "")
                    //    st += " - " + this.NationalMedicalCode;
                    if (this.FileCodeNumber != null && this.FileCodeNumber != "")
                        st += " - " + this.FileCodeNumber;
                    _GeneralInfoString = st;
                }
                return _GeneralInfoString;
            }
            set
            {
                if (_GeneralInfoString != value)
                {
                    _GeneralInfoString = value;
                    RaisePropertyChanged("GeneralInfoString");
                }
            }
        }

        private string _GeneralHeaderInfoInPt;
        [DataMemberAttribute()]
        public string GeneralHeaderInfoInPt
        {
            get
            {
                string st = "";
                if (this.PatientCode != null)
                    st += "(" + this.PatientCode + ") ";
                st += this.FullName;
                if (GenHeaderInfoInpt_DisplayType > 1)
                {
                    if (this.GenderString != null)
                        st += "-" + this.GenderString;
                    if (this.DOB != null)
                        st += "-" + this.DOB.Value.Year.ToString();
                    if (this.FileCodeNumber != null && this.FileCodeNumber != "")
                        st += "-" + this.FileCodeNumber;
                }
                if (InPtAdmissionDate != null)
                    st += " - NV: " + InPtAdmissionDate.Value.ToString();
                if (InPtAdmittedDeptName != null && InPtAdmittedDeptName.Length > 1)
                    st += " - " + InPtAdmittedDeptName;
                if (InPtDischargeDate != null)
                    st += " - XV: " + InPtDischargeDate.Value.ToString("dd/MM/yyyy");
                _GeneralHeaderInfoInPt = st;

                return _GeneralHeaderInfoInPt;
            }
        }

        private int _GenHeaderInfoInpt_DisplayType = 2;

        public int GenHeaderInfoInpt_DisplayType
        {
            get { return _GenHeaderInfoInpt_DisplayType; }
            set
            {
                _GenHeaderInfoInpt_DisplayType = value;
                RaisePropertyChanged("GeneralHeaderInfoInPt");
            }
        }

        private string _InPtAdmittedDeptName = null;
        public string InPtAdmittedDeptName
        {
            get { return _InPtAdmittedDeptName; }
            set
            {
                _InPtAdmittedDeptName = value;
                RaisePropertyChanged("GeneralHeaderInfoInPt");
            }
        }

        private DateTime? _InPtAdmissionDate = null;
        public DateTime? InPtAdmissionDate
        {
            get { return _InPtAdmissionDate; }
            set
            {
                _InPtAdmissionDate = value;
                RaisePropertyChanged("GeneralHeaderInfoInPt");
            }
        }

        private DateTime? _InPtDischargeDate = null;
        public DateTime? InPtDischargeDate
        {
            get { return _InPtDischargeDate; }
            set
            {
                _InPtDischargeDate = value;
                RaisePropertyChanged("GeneralHeaderInfoInPt");
            }
        }

        private string _PCLNum;
        public string PCLNum
        {
            get
            {
                return _PCLNum;
            }
            set
            {
                _PCLNum = value;
                RaisePropertyChanged("PCLNum");
            }
        }


        private string GetHealthInsuranceString()
        {
            string st = string.Empty;
            if (this.CurrentHealthInsurance != null)
            {
                try
                {
                    st += "Số thẻ BHYT: " + this.CurrentHealthInsurance.HICardNo + Environment.NewLine;
                    st += "Mã ĐKBĐ: " + this.CurrentHealthInsurance.RegistrationCode;
                    if (this.CurrentHealthInsurance.RegistrationLocation != null && this.CurrentHealthInsurance.RegistrationLocation.Trim().Length > 0)
                        st += " (" + this.CurrentHealthInsurance.RegistrationLocation + ")";
                    st += Environment.NewLine;
                    DateTime? fromDT = this.CurrentHealthInsurance.ValidDateFrom;
                    DateTime? toDT = this.CurrentHealthInsurance.ValidDateTo;
                    if (fromDT != null && toDT != null)
                    {
                        st += "Giá trị từ: " + String.Format("{0:d/M/yyyy}", fromDT) + " - " + String.Format("{0:d/M/yyyy}", toDT);
                    }
                }
                catch
                {
                    st += "Số thẻ BHYT: " + Environment.NewLine;
                    st += "Mã ĐKBĐ: " + Environment.NewLine;
                    st += "Giá trị từ: ";
                }
            }
            else
            {
                st += "Không có Bảo Hiểm Y Tế";//"Not participate in health insurance/Unknow";
            }

            return st;
        }
        public string HealthInsuranceString
        {
            get
            {
                return GetHealthInsuranceString();
            }
            set
            {
            }
        }

        public override bool Equals(object obj)
        {
            Patient info = obj as Patient;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PatientID > 0 && this.PatientID == info.PatientID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public static bool operator ==(Patient p1, Patient p2)
        {
            return Object.Equals(p1, p2);
        }

        public static bool operator !=(Patient p1, Patient p2)
        {
            return !(p1 == p2);
        }

        //public string PatientClassificationString
        //{
        //    get
        //    {
        //        return this.CurrentClassification.PatientClassName;
        //    }
        //    set
        //    {
        //    }
        //}

        #endregion

        [IgnoreDataMember]
        public Action<Patient> LoadHealthInsurancesDelegate { get; set; }
        [IgnoreDataMember]
        public Action<Patient> LoadClassificationHistoryDelegate { get; set; }

        public static ValidationResult ValidateClassification(PatientClassification classification, ValidationContext context)
        {
            if (classification == null)
            {
                return new ValidationResult("Please select a classification");
            }
            return ValidationResult.Success;
        }

        public void ResetPatientClassHistoryForLazyLoading(Action<Patient> loadingClassHistoryDelegate)
        {
            //Enable classification lazy loading
            this.ClassificationHistoryLoaded = false;
            this.ClassificationHistoryIsLoading = false;
            this.PatientClassHistories = null;

            this.LoadClassificationHistoryDelegate = loadingClassHistoryDelegate;
        }

        public void ResetHealthInsuranceForLazyLoading(Action<Patient> loadingHealthInsurancesDelegate)
        {
            //Enable classification lazy loading
            this.HealthInsuranceLoaded = false;
            this.HealthInsuranceIsLoading = false;
            this.HealthInsurances = null;

            this.LoadHealthInsurancesDelegate = loadingHealthInsurancesDelegate;
        }

        /// Split the FullName property into tree parts: FirstName,Middle Name, LastName

        public void ExtractFullName()
        {
            string[] arr = FullName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length == 0)
                return;
            if (arr.Length == 1)
            {
                this.FirstName = arr[0];
                this.MiddleName = String.Empty;
                this.LastName = String.Empty;
            }
            else if (arr.Length == 2)
            {
                this.FirstName = arr[1];
                this.MiddleName = String.Empty;
                this.LastName = arr[0];
            }
            else
            {
                this.FirstName = arr[arr.Length - 1];
                this.MiddleName = String.Join(" ", arr, 1, arr.Length - 2);
                this.LastName = arr[0];
            }
        }

        public bool ValidatePatient(Patient patient, out ObservableCollection<ValidationResult> results)
        {
            results = new ObservableCollection<ValidationResult>();
            if (patient == null)
            {
                return false;
            }

            var vc = new ValidationContext(patient, null, null);

            bool isValid = Validator.TryValidateObject(patient, vc, results, true);

            if (patient.Age.HasValue)
            {
                if (patient.Age.Value < 0 || patient.Age.Value > 120)
                {
                    var vr = new ValidationResult("Tuổi không hợp lệ.", new[] { "Age" });
                    results.Add(vr);
                    isValid = false;
                }
                // 001 DPT 24/08/2017 Ràng buộc trẻ em dưới 6 tuổi phải có thông tin người thân
                if (patient.AgeOnly == true && (patient.Age.Value <= 6))
                {
                    var vr = new ValidationResult("Trẻ em dưới 6 tuổi phải nhập đầy đủ ngày tháng năm sinh", new[] { "YOB" });
                    results.Add(vr);
                    isValid = false;

                }
                if (patient.Age.Value <= 7)
                {
                    int monthnew;
                    DateTime today = DateTime.Today;
                    DateTime day = Convert.ToDateTime(patient.DOBForBaby);
                    monthnew = (today.Month + today.Year * 12) - (day.Month + day.Year * 12);
                    if (monthnew <= 72)
                    {
                        if (!(patient.V_FamilyRelationship.HasValue) || patient.V_FamilyRelationship == 0 || string.IsNullOrWhiteSpace(patient.FContactFullName) || string.IsNullOrWhiteSpace(patient.FContactCellPhone))
                        {
                            var vr = new ValidationResult("Khách hàng là trẻ em dưới 72 tháng tuổi. Vui lòng nhập đủ thông tin"+Environment.NewLine
                                                        +"các trường [Đthoại Di Động], [Gia đình], [Họ tên] tại Tab liên hệ phụ", new[] { "FContactFullName" });
                            results.Add(vr);
                            isValid = false;
                        }
                    }
                }
                //001
            }

            if (patient.DOBForBaby.HasValue)
            {
                if (patient.DOBForBaby.Value > DateTime.Now.Date)
                {
                    var vr = new ValidationResult("Ngày Sinh không hợp lệ.", new[] { "Age" });
                    results.Add(vr);
                    isValid = false;
                }
            }

            if (patient.CountryID > 0 && patient.CountryID == (VietNamID as long?))
            {
                if (patient.CityProvinceID == null || patient.CityProvinceID < 1)
                {
                    var vr = new ValidationResult("Hãy nhập vào tỉnh thành!", new[] { "CityProvince" });
                    results.Add(vr);
                    isValid = false;
                }
                if (patient.SuburbNameID < 1 && string.IsNullOrEmpty(PatientNotes))
                {
                    var vr = new ValidationResult("Hãy nhập vào quận huyện hoặc thông tin trong ô 'Khác'!", new[] { "SurburbName" });
                    results.Add(vr);
                    isValid = false;
                }
            }

            //if (string.IsNullOrWhiteSpace(patient.PatientCellPhoneNumber) &&
            //    string.IsNullOrWhiteSpace(patient.PatientPhoneNumber))
            //{
            //    var vr = new ValidationResult("Hãy nhập số ĐT di động hoặc ĐT bàn", new[] { "PatientPhoneNumber" });
            //    results.Add(vr);
            //    isValid = false;
            //}
            else
            {
                if ((!string.IsNullOrWhiteSpace(patient.PatientCellPhoneNumber) && patient.PatientCellPhoneNumber.Length < 6))
                {
                    var vr = new ValidationResult("Số Điện Thoại Phải >=6 Ký Tự", new[] { "PatientPhoneNumber" });
                    results.Add(vr);
                    isValid = false;
                }
            }

            return isValid;
        }
    }


    //public static class PatientValidator
    //{
    //    public static ValidationResult ValidateClassification(PatientClassification classification, ValidationContext context)
    //    {
    //        if (classification == null)
    //        {
    //            return new ValidationResult("Please select a classification");
    //        }
    //        return ValidationResult.Success;
    //    }
    //}
}
