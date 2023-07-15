using System;
using System.Net;
using System.Windows;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public class DeptMedServiceItemsSearchCriteria : SearchCriteriaBase
    {
        public DeptMedServiceItemsSearchCriteria()
        {
        }

        private Int64 _DeptID;
        public Int64 DeptID
        {
            get { return _DeptID; }
            set 
            { 
                _DeptID = value;
                RaisePropertyChanged("DeptID");            
            }
        }

        private Int64 _MedicalServiceTypeID;
        public Int64 MedicalServiceTypeID
        {
            get { return _MedicalServiceTypeID; }
            set
            {
                _MedicalServiceTypeID = value;
                RaisePropertyChanged("MedicalServiceTypeID");
            }
        }


        private string _MedServiceCode;
        public string MedServiceCode
        {
            get { return _MedServiceCode; }
            set
            {
                _MedServiceCode = value;
                RaisePropertyChanged("MedServiceCode");
            }
        }

        private string _MedServiceName;
        public string MedServiceName
        {
            get { return _MedServiceName; }
            set 
            { 
                _MedServiceName = value;
                RaisePropertyChanged("MedServiceName");
            }
        }

        private Int64 _MedServiceItemPriceListID;   
        public Int64 MedServiceItemPriceListID
        {
            get { return _MedServiceItemPriceListID; }
            set
            {
                _MedServiceItemPriceListID = value;
                RaisePropertyChanged("MedServiceItemPriceListID");
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