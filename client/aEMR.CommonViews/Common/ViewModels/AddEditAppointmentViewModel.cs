//using eHCMSLanguage;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Linq;
//using System.ServiceModel;
//using System.Text;
//using System.Threading;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.ComponentModel.Composition;
//using Caliburn.Micro;
//using DataEntities;
//using Service.Core.Common;
//using aEMR.ViewContracts;
//using aEMR.Common.BaseModel;
//using aEMR.Infrastructure.Events;
//using Castle.Windsor;
//using aEMR.Infrastructure.CachingUtils;
//using aEMR.Infrastructure;
//using aEMR.ServiceClient;
//using aEMR.DataContracts;
//using aEMR.Common.Collections;
//using aEMR.CommonTasks;
//using aEMR.Controls;
//using aEMR.Common.Printing;

//namespace aEMR.Common.ViewModels
//{
//    [Export(typeof(IAddEditAppointment)), PartCreationPolicy(CreationPolicy.NonShared)]
//    public class AddEditAppointmentViewModel : ViewModelBase, IAddEditAppointment
//        , IHandle<ItemEdited<PatientApptPCLRequests>>
//    {
//        private FormState _currentFormState;
//        public FormState CurrentFormState
//        {
//            get
//            {
//                return _currentFormState;
//            }
//            set
//            {
//                if (_currentFormState != value)
//                {
//                    _currentFormState = value;
//                    NotifyOfPropertyChange(() => CurrentFormState);
//                }
//            }
//        }

//        //Lưu ngày hẹn bệnh để dành kiểm tra.
//        //Nếu cuộc hẹn đã được lưu, sau đó thêm PCL và đổi ngày hẹn thì khi lưu sẽ kiểm tra những PCL đã lưu và chưa lưu.
//        //Nếu cuộc hẹn đã được lưu, sau đó thêm PCL nhưng không đổi ngày hẹn thì chỉ kiểm tra những PCL chưa lưu.
//        private DateTime? _apptDateOrig;
//        public DateTime? ApptDateOrig
//        {
//            get
//            {
//                return _apptDateOrig;
//            }
//            set
//            {
//                _apptDateOrig = value;
//            }
//        }


//        #region busy indicator status
//        public override bool IsProcessing
//        {
//            get
//            {
//                return _isLoadingLocations
//                    || _isLoadingServiceList
//                    || _isLoadingApptTypeList
//                    || _isLoadingSegments
//                    || _isLoadingApptStatusList
//                    || _isLoadingSegmentsPCL
//                    || _isLoadingPclForm
//                    || _isLoadingPclExamTypes
//                    || _isLoadingLocationsPCLLAB
//                    || _isLoadingLocationsPCLNotLAB
//                    || _isSaving
//                    || _isCheckingApptExist
//                    || _isLoadingPatientApptPCLRequestDetails
//                    || _waitingCheckSpace;
//            }
//        }
//        public override string StatusText
//        {
//            get
//            {
//                if (_isLoadingLocations)
//                {
//                    return eHCMSResources.Z1006_G1_LoadDSPhg;
//                }
//                if (_isLoadingServiceList)
//                {
//                    return eHCMSResources.Z1007_G1_LoadDSDV;
//                }
//                if (_isLoadingApptTypeList)
//                {
//                    return eHCMSResources.Z1008_G1_LoadDSLoaiHen;
//                }
//                if (_isLoadingSegments)
//                {
//                    return eHCMSResources.Z1009_G1_LoadTGian;
//                }
//                if (_isLoadingApptStatusList)
//                {
//                    return eHCMSResources.Z1010_G1_LoadTThaiCHen;
//                }
//                if (_isLoadingSegmentsPCL)
//                {
//                    return eHCMSResources.Z1011_G1_LoadTGianPCL;
//                }
//                if (_isLoadingPclForm)
//                {
//                    return eHCMSResources.Z1012_G1_LoadPCLForm;
//                }
//                if (_isLoadingPclExamTypes)
//                {
//                    return eHCMSResources.Z1013_G1_LoadXN;
//                }
//                if (_isLoadingLocationsPCLLAB)
//                {
//                    return eHCMSResources.Z1014_G1_LoadPhgLaboratory;
//                }
//                if (_isLoadingLocationsPCLNotLAB)
//                {
//                    return eHCMSResources.Z1033_G1_LoadPhgPCL;
//                }
//                if (_isCheckingApptExist)
//                {
//                    return eHCMSResources.Z1015_G1_DangKTraNgHen;
//                }
//                if (_isSaving)
//                {
//                    return eHCMSResources.Z1016_G1_DangLuuCuocHen;
//                }
//                if (_isLoadingPatientApptPCLRequestDetails)
//                {
//                    return eHCMSResources.Z1017_G1_LoadCTietXN;
//                }
//                if (_waitingCheckSpace)
//                {
//                    return eHCMSResources.Z1018_G1_DangKTraChoTrongDeHen;
//                }
//                return string.Empty;
//            }
//        }

//        private bool _isLoadingLocations;
//        public bool IsLoadingLocations
//        {
//            get { return _isLoadingLocations; }
//            set
//            {
//                if (_isLoadingLocations != value)
//                {
//                    _isLoadingLocations = value;
//                    NotifyOfPropertyChange(() => IsLoadingLocations);
//                    NotifyWhenBusy();
//                }
//            }
//        }


//        private bool _isLoadingLocationsPCLLAB;
//        public bool IsLoadingLocationsPCLLAB
//        {
//            get { return _isLoadingLocationsPCLLAB; }
//            set
//            {
//                if (_isLoadingLocationsPCLLAB != value)
//                {
//                    _isLoadingLocationsPCLLAB = value;
//                    NotifyOfPropertyChange(() => IsLoadingLocationsPCLLAB);
//                    NotifyWhenBusy();
//                }
//            }
//        }

//        private bool _isLoadingLocationsPCLNotLAB;
//        public bool IsLoadingLocationsPCLNotLAB
//        {
//            get { return _isLoadingLocationsPCLNotLAB; }
//            set
//            {
//                if (_isLoadingLocationsPCLNotLAB != value)
//                {
//                    _isLoadingLocationsPCLNotLAB = value;
//                    NotifyOfPropertyChange(() => IsLoadingLocationsPCLNotLAB);
//                    NotifyWhenBusy();
//                }
//            }
//        }

//        private DateTime _curAppDate;
//        public DateTime curAppDate
//        {
//            get { return _curAppDate; }
//            set
//            {
//                if (_curAppDate != value)
//                {
//                    _curAppDate = value;
//                    NotifyOfPropertyChange(() => curAppDate);
//                }
//            }
//        }

//        private bool _isLoadingServiceList;
//        public bool IsLoadingServiceList
//        {
//            get { return _isLoadingServiceList; }
//            set
//            {
//                if (_isLoadingServiceList != value)
//                {
//                    _isLoadingServiceList = value;
//                    NotifyOfPropertyChange(() => IsLoadingServiceList);
//                    NotifyWhenBusy();
//                }
//            }
//        }

//        private bool _isLoadingApptTypeList;
//        public bool IsLoadingApptTypeList
//        {
//            get { return _isLoadingApptTypeList; }
//            set
//            {
//                if (_isLoadingApptTypeList != value)
//                {
//                    _isLoadingApptTypeList = value;
//                    NotifyOfPropertyChange(() => IsLoadingApptTypeList);
//                    NotifyWhenBusy();
//                }
//            }
//        }


//        private bool _isLoadingApptStatusList;
//        public bool IsLoadingApptStatusList
//        {
//            get { return _isLoadingApptStatusList; }
//            set
//            {
//                if (_isLoadingApptStatusList != value)
//                {
//                    _isLoadingApptStatusList = value;
//                    NotifyOfPropertyChange(() => IsLoadingApptStatusList);
//                    NotifyWhenBusy();
//                }
//            }
//        }


//        private bool _isLoadingSegments;
//        public bool IsLoadingSegments
//        {
//            get { return _isLoadingSegments; }
//            set
//            {
//                if (_isLoadingSegments != value)
//                {
//                    _isLoadingSegments = value;
//                    NotifyOfPropertyChange(() => IsLoadingSegments);
//                    NotifyWhenBusy();
//                }
//            }
//        }


//        private bool _isLoadingSegmentsPCL;
//        public bool IsLoadingSegmentsPCL
//        {
//            get { return _isLoadingSegmentsPCL; }
//            set
//            {
//                if (_isLoadingSegmentsPCL != value)
//                {
//                    _isLoadingSegmentsPCL = value;
//                    NotifyOfPropertyChange(() => IsLoadingSegmentsPCL);
//                    NotifyWhenBusy();
//                }
//            }
//        }


//        private bool _isLoadingPclForm;
//        public bool IsLoadingPclForm
//        {
//            get { return _isLoadingPclForm; }
//            set
//            {
//                if (_isLoadingPclForm != value)
//                {
//                    _isLoadingPclForm = value;
//                    NotifyOfPropertyChange(() => IsLoadingPclForm);
//                    NotifyWhenBusy();
//                }
//            }
//        }



//        private bool _isLoadingPclExamTypes;
//        public bool IsLoadingPclExamTypes
//        {
//            get { return _isLoadingPclExamTypes; }
//            set
//            {
//                if (_isLoadingPclExamTypes != value)
//                {
//                    _isLoadingPclExamTypes = value;
//                    NotifyOfPropertyChange(() => IsLoadingPclExamTypes);
//                    NotifyWhenBusy();
//                }
//            }
//        }



//        private bool _isLoadingPatientApptPCLRequestDetails;
//        public bool IsLoadingPatientApptPCLRequestDetails
//        {
//            get { return _isLoadingPatientApptPCLRequestDetails; }
//            set
//            {
//                if (_isLoadingPatientApptPCLRequestDetails != value)
//                {
//                    _isLoadingPatientApptPCLRequestDetails = value;
//                    NotifyOfPropertyChange(() => IsLoadingPatientApptPCLRequestDetails);
//                    NotifyWhenBusy();
//                }
//            }
//        }


//        private bool _isSaving;
//        public bool IsSaving
//        {
//            get { return _isSaving; }
//            set
//            {
//                if (_isSaving != value)
//                {
//                    _isSaving = value;
//                    NotifyOfPropertyChange(() => IsSaving);
//                    NotifyWhenBusy();
//                }
//            }
//        }



//        private bool _isCheckingApptExist;
//        public bool IsCheckingApptExist
//        {
//            get
//            {
//                return _isCheckingApptExist;
//            }
//            set
//            {
//                if (_isCheckingApptExist != value)
//                {
//                    _isCheckingApptExist = value;
//                    NotifyOfPropertyChange(() => IsCheckingApptExist);
//                    NotifyWhenBusy();
//                }
//            }
//        }

//        private bool _waitingCheckSpace;
//        public bool WaitingCheckSpace
//        {
//            get
//            {
//                return _waitingCheckSpace;
//            }
//            set
//            {
//                if (_waitingCheckSpace != value)
//                {
//                    _waitingCheckSpace = value;
//                    NotifyOfPropertyChange(() => WaitingCheckSpace);
//                    NotifyWhenBusy();
//                }
//            }
//        }
//        #endregion
//        [ImportingConstructor]
//        public AddEditAppointmentViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
//        {
//            eventArg.Subscribe(this);
//            authorization();
//        }
//        protected override void OnActivate()
//        {
//            base.OnActivate();
//            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
//            {
//                CurrentAppointment = new PatientAppointment();
//                //CurrentAppointment.PropertyChanged += new PropertyChangedEventHandler(CurrentAppointment_PropertyChanged);

//                //KMx: Biến này giữ lại cuộc hẹn sau khi lưu. Dùng để kiểm tra khi bấm nút in BHYT (29/08/2014 17:25).
//                CurrentAppointmentCopy = new PatientAppointment();

//                DeptLocationsList = new ObservableCollection<DeptLocation>();

//                MedicalServicesSelected = new RefMedicalServiceItem { MedServiceID = 412 };

//                AppointmentSegmentsSelectedPCL = new ConsultationTimeSegments { ConsultationTimeSegmentID = -1 };

//                AppointmentTypeSelected = new Lookup { LookupID = -1 };

//                SelectedPclExamTypeLocation = new PCLExamTypeLocation { DeptLocationID = -1 };

//                LoadApptTypeList();
//                LoadApptStatusList();
//            }
//        }

//        //void CurrentAppointment_PropertyChanged(object sender, PropertyChangedEventArgs e)
//        //{
//        //    if (e.PropertyName == "HasChronicDisease")
//        //    {
//        //        if (CurrentAppointment.HasChronicDisease == null
//        //            || CurrentAppointment.HasChronicDisease.Value)
//        //        {
//        //            hasSaved = false;
//        //        }
//        //    }
//        //}
//        public void authorization()
//        {
//            if (!Globals.isAccountCheck)
//            {
//                return;
//            }
//            mPrintHIAppointment_New = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
//                                               , (int)eConsultation.mPteAppointmentTab,
//                                               (int)oConsultationEx.mPrintHIAppointment_New, (int)ePermission.mView);
//        }
//        private bool _mPrintHIAppointment_New = true;
//        public bool mPrintHIAppointment_New
//        {
//            get
//            {
//                return _mPrintHIAppointment_New;
//            }
//            set
//            {
//                if (_mPrintHIAppointment_New != value)
//                {
//                    _mPrintHIAppointment_New = value;
//                    NotifyOfPropertyChange(() => mPrintHIAppointment_New);
//                }
//            }
//        }

//        private ConsultationTimeSegments _appointmentSegmentsSelected;
//        public ConsultationTimeSegments AppointmentSegmentsSelected
//        {
//            get
//            {
//                return _appointmentSegmentsSelected;
//            }
//            set
//            {
//                if (_appointmentSegmentsSelected != value)
//                {
//                    _appointmentSegmentsSelected = value;
//                    NotifyOfPropertyChange(() => AppointmentSegmentsSelected);
//                }
//            }
//        }

//        private ObservableCollection<ConsultationTimeSegments> _appointmentSegmentsList;
//        public ObservableCollection<ConsultationTimeSegments> AppointmentSegmentsList
//        {
//            get
//            {
//                return _appointmentSegmentsList;
//            }
//            set
//            {
//                if (_appointmentSegmentsList != value)
//                {
//                    _appointmentSegmentsList = value;
//                    NotifyOfPropertyChange(() => AppointmentSegmentsList);
//                }
//            }
//        }

