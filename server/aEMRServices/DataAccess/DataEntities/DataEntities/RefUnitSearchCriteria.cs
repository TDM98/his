using System;
using System.Net;
using System.Windows;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public class RefUnitSearchCriteria : SearchCriteriaBase
    {
        public RefUnitSearchCriteria()
        {
        }


        private string _UnitName;
        public string UnitName
        {
            get 
            {
                return _UnitName; 
            }
            set 
            {
                _UnitName = value;
                RaisePropertyChanged("UnitName");
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