using System;
using System.Net;
using System.Windows;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public class RefGenMedProductDetailsSearchCriteria : SearchCriteriaBase
    {
        public RefGenMedProductDetailsSearchCriteria()
        {
        }

        private long _V_CatDrugType = (long)AllLookupValues.V_CatDrugType.All;
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

        private string _Code;
        public string Code
        {
            get { return _Code; }
            set
            {
                _Code = value;
                RaisePropertyChanged("Code");
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

        private int _isinsurance = 0;
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

        private int _isActive = 0;
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
        private int _isconsult = 0;
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

        private long _GenericID;
        public long GenericID
        {
            get { return _GenericID; }
            set
            {
                _GenericID = value;
                RaisePropertyChanged("GenericID");
            }
        }
    }
}