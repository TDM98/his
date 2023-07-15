using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
/*
 * 20171220 #001 CMN: Added properties to view button accept or delete on layout
*/
namespace DataEntities
{
    public partial class InPatientTransferDeptReq : NotifyChangedBase, IEditableObject
    {
        public InPatientTransferDeptReq()
            : base()
        {

        }

        private InPatientTransferDeptReq _tempInPatientTransferDeptReq;
        public override bool Equals(object obj)
        {
            InPatientTransferDeptReq info = obj as InPatientTransferDeptReq;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.InPatientTransferDeptReqID > 0 && this.InPatientTransferDeptReqID == info.InPatientTransferDeptReqID;
        }
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempInPatientTransferDeptReq = (InPatientTransferDeptReq)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempInPatientTransferDeptReq)
                CopyFrom(_tempInPatientTransferDeptReq);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(InPatientTransferDeptReq p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new InPatientTransferDeptReq object.

        /// <param name="bedLocNumber">Initial value of the BedLocNumber property.</param>
        /// <param name="allocationID">Initial value of the AllocationID property.</param>
        public static InPatientTransferDeptReq CreateInPatientTransferDeptReq(String bedLocNumber, long allocationID)
        {
            InPatientTransferDeptReq InPatientTransferDeptReq = new InPatientTransferDeptReq();
            //InPatientTransferDeptReq.BedLocNumber = bedLocNumber;
            //InPatientTransferDeptReq.AllocationID = allocationID;
            return InPatientTransferDeptReq;
        }

        #endregion
        #region Primitive Properties


        [DataMemberAttribute()]
        public long InPatientTransferDeptReqID
        {
            get
            {
                return _InPatientTransferDeptReqID;
            }
            set
            {
                OnInPatientTransferDeptReqIDChanging(value);
                _InPatientTransferDeptReqID = value;
                RaisePropertyChanged("InPatientTransferDeptReqID");
                OnInPatientTransferDeptReqIDChanged();
            }
        }
        private long _InPatientTransferDeptReqID;
        partial void OnInPatientTransferDeptReqIDChanging(long value);
        partial void OnInPatientTransferDeptReqIDChanged();




        [DataMemberAttribute()]
        public long InPatientAdmDisDetailID
        {
            get
            {
                return _InPatientAdmDisDetailID;
            }
            set
            {
                OnInPatientAdmDisDetailIDChanging(value);
                _InPatientAdmDisDetailID = value;
                RaisePropertyChanged("InPatientAdmDisDetailID");
                OnInPatientAdmDisDetailIDChanged();
            }
        }
        private long _InPatientAdmDisDetailID;
        partial void OnInPatientAdmDisDetailIDChanging(long value);
        partial void OnInPatientAdmDisDetailIDChanged();

        [DataMemberAttribute()]
        public long ReqDeptID
        {
            get
            {
                return _ReqDeptID;
            }
            set
            {
                OnReqDeptIDChanging(value);
                _ReqDeptID = value;
                RaisePropertyChanged("ReqDeptID");
                OnReqDeptIDChanged();
            }
        }
        private long _ReqDeptID;
        partial void OnReqDeptIDChanging(long value);
        partial void OnReqDeptIDChanged();


        [DataMemberAttribute()]
        public long ReqDeptLocID
        {
            get
            {
                return _ReqDeptLocID;
            }
            set
            {
                OnReqDeptLocIDChanging(value);
                _ReqDeptLocID = value;
                RaisePropertyChanged("ReqDeptLocID");
                OnReqDeptLocIDChanged();
            }
        }
        private long _ReqDeptLocID;
        partial void OnReqDeptLocIDChanging(long value);
        partial void OnReqDeptLocIDChanged();


        [DataMemberAttribute()]
        public long ReqStaffID
        {
            get
            {
                return _ReqStaffID;
            }
            set
            {
                OnReqStaffIDChanging(value);
                _ReqStaffID = value;
                RaisePropertyChanged("ReqStaffID");
                OnReqStaffIDChanged();
            }
        }
        private long _ReqStaffID;
        partial void OnReqStaffIDChanging(long value);
        partial void OnReqStaffIDChanged();


        [DataMemberAttribute()]
        public Nullable<DateTime> ReqDate
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
        private Nullable<DateTime> _ReqDate;
        partial void OnReqDateChanging(Nullable<DateTime> value);
        partial void OnReqDateChanged();




        [DataMemberAttribute()]
        public bool IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                OnIsDeletedChanging(value);
                _IsDeleted = value;
                RaisePropertyChanged("IsDeleted");
                OnIsDeletedChanged();
            }
        }
        private bool _IsDeleted;
        partial void OnIsDeletedChanging(bool value);
        partial void OnIsDeletedChanged();


        [DataMemberAttribute()]
        public bool IsAccepted
        {
            get
            {
                return _IsAccepted;
            }
            set
            {
                OnIsAcceptedChanging(value);
                _IsAccepted = value;
                RaisePropertyChanged("IsAccepted");
                OnIsAcceptedChanged();
            }
        }
        private bool _IsAccepted;
        partial void OnIsAcceptedChanging(bool value);
        partial void OnIsAcceptedChanged();


        [DataMemberAttribute()]
        public long CurDeptID
        {
            get
            {
                return _CurDeptID;
            }
            set
            {
                OnCurDeptIDChanging(value);
                _CurDeptID = value;
                RaisePropertyChanged("CurDeptID");
                OnCurDeptIDChanged();
            }
        }
        private long _CurDeptID;
        partial void OnCurDeptIDChanging(long value);
        partial void OnCurDeptIDChanged();


        [DataMemberAttribute()]
        public ObservableCollection<long> LstRefDepartment
        {
            get
            {
                return _LstRefDepartment;
            }
            set
            {
                OnLstRefDepartmentChanging(value);
                _LstRefDepartment = value;
                RaisePropertyChanged("LstRefDepartment");
                OnLstRefDepartmentChanged();
            }
        }
        private ObservableCollection<long> _LstRefDepartment;
        partial void OnLstRefDepartmentChanging(ObservableCollection<long> value);
        partial void OnLstRefDepartmentChanged();


        public BedPatientAllocs BedAllocation
        {
            get
            {
                return _bedAllocation;
            }
            set
            {
                OnBedAllocationChanging(value);
                _bedAllocation = value;
                RaisePropertyChanged("BedAllocation");
                OnBedAllocationChanged();
            }
        }
        private BedPatientAllocs _bedAllocation;
        partial void OnBedAllocationChanging(BedPatientAllocs value);
        partial void OnBedAllocationChanged();

        /*▼====: #001*/
        [DataMemberAttribute()]
        public bool IsChangedDept
        {
            get
            {
                return _IsChangedDept;
            }
            set
            {
                _IsChangedDept = value;
                RaisePropertyChanged("IsChangedDept");
                RaisePropertyChanged("IsBothDept");
            }
        }
        private bool _IsChangedDept;

        [DataMemberAttribute()]
        public bool IsReceiveDept
        {
            get
            {
                return _IsReceiveDept;
            }
            set
            {
                _IsReceiveDept = value;
                RaisePropertyChanged("IsReceiveDept");
                RaisePropertyChanged("IsBothDept");
            }
        }
        private bool _IsReceiveDept;

        [DataMemberAttribute()]
        public bool IsBothDept
        {
            get
            {
                return IsChangedDept && IsReceiveDept;
            }
        }
        /*▲====: #001*/
        private bool _IsProgressive;
        [DataMemberAttribute()]
        public bool IsProgressive
        {
            get
            {
                return _IsProgressive;
            }
            set
            {
                _IsProgressive = value;
                RaisePropertyChanged("IsProgressive");
            }
        }
        #endregion

        #region Navigation Properties


        [DataMemberAttribute()]
        public InPatientAdmDisDetails InPatientAdmDisDetails
        {
            get
            {
                return _InPatientAdmDisDetails;
            }
            set
            {
                OnInPatientAdmDisDetailsChanging(value);
                _InPatientAdmDisDetails = value;
                if (_InPatientAdmDisDetails != null)
                {
                    InPatientAdmDisDetailID = InPatientAdmDisDetails.InPatientAdmDisDetailID;
                }
                RaisePropertyChanged("InPatientAdmDisDetails");
                OnInPatientAdmDisDetailsChanged();
            }
        }
        private InPatientAdmDisDetails _InPatientAdmDisDetails;
        partial void OnInPatientAdmDisDetailsChanging(InPatientAdmDisDetails value);
        partial void OnInPatientAdmDisDetailsChanged();

        [DataMemberAttribute()]
        public DeptLocation ReqDeptLoc
        {
            get
            {
                return _ReqDeptLoc;
            }
            set
            {
                OnReqDeptLocChanging(value);
                _ReqDeptLoc = value;
                if (_ReqDeptLoc != null)
                {
                    ReqDeptLocID = ReqDeptLoc.DeptLocationID;
                }
                RaisePropertyChanged("ReqDeptLoc");
                OnReqDeptLocChanged();
            }
        }
        private DeptLocation _ReqDeptLoc;
        partial void OnReqDeptLocChanging(DeptLocation value);
        partial void OnReqDeptLocChanged();


        [DataMemberAttribute()]
        public Staff reqStaff
        {
            get
            {
                return _reqStaff;
            }
            set
            {
                OnreqStaffChanging(value);
                _reqStaff = value;
                if (_reqStaff != null)
                {
                    ReqStaffID = reqStaff.StaffID;
                }
                RaisePropertyChanged("reqStaff");
                OnreqStaffChanged();
            }
        }
        private Staff _reqStaff;
        partial void OnreqStaffChanging(Staff value);
        partial void OnreqStaffChanged();


        [DataMemberAttribute()]
        public DeptLocation CurDept
        {
            get
            {
                return _CurDept;
            }
            set
            {
                OnCurDeptChanging(value);
                _CurDept = value;
                if (_CurDept != null)
                {
                    CurDeptID = CurDept.DeptID;
                }
                RaisePropertyChanged("CurDept");
                OnCurDeptChanged();
            }
        }
        private DeptLocation _CurDept;
        partial void OnCurDeptChanging(DeptLocation value);
        partial void OnCurDeptChanged();


        [DataMemberAttribute()]
        public long PtRegistrationID
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
        private long _PtRegistrationID;
        partial void OnPtRegistrationIDChanging(long value);
        partial void OnPtRegistrationIDChanged();

        private long _ConfirmedDTItemID;
        [DataMemberAttribute]
        public long ConfirmedDTItemID
        {
            get
            {
                return _ConfirmedDTItemID;
            }
            set
            {
                if (_ConfirmedDTItemID == value)
                {
                    return;
                }
                _ConfirmedDTItemID = value;
                RaisePropertyChanged("ConfirmedDTItemID");
            }
        }
        #endregion

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}