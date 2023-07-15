using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class StaffPersons : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new Staff object.

        /// <param name="staffID">Initial value of the StaffID property.</param>
        /// <param name="sFirstName">Initial value of the SFirstName property.</param>
        /// <param name="sLastName">Initial value of the SLastName property.</param>
        public static Staff CreateStaff(Int64 staffID, String sFirstName, String sLastName)
        {
            Staff staff = new Staff();
            staff.StaffID = staffID;
            staff.SFirstName = sFirstName;
            staff.SLastName = sLastName;
            return staff;
        }

        public StaffPersons()
        {
            FirstName = "";
            LastName = "";
            MiddleName = "";
            FullName = "";
            Sex =true;
        }
        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 StaffPersonID
        {
            get
            {
                return _StaffPersonID;
            }
            set
            {
                if (_StaffPersonID != value)
                {
                    OnStaffPersonIDChanging(value);
                    _StaffPersonID = value;
                    RaisePropertyChanged("StaffPersonID");
                    OnStaffPersonIDChanged();
                }
            }
        }
        private Int64 _StaffPersonID;
        partial void OnStaffPersonIDChanging(Int64 value);
        partial void OnStaffPersonIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> CountryID
        {
            get
            {
                return _CountryID;
            }
            set
            {
                OnCountryIDChanging(value);
                _CountryID = value;
                RaisePropertyChanged("CountryID");
                OnCountryIDChanged();
            }
        }
        private Nullable<long> _CountryID;
        partial void OnCountryIDChanging(Nullable<long> value);
        partial void OnCountryIDChanged();

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
        public Nullable<Int64> StaffCatgID
        {
            get
            {
                return _StaffCatgID;
            }
            set
            {
                OnStaffCatgIDChanging(value);
                _StaffCatgID = value;
                RaisePropertyChanged("StaffCatgID");
                OnStaffCatgIDChanged();
            }
        }
        private Nullable<Int64> _StaffCatgID;
        partial void OnStaffCatgIDChanging(Nullable<Int64> value);
        partial void OnStaffCatgIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> CityProvinceID
        {
            get
            {
                return _CityProvinceID;
            }
            set
            {
                OnCityProvinceIDChanging(value);
                _CityProvinceID = value;
                RaisePropertyChanged("CityProvinceID");
                OnCityProvinceIDChanged();
            }
        }
        private Nullable<long> _CityProvinceID;
        partial void OnCityProvinceIDChanging(Nullable<long> value);
        partial void OnCityProvinceIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> RoleCode
        {
            get
            {
                return _RoleCode;
            }
            set
            {
                OnRoleCodeChanging(value);
                _RoleCode = value;
                RaisePropertyChanged("RoleCode");
                OnRoleCodeChanged();
            }
        }
        private Nullable<long> _RoleCode;
        partial void OnRoleCodeChanging(Nullable<long> value);
        partial void OnRoleCodeChanged();

        [DataMemberAttribute()]
        public String FirstName
        {
            get
            {
                return _FirstName;
            }
            set
            {
                OnFirstNameChanging(value);
                _FirstName = value;
                RaisePropertyChanged("FirstName");
                OnFirstNameChanged();
            }
        }
        private String _FirstName;
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
                OnMiddleNameChanging(value);
                _MiddleName = value;
                RaisePropertyChanged("MiddleName");
                OnMiddleNameChanged();
            }
        }
        private String _MiddleName;
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
                OnLastNameChanging(value);
                _LastName = value;
                RaisePropertyChanged("LastName");
                OnLastNameChanged();
            }
        }
        private String _LastName;
        partial void OnLastNameChanging(String value);
        partial void OnLastNameChanged();

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
        public Nullable<DateTime> DOB
        {
            get
            {
                return _DOB;
            }
            set
            {
                OnDOBChanging(value);
                _DOB = value;
                RaisePropertyChanged("DOB");
                OnDOBChanged();
            }
        }
        private Nullable<DateTime> _DOB;
        partial void OnDOBChanging(Nullable<DateTime> value);
        partial void OnDOBChanged();

        [DataMemberAttribute()]
        public String BirthPlace
        {
            get
            {
                return _BirthPlace;
            }
            set
            {
                OnBirthPlaceChanging(value);
                _BirthPlace = value;
                RaisePropertyChanged("BirthPlace");
                OnBirthPlaceChanged();
            }
        }
        private String _BirthPlace;
        partial void OnBirthPlaceChanging(String value);
        partial void OnBirthPlaceChanged();

        [DataMemberAttribute()]
        public String IDNumber
        {
            get
            {
                return _IDNumber;
            }
            set
            {
                OnIDNumberChanging(value);
                _IDNumber = value;
                RaisePropertyChanged("IDNumber");
                OnIDNumberChanged();
            }
        }
        private String _IDNumber;
        partial void OnIDNumberChanging(String value);
        partial void OnIDNumberChanged();

        [DataMemberAttribute()]
        public String PlaceOfIssue
        {
            get
            {
                return _PlaceOfIssue;
            }
            set
            {
                OnPlaceOfIssueChanging(value);
                _PlaceOfIssue = value;
                RaisePropertyChanged("PlaceOfIssue");
                OnPlaceOfIssueChanged();
            }
        }
        private String _PlaceOfIssue;
        partial void OnPlaceOfIssueChanging(String value);
        partial void OnPlaceOfIssueChanged();

        [DataMemberAttribute()]
        public String StreetAddress
        {
            get
            {
                return _StreetAddress;
            }
            set
            {
                OnStreetAddressChanging(value);
                _StreetAddress = value;
                RaisePropertyChanged("StreetAddress");
                OnStreetAddressChanged();
            }
        }
        private String _StreetAddress;
        partial void OnStreetAddressChanging(String value);
        partial void OnStreetAddressChanged();

        [DataMemberAttribute()]
        public String Surburb
        {
            get
            {
                return _Surburb;
            }
            set
            {
                OnSurburbChanging(value);
                _Surburb = value;
                RaisePropertyChanged("Surburb");
                OnSurburbChanged();
            }
        }
        private String _Surburb;
        partial void OnSurburbChanging(String value);
        partial void OnSurburbChanged();

        [DataMemberAttribute()]
        public String State
        {
            get
            {
                return _State;
            }
            set
            {
                OnStateChanging(value);
                _State = value;
                RaisePropertyChanged("State");
                OnStateChanged();
            }
        }
        private String _State;
        partial void OnStateChanging(String value);
        partial void OnStateChanged();

        [DataMemberAttribute()]
        public String PhoneNumber
        {
            get
            {
                return _PhoneNumber;
            }
            set
            {
                OnPhoneNumberChanging(value);
                _PhoneNumber = value;
                RaisePropertyChanged("PhoneNumber");
                OnPhoneNumberChanged();
            }
        }
        private String _PhoneNumber;
        partial void OnPhoneNumberChanging(String value);
        partial void OnPhoneNumberChanged();

        [DataMemberAttribute()]
        public String MobiPhoneNumber
        {
            get
            {
                return _MobiPhoneNumber;
            }
            set
            {
                OnMobiPhoneNumberChanging(value);
                _MobiPhoneNumber = value;
                RaisePropertyChanged("MobiPhoneNumber");
                OnMobiPhoneNumberChanged();
            }
        }
        private String _MobiPhoneNumber;
        partial void OnMobiPhoneNumberChanging(String value);
        partial void OnMobiPhoneNumberChanged();

        [DataMemberAttribute()]
        public String EmailAddress
        {
            get
            {
                return _EmailAddress;
            }
            set
            {
                OnEmailAddressChanging(value);
                _EmailAddress = value;
                RaisePropertyChanged("EmailAddress");
                OnEmailAddressChanged();
            }
        }
        private String _EmailAddress;
        partial void OnEmailAddressChanging(String value);
        partial void OnEmailAddressChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> EmployDate
        {
            get
            {
                return _EmployDate;
            }
            set
            {
                OnEmployDateChanging(value);
                _EmployDate = value;
                RaisePropertyChanged("EmployDate");
                OnEmployDateChanged();
            }
        }
        private Nullable<DateTime> _EmployDate;
        partial void OnEmployDateChanging(Nullable<DateTime> value);
        partial void OnEmployDateChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> LeftDate
        {
            get
            {
                return _LeftDate;
            }
            set
            {
                OnLeftDateChanging(value);
                _LeftDate = value;
                RaisePropertyChanged("LeftDate");
                OnLeftDateChanged();
            }
        }
        private Nullable<DateTime> _LeftDate;
        partial void OnLeftDateChanging(Nullable<DateTime> value);
        partial void OnLeftDateChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> V_Religion
        {
            get
            {
                return _V_Religion;
            }
            set
            {
                OnV_ReligionChanging(value);
                _V_Religion = value;
                RaisePropertyChanged("V_Religion");
                OnV_ReligionChanged();
            }
        }
        private Nullable<Int64> _V_Religion;
        partial void OnV_ReligionChanging(Nullable<Int64> value);
        partial void OnV_ReligionChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> V_MaritalStatus
        {
            get
            {
                return _V_MaritalStatus;
            }
            set
            {
                OnV_MaritalStatusChanging(value);
                _V_MaritalStatus = value;
                RaisePropertyChanged("V_MaritalStatus");
                OnV_MaritalStatusChanged();
            }
        }
        private Nullable<Int64> _V_MaritalStatus;
        partial void OnV_MaritalStatusChanging(Nullable<Int64> value);
        partial void OnV_MaritalStatusChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> V_Ethnic
        {
            get
            {
                return _V_Ethnic;
            }
            set
            {
                OnV_EthnicChanging(value);
                _V_Ethnic = value;
                RaisePropertyChanged("V_Ethnic");
                OnV_EthnicChanged();
            }
        }
        private Nullable<Int64> _V_Ethnic;
        partial void OnV_EthnicChanging(Nullable<Int64> value);
        partial void OnV_EthnicChanged();

        [DataMemberAttribute()]
        public String AccountNumber
        {
            get
            {
                return _AccountNumber;
            }
            set
            {
                OnAccountNumberChanging(value);
                _AccountNumber = value;
                RaisePropertyChanged("AccountNumber");
                OnAccountNumberChanged();
            }
        }
        private String _AccountNumber;
        partial void OnAccountNumberChanging(String value);
        partial void OnAccountNumberChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> V_BankName
        {
            get
            {
                return _V_BankName;
            }
            set
            {
                OnV_BankNameChanging(value);
                _V_BankName = value;
                RaisePropertyChanged("V_BankName");
                OnV_BankNameChanged();
            }
        }
        private Nullable<Int64> _V_BankName;
        partial void OnV_BankNameChanging(Nullable<Int64> value);
        partial void OnV_BankNameChanged();

        [DataMemberAttribute()]
        public String EmploymentHistory
        {
            get
            {
                return _EmploymentHistory;
            }
            set
            {
                OnEmploymentHistoryChanging(value);
                _EmploymentHistory = value;
                RaisePropertyChanged("EmploymentHistory");
                OnEmploymentHistoryChanged();
            }
        }
        private String _EmploymentHistory;
        partial void OnEmploymentHistoryChanging(String value);
        partial void OnEmploymentHistoryChanged();

        [DataMemberAttribute()]
        public String Image
        {
            get
            {
                return _Image;
            }
            set
            {
                OnImageChanging(value);
                _Image = value;
                RaisePropertyChanged("Image");
                OnImageChanged();
            }
        }
        private String _Image;
        partial void OnImageChanging(String value);
        partial void OnImageChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> CreateDate
        {
            get
            {
                return _CreateDate;
            }
            set
            {
                OnCreateDateChanging(value);
                _CreateDate = value;
                RaisePropertyChanged("CreateDate");
                OnCreateDateChanged();
            }
        }
        private Nullable<DateTime> _CreateDate;
        partial void OnCreateDateChanging(Nullable<DateTime> value);
        partial void OnCreateDateChanged();

        [DataMemberAttribute()]
        public Byte[] PImage
        {
            get
            {
                return _PImage;
            }
            set
            {
                OnPImageChanging(value);
                _PImage = value;
                RaisePropertyChanged("PImage");
                OnPImageChanged();
            }
        }
        private Byte[] _PImage;
        partial void OnPImageChanging(Byte[] value);
        partial void OnPImageChanged();

        [DataMemberAttribute()]
        public bool Sex
        {
            get
            {
                return _Sex;
            }
            set
            {
                OnSexChanging(value);
                _Sex = value;
                RaisePropertyChanged("Sex");
                OnSexChanged();
            }
        }
        private bool _Sex;
        partial void OnSexChanging(bool value);
        partial void OnSexChanged();

        [DataMemberAttribute()]
        public string UserAccountsName
        {
            get
            {
                return _UserAccountsName;
            }
            set
            {
                OnUserAccountsNameChanging(value);
                _UserAccountsName = value;
                RaisePropertyChanged("UserAccountsName");
                OnUserAccountsNameChanged();
            }
        }
        private string _UserAccountsName;
        partial void OnUserAccountsNameChanging(string value);
        partial void OnUserAccountsNameChanged();
        #endregion

        #region Navigation Properties
        [DataMemberAttribute()]
        public RefDepartment RefDepartment
        {
          get
            {
                return _RefDepartment;
            }
            set
            {
                if (_RefDepartment != value)
                {
                    OnRefDepartmentChanging(value);
                    _RefDepartment = value;
                    RaisePropertyChanged("RefDepartment");
                    OnRefDepartmentChanged();
                }
            }
        }
        private RefDepartment _RefDepartment;
        partial void OnRefDepartmentChanging(RefDepartment value);
        partial void OnRefDepartmentChanged();
     
        [DataMemberAttribute()]
        public RefRole RefRole
        {
            get
            {
                return _RefRole;
            }
            set
            {
                if (_RefRole != value)
                {
                    OnRefRoleChanging(value);
                    _RefRole = value;
                    RaisePropertyChanged("RefRole");
                    OnRefRoleChanged();
                }
            }
        }
        private RefRole _RefRole;
        partial void OnRefRoleChanging(RefRole value);
        partial void OnRefRoleChanged();

        [DataMemberAttribute()]
        public RefStaffCategory RefStaffCategory
        {
            get
            {
                return _RefStaffCategory;
            }
            set
            {
                if (_RefStaffCategory != value)
                {
                    OnRefStaffCategoryChanging(value);
                    _RefStaffCategory = value;
                    RaisePropertyChanged("RefStaffCategory");
                    OnRefStaffCategoryChanged();
                }
            }
        }
        private RefStaffCategory _RefStaffCategory;
        partial void OnRefStaffCategoryChanging(RefStaffCategory value);
        partial void OnRefStaffCategoryChanged();

        [DataMemberAttribute()]
        public Lookup Ethnic
        {
            get
            {
                return _Ethnic;
            }
            set
            {
                if (_Ethnic != value)
                {
                    OnEthnicChanging(value);
                    _Ethnic = value;
                    RaisePropertyChanged("Ethnic");
                    OnEthnicChanged();
                }
            }
        }
        private Lookup _Ethnic;
        partial void OnEthnicChanging(Lookup value);
        partial void OnEthnicChanged();

        [DataMemberAttribute()]
        public Lookup MaritalStatus
        {
            get
            {
                return _MaritalStatus;
            }
            set
            {
                if (_MaritalStatus != value)
                {
                    OnMaritalStatusChanging(value);
                    _MaritalStatus = value;
                    RaisePropertyChanged("MaritalStatus");
                    OnMaritalStatusChanged();
                }
            }
        }
        private Lookup _MaritalStatus;
        partial void OnMaritalStatusChanging(Lookup value);
        partial void OnMaritalStatusChanged();

        [DataMemberAttribute()]
        public Lookup Religion
        {
            get
            {
                return _Religion;
            }
            set
            {
                if (_Religion != value)
                {
                    OnReligionChanging(value);
                    _Religion = value;
                    RaisePropertyChanged("Religion");
                    OnReligionChanged();
                }
            }
        }
        private Lookup _Religion;
        partial void OnReligionChanging(Lookup value);
        partial void OnReligionChanged();

        [DataMemberAttribute()]
        public Lookup BankName
        {
            get
            {
                return _BankName;
            }
            set
            {
                if (_BankName != value)
                {
                    OnBankNameChanging(value);
                    _BankName = value;
                    RaisePropertyChanged("BankName");
                    OnBankNameChanged();
                }
            }
        }
        private Lookup _BankName;
        partial void OnBankNameChanging(Lookup value);
        partial void OnBankNameChanged();

        [DataMemberAttribute()]
        public ObservableCollection<Allocation> Allocations
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public CitiesProvince CitiesProvince
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<CommonMedicalRecord> CommonMedicalRecords
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<Discipline> Disciplines
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<DiseasesReference> DiseasesReferences
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<DMedRscrSellingPriceTable> DMedRscrSellingPriceTables
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<DrugSellingPriceTable> DrugSellingPriceTables
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<EmergencyRecord> EmergencyRecords
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<HistorySalary> HistorySalaries
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<InsuranceBenefit> InsuranceBenefits
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<InwardDMedRscrInvoice> InwardDMedRscrInvoices
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<InwardDrugInvoice> InwardDrugInvoices
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<InwardDrug> InwardDrugs
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<InwardDrug> InwardDrugs1
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<InwardResource> InwardResources
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<JobResult> JobResults
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<MedicalSurgicalConsumable> MedicalSurgicalConsumables
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<NextOfKin> NextOfKins
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<OutwardDrugInvoice> OutwardDrugInvoices
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<OutwardResource> OutwardResources
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<PatientPCLImagingResult> PatientPCLExamResults
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<PatientRegistration> PatientRegistrations
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<PatientRegistrationDetail> PatientRegistrationDetails
        {
            get;
            set;
        }
        [DataMemberAttribute()]
        public ObservableCollection<PatientServiceRecord> PatientServiceRecords
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<PatientTransactionDetail> PatientTransactionDetails
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<PersonalProperty> PersonalProperties
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<Prescription> Prescriptions
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<ProfessionalSkill> ProfessionalSkills
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public RefCountry RefCountry
        {
            get;
            set;
        }


        [DataMemberAttribute()]
        public ObservableCollection<Reward> Rewards
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<SocialAndHealthInsurance> SocialAndHealthInsurances
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<StaffAllowance> StaffAllowances
        {
            get;
            set;
        }
         [DataMemberAttribute()]
        public ObservableCollection<StaffContract> StaffContracts
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<SurgeryTeam> SurgeryTeams
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<TrainingInstitution> TrainingInstitutions
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<UserAccount> UserAccounts
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<WorkingHour> WorkingHours
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<WorkingSchedule> WorkingSchedules
        {
            get;
            set;
        }

        #endregion

        public override bool Equals(object obj)
        {
            StaffPersons seletedStore = obj as StaffPersons;
            if (seletedStore == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.StaffPersonID == seletedStore.StaffPersonID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

}
