using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Collections.Generic;

namespace DataEntities
{
    public partial class CostTableMedDept : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new CostTableMedDept object.

        /// <param name="CoID">Initial value of the CoID property.</param>
        /// <param name="CoNumber">Initial value of the CoNumber property.</param>
        /// <param name="InvoiceNumber">Initial value of the InvoiceNumber property.</param>
        /// <param name="InvoiceDate">Initial value of the InvoiceDate property.</param>
        /// <param name="CreateDate">Initial value of the CreateDate property.</param>
        /// <param name="tempRequireUpdate">Initial value of the TempRequireUpdate property.</param>
        /// <param name="vAT">Initial value of the VAT property.</param>
        public static CostTableMedDept CreateCostTableMedDept(Int64 CoID, String CoNumber, String InvoiceNumber, DateTime InvoiceDate, DateTime CreateDate, Decimal vAT)
        {
            CostTableMedDept CostTableMedDept = new CostTableMedDept();
            CostTableMedDept.CoID = CoID;
            CostTableMedDept.CoNumber = CoNumber;
            CostTableMedDept.InvoiceNumber = InvoiceNumber;
            CostTableMedDept.InvoiceDate = InvoiceDate;
            CostTableMedDept.CreateDate = CreateDate;
            CostTableMedDept.VAT = vAT;
            return CostTableMedDept;
        }

        #endregion

        #region Primitive Properties
        [DataMemberAttribute()]
        public long CoID
        {
            get
            {
                return _CoID;
            }
            set
            {
                if (_CoID != value)
                {
                    OnCoIDChanging(value);
                    _CoID = value;
                    RaisePropertyChanged("CoID");
                    OnCoIDChanged();
                }
            }
        }
        private long _CoID;
        partial void OnCoIDChanging(long value);
        partial void OnCoIDChanged();

        [DataMemberAttribute()]
        public Int64 StaffID
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
        private Int64 _StaffID;
        partial void OnStaffIDChanging(Int64 value);
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
        public String CoNumber
        {
            get
            {
                return _CoNumber;
            }
            set
            {
                if (_CoNumber != value)
                {
                    OnCoNumberChanging(value);
                    _CoNumber = value;
                    RaisePropertyChanged("CoNumber");
                    OnCoNumberChanged();
                }
            }
        }
        private String _CoNumber;
        partial void OnCoNumberChanging(String value);
        partial void OnCoNumberChanged();

        [DataMemberAttribute()]
        public String InvoiceNumber
        {
            get
            {
                return _InvoiceNumber;
            }
            set
            {
                OnInvoiceNumberChanging(value);
                _InvoiceNumber = value;
                RaisePropertyChanged("InvoiceNumber");
                OnInvoiceNumberChanged();
            }
        }
        private String _InvoiceNumber;
        partial void OnInvoiceNumberChanging(String value);
        partial void OnInvoiceNumberChanged();

        [DataMemberAttribute()]
        public DateTime InvoiceDate
        {
            get
            {
                return _InvoiceDate;
            }
            set
            {
                OnInvoiceDateChanging(value);
                _InvoiceDate = value;
                RaisePropertyChanged("InvoiceDate");
                OnInvoiceDateChanged();
            }
        }
        private DateTime _InvoiceDate;
        partial void OnInvoiceDateChanging(DateTime value);
        partial void OnInvoiceDateChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> CreateDate
        {
            get
            {
                return _CreateDate;
            }
            set
            {
                OnCreateDateChanging(value);
                _CreateDate = value;
                RaisePropertyChanged("CreateDate");
                OnCreateDateChanged();
            }
        }
        private Nullable<DateTime> _CreateDate;
        partial void OnCreateDateChanging(Nullable<DateTime> value);
        partial void OnCreateDateChanged();

