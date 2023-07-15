using System;
using eHCMS.Services.Core.Base;
using System.Collections.Generic;

namespace DataEntities
{
    public class SurgeryScheduleDetail : NotifyChangedBase
    {
        private long _SurgeryScheduleDetailID;
        public long SurgeryScheduleDetailID
        {
            get { return _SurgeryScheduleDetailID; }
            set
            {
                _SurgeryScheduleDetailID = value;
                RaiseErrorsChanged("SurgeryScheduleDetailID");
            }
        }
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
        private long _ConsultingDiagnosysID;
        public long ConsultingDiagnosysID
        {
            get { return _ConsultingDiagnosysID; }
            set
            {
                _ConsultingDiagnosysID = value;
                RaiseErrorsChanged("ConsultingDiagnosysID");
            }
        }
        private DateTime? _SSD_Date;
        public DateTime? SSD_Date
        {
            get { return _SSD_Date; }
            set
            {
                _SSD_Date = value;
                RaiseErrorsChanged("SSD_Date");
            }
        }
        private long _SSD_Room;
        public long SSD_Room
        {
            get { return _SSD_Room; }
            set
            {
                _SSD_Room = value;
                RaiseErrorsChanged("SSD_Room");
            }
        }
        private string _PreOpDiagnosys;
        public string PreOpDiagnosys
        {
            get { return _PreOpDiagnosys; }
            set
            {
                _PreOpDiagnosys = value;
                RaiseErrorsChanged("PreOpDiagnosys");
            }
        }
        private string _OpIntervention;
        public string OpIntervention
        {
            get { return _OpIntervention; }
            set
            {
                _OpIntervention = value;
                RaiseErrorsChanged("OpIntervention");
            }
        }
        private Int16 _OpSeqNum;
        public Int16 OpSeqNum
        {
            get { return _OpSeqNum; }
            set
            {
                _OpSeqNum = value;
                RaiseErrorsChanged("OpSeqNum");
            }
        }
        private string _OpNote;
        public string OpNote
        {
            get { return _OpNote; }
            set
            {
                _OpNote = value;
                RaiseErrorsChanged("OpNote");
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
        private DateTime _DeletedDate;
        public DateTime DeletedDate
        {
            get { return _DeletedDate; }
            set
            {
                _DeletedDate = value;
                RaiseErrorsChanged("DeletedDate");
            }
        }
        private List<SurgeryScheduleDetail_TeamMember> _SurgeryScheduleDetail_TeamMember;
        public List<SurgeryScheduleDetail_TeamMember> SurgeryScheduleDetail_TeamMember
        {
            get { return _SurgeryScheduleDetail_TeamMember; }
            set
            {
                _SurgeryScheduleDetail_TeamMember = value;
                RaiseErrorsChanged("SurgeryScheduleDetail_TeamMember");
            }
        }
    }
}
