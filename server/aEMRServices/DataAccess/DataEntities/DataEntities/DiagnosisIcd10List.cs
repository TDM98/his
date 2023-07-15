using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class DiagnosisIcd10List : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new DiagnosisIcd10List object.

        /// <param name="ICD10ListID">Initial value of the ICD10ListID property.</param>
        public static DiagnosisIcd10List CreateDiagnosisIcd10List(long ICD10ListID)
        {
            DiagnosisIcd10List DiagnosisIcd10List = new DiagnosisIcd10List();
            DiagnosisIcd10List.ICD10ListID = ICD10ListID;
            return DiagnosisIcd10List;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public long ICD10ListID
        {
            get
            {
                return _ICD10ListID;
            }
            set
            {
                if (_ICD10ListID != value)
                {
                    OnICD10ListIDChanging(value);
                    _ICD10ListID = value;
                    RaisePropertyChanged("ICD10ListID");
                    OnICD10ListIDChanged();
                }
            }
        }
        private long _ICD10ListID;
        partial void OnICD10ListIDChanging(long value);
        partial void OnICD10ListIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> ServiceRecID
        {
            get
            {
                return _ServiceRecID;
            }
            set
            {
                OnServiceRecIDChanging(value);
                _ServiceRecID = value;
                RaisePropertyChanged("ServiceRecID");
                OnServiceRecIDChanged();
            }
        }
        private Nullable<long> _ServiceRecID;
        partial void OnServiceRecIDChanging(Nullable<long> value);
        partial void OnServiceRecIDChanged();

        [DataMemberAttribute()]
        public long DiagnosisIcd10ListID
        {
            get
            {
                return _DiagnosisIcd10ListID;
            }
            set
            {
                OnDiagnosisIcd10ListIDChanging(value);
                _DiagnosisIcd10ListID = value;
                RaisePropertyChanged("DiagnosisIcd10ListID");
                OnDiagnosisIcd10ListIDChanged();
            }
        }
        private long _DiagnosisIcd10ListID;
        partial void OnDiagnosisIcd10ListIDChanging(long value);
        partial void OnDiagnosisIcd10ListIDChanged();
        
        #endregion

        #region Navigation Properties

        private PatientServiceRecord _PatientServiceRecord;
        [DataMemberAttribute()]
        public PatientServiceRecord PatientServiceRecord
        {
            get
            {
                return _PatientServiceRecord;
            }
            set
            {
                if (_PatientServiceRecord != value)
                {
                    _PatientServiceRecord = value;
                    RaisePropertyChanged("PatientServiceRecord");
                }
            }
        }

        #endregion

        public override bool Equals(object obj)
        {
            DiagnosisIcd10List cond = obj as DiagnosisIcd10List;
            if (cond == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.ICD10ListID == cond.ICD10ListID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
