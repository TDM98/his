using System;
using System.Net;
using System.Windows;
using eHCMS.Services.Core.Base;
using System.Runtime.Serialization;

namespace DataEntities
{
    public partial class ResourceMaintenanceLogStatus : NotifyChangedBase
    {
        [DataMemberAttribute()]        
        public long RscrMainLogStatusID
        {
            get { return _RscrMainLogStatusID; }
            set 
            {
                if (_RscrMainLogStatusID != value)
                {
                    OnRscrMainLogStatusIDChanging(value);
                    _RscrMainLogStatusID = value;
                    RaisePropertyChanged("RscrMainLogStatusID");
                    OnRscrMainLogStatusIDChanged();
                }
            }
        }
        private long _RscrMainLogStatusID;
        partial void OnRscrMainLogStatusIDChanging(Int64 value);
        partial void OnRscrMainLogStatusIDChanged();


        [DataMemberAttribute()]
        public long RscrMaintLogID
        {
            get { return _RscrMaintLogID; }
            set 
            {
                if (_RscrMaintLogID != value)
                {
                    OnRscrMaintLogIDChanging(value);
                    _RscrMaintLogID = value;
                    RaisePropertyChanged("RscrMaintLogID");
                    OnRscrMaintLogIDChanged();
                }
            }
        }
        private long _RscrMaintLogID;
        partial void OnRscrMaintLogIDChanging(Int64 value);
        partial void OnRscrMaintLogIDChanged();


      [DataMemberAttribute()]
        public Nullable<DateTime> RecDateCreated 
        {
            get { return _RecDateCreated ; }
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
      private Nullable<DateTime> _RecDateCreated;
      partial void OnRecDateCreatedChanging(Nullable<DateTime> value);
        partial void OnRecDateCreatedChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> StatusChangeDate
        {
            get { return _StatusChangeDate; }
            set 
            {
                if (_StatusChangeDate != value)
                {
                    OnStatusChangeDateChanging(value);
                    _StatusChangeDate = value;
                    RaisePropertyChanged("StatusChangeDate");
                    OnStatusChangeDateChanged();
                }
            }
        }
        private Nullable<DateTime> _StatusChangeDate;
        partial void OnStatusChangeDateChanging(Nullable<DateTime> value);
        partial void OnStatusChangeDateChanged();

        [DataMemberAttribute()]
        public long UpdateStatusStaffID
        {
            get { return _UpdateStatusStaffID; }
            set 
            {
                if (_UpdateStatusStaffID != value)
                {
                    OnUpdateStatusStaffIDChanging(value);
                    _UpdateStatusStaffID = value;
                    RaisePropertyChanged("UpdateStatusStaffID");
                    OnUpdateStatusStaffIDChanged();
                }
            }
        }
        private long _UpdateStatusStaffID;
        partial void OnUpdateStatusStaffIDChanging(long value);
        partial void OnUpdateStatusStaffIDChanged();


        [DataMemberAttribute()]
        public long V_CurrentStatus
        {
            get { return _V_CurrentStatus; }
            set 
            {
                if (_V_CurrentStatus != value)
                {
                    OnV_CurrentStatusChanging(value);
                    _V_CurrentStatus = value;
                    RaisePropertyChanged("V_CurrentStatus");
                    OnV_CurrentStatusChanged();
                }
            }
        }
        private long _V_CurrentStatus;
        partial void OnV_CurrentStatusChanging(long value);
        partial void OnV_CurrentStatusChanged();

        [DataMemberAttribute()]
        public bool IsActive
        {
            get { return _IsActive; }
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
            get { return _IsDeleted; }
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

        
        #region Navigation Properties
        [DataMemberAttribute()]
        public Lookup VCurrentStatus
        {
            get { return _VCurrentStatus; }
            set
            {
                if (VCurrentStatus != value)
                {
                    OnVCurrentStatusChanging(value);
                    _VCurrentStatus = value;
                    RaisePropertyChanged("VCurrentStatus");
                    OnVCurrentStatusChanged();
                }
            }
        }
        private Lookup _VCurrentStatus;
        partial void OnVCurrentStatusChanging(Lookup value);
        partial void OnVCurrentStatusChanged();

        [DataMemberAttribute()]
        public Staff VUpdateStaff
        {
            get { return _VUpdateStaff; }
            set
            {
                if (_VUpdateStaff != value)
                {
                    OnVUpdateStaffChanging(value);
                    _VUpdateStaff = value;
                    RaisePropertyChanged("VUpdateStaff");
                    OnVUpdateStaffChanged();
                }
            }
        }
        private Staff _VUpdateStaff;
        partial void OnVUpdateStaffChanging(Staff value);
        partial void OnVUpdateStaffChanged();
        #endregion
    }

}
