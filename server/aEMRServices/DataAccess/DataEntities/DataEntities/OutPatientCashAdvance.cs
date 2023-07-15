using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DataEntities
{
    public class OutPatientCashAdvance : NotifyChangedBase
    {
        private long _OutPtCashAdvanceID;
        private long _PtRegistrationID;
        private string _CashAdvReceiptNum;
        private DateTime _RecCreatedDate;
        private DateTime _PaymentDate;
        private long _StaffID;
        private long _V_CashAdvanceType;
        private decimal _PaymentAmount;
        private decimal _BalanceAmount;
        private string _GeneralNote;
        private long? _RptPtCashAdvRemID;
        private long _V_PaymentReason;
        private long _V_PaymentMode;
        private bool _IsDeleted = false;
        private string _RecLog;
        private long _PtTranPaymtID;
        private IList<OutPatientCashAdvanceLink> _OutPatientCashAdvanceLinks;

        [DataMemberAttribute]
        public long OutPtCashAdvanceID
        {
            get => _OutPtCashAdvanceID; set
            {
                _OutPtCashAdvanceID = value;
                RaisePropertyChanged("OutPtCashAdvanceID");
            }
        }
        [DataMemberAttribute]
        public long PtRegistrationID
        {
            get => _PtRegistrationID; set
            {
                _PtRegistrationID = value;
                RaisePropertyChanged("PtRegistrationID");
            }
        }
        [DataMemberAttribute]
        public string CashAdvReceiptNum
        {
            get => _CashAdvReceiptNum; set
            {
                _CashAdvReceiptNum = value;
                RaisePropertyChanged("CashAdvReceiptNum");
            }
        }
        [DataMemberAttribute]
        public DateTime RecCreatedDate
        {
            get => _RecCreatedDate; set
            {
                _RecCreatedDate = value;
                RaisePropertyChanged("RecCreatedDate");
            }
        }
        [DataMemberAttribute]
        public DateTime PaymentDate
        {
            get => _PaymentDate; set
            {
                _PaymentDate = value;
                RaisePropertyChanged("PaymentDate");
            }
        }
        [DataMemberAttribute]
        public long StaffID
        {
            get => _StaffID; set
            {
                _StaffID = value;
                RaisePropertyChanged("StaffID");
            }
        }
        [DataMemberAttribute]
        public long V_CashAdvanceType
        {
            get => _V_CashAdvanceType; set
            {
                _V_CashAdvanceType = value;
                RaisePropertyChanged("V_CashAdvanceType");
            }
        }
        [DataMemberAttribute]
        public decimal PaymentAmount
        {
            get => _PaymentAmount; set
            {
                _PaymentAmount = value;
                RaisePropertyChanged("PaymentAmount");
            }
        }
        [DataMemberAttribute]
        public decimal BalanceAmount
        {
            get => _BalanceAmount; set
            {
                _BalanceAmount = value;
                RaisePropertyChanged("BalanceAmount");
            }
        }
        [DataMemberAttribute]
        public string GeneralNote
        {
            get => _GeneralNote; set
            {
                _GeneralNote = value;
                RaisePropertyChanged("GeneralNote");
            }
        }
        [DataMemberAttribute]
        public long? RptPtCashAdvRemID
        {
            get => _RptPtCashAdvRemID; set
            {
                _RptPtCashAdvRemID = value;
                RaisePropertyChanged("RptPtCashAdvRemID");
            }
        }
        [DataMemberAttribute]
        public long V_PaymentReason
        {
            get => _V_PaymentReason; set
            {
                _V_PaymentReason = value;
                RaisePropertyChanged("V_PaymentReason");
            }
        }
        [DataMemberAttribute]
        public long V_PaymentMode
        {
            get => _V_PaymentMode; set
            {
                _V_PaymentMode = value;
                RaisePropertyChanged("V_PaymentMode");
            }
        }
        [DataMemberAttribute]
        public bool IsDeleted
        {
            get => _IsDeleted; set
            {
                _IsDeleted = value;
                RaisePropertyChanged("IsDeleted");
            }
        }
        [DataMemberAttribute]
        public string RecLog
        {
            get => _RecLog; set
            {
                _RecLog = value;
                RaisePropertyChanged("RecLog");
            }
        }
        [DataMemberAttribute]
        public long PtTranPaymtID
        {
            get => _PtTranPaymtID; set
            {
                _PtTranPaymtID = value;
                RaisePropertyChanged("PtTranPaymtID");
            }
        }
        [DataMemberAttribute]
        public IList<OutPatientCashAdvanceLink> OutPatientCashAdvanceLinks
        {
            get => _OutPatientCashAdvanceLinks; set
            {
                _OutPatientCashAdvanceLinks = value;
                RaisePropertyChanged("OutPatientCashAdvanceLinks");
            }
        }
    }
}