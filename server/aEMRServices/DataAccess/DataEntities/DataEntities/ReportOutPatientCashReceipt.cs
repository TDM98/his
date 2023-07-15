using System;
using System.Runtime.Serialization;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public partial class ReportOutPatientCashReceipt : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new ReportOutPatientCashReceipt object.

        /// <param name="ReportOutPatientCashReceiptID">Initial value of the ReportOutPatientCashReceiptID property.</param>
        /// <param name="ReportOutPatientCashReceiptName">Initial value of the ReportOutPatientCashReceiptName property.</param>
        public static ReportOutPatientCashReceipt CreateReportOutPatientCashReceipt(Int64 ReportOutPatientCashReceiptID)
        {
            ReportOutPatientCashReceipt ReportOutPatientCashReceipt = new ReportOutPatientCashReceipt();
            ReportOutPatientCashReceipt.ReportOutPatientCashReceiptID = ReportOutPatientCashReceiptID;
            return ReportOutPatientCashReceipt;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 ReportOutPatientCashReceiptID
        {
            get
            {
                return _ReportOutPatientCashReceiptID;
            }
            set
            {
                if (_ReportOutPatientCashReceiptID != value)
                {
                    OnReportOutPatientCashReceiptIDChanging(value);
                    _ReportOutPatientCashReceiptID = value;
                    RaisePropertyChanged("ReportOutPatientCashReceiptID");
                    OnReportOutPatientCashReceiptIDChanged();
                }
            }
        }
        private Int64 _ReportOutPatientCashReceiptID;
        partial void OnReportOutPatientCashReceiptIDChanging(Int64 value);
        partial void OnReportOutPatientCashReceiptIDChanged();


        [DataMemberAttribute()]
        public Nullable<Int64> PaymentID
        {
            get
            {
                return _PaymentID;
            }
            set
            {
                OnPaymentIDChanging(value);
                _PaymentID = value;
                RaisePropertyChanged("PaymentID");
                OnPaymentIDChanged();
            }
        }
        private Nullable<Int64> _PaymentID;
        partial void OnPaymentIDChanging(Nullable<Int64> value);
        partial void OnPaymentIDChanged();


        [DataMemberAttribute()]
        public Nullable<Int64> ItemID
        {
            get { return _ItemID; }
            set
            {
                if (_ItemID != value)
                {
                    OnItemIDChanging(value);
                    _ItemID = value;
                    RaisePropertyChanged("ItemID");
                    OnItemIDChanged();
                }
            }
        }
        private Nullable<Int64> _ItemID;
        partial void OnItemIDChanging(Nullable<Int64> value);
        partial void OnItemIDChanged();


        [DataMemberAttribute()]
        public Nullable<Int64> PatientID
        {
            get { return _PatientID; }
            set
            {
                if (_PatientID != value)
                {
                    OnPatientIDChanging(value);
                    ValidateProperty("PatientID", value);
                    _PatientID = value;
                    RaisePropertyChanged("PatientID");
                    OnPatientIDChanged();
                }
            }
        }
        private Nullable<Int64> _PatientID;
        partial void OnPatientIDChanging(Nullable<Int64> value);
        partial void OnPatientIDChanged();

        [DataMemberAttribute()]
        public long ServiceItemType
        {
            get
            {
                return _ServiceItemType;
            }
            set
            {
                _ServiceItemType = value;
                RaisePropertyChanged("ServiceItemType");
            }
        }
        private long _ServiceItemType;


        [DataMemberAttribute()]
        public String ServiceName
        {
            get
            {
                return _ServiceName;
            }
            set
            {
                OnServiceNameChanging(value);
                _ServiceName = value;
                RaisePropertyChanged("ServiceName");
                OnServiceNameChanged();
            }
        }
        private String _ServiceName;
        partial void OnServiceNameChanging(String value);
        partial void OnServiceNameChanged();

        [DataMemberAttribute()]
        public String FullName
        {
            get
            {
                return _FullName;
            }
            set
            {
                OnFullNameChanging(value);
                ValidateProperty("FullName", value);
                _FullName = value;
                RaisePropertyChanged("FullName");
                OnFullNameChanged();
            }
        }
        private String _FullName;
        partial void OnFullNameChanging(String value);
        partial void OnFullNameChanged();


        [DataMemberAttribute()]
        public String StaffName
        {
            get
            {
                return _StaffName;
            }
            set
            {
                OnStaffNameChanging(value);
                _StaffName = value;
                RaisePropertyChanged("StaffName");
                OnStaffNameChanged();
            }
        }
        private String _StaffName;
        partial void OnStaffNameChanging(String value);
        partial void OnStaffNameChanged();


        [DataMemberAttribute()]
        public Nullable<long> PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                OnPtRegistrationIDChanging(value);
                _PtRegistrationID = value;
                RaisePropertyChanged("PtRegistrationID");
                OnPtRegistrationIDChanged();
            }
        }
        private Nullable<long> _PtRegistrationID;
        partial void OnPtRegistrationIDChanging(Nullable<long> value);
        partial void OnPtRegistrationIDChanged();

        [DataMemberAttribute()]
        public decimal Amount
        {
            get
            {
                return _Amount;
            }
            set
            {
                OnAmountChanging(value);
                _Amount = value;
                RaisePropertyChanged("Amount");
                OnAmountChanged();
            }
        }
        private decimal _Amount;
        partial void OnAmountChanging(decimal value);
        partial void OnAmountChanged();


        [DataMemberAttribute()]
        public decimal PatientAmount
        {
            get
            {
                return _PatientAmount;
            }
            set
            {
                OnPatientAmountChanging(value);
                _PatientAmount = value;
                RaisePropertyChanged("PatientAmount");
                OnPatientAmountChanged();
            }
        }
        private decimal _PatientAmount;
        partial void OnPatientAmountChanging(decimal value);
        partial void OnPatientAmountChanged();


        [DataMemberAttribute()]
        public Nullable<long> DeptLocID
        {
            get
            {
                return _deptLocID;
            }
            set
            {
                ValidateProperty("DeptLocID", value);
                _deptLocID = value;
                RaisePropertyChanged("DeptLocID");
            }
        }
        private Nullable<long> _deptLocID = 0;

        private int _serviceSeqNum = 0;
        [DataMemberAttribute()]
        public int ServiceSeqNum
        {
            get
            {
                return _serviceSeqNum;
            }
            set
            {
                _serviceSeqNum = value;
                RaisePropertyChanged("ServiceSeqNum");
            }
        }
        private byte _serviceSeqNumType = 0;
        [DataMemberAttribute()]
        public byte ServiceSeqNumType
        {
            get
            {
                return _serviceSeqNumType;
            }
            set
            {
                _serviceSeqNumType = value;
                RaisePropertyChanged("ServiceSeqNumType");
            }
        }

        private string _serviceSeqNumString;
        [DataMemberAttribute]
        public string ServiceSeqNumString
        {
            get
            {
                return _serviceSeqNumString;
            }
            set
            {
                _serviceSeqNumString = value;
                RaisePropertyChanged("ServiceSeqNumString");
            }
        }

        private long? _OutPtCashAdvanceID;
        [DataMemberAttribute()]
        public long? OutPtCashAdvanceID
        {
            get
            {
                return _OutPtCashAdvanceID;
            }
            set
            {
                _OutPtCashAdvanceID = value;
                RaisePropertyChanged("OutPtCashAdvanceID");
            }
        }
        #endregion

        private decimal _DiscountAmount = 0;

        public decimal DiscountAmount
        {
            get => _DiscountAmount; set
            {
                _DiscountAmount = value;
                RaisePropertyChanged("DiscountAmount");
            }
        }

        private decimal _HIAmount = 0;

        public decimal HIAmount
        {
            get
            {
                return _HIAmount;
            }
            set
            {
                _HIAmount = value;
                RaisePropertyChanged("HIAmount");
            }
        }
    }

    public partial class ReportOutPatientCashReceipt_Payments : NotifyChangedBase
    {
        #region member
        private ReportOutPatientCashReceipt _CurReportOutPatientCashReceipt;
        [DataMemberAttribute()]
        public ReportOutPatientCashReceipt CurReportOutPatientCashReceipt
        {
            get
            {
                return _CurReportOutPatientCashReceipt;
            }
            set
            {
                _CurReportOutPatientCashReceipt = value;
                RaisePropertyChanged("CurReportOutPatientCashReceipt");
            }
        }
        private PatientTransactionPayment _CurPatientTransactionPayment;
        [DataMemberAttribute()]
        public PatientTransactionPayment CurPatientTransactionPayment
        {
            get
            {
                return _CurPatientTransactionPayment;
            }
            set
            {
                _CurPatientTransactionPayment = value;
                RaisePropertyChanged("CurPatientTransactionPayment");
            }
        }
        #endregion
    }
}


