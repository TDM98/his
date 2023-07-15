using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Linq;
using eHCMS.Services.Core.Base;
/*
 *  20200928 #001 TNHX: Add IncludeAmountCoPay for skip amount co pay
 */
namespace DataEntities
{
    public partial class PromoDiscountProgram : NotifyChangedBase
    {
        private long _PromoDiscProgID;
        private string _PromoDiscCode;
        private string _PromoDiscName;
        private short _V_PromoDiscType;
        private long _StaffID;
        private long _AuthorizedStaffID;
        private string _ReasonOrNote;
        private DateTime? _ValidFromDate;
        private DateTime? _ValidToDate;
        private DateTime _RecCreatedDate;
        private decimal _DiscountPercent;
        private bool _IsOnPriceDiscount;
        private long _V_RegistrationType;
        private Lookup _V_DiscountTypeCount;
        [DataMemberAttribute]
        public long PromoDiscProgID
        {
            get => _PromoDiscProgID; set
            {
                _PromoDiscProgID = value;
                RaisePropertyChanged("PromoDiscProgID");
            }
        }
        [DataMemberAttribute]
        public string PromoDiscCode
        {
            get => _PromoDiscCode; set
            {
                _PromoDiscCode = value;
                RaisePropertyChanged("PromoDiscCode");
            }
        }
        [DataMemberAttribute]
        public string PromoDiscName
        {
            get => _PromoDiscName; set
            {
                _PromoDiscName = value;
                RaisePropertyChanged("PromoDiscName");
            }
        }
        [DataMemberAttribute]
        public short V_PromoDiscType
        {
            get => _V_PromoDiscType; set
            {
                _V_PromoDiscType = value;
                RaisePropertyChanged("V_PromoDiscType");
            }
        }
        [DataMemberAttribute]
        public long StaffID
        {
            get => _StaffID; set
            {
                _StaffID = value;
                RaisePropertyChanged("StaffID");
            }
        }
        [DataMemberAttribute]
        public long AuthorizedStaffID
        {
            get => _AuthorizedStaffID; set
            {
                _AuthorizedStaffID = value;
                RaisePropertyChanged("AuthorizedStaffID");
            }
        }
        [DataMemberAttribute]
        public string ReasonOrNote
        {
            get => _ReasonOrNote; set
            {
                _ReasonOrNote = value;
                RaisePropertyChanged("ReasonOrNote");
            }
        }
        [DataMemberAttribute]
        public DateTime? ValidFromDate
        {
            get => _ValidFromDate; set
            {
                _ValidFromDate = value;
                RaisePropertyChanged("ValidFromDate");
            }
        }
        [DataMemberAttribute]
        public DateTime? ValidToDate
        {
            get => _ValidToDate; set
            {
                _ValidToDate = value;
                RaisePropertyChanged("ValidToDate");
            }
        }
        [DataMemberAttribute]
        public DateTime RecCreatedDate
        {
            get => _RecCreatedDate; set
            {
                _RecCreatedDate = value;
                RaisePropertyChanged("RecCreatedDate");
            }
        }
        [DataMemberAttribute]
        public decimal DiscountPercent
        {
            get => _DiscountPercent; set
            {
                _DiscountPercent = value;
                RaisePropertyChanged("DiscountPercent");
            }
        }
        [DataMemberAttribute]
        public bool IsOnPriceDiscount
        {
            get
            {
                return _IsOnPriceDiscount;
            }
            set
            {
                _IsOnPriceDiscount = value;
                RaisePropertyChanged("IsOnPriceDiscount");
            }
        }
        private RecordState _RecordState = RecordState.DETACHED;
        [DataMemberAttribute()]
        public RecordState RecordState
        {
            get
            {
                return _RecordState;
            }
            set
            {
                _RecordState = value;
                RaisePropertyChanged("RecordState");
            }
        }
        [DataMemberAttribute]
        public Staff ConfirmedStaff
        {
            get => _ConfirmedStaff; set
            {
                _ConfirmedStaff = value;
                RaisePropertyChanged("ConfirmedStaff");
            }
        }
        [DataMemberAttribute]
        public long V_RegistrationType
        {
            get
            {
                return _V_RegistrationType;
            }
            set
            {
                _V_RegistrationType = value;
                RaisePropertyChanged("V_RegistrationType");
            }
        }

        private Staff _ConfirmedStaff;
        public string ConvertToXml()
        {
            //20211020 BLQ: Bỏ điều kiện Unchanged vì không áp dụng danh sách miễn giảm tạo trong cấu hình được
            //if (this == null || RecordState == RecordState.UNCHANGED)
            if (this == null)
                return null;
            var mXDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), new XElement("PromoDiscountProgram",
                new XElement("PromoDiscProgID", PromoDiscProgID),
                new XElement("PromoDiscName", PromoDiscName),
                new XElement("DiscountPercent", DiscountPercent),
                new XElement("PromoDiscCode", PromoDiscCode),
                new XElement("ReasonOrNote", ReasonOrNote = ReasonOrNote),
                new XElement("ConfirmedStaffID", ConfirmedStaff == null ? null : (long?)ConfirmedStaff.StaffID),
                new XElement("IsOnPriceDiscount", IsOnPriceDiscount),
                new XElement("V_DiscountTypeCount", V_DiscountTypeCount.LookupID)));
            return mXDocument.ToString();
        }
        public bool CompareValues(PromoDiscountProgram cObj)
        {
            return cObj.PromoDiscCode == PromoDiscCode
                && cObj.PromoDiscName == PromoDiscName
                && cObj.DiscountPercent == DiscountPercent
                && cObj.ReasonOrNote == ReasonOrNote;
        }

        [DataMemberAttribute]
        public Lookup V_DiscountTypeCount
        {
            get
            {
                return _V_DiscountTypeCount;
            }
            set
            {
                _V_DiscountTypeCount = value;
                RaisePropertyChanged("V_DiscountTypeCount");
            }
        }

        private string _StaffName;

        [DataMemberAttribute]
        public string StaffName
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

        private List<PromoDiscountItems> _PromoDiscountItems;

        [DataMemberAttribute]
        public List<PromoDiscountItems> PromoDiscountItems
        {
            get
            {
                return _PromoDiscountItems;
            }
            set
            {
                _PromoDiscountItems = value;
                RaisePropertyChanged("PromoDiscountItems");
            }
        }
        private string _V_RegistrationTypeName;

        [DataMemberAttribute]
        public string V_RegistrationTypeName
        {
            get
            {
                return _V_RegistrationTypeName;
            }
            set
            {
                _V_RegistrationTypeName = value;
                RaisePropertyChanged("V_RegistrationTypeName");
            }
        }
    }
}
