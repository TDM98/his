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
    
        #region Extended Properties

        [DataMemberAttribute()]
        public Nullable<Int64> PtRegistrationID
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
        private Nullable<Int64> _PtRegistrationID;
        partial void OnPtRegistrationIDChanging(Nullable<Int64> value);
        partial void OnPtRegistrationIDChanged();

        [DataMemberAttribute()]
        public DateTime ExamDate
        {
            get
            {
                return _ExamDate;
            }
            set
            {
                OnExamDateChanging(value);
                _ExamDate = value;
                RaisePropertyChanged("ExamDate");
                OnExamDateChanged();
            }
        }
        private DateTime _ExamDate;
        partial void OnExamDateChanging(DateTime value);
        partial void OnExamDateChanged();

        [DataMemberAttribute()]
        public Nullable<long> LatestPrescriptionID
        {
            get
            {
                return _LatestPrescriptionID;
            }
            set
            {
                if (_LatestPrescriptionID != value)
                {
                    OnLatestPrescriptionIDChanging(value);
                    _LatestPrescriptionID = value;
                    RaisePropertyChanged("LatestPrescriptionID");
                    OnLatestPrescriptionIDChanged();
                }
            }
        }
        private Nullable<long> _LatestPrescriptionID;
        partial void OnLatestPrescriptionIDChanging(Nullable<long> value);
        partial void OnLatestPrescriptionIDChanged();


        #endregion
    
    }
}
