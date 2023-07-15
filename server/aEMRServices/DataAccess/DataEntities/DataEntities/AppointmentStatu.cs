using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class AppointmentStatu : NotifyChangedBase, IEditableObject
    {
        public AppointmentStatu()
            : base()
        {

        }

        private AppointmentStatu _tempAppointmentStatu;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempAppointmentStatu = (AppointmentStatu)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempAppointmentStatu)
                CopyFrom(_tempAppointmentStatu);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(AppointmentStatu p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new AppointmentStatu object.

        /// <param name="appStatusID">Initial value of the AppStatusID property.</param>
        /// <param name="status">Initial value of the Status property.</param>
        public static AppointmentStatu CreateAppointmentStatu(Int32 appStatusID, Int16 status)
        {
            AppointmentStatu appointmentStatu = new AppointmentStatu();
            appointmentStatu.AppStatusID = appStatusID;
            appointmentStatu.Status = status;
            return appointmentStatu;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public Int32 AppStatusID
        {
            get
            {
                return _AppStatusID;
            }
            set
            {
                if (_AppStatusID != value)
                {
                    OnAppStatusIDChanging(value);
                    _AppStatusID = value;
                    RaisePropertyChanged("AppStatusID");
                    OnAppStatusIDChanged();
                }
            }
        }
        private Int32 _AppStatusID;
        partial void OnAppStatusIDChanging(Int32 value);
        partial void OnAppStatusIDChanged();
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
                _Status = value;
                RaisePropertyChanged("Status");
                OnStatusChanged();
            }
        }
        private Int16 _Status;
        partial void OnStatusChanging(Int16 value);
        partial void OnStatusChanged();

        #endregion

    }
}
