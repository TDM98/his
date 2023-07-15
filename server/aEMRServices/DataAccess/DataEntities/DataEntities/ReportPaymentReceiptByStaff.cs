using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    [DataContract]
    public partial class ReportPaymentReceiptByStaff : NotifyChangedBase
    {
        public ReportPaymentReceiptByStaff()
        {

        }
        #region Factory Method


        /// Create a new ReportPaymentReceiptByStaff object.

        /// <param name="ptPmtID">Initial value of the PtPmtID property.</param>
        /// <param name="payAmount">Initial value of the PayAmount property.</param>
        public static ReportPaymentReceiptByStaff CreateReportPaymentReceiptByStaff(long repPaymentRecvID)
        {
            ReportPaymentReceiptByStaff ReportPaymentReceiptByStaff = new ReportPaymentReceiptByStaff();
            ReportPaymentReceiptByStaff.RepPaymentRecvID = repPaymentRecvID;
            return ReportPaymentReceiptByStaff;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long RepPaymentRecvID
        {
            get
            {
                return _RepPaymentRecvID;
            }
            set
            {
                OnRepPaymentRecvIDChanging(value);
                _RepPaymentRecvID = value;
                RaisePropertyChanged("RepPaymentRecvID");
                OnRepPaymentRecvIDChanged();
            }
        }
        private long _RepPaymentRecvID;
        partial void OnRepPaymentRecvIDChanging(long value);
        partial void OnRepPaymentRecvIDChanged();


        [DataMemberAttribute()]
        public Nullable<DateTime> ReportDateTime
        {
            get
            {
                return _ReportDateTime;
            }
            set
            {
                OnReportDateTimeChanging(value);
                _ReportDateTime = value;
                RaisePropertyChanged("ReportDateTime");
                OnReportDateTimeChanged();
            }
        }
        private Nullable<DateTime> _ReportDateTime;
        partial void OnReportDateTimeChanging(Nullable<DateTime> value);
        partial void OnReportDateTimeChanged();

        [DataMemberAttribute()]
        public long StaffID
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
        private long _StaffID;
        partial void OnStaffIDChanging(long value);
        partial void OnStaffIDChanged();

        [DataMemberAttribute()]
        public long ApprovedStaffID
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
        private long _ApprovedStaffID;
        partial void OnApprovedStaffIDChanging(long value);
        partial void OnApprovedStaffIDChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> RepFromDate
        {
            get
            {
                return _RepFromDate;
            }
            set
            {
                OnRepFromDateChanging(value);
                _RepFromDate = value;
                RaisePropertyChanged("RepFromDate");
                OnRepFromDateChanged();
            }
        }
        private Nullable<DateTime> _RepFromDate;
        partial void OnRepFromDateChanging(Nullable<DateTime> value);
        partial void OnRepFromDateChanged();


        [DataMemberAttribute()]
        public Nullable<DateTime> RepToDate
        {
            get
            {
                return _RepToDate;
            }
            set
            {
                OnRepToDateChanging(value);
                _RepToDate = value;
                RaisePropertyChanged("RepToDate");
                OnRepToDateChanged();
            }
        }
        private Nullable<DateTime> _RepToDate;
        partial void OnRepToDateChanging(Nullable<DateTime> value);
        partial void OnRepToDateChanged();



        [Required(ErrorMessage = "Hãy nhập vào Tiêu Đề")]
        [DataMemberAttribute()]
        public string RepTittle
        {
            get
            {
                return _RepTittle;
            }
            set
            {
                OnRepTittleChanging(value);
                _RepTittle = value;
                RaisePropertyChanged("RepTittle");
                OnRepTittleChanged();
            }
        }
        private string _RepTittle;
        partial void OnRepTittleChanging(string value);
        partial void OnRepTittleChanged();




        [DataMemberAttribute()]
        public string RepNumCode
        {
            get
            {
                return _RepNumCode;
            }
            set
            {
                OnRepNumCodeChanging(value);
                _RepNumCode = value;
                RaisePropertyChanged("RepNumCode");
                OnRepNumCodeChanged();
            }
        }
        private string _RepNumCode;
        partial void OnRepNumCodeChanging(string value);
        partial void OnRepNumCodeChanged();


        private bool _IsDeleted;
        [DataMemberAttribute()]
        public bool IsDeleted
        {
            get { return _IsDeleted; }
            set
            {
                _IsDeleted = value;
                RaisePropertyChanged("IsDeleted");
            }
        }


        private string _ReceiptIssueMauSo;
        [Required(ErrorMessage = "Hãy nhập vào Mẫu Số")]
        [DataMemberAttribute()]
        public string ReceiptIssueMauSo
        {
            get { return _ReceiptIssueMauSo; }
            set
            {
                _ReceiptIssueMauSo = value;
                RaisePropertyChanged("ReceiptIssueMauSo");
            }
        }

        private string _ReceiptIssueKyHieu;
        [Required(ErrorMessage = "Hãy nhập vào Kí Hiệu")]
        [DataMemberAttribute()]
        public string ReceiptIssueKyHieu
        {
            get { return _ReceiptIssueKyHieu; }
            set
            {
                _ReceiptIssueKyHieu = value;
                RaisePropertyChanged("ReceiptIssueKyHieu");
            }
        }

        private string _ReceiptNumberFrom;
        [Required(ErrorMessage = "Hãy nhập vào Số Từ")]
        [DataMemberAttribute()]
        public string ReceiptNumberFrom
        {
            get { return _ReceiptNumberFrom; }
            set
            {
                _ReceiptNumberFrom = value;
                RaisePropertyChanged("ReceiptNumberFrom");
            }
        }

        
        private string _ReceiptNumberTo;
        [Required(ErrorMessage = "Hãy nhập vào Số Đến")]
        [DataMemberAttribute()]
        public string ReceiptNumberTo
        {
            get { return _ReceiptNumberTo; }
            set
            {
                _ReceiptNumberTo = value;
                RaisePropertyChanged("ReceiptNumberTo");
            }
        }
        #endregion

        #region Navigation Properties
        [DataMemberAttribute()]
        public Staff staff
        {
            get
            {
                return _staff;
            }
            set
            {
                OnstaffChanging(value);
                _staff = value;
                RaisePropertyChanged("staff");
                //StaffID = staff.StaffID;
                OnstaffChanged();
            }
        }
        private Staff _staff;
        partial void OnstaffChanging(Staff value);
        partial void OnstaffChanged();
        #endregion

        public bool ValidateReportPaymentReceiptByStaff(ReportPaymentReceiptByStaff rptPRBS, out ObservableCollection<ValidationResult> results)
        {
            results = new ObservableCollection<ValidationResult>();
            if (rptPRBS == null)
            {
                return false;
            }

            var vc = new ValidationContext(rptPRBS, null, null);

            bool isValid = Validator.TryValidateObject(rptPRBS, vc, results, true);
            return isValid;
        }
    }
}
