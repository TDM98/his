/*
 * 20170517 #001 CMN: Thêm thuộc tính nhận biết toa bảo hiểm hay toa không bảo hiểm
 * 20230223 #002 QTD: Thêm thuộc tính đánh dấu phiếu xuất đã đẩy cổng
*/
using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Collections.Generic;

namespace DataEntities
{
    public partial class OutwardDrugInvoice : NotifyChangedBase, IInvoiceItem
    {
        #region Factory Method

        /// Create a new OutwardDrugInvoice object.
        /// <param name="outiID">Initial value of the outiID property.</param>
        /// <param name="outInvID">Initial value of the OutInvID property.</param>
        public static OutwardDrugInvoice CreateOutwardDrugInvoice(long outiID, String outInvID, DateTime Outdate)
        {
            OutwardDrugInvoice outwardDrugInvoice = new OutwardDrugInvoice();
            outwardDrugInvoice.outiID = outiID;
            outwardDrugInvoice.OutInvID = outInvID;
            outwardDrugInvoice.OutDate = Outdate;
            return outwardDrugInvoice;
        }

        #endregion

        #region Primitive Properties
        [DataMemberAttribute()]
        public long outiID
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
                    RaisePropertyChanged("CanGetMoney");
                    RaisePropertyChanged("CanSaveAndPaid");
                    RaisePropertyChanged("CanSaveAndPaidPrescript");
                    RaisePropertyChanged("CanPrint");
                    RaisePropertyChanged("CanPrint38");
                    RaisePropertyChanged("CanDelete");
                    RaisePropertyChanged("CanEditPrescription");
                    RaisePropertyChanged("CanEditPayed");
                    RaisePropertyChanged("CanCancel");
                    RaisePropertyChanged("RefundMoney");
                    RaisePropertyChanged("CanUpdate");
                    RaiseErrorsChanged("CanEnabeNoteReturn");
                    OnoutiIDChanged();