//        private long _RegistrationID;
//        public long RegistrationID
//        {
//            get
//            {
//                return _RegistrationID;
//            }
//            set
//            {
//                if (_RegistrationID != value)
//                {
//                    _RegistrationID = value;
//                    NotifyOfPropertyChange(() => RegistrationID);
//                }
//            }
//        }

//        private bool _isCreateApptFromConsultation;
//        public bool IsCreateApptFromConsultation
//        {
//            get
//            {
//                return _isCreateApptFromConsultation;
//            }
//            set
//            {
//                if (_isCreateApptFromConsultation != value)
//                {
//                    _isCreateApptFromConsultation = value;
//                    NotifyOfPropertyChange(() => IsCreateApptFromConsultation);
//                }
//            }
//        }

//        //KMx: Đổi sang sử dụng biến CurrentAppointmentCopy để kiểm tra xem có được phép in giấy hẹn BHYT hay không (30/08/2014 11:31).
//        //private bool _hasSaved = false;
//        //public bool hasSaved
//        //{
//        //    get
//        //    {
//        //        return _hasSaved;
//        //    }
//        //    set
//        //    {
//        //        if (_hasSaved != value)
//        //        {
//        //            _hasSaved = value;
//        //            NotifyOfPropertyChange(() => hasSaved);
//        //        }
//        //    }
//        //}

//        private void Segments_WithAppDateDeptLocIDSeqNumber(long DeptLocationID, DateTime ApptDate)
//        {
//            IsLoadingSegments = true;

//            var t = new Thread(() =>
//            {
//                using (var serviceFactory = new AppointmentServiceClient())
//                {
//                    var contract = serviceFactory.ServiceInstance;
//                    contract.BeginSegments_WithAppDateDeptLocIDSeqNumber(DeptLocationID, ApptDate, Globals.DispatchCallback((asyncResult) =>
//                    {
//                        try
//                        {
//                            var results = contract.EndSegments_WithAppDateDeptLocIDSeqNumber(asyncResult);
//                            if (results != null && results.Count > 0)
//                            {
//                                AppointmentSegmentsList = new ObservableCollection<ConsultationTimeSegments>(results);

//                                if (AppointmentSegmentsList != null && AppointmentSegmentsList.Count > 0)
//                                {
//                                    //KMx: Bác sĩ yêu cầu chọn mặc định ca nào còn trống (16/01/2016 14:48).
//                                    //AppointmentSegmentsSelected = AppointmentSegmentsList[0];
//                                    AppointmentSegmentsSelected = AppointmentSegmentsList.Where(x => x.ApptdayMaxNumConsultationAllowed > 0 && x.NumberOfSeq < x.ApptdayMaxNumConsultationAllowed).FirstOrDefault();
//                                }
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//                        }
//                        finally
//                        {
//                            IsLoadingSegments = false;
//                        }

//                    }), null);

//                }

//            });

//            t.Start();
//        }

//        private void ConsultationTimeSegments_ByDeptLocationID(long DeptLocationID, PatientApptServiceDetails RowInfo)
//        {

//            IsLoadingSegments = true;

//            var t = new Thread(() =>
//            {
//                using (var serviceFactory = new ClinicManagementServiceClient())
//                {
//                    var contract = serviceFactory.ServiceInstance;
//                    contract.BeginConsultationTimeSegments_ByDeptLocationID(DeptLocationID, Globals.DispatchCallback((asyncResult) =>
//                    {
//                        try
//                        {
//                            var results = contract.EndConsultationTimeSegments_ByDeptLocationID(asyncResult);

//                            if (RowInfo.ApptTimeSegmentList != null)
//                                RowInfo.ApptTimeSegmentList.Clear();

//                            if (results != null && results.Count > 0)
//                            {
//                                RowInfo.ApptTimeSegmentList = new ObservableCollection<ConsultationTimeSegments>(results);

//                                var firstItem = new ConsultationTimeSegments { ConsultationTimeSegmentID = -1, SegmentName = eHCMSResources.A0015_G1_Chon };
//                                var gio = new DateTime(1900, 01, 01, 0, 0, 0);
//                                firstItem.StartTime = gio;
//                                firstItem.EndTime = gio;

//                                RowInfo.ApptTimeSegmentList.Insert(0, firstItem);

//                                RowInfo.ApptTimeSegment = new ConsultationTimeSegments();
//                                RowInfo.ApptTimeSegment = firstItem;
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//                        }
//                        finally
//                        {
//                            IsLoadingSegments = false;
//                        }

//                    }), null);

//                }

//            });

//            t.Start();
//        }

//        private DeptLocation _deptLocationsSelected;
//        public DeptLocation DeptLocationsSelected
//        {
//            get
//            {
//                return _deptLocationsSelected;
//            }
//            set
//            {
//                if (_deptLocationsSelected != value)
//                {
//                    _deptLocationsSelected = value;
//                    NotifyOfPropertyChange(() => DeptLocationsSelected);

//                    if (DeptLocationsSelected != null && DeptLocationsSelected.DeptLocationID > 0
//                        && CurrentAppointment != null)
//                    {
//                        if (CurrentAppointment.ApptDate != null)
//                        {
//                            Segments_WithAppDateDeptLocIDSeqNumber(DeptLocationsSelected.DeptLocationID, CurrentAppointment.ApptDate.Value);
//                        }
//                    }
//                    else
//                    {
//                        if (AppointmentSegmentsList != null)
//                        {
//                            AppointmentSegmentsList.Clear();
//                        }
//                    }
//                }
//            }
//        }

//        private ObservableCollection<DeptLocation> _deptLocationsList;
//        public ObservableCollection<DeptLocation> DeptLocationsList
//        {
//            get
//            {
//                return _deptLocationsList;
//            }
//            set
//            {
//                if (_deptLocationsList != value)
//                {
//                    _deptLocationsList = value;
//                    NotifyOfPropertyChange(() => DeptLocationsList);
//                }
//            }
//        }

//        private DeptLocation _selectedLocation;
//        public DeptLocation SelectedLocation
//        {
//            get
//            {
//                return _selectedLocation;
//            }
//            set
//            {
//                _selectedLocation = value;
//                NotifyOfPropertyChange(() => SelectedLocation);
//            }
//        }

//        public void LoadLocations(long serviceID)
//        {
//            IsLoadingLocations = true;

//            var t = new Thread(() =>
//            {
//                try
//                {
//                    IsLoadingLocations = true;
//                    using (var serviceFactory = new AppointmentServiceClient())
//                    {
//                        var contract = serviceFactory.ServiceInstance;

//                        contract.BeginGetAllDeptLocationsByService(serviceID,
//                            Globals.DispatchCallback(asyncResult =>
//                            {
//                                try
//                                {
//                                    var Res = contract.EndGetAllDeptLocationsByService(asyncResult);

//                                    if (DeptLocationsList != null)
//                                        DeptLocationsList.Clear();

//                                    DeptLocationsList = new ObservableCollection<DeptLocation>(Res);

//                                    IEnumerable<DeptLocation> DeptLocDefault = (from d in DeptLocationsList
//                                                                                where d.DeptLocationID == Globals.DeptLocation.DeptLocationID
//                                                                                select d);
//                                    if (DeptLocDefault != null && DeptLocDefault.Count() > 0)
//                                    {
//                                        DeptLocationsSelected = DeptLocDefault.FirstOrDefault();
//                                    }
//                                    else
//                                    {
//                                        DeptLocationsSelected = DeptLocationsList.FirstOrDefault();
//                                    }


//                                }
//                                catch (FaultException<AxException> fault)
//                                {
//                                    ClientLoggerHelper.LogInfo(fault.ToString());
//                                    DeptLocationsList = null;
//                                }
//                                catch (Exception ex)
//                                {
//                                    ClientLoggerHelper.LogInfo(ex.ToString());
//                                    DeptLocationsList = null;
//                                }
//                                finally
//                                {
//                                    IsLoadingLocations = false;
//                                }
//                            }), null);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    DeptLocationsList = null;
//                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//                }
//            });
//            t.Start();
//        }

//        private RefMedicalServiceItem _medicalServicesSelected;
//        public RefMedicalServiceItem MedicalServicesSelected
//        {
//            get { return _medicalServicesSelected; }
//            set
//            {
//                if (_medicalServicesSelected != value)
//                {
//                    _medicalServicesSelected = value;
//                    NotifyOfPropertyChange(() => MedicalServicesSelected);

//                    if (MedicalServicesSelected != null && MedicalServicesSelected.MedServiceID > 0)
//                    {
//                        LoadLocations(MedicalServicesSelected.MedServiceID);
//                    }
//                    else
//                    {
//                        if (DeptLocationsList != null)
//                            DeptLocationsList.Clear();
//                        if (AppointmentSegmentsList != null)
//                            AppointmentSegmentsList.Clear();
//                    }
//                }
//            }
//        }

//        private ObservableCollection<RefMedicalServiceItem> _medicalServicesList;
//        public ObservableCollection<RefMedicalServiceItem> MedicalServicesList
//        {
//            get
//            {
//                return _medicalServicesList;
//            }
//            set
//            {
//                if (_medicalServicesList != value)
//                {
//                    _medicalServicesList = value;
//                    NotifyOfPropertyChange(() => MedicalServicesList);
//                }
//            }
//        }

//        public void LoadServices(long appointmentTypeID)
//        {
//            IsLoadingServiceList = true;

//            var t = new Thread(() =>
//            {
//                try
//                {
//                    using (var serviceFactory = new AppointmentServiceClient())
//                    {
//                        var contract = serviceFactory.ServiceInstance;

//                        contract.BeginGetAllServicesByAppointmentType(appointmentTypeID,
//                            Globals.DispatchCallback(asyncResult =>
//                            {
//                                try
//                                {
//                                    MedicalServicesList = contract.EndGetAllServicesByAppointmentType(asyncResult);
//                                    //Default
//                                    MedicalServicesSelected = MedicalServicesList.Where(s => s.MedServiceID == 412).FirstOrDefault();
//                                }
//                                catch (FaultException<AxException> fault)
//                                {
//                                    ClientLoggerHelper.LogInfo(fault.ToString());
//                                    MedicalServicesList = null;
//                                }
//                                catch (Exception ex)
//                                {
//                                    ClientLoggerHelper.LogInfo(ex.ToString());
//                                    MedicalServicesList = null;
//                                }
//                                finally
//                                {
//                                    IsLoadingServiceList = false;
//                                }

//                            }), null);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    MedicalServicesList = null;
//                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//                }
//                finally
//                {
//                    IsLoadingServiceList = false;
//                    Globals.IsBusy = false;
//                }
//            });
//            t.Start();
//        }

//        private Lookup _appointmentTypeSelected;
//        public Lookup AppointmentTypeSelected
//        {
//            get { return _appointmentTypeSelected; }
//            set
//            {
//                if (_appointmentTypeSelected != value)
//                {
//                    _appointmentTypeSelected = value;
//                    NotifyOfPropertyChange(() => AppointmentTypeSelected);

//                    if (AppointmentTypeSelected != null && AppointmentTypeSelected.LookupID > 0)
//                    {
//                        LoadServices(AppointmentTypeSelected.LookupID);
//                    }
//                    else
//                    {
//                        DeptLocationsList.Clear();
//                    }
//                }
//            }
//        }


//        private ObservableCollection<Lookup> _appointmentTypeList;
//        public ObservableCollection<Lookup> AppointmentTypeList
//        {
//            get { return _appointmentTypeList; }
//            set
//            {
//                _appointmentTypeList = value;
//                NotifyOfPropertyChange(() => AppointmentTypeList);
//            }
//        }

//        public void LoadApptTypeList()
//        {
//            IsLoadingApptTypeList = true;

//            var t = new Thread(() =>
//            {
//                try
//                {
//                    using (var serviceFactory = new CommonService_V2Client())
//                    {
//                        var contract = serviceFactory.ServiceInstance;

//                        contract.BeginGetAllLookupValuesByType(LookupValues.APPOINTMENT_TYPE, Globals.DispatchCallback(asyncResult =>
//                            {
//                                try
//                                {
//                                    var allItems = contract.EndGetAllLookupValuesByType(asyncResult);
//                                    AppointmentTypeList = new ObservableCollection<Lookup>(allItems);

//                                    //Default
//                                    //var firstItem = new Lookup {LookupID = -1, ObjectValue = eHCMSResources.A0015_G1_Chon};
//                                    //AppointmentTypeList.Insert(0, firstItem);

//                                    if (allItems != null && allItems.Count > 0)
//                                    {
//                                        AppointmentTypeSelected = allItems[0];
//                                    }

//                                }
//                                catch (FaultException<AxException> fault)
//                                {
//                                    ClientLoggerHelper.LogInfo(fault.ToString());
//                                }
//                                catch (Exception ex)
//                                {
//                                    ClientLoggerHelper.LogInfo(ex.ToString());
//                                }
//                                finally
//                                {
//                                    IsLoadingApptTypeList = false;
//                                }

//                            }), null);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Globals.ShowMessage(ex.Message, eHCMSResources.T0074_G1_I);
//                }
//            });
//            t.Start();
//        }

//        public void LoadApptStatusList()
//        {

//            IsLoadingApptStatusList = true;

//            var t = new Thread(() =>
//            {
//                try
//                {
//                    using (var serviceFactory = new CommonService_V2Client())
//                    {
//                        var contract = serviceFactory.ServiceInstance;

//                        contract.BeginGetAllLookupValuesByType(LookupValues.APPT_STATUS, Globals.DispatchCallback(asyncResult =>
//                            {
//                                try
//                                {
//                                    var allItems = contract.EndGetAllLookupValuesByType(asyncResult);
//                                    AppointmentStatusList = new ObservableCollection<Lookup>(allItems);
//                                }
//                                catch (FaultException<AxException> fault)
//                                {
//                                    ClientLoggerHelper.LogInfo(fault.ToString());
//                                }
//                                catch (Exception ex)
//                                {
//                                    ClientLoggerHelper.LogInfo(ex.ToString());
//                                }
//                                finally
//                                {
//                                    IsLoadingApptStatusList = false;
//                                }
//                            }), null);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Globals.ShowMessage(ex.Message, eHCMSResources.T0074_G1_I);
//                }
//            });
//            t.Start();
//        }

//        private Patient _currentPatient;
//        public Patient CurrentPatient
//        {
//            get
//            {
//                return _currentPatient;
//            }
//            set
//            {
//                if (_currentPatient != value)
//                {
//                    _currentPatient = value;
//                    NotifyOfPropertyChange(() => CurrentPatient);
//                }
//            }
//        }

