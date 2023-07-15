using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using System.Linq;
using aEMR.Controls;
using aEMR.Common.BaseModel;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(ISearchAppointments)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SearchAppointmentsViewModel : ViewModelBase, ISearchAppointments
        , IHandle<DoubleClick>
        , IHandle<PatientAppointment_SearchBegin>
        , IHandle<PatientAppointment_SearchEnd>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        private bool _IsProcessing;
        public bool IsProcessing
        {
            get
            {
                return _IsProcessing;
            }
            set
            {
                if (_IsProcessing != value)
                {
                    _IsProcessing = value;
                    NotifyOfPropertyChange(() => IsProcessing);
                }
            }
        }

        private bool _IsConsultation = true;
        public bool IsConsultation
        {
            get
            {
                return _IsConsultation;
            }
            set
            {
                _IsConsultation = value;
                if (_IsConsultation)
                {
                    TitleSearchForm = string.Format("({0})", eHCMSResources.Z0453_G1_DanhChoBN);
                }
                else
                {
                    TitleSearchForm = string.Format("({0})", eHCMSResources.Z0365_G1_DanhChoDieuDuong);
                }
                if (_searchCriteria != null)
                {
                    _searchCriteria.IsConsultation = IsConsultation;                    
                }

                NotifyOfPropertyChange(() => IsConsultation);
            }
        }

        private bool _IsStaff ;
        public bool IsStaff
        {
            get
            {
                return _IsStaff;
            }
            set
            {
                _IsStaff = value;
                NotifyOfPropertyChange(() => IsStaff);
            }
        }


        private string _TitleForm = eHCMSResources.Z0452_G1_DSHenBenh.ToUpper();
        public string TitleForm
        {
            get
            {
                return _TitleForm;
            }
            set
            {
                _TitleForm = value;
                NotifyOfPropertyChange(() => TitleForm);
            }
        }

        private string _TitleSearchForm = string.Format("({0})", eHCMSResources.Z0453_G1_DanhChoBN);
        public string TitleSearchForm
        {
            get
            {
                return _TitleSearchForm;
            }
            set
            {
                _TitleSearchForm = value;
                NotifyOfPropertyChange(() => TitleSearchForm);
            }
        }

        [ImportingConstructor]
        public SearchAppointmentsViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            var appointmentListingVm = Globals.GetViewModel<IAppointmentListing>();
            AppointmentListingContent = appointmentListingVm;
            ActivateItem(appointmentListingVm);

            //Globals.EventAggregator.Subscribe(this);

            SearchCriteria = new AppointmentSearchCriteria();
            SearchCriteria.V_ApptStatus = -1;
            SearchCriteria.LoaiDV = 0;
            SearchCriteria.DateFrom = Globals.ServerDate.Value.Date;
            SearchCriteria.DateTo = Globals.ServerDate.Value.Date;

            //Appointments = new PagedSortableCollectionView<PatientAppointment>();
            //Appointments.OnRefresh += new WeakEventHandler<RefreshEventArgs>(Appointments_OnRefresh).Handler;
            GetAllConsultationTimeSegments();

            //if (!DesignerProperties.IsInDesignTool)
            bool designTime = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());
            if (!designTime)
            {
                LoadApptStatusList();
                //LoadLocations(95);//khoa phong kham
                
                //Coroutine.BeginExecute(DoLoadLocations(Globals.KhoaPhongKham));
                // Txd 25/05/2014 Replaced ConfigList
                Coroutine.BeginExecute(DoLoadLocations(Globals.ServerConfigSection.Hospitals.KhoaPhongKham));
            }
        }

        private IAppointmentListing _appointmentListingContent;
        public IAppointmentListing AppointmentListingContent
        {
            get { return _appointmentListingContent; }
            set
            {
                _appointmentListingContent = value;
                NotifyOfPropertyChange(() => AppointmentListingContent);
            }
        }

        public void SearchAppointmentsCmd()
        {
            _searchCriteria.LoaiDV = IndexLoaiDV;
            AppointmentListingContent.SearchCriteria = _searchCriteria;
            AppointmentListingContent.StartSearching();
        }

        public void btnResetAppointment() 
        {
            SearchCriteria = new AppointmentSearchCriteria();
            SelectedLocation = Locations.FirstOrDefault();
            SelectedSegments = AppointmentSegmentsList.FirstOrDefault();
            aucHoldConsultDoctor.Text = "";
            cboDichVu.SelectedIndex = 0;
        }

        public void btnPrintAppointment()
        {
            _searchCriteria.LoaiDV = IndexLoaiDV;
            if (SelectedLocation != null && SelectedLocation.DeptLocationID > 0)
            {
                _searchCriteria.LocationName = SelectedLocation.Location.LocationName;
            }
            else
            {
                _searchCriteria.LocationName = "";
            }

            //var proAlloc = Globals.GetViewModel<ICommonPreviewView>();
            //proAlloc.AppointmentCriteria = SearchCriteria;
            //proAlloc.TieuDeRpt = TitleForm;
            //proAlloc.eItem = ReportName.AppointmentReport;

            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<ICommonPreviewView> onInitDlg = (proAlloc) =>
            {
                proAlloc.AppointmentCriteria = SearchCriteria;
                proAlloc.TieuDeRpt = TitleForm;
                proAlloc.eItem = ReportName.AppointmentReport;
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }

        private AppointmentSearchCriteria _searchCriteria;
        public AppointmentSearchCriteria SearchCriteria
        {
            get { return _searchCriteria; }
            set
            {
                _searchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }

        private ObservableCollection<Lookup> _appointmentStatusList;
        public ObservableCollection<Lookup> AppointmentStatusList
        {
            get { return _appointmentStatusList; }
            set
            {
                _appointmentStatusList = value;
                NotifyOfPropertyChange(() => AppointmentStatusList);
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
            }
        }

        private ObservableCollection<ConsultationTimeSegments> _appointmentSegmentsList;
        public ObservableCollection<ConsultationTimeSegments> AppointmentSegmentsList
        {
            get
            {
                return _appointmentSegmentsList;
            }
            set
            {
                if (_appointmentSegmentsList != value)
                {
                    _appointmentSegmentsList = value;
                    NotifyOfPropertyChange(() => AppointmentSegmentsList);
                }
            }
        }

        private ConsultationTimeSegments _SelectedSegments;
        public ConsultationTimeSegments SelectedSegments
        {
            get
            {
                return _SelectedSegments;
            }
            set
            {
                if (_SelectedSegments != value)
                {
                    _SelectedSegments = value;
                    SearchCriteria.ApptTimeSegment = SelectedSegments;
                    NotifyOfPropertyChange(() => SelectedSegments);
                   
                }
            }
        }

        private bool _isDeleting;
        public bool IsDeleting
        {
            get { return _isDeleting; }
            set
            {
                _isDeleting = value;
                NotifyOfPropertyChange(() => IsDeleting);
            }
        }

        private bool _IsDeptLoc = false;
        public bool IsDeptLoc
        {
            get { return _IsDeptLoc; }
            set
            {
                _IsDeptLoc = value;
                NotifyOfPropertyChange(() => IsDeptLoc);
            }
        }

        public void LoadApptStatusList()
        {
            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllLookupValuesByType(LookupValues.APPT_STATUS,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    error = new AxErrorEventArgs(fault);
                                }
                                catch (Exception ex)
                                {
                                    error = new AxErrorEventArgs(ex);
                                }

                                AppointmentStatusList = new ObservableCollection<Lookup>(allItems);
                                Lookup firstItem = new Lookup();
                                firstItem.LookupID = -1;
                                firstItem.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                AppointmentStatusList.Insert(0, firstItem);

                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                }
            });
            t.Start();
        }

        private IEnumerator<IResult> DoLoadLocations(long deptId)
        {
            var deptLoc = new eHCMS.CommonUserControls.CommonTasks.LoadDeptLoctionByIDTask(deptId);
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
            itemDefault.Location.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0116_G1_HayChonPg);
            Locations.Insert(0, itemDefault);
            SelectedLocation = itemDefault;
            yield break;
        }

        public void LoadLocations(long? deptId)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format("{0}.", eHCMSResources.Z0115_G1_LayDSPgBan) });

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
                            itemDefault.Location.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z1098_G1_TatCaPhg);
                            Locations.Insert(0, itemDefault);

                            SelectedLocation = itemDefault;
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        private void GetAllConsultationTimeSegments()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
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
                                AppointmentSegmentsList = new ObservableCollection<ConsultationTimeSegments>();
                                ConsultationTimeSegments tempCTimeSeg = new ConsultationTimeSegments();
                                tempCTimeSeg.SegmentNameExt = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.A0303_G1_Msg_InfoChonCaKham);
                                tempCTimeSeg.ConsultationTimeSegmentID = -1;
                                AppointmentSegmentsList.Add(tempCTimeSeg);
                                SelectedSegments = tempCTimeSeg;
                                foreach(var item in results)
                                {
                                    AppointmentSegmentsList.Add(item);
                                }
                                
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

                    }), null);

                }

            });

            t.Start();
        }

        public void Handle(DoubleClick message)
        {
            if (message.Source != AppointmentListingContent)
            {
                return;
            }
            //var apptVm = Globals.GetViewModel<IAddEditAppointment>();
            //apptVm.SetCurrentAppointment(AppointmentListingContent.SelectedAppointment);

            //Globals.ShowDialog(apptVm as Conductor<object>);

            Action<IAddEditAppointment> onInitDlg = (apptVm) =>
            {
                apptVm.SetCurrentAppointment(AppointmentListingContent.SelectedAppointment);
            };
            GlobalsNAV.ShowDialog<IAddEditAppointment>(onInitDlg);
        }

        public void Handle(PatientAppointment_SearchEnd message)
        {
            IsProcessing = false;
        }

        public void Handle(PatientAppointment_SearchBegin message)
        {
            IsProcessing = true;
        }

        public Int16 IndexLoaiDV { get; set; }
        AxComboBox cboDichVu;
        public void cboDichVu_Loaded(object sender, RoutedEventArgs e)
        {
            cboDichVu = (AxComboBox)sender ;
        }
        public void cboDichVu_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cboDichVu != null)
            {
                IndexLoaiDV = Convert.ToInt16(cboDichVu.SelectedIndex);//0:Cả  Hai;1:KB;2:CLS
                if (IndexLoaiDV == 1)
                {
                    IsDeptLoc = true;
                }
                else
                {
                    IsDeptLoc = false;
                }
                if (IndexLoaiDV > 0)
                {
                    SearchCriteria.LoaiDVName = (cboDichVu.SelectedItemEx as ComboBoxItem).Content.ToString();
                }
                else
                {
                    SearchCriteria.LoaiDVName = "";
                }
            }
        }

        public void cboStatus_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
           aEMR.Controls.AxComboBox cboStatus = sender as aEMR.Controls.AxComboBox;
            if (cboStatus != null)
            {
                if (AppointmentStatusList!=null && cboStatus.SelectedIndex > 0)
                {
                    SearchCriteria.V_ApptStatusName = (cboStatus.SelectedItem as Lookup).ObjectValue;
                }
                else
                {
                    SearchCriteria.V_ApptStatusName = "";
                }
            }
        }

        #region nhan vien
        private ObservableCollection<Staff> _StaffCatgs;
        public ObservableCollection<Staff> StaffCatgs
        {
            get
            {
                return _StaffCatgs;
            }
            set
            {
                if (_StaffCatgs != value)
                {
                    _StaffCatgs = value;
                    NotifyOfPropertyChange(() => StaffCatgs);
                }
            }
        }
        private void SearchStaffCatgs(string SearchKeys)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSearchStaffFullName(SearchKeys, 0, Globals.PageSize, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndSearchStaffFullName(asyncResult);
                            if (results != null)
                            {
                                StaffCatgs = new ObservableCollection<Staff>();
                                foreach (Staff p in results)
                                {
                                    StaffCatgs.Add(p);
                                }
                                NotifyOfPropertyChange(() => StaffCatgs);
                            }
                            aucHoldConsultDoctor.ItemsSource = StaffCatgs;
                            aucHoldConsultDoctor.PopulateComplete();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }


        AutoCompleteBox aucHoldConsultDoctor;

        public void aucHoldConsultDoctor_Loaded(object sender, RoutedEventArgs e)
        {
            aucHoldConsultDoctor = sender as AutoCompleteBox;
        }

        public void aucHoldConsultDoctor_Populating(object sender, PopulatingEventArgs e)
        {
            this.SearchStaffCatgs(e.Parameter);
        }

        public void aucHoldConsultDoctor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (aucHoldConsultDoctor.SelectedItem != null)
            {
                SearchCriteria.StaffID = (aucHoldConsultDoctor.SelectedItem as Staff).StaffID;                
            }
            else
            {
                SearchCriteria.StaffID = 0;
            }
        }



        #endregion

        public void btnSendSMS()
        {
            MessageBox.Show("Chức năng đang được xây dựng");
        }
    }
}



