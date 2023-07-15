using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using DataEntities;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.Configuration.BedAllocations.ViewModels
{
    [Export(typeof(ICfBedPatientAlloc)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class BedPatientAllocViewModel : Conductor<object>, ICfBedPatientAlloc, IHandle<DeptLocBedSelectedEvent>
        ,IHandle<BedAllocEvent>
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
        public BedPatientAllocViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);

            _allBedPatientAllocs = new ObservableCollection<BedPatientAllocs>();
        }
        protected override void OnActivate()
        {
            base.OnActivate();

            var DepartmentTreeVM = Globals.GetViewModel<IDeptTree>();
            UCDepartmentTree = DepartmentTreeVM;
            this.ActivateItem(DepartmentTreeVM);
            Globals.EventAggregator.Subscribe(this);

            if (selectedTempBedPatientAllocs == null)
            {
                selectedTempBedPatientAllocs = new BedPatientAllocs();
            }
            selectedTempBedPatientAllocs.VPtRegistration = new PatientRegistration();
            selectedTempBedPatientAllocs.VPtRegistration.Patient = new Patient();
            selectedTempBedPatientAllocs.VPtRegistration.PtRegistrationID = curPatientRegistration.PtRegistrationID;
            selectedTempBedPatientAllocs.VPtRegistration.FullName = curPatientRegistration.Patient.FullName;
            //selectedTempBedPatientAllocs.VPtRegistration.Patient.Gender = AppManager.PatientInfo.Gender;
            selectedTempBedPatientAllocs.VPtRegistration.PatientCode = curPatientRegistration.Patient.PatientCode;
            Globals.EventAggregator.Subscribe(this);
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
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

        private object _UCDepartmentTree;
        public object UCDepartmentTree
        {
            get { return _UCDepartmentTree; }
            set
            {
                _UCDepartmentTree = value;
                NotifyOfPropertyChange(() => UCDepartmentTree);
            }
        }
        private object _GridBedPatientAlloc;
        public object GridBedPatientAlloc
        {
            get
            {
                return _GridBedPatientAlloc;
            }
            set
            {
                if (_GridBedPatientAlloc == value)
                    return;
                _GridBedPatientAlloc = value;
                NotifyOfPropertyChange(() => GridBedPatientAlloc);
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

        private string _tooltip = "";
        public string tooltip
        {
            get
            {
                return _tooltip;
            }
            set
            {
                if (_tooltip == value)
                    return;
                _tooltip = value;
                NotifyOfPropertyChange(() => tooltip);
            }
        }

        public void GrdLoaded(object sender, RoutedEventArgs e)
        {
            GridBedPatientAlloc = sender as Grid;
            for (int i = 0; i < 5; i++)
            {
                ((Grid)GridBedPatientAlloc).RowDefinitions.Add(new RowDefinition());
            }
            for (int i = 0; i < 7; i++)
            {
                ((Grid)GridBedPatientAlloc).ColumnDefinitions.Add(new ColumnDefinition());
            }
        }
        public void initBedFormEx()
        {
            int Row = 1;
            int Column = 0;
            ((Grid)GridBedPatientAlloc).Children.Clear();
            for (int i = 0; i < allBedPatientAllocs.Count;i++ )
                //foreach (BedPatientAllocs bpa in allBedPatientAllocs)
            {
                BedPatientAllocs bpa = allBedPatientAllocs[i];
                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Vertical;
                sp.Width = 50;
                sp.Height = 55;
                sp.Orientation = Orientation.Vertical;
                
                
                if (bpa.IsActive == false)
                {
                    Image image = new Image();
                    Uri uri =new Uri("/eHCMSCal;component/Assets/Images/Bed6.jpg", UriKind.Relative);
                    ImageSource img = new BitmapImage(uri);
                    image.SetValue(Image.SourceProperty, img);
                    sp.Children.Add(image);
                    sp.MouseLeftButtonUp += new MouseButtonEventHandler(sp_MouseLeftButtonUp);
                    sp.DataContext = bpa;
                    sp.MouseMove += new MouseEventHandler(sp_MouseMove);
                    sp.MouseLeave += new MouseEventHandler(sp_MouseLeave);
                    
                }
                else
                {
                    //Kiem tra co 2 nguoi nam mot giuong ko
                    if (i < allBedPatientAllocs.Count-1
                        && bpa.BedAllocationID == allBedPatientAllocs[i + 1].BedAllocationID)
                    {
                        BedPatientAllocs bpaEx = allBedPatientAllocs[i+1];
                        i++;

                        StackPanel sp1 = new StackPanel();
                        sp1.Width = 50;
                        sp1.Height = 25;
                        sp1.MouseMove += new MouseEventHandler(spChild_MouseMove);
                        sp1.MouseLeave += new MouseEventHandler(spChild_MouseLeave);
                        sp1.MouseLeftButtonUp += new MouseButtonEventHandler(spDelete_MouseLeftButtonUp);
                        
                        sp1.DataContext = bpa;
                        Image image1 = new Image();
                        Uri uri = new Uri("/eHCMSCal;component/Assets/Images/Bed4.png", UriKind.Relative);
                        ImageSource img = new BitmapImage(uri);
                        image1.SetValue(Image.SourceProperty, img);
                        sp1.Children.Add(image1);

                        StackPanel sp2 = new StackPanel();
                        sp2.Width = 50;
                        sp2.Height = 25;
                        sp2.MouseMove += new MouseEventHandler(spChild_MouseMove);
                        sp2.MouseLeave += new MouseEventHandler(spChild_MouseLeave);
                        sp2.MouseLeftButtonUp += new MouseButtonEventHandler(spDelete_MouseLeftButtonUp);
                        
                        sp2.DataContext = bpaEx;

                        Image image2 = new Image();
                        uri = new Uri("/eHCMSCal;component/Assets/Images/Bed4.png", UriKind.Relative);
                        ImageSource img2 = new BitmapImage(uri);
                        image2.SetValue(Image.SourceProperty, img2);
                        sp2.Children.Add(image2);

                        sp.Width = 90;
                        sp.Height = 70;
                        sp.Children.Add(sp1);
                        sp.Children.Add(sp2);

                    }else
                    {
                        Image image = new Image();
                        Uri uri = new Uri("/eHCMSCal;component/Assets/Images/Bed4.png", UriKind.Relative);
                        ImageSource img = new BitmapImage(uri);

                        image.SetValue(Image.SourceProperty, img);
                        sp.Children.Add(image);
                        sp.MouseLeftButtonUp += new MouseButtonEventHandler(sp_MouseLeftButtonUp);
                        
                        sp.MouseMove += new MouseEventHandler(sp_MouseMove);
                        sp.MouseLeave += new MouseEventHandler(sp_MouseLeave);
                        
                        sp.DataContext = bpa;
                        //if (bpa.DischargeDate < DateTime.Now)
                        //{
                        //    sp.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 125, 125));
                        //}    
                    }
                    
                }
                TextBlock tb = new TextBlock();
                tb.Text = bpa.VBedAllocation.BedNumber;
                sp.Children.Add(tb);
                
                ((Grid)GridBedPatientAlloc).Children.Add(sp);
                Grid.SetRow(sp, Row);
                Grid.SetColumn(sp, Column);
                Column++;
                if (Column == 7)
                {
                    Column = 0;
                    Row++;
                }
            }
        }

        public void initBedForm()
        {
            int Row = 1;
            int Column = 0;
            ((Grid)GridBedPatientAlloc).Children.Clear();

            foreach (BedPatientAllocs bpa in allBedPatientAllocs)
            {
                Image image = new Image();
                Uri uri = null;
                StackPanel sp = new StackPanel();

                sp.Width = 50;
                sp.Height = 55;
                sp.Orientation = Orientation.Vertical;
                sp.MouseMove += new MouseEventHandler(sp_MouseMove);
                sp.MouseLeave += new MouseEventHandler(sp_MouseLeave);
                sp.MouseLeftButtonUp += new MouseButtonEventHandler(sp_MouseLeftButtonUp);
                sp.DataContext = bpa;
                if (bpa.IsActive == false)
                {
                    uri = new Uri("/eHCMSCal;component/Assets/Images/Bed6.jpg", UriKind.Relative);
                }
                else
                {
                    uri = new Uri("/eHCMSCal;component/Assets/Images/Bed4.png", UriKind.Relative);
                    //if (bpa.DischargeDate < DateTime.Now)
                    //{
                    //    sp.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 125, 125));
                    //}
                    
                }
                TextBlock tb = new TextBlock();
                tb.Text = bpa.VBedAllocation.BedNumber;
                ImageSource img = new BitmapImage(uri);
                image.SetValue(Image.SourceProperty, img);
                //
                sp.Children.Add(image);
                sp.Children.Add(tb);

                ((Grid)GridBedPatientAlloc).Children.Add(sp);
                Grid.SetRow(sp, Row);
                Grid.SetColumn(sp, Column);
                Column++;
                if (Column == 7)
                {
                    Column = 0;
                    Row++;
                }
            }
        }
        void sp_MouseLeave(object sender, MouseEventArgs e)
        {
            ((StackPanel)sender).Width = 55;
            ((StackPanel)sender).Height = 55;
            isTooltip = false;
            tooltip = "";
        }

        void sp_MouseMove(object sender, MouseEventArgs e)
        {
            ((StackPanel)sender).Width = 70;
            ((StackPanel)sender).Height = 70;
            
            if(isTooltip == false)
            {
                isTooltip = true;
                BedPatientAllocs bpa = new BedPatientAllocs();
                bpa = (BedPatientAllocs)((StackPanel)sender).DataContext;
                GetPatientInfo(bpa.PtRegistrationID, (StackPanel)sender);
            }
        }
        void spChild_MouseLeave(object sender, MouseEventArgs e)
        {
            ((StackPanel)sender).Width = 55;
            ((StackPanel)sender).Height = 25;
            isTooltip = false;
            tooltip = "";
        }

        private bool isTooltip = false;
        void spChild_MouseMove(object sender, MouseEventArgs e)
        {
            ((StackPanel)sender).Width = 80;
            ((StackPanel)sender).Height = 30;

            if (isTooltip == false)
            {
                isTooltip = true;
                BedPatientAllocs bpa = new BedPatientAllocs();
                bpa = (BedPatientAllocs)((StackPanel)sender).DataContext;
                GetPatientInfo(bpa.PtRegistrationID,(StackPanel)sender);
            }
        }
        void sp_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //var cwdBedPatientVM = Globals.GetViewModel<IcwdBedPatient>();
            //BedPatientAllocs bpa = new BedPatientAllocs();
            //bpa = (BedPatientAllocs)((StackPanel)sender).DataContext;
            //cwdBedPatientVM.isDelete = false;
            //cwdBedPatientVM.selectedBedPatientAllocs = bpa;
            //if (cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation == null) 
            //    cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation=new BedAllocation();
            
            //cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation = new DeptLocation();
            //cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.DeptLocationID = CurRefDepartmentsTree.NodeID;
            //cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.RefDepartment = new RefDepartment();
            //cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.Location = new Location();
            //cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.RefDepartment.DeptID = CurRefDepartmentsTree.Parent.NodeID;
            //cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.RefDepartment.DeptName = CurRefDepartmentsTree.Parent.NodeText;
            //cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.Location.LocationName = CurRefDepartmentsTree.NodeText;
            //cwdBedPatientVM.selectedTempBedPatientAllocs = selectedTempBedPatientAllocs;
            //this.ActivateItem(cwdBedPatientVM);
            
            //var instance = cwdBedPatientVM as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<IcwdBedPatient> onInitDlg = (cwdBedPatientVM) =>
            {
                BedPatientAllocs bpa = new BedPatientAllocs();
                bpa = (BedPatientAllocs)((StackPanel)sender).DataContext;
                cwdBedPatientVM.isDelete = false;
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
                this.ActivateItem(cwdBedPatientVM);

            };
            GlobalsNAV.ShowDialog<IcwdBedPatient>();

        }
        void spDelete_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //var cwdBedPatientVM = Globals.GetViewModel<IcwdBedPatient>();
            //BedPatientAllocs bpa = new BedPatientAllocs();
            //bpa = (BedPatientAllocs)((StackPanel)sender).DataContext;
            //cwdBedPatientVM.isDelete = true;
            //cwdBedPatientVM.selectedBedPatientAllocs = bpa;
            //if (cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation == null)
            //    cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation = new BedAllocation();

            //cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation = new DeptLocation();
            //cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.DeptLocationID = CurRefDepartmentsTree.NodeID;
            //cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.RefDepartment = new RefDepartment();
            //cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.Location = new Location();
            //cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.RefDepartment.DeptID = CurRefDepartmentsTree.Parent.NodeID;
            //cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.RefDepartment.DeptName = CurRefDepartmentsTree.Parent.NodeText;
            //cwdBedPatientVM.selectedBedPatientAllocs.VBedAllocation.VDeptLocation.Location.LocationName = CurRefDepartmentsTree.NodeText;
            //cwdBedPatientVM.selectedTempBedPatientAllocs = selectedTempBedPatientAllocs;
            //this.ActivateItem(cwdBedPatientVM);

            //var instance = cwdBedPatientVM as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });


            Action<IcwdBedPatient> onInitDlg = (cwdBedPatientVM) =>
            {
                BedPatientAllocs bpa = new BedPatientAllocs();
                bpa = (BedPatientAllocs)((StackPanel)sender).DataContext;
                cwdBedPatientVM.isDelete = true;
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
                this.ActivateItem(cwdBedPatientVM);

            };
            GlobalsNAV.ShowDialog<IcwdBedPatient>();

        }
        public void Handle(DeptLocBedSelectedEvent obj)
        {
            if (obj != null)
            {
                CurRefDepartmentsTree= ((RefDepartmentsTree)obj.curDeptLoc);
                GetAllBedPatientAllocByDeptID(CurRefDepartmentsTree.NodeID);
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
            if(obj!=null)
            {
                GetAllBedPatientAllocByDeptID(CurRefDepartmentsTree.NodeID);
            }
        }

        #region method
        private void GetAllBedPatientAllocByDeptID(long DeptLocationID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new BedAllocationsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllBedPatientAllocByDeptID(DeptLocationID, false
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
        private int FindPatient = (int)AllLookupValues.V_FindPatientType.NOI_TRU;
        private void GetPatientInfo(long RegistrationID,StackPanel sender)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new PatientRegistrationServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetPatientInfoByPtRegistration(RegistrationID,null,FindPatient, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            PatientInfo = contract.EndGetPatientInfoByPtRegistration(asyncResult);
                            if (PatientInfo==null)
                            {
                               PatientInfo=new Patient(); 
                            }
                            tooltip = string.Format("{0} : ", eHCMSResources.T0834_G1_TenBenhNhan) +PatientInfo.FullName;
                            tooltip += "\nMã Bệnh Nhân : " + PatientInfo.PatientCode;
                            ToolTipService.SetToolTip(sender, tooltip);
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
    }
}
