using System;
using System.Linq;
using System.Runtime.Serialization;
using eHCMS.Services.Core.Base;
/*
 * 20210928 #001 TNHX: 681 Thêm tích xác nhận DV thanh COVID
 * 20220727 #002 QTD:  Thêm tích tính DV
 */
namespace DataEntities
{
    /// <summary>
    /// Lớp này dùng làm base class cho các lớp như PatientRegistrationDetails,OutwarDrug...
    /// </summary>
    /// 
    [DataContract]
    [KnownType(typeof(IInvoiceItem))]
    [KnownType(typeof(PatientRegistrationDetail))]
    [KnownType(typeof(PatientPCLRequestDetail))]
    [KnownType(typeof(OutwardDrugClinicDept))]
    [KnownType(typeof(PatientPCLRequest))]
    public partial class MedRegItemBase : EntityBase, IInvoiceItem//,IEntityState
    {
        public MedRegItemBase()
        {
            Qty = 1;
            _examRegStatus = AllLookupValues.ExamRegStatus.KHONG_XAC_DINH;
        }

        #region IInvoiceItem Members
        [DataMemberAttribute]
        public virtual long ID
        {
            get;
            set;
        }
        private bool _hiApplied = true;
        [DataMemberAttribute()]
        public bool HiApplied
        {
            get { return _hiApplied; }
            set
            {
                _hiApplied = value;
                RaisePropertyChanged("HiApplied");
            }
        }
        private decimal _InvoicePrice;
        [DataMemberAttribute]
        public decimal InvoicePrice
        {
            get
            {
                return _InvoicePrice;
            }
            set
            {
                _InvoicePrice = value;
                RaisePropertyChanged("InvoicePrice");
            }
        }

        private decimal? _HIAllowedPrice;
        [DataMemberAttribute]
        public decimal? HIAllowedPrice
        {
            get
            {
                return _HIAllowedPrice;
            }
            set
            {
                _HIAllowedPrice = value;
                RaisePropertyChanged("HIAllowedPrice");
                RaisePropertyChanged("MaskedHIAllowedPrice");
            }
        }

        public virtual decimal? MaskedHIAllowedPrice
        {
            get
            {
                return HIAllowedPrice;
            }
        }

        private decimal _PriceDifference;
        [DataMemberAttribute]
        public decimal PriceDifference
        {
            get
            {
                return _PriceDifference;
            }
            set
            {
                _PriceDifference = value;
                RaisePropertyChanged("PriceDifference");
            }
        }

        private decimal _HIPayment;
        [DataMemberAttribute]
        public decimal HIPayment
        {
            get
            {
                return _HIPayment;
            }
            set
            {
                _HIPayment = value;
                RaisePropertyChanged("HIPayment");
            }
        }

        private decimal _PatientCoPayment;
        [DataMemberAttribute]
        public decimal PatientCoPayment
        {
            get
            {
                return _PatientCoPayment;
            }
            set
            {
                _PatientCoPayment = value;
                RaisePropertyChanged("PatientCoPayment");
            }
        }
        private decimal _PatientPayment;
        [DataMemberAttribute]
        public decimal PatientPayment
        {
            get
            {
                return _PatientPayment;
            }
            set
            {
                _PatientPayment = value;
                RaisePropertyChanged("PatientPayment");
            }
        }

