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
/*
 * 20200329 #001 TNHX: Thêm biến để kiểm tra phiếu xuất mới này từ phiếu yêu cầu đã được xuất
 * 20210929 #002 TNHX: Thêm biến để kiểm tra phiếu xuất mới này từ phiếu yêu cầu đã được xuất
 */
namespace DataEntities
{
    public partial class OutwardDrugMedDeptInvoice : NotifyChangedBase, IInvoiceItem
    {
        #region Factory Method


        /// Create a new OutwardDrugMedDeptInvoice object.

        /// <param name="outiID">Initial value of the outiID property.</param>
        /// <param name="outInvID">Initial value of the OutInvID property.</param>
        public static OutwardDrugMedDeptInvoice CreateOutwardDrugMedDeptInvoice(Int64 outiID, String outInvID)
        {
            OutwardDrugMedDeptInvoice outwardDrugMedDeptInvoice = new OutwardDrugMedDeptInvoice();
            outwardDrugMedDeptInvoice.outiID = outiID;
            outwardDrugMedDeptInvoice.OutInvID = outInvID;
            return outwardDrugMedDeptInvoice;
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

                    RaisePropertyChanged("CanGetMoney");
                    RaisePropertyChanged("CanPrint");
                    RaisePropertyChanged("IsVisitor");
                    RaisePropertyChanged("IsVisitorAndCanSave");
                    RaisePropertyChanged("IsElseVisitor");
                    RaisePropertyChanged("CanUpdate");
                    RaisePropertyChanged("CanSave");
                    RaisePropertyChanged("CanEditOutward");
                    RaisePropertyChanged("CanSaveAndPaid");
                    RaisePropertyChanged("CanSaveAndPaidPrescript");
                    OnoutiIDChanged();
                }
            }
        }
        private Int64 _outiID;
        partial void OnoutiIDChanging(Int64 value);
        partial void OnoutiIDChanged();

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
        public String FullName
        {
            get
            {
                return _FullName;
            }
            set
            {
                OnFullNameChanging(value);
                _FullName = value;
                RaisePropertyChanged("FullName");
                OnFullNameChanged();
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
            }
        }
        private String _Address;
        partial void OnAddressChanging(String value);
        partial void OnAddressChanged();

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
                RaisePropertyChanged("IsInternal");
                OnTypIDChanged();
            }
        }
        private Nullable<Int64> _TypID;
        partial void OnTypIDChanging(Nullable<Int64> value);
        partial void OnTypIDChanged();


        [DataMemberAttribute()]
        public string TypName
        {
            get
            {
                return _TypName;
            }
            set
            {
                _TypName = value;
                RaisePropertyChanged("TypName");
            }
        }
        private string _TypName;



        [DataMemberAttribute()]
        public Int64 V_MedProductType
        {
            get
            {
                return _V_MedProductType;
            }
            set
            {
                OnV_MedProductTypeChanging(value);
                _V_MedProductType = value;
                RaisePropertyChanged("V_MedProductType");
                OnV_MedProductTypeChanged();
            }
        }
        private Int64 _V_MedProductType;
        partial void OnV_MedProductTypeChanging(Int64 value);
        partial void OnV_MedProductTypeChanged();

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

        private string _CategoryName;
        [DataMemberAttribute()]
        public string CategoryName
        {
            get
            {
                return _CategoryName;
            }
            set
            {
                if (_CategoryName != value)
                {
                    _CategoryName = value;
                    RaisePropertyChanged("CategoryName");
                }
            }
        }

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
            }
        }
        private Nullable<DateTime> _OutDate;
        partial void OnOutDateChanging(Nullable<DateTime> value);
        partial void OnOutDateChanged();

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
                RaisePropertyChanged("IsEnableToStore");
                RaisePropertyChanged("IsVisitor");
                RaisePropertyChanged("IsVisitorAndCanSave");
                RaisePropertyChanged("IsElseVisitor");
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
                RaisePropertyChanged("StringName");
                RaisePropertyChanged("IsVisitor");
                RaisePropertyChanged("IsVisitorAndCanSave");
                RaisePropertyChanged("IsElseVisitor");
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
                RaisePropertyChanged("CanGetMoney");
                RaisePropertyChanged("CanSaveAndPaid");
                RaisePropertyChanged("CanCollection");
                RaisePropertyChanged("CanCancel");
                RaisePropertyChanged("CanDelete");
                RaisePropertyChanged("CanReturn");
                RaisePropertyChanged("CanSaveAndPaidPrescript");

            }
        }
        private Int64 _V_OutDrugInvStatus;

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
                RaisePropertyChanged("CanCollection");
                RaisePropertyChanged("CanCancel");
                RaisePropertyChanged("IsVisitor");
                RaisePropertyChanged("IsVisitorAndCanSave");
                RaisePropertyChanged("IsElseVisitor");
                RaisePropertyChanged("CanUpdate");
                RaisePropertyChanged("CanSave");
                RaisePropertyChanged("CanSaveAndPaid");
                RaisePropertyChanged("CanSaveAndPaidPrescript");
            }
        }
        private bool _CheckedPoint = false;

        [DataMemberAttribute()]
        public long V_ByOutPriceMedDept
        {
            get
            {
                return _V_ByOutPriceMedDept;
            }
            set
            {
                _V_ByOutPriceMedDept = value;
                RaisePropertyChanged("V_ByOutPriceMedDept");
            }
        }
        private long _V_ByOutPriceMedDept = (long)AllLookupValues.V_ByOutPriceMedDept.GIATHONGTHUONG;

        [DataMemberAttribute()]
        public Nullable<Int64> ReturnID
        {
            get
            {
                return _ReturnID;
            }
            set
            {
                _ReturnID = value;
                RaisePropertyChanged("ReturnID");
            }
        }
        private Nullable<Int64> _ReturnID;

        [DataMemberAttribute()]
        public bool AlreadyImported
        {
            get
            {
                return _AlreadyImported;
            }
            set
            {
                _AlreadyImported = value;
                RaisePropertyChanged("AlreadyImported");
            }
        }
        private bool _AlreadyImported;


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
            }
        }
        private Prescription _SelectedPrescription;

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
        public decimal VAT
        {
            get
            {
                return _VAT;
            }
            set
            {
                _VAT = value;
                RaisePropertyChanged("VAT");
            }
        }
        private decimal _VAT = 1;

        #endregion

        #region Navigation Properties

        private ObservableCollection<OutwardDrugMedDept> _OutwardDrugMedDepts;
        [DataMemberAttribute()]
        public ObservableCollection<OutwardDrugMedDept> OutwardDrugMedDepts
        {
            get
            {
                return _OutwardDrugMedDepts;
            }
            set
            {
                _OutwardDrugMedDepts = value;
                RaisePropertyChanged("OutwardDrugMedDepts");
            }
        }

        private ObservableCollection<OutwardDrugMedDept> _OutwardDrugMedDepts_Delete;
        [DataMemberAttribute()]
        public ObservableCollection<OutwardDrugMedDept> OutwardDrugMedDepts_Delete
        {
            get
            {
                return _OutwardDrugMedDepts_Delete;
            }
            set
            {
                _OutwardDrugMedDepts_Delete = value;
                RaisePropertyChanged("OutwardDrugMedDepts_Delete");
            }
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
        public int? IsLoad
        {
            get
            {
                return _IsLoad;
            }
            set
            {
                _IsLoad = value;
                RaisePropertyChanged("IsLoad");
            }
        }
        private int? _IsLoad;

        private bool _isLockedUpdate;
        [DataMemberAttribute()]
        public bool IsLockedUpdate
        {
            get { return _isLockedUpdate; }
            set
            {
                if (_isLockedUpdate != value)
                {
                    _isLockedUpdate = value;
                    RaisePropertyChanged("IsLockedUpdate");
                }
            }
        }

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

        private bool _FromClinicDept;
        [DataMemberAttribute()]
        public bool FromClinicDept
        {
            get
            {
                return _FromClinicDept;
            }
            set
            {
                if (_FromClinicDept != value)
                {
                    _FromClinicDept = value;
                    RaisePropertyChanged("FromClinicDept");
                }
            }
        }
        #endregion

        #region Extension member

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

        private OutwardDrugMedDeptInvoice _OutInvoice;
        [DataMemberAttribute()]
        public OutwardDrugMedDeptInvoice OutInvoice
        {
            get
            {
                return _OutInvoice;
            }
            set
            {
                _OutInvoice = value;
                RaisePropertyChanged("OutInvoice");
            }
        }

        [DataMemberAttribute]
        public string ReturnInvInvoiceNumber
        {
            get
            {
                return _ReturnInvInvoiceNumber;
            }
            set
            {
                _ReturnInvInvoiceNumber = value;
                RaisePropertyChanged("ReturnInvInvoiceNumber");
            }
        }
        private string _ReturnInvInvoiceNumber;
        [DataMemberAttribute]
        public string ReturnSerialNumber
        {
            get
            {
                return _ReturnSerialNumber;
            }
            set
            {
                _ReturnSerialNumber = value;
                RaisePropertyChanged("ReturnSerialNumber");
            }
        }
        private string _ReturnSerialNumber;
        [DataMemberAttribute]
        public string ReturnInvoiceForm
        {
            get
            {
                return _ReturnInvoiceForm;
            }
            set
            {
                _ReturnInvoiceForm = value;
                RaisePropertyChanged("ReturnInvoiceForm");
            }
        }
        private string _ReturnInvoiceForm;
        [DataMemberAttribute]
        public string ReturnNote
        {
            get
            {
                return _ReturnNote;
            }
            set
            {
                _ReturnNote = value;
                RaisePropertyChanged("ReturnNote");
            }
        }
        private string _ReturnNote;
        [DataMemberAttribute]
        public long ReturninviID
        {
            get
            {
                return _ReturninviID;
            }
            set
            {
                _ReturninviID = value;
                RaisePropertyChanged("ReturninviID");
            }
        }
        private long _ReturninviID;
        [DataMemberAttribute]
        public String InvInvoiceNumber
        {
            get
            {
                return _InvInvoiceNumber;
            }
            set
            {
                _InvInvoiceNumber = value;
                RaisePropertyChanged("InvInvoiceNumber");
            }
        }
        private String _InvInvoiceNumber;
        [DataMemberAttribute]
        public String SerialNumber
        {
            get
            {
                return _SerialNumber;
            }
            set
            {
                if (_SerialNumber != value)
                {
                    _SerialNumber = value;
                    RaisePropertyChanged("SerialNumber");
                }
            }
        }
        private String _SerialNumber;
        [DataMemberAttribute]
        public String InvoiceForm
        {
            get
            {
                return _InvoiceForm;
            }
            set
            {
                if (_InvoiceForm != value)
                {
                    _InvoiceForm = value;
                    RaisePropertyChanged("InvoiceForm");
                }
            }
        }
        private String _InvoiceForm;
        #endregion

        public string StringName
        {
            get
            {
                if (V_OutputTo == (long)AllLookupValues.V_OutputTo.KHO_KHAC)
                {
                    return "Kho nhận";
                }
                else if (V_OutputTo == (long)AllLookupValues.V_OutputTo.BACSI || V_OutputTo == (long)AllLookupValues.V_OutputTo.BENHNHAN || V_OutputTo == (long)AllLookupValues.V_OutputTo.KHACH_VANG_LAI)
                {
                    return "Người nhận";
                }
                else if (V_OutputTo == (long)AllLookupValues.V_OutputTo.BVBAN)
                {
                    return "BV nhận";
                }
                else
                {
                    return "Chọn";
                }
            }
        }

        public bool CanReturn
        {
            get { return true; }//(V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.DRUGCOLLECTED || V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.RETURN); }
        }

        public bool IsEnableToStore
        {
            get { return (ReqDrugInClinicDeptID == 0 || ReqDrugInClinicDeptID == null) && CanSaveAndPaid; }
        }

        public bool IsInternal
        {
            //get { return TypID == (long)AllLookupValues.RefOutputType.XUATNOIBO; }
            get
            {
                return TypID == (long)AllLookupValues.RefOutputType.XUATNOIBO || TypID == (long)AllLookupValues.RefOutputType.HUYHANG
                    || TypID == (long)AllLookupValues.RefOutputType.XUAT_DIEUCHUYEN
                    || TypID == (long)AllLookupValues.RefOutputType.XUATNOIBO_CHOMUON || TypID == (long)AllLookupValues.RefOutputType.XUATCHO_BIEU;
            }
        }

        public bool IsVisitor
        {
            get { return (V_OutputTo != (long)AllLookupValues.V_OutputTo.KHACH_VANG_LAI && CanSaveAndPaid && ReqDrugInClinicDeptID.GetValueOrDefault(0) == 0); }
        }

        public bool IsVisitorAndCanSave
        {
            get { return (V_OutputTo != (long)AllLookupValues.V_OutputTo.KHACH_VANG_LAI && CanSave && ReqDrugInClinicDeptID.GetValueOrDefault(0) == 0); }
        }

        public bool IsElseVisitor
        {
            get { return ((V_OutputTo != (long)AllLookupValues.V_OutputTo.KHACH_VANG_LAI || !CanSaveAndPaid)); }
        }
        public bool CanGetMoney
        {
            get { return (outiID > 0) && !PaidTime.HasValue && (V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.SAVE) && (TypID == (long)AllLookupValues.RefOutputType.BANLE || TypID == (long)AllLookupValues.RefOutputType.BANTHEOTOA || TypID == (long)AllLookupValues.RefOutputType.XUAT_HANGKYGOI); }
        }
        public bool RefundMoney
        {
            get { return ((outiID > 0) && PaidTime.HasValue && !RefundTime.HasValue && V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED) || (ReturnID > 0 && PaidTime == null && RefundTime == null && (OutInvoice != null && OutInvoice.TypID != (long)AllLookupValues.RefOutputType.XUATNOIBO_CHOMUON)); }
        }

        public bool CanDelete
        {
            get { return (outiID > 0) && (V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.SAVE) && !CheckedPoint; }
        }

        public bool CanEditOutward
        {
            get { return (outiID == 0); }
        }

        public bool CanSaveAndPaid
        {
            get { return CanSave || CanUpdate; }
        }

        public bool CanSave
        {
            get { return (outiID == 0); }
        }

        public bool CanUpdate
        {
            get { return (outiID > 0 && PaidTime == null && RefundTime == null && !CheckedPoint && !AlreadyImported && ReqDrugInClinicDeptID.GetValueOrDefault(0) != 0); }
        }

        public bool CanCollection
        {
            get { return V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.PAID; }
        }

        public bool CanCancel
        {
            get { return (V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.SAVE || V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.PAID) && !CheckedPoint; }
        }

        //public bool CanPrint
        //{
        //    get { return outiID > 0 && (V_OutDrugInvStatus != (long)AllLookupValues.V_OutDrugInvStatus.PAID || V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.DRUGCOLLECTED || V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED || V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.REFUNDED || V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.RETURN); }
        //}

        public bool CanPrint
        {
            get { return outiID > 0; }
        }


        public bool CanSaveAndPaidPrescript
        {
            get { return (outiID == 0 && (PrescriptID > 0) && !CheckedPoint); }
        }

        public string ConvertDetailsListToXml()
        {
            return ConvertDetailsListToXml(_OutwardDrugMedDepts);
        }

        public string ConvertDetailsListToXml(IEnumerable<OutwardDrugMedDept> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<OutDrugDetails>");
                foreach (OutwardDrugMedDept details in items)
                {
                    if (details.RefGenericDrugDetail != null && details.GenMedProductID > 0)
                    {
                        int EntityState = (int)details.EntityState;
                        sb.Append("<RecInfo>");
                        sb.AppendFormat("<OutID>{0}</OutID>", details.OutID);
                        sb.AppendFormat("<outiID>{0}</outiID>", details.outiID);
                        sb.AppendFormat("<GenMedProductID>{0}</GenMedProductID>", details.RefGenericDrugDetail.GenMedProductID);
                        sb.AppendFormat("<HIBenefit>{0}</HIBenefit>", details.HIBenefit);
                        sb.AppendFormat("<InID>{0}</InID>", details.InID);
                        sb.AppendFormat("<OutQuantity>{0}</OutQuantity>", details.OutQuantity);
                        sb.AppendFormat("<OutPrice>{0}</OutPrice>", details.OutPrice);
                        //----- DPT  fix kí tự dặc biệt trong ghi chú
                        if (details.OutNotes != null)
                        {
                            sb.AppendFormat("<OutNotes>{0}</OutNotes>", Globals.GetSafeXMLString(details.OutNotes));
                        }
                        else
                        {
                            sb.AppendFormat("<OutNotes>{0}</OutNotes>", details.OutNotes);
                        }
                        //--------------------------------------------------

                        sb.AppendFormat("<ReqDrugInDetailID>{0}</ReqDrugInDetailID>", details.ReqDrugInDetailID);
                        sb.AppendFormat("<EntityState>{0}</EntityState>", EntityState);
                        sb.AppendFormat("<RequestQty>{0}</RequestQty>", details.RequestQty);

                        sb.AppendFormat("<OutQuantityReturn>{0}</OutQuantityReturn>", details.OutQuantityReturn);
                        //sb.AppendFormat("<OutHIRebate>{0}</OutHIRebate>", Math.Floor(Convert.ToDouble(details.TotalHIPayment > 0 ? details.TotalHIPayment : details.OutHIRebate.GetValueOrDefault(0))));
                        sb.AppendFormat("<OutHIRebate>{0}</OutHIRebate>", details.TotalHIPayment > 0 ? details.TotalHIPayment : details.OutHIRebate.GetValueOrDefault(0));
                        sb.AppendFormat("<OutPriceDifference>{0}</OutPriceDifference>", details.PriceDifference);
                        sb.AppendFormat("<HIAllowedPrice>{0}</HIAllowedPrice>", details.HIAllowedPrice);
                        //sb.AppendFormat("<OutAmountCoPay>{0}</OutAmountCoPay>", Math.Ceiling(Convert.ToDouble(details.TotalCoPayment > 0 ? details.TotalCoPayment : details.OutAmountCoPay)));
                        sb.AppendFormat("<OutAmountCoPay>{0}</OutAmountCoPay>", details.TotalCoPayment > 0 ? details.TotalCoPayment : details.OutAmountCoPay);
                        sb.AppendFormat("<IsDeleted>{0}</IsDeleted>", details.IsDeleted);
                        sb.AppendFormat("<HIPaymentPercent>{0}</HIPaymentPercent>", details.HIPaymentPercent);
                        sb.AppendFormat("<DrugDeptInIDOrig>{0}</DrugDeptInIDOrig>", details.DrugDeptInIDOrig);
                        sb.Append("</RecInfo>");
                    }
                }
                sb.Append("</OutDrugDetails>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
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
        public void CalTotal()
        {
            TotalInvoicePrice = 0;
            TotalPriceDifference = 0;
            TotalHIPayment = 0;
            TotalCoPayment = 0;
            TotalPatientPayment = 0;

            if (this.OutwardDrugMedDepts != null && this.OutwardDrugMedDepts.Count > 0)
            {
                foreach (var item in OutwardDrugMedDepts)
                {
                    TotalInvoicePrice += item.TotalInvoicePrice;
                    TotalPriceDifference += item.TotalPriceDifference;
                    TotalHIPayment += item.TotalHIPayment;
                    TotalCoPayment += item.TotalCoPayment;
                    TotalPatientPayment += item.TotalPatientPayment;
                }
            }
        }
        #endregion

        public void CalculateState()
        {
            switch (_V_OutDrugInvStatus)
            {
                case (long)AllLookupValues.V_OutDrugInvStatus.CANCELED:
                case (long)AllLookupValues.V_OutDrugInvStatus.REFUNDED:
                case (long)AllLookupValues.V_OutDrugInvStatus.RETURN:
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
        public bool ReqDrugWasExportFromMedDept
        {
            get
            {
                return _ReqDrugWasExportFromMedDept;
            }
            set
            {
                _ReqDrugWasExportFromMedDept = value;
                RaisePropertyChanged("ReqDrugWasExportFromMedDept");
            }
        }
        private bool _ReqDrugWasExportFromMedDept = false;
        //▲====: #001
        //▼====: #002
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
        //▲====: #002
    }
}
