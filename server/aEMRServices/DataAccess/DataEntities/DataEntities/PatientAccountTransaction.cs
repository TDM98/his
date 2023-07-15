using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
using System.Linq;
namespace DataEntities
{
    public partial class PatientAccountTransaction : NotifyChangedBase
    {
        private long _PtAccountTranID;
        [DataMemberAttribute]
        public long PtAccountTranID
        {
            get
            {
                return _PtAccountTranID;
            }
            set
            {
                _PtAccountTranID = value;
                RaisePropertyChanged("PtAccountTranID");
            }
        }
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
        private string _TranReceiptNum;
        [DataMemberAttribute]
        public string TranReceiptNum
        {
            get
            {
                return _TranReceiptNum;
            }
            set
            {
                _TranReceiptNum = value;
                RaisePropertyChanged("TranReceiptNum");
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
        private DateTime _TransactionDate;
        [DataMemberAttribute]
        public DateTime TransactionDate
        {
            get
            {
                return _TransactionDate;
            }
            set
            {
                _TransactionDate = value;
                RaisePropertyChanged("TransactionDate");
            }
        }
        private Staff _Staff;
        [DataMemberAttribute]
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
        private Lookup _V_PtAccountTranType;
        [DataMemberAttribute]
        public Lookup V_PtAccountTranType
        {
            get
            {
                return _V_PtAccountTranType;
            }
            set
            {
                _V_PtAccountTranType = value;
                RaisePropertyChanged("V_PtAccountTranType");
            }
        }
        private decimal _CreditAmount;
        [DataMemberAttribute]
        public decimal CreditAmount
        {
            get
            {
                return _CreditAmount;
            }
            set
            {
                _CreditAmount = value;
                RaisePropertyChanged("CreditAmount");
            }
        }
        private decimal _DebitAmount;
        [DataMemberAttribute]
        public decimal DebitAmount
        {
            get
            {
                return _DebitAmount;
            }
            set
            {
                _DebitAmount = value;
                RaisePropertyChanged("DebitAmount");
            }
        }

        private string _Note;
        [DataMemberAttribute]
        public string Note
        {
            get
            {
                return _Note;
            }
            set
            {
                _Note = value;
                RaisePropertyChanged("Note");
            }
        }
        private Lookup _V_PaymentReason;
        [DataMemberAttribute]
        public Lookup V_PaymentReason
        {
            get
            {
                return _V_PaymentReason;
            }
            set
            {
                _V_PaymentReason = value;
                RaisePropertyChanged("V_PaymentReason");
            }
        }
        private Lookup _V_PaymentMode;
        [DataMemberAttribute]
        public Lookup V_PaymentMode
        {
            get
            {
                return _V_PaymentMode;
            }
            set
            {
                _V_PaymentMode = value;
                RaisePropertyChanged("V_PaymentMode");
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

        private string _RecLog;
        [DataMemberAttribute]
        public string RecLog
        {
            get
            {
                return _RecLog;
            }
            set
            {
                _RecLog = value;
                RaisePropertyChanged("RecLog");
            }
        }

        private long _DeletedStaffID;
        [DataMemberAttribute]
        public long DeletedStaffID
        {
            get
            {
                return _DeletedStaffID;
            }
            set
            {
                _DeletedStaffID = value;
                RaisePropertyChanged("DeletedStaffID");
            }
        }
        private DateTime _DeletedDate;
        [DataMemberAttribute]
        public DateTime DeletedDate
        {
            get
            {
                return _DeletedDate;
            }
            set
            {
                _DeletedDate = value;
                RaisePropertyChanged("DeletedDate");
            }
        }
    }
}