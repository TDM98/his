using System;
using System.Runtime.Serialization;
using eHCMS.Services.Core.Base;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class PatientMedicalFile : NotifyChangedBase
    {
        #region Factory Method
        public static PatientMedicalFile CreateOutwardDrugInvoice(long PatientMedicalFileID, long PatientRecID, string StorageFilePath, string StorageFileName)
        {
            PatientMedicalFile PatientMedicalFile = new PatientMedicalFile();
            PatientMedicalFile.PatientMedicalFileID = PatientMedicalFileID;
            PatientMedicalFile.PatientRecID = PatientRecID;
            PatientMedicalFile.StorageFilePath = StorageFilePath;
            PatientMedicalFile.StorageFileName = StorageFileName;
            return PatientMedicalFile;
        }

        #endregion

        #region Primitive Properties
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

        [DataMemberAttribute()]
        public long PatientRecID
        {
            get
            {
                return _PatientRecID;
            }
            set
            {
                if (_PatientRecID != value)
                {
                    OnPatientRecIDChanging(value);
                    _PatientRecID = value;
                    RaisePropertyChanged("PatientRecID");
                    OnPatientRecIDChanged();
                }
            }
        }
        private long _PatientRecID;
        partial void OnPatientRecIDChanging(long value);
        partial void OnPatientRecIDChanged();

        [StringLength(8, MinimumLength = 6, ErrorMessage = "Số Hồ Sơ Phải >=6 và <= 8 số!")]
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
                    ValidateProperty("FileCodeNumber", value);
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
        public string FileBarcodeNumber
        {
            get
            {
                return _FileBarcodeNumber;
            }
            set
            {
                if (_FileCodeNumber != value)
                {
                    OnFileBarcodeNumberChanging(value);
                    _FileBarcodeNumber = value;
                    RaisePropertyChanged("FileBarcodeNumber");
                    OnFileBarcodeNumberChanged();
                }
            }
        }
        private string _FileBarcodeNumber;
        partial void OnFileBarcodeNumberChanging(string value);
        partial void OnFileBarcodeNumberChanged();

        [DataMemberAttribute()]
        public string StorageFilePath
        {
            get
            {
                return _StorageFilePath;
            }
            set
            {
                if (_StorageFilePath != value)
                {
                    OnStorageFilePathChanging(value);
                    _StorageFilePath = value;
                    RaisePropertyChanged("StorageFilePath");
                    OnStorageFilePathChanged();
                }
            }
        }
        private string _StorageFilePath;
        partial void OnStorageFilePathChanging(string value);
        partial void OnStorageFilePathChanged();

        [DataMemberAttribute()]
        public string StorageFileName
        {
            get
            {
                return _StorageFileName;
            }
            set
            {
                if (_StorageFileName != value)
                {
                    OnStorageFileNameChanging(value);
                    _StorageFileName = value;
                    RaisePropertyChanged("StorageFileName");
                    OnStorageFileNameChanged();
                }
            }
        }
        private string _StorageFileName;
        partial void OnStorageFileNameChanging(string value);
        partial void OnStorageFileNameChanged();


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
        public DateTime? FinishedDate
        {
            get
            {
                return _FinishedDate;
            }
            set
            {
                if (_FinishedDate != value)
                {
                    OnFinishedDateChanging(value);
                    _FinishedDate = value;
                    RaisePropertyChanged("FinishedDate");
                    OnFinishedDateChanged();
                }
            }
        }
        private DateTime? _FinishedDate;
        partial void OnFinishedDateChanging(DateTime? value);
        partial void OnFinishedDateChanged();

        [DataMemberAttribute()]
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                if (_isActive != value)
                {
                    OnIsActiveChanging(value);
                    _isActive = value;
                    RaisePropertyChanged("IsActive");
                    OnIsActiveChanged();
                }
            }
        }
        private bool _isActive;
        partial void OnIsActiveChanging(bool value);
        partial void OnIsActiveChanged();

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
        #endregion

        [DataMemberAttribute()]
        public PatientMedicalRecord PatientMedicalRecord
        {
            get
            {
                return _PatientMedicalRecord;
            }
            set
            {
                if (_PatientMedicalRecord != value)
                {
                    OnPatientMedicalRecordChanging(value);
                    _PatientMedicalRecord = value;
                    RaisePropertyChanged("PatientMedicalRecord");
                    OnPatientMedicalRecordChanged();
                }
            }
        }
        private PatientMedicalRecord _PatientMedicalRecord;
        partial void OnPatientMedicalRecordChanging(PatientMedicalRecord value);
        partial void OnPatientMedicalRecordChanged();

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is PatientMedicalFile))
            {
                return false;
            }
            if (Object.ReferenceEquals(this, obj))
            {
                return true;
            }
            return ((PatientMedicalFile)obj).PatientRecID == this.PatientRecID
                && ((PatientMedicalFile)obj).FileCodeNumber == this.FileCodeNumber;
        }

        public override int GetHashCode()
        {
            return this.PatientRecID.GetHashCode() + this.FileCodeNumber.GetHashCode();
        }
    }
}