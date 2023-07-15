using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using eHCMS.Configurations;

namespace DataEntities
{
    [DataContract(IsReference = true)]
    public partial class OutwardDrugClinicDeptInvoice : NotifyChangedBase, IInvoiceItem
    {
        public OutwardDrugClinicDeptInvoice()
            : base()
        {
            _MedProductType = AllLookupValues.MedProductType.Unknown;
            ExamRegStatus = AllLookupValues.ExamRegStatus.KHONG_XAC_DINH;
        }
        #region Factory Method


        /// Create a new OutwardDrugClinicDeptInvoice object.

        /// <param name="outiID">Initial value of the outiID property.</param>
        /// <param name="outInvID">Initial value of the OutInvID property.</param>
        public static OutwardDrugClinicDeptInvoice CreateOutwardDrugClinicDeptInvoice(Int64 outiID, String outInvID)
        {
            OutwardDrugClinicDeptInvoice outwardDrugClinicDeptInvoice = new OutwardDrugClinicDeptInvoice();
            outwardDrugClinicDeptInvoice.outiID = outiID;
            outwardDrugClinicDeptInvoice.OutInvID = outInvID;
            return outwardDrugClinicDeptInvoice;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 outiID
        {
            get
            {
                return _outiID;
            }
            set
            {
                if (_outiID != value)
                {
                    OnoutiIDChanging(value);
                    _outiID = value;
                    RaisePropertyChanged("outiID");
                    RaisePropertyChanged("CanSaveAndPaid");
                    RaisePropertyChanged("CanPrint");
                    RaisePropertyChanged("CanUpdate");
                    RaisePropertyChanged("CanSave");
                    OnoutiIDChanged();
                }
            }
        }
        private Int64 _outiID;
        partial void OnoutiIDChanging(Int64 value);
        partial void OnoutiIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                OnPtRegistrationIDChanging(value);
                _PtRegistrationID = value;
                RaisePropertyChanged("PtRegistrationID");
                OnPtRegistrationIDChanged();
            }
        }
        private Nullable<Int64> _PtRegistrationID;
        partial void OnPtRegistrationIDChanging(Nullable<Int64> value);
        partial void OnPtRegistrationIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> IMEID
        {
            get
            {
                return _IMEID;
            }
            set
            {
                OnIMEIDChanging(value);
                _IMEID = value;
                RaisePropertyChanged("IMEID");
                OnIMEIDChanged();
            }
        }
        private Nullable<Int64> _IMEID;
        partial void OnIMEIDChanging(Nullable<Int64> value);
        partial void OnIMEIDChanged();

        [DataMemberAttribute()]
        public Nullable<Decimal> HITTypeID
        {
            get
            {
                return _HITTypeID;
            }
            set
            {
                OnHITTypeIDChanging(value);
                _HITTypeID = value;
                RaisePropertyChanged("HITTypeID");
                OnHITTypeIDChanged();
            }
        }
        private Nullable<Decimal> _HITTypeID;
        partial void OnHITTypeIDChanging(Nullable<Decimal> value);
        partial void OnHITTypeIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> StoreID
        {
            get
            {
                return _StoreID;
            }
            set
            {
                OnStoreIDChanging(value);
                _StoreID = value;
                RaisePropertyChanged("StoreID");
                OnStoreIDChanged();
            }
        }
        private Nullable<Int64> _StoreID;
        partial void OnStoreIDChanging(Nullable<Int64> value);
        partial void OnStoreIDChanged();


        [DataMemberAttribute()]
        public String CustomerName
        {
            get
            {
                return _CustomerName;
            }
            set
            {
                OnCustomerNameChanging(value);
                _CustomerName = value;
                RaisePropertyChanged("CustomerName");
                OnCustomerNameChanged();
            }
        }
        private String _CustomerName;
        partial void OnCustomerNameChanging(String value);
        partial void OnCustomerNameChanged();

        [RegularExpression("^[0-9]{9,12}$", ErrorMessage = "Số điện thoại không hợp lệ!")]
        [DataMemberAttribute()]
        public String PhoneNumber
        {
            get
            {
                return _PhoneNumber;
            }
            set
            {
                OnPhoneNumberChanging(value);
                ValidateProperty("PhoneNumber", value);
                _PhoneNumber = value;
                RaisePropertyChanged("PhoneNumber");
                OnPhoneNumberChanged();
            }
        }
        private String _PhoneNumber;
        partial void OnPhoneNumberChanging(String value);
        partial void OnPhoneNumberChanged();

