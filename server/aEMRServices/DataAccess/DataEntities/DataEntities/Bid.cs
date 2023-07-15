using eHCMS.Services.Core.Base;
using System.Runtime.Serialization;
using System;
/*
 * 20210122 #001 TNHX: thêm biên thông tin gói thầu + mã nhóm
 * 20230408 #002 QTD:  Thêm năm
 */
namespace DataEntities
{
    public class Bid : NotifyChangedBase
    {
        private long _V_MedProductType;
        [DataMemberAttribute]
        public long V_MedProductType
        {
            get
            {
                return _V_MedProductType;
            }
            set
            {
                _V_MedProductType = value;
                RaisePropertyChanged("V_MedProductType");
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
                _IsMedDept = value;
                RaisePropertyChanged("IsMedDept");
            }
        }

        private long _StaffID;
        [DataMemberAttribute]
        public long StaffID
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
                _BidID = value;
                RaisePropertyChanged("BidID");
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
                _BidName = value;
                RaisePropertyChanged("BidName");
            }
        }

        private string _PermitNumber;
        [DataMemberAttribute]
        public string PermitNumber
        {
            get
            {
                return _PermitNumber;
            }
            set
            {
                _PermitNumber = value;
                RaisePropertyChanged("PermitNumber");
            }
        }

        private DateTime _ValidDateFrom;
        [DataMemberAttribute]
        public DateTime ValidDateFrom
        {
            get
            {
                return _ValidDateFrom;
            }
            set
            {
                _ValidDateFrom = value;
                RaisePropertyChanged("ValidDateFrom");
            }
        }

        private DateTime _ValidDateTo;
        [DataMemberAttribute]
        public DateTime ValidDateTo
        {
            get
            {
                return _ValidDateTo;
            }
            set
            {
                _ValidDateTo = value;
                RaisePropertyChanged("ValidDateTo");
            }
        }
        
        //▼====: #001
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
                _BidGroupName = value;
                RaisePropertyChanged("BidGroupName");
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
