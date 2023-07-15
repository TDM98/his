using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class HospitalSearchCriteria : SearchCriteriaBase
    {
        public HospitalSearchCriteria()
            : base()
        {

        }
        
        #region Primitive Properties


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
        public bool IsSearchAll
        {
            get
            {
                return _IsSearchAll;
            }
            set
            {
                OnIsSearchAllChanging(value);
                _IsSearchAll = value;
                RaisePropertyChanged("IsSearchAll");
                OnIsSearchAllChanged();
            }
        }
        private bool _IsSearchAll;
        partial void OnIsSearchAllChanging(bool value);
        partial void OnIsSearchAllChanged();


        [DataMemberAttribute()]
        public long V_HospitalType
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
        private long _V_HospitalType;
        partial void OnV_HospitalTypeChanging(long value);
        partial void OnV_HospitalTypeChanged();

        
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
        public bool IsPaperReferal
        {
            get
            {
                return _IsPaperReferal;
            }
            set
            {
                OnIsPaperReferalChanging(value);
                ////ReportPropertyChanging("IsPaperReferal");
                _IsPaperReferal = value;
                RaisePropertyChanged("IsPaperReferal");
                OnIsPaperReferalChanged();
            }
        }
        private bool _IsPaperReferal;
        partial void OnIsPaperReferalChanging(bool value);
        partial void OnIsPaperReferalChanged();

        #endregion


    }
}
