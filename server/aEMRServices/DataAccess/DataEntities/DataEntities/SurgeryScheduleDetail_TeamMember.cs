using System;
using eHCMS.Services.Core.Base;
namespace DataEntities
{
    public class SurgeryScheduleDetail_TeamMember : NotifyChangedBase
    {
        private long _SSD_TeamMemberID;
        public long SSD_TeamMemberID
        {
            get { return _SSD_TeamMemberID; }
            set
            {
                _SSD_TeamMemberID = value;
                RaiseErrorsChanged("SSD_TeamMemberID");
            }
        }
        private long _StaffID;
        public long StaffID
        {
            get { return _StaffID; }
            set
            {
                _StaffID = value;
                RaiseErrorsChanged("StaffID");
            }
        }
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
        private long _V_SurgeryTeamRoleID;
        public long V_SurgeryTeamRoleID
        {
            get { return _V_SurgeryTeamRoleID; }
            set
            {
                _V_SurgeryTeamRoleID = value;
                RaiseErrorsChanged("V_SurgeryTeamRoleID");
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
    }
}
