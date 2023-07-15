using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.CommonTasks;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure.CachingUtils;
using DataEntities;
using eHCMSLanguage;
using System.Windows.Interop;
using aEMR.Common.BaseModel;
using System.Runtime.InteropServices;
using System.Linq;

namespace aEMRMain.ViewModels
{
    [Export(typeof(ISelectLocation)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SelectLocationViewModel : Conductor<object>, ISelectLocation
    {
        [ImportingConstructor]
        public SelectLocationViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            IsLoading = true;
            NewDepartments = new ObservableCollection<RefDepartment>();
            //if(!DesignerProperties.IsInDesignTool)
            bool designTime = System.ComponentModel.DesignerProperties.GetIsInDesignMode(
                    new DependencyObject());
            if (!designTime)
            {
                allDepartmentArray = new ObservableCollection<DepartmentArray>();
                if (Globals.isAccountCheck)
                {
                    CheckResponsibility();
                }
                //else
                //{
                //Load_V_DeptTypeOperation(LstRefDepartment);
                Load_V_DeptTypeOperation();
                //}
            }
        }

        //private V_DeptTypeOperation _V_DeptTypeOperation=V_DeptTypeOperation.KhoaNgoaiTru;
        private V_DeptTypeOperation _V_DeptTypeOperation;
        public V_DeptTypeOperation V_DeptTypeOperation
        {
            get { return _V_DeptTypeOperation; }
            set
            {
                _V_DeptTypeOperation = value;
                NotifyOfPropertyChange(() => V_DeptTypeOperation);
                switch (V_DeptTypeOperation)
                {
                    case V_DeptTypeOperation.KhoaNgoaiTru:
                        DepartName = eHCMSResources.Z1156_G1_KhoaNgoaiTru; break;
                    case V_DeptTypeOperation.KhoaNoi:
                        DepartName = eHCMSResources.Z1157_G1_KhoaNoi; break;
                    case V_DeptTypeOperation.KhoaNgoai:
                        DepartName = eHCMSResources.Z1158_G1_KhoaNgoai; break;
                    case V_DeptTypeOperation.KhoaCanLamSang:
                        DepartName = eHCMSResources.Z1159_G1_KhoaCLS; break;
                }
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


        private Lookup _ObjV_DeptTypeOperation;
        public Lookup ObjV_DeptTypeOperation
        {
            get { return _ObjV_DeptTypeOperation; }
            set
            {
                _ObjV_DeptTypeOperation = value;
                NotifyOfPropertyChange(() => ObjV_DeptTypeOperation);
            }
        }
        private bool _mCancel = false;
        public bool mCancel
        {
            get { return _mCancel; }
            set
            {
                _mCancel = value;
                NotifyOfPropertyChange(() => mCancel);
            }
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

        private ObservableCollection<DepartmentArray> _allDepartmentArray;
        public ObservableCollection<DepartmentArray> allDepartmentArray
        {
            get { return _allDepartmentArray; }
            set
            {
                _allDepartmentArray = value;
                NotifyOfPropertyChange(() => allDepartmentArray);
            }
        }

        private DepartmentArray _curDepartmentArray;
        public DepartmentArray curDepartmentArray
        {
            get { return _curDepartmentArray; }
            set
            {
                _curDepartmentArray = value;
                NotifyOfPropertyChange(() => curDepartmentArray);
                curNewDepartments = curDepartmentArray.allDepartments[0];
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

        private ObservableCollection<Lookup> _allDeptTypeOperation;
        public ObservableCollection<Lookup> allDeptTypeOperation
        {
            get { return _allDeptTypeOperation; }
            set
            {
                _allDeptTypeOperation = value;
                NotifyOfPropertyChange(() => allDeptTypeOperation);
            }
        }

        private const uint MF_BYCOMMAND = 0x00000000;
        private const uint MF_GRAYED = 0x00000001;
        private const uint SC_CLOSE = 0xF060;

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        private static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        protected static void DisableCloseButton(IntPtr windowHandle)
        {
            IntPtr menuHandle = GetSystemMenu(windowHandle, false);
            if (menuHandle != IntPtr.Zero)
            {
                EnableMenuItem(menuHandle, SC_CLOSE, MF_BYCOMMAND | MF_GRAYED);
            }
        }

        public void SelectLocation_Loaded(object view, RoutedEventArgs e)
        {
            base.OnViewLoaded(view);
            var theWindow = (view as FrameworkElement).GetWindow();
            IntPtr hnwdSelPopup = new WindowInteropHelper(theWindow).Handle;

            if (hnwdSelPopup != null)
            {
                IntPtr menuHandle = GetSystemMenu(hnwdSelPopup, false);
                if (menuHandle != IntPtr.Zero)
                {
                    EnableMenuItem(menuHandle, SC_CLOSE, MF_BYCOMMAND | MF_GRAYED);
                }
            }
        }

        private bool bOkCancelButtonPressed = false;

        public override void CanClose(Action<bool> callback)
        {
            if (!bOkCancelButtonPressed)
            {
                callback(false);
                return;
            }
            base.CanClose(callback);
        }

        //public void Load_V_DeptTypeOperation(ObservableCollection<long> _LstRefDepartment)
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

        private ObservableCollection<RefDepartment> _NewDepartments;
        public ObservableCollection<RefDepartment> NewDepartments
        {
            get
            {
                return _NewDepartments;
            }
            set
            {
                _NewDepartments = value;
                NotifyOfPropertyChange(() => NewDepartments);
            }
        }

        private RefDepartment _curNewDepartments;
        public RefDepartment curNewDepartments
        {
            get
            {
                return _curNewDepartments;
            }
            set
            {
                _curNewDepartments = value;
                NotifyOfPropertyChange(() => curNewDepartments);
                NotifyOfPropertyChange(() => CanSelectLocationCmd);
                if (curNewDepartments == null || curNewDepartments.DeptID <= 0)
                {
                    Locations = null;
                    SelectedLocation = null;
                }
                else
                {
                    LoadLocations(curNewDepartments.DeptID);
                }
            }
        }

        private DeptLocation _selectedLocation;
        public DeptLocation SelectedLocation
        {
            get
            {
                return _selectedLocation;
            }
            set
            {
                _selectedLocation = value;
                NotifyOfPropertyChange(() => SelectedLocation);
                NotifyOfPropertyChange(() => CanSelectLocationCmd);
            }
        }

        private ObservableCollection<DeptLocation> _locations;
        public ObservableCollection<DeptLocation> Locations
        {
            get
            {
                return _locations;
            }
            set
            {
                _locations = value;
                NotifyOfPropertyChange(() => Locations);
            }
        }

        public bool CanSelectLocationCmd
        {
            get
            { return (_selectedLocation != null && _selectedLocation.DeptID > 0)
                  || (curNewDepartments == null ? false : curNewDepartments.V_DeptTypeOperation != (long)V_DeptTypeOperation.KhoaNgoaiTru);
            }
        }

        public void SelectLocationCmd()
        {
            if (curNewDepartments != null && curNewDepartments.SelectDeptReqSelectRoom && (_selectedLocation == null || _selectedLocation.DeptLocationID == 0))
            {
                string strMsg = string.Format("{0}.", eHCMSResources.K2102_G1_ChonPgLamViec);
                MessageBox.Show(strMsg);
                return;
            }

            bOkCancelButtonPressed = true;
            var loginVm = Globals.GetViewModel<ILogin>();

            this.ShowBusyIndicator();

            var list = new List<refModule>();

            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetDepartments(_selectedLocation.LID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var allDepartments = contract.EndGetDepartments(asyncResult);
                            loginVm.DeptLocation = SelectedLocation;

                            Globals.EventAggregator.Publish(new LocationSelected_New()
                            {
                                RefDepartment = curNewDepartments
                                ,DeptLocation = _selectedLocation
                                , ItemActivated = ItemActivated });
                            TryClose();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            this.HideBusyIndicator();
                        }
                    }), null);
                }


            });
            t.Start();
        }

        public void CancelCmd()
        {
            bOkCancelButtonPressed = true;
            TryClose();
        }

        public void LoadLocations(long? deptId)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format("{0}.", eHCMSResources.Z0115_G1_LayDSPgBan) });
            this.ShowBusyIndicator();
            var list = new List<refModule>();

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetAllLocationsByDeptIDOld(deptId, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var allItems = contract.EndGetAllLocationsByDeptIDOld(asyncResult);

                            if (allItems != null)
                            {
                                Locations = new ObservableCollection<DeptLocation>(allItems);
                            }
                            else
                            {
                                Locations = new ObservableCollection<DeptLocation>();
                            }

                            var itemDefault = new DeptLocation();
                            itemDefault.DeptID = -1;
                            itemDefault.Location = new Location();
                            itemDefault.Location.LID = -1;
                            itemDefault.Location.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z1152_G1_ChonMotPhg);
                            Locations.Insert(0, itemDefault);

                            SelectedLocation = itemDefault;
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            this.HideBusyIndicator();
                        }
                    }), null);
                }


            });
            t.Start();
        }


        private object _itemActivated;

        public object ItemActivated
        {
            get { return _itemActivated; }
            set
            {
                _itemActivated = value;
                NotifyOfPropertyChange(() => ItemActivated);
            }
        }

        #region Dinh sua cho nay

        //private ObservableCollection<long> _LstRefDepartment;

        //public ObservableCollection<long> LstRefDepartment
        //{
        //    get
        //    {
        //        return _LstRefDepartment;
        //    }
        //    set
        //    {
        //        _LstRefDepartment = value;
        //        NotifyOfPropertyChange(() => LstRefDepartment);
        //    }
        //}

        public void LoadData(ObservableCollection<Lookup> ListV_DeptTypeOperation)
        {
            Coroutine.BeginExecute(NewLoadDepartments(ListV_DeptTypeOperation));
        }
        private IEnumerator<IResult> NewLoadDepartments(ObservableCollection<Lookup> ListV_DeptTypeOperation)
        {
            foreach (var objDeptTypeOperation in ListV_DeptTypeOperation)
            {
                ObservableCollection<RefDepartment> tempDepartments = new ObservableCollection<RefDepartment>();
                if (objDeptTypeOperation != null && objDeptTypeOperation.LookupID > 0)
                {
                    if (Globals.isAccountCheck)
                    {
                        //var departmentTask = new LoadDepartmentsByV_DeptTypeOperationTask(objDeptTypeOperation.LookupID, LstRefDepartment);
                        var departmentTask = new LoadDepartmentsByV_DeptTypeOperationTask(objDeptTypeOperation.LookupID, Globals.LoggedUserAccount.DeptIDResponsibilityList);
                        yield return departmentTask;
                        //NewDepartments = departmentTask.Departments;
                        if (departmentTask.Departments != null
                            && departmentTask.Departments.Count > 0)
                        //&& objDeptTypeOperation.LookupID == (long)V_DeptTypeOperation)
                        {
                            //DepartmentArray newItem = new DepartmentArray(objDeptTypeOperation, ObjectCopier.DeepCopy(departmentTask.Departments));
                            //allDepartmentArray.Add(newItem);
                            //NewDepartments = departmentTask.Departments;
                            if (departmentTask.Departments != null && departmentTask.Departments.Count > 0)
                            {
                                foreach (var item in departmentTask.Departments)
                                {
                                    NewDepartments.Add(item);
                                }
                            }
                        }
                    }
                    else
                    {
                        var departmentTask = new LoadDepartmentsByV_DeptTypeOperationTask(objDeptTypeOperation.LookupID);
                        yield return departmentTask;
                        if (departmentTask.Departments != null
                            && departmentTask.Departments.Count > 0)
                        {
                            //if ((long)V_DeptTypeOperation > 0)
                            //{
                            //    if(objDeptTypeOperation.LookupID == (long)V_DeptTypeOperation)
                            //    {
                            //        NewDepartments = departmentTask.Departments;
                            //    }
                            //}
                            //else 
                            //{
                            //    NewDepartments = departmentTask.Departments;
                            //}
                            if (departmentTask.Departments != null && departmentTask.Departments.Count > 0)
                            {
                                foreach (var item in departmentTask.Departments)
                                {
                                    NewDepartments.Add(item);
                                }
                            }
                        }
                    }
                }
            }
            IsLoading = false;
            if (NewDepartments == null || NewDepartments.Count < 1)
            {
                MessageBox.Show(string.Format("{0} {1} {2} \n{3}", eHCMSResources.A0373_G1_Msg_InfoChuaCauHinh, DepartName, eHCMSResources.A0374_G1_Msg_InfoChiuTrachNhiemChoNhVien, eHCMSResources.Z0969_G1_LHeQTriDeCapQuyen));
            }
            else if (NewDepartments != null && Globals.DeptLocation != null && Globals.DeptLocation.DeptID > 0
                && NewDepartments.Any(x => x.DeptID == Globals.DeptLocation.DeptID))
            {
                curNewDepartments = NewDepartments.First(x => x.DeptID == Globals.DeptLocation.DeptID);
            }
            else
            {
                curNewDepartments = NewDepartments[0];
            }
            yield break;
        }

        private StaffDeptResponsibilities _curStaffDeptResponSearch;
        public StaffDeptResponsibilities curStaffDeptResponSearch
        {
            get
            {
                return _curStaffDeptResponSearch;
            }
            set
            {
                if (_curStaffDeptResponSearch == value)
                    return;
                _curStaffDeptResponSearch = value;
                NotifyOfPropertyChange(() => curStaffDeptResponSearch);

            }
        }

        public void CheckResponsibility()
        {
            if (Globals.isAccountCheck && (Globals.LoggedUserAccount.DeptIDResponsibilityList == null || Globals.LoggedUserAccount.DeptIDResponsibilityList.Count < 1))
            {
                MessageBox.Show(eHCMSResources.A0109_G1_Msg_InfoChuaCauHinhTNKhoaPg);
            }
        }


        //KMx: Khi login thì lấy DepartmentID của nhân viên được cấu hình trách nhiệm luôn, không cần chuyển nữa (10/07/2014 17:14).
        //public void CheckResponsibility()
        //{
        //    List<StaffDeptResponsibilities> results = Globals.LoggedUserAccount.AllStaffDeptResponsibilities;

        //    LstRefDepartment = new ObservableCollection<long>();
        //    if (results != null && results.Count > 0)
        //    {
        //        foreach (var item in results)
        //        {
        //            LstRefDepartment.Add(item.DeptID);
        //        }
        //        NotifyOfPropertyChange(() => LstRefDepartment);
        //        Load_V_DeptTypeOperation(LstRefDepartment);
        //    }

        //    if (LstRefDepartment.Count < 1)
        //    {
        //        MessageBox.Show("Bạn chưa được phân công trách nhiệm với khoa phòng nào. " +
        //                            "\nLiên hệ với người quản trị để được phân bổ khoa phòng chịu trách nhiệm.");
        //    }
        //}

        //KMx: Khi login đã lấy cấu hình trách nhiệm rồi, không cần về server lấy nữa (10/07/2014 17:14).
        //public void CheckResponsibility()
        //{
        //    curStaffDeptResponSearch = new StaffDeptResponsibilities();
        //    curStaffDeptResponSearch.Staff = Globals.LoggedUserAccount.Staff;
        //    curStaffDeptResponSearch.StaffID = (long)Globals.LoggedUserAccount.StaffID;
        //    GetStaffDeptResponsibilitiesByDeptID(curStaffDeptResponSearch, false);
        //}
        //private void GetStaffDeptResponsibilitiesByDeptID(StaffDeptResponsibilities p, bool isHis)
        //{
        //    //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
        //    var t = new Thread(() =>
        //    {
        //        using (var serviceFactory = new UserAccountsServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;
        //            contract.BeginGetStaffDeptResponsibilitiesByDeptID(p, isHis, Globals.DispatchCallback((asyncResult) =>
        //            {
        //                try
        //                {
        //                    var results = contract.EndGetStaffDeptResponsibilitiesByDeptID(asyncResult);
        //                    LstRefDepartment = new ObservableCollection<long>();
        //                    if (results != null && results.Count > 0)
        //                    {
        //                        foreach (var item in results)
        //                        {
        //                            LstRefDepartment.Add(item.DeptID);
        //                        }
        //                        NotifyOfPropertyChange(() => LstRefDepartment);
        //                        Load_V_DeptTypeOperation(LstRefDepartment);
        //                    }

        //                    if (LstRefDepartment.Count < 1)
        //                    {
        //                        MessageBox.Show("Bạn chưa được phân công trách nhiệm với khoa phòng nào. " +
        //                                            "\nLiên hệ với người quản trị để được phân bổ khoa phòng chịu trách nhiệm.");
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                }
        //                finally
        //                {

        //                }

        //            }), null);
        //        }
        //    });

        //    t.Start();
        //}
        #endregion
    }
    public class DepartmentArray : Conductor<object>
    {
        public DepartmentArray()
        {

        }
        public DepartmentArray(Lookup objV_DeptTypeOperation, ObservableCollection<RefDepartment> AllDepartments)
        {
            ObjV_DeptTypeOperation = objV_DeptTypeOperation;
            allDepartments = AllDepartments;
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

        private Lookup _ObjV_DeptTypeOperation;
        public Lookup ObjV_DeptTypeOperation
        {
            get { return _ObjV_DeptTypeOperation; }
            set
            {
                if (_ObjV_DeptTypeOperation != value)
                {
                    _ObjV_DeptTypeOperation = value;
                    NotifyOfPropertyChange(() => ObjV_DeptTypeOperation);
                }
            }
        }

        private ObservableCollection<RefDepartment> _allDepartments;
        public ObservableCollection<RefDepartment> allDepartments
        {
            get
            {
                return _allDepartments;
            }
            set
            {
                _allDepartments = value;
                NotifyOfPropertyChange(() => allDepartments);
            }
        }
    }
}