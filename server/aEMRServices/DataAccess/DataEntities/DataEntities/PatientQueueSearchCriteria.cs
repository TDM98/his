using System;
using System.Net;
using System.Windows;
using eHCMS.Services.Core.Base;


namespace DataEntities
{
    public class PatientQueueSearchCriteria:SearchCriteriaBase
    {
        public PatientQueueSearchCriteria()
        { 
        }

        private Int64 _V_QueueType;
        public Int64 V_QueueType
        {
            get 
            { 
                return _V_QueueType; 
            }
            set 
            { 
                _V_QueueType = value;
                RaisePropertyChanged("V_QueueType");
            }
        }


        private Int64 _LocationID;
        public Int64 LocationID
        {
            get { return _LocationID; }
            set { 
                _LocationID = value;
                RaisePropertyChanged("LocationID");
            }
        }

        private Int64 _V_PatientQueueItemsStatus;
        public Int64 V_PatientQueueItemsStatus
        {
            get { return _V_PatientQueueItemsStatus; }
            set { 
                _V_PatientQueueItemsStatus = value;
                RaisePropertyChanged("V_PatientQueueItemsStatus");
            }
        }

        private Int64 _StaffID;
        public Int64 StaffID
        {
            get { return _StaffID; }
            set
            {
                _StaffID = value;
                RaisePropertyChanged("StaffID");
            }
        }

        private string _OrderBy;
        public string OrderBy
        {
            get { return _OrderBy; }
            set { _OrderBy = value;
            RaisePropertyChanged("OrderBy");
            }
        }

    }
}

