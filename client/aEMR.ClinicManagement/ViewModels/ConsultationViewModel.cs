using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;

namespace aEMR.ClinicManagement.ViewModels
{
    [Export(typeof(IConsultation)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class ConsultationViewModel : Conductor<object>, IConsultation, IHandle<RoomSelectedEvent>, IHandle<DepartmentSelectedEvent>
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

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ConsultationViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);

            var treeDept = Globals.GetViewModel<IRoomTree>();
            GetAllConsultationTimeSegments();
            RoomTree = treeDept;
            this.ActivateItem(treeDept);
            //Globals.EventAggregator.Subscribe(this);
            //GetRefStaffCategories();
            authorization();
            ResetFilter();
            //GetAllRegisDeptLoc(0, 1000, true);
        }
        protected override void OnActivate()
        {
            base.OnActivate();
        }

        private string _curDate=DateTime.Now.Date.ToShortDateString();
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

        private DateTime _appDate;
        public DateTime appDate
        {
            get
            {
                return _appDate;
            }
            set
            {
                if (_appDate == value)
                    return;
                _appDate = value;
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

        private ObservableCollection<Staff> _memAllStaff;
        public ObservableCollection<Staff> memAllStaff
        {
            get
            {
                return _memAllStaff;
            }
            set
            {
                if (_memAllStaff == value)
                    return;
                _memAllStaff = value;
                NotifyOfPropertyChange(() => memAllStaff);
            }
        }

        private ObservableCollection<Staff> _importStaff;
        public ObservableCollection<Staff> importStaff
        {
            get
            {
                return _importStaff;
            }
            set
            {
                if (_importStaff == value)
                    return;
                _importStaff = value;
            }
        }

        private ObservableCollection<ConsultationRoomStaffAllocations> _exportStaff;
        public ObservableCollection<ConsultationRoomStaffAllocations> exportStaff
        {
            get
            {
                return _exportStaff;
            }
            set
            {
                if (_exportStaff == value)
                    return;
                _exportStaff = value;
            }
        }

        private ConsultationTimeSegments _selectedConsultTimeSeg;
        public ConsultationTimeSegments selectedConsultTimeSeg
        {
            get
            {
                return _selectedConsultTimeSeg;
            }
            set
            {
                if (_selectedConsultTimeSeg == value)
                    return;
                _selectedConsultTimeSeg = value;
                NotifyOfPropertyChange(() => selectedConsultTimeSeg);
                
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

        private ConsultationRoomTarget _selectedConsultationRoomTarget;
        public ConsultationRoomTarget selectedConsultationRoomTarget
        {
            get
            {
                return _selectedConsultationRoomTarget;
            }
            set
            {
                if (_selectedConsultationRoomTarget == value)
                    return;
                _selectedConsultationRoomTarget = value;
                NotifyOfPropertyChange(() => selectedConsultationRoomTarget);
                
            }
        }

        private ObservableCollection<ConsultationRoomTarget> _allConsultationRoomTarget;
        public ObservableCollection<ConsultationRoomTarget> allConsultationRoomTarget
        {
            get
            {
                return _allConsultationRoomTarget;
            }
            set
            {
                if (_allConsultationRoomTarget == value)
                    return;
                _allConsultationRoomTarget = value;
                NotifyOfPropertyChange(() => allConsultationRoomTarget);
            }
        }


        private ConsultationRoomStaffAllocations _selectedConsultationRoomStaffAllocations;
        public ConsultationRoomStaffAllocations selectedConsultationRoomStaffAllocations
        {
            get
            {
                return _selectedConsultationRoomStaffAllocations;
            }
            set
            {
                if (_selectedConsultationRoomStaffAllocations == value)
                    return;
                _selectedConsultationRoomStaffAllocations = value;
                NotifyOfPropertyChange(() => selectedConsultationRoomStaffAllocations);
            }
        }

        private ObservableCollection<ConsultationRoomStaffAllocations> _allConsultationRoomStaffAllocations;
        public ObservableCollection<ConsultationRoomStaffAllocations> allConsultationRoomStaffAllocations
        {
            get
            {
                return _allConsultationRoomStaffAllocations;
            }
            set
            {
                if (_allConsultationRoomStaffAllocations == value)
                    return;
                _allConsultationRoomStaffAllocations = value;
                NotifyOfPropertyChange(() => allConsultationRoomStaffAllocations);
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

        private ObservableCollection<PatientRegistrationDetail> _allPatientRegistrationDetail;
        public ObservableCollection<PatientRegistrationDetail> allPatientRegistrationDetail
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

        private bool isDoctor = true;
        //---add loai nhan vien vao combobox
        
        public bool checkValidRoomTarget(ConsultationRoomTarget selectedConsultationRoomTarget)
        {
            if (selectedConsultationRoomTarget.ConsultationTimeSegmentID < 1)
            {
                Globals.ShowMessage("Bạn chưa chọn thời gian khám!", "");
                return false;
            }
            if (selectedConsultationRoomTarget.DeptLocationID < 1)
            {
                Globals.ShowMessage(eHCMSResources.Z1770_G1_ChuaChonPK, "");
                return false;
            }
            return true;
        }
        public void butSave()
        {
            //Check valid 
            if (!checkValidRoomTarget(selectedConsultationRoomTarget))
            {
                return;
            }
            InsertConsultationRoomTarget(selectedConsultationRoomTarget);

        }
        public void cboTimeSegment_SelectionChanged(object sender,SelectionChangedEventArgs e)
        {
            if (selectedConsultationRoomTarget.DeptLocationID>0)
            {
                GetConsultationRoomTargetTimeSegment(selectedConsultationRoomTarget.DeptLocationID,
                                                  selectedConsultationRoomTarget.ConsultationTimeSegmentID);
                GetConsultationRoomStaffAllocations(selectedConsultationRoomTarget.DeptLocationID
                        , selectedConsultationRoomTarget.ConsultationTimeSegmentID);
            }
        }
        public void butLuu()
        {
            
        }
        public void butReset()
        {
            tempAllStaff.Clear();
            tempAllStaff = ObjectCopier.DeepCopy(memAllStaff);
        }

        private DatePicker dtTargetDay { get; set; }
        public void dtTargetDay_OnLoaded(object sender,RoutedEventArgs e)
        {
            dtTargetDay = sender as DatePicker;
        }

        protected void ResetFilter()
        {
            SearchCriteria = new SeachPtRegistrationCriteria();
            SearchCriteria.RegStatus = -1;
            SearchCriteria.StaffID = -1;
            SearchCriteria.FromDate = DateTime.Now;
            SearchCriteria.ToDate = DateTime.Now;
        }
        
        #region method
        
        private void GetAllRegisDeptLoc(ObservableCollection<DeptLocation> lstDeptLocation)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Đang tìm đăng ký..." });

            var t = new Thread(() =>
            {
                IsLoading=true;

                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetAllRegisDeptLocS(lstDeptLocation, Globals.DispatchCallback((asyncResult) =>
                        {
                            lstDeptLocInfo=new ObservableCollection<DeptLocInfo>();
                            try
                            {
                                var item= client.EndGetAllRegisDeptLocS(asyncResult);
                                foreach (var deptLocInfo in item)
                                {
                                    lstDeptLocInfo.Add(deptLocInfo);
                                }
                                if (lstDeptLocInfo!=null)
                                {
                                    initGrid();    
                                }
                                
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
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
                    //Globals.IsBusy = false;
                    IsLoading = false;
                }
            });
            t.Start();
        }

        private void GetAllRegisDetail(ObservableCollection<DeptLocInfo> lstDeptLocIn)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Đang tìm đăng ký..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetAllRegisDetail(lstDeptLocIn, Globals.DispatchCallback((asyncResult) =>
                        {
                            allPatientRegistrationDetail=new ObservableCollection<PatientRegistrationDetail>();
                            try
                            {
                                var item = client.EndGetAllRegisDetail(asyncResult);
                                foreach (var patientRegisDetail in item)
                                {
                                    allPatientRegistrationDetail.Add(patientRegisDetail);
                                }
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
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
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
                finally
                {
                    //Globals.IsBusy = false;
                    IsLoading = false;
                }
            });
            t.Start();
        }
        
        
        
        
        private void InsertConsultationTimeSegments(string SegmentName, string SegmentDescription, DateTime StartTime,
                                            DateTime EndTime, bool IsActive)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginInsertConsultationTimeSegments(SegmentName, SegmentDescription, StartTime,
                                EndTime, null, null, IsActive, Globals.DispatchCallback((asyncResult) =>
                             {
                                 try
                                 {
                                     
                                     var results = contract.EndInsertConsultationTimeSegments(asyncResult);
                                     if (results )
                                     {
                                         
                                     }
                                 }
                                 catch (Exception ex)
                                 {
                                     Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                 }
                                 finally
                                 {
                                     //Globals.IsBusy = false;
                                     IsLoading = false;
                                 }

                             }), null);

                }

            });

            t.Start();
        }

        private void GetRefStaffCategoriesByType(long V_StaffCatType)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetRefStaffCategoriesByType(V_StaffCatType,Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetRefStaffCategoriesByType(asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                if (allRefStaffCategory == null)
                                {
                                    //allRefStaffCategory = new ObservableCollection<RefStaffCategory>();
                                }
                                else
                                {
                                    //allRefStaffCategory.Clear();
                                }
                                foreach (var p in results)
                                {
                                    allRefStaffCategory.Add(p);
                                }

                                NotifyOfPropertyChange(() => allRefStaffCategory);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void GetAllStaff(long StaffCatgID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllStaff(StaffCatgID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetAllStaff(asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                if (allStaff == null)
                                {
                                    allStaff = new ObservableCollection<Staff>();
                                }
                                else
                                {
                                    allStaff.Clear();
                                }
                                foreach (var p in results)
                                {
                                    allStaff.Add(p);
                                }

                                NotifyOfPropertyChange(() => allStaff);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void GetAllConsultationTimeSegments()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

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
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void InsertConsultationRoomTarget(ConsultationRoomTarget curConsultationRoomTarget)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginInsertConsultationRoomTarget(curConsultationRoomTarget
                        , Globals.DispatchCallback((asyncResult) =>
                                {
                                    try
                                    {
                                        var results = contract.EndInsertConsultationRoomTarget(asyncResult);
                                        if (results)
                                        {
                                            Globals.ShowMessage(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK,"");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                    }
                                    finally
                                    {
                                        //Globals.IsBusy = false;
                                        IsLoading = false;
                                    }

                                }), null);

                }

            });

            t.Start();
        }
        private void GetConsultationRoomTargetByDeptID(long DeptLocationID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetConsultationRoomTargetByDeptID(DeptLocationID
                        ,Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetConsultationRoomTargetByDeptID(asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                results[0].DeptLocationID = curDeptLocation.DeptLocationID;
                                selectedConsultationRoomTarget = results[0];
                                
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void GetConsultationRoomTargetTimeSegment(long DeptLocationID, long ConsultationTimeSegmentID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetConsultationRoomTargetTimeSegment(DeptLocationID,ConsultationTimeSegmentID
                        , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetConsultationRoomTargetTimeSegment(asyncResult);
                                //if (results != null && results.Count > 0)
                                //{
                                //    allConsultationRoomTarget = new ObservableCollection<ConsultationRoomTarget>();
                                //    foreach (var ConsultRoomTarget in results)
                                //    {
                                //        allConsultationRoomTarget.Add(ConsultRoomTarget);
                                //        if (ConsultRoomTarget.TargetDate <= DateTime.Now)
                                //        {
                                //            selectedConsultationRoomTarget.TargetDate =ConsultRoomTarget.TargetDate;
                                //            selectedConsultationRoomTarget.TargetNumberOfCases = ConsultRoomTarget.TargetNumberOfCases;
                                //            NotifyOfPropertyChange(() => selectedConsultationRoomTarget);
                                //        }
                                //    }
                                //}
                                if (results != null && results.Count > 0)
                                {
                                    NotifyOfPropertyChange(() => selectedConsultationRoomTarget);
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                //Globals.IsBusy = false;
                                IsLoading = false;
                            }

                        }), null);

                }

            });

            t.Start();
        }


        private void InsertConsultationRoomStaffAllocations(long DeptLocationID
                                                  , long ConsultationTimeSegmentID
                                                  , long StaffID
                                                  , long StaffCatgID
                                                  , DateTime AllocationDate)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginInsertConsultationRoomStaffAllocations( DeptLocationID, ConsultationTimeSegmentID
                                                  , StaffID, StaffCatgID, AllocationDate, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                
                                var results = contract.EndInsertConsultationRoomStaffAllocations(asyncResult);
                                if (results)
                                {
                                    Globals.ShowMessage(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK, "");
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                //Globals.IsBusy = false;
                                IsLoading = false;
                            }

                        }), null);

                }

            });