        private decimal _Qty = 1;
        [DataMemberAttribute()]
        public virtual decimal Qty
        {
            get
            {
                return _Qty;
            }
            set
            {
                _Qty = value;
                RaisePropertyChanged("Qty");
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
                RaisePropertyChanged("MaskedHIAllowedPrice");
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
                RaisePropertyChanged("MaskedHIAllowedPrice");
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

        public virtual IChargeableItemPrice ChargeableItem
        {
            get
            {
                return null;
            }
        }

        public virtual IGenericService GenericServiceItem
        {
            get
            {
                return null;
            }
        }

        /*==== #001 ====*/
        public Int32? MaxQtyHIAllowItem
        {
            get { return ChargeableItem == null || !(ChargeableItem is RefGenMedProductDetails) ? null : ((RefGenMedProductDetails)ChargeableItem).MaxQtyHIAllowItem; }
        }
        public Double? PaymentRateOfHIAddedItem
        {
            get { return ChargeableItem == null || !(ChargeableItem is RefGenMedProductDetails) ? null : ((RefGenMedProductDetails)ChargeableItem).PaymentRateOfHIAddedItem; }
        }
        /*==== #001 ====*/

        private long? _hisID;
        [DataMemberAttribute()]
        public long? HisID
        {
            get
            {
                return _hisID;
            }
            set
            {
                _hisID = value;
            }
        }
        #endregion

        #region IDBRecordState Members
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

        private DateTime _CreatedDate;
        [DataMemberAttribute]
        public virtual DateTime CreatedDate
        {
            get
            {
                return _CreatedDate;
            }
            set
            {
                _CreatedDate = value;
                RaisePropertyChanged("CreatedDate");
            }
        }

        private AllLookupValues.MedProductType _MedProductType = AllLookupValues.MedProductType.Unknown;
        [DataMemberAttribute]
        public virtual AllLookupValues.MedProductType MedProductType
        {
            get
            {
                return _MedProductType;
            }
            set
            {
                _MedProductType = value;
                RaisePropertyChanged("MedProductType");
            }
        }
        #endregion

        //KMx: Cách lấy code này lòng vòng. Lý do: Trước đây ChargeableItemName có trước, người nào đó đã thiết kế lòng vòng nên bây giờ phải làm theo.
        //Nếu muốn sửa lại thiết kế thì mất nhiều thời gian để test, nên anh Tuấn kiu làm đỡ như vậy (30/08/2016 09:16).
        public virtual string ChargeableItemCode
        {
            get
            {
                if (GenericServiceItem != null)
                {
                    return GenericServiceItem.GetCode();
                }
                return String.Empty;
            }
        }

        public virtual string ChargeableItemName
        {
            get
            {
                if (ChargeableItem != null)
                {
                    return ChargeableItem.ToString();
                }
                return String.Empty;
            }
        }

        private long? _StaffID;
        [DataMemberAttribute]
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

        private double? _HIBenefit;
        [DataMemberAttribute]
        public double? HIBenefit
        {
            get
            {
                return _HIBenefit;
            }
            set
            {
                if (_HIBenefit != value)
                {
                    _HIBenefit = value;
                    RaisePropertyChanged("HIBenefit");
                }
            }
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

        private DateTime? _paidTimeTmp;
        [DataMemberAttribute()]
        public DateTime? PaidTimeTmp
        {
            get
            {
                return _paidTimeTmp;
            }
            set
            {
                _paidTimeTmp = value;
                RaisePropertyChanged("PaidTimeTmp");
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

        private long _PaidStaffID;
        [DataMemberAttribute()]
        public long PaidStaffID
        {
            get
            {
                return _PaidStaffID;
            }
            set
            {
                _PaidStaffID = value;
                RaisePropertyChanged("PaidStaffID");
            }
        }

        private string _PaidStaffName;
        [DataMemberAttribute()]
        public string PaidStaffName
        {
            get
            {
                return _PaidStaffName;
            }
            set
            {
                _PaidStaffName = value;
                RaisePropertyChanged("PaidStaffName");
            }
        }

        private string _CancelStaffName;
        [DataMemberAttribute()]
        public string CancelStaffName
        {
            get
            {
                return _CancelStaffName;
            }
            set
            {
                _CancelStaffName = value;
                RaisePropertyChanged("CancelStaffName");
            }
        }

        private string _SpecialNote;
        [DataMemberAttribute()]
        public string SpecialNote
        {
            get
            {
                return _SpecialNote;
            }
            set
            {
                _SpecialNote = value;
                RaisePropertyChanged("SpecialNote");
            }
        }


        [DataMemberAttribute()]
        public virtual AllLookupValues.ExamRegStatus ExamRegStatus
        {
            get
            {
                return _examRegStatus;
            }
            set
            {
                if (_examRegStatus != value)
                {
                    _examRegStatus = value;
                    RaisePropertyChanged("ExamRegStatus");
                }
            }
        }
        private AllLookupValues.ExamRegStatus _examRegStatus;


        [DataMemberAttribute()]
        public bool IsCountHI
        {
            get
            {
                return _IsCountHI;
            }
            set
            {
                _IsCountHI = value;
                RaisePropertyChanged("IsCountHI");
            }
        }
        private bool _IsCountHI = true;


        [DataMemberAttribute()]
        public bool IsCountPatient
        {
            get
            {
                return _IsCountPatient;
            }
            set
            {
                _IsCountPatient = value;
                RaisePropertyChanged("IsCountPatient");
            }
        }
        private bool _IsCountPatient = true;

        [DataMemberAttribute()]
        public bool IsCountHI_Orig
        {
            get
            {
                return _IsCountHI_Orig;
            }
            set
            {
                _IsCountHI_Orig = value;
                RaisePropertyChanged("IsCountHI_Orig");
            }
        }
        private bool _IsCountHI_Orig;

        [DataMemberAttribute()]
        public bool IsCountPatient_Orig
        {
            get
            {
                return _IsCountPatient_Orig;
            }
            set
            {
                _IsCountPatient_Orig = value;
                RaisePropertyChanged("IsCountPatient_Orig");
            }
        }
        private bool _IsCountPatient_Orig;

        [DataMemberAttribute()]
        public bool IsPackageService
        {
            get
            {
                return _IsPackageService;
            }
            set
            {
                _IsPackageService = value;
                RaisePropertyChanged("IsPackageService");
            }
        }
        private bool _IsPackageService;

        [DataMemberAttribute()]
        public bool IsInPackage
        {
            get
            {
                return _IsInPackage;
            }
            set
            {
                _IsInPackage = value;
                RaisePropertyChanged("IsInPackage");
            }
        }
        private bool _IsInPackage;

        [DataMemberAttribute()]
        public Int32 V_NewPriceType
        {
            get
            {
                return _V_NewPriceType;
            }
            set
            {
                _V_NewPriceType = value;
                RaisePropertyChanged("V_NewPriceType");
            }
        }
        private Int32 _V_NewPriceType;

        [DataMemberAttribute()]
        public string ReasonChangePrice
        {
            get
            {
                return _ReasonChangePrice;
            }
            set
            {
                _ReasonChangePrice = value;
                RaisePropertyChanged("ReasonChangePrice");
            }
        }
        private string _ReasonChangePrice;

        [DataMemberAttribute()]
        public bool IsModItemOK
        {
            get
            {
                return _IsModItemOK;
            }
            set
            {
                _IsModItemOK = value;
                RaisePropertyChanged("IsModItemOK");
            }
        }
        private bool _IsModItemOK;


        [DataMemberAttribute()]
        public Staff DoctorStaff
        {
            get
            {
                return _DoctorStaff;
            }
            set
            {
                _DoctorStaff = value;
                RaisePropertyChanged("DoctorStaff");
            }
        }
        private Staff _DoctorStaff;

        [DataMemberAttribute()]
        public Staff DoctorStaff_Orig
        {
            get
            {
                return _DoctorStaff_Orig;
            }
            set
            {
                _DoctorStaff_Orig = value;
                RaisePropertyChanged("DoctorStaff_Orig");
            }
        }
        private Staff _DoctorStaff_Orig;

        [DataMemberAttribute()]
        public DateTime? MedicalInstructionDate
        {
            get
            {
                return _MedicalInstructionDate;
            }
            set
            {
                _MedicalInstructionDate = value;
                RaisePropertyChanged("MedicalInstructionDate");
            }
        }
        private DateTime? _MedicalInstructionDate;
        [DataMemberAttribute()]
        public string MedicalInstructionDateStr
        {
            get
            {
                return _MedicalInstructionDateStr;
            }
            set
            {
                _MedicalInstructionDateStr = value;
                RaisePropertyChanged("MedicalInstructionDateStr");
            }
        }
        private string _MedicalInstructionDateStr;

        [DataMemberAttribute()]
        public DateTime? MedicalInstructionDate_Orig
        {
            get
            {
                return _MedicalInstructionDate_Orig;
            }
            set
            {
                _MedicalInstructionDate_Orig = value;
                RaisePropertyChanged("MedicalInstructionDate_Orig");
            }
        }
        private DateTime? _MedicalInstructionDate_Orig;

        [DataMemberAttribute()]
        public DateTime? ResultDate
        {
            get
            {
                return _ResultDate;
            }
            set
            {
                _ResultDate = value;
                RaisePropertyChanged("ResultDate");
            }
        }
        private DateTime? _ResultDate;


        [DataMemberAttribute]
        public decimal DiscountAmt
        {
            get => _DiscountAmt; set
            {
                _DiscountAmt = value;
                RaisePropertyChanged("DiscountAmt");
                RaisePropertyChanged("IsDiscounted");
            }
        }
        private decimal _DiscountAmt;

        public bool IsDiscounted
        {
            get
            {
                return DiscountAmt > 0;
            }
        }

        public decimal TotalDiscountAmount
        {
            get
            {
                if (this != null && this is PatientPCLRequest)
                {
                    return (this as PatientPCLRequest).PatientPCLRequestIndicators == null ? 0 : (this as PatientPCLRequest).PatientPCLRequestIndicators.Where(x => !x.MarkedAsDeleted && x.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI).Sum(x => x.DiscountAmt);
                }
                return this.DiscountAmt;
            }
        }

        private long? _PromoDiscProgID;
        private bool _IsDiscountChecked;
        [DataMemberAttribute]
        public long? PromoDiscProgID
        {
            get => _PromoDiscProgID; set
            {
                _PromoDiscProgID = value;
                RaisePropertyChanged("PromoDiscProgID");
            }
        }
        [DataMemberAttribute]
        public bool IsDiscountChecked
        {
            get
            {
                return _IsDiscountChecked;
            }
            set
            {
                _IsDiscountChecked = value;
                RaisePropertyChanged("IsDiscountChecked");
            }
        }

        private long? _IntPtDiagDrInstructionID;
        [DataMemberAttribute]
        public long? IntPtDiagDrInstructionID
        {
            get
            {
                return _IntPtDiagDrInstructionID;
            }
            set
            {
                if (_IntPtDiagDrInstructionID == value)
                {
                    return;
                }
                _IntPtDiagDrInstructionID = value;
                RaisePropertyChanged("IntPtDiagDrInstructionID");
            }
        }


        private int _CountValue;
        [DataMemberAttribute]
        public int CountValue
        {
            get
            {
                return _CountValue;
            }
            set
            {
                if (_CountValue != value)
                {
                    _CountValue = value;
                    RaisePropertyChanged("CountValue");
                }
            }
        }

        //20191217 TTM: Set 100 mặc định vì lý do tick time đc dùng để order mà nếu để mặc định là 0 hoặc null thì sẽ order sai.
        private int? _TickTime = 100;
        [DataMemberAttribute]
        public int? TickTime
        {
            get
            {
                return _TickTime;
            }
            set
            {
                if (_TickTime != value)
                {
                    _TickTime = value;
                    RaisePropertyChanged("TickTime");
                }
            }
        }
        public long DisplayID { get; set; }

        public bool SameAsMedItemPrimary(MedRegItemBase CompareObject)
        {
            if (CompareObject is PatientRegistrationDetail && this is PatientRegistrationDetail)
            {
                return (CompareObject as PatientRegistrationDetail).MedServiceID == (this as PatientRegistrationDetail).MedServiceID;
            }
            else if (CompareObject is PatientPCLRequestDetail && this is PatientPCLRequestDetail)
            {
                return (CompareObject as PatientPCLRequestDetail).PCLExamTypeID == (this as PatientPCLRequestDetail).PCLExamTypeID;
            }
            return false;
        }

        //▼====: #001
        [DataMemberAttribute()]
        public bool IsCountPatientCOVID
        {
            get
            {
                return _IsCountPatientCOVID;
            }
            set
            {
                _IsCountPatientCOVID = value;
                RaisePropertyChanged("IsCountPatientCOVID");
            }
        }
        private bool _IsCountPatientCOVID = false;

        [DataMemberAttribute()]
        public decimal OtherAmt
        {
            get
            {
                return _OtherAmt;
            }
            set
            {
                if (_OtherAmt != value)
                {
                    _OtherAmt = value;
                    RaisePropertyChanged("OtherAmt");
                }
            }
        }
        private decimal _OtherAmt;
        //▲====: #001

        //▼====: #002
        [DataMemberAttribute()]
        public bool IsCountSE
        {
            get
            {
                return _IsCountSE;
            }
            set
            {
                _IsCountSE = value;
                RaisePropertyChanged("IsCountSE");
            }
        }
        private bool _IsCountSE = false;
        //▲====: #002
    }
}