using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.Reflection;

namespace DataEntities
{
    public partial class ConsultationRoomTarget : NotifyChangedBase
    {

        #region Factory Method


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patientClassID"></param>
        /// <param name="patientClassName"></param>
        /// <returns></returns>
        public static ConsultationRoomTarget CreateConsultationRoomTarget(long ConsultationRoomTargetID)
        {
            ConsultationRoomTarget ConsultationRoomTarget = new ConsultationRoomTarget();
            ConsultationRoomTarget.ConsultationRoomTargetID = ConsultationRoomTargetID;
            return ConsultationRoomTarget;
        }

        #endregion
        #region Primitive Properties

        private long _ConsultationRoomTargetID;
        private long _ConsultationTimeSegmentID;
        private DateTime _RecCreatedDate;
        private long _DeptLocationID;
        //private Nullable<DateTime> _TargetDate;

        [DataMemberAttribute()]
        public long ConsultationRoomTargetID
        {
            get
            {
                return _ConsultationRoomTargetID;
            }
            set
            {
                OnConsultationRoomTargetIDChanging(value);
                if (_ConsultationRoomTargetID == value)
                    return;
                _ConsultationRoomTargetID = value;
                RaisePropertyChanged("ConsultationRoomTargetID");
                OnConsultationRoomTargetIDChanged();
            }
        }
        partial void OnConsultationRoomTargetIDChanging(long value);
        partial void OnConsultationRoomTargetIDChanged();

        [DataMemberAttribute()]
        public long ConsultationTimeSegmentID
        {
            get
            {
                return _ConsultationTimeSegmentID;
            }
            set
            {
                OnConsultationTimeSegmentIDChanging(value);
                if (_ConsultationTimeSegmentID == value)
                    return;
                _ConsultationTimeSegmentID = value;
                RaisePropertyChanged("ConsultationTimeSegmentID");
                OnConsultationTimeSegmentIDChanged();
            }
        }
        partial void OnConsultationTimeSegmentIDChanging(long value);
        partial void OnConsultationTimeSegmentIDChanged();

        [DataMemberAttribute()]
        public DateTime RecCreatedDate
        {
            get
            {
                return _RecCreatedDate;
            }
            set
            {
                OnRecCreatedDateChanging(value);
                if (_RecCreatedDate == value)
                    return;
                _RecCreatedDate = value;
                RaisePropertyChanged("RecCreatedDate");
                OnRecCreatedDateChanged();
            }
        }
        partial void OnRecCreatedDateChanging(DateTime value);
        partial void OnRecCreatedDateChanged();

        [DataMemberAttribute()]
        public long DeptLocationID
        {
            get
            {
                return _DeptLocationID;
            }
            set
            {
                OnDeptLocationIDChanging(value);
                if (_DeptLocationID == value)
                    return;
                _DeptLocationID = value;
                RaisePropertyChanged("DeptLocationID");
                OnDeptLocationIDChanged();
            }
        }
        partial void OnDeptLocationIDChanging(long value);
        partial void OnDeptLocationIDChanged();

        
        [DataMemberAttribute()] 
        public DateTime CurDate = DateTime.Now;

        [DataMemberAttribute()]
        public string Status
        {
            get
            {
                return _Status;
            }
            set
            {
                OnStatusChanging(value);
                if (_Status == value)
                    return;
                _Status = value;
                RaisePropertyChanged("Status");
                OnStatusChanged();
            }
        }
        private string _Status = "Chưa Đặt Chỉ Tiêu";
        partial void OnStatusChanging(string value);
        partial void OnStatusChanged();

        [DataMemberAttribute()]
        public bool isEdit
        {
            get
            {
                return _isEdit;
            }
            set
            {
                OnisEditChanging(value);
                if (_isEdit == value)
                    return;
                _isEdit = value;
                RaisePropertyChanged("isEdit");
                OnisEditChanged();
            }
        }
        private bool _isEdit ;
        partial void OnisEditChanging(bool value);
        partial void OnisEditChanged();

        
        [DataMemberAttribute()]
        public DateTime EffectiveDate
        {
            get
            {
                return _EffectiveDate;
            }
            set
            {
                OnEffectiveDateChanging(value);
                if (_EffectiveDate == value)
                    return;
                _EffectiveDate = value;
                RaisePropertyChanged("EffectiveDate");
                OnEffectiveDateChanged();
            }
        }
        private DateTime _EffectiveDate;
        partial void OnEffectiveDateChanging(DateTime value);
        partial void OnEffectiveDateChanged();
      
