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
using DataEntities;
using PCLsProxy;
using aEMR.Controls;
using Castle.Windsor;

namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof (ISAMMDongMachChu)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SAMMDongMachChuViewModel : Conductor<object>, ISAMMDongMachChu
        , IHandle<ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest>>
    {
        [ImportingConstructor]
        public SAMMDongMachChuViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Globals.EventAggregator.Subscribe(this);
            GetPCLExamResultTemplateListByTypeID(2, (int)AllLookupValues.PCLResultParamImpID.SIEUAM_MACHMAU);

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
        private PCLExamResultTemplate _curPCLExamResultTemplate;
        private URP_FE_VasculaireAorta _curURP_FE_VasculaireAorta;
        private bool _isSave;
        private bool _isUpdate;
        private ObservableCollection<PCLExamResultTemplate> _lstPCLExamResultTemplate;

        private URP_FE_VasculaireAorta _tempURP_FE_VasculaireAorta;
        private bool isHasPatient;

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

        public URP_FE_VasculaireAorta tempURP_FE_VasculaireAorta
        {
            get { return _tempURP_FE_VasculaireAorta; }
            set
            {
                if (_tempURP_FE_VasculaireAorta == value)
                    return;
                _tempURP_FE_VasculaireAorta = value;
                NotifyOfPropertyChange(() => tempURP_FE_VasculaireAorta);
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
        //    //curURP_FE_VasculaireAorta.PCLImgResultID
        //    //curURP_FE_VasculaireAorta.DoctorStaffID
        //}

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
            curURP_FE_VasculaireAorta = new URP_FE_VasculaireAorta();
            curURP_FE_VasculaireAorta = tempURP_FE_VasculaireAorta;
            NotifyOfPropertyChange(() => curURP_FE_VasculaireAorta);
        }

        public void butSave()
        {
            if (PatientPCLReqID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0301_G1_Msg_InfoChonBN);
                return;
            }

            if (validationSummary.DisplayedErrors.Count > 0)
            {
                MessageBox.Show(eHCMSResources.A0559_G1_Msg_InfoGTriKhHopLe);
                return;
            }

            if (curURP_FE_VasculaireAorta.URP_FE_VasculaireAortaID > 0)
            {
                UpdateURP_FE_VasculaireAorta(curURP_FE_VasculaireAorta);
            }
            else
            {
                curURP_FE_VasculaireAorta.PCLImgResultID = PatientPCLReqID;
                curURP_FE_VasculaireAorta.DoctorStaffID = Globals.LoggedUserAccount.Staff.StaffID;
                AddURP_FE_VasculaireAorta(curURP_FE_VasculaireAorta);
            }
        }

        //public void butUpdate()
        //{
        //    if (validationSummary.DisplayedErrors.Count > 0)
        //    {
        //        MessageBox.Show(eHCMSResources.A0559_G1_Msg_InfoGTriKhHopLe);
        //        return;
        //    }
            
        //}

        public void cboVTemplate_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (curURP_FE_VasculaireAorta == null)
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

        public void ValidationSummary_Loaded(object sender, RoutedEventArgs e)
        {
            validationSummary = sender as ValidationSummary;
        }

        #endregion

        #region method

        private void AddURP_FE_VasculaireAorta(URP_FE_VasculaireAorta entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginAddURP_FE_VasculaireAorta(entity,
                                                                                   Globals.DispatchCallback(
                                                                                       (asyncResult) =>
                                                                                           {
                                                                                               try
                                                                                               {
                                                                                                   bool res =
                                                                                                       contract.
                                                                                                           EndAddURP_FE_VasculaireAorta
                                                                                                           (asyncResult);
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
                                                                                                   Globals.IsBusy =false;
                                                                                               }
                                                                                           }), null);
                                       }
                                   });

            t.Start();
        }

        private void UpdateURP_FE_VasculaireAorta(URP_FE_VasculaireAorta entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginUpdateURP_FE_VasculaireAorta(entity,
                                                                                      Globals.DispatchCallback(
                                                                                          (asyncResult) =>
                                                                                              {
                                                                                                  try
                                                                                                  {
                                                                                                      bool res =contract.EndUpdateURP_FE_VasculaireAorta(asyncResult);
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
                                                                                                      Globals.IsBusy =false;
                                                                                                  }
                                                                                              }), null);
                                       }
                                   });

            t.Start();
        }

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
                                                                                  Globals.DispatchCallback((asyncResult) =>{
                                                                                      try
                                                                                              {
                                                                                                  IList<Lookup> results=contract.EndGetAllLookupValuesByType(asyncResult);
                                                                                                  if (results != null)
                                                                                                  {
                                                                                                      if (allValveOpen ==null)
                                                                                                      {
                                                                                                          allValveOpen =new ObservableCollection<Lookup>();
                                                                                                      }
                                                                                                      else
                                                                                                      {
                                                                                                          allValveOpen.Clear();
                                                                                                      }
                                                                                                      foreach (Lookup p in results)
                                                                                                      {
                                                                                                          allValveOpen.Add(p);
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

        private void GetPCLExamResultTemplateListByTypeID(long PCLExamGroupTemplateResultID, int ParamEnum)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginGetPCLExamResultTemplateListByTypeID(PCLExamGroupTemplateResultID, ParamEnum, Globals.DispatchCallback((asyncResult) =>
                                                                              {
                                                                                  try
                                                                                  {
                                                                                      List<PCLExamResultTemplate>results =contract.EndGetPCLExamResultTemplateListByTypeID(asyncResult);
                                                                                      if (results != null)
                                                                                      {
                                                                                          if (lstPCLExamResultTemplate ==null)
                                                                                          {
                                                                                              lstPCLExamResultTemplate =new ObservableCollection<PCLExamResultTemplate>();
                                                                                          }
                                                                                          else
                                                                                          {
                                                                                              lstPCLExamResultTemplate.Clear();
                                                                                          }
                                                                                          foreach (PCLExamResultTemplate p in results)
                                                                                          {
                                                                                              lstPCLExamResultTemplate.Add(p);
                                                                                          }
                                                                                          NotifyOfPropertyChange(() =>lstPCLExamResultTemplate);
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
                                                                                                  curURP_FE_VasculaireAorta
                                                                                                      .KetLuan =
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

        private void LoadInfo()
        {
            //GetV_ValveOpen();
            //CheckHasPCLImageID();
            //CheckSave();
            //_tempURP_FE_VasculaireAorta = new URP_FE_VasculaireAorta();
            //==== 20161129 CMN Begin: Add button save for all pages
            //curURP_FE_VasculaireAorta = new URP_FE_VasculaireAorta();
            //GetURP_FE_VasculaireAortaByID(0, PatientPCLReqID);
            //==== 20161129 CMN End: Add button save for all pages    
        }
        //==== 20161129 CMN Begin: Add button save for all pages
        public void SetResultDetails(object resDetails)
        {
            curURP_FE_VasculaireAorta = (URP_FE_VasculaireAorta)resDetails;
        }
        //==== 20161129 CMN End: Add button save for all pages
    }
}