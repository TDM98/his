using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using eHCMS.Services.Core;

namespace DataEntities
{
    public class SearchOutwardInfo : SearchCriteriaBase
    {
        public static SearchOutwardInfo CreateSearchOutwardInfo(String OutInvID)
        {
            SearchOutwardInfo p = new SearchOutwardInfo();
            p.OutInvID = OutInvID;
            return p;
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

        private string _OutInvID;
        public string OutInvID
        {
            get
            {
                return _OutInvID;
            }
            set
            {
                _OutInvID = value;
                RaisePropertyChanged("OutInvID");
            }
        }

        private string _CustomerName;
        public string CustomerName
        {
            get
            {
                return _CustomerName;
            }
            set
            {
                _CustomerName = value;
                RaisePropertyChanged("CustomerName");
            }
        }

        private Nullable<DateTime> _fromdate;
        public Nullable<DateTime> fromdate
        {
            get
            {
                return _fromdate;
            }
            set
            {
                _fromdate = value;
                RaisePropertyChanged("fromdate");
            }
        }

        private Nullable<DateTime> _todate;
        public Nullable<DateTime> todate
        {
            get
            {
                return _todate;
            }
            set
            {
                _todate = value;
                RaisePropertyChanged("todate");
            }
        }

        private Nullable<DateTime> _fromdatedk;
        public Nullable<DateTime> fromdatedk
        {
            get
            {
                return _fromdatedk;
            }
            set
            {
                _fromdatedk = value;
                RaisePropertyChanged("fromdatedk");
            }
        }

        private Nullable<DateTime> _todatedk;
        public Nullable<DateTime> todatedk
        {
            get
            {
                return _todatedk;
            }
            set
            {
                _todatedk = value;
                RaisePropertyChanged("todatedk");
            }
        }

        private long? _StoreID;
        public long? StoreID
        {
            get
            {
                return _StoreID;
            }
            set
            {
                _StoreID = value;
                RaisePropertyChanged("StoreID");
            }
        }

        private long? _PtRegistrationID;
        public long? PtRegistrationID
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

        private long? _TypID;
        public long? TypID
        {
            get
            {
                return _TypID;
            }
            set
            {
                _TypID = value;
                RaisePropertyChanged("TypID");
            }
        }

        private string _PatientCode;
        public string PatientCode
        {
            get
            {
                return _PatientCode;
            }
            set
            {
                _PatientCode = value;
                RaisePropertyChanged("PatientCode");
            }
        }

        private string _HICardCode;
        public string HICardCode
        {
            get
            {
                return _HICardCode;
            }
            set
            {
                _HICardCode = value;
                RaisePropertyChanged("HICardCode");
            }
        }

        private string _PatientNameString;
        public string PatientNameString
        {
            get
            {
                return _PatientNameString;
            }
            set
            {
                _PatientNameString = value;
                RaisePropertyChanged("PatientNameString");
            }
        }

        public string PMFCode
        {
            get
            {
                return _PMFCode;
            }
            set
            {
                _PMFCode = value;
                RaisePropertyChanged("PMFCode");
            }
        }
        private string _PMFCode;

        private long? _V_OutDrugInvStatus;
        public long? V_OutDrugInvStatus
        {
            get
            {
                return _V_OutDrugInvStatus;
            }
            set
            {
                _V_OutDrugInvStatus = value;
                RaisePropertyChanged("V_OutDrugInvStatus");
            }
        }

        private long? _ID;
        public long? ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
                RaisePropertyChanged("ID");
            }
        }

        private bool _IsNotSolve;
        public bool IsNotSolve
        {
            get
            {
                return _IsNotSolve;
            }
            set
            {
                _IsNotSolve = value;
                RaisePropertyChanged("IsNotSolve");
            }
        }
       
    }

    public class SearchOutwardReport : SearchCriteriaBase
    {
        private Nullable<DateTime> _fromdate=DateTime.Now;
        public Nullable<DateTime> fromdate
        {
            get
            {
                return _fromdate;
            }
            set
            {
                _fromdate = value;
                RaisePropertyChanged("fromdate");
            }
        }

        private Nullable<DateTime> _todate=DateTime.Now;
        public Nullable<DateTime> todate
        {
            get
            {
                return _todate;
            }
            set
            {
                _todate = value;
                RaisePropertyChanged("todate");
            }
        }

        private long? _StaffID;
        public long? StaffID
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

        private long? _V_PharmacyOutRepType;
        public long? V_PharmacyOutRepType
        {
            get
            {
                return _V_PharmacyOutRepType;
            }
            set
            {
                _V_PharmacyOutRepType = value;
                RaisePropertyChanged("V_PharmacyOutRepType");
            }
        }

        //--▼--02/02/2021 DatTB thêm biến PTTT cho class SearchCriteria
        private long? _V_PaymentMode;
        public long? V_PaymentMode
        {
            get
            {
                return _V_PaymentMode;
            }
            set
            {
                _V_PaymentMode = value;
                RaisePropertyChanged("V_PaymentMode");
}
        }
        //--▲--02/02/2021 DatTB

        private long? _V_TradingPlaces;
        public long? V_TradingPlaces
        {
            get
            {
                return _V_TradingPlaces;
            }
            set
            {
                _V_TradingPlaces = value;
                RaisePropertyChanged("V_TradingPlaces");
            }
        }
        private int? _IsReport;
        public int? IsReport
        {
            get
            {
                return _IsReport;
            }
            set
            {
                _IsReport = value;
                RaisePropertyChanged("IsReport");
            }
        }

        private int? _IsDeleted;//0:dang dung;1:da huy;2:tat ca
        public int? IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                _IsDeleted = value;
                RaisePropertyChanged("IsDeleted");
            }
        }

        private long? _PharmacyOutRepID;
        public long? PharmacyOutRepID
        {
            get
            {
                return _PharmacyOutRepID;
            }
            set
            {
                _PharmacyOutRepID = value;
                RaisePropertyChanged("PharmacyOutRepID");
            }
        }

        public long StoreID { get => _StoreID; set
            {
                _StoreID = value;
                RaisePropertyChanged("StoreID");
            }
        }
        private long _StoreID;
    }
}