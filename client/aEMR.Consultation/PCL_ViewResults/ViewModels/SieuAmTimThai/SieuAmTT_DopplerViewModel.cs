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
using DataEntities;
using PCLsProxy;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof (ISieuAmTT_Doppler_Consultation)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SieuAmTT_DopplerViewModel : Conductor<object>, ISieuAmTT_Doppler_Consultation
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SieuAmTT_DopplerViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            //Globals.EventAggregator.Subscribe(this);
            //Thay bằng
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

        private UltraResParams_FetalEchocardiographyDoppler _curURP_FEDoppler;

        public UltraResParams_FetalEchocardiographyDoppler curURP_FEDoppler
        {
            get { return _curURP_FEDoppler; }
            set
            {
                if (_curURP_FEDoppler == value)
                    return;
                _curURP_FEDoppler = value;
                NotifyOfPropertyChange(() => curURP_FEDoppler);
            }
        }

        #endregion

        #region method

        private void GetUltraResParams_FetalEchocardiographyDopplerByID(long UltraResParams_FetalEchocardiographyDopplerID, long PCLImgResultID)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginGetUltraResParams_FetalEchocardiographyDopplerByID(UltraResParams_FetalEchocardiographyDopplerID, PCLImgResultID, Globals.DispatchCallback((asyncResult) =>
                                                                                              {
                                                                                                  try
                                                                                                  {
                                                                                                      var item=contract.EndGetUltraResParams_FetalEchocardiographyDopplerByID(asyncResult);
                                                                                                      if(item!=null)
                                                                                                      {
                                                                                                          curURP_FEDoppler = item;
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

        public void LoadInfo()
        {
           // GetV_ValveOpen();
            //CheckHasPCLImageID();
            //CheckSave();
            //_tempURP_FEDoppler = new UltraResParams_FetalEchocardiographyDoppler();
            curURP_FEDoppler = new UltraResParams_FetalEchocardiographyDoppler();
            GetUltraResParams_FetalEchocardiographyDopplerByID(0, PatientPCLReqID);
        }

     
    }
}