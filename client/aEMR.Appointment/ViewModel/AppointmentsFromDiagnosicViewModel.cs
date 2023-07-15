using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.ServiceClient;
using aEMR.DataContracts;
using aEMR.Common;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using eHCMS.CommonUserControls.CommonTasks;
using System.Windows;

namespace aEMR.Appointment.ViewModels
{
    [Export(typeof(IAppointmentsFromDiagnosic)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class AppointmentsFromDiagnosicViewModel : Conductor<object>, IAppointmentsFromDiagnosic
        , IHandle<DoubleClick>
        , IHandle<PatientAppointment_SearchBegin>
        , IHandle<PatientAppointment_SearchEnd>
        , IHandle<AddCompleted<PatientAppointment>>
        , IHandle<AppointmentAddEditCloseEvent>
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

        [ImportingConstructor]
        public AppointmentsFromDiagnosicViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            //Globals.EventAggregator.Subscribe(this);

            SearchRegCriteria = new SeachPtRegistrationCriteria();
            SearchRegCriteria.KhamBenh = true;
            SearchRegCriteria.IsHoanTat = true;
            SearchRegCriteria.FromDate = DateTime.Now.Date;
            SearchRegCriteria.ToDate = DateTime.Now.Date;
            SearchRegCriteria.IsAppointment = true;
            SearchRegCriteria.DeptID = 95;
            //if (!DesignerProperties.IsInDesignTool)
            bool designTime = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());
            if (!designTime)
            {
                //Coroutine.BeginExecute(DoLoadLocations(Globals.KhoaPhongKham));
                // Txd 25/05/2014 Replaced ConfigList
                Coroutine.BeginExecute(DoLoadLocations(Globals.ServerConfigSection.Hospitals.KhoaPhongKham));
            }
            RegistrationDetails = new PagedSortableCollectionView<PatientRegistrationDetail>();
            //RegistrationDetails.OnRefresh += RegistrationDetails_OnRefresh;
            RegistrationDetails.PageIndex = 0;
            RegistrationDetails.PageSize = 20;
            //RegistrationDetails.OnRefresh += new EventHandler<RefreshEventArgs>(RegistrationDetails_OnRefresh);
            SearchRegisDetailPrescription(RegistrationDetails.PageIndex, RegistrationDetails.PageSize, true);   
            
        }
        void RegistrationDetails_OnRefresh(object sender, aEMR.Common.Collections.RefreshEventArgs e)
        {
            SearchRegisDetailPrescription(RegistrationDetails.PageIndex, RegistrationDetails.PageSize, true);            
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

        public void btnSearchAppointments()
        {
            SearchRegisDetailPrescription(RegistrationDetails.PageIndex, RegistrationDetails.PageSize, true);
        }

        private SeachPtRegistrationCriteria _searchRegCriteria;
        public SeachPtRegistrationCriteria SearchRegCriteria
        {
            get { return _searchRegCriteria; }
            set
            {
                _searchRegCriteria = value;
                NotifyOfPropertyChange(() => SearchRegCriteria);
            }
        }

        private PagedSortableCollectionView<PatientRegistrationDetail> _RegistrationDetails;
        public PagedSortableCollectionView<PatientRegistrationDetail> RegistrationDetails
        {
            get { return _RegistrationDetails; }
            private set
            {
                _RegistrationDetails = value;
                NotifyOfPropertyChange(() => RegistrationDetails);
            }
        }

        private void SearchRegisDetailPrescription(int pageIndex, int pageSize, bool bCountTotal)
        {
            //IsProcessing = true;
            this.ShowBusyIndicator("");
            if (Globals.DeptLocation == null)
            {
                RegistrationDetails.Clear();
                return;
            }
            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginSearchRegisDetailPrescription(_searchRegCriteria, pageIndex, pageSize, bCountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int totalCount = 0;
                            RegistrationDetails.Clear();
                            IList<PatientRegistrationDetail> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndSearchRegisDetailPrescription(out totalCount, asyncResult);
                                bOK = true;
                                RegistrationDetails.TotalItemCount = totalCount;
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
                                this.HideBusyIndicator();
                            }
                            if (bOK)
                            {
                                if (bCountTotal)
                                {
                                    RegistrationDetails.TotalItemCount = totalCount;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        RegistrationDetails.Add(item);
                                    }
                                }
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                    ClientLoggerHelper.LogInfo(error.ToString());
                    this.HideBusyIndicator();
                }
                if (error != null)
                {
                    //Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                    Globals.ShowMessage(error.ToString(), eHCMSResources.T0432_G1_Error);
                }
            });
            t.Start();
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

        private IEnumerator<IResult> DoLoadLocations(long deptId)
        {
            var deptLoc = new LoadDeptLoctionByIDTask(deptId);
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
            itemDefault.Location.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2094_G1_ChonPg);
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
                            itemDefault.DeptLocationID = -1;
                            itemDefault.Location = new Location();
                            itemDefault.Location.LID = -1;
                            itemDefault.Location.LocationName = "--Hãy chọn Phòng--";
                            Locations.Insert(0, itemDefault);
                            SearchRegCriteria.DeptLocationID = -1;
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

        public void Handle(DoubleClick message)
        {
            if (message.Source != AppointmentListingContent)
                return;

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

        public void DoubleClick(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;
            
            //var apptVm = Globals.GetViewModel<IAddEditAppointment>();
            //var patientRegDetail = eventArgs.Value as PatientRegistrationDetail;
            //apptVm.RegistrationID = patientRegDetail.PtRegistrationID!=null?patientRegDetail.PtRegistrationID.Value:0;
            //if (patientRegDetail.patientAppointment!=null)
            //{
            //    patientRegDetail.patientAppointment.Patient = patientRegDetail.PatientRegistration.Patient;
            //    apptVm.SetCurrentAppointment(patientRegDetail.patientAppointment);
            //    Globals.ShowDialog(apptVm as Conductor<object>); 
            //}
            //else
            //{
            //    apptVm.CreateNewAppointment();
            //    apptVm.SetCurrentPatient(patientRegDetail.PatientRegistration.Patient, patientRegDetail.ServiceRecID);
            //}

            //Globals.ShowDialog(apptVm as Conductor<object>);

            Action<IAddEditAppointment> onInitDlg = (apptVm) =>
            {
                var patientRegDetail = eventArgs.Value as PatientRegistrationDetail;
                apptVm.RegistrationID = patientRegDetail.PtRegistrationID != null ? patientRegDetail.PtRegistrationID.Value : 0;
                if (patientRegDetail.patientAppointment != null)
                {
                    patientRegDetail.patientAppointment.Patient = patientRegDetail.PatientRegistration.Patient;
                    apptVm.SetCurrentAppointment(patientRegDetail.patientAppointment);
                    //Globals.ShowDialog(apptVm as Conductor<object>);
                }
                else
                {
                    apptVm.CreateNewAppointment();
                    apptVm.SetCurrentPatient(patientRegDetail.PatientRegistration.Patient, patientRegDetail.ServiceRecID);
                }
            };
            GlobalsNAV.ShowDialog<IAddEditAppointment>(onInitDlg);
        }

        public void Handle(AddCompleted<PatientAppointment> message)
        {
            if (message != null)
            {
                SearchRegisDetailPrescription(RegistrationDetails.PageIndex, RegistrationDetails.PageSize, true); 
            }
        }

        public void Handle(AppointmentAddEditCloseEvent message)
        {
            if (message != null)
            {
                SearchRegisDetailPrescription(RegistrationDetails.PageIndex, RegistrationDetails.PageSize, true);
            }
        }
    }
}

