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
    public partial class DrugDeptPurchaseOrder : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new DrugDeptPurchaseOrder object.

        /// <param name="DrugDeptPoID">Initial value of the DrugDeptPoID property.</param>
        /// <param name="supplierID">Initial value of the SupplierID property.</param>
        public static DrugDeptPurchaseOrder CreateDrugDeptPurchaseOrder(Int64 DrugDeptPoID, Int64 supplierID)
        {
            DrugDeptPurchaseOrder DrugDeptPurchaseOrder = new DrugDeptPurchaseOrder();
            DrugDeptPurchaseOrder.DrugDeptPoID = DrugDeptPoID;
            DrugDeptPurchaseOrder.SupplierID = supplierID;
            return DrugDeptPurchaseOrder;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 DrugDeptPoID
        {
            get
            {
                return _DrugDeptPoID;
            }
            set
            {
                if (_DrugDeptPoID != value)
                {
                    OnDrugDeptPoIDChanging(value);
                    _DrugDeptPoID = value;
                    RaisePropertyChanged("CanPrint");
                    RaisePropertyChanged("CanDelete");
                    RaisePropertyChanged("DrugDeptPoID");
                    OnDrugDeptPoIDChanged();
                }
            }
        }
        private Int64 _DrugDeptPoID;
        partial void OnDrugDeptPoIDChanging(Int64 value);
        partial void OnDrugDeptPoIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> DrugDeptEstimatePoID
        {
            get
            {
                return _DrugDeptEstimatePoID;
            }
            set
            {
                OnDrugDeptEstimatePoIDChanging(value);
                _DrugDeptEstimatePoID = value;
                RaisePropertyChanged("DrugDeptEstimatePoID");
                OnDrugDeptEstimatePoIDChanged();
            }
        }
        private Nullable<Int64> _DrugDeptEstimatePoID;
        partial void OnDrugDeptEstimatePoIDChanging(Nullable<Int64> value);
        partial void OnDrugDeptEstimatePoIDChanged();

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
            }
        }
        private int _DeliveryMoneyDayNo;

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
            }
        }
        private string _PoNotes;
        partial void OnPoNotesChanging(string value);
        partial void OnPoNotesChanged();

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
                    OnDrugDeptPoIDChanging(value);
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

        [DataMemberAttribute()]
        public Nullable<Int64> V_MedProductType
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
        private Nullable<Int64> _V_MedProductType;
        partial void OnV_MedProductTypeChanging(Nullable<Int64> value);
        partial void OnV_MedProductTypeChanged();

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
        #endregion

        public bool CanWaiting
        {
            get { return (V_PurchaseOrderStatus == (long)AllLookupValues.V_PurchaseOrderStatus.ORDERED || V_PurchaseOrderStatus == (long)AllLookupValues.V_PurchaseOrderStatus.PART_DELIVERY); }
        }

        public bool CanSave
        {
            //get { return V_PurchaseOrderStatus == 0 || (V_PurchaseOrderStatus == (long)AllLookupValues.V_PurchaseOrderStatus.NEW); }
            get { return V_PurchaseOrderStatus == 0 || (V_PurchaseOrderStatus == (long)AllLookupValues.V_PurchaseOrderStatus.NEW) || V_PurchaseOrderStatus == (long)AllLookupValues.V_PurchaseOrderStatus.ORDERED || V_PurchaseOrderStatus == (long)AllLookupValues.V_PurchaseOrderStatus.PART_DELIVERY; }
        }
        public bool CanDelete
        {
            get { return DrugDeptPoID > 0 && (V_PurchaseOrderStatus == (long)AllLookupValues.V_PurchaseOrderStatus.NEW); }
        }
        public bool CanPrint
        {
            get { return DrugDeptPoID > 0; }
        }

        #region Navigation Properties
        [CustomValidation(typeof(DrugDeptPurchaseOrder), "ValidateSelectedSupplier")]
        [DataMemberAttribute()]
        public DrugDeptSupplier SelectedSupplier
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
                }
            }
        }
        private DrugDeptSupplier _SelectedSupplier;
        partial void OnSelectedSupplierChanging(DrugDeptSupplier unit);
        partial void OnSelectedSupplierChanged();


        public static ValidationResult ValidateSelectedSupplier(DrugDeptSupplier value, ValidationContext context)
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
        public DrugDeptEstimationForPO DrugDeptEstimationForPO
        {
            get
            {
                return _DrugDeptEstimationForPO;
            }
            set
            {
                if (_DrugDeptEstimationForPO != value)
                {
                    _DrugDeptEstimationForPO = value;
                    if (_DrugDeptEstimationForPO != null)
                    {
                        _DrugDeptEstimatePoID = _DrugDeptEstimationForPO.DrugDeptEstimatePoID;
                    }
                    else
                    {
                        _DrugDeptEstimatePoID = 0;
                    }
                    RaisePropertyChanged("DrugDeptEstimationForPO");
                }
            }
        }
        private DrugDeptEstimationForPO _DrugDeptEstimationForPO;


        [DataMemberAttribute()]
        private ObservableCollection<DrugDeptPurchaseOrderDetail> _PurchaseOrderDetails;
        public ObservableCollection<DrugDeptPurchaseOrderDetail> PurchaseOrderDetails
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
                }
            }
        }

        [DataMemberAttribute()]
        private ObservableCollection<DrugDeptPurchaseOrderDetail> _PurchaseOrderDetailDeleted;
        public ObservableCollection<DrugDeptPurchaseOrderDetail> PurchaseOrderDetailDeleted
        {
            get
            {
                return _PurchaseOrderDetailDeleted;
            }
            set
            {
                if (_PurchaseOrderDetailDeleted != value)
                {
                    _PurchaseOrderDetailDeleted = value;
                    RaisePropertyChanged("PurchaseOrderDetailDeleted");
                }
            }
        }

        private long? _BidID;
        [DataMemberAttribute]
        public long? BidID
        {
            get
            {
                return _BidID;
            }
            set
            {
                _BidID = value;
                RaisePropertyChanged("BidID");
            }
        }
        #endregion

        #region Convert XML
        public string ConvertDetailsListToXml()
        {
            return ConvertDetailsListToXml(_PurchaseOrderDetails);
        }
        public string ConvertDetailsListToXml(IEnumerable<DrugDeptPurchaseOrderDetail> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<PurchaseOrderDetails>");
                foreach (DrugDeptPurchaseOrderDetail details in items)
                {
                    if (details.GenMedProductID > 0)
                    {
                        int EntityState = (int)details.EntityState;
                        sb.Append("<RecInfo>");
                        sb.AppendFormat("<DrugDeptPoDetailID>{0}</DrugDeptPoDetailID>", details.DrugDeptPoDetailID);
                        sb.AppendFormat("<DrugDeptPoID>{0}</DrugDeptPoID>", details.DrugDeptPoID);
                        sb.AppendFormat("<GenMedProductID>{0}</GenMedProductID>", details.GenMedProductID);
                        sb.AppendFormat("<PoUnitQty>{0}</PoUnitQty>", details.PoUnitQty);
                        sb.AppendFormat("<PoPackageQty>{0}</PoPackageQty>", details.PoPackageQty);
                        sb.AppendFormat("<InQuantity>{0}</InQuantity>", details.InQuantity);
                        sb.AppendFormat("<WaitingDeliveryQty>{0}</WaitingDeliveryQty>", details.WaitingDeliveryQty);
                        sb.AppendFormat("<EstimateQty>{0}</EstimateQty>", details.EstimateQty);
                        sb.AppendFormat("<UnitPrice>{0}</UnitPrice>", details.UnitPrice);
                        sb.AppendFormat("<PackagePrice>{0}</PackagePrice>", details.PackagePrice);
                        sb.AppendFormat("<PoNotes>{0}</PoNotes>", details.PoNotes);
                        sb.AppendFormat("<EntityState>{0}</EntityState>", EntityState);
                        sb.AppendFormat("<DrugDeptEstPoDetailID>{0}</DrugDeptEstPoDetailID>", details.DrugDeptEstPoDetailID);
                        sb.AppendFormat("<VAT>{0}</VAT>", details.VAT);
                        sb.AppendFormat("<IsNotVat>{0}</IsNotVat>", details.IsNotVat);
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

        public string ConvertDetailDeleteListToXml()
        {
            return ConvertDetailDeleteListToXml(_PurchaseOrderDetailDeleted);
        }
        public string ConvertDetailDeleteListToXml(IEnumerable<DrugDeptPurchaseOrderDetail> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<PurchaseOrderDetails>");
                foreach (DrugDeptPurchaseOrderDetail details in items)
                {
                    if (details.GenMedProductID > 0)
                    {
                        int EntityState = (int)details.EntityState;
                        sb.Append("<RecInfo>");
                        sb.AppendFormat("<DrugDeptPoDetailID>{0}</DrugDeptPoDetailID>", details.DrugDeptPoDetailID);
                        sb.AppendFormat("<DrugDeptPoID>{0}</DrugDeptPoID>", details.DrugDeptPoID);
                        sb.AppendFormat("<GenMedProductID>{0}</GenMedProductID>", details.GenMedProductID);
                        sb.AppendFormat("<PoUnitQty>{0}</PoUnitQty>", details.PoUnitQty);
                        sb.AppendFormat("<PoPackageQty>{0}</PoPackageQty>", details.PoPackageQty);
                        sb.AppendFormat("<InQuantity>{0}</InQuantity>", details.InQuantity);
                        sb.AppendFormat("<WaitingDeliveryQty>{0}</WaitingDeliveryQty>", details.WaitingDeliveryQty);
                        sb.AppendFormat("<EstimateQty>{0}</EstimateQty>", details.EstimateQty);
                        sb.AppendFormat("<UnitPrice>{0}</UnitPrice>", details.UnitPrice);
                        sb.AppendFormat("<PackagePrice>{0}</PackagePrice>", details.PackagePrice);
                        sb.AppendFormat("<PoNotes>{0}</PoNotes>", details.PoNotes);
                        sb.AppendFormat("<EntityState>{0}</EntityState>", EntityState);
                        sb.AppendFormat("<DrugDeptEstPoDetailID>{0}</DrugDeptEstPoDetailID>", details.DrugDeptEstPoDetailID);
                        sb.AppendFormat("<VAT>{0}</VAT>", details.VAT);
                        sb.AppendFormat("<IsNotVat>{0}</IsNotVat>", details.IsNotVat);
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

    public partial class DrugDeptPurchaseCheckOrder : NotifyChangedBase
    {
        public static DrugDeptPurchaseCheckOrder CreateDrugDeptPurchaseOrder(Int64 DrugDeptPoID, Int64 supplierID)
        {
            DrugDeptPurchaseCheckOrder DrugDeptPurchaseOrder = new DrugDeptPurchaseCheckOrder();
            DrugDeptPurchaseOrder.DrugDeptPoID = DrugDeptPoID;
            DrugDeptPurchaseOrder.SupplierID = supplierID;
            return DrugDeptPurchaseOrder;
        }

        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 DrugDeptPoDetailID
        {
            get
            {
                return _DrugDeptPoDetailID;
            }
            set
            {
                if (_DrugDeptPoDetailID != value)
                {
                    OnDrugDeptPoDetailIDChanging(value);
                    _DrugDeptPoDetailID = value;
                    RaisePropertyChanged("DrugDeptPoDetailID");
                    OnDrugDeptPoDetailIDChanged();
                }
            }
        }
        private Int64 _DrugDeptPoDetailID;
        partial void OnDrugDeptPoDetailIDChanging(Int64 value);
        partial void OnDrugDeptPoDetailIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> DrugDeptPoID
        {
            get
            {
                return _DrugDeptPoID;
            }
            set
            {
                OnDrugDeptPoIDChanging(value);
                _DrugDeptPoID = value;
                RaisePropertyChanged("DrugDeptPoID");
                OnDrugDeptPoIDChanged();
            }
        }
        private Nullable<Int64> _DrugDeptPoID;
        partial void OnDrugDeptPoIDChanging(Nullable<Int64> value);
        partial void OnDrugDeptPoIDChanged();

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
    public partial class DrugDeptPurchaseCheckOrderInward : NotifyChangedBase
    {
        public static DrugDeptPurchaseCheckOrderInward CreateDrugDeptPurchaseOrder()
        {
            DrugDeptPurchaseCheckOrderInward DrugDeptPurchaseOrder = new DrugDeptPurchaseCheckOrderInward();
         
            return DrugDeptPurchaseOrder;
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
