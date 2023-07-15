using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
    [Export(typeof(IConsultationTargetEx)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ConsultationTargetExViewModel : Conductor<object>, IConsultationTargetEx, IHandle<RoomSelectedEvent>
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
        public ConsultationTargetExViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);

            GetAllConsultationTimeSegments();
            var treeDept = Globals.GetViewModel<IRoomTree>();
            RoomTree = treeDept;
            this.ActivateItem(treeDept);
            //Globals.EventAggregator.Subscribe(this);
            selectedConsultationRoomTarget = new ConsultationRoomTarget();
            selectedConsultationRoomTarget.ConsultationTimeSegments = new ConsultationTimeSegments();
            selectedTempConsultationRoomTarget = new ConsultationRoomTarget();
            selectedConsultTimeSeg = new ConsultationTimeSegments();
            allConsultationRoomTarget = new ObservableCollection<ConsultationRoomTarget>();
            AllHisConsultationRoomTarget=new ObservableCollection<ConsultationRoomTarget>();
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

        public DateTime CurDateTime = DateTime.Now;
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
                
                if (selectedConsultTimeSeg.ConsultationTimeSegmentID>0)
                {
                    AllHisConsultationRoomTarget=new ObservableCollection<ConsultationRoomTarget>();
                    foreach (var crt in AllHisTempConsultationRoomTarget)
                    {
                        if (crt.ConsultationTimeSegmentID == selectedConsultTimeSeg.ConsultationTimeSegmentID)
                        {
                            AllHisConsultationRoomTarget.Add(crt);
                        }
                    }
                }
                NotifyOfPropertyChange(() => AllHisConsultationRoomTarget);
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
                if (_selectedConsultationRoomTarget != value)
                    _selectedConsultationRoomTarget = value;
                NotifyOfPropertyChange(() => selectedConsultationRoomTarget);
            }
        }

        private ConsultationRoomTarget _selectedTempConsultationRoomTarget;
        public ConsultationRoomTarget selectedTempConsultationRoomTarget
        {
            get
            {
                return _selectedTempConsultationRoomTarget;
            }
            set
            {
                if (_selectedTempConsultationRoomTarget == value)
                    return;
                _selectedTempConsultationRoomTarget = value;
                NotifyOfPropertyChange(() => selectedTempConsultationRoomTarget);

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
                //Kiem tra chi tieu hien tai
                //GetCurTimeSegment();
                NotifyOfPropertyChange(()=>curAllConsultationRoomTarget);
            }
        }

        private ObservableCollection<ConsultationRoomTarget> _curAllConsultationRoomTarget;
        public ObservableCollection<ConsultationRoomTarget> curAllConsultationRoomTarget
        {
            get
            {
                return _curAllConsultationRoomTarget;
            }
            set
            {
                if (_curAllConsultationRoomTarget == value)
                    return;
                _curAllConsultationRoomTarget = value;
                NotifyOfPropertyChange(() => curAllConsultationRoomTarget);
            }
        }

        private ObservableCollection<ConsultationRoomTarget> _AllTempConsultationRoomTarget;
        public ObservableCollection<ConsultationRoomTarget> AllTempConsultationRoomTarget
        {
            get
            {
                return _AllTempConsultationRoomTarget;
            }
            set
            {
                if (_AllTempConsultationRoomTarget == value)
                    return;
                _AllTempConsultationRoomTarget = value;
                NotifyOfPropertyChange(() => AllTempConsultationRoomTarget);
            }
        }

        private ObservableCollection<ConsultationRoomTarget> _AllHisConsultationRoomTarget;
        public ObservableCollection<ConsultationRoomTarget> AllHisConsultationRoomTarget
        {
            get
            {
                return _AllHisConsultationRoomTarget;
            }
            set
            {
                if (_AllHisConsultationRoomTarget == value)
                    return;
                _AllHisConsultationRoomTarget = value;
                NotifyOfPropertyChange(() => AllHisConsultationRoomTarget);
            }
        }

        private ObservableCollection<ConsultationRoomTarget> _AllHisTempConsultationRoomTarget;
        public ObservableCollection<ConsultationRoomTarget> AllHisTempConsultationRoomTarget
        {
            get
            {
                return _AllHisTempConsultationRoomTarget;
            }
            set
            {
                if (_AllHisTempConsultationRoomTarget == value)
                    return;
                _AllHisTempConsultationRoomTarget = value;
                NotifyOfPropertyChange(() => AllHisTempConsultationRoomTarget);
            }
        }

        #endregion
        
        public void hplCheckDay()
        {
            //var DaysCheckVM = Globals.GetViewModel<IcwdDaysCheck>();
            //DaysCheckVM.curConsultationRoomTarget = selectedConsultationRoomTarget;
            //var instance = DaysCheckVM as Conductor<object>;

            //this.ActivateItem(DaysCheckVM);
            //Globals.ShowDialog(instance, (o) => { });

            Action<IcwdDaysCheck> onInitDlg = (DaysCheckVM) =>
            {
                DaysCheckVM.curConsultationRoomTarget = selectedConsultationRoomTarget;
                this.ActivateItem(DaysCheckVM);
            };
            GlobalsNAV.ShowDialog<IcwdDaysCheck>(onInitDlg);
        }

        public void lnkDefaultTargetClick(object sender)
        {
            selectedConsultationRoomTarget.MondayTargetNumberOfCases = selectedConsultationRoomTarget.TuesdayTargetNumberOfCases
                = selectedConsultationRoomTarget.WednesdayTargetNumberOfCases = selectedConsultationRoomTarget.ThursdayTargetNumberOfCases
                = selectedConsultationRoomTarget.FridayTargetNumberOfCases = selectedConsultationRoomTarget.SaturdayTargetNumberOfCases = selectedConsultationRoomTarget.SundayTargetNumberOfCases 
                = selectedConsultationRoomTarget.SundayTargetNumberOfCases 
                = selectedConsultationRoomTarget.DefaultTargetNumberOfCases;
            NotifyOfPropertyChange(() => selectedConsultationRoomTarget);
        }

        public void lnkDefaultMaxClick(object sender)
        {
            selectedConsultationRoomTarget.MondayMaxNumConsultationAllowed = selectedConsultationRoomTarget.TuesdayMaxNumConsultationAllowed
                = selectedConsultationRoomTarget.WednesdayMaxNumConsultationAllowed = selectedConsultationRoomTarget.ThursdayMaxNumConsultationAllowed
                = selectedConsultationRoomTarget.FridayMaxNumConsultationAllowed = selectedConsultationRoomTarget.SaturdayMaxNumConsultationAllowed = selectedConsultationRoomTarget.SundayMaxNumConsultationAllowed
                = selectedConsultationRoomTarget.SundayMaxNumConsultationAllowed
                = selectedConsultationRoomTarget.DefaultMaxNumConsultationAllowed;
            NotifyOfPropertyChange(() => selectedConsultationRoomTarget);
        }
        
        public void butUpdate()
        {
            curAllConsultationRoomTarget=new ObservableCollection<ConsultationRoomTarget>();

            for (int i = 0; i < allConsultationRoomTarget.Count; i++)
			{
			    if(!allConsultationRoomTarget[i].propertiesEqual(AllTempConsultationRoomTarget[i]))
                    curAllConsultationRoomTarget.Add(allConsultationRoomTarget[i]);
			} 
            
            UpdateConsultationRoomTarget(curAllConsultationRoomTarget);
        }
        public void butGetAll()
        {
            AllHisConsultationRoomTarget=new ObservableCollection<ConsultationRoomTarget>();
            AllHisConsultationRoomTarget = AllHisTempConsultationRoomTarget;
            NotifyOfPropertyChange(() => AllHisConsultationRoomTarget);
            selectedConsultTimeSeg=lstConsultationTimeSegments[0];
        }

        public void butReset()
        {
            allConsultationRoomTarget=new ObservableCollection<ConsultationRoomTarget>();
            allConsultationRoomTarget = ObjectCopier.DeepCopy(AllTempConsultationRoomTarget);
        }
        
        private DatePicker dtTargetDay { get; set; }
        public void dtTargetDay_OnLoaded(object sender,RoutedEventArgs e)
        {
            dtTargetDay = sender as DatePicker;
        }
        public void lnkDeleteClick(object sender)
        {
            if (MessageBox.Show("Bạn Có Muốn Xóa Chỉ Tiêu Này! Tiếp Tục?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                return;
            }
            DeleteConsultationRoomTarget(selectedTempConsultationRoomTarget.ConsultationRoomTargetID);
        }
        public void grdListTarget_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            ConsultationRoomTarget objRows = e.Row.DataContext as ConsultationRoomTarget;
            if (objRows != null)
            {
                switch (objRows.IsDeleted)
                {
                    case false:
                    {
                        e.Row.Background = new SolidColorBrush(Colors.Orange);
                        break;
                    }
                }
            }
        }

        public Expander expXemChiTieu { get; set; }
        public void expXemChiTieu_Loaded(object sender)
        {
            expXemChiTieu=(Expander)sender;
        }

        public Expander expDatChiTieu { get; set; }
        public void expDatChiTieu_Loaded(object sender)
        {
            expDatChiTieu = (Expander)sender;
        }


        public void expXemChiTieu_Expanded(object sender)
        {
            expDatChiTieu.IsExpanded = false;
        }

        public void expDatChiTieu_Expanded(object sender)
        {
            expXemChiTieu.IsExpanded= false;
        }

        #region method
        private void InsertConsultationTimeSegments(string SegmentName, string SegmentDescription, DateTime StartTime,
                                            DateTime EndTime, bool IsActive)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading=true;

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
                                     IsLoading = false;
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
                                ConsultationTimeSegments curConsTimeSegments=new ConsultationTimeSegments();
                                curConsTimeSegments.SegmentName = "--Chọn Ca Khám--";
                                curConsTimeSegments.ConsultationTimeSegmentID = 0;
                                lstConsultationTimeSegments.Add(curConsTimeSegments);
                                foreach (var consTimeSeg in results)
                                {
                                    lstConsultationTimeSegments.Add(consTimeSeg);
                                }
                                selectedConsultTimeSeg = curConsTimeSegments;
                                selectedConsultationRoomTarget.ConsultationTimeSegments = curConsTimeSegments;
                            }
                        }
                        catch (Exception ex)
                        {
                            IsLoading = false;
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
                    contract.BeginInsertConsultationRoomTarget(curConsultationRoomTarget, Globals.DispatchCallback((asyncResult) =>
                                {
                                    try
                                    {
                                        
                                        var results = contract.EndInsertConsultationRoomTarget(asyncResult);
                                        if (results)
                                        {
                                            Globals.ShowMessage(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK,"");
                                            Globals.EventAggregator.Publish(new AddNewRoomTargetEvent());
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        IsLoading = false;
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

        private void DeleteConsultationRoomTarget(long ConsultationRoomTargetID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;
                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDeleteConsultationRoomTarget(ConsultationRoomTargetID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var results = contract.EndDeleteConsultationRoomTarget(asyncResult);
                            if (results)
                            {
                                Globals.ShowMessage(eHCMSResources.K0537_G1_XoaOk, "");
                                Globals.EventAggregator.Publish(new AddNewRoomTargetEvent());
                            }
                        }
                        catch (Exception ex)
                        {
                            IsLoading = false;
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

        private void UpdateConsultationRoomTarget(ObservableCollection<ConsultationRoomTarget> lstConsultationRoomTarget)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;
                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateConsultationRoomTargetXML(lstConsultationRoomTarget, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var results = contract.EndUpdateConsultationRoomTargetXML(asyncResult);
                            if (results)
                            {
                                Globals.ShowMessage(eHCMSResources.A0279_G1_Msg_InfoCNhatOK, "");
                                Globals.EventAggregator.Publish(new AddNewRoomTargetEvent());
                            }
                        }
                        catch (Exception ex)
                        {
                            IsLoading = false;
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
                                //selectedConsultationRoomTarget = results[0];
                                
                            }
                        }
                        catch (Exception ex)
                        {
                            IsLoading = false;
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

        private void GetConsultationRoomTargetTSegment(long DeptLocationID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetConsultationRoomTargetTSegment(DeptLocationID,false
                        , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                DateTime curDate = DateTime.Now;
                                var results = contract.EndGetConsultationRoomTargetTSegment(out curDate,asyncResult);                                
                                if (results != null && results.Count > 0)
                                {
                                    allConsultationRoomTarget= new ObservableCollection<ConsultationRoomTarget>();
                                    AllTempConsultationRoomTarget = new ObservableCollection<ConsultationRoomTarget>();
                                    foreach (var item in results)
                                    {
                                        if (item.DeptLocationID == 0) 
                                        {
                                            item.DeptLocationID = CurRefDepartmentsTree.NodeID;
                                            item.DeptLocation = new DeptLocation();
                                            item.DeptLocation.DeptLocationID = item.DeptLocationID;
                                        }
                                        allConsultationRoomTarget.Add(item);
                                        AllTempConsultationRoomTarget.Add(ObjectCopier.DeepCopy(item));
                                    }
                                    NotifyOfPropertyChange(() => allConsultationRoomTarget);
                                }
                            }
                            catch (Exception ex)
                            {
                                IsLoading = false;
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

        private void GetHisConsultationRoomTargetTSegment(long DeptLocationID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetConsultationRoomTargetTSegment(DeptLocationID, true
                        , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                DateTime curDate = DateTime.Now;
                                var results = contract.EndGetConsultationRoomTargetTSegment(out curDate,asyncResult);
                                if (results != null && results.Count > 0)
                                {
                                    AllHisConsultationRoomTarget = new ObservableCollection<ConsultationRoomTarget>();
                                    AllHisTempConsultationRoomTarget = new ObservableCollection<ConsultationRoomTarget>();
                                    foreach (var item in results)
                                    {
                                        if (item.DeptLocationID == 0)
                                        {
                                            item.DeptLocationID = CurRefDepartmentsTree.NodeID;
                                            item.DeptLocation = new DeptLocation();
                                            item.DeptLocation.DeptLocationID = item.DeptLocationID;
                                        }
                                        AllHisConsultationRoomTarget.Add(item);
                                        AllHisTempConsultationRoomTarget.Add(item);
                                    }
                                    NotifyOfPropertyChange(() => AllHisConsultationRoomTarget);
                                    NotifyOfPropertyChange(() => AllHisTempConsultationRoomTarget);
                                }
                            }
                            catch (Exception ex)
                            {
                                IsLoading = false;
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
                //selectedConsultationRoomTarget=new ConsultationRoomTarget();
                
                CurRefDepartmentsTree= (RefDepartmentsTree)Obj.curDeptLoc;
                GetConsultationRoomTargetTSegment(CurRefDepartmentsTree.NodeID);
                GetHisConsultationRoomTargetTSegment(CurRefDepartmentsTree.NodeID);
                selectedConsultTimeSeg = lstConsultationTimeSegments[0];
            }
        }



#endregion

#region   animator

        Point midRec=new Point(0,0);

        public Grid LayoutRoot { get; set; }
        public StackPanel ChildRec { get; set; }
        public TranslateTransform RecTranslateTransform { get; set; }
        public void initGrid()
        {
            for (int i = 0; i < 3; i++)
            {
                LayoutRoot.RowDefinitions.Add(new RowDefinition());
            }
            for (int i = 0; i < 3; i++)
            {
                LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());
            }
        }
        public void removeGrid()
        {
            try
            {
                List<UIElement> removedItems = new List<UIElement>();
                foreach (UIElement child in LayoutRoot.Children)
                {
                    if (child is StackPanel)
                        removedItems.Add(child);
                }
                foreach (var removedItem in removedItems)
                {
                    LayoutRoot.Children.Remove(removedItem);
                }    
            }
            catch(Exception ex)
            {
                ClientLoggerHelper.LogInfo(ex.ToString());
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


        #region Số bắt đầu hẹn và kết thúc hẹn
        public void lnkDefaultStartSequenceNumberClick(object sender)
        {
            selectedConsultationRoomTarget.MondayStartSequenceNumber = selectedConsultationRoomTarget.TuesdayStartSequenceNumber
                = selectedConsultationRoomTarget.WednesdayStartSequenceNumber = selectedConsultationRoomTarget.ThursdayStartSequenceNumber
                = selectedConsultationRoomTarget.FridayStartSequenceNumber = selectedConsultationRoomTarget.SaturdayStartSequenceNumber = selectedConsultationRoomTarget.SundayStartSequenceNumber
                = selectedConsultationRoomTarget.SundayStartSequenceNumber
                = selectedConsultationRoomTarget.DefaultStartSequenceNumber;
            NotifyOfPropertyChange(() => selectedConsultationRoomTarget);
        }

        public void lnkDefaultEndSequenceNumberClick(object sender)
        {
            selectedConsultationRoomTarget.MondayEndSequenceNumber = selectedConsultationRoomTarget.TuesdayEndSequenceNumber
                = selectedConsultationRoomTarget.WednesdayEndSequenceNumber = selectedConsultationRoomTarget.ThursdayEndSequenceNumber
                = selectedConsultationRoomTarget.FridayEndSequenceNumber = selectedConsultationRoomTarget.SaturdayEndSequenceNumber = selectedConsultationRoomTarget.SundayEndSequenceNumber
                = selectedConsultationRoomTarget.SundayEndSequenceNumber
                = selectedConsultationRoomTarget.DefaultEndSequenceNumber;
            NotifyOfPropertyChange(() => selectedConsultationRoomTarget);
        }

        #endregion


        #region binding visibilty
        #endregion

        #endregion
    }
}
