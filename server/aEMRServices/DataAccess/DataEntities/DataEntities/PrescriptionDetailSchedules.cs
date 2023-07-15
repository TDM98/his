using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;


namespace DataEntities
{
    public partial class PrescriptionDetailSchedules : NotifyChangedBase
    {
        [DataMember()]
        public Int64 PrescriptDetailScheduleID
        {
            get
            {
                return _PrescriptDetailScheduleID;
            }
            set
            {
                if (_PrescriptDetailScheduleID != value)
                {
                    OnPrescriptDetailScheduleIDChanging(value);
                    _PrescriptDetailScheduleID = value;
                    RaisePropertyChanged("PrescriptDetailScheduleID");
                    OnPrescriptDetailScheduleIDChanged();
                }
            }
        }
        private Int64 _PrescriptDetailScheduleID;
        partial void OnPrescriptDetailScheduleIDChanging(Int64 value);
        partial void OnPrescriptDetailScheduleIDChanged();


        [DataMember()]
        public Int64 PrescriptDetailID
        {
            get
            {
                return _PrescriptDetailID;
            }
            set
            {
                if (_PrescriptDetailID != value)
                {
                    OnPrescriptDetailIDChanging(value);
                    _PrescriptDetailID = value;
                    RaisePropertyChanged("PrescriptDetailID");
                    OnPrescriptDetailIDChanged();
                }
            }
        }
        private Int64 _PrescriptDetailID;
        partial void OnPrescriptDetailIDChanging(Int64 value);
        partial void OnPrescriptDetailIDChanged();


        [DataMember()]
        public Nullable<Int64> V_PeriodOfDay
        {
            get
            {
                return _V_PeriodOfDay;
            }
            set
            {
                if (_V_PeriodOfDay != value)
                {
                    OnV_PeriodOfDayChanging(value);
                    _V_PeriodOfDay = value;
                    RaisePropertyChanged("V_PeriodOfDay");
                    OnV_PeriodOfDayChanged();
                }
            }
        }
        private Nullable<Int64>  _V_PeriodOfDay;
        partial void OnV_PeriodOfDayChanging(Nullable<Int64> value);
        partial void OnV_PeriodOfDayChanged();


        [DataMember()]
        public Lookup ObjV_PeriodOfDay
        {
            get
            {
                return _ObjV_PeriodOfDay;
            }
            set
            {
                if (_ObjV_PeriodOfDay != value)
                {
                    OnObjV_PeriodOfDayChanging(value);
                    _ObjV_PeriodOfDay = value;
                    RaisePropertyChanged("ObjV_PeriodOfDay");
                    OnObjV_PeriodOfDayChanged();
                }
            }
        }
        private Lookup _ObjV_PeriodOfDay;
        partial void OnObjV_PeriodOfDayChanging(Lookup value);
        partial void OnObjV_PeriodOfDayChanged();


        [Range(0.0, 99999999999.0, ErrorMessage = "Liều dùng Thứ 2 không được nhỏ hơn 0")]
        [DataMemberAttribute()]
        public Nullable<Single> Monday
        {
            get
            {
                return _Monday;
            }
            set
            {
                OnMondayChanging(value);
                _Monday = value;
                RaisePropertyChanged("Monday");
                OnMondayChanged();
                
            }
        }
        private Nullable<Single> _Monday;
        partial void OnMondayChanging(Nullable<Single> value);
        partial void OnMondayChanged();

        [Range(0.0, 99999999999.0, ErrorMessage = "Liều dùng Thứ 3 không được nhỏ hơn 0")]
        [DataMemberAttribute()]
        public Nullable<Single> Tuesday
        {
            get
            {
                return _Tuesday;
            }
            set
            {
                OnTuesdayChanging(value);
                _Tuesday = value;
                RaisePropertyChanged("Tuesday");
                OnTuesdayChanged();

            }
        }
        private Nullable<Single> _Tuesday;
        partial void OnTuesdayChanging(Nullable<Single> value);
        partial void OnTuesdayChanged();

        [Range(0.0, 99999999999.0, ErrorMessage = "Liều dùng Thứ 4 không được nhỏ hơn 0")]
        [DataMemberAttribute()]
        public Nullable<Single> Wednesday
        {
            get
            {
                return _Wednesday;
            }
            set
            {
                OnWednesdayChanging(value);
                _Wednesday = value;
                RaisePropertyChanged("Wednesday");
                OnWednesdayChanged();

            }
        }
        private Nullable<Single> _Wednesday;
        partial void OnWednesdayChanging(Nullable<Single> value);
        partial void OnWednesdayChanged();


