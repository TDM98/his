using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Collections.Generic;
/*
 * 20190522 #001 TNHX:  [BM0010766] Thêm trường ngày xuất của phiếu xuất để nhập vào kho
 * 20221114 #002 QTD    Thêm trường đánh dấu phiếu nhập trả kho ký gửi
 * 20221210 #003 QTD    Thêm trường đánh dấu Cho phép cập nhật phiếu nhập NCC
 */
namespace DataEntities
{
    public partial class InwardDrugMedDeptInvoice : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new InwardDrugMedDeptInvoice object.

        /// <param name="inviID">Initial value of the inviID property.</param>
        /// <param name="invID">Initial value of the InvID property.</param>
        /// <param name="invInvoiceNumber">Initial value of the InvInvoiceNumber property.</param>
        /// <param name="invDateInvoice">Initial value of the InvDateInvoice property.</param>
        /// <param name="dSPTModifiedDate">Initial value of the DSPTModifiedDate property.</param>
        /// <param name="tempRequireUpdate">Initial value of the TempRequireUpdate property.</param>
        /// <param name="vAT">Initial value of the VAT property.</param>
        public static InwardDrugMedDeptInvoice CreateInwardDrugMedDeptInvoice(Int64 inviID, String invID, String invInvoiceNumber, DateTime invDateInvoice, DateTime dSPTModifiedDate, Boolean tempRequireUpdate, Decimal vAT)
        {
            InwardDrugMedDeptInvoice inwardDrugMedDeptInvoice = new InwardDrugMedDeptInvoice();
            inwardDrugMedDeptInvoice.inviID = inviID;
            inwardDrugMedDeptInvoice.InvID = invID;
            inwardDrugMedDeptInvoice.InvInvoiceNumber = invInvoiceNumber;
            inwardDrugMedDeptInvoice.InvDateInvoice = invDateInvoice;
            inwardDrugMedDeptInvoice.DSPTModifiedDate = dSPTModifiedDate;
            inwardDrugMedDeptInvoice.TempRequireUpdate = tempRequireUpdate;
            inwardDrugMedDeptInvoice.VAT = vAT;
            return inwardDrugMedDeptInvoice;
        }

        #endregion

        #region Primitive Properties
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
                    RaisePropertyChanged("CanAdd");
                    RaisePropertyChanged("CanPrint");
                    RaisePropertyChanged("inviID");
                    RaisePropertyChanged("CanUpdate");

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

        [CustomValidation(typeof(InwardDrugMedDeptInvoice), "ValidateVAT")]
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

        [DataMemberAttribute()]
        public Int64 V_MedProductType
        {
            get
            {
                return _V_MedProductType;
            }
            set
            {
                OnV_MedProductTypeChanging(value);
                _V_MedProductType = value;
                RaisePropertyChanged("V_MedProductType");
                OnV_MedProductTypeChanged();
            }
        }
        private Int64 _V_MedProductType;
        partial void OnV_MedProductTypeChanging(Int64 value);
        partial void OnV_MedProductTypeChanged();

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

