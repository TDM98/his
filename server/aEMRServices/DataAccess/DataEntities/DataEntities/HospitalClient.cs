using eHCMS.Services.Core.Base;
using System;
using System.Runtime.Serialization;

namespace DataEntities
{
    public class HospitalClient : NotifyChangedBase
    {
        private long _HosClientID;
        private string _HosClientCode;
        private string _ClientName;
        private string _CompanyName;
        private string _TaxNumber;
        private string _Address;
        private string _CityStateZipCode;
        private string _ContactPerson;
        private string _TelephoneNumber;
        private string _FaxNumber;
        private string _EmailAddress;
        private string _WebSite;
        private string _ClientDescription;
        private bool _IsActive;
        private long _V_HosClientType;
        private string _AccountNumber;
        private string _BankName;
        private string _BranchName;
        private Lookup _HosClientType;
        [DataMemberAttribute]
        public long HosClientID
        {
            get
            {
                return _HosClientID;
            }
            set
            {
                _HosClientID = value;
                RaisePropertyChanged("HosClientID");
            }
        }
        [DataMemberAttribute]
        public string HosClientCode
        {
            get
            {
                return _HosClientCode;
            }
            set
            {
                _HosClientCode = value;
                RaisePropertyChanged("HosClientCode");
            }
        }
        [DataMemberAttribute]
        public string ClientName
        {
            get
            {
                return _ClientName;
            }
            set
            {
                _ClientName = value;
                RaisePropertyChanged("ClientName");
            }
        }
        [DataMemberAttribute]
        public string CompanyName
        {
            get
            {
                return _CompanyName;
            }
            set
            {
                _CompanyName = value;
                RaisePropertyChanged("CompanyName");
            }
        }
        [DataMemberAttribute]
        public string TaxNumber
        {
            get
            {
                return _TaxNumber;
            }
            set
            {
                _TaxNumber = value;
                RaisePropertyChanged("TaxNumber");
            }
        }
        [DataMemberAttribute]
        public string Address
        {
            get
            {
                return _Address;
            }
            set
            {
                _Address = value;
                RaisePropertyChanged("Address");
            }
        }
        [DataMemberAttribute]
        public string CityStateZipCode
        {
            get
            {
                return _CityStateZipCode;
            }
            set
            {
                _CityStateZipCode = value;
                RaisePropertyChanged("CityStateZipCode");
            }
        }
        [DataMemberAttribute]
        public string ContactPerson
        {
            get
            {
                return _ContactPerson;
            }
            set
            {
                _ContactPerson = value;
                RaisePropertyChanged("ContactPerson");
            }
        }
        [DataMemberAttribute]
        public string TelephoneNumber
        {
            get
            {
                return _TelephoneNumber;
            }
            set
            {
                _TelephoneNumber = value;
                RaisePropertyChanged("TelephoneNumber");
            }
        }
        [DataMemberAttribute]
        public string FaxNumber
        {
            get
            {
                return _FaxNumber;
            }
            set
            {
                _FaxNumber = value;
                RaisePropertyChanged("FaxNumber");
            }
        }
        [DataMemberAttribute]
        public string EmailAddress
        {
            get
            {
                return _EmailAddress;
            }
            set
            {
                _EmailAddress = value;
                RaisePropertyChanged("EmailAddress");
            }
        }
        [DataMemberAttribute]
        public string WebSite
        {
            get
            {
                return _WebSite;
            }
            set
            {
                _WebSite = value;
                RaisePropertyChanged("WebSite");
            }
        }
        [DataMemberAttribute]
        public string ClientDescription
        {
            get
            {
                return _ClientDescription;
            }
            set
            {
                _ClientDescription = value;
                RaisePropertyChanged("ClientDescription");
            }
        }
        [DataMemberAttribute]
        public bool IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                _IsActive = value;
                RaisePropertyChanged("IsActive");
            }
        }
        [DataMemberAttribute]
        public long V_HosClientType
        {
            get
            {
                return _V_HosClientType;
            }
            set
            {
                _V_HosClientType = value;
                RaisePropertyChanged("V_HosClientType");
            }
        }
        [DataMemberAttribute]
        public string AccountNumber
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
        [DataMemberAttribute]
        public string BankName
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
        [DataMemberAttribute]
        public string BranchName
        {
            get
            {
                return _BranchName;
            }
            set
            {
                _BranchName = value;
                RaisePropertyChanged("BranchName");
            }
        }
        [DataMemberAttribute]
        public Lookup HosClientType
        {
            get
            {
                return _HosClientType;
            }
            set
            {
                if (_HosClientType == value)
                {
                    return;
                }
                _HosClientType = value;
                RaisePropertyChanged("HosClientType");
            }
        }
        //20190816 TBL: Để khi nhập vào combobox thì sẽ lọc được
        public override string ToString()
        {
            return ClientName;
        }
    }
}