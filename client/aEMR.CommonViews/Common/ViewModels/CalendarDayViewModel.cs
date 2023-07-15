using aEMR.Common;
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.Common.HotKeyManagement;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.WPFControls;
using DataEntities;
using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace aEMR.CommonViews.ViewModels
{
    [Export(typeof(ICalendarDay)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CalendarDayViewModel : ViewModelBase, ICalendarDay
    {
        [ImportingConstructor]
        public CalendarDayViewModel()
        {
            base.HasInputBindingCmd = true;
        }
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
        public Appointments AppointmentCollection = new Appointments();
        public Patient CurrentPatient { get; set; }
        public OnDateChanged OnDateChangedCallback { get; set; }
        public DeptLocation CurrentLocation { get; set; }
        public long CurrentAppointmentID { get; set; }
        public long? CurrentConsultationRoomStaffAllocID { get; set; }
        private byte _CurrentViewCase = (byte)ViewCase.Normal;
        public byte CurrentViewCase
        {
            get
            {
                return _CurrentViewCase;
            }
            set
            {
                if (_CurrentViewCase == value)
                {
                    return;
                }
                _CurrentViewCase = value;
                NotifyOfPropertyChange(() => CurrentViewCase);
                NotifyOfPropertyChange(() => IsRegistrationView);
            }
        }
        public bool IsRegistrationView
        {
            get
            {
                return CurrentViewCase == (byte)ViewCase.Registration;
            }
        }
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
        private List<ConsultationRoomStaffAllocations> RoomStaffAllocationCollection;
        public long CurrentMedServiceID { get; set; }
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
        public bool IsConfirmed { get; set; } = false;
        private DateTime[] _CurrentValidDateTime;
        public DateTime[] CurrentValidDateTime
        {
            get
            {
                return _CurrentValidDateTime;
            }
            set
            {
                if (_CurrentValidDateTime == value)
                {
                    return;
                }
                _CurrentValidDateTime = value;
                NotifyOfPropertyChange(() => CurrentValidDateTime);
            }
        }
        #endregion
        #region Events
        public void Calendar_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            (sender as WPFControls.Calendar).Appointments = AppointmentCollection;
            if (!IsRegistrationView)
            {
                GetBookedAppointmentCollection(CurrentDate);
            }
        }
        public void Calendar_AddAppointment(object sender, RoutedEventArgs e)
        {
            CalendarTimeslotItem item = e.OriginalSource as CalendarTimeslotItem;
            if (item != null)
            {
                ICalendarDayBlockSelection DialogView = Globals.GetViewModel<ICalendarDayBlockSelection>();
                GlobalsNAV.ShowDialog_V3(DialogView);
                if (DialogView.BlockQty == 0)
                {
                    return;
                }
                aEMR.WPFControls.Appointment CurrentAppointment = new aEMR.WPFControls.Appointment { AppointmentID = CurrentAppointmentID };
                CurrentAppointment.StartTime = item.StartTime;
                CurrentAppointment.EndTime = item.StartTime + TimeSpan.FromMinutes(DialogView.BlockQty * 15);
                CurrentAppointment.Subject = CurrentPatient.FullName;
                if (AppointmentCollection.Any(x => x.AppointmentID != CurrentAppointment.AppointmentID && !(CurrentAppointment.EndTime <= x.StartTime || CurrentAppointment.StartTime >= x.EndTime)))
                {
                    Globals.ShowMessage("Khung giờ không hợp lệ!", eHCMSResources.G0442_G1_TBao);
                    return;
                }
                if (AppointmentCollection.Any(x => x.AppointmentID == CurrentAppointment.AppointmentID))
                {
                    AppointmentCollection.Remove(AppointmentCollection.First(x => x.AppointmentID == CurrentAppointment.AppointmentID));
                }
                AppointmentCollection.Add(CurrentAppointment);
                if (OnDateChangedCallback == null)
                {
                    return;
                }
                OnDateChangedCallback(CurrentAppointment.StartTime, CurrentAppointment.EndTime);
            }
        }
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            if (CurrentViewCase == (byte)ViewCase.Registration)
            {
                LoadAllContentSchedule();
            }
        }
        public void cboStaff_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsRegistrationView)
            {
                return;
            }
            SelectedStaff = (sender as ComboBox).SelectedItem as Staff;
            if (SelectedStaff == null)
            {
                return;
            }
            if (!RoomStaffAllocationCollection.Any(x => x.StaffID == SelectedStaff.StaffID))
            {
                TimeSegmentCollection = new ObservableCollection<ConsultationTimeSegments>();
                return;
            }
            TimeSegmentCollection = RoomStaffAllocationCollection.Where(x => x.StaffID == SelectedStaff.StaffID && x.AllocationDate.Date == CurrentDate.Date).Select(x => x.ConsultationTimeSegments).Distinct().ToObservableCollection();
            TimeSegmentCollection.First().IsChecked = true;
            GetBookedAppointmentCollection(CurrentDate);
        }
        public void RadioButtonTimeSegment_CheckedChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!IsRegistrationView)
            {
                return;
            }
            GetBookedAppointmentCollection(CurrentDate);
        }
        #endregion
        #region Methods
        private void GetBookedAppointmentCollection(DateTime ApptDate)
        {
            if (IsRegistrationView)
            {
                if (TimeSegmentCollection == null || !TimeSegmentCollection.Any(x => x.IsChecked))
                {
                    return;
                }
                var ConfirmedTimeSegment = TimeSegmentCollection.First(x => x.IsChecked);
                CurrentConsultationRoomStaffAllocID = RoomStaffAllocationCollection.FirstOrDefault(x => x.StaffID == SelectedStaff.StaffID && x.AllocationDate.Date == CurrentDate.Date && x.ConsultationTimeSegmentID == ConfirmedTimeSegment.ConsultationTimeSegmentID).ConsultationRoomStaffAllocID;
                if (ConfirmedTimeSegment.StartTime != null && ConfirmedTimeSegment.EndTime != null)
                {
                    CurrentValidDateTime = new DateTime[] { ConfirmedTimeSegment.StartTime, ConfirmedTimeSegment.EndTime };
                }
                else
                {
                    CurrentValidDateTime = null;
                }
            }
            AppointmentSearchCriteria SearchCriteria = new AppointmentSearchCriteria
            {
                DateFrom = ApptDate,
                DateTo = ApptDate,
                IsHasEndDate = true,
                DeptLocationID = CurrentLocation.DeptLocationID,
                ConsultationRoomStaffAllocID = CurrentConsultationRoomStaffAllocID == null ? 0 : CurrentConsultationRoomStaffAllocID.Value
            };
            this.DlgShowBusyIndicator(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.G1160_G1_TimCuocHen));
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var CurrentFactory = new AppointmentServiceClient())
                    {
                        var CurrentContract = CurrentFactory.ServiceInstance;
                        CurrentContract.BeginSearchAppointments(SearchCriteria, 0, Int16.MaxValue, false, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                int TotalCount = 0;
                                List<PatientAppointment> ItemCollection = CurrentContract.EndSearchAppointments(out TotalCount, asyncResult);
                                AppointmentCollection.Clear();
                                if (ItemCollection != null && ItemCollection.Count > 0)
                                {
                                    foreach (var aItem in ItemCollection.Where(x => x.ApptDate != null && x.EndDate != null).Select(x => new aEMR.WPFControls.Appointment
                                    {
                                        AppointmentID = x.AppointmentID,
                                        StartTime = x.ApptDate.Value,
                                        EndTime = x.EndDate.Value,
                                        Subject = x.Patient == null ? "" : x.Patient.FullName
                                    }))
                                    {
                                        AppointmentCollection.Add(aItem);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
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
        private void LoadAllContentSchedule()
        {
            this.DlgShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                using (var CurrentFactory = new ClinicManagementServiceClient())
                {
                    var CurrentContract = CurrentFactory.ServiceInstance;
                    CurrentContract.BeginGetStaffConsultationTimeSegmentByDate(0, CurrentDate.Date
                        , CurrentDate.Date.AddDays(1).AddSeconds(-1), Globals.DispatchCallback((asyncResult) =>
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
        #endregion
        #region KeyHandles
        public override void HandleHotKey_Action_New(object sender, LocalHotKeyEventArgs e)
        {
            foreach (var inputBindingCommand in ListInputBindingCmds)
            {
                if (inputBindingCommand.HotKey_Registered_Name == e.HotKey.Name)
                {
                    inputBindingCommand._executeDelegate.Invoke(this);
                    break;
                }
            }
        }
        protected override IEnumerable<InputBindingCommand> GetInputBindingCommands()
        {
            yield return new InputBindingCommand(() => { IsConfirmed = true; TryClose(); })
            {
                HotKey_Registered_Name = "SaveCalendarDay",
                GestureModifier = ModifierKeys.Control,
                GestureKey = (Key)Keys.S
            };
        }
        #endregion
        public enum ViewCase : byte
        {
            Normal = 0,
            Registration = 1
        }
    }
}