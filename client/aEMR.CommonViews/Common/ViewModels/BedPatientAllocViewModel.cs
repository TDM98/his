using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.ViewContracts.Configuration;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.BaseModel;
/*
* 20200407 #001 TTM: BM 0027034: Không tắt busy tại màn hình chuyển khoa
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IBedPatientAlloc)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class BedPatientAllocViewModel : ViewModelBase, IBedPatientAlloc, IHandle<DeptLocBedSelectedEvent>
        , IHandle<BedAllocEvent>
        , IHandle<CloseBedPatientAllocViewEvent>
    //, IHandle<ItemSelected<IcwdBedPatientCommon, BedPatientAllocs>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public BedPatientAllocViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAgr, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAgr.Subscribe(this);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
            //▼===== #001
            if (eFireBookingBedEventTo == eFireBookingBedEvent.AcceptChangeDeptView)
            {
                this.HideBusyIndicator();
            }
            //▲===== #001
        }

        private IBedPatientAllocView _currentView;
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            _currentView = view as IBedPatientAllocView;
            _allBedPatientAllocs = new ObservableCollection<BedPatientAllocs>();
            InitData();
        }

        //Init Data
        private void InitData()
        {
            try
            {
                var DepartmentTreeVM = Globals.GetViewModel<IDeptTreeCommon>();
                UCDepartmentTree = DepartmentTreeVM;
                UCDepartmentTree.DefaultDepartment = DefaultDepartment;
                //UCDepartmentTree.LstRefDepartment = LstRefDepartment;

                if (isLoadAllDept)
                {
                    UCDepartmentTree.GetDeptLocTreeAll();
                }
                else
                {
                    //UCDepartmentTree.GetDeptLocTreeDeptID();
                    CurRefDepartmentsTree = new RefDepartmentsTree();
                    CurRefDepartmentsTree.Parent = new RefDepartmentsTree();
                    CurRefDepartmentsTree.Parent.NodeText = DefaultDepartment.DeptName;
                    if (SelectedDeptLocation != null && SelectedDeptLocation.DeptLocationID > 0)
                    {
                        CurRefDepartmentsTree.NodeText = SelectedDeptLocation.Location.LocationName;
                        GetAllBedPatientAllocByDeptID(SelectedDeptLocation.DeptLocationID);
                    }
                    else
                    {
                        CurRefDepartmentsTree.NodeText = DefaultDepartment.DeptLocations[0].Location.LocationName;
                        GetAllBedPatientAllocByDeptID(DefaultDepartment.DeptLocations[0].DeptLocationID);
                    }
                }
                //GetAllBedPatientAllocByDeptID(DefaultDepartment.DeptLocations[0].DeptLocationID);
                ActivateItem(DepartmentTreeVM);
                Globals.EventAggregator.Subscribe(this);

                if (selectedTempBedPatientAllocs == null)
                {
                    selectedTempBedPatientAllocs = new BedPatientAllocs();
                }
                selectedTempBedPatientAllocs.VPtRegistration = curPatientRegistration;
                //KMx: Ở trên Subscribe rồi (10/09/2014 14:02).
                //Globals.EventAggregator.Subscribe(this);
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogInfo(ex.ToString());
            }
        }

        //private ObservableCollection<long> _LstRefDepartment;
        //public ObservableCollection<long> LstRefDepartment
        //{
        //    get { return _LstRefDepartment; }
        //    set
        //    {
        //        if (_LstRefDepartment != value)
        //        {
        //            _LstRefDepartment = value;
        //            NotifyOfPropertyChange(() => LstRefDepartment);
        //        }
        //    }
        //}

        private bool _isShowPatientInfo = true;
        public bool IsShowPatientInfo
        {
            get { return _isShowPatientInfo; }
            set
            {
                if (_isShowPatientInfo != value)
                {
                    _isShowPatientInfo = value;
                    NotifyOfPropertyChange(() => IsShowPatientInfo);
                }
            }
        }

        private bool _IsReSelectBed = false;
        public bool IsReSelectBed
        {
            get { return _IsReSelectBed; }
            set
            {
                if (_IsReSelectBed != value)
                {
                    _IsReSelectBed = value;
                    NotifyOfPropertyChange(() => IsReSelectBed);
                }
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
                Globals.EventAggregator.Publish(new DeptLocSelectedEvent() { curDeptLoc = CurRefDepartmentsTree });
            }

        }

        private RefDepartment _responsibleDepartment;
        public RefDepartment ResponsibleDepartment
        {
            get { return _responsibleDepartment; }
            set { _responsibleDepartment = value; }
        }

        private DeptLocation _SelectedDeptLocation;
        public DeptLocation SelectedDeptLocation
        {
            get { return _SelectedDeptLocation; }
            set { _SelectedDeptLocation = value; }
        }

        private RefDepartment _defaultDepartment;
        public RefDepartment DefaultDepartment
        {
            get { return _defaultDepartment; }
            set
            {
                if (_defaultDepartment != value)
                {
                    _defaultDepartment = value;
                    NotifyOfPropertyChange(() => DefaultDepartment);

                    if (UCDepartmentTree != null)
                    {
                        UCDepartmentTree.DefaultDepartment = _defaultDepartment;
                    }
                }
            }
        }

        private long _deptID;
        public long deptID
        {
            get
            {
                return _deptID;
            }
            set
            {
                if (_deptID == value)
                    return;
                _deptID = value;
                NotifyOfPropertyChange(() => deptID);
            }
        }

        private int FindPatient = (int)AllLookupValues.V_FindPatientType.NOI_TRU;

        private bool _isLoadAllDept;
        public bool isLoadAllDept
        {
            get
            {
                return _isLoadAllDept;
            }
            set
            {
                if (_isLoadAllDept == value)
                    return;
                _isLoadAllDept = value;
                NotifyOfPropertyChange(() => isLoadAllDept);
            }
        }

        private IDeptTreeCommon _UCDepartmentTree;
        public IDeptTreeCommon UCDepartmentTree
        {
            get { return _UCDepartmentTree; }
            set
            {
                _UCDepartmentTree = value;
                NotifyOfPropertyChange(() => UCDepartmentTree);
            }
        }

        private ObservableCollection<BedPatientAllocs> _allBedPatientAllocs;
        public ObservableCollection<BedPatientAllocs> allBedPatientAllocs
        {
            get
            {
                return _allBedPatientAllocs;
            }
            set
            {
                if (_allBedPatientAllocs == value)
                    return;
                _allBedPatientAllocs = value;
                NotifyOfPropertyChange(() => allBedPatientAllocs);

            }
        }

        private BedPatientAllocs _selectedTempBedPatientAllocs;
        public BedPatientAllocs selectedTempBedPatientAllocs
        {
            get
            {
                return _selectedTempBedPatientAllocs;
            }
            set
            {
                if (_selectedTempBedPatientAllocs == value)
                    return;
                _selectedTempBedPatientAllocs = value;
                NotifyOfPropertyChange(() => selectedTempBedPatientAllocs);
            }
        }

        private Patient _PatientInfo;
        public Patient PatientInfo
        {
            get { return _PatientInfo; }
            set
            {
                if (_PatientInfo != value)
                {
                    _PatientInfo = value;
                }
            }
        }

        private static long _PtRegistrationID;
        public static long PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                if (_PtRegistrationID != value)
                {
                    _PtRegistrationID = value;
                }
            }
        }

        private PatientRegistration _curPatientRegistration;
        public PatientRegistration curPatientRegistration
        {
            get
            {
                return _curPatientRegistration;
            }
            set
            {
                if (_curPatientRegistration == value)
                    return;
                _curPatientRegistration = value;
                NotifyOfPropertyChange(() => curPatientRegistration);
            }
        }

        private InPatientDeptDetail _InPtDeptDetail;
        public InPatientDeptDetail InPtDeptDetail
        {
            get { return _InPtDeptDetail; }
            set
            {
                if (_InPtDeptDetail != value)
                {
                    _InPtDeptDetail = value;
                    NotifyOfPropertyChange(() => InPtDeptDetail);
                }
            }
        }

        private int minHeight = 60;
        private int minWidth = 250;
        private int maxHeight = 100;
        private int maxWidth = 300;

        private int mColumn = 4;

        public void initBedFormEx()
        {
            int Row = 0;
            int Column = 0;
            Grid GridBedPatientAlloc = new Grid();

            int mRow = (int)Math.Ceiling(allBedPatientAllocs.Count / (float)mColumn);//(int)Math.Ceiling((count / (float)mColumn));
            for (int i = 0; i < mRow; i++)
            {
                RowDefinition rd = new RowDefinition();
                rd.MinHeight = maxHeight;
                GridBedPatientAlloc.RowDefinitions.Add(rd);
            }

            GridBedPatientAlloc.RowDefinitions.Add(new RowDefinition());//luc nao cung de dong trong o duoi,de tu dong day Grid len tren
            for (int i = 0; i < mColumn; i++)
            {
                GridBedPatientAlloc.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < allBedPatientAllocs.Count; i++)
            {
                BedPatientAllocs bpa = allBedPatientAllocs[i];
                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;
                sp.Width = minWidth;
                sp.Height = minHeight;
                sp.MouseMove += new MouseEventHandler(sp_MouseMove);
                sp.MouseLeave += new MouseEventHandler(sp_MouseLeave);

                StackPanel spPar = new StackPanel();
                spPar.Orientation = Orientation.Vertical;
                spPar.Width = 50;
                spPar.Height = 65;

                StackPanel spName = new StackPanel();
                spName.Orientation = Orientation.Vertical;
                spName.Width = 180;
                spName.Height = 60;

                if (bpa.IsActive == false)
                {
                    Image image = new Image();
                    Uri uri = new Uri("/aEMR.CommonViews;component/Assets/Images/Bed6.jpg", UriKind.Relative);
                    ImageSource img = new BitmapImage(uri);
                    image.SetValue(Image.SourceProperty, img);
                    spPar.Children.Add(image);
                    spPar.MouseLeftButtonUp += new MouseButtonEventHandler(sp_MouseLeftButtonUp);
                    spPar.DataContext = bpa;
                }
                else
                {
                    //StackPanel sp1 = new StackPanel();
                    //sp1.Width = 50;
                    //sp1.Height = 65;
                    //sp1.DataContext = bpa;
                    Image image1 = new Image();
                    Uri uri = new Uri("/aEMR.CommonViews;component/Assets/Images/Bed4.png", UriKind.Relative);
                    ImageSource img = new BitmapImage(uri);
                    image1.SetValue(Image.SourceProperty, img);
                    //sp1.Children.Add(image1);

                    spPar.Children.Add(image1);
                    //spPar.MouseLeftButtonUp += new MouseButtonEventHandler(sp_MouseLeftButtonUp);
                    spPar.DataContext = bpa;
                    //Kiem tra co 2 nguoi nam mot giuong ko
                    if (i < allBedPatientAllocs.Count - 1
                        && bpa.BedAllocationID == allBedPatientAllocs[i + 1].BedAllocationID)
                    {
                        BedPatientAllocs bpaEx = allBedPatientAllocs[i + 1];
                        i++;
                        GetPatientInfoData(bpaEx, spName);
                        GetPatientInfoData(bpa, spName);
                    }
                    else
                    {
                        //spPar.DataContext = bpa;
                        //if (bpa.DischargeDate < DateTime.Now)
                        //{
                        //    spPar.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 125, 125));
                        //}
                        GetPatientInfoData(bpa, spName);
                        if (bpa.PtRegistrationID == 0)
                        {
                            Button hplkDG = new Button();
                            hplkDG.FontSize = 11;
                            hplkDG.Click += new RoutedEventHandler(hplkDG_Click);
                            hplkDG.Content = eHCMSResources.K3103_G1_DatGiuong;
                            hplkDG.DataContext = bpa;
                            hplkDG.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 0, 102));
                            string toolText = eHCMSResources.K2385_G1_ClkDeDatGiuongChoBN;
                            ToolTipService.SetToolTip(hplkDG, toolText);
                            spName.Children.Add(hplkDG);
                        }
                    }
                }
                TextBlock tb = new TextBlock();
                tb.Text = bpa.VBedAllocation.BedNumber;
                spPar.Children.Add(tb);

                sp.Children.Add(spPar);
                spPar.MouseMove += new MouseEventHandler(spPar_MouseMove);
                spPar.MouseLeave += new MouseEventHandler(spPar_MouseLeave);
                sp.Children.Add(spName);

                ((Grid)GridBedPatientAlloc).Children.Add(sp);
                Grid.SetRow(sp, Row);
                Grid.SetColumn(sp, Column);
                Column++;
                if (Column == mColumn)
                {
                    Column = 0;
                    Row++;
                }
            }

            if (_currentView != null)
            {
                _currentView.LoadGrid(GridBedPatientAlloc);
            }
        }

        void spPar_MouseLeave(object sender, MouseEventArgs e)
        {
            ((StackPanel)sender).Width = 50;
            ((StackPanel)sender).Height = 65;
        }

        void spPar_MouseMove(object sender, MouseEventArgs e)
        {
            ((StackPanel)sender).Width = 70;
            ((StackPanel)sender).Height = 70;
        }

        //KMx: Chỉ được xem, không được đặt giường (05/09/2014 16:53).
        private bool _isReadOnly;
        public bool IsReadOnly
        {
            get
            {
                return _isReadOnly;
            }
            set
            {
                if (_isReadOnly == value)
                    return;
                _isReadOnly = value;
                NotifyOfPropertyChange(() => IsReadOnly);
            }
        }

        void hplkDG_Click(object sender, RoutedEventArgs e)
        {
            if (IsReadOnly)
            {
                MessageBox.Show(eHCMSResources.A0178_G1_Msg_InfoDangXemGiuongKhongCSua, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            var cwdBedPatientVM = Globals.GetViewModel<IcwdBedPatientCommon>();

            cwdBedPatientVM.InPtDeptDetail = InPtDeptDetail;

            BedPatientAllocs bpa = new BedPatientAllocs();
            bpa = (BedPatientAllocs)((Button)sender).DataContext;
            cwdBedPatientVM.isDelete = false;
            cwdBedPatientVM.IsEdit = false;

            cwdBedPatientVM.selectedBedPatientAllocs = bpa;
            if (cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation == null)
                cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation = new BedAllocation();

            cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation = new DeptLocation();
            cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.DeptLocationID = CurRefDepartmentsTree.NodeID;
            cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.RefDepartment = new RefDepartment();
            cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.Location = new Location();
            cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.RefDepartment.DeptID = CurRefDepartmentsTree.Parent.NodeID;
            cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.RefDepartment.DeptName = CurRefDepartmentsTree.Parent.NodeText;
            cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.Location.LocationName = CurRefDepartmentsTree.NodeText;
            cwdBedPatientVM.selectedTempBedPatientAllocs = selectedTempBedPatientAllocs;
            cwdBedPatientVM.selectedBedPatientAllocs.ResponsibleDepartment =
               ResponsibleDepartment;// cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.RefDepartment;
            if (cwdBedPatientVM.selectedBedPatientAllocs.PtRegistrationID ==
                selectedTempBedPatientAllocs.VPtRegistration.PtRegistrationID)
            {
                MessageBox.Show(eHCMSResources.A0243_G1_Msg_InfoBNDangNamGiuong);
                return;
            }
            cwdBedPatientVM.IsReSelectBed = IsReSelectBed;
            ActivateItem(cwdBedPatientVM);

            if (cwdBedPatientVM != null)
            {
                //KMx: Đổi biến BookBedAllocOnly thành eFireBookingBedEventTo để bắn event (06/09/2014 17:20).
                //cwdBedPatientVM.BookBedAllocOnly = BookBedAllocOnly;
                cwdBedPatientVM.eFireBookingBedEventTo = eFireBookingBedEventTo;
                //var instance = cwdBedPatientVM as Conductor<object>;
                //Globals.ShowDialog(instance, (o) => { });

                GlobalsNAV.ShowDialog<IcwdBedPatientCommon>();
            }
        }

        void sp_MouseLeave(object sender, MouseEventArgs e)
        {
            ((StackPanel)sender).Width = minWidth;
            ((StackPanel)sender).Height = minHeight;
            //isTooltip = false;
            //tooltip = "";
        }

        void sp_MouseMove(object sender, MouseEventArgs e)
        {
            ((StackPanel)sender).Width = maxWidth;
            ((StackPanel)sender).Height = maxHeight;

        }

        void spChild_MouseLeave(object sender, MouseEventArgs e)
        {
            ((StackPanel)sender).Width = 55;
            ((StackPanel)sender).Height = 25;

        }

        void spChild_MouseMove(object sender, MouseEventArgs e)
        {
            ((StackPanel)sender).Width = 80;
            ((StackPanel)sender).Height = 30;

        }

        void sp_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsReadOnly)
            {
                MessageBox.Show(eHCMSResources.A0178_G1_Msg_InfoDangXemGiuongKhongCSua, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            var cwdBedPatientVM = Globals.GetViewModel<IcwdBedPatientCommon>();

            cwdBedPatientVM.InPtDeptDetail = InPtDeptDetail;

            BedPatientAllocs bpa = new BedPatientAllocs();
            bpa = (BedPatientAllocs)((StackPanel)sender).DataContext;
            bpa.BedAllocationID = bpa.VBedAllocation.BedAllocationID;
            //cwdBedPatientVM.BookOnly = (curPatientRegistration == null) || curPatientRegistration.PtRegistrationID == 0;

            //KMx: Đổi biến BookBedAllocOnly thành eFireBookingBedEventTo để bắn event (06/09/2014 17:20).
            //cwdBedPatientVM.BookBedAllocOnly = BookBedAllocOnly;
            cwdBedPatientVM.eFireBookingBedEventTo = eFireBookingBedEventTo;

            //cwdBedPatientVM.ResponsibleDepartment = ResponsibleDepartment;
            cwdBedPatientVM.ResponsibleDepartment = ResponsibleDepartment;

            cwdBedPatientVM.isDelete = false;
            cwdBedPatientVM.IsEdit = false;
            cwdBedPatientVM.selectedBedPatientAllocs = bpa;
            if (cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation == null)
                cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation = new BedAllocation();
            if (cwdBedPatientVM.CheckInDateTime != null)
            {
                cwdBedPatientVM.CheckInDateTime.DateTime = Globals.GetCurServerDateTime();
            }
            cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation = new DeptLocation();
            cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.DeptLocationID = CurRefDepartmentsTree.NodeID;
            cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.RefDepartment = new RefDepartment();
            cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.Location = new Location();
            cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.RefDepartment.DeptID = CurRefDepartmentsTree.Parent.NodeID;
            cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.RefDepartment.DeptName = CurRefDepartmentsTree.Parent.NodeText;
            cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.Location.LocationName = CurRefDepartmentsTree.NodeText;
            cwdBedPatientVM.selectedTempBedPatientAllocs = selectedTempBedPatientAllocs;
            cwdBedPatientVM.selectedBedPatientAllocs.ResponsibleDepartment = ResponsibleDepartment;
            cwdBedPatientVM.IsReSelectBed = IsReSelectBed;
            DeptMedServiceItemsSearchCriteria SearchCriteria = new DeptMedServiceItemsSearchCriteria
            {
                DeptID = ResponsibleDepartment.DeptID
            };
            cwdBedPatientVM.GetBedAllocationAll_ByDeptID(SearchCriteria);
            if (cwdBedPatientVM.selectedBedPatientAllocs.PtRegistrationID ==
                selectedTempBedPatientAllocs.VPtRegistration.PtRegistrationID)
            {
                MessageBox.Show(eHCMSResources.A0243_G1_Msg_InfoBNDangNamGiuong);
                return;
            }

            ActivateItem(cwdBedPatientVM);

            //var instance = cwdBedPatientVM as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });
            GlobalsNAV.ShowDialog<IcwdBedPatientCommon>();

        }
        void spDelete_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var cwdBedPatientVM = Globals.GetViewModel<IcwdBedPatientCommon>();
            BedPatientAllocs bpa = new BedPatientAllocs();

            cwdBedPatientVM.InPtDeptDetail = InPtDeptDetail;

            bpa = (BedPatientAllocs)((StackPanel)sender).DataContext;
            cwdBedPatientVM.isDelete = true;
            cwdBedPatientVM.IsEdit = false;
            cwdBedPatientVM.selectedBedPatientAllocs = bpa;
            if (cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation == null)
                cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation = new BedAllocation();

            cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation = new DeptLocation();
            cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.DeptLocationID = CurRefDepartmentsTree.NodeID;
            cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.RefDepartment = new RefDepartment();
            cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.Location = new Location();
            cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.RefDepartment.DeptID = CurRefDepartmentsTree.Parent.NodeID;
            cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.RefDepartment.DeptName = CurRefDepartmentsTree.Parent.NodeText;
            cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.Location.LocationName = CurRefDepartmentsTree.NodeText;
            cwdBedPatientVM.selectedTempBedPatientAllocs = selectedTempBedPatientAllocs;
            cwdBedPatientVM.selectedBedPatientAllocs.ResponsibleDepartment = ResponsibleDepartment;
            ActivateItem(cwdBedPatientVM);

            //var instance = cwdBedPatientVM as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });
            GlobalsNAV.ShowDialog<IcwdBedPatientCommon>();

        }
        public void Handle(DeptLocBedSelectedEvent obj)
        {
            if (obj != null)
            {
                CurRefDepartmentsTree = ((RefDepartmentsTree)obj.curDeptLoc);
                GetAllBedPatientAllocByDeptID(CurRefDepartmentsTree.NodeID);
                //GetAllBedPatientAllocByDeptID(DefaultDepartment.DeptLocations[0].DeptLocationID);
            }
        }

        public void butSave()
        {

        }
        public void butExit()
        {
            TryClose();
        }
        public void Handle(BedAllocEvent obj)
        {
            if (obj != null)
            {
                if (SelectedDeptLocation != null && SelectedDeptLocation.DeptLocationID > 0)
                {
                    GetAllBedPatientAllocByDeptID(SelectedDeptLocation.DeptLocationID);
                }
                else
                {
                    GetAllBedPatientAllocByDeptID(DefaultDepartment.DeptLocations[0].DeptLocationID);
                }
            }
        }

        #region method
        private void GetAllBedPatientAllocByDeptID(long DeptLocationID)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                using (var serviceFactory = new BedAllocationsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllBedPatientAllocByDeptID(DeptLocationID, IsReadOnly
                        , Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int total = 0;
                            var results = contract.EndGetAllBedPatientAllocByDeptID(out total, asyncResult);
                            if (results != null)
                            {
                                if (allBedPatientAllocs == null)
                                {
                                    allBedPatientAllocs = new ObservableCollection<BedPatientAllocs>();
                                }
                                else
                                {
                                    allBedPatientAllocs.Clear();
                                }
                                foreach (var p in results)
                                {
                                    allBedPatientAllocs.Add(p);
                                }
                                NotifyOfPropertyChange(() => allBedPatientAllocs);
                                initBedFormEx();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
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
                MessageBox.Show(ex.Message);
                this.DlgHideBusyIndicator();
            }
        });

            t.Start();
        }

        private void GetPatientInfo(long RegistrationID, StackPanel sender)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetPatientInfoByPtRegistration(RegistrationID, null, FindPatient, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                PatientInfo = contract.EndGetPatientInfoByPtRegistration(asyncResult);
                                if (PatientInfo == null)
                                {
                                    PatientInfo = new Patient();
                                }
                                Button hplkName = new Button();
                                hplkName.Click += new RoutedEventHandler(hplkName_Click);
                                hplkName.FontSize = 10;
                                hplkName.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 53, 149, 203));

                                hplkName.Content = PatientInfo.FullName;

                                TextBlock tbID = new TextBlock();
                                tbID.Text = PatientInfo.PatientCode;
                                tbID.FontSize = 8;

                                sender.Children.Add(hplkName);
                                sender.Children.Add(tbID);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
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
                    MessageBox.Show(ex.Message);
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }

        private void GetPatientInfoData(BedPatientAllocs bpa, StackPanel sender)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetPatientInfoByPtRegistration(bpa.PtRegistrationID, null, FindPatient
                            , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                PatientInfo = contract.EndGetPatientInfoByPtRegistration(asyncResult);
                                if (PatientInfo == null)
                                {
                                    PatientInfo = new Patient();
                                }
                            //HyperlinkButton hplkName = new HyperlinkButton();
                            //hplkName.Click += new RoutedEventHandler(hplkName_Click);
                            //hplkName.FontSize = 10;
                            //hplkName.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 53, 149, 203));
                            //hplkName.DataContext = bpa;
                            //hplkName.Content = PatientInfo.FullName;
                            //string toolText= eHCMSResources.K2386_G1_ClkDeTraGiuongChoBNNay;
                            //ToolTipService.SetToolTip(hplkName, toolText);

                            TextBlock hplkName = new TextBlock();
                                hplkName.FontSize = 10;
                                hplkName.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 53, 149, 203));
                                hplkName.DataContext = bpa;
                                hplkName.Text = PatientInfo.FullName;


                                TextBlock tbID = new TextBlock();
                                tbID.Text = PatientInfo.PatientCode;
                                tbID.FontSize = 8;

                                sender.Children.Add(hplkName);
                                sender.Children.Add(tbID);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
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
                    MessageBox.Show(ex.Message);
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }

        void hplkName_Click(object sender, RoutedEventArgs e)
        {
            if (IsReadOnly)
            {
                MessageBox.Show(eHCMSResources.A0178_G1_Msg_InfoDangXemGiuongKhongCSua, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            var cwdBedPatientVM = Globals.GetViewModel<IcwdBedPatientCommon>();

            cwdBedPatientVM.InPtDeptDetail = InPtDeptDetail;

            BedPatientAllocs bpa = new BedPatientAllocs();
            bpa = (BedPatientAllocs)((Button)sender).DataContext;
            cwdBedPatientVM.isDelete = true;
            cwdBedPatientVM.IsEdit = false;
            cwdBedPatientVM.selectedBedPatientAllocs = bpa;
            if (cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation == null)
                cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation = new BedAllocation();

            cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation = new DeptLocation();
            cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.DeptLocationID = CurRefDepartmentsTree.NodeID;
            cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.RefDepartment = new RefDepartment();
            cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.Location = new Location();
            cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.RefDepartment.DeptID = CurRefDepartmentsTree.Parent.NodeID;
            cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.RefDepartment.DeptName = CurRefDepartmentsTree.Parent.NodeText;
            cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.Location.LocationName = CurRefDepartmentsTree.NodeText;
            cwdBedPatientVM.selectedTempBedPatientAllocs = selectedTempBedPatientAllocs;
            cwdBedPatientVM.selectedBedPatientAllocs.ResponsibleDepartment = ResponsibleDepartment;

            cwdBedPatientVM.selectedTempBedPatientAllocs = selectedTempBedPatientAllocs;
            if (cwdBedPatientVM.selectedBedPatientAllocs.PtRegistrationID ==
                selectedTempBedPatientAllocs.VPtRegistration.PtRegistrationID)
            {
                MessageBox.Show(eHCMSResources.A0243_G1_Msg_InfoBNDangNamGiuong);
                return;
            }
            this.ActivateItem(cwdBedPatientVM);

            //var instance = cwdBedPatientVM as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });
            GlobalsNAV.ShowDialog<IcwdBedPatientCommon>();
        }
        #endregion

        //KMx: BookBedAllocOnly = true: Khi bấm đặt giường là lưu xuống Database luôn, ngược lại thì khi form cha lưu thì mới lưu giường sau (05/09/2014 17:11). 
        private bool _bookBedAllocOnly;
        public bool BookBedAllocOnly
        {
            get { return _bookBedAllocOnly; }
            set { _bookBedAllocOnly = value; }
        }

        //KMx: NONE: Đặt giường là lưu xuống Database luôn. Khác: Bắn event về cho parent, khi nào parent lưu thì mới lưu giường (06/09/2014 16:56).
        private eFireBookingBedEvent _eFireBookingBedEventTo;
        public eFireBookingBedEvent eFireBookingBedEventTo
        {
            get { return _eFireBookingBedEventTo; }
            set
            {
                if (_eFireBookingBedEventTo != value)
                {
                    _eFireBookingBedEventTo = value;
                    NotifyOfPropertyChange(() => eFireBookingBedEventTo);
                }
            }
        }


        //public void Handle(ItemSelected<IcwdBedPatientCommon, BedPatientAllocs> message)
        //{
        //    if (this.GetView() != null && message != null)
        //    {
        //        if (this.BookBedAllocOnly)
        //        {
        //            TryClose();
        //        }
        //    }
        //}

        public void Handle(CloseBedPatientAllocViewEvent message)
        {
            if (this.GetView() != null)
            {
                TryClose();
            }
        }
    }
}
