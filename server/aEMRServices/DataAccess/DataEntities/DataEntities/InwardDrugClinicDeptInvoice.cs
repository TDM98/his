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
 */
namespace DataEntities
{
    public partial class InwardDrugClinicDeptInvoice : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new InwardDrugClinicDeptInvoice object.

        /// <param name="inviID">Initial value of the inviID property.</param>
        /// <param name="invID">Initial value of the InvID property.</param>
        /// <param name="invInvoiceNumber">Initial value of the InvInvoiceNumber property.</param>
        /// <param name="invDateInvoice">Initial value of the InvDateInvoice property.</param>
        /// <param name="dSPTModifiedDate">Initial value of the DSPTModifiedDate property.</param>
        /// <param name="tempRequireUpdate">Initial value of the TempRequireUpdate property.</param>
        /// <param name="vAT">Initial value of the VAT property.</param>
        public static InwardDrugClinicDeptInvoice CreateInwardDrugClinicDeptInvoice(Int64 inviID, String invID, String invInvoiceNumber, DateTime invDateInvoice, DateTime dSPTModifiedDate, Boolean tempRequireUpdate, Decimal vAT)
        {
            InwardDrugClinicDeptInvoice InwardDrugClinicDeptInvoice = new InwardDrugClinicDeptInvoice();
            InwardDrugClinicDeptInvoice.inviID = inviID;
            InwardDrugClinicDeptInvoice.InvID = invID;
            InwardDrugClinicDeptInvoice.InvInvoiceNumber = invInvoiceNumber;
            InwardDrugClinicDeptInvoice.InvDateInvoice = invDateInvoice;
            InwardDrugClinicDeptInvoice.DSPTModifiedDate = dSPTModifiedDate;
            InwardDrugClinicDeptInvoice.TempRequireUpdate = tempRequireUpdate;
            InwardDrugClinicDeptInvoice.VAT = vAT;
            return InwardDrugClinicDeptInvoice;
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
        [RegularExpression(@"^([\w-\.]+)$", ErrorMessage = "Số hóa đơn không hợp lệ")]
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
        public Decimal VAT
        {
            get
            {
                return _VAT;
            }
            set
            {
                OnVATChanging(value);
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
                        DiscountingByPercent = 1 + (_Discounting / (_TotalPriceNotVAT - _TotalDiscountOnProduct));
                        RaisePropertyChanged("DiscountingByPercent");
                    }
                }
                RaisePropertyChanged("Discounting");
            }
        }
        private decimal _Discounting;

        [DataMemberAttribute()]
        public decimal DiscountingByPercent
        {
            get
            {
                return _DiscountingByPercent;
            }
            set
            {
                _DiscountingByPercent = value;
                if (_IsPercent)
                {
                    if (_DiscountingByPercent != 0)
                    {
                        Discounting = (_DiscountingByPercent - 1) * (_TotalPriceNotVAT - _TotalDiscountOnProduct);
                    }
                    else
                    {
                        Discounting = 0;
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
                        Discounting = (_DiscountingByPercent - 1) * (_TotalPriceNotVAT - _TotalDiscountOnProduct);
                    }
                    else
                    {
                        Discounting = 0;
                    }
                    RaisePropertyChanged("Discounting");
                }
                else
                {
                    if (_TotalPriceNotVAT - _TotalDiscountOnProduct != 0)
                    {
                        DiscountingByPercent = 1 + (_Discounting / (_TotalPriceNotVAT - _TotalDiscountOnProduct));
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

        [DataMemberAttribute()]
        public long? outiID_Clinic
        {
            get
            {
                return _outiID_Clinic;
            }
            set
            {
                if (_outiID_Clinic != value)
                {
                    OnoutiID_ClinicChanging(value);
                    _outiID_Clinic = value;
                    RaisePropertyChanged("outiID_Clinic");
                    OnoutiID_ClinicChanged();
                }
            }
        }

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
                _OutDate = value;
                RaisePropertyChanged("OutDate");
            }
        }
        private DateTime? _OutDate;
        //▲====: #001

        private long? _outiID_Clinic;
        partial void OnoutiID_ClinicChanging(long? value);
        partial void OnoutiID_ClinicChanged();

        public bool CanEditAndDelete
        {
            get { return inviID > 0 && !CheckedPoint; }
        }
        public bool CanSave
        {
            get { return !CheckedPoint; }
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
        private ObservableCollection<InwardDrugClinicDept> _InwardDrugs;
        public ObservableCollection<InwardDrugClinicDept> InwardDrugs
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

        [CustomValidation(typeof(InwardDrugClinicDeptInvoice), "ValidateSupplier")]
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
                    else
                    {
                        _StoreIDOut = 0;
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


        private bool _isRequiredUpdate;
        [DataMemberAttribute()]
        public bool IsRequiredUpdate
        {
            get
            {
                return _isRequiredUpdate;
            }
            set
            {
                if (_isRequiredUpdate != value)
                {
                    _isRequiredUpdate = value;
                    RaisePropertyChanged("IsRequiredUpdate");
                }
            }
        }

        #endregion

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
                        Discounting = (_DiscountingByPercent - 1) * (_TotalPriceNotVAT - _TotalDiscountOnProduct);
                    }
                    else
                    {
                        Discounting = 0;
                    }
                    RaisePropertyChanged("Discounting");
                }
                else
                {
                    if (_TotalPriceNotVAT - _TotalDiscountOnProduct != 0)
                    {
                        DiscountingByPercent = 1 + (_Discounting / (_TotalPriceNotVAT - _TotalDiscountOnProduct));
                        RaisePropertyChanged("DiscountingByPercent");
                    }
                }
                RaisePropertyChanged("TotalPriceNotVAT");


            }
        }
        private Decimal _TotalPriceNotVAT;

        [DataMemberAttribute()]
        public Decimal TotalPriceVAT
        {
            get
            {
                return _TotalPriceVAT;
            }
            set
            {
                _TotalPriceVAT = value;

                RaisePropertyChanged("TotalPriceVAT");
            }
        }
        private Decimal _TotalPriceVAT;

        [DataMemberAttribute()]
        public Decimal TotalPriceActual
        {
            get
            {
                return _TotalPriceActual;
            }
            set
            {
                _TotalPriceActual = value;
                _SaiSo = _TotalPriceActual - _TotalPriceVAT;
                RaisePropertyChanged("SaiSo");
                RaisePropertyChanged("TotalPriceActual");
            }
        }
        private Decimal _TotalPriceActual;

        [DataMemberAttribute()]
        public Decimal SaiSo
        {
            get
            {
                return _SaiSo;
            }
            set
            {
                _SaiSo = value;
                RaisePropertyChanged("SaiSo");
            }
        }
        private Decimal _SaiSo;

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
                        Discounting = (_DiscountingByPercent - 1) * (_TotalPriceNotVAT - _TotalDiscountOnProduct);
                    }
                    else
                    {
                        Discounting = 0;
                    }
                    RaisePropertyChanged("Discounting");
                }
                else
                {
                    if (_TotalPriceNotVAT - _TotalDiscountOnProduct != 0)
                    {
                        DiscountingByPercent = 1 + (_Discounting / (_TotalPriceNotVAT - _TotalDiscountOnProduct));
                        RaisePropertyChanged("DiscountingByPercent");
                    }
                }
                RaisePropertyChanged("TotalDiscountOnProduct");
            }
        }
        private Decimal _TotalDiscountOnProduct;

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
            }
        }
        private Decimal _TotalDiscountInvoice;


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
            }
        }
        private Decimal _TotalPrice;
        #endregion

        #region Convert XML

        public bool ValidInwardDrugs()
        {
            if (null == _InwardDrugs)
            {
                return false;
            }
            foreach (InwardDrugClinicDept details in _InwardDrugs)
            {
                if (null == details.OutID || 0 == details.OutID) return false;
            }

            return true;
        }

        public string GetOutIDLst()
        {
            if (null == _InwardDrugs)
            {
                return null;
            }
            List<long> outIDLst = new List<long>();
            foreach (InwardDrugClinicDept details in _InwardDrugs)
            {
                if (null == details.OutID || 0 == details.OutID) return null;
                outIDLst.Add(details.OutID.Value);
            }

            return String.Join(",", outIDLst.ToArray());
        }

        public string ConvertDetailsListToXml()
        {
            return ConvertDetailsListToXml(_InwardDrugs);
        }
        public string ConvertDetailsListToXml(IEnumerable<InwardDrugClinicDept> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<InClinicDetails>");
                foreach (InwardDrugClinicDept details in items)
                {
                    if (details.RefGenMedProductDetails != null && details.RefGenMedProductDetails.GenMedProductID > 0)
                    {
                        int EntityState = (int)details.EntityState;
                        sb.Append("<RecInfo>");
                        sb.AppendFormat("<DrugDeptPoDetailID>{0}</DrugDeptPoDetailID>", details.DrugDeptPoDetailID);
                        sb.AppendFormat("<DrugDeptPoID>{0}</DrugDeptPoID>", details.DrugDeptPoID);
                        sb.AppendFormat("<GenMedProductID>{0}</GenMedProductID>", details.RefGenMedProductDetails.GenMedProductID);
                        sb.AppendFormat("<InQuantity>{0}</InQuantity>", details.InQuantity);
                        if (details.InProductionDate != null)
                        {
                            sb.AppendFormat("<InProductionDate>{0}</InProductionDate>", details.InProductionDate != null ? details.InProductionDate.GetValueOrDefault().ToString("yyyy/MM/dd") : null);
                        }
                        else
                        {
                            sb.AppendFormat("<InProductionDate>{0}</InProductionDate>", DBNull.Value);
                        }

                        if (this.outiID_Clinic.GetValueOrDefault() > 0)
                        {

                        }
                        sb.AppendFormat("<InExpiryDate>{0}</InExpiryDate>", details.InExpiryDate!=null ?details.InExpiryDate.GetValueOrDefault().ToString("yyyy/MM/dd"):null);
                        // 20191024 TNHX: 	0018493 Add InCost for InwardDrugs
                        sb.AppendFormat("<InBuyingPrice>{0}</InBuyingPrice>", details.InBuyingPrice);
                        sb.AppendFormat("<InCost>{0}</InCost>", details.InCost);
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
                        sb.AppendFormat("<GenMedVersionID>{0}</GenMedVersionID>", details.GenMedVersionID);
                        if (outiID_Clinic.GetValueOrDefault() > 0)
                        {
                            sb.AppendFormat("<OutID_Clinic>{0}</OutID_Clinic>", details.OutID);
                        }
                        else
                        {
                            sb.AppendFormat("<OutID>{0}</OutID>", details.OutID);
                        }
                        if (details.DrugDeptInIDOrig.GetValueOrDefault() > 0)
                        {
                            sb.AppendFormat("<DrugDeptInIDOrig>{0}</DrugDeptInIDOrig>", details.DrugDeptInIDOrig);
                        }
                        else
                        {
                            sb.AppendFormat("<DrugDeptInIDOrig>{0}</DrugDeptInIDOrig>", details.RefGenMedProductDetails.DrugDeptInIDOrig);
                        }
                        sb.AppendFormat("<EntityState>{0}</EntityState>", EntityState);
                        sb.AppendFormat("<VAT>{0}</VAT>", details.VAT);
                        sb.AppendFormat("<IsNotVat>{0}</IsNotVat>", details.IsNotVat);
                        sb.Append("</RecInfo>");
                    }
                }
                sb.Append("</InClinicDetails>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }

        #endregion

    }
}
