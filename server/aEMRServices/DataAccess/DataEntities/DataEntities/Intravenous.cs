using System;
using System.Net;
using System.Windows;
using System.Runtime.Serialization;
using eHCMS.Services.Core.Base;
using System.Collections.ObjectModel;

namespace DataEntities
{
    public class Intravenous : NotifyChangedBase
    {
        private long _intravenousID;
        [DataMemberAttribute()]
        public long IntravenousID
        {
            get 
            {
                return _intravenousID;
            }
            set
            {
                _intravenousID = value;
                RaisePropertyChanged("IntravenousID");
            }
        }

        
        private Lookup _V_InfusionType;
        [DataMemberAttribute()]
        public Lookup V_InfusionType
        {
            get
            {
                return _V_InfusionType;
            }
            set
            {
                _V_InfusionType = value;
                RaisePropertyChanged("V_InfusionType");
            }
        }

        private Lookup _V_InfusionProcessType;
        [DataMemberAttribute()]
        public Lookup V_InfusionProcessType
        {
            get
            {
                return _V_InfusionProcessType;
            }
            set
            {
                _V_InfusionProcessType = value;
                RaisePropertyChanged("V_InfusionProcessType");
            }
        }

        private string _flowRate;
        [DataMemberAttribute()]
        public string FlowRate
        {
            get
            {
                return _flowRate;
            }
            set
            {
                _flowRate = value;
                RaisePropertyChanged("FlowRate");
            }
        }

        private decimal _infusionTime;
        [DataMemberAttribute()]
        public decimal InfusionTime
        {
            get
            {
                return _infusionTime;
            }
            set
            {
                _infusionTime = value;
                RaisePropertyChanged("InfusionTime");
            }
        }

        private string _numOfTimes;
        [DataMemberAttribute()]
        public string NumOfTimes
        {
            get
            {
                return _numOfTimes;
            }
            set
            {
                _numOfTimes = value;
                RaisePropertyChanged("NumOfTimes");
            }
        }

        private Int16 _timeInterval;
        [DataMemberAttribute()]
        public Int16 TimeInterval
        {
            get
            {
                return _timeInterval;
            }
            set
            {
                _timeInterval = value;
                RaisePropertyChanged("TimeInterval");
            }
        }

        private Lookup _V_TimeIntervalUnit;
        [DataMemberAttribute()]
        public Lookup V_TimeIntervalUnit
        {
            get
            {
                return _V_TimeIntervalUnit;
            }
            set
            {
                _V_TimeIntervalUnit = value;
                RaisePropertyChanged("V_TimeIntervalUnit");
            }
        }

        private Nullable<DateTime> _startDateTime;
        [DataMemberAttribute()]
        public Nullable<DateTime> StartDateTime
        {
            get
            {
                return _startDateTime;
            }
            set
            {
                _startDateTime = value;
                RaisePropertyChanged("StartDateTime");
            }
        }

        private Nullable<DateTime> _stopDateTime;
        [DataMemberAttribute()]
        public Nullable<DateTime> StopDateTime
        {
            get
            {
                return _stopDateTime;
            }
            set
            {
                _stopDateTime = value;
                RaisePropertyChanged("StopDateTime");
            }
        }

        [DataMemberAttribute()]
        public ObservableCollection<ReqOutwardDrugClinicDeptPatient> IntravenousDetails
        {
            get
            {
                return _intravenousDetails;
            }
            set
            {
                if (_intravenousDetails != value)
                {
                    _intravenousDetails = value;
                    RaisePropertyChanged("IntravenousDetails");
                }
            }
        }
        private ObservableCollection<ReqOutwardDrugClinicDeptPatient> _intravenousDetails;
    }
}
