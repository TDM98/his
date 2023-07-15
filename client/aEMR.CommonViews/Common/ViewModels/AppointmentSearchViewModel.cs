using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using aEMR.CommonTasks;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.Common;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;


namespace aEMR.Common.ViewModels
{
    [Export(typeof(IAppointmentSearch)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class AppointmentSearchViewModel : ViewModelBase, IAppointmentSearch
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public AppointmentSearchViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAgr, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAgr.Subscribe(this);

            AppointmentList = new PagedSortableCollectionView<PatientAppointment>();
            AppointmentList.PageSize = 10;// int.MaxValue;
            _searchCriteria = new AppointmentSearchCriteria();
            Coroutine.BeginExecute(LoadAppointmentStatus());
        }
        public override string ChildWindowTitle
        {
            get
            {
                return eHCMSResources.Z0247_G1_DSCuocHen;
            }
        }
        public override bool IsProcessing
        {
            get
            {
                return _isSearching;
            }
        }
        public override string StatusText
        {
            get
            {
                if (_isSearching)
                {
                    return string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z1025_G1_DangTimDSHen);
                }
                return string.Empty;
            }
        }


        private bool _isSearching;
        public bool IsSearching
        {
            get { return _isSearching; }
            set
            {
                _isSearching = value;
                NotifyOfPropertyChange(()=>IsSearching);
                NotifyOfPropertyChange(() => CanSearchAppointmentCmd);
                NotifyWhenBusy();
            }
        }

        private bool _isAddingToOutstdTask;
        public bool IsAddingToOutstdTask
        {
            get { return _isAddingToOutstdTask; }
            set
            {
                _isAddingToOutstdTask = value;
                NotifyOfPropertyChange(() => IsAddingToOutstdTask);

                NotifyOfPropertyChange(() => CanAddToOutstdTaskCmd);
            }
        }

        private ObservableCollection<Lookup> _apptStatusList;
        public ObservableCollection<Lookup> ApptStatusList
        {
            get
            {
                return _apptStatusList;
            }
            set
            {
                _apptStatusList = value;
                NotifyOfPropertyChange(()=>ApptStatusList);
            }
        }
        private AppointmentSearchCriteria _criteria;
        private AppointmentSearchCriteria _searchCriteria;
        /// <summary>
        /// Tiêu chuẩn tìm kiếm các cuộc hẹn
        /// </summary>
        public AppointmentSearchCriteria SearchCriteria
        {
            get
            {
                return _searchCriteria;
            }
            set
            {
                _searchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }

        PagedSortableCollectionView<PatientAppointment> _appointmentList;
        public PagedSortableCollectionView<PatientAppointment> AppointmentList
        {
            get
            {
                return _appointmentList;
            }
            set
            {
                if (_appointmentList != value)
                {
                    _appointmentList = value;
                    NotifyOfPropertyChange(()=>AppointmentList);
                   
                    NotifyOfPropertyChange(() => CanAddToOutstdTaskCmd);
                }
            }
        }

        private IEnumerator<IResult> LoadAppointmentStatus()
        {
            var apptStatusTask = new LoadLookupListTask(LookupValues.APPT_STATUS);
            yield return apptStatusTask;
            if(apptStatusTask.Error == null)
            {
                ApptStatusList = apptStatusTask.LookupList;
            }
            else
            {
                ApptStatusList = null;
            }
            yield break;
        }
        public bool CanSearchAppointmentCmd
        {
            get
            {
                return !IsSearching;
            }
        }
        public void SearchAppointmentCmd()
        {
            StartSearching();
        }
        public void CloseCmd()
        {
            TryClose();
        }
        //KMx: Anh Tuấn không cho insert vào bảng PatientQueue
        //public void AddToOutstdTaskCmd()
        //{
        //    AddAppointmentsToQueue();
        //}

        public bool CanAddToOutstdTaskCmd
        {
            get
            {
                return AppointmentList != null && AppointmentList.Count > 0
                    && !IsAddingToOutstdTask;
            }
        }
        public void StartSearching()
        {
            _searchCriteria.DateTo = _searchCriteria.DateFrom;
            _criteria = _searchCriteria.DeepCopy();

            AppointmentList.Clear();
            AppointmentList.PageIndex = 0;
            //Neu chi lay 1 ngay thi lay het khoi phan trang (giong code truoc);
            if (_searchCriteria.DateFrom.HasValue && _searchCriteria.DateFrom.Value > DateTime.MinValue)
            {
                AppointmentList.PageSize = 500;
            }
            else
            {
                AppointmentList.PageSize = 10;
            }
            LoadAppointments(AppointmentList.PageIndex, AppointmentList.PageSize, true);
        }
        private void LoadAppointments(int pageIndex, int pageSize, bool bCountTotal)
        {
            var t = new Thread(() =>
            {
                if (_criteria == null)
                {
                    _criteria = _searchCriteria;
                }
                IsSearching = true;
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new AppointmentServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetAppointmentsDay(_criteria, pageIndex, pageSize, bCountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int totalCount = 0;
                            IList<PatientAppointment> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndGetAppointmentsDay(out totalCount, asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                error = new AxErrorEventArgs(fault);
                            }
                            catch (Exception ex)
                            {
                                error = new AxErrorEventArgs(ex);
                            }

                            AppointmentList.Clear();
                            if (bOK)
                            {
                                if (bCountTotal)
                                {
                                    AppointmentList.TotalItemCount = totalCount;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        AppointmentList.Add(item);
                                    }
                                }
                            }
                            
                            NotifyOfPropertyChange(() => CanAddToOutstdTaskCmd);

                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {
                    IsSearching = false;
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                }
            });
            t.Start();
        }

        //KMx: Anh Tuấn không cho insert vào bảng PatientQueue
        //private void AddAppointmentsToQueue()
        //{
        //    ObservableCollection<PatientQueue> lstPQ = new ObservableCollection<PatientQueue>();
        //    foreach (PatientAppointment pa in AppointmentList)
        //    {
        //        PatientQueue queue = new PatientQueue();
        //        queue.V_QueueType = (long)(int)AllLookupValues.QueueType.DANG_KY_HEN_BENH;
        //        queue.PatientAppointmentID = pa.AppointmentID;
        //        queue.PatientID = pa.Patient.PatientID;
        //        queue.FullName = pa.Patient.FullName;

        //        lstPQ.Add(queue);
        //    }
            
        //    var t = new Thread(() =>
        //    {
        //        if (_criteria == null)
        //        {
        //            _criteria = _searchCriteria;
        //        }
        //        IsAddingToOutstdTask = true;
        //        AxErrorEventArgs error = null;
        //        try
        //        {
        //            using (var serviceFactory = new AppointmentServiceClient())
        //            {
        //                var client = serviceFactory.ServiceInstance;
        //                client.BeginPatientQueue_InsertList(lstPQ, Globals.DispatchCallback((asyncResult) =>
        //                {
        //                    IList<string> allItems = null;
        //                    try
        //                    {
        //                        client.EndPatientQueue_InsertList(out allItems, asyncResult);
        //                    }
        //                    catch (FaultException<AxException> fault)
        //                    {
        //                        error = new AxErrorEventArgs(fault);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        error = new AxErrorEventArgs(ex);
        //                    }
        //                }), null)
        //                    ;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            error = new AxErrorEventArgs(ex);
        //        }
        //        finally
        //        {
        //            IsAddingToOutstdTask = false;
        //        }
        //        if (error != null)
        //        {
        //            Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
        //        }
        //    });
        //    t.Start();
        //}
    }
}