        [DataMemberAttribute()]
        public int MondayTargetNumberOfCases
        {
            get
            {
                return _MondayTargetNumberOfCases;
            }
            set
            {
                OnMondayTargetNumberOfCasesChanging(value);
                if (_MondayTargetNumberOfCases == value)
                    return;
                _MondayTargetNumberOfCases = value;
                RaisePropertyChanged("MondayTargetNumberOfCases");
                OnMondayTargetNumberOfCasesChanged();
            }
        }
        private int _MondayTargetNumberOfCases;
        partial void OnMondayTargetNumberOfCasesChanging(int value);
        partial void OnMondayTargetNumberOfCasesChanged();
      
        [DataMemberAttribute()]
        public int MondayMaxNumConsultationAllowed
        {
            get
            {
                return _MondayMaxNumConsultationAllowed;
            }
            set
            {
                OnMondayMaxNumConsultationAllowedChanging(value);
                if (_MondayMaxNumConsultationAllowed == value)
                    return;
                _MondayMaxNumConsultationAllowed = value;
                RaisePropertyChanged("MondayMaxNumConsultationAllowed");
                OnMondayMaxNumConsultationAllowedChanged();
            }
        }
        private int _MondayMaxNumConsultationAllowed;
        partial void OnMondayMaxNumConsultationAllowedChanging(int value);
        partial void OnMondayMaxNumConsultationAllowedChanged();
      
        [DataMemberAttribute()]
        public int TuesdayTargetNumberOfCases
        {
            get
            {
                return _TuesdayTargetNumberOfCases;
            }
            set
            {
                OnTuesdayTargetNumberOfCasesChanging(value);
                if (_TuesdayTargetNumberOfCases == value)
                    return;
                _TuesdayTargetNumberOfCases = value;
                RaisePropertyChanged("TuesdayTargetNumberOfCases");
                OnTuesdayTargetNumberOfCasesChanged();
            }
        }
        private int _TuesdayTargetNumberOfCases;
        partial void OnTuesdayTargetNumberOfCasesChanging(int value);
        partial void OnTuesdayTargetNumberOfCasesChanged();
      
        [DataMemberAttribute()]
        public int TuesdayMaxNumConsultationAllowed
        {
            get
            {
                return _TuesdayMaxNumConsultationAllowed;
            }
            set
            {
                OnTuesdayMaxNumConsultationAllowedChanging(value);
                if (_TuesdayMaxNumConsultationAllowed == value)
                    return;
                _TuesdayMaxNumConsultationAllowed = value;
                RaisePropertyChanged("TuesdayMaxNumConsultationAllowed");
                OnTuesdayMaxNumConsultationAllowedChanged();
            }
        }
        private int _TuesdayMaxNumConsultationAllowed;
        partial void OnTuesdayMaxNumConsultationAllowedChanging(int value);
        partial void OnTuesdayMaxNumConsultationAllowedChanged();
      
        [DataMemberAttribute()]
        public int WednesdayTargetNumberOfCases
        {
            get
            {
                return _WednesdayTargetNumberOfCases;
            }
            set
            {
                OnWednesdayTargetNumberOfCasesChanging(value);
                if (_WednesdayTargetNumberOfCases == value)
                    return;
                _WednesdayTargetNumberOfCases = value;
                RaisePropertyChanged("WednesdayTargetNumberOfCases");
                OnWednesdayTargetNumberOfCasesChanged();
            }
        }
        private int _WednesdayTargetNumberOfCases;
        partial void OnWednesdayTargetNumberOfCasesChanging(int value);
        partial void OnWednesdayTargetNumberOfCasesChanged();
      
