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
    [Export(typeof (ISAMMKhac_Consultation)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SAMMKhacViewModel : Conductor<object>, ISAMMKhac_Consultation
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SAMMKhacViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
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

        private URP_FE_VasculaireAnother _curURP_FE_VasculaireAnother;
      
        public URP_FE_VasculaireAnother curURP_FE_VasculaireAnother
        {
            get { return _curURP_FE_VasculaireAnother; }
            set
            {
                if (_curURP_FE_VasculaireAnother == value)
                    return;
                _curURP_FE_VasculaireAnother = value;
                NotifyOfPropertyChange(() => curURP_FE_VasculaireAnother);
            }
        }


        #endregion

        #region method

        private void GetURP_FE_VasculaireAnotherByID(long URP_FE_VasculaireAnotherID, long PCLImgResultID)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginGetURP_FE_VasculaireAnother(URP_FE_VasculaireAnotherID, PCLImgResultID,Globals.DispatchCallback((asyncResult) =>
                                                                                             {
                                                                                                 try
                                                                                                 {
                                                                                                     var item=contract.EndGetURP_FE_VasculaireAnother(asyncResult);
                                                                                                     if(item!=null)
                                                                                                     {
                                                                                                         curURP_FE_VasculaireAnother = item;
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
            //_tempURP_FE_VasculaireAnother = new URP_FE_VasculaireAnother();
            curURP_FE_VasculaireAnother = new URP_FE_VasculaireAnother();

            GetURP_FE_VasculaireAnotherByID(0, PatientPCLReqID);
           // GetPCLExamResultTemplateListByTypeID(4, (int) AllLookupValues.PCLResultParamImpID.SIEUAM_MACHMAU);
            //V_MotaEx
        }
    }
}