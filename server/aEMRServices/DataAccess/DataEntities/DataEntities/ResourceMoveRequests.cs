using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using eHCMS.Services.Core;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public partial class ResourceMoveRequests : EntityBase, IEditableObject 
    {
        public ResourceMoveRequests()
            : base() 
        {
        
        }
        private ResourceMoveRequests _tempResources;
    #region IEditableObject Members
        public void BeginEdit()
        {
            _tempResources = (ResourceMoveRequests)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempResources)
                CopyFrom(_tempResources);            
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(ResourceMoveRequests p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }
        
        #endregion
        #region Factory Method

        /// Create a new HospitalizationHistory object.

        public static ResourceMoveRequests CreateResourceMoveRequests(
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
            ResourceMoveRequests resourceMoveRequests = new ResourceMoveRequests();
            //resources.RscrID = RscrID;                   

            return resourceMoveRequests;
        }
#endregion
        #region Primitive Properties
           
           
           


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
        public long RequestStaffID
        {
            get
            {
                return _RequestStaffID;
            }
            set
            {
                if (_RequestStaffID != value)
                {
                    OnRequestStaffIDChanging(value);
                    _RequestStaffID = value;
                    RaisePropertyChanged("RequestStaffID");
                    OnRequestStaffIDChanged();
                }
            }
        }
        private long _RequestStaffID;
        partial void OnRequestStaffIDChanging(long value);
        partial void OnRequestStaffIDChanged();

        [DataMemberAttribute()]
        public string MoveReason
        {
            get
            {
                return _MoveReason;
            }
            set
            {
                if (_MoveReason != value)
                {
                    OnMoveReasonChanging(value);
                    _MoveReason = value;
                    RaisePropertyChanged("MoveReason");
                    OnMoveReasonChanged();
                }
            }
        }
        private string _MoveReason;
        partial void OnMoveReasonChanging(string value);
        partial void OnMoveReasonChanged();

        [DataMemberAttribute()]
        public DateTime EffectiveMoveDate
        {
            get
            {
                return _EffectiveMoveDate;
            }
            set
            {
                if (_EffectiveMoveDate != value)
                {
                    OnEffectiveMoveDateChanging(value);
                    _EffectiveMoveDate = value;
                    RaisePropertyChanged("EffectiveMoveDate");
                    OnEffectiveMoveDateChanged();
                }
            }
        }
        private DateTime _EffectiveMoveDate;
        partial void OnEffectiveMoveDateChanging(DateTime value);
        partial void OnEffectiveMoveDateChanged();

        [DataMemberAttribute()]
        public long FromDeptLocID
        {
            get
            {
                return _FromDeptLocID;
            }
            set
            {
                if (_FromDeptLocID != value)
                {
                    OnFromDeptLocIDChanging(value);
                    _FromDeptLocID = value;
                    RaisePropertyChanged("FromDeptLocID");
                    OnFromDeptLocIDChanged();
                }
            }
        }
        private long _FromDeptLocID;
        partial void OnFromDeptLocIDChanging(long value);
        partial void OnFromDeptLocIDChanged();

        [DataMemberAttribute()]
        public long ToDeptLocID
        {
            get
            {
                return _ToDeptLocID;
            }
            set
            {
                if (_ToDeptLocID != value)
                {
                    OnToDeptLocIDChanging(value);
                    _ToDeptLocID = value;
                    RaisePropertyChanged("ToDeptLocID");
                    OnToDeptLocIDChanged();
                }
            }
        }
        private long _ToDeptLocID;
        partial void OnToDeptLocIDChanging(long value);
        partial void OnToDeptLocIDChanged();

        [DataMemberAttribute()]
        public long ApprovedStaffID
        {
            get
            {
                return _ApprovedStaffID;
            }
            set
            {
                if (_ApprovedStaffID != value)
                {
                    OnApprovedStaffIDChanging(value);
                    _ApprovedStaffID = value;
                    RaisePropertyChanged("ApprovedStaffID");
                    OnApprovedStaffIDChanged();
                }
            }
        }
        private long _ApprovedStaffID;
        partial void OnApprovedStaffIDChanging(long value);
        partial void OnApprovedStaffIDChanged();

        [DataMemberAttribute()]
        public bool GetAll
        {
            get
            {
                return _GetAll;
            }
            set
            {
                if (_GetAll != value)
                {
                    OnGetAllChanging(value);
                    _GetAll = value;
                    RaisePropertyChanged("GetAll");
                    OnGetAllChanged();
                }
            }
        }
        private bool _GetAll;
        partial void OnGetAllChanging(bool value);
        partial void OnGetAllChanged();

        

        [DataMemberAttribute()]
        public string Note
        {
            get
            {
                return _Note;
            }
            set
            {
                if (_Note != value)
                {
                    OnNoteChanging(value);
                    _Note = value;
                    RaisePropertyChanged("Note");
                    OnNoteChanged();
                }
            }
        }
        private string _Note;
        partial void OnNoteChanging(string value);
        partial void OnNoteChanged();

        [DataMemberAttribute()]
        public int QtyAllocEx
        {
            get
            {
                return _QtyAllocEx;
            }
            set
            {
                if (_QtyAllocEx != value)
                {
                    OnQtyAllocExChanging(value);
                    _QtyAllocEx = value;
                    RaisePropertyChanged("QtyAllocEx");
                    OnQtyAllocExChanged();
                }
            }
        }
        private int _QtyAllocEx;
        partial void OnQtyAllocExChanging(int value);
        partial void OnQtyAllocExChanged();

        [DataMemberAttribute()]
        public int QtyInUseEx
        {
            get
            {
                return _QtyInUseEx;
            }
            set
            {
                if (_QtyInUseEx != value)
                {
                    OnQtyInUseExChanging(value);
                    _QtyInUseEx = value;
                    RaisePropertyChanged("QtyInUseEx");
                    OnQtyInUseExChanged();
                }
            }
        }
        private int _QtyInUseEx;
        partial void OnQtyInUseExChanging(int value);
        partial void OnQtyInUseExChanged();
        #endregion
        #region Navigation Properties
        [DataMemberAttribute()]
        public DeptLocation VFromDeptLoc
        {
            get
            {
                return _VFromDeptLoc;
            }
            set
            {
                if (_VFromDeptLoc != value)
                {
                    OnVFromDeptLocChanging(value);
                    _VFromDeptLoc = value;
                    RaisePropertyChanged("VFromDeptLoc");
                    OnVFromDeptLocChanged();
                }
            }
        }
        private DeptLocation _VFromDeptLoc;
        partial void OnVFromDeptLocChanging(DeptLocation value);
        partial void OnVFromDeptLocChanged();

        [DataMemberAttribute()]
        public DeptLocation VToDeptLoc
        {
            get
            {
                return _VToDeptLoc;
            }
            set
            {
                if (_VToDeptLoc != value)
                {
                    OnVToDeptLocChanging(value);
                    _VToDeptLoc = value;
                    RaisePropertyChanged("VToDeptLoc");
                    OnVToDeptLocChanged();
                }
            }
        }
        private DeptLocation _VToDeptLoc;
        partial void OnVToDeptLocChanging(DeptLocation value);
        partial void OnVToDeptLocChanged();
        
        
        
        [DataMemberAttribute()]
        public Staff VApprovedStaff
        {
            get
            {
                return _VApprovedStaff;
            }
            set
            {
                if (_VApprovedStaff != value)
                {
                    OnVApprovedStaffChanging(value);
                    _VApprovedStaff = value;
                    RaisePropertyChanged("VApprovedStaff");
                    OnVApprovedStaffChanged();
                }
            }
        }
        private Staff _VApprovedStaff;
        partial void OnVApprovedStaffChanging(Staff value);
        partial void OnVApprovedStaffChanged();

        [DataMemberAttribute()]
        public Staff VRequestStaff
        {
            get
            {
                return _VRequestStaff;
            }
            set
            {
                if (_VRequestStaff != value)
                {
                    OnVRequestStaffChanging(value);
                    _VRequestStaff = value;
                    RaisePropertyChanged("VRequestStaff");
                    OnVRequestStaffChanged();
                }
            }
        }
        private Staff _VRequestStaff;
        partial void OnVRequestStaffChanging(Staff value);
        partial void OnVRequestStaffChanged();


#endregion

    }
}