        [DataMemberAttribute()]
        public int WednesdayMaxNumConsultationAllowed
        {
            get
            {
                return _WednesdayMaxNumConsultationAllowed;
            }
            set
            {
                OnWednesdayMaxNumConsultationAllowedChanging(value);
                if (_WednesdayMaxNumConsultationAllowed == value)
                    return;
                _WednesdayMaxNumConsultationAllowed = value;
                RaisePropertyChanged("WednesdayMaxNumConsultationAllowed");
                OnWednesdayMaxNumConsultationAllowedChanged();
            }
        }
        private int _WednesdayMaxNumConsultationAllowed;
        partial void OnWednesdayMaxNumConsultationAllowedChanging(int value);
        partial void OnWednesdayMaxNumConsultationAllowedChanged();
      
        [DataMemberAttribute()]
        public int ThursdayTargetNumberOfCases
        {
            get
            {
                return _ThursdayTargetNumberOfCases;
            }
            set
            {
                OnThursdayTargetNumberOfCasesChanging(value);
                if (_ThursdayTargetNumberOfCases == value)
                    return;
                _ThursdayTargetNumberOfCases = value;
                RaisePropertyChanged("ThursdayTargetNumberOfCases");
                OnThursdayTargetNumberOfCasesChanged();
            }
        }
        private int _ThursdayTargetNumberOfCases;
        partial void OnThursdayTargetNumberOfCasesChanging(int value);
        partial void OnThursdayTargetNumberOfCasesChanged();
      
        [DataMemberAttribute()]
        public int ThursdayMaxNumConsultationAllowed
        {
            get
            {
                return _ThursdayMaxNumConsultationAllowed;
            }
            set
            {
                OnThursdayMaxNumConsultationAllowedChanging(value);
                if (_ThursdayMaxNumConsultationAllowed == value)
                    return;
                _ThursdayMaxNumConsultationAllowed = value;
                RaisePropertyChanged("ThursdayMaxNumConsultationAllowed");
                OnThursdayMaxNumConsultationAllowedChanged();
            }
        }
        private int _ThursdayMaxNumConsultationAllowed;
        partial void OnThursdayMaxNumConsultationAllowedChanging(int value);
        partial void OnThursdayMaxNumConsultationAllowedChanged();
      
        [DataMemberAttribute()]
        public int FridayTargetNumberOfCases
        {
            get
            {
                return _FridayTargetNumberOfCases;
            }
            set
            {
                OnFridayTargetNumberOfCasesChanging(value);
                if (_FridayTargetNumberOfCases == value)
                    return;
                _FridayTargetNumberOfCases = value;
                RaisePropertyChanged("FridayTargetNumberOfCases");
                OnFridayTargetNumberOfCasesChanged();
            }
        }
        private int _FridayTargetNumberOfCases;
        partial void OnFridayTargetNumberOfCasesChanging(int value);
        partial void OnFridayTargetNumberOfCasesChanged();
      
        [DataMemberAttribute()]
        public int FridayMaxNumConsultationAllowed
        {
            get
            {
                return _FridayMaxNumConsultationAllowed;
            }
            set
            {
                OnFridayMaxNumConsultationAllowedChanging(value);
                if (_FridayMaxNumConsultationAllowed == value)
                    return;
                _FridayMaxNumConsultationAllowed = value;
                RaisePropertyChanged("FridayMaxNumConsultationAllowed");
                OnFridayMaxNumConsultationAllowedChanged();
            }
        }
        private int _FridayMaxNumConsultationAllowed;
        partial void OnFridayMaxNumConsultationAllowedChanging(int value);
        partial void OnFridayMaxNumConsultationAllowedChanged();
      
        [DataMemberAttribute()]
        public int SaturdayTargetNumberOfCases
        {
            get
            {
                return _SaturdayTargetNumberOfCases;
            }
            set
            {
                OnSaturdayTargetNumberOfCasesChanging(value);
                if (_SaturdayTargetNumberOfCases == value)
                    return;
                _SaturdayTargetNumberOfCases = value;
                RaisePropertyChanged("SaturdayTargetNumberOfCases");
                OnSaturdayTargetNumberOfCasesChanged();
            }
        }
        private int _SaturdayTargetNumberOfCases;
        partial void OnSaturdayTargetNumberOfCasesChanging(int value);
        partial void OnSaturdayTargetNumberOfCasesChanged();
      
