using eHCMS.Services.Core.Base;
using System;
using System.Runtime.Serialization;

namespace DataEntities
{
    public class ClientContractServiceItemPatientLink : NotifyChangedBase
    {
        private long _ClientContractSvcPtID;
        private ClientContractServiceItem _ContractMedRegItem;
        private HosClientContractPatient _ContractPatient;
        private DateTime _RecordCreatedDate;
        private bool _IsDeleted;
        [DataMemberAttribute]
        public long ClientContractSvcPtID
        {
            get
            {
                return _ClientContractSvcPtID;
            }
            set
            {
                _ClientContractSvcPtID = value;
                RaisePropertyChanged("ClientContractSvcPtID");
            }
        }
        [DataMemberAttribute]
        public ClientContractServiceItem ContractMedRegItem
        {
            get
            {
                return _ContractMedRegItem;
            }
            set
            {
                _ContractMedRegItem = value;
                RaisePropertyChanged("ContractMedRegItem");
            }
        }
        [DataMemberAttribute]
        public HosClientContractPatient ContractPatient
        {
            get
            {
                return _ContractPatient;
            }
            set
            {
                _ContractPatient = value;
                RaisePropertyChanged("ContractPatient");
            }
        }
        [DataMemberAttribute]
        public DateTime RecordCreatedDate
        {
            get
            {
                return _RecordCreatedDate;
            }
            set
            {
                _RecordCreatedDate = value;
                RaisePropertyChanged("RecordCreatedDate");
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
    }
}