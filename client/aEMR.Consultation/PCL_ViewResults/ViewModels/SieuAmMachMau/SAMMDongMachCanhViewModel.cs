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
    [Export(typeof (ISAMMDongMachCanh_Consultation)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SAMMDongMachCanhViewModel : Conductor<object>, ISAMMDongMachCanh_Consultation
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SAMMDongMachCanhViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
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

        private URP_FE_VasculaireCarotid _curURP_FE_VasculaireCarotid;
        private URP_FE_VasculaireExam _curURP_FE_VasculaireExam;
        public URP_FE_VasculaireCarotid curURP_FE_VasculaireCarotid
        {
            get { return _curURP_FE_VasculaireCarotid; }
            set
            {
                if (_curURP_FE_VasculaireCarotid == value)
                    return;
                _curURP_FE_VasculaireCarotid = value;
                NotifyOfPropertyChange(() => curURP_FE_VasculaireCarotid);
            }
        }
        public URP_FE_VasculaireExam curURP_FE_VasculaireExam
        {
            get { return _curURP_FE_VasculaireExam; }
            set
            {
                if (_curURP_FE_VasculaireExam == value)
                    return;
                _curURP_FE_VasculaireExam = value;
                NotifyOfPropertyChange(() => curURP_FE_VasculaireExam);
            }
        }

        #endregion

        #region method

        private void GetURP_FE_VasculaireCarotidByID(long URP_FE_VasculaireCarotidID, long PCLImgResultID)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginGetURP_FE_VasculaireCarotid(URP_FE_VasculaireCarotidID, PCLImgResultID,Globals.DispatchCallback((asyncResult) =>
                                                                                             {
                                                                                                 try
                                                                                                 {
                                                                                                     var item=contract.EndGetURP_FE_VasculaireCarotid(asyncResult);
                                                                                                     if(item!=null)
                                                                                                     {
                                                                                                         curURP_FE_VasculaireCarotid= item;
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

        private void GetURP_FE_VasculaireExamByID(long URP_FE_VasculaireExamID, long PCLImgResultID)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginGetURP_FE_VasculaireExam(URP_FE_VasculaireExamID, PCLImgResultID,Globals.DispatchCallback((asyncResult) =>
                                                                                          {
                                                                                              try
                                                                                              {
                                                                                                  var item=contract.EndGetURP_FE_VasculaireExam(asyncResult);
                                                                                                  if(item!=null)
                                                                                                  {
                                                                                                      curURP_FE_VasculaireExam= item;
                                                                                                  }
                                                                                              }
                                                                                              catch (Exception ex)
                                                                                              {
                                                                                                  MessageBox.Show(ex.Message);
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

        #endregion

        private void LoadInfo()
        {
            //GetV_ValveOpen();
            //CheckHasPCLImageID();
            //CheckSave();
            //_tempURP_FE_VasculaireCarotid = new URP_FE_VasculaireCarotid();
            curURP_FE_VasculaireCarotid = new URP_FE_VasculaireCarotid();

            curURP_FE_VasculaireExam = new URP_FE_VasculaireExam();
            //_tempURP_FE_VasculaireExam = new URP_FE_VasculaireExam();
            GetURP_FE_VasculaireCarotidByID(0, PatientPCLReqID);
            GetURP_FE_VasculaireExamByID(0, PatientPCLReqID);
            //GetPCLExamResultTemplateListByTypeID(1, (int)AllLookupValues.PCLResultParamImpID.SIEUAM_MACHMAU);
        }
    }
}