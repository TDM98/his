using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class Hospital : NotifyChangedBase, IEditableObject
    {
        public Hospital()
            : base()
        {

        }

        private Hospital _tempHospital;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempHospital = (Hospital)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempHospital)
                CopyFrom(_tempHospital);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(Hospital p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new Hospital object.

        /// <param name="hosID">Initial value of the HosID property.</param>
        /// <param name="hosName">Initial value of the HosName property.</param>
        /// <param name="hosAddress">Initial value of the HosAddress property.</param>
        public static Hospital CreateHospital(long hosID, String hosName, String hosAddress)
        {
            Hospital hospital = new Hospital();
            hospital.HosID = hosID;
            hospital.HosName = hosName;
            hospital.HosAddress = hosAddress;
            return hospital;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public bool ThongTuyen
        {
            get
            {
                return _ThongTuyen;
            }
            set
            {
                if (_ThongTuyen != value)
                {
                    _ThongTuyen = value;
                    RaisePropertyChanged("ThongTuyen");
                }
            }
        }
        private bool _ThongTuyen;

        [DataMemberAttribute()]
        public long HosID
        {
            get
            {
                return _HosID;
            }
            set
            {
                if (_HosID != value)
                {
                    OnHosIDChanging(value);
                    ////ReportPropertyChanging("HosID");
                    _HosID = value;
                    RaisePropertyChanged("HosID");
                    OnHosIDChanged();
                }
            }
        }
        private long _HosID;
        partial void OnHosIDChanging(long value);
        partial void OnHosIDChanged();


        private string _CityProvinceName;
         [DataMemberAttribute()]
        public  string CityProvinceName
        {
            get
            { return _CityProvinceName; }
            set
            {
                _CityProvinceName = value;
                RaisePropertyChanged("CityProvinceName");
            }
        }

         private string _CityProvinceHICode;
         [DataMemberAttribute()]
         public string CityProvinceHICode
         {
             get
             { return _CityProvinceHICode; }
             set
             {
                 if (_CityProvinceHICode != value)
                 {
                     _CityProvinceHICode = value;
                     RaisePropertyChanged("CityProvinceHICode");
                 }
             }
         }


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
                ////ReportPropertyChanging("CityProvinceID");
                _CityProvinceID = value;
                RaisePropertyChanged("CityProvinceID");
                OnCityProvinceIDChanged();
            }
        }
        private Nullable<long> _CityProvinceID;
        partial void OnCityProvinceIDChanging(Nullable<long> value);
        partial void OnCityProvinceIDChanged();

        [DataMemberAttribute()]
        public String HospitalCode
        {
            get
            {
                return _HospitalCode;
            }
            set
            {
                OnHospitalCodeChanging(value);
                ////ReportPropertyChanging("HospitalCode");
                _HospitalCode = value;
                RaisePropertyChanged("HospitalCode");
                OnHospitalCodeChanged();
            }
        }
        private String _HospitalCode;
        partial void OnHospitalCodeChanging(String value);
        partial void OnHospitalCodeChanged();

        [DataMemberAttribute()]
        public String HosShortName
        {
            get
            {
                return _HosShortName;
            }
            set
            {
                OnHosShortNameChanging(value);
                ////ReportPropertyChanging("HosShortName");
                _HosShortName = value;
                RaisePropertyChanged("HosShortName");
                OnHosShortNameChanged();
            }
        }
        private String _HosShortName;
        partial void OnHosShortNameChanging(String value);
        partial void OnHosShortNameChanged();

        [DataMemberAttribute()]
        public String HosName
        {
            get
            {
                return _HosName;
            }
            set
            {
                OnHosNameChanging(value);
                ////ReportPropertyChanging("HosName");
                _HosName = value;
                RaisePropertyChanged("HosName");
                OnHosNameChanged();
            }
        }
        private String _HosName;
        partial void OnHosNameChanging(String value);
        partial void OnHosNameChanged();


        [DataMemberAttribute()]
        public String HosAddress
        {
            get
            {
                return _HosAddress;
            }
            set
            {
                OnHosAddressChanging(value);
                ////ReportPropertyChanging("HosAddress");
                _HosAddress = value;
                RaisePropertyChanged("HosAddress");
                OnHosAddressChanged();
            }
        }
        private String _HosAddress;
        partial void OnHosAddressChanging(String value);
        partial void OnHosAddressChanged();


        [DataMemberAttribute()]
        public String HosLogoImgPath
        {
            get
            {
                return _HosLogoImgPath;
            }
            set
            {
                OnHosLogoImgPathChanging(value);
                ////ReportPropertyChanging("HosLogoImgPath");
                _HosLogoImgPath = value;
                RaisePropertyChanged("HosLogoImgPath");
                OnHosLogoImgPathChanged();
            }
        }
        private String _HosLogoImgPath;
        partial void OnHosLogoImgPathChanging(String value);
        partial void OnHosLogoImgPathChanged();


        [DataMemberAttribute()]
        public String Slogan
        {
            get
            {
                return _Slogan;
            }
            set
            {
                OnSloganChanging(value);
                ////ReportPropertyChanging("Slogan");
                _Slogan = value;
                RaisePropertyChanged("Slogan");
                OnSloganChanged();
            }
        }
        private String _Slogan;
        partial void OnSloganChanging(String value);
        partial void OnSloganChanged();


        [DataMemberAttribute()]
        public String HosPhone
        {
            get
            {
                return _HosPhone;
            }
            set
            {
                OnHosPhoneChanging(value);
                ////ReportPropertyChanging("HosPhone");
                _HosPhone = value;
                RaisePropertyChanged("HosPhone");
                OnHosPhoneChanged();
            }
        }
        private String _HosPhone;
        partial void OnHosPhoneChanging(String value);
        partial void OnHosPhoneChanged();


        [DataMemberAttribute()]
        public String HosWebSite
        {
            get
            {
                return _HosWebSite;
            }
            set
            {
                OnHosWebSiteChanging(value);
                ////ReportPropertyChanging("HosWebSite");
                _HosWebSite = value;
                RaisePropertyChanged("HosWebSite");
                OnHosWebSiteChanged();
            }
        }
        private String _HosWebSite;
        partial void OnHosWebSiteChanging(String value);
        partial void OnHosWebSiteChanged();


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
                ////ReportPropertyChanging("V_HospitalType");
                _V_HospitalType = value;
                RaisePropertyChanged("V_HospitalType");
                OnV_HospitalTypeChanged();
            }
        }
        private Nullable<Int64> _V_HospitalType;
        partial void OnV_HospitalTypeChanging(Nullable<Int64> value);
        partial void OnV_HospitalTypeChanged();

        [DataMemberAttribute()]
        public String V_HospitalTypeString
        {
            get
            {
                return _V_HospitalTypeString;
            }
            set
            {
                OnV_HospitalTypeStringChanging(value);
                ////ReportPropertyChanging("V_HospitalType");
                _V_HospitalTypeString = value;
                RaisePropertyChanged("V_HospitalTypeString");
                OnV_HospitalTypeStringChanged();
            }
        }
        private String _V_HospitalTypeString;
        partial void OnV_HospitalTypeStringChanging(String value);
        partial void OnV_HospitalTypeStringChanged();

        //[StringLength(6,MinimumLength=5, ErrorMessage="Mã Bệnh Viện gồm 5 kí tự số")]
        [DataMemberAttribute()]
        public String HICode
        {
            get
            {
                return _HICode;
            }
            set
            {
                OnHICodeChanging(value);
                ////ReportPropertyChanging("HICode");
                _HICode = value;
                RaisePropertyChanged("HICode");
                OnHICodeChanged();
            }
        }
        private String _HICode;
        partial void OnHICodeChanging(String value);
        partial void OnHICodeChanged();

        
        [DataMemberAttribute()]
        public bool UsedForPaperReferralOnly
        {
            get
            {
                return _UsedForPaperReferralOnly;
            }
            set
            {
                OnUsedForPaperReferralOnlyChanging(value);
                ////ReportPropertyChanging("UsedForPaperReferralOnly");
                _UsedForPaperReferralOnly = value;
                RaisePropertyChanged("UsedForPaperReferralOnly");
                OnUsedForPaperReferralOnlyChanged();
            }
        }
        private bool _UsedForPaperReferralOnly;
        partial void OnUsedForPaperReferralOnlyChanging(bool value);
        partial void OnUsedForPaperReferralOnlyChanged();

        [DataMemberAttribute()]
        public bool IsFriends
        {
            get
            {
                return _IsFriends;
            }
            set
            {
                OnIsFriendsChanging(value);
                _IsFriends = value;
                RaisePropertyChanged("IsFriends");
                OnIsFriendsChanged();
            }
        }
        private bool _IsFriends;
        partial void OnIsFriendsChanging(bool value);
        partial void OnIsFriendsChanged();

        
        [DataMemberAttribute]
        public DateTime DateModified
        {
            get
            {
                return _DateModified;
            }
            set
            {
                if (_DateModified != value)
                {
                    OnDateModifiedChanging(value);
                    ////ReportPropertyChanging("IBID");
                    _DateModified = value;
                    RaisePropertyChanged("DateModified");
                    OnDateModifiedChanged();
                }
            }
        }
        private DateTime _DateModified;
        partial void OnDateModifiedChanging(DateTime value);
        partial void OnDateModifiedChanged();


        
        [DataMemberAttribute]
        public String ModifiedLog
        {
            get
            {
                return _ModifiedLog;
            }
            set
            {
                if (_ModifiedLog != value)
                {
                    OnModifiedLogChanging(value);
                    ////ReportPropertyChanging("IBID");
                    _ModifiedLog = value;
                    RaisePropertyChanged("ModifiedLog");
                    OnModifiedLogChanged();
                }
            }
        }
        private String _ModifiedLog;
        partial void OnModifiedLogChanging(String value);
        partial void OnModifiedLogChanged();
        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_HOSPITAL_REL_DM03_CITIESPR", "CitiesProvinces")]
        public CitiesProvince CitiesProvince
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_HOSPITAL_REL_PCMD2_HOSPITAL", "HospitalizationHistory")]
        public ObservableCollection<HospitalizationHistory> HospitalizationHistories
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_HOSZ_REL_PCMD2_HOS", "HospitalizationHistory")]
        public ObservableCollection<HospitalizationHistory> HospitalizationHistories1
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_HOSPITAL_REL_DM02_HOSPITAL", "HospitalSpecialists")]
        public ObservableCollection<HospitalSpecialist> HospitalSpecialists
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_TESTINGA_REL_DM01_HOSPITAL", "TestingAgency")]
        public ObservableCollection<TestingAgency> TestingAgencies
        {
            get;
            set;
        }

        #endregion

        public override bool Equals(object obj)
        {
            Hospital seletedUnit = obj as Hospital;
            if (seletedUnit == null)
                return false;

            //if (Object.ReferenceEquals(this, obj))
            //    return true;
            return this.HosID + this.HosName == seletedUnit.HosID + seletedUnit.HosName;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private bool _IsNoneHICodeOrig;
        public bool IsNoneHICodeOrig
        {
            get
            {
                return _IsNoneHICodeOrig;
            }
            set
            {
                _IsNoneHICodeOrig = value;
            }
        }
        public string V_HospitalClassString { get; set; }
        public long V_HospitalClass { get; set; }
        public bool IsUsed { get; set; }
        public string LeaderPhone { get; set; }
    }
}
