using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{

    public partial class JobResult : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new JobResult object.

        /// <param name="jR_ID">Initial value of the JR_ID property.</param>
        public static JobResult CreateJobResult(Int32 jR_ID)
        {
            JobResult jobResult = new JobResult();
            jobResult.JR_ID = jR_ID;
            return jobResult;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public Int32 JR_ID
        {
            get
            {
                return _JR_ID;
            }
            set
            {
                if (_JR_ID != value)
                {
                    OnJR_IDChanging(value);
                    ////ReportPropertyChanging("JR_ID");
                    _JR_ID = value;
                    RaisePropertyChanged("JR_ID");
                    OnJR_IDChanged();
                }
            }
        }
        private Int32 _JR_ID;
        partial void OnJR_IDChanging(Int32 value);
        partial void OnJR_IDChanged();





        [DataMemberAttribute()]
        public Nullable<Int64> StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                OnStaffIDChanging(value);
                ////ReportPropertyChanging("StaffID");
                _StaffID = value;
                RaisePropertyChanged("StaffID");
                OnStaffIDChanged();
            }
        }
        private Nullable<Int64> _StaffID;
        partial void OnStaffIDChanging(Nullable<Int64> value);
        partial void OnStaffIDChanged();





        [DataMemberAttribute()]
        public Nullable<Int32> SJ_ID
        {
            get
            {
                return _SJ_ID;
            }
            set
            {
                OnSJ_IDChanging(value);
                ////ReportPropertyChanging("SJ_ID");
                _SJ_ID = value;
                RaisePropertyChanged("SJ_ID");
                OnSJ_IDChanged();
            }
        }
        private Nullable<Int32> _SJ_ID;
        partial void OnSJ_IDChanging(Nullable<Int32> value);
        partial void OnSJ_IDChanged();





        [DataMemberAttribute()]
        public String Comment
        {
            get
            {
                return _Comment;
            }
            set
            {
                OnCommentChanging(value);
                ////ReportPropertyChanging("Comment");
                _Comment = value;
                RaisePropertyChanged("Comment");
                OnCommentChanged();
            }
        }
        private String _Comment;
        partial void OnCommentChanging(String value);
        partial void OnCommentChanged();





        [DataMemberAttribute()]
        public Nullable<DateTime> DateCreated
        {
            get
            {
                return _DateCreated;
            }
            set
            {
                OnDateCreatedChanging(value);
                ////ReportPropertyChanging("DateCreated");
                _DateCreated = value;
                RaisePropertyChanged("DateCreated");
                OnDateCreatedChanged();
            }
        }
        private Nullable<DateTime> _DateCreated;
        partial void OnDateCreatedChanging(Nullable<DateTime> value);
        partial void OnDateCreatedChanged();





        [DataMemberAttribute()]
        public Nullable<DateTime> DateModified
        {
            get
            {
                return _DateModified;
            }
            set
            {
                OnDateModifiedChanging(value);
                ////ReportPropertyChanging("DateModified");
                _DateModified = value;
                RaisePropertyChanged("DateModified");
                OnDateModifiedChanged();
            }
        }
        private Nullable<DateTime> _DateModified;
        partial void OnDateModifiedChanging(Nullable<DateTime> value);
        partial void OnDateModifiedChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_JOBRESUL_REL_PTAPP_SCHEDULE", "ScheduledJob")]
        public ScheduledJob ScheduledJob
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_JOBRESUL_REL_PTAPP_STAFFS", "Staffs")]
        public Staff Staff
        {
            get;
            set;
        }

        #endregion
    }
}