        [Range(0.0, 99999999999.0, ErrorMessage = "Liều dùng Thứ 5 không được nhỏ hơn 0")]
        [DataMemberAttribute()]
        public Nullable<Single> Thursday
        {
            get
            {
                return _Thursday;
            }
            set
            {
                OnThursdayChanging(value);
                _Thursday = value;
                RaisePropertyChanged("Thursday");
                OnThursdayChanged();

            }
        }
        private Nullable<Single> _Thursday;
        partial void OnThursdayChanging(Nullable<Single> value);
        partial void OnThursdayChanged();


        [Range(0.0, 99999999999.0, ErrorMessage = "Liều dùng Thứ 6 không được nhỏ hơn 0")]
        [DataMemberAttribute()]
        public Nullable<Single> Friday
        {
            get
            {
                return _Friday;
            }
            set
            {
                OnFridayChanging(value);
                _Friday = value;
                RaisePropertyChanged("Friday");
                OnFridayChanged();

            }
        }
        private Nullable<Single> _Friday;
        partial void OnFridayChanging(Nullable<Single> value);
        partial void OnFridayChanged();


        [Range(0.0, 99999999999.0, ErrorMessage = "Liều dùng Thứ 7 không được nhỏ hơn 0")]
        [DataMemberAttribute()]
        public Nullable<Single> Saturday
        {
            get
            {
                return _Saturday;
            }
            set
            {
                OnSaturdayChanging(value);
                _Saturday = value;
                RaisePropertyChanged("Saturday");
                OnSaturdayChanged();

            }
        }
        private Nullable<Single> _Saturday;
        partial void OnSaturdayChanging(Nullable<Single> value);
        partial void OnSaturdayChanged();

        [Range(0.0, 99999999999.0, ErrorMessage = "Liều dùng Chủ Nhật không được nhỏ hơn 0")]
        [DataMemberAttribute()]
        public Nullable<Single> Sunday
        {
            get
            {
                return _Sunday;
            }
            set
            {
                OnSundayChanging(value);
                _Sunday = value;
                RaisePropertyChanged("Sunday");
                OnSundayChanged();

            }
        }
        private Nullable<Single> _Sunday;
        partial void OnSundayChanging(Nullable<Single> value);
        partial void OnSundayChanged();


        [DataMemberAttribute()]
        public string MondayStr
        {
            get
            {
                return _MondayStr;
            }
            set
            {
                OnMondayStrChanging(value);
                _MondayStr = value;
                RaisePropertyChanged("MondayStr");
                OnMondayStrChanged();

            }
        }
        private string _MondayStr ="";
        partial void OnMondayStrChanging(string value);
        partial void OnMondayStrChanged();

        [DataMemberAttribute()]
        public string TuesdayStr
        {
            get
            {
                return _TuesdayStr;
            }
            set
            {
                OnTuesdayStrChanging(value);
                _TuesdayStr = value;
                RaisePropertyChanged("TuesdayStr");
                OnTuesdayStrChanged();

            }
        }
        private string _TuesdayStr = "";
        partial void OnTuesdayStrChanging(string value);
        partial void OnTuesdayStrChanged();

        
        [DataMemberAttribute()]
        public string WednesdayStr
        {
            get
            {
                return _WednesdayStr;
            }
            set
            {
                OnWednesdayStrChanging(value);
                _WednesdayStr = value;
                RaisePropertyChanged("WednesdayStr");
                OnWednesdayStrChanged();

            }
        }
        private string _WednesdayStr = "";
        partial void OnWednesdayStrChanging(string value);
        partial void OnWednesdayStrChanged();


        [DataMemberAttribute()]
        public string ThursdayStr
        {
            get
            {
                return _ThursdayStr;
            }
            set
            {
                OnThursdayStrChanging(value);
                _ThursdayStr = value;
                RaisePropertyChanged("ThursdayStr");
                OnThursdayStrChanged();

            }
        }
        private string _ThursdayStr = "";
        partial void OnThursdayStrChanging(string value);
        partial void OnThursdayStrChanged();


