using System;
using System.Collections.Generic;
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
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Common.Utilities;
using Caliburn.Micro;
using DataEntities;
using Castle.Windsor;
using Castle.Core.Logging;
using eHCMSLanguage;
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IDiagnosisTextBox)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DiagnosisTextBoxViewModel : Conductor<object>, IDiagnosisTextBox
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public DiagnosisTextBoxViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAgr, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            eventAgr.Subscribe(this);
        }
        private PatientPCLRequest _currentPclRequest;
        public PatientPCLRequest CurrentPclRequest
        {
            get
            {
                return _currentPclRequest;
            }
            set
            {
                if (_currentPclRequest != value)
                {
                    _currentPclRequest = value;
                    NotifyOfPropertyChange(() => CurrentPclRequest);
                }
            }
        }

        private string _DiagnosisTreament;
        public string DiagnosisTreament
        {
            get
            {
                return _DiagnosisTreament;
            }
            set
            {
                if (_DiagnosisTreament != value)
                {
                    _DiagnosisTreament = value;
                    NotifyOfPropertyChange(() => DiagnosisTreament);
                }
            }
        }
        public void txtDiagnosis_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(DiagnosisTreament) || CurrentPclRequest == null)
            {
                return;
            }
            CurrentPclRequest.Diagnosis = DiagnosisTreament;
        }
        public bool bOK = false;
        public void UpdatePCLRequestInfo()
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdatePclRequestInfo(CurrentPclRequest,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    bOK = contract.EndUpdatePclRequestInfo(asyncResult);
                                    if (!bOK)
                                    {
                                        MessageBox.Show(string.Format("{0}!", eHCMSResources.A0270_G1_Msg_CNhatCDoan_YCFail));
                                    }
                                    else
                                    {
                                        MessageBox.Show(string.Format("{0}!", eHCMSResources.A0279_G1_Msg_InfoCNhatOK));
                                    }
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                    MessageBox.Show(fault.ToString(), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                }
                                finally
                                {
                                    if (bOK)
                                    {
                                        this.TryClose();
                                    }
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });
            t.Start();
        }
    }
}
