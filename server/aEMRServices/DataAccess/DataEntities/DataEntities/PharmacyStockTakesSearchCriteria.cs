using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using eHCMS.Services.Core;

namespace DataEntities
{
    public class PharmacyStockTakesSearchCriteria : SearchCriteriaBase
    {
        public PharmacyStockTakesSearchCriteria()
        {

        }
        private Nullable<long> _PharmacyStockTakeID;
        public Nullable<long> PharmacyStockTakeID
        {
            get
            {
                return _PharmacyStockTakeID;
            }
            set
            {
                _PharmacyStockTakeID = value;
                RaisePropertyChanged("PharmacyStockTakeID");
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

    }
}
