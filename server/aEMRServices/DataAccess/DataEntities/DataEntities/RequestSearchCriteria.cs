using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using eHCMS.Services.Core;

namespace DataEntities
{
    public class RequestSearchCriteria : SearchCriteriaBase
    {
        public RequestSearchCriteria()
        {

        }
        private string _Code;
        public string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                _Code = value;
                RaisePropertyChanged("Code");
            }
        }
        private Nullable<long> _StaffID;
        public Nullable<long> StaffID
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

        private Nullable<long> _SupplierID;
        public Nullable<long> SupplierID
        {
            get
            {
                return _SupplierID;
            }
            set
            {
                _SupplierID = value;
                RaisePropertyChanged("SupplierID");
            }
        }

        private Nullable<DateTime> _FromDate;
        public Nullable<DateTime> FromDate
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

        //Tim theo 2 code : VD : neu chung ta tim phieu dat hang thi code dau tien se la code cua dat hang 
        //code thu 2 se la code cua phieu du tru
        private string _Code1;
        public string Code1
        {
            get
            {
                return _Code1;
            }
            set
            {
                _Code1 = value;
                RaisePropertyChanged("Code1");
            }
        }

        private Nullable<bool> _IsNotOrder;
        public Nullable<bool> IsNotOrder
        {
            get
            {
                return _IsNotOrder;
            }
            set
            {
                _IsNotOrder = value;
                RaisePropertyChanged("IsNotOrder");
            }
        }


        private Nullable<bool> _DaNhanHang;
        public Nullable<bool> DaNhanHang
        {
            get
            {
                return _DaNhanHang;
            }
            set
            {
                _DaNhanHang = value;
                RaisePropertyChanged("DaNhanHang");
            }
        }

        private Nullable<bool> _IsApproved;
        public Nullable<bool> IsApproved
        {
            get
            {
                return _IsApproved;
            }
            set
            {
                _IsApproved = value;
                RaisePropertyChanged("IsApproved");
            }
        }

        private bool _FindByApprovedDate;
        public bool FindByApprovedDate
        {
            get
            {
                return _FindByApprovedDate;
            }
            set
            {
                _FindByApprovedDate = value;
                RaisePropertyChanged("FindByApprovedDate");
            }
        }

        private Nullable<long> _V_LookupID;
        public Nullable<long> V_LookupID
        {
            get
            {
                return _V_LookupID;
            }
            set
            {
                _V_LookupID = value;
                RaisePropertyChanged("V_LookupID");
            }
        }

        private Nullable<long> _RequestStoreID;
        public Nullable<long> RequestStoreID
        {
            get
            {
                return _RequestStoreID;
            }
            set
            {
                _RequestStoreID = value;
                RaisePropertyChanged("RequestStoreID");
            }
        }

        private Nullable<long> _PtRegistrationID;
        public Nullable<long> PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                _PtRegistrationID = value;
                RaisePropertyChanged("PtRegistrationID");
            }
        }


    }
}
