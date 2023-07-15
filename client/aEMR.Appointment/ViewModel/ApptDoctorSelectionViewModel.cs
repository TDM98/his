using aEMR.Common;
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using DataEntities;
using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;

namespace aEMR.Appointment.ViewModels
{
    [Export(typeof(IApptDoctorSelection)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ApptDoctorSelectionViewModel : ViewModelBase, IApptDoctorSelection
    {
        #region Properties
        private DateTime _CurrentDate;
        public DateTime CurrentDate
        {
            get
            {
                return _CurrentDate;
            }
            set
            {
                if (_CurrentDate == value)
                {
                    return;
                }
                _CurrentDate = value;
                NotifyOfPropertyChange(() => CurrentDate);
            }
        }
        public DeptLocation CurrentLocation { get; set; }
        public long CurrentMedServiceID { get; set; }
        private ObservableCollection<Staff> _DoctorStaffCollecion;
        public ObservableCollection<Staff> DoctorStaffCollecion
        {
            get
            {
                return _DoctorStaffCollecion;
            }
            set
            {
                if (_DoctorStaffCollecion == value)
                {
                    return;
                }
                _DoctorStaffCollecion = value;
                NotifyOfPropertyChange(() => DoctorStaffCollecion);
            }
        }
        ObservableCollection<CalendarWeek> _CalendarWeekCollection;
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
        private List<ConsultationRoomStaffAllocations> RoomStaffAllocationCollection;
        private ObservableCollection<ConsultationTimeSegments> _TimeSegmentCollection;
        public ObservableCollection<ConsultationTimeSegments> TimeSegmentCollection
        {
            get
            {
                return _TimeSegmentCollection;
            }
            set
            {
                if (_TimeSegmentCollection == value)
                {
                    return;
                }
                _TimeSegmentCollection = value;
                NotifyOfPropertyChange(() => TimeSegmentCollection);
            }
        }
        private Staff _SelectedStaff;
        public Staff SelectedStaff
        {
            get
            {
                return _SelectedStaff;
            }
            set
            {
                if (_SelectedStaff == value)
                {
                    return;
                }
                _SelectedStaff = value;
                NotifyOfPropertyChange(() => SelectedStaff);
            }
        }
        public DateTime? SelectedDate { get; set; }
        public DateTime? ConfirmedDate { get; set; }
        public ConsultationTimeSegments ConfirmedTimeSegment { get; set; }
        public Patient CurrentPatient { get; set; }
        public long CurrentAppointmentID { get; set; }
        public DateTime? EndDate { get; set; }
        public ConsultationRoomStaffAllocations ConfirmedAllocation { get; set; }
        #endregion
        #region Events
        [ImportingConstructor]
        public ApptDoctorSelectionViewModel()
        {
        }
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            InitCalendar();
        }
        public void cboStaff_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem == null)
            {
                return;
            }
            SelectedStaff = (sender as ComboBox).SelectedItem as Staff;
            foreach (var aWeek in CalendarWeekCollection)
            {
                foreach (var aDay in aWeek.CalendarDayCollection)
                {
                    aDay.IsOpened = false;
                }
            }
            TimeSegmentCollection = new ObservableCollection<ConsultationTimeSegments>();
            SelectedDate = null;
            EndDate = null;
            if (!RoomStaffAllocationCollection.Any(x => x.StaffID == SelectedStaff.StaffID))
            {
                return;
            }
            foreach (var aWeek in CalendarWeekCollection)
            {
                foreach (var aDay in aWeek.CalendarDayCollection)
                {
                    if (aDay.CalendarDate == null)
                    {
                        continue;
                    }
                    if (RoomStaffAllocationCollection.Any(x => x.StaffID == SelectedStaff.StaffID && x.AllocationDate.Date == aDay.CalendarDate.Value.Date))
                    {
                        aDay.IsOpened = true;
                    }
                }
            }
        }
        public void CalandarDay_Click(CalendarDay CurrentCalendarDay)
        {
            if (CurrentCalendarDay == null || CurrentCalendarDay.CalendarDate == null || !CurrentCalendarDay.IsOpened)
            {
                return;
            }
            SelectedDate = CurrentCalendarDay.CalendarDate.Value;
            TimeSegmentCollection = RoomStaffAllocationCollection.Where(x => x.StaffID == SelectedStaff.StaffID && x.AllocationDate.Date == CurrentCalendarDay.CalendarDate.Value.Date).Select(x => x.ConsultationTimeSegments).Distinct().ToObservableCollection();
            TimeSegmentCollection.First().IsChecked = true;
        }
        public void SaveButton()
        {
            if (SelectedStaff == null)
            {
                return;
            }
            if (SelectedDate != null && TimeSegmentCollection.Any(x => x.IsChecked))
            {
                ConfirmedDate = SelectedDate;
                ConfirmedTimeSegment = TimeSegmentCollection.First(x => x.IsChecked);
                ConfirmedAllocation = RoomStaffAllocationCollection.FirstOrDefault(x => x.StaffID == SelectedStaff.StaffID && x.AllocationDate.Date == SelectedDate.Value.Date && x.ConsultationTimeSegmentID == ConfirmedTimeSegment.ConsultationTimeSegmentID);
                if (ConfirmedTimeSegment != null && ConfirmedAllocation != null)
                {
                    CurrentLocation = ConfirmedAllocation.DeptLocation;
                }
                else
                {
                    CurrentLocation = null;
                }
            }
            TryClose();
        }
        public void SelectHourButton()
        {
            if (TimeSegmentCollection == null || !TimeSegmentCollection.Any(x => x.IsChecked) || SelectedDate == null)
            {
                return;
            }
            ICalendarDay DayView = Globals.GetViewModel<ICalendarDay>();
            var CurrentTimeSegment = TimeSegmentCollection.First(x => x.IsChecked);
            if (CurrentTimeSegment.StartTime != null && CurrentTimeSegment.EndTime != null)
            {
                DayView.CurrentValidDateTime = new DateTime[] { CurrentTimeSegment.StartTime, CurrentTimeSegment.EndTime };
            }
            var CurrentAllocation = RoomStaffAllocationCollection.FirstOrDefault(x => x.StaffID == SelectedStaff.StaffID && x.AllocationDate.Date == SelectedDate.Value.Date && x.ConsultationTimeSegmentID == CurrentTimeSegment.ConsultationTimeSegmentID);
            DayView.CurrentConsultationRoomStaffAllocID = CurrentAllocation == null ? 0 : CurrentAllocation.ConsultationRoomStaffAllocID;
            DayView.CurrentLocation = CurrentLocation;
            DayView.CurrentDate = SelectedDate.Value;
            DayView.CurrentPatient = CurrentPatient;
            DayView.CurrentAppointmentID = CurrentAppointmentID;
            DayView.OnDateChangedCallback = new OnDateChanged((aStartDate, aEndDate) =>
            {
                if (aEndDate.HasValue && aEndDate != null && aEndDate > new DateTime(2010, 1, 1))
                {
                    SelectedDate = aStartDate;
                    EndDate = aEndDate;
                }
            });
            GlobalsNAV.ShowDialog_V3(DayView);
            if (EndDate == null || !EndDate.HasValue || !DayView.IsConfirmed)
            {
                return;
            }
            if (CurrentDate != null && CurrentDate > new DateTime(2010, 1, 1) && SelectedDate.Value.Date != CurrentDate.Date)
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
            SaveButton();
        }
        public void PrevMonthButton()
        {
            CurrentDate = CurrentDate.AddMonths(-1);
            InitCalendar();
        }
        public void NextMonthButton()
        {
            CurrentDate = CurrentDate.AddMonths(1);
            InitCalendar();
        }
        #endregion
        #region Methods
        private void LoadAllContentSchedule()
        {
            this.DlgShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                using (var CurrentFactory = new ClinicManagementServiceClient())
                {
                    var CurrentContract = CurrentFactory.ServiceInstance;
                    CurrentContract.BeginGetStaffConsultationTimeSegmentByDate(0, new DateTime(CurrentDate.Year, CurrentDate.Month, 1)
                        , new DateTime(CurrentDate.Year, CurrentDate.Month, 1).AddMonths(1).AddSeconds(-1), Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var ItemCollection = CurrentContract.EndGetStaffConsultationTimeSegmentByDate(asyncResult);
                            if (ItemCollection != null && ItemCollection.Count > 0)
                            {
                                if (!ItemCollection.Any(x => x.ServiceItemCollection != null && x.ServiceItemCollection.Any(i => i.MedServiceID == CurrentMedServiceID)))
                                {
                                    ItemCollection = new List<ConsultationRoomStaffAllocations>();
                                }
                                else
                                {
                                    ItemCollection = ItemCollection.Where(x => x.ServiceItemCollection != null && x.ServiceItemCollection.Any(i => i.MedServiceID == CurrentMedServiceID)).ToList();
                                }
                            }
                            RoomStaffAllocationCollection = ItemCollection;
                            if (RoomStaffAllocationCollection != null && RoomStaffAllocationCollection.Count > 0 && Globals.AllStaffs.Any(x => RoomStaffAllocationCollection.Any(i => i.StaffID == x.StaffID)))
                            {
                                DoctorStaffCollecion = Globals.AllStaffs.Where(x => RoomStaffAllocationCollection.Any(i => i.StaffID == x.StaffID)).ToObservableCollection();
                            }
                            else
                            {
                                DoctorStaffCollecion = new ObservableCollection<Staff>();
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
        private void InitCalendar()
        {
            List<CalendarWeek> CurrentCalendarWeekCollection = new List<CalendarWeek> { new CalendarWeek() };
            int CurrentYear = CurrentDate.Year;
            int CurrentMonth = CurrentDate.Month;
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
            CalendarWeekCollection = CurrentCalendarWeekCollection.ToObservableCollection();
            LoadAllContentSchedule();
        }
        #endregion
    }
}