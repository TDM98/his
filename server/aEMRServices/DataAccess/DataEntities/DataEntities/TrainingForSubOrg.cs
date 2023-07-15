using System;
using System.Net;
using System.Windows;

using System.Runtime.Serialization;
using eHCMS.Services.Core.Base;
namespace DataEntities
{
    public class TrainingForSubOrg : NotifyChangedBase
    {
        [DataMemberAttribute()]
        public Nullable<long> TrainingID
        {
            get
            {
                return _TrainingID;
            }
            set
            {
                _TrainingID = value;
                RaisePropertyChanged("TrainingID");
            }
        }
        private Nullable<long> _TrainingID;


        [DataMemberAttribute()]
        public string TrainingName
        {
            get
            {
                return _TrainingName;
            }
            set
            {
                _TrainingName = value;
                RaisePropertyChanged("TrainingName");
            }
        }
        private string _TrainingName;


        [DataMemberAttribute()]
        public Nullable<DateTime> TrainingStartDate
        {
            get
            {
                return _TrainingStartDate;
            }
            set
            {
                _TrainingStartDate = value;
                RaisePropertyChanged("TrainingStartDate");
            }
        }
        private Nullable<DateTime> _TrainingStartDate;


        [DataMemberAttribute()]
        public Nullable<DateTime> TrainingEndDate
        {
            get
            {
                return _TrainingEndDate;
            }
            set
            {
                _TrainingEndDate = value;
                RaisePropertyChanged("TrainingEndDate");
            }
        }
        private Nullable<DateTime> _TrainingEndDate;


        [DataMemberAttribute()]
        public string TrainingPerson
        {
            get
            {
                return _TrainingPerson;
            }
            set
            {
                _TrainingPerson = value;
                RaisePropertyChanged("TrainingPerson");
            }
        }
        private string _TrainingPerson;


        [DataMemberAttribute()]
        public Nullable<int> TotalAttendees
        {
            get
            {
                return _TotalAttendees;
            }
            set
            {
                _TotalAttendees = value;
                RaisePropertyChanged("TotalAttendees");
            }
        }
        private Nullable<int> _TotalAttendees;


        [DataMemberAttribute()]
        public string TrainingPlace
        {
            get
            {
                return _TrainingPlace;
            }
            set
            {
                _TrainingPlace = value;
                RaisePropertyChanged("TrainingPlace");
            }
        }
        private string _TrainingPlace;


        [DataMemberAttribute()]
        public Nullable<long> V_TrainingType
        {
            get
            {
                return _V_TrainingType;
            }
            set
            {
                _V_TrainingType = value;
                RaisePropertyChanged("V_TrainingType");
            }
        }
        private Nullable<long> _V_TrainingType;


        [DataMemberAttribute()]
        public string V_TrainingTypeName
        {
            get
            {
                return _V_TrainingTypeName;
            }
            set
            {
                _V_TrainingTypeName = value;
                RaisePropertyChanged("V_TrainingTypeName");
            }
        }
        private string _V_TrainingTypeName;

        [DataMemberAttribute()]
        public Nullable<long> ActivityClassID
        {
            get
            {
                return _ActivityClassID;
            }
            set
            {
                _ActivityClassID = value;
                RaisePropertyChanged("ActivityClassID");
            }
        }
        private Nullable<long> _ActivityClassID;

          [DataMemberAttribute()]
        public Nullable<long> V_ActivityClassType
        {
            get
            {
                return _V_ActivityClassType;
            }
            set
            {
                _V_ActivityClassType = value;
                RaisePropertyChanged("V_ActivityClassType");
            }
        }
          private Nullable<long> _V_ActivityClassType;




    }

}
