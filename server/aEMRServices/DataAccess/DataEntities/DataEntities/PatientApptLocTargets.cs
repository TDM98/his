using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class PatientApptLocTargets : NotifyChangedBase
    {
        public PatientApptLocTargets()
            : base()
        {

        }
        
        #region Primitive Properties

        [DataMemberAttribute()]
        public long PatientApptTargetID
        {
            get
            {
                return _PatientApptTargetID;
            }
            set
            {
                if (_PatientApptTargetID != value)
                {
                    OnPatientApptTargetIDChanging(value);
                    _PatientApptTargetID = value;
                    RaisePropertyChanged("PatientApptTargetID");
                    OnPatientApptTargetIDChanged();
                }
            }
        }
        private long _PatientApptTargetID;
        partial void OnPatientApptTargetIDChanging(long value);
        partial void OnPatientApptTargetIDChanged();


        //[DataMemberAttribute()]
        //public long DepartmentLocID
        //{
        //    get
        //    {
        //        return _DepartmentLocID;
        //    }
        //    set
        //    {
        //        if (_DepartmentLocID != value)
        //        {
        //            OnDepartmentLocIDChanging(value);
        //            _DepartmentLocID = value;
        //            RaisePropertyChanged("DepartmentLocID");
        //            OnDepartmentLocIDChanged();
        //        }
        //    }
        //}
        //private long _DepartmentLocID;
        //partial void OnDepartmentLocIDChanging(long value);
        //partial void OnDepartmentLocIDChanged();


        [DataMemberAttribute()]
        public DeptLocation ObjDepartmentLocID
        {
            get
            {
                return _ObjDepartmentLocID;
            }
            set
            {
                if (_ObjDepartmentLocID != value)
                {
                    OnObjDepartmentLocIDChanging(value);
                    _ObjDepartmentLocID = value;
                    RaisePropertyChanged("ObjDepartmentLocID");
                    OnObjDepartmentLocIDChanged();
                }
            }
        }
        private DeptLocation _ObjDepartmentLocID;
        partial void OnObjDepartmentLocIDChanging(DeptLocation value);
        partial void OnObjDepartmentLocIDChanged();


        //[DataMemberAttribute()]
        //public long ApptTimeSegmentID
        //{
        //    get
        //    {
        //        return _ApptTimeSegmentID;
        //    }
        //    set
        //    {
        //        if (_ApptTimeSegmentID != value)
        //        {
        //            OnApptTimeSegmentIDChanging(value);
        //            _ApptTimeSegmentID = value;
        //            RaisePropertyChanged("ApptTimeSegmentID");
        //            OnApptTimeSegmentIDChanged();
        //        }
        //    }
        //}
        //private long _ApptTimeSegmentID;
        //partial void OnApptTimeSegmentIDChanging(long value);
        //partial void OnApptTimeSegmentIDChanged();


        [DataMemberAttribute()]
        public ConsultationTimeSegments ObjApptTimeSegmentID
        {
            get
            {
                return _ObjApptTimeSegmentID;
            }
            set
            {
                if (_ObjApptTimeSegmentID != value)
                {
                    OnObjApptTimeSegmentIDChanging(value);
                    _ObjApptTimeSegmentID = value;
                    RaisePropertyChanged("ObjApptTimeSegmentID");
                    OnObjApptTimeSegmentIDChanged();
                }
            }
        }
        private ConsultationTimeSegments _ObjApptTimeSegmentID;
        partial void OnObjApptTimeSegmentIDChanging(ConsultationTimeSegments value);
        partial void OnObjApptTimeSegmentIDChanged();


        [DataMemberAttribute()]
        public Int16 NumberOfAppt
        {
            get
            {
                return _NumberOfAppt;
            }
            set
            {
                if (_NumberOfAppt != value)
                {
                    OnNumberOfApptChanging(value);
                    _NumberOfAppt = value;
                    RaisePropertyChanged("NumberOfAppt");
                    OnNumberOfApptChanged();
                }
            }
        }
        private Int16 _NumberOfAppt;
        partial void OnNumberOfApptChanging(Int16 value);
        partial void OnNumberOfApptChanged();


        [DataMemberAttribute()]
        public Int16 StartSequenceNumber
        {
            get
            {
                return _StartSequenceNumber;
            }
            set
            {
                if (_StartSequenceNumber != value)
                {
                    OnStartSequenceNumberChanging(value);
                    _StartSequenceNumber = value;
                    RaisePropertyChanged("StartSequenceNumber");
                    OnStartSequenceNumberChanged();
                }
            }
        }
        private Int16 _StartSequenceNumber;
        partial void OnStartSequenceNumberChanging(Int16 value);
        partial void OnStartSequenceNumberChanged();


        [DataMemberAttribute()]
        public Int16 EndSequenceNumber
        {
            get
            {
                return _EndSequenceNumber;
            }
            set
            {
                if (_EndSequenceNumber != value)
                {
                    OnEndSequenceNumberChanging(value);
                    _EndSequenceNumber = value;
                    RaisePropertyChanged("EndSequenceNumber");
                    OnEndSequenceNumberChanged();
                }
            }
        }
        private Int16 _EndSequenceNumber;
        partial void OnEndSequenceNumberChanging(Int16 value);
        partial void OnEndSequenceNumberChanged();

        #endregion

        public override bool Equals(object obj)
        {
            PatientApptLocTargets info = obj as PatientApptLocTargets;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PatientApptTargetID == info.PatientApptTargetID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
