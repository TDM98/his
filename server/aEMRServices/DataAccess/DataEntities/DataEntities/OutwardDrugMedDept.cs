using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using Service.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class OutwardDrugMedDept : MedRegItemBase
    {
        public OutwardDrugMedDept()
        {
            MedProductType = AllLookupValues.MedProductType.THUOC;
        }
        #region Factory Method


        /// Create a new OutwardDrugMedDept object.

        /// <param name="outID">Initial value of the OutID property.</param>
        /// <param name="outQuantity">Initial value of the OutQuantity property.</param>
        /// <param name="outPrice">Initial value of the OutPrice property.</param>
        /// <param name="outPriceDifference">Initial value of the OutPriceDifference property.</param>
        /// <param name="qty">Initial value of the Qty property.</param>
        /// <param name="qtyReturn">Initial value of the QtyReturn property.</param>
        public static OutwardDrugMedDept CreateOutwardDrugMedDept(Int64 outID, Int32 outQuantity, Decimal outPrice, Decimal outPriceDifference, Int32 qtyReturn)
        {
            OutwardDrugMedDept outwardDrugMedDept = new OutwardDrugMedDept();
            outwardDrugMedDept.OutID = outID;
            outwardDrugMedDept.OutQuantity = outQuantity;
            outwardDrugMedDept.OutPrice = outPrice;
            outwardDrugMedDept.OutPriceDifference = outPriceDifference;
            outwardDrugMedDept.QtyReturn = qtyReturn;
            return outwardDrugMedDept;
        }

        #endregion
        #region Primitive Properties
        [DataMemberAttribute()]
        public Int64 OutID
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
        private Int64 _OutID;
        partial void OnOutIDChanging(Int64 value);
        partial void OnOutIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> outiID
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
        private Nullable<Int64> _outiID;
        partial void OnoutiIDChanging(Nullable<Int64> value);
        partial void OnoutiIDChanged();


        [DataMemberAttribute()]
        public Nullable<Int64> GenMedProductID
        {
            get
            {
                return _GenMedProductID;
            }
            set
            {
                _GenMedProductID = value;
                RaisePropertyChanged("GenMedProductID");
            }
        }
        private Nullable<Int64> _GenMedProductID;

        [DataMemberAttribute()]
        public Nullable<Int64> InID
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
        private Nullable<Int64> _InID;
        partial void OnInIDChanging(Nullable<Int64> value);
        partial void OnInIDChanged();

        [DataMemberAttribute()]
        public decimal RequestQty
        {
            get
            {
                return _RequestQty;
            }
            set
            {
                OnRequestQtyChanging(value);
                _RequestQty = value;
                RaisePropertyChanged("RequestQty");
                OnRequestQtyChanged();
            }
        }
        private decimal _RequestQty;
        partial void OnRequestQtyChanging(decimal value);
        partial void OnRequestQtyChanged();

        [CustomValidation(typeof(OutwardDrugMedDept), "ValidateQtySell")]
        [DataMemberAttribute()]
        public decimal OutQuantity
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
                OutAmount = _OutPrice * _OutQuantity;
                RaisePropertyChanged("OutQuantity");
                RaisePropertyChanged("OutAmount");
                OnOutQuantityChanged();
            }
        }
        private decimal _OutQuantity;
        partial void OnOutQuantityChanging(decimal value);
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
                _OutPrice = value;
                OutAmount = _OutPrice * _OutQuantity;
                RaisePropertyChanged("OutPrice");
                RaisePropertyChanged("OutAmount");
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

        [DataMemberAttribute()]
        public decimal QtyReturn
        {
            get
            {
                return _QtyReturn;
            }
            set
            {
                OnQtyReturnChanging(value);
                _QtyReturn = value;
                RaisePropertyChanged("QtyReturn");
                OnQtyReturnChanged();
            }
        }
        private decimal _QtyReturn;
        partial void OnQtyReturnChanging(decimal value);
        partial void OnQtyReturnChanged();

        [DataMemberAttribute()]
        public long ReqDrugInDetailID
        {
            get
            {
                return _ReqDrugInDetailID;
            }
            set
            {
                OnReqDrugInDetailIDChanging(value);
                _ReqDrugInDetailID = value;
                RaisePropertyChanged("ReqDrugInDetailID");
                OnReqDrugInDetailIDChanged();
            }
        }
        private long _ReqDrugInDetailID;
        partial void OnReqDrugInDetailIDChanging(long value);
        partial void OnReqDrugInDetailIDChanged();


        [DataMemberAttribute()]
        public double DayRpts
        {
            get
            {
                return _DayRpts;
            }
            set
            {
                OnDayRptsChanging(value);
                _DayRpts = value;
                RaisePropertyChanged("DayRpts");
                OnDayRptsChanged();
            }
        }
        private double _DayRpts;
        partial void OnDayRptsChanging(double value);
        partial void OnDayRptsChanged();


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
        public GetGenMedProductForSell GetGenMedProductForSell
        {
            get
            {
                return _GetGenMedProductForSell;
            }
            set
            {
                OnGetGenMedProductForSellChanging(value);
                _GetGenMedProductForSell = value;
                if (_GetGenMedProductForSell != null)
                {
                    DayRpts = _GetGenMedProductForSell.DayRpts;
                }
                else
                {
                    DayRpts = 0;
                }
                OnGetGenMedProductForSellChanged();
                RaisePropertyChanged("GetGenMedProductForSell");

            }
        }

        private GetGenMedProductForSell _GetGenMedProductForSell;
        partial void OnGetGenMedProductForSellChanging(GetGenMedProductForSell value);
        partial void OnGetGenMedProductForSellChanged();

        private Decimal _NormalPrice;
        [DataMemberAttribute()]
        public Decimal NormalPrice
        {
            get
            {
                return _NormalPrice;
            }
            set
            {
                _NormalPrice = value;
                RaisePropertyChanged("NormalPrice");

            }
        }


        [DataMemberAttribute()]
        public int QtyOffer
        {
            get
            {
                return _QtyOffer;
            }
            set
            {
                if (_QtyOffer != value)
                {
                    _QtyOffer = value;
                    RaisePropertyChanged("QtyOffer");
                }

            }
        }
        private int _QtyOffer;

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
        public decimal QtyForDay
        {
            get
            {
                return _QtyForDay;
            }
            set
            {
                _QtyForDay = value;
                RaisePropertyChanged("QtyForDay");
            }
        }
        private decimal _QtyForDay;


        [DataMemberAttribute()]
        public double QtyMaxAllowed
        {
            get
            {
                return _QtyMaxAllowed;
            }
            set
            {
                _QtyMaxAllowed = value;
                RaisePropertyChanged("QtyMaxAllowed");
            }
        }
        private double _QtyMaxAllowed;


        [DataMemberAttribute()]
        public Nullable<Single> QtySchedMon
        {
            get
            {
                return _QtySchedMon;
            }
            set
            {
                _QtySchedMon = value;
                RaisePropertyChanged("QtySchedMon");
            }
        }
        private Nullable<Single> _QtySchedMon;


        [DataMemberAttribute()]
        public Nullable<Single> QtySchedTue
        {
            get
            {
                return _QtySchedTue;
            }
            set
            {
                _QtySchedTue = value;
                RaisePropertyChanged("QtySchedTue");
            }
        }
        private Nullable<Single> _QtySchedTue;


        [DataMemberAttribute()]
        public Nullable<Single> QtySchedWed
        {
            get
            {
                return _QtySchedWed;
            }
            set
            {
                _QtySchedWed = value;
                RaisePropertyChanged("QtySchedWed");
            }
        }
        private Nullable<Single> _QtySchedWed;


        [DataMemberAttribute()]
        public Nullable<Single> QtySchedThu
        {
            get
            {
                return _QtySchedThu;
            }
            set
            {
                _QtySchedThu = value;
                RaisePropertyChanged("QtySchedThu");
            }
        }
        private Nullable<Single> _QtySchedThu;


        [DataMemberAttribute()]
        public Nullable<Single> QtySchedFri
        {
            get
            {
                return _QtySchedFri;
            }
            set
            {
                _QtySchedFri = value;
                RaisePropertyChanged("QtySchedFri");
            }
        }
        private Nullable<Single> _QtySchedFri;


        [DataMemberAttribute()]
        public Nullable<Single> QtySchedSat
        {
            get
            {
                return _QtySchedSat;
            }
            set
            {
                _QtySchedSat = value;
                RaisePropertyChanged("QtySchedSat");
            }
        }
        private Nullable<Single> _QtySchedSat;


        [DataMemberAttribute()]
        public Nullable<Single> QtySchedSun
        {
            get
            {
                return _QtySchedSun;
            }
            set
            {
                _QtySchedSun = value;
                RaisePropertyChanged("QtySchedSun");
            }
        }
        private Nullable<Single> _QtySchedSun;


        [DataMemberAttribute()]
        public Nullable<byte> SchedBeginDOW
        {
            get
            {
                return _SchedBeginDOW;
            }
            set
            {
                _SchedBeginDOW = value;
                RaisePropertyChanged("SchedBeginDOW");
            }
        }
        private Nullable<byte> _SchedBeginDOW;


        [DataMemberAttribute()]
        public double DispenseVolume
        {
            get
            {
                return _DispenseVolume;
            }
            set
            {
                _DispenseVolume = value;
                RaisePropertyChanged("DispenseVolume");
            }
        }
        private double _DispenseVolume;

        [DataMemberAttribute()]
        public Nullable<Boolean> HI
        {
            get
            {
                return _HI;
            }
            set
            {
                _HI = value;
                RaisePropertyChanged("HI");
            }
        }
        private Nullable<Boolean> _HI;

        #endregion
        #region Navigation Properties

        private RefGenMedProductDetails _RefGenericDrugDetail;
        [DataMemberAttribute()]
        public RefGenMedProductDetails RefGenericDrugDetail
        {
            get
            {
                return _RefGenericDrugDetail;
            }
            set
            {
                if (_RefGenericDrugDetail != value)
                {
                    _RefGenericDrugDetail = value;
                    if (_RefGenericDrugDetail != null)
                    {
                        _GenMedProductID = _RefGenericDrugDetail.GenMedProductID;
                    }
                    else
                    {
                        _GenMedProductID = null;
                    }
                    RaisePropertyChanged("RefGenericDrugDetail");
                }
            }
        }

        private InwardDrugMedDept _InwardDrugMedDept;
        [DataMemberAttribute()]
        public InwardDrugMedDept InwardDrugMedDept
        {
            get
            {
                return _InwardDrugMedDept;
            }
            set
            {
                _InwardDrugMedDept = value;
                RaisePropertyChanged("InwardDrugMedDept");
            }
        }
        #endregion

        private EntityState _EntityState = EntityState.NEW;
        [DataMemberAttribute()]
        public override EntityState EntityState
        {
            get
            {
                return _EntityState;
            }
            set
            {
                _EntityState = value;
                RaisePropertyChanged("EntityState");
            }
        }
        //Isload = 0; lay len nhung chua luu,2:da luu
        [DataMemberAttribute()]
        public int? IsLoad
        {
            get
            {
                return _IsLoad;
            }
            set
            {
                _IsLoad = value;
                RaisePropertyChanged("IsLoad");
            }
        }
        private int? _IsLoad;

        [DataMemberAttribute()]
        public bool? IsDeleted
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
        private bool? _IsDeleted;


        private RefGenMedProductDetails _RefGenericDrugItem;
        [DataMemberAttribute()]
        public RefGenMedProductDetails RefGenericDrugItem
        {
            get
            {
                return _RefGenericDrugItem;
            }
            set
            {
                if (_RefGenericDrugItem != value)
                {
                    _RefGenericDrugItem = value;
                    RaisePropertyChanged("RefGenericDrugItem");
                }
            }
        }


        #region IInvoiceItem Members


        public override IChargeableItemPrice ChargeableItem
        {
            get
            {
                //KMx: Sửa lại cho giống nhà thuốc, nếu không sẽ không có "giá BH đồng ý trả" khi xuất thuốc theo toa (29/11/2014 17:35).
                //return _RefGenericDrugItem;
                return _InwardDrugMedDept;
            }
        }

        #endregion

        public override string ChargeableItemName
        {
            get
            {
                if (_RefGenericDrugItem != null)
                {
                    return _RefGenericDrugItem.BrandName;
                }
                return base.ChargeableItemName;
            }
        }

        #region Validator Custommer
        public static ValidationResult ValidateQtySell(int OutQuantity, ValidationContext context)
        {
            if (OutQuantity < 0)
            {
                return new ValidationResult("Số lượng không được nhỏ hơn 0!", new string[] { "OutQuantity" });
            }
            //if (((OutwardDrugMedDept)context.ObjectInstance).RefGenericDrugDetail != null)
            //{
            //    if (((OutwardDrugMedDept)context.ObjectInstance).OutQuantity > ((OutwardDrugMedDept)context.ObjectInstance).RefGenericDrugDetail.RemainingFirst)
            //    {
            //        return new ValidationResult("Số lượng còn lại không đủ để xuất!", new string[] { "OutQuantity" });
            //    }
            //    return ValidationResult.Success;
            //}
            return ValidationResult.Success;
        }
        #endregion

        #region Extention Member
        [DataMemberAttribute()]
        public decimal OutQuantityOld
        {
            get
            {
                return _OutQuantityOld;
            }
            set
            {
                OnOutQuantityOldChanging(value);
                _OutQuantityOld = value;
                OnOutQuantityOldChanged();
            }
        }
        private decimal _OutQuantityOld;
        partial void OnOutQuantityOldChanging(decimal value);
        partial void OnOutQuantityOldChanged();

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
        public long STT
        {
            get
            {
                return _STT;
            }
            set
            {
                OnSTTChanging(value);
                _STT = value;
                RaisePropertyChanged("STT");
                OnSTTChanged();
            }
        }
        private long _STT;
        partial void OnSTTChanging(long value);
        partial void OnSTTChanged();

        [DataMemberAttribute()]
        public String SdlDescription
        {
            get
            {
                return _SdlDescription;
            }
            set
            {
                OnSdlDescriptionChanging(value);
                _SdlDescription = value;
                RaisePropertyChanged("SdlDescription");
                OnSdlDescriptionChanged();
            }
        }
        private String _SdlDescription;
        partial void OnSdlDescriptionChanging(String value);
        partial void OnSdlDescriptionChanged();

        [DataMemberAttribute()]
        public DateTime? InExpiryDate
        {
            get
            {
                return _InExpiryDate;
            }
            set
            {
                OnInExpiryDateChanging(value);
                _InExpiryDate = value;
                RaisePropertyChanged("InExpiryDate");
                OnInExpiryDateChanged();

            }
        }
        private DateTime? _InExpiryDate;
        partial void OnInExpiryDateChanging(DateTime? value);
        partial void OnInExpiryDateChanged();

        [DataMemberAttribute()]
        public int OutQuantityReturn
        {
            get
            {
                return _OutQuantityReturn;
            }
            set
            {
                OnOutQuantityReturnChanging(value);
                _OutQuantityReturn = value;

                TotalPriceReturn = _OutQuantityReturn * _OutPrice;
                RaisePropertyChanged("OutQuantityReturn");
                RaisePropertyChanged("TotalPriceReturn");
                OnOutQuantityReturnChanged();
            }
        }
        private int _OutQuantityReturn;
        partial void OnOutQuantityReturnChanging(int value);
        partial void OnOutQuantityReturnChanged();

        public decimal TotalPriceReturn
        {
            get
            {
                return _TotalPriceReturn;
            }
            set
            {
                _TotalPriceReturn = value;
                RaisePropertyChanged("TotalPriceReturn");
            }
        }
        private decimal _TotalPriceReturn;

        [DataMemberAttribute()]
        public Nullable<Decimal> OutHIRebateReturn
        {
            get
            {
                return _OutHIRebateReturn;
            }
            set
            {
                _OutHIRebateReturn = value;
                RaisePropertyChanged("OutHIRebateReturn");
            }
        }
        private Nullable<Decimal> _OutHIRebateReturn;

        [DataMemberAttribute()]
        public Nullable<Decimal> PatientReturn
        {
            get
            {
                return _PatientReturn;
            }
            set
            {
                _PatientReturn = value;
                RaisePropertyChanged("PatientReturn");
            }
        }
        private Nullable<Decimal> _PatientReturn;


        private bool SoSanhNgay(DateTime t1, DateTime t2)
        {
            //false:t1>t2 ;true t2 >= t1 
            int year1 = t1.Year;
            int year2 = t2.Year;
            int month1 = t1.Month;
            int month2 = t2.Month;
            int day1 = t1.Day;
            int day2 = t2.Day;
            if (year1 > year2)
            {
                //t1>t2
                return false;
            }
            else
            {
                if (year1 == year2)
                {
                    if (month1 > month2)
                    {
                        return false;
                    }
                    else
                    {
                        if (month1 == month2)
                        {
                            if (day1 > day2)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return true;
        }

        partial void OnOutQuantityReturnChanging(int value)
        {
            string StrError2 = "Thuốc này đã hết hạn dùng.Không thể trả được!";
            string StrError1 = "Số lượng trả phải <= " + (OutQuantityOld - QtyReturn).ToString();
            string StrError0 = "Số lượng trả không hợp lệ.";
            if (value > 0)
            {
                RemoveError("OutQuantityReturn", StrError0);
                //if (!SoSanhNgay(DateTime.Now, InExpiryDate))
                //{
                //    AddError("OutQuantityReturn", StrError2, false);
                //}
                //else
                //{
                //    RemoveError("OutQuantityReturn", StrError2);
                //}


                if (value > (OutQuantityOld - QtyReturn))
                {
                    AddError("OutQuantityReturn", StrError1, false);
                }
                else
                {
                    RemoveError("OutQuantityReturn", StrError1);
                }
            }
            else if (value < 0)
            {
                AddError("OutQuantityReturn", StrError0, false);
                RemoveError("OutQuantityReturn", StrError1);
                RemoveError("OutQuantityReturn", StrError2);
            }
            else
            {
                RemoveError("OutQuantityReturn", StrError0);
                RemoveError("OutQuantityReturn", StrError1);
                RemoveError("OutQuantityReturn", StrError2);
            }

        }
        #endregion


        //Xuất Tạm cái này phải kết với InwardDrugMedDeptInvoice thì mới ra SupplierID nào
        private ObservableCollection<DrugDeptSupplier> _ObjSupplierList;
        [DataMemberAttribute()]
        public ObservableCollection<DrugDeptSupplier> ObjSupplierList
        {
            get
            {
                return _ObjSupplierList;
            }
            set
            {
                _ObjSupplierList = value;
                RaisePropertyChanged("ObjSupplierList");
            }
        }

        private Nullable<long> _SupplierID;
        [DataMemberAttribute()]
        public Nullable<long> SupplierID
        {
            get
            {
                return _SupplierID;
            }
            set
            {
                _SupplierID = value;
                RaisePropertyChanged("SupplierID");
            }
        }


        private DrugDeptSupplier _ObjSupplierID;
        [DataMemberAttribute()]
        public DrugDeptSupplier ObjSupplierID
        {
            get
            {
                return _ObjSupplierID;
            }
            set
            {
                _ObjSupplierID = value;
                RaisePropertyChanged("ObjSupplierID");
            }
        }
        //Xuất Tạm cái này phải kết với InwardDrugMedDeptInvoice thì mới ra SupplierID nào

        private decimal _Remaining;
        [DataMemberAttribute()]
        public decimal Remaining
        {
            get
            {
                return _Remaining;
            }
            set
            {
                _Remaining = value;
                RaisePropertyChanged("Remaining");
            }
        }

        private double? _HIPaymentPercent;
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

        private string _Visa;
        private string _BidCode;
        private long? _DrugDeptInIDOrig;
        [DataMemberAttribute]
        public string Visa
        {
            get => _Visa; set
            {
                _Visa = value;
                RaisePropertyChanged("Visa");
            }
        }
        [DataMemberAttribute]
        public string BidCode
        {
            get => _BidCode; set
            {
                _BidCode = value;
                RaisePropertyChanged("BidCode");
            }
        }
        [DataMemberAttribute]
        public long? DrugDeptInIDOrig
        {
            get
            {
                return _DrugDeptInIDOrig;
            }
            set
            {
                if (_DrugDeptInIDOrig == value)
                {
                    return;
                }
                _DrugDeptInIDOrig = value;
                RaisePropertyChanged("DrugDeptInIDOrig");
            }
        }

        [DataMemberAttribute]
        public double? VAT
        {
            get
            {
                return _VAT;
            }
            set
            {
                if (_VAT == value)
                {
                    return;
                }
                _VAT = value;
                RaisePropertyChanged("VAT");
            }
        }
        private double? _VAT;
    }
}