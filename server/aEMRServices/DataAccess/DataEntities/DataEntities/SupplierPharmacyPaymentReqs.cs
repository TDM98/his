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
    public partial class SupplierPharmacyPaymentReqs : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new SupplierPharmacyPaymentReqs object.

        /// <param name="PharmacySupplierPaymentReqID">Initial value of the PharmacySupplierPaymentReqID property.</param>
        /// <param name="supplierID">Initial value of the SupplierID property.</param>
        /// <param name="StaffID">Initial value of the StaffID property.</param>
        /// <param name="RequestedDate">Initial value of the RequestedDate property.</param>
        public static SupplierPharmacyPaymentReqs CreateSupplierPharmacyPaymentReqs(Int64 PharmacySupplierPaymentReqID, String SequenceNum, Int64 supplierID, Int64 StaffID, DateTime RequestedDate)
        {
            SupplierPharmacyPaymentReqs SupplierPharmacyPaymentReqs = new SupplierPharmacyPaymentReqs();
            SupplierPharmacyPaymentReqs.PharmacySupplierPaymentReqID = PharmacySupplierPaymentReqID;
            SupplierPharmacyPaymentReqs.SequenceNum = SequenceNum;
            SupplierPharmacyPaymentReqs.SupplierID = supplierID;
            SupplierPharmacyPaymentReqs.StaffID = StaffID;
            SupplierPharmacyPaymentReqs.RequestedDate = RequestedDate;
            return SupplierPharmacyPaymentReqs;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 PharmacySupplierPaymentReqID
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
                    RaisePropertyChanged("CanSave");
                    RaisePropertyChanged("CanDelete");
                    RaisePropertyChanged("CanPrint");
                    OnPharmacySupplierPaymentReqIDChanged();
                }
            }
        }
        private Int64 _PharmacySupplierPaymentReqID;
        partial void OnPharmacySupplierPaymentReqIDChanging(Int64 value);
        partial void OnPharmacySupplierPaymentReqIDChanged();

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
        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public Supplier SelectedSupplier
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
        private Supplier _SelectedSupplier;
        partial void OnSelectedSupplierChanging(Supplier unit);
        partial void OnSelectedSupplierChanged();

        [DataMemberAttribute()]
        public ObservableCollection<InwardDrugInvoice> InwardDrugInvoices
        {
            get
            {
                return _InwardDrugInvoices;
            }
            set
            {
                _InwardDrugInvoices = value;
                RaisePropertyChanged("InwardDrugInvoices");
            }
        }
        private ObservableCollection<InwardDrugInvoice> _InwardDrugInvoices;

        #endregion

        public bool CanApproved
        {
            get { return (V_PaymentReqStatus == (long)AllLookupValues.V_PaymentReqStatus.WAITING_APPROVED); }
        }

        public bool CanDelete
        {
            get { return (PharmacySupplierPaymentReqID > 0 && V_PaymentReqStatus == (long)AllLookupValues.V_PaymentReqStatus.NEW); }
        }

        public bool CanSave
        {
            get { return (PharmacySupplierPaymentReqID == 0 || V_PaymentReqStatus==(long)AllLookupValues.V_PaymentReqStatus.NEW); }
        }
        public bool CanPrint
        {
            get { return PharmacySupplierPaymentReqID > 0 && (V_PaymentReqStatus == (long)AllLookupValues.V_PaymentReqStatus.WAITING_APPROVED || V_PaymentReqStatus == (long)AllLookupValues.V_PaymentReqStatus.APPROVED); }
        }

        #region Convert XML

        public string ConvertDetailsListToXml()
        {
            return ConvertDetailsListToXml(_InwardDrugInvoices);
        }
        public string ConvertDetailsListToXml(IEnumerable<InwardDrugInvoice> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<InwardDrugInvoices>");
                foreach (InwardDrugInvoice item in items)
                {
                    sb.Append("<RecInfo>");
                    sb.AppendFormat("<inviID>{0}</inviID>", item.inviID);
                    sb.AppendFormat("<PharmacySupplierPaymentNotes>{0}</PharmacySupplierPaymentNotes>", item.PharmacySupplierPaymentNotes);
                    sb.AppendFormat("<PharmacySupplierPaymentReqID>{0}</PharmacySupplierPaymentReqID>", item.PharmacySupplierPaymentReqID);
                    sb.Append("</RecInfo>");
                }
                sb.Append("</InwardDrugInvoices>");
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
