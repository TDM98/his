using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using aEMR.Common.BaseModel;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using aEMR.Common.Collections;
using aEMR.Infrastructure;
using System.Threading;
using aEMR.ServiceClient;
using eHCMSLanguage;
using DataEntities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using aEMR.CommonTasks;
using Caliburn.Micro;
using System.Xml.Linq;
using aEMR.Controls;
using System.ComponentModel;
using System.Windows.Data;
using System.Globalization;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure.Events;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IOvertimeWorkingSchedule)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class OvertimeWorkingScheduleViewModel : ViewModelBase, IOvertimeWorkingSchedule
    {
        #region Properties
        private int[] _WeekCollection = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25,
            26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52 };
        public int[] WeekCollection
        {
            get
            {
                return _WeekCollection;
            }
            set
            {
                _WeekCollection = value;
                NotifyOfPropertyChange(() => WeekCollection);
            }
        }
        private int _CurrentWeek;
        public int CurrentWeek
        {
            get
            {
                return _CurrentWeek;
            }
            set
            {
                if (_CurrentWeek == value)
                {
                    return;
                }
                _CurrentWeek = value;
                NotifyOfPropertyChange(() => CurrentWeek);
            }
        }
        private int[] _YearCollection = new int[] { DateTime.Now.Year - 1, DateTime.Now.Year, DateTime.Now.Year + 1 };
        public int[] YearCollection
        {
            get
            {
                return _YearCollection;
            }
            set
            {
                if (_YearCollection == value)
                {
                    return;
                }
                _YearCollection = value;
                NotifyOfPropertyChange(() => YearCollection);
            }
        }
        private int _CurrentYear = DateTime.Now.Year;
        public int CurrentYear
        {
            get
            {
                return _CurrentYear;
            }
            set
            {
                if (_CurrentYear == value)
                {
                    return;
                }
                _CurrentYear = value;
                NotifyOfPropertyChange(() => CurrentYear);
            }
        }
        private string _DayWeek;
        public string DayWeek
        {
            get
            {
                return _DayWeek;
            }
            set
            {
                if (_DayWeek == value)
                    return;
                _DayWeek = value;
                NotifyOfPropertyChange(() => DayWeek);
            }
        }
        private List<DateTime> _ListDateOfCurWeek;
        public List<DateTime> ListDateOfCurWeek
        {
            get
            {
                return _ListDateOfCurWeek;
            }
            set
            {
                if (_ListDateOfCurWeek == value)
                    return;
                _ListDateOfCurWeek = value;
                NotifyOfPropertyChange(() => ListDateOfCurWeek);
            }
        }
        //private DateTime _FromDate;
        //public DateTime FromDate
        //{
        //    get
        //    {
        //        return _FromDate;
        //    }
        //    set
        //    {
        //        if (_FromDate == value)
        //        {
        //            return;
        //        }
        //        _FromDate = value;
        //        NotifyOfPropertyChange(() => FromDate);
        //    }
        //}
        //private DateTime _ToDate;
        //public DateTime ToDate
        //{
        //    get
        //    {
        //        return _ToDate;
        //    }
        //    set
        //    {
        //        if (_ToDate == value)
        //        {
        //            return;
        //        }
        //        _ToDate = value;
        //        NotifyOfPropertyChange(() => ToDate);
        //    }
        //}
        private DateTime _MondayDay;
        public DateTime MondayDay
        {
            get
            {
                return _MondayDay;
            }
            set
            {
                if (_MondayDay == value)
                    return;
                _MondayDay = value;
                NotifyOfPropertyChange(() => MondayDay);
            }
        }
        private DateTime _TuesdayDay;
        public DateTime TuesdayDay
        {
            get
            {
                return _TuesdayDay;
            }
            set
            {
                if (_TuesdayDay == value)
                    return;
                _TuesdayDay = value;
                NotifyOfPropertyChange(() => TuesdayDay);
            }
        }
        private DateTime _WednesdayDay;
        public DateTime WednesdayDay
        {
            get
            {
                return _WednesdayDay;
            }
            set
            {
                if (_WednesdayDay == value)
                    return;
                _WednesdayDay = value;
                NotifyOfPropertyChange(() => WednesdayDay);
            }
        }
        private DateTime _ThursdayDay;
        public DateTime ThursdayDay
        {
            get
            {
                return _ThursdayDay;
            }
            set
            {
                if (_ThursdayDay == value)
                    return;
                _ThursdayDay = value;
                NotifyOfPropertyChange(() => ThursdayDay);
            }
        }
        private DateTime _FridayDay;
        public DateTime FridayDay
        {
            get
            {
                return _FridayDay;
            }
            set
            {
                if (_FridayDay == value)
                    return;
                _FridayDay = value;
                NotifyOfPropertyChange(() => FridayDay);
            }
        }
        private DateTime _SaturdayDay;
        public DateTime SaturdayDay
        {
            get
            {
                return _SaturdayDay;
            }
            set
            {
                if (_SaturdayDay == value)
                    return;
                _SaturdayDay = value;
                NotifyOfPropertyChange(() => SaturdayDay);
            }
        }
        private DateTime _SundayDay;
        public DateTime SundayDay
        {
            get
            {
                return _SundayDay;
            }
            set
            {
                if (_SundayDay == value)
                    return;
                _SundayDay = value;
                NotifyOfPropertyChange(() => SundayDay);
            }
        }
        private OvertimeWorkingWeek _CurrentOvertimeWorkingWeek;
        public OvertimeWorkingWeek CurrentOvertimeWorkingWeek
        {
            get
            {
                return _CurrentOvertimeWorkingWeek;
            }
            set
            {
                if (_CurrentOvertimeWorkingWeek == value)
                    return;
                _CurrentOvertimeWorkingWeek = value;
                NotifyOfPropertyChange(() => CurrentOvertimeWorkingWeek);
            }
        }
        private ObservableCollection<OvertimeWorkingSchedule> _MondaySchedule;
        public ObservableCollection<OvertimeWorkingSchedule> MondaySchedule
        {
            get
            {
                return _MondaySchedule;
            }
            set
            {
                if (_MondaySchedule == value)
                    return;
                _MondaySchedule = value;
                NotifyOfPropertyChange(() => MondaySchedule);
            }
        }
        private ObservableCollection<OvertimeWorkingSchedule> _TuesdaySchedule;
        public ObservableCollection<OvertimeWorkingSchedule> TuesdaySchedule
        {
            get
            {
                return _TuesdaySchedule;
            }
            set
            {
                if (_TuesdaySchedule == value)
                    return;
                _TuesdaySchedule = value;
                NotifyOfPropertyChange(() => TuesdaySchedule);
            }
        }
        private ObservableCollection<OvertimeWorkingSchedule> _WednesdaySchedule;
        public ObservableCollection<OvertimeWorkingSchedule> WednesdaySchedule
        {
            get
            {
                return _WednesdaySchedule;
            }
            set
            {
                if (_WednesdaySchedule == value)
                    return;
                _WednesdaySchedule = value;
                NotifyOfPropertyChange(() => WednesdaySchedule);
            }
        }
        private ObservableCollection<OvertimeWorkingSchedule> _ThursdaySchedule;
        public ObservableCollection<OvertimeWorkingSchedule> ThursdaySchedule
        {
            get
            {
                return _ThursdaySchedule;
            }
            set
            {
                if (_ThursdaySchedule == value)
                    return;
                _ThursdaySchedule = value;
                NotifyOfPropertyChange(() => ThursdaySchedule);
            }
        }
        private ObservableCollection<OvertimeWorkingSchedule> _FridaySchedule;
        public ObservableCollection<OvertimeWorkingSchedule> FridaySchedule
        {
            get
            {
                return _FridaySchedule;
            }
            set
            {
                if (_FridaySchedule == value)
                    return;
                _FridaySchedule = value;
                NotifyOfPropertyChange(() => FridaySchedule);
            }
        }
        private ObservableCollection<OvertimeWorkingSchedule> _SaturdaySchedule;
        public ObservableCollection<OvertimeWorkingSchedule> SaturdaySchedule
        {
            get
            {
                return _SaturdaySchedule;
            }
            set
            {
                if (_SaturdaySchedule == value)
                    return;
                _SaturdaySchedule = value;
                NotifyOfPropertyChange(() => SaturdaySchedule);
            }
        }
        private ObservableCollection<OvertimeWorkingSchedule> _SundaySchedule;
        public ObservableCollection<OvertimeWorkingSchedule> SundaySchedule
        {
            get
            {
                return _SundaySchedule;
            }
            set
            {
                if (_SundaySchedule == value)
                    return;
                _SundaySchedule = value;
                NotifyOfPropertyChange(() => SundaySchedule);
            }
        }
        private ObservableCollection<Staff> _DoctorStaffs;
        public ObservableCollection<Staff> DoctorStaffs
        {
            get
            {
                return _DoctorStaffs;
            }
            set
            {
                if (_DoctorStaffs != value)
                {
                    _DoctorStaffs = value;
                    NotifyOfPropertyChange(() => DoctorStaffs);
                }
            }
        }
        private ObservableCollection<DeptLocation> _LocationCollection;
        public ObservableCollection<DeptLocation> LocationCollection
        {
            get
            {
                return _LocationCollection;
            }
            set
            {
                if (_LocationCollection == value)
                {
                    return;
                }
                _LocationCollection = value;
                NotifyOfPropertyChange(() => LocationCollection);
            }
        }
        private bool _bOvertimeClinicManager_KHTH = true;
        public bool bOvertimeClinicManager_KHTH
        {
            get
            {
                return _bOvertimeClinicManager_KHTH;
            }
            set
            {
                if (_bOvertimeClinicManager_KHTH == value)
                    return;
                _bOvertimeClinicManager_KHTH = value;
            }
        }
        private bool _bOvertimeClinicManager_TC = true;
        public bool bOvertimeClinicManager_TC
        {
            get
            {
                return _bOvertimeClinicManager_TC;
            }
            set
            {
                if (_bOvertimeClinicManager_TC == value)
                    return;
                _bOvertimeClinicManager_TC = value;
            }
        }
        private bool _btnSaveEnabled = true;
        public bool btnSaveEnabled
        {
            get
            {
                return _btnSaveEnabled;
            }
            set
            {
                if (_btnSaveEnabled == value)
                    return;
                _btnSaveEnabled = value;
                NotifyOfPropertyChange(() => btnSaveEnabled);
            }
        }
        private bool _btnSendOrganizeRoomEnabled = true;
        public bool btnSendOrganizeRoomEnabled
        {
            get
            {
                return _btnSendOrganizeRoomEnabled;
            }
            set
            {
                if (_btnSendOrganizeRoomEnabled == value)
                    return;
                _btnSendOrganizeRoomEnabled = value;
                NotifyOfPropertyChange(() => btnSendOrganizeRoomEnabled);
            }
        }
        private bool _btnSendGeneralPlanRoomEnabled = true;
        public bool btnSendGeneralPlanRoomEnabled
        {
            get
            {
                return _btnSendGeneralPlanRoomEnabled;
            }
            set
            {
                if (_btnSendGeneralPlanRoomEnabled == value)
                    return;
                _btnSendGeneralPlanRoomEnabled = value;
                NotifyOfPropertyChange(() => btnSendGeneralPlanRoomEnabled);
            }
        }
        private bool _btnConfirmEnabled = true;
        public bool btnConfirmEnabled
        {
            get
            {
                return _btnConfirmEnabled;
            }
            set
            {
                if (_btnConfirmEnabled == value)
                    return;
                _btnConfirmEnabled = value;
                NotifyOfPropertyChange(() => btnConfirmEnabled);
            }
        }
        private bool _btnLockEnabled = true;
        public bool btnLockEnabled
        {
            get
            {
                return _btnLockEnabled;
            }
            set
            {
                if (_btnLockEnabled == value)
                    return;
                _btnLockEnabled = value;
                NotifyOfPropertyChange(() => btnLockEnabled);
            }
        }
        private DateTime _SelectedDate = Globals.GetCurServerDateTime();
        public DateTime SelectedDate
        {
            get
            {
                return _SelectedDate;
            }
            set
            {
                if (_SelectedDate == value)
                    return;
                _SelectedDate = value;
                NotifyOfPropertyChange(() => SelectedDate);
            }
        }
        #endregion
        #region Function
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

            bOvertimeClinicManager_KHTH = Globals.CheckOperation(Globals.listRefModule, (int)eModules.mClinicManagement, (int)eClinicManagement.mLichLamViecNgoaiGio
                , (int)oClinicManagementEx.mLichNgoaiGio_KHTH);
            bOvertimeClinicManager_TC = Globals.CheckOperation(Globals.listRefModule, (int)eModules.mClinicManagement, (int)eClinicManagement.mLichLamViecNgoaiGio
                , (int)oClinicManagementEx.mLichNgoaiGio_TC);
        }
        private int GetCurrentWeek(DateTime inputDate)
        {
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(inputDate, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
        }
        private DateTime GetFirstDayOfWeek(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Sunday - jan1.DayOfWeek;
            DateTime firstMonday = jan1.AddDays(daysOffset+1);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstMonday, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
            var weekNum = weekOfYear;
            if (firstWeek == 1)
            {
                weekNum -= 1;
            }
            return firstMonday.AddDays(weekNum * 7);
            //DateTime jan1 = new DateTime(year, 1, 1);
            //int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            //// Use first Thursday in January to get first week of the year as
            //// it will never be in Week 52/53
            //DateTime firstThursday = jan1.AddDays(daysOffset);
            //var cal = CultureInfo.CurrentCulture.Calendar;
            //int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);

            //var weekNum = weekOfYear;
            //// As we're adding days to a date in Week 1,
            //// we need to subtract 1 in order to get the right date for week #1
            //if (firstWeek == 1)
            //{
            //    weekNum -= 1;
            //}

            //// Using the first Thursday as starting week ensures that we are starting in the right year
            //// then we add number of weeks multiplied with days
            //var result = firstThursday.AddDays(weekNum * 7);

            //// Subtract 3 days from Thursday to get Monday, which is the first weekday in ISO8601
            //return result.AddDays(-3);
        }
       
        private void InitCalendar()
        {
            DateTime firstDayCurWeek = GetFirstDayOfWeek(CurrentYear,CurrentWeek);
            //DateTime today = DateTime.Today;
            //int currentDayOfWeek = (int)today.DayOfWeek;
            //DateTime sunday = firstDayCurWeek.AddDays(-currentDayOfWeek);
            //DateTime monday = sunday.AddDays(1);
            // If we started on Sunday, we should actually have gone *back*
            // 6 days instead of forward 1...
            //if (currentDayOfWeek == 0)
            //{
            //    monday = monday.AddDays(-7);
            //}
            ListDateOfCurWeek = Enumerable.Range(0, 7).Select(days => firstDayCurWeek.AddDays(days)).ToList();
            foreach (var date in ListDateOfCurWeek)
            {
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        DayWeek = date.Date.ToString("dd/MM/yyyy");
                        MondayDay = date.Date;
                        //FromDate = date.Date;
                        break;
                    case DayOfWeek.Tuesday:
                        TuesdayDay = date.Date;
                        break;
                    case DayOfWeek.Wednesday:
                        WednesdayDay = date.Date;
                        break;
                    case DayOfWeek.Thursday:
                        ThursdayDay = date.Date;
                        break;
                    case DayOfWeek.Friday:
                        FridayDay = date.Date;
                        break;
                    case DayOfWeek.Saturday:
                        SaturdayDay = date.Date;
                        break;
                    case DayOfWeek.Sunday:
                        DayWeek += " - "+ date.Date.ToString("dd/MM/yyyy");
                        SundayDay = date.Date;
                        //ToDate = date.Date;
                        break;
                }
            }
            LoadOvertimeWorkingWeekByDate();
        }
        private void InitNewWeek()
        {
            CurrentOvertimeWorkingWeek = new OvertimeWorkingWeek {
                Week = CurrentWeek,
                FromDate = MondayDay,
                ToDate = SundayDay,
                V_OvertimeWorkingWeekStatus = (long)AllLookupValues.V_OvertimeWorkingWeekStatus.Luu,
                CreatedStaffID = Globals.LoggedUserAccount.Staff.StaffID,
                CreatedDate = Globals.GetCurServerDateTime(),
            };
            InitNewSchedule();
            SetButtonByOvertimeWorkingWeekStatus();
        }
        private void InitNewSchedule()
        {
            MondaySchedule = new ObservableCollection<OvertimeWorkingSchedule>();
            TuesdaySchedule = new ObservableCollection<OvertimeWorkingSchedule>();
            WednesdaySchedule = new ObservableCollection<OvertimeWorkingSchedule>();
            ThursdaySchedule = new ObservableCollection<OvertimeWorkingSchedule>();
            FridaySchedule = new ObservableCollection<OvertimeWorkingSchedule>();
            SaturdaySchedule = new ObservableCollection<OvertimeWorkingSchedule>();
            SundaySchedule = new ObservableCollection<OvertimeWorkingSchedule>();
        }
        private void LoadOvertimeWorkingWeekByDate()
        {
            if (CurrentWeek == 0 || MondayDay == null || SundayDay == null)
            {
                return;
            }

            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetOvertimeWorkingWeekByDate(CurrentWeek, MondayDay, SundayDay, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetOvertimeWorkingWeekByDate(asyncResult);
                            if (results != null)
                            {
                                CurrentOvertimeWorkingWeek = results;
                                LoadOvertimeWorkingScheduleByDate();
                                SetButtonByOvertimeWorkingWeekStatus();
                            }
                            else
                            {
                                InitNewWeek();
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            t.Start();
        }
        private void LoadOvertimeWorkingScheduleByDate()
        {
            if (CurrentWeek == 0 || MondayDay == null || SundayDay == null)
            {
                return;
            }
           
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetOvertimeWorkingScheduleByDate(MondayDay, SundayDay, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetOvertimeWorkingScheduleByDate(asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                //    foreach (var aWeek in CalendarWeekCollection)
                                //    {
                                //        foreach (var aDay in aWeek.CalendarDayCollection)
                                //        {
                                //            if (results.Any(x => x.AllocationDate == aDay.CalendarDate))
                                //            {
                                //                aDay.StaffWorkingSchedule = results.Where(x => x.AllocationDate == aDay.CalendarDate).Select(x => new StaffWorkingSchedule { ConsultationRoomStaffAllocationServiceListID = x.ConsultationRoomStaffAllocationServiceListID, ServiceItemCollection = x.ServiceItemCollection, CurrentDeptLocation = x.DeptLocation != null && x.DeptLocation.DeptLocationID == 0 ? null : x.DeptLocation, ConsultationSegment = new ConsultationTimeSegments { ConsultationTimeSegmentID = x.ConsultationTimeSegmentID, SegmentName = x.ConsultationTimeSegments.SegmentName }, DoctorStaff = x.Staff }).ToObservableCollection();
                                //                if (aDay.StaffWorkingScheduleView.Filter == null)
                                //                {
                                //                    aDay.StaffWorkingScheduleView.Filter = CustomerFilter;
                                //                }
                                //            }
                                //        }
                                //    }
                                foreach (var date in ListDateOfCurWeek)
                                {
                                    var tempSchedule = results.Where(x => x.WorkDate.Date == date.Date);
                                    if (tempSchedule.Count() > 0)
                                    {
                                        switch (date.DayOfWeek)
                                        {
                                            case DayOfWeek.Monday:
                                                MondaySchedule = tempSchedule.ToObservableCollection();
                                                break;
                                            case DayOfWeek.Tuesday:
                                                TuesdaySchedule = tempSchedule.ToObservableCollection();
                                                break;
                                            case DayOfWeek.Wednesday:
                                                WednesdaySchedule = tempSchedule.ToObservableCollection();
                                                break;
                                            case DayOfWeek.Thursday:
                                                ThursdaySchedule = tempSchedule.ToObservableCollection();
                                                break;
                                            case DayOfWeek.Friday:
                                                FridaySchedule = tempSchedule.ToObservableCollection();
                                                break;
                                            case DayOfWeek.Saturday:
                                                SaturdaySchedule = tempSchedule.ToObservableCollection();
                                                break;
                                            case DayOfWeek.Sunday:
                                                SundaySchedule = tempSchedule.ToObservableCollection();
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        switch (date.DayOfWeek)
                                        {
                                            case DayOfWeek.Monday:
                                                MondaySchedule = new ObservableCollection<OvertimeWorkingSchedule>();
                                                break;
                                            case DayOfWeek.Tuesday:
                                                TuesdaySchedule = new ObservableCollection<OvertimeWorkingSchedule>();
                                                break;
                                            case DayOfWeek.Wednesday:
                                                WednesdaySchedule = new ObservableCollection<OvertimeWorkingSchedule>();
                                                break;
                                            case DayOfWeek.Thursday:
                                                ThursdaySchedule = new ObservableCollection<OvertimeWorkingSchedule>();
                                                break;
                                            case DayOfWeek.Friday:
                                                FridaySchedule = new ObservableCollection<OvertimeWorkingSchedule>();
                                                break;
                                            case DayOfWeek.Saturday:
                                                SaturdaySchedule = new ObservableCollection<OvertimeWorkingSchedule>();
                                                break;
                                            case DayOfWeek.Sunday:
                                                SundaySchedule = new ObservableCollection<OvertimeWorkingSchedule>();
                                                break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                InitNewSchedule();
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            t.Start();
        }
        private void LoadLocations()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetLocationForOvertimeWorkingWeek(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetLocationForOvertimeWorkingWeek(asyncResult);
                            if (results != null)
                            {
                                LocationCollection = results.ToObservableCollection();
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            t.Start();
        }

        private void LoadDoctorStaffCollection()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDoctorForOvertimeWorkingWeek(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDoctorForOvertimeWorkingWeek(asyncResult);
                            if (results != null)
                            {
                                DoctorStaffs = results.ToObservableCollection();
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            t.Start();
        }
        private void DeleteOvertimeWorkingSchedule(long OvertimeWorkingScheduleID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDeleteOvertimeWorkingSchedule(OvertimeWorkingScheduleID, Globals.LoggedUserAccount.Staff.StaffID, 
                        Globals.GetCurServerDateTime(), Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndDeleteOvertimeWorkingSchedule(asyncResult);
                            if (results)
                            {
                                LoadOvertimeWorkingScheduleByDate();
                                MessageBox.Show(eHCMSResources.A0478_G1_Msg_InfoXoaOK);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            t.Start();
        }
        private void SaveOvertimeWorkingSchedule()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSaveOvertimeWorkingSchedule(CurrentOvertimeWorkingWeek, null,
                        Globals.LoggedUserAccount.Staff.StaffID, Globals.GetCurServerDateTime(), Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndSaveOvertimeWorkingSchedule(asyncResult);
                                if (results)
                                {
                                    SetButtonByOvertimeWorkingWeekStatus();
                                    MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong);
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                }
            });
            t.Start();
        }
        private void UpdateOvertimeWorkingWeekStatus()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateOvertimeWorkingWeekStatus(CurrentOvertimeWorkingWeek, Globals.LoggedUserAccount.Staff.StaffID, 
                        Globals.GetCurServerDateTime(), Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndUpdateOvertimeWorkingWeekStatus(asyncResult);
                            if (results)
                            {
                                SetButtonByOvertimeWorkingWeekStatus();
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            t.Start();
        }
        #endregion
        #region Events
        [ImportingConstructor]
        public OvertimeWorkingScheduleViewModel(IWindsorContainer aContainer, INavigationService aNavigation, ISalePosCaching aCaching)
        {
            authorization();
            CurrentWeek = GetCurrentWeek(Globals.GetCurServerDateTime());
            InitCalendar();
            LoadLocations();
            LoadDoctorStaffCollection();
        }
        public void btnAddThu2()
        {
            AddDoctor(DayOfWeek.Monday, MondayDay);
        }
        public void btnAddThu3()
        {
            AddDoctor(DayOfWeek.Tuesday, TuesdayDay);
        }
        public void btnAddThu4()
        {
            AddDoctor(DayOfWeek.Wednesday, WednesdayDay);
        }
        public void btnAddThu5()
        {
            AddDoctor(DayOfWeek.Thursday, ThursdayDay);
        }
        public void btnAddThu6()
        {
            AddDoctor(DayOfWeek.Friday, FridayDay);
        }
        public void btnAddThu7()
        {
            AddDoctor(DayOfWeek.Saturday, SaturdayDay);
        }
        public void btnAddCN()
        {
            AddDoctor(DayOfWeek.Sunday, SundayDay);
        }
        public void RemoveItemCmd(object datacontext, object eventArgs)
        {
            if (datacontext == null || !(datacontext is OvertimeWorkingSchedule))
            {
                return;
            }
            if (CurrentOvertimeWorkingWeek != null
                && CurrentOvertimeWorkingWeek.OvertimeWorkingWeekID > 0
                && (CurrentOvertimeWorkingWeek.V_OvertimeWorkingWeekStatus == (long)AllLookupValues.V_OvertimeWorkingWeekStatus.Gui_Phong_TC
                || CurrentOvertimeWorkingWeek.V_OvertimeWorkingWeekStatus == (long)AllLookupValues.V_OvertimeWorkingWeekStatus.Xac_Nhan
                || CurrentOvertimeWorkingWeek.V_OvertimeWorkingWeekStatus == (long)AllLookupValues.V_OvertimeWorkingWeekStatus.Khoa))
            {
                MessageBox.Show("Lịch đã ban hành và Khóa. Vui lòng liên hệ Phòng Kế hoạch tổng hợp để được hỗ trợ!");
                return;
            }
            if (MessageBox.Show("Bạn có muốn xóa lịch làm việc này", eHCMSResources.T0074_G1_I, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                OvertimeWorkingSchedule temp = datacontext as OvertimeWorkingSchedule;
                DeleteOvertimeWorkingSchedule(temp.OvertimeWorkingScheduleID);
            }
        }
        public void EditItemCmd(object datacontext, object eventArgs)
        {
            if(datacontext == null || !(datacontext is OvertimeWorkingSchedule))
            {
                return;
            }
            if(CurrentOvertimeWorkingWeek != null 
                && CurrentOvertimeWorkingWeek.OvertimeWorkingWeekID >0 
                && (CurrentOvertimeWorkingWeek.V_OvertimeWorkingWeekStatus == (long)AllLookupValues.V_OvertimeWorkingWeekStatus.Gui_Phong_TC
                || CurrentOvertimeWorkingWeek.V_OvertimeWorkingWeekStatus == (long)AllLookupValues.V_OvertimeWorkingWeekStatus.Xac_Nhan
                || CurrentOvertimeWorkingWeek.V_OvertimeWorkingWeekStatus == (long)AllLookupValues.V_OvertimeWorkingWeekStatus.Khoa))
            {
                MessageBox.Show("Lịch đã ban hành và Khóa. Vui lòng liên hệ Phòng Kế hoạch tổng hợp để được hỗ trợ!");
                return;
            }
            OvertimeWorkingSchedule temp = datacontext as OvertimeWorkingSchedule;
            IOvertimeWorkingScheduleDetail aView = Globals.GetViewModel<IOvertimeWorkingScheduleDetail>();
            //aView.CurDayOfWeek = dayOfWeek;
            //aView.CurrentDate = CurrentDate;
            aView.CurrentOvertimeWorkingWeek = CurrentOvertimeWorkingWeek;
            aView.LocationCollection = LocationCollection;
            aView.DoctorStaffs = DoctorStaffs;
            aView.CurrentOvertimeWorkingSchedule = temp;
            aView.InitOvertimeWorkingSchedule();
            GlobalsNAV.ShowDialog_V3<IOvertimeWorkingScheduleDetail>(aView, null, null, false, false);
            LoadOvertimeWorkingScheduleByDate();
        }
        private void AddDoctor(DayOfWeek dayOfWeek, DateTime CurrentDate)
        {
            if (CurrentOvertimeWorkingWeek != null
               && CurrentOvertimeWorkingWeek.OvertimeWorkingWeekID > 0
               && (CurrentOvertimeWorkingWeek.V_OvertimeWorkingWeekStatus == (long)AllLookupValues.V_OvertimeWorkingWeekStatus.Gui_Phong_TC
               || CurrentOvertimeWorkingWeek.V_OvertimeWorkingWeekStatus == (long)AllLookupValues.V_OvertimeWorkingWeekStatus.Xac_Nhan
               || CurrentOvertimeWorkingWeek.V_OvertimeWorkingWeekStatus == (long)AllLookupValues.V_OvertimeWorkingWeekStatus.Khoa))
            {
                MessageBox.Show("Lịch đã ban hành và Khóa. Vui lòng liên hệ Phòng Kế hoạch tổng hợp để được hỗ trợ!");
                return;
            }
            IOvertimeWorkingScheduleDetail aView = Globals.GetViewModel<IOvertimeWorkingScheduleDetail>();
            aView.CurDayOfWeek = dayOfWeek;
            //aView.CurrentDate = CurrentDate;
            aView.CurrentOvertimeWorkingWeek = CurrentOvertimeWorkingWeek;
            aView.LocationCollection = LocationCollection;
            aView.DoctorStaffs = DoctorStaffs;
            aView.CurrentOvertimeWorkingSchedule = new OvertimeWorkingSchedule
            {
                OvertimeWorkingWeekID = CurrentOvertimeWorkingWeek.OvertimeWorkingWeekID,
                Week = CurrentWeek,
                WorkDate = CurrentDate,
                FromTime = Globals.GetCurServerDateTime(),
                ToTime = Globals.GetCurServerDateTime(),
                CreatedStaffID = Globals.LoggedUserAccount.Staff.StaffID,
                CreatedDate = Globals.GetCurServerDateTime(),
            };
            GlobalsNAV.ShowDialog_V3<IOvertimeWorkingScheduleDetail>(aView, null, null, false, false);
            LoadOvertimeWorkingWeekByDate();
        }
       
        public void cboYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime jan1 = new DateTime(CurrentYear, 1, 1);
            if (jan1.DayOfWeek == DayOfWeek.Monday)
            {
                WeekCollection = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25,
                    26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53 };
            }
            else
            {
                WeekCollection = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25,
                    26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52 };
            }
            if(CurrentWeek == 1)
            {
                InitCalendar();
            }
            else
            {
                CurrentWeek = 1;
            }
        }
        public void cboWeek_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitCalendar();
        }
        public void btnClearFilter()
        {
            //FilteredStaff = null;
            
            if(SelectedDate == null)
            {
                return;
            }
            if(SelectedDate.Year < DateTime.Now.Year - 1 || SelectedDate.Year > DateTime.Now.Year + 1)
            {
                return;
            }
            CurrentYear = SelectedDate.Year;
            CurrentYear = SelectedDate.Year;
            CurrentWeek = GetCurrentWeek(SelectedDate);
            //InitCalendar();
        }
        public void btnSave()
        {
            if (CurrentOvertimeWorkingWeek == null)
            {
                MessageBox.Show("Không tạo được lịch");
                return;
            }
            CurrentOvertimeWorkingWeek.V_OvertimeWorkingWeekStatus = (long)AllLookupValues.V_OvertimeWorkingWeekStatus.Luu;
            SaveOvertimeWorkingSchedule();
        }
        public void btnSendOrganizeRoom()
        {
            if (CurrentOvertimeWorkingWeek == null || CurrentOvertimeWorkingWeek.OvertimeWorkingWeekID == 0)
            {
                MessageBox.Show("Chưa lưu lịch trong tuần");
                return;
            }
            CurrentOvertimeWorkingWeek.V_OvertimeWorkingWeekStatus = (long)AllLookupValues.V_OvertimeWorkingWeekStatus.Gui_Phong_TC;
            UpdateOvertimeWorkingWeekStatus();
        }
        public void btnSendGeneralPlanRoom()
        {
            if (CurrentOvertimeWorkingWeek == null || CurrentOvertimeWorkingWeek.OvertimeWorkingWeekID == 0)
            {
                MessageBox.Show("Chưa lưu lịch trong tuần");
                return;
            }
            CurrentOvertimeWorkingWeek.V_OvertimeWorkingWeekStatus = (long)AllLookupValues.V_OvertimeWorkingWeekStatus.Gui_Phong_KHTH;
            UpdateOvertimeWorkingWeekStatus();
        }
        public void btnConfirm()
        {
            if (CurrentOvertimeWorkingWeek == null || CurrentOvertimeWorkingWeek.OvertimeWorkingWeekID == 0)
            {
                MessageBox.Show("Chưa lưu lịch trong tuần");
                return;
            }
            CurrentOvertimeWorkingWeek.V_OvertimeWorkingWeekStatus = (long)AllLookupValues.V_OvertimeWorkingWeekStatus.Xac_Nhan;
            UpdateOvertimeWorkingWeekStatus();
        }
        public void btnLock()
        {
            if (CurrentOvertimeWorkingWeek == null || CurrentOvertimeWorkingWeek.OvertimeWorkingWeekID == 0)
            {
                MessageBox.Show("Chưa lưu lịch trong tuần");
                return;
            }
            CurrentOvertimeWorkingWeek.V_OvertimeWorkingWeekStatus = (long)AllLookupValues.V_OvertimeWorkingWeekStatus.Khoa;
            UpdateOvertimeWorkingWeekStatus();
        }
        public void btnPreview()
        {
            if (CurrentOvertimeWorkingWeek == null
                || CurrentOvertimeWorkingWeek.OvertimeWorkingWeekID == 0)
            {
                return;
            }
            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
            {
                proAlloc.eItem = ReportName.XRpt_LichDangKyKhamNgoaiGio;
                proAlloc.ID = CurrentOvertimeWorkingWeek.OvertimeWorkingWeekID;

            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        }
        private void SetButtonByOvertimeWorkingWeekStatus()
        {
            if(CurrentOvertimeWorkingWeek == null 
                || CurrentOvertimeWorkingWeek.OvertimeWorkingWeekID == 0 
                || CurrentOvertimeWorkingWeek.V_OvertimeWorkingWeekStatus == 0)
            {
                btnSaveEnabled = false;
                btnSendOrganizeRoomEnabled = false;
                btnSendGeneralPlanRoomEnabled = false;
                btnConfirmEnabled = false;
                btnLockEnabled = false;
                return;
            }
            switch (CurrentOvertimeWorkingWeek.V_OvertimeWorkingWeekStatus)
            {
                case (long)AllLookupValues.V_OvertimeWorkingWeekStatus.Luu:
                    btnSaveEnabled = true;
                    btnSendOrganizeRoomEnabled = true;
                    btnSendGeneralPlanRoomEnabled = false;
                    btnConfirmEnabled = false;
                    btnLockEnabled = false;
                    break;
                case (long)AllLookupValues.V_OvertimeWorkingWeekStatus.Gui_Phong_TC:
                    btnSaveEnabled = false;
                    btnSendOrganizeRoomEnabled = false;
                    btnSendGeneralPlanRoomEnabled = true;
                    btnConfirmEnabled = true;
                    btnLockEnabled = false;
                    break;
                case (long)AllLookupValues.V_OvertimeWorkingWeekStatus.Gui_Phong_KHTH:
                    btnSaveEnabled = true;
                    btnSendOrganizeRoomEnabled = true;
                    btnSendGeneralPlanRoomEnabled = false;
                    btnConfirmEnabled = false;
                    btnLockEnabled = false;
                    break;
                case (long)AllLookupValues.V_OvertimeWorkingWeekStatus.Xac_Nhan:
                    btnSaveEnabled = false;
                    btnSendOrganizeRoomEnabled = false;
                    btnSendGeneralPlanRoomEnabled = true;
                    btnConfirmEnabled = false;
                    btnLockEnabled = true;
                    break;
                case (long)AllLookupValues.V_OvertimeWorkingWeekStatus.Khoa:
                    btnSaveEnabled = false;
                    btnSendOrganizeRoomEnabled = false;
                    btnSendGeneralPlanRoomEnabled = false;
                    btnConfirmEnabled = false;
                    btnLockEnabled = false;
                    break;
            }
        }
        #endregion

    }
}
