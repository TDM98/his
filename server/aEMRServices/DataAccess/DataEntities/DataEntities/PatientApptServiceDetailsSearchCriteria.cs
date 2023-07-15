using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
using Service.Core.Common;

namespace DataEntities
{
    public partial class PatientApptServiceDetailsSearchCriteria : SearchCriteriaBase
    {
        public PatientApptServiceDetailsSearchCriteria()
            : base()
        {

        }

        #region IEditableObject Members

        #endregion

        private long _AppointmentID;        
        public long AppointmentID
        {
            get
            {
                return _AppointmentID;
            }
            set
            {
                if (_AppointmentID != value)
                {
                    _AppointmentID = value;
                    RaisePropertyChanged("AppointmentID");
                }
            }
        }

        private long _ApptSvcDetailID;        
        public long ApptSvcDetailID
        {
            get
            {
                return _ApptSvcDetailID;
            }
            set
            {
                if (_ApptSvcDetailID != value)
                {
                    _ApptSvcDetailID = value;
                    RaisePropertyChanged("ApptSvcDetailID");
                }
            }
        }

        private long _MedServiceID;        
        public long MedServiceID
        {
            get
            {
                return _MedServiceID;
            }
            set
            {
                if (_MedServiceID != value)
                {
                    _MedServiceID = value;
                    RaisePropertyChanged("MedServiceID");
                }
            }
        }

        
        private long _DeptLocationID;        
        public long DeptLocationID
        {
            get
            {
                return _DeptLocationID;
            }
            set
            {
                if (_DeptLocationID != value)
                {
                    _DeptLocationID = value;
                    RaisePropertyChanged("DeptLocationID"); 
                }
            }
        }


        private short _ApptTimeSegmentID;        
        public short ApptTimeSegmentID
        {
            get
            {
                return _ApptTimeSegmentID;
            }
            set
            {
                if (_ApptTimeSegmentID != value)
                {
                    _ApptTimeSegmentID = value;
                    RaisePropertyChanged("ApptTimeSegmentID"); 
                }
            }
        }

        
        public Int16 ServiceSeqNum
        {
            get
            {
                return _ServiceSeqNum;
            }
            set
            {
                if (_ServiceSeqNum != value)
                {
                    _ServiceSeqNum = value;
                    RaisePropertyChanged("ServiceSeqNum"); 
                }
            }
        }
        private Int16 _ServiceSeqNum;

        public Byte ServiceSeqNumType
        {
            get
            {
                return _ServiceSeqNumType;
            }
            set
            {
                if (_ServiceSeqNumType != value)
                {
                    _ServiceSeqNumType = value;
                    RaisePropertyChanged("ServiceSeqNumType");
                }
            }
        }
        private Byte _ServiceSeqNumType;

        private long _V_AppointmentType;        
        public long V_AppointmentType
        {
            get
            {
                return _V_AppointmentType;
            }
            set
            {
                if (_V_AppointmentType != value)
                {
                    _V_AppointmentType = value;
                    RaisePropertyChanged("V_AppointmentType"); 
                }
            }
        }

        private long _StaffID;        
        public long StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                if (_StaffID != value)
                {
                    _StaffID = value;
                    RaisePropertyChanged("StaffID");
                }
            }
        }

        private DateTime _FromDate;
        public DateTime FromDate
        {
            get
            {
                return _FromDate;
            }
            set
            {
                if (_FromDate != value)
                {
                    _FromDate = value;
                    RaisePropertyChanged("FromDate");
                }
            }
        }

        private DateTime _ToDate;
        public DateTime ToDate
        {
            get
            {
                return _ToDate;
            }
            set
            {
                if (_ToDate != value)
                {
                    _ToDate = value;
                    RaisePropertyChanged("ToDate");
                }
            }
        }
       
    }
}
