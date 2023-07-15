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
using aEMR.DataContracts;
using aEMR.Common;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using eHCMS.CommonUserControls.CommonTasks;
using System.Windows;
using aEMR.ServiceClient;
using System.Windows.Controls;

namespace aEMR.Appointment.ViewModels
{
    [Export(typeof(IPatientApptServicesDetails)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientApptServicesDetailsViewModel : Conductor<object>, IPatientApptServicesDetails        
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

        private string _TitleForm = "DANH SÁCH HẸN CHI TIẾT DỊCH VỤ";
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
        public PatientApptServicesDetailsViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Globals.EventAggregator.Subscribe(this);

            SearchRegCriteria = new PatientApptServiceDetailsSearchCriteria();
            SearchRegCriteria.FromDate = DateTime.Now.Date;
            SearchRegCriteria.ToDate = DateTime.Now.Date;

            //if (!DesignerProperties.IsInDesignTool)
            bool designTime = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());
            if (!designTime)
            {
                // Txd 25/05/2014 Replaced ConfigList
                Coroutine.BeginExecute(DoLoadLocations(Globals.ServerConfigSection.Hospitals.KhoaPhongKham));
                LoadServices((long)V_AppointmentType.HenTaiKham);
            }
            ApptServicesDetails = new PagedSortableCollectionView<PatientApptServiceDetails>();
            //ApptServicesDetails.OnRefresh += ApptServicesDetails_OnRefresh;
            ApptServicesDetails.PageIndex = 0;
            ApptServicesDetails.PageSize = 100;
            //ApptServicesDetails.OnRefresh += new EventHandler<RefreshEventArgs>(ApptServicesDetails_OnRefresh);
        }

        void ApptServicesDetails_OnRefresh(object sender, aEMR.Common.Collections.RefreshEventArgs e)
        {
            SearchRegisDetailPrescription(ApptServicesDetails.PageIndex, ApptServicesDetails.PageSize, false);            
        }

        public void btnSearchAppointments()
        {
            SearchRegisDetailPrescription(ApptServicesDetails.PageIndex, ApptServicesDetails.PageSize, false);
        }

        private PatientApptServiceDetailsSearchCriteria _searchRegCriteria;
        public PatientApptServiceDetailsSearchCriteria SearchRegCriteria
        {
            get { return _searchRegCriteria; }
            set
            {
                _searchRegCriteria = value;
                NotifyOfPropertyChange(() => SearchRegCriteria);
            }
        }

        private PagedSortableCollectionView<PatientApptServiceDetails> _ApptServicesDetails;
        public PagedSortableCollectionView<PatientApptServiceDetails> ApptServicesDetails
        {
            get { return _ApptServicesDetails; }
            private set
            {
                _ApptServicesDetails = value;
                NotifyOfPropertyChange(() => ApptServicesDetails);
            }
        }

        private void SearchRegisDetailPrescription(int pageIndex, int pageSize, bool bCountTotal)
        {
            IsProcessing = true;
            if (Globals.DeptLocation == null)
            {
                ApptServicesDetails.Clear();
                return;
            }
            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new AppointmentServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPatientApptServiceDetailsGetAll(_searchRegCriteria, pageIndex, pageSize,"" ,bCountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int totalCount = 0;
                            ApptServicesDetails.Clear();
                            bool bOK = false;
                            try
                            {
                                var allItems = client.EndPatientApptServiceDetailsGetAll(out totalCount, asyncResult);
                                bOK = true;
                                ApptServicesDetails.TotalItemCount = totalCount;
                                if (bOK)
                                {
                                    if (bCountTotal)
                                    {
                                        ApptServicesDetails.TotalItemCount = totalCount;
                                    }
                                    if (allItems != null)
                                    {
                                        foreach (var item in allItems)
                                        {
                                            ApptServicesDetails.Add(item);
                                        }
                                    }
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
                            
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {
                    IsProcessing = false;
                    Globals.IsBusy = false;
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
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

        private ObservableCollection<RefMedicalServiceItem> _medicalServicesList;
        public ObservableCollection<RefMedicalServiceItem> MedicalServicesList
        {
            get
            {
                return _medicalServicesList;
            }
            set
            {
                if (_medicalServicesList != value)
                {
                    _medicalServicesList = value;
                    NotifyOfPropertyChange(() => MedicalServicesList);
                }
            }
        }

        public void LoadServices(long appointmentTypeID)
        {
            //IsProcessing = true;
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new AppointmentServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllServicesByAppointmentType(appointmentTypeID,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    MedicalServicesList = contract.EndGetAllServicesByAppointmentType(asyncResult);
                                    MedicalServicesList.Insert(0, new RefMedicalServiceItem { MedServiceID=-1,
                                    MedServiceName="--Chọn một dịch vụ--"});
                                    //Default
                                    //MedicalServicesSelected = MedicalServicesList.Where(s => s.MedServiceID == 412).FirstOrDefault();
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                    MedicalServicesList = null;
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                    MedicalServicesList = null;
                                }
                                finally
                                {
                                    //IsProcessing = false;
                                    this.HideBusyIndicator();
                                }

                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MedicalServicesList = null;
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    //Globals.IsBusy = false;
                }
            });
            t.Start();
        }



        public void Handle(DoubleClick message)
        {
            //if (message.Source != AppointmentListingContent)
            //    return;

            //var apptVm = Globals.GetViewModel<IAddEditAppointment>();
            //apptVm.SetCurrentAppointment(AppointmentListingContent.SelectedAppointment);

            //Globals.ShowDialog(apptVm as Conductor<object>);
        }

        public void DoubleClick(object args)
        {
            //EventArgs<object> eventArgs = args as EventArgs<object>;
            
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
        }

        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

    }
}