//        public void SetCurrentPatient(Patient patient)
//        {
//            CurrentPatient = patient;
//        }

//        public void SetCurrentPatient(Patient patient, long serviceRecID)
//        {
//            CurrentPatient = patient;
//            ServiceRecID = serviceRecID;
//            CurrentAppointment.ServiceRecID = serviceRecID;
//        }

//        private PatientAppointment _currentAppointment;
//        public PatientAppointment CurrentAppointment
//        {
//            get
//            {
//                return _currentAppointment;
//            }
//            set
//            {
//                if (_currentAppointment != value)
//                {
//                    _currentAppointment = value;
//                    if (CurrentAppointment != null)
//                    {
//                        //CurrentAppointment.PropertyChanged += new PropertyChangedEventHandler(CurrentAppointment_PropertyChanged);
//                        //KMx: (29/08/2014 13:51).
//                        //hasSaved = CurrentAppointment.HasChronicDisease.GetValueOrDefault();
//                        //hasSaved = CurrentAppointment.AllowPaperReferralUseNextConsult && (CurrentAppointment.HasChronicDisease == true || !string.IsNullOrWhiteSpace(CurrentAppointment.ReasonToAllowPaperReferral));

//                        // TxD 03/02/2015: New or existing Appointment now looking at the current Registration to allow ticking the 'Allow using paper referral for next consultation'
//                        if (Registration_DataStorage.CurrentPatientRegistration != null && Registration_DataStorage.CurrentPatientRegistration.HisID.HasValue &&
//                            Registration_DataStorage.CurrentPatientRegistration.HisID > 0 && Registration_DataStorage.CurrentPatientRegistration.IsCrossRegion.HasValue &&
//                            Registration_DataStorage.CurrentPatientRegistration.IsCrossRegion.Value == false)
//                        {
//                            CurrentAppointment.isHIAppt = true;
//                        }

//                        if (Registration_DataStorage.CurrentPatientRegistration != null && Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID > 0 && 
//                            Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
//                        {
//                            CurrentAppointment.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
//                            CurrentAppointment.CreatedByInPtRegis = true;
//                        }

//                    }

//                    NotifyOfPropertyChange(() => CurrentAppointment);
//                }
//            }
//        }

//        //KMx: Biến này dùng để kiểm tra xem cuộc hẹn được lưu ở DB có cho in giấy hẹn BHYT không.
//        //Vì nếu dựa vào CurrentAppointment thì không đúng do có thể người dùng chỉnh sửa nhưng chưa lưu (29/08/2014 16:34).
//        private PatientAppointment _currentAppointmentCopy;
//        public PatientAppointment CurrentAppointmentCopy
//        {
//            get
//            {
//                return _currentAppointmentCopy;
//            }
//            set
//            {
//                if (_currentAppointmentCopy != value)
//                {
//                    _currentAppointmentCopy = value;
//                    NotifyOfPropertyChange(() => CurrentAppointmentCopy);
//                }
//            }
//        }

//        private bool _HasDone = true;
//        public bool HasDone
//        {
//            get
//            {
//                return _HasDone;
//            }
//            set
//            {
//                if (_HasDone != value)
//                {
//                    _HasDone = value;
//                    NotifyOfPropertyChange(() => HasDone);
//                    NotifyOfPropertyChange(() => DetailCuocHenIsEnabled);
//                }
//            }
//        }

//        private bool _IsCanEdit = true;
//        public bool IsCanEdit
//        {
//            get
//            {
//                return _IsCanEdit &&
//                    CurrentAppointment != null
//                        && CurrentAppointment.ApptDate != null
//                        && CurrentAppointment.ApptDate.Value.Date.Subtract(DateServer.Date).Days >= 0;
//            }

//        }

//        private bool _butCLS = true;
//        public bool butCLS
//        {
//            get
//            {
//                return _butCLS && _IsCanEdit;
//            }
//            set
//            {
//                if (_butCLS != value)
//                {
//                    _butCLS = value;
//                    NotifyOfPropertyChange(() => butCLS);
//                }
//            }
//        }

//        private long _ServiceRecID;
//        public long ServiceRecID
//        {
//            get
//            {
//                return _ServiceRecID;
//            }
//            set
//            {
//                if (_ServiceRecID != value)
//                {
//                    _ServiceRecID = value;
//                    NotifyOfPropertyChange(() => ServiceRecID);
//                }
//            }
//        }


//        public void CreateNewAppointment()
//        {
//            CurrentAppointment = new PatientAppointment { V_ApptStatus = AllLookupValues.ApptStatus.PENDING };

//            CurrentAppointment.ApptStatus = new Lookup { LookupID = (long)AllLookupValues.ApptStatus.PENDING };

//            CurrentAppointment.HasChronicDisease = false;
//            CurrentAppointment.PtRegistrationID = RegistrationID;
//            CurrentAppointment.PatientApptServiceDetailList = new ObservableCollection<PatientApptServiceDetails>();
//            CurrentAppointment.PatientApptPCLRequestsList = new ObservableCollection<PatientApptPCLRequests>();

//            CurrentAppointment.ObjApptServiceDetailsList_Add = new ObservableCollection<PatientApptServiceDetails>();
//            CurrentAppointment.ObjApptServiceDetailsList_Update = new ObservableCollection<PatientApptServiceDetails>();
//            CurrentAppointment.ObjApptServiceDetailsList_Delete = new ObservableCollection<PatientApptServiceDetails>();

//            CurrentAppointment.ObjApptPCLRequestsList_Add = new ObservableCollection<PatientApptPCLRequests>();
//            CurrentAppointment.ObjApptPCLRequestsList_Update = new ObservableCollection<PatientApptPCLRequests>();
//            CurrentAppointment.ObjApptPCLRequestsList_Delete = new ObservableCollection<PatientApptPCLRequests>();

//            NotifyOfPropertyChange(() => DetailCuocHenIsEnabled);
//            NotifyOfPropertyChange(() => IsCanEdit);

//        }

//        public Nullable<DateTime> ApptDate_Root;

//        public DateTime DateServer;

//        private void InitDateServer(PatientAppointment appt)
//        {
//            // TxD 02/08/2014 Use Global Server Time instead
//            //Coroutine.BeginExecute(LoadDateServer(appt));

//            DateServer = Globals.GetCurServerDateTime().Date;

//            //Lay thoi gian mac dinh
//            Segments_WithAppDateDeptLocIDSeqNumber(Globals.DeptLocation.DeptLocationID, Globals.GetCurServerDateTime());
//            SetCurrentPatient(appt.Patient);
//            LoadAppointment(appt);

//        }

//        // TxD 02/08/2014 The following method is nolonger required see above code block
//        //private IEnumerator<IResult> LoadDateServer(PatientAppointment appt)
//        //{
//        //    var loadCurrentDate = new LoadCurrentDateTask();
//        //    yield return loadCurrentDate;
//        //    if (loadCurrentDate.CurrentDate == DateTime.MinValue)
//        //    {
//        //        DateServer = Globals.ServerDate.Value.Date;
//        //        MessageBoxTask _msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z1257_G1_KgLayDcNgThTuServer), eHCMSResources.G0442_G1_TBao);
//        //        yield return _msgTask;
//        //    }
//        //    else
//        //    {
//        //        DateServer = loadCurrentDate.CurrentDate.Date;
//        //    }


//        //    //Lay thoi gian mac dinh
//        //    Segments_WithAppDateDeptLocIDSeqNumber(Globals.DeptLocation.DeptLocationID, Globals.ServerDate.Value);
//        //    SetCurrentPatient(appt.Patient);
//        //    LoadAppointment(appt);

//        //    yield break;
//        //}


//        public bool DetailCuocHenIsEnabled
//        {
//            get
//            {
//                return ((CurrentAppointment != null
//                        && CurrentAppointment.ApptDate != null
//                        && !(CurrentAppointment.ApptDate.Value.Date.Subtract(DateServer.Date).Days < 1
//                            && CurrentAppointment.V_ApptStatus != AllLookupValues.ApptStatus.BOOKED))
//                            || !HasDone);

//            }
//        }

//        public void dtpApptDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
//        {
//            //NotifyOfPropertyChange(() => DetailCuocHenIsEnabled);
//            //NotifyOfPropertyChange(() => IsCanEdit);
//            //TinhNgayHenDefault();
//            ////tim lai cac ca kham o phong hen nay vao ngay hen nay
//            //if (DeptLocationsSelected != null && CurrentAppointment.ApptDate != null)
//            //{
//            //    Segments_WithAppDateDeptLocIDSeqNumber(DeptLocationsSelected.DeptLocationID, CurrentAppointment.ApptDate.Value);
//            //}
//        }

//        public void dtpApptDate_CalendarClosed(object sender, System.Windows.RoutedEventArgs e)
//        {
//            NotifyOfPropertyChange(() => DetailCuocHenIsEnabled);
//            NotifyOfPropertyChange(() => IsCanEdit);
//            TinhNgayHenDefault();
//            //tim lai cac ca kham o phong hen nay vao ngay hen nay
//            if (DeptLocationsSelected != null && CurrentAppointment.ApptDate != null)
//            {
//                Segments_WithAppDateDeptLocIDSeqNumber(DeptLocationsSelected.DeptLocationID, CurrentAppointment.ApptDate.Value);
//            }
//        }

//        protected void CheckAppointmentExists(long patientID, DateTime apptDate)
//        {
//            IsCheckingApptExist = true;

//            var t = new Thread(() =>
//            {
//                try
//                {
//                    using (var serviceFactory = new AppointmentServiceClient())
//                    {
//                        var contract = serviceFactory.ServiceInstance;

//                        contract.BeginGetAppointmentOfPatientByDate(patientID, apptDate,
//                            Globals.DispatchCallback(asyncResult =>
//                            {
//                                try
//                                {
//                                    var appt = contract.EndGetAppointmentOfPatientByDate(asyncResult);

//                                    if (appt != null) //Ton tai appointment roi
//                                    {
//                                        MessageBox.Show(eHCMSResources.A1036_G1_Msg_InfoDetailCuocHen + CurrentAppointment.ApptDate.Value.ToString("dd/MM/yyyy"));

//                                        CurrentAppointment = appt;

//                                        LoadAppointmentByID(appt.AppointmentID);
//                                    }
//                                    else
//                                    {
//                                        //Mở Detail của Bác Sĩ đã ghi nhận ra
//                                        //Có bên Bác Sĩ Hẹn
//                                        if (
//                                            (CurrentAppointment.PatientApptPCLRequestsList != null && CurrentAppointment.PatientApptPCLRequestsList.Count > 0)
//                                            ||
//                                            (CurrentAppointment.PatientApptServiceDetailList != null && CurrentAppointment.PatientApptServiceDetailList.Count > 0)
//                                            )
//                                        {
//                                            //Làm cái gì chưa biết
//                                            MessageBox.Show(eHCMSResources.A0293_G1_Msg_InfoCTietBSDaHen);
//                                        }
//                                        else
//                                        {
//                                            //Chua co thi khong lam gi het
//                                            CurrentAppointment.PatientApptServiceDetailList.Clear();
//                                            CurrentAppointment.PatientApptPCLRequestsList.Clear();
//                                        }

//                                    }

//                                    BookMarkForm(CurrentAppointment);

//                                }
//                                catch (FaultException<AxException> fault)
//                                {
//                                    ClientLoggerHelper.LogInfo(fault.ToString());
//                                }
//                                catch (Exception ex)
//                                {
//                                    ClientLoggerHelper.LogInfo(ex.ToString());
//                                }
//                                finally
//                                {
//                                    IsCheckingApptExist = false;
//                                }

//                            }), null);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//                }
//            });
//            t.Start();
//        }

//        protected void BookMarkForm(PatientAppointment appointment)
//        {
//            CurrentFormState = _currentAppointment.AppointmentID > 0 ? FormState.EDIT : FormState.NEW;
//        }

//        private string _textBacSiDeXuat;
//        public string TextBacSiDeXuat
//        {
//            get
//            {
//                return _textBacSiDeXuat;
//            }
//            set
//            {
//                if (_textBacSiDeXuat != value)
//                {
//                    _textBacSiDeXuat = value;
//                    NotifyOfPropertyChange(() => TextBacSiDeXuat);
//                }
//            }
//        }

//        public void LoadAppointmentByID(long appointmentByID)
//        {
//            var t = new Thread(() =>
//            {
//                AxErrorEventArgs error = null;
//                try
//                {
//                    using (var serviceFactory = new AppointmentServiceClient())
//                    {
//                        var contract = serviceFactory.ServiceInstance;

//                        contract.BeginGetAppointmentByID(appointmentByID,
//                            Globals.DispatchCallback(asyncResult =>
//                            {
//                                try
//                                {
//                                    var appt = contract.EndGetAppointmentByID(asyncResult);

//                                    CurrentAppointment = appt;
//                                    CurrentAppointmentCopy = appt.DeepCopy();

//                                    ApptDateOrig = CurrentAppointment.ApptDate;
//                                    curAppDate = CurrentAppointment.ApptDate == null && CurrentAppointment.NDay > 0 ?
//                                        CurrentAppointment.RecDateCreated.AddDays((int)CurrentAppointment.NDay) : CurrentAppointment.ApptDate.Value;

//                                    NotifyOfPropertyChange(() => IsCanEdit);
//                                    ApptDate_Root = CurrentAppointment.ApptDate;

//                                    HasDone = CurrentAppointment.IsCanEdit;

//                                    if (CurrentAppointment.NDay > 0)
//                                    {
//                                        TextBacSiDeXuat = string.Format(eHCMSResources.Z1020_G1_CuocHenBSiDeXuat, CurrentAppointment.NDay.ToString());

//                                        if (CurrentAppointment.ApptDate == null)
//                                        {
//                                            //CurrentAppointment.ApptDate = DateServer.AddDays(CurrentAppointment.NDay.Value);
//                                            CurrentAppointment.ApptDate = CurrentAppointment.RecDateCreated.AddDays(CurrentAppointment.NDay.Value);
//                                        }
//                                    }
//                                    else
//                                    {
//                                        TextBacSiDeXuat = string.Format("{0}:", eHCMSResources.K2749_G1_CHen);
//                                    }

//                                    if (CurrentAppointment.PatientApptPCLRequestsList != null && CurrentAppointment.PatientApptPCLRequestsList.Count > 0)
//                                    {
//                                        SelectedApptPclRequest = CurrentAppointment.PatientApptPCLRequestsList[0];
//                                    }

