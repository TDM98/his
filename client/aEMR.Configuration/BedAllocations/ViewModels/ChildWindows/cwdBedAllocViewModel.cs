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
    [Export(typeof(IcwdBedAlloc)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class cwdBedAllocViewModel : Conductor<object>, IcwdBedAlloc
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
        public cwdBedAllocViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            _allBedPatientAllocs =new ObservableCollection<BedPatientAllocs>();
            GetAllBedPatientAllocByDeptID(deptID);
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            GetAllBedPatientAllocByDeptID(deptID);
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

        public void GrdLoaded(object sender,RoutedEventArgs e)
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
                if(bpa.IsActive==false)
                {
                    uri = new Uri("/eHCMSCal;component/Assets/Images/Bed.png", UriKind.Relative);
                }else
                {
                    uri = new Uri("/eHCMSCal;component/Assets/Images/Bed4.png", UriKind.Relative);
                    if (bpa.CheckOutDate < DateTime.Now)
                    {
                        sp.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 125, 125));
                    }
                    else 
                    {
                        
                    }
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

        private ObservableCollection<BedAllocation> _allBedAllocation;
        public ObservableCollection<BedAllocation> allBedAllocation
        {
            get
            {
                return _allBedAllocation;
            }
            set
            {
                if (_allBedAllocation == value)
                    return;
                _allBedAllocation = value;
                NotifyOfPropertyChange(() => allBedAllocation);
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

        void sp_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //BedPatientAllocs bpa = new DataEntities.BedPatientAllocs();
            //bpa = (BedPatientAllocs)((StackPanel)sender).DataContext;
            //BedPatientAllocVM.selectedBedPatientAllocs = bpa;
            //var cw = new eHCMSApp.Views.ConfigurationManager.BedAllocation.ChildWindows.cwdBedPatient();
            //cw.DataContext = BedPatientAllocVM;
            //cw.Show();
        }

        void sp_MouseLeave(object sender, MouseEventArgs e)
        {
            ((StackPanel)sender).Width = 55;
            ((StackPanel)sender).Height = 55;
        }

        void sp_MouseMove(object sender, MouseEventArgs e)
        {
            ((StackPanel)sender).Width = 70;
            ((StackPanel)sender).Height = 70;
        }
        private void GetAllBedPatientAllocByDeptID(long DeptLocationID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading=true;

                using (var serviceFactory = new BedAllocationsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllBedPatientAllocByDeptID(DeptLocationID, false
                        , Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int total=0;
                            var results = contract.EndGetAllBedPatientAllocByDeptID(out total,asyncResult);
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
                                initBedForm();
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
        public void butSave()
        {
            TryClose();
        }
        public void butExit()
        {
            TryClose();
        }
    }
}
