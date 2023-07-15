using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class PatientSession : NotifyChangedBase
    {
        public PatientSession()
            : base()
        {
           
        }

        private long _ID;
        public long ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
                RaisePropertyChanged("ID");
            }
        }
        private long _RegistrationID;
        public long RegistrationID
        {
            get
            {
                return _RegistrationID;
            }
            set
            {
                _RegistrationID = value;
                RaisePropertyChanged("RegistrationID");
            }
        }
        private long? _PatientID;
        public long? PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                _PatientID = value;
                RaisePropertyChanged("PatientID");
            }
        }
        private long? _LocationID;
        public long? LocationID
        {
            get
            {
                return _LocationID;
            }
            set
            {
                _LocationID = value;
                RaisePropertyChanged("LocationID");
            }
        }
        private long _V_SessionActivity;
        public long V_SessionActivity
        {
            get
            {
                return _V_SessionActivity;
            }
            set
            {
                _V_SessionActivity = value;
                RaisePropertyChanged("V_SessionActivity");
            }
        }

        private DateTime? _Waiting;
        public DateTime? Waiting
        {
            get
            {
                return _Waiting;
            }
            set
            {
                _Waiting = value;
                RaisePropertyChanged("Waiting");
            }
        }
        private DateTime? _Begin;
        public DateTime? Begin
        {
            get
            {
                return _Begin;
            }
            set
            {
                _Begin = value;
                RaisePropertyChanged("Begin");
            }
        }
        private DateTime? _End;
        public DateTime? End
        {
            get
            {
                return _End;
            }
            set
            {
                _End = value;
                RaisePropertyChanged("End");
            }
        }
    }

}
