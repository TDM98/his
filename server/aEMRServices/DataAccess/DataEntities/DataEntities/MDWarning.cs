using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{

    public partial class MDWarning : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new MDWarning object.

        /// <param name="wItemID">Initial value of the WItemID property.</param>
        /// <param name="warningItems">Initial value of the WarningItems property.</param>
        public static MDWarning CreateMDWarning(long wItemID, String warningItems)
        {
            MDWarning mDWarning = new MDWarning();
            mDWarning.WItemID = wItemID;
            mDWarning.WarningItems = warningItems;
            return mDWarning;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long WItemID
        {
            get
            {
                return _WItemID;
            }
            set
            {
                if (_WItemID != value)
                {
                    OnWItemIDChanging(value);
                    _WItemID = value;
                    RaisePropertyChanged("WItemID");
                    OnWItemIDChanged();
                }
            }
        }
        private long _WItemID;
        partial void OnWItemIDChanging(long value);
        partial void OnWItemIDChanged();


        [DataMemberAttribute()]
        public DateTime RecCreatedDate
        {
            get
            {
                return _RecCreatedDate;
            }
            set
            {
                if (_RecCreatedDate != value)
                {
                    OnRecCreatedDateChanging(value);
                    _RecCreatedDate = value;
                    RaisePropertyChanged("RecCreatedDate");
                    OnRecCreatedDateChanged();
                }
            }
        }
        private DateTime _RecCreatedDate;
        partial void OnRecCreatedDateChanging(DateTime value);
        partial void OnRecCreatedDateChanged();


        [DataMemberAttribute()]
        public long PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                if (_PatientID != value)
                {
                    OnPatientIDChanging(value);
                    _PatientID = value;
                    RaisePropertyChanged("PatientID");
                    OnPatientIDChanged();
                }
            }
        }
        private long _PatientID;
        partial void OnPatientIDChanging(long value);
        partial void OnPatientIDChanged();


        [DataMemberAttribute()]
        public long StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                if (_StaffID != value)
                {
                    OnStaffIDChanging(value);
                    _StaffID = value;
                    RaisePropertyChanged("StaffID");
                    OnStaffIDChanged();
                }
            }
        }
        private long _StaffID;
        partial void OnStaffIDChanging(long value);
        partial void OnStaffIDChanged();


        [DataMemberAttribute()]
        public String WarningItems
        {
            get
            {
                return _WarningItems;
            }
            set
            {
                OnWarningItemsChanging(value);
                _WarningItems = value;
                RaisePropertyChanged("WarningItems");
                OnWarningItemsChanged();
            }
        }
        private String _WarningItems;
        partial void OnWarningItemsChanging(String value);
        partial void OnWarningItemsChanged();


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
        public DateTime DateDeleted
        {
            get
            {
                return _DateDeleted;
            }
            set
            {
                if (_DateDeleted != value)
                {
                    OnDateDeletedChanging(value);
                    _DateDeleted = value;
                    RaisePropertyChanged("DateDeleted");
                    OnDateDeletedChanged();
                }
            }
        }
        private DateTime _DateDeleted;
        partial void OnDateDeletedChanging(DateTime value);
        partial void OnDateDeletedChanged();


        #endregion
        

        #region IEditableObject Members
        private MDWarning _tempMDWarning;
        public void BeginEdit()
        {
            _tempMDWarning = (MDWarning)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempMDWarning)
                CopyFrom(_tempMDWarning);
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(MDWarning p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion

    }
}
