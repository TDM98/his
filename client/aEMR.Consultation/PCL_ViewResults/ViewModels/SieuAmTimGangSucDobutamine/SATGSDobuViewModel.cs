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
    [Export(typeof (ISATGSDobu_Consultation)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SATGSDobuViewModel : Conductor<object>, ISATGSDobu_Consultation
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SATGSDobuViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
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

        private URP_FE_StressDobutamine _curURP_FE_StressDobutamine;
        private URP_FE_VasculaireExam _curURP_FE_VasculaireExam;

        public URP_FE_StressDobutamine curURP_FE_StressDobutamine
        {
            get { return _curURP_FE_StressDobutamine; }
            set
            {
                if (_curURP_FE_StressDobutamine == value)
                    return;
                _curURP_FE_StressDobutamine = value;
                NotifyOfPropertyChange(() => curURP_FE_StressDobutamine);
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

        private void AddURP_FE_StressDobutamine(URP_FE_StressDobutamine entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginAddURP_FE_StressDobutamine(entity,
                                                                                    Globals.DispatchCallback(
                                                                                        (asyncResult) =>
                                                                                            {
                                                                                                try
                                                                                                {
                                                                                                    bool res =contract.EndAddURP_FE_StressDobutamine(asyncResult);
                                                                                                    if (res)
                                                                                                    {
                                                                                                        MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong);
                                                                                                    }
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

        private void UpdateURP_FE_StressDobutamine(URP_FE_StressDobutamine entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginUpdateURP_FE_StressDobutamine(entity,
                                                                                       Globals.DispatchCallback(
                                                                                           (asyncResult) =>
                                                                                               {
                                                                                                   try
                                                                                                   {
                                                                                                       bool res =contract.EndUpdateURP_FE_StressDobutamine(asyncResult);
                                                                                                       if (res)
                                                                                                       {
                                                                                                           MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                                                                                                       }
                                                                                                       else
                                                                                                       {
                                                                                                           MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong);
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

        private void GetURP_FE_StressDobutamineByID(long URP_FE_StressDobutamineID, long PCLImgResultID)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginGetURP_FE_StressDobutamine(URP_FE_StressDobutamineID, PCLImgResultID,Globals.DispatchCallback((asyncResult) =>
                                                                                            {
                                                                                                try
                                                                                                {
                                                                                                    var item = contract.EndGetURP_FE_StressDobutamine(asyncResult);
                                                                                                    if(item!=null)
                                                                                                    {
                                                                                                        curURP_FE_StressDobutamine = item;
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
            //CheckHasPCLImageID();
            //CheckSave();
            //_tempURP_FE_StressDobutamine = new URP_FE_StressDobutamine();
            curURP_FE_StressDobutamine = new URP_FE_StressDobutamine();
            GetURP_FE_StressDobutamineByID(0, PatientPCLReqID);
        }
    }
}