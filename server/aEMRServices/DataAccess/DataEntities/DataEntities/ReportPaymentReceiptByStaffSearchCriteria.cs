using System;
using System.Net;
using System.Windows;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public class ReportPaymentReceiptByStaffSearchCriteria : SearchCriteriaBase
    {
        public ReportPaymentReceiptByStaffSearchCriteria()
        {
            FromDate = DateTime.Now;
            ToDate = DateTime.Now;
        }

        private DateTime _FromDate;
        public DateTime FromDate
        {
            get 
            {
                return _FromDate; 
            }
            set 
            {
                _FromDate = value;
                RaisePropertyChanged("FromDate");
            }
        }

        private DateTime _ToDate;
        public DateTime ToDate
        {
            get
            {
                return _ToDate;
            }
            set
            {
                _ToDate = value;
                RaisePropertyChanged("ToDate");
            }
        }

        private Int64 _StaffID;
        public Int64 StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                _StaffID = value;
                RaisePropertyChanged("StaffID");
            }
        }

    }
}