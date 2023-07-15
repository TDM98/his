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
using aEMR.Infrastructure;
using aEMR.Common;
using eHCMSLanguage;
using DataEntities;

namespace aEMR.Appointment.ViewModels
{
    [Export(typeof(IServicesBookingSchedule)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ServicesBookingScheduleViewModel : ViewModelBase, IServicesBookingSchedule
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
        public DeptLocation CurrentLocation { get; set; }
        public Patient CurrentPatient { get; set; }
        public DateTime? EndDate { get; set; }
        public long CurrentAppointmentID { get; set; }
        #endregion
        #region Events
        [ImportingConstructor]
        public ServicesBookingScheduleViewModel(IWindsorContainer aContainer, INavigationService aNavigation, ISalePosCaching aCaching)
        {
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
            EndDate = null;
            ICalendarDay DayView = Globals.GetViewModel<ICalendarDay>();
            DayView.CurrentLocation = CurrentLocation;
            DayView.CurrentDate = CalendarDate.Value;
            DayView.CurrentPatient = CurrentPatient;
            DayView.CurrentAppointmentID = CurrentAppointmentID;
            DayView.OnDateChangedCallback = new OnDateChanged((aStartDate, aEndDate) =>
            {
                if (aEndDate.HasValue && aEndDate != null && aEndDate > new DateTime(2010, 1, 1))
                {
                    CalendarDate = aStartDate;
                    EndDate = aEndDate;
                }
            });
            GlobalsNAV.ShowDialog_V3(DayView);
            if (!EndDate.HasValue || EndDate == null)
            {
                return;
            }
            if (OriginalDate != null && OriginalDate > new DateTime(2010, 1, 1) && CalendarDate.Value.Date != OriginalDate.Date)
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
        }
        #endregion
    }
}