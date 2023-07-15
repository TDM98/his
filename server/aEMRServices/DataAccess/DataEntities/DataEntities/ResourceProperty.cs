using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using eHCMS.Services.Core;
using eHCMS.Services.Core.Base;
namespace DataEntities
{
    public partial class ResourceProperty: EntityBase, IEditableObject 
    {
        public ResourceProperty()
            : base() 
        {
        
        }
        private ResourceProperty _tempResources;
    #region IEditableObject Members
        public void BeginEdit()
        {
            _tempResources = (ResourceProperty)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempResources)
                CopyFrom(_tempResources);            
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(ResourceProperty p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }
        
        #endregion
        #region Factory Method

        /// Create a new HospitalizationHistory object.

        public static ResourceProperty CreateResources(                                                
                                               bool IsDeleted)
        {
            ResourceProperty resourceProperty = new ResourceProperty();
            //resources.RscrID = RscrID;                   
            //resources.DeprecTypeID=DeprecTypeID;
            //resources.RscrTypeID =RscrTypeID;
            //resources.SupplierID =SupplierID;
            //resources.ItemName =ItemName;
            //resources.ItemBrand=ItemBrand;
            //resources.Functions = Functions;
            //resources.GeneralTechInfo = GeneralTechInfo;
            //resources.DepreciationByTimeRate = DepreciationByTimeRate;
            //resources.DepreciationByUsageRate = DepreciationByUsageRate;
            //resources.BuyPrice = BuyPrice;
            //resources.V_RscrUnit = V_RscrUnit;
            //resources.OnHIApprovedList = OnHIApprovedList;
            //resources.WarrantyTime = WarrantyTime;
            //resources.IsLocatable = IsLocatable;
            //resources.IsDeleted = IsDeleted;
            return resourceProperty;
        }
#endregion
           
        #region Primitive Properties

        [DataMemberAttribute()]
        public long RscrPropertyID
        {
            get
            {
                return _RscrPropertyID;
            }
            set
            {
                if (_RscrPropertyID != value)
                {
                    OnRscrPropertyIDChanging(value);
                    _RscrPropertyID = value;
                    RaisePropertyChanged("RscrPropertyID");
                    OnRscrPropertyIDChanged();
                }
            }
        }
        private long _RscrPropertyID;
        partial void OnRscrPropertyIDChanging(long value);
        partial void OnRscrPropertyIDChanged();

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
        public string RscrGUID
        {
            get
            {
                return _RscrGUID;
            }
            set
            {
                if (_RscrGUID != value)
                {
                    OnRscrGUIDChanging(value);
                    _RscrGUID = value;
                    RaisePropertyChanged("RscrGUID");
                    OnRscrGUIDChanged();
                }
            }
        }
        private string _RscrGUID;
        partial void OnRscrGUIDChanging(string value);
        partial void OnRscrGUIDChanged();
        

        [DataMemberAttribute()]
        public DateTime RecDateCreated
        {
            get
            {
                return _RecDateCreated;
            }
            set
            {
                if (_RecDateCreated != value)
                {
                    OnRecDateCreatedChanging(value);
                    _RecDateCreated = value;
                    RaisePropertyChanged("RecDateCreated");
                    OnRecDateCreatedChanged();
                }
            }
        }
        private DateTime _RecDateCreated;
        partial void OnRecDateCreatedChanging(DateTime value);
        partial void OnRecDateCreatedChanged();
        
        
         [DataMemberAttribute()]
        public bool HasIdentity
        {
            get
            {
                return _HasIdentity;
            }
            set
            {
                if (_HasIdentity != value)
                {
                    OnHasIdentityChanging(value);
                    _HasIdentity = value;
                    RaisePropertyChanged("HasIdentity");
                    OnHasIdentityChanged();
                }
            }
        }
        private bool _HasIdentity;
        partial void OnHasIdentityChanging(bool value);
        partial void OnHasIdentityChanged();
   
           
        [DataMemberAttribute()]
        public string RscrCode
        {
            get
            {
                return _RscrCode;
            }
            set
            {
                if (_RscrCode != value)
                {
                    OnRscrCodeChanging(value);
                    _RscrCode = value;
                    RaisePropertyChanged("RscrCode");
                    OnRscrCodeChanged();
                }
            }
        }
        private string _RscrCode;
        partial void OnRscrCodeChanging(string value);
        partial void OnRscrCodeChanged();

        [DataMemberAttribute()]
        public string RscrBarcode
        {
            get
            {
                return _RscrBarcode;
            }
            set
            {
                if (_RscrBarcode != value)
                {
                    OnRscrBarcodeChanging(value);
                    _RscrBarcode = value;
                    RaisePropertyChanged("RscrBarcode");
                    OnRscrBarcodeChanged();
                }
            }
        }
        private string _RscrBarcode;
        partial void OnRscrBarcodeChanging(string value);
        partial void OnRscrBarcodeChanged();

        [DataMemberAttribute()]
        public string SerialNumber
        {
            get
            {
                return _SerialNumber;
            }
            set
            {
                if (_SerialNumber != value)
                {
                    OnSerialNumberChanging(value);
                    _SerialNumber = value;
                    RaisePropertyChanged("SerialNumber");
                    OnSerialNumberChanged();
                }
            }
        }
        private string _SerialNumber;
        partial void OnSerialNumberChanging(string value);
        partial void OnSerialNumberChanged();

        [DataMemberAttribute()]
        [Required(ErrorMessage = "Bạn phải so luong Vật Tư!")]
        [Range(1, 99999999999.0, ErrorMessage = "So luong không được nhỏ hơn 0")]
        public int QtyAlloc
        {
            get
            {
                return _QtyAlloc;
            }
            set
            {
                if (_QtyAlloc != value)
                {
                    OnQtyAllocChanging(value);
                    _QtyAlloc = value;
                    RaisePropertyChanged("QtyAlloc");
                    OnQtyAllocChanged();
                }
            }
        }
        private int _QtyAlloc=1;
        partial void OnQtyAllocChanging(int value);
        partial void OnQtyAllocChanged();

        [DataMemberAttribute()]
        public bool IsDelete 
        {
            get
            {
                return _IsDelete;
            }
            set
            {
                if (_IsDelete != value)
                {
                    OnIsDeleteChanging(value);
                    _IsDelete = value;
                    RaisePropertyChanged("IsDelete");
                    OnIsDeleteChanged();
                }
            }
        }
        private bool _IsDelete;
        partial void OnIsDeleteChanging(bool value);
        partial void OnIsDeleteChanged();

        [DataMemberAttribute()]
        public bool IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                if (_IsActive != value)
                {
                    OnIsActiveChanging(value);
                    _IsActive = value;
                    RaisePropertyChanged("IsActive");
                    OnIsActiveChanged();
                }
            }
        }
        private bool _IsActive;
        partial void OnIsActiveChanging(bool value);
        partial void OnIsActiveChanged();

#endregion
        #region Navigation Properties
        [DataMemberAttribute()]
        public Resources VResources
        {
            get
            {
                return _VResources;
            }
            set
            {
                if (_VResources != value)
                {
                    OnVResourcesChanging(value);
                    _VResources = value;
                    RaisePropertyChanged("VResources");
                    OnVResourcesChanged();
                }
            }
        }
        private Resources _VResources;
        partial void OnVResourcesChanging(Resources value);
        partial void OnVResourcesChanged();       

        #endregion

        public override bool Equals(object obj)
        {
            ResourceProperty info = obj as ResourceProperty;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.RscrPropertyID > 0 && this.RscrPropertyID == info.RscrPropertyID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }


}
