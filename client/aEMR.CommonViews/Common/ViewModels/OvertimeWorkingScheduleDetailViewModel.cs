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
    [Export(typeof(IOvertimeWorkingScheduleDetail)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class OvertimeWorkingScheduleDetailViewModel : ViewModelBase, IOvertimeWorkingScheduleDetail
    {
        [ImportingConstructor]
        public OvertimeWorkingScheduleDetailViewModel(IWindsorContainer aContainer, INavigationService aNavigation, ISalePosCaching aCaching)
        {
            //LoadLocations();
            //LoadDoctorStaffCollection();
            //FromTimeHour = CurrentOvertimeWorkingSchedule.FromTime.Hour;
            FromTimeHour = Globals.GetViewModel<IMinHourControl>();
            FromTimeHour.DateTime = Globals.GetCurServerDateTime().Date;
            ToTimeHour = Globals.GetViewModel<IMinHourControl>();
            ToTimeHour.DateTime = Globals.GetCurServerDateTime().Date;

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
        private Staff _gSelectedDoctorStaff;
        public Staff gSelectedDoctorStaff
        {
            get
            {
                return _gSelectedDoctorStaff;
            }
            set
            {
                if (_gSelectedDoctorStaff == value) return;
                _gSelectedDoctorStaff = value;
                NotifyOfPropertyChange(() => gSelectedDoctorStaff);
            }
        }
        private DayOfWeek _CurDayOfWeek;
        public DayOfWeek CurDayOfWeek
        {
            get
            {
                return _CurDayOfWeek;
            }
            set
            {
                if (_CurDayOfWeek == value) return;
                _CurDayOfWeek = value;
                NotifyOfPropertyChange(() => CurDayOfWeek);
            }
        }
        private DateTime _CurrentDate;
        public DateTime CurrentDate
        {
            get
            {
                return _CurrentDate;
            }
            set
            {
                if (_CurrentDate == value) return;
                _CurrentDate = value;
                NotifyOfPropertyChange(() => CurrentDate);
            }
        }
        private IMinHourControl _FromTimeHour;
        
        public IMinHourControl FromTimeHour
        {
            get
            {
                return _FromTimeHour;
            }
            set
            {
                if (_FromTimeHour == value)
                    return;
                _FromTimeHour = value;
                NotifyOfPropertyChange(() => FromTimeHour);
            }
        }
        private IMinHourControl _ToTimeHour;
        
        public IMinHourControl ToTimeHour
        {
            get
            {
                return _ToTimeHour;
            }
            set
            {
                if (_ToTimeHour == value)
                    return;
                _ToTimeHour = value;
                NotifyOfPropertyChange(() => ToTimeHour);
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
            }
        }
        private OvertimeWorkingSchedule _CurrentOvertimeWorkingSchedule;
        public OvertimeWorkingSchedule CurrentOvertimeWorkingSchedule
        {
            get
            {
                return _CurrentOvertimeWorkingSchedule;
            }
            set
            {
                if (_CurrentOvertimeWorkingSchedule == value)
                {
                    return;
                }
                _CurrentOvertimeWorkingSchedule = value;
                NotifyOfPropertyChange(() => CurrentOvertimeWorkingSchedule);
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
                {
                    return;
                }
                _CurrentOvertimeWorkingWeek = value;
                NotifyOfPropertyChange(() => CurrentOvertimeWorkingWeek);
            }
        }
       
        
        public void DoctorStaff_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            //if (Globals.ServerConfigSection.ConsultationElements.FilterDoctorByDeptResponsibilitiesInPt)
            //{
            //    string tempCurDeptID = SelectedInPatientDeptDetail != null && SelectedInPatientDeptDetail.DeptLocation != null ? SelectedInPatientDeptDetail.DeptLocation.DeptID.ToString() : "";
            //    DoctorStaffs = DoctorStaffs.Where(x => x.ListDeptResponsibilities != null && ((x.ListDeptResponsibilities.Contains(tempCurDeptID) || tempCurDeptID == ""))).ToObservableCollection();
            //}
            //if (Globals.ServerConfigSection.CommonItems.IsApplyTimeSegments)
            //{
            //    DoctorStaffs = DoctorStaffs.Where(x =>
            //                x.ConsultationTimeSegmentsList != null &&
            //                (x.ConsultationTimeSegmentsList.Where(y =>
            //                        y.StartTime.TimeOfDay < Globals.ServerDate.Value.TimeOfDay
            //                        && y.EndTime.TimeOfDay > Globals.ServerDate.Value.TimeOfDay).Count() > 0
            //                || x.ConsultationTimeSegmentsList.Where(y =>
            //                        y.EndTime2 != null
            //                        && y.StartTime2.Value.TimeOfDay < Globals.ServerDate.Value.TimeOfDay
            //                        && y.EndTime2.Value.TimeOfDay > Globals.ServerDate.Value.TimeOfDay).Count() > 0)).ToObservableCollection();
            //}
            var AllItemsContext = new ObservableCollection<Staff>(DoctorStaffs.Where(x => Globals.RemoveVietnameseString(x.FullName).ToLower().Contains(Globals.RemoveVietnameseString(cboContext.SearchText).ToLower())));
            cboContext.ItemsSource = AllItemsContext;
            cboContext.PopulateComplete();
        }
        public void DoctorStaff_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            gSelectedDoctorStaff = ((AutoCompleteBox)sender).SelectedItem as Staff;
            CurrentOvertimeWorkingSchedule.DoctorStaffID = gSelectedDoctorStaff == null ? 0 : gSelectedDoctorStaff.StaffID;
        }
        public void Location_Selected(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            CurrentOvertimeWorkingSchedule.DeptLocID = SelectedLocation == null ? 0 : SelectedLocation.DeptLocationID;
        }
        public void btnSave()
        {
            if(gSelectedDoctorStaff == null)
            {
                MessageBox.Show(eHCMSResources.A0376_G1_Msg_InfoChuaChonBS);
                return;
            }
            if(SelectedLocation == null)
            {
                MessageBox.Show(eHCMSResources.A0101_G1_Msg_InfoChuaChonPg);
                return;
            }
            if(FromTimeHour == null || ToTimeHour == null)
            {
                return;
            }
            CurrentOvertimeWorkingSchedule.FromTime = (DateTime)FromTimeHour.DateTime;
            CurrentOvertimeWorkingSchedule.ToTime = (DateTime)ToTimeHour.DateTime;
            if(CurrentOvertimeWorkingSchedule.FromTime >= CurrentOvertimeWorkingSchedule.ToTime)
            {
                MessageBox.Show("Giờ đến không được nhỏ hơn hoặc bằng giờ bắt đầu");
                return;
            }
            SaveOvertimeWorkingSchedule();
        }
        private void SaveOvertimeWorkingSchedule()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSaveOvertimeWorkingSchedule(CurrentOvertimeWorkingWeek, CurrentOvertimeWorkingSchedule, 
                        Globals.LoggedUserAccount.Staff.StaffID, Globals.GetCurServerDateTime(), Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndSaveOvertimeWorkingSchedule(asyncResult);
                            if (results)
                            {
                                MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                                TryClose();
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
        public void btnClose()
        {
            TryClose();
        }
        public void InitOvertimeWorkingSchedule()
        {
            if(CurrentOvertimeWorkingSchedule == null || CurrentOvertimeWorkingSchedule.OvertimeWorkingScheduleID == 0)
            {
                return;
            }
            gSelectedDoctorStaff = DoctorStaffs.Where(x => x.StaffID == CurrentOvertimeWorkingSchedule.DoctorStaffID).FirstOrDefault();
            SelectedLocation = LocationCollection.Where(x => x.DeptLocationID == CurrentOvertimeWorkingSchedule.DeptLocID).FirstOrDefault();
            FromTimeHour.DateTime = CurrentOvertimeWorkingSchedule.FromTime;
            ToTimeHour.DateTime = CurrentOvertimeWorkingSchedule.ToTime;
        }
    }
}
