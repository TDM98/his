using System;
using eHCMS.Services.Core.Base;
using System.Runtime.Serialization;
/*
 * 20220811 #001 QTD: Thêm thông tin hồ sơ bệnh nhân
 */
namespace DataEntities
{
    public partial class PatientMedicalFileStorage : NotifyChangedBase
    {
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is PatientMedicalFileStorage))
            {
                return false;
            }
            if (Object.ReferenceEquals(this, obj))
            {
                return true;
            }
            return ((PatientMedicalFileStorage)obj).PatientMedicalFileID == this.PatientMedicalFileID
                && ((PatientMedicalFileStorage)obj).FileCreatedDate == this.FileCreatedDate;
        }
        public override int GetHashCode()
        {
            return this.PatientMedicalFileID.GetHashCode() + this.FileCreatedDate.GetHashCode();
        }

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
        public long PatientMedicalFileID
        {
            get
            {
                return _PatientMedicalFileID;
            }
            set
            {
                if (_PatientMedicalFileID != value)
                {
                    OnPatientMedicalFileIDChanging(value);
                    _PatientMedicalFileID = value;
                    RaisePropertyChanged("PatientMedicalFileID");
                    OnPatientMedicalFileIDChanged();
                }
            }
        }
        private long _PatientMedicalFileID;
        partial void OnPatientMedicalFileIDChanging(long value);
        partial void OnPatientMedicalFileIDChanged();

        [DataMemberAttribute()]
        public long RefShelfDetailID
        {
            get
            {
                return _RefShelfDetailID;
            }
            set
            {
                if (_RefShelfDetailID != value)
                {
                    OnRefShelfDetailIDChanging(value);
                    _RefShelfDetailID = value;
                    RaisePropertyChanged("RefShelfDetailID");
                    OnRefShelfDetailIDChanged();
                }
            }
        }
        private long _RefShelfDetailID;
        partial void OnRefShelfDetailIDChanging(long value);
        partial void OnRefShelfDetailIDChanged();

        [DataMemberAttribute()]
        public DateTime CreatedDate
        {
            get
            {
                return _CreatedDate;
            }
            set
            {
                if (_CreatedDate != value)
                {
                    OnCreatedDateChanging(value);
                    _CreatedDate = value;
                    RaisePropertyChanged("CreatedDate");
                    OnCreatedDateChanged();
                }
            }
        }
        private DateTime _CreatedDate;
        partial void OnCreatedDateChanging(DateTime value);
        partial void OnCreatedDateChanged();

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
                    OnFileCreatedDateChanging(value);
                    _FileCreatedDate = value;
                    RaisePropertyChanged("FileCreatedDate");
                    OnFileCreatedDateChanged();
                }
            }
        }
        private DateTime _FileCreatedDate;
        partial void OnFileCreatedDateChanging(DateTime value);
        partial void OnFileCreatedDateChanged();

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
                    _FullName = value;
                    RaisePropertyChanged("FullName");
                }
            }
        }
        private string _FullName;

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
        public string RefShelfName
        {
            get
            {
                return _RefShelfName;
            }
            set
            {
                _RefShelfName = value;
                RaisePropertyChanged("RefShelfName");
            }
        }
        private string _RefShelfName;

        [DataMemberAttribute()]
        public string LocName
        {
            get
            {
                return _LocName;
            }
            set
            {
                _LocName = value;
                RaisePropertyChanged("LocName");
            }
        }
        private string _LocName;

        //▼==== #001
        [DataMemberAttribute()]
        public string PatientCode
        {
            get
            {
                return _PatientCode;
            }
            set
            {
                _PatientCode = value;
                RaisePropertyChanged("PatientCode");
            }
        }
        private string _PatientCode;

        public string TreatmentProgCode
        {
            get
            {
                return _TreatmentProgCode;
            }
            set
            {
                _TreatmentProgCode = value;
                RaisePropertyChanged("TreatmentProgCode");
            }
        }
        private string _TreatmentProgCode;
        public DateTime ProgDateFrom
        {
            get
            {
                return _ProgDateFrom;
            }
            set
            {
                _ProgDateFrom = value;
                RaisePropertyChanged("ProgDateFrom");
            }
        }
        private DateTime _ProgDateFrom;
        public DateTime ProgDateTo
        {
            get
            {
                return _ProgDateTo;
            }
            set
            {
                _ProgDateTo = value;
                RaisePropertyChanged("ProgDateTo");
            }
        }
        private DateTime _ProgDateTo;
        public string ExpiryTime
        {
            get
            {
                return _ExpiryTime;
            }
            set
            {
                _ExpiryTime = value;
                RaisePropertyChanged("ExpiryTime");
            }
        }
        private string _ExpiryTime;
        [DataMemberAttribute()]
        public long OutPtTreatmentProgramID
        {
            get
            {
                return _OutPtTreatmentProgramID;
            }
            set
            {
                _OutPtTreatmentProgramID = value;
                RaisePropertyChanged("OutPtTreatmentProgramID");
            }
        }
        private long _OutPtTreatmentProgramID;
        //▲==== #001
    }
}
