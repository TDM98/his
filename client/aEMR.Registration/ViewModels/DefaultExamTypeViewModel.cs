using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Threading;
using aEMR.DataContracts;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using System.ComponentModel.Composition;
using DataEntities;
using aEMR.Common.BaseModel;
using aEMR.Infrastructure;
using aEMR.Common.Collections;
using aEMR.Common;
using eHCMSLanguage;
using Castle.Windsor;
using Caliburn.Micro;

namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IDefaultExamType)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DefaultExamTypeViewModel : ViewModelBase, IDefaultExamType
    {
        [ImportingConstructor]
        public DefaultExamTypeViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {

        }
        private long _medServiceID;

        public long MedServiceID
        {
            get { return _medServiceID; }
            set 
            {
                _medServiceID = value;
                NotifyOfPropertyChange(()=>MedServiceID);

                LoadDefaultPCLExamTypes();
            }
        }

        private ObservableCollection<PCLExamType> _pclExamTypes;

        public ObservableCollection<PCLExamType> PCLExamTypes
        {
            get { return _pclExamTypes;}
            set { _pclExamTypes = value; 
            NotifyOfPropertyChange(()=> PCLExamTypes);}
        }

        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            set 
            {
                _isLoading = value; 
                NotifyOfPropertyChange(()=>IsLoading);
                NotifyWhenBusy();
            }
        }

        public override bool IsProcessing
        {
            get
            {
                return _isLoading;
            }
        }
        public override string StatusText
        {
            get
            {
                if (_isLoading)
                {
                    return eHCMSResources.Z0141_G1_DangLayDSCLS;
                }
                return "";
            }
        }

        public void LoadDefaultPCLExamTypes()
        {
            if (PCLExamTypes != null)
            {
                PCLExamTypes.Clear();
            }
            if(MedServiceID <= 0)
            {
                return;
            }
            var t = new Thread(() =>
            {
                IsLoading = true;
                using (var serviceFactory = new PatientRegistrationServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetPCLExamTypesByMedServiceID(MedServiceID,
                        Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var allItems = contract.EndGetPCLExamTypesByMedServiceID(asyncResult);
                                if (allItems != null)
                                {
                                    PCLExamTypes = allItems.ToObservableCollection();
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
                            finally
                            {
                                IsLoading = false;
                            }
                        }), null);
                }
            });
            t.Start();
        }

        public void gridExamTypes_Loaded(object source,object eventArgs)
        {
            
        }
    }
}
