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
    [Export(typeof (ISAMMChiDuoi_Consultation)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SAMMChiDuoiViewModel : Conductor<object>, ISAMMChiDuoi_Consultation
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SAMMChiDuoiViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Globals.EventAggregator.Subscribe(this);

            if(Globals.PatientPCLReqID_Imaging>0)
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
        private URP_FE_StressDobutamineImages _curURP_FE_StressDobutamineImages;
      
        public URP_FE_StressDobutamineImages curURP_FE_StressDobutamineImages
        {
            get { return _curURP_FE_StressDobutamineImages; }
            set
            {
                if (_curURP_FE_StressDobutamineImages == value)
                    return;
                _curURP_FE_StressDobutamineImages = value;
                NotifyOfPropertyChange(() => curURP_FE_StressDobutamineImages);
            }
        }
        #endregion

        #region method

        private void GetURP_FE_StressDobutamineImagesByID(long URP_FE_StressDobutamineImagesID
                                                          , long PCLImgResultID)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginGetURP_FE_StressDobutamineImages(URP_FE_StressDobutamineImagesID, PCLImgResultID, Globals.DispatchCallback((asyncResult) =>
                                                                                              {
                                                                                                  try
                                                                                                  {
                                                                                                      var item = contract.EndGetURP_FE_StressDobutamineImages(asyncResult);
                                                                                                      if(item!=null)
                                                                                                      {
                                                                                                          curURP_FE_StressDobutamineImages = item;
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
            //GetV_ValveOpen();
            //CheckHasPCLImageID();
            //CheckSave();

            //_tempURP_FE_StressDobutamineImages = new URP_FE_StressDobutamineImages();
            curURP_FE_StressDobutamineImages = new URP_FE_StressDobutamineImages();

            GetURP_FE_StressDobutamineImagesByID(0, PatientPCLReqID);
           // GetPCLExamResultTemplateListByTypeID(3, (int)AllLookupValues.PCLResultParamImpID.SIEUAM_MACHMAU);
        }
    }
}