//                                    BookMarkForm(CurrentAppointment);
//                                    dtpApptDate_CalendarClosed(null, null);
//                                }
//                                catch (FaultException<AxException> fault)
//                                {
//                                    error = new AxErrorEventArgs(fault);
//                                }
//                                catch (Exception ex)
//                                {
//                                    error = new AxErrorEventArgs(ex);
//                                }
//                            }), null);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    error = new AxErrorEventArgs(ex);
//                }
//                if (error != null)
//                {
//                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
//                }
//            });
//            t.Start();
//        }

//        private void TinhNgayHenDefault()
//        {
//            if (CurrentAppointment.ApptDate == null)
//            {
//                if (CurrentAppointment.NDay != null)
//                {
//                    CurrentAppointment.ApptDate = DateServer.AddDays(CurrentAppointment.NDay.Value);
//                }
//            }

//            if (CurrentAppointment == null)
//                return;
//            if (CurrentAppointment.ApptDate == null)
//                return;

//            //Nếu là ngày CN thì tăng lên 1 bữa t2
//            if (!Globals.ServerConfigSection.ConsultationElements.AllowWorkingOnSunday && CurrentAppointment.ApptDate.Value.DayOfWeek == DayOfWeek.Sunday)
//            {
//                CurrentAppointment.ApptDate = CurrentAppointment.ApptDate.Value.AddDays(1);
//                MessageBox.Show(string.Format(eHCMSResources.A0841_G1_Msg_InfoDoiNgHenSangThuHai, CurrentAppointment.ApptDate.Value.ToString("dd/MM/yyyy")));
//                NotifyOfPropertyChange(() => DetailCuocHenIsEnabled);
//                NotifyOfPropertyChange(() => IsCanEdit);
//            }

//        }

//        public void btAddApptMedService_Cmd()
//        {

//            if (AppointmentTypeSelected == null || AppointmentTypeSelected.LookupID <= 0)
//            {
//                MessageBox.Show(eHCMSResources.A0329_G1_Msg_InfoChonLoaiHen);
//                return;
//            }

//            if (DeptLocationsSelected == null || DeptLocationsSelected.DeptLocationID <= 0)
//            {
//                MessageBox.Show(eHCMSResources.K2094_G1_ChonPg);
//                return;
//            }

//            if (AppointmentSegmentsSelected == null || AppointmentSegmentsSelected.ConsultationTimeSegmentID <= 0)
//            {
//                MessageBox.Show(eHCMSResources.A0364_G1_Msg_InfoChonTGian);
//                return;
//            }

//            var item = new PatientApptServiceDetails
//                {
//                    V_AppointmentType = AppointmentTypeSelected.LookupID,
//                    AppointmentType = AppointmentTypeSelected,
//                    DeptLocationID = DeptLocationsSelected.DeptLocationID,
//                    DeptLocation = DeptLocationsSelected,
//                    MedService = MedicalServicesSelected,
//                    MedServiceID = MedicalServicesSelected.MedServiceID,
//                    ApptTimeSegmentID = (short)AppointmentSegmentsSelected.ConsultationTimeSegmentID,
//                    ApptTimeSegment = AppointmentSegmentsSelected,
//                    EntityState = EntityState.DETACHED
//                };

//            if (CheckNotExistsService(MedicalServicesSelected))
//            {
//                CurrentAppointment.PatientApptServiceDetailList.Add(item);

//                if (CurrentAppointment.ObjApptServiceDetailsList_Add == null)
//                    CurrentAppointment.ObjApptServiceDetailsList_Add = new ObservableCollection<PatientApptServiceDetails>();

//                CurrentAppointment.ObjApptServiceDetailsList_Add.Add(item);
//            }
//        }

//        private bool CheckNotExistsService(RefMedicalServiceItem objMedServiceID)
//        {
//            if (CurrentAppointment.PatientApptServiceDetailList.Any(patientApptServiceDetailse => patientApptServiceDetailse.MedServiceID == objMedServiceID.MedServiceID))
//            {
//                MessageBox.Show(string.Format(eHCMSResources.Z0357_G1_DVDaChonRoi, objMedServiceID.MedServiceName.Trim()));
//                return false;
//            }
//            return true;
//        }

//        private bool _isAdding;
//        public bool IsAdding
//        {
//            get
//            {
//                return _isAdding;
//            }
//            set
//            {
//                if (_isAdding != value)
//                {
//                    _isAdding = value;
//                    NotifyOfPropertyChange(() => IsAdding);
//                    NotifyWhenBusy();
//                }
//            }
//        }

//        public void LoadAppointment(object appointment)
//        {
//            var p = appointment as PatientAppointment;
//            if (p == null)
//            {
//                //Thong bao loi.
//                return;
//            }
//            LoadAppointmentByID(p.AppointmentID);
//        }


//        public void Notify(string errMessage)
//        {
//            Globals.EventAggregator.Publish(new ErrorNotification { Message = errMessage });
//        }

//        public void CheckAvailableSpaceService(DateTime? apptDate, DeptLocation deptLoc, PatientApptTimeSegment timeSegment, PatientApptServiceDetails item)
//        {
//            if (apptDate.HasValue && deptLoc != null && timeSegment != null)
//            {
//                WaitingCheckSpace = true;

//                var t = new Thread(() =>
//                {
//                    try
//                    {
//                        using (var serviceFactory = new AppointmentServiceClient())
//                        {
//                            var contract = serviceFactory.ServiceInstance;

//                            contract.BeginGetNumberOfAvailablePosition(apptDate.Value, deptLoc.DeptLocationID, timeSegment.ApptTimeSegmentID, Globals.DispatchCallback(asyncResult =>
//                                {
//                                    try
//                                    {
//                                        int maxNumOfAppts;
//                                        int numOfAppts;
//                                        contract.EndGetNumberOfAvailablePosition(out maxNumOfAppts, out numOfAppts, asyncResult);

//                                        if (maxNumOfAppts == numOfAppts)
//                                        {
//                                            MessageBox.Show(eHCMSResources.A0957_G1_Msg_Pg + string.Format(eHCMSResources.Z0358_G1_TGianKgConChoTrongDeHen, deptLoc.Location.LocationName.Trim(), timeSegment.SegmentName.Trim()));
//                                        }
//                                        else if (numOfAppts < maxNumOfAppts)
//                                        {
//                                            CurrentAppointment.PatientApptServiceDetailList.Add(item);

//                                            CurrentAppointment.ObjApptServiceDetailsList_Add.Add(item);
//                                        }

//                                    }
//                                    catch (FaultException<AxException> fault)
//                                    {
//                                        ClientLoggerHelper.LogInfo(fault.ToString());
//                                    }
//                                    catch (Exception ex)
//                                    {
//                                        ClientLoggerHelper.LogInfo(ex.ToString());
//                                    }
//                                    finally
//                                    {
//                                        WaitingCheckSpace = false;
//                                    }

//                                }), null);
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        Globals.ShowMessage(ex.Message, eHCMSResources.T0074_G1_I);
//                    }

//                });
//                t.Start();
//            }
//        }

//        public void SetCurrentAppointment(PatientAppointment appt)
//        {
//            //SetCurrentPatient(appt.Patient);
//            //LoadAppointment(appt);
//            //KMx: Dời set HasDone vào trong LoadAppointmentByID() để khỏi cần dùng appt.IsCanEdit được set từ bên ngoài (ra toa gọi hẹn bệnh không có IsCanEdit) (22/03/2016 11:26)
//            //HasDone = appt.IsCanEdit;
//            //NotifyOfPropertyChange(() => DetailCuocHenIsEnabled);
//            InitDateServer(appt);
//        }

//        private ObservableCollection<Lookup> _appointmentStatusList;
//        public ObservableCollection<Lookup> AppointmentStatusList
//        {
//            get { return _appointmentStatusList; }
//            set
//            {
//                _appointmentStatusList = value;
//                NotifyOfPropertyChange(() => AppointmentStatusList);
//            }
//        }

//        #region "Hẹn CLS"
//        private ConsultationTimeSegments _appointmentSegmentsSelectedPCL;
//        public ConsultationTimeSegments AppointmentSegmentsSelectedPCL
//        {
//            get
//            {
//                return _appointmentSegmentsSelectedPCL;
//            }
//            set
//            {
//                if (_appointmentSegmentsSelectedPCL != value)
//                {
//                    _appointmentSegmentsSelectedPCL = value;
//                    NotifyOfPropertyChange(() => AppointmentSegmentsSelectedPCL);
//                }
//            }
//        }

//        private PCLFormsSearchCriteria _searchCriteria;
//        public PCLFormsSearchCriteria SearchCriteria
//        {
//            get
//            {
//                return _searchCriteria;
//            }
//            set
//            {
//                _searchCriteria = value;
//                NotifyOfPropertyChange(() => SearchCriteria);

//            }
//        }

//        private PCLExamTypeLocation _selectedPclExamTypeLocation;
//        public PCLExamTypeLocation SelectedPclExamTypeLocation
//        {
//            get { return _selectedPclExamTypeLocation; }
//            set
//            {
//                _selectedPclExamTypeLocation = value;
//                NotifyOfPropertyChange(() => SelectedPclExamTypeLocation);
//            }
//        }

//        IAddEditApptPCLRequest _addEditAppPclVm;
//        public bool CanAddApptPclCmd
//        {
//            get
//            {
//                return true;
//            }
//        }
//        IPatientPCLRequestEdit _patientPCLApptRequest;
//        public void AddApptPclCmd()
//        {
//            butCLS = false;
//            _patientPCLApptRequest = Globals.GetViewModel<IPatientPCLRequestEdit>();
//            _patientPCLApptRequest.IsAppointment = true;
//            _patientPCLApptRequest.IsEdit = false;

//            // TxD26/03/2015: Why a DeptLocation is required HERE ????
//            //                Comment it out for noew if required then REVIEW
//            //if (Globals.DeptLocation == null || Globals.DeptLocation.DeptLocationID <= 0)
//            //{
//            //    MessageBox.Show(string.Format("{0}!", eHCMSResources.K2102_G1_ChonPgLamViec));
//            //    return;
//            //}

//            var newApptPCLRequest = new PatientApptPCLRequests
//            {
//                PatientAppointment = CurrentAppointment
//                ,
//                PCLRequestNumID = Guid.NewGuid().ToString()
//                ,
//                ReqFromDeptLocID = Globals.DeptLocation.DeptLocationID
//                ,
//                V_PCLMainCategory = (long)AllLookupValues.V_PCLMainCategory.Laboratory
//            };
//            _patientPCLApptRequest.InitPatientInfo(CurrentPatient);
//            _patientPCLApptRequest.StartEditing(CurrentPatient.PatientID, newApptPCLRequest);
//            //21072018 TTM
//            //Comment cái action và globalsNav viết thành lại ở line 1721
//            //Action<IPatientPCLRequestEdit> onInitDlg = delegate (IPatientPCLRequestEdit patientPCLApptRequest)
//            //{
//            //    patientPCLApptRequest = _patientPCLApptRequest;
//            //};
//            //GlobalsNAV.ShowDialog<IPatientPCLRequestEdit>(onInitDlg);
//            GlobalsNAV.ShowDialog_V3<IPatientPCLRequestEdit>(_patientPCLApptRequest);
//            //butCLS = true;
//        }

//        IPatientPCLRequestEditImage _patientPCLApptRequestImage;
//        public void AddApptPclImageCmd()
//        {
//            butCLS = false;
//            _patientPCLApptRequestImage = Globals.GetViewModel<IPatientPCLRequestEditImage>();
//            _patientPCLApptRequestImage.IsAppointment = true;
//            _patientPCLApptRequestImage.IsEdit = false;

//            // TxD26/03/2015: Why a DeptLocation is required HERE ????
//            //                Comment it out for noew if required then REVIEW
//            //if (Globals.DeptLocation == null || Globals.DeptLocation.DeptLocationID <= 0)
//            //{
//            //    MessageBox.Show(string.Format("{0}!", eHCMSResources.K2102_G1_ChonPgLamViec));
//            //    return;
//            //}

//            var newApptPCLRequest = new PatientApptPCLRequests
//            {
//                PatientAppointment = CurrentAppointment
//                ,
//                PCLRequestNumID = Guid.NewGuid().ToString()
//                ,
//                ReqFromDeptLocID = Globals.DeptLocation.DeptLocationID
//                ,
//                V_PCLMainCategory = (long)AllLookupValues.V_PCLMainCategory.Imaging
//            };
//            _patientPCLApptRequestImage.InitPatientInfo(CurrentPatient);
//            _patientPCLApptRequestImage.StartEditing(CurrentPatient.PatientID, newApptPCLRequest);
//            Action<IPatientPCLRequestEditImage> onInitDlg = delegate (IPatientPCLRequestEditImage patientPCLApptRequestImage)
//            {
//                patientPCLApptRequestImage = _patientPCLApptRequestImage;
//            };
//            GlobalsNAV.ShowDialog<IPatientPCLRequestEditImage>(onInitDlg);
//        }
//        public void AddApptPclCmdOld()
//        {
//            _addEditAppPclVm = Globals.GetViewModel<IAddEditApptPCLRequest>();
//            _addEditAppPclVm.Registration_DataStorage = Registration_DataStorage;
//            // TxD26/03/2015: Why a DeptLocation is required HERE ????
//            //                Comment it out for noew if required then REVIEW
//            //if (Globals.DeptLocation == null || Globals.DeptLocation.DeptLocationID <= 0)
//            //{
//            //    MessageBox.Show(string.Format("{0}!", eHCMSResources.K2102_G1_ChonPgLamViec));
//            //    return;
//            //}

//            var newApptPCLRequest = new PatientApptPCLRequests { PatientAppointment = CurrentAppointment, PCLRequestNumID = Guid.NewGuid().ToString(), ReqFromDeptLocID = Globals.DeptLocation.DeptLocationID };
//            _addEditAppPclVm.StartEditing(CurrentPatient.PatientID, newApptPCLRequest);
//            Action<IAddEditApptPCLRequest> onInitDlg = delegate (IAddEditApptPCLRequest addEditAppPclVm)
//            {
//                addEditAppPclVm.Registration_DataStorage = Registration_DataStorage;
//                addEditAppPclVm = _addEditAppPclVm;
//            };
//            GlobalsNAV.ShowDialog<IAddEditApptPCLRequest>(onInitDlg);
//        }