                    RaisePropertyChanged("CanNew");
                    RaisePropertyChanged("OutDrugInvStatus");
                }
            }
        }
        private long _outiID;
        partial void OnoutiIDChanging(long value);
        partial void OnoutiIDChanged();

        [DataMemberAttribute()]
        public long ModFromOutiID
        {
            get
            {
                return _ModFromOutiID;
            }
            set
            {
                _ModFromOutiID = value;
                RaisePropertyChanged("ModFromOutiID");
            }
        }
        private long _ModFromOutiID;



        [DataMemberAttribute()]
        public Nullable<long> PrescriptID
        {
            get
            {
                return _PrescriptID;
            }
            set
            {
                OnPrescriptIDChanging(value);
                _PrescriptID = value;
                RaisePropertyChanged("PrescriptID");
                RaisePropertyChanged("CanSaveAndPaidPrescript");
                OnPrescriptIDChanged();
            }
        }
        private Nullable<long> _PrescriptID;
        partial void OnPrescriptIDChanging(Nullable<long> value);
        partial void OnPrescriptIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> IMEID
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
        private Nullable<long> _IMEID;
        partial void OnIMEIDChanging(Nullable<long> value);
        partial void OnIMEIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> HITTypeID
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
        private Nullable<long> _HITTypeID;
        partial void OnHITTypeIDChanging(Nullable<long> value);
        partial void OnHITTypeIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> StoreID
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
        private Nullable<long> _StoreID;
        partial void OnStoreIDChanging(Nullable<long> value);
        partial void OnStoreIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> MSCID
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
        private Nullable<long> _MSCID;
        partial void OnMSCIDChanging(Nullable<long> value);
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
                if (_StaffID != value)
                {
                    OnStaffIDChanging(value);
                    _StaffID = value;
                    if (_StaffID != null && StaffID > 0)
                    {
                        PaidStaffID = _StaffID.Value;
                    }
                    RaisePropertyChanged("StaffID");
                    OnStaffIDChanged();
                }
            }
        }
        private Nullable<Int64> _StaffID;
        partial void OnStaffIDChanging(Nullable<Int64> value);
        partial void OnStaffIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> TypID
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
                //RaisePropertyChanged("IsInternal_Sell");
                //RaisePropertyChanged("IsInternal_Loan");
                RaisePropertyChanged("CanReturnXNB");
                RaisePropertyChanged("OutDrugInvStatus");
                OnTypIDChanged();
            }
        }
        private Nullable<long> _TypID;
        partial void OnTypIDChanging(Nullable<long> value);
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
        public String OutInvIDString
        {
            get
            {
                return _OutInvIDString;
            }
            set
            {
                _OutInvIDString = value;
                RaisePropertyChanged("OutInvIDString");
            }
        }
        private String _OutInvIDString;

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

        [CustomValidation(typeof(OutwardDrugInvoice), "ValidateOutDate")]
        [DataMemberAttribute()]
        public DateTime OutDate
        {
            get
            {
                return _OutDate;
            }
            set
            {
                OnOutDateChanging(value);
                ValidateProperty("OutDate", value);
                _OutDate = value;
                RaisePropertyChanged("OutDate");
                RaisePropertyChanged("CanEditPayed");
                OnOutDateChanged();
                //De cho tuong thich:
                _createdDate = _OutDate;
                RaisePropertyChanged("CreatedDate");
            }
        }
        private DateTime _OutDate = DateTime.Now;
        partial void OnOutDateChanging(DateTime value);
        partial void OnOutDateChanged();

        [DataMemberAttribute()]
        public String ContactVisitor
        {
            get
            {
                return _ContactVisitor;
            }
            set
            {
                OnContactVisitorChanging(value);
                _ContactVisitor = value;
                RaisePropertyChanged("ContactVisitor");
                OnContactVisitorChanged();
            }
        }
        private String _ContactVisitor;
        partial void OnContactVisitorChanging(String value);
        partial void OnContactVisitorChanged();

        [Required(ErrorMessage = "Vui lòng nhập tên khách hàng")]
        [DataMemberAttribute()]
        public String FullName
        {
            get
            {
                return _FullName;
            }
            set
            {
                OnFullNameChanging(value);
                ValidateProperty("FullName", value);
                _FullName = value;
                RaisePropertyChanged("FullName");
                OnFullNameChanged();

                RaisePropertyChanged("CanNew");
            }
        }
        private String _FullName;
        partial void OnFullNameChanging(String value);
        partial void OnFullNameChanged();

        [RegularExpression("^[0-9]{9,12}$", ErrorMessage = "Số điện thoại không hợp lệ!")]
        [DataMemberAttribute()]
        public String NumberPhone
        {
            get
            {
                return _NumberPhone;
            }
            set
            {
                OnNumberPhoneChanging(value);
                ValidateProperty("NumberPhone", value);
                _NumberPhone = value;
                RaisePropertyChanged("NumberPhone");
                OnNumberPhoneChanged();

                RaisePropertyChanged("CanNew");
            }
        }
        private String _NumberPhone;
        partial void OnNumberPhoneChanging(String value);
        partial void OnNumberPhoneChanged();

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

                RaisePropertyChanged("CanNew");
            }
        }
        private String _Address;
        partial void OnAddressChanging(String value);
        partial void OnAddressChanged();

        //KMx: Năm sinh phải có 4 chữ số (04/09/2014 10:46).
        //[RegularExpression("^[0-9]{2,4}$", ErrorMessage = "Năm sinh không hợp lệ!")]
        [RegularExpression("^[0-9]{4}$", ErrorMessage = "Năm sinh không hợp lệ!")]
        [DataMemberAttribute()]
        public String DOBString
        {
            get
            {
                return _DOBString;
            }
            set
            {
                ValidateProperty("DOBString", value);
                _DOBString = value;
                RaisePropertyChanged("DOBString");
                RaisePropertyChanged("CanNew");
            }
        }
        private String _DOBString;



        [DataMemberAttribute()]
        public Nullable<long> IssueID
        {
            get
            {
                return _IssueID;
            }
            set
            {
                OnIssueIDChanging(value);
                _IssueID = value;
                RaisePropertyChanged("IssueID");
                OnIssueIDChanged();
            }
        }
        private Nullable<long> _IssueID;
        partial void OnIssueIDChanging(Nullable<long> value);
        partial void OnIssueIDChanged();

        private OutwardDrugInvoice _drugInvoice;

        [DataMemberAttribute()]
        public OutwardDrugInvoice DrugInvoice
        {
            get
            {
                return _drugInvoice;
            }
            set
            {
                _drugInvoice = value;
                RaisePropertyChanged("DrugInvoice");
                RaiseErrorsChanged("CanEnabeNoteReturn");
            }
        }



        [DataMemberAttribute()]
        public Nullable<Int64> PtRegistrationID
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
        private Nullable<Int64> _PtRegistrationID;

        [DataMemberAttribute()]
        public String ReturnInvoice
        {
            get
            {
                return _ReturnInvoice;
            }
            set
            {
                //ma phieu xuat ban dau
                _ReturnInvoice = value;
                RaisePropertyChanged("ReturnInvoice");

            }
        }
        private String _ReturnInvoice;

        [DataMemberAttribute()]
        public Nullable<Int64> ReturnID
        {
            get
            {
                return _ReturnID;
            }
            set
            {
                //ma phieu xuat ban dau
                _ReturnID = value;
                RaisePropertyChanged("ReturnID");

            }
        }
        private Nullable<Int64> _ReturnID;

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
                RaisePropertyChanged("CanGetMoney");
                RaisePropertyChanged("CanSaveAndPaid");
                RaisePropertyChanged("CanSaveAndPaidPrescript");
                RaisePropertyChanged("CanCollection");
                RaisePropertyChanged("CanCancel");
                RaisePropertyChanged("CanDelete");
                RaisePropertyChanged("CanEditPayed");
                RaisePropertyChanged("CanReturn");
                RaisePropertyChanged("CanReturnXNB");
                RaisePropertyChanged("RefundMoney");
                RaisePropertyChanged("CanPrint");
                RaisePropertyChanged("CanPrint38");
                RaisePropertyChanged("OutDrugInvStatus");
            }
        }
        private Int64 _V_OutDrugInvStatus;

        [DataMemberAttribute()]
        public bool AlreadyReported
        {
            get
            {
                return _AlreadyReported;
            }
            set
            {
                _AlreadyReported = value;
            }
        }
        public bool _AlreadyReported;
        /// <summary>
        /// Tinh lai cho dung voi truong hop tong quat(giong dich vu, CLS)
        /// </summary>
        public void CalculateState()
        {
            switch (_V_OutDrugInvStatus)
            {
                case (long)AllLookupValues.V_OutDrugInvStatus.CANCELED:
                case (long)AllLookupValues.V_OutDrugInvStatus.REFUNDED:
                    //case (long)AllLookupValues.V_OutDrugInvStatus.RETURN:khong xai cai nay dc
                    ExamRegStatus = AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI;
                    break;

                case (long)AllLookupValues.V_OutDrugInvStatus.SAVE:
                case (long)AllLookupValues.V_OutDrugInvStatus.PAID:
                    ExamRegStatus = AllLookupValues.ExamRegStatus.DANG_KY_KHAM;
                    break;

                case (long)AllLookupValues.V_OutDrugInvStatus.DRUGCOLLECTED:
                    ExamRegStatus = AllLookupValues.ExamRegStatus.HOAN_TAT;
                    break;

                default:
                    ExamRegStatus = AllLookupValues.ExamRegStatus.HOAN_TAT;
                    break;
            }
            RaisePropertyChanged("OutDrugInvStatus");
        }

        public string OutDrugInvStatus
        {
            get
            {
                if (outiID <= 0)
                {
                    return "Hoàn toàn mới";
                }
                //if (ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                //{
                //    return RefundTime != null ? "Hủy-Đã hoàn tiền" : "Hủy-Chưa hoàn tiền";
                //}
                if (V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.DRUGCOLLECTED)
                {
                    return "Đã lấy thuốc";
                }
                else if (V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.RETURN)
                {
                    return "Đã Trả Thuốc";
                }
                else if (V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED)
                {
                    if (PaidTime == null && RefundTime == null)
                    {
                        return "Đã hủy phiếu";
                    }
                    else
                    {
                        return RefundTime != null ? "Hủy-Đã hoàn tiền" : "Hủy-Chưa hoàn tiền";
                    }
                }
                else if (PaidTime != null)
                {
                    return "Đã trả tiền";
                }
                else if (TypID == (long)AllLookupValues.RefOutputType.HOANTRATHUOC)
                {
                    if (PaidTime != null || RefundTime != null)
                    {
                        return "Đã hoàn tiền";
                    }
                }

                return "Chưa trả tiền";
            }
        }
        //private string _OutDrugInvStatus = "Hoàn toàn mới";

        [DataMemberAttribute()]
        public String PatientCode
        {
            get
            {
                return _PatientCode;
            }
            set
            {
                _PatientCode = value;
                RaisePropertyChanged("Address");
            }
        }
        private String _PatientCode;

        [DataMemberAttribute()]
        public String HICardNo
        {
            get
            {
                return _HICardNo;
            }
            set
            {
                _HICardNo = value;
                RaisePropertyChanged("HICardNo");
            }
        }
        private String _HICardNo;

        [DataMemberAttribute()]
        public Nullable<Int64> ReqDrugInID
        {
            get
            {
                return _ReqDrugInID;
            }
            set
            {
                //ma phieu xuat ban dau
                _ReqDrugInID = value;
                RaisePropertyChanged("ReqDrugInID");
                RaisePropertyChanged("IsEnableToStore");

            }
        }
        private Nullable<Int64> _ReqDrugInID;

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
        public Nullable<Int64> ToStaffID
        {
            get
            {
                return _ToStaffID;
            }
            set
            {
                _ToStaffID = value;
                RaisePropertyChanged("ToStaffID");

            }
        }
        private Nullable<Int64> _ToStaffID;

        [DataMemberAttribute()]
        public Nullable<Int64> ToStoreID
        {
            get
            {
                return _ToStoreID;
            }
            set
            {
                //ma phieu xuat ban dau
                _ToStoreID = value;
                RaisePropertyChanged("ToStoreID");
            }
        }
        private Nullable<Int64> _ToStoreID;

        [DataMemberAttribute()]
        public Nullable<Int64> HosID
        {
            get
            {
                return _HosID;
            }
            set
            {
                _HosID = value;
                RaisePropertyChanged("HosID");

            }
        }
        private Nullable<Int64> _HosID;

        [DataMemberAttribute()]
        public String ToStoreName
        {
            get
            {
                return _ToStoreName;
            }
            set
            {
                _ToStoreName = value;
                RaisePropertyChanged("ToStoreName");
            }
        }
        private String _ToStoreName;

        [DataMemberAttribute()]
        public String ToStaffName
        {
            get
            {
                return _ToStaffName;
            }
            set
            {
                _ToStaffName = value;
                RaisePropertyChanged("ToStaffName");
            }
        }
        private String _ToStaffName;

        [DataMemberAttribute()]
        public String HosName
        {
            get
            {
                return _HosName;
            }
            set
            {
                _HosName = value;
                RaisePropertyChanged("HosName");
            }
        }
        private String _HosName;

        [DataMemberAttribute()]
        public int ColectDrugSeqNum
        {
            get
            {
                return _colectDrugSeqNum;
            }
            set
            {
                _colectDrugSeqNum = value;
                RaisePropertyChanged("ColectDrugSeqNum");

            }
        }
        private int _colectDrugSeqNum;

        [DataMemberAttribute()]
        public byte ColectDrugSeqNumType
        {
            get
            {
                return _colectDrugSeqNumType;
            }
            set
            {
                _colectDrugSeqNumType = value;
                RaisePropertyChanged("ColectDrugSeqNumType");

            }
        }
        private byte _colectDrugSeqNumType;

        private string _colectDrugSeqNumString;
        [DataMemberAttribute]
        public string ColectDrugSeqNumString
        {
            get
            {
                return _colectDrugSeqNumString;
            }
            set
            {
                _colectDrugSeqNumString = value;
                RaisePropertyChanged("ColectDrugSeqNumString");
            }
        }

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
        public bool? IsHICount
        {
            get
            {
                return _IsHICount;
            }
            set
            {
                _IsHICount = value;
                RaisePropertyChanged("IsHICount");
                RaisePropertyChanged("CanPrint38");
            }
        }
        private bool? _IsHICount = false;

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
                RaisePropertyChanged("CanDelete");
                RaisePropertyChanged("CanSaveAndPaid");
                RaisePropertyChanged("CanSaveAndPaidPrescript");
                RaisePropertyChanged("CanCollection");
                RaisePropertyChanged("CanCancel");
                RaisePropertyChanged("CanUpdate");
            }
        }
        private bool _CheckedPoint = false;

        [DataMemberAttribute()]
        public long V_ByOutPrice
        {
            get
            {
                return _V_ByOutPrice;
            }
            set
            {
                _V_ByOutPrice = value;
                RaisePropertyChanged("V_ByOutPrice");
            }
        }
        private long _V_ByOutPrice = (long)AllLookupValues.V_ByOutPrice.GIAVON;

        //[DataMemberAttribute()]
        //public bool IsInternal_Sell
        //{
        //    get
        //    {
        //        return _IsInternal_Sell;
        //    }
        //    set
        //    {
        //        _IsInternal_Sell = value;
        //        RaisePropertyChanged("IsInternal_Sell");
        //        RaisePropertyChanged("IsInternal_Loan");
        //    }
        //}
        //private bool _IsInternal_Sell = true;

        //public bool IsInternal_Loan
        //{
        //    get { return !IsInternal_Sell; }
        //}

        [DataMemberAttribute()]
        public bool Checked
        {
            get
            {
                return _Checked;
            }
            set
            {
                _Checked = value;
                RaisePropertyChanged("Checked");
            }
        }
        private bool _Checked;
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

        [DataMemberAttribute()]
        public ObservableCollection<OutwardDrug> _OutwardDrugs;
        public ObservableCollection<OutwardDrug> OutwardDrugs
        {
            get
            {
                return _OutwardDrugs;
            }
            set
            {
                if (_OutwardDrugs != value)
                {
                    _OutwardDrugs = value;
                    RaisePropertyChanged("OutwardDrugs");

                    RaisePropertyChanged("CanNew");
                }
            }
        }

        [DataMemberAttribute()]
        public Prescription SelectedPrescription
        {
            get
            {
                return _SelectedPrescription;
            }
            set
            {
                _SelectedPrescription = value;
                RaisePropertyChanged("SelectedPrescription");
                RaisePropertyChanged("CanEditPrescription");

                RaisePropertyChanged("CanNew");
            }
        }
        private Prescription _SelectedPrescription;


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


        [DataMemberAttribute()]
        public ObservableCollection<PatientTransactionDetail> PatientTransactionDetails
        {
            get;
            set;
        }

        #endregion

        public override bool Equals(object obj)
        {
            OutwardDrugInvoice info = obj as OutwardDrugInvoice;
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

        //tuong duong voi nut bo qua
        public bool CanNew
        {
            get { return outiID > 0 || !string.IsNullOrEmpty(FullName) || !string.IsNullOrEmpty(NumberPhone) || !string.IsNullOrEmpty(Address) || (OutwardDrugs != null && OutwardDrugs.Count > 0) || (SelectedPrescription != null && SelectedPrescription.PrescriptID > 0); }
        }

        public bool CanReturn
        {
            get { return (V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.DRUGCOLLECTED || V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.RETURN); }
        }

        public bool CanReturnXNB
        {
            get { return (V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.DRUGCOLLECTED && TypID == (long)AllLookupValues.RefOutputType.XUATNOIBO_CHOMUON); }
        }

        public bool CanEnabeNoteReturn
        {
            get { return outiID > 0 && (DrugInvoice == null || DrugInvoice.outiID == 0); }
        }

        public bool IsEnableToStore
        {
            get { return (ReqDrugInID == 0 || ReqDrugInID == null) && CanSaveAndPaid; }
        }

        //▼===== 20200612 TTM: Anh Tuân nói nút thu tiền luôn luôn hiển thị (Duyệt toa - Xác nhận - Bán thuốc)
        public bool CanGetMoney
        {
            get { return ((outiID > 0) && !PaidTime.HasValue && (V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.SAVE) && TypID != (long)AllLookupValues.RefOutputType.XUATNOIBO_LUANCHUYENKHO); }
        }
        //public bool CanGetMoney
        //{
        //    get { return true; }
        //}

        public bool RefundMoney
        {
            get { return ((outiID > 0) && PaidTime.HasValue && !RefundTime.HasValue && V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED) || (ReturnID > 0 && PaidTime == null && RefundTime == null && (DrugInvoice != null && DrugInvoice.TypID != (long)AllLookupValues.RefOutputType.XUATNOIBO_CHOMUON)); }
        }

        public bool CanDelete
        {
            get { return (outiID > 0) && (V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.SAVE) && !CheckedPoint; }
        }

        public bool CanSaveAndPaid
        {
            get { return (outiID == 0 && !CheckedPoint); }
        }

        public bool CanSaveAndPaidPrescript
        {
            get { return (outiID == 0 && (PrescriptID > 0) && !CheckedPoint); }
        }

        public bool CanCollection
        {
            get
            {
                //return V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.PAID;
                return PaidTime.HasValue && V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.SAVE;
            }
        }

        public bool CanCancel
        {
            get { return (outiID > 0 && V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.SAVE || V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.PAID) && !CheckedPoint; }
        }

        public bool CanPrint
        {
            get
            {
                if (TypID == 3)
                {
                    return outiID > 0;
                }
                else
                {
                    return outiID > 0 && (PaidTime.HasValue || RefundTime.HasValue || V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.DRUGCOLLECTED || V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.RETURN);
                }
            }
        }
        public bool CanPrint38
        {
            get
            {
                return outiID > 0 && IsHICount.GetValueOrDefault()
                    && (PaidTime.HasValue || V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.DRUGCOLLECTED || V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED || RefundTime.HasValue || V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.RETURN);

            }
        }
        public bool CanEditPrescription
        {
            get { return (outiID == 0 && SelectedPrescription != null && !SelectedPrescription.IsSold); }
        }

        public bool CanEditPayed
        {
            get
            {
                //Sau này để vào cấu hình.
                //Bán thuốc theo toa: Nếu ngày xuất không quá 7 ngày so với ngày hiện tại thì cho cập nhật phiếu xuất.
                TimeSpan daysFromOutDateToNow = DateTime.Now - OutDate;
                //return (outiID > 0 && (OutDate.ToShortDateString() == DateTime.Now.ToShortDateString() && (V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.SAVE || V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.DRUGCOLLECTED ))) && !CheckedPoint;
                //KMx: Add condition TypID != (long)AllLookupValues.RefOutputType.XUATNOIBO_LUANCHUYENKHO (14/02/2014 16:53)
                //return (outiID > 0 && (daysFromOutDateToNow.Days <= 7 && (V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.SAVE || V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.DRUGCOLLECTED))) && !CheckedPoint && TypID != (long)AllLookupValues.RefOutputType.XUATNOIBO_LUANCHUYENKHO;
                return (outiID > 0 && (daysFromOutDateToNow.Days <= 7 && V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.SAVE)) && !CheckedPoint;
            }
        }
        //dung cho cap nhat phieu huy
        public bool CanUpdate
        {
            get { return (outiID > 0 && !CheckedPoint); }
        }
        #region IDBRecordState Members

        private RecordState _RecordState;
        public RecordState RecordState
        {
            get
            {
                return _RecordState;
            }
            set
            {
                if (_RecordState != value)
                {
                    _RecordState = value;
                    RaisePropertyChanged("RecordState");
                }
            }
        }

        #endregion

        #region Convert XML

        public string ConvertDetailsListToXml()
        {
            return ConvertDetailsListToXml(_OutwardDrugs);
        }
        public string ConvertDetailsListToXml(IEnumerable<OutwardDrug> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<OutwardDrugs>");
                foreach (OutwardDrug details in items)
                {
                    if (details.GetDrugForSellVisitor != null && details.DrugID > 0)
                    {
                        int EntityState = (int)details.EntityState;
                        sb.Append("<RecInfo>");
                        sb.AppendFormat("<OutID>{0}</OutID>", details.OutID);
                        sb.AppendFormat("<outiID>{0}</outiID>", details.outiID);
                        sb.AppendFormat("<DrugID>{0}</DrugID>", details.DrugID);
                        sb.AppendFormat("<HIBenefit>{0}</HIBenefit>", details.HIBenefit);
                        sb.AppendFormat("<OutQuantity>{0}</OutQuantity>", details.OutQuantity);
                        sb.AppendFormat("<OutQuantityReturn>{0}</OutQuantityReturn>", details.OutQuantityReturn);
                        sb.AppendFormat("<OutPrice>{0}</OutPrice>", details.OutPrice);
                        //sb.AppendFormat("<OutHIRebate>{0}</OutHIRebate>", Math.Floor(Convert.ToDouble(details.TotalHIPayment)));
                        //sb.AppendFormat("<OutAmountCoPay>{0}</OutAmountCoPay>", Math.Ceiling(Convert.ToDouble(details.TotalCoPayment)));
                        //sb.AppendFormat("<OutDate>{0}</OutDate>", details.OutDate);
                        sb.AppendFormat("<OutHIRebate>{0}</OutHIRebate>", details.TotalHIPayment);
                        sb.AppendFormat("<OutAmountCoPay>{0}</OutAmountCoPay>", details.TotalCoPayment);
                        sb.AppendFormat("<OutHIAllowedPrice>{0}</OutHIAllowedPrice>", details.HIAllowedPrice);
                        sb.AppendFormat("<OutPriceDifference>{0}</OutPriceDifference>", details.PriceDifference);
                        sb.AppendFormat("<InID>{0}</InID>", details.InID);
                        sb.AppendFormat("<OutNotes>{0}</OutNotes>", details.OutNotes);
                        sb.AppendFormat("<QtyOffer>{0}</QtyOffer>", details.QtyOffer);
                        sb.AppendFormat("<DoseString>{0}</DoseString>", details.DoseString);
                        sb.AppendFormat("<VAT>{0}</VAT>", details.VAT);
                        sb.AppendFormat("<IsNotVat>{0}</IsNotVat>", details.IsNotVat);
                        sb.Append("</RecInfo>");
                    }
                }
                sb.Append("</OutwardDrugs>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region close member
        private ObservableCollection<OutwardDrugInvoice> _returnedInvoices;
        [DataMemberAttribute()]
        public ObservableCollection<OutwardDrugInvoice> ReturnedInvoices
        {
            get
            {
                return _returnedInvoices;
            }
            set
            {
                _returnedInvoices = value;
                RaisePropertyChanged("ReturnedInvoices");
            }
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

        public IChargeableItemPrice ChargeableItem { get; set; }

        public double? HIBenefit
        {
            get;
            set;
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
                RaisePropertyChanged("CanPrint");
                RaisePropertyChanged("CanPrint38");
                RaisePropertyChanged("OutDrugInvStatus");
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
                RaisePropertyChanged("RefundMoney");
                RaisePropertyChanged("OutDrugInvStatus");

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
        private AllLookupValues.ExamRegStatus _examRegStatus = AllLookupValues.ExamRegStatus.KHONG_XAC_DINH;

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

        //KMx: Trong class này không sử dụng được MidpointRounding nên dời hàm qua RegAndPaymentProcessorBase.cs (22/08/2014 11:33).
        //KMx: OnlyRoundResultForOutward = true: Tính tổng tiền BH trả rồi mới làm tròn. Ngược lại thì làm tròn tiền BH trả trên từng dòng thuốc rồi mới tính tổng (02/08/2014 18:24).
        //public void CalTotal(bool onlyRoundResultForOutward)
        //{
        //    TotalInvoicePrice = 0;
        //    TotalPriceDifference = 0;
        //    TotalHIPayment = 0;
        //    TotalCoPayment = 0;
        //    TotalPatientPayment = 0;

        //    decimal totalHIAllowedPrice = 0;

        //    if (this.OutwardDrugs != null && this.OutwardDrugs.Count > 0)
        //    {
        //        if (!onlyRoundResultForOutward)
        //        {
        //            foreach (var item in OutwardDrugs)
        //            {
        //                TotalInvoicePrice += item.TotalInvoicePrice;
        //                TotalPriceDifference += item.TotalPriceDifference;
        //                TotalHIPayment += item.TotalHIPayment;
        //                TotalCoPayment += item.TotalCoPayment;
        //                TotalPatientPayment += item.TotalPatientPayment;
        //            }
        //        }
        //        else
        //        {
        //            foreach (var item in OutwardDrugs)
        //            {
        //                TotalInvoicePrice += item.TotalInvoicePrice;
        //                TotalPriceDifference += item.TotalPriceDifference;
        //                TotalHIPayment += item.TotalHIPayment;
        //                //KMx: BN đồng chi trả = Giá BH - Số tiền BH bệnh nhân được hưởng (01/08/2014 16:40).
        //                //TotalCoPayment += item.TotalCoPayment;
        //                TotalPatientPayment += item.TotalPatientPayment;
        //                totalHIAllowedPrice += item.HIAllowedPrice.GetValueOrDefault() * item.OutQuantity;
        //            }
        //            TotalInvoicePrice = Math.Round(TotalInvoicePrice);
        //            TotalHIPayment = Math.Round(TotalHIPayment);
        //            TotalCoPayment = totalHIAllowedPrice - TotalHIPayment;
        //        }
        //    }
        //}
        #endregion

        public static ValidationResult ValidateOutDate(DateTime value, ValidationContext context)
        {
            if (AxHelper.CompareDate(value, DateTime.Now) == 1)
            {
                return new ValidationResult("Ngày xuất không được lớn hơn ngày hiện tại", new string[] { "OutDate" });
            }
            return ValidationResult.Success;
        }

        private AllLookupValues.RegistrationType _V_RegistrationType = AllLookupValues.RegistrationType.NGOAI_TRU;
        [DataMemberAttribute()]
        public AllLookupValues.RegistrationType V_RegistrationType
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

        private bool? _IsUpdate = false;
        [DataMemberAttribute()]
        public bool? IsUpdate
        {
            get
            {
                return _IsUpdate;
            }
            set
            {
                _IsUpdate = value;
                RaisePropertyChanged("IsUpdate");
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

        /*==== #001 ====*/
        public bool _IsOutHIPt = false;
        [DataMemberAttribute()]
        public bool IsOutHIPt
        {
            get
            {
                return _IsOutHIPt;
            }
            set
            {
                _IsOutHIPt = value;
                RaisePropertyChanged("IsOutHIPt");
            }
        }
        /*==== #001 ====*/

        public override string ToString()
        {
            return this.OutInvoiceNumber;
        }

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

        private DiseasesReference _MainICD10;
        private string _IssuedStaffFullName;
        private Gender _PatientGender;
        [DataMemberAttribute]
        public DiseasesReference MainICD10
        {
            get
            {
                return _MainICD10;
            }
            set
            {
                _MainICD10 = value;
                RaisePropertyChanged("MainICD10");
            }
        }
        [DataMemberAttribute]
        public string IssuedStaffFullName
        {
            get
            {
                return _IssuedStaffFullName;
            }
            set
            {
                _IssuedStaffFullName = value;
                RaisePropertyChanged("IssuedStaffFullName");
            }
        }
        [DataMemberAttribute]
        public Gender PatientGender
        {
            get
            {
                return _PatientGender;
            }
            set
            {
                _PatientGender = value;
                RaisePropertyChanged("PatientGender");
            }
        }

        [DataMemberAttribute]
        public long V_TradingPlaces
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
        private long _V_TradingPlaces;

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
        [DataMemberAttribute()]
        public bool IsWaiting
        {
            get
            {
                return _IsWaiting;
            }
            set
            {
                _IsWaiting = value;
                RaisePropertyChanged("IsWaiting");
            }
        }
        private bool _IsWaiting;
        [DataMemberAttribute()]
        public int CountPrint
        {
            get
            {
                return _CountPrint;
            }
            set
            {
                _CountPrint = value;
                RaisePropertyChanged("CountPrint");
            }
        }
        private int _CountPrint;
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
        public long DQGReportID
        {
            get
            {
                return _DQGReportID;
            }
            set
            {
                if (_DQGReportID != value)
                {
                    _DQGReportID = value;
                    RaisePropertyChanged("DQGReportID");
                }
            }
        }
        private long _DQGReportID;
        //▲====: #002
    }
}