using System;
using System.Net;
using System.Windows;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public class SupplierGenericDrugPriceSearchCriteria: SearchCriteriaBase
    {
        public SupplierGenericDrugPriceSearchCriteria()
        { 
        }

        private Int64 _SupplierID;
        public Int64 SupplierID
        {
            get { return _SupplierID; }
            set 
            { 
                _SupplierID = value;
                RaisePropertyChanged("SupplierID");
            }
        }

        private string _SupplierName;
        public string SupplierName
        {
            get { return _SupplierName; }
            set 
            { 
                _SupplierName = value;
                RaisePropertyChanged("SupplierName");
            }
        }

        private string _Address;
        public string Address
        {
            get { return _Address; }
            set 
            { 
                _Address = value;
                RaisePropertyChanged("Address");
            }
        }

        private string _BrandName;
        public string BrandName
        {
            get { return _BrandName; }
            set 
            { 
                _BrandName = value;
                RaisePropertyChanged("BrandName");
            }
        }

        private string _GenericName;
        public string GenericName
        {
            get { return _GenericName; }
            set 
            { 
                _GenericName = value;
                RaisePropertyChanged("GenericName");
            }
        }

        private Int64 _DrugID;
        public Int64 DrugID
        {
            get { return _DrugID; }
            set 
            { 
                _DrugID = value;
                RaisePropertyChanged("DrugID");
            }
        }

        //-1:All; 0: Old;1: Current: 2: New(Future)
        private Int32 _PriceType;
        public Int32 PriceType
        {
            get { return _PriceType; }
            set
            {
                _PriceType = value;
                RaisePropertyChanged("PriceType");
            }
        }

        private Nullable<DateTime> _FromDate;
        public Nullable<DateTime> FromDate
        {
            get { return _FromDate; }
            set
            {
                _FromDate = value;
                RaisePropertyChanged("FromDate");
            }
        }

        private Nullable<DateTime> _ToDate;
        public Nullable<DateTime> ToDate
        {
            get { return _ToDate; }
            set
            {
                _ToDate = value;
                RaisePropertyChanged("ToDate");
            }
        }

        private string _OrderBy;
        public string OrderBy
        {
            get { return _OrderBy; }
            set { _OrderBy = value;
            RaisePropertyChanged("OrderBy");
            }
        }

    }
}

