using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities
{
    /*
     * 20220530 #001 BLQ: Kiểm tra thời gian thao tác của bác sĩ CreateNew
     */
    public partial class DoctorContactPatientTime : NotifyChangedBase
    {
        private long _DoctorContactPatientTimeID;
        [DataMemberAttribute()]
        public long DoctorContactPatientTimeID
        {
            get
            {
                return _DoctorContactPatientTimeID;
            }
            set
            {
                _DoctorContactPatientTimeID = value;
                RaisePropertyChanged("DoctorContactPatientTimeID");
            }
        }
        private long _PtRegistrationID;
        [DataMemberAttribute()]
        public long PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                _PtRegistrationID = value;
                RaisePropertyChanged("PtRegistrationID");
            }
        }
        private long _PatientID;
        [DataMemberAttribute()]
        public long PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                _PatientID = value;
                RaisePropertyChanged("PatientID");
            }
        }
        private long _PtRegDetailID;
        [DataMemberAttribute()]
        public long PtRegDetailID
        {
            get
            {
                return _PtRegDetailID;
            }
            set
            {
                _PtRegDetailID = value;
                RaisePropertyChanged("PtRegDetailID");
            }
        }
        private DateTime _StartDatetime;
        [DataMemberAttribute()]
        public DateTime StartDatetime
        {
            get
            {
                return _StartDatetime;
            }
            set
            {
                _StartDatetime = value;
                RaisePropertyChanged("StartDatetime");
            }
        }
        private DateTime? _EndDatetime = null;
        [DataMemberAttribute()]
        public DateTime? EndDatetime
        {
            get
            {
                return _EndDatetime;
            }
            set
            {
                _EndDatetime = value;
                RaisePropertyChanged("EndDatetime");
            }
        }
        private long _DoctorStaffID;
        [DataMemberAttribute()]
        public long DoctorStaffID
        {
            get
            {
                return _DoctorStaffID;
            }
            set
            {
                _DoctorStaffID = value;
                RaisePropertyChanged("DoctorStaffID");
            }
        }
        private string _Log;
        [DataMemberAttribute()]
        public string Log
        {
            get
            {
                return _Log;
            }
            set
            {
                _Log = value;
                RaisePropertyChanged("Log");
            }
        }
       
    }
}