            t.Start();
        }

        private void UpdateConsultationRoomStaffAllocations(long ConsultationRoomStaffAllocID, bool IsActive)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateConsultationRoomStaffAllocations(ConsultationRoomStaffAllocID, IsActive
                        , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                
                                var results = contract.EndUpdateConsultationRoomStaffAllocations(asyncResult);
                                if (results)
                                {
                                    Globals.ShowMessage(eHCMSResources.A0279_G1_Msg_InfoCNhatOK, "");
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                //Globals.IsBusy = false;
                                IsLoading = false;
                            }

                        }), null);

                }

            });

            t.Start();
        }
        private void GetConsultationRoomStaffAllocations(long DeptLocationID, long ConsultationTimeSegmentID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetConsultationRoomStaffAllocations(DeptLocationID, ConsultationTimeSegmentID
                        , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                allConsultationRoomStaffAllocations = new ObservableCollection<ConsultationRoomStaffAllocations>();
                                var results = contract.EndGetConsultationRoomStaffAllocations(asyncResult);
                                if (results != null )
                                {
                                    
                                    tempAllStaff=new ObservableCollection<Staff>();
                                    memAllStaff=new ObservableCollection<Staff>();
                                    allConsultationRoomStaffAllocations.Clear();
                                    tempAllStaff.Clear();
                                    if(results.Count>0 )
                                    {
                                        appDate = results[0].AllocationDate;
                                        selectedConsultationRoomStaffAllocations = results[0];
                                        foreach (var RoomStaffAllocation in results)
                                        {
                                            if (RoomStaffAllocation.AllocationDate.ToShortDateString() == appDate.ToShortDateString())
                                            {
                                                allConsultationRoomStaffAllocations.Add(RoomStaffAllocation);
                                                if (isDoctor)
                                                {
                                                    if (RoomStaffAllocation.Staff.RefStaffCategory.V_StaffCatType ==
                                                        (long)V_StaffCatType.BacSi)
                                                    {
                                                        tempAllStaff.Add(RoomStaffAllocation.Staff);
                                                        memAllStaff.Add(RoomStaffAllocation.Staff);
                                                    }
                                                }
                                                else
                                                {
                                                    if (RoomStaffAllocation.Staff.RefStaffCategory.V_StaffCatType ==
                                                        (long)V_StaffCatType.PhuTa)
                                                    {
                                                        tempAllStaff.Add(RoomStaffAllocation.Staff);
                                                        memAllStaff.Add(RoomStaffAllocation.Staff);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //selectedConsultationRoomStaffAllocations.AllocationDate = DBNull.Value;
                                    }
                                    
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                //Globals.IsBusy = false;
                                IsLoading = false;
                            }

                        }), null);

                }

            });

            t.Start();
        }
        #endregion

