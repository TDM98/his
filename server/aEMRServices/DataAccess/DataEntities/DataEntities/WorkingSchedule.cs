using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class WorkingSchedule : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new WorkingSchedule object.

        /// <param name="sCheduleID">Initial value of the SCheduleID property.</param>
        public static WorkingSchedule CreateWorkingSchedule(long sCheduleID)
        {
            WorkingSchedule workingSchedule = new WorkingSchedule();
            workingSchedule.SCheduleID = sCheduleID;
            return workingSchedule;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long SCheduleID
        {
            get
            {
                return _SCheduleID;
            }
            set
            {
                if (_SCheduleID != value)
                {
                    OnSCheduleIDChanging(value);
                    ////ReportPropertyChanging("SCheduleID");
                    _SCheduleID = value;
                    RaisePropertyChanged("SCheduleID");
                    OnSCheduleIDChanged();
                }
            }
        }
        private long _SCheduleID;
        partial void OnSCheduleIDChanging(long value);
        partial void OnSCheduleIDChanged();





        [DataMemberAttribute()]
        public Nullable<Int64> StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                OnStaffIDChanging(value);
                ////ReportPropertyChanging("StaffID");
                _StaffID = value;
                RaisePropertyChanged("StaffID");
                OnStaffIDChanged();
            }
        }
        private Nullable<Int64> _StaffID;
        partial void OnStaffIDChanging(Nullable<Int64> value);
        partial void OnStaffIDChanged();





        [DataMemberAttribute()]
        public Nullable<DateTime> ScheduleDate
        {
            get
            {
                return _ScheduleDate;
            }
            set
            {
                OnScheduleDateChanging(value);
                ////ReportPropertyChanging("ScheduleDate");
                _ScheduleDate = value;
                RaisePropertyChanged("ScheduleDate");
                OnScheduleDateChanged();
            }
        }
        private Nullable<DateTime> _ScheduleDate;
        partial void OnScheduleDateChanging(Nullable<DateTime> value);
        partial void OnScheduleDateChanged();





        [DataMemberAttribute()]
        public String H0001
        {
            get
            {
                return _H0001;
            }
            set
            {
                OnH0001Changing(value);
                ////ReportPropertyChanging("H0001");
                _H0001 = value;
                RaisePropertyChanged("H0001");
                OnH0001Changed();
            }
        }
        private String _H0001;
        partial void OnH0001Changing(String value);
        partial void OnH0001Changed();





        [DataMemberAttribute()]
        public Nullable<Int32> Loca0001
        {
            get
            {
                return _Loca0001;
            }
            set
            {
                OnLoca0001Changing(value);
                ////ReportPropertyChanging("Loca0001");
                _Loca0001 = value;
                RaisePropertyChanged("Loca0001");
                OnLoca0001Changed();
            }
        }
        private Nullable<Int32> _Loca0001;
        partial void OnLoca0001Changing(Nullable<Int32> value);
        partial void OnLoca0001Changed();





        [DataMemberAttribute()]
        public String H0102
        {
            get
            {
                return _H0102;
            }
            set
            {
                OnH0102Changing(value);
                ////ReportPropertyChanging("H0102");
                _H0102 = value;
                RaisePropertyChanged("H0102");
                OnH0102Changed();
            }
        }
        private String _H0102;
        partial void OnH0102Changing(String value);
        partial void OnH0102Changed();





        [DataMemberAttribute()]
        public Nullable<Int32> Loca0102
        {
            get
            {
                return _Loca0102;
            }
            set
            {
                OnLoca0102Changing(value);
                ////ReportPropertyChanging("Loca0102");
                _Loca0102 = value;
                RaisePropertyChanged("Loca0102");
                OnLoca0102Changed();
            }
        }
        private Nullable<Int32> _Loca0102;
        partial void OnLoca0102Changing(Nullable<Int32> value);
        partial void OnLoca0102Changed();





        [DataMemberAttribute()]
        public String H0203
        {
            get
            {
                return _H0203;
            }
            set
            {
                OnH0203Changing(value);
                ////ReportPropertyChanging("H0203");
                _H0203 = value;
                RaisePropertyChanged("H0203");
                OnH0203Changed();
            }
        }
        private String _H0203;
        partial void OnH0203Changing(String value);
        partial void OnH0203Changed();





        [DataMemberAttribute()]
        public Nullable<Int32> Loca0203
        {
            get
            {
                return _Loca0203;
            }
            set
            {
                OnLoca0203Changing(value);
                ////ReportPropertyChanging("Loca0203");
                _Loca0203 = value;
                RaisePropertyChanged("Loca0203");
                OnLoca0203Changed();
            }
        }
        private Nullable<Int32> _Loca0203;
        partial void OnLoca0203Changing(Nullable<Int32> value);
        partial void OnLoca0203Changed();





        [DataMemberAttribute()]
        public String H0304
        {
            get
            {
                return _H0304;
            }
            set
            {
                OnH0304Changing(value);
                ////ReportPropertyChanging("H0304");
                _H0304 = value;
                RaisePropertyChanged("H0304");
                OnH0304Changed();
            }
        }
        private String _H0304;
        partial void OnH0304Changing(String value);
        partial void OnH0304Changed();





        [DataMemberAttribute()]
        public Nullable<Int32> Loca0304
        {
            get
            {
                return _Loca0304;
            }
            set
            {
                OnLoca0304Changing(value);
                ////ReportPropertyChanging("Loca0304");
                _Loca0304 = value;
                RaisePropertyChanged("Loca0304");
                OnLoca0304Changed();
            }
        }
        private Nullable<Int32> _Loca0304;
        partial void OnLoca0304Changing(Nullable<Int32> value);
        partial void OnLoca0304Changed();





        [DataMemberAttribute()]
        public String H0405
        {
            get
            {
                return _H0405;
            }
            set
            {
                OnH0405Changing(value);
                ////ReportPropertyChanging("H0405");
                _H0405 = value;
                RaisePropertyChanged("H0405");
                OnH0405Changed();
            }
        }
        private String _H0405;
        partial void OnH0405Changing(String value);
        partial void OnH0405Changed();





        [DataMemberAttribute()]
        public Nullable<Int32> Loca0405
        {
            get
            {
                return _Loca0405;
            }
            set
            {
                OnLoca0405Changing(value);
                ////ReportPropertyChanging("Loca0405");
                _Loca0405 = value;
                RaisePropertyChanged("Loca0405");
                OnLoca0405Changed();
            }
        }
        private Nullable<Int32> _Loca0405;
        partial void OnLoca0405Changing(Nullable<Int32> value);
        partial void OnLoca0405Changed();





        [DataMemberAttribute()]
        public String H0506
        {
            get
            {
                return _H0506;
            }
            set
            {
                OnH0506Changing(value);
                ////ReportPropertyChanging("H0506");
                _H0506 = value;
                RaisePropertyChanged("H0506");
                OnH0506Changed();
            }
        }
        private String _H0506;
        partial void OnH0506Changing(String value);
        partial void OnH0506Changed();





        [DataMemberAttribute()]
        public Nullable<Int32> Loca0506
        {
            get
            {
                return _Loca0506;
            }
            set
            {
                OnLoca0506Changing(value);
                ////ReportPropertyChanging("Loca0506");
                _Loca0506 = value;
                RaisePropertyChanged("Loca0506");
                OnLoca0506Changed();
            }
        }
        private Nullable<Int32> _Loca0506;
        partial void OnLoca0506Changing(Nullable<Int32> value);
        partial void OnLoca0506Changed();





        [DataMemberAttribute()]
        public String H0607
        {
            get
            {
                return _H0607;
            }
            set
            {
                OnH0607Changing(value);
                ////ReportPropertyChanging("H0607");
                _H0607 = value;
                RaisePropertyChanged("H0607");
                OnH0607Changed();
            }
        }
        private String _H0607;
        partial void OnH0607Changing(String value);
        partial void OnH0607Changed();





        [DataMemberAttribute()]
        public Nullable<Int32> Loca0607
        {
            get
            {
                return _Loca0607;
            }
            set
            {
                OnLoca0607Changing(value);
                ////ReportPropertyChanging("Loca0607");
                _Loca0607 = value;
                RaisePropertyChanged("Loca0607");
                OnLoca0607Changed();
            }
        }
        private Nullable<Int32> _Loca0607;
        partial void OnLoca0607Changing(Nullable<Int32> value);
        partial void OnLoca0607Changed();





        [DataMemberAttribute()]
        public String H0708
        {
            get
            {
                return _H0708;
            }
            set
            {
                OnH0708Changing(value);
                ////ReportPropertyChanging("H0708");
                _H0708 = value;
                RaisePropertyChanged("H0708");
                OnH0708Changed();
            }
        }
        private String _H0708;
        partial void OnH0708Changing(String value);
        partial void OnH0708Changed();





        [DataMemberAttribute()]
        public Nullable<Int32> Loca0708
        {
            get
            {
                return _Loca0708;
            }
            set
            {
                OnLoca0708Changing(value);
                ////ReportPropertyChanging("Loca0708");
                _Loca0708 = value;
                RaisePropertyChanged("Loca0708");
                OnLoca0708Changed();
            }
        }
        private Nullable<Int32> _Loca0708;
        partial void OnLoca0708Changing(Nullable<Int32> value);
        partial void OnLoca0708Changed();





        [DataMemberAttribute()]
        public String H0809
        {
            get
            {
                return _H0809;
            }
            set
            {
                OnH0809Changing(value);
                ////ReportPropertyChanging("H0809");
                _H0809 = value;
                RaisePropertyChanged("H0809");
                OnH0809Changed();
            }
        }
        private String _H0809;
        partial void OnH0809Changing(String value);
        partial void OnH0809Changed();





        [DataMemberAttribute()]
        public Nullable<Int32> Loca0809
        {
            get
            {
                return _Loca0809;
            }
            set
            {
                OnLoca0809Changing(value);
                ////ReportPropertyChanging("Loca0809");
                _Loca0809 = value;
                RaisePropertyChanged("Loca0809");
                OnLoca0809Changed();
            }
        }
        private Nullable<Int32> _Loca0809;
        partial void OnLoca0809Changing(Nullable<Int32> value);
        partial void OnLoca0809Changed();





        [DataMemberAttribute()]
        public String H0910
        {
            get
            {
                return _H0910;
            }
            set
            {
                OnH0910Changing(value);
                ////ReportPropertyChanging("H0910");
                _H0910 = value;
                RaisePropertyChanged("H0910");
                OnH0910Changed();
            }
        }
        private String _H0910;
        partial void OnH0910Changing(String value);
        partial void OnH0910Changed();





        [DataMemberAttribute()]
        public Nullable<Int32> Loca0910
        {
            get
            {
                return _Loca0910;
            }
            set
            {
                OnLoca0910Changing(value);
                ////ReportPropertyChanging("Loca0910");
                _Loca0910 = value;
                RaisePropertyChanged("Loca0910");
                OnLoca0910Changed();
            }
        }
        private Nullable<Int32> _Loca0910;
        partial void OnLoca0910Changing(Nullable<Int32> value);
        partial void OnLoca0910Changed();





        [DataMemberAttribute()]
        public String H1011
        {
            get
            {
                return _H1011;
            }
            set
            {
                OnH1011Changing(value);
                ////ReportPropertyChanging("H1011");
                _H1011 = value;
                RaisePropertyChanged("H1011");
                OnH1011Changed();
            }
        }
        private String _H1011;
        partial void OnH1011Changing(String value);
        partial void OnH1011Changed();





        [DataMemberAttribute()]
        public Nullable<Int32> Loca1011
        {
            get
            {
                return _Loca1011;
            }
            set
            {
                OnLoca1011Changing(value);
                ////ReportPropertyChanging("Loca1011");
                _Loca1011 = value;
                RaisePropertyChanged("Loca1011");
                OnLoca1011Changed();
            }
        }
        private Nullable<Int32> _Loca1011;
        partial void OnLoca1011Changing(Nullable<Int32> value);
        partial void OnLoca1011Changed();





        [DataMemberAttribute()]
        public String H1112
        {
            get
            {
                return _H1112;
            }
            set
            {
                OnH1112Changing(value);
                ////ReportPropertyChanging("H1112");
                _H1112 = value;
                RaisePropertyChanged("H1112");
                OnH1112Changed();
            }
        }
        private String _H1112;
        partial void OnH1112Changing(String value);
        partial void OnH1112Changed();





        [DataMemberAttribute()]
        public Nullable<Int32> Loca1112
        {
            get
            {
                return _Loca1112;
            }
            set
            {
                OnLoca1112Changing(value);
                ////ReportPropertyChanging("Loca1112");
                _Loca1112 = value;
                RaisePropertyChanged("Loca1112");
                OnLoca1112Changed();
            }
        }
        private Nullable<Int32> _Loca1112;
        partial void OnLoca1112Changing(Nullable<Int32> value);
        partial void OnLoca1112Changed();





        [DataMemberAttribute()]
        public String H1213
        {
            get
            {
                return _H1213;
            }
            set
            {
                OnH1213Changing(value);
                ////ReportPropertyChanging("H1213");
                _H1213 = value;
                RaisePropertyChanged("H1213");
                OnH1213Changed();
            }
        }
        private String _H1213;
        partial void OnH1213Changing(String value);
        partial void OnH1213Changed();





        [DataMemberAttribute()]
        public Nullable<Int32> Loca1213
        {
            get
            {
                return _Loca1213;
            }
            set
            {
                OnLoca1213Changing(value);
                ////ReportPropertyChanging("Loca1213");
                _Loca1213 = value;
                RaisePropertyChanged("Loca1213");
                OnLoca1213Changed();
            }
        }
        private Nullable<Int32> _Loca1213;
        partial void OnLoca1213Changing(Nullable<Int32> value);
        partial void OnLoca1213Changed();





        [DataMemberAttribute()]
        public String H1314
        {
            get
            {
                return _H1314;
            }
            set
            {
                OnH1314Changing(value);
                ////ReportPropertyChanging("H1314");
                _H1314 = value;
                RaisePropertyChanged("H1314");
                OnH1314Changed();
            }
        }
        private String _H1314;
        partial void OnH1314Changing(String value);
        partial void OnH1314Changed();





        [DataMemberAttribute()]
        public Nullable<Int32> Loca1314
        {
            get
            {
                return _Loca1314;
            }
            set
            {
                OnLoca1314Changing(value);
                ////ReportPropertyChanging("Loca1314");
                _Loca1314 = value;
                RaisePropertyChanged("Loca1314");
                OnLoca1314Changed();
            }
        }
        private Nullable<Int32> _Loca1314;
        partial void OnLoca1314Changing(Nullable<Int32> value);
        partial void OnLoca1314Changed();





        [DataMemberAttribute()]
        public String H1415
        {
            get
            {
                return _H1415;
            }
            set
            {
                OnH1415Changing(value);
                ////ReportPropertyChanging("H1415");
                _H1415 = value;
                RaisePropertyChanged("H1415");
                OnH1415Changed();
            }
        }
        private String _H1415;
        partial void OnH1415Changing(String value);
        partial void OnH1415Changed();





        [DataMemberAttribute()]
        public Nullable<Int32> Loca1415
        {
            get
            {
                return _Loca1415;
            }
            set
            {
                OnLoca1415Changing(value);
                ////ReportPropertyChanging("Loca1415");
                _Loca1415 = value;
                RaisePropertyChanged("Loca1415");
                OnLoca1415Changed();
            }
        }
        private Nullable<Int32> _Loca1415;
        partial void OnLoca1415Changing(Nullable<Int32> value);
        partial void OnLoca1415Changed();





        [DataMemberAttribute()]
        public String H1516
        {
            get
            {
                return _H1516;
            }
            set
            {
                OnH1516Changing(value);
                ////ReportPropertyChanging("H1516");
                _H1516 = value;
                RaisePropertyChanged("H1516");
                OnH1516Changed();
            }
        }
        private String _H1516;
        partial void OnH1516Changing(String value);
        partial void OnH1516Changed();





        [DataMemberAttribute()]
        public Nullable<Int32> Loca1516
        {
            get
            {
                return _Loca1516;
            }
            set
            {
                OnLoca1516Changing(value);
                ////ReportPropertyChanging("Loca1516");
                _Loca1516 = value;
                RaisePropertyChanged("Loca1516");
                OnLoca1516Changed();
            }
        }
        private Nullable<Int32> _Loca1516;
        partial void OnLoca1516Changing(Nullable<Int32> value);
        partial void OnLoca1516Changed();





        [DataMemberAttribute()]
        public String H1617
        {
            get
            {
                return _H1617;
            }
            set
            {
                OnH1617Changing(value);
                ////ReportPropertyChanging("H1617");
                _H1617 = value;
                RaisePropertyChanged("H1617");
                OnH1617Changed();
            }
        }
        private String _H1617;
        partial void OnH1617Changing(String value);
        partial void OnH1617Changed();





        [DataMemberAttribute()]
        public Nullable<Int32> Loca1617
        {
            get
            {
                return _Loca1617;
            }
            set
            {
                OnLoca1617Changing(value);
                ////ReportPropertyChanging("Loca1617");
                _Loca1617 = value;
                RaisePropertyChanged("Loca1617");
                OnLoca1617Changed();
            }
        }
        private Nullable<Int32> _Loca1617;
        partial void OnLoca1617Changing(Nullable<Int32> value);
        partial void OnLoca1617Changed();





        [DataMemberAttribute()]
        public String H1718
        {
            get
            {
                return _H1718;
            }
            set
            {
                OnH1718Changing(value);
                ////ReportPropertyChanging("H1718");
                _H1718 = value;
                RaisePropertyChanged("H1718");
                OnH1718Changed();
            }
        }
        private String _H1718;
        partial void OnH1718Changing(String value);
        partial void OnH1718Changed();





        [DataMemberAttribute()]
        public Nullable<Int32> Loca1718
        {
            get
            {
                return _Loca1718;
            }
            set
            {
                OnLoca1718Changing(value);
                ////ReportPropertyChanging("Loca1718");
                _Loca1718 = value;
                RaisePropertyChanged("Loca1718");
                OnLoca1718Changed();
            }
        }
        private Nullable<Int32> _Loca1718;
        partial void OnLoca1718Changing(Nullable<Int32> value);
        partial void OnLoca1718Changed();





        [DataMemberAttribute()]
        public String H1819
        {
            get
            {
                return _H1819;
            }
            set
            {
                OnH1819Changing(value);
                ////ReportPropertyChanging("H1819");
                _H1819 = value;
                RaisePropertyChanged("H1819");
                OnH1819Changed();
            }
        }
        private String _H1819;
        partial void OnH1819Changing(String value);
        partial void OnH1819Changed();





        [DataMemberAttribute()]
        public Nullable<Int32> Loca1819
        {
            get
            {
                return _Loca1819;
            }
            set
            {
                OnLoca1819Changing(value);
                ////ReportPropertyChanging("Loca1819");
                _Loca1819 = value;
                RaisePropertyChanged("Loca1819");
                OnLoca1819Changed();
            }
        }
        private Nullable<Int32> _Loca1819;
        partial void OnLoca1819Changing(Nullable<Int32> value);
        partial void OnLoca1819Changed();





        [DataMemberAttribute()]
        public String H1920
        {
            get
            {
                return _H1920;
            }
            set
            {
                OnH1920Changing(value);
                ////ReportPropertyChanging("H1920");
                _H1920 = value;
                RaisePropertyChanged("H1920");
                OnH1920Changed();
            }
        }
        private String _H1920;
        partial void OnH1920Changing(String value);
        partial void OnH1920Changed();





        [DataMemberAttribute()]
        public Nullable<Int32> Loca1920
        {
            get
            {
                return _Loca1920;
            }
            set
            {
                OnLoca1920Changing(value);
                ////ReportPropertyChanging("Loca1920");
                _Loca1920 = value;
                RaisePropertyChanged("Loca1920");
                OnLoca1920Changed();
            }
        }
        private Nullable<Int32> _Loca1920;
        partial void OnLoca1920Changing(Nullable<Int32> value);
        partial void OnLoca1920Changed();





        [DataMemberAttribute()]
        public String H2021
        {
            get
            {
                return _H2021;
            }
            set
            {
                OnH2021Changing(value);
                ////ReportPropertyChanging("H2021");
                _H2021 = value;
                RaisePropertyChanged("H2021");
                OnH2021Changed();
            }
        }
        private String _H2021;
        partial void OnH2021Changing(String value);
        partial void OnH2021Changed();





        [DataMemberAttribute()]
        public Nullable<Int32> Loca2021
        {
            get
            {
                return _Loca2021;
            }
            set
            {
                OnLoca2021Changing(value);
                ////ReportPropertyChanging("Loca2021");
                _Loca2021 = value;
                RaisePropertyChanged("Loca2021");
                OnLoca2021Changed();
            }
        }
        private Nullable<Int32> _Loca2021;
        partial void OnLoca2021Changing(Nullable<Int32> value);
        partial void OnLoca2021Changed();





        [DataMemberAttribute()]
        public String H2122
        {
            get
            {
                return _H2122;
            }
            set
            {
                OnH2122Changing(value);
                ////ReportPropertyChanging("H2122");
                _H2122 = value;
                RaisePropertyChanged("H2122");
                OnH2122Changed();
            }
        }
        private String _H2122;
        partial void OnH2122Changing(String value);
        partial void OnH2122Changed();





        [DataMemberAttribute()]
        public Nullable<Int32> Loca2122
        {
            get
            {
                return _Loca2122;
            }
            set
            {
                OnLoca2122Changing(value);
                ////ReportPropertyChanging("Loca2122");
                _Loca2122 = value;
                RaisePropertyChanged("Loca2122");
                OnLoca2122Changed();
            }
        }
        private Nullable<Int32> _Loca2122;
        partial void OnLoca2122Changing(Nullable<Int32> value);
        partial void OnLoca2122Changed();





        [DataMemberAttribute()]
        public String H2223
        {
            get
            {
                return _H2223;
            }
            set
            {
                OnH2223Changing(value);
                ////ReportPropertyChanging("H2223");
                _H2223 = value;
                RaisePropertyChanged("H2223");
                OnH2223Changed();
            }
        }
        private String _H2223;
        partial void OnH2223Changing(String value);
        partial void OnH2223Changed();





        [DataMemberAttribute()]
        public Nullable<Int32> Loca2223
        {
            get
            {
                return _Loca2223;
            }
            set
            {
                OnLoca2223Changing(value);
                ////ReportPropertyChanging("Loca2223");
                _Loca2223 = value;
                RaisePropertyChanged("Loca2223");
                OnLoca2223Changed();
            }
        }
        private Nullable<Int32> _Loca2223;
        partial void OnLoca2223Changing(Nullable<Int32> value);
        partial void OnLoca2223Changed();





        [DataMemberAttribute()]
        public String H2324
        {
            get
            {
                return _H2324;
            }
            set
            {
                OnH2324Changing(value);
                ////ReportPropertyChanging("H2324");
                _H2324 = value;
                RaisePropertyChanged("H2324");
                OnH2324Changed();
            }
        }
        private String _H2324;
        partial void OnH2324Changing(String value);
        partial void OnH2324Changed();





        [DataMemberAttribute()]
        public Nullable<Int32> Loca2324
        {
            get
            {
                return _Loca2324;
            }
            set
            {
                OnLoca2324Changing(value);
                ////ReportPropertyChanging("Loca2324");
                _Loca2324 = value;
                RaisePropertyChanged("Loca2324");
                OnLoca2324Changed();
            }
        }
        private Nullable<Int32> _Loca2324;
        partial void OnLoca2324Changing(Nullable<Int32> value);
        partial void OnLoca2324Changed();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_SCHEDULE_REL_PTAPP_WORKINGS", "ScheduledJob")]
        public ObservableCollection<ScheduledJob> ScheduledJobs
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_WORKINGS_REL_PTAPP_STAFFS", "Staffs")]
        public Staff Staff
        {
            get;
            set;
        }

        #endregion
    }
}
