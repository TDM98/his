using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts.Configuration;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IfrmBloodType)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class frmBloodTypeViewModel : Conductor<object>, IfrmBloodType
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public frmBloodTypeViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            _allBloodType = new ObservableCollection<BloodType>();
            GetAllBloodTypes();
        }

        private ObservableCollection<BloodType> _allBloodType;
        public ObservableCollection<BloodType> allBloodType
        {
            get
            {
                return _allBloodType;
            }
            set
            {
                if (_allBloodType == value)
                    return;
                _allBloodType = value;
                NotifyOfPropertyChange(() => allBloodType);
            }
        }

        private BloodType _selectedBloodType;
        public BloodType selectedBloodType
        {
            get
            {
                return _selectedBloodType;
            }
            set
            {
                _selectedBloodType = value;
                NotifyOfPropertyChange(() => selectedBloodType);
            }
        }
        private  Patient _PatientInfo;
        public  Patient PatientInfo
        {
            get { return _PatientInfo; }
            set
            {
                if (_PatientInfo != value)
                {
                    _PatientInfo = value;
                    if (_PatientInfo.VBloodType != null)
                    {
                        selectedBloodType = _PatientInfo.VBloodType;
                    }
                    NotifyOfPropertyChange(() => PatientInfo);
                }
            }
        }
        public void butExit()
        {
            TryClose();
        }
    
        public void butSave()
        {
            PatientInfo.VBloodType = selectedBloodType;
            UpdatePatientBloodType(PatientInfo.PatientID, selectedBloodType.BloodTypeID);
            TryClose();
        }


        public void GetAllBloodTypes()
        {
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true,Message =string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.K2871_G1_DangLoadDLieu)});
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllBloodTypes(
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                var allItems = contract.EndGetAllBloodTypes(asyncResult);

                                if (allItems != null)
                                {
                                    allBloodType = new ObservableCollection<BloodType>();
                                    foreach (var bloodType in allItems)
                                    {
                                        allBloodType.Add(bloodType);
                                    }
                                }
                                
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    Globals.IsBusy = false;
                }
            });
            t.Start();
        }
        public void UpdatePatientBloodType(long PatientID, int? BloodTypeID)
        {
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.K2871_G1_DangLoadDLieu });
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginUpdatePatientBloodType(PatientID,  BloodTypeID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                var allItems = contract.EndUpdatePatientBloodType(asyncResult);
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    Globals.IsBusy = false;
                }
            });
            t.Start();
        }
    }
}
