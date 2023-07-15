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
    [Export(typeof (ISAMMDongMachCanh)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SAMMDongMachCanhViewModel : Conductor<object>, ISAMMDongMachCanh
        , IHandle<ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest>>
        , IHandle<SaveEventWithKey<String, Nullable<bool>>>
    {
        [ImportingConstructor]
        public SAMMDongMachCanhViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Globals.EventAggregator.Subscribe(this);
            GetPCLExamResultTemplateListByTypeID(1, (int)AllLookupValues.PCLResultParamImpID.SIEUAM_MACHMAU);

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
        private URP_FE_VasculaireCarotid _curURP_FE_VasculaireCarotid;
        private URP_FE_VasculaireExam _curURP_FE_VasculaireExam;
        private bool _isSave;
        private bool _isUpdate;
        private ObservableCollection<PCLExamResultTemplate> _lstPCLExamResultTemplate;

        private URP_FE_VasculaireCarotid _tempURP_FE_VasculaireCarotid;
        private URP_FE_VasculaireExam _tempURP_FE_VasculaireExam;
        private bool isHasPatient;
        private bool validate = true;

        public URP_FE_VasculaireCarotid curURP_FE_VasculaireCarotid
        {
            get { return _curURP_FE_VasculaireCarotid; }
            set
            {
                if (_curURP_FE_VasculaireCarotid == value)
                    return;
                _curURP_FE_VasculaireCarotid = value;
                NotifyOfPropertyChange(() => curURP_FE_VasculaireCarotid);
            }
        }

        public URP_FE_VasculaireCarotid tempURP_FE_VasculaireCarotid
        {
            get { return _tempURP_FE_VasculaireCarotid; }
            set
            {
                if (_tempURP_FE_VasculaireCarotid == value)
                    return;
                _tempURP_FE_VasculaireCarotid = value;
                NotifyOfPropertyChange(() => tempURP_FE_VasculaireCarotid);
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

        public URP_FE_VasculaireExam tempURP_FE_VasculaireExam
        {
            get { return _tempURP_FE_VasculaireExam; }
            set
            {
                if (_tempURP_FE_VasculaireExam == value)
                    return;
                _tempURP_FE_VasculaireExam = value;
                NotifyOfPropertyChange(() => tempURP_FE_VasculaireExam);
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
        //    //curURP_FE_VasculaireCarotid.PCLImgResultID
        //    //curURP_FE_VasculaireCarotid.DoctorStaffID
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

        //public void butReset()
        //{
        //    curURP_FE_VasculaireCarotid = new URP_FE_VasculaireCarotid();
        //    curURP_FE_VasculaireCarotid = tempURP_FE_VasculaireCarotid;
        //    NotifyOfPropertyChange(() => curURP_FE_VasculaireCarotid);
        //}

        public bool Validate(URP_FE_VasculaireCarotid curURP_FE_VasculaireCarotid)
        {
            if (validate == false)
            {
                validate = true;
                return false;
            }

            return true;
        }

        public bool Validate(URP_FE_VasculaireExam curURP_FE_VasculaireExam)
        {
            bool isValid = curURP_FE_VasculaireExam.Validate();
            if (!isValid)
            {
                return isValid;
            }
            return true;
        }

        private Nullable<bool> SaveCarotid;
        private Nullable<bool> SaveExam;
        public void butSave()
        {
            SaveCarotid = null;
            SaveExam = null;

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

            if (curURP_FE_VasculaireCarotid.URP_FE_VasculaireCarotidID>0)
            {
                UpdateURP_FE_VasculaireCarotid(curURP_FE_VasculaireCarotid);
            }
            else
            {
                curURP_FE_VasculaireCarotid.PCLImgResultID = PatientPCLReqID;
                curURP_FE_VasculaireCarotid.DoctorStaffID = Globals.LoggedUserAccount.Staff.StaffID;
                AddURP_FE_VasculaireCarotid(curURP_FE_VasculaireCarotid);
            }

            if(curURP_FE_VasculaireExam.URP_FE_VasculaireExamID>0)
            {
                UpdateURP_FE_VasculaireExam(curURP_FE_VasculaireExam);
            }
            else
            {
                curURP_FE_VasculaireExam.PCLImgResultID = PatientPCLReqID;
                curURP_FE_VasculaireExam.DoctorStaffID = Globals.LoggedUserAccount.Staff.StaffID;
                AddURP_FE_VasculaireExam(curURP_FE_VasculaireExam);
            }
           
        }

        //public void butUpdate()
        //{
        //    if (validationSummary.DisplayedErrors.Count > 0)
        //    {
        //        MessageBox.Show(eHCMSResources.A0559_G1_Msg_InfoGTriKhHopLe);
        //        return;
        //    }

            
        //    if (curURP_FE_VasculaireExam.PCLImgResultID == 0)
        //    {
        //        curURP_FE_VasculaireExam.PCLImgResultID = curURP_FE_VasculaireCarotid.PCLImgResultID;
        //        AddURP_FE_VasculaireExam(curURP_FE_VasculaireExam);
        //    }
        //    else
        //    {
            
        //    }
        //}

        public void cboVTemplate_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (curURP_FE_VasculaireCarotid == null)
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

        private void AddURP_FE_VasculaireCarotid(URP_FE_VasculaireCarotid entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginAddURP_FE_VasculaireCarotid(entity,Globals.DispatchCallback((asyncResult) =>
                                                                                             {
                                                                                                 try
                                                                                                 {
                                                                                                     bool res =contract.EndAddURP_FE_VasculaireCarotid(asyncResult);
                                                                                                     if (res)
                                                                                                     {
                                                                                                         //MessageBox.Show(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK);
                                                                                                         SaveCarotid = true;
                                                                                                     }
                                                                                                     else
                                                                                                     {
                                                                                                         //MessageBox.Show("Thêm mới thất bại!");
                                                                                                         SaveCarotid = false;
                                                                                                     }
                                                                                                     Globals.EventAggregator.Publish(new SaveEventWithKey<String,Nullable<Boolean>>() { Key = "SaveCarotid-SAMachMau", Result = SaveCarotid.Value });
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

        private void UpdateURP_FE_VasculaireCarotid(URP_FE_VasculaireCarotid entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginUpdateURP_FE_VasculaireCarotid(entity,Globals.DispatchCallback((asyncResult) =>
                                                                                                {
                                                                                                    try
                                                                                                    {
                                                                                                        bool res =contract.EndUpdateURP_FE_VasculaireCarotid(asyncResult);
                                                                                                        if (res)
                                                                                                        {
                                                                                                            //MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK);
                                                                                                            SaveCarotid = true;
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            //MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail);
                                                                                                            SaveCarotid = false;
                                                                                                        }
                                                                                                        
                                                                                                        Globals.EventAggregator.Publish(new SaveEventWithKey<String,Nullable<Boolean>>() { Key = "SaveCarotid-SAMachMau", Result = SaveCarotid.Value });
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
        
        private void AddURP_FE_VasculaireExam(URP_FE_VasculaireExam entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginAddURP_FE_VasculaireExam(entity,Globals.DispatchCallback((asyncResult) =>
                                                                                          {
                                                                                              try
                                                                                              {
                                                                                                  bool res =contract.EndAddURP_FE_VasculaireExam(asyncResult);
                                                                                                  if (res)
                                                                                                  {
                                                                                                      //MessageBox.Show(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK);
                                                                                                      SaveExam = true;
                                                                                                  }
                                                                                                  else
                                                                                                  {
                                                                                                      //MessageBox.Show("Thêm mới thất bại!");
                                                                                                      SaveExam = false;
                                                                                                  }
                                                                                                  Globals.EventAggregator.Publish(new SaveEventWithKey<String,Nullable<Boolean>>() { Key = "SaveExam-SAMachMau", Result = SaveExam.Value });
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

        private void UpdateURP_FE_VasculaireExam(URP_FE_VasculaireExam entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginUpdateURP_FE_VasculaireExam(entity,
                                                                                     Globals.DispatchCallback(
                                                                                         (asyncResult) =>
                                                                                             {
                                                                                                 try
                                                                                                 {
                                                                                                     bool res =contract.EndUpdateURP_FE_VasculaireExam(asyncResult);
                                                                                                     if (res)
                                                                                                     {
                                                                                                         //MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK);
                                                                                                         SaveExam = true;
                                                                                                     }
                                                                                                     else
                                                                                                     {
                                                                                                         //MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail);
                                                                                                         SaveExam = false;
                                                                                                     }
                                                                                                     Globals.EventAggregator.Publish(new SaveEventWithKey<String,Nullable<Boolean>>() { Key = "SaveExam-SAMachMau", Result = SaveExam });
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

        private void GetURP_FE_VasculaireCarotidByID(long URP_FE_VasculaireCarotidID, long PCLImgResultID)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginGetURP_FE_VasculaireCarotid(URP_FE_VasculaireCarotidID, PCLImgResultID,Globals.DispatchCallback((asyncResult) =>
                                                                                             {
                                                                                                 try
                                                                                                 {
                                                                                                     var item=contract.EndGetURP_FE_VasculaireCarotid(asyncResult);
                                                                                                     if(item!=null)
                                                                                                     {
                                                                                                         curURP_FE_VasculaireCarotid= item;
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

        private void GetURP_FE_VasculaireExamByID(long URP_FE_VasculaireExamID, long PCLImgResultID)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginGetURP_FE_VasculaireExam(URP_FE_VasculaireExamID, PCLImgResultID,Globals.DispatchCallback((asyncResult) =>
                                                                                          {
                                                                                              try
                                                                                              {
                                                                                                  var item=contract.EndGetURP_FE_VasculaireExam(asyncResult);
                                                                                                  if(item!=null)
                                                                                                  {
                                                                                                      curURP_FE_VasculaireExam= item;
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

        private void GetV_ValveOpen()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>{
                using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllLookupValuesByType(LookupValues.V_ValveOpen,Globals.DispatchCallback((asyncResult) =>
                        {
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
                                    NotifyOfPropertyChange(() =>allValveOpen);
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message,eHCMSResources.T0432_G1_Error);
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
                                                                                                  curPCLExamResultTemplate=contract.EndGetPCLExamResultTemplate(asyncResult);
                                                                                                  curURP_FE_VasculaireCarotid.KetLuan =curPCLExamResultTemplate.ResultContent;
                                                                                                  NotifyOfPropertyChange(() =>lstPCLExamResultTemplate);
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
            //_tempURP_FE_VasculaireCarotid = new URP_FE_VasculaireCarotid();
            //_tempURP_FE_VasculaireExam = new URP_FE_VasculaireExam();
            //==== 20161129 CMN Begin: Add button save for all pages
            //curURP_FE_VasculaireCarotid = new URP_FE_VasculaireCarotid();
            //curURP_FE_VasculaireExam = new URP_FE_VasculaireExam();
            //GetURP_FE_VasculaireCarotidByID(0, PatientPCLReqID);
            //GetURP_FE_VasculaireExamByID(0, PatientPCLReqID);
            //==== 20161129 CMN End: Add button save for all pages
        }


        public void Handle(SaveEventWithKey<string, bool?> message)
        {
            if(message!=null)
            {
                if (message.Key == "SaveCarotid-SAMachMau")
                {
                    if(SaveExam!=null && SaveCarotid!=null)
                    {
                        if(SaveExam==true && SaveCarotid ==true)
                        {
                            MessageBox.Show(eHCMSResources.Z1562_G1_DaLuu);
                        }
                        else
                        {
                            MessageBox.Show("Lưu Không Thành Công!");
                        }
                    }
                }
                if (message.Key == "SaveExam-SAMachMau")
                {
                    if (SaveExam != null && SaveCarotid != null)
                    {
                        if (SaveExam == true && SaveCarotid == true)
                        {
                            MessageBox.Show(eHCMSResources.Z1562_G1_DaLuu);
                        }
                        else
                        {
                            MessageBox.Show("Lưu Không Thành Công!");
                        }
                    }
                }
            }
        }

        //==== 20161129 CMN Begin: Add button save for all pages
        public void SetResultDetails(object resDetails1, object resDetails2)
        {
            curURP_FE_VasculaireCarotid = (URP_FE_VasculaireCarotid)resDetails1;
            curURP_FE_VasculaireExam = (URP_FE_VasculaireExam)resDetails2;
        }
        //==== 20161129 CMN End: Add button save for all pages
    }
}