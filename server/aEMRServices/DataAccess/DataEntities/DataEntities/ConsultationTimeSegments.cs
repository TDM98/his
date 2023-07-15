using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class ConsultationTimeSegments : NotifyChangedBase
    {
        public ConsultationTimeSegments()
            : base()
        {

        }

        #region Factory Method

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ConsultationTimeSegmentID"></param>
        /// <param name="SegmentName"></param>
        /// <returns></returns>
        public static ConsultationTimeSegments CreateConsultationTimeSegments(long ConsultationTimeSegmentID, String SegmentName)
        {
            ConsultationTimeSegments ConsultationTimeSegments = new ConsultationTimeSegments();
            ConsultationTimeSegments.ConsultationTimeSegmentID = ConsultationTimeSegmentID;
            ConsultationTimeSegments.SegmentName = SegmentName;
            return ConsultationTimeSegments;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long ConsultationTimeSegmentID
        {
            get
            {
                return _ConsultationTimeSegmentID;
            }
            set
            {
                if (_ConsultationTimeSegmentID != value)
                {
                    OnConsultationTimeSegmentIDChanging(value);
                    ////ReportPropertyChanging("ConsultationTimeSegmentID");
                    _ConsultationTimeSegmentID = value;
                    RaisePropertyChanged("ConsultationTimeSegmentID");
                    OnConsultationTimeSegmentIDChanged();
                }
            }
        }
        private long _ConsultationTimeSegmentID;
        partial void OnConsultationTimeSegmentIDChanging(long value);
        partial void OnConsultationTimeSegmentIDChanged();


        [DataMemberAttribute()]
        public string SegmentName
        {
            get
            {
                return _SegmentName;
            }
            set
            {
                if (_SegmentName != value)
                {
                    OnSegmentNameChanging(value);
                    ////ReportPropertyChanging("SegmentName");
                    _SegmentName = value;
                    RaisePropertyChanged("SegmentName");
                    OnSegmentNameChanged();
                }
            }
        }
        private string _SegmentName;
        partial void OnSegmentNameChanging(string value);
        partial void OnSegmentNameChanged();


        [DataMemberAttribute()]
        public string SegmentNameExt
        {
            get
            {
                return _SegmentNameExt;
            }
            set
            {
                if (_SegmentNameExt != value)
                {
                    OnSegmentNameExtChanging(value);
                    _SegmentNameExt = value;
                    RaisePropertyChanged("SegmentNameExt");
                    OnSegmentNameExtChanged();
                }
            }
        }
        private string _SegmentNameExt;
        partial void OnSegmentNameExtChanging(string value);
        partial void OnSegmentNameExtChanged();


        [DataMemberAttribute()]
        public string SegmentDescription
        {
            get
            {
                return _SegmentDescription;
            }
            set
            {
                if (_SegmentDescription != value)
                {
                    OnSegmentDescriptionChanging(value);
                    ////ReportPropertyChanging("SegmentDescription");
                    _SegmentDescription = value;
                    RaisePropertyChanged("SegmentDescription");
                    OnSegmentDescriptionChanged();
                }
            }
        }
        private string _SegmentDescription;
        partial void OnSegmentDescriptionChanging(string value);
        partial void OnSegmentDescriptionChanged();

        [DataMemberAttribute()]
        public DateTime StartTime
        {
            get
            {
                return _StartTime;
            }
            set
            {
                if (_StartTime != value)
                {
                    OnStartTimeChanging(value);
                    ////ReportPropertyChanging("StartTime");
                    _StartTime = value;
                    RaisePropertyChanged("StartTime");
                    stStartTime = StartTime.ToShortTimeString();
                    OnStartTimeChanged();
                }
            }
        }
        private DateTime _StartTime=DateTime.Now;
        partial void OnStartTimeChanging(DateTime value);
        partial void OnStartTimeChanged();

        [DataMemberAttribute()]
        public string stStartTime
        {
            get
            {
                return _stStartTime;
            }
            set
            {
                if (_stStartTime != value)
                {
                    OnstStartTimeChanging(value);
                    _stStartTime = value;
                    RaisePropertyChanged("stStartTime");
                    OnstStartTimeChanged();
                }
            }
        }
        private string _stStartTime;
        partial void OnstStartTimeChanging(string value);
        partial void OnstStartTimeChanged();

        [DataMemberAttribute()]
        public DateTime EndTime
        {
            get
            {
                return _EndTime;
            }
            set
            {
                if (_EndTime != value)
                {
                    OnEndTimeChanging(value);
                    ////ReportPropertyChanging("EndTime");
                    _EndTime = value;
                    stEndTime = EndTime.ToShortTimeString();
                    RaisePropertyChanged("EndTime");
                    OnEndTimeChanged();
                }
            }
        }
        private DateTime _EndTime = DateTime.Now;
        partial void OnEndTimeChanging(DateTime value);
        partial void OnEndTimeChanged();

        [DataMemberAttribute()]
        public string stEndTime
        {
            get
            {
                return _stEndTime;
            }
            set
            {
                if (_stEndTime != value)
                {
                    OnstEndTimeChanging(value);
                    ////ReportPropertyChanging("stEndTime");
                    _stEndTime = value;
                    RaisePropertyChanged("stEndTime");
                    OnstEndTimeChanged();
                }
            }
        }
        private string _stEndTime;
        partial void OnstEndTimeChanging(string value);
        partial void OnstEndTimeChanged();

        [DataMemberAttribute()]
        public bool IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                if (_IsActive != value)
                {
                    OnIsActiveChanging(value);
                    ////ReportPropertyChanging("IsActive");
                    _IsActive = value;
                    RaisePropertyChanged("IsActive");
                    OnIsActiveChanged();
                }
            }
        }
        private bool _IsActive;
        partial void OnIsActiveChanging(bool value);
        partial void OnIsActiveChanged();


        [DataMemberAttribute()]
        public Int16 CurrentSeqNumber
        {
            get
            {
                return _CurrentSeqNumber;
            }
            set
            {
                OnCurrentSeqNumberChanging(value);
                _CurrentSeqNumber = value;
                RaisePropertyChanged("CurrentSeqNumber");
                OnCurrentSeqNumberChanged();
            }
        }
        private Int16 _CurrentSeqNumber;
        partial void OnCurrentSeqNumberChanging(Int16 value);
        partial void OnCurrentSeqNumberChanged();


        [DataMemberAttribute()]
        public Int16 NumberOfSeq
        {
            get
            {
                return _NumberOfSeq;
            }
            set
            {
                OnNumberOfSeqChanging(value);
                _NumberOfSeq = value;
                RaisePropertyChanged("NumberOfSeq");
                OnNumberOfSeqChanged();
            }
        }
        private Int16 _NumberOfSeq;
        partial void OnNumberOfSeqChanging(Int16 value);
        partial void OnNumberOfSeqChanged();

        [DataMemberAttribute()]
        public Int16 ApptdayMaxNumConsultationAllowed
        {
            get
            {
                return _ApptdayMaxNumConsultationAllowed;
            }
            set
            {
                _ApptdayMaxNumConsultationAllowed = value;
                RaisePropertyChanged("ApptdayMaxNumConsultationAllowed");
            }
        }
        private Int16 _ApptdayMaxNumConsultationAllowed;

        
        [DataMemberAttribute()]
        public long V_TimeSegment
        {
            get
            {
                return _V_TimeSegment;
            }
            set
            {
                OnV_TimeSegmentChanging(value);
                _V_TimeSegment = value;
                RaisePropertyChanged("V_TimeSegment");
                OnV_TimeSegmentChanged();
            }
        }
        private long _V_TimeSegment;
        partial void OnV_TimeSegmentChanging(long value);
        partial void OnV_TimeSegmentChanged();

        private bool _IsChecked = false;
        public bool IsChecked
        {
            get
            {
                return _IsChecked;
            }
            set
            {
                if (_IsChecked == value)
                {
                    return;
                }
                _IsChecked = value;
                RaisePropertyChanged("IsChecked");
            }
        }
        private DateTime? _StartTime2 = DateTime.Now;
        [DataMemberAttribute()]
        public DateTime? StartTime2
        {
            get
            {
                return _StartTime2;
            }
            set
            {
                _StartTime2 = value;
                RaisePropertyChanged("StartTime2");
            }
        }
        private DateTime? _EndTime2 = DateTime.Now;
        [DataMemberAttribute()]
        public DateTime? EndTime2
        {
            get
            {
                return _EndTime2;
            }
            set
            {
                _EndTime2 = value;
                RaisePropertyChanged("EndTime2");
            }
        }
        #endregion
        #region Navigation Properties
        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PATIENTC_REL_PTINF_PATIENTC", "PatientClassHistory")]
        public ObservableCollection<ConsultationTimeSegments> consultationTimeSegments
        {
            get;
            set;
        }
        #endregion

        public override bool Equals(object obj)
        {
            ConsultationTimeSegments info = obj as ConsultationTimeSegments;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return ConsultationTimeSegmentID > 0 && this.ConsultationTimeSegmentID == info.ConsultationTimeSegmentID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
