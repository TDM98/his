using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
/*
 * 20181124 #001 TTM: BM 0005309: Tạo mới property GenMedProductItem để lưu thông tin chi tiết thuốc từ phiếu xuất.
 */
namespace DataEntities
{
    public partial class OutwardDrug : MedRegItemBase, IEditableObject// NotifyChangedBase, IEditableObject,IInvoiceItem
    {
        public OutwardDrug()
            : base()
        {
            MedProductType = AllLookupValues.MedProductType.THUOC;
        }

        public override long ID
        {
            get { return OutID; }
        }

        private OutwardDrug _tempOutwardDrug;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempOutwardDrug = (OutwardDrug)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempOutwardDrug)
                CopyFrom(_tempOutwardDrug);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(OutwardDrug p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new OutwardDrug object.

        /// <param name="outID">Initial value of the OutID property.</param>
        /// <param name="outQuantity">Initial value of the OutQuantity property.</param>
        /// <param name="outPrice">Initial value of the OutPrice property.</param>
        public static OutwardDrug CreateOutwardDrug(long outID, int outQuantity, Decimal outPrice)
        {
            OutwardDrug outwardDrug = new OutwardDrug();
            outwardDrug.OutID = outID;
            outwardDrug.OutQuantity = outQuantity;
            outwardDrug.OutPrice = outPrice;
            return outwardDrug;
        }

        #endregion

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
        public String InBatchNumber
        {
            get
            {
                return _InBatchNumber;
            }
            set
            {
                OnInBatchNumberChanging(value);
                _InBatchNumber = value;
                RaisePropertyChanged("InBatchNumber");
                OnInBatchNumberChanged();
            }
        }
        private String _InBatchNumber;
        partial void OnInBatchNumberChanging(String value);
        partial void OnInBatchNumberChanged();

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

        [CustomValidation(typeof(OutwardDrug), "ValidateQtySell")]
        [DataMemberAttribute()]
        public int OutQuantity
        {
            get
            {
                return _OutQuantity;
            }
            set
            {
                OnOutQuantityChanging(value);
                ValidateProperty("OutQuantity", value);
                _OutQuantity = value;
                TotalPrice = (decimal)_OutQuantity * _OutPrice;
                TotalPriceDifference = (decimal)_OutQuantity * PriceDifference;

                Qty = value;
                RaisePropertyChanged("OutQuantity");
                RaisePropertyChanged("TotalPrice");
                RaisePropertyChanged("TotalPriceDifference");
                OnOutQuantityChanged();
            }
        }
        private int _OutQuantity;
        partial void OnOutQuantityChanging(int value);
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

        [DataMemberAttribute()]
        public Int64 V_DrugType
        {
            get
            {
                return _V_DrugType;
            }
            set
            {
                _V_DrugType = value;
            }
        }
        private Int64 _V_DrugType;

        [DataMemberAttribute()]
        public double? HIPaymentPercent
        {
            get
            {
                return _HIPaymentPercent;
            }
            set
            {
                _HIPaymentPercent = value;
                RaisePropertyChanged("HIPaymentPercent");
            }
        }
        private double? _HIPaymentPercent;

        #endregion

        #region Navigation Properties
        [DataMemberAttribute()]
        public GetDrugForSellVisitor GetDrugForSellVisitor
        {
            get
            {
                return _GetDrugForSellVisitor;
            }
            set
            {
                OnGetDrugForSellVisitorChanging(value);
                _GetDrugForSellVisitor = value;
                if (_GetDrugForSellVisitor != null)
                {
                    DayRpts = _GetDrugForSellVisitor.DayRpts;
                }
                else
                {
                    DayRpts = 0;
                }
                OnGetDrugForSellVisitorChanged();
                RaisePropertyChanged("GetDrugForSellVisitor");

            }
        }

        private GetDrugForSellVisitor _GetDrugForSellVisitor;
        partial void OnGetDrugForSellVisitorChanging(GetDrugForSellVisitor value);
        partial void OnGetDrugForSellVisitorChanged();

        private OutwardDrugInvoice _OutwardDrugInvoice;
        [DataMemberAttribute()]
        public OutwardDrugInvoice OutwardDrugInvoice
        {
            get
            {
                return _OutwardDrugInvoice;
            }
            set
            {
                if (_OutwardDrugInvoice != value)
                {
                    _OutwardDrugInvoice = value;
                    RaisePropertyChanged("OutwardDrugInvoice");
                }
            }
        }

        [DataMemberAttribute()]
        public RefGenericDrugDetail RefGenericDrugDetail
        {
            get;
            set;
        }

        private string _PrescriptionIssueCode;
        private string _ICD10List;
        [DataMemberAttribute]
        public string PrescriptionIssueCode
        {
            get => _PrescriptionIssueCode; set
            {
                _PrescriptionIssueCode = value;
                RaisePropertyChanged("PrescriptionIssueCode");
                RaisePropertyChanged("PrescriptionGroupString");
            }
        }
        [DataMemberAttribute]
        public string ICD10List
        {
            get => _ICD10List; set
            {
                _ICD10List = value;
                RaisePropertyChanged("ICD10List");
                RaisePropertyChanged("PrescriptionGroupString");
            }
        }

        public string PrescriptionGroupString
        {
            get { return string.Format("{0}: {1}", PrescriptionIssueCode, ICD10List); }
        }
        //▼====== #001
        [DataMemberAttribute]
        public RefGenMedProductDetails GenMedProductItem
        {
            get
            {
                return _GenMedProductItem;
            }
            set
            {
                ValidateProperty("GenMedProductItem", value);
                _GenMedProductItem = value;
                RaisePropertyChanged("GenMedProductItem");
            }
        }
        private RefGenMedProductDetails _GenMedProductItem;
        //▲====== #001
        #endregion

        private PrescriptionDetail _PrescriptionDetailObj;
        private string _DoseString;
        [DataMemberAttribute]
        public PrescriptionDetail PrescriptionDetailObj
        {
            get => _PrescriptionDetailObj; set
            {
                _PrescriptionDetailObj = value;
                RaisePropertyChanged("PrescriptionDetailObj");
            }
        }
        [DataMemberAttribute]
        public string DoseString
        {
            get
            {
                return _DoseString;
            }
            set
            {
                _DoseString = value;
                RaisePropertyChanged("DoseString");
            }
        }

        private Decimal _InCost;
        [DataMemberAttribute]
        public Decimal InCost
        {
            get
            {
                return _InCost;
            }
            set
            {
                _InCost = value;
                RaisePropertyChanged("InCost");
            }
        }

        private decimal? _VAT;
        [DataMemberAttribute]
        public decimal? VAT
        {
            get
            {
                return _VAT;
            }
            set
            {
                _VAT = value;
                RaisePropertyChanged("VAT");
            }
        }

        private decimal _DrugVersionID;
        [DataMemberAttribute]
        public decimal DrugVersionID
        {
            get
            {
                return _DrugVersionID;
            }
            set
            {
                _DrugVersionID = value;
                RaisePropertyChanged("DrugVersionID");
            }
        }
    }
}