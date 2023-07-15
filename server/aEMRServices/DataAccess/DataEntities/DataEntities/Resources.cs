/*
 * 29052018 #001 TTM: Add new property HIRepResourceCode
 * 09062018 #002 TTM: Add new Property V_PCLMainCategory
 * 20230424 #003 DatTB: Thêm các cột quản lý vật tư
 * 20230501 #004 DatTB: Thêm ResourceCode (model) thiết bị
 * 20230609 #005 DatTB: Thêm cột tên tiếng anh
 */
using System;
using System.Net;
using System.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using eHCMS.Services.Core;
using eHCMS.Services.Core.Base;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class Resources : EntityBase, IEditableObject
    {
        public Resources() : base() 
        {
            V_UnitLookup = new Lookup();
            V_UnitLookup.LookupID=5700;
            V_UnitLookup.ObjectValue = "Cai";

            V_ExpenditureSource = 70001;

            V_RscrStatus = new Lookup();
            V_RscrStatus.LookupID = 908000;
        }
        private Resources _tempResources;
        #region IEditableObject Members
        public void BeginEdit()
        {
            _tempResources = (Resources)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempResources)
                CopyFrom(_tempResources);            
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(Resources p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }
        
        #endregion
        #region Factory Method

        /// Create a new HospitalizationHistory object.

        public static Resources CreateResources(
                                                long RscrID
                                                ,int DeprecTypeID
                                               ,long RscrTypeID
                                               ,long SupplierID
                                               ,string ItemName
                                               ,string ItemBrand
                                               ,string Functions
                                               ,string GeneralTechInfo
                                               ,float DepreciationByTimeRate
                                               ,float DepreciationByUsageRate
                                               ,decimal BuyPrice
                                               ,long V_RscrUnit
                                               ,bool OnHIApprovedList
                                               ,int WarrantyTime
                                               ,bool IsLocatable
                                               ,bool IsDeleted
                                                , string RsrcHisCode
                                                , ObservableCollection<ResourceTypeLists> RscrTypeLists
                                                , string HisItemName
                                                , long UseForDeptID
                                                , DateTime? ManufactureDate
                                                , DateTime? UseDate
                                                , long Manufacturer
                                                , long ManufactureCountry
                                                , string CirculationNumber
                                                , DateTime? ContractFrom
                                                , DateTime? ContractTo
                                                , string ContractNumber
                                                , string SeriNumber
                                                , Lookup V_RscrStatus
                                                , string log
                                               , string ItemNameEng)
        {
            Resources resources = new Resources();
            resources.RscrID = RscrID;                   
            resources.DeprecTypeID=DeprecTypeID;
            resources.RscrTypeID =RscrTypeID;
            resources.SupplierID =SupplierID;
            resources.ItemName =ItemName;
            resources.ItemBrand=ItemBrand;
            resources.Functions = Functions;
            resources.GeneralTechInfo = GeneralTechInfo;
            resources.DepreciationByTimeRate = DepreciationByTimeRate;
            resources.DepreciationByUsageRate = DepreciationByUsageRate;
            resources.BuyPrice = BuyPrice;
            resources.V_RscrUnit = V_RscrUnit;
            resources.OnHIApprovedList = OnHIApprovedList;
            resources.WarrantyTime = WarrantyTime;
            resources.IsLocatable = IsLocatable;
            resources.IsDeleted = IsDeleted;
            resources.RsrcHisCode = RsrcHisCode;
            resources.RscrTypeLists = RscrTypeLists;
            resources.HisItemName = HisItemName;
            resources.UseForDeptID = UseForDeptID;
            resources.ManufactureDate = ManufactureDate;
            resources.UseDate = UseDate;
            resources.Manufacturer = Manufacturer;
            resources.ManufactureCountry = ManufactureCountry;
            resources.CirculationNumber = CirculationNumber;
            resources.ContractFrom = ContractFrom;
            resources.ContractTo = ContractTo;
            resources.ContractNumber = ContractNumber;
            resources.SeriNumber = SeriNumber;
            resources.V_RscrStatus = V_RscrStatus;
            resources.log = log;
            resources.ItemNameEng = ItemNameEng;
            return resources;
        }

        #endregion
        #region Primitive Properties
        [DataMemberAttribute()]
        public long RscrID
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
                    _RscrID = value;
                    RaisePropertyChanged("RscrID");
                    OnRscrIDChanged();
                }
            }
        }
        private long _RscrID;
        partial void OnRscrIDChanging(long value);
        partial void OnRscrIDChanged();

        [DataMemberAttribute()]
        public int DeprecTypeID
        {
            get
            {
                return _DeprecTypeID;
            }
            set
            {
                if (_DeprecTypeID != value)
                {
                    OnDeprecTypeIDChanging(value);
                    _DeprecTypeID = value;
                    RaisePropertyChanged("DeprecTypeID");
                    OnDeprecTypeIDChanged();
                }
            }
        }
        private int _DeprecTypeID;
        partial void OnDeprecTypeIDChanging(int value);
        partial void OnDeprecTypeIDChanged();

        [DataMemberAttribute()]
        [Required(ErrorMessage = "Bạn phải chọn Nhóm Vật Tư!")]
        public long RscrGroupID
        {
            get
            {
                return _RscrGroupID;
            }
            set
            {
                if (_RscrGroupID != value)
                {
                    OnRscrGroupIDChanging(value);
                    ValidateProperty("RscrGroupID", value);
                    _RscrGroupID = value;
                    RaisePropertyChanged("RscrGroupID");
                    OnRscrGroupIDChanged();
                }
            }
        }
        private long _RscrGroupID;
        partial void OnRscrGroupIDChanging(long value);
        partial void OnRscrGroupIDChanged();

        [DataMemberAttribute()]
        [Required(ErrorMessage = "Bạn phải chọn Loại Vật Tư!")]
        public long RscrTypeID
        {
            get
            {
                return _RscrTypeID;
            }
            set
            {
                if (_RscrTypeID != value)
                {
                    OnRscrTypeIDChanging(value);
                    ValidateProperty("RscrTypeID", value);
                    _RscrTypeID = value;
                    RaisePropertyChanged("RscrTypeID");
                    OnRscrTypeIDChanged();
                }
            }
        }
        private long _RscrTypeID;
        partial void OnRscrTypeIDChanging(long value);
        partial void OnRscrTypeIDChanged();

        [DataMemberAttribute()]
        public long SupplierID
        {
            get
            {
                return _SupplierID;
            }
            set
            {
                if (_SupplierID != value)
                {
                    OnSupplierIDChanging(value);
                    _SupplierID = value;
                    RaisePropertyChanged("SupplierID");
                    OnSupplierIDChanged();
                }
            }
        }
        private long _SupplierID;
        partial void OnSupplierIDChanging(long value);
        partial void OnSupplierIDChanged();

        [DataMemberAttribute()]
        //[Required(ErrorMessage = "Bạn phải nhập Tên Vật Tư!")]
        public string ItemName
        {
            get
            {
                return _ItemName;
            }
            set
            {
                if (_ItemName != value)
                {
                    OnItemNameChanging(value);
                    ValidateProperty("ItemName", value);
                    _ItemName = value;
                    RaisePropertyChanged("ItemName");
                    OnItemNameChanged();
                }
            }
        }
        private string _ItemName;
        partial void OnItemNameChanging(string value);
        partial void OnItemNameChanged();

        [DataMemberAttribute()]
        //[Required(ErrorMessage = "Bạn phải nhập Nhãn hiệu Vật Tư!")]
        //[StringLength(128, ErrorMessage = "Nhãn hiệu Vật Tư quá dài!")]
        public string ItemBrand
        {
            get
            {
                return _ItemBrand;
            }
            set
            {
                if (_ItemBrand != value)
                {
                    OnItemBrandChanging(value);
                    ValidateProperty("ItemBrand", value);
                    _ItemBrand = value;
                    RaisePropertyChanged("ItemBrand");
                    OnItemBrandChanged();
                }
            }
        }
        private string _ItemBrand;
        partial void OnItemBrandChanging(string value);
        partial void OnItemBrandChanged();

        [DataMemberAttribute()]
        public string Functions
        {
            get
            {
                return _Functions;
            }
            set
            {
                if (_Functions != value)
                {
                    OnFunctionsChanging(value);
                    _Functions = value;
                    RaisePropertyChanged("Functions");
                    OnFunctionsChanged();
                }
            }
        }
        private string _Functions;
        partial void OnFunctionsChanging(string value);
        partial void OnFunctionsChanged();

        [DataMemberAttribute()]
        public string GeneralTechInfo
        {
            get
            {
                return _GeneralTechInfo;
            }
            set
            {
                if (_GeneralTechInfo != value)
                {
                    OnGeneralTechInfoChanging(value);
                    _GeneralTechInfo = value;
                    RaisePropertyChanged("GeneralTechInfo");
                    OnGeneralTechInfoChanged();
                }
            }
        }
        private string _GeneralTechInfo;
        partial void OnGeneralTechInfoChanging(string value);
        partial void OnGeneralTechInfoChanged();

        [DataMemberAttribute()]
        public float DepreciationByTimeRate
        {
            get
            {
                return _DepreciationByTimeRate;
            }
            set
            {
                if (_DepreciationByTimeRate != value)
                {
                    OnDepreciationByTimeRateChanging(value);
                    _DepreciationByTimeRate = value;
                    RaisePropertyChanged("DepreciationByTimeRate");
                    OnDepreciationByTimeRateChanged();
                }
            }
        }
        private float _DepreciationByTimeRate;
        partial void OnDepreciationByTimeRateChanging(float value);
        partial void OnDepreciationByTimeRateChanged();

        [DataMemberAttribute()]
        public float DepreciationByUsageRate
        {
            get
            {
                return _DepreciationByUsageRate;
            }
            set
            {
                if (_DepreciationByUsageRate != value)
                {
                    OnDepreciationByUsageRateChanging(value);
                    _DepreciationByUsageRate = value;
                    RaisePropertyChanged("DepreciationByUsageRate");
                    OnDepreciationByUsageRateChanged();
                }
            }
        }
        private float _DepreciationByUsageRate;
        partial void OnDepreciationByUsageRateChanging(float value);
        partial void OnDepreciationByUsageRateChanged();

        [DataMemberAttribute()]
        [Required(ErrorMessage = "Bạn phải nhập Giá cả vật tư!")]
        [Range(0, 99999999999.0, ErrorMessage = "Giá cả vật tư phải > 1000")]
        public decimal BuyPrice
        {
            get
            {
                return _BuyPrice;
            }
            set
            {
                if (_BuyPrice != value)
                {
                    OnBuyPriceChanging(value);
                    ValidateProperty("BuyPrice", value);
                    _BuyPrice = value;
                    RaisePropertyChanged("BuyPrice");
                    OnBuyPriceChanged();
                }
            }
        }
        private decimal _BuyPrice;
        partial void OnBuyPriceChanging(decimal value);
        partial void OnBuyPriceChanged();

        [DataMemberAttribute()]
        [Required(ErrorMessage = "Vui lòng chọn đơn vị tính")]
        public long V_RscrUnit
        {
            get
            {
                return _V_RscrUnit;
            }
            set
            {
                if (_V_RscrUnit != value)
                {
                    OnV_RscrUnitChanging(value);
                    ValidateProperty("V_RscrUnit", value);
                    _V_RscrUnit = value;
                    RaisePropertyChanged("V_RscrUnit");
                    OnV_RscrUnitChanged();
                }
            }
        }
        private long _V_RscrUnit;
        partial void OnV_RscrUnitChanging(long value);
        partial void OnV_RscrUnitChanged();

        [DataMemberAttribute()]
        public bool OnHIApprovedList
        {
            get
            {
                return _OnHIApprovedList;
            }
            set
            {
                if (_OnHIApprovedList != value)
                {
                    OnOnHIApprovedListChanging(value);
                    _OnHIApprovedList = value;
                    RaisePropertyChanged("OnHIApprovedList");
                    OnOnHIApprovedListChanged();
                }
            }
        }
        private bool _OnHIApprovedList;
        partial void OnOnHIApprovedListChanging(bool value);
        partial void OnOnHIApprovedListChanged();

        [DataMemberAttribute()]
        public int WarrantyTime
        {
            get
            {
                return _WarrantyTime;
            }
            set
            {
                if (_WarrantyTime != value)
                {
                    OnWarrantyTimeChanging(value);
                    _WarrantyTime = value;
                    RaisePropertyChanged("WarrantyTime");
                    OnWarrantyTimeChanged();
                }
            }
        }
        private int _WarrantyTime;
        partial void OnWarrantyTimeChanging(int value);
        partial void OnWarrantyTimeChanged();

        [DataMemberAttribute()]
        public bool IsLocatable
        {
            get
            {
                return _IsLocatable;
            }
            set
            {
                if (_IsLocatable != value)
                {
                    OnIsLocatableChanging(value);
                    _IsLocatable = value;
                    RaisePropertyChanged("IsLocatable");
                    OnIsLocatableChanged();
                }
            }
        }
        private bool _IsLocatable;
        partial void OnIsLocatableChanging(bool value);
        partial void OnIsLocatableChanged();

        [DataMemberAttribute()]
        public bool IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                if (_IsDeleted != value)
                {
                    OnIsDeletedChanging(value);
                    _IsDeleted = value;
                    RaisePropertyChanged("IsDeleted");
                    OnIsDeletedChanged();
                }
            }
        }
        private bool _IsDeleted;
        partial void OnIsDeletedChanging(bool value);
        partial void OnIsDeletedChanged();

        [DataMemberAttribute()]
        public Lookup UnitLookup
        {
            get
            {
                return _UnitLookup;
            }
            set
            {
                if (_UnitLookup != value)
                {
                    OnUnitLookupChanging(value);
                    _UnitLookup = value;
                    RaisePropertyChanged("UnitLookup");
                    OnUnitLookupChanged();
                }
            }
        }
        private Lookup _UnitLookup;
        partial void OnUnitLookupChanging(Lookup value);
        partial void OnUnitLookupChanged();

        [DataMemberAttribute()]
        public ResourceGroup refResourceGroup
        {
            get
            {
                return _refResourceGroup;
            }
            set
            {
                if (_refResourceGroup != value)
                {
                    OnrefResourceGroupChanging(value);
                    _refResourceGroup = value;
                    RaisePropertyChanged("refResourceGroup");
                    OnrefResourceGroupChanged();
                }
            }
        }
        private ResourceGroup _refResourceGroup;
        partial void OnrefResourceGroupChanging(ResourceGroup value);
        partial void OnrefResourceGroupChanged();

        [DataMemberAttribute()]
        public ResourceType refResourceType
        {
            get
            {
                return _refResourceType;
            }
            set
            {
                if (_refResourceType != value)
                {
                    OnrefResourceTypeChanging(value);
                    _refResourceType = value;
                    RaisePropertyChanged("refResourceType");
                    OnrefResourceTypeChanged();
                }
            }
        }
        private ResourceType _refResourceType;
        partial void OnrefResourceTypeChanging(ResourceType value);
        partial void OnrefResourceTypeChanged();

        [DataMemberAttribute()]
        public long V_ExpenditureSource
        {
            get
            {
                return _V_ExpenditureSource;
            }
            set
            {
                _V_ExpenditureSource = value;
                RaisePropertyChanged("V_ExpenditureSource");
            }
        }
        private long _V_ExpenditureSource;
        //▼====#001 29/05/2018 TTM: Thêm property mới HIRepResourceCode
        [DataMemberAttribute()]
        public string HIRepResourceCode
        {
            get
            {
                return _HIRepResourceCode;
            }
            set
            {
                if (_HIRepResourceCode != value)
                {
                    _HIRepResourceCode = value;
                    RaisePropertyChanged("HIRepResourceCode");
                }
            }
        }
        private string _HIRepResourceCode;
        //▲====#001
        //▼====#002
        [DataMemberAttribute()]
        public long? V_PCLMainCategory
        {
            get
            {
                return _V_PCLMainCategory;
            }
            set
            {
                _V_PCLMainCategory = value;
                RaisePropertyChanged("V_PCLMainCategory");
            }
        }
        private long? _V_PCLMainCategory;

        //▲====#002

        //▼====#003
        [DataMemberAttribute()]
        public string RsrcHisCode
        {
            get
            {
                return _RsrcHisCode;
            }
            set
            {
                _RsrcHisCode = value;
                RaisePropertyChanged("RsrcHisCode");
            }
        }
        private string _RsrcHisCode;

        [DataMemberAttribute()]
        public ObservableCollection<ResourceTypeLists> RscrTypeLists
        {
            get
            {
                return _RscrTypeLists;
            }
            set
            {
                _RscrTypeLists = value;
                RaisePropertyChanged("RscrTypeLists");
            }
        }
        private ObservableCollection<ResourceTypeLists> _RscrTypeLists;

        [DataMemberAttribute()]
        public string HisItemName
        {
            get
            {
                return _HisItemName;
            }
            set
            {
                _HisItemName = value;
                RaisePropertyChanged("HisItemName");
            }
        }
        private string _HisItemName;

        [DataMemberAttribute()]
        public long UseForDeptID
        {
            get
            {
                return _UseForDeptID;
            }
            set
            {
                _UseForDeptID = value;
                RaisePropertyChanged("UseForDeptID");
            }
        }
        private long _UseForDeptID;

        [DataMemberAttribute()]
        public DateTime? ManufactureDate
        {
            get
            {
                return _ManufactureDate;
            }
            set
            {
                _ManufactureDate = value;
                RaisePropertyChanged("ManufactureDate");
            }
        }
        private DateTime? _ManufactureDate;

        [DataMemberAttribute()]
        public DateTime? UseDate
        {
            get
            {
                return _UseDate;
            }
            set
            {
                _UseDate = value;
                RaisePropertyChanged("UseDate");
            }
        }
        private DateTime? _UseDate;

        [DataMemberAttribute()]
        public long Manufacturer
        {
            get
            {
                return _Manufacturer;
            }
            set
            {
                _Manufacturer = value;
                RaisePropertyChanged("Manufacturer");
            }
        }
        private long _Manufacturer;

        [DataMemberAttribute()]
        public string ManufacturerStr
        {
            get
            {
                return _ManufacturerStr;
            }
            set
            {
                _ManufacturerStr = value;
                RaisePropertyChanged("ManufacturerStr");
            }
        }
        private string _ManufacturerStr;

        [DataMemberAttribute()]
        public long ManufactureCountry
        {
            get
            {
                return _ManufactureCountry;
            }
            set
            {
                _ManufactureCountry = value;
                RaisePropertyChanged("ManufactureCountry");
            }
        }
        private long _ManufactureCountry;

        [DataMemberAttribute()]
        public string CirculationNumber
        {
            get
            {
                return _CirculationNumber;
            }
            set
            {
                _CirculationNumber = value;
                RaisePropertyChanged("CirculationNumber");
            }
        }
        private string _CirculationNumber;

        [DataMemberAttribute()]
        public DateTime? ContractFrom
        {
            get
            {
                return _ContractFrom;
            }
            set
            {
                _ContractFrom = value;
                RaisePropertyChanged("ContractFrom");
            }
        }
        private DateTime? _ContractFrom;

        [DataMemberAttribute()]
        public DateTime? ContractTo
        {
            get
            {
                return _ContractTo;
            }
            set
            {
                _ContractTo = value;
                RaisePropertyChanged("ContractTo");
            }
        }
        private DateTime? _ContractTo;

        [DataMemberAttribute()]
        public string ContractNumber
        {
            get
            {
                return _ContractNumber;
            }
            set
            {
                _ContractNumber = value;
                RaisePropertyChanged("ContractNumber");
            }
        }
        private string _ContractNumber;

        [DataMemberAttribute()]
        public string SeriNumber
        {
            get
            {
                return _SeriNumber;
            }
            set
            {
                _SeriNumber = value;
                RaisePropertyChanged("SeriNumber");
            }
        }
        private string _SeriNumber;

        [DataMemberAttribute()]
        public Lookup V_RscrStatus
        {
            get
            {
                return _V_RscrStatus;
            }
            set
            {
                _V_RscrStatus = value;
                RaisePropertyChanged("V_RscrStatus");
            }
        }
        private Lookup _V_RscrStatus;

        [DataMemberAttribute()]
        public string log
        {
            get
            {
                return _log;
            }
            set
            {
                _log = value;
                RaisePropertyChanged("log");
            }
        }
        private string _log;
        
        [DataMemberAttribute]
        public long CreatedStaffID
        {
            get
            {
                return _CreatedStaffID;
            }
            set
            {
                if (_CreatedStaffID == value)
                {
                    return;
                }
                _CreatedStaffID = value;
                RaisePropertyChanged("CreatedStaffID");
            }
        }
        private long _CreatedStaffID;

        [DataMemberAttribute]
        public DateTime CreatedDate
        {
            get
            {
                return _CreatedDate;
            }
            set
            {
                if (_CreatedDate == value)
                {
                    return;
                }
                _CreatedDate = value;
                RaisePropertyChanged("CreatedDate");
            }
        }
        private DateTime _CreatedDate;

        [DataMemberAttribute]
        public long LastUpdateStaffID
        {
            get
            {
                return _LastUpdateStaffID;
            }
            set
            {
                if (_LastUpdateStaffID == value)
                {
                    return;
                }
                _LastUpdateStaffID = value;
                RaisePropertyChanged("LastUpdateStaffID");
            }
        }
        private long _LastUpdateStaffID;

        [DataMemberAttribute]
        public DateTime LastUpdateDate
        {
            get
            {
                return _LastUpdateDate;
            }
            set
            {
                if (_LastUpdateDate == value)
                {
                    return;
                }
                _LastUpdateDate = value;
                RaisePropertyChanged("LastUpdateDate");
            }
        }
        private DateTime _LastUpdateDate;
               
        [DataMemberAttribute]
        public long STT
        {
            get
            {
                return _STT;
            }
            set
            {
                if (_STT == value)
                {
                    return;
                }
                _STT = value;
                RaisePropertyChanged("STT");
            }
        }
        private long _STT;
        //▲====#003
        //▼==== #004
        [DataMemberAttribute()]
        public string ResourceCode
        {
            get
            {
                return _ResourceCode;
            }
            set
            {
                if (_ResourceCode != value)
                {
                    _ResourceCode = value;
                    RaisePropertyChanged("ResourceCode");
                }
            }
        }
        private string _ResourceCode;
        //▲==== #004
        //▼==== #005
        [DataMemberAttribute()]
        public string ItemNameEng
        {
            get
            {
                return _ItemNameEng;
            }
            set
            {
                if (_ItemNameEng != value)
                {
                    OnItemNameEngChanging(value);
                    ValidateProperty("ItemNameEng", value);
                    _ItemNameEng = value;
                    RaisePropertyChanged("ItemNameEng");
                    OnItemNameEngChanged();
                }
            }
        }
        private string _ItemNameEng;
        partial void OnItemNameEngChanging(string value);
        partial void OnItemNameEngChanged();
        //▲==== #005

        #endregion
        #region Navigation Properties
        [DataMemberAttribute()]
        //[Required(ErrorMessage = "Bạn phải chon Loai Vật Tư!")]
        public ResourceType VResourceType
        {
            get
            {
                return _VResourceType;
            }
            set
            {
                if (_VResourceType != value)
                {
                    OnVResourceTypeChanging(value);
                    _VResourceType = value;
                    RaisePropertyChanged("VResourceType");
                    RscrTypeID = VResourceType.RscrTypeID;
                    OnVResourceTypeChanged();
                }
            }
        }
        private ResourceType _VResourceType;
        partial void OnVResourceTypeChanging(ResourceType value);
        partial void OnVResourceTypeChanged();

        [DataMemberAttribute()]
        //[Required(ErrorMessage = "Bạn phải chon Nhom Vật Tư!")]
        public ResourceGroup VResourceGroup
        {
            get
            {
                return _VResourceGroup;
            }
            set
            {
                if (_VResourceGroup != value)
                {
                    OnVResourceGroupChanging(value);
                    _VResourceGroup = value;
                    RaisePropertyChanged("VResourceGroup");
                    RscrGroupID=VResourceGroup.RscrGroupID;
                    OnVResourceGroupChanged();
                }
            }
        }
        private ResourceGroup _VResourceGroup;
        partial void OnVResourceGroupChanging(ResourceGroup value);
        partial void OnVResourceGroupChanged();


        [DataMemberAttribute()]
        public Supplier VSupplier
        {
            get
            {
                return _VSupplier;
            }
            set
            {
                if (_VSupplier != value)
                {
                    OnVSupplierChanging(value);
                    _VSupplier = value;
                    RaisePropertyChanged("VSupplier");
                    SupplierID=VSupplier.SupplierID;
                    OnVSupplierChanged();
                }
            }
        }
        private Supplier _VSupplier;
        partial void OnVSupplierChanging(Supplier value);
        partial void OnVSupplierChanged();
        [DataMemberAttribute()]
        public Lookup V_UnitLookup
        {
            get
            {
                return _V_UnitLookup;
            }
            set
            {
                if (_V_UnitLookup != value)
                {
                    OnV_UnitLookupChanging(value);
                    _V_UnitLookup = value;
                    V_RscrUnit = V_UnitLookup.LookupID;
                    RaisePropertyChanged("V_UnitLookup");
                    OnV_UnitLookupChanged();
                }
            }
        }
        private Lookup _V_UnitLookup;
        partial void OnV_UnitLookupChanging(Lookup value);
        partial void OnV_UnitLookupChanged();

        #endregion

        public override bool Equals(object obj)
        {
            Resources info = obj as Resources;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.RscrID > 0 && this.RscrID == info.RscrID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