        [DataMemberAttribute()]
        public int SaturdayMaxNumConsultationAllowed
        {
            get
            {
                return _SaturdayMaxNumConsultationAllowed;
            }
            set
            {
                OnSaturdayMaxNumConsultationAllowedChanging(value);
                if (_SaturdayMaxNumConsultationAllowed == value)
                    return;
                _SaturdayMaxNumConsultationAllowed = value;
                RaisePropertyChanged("SaturdayMaxNumConsultationAllowed");
                OnSaturdayMaxNumConsultationAllowedChanged();
            }
        }
        private int _SaturdayMaxNumConsultationAllowed;
        partial void OnSaturdayMaxNumConsultationAllowedChanging(int value);
        partial void OnSaturdayMaxNumConsultationAllowedChanged();
      
        [DataMemberAttribute()]
        public int SundayTargetNumberOfCases
        {
            get
            {
                return _SundayTargetNumberOfCases;
            }
            set
            {
                OnSundayTargetNumberOfCasesChanging(value);
                if (_SundayTargetNumberOfCases == value)
                    return;
                _SundayTargetNumberOfCases = value;
                RaisePropertyChanged("SundayTargetNumberOfCases");
                OnSundayTargetNumberOfCasesChanged();
            }
        }
        private int _SundayTargetNumberOfCases;
        partial void OnSundayTargetNumberOfCasesChanging(int value);
        partial void OnSundayTargetNumberOfCasesChanged();
      
        [DataMemberAttribute()]
        public int SundayMaxNumConsultationAllowed
        {
            get
            {
                return _SundayMaxNumConsultationAllowed;
            }
            set
            {
                OnSundayMaxNumConsultationAllowedChanging(value);
                if (_SundayMaxNumConsultationAllowed == value)
                    return;
                _SundayMaxNumConsultationAllowed = value;
                RaisePropertyChanged("SundayMaxNumConsultationAllowed");
                OnSundayMaxNumConsultationAllowedChanged();
            }
        }
        private int _SundayMaxNumConsultationAllowed;
        partial void OnSundayMaxNumConsultationAllowedChanging(int value);
        partial void OnSundayMaxNumConsultationAllowedChanged();

        [DataMemberAttribute()]
        public int DefaultTargetNumberOfCases
        {
            get
            {
                return _DefaultTargetNumberOfCases;
            }
            set
            {
                OnDefaultTargetNumberOfCasesChanging(value);
                if (_DefaultTargetNumberOfCases == value)
                    return;
                _DefaultTargetNumberOfCases = value;                
                RaisePropertyChanged("DefaultTargetNumberOfCases");
                OnDefaultTargetNumberOfCasesChanged();
            }
        }
        private int _DefaultTargetNumberOfCases;
        partial void OnDefaultTargetNumberOfCasesChanging(int value);
        partial void OnDefaultTargetNumberOfCasesChanged();

        [DataMemberAttribute()]
        public int DefaultMaxNumConsultationAllowed
        {
            get
            {
                return _DefaultMaxNumConsultationAllowed;
            }
            set
            {
                OnDefaultMaxNumConsultationAllowedChanging(value);
                if (_DefaultMaxNumConsultationAllowed == value)
                    return;
                _DefaultMaxNumConsultationAllowed = value;
                
                RaisePropertyChanged("DefaultMaxNumConsultationAllowed");
                OnDefaultMaxNumConsultationAllowedChanged();
            }
        }
        private int _DefaultMaxNumConsultationAllowed;
        partial void OnDefaultMaxNumConsultationAllowedChanging(int value);
        partial void OnDefaultMaxNumConsultationAllowedChanged();

