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
using Caliburn.Micro;
using CommonServiceProxy;
using DataEntities;
using PCLsProxy;
using aEMR.Controls;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof (ISATQuaThucQuanCD)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SATQuaThucQuanCDViewModel : Conductor<object>, ISATQuaThucQuanCD
         , IHandle<ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SATQuaThucQuanCDViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            GetPCLExamResultTemplateListByTypeID(2, (int)AllLookupValues.PCLResultParamImpID.SIEUAM_THUCQUAN);
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

        private PCLExamResultTemplate _curPCLExamResultTemplate;
        private URP_FE_OesophagienneDiagnosis _curURP_FE_OesophagienneDiagnosis;
        private ObservableCollection<PCLExamResultTemplate> _lstPCLExamResultTemplate;


        public URP_FE_OesophagienneDiagnosis curURP_FE_OesophagienneDiagnosis
        {
            get { return _curURP_FE_OesophagienneDiagnosis; }
            set
            {
                if (_curURP_FE_OesophagienneDiagnosis == value)
                    return;
                _curURP_FE_OesophagienneDiagnosis = value;
                NotifyOfPropertyChange(() => curURP_FE_OesophagienneDiagnosis);
            }
        }

        public ObservableCollection<PCLExamResultTemplate> lstPCLExamResultTemplate
        {
            get { return _lstPCLExamResultTemplate; }
            set
            {
                if (_lstPCLExamResultTemplate == value)
                    return;
                _lstPCLExamResultTemplate = value;
                NotifyOfPropertyChange(() => lstPCLExamResultTemplate);
            }
        }

        public PCLExamResultTemplate curPCLExamResultTemplate
        {
            get { return _curPCLExamResultTemplate; }
            set
            {
                if (_curPCLExamResultTemplate == value)
                    return;
                _curPCLExamResultTemplate = value;
                NotifyOfPropertyChange(() => curPCLExamResultTemplate);
            }
        }

        #endregion

        #region fuction

        //public void CheckHasPCLImageID()
        //{
        //    isHasPatient = true;
        //    //if()
        //    //{
        //    //    isHasPatient = true;
        //    //}else
        //    //{
        //    //    isHasPatient = false;
        //    //}
        //    //curURP_FE_OesophagienneDiagnosis.PCLImgResultID
        //    //curURP_FE_OesophagienneDiagnosis.DoctorStaffID
        //}

        //public void CheckSave()
        //{
        //    isSave = true;
        //    isUpdate = false;
        //}

        //public void CheckUpdate()
        //{
        //    isSave = false;
        //    isUpdate = true;
        //}

        public void butSave()
        {
            if (PatientPCLReqID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0301_G1_Msg_InfoChonBN);
                return;
            }

            if (curURP_FE_OesophagienneDiagnosis.URP_FE_OesophagienneDiagnosisID >0)
            {
                UpdateURP_FE_OesophagienneDiagnosis(curURP_FE_OesophagienneDiagnosis);
            }
            else
            {
                curURP_FE_OesophagienneDiagnosis.PCLImgResultID = PatientPCLReqID;
                curURP_FE_OesophagienneDiagnosis.DoctorStaffID = Globals.LoggedUserAccount.Staff.StaffID;
                AddURP_FE_OesophagienneDiagnosis(curURP_FE_OesophagienneDiagnosis);
            }

        }   
        

        public void cboVTemplate_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (curURP_FE_OesophagienneDiagnosis == null)
                return;
            if (((AxComboBox) sender).SelectedItem == null)
            {
                return;
            }
            curPCLExamResultTemplate = new PCLExamResultTemplate();
            curPCLExamResultTemplate.PCLExamResultTemplateID =
                ((PCLExamResultTemplate) ((AxComboBox) sender).SelectedItem).PCLExamResultTemplateID;
            GetPCLExamResultTemplate(curPCLExamResultTemplate.PCLExamResultTemplateID);
        }

        #endregion

        #region method

        private void AddURP_FE_OesophagienneDiagnosis(URP_FE_OesophagienneDiagnosis entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginAddURP_FE_OesophagienneDiagnosis(entity,Globals.DispatchCallback((asyncResult) =>
                                                                                                  {
                                                                                                      try
                                                                                                      {
                                                                                                          bool res =contract.EndAddURP_FE_OesophagienneDiagnosis(asyncResult);
                                                                                                          if (res)
                                                                                                          {
                                                                                                              MessageBox.Show(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK);
                                                                                                          }
                                                                                                          else
                                                                                                          {
                                                                                                              MessageBox.Show("Thêm mới thất bại!");
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

        private void UpdateURP_FE_OesophagienneDiagnosis(URP_FE_OesophagienneDiagnosis entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginUpdateURP_FE_OesophagienneDiagnosis(entity,
                                                                                             Globals.DispatchCallback(
                                                                                                 (asyncResult) =>
                                                                                                     {
                                                                                                         try
                                                                                                         {
                                                                                                             bool res =contract.EndUpdateURP_FE_OesophagienneDiagnosis(asyncResult);
                                                                                                             if (res)
                                                                                                             {
                                                                                                                 MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK);
                                                                                                             }
                                                                                                             else
                                                                                                             {
                                                                                                                 MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail);
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

        private void GetURP_FE_OesophagienneDiagnosisByID(long URP_FE_OesophagienneDiagnosisID
                                                          , long PCLImgResultID)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginGetURP_FE_OesophagienneDiagnosis(URP_FE_OesophagienneDiagnosisID, PCLImgResultID, Globals.DispatchCallback((asyncResult) =>
                                                                                              {
                                                                                                  try
                                                                                                  {
                                                                                                      var item= contract.EndGetURP_FE_OesophagienneDiagnosis(asyncResult);
                                                                                                      if(item!=null)
                                                                                                      {
                                                                                                          curURP_FE_OesophagienneDiagnosis= item;
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

        //private void GetV_ValveOpen()
        //{
        //    Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
        //    var t = new Thread(() =>
        //                           {
        //                               using (var serviceFactory = new CommonServiceClient())
        //                               {
        //                                   ICommonService contract = serviceFactory.ServiceInstance;
        //                                   contract.BeginGetAllLookupValuesByType(LookupValues.V_ValveOpen
        //                                                                          ,
        //                                                                          Globals.DispatchCallback(
        //                                                                              (asyncResult) =>
        //                                                                                  {
        //                                                                                      try
        //                                                                                      {
        //                                                                                          IList<Lookup> results
        //                                                                                              =
        //                                                                                              contract.
        //                                                                                                  EndGetAllLookupValuesByType
        //                                                                                                  (asyncResult);
        //                                                                                          if (results != null)
        //                                                                                          {
        //                                                                                              if (
        //                                                                                                  allValveOpen ==
        //                                                                                                  null)
        //                                                                                              {
        //                                                                                                  allValveOpen =
        //                                                                                                      new ObservableCollection
        //                                                                                                          <
        //                                                                                                              Lookup
        //                                                                                                              >();
        //                                                                                              }
        //                                                                                              else
        //                                                                                              {
        //                                                                                                  allValveOpen.
        //                                                                                                      Clear();
        //                                                                                              }
        //                                                                                              foreach (
        //                                                                                                  Lookup p in
        //                                                                                                      results)
        //                                                                                              {
        //                                                                                                  allValveOpen.
        //                                                                                                      Add(p);
        //                                                                                              }
        //                                                                                              NotifyOfPropertyChange
        //                                                                                                  (() =>
        //                                                                                                   allValveOpen);
        //                                                                                          }
        //                                                                                      }
        //                                                                                      catch (Exception ex)
        //                                                                                      {
        //                                                                                          Globals.ShowMessage(
        //                                                                                              ex.Message,
        //                                                                                              eHCMSResources.T0432_G1_Error);
        //                                                                                      }
        //                                                                                      finally
        //                                                                                      {
        //                                                                                          Globals.IsBusy = false;
        //                                                                                      }
        //                                                                                  }), null);
        //                               }
        //                           });

        //    t.Start();
        //}

        private void GetPCLExamResultTemplateListByTypeID(long PCLExamGroupTemplateResultID, int ParamEnum)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginGetPCLExamResultTemplateListByTypeID(
                                               PCLExamGroupTemplateResultID, ParamEnum
                                               , Globals.DispatchCallback((asyncResult) =>
                                                                              {
                                                                                  try
                                                                                  {
                                                                                      List<PCLExamResultTemplate>
                                                                                          results =
                                                                                              contract.
                                                                                                  EndGetPCLExamResultTemplateListByTypeID
                                                                                                  (asyncResult);
                                                                                      if (results != null)
                                                                                      {
                                                                                          if (
                                                                                              lstPCLExamResultTemplate ==
                                                                                              null)
                                                                                          {
                                                                                              lstPCLExamResultTemplate =
                                                                                                  new ObservableCollection
                                                                                                      <
                                                                                                          PCLExamResultTemplate
                                                                                                          >();
                                                                                          }
                                                                                          else
                                                                                          {
                                                                                              lstPCLExamResultTemplate.
                                                                                                  Clear();
                                                                                          }
                                                                                          foreach (
                                                                                              PCLExamResultTemplate p in
                                                                                                  results)
                                                                                          {
                                                                                              lstPCLExamResultTemplate.
                                                                                                  Add(p);
                                                                                          }
                                                                                          NotifyOfPropertyChange(
                                                                                              () =>
                                                                                              lstPCLExamResultTemplate);
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

        private void GetPCLExamResultTemplate(long PCLExamGroupTemplateResultID)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginGetPCLExamResultTemplate(PCLExamGroupTemplateResultID
                                                                                  ,
                                                                                  Globals.DispatchCallback(
                                                                                      (asyncResult) =>
                                                                                          {
                                                                                              try
                                                                                              {
                                                                                                  curPCLExamResultTemplate
                                                                                                      =
                                                                                                      contract.
                                                                                                          EndGetPCLExamResultTemplate
                                                                                                          (asyncResult);
                                                                                                  curURP_FE_OesophagienneDiagnosis
                                                                                                      .
                                                                                                      ChanDoanQuaThucQuan
                                                                                                      =
                                                                                                      curPCLExamResultTemplate
                                                                                                          .ResultContent;
                                                                                                  NotifyOfPropertyChange
                                                                                                      (() =>
                                                                                                       lstPCLExamResultTemplate);
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
            //GetV_ValveOpen();
            //CheckHasPCLImageID();
            //CheckSave();
            //_tempURP_FE_OesophagienneDiagnosis = new URP_FE_OesophagienneDiagnosis();
            //==== 20161129 CMN Begin: Add button save for all pages
            //curURP_FE_OesophagienneDiagnosis = new URP_FE_OesophagienneDiagnosis();
            //GetURP_FE_OesophagienneDiagnosisByID(0, PatientPCLReqID);
            //==== 20161129 CMN End: Add button save for all pages 
        }
        //==== 20161129 CMN Begin: Add button save for all pages
        public void SetResultDetails(object resDetails)
        {
            curURP_FE_OesophagienneDiagnosis = (URP_FE_OesophagienneDiagnosis)resDetails;
        }
        //==== 20161129 CMN End: Add button save for all pages
    }
}