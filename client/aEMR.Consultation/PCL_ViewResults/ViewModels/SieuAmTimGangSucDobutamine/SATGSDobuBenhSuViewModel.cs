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
    [Export(typeof (ISATGSDobuBenhSu_Consultation)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SATGSDobuBenhSuViewModel : Conductor<object>, ISATGSDobuBenhSu_Consultation
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SATGSDobuBenhSuViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
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

        private URP_FE_Exam _curURP_FE_Exam;
        private URP_FE_StressDobutamineExam _curURP_FE_StressDobutamineExam;
    
        public URP_FE_StressDobutamineExam curURP_FE_StressDobutamineExam
        {
            get { return _curURP_FE_StressDobutamineExam; }
            set
            {
                if (_curURP_FE_StressDobutamineExam == value)
                    return;
                _curURP_FE_StressDobutamineExam = value;
                NotifyOfPropertyChange(() => curURP_FE_StressDobutamineExam);
            }
        }

        public URP_FE_Exam curURP_FE_Exam
        {
            get { return _curURP_FE_Exam; }
            set
            {
                if (_curURP_FE_Exam == value)
                    return;
                _curURP_FE_Exam = value;
                NotifyOfPropertyChange(() => curURP_FE_Exam);
            }
        }

        #endregion

        #region method

        private void GetURP_FE_StressDobutamineExamByID(long URP_FE_StressDobutamineExamID, long PCLImgResultID)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginGetURP_FE_StressDobutamineExam(URP_FE_StressDobutamineExamID, PCLImgResultID,
                                                                                        Globals.DispatchCallback(
                                                                                            (asyncResult) =>
                                                                                                {
                                                                                                    try
                                                                                                    {
                                                                                                        var item=contract.EndGetURP_FE_StressDobutamineExam(asyncResult);
                                                                                                        if(item!=null)
                                                                                                        {
                                                                                                            curURP_FE_StressDobutamineExam = item;  
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

        private void GetURP_FE_ExamByID(long URP_FE_ExamID, long PCLImgResultID)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginGetURP_FE_Exam(URP_FE_ExamID, PCLImgResultID,Globals.DispatchCallback((asyncResult) =>
                                                                                                     {
                                                                                                         try
                                                                                                         {
                                                                                                             var item=contract.EndGetURP_FE_Exam(asyncResult);
                                                                                                             if(item!=null)
                                                                                                             {
                                                                                                                 curURP_FE_Exam = item;
                                                                                                             }
                                                                                                         }
                                                                                                         catch (Exception ex)
                                                                                                         {
                                                                                                             MessageBox.Show(ex.Message);
                                                                                                         }
                                                                                                         finally
                                                                                                         {
                                                                                                             Globals.IsBusy= false;
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
            //_tempURP_FE_StressDobutamineExam = new URP_FE_StressDobutamineExam();
            curURP_FE_StressDobutamineExam = new URP_FE_StressDobutamineExam();
            //_tempURP_FE_Exam = new URP_FE_Exam();
            curURP_FE_Exam = new URP_FE_Exam();

            GetURP_FE_StressDobutamineExamByID(0, PatientPCLReqID);
            GetURP_FE_ExamByID(0, PatientPCLReqID);

        }

    }
}