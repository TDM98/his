using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Threading;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using aEMR.CommonTasks;
using System.Windows.Controls;
using System.Windows;
using eHCMS.CommonUserControls.CommonTasks;
using Microsoft.Win32;


namespace aEMR.TransactionManager.ViewModels
{
    [Export(typeof(IThongKeNgoaiTru)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ThongKeNgoaiTruViewModel : Conductor<object>, IThongKeNgoaiTru
    {
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private IAucHoldConsultDoctor _aucHoldConsultDoctor;
        public IAucHoldConsultDoctor aucHoldConsultDoctor
        {
            get
            {
                return _aucHoldConsultDoctor;
            }
            set
            {
                if (_aucHoldConsultDoctor != value)
                {
                    _aucHoldConsultDoctor = value;
                    NotifyOfPropertyChange(() => aucHoldConsultDoctor);
                }
            }
        }
        private V_DeptTypeOperation _DeptTypeOperation = V_DeptTypeOperation.KhoaNgoaiTru;
        public V_DeptTypeOperation DeptTypeOperation
        {
            get { return _DeptTypeOperation; }
            set
            {
                if (_DeptTypeOperation != value)
                {
                    _DeptTypeOperation = value;
                    NotifyOfPropertyChange(() => DeptTypeOperation);
                }
            }
        }

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ThongKeNgoaiTruViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            //var treeDept = Globals.GetViewModel<IConsultRoomTree>();
            //RoomTree = treeDept;
            //this.ActivateItem(treeDept);
            //Globals.EventAggregator.Subscribe(this);

            Coroutine.BeginExecute(LoadStaffs());
            allPatientRegistrationDetail = new PagedSortableCollectionView<PatientRegistrationDetailEx>();
            allPatientRegistrationDetail.PageIndex = 0;
            allPatientRegistrationDetail.PageSize = 60;
            allPatientRegistrationDetail.OnRefresh += new EventHandler<RefreshEventArgs>(allPatientRegistrationDetail_OnRefresh);
            //LoadRefDept();
            authorization();
            ResetFilter();
            aucHoldConsultDoctor = Globals.GetViewModel<IAucHoldConsultDoctor>();
            aucHoldConsultDoctor.StaffCatType = (long)V_StaffCatType.BacSi;

        }

        void allPatientRegistrationDetail_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchCriteria.pageIndex = allPatientRegistrationDetail.PageIndex;
            GetPatientRegistrationDetailsByRoom();
        }
        protected override void OnActivate()
        {
            base.OnActivate();
        }

        private string _curDate = DateTime.Now.Date.ToShortDateString();
        public string curDate
        {
            get
            {
                return _curDate;
            }
            set
            {
                if (_curDate == value)
                    return;
                _curDate = value;
            }
        }

        private DateTime _BeginDate = DateTime.Now;
        public DateTime BeginDate
        {
            get
            {
                return _BeginDate;
            }
            set
            {
                if (_BeginDate == value)
                    return;
                _BeginDate = value;
            }
        }

        private DateTime _EndDate = DateTime.Now;
        public DateTime EndDate
        {
            get
            {
                return _EndDate;
            }
            set
            {
                if (_EndDate == value)
                    return;
                _EndDate = value;
            }
        }

        public object RoomTree { get; set; }

        private DeptLocation _curDeptLocation;
        public DeptLocation curDeptLocation
        {
            get
            {
                return _curDeptLocation;
            }
            set
            {
                if (_curDeptLocation == value)
                    return;
                _curDeptLocation = value;
            }
        }

        private PatientRegistrationDetail _curPatientRegistrationDetail;
        public PatientRegistrationDetail curPatientRegistrationDetail
        {
            get
            {
                return _curPatientRegistrationDetail;
            }
            set
            {
                if (_curPatientRegistrationDetail == value)
                    return;
                _curPatientRegistrationDetail = value;

            }
        }

        private RefDepartmentsTree _CurRefDepartmentsTree;
        public RefDepartmentsTree CurRefDepartmentsTree
        {
            get { return _CurRefDepartmentsTree; }
            set
            {
                _CurRefDepartmentsTree = value;
                NotifyOfPropertyChange(() => CurRefDepartmentsTree);
            }

        }
        #region properties
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

        private Staff _SelectedStaffGrid;
        public Staff SelectedStaffGrid
        {
            get
            {
                return _SelectedStaffGrid;
            }
            set
            {
                if (_SelectedStaffGrid == value)
                    return;
                _SelectedStaffGrid = value;
            }
        }

        private ObservableCollection<Staff> _tempAllStaff;
        public ObservableCollection<Staff> tempAllStaff
        {
            get
            {
                return _tempAllStaff;
            }
            set
            {
                if (_tempAllStaff == value)
                    return;
                _tempAllStaff = value;
                NotifyOfPropertyChange(() => tempAllStaff);
            }
        }


        private ObservableCollection<DeptLocation> _lstDeptLocation;
        public ObservableCollection<DeptLocation> lstDeptLocation
        {
            get
            {
                return _lstDeptLocation;
            }
            set
            {
                if (_lstDeptLocation == value)
                    return;
                _lstDeptLocation = value;
                NotifyOfPropertyChange(() => lstDeptLocation);
            }
        }

        private SeachPtRegistrationCriteria _searchCriteria;
        public SeachPtRegistrationCriteria SearchCriteria
        {
            get { return _searchCriteria; }
            set
            {
                _searchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }


        private ObservableCollection<DeptLocInfo> _lstDeptLocInfo;
        public ObservableCollection<DeptLocInfo> lstDeptLocInfo
        {
            get
            {
                return _lstDeptLocInfo;
            }
            set
            {
                if (_lstDeptLocInfo == value)
                    return;
                _lstDeptLocInfo = value;
                NotifyOfPropertyChange(() => lstDeptLocInfo);
            }
        }

        private PagedSortableCollectionView<PatientRegistrationDetailEx> _allPatientRegistrationDetail;
        public PagedSortableCollectionView<PatientRegistrationDetailEx> allPatientRegistrationDetail
        {
            get
            {
                return _allPatientRegistrationDetail;
            }
            set
            {
                if (_allPatientRegistrationDetail == value)
                    return;
                _allPatientRegistrationDetail = value;
                NotifyOfPropertyChange(() => allPatientRegistrationDetail);
            }
        }
        #endregion



        protected void ResetFilter()
        {
            SearchCriteria = new SeachPtRegistrationCriteria();
            SearchCriteria.RegStatus = -1;
            SearchCriteria.StaffID = 0;
            SearchCriteria.DeptLocationID = 0;
            SearchCriteria.FromDate = DateTime.Now;
            SearchCriteria.ToDate = DateTime.Now;
            SearchCriteria.pageIndex = 0;
            SearchCriteria.pageSize = allPatientRegistrationDetail.PageSize;

        }
        public void cboStaff_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SearchCmd();
        }

        public void GridPatient_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        public void SearchCmd()
        {
            //if (CurRefDepartmentsTree != null)
            {
                allPatientRegistrationDetail.PageIndex = 0;
                GetPatientRegistrationDetailsByRoom();
            }
        }
        public void InCmd()
        {
            //if (CurRefDepartmentsTree != null)
            {
                //var proAlloc = Globals.GetViewModel<ICommonPreviewView>();
                //proAlloc.DeptID = SearchCriteria.DeptLocationID.Value;
                //proAlloc.FromDate = SearchCriteria.FromDate;
                //proAlloc.ToDate = SearchCriteria.ToDate;
                //proAlloc.DoctorStaffID = SearchCriteria.StaffID.Value;
                //proAlloc.eItem = ReportName.CONSULTATION_BANGKECHITIETKHAM;

                //var instance = proAlloc as Conductor<object>;
                //Globals.ShowDialog(instance, (o) => { });


                Action<ICommonPreviewView> onInitDlg = (proAlloc) =>
                {
                    proAlloc.DeptID = SearchCriteria.DeptLocationID.Value;
                    proAlloc.FromDate = SearchCriteria.FromDate;
                    proAlloc.ToDate = SearchCriteria.ToDate;
                    proAlloc.DoctorStaffID = SearchCriteria.StaffID.Value;
                    proAlloc.eItem = ReportName.CONSULTATION_BANGKECHITIETKHAM;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);




            }
        }

        #region method

        private void GetAllRegisDeptLoc(ObservableCollection<DeptLocation> lstDeptLocation)
        {
            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetAllRegisDeptLocS(lstDeptLocation, Globals.DispatchCallback((asyncResult) =>
                        {
                            lstDeptLocInfo = new ObservableCollection<DeptLocInfo>();
                            try
                            {
                                var item = client.EndGetAllRegisDeptLocS(asyncResult);
                                foreach (var deptLocInfo in item)
                                {
                                    lstDeptLocInfo.Add(deptLocInfo);
                                }

                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                                this.HideBusyIndicator();
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                this.HideBusyIndicator();
                            }

                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        private void GetPatientRegistrationDetailsByRoom()
        {
            this.ShowBusyIndicator();
            SearchCriteria.StaffID = aucHoldConsultDoctor.StaffID;
            SearchCriteria.pageIndex = allPatientRegistrationDetail.PageIndex;
            if (SearchCriteria.StaffID < 1)
            {
                aucHoldConsultDoctor.setDefault();
            }
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePMRsServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetPatientRegistrationDetailsByRoom(SearchCriteria
                            , Globals.DispatchCallback((asyncResult) =>
                        {
                            allPatientRegistrationDetail.Clear();
                            int total = 0;
                            try
                            {
                                var item = client.EndGetPatientRegistrationDetailsByRoom(out total, asyncResult);
                                if (item != null)
                                {
                                    allPatientRegistrationDetail.Clear();
                                    allPatientRegistrationDetail.TotalItemCount = total;
                                    foreach (PatientRegistrationDetailEx patientRegisDetail in item)
                                    {
                                        allPatientRegistrationDetail.Add(patientRegisDetail);
                                    }
                                    NotifyOfPropertyChange(() => allPatientRegistrationDetail);
                                }
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                                this.HideBusyIndicator();
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                this.HideBusyIndicator();
                            }

                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        #endregion

        #region subcribe
        //public void Handle(RoomSelectedEvent Obj)
        //{
        //    CurRefDepartmentsTree = (RefDepartmentsTree)Obj.curDeptLoc;
        //    GetPatientRegistrationDetailsByRoom(((RefDepartmentsTree)Obj.curDeptLoc).NodeID, BeginDate,
        //                                        EndDate);
        //}

        public void Handle(DepartmentSelectedEvent Obj)
        {
            if (Obj != null)
            {

            }
        }

        #endregion

        #region authoriztion
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            mThongKeNgoaiTru_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mYVu_Management
                                               , (int)eYVu_Management.mThongKeNgoaiTru,
                                               (int)oYVu_ManagementEx.mSearch, (int)ePermission.mView);
            mThongKeNgoaiTru_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mYVu_Management
                                               , (int)eYVu_Management.mThongKeNgoaiTru,
                                               (int)oYVu_ManagementEx.mViewAndPrint, (int)ePermission.mView);
            mThongKeNgoaiTru_XuatExcel = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mYVu_Management
                                               , (int)eYVu_Management.mThongKeNgoaiTru,
                                               (int)oYVu_ManagementEx.mExportToExcel, (int)ePermission.mView);

        }
        #region checking account

        private bool _mThongKeNgoaiTru_Tim = true;
        private bool _mThongKeNgoaiTru_XemIn = true;
        private bool _mThongKeNgoaiTru_XuatExcel = true;

        public bool mThongKeNgoaiTru_Tim
        {
            get
            {
                return _mThongKeNgoaiTru_Tim;
            }
            set
            {
                if (_mThongKeNgoaiTru_Tim == value)
                    return;
                _mThongKeNgoaiTru_Tim = value;
            }
        }

        public bool mThongKeNgoaiTru_XemIn
        {
            get
            {
                return _mThongKeNgoaiTru_XemIn;
            }
            set
            {
                if (_mThongKeNgoaiTru_XemIn == value)
                    return;
                _mThongKeNgoaiTru_XemIn = value;
            }
        }

        public bool mThongKeNgoaiTru_XuatExcel
        {
            get
            {
                return _mThongKeNgoaiTru_XuatExcel;
            }
            set
            {
                if (_mThongKeNgoaiTru_XuatExcel == value)
                    return;
                _mThongKeNgoaiTru_XuatExcel = value;
            }
        }

        #endregion
        #region binding visibilty


        #endregion
        #endregion

        private long? _StaffID;
        public long? StaffID
        {
            get { return _StaffID; }
            set
            {
                _StaffID = value;
                NotifyOfPropertyChange(() => StaffID);
            }
        }

        private ObservableCollection<Staff> _Staffs;
        public ObservableCollection<Staff> Staffs
        {
            get { return _Staffs; }
            set
            {
                _Staffs = value;
                NotifyOfPropertyChange(() => Staffs);
            }
        }

        private IEnumerator<IResult> LoadStaffs()
        {
            var paymentTypeTask = new LoadStaffListTask(false, true, (long)AllLookupValues.StaffCatType.BAC_SI);
            yield return paymentTypeTask;
            Staffs = paymentTypeTask.StaffList;
            yield break;
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
                if (curNewDepartments == null || curNewDepartments.DeptID <= 0)
                {
                    Locations = null;
                    SelectedLocation = null;
                    SearchCriteria.DeptID = 0;
                }
                else
                {
                    SearchCriteria.DeptID = curNewDepartments.DeptID;
                    //LoadLocations(curNewDepartments.DeptID);
                    Coroutine.BeginExecute(DoLoadLocations(curNewDepartments.DeptID));
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
                SearchCriteria.DeptLocationID = SelectedLocation != null && SelectedLocation.DeptLocationID > 0 ?
                    SelectedLocation.DeptLocationID : 0;

                allPatientRegistrationDetail.PageIndex = 0;
                GetPatientRegistrationDetailsByRoom();
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

        public void LoadRefDept(V_DeptTypeOperation VDeptTypeOperation)
        {
            Coroutine.BeginExecute(NewLoadDepartments(VDeptTypeOperation));
        }
        private IEnumerator<IResult> NewLoadDepartments(V_DeptTypeOperation VDeptTypeOperation)
        {
            ObservableCollection<RefDepartment> tempDepartments = new ObservableCollection<RefDepartment>();
            var departmentTask = new LoadDepartmentsByV_DeptTypeOperationTask((long)VDeptTypeOperation);
            yield return departmentTask;

            //NewDepartments = departmentTask.Departments.Where(item => item.DeptID == Convert.ToInt32(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.KhoaPhongKham])).ToObservableCollection();
            // Txd 25/05/2014 Replaced ConfigList
            NewDepartments = departmentTask.Departments.Where(item => item.DeptID == Globals.ServerConfigSection.Hospitals.KhoaPhongKham).ToObservableCollection();

            curNewDepartments = NewDepartments.FirstOrDefault();
        }

        private IEnumerator<IResult> DoLoadLocations(long deptId)
        {
            var deptLoc = new LoadDeptLoctionByIDTask(deptId);
            yield return deptLoc;
            if (deptLoc.DeptLocations != null)
            {
                Locations = new ObservableCollection<DeptLocation>(deptLoc.DeptLocations);
            }
            else
            {
                Locations = new ObservableCollection<DeptLocation>();
            }

            var itemDefault = new DeptLocation();
            itemDefault.Location = new Location();
            itemDefault.Location.LID = -1;
            itemDefault.Location.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2094_G1_ChonPg);
            Locations.Insert(0, itemDefault);
            SelectedLocation = itemDefault;
            yield break;
        }

        public void LoadLocations(long? deptId)
        {
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
                            itemDefault.Location.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K0705_G1_TimTatCa);
                            Locations.Insert(0, itemDefault);

                            SelectedLocation = itemDefault;
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

        public void ExportToExcelCmd()
        {
            if (SearchCriteria != null)
            {
                SaveFileDialog objSFD = new SaveFileDialog()
                {
                    DefaultExt = ".xls",
                    Filter = "Excel xls (*.xls)|*.xls",
                    //Filter = "Excel (2003)(.xls)|*.xls|Excel (2010) (.xlsx)|*.xlsx |RichText File (.rtf)|*.rtf |Pdf File (.pdf)|*.pdf |Html File (.html)|*.html",
                    FilterIndex = 1
                };
                if (objSFD.ShowDialog() != true)
                {
                    return;
                }
                Coroutine.BeginExecute(ExportToExcelBangKeChiTietKhamBenh(SearchCriteria, objSFD));
            }
            else
            {
                MessageBox.Show("Không thể xuất excel.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
        }

        private IEnumerator<IResult> ExportToExcelBangKeChiTietKhamBenh(SeachPtRegistrationCriteria _SearchCriteria, SaveFileDialog objSFD)
        {
            var res = new ExportToExcelBangKeChiTietKhamBenhTask(_SearchCriteria, objSFD);
            yield return res;
            //IsProcessing = false;
            yield break;
        }

    }
}
