using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class HIPremiumPayment : NotifyChangedBase, IEditableObject
    {
        public HIPremiumPayment()
            : base()
        {

        }

        private HIPremiumPayment _tempHIPremiumPayment;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempHIPremiumPayment = (HIPremiumPayment)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempHIPremiumPayment)
                CopyFrom(_tempHIPremiumPayment);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(HIPremiumPayment p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new HIPremiumPayment object.

        /// <param name="hIPPmtID">Initial value of the HIPPmtID property.</param>
        public static HIPremiumPayment CreateHIPremiumPayment(long hIPPmtID)
        {
            HIPremiumPayment hIPremiumPayment = new HIPremiumPayment();
            hIPremiumPayment.HIPPmtID = hIPPmtID;
            return hIPremiumPayment;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long HIPPmtID
        {
            get
            {
                return _HIPPmtID;
            }
            set
            {
                if (_HIPPmtID != value)
                {
                    OnHIPPmtIDChanging(value);
                    ////ReportPropertyChanging("HIPPmtID");
                    _HIPPmtID = value;
                    RaisePropertyChanged("HIPPmtID");
                    OnHIPPmtIDChanged();
                }
            }
        }
        private long _HIPPmtID;
        partial void OnHIPPmtIDChanging(long value);
        partial void OnHIPPmtIDChanged();





        [DataMemberAttribute()]
        public Nullable<DateTime> GenerateDate
        {
            get
            {
                return _GenerateDate;
            }
            set
            {
                OnGenerateDateChanging(value);
                ////ReportPropertyChanging("GenerateDate");
                _GenerateDate = value;
                RaisePropertyChanged("GenerateDate");
                OnGenerateDateChanged();
            }
        }
        private Nullable<DateTime> _GenerateDate;
        partial void OnGenerateDateChanging(Nullable<DateTime> value);
        partial void OnGenerateDateChanged();





        [DataMemberAttribute()]
        public Nullable<Int64> StatementNumber
        {
            get
            {
                return _StatementNumber;
            }
            set
            {
                OnStatementNumberChanging(value);
                ////ReportPropertyChanging("StatementNumber");
                _StatementNumber = value;
                RaisePropertyChanged("StatementNumber");
                OnStatementNumberChanged();
            }
        }
        private Nullable<Int64> _StatementNumber;
        partial void OnStatementNumberChanging(Nullable<Int64> value);
        partial void OnStatementNumberChanged();





        [DataMemberAttribute()]
        public Nullable<DateTime> BillingCoverageDate
        {
            get
            {
                return _BillingCoverageDate;
            }
            set
            {
                OnBillingCoverageDateChanging(value);
                ////ReportPropertyChanging("BillingCoverageDate");
                _BillingCoverageDate = value;
                RaisePropertyChanged("BillingCoverageDate");
                OnBillingCoverageDateChanged();
            }
        }
        private Nullable<DateTime> _BillingCoverageDate;
        partial void OnBillingCoverageDateChanging(Nullable<DateTime> value);
        partial void OnBillingCoverageDateChanged();





        [DataMemberAttribute()]
        public String ReceiptNumber
        {
            get
            {
                return _ReceiptNumber;
            }
            set
            {
                OnReceiptNumberChanging(value);
                ////ReportPropertyChanging("ReceiptNumber");
                _ReceiptNumber = value;
                RaisePropertyChanged("ReceiptNumber");
                OnReceiptNumberChanged();
            }
        }
        private String _ReceiptNumber;
        partial void OnReceiptNumberChanging(String value);
        partial void OnReceiptNumberChanged();





        [DataMemberAttribute()]
        public Nullable<Decimal> RequestPaymentAmount
        {
            get
            {
                return _RequestPaymentAmount;
            }
            set
            {
                OnRequestPaymentAmountChanging(value);
                ////ReportPropertyChanging("RequestPaymentAmount");
                _RequestPaymentAmount = value;
                RaisePropertyChanged("RequestPaymentAmount");
                OnRequestPaymentAmountChanged();
            }
        }
        private Nullable<Decimal> _RequestPaymentAmount;
        partial void OnRequestPaymentAmountChanging(Nullable<Decimal> value);
        partial void OnRequestPaymentAmountChanged();





        [DataMemberAttribute()]
        public Nullable<Decimal> CostOutOfHIFund
        {
            get
            {
                return _CostOutOfHIFund;
            }
            set
            {
                OnCostOutOfHIFundChanging(value);
                ////ReportPropertyChanging("CostOutOfHIFund");
                _CostOutOfHIFund = value;
                RaisePropertyChanged("CostOutOfHIFund");
                OnCostOutOfHIFundChanged();
            }
        }
        private Nullable<Decimal> _CostOutOfHIFund;
        partial void OnCostOutOfHIFundChanging(Nullable<Decimal> value);
        partial void OnCostOutOfHIFundChanged();





        [DataMemberAttribute()]
        public Nullable<Decimal> ReimbursementAmount
        {
            get
            {
                return _ReimbursementAmount;
            }
            set
            {
                OnReimbursementAmountChanging(value);
                ////ReportPropertyChanging("ReimbursementAmount");
                _ReimbursementAmount = value;
                RaisePropertyChanged("ReimbursementAmount");
                OnReimbursementAmountChanged();
            }
        }
        private Nullable<Decimal> _ReimbursementAmount;
        partial void OnReimbursementAmountChanging(Nullable<Decimal> value);
        partial void OnReimbursementAmountChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_HIPAYMEN_REL_HOSFM_HIPREMIU", "HIPaymentTransaction")]
        public ObservableCollection<HIPaymentTransaction> HIPaymentTransactions
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_HIPREMIU_REL_HOSFM_HIPREMIU", "HIPremiumPaymentDetails")]
        public ObservableCollection<HIPremiumPaymentDetail> HIPremiumPaymentDetails
        {
            get;
            set;
        }

        #endregion
    }
}
