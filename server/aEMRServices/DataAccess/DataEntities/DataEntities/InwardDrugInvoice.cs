using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text;

namespace DataEntities
{
    public partial class InwardDrugInvoice : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new InwardDrugInvoice object.

        /// <param name="inviID">Initial value of the inviID property.</param>
        /// <param name="invID">Initial value of the InvID property.</param>
        /// <param name="invInvoiceNumber">Initial value of the InvInvoiceNumber property.</param>
        /// <param name="invDateInvoice">Initial value of the InvDateInvoice property.</param>
        /// <param name="dSPTModifiedDate">Initial value of the DSPTModifiedDate property.</param>
        /// <param name="tempRequireUpdate">Initial value of the TempRequireUpdate property.</param>
        /// <param name="vAT">Initial value of the VAT property.</param>
        public static InwardDrugInvoice CreateInwardDrugInvoice(long inviID, String invID, String invInvoiceNumber, DateTime invDateInvoice, DateTime dSPTModifiedDate, Boolean tempRequireUpdate, Decimal vAT)
        {
            InwardDrugInvoice inwardDrugInvoice = new InwardDrugInvoice();
            inwardDrugInvoice.inviID = inviID;
            inwardDrugInvoice.InvID = invID;
            inwardDrugInvoice.InvInvoiceNumber = invInvoiceNumber;
            inwardDrugInvoice.InvDateInvoice = invDateInvoice;
            inwardDrugInvoice.DSPTModifiedDate = dSPTModifiedDate;
            inwardDrugInvoice.TempRequireUpdate = tempRequireUpdate;
            inwardDrugInvoice.VAT = vAT;
            return inwardDrugInvoice;
        }

        #endregion

        #region Primitive Properties
        [DataMemberAttribute()]
        public long outiID
        {
            get
            {
                return _outiID;
            }
            set
            {
                if (_outiID != value)
                {
                    OnoutiIDChanging(value);
                    _outiID = value;
                    RaisePropertyChanged("outiID");

                    OnoutiIDChanged();
                }
            }
        }
        private long _outiID;
        partial void OnoutiIDChanging(long value);
        partial void OnoutiIDChanged();

