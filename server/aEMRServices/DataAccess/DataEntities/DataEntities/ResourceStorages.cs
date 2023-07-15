using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using eHCMS.Services.Core;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public partial class ResourceStorages : EntityBase, IEditableObject 
    {
        public ResourceStorages()
            : base() 
        {
        
        }
        private ResourceStorages _tempResources;
    #region IEditableObject Members
        public void BeginEdit()
        {
            _tempResources = (ResourceStorages)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempResources)
                CopyFrom(_tempResources);            
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(ResourceStorages p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }
        
        #endregion
        #region Factory Method

        /// Create a new HospitalizationHistory object.

        public static ResourceStorages CreateResourceStorages(
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
            ResourceStorages resourceStorages = new ResourceStorages();
            //resources.RscrID = RscrID;                   
            
            return resourceStorages;
        }
#endregion
        #region Primitive Properties
        [DataMemberAttribute()]
        public long RscrStorageID
        {
            get
            {
                return _RscrStorageID;
            }
            set
            {
                if (_RscrStorageID != value)
                {
                    OnRscrStorageIDChanging(value);
                    _RscrStorageID = value;
                    RaisePropertyChanged("RscrStorageID");
                    OnRscrStorageIDChanged();
                }
            }
        }
        private long _RscrStorageID;
        partial void OnRscrStorageIDChanging(long value);
        partial void OnRscrStorageIDChanged(); 

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
        public long StorageStaffID
        {
            get
            {
                return _StorageStaffID;
            }
            set
            {
                if (_StorageStaffID != value)
                {
                    OnStorageStaffIDChanging(value);
                    _StorageStaffID = value;
                    RaisePropertyChanged("StorageStaffID");
                    OnStorageStaffIDChanged();
                }
            }
        }
        private long _StorageStaffID;
        partial void OnStorageStaffIDChanging(long value);
        partial void OnStorageStaffIDChanged(); 

        [DataMemberAttribute()]
        public Nullable<DateTime> StorageDate
        {
            get
            {
                return _StorageDate;
            }
            set
            {
                if (_StorageDate != value)
                {
                    OnStorageDateChanging(value);
                    _StorageDate = value;
                    RaisePropertyChanged("StorageDate");
                    OnStorageDateChanged();
                }
            }
        }
        private Nullable<DateTime> _StorageDate;
        partial void OnStorageDateChanging(Nullable<DateTime> value);
        partial void OnStorageDateChanged(); 

               [DataMemberAttribute()]
        public long V_StorageStatus
        {
            get
            {
                return _V_StorageStatus;
            }
            set
            {
                if (_V_StorageStatus != value)
                {
                    OnV_StorageStatusChanging(value);
                    _V_StorageStatus = value;
                    RaisePropertyChanged("V_StorageStatus");
                    OnV_StorageStatusChanged();
                }
            }
        }
        private long _V_StorageStatus;
        partial void OnV_StorageStatusChanging(long value);
        partial void OnV_StorageStatusChanged(); 

           
               [DataMemberAttribute()]
        public long V_StorageReason
        {
            get
            {
                return _V_StorageReason;
            }
            set
            {
                if (_V_StorageReason != value)
                {
                    OnV_StorageReasonChanging(value);
                    _V_StorageReason = value;
                    RaisePropertyChanged("V_StorageReason");
                    OnV_StorageReasonChanged();
                }
            }
        }
        private long _V_StorageReason;
        partial void OnV_StorageReasonChanging(long value);
        partial void OnV_StorageReasonChanged(); 

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
        public int QtyStorage
        {
            get
            {
                return _QtyStorage;
            }
            set
            {
                if (_QtyStorage != value)
                {
                    OnQtyStorageChanging(value);
                    _QtyStorage = value;
                    RaisePropertyChanged("QtyStorage");
                    OnQtyStorageChanged();
                }
            }
        }
        private int _QtyStorage=1;
        partial void OnQtyStorageChanging(int value);
        partial void OnQtyStorageChanged();   
           
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
        public Staff VStorageStaff
        {
            get
            {
                return _VStorageStaff;
            }
            set
            {
                if (_VStorageStaff != value)
                {
                    OnVStorageStaffChanging(value);
                    _VStorageStaff = value;
                    RaisePropertyChanged("VStorageStaff");
                    OnVStorageStaffChanged();
                }
            }
        }
        private Staff _VStorageStaff;
        partial void OnVStorageStaffChanging(Staff value);
        partial void OnVStorageStaffChanged();

        [DataMemberAttribute()]
        public Lookup VStorageStatus
        {
            get
            {
                return _VStorageStatus;
            }
            set
            {
                if (_VStorageStatus != value)
                {
                    OnVStorageStatusChanging(value);
                    _VStorageStatus = value;
                    RaisePropertyChanged("VStorageStatus");
                    OnVStorageStatusChanged();
                }
            }
        }
        private Lookup _VStorageStatus;
        partial void OnVStorageStatusChanging(Lookup value);
        partial void OnVStorageStatusChanged();

        [DataMemberAttribute()]
        public Lookup VStorageReason
        {
            get
            {
                return _VStorageReason;
            }
            set
            {
                if (_VStorageReason != value)
                {
                    OnVStorageReasonChanging(value);
                    _VStorageReason = value;
                    RaisePropertyChanged("VStorageReason");
                    OnVStorageReasonChanged();
                }
            }
        }
        private Lookup _VStorageReason;
        partial void OnVStorageReasonChanging(Lookup value);
        partial void OnVStorageReasonChanged();

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
