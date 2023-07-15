using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Service.Core.Common;
using eHCMS.Configurations;

namespace DataEntities
{
    public partial class RequestDrugInwardClinicDept : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new RequestDrugInwardClinicDept object.

        /// <param name="reqDrugInClinicDeptID">Initial value of the ReqDrugInClinicDeptID property.</param>
        /// <param name="staffID">Initial value of the StaffID property.</param>
        /// <param name="reqDate">Initial value of the ReqDate property.</param>
        public static RequestDrugInwardClinicDept CreateRequestDrugInwardClinicDept(Int64 reqDrugInClinicDeptID, Int64 staffID, DateTime reqDate)
        {
            RequestDrugInwardClinicDept requestDrugInwardClinicDept = new RequestDrugInwardClinicDept();
            requestDrugInwardClinicDept.ReqDrugInClinicDeptID = reqDrugInClinicDeptID;
            requestDrugInwardClinicDept.StaffID = staffID;
            requestDrugInwardClinicDept.ReqDate = reqDate;
            return requestDrugInwardClinicDept;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 ReqDrugInClinicDeptID
        {
            get
            {
                return _ReqDrugInClinicDeptID;
            }
            set
            {
                if (_ReqDrugInClinicDeptID != value)
                {
                    OnReqDrugInClinicDeptIDChanging(value);
                    _ReqDrugInClinicDeptID = value;
                    RaisePropertyChanged("ReqDrugInClinicDeptID");
                    OnReqDrugInClinicDeptIDChanged();

                    RaisePropertyChanged("CanEdit");
                    RaisePropertyChanged("CanSave");
                    RaisePropertyChanged("CanDelete");
                    RaisePropertyChanged("CanPrint");
                    RaisePropertyChanged("CanApprove");
                    RaisePropertyChanged("CanCheckIsInstructionFuture");
                }
            }
        }
        private Int64 _ReqDrugInClinicDeptID;
        partial void OnReqDrugInClinicDeptIDChanging(Int64 value);
        partial void OnReqDrugInClinicDeptIDChanged();

        [DataMemberAttribute()]
        public Int64 StaffID
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
        private Int64 _StaffID;
        partial void OnStaffIDChanging(Int64 value);
        partial void OnStaffIDChanged();


        [DataMemberAttribute()]
        public Nullable<Int64> StaffIDAllow
        {
            get
            {
                return _StaffIDAllow;
            }
            set
            {
                _StaffIDAllow = value;
                RaisePropertyChanged("StaffIDAllow");
            }
        }
        private Nullable<Int64> _StaffIDAllow;




        [DataMemberAttribute()]
        public Nullable<Int64> DeptID
        {
            get
            {
                return _DeptID;
            }
            set
            {
                OnDeptIDChanging(value);
                _DeptID = value;
                RaisePropertyChanged("DeptID");
                OnDeptIDChanged();
            }
        }
        private Nullable<Int64> _DeptID;
        partial void OnDeptIDChanging(Nullable<Int64> value);
        partial void OnDeptIDChanged();


        [DataMemberAttribute()]
        public DateTime ReqDate
        {
            get
            {
                return _ReqDate;
            }
            set
            {
                OnReqDateChanging(value);
                _ReqDate = value;
                RaisePropertyChanged("ReqDate");
                OnReqDateChanged();
            }
        }
        private DateTime _ReqDate = DateTime.Now;
        partial void OnReqDateChanging(DateTime value);
        partial void OnReqDateChanged();

        [DataMemberAttribute()]
        public String ReqNumCode
        {
            get
            {
                return _ReqNumCode;
            }
            set
            {
                OnReqNumCodeChanging(value);
                _ReqNumCode = value;
                RaisePropertyChanged("ReqNumCode");
                OnReqNumCodeChanged();
            }
        }
        private String _ReqNumCode;
        partial void OnReqNumCodeChanging(String value);
        partial void OnReqNumCodeChanged();


        [DataMemberAttribute()]
        public Nullable<Int64> OutFromStoreID
        {
            get
            {
                return _OutFromStoreID;
            }
            set
            {
                OnOutFromStoreIDChanging(value);
                _OutFromStoreID = value;
                RaisePropertyChanged("OutFromStoreID");
                OnOutFromStoreIDChanged();
            }
        }
        private Nullable<Int64> _OutFromStoreID;
        partial void OnOutFromStoreIDChanging(Nullable<Int64> value);
        partial void OnOutFromStoreIDChanged();


        [DataMemberAttribute()]
        public Nullable<Int64> InDeptStoreID
        {
            get
            {
                return _InDeptStoreID;
            }
            set
            {
                OnInDeptStoreIDChanging(value);
                _InDeptStoreID = value;
                RaisePropertyChanged("InDeptStoreID");
                OnInDeptStoreIDChanged();
            }
        }
        private Nullable<Int64> _InDeptStoreID;
        partial void OnInDeptStoreIDChanging(Nullable<Int64> value);
        partial void OnInDeptStoreIDChanged();


        [DataMemberAttribute()]
        public String Comment
        {
            get
            {
                return _Comment;
            }
            set
            {
                OnCommentChanging(value);
                _Comment = value;
                RaisePropertyChanged("Comment");
                OnCommentChanged();
            }
        }
        private String _Comment;
        partial void OnCommentChanging(String value);
        partial void OnCommentChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> ReqStatus
        {
            get
            {
                return _ReqStatus;
            }
            set
            {
                OnReqStatusChanging(value);
                _ReqStatus = value;
                RaisePropertyChanged("ReqStatus");
                OnReqStatusChanged();
            }
        }
        private Nullable<Int64> _ReqStatus;
        partial void OnReqStatusChanging(Nullable<Int64> value);
        partial void OnReqStatusChanged();


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

        [DataMemberAttribute()]
        public Nullable<Boolean> IsApproved
        {
            get
            {
                return _IsApproved;
            }
            set
            {
                OnIsApprovedChanging(value);
                _IsApproved = value;
                RaisePropertyChanged("IsApproved");
                OnIsApprovedChanged();
                RaisePropertyChanged("CanOutward");
                RaisePropertyChanged("CanDelete");
                RaisePropertyChanged("CanSave");
            }
        }
        private Nullable<Boolean> _IsApproved;
        partial void OnIsApprovedChanging(Nullable<Boolean> value);
        partial void OnIsApprovedChanged();

        [DataMemberAttribute()]
        public long? ApprovedStaffID
        {
            get
            {
                return _ApprovedStaffID;
            }
            set
            {
                if (_ApprovedStaffID != value)
                {
                    _ApprovedStaffID = value;
                    RaisePropertyChanged("ApprovedStaffID");
                }
            }
        }
        private long? _ApprovedStaffID;

        [DataMemberAttribute()]
        public DateTime? ApprovedDate
        {
            get
            {
                return _ApprovedDate;
            }
            set
            {
                if (_ApprovedDate != value)
                {
                    _ApprovedDate = value;
                    RaisePropertyChanged("ApprovedDate");
                }
            }
        }
        private DateTime? _ApprovedDate;

        [DataMemberAttribute()]
        public DateTime? FromDate
        {
            get
            {
                return _FromDate;
            }
            set
            {
                if (_FromDate != value)
                {
                    _FromDate = value;
                    RaisePropertyChanged("FromDate");
                }
            }
        }
        private DateTime? _FromDate = DateTime.Now;

        [DataMemberAttribute()]
        public DateTime? ToDate
        {
            get
            {
                return _ToDate;
            }
            set
            {
                if (_ToDate != value)
                {
                    _ToDate = value;
                    RaisePropertyChanged("ToDate");
                }
            }
        }
        private DateTime? _ToDate = DateTime.Now;

        [DataMemberAttribute()]
        public bool IsInstructionFuture
        {
            get
            {
                return _IsInstructionFuture;
            }
            set
            {
                if (_IsInstructionFuture != value)
                {
                    _IsInstructionFuture = value;
                    RaisePropertyChanged("IsInstructionFuture");
                }
            }
        }
        private bool _IsInstructionFuture;
        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public ObservableCollection<RequestDrugInwardClinicDeptDetail> RequestDetails
        {
            get
            {
                return _RequestDetails;
            }
            set
            {
                if (_RequestDetails != value)
                {
                    _RequestDetails = value;
                    RaisePropertyChanged("RequestDetails");
                }
            }
        }
        private ObservableCollection<RequestDrugInwardClinicDeptDetail> _RequestDetails;

        [DataMemberAttribute()]
        public ObservableCollection<ReqOutwardDrugClinicDeptPatient> ReqOutwardDetails
        {
            get
            {
                return _ReqOutwardDetails;
            }
            set
            {
                if (_ReqOutwardDetails != value)
                {
                    _ReqOutwardDetails = value;
                    RaisePropertyChanged("ReqOutwardDetails");
                }
            }
        }
        private ObservableCollection<ReqOutwardDrugClinicDeptPatient> _ReqOutwardDetails;

        [DataMemberAttribute()]
        public Staff SelectedStaff
        {
            get
            {
                return _SelectedStaff;
            }
            set
            {
                if (_SelectedStaff != value)
                {
                    _SelectedStaff = value;
                    RaisePropertyChanged("SelectedStaff");
                }
            }
        }
        private Staff _SelectedStaff;

        [DataMemberAttribute()]
        public Staff SelectedStaffAllow
        {
            get
            {
                return _SelectedStaffAllow;
            }
            set
            {
                if (_SelectedStaffAllow != value)
                {
                    _SelectedStaffAllow = value;
                    RaisePropertyChanged("SelectedStaffAllow");
                }
            }
        }
        private Staff _SelectedStaffAllow;

        [DataMemberAttribute()]
        public RefStorageWarehouseLocation OutFromStoreObject
        {
            get
            {
                return _OutFromStoreObject;
            }
            set
            {
                if (_OutFromStoreObject != value)
                {
                    _OutFromStoreObject = value;
                    if (_OutFromStoreObject != null)
                    {
                        _OutFromStoreID = _OutFromStoreObject.StoreID;
                    }
                    RaisePropertyChanged("OutFromStoreObject");
                }
            }
        }
        private RefStorageWarehouseLocation _OutFromStoreObject;


        [DataMemberAttribute()]
        public RefStorageWarehouseLocation InDeptStoreObject
        {
            get
            {
                return _InDeptStoreObject;
            }
            set
            {
                if (_InDeptStoreObject != value)
                {
                    _InDeptStoreObject = value;
                    if (_InDeptStoreObject != null)
                    {
                        _InDeptStoreID = _InDeptStoreObject.StoreID;
                    }
                    RaisePropertyChanged("InDeptStoreObject");
                }
            }
        }
        private RefStorageWarehouseLocation _InDeptStoreObject;
        #endregion


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

        [DataMemberAttribute()]
        public bool? DaNhanHang
        {
            get
            {
                return _DaNhanHang;
            }
            set
            {

                _DaNhanHang = value;
                RaisePropertyChanged("DaNhanHang");
                RaisePropertyChanged("CanSave");
                RaisePropertyChanged("CanDelete");
                RaisePropertyChanged("CanOutward");
                RaisePropertyChanged("CanApprove");
            }
        }
        private bool? _DaNhanHang;

        public bool CanEdit
        {
            get { return ReqDrugInClinicDeptID <= 0; }
        }

        public bool CanSave
        {
            get { return IsApproved.GetValueOrDefault() == false; }
        }

        public bool CanApprove
        {
            get { return ReqDrugInClinicDeptID > 0 && DaNhanHang.GetValueOrDefault() == false; }
        }

        public bool CanDelete
        {
            get { return ReqDrugInClinicDeptID > 0 && IsApproved.GetValueOrDefault() == false; }
        }
        public bool CanOutward
        {
            get { return (DaNhanHang.GetValueOrDefault() == false && IsApproved.GetValueOrDefault() == true); }
        }
        public bool CanPrint
        {
            get { return ReqDrugInClinicDeptID > 0; }
        }
        public bool CanCheckIsInstructionFuture
        {
            get { return ReqDrugInClinicDeptID == 0; }
        }

        #region convert XML member

        public string ConvertDetailsListToXml()
        {
            return ConvertDetailsListToXml(_RequestDetails);
        }
        public string ConvertDetailsListToXml(IEnumerable<RequestDrugInwardClinicDeptDetail> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<RequestDetails>");
                foreach (RequestDrugInwardClinicDeptDetail details in items)
                {
                    int EntityState = (int)details.EntityState;
                    sb.Append("<RecInfo>");

                    sb.AppendFormat("<ReqDrugInDetailID)>{0}</ReqDrugInDetailID)>", details.ReqDrugInDetailID);
                    sb.AppendFormat("<ReqDrugInClinicDeptID>{0}</ReqDrugInClinicDeptID>", details.ReqDrugInClinicDeptID);
                    sb.AppendFormat("<DrugID>{0}</DrugID>", details.GenMedProductID);
                    sb.AppendFormat("<Qty>{0}</Qty>", details.Qty);
                    sb.AppendFormat("<Note>{0}</Note>", details.Note);
                    sb.AppendFormat("<EntityState>{0}</EntityState>", EntityState);
                    sb.Append("</RecInfo>");
                }
                sb.Append("</RequestDetails>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }

        public string ConvertOutwardDetailsListToXml()
        {
            return ConvertOutwardDetailsListToXml(_ReqOutwardDetails);
        }
        public string ConvertOutwardDetailsListToXml(IEnumerable<ReqOutwardDrugClinicDeptPatient> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<RequestDetails>");
                foreach (ReqOutwardDrugClinicDeptPatient details in items)
                {
                    int EntityState = (int)details.EntityState;
                    sb.Append("<RecInfo>");

                    sb.AppendFormat("<OutClinicDeptReqID>{0}</OutClinicDeptReqID>", details.OutClinicDeptReqID);
                    sb.AppendFormat("<ReqDrugInClinicDeptID>{0}</ReqDrugInClinicDeptID>", details.ReqDrugInClinicDeptID);
                    sb.AppendFormat("<GenMedProductID>{0}</GenMedProductID>", details.GenMedProductID);
                    sb.AppendFormat("<PrescribedQty>{0}</PrescribedQty>", details.PrescribedQty);
                    sb.AppendFormat("<ReqQty>{0}</ReqQty>", details.ReqQty);
                    sb.AppendFormat("<ReqQtyStr>{0}</ReqQtyStr>", details.ReqQtyStr);
                    sb.AppendFormat("<ApprovedQty>{0}</ApprovedQty>", details.ApprovedQty);
                    //----- DPT  fix kí tự dặc biệt trong ghi chú
                    //sb.AppendFormat("<Notes>{0}</Notes>", details.Notes);
                    if (details.Notes != null)
                    {
                        sb.AppendFormat("<Notes>{0}</Notes>", Globals.GetSafeXMLString(details.Notes));
                    }
                    else
                    {
                        sb.AppendFormat("<Notes>{0}</Notes>", details.Notes);
                    }
                    if (details.ApprovedNotes != null)
                    {
                        sb.AppendFormat("<ApprovedNotes>{0}</ApprovedNotes>", Globals.GetSafeXMLString(details.ApprovedNotes));
                    }
                    else
                    {
                        sb.AppendFormat("<ApprovedNotes>{0}</ApprovedNotes>", details.ApprovedNotes);
                    }
                    //----- DPT

                    sb.AppendFormat("<StaffID>{0}</StaffID>", details.StaffID.GetValueOrDefault(0));
                    sb.AppendFormat("<DateTimeSelection>{0}</DateTimeSelection>", details.DateTimeSelection != null && details.DateTimeSelection != default(DateTime) ? details.DateTimeSelection.ToString("dd-MM-yyyy HH:mm:ss") : null);
                    sb.AppendFormat("<ItemVerfStat>{0}</ItemVerfStat>", details.ItemVerfStat.ToString());
                    if (details.CurPatientRegistration != null && details.OutPtRegistrationID == null)
                    {
                        sb.AppendFormat("<PtRegistrationID>{0}</PtRegistrationID>", details.CurPatientRegistration.PtRegistrationID);
                        sb.AppendFormat("<OutPtRegistrationID>{0}</OutPtRegistrationID>", 0);
                    }
                    else
                    {
                        sb.AppendFormat("<PtRegistrationID>{0}</PtRegistrationID>", 0);
                        sb.AppendFormat("<OutPtRegistrationID>{0}</OutPtRegistrationID>", details.OutPtRegistrationID);
                    }
                   
                    sb.AppendFormat("<MDose>{0}</MDose>", details.MDose);
                    sb.AppendFormat("<ADose>{0}</ADose>", details.ADose);
                    sb.AppendFormat("<EDose>{0}</EDose>", details.EDose);
                    sb.AppendFormat("<NDose>{0}</NDose>", details.NDose);

                    sb.AppendFormat("<MDoseStr>{0}</MDoseStr>", details.MDoseStr);
                    sb.AppendFormat("<ADoseStr>{0}</ADoseStr>", details.ADoseStr);
                    sb.AppendFormat("<EDoseStr>{0}</EDoseStr>", details.EDoseStr);
                    sb.AppendFormat("<NDoseStr>{0}</NDoseStr>", details.NDoseStr);

                    sb.AppendFormat("<DoctorStaffID>{0}</DoctorStaffID>", details.DoctorStaff != null ? details.DoctorStaff.StaffID : 0);
                    sb.AppendFormat("<MedicalInstructionDate>{0}</MedicalInstructionDate>", details.MedicalInstructionDate.HasValue ? details.MedicalInstructionDate.Value.ToString("dd-MM-yyyy HH:mm:ss") : null);
                    sb.AppendFormat("<IntPtDiagDrInstructionID>{0}</IntPtDiagDrInstructionID>", details.IntPtDiagDrInstructionID);
                    sb.AppendFormat("<EntityState>{0}</EntityState>", EntityState);
                    sb.Append("</RecInfo>");
                }
                sb.Append("</RequestDetails>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }

        #endregion
    }

    public partial class ReqOutwardDrugClinicDeptPatient : EntityBase, IDosage
    {
        #region Factory Method


        /// Create a new RequestDrugInwardClinicDeptDetail object.

        /// <param name="OutClinicDeptReqID">Initial value of the OutClinicDeptReqID property.</param>
        /// <param name="reqDrugInClinicDeptID">Initial value of the ReqDrugInClinicDeptID property.</param>
        /// <param name="drugID">Initial value of the DrugID property.</param>
        /// <param name="ReqQty">Initial value of the ReqQty property.</param>
        //public static ReqOutwardDrugClinicDeptPatient CreateReqOutwardDrugClinicDeptPatient(Int64 OutClinicDeptReqID, Int64 reqDrugInClinicDeptID, Int32 ReqQty)
        //{
        //    ReqOutwardDrugClinicDeptPatient requestDrugInwardClinicDeptDetail = new ReqOutwardDrugClinicDeptPatient();
        //    requestDrugInwardClinicDeptDetail.OutClinicDeptReqID = OutClinicDeptReqID;
        //    requestDrugInwardClinicDeptDetail.ReqDrugInClinicDeptID = reqDrugInClinicDeptID;
        //    requestDrugInwardClinicDeptDetail.ReqQty = ReqQty;
        //    return requestDrugInwardClinicDeptDetail;
        //}

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 OutClinicDeptReqID
        {
            get
            {
                return _OutClinicDeptReqID;
            }
            set
            {
                if (_OutClinicDeptReqID != value)
                {
                    OnOutClinicDeptReqIDChanging(value);
                    _OutClinicDeptReqID = value;
                    RaisePropertyChanged("OutClinicDeptReqID");
                    OnOutClinicDeptReqIDChanged();
                }
            }
        }
        private Int64 _OutClinicDeptReqID;
        partial void OnOutClinicDeptReqIDChanging(Int64 value);
        partial void OnOutClinicDeptReqIDChanged();

        [DataMemberAttribute()]
        public Int64 ReqDrugInClinicDeptID
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
        private Int64 _ReqDrugInClinicDeptID;
        partial void OnReqDrugInClinicDeptIDChanging(Int64 value);
        partial void OnReqDrugInClinicDeptIDChanged();

        [Required(ErrorMessage = "Bạn phải nhập hàng vào")]
        [DataMemberAttribute()]
        public Nullable<long> GenMedProductID
        {
            get
            {
                return _GenMedProductID;
            }
            set
            {
                if (_GenMedProductID != value)
                {
                    OnDrugIDChanging(value);
                    ValidateProperty("GenMedProductID", value);
                    _GenMedProductID = value;
                    RaisePropertyChanged("GenMedProductID");
                    OnDrugIDChanged();
                }
            }
        }
        private Nullable<long> _GenMedProductID;
        partial void OnDrugIDChanging(Nullable<long> value);
        partial void OnDrugIDChanged();


        //Huyen: added some property for reader when executed stored Procedure spRpt_ClinicDept_InOutStocks to get remainingQty
        [DataMemberAttribute()]
        public decimal RemainingQty
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
        private decimal _RemainingQty;

        [Range(0.0, 99999999999.0, ErrorMessage = "Số lượng Chỉ Định cho bệnh nhân không được nhỏ hơn 0")]
        [DataMemberAttribute()]
        public decimal PrescribedQty
        {
            get
            {
                return _PrescribedQty;
            }
            set
            {
                OnPrescribedQtyChanging(value);
                ValidateProperty("PrescribedQty", value);
                _PrescribedQty = value;
                RaisePropertyChanged("PrescribedQty");
                OnPrescribedQtyChanged();

            }
        }
        private decimal _PrescribedQty;
        partial void OnPrescribedQtyChanging(decimal value);
        partial void OnPrescribedQtyChanged();

        [DataMemberAttribute()]
        public Int16 ItemVerfStat
        {
            get { return _ItemVerfStat; }
            set
            {
                OnItemVerfStatChanging(value);
                ValidateProperty("ItemVerfStat", value);
                _ItemVerfStat = value;
                RaisePropertyChanged("ItemVerfStat");
                OnItemVerfStatChanged();
            }
        }
        private Int16 _ItemVerfStat = 0;
        partial void OnItemVerfStatChanging(decimal value);
        partial void OnItemVerfStatChanged();


        [Range(0.0, 99999999999.0, ErrorMessage = "Số lượng yêu cầu không được nhỏ hơn 0")]
        [DataMemberAttribute()]
        public decimal ReqQty
        {
            get
            {
                return _ReqQty;
            }
            set
            {
                OnReqQtyChanging(value);
                ValidateProperty("ReqQty", value);
                _ReqQty = value;
                RaisePropertyChanged("ReqQty");
                OnReqQtyChanged();

            }
        }
        private decimal _ReqQty;
        partial void OnReqQtyChanging(decimal value);
        partial void OnReqQtyChanged();

        [DataMemberAttribute()]
        public string ReqQtyStr
        {
            get
            {
                return _ReqQtyStr;
            }
            set
            {
                if (_ReqQtyStr != value)
                {
                    _ReqQtyStr = value;
                    RaisePropertyChanged("ReqQtyStr");
                }
            }
        }
        private string _ReqQtyStr = "0";

        [Range(0.0, 99999999999.0, ErrorMessage = "Số lượng không được nhỏ hơn 0")]
        [DataMemberAttribute()]
        public decimal ApprovedQty
        {
            get
            {
                return _ApprovedQty;
            }
            set
            {
                OnApprovedQtyChanging(value);
                ValidateProperty("ApprovedQty", value);
                _ApprovedQty = value;
                RaisePropertyChanged("ApprovedQty");
                OnApprovedQtyChanged();

            }
        }
        private decimal _ApprovedQty;
        partial void OnApprovedQtyChanging(decimal value);
        partial void OnApprovedQtyChanged();

        [DataMemberAttribute()]
        public String Notes
        {
            get
            {
                return _Notes;
            }
            set
            {
                OnNoteChanging(value);
                //ReportPropertyChanging("Note");
                _Notes = value;
                RaisePropertyChanged("Notes");
                OnNoteChanged();
                if (OrgNotes != null && OrgNotes != _Notes)
                {
                    this.RecordState = RecordState.MODIFIED;
                }
            }
        }
        private String _Notes;
        partial void OnNoteChanging(String value);
        partial void OnNoteChanged();

        [DataMemberAttribute()]
        public String ApprovedNotes
        {
            get
            {
                return _ApprovedNotes;
            }
            set
            {
                _ApprovedNotes = value;
                RaisePropertyChanged("ApprovedNotes");
            }
        }
        private String _ApprovedNotes;

        [DataMemberAttribute()]
        public long? PtRegistrationID
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
        private long? _PtRegistrationID;
        partial void OnPtRegistrationIDChanging(long? value);
        partial void OnPtRegistrationIDChanged();

        [DataMemberAttribute()]
        public long? OutPtRegistrationID
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
        private long? _OutPtRegistrationID;

        [DataMemberAttribute()]
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
        private long? _StaffID;

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

        #endregion

        #region Navigation Properties

        private RefGenMedProductDetails _RefGenericDrugDetail;
        [DataMemberAttribute()]
        public RefGenMedProductDetails RefGenericDrugDetail
        {
            get
            {
                return _RefGenericDrugDetail;
            }
            set
            {
                if (_RefGenericDrugDetail != value)
                {
                    _RefGenericDrugDetail = value;
                    if (_RefGenericDrugDetail != null)
                    {
                        _GenMedProductID = _RefGenericDrugDetail.GenMedProductID;
                    }
                    else
                    {
                        _GenMedProductID = null;
                    }
                    RaisePropertyChanged("RefGenericDrugDetail");
                }
            }
        }

        private PatientRegistration _CurPatientRegistration;
        [DataMemberAttribute()]
        public PatientRegistration CurPatientRegistration
        {
            get
            {
                return _CurPatientRegistration;
            }
            set
            {
                if (_CurPatientRegistration != value)
                {
                    _CurPatientRegistration = value;
                    RaisePropertyChanged("CurPatientRegistration");
                }
            }
        }
        #endregion

        private EntityState _EntityState = EntityState.NEW;
        [DataMemberAttribute()]
        public override EntityState EntityState
        {
            get
            {
                return _EntityState;
            }
            set
            {
                _EntityState = value;
                RaisePropertyChanged("EntityState");
            }
        }

        #region extension
        [DataMemberAttribute()]
        public int QtyOutward
        {
            get
            {
                return _QtyOutward;
            }
            set
            {
                _QtyOutward = value;
                RaisePropertyChanged("QtyOutward");
            }
        }
        private int _QtyOutward;

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

        [DataMemberAttribute()]
        public bool ItemVerified
        {
            get
            {
                return _ItemVerified;
            }
            set
            {
                _ItemVerified = value;
                RaisePropertyChanged("ItemVerified");
            }
        }
        private bool _ItemVerified;

        [DataMemberAttribute()]
        public int QtyRemaining
        {
            get
            {
                return _QtyRemaining;
            }
            set
            {
                _QtyRemaining = value;
                RaisePropertyChanged("QtyRemaining");
            }
        }
        private int _QtyRemaining;
        #endregion

        [DataMemberAttribute()]
        public DateTime DateTimeSelection
        {
            get
            {
                return _dateTimeSelection;
            }
            set
            {
                _dateTimeSelection = value;
                RaisePropertyChanged("DateTimeSelection");
            }
        }
        private DateTime _dateTimeSelection;

        private int _DisplayGridRowNumber = 0;
        public int DisplayGridRowNumber
        {
            get { return _DisplayGridRowNumber; }
            set
            {
                _DisplayGridRowNumber = value;
                RaisePropertyChanged("DisplayGridRowNumber");
            }
        }


        [DataMemberAttribute()]
        public string MDoseStr
        {
            get
            {
                return _MDoseStr;
            }
            set
            {
                if (_MDoseStr != value)
                {
                    _MDoseStr = value;
                    RaisePropertyChanged("MDoseStr");
                }
            }
        }
        private string _MDoseStr = "0";


        [DataMemberAttribute()]
        public string ADoseStr
        {
            get
            {
                return _ADoseStr;
            }
            set
            {
                if (_ADoseStr != value)
                {
                    _ADoseStr = value;
                    RaisePropertyChanged("ADoseStr");
                }
            }
        }
        private string _ADoseStr = "0";

        [DataMemberAttribute()]
        public string EDoseStr
        {
            get
            {
                return _EDoseStr;
            }
            set
            {
                if (_EDoseStr != value)
                {
                    _EDoseStr = value;
                    RaisePropertyChanged("EDoseStr");
                }
            }
        }
        private string _EDoseStr = "0";

        [DataMemberAttribute()]
        public string NDoseStr
        {
            get
            {
                return _NDoseStr;
            }
            set
            {
                if (_NDoseStr != value)
                {
                    _NDoseStr = value;
                    RaisePropertyChanged("NDoseStr");
                }
            }
        }
        private string _NDoseStr = "0";


        [DataMemberAttribute()]
        public Single MDose
        {
            get
            {
                return _MDose;
            }
            set
            {
                _MDose = value;
                RaisePropertyChanged("MDose");
                RaisePropertyChanged("IsHashChanged");
            }
        }
        private Single _MDose;

        [DataMemberAttribute()]
        public Single ADose
        {
            get
            {
                return _ADose;
            }
            set
            {
                _ADose = value;
                RaisePropertyChanged("ADose");
                RaisePropertyChanged("IsHashChanged");
            }
        }
        private Single _ADose;

        [DataMemberAttribute()]
        public Single EDose
        {
            get
            {
                return _EDose;
            }
            set
            {
                _EDose = value;
                RaisePropertyChanged("EDose");
                RaisePropertyChanged("IsHashChanged");
            }
        }
        private Single _EDose;

        [DataMemberAttribute()]
        public Single NDose
        {
            get
            {
                return _NDose;
            }
            set
            {
                _NDose = value;
                RaisePropertyChanged("NDose");
                RaisePropertyChanged("IsHashChanged");
            }
        }
        private Single _NDose;

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
        //==== #001
        [DataMemberAttribute()]
        public long? IntravenousPlan_InPtID
        {
            get
            {
                return _IntravenousPlan_InPtID;
            }
            set
            {
                _IntravenousPlan_InPtID = value;
                RaisePropertyChanged("IntravenousPlan_InPtID");
            }
        }
        private long? _IntravenousPlan_InPtID;
        [DataMemberAttribute()]
        public long? IntPtDiagDrInstructionID
        {
            get
            {
                return _IntPtDiagDrInstructionID;
            }
            set
            {
                _IntPtDiagDrInstructionID = value;
                RaisePropertyChanged("IntPtDiagDrInstructionID");
            }
        }
        private long? _IntPtDiagDrInstructionID;
        private string _IDAndInfusionProcessType;
        [DataMemberAttribute()]
        public string IDAndInfusionProcessType
        {
            get
            {
                return _IDAndInfusionProcessType;
            }
            set
            {
                _IDAndInfusionProcessType = value;
                RaisePropertyChanged("IDAndInfusionProcessType");
            }
        }
        //==== #001
        private bool _IsDeleted = false;
        [DataMemberAttribute()]
        public bool IsDeleted
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

        private RecordState _RecordState = RecordState.UNCHANGED;
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

        private string _OrgNotes = null;
        [DataMemberAttribute()]
        public string OrgNotes
        {
            get
            {
                return _OrgNotes;
            }
            set
            {
                _OrgNotes = value;
                RaisePropertyChanged("OrgNotes");
            }
        }

        private long? _outiID;
        [DataMemberAttribute]
        public long? outiID
        {
            get
            {
                return _outiID;
            }
            set
            {
                if (_outiID == value)
                {
                    return;
                }
                _outiID = value;
                RaisePropertyChanged("outiID");
            }
        }


        private string _NumOfTimes = "";
        private string _FlowRate;
        private DateTime? _StartDateTime;
        private int _GroupOrdinal = 0;
        [DataMemberAttribute]
        public string NumOfTimes
        {
            get
            {
                return _NumOfTimes;
            }
            set
            {
                if (_NumOfTimes == value)
                {
                    return;
                }
                _NumOfTimes = value;
                RaisePropertyChanged("NumOfTimes");
            }
        }
        [DataMemberAttribute]
        public string FlowRate
        {
            get
            {
                return _FlowRate;
            }
            set
            {
                if (_FlowRate == value)
                {
                    return;
                }
                _FlowRate = value;
                RaisePropertyChanged("FlowRate");
            }
        }
        [DataMemberAttribute]
        public DateTime? StartDateTime
        {
            get
            {
                return _StartDateTime;
            }
            set
            {
                if (_StartDateTime == value)
                {
                    return;
                }
                _StartDateTime = value;
                RaisePropertyChanged("StartDateTime");
                RaisePropertyChanged("IsHasStartDateTime");
            }
        }
        [DataMemberAttribute]
        public int GroupOrdinal
        {
            get
            {
                return _GroupOrdinal;
            }
            set
            {
                if (_GroupOrdinal == value)
                {
                    return;
                }
                _GroupOrdinal = value;
                RaisePropertyChanged("GroupOrdinal");
            }
        }
        public bool IsHasStartDateTime
        {
            get
            {
                return StartDateTime.HasValue && StartDateTime != null && StartDateTime.Value >= new DateTime(2010, 01, 01);
            }
        }
        private AntibioticTreatment _CurrentAntibioticTreatment;
        [DataMemberAttribute]
        public AntibioticTreatment CurrentAntibioticTreatment
        {
            get
            {
                return _CurrentAntibioticTreatment;
            }
            set
            {
                if (_CurrentAntibioticTreatment == value)
                {
                    return;
                }
                _CurrentAntibioticTreatment = value;
                RaisePropertyChanged("CurrentAntibioticTreatment");
                RaisePropertyChanged("AntibioticTreatmentID");
            }
        }
        public long? AntibioticTreatmentID
        {
            get
            {
                return CurrentAntibioticTreatment == null ? (long?)null : CurrentAntibioticTreatment.AntibioticTreatmentID;
            }
        }
        private int? _AntibioticOrdinal;
        [DataMemberAttribute]
        public int? AntibioticOrdinal
        {
            get
            {
                return _AntibioticOrdinal;
            }
            set
            {
                if (_AntibioticOrdinal == value)
                {
                    return;
                }
                _AntibioticOrdinal = value;
                RaisePropertyChanged("AntibioticOrdinal");
            }
        }

        private bool _IsReplaceMedMat;
        [DataMemberAttribute]
        public bool IsReplaceMedMat
        {
            get
            {
                return _IsReplaceMedMat;
            }
            set
            {
                if (_IsReplaceMedMat == value)
                {
                    return;
                }
                _IsReplaceMedMat = value;
                RaisePropertyChanged("IsReplaceMedMat");
            }
        }

        private bool _IsDisposeMedMat;
        [DataMemberAttribute]
        public bool IsDisposeMedMat
        {
            get
            {
                return _IsDisposeMedMat;
            }
            set
            {
                if (_IsDisposeMedMat == value)
                {
                    return;
                }
                _IsDisposeMedMat = value;
                RaisePropertyChanged("IsDisposeMedMat");
            }
        }

        private bool _IsCreatedOutward;
        [DataMemberAttribute]
        public bool IsCreatedOutward
        {
            get
            {
                return _IsCreatedOutward;
            }
            set
            {
                if (_IsCreatedOutward == value)
                {
                    return;
                }
                _IsCreatedOutward = value;
                RaisePropertyChanged("IsCreatedOutward");
            }
        }

        private string _UsageDistance;
        [DataMemberAttribute]
        public string UsageDistance
        {
            get
            {
                return _UsageDistance;
            }
            set
            {
                if (_UsageDistance == value)
                {
                    return;
                }
                _UsageDistance = value;
                RaisePropertyChanged("UsageDistance");
            }
        }
        private long _V_RouteOfAdministration;
        [DataMemberAttribute]
        public long V_RouteOfAdministration
        {
            get
            {
                return _V_RouteOfAdministration;
            }
            set
            {
                if (_V_RouteOfAdministration == value)
                {
                    return;
                }
                _V_RouteOfAdministration = value;
                //IsTruyenTinhMach = V_RouteOfAdministration == 61319 ? true  : false;
                RaisePropertyChanged("V_RouteOfAdministration");
            }
        }
        private int _TransferRate;
        [DataMemberAttribute]
        public int TransferRate
        {
            get
            {
                return _TransferRate;
            }
            set
            {
                if (_TransferRate == value)
                {
                    return;
                }
                _TransferRate = value;
                RaisePropertyChanged("TransferRate");
            }
        }
        private long _V_TransferRateUnit;
        [DataMemberAttribute]
        public long V_TransferRateUnit
        {
            get
            {
                return _V_TransferRateUnit;
            }
            set
            {
                if (_V_TransferRateUnit == value)
                {
                    return;
                }
                _V_TransferRateUnit = value;
                RaisePropertyChanged("V_TransferRateUnit");
            }
        }
        [DataMemberAttribute()]
        public bool IsTruyenTinhMach
        {
            get
            {
                return _IsTruyenTinhMach;
            }
            set
            {

                _IsTruyenTinhMach = value;
                RaisePropertyChanged("IsTruyenTinhMach");
            }
        }
        private bool _IsTruyenTinhMach;

        private int? CurrentHash = null;
        private int GetRecordEditableHash()
        {
            return ((int)MDose).GetHashCode() 
                ^ ((int)ADose).GetHashCode() 
                ^ ((int)EDose).GetHashCode() 
                ^ ((int)NDose).GetHashCode() 
                ^ ((long)V_RouteOfAdministration).GetHashCode()
                ^ ((long)V_TransferRateUnit).GetHashCode()
                ^ ((int)TransferRate).GetHashCode()
                ^ ((string)UsageDistance).GetHashCode();
        }
        public void BackupCurrentHash()
        {
            CurrentHash = this.GetRecordEditableHash();
        }
        public bool IsHashChanged
        {
            get
            {
                return CurrentHash != null && CurrentHash != this.GetRecordEditableHash();
            }
        }

        private long _ReqForTechID;
        [DataMemberAttribute()]
        public long ReqForTechID
        {
            get
            {
                return _ReqForTechID;
            }
            set
            {

                _ReqForTechID = value;
                RaisePropertyChanged("ReqForTechID");
            }
        }
    }

    public partial class RequestDrugForTechnicalService : NotifyChangedBase
    {
        public static RequestDrugForTechnicalService CreateRequestDrugForTechnicalService(Int64 ReqForTechID, Int64 staffID, DateTime reqDate)
        {
            RequestDrugForTechnicalService RequestDrugForTechnicalService = new RequestDrugForTechnicalService();
            RequestDrugForTechnicalService.ReqForTechID = ReqForTechID;
            RequestDrugForTechnicalService.StaffID = staffID;
            RequestDrugForTechnicalService.ReqDate = reqDate;
            return RequestDrugForTechnicalService;
        }

        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 ReqForTechID
        {
            get
            {
                return _ReqForTechID;
            }
            set
            {
                if (_ReqForTechID != value)
                {
                    OnReqForTechIDChanging(value);
                    _ReqForTechID = value;
                    RaisePropertyChanged("ReqForTechID");
                    OnReqForTechIDChanged();

                    RaisePropertyChanged("CanEdit");
                    RaisePropertyChanged("CanSave");
                    RaisePropertyChanged("CanDelete");
                    RaisePropertyChanged("CanPrint");
                }
            }
        }       

        private Int64 _ReqForTechID;

        partial void OnReqForTechIDChanging(Int64 value);
        partial void OnReqForTechIDChanged();

        private long _PtRegDetailID;
        public long PtRegDetailID
        {
            get
            {
                return _PtRegDetailID;
            }
            set
            {
                if (_PtRegDetailID == value)
                {
                    return;
                }
                _PtRegDetailID = value;
                RaisePropertyChanged("PtRegDetailID");
            }
        }

        [DataMemberAttribute()]
        public Int64 StaffID
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
        private Int64 _StaffID;
        partial void OnStaffIDChanging(Int64 value);
        partial void OnStaffIDChanged();

        [DataMemberAttribute()]
        public DateTime ReqDate
        {
            get
            {
                return _ReqDate;
            }
            set
            {
                OnReqDateChanging(value);
                _ReqDate = value;
                RaisePropertyChanged("ReqDate");
                OnReqDateChanged();
            }
        }
        private DateTime _ReqDate = DateTime.Now;
        partial void OnReqDateChanging(DateTime value);
        partial void OnReqDateChanged();    

        [DataMemberAttribute()]
        public Nullable<Int64> OutFromStoreID
        {
            get
            {
                return _OutFromStoreID;
            }
            set
            {
                OnOutFromStoreIDChanging(value);
                _OutFromStoreID = value;
                RaisePropertyChanged("OutFromStoreID");
                OnOutFromStoreIDChanged();
            }
        }
        private Nullable<Int64> _OutFromStoreID;
        partial void OnOutFromStoreIDChanging(Nullable<Int64> value);
        partial void OnOutFromStoreIDChanged();


        [DataMemberAttribute()]
        public Nullable<Int64> InDeptStoreID
        {
            get
            {
                return _InDeptStoreID;
            }
            set
            {
                OnInDeptStoreIDChanging(value);
                _InDeptStoreID = value;
                RaisePropertyChanged("InDeptStoreID");
                OnInDeptStoreIDChanged();
            }
        }
        private Nullable<Int64> _InDeptStoreID;
        partial void OnInDeptStoreIDChanging(Nullable<Int64> value);
        partial void OnInDeptStoreIDChanged();


        [DataMemberAttribute()]
        public String Comment
        {
            get
            {
                return _Comment;
            }
            set
            {
                OnCommentChanging(value);
                _Comment = value;
                RaisePropertyChanged("Comment");
                OnCommentChanged();
            }
        }
        private String _Comment;
        partial void OnCommentChanging(String value);
        partial void OnCommentChanged();

        [DataMemberAttribute()]
        public bool IsLock
        {
            get
            {
                return _IsLock;
            }
            set
            {
                OnIsLockChanging(value);
                _IsLock = value;
                RaisePropertyChanged("IsLock");
                OnIsLockChanged();
            }
        }
        private bool _IsLock;
        partial void OnIsLockChanging(bool value);
        partial void OnIsLockChanged();

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

        #endregion

        #region Navigation Properties       

        [DataMemberAttribute()]
        public ObservableCollection<ReqOutwardDrugClinicDeptPatient> ReqOutwardDetails
        {
            get
            {
                return _ReqOutwardDetails;
            }
            set
            {
                if (_ReqOutwardDetails != value)
                {
                    _ReqOutwardDetails = value;
                    RaisePropertyChanged("ReqOutwardDetails");
                }
            }
        }
        private ObservableCollection<ReqOutwardDrugClinicDeptPatient> _ReqOutwardDetails;       

        [DataMemberAttribute()]
        public RefStorageWarehouseLocation OutFromStoreObject
        {
            get
            {
                return _OutFromStoreObject;
            }
            set
            {
                if (_OutFromStoreObject != value)
                {
                    _OutFromStoreObject = value;
                    if (_OutFromStoreObject != null)
                    {
                        _OutFromStoreID = _OutFromStoreObject.StoreID;
                    }
                    RaisePropertyChanged("OutFromStoreObject");
                }
            }
        }
        private RefStorageWarehouseLocation _OutFromStoreObject;


        [DataMemberAttribute()]
        public RefStorageWarehouseLocation InDeptStoreObject
        {
            get
            {
                return _InDeptStoreObject;
            }
            set
            {
                if (_InDeptStoreObject != value)
                {
                    _InDeptStoreObject = value;
                    if (_InDeptStoreObject != null)
                    {
                        _InDeptStoreID = _InDeptStoreObject.StoreID;
                    }
                    RaisePropertyChanged("InDeptStoreObject");
                }
            }
        }
        private RefStorageWarehouseLocation _InDeptStoreObject;
        #endregion     

        public bool CanEdit
        {
            get { return ReqForTechID <= 0; }
        }

        public bool CanSave
        {
            get { return IsLock == false; }
        }

        public bool CanDelete
        {
            get { return ReqForTechID > 0 && IsLock == false; }
        }

        public bool CanPrint
        {
            get { return ReqForTechID > 0; }
        }

        #region convert XML member     

        public string ConvertOutwardDetailsListToXml()
        {
            return ConvertOutwardDetailsListToXml(_ReqOutwardDetails);
        }
        public string ConvertOutwardDetailsListToXml(IEnumerable<ReqOutwardDrugClinicDeptPatient> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<RequestDetails>");
                foreach (ReqOutwardDrugClinicDeptPatient details in items)
                {
                    int EntityState = (int)details.EntityState;
                    sb.Append("<RecInfo>");

                    sb.AppendFormat("<OutClinicDeptReqID>{0}</OutClinicDeptReqID>", details.OutClinicDeptReqID);
                    sb.AppendFormat("<ReqForTechID>{0}</ReqForTechID>", details.ReqForTechID);
                    sb.AppendFormat("<GenMedProductID>{0}</GenMedProductID>", details.GenMedProductID);
                    sb.AppendFormat("<PrescribedQty>{0}</PrescribedQty>", details.PrescribedQty);
                    sb.AppendFormat("<ReqQty>{0}</ReqQty>", details.ReqQty);
                    sb.AppendFormat("<ReqQtyStr>{0}</ReqQtyStr>", details.ReqQtyStr);
                    sb.AppendFormat("<ApprovedQty>{0}</ApprovedQty>", details.ApprovedQty);
                    if (details.Notes != null)
                    {
                        sb.AppendFormat("<Notes>{0}</Notes>", Globals.GetSafeXMLString(details.Notes));
                    }
                    else
                    {
                        sb.AppendFormat("<Notes>{0}</Notes>", details.Notes);
                    }
                    if (details.ApprovedNotes != null)
                    {
                        sb.AppendFormat("<ApprovedNotes>{0}</ApprovedNotes>", Globals.GetSafeXMLString(details.ApprovedNotes));
                    }
                    else
                    {
                        sb.AppendFormat("<ApprovedNotes>{0}</ApprovedNotes>", details.ApprovedNotes);
                    }

                    sb.AppendFormat("<StaffID>{0}</StaffID>", details.StaffID.GetValueOrDefault(0));
                    sb.AppendFormat("<DateTimeSelection>{0}</DateTimeSelection>", details.DateTimeSelection != null && details.DateTimeSelection != default(DateTime) ? details.DateTimeSelection.ToString("dd-MM-yyyy HH:mm:ss") : null);
                    sb.AppendFormat("<ItemVerfStat>{0}</ItemVerfStat>", details.ItemVerfStat.ToString());
                    if (details.CurPatientRegistration != null && details.OutPtRegistrationID == null)
                    {
                        sb.AppendFormat("<PtRegistrationID>{0}</PtRegistrationID>", details.CurPatientRegistration.PtRegistrationID);
                        sb.AppendFormat("<OutPtRegistrationID>{0}</OutPtRegistrationID>", 0);
                    }
                    else
                    {
                        sb.AppendFormat("<PtRegistrationID>{0}</PtRegistrationID>", 0);
                        sb.AppendFormat("<OutPtRegistrationID>{0}</OutPtRegistrationID>", details.OutPtRegistrationID);
                    }

                    sb.AppendFormat("<MDose>{0}</MDose>", details.MDose);
                    sb.AppendFormat("<ADose>{0}</ADose>", details.ADose);
                    sb.AppendFormat("<EDose>{0}</EDose>", details.EDose);
                    sb.AppendFormat("<NDose>{0}</NDose>", details.NDose);

                    sb.AppendFormat("<MDoseStr>{0}</MDoseStr>", details.MDoseStr);
                    sb.AppendFormat("<ADoseStr>{0}</ADoseStr>", details.ADoseStr);
                    sb.AppendFormat("<EDoseStr>{0}</EDoseStr>", details.EDoseStr);
                    sb.AppendFormat("<NDoseStr>{0}</NDoseStr>", details.NDoseStr);

                    sb.AppendFormat("<DoctorStaffID>{0}</DoctorStaffID>", details.DoctorStaff != null ? details.DoctorStaff.StaffID : 0);
                    sb.AppendFormat("<MedicalInstructionDate>{0}</MedicalInstructionDate>", details.MedicalInstructionDate.HasValue ? details.MedicalInstructionDate.Value.ToString("dd-MM-yyyy HH:mm:ss") : null);
                    sb.AppendFormat("<IntPtDiagDrInstructionID>{0}</IntPtDiagDrInstructionID>", details.IntPtDiagDrInstructionID);
                    sb.AppendFormat("<EntityState>{0}</EntityState>", EntityState);
                    sb.Append("</RecInfo>");
                }
                sb.Append("</RequestDetails>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }
        #endregion       
    }
}