        [DataMemberAttribute()]
        public long inviID
        {
            get
            {
                return _inviID;
            }
            set
            {
                if (_inviID != value)
                {
                    OninviIDChanging(value);
                    _inviID = value;
                    RaisePropertyChanged("CanEditAndDelete");
                    RaisePropertyChanged("CanUpdate");
                    RaisePropertyChanged("CanEdit");
                    RaisePropertyChanged("CanAdd");
                    RaisePropertyChanged("CanPrint");
                    RaisePropertyChanged("inviID");

                    OninviIDChanged();
                }
            }
        }
        private long _inviID;
        partial void OninviIDChanging(long value);
        partial void OninviIDChanged();

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
                _StaffID = value;
                RaisePropertyChanged("StaffID");
                OnStaffIDChanged();
            }
        }
        private Nullable<Int64> _StaffID;
        partial void OnStaffIDChanging(Nullable<Int64> value);
        partial void OnStaffIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> SupplierID
        {
            get
            {
                return _SupplierID;
            }
            set
            {
                OnSupplierIDChanging(value);
                _SupplierID = value;
                RaisePropertyChanged("SupplierID");
                OnSupplierIDChanged();
            }
        }
        private Nullable<long> _SupplierID;
        partial void OnSupplierIDChanging(Nullable<long> value);
        partial void OnSupplierIDChanged();
        [DataMemberAttribute()]
        public String InvID
        {
            get
            {
                return _InvID;
            }
            set
            {
                if (_InvID != value)
                {
                    OnInvIDChanging(value);
                    _InvID = value;
                    RaisePropertyChanged("InvID");
                    OnInvIDChanged();
                }
            }
        }
        private String _InvID;
        partial void OnInvIDChanging(String value);
        partial void OnInvIDChanged();

        [Required(ErrorMessage = "Bạn phải nhập số hóa đơn!")]
        //[RegularExpression(@"^([\w-\.]+)$", ErrorMessage = "Số hóa đơn không hợp lệ")]
        [DataMemberAttribute()]
        public String InvInvoiceNumber
        {
            get
            {
                return _InvInvoiceNumber;
            }
            set
            {
                OnInvInvoiceNumberChanging(value);
                ValidateProperty("InvInvoiceNumber", value);
                _InvInvoiceNumber = value;
                RaisePropertyChanged("InvInvoiceNumber");
                OnInvInvoiceNumberChanged();
            }
        }
        private String _InvInvoiceNumber;
        partial void OnInvInvoiceNumberChanging(String value);
        partial void OnInvInvoiceNumberChanged();

        [Required(ErrorMessage = "Bạn phải nhập ngày hóa đơn")]
        [DataMemberAttribute()]
        public DateTime InvDateInvoice
        {
            get
            {
                return _InvDateInvoice;
            }
            set
            {
                OnInvDateInvoiceChanging(value);
                ValidateProperty("InvDateInvoice", value);
                _InvDateInvoice = value;
                RaisePropertyChanged("InvDateInvoice");
                OnInvDateInvoiceChanged();
            }
        }
        private DateTime _InvDateInvoice;
        partial void OnInvDateInvoiceChanging(DateTime value);
        partial void OnInvDateInvoiceChanged();

        [DataMemberAttribute()]
        public DateTime DSPTModifiedDate
        {
            get
            {
                return _DSPTModifiedDate;
            }
            set
            {
                OnDSPTModifiedDateChanging(value);
                _DSPTModifiedDate = value;
                RaisePropertyChanged("DSPTModifiedDate");
                OnDSPTModifiedDateChanged();
            }
        }
        private DateTime _DSPTModifiedDate;
        partial void OnDSPTModifiedDateChanging(DateTime value);
        partial void OnDSPTModifiedDateChanged();

        [DataMemberAttribute()]
        public Boolean TempRequireUpdate
        {
            get
            {
                return _TempRequireUpdate;
            }
            set
            {
                OnTempRequireUpdateChanging(value);
                _TempRequireUpdate = value;
                RaisePropertyChanged("TempRequireUpdate");
                OnTempRequireUpdateChanged();
            }
        }
        private Boolean _TempRequireUpdate;
        partial void OnTempRequireUpdateChanging(Boolean value);
        partial void OnTempRequireUpdateChanged();


        [DataMemberAttribute()]
        public Decimal CustomTax
        {
            get
            {
                return _CustomTax;
            }
            set
            {
                OnCustomTaxChanging(value);
                // ValidateProperty("CustomTax", value);
                _CustomTax = value;
                RaisePropertyChanged("CustomTax");
                OnCustomTaxChanged();
            }
        }
        private Decimal _CustomTax;
        partial void OnCustomTaxChanging(Decimal value);
        partial void OnCustomTaxChanged();

        [CustomValidation(typeof(InwardDrugInvoice), "ValidateVAT")]
        [DataMemberAttribute()]
        public Decimal VAT
        {
            get
            {
                return _VAT;
            }
            set
            {
                OnVATChanging(value);
                ValidateProperty("VAT", value);
                _VAT = value;
                RaisePropertyChanged("VAT");
                OnVATChanged();
            }
        }
        private Decimal _VAT;
        partial void OnVATChanging(Decimal value);
        partial void OnVATChanged();

        [DataMemberAttribute()]
        public Int64? StoreIDOut
        {
            get
            {
                return _StoreIDOut;
            }
            set
            {
                _StoreIDOut = value;
                RaisePropertyChanged("StoreIDOut");
            }
        }
        private Int64? _StoreIDOut;

        [DataMemberAttribute()]
        public Int64 StoreID
        {
            get
            {
                return _StoreID;
            }
            set
            {
                OnStoreIDChanging(value);
                _StoreID = value;
                RaisePropertyChanged("StoreID");
                OnStoreIDChanged();
            }
        }
        private Int64 _StoreID;
        partial void OnStoreIDChanging(Int64 value);
        partial void OnStoreIDChanged();


        [DataMemberAttribute()]
        public Int64 CurrencyID
        {
            get
            {
                return _CurrencyID;
            }
            set
            {
                OnCurrencyIDChanging(value);
                _CurrencyID = value;
                RaisePropertyChanged("CurrencyID");
                OnCurrencyIDChanged();
            }
        }
        private Int64 _CurrencyID;
        partial void OnCurrencyIDChanging(Int64 value);
        partial void OnCurrencyIDChanged();

        [Range(0.0, 99999999999.0, ErrorMessage = "Tỷ Giá không được nhỏ hơn 0")]
        [DataMemberAttribute()]
        public Double ExchangeRates
        {
            get
            {
                return _ExchangeRates;
            }
            set
            {
                OnExchangeRatesChanging(value);
                ValidateProperty("ExchangeRates", value);
                _ExchangeRates = value;
                RaisePropertyChanged("ExchangeRates");
                OnExchangeRatesChanged();

                _TotalPriceActual_QĐ = _TotalPriceActual * (decimal)ExchangeRates;
                RaisePropertyChanged("TotalPriceActual_QĐ");

                RaisePropertyChanged("TotalDiscountInvoice_QĐ");
                RaisePropertyChanged("TotalDiscountOnProduct_QĐ");
                RaisePropertyChanged("TotalPrice_QĐ");
                RaisePropertyChanged("TotalPriceNotVAT_QĐ");
                RaisePropertyChanged("TotalPriceVAT_QĐ");
                RaisePropertyChanged("TotalHaveCustomTax_QĐ");
                RaisePropertyChanged("DifferenceValue_QĐ");
            }
        }
        private Double _ExchangeRates;
        partial void OnExchangeRatesChanging(Double value);
        partial void OnExchangeRatesChanged();

        [DataMemberAttribute()]
        public bool IsForeign
        {
            get
            {
                return _IsForeign;
            }
            set
            {
                _IsForeign = value;
                RaisePropertyChanged("IsForeign");
                RaisePropertyChanged("IsTrongNuoc");
            }
        }
        private bool _IsForeign = false;

        public bool IsTrongNuoc
        {
            get { return !IsForeign; }
            set { _IsForeign = !value; }
        }

        [DataMemberAttribute()]
        public string Notes
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
        private string _Notes;
        partial void OnNotesChanging(string value);
        partial void OnNotesChanged();

        [Range(0.0, 99999999999.0, ErrorMessage = "Tiền chiết khấu không được < 0")]
        [DataMemberAttribute()]
        public decimal Discounting
        {
            get
            {
                return _Discounting;
            }
            set
            {
                ValidateProperty("Discounting", value);
                _Discounting = value;
                if (!_IsPercent)
                {
                    if (_TotalPriceNotVAT - _TotalDiscountOnProduct != 0)
                    {
                        _DiscountingByPercent = 1 + (_Discounting / (_TotalPriceNotVAT - _TotalDiscountOnProduct));
                    }
                    else
                    {
                        _DiscountingByPercent = 1 + (_Discounting / (decimal)0.1);
                    }
                    RaisePropertyChanged("DiscountingByPercent");
                }
                RaisePropertyChanged("Discounting");
            }
        }
        private decimal _Discounting;

        [CustomValidation(typeof(InwardDrugInvoice), "ValidateDiscounting")]
        [DataMemberAttribute()]
        public decimal DiscountingByPercent
        {
            get
            {
                return _DiscountingByPercent;
            }
            set
            {
                ValidateProperty("DiscountingByPercent", value);
                _DiscountingByPercent = value;
                if (_IsPercent)
                {
                    if (_DiscountingByPercent != 0)
                    {
                        _Discounting = (_DiscountingByPercent - 1) * (_TotalPriceNotVAT - _TotalDiscountOnProduct);
                    }
                    else
                    {
                        _Discounting = 0;
                    }
                    RaisePropertyChanged("Discounting");
                }
                RaisePropertyChanged("DiscountingByPercent");
            }
        }
        private decimal _DiscountingByPercent;

        [DataMemberAttribute()]
        public bool IsPercent
        {
            get
            {
                return _IsPercent;
            }
            set
            {
                _IsPercent = value;
                if (_IsPercent)
                {
                    if (_DiscountingByPercent != 0)
                    {
                        _Discounting = (_DiscountingByPercent - 1) * (_TotalPriceNotVAT - _TotalDiscountOnProduct);
                    }
                    else
                    {
                        _Discounting = 0;
                    }
                    RaisePropertyChanged("Discounting");
                }
                else
                {
                    if (_TotalPriceNotVAT - _TotalDiscountOnProduct != 0)
                    {
                        _DiscountingByPercent = 1 + (_Discounting / (_TotalPriceNotVAT - _TotalDiscountOnProduct));
                        RaisePropertyChanged("DiscountingByPercent");
                    }
                }
                RaisePropertyChanged("CanPercentIsEnable");
                RaisePropertyChanged("IsPercent");
            }
        }
        private bool _IsPercent;


        [DataMemberAttribute()]
        public bool CheckedPoint
        {
            get
            {
                return _CheckedPoint;
            }
            set
            {
                _CheckedPoint = value;
                RaisePropertyChanged("CanEditAndDelete");
                RaisePropertyChanged("CanUpdate");
                RaisePropertyChanged("CanEdit");
                RaisePropertyChanged("CanAdd");
                RaisePropertyChanged("CheckedPoint");
            }
        }
        private bool _CheckedPoint;


        [DataMemberAttribute()]
        public Int64? PharmacySupplierPaymentReqID
        {
            get
            {
                return _PharmacySupplierPaymentReqID;
            }
            set
            {
                if (_PharmacySupplierPaymentReqID != value)
                {
                    OnPharmacySupplierPaymentReqIDChanging(value);
                    _PharmacySupplierPaymentReqID = value;
                    RaisePropertyChanged("PharmacySupplierPaymentReqID");
                    OnPharmacySupplierPaymentReqIDChanged();
                }
            }
        }
        private Int64? _PharmacySupplierPaymentReqID;
        partial void OnPharmacySupplierPaymentReqIDChanging(Int64? value);
        partial void OnPharmacySupplierPaymentReqIDChanged();

        [DataMemberAttribute()]
        public string PharmacySupplierPaymentNotes
        {
            get
            {
                return _PharmacySupplierPaymentNotes;
            }
            set
            {
                if (_PharmacySupplierPaymentNotes != value)
                {
                    _PharmacySupplierPaymentNotes = value;
                    RaisePropertyChanged("PharmacySupplierPaymentNotes");
                }
            }
        }
        private string _PharmacySupplierPaymentNotes;

        [DataMemberAttribute()]
        public Int64? TypID
        {
            get
            {
                return _TypID;
            }
            set
            {
                if (_TypID != value)
                {
                    _TypID = value;
                    RaisePropertyChanged("TypID");
                }
            }
        }
        private Int64? _TypID;

        public bool CanEditAndDelete
        {
            get { return inviID > 0 && !CheckedPoint && (PharmacySupplierPaymentReqID == null || PharmacySupplierPaymentReqID == 0); }
        }

        //KMx: CanUpdate và CanEdit được tách ra từ CanEditAndDelete. CanUpdate: Cho phép cập nhật sau khi kết chuyển.
        //CanEdit: Ngược lại với CanUpdate. (07/05/2014 11:10)
        public bool CanUpdate
        {
            get { return inviID > 0 && (PharmacySupplierPaymentReqID == null || PharmacySupplierPaymentReqID == 0); }
        }

        public bool CanEdit
        {
            get { return !CheckedPoint && (PharmacySupplierPaymentReqID == null || PharmacySupplierPaymentReqID == 0); }
        }

        public bool CanAdd
        {
            get { return inviID == 0; }
        }
        public bool CanPrint
        {
            get { return inviID > 0; }
        }
        public bool CanPercentIsEnable
        {
            get { return IsPercent; }
        }

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        private ObservableCollection<InwardDrug> _InwardDrugs;
        public ObservableCollection<InwardDrug> InwardDrugs
        {
            get
            {
                return _InwardDrugs;
            }
            set
            {
                _InwardDrugs = value;
                RaisePropertyChanged("InwardDrugs");
            }
        }

        [DataMemberAttribute()]
        public Staff SelectedStaff
        {
            get
            {
                return _selectedStaff;
            }
            set
            {
                OnSelectedStaffChanging(value);
                _selectedStaff = value;
                RaisePropertyChanged("SelectedStaff");
                OnSelectedStaffChanged();
            }
        }
        private Staff _selectedStaff;
        partial void OnSelectedStaffChanging(Staff value);
        partial void OnSelectedStaffChanged();

        [CustomValidation(typeof(InwardDrugInvoice), "ValidateSupplier")]
        [DataMemberAttribute()]
        public Supplier SelectedSupplier
        {
            get
            {
                return _selectedSupplier;
            }
            set
            {
                OnSelectedSupplierChanging(value);
                ValidateProperty("SelectedSupplier", value);
                _selectedSupplier = value;
                if (_selectedSupplier != null)
                {
                    _SupplierID = _selectedSupplier.SupplierID;
                }
                else
                {
                    _SupplierID = 0;
                }
                RaisePropertyChanged("SupplierID");
                RaisePropertyChanged("SelectedSupplier");
                OnSelectedSupplierChanged();
            }
        }
        private Supplier _selectedSupplier;
        partial void OnSelectedSupplierChanging(Supplier value);
        partial void OnSelectedSupplierChanged();

        public Currency SelectedCurrency
        {
            get
            {
                return _SelectedCurrency;
            }
            set
            {
                _SelectedCurrency = value;
                if (_SelectedCurrency != null)
                {
                    _CurrencyID = _SelectedCurrency.CurrencyID;
                }
                else
                {
                    _CurrencyID = 0;
                }
                RaisePropertyChanged("CurrencyID");
                RaisePropertyChanged("SelectedCurrency");
                RaisePropertyChanged("IsInputExchangeRate");
                if (SelectedCurrency == null || SelectedCurrency.CurrencyID == (long)AllLookupValues.CurrencyTable.VND)
                {
                    ExchangeRates = 0;
                }
            }
        }
        private Currency _SelectedCurrency;

        private RefStorageWarehouseLocation _SelectedStorageOut;
        [DataMemberAttribute()]
        public RefStorageWarehouseLocation SelectedStorageOut
        {
            get
            {
                return _SelectedStorageOut;
            }
            set
            {
                if (_SelectedStorageOut != value)
                {
                    _SelectedStorageOut = value;
                    if (_SelectedStorageOut != null)
                    {
                        _StoreIDOut = _SelectedStorageOut.StoreID;
                    }
                    RaisePropertyChanged("StoreIDOut");
                    RaisePropertyChanged("SelectedStorageOut");
                }
            }
        }

        private RefStorageWarehouseLocation _SelectedStorage;
        [Required(ErrorMessage = "Vui lòng chọn kho nhập vào.")]
        [DataMemberAttribute()]
        public RefStorageWarehouseLocation SelectedStorage
        {
            get
            {
                return _SelectedStorage;
            }
            set
            {
                if (_SelectedStorage != value)
                {
                    _SelectedStorage = value;
                    ValidateProperty("SelectedStorage", value);
                    if (_SelectedStorage != null)
                    {
                        _StoreID = _SelectedStorage.StoreID;
                    }

                    RaisePropertyChanged("SelectedStorage");
                }
            }
        }
        private bool _IsVATCredit = false;
        [DataMemberAttribute]
        public bool IsVATCredit
        {
            get => _IsVATCredit; set
            {
                _IsVATCredit = value;
                RaisePropertyChanged("IsVATCredit");
            }
        }
        #endregion

        public static ValidationResult ValidateSupplier(Supplier SelectedSupplier, ValidationContext context)
        {
            if (SelectedSupplier == null)
            {
                return new ValidationResult("Vui lòng chọn NCC!", new string[] { "SelectedSupplier" });
            }
            else
            {
                if (SelectedSupplier.SupplierID == 0)
                {
                    return new ValidationResult("Vui lòng chọn NCC!", new string[] { "SelectedSupplier" });
                }
                return ValidationResult.Success;
            }
        }

        #region Extention Member

        [DataMemberAttribute()]
        public String OutInvID
        {
            get
            {
                return _OutInvID;
            }
            set
            {
                if (_OutInvID != value)
                {
                    _OutInvID = value;
                    RaisePropertyChanged("OutInvID");
                }
            }
        }
        private String _OutInvID;

        //chi dung trong phieu de nghi thanh toan
        [DataMemberAttribute()]
        public Int32 TypeInvoice
        {
            get
            {
                return _TypeInvoice;
            }
            set
            {
                _TypeInvoice = value;
                RaisePropertyChanged("TypeInvoice");
            }
        }
        private Int32 _TypeInvoice;

        [DataMemberAttribute()]
        public Decimal TotalPriceNotVAT
        {
            get
            {
                return _TotalPriceNotVAT;
            }
            set
            {
                _TotalPriceNotVAT = value;
                if (IsPercent)
                {
                    if (_DiscountingByPercent != 0)
                    {
                        _Discounting = (_DiscountingByPercent - 1) * (_TotalPriceNotVAT - _TotalDiscountOnProduct);
                    }
                    else
                    {
                        _Discounting = 0;
                    }
                    RaisePropertyChanged("Discounting");
                }
                else
                {
                    if (_TotalPriceNotVAT - _TotalDiscountOnProduct != 0)
                    {
                        _DiscountingByPercent = 1 + (_Discounting / (_TotalPriceNotVAT - _TotalDiscountOnProduct));
                    }
                    else
                    {
                        _DiscountingByPercent = 1 + (_Discounting / (decimal)0.1);
                    }
                    RaisePropertyChanged("DiscountingByPercent");
                }
                RaisePropertyChanged("TotalPriceNotVAT");
                RaisePropertyChanged("TotalPriceNotVAT_QĐ");


            }
        }
        private Decimal _TotalPriceNotVAT;

        public Decimal TotalPriceNotVAT_QĐ
        {
            get { return TotalPriceNotVAT * (decimal)ExchangeRates; }//neu VND
        }

        [DataMemberAttribute()]
        public Decimal TotalPriceVAT
        {
            get
            {
                return _TotalPriceVAT;
            }
            set
            {
                if (_TotalPriceVAT != value)
                {
                    _TotalPriceVAT = value;
                    RaisePropertyChanged("TotalPriceVAT");
                    _TotalPriceActual = _TotalPriceVAT + _DifferenceValue;
                    RaisePropertyChanged("TotalPriceActual");
                    _TotalPriceActual_QĐ = _TotalPriceActual * (decimal)ExchangeRates;
                    RaisePropertyChanged("TotalPriceActual_QĐ");
                    RaisePropertyChanged("TotalPriceVAT_QĐ");
                    RaisePropertyChanged("DifferenceValue_QĐ");
                    RaisePropertyChanged("TotalVATAmount");
                }
            }
        }
        private Decimal _TotalPriceVAT;

        public Decimal TotalPriceVAT_QĐ
        {
            get { return TotalPriceVAT * (decimal)ExchangeRates; }//neu VND
        }

        [DataMemberAttribute()]
        public Decimal TotalPriceActual
        {
            get
            {
                return _TotalPriceActual;
            }
            set
            {
                if (_TotalPriceActual != value)
                {
                    _TotalPriceActual = value;
                    RaisePropertyChanged("TotalPriceActual");
                    _TotalPriceActual_QĐ = _TotalPriceActual * (decimal)ExchangeRates;
                    RaisePropertyChanged("TotalPriceActual_QĐ");
                    if (_TotalPriceVAT > 0)
                    {
                        _DifferenceValue = _TotalPriceActual - decimal.Parse(_TotalPriceVAT.ToString("#,0.##", System.Threading.Thread.CurrentThread.CurrentCulture));
                        RaisePropertyChanged("DifferenceValue");
                        RaisePropertyChanged("DifferenceValue_QĐ");
                    }
                }
            }
        }
        private Decimal _TotalPriceActual;

        private Decimal _TotalPriceActual_QĐ;
        public Decimal TotalPriceActual_QĐ
        {
            get { return _TotalPriceActual_QĐ; }
            //get { return TotalPriceActual * (decimal)ExchangeRates; }//neu VND
            set
            {
                if (_TotalPriceActual_QĐ != value)
                {
                    _TotalPriceActual_QĐ = value;
                    RaisePropertyChanged("TotalPriceActual_QĐ");
                    if (ExchangeRates > 0)
                    {
                        _TotalPriceActual = _TotalPriceActual_QĐ / (decimal)ExchangeRates;
                        RaisePropertyChanged("TotalPriceActual");
                        if (_TotalPriceVAT > 0)
                        {
                            _DifferenceValue = _TotalPriceActual - decimal.Parse(_TotalPriceVAT.ToString("#,0.##", System.Threading.Thread.CurrentThread.CurrentCulture));
                            RaisePropertyChanged("DifferenceValue");
                            RaisePropertyChanged("DifferenceValue_QĐ");
                        }
                    }
                }
            }
        }

        [DataMemberAttribute()]
        public Decimal DifferenceValue
        {
            get
            {
                return _DifferenceValue;
            }
            set
            {
                if (_DifferenceValue != value)
                {
                    _DifferenceValue = value;
                    RaisePropertyChanged("DifferenceValue");
                    RaisePropertyChanged("DifferenceValue_QĐ");
                    //if (_TotalPriceVAT > 0)
                    //{
                    //    _TotalPriceActual = _DifferenceValue + _TotalPriceVAT;
                    //    RaisePropertyChanged("TotalPriceActual");
                    //    _TotalPriceActual_QĐ = _TotalPriceActual * (decimal)ExchangeRates;
                    //    RaisePropertyChanged("TotalPriceActual_QĐ");
                    //    RaisePropertyChanged("DifferenceValue_QĐ");
                    //}
                }
            }
        }
        private Decimal _DifferenceValue;

        public Decimal DifferenceValue_QĐ
        {
            get { return _DifferenceValue * (decimal)ExchangeRates; }//neu VND
            //get
            //{ return _DifferenceValue_QĐ; }
            //set
            //{
            //    if (_DifferenceValue_QĐ != value)
            //    {
            //        _DifferenceValue_QĐ = value;
            //        RaisePropertyChanged("DifferenceValue_QĐ");
            //    }
            //}
        }

        [DataMemberAttribute()]
        public Decimal TotalDiscountOnProduct
        {
            get
            {
                return _TotalDiscountOnProduct;
            }
            set
            {
                _TotalDiscountOnProduct = value;
                if (IsPercent)
                {
                    if (_DiscountingByPercent != 0)
                    {
                        _Discounting = (_DiscountingByPercent - 1) * (_TotalPriceNotVAT - _TotalDiscountOnProduct);
                    }
                    else
                    {
                        _Discounting = 0;
                    }
                    RaisePropertyChanged("Discounting");
                }
                else
                {
                    if (_TotalPriceNotVAT - _TotalDiscountOnProduct != 0)
                    {
                        _DiscountingByPercent = 1 + (_Discounting / (_TotalPriceNotVAT - _TotalDiscountOnProduct));
                        RaisePropertyChanged("DiscountingByPercent");
                    }
                }
                RaisePropertyChanged("TotalDiscountOnProduct");
                RaisePropertyChanged("TotalDiscountOnProduct_QĐ");
            }
        }
        private Decimal _TotalDiscountOnProduct;

        public Decimal TotalDiscountOnProduct_QĐ
        {
            get { return TotalDiscountOnProduct * (decimal)ExchangeRates; }//neu VND
        }


        [DataMemberAttribute()]
        public Decimal TotalDiscountInvoice
        {
            get
            {
                return _TotalDiscountInvoice;
            }
            set
            {
                _TotalDiscountInvoice = value;
                RaisePropertyChanged("TotalDiscountInvoice");
                RaisePropertyChanged("TotalDiscountInvoice_QĐ");
            }
        }
        private Decimal _TotalDiscountInvoice;

        public Decimal TotalDiscountInvoice_QĐ
        {
            get { return TotalDiscountInvoice * (decimal)ExchangeRates; }//neu VND
        }

        [DataMemberAttribute()]
        public Decimal TotalHaveCustomTax
        {
            get
            {
                return _TotalHaveCustomTax;
            }
            set
            {
                _TotalHaveCustomTax = value;
                RaisePropertyChanged("TotalHaveCustomTax");
                RaisePropertyChanged("TotalHaveCustomTax_QĐ");
                RaisePropertyChanged("TotalVATAmount");
            }
        }
        private Decimal _TotalHaveCustomTax;

        public Decimal TotalHaveCustomTax_QĐ
        {
            get { return TotalHaveCustomTax * (decimal)ExchangeRates; }//neu VND
        }

        [DataMemberAttribute()]
        public Decimal TotalPrice
        {
            get
            {
                return _TotalPrice;
            }
            set
            {
                _TotalPrice = value;
                RaisePropertyChanged("TotalPrice");
                RaisePropertyChanged("TotalPrice_QĐ");
            }
        }
        private Decimal _TotalPrice;

        private Decimal _VATSearch;

        [DataMemberAttribute()]
        public Decimal VATSearch
        {
            get
            {
                return _VATSearch;
            }
            set
            {
                _VATSearch = value;
                RaisePropertyChanged("VATSearch");
            }
        }
        public Decimal TotalPrice_QĐ
        {
            get { return TotalPrice * (decimal)ExchangeRates; }//neu VND
        }

        public bool IsInputExchangeRate
        {
            get { return SelectedCurrency != null && SelectedCurrency.CurrencyID != (long)AllLookupValues.CurrencyTable.VND && SelectedCurrency.CurrencyID != 0; }//neu VND
        }
        #endregion

        #region Convert XML

        public string ConvertDetailsListToXml()
        {
            return ConvertDetailsListToXml(_InwardDrugs);
        }
        public string ConvertDetailsListToXml(IEnumerable<InwardDrug> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<InwardDrugDetails>");
                foreach (InwardDrug details in items)
                {
                    if (details.DrugID > 0)
                    {
                        int EntityState = (int)details.EntityState;
                        sb.Append("<RecInfo>");
                        sb.AppendFormat("<PharmacyPoDetailID>{0}</PharmacyPoDetailID>", details.PharmacyPoDetailID);
                        sb.AppendFormat("<PharmacyPoID>{0}</PharmacyPoID>", details.PharmacyPoID);
                        sb.AppendFormat("<DrugID>{0}</DrugID>", details.DrugID);
                        sb.AppendFormat("<InQuantity>{0}</InQuantity>", details.InQuantity);
                        sb.AppendFormat("<InProductionDate>{0}</InProductionDate>", !details.InProductionDate.HasValue ? null : details.InProductionDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                        sb.AppendFormat("<InExpiryDate>{0}</InExpiryDate>", !details.InExpiryDate.HasValue ? null : details.InExpiryDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                        sb.AppendFormat("<InBuyingPrice>{0}</InBuyingPrice>", details.InBuyingPrice);
                        sb.AppendFormat("<InBuyingPriceActual>{0}</InBuyingPriceActual>", details.InBuyingPriceActual);
                        sb.AppendFormat("<InBatchNumber>{0}</InBatchNumber>", details.InBatchNumber);
                        sb.AppendFormat("<InID>{0}</InID>", details.InID);
                        sb.AppendFormat("<inviID>{0}</inviID>", details.inviID);
                        sb.AppendFormat("<Discounting>{0}</Discounting>", details.Discounting);
                        sb.AppendFormat("<DiscountingByPercent>{0}</DiscountingByPercent>", details.DiscountingByPercent);
                        sb.AppendFormat("<HIAllowedPrice>{0}</HIAllowedPrice>", details.HIAllowedPrice);
                        sb.AppendFormat("<HIPatientPrice>{0}</HIPatientPrice>", details.HIPatientPrice);
                        sb.AppendFormat("<NormalPrice>{0}</NormalPrice>", details.NormalPrice);
                        sb.AppendFormat("<IsPercent>{0}</IsPercent>", details.IsPercent);
                        sb.AppendFormat("<SdlDescription>{0}</SdlDescription>", details.SdlDescription);
                        sb.AppendFormat("<SdlID>{0}</SdlID>", details.SdlID);
                        sb.AppendFormat("<V_GoodsType>{0}</V_GoodsType>", details.V_GoodsType);
                        sb.AppendFormat("<EntityState>{0}</EntityState>", EntityState);
                        sb.AppendFormat("<OutID>{0}</OutID>", details.OutID);
                        sb.AppendFormat("<VAT>{0}</VAT>", details.VAT);
                        //Them DrugVersionID cho phieu tang giam KK
                        sb.AppendFormat("<GenMedVersionID>{0}</GenMedVersionID>", details.DrugVersionID);
                        sb.AppendFormat("<IsNotVat>{0}</IsNotVat>", details.IsNotVat);
                        sb.Append("</RecInfo>");
                    }
                }
                sb.Append("</InwardDrugDetails>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }
        #endregion

        public static ValidationResult ValidateVAT(Decimal VAT, ValidationContext context)
        {
            if (VAT < 0)
            {
                return new ValidationResult("VAT không hợp lệ!", new string[] { "VAT" });
            }
            if (VAT > 0 && VAT < 1)
            {
                return new ValidationResult("VAT không hợp lệ!", new string[] { "VAT" });
            }
            if (VAT > 2)
            {
                return new ValidationResult("VAT không hợp lệ!", new string[] { "VAT" });
            }
            return ValidationResult.Success;
        }
        public static ValidationResult ValidateDiscounting(Decimal ValidateDiscounting, ValidationContext context)
        {
            if (ValidateDiscounting < 0)
            {
                return new ValidationResult("Chiết khấu không hợp lệ!", new string[] { "DiscountingByPercent" });
            }
            if (ValidateDiscounting > 0 && ValidateDiscounting < 1)
            {
                return new ValidationResult("Chiết khấu không hợp lệ!", new string[] { "DiscountingByPercent" });
            }
            if (ValidateDiscounting > 2)
            {
                return new ValidationResult("Chiết khấu không hợp lệ!", new string[] { "DiscountingByPercent" });
            }
            return ValidationResult.Success;
        }

        public decimal TotalVATAmount
        {
            get
            {
                return TotalPriceVAT - TotalHaveCustomTax;
            }
        }
        private decimal _TotalVATDifferenceAmount = 0;
        private decimal _TotalVATAmountActual = 0;
        [DataMemberAttribute]
        public decimal TotalVATDifferenceAmount
        {
            get => _TotalVATDifferenceAmount; set
            {
                _TotalVATDifferenceAmount = value;
                RaisePropertyChanged("TotalVATDifferenceAmount");
            }
        }
        [DataMemberAttribute]
        public decimal TotalVATAmountActual
        {
            get => _TotalVATAmountActual; set
            {
                _TotalVATAmountActual = value;
                TotalVATDifferenceAmount = TotalVATAmountActual - TotalVATAmount;
                RaisePropertyChanged("TotalVATAmountActual");
                RaisePropertyChanged("TotalVATDifferenceAmount");
            }
        }
    }
}