#region subcribe
        public void Handle(RoomSelectedEvent Obj)
        {
            
        }

        public void Handle(DepartmentSelectedEvent Obj)
        {
            if (Obj != null)
            {
                selectedConsultationRoomTarget=new ConsultationRoomTarget();
                
                CurRefDepartmentsTree= (RefDepartmentsTree)Obj.curDepartment;
                lstDeptLocation=new ObservableCollection<DeptLocation>();
                foreach (var depLoc in CurRefDepartmentsTree.Children)
                {
                    DeptLocation dt=new DeptLocation();
                    dt.DeptLocationID = depLoc.NodeID;
                    dt.Location=new Location();
                    dt.Location.LocationName = depLoc.NodeText;
                    dt.RefDepartment=new RefDepartment();
                    lstDeptLocation.Add(dt);
                    //kiem tra xem so benh nhan cho kham o day


                }
                GetAllRegisDeptLoc(lstDeptLocation);
                

            }
        }

#endregion

#region   animator
        
        public Grid GridRoomConsult { get; set; }
        public void GridRoomConsult_Loaded(object sender, RoutedEventArgs e)
        {
            GridRoomConsult = sender as Grid;
            //for (int i = 0; i < lstDeptLocInfo.Count; i++)
            //{
            //    ColumnDefinition cd=new ColumnDefinition();
            //    cd.Width = new GridLength(150, GridUnitType.Auto);
            //    GridRoomConsult.ColumnDefinitions.Add(new ColumnDefinition());
            //}
            
        }

        public void initGrid()
        {
            int Row = 0;
            int Column = 0;
            GridRoomConsult.ColumnDefinitions.Clear();
            for (int i = 0; i < lstDeptLocInfo.Count; i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength(180, GridUnitType.Auto);
                GridRoomConsult.ColumnDefinitions.Add(new ColumnDefinition());
            }
            GridRoomConsult.Children.Clear();
            for (int i = 0; i < lstDeptLocInfo.Count; i++)
            {
                DeptLocInfo bpa = lstDeptLocInfo[i];
                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Vertical;
                sp.Width = minWidth;
                sp.Height = minHeight;
                sp.Orientation = Orientation.Vertical;
                sp.DataContext = bpa;

                TextBlock roomName = new TextBlock();
                roomName.HorizontalAlignment = HorizontalAlignment.Center;
                roomName.VerticalAlignment = VerticalAlignment.Center;
                roomName.FontWeight =FontWeights.Bold;
                //roomName.FontSize = 10;
                roomName.Foreground = new SolidColorBrush(Color.FromArgb(255, 53, 149, 203));
                roomName.Text = bpa.Location.LocationName;
                sp.Children.Add(roomName);

                Image image = new Image();
                image.Height = 90;
                image.Width = 90;
                Uri uri = new Uri("/aEMR.CommonViews;component/Assets/Images/hospital.png", UriKind.Relative);
                ImageSource img = new BitmapImage(uri);
                image.SetValue(Image.SourceProperty, img);
                sp.Children.Add(image);

                TextBlock ck = new TextBlock();
                ck.HorizontalAlignment = HorizontalAlignment.Center;
                ck.VerticalAlignment = VerticalAlignment.Center;
                ck.FontWeight = FontWeights.Bold;
                ck.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 102));
                ck.Text = string.Format("{0}: ", eHCMSResources.K1895_G1_ChoKham2)+bpa.ChoKham;
                sp.Children.Add(ck);

                TextBlock kr = new TextBlock();
                kr.HorizontalAlignment = HorizontalAlignment.Center;
                kr.VerticalAlignment = VerticalAlignment.Center;
                kr.FontWeight = FontWeights.Bold;
                kr.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 102));
                kr.Text = "Khám rồi: " +bpa.KhamRoi;
                sp.Children.Add(kr);

                sp.DataContext = bpa;
                sp.MouseMove += new MouseEventHandler(sp_MouseMove);
                sp.MouseLeave += new MouseEventHandler(sp_MouseLeave);
                sp.MouseLeftButtonUp+=new MouseButtonEventHandler(sp_MouseLeftButtonUp);

                GridRoomConsult.Children.Add(sp);
                Grid.SetRow(sp, Row);
                Grid.SetColumn(sp, Column);
                Column++;
                //if (Column ==5)
                //{
                //    Column = 0;
                //    Row++;
                //}
            }
        }
        private int minWidth=120;
        private int minHeight = 170;
        private int maxWidth = 140;
        private int maxHeight = 190;
        void sp_MouseLeave(object sender, MouseEventArgs e)
        {
            ((StackPanel)sender).Width = minWidth;
            ((StackPanel)sender).Height = minHeight;
        }

        void sp_MouseMove(object sender, MouseEventArgs e)
        {
            ((StackPanel)sender).Width = maxWidth;
            ((StackPanel)sender).Height = maxHeight;
        }
        void  sp_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
 	        //Kiem tra so nguoi cho kham o day
            ObservableCollection<DeptLocInfo> lstDeptIn=new ObservableCollection<DeptLocInfo>();
            lstDeptIn.Add((DeptLocInfo)((StackPanel)sender).DataContext);
            GetAllRegisDetail(lstDeptIn);

        }
