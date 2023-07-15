using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class PatientAddressHistory : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new PatientAddressHistory object.

        /// <param name="ptAddressHisID">Initial value of the PtAddressHisID property.</param>
        /// <param name="patientID">Initial value of the PatientID property.</param>
        /// <param name="modifiedDate">Initial value of the ModifiedDate property.</param>
        /// <param name="pAHStreetAddress">Initial value of the PAHStreetAddress property.</param>
        public static PatientAddressHistory CreatePatientAddressHistory(long ptAddressHisID, long patientID, DateTime modifiedDate, String pAHStreetAddress)
        {
            PatientAddressHistory patientAddressHistory = new PatientAddressHistory();
            patientAddressHistory.PtAddressHisID = ptAddressHisID;
            patientAddressHistory.PatientID = patientID;
            patientAddressHistory.ModifiedDate = modifiedDate;
            patientAddressHistory.PAHStreetAddress = pAHStreetAddress;
            return patientAddressHistory;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long PtAddressHisID
        {
            get
            {
                return _PtAddressHisID;
            }
            set
            {
                if (_PtAddressHisID != value)
                {
                    OnPtAddressHisIDChanging(value);
                    ////ReportPropertyChanging("PtAddressHisID");
                    _PtAddressHisID = value;
                    RaisePropertyChanged("PtAddressHisID");
                    OnPtAddressHisIDChanged();
                }
            }
        }
        private long _PtAddressHisID;
        partial void OnPtAddressHisIDChanging(long value);
        partial void OnPtAddressHisIDChanged();





        [DataMemberAttribute()]
        public long PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                OnPatientIDChanging(value);
                ////ReportPropertyChanging("PatientID");
                _PatientID = value;
                RaisePropertyChanged("PatientID");
                OnPatientIDChanged();
            }
        }
        private long _PatientID;
        partial void OnPatientIDChanging(long value);
        partial void OnPatientIDChanged();





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
        public DateTime ModifiedDate
        {
            get
            {
                return _ModifiedDate;
            }
            set
            {
                OnModifiedDateChanging(value);
                ////ReportPropertyChanging("ModifiedDate");
                _ModifiedDate = value;
                RaisePropertyChanged("ModifiedDate");
                OnModifiedDateChanged();
            }
        }
        private DateTime _ModifiedDate;
        partial void OnModifiedDateChanging(DateTime value);
        partial void OnModifiedDateChanged();





        [DataMemberAttribute()]
        public String PAHStreetAddress
        {
            get
            {
                return _PAHStreetAddress;
            }
            set
            {
                OnPAHStreetAddressChanging(value);
                ////ReportPropertyChanging("PAHStreetAddress");
                _PAHStreetAddress = value;
                RaisePropertyChanged("PAHStreetAddress");
                OnPAHStreetAddressChanged();
            }
        }
        private String _PAHStreetAddress;
        partial void OnPAHStreetAddressChanging(String value);
        partial void OnPAHStreetAddressChanged();





        [DataMemberAttribute()]
        public String PAHSurburb
        {
            get
            {
                return _PAHSurburb;
            }
            set
            {
                OnPAHSurburbChanging(value);
                ////ReportPropertyChanging("PAHSurburb");
                _PAHSurburb = value;
                RaisePropertyChanged("PAHSurburb");
                OnPAHSurburbChanged();
            }
        }
        private String _PAHSurburb;
        partial void OnPAHSurburbChanging(String value);
        partial void OnPAHSurburbChanged();





        [DataMemberAttribute()]
        public String PAHPhoneNumber
        {
            get
            {
                return _PAHPhoneNumber;
            }
            set
            {
                OnPAHPhoneNumberChanging(value);
                ////ReportPropertyChanging("PAHPhoneNumber");
                _PAHPhoneNumber = value;
                RaisePropertyChanged("PAHPhoneNumber");
                OnPAHPhoneNumberChanged();
            }
        }
        private String _PAHPhoneNumber;
        partial void OnPAHPhoneNumberChanging(String value);
        partial void OnPAHPhoneNumberChanged();





        [DataMemberAttribute()]
        public String PAHCellPhoneNumber
        {
            get
            {
                return _PAHCellPhoneNumber;
            }
            set
            {
                OnPAHCellPhoneNumberChanging(value);
                ////ReportPropertyChanging("PAHCellPhoneNumber");
                _PAHCellPhoneNumber = value;
                RaisePropertyChanged("PAHCellPhoneNumber");
                OnPAHCellPhoneNumberChanged();
            }
        }
        private String _PAHCellPhoneNumber;
        partial void OnPAHCellPhoneNumberChanging(String value);
        partial void OnPAHCellPhoneNumberChanged();





        [DataMemberAttribute()]
        public String PAHEmailAddress
        {
            get
            {
                return _PAHEmailAddress;
            }
            set
            {
                OnPAHEmailAddressChanging(value);
                ////ReportPropertyChanging("PAHEmailAddress");
                _PAHEmailAddress = value;
                RaisePropertyChanged("PAHEmailAddress");
                OnPAHEmailAddressChanged();
            }
        }
        private String _PAHEmailAddress;
        partial void OnPAHEmailAddressChanging(String value);
        partial void OnPAHEmailAddressChanged();





        [DataMemberAttribute()]
        public String PAHEmployer
        {
            get
            {
                return _PAHEmployer;
            }
            set
            {
                OnPAHEmployerChanging(value);
                ////ReportPropertyChanging("PAHEmployer");
                _PAHEmployer = value;
                RaisePropertyChanged("PAHEmployer");
                OnPAHEmployerChanged();
            }
        }
        private String _PAHEmployer;
        partial void OnPAHEmployerChanging(String value);
        partial void OnPAHEmployerChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PATIENTA_REL_PTINF_CITIESPR", "CitiesProvinces")]
        public CitiesProvince CitiesProvince
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PATIENTA_REL_PTINF_PATIENTS", "Patients")]
        public Patient Patient
        {
            get;
            set;
        }

        #endregion
    }
}
