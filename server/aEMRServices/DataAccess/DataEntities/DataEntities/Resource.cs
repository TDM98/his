using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class Resource : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new Resource object.

        /// <param name="rscrID">Initial value of the RscrID property.</param>
        public static Resource CreateResource(Int64 rscrID)
        {
            Resource resource = new Resource();
            resource.RscrID = rscrID;
            return resource;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 RscrID
        {
            get
            {
                return _RscrID;
            }
            set
            {
                if (_RscrID != value)
                {
                    OnRscrIDChanging(value);
                    ////ReportPropertyChanging("RscrID");
                    _RscrID = value;
                    RaisePropertyChanged("RscrID");
                    OnRscrIDChanged();
                }
            }
        }
        private Int64 _RscrID;
        partial void OnRscrIDChanging(Int64 value);
        partial void OnRscrIDChanged();





        [DataMemberAttribute()]
        public Nullable<Byte> DeprecTypeID
        {
            get
            {
                return _DeprecTypeID;
            }
            set
            {
                OnDeprecTypeIDChanging(value);
                ////ReportPropertyChanging("DeprecTypeID");
                _DeprecTypeID = value;
                RaisePropertyChanged("DeprecTypeID");
                OnDeprecTypeIDChanged();
            }
        }
        private Nullable<Byte> _DeprecTypeID;
        partial void OnDeprecTypeIDChanging(Nullable<Byte> value);
        partial void OnDeprecTypeIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> MedRescrTypeID
        {
            get
            {
                return _MedRescrTypeID;
            }
            set
            {
                OnMedRescrTypeIDChanging(value);
                ////ReportPropertyChanging("MedRescrTypeID");
                _MedRescrTypeID = value;
                RaisePropertyChanged("MedRescrTypeID");
                OnMedRescrTypeIDChanged();
            }
        }
        private Nullable<long> _MedRescrTypeID;
        partial void OnMedRescrTypeIDChanging(Nullable<long> value);
        partial void OnMedRescrTypeIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> SupplierID
        {
            get
            {
                return _SupplierID;
            }
            set
            {
                OnSupplierIDChanging(value);
                ////ReportPropertyChanging("SupplierID");
                _SupplierID = value;
                RaisePropertyChanged("SupplierID");
                OnSupplierIDChanged();
            }
        }
        private Nullable<long> _SupplierID;
        partial void OnSupplierIDChanging(Nullable<long> value);
        partial void OnSupplierIDChanged();





        [DataMemberAttribute()]
        public String RscrItemCode
        {
            get
            {
                return _RscrItemCode;
            }
            set
            {
                OnRscrItemCodeChanging(value);
                ////ReportPropertyChanging("RscrItemCode");
                _RscrItemCode = value;
                RaisePropertyChanged("RscrItemCode");
                OnRscrItemCodeChanged();
            }
        }
        private String _RscrItemCode;
        partial void OnRscrItemCodeChanging(String value);
        partial void OnRscrItemCodeChanged();





        [DataMemberAttribute()]
        [Required(ErrorMessage = "Bạn phải nhập Nhãn hiệu Vật Tư!")]
        [StringLength(128, ErrorMessage = "Nhãn hiệu Vật Tư quá dài!")]
        public String RscrName_Brand
        {
            get
            {
                return _RscrName_Brand;
            }
            set
            {
                OnRscrName_BrandChanging(value);
                ValidateProperty("RscrName_Brand", value);
                _RscrName_Brand = value;
                RaisePropertyChanged("RscrName_Brand");
                OnRscrName_BrandChanged();
            }
        }
        private String _RscrName_Brand;
        partial void OnRscrName_BrandChanging(String value);
        partial void OnRscrName_BrandChanged();





        [DataMemberAttribute()]
        [Required(ErrorMessage = "Bạn phải nhập Function!")]
        public String RscrFunctions
        {
            get
            {
                return _RscrFunctions;
            }
            set
            {
                OnRscrFunctionsChanging(value);
                ValidateProperty("RscrFunctions", value);
                _RscrFunctions = value;
                RaisePropertyChanged("RscrFunctions");
                OnRscrFunctionsChanged();
            }
        }
        private String _RscrFunctions;
        partial void OnRscrFunctionsChanging(String value);
        partial void OnRscrFunctionsChanged();


        [DataMemberAttribute()]
        public String RscrTechInfo
        {
            get
            {
                return _RscrTechInfo;
            }
            set
            {
                OnRscrTechInfoChanging(value);
                ////ReportPropertyChanging("RscrTechInfo");
                _RscrTechInfo = value;
                RaisePropertyChanged("RscrTechInfo");
                OnRscrTechInfoChanged();
            }
        }
        private String _RscrTechInfo;
        partial void OnRscrTechInfoChanging(String value);
        partial void OnRscrTechInfoChanged();

        [DataMemberAttribute()]
        public Nullable<Double> RscrDepreciationRate
        {
            get
            {
                return _RscrDepreciationRate;
            }
            set
            {
                OnRscrDepreciationRateChanging(value);
                ////ReportPropertyChanging("RscrDepreciationRate");
                _RscrDepreciationRate = value;
                RaisePropertyChanged("RscrDepreciationRate");
                OnRscrDepreciationRateChanged();
            }
        }
        private Nullable<Double> _RscrDepreciationRate;
        partial void OnRscrDepreciationRateChanging(Nullable<Double> value);
        partial void OnRscrDepreciationRateChanged();


        [DataMemberAttribute()]
        [Required(ErrorMessage = "Bạn phải nhập Giá cả vật tư!")]
        [Range(1000, 10000000, ErrorMessage = "Giá cả vật tư phải > 1000")]
        public Nullable<Decimal> RscrPrice
        {
            get
            {
                return _RscrPrice;
            }
            set
            {
                OnRscrPriceChanging(value);
                ValidateProperty("RscrPrice", value);
                _RscrPrice = value;
                RaisePropertyChanged("RscrPrice");
                OnRscrPriceChanged();
            }
        }
        private Nullable<Decimal> _RscrPrice;
        partial void OnRscrPriceChanging(Nullable<Decimal> value);
        partial void OnRscrPriceChanged();


        [DataMemberAttribute()]
        [Required(ErrorMessage = "Vui lòng chọn đơn vị tính")]
        public String V_RscrUnit
        {
            get
            {
                return _V_RscrUnit;
            }
            set
            {
                OnV_RscrUnitChanging(value);
                ValidateProperty("V_RscrUnit", value);
                _V_RscrUnit = value;
                RaisePropertyChanged("V_RscrUnit");
                OnV_RscrUnitChanged();
            }
        }
        private String _V_RscrUnit;
        partial void OnV_RscrUnitChanging(String value);
        partial void OnV_RscrUnitChanged();





        [DataMemberAttribute()]
        public Nullable<Boolean> BeOfHIMedicineList
        {
            get
            {
                return _BeOfHIMedicineList;
            }
            set
            {
                OnBeOfHIMedicineListChanging(value);
                ////ReportPropertyChanging("BeOfHIMedicineList");
                _BeOfHIMedicineList = value;
                RaisePropertyChanged("BeOfHIMedicineList");
                OnBeOfHIMedicineListChanged();
            }
        }
        private Nullable<Boolean> _BeOfHIMedicineList;
        partial void OnBeOfHIMedicineListChanging(Nullable<Boolean> value);
        partial void OnBeOfHIMedicineListChanged();





        [DataMemberAttribute()]
        public Nullable<Byte> ResourceType
        {
            get
            {
                return _ResourceType;
            }
            set
            {
                OnResourceTypeChanging(value);
                ////ReportPropertyChanging("ResourceType");
                _ResourceType = value;
                RaisePropertyChanged("ResourceType");
                OnResourceTypeChanged();
            }
        }
        private Nullable<Byte> _ResourceType;
        partial void OnResourceTypeChanging(Nullable<Byte> value);
        partial void OnResourceTypeChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_CHEMICAL_REL_INHER_RESOURCE", "ChemicalResources")]
        public ChemicalResource ChemicalResource
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_RESOURCE_REL_RM10_DEPRECIA", "DepreciationTypes")]
        public DepreciationType DepreciationType
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_INWARDRE_REL_RM17_RESOURCE", "InwardResources")]
        public ObservableCollection<InwardResource> InwardResources
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_MEDICALE_REL_INHER_RESOURCE", "MedicalEquimentsResources")]
        public MedicalEquimentsResource MedicalEquimentsResource
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_MEDICALU_REL_INHER_RESOURCE", "MedicalUtilitiesResources")]
        public MedicalUtilitiesResource MedicalUtilitiesResource
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_RESOURCE_REL_RM16_MEDRESOU", "MedResourceType")]
        public MedResourceType MedResourceType
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_OTHERRES_REL_INHER_RESOURCE", "OtherResources")]
        public OtherResource OtherResource
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_OUTWARDR_REL_RM18_RESOURCE", "OutwardResources")]
        public ObservableCollection<OutwardResource> OutwardResources
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_RESOURCE_REL_RM07_SUPPLIER", "Supplier")]
        public Supplier Supplier
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_RESOURCE_REL_RM08_RESOURCE", "ResourcesForMedicalServices")]
        public ObservableCollection<ResourcesForMedicalService> ResourcesForMedicalServices
        {
            get;
            set;
        }

        #endregion
    }
}
