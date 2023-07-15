using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class PharmacyPurchaseOrder : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new PharmacyPurchaseOrder object.

        /// <param name="pharmacyPoID">Initial value of the PharmacyPoID property.</param>
        /// <param name="supplierID">Initial value of the SupplierID property.</param>
        public static PharmacyPurchaseOrder CreatePharmacyPurchaseOrder(Int64 pharmacyPoID, Int64 supplierID)
        {
            PharmacyPurchaseOrder pharmacyPurchaseOrder = new PharmacyPurchaseOrder();
            pharmacyPurchaseOrder.PharmacyPoID = pharmacyPoID;
            pharmacyPurchaseOrder.SupplierID = supplierID;
            return pharmacyPurchaseOrder;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 PharmacyPoID
        {
            get
            {
                return _PharmacyPoID;
            }
            set
            {
                if (_PharmacyPoID != value)
                {
                    OnPharmacyPoIDChanging(value);
                    _PharmacyPoID = value;
                    RaisePropertyChanged("CanPrint");
                    RaisePropertyChanged("CanDelete");
                    RaisePropertyChanged("PharmacyPoID");
                    OnPharmacyPoIDChanged();

                    RaisePropertyChanged("CanNew");
                }
            }
        }
        private Int64 _PharmacyPoID;
        partial void OnPharmacyPoIDChanging(Int64 value);
        partial void OnPharmacyPoIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> PharmacyEstimatePoID
        {
            get
            {
                return _PharmacyEstimatePoID;
            }
            set
            {
                OnPharmacyEstimatePoIDChanging(value);
                _PharmacyEstimatePoID = value;
                RaisePropertyChanged("PharmacyEstimatePoID");
                OnPharmacyEstimatePoIDChanged();
            }
        }
        private Nullable<Int64> _PharmacyEstimatePoID;
        partial void OnPharmacyEstimatePoIDChanging(Nullable<Int64> value);
        partial void OnPharmacyEstimatePoIDChanged();

        [DataMemberAttribute()]
        public Int64 SupplierID
        {
            get
            {
                return _SupplierID;
            }
            set
            {
                if (_SupplierID != value)
                {
                    OnSupplierIDChanging(value);
                    _SupplierID = value;
                    RaisePropertyChanged("SupplierID");
                    OnSupplierIDChanged();
                }
            }
        }
        private Int64 _SupplierID;
        partial void OnSupplierIDChanging(Int64 value);
        partial void OnSupplierIDChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> OrderDate
        {
            get
            {
                return _OrderDate;
            }
            set
            {
                OnOrderDateChanging(value);
                _OrderDate = value;
                RaisePropertyChanged("OrderDate");
                OnOrderDateChanged();
            }
        }
        private Nullable<DateTime> _OrderDate = DateTime.Now;
        partial void OnOrderDateChanging(Nullable<DateTime> value);
        partial void OnOrderDateChanged();

        [DataMemberAttribute()]
        public String PONumber
        {
            get
            {
                return _PONumber;
            }
            set
            {
                OnPONumberChanging(value);
                _PONumber = value;
                RaisePropertyChanged("PONumber");
                OnPONumberChanged();
            }
        }
        private String _PONumber;
        partial void OnPONumberChanging(String value);
        partial void OnPONumberChanged();


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

                RaisePropertyChanged("CanNew");
            }
        }
        private Double _ExchangeRates;
        partial void OnExchangeRatesChanging(Double value);
        partial void OnExchangeRatesChanged();

        [Range(0.0, 999999999.0, ErrorMessage = "VAT không được < 0")]
        [DataMemberAttribute()]
        public Double VAT
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

                RaisePropertyChanged("CanNew");
            }
        }
        private Double _VAT;
        partial void OnVATChanging(Double value);
        partial void OnVATChanged();

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
        public DateTime RecDateCreated
        {
            get
            {
                return _RecDateCreated;
            }
            set
            {
                OnRecDateCreatedChanging(value);
                _RecDateCreated = value;
                RaisePropertyChanged("RecDateCreated");
                OnRecDateCreatedChanged();
            }
        }
        private DateTime _RecDateCreated;
        partial void OnRecDateCreatedChanging(DateTime value);
        partial void OnRecDateCreatedChanged();

        [Range(0, 999999999, ErrorMessage = "Số ngày giao hàng không hợp lệ")]
        [DataMemberAttribute()]
        public int DeliveryDayNo
        {
            get
            {
                return _DeliveryDayNo;
            }
            set
            {
                ValidateProperty("DeliveryDayNo", value);
                _DeliveryDayNo = value;
                RaisePropertyChanged("DeliveryDayNo");

                RaisePropertyChanged("CanNew");
            }
        }
        private int _DeliveryDayNo;


        [Range(0, 999999999, ErrorMessage = "Số ngày nhận tiền không hợp lệ")]
        [DataMemberAttribute()]
        public int DeliveryMoneyDayNo
        {
            get
            {
                return _DeliveryMoneyDayNo;
            }
            set
            {
                ValidateProperty("DeliveryMoneyDayNo", value);
                _DeliveryMoneyDayNo = value;
                RaisePropertyChanged("DeliveryMoneyDayNo");

                RaisePropertyChanged("CanNew");
            }
        }
        private int _DeliveryMoneyDayNo;


        [DataMemberAttribute()]
        public DateTime? ExpectedDeliveryDate
        {
            get
            {
                return _ExpectedDeliveryDate;
            }
            set
            {
                OnExpectedDeliveryDateChanging(value);
                _ExpectedDeliveryDate = value;
                RaisePropertyChanged("ExpectedDeliveryDate");
                OnExpectedDeliveryDateChanged();
            }
        }
        private DateTime? _ExpectedDeliveryDate;
        partial void OnExpectedDeliveryDateChanging(DateTime? value);
        partial void OnExpectedDeliveryDateChanged();

        [DataMemberAttribute()]
        public DateTime? DeliveryDate
        {
            get
            {
                return _DeliveryDate;
            }
            set
            {
                OnDeliveryDateChanging(value);
                _DeliveryDate = value;
                RaisePropertyChanged("DeliveryDate");
                OnDeliveryDateChanged();
            }
        }
        private DateTime? _DeliveryDate;
        partial void OnDeliveryDateChanging(DateTime? value);
        partial void OnDeliveryDateChanged();

        [DataMemberAttribute()]
        public string PoNotes
        {
            get
            {
                return _PoNotes;
            }
            set
            {
                OnPoNotesChanging(value);
                _PoNotes = value;
                RaisePropertyChanged("PoNotes");
                OnPoNotesChanged();

                RaisePropertyChanged("CanNew");
            }
        }
        private string _PoNotes;
        partial void OnPoNotesChanging(string value);
        partial void OnPoNotesChanged();

        [DataMemberAttribute()]
        public Int64 V_PaymentMode
        {
            get
            {
                return _V_PaymentMode;
            }
            set
            {
                OnV_PaymentModeChanging(value);
                _V_PaymentMode = value;
                RaisePropertyChanged("V_PaymentMode");
                OnV_PaymentModeChanged();
            }
        }
        private Int64 _V_PaymentMode;
        partial void OnV_PaymentModeChanging(Int64 value);
        partial void OnV_PaymentModeChanged();

        [DataMemberAttribute()]
        public Int64 V_PurchaseOrderStatus
        {
            get
            {
                return _V_PurchaseOrderStatus;
            }
            set
            {
                if (_V_PurchaseOrderStatus != value)
                {
                    OnPharmacyPoIDChanging(value);
                    _V_PurchaseOrderStatus = value;
                    RaisePropertyChanged("CanSave");
                    RaisePropertyChanged("CanDelete");
                    RaisePropertyChanged("CanWaiting");
                    RaisePropertyChanged("V_PurchaseOrderStatus");
                    OnV_PurchaseOrderStatusChanged();
                }
            }
        }
        private Int64 _V_PurchaseOrderStatus;
        partial void OnV_PurchaseOrderStatusChanging(Int64 value);
        partial void OnV_PurchaseOrderStatusChanged();

        [DataMemberAttribute()]
        public string V_PurchaseOrderStatusName
        {
            get
            {
                return _V_PurchaseOrderStatusName;
            }
            set
            {
                _V_PurchaseOrderStatusName = value;
                RaisePropertyChanged("V_PurchaseOrderStatusName");
            }
        }
        private string _V_PurchaseOrderStatusName;

        #endregion

        public bool CanNew
        {
            get { return (PharmacyPoID > 0 || ExchangeRates > 0 || VAT > 1  || DeliveryDayNo > 0 || DeliveryMoneyDayNo > 0 ||!string.IsNullOrEmpty(PoNotes) || (PurchaseOrderDetails != null && PurchaseOrderDetails.Count > 0) || (SelectedSupplier != null && SelectedSupplier.SupplierID > 0) || (PharmacyEstimationForPO != null && PharmacyEstimationForPO.PharmacyEstimatePoID > 0)); }
        }

        public bool CanWaiting
        {
            get { return (V_PurchaseOrderStatus == (long)AllLookupValues.V_PurchaseOrderStatus.ORDERED || V_PurchaseOrderStatus == (long)AllLookupValues.V_PurchaseOrderStatus.PART_DELIVERY); }
        }

        public bool CanSave
        {
            get { return V_PurchaseOrderStatus==0 || (V_PurchaseOrderStatus == (long)AllLookupValues.V_PurchaseOrderStatus.NEW); }
        }
        public bool CanDelete
        {
            get { return PharmacyPoID > 0 && (V_PurchaseOrderStatus == (long)AllLookupValues.V_PurchaseOrderStatus.NEW); }
        }
        public bool CanPrint
        {
            get { return PharmacyPoID > 0; }
        }

        #region Navigation Properties
         [CustomValidation(typeof(PharmacyPurchaseOrder), "ValidateSelectedSupplier")]
        [DataMemberAttribute()]
        public Supplier SelectedSupplier
        {
            get
            {
                return _SelectedSupplier;
            }
            set
            {
                if (_SelectedSupplier != value)
                {
                    OnSelectedSupplierChanging(value);
                    ValidateProperty("SelectedSupplier", value);
                    _SelectedSupplier = value;
                    if (_SelectedSupplier != null)
                    {
                        _SupplierID = _SelectedSupplier.SupplierID;
                    }
                    else
                    {
                        _SupplierID = 0;
                    }
                    RaisePropertyChanged("SupplierID");
                    RaisePropertyChanged("SelectedSupplier");
                    OnSelectedSupplierChanged();

                    RaisePropertyChanged("CanNew");
                }
            }
        }
        private Supplier _SelectedSupplier;
        partial void OnSelectedSupplierChanging(Supplier unit);
        partial void OnSelectedSupplierChanged();


        public static ValidationResult ValidateSelectedSupplier(Supplier value, ValidationContext context)
        {
            if (value == null)
            {
                return new ValidationResult("Vui lòng chọn NCC", new string[] { "SelectedSupplier" });
            }
            else
            {
                if (value.SupplierID == 0)
                {
                    return new ValidationResult("Vui lòng chọn NCC", new string[] { "SelectedSupplier" });
                }
            }
            return ValidationResult.Success;
        }

        public Staff SelectedStaff
        {
            get
            {
                return _SelectedStaff;
            }
            set
            {
                if (_SelectedStaff != value)
                {
                    _SelectedStaff = value;
                    RaisePropertyChanged("SelectedStaff");
                }
            }
        }
        private Staff _SelectedStaff;

        [DataMemberAttribute()]
        public PharmacyEstimationForPO PharmacyEstimationForPO
        {
            get
            {
                return _PharmacyEstimationForPO;
            }
            set
            {
                if (_PharmacyEstimationForPO != value)
                {
                    _PharmacyEstimationForPO = value;
                    if (_PharmacyEstimationForPO != null)
                    {
                        _PharmacyEstimatePoID = _PharmacyEstimationForPO.PharmacyEstimatePoID;
                    }
                    else
                    {
                        _PharmacyEstimatePoID = 0;
                    }
                    RaisePropertyChanged("PharmacyEstimationForPO");

                    RaisePropertyChanged("CanNew");
                }
            }
        }
        private PharmacyEstimationForPO _PharmacyEstimationForPO;


        [DataMemberAttribute()]
        public ObservableCollection<PharmacyPurchaseOrderDetail> PurchaseOrderDetails
        {
            get
            {
                return _PurchaseOrderDetails;
            }
            set
            {
                if (_PurchaseOrderDetails != value)
                {
                    _PurchaseOrderDetails = value;
                    RaisePropertyChanged("PurchaseOrderDetails");

                    RaisePropertyChanged("CanNew");
                }
            }
        }
        private ObservableCollection<PharmacyPurchaseOrderDetail> _PurchaseOrderDetails;
        #endregion

        #region Convert XML

        public string ConvertDetailsListToXml()
        {
            return ConvertDetailsListToXml(_PurchaseOrderDetails);
        }
        public string ConvertDetailsListToXml(IEnumerable<PharmacyPurchaseOrderDetail> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<PurchaseOrderDetails>");
                foreach (PharmacyPurchaseOrderDetail details in items)
                {
                    if (details.DrugID > 0)
                    {
                        int EntityState = (int)details.EntityState;
                        sb.Append("<RecInfo>");
                        sb.AppendFormat("<PharmacyPoDetailID>{0}</PharmacyPoDetailID>", details.PharmacyPoDetailID);
                        sb.AppendFormat("<PharmacyPoID>{0}</PharmacyPoID>", details.PharmacyPoID);
                        sb.AppendFormat("<DrugID>{0}</DrugID>", details.DrugID);
                        sb.AppendFormat("<PoUnitQty>{0}</PoUnitQty>", details.PoUnitQty);
                        sb.AppendFormat("<PoPackageQty>{0}</PoPackageQty>",Math.Round(details.PoPackageQty,4));
                        sb.AppendFormat("<WaitingDeliveryQty>{0}</WaitingDeliveryQty>", details.WaitingDeliveryQty);
                        sb.AppendFormat("<EstimateQty>{0}</EstimateQty>", details.EstimateQty);
                        sb.AppendFormat("<UnitPrice>{0}</UnitPrice>", details.UnitPrice);
                        sb.AppendFormat("<PackagePrice>{0}</PackagePrice>", details.PackagePrice);
                        sb.AppendFormat("<PoNotes>{0}</PoNotes>", details.PoNotes);
                        sb.AppendFormat("<IsUnitPackage>{0}</IsUnitPackage>", details.IsUnitPackage);
                        sb.AppendFormat("<EntityState>{0}</EntityState>", EntityState);
                        sb.AppendFormat("<IsNotVat>{0}</IsNotVat>", details.IsNotVat);
                        sb.AppendFormat("<VAT>{0}</VAT>", details.RefGenericDrugDetail != null ? details.RefGenericDrugDetail.VAT : null);
                        sb.Append("</RecInfo>");
                    }
                }
                sb.Append("</PurchaseOrderDetails>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }

        #endregion
    }

    public partial class PharmacyPurchaseCheckOrder : NotifyChangedBase
    {
        public static PharmacyPurchaseCheckOrder CreatePharmacyPurchaseOrder(Int64 PharmacyPoID, Int64 supplierID)
        {
            PharmacyPurchaseCheckOrder PharmacyPurchaseOrder = new PharmacyPurchaseCheckOrder();
            PharmacyPurchaseOrder.PharmacyPoID = PharmacyPoID;
            PharmacyPurchaseOrder.SupplierID = supplierID;
            return PharmacyPurchaseOrder;
        }

        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 PharmacyPoDetailID
        {
            get
            {
                return _PharmacyPoDetailID;
            }
            set
            {
                if (_PharmacyPoDetailID != value)
                {
                    OnPharmacyPoDetailIDChanging(value);
                    _PharmacyPoDetailID = value;
                    RaisePropertyChanged("PharmacyPoDetailID");
                    OnPharmacyPoDetailIDChanged();
                }
            }
        }
        private Int64 _PharmacyPoDetailID;
        partial void OnPharmacyPoDetailIDChanging(Int64 value);
        partial void OnPharmacyPoDetailIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> PharmacyPoID
        {
            get
            {
                return _PharmacyPoID;
            }
            set
            {
                OnPharmacyPoIDChanging(value);
                _PharmacyPoID = value;
                RaisePropertyChanged("PharmacyPoID");
                OnPharmacyPoIDChanged();
            }
        }
        private Nullable<Int64> _PharmacyPoID;
        partial void OnPharmacyPoIDChanging(Nullable<Int64> value);
        partial void OnPharmacyPoIDChanged();

        [DataMemberAttribute()]
        public String PONumber
        {
            get
            {
                return _PONumber;
            }
            set
            {
                OnPONumberChanging(value);
                _PONumber = value;
                RaisePropertyChanged("PONumber");
                OnPONumberChanged();
            }
        }
        private String _PONumber;
        partial void OnPONumberChanging(String value);
        partial void OnPONumberChanged();

        [DataMemberAttribute()]
        public Int64 GenMedProductID
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
        private Int64 _GenMedProductID;
        partial void OnGenMedProductIDChanging(Int64 value);
        partial void OnGenMedProductIDChanged();

        [DataMemberAttribute()]
        public Int32 PoUnitQty
        {
            get
            {
                return _PoUnitQty;
            }
            set
            {
                if (_PoUnitQty != value)
                {
                    OnPoUnitQtyChanging(value);
                    _PoUnitQty = value;
                    RaisePropertyChanged("PoUnitQty");
                    OnPoUnitQtyChanged();
                }
            }
        }
        private Int32 _PoUnitQty;
        partial void OnPoUnitQtyChanging(Int32 value);
        partial void OnPoUnitQtyChanged();

        [DataMemberAttribute()]
        public Int32 WaitingDeliveryQty
        {
            get
            {
                return _WaitingDeliveryQty;
            }
            set
            {
                OnWaitingDeliveryQtyChanging(value);
                _WaitingDeliveryQty = value;
                RaisePropertyChanged("WaitingDeliveryQty");
                OnWaitingDeliveryQtyChanged();
            }
        }
        private Int32 _WaitingDeliveryQty;
        partial void OnWaitingDeliveryQtyChanging(Int32 value);
        partial void OnWaitingDeliveryQtyChanged();

        [DataMemberAttribute()]
        public Double InQuantity
        {
            get
            {
                return _InQuantity;
            }
            set
            {
                OnInQuantityChanging(value);
                _InQuantity = value;
                RaisePropertyChanged("InQuantity");

                OnInQuantityChanged();
            }
        }
        private Double _InQuantity;
        partial void OnInQuantityChanging(Double value);
        partial void OnInQuantityChanged();

        [DataMemberAttribute()]
        public string SupplierCode
        {
            get
            {
                return _SupplierCode;
            }
            set
            {
                OnSupplierCodeChanging(value);
                _SupplierCode = value;
                RaisePropertyChanged("SupplierCode");
                OnSupplierCodeChanged();
            }
        }
        private string _SupplierCode;
        partial void OnSupplierCodeChanging(string value);
        partial void OnSupplierCodeChanged();

        [DataMemberAttribute()]
        public string SupplierName
        {
            get
            {
                return _SupplierName;
            }
            set
            {
                OnSupplierNameChanging(value);
                _SupplierName = value;
                RaisePropertyChanged("SupplierName");
                OnSupplierNameChanged();
            }
        }
        private string _SupplierName;
        partial void OnSupplierNameChanging(string value);
        partial void OnSupplierNameChanged();

        [DataMemberAttribute()]
        public Int64 SupplierID
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
        private Int64 _SupplierID;
        partial void OnSupplierIDChanging(Int64 value);
        partial void OnSupplierIDChanged();

        [DataMemberAttribute()]
        public bool IsOrderRemaining
        {
            get
            {
                return _IsOrderRemaining;
            }
            set
            {
                _IsOrderRemaining = value;
                RaisePropertyChanged("IsOrderRemaining");
            }
        }
        private bool _IsOrderRemaining;
        #endregion
    }
    public partial class PharmacyPurchaseCheckOrderInward : NotifyChangedBase
    {
        public static PharmacyPurchaseCheckOrderInward CreatePharmacyPurchaseOrder()
        {
            PharmacyPurchaseCheckOrderInward PharmacyPurchaseOrder = new PharmacyPurchaseCheckOrderInward();

            return PharmacyPurchaseOrder;
        }

        #region Primitive Properties

        [DataMemberAttribute()]
        public Double InQuantity
        {
            get
            {
                return _InQuantity;
            }
            set
            {
                OnInQuantityChanging(value);
                _InQuantity = value;
                RaisePropertyChanged("InQuantity");

                OnInQuantityChanged();
            }
        }
        private Double _InQuantity;
        partial void OnInQuantityChanging(Double value);
        partial void OnInQuantityChanged();

        [DataMemberAttribute()]
        public string InvID
        {
            get
            {
                return _InvID;
            }
            set
            {
                OnInvIDChanging(value);
                _InvID = value;
                RaisePropertyChanged("InvID");
                OnInvIDChanged();
            }
        }
        private string _InvID;
        partial void OnInvIDChanging(string value);
        partial void OnInvIDChanged();

        #endregion
    }
}
