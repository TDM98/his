using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using eHCMS.Services.Core;

namespace DataEntities
{
    public class SupplierSearchCriteria : SearchCriteriaBase
    {
        private string _orderBy;
        public string OrderBy
        {
            get
            {
                return _orderBy;
            }
            set
            {
                _orderBy = value;
                RaisePropertyChanged("OrderBy");
            }
        }

        private string _SupplierName;
        public string SupplierName
        {
            get
            {
                return _SupplierName;
            }
            set
            {
                _SupplierName = value;
                RaisePropertyChanged("SupplierName");
            }
        }

        private long? _V_SupplierType;
        public long? V_SupplierType
        {
            get
            {
                return _V_SupplierType;
            }
            set
            {
                _V_SupplierType = value;
                RaisePropertyChanged("V_SupplierType");
            }
        }

        private long? _PharmacyEstimatePoID;
        public long? PharmacyEstimatePoID
        {
            get
            {
                return _PharmacyEstimatePoID;
            }
            set
            {
                _PharmacyEstimatePoID = value;
                RaisePropertyChanged("PharmacyEstimatePoID");
            }
        }

        private byte? _IsMain;
        public byte? IsMain
        {
            get
            {
                return _IsMain;
            }
            set
            {
                _IsMain = value;
                RaisePropertyChanged("IsMain");
            }
        }


        private long? _V_MedProductType;
        public long? V_MedProductType
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

       
    }
}
