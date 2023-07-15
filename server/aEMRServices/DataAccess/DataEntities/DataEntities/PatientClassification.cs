using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class PatientClassification : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new PatientClassification object.

        /// <param name="patientClassID">Initial value of the PatientClassID property.</param>
        /// <param name="patientClassName">Initial value of the PatientClassName property.</param>
        public static PatientClassification CreatePatientClassification(long patientClassID, String patientClassName)
        {
            PatientClassification patientClassification = new PatientClassification();
            patientClassification.PatientClassID = patientClassID;
            patientClassification.PatientClassName = patientClassName;
            return patientClassification;
        }

        #endregion
        #region Primitive Properties


        [DataMemberAttribute()]
        public long PatientClassID
        {
            get
            {
                return _PatientClassID;
            }
            set
            {
                if (_PatientClassID != value)
                {
                    OnPatientClassIDChanging(value);
                    ////ReportPropertyChanging("PatientClassID");
                    _PatientClassID = value;
                    RaisePropertyChanged("PatientClassID");
                    OnPatientClassIDChanged();
                }
            }
        }
        private long _PatientClassID;
        partial void OnPatientClassIDChanging(long value);
        partial void OnPatientClassIDChanged();


        [DataMemberAttribute()]
        public String PatientClassName
        {
            get
            {
                return _PatientClassName;
            }
            set
            {
                OnPatientClassNameChanging(value);
                ////ReportPropertyChanging("PatientClassName");
                _PatientClassName = value;
                RaisePropertyChanged("PatientClassName");
                OnPatientClassNameChanged();
            }
        }
        private String _PatientClassName;
        partial void OnPatientClassNameChanging(String value);
        partial void OnPatientClassNameChanged();


        [DataMemberAttribute()]
        public String PCNotes
        {
            get
            {
                return _PCNotes;
            }
            set
            {
                OnPCNotesChanging(value);
                ////ReportPropertyChanging("PCNotes");
                _PCNotes = value;
                RaisePropertyChanged("PCNotes");
                OnPCNotesChanged();
            }
        }
        private String _PCNotes;
        partial void OnPCNotesChanging(String value);
        partial void OnPCNotesChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PATIENTC_REL_PTINF_PATIENTC", "PatientClassHistory")]
        public ObservableCollection<PatientClassHistory> PatientClassHistories
        {
            get;
            set;
        }

        #endregion

        public PatientType PatientType
        {
            get
            {
                switch ((int)_PatientClassID)
                {
                    case 1:
                        return PatientType.NORMAL_PATIENT;
                    case 2:
                    case 6:
                        return PatientType.INSUARED_PATIENT;
                    case 7:
                        return PatientType.TRANSFERRED_PATIENT;

                    default:
                        return PatientType.OTHERS;
                }
            }
        }

        public override bool Equals(object obj)
        {
            PatientClassification info = obj as PatientClassification;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PatientClassID == info.PatientClassID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
