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
    [Export(typeof(ISATQuaThucQuanBangKiemTra_Consultation)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SATQuaThucQuanBangKiemTraViewModel : Conductor<object>, ISATQuaThucQuanBangKiemTra_Consultation
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SATQuaThucQuanBangKiemTraViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            //Globals.EventAggregator.Subscribe(this);
            //Thay bằng
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

        private URP_FE_OesophagienneCheck _curURP_FE_OesophagienneCheck;
        
        public URP_FE_OesophagienneCheck curURP_FE_OesophagienneCheck
        {
            get { return _curURP_FE_OesophagienneCheck; }
            set
            {
                if (_curURP_FE_OesophagienneCheck == value)
                    return;
                _curURP_FE_OesophagienneCheck = value;
                NotifyOfPropertyChange(() => curURP_FE_OesophagienneCheck);
            }
        }

        #endregion

        #region method

        private void GetURP_FE_OesophagienneCheckByID(long URP_FE_OesophagienneCheckID, long PCLImgResultID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetURP_FE_OesophagienneCheck(URP_FE_OesophagienneCheckID, PCLImgResultID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var item = contract.EndGetURP_FE_OesophagienneCheck(asyncResult);
                            if (item != null)
                            {
                                curURP_FE_OesophagienneCheck = item;
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

        public void LoadInfo()
        {
            //GetV_ValveOpen();
            //CheckHasPCLImageID();
            //CheckSave();
            //_tempURP_FE_OesophagienneCheck = new URP_FE_OesophagienneCheck();
            curURP_FE_OesophagienneCheck = new URP_FE_OesophagienneCheck();
            GetURP_FE_OesophagienneCheckByID(0, PatientPCLReqID);
        }

      
    }
}