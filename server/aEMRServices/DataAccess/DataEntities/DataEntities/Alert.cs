using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class Alert : NotifyChangedBase,IEditableObject
    {
        public Alert()
            : base()
        {
           
        }

        private Alert _tempAlert;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempAlert = (Alert)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempAlert)
                CopyFrom(_tempAlert);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(Alert p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method

      
        /// Create a new Alert object.
        
        /// <param name="alertID">Initial value of the AlertID property.</param>
        /// <param name="status">Initial value of the Status property.</param>
        public static Alert CreateAlert(Int32 alertID, Int16 status)
        {
            Alert alert = new Alert();
            alert.AlertID = alertID;
            alert.Status = status;
            return alert;
        }

        #endregion
        #region Primitive Properties
        [DataMemberAttribute()]
        public Int32 AlertID
        {
            get
            {
                return _AlertID;
            }
            set
            {
                if (_AlertID != value)
                {
                    OnAlertIDChanging(value);
                    _AlertID =value;
                    RaisePropertyChanged("AlertID");
                    OnAlertIDChanged();
                }
            }
        }
        private Int32 _AlertID;
        partial void OnAlertIDChanging(Int32 value);
        partial void OnAlertIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int32> AlertTypeID
        {
            get
            {
                return _AlertTypeID;
            }
            set
            {
                OnAlertTypeIDChanging(value);
                _AlertTypeID =value;
                RaisePropertyChanged("AlertTypeID");
                OnAlertTypeIDChanged();
            }
        }
        private Nullable<Int32> _AlertTypeID;
        partial void OnAlertTypeIDChanging(Nullable<Int32> value);
        partial void OnAlertTypeIDChanged();
       
        [DataMemberAttribute()]
        public Nullable<Int32> ApointmentID
        {
            get
            {
                return _ApointmentID;
            }
            set
            {
                OnApointmentIDChanging(value);
                _ApointmentID =value;
                RaisePropertyChanged("ApointmentID");
                OnApointmentIDChanged();
            }
        }
        private Nullable<Int32> _ApointmentID;
        partial void OnApointmentIDChanging(Nullable<Int32> value);
        partial void OnApointmentIDChanged();

        [DataMemberAttribute()]
        public Int16 Status
        {
            get
            {
                return _Status;
            }
            set
            {
                OnStatusChanging(value);
                _Status =value;
                RaisePropertyChanged("Status");
                OnStatusChanged();
            }
        }
        private Int16 _Status;
        partial void OnStatusChanging(Int16 value);
        partial void OnStatusChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> AlertTime
        {
            get
            {
                return _AlertTime;
            }
            set
            {
                OnAlertTimeChanging(value);
                _AlertTime =value;
                RaisePropertyChanged("AlertTime");
                OnAlertTimeChanged();
            }
        }
        private Nullable<DateTime> _AlertTime;
        partial void OnAlertTimeChanging(Nullable<DateTime> value);
        partial void OnAlertTimeChanged();
        #endregion

        #region Navigation Properties

        //[DataMemberAttribute()]
        //public Appointment Appointment
        //{
        //    get;
        //    set;
        //}

        [DataMemberAttribute()]
        public RefAllertType RefAllertType
        {
            get;
            set;
        }
        #endregion
    }

}
