using eHCMS.Services.Core.Base;
using System;
using System.Runtime.Serialization;

namespace DataEntities
{
    public partial class PCLResultFileStorageDetailSearchCriteria : NotifyChangedBase
    {
        #region PatientPCLRequest
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
        public long PatientPCLReqID
        {
            get
            {
                return _PatientPCLReqID;
            }
            set
            {
                OnPatientPCLReqIDChanging(value);
                _PatientPCLReqID = value;
                RaisePropertyChanged("PatientPCLReqID");
                OnPatientPCLReqIDChanged();
            }
        }
        private long _PatientPCLReqID;
        partial void OnPatientPCLReqIDChanging(long value);
        partial void OnPatientPCLReqIDChanged();

        [DataMemberAttribute()]
        public long PCLGroupID
        {
            get
            {
                return _PCLGroupID;
            }
            set
            {
                OnPCLGroupIDChanging(value);
                _PCLGroupID = value;
                RaisePropertyChanged("PCLGroupID");
                OnPCLGroupIDChanged();
            }
        }
        private long _PCLGroupID;
        partial void OnPCLGroupIDChanging(long value);
        partial void OnPCLGroupIDChanged();


        [DataMemberAttribute()]
        public long PCLExamTypeID
        {
            get
            {
                return _PCLExamTypeID;
            }
            set
            {
                OnPCLExamTypeIDChanging(value);
                _PCLExamTypeID = value;
                RaisePropertyChanged("PCLExamTypeID");
                OnPCLExamTypeIDChanged();
            }
        }
        private long _PCLExamTypeID;
        partial void OnPCLExamTypeIDChanging(long value);
        partial void OnPCLExamTypeIDChanged();


        [DataMemberAttribute()]
        public Nullable<Boolean> IsExternalExam
        {
            get
            {
                return _IsExternalExam;
            }
            set
            {
                OnIsExternalExamChanging(value);
                _IsExternalExam = value;
                RaisePropertyChanged("IsExternalExam");
                OnIsExternalExamChanged();
            }
        }
        private Nullable<Boolean> _IsExternalExam;
        partial void OnIsExternalExamChanging(Nullable<Boolean> value);
        partial void OnIsExternalExamChanged();

        private long _V_PCLRequestType;
        [DataMemberAttribute]
        public long V_PCLRequestType
        {
            get
            {
                return _V_PCLRequestType;
            }
            set
            {
                if (_V_PCLRequestType == value)
                {
                    return;
                }
                _V_PCLRequestType = value;
                RaisePropertyChanged("V_PCLRequestType");
            }
        }
        #endregion
    }
}