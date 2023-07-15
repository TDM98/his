using eHCMSLanguage;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using PCLsProxy;
using aEMR.Controls;

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof (ISATGSDipyHinh_Consultation)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SATGSDipyHinhViewModel : Conductor<object>, ISATGSDipyHinh_Consultation
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SATGSDipyHinhViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Globals.EventAggregator.Subscribe(this);

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

        private URP_FE_StressDipyridamoleImage _curURP_FE_StressDipyridamoleImage;

        public URP_FE_StressDipyridamoleImage curURP_FE_StressDipyridamoleImage
        {
            get { return _curURP_FE_StressDipyridamoleImage; }
            set
            {
                if (_curURP_FE_StressDipyridamoleImage == value)
                    return;
                _curURP_FE_StressDipyridamoleImage = value;
                NotifyOfPropertyChange(() => curURP_FE_StressDipyridamoleImage);
            }
        }

        #endregion


        #region method

        private void GetURP_FE_StressDipyridamoleImageByID(long URP_FE_StressDipyridamoleImageID, long PCLImgResultID)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginGetURP_FE_StressDipyridamoleImage(URP_FE_StressDipyridamoleImageID, PCLImgResultID, Globals.DispatchCallback((asyncResult) =>
                                                                                              {
                                                                                                  try
                                                                                                  {
                                                                                                      var item = contract.EndGetURP_FE_StressDipyridamoleImage(asyncResult);
                                                                                                      if(item!=null)
                                                                                                      {
                                                                                                          curURP_FE_StressDipyridamoleImage = item;
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
            //CheckHasPCLImageID();
            //CheckSave();
            //_tempURP_FE_StressDipyridamoleImage = new URP_FE_StressDipyridamoleImage();
            curURP_FE_StressDipyridamoleImage = new URP_FE_StressDipyridamoleImage();
            GetURP_FE_StressDipyridamoleImageByID(0, PatientPCLReqID);
            //GetPCLExamResultTemplateListByTypeID(5, (int)AllLookupValues.PCLResultParamImpID.SIEUAM_GANGSUC_Dipyridamole);
        }

    }
}