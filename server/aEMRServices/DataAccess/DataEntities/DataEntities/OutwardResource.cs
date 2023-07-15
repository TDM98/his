using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class OutwardResource : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new OutwardResource object.

        /// <param name="outwardRscrID">Initial value of the OutwardRscrID property.</param>
        /// <param name="outQuantity">Initial value of the OutQuantity property.</param>
        /// <param name="outPrice">Initial value of the OutPrice property.</param>
        public static OutwardResource CreateOutwardResource(long outwardRscrID, Double outQuantity, Decimal outPrice)
        {
            OutwardResource outwardResource = new OutwardResource();
            outwardResource.OutwardRscrID = outwardRscrID;
            outwardResource.OutQuantity = outQuantity;
            outwardResource.OutPrice = outPrice;
            return outwardResource;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long OutwardRscrID
        {
            get
            {
                return _OutwardRscrID;
            }
            set
            {
                if (_OutwardRscrID != value)
                {
                    OnOutwardRscrIDChanging(value);
                    ////ReportPropertyChanging("OutwardRscrID");
                    _OutwardRscrID = value;
                    RaisePropertyChanged("OutwardRscrID");
                    OnOutwardRscrIDChanged();
                }
            }
        }
        private long _OutwardRscrID;
        partial void OnOutwardRscrIDChanging(long value);
        partial void OnOutwardRscrIDChanged();





        [DataMemberAttribute()]
        public Nullable<Int64> StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                OnStaffIDChanging(value);
                ////ReportPropertyChanging("StaffID");
                _StaffID = value;
                RaisePropertyChanged("StaffID");
                OnStaffIDChanged();
            }
        }
        private Nullable<Int64> _StaffID;
        partial void OnStaffIDChanging(Nullable<Int64> value);
        partial void OnStaffIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> OutRscrInvID
        {
            get
            {
                return _OutRscrInvID;
            }
            set
            {
                OnOutRscrInvIDChanging(value);
                ////ReportPropertyChanging("OutRscrInvID");
                _OutRscrInvID = value;
                RaisePropertyChanged("OutRscrInvID");
                OnOutRscrInvIDChanged();
            }
        }
        private Nullable<long> _OutRscrInvID;
        partial void OnOutRscrInvIDChanging(Nullable<long> value);
        partial void OnOutRscrInvIDChanged();





        [DataMemberAttribute()]
        public Nullable<Int64> RscrID
        {
            get
            {
                return _RscrID;
            }
            set
            {
                OnRscrIDChanging(value);
                ////ReportPropertyChanging("RscrID");
                _RscrID = value;
                RaisePropertyChanged("RscrID");
                OnRscrIDChanged();
            }
        }
        private Nullable<Int64> _RscrID;
        partial void OnRscrIDChanging(Nullable<Int64> value);
        partial void OnRscrIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> InwardRscrID
        {
            get
            {
                return _InwardRscrID;
            }
            set
            {
                OnInwardRscrIDChanging(value);
                ////ReportPropertyChanging("InwardRscrID");
                _InwardRscrID = value;
                RaisePropertyChanged("InwardRscrID");
                OnInwardRscrIDChanged();
            }
        }
        private Nullable<long> _InwardRscrID;
        partial void OnInwardRscrIDChanging(Nullable<long> value);
        partial void OnInwardRscrIDChanged();





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
                ////ReportPropertyChanging("OutQuantity");
                _OutQuantity = value;
                RaisePropertyChanged("OutQuantity");
                OnOutQuantityChanged();
            }
        }
        private Double _OutQuantity;
        partial void OnOutQuantityChanging(Double value);
        partial void OnOutQuantityChanged();





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
                ////ReportPropertyChanging("OutPrice");
                _OutPrice = value;
                RaisePropertyChanged("OutPrice");
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
                ////ReportPropertyChanging("OutNotes");
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
                ////ReportPropertyChanging("OutAmount");
                _OutAmount = value;
                RaisePropertyChanged("OutAmount");
                OnOutAmountChanged();
            }
        }
        private Nullable<Decimal> _OutAmount;
        partial void OnOutAmountChanging(Nullable<Decimal> value);
        partial void OnOutAmountChanged();





        [DataMemberAttribute()]
        public Nullable<Decimal> OutPriceDifference
        {
            get
            {
                return _OutPriceDifference;
            }
            set
            {
                OnOutPriceDifferenceChanging(value);
                ////ReportPropertyChanging("OutPriceDifference");
                _OutPriceDifference = value;
                RaisePropertyChanged("OutPriceDifference");
                OnOutPriceDifferenceChanged();
            }
        }
        private Nullable<Decimal> _OutPriceDifference;
        partial void OnOutPriceDifferenceChanging(Nullable<Decimal> value);
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
                ////ReportPropertyChanging("OutAmountCoPay");
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
                ////ReportPropertyChanging("OutHIRebate");
                _OutHIRebate = value;
                RaisePropertyChanged("OutHIRebate");
                OnOutHIRebateChanged();
            }
        }
        private Nullable<Decimal> _OutHIRebate;
        partial void OnOutHIRebateChanging(Nullable<Decimal> value);
        partial void OnOutHIRebateChanged();





        [DataMemberAttribute()]
        public Nullable<Boolean> ForInternalUse
        {
            get
            {
                return _ForInternalUse;
            }
            set
            {
                OnForInternalUseChanging(value);
                ////ReportPropertyChanging("ForInternalUse");
                _ForInternalUse = value;
                RaisePropertyChanged("ForInternalUse");
                OnForInternalUseChanged();
            }
        }
        private Nullable<Boolean> _ForInternalUse;
        partial void OnForInternalUseChanging(Nullable<Boolean> value);
        partial void OnForInternalUseChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_ASSIGNME_REL_RM05_OUTWARDR", "AssignMedEquip")]
        public ObservableCollection<AssignMedEquip> AssignMedEquips
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_OUTWARDR_REL_RM24_INWARDRE", "InwardResources")]
        public InwardResource InwardResource
        {
            get;
            set;
        }


        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_OUTWARDR_REL_RM23_OUTWARDR", "OutwardResourceInvoices")]
        public OutwardResourceInvoice OutwardResourceInvoice
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_OUTWARDR_REL_RM18_RESOURCE", "Resources")]
        public Resource Resource
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_OUTWARDR_REL_RM27_STAFFS", "Staffs")]
        public Staff Staff
        {
            get;
            set;
        }

        #endregion
    }
}
