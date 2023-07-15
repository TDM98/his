using System;
using System.Net;
using System.Windows;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public class MedServiceItemPriceSearchCriteria: SearchCriteriaBase
    {
        public MedServiceItemPriceSearchCriteria()
        { 
        }

        private Int64 _DeptMedServItemID;
        public Int64 DeptMedServItemID
        {
            get { return _DeptMedServItemID; }
            set 
            { 
                _DeptMedServItemID = value;
                RaisePropertyChanged("DeptMedServItemID");
            }
        }

        public Int64 MedServiceID
        {
            get
            {
                return _MedServiceID;
            }
            set
            {
                if (_MedServiceID != value)
                {
                    _MedServiceID = value;
                    RaisePropertyChanged("MedServiceID");
                }
            }
        }
        private Int64 _MedServiceID;

        public Nullable<Int64> MedServiceItemPriceListID
        {
            get { return _MedServiceItemPriceListID; }
            set
            {
                if (_MedServiceItemPriceListID != value)
                {
                    _MedServiceItemPriceListID = value;
                    RaisePropertyChanged("MedServiceItemPriceListID");
                }
            }
        }
        private Nullable<Int64> _MedServiceItemPriceListID;

        public long MedicalServiceTypeID
        {
            get
            {
                return _MedicalServiceTypeID;
            }
            set
            {
                _MedicalServiceTypeID = value;
                RaisePropertyChanged("MedicalServiceTypeID");
            }
        }
        private long _MedicalServiceTypeID;

        public String MedServiceCode
        {
            get
            {
                return _MedServiceCode;
            }
            set
            {
                _MedServiceCode = value;
                RaisePropertyChanged("MedServiceCode");
            }
        }
        private String _MedServiceCode;

        public String MedServiceName
        {
            get
            {
                return _MedServiceName;
            }
            set
            {
                _MedServiceName = value;
                RaisePropertyChanged("MedServiceName");
            }
        }
        private String _MedServiceName;

        private Int64 _V_TypePrice;
        public Int64 V_TypePrice
        {
            get { return _V_TypePrice; }
            set 
            {
                _V_TypePrice = value;
                RaisePropertyChanged("V_TypePrice");
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

