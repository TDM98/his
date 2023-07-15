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
    [Export(typeof(ISATQuaThucQuanChanDoan_Consultation)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SATQuaThucQuanChanDoanViewModel : Conductor<object>, ISATQuaThucQuanChanDoan_Consultation
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SATQuaThucQuanChanDoanViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
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

        private URP_FE_Oesophagienne _curURP_FE_Oesophagienne;
        public URP_FE_Oesophagienne curURP_FE_Oesophagienne
        {
            get { return _curURP_FE_Oesophagienne; }
            set
            {
                if (_curURP_FE_Oesophagienne != value)
                {
                    _curURP_FE_Oesophagienne = value;
                    NotifyOfPropertyChange(() => curURP_FE_Oesophagienne);
                }
            }
        }

        #endregion

        #region method

        private void GetURP_FE_OesophagienneByID(long URP_FE_OesophagienneID, long PCLImgResultID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetURP_FE_Oesophagienne(URP_FE_OesophagienneID, PCLImgResultID, Globals.DispatchCallback((asyncResult) =>
                                                                  {
                                                                      try
                                                                      {
                                                                          var item = contract.EndGetURP_FE_Oesophagienne(asyncResult);
                                                                          if (item != null)
                                                                          {
                                                                              curURP_FE_Oesophagienne = item;
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
            //CheckSave();
            curURP_FE_Oesophagienne = new URP_FE_Oesophagienne();
            GetURP_FE_OesophagienneByID(0, PatientPCLReqID);
           // GetPCLExamResultTemplateListByTypeIDChiDinh(4, (int)AllLookupValues.PCLResultParamImpID.SIEUAM_THUCQUAN);
           // GetPCLExamResultTemplateListByTypeIDThanhNguc(5, (int)AllLookupValues.PCLResultParamImpID.SIEUAM_THUCQUAN);
        }
    }
}