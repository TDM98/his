using System;
using System.Net;
using System.Windows;

using System.Runtime.Serialization;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public class ScientificResearchActivities : NotifyChangedBase
    {
        [DataMemberAttribute()]
        public Nullable<long> ActivityID
        {
            get
            {
                return _ActivityID;
            }
            set
            {
                _ActivityID = value;
                RaisePropertyChanged("ActivityID");
            }
        }
        private Nullable<long> _ActivityID;


        [DataMemberAttribute()]
        public string ActivityName
        {
            get
            {
                return _ActivityName;
            }
            set
            {
                _ActivityName = value;
                RaisePropertyChanged("ActivityName");
            }
        }
        private string _ActivityName;


        [DataMemberAttribute()]
        public Nullable<Int64> V_ActivityType
        {
            get
            {
                return _V_ActivityType;
            }
            set
            {
                _V_ActivityType = value;
                RaisePropertyChanged("V_ActivityType");
            }
        }
        private Nullable<Int64> _V_ActivityType;

        [DataMemberAttribute()]
        public Nullable<Int64> V_ActivityMethodType
        {
            get
            {
                return _V_ActivityMethodType;
            }
            set
            {
                _V_ActivityMethodType = value;
                RaisePropertyChanged("V_ActivityMethodType");
            }
        }
        private Nullable<Int64> _V_ActivityMethodType;


        [DataMemberAttribute()]
        public Nullable<DateTime> StartDate
        {
            get
            {
                return _StartDate;
            }
            set
            {
                _StartDate = value;
                RaisePropertyChanged("StartDate");
            }
        }
        private Nullable<DateTime> _StartDate;


        [DataMemberAttribute()]
        public Nullable<DateTime> ApprovedDate
        {
            get
            {
                return _ApprovedDate;
            }
            set
            {
                _ApprovedDate = value;
                RaisePropertyChanged("ApprovedDate");
            }
        }
        private Nullable<DateTime> _ApprovedDate;

        [DataMemberAttribute()]
        public Nullable<DateTime> AcceptedDate
        {
            get
            {
                return _AcceptedDate;
            }
            set
            {
                _AcceptedDate = value;
                RaisePropertyChanged("AcceptedDate");
            }
        }
        private Nullable<DateTime> _AcceptedDate;

        [DataMemberAttribute()]
        public Nullable<DateTime> EndDate
        {
            get
            {
                return _EndDate;
            }
            set
            {
                _EndDate = value;
                RaisePropertyChanged("EndDate");
            }
        }
        private Nullable<DateTime> _EndDate;


        [DataMemberAttribute()]
        public string AttendeeName
        {
            get
            {
                return _AttendeeName;
            }
            set
            {
                _AttendeeName = value;
                RaisePropertyChanged("AttendeeName");
            }
        }
        private string _AttendeeName;

        [DataMemberAttribute()]
        public string Note
        {
            get
            {
                return _Note;
            }
            set
            {
                _Note = value;
                RaisePropertyChanged("Note");
            }
        }
        private string _Note;

        [DataMemberAttribute()]
        public Nullable<DateTime> RecCreatedDate
        {
            get
            {
                return _RecCreatedDate;
            }
            set
            {
                _RecCreatedDate = value;
                RaisePropertyChanged("RecCreatedDate");
            }
        }
        private Nullable<DateTime> _RecCreatedDate;


        [DataMemberAttribute()]
        public string V_ActivityTypeName
        {
            get
            {
                return _V_ActivityTypeName;
            }
            set
            {
                _V_ActivityTypeName = value;
                RaisePropertyChanged("V_ActivityTypeName");
            }
        }
        private string _V_ActivityTypeName;

        [DataMemberAttribute()]
        public string V_ActivityMethodTypeName
        {
            get
            {
                return _V_ActivityMethodTypeName;
            }
            set
            {
                _V_ActivityMethodTypeName = value;
                RaisePropertyChanged("V_ActivityMethodTypeName");
            }
        }
        private string _V_ActivityMethodTypeName;

    }


}
