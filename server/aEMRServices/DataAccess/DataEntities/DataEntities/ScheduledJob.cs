using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{

    public partial class ScheduledJob : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new ScheduledJob object.

        /// <param name="sJ_ID">Initial value of the SJ_ID property.</param>
        /// <param name="jobName">Initial value of the JobName property.</param>
        /// <param name="jobDescription">Initial value of the JobDescription property.</param>
        /// <param name="timeBegin">Initial value of the TimeBegin property.</param>
        public static ScheduledJob CreateScheduledJob(Int32 sJ_ID, String jobName, String jobDescription, DateTime timeBegin)
        {
            ScheduledJob scheduledJob = new ScheduledJob();
            scheduledJob.SJ_ID = sJ_ID;
            scheduledJob.JobName = jobName;
            scheduledJob.JobDescription = jobDescription;
            scheduledJob.TimeBegin = timeBegin;
            return scheduledJob;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public Int32 SJ_ID
        {
            get
            {
                return _SJ_ID;
            }
            set
            {
                if (_SJ_ID != value)
                {
                    OnSJ_IDChanging(value);
                    ////ReportPropertyChanging("SJ_ID");
                    _SJ_ID = value;
                    RaisePropertyChanged("SJ_ID");
                    OnSJ_IDChanged();
                }
            }
        }
        private Int32 _SJ_ID;
        partial void OnSJ_IDChanging(Int32 value);
        partial void OnSJ_IDChanged();





        [DataMemberAttribute()]
        public Nullable<long> SCheduleID
        {
            get
            {
                return _SCheduleID;
            }
            set
            {
                OnSCheduleIDChanging(value);
                ////ReportPropertyChanging("SCheduleID");
                _SCheduleID = value;
                RaisePropertyChanged("SCheduleID");
                OnSCheduleIDChanged();
            }
        }
        private Nullable<long> _SCheduleID;
        partial void OnSCheduleIDChanging(Nullable<long> value);
        partial void OnSCheduleIDChanged();





        [DataMemberAttribute()]
        public Nullable<long> MedServiceID
        {
            get
            {
                return _MedServiceID;
            }
            set
            {
                OnMedServiceIDChanging(value);
                ////ReportPropertyChanging("MedServiceID");
                _MedServiceID = value;
                RaisePropertyChanged("MedServiceID");
                OnMedServiceIDChanged();
            }
        }
        private Nullable<long> _MedServiceID;
        partial void OnMedServiceIDChanging(Nullable<long> value);
        partial void OnMedServiceIDChanged();





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
                ////ReportPropertyChanging("JobName");
                _JobName = value;
                RaisePropertyChanged("JobName");
                OnJobNameChanged();
            }
        }
        private String _JobName;
        partial void OnJobNameChanging(String value);
        partial void OnJobNameChanged();





        [DataMemberAttribute()]
        public String JobDescription
        {
            get
            {
                return _JobDescription;
            }
            set
            {
                OnJobDescriptionChanging(value);
                ////ReportPropertyChanging("JobDescription");
                _JobDescription = value;
                RaisePropertyChanged("JobDescription");
                OnJobDescriptionChanged();
            }
        }
        private String _JobDescription;
        partial void OnJobDescriptionChanging(String value);
        partial void OnJobDescriptionChanged();





        [DataMemberAttribute()]
        public DateTime TimeBegin
        {
            get
            {
                return _TimeBegin;
            }
            set
            {
                OnTimeBeginChanging(value);
                ////ReportPropertyChanging("TimeBegin");
                _TimeBegin = value;
                RaisePropertyChanged("TimeBegin");
                OnTimeBeginChanged();
            }
        }
        private DateTime _TimeBegin;
        partial void OnTimeBeginChanging(DateTime value);
        partial void OnTimeBeginChanged();





        [DataMemberAttribute()]
        public Nullable<DateTime> TimeEnd
        {
            get
            {
                return _TimeEnd;
            }
            set
            {
                OnTimeEndChanging(value);
                ////ReportPropertyChanging("TimeEnd");
                _TimeEnd = value;
                RaisePropertyChanged("TimeEnd");
                OnTimeEndChanged();
            }
        }
        private Nullable<DateTime> _TimeEnd;
        partial void OnTimeEndChanging(Nullable<DateTime> value);
        partial void OnTimeEndChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_JOBRESUL_REL_PTAPP_SCHEDULE", "JobResult")]
        public ObservableCollection<JobResult> JobResults
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_SCHEDULE_REL_PTAPP_REFMEDIC", "RefMedicalServiceItems")]
        public RefMedicalServiceItem RefMedicalServiceItem
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_SCHEDULE_REL_PTAPP_WORKINGS", "WorkingSchedules")]
        public WorkingSchedule WorkingSchedule
        {
            get;
            set;
        }

        #endregion
    }
}
