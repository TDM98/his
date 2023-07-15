using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.Collections.Generic;
using System.Text;

namespace DataEntities
{
    [DataContract]
    public partial class PatientApptTimeSegment : EntityBase, IEditableObject
    {
        public PatientApptTimeSegment()
            : base()
        {

        }

        private PatientApptTimeSegment _tempAppointment;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempAppointment = (PatientApptTimeSegment)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempAppointment)
                CopyFrom(_tempAppointment);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(PatientApptTimeSegment p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        private short _ApptTimeSegmentID;
        [DataMemberAttribute()]
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

        private string _SegmentName;
        [DataMemberAttribute()]
        public string SegmentName
        {
            get
            {
                return _SegmentName;
            }
            set
            {

                _SegmentName = value;
                RaisePropertyChanged("SegmentName");
            }
        }

        private DateTime _StartTime;
        [DataMemberAttribute()]
        public DateTime StartTime
        {
            get
            {
                return _StartTime;
            }
            set
            {
                _StartTime = value;
                RaisePropertyChanged("StartTime");
            }
        }

        private DateTime _EndTime;
        [DataMemberAttribute()]
        public DateTime EndTime
        {
            get
            {
                return _EndTime;
            }
            set
            {
                _EndTime = value;
                RaisePropertyChanged("EndTime");
            }
        }

        private long _V_DayOfWeek;
        [DataMemberAttribute()]
        public long V_DayOfWeek
        {
            get
            {
                return _V_DayOfWeek;
            }
            set
            {
                _V_DayOfWeek = value;
                RaisePropertyChanged("V_DayOfWeek");
            }
        }


        public override bool Equals(object obj)
        {
            PatientApptTimeSegment info = obj as PatientApptTimeSegment;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.ApptTimeSegmentID == info.ApptTimeSegmentID;
        }

        public override int GetHashCode()
        {
            return this.ApptTimeSegmentID.GetHashCode();
        }

    }
}
