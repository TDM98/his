using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof (ISieuAmTT_Doppler)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SieuAmTT_DopplerViewModel : Conductor<object>, ISieuAmTT_Doppler
    
         , IHandle<ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SieuAmTT_DopplerViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            //Globals.EventAggregator.Subscribe(this);
            GetV_ValveOpen();
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

        private ObservableCollection<Lookup> _allValveOpen;
        private UltraResParams_FetalEchocardiographyDoppler _curURP_FEDoppler;
        private bool _isSave;
        private bool _isUpdate;

        private UltraResParams_FetalEchocardiographyDoppler _tempURP_FEDoppler;

        //curURP_FEDoppler.UltraResParams_FetalEchocardiographyDopplerID
        //curURP_FEDoppler.PCLImgResultID
        //curURP_FEDoppler.DoctorStaffID
        private bool isHasPatient;

        public UltraResParams_FetalEchocardiographyDoppler curURP_FEDoppler
        {
            get { return _curURP_FEDoppler; }
            set
            {
                if (_curURP_FEDoppler == value)
                    return;
                _curURP_FEDoppler = value;
                NotifyOfPropertyChange(() => curURP_FEDoppler);
            }
        }

        public UltraResParams_FetalEchocardiographyDoppler tempURP_FEDoppler
        {
            get { return _tempURP_FEDoppler; }
            set
            {
                if (_tempURP_FEDoppler == value)
                    return;
                _tempURP_FEDoppler = value;
                NotifyOfPropertyChange(() => tempURP_FEDoppler);
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

        public ObservableCollection<Lookup> allValveOpen
        {
            get { return _allValveOpen; }
            set
            {
                if (_allValveOpen == value)
                    return;
                _allValveOpen = value;
                NotifyOfPropertyChange(() => allValveOpen);
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
            //curURP_FEDoppler.PCLImgResultID
            //curURP_FEDoppler.DoctorStaffID
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
            curURP_FEDoppler = new UltraResParams_FetalEchocardiographyDoppler();
            curURP_FEDoppler = tempURP_FEDoppler;
            NotifyOfPropertyChange(() => curURP_FEDoppler);
        }

        public void butSave()
        {
            if (PatientPCLReqID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0301_G1_Msg_InfoChonBN);
            }

            if (validationSummary.DisplayedErrors.Count > 0)
            {
                MessageBox.Show(eHCMSResources.A0559_G1_Msg_InfoGTriKhHopLe);
                return;
            }

            if(curURP_FEDoppler.UltraResParams_FetalEchocardiographyDopplerID >0)
            {
                UpdateUltraResParams_FetalEchocardiographyDoppler(curURP_FEDoppler);
            }
            else
            {
                curURP_FEDoppler.PCLImgResultID = PatientPCLReqID;
                curURP_FEDoppler.DoctorStaffID = Globals.LoggedUserAccount.Staff.StaffID;
                AddUltraResParams_FetalEchocardiographyDoppler(curURP_FEDoppler);
            }
            
        }

        public void butUpdate()
        {
            if (validationSummary.DisplayedErrors.Count > 0)
            {
                MessageBox.Show(eHCMSResources.A0559_G1_Msg_InfoGTriKhHopLe);
                return;
            }
            UpdateUltraResParams_FetalEchocardiographyDoppler(curURP_FEDoppler);
        }

        public void ValidationSummary_Loaded(object sender, RoutedEventArgs e)
        {
            validationSummary = sender as ValidationSummary;
        }

        #endregion

        #region method

        private void AddUltraResParams_FetalEchocardiographyDoppler(UltraResParams_FetalEchocardiographyDoppler entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginAddUltraResParams_FetalEchocardiographyDoppler(entity,Globals.DispatchCallback((asyncResult)=>
                                                                                                                 {
                                                                                                                     try
                                                                                                                     {
                                                                                                                         bool res=contract.EndAddUltraResParams_FetalEchocardiographyDoppler(asyncResult);
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
                                                                                                                 }),
                                                                                                        null);
                                       }
                                   });

            t.Start();
        }

        private void UpdateUltraResParams_FetalEchocardiographyDoppler(
            UltraResParams_FetalEchocardiographyDoppler entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginUpdateUltraResParams_FetalEchocardiographyDoppler(entity,
                                                                                                           Globals.
                                                                                                               DispatchCallback
                                                                                                               ((
                                                                                                                   asyncResult)
                                                                                                                =>
                                                                                                                    {
                                                                                                                        try
                                                                                                                        {
                                                                                                                            bool res=contract.EndUpdateUltraResParams_FetalEchocardiographyDoppler(asyncResult);
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
                                                                                                                    }),
                                                                                                           null);
                                       }
                                   });

            t.Start();
        }

        private void GetUltraResParams_FetalEchocardiographyDopplerByID(long UltraResParams_FetalEchocardiographyDopplerID, long PCLImgResultID)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginGetUltraResParams_FetalEchocardiographyDopplerByID(UltraResParams_FetalEchocardiographyDopplerID, PCLImgResultID, Globals.DispatchCallback((asyncResult) =>
                                                                                              {
                                                                                                  try
                                                                                                  {
                                                                                                      var item=contract.EndGetUltraResParams_FetalEchocardiographyDopplerByID(asyncResult);
                                                                                                      if(item!=null)
                                                                                                      {
                                                                                                          curURP_FEDoppler = item;
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

        private void GetV_ValveOpen()
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new CommonService_V2Client())
                                       {
                                           var contract = serviceFactory.ServiceInstance;
                                           contract.BeginGetAllLookupValuesByType(LookupValues.V_ValveOpen
                                                                                  ,
                                                                                  Globals.DispatchCallback(
                                                                                      (asyncResult) =>
                                                                                          {
                                                                                              try
                                                                                              {
                                                                                                  IList<Lookup> results
                                                                                                      =
                                                                                                      contract.
                                                                                                          EndGetAllLookupValuesByType
                                                                                                          (asyncResult);
                                                                                                  if (results != null)
                                                                                                  {
                                                                                                      if (
                                                                                                          allValveOpen ==
                                                                                                          null)
                                                                                                      {
                                                                                                          allValveOpen =
                                                                                                              new ObservableCollection
                                                                                                                  <
                                                                                                                      Lookup
                                                                                                                      >();
                                                                                                      }
                                                                                                      else
                                                                                                      {
                                                                                                          allValveOpen.
                                                                                                              Clear();
                                                                                                      }
                                                                                                      foreach (
                                                                                                          Lookup p in
                                                                                                              results)
                                                                                                      {
                                                                                                          allValveOpen.
                                                                                                              Add(p);
                                                                                                      }
                                                                                                      NotifyOfPropertyChange
                                                                                                          (() =>
                                                                                                           allValveOpen);
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
            //_tempURP_FEDoppler = new UltraResParams_FetalEchocardiographyDoppler();
            //==== 20161129 CMN Begin: Add button save for all pages
            //curURP_FEDoppler = new UltraResParams_FetalEchocardiographyDoppler();
            //GetUltraResParams_FetalEchocardiographyDopplerByID(0, PatientPCLReqID);
            //==== 20161129 CMN End: Add button save for all pages
        }

        //==== 20161129 CMN Begin: Add button save for all pages
        public void SetResultDetails(object resDetails)
        {
            curURP_FEDoppler = (UltraResParams_FetalEchocardiographyDoppler)resDetails;
        }
        //==== 20161129 CMN End: Add button save for all pages
    }
}