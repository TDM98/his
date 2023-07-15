using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using aEMR.Common;
using System.Linq;
using System.Collections.Generic;
using aEMR.CommonTasks;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;

namespace aEMR.ViewModels
{
    [Export(typeof(IStaffPresence)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class StaffPresenceViewModel : Conductor<object>, IStaffPresence
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public StaffPresenceViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            GetAllStaffDeptPresenceInfo();
            IsLoading = true;
            AllDepartments = new ObservableCollection<RefDepartment>();
            Load_V_DeptTypeOperation();
            CurrentStaffDeptPresence = new StaffDeptPresence();
        }
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                NotifyOfPropertyChange(() => IsLoading);
            }
        }
        private bool _IsAllDeparment = false;
        public bool IsAllDeparment
        {
            get { return _IsAllDeparment; }
            set
            {
                _IsAllDeparment = value;
                NotifyOfPropertyChange(() => IsAllDeparment);
            }
        }
        private bool _IsUpdateRequiredNumber = true;
        public bool IsUpdateRequiredNumber
        {
            get
            {
                return _IsUpdateRequiredNumber;
            }
            set
            {
                _IsUpdateRequiredNumber = value;
                NotifyOfPropertyChange(() => IsUpdateRequiredNumber);
            }
        }
        private bool _IsClinicDept = true;
        public bool IsClinicDept
        {
            get
            {
                return IsUpdateRequiredNumber == true ? true : SelectedDepartments != null && SelectedDepartments.DeptID == 95;
            }
            set
            {
                _IsClinicDept = value;
                NotifyOfPropertyChange(() => IsClinicDept);
            }
        }
        private ObservableCollection<RefDepartment> _AllDepartments;
        public ObservableCollection<RefDepartment> AllDepartments
        {
            get
            {
                return _AllDepartments;
            }
            set
            {
                _AllDepartments = value;
                NotifyOfPropertyChange(() => AllDepartments);
            }
        }
        private RefDepartment _SelectedDepartments;
        public RefDepartment SelectedDepartments
        {
            get
            {
                return _SelectedDepartments;
            }
            set
            {
                _SelectedDepartments = value;
                NotifyOfPropertyChange(() => SelectedDepartments);
                NotifyOfPropertyChange(() => IsClinicDept);
                CurrentStaffDeptPresence.StaffCountDate = Globals.GetCurServerDateTime();
                GetAllStaffDeptPresenceInfo();
            }
        }
        private StaffDeptPresence _CurrentStaffDeptPresence;
        public StaffDeptPresence CurrentStaffDeptPresence
        {
            get
            {
                return _CurrentStaffDeptPresence;
            }
            set
            {
                _CurrentStaffDeptPresence = value;
                NotifyOfPropertyChange(() => CurrentStaffDeptPresence);
            }
        }
        private StaffDeptPresence _StaffDeptPresenceInput;
        public StaffDeptPresence StaffDeptPresenceInput
        {
            get
            {
                return _StaffDeptPresenceInput;
            }
            set
            {
                _StaffDeptPresenceInput = value;
                NotifyOfPropertyChange(() => StaffDeptPresenceInput);
            }
        }
        private ObservableCollection<Lookup> _ListV_DeptTypeOperation;
        public ObservableCollection<Lookup> ListV_DeptTypeOperation
        {
            get { return _ListV_DeptTypeOperation; }
            set
            {
                _ListV_DeptTypeOperation = value;
                NotifyOfPropertyChange(() => ListV_DeptTypeOperation);
            }
        }
        private string _DepartName = eHCMSResources.Z1156_G1_KhoaNgoaiTru;
        public string DepartName
        {
            get { return _DepartName; }
            set
            {
                _DepartName = value;
                NotifyOfPropertyChange(() => DepartName);
            }
        }
        //hàm đọc dữ liệu từ DTB
        public void GetAllStaffDeptPresenceInfo()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllStaffDeptPresenceInfo(SelectedDepartments != null && SelectedDepartments.DeptID > 0 ? SelectedDepartments.DeptID : Globals.DeptLocation.DeptID, CurrentStaffDeptPresence != null ? CurrentStaffDeptPresence.StaffCountDate : Globals.GetCurServerDateTime(), Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            StaffDeptPresence result = contract.EndGetAllStaffDeptPresenceInfo(asyncResult);
                            if (result != null)
                            {
                                CurrentStaffDeptPresence = result;
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
        //hàm lưu thông tin
        public void SaveAllDeptManagementInfo()
        {
            if (CurrentStaffDeptPresence.StaffCountDate.Date > Globals.GetCurServerDateTime().Date)
            {
                MessageBox.Show(eHCMSResources.Z1926_G1_NgCNhatKgLonHonNgHTai);
                return;
            }
            CurrentStaffDeptPresence.DeptID = SelectedDepartments != null && SelectedDepartments.DeptID > 0 ? SelectedDepartments.DeptID : Globals.DeptLocation.DeptID;
            CurrentStaffDeptPresence.StaffID = Globals.LoggedUserAccount.StaffID.GetValueOrDefault();
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginSaveAllStaffDeptPresenceInfo(CurrentStaffDeptPresence, IsUpdateRequiredNumber, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            StaffDeptPresence result = contract.EndSaveAllStaffDeptPresenceInfo(asyncResult);
                            if (result != null)
                            {
                                CurrentStaffDeptPresence = result;
                                MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
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
        public void PrintReport()
        {
            //var proAlloc = Globals.GetViewModel<ICommonPreviewView>();
            //proAlloc.FromDate = CurrentStaffDeptPresence.StaffCountDate.Date;
            //proAlloc.DeptID = IsAllDeparment == true ? 0 : (SelectedDepartments != null ? SelectedDepartments.DeptID : Globals.DeptLocation.DeptID);
            //proAlloc.eItem = ReportName.STAFFDEPTPRESENCE;
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });
            Action<ICommonPreviewView> onInitDlg = (Alloc) =>
            {
                Alloc.FromDate = CurrentStaffDeptPresence.StaffCountDate.Date;
                Alloc.DeptID = IsAllDeparment == true ? 0 : (SelectedDepartments != null ? SelectedDepartments.DeptID : Globals.DeptLocation.DeptID);
                Alloc.eItem = ReportName.STAFFDEPTPRESENCE;
            };
            Action<ICommonPreviewView> onClose = (vm) => {};
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }
        public void Load_V_DeptTypeOperation()
        {
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.T2785_G1_LoaiKhoa) });
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllLookupValuesByType(LookupValues.V_DeptTypeOperation,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);
                                    ListV_DeptTypeOperation = new ObservableCollection<Lookup>(allItems);
                                    LoadData(ListV_DeptTypeOperation);
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                }

                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    Globals.IsBusy = false;
                }
            });
            t.Start();
        }
        public void LoadData(ObservableCollection<Lookup> ListV_DeptTypeOperation)
        {
            Coroutine.BeginExecute(LoadDepartments(ListV_DeptTypeOperation));
        }
        private IEnumerator<IResult> LoadDepartments(ObservableCollection<Lookup> ListV_DeptTypeOperation)
        {
            foreach (var objDeptTypeOperation in ListV_DeptTypeOperation)
            {
                ObservableCollection<RefDepartment> tempDepartments = new ObservableCollection<RefDepartment>();
                if (objDeptTypeOperation != null && objDeptTypeOperation.LookupID > 0)
                {
                    if (Globals.isAccountCheck)
                    {
                        var departmentTask = new LoadDepartmentsByV_DeptTypeOperationTask(objDeptTypeOperation.LookupID, Globals.LoggedUserAccount.DeptIDResponsibilityList);
                        yield return departmentTask;
                        if (departmentTask.Departments != null && departmentTask.Departments.Count > 0)
                        {
                            if (departmentTask.Departments != null && departmentTask.Departments.Count > 0)
                            {
                                foreach (var item in departmentTask.Departments)
                                {
                                    AllDepartments.Add(item);
                                }
                            }
                        }
                    }
                    else
                    {
                        var departmentTask = new LoadDepartmentsByV_DeptTypeOperationTask(objDeptTypeOperation.LookupID);
                        yield return departmentTask;
                        if (departmentTask.Departments != null && departmentTask.Departments.Count > 0)
                        {
                            if (departmentTask.Departments != null && departmentTask.Departments.Count > 0)
                            {
                                foreach (var item in departmentTask.Departments)
                                {
                                    AllDepartments.Add(item);
                                }
                            }
                        }
                    }

                }
            }
            IsLoading = false;
            if (AllDepartments == null
                            || AllDepartments.Count < 1)
            {
                MessageBox.Show(string.Format("{0} {1} {2} \n{3}", eHCMSResources.A0373_G1_Msg_InfoChuaCauHinh, DepartName, eHCMSResources.A0374_G1_Msg_InfoChiuTrachNhiemChoNhVien, eHCMSResources.Z0969_G1_LHeQTriDeCapQuyen));
            }
            else
            {
                if (AllDepartments.Any(x => x.DeptID == Globals.DeptLocation.DeptID))
                    SelectedDepartments = AllDepartments.Where(x => x.DeptID == Globals.DeptLocation.DeptID).FirstOrDefault();
                else
                    SelectedDepartments = AllDepartments[0];
            }

            yield break;

        }
        public void dpkFromdate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            GetAllStaffDeptPresenceInfo();
        }
    }
}
