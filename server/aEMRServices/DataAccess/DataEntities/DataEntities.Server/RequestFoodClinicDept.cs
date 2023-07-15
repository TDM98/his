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
    public partial class RequestFoodClinicDept : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new RequestFoodClinicDept object.
        public static RequestFoodClinicDept CreateRequestFoodClinicDept(long ReqFoodClinicDeptID, long staffID, DateTime reqDate)
        {
            RequestFoodClinicDept RequestFoodClinicDept = new RequestFoodClinicDept
            {
                ReqFoodClinicDeptID = ReqFoodClinicDeptID,
                StaffID = staffID,
                ReqDate = reqDate
            };
            return RequestFoodClinicDept;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public long ReqFoodClinicDeptID
        {
            get
            {
                return _ReqFoodClinicDeptID;
            }
            set
            {
                if (_ReqFoodClinicDeptID != value)
                {
                    _ReqFoodClinicDeptID = value;
                    RaisePropertyChanged("ReqFoodClinicDeptID");
                }
            }
        }
        private long _ReqFoodClinicDeptID;

        [DataMemberAttribute()]
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
        private long _StaffID;

        [DataMemberAttribute()]
        public long DeptID
        {
            get
            {
                return _DeptID;
            }
            set
            {
                _DeptID = value;
                RaisePropertyChanged("DeptID");
            }
        }
        private long _DeptID;

        [DataMemberAttribute()]
        public DateTime ReqDate
        {
            get
            {
                return _ReqDate;
            }
            set
            {
                _ReqDate = value;
                RaisePropertyChanged("ReqDate");
            }
        }
        private DateTime _ReqDate = DateTime.Now;

        [DataMemberAttribute()]
        public string ReqNumCode
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
        private string _ReqNumCode;

        [DataMemberAttribute()]
        public string Comment
        {
            get
            {
                return _Comment;
            }
            set
            {
                _Comment = value;
                RaisePropertyChanged("Comment");
            }
        }
        private string _Comment;

        [DataMemberAttribute()]
        public long ReqStatus
        {
            get
            {
                return _ReqStatus;
            }
            set
            {
                _ReqStatus = value;
                RaisePropertyChanged("ReqStatus");
            }
        }
        private long _ReqStatus;


        [DataMemberAttribute()]
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
        private bool _IsApproved;

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
        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public ObservableCollection<ReqFoodClinicDeptDetail> RequestDetails
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
        private ObservableCollection<ReqFoodClinicDeptDetail> _RequestDetails;

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
            }
        }
        private bool? _DaNhanHang;

        #region convert XML member

        public string ConvertFoodDetailsListToXml()
        {
            return ConvertFoodDetailsListToXml(_RequestDetails);
        }

        public string ConvertFoodDetailsListToXml(IEnumerable<ReqFoodClinicDeptDetail> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<RequestDetails>");
                foreach (ReqFoodClinicDeptDetail details in items)
                {
                    int EntityState = (int)details.EntityState;
                    sb.Append("<RecInfo>");
                    sb.AppendFormat("<ReqFoodClinicDeptID>{0}</ReqFoodClinicDeptID>", details.ReqFoodClinicDeptID);
                    sb.AppendFormat("<ItemID>{0}</ItemID>", details.ItemID);
                    sb.AppendFormat("<ReqQty>{0}</ReqQty>", details.ReqQty);
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
                    sb.AppendFormat("<DateTimeSelection>{0}</DateTimeSelection>", details.DateTimeSelection != null && details.DateTimeSelection != default(DateTime) ? details.DateTimeSelection.ToString("dd-MM-yyyy HH:mm:ss") : null);
                    sb.AppendFormat("<UpdatedStaffID>{0}</UpdatedStaffID>", details.UpdateDoctorStaff != null ? details.UpdateDoctorStaff.StaffID : 0);
                    sb.AppendFormat("<ReqFoodClinicDeptDetailLinks>{0}</ReqFoodClinicDeptDetailLinks>", details.ConvertFoodDetailLinkListToXml());
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

    public partial class ReqFoodClinicDeptDetail : EntityBase
    {
        #region Factory Method
        /// Create a new ReqFoodClinicDeptDetail object.
        #endregion

        #region Primitive Properties
        [DataMemberAttribute()]
        public long ReqFoodClinicDeptDetailID
        {
            get
            {
                return _ReqFoodClinicDeptDetailID;
            }
            set
            {
                _ReqFoodClinicDeptDetailID = value;
                RaisePropertyChanged("ReqFoodClinicDeptDetailID");
            }
        }
        private long _ReqFoodClinicDeptDetailID;
        [DataMemberAttribute()]
        public long ReqFoodClinicDeptID
        {
            get
            {
                return _ReqFoodClinicDeptID;
            }
            set
            {
                _ReqFoodClinicDeptID = value;
                RaisePropertyChanged("ReqFoodClinicDeptID");
            }
        }
        private long _ReqFoodClinicDeptID;

        [DataMemberAttribute()]
        public long ItemID
        {
            get
            {
                return _ItemID;
            }
            set
            {
                if (_ItemID != value)
                {
                    _ItemID = value;
                    RaisePropertyChanged("ItemID");
                }
            }
        }
        private long _ItemID;
        [DataMemberAttribute()]
        public string MedServiceName
        {
            get
            {
                return _MedServiceName;
            }
            set
            {
                _MedServiceName = value;
                RaisePropertyChanged("MedServiceName");
            }
        }
        private string _MedServiceName;
        [DataMemberAttribute()]
        public string MedServiceCode
        {
            get
            {
                return _MedServiceCode;
            }
            set
            {
                _MedServiceCode = value;
                RaisePropertyChanged("MedServiceCode");
            }
        }
        private string _MedServiceCode;
        [DataMemberAttribute()]
        public string UnitName
        {
            get
            {
                return _UnitName;
            }
            set
            {
                _UnitName = value;
                RaisePropertyChanged("UnitName");
            }
        }
        private string _UnitName;

        private decimal _Qty;
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
                ValidateProperty("ReqQty", value);
                _ReqQty = value;
                RaisePropertyChanged("ReqQty");
            }
        }
        private decimal _ReqQty;
        [Range(0.0, 99999999999.0, ErrorMessage = "Số lượng yêu cầu không được nhỏ hơn 0")]
        [DataMemberAttribute()]
        public decimal ApprovedQty
        {
            get
            {
                return _ApprovedQty;
            }
            set
            {
                ValidateProperty("ApprovedQty", value);
                _ApprovedQty = value;
                RaisePropertyChanged("ApprovedQty");
            }
        }
        private decimal _ApprovedQty;

        [DataMemberAttribute()]
        public string Notes
        {
            get
            {
                return _Notes;
            }
            set
            {
                OnNoteChanging(value);
                _Notes = value;
                RaisePropertyChanged("Notes");
                OnNoteChanged();
            }
        }
        private string _Notes;
        partial void OnNoteChanging(string value);
        partial void OnNoteChanged();

        [DataMemberAttribute()]
        public string ApprovedNotes
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
        private string _ApprovedNotes;
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

        [DataMemberAttribute()]
        public Staff UpdateDoctorStaff
        {
            get
            {
                return _UpdateDoctorStaff;
            }
            set
            {
                _UpdateDoctorStaff = value;
                RaisePropertyChanged("UpdateDoctorStaff");
            }
        }
        private Staff _UpdateDoctorStaff;

        [DataMemberAttribute()]
        public DateTime? UpdatedDate
        {
            get
            {
                return _UpdatedDate;
            }
            set
            {
                _UpdatedDate = value;
                RaisePropertyChanged("UpdatedDate");
            }
        }
        private DateTime? _UpdatedDate;

        [DataMemberAttribute()]
        public ObservableCollection<ReqFoodClinicDeptDetailLink> ReqFoodClinicDeptDetailLinks
        {
            get
            {
                return _ReqFoodClinicDeptDetailLinks;
            }
            set
            {
                if (_ReqFoodClinicDeptDetailLinks != value)
                {
                    _ReqFoodClinicDeptDetailLinks = value;
                    RaisePropertyChanged("ReqFoodClinicDeptDetailLinks");
                }
            }
        }
        private ObservableCollection<ReqFoodClinicDeptDetailLink> _ReqFoodClinicDeptDetailLinks;

        public string ConvertFoodDetailLinkListToXml()
        {
            return ConvertFoodDetailLinkListToXml(_ReqFoodClinicDeptDetailLinks);
        }

        public string ConvertFoodDetailLinkListToXml(IEnumerable<ReqFoodClinicDeptDetailLink> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<ReqFoodClinicDeptDetailLink>");
                foreach (ReqFoodClinicDeptDetailLink details in items)
                {
                    int EntityState = (int)details.EntityState;
                    sb.AppendFormat("<ReqFoodClinicDeptDetailID>{0}</ReqFoodClinicDeptDetailID>", details.ReqFoodClinicDeptDetailID);
                    sb.AppendFormat("<Qty>{0}</Qty>", details.Qty);
                    sb.AppendFormat("<PtRegDetailID>{0}</PtRegDetailID>", details.PtRegDetailID);
                    sb.AppendFormat("<IntPtDiagDrInstructionID>{0}</IntPtDiagDrInstructionID>", details.IntPtDiagDrInstructionID);
                    sb.AppendFormat("<DoctorStaffID>{0}</DoctorStaffID>", details.DoctorStaff.StaffID);
                    sb.AppendFormat("<MedicalInstructionDate>{0}</MedicalInstructionDate>", details.MedicalInstructionDate.HasValue ? details.MedicalInstructionDate.Value.ToString("dd-MM-yyyy HH:mm:ss") : null);
                    sb.AppendFormat("<IsDeleted>{0}</IsDeleted>", details.IsDeleted);
                    sb.AppendFormat("<EntityState>{0}</EntityState>", EntityState);
                }
                sb.Append("</ReqFoodClinicDeptDetailLink>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }
    }

    public partial class ReqFoodClinicDeptDetailLink : EntityBase
    {
        #region Factory Method
        /// Create a new ReqFoodClinicDeptDetail object.
        #endregion

        #region Primitive Properties
        [DataMemberAttribute()]
        public long ReqFoodClinicDeptDetailLinkID
        {
            get
            {
                return _ReqFoodClinicDeptDetailLinkID;
            }
            set
            {
                _ReqFoodClinicDeptDetailLinkID = value;
                RaisePropertyChanged("ReqFoodClinicDeptDetailLinkID");
            }
        }

        private long _ReqFoodClinicDeptDetailLinkID;
        [DataMemberAttribute()]
        public long ReqFoodClinicDeptDetailID
        {
            get
            {
                return _ReqFoodClinicDeptDetailID;
            }
            set
            {
                _ReqFoodClinicDeptDetailID = value;
                RaisePropertyChanged("ReqFoodClinicDeptDetailID");
            }
        }

        private long _ReqFoodClinicDeptDetailID;
        [DataMemberAttribute()]
        public string MedServiceName
        {
            get
            {
                return _MedServiceName;
            }
            set
            {
                if (_MedServiceName != value)
                {
                    _MedServiceName = value;
                    RaisePropertyChanged("MedServiceName");
                }
            }
        }

        private string _MedServiceName;
        [DataMemberAttribute()]
        public long MedServiceID
        {
            get
            {
                return _MedServiceID;
            }
            set
            {
                if (_MedServiceID != value)
                {
                    _MedServiceID = value;
                    RaisePropertyChanged("MedServiceID");
                }
            }
        }

        private long _MedServiceID;
        [DataMemberAttribute()]
        public long PtRegDetailID
        {
            get
            {
                return _PtRegDetailID;
            }
            set
            {
                if (_PtRegDetailID != value)
                {
                    _PtRegDetailID = value;
                    RaisePropertyChanged("PtRegDetailID");
                }
            }
        }
        private long _PtRegDetailID;

        [Range(0.0, 99999999999.0, ErrorMessage = "Số lượng yêu cầu không được nhỏ hơn 0")]
        [DataMemberAttribute()]
        public decimal Qty
        {
            get
            {
                return _Qty;
            }
            set
            {
                ValidateProperty("Qty", value);
                _Qty = value;
                RaisePropertyChanged("Qty");
            }
        }
        private decimal _Qty;
        #endregion

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
    }
}
