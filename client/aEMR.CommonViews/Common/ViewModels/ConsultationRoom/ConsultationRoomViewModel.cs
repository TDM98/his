using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
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
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using Castle.Core.Logging;
using DataEntities;
using eHCMSLanguage;
using aEMR.Common.BaseModel;

namespace aEMR.Common.ViewModels
{
    [Export(typeof (IConsultationRoom)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ConsultationRoomViewModel : ViewModelBase, IConsultationRoom
    {
        private readonly INavigationService _navigationService;
        private readonly IEventAggregator _eventAggregator;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ConsultationRoomViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            ResetFilter();
            GetDeptLocTreeViewFunction(((long)VRoomType.Khoa), (long)RoomFunction.KHAM_BENH);
            lstDeptLocation=new ObservableCollection<DeptLocation>();
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            if (lstDeptLocation.Count > 0)
            {
                GetAllRegisDeptLoc(lstDeptLocation);    
            }
            
        }
        public void butExit()
        {
            this.TryClose();
        }
        protected void ResetFilter()
        {
            SearchCriteria = new SeachPtRegistrationCriteria();
            SearchCriteria.RegStatus = -1;
            SearchCriteria.StaffID = -1;
            SearchCriteria.FromDate = Globals.ServerDate.Value;
            SearchCriteria.ToDate = Globals.ServerDate.Value;
        }

#region properties

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

        private ObservableCollection<RefDepartmentsTree> _allRefDepartmentsTree;
        public ObservableCollection<RefDepartmentsTree> allRefDepartmentsTree
        {
            get
            {
                return _allRefDepartmentsTree;
            }
            set
            {
                if (_allRefDepartmentsTree == value)
                    return;
                _allRefDepartmentsTree = value;
                NotifyOfPropertyChange(() => allRefDepartmentsTree);
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
#endregion

        #region method
        
        private void GetDeptLocTreeViewFunction(long DeptType, long RoomFunction)
        {
            // su dung ham nay ne RefDepartments_Tree(strV_DeptType, ShowDeptLocation);
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z1026_G1_DSRoomType });
            this.DlgShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetDeptLocationFunc(DeptType, RoomFunction
                        , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var items = contract.EndGetDeptLocationFunc(asyncResult);
                                if (items != null)
                                {
                                    allRefDepartmentsTree=new ObservableCollection<RefDepartmentsTree>();
                                    lstDeptLocation = new ObservableCollection<DeptLocation>();
                                    lstDeptLocInfo = new ObservableCollection<DeptLocInfo>();
                                    foreach (var Department in items)
                                    {
                                        lstDeptLocation.Add(Department);
                                        DeptLocInfo dt = new DeptLocInfo();
                                        dt.DeptLocationID = Department.DeptLocationID;
                                        dt.Location = new Location();
                                        dt.Location.LocationName = Department.Location.LocationName;
                                        dt.RefDepartment = new RefDepartment();
                                        dt.RefDepartment.DeptName = Department.RefDepartment.DeptName;
                                        lstDeptLocInfo.Add(dt);
                                    }
                                    GetAllRegisDeptLoc(lstDeptLocation);
                                }

                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                }


            });
            t.Start();
        }

        private void GetAllRegisDeptLoc(ObservableCollection<DeptLocation> lstDeptLocation)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0601_G1_DangTimDK) });
            this.DlgShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetAllRegisDeptLoc_ForXML(lstDeptLocation, Globals.DispatchCallback((asyncResult) =>
                        {
                            lstDeptLocInfo = new ObservableCollection<DeptLocInfo>();
                            
                            try
                            {
                                var item = client.EndGetAllRegisDeptLoc_ForXML( asyncResult);
                               
                                foreach (var deptLocInfo in item)
                                {
                                    lstDeptLocInfo.Add(deptLocInfo);
                                }
                                if (lstDeptLocInfo != null)
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
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
            });
            t.Start();
        }

#endregion
        #region   animator

        public Grid GridRoomConsult { get; set; }
        public void GridRoomConsult_Loaded(object sender, RoutedEventArgs e)
        {
            GridRoomConsult = sender as Grid;
            //for (int i = 0; i < 8; i++)
            //{
            //    ColumnDefinition cd = new ColumnDefinition();
            //    cd.Width = new GridLength(170, GridUnitType.Auto);
            //    GridRoomConsult.ColumnDefinitions.Add(new ColumnDefinition());
            //}
        }

        public void initGrid()
        {
            int Row = 0;
            int Column = 0;
            GridRoomConsult.ColumnDefinitions.Clear();
            GridRoomConsult.Children.Clear();
            for (int i = 0; i < lstDeptLocInfo.Count; i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength(180, GridUnitType.Auto);
                GridRoomConsult.ColumnDefinitions.Add(new ColumnDefinition());
            }
            
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
                roomName.FontSize = 10;
                roomName.FontWeight = FontWeights.Bold;
                roomName.Foreground = new SolidColorBrush(Color.FromArgb(255, 53, 149, 203));
                roomName.Text = bpa.Location.LocationName;
                sp.Children.Add(roomName);

                TextBlock deptName = new TextBlock();
                deptName.HorizontalAlignment = HorizontalAlignment.Center;
                deptName.VerticalAlignment = VerticalAlignment.Center;
                deptName.FontSize = 10;
                deptName.FontWeight = FontWeights.Bold;
                deptName.Foreground = new SolidColorBrush(Color.FromArgb(255, 53, 149, 203));
                deptName.Text = bpa.RefDepartment.DeptName;
                sp.Children.Add(deptName);

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
                ck.Text = string.Format("{0}: ", eHCMSResources.K1895_G1_ChoKham2) + bpa.ChoKham;
                sp.Children.Add(ck);

                TextBlock kr = new TextBlock();
                kr.HorizontalAlignment = HorizontalAlignment.Center;
                kr.VerticalAlignment = VerticalAlignment.Center;
                kr.FontWeight = FontWeights.Bold;
                kr.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 102));
                kr.Text = string.Format("{0}: ", eHCMSResources.T2131_G1_KhamRoi) + bpa.KhamRoi;
                sp.Children.Add(kr);

                sp.DataContext = bpa;
                sp.MouseMove += new MouseEventHandler(sp_MouseMove);
                sp.MouseLeave += new MouseEventHandler(sp_MouseLeave);
                sp.MouseLeftButtonUp += new MouseButtonEventHandler(sp_MouseLeftButtonUp);

                GridRoomConsult.Children.Add(sp);
                Grid.SetRow(sp, Row);
                Grid.SetColumn(sp, Column);
                Column++;
                
            }
            NotifyOfPropertyChange(() => GridRoomConsult); 
        }
        private int minWidth = 120;
        private int minHeight = 180;
        private int maxWidth = 160;
        private int maxHeight = 200;
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
        void sp_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Kiem tra so nguoi cho kham o day


        }
        #endregion

    }
}