        [DataMemberAttribute()]
        public string FridayStr
        {
            get
            {
                return _FridayStr;
            }
            set
            {
                OnFridayStrChanging(value);
                _FridayStr = value;
                RaisePropertyChanged("FridayStr");
                OnFridayStrChanged();

            }
        }
        private string _FridayStr = "";
        partial void OnFridayStrChanging(string value);
        partial void OnFridayStrChanged();


        [DataMemberAttribute()]
        public string SaturdayStr
        {
            get
            {
                return _SaturdayStr;
            }
            set
            {
                OnSaturdayStrChanging(value);
                _SaturdayStr = value;
                RaisePropertyChanged("SaturdayStr");
                OnSaturdayStrChanged();

            }
        }
        private string _SaturdayStr = "";
        partial void OnSaturdayStrChanging(string value);
        partial void OnSaturdayStrChanged();

        [DataMemberAttribute()]
        public string SundayStr
        {
            get
            {
                return _SundayStr;
            }
            set
            {
                OnSundayStrChanging(value);
                _SundayStr = value;
                RaisePropertyChanged("SundayStr");
                OnSundayStrChanged();

            }
        }
        private string _SundayStr = "";
        partial void OnSundayStrChanging(string value);
        partial void OnSundayStrChanged();

        [DataMemberAttribute()]
        public String UsageNote
        {
            get
            {
                return _UsageNote;
            }
            set
            {
                OnUsageNoteChanging(value);
                _UsageNote = value;
                RaisePropertyChanged("UsageNote");
                OnUsageNoteChanged();
            }
        }
        private String _UsageNote;
        partial void OnUsageNoteChanging(String value);
        partial void OnUsageNoteChanged();

        
        /*
        [DataMemberAttribute()]
        public List<PrescriptionDetailSchedulesLieuDung> ObjPrescriptionDetailSchedulesLieuDung
        {
            get
            {
                return _ObjPrescriptionDetailSchedulesLieuDung;
            }
            set
            {
                if (_ObjPrescriptionDetailSchedulesLieuDung != value)
                {
                    OnObjPrescriptionDetailSchedulesLieuDungChanging(value);
                    _ObjPrescriptionDetailSchedulesLieuDung = value;
                    RaisePropertyChanged("ObjPrescriptionDetailSchedulesLieuDung");
                    OnObjPrescriptionDetailSchedulesLieuDungChanged();
                }
            }
        }
        private List<PrescriptionDetailSchedulesLieuDung> _ObjPrescriptionDetailSchedulesLieuDung;
        partial void OnObjPrescriptionDetailSchedulesLieuDungChanging(List<PrescriptionDetailSchedulesLieuDung> value);
        partial void OnObjPrescriptionDetailSchedulesLieuDungChanged();
        */

        //them vao cho nay de tinh 
        [DataMemberAttribute()]
        public PrescriptionDetailSchedulesLieuDung ScheGen
        {
            get
            {
                return _ScheGen;
            }
            set
            {
                OnScheGenChanging(value);
                _ScheGen = value;
                RaisePropertyChanged("ScheGen");
                OnScheGenChanged();

            }
        }
        private PrescriptionDetailSchedulesLieuDung _ScheGen;
        partial void OnScheGenChanging(PrescriptionDetailSchedulesLieuDung value);
        partial void OnScheGenChanged();

        [DataMemberAttribute()]
        public PrescriptionDetailSchedulesLieuDung ScheMonday
        {
            get
            {
                return _ScheMonday;
            }
            set
            {
                OnScheMondayChanging(value);
                _ScheMonday = value;
                if (_ScheMonday!=null)
                {
                    Monday = _ScheMonday.ID;
                    MondayStr = _ScheMonday.Name;
                }
                RaisePropertyChanged("ScheMonday");
                OnScheMondayChanged();

            }
        }
        private PrescriptionDetailSchedulesLieuDung _ScheMonday;
        partial void OnScheMondayChanging(PrescriptionDetailSchedulesLieuDung value);
        partial void OnScheMondayChanged();

        [DataMemberAttribute()]
        public PrescriptionDetailSchedulesLieuDung ScheTuesday
        {
            get
            {
                return _ScheTuesday;
            }
            set
            {
                OnScheTuesdayChanging(value);
                _ScheTuesday = value;
                if (_ScheTuesday != null)
                {
                    Tuesday = _ScheTuesday.ID;
                    TuesdayStr = _ScheTuesday.Name;
                }
                RaisePropertyChanged("ScheTuesday");
                OnScheTuesdayChanged();

            }
        }
        private PrescriptionDetailSchedulesLieuDung _ScheTuesday;
        partial void OnScheTuesdayChanging(PrescriptionDetailSchedulesLieuDung value);
        partial void OnScheTuesdayChanged();

