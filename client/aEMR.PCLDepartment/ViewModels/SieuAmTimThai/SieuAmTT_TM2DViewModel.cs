using eHCMSLanguage;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.Controls;
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

namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof (ISieuAmTT_TM2D)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SieuAmTT_TM2DViewModel : Conductor<object>, ISieuAmTT_TM2D
      , IHandle<ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SieuAmTT_TM2DViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
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

        private UltraResParams_FetalEchocardiography2D _curURP_FE2D;
        private bool _isSave;
        private bool _isUpdate;

        private UltraResParams_FetalEchocardiography2D _tempURP_FE2D;

        //curURP_FE2D.UltraResParams_FetalEchocardiography2DID
        //curURP_FE2D.PCLImgResultID
        //curURP_FE2D.DoctorStaffID
        private bool isHasPatient;

        public UltraResParams_FetalEchocardiography2D curURP_FE2D
        {
            get { return _curURP_FE2D; }
            set
            {
                if (_curURP_FE2D == value)
                    return;
                _curURP_FE2D = value;
                NotifyOfPropertyChange(() => curURP_FE2D);
            }
        }

        public UltraResParams_FetalEchocardiography2D tempURP_FE2D
        {
            get { return _tempURP_FE2D; }
            set
            {
                if (_tempURP_FE2D == value)
                    return;
                _tempURP_FE2D = value;
                NotifyOfPropertyChange(() => tempURP_FE2D);
            }
        }

        public bool IsHasPatient
        {
            get { return isHasPatient; }
            set
            {
                if (isHasPatient == value)
                    return;
                isHasPatient = value;
                NotifyOfPropertyChange(() => isHasPatient);
            }
        }

        public bool isSave
        {
            get { return _isSave; }
            set
            {
                if (_isSave == value)
                    return;
                _isSave = value;
                NotifyOfPropertyChange(() => isSave);
                //_isUpdate= !isSave;
            }
        }

        public bool isUpdate
        {
            get { return _isUpdate; }
            set
            {
                if (_isUpdate == value)
                    return;
                _isUpdate = value;
                NotifyOfPropertyChange(() => isUpdate);
                //_isSave = !isUpdate;
            }
        }

        #endregion

        #region fuction

        private ValidationSummary validationSummary { get; set; }

        public void CheckHasPCLImageID()
        {
            isHasPatient = true;
            //if()
            //{
            //    isHasPatient = true;
            //}else
            //{
            //    isHasPatient = false;
            //}
        }

        public void CheckSave()
        {
            isSave = true;
            isUpdate = false;
        }

        public void CheckUpdate()
        {
            isSave = false;
            isUpdate = true;
        }

        public void butReset()
        {
            curURP_FE2D = new UltraResParams_FetalEchocardiography2D();
            curURP_FE2D = tempURP_FE2D;
            NotifyOfPropertyChange(() => curURP_FE2D);
        }

        public void butSave()
        {
            if (validationSummary.DisplayedErrors.Count > 0)
            {
                MessageBox.Show(eHCMSResources.A0559_G1_Msg_InfoGTriKhHopLe);
                return;
            }

            if (curURP_FE2D.UltraResParams_FetalEchocardiography2DID>0)
            {
                UpdateUltraResParams_FetalEchocardiography2D(curURP_FE2D);
            }
            else
            {
                curURP_FE2D.PCLImgResultID = PatientPCLReqID;
                curURP_FE2D.DoctorStaffID = Globals.LoggedUserAccount.Staff.StaffID;
                AddUltraResParams_FetalEchocardiography2D(curURP_FE2D);
            }
        }

        public void butUpdate()
        {
            if (validationSummary.DisplayedErrors.Count > 0)
            {
                MessageBox.Show(eHCMSResources.A0559_G1_Msg_InfoGTriKhHopLe);
                return;
            }
            
        }

        public void ValidationSummary_Loaded(object sender, RoutedEventArgs e)
        {
            validationSummary = sender as ValidationSummary;
        }

        #endregion

        #region method

        private void AddUltraResParams_FetalEchocardiography2D(UltraResParams_FetalEchocardiography2D entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginAddUltraResParams_FetalEchocardiography2D(entity,
                                                                                                   Globals.
                                                                                                       DispatchCallback(
                                                                                                           (asyncResult)
                                                                                                           =>
                                                                                                               {
                                                                                                                   try
                                                                                                                   {
                                                                                                                       bool res=contract.EndAddUltraResParams_FetalEchocardiography2D(asyncResult);
                                                                                                                       if(res)
                                                                                                                       {
                                                                                                                           MessageBox.Show(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK);
                                                                                                                       }
                                                                                                                       else
                                                                                                                       {
                                                                                                                           MessageBox.Show("Thêm mới thất bại!");
                                                                                                                       }
                                                                                                                   }
                                                                                                                   catch(Exception ex)
                                                                                                                   {
                                                                                                                       MessageBox.Show(ex.Message);
                                                                                                                   }
                                                                                                                   finally
                                                                                                                   {
                                                                                                                       Globals.IsBusy=false;
                                                                                                                   }
                                                                                                               }), null);
                                       }
                                   });

            t.Start();
        }

        private void UpdateUltraResParams_FetalEchocardiography2D(UltraResParams_FetalEchocardiography2D entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginUpdateUltraResParams_FetalEchocardiography2D(entity,
                                                                                                      Globals.
                                                                                                          DispatchCallback
                                                                                                          ((asyncResult)
                                                                                                           =>
                                                                                                               {
                                                                                                                   try
                                                                                                                   {
                                                                                                                       bool res=contract.EndUpdateUltraResParams_FetalEchocardiography2D(asyncResult);
                                                                                                                       if(res)
                                                                                                                       {
                                                                                                                           MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK);
                                                                                                                       }
                                                                                                                       else
                                                                                                                       {
                                                                                                                           MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail);
                                                                                                                       }
                                                                                                                   }
                                                                                                                   catch(Exception ex)
                                                                                                                   {
                                                                                                                       MessageBox.Show(ex.Message);
                                                                                                                   }
                                                                                                                   finally
                                                                                                                   {
                                                                                                                       Globals.IsBusy=false;
                                                                                                                   }
                                                                                                               }), null);
                                       }
                                   });

            t.Start();
        }

        private void GetUltraResParams_FetalEchocardiography2D(long UltraResParams_FetalEchocardiography2DID, long PCLImgResultID)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginGetUltraResParams_FetalEchocardiography2D(UltraResParams_FetalEchocardiography2DID, PCLImgResultID, Globals.DispatchCallback((asyncResult) =>
                                                                                              {
                                                                                                  try
                                                                                                  {
                                                                                                      var item=contract.EndGetUltraResParams_FetalEchocardiography2D(asyncResult);
                                                                                                      if(item!=null)
                                                                                                      {
                                                                                                          curURP_FE2D = item;
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


        public void Handle(DbClickSelectedObjectEvent<PatientPCLRequest> message)
        {
            if (message != null)
            {
                PatientPCLReqID = message.Result.PatientPCLReqID;
                LoadInfo();
            }
        }

        public void Handle(ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest> message)
        {
            if (message != null)
            {
                PatientPCLReqID = message.PCLRequest.PatientPCLReqID;
                LoadInfo();
            }
        }

        public void LoadInfo()
        {
            //CheckHasPCLImageID();
            //CheckSave();
            //_tempURP_FE2D = new UltraResParams_FetalEchocardiography2D();
            //==== 20161129 CMN Begin: Add button save for all pages
            //curURP_FE2D = new UltraResParams_FetalEchocardiography2D();
            //GetUltraResParams_FetalEchocardiography2D(0, PatientPCLReqID);
            //==== 20161129 CMN End: Add button save for all pages
        }
        //==== 20161129 CMN Begin: Add button save for all pages
        public void SetResultDetails(object resDetails)
        {
            curURP_FE2D = (UltraResParams_FetalEchocardiography2D)resDetails;
        }
        //==== 20161129 CMN End: Add button save for all pages
    }
}