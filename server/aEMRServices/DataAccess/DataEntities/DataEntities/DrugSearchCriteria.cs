using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using eHCMS.Services.Core;

namespace DataEntities
{
    public class DrugSearchCriteria : MedProductSearchCriteria
    {
        public DrugSearchCriteria():base()
        {

        }
    }

    public class MedProductSearchCriteria : SearchCriteriaBase
    {
        public MedProductSearchCriteria()
        {

        }
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

        public long StoreID
        {
            get
            {
                if (_storage == null)
                {
                    return 0;
                }
                return _storage.StoreID;
            }
        }

        private RefStorageWarehouseLocation _storage;
        
        public RefStorageWarehouseLocation Storage
        {
            get
            {
                return _storage;
            }
            set
            {
                _storage = value;
                RaisePropertyChanged("Storage");
            }
        }

        private string _brandName;
        public string BrandName
        {
            get
            {
                return _brandName;
            }
            set
            {
                _brandName = value;
                RaisePropertyChanged("BrandName");
            }
        }
        private int _isinsurance;
        public int IsInsurance
        {
            get
            {
                return _isinsurance;
            }
            set
            {
                _isinsurance = value;
                RaisePropertyChanged("IsInsurance");
            }
        }

        private int _isActive;
        public int IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
                RaisePropertyChanged("IsActive");
            }
        }
        private int _isconsult;
        public int IsConsult
        {
            get
            {
                return _isconsult;
            }
            set
            {
                _isconsult = value;
                RaisePropertyChanged("IsConsult");
            }
        }

        private int _isShow;
        public int IsShow
        {
            get
            {
                return _isShow;
            }
            set
            {
                _isShow = value;
                RaisePropertyChanged("IsShow");
            }
        }

        private Nullable<decimal> _faID;
        public Nullable<decimal> FaID
        {
            get
            {
                return _faID;
            }
            set
            {
                _faID = value;
                RaisePropertyChanged("FaID");
            }
        }
        private AllLookupValues.MedProductType _MedProductType = AllLookupValues.MedProductType.Unknown;
        public AllLookupValues.MedProductType MedProductType
        {
            get
            {
                return _MedProductType;
            }
            set
            {
                if (MedProductType != value)
                {
                    _MedProductType = value;
                    RaisePropertyChanged("MedProductType"); 
                }
            }
        }

        private long _V_CatDrugType = (long)AllLookupValues.V_CatDrugType.Pharmacy;
        public long V_CatDrugType
        {
            get
            {
                return _V_CatDrugType;
            }
            set
            {
                if (_V_CatDrugType != value)
                {
                    _V_CatDrugType = value;
                    RaisePropertyChanged("V_CatDrugType"); 
                }
            }
        }
    }

    public class MedicalServiceSearchCriteria : SearchCriteriaBase
    {
        public MedicalServiceSearchCriteria()
        {

        }
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

        private string _MedicalServiceName;
        public string MedicalServiceName
        {
            get
            {
                return _MedicalServiceName;
            }
            set
            {
                _MedicalServiceName = value;
                RaisePropertyChanged("MedicalServiceName");
            }
        }

        private string _ServiceTypeIDList = "1";
        public string ServiceTypeIDList
        {
            get
            {
                return _ServiceTypeIDList;
            }
            set
            {
                _ServiceTypeIDList = value;
                RaisePropertyChanged("ServiceTypeIDList");
            }
        }
    }

    public class PCLItemSearchCriteria : SearchCriteriaBase
    {
        public PCLItemSearchCriteria()
        {

        }
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

        private string _PCLExamTypeName;
        public string PCLExamTypeName
        {
            get
            {
                return _PCLExamTypeName;
            }
            set
            {
                _PCLExamTypeName = value;
                RaisePropertyChanged("PCLExamTypeName");
            }
        }
    }
}
