using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.Collections.Generic;
/*
* 20180409 #001 TTM: Them get set cho certificate va Scode
* 20210803 #002 TNHX: Them get set cho certificate va Scode
* 20211002 #003 TNHX: Them get set cho IsStopUsing
* 20221118 #004 BLQ: Thêm đánh dấu làm việc ngoài giờ
* 20221207 #005 TNHX: 994 Thêm trường cho bsi sử dụng để đăng nhập đơn thuốc điện tử
* 20230710 #006 BLQ: Thêm trường chức vụ 
*/
namespace DataEntities
{
    public partial class Staff : NotifyChangedBase
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

        public Staff ()
        {
            SFirstName = "";
            SLastName = "";
            SMiddleName = "";
            FullName = "";
            Sex =true;
        }
        #endregion
        #region Primitive Properties
        //▼====: #006
        private long _V_JobPosition;
        [DataMemberAttribute()]
        public long V_JobPosition
        {
            get
            {
                return _V_JobPosition;
            }
            set
            {
                _V_JobPosition = value;
                RaisePropertyChanged("V_JobPosition");
            }
        }
        //▲====: #006
        //▼====: #003
        private bool _IsStopUsing;
        [DataMemberAttribute()]
        public bool IsStopUsing
        {
            get
            {
                return _IsStopUsing;
            }
            set
            {
                _IsStopUsing = value;
                RaisePropertyChanged("IsStopUsing");
            }
        }
        //▲====: #003

