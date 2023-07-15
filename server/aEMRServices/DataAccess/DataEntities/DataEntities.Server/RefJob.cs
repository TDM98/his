using eHCMS.Services.Core.Base;
using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;


namespace DataEntities
{
    public partial class RefJob: NotifyChangedBase
    {
        #region Factory Method
        public static RefJob CreateRefJob(long JobID, String JobName, String JobCode, long JobParentID)
        {
            RefJob RefJob = new RefJob();
            RefJob.JobID = JobID;
            RefJob.JobName = JobName;
            RefJob.JobCode = JobCode;
            RefJob.JobParentID = JobParentID;
            return RefJob;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long JobID
        {
            get
            {
                return _JobID;
            }
            set
            {
                if (_JobID != value)
                {
                    OnJobIDChanging(value);
                    _JobID = value;
                    RaisePropertyChanged("JobID");
                    OnJobIDChanged();
                }
            }
        }
        private long _JobID;
        partial void OnJobIDChanging(long value);
        partial void OnJobIDChanged();

        [DataMemberAttribute()]
        public long JobParentID
        {
            get
            {
                return _JobParentID;
            }
            set
            {
                if (_JobParentID != value)
                {
                    OnJobParentIDChanging(value);
                    _JobParentID = value;
                    RaisePropertyChanged("JobParentID");
                    OnJobParentIDChanged();
                }
            }
        }
        private long _JobParentID;
        partial void OnJobParentIDChanging(long value);
        partial void OnJobParentIDChanged();

        [DataMemberAttribute()]
        public String JobName
        {
            get
            {
                return _JobName;
            }
            set
            {
                OnJobNameChanging(value);
                _JobName = value;
                RaisePropertyChanged("JobName");
                OnJobNameChanged();
            }
        }
        private String _JobName;
        partial void OnJobNameChanging(String value);
        partial void OnJobNameChanged();

        [DataMemberAttribute()]
        public String JobCode
        {
            get
            {
                return _JobCode;
            }
            set
            {
                OnJobCodeChanging(value);
                _JobCode = value;
                RaisePropertyChanged("JobCode");
                OnJobCodeChanged();
            }
        }
        private String _JobCode;
        partial void OnJobCodeChanging(String value);
        partial void OnJobCodeChanged();

        #endregion

        #region Navigation Properties       

        #endregion
        public override bool Equals(object obj)
        {
            RefJob seletedJob = obj as RefJob;
            if (seletedJob == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.JobID == seletedJob.JobID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
