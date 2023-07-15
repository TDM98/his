/*
 * 20170519 #001 CMN: Save HIBenefit also on bill
 * 20191102 #002 TNHX: BM 0017411: UPDATE IsValidBill, 0 or NULL: skip, 1: ChangeHIBenifit, 2: Change from BH <=> DV + add MedPriceListID, PCLPriceListID
 * 20210929 #003 TNHX: 681 Thêm giá Nhà nước chi trả cho bn điều trị covid
 * 20220518 #004 BLQ: Thêm tích khoá bill
*/
using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    [DataContract]
    public partial class InPatientBillingInvoice : EntityBase,ITotalPrice,IDBRecordState
    {
        public InPatientBillingInvoice()
            : base()
        {
            _V_BillingInvType = AllLookupValues.V_BillingInvType.TINH_TIEN_NOI_TRU;
            _V_InPatientBillingInvStatus = AllLookupValues.V_InPatientBillingInvStatus.NEW;
        }

        private long _inPatientBillingInvID;
        [DataMemberAttribute()]
        public long InPatientBillingInvID
        {
            get
            {
                return _inPatientBillingInvID;
            }
            set
            {
                if (_inPatientBillingInvID != value)
                {
                    _inPatientBillingInvID = value;
                    RaisePropertyChanged("InPatientBillingInvID");
                    RaisePropertyChanged("CanEditPeriodOfTime");
                }
            }
        }
        private string _billingInvNum;
        [DataMemberAttribute()]
        public string BillingInvNum
        {
            get
            {
                return _billingInvNum;
            }
            set
            {
                if (_billingInvNum != value)
                {
                    _billingInvNum = value;
                    RaisePropertyChanged("BillingInvNum");
                }
            }
        }
        private long _PtRegistrationID;
        [DataMemberAttribute()]
        public long PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                if (_PtRegistrationID != value)
                {
                    _PtRegistrationID = value;
                    RaisePropertyChanged("PtRegistrationID");
                }
            }
        }
        private long _StaffID;
        [DataMemberAttribute()]
        public long StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                if (_StaffID != value)
                {
                    _StaffID = value;
                    RaisePropertyChanged("StaffID");
                }
            }
        }

        private string _FullName;
        [DataMemberAttribute()]
        public string FullName
        {
            get
            {
                return _FullName;
            }
            set
            {
                if (_FullName != value)
                {
                    _FullName = value;
                    RaisePropertyChanged("FullName");
                }
            }
        }
        private DateTime _InvDate;
        [DataMemberAttribute()]
        public DateTime InvDate
        {
            get
            {
                return _InvDate;
            }
            set
            {
                if (_InvDate != value)
                {
                    _InvDate = value;
                    RaisePropertyChanged("InvDate");
                }
            }
        }
        private AllLookupValues.V_BillingInvType _V_BillingInvType;
        [DataMemberAttribute()]
        public AllLookupValues.V_BillingInvType V_BillingInvType
        {
            get
            {
                return _V_BillingInvType;
            }
            set
            {
                if (_V_BillingInvType != value)
                {
                    _V_BillingInvType = value;
                    RaisePropertyChanged("V_BillingInvType");
                }
            }
        }
        
        private AllLookupValues.V_InPatientBillingInvStatus _V_InPatientBillingInvStatus;
        [DataMemberAttribute()]
        public AllLookupValues.V_InPatientBillingInvStatus V_InPatientBillingInvStatus
        {
            get
            {
                return _V_InPatientBillingInvStatus;
            }
            set
            {
                if (_V_InPatientBillingInvStatus != value)
                {
                    _V_InPatientBillingInvStatus = value;
                    RaisePropertyChanged("V_InPatientBillingInvStatus");
                }
            }
        }

        //private ObservableCollection<MedRegItemBase> _allRegistrationItems;
        //[DataMemberAttribute()]
        //public ObservableCollection<MedRegItemBase> AllRegistrationItems
        //{
        //    get { return _allRegistrationItems; }
        //    set
        //    {
        //        _allRegistrationItems = value;
        //        RaisePropertyChanged("AllRegistrationItems");
        //    }
        //}

        private ObservableCollection<PatientRegistrationDetail> _registrationDetails;
        [DataMemberAttribute()]
        public ObservableCollection<PatientRegistrationDetail> RegistrationDetails
        {
            get { return _registrationDetails; }
            set
            {
                _registrationDetails = value;
                RaisePropertyChanged("RegistrationDetails");
            }
        }

        private ObservableCollection<PatientPCLRequest> _pclRequests;
        [DataMemberAttribute()]
        public ObservableCollection<PatientPCLRequest> PclRequests
        {
            get { return _pclRequests; }
            set
            {
                _pclRequests = value;
                RaisePropertyChanged("PclRequests");
            }
        }

        private ObservableCollection<OutwardDrugClinicDeptInvoice> _outwardDrugClinicDeptInvoices;
        [DataMemberAttribute()]
        public ObservableCollection<OutwardDrugClinicDeptInvoice> OutwardDrugClinicDeptInvoices
        {
            get { return _outwardDrugClinicDeptInvoices; }
            set
            {
                _outwardDrugClinicDeptInvoices = value;
                RaisePropertyChanged("OutwardDrugClinicDeptInvoices");
            }
        }

        private ObservableCollection<OutwardDrugInvoice> _outwardDrugInvoices;
        [DataMemberAttribute()]
        public ObservableCollection<OutwardDrugInvoice> OutwardDrugInvoices
        {
            get { return _outwardDrugInvoices; }
            set
            {
                _outwardDrugInvoices = value;
                RaisePropertyChanged("OutwardDrugInvoices");
            }
        }

        private decimal _TotalInvoicePrice;
        [DataMemberAttribute]
        public decimal TotalInvoicePrice
        {
            get
            {
                return _TotalInvoicePrice;
            }
            set
            {
                _TotalInvoicePrice = value;
                RaisePropertyChanged("TotalInvoicePrice");
            }
        }

        private decimal _TotalPriceDifference;
        [DataMemberAttribute]
        public decimal TotalPriceDifference
        {
            get
            {
                return _TotalPriceDifference;
            }
            set
            {
                _TotalPriceDifference = value;
                RaisePropertyChanged("TotalPriceDifference");
            }
        }

        private decimal _TotalHIPayment;
        [DataMemberAttribute]
        public decimal TotalHIPayment
        {
            get
            {
                return _TotalHIPayment;
            }
            set
            {
                _TotalHIPayment = value;
                RaisePropertyChanged("TotalHIPayment");
            }
        }
        private decimal _TotalCoPayment;
        [DataMemberAttribute]
        public decimal TotalCoPayment
        {
            get
            {
                return _TotalCoPayment;
            }
            set
            {
                _TotalCoPayment = value;
                RaisePropertyChanged("TotalCoPayment");
            }
        }

        private decimal _TotalPatientPayment;
        [DataMemberAttribute]
        public decimal TotalPatientPayment
        {
            get
            {
                return _TotalPatientPayment;
            }
            set
            {
                _TotalPatientPayment = value;
                RaisePropertyChanged("TotalPatientPayment");
            }
        }

        private decimal _TotalPatientPaid;
        [DataMemberAttribute]
        public decimal TotalPatientPaid
        {
            get
            {
                return _TotalPatientPaid;
            }
            set
            {
                _TotalPatientPaid = value;
                RaisePropertyChanged("TotalPatientPaid");
            }
        }


        private decimal _TotalSupportFund;
        [DataMemberAttribute]
        public decimal TotalSupportFund
        {
            get
            {
                return _TotalSupportFund;
            }
            set
            {
                _TotalSupportFund = value;
                RaisePropertyChanged("TotalSupportFund");
            }
        }

        private byte _modifiedCount;
        [DataMemberAttribute]
        public byte ModifiedCount
        {
            get
            {
                return _modifiedCount;
            }
            set
            {
                _modifiedCount = value;
                RaisePropertyChanged("ModifiedCount");
                RaisePropertyChanged("IsAlreadyUpdated");
            }
        }

        public bool IsAlreadyUpdated
        {
            get { return ModifiedCount > 0; }
        }

        private DateTime? _paidTime;
        /// <summary>
        /// Ngay tra tien. Neu co gia tri => item nay da duoc tra tien roi.
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? PaidTime
        {
            get
            {
                return _paidTime;
            }
            set
            {
                _paidTime = value;
                RaisePropertyChanged("PaidTime");
            }
        }

        private DateTime? _refundTime;
        /// <summary>
        /// Ngay hoan tien. Neu co gia tri => item nay da duoc tra tien roi.
        /// </summary>
        [DataMemberAttribute()]
        public DateTime? RefundTime
        {
            get
            {
                return _refundTime;
            }
            set
            {
                _refundTime = value;
                RaisePropertyChanged("RefundTime");
            }
        }

        private RecordState _recordState = RecordState.DETACHED;
        [DataMemberAttribute()]
        public RecordState RecordState
        {
            get
            {
                return _recordState;
            }
            set
            {
                _recordState = value;
                RaisePropertyChanged("RecordState");
            }
        }

        [DataMemberAttribute()]
        public long? DeptID
        {
            get { return _DeptID; }
            set
            {
                if (_DeptID != value)
                {
                    _DeptID = value;
                    RaisePropertyChanged("DeptID");
                }
            }
        }
        private long? _DeptID;

        // TxD 12/05/2015: New Field DeptLocationID added to InPatientBillingInvoice mainly to serve USIC-CC department
        [DataMemberAttribute()]
        public long? DeptLocationID
        {
            get { return _DeptLocationID; }
            set
            {
                if (_DeptLocationID != value)
                {
                    _DeptLocationID = value;
                    RaisePropertyChanged("DeptLocationID");
                }
            }
        }
        private long? _DeptLocationID;


        [DataMemberAttribute()]
        public RefDepartment Department
        {
            get { return _department; }
            set
            {
                if (_department != value)
                {
                    _department = value;
                    RaisePropertyChanged("Department");
                }
            }
        }
        private RefDepartment _department;

        [DataMemberAttribute()]
        public DateTime? BillFromDate
        {
            get { return _BillFromDate; }
            set
            {
                if (_BillFromDate != value)
                {
                    _BillFromDate = value;
                    RaisePropertyChanged("BillFromDate");
                }
            }
        }
        private DateTime? _BillFromDate;


        [DataMemberAttribute()]
        public DateTime? BillToDate
        {
            get { return _BillToDate; }
            set
            {
                if (_BillToDate != value)
                {
                    _BillToDate = value;
                    RaisePropertyChanged("BillToDate");
                }
            }
        }
        private DateTime? _BillToDate;

        [DataMemberAttribute()]
        public bool IsHighTechServiceBill
        {
            get { return _IsHighTechServiceBill; }
            set
            {
                if (_IsHighTechServiceBill != value)
                {
                    _IsHighTechServiceBill = value;
                    RaisePropertyChanged("IsHighTechServiceBill");
                }
            }
        }
        private bool _IsHighTechServiceBill;

        [DataMemberAttribute()]
        public long V_TreatmentTypeBill
        {
            get { return _V_TreatmentTypeBill; }
            set
            {
                if (_V_TreatmentTypeBill != value)
                {
                    _V_TreatmentTypeBill = value;
                    RaisePropertyChanged("V_TreatmentTypeBill");
                }
            }
        }
        private long _V_TreatmentTypeBill;

        [DataMemberAttribute()]
        public decimal MaxHIPay
        {
            get { return _MaxHIPay; }
            set
            {
                if (_MaxHIPay != value)
                {
                    _MaxHIPay = value;
                    RaisePropertyChanged("MaxHIPay");
                }
            }
        }
        private decimal _MaxHIPay;

        [DataMemberAttribute()]
        public bool NotApplyMaxHIPay
        {
            get { return _NotApplyMaxHIPay; }
            set
            {
                if (_NotApplyMaxHIPay != value)
                {
                    _NotApplyMaxHIPay = value;
                    RaisePropertyChanged("NotApplyMaxHIPay");
                }
            }
        }
        private bool _NotApplyMaxHIPay;

        [DataMemberAttribute()]
        public bool IsAdditionalFee
        {
            get { return _IsAdditionalFee; }
            set
            {
                if (_IsAdditionalFee != value)
                {
                    _IsAdditionalFee = value;
                    RaisePropertyChanged("IsAdditionalFee");
                }
            }
        }
        private bool _IsAdditionalFee;

        [DataMemberAttribute()]
        public long RptPtCashAdvRemID
        {
            get { return _RptPtCashAdvRemID; }
            set
            {
                if (_RptPtCashAdvRemID != value)
                {
                    _RptPtCashAdvRemID = value;
                    RaisePropertyChanged("RptPtCashAdvRemID");
                }
            }
        }
        private long _RptPtCashAdvRemID;

        [DataMemberAttribute()]
        public bool CanEditPeriodOfTime
        {
            get { return _CanEditPeriodOfTime || InPatientBillingInvID <= 0; }
            set
            {
                if (_CanEditPeriodOfTime != value)
                {
                    _CanEditPeriodOfTime = value;
                    RaisePropertyChanged("CanEditPeriodOfTime");
                }
            }
        }
        private bool _CanEditPeriodOfTime;

        public bool BillingInvIsFinalized
        {
            get
            {
                if (PaidTime != null && TotalPatientPaid + TotalSupportFund >= TotalPatientPayment)
                    return true;
                return false;
            }
        }

        private ObservableCollection<CharitySupportFund> _SupportFunds;
        public ObservableCollection<CharitySupportFund> SupportFunds
        {
            get
            {
                return _SupportFunds;
            }
            set
            {
                _SupportFunds = value;
            }
        }

        /*==== #001 ====*/
        [DataMemberAttribute()]
        public double? HIBenefit
        {
            get { return _HIBenefit; }
            set
            {
                if (_HIBenefit != value)
                {
                    _HIBenefit = value;
                    RaisePropertyChanged("HIBenefit");
                }
            }
        }
        private double? _HIBenefit = 1;

        [DataMemberAttribute()]
        public bool? IsHICard_FiveYearsCont_NoPaid
        {
            get { return _IsHICard_FiveYearsCont_NoPaid; }
            set
            {
                if (_IsHICard_FiveYearsCont_NoPaid != value)
                {
                    _IsHICard_FiveYearsCont_NoPaid = value;
                    RaisePropertyChanged("IsHICard_FiveYearsCont_NoPaid");
                }
            }
        }
        private bool? _IsHICard_FiveYearsCont_NoPaid = false;
        /*==== #001 ====*/

        private long _OutPtRegistrationID;
        [DataMemberAttribute]
        public long OutPtRegistrationID
        {
            get
            {
                return _OutPtRegistrationID;
            }
            set
            {
                _OutPtRegistrationID = value;
                RaisePropertyChanged("OutPtRegistrationID");
            }
        }

        //▼====: #002
        private int? _IsValidForBill = 0;
        [DataMemberAttribute()]
        public int? IsValidForBill
        {
            get { return _IsValidForBill; }
            set
            {
                if (_IsValidForBill != value)
                {
                    _IsValidForBill = value;
                    RaisePropertyChanged("IsValidForBill");
                }
            }
        }

        private decimal _DiscountAmt;
        [DataMemberAttribute]
        public decimal DiscountAmt
        {
            get
            {
                return _DiscountAmt;
            }
            set
            {
                if (_DiscountAmt == value)
                {
                    return;
                }
                _DiscountAmt = value;
                RaisePropertyChanged("DiscountAmt");
            }
        }

        private long _MedServiceItemPriceListID;
        [DataMemberAttribute]
        public long MedServiceItemPriceListID
        {
            get
            {
                return _MedServiceItemPriceListID;
            }
            set
            {
                _MedServiceItemPriceListID = value;
                RaisePropertyChanged("MedServiceItemPriceListID");
            }
        }

        private long _PCLExamTypePriceListID;
        [DataMemberAttribute]
        public long PCLExamTypePriceListID
        {
            get
            {
                return _PCLExamTypePriceListID;
            }
            set
            {
                _PCLExamTypePriceListID = value;
                RaisePropertyChanged("PCLExamTypePriceListID");
            }
        }
        //▲====: #002

        private string _QuotationTitle;
        [DataMemberAttribute]
        public string QuotationTitle
        {
            get
            {
                return _QuotationTitle;
            }
            set
            {
                if (_QuotationTitle == value)
                {
                    return;
                }
                _QuotationTitle = value;
                RaisePropertyChanged("QuotationTitle");
            }
        }

        private Patient _CurrentPatient;
        [DataMemberAttribute]
        public Patient CurrentPatient
        {
            get
            {
                return _CurrentPatient;
            }
            set
            {
                _CurrentPatient = value;
                RaisePropertyChanged("CurrentPatient");
            }
        }

        //▼====: #003
        private decimal _OtherAmt;
        [DataMemberAttribute]
        public decimal OtherAmt
        {
            get
            {
                return _OtherAmt;
            }
            set
            {
                if (_OtherAmt == value)
                {
                    return;
                }
                _OtherAmt = value;
                RaisePropertyChanged("OtherAmt");
            }
        }
        //▲====: #003  
        //▼====: #004
        private bool _IsBlock;
        [DataMemberAttribute]
        public bool IsBlock
        {
            get
            {
                return _IsBlock;
            }
            set
            {
                if (_IsBlock == value)
                {
                    return;
                }
                _IsBlock = value;
                RaisePropertyChanged("IsBlock");
            }
        }
        //▲====: #004
    }
}