        [DataMemberAttribute()]
        public Int64 StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                if (_StaffID != value)
                {
                    OnStaffIDChanging(value);
                    _StaffID = value;
                    RaisePropertyChanged("StaffID");
                    OnStaffIDChanged();
                }
            }
        }
        private Int64 _StaffID;
        partial void OnStaffIDChanging(Int64 value);
        partial void OnStaffIDChanged();

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
        public String SFirstName
        {
            get
            {
                return _SFirstName;
            }
            set
            {
                OnSFirstNameChanging(value);
                _SFirstName = value;
                RaisePropertyChanged("SFirstName");
                OnSFirstNameChanged();
            }
        }
        private String _SFirstName;
        partial void OnSFirstNameChanging(String value);
        partial void OnSFirstNameChanged();

        [DataMemberAttribute()]
        public String SMiddleName
        {
            get
            {
                return _SMiddleName;
            }
            set
            {
                OnSMiddleNameChanging(value);
                _SMiddleName = value;
                RaisePropertyChanged("SMiddleName");
                OnSMiddleNameChanged();
            }
        }
        private String _SMiddleName;
        partial void OnSMiddleNameChanging(String value);
        partial void OnSMiddleNameChanged();

        [DataMemberAttribute()]
        public String SLastName
        {
            get
            {
                return _SLastName;
            }
            set
            {
                OnSLastNameChanging(value);
                _SLastName = value;
                RaisePropertyChanged("SLastName");
                OnSLastNameChanged();
            }
        }
        private String _SLastName;
        partial void OnSLastNameChanging(String value);
        partial void OnSLastNameChanged();

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
        public Nullable<DateTime> SDOB
        {
            get
            {
                return _SDOB;
            }
            set
            {
                OnSDOBChanging(value);
                _SDOB = value;
                RaisePropertyChanged("SDOB");
                OnSDOBChanged();
            }
        }
        private Nullable<DateTime> _SDOB;
        partial void OnSDOBChanging(Nullable<DateTime> value);
        partial void OnSDOBChanged();

        [DataMemberAttribute()]
        public String SBirthPlace
        {
            get
            {
                return _SBirthPlace;
            }
            set
            {
                OnSBirthPlaceChanging(value);
                _SBirthPlace = value;
                RaisePropertyChanged("SBirthPlace");
                OnSBirthPlaceChanged();
            }
        }
        private String _SBirthPlace;
        partial void OnSBirthPlaceChanging(String value);
        partial void OnSBirthPlaceChanged();

        [DataMemberAttribute()]
        public String SIDNumber
        {
            get
            {
                return _SIDNumber;
            }
            set
            {
                OnSIDNumberChanging(value);
                _SIDNumber = value;
                RaisePropertyChanged("SIDNumber");
                OnSIDNumberChanged();
            }
        }
        private String _SIDNumber;
        partial void OnSIDNumberChanging(String value);
        partial void OnSIDNumberChanged();

        [DataMemberAttribute()]
        public String SPlaceOfIssue
        {
            get
            {
                return _SPlaceOfIssue;
            }
            set
            {
                OnSPlaceOfIssueChanging(value);
                _SPlaceOfIssue = value;
                RaisePropertyChanged("SPlaceOfIssue");
                OnSPlaceOfIssueChanged();
            }
        }
        private String _SPlaceOfIssue;
        partial void OnSPlaceOfIssueChanging(String value);
        partial void OnSPlaceOfIssueChanged();

        [DataMemberAttribute()]
        public String SStreetAddress
        {
            get
            {
                return _SStreetAddress;
            }
            set
            {
                OnSStreetAddressChanging(value);
                _SStreetAddress = value;
                RaisePropertyChanged("SStreetAddress");
                OnSStreetAddressChanged();
            }
        }
        private String _SStreetAddress;
        partial void OnSStreetAddressChanging(String value);
        partial void OnSStreetAddressChanged();

        [DataMemberAttribute()]
        public String SSurburb
        {
            get
            {
                return _SSurburb;
            }
            set
            {
                OnSSurburbChanging(value);
                _SSurburb = value;
                RaisePropertyChanged("SSurburb");
                OnSSurburbChanged();
            }
        }
        private String _SSurburb;
        partial void OnSSurburbChanging(String value);
        partial void OnSSurburbChanged();

        [DataMemberAttribute()]
        public String SState
        {
            get
            {
                return _SState;
            }
            set
            {
                OnSStateChanging(value);
                _SState = value;
                RaisePropertyChanged("SState");
                OnSStateChanged();
            }
        }
        private String _SState;
        partial void OnSStateChanging(String value);
        partial void OnSStateChanged();

        [DataMemberAttribute()]
        public String SPhoneNumber
        {
            get
            {
                return _SPhoneNumber;
            }
            set
            {
                OnSPhoneNumberChanging(value);
                _SPhoneNumber = value;
                RaisePropertyChanged("SPhoneNumber");
                OnSPhoneNumberChanged();
            }
        }
        private String _SPhoneNumber;
        partial void OnSPhoneNumberChanging(String value);
        partial void OnSPhoneNumberChanged();

        [DataMemberAttribute()]
        public String SMobiPhoneNumber
        {
            get
            {
                return _SMobiPhoneNumber;
            }
            set
            {
                OnSMobiPhoneNumberChanging(value);
                _SMobiPhoneNumber = value;
                RaisePropertyChanged("SMobiPhoneNumber");
                OnSMobiPhoneNumberChanged();
            }
        }
        private String _SMobiPhoneNumber;
        partial void OnSMobiPhoneNumberChanging(String value);
        partial void OnSMobiPhoneNumberChanged();

        [DataMemberAttribute()]
        public String SEmailAddress
        {
            get
            {
                return _SEmailAddress;
            }
            set
            {
                OnSEmailAddressChanging(value);
                _SEmailAddress = value;
                RaisePropertyChanged("SEmailAddress");
                OnSEmailAddressChanged();
            }
        }
        private String _SEmailAddress;
        partial void OnSEmailAddressChanging(String value);
        partial void OnSEmailAddressChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> SEmployDate
        {
            get
            {
                return _SEmployDate;
            }
            set
            {
                OnSEmployDateChanging(value);
                _SEmployDate = value;
                RaisePropertyChanged("SEmployDate");
                OnSEmployDateChanged();
            }
        }
        private Nullable<DateTime> _SEmployDate;
        partial void OnSEmployDateChanging(Nullable<DateTime> value);
        partial void OnSEmployDateChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> SLeftDate
        {
            get
            {
                return _SLeftDate;
            }
            set
            {
                OnSLeftDateChanging(value);
                _SLeftDate = value;
                RaisePropertyChanged("SLeftDate");
                OnSLeftDateChanged();
            }
        }
        private Nullable<DateTime> _SLeftDate;
        partial void OnSLeftDateChanging(Nullable<DateTime> value);
        partial void OnSLeftDateChanged();

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
        public String SAccountNumber
        {
            get
            {
                return _SAccountNumber;
            }
            set
            {
                OnSAccountNumberChanging(value);
                _SAccountNumber = value;
                RaisePropertyChanged("SAccountNumber");
                OnSAccountNumberChanged();
            }
        }
        private String _SAccountNumber;
        partial void OnSAccountNumberChanging(String value);
        partial void OnSAccountNumberChanged();
        //▼====: #001
        [DataMemberAttribute()]
        public String SCertificateNumber
        {
            get
            {
                return _SCertificateNumber;
            }
            set
            {
                OnSCertificateNumberChanging(value);
                _SCertificateNumber = value;
                RaisePropertyChanged("SCertificateNumber");
                OnSCertificateNumberChanged();
            }
        }
        private String _SCertificateNumber;
        partial void OnSCertificateNumberChanging(String value);
        partial void OnSCertificateNumberChanged();

        [DataMemberAttribute()]
        public String SCode
        {
            get
            {
                return _SCode;
            }
            set
            {
                OnSCodeChanging(value);
                _SCode = value;
                RaisePropertyChanged("SCode");
                OnSCodeChanged();
            }
        }
        private String _SCode;
        partial void OnSCodeChanging(String value);
        partial void OnSCodeChanged();

        [DataMemberAttribute()]
        //▲====: #001
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
        public String SEmploymentHistory
        {
            get
            {
                return _SEmploymentHistory;
            }
            set
            {
                OnSEmploymentHistoryChanging(value);
                _SEmploymentHistory = value;
                RaisePropertyChanged("SEmploymentHistory");
                OnSEmploymentHistoryChanged();
            }
        }
        private String _SEmploymentHistory;
        partial void OnSEmploymentHistoryChanging(String value);
        partial void OnSEmploymentHistoryChanged();

        [DataMemberAttribute()]
        public String SImage
        {
            get
            {
                return _SImage;
            }
            set
            {
                OnSImageChanging(value);
                _SImage = value;
                RaisePropertyChanged("SImage");
                OnSImageChanged();
            }
        }
        private String _SImage;
        partial void OnSImageChanging(String value);
        partial void OnSImageChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> SCreateDate
        {
            get
            {
                return _SCreateDate;
            }
            set
            {
                OnSCreateDateChanging(value);
                _SCreateDate = value;
                RaisePropertyChanged("SCreateDate");
                OnSCreateDateChanged();
            }
        }
        private Nullable<DateTime> _SCreateDate;
        partial void OnSCreateDateChanging(Nullable<DateTime> value);
        partial void OnSCreateDateChanged();

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

        private bool _IsFund = false;
        [DataMemberAttribute()]
        public bool IsFund
        {
            get
            {
                return _IsFund;
            }
            set
            {
                _IsFund = value;
                RaisePropertyChanged("IsFund");
            }
        }

        private long? _V_AcademicRank;
        [DataMemberAttribute()]
        public long? V_AcademicRank
        {
            get
            {
                return _V_AcademicRank;
            }
            set
            {
                _V_AcademicRank = value;
                RaisePropertyChanged("V_AcademicRank");
            }
        }

        private long? _V_AcademicDegree;
        [DataMemberAttribute()]
        public long? V_AcademicDegree
        {
            get
            {
                return _V_AcademicDegree;
            }
            set
            {
                _V_AcademicDegree = value;
                RaisePropertyChanged("V_AcademicDegree");
            }
        }

        private long? _V_Education;
        [DataMemberAttribute()]
        public long? V_Education
        {
            get
            {
                return _V_Education;
            }
            set
            {
                _V_Education = value;
                RaisePropertyChanged("V_Education");
            }
        }

        private bool _AllowRegWithoutTicket = false;
        [DataMemberAttribute()]
        public bool AllowRegWithoutTicket
        {
            get
            {
                return _AllowRegWithoutTicket;
            }
            set
            {
                _AllowRegWithoutTicket = value;
                RaisePropertyChanged("AllowRegWithoutTicket");
            }
        }

        private bool _IsReport = false;
        [DataMemberAttribute()]
        public bool IsReport
        {
            get
            {
                return _IsReport;
            }
            set
            {
                _IsReport = value;
                RaisePropertyChanged("IsReport");
            }
        }

        private bool _HiddenFullNameOnReport = false;
        [DataMemberAttribute()]
        public bool HiddenFullNameOnReport
        {
            get
            {
                return _HiddenFullNameOnReport;
            }
            set
            {
                _HiddenFullNameOnReport = value;
                RaisePropertyChanged("HiddenFullNameOnReport");
            }
        }

        private string _ListDeptResponsibilities;
        [DataMemberAttribute()]
        public string ListDeptResponsibilities
        {
            get
            {
                return _ListDeptResponsibilities;
            }
            set
            {
                _ListDeptResponsibilities = value;
                RaisePropertyChanged("ListDeptResponsibilities");
            }
        }
        private string _UserDomain;
        [DataMemberAttribute()]
        public string UserDomain
        {
            get
            {
                return _UserDomain;
            }
            set
            {
                _UserDomain = value;
                RaisePropertyChanged("UserDomain");
            }
        }
        private bool _MarkDeleted;
        [DataMemberAttribute()]
        public bool MarkDeleted
        {
            get
            {
                return _MarkDeleted;
            }
            set
            {
                _MarkDeleted = value;
                RaisePropertyChanged("MarkDeleted");
            }
        }

        //▼====: #002
        private string _SocialInsuranceNumber;
        [DataMemberAttribute()]
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
        //▲====: #002
        //▼====: #004
        private bool _IsOvertimeWorking;
        [DataMemberAttribute()]
        public bool IsOvertimeWorking
        {
            get
            {
                return _IsOvertimeWorking;
            }
            set
            {
                _IsOvertimeWorking = value;
                RaisePropertyChanged("IsOvertimeWorking");
            }
        }
        //▲====: #004
        //▼====: #005
        private string _MaLienThongBacSi;
        [DataMemberAttribute()]
        public string MaLienThongBacSi
        {
            get
            {
                return _MaLienThongBacSi;
            }
            set
            {
                _MaLienThongBacSi = value;
                RaisePropertyChanged("MaLienThongBacSi");
            }
        }
        private string _MatKhauLienThongBacSi;
        [DataMemberAttribute()]
        public string MatKhauLienThongBacSi
        {
            get
            {
                return _MatKhauLienThongBacSi;
            }
            set
            {
                _MatKhauLienThongBacSi = value;
                RaisePropertyChanged("MatKhauLienThongBacSi");
            }
        }
        private string _MaDinhDanhBsi;
        [DataMemberAttribute()]
        public string MaDinhDanhBsi
        {
            get
            {
                return _MaDinhDanhBsi;
            }
            set
            {
                _MaDinhDanhBsi = value;
                RaisePropertyChanged("MaDinhDanhBsi");
            }
        }
        private bool _ModifiedPasswordDT;
        [DataMemberAttribute()]
        public bool ModifiedPasswordDT
        {
            get
            {
                return _ModifiedPasswordDT;
            }
            set
            {
                _ModifiedPasswordDT = value;
                RaisePropertyChanged("ModifiedPasswordDT");
            }
        }
        //▲====: #005
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

        private string _PrintTitle;
        [DataMemberAttribute()]
        public string PrintTitle
        {
            get { return _PrintTitle; }
            set
            {
                if (_PrintTitle != value)
                {
                    _PrintTitle = value;
                    RaisePropertyChanged("PrintTitle");
                }
            }
        }

        private bool _IsUnitLeader;
        [DataMemberAttribute()]
        public bool IsUnitLeader
        {
            get { return _IsUnitLeader; }
            set
            {
                if (_IsUnitLeader != value)
                {
                    _IsUnitLeader = value;
                    RaisePropertyChanged("IsUnitLeader");
                }
            }
        }

        private string _ListPCLResultParamImpID;
        [DataMemberAttribute()]
        public string ListPCLResultParamImpID
        {
            get { return _ListPCLResultParamImpID; }
            set
            {
                if (_ListPCLResultParamImpID != value)
                {
                    _ListPCLResultParamImpID = value;
                    RaisePropertyChanged("ListPCLResultParamImpID");
                }
            }
        }

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
        private List<ConsultationTimeSegments> _ConsultationTimeSegmentsList;
        [DataMemberAttribute()]
        public List<ConsultationTimeSegments> ConsultationTimeSegmentsList
        {
            get
            {
                return _ConsultationTimeSegmentsList;
            }
            set
            {
                _ConsultationTimeSegmentsList = value;
                RaisePropertyChanged("ConsultationTimeSegmentsList");
            }
        }
        #endregion
        public override bool Equals(object obj)
        {
            Staff seletedStore = obj as Staff;
            if (seletedStore == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.StaffID == seletedStore.StaffID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return FullName;
        }
    }
    public class APIStaff
    {
        [DataMemberAttribute]
        public long StaffID { get; set; }
        [DataMemberAttribute]
        public string FullName { get; set; }
        [DataMemberAttribute]
        public string UserDomain { get; set; }
    }
}