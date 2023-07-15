using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DataEntities
{
    public class HosClientContractPatient : NotifyChangedBase
    {
        private long _HosContractPtID;
        private long _HosClientContractID;
        private Patient _PatientObj;
        private string _ClientClassification;
        private string _ClientGroup;
        private DateTime _RecCreatedDate;
        private bool _IsDeleted;
        private decimal _TotalInvoicePrice;
        private decimal _TotalHIPrice;
        private bool _IsSelected;
        private bool _IsScheduled;
        private decimal _TotalContractPaidAmount;
        private List<HosClientContractPatientGroup> _PatientGroupCollection;
        private bool _IsProcessed = false;
        private long? _PtRegistrationID;
        [DataMemberAttribute]
        public long HosContractPtID
        {
            get
            {
                return _HosContractPtID;
            }
            set
            {
                _HosContractPtID = value;
                RaisePropertyChanged("HosContractPtID");
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
        public Patient PatientObj
        {
            get
            {
                return _PatientObj;
            }
            set
            {
                _PatientObj = value;
                RaisePropertyChanged("PatientObj");
            }
        }
        [DataMemberAttribute]
        public string ClientClassification
        {
            get
            {
                return _ClientClassification;
            }
            set
            {
                _ClientClassification = value;
                RaisePropertyChanged("ClientClassification");
            }
        }
        [DataMemberAttribute]
        public string ClientGroup
        {
            get
            {
                return _ClientGroup;
            }
            set
            {
                _ClientGroup = value;
                RaisePropertyChanged("ClientGroup");
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
        public decimal TotalInvoicePrice
        {
            get
            {
                return _TotalInvoicePrice;
            }
            set
            {
                _TotalInvoicePrice = value;
                RaisePropertyChanged("TotalInvoicePrice");
            }
        }
        public decimal TotalHIPrice
        {
            get
            {
                return _TotalHIPrice;
            }
            set
            {
                _TotalHIPrice = value;
                RaisePropertyChanged("TotalHIPrice");
            }
        }
        [DataMemberAttribute]
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                _IsSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }
        [DataMemberAttribute]
        public bool IsScheduled
        {
            get
            {
                return _IsScheduled;
            }
            set
            {
                if (_IsScheduled == value)
                {
                    return;
                }
                _IsScheduled = value;
                RaisePropertyChanged("IsScheduled");
            }
        }
        [DataMemberAttribute]
        public decimal TotalContractPaidAmount
        {
            get
            {
                return _TotalContractPaidAmount;
            }
            set
            {
                if (_TotalContractPaidAmount == value)
                {
                    return;
                }
                _TotalContractPaidAmount = value;
                RaisePropertyChanged("TotalContractPaidAmount");
            }
        }
        [DataMemberAttribute]
        public List<HosClientContractPatientGroup> PatientGroupCollection
        {
            get
            {
                return _PatientGroupCollection;
            }
            set
            {
                if (_PatientGroupCollection == value)
                {
                    return;
                }
                _PatientGroupCollection = value;
                RaisePropertyChanged("PatientGroupCollection");
                RaisePropertyChanged("PatientGroupString");
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
        public string PatientGroupString
        {
            get
            {
                return PatientGroupCollection == null ? null : string.Join(",", PatientGroupCollection.Select(x => x.HosClientContractPatientGroupName));
            }
        }
        [DataMemberAttribute]
        public long? PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                if (_PtRegistrationID == value)
                {
                    return;
                }
                _PtRegistrationID = value;
                RaisePropertyChanged("PtRegistrationID");
            }
        }
        public void NotifyChangedPatientGroupString()
        {
            RaisePropertyChanged("PatientGroupString");
        }
    }
}