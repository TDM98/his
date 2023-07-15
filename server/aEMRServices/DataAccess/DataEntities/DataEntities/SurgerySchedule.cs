using System;
using eHCMS.Services.Core.Base;
namespace DataEntities
{
    public class SurgerySchedule : NotifyChangedBase
    {
        private long _SurgeryScheduleID;
        public long SurgeryScheduleID
        {
            get { return _SurgeryScheduleID; }
            set
            {
                _SurgeryScheduleID = value;
                RaiseErrorsChanged("SurgeryScheduleID");
            }
        }
        private string _SSName;
        public string SSName
        {
            get { return _SSName; }
            set
            {
                _SSName = value;
                RaiseErrorsChanged("SSName");
            }
        }
        private DateTime _SSCreationDate;
        public DateTime SSCreationDate
        {
            get { return _SSCreationDate; }
            set
            {
                _SSCreationDate = value;
                RaiseErrorsChanged("SSCreationDate");
            }
        }
        private DateTime _SSFromDate;
        public DateTime SSFromDate
        {
            get { return _SSFromDate; }
            set
            {
                _SSFromDate = value;
                RaiseErrorsChanged("SSFromDate");
            }
        }
        private DateTime _SSToDate;
        public DateTime SSToDate
        {
            get { return _SSToDate; }
            set
            {
                _SSToDate = value;
                RaiseErrorsChanged("SSToDate");
            }
        }
        private long _SSCreatorStaffID;
        public long SSCreatorStaffID
        {
            get { return _SSCreatorStaffID; }
            set
            {
                _SSCreatorStaffID = value;
                RaiseErrorsChanged("SSCreatorStaffID");
            }
        }
        private DateTime _RecCreatedDate;
        public DateTime RecCreatedDate
        {
            get { return _RecCreatedDate; }
            set
            {
                _RecCreatedDate = value;
                RaiseErrorsChanged("RecCreatedDate");
            }
        }
        private string _SSNote;
        public string SSNote
        {
            get { return _SSNote; }
            set
            {
                _SSNote = value;
                RaiseErrorsChanged("SSNote");
            }
        }
    }
}
