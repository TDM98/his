using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using eHCMS.Services.Core;
using eHCMS.Services.Core.Base;
namespace DataEntities
{
    public partial class ResourceAllocations: EntityBase, IEditableObject 
    {
        public ResourceAllocations() : base() 
        {
        
        }
        private ResourceAllocations _tempResources;
    #region IEditableObject Members
        public void BeginEdit()
        {
            _tempResources = (ResourceAllocations)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempResources)
                CopyFrom(_tempResources);            
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(ResourceAllocations p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }
        
        #endregion
        #region Factory Method

        /// Create a new HospitalizationHistory object.

        public static ResourceAllocations CreateResources(
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
                                               ,bool IsDeleted)
        {
            ResourceAllocations resourceAllocations= new ResourceAllocations();
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
            return resourceAllocations;
        }
#endregion
           
        #region Primitive Properties

        [DataMemberAttribute()]
        public long RscrAllocID
        {
            get
            {
                return _RscrAllocID;
            }
            set
            {
                if (_RscrAllocID != value)
                {
                    OnRscrAllocIDChanging(value);
                    _RscrAllocID = value;
                    RaisePropertyChanged("RscrAllocID");
                    OnRscrAllocIDChanged();
                }
            }
        }
        private long _RscrAllocID;
        partial void OnRscrAllocIDChanging(long value);
        partial void OnRscrAllocIDChanged();

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
        public long DeptLocationID
        {
            get
            {
                return _DeptLocationID;
            }
            set
            {
                if (_DeptLocationID != value)
                {
                    OnDeptLocationIDChanging(value);
                    _DeptLocationID = value;
                    RaisePropertyChanged("DeptLocationID");
                    OnDeptLocationIDChanged();
                }
            }
        }
        private long _DeptLocationID;
        partial void OnDeptLocationIDChanging(long value);
        partial void OnDeptLocationIDChanged();


         


         [DataMemberAttribute()]
        public long RscrMoveRequestID
        {
            get
            {
                return _RscrMoveRequestID;
            }
            set
            {
                if (_RscrMoveRequestID != value)
                {
                    OnRscrMoveRequestIDChanging(value);
                    _RscrMoveRequestID = value;
                    RaisePropertyChanged("RscrMoveRequestID");
                    OnRscrMoveRequestIDChanged();
                }
            }
        }
        private long _RscrMoveRequestID;
        partial void OnRscrMoveRequestIDChanging(long value);
        partial void OnRscrMoveRequestIDChanged();


         [DataMemberAttribute()]
        public long AllocStaffID
        {
            get
            {
                return _AllocStaffID;
            }
            set
            {
                if (_AllocStaffID != value)
                {
                    OnAllocStaffIDChanging(value);
                    _AllocStaffID = value;
                    RaisePropertyChanged("AllocStaffID");
                    OnAllocStaffIDChanged();
                }
            }
        }
        private long _AllocStaffID;
        partial void OnAllocStaffIDChanging(long value);
        partial void OnAllocStaffIDChanged();


         [DataMemberAttribute()]
        public Nullable<DateTime> AllocDate
        {
            get
            {
                return _AllocDate;
            }
            set
            {
                if (_AllocDate != value)
                {
                    OnAllocDateChanging(value);
                    _AllocDate = value;
                    RaisePropertyChanged("AllocDate");
                    OnAllocDateChanged();
                }
            }
        }
         private Nullable<DateTime> _AllocDate;
         partial void OnAllocDateChanging(Nullable<DateTime> value);
        partial void OnAllocDateChanged();


         [DataMemberAttribute()]
        public Nullable<DateTime> StartUseDate
        {
            get
            {
                return _StartUseDate;
            }
            set
            {
                if (_StartUseDate != value)
                {
                    OnStartUseDateChanging(value);
                    _StartUseDate = value;
                    RaisePropertyChanged("StartUseDate");
                    OnStartUseDateChanged();
                }
            }
        }
         private Nullable<DateTime> _StartUseDate;
         partial void OnStartUseDateChanging(Nullable<DateTime> value);
        partial void OnStartUseDateChanged();


