using System;
using System.Runtime.Serialization;
using Service.Core.Common;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    [DataContract]
    public partial class OutwardDrugClinicDept : MedRegItemBase, IDosage
    {
        public OutwardDrugClinicDept()
            : base()
        {
            ExamRegStatus = AllLookupValues.ExamRegStatus.BAT_DAU_THUC_HIEN;
        }

        #region Factory Method

        [DataMemberAttribute]
        public override long ID
        {
            get { return OutID; }
        }
        /// Create a new OutwardDrugClinicDept object.

        /// <param name="outID">Initial value of the OutID property.</param>
        /// <param name="outQuantity">Initial value of the OutQuantity property.</param>
        /// <param name="outPrice">Initial value of the OutPrice property.</param>
        /// <param name="outPriceDifference">Initial value of the OutPriceDifference property.</param>
        /// <param name="qty">Initial value of the Qty property.</param>
        /// <param name="qtyReturn">Initial value of the QtyReturn property.</param>
        public static OutwardDrugClinicDept CreateOutwardDrugClinicDept(Int64 outID, decimal outQuantity, Decimal outPrice, Decimal outPriceDifference, decimal qty, decimal qtyReturn)
        {
            OutwardDrugClinicDept outwardDrugClinicDept = new OutwardDrugClinicDept();
            outwardDrugClinicDept.OutID = outID;
            outwardDrugClinicDept.OutQuantity = outQuantity;
            outwardDrugClinicDept.OutPrice = outPrice;
            outwardDrugClinicDept.OutPriceDifference = outPriceDifference;
            outwardDrugClinicDept.Qty = qty;
            outwardDrugClinicDept.QtyReturn = qtyReturn;
            return outwardDrugClinicDept;
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
                OnGenMedProductIDChanging(value);
                _GenMedProductID = value;
                RaisePropertyChanged("GenMedProductID");
                OnGenMedProductIDChanged();
            }
        }
        private Nullable<Int64> _GenMedProductID;
        partial void OnGenMedProductIDChanging(Nullable<Int64> value);
        partial void OnGenMedProductIDChanged();

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
        public Nullable<Int64> InID_Orig
        {
            get
            {
                return _InID_Orig;
            }
            set
            {
                _InID_Orig = value;
                RaisePropertyChanged("InID_Orig");
            }
        }
        private Nullable<Int64> _InID_Orig;

        [CustomValidation(typeof(OutwardDrugClinicDept), "ValidateQtySell")]
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
                _OutAmount = _OutPrice * _OutQuantity;
                RaisePropertyChanged("OutQuantity");
                RaisePropertyChanged("OutAmount");
                OnOutQuantityChanged();
            }
        }
        private decimal _OutQuantity;
        partial void OnOutQuantityChanging(decimal value);
        partial void OnOutQuantityChanged();

        [DataMemberAttribute()]
        public decimal OutQuantity_Orig
        {
            get
            {
                return _OutQuantity_Orig;
            }
            set
            {
                _OutQuantity_Orig = value;
                RaisePropertyChanged("OutQuantity_Orig");
            }
        }
        private decimal _OutQuantity_Orig;


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
                _OutAmount = _OutPrice * _OutQuantity;
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
        public String OutNotes_Orig
        {
            get
            {
                return _OutNotes_Orig;
            }
            set
            {
                _OutNotes_Orig = value;
                RaisePropertyChanged("OutNotes_Orig");
            }
        }
        private String _OutNotes_Orig;

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

        private decimal _Qty = 1;
        [Range(0, int.MaxValue, ErrorMessage = "Giá trị số lượng không hợp lệ")]
        [DataMemberAttribute()]
        public override decimal Qty
        {
            get
            {
                return _Qty;
            }
            set
            {
                if (_Qty != value)
                {
                    _Qty = value;
                    RaisePropertyChanged("Qty");
                }
            }
        }

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
        public Nullable<Int64> OutClinicDeptReqID
        {
            get
            {
                return _OutClinicDeptReqID;
            }
            set
            {
                _OutClinicDeptReqID = value;
                RaisePropertyChanged("outiID");
            }
        }
        private Nullable<Int64> _OutClinicDeptReqID;

        #endregion
        #region Navigation Properties

        private InwardDrugClinicDept _InwardDrugClinicDept;
        [DataMemberAttribute()]
        public InwardDrugClinicDept InwardDrugClinicDept
        {
            get
            {
                return _InwardDrugClinicDept;
            }
            set
            {
                if (_InwardDrugClinicDept != value)
                {
                    _InwardDrugClinicDept = value;
                    //if (_InwardDrugClinicDept != null)
                    //{
                    //    _InID = _InwardDrugClinicDept.InID;
                    //}
                    //else
                    //{
                    //    _InID = null;
                    //}
                    RaisePropertyChanged("InwardDrugClinicDept");
                }
            }
        }
        #endregion

        private OutwardDrugClinicDeptInvoice _drugInvoice;
        [DataMemberAttribute()]
        public OutwardDrugClinicDeptInvoice DrugInvoice
        {
            get
            {
                return _drugInvoice;
            }
            set
            {
                _drugInvoice = value;
            }
        }

        public string InvoiceID
        {
            get
            {
                return DrugInvoice == null ? null : DrugInvoice.OutInvID;
            }
        }

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

        public override IChargeableItemPrice ChargeableItem
        {
            get
            {
                return _GenMedProductItem;
            }
        }

        public override string ChargeableItemCode
        {
            get
            {
                if (_GenMedProductItem != null)
                {
                    return _GenMedProductItem.Code;
                }
                return base.ChargeableItemCode;
            }
        }

        //Tam thoi de vay.
        public override string ChargeableItemName
        {
            get
            {
                if (_GenMedProductItem != null)
                {
                    return _GenMedProductItem.BrandName;
                }
                return base.ChargeableItemName;
            }
        }

        private RefGenMedProductDetails _GenMedProductItem;
        [DataMemberAttribute()]
        public RefGenMedProductDetails GenMedProductItem
        {
            get
            {
                return _GenMedProductItem;
            }
            set
            {
                _GenMedProductItem = value;
                RaisePropertyChanged("GenMedProductItem");
            }
        }


        [DataMemberAttribute()]
        public long V_MedicalMaterial
        {
            get
            {
                return _V_MedicalMaterial;
            }
            set
            {
                _V_MedicalMaterial = value;
                RaisePropertyChanged("V_MedicalMaterial");
            }
        }
        private long _V_MedicalMaterial;

        [DataMemberAttribute()]
        public long V_MedicalMaterial_Orig
        {
            get
            {
                return _V_MedicalMaterial_Orig;
            }
            set
            {
                _V_MedicalMaterial_Orig = value;
                RaisePropertyChanged("V_MedicalMaterial_Orig");
            }
        }
        private long _V_MedicalMaterial_Orig;

        //KMx: Vật tư y tế thay thế
        [DataMemberAttribute()]
        public bool IsReplaceMedMat
        {
            get
            {
                return _IsReplaceMedMat;
            }
            set
            {
                _IsReplaceMedMat = value;
                if (_IsReplaceMedMat)
                {
                    IsDisposeMedMat = !_IsReplaceMedMat;
                }
                RaisePropertyChanged("IsReplaceMedMat");
            }
        }
        private bool _IsReplaceMedMat;


        //KMx: Vật tư y tế tiêu hao
        [DataMemberAttribute()]
        public bool IsDisposeMedMat
        {
            get
            {
                return _IsDisposeMedMat;
            }
            set
            {
                _IsDisposeMedMat = value;
                if (_IsDisposeMedMat)
                {
                    IsReplaceMedMat = !_IsDisposeMedMat;
                }
                RaisePropertyChanged("IsDisposeMedMat");
            }
        }
        private bool _IsDisposeMedMat;

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

        [DataMemberAttribute()]
        public string MDoseStr
        {
            get
            {
                return _MDoseStr;
            }
            set
            {
                if (_MDoseStr != value)
                {
                    _MDoseStr = value;
                    RaisePropertyChanged("MDoseStr");
                }
            }
        }
        private string _MDoseStr = "0";


        [DataMemberAttribute()]
        public string ADoseStr
        {
            get
            {
                return _ADoseStr;
            }
            set
            {
                if (_ADoseStr != value)
                {
                    _ADoseStr = value;
                    RaisePropertyChanged("ADoseStr");
                }
            }
        }
        private string _ADoseStr = "0";

        [DataMemberAttribute()]
        public string EDoseStr
        {
            get
            {
                return _EDoseStr;
            }
            set
            {
                if (_EDoseStr != value)
                {
                    _EDoseStr = value;
                    RaisePropertyChanged("EDoseStr");
                }
            }
        }
        private string _EDoseStr = "0";

        [DataMemberAttribute()]
        public string NDoseStr
        {
            get
            {
                return _NDoseStr;
            }
            set
            {
                if (_NDoseStr != value)
                {
                    _NDoseStr = value;
                    RaisePropertyChanged("NDoseStr");
                }
            }
        }
        private string _NDoseStr = "0";


        [DataMemberAttribute()]
        public string MDoseStr_Orig
        {
            get
            {
                return _MDoseStr_Orig;
            }
            set
            {
                if (_MDoseStr_Orig != value)
                {
                    _MDoseStr_Orig = value;
                    RaisePropertyChanged("MDoseStr_Orig");
                }
            }
        }
        private string _MDoseStr_Orig;


        [DataMemberAttribute()]
        public string ADoseStr_Orig
        {
            get
            {
                return _ADoseStr_Orig;
            }
            set
            {
                if (_ADoseStr_Orig != value)
                {
                    _ADoseStr_Orig = value;
                    RaisePropertyChanged("ADoseStr_Orig");
                }
            }
        }
        private string _ADoseStr_Orig;

        [DataMemberAttribute()]
        public string EDoseStr_Orig
        {
            get
            {
                return _EDoseStr_Orig;
            }
            set
            {
                if (_EDoseStr_Orig != value)
                {
                    _EDoseStr_Orig = value;
                    RaisePropertyChanged("EDoseStr_Orig");
                }
            }
        }
        private string _EDoseStr_Orig;

        [DataMemberAttribute()]
        public string NDoseStr_Orig
        {
            get
            {
                return _NDoseStr_Orig;
            }
            set
            {
                if (_NDoseStr_Orig != value)
                {
                    _NDoseStr_Orig = value;
                    RaisePropertyChanged("NDoseStr_Orig");
                }
            }
        }
        private string _NDoseStr_Orig;

        [DataMemberAttribute()]
        public Single MDose
        {
            get
            {
                return _MDose;
            }
            set
            {
                _MDose = value;
                RaisePropertyChanged("MDose");

            }
        }
        private Single _MDose;

        [DataMemberAttribute()]
        public Single ADose
        {
            get
            {
                return _ADose;
            }
            set
            {
                _ADose = value;
                RaisePropertyChanged("ADose");

            }
        }
        private Single _ADose;

        [DataMemberAttribute()]
        public Single EDose
        {
            get
            {
                return _EDose;
            }
            set
            {
                _EDose = value;
                RaisePropertyChanged("EDose");
            }
        }
        private Single _EDose;

        [DataMemberAttribute()]
        public Single NDose
        {
            get
            {
                return _NDose;
            }
            set
            {
                _NDose = value;
                RaisePropertyChanged("NDose");
            }
        }
        private Single _NDose;

        private string _Administration;
        [DataMemberAttribute()]
        public string Administration
        {
            get
            {
                return _Administration;
            }
            set
            {
                _Administration = value;
                RaisePropertyChanged("Administration");
            }
        }

        private string _Administration_Orig;
        [DataMemberAttribute()]
        public string Administration_Orig
        {
            get
            {
                return _Administration_Orig;
            }
            set
            {
                _Administration_Orig = value;
                RaisePropertyChanged("Administration_Orig");
            }
        }

        public override bool Equals(object obj)
        {
            OutwardDrugClinicDept info = obj as OutwardDrugClinicDept;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.OutID > 0 && this.OutID == info.OutID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region Extention Member

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
        public DateTime InwardDate
        {
            get
            {
                return _InwardDate;
            }
            set
            {
                _InwardDate = value;
                RaisePropertyChanged("InwardDate");
            }
        }
        private DateTime _InwardDate;

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
        public decimal OutQuantityReturn
        {
            get
            {
                return _OutQuantityReturn;
            }
            set
            {
                OnOutQuantityReturnChanging(value);
                _OutQuantityReturn = value;

                _TotalPriceReturn = _OutQuantityReturn * _OutPrice;
                RaisePropertyChanged("OutQuantityReturn");
                RaisePropertyChanged("TotalPriceReturn");
                OnOutQuantityReturnChanged();
            }
        }
        private decimal _OutQuantityReturn;
        partial void OnOutQuantityReturnChanging(decimal value);
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

        partial void OnOutQuantityReturnChanging(decimal value)
        {
            string StrError2 = "Thuốc này đã hết hạn dùng.Không thể trả được!";
            string StrError1 = "Số lượng trả phải <= " + (OutQuantityOld - QtyReturn).ToString();
            string StrError0 = "Số lượng trả không hợp lệ.";
            if (value > 0)
            {
                RemoveError("OutQuantityReturn", StrError0);
                //if (AxHelper.CompareDate(DateTime.Now, InExpiryDate) == 2 || AxHelper.CompareDate(DateTime.Now, InExpiryDate) == 0)
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

        private long? _DrugDeptInIDOrig;
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
        #endregion

        public static ValidationResult ValidateQtySell(decimal OutQuantity, ValidationContext context)
        {
            if (OutQuantity < 0)
            {
                return new ValidationResult("Số lượng không được nhỏ hơn 0!", new string[] { "OutQuantity" });
            }
            if (((OutwardDrugClinicDept)context.ObjectInstance).QtyReturn > 0)
            {
                if (OutQuantity != ((OutwardDrugClinicDept)context.ObjectInstance).OutQuantityOld)
                {
                    return new ValidationResult("Thuốc/Y Cụ/Hóa Chất đã được trả hàng.Bạn không thể cập nhật!Bạn có thể tiếp tục trả hàng hoặc tạo hóa đơn mới!", new string[] { "OutQuantity" });
                }
            }
            if (((OutwardDrugClinicDept)context.ObjectInstance).GenMedProductItem != null)
            {
                if (OutQuantity > ((OutwardDrugClinicDept)context.ObjectInstance).GenMedProductItem.RemainingFirst)
                {
                    return new ValidationResult("Số lượng còn lại " + ((OutwardDrugClinicDept)context.ObjectInstance).GenMedProductItem.RemainingFirst.ToString() + " không đủ để bán!", new string[] { "OutQuantity" });
                }
            }

            return ValidationResult.Success;
        }

        private long _IntravenousPlan_InPtID;
        [DataMemberAttribute()]
        public long IntravenousPlan_InPtID
        {
            get
            {
                return _IntravenousPlan_InPtID;
            }
            set
            {
                if (_IntravenousPlan_InPtID != value)
                {
                    _IntravenousPlan_InPtID = value;
                    RaisePropertyChanged("IntravenousPlan_InPtID");
                }
            }
        }

        private string _Notes;
        [DataMemberAttribute()]
        public string Notes
        {
            get
            {
                return _Notes;
            }
            set
            {
                if (_Notes != value)
                {
                    _Notes = value;
                    RaisePropertyChanged("Notes");
                }
            }
        }

        private LimQtyHiItemMaxPaymtPerc _LimQtyAndHIPrice;
        [DataMemberAttribute]
        public LimQtyHiItemMaxPaymtPerc LimQtyAndHIPrice
        {
            get
            {
                return _LimQtyAndHIPrice;
            }
            set
            {
                if (_LimQtyAndHIPrice != value)
                {
                    _LimQtyAndHIPrice = value;
                    RaisePropertyChanged("LimQtyAndHIPrice");
                }
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

        private long? _GenMedVersionID;
        [DataMemberAttribute]
        public long? GenMedVersionID
        {
            get
            {
                return _GenMedVersionID;
            }
            set
            {
                if (_GenMedVersionID != value)
                {
                    _GenMedVersionID = value;
                    RaisePropertyChanged("GenMedVersionID");
                }

            }
        }

    }
}