        [DataMemberAttribute()]
        public PrescriptionDetailSchedulesLieuDung ScheWednesday
        {
            get
            {
                return _ScheWednesday;
            }
            set
            {
                OnScheWednesdayChanging(value);
                _ScheWednesday = value;
                if (_ScheWednesday != null)
                {
                    Wednesday = _ScheWednesday.ID;
                    WednesdayStr = _ScheWednesday.Name;
                }
                RaisePropertyChanged("ScheWednesday");
                OnScheWednesdayChanged();

            }
        }
        private PrescriptionDetailSchedulesLieuDung _ScheWednesday;
        partial void OnScheWednesdayChanging(PrescriptionDetailSchedulesLieuDung value);
        partial void OnScheWednesdayChanged();

        [DataMemberAttribute()]
        public PrescriptionDetailSchedulesLieuDung ScheThursday
        {
            get
            {
                return _ScheThursday;
            }
            set
            {
                OnScheThursdayChanging(value);
                _ScheThursday = value;
                if (_ScheThursday != null)
                {
                    Thursday = _ScheThursday.ID;
                    ThursdayStr = _ScheThursday.Name;
                }
                RaisePropertyChanged("ScheThursday");
                OnScheThursdayChanged();

            }
        }
        private PrescriptionDetailSchedulesLieuDung _ScheThursday;
        partial void OnScheThursdayChanging(PrescriptionDetailSchedulesLieuDung value);
        partial void OnScheThursdayChanged();

        [DataMemberAttribute()]
        public PrescriptionDetailSchedulesLieuDung ScheFriday
        {
            get
            {
                return _ScheFriday;
            }
            set
            {
                OnScheFridayChanging(value);
                _ScheFriday = value;
                if (_ScheFriday != null)
                {
                    Friday = _ScheFriday.ID;
                    FridayStr = _ScheFriday.Name;
                }
                RaisePropertyChanged("ScheFriday");
                OnScheFridayChanged();

            }
        }
        private PrescriptionDetailSchedulesLieuDung _ScheFriday;
        partial void OnScheFridayChanging(PrescriptionDetailSchedulesLieuDung value);
        partial void OnScheFridayChanged();

        [DataMemberAttribute()]
        public PrescriptionDetailSchedulesLieuDung ScheSaturday
        {
            get
            {
                return _ScheSaturday;
            }
            set
            {
                OnScheSaturdayChanging(value);
                _ScheSaturday = value;
                if (_ScheSaturday != null)
                {
                    Saturday = _ScheSaturday.ID;
                    SaturdayStr = _ScheSaturday.Name;
                }
                RaisePropertyChanged("ScheSaturday");
                OnScheSaturdayChanged();

            }
        }
        private PrescriptionDetailSchedulesLieuDung _ScheSaturday;
        partial void OnScheSaturdayChanging(PrescriptionDetailSchedulesLieuDung value);
        partial void OnScheSaturdayChanged();

        [DataMemberAttribute()]
        public PrescriptionDetailSchedulesLieuDung ScheSunday
        {
            get
            {
                return _ScheSunday;
            }
            set
            {
                OnScheSundayChanging(value);
                _ScheSunday = value;
                if (_ScheSunday != null)
                {
                    Sunday = _ScheSunday.ID;
                    SundayStr = _ScheSunday.Name;
                }
                RaisePropertyChanged("ScheSunday");
                OnScheSundayChanged();

            }
        }
        private PrescriptionDetailSchedulesLieuDung _ScheSunday;
        partial void OnScheSundayChanging(PrescriptionDetailSchedulesLieuDung value);
        partial void OnScheSundayChanged();

        [DataMemberAttribute()]
        public bool IsNotInCat
        {
            get
            {
                return _IsNotInCat;
            }
            set
            {
                OnIsNotInCatChanging(value);
                _IsNotInCat = value;                
                RaisePropertyChanged("IsNotInCat");
                OnIsNotInCatChanged();

            }
        }
        private bool _IsNotInCat;
        partial void OnIsNotInCatChanging(bool value);
        partial void OnIsNotInCatChanged();

    }
}
