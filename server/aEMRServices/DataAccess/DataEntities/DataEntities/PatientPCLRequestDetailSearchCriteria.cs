using System;
using System.Net;
using System.Windows;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public class PatientPCLRequestDetailSearchCriteria : SearchCriteriaBase
    {
        public PatientPCLRequestDetailSearchCriteria()
        {
        }

        private Int64 _PatientPCLReqID;
        public Int64 PatientPCLReqID
        {
            get 
            {
                return _PatientPCLReqID; 
            }
            set 
            {
                _PatientPCLReqID = value;
                RaisePropertyChanged("PatientPCLReqID");
            }
        }

        private Int64 _V_ExamRegStatus;
        public Int64 V_ExamRegStatus
        {
            get
            {
                return _V_ExamRegStatus;
            }
            set
            {
                _V_ExamRegStatus = value;
                RaisePropertyChanged("V_ExamRegStatus");
            }
        }

    }
}