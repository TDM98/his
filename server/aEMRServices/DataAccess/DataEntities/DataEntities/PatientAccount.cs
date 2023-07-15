using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
using System.Linq;
namespace DataEntities
{
    public partial class PatientAccount : NotifyChangedBase
    {
        private long _PatientAccountID;
        [DataMemberAttribute]
        public long PatientAccountID
        {
            get
            {
                return _PatientAccountID;
            }
            set
            {
                _PatientAccountID = value;
                RaisePropertyChanged("PatientAccountID");
            }
        }
        private string _AccountNumber;
        [DataMemberAttribute]
        public string AccountNumber
        {
            get
            {
                return _AccountNumber;
            }
            set
            {
                _AccountNumber = value;
                RaisePropertyChanged("AccountNumber");
            }
        }
        private long _PatientID;
        [DataMemberAttribute]
        public long PatientID
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

        private bool _IsActive;
        [DataMemberAttribute]
        public bool IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                _IsActive = value;
                RaisePropertyChanged("IsActive");
            }
        }

        private bool _IsDeleted;
        [DataMemberAttribute]
        public bool IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                _IsDeleted = value;
                RaisePropertyChanged("IsDeleted");
            }
        }

        private DateTime _RecCreatedDate;
        [DataMemberAttribute]
        public DateTime RecCreatedDate
        {
            get
            {
                return _RecCreatedDate;
            }
            set
            {
                _RecCreatedDate = value;
                RaisePropertyChanged("RecCreatedDate");
            }
        }
    }
}