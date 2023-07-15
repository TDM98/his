using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using eHCMS.Services.Core;

namespace DataEntities
{
    public class InwardInvoiceSearchCriteria : SearchCriteriaBase
    {
        public InwardInvoiceSearchCriteria()
        {
            _orderBy = String.Empty;
        }
        private string _orderBy;
        public string OrderBy
        {
            get
            {
                return _orderBy;
            }
            set
            {
                _orderBy = value;
                RaisePropertyChanged("OrderBy");
            }
        }

        private string _inwardID;
        public string InwardID
        {
            get
            {
                return _inwardID;
            }
            set
            {
                _inwardID = value;
                RaisePropertyChanged("InwardID");
            }
        }
        private string _invoiceNumber;
        public string InvoiceNumber
        {
            get
            {
                return _invoiceNumber;
            }
            set
            {
                _invoiceNumber = value;
                RaisePropertyChanged("InvoiceNumber");
            }
        }

        private Nullable<DateTime> _dateInvoice;
        public Nullable<DateTime> DateInvoice
        {
            get
            {
                return _dateInvoice;
            }
            set
            {
                _dateInvoice = value;
                RaisePropertyChanged("DateInvoice");
            }
        }

        private Nullable<long> _supplierID;
        public Nullable<long> SupplierID
        {
            get
            {
                return _supplierID;
            }
            set
            {
                _supplierID = value;
                RaisePropertyChanged("SupplierID");
            }
        }

        private Nullable<DateTime> _Fromdate;
        public Nullable<DateTime> FromDate
        {
            get
            {
                return _Fromdate;
            }
            set
            {
                _Fromdate = value;
                RaisePropertyChanged("FromDate");
            }
        }

        private Nullable<DateTime> _ToDate;
        public Nullable<DateTime> ToDate
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

        private Nullable<Boolean> _IsInputMedDept;
        public Nullable<Boolean> IsInputMedDept
        {
            get
            {
                return _IsInputMedDept;
            }
            set
            {
                _IsInputMedDept = value;
                RaisePropertyChanged("IsInputMedDept");
            }
        }

        private long _InDeptID;
        public long InDeptID
        {
            get
            {
                return _InDeptID;
            }
            set
            {
                _InDeptID = value;
                RaisePropertyChanged("InDeptID");
            }
        }
        private bool _IsMedDeptSubStorage = false;
        public bool IsMedDeptSubStorage
        {
            get => _IsMedDeptSubStorage; set
            {
                _IsMedDeptSubStorage = value;
                RaisePropertyChanged("IsMedDeptSubStorage");
            }
        }

        private long _inviID;
        public long inviID
        {
            get => _inviID; set
            {
                _inviID = value;
                RaisePropertyChanged("inviID");
            }
        }
    }
}