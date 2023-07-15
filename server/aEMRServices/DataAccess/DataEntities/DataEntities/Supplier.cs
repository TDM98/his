using System;
using System.Runtime.Serialization;
using eHCMS.Services.Core.Base;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class Supplier : NotifyChangedBase
    {
        #region Factory Method

     
        /// Create a new Supplier object.
     
        /// <param name="supplierID">Initial value of the SupplierID property.</param>
        /// <param name="supplierName">Initial value of the SupplierName property.</param>
        /// <param name="address">Initial value of the Address property.</param>
        /// <param name="cityStateZipCode">Initial value of the CityStateZipCode property.</param>
        /// <param name="telephoneNumber">Initial value of the TelephoneNumber property.</param>
        /// <param name="active">Initial value of the Active property.</param>
        public static Supplier CreateSupplier(long supplierID, String supplierName, String address, String cityStateZipCode, String telephoneNumber, Boolean active)
        {
            Supplier supplier = new Supplier();
            supplier.SupplierID = supplierID;
            supplier.SupplierName = supplierName;
            supplier.Address = address;
            supplier.CityStateZipCode = cityStateZipCode;
            supplier.TelephoneNumber = telephoneNumber;
            supplier.Active = active;
            return supplier;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public long SupplierID
        {
            get
            {
                return _SupplierID;
            }
            set
            {
                if (_SupplierID != value)
                {
                    OnSupplierIDChanging(value);
                    _SupplierID = value;
                    RaisePropertyChanged("SupplierID");
                    OnSupplierIDChanged();
                }
            }
        }
        private long _SupplierID;
        partial void OnSupplierIDChanging(long value);
        partial void OnSupplierIDChanged();

        [Required(ErrorMessage = "Bạn phải nhập tên NCC")]
        [StringLength(128, MinimumLength = 0, ErrorMessage = "Tên NCC phải quá dài")]
        [DataMemberAttribute()]
        public String SupplierName
        {
            get
            {
                return _SupplierName;
            }
            set
            {
                OnSupplierNameChanging(value);
                ValidateProperty("SupplierName", value);
                _SupplierName = value;
                RaisePropertyChanged("SupplierName");
                OnSupplierNameChanged();
            }
        }
        private String _SupplierName;
        partial void OnSupplierNameChanging(String value);
        partial void OnSupplierNameChanged();

        [Required(ErrorMessage = "Bạn phải nhập địa chỉ")]
        [StringLength(128, MinimumLength = 0, ErrorMessage = "Địa chỉ quá dài")]
        [DataMemberAttribute()]
        public String Address
        {
            get
            {
                return _Address;
            }
            set
            {
                OnAddressChanging(value);
                ValidateProperty("Address", value);
                _Address = value;
                RaisePropertyChanged("Address");
                OnAddressChanged();
            }
        }
        private String _Address;
        partial void OnAddressChanging(String value);
        partial void OnAddressChanged();

        [Required(ErrorMessage = "Bạn phải nhập Mã Tỉnh/TP ")]
        [StringLength(128, MinimumLength = 0, ErrorMessage = "Mã Tỉnh/TP quá dài")]
        [RegularExpression(@"^([\w-\.]+)$", ErrorMessage = "Mã Tỉnh/TP không hợp lệ")]
        [DataMemberAttribute()]
        public String CityStateZipCode
        {
            get
            {
                return _CityStateZipCode;
            }
            set
            {
                OnCityStateZipCodeChanging(value);
                ValidateProperty("CityStateZipCode", value);
                _CityStateZipCode = value;
                RaisePropertyChanged("CityStateZipCode");
                OnCityStateZipCodeChanged();
            }
        }
        private String _CityStateZipCode;
        partial void OnCityStateZipCodeChanging(String value);
        partial void OnCityStateZipCodeChanged();

        [DataMemberAttribute()]
        public String ContactPerson
        {
            get
            {
                return _ContactPerson;
            }
            set
            {
                OnContactPersonChanging(value);
                _ContactPerson = value;
                RaisePropertyChanged("ContactPerson");
                OnContactPersonChanged();
            }
        }
        private String _ContactPerson;
        partial void OnContactPersonChanging(String value);
        partial void OnContactPersonChanged();


        [DataMemberAttribute()]
        public long SupplierType
        {
            get
            {
                return _SupplierType;
            }
            set
            {
                OnSupplierTypeChanging(value);
                _SupplierType = value;
                RaisePropertyChanged("SupplierType");
                OnSupplierTypeChanged();
            }
        }
        private long _SupplierType;
        partial void OnSupplierTypeChanging(long value);
        partial void OnSupplierTypeChanged();

        [Required(ErrorMessage = "Bạn phải nhập số điện thoại")]
        [StringLength(128, MinimumLength = 0, ErrorMessage = "Số điện thoại quá dài")]
        // [RegularExpression(@"^\(\d{3}\)\d{3}-\d{4}$", ErrorMessage = "Telephone Number is InValid")]
        [DataMemberAttribute()]
        public String TelephoneNumber
        {
            get
            {
                return _TelephoneNumber;
            }
            set
            {
                OnTelephoneNumberChanging(value);
                ValidateProperty("TelephoneNumber", value);
                _TelephoneNumber = value;
                RaisePropertyChanged("TelephoneNumber");
                OnTelephoneNumberChanged();
            }
        }
        private String _TelephoneNumber;
        partial void OnTelephoneNumberChanging(String value);
        partial void OnTelephoneNumberChanged();

        [DataMemberAttribute()]
        public String FaxNumber
        {
            get
            {
                return _FaxNumber;
            }
            set
            {
                OnFaxNumberChanging(value);
                _FaxNumber = value;
                RaisePropertyChanged("FaxNumber");
                OnFaxNumberChanged();
            }
        }
        private String _FaxNumber;
        partial void OnFaxNumberChanging(String value);
        partial void OnFaxNumberChanged();

        [RegularExpression(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Email không hợp lệ")]
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
                ValidateProperty("PAHEmailAddress", value);
                _PAHEmailAddress = value;
                RaisePropertyChanged("PAHEmailAddress");
                OnPAHEmailAddressChanged();
            }
        }
        private String _PAHEmailAddress;
        partial void OnPAHEmailAddressChanging(String value);
        partial void OnPAHEmailAddressChanged();

        [DataMemberAttribute()]
        public String WebSiteAddress
        {
            get
            {
                return _WebSiteAddress;
            }
            set
            {
                OnWebSiteAddressChanging(value);
                _WebSiteAddress = value;
                RaisePropertyChanged("WebSiteAddress");
                OnWebSiteAddressChanged();
            }
        }
        private String _WebSiteAddress;
        partial void OnWebSiteAddressChanging(String value);
        partial void OnWebSiteAddressChanged();

        [DataMemberAttribute()]
        public String CertificateAgency
        {
            get
            {
                return _CertificateAgency;
            }
            set
            {
                OnCertificateAgencyChanging(value);
                _CertificateAgency = value;
                RaisePropertyChanged("CertificateAgency");
                OnCertificateAgencyChanged();
            }
        }
        private String _CertificateAgency;
        partial void OnCertificateAgencyChanging(String value);
        partial void OnCertificateAgencyChanged();

        [DataMemberAttribute()]
        public String SupplierDescription
        {
            get
            {
                return _SupplierDescription;
            }
            set
            {
                OnSupplierDescriptionChanging(value);
                _SupplierDescription = value;
                RaisePropertyChanged("SupplierDescription");
                OnSupplierDescriptionChanged();
            }
        }
        private String _SupplierDescription;
        partial void OnSupplierDescriptionChanging(String value);
        partial void OnSupplierDescriptionChanged();

        [DataMemberAttribute()]
        public Boolean Active
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
        private Boolean _Active;
        partial void OnActiveChanging(Boolean value);
        partial void OnActiveChanged();

        [DataMemberAttribute()]
        public String AccountNumber
        {
            get
            {
                return _AccountNumber;
            }
            set
            {
                _AccountNumber = value;
                RaisePropertyChanged("AccountNumber");
            }
        }
        private String _AccountNumber;

        [DataMemberAttribute()]
        public String BankName
        {
            get
            {
                return _BankName;
            }
            set
            {
                _BankName = value;
                RaisePropertyChanged("BankName");
            }
        }
        private String _BankName;

        [DataMemberAttribute()]
        public String SupplierCode
        {
            get
            {
                return _SupplierCode;
            }
            set
            {
                _SupplierCode = value;
                RaisePropertyChanged("SupplierCode");
            }
        }
        private String _SupplierCode;

        [DataMemberAttribute()]
        public Lookup VSupplierType
        {
            get
            {
                return _VSupplierType;
            }
            set
            {
                OnVSupplierTypeChanging(value);
                _VSupplierType = value;
                RaisePropertyChanged("VSupplierType");
                OnVSupplierTypeChanged();
            }
        }
        private Lookup _VSupplierType;
        partial void OnVSupplierTypeChanging(Lookup value);
        partial void OnVSupplierTypeChanged();

        [DataMemberAttribute()]
        public byte SupplierDrugDeptPharmOthers
        {
            get
            {
                return _SupplierDrugDeptPharmOthers;
            }
            set
            {
                _SupplierDrugDeptPharmOthers = value;
                RaisePropertyChanged("SupplierDrugDeptPharmOthers");
            }
        }
        private byte _SupplierDrugDeptPharmOthers;

        private String _SupplierDrugDeptPharmOthersName;
        [DataMemberAttribute()]
        public String SupplierDrugDeptPharmOthersName
        {
            get
            {
                return _SupplierDrugDeptPharmOthersName;
            }
            set
            {
                _SupplierDrugDeptPharmOthersName = value;
                RaisePropertyChanged("SupplierDrugDeptPharmOthersName");
            }
        }

        #endregion

        public override bool Equals(object obj)
        {
            Supplier seletedSupplier = obj as Supplier;
            if (seletedSupplier == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.SupplierID == seletedSupplier.SupplierID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #region Extention member

        [DataMemberAttribute()]
        public String ListPCOID
        {
            get
            {
                return _ListPCOID;
            }
            set
            {
                _ListPCOID = value;
                RaisePropertyChanged("ListPCOID");
            }
        }
        private String _ListPCOID;

        #endregion

    }
}
