using eHCMSLanguage;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using DataEntities;
using PCLsProxy;

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(ISieuAmTim_2D_Consultation)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SieuAmTim_2DViewModel : Conductor<object>, ISieuAmTim_2D_Consultation
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SieuAmTim_2DViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            //Globals.EventAggregator.Subscribe(this);


            if (Globals.PatientPCLReqID_Imaging > 0)
            {
                PatientPCLReqID = Globals.PatientPCLReqID_Imaging;
                LoadInfo();
            }
         
        }

        private long _PatientPCLReqID;
        public long PatientPCLReqID
        {
            get { return _PatientPCLReqID; }
            set
            {
                if (_PatientPCLReqID != value)
                {
                    _PatientPCLReqID = value;
                    NotifyOfPropertyChange(() => PatientPCLReqID);
                }
            }
        }

        #region properties

        private UltraResParams_EchoCardiography _curUltraResParams_EchoCardiography;

        public UltraResParams_EchoCardiography curUltraResParams_EchoCardiography
        {
            get { return _curUltraResParams_EchoCardiography; }
            set
            {
                if (_curUltraResParams_EchoCardiography == value)
                    return;
                _curUltraResParams_EchoCardiography = value;
                NotifyOfPropertyChange(() => curUltraResParams_EchoCardiography);
            }
        }

        #endregion


        #region method

        private void GetUltraResParams_EchoCardiographyByID(long UltraResParams_EchoCardiographyID, long PCLImgResultID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetUltraResParams_EchoCardiography(UltraResParams_EchoCardiographyID, PCLImgResultID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var item = contract.EndGetUltraResParams_EchoCardiography(asyncResult);
                            if (item != null)
                            {
                                curUltraResParams_EchoCardiography = item;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            Globals.IsBusy =false;
                        }
                    }), null);
                }
            });

            t.Start();
        }

        #endregion

        private void LoadInfo()
        {
           // GetV_ValveOpen();
            //CheckHasPCLImageID();
            //CheckSave();
            curUltraResParams_EchoCardiography = new UltraResParams_EchoCardiography();
            GetUltraResParams_EchoCardiographyByID(0, PatientPCLReqID);

        }

      
    }
}