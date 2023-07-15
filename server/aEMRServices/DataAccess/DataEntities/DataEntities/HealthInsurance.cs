using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
using Service.Core.Common;
/*
 * 20181113 #001 TTM: BM 0005228: Bổ sung thêm phường xã.
 */
namespace DataEntities
{
    public partial class HealthInsurance : EntityBase, IEditableObject
    {
        public HealthInsurance()
            : base()
        {
            CanEdit = true;
            CanDelete = true;
        }

        private HealthInsurance _tempHealthInsurance;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempHealthInsurance = (HealthInsurance)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempHealthInsurance)
                CopyFrom(_tempHealthInsurance);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(HealthInsurance p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new HealthInsurance object.

        /// <param name="hIID">Initial value of the HIID property.</param>
        /// <param name="hICardNo">Initial value of the HICardNo property.</param>
        public static HealthInsurance CreateHealthInsurance(long hIID, String hICardNo)
        {
            HealthInsurance healthInsurance = new HealthInsurance();
            healthInsurance.HIID = hIID;
            healthInsurance.HICardNo = hICardNo;
            return healthInsurance;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long HIID
        {
            get
            {
                return _HIID;
            }
            set
            {
                if (_HIID != value)
                {
                    OnHIIDChanging(value);
                    ////ReportPropertyChanging("HIID");
                    _HIID = value;
                    RaisePropertyChanged("HIID");
                    OnHIIDChanged();
                }
            }
        }
        private long _HIID;
        partial void OnHIIDChanging(long value);
        partial void OnHIIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> PatientID
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
                    ////ReportPropertyChanging("PatientID");
                    _PatientID = value;
                    RaisePropertyChanged("PatientID");
                    OnPatientIDChanged(); 
                }
            }
        }
        private Nullable<long> _PatientID;
        partial void OnPatientIDChanging(Nullable<long> value);
        partial void OnPatientIDChanged();





        [DataMemberAttribute()]
        public Nullable<Int32> IBID
        {
            get
            {
                return _IBID;
            }
            set
            {
                if (_IBID != value)
                {
                    OnIBIDChanging(value);
                    ////ReportPropertyChanging("IBID");
                    _IBID = value;
                    RaisePropertyChanged("IBID");
                    OnIBIDChanged(); 
                }
            }
        }
        private Nullable<Int32> _IBID;
        partial void OnIBIDChanging(Nullable<Int32> value);
        partial void OnIBIDChanged();





        [DataMemberAttribute()]
        public String HIPCode
        {
            get
            {
                return _HIPCode;
            }
            set
            {
                if (_HIPCode != value)
                {
                    OnHIPCodeChanging(value);
                    ////ReportPropertyChanging("HIPCode");
                    _HIPCode = value;
                    RaisePropertyChanged("HIPCode");
                    OnHIPCodeChanged(); 
                }
            }
        }
        private String _HIPCode;
        partial void OnHIPCodeChanging(String value);
        partial void OnHIPCodeChanged();


        //KMx: Khi Set HICardNo thì không Validate, khi lưu thẻ BH mới Validate (23/02/2014 14:23).
        //Vì khi Validate phải dựa vào V_HICardType. Nếu Set HiCardNo trước, sau đó mới Set V_HICardType thì khi Validate HiCardNo sẽ bị sai.
        //[CustomValidation(typeof(HealthInsurance), "ValidateHICardNo")]
        [Required(ErrorMessage = "Phải nhập vào Mã Thẻ")]
        [DataMemberAttribute()]
        public string HICardNo
        {
            get
            {
                return _HICardNo;
            }
            set
            {
                if (_HICardNo != value)
                {
                    OnHICardNoChanging(value);
                    //ValidateProperty("HICardNo", value);
                    _HICardNo = value;
                    RaisePropertyChanged("HICardNo");
                    OnHICardNoChanged(); 
                }
            }
        }
        private String _HICardNo;
        partial void OnHICardNoChanging(String value);
        partial void OnHICardNoChanged();





        [DataMemberAttribute()]
        //[Required(ErrorMessage="Phải nhập vào nơi khám chữa bệnh ban đầu.")]
        //[StringLength(5, ErrorMessage = "Dữ liệu không hợp lệ",MinimumLength = 5)]
        public String RegistrationCode
        {
            get
            {
                return _RegistrationCode;
            }
            set
            {
                if (_RegistrationCode != value)
                {
                    OnRegistrationCodeChanging(value);
                    ValidateProperty("RegistrationCode", value);
                    _RegistrationCode = value;
                    RaisePropertyChanged("RegistrationCode");
                    OnRegistrationCodeChanged(); 
                }
            }
        }
        private String _RegistrationCode;
        partial void OnRegistrationCodeChanging(String value);
        partial void OnRegistrationCodeChanged();



        [DataMemberAttribute()]
        [Required(ErrorMessage = "Phải nhập vào Nơi cấp")]
        public String RegistrationLocation
        {
            get
            {
                return _RegistrationLocation;
            }
            set
            {
                if (_RegistrationLocation != value)
                {
                    OnRegistrationLocationChanging(value);
                    ValidateProperty("RegistrationLocation", value);
                    _RegistrationLocation = value;
                    RaisePropertyChanged("RegistrationLocation");
                    OnRegistrationLocationChanged(); 
                }
            }
        }
        private String _RegistrationLocation;
        partial void OnRegistrationLocationChanging(String value);
        partial void OnRegistrationLocationChanged();





        //[CustomValidation(typeof(HealthInsurance), "ValidateValidDateFrom")]
        [Required(ErrorMessage = "Nhập giá trị Ngày bắt đầu")]
        [DataMemberAttribute()]
        public Nullable<DateTime> ValidDateFrom
        {
            get
            {
                return _ValidDateFrom;
            }
            set
            {
                if (_ValidDateFrom != value)
                {
                    OnValidDateFromChanging(value);
                    //ValidateProperty("ValidDateFrom", value);
                    _ValidDateFrom = value;
                    RaisePropertyChanged("ValidDateFrom");
                    RaisePropertyChanged("IsValid");
                    OnValidDateFromChanged(); 
                }
            }
        }
        private Nullable<DateTime> _ValidDateFrom;
        partial void OnValidDateFromChanging(Nullable<DateTime> value);
        partial void OnValidDateFromChanged();




        //[CustomValidation(typeof(HealthInsurance), "ValidateValidDateTo")]
        [Required(ErrorMessage = "Nhập giá trị Ngày hết hạn")]
        [DataMemberAttribute()]
        public Nullable<DateTime> ValidDateTo
        {
            get
            {
                return _ValidDateTo;
            }
            set
            {
                if (_ValidDateTo != value)
                {
                    OnValidDateToChanging(value);
                    //ValidateProperty("ValidDateTo", value);
                    _ValidDateTo = value;
                    RaisePropertyChanged("ValidDateTo");
                    RaisePropertyChanged("IsValid");
                    OnValidDateToChanged(); 
                }
            }
        }
        private Nullable<DateTime> _ValidDateTo;
        partial void OnValidDateToChanging(Nullable<DateTime> value);
        partial void OnValidDateToChanged();

        private string _CheckedHICardValidResult;
        [DataMemberAttribute()]
        public string CheckedHICardValidResult
        {
            get
            {
                return _CheckedHICardValidResult;
            }
            set
            {
                _CheckedHICardValidResult = value;
                RaisePropertyChanged("CheckedHICardValidResult");
            }
        }
        #endregion

        #region Navigation Properties


        [DataMemberAttribute()]
        public ObservableCollection<HealthInsuranceHistory> HealthInsuranceHistories
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_HEALTHIN_REL_PTINF_PATIENTS", "Patients")]
        public Patient Patient
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_HI_REL_PTINF_HIPOLOCY", "HealthInsurancePolicy")]
        public HealthInsurancePolicy HealthInsurancePolicy
        {
            get;
            set;
        }

        private InsuranceBenefit _InsuranceBenefit;
        [DataMemberAttribute()]
        public InsuranceBenefit InsuranceBenefit
        {
            get
            {
                return _InsuranceBenefit;
            }
            set
            {
                _InsuranceBenefit = value;
                RaisePropertyChanged("InsuranceBenefit");
            }
        }


        #endregion

        /// <summary>
        /// xem la lấy thông tin lần đầu hay là đi xác nhận
        /// </summary>
        private bool _isDoing;
        [DataMemberAttribute()]
        public bool isDoing
        {
            get
            {
                return _isDoing;
            }
            set
            {
                if (_isDoing != value)
                {
                    _isDoing = value;
                    RaisePropertyChanged("isDoing");
                }
            }
        }

        private bool _EditLocked;
        [DataMemberAttribute()]
        public bool EditLocked
        {
            get
            {
                return _EditLocked;
            }
            set
            {
                if (_EditLocked != value)
                {
                    _EditLocked = value;
                    RaisePropertyChanged("EditLocked"); 
                }
            }
        }

        private bool _InvalidFlag;
        [DataMemberAttribute()]
        public bool InvalidFlag
        {
            get
            {
                return _InvalidFlag;
            }
            set
            {
                if (_InvalidFlag != value)
                {
                    _InvalidFlag = value;
                    RaisePropertyChanged("InvalidFlag");
                }
            }
        }
        private bool _MarkAsDeleted;
        [DataMemberAttribute()]
        public bool MarkAsDeleted
        {
            get
            {
                return _MarkAsDeleted;
            }
            set
            {
                if (_MarkAsDeleted != value)
                {
                    _MarkAsDeleted = value;
                    RaisePropertyChanged("MarkAsDeleted");
                }
            }
        }

        private Lookup _HICardType;
        [Required(ErrorMessage="Card Type is required")]
        [DataMemberAttribute()]
        public Lookup HICardType
        {
            get
            {
                return _HICardType;
            }
            set
            {
                if (_HICardType != value)
                {
                    _HICardType = value;
                    RaisePropertyChanged("HICardType"); 
                }
            }
        }
        private long _V_HICardType;
        [DataMemberAttribute()]
        public long V_HICardType
        {
            get
            {
                return _V_HICardType;
            }
            set
            {
                _V_HICardType = value;
                RaisePropertyChanged("V_HICardType");
            }
        }

        private string _CityProvinceName;
        [DataMemberAttribute()]
        public string CityProvinceName
        {
            get
            {
                return _CityProvinceName;
            }
            set
            {
                _CityProvinceName = value;
                RaisePropertyChanged("CityProvinceName");
            }
        }


        private string _ProvinceHICode;
        [DataMemberAttribute()]
        public string ProvinceHICode
        {
            get
            {
                return _ProvinceHICode;
            }
            set
            {
                _ProvinceHICode = value;
                RaisePropertyChanged("ProvinceHICode");
            }
        }

        private long _CityProvinceID_Address;
        [DataMemberAttribute()]
        public long CityProvinceID_Address
        {
            get
            {
                return _CityProvinceID_Address;
            }
            set
            {
                _CityProvinceID_Address = value;
                RaisePropertyChanged("CityProvinceID_Address");
            }
        }

        private long _SuburbNameID;
        [DataMemberAttribute()]
        public long SuburbNameID
        {
            get
            {
                return _SuburbNameID;
            }
            set
            {
                _SuburbNameID = value;
                RaisePropertyChanged("SuburbNameID");
            }
        }

        private string _PatientStreetAddress;
        [DataMemberAttribute()]
        public string PatientStreetAddress
        {
            get
            {
                return _PatientStreetAddress;
            }
            set
            {
                _PatientStreetAddress = value;
                RaisePropertyChanged("PatientStreetAddress");
            }
        }

        private long _KVCode;
        [DataMemberAttribute()]
        public long KVCode
        {
            get
            {
                return _KVCode;
            }
            set
            {
                _KVCode = value;
                RaisePropertyChanged("KVCode");
            }
        }


        private string _ArchiveNumber;
        [DataMemberAttribute()]
        public string ArchiveNumber
        {
            get
            {
                return _ArchiveNumber;
            }
            set
            {
                _ArchiveNumber = value;
                RaisePropertyChanged("ArchiveNumber");
            }
        }
        //▼====== #001
        private long _WardNameID;
        [DataMemberAttribute()]
        public long WardNameID
        {
            get
            {
                return _WardNameID;
            }
            set
            {
                _WardNameID = value;
                RaisePropertyChanged("WardNameID");
            }
        }

        private string _WardName;
        [DataMemberAttribute()]
        public string WardName
        {
            get
            {
                return _WardName;
            }
            set
            {
                _WardName = value;
                RaisePropertyChanged("WardName");
            }
        }
        //▲====== #001

        private long _IBeID;
        [DataMemberAttribute()]
        public long IBeID
        {
            get
            {
                return _IBeID;
            }
            set
            {
                _IBeID = value;
                RaisePropertyChanged("IBeID");
            }
        }

    }
}
