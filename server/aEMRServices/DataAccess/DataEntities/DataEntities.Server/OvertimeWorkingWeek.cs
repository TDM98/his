using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class OvertimeWorkingWeek : EntityBase, IEditableObject
    {
        public static OvertimeWorkingWeek CreateOvertimeWorkingWeek(Int64 OvertimeWorkingWeekID)
        {
            OvertimeWorkingWeek ac = new OvertimeWorkingWeek();
           
            return ac;
        }
        #region Primitive Properties

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
        public DateTime FromDate
        {
            get
            {
                return _FromDate;
            }
            set
            {
                if (_FromDate == value)
                {
                    return;
                }
                _FromDate = value;
                RaisePropertyChanged("FromDate");
            }
        }
        private DateTime _FromDate;

        [DataMemberAttribute()]
        public DateTime ToDate
        {
            get
            {
                return _ToDate;
            }
            set
            {
                if (_ToDate == value)
                {
                    return;
                }
                _ToDate = value;
                RaisePropertyChanged("ToDate");
            }
        }
        private DateTime _ToDate;

        [DataMemberAttribute()]
        public string OvertimeWorkingNotes
        {
            get
            {
                return _OvertimeWorkingNotes;
            }
            set
            {
                if (_OvertimeWorkingNotes == value)
                {
                    return;
                }
                _OvertimeWorkingNotes = value;
                RaisePropertyChanged("OvertimeWorkingNotes");
            }
        }
        private string _OvertimeWorkingNotes;

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

        [DataMemberAttribute]
        public long V_OvertimeWorkingWeekStatus
        {
            get
            {
                return _V_OvertimeWorkingWeekStatus;
            }
            set
            {
                if (_V_OvertimeWorkingWeekStatus == value)
                {
                    return;
                }
                _V_OvertimeWorkingWeekStatus = value;
                RaisePropertyChanged("V_OvertimeWorkingWeekStatus");
            }
        }
        private long _V_OvertimeWorkingWeekStatus;

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
