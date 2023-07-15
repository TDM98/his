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

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof (ISATGSDobuKetQua_Consultation)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SATGSDobuKetQuaViewModel : Conductor<object>, ISATGSDobuKetQua_Consultation
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SATGSDobuKetQuaViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
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

        private URP_FE_StressDobutamineResult _curURP_FE_StressDobutamineResult;
      
        public URP_FE_StressDobutamineResult curURP_FE_StressDobutamineResult
        {
            get { return _curURP_FE_StressDobutamineResult; }
            set
            {
                if (_curURP_FE_StressDobutamineResult == value)
                    return;
                _curURP_FE_StressDobutamineResult = value;
                NotifyOfPropertyChange(() => curURP_FE_StressDobutamineResult);
            }
        }

        #endregion

        #region method

        private void GetURP_FE_StressDobutamineResultByID(long URP_FE_StressDobutamineResultID, long PCLImgResultID)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginGetURP_FE_StressDobutamineResult(URP_FE_StressDobutamineResultID, PCLImgResultID, Globals.DispatchCallback((asyncResult) =>
                                                                                              {
                                                                                                  try
                                                                                                  {
                                                                                                      var item = contract.EndGetURP_FE_StressDobutamineResult(asyncResult);
                                                                                                      if(item!=null)
                                                                                                      {
                                                                                                          curURP_FE_StressDobutamineResult = item;
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

        private void GetURP_FE_StressDobutamineResultByIDResult(long URP_FE_StressDobutamineResultID
                                                                , long PCLImgResultID)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginGetUltraResParams_EchoCardiographyResult(
                                               URP_FE_StressDobutamineResultID
                                               , PCLImgResultID, Globals.DispatchCallback((asyncResult) =>
                                                                                              {
                                                                                                  try
                                                                                                  {
                                                                                                      string st =
                                                                                                          contract.
                                                                                                              EndGetUltraResParams_EchoCardiographyResult
                                                                                                              (asyncResult);
                                                                                                      curURP_FE_StressDobutamineResult
                                                                                                          .
                                                                                                          KetQuaSieuAmTim
                                                                                                          = st;

                                                                                                      //else
                                                                                                      //{
                                                                                                      //    curURP_FE_StressDobutamineResult = new URP_FE_StressDobutamineResult();
                                                                                                      //    CheckSave();
                                                                                                      //}
                                                                                                  }
                                                                                                  catch (Exception ex)
                                                                                                  {
                                                                                                      MessageBox.Show(ex.Message);
                                                                                                  }
                                                                                                  finally
                                                                                                  {
                                                                                                      Globals.IsBusy =
                                                                                                          false;
                                                                                                  }
                                                                                              }), null);
                                       }
                                   });

            t.Start();
        }

        #endregion

        public void LoadInfo()
        {
            //GetV_CardialStatus();
            //GetV_CardialResult();
            //CheckHasPCLImageID();
            //CheckSave();
            //_tempURP_FE_StressDobutamineResult = new URP_FE_StressDobutamineResult();
            curURP_FE_StressDobutamineResult = new URP_FE_StressDobutamineResult();
            GetURP_FE_StressDobutamineResultByID(0, PatientPCLReqID);
            
        }
    }
}