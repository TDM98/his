using System;
using System.Net;
using System.Windows;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public class DrugClassSearchCriteria : SearchCriteriaBase
    {
        public DrugClassSearchCriteria()
        {
        }

        private string _FaName;
        public string FaName
        {
            get 
            {
                return _FaName; 
            }
            set 
            {
                _FaName = value;
                RaisePropertyChanged("FaName");
            }
        }

        
        private Int64 _V_MedProductType;
        public Int64 V_MedProductType
        {
            get { return _V_MedProductType; }
            set 
            { 
                _V_MedProductType = value;
                RaisePropertyChanged("V_MedProductType");
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