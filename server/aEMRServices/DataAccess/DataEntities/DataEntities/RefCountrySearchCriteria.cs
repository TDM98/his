using System;
using System.Net;
using System.Windows;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public class RefCountrySearchCriteria : SearchCriteriaBase
    {
        public RefCountrySearchCriteria()
        {
        }


        private string _CountryName;
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

        private string _OrderBy;
        public string OrderBy
        {
            get { return _OrderBy; }
            set
            {
                _OrderBy = value;
                RaisePropertyChanged("OrderBy");
            }
        }

    }
}