using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using eHCMS.Services.Core;
using eHCMS.Services.Core.Base;
using System.Reflection;


namespace DataEntities
{
    public partial class ResourcePropLocations : EntityBase, IEditableObject
    {
        public ResourcePropLocations()
            : base() 
        {
        
        }
        private ResourcePropLocations _tempResources;
    #region IEditableObject Members
        public void BeginEdit()
        {
            _tempResources = (ResourcePropLocations)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempResources)
                CopyFrom(_tempResources);            
        }

        public void EndEdit()
        {
            
        }

        public void CopyFrom(ResourcePropLocations p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }
        public static ResourcePropLocations Copy(ResourcePropLocations p)
        {
            return (ResourcePropLocations)p.MemberwiseClone();
        }
        
        public object DeepCopy(ResourcePropLocations obj)
        {
            var memberwiseClone = typeof(ResourcePropLocations).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);

            var newCopy = memberwiseClone.Invoke(obj, new ResourcePropLocations[0]);

            foreach (var field in newCopy.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (!field.FieldType.IsPrimitive && field.FieldType != typeof(string))
                {
                    var fieldCopy = DeepCopy((ResourcePropLocations)field.GetValue(newCopy));
                    field.SetValue(newCopy, fieldCopy);
                }
            }
            return newCopy;
        }
        public static object DeepCopyMine(object obj)
        {
            if (obj == null) return null;

            object newCopy;
            if (obj.GetType().IsArray)
            {
                var t = obj.GetType();
                var e = t.GetElementType();
                var r = t.GetArrayRank();
                Array a = (Array)obj;
                newCopy = Array.CreateInstance(e, a.Length);
                Array n = (Array)newCopy;
                for (int i = 0; i < a.Length; i++)
                {
                    n.SetValue(DeepCopyMine(a.GetValue(i)), i);
                }
                return newCopy;
            }
            else
            {
                newCopy = Activator.CreateInstance(obj.GetType(), true);
            }

            foreach (var field in newCopy.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (!field.FieldType.IsPrimitive && field.FieldType != typeof(string))
                {
                    var fieldCopy = DeepCopyMine(field.GetValue(obj));
                    field.SetValue(newCopy, fieldCopy);
                }
                else
                {
                    field.SetValue(newCopy, field.GetValue(obj));
                }
            }
            return newCopy;
        }

        #endregion
        #region Factory Method

        /// Create a new HospitalizationHistory object.

        public static ResourcePropLocations CreateResources(
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
            ResourcePropLocations ResourcePropLocations = new ResourcePropLocations();
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
            return ResourcePropLocations;
        }
#endregion
           
        #region Primitive Properties

        [DataMemberAttribute()]
        public long RscrPropLocID
        {
            get
            {
                return _RscrPropLocID;
            }
            set
            {
                if (_RscrPropLocID != value)
                {
                    OnRscrPropLocIDChanging(value);
                    _RscrPropLocID = value;
                    RaisePropertyChanged("RscrPropLocID");
                    OnRscrPropLocIDChanged();
                }
            }
        }
        private long _RscrPropLocID;
        partial void OnRscrPropLocIDChanging(long value);
        partial void OnRscrPropLocIDChanged();

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
        
        public int QtyEx
        {
            get
            {
                return _QtyEx;
            }
            set
            {
                if (_QtyEx != value)
                {
                    OnQtyExChanging(value);
                    _QtyEx = value;
                    RaisePropertyChanged("QtyEx");
                    OnQtyExChanged();
                }
            }
        }
        private int _QtyEx;
        partial void OnQtyExChanging(int value);
        partial void OnQtyExChanged();
         


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
         private Nullable<DateTime> _AllocDate=DateTime.Now.Date;
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
         private Nullable<DateTime> _StartUseDate=DateTime.Now.Date;
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
        public ResourceProperty VRscrProperty
        {
            get
            {
                return _VRscrProperty;
            }
            set
            {
                if (_VRscrProperty != value)
                {
                    OnVRscrPropertyChanging(value);
                    _VRscrProperty = value;
                    RaisePropertyChanged("VRscrProperty");
                    OnVRscrPropertyChanged();
                }
            }
        }
        private ResourceProperty _VRscrProperty;
        partial void OnVRscrPropertyChanging(ResourceProperty value);
        partial void OnVRscrPropertyChanged();
        
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

        public override bool Equals(object obj)
        {
            ResourcePropLocations info = obj as ResourcePropLocations;
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
