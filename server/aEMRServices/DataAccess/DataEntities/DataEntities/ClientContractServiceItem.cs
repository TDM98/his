using eHCMS.Services.Core.Base;
using System;
using System.Runtime.Serialization;

namespace DataEntities
{
    public class ClientContractServiceItem : NotifyChangedBase
    {
        private long _ClientContractSvcID;
        private Staff _CreatedStaff;
        private MedRegItemBase _MedRegItem;
        private DateTime _RecCreatedDate;
        private bool _IsDeleted;
        private bool? _IsChecked = false;
        private long _HosClientContractID;
        private bool _IsProcessed = false;
        [DataMemberAttribute]
        public long ClientContractSvcID
        {
            get
            {
                return _ClientContractSvcID;
            }
            set
            {
                _ClientContractSvcID = value;
                RaisePropertyChanged("ClientContractSvcID");
            }
        }
        [DataMemberAttribute]
        public Staff CreatedStaff
        {
            get
            {
                return _CreatedStaff;
            }
            set
            {
                _CreatedStaff = value;
                RaisePropertyChanged("CreatedStaff");
            }
        }
        [DataMemberAttribute]
        public MedRegItemBase MedRegItem
        {
            get
            {
                return _MedRegItem;
            }
            set
            {
                _MedRegItem = value;
                RaisePropertyChanged("MedRegItem");
            }
        }
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
        public bool? IsChecked
        {
            get
            {
                return _IsChecked;
            }
            set
            {
                if (_IsChecked == value)
                {
                    return;
                }
                _IsChecked = value;
                RaisePropertyChanged("IsChecked");
            }
        }
        [DataMemberAttribute]
        public long HosClientContractID
        {
            get
            {
                return _HosClientContractID;
            }
            set
            {
                _HosClientContractID = value;
                RaisePropertyChanged("HosClientContractID");
            }
        }
        [DataMemberAttribute]
        public bool IsProcessed
        {
            get
            {
                return _IsProcessed;
            }
            set
            {
                if (_IsProcessed == value)
                {
                    return;
                }
                _IsProcessed = value;
                RaisePropertyChanged("IsProcessed");
            }
        }
    }
}