         [DataMemberAttribute()]
        public long  V_AllocStatus
        {
            get
            {
                return _V_AllocStatus;
            }
            set
            {
                if (_V_AllocStatus != value)
                {
                    OnV_AllocStatusChanging(value);
                    _V_AllocStatus = value;
                    RaisePropertyChanged("V_AllocStatus");
                    OnV_AllocStatusChanged();
                }
            }
        }
        private long _V_AllocStatus;
        partial void OnV_AllocStatusChanging(long value);
        partial void OnV_AllocStatusChanged();


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
        public int QtyInUse
        {
            get
            {
                return _QtyInUse;
            }
            set
            {
                if (_QtyInUse != value)
                {
                    OnQtyInUseChanging(value);
                    _QtyInUse = value;
                    RaisePropertyChanged("QtyInUse");
                    OnQtyInUseChanged();
                }
            }
        }
        private int _QtyInUse=1;
        partial void OnQtyInUseChanging(int value);
        partial void OnQtyInUseChanged();

        [DataMemberAttribute()]
        public long ResponsibleStaffID
        {
            get
            {
                return _ResponsibleStaffID;
            }
            set
            {
                if (_ResponsibleStaffID != value)
                {
                    OnResponsibleStaffIDChanging(value);
                    _ResponsibleStaffID = value;
                    RaisePropertyChanged("ResponsibleStaffID");
                    OnResponsibleStaffIDChanged();
                }
            }
        }
        private long _ResponsibleStaffID;
        partial void OnResponsibleStaffIDChanging(long value);
        partial void OnResponsibleStaffIDChanged();

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

        [DataMemberAttribute()]
        public DeptLocation VDeptLocation
        {
            get
            {
                return _VDeptLocation;
            }
            set
            {
                if (_VDeptLocation != value)
                {
                    OnVDeptLocationChanging(value);
                    _VDeptLocation = value;
                    RaisePropertyChanged("VDeptLocation");
                    OnVDeptLocationChanged();
                }
            }
        }
        private DeptLocation _VDeptLocation;
        partial void OnVDeptLocationChanging(DeptLocation value);
        partial void OnVDeptLocationChanged();

        [DataMemberAttribute()]
        public ResourceMoveRequests VRscrMoveRequest
        {
            get
            {
                return _VRscrMoveRequest;
            }
            set
            {
                if (_VRscrMoveRequest != value)
                {
                    OnVRscrMoveRequestChanging(value);
                    _VRscrMoveRequest = value;
                    RaisePropertyChanged("VRscrMoveRequest");
                    OnVRscrMoveRequestChanged();
                }
            }
        }
        private ResourceMoveRequests _VRscrMoveRequest;
        partial void OnVRscrMoveRequestChanging(ResourceMoveRequests value);
        partial void OnVRscrMoveRequestChanged();

        [DataMemberAttribute()]
        public Staff VAllocStaff
        {
            get
            {
                return _VAllocStaff;
            }
            set
            {
                if (_VAllocStaff != value)
                {
                    OnVAllocStaffChanging(value);
                    _VAllocStaff = value;
                    RaisePropertyChanged("VAllocStaff");
                    OnVAllocStaffChanged();
                }
            }
        }
        private Staff _VAllocStaff;
        partial void OnVAllocStaffChanging(Staff value);
        partial void OnVAllocStaffChanged();


        [DataMemberAttribute()]
        public Lookup VAllocStatus
        {
            get
            {
                return _VAllocStatus;
            }
            set
            {
                if (_VAllocStatus != value)
                {
                    OnVAllocStatusChanging(value);
                    _VAllocStatus = value;
                    RaisePropertyChanged("VAllocStatus");
                    OnVAllocStatusChanged();
                }
            }
        }
        private Lookup _VAllocStatus;
        partial void OnVAllocStatusChanging(Lookup value);
        partial void OnVAllocStatusChanged();

        [DataMemberAttribute()]
        public Staff VResponsibleStaff
        {
            get
            {
                return _VResponsibleStaff;
            }
            set
            {
                if (_VResponsibleStaff != value)
                {
                    OnVResponsibleStaffChanging(value);
                    _VResponsibleStaff = value;
                    RaisePropertyChanged("VResponsibleStaff");
                    OnVResponsibleStaffChanged();
                }
            }
        }
        private Staff _VResponsibleStaff;
        partial void OnVResponsibleStaffChanging(Staff value);
        partial void OnVResponsibleStaffChanged();

        #endregion
    }


}
