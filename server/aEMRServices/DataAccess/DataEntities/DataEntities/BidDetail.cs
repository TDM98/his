using eHCMS.Services.Core.Base;
using System.Runtime.Serialization;
/*
 * 20210122 #001 TNHX: thêm biên thông tin gói thầu + mã nhóm
 * 20230323 #002 QTD:  Thêm năm
 */
namespace DataEntities
{
    public class BidDetail : NotifyChangedBase
    {
        private long _BidDetailID;
        [DataMemberAttribute]
        public long BidDetailID
        {
            get
            {
                return _BidDetailID;
            }
            set
            {
                if (_BidDetailID == value)
                {
                    return;
                }
                _BidDetailID = value;
                RaisePropertyChanged("BidDetailID");
            }
        }

        private long _BidID;
        [DataMemberAttribute]
        public long BidID
        {
            get
            {
                return _BidID;
            }
            set
            {
                if (_BidID == value)
                {
                    return;
                }
                _BidID = value;
                RaisePropertyChanged("BidID");
            }
        }

        private long _DrugID;
        [DataMemberAttribute]
        public long DrugID
        {
            get
            {
                return _DrugID;
            }
            set
            {
                if (_DrugID == value)
                {
                    return;
                }
                _DrugID = value;
                RaisePropertyChanged("DrugID");
            }
        }

        private long _VersionID;
        [DataMemberAttribute]
        public long VersionID
        {
            get
            {
                return _VersionID;
            }
            set
            {
                if (_VersionID == value)
                {
                    return;
                }
                _VersionID = value;
                RaisePropertyChanged("VersionID");
            }
        }

        private long _ApprovedVersionID;
        [DataMemberAttribute]
        public long ApprovedVersionID
        {
            get
            {
                return _ApprovedVersionID;
            }
            set
            {
                if (_ApprovedVersionID == value)
                {
                    return;
                }
                _ApprovedVersionID = value;
                RaisePropertyChanged("ApprovedVersionID");
            }
        }

        private bool _IsMedDept;
        [DataMemberAttribute]
        public bool IsMedDept
        {
            get
            {
                return _IsMedDept;
            }
            set
            {
                if (_IsMedDept == value)
                {
                    return;
                }
                _IsMedDept = value;
                RaisePropertyChanged("IsMedDept");
            }
        }

        private string _DrugCode;
        [DataMemberAttribute]
        public string DrugCode
        {
            get
            {
                return _DrugCode;
            }
            set
            {
                if (_DrugCode == value)
                {
                    return;
                }
                _DrugCode = value;
                RaisePropertyChanged("DrugCode");
            }
        }

        private string _ReportBrandName;
        [DataMemberAttribute]
        public string ReportBrandName
        {
            get
            {
                return _ReportBrandName;
            }
            set
            {
                if (_ReportBrandName == value)
                {
                    return;
                }
                _ReportBrandName = value;
                RaisePropertyChanged("ReportBrandName");
            }
        }

        private string _Visa;
        [DataMemberAttribute]
        public string Visa
        {
            get
            {
                return _Visa;
            }
            set
            {
                if (_Visa == value)
                {
                    return;
                }
                _Visa = value;
                RaisePropertyChanged("Visa");
            }
        }

