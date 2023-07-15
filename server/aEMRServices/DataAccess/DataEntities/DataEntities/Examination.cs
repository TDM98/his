using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class Examination : NotifyChangedBase, IEditableObject
    {
        public Examination()
            : base()
        {

        }

        private Examination _tempExamination;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempExamination = (Examination)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempExamination)
                CopyFrom(_tempExamination);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(Examination p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new Examination object.

        /// <param name="patientRecDetailsID">Initial value of the PatientRecDetailsID property.</param>
        public static Examination CreateExamination(long patientRecDetailsID)
        {
            Examination examination = new Examination();
            examination.PatientRecDetailsID = patientRecDetailsID;
            return examination;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long PatientRecDetailsID
        {
            get
            {
                return _PatientRecDetailsID;
            }
            set
            {
                if (_PatientRecDetailsID != value)
                {
                    OnPatientRecDetailsIDChanging(value);
                    ////ReportPropertyChanging("PatientRecDetailsID");
                    _PatientRecDetailsID = value;
                    RaisePropertyChanged("PatientRecDetailsID");
                    OnPatientRecDetailsIDChanged();
                }
            }
        }
        private long _PatientRecDetailsID;
        partial void OnPatientRecDetailsIDChanging(long value);
        partial void OnPatientRecDetailsIDChanged();





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
                ////ReportPropertyChanging("ServiceRecID");
                _ServiceRecID = value;
                RaisePropertyChanged("ServiceRecID");
                OnServiceRecIDChanged();
            }
        }
        private Nullable<long> _ServiceRecID;
        partial void OnServiceRecIDChanging(Nullable<long> value);
        partial void OnServiceRecIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> MRTDetailsID
        {
            get
            {
                return _MRTDetailsID;
            }
            set
            {
                OnMRTDetailsIDChanging(value);
                ////ReportPropertyChanging("MRTDetailsID");
                _MRTDetailsID = value;
                RaisePropertyChanged("MRTDetailsID");
                OnMRTDetailsIDChanged();
            }
        }
        private Nullable<long> _MRTDetailsID;
        partial void OnMRTDetailsIDChanging(Nullable<long> value);
        partial void OnMRTDetailsIDChanged();





        [DataMemberAttribute()]
        public String PRDTextValue
        {
            get
            {
                return _PRDTextValue;
            }
            set
            {
                OnPRDTextValueChanging(value);
                ////ReportPropertyChanging("PRDTextValue");
                _PRDTextValue = value;
                RaisePropertyChanged("PRDTextValue");
                OnPRDTextValueChanged();
            }
        }
        private String _PRDTextValue;
        partial void OnPRDTextValueChanging(String value);
        partial void OnPRDTextValueChanged();





        [DataMemberAttribute()]
        public Nullable<Boolean> PRDYesNo
        {
            get
            {
                return _PRDYesNo;
            }
            set
            {
                OnPRDYesNoChanging(value);
                ////ReportPropertyChanging("PRDYesNo");
                _PRDYesNo = value;
                RaisePropertyChanged("PRDYesNo");
                OnPRDYesNoChanged();
            }
        }
        private Nullable<Boolean> _PRDYesNo;
        partial void OnPRDYesNoChanging(Nullable<Boolean> value);
        partial void OnPRDYesNoChanged();





        [DataMemberAttribute()]
        public String PRDExplainOrNotes
        {
            get
            {
                return _PRDExplainOrNotes;
            }
            set
            {
                OnPRDExplainOrNotesChanging(value);
                ////ReportPropertyChanging("PRDExplainOrNotes");
                _PRDExplainOrNotes = value;
                RaisePropertyChanged("PRDExplainOrNotes");
                OnPRDExplainOrNotesChanged();
            }
        }
        private String _PRDExplainOrNotes;
        partial void OnPRDExplainOrNotesChanging(String value);
        partial void OnPRDExplainOrNotesChanged();

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_EXAMINAT_REL_PMR10_MRTDETAI", "MRTDetails")]
        public MRTDetail MRTDetail
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_EXAMINAT_REL_PMR30_PATIENTS", "PatientServiceRecords")]
        public PatientServiceRecord PatientServiceRecord
        {
            get;
            set;
        }

        #endregion
    }
}
