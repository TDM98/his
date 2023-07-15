using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities
{

    public class ExemptionsMedServiceItemsSearchCriteria : SearchCriteriaBase
    {
        public ExemptionsMedServiceItemsSearchCriteria()
        {
        }

        private Int64 _PromoDiscProgID;
        public Int64 PromoDiscProgID
        {
            get { return _PromoDiscProgID; }
            set
            {
                _PromoDiscProgID = value;
                RaisePropertyChanged("PromoDiscProgID");
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