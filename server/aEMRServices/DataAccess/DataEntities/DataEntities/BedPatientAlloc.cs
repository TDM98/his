using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
/*
 * 20210812 #001 TNHX 436 Thêm bsi chỉ định cho đặt giường
 * 20210827 #002 TNHX 436 thêm WasLoadedIntoBill để đánh dấu đã loadbill
 */
namespace DataEntities
{
    public partial class BedPatientAllocs : EntityBase, IEditableObject
    {
        public BedPatientAllocs()
            : base()
        {

        }
        public override bool Equals(object obj)
        {
            BedPatientAllocs info = obj as BedPatientAllocs;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.BedAllocationID > 0 && this.BedAllocationID == info.BedAllocationID;
        }
        private BedPatientAllocs _tempBedPatientAllocs;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempBedPatientAllocs = (BedPatientAllocs)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempBedPatientAllocs)
                CopyFrom(_tempBedPatientAllocs);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(BedPatientAllocs p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new BedPatientAllocs object.

        /// <param name="bedLocNumber">Initial value of the BedLocNumber property.</param>
        /// <param name="allocationID">Initial value of the AllocationID property.</param>
        public static BedPatientAllocs CreateBedPatientAllocs(String bedLocNumber, long allocationID)
        {
            BedPatientAllocs BedPatientAllocs = new BedPatientAllocs();
            
            return BedPatientAllocs;
        }

        #endregion
        #region Primitive Properties
        
        [DataMemberAttribute()]
        public long BedPatientID
        {
            get
            {
                return _BedPatientID;
            }
            set
            {
                OnBedPatientIDChanging(value);
                _BedPatientID = value;
                RaisePropertyChanged("BedPatientID");
                OnBedPatientIDChanged();
            }
        }
        private long _BedPatientID;
        partial void OnBedPatientIDChanging(long value);
        partial void OnBedPatientIDChanged();




        [DataMemberAttribute()]
        public long BedAllocationID
        {
            get
            {
                return _BedAllocationID;
            }
            set
            {
                OnBedAllocationIDChanging(value);
                _BedAllocationID = value;
                RaisePropertyChanged("BedAllocationID");
                OnBedAllocationIDChanged();
            }
        }
        private long _BedAllocationID;
        partial void OnBedAllocationIDChanging(long value);
        partial void OnBedAllocationIDChanged();

        private long _ResponsibleDeptID;
        [DataMemberAttribute()]
        public long ResponsibleDeptID
        {
            get
            {
                return _ResponsibleDeptID;
            }
            set
            {
                _ResponsibleDeptID = value;
                RaisePropertyChanged("ResponsibleDeptID");
            }
        }

        private RefDepartment _ResponsibleDept;
        [DataMemberAttribute()]
        public RefDepartment ResponsibleDepartment
        {
            get
            {
                return _ResponsibleDept;
            }
            set
            {
                _ResponsibleDept = value;
                RaisePropertyChanged("ResponsibleDepartment");
            }
        }


        private DeptLocation _responsibleDeptLocation;
        [DataMemberAttribute()]
        public DeptLocation ResponsibleDeptLocation
        {
            get
            {
                return _responsibleDeptLocation;
            }
            set
            {
                _responsibleDeptLocation = value;
                RaisePropertyChanged("ResponsibleDeptLocation");
            }
        }

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

        [DataMemberAttribute()]
        public string PStatus
        {
            get
            {
                return _PStatus;
            }
            set
            {
                OnPStatusChanging(value);
                _PStatus = value;
                RaisePropertyChanged("PStatus");
                OnPStatusChanged();
            }
        }
        private string _PStatus;
        partial void OnPStatusChanging(string value);
        partial void OnPStatusChanged();


        [DataMemberAttribute()]
        public DateTime? CheckInDate
        {
            get
            {
                return _checkInDate;
            }
            set
            {
                _checkInDate = value;
                RaisePropertyChanged("CheckInDate");
            }
        }
        private DateTime? _checkInDate;

        [DataMemberAttribute()]
        public DateTime? CheckOutDate
        {
            get
            {
                return _checkOutDate;
            }
            set
            {
                _checkOutDate = value;
                RaisePropertyChanged("CheckOutDate");
            }
        }
        private DateTime? _checkOutDate;




        [DataMemberAttribute()]
        public int ExpectedStayingDays
        {
            get
            {
                return _ExpectedStayingDays;
            }
            set
            {
                OnExpectedStayingDaysChanging(value);
                _ExpectedStayingDays = value;
                RaisePropertyChanged("ExpectedStayingDays");
                OnExpectedStayingDaysChanged();
            }
        }
        private int _ExpectedStayingDays=1;
        partial void OnExpectedStayingDaysChanging(int value);
        partial void OnExpectedStayingDaysChanged();

        private bool _PatientInBed;
        [DataMemberAttribute()]
        public bool PatientInBed
        {
            get
            {
                return _PatientInBed;
            }
            set
            {
                _PatientInBed = value;
                RaisePropertyChanged("PatientInBed");
            }
        }


        [DataMemberAttribute()]
        public bool IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                OnIsActiveChanging(value);
                _IsActive = value;
                RaisePropertyChanged("IsActive");
                OnIsActiveChanged();
            }
        }
        private bool _IsActive;
        partial void OnIsActiveChanging(bool value);
        partial void OnIsActiveChanged();

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public BedAllocation VBedAllocation
        {
            get
            {
                return _VBedAllocation;
            }
            set
            {
                OnVBedAllocationChanging(value);
                _VBedAllocation = value;
                RaisePropertyChanged("VBedAllocation");
                OnVBedAllocationChanged();
            }
        }
        private BedAllocation _VBedAllocation;
        partial void OnVBedAllocationChanging(BedAllocation value);
        partial void OnVBedAllocationChanged();

        [DataMemberAttribute()]
        public Patient VBedPatient
        {
            get
            {
                return _VBedPatient;
            }
            set
            {
                OnVBedPatientChanging(value);
                _VBedPatient = value;
                RaisePropertyChanged("VBedPatient");
                OnVBedPatientChanged();
            }
        }
        private Patient _VBedPatient;
        partial void OnVBedPatientChanging(Patient value);
        partial void OnVBedPatientChanged();

        [DataMemberAttribute()]
        public PatientRegistration VPtRegistration
        {
            get
            {
                return _VPtRegistration;
            }
            set
            {
                OnVPtRegistrationChanging(value);
                _VPtRegistration = value;
                RaisePropertyChanged("VPtRegistration");
                OnVPtRegistrationChanged();
            }
        }
        private PatientRegistration _VPtRegistration;
        partial void OnVPtRegistrationChanging(PatientRegistration value);
        partial void OnVPtRegistrationChanged();
        #endregion

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private bool _isDeleted;
        public bool IsDeleted
        {
            get
            {
                return _isDeleted;
            }
            set
            {
                _isDeleted = value;
                RaisePropertyChanged("IsDeleted");
            }
        }

        private ObservableCollection<BedPatientRegDetail> _bedPatientRegDetails;
        [DataMemberAttribute()]
        public ObservableCollection<BedPatientRegDetail> BedPatientRegDetails
        {
            get
            {
                return _bedPatientRegDetails;
            }
            set
            {
                _bedPatientRegDetails = value;
                RaisePropertyChanged("BedPatientRegDetails");
            }
        }

        private long? _InPatientDeptDetailID;
        [DataMemberAttribute]
        public long? InPatientDeptDetailID
        {
            get
            {
                return _InPatientDeptDetailID;
            }
            set
            {
                _InPatientDeptDetailID = value;
                RaisePropertyChanged("InPatientDeptDetailID");
            }
        }

        private long? _BAMedServiceID;
        [DataMemberAttribute]
        public long? BAMedServiceID
        {
            get
            {
                return _BAMedServiceID;
            }
            set
            {
                _BAMedServiceID = value;
                RaisePropertyChanged("BAMedServiceID");
            }
        }

        //▼====: #001
        private long? _DoctorStaffID;
        [DataMemberAttribute]
        public long? DoctorStaffID
        {
            get
            {
                return _DoctorStaffID;
            }
            set
            {
                _DoctorStaffID = value;
                RaisePropertyChanged("DoctorStaffID");
            }
        }

        private Staff _DoctorStaff;
        [DataMemberAttribute]
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

        private bool _IsEditing;
        [DataMemberAttribute]
        public bool IsEditing
        {
            get
            {
                return _IsEditing;
            }
            set
            {
                _IsEditing = value;
                RaisePropertyChanged("IsEditing");
            }
        }
        //▲====: #001
        //▼====: #002
        private bool _WasLoadedIntoBill;
        [DataMemberAttribute]
        public bool WasLoadedIntoBill
        {
            get
            {
                return _WasLoadedIntoBill;
            }
            set
            {
                _WasLoadedIntoBill = value;
                RaisePropertyChanged("WasLoadedIntoBill");
            }
        }

        private int _NumDeptInDay;
        [DataMemberAttribute]
        public int NumDeptInDay
        {
            get
            {
                return _NumDeptInDay;
            }
            set
            {
                _NumDeptInDay = value;
                RaisePropertyChanged("NumDeptInDay");
            }
        }
        private int _CountBedInDeptDetail;
        [DataMemberAttribute]
        public int CountBedInDeptDetail
        {
            get
            {
                return _CountBedInDeptDetail;
            }
            set
            {
                _CountBedInDeptDetail = value;
                RaisePropertyChanged("CountBedInDeptDetail");
            }
        }
        //▲====: #002
    }
}