#endregion

        #region authoriztion
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bQuanEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mClinicManagement
                                               , (int)eClinicManagement.mQuanLyPhongKham,
                                               (int)oClinicManagementEx.mQuanLySoLuong, (int)ePermission.mEdit);
            bQuanAdd = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mClinicManagement
                                               , (int)eClinicManagement.mQuanLyPhongKham,
                                               (int)oClinicManagementEx.mQuanLySoLuong, (int)ePermission.mAdd);
            bQuanDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mClinicManagement
                                               , (int)eClinicManagement.mQuanLyPhongKham,
                                               (int)oClinicManagementEx.mQuanLySoLuong, (int)ePermission.mDelete);
            bQuanView = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mClinicManagement
                                               , (int)eClinicManagement.mQuanLyPhongKham,
                                               (int)oClinicManagementEx.mQuanLySoLuong, (int)ePermission.mView);

            bStaffEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mClinicManagement
                                               , (int)eClinicManagement.mQuanLyPhongKham,
                                               (int)oClinicManagementEx.mPhanBoNhanVien, (int)ePermission.mEdit);
            bStaffAdd = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mClinicManagement
                                               , (int)eClinicManagement.mQuanLyPhongKham,
                                               (int)oClinicManagementEx.mPhanBoNhanVien, (int)ePermission.mAdd);
            bStaffDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mClinicManagement
                                               , (int)eClinicManagement.mQuanLyPhongKham,
                                               (int)oClinicManagementEx.mPhanBoNhanVien, (int)ePermission.mDelete);
            bStaffView = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mClinicManagement
                                               , (int)eClinicManagement.mQuanLyPhongKham,
                                               (int)oClinicManagementEx.mPhanBoNhanVien, (int)ePermission.mView);
            
        }
        #region checking account

        private bool _bQuanEdit = true;
        private bool _bQuanAdd = true;
        private bool _bQuanDelete = true;
        private bool _bQuanView = true;

        private bool _bStaffEdit = true;
        private bool _bStaffAdd = true;
        private bool _bStaffDelete = true;
        private bool _bStaffView = true;

        public bool bQuanEdit
        {
            get
            {
                return _bQuanEdit;
            }
            set
            {
                if (_bQuanEdit == value)
                    return;
                _bQuanEdit = value;
            }
        }
        public bool bQuanAdd
        {
            get
            {
                return _bQuanAdd;
            }
            set
            {
                if (_bQuanAdd == value)
                    return;
                _bQuanAdd = value;
            }
        }
        public bool bQuanDelete
        {
            get
            {
                return _bQuanDelete;
            }
            set
            {
                if (_bQuanDelete == value)
                    return;
                _bQuanDelete = value;
            }
        }
        public bool bQuanView
        {
            get
            {
                return _bQuanView;
            }
            set
            {
                if (_bQuanView == value)
                    return;
                _bQuanView = value;
            }
        }

        public bool bStaffEdit
        {
            get
            {
                return _bStaffEdit;
            }
            set
            {
                if (_bStaffEdit == value)
                    return;
                _bStaffEdit = value;
            }
        }
        public bool bStaffAdd
        {
            get
            {
                return _bStaffAdd;
            }
            set
            {
                if (_bStaffAdd == value)
                    return;
                _bStaffAdd = value;
            }
        }
        public bool bStaffDelete
        {
            get
            {
                return _bStaffDelete;
            }
            set
            {
                if (_bStaffDelete == value)
                    return;
                _bStaffDelete = value;
            }
        }
        public bool bStaffView
        {
            get
            {
                return _bStaffView;
            }
            set
            {
                if (_bStaffView == value)
                    return;
                _bStaffView = value;
            }
        }
        
        #endregion
        #region binding visibilty


        #endregion
#endregion
    }
}
