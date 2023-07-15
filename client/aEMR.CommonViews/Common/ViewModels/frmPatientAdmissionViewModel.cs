using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.ViewContracts.Configuration;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using eHCMSLanguage;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IfrmPatientAdmission)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class frmPatientAdmissionViewModel : Conductor<object>, IfrmPatientAdmission
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public frmPatientAdmissionViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            GetAllAdmissionType();
            curInPatientAdmDisDetails=new InPatientAdmDisDetails();
        }

        //
        private ObservableCollection<Lookup> _allAdmissionType;
        public ObservableCollection<Lookup> allAdmissionType
        {
            get
            {
                return _allAdmissionType;
            }
            set
            {
                if (_allAdmissionType == value)
                    return;
                _allAdmissionType = value;
                NotifyOfPropertyChange(() => allAdmissionType);
            }
        }

        private Lookup _curAdmissionType;
        public Lookup curAdmissionType
        {
            get
            {
                return _curAdmissionType;
            }
            set
            {
                if (_curAdmissionType == value)
                    return;
                _curAdmissionType = value;
                NotifyOfPropertyChange(() => curAdmissionType);
            }
        }

        private PatientRegistration _curPatientRegistration;
        public PatientRegistration curPatientRegistration
        {
            get
            {
                return _curPatientRegistration;
            }
            set
            {
                if (_curPatientRegistration == value)
                    return;
                _curPatientRegistration = value;
                
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
                    NotifyOfPropertyChange(() => PatientInfo);
                }
            }
        }

        private InPatientAdmDisDetails _curInPatientAdmDisDetails;
        public InPatientAdmDisDetails curInPatientAdmDisDetails
        {
            get
            {
                return _curInPatientAdmDisDetails;
            }
            set
            {
                if (_curInPatientAdmDisDetails == value)
                    return;
                _curInPatientAdmDisDetails = value;
                NotifyOfPropertyChange(() => curInPatientAdmDisDetails);
            }
        }

        public void butExit()
        {
            TryClose();
        }

    
        public void butSave()
        {
            //PatientInfo.VBloodType = selectedBloodType;
            //UpdatePatientBloodType(PatientInfo.PatientID, selectedBloodType.BloodTypeID);
            curInPatientAdmDisDetails.PtRegistrationID = curPatientRegistration.PtRegistrationID;
            curInPatientAdmDisDetails.DeptID =(long) curPatientRegistration.DeptID;
            InsertInPatientAdmDisDetails(curInPatientAdmDisDetails);
            TryClose();
        }


        private void GetAllAdmissionType()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllLookupValuesByType(LookupValues.ADMISSION_TYPE, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetAllLookupValuesByType(asyncResult);
                            if (results != null)
                            {
                                if (allAdmissionType == null)
                                {
                                    allAdmissionType = new ObservableCollection<Lookup>();
                                }
                                else
                                {
                                    allAdmissionType.Clear();
                                }
                                foreach (var p in results)
                                {
                                    allAdmissionType.Add(p);
                                }

                                NotifyOfPropertyChange(() => allAdmissionType);
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

                    }), null);

                }

            });

            t.Start();
        }

        private void InsertInPatientAdmDisDetails(InPatientAdmDisDetails entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginInsertInPatientAdmDisDetails(entity, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndInsertInPatientAdmDisDetails(asyncResult);
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
        
    }
}