        //Số bắt đầu và kết thúc hẹn
        [DataMemberAttribute()]
        public int DefaultStartSequenceNumber
        {
            get
            {
                return _DefaultStartSequenceNumber;
            }
            set
            {
                OnDefaultStartSequenceNumberChanging(value);
                if (_DefaultStartSequenceNumber == value)
                    return;
                _DefaultStartSequenceNumber = value;
                RaisePropertyChanged("DefaultStartSequenceNumber");
                OnDefaultStartSequenceNumberChanged();
            }
        }
        private int _DefaultStartSequenceNumber;
        partial void OnDefaultStartSequenceNumberChanging(int value);
        partial void OnDefaultStartSequenceNumberChanged();
        [DataMemberAttribute()]
        public int DefaultEndSequenceNumber
        {
            get
            {
                return _DefaultEndSequenceNumber;
            }
            set
            {
                OnDefaultEndSequenceNumberChanging(value);
                if (_DefaultEndSequenceNumber == value)
                    return;
                _DefaultEndSequenceNumber = value;
                RaisePropertyChanged("DefaultEndSequenceNumber");
                OnDefaultEndSequenceNumberChanged();
            }
        }
        private int _DefaultEndSequenceNumber;
        partial void OnDefaultEndSequenceNumberChanging(int value);
        partial void OnDefaultEndSequenceNumberChanged();


        [DataMemberAttribute()]
        public int MondayStartSequenceNumber
        {
            get
            {
                return _MondayStartSequenceNumber;
            }
            set
            {
                OnMondayStartSequenceNumberChanging(value);
                if (_MondayStartSequenceNumber == value)
                    return;
                _MondayStartSequenceNumber = value;
                RaisePropertyChanged("MondayStartSequenceNumber");
                OnMondayStartSequenceNumberChanged();
            }
        }
        private int _MondayStartSequenceNumber;
        partial void OnMondayStartSequenceNumberChanging(int value);
        partial void OnMondayStartSequenceNumberChanged();
        [DataMemberAttribute()]
        public int MondayEndSequenceNumber
        {
            get
            {
                return _MondayEndSequenceNumber;
            }
            set
            {
                OnMondayEndSequenceNumberChanging(value);
                if (_MondayEndSequenceNumber == value)
                    return;
                _MondayEndSequenceNumber = value;
                RaisePropertyChanged("MondayEndSequenceNumber");
                OnMondayEndSequenceNumberChanged();
            }
        }
        private int _MondayEndSequenceNumber;
        partial void OnMondayEndSequenceNumberChanging(int value);
        partial void OnMondayEndSequenceNumberChanged();


        [DataMemberAttribute()]
        public int TuesdayStartSequenceNumber
        {
            get
            {
                return _TuesdayStartSequenceNumber;
            }
            set
            {
                OnTuesdayStartSequenceNumberChanging(value);
                if (_TuesdayStartSequenceNumber == value)
                    return;
                _TuesdayStartSequenceNumber = value;
                RaisePropertyChanged("TuesdayStartSequenceNumber");
                OnTuesdayStartSequenceNumberChanged();
            }
        }
        private int _TuesdayStartSequenceNumber;
        partial void OnTuesdayStartSequenceNumberChanging(int value);
        partial void OnTuesdayStartSequenceNumberChanged();
        [DataMemberAttribute()]
        public int TuesdayEndSequenceNumber
        {
            get
            {
                return _TuesdayEndSequenceNumber;
            }
            set
            {
                OnTuesdayEndSequenceNumberChanging(value);
                if (_TuesdayEndSequenceNumber == value)
                    return;
                _TuesdayEndSequenceNumber = value;
                RaisePropertyChanged("TuesdayEndSequenceNumber");
                OnTuesdayEndSequenceNumberChanged();
            }
        }
        private int _TuesdayEndSequenceNumber;
        partial void OnTuesdayEndSequenceNumberChanging(int value);
        partial void OnTuesdayEndSequenceNumberChanged();