//        public bool CanSaveAppointmentCmd
//        {
//            get
//            {
//                return !IsSaving;
//            }
//        }

//        public bool CheckAppointment(PatientAppointment currentAppointment, ref string errStr)
//        {
//            errStr = "";
//            if (currentAppointment == null)
//            {
//                errStr = eHCMSResources.Z1021_G1_ChuaCoCuocHen;
//                return false;
//            }

//            if (CurrentAppointment.PatientApptServiceDetailList.Count == 0
//                && CurrentAppointment.PatientApptPCLRequestsList.Count == 0)
//            {
//                errStr = eHCMSResources.Z0353_G1_KgTheLuuCuocHen;
//                return false;
//            }

//            if (CurrentAppointment.AllowPaperReferralUseNextConsult && (!CurrentAppointment.HasChronicDisease.GetValueOrDefault() && string.IsNullOrWhiteSpace(CurrentAppointment.ReasonToAllowPaperReferral)))
//            {
//                errStr = eHCMSResources.Z0973_G1_ChonLyDoSDGCV;
//                return false;
//            }

//            return true;
//        }

//        public void SaveAppointmentCmd()
//        {
//            if (CurrentPatient == null || CurrentPatient.PatientID <= 0)
//            {
//                MessageBox.Show(eHCMSResources.A0378_G1_Msg_InfoChuaChonBN);
//                return;
//            }

//            if (!CurrentAppointment.ApptDate.HasValue || CurrentAppointment.ApptDate == null)
//            {
//                MessageBox.Show(eHCMSResources.A0393_G1_Msg_InfoChuaChonNgHen);
//                return;
//            }

//            if (DateServer == DateTime.MinValue)
//            {
//                DateServer = Globals.ServerDate.GetValueOrDefault();
//            }

//            if (CurrentAppointment.ApptDate.Value.Date.Subtract(DateServer.Date).Days < 0)
//            {
//                MessageBox.Show(string.Format(eHCMSResources.Z0373_G1_NgHenKgHopLe, DateServer.ToString("dd/MM/yyyy")));
//                return;
//            }

//            string errStr = "";
//            if (!CheckAppointment(CurrentAppointment, ref errStr))
//            {
//                MessageBox.Show(errStr);
//                return;
//            }

//            if (CheckPhongThoiGianServiceList() == false)
//                return;



//            CurrentAppointment.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
//            CurrentAppointment.PatientID = CurrentPatient.PatientID;

//            CurrentAppointment.V_ApptStatus = AllLookupValues.ApptStatus.BOOKED;

//            BeginProcess(true);
//        }

//        public void CheckAndPrintHIAppointment(bool isPrintNewTemplate)
//        {
//            if (CurrentAppointment.PtRegistrationID < 1)
//            {
//                return;
//            }
//            if (CurrentAppointment == null || CurrentAppointment.PatientApptServiceDetailList == null || CurrentAppointment.PatientApptServiceDetailList.Count <= 0)
//            {
//                MessageBox.Show(eHCMSResources.A0720_G1_Msg_InfoKhTheXemInDVKBRong);
//                return;
//            }

//            if (CurrentAppointmentCopy == null)
//            {
//                MessageBox.Show(eHCMSResources.Z0374_G1_KgTheXemInCuocHenChuaLuu, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                return;
//            }

//            if (CurrentAppointmentCopy.AllowPaperReferralUseNextConsult != CurrentAppointment.AllowPaperReferralUseNextConsult
//                || CurrentAppointmentCopy.HasChronicDisease != CurrentAppointment.HasChronicDisease
//                || CurrentAppointmentCopy.ReasonToAllowPaperReferral != CurrentAppointment.ReasonToAllowPaperReferral)
//            {
//                MessageBox.Show(eHCMSResources.Z0375_G1_KgTheXemInCuocHenCoThayDoi, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                return;
//            }

//            if (!CurrentAppointmentCopy.AllowPaperReferralUseNextConsult)
//            {
//                MessageBox.Show(eHCMSResources.Z0376_G1_ChuaLuuChoPhepSDGCV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                return;
//            }

//            if (!CurrentAppointmentCopy.HasChronicDisease.GetValueOrDefault() && string.IsNullOrWhiteSpace(CurrentAppointmentCopy.ReasonToAllowPaperReferral))
//            {
//                MessageBox.Show(eHCMSResources.Z0377_G1_ChuaLuuLiDoSDGCV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                return;
//            }

//            bool canPrint = false;
//            bool HasReminder = false;

//            foreach (var serItem in CurrentAppointment.PatientApptServiceDetailList)
//            {
//                if (serItem.ApptSvcDetailID > 0)
//                {
//                    canPrint = true;
//                }
//                if (serItem.ApptSvcDetailID <= 0)
//                {
//                    HasReminder = true;
//                }
//            }

//            if (canPrint == false)
//            {
//                MessageBox.Show(eHCMSResources.A0719_G1_Msg_InfoKhTheXemInDVKBChuaLuu);
//            }
//            else
//            {

//                if (HasReminder && MessageBox.Show(eHCMSResources.Z0378_G1_DSDVKBCoThayDoiChuaLuu + Environment.NewLine + eHCMSResources.Z0379_G1_ChiHienThiDVDaLuu + Environment.NewLine + eHCMSResources.Z0380_G1_CoMuonXemInKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
//                {
//                    return;
//                }
//                if (isPrintNewTemplate)
//                {
//                    StartViewPrintHI_New();
//                }
//                else
//                {
//                    StartViewPrintHI();
//                }  
//            }
//        }


//        public void HIAppt_NewCmd()
//        {
//            CheckAndPrintHIAppointment(true);
//        }

//        //public void HIApptCmd_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
//        public void HIApptCmd()
//        {
//            CheckAndPrintHIAppointment(false);
//        }

//        private void BeginProcess(bool PassCheckFullTarget)
//        {
//            if (CheckCoHenVaDoiNgayHen() == false)
//            {
//                //string sa = "xl cud hẹn";
//                OrderDataBeforeSave(PassCheckFullTarget);
//            }
//            else
//            {
//                //string sa = "xl doi ngay";
//                IsLoadingPclExamTypes = true;
//                Coroutine.BeginExecute(PatientAppointments_SaveCorountine(PassCheckFullTarget), null, (o, e) =>
//                {
//                    IsLoadingPclExamTypes = false;
//                });
//            }
//        }

//        private bool CheckCoHenVaDoiNgayHen()
//        {
//            bool Res = false;

//            if (ApptDate_Root == null)
//                return false;

//            if (
//                (CurrentAppointment.PatientApptServiceDetailList != null && CurrentAppointment.PatientApptServiceDetailList.Count > 0)
//                ||
//                (CurrentAppointment.PatientApptPCLRequestsList != null && CurrentAppointment.PatientApptPCLRequestsList.Count > 0)
//                )
//            {
//                if (CurrentAppointment.ApptDate.Value.Date.Subtract(ApptDate_Root.Value.Date).Days != 0)
//                {
//                    Res = true;
//                }
//            }

//            return Res;
//        }


//        private bool CheckPhongThoiGianServiceList()
//        {
//            for (int i = 0; i < CurrentAppointment.PatientApptServiceDetailList.Count; i++)
//            {
//                PatientApptServiceDetails row = CurrentAppointment.PatientApptServiceDetailList[i];

//                if (row.DeptLocation == null || row.DeptLocation.DeptLocationID <= 0)
//                {
//                    MessageBox.Show(string.Format("{0} ", eHCMSResources.A0522_G1_Msg_Dong) + (i + 1).ToString() + eHCMSResources.A0523_G1_Msg_InfoChuaChonPg2);
//                    return false;
//                }

//                if (row.ApptTimeSegment == null || row.ApptTimeSegment.ConsultationTimeSegmentID <= 0)
//                {
//                    MessageBox.Show(string.Format("{0} ", eHCMSResources.A0522_G1_Msg_Dong) + (i + 1).ToString() + eHCMSResources.A0523_G1_Msg_InfoChuaChonPg2);
//                    return false;
//                }
//            }
//            return true;
//        }

//        private void CopyPatientApptPCLRequests(ref PatientApptPCLRequests target, PatientApptPCLRequests source)
//        {
//            target.PatientPCLReqID = source.PatientPCLReqID;
//            target.AppointmentID = source.AppointmentID;
//            target.PCLRequestNumID = source.PCLRequestNumID;
//            target.StaffID = source.StaffID;
//            target.ReqFromDeptLocID = source.ReqFromDeptLocID;
//            target.Diagnosis = source.Diagnosis;
//            target.ICD10List = source.ICD10List;
//            target.ApptPCLNote = source.ApptPCLNote;
//        }

//        private void OrderDataBeforeSave(bool PassCheckFullTarget)
//        {
//            IEnumerable<PatientApptServiceDetails> xmlPatientApptServiceDetails_Add;
//            IEnumerable<PatientApptServiceDetails> xmlPatientApptServiceDetails_Update;


//            xmlPatientApptServiceDetails_Add = (from c in CurrentAppointment.PatientApptServiceDetailList
//                                                where c.EntityState == EntityState.DETACHED
//                                                || c.EntityState == EntityState.NEW
//                                                select c);

//            xmlPatientApptServiceDetails_Update = (from c in CurrentAppointment.PatientApptServiceDetailList
//                                                   where c.EntityState == EntityState.MODIFIED
//                                                   select c);

//            var xmlPatientApptPCLRequests_Add = new ObservableCollection<PatientApptPCLRequests>();
//            var xmlPatientApptPCLRequests_Update = new ObservableCollection<PatientApptPCLRequests>();
//            var xmlPatientApptPCLRequests_Delete = new ObservableCollection<PatientApptPCLRequests>();

//            foreach (var check in CurrentAppointment.PatientApptPCLRequestsList)
//            {
//                PatientApptPCLRequests_UpdateTemplate(check);
//                var pclAdd = new PatientApptPCLRequests();
//                var pclUpdate = new PatientApptPCLRequests();
//                var pclDelete = new PatientApptPCLRequests();

//                var ObjrqI = check.DeepCopy();

//                var listPCLUpdate = (from c in check.ObjPatientApptPCLRequestDetailsList
//                                     where c.EntityState == EntityState.MODIFIED
//                                     select c);

//                var listPCLDelete = (from c in check.ObjPatientApptPCLRequestDetailsList
//                                     where c.EntityState == EntityState.DELETED_MODIFIED
//                                     select c);

//                pclUpdate.ObjPatientApptPCLRequestDetailsList = new ObservableCollection<PatientApptPCLRequestDetails>(listPCLUpdate);
//                CopyPatientApptPCLRequests(ref pclUpdate, ObjrqI);


//                pclDelete.ObjPatientApptPCLRequestDetailsList = new ObservableCollection<PatientApptPCLRequestDetails>(listPCLDelete);
//                CopyPatientApptPCLRequests(ref pclDelete, ObjrqI);

//                if (pclUpdate.ObjPatientApptPCLRequestDetailsList.Count > 0)
//                {
//                    xmlPatientApptPCLRequests_Update.Add(pclUpdate);
//                }
//                if (pclDelete.ObjPatientApptPCLRequestDetailsList.Count > 0)
//                {
//                    xmlPatientApptPCLRequests_Delete.Add(pclDelete);
//                }
//            }

//            if (xmlPatientApptServiceDetails_Add != null)
//            {
//                CurrentAppointment.ObjApptServiceDetailsList_Add = xmlPatientApptServiceDetails_Add.ToObservableCollection();
//            }
//            else
//            {
//                CurrentAppointment.ObjApptServiceDetailsList_Add = null;
//            }

//            if (xmlPatientApptServiceDetails_Update != null)
//            {
//                CurrentAppointment.ObjApptServiceDetailsList_Update = xmlPatientApptServiceDetails_Update.ToObservableCollection();
//            }
//            else
//            {
//                CurrentAppointment.ObjApptServiceDetailsList_Update = null;
//            }

//            if (PatientApptServiceDetails_ListDelete != null && PatientApptServiceDetails_ListDelete.Count > 0)
//            {
//                CurrentAppointment.ObjApptServiceDetailsList_Delete = new ObservableCollection<PatientApptServiceDetails>(PatientApptServiceDetails_ListDelete);
//            }
//            else
//            {
//                CurrentAppointment.ObjApptServiceDetailsList_Delete = null;
//            }


//            if (xmlPatientApptPCLRequests_Update != null && xmlPatientApptPCLRequests_Update.Count > 0)
//            {
//                CurrentAppointment.ObjApptPCLRequestsList_Update = xmlPatientApptPCLRequests_Update.ToObservableCollection();
//            }
//            else
//            {
//                CurrentAppointment.ObjApptPCLRequestsList_Update = null;
//            }

//            if (PatientApptPCLRequests_ListDelete != null && PatientApptPCLRequests_ListDelete.Count > 0)
//            {
//                CurrentAppointment.ObjApptPCLRequestsList_Delete = PatientApptPCLRequests_ListDelete.ToObservableCollection();
//                if (xmlPatientApptPCLRequests_Delete != null && xmlPatientApptPCLRequests_Delete.Count > 0)
//                {
//                    foreach (var item in xmlPatientApptPCLRequests_Delete)
//                    {
//                        CurrentAppointment.ObjApptPCLRequestsList_Delete.Add(item);
//                    }
//                }

//                //KMx: Ở trên đã gán rồi, xuống đây gán chi nữa??? (20/03/2014 13:56)
//                //Bỏ code bên dưới vì dư thừa.
//                //if (PatientApptPCLRequests_ListDelete != null && PatientApptPCLRequests_ListDelete.Count > 0)
//                //{
//                //    foreach (var item in PatientApptPCLRequests_ListDelete)
//                //    {
//                //        CurrentAppointment.ObjApptPCLRequestsList_Delete.Add(item);
//                //    }
//                //}
//            }
//            else
//            {
//                CurrentAppointment.ObjApptPCLRequestsList_Delete = null;
//            }

//            IsLoadingPclExamTypes = true;
//            Coroutine.BeginExecute(PatientAppointments_SaveCorountine(PassCheckFullTarget), null, (o, e) =>
//            {
//                IsLoadingPclExamTypes = false;
//            });
//        }
//        WarningWithConfirmMsgBoxTask warnConfDlg = null;

