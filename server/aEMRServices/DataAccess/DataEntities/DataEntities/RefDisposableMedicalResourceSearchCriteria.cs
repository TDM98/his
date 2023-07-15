using System;
using System.Net;
using System.Windows;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public class RefDisposableMedicalResourceSearchCriteria : SearchCriteriaBase
    {
        public RefDisposableMedicalResourceSearchCriteria()
        {
        }

        private Int64 _DMedRscrTypeID;
        public Int64 DMedRscrTypeID
        {
            get { return _DMedRscrTypeID; }
            set 
            {
                _DMedRscrTypeID = value;
                RaisePropertyChanged("DMedRscrTypeID");
            }
        }

        private string _DMedRscrName;
        public string DMedRscrName
        {
            get { return _DMedRscrName; }
            set { 
                _DMedRscrName = value;
                RaisePropertyChanged("DMedRscrName");
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