        [DataMemberAttribute()]
        public int WednesdayStartSequenceNumber
        {
            get
            {
                return _WednesdayStartSequenceNumber;
            }
            set
            {
                OnWednesdayStartSequenceNumberChanging(value);
                if (_WednesdayStartSequenceNumber == value)
                    return;
                _WednesdayStartSequenceNumber = value;
                RaisePropertyChanged("WednesdayStartSequenceNumber");
                OnWednesdayStartSequenceNumberChanged();
            }
        }
        private int _WednesdayStartSequenceNumber;
        partial void OnWednesdayStartSequenceNumberChanging(int value);
        partial void OnWednesdayStartSequenceNumberChanged();
        [DataMemberAttribute()]
        public int WednesdayEndSequenceNumber
        {
            get
            {
                return _WednesdayEndSequenceNumber;
            }
            set
            {
                OnWednesdayEndSequenceNumberChanging(value);
                if (_WednesdayEndSequenceNumber == value)
                    return;
                _WednesdayEndSequenceNumber = value;
                RaisePropertyChanged("WednesdayEndSequenceNumber");
                OnWednesdayEndSequenceNumberChanged();
            }
        }
        private int _WednesdayEndSequenceNumber;
        partial void OnWednesdayEndSequenceNumberChanging(int value);
        partial void OnWednesdayEndSequenceNumberChanged();


        [DataMemberAttribute()]
        public int ThursdayStartSequenceNumber
        {
            get
            {
                return _ThursdayStartSequenceNumber;
            }
            set
            {
                OnThursdayStartSequenceNumberChanging(value);
                if (_ThursdayStartSequenceNumber == value)
                    return;
                _ThursdayStartSequenceNumber = value;
                RaisePropertyChanged("ThursdayStartSequenceNumber");
                OnThursdayStartSequenceNumberChanged();
            }
        }
        private int _ThursdayStartSequenceNumber;
        partial void OnThursdayStartSequenceNumberChanging(int value);
        partial void OnThursdayStartSequenceNumberChanged();
        [DataMemberAttribute()]
        public int ThursdayEndSequenceNumber
        {
            get
            {
                return _ThursdayEndSequenceNumber;
            }
            set
            {
                OnThursdayEndSequenceNumberChanging(value);
                if (_ThursdayEndSequenceNumber == value)
                    return;
                _ThursdayEndSequenceNumber = value;
                RaisePropertyChanged("ThursdayEndSequenceNumber");
                OnThursdayEndSequenceNumberChanged();
            }
        }
        private int _ThursdayEndSequenceNumber;
        partial void OnThursdayEndSequenceNumberChanging(int value);
        partial void OnThursdayEndSequenceNumberChanged();


        [DataMemberAttribute()]
        public int FridayStartSequenceNumber
        {
            get
            {
                return _FridayStartSequenceNumber;
            }
            set
            {
                OnFridayStartSequenceNumberChanging(value);
                if (_FridayStartSequenceNumber == value)
                    return;
                _FridayStartSequenceNumber = value;
                RaisePropertyChanged("FridayStartSequenceNumber");
                OnFridayStartSequenceNumberChanged();
            }
        }
        private int _FridayStartSequenceNumber;
        partial void OnFridayStartSequenceNumberChanging(int value);
        partial void OnFridayStartSequenceNumberChanged();
        [DataMemberAttribute()]
        public int FridayEndSequenceNumber
        {
            get
            {
                return _FridayEndSequenceNumber;
            }
            set
            {
                OnFridayEndSequenceNumberChanging(value);
                if (_FridayEndSequenceNumber == value)
                    return;
                _FridayEndSequenceNumber = value;
                RaisePropertyChanged("FridayEndSequenceNumber");
                OnFridayEndSequenceNumberChanged();
            }
        }
        private int _FridayEndSequenceNumber;
        partial void OnFridayEndSequenceNumberChanging(int value);
        partial void OnFridayEndSequenceNumberChanged();


        [DataMemberAttribute()]
        public int SaturdayStartSequenceNumber
        {
            get
            {
                return _SaturdayStartSequenceNumber;
            }
            set
            {
                OnSaturdayStartSequenceNumberChanging(value);
                if (_SaturdayStartSequenceNumber == value)
                    return;
                _SaturdayStartSequenceNumber = value;
                RaisePropertyChanged("SaturdayStartSequenceNumber");
                OnSaturdayStartSequenceNumberChanged();
            }
        }
        private int _SaturdayStartSequenceNumber;
        partial void OnSaturdayStartSequenceNumberChanging(int value);
        partial void OnSaturdayStartSequenceNumberChanged();
        [DataMemberAttribute()]
        public int SaturdayEndSequenceNumber
        {
            get
            {
                return _SaturdayEndSequenceNumber;
            }
            set
            {
                OnSaturdayEndSequenceNumberChanging(value);
                if (_SaturdayEndSequenceNumber == value)
                    return;
                _SaturdayEndSequenceNumber = value;
                RaisePropertyChanged("SaturdayEndSequenceNumber");
                OnSaturdayEndSequenceNumberChanged();
            }
        }
        private int _SaturdayEndSequenceNumber;
        partial void OnSaturdayEndSequenceNumberChanging(int value);
        partial void OnSaturdayEndSequenceNumberChanged();


