using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class DrugDeptPharmaceuticalCompany : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new DrugDeptPharmaceuticalCompany object.

        /// <param name="pCOID">Initial value of the PCOID property.</param>
        /// <param name="pCOName">Initial value of the PCOName property.</param>
        public static DrugDeptPharmaceuticalCompany CreateDrugDeptPharmaceuticalCompany(long pCOID, String pCOName)
        {
            DrugDeptPharmaceuticalCompany DrugDeptPharmaceuticalCompany = new DrugDeptPharmaceuticalCompany();
            DrugDeptPharmaceuticalCompany.PCOID = pCOID;
            DrugDeptPharmaceuticalCompany.PCOName = pCOName;
            return DrugDeptPharmaceuticalCompany;
        }

        #endregion
        #region Primitive Properties
        [DataMemberAttribute()]
        public long PCOID
        {
            get
            {
                return _PCOID;
            }
            set
            {
                if (_PCOID != value)
                {
                    OnPCOIDChanging(value);
                    ////ReportPropertyChanging("PCOID");
                    _PCOID = value;
                    RaisePropertyChanged("PCOID");
                    OnPCOIDChanged();
                }
            }
        }
        private long _PCOID;
        partial void OnPCOIDChanging(long value);
        partial void OnPCOIDChanged();

        [DataMemberAttribute()]
        [Required(ErrorMessage = "Bạn phải nhập Tên NSX!")]
        public String PCOName
        {
            get
            {
                return _PCOName;
            }
            set
            {
                OnPCONameChanging(value);
                ValidateProperty("PCOName",value);
                _PCOName = value;
                RaisePropertyChanged("PCOName");
                OnPCONameChanged();
            }
        }
        private String _PCOName;
        partial void OnPCONameChanging(String value);
        partial void OnPCONameChanged();


        [DataMemberAttribute()]
        //[Required(ErrorMessage = "Bạn phải nhập Địa Chỉ!")]
        public String PCOAddress
        {
            get
            {
                return _PCOAddress;
            }
            set
            {
                OnPCOAddressChanging(value);
                //ValidateProperty("PCOAddress", value);
                _PCOAddress = value;
                RaisePropertyChanged("PCOAddress");
                OnPCOAddressChanged();
            }
        }
        private String _PCOAddress;
        partial void OnPCOAddressChanging(String value);
        partial void OnPCOAddressChanged();

        [DataMemberAttribute()]
        public String PCOState
        {
            get
            {
                return _PCOState;
            }
            set
            {
                OnPCOStateChanging(value);
                _PCOState = value;
                RaisePropertyChanged("PCOState");
                OnPCOStateChanged();
            }
        }
        private String _PCOState;
        partial void OnPCOStateChanging(String value);
        partial void OnPCOStateChanged();

        [DataMemberAttribute()]
        public String PCOZipCode
        {
            get
            {
                return _PCOZipCode;
            }
            set
            {
                OnPCOZipCodeChanging(value);
                _PCOZipCode = value;
                RaisePropertyChanged("PCOZipCode");
                OnPCOZipCodeChanged();
            }
        }
        private String _PCOZipCode;
        partial void OnPCOZipCodeChanging(String value);
        partial void OnPCOZipCodeChanged();

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
        public string CountryName
        {
            get
            {
                return _CountryName;
            }
            set
            {
                _CountryName = value;
                RaisePropertyChanged("CountryName");
            }
        }
        private string _CountryName;

        [DataMemberAttribute()]
        public String PCOTelephone
        {
            get
            {
                return _PCOTelephone;
            }
            set
            {
                OnPCOTelephoneChanging(value);
                _PCOTelephone = value;
                RaisePropertyChanged("PCOTelephone");
                OnPCOTelephoneChanged();
            }
        }
        private String _PCOTelephone;
        partial void OnPCOTelephoneChanging(String value);
        partial void OnPCOTelephoneChanged();

        [DataMemberAttribute()]
        public String ContactName
        {
            get
            {
                return _ContactName;
            }
            set
            {
                OnContactNameChanging(value);
                _ContactName = value;
                RaisePropertyChanged("ContactName");
                OnContactNameChanged();
            }
        }
        private String _ContactName;
        partial void OnContactNameChanging(String value);
        partial void OnContactNameChanged();

        [DataMemberAttribute()]
        public Nullable<Boolean> Active
        {
            get
            {
                return _Active;
            }
            set
            {
                OnActiveChanging(value);
                _Active = value;
                RaisePropertyChanged("Active");
                OnActiveChanged();
            }
        }
        private Nullable<Boolean> _Active;
        partial void OnActiveChanging(Nullable<Boolean> value);
        partial void OnActiveChanged();

        #endregion
        public override bool Equals(object obj)
        {
            DrugDeptPharmaceuticalCompany seletedStore = obj as DrugDeptPharmaceuticalCompany;
            if (seletedStore == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PCOID == seletedStore.PCOID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