//        private IEnumerator<IResult> PatientAppointments_SaveCorountine(bool PassCheckFullTarget)
//        {
//            //EntityState.NEW: Trạng thái TRƯỚC khi lưu.
//            //EntityState.PERSITED: Trạng thái SAU khi lưu.
//            List<PatientApptPCLRequestDetails> listPclApptReqDetails = new List<PatientApptPCLRequestDetails>();
//            //Lấy DS PCL chưa lưu để kiểm tra.
//            var listitems = CurrentAppointment.PatientApptPCLRequestsList.SelectMany(x => x.ObjPatientApptPCLRequestDetailsList).Where(x => x.EntityState == EntityState.NEW).ToList();
//            listPclApptReqDetails = listitems;
//            TimeSpan temp = ApptDateOrig.GetValueOrDefault() - CurrentAppointment.ApptDate.GetValueOrDefault();
//            //Nếu đổi ngày hẹn thì lấy những PCL đã lưu rồi kiểm tra luôn.
//            if (temp.TotalDays != 0)
//            {
//                var listitems2 = CurrentAppointment.PatientApptPCLRequestsList.SelectMany(x => x.ObjPatientApptPCLRequestDetailsList).Where(x => x.EntityState == EntityState.PERSITED).ToList();
//                listPclApptReqDetails.AddRange(listitems2);
//            }
//            if (listPclApptReqDetails != null && listPclApptReqDetails.Count() > 0)
//            {
//                foreach (var item in listPclApptReqDetails)
//                {
//                    //se kiem tra o day.......!!can lam sang so
//                    var TargetTask = new PCLExamTypeServiceTarget_CheckedAppointmentTask(item.ObjPCLExamTypes.PCLExamTypeID, CurrentAppointment.ApptDate.GetValueOrDefault());
//                    yield return TargetTask;

//                    if (TargetTask.Error != null)
//                    {
//                        Globals.EventAggregator.Publish(new ErrorNotification { Message = string.Format("{0}!", eHCMSResources.Z1041_G1_LoiXayRaCLSSo) });
//                        yield break;
//                    }
//                    else
//                    {
//                        if (!TargetTask.Result)
//                        {
//                            Globals.EventAggregator.Publish(new ErrorNotification { Message = string.Format(eHCMSResources.Z1496_G1_0DaVuotChiTieu, item.ObjPCLExamTypes.PCLExamTypeName) });
//                            yield break;
//                        }
//                    }
//                }
//            }
//            //KMx: Phần kiểm tra bên dưới được copy và chỉnh sửa từ "ra toa" (15/01/016 14:59)
//            if (IsCreateApptFromConsultation && CurrentAppointment.AppointmentID <= 0)
//            {
//                if (Registration_DataStorage.CurrentPatientRegistration == null || Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID <= 0)
//                {
//                    MessageBox.Show(eHCMSResources.A0634_G1_Msg_InfoKhCoDKDeHen, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                    yield break;
//                }

//                if (Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
//                {
//                    if (Registration_DataStorage.PatientServiceRecordCollection == null || Registration_DataStorage.PatientServiceRecordCollection.Count < 1)
//                    {
//                        MessageBox.Show(eHCMSResources.A0405_G1_Msg_InfoChuaCoCD, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                        yield break;
//                    }
//                    CurrentAppointment.ServiceRecID = Registration_DataStorage.PatientServiceRecordCollection[0].DiagnosisTreatments[0].ServiceRecID;
//                    CurrentAppointment.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
//                }
//                else
//                {
//                    if (Registration_DataStorage.PatientServiceRecordCollection == null || Registration_DataStorage.PatientServiceRecordCollection.Count <= 0 || Registration_DataStorage.PatientServiceRecordCollection[0].DiagnosisTreatments.Count <= 0
//                        || !Registration_DataStorage.PatientServiceRecordCollection[0].DiagnosisTreatments.Any(x => x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS)) 
//                    {
//                        MessageBox.Show(eHCMSResources.Z0381_G1_ChuaCoCDoanXV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                        yield break;
//                    }
//                    CurrentAppointment.ServiceRecID = Registration_DataStorage.PatientServiceRecordCollection[0].DiagnosisTreatments.Where(x => x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS).FirstOrDefault().ServiceRecID;
//                    CurrentAppointment.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
//                    CurrentAppointment.CreatedByInPtRegis = true;
//                }
//            }
//            // HPT_20160709: Phải đưa khoa cấp cứu vào Lookup, không thể lấy số mặc định 113 như vầy
//            // Tạm thời kiểm tra như vầy. Trong trường hợp ViewModel này được gọi từ AppointmentListingViewModel, tức là từ module Hẹn bệnh --> Quản lý hẹn bệnh thì Registration_DataStorage.CurrentPatientRegistration = null
//            // Trong Quản lý hẹn bệnh chưa có sẵn các thông tin về đăng ký hiện tại của bệnh nhân. Nếu muốn nhắc nhở phải thêm code lấy lên --> mất thời gian trong khi đó, anh Kiên có từng nói người ta ít dùng module hẹn bệnh nên sẽ điều chỉnh sau

//            /*HPT 2707/2016: the following code mean:  
//             * Nếu cuộc hẹn được xác nhận là hẹn cấp cứu tái khám mà bệnh nhân không thỏa mãn một trong các điều kiện dưới đây (có nghi vấn - A.Tuấn) thì cần xác nhận lại:
//             *  1. Cuộc hẹn không được tạo từ đăng ký
//             *  2.1 Hoặc Không xác định được thông tin đăng ký đang dùng để thực tạo cuộc hẹn
//             *  2.2 Hoặc Có thông tin đăng ký nhưng
//             *      2.2.a.Đăng ký đang dùng để tạo cuộc hẹn được nhập viện vào khoa cấp cứu
//             *      2.2.b.Đăng ký đang dùng để tạo cuộc hẹn không được xác nhận cấp cứu
//             * Anh Tuấn nói tạm thời không cần nhắc người ta khi người ta không tick vô, khi nào VT yêu cầu thì mới làm 
//             */
            
//            if (CurrentAppointment.IsEmergInPtReExamApp)
//            {
//                if (Registration_DataStorage.CurrentPatientRegistration == null || Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
//                {
//                    CurrentAppointment.IsEmergInPtReExamApp = false;
//                }
//                else
//                {
//                    if (Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo == null || Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.DeptID != Globals.ServerConfigSection.InRegisElements.EmerDeptID
//                        && (!Registration_DataStorage.CurrentPatientRegistration.EmergRecID.HasValue || Registration_DataStorage.CurrentPatientRegistration.EmergRecID.GetValueOrDefault() == 0))
//                    {
//                        warnConfDlg = new WarningWithConfirmMsgBoxTask(eHCMSResources.Z1264_G1_I, string.Format("{0}!", eHCMSResources.Z1265_G1_TiepTucLuuCHTK));
//                        yield return warnConfDlg;
//                        if (!warnConfDlg.IsAccept)
//                        {
//                            yield break;
//                        }
//                        warnConfDlg = null;
//                    }
//                }
//            }
//            PatientAppointments_Save(PassCheckFullTarget);
//        }

//        public bool CanConfirmEmerInPtReExamApp
//        {
//            get
//            {
//                return (CurrentAppointment != null && CurrentAppointment.isHIAppt && Registration_DataStorage.CurrentPatientRegistration != null
//                        && Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU);
//            }
//        }
//        private void PatientAppointments_Save(bool PassCheckFullTarget)
//        {
//            // TxD 03/02/2015: Just double check here and set the flag accordingly just in case there is a bug somewhere that did not set this flag 
//            //if (Registration_DataStorage.CurrentPatientRegistration != null && Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID > 0 && 
//            //    Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
//            //{
//            //    CurrentAppointment.CreatedByInPtRegis = true;
//            //}

//            //HPT_20160711: đã dời code kiểm tra lên hàm PatientAppointments_SaveCorountine. Vì phải thực hiện thêm kiểm tra và cảnh báo xác nhận cho bệnh nhân cấp cứu tái khám nên phải thực hiện trong Coroutine
           
//            IsSaving = true;

//            var t = new Thread(() =>
//            {
//                using (var serviceFactory = new AppointmentServiceClient())
//                {
//                    var contract = serviceFactory.ServiceInstance;
//                    contract.BeginPatientAppointments_Save(CurrentAppointment, PassCheckFullTarget,
//                        Globals.DispatchCallback((asyncResult) =>
//                        {
//                            try
//                            {
//                                long AppointmentID = 0;
//                                string ErrorDetail = "";

//                                string ListNotConfig = "";
//                                string ListTargetFull = "";
//                                string ListMax = "";
//                                string ListRequestID = "";

//                                var b = contract.EndPatientAppointments_Save(out AppointmentID, out ErrorDetail, out ListNotConfig, out ListTargetFull, out ListMax, out ListRequestID, asyncResult);
//                                if (b && AppointmentID > 0)
//                                {
//                                    MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
//                                    ApptDateOrig = CurrentAppointment.ApptDate;
//                                    //hasSaved = true && IsCanEdit;

//                                    //KMx: Sau khi lưu thành công thì phải gán 2 list Delete = null, tránh trường hợp user bấm lưu hẹn nhiều lần, dẫn đến delete nhiều lần => lấy lại STT nhiều lần => trùng STT hẹn (20/03/2014 12:07).
//                                    PatientApptServiceDetails_ListDelete = null;
//                                    PatientApptPCLRequests_ListDelete = null;

//                                    //KMx: Sau khi lưu thành công sẽ tự động hiện Report lên. Nhưng vì Report đã chuyển sang in tổng hợp, nên Report in từng phiếu này không giống.
//                                    //     Nếu muốn tự động hiện Report thì phải chuyển sang đường mới. Khi nào có thời gian thì chuyển Report này sang đường mới.
//                                    //if (!string.IsNullOrEmpty(ListRequestID))
//                                    //{
//                                    //    var proAlloc = Globals.GetViewModel<ICommonPreviewView>();
//                                    //    proAlloc.Result = ListRequestID;
//                                    //    proAlloc.eItem = ReportName.RptPatientApptPCLRequests_XML;

//                                    //    var instance = proAlloc as Conductor<object>;
//                                    //    Globals.ShowDialog(instance, (o) => { });

//                                    //}

//                                    Globals.EventAggregator.Publish(new AddCompleted<PatientAppointment>());

//                                    LoadAppointmentByID(AppointmentID);

//                                    //đọc lại để ComboBox Thời Gian biết tới số nhiêu rồi
//                                    if (DeptLocationsSelected != null && DeptLocationsSelected.DeptLocationID > 0)
//                                    {
//                                        if (CurrentAppointment.ApptDate != null)
//                                        {
//                                            Segments_WithAppDateDeptLocIDSeqNumber(DeptLocationsSelected.DeptLocationID, CurrentAppointment.ApptDate.Value);
//                                        }
//                                    }
//                                    //đọc lại để ComboBox Thời Gian biết tới số nhiêu rồi

//                                }
//                                else
//                                {
//                                    bool bNotConfig = false;
//                                    bool bTargetFull = false;
//                                    bool bMax = false;

//                                    if ((!string.IsNullOrEmpty(ListNotConfig))
//                                        ||
//                                        (!string.IsNullOrEmpty(ListTargetFull))
//                                        ||
//                                        (!string.IsNullOrEmpty(ListMax))
//                                        )
//                                    {
//                                        //Kiểm tra lại có dạng này thì xuất thông báo lỗi ra
//                                        /*
//                                        --<DS>
//                                        --<RecInfo>                                         
//                                        --<MedServiceID></MedServiceID>
//                                        --<DeptLocationID></DeptLocationID>
//                                        --<ApptTimeSegmentID></ApptTimeSegmentID>
//                                        --<Result></Result>  
//                                        --</RecInfo>                                         
//                                        --</DS>
//                                         */
//                                        string err = "";

//                                        if (!string.IsNullOrEmpty(ListNotConfig))
//                                        {
//                                            bNotConfig = true;
//                                            err = ListNotConfig;
//                                        }

//                                        if (!string.IsNullOrEmpty(ListMax))
//                                        {
//                                            bMax = true;
//                                            err = err + Environment.NewLine + ListMax;
//                                        }

//                                        if (!string.IsNullOrEmpty(ListTargetFull))
//                                        {
//                                            bTargetFull = true;
//                                            err = err + Environment.NewLine + ListTargetFull;
//                                        }

//                                        if ((bTargetFull) && (bNotConfig == false) && (bMax == false))
//                                        {
//                                            if (MessageBox.Show(err.ToString() + Environment.NewLine + eHCMSResources.T0056_G1_I, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
//                                            {
//                                                BeginProcess(false);
//                                            }
//                                        }
//                                        else
//                                        {
//                                            MessageBox.Show(err.ToString() + Environment.NewLine + eHCMSResources.T0057_G1_KhongTheLuu, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                                        }
//                                    }
//                                    else
//                                    {
//                                        MessageBox.Show(eHCMSResources.A0803_G1_Msg_InfoLuuCuocHenFail);
//                                    }
//                                }
//                            }
//                            catch (Exception ex)
//                            {
//                                MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                            }
//                            finally
//                            {
//                                IsSaving = false;
//                                //hasSaved = true && IsCanEdit;
//                            }
//                        }), null);
//                }


//            });
//            t.Start();
//        }

//        public void PatientApptPCLRequests_UpdateTemplate(PatientApptPCLRequests apptPCLRequest)
//        {
//            var t = new Thread(() =>
//            {
//                try
//                {
//                    using (var serviceFactory = new AppointmentServiceClient())
//                    {
//                        var contract = serviceFactory.ServiceInstance;
//                        contract.BeginPatientApptPCLRequests_UpdateTemplate(apptPCLRequest, Globals.DispatchCallback((asyncResult) =>
//                        {
//                            try
//                            {
//                                var res = contract.EndPatientApptPCLRequests_UpdateTemplate(asyncResult);

//                            }
//                            catch (FaultException<AxException> fault)
//                            {
//                                ClientLoggerHelper.LogInfo(fault.ToString());
//                            }
//                            catch (Exception ex)
//                            {
//                                ClientLoggerHelper.LogInfo(ex.ToString());
//                            }

//                        }), null);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//                }
//                finally
//                {

//                }
//            });
//            t.Start();
//        }

//        private PatientApptPCLRequests _selectedApptPCLRequest;
//        public PatientApptPCLRequests SelectedApptPclRequest
//        {
//            get
//            {
//                return _selectedApptPCLRequest;
//            }
//            set
//            {
//                if (_selectedApptPCLRequest != value)
//                {
//                    _selectedApptPCLRequest = value;
//                    NotifyOfPropertyChange(() => SelectedApptPclRequest);

