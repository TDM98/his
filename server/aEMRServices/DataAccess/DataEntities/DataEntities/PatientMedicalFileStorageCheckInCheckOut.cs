using System;
using eHCMS.Services.Core.Base;
using System.Runtime.Serialization;
using eHCMSLanguage;
using System.Collections.Generic;
/*
* 20220820 #001 QTD: Thêm trường cho quản lý hồ sơ
*/
namespace DataEntities
{
    public partial class PatientMedicalFileStorageCheckInCheckOut : NotifyChangedBase
    {
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is PatientMedicalFileStorageCheckInCheckOut))
            {
                return false;
            }
            if (Object.ReferenceEquals(this, obj))
            {
                return true;
            }
            return ((PatientMedicalFileStorageCheckInCheckOut)obj).PatientMedicalFileCheckoutID == this.PatientMedicalFileCheckoutID
                && ((PatientMedicalFileStorageCheckInCheckOut)obj).CheckoutDate == this.CheckoutDate;
        }
        public override int GetHashCode()
        {
            return this.PatientMedicalFileCheckoutID.GetHashCode() + this.CheckoutDate.GetHashCode();
        }

        [DataMemberAttribute()]
        public long PatientMedicalFileCheckoutID
        {
            get
            {
                return _PatientMedicalFileCheckoutID;
            }
            set
            {
                if (_PatientMedicalFileCheckoutID != value)
                {
                    OnPatientMedicalFileCheckoutIDChanging(value);
                    _PatientMedicalFileCheckoutID = value;
                    RaisePropertyChanged("PatientMedicalFileCheckoutID");
                    OnPatientMedicalFileCheckoutIDChanged();
                }
            }
        }
        private long _PatientMedicalFileCheckoutID;
        partial void OnPatientMedicalFileCheckoutIDChanging(long value);
        partial void OnPatientMedicalFileCheckoutIDChanged();

        [DataMemberAttribute()]
        public long PatientMedicalFileStorageID
        {
            get
            {
                return _PatientMedicalFileStorageID;
            }
            set
            {
                if (_PatientMedicalFileStorageID != value)
                {
                    OnPatientMedicalFileStorageIDChanging(value);
                    _PatientMedicalFileStorageID = value;
                    RaisePropertyChanged("PatientMedicalFileStorageID");
                    OnPatientMedicalFileStorageIDChanged();
                }
            }
        }
        private long _PatientMedicalFileStorageID;
        partial void OnPatientMedicalFileStorageIDChanging(long value);
        partial void OnPatientMedicalFileStorageIDChanged();

        [DataMemberAttribute()]
        public long CheckinStaffID
        {
            get
            {
                return _CheckinStaffID;
            }
            set
            {
                if (_CheckinStaffID != value)
                {
                    OnCheckinStaffIDChanging(value);
                    _CheckinStaffID = value;
                    RaisePropertyChanged("CheckinStaffID");
                    OnCheckinStaffIDChanged();
                }
            }
        }
        private long _CheckinStaffID;
        partial void OnCheckinStaffIDChanging(long value);
        partial void OnCheckinStaffIDChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> CheckinDate
        {
            get
            {
                return _CheckinDate;
            }
            set
            {
                if (_CheckinDate != value)
                {
                    OnCheckinDateChanging(value);
                    _CheckinDate = value;
                    RaisePropertyChanged("CheckinDate");
                    OnCheckinDateChanged();
                }
            }
        }
        private Nullable<DateTime> _CheckinDate;
        partial void OnCheckinDateChanging(Nullable<DateTime> value);
        partial void OnCheckinDateChanged();

        [DataMemberAttribute()]
        public long CheckoutStaffID
        {
            get
            {
                return _CheckoutStaffID;
            }
            set
            {
                if (_CheckoutStaffID != value)
                {
                    OnCheckoutStaffIDChanging(value);
                    _CheckoutStaffID = value;
                    RaisePropertyChanged("CheckoutStaffID");
                    OnCheckoutStaffIDChanged();
                }
            }
        }
        private long _CheckoutStaffID;
        partial void OnCheckoutStaffIDChanging(long value);
        partial void OnCheckoutStaffIDChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> CheckoutDate
        {
            get
            {
                return _CheckoutDate;
            }
            set
            {
                if (_CheckoutDate != value)
                {
                    OnCheckoutDateChanging(value);
                    _CheckoutDate = value;
                    RaisePropertyChanged("CheckoutDate");
                    OnCheckoutDateChanged();
                }
            }
        }
        private Nullable<DateTime> _CheckoutDate;
        partial void OnCheckoutDateChanging(Nullable<DateTime> value);
        partial void OnCheckoutDateChanged();

        [DataMemberAttribute()]
        public long StaffPersonID
        {
            get
            {
                return _StaffPersonID;
            }
            set
            {
                if (_StaffPersonID != value)
                {
                    OnStaffPersonIDChanging(value);
                    _StaffPersonID = value;
                    RaisePropertyChanged("StaffPersonID");
                    OnStaffPersonIDChanged();
                }
            }
        }
        private long _StaffPersonID;
        partial void OnStaffPersonIDChanging(long value);
        partial void OnStaffPersonIDChanged();

        [DataMemberAttribute()]
        public long DeptID
        {
            get
            {
                return _DeptID;
            }
            set
            {
                if (_DeptID != value)
                {
                    OnDeptIDChanging(value);
                    _DeptID = value;
                    RaisePropertyChanged("DeptID");
                    OnDeptIDChanged();
                }
            }
        }
        private long _DeptID;
        partial void OnDeptIDChanging(long value);
        partial void OnDeptIDChanged();

        [DataMemberAttribute()]
        public long DeptLocID
        {
            get
            {
                return _DeptLocID;
            }
            set
            {
                if (_DeptLocID != value)
                {
                    OnDeptLocIDChanging(value);
                    _DeptLocID = value;
                    RaisePropertyChanged("DeptLocID");
                    OnDeptLocIDChanged();
                }
            }
        }
        private long _DeptLocID;
        partial void OnDeptLocIDChanging(long value);
        partial void OnDeptLocIDChanged();

        [DataMemberAttribute()]
        public DateTime RecCreatedDate
        {
            get
            {
                return _RecCreatedDate;
            }
            set
            {
                if (_RecCreatedDate != value)
                {
                    OnRecCreatedDateChanging(value);
                    _RecCreatedDate = value;
                    RaisePropertyChanged("RecCreatedDate");
                    OnRecCreatedDateChanged();
                }
            }
        }
        private DateTime _RecCreatedDate;
        partial void OnRecCreatedDateChanging(DateTime value);
        partial void OnRecCreatedDateChanged();

        [DataMemberAttribute()]
        public string LocCode
        {
            get
            {
                return _LocCode;
            }
            set
            {
                if (_LocCode != value)
                {
                    OnLocCodeChanging(value);
                    _LocCode = value;
                    RaisePropertyChanged("LocCode");
                    OnLocCodeChanged();
                }
            }
        }
        private string _LocCode;
        partial void OnLocCodeChanging(string value);
        partial void OnLocCodeChanged();

        [DataMemberAttribute()]
        public string LocName
        {
            get
            {
                return _LocName;
            }
            set
            {
                if (_LocName != value)
                {
                    _LocName = value;
                    RaisePropertyChanged("LocName");
                }
            }
        }
        private string _LocName;

        [DataMemberAttribute()]
        public string RefShelfCode
        {
            get
            {
                return _RefShelfCode;
            }
            set
            {
                if (_RefShelfCode != value)
                {
                    OnRefShelfCodeChanging(value);
                    _RefShelfCode = value;
                    RaisePropertyChanged("RefShelfCode");
                    OnRefShelfCodeChanged();
                }
            }
        }
        private string _RefShelfCode;
        partial void OnRefShelfCodeChanging(string value);
        partial void OnRefShelfCodeChanged();

        [DataMemberAttribute()]
        public DateTime? ImportedDate
        {
            get
            {
                return _ImportedDate;
            }
            set
            {
                if (_ImportedDate != value)
                {
                    _ImportedDate = value;
                    RaisePropertyChanged("ImportedDate");
                }
            }
        }
        private DateTime? _ImportedDate;

        [DataMemberAttribute()]
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
                    OnFullNameChanged();
                }
            }
        }
        private string _FullName;
        partial void OnFullNameChanging(string value);
        partial void OnFullNameChanged();

        [DataMemberAttribute()]
        public FileStorageStatus FileStorageStatus
        {
            get
            {
                return PatientMedicalFileCheckoutID > 0 && CheckinDate == null ? FileStorageStatus.OutStore : string.IsNullOrEmpty(LocCode) ? FileStorageStatus.UnSave : FileStorageStatus.InStore;
            }
        }

        [DataMemberAttribute()]
        public string Status
        {
            get
            {
                return PatientMedicalFileCheckoutID > 0 && CheckinDate == null ? eHCMSResources.Z1960_G1_DaDuocMuon : string.IsNullOrEmpty(LocCode) ? eHCMSResources.Z0863_G1_KgTimThay : eHCMSResources.Z1961_G1_NamTrongKe;
            }
        }

        [DataMemberAttribute()]
        public string FileCodeNumber
        {
            get
            {
                return _FileCodeNumber;
            }
            set
            {
                if (_FileCodeNumber != value)
                {
                    OnFileCodeNumberChanging(value);
                    _FileCodeNumber = value;
                    RaisePropertyChanged("FileCodeNumber");
                    OnFileCodeNumberChanged();
                }
            }
        }
        private string _FileCodeNumber;
        partial void OnFileCodeNumberChanging(string value);
        partial void OnFileCodeNumberChanged();

        [DataMemberAttribute()]
        public DateTime FileCreatedDate
        {
            get
            {
                return _FileCreatedDate;
            }
            set
            {
                if (_FileCreatedDate != value)
                {
                    _FileCreatedDate = value;
                    RaisePropertyChanged("FileCreatedDate");
                }
            }
        }
        private DateTime _FileCreatedDate;

        [DataMemberAttribute()]
        public string DeptName
        {
            get
            {
                return _DeptName;
            }
            set
            {
                if (_DeptName != value)
                {
                    OnDeptNameChanging(value);
                    _DeptName = value;
                    RaisePropertyChanged("DeptName");
                    OnDeptNameChanged();
                }
            }
        }
        private string _DeptName;

        //TMA
        [DataMemberAttribute()]
        public bool IsAgeOnly
        {
            get
            {
                return _IsAgeOnly;
            }
            set
            {
                if (_IsAgeOnly != value)
                {
                    _IsAgeOnly = value;
                    RaisePropertyChanged("IsAgeOnly");
                }
            }
        }
        private bool _IsAgeOnly;

        [DataMemberAttribute()]
        public DateTime yDOB
        {
            get
            {
                return _yDOB;
            }
            set
            {
                if (_yDOB != value)
                {
                    _yDOB = value;
                    RaisePropertyChanged("yDOB");
                }
            }
        }
        private DateTime _yDOB;

        [DataMemberAttribute()]
        public long PatientID
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
        private long _PatientID;
        partial void OnPatientIDChanging(long value);
        partial void OnPatientIDChanged();
        //
        partial void OnDeptNameChanging(string value);
        partial void OnDeptNameChanged();

        [DataMemberAttribute()]
        public string LocationName
        {
            get
            {
                return _LocationName;
            }
            set
            {
                if (_LocationName != value)
                {
                    OnLocationNameChanging(value);
                    _LocationName = value;
                    RaisePropertyChanged("LocationName");
                    OnLocationNameChanged();
                }
            }
        }
        private string _LocationName;
        partial void OnLocationNameChanging(string value);
        partial void OnLocationNameChanged();

        [DataMemberAttribute()]
        public long PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                if (_PtRegistrationID != value)
                {
                    OnPtRegistrationIDChanging(value);
                    _PtRegistrationID = value;
                    RaisePropertyChanged("PtRegistrationID");
                    OnPtRegistrationIDChanged();
                }
            }
        }
        private long _PtRegistrationID;
        partial void OnPtRegistrationIDChanging(long value);
        partial void OnPtRegistrationIDChanged();

        [DataMemberAttribute()]
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                if (_IsSelected != value)
                {
                    OnIsSelectedChanging(value);
                    _IsSelected = value;
                    RaisePropertyChanged("IsSelected");
                    OnIsSelectedChanged();
                }
            }
        }
        private bool _IsSelected;
        partial void OnIsSelectedChanging(bool value);
        partial void OnIsSelectedChanged();

        [DataMemberAttribute()]
        public bool IsInPT
        {
            get
            {
                return _IsInPT;
            }
            set
            {
                if (_IsInPT != value)
                {
                    _IsInPT = value;
                    RaisePropertyChanged("IsInPT");
                }
            }
        }
        private bool _IsInPT;

        [DataMemberAttribute()]
        public string RefShelfName
        {
            get
            {
                return _RefShelfName;
            }
            set
            {
                if (_RefShelfName != value)
                {
                    _RefShelfName = value;
                    RaisePropertyChanged("RefShelfName");
                }
            }
        }
        private string _RefShelfName;

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
                    _PatientCode = value;
                    RaisePropertyChanged("PatientCode");
                }
            }
        }
        private string _PatientCode;

        [DataMemberAttribute()]
        public string swhlName
        {
            get
            {
                return _swhlName;
            }
            set
            {
                _swhlName = value;
                RaisePropertyChanged("swhlName");
            }
        }
        private string _swhlName;

        [DataMemberAttribute()]
        public string BorrowBy
        {
            get
            {
                return _BorrowBy;
            }
            set
            {
                _BorrowBy = value;
                RaisePropertyChanged("BorrowBy");
            }
        }
        private string _BorrowBy;

        [DataMemberAttribute()]
        public bool IsPrinted
        {
            get
            {
                return PrintedDate != null;
            }
        }

        [DataMemberAttribute()]
        public DateTime? PrintedDate
        {
            get
            {
                return _PrintedDate;
            }
            set
            {
                _PrintedDate = value;
                RaisePropertyChanged("PrintedDate");
            }
        }
        private DateTime? _PrintedDate = null;

        [DataMemberAttribute()]
        public long? MedicalFileStorageCheckID
        {
            get
            {
                return _MedicalFileStorageCheckID;
            }
            set
            {
                _MedicalFileStorageCheckID = value;
                RaisePropertyChanged("MedicalFileStorageCheckID");
            }
        }
        private long? _MedicalFileStorageCheckID = null;
        //▼====: #001
        [DataMemberAttribute()]
        public string RefRowName
        {
            get
            {
                return _RefRowName;
            }
            set
            {
                if (_RefRowName != value)
                {
                    _RefRowName = value;
                    RaisePropertyChanged("RefRowName");
                }
            }
        }
        private string _RefRowName;
        [DataMemberAttribute()]
        public string ExpiryTime
        {
            get
            {
                return _ExpiryTime;
            }
            set
            {
                if (_ExpiryTime != value)
                {
                    _ExpiryTime = value;
                    RaisePropertyChanged("ExpiryTime");
                }
            }
        }
        private string _ExpiryTime;
        [DataMemberAttribute()]
        public string MedicalFileStatus
        {
            get
            {
                return _MedicalFileStatus;
            }
            set
            {
                if (_MedicalFileStatus != value)
                {
                    _MedicalFileStatus = value;
                    RaisePropertyChanged("MedicalFileStatus");
                }
            }
        }
        private string _MedicalFileStatus;
        [DataMemberAttribute()]
        public long OutPtTreatmentProgramID
        {
            get
            {
                return _OutPtTreatmentProgramID;
            }
            set
            {
                if (_OutPtTreatmentProgramID != value)
                {
                    _OutPtTreatmentProgramID = value;
                    RaisePropertyChanged("OutPtTreatmentProgramID");
                }
            }
        }
        private long _OutPtTreatmentProgramID;

        public DateTime FileEndDate
        {
            get
            {
                return _FileEndDate;
            }
            set
            {
                if (_FileEndDate != value)
                {
                    _FileEndDate = value;
                    RaisePropertyChanged("FileEndDate");
                }
            }
        }
        private DateTime _FileEndDate;
        public long V_ReasonType
        {
            get
            {
                return _V_ReasonType;
            }
            set
            {
                if (_V_ReasonType != value)
                {
                    _V_ReasonType = value;
                    RaisePropertyChanged("V_ReasonType");
                }
            }
        }
        private long _V_ReasonType;
        public string Notes
        {
            get
            {
                return _Notes;
            }
            set
            {
                if (_Notes != value)
                {
                    _Notes = value;
                    RaisePropertyChanged("Notes");
                }
            }
        }
        private string _Notes;
        public int BorrowingDay
        {
            get
            {
                return _BorrowingDay;
            }
            set
            {
                if (_BorrowingDay != value)
                {
                    _BorrowingDay = value;
                    RaisePropertyChanged("BorrowingDay");
                }
            }
        }
        private int _BorrowingDay;
        public long StaffPersonInID
        {
            get
            {
                return _StaffPersonInID;
            }
            set
            {
                if (_StaffPersonInID != value)
                {
                    _StaffPersonInID = value;
                    RaisePropertyChanged("StaffPersonInID");
                }
            }
        }
        private long _StaffPersonInID;
        public long DeptLocInID
        {
            get
            {
                return _DeptLocInID;
            }
            set
            {
                if (_DeptLocInID != value)
                {
                    _DeptLocInID = value;
                    RaisePropertyChanged("DeptLocInID");
                }
            }
        }
        private long _DeptLocInID;

        private List<PatientRegistration> _ListPtRegistration;

        public List<PatientRegistration> ListPtRegistration
        {
            get
            {
                return _ListPtRegistration;
            }
            set
            {
                if (_ListPtRegistration != value)
                {
                    _ListPtRegistration = value;
                    RaisePropertyChanged("ListPtRegistration");
                }
            }
        }
        public DateTime ExamDate
        {
            get
            {
                return _ExamDate;
            }
            set
            {
                if (_ExamDate != value)
                {
                    _ExamDate = value;
                    RaisePropertyChanged("ExamDate");
                }
            }
        }
        private DateTime _ExamDate;
        public DateTime DischargeDate
        {
            get
            {
                return _DischargeDate;
            }
            set
            {
                if (_DischargeDate != value)
                {
                    _DischargeDate = value;
                    RaisePropertyChanged("DischargeDate");
                }
            }
        }
        private DateTime _DischargeDate;
        //▲====: #001
    }
}
