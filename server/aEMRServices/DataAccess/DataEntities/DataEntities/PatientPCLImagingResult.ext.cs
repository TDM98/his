using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;


namespace DataEntities
{
    public partial class PatientPCLImagingResult : NotifyChangedBase
    {

        #region Adding PatientServiceRecords

        [DataMemberAttribute()]
        public Nullable<Int64> PtRegDetailID
        {
            get
            {
                return _PtRegDetailID;
            }
            set
            {
                OnPtRegDetailIDChanging(value);
                _PtRegDetailID = value;
                RaisePropertyChanged("PtRegDetailID");
                OnPtRegDetailIDChanged();
            }
        }
        private Nullable<Int64> _PtRegDetailID;
        partial void OnPtRegDetailIDChanging(Nullable<Int64> value);
        partial void OnPtRegDetailIDChanged();

        private Staff _Doctor;
        [DataMemberAttribute()]
        public Staff Doctor
        {
            get
            {
                return _Doctor;
            }
            set
            {
                if (_Doctor != value)
                {
                    _Doctor = value;
                    RaisePropertyChanged("Doctor");
                }
            }
        }

     
        
     
        [DataMemberAttribute()]
        public Nullable<DateTime> ExamDate
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
        private Nullable<DateTime> _ExamDate;
        partial void OnExamDateChanging(Nullable<DateTime> value);
        partial void OnExamDateChanged();

     
        
     
        [DataMemberAttribute()]
        public Nullable<Int64> V_ProcessingType
        {
            get
            {
                return _V_ProcessingType;
            }
            set
            {
                OnV_ProcessingTypeChanging(value);
                _V_ProcessingType = value;
                RaisePropertyChanged("V_ProcessingType");
                OnV_ProcessingTypeChanged();
            }
        }
        private Nullable<Int64> _V_ProcessingType;
        partial void OnV_ProcessingTypeChanging(Nullable<Int64> value);
        partial void OnV_ProcessingTypeChanged();

     
        
     
        [DataMemberAttribute()]
        public String ProcessingType
        {
            get
            {
                return _ProcessingType;
            }
            set
            {
                OnProcessingTypeChanging(value);
                _ProcessingType = value;
                RaisePropertyChanged("ProcessingType");
                OnProcessingTypeChanged();
            }
        }
        private String _ProcessingType;
        partial void OnProcessingTypeChanging(String value);
        partial void OnProcessingTypeChanged();

     
        
     
        [DataMemberAttribute()]
        public Nullable<Int64> V_Behaving
        {
            get
            {
                return _V_Behaving;
            }
            set
            {
                OnV_BehavingChanging(value);
                _V_Behaving = value;
                RaisePropertyChanged("V_Behaving");
                OnV_BehavingChanged();
            }
        }
        private Nullable<Int64> _V_Behaving;
        partial void OnV_BehavingChanging(Nullable<Int64> value);
        partial void OnV_BehavingChanged();

     
        
     
        [DataMemberAttribute()]
        public String Behaving
        {
            get
            {
                return _Behaving;
            }
            set
            {
                OnBehavingChanging(value);
                _Behaving = value;
                RaisePropertyChanged("Behaving");
                OnBehavingChanged();
            }
        }
        private String _Behaving;
        partial void OnBehavingChanging(String value);
        partial void OnBehavingChanged();

        #endregion


        #region add property
        [DataMemberAttribute()]
        public long PCLExamGroupID
        {
            get
            {
                return _PCLExamGroupID;
            }
            set
            {
                OnPCLExamGroupIDChanging(value);
                _PCLExamGroupID = value;
                RaisePropertyChanged("PCLExamGroupID");
                OnPCLExamGroupIDChanged();
            }
        }
        private long _PCLExamGroupID;
        partial void OnPCLExamGroupIDChanging(long value);
        partial void OnPCLExamGroupIDChanged();
        #endregion
    }
}
