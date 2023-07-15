using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class PatientClassHistory : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new PatientClassHistory object.

        /// <param name="pCHisID">Initial value of the PCHisID property.</param>
        /// <param name="patientClassID">Initial value of the PatientClassID property.</param>
        /// <param name="patientID">Initial value of the PatientID property.</param>
        /// <param name="pCFromDate">Initial value of the PCFromDate property.</param>
        public static PatientClassHistory CreatePatientClassHistory(Int64 pCHisID, long patientClassID, long patientID, DateTime pCFromDate)
        {
            PatientClassHistory patientClassHistory = new PatientClassHistory();
            patientClassHistory.PCHisID = pCHisID;
            patientClassHistory.PatientClassID = patientClassID;
            patientClassHistory.PatientID = patientID;
            patientClassHistory.PCFromDate = pCFromDate;
            return patientClassHistory;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public Int64 PCHisID
        {
            get
            {
                return _PCHisID;
            }
            set
            {
                if (_PCHisID != value)
                {
                    OnPCHisIDChanging(value);
                    ////ReportPropertyChanging("PCHisID");
                    _PCHisID = value;
                    RaisePropertyChanged("PCHisID");
                    OnPCHisIDChanged();
                }
            }
        }
        private Int64 _PCHisID;
        partial void OnPCHisIDChanging(Int64 value);
        partial void OnPCHisIDChanged();





        [DataMemberAttribute()]
        public long PatientClassID
        {
            get
            {
                return _PatientClassID;
            }
            set
            {
                OnPatientClassIDChanging(value);
                ////ReportPropertyChanging("PatientClassID");
                _PatientClassID = value;
                RaisePropertyChanged("PatientClassID");
                OnPatientClassIDChanged();
            }
        }
        private long _PatientClassID;
        partial void OnPatientClassIDChanging(long value);
        partial void OnPatientClassIDChanged();





        [DataMemberAttribute()]
        public long PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                OnPatientIDChanging(value);
                ////ReportPropertyChanging("PatientID");
                _PatientID = value;
                RaisePropertyChanged("PatientID");
                OnPatientIDChanged();
            }
        }
        private long _PatientID;
        partial void OnPatientIDChanging(long value);
        partial void OnPatientIDChanged();





        [DataMemberAttribute()]
        public DateTime PCFromDate
        {
            get
            {
                return _PCFromDate;
            }
            set
            {
                OnPCFromDateChanging(value);
                ////ReportPropertyChanging("PCFromDate");
                _PCFromDate = value;
                RaisePropertyChanged("PCFromDate");
                OnPCFromDateChanged();
            }
        }
        private DateTime _PCFromDate;
        partial void OnPCFromDateChanging(DateTime value);
        partial void OnPCFromDateChanged();





        [DataMemberAttribute()]
        public Nullable<DateTime> PCToDate
        {
            get
            {
                return _PCToDate;
            }
            set
            {
                OnPCToDateChanging(value);
                ////ReportPropertyChanging("PCToDate");
                _PCToDate = value;
                RaisePropertyChanged("PCToDate");
                OnPCToDateChanged();
            }
        }
        private Nullable<DateTime> _PCToDate;
        partial void OnPCToDateChanging(Nullable<DateTime> value);
        partial void OnPCToDateChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PATIENTC_REL_PTINF_PATIENTC", "PatientClassification")]
        public PatientClassification PatientClassification
        {
            get;
            set;
        }


        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PATIENTC_REL_PTINF_PATIENTS", "Patients")]
        public Patient Patient
        {
            get;
            set;
        }

        #endregion
    }
}