//                    NotifyOfPropertyChange(() => CanbtUpdateApptPCL_Cmd);
//                    NotifyOfPropertyChange(() => FilteredCollection);
//                }
//            }
//        }
//        public ICollectionView FilteredCollection
//        {
//            get
//            {
//                if (SelectedApptPclRequest == null || SelectedApptPclRequest.ObjPatientApptPCLRequestDetailsList == null)
//                {
//                    return null;
//                }
//                var source = new CollectionViewSource
//                {
//                    Source = SelectedApptPclRequest.ObjPatientApptPCLRequestDetailsList
//                };
//                if (source.View != null)
//                {
//                    source.View.Filter = Filter;
//                }
//                return source.View;
//            }
//        }

//        public bool Filter(object obj)
//        {
//            var details = (PatientApptPCLRequestDetails)obj;
//            return details.EntityState != EntityState.DELETED_PERSITED && details.EntityState != EntityState.DELETED_MODIFIED;
//        }
//        private List<PatientApptServiceDetails> _patientApptServiceDetailsListDelete = new List<PatientApptServiceDetails>();
//        public List<PatientApptServiceDetails> PatientApptServiceDetails_ListDelete
//        {
//            get
//            {
//                return _patientApptServiceDetailsListDelete;
//            }
//            set
//            {
//                if (_patientApptServiceDetailsListDelete != value)
//                {
//                    _patientApptServiceDetailsListDelete = value;
//                    NotifyOfPropertyChange(() => PatientApptServiceDetails_ListDelete);
//                }
//            }
//        }

//        private List<PatientApptPCLRequests> _PatientApptPCLRequests_ListDelete = new List<PatientApptPCLRequests>();
//        public List<PatientApptPCLRequests> PatientApptPCLRequests_ListDelete
//        {
//            get
//            {
//                return _PatientApptPCLRequests_ListDelete;
//            }
//            set
//            {
//                if (_PatientApptPCLRequests_ListDelete != value)
//                {
//                    _PatientApptPCLRequests_ListDelete = value;
//                    NotifyOfPropertyChange(() => PatientApptPCLRequests_ListDelete);
//                }
//            }
//        }

//        public void hplPCLReqDelete_Click(object selectedItem)
//        {
//            if (MessageBox.Show(eHCMSResources.A0174_G1_Msg_ConfXoaYCCLS, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
//            {
//                var p = (selectedItem as PatientApptPCLRequests);

//                CurrentAppointment.PatientApptPCLRequestsList.Remove(p);

//                if (CurrentFormState == FormState.EDIT)
//                {
//                    //Bỏ vào ds Delete
//                    if (PatientApptPCLRequests_ListDelete == null)
//                    {
//                        PatientApptPCLRequests_ListDelete = new List<PatientApptPCLRequests>();
//                    }

//                    PatientApptPCLRequests_ListDelete.Add(p);
//                }
//            }
//        }

//        public void hplDeleteApptService_Click(object selectedItem)
//        {
//            if (MessageBox.Show(eHCMSResources.K0483_G1_BanCoChacChanMuonXoa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
//            {
//                var p = (selectedItem as PatientApptServiceDetails);

//                CurrentAppointment.PatientApptServiceDetailList.Remove(p);

//                if (CurrentFormState == FormState.EDIT)
//                {
//                    //Bỏ vào ds Delete
//                    if (PatientApptServiceDetails_ListDelete == null)
//                    {
//                        PatientApptServiceDetails_ListDelete = new List<PatientApptServiceDetails>();
//                    }

//                    PatientApptServiceDetails_ListDelete.Add(p);
//                }
//            }
//        }

//        public void CancelAppointmentCmd()
//        {
//            Globals.EventAggregator.Publish(new AppointmentAddEditCloseEvent());
//            TryClose();
//        }

//        #endregion

//        public bool CanbtUpdateApptPCL_Cmd
//        {
//            get
//            {
//                if (SelectedApptPclRequest != null && SelectedApptPclRequest.PCLRequestNumID != "")
//                {
//                    return true;
//                }
//                return false;
//            }
//        }

//        public bool CanEditApptPclRequestCmd
//        {
//            get
//            {
//                return true;
//            }
//        }
//        public void EditApptPclRequestCmd()
//        {
//            butCLS = false;
//            if (SelectedApptPclRequest == null)
//            {
//                MessageBox.Show(eHCMSResources.A0299_G1_Msg_InfoChon1YCCLS);
//                butCLS = true;
//                return;
//            }
//            if (SelectedApptPclRequest.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Imaging)
//            {
//                _patientPCLApptRequestImage = Globals.GetViewModel<IPatientPCLRequestEditImage>();
//                _patientPCLApptRequestImage.IsAppointment = true;
//                _patientPCLApptRequestImage.IsEdit = true;

//                _patientPCLApptRequestImage.InitPatientInfo(CurrentPatient);
//                SelectedApptPclRequest.PatientAppointment = CurrentAppointment;
//                _patientPCLApptRequestImage.StartEditing(CurrentPatient.PatientID, SelectedApptPclRequest);
//                Action<IPatientPCLRequestEditImage> onInitDlg = delegate (IPatientPCLRequestEditImage patientPCLApptRequestImage)
//                {
//                    patientPCLApptRequestImage = _patientPCLApptRequestImage;
//                };
//                GlobalsNAV.ShowDialog<IPatientPCLRequestEditImage>(onInitDlg);
//            }
//            else
//            {
//                _patientPCLApptRequest = Globals.GetViewModel<IPatientPCLRequestEdit>();
//                _patientPCLApptRequest.IsAppointment = true;
//                _patientPCLApptRequest.IsEdit = true;
//                _patientPCLApptRequest.InitPatientInfo(CurrentPatient);
//                SelectedApptPclRequest.PatientAppointment = CurrentAppointment;
//                _patientPCLApptRequest.StartEditing(CurrentPatient.PatientID, SelectedApptPclRequest);
//                Action<IPatientPCLRequestEdit> onInitDlg = delegate (IPatientPCLRequestEdit patientPCLApptRequest)
//                {
//                    patientPCLApptRequest = _patientPCLApptRequest;
//                };
//                GlobalsNAV.ShowDialog<IPatientPCLRequestEdit>(onInitDlg);
//            }
//        }
//        public void EditApptPclRequestCmdOld()
//        {
//            if (SelectedApptPclRequest == null)
//            {
//                MessageBox.Show(eHCMSResources.A0299_G1_Msg_InfoChon1YCCLS);
//                return;
//            }
//            _addEditAppPclVm = Globals.GetViewModel<IAddEditApptPCLRequest>();
//            _addEditAppPclVm.Registration_DataStorage = Registration_DataStorage;
//            SelectedApptPclRequest.PatientAppointment = CurrentAppointment;
//            _addEditAppPclVm.StartEditing(CurrentPatient.PatientID, SelectedApptPclRequest);
//            Action<IAddEditApptPCLRequest> onInitDlg = delegate (IAddEditApptPCLRequest addEditAppPclVm)
//            {
//                addEditAppPclVm.Registration_DataStorage = Registration_DataStorage;
//                addEditAppPclVm = _addEditAppPclVm;
//            };
//            GlobalsNAV.ShowDialog<IAddEditApptPCLRequest>(onInitDlg);
//        }

//        public void EditGeneralInfoCmd()
//        {
//            if (CurrentPatient == null)
//                return;

//            // Txd 14/07/2014: Commented the following block of code and replaced 
//            //                  with a a call to Coroutine method OpenPatientDetailDialog

//            //var patientDetailsVm = Globals.GetViewModel<IPatientDetails>();
//            //patientDetailsVm.CurrentAction = eHCMSResources.Z1023_G1_XemTTinBN;

//            //patientDetailsVm.FormState = FormState.READONLY;
//            //patientDetailsVm.CloseWhenFinish = true;
//            //patientDetailsVm.StartEditingPatientLazyLoad(CurrentPatient);
//            //patientDetailsVm.ActiveTab = PatientInfoTabs.GENERAL_INFO;
//            ////patientDetailsVm.ActivationMode = ActivationMode.EDIT_PATIENT_GENERAL_INFO;
//            //patientDetailsVm.ActivationMode = ActivationMode.PATIENT_GENERAL_HI_VIEW;
//            //Globals.ShowDialog(patientDetailsVm as Conductor<object>);

//            Coroutine.BeginExecute(OpenPatientDetailDialog());
//        }

//        private IEnumerator<IResult> OpenPatientDetailDialog()
//        {
//            var patientDetailsVm = Globals.GetViewModel<IPatientDetails>();
//            patientDetailsVm.CurrentAction = eHCMSResources.Z1023_G1_XemTTinBN;

//            patientDetailsVm.FormState = FormState.READONLY;
//            patientDetailsVm.CloseWhenFinish = true;
//            patientDetailsVm.InitLoadControlData_FromExt(null);

//            yield return new GenericCoRoutineTask(patientDetailsVm.LoadPatientDetailsAndHI_GenAction, CurrentPatient, true);

//            patientDetailsVm.ActiveTab = PatientInfoTabs.GENERAL_INFO;
//            //patientDetailsVm.ActivationMode = ActivationMode.EDIT_PATIENT_GENERAL_INFO;
//            patientDetailsVm.ActivationMode = ActivationMode.PATIENT_GENERAL_HI_VIEW;
//            Action<IPatientDetails> onInitDlg = delegate (IPatientDetails _patientDetailsVm)
//            {
//                _patientDetailsVm = patientDetailsVm;
//            };
//            GlobalsNAV.ShowDialog<IPatientDetails>(onInitDlg);

//            yield break;
//        }

//        //List Ca khi chọn phòng trong lưới
//        public void cboDeptLocationService_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            var Ctr = (sender as AxComboBox);
//            if (Ctr != null)
//            {
//                if (Ctr.SelectedItemEx != null)
//                {
//                    var rowInfo = (Ctr.DataContext) as PatientApptServiceDetails;

//                    DeptLocation DeptLocation_tmp = Ctr.SelectedItemEx as DeptLocation;

//                    if (rowInfo != null)
//                    {
//                        if (rowInfo.EntityState == EntityState.PERSITED)
//                        {
//                            rowInfo.EntityState = EntityState.MODIFIED;
//                        }

//                        ConsultationTimeSegments_ByDeptLocationID(DeptLocation_tmp.DeptLocationID, rowInfo);
//                    }
//                }
//            }
//        }

//        public void cboSegmentService_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            var Ctr = (sender as AxComboBox);
//            if (Ctr != null)
//            {
//                if (Ctr.SelectedItemEx != null)
//                {
//                    var rowInfo = (Ctr.DataContext) as PatientApptServiceDetails;

//                    if (rowInfo != null && rowInfo.DeptLocation != null && rowInfo.DeptLocation.DeptLocationID > 0)
//                    {
//                        if (rowInfo.EntityState == EntityState.PERSITED)
//                        {
//                            rowInfo.EntityState = EntityState.MODIFIED;
//                        }
//                    }
//                }
//            }
//        }

//        public void Handle(ItemEdited<PatientApptPCLRequests> message)
//        {
//            butCLS = true;
//            if (message.Item == null)
//            {
//                return;
//            }
//            if (GetView() == null)
//            {
//                return;
//            }
//            if (message.Source != null && message.Source == _patientPCLApptRequest)
//            {
//                if (!CurrentAppointment.PatientApptPCLRequestsList.Contains(message.Item))
//                {
//                    CurrentAppointment.PatientApptPCLRequestsList.Add(message.Item);
//                }
//                // 20181012 TNHX: [BM0002166] Fix error when click "Dong y" at add CLS-XN for HenBenh
//                var tmp = (_patientPCLApptRequest as Conductor<object>.Collection.AllActive);
//                tmp.TryClose();
//                NotifyOfPropertyChange(() => FilteredCollection);
//            }
//            else if (message.Source != null && message.Source == _patientPCLApptRequestImage)
//            {
//                if (!CurrentAppointment.PatientApptPCLRequestsList.Contains(message.Item))
//                {
//                    CurrentAppointment.PatientApptPCLRequestsList.Add(message.Item);
//                }
//                // 20181012 TNHX: [BM0002166] Fix error when click "Dong y" at add CLS-HA for HenBenh
//                var tmp = (_patientPCLApptRequestImage as Conductor<object>.Collection.AllActive);
//                tmp.TryClose();
//                NotifyOfPropertyChange(() => FilteredCollection);
//            }
//        }


//        public void btPrintKB_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
//        {
//            if (e.ClickCount > 1)
//            {
//                return;
//            }
//            if (CurrentAppointment == null || CurrentAppointment.PatientApptServiceDetailList == null || CurrentAppointment.PatientApptServiceDetailList.Count <= 0)
//            {
//                MessageBox.Show(eHCMSResources.A0720_G1_Msg_InfoKhTheXemInDVKBRong);
//                return;
//            }
//            else
//            {
//                bool canPrint = false;
//                bool HasReminder = false;

//                foreach (var serItem in CurrentAppointment.PatientApptServiceDetailList)
//                {
//                    if (serItem.ApptSvcDetailID > 0)
//                    {
//                        canPrint = true;
//                    }
//                    if (serItem.ApptSvcDetailID <= 0)
//                    {
//                        HasReminder = true;
//                    }
//                }

//                if (canPrint == false)
//                {
//                    MessageBox.Show(eHCMSResources.A0719_G1_Msg_InfoKhTheXemInDVKBChuaLuu);
//                }
//                else
//                {
//                    if (HasReminder)
//                    {
//                        if (MessageBox.Show(eHCMSResources.Z0378_G1_DSDVKBCoThayDoiChuaLuu + Environment.NewLine + eHCMSResources.Z0379_G1_ChiHienThiDVDaLuu + Environment.NewLine + eHCMSResources.Z0380_G1_CoMuonXemInKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
//                        {
//                            StartViewPrintKB();
//                        }
//                    }
//                    else
//                    {
//                        StartViewPrintKB();
//                    }
//                }

//            }

//        }


//        private void StartViewPrintHI_New()
//        {
//            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
//            {
//                proAlloc.AppointmentID = CurrentAppointment.AppointmentID;

//                proAlloc.eItem = ReportName.HI_APPOINTMENT;
//            };
//            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
//        }

