using System;
using System.Net;
using System.Windows;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public class PCLGroupsSearchCriteria : SearchCriteriaBase
    {
        public PCLGroupsSearchCriteria()
        {
        }

        private Int64 _V_PCLCategory;
        public Int64 V_PCLCategory
        {
            get { return _V_PCLCategory; }
            set 
            { 
                _V_PCLCategory = value;
                RaisePropertyChanged("V_PCLCategory");
            }
        }

        private string _PCLGroupName;
        public string PCLGroupName
        {
            get 
            { 
                return _PCLGroupName; 
            }
            set 
            {
                _PCLGroupName = value;
                RaisePropertyChanged("PCLGroupName");
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