        [DataMemberAttribute()]
        public int SundayStartSequenceNumber
        {
            get
            {
                return _SundayStartSequenceNumber;
            }
            set
            {
                OnSundayStartSequenceNumberChanging(value);
                if (_SundayStartSequenceNumber == value)
                    return;
                _SundayStartSequenceNumber = value;
                RaisePropertyChanged("SundayStartSequenceNumber");
                OnSundayStartSequenceNumberChanged();
            }
        }
        private int _SundayStartSequenceNumber;
        partial void OnSundayStartSequenceNumberChanging(int value);
        partial void OnSundayStartSequenceNumberChanged();
        [DataMemberAttribute()]
        public int SundayEndSequenceNumber
        {
            get
            {
                return _SundayEndSequenceNumber;
            }
            set
            {
                OnSundayEndSequenceNumberChanging(value);
                if (_SundayEndSequenceNumber == value)
                    return;
                _SundayEndSequenceNumber = value;
                RaisePropertyChanged("SundayEndSequenceNumber");
                OnSundayEndSequenceNumberChanged();
            }
        }
        private int _SundayEndSequenceNumber;
        partial void OnSundayEndSequenceNumberChanging(int value);
        partial void OnSundayEndSequenceNumberChanged();
        //Số bắt đầu và kết thúc hẹn

