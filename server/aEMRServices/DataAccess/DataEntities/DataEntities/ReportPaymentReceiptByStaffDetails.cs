using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    [DataContract]
    public partial class ReportPaymentReceiptByStaffDetails : NotifyChangedBase
    {
        public ReportPaymentReceiptByStaffDetails()
        {
            
        }
        #region Factory Method


        /// Create a new ReportPaymentReceiptByStaffDetails object.

        /// <param name="ptPmtID">Initial value of the PtPmtID property.</param>
        /// <param name="payAmount">Initial value of the PayAmount property.</param>
        public static ReportPaymentReceiptByStaffDetails CreateReportPaymentReceiptByStaffDetails(long repPaymentRecvDetailID)
        {
            ReportPaymentReceiptByStaffDetails ReportPaymentReceiptByStaffDetails = new ReportPaymentReceiptByStaffDetails();
            ReportPaymentReceiptByStaffDetails.RepPaymentRecvDetailID = repPaymentRecvDetailID;
            return ReportPaymentReceiptByStaffDetails;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long RepPaymentRecvDetailID
        {
            get
            {
                return _RepPaymentRecvDetailID;
            }
            set
            {
                OnRepPaymentRecvDetailIDChanging(value);
                _RepPaymentRecvDetailID = value;
                RaisePropertyChanged("RepPaymentRecvDetailID");
                OnRepPaymentRecvDetailIDChanged();
            }
        }
        private long _RepPaymentRecvDetailID;
        partial void OnRepPaymentRecvDetailIDChanging(long value);
        partial void OnRepPaymentRecvDetailIDChanged();




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
        public long PtPmtID
        {
            get
            {
                return _PtPmtID;
            }
            set
            {
                OnPtPmtIDChanging(value);
                _PtPmtID = value;
                RaisePropertyChanged("PtPmtID");
                OnPtPmtIDChanged();
            }
        }
        private long _PtPmtID;
        partial void OnPtPmtIDChanging(long value);
        partial void OnPtPmtIDChanged();

        #endregion

        #region Navigation Properties
        [DataMemberAttribute()]
        public List<PatientTransactionPayment> allPatientPayment
        {
            get
            {
                return _allPatientPayment;
            }
            set
            {
                OnallPatientPaymentChanging(value);
                _allPatientPayment = value;
                RaisePropertyChanged("allPatientPayment");
                OnallPatientPaymentChanged();
            }
        }
        private List<PatientTransactionPayment> _allPatientPayment;
        partial void OnallPatientPaymentChanging(List<PatientTransactionPayment> value);
        partial void OnallPatientPaymentChanged();

        #endregion

        
    }
}
