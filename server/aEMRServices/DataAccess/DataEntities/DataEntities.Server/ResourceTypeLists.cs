using eHCMS.Services.Core.Base;
using System;
using System.Runtime.Serialization;
namespace DataEntities
{
    public partial class ResourceTypeLists : EntityBase
    {
        #region Primitive Properties
        [DataMemberAttribute()]
        public long RscrTypeListID
        {
            get
            {
                return _RscrTypeListID;
            }
            set
            {
                if (_RscrTypeListID == value)
                {
                    return;
                }
                _RscrTypeListID = value;
                RaisePropertyChanged("RscrTypeListID");
            }
        }
        private long _RscrTypeListID;

        [DataMemberAttribute()]
        public long RscrID
        {
            get
            {
                return _RscrID;
            }
            set
            {
                if (_RscrID == value)
                {
                    return;
                }
                _RscrID = value;
                RaisePropertyChanged("RscrID");
            }
        }
        private long _RscrID;

        [DataMemberAttribute()]
        public long MedicalServiceTypeID
        {
            get
            {
                return _MedicalServiceTypeID;
            }
            set
            {
                if (_MedicalServiceTypeID == value)
                {
                    return;
                }
                _MedicalServiceTypeID = value;
                RaisePropertyChanged("MedicalServiceTypeID");
            }
        }
        private long _MedicalServiceTypeID;

        [DataMemberAttribute]
        public long CreatedStaffID
        {
            get
            {
                return _CreatedStaffID;
            }
            set
            {
                if (_CreatedStaffID == value)
                {
                    return;
                }
                _CreatedStaffID = value;
                RaisePropertyChanged("CreatedStaffID");
            }
        }
        private long _CreatedStaffID;

        [DataMemberAttribute]
        public DateTime CreatedDate
        {
            get
            {
                return _CreatedDate;
            }
            set
            {
                if (_CreatedDate == value)
                {
                    return;
                }
                _CreatedDate = value;
                RaisePropertyChanged("CreatedDate");
            }
        }
        private DateTime _CreatedDate;

        [DataMemberAttribute]
        public long LastUpdateStaffID
        {
            get
            {
                return _LastUpdateStaffID;
            }
            set
            {
                if (_LastUpdateStaffID == value)
                {
                    return;
                }
                _LastUpdateStaffID = value;
                RaisePropertyChanged("LastUpdateStaffID");
            }
        }
        private long _LastUpdateStaffID;

        [DataMemberAttribute]
        public DateTime LastUpdateDate
        {
            get
            {
                return _LastUpdateDate;
            }
            set
            {
                if (_LastUpdateDate == value)
                {
                    return;
                }
                _LastUpdateDate = value;
                RaisePropertyChanged("LastUpdateDate");
            }
        }
        private DateTime _LastUpdateDate;
        #endregion
    }
}
