using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
using System.Linq;
/*
 * 20170113 #001 CMN: Add QRCode
 * 20181113 #002 TTM: BM 0005228: Bổ sung thêm phường xã.
 * 20181122 #003 TTM: Thêm mới thuộc tính DOBNumIndex để phục vụ việc tìm kiếm bệnh nhân bằng tên đi kèm DOB
 * 20210618 #004 TNHX: Thêm mới thuộc tính SocialInsuranceNumber để lưu số bảo hiểm xã hội
 * 20210704 #005 TNHX: 385 Thêm mới thuộc tính DateCreatedQMSTicket
 * 20230316 #006 QTD:  Dữ liệu 130
 */
namespace DataEntities
{
    public partial class Patient : NotifyChangedBase
    {
        public Patient()
        {
            DateBecamePatient = DateTime.Now;
        }
        public Patient(GenericPayment aGenericPayment)
        {
            if (aGenericPayment == null)
            {
                return;
            }
            PatientCode = !string.IsNullOrEmpty(aGenericPayment.PatientCode) ? aGenericPayment.PatientCode : string.Format("GP{0}", aGenericPayment.GenericPaymentCode);
            FullName = aGenericPayment.PersonName;
            PatientFullStreetAddress = aGenericPayment.PersonAddress;
            PatientPhoneNumber = aGenericPayment.PhoneNumber;
        }
        #region Factory Method


        /// Create a new Patient object.

        /// <param name="patientID">Initial value of the PatientID property.</param>
        /// <param name="patientCode">Initial value of the PatientCode property.</param>
        /// <param name="iDNumber">Initial value of the IDNumber property.</param>
        /// <param name="firstName">Initial value of the FirstName property.</param>
        /// <param name="lastName">Initial value of the LastName property.</param>
        /// <param name="fullName">Initial value of the FullName property.</param>
        /// <param name="gender">Initial value of the Gender property.</param>
        public static Patient CreatePatient(long patientID, String patientCode, String iDNumber, String firstName, String lastName, String fullName, String gender)
        {
            Patient patient = new Patient();
            patient.PatientID = patientID;
            patient.PatientCode = patientCode;
            patient.IDNumber = iDNumber;
            patient.FirstName = firstName;
            patient.LastName = lastName;
            patient.FullName = fullName;
            patient.Gender = gender;
            patient.DateBecamePatient = DateTime.Now;
            return patient;
        }

        #endregion
        #region Primitive Properties


