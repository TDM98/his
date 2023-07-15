using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class OvertimeWorkingSchedule : EntityBase, IEditableObject
    {
        public static OvertimeWorkingSchedule CreateOvertimeWorkingSchedule(Int64 OvertimeWorkingScheduleID)
        {
            OvertimeWorkingSchedule ac = new OvertimeWorkingSchedule();
           
            return ac;
        }
        #region Primitive Properties

        [DataMemberAttribute()]
        public long OvertimeWorkingScheduleID
        {
            get
            {
                return _OvertimeWorkingScheduleID;
            }
            set
            {
                if (_OvertimeWorkingScheduleID == value)
                {
                    return;
                }
                _OvertimeWorkingScheduleID = value;
                RaisePropertyChanged("OvertimeWorkingScheduleID");
            }
        }
        private long _OvertimeWorkingScheduleID;
        [DataMemberAttribute()]
        public long OvertimeWorkingWeekID
        {
            get
            {
                return _OvertimeWorkingWeekID;
            }
            set
            {
                if (_OvertimeWorkingWeekID == value)
                {
                    return;
                }
                _OvertimeWorkingWeekID = value;
                RaisePropertyChanged("OvertimeWorkingWeekID");
            }
        }
        private long _OvertimeWorkingWeekID;

        [DataMemberAttribute()]
        public long DoctorStaffID
        {
            get
            {
                return _DoctorStaffID;
            }
            set
            {
                if (_DoctorStaffID == value)
                {
                    return;
                }
                _DoctorStaffID = value;
                RaisePropertyChanged("DoctorStaffID");
            }
        }
        private long _DoctorStaffID;
        [DataMemberAttribute()]
        public string StaffName
        {
            get
            {
                return _StaffName;
            }
            set
            {
                if (_StaffName == value)
                {
                    return;
                }
                _StaffName = value;
                RaisePropertyChanged("StaffName");
            }
        }
        private string _StaffName;

        [DataMemberAttribute()]
        public int Week
        {
            get
            {
                return _Week;
            }
            set
            {
                if (_Week == value)
                {
                    return;
                }
                _Week = value;
                RaisePropertyChanged("Week");
            }
        }
        private int _Week;

        [DataMemberAttribute()]
        public DateTime WorkDate
        {
            get
            {
                return _WorkDate;
            }
            set
            {
                if (_WorkDate == value)
                {
                    return;
                }
                _WorkDate = value;
                RaisePropertyChanged("WorkDate");
            }
        }
        private DateTime _WorkDate;

        [DataMemberAttribute()]
        public DateTime FromTime
        {
            get
            {
                return _FromTime;
            }
            set
            {
                if (_FromTime == value)
                {
                    return;
                }
                _FromTime = value;
                RaisePropertyChanged("FromTime");
            }
        }
        private DateTime _FromTime;

        [DataMemberAttribute()]
        public DateTime ToTime
        {
            get
            {
                return _ToTime;
            }
            set
            {
                if (_ToTime == value)
                {
                    return;
                }
                _ToTime = value;
                RaisePropertyChanged("ToTime");
            }
        }
        private DateTime _ToTime;

        [DataMemberAttribute()]
        public string WorkTime
        {
            get
            {
                return _WorkTime;
            }
            set
            {
                if (_WorkTime == value)
                {
                    return;
                }
                _WorkTime = value;
                RaisePropertyChanged("WorkTime");
            }
        }
        private string _WorkTime;

        [DataMemberAttribute()]
        public long DeptLocID
        {
            get
            {
                return _DeptLocID;
            }
            set
            {
                if (_DeptLocID == value)
                {
                    return;
                }
                _DeptLocID = value;
                RaisePropertyChanged("DeptLocID");
            }
        }
        private long _DeptLocID;

        [DataMemberAttribute()]
        public string LocationName
        {
            get
            {
                return _LocationName;
            }
            set
            {
                if (_LocationName == value)
                {
                    return;
                }
                _LocationName = value;
                RaisePropertyChanged("LocationName");
            }
        }
        private string _LocationName;

        [DataMemberAttribute]
        public long? CreatedStaffID
        {
            get
            {
                return _CreatedStaffID;
            }
            set
            {
                if (_CreatedStaffID == value)
                {
                    return;
                }
                _CreatedStaffID = value;
                RaisePropertyChanged("CreatedStaffID");
            }
        }
        private long? _CreatedStaffID;

        [DataMemberAttribute]
        public DateTime? CreatedDate
        {
            get
            {
                return _CreatedDate;
            }
            set
            {
                if (_CreatedDate == value)
                {
                    return;
                }
                _CreatedDate = value;
                RaisePropertyChanged("CreatedDate");
            }
        }
        private DateTime? _CreatedDate;

        [DataMemberAttribute]
        public long? LastUpdateStaffID
        {
            get
            {
                return _LastUpdateStaffID;
            }
            set
            {
                if (_LastUpdateStaffID == value)
                {
                    return;
                }
                _LastUpdateStaffID = value;
                RaisePropertyChanged("LastUpdateStaffID");
            }
        }
        private long? _LastUpdateStaffID;

        [DataMemberAttribute]
        public DateTime? LastUpdateDate
        {
            get
            {
                return _LastUpdateDate;
            }
            set
            {
                if (_LastUpdateDate == value)
                {
                    return;
                }
                _LastUpdateDate = value;
                RaisePropertyChanged("LastUpdateDate");
            }
        }
        private DateTime? _LastUpdateDate;


        [DataMemberAttribute]
        public bool Is_Deleted 
        {
            get
            {
                return _Is_Deleted;
            }
            set
            {
                if (_Is_Deleted == value)
                {
                    return;
                }
                _Is_Deleted = value;
                RaisePropertyChanged("Is_Deleted");
            }
        }
        private bool _Is_Deleted = false;

        public void BeginEdit()
        {
            throw new NotImplementedException();
        }

        public void EndEdit()
        {
            throw new NotImplementedException();
        }

        public void CancelEdit()
        {
            throw new NotImplementedException();
        }


        #endregion

    }
}