        [DataMemberAttribute]
        public bool IsForeign
        {
            get
            {
                return _IsForeign;
            }
            set
            {
                _IsForeign = value;
                if (_IsForeign)
                {
                    IsTrongNuoc = !_IsForeign;
                }
                RaisePropertyChanged("IsForeign");
            }
        }
        private bool _IsForeign = false;
        private bool _IsTrongNuoc = true;
        [DataMemberAttribute]
        public bool IsTrongNuoc
        {
            get
            {
                return _IsTrongNuoc;
            }
            set
            {
                _IsTrongNuoc = value;
                if (_IsTrongNuoc)
                {
                    IsForeign = !_IsTrongNuoc;
                }
                RaisePropertyChanged("IsTrongNuoc");
            }
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
                        DiscountingByPercent = 1 + (_Discounting / (decimal)0.1);
                    }
                    ValidateProperty("DiscountingByPercent", _DiscountingByPercent);
                    RaisePropertyChanged("DiscountingByPercent");
                }
                RaisePropertyChanged("Discounting");
               
            }
        }
        private decimal _Discounting;

        [CustomValidation(typeof(InwardDrugMedDeptInvoice), "ValidateDiscounting")]
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
                    ValidateProperty("Discounting", _Discounting);
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
                RaisePropertyChanged("CanAdd");
                RaisePropertyChanged("CanSave");
                RaisePropertyChanged("CheckedPoint");
            }
        }
        private bool _CheckedPoint;


        [DataMemberAttribute()]
        public Int64? DrugDeptSupplierPaymentReqID
        {
            get
            {
                return _DrugDeptSupplierPaymentReqID;
            }
            set
            {
                if (_DrugDeptSupplierPaymentReqID != value)
                {
                    OnDrugDeptSupplierPaymentReqIDChanging(value);
                    _DrugDeptSupplierPaymentReqID = value;
                    RaisePropertyChanged("DrugDeptSupplierPaymentReqID");
                    OnDrugDeptSupplierPaymentReqIDChanged();
                }
            }
        }
        private Int64? _DrugDeptSupplierPaymentReqID;
        partial void OnDrugDeptSupplierPaymentReqIDChanging(Int64? value);
        partial void OnDrugDeptSupplierPaymentReqIDChanged();

        [DataMemberAttribute()]
        public string DrugDeptSupplierPaymentNotes
        {
            get
            {
                return _DrugDeptSupplierPaymentNotes;
            }
            set
            {
                if (_DrugDeptSupplierPaymentNotes != value)
                {
                    _DrugDeptSupplierPaymentNotes = value;
                    RaisePropertyChanged("DrugDeptSupplierPaymentNotes");
                }
            }
        }
        private string _DrugDeptSupplierPaymentNotes;

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

        public bool IsInputExchangeRate
        {
            get { return SelectedCurrency != null && SelectedCurrency.CurrencyID != 129 && SelectedCurrency.CurrencyID != 0; }//neu VND
        }

        public bool CanEditAndDelete
        {
            get { return inviID > 0 && !CheckedPoint && (DrugDeptSupplierPaymentReqID == null || DrugDeptSupplierPaymentReqID == 0); }
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


        [DataMemberAttribute()]
        public bool IsInputTemp
        {
            get
            {
                return _IsInputTemp;
            }
            set
            {
                OnIsInputTempChanging(value);
                ValidateProperty("IsInputTemp", value);
                _IsInputTemp = value;
                RaisePropertyChanged("IsInputTemp");
                OnIsInputTempChanged();
            }
        }
        private bool _IsInputTemp;
        partial void OnIsInputTempChanging(bool value);
        partial void OnIsInputTempChanged();

        //ma phieu xuat ma kho duoc
        [DataMemberAttribute()]
        public long? outiID
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
        private long? _outiID;
        partial void OnoutiIDChanging(long? value);
        partial void OnoutiIDChanged();

        private bool _IsCheckBuyingPrice;
        public bool IsCheckBuyingPrice
        {
            get
            {
                return _IsCheckBuyingPrice;
            }
            set
            {
                if (_IsCheckBuyingPrice == value) return;
                _IsCheckBuyingPrice = value;
                RaisePropertyChanged("IsCheckBuyingPrice");
            }
        }

        [DataMemberAttribute]
        public string ReturnInvInvoiceNumber
        {
            get
            {
                return _ReturnInvInvoiceNumber;
            }
            set
            {
                _ReturnInvInvoiceNumber = value;
                RaisePropertyChanged("ReturnInvInvoiceNumber");
            }
        }
        private string _ReturnInvInvoiceNumber;
        [DataMemberAttribute]
        public string ReturnSerialNumber
        {
            get
            {
                return _ReturnSerialNumber;
            }
            set
            {
                _ReturnSerialNumber = value;
                RaisePropertyChanged("ReturnSerialNumber");
            }
        }
        private string _ReturnSerialNumber;
        [DataMemberAttribute]
        public string ReturnInvoiceForm
        {
            get
            {
                return _ReturnInvoiceForm;
            }
            set
            {
                _ReturnInvoiceForm = value;
                RaisePropertyChanged("ReturnInvoiceForm");
            }
        }
        private string _ReturnInvoiceForm;
        [DataMemberAttribute]
        public string ReturnNote
        {
            get
            {
                return _ReturnNote;
            }
            set
            {
                _ReturnNote = value;
                RaisePropertyChanged("ReturnNote");
            }
        }
        private string _ReturnNote;

        [CustomValidation(typeof(InwardDrugMedDeptInvoice), "ValidateVAT")]
        [DataMemberAttribute]
        public decimal ReturnVAT
        {
            get
            {
                return _ReturnVAT;
            }
            set
            {
                _ReturnVAT = value;
                RaisePropertyChanged("ReturnVAT");
            }
        }
        private decimal _ReturnVAT = 1;

        [DataMemberAttribute]
        public long ReturnoutiID
        {
            get
            {
                return _ReturnoutiID;
            }
            set
            {
                _ReturnoutiID = value;
                RaisePropertyChanged("ReturnoutiID");
            }
        }
        private long _ReturnoutiID;
        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        private ObservableCollection<CostTableMedDeptList> _CostTableMedDeptLists;
        public ObservableCollection<CostTableMedDeptList> CostTableMedDeptLists
        {
            get
            {
                return _CostTableMedDeptLists;
            }
            set
            {
                _CostTableMedDeptLists = value;
                RaisePropertyChanged("CostTableMedDeptLists");
            }
        }

        public OutwardDrugClinicDeptInvoice CurrentOutwardDrugClinicDeptInvoice
        {
            get
            {
                return _CurrentOutwardDrugClinicDeptInvoice;
            }
            set
            {
                _CurrentOutwardDrugClinicDeptInvoice = value;
                if (_CurrentOutwardDrugClinicDeptInvoice != null)
                {
                    _SelectedStorageOut = _CurrentOutwardDrugClinicDeptInvoice.SelectedStorage;
                    RaisePropertyChanged("SelectedStorageOut");
                }
                RaisePropertyChanged("CurrentOutwardDrugClinicDeptInvoice");
            }
        }
        private OutwardDrugClinicDeptInvoice _CurrentOutwardDrugClinicDeptInvoice;

        public OutwardDrugMedDeptInvoice CurrentOutwardDrugMedDeptInvoice
        {
            get
            {
                return _CurrentOutwardDrugMedDeptInvoice;
            }
            set
            {
                _CurrentOutwardDrugMedDeptInvoice = value;
                if (_CurrentOutwardDrugMedDeptInvoice != null)
                {
                    _SelectedStorageOut = _CurrentOutwardDrugMedDeptInvoice.SelectedStorage;
                    RaisePropertyChanged("SelectedStorageOut");
                }
                RaisePropertyChanged("CurrentOutwardDrugMedDeptInvoice");
            }
        }
        private OutwardDrugMedDeptInvoice _CurrentOutwardDrugMedDeptInvoice;

        [DataMemberAttribute()]
        private ObservableCollection<InwardDrugMedDept> _InwardDrugs;
        public ObservableCollection<InwardDrugMedDept> InwardDrugs
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

        [CustomValidation(typeof(InwardDrugMedDeptInvoice), "ValidateSupplier")]
        [DataMemberAttribute()]
        public DrugDeptSupplier SelectedSupplier
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
        private DrugDeptSupplier _selectedSupplier;
        partial void OnSelectedSupplierChanging(DrugDeptSupplier value);
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
                    RaisePropertyChanged("SelectedStorage");
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
        #endregion

        #region Extention Member

        [Required(ErrorMessage = "Bạn phải nhập số seri")]
        [DataMemberAttribute()]
        public String SerialNumber
        {
            get
            {
                return _SerialNumber;
            }
            set
            {
                if (_SerialNumber != value)
                {
                    _SerialNumber = value;
                    ValidateProperty("SerialNumber", value);
                    RaisePropertyChanged("SerialNumber");
                }
            }
        }
        private String _SerialNumber;

        [DataMemberAttribute()]
        public String InvoiceForm
        {
            get
            {
                return _InvoiceForm;
            }
            set
            {
                if (_InvoiceForm != value)
                {
                    _InvoiceForm = value;
                    RaisePropertyChanged("InvoiceForm");
                }
            }
        }
        private String _InvoiceForm;

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

        //▼====: #001
        [DataMemberAttribute()]
        public DateTime? OutDate
        {
            get
            {
                return _OutDate;
            }
            set
            {
                if (_OutDate != value)
                {
                    _OutDate = value;
                    RaisePropertyChanged("OutDate");
                }
            }
        }
        private DateTime? _OutDate;
        //▲====: #001

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
                    if (_TotalPriceVAT > 0)
                    {
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
                        _DifferenceValue = _TotalPriceActual - decimal.Parse(_TotalPriceVAT.ToString("#,0.####", System.Threading.Thread.CurrentThread.CurrentCulture));
                        RaisePropertyChanged("DifferenceValue");
                        RaisePropertyChanged("DifferenceValue_QĐ");
                    }
                }
            }
        }
        private Decimal _TotalPriceActual;

          [DataMemberAttribute()]
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
                    //if (_TotalPriceVAT > 0)
                    //{
                    //    _TotalPriceActual = _DifferenceValue + _TotalPriceVAT;
                    //    RaisePropertyChanged("TotalPriceActual");
                    //    _TotalPriceActual_QĐ = _TotalPriceActual * (decimal)ExchangeRates;
                    //    RaisePropertyChanged("TotalPriceActual_QĐ");
                    //    RaisePropertyChanged("DifferenceValue_QĐ");
                    //}
                    RaisePropertyChanged("DifferenceValue_QĐ");
                }
            }
        }
        private Decimal _DifferenceValue;

        public Decimal DifferenceValue_QĐ
        {
             get { return _DifferenceValue * (decimal)ExchangeRates; }//neu VND
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

        public Decimal TotalPrice_QĐ
        {
            get { return TotalPrice * (decimal)ExchangeRates; }//neu VND
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

        private decimal _VATSearch = 0;
        [DataMemberAttribute]
        public decimal VATSearch
        {
            get => _VATSearch; set
            {
                _VATSearch = value;
                RaisePropertyChanged("VATSearch");
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

        #region Convert XML
        public string ConvertDetailsListToXml()
        {
            return ConvertDetailsListToXml(_InwardDrugs);
        }

        public string ConvertDetailsListToXml(IEnumerable<InwardDrugMedDept> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<InwardDrugDetails>");
                foreach (InwardDrugMedDept details in items)
                {
                    if (details.RefGenMedProductDetails != null)
                    {
                        int EntityState = (int)details.EntityState;
                        sb.Append("<RecInfo>");
                        sb.AppendFormat("<DrugDeptPoDetailID>{0}</DrugDeptPoDetailID>", details.DrugDeptPoDetailID);
                        sb.AppendFormat("<DrugDeptPoID>{0}</DrugDeptPoID>", details.DrugDeptPoID);
                        sb.AppendFormat("<GenMedProductID>{0}</GenMedProductID>", details.RefGenMedProductDetails.GenMedProductID);
                        sb.AppendFormat("<InQuantity>{0}</InQuantity>", details.InQuantity);
                        sb.AppendFormat("<InProductionDate>{0}</InProductionDate>", details.InProductionDate == null ? null : details.InProductionDate.GetValueOrDefault().ToString("yyyy/MM/dd"));
                        sb.AppendFormat("<InExpiryDate>{0}</InExpiryDate>", details.InExpiryDate == null ? null : details.InExpiryDate.GetValueOrDefault().ToString("yyyy/MM/dd"));
                        sb.AppendFormat("<InBuyingPrice>{0}</InBuyingPrice>", details.InBuyingPrice);
                        // VuTTM - Them InCost cho viec luu thong tin phieu nhap
                        sb.AppendFormat("<InCost>{0}</InCost>", details.InCost);
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
                        sb.AppendFormat("<GenMedVersionID>{0}</GenMedVersionID>", details.GenMedVersionID);
                        sb.AppendFormat("<OutID>{0}</OutID>", details.OutID);
                        sb.AppendFormat("<EntityState>{0}</EntityState>", EntityState);
                        sb.AppendFormat("<DrugDeptInIDOrig>{0}</DrugDeptInIDOrig>", details.DrugDeptInIDOrig);
                        sb.AppendFormat("<VAT>{0}</VAT>", details.VAT);
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

        #region Validation member
        public static ValidationResult ValidateSupplier(DrugDeptSupplier SelectedSupplier, ValidationContext context)
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
        #endregion

        public bool CanSave
        {
            get { return !CheckedPoint; }
        }

        public override bool Equals(object obj)
        {
            InwardDrugMedDeptInvoice info = obj as InwardDrugMedDeptInvoice;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.inviID > 0 && this.inviID == info.inviID;
        }

        public override int GetHashCode()
        {  
            return this.inviID.GetHashCode();
        }
        //▼==== #002
        private bool _IsReturnInvoiceConsignment;
        public bool IsReturnInvoiceConsignment
        {
            get
            {
                return _IsReturnInvoiceConsignment;
            }
            set
            {
                if (_IsReturnInvoiceConsignment == value) return;
                _IsReturnInvoiceConsignment = value;
                RaisePropertyChanged("IsReturnInvoiceConsignment");
            }
        }
        //▲==== #002

        public bool CanUpdate
        {
            get { return inviID > 0 && (DrugDeptSupplierPaymentReqID == null || DrugDeptSupplierPaymentReqID == 0); }
        }
    }
}