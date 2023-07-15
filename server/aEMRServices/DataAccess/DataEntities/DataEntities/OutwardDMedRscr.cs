using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class OutwardDMedRscr : MedRegItemBase
    {
        public OutwardDMedRscr()
        {
            MedProductType = AllLookupValues.MedProductType.Y_CU;
        }
        #region Factory Method


        /// Create a new OutwardDMedRscr object.

        /// <param name="outwMedRscID">Initial value of the OutwMedRscID property.</param>
        /// <param name="inWarehouseDateTime">Initial value of the InWarehouseDateTime property.</param>
        /// <param name="evenQuantity">Initial value of the EvenQuantity property.</param>
        /// <param name="oddQuantity">Initial value of the OddQuantity property.</param>
        /// <param name="amount">Initial value of the Amount property.</param>
        public static OutwardDMedRscr CreateOutwardDMedRscr(long outwMedRscID, DateTime inWarehouseDateTime, Double evenQuantity, Double oddQuantity, Decimal amount)
        {
            OutwardDMedRscr outwardDMedRscr = new OutwardDMedRscr();
            outwardDMedRscr.OutwMedRscID = outwMedRscID;
            outwardDMedRscr.InWarehouseDateTime = inWarehouseDateTime;
            outwardDMedRscr.EvenQuantity = evenQuantity;
            outwardDMedRscr.OddQuantity = oddQuantity;
            outwardDMedRscr.Amount = amount;
            return outwardDMedRscr;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long OutwMedRscID
        {
            get
            {
                return _OutwMedRscID;
            }
            set
            {
                if (_OutwMedRscID != value)
                {
                    OnOutwMedRscIDChanging(value);
                    _OutwMedRscID = value;
                    RaisePropertyChanged("OutwMedRscID");
                    OnOutwMedRscIDChanged();
                }
            }
        }
        private long _OutwMedRscID;
        partial void OnOutwMedRscIDChanging(long value);
        partial void OnOutwMedRscIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                OnPtRegistrationIDChanging(value);
                _PtRegistrationID = value;
                RaisePropertyChanged("PtRegistrationID");
                OnPtRegistrationIDChanged();
            }
        }
        private Nullable<long> _PtRegistrationID;
        partial void OnPtRegistrationIDChanging(Nullable<long> value);
        partial void OnPtRegistrationIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> InwDMedRscrID
        {
            get
            {
                return _InwDMedRscrID;
            }
            set
            {
                OnInwDMedRscrIDChanging(value);
                _InwDMedRscrID = value;
                RaisePropertyChanged("InwDMedRscrID");
                OnInwDMedRscrIDChanged();
            }
        }
        private Nullable<long> _InwDMedRscrID;
        partial void OnInwDMedRscrIDChanging(Nullable<long> value);
        partial void OnInwDMedRscrIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> OutDMedRscrID
        {
            get
            {
                return _OutDMedRscrID;
            }
            set
            {
                OnOutDMedRscrIDChanging(value);
                _OutDMedRscrID = value;
                RaisePropertyChanged("OutDMedRscrID");
                OnOutDMedRscrIDChanged();
            }
        }
        private Nullable<long> _OutDMedRscrID;
        partial void OnOutDMedRscrIDChanging(Nullable<long> value);
        partial void OnOutDMedRscrIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> TypID
        {
            get
            {
                return _TypID;
            }
            set
            {
                OnTypIDChanging(value);
                _TypID = value;
                RaisePropertyChanged("TypID");
                OnTypIDChanged();
            }
        }
        private Nullable<long> _TypID;
        partial void OnTypIDChanging(Nullable<long> value);
        partial void OnTypIDChanged();

        [DataMemberAttribute()]
        public DateTime InWarehouseDateTime
        {
            get
            {
                return _InWarehouseDateTime;
            }
            set
            {
                OnInWarehouseDateTimeChanging(value);
                _InWarehouseDateTime = value;
                RaisePropertyChanged("InWarehouseDateTime");
                OnInWarehouseDateTimeChanged();
            }
        }
        private DateTime _InWarehouseDateTime;
        partial void OnInWarehouseDateTimeChanging(DateTime value);
        partial void OnInWarehouseDateTimeChanged();

        [DataMemberAttribute()]
        public Double EvenQuantity
        {
            get
            {
                return _EvenQuantity;
            }
            set
            {
                OnEvenQuantityChanging(value);
                _EvenQuantity = value;
                RaisePropertyChanged("EvenQuantity");
                OnEvenQuantityChanged();
            }
        }
        private Double _EvenQuantity;
        partial void OnEvenQuantityChanging(Double value);
        partial void OnEvenQuantityChanged();

        [DataMemberAttribute()]
        public Double OddQuantity
        {
            get
            {
                return _OddQuantity;
            }
            set
            {
                OnOddQuantityChanging(value);
                _OddQuantity = value;
                RaisePropertyChanged("OddQuantity");
                OnOddQuantityChanged();
            }
        }
        private Double _OddQuantity;
        partial void OnOddQuantityChanging(Double value);
        partial void OnOddQuantityChanged();


        [DataMemberAttribute()]
        public Decimal Amount
        {
            get
            {
                return _Amount;
            }
            set
            {
                OnAmountChanging(value);
                _Amount = value;
                RaisePropertyChanged("Amount");
                OnAmountChanged();
            }
        }
        private Decimal _Amount;
        partial void OnAmountChanging(Decimal value);
        partial void OnAmountChanged();

    


        [DataMemberAttribute()]
        public Nullable<Decimal> AmountCoPay
        {
            get
            {
                return _AmountCoPay;
            }
            set
            {
                OnAmountCoPayChanging(value);
                _AmountCoPay = value;
                RaisePropertyChanged("AmountCoPay");
                OnAmountCoPayChanged();
            }
        }
        private Nullable<Decimal> _AmountCoPay;
        partial void OnAmountCoPayChanging(Nullable<Decimal> value);
        partial void OnAmountCoPayChanged();

        [DataMemberAttribute()]
        public Nullable<Decimal> HIRebate
        {
            get
            {
                return _HIRebate;
            }
            set
            {
                OnHIRebateChanging(value);
                _HIRebate = value;
                RaisePropertyChanged("HIRebate");
                OnHIRebateChanged();
            }
        }
        private Nullable<Decimal> _HIRebate;
        partial void OnHIRebateChanging(Nullable<Decimal> value);
        partial void OnHIRebateChanged();

        [DataMemberAttribute()]
        public String Notes
        {
            get
            {
                return _Notes;
            }
            set
            {
                OnNotesChanging(value);
                _Notes = value;
                RaisePropertyChanged("Notes");
                OnNotesChanged();
            }
        }
        private String _Notes;
        partial void OnNotesChanging(String value);
        partial void OnNotesChanged();

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public InwardDMedRscr InwardDMedRscr
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public OutwardDMedRscrInvoice OutwardDMedRscrInvoice
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public PatientRegistration PatientRegistration
        {
            get;
            set;
        }


        [DataMemberAttribute()]
        public RefOutputType RefOutputType
        {
            get;
            set;
        }

        #endregion

        private RefGenMedProductDetails _RefGenericProductItem;
        [DataMemberAttribute()]
        public RefGenMedProductDetails RefGenericProductItem
        {
            get
            {
                return _RefGenericProductItem;
            }
            set
            {
                if (_RefGenericProductItem != value)
                {
                    _RefGenericProductItem = value;
                    RaisePropertyChanged("RefGenericProductItem");
                }
            }
        }


        #region IInvoiceItem Members


        public override IChargeableItemPrice ChargeableItem
        {
            get
            {
                return _RefGenericProductItem;
            }
        }

        #endregion

        public override string ChargeableItemName
        {
            get
            {
                if (_RefGenericProductItem != null)
                {
                    return _RefGenericProductItem.BrandName;
                }
                return base.ChargeableItemName;
            }
        }
    }
}
