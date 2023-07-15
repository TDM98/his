using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Common.Collections;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.BaseModel;
using eHCMSLanguage;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IAppointmentListingKSK)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class AppointmentListingKSKViewModel : ViewModelBase, IAppointmentListingKSK
        , IHandle<AddCompleted<PatientAppointment>>
        , IHandle<UpdateCompleted<PatientAppointment>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public AppointmentListingKSKViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAgr, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            eventAgr.Subscribe(this);

            SearchCriteria = new AppointmentSearchCriteria {V_ApptStatus = -1};
            PatientAppointment a = new PatientAppointment();
            Appointments = new PagedSortableCollectionView<PatientAppointment>();
            Appointments.OnRefresh += new WeakEventHandler<RefreshEventArgs>(Appointments_OnRefresh).Handler;

        }
        /// <summary>
        /// Biến này lưu lại thông tin người dùng tìm kiếm trên form, 
        /// </summary>
        private AppointmentSearchCriteria _criteria;
        public void Appointments_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchAppointments(Appointments.PageIndex, Appointments.PageSize, false);
        }
        public void StartSearching()
        {
            Appointments.PageIndex = 0;
            _criteria = _searchCriteria.DeepCopy();
            SearchAppointments(0, Appointments.PageSize, true);
        }

        public void DoubleClick(object args)
        {
            var eventArgs = args as EventArgs<object>;
            SelectedAppointment = eventArgs.Value as PatientAppointment;

            Globals.EventAggregator.Publish(new DoubleClick { Source = this, EventArgs = eventArgs });
        }

        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                NotifyOfPropertyChange(()=>IsLoading);
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

        private void SearchAppointments(int pageIndex, int pageSize, bool bCountTotal)
        {
            //IsLoading = true;
            this.DlgShowBusyIndicator(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.G1160_G1_TimCuocHen));
            var t = new Thread(() =>
            {
                try
                {
                    //IsLoading = true;
                    using (var serviceFactory = new AppointmentServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginSearchAppointments(_criteria, pageIndex, pageSize, bCountTotal, Globals.DispatchCallback(asyncResult =>
                        {
                            int totalCount = 0;
                            List<PatientAppointment> allItems = null;
                            bool bOK;
                            try
                            {
                                allItems = client.EndSearchAppointments(out totalCount, asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                                bOK = false;
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                bOK = false;
                            }
                            finally
                            {
                                //IsLoading = false;
                                this.DlgHideBusyIndicator();
                            }

                            Appointments.Clear();
                            if (bOK)
                            {
                                if (bCountTotal)
                                {
                                    Appointments.TotalItemCount = totalCount;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        Appointments.Add(item);
                                    }

                                }
                            }
                            else
                            {
                                Appointments.Clear();
                            }
                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    Appointments.Clear();
                    //IsLoading = false;
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
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
     
        public void Handle(AddCompleted<PatientAppointment> message)
        {
            if(message != null)
            {
                StartSearching();
            }
        }

        public void Handle(UpdateCompleted<PatientAppointment> message)
        {
            if (message != null)
            {
                StartSearching();
            }
        }
    }
}