//        private void StartViewPrintHI()
//        {
//            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
//            {
//                proAlloc.RegistrationID = CurrentAppointment.PtRegistrationID;
//                //proAlloc.ServiceRecID = RegistrationID.Value;
//                proAlloc.AppointmentID = CurrentAppointment.AppointmentID;

//                if (CurrentAppointment.CreatedByInPtRegis)
//                {
//                    proAlloc.eItem = ReportName.REGISTRATION_HI_APPOINTMENT_INPT;
//                }
//                else
//                {
//                    proAlloc.eItem = ReportName.REGISTRATION_HI_APPOINTMENT;
//                }
//            };
//            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
//        }

//        private void StartViewPrintKB()
//        {
//            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
//            {
//                proAlloc.AppointmentID = CurrentAppointment.AppointmentID;

//                proAlloc.eItem = ReportName.RptPatientApptServiceDetails;
//            };
//            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
//        }

//        private void StartViewPrintCLS()
//        {
//            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
//            {
//                proAlloc.AppointmentID = CurrentAppointment.AppointmentID;
//                //proAlloc.PatientPCLReqID = SelectedApptPclRequest.PatientPCLReqID;
//                proAlloc.strIDList = strPCLRequestIDList;
//                proAlloc.eItem = ReportName.RptPatientApptPCLRequests;
//            };
//            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
//        }

//        private string GetPCLRequestIDList(List<PatientApptPCLRequests> ListItems)
//        {
//            StringBuilder sb = new StringBuilder();
            
//            //if (CurrentAppointment.PatientApptPCLRequestsList != null)
//            if (ListItems != null)
//            {
//                sb.Append("<Root>");
//                foreach (var PCLRequest in ListItems)
//                {
//                    sb.Append("<PCLReqIDList>");
//                    sb.AppendFormat("<PCLReqID>{0}</PCLReqID>", PCLRequest.PatientPCLReqID);
//                    sb.Append("</PCLReqIDList>");
//                }
//                sb.Append("</Root>");
//            }
//            else
//            {
//                return string.Empty;
//            }

//            return sb.ToString();
//        }

//        private string strPCLRequestIDList = string.Empty;

//        public void btPrintCLS_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
//        {
//            if (e.ClickCount > 1)
//            {
//                return;
//            }

//            if (SelectedApptPclRequest != null && SelectedApptPclRequest.PatientPCLReqID > 0)
//            {
//                //KMx: Thực ra chỉ có 1 ApptPclRequest.
//                //     Nhưng vì sử dụng chung hàm GetPCLRequestIDList() với "In tổng hợp Hình ảnh" và "In tổng hợp Xét nghiệm" nên phải tạo list.
//                List<PatientApptPCLRequests> PCLItems = new List<PatientApptPCLRequests>();
//                PCLItems.Add(SelectedApptPclRequest);
//                strPCLRequestIDList = GetPCLRequestIDList(PCLItems);
//                StartViewPrintCLS();
//                strPCLRequestIDList = null;
//            }
//            else
//            {
//                if (SelectedApptPclRequest == null)
//                {
//                    MessageBox.Show(eHCMSResources.A0297_G1_Msg_InfoChonPhCLSDeXemIn);
//                    return;
//                }
//                if (SelectedApptPclRequest.PatientPCLReqID <= 0)
//                {
//                    MessageBox.Show(eHCMSResources.A0723_G1_Msg_InfoKhTheInPhChuaLuu);
//                    return;
//                }
//            }
//        }

//        public void btPrintTongHopHA_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
//        {
//            if (e.ClickCount > 1)
//            {
//                return;
//            }
//            List<PatientApptPCLRequests> PCLImagingItems = new List<PatientApptPCLRequests>();
//            PCLImagingItems = CurrentAppointment.PatientApptPCLRequestsList.Where(x => x.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Imaging).ToList();
//            if (PCLImagingItems != null && PCLImagingItems.Count > 0)
//            {
//                strPCLRequestIDList = GetPCLRequestIDList(PCLImagingItems);
//                StartViewPrintCLS();
//                strPCLRequestIDList = null;
//            }
//            else
//            {
//                MessageBox.Show(eHCMSResources.A0721_G1_Msg_InfoKhTheXemInPhHA, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                return;
//            }
//        }

//        public void btPrintTongHopXN_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
//        {
//            if (e.ClickCount > 1)
//            {
//                return;
//            }

//            List<PatientApptPCLRequests> PCLLaboratoryItems = new List<PatientApptPCLRequests>();
//            PCLLaboratoryItems = CurrentAppointment.PatientApptPCLRequestsList.Where(x => x.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Laboratory).ToList();
//            if (PCLLaboratoryItems != null && PCLLaboratoryItems.Count > 0)
//            {
//                strPCLRequestIDList = GetPCLRequestIDList(PCLLaboratoryItems);
//                StartViewPrintCLS();
//                strPCLRequestIDList = null;
//            }
//            else
//            {
//                MessageBox.Show(eHCMSResources.A0722_G1_Msg_InfoKhTheXemInPhXN, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                return;
//            }
//        }

//        CheckBox chkAllowPaperReferralUseNextConsult;
//        public void chkAllowPaperReferralUseNextConsult_Loaded(object sender, RoutedEventArgs e)
//        {
//            chkAllowPaperReferralUseNextConsult = sender as CheckBox;
//        }

//        public void chkAllowPaperReferralUseNextConsult_UnCheck(object sender, RoutedEventArgs e)
//        {
//            if (chkAllowPaperReferralUseNextConsult == null || CurrentAppointment == null || (!CurrentAppointment.HasChronicDisease.GetValueOrDefault() && string.IsNullOrWhiteSpace(CurrentAppointment.ReasonToAllowPaperReferral)))
//            {
//                return;
//            }

//            if (MessageBox.Show(eHCMSResources.A0428_G1_Msg_InfoTuDongXoaLyDo, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
//            {
//                CurrentAppointment.HasChronicDisease = false;
//                CurrentAppointment.ReasonToAllowPaperReferral = "";
//            }
//            else
//            {
//                chkAllowPaperReferralUseNextConsult.IsChecked = true;
//            }
//        }


//        public bool CheckBeforePrintAll()
//        {
//            if (CurrentAppointment == null || CurrentAppointmentCopy == null
//                || (
//                    (CurrentAppointment.PatientApptServiceDetailList == null || !CurrentAppointment.PatientApptServiceDetailList.Any(x => x.ApptSvcDetailID > 0))
//                    && 
//                    (CurrentAppointment.PatientApptPCLRequestsList == null ||!CurrentAppointment.PatientApptPCLRequestsList.Any(y => y.PatientPCLReqID > 0))
//                    )
//                )
//            {
//                MessageBox.Show(eHCMSResources.A0631_G1_Msg_InfoInCuocHen, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                return false;
//            }

//            if (CurrentAppointmentCopy.AllowPaperReferralUseNextConsult != CurrentAppointment.AllowPaperReferralUseNextConsult
//                || CurrentAppointmentCopy.HasChronicDisease != CurrentAppointment.HasChronicDisease
//                || CurrentAppointmentCopy.ReasonToAllowPaperReferral != CurrentAppointment.ReasonToAllowPaperReferral)
//            {
//                MessageBox.Show(eHCMSResources.Z0375_G1_KgTheXemInCuocHenCoThayDoi, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                return false;
//            }

//            if (CurrentAppointmentCopy.AllowPaperReferralUseNextConsult &&
//                !CurrentAppointmentCopy.HasChronicDisease.GetValueOrDefault() && string.IsNullOrWhiteSpace(CurrentAppointmentCopy.ReasonToAllowPaperReferral))
//            {
//                MessageBox.Show(eHCMSResources.Z0377_G1_ChuaLuuLiDoSDGCV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                return false;
//            }
 
//            return true;
//        }

//        public bool CheckBeforePrinPCL()
//        {
//            if (CurrentAppointment == null || CurrentAppointmentCopy == null
//                || (CurrentAppointment.PatientApptPCLRequestsList == null || !CurrentAppointment.PatientApptPCLRequestsList.Any(y => y.PatientPCLReqID > 0)))
//            {
//                MessageBox.Show(eHCMSResources.Z0521_G1_KgCoHenCLSDeIn, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                return false;
//            }

//            return true;
//        }

//        public void PreparePrintAll(bool isPrintMedService, bool isPrintPCL, bool isPrintNewTemplate)
//        {
//            //KMx: Nếu không có hẹn DV thì không in phiếu hẹn DV (30/11/2015 15:47).
//            if (isPrintMedService && CurrentAppointment.PatientApptServiceDetailList.Any(x => x.ApptSvcDetailID > 0))
//            {
//                if (!CurrentAppointment.AllowPaperReferralUseNextConsult)
//                {
//                    CurrentAppointment.ServiceDetailPrintType = AllLookupValues.AppServiceDetailPrintType.NormalApp;
//                }
//                else
//                {
//                    if (isPrintNewTemplate)
//                    {
//                        CurrentAppointment.ServiceDetailPrintType = AllLookupValues.AppServiceDetailPrintType.HIApp_New;
//                    }
//                    else
//                    {
//                        if (CurrentAppointment.CreatedByInPtRegis)
//                        {
//                            CurrentAppointment.ServiceDetailPrintType = AllLookupValues.AppServiceDetailPrintType.HIApp_InPt;
//                        }
//                        else
//                        {
//                            CurrentAppointment.ServiceDetailPrintType = AllLookupValues.AppServiceDetailPrintType.HIApp;
//                        }
//                    }
//                }
//            }
//            else
//            {
//                CurrentAppointment.ServiceDetailPrintType = AllLookupValues.AppServiceDetailPrintType.None;
//            }

//            List<PatientApptPCLRequests> PCLLaboratoryItems = new List<PatientApptPCLRequests>();
//            PCLLaboratoryItems = CurrentAppointment.PatientApptPCLRequestsList.Where(x => x.PatientPCLReqID > 0 && x.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Laboratory).ToList();
//            if (isPrintPCL && PCLLaboratoryItems != null && PCLLaboratoryItems.Count > 0)
//            {
//                CurrentAppointment.LaboratoryPCLRequestIDListXml = GetPCLRequestIDList(PCLLaboratoryItems);
//                CurrentAppointment.IsPrintLaboratoryPCLApp = true;
//            }
//            else
//            {
//                CurrentAppointment.LaboratoryPCLRequestIDListXml = null;
//                CurrentAppointment.IsPrintLaboratoryPCLApp = false;
//            }

//            List<PatientApptPCLRequests> PCLImagingItems = new List<PatientApptPCLRequests>();
//            PCLImagingItems = CurrentAppointment.PatientApptPCLRequestsList.Where(x => x.PatientPCLReqID > 0 && x.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Imaging).ToList();
//            if (isPrintPCL && PCLImagingItems != null && PCLImagingItems.Count > 0)
//            {
//                CurrentAppointment.ImagingPCLRequestIDListXml = GetPCLRequestIDList(PCLImagingItems);
//                CurrentAppointment.IsPrintImagingPCLApp = true;
//            }
//            else
//            {
//                CurrentAppointment.ImagingPCLRequestIDListXml = null;
//                CurrentAppointment.IsPrintImagingPCLApp = false;
//            }
//        }

//        private void PrintAllXRptAppointment(bool isPrintMedService, bool isPrintPCL, bool isPrintNewTemplate = false)
//        {
//            //if (!CheckBeforePrintAll())
//            //{
//            //    return;
//            //}

//            PreparePrintAll(isPrintMedService, isPrintPCL, isPrintNewTemplate);

//            this.ShowBusyIndicator();
//            var t = new Thread(() =>
//            {
//                try
//                {

//                    using (var serviceFactory = new ReportServiceClient())
//                    {
//                        var contract = serviceFactory.ServiceInstance;

//                        contract.BeginGetAllXRptAppointment(CurrentAppointment, Globals.DispatchCallback((asyncResult) =>
//                        {
//                            try
//                            {
//                                var results = contract.EndGetAllXRptAppointment(asyncResult);
//                                //var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, results, ActiveXPrintType.ByteArray, numOfCopies);
//                                var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, results, ActiveXPrintType.ByteArray);
//                                Globals.EventAggregator.Publish(printEvt);

//                            }
//                            catch (Exception ex)
//                            {
//                                MessageBox.Show(ex.Message);
//                            }
//                            finally
//                            {
//                                this.HideBusyIndicator();
//                            }

//                        }), null);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    ClientLoggerHelper.LogInfo(ex.ToString());
//                    this.HideBusyIndicator();
//                }
//            });
//            t.Start();
//        }

//        public void btnPrintAll_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
//        {
//            if (e.ClickCount > 1)
//            {
//                return;
//            }

//            if (!CheckBeforePrintAll())
//            {
//                return;
//            }

//            PrintAllXRptAppointment(true, true, false);
//        }

//        public void btnPrintAll_New_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
//        {
//            if (e.ClickCount > 1)
//            {
//                return;
//            }

//            if (!CheckBeforePrintAll())
//            {
//                return;
//            }

//            PrintAllXRptAppointment(true, true, true);
//        }

//        public void btnPrintPCL_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
//        {
//            if (e.ClickCount > 1)
//            {
//                return;
//            }

//            if (!CheckBeforePrinPCL())
//            {
//                return;
//            }

//            PrintAllXRptAppointment(false, true);
//        }
//        private IRegistration_DataStorage _Registration_DataStorage;
//        public IRegistration_DataStorage Registration_DataStorage
//        {
//            get
//            {
//                return _Registration_DataStorage;
//            }
//            set
//            {
//                if (_Registration_DataStorage == value)
//                {
//                    return;
//                }
//                _Registration_DataStorage = value;
//                NotifyOfPropertyChange(() => Registration_DataStorage);
//            }
//        }
//        private bool _IsAppointment = false;
//        public bool IsAppointment
//        {
//            get
//            {
//                return _IsAppointment;
//            }
//            set
//            {
//                if (_IsAppointment == value)
//                {
//                    return;
//                }
//                _IsAppointment = value;
//                NotifyOfPropertyChange(() => IsAppointment);
//            }
//        }
//        //1: Hẹn bệnh CLS sổ, 0: Hẹn tái khám
//        private bool _IsPCLBookingView = false;
//        public bool IsPCLBookingView
//        {
//            get
//            {
//                return _IsPCLBookingView;
//            }
//            set
//            {
//                if (_IsPCLBookingView == value)
//                {
//                    return;
//                }
//                _IsPCLBookingView = value;
//                NotifyOfPropertyChange(() => IsPCLBookingView);
//            }
//        }
//    }
//}