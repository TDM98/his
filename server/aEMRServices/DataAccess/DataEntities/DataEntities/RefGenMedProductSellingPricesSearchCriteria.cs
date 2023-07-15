using System;
using System.Net;
using System.Windows;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public class RefGenMedProductSellingPricesSearchCriteria: SearchCriteriaBase
    {
        public RefGenMedProductSellingPricesSearchCriteria()
        { 
        }

        private Int64 _GenMedProductID;
        public Int64 GenMedProductID
        {
            get { return _GenMedProductID; }
            set 
            {
                _GenMedProductID = value;
                RaisePropertyChanged("GenMedProductID");
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
          set { _FromDate = value; 
            RaisePropertyChanged("FromDate");
          }
        }

        private Nullable<DateTime> _ToDate;
        public Nullable<DateTime> ToDate
        {
          get { return _ToDate; }
          set { _ToDate = value; 
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

