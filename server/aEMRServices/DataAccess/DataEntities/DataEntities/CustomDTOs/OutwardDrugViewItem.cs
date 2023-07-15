using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class OutwardDrugViewItem : NotifyChangedBase
    {
     
        #region Primitive Properties

        [DataMemberAttribute()]
        public long OutID
        {
            get
            {
                return _OutID;
            }
            set
            {
                if (_OutID != value)
                {
                    OnOutIDChanging(value);
                    ////ReportPropertyChanging("OutID");
                    _OutID = value;
                    RaisePropertyChanged("OutID");
                    OnOutIDChanged();
                }
            }
        }
        private long _OutID;
        partial void OnOutIDChanging(long value);
        partial void OnOutIDChanged();

   

        [DataMemberAttribute()]
        public Nullable<long> outiID
        {
            get
            {
                return _outiID;
            }
            set
            {
                OnoutiIDChanging(value);
                ////ReportPropertyChanging("outiID");
                _outiID = value;
                RaisePropertyChanged("outiID");
                OnoutiIDChanged();
            }
        }
        private Nullable<long> _outiID;
        partial void OnoutiIDChanging(Nullable<long> value);
        partial void OnoutiIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> DrugID
        {
            get
            {
                return _DrugID;
            }
            set
            {
                OnDrugIDChanging(value);
                ////ReportPropertyChanging("DrugID");
                _DrugID = value;
                RaisePropertyChanged("DrugID");
                OnDrugIDChanged();
            }
        }
        private Nullable<long> _DrugID;
        partial void OnDrugIDChanging(Nullable<long> value);
        partial void OnDrugIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> InID
        {
            get
            {
                return _InID;
            }
            set
            {
                OnInIDChanging(value);
                _InID = value;
                RaisePropertyChanged("InID");
                OnInIDChanged();
            }
        }
        private Nullable<long> _InID;
        partial void OnInIDChanging(Nullable<long> value);
        partial void OnInIDChanged();

        [DataMemberAttribute()]
        public Double OutQuantity
        {
            get
            {
                return _OutQuantity;
            }
            set
            {
                OnOutQuantityChanging(value);
                _OutQuantity = value;
                TotalPrice = (decimal)_OutQuantity * _OutPrice;
                RaisePropertyChanged("OutQuantity");
                RaisePropertyChanged("TotalPrice");
                OnOutQuantityChanged();
            }
        }
        private Double _OutQuantity;
        partial void OnOutQuantityChanging(Double value);
        partial void OnOutQuantityChanged();


        [DataMemberAttribute()]
        public Decimal TotalPrice
        {
            get
            {
                return _TotalPrice;
            }
            set
            {
                OnTotalPriceChanging(value);
                _TotalPrice = value;
                RaisePropertyChanged("TotalPrice");
                OnTotalPriceChanged();
            }
        }
        private Decimal _TotalPrice;
        partial void OnTotalPriceChanging(Decimal value);
        partial void OnTotalPriceChanged();

        [DataMemberAttribute()]
        public Decimal TotalPriceDifference
        {
            get
            {
                return _TotalPriceDifference;
            }
            set
            {
                _TotalPriceDifference = value;
                RaisePropertyChanged("TotalPriceDifference");
            }
        }
        private Decimal _TotalPriceDifference;


        [DataMemberAttribute()]
        public Decimal OutPrice
        {
            get
            {
                return _OutPrice;
            }
            set
            {
                OnOutPriceChanging(value);
                _OutPrice = value;
                TotalPrice = (decimal)_OutQuantity * _OutPrice;
                RaisePropertyChanged("OutPrice");
                RaisePropertyChanged("TotalPrice");
                OnOutPriceChanged();
            }
        }

       
        private Decimal _OutPrice;
        partial void OnOutPriceChanging(Decimal value);
        partial void OnOutPriceChanged();


        [DataMemberAttribute()]
        public String OutNotes
        {
            get
            {
                return _OutNotes;
            }
            set
            {
                OnOutNotesChanging(value);
                _OutNotes = value;
                RaisePropertyChanged("OutNotes");
                OnOutNotesChanged();
            }
        }
        private String _OutNotes;
        partial void OnOutNotesChanging(String value);
        partial void OnOutNotesChanged();





        [DataMemberAttribute()]
        public Nullable<Decimal> OutAmount
        {
            get
            {
                return _OutAmount;
            }
            set
            {
                OnOutAmountChanging(value);
                _OutAmount = value;
                RaisePropertyChanged("OutAmount");
                OnOutAmountChanged();
            }
        }
        private Nullable<Decimal> _OutAmount;
        partial void OnOutAmountChanging(Nullable<Decimal> value);
        partial void OnOutAmountChanged();

        [DataMemberAttribute()]
        public Decimal OutPriceDifference
        {
            get
            {
                return _OutPriceDifference;
            }
            set
            {
                OnOutPriceDifferenceChanging(value);
                 _OutPriceDifference = value;
                 TotalPriceDifference =  (decimal)_OutQuantity*_OutPriceDifference ;
                RaisePropertyChanged("OutPriceDifference");
                OnOutPriceDifferenceChanged();
            }
        }
        private Decimal _OutPriceDifference;
        partial void OnOutPriceDifferenceChanging(Decimal value);
        partial void OnOutPriceDifferenceChanged();

        [DataMemberAttribute()]
        public Nullable<Decimal> OutAmountCoPay
        {
            get
            {
                return _OutAmountCoPay;
            }
            set
            {
                OnOutAmountCoPayChanging(value);
                _OutAmountCoPay = value;
                RaisePropertyChanged("OutAmountCoPay");
                OnOutAmountCoPayChanged();
            }
        }
        private Nullable<Decimal> _OutAmountCoPay;
        partial void OnOutAmountCoPayChanging(Nullable<Decimal> value);
        partial void OnOutAmountCoPayChanged();

        [DataMemberAttribute()]
        public Nullable<Decimal> OutHIRebate
        {
            get
            {
                return _OutHIRebate;
            }
            set
            {
                OnOutHIRebateChanging(value);
                _OutHIRebate = value;
                RaisePropertyChanged("OutHIRebate");
                OnOutHIRebateChanged();
            }
        }
        private Nullable<Decimal> _OutHIRebate;
        partial void OnOutHIRebateChanging(Nullable<Decimal> value);
        partial void OnOutHIRebateChanged();

        #endregion

        private string _BrandName;
        public string BrandName
        {
            get
            {
                return _BrandName;
            }
            set
            {
                _BrandName = value;
                RaisePropertyChanged("BrandName");
            }
        }
        private string _OutInvID;
        public string OutInvID
        {
            get
            {
                return _OutInvID;
            }
            set
            {
                _OutInvID = value;
                RaisePropertyChanged("OutInvID");
            }
        }
        private DateTime? _OutDate;
        public DateTime? OutDate
        {
            get
            {
                return _OutDate;
            }
            set
            {
                _OutDate = value;
                RaisePropertyChanged("OutDate");
            }
        }
    }
}
