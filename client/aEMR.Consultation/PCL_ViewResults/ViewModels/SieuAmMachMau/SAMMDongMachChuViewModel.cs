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
    [Export(typeof (ISAMMDongMachChu_Consultation)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SAMMDongMachChuViewModel : Conductor<object>, ISAMMDongMachChu_Consultation
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SAMMDongMachChuViewModel(IWindsorContainer container, INavigationService navigationService , ISalePosCaching salePosCaching)
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

        private URP_FE_VasculaireAorta _curURP_FE_VasculaireAorta;
      
        public URP_FE_VasculaireAorta curURP_FE_VasculaireAorta
        {
            get { return _curURP_FE_VasculaireAorta; }
            set
            {
                if (_curURP_FE_VasculaireAorta == value)
                    return;
                _curURP_FE_VasculaireAorta = value;
                NotifyOfPropertyChange(() => curURP_FE_VasculaireAorta);
            }
        }

        #endregion


        #region method

        private void GetURP_FE_VasculaireAortaByID(long URP_FE_VasculaireAortaID, long PCLImgResultID)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginGetURP_FE_VasculaireAorta(URP_FE_VasculaireAortaID, PCLImgResultID,Globals.DispatchCallback((asyncResult) =>
                                                                                           {
                                                                                               try
                                                                                               {
                                                                                                   
                                                                                                   var item=contract.EndGetURP_FE_VasculaireAorta(asyncResult);
                                                                                                   if (item != null)
                                                                                                   {
                                                                                                       curURP_FE_VasculaireAorta= item;
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
            //_tempURP_FE_VasculaireAorta = new URP_FE_VasculaireAorta();
            curURP_FE_VasculaireAorta = new URP_FE_VasculaireAorta();

            GetURP_FE_VasculaireAortaByID(0, PatientPCLReqID);
           // GetPCLExamResultTemplateListByTypeID(2, (int) AllLookupValues.PCLResultParamImpID.SIEUAM_MACHMAU);
        }
    }
}