using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ServiceModel;
using System.Threading;
using System.Windows.Input;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using Caliburn.Micro;
using aEMR.ViewContracts;
using System.ComponentModel.Composition;
using DataEntities;
using aEMR.Common.Collections;
using eHCMS.Services.Core;
using System.Windows;
using aEMR.Controls;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Linq;
using System.Windows.Controls;
using aEMR.Common.BaseModel;
using System.Text.RegularExpressions;
using aEMR.Common;
/*
* 20170113 #001 CMN: Added QRCode
* 20171103 #002 CMN: Added Show HICardNo
* 20180904 #003 TTM: Ngăn không cho tìm kiếm rỗng
* 20181122 #004 TTM: BM 0005299: Cho phép tìm kiếm bệnh nhân kèm DOB
*/
namespace aEMR.Appointment.ViewModels
{
    [Export(typeof(IHealthExaminationRecordInfo)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class HealthExaminationRecordInfoViewModel : ViewModelBase, IHealthExaminationRecordInfo
         , IHandle<AddCompleted<Patient>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
        }
        protected override void OnDeactivate(bool close)
        {
            Globals.EventAggregator.Unsubscribe(this);
            base.OnDeactivate(close);
        }

        [ImportingConstructor]
        public HealthExaminationRecordInfoViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            LoadHealthClassification();
        }

        private void LoadHealthClassification()
        {
            HealthClassifications = new List<HealthClassification>(){
                new HealthClassification(){Name = "Loại I", Value = 1},
                new HealthClassification(){Name = "Loại II", Value = 2 },
                new HealthClassification(){Name = "Loại III", Value = 3 },
                new HealthClassification(){Name = "Loại IV", Value = 4 },
                new HealthClassification(){Name = "Loại V", Value = 5 },
            };

            cboHealthClassification = new AxComboBox();
            cboHealthClassification.ItemsSource = HealthClassifications;
            SelectedHealthClassification = HealthClassifications.FirstOrDefault();
        }

        public class HealthClassification
        {
            public string Name { get; set; }
            public long Value { get; set; }
        }

        public void CancelCmd()
        {
            SelectedPatient = null;
            TryClose();
        }

        public void OkCmd()
        {
            if (MedicalResult != null)
                AddResults(MedicalResult);
            // Globals.ShowMessage(MedicalResult.CurrentHealth.ToString(), eHCMSResources.T0432_G1_Error);
            //Globals.EventAggregator.Publish(new ItemSelected<Patient>() { Item = SelectedPatient });
            //TryClose();
        }

        private Patient _selectedPatient;

        public Patient SelectedPatient
        {
            get { return _selectedPatient; }
            set
            {
                _selectedPatient = value;
                NotifyOfPropertyChange(() => SelectedPatient);
            }
        }

        private HosClientContractPatient _HosPatient;
        public HosClientContractPatient HosPatient
        {
            get { return _HosPatient; }
            set
            {
                _HosPatient = value;
                if (HosPatient.PtRegistrationID == null) HosPatient.PtRegistrationID = 0;
                LoadMedicalExaminationResultNew((long)HosPatient.PtRegistrationID);
                NotifyOfPropertyChange(() => HosPatient);
            }
        }

        private MedicalExaminationResult _MedicalResult;
        public MedicalExaminationResult MedicalResult
        {
            get { return _MedicalResult; }
            set
            {
                _MedicalResult = value;
                NotifyOfPropertyChange(() => MedicalResult);
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
        
        public void Handle(AddCompleted<Patient> message)
        {
            if (message != null && message.Item != null)
            {

            }
        }

        private HealthClassification _SelectedHealthClassification;
        public HealthClassification SelectedHealthClassification
        {
            get { return _SelectedHealthClassification; }
            set
            {
                _SelectedHealthClassification = value;
                NotifyOfPropertyChange(() => SelectedHealthClassification);
            }
        }

        private List<HealthClassification> _HealthClassifications;
        public List<HealthClassification> HealthClassifications
        {
            get { return _HealthClassifications; }
            set
            {
                _HealthClassifications = value;
                NotifyOfPropertyChange(() => HealthClassifications);
            }
        }

        AxComboBox cboHealthClassification { get; set; }
        public void cboHealthClassification_Loaded(object sender, RoutedEventArgs e)
        {
            cboHealthClassification = sender as AxComboBox;
        }
        public void cboHealthClassification_SelectionChanged(object sender, RoutedEventArgs e)
        {
            cboHealthClassification = sender as AxComboBox;
            if (cboHealthClassification == null)
            {
                return;
            }
            SelectedHealthClassification = cboHealthClassification.SelectedItemEx as HealthClassification;
            MedicalResult.HealthClassification = SelectedHealthClassification.Value;
        }

        private void AddResults(MedicalExaminationResult Result)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                IsLoading = true;
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z1102_G1_DangLuuTTinBN)
                });
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new ePMRsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginUpdateConclusionMedicalExaminationResult(Result, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var bOK = contract.EndUpdateConclusionMedicalExaminationResult(asyncResult);
                                
                                MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                                TryClose();
                            }
                            catch (Exception innerEx)
                            {
                                error = new AxErrorEventArgs(innerEx);
                            }
                        }), null);
                    }
                }
                catch (FaultException<AxException> fault)
                {
                    error = new AxErrorEventArgs(fault);
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {
                    Globals.IsBusy = false;
                    IsLoading = false;
                    this.HideBusyIndicator();
                }
                if (error != null)
                {
                    MessageBox.Show(error.ClientError.Message);
                    // Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
                }
            });
            t.Start();
        }

        private void LoadMedicalExaminationResultNew(long aPtRegistrationID)
        {
            MedicalResult = new MedicalExaminationResult { PtRegistrationID = aPtRegistrationID };
            if (aPtRegistrationID == 0)
            {
                return;
            }
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var mThread = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePMRsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetMedicalExaminationResultByPtRegistrationID(aPtRegistrationID, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                MedicalResult = contract.EndGetMedicalExaminationResultByPtRegistrationID(asyncResult);
                                if (MedicalResult == null || MedicalResult.PtRegistrationID == 0)
                                {
                                    MedicalResult = new MedicalExaminationResult {
                                        PtRegistrationID = aPtRegistrationID,
                                        HealthClassification = 1,
                                        CurrentHealth = "Đủ sức khỏe làm việc",
                                        HealthCheckUpDate = Globals.GetCurServerDateTime().ToLocalTime(),
                                        ExpiryDateHealthCertificate = Globals.GetCurServerDateTime().ToLocalTime().AddYears(1)
                                    };
                                }
                                if (MedicalResult.HealthClassification > 0)
                                    SelectedHealthClassification = HealthClassifications[(int)MedicalResult.HealthClassification - 1];
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            mThread.Start();
        }
    }
}