        [DataMemberAttribute()]
        public String Address
        {
            get
            {
                return _Address;
            }
            set
            {
                OnAddressChanging(value);
                _Address = value;
                RaisePropertyChanged("Address");
                OnAddressChanged();
            }
        }
        private String _Address;
        partial void OnAddressChanging(String value);
        partial void OnAddressChanged();


        [DataMemberAttribute()]
        public Nullable<Int64> MSCID
        {
            get
            {
                return _MSCID;
            }
            set
            {
                OnMSCIDChanging(value);
                _MSCID = value;
                RaisePropertyChanged("MSCID");
                OnMSCIDChanged();
            }
        }
        private Nullable<Int64> _MSCID;
        partial void OnMSCIDChanging(Nullable<Int64> value);
        partial void OnMSCIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                OnStaffIDChanging(value);
                _StaffID = value;
                RaisePropertyChanged("StaffID");
                OnStaffIDChanged();
            }
        }
        private Nullable<Int64> _StaffID;
        partial void OnStaffIDChanging(Nullable<Int64> value);
        partial void OnStaffIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> TypID
        {
            get
            {
                return _TypID;
            }
            set
            {
                OnTypIDChanging(value);
                _TypID = value;
                RaisePropertyChanged("TypID");
                OnTypIDChanged();
            }
        }
        private Nullable<Int64> _TypID;
        partial void OnTypIDChanging(Nullable<Int64> value);
        partial void OnTypIDChanged();

        [DataMemberAttribute()]
        public String OutInvID
        {
            get
            {
                return _OutInvID;
            }
            set
            {
                OnOutInvIDChanging(value);
                _OutInvID = value;
                RaisePropertyChanged("OutInvID");
                OnOutInvIDChanged();
            }
        }
        private String _OutInvID;
        partial void OnOutInvIDChanging(String value);
        partial void OnOutInvIDChanged();

        [DataMemberAttribute()]
        public String StaffName
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
        private String _StaffName;


        [DataMemberAttribute()]
        public String OutInvoiceNumber
        {
            get
            {
                return _OutInvoiceNumber;
            }
            set
            {
                OnOutInvoiceNumberChanging(value);
                _OutInvoiceNumber = value;
                RaisePropertyChanged("OutInvoiceNumber");
                OnOutInvoiceNumberChanged();
            }
        }
        private String _OutInvoiceNumber;
        partial void OnOutInvoiceNumberChanging(String value);
        partial void OnOutInvoiceNumberChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> OutDate
        {
            get
            {
                return _OutDate;
            }
            set
            {
                OnOutDateChanging(value);
                _OutDate = value;
                RaisePropertyChanged("OutDate");
                OnOutDateChanged();
                //De cho tuong thich:
                _createdDate = _OutDate.GetValueOrDefault(DateTime.MinValue);
                RaisePropertyChanged("CreatedDate");
            }
        }
        private Nullable<DateTime> _OutDate;
        partial void OnOutDateChanging(Nullable<DateTime> value);
        partial void OnOutDateChanged();

        [DataMemberAttribute()]
        public Nullable<Boolean> IsCommitted
        {
            get
            {
                return _IsCommitted;
            }
            set
            {
                OnIsCommittedChanging(value);
                _IsCommitted = value;
                RaisePropertyChanged("IsCommitted");
                OnIsCommittedChanged();
            }
        }
        private Nullable<Boolean> _IsCommitted;
        partial void OnIsCommittedChanging(Nullable<Boolean> value);
        partial void OnIsCommittedChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> ReturnID
        {
            get
            {
                return _ReturnID;
            }
            set
            {
                OnReturnIDChanging(value);
                _ReturnID = value;
                RaisePropertyChanged("ReturnID");
                OnReturnIDChanged();
            }
        }
        private Nullable<Int64> _ReturnID;
        partial void OnReturnIDChanging(Nullable<Int64> value);
        partial void OnReturnIDChanged();

        [DataMemberAttribute()]
        public String Notes
        {
            get
            {
                return _Notes;
            }
            set
            {
                _Notes = value;
                RaisePropertyChanged("Notes");
            }
        }
        private String _Notes;

        [DataMemberAttribute()]
        public Int64? OutputToID
        {
            get
            {
                return _OutputToID;
            }
            set
            {
                _OutputToID = value;
                RaisePropertyChanged("OutputToID");
            }
        }
        private Int64? _OutputToID;

        [DataMemberAttribute()]
        public string OutputToIDName
        {
            get
            {
                return _OutputToIDName;
            }
            set
            {
                _OutputToIDName = value;
                RaisePropertyChanged("OutputToIDName");
            }
        }
        private string _OutputToIDName;

        [DataMemberAttribute()]
        public Int64 V_OutputTo
        {
            get
            {
                return _V_OutputTo;
            }
            set
            {
                _V_OutputTo = value;
                RaisePropertyChanged("V_OutputTo");
            }
        }
        private Int64 _V_OutputTo;

        [DataMemberAttribute()]
        public Int64 V_OutDrugInvStatus
        {
            get
            {
                return _V_OutDrugInvStatus;
            }
            set
            {
                //ma phieu xuat ban dau
                _V_OutDrugInvStatus = value;
                RaisePropertyChanged("V_OutDrugInvStatus");
                RaisePropertyChanged("CanSaveAndPaid");

            }
        }
        private Int64 _V_OutDrugInvStatus;

        [DataMemberAttribute()]
        public string OutDrugInvStatus
        {
            get
            {
                return _OutDrugInvStatus;
            }
            set
            {
                //ma phieu xuat ban dau
                _OutDrugInvStatus = value;
                RaisePropertyChanged("OutDrugInvStatus");

            }
        }
        private string _OutDrugInvStatus = "Hoàn toàn mới";

        //--▼--30/12/2020 DatTB
        [DataMemberAttribute()]
        public long RefGenDrugCatID_1
        {
            get
            {
                return _RefGenDrugCatID_1;
            }
            set
            {
                if (_RefGenDrugCatID_1 != value)
                {
                    OnRefGenDrugCatID_1Changing(value);
                    _RefGenDrugCatID_1 = value;
                    RaisePropertyChanged("RefGenDrugCatID_1");
                    OnRefGenDrugCatID_1Changed();
                }
            }
        }
        private long _RefGenDrugCatID_1;
        partial void OnRefGenDrugCatID_1Changing(long value);
        partial void OnRefGenDrugCatID_1Changed();
        //--▲--30/12/2020 DatTB

        #endregion

        #region Navigation Properties
        [DataMemberAttribute()]
        public IHTransactionType IHTransactionType
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public IncurredMedicalExpens IncurredMedicalExpens
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public MedicalSurgicalConsumable MedicalSurgicalConsumable
        {
            get;
            set;
        }

        private ObservableCollection<OutwardDrugClinicDept> _OutwardDrugClinicDepts;
        [DataMemberAttribute()]
        public ObservableCollection<OutwardDrugClinicDept> OutwardDrugClinicDepts
        {
            get
            {
                return _OutwardDrugClinicDepts;
            }
            set
            {
                _OutwardDrugClinicDepts = value;
                RaisePropertyChanged("OutwardDrugClinicDepts");
            }
        }

        private ObservableCollection<OutwardDrugClinicDept> _OutwardDrugClinicDepts_Add;
        [DataMemberAttribute()]
        public ObservableCollection<OutwardDrugClinicDept> OutwardDrugClinicDepts_Add
        {
            get
            {
                return _OutwardDrugClinicDepts_Add;
            }
            set
            {
                _OutwardDrugClinicDepts_Add = value;
                RaisePropertyChanged("OutwardDrugClinicDepts_Add");
            }
        }

        private ObservableCollection<OutwardDrugClinicDept> _OutwardDrugClinicDepts_Update;
        [DataMemberAttribute()]
        public ObservableCollection<OutwardDrugClinicDept> OutwardDrugClinicDepts_Update
        {
            get
            {
                return _OutwardDrugClinicDepts_Update;
            }
            set
            {
                _OutwardDrugClinicDepts_Update = value;
                RaisePropertyChanged("OutwardDrugClinicDepts_Update");
            }
        }

        private ObservableCollection<OutwardDrugClinicDept> _OutwardDrugClinicDepts_Delete;
        [DataMemberAttribute()]
        public ObservableCollection<OutwardDrugClinicDept> OutwardDrugClinicDepts_Delete
        {
            get
            {
                return _OutwardDrugClinicDepts_Delete;
            }
            set
            {
                _OutwardDrugClinicDepts_Delete = value;
                RaisePropertyChanged("OutwardDrugClinicDepts_Delete");
            }
        }

        private ObservableCollection<RequestDrugInwardClinicDept> _RequestDrugInwardClinicDepts;
        [DataMemberAttribute()]
        public ObservableCollection<RequestDrugInwardClinicDept> RequestDrugInwardClinicDepts
        {
            get
            {
                return _RequestDrugInwardClinicDepts;
            }
            set
            {
                _RequestDrugInwardClinicDepts = value;
                RaisePropertyChanged("RequestDrugInwardClinicDepts");
            }
        }

        [DataMemberAttribute()]
        public PatientRegistration PatientRegistration
        {
            get
            {
                return _PatientRegistration;
            }
            set
            {
                _PatientRegistration = value;
                RaisePropertyChanged("PatientRegistration");
                RaisePropertyChanged("CanSave");
            }
        }
        private PatientRegistration _PatientRegistration;

        [DataMemberAttribute()]
        public RefOutputType RefOutputType
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public RefStorageWarehouseLocation SelectedStorage
        {
            get
            {
                return _SelectedStorage;
            }
            set
            {
                OnSelectedStorageChanging(value);
                _SelectedStorage = value;
                RaisePropertyChanged("SelectedStorage");
                OnSelectedStorageChanged();
            }
        }
        private RefStorageWarehouseLocation _SelectedStorage;
        partial void OnSelectedStorageChanging(RefStorageWarehouseLocation value);
        partial void OnSelectedStorageChanged();


        [DataMemberAttribute()]
        public Staff SelectedStaff
        {
            get
            {
                return _selectedStaff;
            }
            set
            {
                OnSelectedStaffChanging(value);
                _selectedStaff = value;
                RaisePropertyChanged("SelectedStaff");
                OnSelectedStaffChanged();
            }
        }
        private Staff _selectedStaff;
        partial void OnSelectedStaffChanging(Staff value);
        partial void OnSelectedStaffChanged();

        #endregion

        private long? _IssueID;
        [DataMemberAttribute()]
        public long? IssueID
        {
            get
            {
                return _IssueID;
            }
            set
            {
                _IssueID = value;
                RaisePropertyChanged("IssueID");
            }
        }
        private AllLookupValues.MedProductType _MedProductType;
        [DataMemberAttribute()]
        public AllLookupValues.MedProductType MedProductType
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

        private bool? _confirmed;
        [DataMemberAttribute()]
        public bool? Confirmed
        {
            get { return _confirmed; }
            set
            {
                _confirmed = value;
                RaisePropertyChanged("Confirmed");
            }
        }
        public string ConvertDetailsListToXml()
        {
            return ConvertDetailsListToXml(_OutwardDrugClinicDepts);
        }

        public string ConvertDetailsListToXml(IEnumerable<OutwardDrugClinicDept> items)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<OutwardDrugClinicDepts>");
            if (items != null)
            {
                foreach (OutwardDrugClinicDept details in items)
                {
                    sb.Append("<OutwardDrugClinicDept>");
                    sb.AppendFormat("<GenMedProductID>{0}</GenMedProductID><InID>{1}</InID><HIBenefit>{2}</HIBenefit><OutQuantity>{3}</OutQuantity>",
                        details.GenMedProductItem.GenMedProductID, details.InID, details.HIBenefit, details.Qty);

                    sb.AppendFormat("<OutPrice>{0}</OutPrice><OutNotes>{1}</OutNotes><OutAmount>{2}</OutAmount>", details.InvoicePrice, details.OutNotes, details.TotalInvoicePrice);

                    sb.AppendFormat("<OutPriceDifference>{0}</OutPriceDifference><OutAmountCoPay>{1}</OutAmountCoPay><OutHIRebate>{2}</OutHIRebate>",
                        details.TotalPriceDifference, details.TotalCoPayment, details.TotalHIPayment);

                    sb.AppendFormat("<Qty>{0}</Qty><HIAllowedPrice>{1}</HIAllowedPrice><QtyReturn>{2}</QtyReturn>", details.Qty, details.HIAllowedPrice, details.QtyReturn);

                    sb.Append("</OutwardDrugClinicDept>");
                }
            }
            sb.Append("</OutwardDrugClinicDepts>");
            return sb.ToString();
        }


        public string ConvertDetailsListToXmlNy()
        {
            return ConvertDetailsListToXmlNy(_OutwardDrugClinicDepts);
        }


        public string ConvertDetailsListToXmlNy(IEnumerable<OutwardDrugClinicDept> items)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<OutwardDrugClinicDepts>");
            if (items != null)
            {
                foreach (OutwardDrugClinicDept details in items)
                {
                    sb.Append("<OutwardDrugClinicDept>");
                    sb.AppendFormat("<OutID>{0}</OutID><GenMedProductID>{1}</GenMedProductID><InID>{2}</InID><HIBenefit>{3}</HIBenefit><OutQuantity>{4}</OutQuantity>",
                        details.OutID, details.GenMedProductItem.GenMedProductID, details.InID, details.HIBenefit, details.OutQuantity);

                    sb.AppendFormat("<OutPrice>{0}</OutPrice><OutNotes>{1}</OutNotes><OutAmount>{2}</OutAmount>", details.InvoicePrice, details.OutNotes, details.TotalInvoicePrice);

                    sb.AppendFormat("<OutPriceDifference>{0}</OutPriceDifference><OutAmountCoPay>{1}</OutAmountCoPay><OutHIRebate>{2}</OutHIRebate>",
                        details.TotalPriceDifference, details.TotalCoPayment, details.OutHIRebate > 0 ? details.OutHIRebate : details.TotalHIPayment);

                    sb.AppendFormat("<Qty>{0}</Qty><HIAllowedPrice>{1}</HIAllowedPrice><QtyReturn>{2}</QtyReturn><OutClinicDeptReqID>{3}</OutClinicDeptReqID>", details.RequestQty, details.HIAllowedPrice, details.QtyReturn, details.OutClinicDeptReqID);

                    sb.AppendFormat("<IsCountHI>{0}</IsCountHI><IsCountPatient>{1}</IsCountPatient><V_MedicalMaterial>{2}</V_MedicalMaterial>", details.IsCountHI, details.IsCountPatient, details.V_MedicalMaterial);

                    sb.AppendFormat("<HIPaymentPercent>{0}</HIPaymentPercent>", details.HIPaymentPercent);

                    sb.AppendFormat("<MDose>{0}</MDose>", details.MDose);
                    sb.AppendFormat("<ADose>{0}</ADose>", details.ADose);
                    sb.AppendFormat("<EDose>{0}</EDose>", details.EDose);
                    sb.AppendFormat("<NDose>{0}</NDose>", details.NDose);

                    sb.AppendFormat("<MDoseStr>{0}</MDoseStr>", details.MDoseStr);
                    sb.AppendFormat("<ADoseStr>{0}</ADoseStr>", details.ADoseStr);
                    sb.AppendFormat("<EDoseStr>{0}</EDoseStr>", details.EDoseStr);
                    sb.AppendFormat("<NDoseStr>{0}</NDoseStr>", details.NDoseStr);
                    sb.AppendFormat("<Administration>{0}</Administration>", details.Administration == null ? null : Globals.GetSafeXMLString(details.Administration));

                    sb.AppendFormat("<DoctorStaffID>{0}</DoctorStaffID>", details.DoctorStaff != null ? details.DoctorStaff.StaffID : 0);
                    sb.AppendFormat("<MedicalInstructionDate>{0}</MedicalInstructionDate>", details.MedicalInstructionDate.HasValue && details.MedicalInstructionDate != null ? details.MedicalInstructionDate.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : null);
                    sb.AppendFormat("<DrugDeptInIDOrig>{0}</DrugDeptInIDOrig>", details.DrugDeptInIDOrig);
                    sb.AppendFormat("<VAT>{0}</VAT>", details.VAT);
                    sb.Append("</OutwardDrugClinicDept>");
                }
            }
            sb.Append("</OutwardDrugClinicDepts>");
            return sb.ToString();
        }

        public string ConvertDetailsListToXml_Add()
        {
            return ConvertDetailsListToXmlNy(_OutwardDrugClinicDepts_Add);
        }

        public string ConvertDetailsListToXml_Update()
        {
            return ConvertDetailsListToXmlNy(_OutwardDrugClinicDepts_Update);
        }

        public string ConvertDetailsListToXml_Delete()
        {
            return ConvertDetailsListToXmlNy(_OutwardDrugClinicDepts_Delete);
        }

        public string ConvertDetailsListRequestToXml()
        {
            return ConvertDetailsListRequestToXml(_RequestDrugInwardClinicDepts);
        }

        public string ConvertDetailsListRequestToXml(IEnumerable<RequestDrugInwardClinicDept> items)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<RequestDrugInwardClinicDepts>");
            if (items != null)
            {
                foreach (RequestDrugInwardClinicDept details in items)
                {
                    sb.Append("<RequestDrugInwardClinicDept>");
                    sb.AppendFormat("<ReqDrugInClinicDeptID>{0}</ReqDrugInClinicDeptID>",
                        details.ReqDrugInClinicDeptID);
                    sb.Append("</RequestDrugInwardClinicDept>");
                }
            }
            sb.Append("</RequestDrugInwardClinicDepts>");
            return sb.ToString();
        }


        public override bool Equals(object obj)
        {
            OutwardDrugClinicDeptInvoice info = obj as OutwardDrugClinicDeptInvoice;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.outiID > 0 && this.outiID == info.outiID && this.ReturnID == info.ReturnID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        private bool _IsCountHI = true;
        [DataMemberAttribute()]
        public bool IsCountHI
        {
            get { return _IsCountHI; }
            set
            {
                _IsCountHI = value;
                RaisePropertyChanged("IsCountHI");
            }
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
        public long ID { get; set; }

        public decimal InvoicePrice
        {
            get;
            set;
        }

        public decimal? HIAllowedPrice
        {
            get;
            set;
        }
        public decimal? MaskedHIAllowedPrice
        {
            get
            {
                return HIAllowedPrice;
            }
        }

        public decimal PriceDifference
        {
            get;
            set;
        }

        public decimal HIPayment
        {
            get;
            set;
        }

        public decimal PatientCoPayment
        {
            get;
            set;
        }

        public decimal PatientPayment
        {
            get;
            set;
        }

        public decimal Qty
        {
            get;
            set;
        }

        public IChargeableItemPrice ChargeableItem
        { get; set; }

        public double? HIBenefit
        {
            get;
            set;
        }

        public long? HisID
        {
            get;
            set;
        }

        private DateTime? _paidTime;
        [DataMemberAttribute]
        public DateTime? PaidTime
        {
            get { return _paidTime; }
            set
            {
                _paidTime = value;
                RaisePropertyChanged("PaidTime");
                RaisePropertyChanged("CanUpdate");
            }
        }

        public DateTime? _refundTime;
        public DateTime? RefundTime
        {
            get { return _refundTime; }
            set
            {
                _refundTime = value;
                RaisePropertyChanged("RefundTime");
                RaisePropertyChanged("CanUpdate");
            }
        }
        private DateTime _createdDate;
        [DataMemberAttribute]
        public DateTime CreatedDate
        {
            get
            {
                return _createdDate;
            }
            set
            {
                _createdDate = value;
                RaisePropertyChanged("CreatedDate");
                _OutDate = _createdDate;
                RaisePropertyChanged("OutDate");
            }
        }

        //KMx: Trong phương thức set của OutDate và CreatedDate có thay đổi giá trị lẫn nhau, nên không dùng CreatedDate mà tạo ra RecCreatedDate dùng độc lập (15/06/2015 10:05).
        private DateTime? _recCreatedDate;
        [DataMemberAttribute]
        public DateTime? RecCreatedDate
        {
            get
            {
                return _recCreatedDate;
            }
            set
            {
                if (_recCreatedDate != value)
                {
                    _recCreatedDate = value;
                    RaisePropertyChanged("RecCreatedDate");
                }
            }
        }
        public AllLookupValues.ExamRegStatus ExamRegStatus
        {
            get;
            set;
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
            }
        }

        public decimal TotalInvoicePrice
        {
            get;
            set;
        }

        public decimal TotalPriceDifference
        {
            get;
            set;
        }

        public decimal TotalHIPayment
        {
            get;
            set;
        }

        public decimal TotalCoPayment
        {
            get;
            set;
        }

        public decimal TotalPatientPayment
        {
            get;
            set;
        }
        private long? _inPatientBillingInvID;
        [DataMemberAttribute()]
        public long? InPatientBillingInvID
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
                }
            }
        }

        private string _BillingInvNum;
        [DataMemberAttribute()]
        public string BillingInvNum
        {
            get
            {
                return _BillingInvNum;
            }
            set
            {
                if (_BillingInvNum != value)
                {
                    _BillingInvNum = value;
                    RaisePropertyChanged("BillingInvNum");
                }
            }
        }

        public bool CanSaveAndPaid
        {
            get { return CanSave || CanUpdate; }
        }

        public bool CanSave
        {
            get { return (outiID == 0 && (PatientRegistration != null ? !PatientRegistration.IsDischarge : true)); }
        }

        public bool CanUpdate
        {
            //KMx: Thêm ĐK nếu phiếu đó đã tạo bill thì không được cập nhật (20/08/2014 15:06)
            get
            {
                return (outiID > 0 && PaidTime == null && RefundTime == null && !CheckedPoint && InPatientBillingInvID.GetValueOrDefault(0) <= 0);
            }
        }

        public bool CanPrint
        {
            get { return (outiID > 0); }
        }


        [DataMemberAttribute()]
        public Nullable<Int64> ReqDrugInClinicDeptID
        {
            get
            {
                return _ReqDrugInClinicDeptID;
            }
            set
            {
                OnReqDrugInClinicDeptIDChanging(value);
                _ReqDrugInClinicDeptID = value;
                RaisePropertyChanged("ReqDrugInClinicDeptID");
                OnReqDrugInClinicDeptIDChanged();
            }
        }
        private Nullable<Int64> _ReqDrugInClinicDeptID;
        partial void OnReqDrugInClinicDeptIDChanging(Nullable<Int64> value);
        partial void OnReqDrugInClinicDeptIDChanged();

        [DataMemberAttribute()]
        public String ReqNumCode
        {
            get
            {
                return _ReqNumCode;
            }
            set
            {
                _ReqNumCode = value;
                RaisePropertyChanged("ReqNumCode");
            }
        }
        private String _ReqNumCode;


        [DataMemberAttribute()]
        public bool CheckedPoint
        {
            get
            {
                return _CheckedPoint;
            }
            set
            {
                _CheckedPoint = value;
                RaisePropertyChanged("CheckedPoint");
            }
        }
        private bool _CheckedPoint = false;

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

        private long _V_RegistrationType;
        public long V_RegistrationType
        {
            get => _V_RegistrationType; set
            {
                _V_RegistrationType = value;
                RaisePropertyChanged("V_RegistrationType");
            }
        }

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

        private InPatientInstruction[] _FromIntPtDiagDrInstructionIDCollection;
        [DataMemberAttribute]
        public InPatientInstruction[] FromIntPtDiagDrInstructionIDCollection
        {
            get
            {
                return _FromIntPtDiagDrInstructionIDCollection;
            }
            set
            {
                if (_FromIntPtDiagDrInstructionIDCollection == value)
                {
                    return;
                }
                _FromIntPtDiagDrInstructionIDCollection = value;
                RaisePropertyChanged("FromIntPtDiagDrInstructionIDCollection");
                RaisePropertyChanged("IsFromInstruction");
            }
        }

        public bool IsFromInstruction
        {
            get
            {
                return FromIntPtDiagDrInstructionIDCollection != null && FromIntPtDiagDrInstructionIDCollection.Length > 0;
            }
        }
        [DataMemberAttribute()]
        public DateTime DSPTModifiedDate
        {
            get
            {
                return _DSPTModifiedDate;
            }
            set
            {
                _DSPTModifiedDate = value;
                RaisePropertyChanged("DSPTModifiedDate");
            }
        }
        private DateTime _DSPTModifiedDate;
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
    }
}