        [CustomValidation(typeof(CostTableMedDept), "ValidateVAT")]
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
                RaisePropertyChanged("VATValueString");
                OnVATChanged();
                if (VAT >= 1 && VAT <= 2)
                {
                    TotalVAT = TotalNotVAT * VAT;
                    VATValue = TotalVAT - TotalNotVAT;
                    RaisePropertyChanged("TotalVAT");
                    RaisePropertyChanged("VATValue");
                }
                else
                {
                    TotalVAT = TotalNotVAT;
                    VATValue = TotalVAT - TotalNotVAT;
                    RaisePropertyChanged("TotalVAT");
                    RaisePropertyChanged("VATValue");
                }
            }
        }
        private Decimal _VAT;
        partial void OnVATChanging(Decimal value);
        partial void OnVATChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> CurrencyID
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
        private Nullable<Int64> _CurrencyID;
        partial void OnCurrencyIDChanging(Nullable<Int64> value);
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
            }
        }
        private Double _ExchangeRates;
        partial void OnExchangeRatesChanging(Double value);
        partial void OnExchangeRatesChanged();

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
        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        private ObservableCollection<InwardDrugMedDeptInvoice> _InwardDrugMedDeptInvoices;
        public ObservableCollection<InwardDrugMedDeptInvoice> InwardDrugMedDeptInvoices
        {
            get
            {
                return _InwardDrugMedDeptInvoices;
            }
            set
            {
                _InwardDrugMedDeptInvoices = value;
                RaisePropertyChanged("InwardDrugMedDeptInvoices");
            }
        }

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

        #endregion

        #region Convert XML

        public string ConvertDetailsListToXml()
        {
            return ConvertDetailsListToXml(_CostTableMedDeptLists);
        }

        public string ConvertDetailsListToXml(IEnumerable<CostTableMedDeptList> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<CostTableMedDeptLists>");
                foreach (CostTableMedDeptList details in items)
                {
                    sb.Append("<RecInfo>");
                    sb.AppendFormat("<CoID>{0}</CoID>", details.CoID);
                    sb.AppendFormat("<CoListName>{0}</CoListName>", details.CoListName);
                    sb.AppendFormat("<CoListID>{0}</CoListID>", details.CoListID);
                    sb.AppendFormat("<CoListNotes>{0}</CoListNotes>", details.CoListNotes);
                    sb.AppendFormat("<TotalValue>{0}</TotalValue>", details.TotalValue);
                    sb.Append("</RecInfo>");
                }
                sb.Append("</CostTableMedDeptLists>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Convert XML Invoive

        public string ConvertDetailsListToXmlInvoice()
        {
            return ConvertDetailsListToXmlInvoice(_InwardDrugMedDeptInvoices);
        }
        public string ConvertDetailsListToXmlInvoice(IEnumerable<InwardDrugMedDeptInvoice> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<InwardDrugMedDeptInvoices>");
                foreach (InwardDrugMedDeptInvoice details in items)
                {
                    sb.Append("<RecInfo>");
                    sb.AppendFormat("<inviID>{0}</inviID>", details.inviID);
                    sb.Append("</RecInfo>");
                }
                sb.Append("</InwardDrugMedDeptInvoices>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Extension member

        public string VATValueString
        {
            get
            {
                if (VAT != 0 && VAT >= 1)
                {
                    return ((VAT - 1) * 100).ToString("#,###.##") + "% VAT";
                }
                else
                {
                    return "0% VAT";
                }
            }
        }

        [DataMemberAttribute()]
        public decimal TotalNotVAT
        {
            get
            {
                return _TotalNotVAT;
            }
            set
            {
                if (_TotalNotVAT != value)
                {
                    _TotalNotVAT = value;
                    RaisePropertyChanged("TotalNotVAT");

                    if (VAT >= 1 && VAT <= 2)
                    {
                        TotalVAT = TotalNotVAT * VAT;
                        VATValue = TotalVAT - TotalNotVAT;
                        RaisePropertyChanged("TotalVAT");
                        RaisePropertyChanged("VATValue");
                    }
                    else
                    {
                        TotalVAT = TotalNotVAT;
                        VATValue = TotalVAT - TotalNotVAT;
                        RaisePropertyChanged("TotalVAT");
                        RaisePropertyChanged("VATValue");
                    }
                }
            }
        }
        private decimal _TotalNotVAT;

        [DataMemberAttribute()]
        public decimal VATValue
        {
            get
            {
                return _VATValue;
            }
            set
            {
                if (_VATValue != value)
                {
                    _VATValue = value;
                    RaisePropertyChanged("VATValue");
                }
            }
        }
        private decimal _VATValue;

        [DataMemberAttribute()]
        public decimal TotalVAT
        {
            get
            {
                return _TotalVAT;
            }
            set
            {
                if (_TotalVAT != value)
                {
                    _TotalVAT = value;
                    RaisePropertyChanged("TotalVAT");
                }
            }
        }
        private decimal _TotalVAT;

        #endregion

        #region Validation member

        public static ValidationResult ValidateVAT(decimal VAT, ValidationContext context)
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


        #endregion
    }

    public partial class CostTableMedDeptList : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new CostTableMedDept object.

        /// <param name="CoID">Initial value of the CoID property.</param>
        /// <param name="CoListID">Initial value of the CoNumber property.</param>
        /// <param name="CoListName">Initial value of the InvoiceNumber property.</param>
        /// <param name="TotalValue">Initial value of the VAT property.</param>
        public static CostTableMedDeptList CreateCostTableMedDeptList(Int64 CoListID, Int64 CoID, String CoListName, Decimal TotalValue)
        {
            CostTableMedDeptList CostTableMedDept = new CostTableMedDeptList();
            CostTableMedDept.CoID = CoID;
            CostTableMedDept.CoListID = CoListID;
            CostTableMedDept.CoListName = CoListName;
            CostTableMedDept.TotalValue = TotalValue;
            return CostTableMedDept;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public long CoListID
        {
            get
            {
                return _CoListID;
            }
            set
            {
                if (_CoListID != value)
                {
                    OnCoListIDChanging(value);
                    _CoListID = value;
                    RaisePropertyChanged("CoListID");
                    OnCoListIDChanged();
                }
            }
        }
        private long _CoListID;
        partial void OnCoListIDChanging(long value);
        partial void OnCoListIDChanged();

        [DataMemberAttribute()]
        public long CoID
        {
            get
            {
                return _CoID;
            }
            set
            {
                if (_CoID != value)
                {
                    OnCoIDChanging(value);
                    _CoID = value;
                    RaisePropertyChanged("CoID");
                    OnCoIDChanged();
                }
            }
        }
        private long _CoID;
        partial void OnCoIDChanging(long value);
        partial void OnCoIDChanged();

        [DataMemberAttribute()]
        public string CoListName
        {
            get
            {
                return _CoListName;
            }
            set
            {
                OnCoListNameChanging(value);
                _CoListName = value;
                RaisePropertyChanged("CoListName");
                OnCoListNameChanged();
            }
        }
        private string _CoListName;
        partial void OnCoListNameChanging(string value);
        partial void OnCoListNameChanged();

        [DataMemberAttribute()]
        public string CoListNotes
        {
            get
            {
                return _CoListNotes;
            }
            set
            {
                OnCoListNotesChanging(value);
                _CoListNotes = value;
                RaisePropertyChanged("CoListNotes");
                OnCoListNotesChanged();
            }
        }
        private string _CoListNotes;
        partial void OnCoListNotesChanging(string value);
        partial void OnCoListNotesChanged();

        [DataMemberAttribute()]
        public Decimal TotalValue
        {
            get
            {
                return _TotalValue;
            }
            set
            {
                OnTotalValueChanging(value);
                _TotalValue = value;
                RaisePropertyChanged("TotalValue");
                OnTotalValueChanged();
            }
        }
        private Decimal _TotalValue;
        partial void OnTotalValueChanging(Decimal value);
        partial void OnTotalValueChanged();

        #endregion

        #region Extension member

        [DataMemberAttribute()]
        public string CurrencyName
        {
            get
            {
                return _CurrencyName;
            }
            set
            {
                _CurrencyName = value;
                RaisePropertyChanged("CurrencyName");
            }
        }
        private string _CurrencyName;

        [DataMemberAttribute()]
        public Double ExchangeRates
        {
            get
            {
                return _ExchangeRates;
            }
            set
            {
                _ExchangeRates = value;
                RaisePropertyChanged("ExchangeRates");
            }
        }
        private Double _ExchangeRates;

        [DataMemberAttribute()]
        public Decimal TotalValueVAT
        {
            get
            {
                return _TotalValueVAT;
            }
            set
            {
                _TotalValueVAT = value;
                RaisePropertyChanged("TotalValueVAT");
            }
        }
        private Decimal _TotalValueVAT;

        [DataMemberAttribute()]
        public Decimal TotalValueDaDoi
        {
            get
            {
                return _TotalValueDaDoi;
            }
            set
            {
                _TotalValueDaDoi = value;
                RaisePropertyChanged("TotalValueDaDoi");
            }
        }
        private Decimal _TotalValueDaDoi;

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

        [DataMemberAttribute()]
        public Double Rates
        {
            get
            {
                return _Rates;
            }
            set
            {
                _Rates = value;
                RaisePropertyChanged("Rates");
            }
        }
        private Double _Rates;

        #endregion

    }

}
