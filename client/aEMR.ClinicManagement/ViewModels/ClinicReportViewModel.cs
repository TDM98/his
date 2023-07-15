using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Common.PagedCollectionView;

namespace aEMR.ClinicManagement.ViewModels
{
    [Export(typeof(IClinicReport)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ClinicReportViewModel : Conductor<object>, IClinicReport, IHandle<RoomSelectedEvent>
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


        public class GroupName
        {
            public string NameField { get; set; }
            public string NameShow { get; set; }
        }
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ClinicReportViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
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
            FillGroupName();
            selectedConsultationRoomTarget=new ConsultationRoomTarget();
            _tempAllStaff=new ObservableCollection<Staff>();
            _memAllStaff=new ObservableCollection<Staff>();
            _allConsultationRoomStaffAllocations=new ObservableCollection<ConsultationRoomStaffAllocations>();
            //chon radio bac si dau tien
            _allRefStaffCategory = new ObservableCollection<RefStaffCategory>();
            _allRefStaffCategory.Clear();
            GetRefStaffCategoriesByType((long)V_StaffCatType.BacSi);
            authorization();
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

        public object RoomTree { get; set; }
        
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

        private ObservableCollection<GroupName> _GroupNames;
        public ObservableCollection<GroupName> GroupNames
        {
            get
            {
                return _GroupNames;
            }
            set
            {
                if (_GroupNames == value)
                    return;
                _GroupNames = value;
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
                //selectedConsultationRoomStaffAllocations.AllocationDate
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

        public PagedCollectionView TestCollection { get; set; }

        private PagedSortableCollectionView<ConsultationRoomStaffAllocations> _collection;
        public PagedSortableCollectionView<ConsultationRoomStaffAllocations> collection
        {
            get
            {
                return _collection;
            }
            set
            {
                if (_collection == value)
                    return;
                _collection = value;
                NotifyOfPropertyChange(() => collection);
            }
        }

        private ObservableCollection<DateTime> _calRange;
        public ObservableCollection<DateTime> calRange
        {
            get
            {
                return _calRange;
            }
            set
            {
                if (_calRange == value)
                    return;
                _calRange = value;
                NotifyOfPropertyChange(() => calRange);
            }
        }
        #endregion

        //---add loai nhan vien vao combobox
        
        public void cboTimeSegment_SelectionChanged(object sender,SelectionChangedEventArgs e)
        {
            if (selectedConsultationRoomTarget.DeptLocationID > 0)
            {
                GetConsultationRoomTargetTimeSegment(selectedConsultationRoomTarget.DeptLocationID,
                                                  selectedConsultationRoomTarget.ConsultationTimeSegmentID);
                GetConsultationRoomStaffAllocations(selectedConsultationRoomTarget.DeptLocationID
                        , selectedConsultationRoomTarget.ConsultationTimeSegmentID);
            }
        }

        void FillGroupName()
        {
            GroupNames = new ObservableCollection<GroupName>(); 

            GroupNames.Add(new GroupName(){ NameField="",NameShow = "--No Group--"});
            //
            GroupNames.Add(new GroupName() { NameField = "AllocationDate", NameShow = eHCMSResources.N0082_G1_NgKham });
        }

        
        #region method
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
                            IsLoading=false;    
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
                                allConsultationRoomTarget=new ObservableCollection<ConsultationRoomTarget>();
                                foreach (var ConsultRoomTarget in results)
                                {
                                    allConsultationRoomTarget.Add(ConsultRoomTarget);
                                    //if (ConsultRoomTarget.TargetDate<DateTime.Now)
                                    {
                                        selectedConsultationRoomTarget = ConsultRoomTarget;
                                        NotifyOfPropertyChange(() => selectedConsultationRoomTarget);
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
                                if (results != null && results.Count > 0)
                                {
                                    allConsultationRoomTarget = new ObservableCollection<ConsultationRoomTarget>();
                                    foreach (var ConsultRoomTarget in results)
                                    {
                                        allConsultationRoomTarget.Add(ConsultRoomTarget);
                                        //if (ConsultRoomTarget.TargetDate <= DateTime.Now)
                                        {
                                            NotifyOfPropertyChange(() => selectedConsultationRoomTarget);
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
                                collection=new PagedSortableCollectionView<ConsultationRoomStaffAllocations>();
                                var results = contract.EndGetConsultationRoomStaffAllocations(asyncResult);
                                if (results != null && results.Count > 0)
                                {
                                    tempAllStaff=new ObservableCollection<Staff>();
                                    memAllStaff=new ObservableCollection<Staff>();
                                    allConsultationRoomStaffAllocations.Clear();
                                    tempAllStaff.Clear();

                                    ObservableCollection<ConsultationRoomStaffAllocations> test = new ObservableCollection<ConsultationRoomStaffAllocations>();
                                    foreach (var RoomStaffAllocation in results)
                                    {
                                        allConsultationRoomStaffAllocations.Add(RoomStaffAllocation);
                                        collection.Add(RoomStaffAllocation);

                                        test.Add(RoomStaffAllocation);
                                    }
                                    TestCollection = new PagedCollectionView(test);
                                    TestCollection.GroupDescriptions.Add(new Common.PagedCollectionView.PropertyGroupDescription("AllocationDate"));
                                    TestCollection.GroupDescriptions.Add(new Common.PagedCollectionView.PropertyGroupDescription("ConsultationTimeSegments.SegmentName"));
                                    NotifyOfPropertyChange(() => TestCollection);
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
            if (Obj != null)
            {
                selectedConsultationRoomTarget=new ConsultationRoomTarget();
                CurRefDepartmentsTree= (RefDepartmentsTree)Obj.curDeptLoc;
                selectedConsultationRoomTarget.DeptLocationID = CurRefDepartmentsTree.NodeID;
                //if (!bView)
                //{
                //    Globals.ShowMessage("Bạn chưa được cấp quyền để xem thông tin ở đây","");
                //    return;
                //}
                if (selectedConsultationRoomTarget.DeptLocationID>0)
                {
                    tempAllStaff.Clear();
                    ObservableCollection<ConsultationRoomStaffAllocations> test = new ObservableCollection<ConsultationRoomStaffAllocations>();
                    TestCollection = new PagedCollectionView(test);
                    NotifyOfPropertyChange(() => TestCollection);
                    GetConsultationRoomTargetByDeptID(selectedConsultationRoomTarget.DeptLocationID);
                    GetConsultationRoomStaffAllocations(selectedConsultationRoomTarget.DeptLocationID
                        ,selectedConsultationRoomTarget.ConsultationTimeSegmentID);
                }
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
            bEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mClinicManagement
                                               , (int)eClinicManagement.mQuanLyCaKham,
                                               (int)oClinicManagementEx.mHistoryPhongKham, (int)ePermission.mEdit);
            bAdd = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mClinicManagement
                                               , (int)eClinicManagement.mQuanLyCaKham,
                                               (int)oClinicManagementEx.mHistoryPhongKham, (int)ePermission.mAdd);
            bDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mClinicManagement
                                               , (int)eClinicManagement.mQuanLyCaKham,
                                               (int)oClinicManagementEx.mHistoryPhongKham, (int)ePermission.mDelete);
            bView = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mClinicManagement
                                               , (int)eClinicManagement.mQuanLyCaKham,
                                               (int)oClinicManagementEx.mHistoryPhongKham, (int)ePermission.mView);



        }
        #region checking account

        private bool _bEdit = true;
        private bool _bAdd = true;
        private bool _bDelete = true;
        private bool _bView = true;

        public bool bEdit
        {
            get
            {
                return _bEdit;
            }
            set
            {
                if (_bEdit == value)
                    return;
                _bEdit = value;
                NotifyOfPropertyChange(() => bEdit);
            }
        }
        public bool bAdd
        {
            get
            {
                return _bAdd;
            }
            set
            {
                if (_bAdd == value)
                    return;
                _bAdd = value;
                NotifyOfPropertyChange(() => bAdd);
            }
        }
        public bool bDelete
        {
            get
            {
                return _bDelete;
            }
            set
            {
                if (_bDelete == value)
                    return;
                _bDelete = value;
                NotifyOfPropertyChange(() => bDelete);
            }
        }
        public bool bView
        {
            get
            {
                return _bView;
            }
            set
            {
                if (_bView == value)
                    return;
                _bView = value;
                NotifyOfPropertyChange(() => bView);
            }
        }


        #endregion
        #region binding visibilty

        public void lnkDeleteLoaded(object sender, RoutedEventArgs e)
        {
            ((Button)sender).Visibility = Globals.convertVisibility(bDelete);
        }
        #endregion
        #endregion
    }
}
