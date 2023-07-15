using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using eHCMS.Services.Core;

namespace DataEntities
{
    public class ClinicDeptStockTakesSearchCriteria : SearchCriteriaBase
    {
        public ClinicDeptStockTakesSearchCriteria()
        {

        }
        private Nullable<long> _ClinicDeptStockTakeID;
        public Nullable<long> ClinicDeptStockTakeID
        {
            get
            {
                return _ClinicDeptStockTakeID;
            }
            set
            {
                _ClinicDeptStockTakeID = value;
                RaisePropertyChanged("ClinicDeptStockTakeID");
            }
        }

        private Nullable<long> _StoreID;
        public Nullable<long> StoreID
        {
            get
            {
                return _StoreID;
            }
            set
            {
                _StoreID = value;
                RaisePropertyChanged("StoreID");
            }
        }

        private string _StockTakePeriodName;
        public string StockTakePeriodName
        {
            get
            {
                return _StockTakePeriodName;
            }
            set
            {
                _StockTakePeriodName = value;
                RaisePropertyChanged("StockTakePeriodName");
            }
        }

        private Nullable<DateTime> _FromDate;
        public Nullable<DateTime> FromDate
        {
            get
            {
                return _FromDate;
            }
            set
            {
                _FromDate = value;
                RaisePropertyChanged("FromDate");
            }
        }

        private Nullable<DateTime> _ToDate;
        public Nullable<DateTime> ToDate
        {
            get
            {
                return _ToDate;
            }
            set
            {
                _ToDate = value;
                RaisePropertyChanged("ToDate");
            }
        }

        public Int64? V_MedProductType
        {
            get
            {
                return _V_MedProductType;
            }
            set
            {
                _V_MedProductType = value;
                RaisePropertyChanged("V_MedProductType");
            }
        }
        private Int64? _V_MedProductType;

    }
}
