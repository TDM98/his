using System;
using System.Net;
using System.Windows;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public class PCLFormsSearchCriteria : SearchCriteriaBase
    {
        public PCLFormsSearchCriteria()
        {
        }

        private Int64 _V_PCLMainCategory;
        public Int64 V_PCLMainCategory
        {
            get { return _V_PCLMainCategory; }
            set
            {
                if (_V_PCLMainCategory != value)
                {
                    _V_PCLMainCategory = value;
                    RaisePropertyChanged("V_PCLMainCategory");
                }
            }
        }


        private string _PCLFormName;
        public string PCLFormName
        {
            get 
            { 
                return _PCLFormName; 
            }
            set 
            {
                _PCLFormName = value;
                RaisePropertyChanged("PCLFormName");
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