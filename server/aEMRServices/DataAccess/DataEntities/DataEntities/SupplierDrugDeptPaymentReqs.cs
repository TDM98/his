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
    public partial class SupplierDrugDeptPaymentReqs : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new SupplierDrugDeptPaymentReqs object.

        /// <param name="DrugDeptSupplierPaymentReqID">Initial value of the DrugDeptSupplierPaymentReqID property.</param>
        /// <param name="supplierID">Initial value of the SupplierID property.</param>
        /// <param name="StaffID">Initial value of the StaffID property.</param>
        /// <param name="RequestedDate">Initial value of the RequestedDate property.</param>
        public static SupplierDrugDeptPaymentReqs CreateSupplierDrugDeptPaymentReqs(Int64 DrugDeptSupplierPaymentReqID, String SequenceNum, Int64 supplierID, Int64 StaffID, DateTime RequestedDate)
        {
            SupplierDrugDeptPaymentReqs SupplierDrugDeptPaymentReqs = new SupplierDrugDeptPaymentReqs();
            SupplierDrugDeptPaymentReqs.DrugDeptSupplierPaymentReqID = DrugDeptSupplierPaymentReqID;
            SupplierDrugDeptPaymentReqs.SequenceNum = SequenceNum;
            SupplierDrugDeptPaymentReqs.SupplierID = supplierID;
            SupplierDrugDeptPaymentReqs.StaffID = StaffID;
            SupplierDrugDeptPaymentReqs.RequestedDate = RequestedDate;
            return SupplierDrugDeptPaymentReqs;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 DrugDeptSupplierPaymentReqID
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
                    RaisePropertyChanged("CanSave");
                    RaisePropertyChanged("CanDelete");
                    RaisePropertyChanged("CanPrint");
                    OnDrugDeptSupplierPaymentReqIDChanged();
                }
            }
        }
        private Int64 _DrugDeptSupplierPaymentReqID;
        partial void OnDrugDeptSupplierPaymentReqIDChanging(Int64 value);
        partial void OnDrugDeptSupplierPaymentReqIDChanged();

        [DataMemberAttribute()]
        public String SequenceNum
        {
            get
            {
                return _SequenceNum;
            }
            set
            {
                OnSequenceNumChanging(value);
                ValidateProperty("SequenceNum", value);
                _SequenceNum = value;
                RaisePropertyChanged("SequenceNum");
                OnSequenceNumChanged();
            }
        }
        private String _SequenceNum;
        partial void OnSequenceNumChanging(String value);
        partial void OnSequenceNumChanged();

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
        public DateTime RequestedDate
        {
            get
            {
                return _RequestedDate;
            }
            set
            {
                OnRequestedDateChanging(value);
                _RequestedDate = value;
                RaisePropertyChanged("RequestedDate");
                OnRequestedDateChanged();
            }
        }
        private DateTime _RequestedDate;
        partial void OnRequestedDateChanging(DateTime value);
        partial void OnRequestedDateChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> ApprovedDate
        {
            get
            {
                return _ApprovedDate;
            }
            set
            {
                OnApprovedDateChanging(value);
                _ApprovedDate = value;
                RaisePropertyChanged("ApprovedDate");
                OnApprovedDateChanged();
            }
        }
        private Nullable<DateTime> _ApprovedDate;
        partial void OnApprovedDateChanging(Nullable<DateTime> value);
        partial void OnApprovedDateChanged();

        [DataMemberAttribute()]
        public Int64? ApprovedStaffID
        {
            get
            {
                return _ApprovedStaffID;
            }
            set
            {
                OnApprovedStaffIDChanging(value);
                _ApprovedStaffID = value;
                RaisePropertyChanged("ApprovedStaffID");
                OnApprovedStaffIDChanged();
            }
        }
        private Int64? _ApprovedStaffID;
        partial void OnApprovedStaffIDChanging(Int64? value);
        partial void OnApprovedStaffIDChanged();

        [DataMemberAttribute()]
        public DateTime? SupplierInvDateFrom
        {
            get
            {
                return _SupplierInvDateFrom;
            }
            set
            {
                OnSupplierInvDateFromChanging(value);
                _SupplierInvDateFrom = value;
                RaisePropertyChanged("SupplierInvDateFrom");
                OnSupplierInvDateFromChanged();
            }
        }
        private DateTime? _SupplierInvDateFrom;
        partial void OnSupplierInvDateFromChanging(DateTime? value);
        partial void OnSupplierInvDateFromChanged();

        [DataMemberAttribute()]
        public DateTime? SupplierInvDateTo
        {
            get
            {
                return _SupplierInvDateTo;
            }
            set
            {
                OnSupplierInvDateToChanging(value);
                _SupplierInvDateTo = value;
                RaisePropertyChanged("SupplierInvDateTo");
                OnSupplierInvDateToChanged();
            }
        }
        private DateTime? _SupplierInvDateTo;
        partial void OnSupplierInvDateToChanging(DateTime? value);
        partial void OnSupplierInvDateToChanged();

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
        public String SupplierAccountNum
        {
            get
            {
                return _SupplierAccountNum;
            }
            set
            {
                OnSupplierAccountNumChanging(value);
                _SupplierAccountNum = value;
                RaisePropertyChanged("SupplierAccountNum");
                OnSupplierAccountNumChanged();
            }
        }
        private String _SupplierAccountNum;
        partial void OnSupplierAccountNumChanging(String value);
        partial void OnSupplierAccountNumChanged();

        [DataMemberAttribute()]
        public String SupplierBank
        {
            get
            {
                return _SupplierBank;
            }
            set
            {
                OnSupplierBankChanging(value);
                _SupplierBank = value;
                RaisePropertyChanged("SupplierBank");
                OnSupplierBankChanged();
            }
        }
        private String _SupplierBank;
        partial void OnSupplierBankChanging(String value);
        partial void OnSupplierBankChanged();

        [DataMemberAttribute()]
        public Int64 V_PaymentReqStatus
        {
            get
            {
                return _V_PaymentReqStatus;
            }
            set
            {
                OnV_PaymentReqStatusChanging(value);
                _V_PaymentReqStatus = value;
                RaisePropertyChanged("V_PaymentReqStatus");
                RaisePropertyChanged("CanSave");
                RaisePropertyChanged("CanDelete");
                RaisePropertyChanged("CanPrint");
                RaisePropertyChanged("CanApproved");
                OnV_PaymentReqStatusChanged();
            }
        }
        private Int64 _V_PaymentReqStatus;
        partial void OnV_PaymentReqStatusChanging(Int64 value);
        partial void OnV_PaymentReqStatusChanged();

        [DataMemberAttribute()]
        public String V_PaymentReqStatusName
        {
            get
            {
                return _V_PaymentReqStatusName;
            }
            set
            {
                _V_PaymentReqStatusName = value;
                RaisePropertyChanged("V_PaymentReqStatusName");
            }
        }
        private String _V_PaymentReqStatusName;

        [DataMemberAttribute()]
        public String StaffName
        {
            get
            {
                return _StaffName;
            }
            set
            {
                _StaffName = value;
                RaisePropertyChanged("StaffName");
            }
        }
        private String _StaffName;

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
        public DrugDeptSupplier SelectedSupplier
        {
            get
            {
                return _SelectedSupplier;
            }
            set
            {
                OnSelectedSupplierChanging(value);
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
        private DrugDeptSupplier _SelectedSupplier;
        partial void OnSelectedSupplierChanging(DrugDeptSupplier unit);
        partial void OnSelectedSupplierChanged();

        [DataMemberAttribute()]
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
        private ObservableCollection<InwardDrugMedDeptInvoice> _InwardDrugMedDeptInvoices;

        #endregion

        public bool CanApproved
        {
            get { return (V_PaymentReqStatus == (long)AllLookupValues.V_PaymentReqStatus.WAITING_APPROVED); }
        }

        public bool CanDelete
        {
            //KMx: Trước đây nếu trạng thái chờ duyệt thì không cho sửa phiếu. Bây giờ sửa lại, nếu chờ duyệt thì vẫn cho sửa (22/04/2015 10:57).
            //get { return (DrugDeptSupplierPaymentReqID > 0 && V_PaymentReqStatus == (long)AllLookupValues.V_PaymentReqStatus.NEW); }
            get { return (DrugDeptSupplierPaymentReqID > 0 && V_PaymentReqStatus == (long)AllLookupValues.V_PaymentReqStatus.NEW || V_PaymentReqStatus == (long)AllLookupValues.V_PaymentReqStatus.WAITING_APPROVED); }
        }

        public bool CanSave
        {
            //KMx: Trước đây nếu trạng thái chờ duyệt thì không cho sửa phiếu. Bây giờ sửa lại, nếu chờ duyệt thì vẫn cho sửa (22/04/2015 10:57).
            //get { return (DrugDeptSupplierPaymentReqID == 0 || V_PaymentReqStatus==(long)AllLookupValues.V_PaymentReqStatus.NEW); }
            get { return (DrugDeptSupplierPaymentReqID == 0 || V_PaymentReqStatus == (long)AllLookupValues.V_PaymentReqStatus.NEW || V_PaymentReqStatus == (long)AllLookupValues.V_PaymentReqStatus.WAITING_APPROVED); }
        }
        public bool CanPrint
        {
            get { return DrugDeptSupplierPaymentReqID > 0 && (V_PaymentReqStatus == (long)AllLookupValues.V_PaymentReqStatus.WAITING_APPROVED || V_PaymentReqStatus == (long)AllLookupValues.V_PaymentReqStatus.APPROVED); }
        }

        #region Convert XML

        public string ConvertDetailsListToXml()
        {
            return ConvertDetailsListToXml(_InwardDrugMedDeptInvoices);
        }
        public string ConvertDetailsListToXml(IEnumerable<InwardDrugMedDeptInvoice> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<InwardDrugMedDeptInvoices>");
                foreach (InwardDrugMedDeptInvoice item in items)
                {
                    sb.Append("<RecInfo>");
                    sb.AppendFormat("<TypeInvoice>{0}</TypeInvoice>", item.TypeInvoice);
                    sb.AppendFormat("<inviID>{0}</inviID>", item.inviID);
                    sb.AppendFormat("<DrugDeptSupplierPaymentNotes>{0}</DrugDeptSupplierPaymentNotes>", item.DrugDeptSupplierPaymentNotes);
                    sb.AppendFormat("<DrugDeptSupplierPaymentReqID>{0}</DrugDeptSupplierPaymentReqID>", item.DrugDeptSupplierPaymentReqID);
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

    }



}
