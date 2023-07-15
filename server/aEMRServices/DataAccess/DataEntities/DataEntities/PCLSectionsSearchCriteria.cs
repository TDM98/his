using System;
using System.Net;
using System.Windows;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public class PCLSectionsSearchCriteria : SearchCriteriaBase
    {
        public PCLSectionsSearchCriteria()
        {
        }

        //private Int64 _PCLFormID;
        //public Int64 PCLFormID
        //{
        //    get { return _PCLFormID; }
        //    set 
        //    { 
        //        _PCLFormID = value;
        //        RaisePropertyChanged("PCLFormID");
        //    }
        //}

        private string _PCLSectionName;
        public string PCLSectionName
        {
            get 
            { 
                return _PCLSectionName; 
            }
            set 
            {
                _PCLSectionName = value;
                RaisePropertyChanged("PCLSectionName");
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