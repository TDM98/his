using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{

    public partial class PatientMedicalRecord : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new PatientMedicalRecord object.

        /// <param name="patientRecID">Initial value of the PatientRecID property.</param>
        public static PatientMedicalRecord CreatePatientMedicalRecord(long patientRecID)
        {
            PatientMedicalRecord patientMedicalRecord = new PatientMedicalRecord();
            patientMedicalRecord.PatientRecID = patientRecID;
            return patientMedicalRecord;
        }

        #endregion
        #region Primitive Properties
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

     
        
     
        [DataMemberAttribute()]
        public Nullable<long> PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                OnPatientIDChanging(value);
                _PatientID = value;
                RaisePropertyChanged("PatientID");
                OnPatientIDChanged();
            }
        }
        private Nullable<long> _PatientID;
        partial void OnPatientIDChanging(Nullable<long> value);
        partial void OnPatientIDChanged();
    
        [DataMemberAttribute()]
        public String NationalMedicalCode
        {
            get
            {
                return _NationalMedicalCode;
            }
            set
            {
                OnNationalMedicalCodeChanging(value);
                _NationalMedicalCode = value;
                RaisePropertyChanged("NationalMedicalCode");
                OnNationalMedicalCodeChanged();
            }
        }
        private String _NationalMedicalCode;
        partial void OnNationalMedicalCodeChanging(String value);
        partial void OnNationalMedicalCodeChanged();

        [DataMemberAttribute()]
        public String PatientRecBarCode
        {
            get
            {
                return _PatientRecBarCode;
            }
            set
            {
                OnPatientRecBarCodeChanging(value);
                _PatientRecBarCode = value;
                RaisePropertyChanged("PatientRecBarCode");
                OnPatientRecBarCodeChanged();
            }
        }
        private String _PatientRecBarCode;
        partial void OnPatientRecBarCodeChanging(String value);
        partial void OnPatientRecBarCodeChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> CreatedDate
        {
            get
            {
                return _CreatedDate;
            }
            set
            {
                OnCreatedDateChanging(value);
                _CreatedDate = value;
                RaisePropertyChanged("CreatedDate");
                OnCreatedDateChanged();
            }
        }
        private Nullable<DateTime> _CreatedDate;
        partial void OnCreatedDateChanging(Nullable<DateTime> value);
        partial void OnCreatedDateChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> FinishedDate
        {
            get
            {
                return _FinishedDate;
            }
            set
            {
                OnFinishedDateChanging(value);
                _FinishedDate = value;
                RaisePropertyChanged("FinishedDate");
                OnFinishedDateChanged();
            }
        }
        private Nullable<DateTime> _FinishedDate;
        partial void OnFinishedDateChanging(Nullable<DateTime> value);
        partial void OnFinishedDateChanged();

        //[DataMemberAttribute()]
        //public Nullable<Boolean> IsExpiredDate
        //{
        //    get
        //    {
        //        return _IsExpiredDate;
        //    }
        //    set
        //    {
        //        OnIsExpiredDateChanging(value);
        //        _IsExpiredDate = value;
        //        RaisePropertyChanged("IsExpiredDate");
        //        OnIsExpiredDateChanged();
        //    }
        //}
        //private Nullable<Boolean> _IsExpiredDate;
        //partial void OnIsExpiredDateChanging(Nullable<Boolean> value);
        //partial void OnIsExpiredDateChanged();

        #endregion

        #region Navigation Properties

        private ObservableCollection<PatientServiceRecord> _PatientServiceRecords;
        [DataMemberAttribute()]
        public ObservableCollection<PatientServiceRecord> PatientServiceRecords
        {
            get
            {
                return _PatientServiceRecords;
            }
            set
            {
                if (_PatientServiceRecords != value)
                {
                    _PatientServiceRecords = value;
                    RaisePropertyChanged("PatientServiceRecords");
                }
            }
        }

        private ObservableCollection<PatientMedicalFile> _PatientMedicalFiles;
        [DataMemberAttribute()]
        public ObservableCollection<PatientMedicalFile> PatientMedicalFiles
        {
            get
            {
                return _PatientMedicalFiles;
            }
            set
            {
                if (_PatientMedicalFiles != value)
                {
                    _PatientMedicalFiles = value;
                    RaisePropertyChanged("PatientMedicalFiles");
                }
            }
        }

        #endregion

        #region IEditableObject Members
        private PatientMedicalRecord _tempPatientMedicalRecord;
        public void BeginEdit()
        {
            _tempPatientMedicalRecord = (PatientMedicalRecord)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempPatientMedicalRecord)
                CopyFrom(_tempPatientMedicalRecord);
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(PatientMedicalRecord p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion

        public override bool Equals(object obj)
        {
            PatientMedicalRecord cond = obj as PatientMedicalRecord;
            if (cond == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PatientRecID == cond.PatientRecID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}

