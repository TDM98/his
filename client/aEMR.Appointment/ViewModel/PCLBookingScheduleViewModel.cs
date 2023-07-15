using aEMR.Common.BaseModel;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ViewContracts;
using Castle.Windsor;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System;
using System.Collections.Generic;
using aEMR.Common.Collections;
using System.Threading;
using aEMR.ServiceClient;
using aEMR.Infrastructure;
using aEMR.Common;
using PCLsProxy;
using eHCMSLanguage;
using DataEntities;
using System.Diagnostics;

namespace aEMR.Appointment.ViewModels
{
    [Export(typeof(IPCLBookingSchedule)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLBookingScheduleViewModel : ViewModelBase, IPCLBookingSchedule
    {
        #region Properties
        private int[] _MonthCollection = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
        public int[] MonthCollection
        {
            get
            {
                return _MonthCollection;
            }
            set
            {
                _MonthCollection = value;
                NotifyOfPropertyChange(() => MonthCollection);
            }
        }
        private int _CurrentMonth = DateTime.Now.Month;
        public int CurrentMonth
        {
            get
            {
                return _CurrentMonth;
            }
            set
            {
                if (_CurrentMonth == value)
                {
                    return;
                }
                _CurrentMonth = value;
                NotifyOfPropertyChange(() => CurrentMonth);
            }
        }
        private int[] _YearCollection = new int[] { DateTime.Now.Year };
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
        private ObservableCollection<CalendarWeek> _CalendarWeekCollection;
        public ObservableCollection<CalendarWeek> CalendarWeekCollection
        {
            get
            {
                return _CalendarWeekCollection;
            }
            set
            {
                if (_CalendarWeekCollection == value)
                {
                    return;
                }
                _CalendarWeekCollection = value;
                NotifyOfPropertyChange(() => CalendarWeekCollection);
            }
        }
        public long PCLExamTypeID { get; set; }
        private bool _IsHasTarget = false;
        public bool IsHasTarget
        {
            get
            {
                return _IsHasTarget;
            }
            set
            {
                if (_IsHasTarget == value)
                {
                    return;
                }
                _IsHasTarget = value;
                NotifyOfPropertyChange(() => IsHasTarget);
            }
        }
        public bool IsHasSelected { get; set; } = false;
        public DateTime CurrentDate { get; set; }
        public DateTime OriginalDate { get; set; }
        private ObservableCollection<PCLExamType> _AllServiceHasTargetCollecion;
        public ObservableCollection<PCLExamType> AllServiceHasTargetCollecion
        {
            get
            {
                return _AllServiceHasTargetCollecion;
            }
            set
            {
                if (_AllServiceHasTargetCollecion == value)
                {
                    return;
                }
                _AllServiceHasTargetCollecion = value;
                NotifyOfPropertyChange(() => AllServiceHasTargetCollecion);
            }
        }
        #endregion
        #region Events
        [ImportingConstructor]
        public PCLBookingScheduleViewModel(IWindsorContainer aContainer, INavigationService aNavigation, ISalePosCaching aCaching)
        {
            if (Globals.ListPclExamTypesAllPCLFormImages != null && Globals.ListPclExamTypesAllPCLFormImages.Any(x => x.IsCasePermitted))
            {
                List<PCLExamType> PCLExamTypeCollection = new List<PCLExamType>();
                foreach (var aItem in Globals.ListPclExamTypesAllPCLFormImages.Where(x => x.IsCasePermitted))
                {
                    if (PCLExamTypeCollection.Any(x => x.PCLExamTypeID == aItem.PCLExamTypeID))
                    {
                        continue;
                    }
                    PCLExamTypeCollection.Add(aItem);
                }
                AllServiceHasTargetCollecion = PCLExamTypeCollection.ToObservableCollection();
            }
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            InitCalendar();
        }
        public void CalendarItem_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in CalendarWeekCollection.Where(x => x.CalendarDayCollection.Any(c => c.IsSelected)))
            {
                foreach (var child in item.CalendarDayCollection.Where(c => c.IsSelected))
                {
                    child.IsSelected = false;
                }
            }
            ((sender as Grid).DataContext as CalendarDay).IsSelected = true;
        }
        public void cboMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitCalendar();
        }
        public void gvScheduleStaffs_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is DataGrid))
            {
                return;
            }
            DataGrid CurrentDataGrid = sender as DataGrid;
            if (!(CurrentDataGrid.DataContext is CalendarDay))
            {
                return;
            }
            var CalendarDate = (CurrentDataGrid.DataContext as CalendarDay).CalendarDate;
            if (!CalendarDate.HasValue || CalendarDate.Value == null)
            {
                return;
            }
            if (OriginalDate != null && OriginalDate > new DateTime(2010, 1, 1) && CalendarDate.Value != OriginalDate)
            {
                IErrorBold MessBox = Globals.GetViewModel<IErrorBold>();
                MessBox.FireOncloseEvent = true;
                MessBox.isCheckBox = true;
                MessBox.SetMessage(eHCMSResources.Z2896_G1_ThayDoiNgayHenBenh, eHCMSResources.K3847_G1_DongY);
                GlobalsNAV.ShowDialog_V3(MessBox);
                if (!MessBox.IsAccept)
                {
                    return;
                }
            }
            CurrentDate = CalendarDate.Value;
            IsHasSelected = true;
            TryClose();
        }
        public void cboPCLExamType_DropDownClosed(object sender, System.EventArgs e)
        {
            InitCalendar();
        }
        #endregion
        #region Methods
        private void InitCalendar()
        {
            List<CalendarWeek> CurrentCalendarWeekCollection = new List<CalendarWeek> { new CalendarWeek() };
            int CurrentYear = DateTime.Now.Year;
            int CurrentWeek = 0;
            int CurrentDay = 0;
            int DaysOfMonth = DateTime.DaysInMonth(CurrentYear, CurrentMonth);
            for (int aDay = 1; aDay <= DaysOfMonth; aDay++)
            {
                DateTime CurrentDate = new DateTime(CurrentYear, CurrentMonth, aDay);
                if (CurrentWeek == 0 && aDay == 1)
                {
                    switch (CurrentDate.DayOfWeek)
                    {
                        case DayOfWeek.Tuesday:
                            CurrentDay = 1;
                            break;
                        case DayOfWeek.Wednesday:
                            CurrentDay = 2;
                            break;
                        case DayOfWeek.Thursday:
                            CurrentDay = 3;
                            break;
                        case DayOfWeek.Friday:
                            CurrentDay = 4;
                            break;
                        case DayOfWeek.Saturday:
                            CurrentDay = 5;
                            break;
                        case DayOfWeek.Sunday:
                            CurrentDay = 6;
                            break;
                    }
                    for (int i = 0; i < CurrentDay; i++)
                    {
                        CurrentCalendarWeekCollection[CurrentWeek].CalendarDayCollection[i].CalendarDate = new DateTime(CurrentYear, CurrentMonth, aDay).AddDays(-1 * (CurrentDay - i));
                        CurrentCalendarWeekCollection[CurrentWeek].CalendarDayCollection[i].IsExtendDate = true;
                    }
                }
                CurrentCalendarWeekCollection[CurrentWeek].CalendarDayCollection[CurrentDay].CalendarDate = new DateTime(CurrentYear, CurrentMonth, aDay);
                CurrentDay++;
                if (CurrentDay == 7)
                {
                    CurrentDay = 0;
                    CurrentWeek++;
                    CurrentCalendarWeekCollection.Add(new CalendarWeek());
                }
            }
            CurrentDay--;
            for (int i = CurrentDay + 1; i < 7; i++)
            {
                CurrentCalendarWeekCollection[CurrentWeek].CalendarDayCollection[i].CalendarDate = new DateTime(CurrentYear, CurrentMonth, DaysOfMonth).AddDays((i - CurrentDay));
                CurrentCalendarWeekCollection[CurrentWeek].CalendarDayCollection[i].IsExtendDate = true;
            }
            CalendarWeekCollection = CurrentCalendarWeekCollection.ToObservableCollection();
            if (CurrentDate >= new DateTime(2010, 01, 01))
            {
                foreach (var aWeek in CalendarWeekCollection)
                {
                    foreach(var aDay in aWeek.CalendarDayCollection)
                    {
                        if (!aDay.CalendarDate.HasValue || aDay.CalendarDate == null)
                        {
                            continue;
                        }
                        if (aDay.CalendarDate.Value.Date == CurrentDate.Date)
                        {
                            aDay.IsSelected = true;
                        }
                    }
                }
            }
            GetTargetFromPCLExamtypeID();
        }
        private void GetTargetFromPCLExamtypeID()
        {
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    IsHasTarget = false;
                    this.DlgShowBusyIndicator();
                    using (var serviceFactory = new PCLsClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLExamTypeServiceTarget_GetAll(PCLExamTypeID.ToString(), "", 0, 1, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int Total = 0;
                                var allItems = client.EndPCLExamTypeServiceTarget_GetAll(out Total, asyncResult);
                                if (allItems != null && allItems.Count > 0)
                                {
                                    IsHasTarget = true;
                                    var CurrentPCLTarget = allItems.First();
                                    DateTime Now = Globals.GetCurServerDateTime();
                                    foreach (var aWeek in CalendarWeekCollection)
                                    {
                                        foreach (var aDay in aWeek.CalendarDayCollection)
                                        {
                                            if (aDay.CalendarDate <= Now || aDay.CalendarDate == null)
                                            {
                                                continue;
                                            }
                                            switch (aDay.CalendarDate.Value.DayOfWeek)
                                            {
                                                case DayOfWeek.Monday:
                                                    aDay.TargetNumberOfCases = CurrentPCLTarget.MondayTargetNumberOfCases;
                                                    break;
                                                case DayOfWeek.Tuesday:
                                                    aDay.TargetNumberOfCases = CurrentPCLTarget.TuesdayTargetNumberOfCases;
                                                    break;
                                                case DayOfWeek.Wednesday:
                                                    aDay.TargetNumberOfCases = CurrentPCLTarget.WednesdayTargetNumberOfCases;
                                                    break;
                                                case DayOfWeek.Thursday:
                                                    aDay.TargetNumberOfCases = CurrentPCLTarget.ThursdayTargetNumberOfCases;
                                                    break;
                                                case DayOfWeek.Friday:
                                                    aDay.TargetNumberOfCases = CurrentPCLTarget.FridayTargetNumberOfCases;
                                                    break;
                                                case DayOfWeek.Saturday:
                                                    aDay.TargetNumberOfCases = CurrentPCLTarget.SaturdayTargetNumberOfCases;
                                                    break;
                                                case DayOfWeek.Sunday:
                                                    aDay.TargetNumberOfCases = CurrentPCLTarget.SundayTargetNumberOfCases;
                                                    break;
                                            }
                                        }
                                    }
                                    GetPCLExamTypeServiceApmtTarget();
                                }
                            }
                            catch (Exception e)
                            {
                                ClientLoggerHelper.LogInfo(e.ToString());
                                Globals.ShowMessage(e.Message, eHCMSLanguage.eHCMSResources.T0074_G1_I);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.DlgHideBusyIndicator();
                }
            });
            CurrentThread.Start();
        }
        private void GetPCLExamTypeServiceApmtTarget()
        {
            this.DlgShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    DateTime Now = Globals.GetCurServerDateTime();
                    contract.BeginGetPCLExamTypeServiceApmtTarget(PCLExamTypeID
                        , CalendarWeekCollection.SelectMany(x => x.CalendarDayCollection).Where(x => x.CalendarDate.HasValue && x.CalendarDate > new DateTime(2010, 1, 1)).Min(x => x.CalendarDate).Value
                        , CalendarWeekCollection.SelectMany(x => x.CalendarDayCollection).Where(x => x.CalendarDate.HasValue && x.CalendarDate > new DateTime(2010, 1, 1)).Max(x => x.CalendarDate).Value.AddDays(1).AddSeconds(-1), Globals.DispatchCallback((asyncResult) =>
                      {
                          try
                          {
                              var ItemCollection = contract.EndGetPCLExamTypeServiceApmtTarget(asyncResult);
                              if (ItemCollection != null && ItemCollection.Count > 0)
                              {
                                  foreach (var aWeek in CalendarWeekCollection)
                                  {
                                      foreach (var aDay in aWeek.CalendarDayCollection)
                                      {
                                          if (aDay.CalendarDate <= Now || aDay.CalendarDate == null || !ItemCollection.Any(x => x.AppointmentDate != null && x.AppointmentDate.Value.Date == aDay.CalendarDate.Value.Date))
                                          {
                                              continue;
                                          }
                                          aDay.QuantityOfProcessedCases = ItemCollection.Count(x => x.AppointmentDate != null && x.AppointmentDate.Value.Date == aDay.CalendarDate.Value.Date);
                                          var CurrentStaffWorkingScheduleCollection = ItemCollection.Where(x => x.AppointmentDate != null && x.AppointmentDate.Value.Date == aDay.CalendarDate.Value.Date).Select(x => new StaffWorkingSchedule { DoctorStaff = new DataEntities.Staff { FullName = x.PatientPCLRequest.FullName } });
                                          aDay.StaffWorkingSchedule = CurrentStaffWorkingScheduleCollection.ToObservableCollection();
                                      }
                                  }
                              }
                          }
                          catch (Exception ex)
                          {
                              MessageBox.Show(ex.Message);
                          }
                          finally
                          {
                              this.DlgHideBusyIndicator();
                          }
                      }), null);
                }
            });
            CurrentThread.Start();
        }
        #endregion
    }
}