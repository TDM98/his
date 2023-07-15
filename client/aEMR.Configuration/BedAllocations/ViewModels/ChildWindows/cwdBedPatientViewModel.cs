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
    [Export(typeof(IcwdBedPatient)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class cwdBedPatientViewModel : Conductor<object>, IcwdBedPatient 
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
        public cwdBedPatientViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            _PatientInfo =new Patient();
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            
        }

        public void imBedLoaded(object sender, RoutedEventArgs e)
        {
            imBed = sender as Image;
            initForm();
        }

        private bool _isDelete=false;
        public bool isDelete
        {
            get
            {
                return _isDelete;
            }
            set
            {
                if (_isDelete == value)
                    return;
                _isDelete = value;
            }
        }
        
        private BedPatientAllocs _selectedBedPatientAllocs;
        public BedPatientAllocs selectedBedPatientAllocs
        {
            get
            {
                return _selectedBedPatientAllocs;
            }
            set
            {
                if (_selectedBedPatientAllocs == value)
                    return;
                _selectedBedPatientAllocs = value;
                NotifyOfPropertyChange(() => selectedBedPatientAllocs);
            }
        }

        private bool _IsSaveEnable;
        private bool _IsDeleteEnable;

        public bool IsSaveEnable
        {
            get
            {
                return _IsSaveEnable;
            }
            set
            {
                if (_IsSaveEnable == value)
                    return;
                _IsSaveEnable = value;
                NotifyOfPropertyChange(() => IsSaveEnable);
            }
        }
        public bool IsDeleteEnable
        {
            get
            {
                return _IsDeleteEnable;
            }
            set
            {
                if (_IsDeleteEnable == value)
                    return;
                _IsDeleteEnable = value;
                NotifyOfPropertyChange(() => IsDeleteEnable);
            }
        }

        private string _txtFullNameOld;
        private string _txtPatientCodeOld;
        private string _txtFullNameNew;
        private string _txtPatientCodeNew;

        public string txtFullNameOld
        {
            get
            {
                return _txtFullNameOld;
            }
            set
            {
                if (_txtFullNameOld == value)
                    return;
                _txtFullNameOld = value;
                NotifyOfPropertyChange(() => txtFullNameOld);
            }
        }
        public string txtPatientCodeOld
        {
            get
            {
                return _txtPatientCodeOld;
            }
            set
            {
                if (_txtPatientCodeOld == value)
                    return;
                _txtPatientCodeOld = value;
                NotifyOfPropertyChange(() => txtPatientCodeOld);
            }
        }
        public string txtFullNameNew
        {
            get
            {
                return _txtFullNameNew;
            }
            set
            {
                if (_txtFullNameNew == value)
                    return;
                _txtFullNameNew = value;
                NotifyOfPropertyChange(() => txtFullNameNew);
            }
        }
        public string txtPatientCodeNew
        {
            get
            {
                return _txtPatientCodeNew;
            }
            set
            {
                if (_txtPatientCodeNew == value)
                    return;
                _txtPatientCodeNew = value;
                NotifyOfPropertyChange(() => txtPatientCodeNew);
            }
        }

        private BedPatientAllocs _selectedTempBedPatientAllocs;
        public BedPatientAllocs selectedTempBedPatientAllocs
        {
            get
            {
                return _selectedTempBedPatientAllocs;
            }
            set
            {
                if (_selectedTempBedPatientAllocs == value)
                    return;
                _selectedTempBedPatientAllocs = value;
                NotifyOfPropertyChange(() => selectedTempBedPatientAllocs);
            }
        }

        private Patient _PatientInfo;
        public Patient PatientInfo
        {
            get
            {
                return _PatientInfo;
            }
            set
            {
                if (_PatientInfo == value)
                    return;
                _PatientInfo = value;
                NotifyOfPropertyChange(() => PatientInfo);
            }
        }

        public void butDelete()
        {
            if (MessageBox.Show(eHCMSResources.A0249_G1_Msg_ConfBNMuonTraGiuong, "Message", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeleteBedPatientAllocs(selectedBedPatientAllocs.BedPatientID);
            }
            TryClose();
        }
        public void butSave()
        {
            if (selectedBedPatientAllocs.CheckInDate == null)
            {
                selectedBedPatientAllocs.CheckInDate = DateTime.Now;
            }
            DateTime dateTime = (DateTime)selectedBedPatientAllocs.CheckInDate;

            DateTime dT = new System.DateTime(dateTime.Year
                                        , dateTime.Month
                                        , dateTime.Day + selectedBedPatientAllocs.ExpectedStayingDays
                                        );
            if (selectedTempBedPatientAllocs.VPtRegistration.PtRegistrationID < 1)
                //|| selectedTempBedPatientAllocs.VPtRegistration.PtRegistrationID <1)
            {
                selectedBedPatientAllocs.PtRegistrationID = 1;
            }
            else
            {
                selectedBedPatientAllocs.PtRegistrationID = selectedTempBedPatientAllocs.VPtRegistration.PtRegistrationID;
            }
            AddNewBedPatientAllocs(selectedBedPatientAllocs.BedAllocationID
                , selectedBedPatientAllocs.PtRegistrationID
                , selectedBedPatientAllocs.CheckInDate
                , selectedBedPatientAllocs.ExpectedStayingDays
                , dT
                , true);
            TryClose();
        }
        public void initForm()
        {
            Image image = new Image();
            Uri uri = null;
            if(isDelete==true)
            {
                IsSaveEnable = false;
                IsDeleteEnable = true;
                uri = new Uri("/aEMR.CommonViews;component/Assets/Images/Bed4.png", UriKind.Relative);
                GetPatientInfo(selectedBedPatientAllocs.PtRegistrationID);
                txtFullNameNew = "";
                txtPatientCodeNew= "";
            }
            else
            if (selectedBedPatientAllocs.IsActive == false)
            {
                IsSaveEnable = true;
                IsDeleteEnable = false;
                uri = new Uri("/aEMR.CommonViews;component/Assets/Images/Bed6.jpg", UriKind.Relative);
                txtFullNameNew = selectedTempBedPatientAllocs.VPtRegistration.FullName;
                txtPatientCodeNew= selectedTempBedPatientAllocs.VPtRegistration.PatientCode;
                txtFullNameOld = "";
                txtPatientCodeOld = "";
            }
            else
            {
                IsSaveEnable = true;
                IsDeleteEnable = true;
                txtFullNameNew = selectedTempBedPatientAllocs.VPtRegistration.FullName;
                txtPatientCodeNew = selectedTempBedPatientAllocs.VPtRegistration.PatientCode;
                uri = new Uri("/aEMR.CommonViews;component/Assets/Images/Bed4.png", UriKind.Relative);              
                GetPatientInfo(selectedBedPatientAllocs.PtRegistrationID);
                
            }

            ImageSource img = new BitmapImage(uri);
            ((Image)imBed).SetValue(Image.SourceProperty, img);
        }

        
        private object _imBed;
        public object imBed
        {
            get
            {
                return _imBed;
            }
            set
            {
                if (_imBed == value)
                    return;
                _imBed = value;
            }
        }
        public void butExit()
        {
            TryClose();
        }
        private BedPatientAllocs CreateBedPatientAlloc(long BedAllocationID
                                                  , long PtRegistrationID
                                                  , DateTime? AdmissionDate
                                                  , int ExpectedStayingDays
                                                  , DateTime? DischargeDate
                                                  , bool IsActive)
        {
            BedPatientAllocs alloc = new BedPatientAllocs();
            alloc.BedAllocationID = BedAllocationID;
            alloc.PtRegistrationID = PtRegistrationID;
            alloc.CheckInDate = AdmissionDate;
            alloc.ExpectedStayingDays = ExpectedStayingDays;
            alloc.CheckOutDate = DischargeDate;
            alloc.IsActive = IsActive;
            return alloc;
        }
        private void AddNewBedPatientAllocs(long BedAllocationID
                                                  , long PtRegistrationID
                                                  , DateTime? AdmissionDate
                                                  , int ExpectedStayingDays
                                                  , DateTime? DischargeDate
                                                  , bool IsActive)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading=true;

                using (var serviceFactory = new BedAllocationsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    var alloc = CreateBedPatientAlloc(BedAllocationID
                                                 , PtRegistrationID
                                                 , AdmissionDate
                                                 , ExpectedStayingDays
                                                 , DischargeDate
                                                 , IsActive);
                    contract.BeginAddNewBedPatientAllocs(alloc, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndAddNewBedPatientAllocs(asyncResult);
                            if (results == 1)
                            {
                                Globals.EventAggregator.Publish(new BedAllocEvent { });
                                MessageBox.Show(eHCMSResources.A0507_G1_Msg_InfoDatGiuongOK);
                                Globals.EventAggregator.Publish(new AddCompleted<BedPatientAllocs>() { Item = asyncResult.AsyncState as BedPatientAllocs });
                            }
                            else if (results > 1)
                            {
                                MessageBox.Show(eHCMSResources.A0562_G1_Msg_InfoGiuongDuBN);
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.A0506_G1_Msg_InfoDatGiuongFail);
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

        private void DeleteBedPatientAllocs(long BedPatientID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new BedAllocationsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDeleteBedPatientAllocs(BedPatientID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndDeleteBedPatientAllocs(asyncResult);
                            if(results==true)
                            {
                                Globals.EventAggregator.Publish(new BedAllocEvent{});
                                Globals.ShowMessage("Đặt giường cho bệnh nhân thành công!","");
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
        private int FindPatient = (int)AllLookupValues.V_FindPatientType.NOI_TRU;
        private void GetPatientInfo(long RegistrationID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new PatientRegistrationServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetPatientInfoByPtRegistration(RegistrationID, null, FindPatient,Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            PatientInfo= contract.EndGetPatientInfoByPtRegistration(asyncResult);
                            txtFullNameOld = PatientInfo.FullName;
                            txtPatientCodeOld = PatientInfo.PatientCode;
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
    }
}

