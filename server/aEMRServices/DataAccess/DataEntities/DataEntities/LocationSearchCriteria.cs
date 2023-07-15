using System;
using System.Net;
using System.Windows;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public class LocationSearchCriteria : SearchCriteriaBase
    {
        public LocationSearchCriteria()
        {
        }

        private Int64 _RmTypeID;
        public Int64 RmTypeID
        {
            get { return _RmTypeID; }
            set 
            { 
                _RmTypeID = value;
                RaisePropertyChanged("RmTypeID");
            }
        }

        private string _LocationName;
        public string LocationName
        {
            get 
            { 
                return _LocationName; 
            }
            set 
            {
                _LocationName = value;
                RaisePropertyChanged("LocationName");
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