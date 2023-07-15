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
    [Export(typeof (ISATGSDipyBenhSu)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SATGSDipyBenhSuViewModel : Conductor<object>, ISATGSDipyBenhSu
         , IHandle<ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest>>
        , IHandle<SaveEventWithKey<String, Nullable<bool>>>
    {
        [ImportingConstructor]
        public SATGSDipyBenhSuViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
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

        private ObservableCollection<Lookup> _allValveOpen;
        private URP_FE_Exam _curURP_FE_Exam;
        private URP_FE_StressDipyridamoleExam _curURP_FE_StressDipyridamoleExam;
        private bool _isSave;
        private bool _isUpdate;
        private URP_FE_Exam _tempURP_FE_Exam;

        private URP_FE_StressDipyridamoleExam _tempURP_FE_StressDipyridamoleExam;
        private bool isHasPatient;

        public URP_FE_StressDipyridamoleExam curURP_FE_StressDipyridamoleExam
        {
            get { return _curURP_FE_StressDipyridamoleExam; }
            set
            {
                if (_curURP_FE_StressDipyridamoleExam == value)
                    return;
                _curURP_FE_StressDipyridamoleExam = value;
                NotifyOfPropertyChange(() => curURP_FE_StressDipyridamoleExam);
            }
        }

        public URP_FE_StressDipyridamoleExam tempURP_FE_StressDipyridamoleExam
        {
            get { return _tempURP_FE_StressDipyridamoleExam; }
            set
            {
                if (_tempURP_FE_StressDipyridamoleExam == value)
                    return;
                _tempURP_FE_StressDipyridamoleExam = value;
                NotifyOfPropertyChange(() => tempURP_FE_StressDipyridamoleExam);
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

        public URP_FE_Exam tempURP_FE_Exam
        {
            get { return _tempURP_FE_Exam; }
            set
            {
                if (_tempURP_FE_Exam == value)
                    return;
                _tempURP_FE_Exam = value;
                NotifyOfPropertyChange(() => tempURP_FE_Exam);
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
            //curURP_FE_StressDipyridamoleExam.PCLImgResultID
            //curURP_FE_StressDipyridamoleExam.DoctorStaffID
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
            curURP_FE_StressDipyridamoleExam = new URP_FE_StressDipyridamoleExam();
            curURP_FE_StressDipyridamoleExam = tempURP_FE_StressDipyridamoleExam;
            curURP_FE_Exam = new URP_FE_Exam();
            tempURP_FE_Exam = new URP_FE_Exam();

            NotifyOfPropertyChange(() => curURP_FE_StressDipyridamoleExam);
        }

        private Nullable<bool> SaveStressDipyridamole;
        private Nullable<bool> SaveExam;

        public void butSave()
        {
            SaveStressDipyridamole = null;
            SaveExam = null;

            if (PatientPCLReqID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0301_G1_Msg_InfoChonBN);
                return; 
            }


             if (validationSummary.DisplayedErrors.Count > 0)
             {
                 MessageBox.Show(eHCMSResources.A0559_G1_Msg_InfoGTriKhHopLe) ;
                 return;
             }


            if(curURP_FE_StressDipyridamoleExam.URP_FE_StressDipyridamoleExamID>0)
            {
                 UpdateURP_FE_StressDipyridamoleExam(curURP_FE_StressDipyridamoleExam);
            }
            else
            {
                curURP_FE_StressDipyridamoleExam.PCLImgResultID = PatientPCLReqID;
                curURP_FE_StressDipyridamoleExam.DoctorStaffID = Globals.LoggedUserAccount.Staff.StaffID;
                AddURP_FE_StressDipyridamoleExam(curURP_FE_StressDipyridamoleExam);
            }


            if(curURP_FE_Exam.URP_FE_ExamID >0)
            {
                UpdateURP_FE_Exam(curURP_FE_Exam);
            }
            else
            {
                curURP_FE_Exam.PCLImgResultID = PatientPCLReqID;
                curURP_FE_Exam.DoctorStaffID = Globals.LoggedUserAccount.Staff.StaffID;
                AddURP_FE_Exam(curURP_FE_Exam);
            }
        }

        //public void butUpdate()
        //{
        //    if (validationSummary.DisplayedErrors.Count > 0)
        //    {
        //        MessageBox.Show(eHCMSResources.A0559_G1_Msg_InfoGTriKhHopLe);
        //        return;
        //    }
        //    if (curURP_FE_Exam.PCLImgResultID == 0)
        //    {
        //        curURP_FE_Exam.PCLImgResultID = curURP_FE_StressDipyridamoleExam.PCLImgResultID;
        //        AddURP_FE_Exam(curURP_FE_Exam);
        //    }
        //    else
        //    {
        //        UpdateURP_FE_Exam(curURP_FE_Exam);
        //    }

        //    if (curURP_FE_StressDipyridamoleExam.PCLImgResultID == 0)
        //    {
        //        curURP_FE_StressDipyridamoleExam.PCLImgResultID = curURP_FE_Exam.PCLImgResultID;
        //        AddURP_FE_StressDipyridamoleExam(curURP_FE_StressDipyridamoleExam);
        //    }
        //    else
        //    {
        //        UpdateURP_FE_StressDipyridamoleExam(curURP_FE_StressDipyridamoleExam);
        //    }
        //}

        public void ValidationSummary_Loaded(object sender, RoutedEventArgs e)
        {
            validationSummary = sender as ValidationSummary;
        }

        #endregion

        #region method

        private void AddURP_FE_StressDipyridamoleExam(URP_FE_StressDipyridamoleExam entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginAddURP_FE_StressDipyridamoleExam(entity,
                                                                                          Globals.DispatchCallback((asyncResult) =>
                                                                                                  {
                                                                                                      try
                                                                                                      {
                                                                                                          bool res =contract.EndAddURP_FE_StressDipyridamoleExam(asyncResult);
                                                                                                          if (res)
                                                                                                          {
                                                                                                              //MessageBox.Show(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK);
                                                                                                              SaveStressDipyridamole = true;
                                                                                                          }
                                                                                                          else
                                                                                                          {
                                                                                                              //MessageBox.Show("Thêm mới thất bại!");
                                                                                                              SaveStressDipyridamole = false;
                                                                                                          }
                                                                                                          Globals.EventAggregator.Publish(new SaveEventWithKey<String, Nullable<Boolean>>() { Key = "SaveStressDipyridamole-Dipyridamole", Result = SaveStressDipyridamole.Value });
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

        private void UpdateURP_FE_StressDipyridamoleExam(URP_FE_StressDipyridamoleExam entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginUpdateURP_FE_StressDipyridamoleExam(entity,
                                                                                             Globals.DispatchCallback(
                                                                                                 (asyncResult) =>
                                                                                                     {
                                                                                                         try
                                                                                                         {
                                                                                                             bool res =contract.EndUpdateURP_FE_StressDipyridamoleExam(asyncResult);
                                                                                                             if (res)
                                                                                                             {
                                                                                                                 //MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK);
                                                                                                                 SaveStressDipyridamole = true;
                                                                                                             }
                                                                                                             else
                                                                                                             {
                                                                                                                 //MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail);
                                                                                                                 SaveStressDipyridamole = false;
                                                                                                             }

                                                                                                             Globals.EventAggregator.Publish(new SaveEventWithKey<String, Nullable<Boolean>>() { Key = "SaveStressDipyridamole-Dipyridamole", Result = SaveStressDipyridamole.Value });
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

        private void AddURP_FE_Exam(URP_FE_Exam entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginAddURP_FE_Exam(entity,
                                                                        Globals.DispatchCallback((asyncResult) =>
                                                                                                     {
                                                                                                         try
                                                                                                         {
                                                                                                             bool res =contract.EndAddURP_FE_Exam(asyncResult);
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
                                                                                                             Globals.EventAggregator.Publish(new SaveEventWithKey<String, Nullable<Boolean>>() { Key = "SaveExam-Dipyridamole", Result = SaveExam.Value });
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

        private void UpdateURP_FE_Exam(URP_FE_Exam entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginUpdateURP_FE_Exam(entity,
                                                                           Globals.DispatchCallback((asyncResult) =>
                                                                                                        {
                                                                                                            try
                                                                                                            {
                                                                                                                bool res=contract.EndUpdateURP_FE_Exam(asyncResult);
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
                                                                                                                Globals.EventAggregator.Publish(new SaveEventWithKey<String, Nullable<Boolean>>() { Key = "SaveExam-Dipyridamole", Result = SaveExam.Value });
                                                                                                            }
                                                                                                            catch (Exception ex)
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


        private void GetURP_FE_StressDipyridamoleExamByID(long URP_FE_StressDipyridamoleExamID, long PCLImgResultID){
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginGetURP_FE_StressDipyridamoleExam(URP_FE_StressDipyridamoleExamID, PCLImgResultID, Globals.DispatchCallback((asyncResult) =>
                                                                                              {
                                                                                                  try
                                                                                                  {
                                                                                                      var item=contract.EndGetURP_FE_StressDipyridamoleExam(asyncResult);
                                                                                                      if(item!=null)
                                                                                                      {
                                                                                                          curURP_FE_StressDipyridamoleExam=item;
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
                                                                                                                 curURP_FE_Exam=item;
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
        private void LoadInfo()
        {
            //GetV_ValveOpen();
            //CheckHasPCLImageID();
            //CheckSave();
            //_tempURP_FE_StressDipyridamoleExam = new URP_FE_StressDipyridamoleExam();
            //curURP_FE_StressDipyridamoleExam = new URP_FE_StressDipyridamoleExam();
            //_tempURP_FE_Exam = new URP_FE_Exam();
            //curURP_FE_Exam = new URP_FE_Exam();

            //GetURP_FE_StressDipyridamoleExamByID(0, PatientPCLReqID);
            //GetURP_FE_ExamByID(0, PatientPCLReqID);

            //NhanApMP
            //NhanApMT
        }

        public void Handle(SaveEventWithKey<string, bool?> message)
        {
            
            if (message != null)
            {
                if (message.Key == "SaveStressDipyridamole-Dipyridamole")
                {
                    if (SaveExam != null && SaveStressDipyridamole!= null)
                    {
                        if (SaveExam == true && SaveStressDipyridamole == true)
                        {
                            MessageBox.Show(eHCMSResources.Z1562_G1_DaLuu);
                        }
                        else
                        {
                            MessageBox.Show("Lưu Không Thành Công!");
                        }
                    }
                }
                if (message.Key == "SaveExam-Dipyridamole")
                {
                    if (SaveExam != null && SaveStressDipyridamole != null)
                    {
                        if (SaveExam == true && SaveStressDipyridamole == true)
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
        //==== 20161201 CMN Begin: Add button save for all pages
        public void SetResultDetails(object resDetails1, object resDetails2)
        {
            curURP_FE_StressDipyridamoleExam = (URP_FE_StressDipyridamoleExam)resDetails1;
            curURP_FE_Exam = (URP_FE_Exam)resDetails2;
        }
        public bool CheckValidate()
        {
            return (validationSummary != null && validationSummary.DisplayedErrors.Count == 0) || validationSummary == null;
        }
        //==== 20161201 CMN End.
    }
}