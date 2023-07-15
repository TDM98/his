using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{

    public class PatientOutstandingItem : OutstandingItem
    {
        private string _patientName;
        private object _patientID;

        public override string HeaderText
        {
            get
            {
                return _patientName;
            }
            set
            {
                if (_patientName != value)
                {
                    _patientName = value;
                    RaisePropertyChanged("HeaderText");
                }
            }
        }

        public override object ID
        {
            get
            {
                return _patientID;
            }
            set
            {
                if (_patientID != value)
                {
                    _patientID = value;
                    RaisePropertyChanged("ID");
                }
            }
        }

        private int _Age;
        public int Age
        {
            get
            {
                return _Age;
            }
            set
            {
                _Age = value;
                RaisePropertyChanged("Age");
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
        private long _ServiceID;
        public long ServiceID
        {
            get
            {
                return _ServiceID;
            }
            set
            {
                _ServiceID = value;
                RaisePropertyChanged("ServiceID");
            }
        }
        private string _ServiceName;
        public string ServiceName
        {
            get
            {
                return _ServiceName;
            }
            set
            {
                _ServiceName = value;
                RaisePropertyChanged("ServiceName");
            }
        }
        private long _PaymentID;
        public long PaymentID
        {
            get
            {
                return _PaymentID;
            }
            set
            {
                _PaymentID = value;
                RaisePropertyChanged("PaymentID");
            }
        }
        public override bool Equals(object obj)
        {
            PatientOutstandingItem info = obj as PatientOutstandingItem;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PaymentID == info.PaymentID && this.ServiceID == info.ServiceID && this.ID == info.ID
                && this.RegistrationID == info.RegistrationID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
