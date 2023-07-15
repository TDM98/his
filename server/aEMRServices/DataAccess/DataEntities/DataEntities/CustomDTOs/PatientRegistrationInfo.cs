using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{

    public partial class PatientRegistrationInfo : NotifyChangedBase
    {
        public PatientRegistrationInfo()
            : base()
        {

        }
        private int _ReceptionistID;
        public int ReceptionistID
        {
            get
            {
                return _ReceptionistID;
            }
            set
            {
                _ReceptionistID = value;
                RaisePropertyChanged("ReceptionistID");
            }
        }

        private int _PatientID;
        public int PatientID
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

        private int _RegistrationTypeID;
        public int RegistrationTypeID
        {
            get
            {
                return _RegistrationTypeID;
            }
            set
            {
                _RegistrationTypeID = value;
                RaisePropertyChanged("RegistrationTypeID");
            }
        }
        private int _DepartmentID;
        public int DepartmentID
        {
            get
            {
                return _DepartmentID;
            }
            set
            {
                _DepartmentID = value;
                RaisePropertyChanged("DepartmentID");
            }
        }
        private int _DoctorID;
        public int DoctorID
        {
            get
            {
                return _DoctorID;
            }
            set
            {
                _DoctorID = value;
                RaisePropertyChanged("DoctorID");
            }
        }
        private int _ServiceItemID;
        public int ServiceItemID
        {
            get
            {
                return _ServiceItemID;
            }
            set
            {
                _ServiceItemID = value;
                RaisePropertyChanged("ServiceItemID");
            }
        }
        private int _LocationID;
        public int LocationID
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
    }
}
