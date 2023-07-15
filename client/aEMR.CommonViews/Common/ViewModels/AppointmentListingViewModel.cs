using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.CommonTasks;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;
using System.Linq;
using aEMR.Common.BaseModel;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IAppointmentListing)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class AppointmentListingViewModel : ViewModelBase, IAppointmentListing
        , IHandle<AddCompleted<PatientAppointment>>
        , IHandle<UpdateCompleted<PatientAppointment>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public AppointmentListingViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAgr, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAgr.Subscribe(this);

            SearchCriteria = new AppointmentSearchCriteria();
            SearchCriteria.V_ApptStatus = -1;

            Appointments = new PagedSortableCollectionView<PatientAppointment>();
            Appointments.OnRefresh += new WeakEventHandler<aEMR.Common.Collections.RefreshEventArgs>(Appointments_OnRefresh).Handler;
            //20191024 TBL: Mỗi trang lấy 20 dòng
            Appointments.PageSize = 20;
        }
        /// <summary>
        /// Biến này lưu lại thông tin người dùng tìm kiếm trên form, 
        /// </summary>
        private AppointmentSearchCriteria _criteria;
        public void Appointments_OnRefresh(object sender, aEMR.Common.Collections.RefreshEventArgs  e)
        {
            if (Appointments.ItemCount > 0)
            {
                //SearchAppointments(Appointments.PageIndex, Appointments.PageSize, false);
                SearchAppointments(null, Appointments.PageIndex, Appointments.PageSize, false);
            }
        }
        public void StartSearching()
        {
            Appointments.PageIndex = 0;
            _criteria = _searchCriteria.DeepCopy();
            SearchAppointments(null, 0, Appointments.PageSize, true);
        }

        public void DoubleClick(object args)
        {
            var eventArgs = args as EventArgs<object>;
            SelectedAppointment = eventArgs.Value as PatientAppointment;

            if (SelectedAppointment.IsCanEdit == false)
            {
                MessageBox.Show(eHCMSResources.A0439_G1_Msg_InfoCuocHenDaTHien_KhSua);//Báo thì báo vẫn cho xem
            }

            //var apptVm = Globals.GetViewModel<IAddEditAppointment>();

            //apptVm.SetCurrentAppointment(SelectedAppointment);

            //apptVm.IsCreateApptFromConsultation = IsCreateApptFromConsultation;

            //Globals.ShowDialog(apptVm as Conductor<object>);
            if (!Globals.ServerConfigSection.OutRegisElements.IsPerformingTMVFunctionsA)
            {
                Action<IAddEditAppointment> onInitDlg = (apptVm) =>
                {
                    apptVm.SetCurrentAppointment(SelectedAppointment);
                    apptVm.IsCreateApptFromConsultation = IsCreateApptFromConsultation;
                };
                GlobalsNAV.ShowDialog<IAddEditAppointment>(onInitDlg);
            }
            else
            {
                Action<IAddEditAppointmentTMV> onInitDlg = (apptVm) =>
                {
                    apptVm.SetCurrentAppointment(SelectedAppointment);
                    apptVm.IsCreateApptFromConsultation = IsCreateApptFromConsultation;
                };
                GlobalsNAV.ShowDialog<IAddEditAppointmentTMV> (onInitDlg);
            }
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

        private bool _isCreateApptFromConsultation;
        public bool IsCreateApptFromConsultation
        {
            get
            {
                return _isCreateApptFromConsultation;
            }
            set
            {
                if (_isCreateApptFromConsultation != value)
                {
                    _isCreateApptFromConsultation = value;
                    NotifyOfPropertyChange(() => IsCreateApptFromConsultation);
                }
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

        public bool checkIsNeedNewAppointment(List<PatientAppointment> allAppointment) 
        {
            if (allAppointment == null || allAppointment.Count<1)
            {
                return true;
            }
            if (allAppointment.FirstOrDefault().ApptDate == null) 
            {
                return false;
            }
            //foreach (var item in allAppointment)
            //{
            //    if (item.ApptDate==null)
            //    {
            //        return false;
            //    }
            //}
            return true;
        }
        public void ClearItemSource() 
        {
            Appointments.Clear();
        }

        public void SearchAppointments(GenericCoRoutineTask genTask, object _pageIndex, object _pageSize, object _bCountTotal)
        {
            int pageIndex = Convert.ToInt32(_pageIndex);
            int pageSize = Convert.ToInt32(_pageSize);
            bool bCountTotal = Convert.ToBoolean(_bCountTotal);

            var t = new Thread(() =>
            {
                this.ShowBusyIndicator();
                try
                {
                    IsLoading = true;
                    using (var serviceFactory = new AppointmentServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetAppointments(_criteria, pageIndex, pageSize, bCountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int totalCount = 0;
                            List<PatientAppointment> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndGetAppointments(out totalCount, asyncResult);
                                bOK = true;
                                if (allItems == null || allItems.Count < 1)
                                {
                                    Globals.EventAggregator.Publish(new SearchAppointmentResultEvent { result = true });
                                }
                                else
                                {
                                    Globals.EventAggregator.Publish(new SearchAppointmentResultEvent { result = checkIsNeedNewAppointment(allItems) });
                                }
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                                bOK = false;
                                Globals.EventAggregator.Publish(new SearchAppointmentResultEvent { result = true });
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                bOK = false;
                            }
                            finally
                            {
                                if (genTask != null)
                                {
                                    genTask.ActionComplete(true);
                                }
                                this.HideBusyIndicator();
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
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    if (genTask != null)
                    {
                        genTask.ActionComplete(true);
                    }
                    MessageBox.Show(ex.ToString());
                    ClientLoggerHelper.LogError(ex.ToString());
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        private IEnumerator<IResult> SearchApptAndReloadServiceRecord()
        {
            yield return GenericCoRoutineTask.StartTask(SearchAppointments, Appointments.PageIndex, Appointments.PageSize, true);

            if (IsCreateApptFromConsultation)
            {
                IConsultationModule consultVM = Globals.GetViewModel<IConsultationModule>();

                consultVM.PatientServiceRecordsGetForKhamBenh_Ext();
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
        public void DeleteAppointmentCmd(RoutedEventArgs eventArgs)
        {
            //_appointmentCatalog.DeletePatientAppointments(AppointmentID);
            
            FrameworkElement elem = eventArgs.OriginalSource as FrameworkElement;
            if(elem == null)
            {
                return;
            }
            
            PatientAppointment appointment = elem.DataContext as PatientAppointment;
            if(appointment == null || appointment.AppointmentID <= 0)
            {
                return;
            }

            if (appointment.V_ApptStatus == AllLookupValues.ApptStatus.ACTIONED)
            {
                MessageBox.Show(eHCMSResources.A0438_G1_Msg_InfoCuocHenDaTHien_KhXoa);
                return;
            }

            MessageBoxResult result = MessageBox.Show(eHCMSResources.A0155_G1_Msg_ConfXoaCuocHen, eHCMSResources.G0442_G1_TBao,
                                                      MessageBoxButton.OKCancel);
            if(result == MessageBoxResult.Cancel)
            {
                return;
            }

            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0613_G1_DangXoaCHen) });
                try
                {
                    IsDeleting = true;
                    using (var serviceFactory = new AppointmentServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginDeletePatientAppointments(appointment.AppointmentID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                client.EndDeletePatientAppointments(asyncResult);

                                //KMx: Sau khi xóa cuộc hẹn thành công thì phải load lại Service Record, nếu không thì toa thuốc vẫn còn lưu cuộc hẹn cũ, dẫn đến hẹn bệnh bị lỗi (25/03/2016 17:13).
                                //KMx: Chuyển hàm này thành GenericCoroutine để không bị chạy đua với hàm load lại PatientServiceRecord(25/03/2016 17:08).
                                //SearchAppointments(Appointments.PageIndex, Appointments.PageSize, true);

                                Coroutine.BeginExecute(SearchApptAndReloadServiceRecord());
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }

                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    Appointments.Clear();
                }
                finally
                {
                    IsDeleting = false;
                    Globals.IsBusy = false;
                }
            });
            t.Start();
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
        private IRegistration_DataStorage _Registration_DataStorage;
        public IRegistration_DataStorage Registration_DataStorage
        {
            get
            {
                return _Registration_DataStorage;
            }
            set
            {
                if (_Registration_DataStorage == value)
                {
                    return;
                }
                _Registration_DataStorage = value;
                NotifyOfPropertyChange(() => Registration_DataStorage);
            }
        }
        private bool _IsPerformingTMVFunctionsA = Globals.ServerConfigSection.OutRegisElements.IsPerformingTMVFunctionsA;
        public bool IsPerformingTMVFunctionsA
        {
            get
            {
                return _IsPerformingTMVFunctionsA;
            }
            set
            {
                if (_IsPerformingTMVFunctionsA == value)
                    return;
                _IsPerformingTMVFunctionsA = value;
                NotifyOfPropertyChange(() => IsPerformingTMVFunctionsA);
            }
        }
        public void chkAppointment_Click(object source, object sender)
        {

        }
        private bool _AllChecked;
        public bool AllChecked
        {
            get
            {
                return _AllChecked;
            }
            set
            {
                if (_AllChecked != value)
                {
                    _AllChecked = value;
                    NotifyOfPropertyChange(() => AllChecked);
                    SetAllAppointment();
                }
            }
        }
        public void SetAllAppointment()
        {
            if (Appointments == null || Appointments.Count <= 0)
            {
                return;
            }

            foreach (PatientAppointment item in Appointments)
            {
                if (AllChecked && !item.IsChecked)
                {
                    item.IsChecked = true;
                }
                else
                {
                    item.IsChecked = false;
                }
            }
        }
    }
}
