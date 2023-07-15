using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    
    public partial class TransactionFinalization : NotifyChangedBase
    {
        public TransactionFinalization()
        {
        }
     
        private long _TranFinalizationID;
        [DataMemberAttribute()]
        public long TranFinalizationID
        {
            get
            {
                return _TranFinalizationID;
            }
            set
            {
                if (_TranFinalizationID != value)
                {
                    _TranFinalizationID = value;
                    RaisePropertyChanged("TranFinalizationID");
                }
            }
        }
        private DateTime _DateFinalize;
        [DataMemberAttribute()]
        public DateTime DateFinalize
        {
            get
            {
                return _DateFinalize;
            }
            set
            {
                if (_DateFinalize != value)
                {
                    _DateFinalize = value;
                    RaisePropertyChanged("DateFinalize");
                }
            }
        }
        private long _PtRegistrationID;
        [DataMemberAttribute()]
        public long PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                if (_PtRegistrationID != value)
                {
                    _PtRegistrationID = value;
                    RaisePropertyChanged("PtRegistrationID");
                }
            }
        }

        //KMx: Sử dụng SettlementStaff để có đầy đủ thông tin (28/12/2014 15:57).
        //private long _StaffID;
        //[DataMemberAttribute()]
        //public long StaffID
        //{
        //    get
        //    {
        //        return _StaffID;
        //    }
        //    set
        //    {
        //        if (_StaffID != value)
        //        {
        //            _StaffID = value;
        //            RaisePropertyChanged("StaffID");
        //        }
        //    }
        //}

        private DateTime _TranDateFrom;
        [DataMemberAttribute()]
        public DateTime TranDateFrom
        {
            get
            {
                return _TranDateFrom;
            }
            set
            {
                if (_TranDateFrom != value)
                {
                    _TranDateFrom = value;
                    RaisePropertyChanged("TranDateFrom");
                }
            }
        }
        private DateTime _TranDateTo;
        [DataMemberAttribute()]
        public DateTime TranDateTo
        {
            get
            {
                return _TranDateTo;
            }
            set
            {
                if (_TranDateTo != value)
                {
                    _TranDateTo = value;
                    RaisePropertyChanged("TranDateTo");
                }
            }
        }
        private long _V_TranFinalizationType;
        [DataMemberAttribute()]
        public long V_TranFinalizationType
        {
            get
            {
                return _V_TranFinalizationType;
            }
            set
            {
                if (_V_TranFinalizationType != value)
                {
                    _V_TranFinalizationType = value;
                    RaisePropertyChanged("V_TranFinalizationType");
                }
            }
        }

        private string _FinalizedReceiptNum;
        [DataMemberAttribute()]
        public string FinalizedReceiptNum
        {
            get
            {
                return _FinalizedReceiptNum;
            }
            set
            {
                if (_FinalizedReceiptNum != value)
                {
                    _FinalizedReceiptNum = value;
                    RaisePropertyChanged("FinalizedReceiptNum");
                }
            }
        }

        private Staff _SettlementStaff;
        [DataMemberAttribute()]
        public Staff SettlementStaff
        {
            get
            {
                return _SettlementStaff;
            }
            set
            {
                if (_SettlementStaff != value)
                {
                    _SettlementStaff = value;
                    RaisePropertyChanged("SettlementStaff");
                }
            }
        }

        private Decimal _TotalPatientPayment;
        [DataMemberAttribute()]
        public Decimal TotalPatientPayment
        {
            get
            {
                return _TotalPatientPayment;
            }
            set
            {
                if (_TotalPatientPayment != value)
                {
                    _TotalPatientPayment = value;
                    RaisePropertyChanged("TotalPatientPayment");
                }
            }
        }

        private Decimal _TotalHIPayment;
        [DataMemberAttribute()]
        public Decimal TotalHIPayment
        {
            get
            {
                return _TotalHIPayment;
            }
            set
            {
                if (_TotalHIPayment != value)
                {
                    _TotalHIPayment = value;
                    RaisePropertyChanged("TotalHIPayment");
                }
            }
        }

        private Decimal _TotalInvoicePrice;
        [DataMemberAttribute()]
        public Decimal TotalInvoicePrice
        {
            get
            {
                return _TotalInvoicePrice;
            }
            set
            {
                if (_TotalInvoicePrice != value)
                {
                    _TotalInvoicePrice = value;
                    RaisePropertyChanged("TotalInvoicePrice");
                }
            }
        }

        private Decimal _TotalSupported;
        [DataMemberAttribute()]
        public Decimal TotalSupported
        {
            get
            {
                return _TotalSupported;
            }
            set
            {
                if (_TotalSupported != value)
                {
                    _TotalSupported = value;
                    RaisePropertyChanged("TotalSupported");
                }
            }
        }

        private Decimal _TotalSupported_HighTech;
        [DataMemberAttribute()]
        public Decimal TotalSupported_HighTech
        {
            get
            {
                return _TotalSupported_HighTech;
            }
            set
            {
                if (_TotalSupported_HighTech != value)
                {
                    _TotalSupported_HighTech = value;
                    RaisePropertyChanged("TotalSupported_HighTech");
                }
            }
        }

        private Decimal _TotalSupportFund;
        [DataMemberAttribute()]
        public Decimal TotalSupportFund
        {
            get
            {
                return _TotalSupportFund;
            }
            set
            {
                if (_TotalSupportFund != value)
                {
                    _TotalSupportFund = value;
                    RaisePropertyChanged("TotalSupportFund");
                }
            }
        }

        public override bool Equals(object obj)
        {
            var info = obj as TransactionFinalization;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.TranFinalizationID == info.TranFinalizationID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }



        /// <summary>
        /// 31-08-2012 Dinh
        /// Thêm trạng thái để phân biệt nội trú và ngoại trú
        /// </summary>
        private RegistrationType _RegistrationType;
        [DataMemberAttribute()]
        public RegistrationType RegistrationType
        {
            get
            {
                return _RegistrationType;
            }
            set
            {
                _RegistrationType = value;
                RaisePropertyChanged("RegistrationType");
            }
        }


        private AllLookupValues.RegistrationType _V_RegistrationType = AllLookupValues.RegistrationType.NGOAI_TRU;
        [DataMemberAttribute()]
        public AllLookupValues.RegistrationType V_RegistrationType
        {
            get
            {
                return _V_RegistrationType;
            }
            set
            {
                _V_RegistrationType = value;
                RaisePropertyChanged("V_RegistrationType");
            }
        }
    }
}
