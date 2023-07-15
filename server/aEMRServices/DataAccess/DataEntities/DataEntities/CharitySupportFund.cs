using System;
using System.Runtime.Serialization;
using eHCMS.Services.Core.Base;
using System.ComponentModel.DataAnnotations;
namespace DataEntities
{
    public partial class CharityOrganization : NotifyChangedBase
    {
        public CharityOrganization()
        {
 
        }
        
        //OrganizeID, OrganizeName,  ContactInfo
        [DataMemberAttribute()]
        public long CharityOrgID
        {
            get
            {
                return _CharityOrgID;
            }
            set
            {
                _CharityOrgID = value;
                RaisePropertyChanged("CharityOrgID");
            }
        }
        private long _CharityOrgID;

        [DataMemberAttribute()]
        public string CharityOrgName
        {
            get
            {
                return _CharityOrgName;
            }
            set
            {
                _CharityOrgName = value;
                RaisePropertyChanged("CharityOrgName");
            }
        }
        private string _CharityOrgName;

        [DataMemberAttribute()]
        public string ContactInfo
        {
            get
            {
                return _ContactInfo;
            }
            set
            {
                _ContactInfo = value;
                RaisePropertyChanged("ContactInfo");
            }
        }
        private string _ContactInfo;

        private string _TaxCode;
        private string _TaxMemberAddress;
        private string _TaxMemberName;
        [DataMemberAttribute]
        public string TaxCode
        {
            get
            {
                return _TaxCode;
            }
            set
            {
                _TaxCode = value;
                RaisePropertyChanged("TaxCode");
            }
        }
        [DataMemberAttribute]
        public string TaxMemberAddress
        {
            get
            {
                return _TaxMemberAddress;
            }
            set
            {
                _TaxMemberAddress = value;
                RaisePropertyChanged("TaxMemberAddress");
            }
        }
        [DataMemberAttribute]
        public string TaxMemberName
        {
            get
            {
                return _TaxMemberName;
            }
            set
            {
                _TaxMemberName = value;
                RaisePropertyChanged("TaxMemberName");
            }
        }
    }
    public partial class CharitySupportFund : NotifyChangedBase
    {
        public CharitySupportFund()
        {
            _IsUsedPercent = true;
        }

        [DataMemberAttribute()]
        public bool IsUsedPercent
        {
            get
            {
                return _IsUsedPercent;
            }
            set
            {
                _IsUsedPercent = value;
                RaisePropertyChanged("IsUsedPercent");
            }
        }
        private bool _IsUsedPercent;

        [DataMemberAttribute()]
        public long CharityFundID
        {
            get
            {
                return _CharityFundID;
            }
            set
            {
                _CharityFundID = value;
                RaisePropertyChanged("CharityFundID");
            }
        }
        private long _CharityFundID;

        [DataMemberAttribute()]
        public long PtRegistrationID
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
        private long _PtRegistrationID;

        [DataMemberAttribute()]
        public long BillingInvID
        {
            get
            {
                return _BillingInvID;
            }
            set
            {
                _BillingInvID = value;
                RaisePropertyChanged("BillingInvID");
            }
        }
        private long _BillingInvID;

        [DataMemberAttribute()]
        public CharityOrganization CharityOrgInfo
        {
            get
            {
                return _CharityOrgInfo;
            }
            set
            {
                _CharityOrgInfo = value;
                RaisePropertyChanged("CharityOrgInfo");
            }
        }
        private CharityOrganization _CharityOrgInfo;       

        [DataMemberAttribute()]
        public Decimal PercentValue
        {
            get
            {
                return _PercentValue;
            }
            set
            {
                _PercentValue = value;
                RaisePropertyChanged("PercentValue");
            }
        }
        private Decimal _PercentValue;

        [DataMemberAttribute()]
        public Decimal AmountValue
        {
            get
            {
                return _AmountValue;
            }
            set
            {
                _AmountValue = value;
                RaisePropertyChanged("AmountValue");
            }
        }
        private Decimal _AmountValue;

        [DataMemberAttribute()]
        public Decimal? BalanceAmount
        {
            get
            {
                return _BalanceAmount;
            }
            set
            {
                _BalanceAmount = value;
                RaisePropertyChanged("BalanceAmount");
            }
        }
        private Decimal? _BalanceAmount;

        [DataMemberAttribute()]
        public Staff CreatedStaff
        {
            get
            {
                return _CreatedStaff;
            }
            set
            {
                _CreatedStaff = value;
                RaisePropertyChanged("CreatedStaff");
            }
        }
        private Staff _CreatedStaff;

        [DataMemberAttribute()]
        public CommonRecordState RecordState
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
        private CommonRecordState _RecordState = CommonRecordState.UNCHANGED;

        [DataMemberAttribute()]
        public bool IsHighTechServiceBill
        {
            get
            {
                return _IsHighTechServiceBill;
            }
            set
            {
                _IsHighTechServiceBill = value;
                RaisePropertyChanged("IsHighTechServiceBill");
            }
        }
        private bool _IsHighTechServiceBill;

        /*TMA 10/11/2017*/
        [Required(ErrorMessage = "Chọn đối tượng!")]
        [DataMemberAttribute()]
        public Int64 V_CharityObjectType
        {
            get { return _V_CharityObjectType; }
            set
            {
                if (
                    _V_CharityObjectType != value)
                {
                    OnV_CharityObjectTypeChanging(value);
                    ValidateProperty("V_CharityObjectType", value);
                    _V_CharityObjectType = value;
                    RaisePropertyChanged("V_CharityObjectType");
                    OnV_CharityObjectTypeChanged();
                }
            }
        }
        private Int64 _V_CharityObjectType;
        partial void OnV_CharityObjectTypeChanging(Int64 value);
        partial void OnV_CharityObjectTypeChanged();

        [DataMemberAttribute()]
        public Lookup ObjV_CharityObjectType
        {
            get { return _ObjV_CharityObjectType; }
            set
            {
                if (_ObjV_CharityObjectType != value)
                {
                    OnObjV_CharityObjectTypeChanging(value);
                    _ObjV_CharityObjectType = value;
                    RaisePropertyChanged("ObjV_CharityObjectType");
                    if (ObjV_CharityObjectType != null)
                    {
                        V_CharityObjectType = ObjV_CharityObjectType.LookupID;
                    }
                    OnObjV_CharityObjectTypeChanged();
                }
            }
        }
        private Lookup _ObjV_CharityObjectType;
        partial void OnObjV_CharityObjectTypeChanging(Lookup value);
        partial void OnObjV_CharityObjectTypeChanged();
        /*TMA*/
    }
}

     
      