        [DataMemberAttribute()]
        public bool IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                OnIsDeletedChanging(value);
                if (_IsDeleted == value)
                    return;
                _IsDeleted = value;
                RaisePropertyChanged("IsDeleted");
                OnIsDeletedChanged();
            }
        }
        private bool _IsDeleted;
        partial void OnIsDeletedChanging(bool value);
        partial void OnIsDeletedChanged();


        #endregion

        #region Navigation Properties
                
        [DataMemberAttribute()]
        public ConsultationTimeSegments ConsultationTimeSegments
        {
            get
            {
                return _ConsultationTimeSegments;
            }
            set
            {
                if (_ConsultationTimeSegments != value)
                {
                    OnConsultationTimeSegmentsChanging(value);
                    _ConsultationTimeSegments = value;
                    ConsultationTimeSegmentID = ConsultationTimeSegments.ConsultationTimeSegmentID;
                    RaisePropertyChanged("ConsultationTimeSegments");
                    OnConsultationTimeSegmentsChanged();
                    
                }
            }
        }
        private ConsultationTimeSegments _ConsultationTimeSegments;
        partial void OnConsultationTimeSegmentsChanging(ConsultationTimeSegments value);
        partial void OnConsultationTimeSegmentsChanged();

        [DataMemberAttribute()]
        public DeptLocation DeptLocation
        {
            get
            {
                return _DeptLocation;
            }
            set
            {
                if (_DeptLocation != value)
                {
                    OnDeptLocationChanging(value);
                    _DeptLocation = value;
                    RaisePropertyChanged("DeptLocation");
                    OnDeptLocationChanged();
                }
            }
        }
        private DeptLocation _DeptLocation;
        partial void OnDeptLocationChanging(DeptLocation value);
        partial void OnDeptLocationChanged();
        #endregion

        public override bool Equals(object obj)
        {
            ConsultationRoomTarget info = obj as ConsultationRoomTarget;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.ConsultationRoomTargetID == info.ConsultationRoomTargetID
                && this.ConsultationTimeSegmentID == info.ConsultationTimeSegmentID;
        }

        public bool propertiesEqual(object comparisonObject) 
        {
            Type sourceType = this.GetType();
            Type destinationType = comparisonObject.GetType();
            if(sourceType==destinationType)
            {
                PropertyInfo[] sourceProperties = sourceType.GetProperties();
                foreach (var item in sourceProperties)
                {
                    if (item.ToString() == "Int32 MyID"
                        ||item.ToString()=="Boolean HasErrors")
                    {
                        continue;
                    }
                    if (sourceType.GetProperty(item.Name).GetValue(this,null)!=null
                        && destinationType.GetProperty(item.Name).GetValue(comparisonObject, null) != null)
                    {
                        if (sourceType.GetProperty(item.Name).GetValue(this, null).ToString() !=
                             destinationType.GetProperty(item.Name).GetValue(comparisonObject, null).ToString())
                        {
                            return false;
                        }
                    }
                } 
            }
            return true;
        }

        public void GetAllMaxNumDefaulValue() 
        {
            MondayMaxNumConsultationAllowed = TuesdayMaxNumConsultationAllowed = WednesdayMaxNumConsultationAllowed = ThursdayMaxNumConsultationAllowed
                = FridayMaxNumConsultationAllowed = SaturdayMaxNumConsultationAllowed = SundayMaxNumConsultationAllowed = DefaultMaxNumConsultationAllowed;
        }
        public void GetAllTargetNumDefaulValue()
        {
            MondayTargetNumberOfCases = TuesdayTargetNumberOfCases = WednesdayTargetNumberOfCases = ThursdayTargetNumberOfCases
                = FridayTargetNumberOfCases = SaturdayTargetNumberOfCases = SundayTargetNumberOfCases = DefaultTargetNumberOfCases;
        }

        public void GetDefaultStartSequenceNumberValue()
        {
            MondayStartSequenceNumber = TuesdayStartSequenceNumber = WednesdayStartSequenceNumber = ThursdayStartSequenceNumber
                = FridayStartSequenceNumber = SaturdayStartSequenceNumber = SundayStartSequenceNumber = DefaultStartSequenceNumber;
        }

        public void GetDefaultEndSequenceNumberValue()
        {
            MondayEndSequenceNumber = TuesdayEndSequenceNumber = WednesdayEndSequenceNumber = ThursdayEndSequenceNumber
                = FridayEndSequenceNumber = SaturdayEndSequenceNumber = SundayEndSequenceNumber = DefaultEndSequenceNumber;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        //public enum DaysDefine
        //{
        //    Mon = 0x001,
        //    Tues = 0x002,
        //    Wed = 0x004,
        //    Thur = 0x008,
        //    Fri = 0x010,
        //    Sat = 0x020,
        //    Sun = 0x040,
        //}
        //public int GetNumOfDay()
        //{
        //    int temp = 0;
        //    if (Monday)
        //        temp = temp | (int)DaysDefine.Mon;
        //    if (Tuesday)
        //        temp = temp | (int)DaysDefine.Tues;
        //    if (Wednesday)
        //        temp = temp | (int)DaysDefine.Wed;
        //    if (Thursday)
        //        temp = temp | (int)DaysDefine.Thur;
        //    if (Friday)
        //        temp = temp | (int)DaysDefine.Fri;
        //    if (Saturday)
        //        temp = temp | (int)DaysDefine.Sat;
        //    if (Sunday)
        //        temp = temp | (int)DaysDefine.Sun;
        //    return temp;
        //}
        //public void CheckDay(int temp)
        //{
        //    Monday = (temp & (int)DaysDefine.Mon) == (int)DaysDefine.Mon ? true : false;
        //    Tuesday = (temp & (int)DaysDefine.Tues) == (int)DaysDefine.Tues ? true : false;
        //    Wednesday = (temp & (int)DaysDefine.Wed) == (int)DaysDefine.Wed ? true : false;
        //    Thursday = (temp & (int)DaysDefine.Thur) == (int)DaysDefine.Thur ? true : false;
        //    Friday = (temp & (int)DaysDefine.Fri) == (int)DaysDefine.Fri ? true : false;
        //    Saturday = (temp & (int)DaysDefine.Sat) == (int)DaysDefine.Sat ? true : false;
        //    Sunday = (temp & (int)DaysDefine.Sun) == (int)DaysDefine.Sun ? true : false;
        //}
    }
}
