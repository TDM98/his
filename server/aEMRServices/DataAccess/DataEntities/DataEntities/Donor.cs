using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class Donor : NotifyChangedBase, IEditableObject
    {
        public Donor()
            : base()
        {

        }

        private Donor _tempDonor;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempDonor = (Donor)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempDonor)
                CopyFrom(_tempDonor);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(Donor p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new Donor object.

        /// <param name="donorID">Initial value of the DonorID property.</param>
        /// <param name="gender">Initial value of the Gender property.</param>
        /// <param name="dOB">Initial value of the DOB property.</param>
        /// <param name="firstName">Initial value of the FirstName property.</param>
        /// <param name="lastName">Initial value of the LastName property.</param>
        /// <param name="fullName">Initial value of the FullName property.</param>
        public static Donor CreateDonor(long donorID, String gender, DateTime dOB, String firstName, String lastName, String fullName)
        {
            Donor donor = new Donor();
            donor.DonorID = donorID;
            donor.Gender = gender;
            donor.DOB = dOB;
            donor.FirstName = firstName;
            donor.LastName = lastName;
            donor.FullName = fullName;
            return donor;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long DonorID
        {
            get
            {
                return _DonorID;
            }
            set
            {
                if (_DonorID != value)
                {
                    OnDonorIDChanging(value);
                    ////ReportPropertyChanging("DonorID");
                    _DonorID = value;
                    RaisePropertyChanged("DonorID");
                    OnDonorIDChanged();
                }
            }
        }
        private long _DonorID;
        partial void OnDonorIDChanging(long value);
        partial void OnDonorIDChanged();





        [DataMemberAttribute()]
        public Nullable<Byte> BloodTypeID
        {
            get
            {
                return _BloodTypeID;
            }
            set
            {
                OnBloodTypeIDChanging(value);
                ////ReportPropertyChanging("BloodTypeID");
                _BloodTypeID = value;
                RaisePropertyChanged("BloodTypeID");
                OnBloodTypeIDChanged();
            }
        }
        private Nullable<Byte> _BloodTypeID;
        partial void OnBloodTypeIDChanging(Nullable<Byte> value);
        partial void OnBloodTypeIDChanged();





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
                ////ReportPropertyChanging("CountryID");
                _CountryID = value;
                RaisePropertyChanged("CountryID");
                OnCountryIDChanged();
            }
        }
        private Nullable<long> _CountryID;
        partial void OnCountryIDChanging(Nullable<long> value);
        partial void OnCountryIDChanged();





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
        public String Gender
        {
            get
            {
                return _Gender;
            }
            set
            {
                OnGenderChanging(value);
                ////ReportPropertyChanging("Gender");
                _Gender = value;
                RaisePropertyChanged("Gender");
                OnGenderChanged();
            }
        }
        private String _Gender;
        partial void OnGenderChanging(String value);
        partial void OnGenderChanged();





        [DataMemberAttribute()]
        public DateTime DOB
        {
            get
            {
                return _DOB;
            }
            set
            {
                OnDOBChanging(value);
                ////ReportPropertyChanging("DOB");
                _DOB = value;
                RaisePropertyChanged("DOB");
                OnDOBChanged();
            }
        }
        private DateTime _DOB;
        partial void OnDOBChanging(DateTime value);
        partial void OnDOBChanged();





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
                ////ReportPropertyChanging("FirstName");
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
                ////ReportPropertyChanging("MiddleName");
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
                ////ReportPropertyChanging("LastName");
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
                ////ReportPropertyChanging("FullName");
                _FullName = value;
                RaisePropertyChanged("FullName");
                OnFullNameChanged();
            }
        }
        private String _FullName;
        partial void OnFullNameChanging(String value);
        partial void OnFullNameChanged();





        [DataMemberAttribute()]
        public String HomePhone
        {
            get
            {
                return _HomePhone;
            }
            set
            {
                OnHomePhoneChanging(value);
                ////ReportPropertyChanging("HomePhone");
                _HomePhone = value;
                RaisePropertyChanged("HomePhone");
                OnHomePhoneChanged();
            }
        }
        private String _HomePhone;
        partial void OnHomePhoneChanging(String value);
        partial void OnHomePhoneChanged();





        [DataMemberAttribute()]
        public String CellPhone
        {
            get
            {
                return _CellPhone;
            }
            set
            {
                OnCellPhoneChanging(value);
                ////ReportPropertyChanging("CellPhone");
                _CellPhone = value;
                RaisePropertyChanged("CellPhone");
                OnCellPhoneChanged();
            }
        }
        private String _CellPhone;
        partial void OnCellPhoneChanging(String value);
        partial void OnCellPhoneChanged();





        [DataMemberAttribute()]
        public String WorkPhone
        {
            get
            {
                return _WorkPhone;
            }
            set
            {
                OnWorkPhoneChanging(value);
                ////ReportPropertyChanging("WorkPhone");
                _WorkPhone = value;
                RaisePropertyChanged("WorkPhone");
                OnWorkPhoneChanged();
            }
        }
        private String _WorkPhone;
        partial void OnWorkPhoneChanging(String value);
        partial void OnWorkPhoneChanged();





        [DataMemberAttribute()]
        public String MedicalCondition
        {
            get
            {
                return _MedicalCondition;
            }
            set
            {
                OnMedicalConditionChanging(value);
                ////ReportPropertyChanging("MedicalCondition");
                _MedicalCondition = value;
                RaisePropertyChanged("MedicalCondition");
                OnMedicalConditionChanged();
            }
        }
        private String _MedicalCondition;
        partial void OnMedicalConditionChanging(String value);
        partial void OnMedicalConditionChanged();





        [DataMemberAttribute()]
        public Nullable<Int64> IsPatient
        {
            get
            {
                return _IsPatient;
            }
            set
            {
                OnIsPatientChanging(value);
                ////ReportPropertyChanging("IsPatient");
                _IsPatient = value;
                RaisePropertyChanged("IsPatient");
                OnIsPatientChanged();
            }
        }
        private Nullable<Int64> _IsPatient;
        partial void OnIsPatientChanging(Nullable<Int64> value);
        partial void OnIsPatientChanged();





        [DataMemberAttribute()]
        public Nullable<Int64> IsStaff
        {
            get
            {
                return _IsStaff;
            }
            set
            {
                OnIsStaffChanging(value);
                ////ReportPropertyChanging("IsStaff");
                _IsStaff = value;
                RaisePropertyChanged("IsStaff");
                OnIsStaffChanged();
            }
        }
        private Nullable<Int64> _IsStaff;
        partial void OnIsStaffChanging(Nullable<Int64> value);
        partial void OnIsStaffChanged();

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_BLOODDON_REL_BB04_DONORS", "BloodDonations")]
        public ObservableCollection<BloodDonation> BloodDonations
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_DONORS_REL_BB06_BLOODTYP", "BloodTypes")]
        public BloodType BloodType
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_DONORS_REL_BB13_CITIESPR", "CitiesProvinces")]
        public CitiesProvince CitiesProvince
        {
            get;
            set;
        }



        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_DONORS_REL_BB12_REFCOUNT", "RefCountries")]
        public RefCountry RefCountry
        {
            get;
            set;
        }


        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_DONORSME_REL_BB11_DONORS", "DonorsMedicalConditions")]
        public ObservableCollection<DonorsMedicalCondition> DonorsMedicalConditions
        {
            get;
            set;
        }

        #endregion
    }
}
