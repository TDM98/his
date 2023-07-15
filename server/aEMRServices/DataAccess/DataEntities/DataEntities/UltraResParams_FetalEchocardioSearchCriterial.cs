using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using eHCMS.Services.Core;

namespace DataEntities
{
     public class UltraResParams_FetalEchocardioSearchCriterial : SearchCriteriaBase
    {
         public UltraResParams_FetalEchocardioSearchCriterial()
        {

        }
        
        private bool _searchAll;
        public bool searchAll
        {
            get
            {
                return _searchAll;
            }
            set
            {
                _searchAll = value;
                RaisePropertyChanged("address");
            }
        }

        private string _yearOfBirth;
        public string yearOfBirth
        {
            get
            {
                return _yearOfBirth;
            }
            set
            {
                _yearOfBirth = value;
                RaisePropertyChanged("yearOfBirth");
            }
        }

        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                RaisePropertyChanged("Name");
            }
        }

        private string _address;
        public string address
        {
            get
            {
                return _address;
            }
            set
            {
                _address = value;
                RaisePropertyChanged("address");
            }
        }

        private string _PhoneNumber;
        public string PhoneNumber
        {
            get
            {
                return _PhoneNumber;
            }
            set
            {
                _PhoneNumber = value;
                RaisePropertyChanged("PhoneNumber");
            }
        }

        private string _Day;
        public string Day
        {
            get
            {
                return _Day;
            }
            set
            {
                _Day = value;
                RaisePropertyChanged("Day");
            }
        }
        
    }

    
}
