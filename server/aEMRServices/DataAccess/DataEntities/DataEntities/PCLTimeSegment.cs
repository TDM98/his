using System;
using System.Runtime.Serialization;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public partial class PCLTimeSegment : NotifyChangedBase
    {
        public PCLTimeSegment()
            : base()
        {

        }

        #region Primitive Properties

        [DataMemberAttribute()]
        public long ParaclinicalTimeSegmentID
        {
            get
            {
                return _paraclinicalTimeSegmentID;
            }
            set
            {
                if (_paraclinicalTimeSegmentID != value)
                {
                    _paraclinicalTimeSegmentID = value;
                    RaisePropertyChanged("ParaclinicalTimeSegmentID");
                }
            }
        }
        private long _paraclinicalTimeSegmentID;


        [DataMemberAttribute()]
        public string SegmentName
        {
            get
            {
                return _segmentName;
            }
            set
            {
                if (_segmentName != value)
                {
                    _segmentName = value;
                    RaisePropertyChanged("SegmentName");
                }
            }
        }
        private string _segmentName;

        [DataMemberAttribute]
        public string SegmentDescription
        {
            get
            {
                return _segmentDescription;
            }
            set
            {
                if (_segmentDescription != value)
                {
                    _segmentDescription = value;
                    RaisePropertyChanged("SegmentDescription");
                }
            }
        }
        private string _segmentDescription;

        [DataMemberAttribute]
        public DateTime? StartTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                if (_startTime != value)
                {
                    _startTime = value;
                    RaisePropertyChanged("StartTime");
                }
            }
        }
        private DateTime? _startTime = DateTime.Now;

        [DataMemberAttribute]
        public DateTime? EndTime
        {
            get
            {
                return _endTime;
            }
            set
            {
                if (_endTime != value)
                {
                    _endTime = value;
                    RaisePropertyChanged("EndTime");
                }
            }
        }
        private DateTime? _endTime = DateTime.Now;

        private AllLookupValues.V_PCLMainCategory _vPCLMainCategory;
        [DataMemberAttribute]
        public  AllLookupValues.V_PCLMainCategory V_PCLMainCategory
        {
            get
            {
                return _vPCLMainCategory;
            }
            set
            {
                _vPCLMainCategory = value;
                RaisePropertyChanged("V_PCLMainCategory");
            }
        }
        #endregion

        public override bool Equals(object obj)
        {
            var info = obj as PCLTimeSegment;
            if (info == null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            return ParaclinicalTimeSegmentID > 0 && ParaclinicalTimeSegmentID == info.ParaclinicalTimeSegmentID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
