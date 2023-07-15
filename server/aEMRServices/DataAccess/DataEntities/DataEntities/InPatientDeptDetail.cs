using System;
using System.Runtime.Serialization;
using eHCMS.Services.Core.Base;
/*
 * 20211028 #001 TNHX: 757 Thêm đánh dấu khoa dùng để điều trị COVID
 */
namespace DataEntities
{
    public partial class InPatientDeptDetail : NotifyChangedBase
    {
        //▼====: #001
        [DataMemberAttribute()]
        public bool IsTreatmentCOVID
        {
            get { return _IsTreatmentCOVID; }
            set
            {
                _IsTreatmentCOVID = value;
                RaisePropertyChanged("IsTreatmentCOVID");
            }
        }
        private bool _IsTreatmentCOVID = false;
        //▲====: #001
        private long _inPatientDeptDetailID;
        [DataMemberAttribute()]
        public long InPatientDeptDetailID
        {
            get { return _inPatientDeptDetailID; }
            set
            {
                _inPatientDeptDetailID = value;
                RaisePropertyChanged("InPatientDeptDetailID");
            }
        }

        private long _inPatientAdmDisDetailID;
        [DataMemberAttribute()]
        public long InPatientAdmDisDetailID
        {
            get { return _inPatientAdmDisDetailID; }
            set
            {
                _inPatientAdmDisDetailID = value;
                RaisePropertyChanged("InPatientAdmDisDetailID");
            }
        }

        private long _deptLocID;
        [DataMemberAttribute()]
        public long DeptLocID
        {
            get { return _deptLocID; }
            set
            {
                _deptLocID = value;
                RaisePropertyChanged("DeptLocID");
            }
        }

        private DeptLocation _deptLoc;
        [DataMemberAttribute()]
        public DeptLocation DeptLocation
        {
            get { return _deptLoc; }
            set
            {
                _deptLoc = value;
                RaisePropertyChanged("DeptLocation");
            }
        }

        private DateTime _fromDate;
        [DataMemberAttribute()]
        public DateTime FromDate
        {
            get { return _fromDate; }
            set
            {
                _fromDate = value;
                RaisePropertyChanged("FromDate");
            }
        }

        private DateTime? _toDate;
        [DataMemberAttribute()]
        public DateTime? ToDate
        {
            get { return _toDate; }
            set
            {
                _toDate = value;
                RaisePropertyChanged("ToDate");
            }
        }


        private bool? _isAdmittedRecord;
        [DataMemberAttribute()]
        public bool? IsAdmittedRecord
        {
            get { return _isAdmittedRecord; }
            set
            {
                _isAdmittedRecord = value;
                RaisePropertyChanged("IsAdmittedRecord");
            }
        }

        private AllLookupValues.InPatientDeptStatus _vInPatientDeptStatus;
        [DataMemberAttribute()]
        public AllLookupValues.InPatientDeptStatus V_InPatientDeptStatus
        {
            get { return _vInPatientDeptStatus; }
            set
            {
                _vInPatientDeptStatus = value;
                RaisePropertyChanged("V_InPatientDeptStatus");
            }
        }

        private BedPatientAllocs _bedAllocation;
        [DataMemberAttribute()]
        public BedPatientAllocs BedAllocation
        {
            get { return _bedAllocation; }
            set
            {
                _bedAllocation = value;
                RaisePropertyChanged("BedAllocation");
            }
        }

        // Hpt 31/10/2015: Thêm Guid
        private Guid? _inPtDeptGuid;
        [DataMemberAttribute()]
        public Guid? InPtDeptGuid
        {
            get
            {
                return _inPtDeptGuid;
            }
            set
            {
                _inPtDeptGuid = value;
                RaisePropertyChanged("InPtDeptGuid");
            }
        }

        private long? _docTypeRequired;
        [DataMemberAttribute()]
        public long? DocTypeRequired
        {
            get
            {
                return _docTypeRequired;
            }
            set
            {
                _docTypeRequired = value;
                RaisePropertyChanged("DocTypeRequired");
            }
        }

        private long? _docTypeRequired_Status;
        [DataMemberAttribute()]
        public long? DocTypeRequired_Status
        {
            get
            {
                return _docTypeRequired_Status;
            }
            set
            {
                _docTypeRequired_Status = value;
                RaisePropertyChanged("DocTypeRequired_Status");
            }
        }

        private DateTime? _completedRequiredFromDate;
        [DataMemberAttribute()]
        public DateTime? CompletedRequiredFromDate
        {
            get
            {
                return _completedRequiredFromDate;
            }
            set
            {
                _completedRequiredFromDate = value;
                RaisePropertyChanged("CompletedRequiredFromDate");
            }
        }
        // HPT 03/04/2017: Add new property to mark the InDept row which is temporary
        private bool _IsTemp;
        [DataMemberAttribute()]
        public bool IsTemp
        {
            get { return _IsTemp; }
            set
            {
                _IsTemp = value;
                RaisePropertyChanged("IsTemp");
            }
        }

        private Int16 _DeptEntrySeqNum;
        [DataMemberAttribute()]
        public Int16 DeptEntrySeqNum
        {
            get { return _DeptEntrySeqNum; }
            set
            {
                _DeptEntrySeqNum = value;
                RaisePropertyChanged("DeptEntrySeqNum");
            }
        }
        
        private bool _IsActive;
        [DataMemberAttribute()]
        public bool IsActive
        {
            get { return _IsActive; }
            set
            {
                _IsActive = value;
                RaisePropertyChanged("IsActive");
            }
        }
    }
    
    // HPT 28/07/2016: thêm DataEntites để lưu danh sách các tài liệu yêu cầu khi chuyển khoa
    public partial class DeptTransferDocReq : NotifyChangedBase
    {
        private long _DeptTransferDocReqID;
        [DataMemberAttribute()]
        public long DeptTransferDocReqID
        {
            get { return _DeptTransferDocReqID; }
            set
            {
                _DeptTransferDocReqID = value;
                RaisePropertyChanged("DeptTransferDocReqID");
            }
        }

        private long _FromDeptID;
        [DataMemberAttribute()]
        public long FromDeptID
        {
            get { return _FromDeptID; }
            set
            {
                _FromDeptID = value;
                RaisePropertyChanged("FromDeptID");
            }
        }

        private long _ToDeptID;
        [DataMemberAttribute()]
        public long ToDeptID
        {
            get { return _ToDeptID; }
            set
            {
                _ToDeptID = value;
                RaisePropertyChanged("ToDeptID");
            }
        }

        private long _DocTypeRequired;
        [DataMemberAttribute()]
        public long DocTypeRequired
        {
            get { return _DocTypeRequired; }
            set
            {
                _DocTypeRequired = value;
                RaisePropertyChanged("DocTypeRequired");
            }
        }

        private string _Comment;
        [DataMemberAttribute()]
        public string Comment
        {
            get { return _Comment; }
            set
            {
                _Comment = value;
                RaisePropertyChanged("Comment");
            }
        }

        private bool _IsActive;
        [DataMemberAttribute()]
        public bool IsActive
        {
            get { return _IsActive; }
            set
            {
                _IsActive = value;
                RaisePropertyChanged("IsActive");
            }
        }       
    }
}
