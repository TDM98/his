using System.Collections.Generic;
using System.Windows.Input;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System;
using System.Windows;
using eHCMSLanguage;
using aEMR.Controls;
using System.Collections.ObjectModel;
using System.Threading;
using aEMR.ServiceClient;
using System.Windows.Controls;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IFindAppointmentKSK)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class FindAppointmentKSKViewModel : ViewModelBase, IFindAppointmentKSK
        , IHandle<DoubleClick>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        protected override void OnActivate()
        {
            base.OnActivate();

            _criteria = _searchCriteria;
            Appointments.PageIndex = 0;
            GetHospitalClientsData();
            //SearchAppointments(0, Appointments.PageSize, true);
            //==== 20161115 CMN Begin: Fix Choose poppup handle
            Globals.EventAggregator.Subscribe(this);
            //==== 20161115 CMN End.
        }
        [ImportingConstructor]
        public FindAppointmentKSKViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            SearchCriteria = new AppointmentSearchCriteria();
            Appointments = new PagedSortableCollectionView<PatientAppointment>();
            var appointmentListingVm = Globals.GetViewModel<IAppointmentListingKSK>();
            AppointmentListingContent = appointmentListingVm;
            ActivateItem(appointmentListingVm);
        }
        public void CancelCmd()
        {
            SelectedAppointment = null;
            TryClose();
        }

        public void OkCmd()
        {
            Globals.EventAggregator.Publish(new ItemSelected<PatientAppointment> { Item = SelectedAppointment});
            TryClose();
        }
        #region property
        private IAppointmentListingKSK _appointmentListingContent;
        public IAppointmentListingKSK AppointmentListingContent
        {
            get { return _appointmentListingContent; }
            set
            {
                _appointmentListingContent = value;
                NotifyOfPropertyChange(() => AppointmentListingContent);
            }
        }

        private PatientAppointment _selectedAppointment;

        public PatientAppointment SelectedAppointment
        {
            get { return _selectedAppointment; }
            set
            {
                _selectedAppointment = value;
                NotifyOfPropertyChange(()=>SelectedAppointment);
            }
        }

        /// <summary>
        /// Biến này lưu lại thông tin người dùng tìm kiếm trên form, 
        /// </summary>
        private AppointmentSearchCriteria _criteria;

        private AppointmentSearchCriteria _searchCriteria;
        public AppointmentSearchCriteria SearchCriteria
        {
            get { return _searchCriteria; }
            set 
            {
                _searchCriteria = value; 
                NotifyOfPropertyChange(()=>SearchCriteria);
            }
        }

        private PagedSortableCollectionView<PatientAppointment> _appointments;
        public PagedSortableCollectionView<PatientAppointment> Appointments
        {
            get { return _appointments; }
            private set
            {
                _appointments = value;
                NotifyOfPropertyChange(()=>Appointments);
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
        private HospitalClient _HospitalClientSelected;
        public HospitalClient HospitalClientSelected
        {
            get
            {
                return _HospitalClientSelected;
            }
            set
            {
                if (_HospitalClientSelected == value)
                {
                    return;
                }
                _HospitalClientSelected = value;
                NotifyOfPropertyChange(() => HospitalClientSelected);
            }
        }

        private ObservableCollection<HospitalClient> _HospitalClientCollection;
        public ObservableCollection<HospitalClient> HospitalClientCollection
        {
            get
            {
                return _HospitalClientCollection;
            }
            set
            {
                if (_HospitalClientCollection == value)
                {
                    return;
                }
                _HospitalClientCollection = value;
                NotifyOfPropertyChange(() => HospitalClientCollection);
            }
        }
        private HospitalClientContract _HospitalClientContractSelected;
        public HospitalClientContract HospitalClientContractSelected
        {
            get
            {
                return _HospitalClientContractSelected;
            }
            set
            {
                if (_HospitalClientContractSelected == value)
                {
                    return;
                }
                _HospitalClientContractSelected = value;
                NotifyOfPropertyChange(() => HospitalClientContractSelected);
            }
        }
        private ObservableCollection<HospitalClientContract> _HospitalClientContractCollection;
        public ObservableCollection<HospitalClientContract> HospitalClientContractCollection
        {
            get
            {
                return _HospitalClientContractCollection;
            }
            set
            {
                if (_HospitalClientContractCollection == value)
                {
                    return;
                }
                _HospitalClientContractCollection = value;
                NotifyOfPropertyChange(() => HospitalClientContractCollection);
            }
        }
        #endregion
        AxSearchPatientTextBox txtNameTextBox;
        public void txtName_Loaded(object sender, RoutedEventArgs e)
        {
            txtNameTextBox = (AxSearchPatientTextBox)sender;
        }
        public void SearchCmd()
        {
            if (_searchCriteria.DateFrom.HasValue && _searchCriteria.DateTo.HasValue &&
                _searchCriteria.DateFrom.Value > _searchCriteria.DateTo.Value)
            {
                var temp = _searchCriteria.DateFrom;
                _searchCriteria.DateFrom = _searchCriteria.DateTo;
                _searchCriteria.DateTo = temp;
            }
            _searchCriteria.IsAppointmentKSK = true;
            if (HospitalClientSelected != null)
            {
                _searchCriteria.HosClientID = HospitalClientSelected.HosClientID;
            }
            else
            {
                _searchCriteria.HosClientID = 0;
            }
            if (HospitalClientContractSelected != null)
            {
                _searchCriteria.HosClientContractID = HospitalClientContractSelected.HosClientContractID;
            }
            else
            {
                _searchCriteria.HosClientContractID = 0;
            }
            AppointmentListingContent.SearchCriteria = _searchCriteria;
            AppointmentListingContent.StartSearching();
        }
        public void DoubleClick(object args)
        {
            var eventArgs = args as EventArgs<object>;
            if (eventArgs != null)
            {
                SelectedAppointment = eventArgs.Value as PatientAppointment;

                Globals.EventAggregator.Publish(new ItemSelected<PatientAppointment> { Item = eventArgs.Value as PatientAppointment });
            }
            TryClose();
        }

        public void SelectPatientAndClose(object context)
        {
            SelectedAppointment = context as PatientAppointment;

            if (SelectedAppointment != null)
            {
                Globals.EventAggregator.Publish(new ItemSelected<PatientAppointment> { Item = SelectedAppointment });
            }

            TryClose();
        }
        public void KeyUpCmd(KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (!IsLoading)
                {
                    if (e.Key == Key.Right)
                    {
                        //Go To Next Page.
                        Appointments.MoveToNextPage();
                        e.Handled = true;
                    }
                    else if (e.Key == Key.Left)
                    {
                        //Back to Prev Page.
                        Appointments.MoveToPreviousPage();
                        e.Handled = true;
                    }
                }
            }
        }

        /// <summary>
        /// Copy danh sách bệnh nhân vào Patient list. Khỏi mắc công search lại.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="criteria"></param>
        /// <param name="total"></param>
        public void CopyExistingAppointmentList(IList<PatientAppointment> items, AppointmentSearchCriteria criteria, int total)
        {
            _criteria = criteria.DeepCopy();
            _appointments.Clear();
            if (items != null && items.Count > 0)
            {
                foreach (var p in items)
                {
                    _appointments.Add(p);
                }
                _appointments.TotalItemCount = total;
            }
            NotifyOfPropertyChange(()=>Appointments);
        }

        //public void Handle(AddCompleted<Patient> message)
        //{
        //    if(message != null && message.Item != null)
        //    {
        //        if(IsActive)
        //        {
        //            SearchAppointments(Appointments.PageIndex, Appointments.PageSize, true);
        //        }
        //    }
        //}
        public void Handle(DoubleClick message)
        {
            if(message.Source != AppointmentListingContent)
                return;
            var eventArgs = message.EventArgs;
            SelectedAppointment = eventArgs.Value as PatientAppointment;
            Globals.EventAggregator.Publish(new ItemSelecting<object, PatientAppointment> { Sender = this, Item = SelectedAppointment });
        }

        //▼====== #002
        private void GetHospitalClientsData()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var mFactory = new AppointmentServiceClient())
                    {
                        var mContract = mFactory.ServiceInstance;
                        mContract.BeginGetHospitalClients(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var Collection = mContract.EndGetHospitalClients(asyncResult);
                                if (Collection == null || Collection.Count == 0)
                                {
                                    HospitalClientCollection = new ObservableCollection<HospitalClient>();
                                }
                                else
                                {
                                    HospitalClientCollection = new ObservableCollection<HospitalClient>(Collection);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        //▲====== #002
        private void GetContractName_ByHosClientID(long HosClientID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var mFactory = new AppointmentServiceClient())
                    {
                        var mContract = mFactory.ServiceInstance;
                        mContract.BeginGetContractName_ByHosClientID(HosClientID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var Collection = mContract.EndGetContractName_ByHosClientID(asyncResult);
                                if (Collection == null || Collection.Count == 0)
                                {
                                    HospitalClientContractCollection = new ObservableCollection<HospitalClientContract>();
                                }
                                else
                                {
                                    HospitalClientContractCollection = Collection.ToObservableCollection();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public void KeySearchableComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HospitalClientSelected != null)
            {
                GetContractName_ByHosClientID(HospitalClientSelected.HosClientID);
            }
        }
    }
}
