/*
20170216 #001 CMN: Add Show InPtRegistration
*/
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
using Caliburn.Micro;
using DataEntities;
using System.Windows;
using System.Windows.Controls;
using eHCMSLanguage;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
using Castle.Windsor;

namespace aEMR.Common.ViewModels
{
    [Export(typeof (IRegistrationList)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class RegistrationListViewModel : Conductor<object>, IRegistrationList
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public RegistrationListViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            _registrations = new ObservableCollection<PatientRegistration>();
        }

        public void OkCmd()
        {
            TryClose();
        }

        public void DoubleClick(object args)
        {
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

        private Patient _currentPatient;

        public Patient CurrentPatient
        {
            get { return _currentPatient; }
            set
            {
                if (_currentPatient != value)
                {
                    _currentPatient = value;
                    NotifyOfPropertyChange(() => CurrentPatient);

                    if (_currentPatient == null)
                    {
                        Registrations.Clear();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(CurrentPatient.DemographicString))
                        {
                            CurrentPatient.DemographicString = CurrentPatient.DemographicString.Replace("\r\n", "")
                                .Replace("\n", "").Replace("\r", "");

                            CurrentPatient.DemographicString = CurrentPatient.DemographicString.Replace("\n","");
                            CurrentPatient.DemographicString = CurrentPatient.DemographicString.Replace("\r", "");
                            CurrentPatient.DemographicString = CurrentPatient.DemographicString.Replace(Environment.NewLine, "");
                            
                        }
                        SearchRegistrations(_currentPatient);
                    } 
                }
            }
        }
        //==== #001
        private bool _IsInPtRegistration = false;
        public bool IsInPtRegistration
        {
            get { return _IsInPtRegistration; }
            set
            {
                _IsInPtRegistration = value;
                NotifyOfPropertyChange(() => IsInPtRegistration);
            }
        }
        //==== #001
        private ObservableCollection<PatientRegistration> _registrations;
        public ObservableCollection<PatientRegistration> Registrations
        {
            get { return _registrations; }
            private set
            {
                _registrations = value;
                NotifyOfPropertyChange(()=>Registrations);
            }
        }
        /// <summary>
        /// Tìm những đăng ký của bệnh nhân hiện tại.
        /// </summary>
        private void SearchRegistrations(Patient p)
        {
            Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z1125_G1_DangTimKiem) });

            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                try
                {
                    IsLoading = true;
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetAllRegistrations(p.PatientID, IsInPtRegistration, Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<PatientRegistration> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndGetAllRegistrations(asyncResult);
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

                            _registrations.Clear();
                            if (bOK && allItems != null)
                            {
                                foreach (var item in allItems)
                                {
                                    _registrations.Add(item);
                                }
                            }
                            NotifyOfPropertyChange(()=> Registrations);
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
                    IsLoading = false;
                    Globals.IsBusy = false;
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                }
            });
            t.Start();
        }

        public void MaDKClick(object sender, RoutedEventArgs e) 
        {
            Globals.EventAggregator.Publish(new ItemSelected<PatientRegistration>() { Item = (PatientRegistration)((Button)sender).DataContext });
            TryClose();
        }
    }
}