        [DataMemberAttribute()]
        public long PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                if (_PatientID != value)
                {
                    OnPatientIDChanging(value);
                    _PatientID = value;
                    RaisePropertyChanged("PatientID");
                    OnPatientIDChanged();
                }
            }
        }
        private long _PatientID;
        partial void OnPatientIDChanging(long value);
        partial void OnPatientIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> CountryID
        {
            get
            {
                return _CountryID;
            }
            set
            {
                if (_CountryID != value)
                {
                    OnCountryIDChanging(value);
                    _CountryID = value;
                    RaisePropertyChanged("CountryID");
                    OnCountryIDChanged();
                }
            }
        }
        private Nullable<long> _CountryID;
        partial void OnCountryIDChanging(Nullable<long> value);
        partial void OnCountryIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> NationalityID
        {
            get
            {
                return _NationalityID;
            }
            set
            {
                if (_NationalityID != value)
                {
                    OnNationalityIDChanging(value);
                    _NationalityID = value;
                    RaisePropertyChanged("NationalityID");
                    OnNationalityIDChanged();
                }
            }
        }
        private Nullable<long> _NationalityID;
        partial void OnNationalityIDChanging(Nullable<long> value);
        partial void OnNationalityIDChanged();


        //[Required( ErrorMessage="Hãy nhập vào Tỉnh/Thành")]
        [DataMemberAttribute()]
        public Nullable<long> CityProvinceID
        {
            get
            {
                return _CityProvinceID;
            }
            set
            {
                if (_CityProvinceID != value)
                {
                    OnCityProvinceIDChanging(value);
                    ValidateProperty("CityProvinceID", value);
                    _CityProvinceID = value;
                    RaisePropertyChanged("CityProvinceID");
                    OnCityProvinceIDChanged();
                }
            }
        }
        private Nullable<long> _CityProvinceID;
        partial void OnCityProvinceIDChanging(Nullable<long> value);
        partial void OnCityProvinceIDChanged();


        [DataMemberAttribute()]
        public long SuburbNameID
        {
            get
            {
                return _SuburbNameID;
            }
            set
            {
                if (_SuburbNameID != value)
                {
                    OnSuburbNameIDChanging(value);
                    ValidateProperty("SuburbNameID", value);
                    _SuburbNameID = value;
                    RaisePropertyChanged("SuburbNameID");
                    OnSuburbNameIDChanged();
                }
            }
        }
        private long _SuburbNameID;
        partial void OnSuburbNameIDChanging(long value);
        partial void OnSuburbNameIDChanged();

        [DataMemberAttribute()]
        public String PatientCode
        {
            get
            {
                return _PatientCode;
            }
            set
            {
                if (_PatientCode != value)
                {
                    OnPatientCodeChanging(value);
                    _PatientCode = value;
                    RaisePropertyChanged("PatientCode");
                    OnPatientCodeChanged();
                }
            }
        }
        private String _PatientCode;
        partial void OnPatientCodeChanging(String value);
        partial void OnPatientCodeChanged();





        [DataMemberAttribute()]
        public String PatientBarcode
        {
            get
            {
                return _PatientBarcode;
            }
            set
            {
                if (_PatientBarcode != value)
                {
                    OnPatientBarcodeChanging(value);
                    ////ReportPropertyChanging("PatientBarcode");
                    _PatientBarcode = value;
                    RaisePropertyChanged("PatientBarcode");
                    OnPatientBarcodeChanged();
                }
            }
        }
        private String _PatientBarcode;
        partial void OnPatientBarcodeChanging(String value);
        partial void OnPatientBarcodeChanged();


        [DataMemberAttribute()]
        public String IDNumber
        {
            get
            {
                return _IDNumber;
            }
            set
            {
                if (_IDNumber != value)
                {
                    OnIDNumberChanging(value);
                    ValidateProperty("IDNumber", value);
                    _IDNumber = value;
                    RaisePropertyChanged("IDNumber");
                    OnIDNumberChanged();
                }
            }
        }
        private String _IDNumber;
        partial void OnIDNumberChanging(String value);
        partial void OnIDNumberChanged();


        [DataMemberAttribute()]
        public int? BloodTypeID
        {
            get
            {
                return _BloodTypeID;
            }
            set
            {
                if (_BloodTypeID != value)
                {
                    OnBloodTypeIDChanging(value);
                    ValidateProperty("BloodTypeID", value);
                    _BloodTypeID = value;
                    RaisePropertyChanged("BloodTypeID");
                    OnBloodTypeIDChanged();
                }
            }
        }
        private int? _BloodTypeID;
        partial void OnBloodTypeIDChanging(int? value);
        partial void OnBloodTypeIDChanged();



        [DataMemberAttribute()]
        public String FirstName
        {
            get
            {
                return _FirstName;
            }
            set
            {
                if (_FirstName != value)
                {
                    OnFirstNameChanging(value);
                    ////ReportPropertyChanging("FirstName");
                    _FirstName = value;
                    RaisePropertyChanged("FirstName");
                    OnFirstNameChanged();
                }
            }
        }
        private String _FirstName = "";
        partial void OnFirstNameChanging(String value);
        partial void OnFirstNameChanged();





        [DataMemberAttribute()]
        public String MiddleName
        {
            get
            {
                return _MiddleName;
            }
            set
            {
                if (_MiddleName != value)
                {
                    OnMiddleNameChanging(value);
                    ////ReportPropertyChanging("MiddleName");
                    _MiddleName = value;
                    RaisePropertyChanged("MiddleName");
                    OnMiddleNameChanged();
                }
            }
        }
        private String _MiddleName = "";
        partial void OnMiddleNameChanging(String value);
        partial void OnMiddleNameChanged();

        [DataMemberAttribute()]
        public String LastName
        {
            get
            {
                return _LastName;
            }
            set
            {
                if (_LastName != value)
                {
                    OnLastNameChanging(value);
                    ////ReportPropertyChanging("LastName");
                    _LastName = value;
                    RaisePropertyChanged("LastName");
                    OnLastNameChanged();
                }
            }
        }
        private String _LastName = "";
        partial void OnLastNameChanging(String value);
        partial void OnLastNameChanged();

        [DataMemberAttribute()]
        [Required(ErrorMessage = "Hãy nhập vào Họ Tên")]
        public String FullName
        {
            get
            {
                return _FullName;
            }
            set
            {
                if (_FullName != value)
                {
                    OnFullNameChanging(value);
                    ValidateProperty("FullName", value);
                    _FullName = value;
                    RaisePropertyChanged("FullName");
                    OnFullNameChanged();
                }
            }
        }
        private String _FullName;
        partial void OnFullNameChanging(String value);
        partial void OnFullNameChanged();

        [DataMemberAttribute()]
        public String OldPtNamesLog
        {
            get
            {
                return _OldPtNamesLog;
            }
            set
            {
                if (_OldPtNamesLog != value)
                {
                    OnOldPtNamesLogChanging(value);
                    ValidateProperty("OldPtNamesLog", value);
                    _OldPtNamesLog = value;
                    RaisePropertyChanged("OldPtNamesLog");
                    OnOldPtNamesLogChanged();
                }
            }
        }
        private String _OldPtNamesLog;
        partial void OnOldPtNamesLogChanging(String value);
        partial void OnOldPtNamesLogChanged();


        [DataMemberAttribute()]
        public String Gender
        {
            get
            {
                return _Gender;
            }
            set
            {
                if (_Gender != value)
                {
                    OnGenderChanging(value);
                    ////ReportPropertyChanging("Gender");
                    _Gender = value;
                    RaisePropertyChanged("Gender");
                    OnGenderChanged();
                }
            }
        }
        private String _Gender;
        partial void OnGenderChanging(String value);
        partial void OnGenderChanged();



        private DateTime? _MaskedDOB;
        [DataMemberAttribute()]
        public DateTime? MaskedDOB
        {
            get
            {
                return _MaskedDOB;
            }
            set
            {
                if (_MaskedDOB != value)
                {
                    _MaskedDOB = value;
                    RaisePropertyChanged("MaskedDOB");

                    //DOB = value; 
                }
            }
        }
        private int _monthsOld;
        [DataMemberAttribute()]
        public int MonthsOld
        {
            get
            {
                return _monthsOld;
            }
            set
            {
                _monthsOld = value;
                RaisePropertyChanged("MonthsOld");
            }
        }

        [Required(ErrorMessage = "Hãy nhập vào Năm Sinh")]
        [DataMemberAttribute()]
        public Nullable<DateTime> DOB
        {
            get
            {
                return _DOB;
            }
            set
            {
                //if (_DOB != value)
                {
                    _DOB = value;
                    RaisePropertyChanged("DOB");
                    //if (_DOB != null)
                    //{
                    //    AgeOnly = false;
                    //}
                    //_MaskedDOB = value;
                    //RaisePropertyChanged("MaskedDOB");

                    //if (_DOB != null)
                    //{
                    //    int yearsOld, monthsOld;
                    //    AxHelper.ConvertAge(_DOB.Value,  out yearsOld,out monthsOld);
                    //    _age = yearsOld;
                    //    MonthsOld = monthsOld;
                    //    //int diff = DateTime.Now.Year - _DOB.Value.Year;
                    //    //if (diff >= 0)
                    //    //{
                    //    //    _age = diff;
                    //    //}
                    //    //else
                    //    //{
                    //    //    _age = 0;
                    //    //}
                    //    RaisePropertyChanged("Age");
                    //}
                }
            }
        }
        private Nullable<DateTime> _DOB;

        //[Range(DateTime.Now.AddYears(-100), DateTime.Now.Date, ErrorMessage = "Ngày Sinh Không Hợp Lệ!")]
        [DataMemberAttribute()]
        public Nullable<DateTime> DOBForBaby
        {
            get
            {
                return _DOBForBaby;
            }
            set
            {
                //if (_DOBForBaby != value)
                {
                    _DOBForBaby = value;
                    RaisePropertyChanged("DOBForBaby");
                }
            }
        }
        private Nullable<DateTime> _DOBForBaby;

        public void CalDOB()
        {
            if (_AgeOnly.GetValueOrDefault(false))
            {
                int yearOfBirth = DateTime.Now.Year;
                if (_age.HasValue)
                    yearOfBirth -= _age.Value;
                _DOB = new DateTime(yearOfBirth, 1, 1);
                RaisePropertyChanged("DOB");

                _MaskedDOB = null;
                RaisePropertyChanged("MaskedDOB");
            }
            //else
            //{
            //    if (_DOB != null)
            //    {
            //        int diff = DateTime.Now.Year - _DOB.Value.Year;
            //        if (diff >= 0)
            //        {
            //            _age = diff;
            //        }
            //        else
            //        {
            //            _age = 0;
            //        }
            //        RaisePropertyChanged("Age");
            //    }
            //}
        }

        public void NormalizeDOB()
        {
            if (!_AgeOnly.GetValueOrDefault(true))
            {
                if (_DOB != null)
                {
                    int yearsOld, monthsOld;
                    AxHelper.ConvertAge(_DOB.Value, out yearsOld, out monthsOld);
                    Age = yearsOld;
                    //YOB = DateTime.Now.Year - Age;
                    if (DOB.HasValue)
                    {
                        YOB = DOB.Value.Year;
                    }
                    else
                    {
                        YOB = DateTime.Now.Year - Age;
                    }
                    MonthsOld = monthsOld;
                }
            }
            else
            {
                if (_DOB.HasValue)
                {
                    int diff = DateTime.Now.Year - _DOB.Value.Year;
                    if (diff >= 0)
                    {
                        Age = diff;
                    }
                    else
                    {
                        Age = 0;
                    }
                    YOB = DateTime.Now.Year - Age;
                    MonthsOld = 0;
                    //_DOB = null;
                }
            }
        }

        public void YOBToDOB()
        {
            //if (!_yob.GetValueOrDefault(true))
            {
                if (YOB != null && YOB > 0)
                {
                    DOB = new DateTime((int)YOB, 1, 1);
                    int diff = DateTime.Now.Year - DOB.Value.Year;
                    //if (diff >= 0)
                    if (diff > 0)
                    {
                        Age = diff;
                    }
                    else
                    {
                        //Age = 0;
                        Age = 1;
                    }
                }
            }
        }

        [DataMemberAttribute()]
        public Nullable<Boolean> AgeOnly
        {
            get
            {
                return _AgeOnly;
            }
            set
            {
                if (_AgeOnly != value)
                {
                    OnAgeOnlyChanging(value);
                    _AgeOnly = value;
                    RaisePropertyChanged("AgeOnly");
                    OnAgeOnlyChanged();
                }
            }
        }
        private Nullable<Boolean> _AgeOnly;
        partial void OnAgeOnlyChanging(Nullable<Boolean> value);
        partial void OnAgeOnlyChanged();


        [DataMemberAttribute()]
        public String DOBText
        {
            get
            {
                return _DOBText;
            }
            set
            {
                if (_DOBText != value)
                {
                    OnDOBTextChanging(value);
                    ////ReportPropertyChanging("DOBText");
                    _DOBText = value;
                    RaisePropertyChanged("DOBText");
                    OnDOBTextChanged();
                }
            }
        }
        private String _DOBText;
        partial void OnDOBTextChanging(String value);
        partial void OnDOBTextChanged();



        [DataMemberAttribute()]
        public Nullable<DateTime> DateBecamePatient
        {
            get
            {
                return _DateBecamePatient;
            }
            set
            {
                if (_DateBecamePatient != value)
                {
                    OnDateBecamePatientChanging(value);
                    ////ReportPropertyChanging("DateBecamePatient");
                    _DateBecamePatient = value;
                    RaisePropertyChanged("DateBecamePatient");
                    OnDateBecamePatientChanged();
                }
            }
        }
        private Nullable<DateTime> _DateBecamePatient;
        partial void OnDateBecamePatientChanging(Nullable<DateTime> value);
        partial void OnDateBecamePatientChanged();





        [DataMemberAttribute()]
        public Nullable<Int64> V_MaritalStatus
        {
            get
            {
                return _V_MaritalStatus;
            }
            set
            {
                if (V_MaritalStatus != value)
                {
                    OnV_MaritalStatusChanging(value);
                    ////ReportPropertyChanging("V_MaritalStatus");
                    _V_MaritalStatus = value;
                    RaisePropertyChanged("V_MaritalStatus");
                    OnV_MaritalStatusChanged();
                }
            }
        }
        private Nullable<Int64> _V_MaritalStatus;
        partial void OnV_MaritalStatusChanging(Nullable<Int64> value);
        partial void OnV_MaritalStatusChanged();


        [DataMemberAttribute()]
        public String PatientPhoto
        {
            get
            {
                return _PatientPhoto;
            }
            set
            {
                if (_PatientPhoto != value)
                {
                    OnPatientPhotoChanging(value);
                    ////ReportPropertyChanging("PatientPhoto");
                    _PatientPhoto = value;
                    RaisePropertyChanged("PatientPhoto");
                    OnPatientPhotoChanged();
                }
            }
        }
        private String _PatientPhoto;
        partial void OnPatientPhotoChanging(String value);
        partial void OnPatientPhotoChanged();





        [DataMemberAttribute()]
        public String PatientNotes
        {
            get
            {
                return _PatientNotes;
            }
            set
            {
                if (_PatientNotes != value)
                {
                    OnPatientNotesChanging(value);
                    ////ReportPropertyChanging("PatientNotes");
                    _PatientNotes = value;
                    RaisePropertyChanged("PatientNotes");
                    OnPatientNotesChanged();
                }
            }
        }
        private String _PatientNotes;
        partial void OnPatientNotesChanging(String value);
        partial void OnPatientNotesChanged();

        [Required(ErrorMessage = "Hãy nhập vào Địa Chỉ")]
        [DataMemberAttribute()]
        public String PatientStreetAddress
        {
            get
            {
                return _PatientStreetAddress;
            }
            set
            {
                if (_PatientStreetAddress != value)
                {
                    OnPatientStreetAddressChanging(value);
                    ValidateProperty("PatientStreetAddress", value);
                    _PatientStreetAddress = value;
                    RaisePropertyChanged("PatientStreetAddress");
                    OnPatientStreetAddressChanged();
                }
            }
        }
        private String _PatientStreetAddress;
        partial void OnPatientStreetAddressChanging(String value);
        partial void OnPatientStreetAddressChanged();

        public void AddressConvert()
        {
            object[] address = new string[] { PatientStreetAddress, PatientSurburb, CitiesProvince == null ? "" : CitiesProvince.CityProvinceName };
            PatientStreetAddress = String.Join(",", address.Where(o => o != null && string.IsNullOrWhiteSpace(o.ToString())));
        }

        [DataMemberAttribute()]
        public String PatientSurburb
        {
            get
            {
                return _PatientSurburb;
            }
            set
            {
                if (_PatientSurburb != value)
                {
                    OnPatientSurburbChanging(value);
                    ////ReportPropertyChanging("PatientSurburb");
                    _PatientSurburb = value;
                    RaisePropertyChanged("PatientSurburb");
                    OnPatientSurburbChanged();
                }
            }
        }
        private String _PatientSurburb;
        partial void OnPatientSurburbChanging(String value);
        partial void OnPatientSurburbChanged();

        [DataType(DataType.PhoneNumber)]
        [DataMemberAttribute()]
        public String PatientPhoneNumber
        {
            get
            {
                return _PatientPhoneNumber;
            }
            set
            {
                if (_PatientPhoneNumber != value)
                {
                    OnPatientPhoneNumberChanging(value);
                    ////ReportPropertyChanging("PatientPhoneNumber");
                    _PatientPhoneNumber = value;
                    RaisePropertyChanged("PatientPhoneNumber");
                    OnPatientPhoneNumberChanged();
                }
            }
        }
        private String _PatientPhoneNumber;
        partial void OnPatientPhoneNumberChanging(String value);
        partial void OnPatientPhoneNumberChanged();

        //[StringLength(20,MinimumLength=6,ErrorMessage="Số Điện Thoại Phải >=6 Ký Tự!")]
        [DataMemberAttribute()]
        public String PatientCellPhoneNumber
        {
            get
            {
                return _PatientCellPhoneNumber;
            }
            set
            {
                if (_PatientCellPhoneNumber != value)
                {
                    OnPatientCellPhoneNumberChanging(value);
                    ////ReportPropertyChanging("PatientCellPhoneNumber");
                    _PatientCellPhoneNumber = value;
                    RaisePropertyChanged("PatientCellPhoneNumber");
                    OnPatientCellPhoneNumberChanged();
                }
            }
        }
        private String _PatientCellPhoneNumber;
        partial void OnPatientCellPhoneNumberChanging(String value);
        partial void OnPatientCellPhoneNumberChanged();

        [RegularExpression(AxHelper.EmailRegularExpression, ErrorMessage = "Địa chỉ Email không hợp lệ")]
        //[CustomValidation(typeof(Patient), "ValidatePatientEmailAddress")]
        [DataMemberAttribute()]
        public String PatientEmailAddress
        {
            get
            {
                return _PatientEmailAddress;
            }
            set
            {
                if (_PatientEmailAddress != value)
                {
                    OnPatientEmailAddressChanging(value);
                    ValidateProperty("PatientEmailAddress", value);
                    _PatientEmailAddress = value;
                    RaisePropertyChanged("PatientEmailAddress");
                    OnPatientEmailAddressChanged();
                }
            }
        }
        private String _PatientEmailAddress;
        partial void OnPatientEmailAddressChanging(String value);
        partial void OnPatientEmailAddressChanged();





        [DataMemberAttribute()]
        public String PatientEmployer
        {
            get
            {
                return _PatientEmployer;
            }
            set
            {
                if (_PatientEmployer != value)
                {
                    OnPatientEmployerChanging(value);
                    _PatientEmployer = value;
                    RaisePropertyChanged("PatientEmployer");
                    OnPatientEmployerChanged();
                }
            }
        }
        private String _PatientEmployer;
        partial void OnPatientEmployerChanging(String value);
        partial void OnPatientEmployerChanged();





        [DataMemberAttribute()]
        public String PatientOccupation
        {
            get
            {
                return _PatientOccupation;
            }
            set
            {
                if (_PatientOccupation != value)
                {
                    OnPatientOccupationChanging(value);
                    _PatientOccupation = value;
                    RaisePropertyChanged("PatientOccupation");
                    OnPatientOccupationChanged();
                }
            }
        }
        private String _PatientOccupation;
        partial void OnPatientOccupationChanging(String value);
        partial void OnPatientOccupationChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> V_Ethnic
        {
            get
            {
                return _V_Ethnic;
            }
            set
            {
                if (_V_Ethnic != value)
                {
                    OnV_EthnicChanging(value);
                    _V_Ethnic = value;
                    RaisePropertyChanged("V_Ethnic");
                    OnV_EthnicChanged();
                }
            }
        }
        private Nullable<Int64> _V_Ethnic;
        partial void OnV_EthnicChanging(Nullable<Int64> value);
        partial void OnV_EthnicChanged();

        [DataMemberAttribute()]
        private string _EthnicName;
        public string EthnicName
        {
            get
            {
                return _EthnicName;
            }
            set
            {
                _EthnicName = value;
                RaisePropertyChanged("EthnicName");
            }
        }

        [DataMemberAttribute()]
        private string _EthnicCode;
        public string EthnicCode
        {
            get
            {
                return _EthnicCode;
            }
            set
            {
                _EthnicCode = value;
                RaisePropertyChanged("EthnicCode");
            }
        }

        [DataMemberAttribute()]
        public String FContactFullName
        {
            get
            {
                return _FContactFullName;
            }
            set
            {
                if (_FContactFullName != value)
                {
                    OnFContactFullNameChanging(value);
                    ////ReportPropertyChanging("FContactFullName");
                    _FContactFullName = value;
                    RaisePropertyChanged("FContactFullName");
                    OnFContactFullNameChanged();
                }
            }
        }
        private String _FContactFullName;
        partial void OnFContactFullNameChanging(String value);
        partial void OnFContactFullNameChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> V_FamilyRelationship
        {
            get
            {
                return _V_FamilyRelationship;
            }
            set
            {
                if (_V_FamilyRelationship != value)
                {
                    OnV_FamilyRelationshipChanging(value);
                    ////ReportPropertyChanging("V_FamilyRelationship");
                    _V_FamilyRelationship = value;
                    RaisePropertyChanged("V_FamilyRelationship");
                    OnV_FamilyRelationshipChanged();
                }
            }
        }
        private Nullable<Int64> _V_FamilyRelationship;
        partial void OnV_FamilyRelationshipChanging(Nullable<Int64> value);
        partial void OnV_FamilyRelationshipChanged();

        [DataMemberAttribute()]
        public String FContactAddress
        {
            get
            {
                return _FContactAddress;
            }
            set
            {
                if (_FContactAddress != value)
                {
                    OnFContactAddressChanging(value);
                    ////ReportPropertyChanging("FContactAddress");
                    _FContactAddress = value;
                    RaisePropertyChanged("FContactAddress");
                    OnFContactAddressChanged();
                }
            }
        }
        private String _FContactAddress = "";
        partial void OnFContactAddressChanging(String value);
        partial void OnFContactAddressChanged();

        [DataMemberAttribute()]
        public String FContactHomePhone
        {
            get
            {
                return _FContactHomePhone;
            }
            set
            {
                if (_FContactHomePhone != value)
                {
                    OnFContactHomePhoneChanging(value);
                    ////ReportPropertyChanging("FContactHomePhone");
                    _FContactHomePhone = value;
                    RaisePropertyChanged("FContactHomePhone");
                    OnFContactHomePhoneChanged();
                }
            }
        }
        private String _FContactHomePhone;
        partial void OnFContactHomePhoneChanging(String value);
        partial void OnFContactHomePhoneChanged();

        [DataMemberAttribute()]
        public String FContactBusinessPhone
        {
            get
            {
                return _FContactBusinessPhone;
            }
            set
            {
                if (_FContactBusinessPhone != value)
                {
                    OnFContactBusinessPhoneChanging(value);
                    ////ReportPropertyChanging("FContactBusinessPhone");
                    _FContactBusinessPhone = value;
                    RaisePropertyChanged("FContactBusinessPhone");
                    OnFContactBusinessPhoneChanged();
                }
            }
        }
        private String _FContactBusinessPhone;
        partial void OnFContactBusinessPhoneChanging(String value);
        partial void OnFContactBusinessPhoneChanged();

        [DataMemberAttribute()]
        public String FContactCellPhone
        {
            get
            {
                return _FContactCellPhone;
            }
            set
            {
                if (_FContactCellPhone != value)
                {
                    OnFContactCellPhoneChanging(value);
                    ////ReportPropertyChanging("FContactCellPhone");
                    _FContactCellPhone = value;
                    RaisePropertyChanged("FContactCellPhone");
                    OnFContactCellPhoneChanged();
                }
            }
        }
        private String _FContactCellPhone;
        partial void OnFContactCellPhoneChanging(String value);
        partial void OnFContactCellPhoneChanged();

        [DataMemberAttribute()]
        public String FAlternateContact
        {
            get
            {
                return _FAlternateContact;
            }
            set
            {
                if (_FAlternateContact != value)
                {
                    OnFAlternateContactChanging(value);
                    _FAlternateContact = value;
                    RaisePropertyChanged("FAlternateContact");
                    OnFAlternateContactChanged();
                }
            }
        }
        private String _FAlternateContact;
        partial void OnFAlternateContactChanging(String value);
        partial void OnFAlternateContactChanged();

        [DataMemberAttribute()]
        public String FAlternatePhone
        {
            get
            {
                return _FAlternatePhone;
            }
            set
            {
                if (_FAlternatePhone != value)
                {
                    OnFAlternatePhoneChanging(value);
                    ////ReportPropertyChanging("FAlternatePhone");
                    _FAlternatePhone = value;
                    RaisePropertyChanged("FAlternatePhone");
                    OnFAlternatePhoneChanged();
                }
            }
        }
        private String _FAlternatePhone;
        partial void OnFAlternatePhoneChanging(String value);
        partial void OnFAlternatePhoneChanged();

        [DataMemberAttribute()]
        public Nullable<Boolean> IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                OnIsDeletedChanging(value);
                _IsDeleted = value;
                RaisePropertyChanged("IsDeleted");
                OnIsDeletedChanged();
            }
        }
        private Nullable<Boolean> _IsDeleted;
        partial void OnIsDeletedChanging(Nullable<Boolean> value);
        partial void OnIsDeletedChanged();

        [DataMemberAttribute()]
        public string NationalMedicalCode
        {
            get
            {
                return _NationalMedicalCode;
            }
            set
            {
                OnNationalMedicalCodeChanging(value);
                _NationalMedicalCode = value;
                RaisePropertyChanged("NationalMedicalCode");
                OnNationalMedicalCodeChanged();
            }
        }
        private string _NationalMedicalCode;
        partial void OnNationalMedicalCodeChanging(string value);
        partial void OnNationalMedicalCodeChanged();

        [DataMemberAttribute()]
        public string FileCodeNumber
        {
            get
            {
                return _FileCodeNumber;
            }
            set
            {
                OnFileCodeNumberChanging(value);
                _FileCodeNumber = value;
                RaisePropertyChanged("FileCodeNumber");
                OnFileCodeNumberChanged();
            }
        }
        private string _FileCodeNumber;
        partial void OnFileCodeNumberChanging(string value);
        partial void OnFileCodeNumberChanged();

        private long? _ConsultingDiagnosysID;
        [DataMemberAttribute()]
        public long? ConsultingDiagnosysID
        {
            get
            {
                return _ConsultingDiagnosysID;
            }
            set
            {
                _ConsultingDiagnosysID = value;
                RaisePropertyChanged("ConsultingDiagnosysID");
            }
        }

        private Int16 _NumberOfUpdate;
        [DataMemberAttribute()]
        public Int16 NumberOfUpdate
        {
            //NumberOfUpdate = 7: Cập nhật toàn bộ thông tin bệnh nhân: Thông tin hành chính, thông tin về thẻ đang sử dụng, thông tin về giấy CV đang sử dụng.
            //NumberOfUpdate = 6: Cập nhật thông tin hành chính và thẻ BHYT của bệnh nhân
            //NumberOfUpdate = 5: Cập nhật thông tin hành chính và giấy chuyển viện của bệnh nhân
            //NumberOfUpdate = 4: Chỉ cập nhật thông tin hành chính của bệnh nhân.
            //NumberOfUpdate = 3: Cập nhật thẻ BHYT và Giấy CV của bệnh nhân.
            //NumberOfUpdate = 2: Chỉ cập nhật thẻ bảo hiểm của bệnh nhân.
            //NumberOfUpdate = 1: Chỉ cập nhật giấy chuyển viện của bệnh nhân.
            get
            {
                return _NumberOfUpdate;
            }
            set
            {
                _NumberOfUpdate = value;
                RaisePropertyChanged("NumberOfUpdate");
            }
        }
        #endregion

        #region Navigation Properties
        private CitiesProvince _CitiesProvince;
        [DataMemberAttribute()]
        public CitiesProvince CitiesProvince
        {
            get
            {
                return _CitiesProvince;
            }
            set
            {
                if (_CitiesProvince != value)
                {
                    _CitiesProvince = value;
                    RaisePropertyChanged("CitiesProvince");
                }
            }
        }


        private SuburbNames _SuburbName;
        [DataMemberAttribute()]
        public SuburbNames SuburbName
        {
            get
            {
                return _SuburbName;
            }
            set
            {
                OnSuburbNameChanging(value);
                //if (_SuburbName != value)
                {
                    _SuburbName = value;
                    if (_SuburbName != null
                        && _SuburbName.SuburbNameID > 0)
                    {
                        SuburbNameID = _SuburbName.SuburbNameID;
                    }
                    RaisePropertyChanged("SuburbName");
                }
                OnSuburbNameChanged();
            }
        }
        partial void OnSuburbNameChanging(SuburbNames value);
        partial void OnSuburbNameChanged();

        private ObservableCollection<HealthInsurance> _healthInsurances;
        [DataMemberAttribute()]
        public ObservableCollection<HealthInsurance> HealthInsurances
        {
            get
            {
                //Lazy Loading
                if (_healthInsurances == null && HealthInsuranceLoaded == false)//hasn't been loaded yet
                {
                    if (this.HealthInsuranceIsLoading == false)
                    {
                        if (LoadHealthInsurancesDelegate != null)
                        {
                            this.HealthInsuranceIsLoading = true;
                            LoadHealthInsurancesDelegate(this);
                        }
                    }
                }
                return _healthInsurances;
            }
            set
            {
                if (_healthInsurances != value)
                {
                    _healthInsurances = value;
                    RaisePropertyChanged("HealthInsurances");
                    //Xu ly truong hop update the bao hiem

                    if (_tempPatient != null)
                    {
                        _tempPatient.HealthInsurances = value;
                    }
                }
            }
        }


        private ObservableCollection<PatientClassHistory> _patientClassHistories;
        [DataMemberAttribute()]
        public ObservableCollection<PatientClassHistory> PatientClassHistories
        {
            get
            {
                //Lazy Loading
                if (_patientClassHistories == null && ClassificationHistoryLoaded == false)//hasn't been loaded yet
                {
                    if (this.ClassificationHistoryIsLoading == false)
                    {
                        if (LoadClassificationHistoryDelegate != null)
                        {
                            this.ClassificationHistoryIsLoading = true;
                            LoadClassificationHistoryDelegate(this);
                        }
                    }
                }
                return _patientClassHistories;
            }
            set
            {
                if (_patientClassHistories != value)
                {
                    _patientClassHistories = value;
                    RaisePropertyChanged("PatientClassHistories");
                }
            }
        }
        private PaperReferal _ActivePaperReferal;
        [DataMemberAttribute()]
        public PaperReferal ActivePaperReferal
        {
            get
            {
                return _ActivePaperReferal;
            }
            set
            {
                if (_ActivePaperReferal != value)
                {
                    _ActivePaperReferal = value;
                    RaisePropertyChanged("ActivePaperReferal");
                }
            }
        }

        private ObservableCollection<PaperReferal> _PaperReferals;
        [DataMemberAttribute()]
        public ObservableCollection<PaperReferal> PaperReferals
        {
            get
            {
                return _PaperReferals;
            }
            set
            {
                if (_PaperReferals != value)
                {
                    _PaperReferals = value;
                    RaisePropertyChanged("PaperReferals");
                }
            }
        }


        [DataMemberAttribute()]
        public ObservableCollection<PatientAddressHistory> PatientAddressHistories
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<PatientRegistration> LastNewPatientRegistrations
        {
            get;
            set;
        }


        private byte[] _patientPhotoData;
        public byte[] PatientPhotoData
        {
            get
            {
                return _patientPhotoData;
            }
            set
            {
                if (_patientPhotoData != value)
                {
                    _patientPhotoData = value;
                    //this.RaisePropertyChanged("PatientPhotoData");
                }
            }
        }




        [DataMemberAttribute()]
        public RefCountry RefCountry
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public BloodType VBloodType
        {
            get
            {
                return _VBloodType;
            }
            set
            {
                if (_VBloodType != value)
                {
                    OnVBloodTypeChanging(value);
                    ValidateProperty("VBloodType", value);
                    _VBloodType = value;
                    RaisePropertyChanged("VBloodType");
                    OnVBloodTypeChanged();
                }
            }
        }
        private BloodType _VBloodType;
        partial void OnVBloodTypeChanging(BloodType value);
        partial void OnVBloodTypeChanged();

        [DataMemberAttribute()]
        public bool FromAppointment
        {
            get
            {
                return _FromAppointment;
            }
            set
            {
                if (_FromAppointment != value)
                {
                    OnFromAppointmentChanging(value);
                    ValidateProperty("FromAppointment", value);
                    _FromAppointment = value;
                    RaisePropertyChanged("FromAppointment");
                    OnFromAppointmentChanged();
                }
            }
        }
        private bool _FromAppointment = false;
        partial void OnFromAppointmentChanging(bool value);
        partial void OnFromAppointmentChanged();

        //▼====== #002
        [DataMemberAttribute()]
        public WardNames WardName
        {
            get
            {
                return _WardName;
            }
            set
            {
                if (_WardName != value)
                {
                    _WardName = value;
                    RaisePropertyChanged("WardName");
                }
            }
        }
        private WardNames _WardName;
        //▲====== #002
        [DataMemberAttribute()]
        public long WardNameID
        {
            get
            {
                return _WardNameID;
            }
            set
            {
                if (_WardNameID != value)
                {
                    _WardNameID = value;
                    RaisePropertyChanged("WardNameID");
                }
            }
        }
        private long _WardNameID;

        //▼====== #003
        [DataMemberAttribute()]
        public int DOBNumIndex
        {
            get
            {
                return _DOBNumIndex;
            }
            set
            {
                if (_DOBNumIndex != value)
                {
                    _DOBNumIndex = value;
                    RaisePropertyChanged("DOBNumIndex");
                }
            }
        }
        private int _DOBNumIndex;
        //▲====== #003

        private string _PatientFullStreetAddress;
        public string PatientFullStreetAddress
        {
            get => _PatientFullStreetAddress; set
            {
                _PatientFullStreetAddress = value;
                RaisePropertyChanged("PatientFullStreetAddress");
            }
        }
        #endregion

        #region Ny add member

        private PatientRegistration _LastNewPatientRegistration;
        [DataMemberAttribute()]
        public PatientRegistration LastNewPatientRegistration
        {
            get { return _LastNewPatientRegistration; }
            set
            {
                _LastNewPatientRegistration = value;
                RaisePropertyChanged("LastNewPatientRegistration");
            }
        }

        private PatientClassification _LastNewPatientClassification;
        [DataMemberAttribute()]
        public PatientClassification LastNewPatientClassification
        {
            get { return _LastNewPatientClassification; }
            set
            {
                _LastNewPatientClassification = value;
                RaisePropertyChanged("LastNewPatientClassification");
            }
        }

        private HealthInsurance _LastNewHealthInsurance;
        [DataMemberAttribute()]
        public HealthInsurance LastNewHealthInsurance
        {
            get { return _LastNewHealthInsurance; }
            set
            {
                _LastNewHealthInsurance = value;
                RaisePropertyChanged("LastNewHealthInsurance");
            }
        }
        #endregion

        /*▼====: #001*/
        public HIQRCode QRCode { get; set; }
        /*▲====: #001*/
        private bool _IsSelected = false;
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

        private DateTime? _IDCreatedDate;
        private DateTime? _OccupationDate;
        private string _IDCreatedFrom;
        [DataMemberAttribute]
        public DateTime? IDCreatedDate
        {
            get
            {
                return _IDCreatedDate;
            }
            set
            {
                _IDCreatedDate = value;
                RaisePropertyChanged("IDCreatedDate");
            }
        }
        [DataMemberAttribute]
        public DateTime? OccupationDate
        {
            get
            {
                return _OccupationDate;
            }
            set
            {
                _OccupationDate = value;
                RaisePropertyChanged("OccupationDate");
            }
        }
        [DataMemberAttribute]
        public string IDCreatedFrom
        {
            get
            {
                return _IDCreatedFrom;
            }
            set
            {
                _IDCreatedFrom = value;
                RaisePropertyChanged("IDCreatedFrom");
            }
        }

        private long _OrderNumber;
        public long OrderNumber
        {
            get
            {
                return _OrderNumber;
            }
            set
            {
                _OrderNumber = value;
            }
        }

        private DateTime _ServiceStartedAt;
        public DateTime ServiceStartedAt
        {
            get
            {
                return _ServiceStartedAt;
            }
            set
            {
                _ServiceStartedAt = value;
            }
        }

        private DateTime _ServiceEndedAt;
        public DateTime ServiceEndedAt
        {
            get
            {
                return _ServiceEndedAt;
            }
            set
            {
                _ServiceEndedAt = value;
            }
        }

        //▼====: #004
        private string _SocialInsuranceNumber;
        [DataMemberAttribute]
        public string SocialInsuranceNumber
        {
            get
            {
                return _SocialInsuranceNumber;
            }
            set
            {
                _SocialInsuranceNumber = value;
                RaisePropertyChanged("SocialInsuranceNumber");
            }
        }
        //▲====: #004
        //▼====: #005
        private DateTime? _DateCreatedQMSTicket;
        [DataMemberAttribute]
        public DateTime? DateCreatedQMSTicket
        {
            get
            {
                return _DateCreatedQMSTicket;
            }
            set
            {
                _DateCreatedQMSTicket = value;
                RaisePropertyChanged("DateCreatedQMSTicket");
            }
        }
        //▲====: #005
        private long _JobID130;
        [DataMemberAttribute]
        public long JobID130
        {
            get
            {
                return _JobID130;
            }
            set
            {
                _JobID130 = value;
                RaisePropertyChanged("JobID130");
            }
        }
        private string _Nationality;
        [DataMemberAttribute]
        public string Nationality
        {
            get
            {
                return _Nationality;
            }
            set
            {
                _Nationality = value;
                RaisePropertyChanged("Nationality");
            }
        }
        private string _Passport;
        [DataMemberAttribute]
        public string Passport
        {
            get
            {
                return _Passport;
            }
            set
            {
                _Passport = value;
                RaisePropertyChanged("Passport");
            }
        }
        private string _AllFContact;
        [DataMemberAttribute]
        public string AllFContact
        {
            get
            {
                return _AllFContact;
            }
            set
            {
                _AllFContact = value;
                RaisePropertyChanged("AllFContact");
            }
        }

        private long _V_Job;
        [DataMemberAttribute]
        public long V_Job
        {
            get
            {
                return _V_Job;
            }
            set
            {
                _V_Job = value;
                RaisePropertyChanged("V_Job");
            }
        }

        private string _FContactEmployer;
        [DataMemberAttribute]
        public string FContactEmployer
        {
            get
            {
                return _FContactEmployer;
            }
            set
            {
                _FContactEmployer = value;
                RaisePropertyChanged("FContactEmployer");
            }
        }
        private string _FContactSocialInsuranceNumber;
        [DataMemberAttribute]
        public string FContactSocialInsuranceNumber
        {
            get
            {
                return _FContactSocialInsuranceNumber;
            }
            set
            {
                _FContactSocialInsuranceNumber = value;
                RaisePropertyChanged("FContactSocialInsuranceNumber");
            }
        }
    }
    [DataContract]
    public class APIPatient
    {
        [DataMemberAttribute]
        public string PatientCode { get; set; }
        [DataMemberAttribute]
        public string FullName { get; set; }
        [DataMemberAttribute]
        public string AgeString { get; set; }
        [DataMemberAttribute]
        public int MonthsOld { get; set; }
        [DataMemberAttribute]
        public string GenderString { get; set; }
        [DataMemberAttribute]
        public string PatientFullStreetAddress { get; set; }

        [DataMemberAttribute]
        public string PatientCellPhoneNumber { get; set; }
        [DataMemberAttribute]
        public string DOB { get; set; }
        [DataMemberAttribute]
        public string ContractNo { get; set; }
        [DataMemberAttribute]
        public long? ContractPatientGroupID { get; set; }
    }
}