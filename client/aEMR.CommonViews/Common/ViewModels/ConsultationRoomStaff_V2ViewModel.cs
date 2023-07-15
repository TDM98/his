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

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IConsultationRoomStaff_V2)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ConsultationRoomStaff_V2ViewModel : ViewModelBase, IConsultationRoomStaff_V2
    {
        private long StaffCatType = (long)V_StaffCatType.BacSi;
        public DateTime CurDateTime = DateTime.Now;
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
        private ObservableCollection<ConsultationTimeSegments> _lstConsultationTimeSegments;
        public ObservableCollection<ConsultationTimeSegments> lstConsultationTimeSegments
        {
            get
            {
                return _lstConsultationTimeSegments;
            }
            set
            {
                if (_lstConsultationTimeSegments == value)
                    return;
                _lstConsultationTimeSegments = value;
                NotifyOfPropertyChange(() => lstConsultationTimeSegments);
            }
        }
        private ObservableCollection<RefStaffCategory> _allRefStaffCategory;
        public ObservableCollection<RefStaffCategory> allRefStaffCategory
        {
            get
            {
                return _allRefStaffCategory;
            }
            set
            {
                if (_allRefStaffCategory == value)
                    return;
                _allRefStaffCategory = value;
                NotifyOfPropertyChange(() => allRefStaffCategory);
            }
        }
        private ObservableCollection<Staff> _allStaff;
        public ObservableCollection<Staff> allStaff
        {
            get
            {
                return _allStaff;
            }
            set
            {
                if (_allStaff == value)
                    return;
                _allStaff = value;
                NotifyOfPropertyChange(() => allStaff);
                StaffViewCollection = CollectionViewSource.GetDefaultView(allStaff);
                StaffViewCollection.Filter = (x) => { return string.IsNullOrEmpty(FilterDocterString) || (!string.IsNullOrEmpty((x as Staff).FullName) && Globals.RemoveVietnameseString((x as Staff).FullName).ToLower().Contains(Globals.RemoveVietnameseString(FilterDocterString).ToLower())); };
            }
        }
        private ICollectionView _StaffViewCollection;
        public ICollectionView StaffViewCollection
        {
            get
            {
                return _StaffViewCollection;
            }
            set
            {
                if (_StaffViewCollection == value)
                {
                    return;
                }
                _StaffViewCollection = value;
                NotifyOfPropertyChange(() => StaffViewCollection);
            }
        }
        private RefStaffCategory _SelectedRefStaffCategory;
        public RefStaffCategory SelectedRefStaffCategory
        {
            get
            {
                return _SelectedRefStaffCategory;
            }
            set
            {
                if (_SelectedRefStaffCategory == value)
                    return;
                _SelectedRefStaffCategory = value;
                NotifyOfPropertyChange(() => SelectedRefStaffCategory);
                if (SelectedRefStaffCategory != null)
                {
                    GetAllStaff(SelectedRefStaffCategory.StaffCatgID);
                }
            }
        }
        private ObservableCollection<ConsultationRoomStaffAllocations> _tempConsRoomStaffAlloc;
        public ObservableCollection<ConsultationRoomStaffAllocations> tempConsRoomStaffAlloc
        {
            get
            {
                return _tempConsRoomStaffAlloc;
            }
            set
            {
                if (_tempConsRoomStaffAlloc == value)
                    return;
                _tempConsRoomStaffAlloc = value;
            }
        }
        private ObservableCollection<ConsultationRoomStaffAllocations> _curAllConsulRoomStaffAlloc;
        public ObservableCollection<ConsultationRoomStaffAllocations> curAllConsulRoomStaffAlloc
        {
            get
            {
                return _curAllConsulRoomStaffAlloc;
            }
            set
            {
                if (_curAllConsulRoomStaffAlloc == value)
                    return;
                _curAllConsulRoomStaffAlloc = value;
                NotifyOfPropertyChange(() => curAllConsulRoomStaffAlloc);

            }
        }
        private ObservableCollection<ConsultationRoomStaffAllocations> _allConsulRoomStaffAlloc;
        public ObservableCollection<ConsultationRoomStaffAllocations> allConsulRoomStaffAlloc
        {
            get
            {
                return _allConsulRoomStaffAlloc;
            }
            set
            {
                if (_allConsulRoomStaffAlloc == value)
                    return;
                _allConsulRoomStaffAlloc = value;
                NotifyOfPropertyChange(() => allConsulRoomStaffAlloc);
                GetCurRoomStaffAllocations();
                NotifyOfPropertyChange(() => curAllConsulRoomStaffAlloc);
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
                    return;
                _SelectedStaff = value;
            }
        }
        private ConsultationTimeSegments _SelectedConsultationTimeSegments;
        public ConsultationTimeSegments SelectedConsultationTimeSegments
        {
            get
            {
                return _SelectedConsultationTimeSegments;
            }
            set
            {
                if (_SelectedConsultationTimeSegments == value)
                    return;
                _SelectedConsultationTimeSegments = value;
                NotifyOfPropertyChange(() => SelectedConsultationTimeSegments);
            }
        }
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
        private int[] _YearCollection = new int[] { DateTime.Now.Year, DateTime.Now.Year + 1 };
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
        private ObservableCollection<RefDepartment> _DepartmentCollection;
        public ObservableCollection<RefDepartment> DepartmentCollection
        {
            get
            {
                return _DepartmentCollection;
            }
            set
            {
                if (_DepartmentCollection == value)
                {
                    return;
                }
                _DepartmentCollection = value;
                NotifyOfPropertyChange(() => DepartmentCollection);
            }
        }
        private RefDepartment _SelectedDepartment;
        public RefDepartment SelectedDepartment
        {
            get
            {
                return _SelectedDepartment;
            }
            set
            {
                if (_SelectedDepartment == value)
                {
                    return;
                }
                _SelectedDepartment = value;
                NotifyOfPropertyChange(() => SelectedDepartment);
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
        private DeptLocation _SelectedLocation;
        public DeptLocation SelectedLocation
        {
            get
            {
                return _SelectedLocation;
            }
            set
            {
                if (_SelectedLocation == value)
                {
                    return;
                }
                _SelectedLocation = value;
                NotifyOfPropertyChange(() => SelectedLocation);
                //LoadAllContentSchedule();
            }
        }
        private ObservableCollection<Staff> _StaffCollection;
        public ObservableCollection<Staff> StaffCollection
        {
            get
            {
                return _StaffCollection;
            }
            set
            {
                if (_StaffCollection == value)
                {
                    return;
                }
                _StaffCollection = value;
                NotifyOfPropertyChange(() => StaffCollection);
            }
        }
        private Staff _FilteredStaff;
        public Staff FilteredStaff
        {
            get
            {
                return _FilteredStaff;
            }
            set
            {
                if (_FilteredStaff == value)
                {
                    return;
                }
                _FilteredStaff = value;
                NotifyOfPropertyChange(() => FilteredStaff);
                ChangedViewAfterFiltered();
            }
        }
        private bool _IsEditViewCase = true;
        public bool IsEditViewCase
        {
            get
            {
                return _IsEditViewCase;
            }
            set
            {
                if (_IsEditViewCase == value)
                {
                    return;
                }
                _IsEditViewCase = value;
                NotifyOfPropertyChange(() => IsEditViewCase);
            }
        }
        private bool _IsSuccessedCalled = false;
        public bool IsSuccessedCalled
        {
            get
            {
                return _IsSuccessedCalled;
            }
            set
            {
                if (_IsSuccessedCalled == value)
                {
                    return;
                }
                _IsSuccessedCalled = value;
                NotifyOfPropertyChange(() => IsSuccessedCalled);
            }
        }
        private long _SelectedTimeSegmentID;
        public long SelectedTimeSegmentID
        {
            get
            {
                return _SelectedTimeSegmentID;
            }
            set
            {
                if (_SelectedTimeSegmentID == value)
                {
                    return;
                }
                _SelectedTimeSegmentID = value;
                NotifyOfPropertyChange(() => SelectedTimeSegmentID);
            }
        }
        private DateTime _SelectedCalendarDate;
        public DateTime SelectedCalendarDate
        {
            get
            {
                return _SelectedCalendarDate;
            }
            set
            {
                if (_SelectedCalendarDate == value)
                {
                    return;
                }
                _SelectedCalendarDate = value;
                NotifyOfPropertyChange(() => SelectedCalendarDate);
            }
        }
        private ObservableCollection<Lookup> _SpecialistTypeCollection;
        public ObservableCollection<Lookup> SpecialistTypeCollection
        {
            get
            {
                return _SpecialistTypeCollection;
            }
            set
            {
                if (_SpecialistTypeCollection == value)
                {
                    return;
                }
                _SpecialistTypeCollection = value;
                NotifyOfPropertyChange(() => SpecialistTypeCollection);
            }
        }
        private long _V_SpecialistType;
        public long V_SpecialistType
        {
            get
            {
                return _V_SpecialistType;
            }
            set
            {
                if (_V_SpecialistType == value)
                {
                    return;
                }
                _V_SpecialistType = value;
                NotifyOfPropertyChange(() => V_SpecialistType);
            }
        }
        private ConsultationRoomStaffAllocationServiceList _ConfirmedServiceList;
        public ConsultationRoomStaffAllocationServiceList ConfirmedServiceList
        {
            get
            {
                return _ConfirmedServiceList;
            }
            set
            {
                if (_ConfirmedServiceList == value)
                {
                    return;
                }
                _ConfirmedServiceList = value;
                NotifyOfPropertyChange(() => ConfirmedServiceList);
                NotifyOfPropertyChange(() => SelectedServiceItemCollection);
                NotifyOfPropertyChange(() => SelectedServiceItemString);
            }
        }
        public IList<RefMedicalServiceItem> SelectedServiceItemCollection
        {
            get
            {
                return ConfirmedServiceList == null || ConfirmedServiceList.ServiceCollection == null ? null : ConfirmedServiceList.ServiceCollection.Select(x => x.MedicalService).ToObservableCollection();
            }
        }
        public string SelectedServiceItemString
        {
            get
            {
                return SelectedServiceItemCollection == null || SelectedServiceItemCollection.Count == 0 ? "" : string.Join(",", SelectedServiceItemCollection.Select(x => x.MedServiceName));
            }
        }
        private void InitCalendar()
        {
            List<CalendarWeek> CurrentCalendarWeekCollection = new List<CalendarWeek> { new CalendarWeek() };
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
        private void GetAllConsultationTimeSegments(GenericCoRoutineTask genTask)
        {
            var t = new Thread(() =>
            {
                this.ShowBusyIndicator();
                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllConsultationTimeSegments(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetAllConsultationTimeSegments(asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                lstConsultationTimeSegments = new ObservableCollection<ConsultationTimeSegments>();
                                foreach (var consTimeSeg in results)
                                {
                                    lstConsultationTimeSegments.Add(consTimeSeg);
                                }
                                SelectedConsultationTimeSegments = lstConsultationTimeSegments.FirstOrDefault();
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                            genTask.ActionComplete(true);
                        }
                    }), null);
                }
            });
            t.Start();
        }
        private void GetRefStaffCategories()
        {
            allRefStaffCategory = new ObservableCollection<RefStaffCategory>();
            GetRefStaffCategoriesByType((long)V_StaffCatType.BacSi);
            GetRefStaffCategoriesByType((long)V_StaffCatType.PhuTa);
        }
        private void GetRefStaffCategoriesByType(long V_StaffCatType)
        {
            var t = new Thread(() =>
            {
                this.ShowBusyIndicator();
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetRefStaffCategoriesByType(V_StaffCatType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetRefStaffCategoriesByType(asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                foreach (var p in results)
                                {
                                    allRefStaffCategory.Add(p);
                                }
                                SelectedRefStaffCategory = allRefStaffCategory.FirstOrDefault();
                            }
                            else
                            {
                                allRefStaffCategory.Clear();
                                SelectedRefStaffCategory = null;
                            }
                            NotifyOfPropertyChange(() => allRefStaffCategory);
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

        private void GetAllStaff(long StaffCatgID)
        {
            var t = new Thread(() =>
            {
                this.ShowBusyIndicator();
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllStaff(StaffCatgID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetAllStaff(asyncResult);
                            if (allStaff == null)
                            {
                                allStaff = new ObservableCollection<Staff>();
                            }
                            if (results != null && results.Count > 0)
                            {
                                allStaff = results.ToObservableCollection();
                            }
                            else
                            {
                                allStaff.Clear();
                            }
                            NotifyOfPropertyChange(() => allStaff);
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

        private void GetAllStaffCollection()
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllStaff(0, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetAllStaff(asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                StaffCollection = results.ToObservableCollection().Where(x => x.RefStaffCategory != null
                                         && (x.RefStaffCategory.V_StaffCatType == (long)V_StaffCatType.BacSi)).ToObservableCollection();
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

        private ObservableCollection<ConsultationRoomStaffAllocations> GetRoomStaffAlloByType(long StaffCatType)
        {
            ObservableCollection<ConsultationRoomStaffAllocations> tempConsRoom = new ObservableCollection<ConsultationRoomStaffAllocations>();
            if (allConsulRoomStaffAlloc != null)
            {
                foreach (var RoomStaffAllocation in allConsulRoomStaffAlloc)
                {
                    if (RoomStaffAllocation.Staff.RefStaffCategory.V_StaffCatType == StaffCatType)
                    {
                        tempConsRoom.Add(RoomStaffAllocation);
                    }
                }
            }
            return tempConsRoom;
        }

        private void GetCurRoomStaffAllocations()
        {
            curAllConsulRoomStaffAlloc = new ObservableCollection<ConsultationRoomStaffAllocations>();
            foreach (var cts in lstConsultationTimeSegments)
            {
                if (cts.ConsultationTimeSegmentID < 1)
                    continue;
                bool flag = false;
                if (allConsulRoomStaffAlloc == null ||
                    allConsulRoomStaffAlloc.Count < 1)
                {
                    return;
                }
                foreach (var crt in allConsulRoomStaffAlloc)
                {
                    if (crt.ConsultationTimeSegmentID == cts.ConsultationTimeSegmentID
                        && crt.AllocationDate.Date <= CurDateTime.Date
                        && crt.Staff.RefStaffCategory.V_StaffCatType == StaffCatType)
                    {
                        crt.Status = eHCMSResources.G2355_G1_X.ToUpper();
                        flag = true;
                        curAllConsulRoomStaffAlloc.Add(ObjectCopier.DeepCopy(crt));
                        break;
                    }
                }
                if (!flag)
                {
                    ConsultationRoomStaffAllocations crt = new ConsultationRoomStaffAllocations();
                    crt.ConsultationTimeSegments = new ConsultationTimeSegments();
                    crt.ConsultationTimeSegments.SegmentName = cts.SegmentName;
                    curAllConsulRoomStaffAlloc.Add(ObjectCopier.DeepCopy(crt));
                }
            }
            NotifyOfPropertyChange(() => curAllConsulRoomStaffAlloc);
        }

        private IEnumerator<IResult> LoadDepartments()
        {
            this.ShowBusyIndicator();
            var GetDepartmentTask = new LoadDepartmentsByV_DeptTypeOperationTask(true);
            yield return GetDepartmentTask;
            if (GetDepartmentTask.Departments != null
                && GetDepartmentTask.Departments.Count > 0)
            {
                DepartmentCollection = GetDepartmentTask.Departments.ToObservableCollection();
                if (DepartmentCollection.Count > 0 && DepartmentCollection.Any(x => x.V_DeptTypeOperation == (long)V_DeptTypeOperation.KhoaNgoaiTru))
                {
                    SelectedDepartment = DepartmentCollection.First(x => x.V_DeptTypeOperation == (long)V_DeptTypeOperation.KhoaNgoaiTru);
                }
                else
                {
                    SelectedDepartment = DepartmentCollection.FirstOrDefault();
                }
                if (SelectedDepartment != null && SelectedDepartment.DeptID > 0)
                {
                    LoadLocations(SelectedDepartment.DeptID);
                }
                NotifyOfPropertyChange(() => DepartmentCollection);
            }
            else
            {
                DepartmentCollection = new ObservableCollection<RefDepartment>();
                SelectedDepartment = null;
            }
            this.HideBusyIndicator();
            yield break;
        }
        private void LoadLocations(long aDeptID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllLocationsByDeptIDOld(aDeptID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var allItems = contract.EndGetAllLocationsByDeptIDOld(asyncResult);
                            ObservableCollection<DeptLocation> ItemCollection = new ObservableCollection<DeptLocation>();
                            if (allItems != null)
                            {
                                ItemCollection = allItems.ToObservableCollection();
                            }
                            ItemCollection.Insert(0, new DeptLocation { DeptLocationID = 0, Location = new Location { LocationName = string.Empty } });
                            if (SelectedLocation != null)
                            {
                                var BackupLocation = SelectedLocation.DeepCopy();
                                LocationCollection = ItemCollection;
                                //SelectedLocation = BackupLocation;
                                SelectedLocation = LocationCollection.FirstOrDefault();
                            }
                            else
                            {
                                LocationCollection = ItemCollection;
                                SelectedLocation = LocationCollection.FirstOrDefault();
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
        private string ConvertCalendarToXml(IList<CalendarWeek> aCalendarWeekCollection)
        {
            if (!aCalendarWeekCollection.Any(x => x.IsHasValue && x.CalendarDayCollection.Any(c => c.IsHasValue)))
            {
                return null;
            }
            List<CalendarDay> AllCalendarDay = new List<CalendarDay>();
            foreach (var aDay in aCalendarWeekCollection.Where(x => x.IsHasValue && x.CalendarDayCollection.Any(c => c.IsHasValue)).SelectMany(x => x.CalendarDayCollection).Where(x => x.IsHasValue).ToList())
            {
                if (aDay.StaffWorkingSchedule == null || aDay.StaffWorkingSchedule.Count == 0)
                {
                    continue;
                }
                AllCalendarDay.Add(aDay);
            }
            XDocument mDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement("Calendar", AllCalendarDay.Select(aItem => new XElement("item", new XElement[] {
                    new XElement("CalendarDate", aItem.CalendarDate == null ? "" : aItem.CalendarDate.Value.ToString("yyyy-MM-dd")),
                    new XElement("WorkingSchedule", aItem.StaffWorkingSchedule.Select(aSchedule => new XElement("item", new XElement[] {
                        new XElement("SegmentID", aSchedule.ConsultationSegment.ConsultationTimeSegmentID),
                        new XElement("DeptID", aSchedule.CurrentDepartment == null ? (long?)null : aSchedule.CurrentDepartment.DeptID),
                        new XElement("DeptLocationID", aSchedule.CurrentDeptLocation == null ? (long?)null : aSchedule.CurrentDeptLocation.DeptLocationID),
                        new XElement("StaffID", aSchedule.DoctorStaff.StaffID),
                        new XElement("ConsultationRoomStaffAllocationServiceListID", aSchedule.ConsultationRoomStaffAllocationServiceListID),
                        new XElement("MedServiceItems", aSchedule.ServiceItemCollection == null ? null : aSchedule.ServiceItemCollection.Select(x => new XElement("Item", new XElement[] { new XElement("MedServiceID", x.MedServiceID) })).ToArray())
                    })))
                })).ToArray()));
            return mDocument.ToString();
        }
        private void LoadAllContentSchedule()
        {
            if (CalendarWeekCollection == null
                || !CalendarWeekCollection.Any(x => x.CalendarDayCollection != null && x.CalendarDayCollection.Any(c => c.CalendarDate.HasValue && c.CalendarDate != null)))
            {
                return;
            }
            foreach (var aWeek in CalendarWeekCollection)
            {
                if (!aWeek.CalendarDayCollection.Any(x => x.StaffWorkingSchedule != null))
                {
                    continue;
                }
                foreach (var aDay in aWeek.CalendarDayCollection.Where(x => x.StaffWorkingSchedule != null))
                {
                    aDay.StaffWorkingSchedule.Clear();
                }
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetStaffConsultationTimeSegmentByDate(0, CalendarWeekCollection.SelectMany(x => x.CalendarDayCollection).Where(c => c.CalendarDate.HasValue && c.CalendarDate != null).Min(c => c.CalendarDate.Value)
                        , CalendarWeekCollection.SelectMany(x => x.CalendarDayCollection).Where(c => c.CalendarDate.HasValue && c.CalendarDate != null).Max(c => c.CalendarDate.Value), Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetStaffConsultationTimeSegmentByDate(asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                foreach (var aWeek in CalendarWeekCollection)
                                {
                                    foreach (var aDay in aWeek.CalendarDayCollection)
                                    {
                                        if (results.Any(x => x.AllocationDate == aDay.CalendarDate))
                                        {
                                            aDay.StaffWorkingSchedule = results.Where(x => x.AllocationDate == aDay.CalendarDate).Select(x => new StaffWorkingSchedule { ConsultationRoomStaffAllocationServiceListID = x.ConsultationRoomStaffAllocationServiceListID, ServiceItemCollection = x.ServiceItemCollection, CurrentDeptLocation = x.DeptLocation != null && x.DeptLocation.DeptLocationID == 0 ? null : x.DeptLocation, ConsultationSegment = new ConsultationTimeSegments { ConsultationTimeSegmentID = x.ConsultationTimeSegmentID, SegmentName = x.ConsultationTimeSegments.SegmentName }, DoctorStaff = x.Staff }).ToObservableCollection();
                                            if (aDay.StaffWorkingScheduleView.Filter == null)
                                            {
                                                aDay.StaffWorkingScheduleView.Filter = CustomerFilter;
                                            }
                                        }
                                    }
                                }
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
        private bool CustomerFilter(object aSchedule)
        {
            if (FilteredStaff == null || !(aSchedule is StaffWorkingSchedule))
            {
                return true;
            }
            StaffWorkingSchedule mSchedule = aSchedule as StaffWorkingSchedule;
            return mSchedule.DoctorStaff != null && mSchedule.DoctorStaff.StaffID == FilteredStaff.StaffID;
        }
        private void ChangedViewAfterFiltered()
        {
            if (CalendarWeekCollection == null)
            {
                return;
            }
            if (!CalendarWeekCollection.Any(x => x.CalendarDayCollection != null && x.CalendarDayCollection.Length > 0))
            {
                return;
            }
            foreach (var cWeek in CalendarWeekCollection.Where(x => x.CalendarDayCollection != null && x.CalendarDayCollection.Length > 0))
            {
                foreach (var cDay in cWeek.CalendarDayCollection)
                {
                    if (cDay.StaffWorkingSchedule == null || cDay.StaffWorkingSchedule.Count == 0)
                    {
                        continue;
                    }
                    cDay.StaffWorkingScheduleView.Refresh();
                }
            }
        }
        public void ScheduleStaffs_DoubleClick(DateTime aCalendarDate, long aConsultationTimeSegmentID)
        {
            IsSuccessedCalled = true;
            SelectedCalendarDate = aCalendarDate;
            SelectedTimeSegmentID = aConsultationTimeSegmentID;
            TryClose();
        }
        #region Events
        public ConsultationRoomStaff_V2ViewModel()
        {
            SpecialistTypeCollection = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_SpecialistType).ToObservableCollection();
            V_SpecialistType = SpecialistTypeCollection.First().LookupID;
            Coroutine.BeginExecute(GetData_Routine(), null, (o, e) =>
            {
                radBacSi_Click(null, null);
            });
            InitCalendar();
            Coroutine.BeginExecute(LoadDepartments());
            GetAllStaffCollection();
        }
        private IEnumerator<IResult> GetData_Routine()
        {
            yield return GenericCoRoutineTask.StartTask(GetAllConsultationTimeSegments);
        }
        public void radBacSi_Click(object sender, RoutedEventArgs e)
        {
            StaffCatType = (long)V_StaffCatType.BacSi;
            allRefStaffCategory = new ObservableCollection<RefStaffCategory>();
            allRefStaffCategory.Clear();
            GetRefStaffCategoriesByType(StaffCatType);
            allStaff = new ObservableCollection<Staff>();
            tempConsRoomStaffAlloc = GetRoomStaffAlloByType(StaffCatType);
            NotifyOfPropertyChange(() => tempConsRoomStaffAlloc);
            GetCurRoomStaffAllocations();
            NotifyOfPropertyChange(() => curAllConsulRoomStaffAlloc);
        }
        public void radTroLy_Click(object sender, RoutedEventArgs e)
        {
            StaffCatType = (long)V_StaffCatType.PhuTa;
            allRefStaffCategory = new ObservableCollection<RefStaffCategory>();
            allRefStaffCategory.Clear();
            GetRefStaffCategoriesByType(StaffCatType);
            allStaff = new ObservableCollection<Staff>();
            tempConsRoomStaffAlloc = GetRoomStaffAlloByType(StaffCatType);
            NotifyOfPropertyChange(() => tempConsRoomStaffAlloc);
            GetCurRoomStaffAllocations();
            NotifyOfPropertyChange(() => curAllConsulRoomStaffAlloc);
        }
        public void grdListStaffDoubleClick(object sender)
        {
            //if (SelectedServiceItemCollection == null || SelectedServiceItemCollection.Count == 0 || ConfirmedServiceList == null)
            //{
            //    Globals.ShowMessage(eHCMSResources.Z0162_G1_HayChonDV, eHCMSResources.G0442_G1_TBao);
            //    return;
            //}
            if (SelectedConsultationTimeSegments == null
                || SelectedConsultationTimeSegments.ConsultationTimeSegmentID == 0)
            {
                return;
            }
            if (!CalendarWeekCollection.Any(x => x.CalendarDayCollection.Any(c => c.IsSelected)))
            {
                return;
            }
            foreach (var cWeek in CalendarWeekCollection.Where(x => x.CalendarDayCollection.Any(c => c.IsSelected)))
            {
                foreach (var cDay in cWeek.CalendarDayCollection.Where(c => c.IsSelected))
                {
                    if (cDay.StaffWorkingSchedule == null)
                    {
                        cDay.StaffWorkingSchedule = new ObservableCollection<StaffWorkingSchedule>();
                        cDay.StaffWorkingScheduleView.Filter = CustomerFilter;
                    }
                    if (cDay.StaffWorkingSchedule.Any(x => x.ConsultationSegment != null && x.ConsultationSegment.ConsultationTimeSegmentID == SelectedConsultationTimeSegments.ConsultationTimeSegmentID && x.DoctorStaff != null && x.DoctorStaff.StaffID == SelectedStaff.StaffID))
                    {
                        continue;
                    }
                    cDay.StaffWorkingSchedule.Add(new StaffWorkingSchedule { //ConsultationRoomStaffAllocationServiceListID = ConfirmedServiceList.ConsultationRoomStaffAllocationServiceListID,
                        CurrentDeptLocation = SelectedLocation != null && SelectedLocation.LID == 0 ? null : SelectedLocation.DeepCopy(),
                        CurrentDepartment = SelectedDepartment != null && SelectedDepartment.DeptID == 0 ? null : SelectedDepartment.DeepCopy(),
                        ConsultationSegment = new ConsultationTimeSegments { ConsultationTimeSegmentID = SelectedConsultationTimeSegments.ConsultationTimeSegmentID, SegmentName = SelectedConsultationTimeSegments.SegmentName }, DoctorStaff = SelectedStaff });
                    cDay.StaffWorkingScheduleView.Refresh();
                }
            }
        }
        public void CalendarItem_Click(object sender, RoutedEventArgs e)
        {
            if (!IsEditViewCase)
            {
                return;
            }
            if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                foreach (var item in CalendarWeekCollection.Where(x => x.CalendarDayCollection.Any(c => c.IsSelected)))
                {
                    foreach (var child in item.CalendarDayCollection.Where(c => c.IsSelected))
                    {
                        child.IsSelected = false;
                    }
                }
            }
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {

            }

                ((sender as Grid).DataContext as CalendarDay).IsSelected = !((sender as Grid).DataContext as CalendarDay).IsSelected;
        }
        public void cboMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitCalendar();
        }
        public void cboDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedDepartment == null || SelectedDepartment.DeptID == 0)
            {
                LocationCollection = new ObservableCollection<DeptLocation>();
                return;
            }
            LoadLocations(SelectedDepartment.DeptID);
        }
        public void btnSave()
        {
            var XmlString = ConvertCalendarToXml(CalendarWeekCollection);
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginEditStaffConsultationTimeSegments(XmlString, 0, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            contract.EndEditStaffConsultationTimeSegments(asyncResult);
                            NotifyOfPropertyChange(() => allRefStaffCategory);
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
        public void RemoveItemCmd(object sender, RoutedEventArgs e)
        {
            var DataGrid = (sender as Button).FindParentOfType<DataGrid>();
            if (DataGrid != null && DataGrid.DataContext is CalendarDay)
            {
                if ((sender as Button).DataContext == null && !((sender as Button).DataContext is StaffWorkingSchedule))
                {
                    return;
                }
                (DataGrid.DataContext as CalendarDay).StaffWorkingSchedule.Remove((sender as Button).DataContext as StaffWorkingSchedule);
            }
        }
        public void EditServicesOfItemCmd(object sender, RoutedEventArgs e)
        {
            var DataGrid = (sender as Button).FindParentOfType<DataGrid>();
            if (DataGrid != null && DataGrid.DataContext is CalendarDay)
            {
                if ((sender as Button).DataContext == null && !((sender as Button).DataContext is StaffWorkingSchedule))
                {
                    return;
                }
                IUCServiceListsSelection DialogView = Globals.GetViewModel<IUCServiceListsSelection>();
                if (((sender as Button).DataContext as StaffWorkingSchedule).ConsultationRoomStaffAllocationServiceListID > 0)
                {
                    DialogView.ServiceItemCollectionLoadCompletedCallback = new ServiceItemCollectionLoadCompleted(() =>
                    {
                        DialogView.ApplySelectedServiceCollection(((sender as Button).DataContext as StaffWorkingSchedule).ConsultationRoomStaffAllocationServiceListID);
                    });
                }
                GlobalsNAV.ShowDialog_V3(DialogView);
                if (DialogView.IsConfirmed)
                {
                    ((sender as Button).DataContext as StaffWorkingSchedule).ConsultationRoomStaffAllocationServiceListID = DialogView.ConfirmedServiceList.ConsultationRoomStaffAllocationServiceListID;
                }
            }
        }
        public void btnClearFilter()
        {
            FilteredStaff = null;
        }
        public void gvScheduleStaffs_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsEditViewCase)
            {
                return;
            }
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
            if (CurrentDataGrid.SelectedItem == null || !(CurrentDataGrid.SelectedItem is StaffWorkingSchedule))
            {
                return;
            }
            if ((CurrentDataGrid.SelectedItem as StaffWorkingSchedule).ConsultationSegment == null)
            {
                return;
            }
            var ConsultationTimeSegmentID = (CurrentDataGrid.SelectedItem as StaffWorkingSchedule).ConsultationSegment.ConsultationTimeSegmentID;
            ScheduleStaffs_DoubleClick(CalendarDate.Value, ConsultationTimeSegmentID);
        }
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            if (SelectedCalendarDate != null && SelectedCalendarDate > new DateTime(2010, 1, 1))
            {
                CurrentYear = SelectedCalendarDate.Year;
                CurrentMonth = SelectedCalendarDate.Month;
                cboMonth_SelectionChanged(null, null);
            }
        }
        private string _FilterDocterString;
        public string FilterDocterString
        {
            get
            {
                return _FilterDocterString;
            }
            set
            {
                if (_FilterDocterString == value)
                {
                    return;
                }
                _FilterDocterString = value;
                NotifyOfPropertyChange(() => FilterDocterString);
                if (StaffViewCollection == null)
                {
                    return;
                }
                StaffViewCollection.Refresh();
            }
        }
        public void txtDocterFiler_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
        public void SelectServicesCmd()
        {
            IUCServiceListsSelection DialogView = Globals.GetViewModel<IUCServiceListsSelection>();
            GlobalsNAV.ShowDialog_V3(DialogView);
            if (!DialogView.IsConfirmed)
            {
                return;
            }
            ConfirmedServiceList = DialogView.ConfirmedServiceList;
        }
        #endregion
        private DateTime _FromDate;
        public DateTime FromDate
        {
            get
            {
                return _FromDate;
            }
            set
            {
                if (_FromDate == value)
                {
                    return;
                }
                _FromDate = value;
                NotifyOfPropertyChange(() => FromDate);
            }
        }
        private DateTime _ToDate;
        public DateTime ToDate
        {
            get
            {
                return _ToDate;
            }
            set
            {
                if (_ToDate == value)
                {
                    return;
                }
                _ToDate = value;
                NotifyOfPropertyChange(() => ToDate);
            }
        }
        public void btnSelectDay()
        {
            foreach (var item in CalendarWeekCollection)
            {
                foreach (var child in item.CalendarDayCollection.Where(c => c.CalendarDate<=ToDate && c.CalendarDate>=FromDate))
                {
                    child.IsSelected = true;
                }
            }
        }
    }
}
