using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{

    public partial class PatientQueue : OutstandingItem
    {

        public override string HeaderText
        {
            get
            {
                //if (!string.IsNullOrWhiteSpace(_ServiceName))
                //    return string.Format("{0} ({1})", _FullName, _ServiceName);
                //return _FullName;
                return FullName
                + (!string.IsNullOrEmpty(PatientCode) ? " - " + PatientCode : "")
                + (!string.IsNullOrEmpty(DOBText) ? " - " + DOBText : "");
            }
            set
            {
                //if (_FullName != value)
                //{
                //    _FullName = value;
                //    RaisePropertyChanged("HeaderText");
                //}
            }
        }

        public override object ID
        {
            get
            {
                return _QueueID;
            }
            set
            {
                if (_QueueID != (long)value)
                {
                    _QueueID = (long)value;
                    RaisePropertyChanged("ID");
                }
            }
        }

        public PatientQueue()
            : base()
        {

        }

        [DataMemberAttribute()]
        private long _QueueID;
        public long QueueID
        {
            get
            {
                return _QueueID;
            }
            set
            {
                if (_QueueID != value)
                {
                    OnQueueIDChanging(value);
                    _QueueID = value;
                    RaisePropertyChanged("QueueID");
                    OnQueueIDChanged();
                }
            }
        }
        partial void OnQueueIDChanging(long value);
        partial void OnQueueIDChanged();


        [DataMemberAttribute()]
        private long _RegistrationID;
        public long RegistrationID
        {
            get
            {
                return _RegistrationID;
            }
            set
            {
                if (_RegistrationID != value)
                {
                    OnRegistrationIDChanging(value);
                    _RegistrationID = value;
                    RaisePropertyChanged("RegistrationID");
                    OnRegistrationIDChanged();
                }
            }
        }
        partial void OnRegistrationIDChanging(long value);
        partial void OnRegistrationIDChanged();

        [DataMemberAttribute()]
        private Nullable<Int64> _PatientPCLReqID;
        public Nullable<Int64> PatientPCLReqID
        {
            get { return _PatientPCLReqID; }
            set
            {
                if (_PatientPCLReqID != value)
                {
                    OnPatientPCLReqIDChanging(value);
                    _PatientPCLReqID = value;
                    RaisePropertyChanged("PatientPCLReqID");
                    OnPatientPCLReqIDChanged();
                }
            }
        }
        partial void OnPatientPCLReqIDChanging(Nullable<Int64> value);
        partial void OnPatientPCLReqIDChanged();

        [DataMemberAttribute()]
        private Nullable<Int64> _PatientAppointmentID;
        public Nullable<Int64> PatientAppointmentID
        {
            get { return _PatientAppointmentID; }
            set
            {
                if (_PatientAppointmentID != value)
                {
                    OnPatientAppointmentIDChanging(value);
                    _PatientAppointmentID = value;
                    RaisePropertyChanged("PatientAppointmentID");
                    OnPatientAppointmentIDChanged();
                }
            }
        }
        partial void OnPatientAppointmentIDChanging(Nullable<Int64> value);
        partial void OnPatientAppointmentIDChanged();

        [DataMemberAttribute()]
        private Nullable<Int64> _RegistrationDetailsID;
        public Nullable<Int64> RegistrationDetailsID
        {
            get
            {
                return _RegistrationDetailsID;
            }
            set
            {
                if (_RegistrationDetailsID != value)
                {
                    OnRegistrationDetailsIDChanging(value);
                    _RegistrationDetailsID = value;
                    RaisePropertyChanged("RegistrationDetailsID");
                    OnRegistrationDetailsIDChanged();
                }
            }
        }
        partial void OnRegistrationDetailsIDChanging(Nullable<Int64> value);
        partial void OnRegistrationDetailsIDChanged();


        [DataMemberAttribute()]
        private Int64? _PrescriptionIssueID;
        public Int64? PrescriptionIssueID
        {
            get { return _PrescriptionIssueID; }
            set
            {
                if (_PrescriptionIssueID != value)
                {
                    OnPrescriptionIssueIDChanging(value);
                    _PrescriptionIssueID = value;
                    RaisePropertyChanged("PrescriptionIssueID");
                    OnPrescriptionIssueIDChanged();
                }
            }
        }
        partial void OnPrescriptionIssueIDChanging(Int64? value);
        partial void OnPrescriptionIssueIDChanged();

        [DataMemberAttribute()]
        private long? _PatientID;
        public long? PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                if (_PatientID != value)
                {
                    OnPatientIDChanging(value);
                    _PatientID = value;
                    RaisePropertyChanged("PatientID");
                    OnPatientIDChanged();
                }
            }
        }
        partial void OnPatientIDChanging(long? value);
        partial void OnPatientIDChanged();

        [DataMemberAttribute()]
        private int? _SequenceNo;
        public int? SequenceNo
        {
            get
            {
                return _SequenceNo;
            }
            set
            {
                if (_SequenceNo != value)
                {
                    OnSequenceNoChanging(value);
                    _SequenceNo = value;
                    RaisePropertyChanged("SequenceNo");
                    OnSequenceNoChanged();
                }
            }
        }
        partial void OnSequenceNoChanging(int? value);
        partial void OnSequenceNoChanged();


        [DataMemberAttribute()]
        private string _FullName;
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
                    OnFullNameChanging(value);
                    _FullName = value;
                    RaisePropertyChanged("FullName");
                    RaisePropertyChanged("HeaderText");
                    OnFullNameChanged();
                }
            }
        }
        partial void OnFullNameChanging(string value);
        partial void OnFullNameChanged();


        [DataMemberAttribute()]
        private long? _deptLocID;
        public long? DeptLocID
        {
            get
            {
                return _deptLocID;
            }
            set
            {
                if (_deptLocID != value)
                {
                    _deptLocID = value;
                    RaisePropertyChanged("DeptLocID");
                }
            }
        }

        [DataMemberAttribute()]
        private long _V_QueueType;
        public long V_QueueType
        {
            get
            {
                return _V_QueueType;
            }
            set
            {
                if (_V_QueueType != value)
                {
                    OnV_QueueTypeChanging(value);
                    _V_QueueType = value;
                    RaisePropertyChanged("V_QueueType");
                    OnV_QueueTypeChanged();
                }
            }
        }
        partial void OnV_QueueTypeChanging(long value);
        partial void OnV_QueueTypeChanged();


        [DataMemberAttribute()]
        private bool _ActionPending;
        public bool ActionPending
        {
            get
            {
                return _ActionPending;
            }
            set
            {
                if (_ActionPending != value)
                {
                    OnActionPendingChanging(value);
                    _ActionPending = value;
                    RaisePropertyChanged("ActionPending");
                    OnActionPendingChanged();
                }
            }
        }
        partial void OnActionPendingChanging(bool value);
        partial void OnActionPendingChanged();

        [DataMemberAttribute()]
        private int _EnqueueSequenceNo;
        public int EnqueueSequenceNo
        {
            get
            {
                return _EnqueueSequenceNo;
            }
            set
            {
                if (_EnqueueSequenceNo != value)
                {
                    OnEnqueueSequenceNoChanging(value);
                    _EnqueueSequenceNo = value;
                    RaisePropertyChanged("EnqueueSequenceNo");
                    OnEnqueueSequenceNoChanged();
                }
            }
        }
        partial void OnEnqueueSequenceNoChanging(int value);
        partial void OnEnqueueSequenceNoChanged();


        [DataMemberAttribute()]
        private int _Priority;
        public int Priority
        {
            get
            {
                return _Priority;
            }
            set
            {
                if (_Priority != value)
                {
                    OnPriorityChanging(value);
                    _Priority = value;
                    RaisePropertyChanged("Priority");
                    OnPriorityChanged();
                }
            }
        }
        partial void OnPriorityChanging(int value);
        partial void OnPriorityChanged();


        [DataMemberAttribute()]
        private DateTime? _EnqueueTime;
        public DateTime? EnqueueTime
        {
            get
            {
                return _EnqueueTime;
            }
            set
            {
                if (_EnqueueTime != value)
                {
                    OnEnqueueTimeChanging(value);
                    _EnqueueTime = value;
                    RaisePropertyChanged("EnqueueTime");
                    OnEnqueueTimeChanged();
                }
            }
        }
        partial void OnEnqueueTimeChanging(DateTime? value);
        partial void OnEnqueueTimeChanged();

        [DataMemberAttribute()]
        private long? _MedServiceID;
        public long? MedServiceID
        {
            get
            {
                return _MedServiceID;
            }
            set
            {
                if (_MedServiceID != value)
                {
                    OnMedServiceIDChanging(value);
                    _MedServiceID = value;
                    RaisePropertyChanged("MedServiceID");
                    OnMedServiceIDChanged();
                }
            }
        }
        partial void OnMedServiceIDChanging(long? value);
        partial void OnMedServiceIDChanged();

        [DataMemberAttribute()]
        private Int64 _V_PatientQueueItemsStatus;
        public Int64 V_PatientQueueItemsStatus
        {
            get
            {
                return _V_PatientQueueItemsStatus;
            }
            set
            {
                if (_V_PatientQueueItemsStatus != value)
                {
                    OnV_PatientQueueItemsStatusChanging(value);
                    _V_PatientQueueItemsStatus = value;
                    RaisePropertyChanged("V_PatientQueueItemsStatus");
                    OnV_PatientQueueItemsStatusChanged();
                }
            }
        }
        partial void OnV_PatientQueueItemsStatusChanging(Int64 value);
        partial void OnV_PatientQueueItemsStatusChanged();


        [DataMemberAttribute()]
        private bool _IsDeleted;
        public bool IsDeleted
        {
            get { return _IsDeleted; }
            set
            {
                if (_IsDeleted != value)
                {
                    OnIsDeletedChanging(value);
                    _IsDeleted = value;
                    RaisePropertyChanged("IsDeleted");
                    OnIsDeletedChanged();
                }
            }
        }
        partial void OnIsDeletedChanging(bool value);
        partial void OnIsDeletedChanged();

        [DataMemberAttribute()]
        private string _ServiceName;
        public string ServiceName
        {
            get
            {
                return _ServiceName;
            }
            set
            {
                OnServiceNameChanging(value);
                _ServiceName = value;
                RaisePropertyChanged("ServiceName");
                RaisePropertyChanged("HeaderText");
                OnServiceNameChanged();
            }
        }
        partial void OnServiceNameChanging(string value);
        partial void OnServiceNameChanged();

        private string _PatientCode;
        [DataMemberAttribute()]
        public string PatientCode
        {
            get
            {
                return _PatientCode;
            }
            set
            {
                if (_PatientCode != value)
                {
                    OnPatientCodeChanging(value);
                    _PatientCode = value;
                    RaisePropertyChanged("PatientCode");
                    OnPatientCodeChanged();
                }
            }
        }
        partial void OnPatientCodeChanging(string value);
        partial void OnPatientCodeChanged();

        private string _DOBText;
        [DataMemberAttribute()]
        public string DOBText
        {
            get
            {
                return _DOBText;
            }
            set
            {
                if (_DOBText != value)
                {
                    OnDOBTextChanging(value);
                    _DOBText = value;
                    RaisePropertyChanged("DOBText");
                    OnDOBTextChanged();
                }
            }
        }
        partial void OnDOBTextChanging(string value);
        partial void OnDOBTextChanged();

        private string _Display;
        [DataMemberAttribute()]
        public string Display
        {
            get
            {
                return _Display;
            }
            set
            {
                if (_Display != value)
                {
                    OnDisplayChanging(value);
                    _Display = value;
                    RaisePropertyChanged("Display");
                    OnDisplayChanged();
                }
            }
        }
        partial void OnDisplayChanging(string value);
        partial void OnDisplayChanged();

        public void GetFullInfo()
        {
            Display = FullName
                + (!string.IsNullOrEmpty(PatientCode) ? " - " + PatientCode : "")
                + (!string.IsNullOrEmpty(DOBText) ? " - " + DOBText : "");
        }

        public override bool Equals(object obj)
        {
            PatientQueue cond = obj as PatientQueue;
            if (cond == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.ID == cond.ID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

}
