using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{

    public partial class RefCountry : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new RefCountry object.

        /// <param name="countryID">Initial value of the CountryID property.</param>
        /// <param name="countryName">Initial value of the CountryName property.</param>
        public static RefCountry CreateRefCountry(long countryID, String countryName)
        {
            RefCountry refCountry = new RefCountry();
            refCountry.CountryID = countryID;
            refCountry.CountryName = countryName;
            return refCountry;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long CountryID
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
                    ////ReportPropertyChanging("CountryID");
                    _CountryID = value;
                    RaisePropertyChanged("CountryID");
                    OnCountryIDChanged();
                }
            }
        }
        private long _CountryID;
        partial void OnCountryIDChanging(long value);
        partial void OnCountryIDChanged();





        [DataMemberAttribute()]
        public String CountryName
        {
            get
            {
                return _CountryName;
            }
            set
            {
                OnCountryNameChanging(value);
                ////ReportPropertyChanging("CountryName");
                _CountryName = value;
                RaisePropertyChanged("CountryName");
                OnCountryNameChanged();
            }
        }
        private String _CountryName;
        partial void OnCountryNameChanging(String value);
        partial void OnCountryNameChanged();





        [DataMemberAttribute()]
        public String CountryCode
        {
            get
            {
                return _CountryCode;
            }
            set
            {
                OnCountryCodeChanging(value);
                ////ReportPropertyChanging("CountryCode");
                _CountryCode = value;
                RaisePropertyChanged("CountryCode");
                OnCountryCodeChanged();
            }
        }
        private String _CountryCode;
        partial void OnCountryCodeChanging(String value);
        partial void OnCountryCodeChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_DONORS_REL_BB12_REFCOUNT", "Donors")]
        public ObservableCollection<Donor> Donors
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PATIENTS_REL_PTINF_REFCOUNT", "Patients")]
        public ObservableCollection<Patient> Patients
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_REFGENER_REL_DMGMT_REFCOUNT", "RefGenericDrugDetails")]
        public ObservableCollection<RefGenericDrugDetail> RefGenericDrugDetails
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_STAFFS_REL_HR09_REFCOUNT", "Staffs")]
        public ObservableCollection<Staff> Staffs
        {
            get;
            set;
        }

        #endregion
        public override bool Equals(object obj)
        {
            RefCountry seletedCountry = obj as RefCountry;
            if (seletedCountry == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.CountryID == seletedCountry.CountryID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
