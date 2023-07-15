using eHCMS.Services.Core.Base;
using System;
using System.Runtime.Serialization;

namespace DataEntities
{
    public class HosClientContractFinalization : NotifyChangedBase
    {
        private long _ClientContractFinalizationID;
        private long _HosClientContractID;
        private string _FinalizedReceiptNum;
        private DateTime _DateFinalize;
        private Staff _Staff;
        private decimal _Amount;
        public long ClientContractFinalizationID
        {
            get
            {
                return _ClientContractFinalizationID;
            }
            set
            {
                _ClientContractFinalizationID = value;
                RaisePropertyChanged("ClientContractFinalizationID");
            }
        }
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
        public string FinalizedReceiptNum
        {
            get
            {
                return _FinalizedReceiptNum;
            }
            set
            {
                _FinalizedReceiptNum = value;
                RaisePropertyChanged("FinalizedReceiptNum");
            }
        }
        public DateTime DateFinalize
        {
            get
            {
                return _DateFinalize;
            }
            set
            {
                _DateFinalize = value;
                RaisePropertyChanged("DateFinalize");
            }
        }
        public Staff Staff
        {
            get
            {
                return _Staff;
            }
            set
            {
                _Staff = value;
                RaisePropertyChanged("Staff");
            }
        }
        public decimal Amount
        {
            get
            {
                return _Amount;
            }
            set
            {
                _Amount = value;
                RaisePropertyChanged("Amount");
            }
        }
    }
}