        private bool _IsDeleted;
        [DataMemberAttribute]
        public bool IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                if (_IsDeleted == value)
                {
                    return;
                }
                _IsDeleted = value;
                RaisePropertyChanged("IsDeleted");
            }
        }

        private string _BidName;
        [DataMemberAttribute]
        public string BidName
        {
            get
            {
                return _BidName;
            }
            set
            {
                if (_BidName == value)
                {
                    return;
                }
                _BidName = value;
                RaisePropertyChanged("BidName");
            }
        }

        private string _HICode;
        [DataMemberAttribute]
        public string HICode
        {
            get
            {
                return _HICode;
            }
            set
            {
                if (_HICode == value)
                {
                    return;
                }
                _HICode = value;
                RaisePropertyChanged("HICode");
            }
        }

        private string _GroupCode;
        [DataMemberAttribute]
        public string GroupCode
        {
            get
            {
                return _GroupCode;
            }
            set
            {
                if (_GroupCode == value)
                {
                    return;
                }
                _GroupCode = value;
                RaisePropertyChanged("GroupCode");
            }
        }

        private string _BidCode;
        [DataMemberAttribute]
        public string BidCode
        {
            get
            {
                return _BidCode;
            }
            set
            {
                if (_BidCode == value)
                {
                    return;
                }
                _BidCode = value;
                RaisePropertyChanged("BidCode");
            }
        }

        private string _Content;
        private int _ApprovedQty;
        private bool _IsApproved = false;
        private int _RemainingQty;
        private int _InQuantity;
        private DrugDeptSupplier _Supplier;
        private decimal _InCost;
        [DataMemberAttribute]
        public string Content
        {
            get
            {
                return _Content;
            }
            set
            {
                if (_Content == value)
                {
                    return;
                }
                _Content = value;
                RaisePropertyChanged("Content");
            }
        }
        [DataMemberAttribute]
        public int ApprovedQty
        {
            get
            {
                return _ApprovedQty;
            }
            set
            {
                if (_ApprovedQty == value)
                {
                    return;
                }
                _ApprovedQty = value;
                RaisePropertyChanged("ApprovedQty");
            }
        }
        [DataMemberAttribute]
        public bool IsApproved
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
        [DataMemberAttribute]
        public int RemainingQty
        {
            get
            {
                return _RemainingQty;
            }
            set
            {
                _RemainingQty = value;
                RaisePropertyChanged("RemainingQty");
            }
        }
        [DataMemberAttribute]
        public int InQuantity
        {
            get
            {
                return _InQuantity;
            }
            set
            {
                _InQuantity = value;
                RaisePropertyChanged("InQuantity");
            }
        }
        [DataMemberAttribute]
        public DrugDeptSupplier Supplier
        {
            get
            {
                return _Supplier;
            }
            set
            {
                if (_Supplier == value)
                {
                    return;
                }
                _Supplier = value;
                RaisePropertyChanged("Supplier");
            }
        }
        [DataMemberAttribute]
        public decimal InCost
        {
            get
            {
                return _InCost;
            }
            set
            {
                _InCost = value;
                RaisePropertyChanged("InCost");
            }
        }

        private string _ContractNo;
        [DataMemberAttribute]
        public string ContractNo
        {
            get
            {
                return _ContractNo;
            }
            set
            {
                if (_ContractNo == value)
                {
                    return;
                }
                _ContractNo = value;
                RaisePropertyChanged("ContractNo");
            }
        }

        private bool _IsEditable = true;
        [DataMemberAttribute]
        public bool IsEditable
        {
            get
            {
                return _IsEditable;
            }
            set
            {
                if (_IsEditable == value)
                {
                    return;
                }
                _IsEditable = value;
                RaisePropertyChanged("IsEditable");
            }
        }
        private decimal _RemainingOutQty;
        [DataMemberAttribute]
        public decimal RemainingOutQty
        {
            get
            {
                return _RemainingOutQty;
            }
            set
            {
                if (_RemainingOutQty != value)
                {
                    _RemainingOutQty = value;
                    RaisePropertyChanged("RemainingOutQty");
                }
            }
        }
        private string _UnitName;
        [DataMemberAttribute()]
        public string UnitName
        {
            get
            {
                return _UnitName;
            }
            set
            {
                if (_UnitName != value)
                {
                    _UnitName = value;
                    RaisePropertyChanged("UnitName");
                }
            }
        }
        private string _RouteOfAdministration;
        [DataMemberAttribute()]
        public string RouteOfAdministration
        {
            get
            {
                return _RouteOfAdministration;
            }
            set
            {
                if (_RouteOfAdministration != value)
                {
                    _RouteOfAdministration = value;
                    RaisePropertyChanged("RouteOfAdministration");
                }
            }
        }

        //▼====: #001
        private bool _IsAddNew;
        [DataMemberAttribute]
        public bool IsAddNew
        {
            get
            {
                return _IsAddNew;
            }
            set
            {
                if (_IsAddNew == value)
                {
                    return;
                }
                _IsAddNew = value;
                RaisePropertyChanged("IsAddNew");
            }
        }

        private string _BidCodeStr;
        [DataMemberAttribute]
        public string BidCodeStr
        {
            get
            {
                return _BidCodeStr;
            }
            set
            {
                if (_BidCodeStr == value)
                {
                    return;
                }
                _BidCodeStr = value;
                RaisePropertyChanged("BidCodeStr");
            }
        }

        private string _BidGroupName;
        [DataMemberAttribute]
        public string BidGroupName
        {
            get
            {
                return _BidGroupName;
            }
            set
            {
                if (_BidGroupName == value)
                {
                    return;
                }
                _BidGroupName = value;
                RaisePropertyChanged("BidGroupName");
            }
        }

        private string _TCKTAndTCCNGroup;
        [DataMemberAttribute]
        public string TCKTAndTCCNGroup
        {
            get
            {
                return _TCKTAndTCCNGroup;
            }
            set
            {
                if (_TCKTAndTCCNGroup == value)
                {
                    return;
                }
                _TCKTAndTCCNGroup = value;
                RaisePropertyChanged("TCKTAndTCCNGroup");
            }
        }
        //▲====: #001

        //▼====: #002
        private string _YearStr;
        [DataMemberAttribute]
        public string YearStr
        {
            get
            {
                return _YearStr;
            }
            set
            {
                _YearStr = value;
                RaisePropertyChanged("YearStr");
            }
        }
        //▲====: #002
    }
}