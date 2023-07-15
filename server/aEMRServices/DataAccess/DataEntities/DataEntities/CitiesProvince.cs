using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class CitiesProvince : NotifyChangedBase, IEditableObject
    {
        public CitiesProvince()
            : base()
        {

        }

        private CitiesProvince _tempCitiesProvince;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempCitiesProvince = (CitiesProvince)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempCitiesProvince)
                CopyFrom(_tempCitiesProvince);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(CitiesProvince p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }
        public override bool Equals(object obj)
        {
            CitiesProvince info = obj as CitiesProvince;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.CityProvinceID > 0 && this.CityProvinceID == info.CityProvinceID;
        }
        #endregion
        #region Factory Method


        /// Create a new CitiesProvince object.

        /// <param name="cityProvinceID">Initial value of the CityProvinceID property.</param>
        /// <param name="cityProvinceName">Initial value of the CityProvinceName property.</param>
        /// <param name="cityProviceCode">Initial value of the CityProviceCode property.</param>
        public static CitiesProvince CreateCitiesProvince(long cityProvinceID, String cityProvinceName, String cityProviceCode)
        {
            CitiesProvince citiesProvince = new CitiesProvince();
            citiesProvince.CityProvinceID = cityProvinceID;
            citiesProvince.CityProvinceName = cityProvinceName;
            citiesProvince.CityProviceCode = cityProviceCode;
            return citiesProvince;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long CityProvinceID
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
                    _CityProvinceID = value;
                    RaisePropertyChanged("CityProvinceID");
                    OnCityProvinceIDChanged();
                }
            }
        }
        private long _CityProvinceID;
        partial void OnCityProvinceIDChanging(long value);
        partial void OnCityProvinceIDChanged();
        [DataMemberAttribute()]
        public String CityProvinceName
        {
            get
            {
                return _CityProvinceName;
            }
            set
            {
                OnCityProvinceNameChanging(value);
                _CityProvinceName = value;
                RaisePropertyChanged("CityProvinceName");
                OnCityProvinceNameChanged();
            }
        }
        private String _CityProvinceName;
        partial void OnCityProvinceNameChanging(String value);
        partial void OnCityProvinceNameChanged();

        [DataMemberAttribute()]
        public String CityProviceCode
        {
            get
            {
                return _CityProviceCode;
            }
            set
            {
                OnCityProviceCodeChanging(value);
                _CityProviceCode = value;
                RaisePropertyChanged("CityProviceCode");
                OnCityProviceCodeChanged();
            }
        }
        private String _CityProviceCode;
        partial void OnCityProviceCodeChanging(String value);
        partial void OnCityProviceCodeChanged();

        [DataMemberAttribute()]
        public String CityProviceHICode
        {
            get
            {
                return _CityProviceHICode;
            }
            set
            {
                OnCityProviceHICodeChanging(value);
                _CityProviceHICode = value;
                RaisePropertyChanged("CityProviceHICode");
                OnCityProviceHICodeChanged();
            }
        }
        private String _CityProviceHICode;
        partial void OnCityProviceHICodeChanging(String value);
        partial void OnCityProviceHICodeChanged();

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
        public ObservableCollection<Donor> Donors
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
        public ObservableCollection<Hospital> Hospitals
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<PatientAddressHistory> PatientAddressHistories
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<Patient> Patients
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<Staff> Staffs
        {
            get;
            set;
        }

        #endregion

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
