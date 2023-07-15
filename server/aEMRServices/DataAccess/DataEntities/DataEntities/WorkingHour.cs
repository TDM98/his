using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class WorkingHour : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new WorkingHour object.

        /// <param name="wHID">Initial value of the WHID property.</param>
        /// <param name="cIODate">Initial value of the CIODate property.</param>
        public static WorkingHour CreateWorkingHour(long wHID, DateTime cIODate)
        {
            WorkingHour workingHour = new WorkingHour();
            workingHour.WHID = wHID;
            workingHour.CIODate = cIODate;
            return workingHour;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long WHID
        {
            get
            {
                return _WHID;
            }
            set
            {
                if (_WHID != value)
                {
                    OnWHIDChanging(value);
                    ////ReportPropertyChanging("WHID");
                    _WHID = value;
                    RaisePropertyChanged("WHID");
                    OnWHIDChanged();
                }
            }
        }
        private long _WHID;
        partial void OnWHIDChanging(long value);
        partial void OnWHIDChanged();





        [DataMemberAttribute()]
        public Nullable<Int64> StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                OnStaffIDChanging(value);
                ////ReportPropertyChanging("StaffID");
                _StaffID = value;
                RaisePropertyChanged("StaffID");
                OnStaffIDChanged();
            }
        }
        private Nullable<Int64> _StaffID;
        partial void OnStaffIDChanging(Nullable<Int64> value);
        partial void OnStaffIDChanged();





        [DataMemberAttribute()]
        public DateTime CIODate
        {
            get
            {
                return _CIODate;
            }
            set
            {
                OnCIODateChanging(value);
                ////ReportPropertyChanging("CIODate");
                _CIODate = value;
                RaisePropertyChanged("CIODate");
                OnCIODateChanged();
            }
        }
        private DateTime _CIODate;
        partial void OnCIODateChanging(DateTime value);
        partial void OnCIODateChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_WORKINGH_REL_HR13_STAFFS", "Staffs")]
        public Staff Staff
        {
            get;
            set;
        }

        #endregion
    }
}
