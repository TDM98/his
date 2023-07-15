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
using ResourcesManagementProxy;
using aEMR.Controls;
using Castle.Windsor;
/*
 * #001: 20180904 TTM:  Thay đổi cách lấy danh sách Staff theo Category từ chạy đi chạy về database -> lấy từ Globals.
 *                      Vì danh sách Staff đã được lấy lên lúc log-in không có lý do gì phải lấy lại từng lần theo Category như vậy.
 */
namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof (ISieuAmTT_KetLuan)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SieuAmTT_KetLuanViewModel : Conductor<object>, ISieuAmTT_KetLuan
        
         , IHandle<ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest>>
    {
        [ImportingConstructor]
        public SieuAmTT_KetLuanViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Globals.EventAggregator.Subscribe(this);
            //▼====== #001
            GetStaffCategoriesByType();
            //▲====== #001
            //GetStaffCategoriesByType((long)AllLookupValues.StaffCatType.BAC_SI);
            GetPCLExamResultTemplateListByTypeIDDeNghi(5, (int)AllLookupValues.PCLResultParamImpID.SIEUAM_TIMTHAI);
            GetPCLExamResultTemplateListByTypeID(4, (int)AllLookupValues.PCLResultParamImpID.SIEUAM_TIMTHAI);

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

        private ObservableCollection<RefStaffCategory> _allRefStaffCategory;
        private ObservableCollection<Staff> _allStaff;
        private PCLExamResultTemplate _curPCLExamResultTemplate;
        private PCLExamResultTemplate _curPCLExamResultTemplateDeNghi;
        private UltraResParams_FetalEchocardiographyResult _curURP_FEResult;
        private bool _isSave;
        private bool _isUpdate;
        private ObservableCollection<PCLExamResultTemplate> _lstPCLExamResultTemplate;
        private ObservableCollection<PCLExamResultTemplate> _lstPCLExamResultTemplateDeNghi;

        private UltraResParams_FetalEchocardiographyResult _tempURP_FEResult;
        private bool isHasPatient;

        public UltraResParams_FetalEchocardiographyResult curURP_FEResult
        {
            get { return _curURP_FEResult; }
            set
            {
                if (_curURP_FEResult == value)
                    return;
                _curURP_FEResult = value;
                NotifyOfPropertyChange(() => curURP_FEResult);
            }
        }

        public UltraResParams_FetalEchocardiographyResult tempURP_FEResult
        {
            get { return _tempURP_FEResult; }
            set
            {
                if (_tempURP_FEResult == value)
                    return;
                _tempURP_FEResult = value;
                NotifyOfPropertyChange(() => tempURP_FEResult);
            }
        }

        public ObservableCollection<RefStaffCategory> allRefStaffCategory
        {
            get { return _allRefStaffCategory; }
            set
            {
                if (_allRefStaffCategory == value)
                    return;
                _allRefStaffCategory = value;
                NotifyOfPropertyChange(() => allRefStaffCategory);
            }
        }

        public ObservableCollection<Staff> allStaff
        {
            get { return _allStaff; }
            set
            {
                if (_allStaff == value)
                    return;
                _allStaff = value;
                NotifyOfPropertyChange(() => allStaff);
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

        public ObservableCollection<PCLExamResultTemplate> lstPCLExamResultTemplateDeNghi
        {
            get { return _lstPCLExamResultTemplateDeNghi; }
            set
            {
                if (_lstPCLExamResultTemplateDeNghi == value)
                    return;
                _lstPCLExamResultTemplateDeNghi = value;
                NotifyOfPropertyChange(() => lstPCLExamResultTemplateDeNghi);
            }
        }

        public PCLExamResultTemplate curPCLExamResultTemplateDeNghi
        {
            get { return _curPCLExamResultTemplateDeNghi; }
            set
            {
                if (_curPCLExamResultTemplateDeNghi == value)
                    return;
                _curPCLExamResultTemplateDeNghi = value;
                NotifyOfPropertyChange(() => curPCLExamResultTemplateDeNghi);
            }
        }


        //curURP_FEResult.UltraResParams_FetalEchocardiographyResultID
        //curURP_FEResult.PCLImgResultID
        //curURP_FEResult.DoctorStaffID

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
            curURP_FEResult = new UltraResParams_FetalEchocardiographyResult();
            curURP_FEResult = tempURP_FEResult;
            NotifyOfPropertyChange(() => curURP_FEResult);
        }

        public void butSave()
        {
            if (PatientPCLReqID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0301_G1_Msg_InfoChonBN);
                return; 
            }

                if(curURP_FEResult.UltraResParams_FetalEchocardiographyResultID >0)
                {
                    UpdateUltraResParams_FetalEchocardiographyResult(curURP_FEResult);
                }
                else
                {
                    curURP_FEResult.PCLImgResultID = PatientPCLReqID;

                    AddUltraResParams_FetalEchocardiographyResult(curURP_FEResult);
                }

        }

        public void butUpdate()
        {
            
        }

        public void cboVTemplate_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (curURP_FEResult == null)
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

        public void cboVTemplateDeNghi_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (curURP_FEResult == null)
                return;
            if (((AxComboBox) sender).SelectedItem == null)
            {
                return;
            }
            curPCLExamResultTemplateDeNghi = new PCLExamResultTemplate();
            curPCLExamResultTemplateDeNghi.PCLExamResultTemplateID =
                ((PCLExamResultTemplate) ((AxComboBox) sender).SelectedItem).PCLExamResultTemplateID;
            GetPCLExamResultTemplateDeNghi(curPCLExamResultTemplateDeNghi.PCLExamResultTemplateID);
        }

        #endregion

        #region method

        private void AddUltraResParams_FetalEchocardiographyResult(UltraResParams_FetalEchocardiographyResult entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginAddUltraResParams_FetalEchocardiographyResult(entity,Globals.DispatchCallback((asyncResult)=>
                                                                                                                {
                                                                                                                    try
                                                                                                                    {
                                                                                                                        bool res=contract.EndAddUltraResParams_FetalEchocardiographyResult(asyncResult);
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

        private void UpdateUltraResParams_FetalEchocardiographyResult(UltraResParams_FetalEchocardiographyResult entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginUpdateUltraResParams_FetalEchocardiographyResult(entity,
                                                                                                          Globals.
                                                                                                              DispatchCallback
                                                                                                              ((
                                                                                                                  asyncResult)
                                                                                                               =>
                                                                                                                   {
                                                                                                                       try
                                                                                                                       {
                                                                                                                           bool res=contract.EndUpdateUltraResParams_FetalEchocardiographyResult(asyncResult);
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

        private void GetUltraResParams_FetalEchocardiographyResult(long UltraResParams_FetalEchocardiographyResultID, long PCLImgResultID)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginGetUltraResParams_FetalEchocardiographyResultByID(UltraResParams_FetalEchocardiographyResultID, PCLImgResultID, Globals.DispatchCallback((asyncResult) =>
                                                                                              {
                                                                                                  try
                                                                                                  {
                                                                                                      var item = contract.EndGetUltraResParams_FetalEchocardiographyResultByID(asyncResult);
                                                                                                      if(item!=null)
                                                                                                      {
                                                                                                          curURP_FEResult =item;
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



        private void GetPCLExamResultTemplateListByTypeIDDeNghi(long PCLExamGroupTemplateResultID, int ParamEnum)
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
                                                                                              lstPCLExamResultTemplateDeNghi ==
                                                                                              null)
                                                                                          {
                                                                                              lstPCLExamResultTemplateDeNghi
                                                                                                  =
                                                                                                  new ObservableCollection
                                                                                                      <
                                                                                                          PCLExamResultTemplate
                                                                                                          >();
                                                                                          }
                                                                                          else
                                                                                          {
                                                                                              lstPCLExamResultTemplateDeNghi
                                                                                                  .Clear();
                                                                                          }
                                                                                          foreach (
                                                                                              PCLExamResultTemplate p in
                                                                                                  results)
                                                                                          {
                                                                                              lstPCLExamResultTemplateDeNghi
                                                                                                  .Add(p);
                                                                                          }
                                                                                          NotifyOfPropertyChange(
                                                                                              () =>
                                                                                              lstPCLExamResultTemplateDeNghi);
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
                                                                                                  curURP_FEResult.
                                                                                                      CardialAbnormalDetail
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

        private void GetPCLExamResultTemplateDeNghi(long PCLExamGroupTemplateResultID)
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
                                                                                                  curPCLExamResultTemplateDeNghi
                                                                                                      =
                                                                                                      contract.
                                                                                                          EndGetPCLExamResultTemplate
                                                                                                          (asyncResult);
                                                                                                  curURP_FEResult.
                                                                                                      Susgest =
                                                                                                      curPCLExamResultTemplateDeNghi
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
           
            //CheckHasPCLImageID();
            //CheckSave();
            //_tempURP_FEResult = new UltraResParams_FetalEchocardiographyResult();
            //==== 20161129 CMN Begin: Add button save for all pages
            //curURP_FEResult = new UltraResParams_FetalEchocardiographyResult();
            //GetUltraResParams_FetalEchocardiographyResult(0, PatientPCLReqID);
            //==== 20161129 CMN End: Add button save for all pages
        }
        //==== 20161129 CMN Begin: Add button save for all pages
        public void SetResultDetails(object resDetails)
        {
            curURP_FEResult = (UltraResParams_FetalEchocardiographyResult)resDetails;
        }
        //==== 20161129 CMN End: Add button save for all pages

        //▼====== #001
        private void GetStaffCategoriesByType()
        {
            allStaff = new ObservableCollection<Staff>();
            foreach (var item in Globals.AllStaffs)
            {
                if (item.StaffCatgID == (long)StaffCatg.Bs ||
                    item.StaffCatgID == (long)StaffCatg.BsDongY ||
                    item.StaffCatgID == (long)StaffCatg.BsRHM ||
                    item.StaffCatgID == (long)StaffCatg.BsSan ||
                    item.StaffCatgID == (long)StaffCatg.BsTMH ||
                    item.StaffCatgID == (long)StaffCatg.ThS_BS ||
                    item.StaffCatgID == (long)StaffCatg.TS_BS)
                {
                    allStaff.Add(item);
                }
            }
            NotifyOfPropertyChange(() => allStaff);
        }
    }
    //private void GetStaffCategoriesByType(long V_StaffCatType)
    //{
    //    Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
    //    var t = new Thread(() =>
    //                           {
    //                               using (var serviceFactory = new ResourcesManagementServiceClient())
    //                               {
    //                                   IResourcesManagementService contract = serviceFactory.ServiceInstance;
    //                                   contract.BeginGetRefStaffCategoriesByType(V_StaffCatType,
    //                                                                             Globals.DispatchCallback(
    //                                                                                 (asyncResult) =>
    //                                                                                     {
    //                                                                                         try
    //                                                                                         {
    //                                                                                             List
    //                                                                                                 <
    //                                                                                                     RefStaffCategory
    //                                                                                                     > results =
    //                                                                                                         contract
    //                                                                                                             .
    //                                                                                                             EndGetRefStaffCategoriesByType
    //                                                                                                             (asyncResult);
    //                                                                                             if (results != null &&
    //                                                                                                 results.Count >
    //                                                                                                 0)
    //                                                                                             {
    //                                                                                                 allStaff =
    //                                                                                                     new ObservableCollection
    //                                                                                                         <Staff>
    //                                                                                                         ();
    //                                                                                                 foreach (
    //                                                                                                     RefStaffCategory
    //                                                                                                         p in
    //                                                                                                         results
    //                                                                                                     )
    //                                                                                                 {
    //                                                                                                     GetAllStaff
    //                                                                                                         (p.
    //                                                                                                              StaffCatgID);
    //                                                                                                 }
    //                                                                                                 NotifyOfPropertyChange
    //                                                                                                     (() =>
    //                                                                                                      allStaff);
    //                                                                                             }
    //                                                                                         }
    //                                                                                         catch (Exception ex)
    //                                                                                         {
    //                                                                                             MessageBox.Show(ex.Message);
    //                                                                                         }
    //                                                                                         finally
    //                                                                                         {
    //                                                                                             Globals.IsBusy =
    //                                                                                                 false;
    //                                                                                         }
    //                                                                                     }), null);
    //                               }
    //                           });

    //    t.Start();
    //}

    //private void GetAllStaff(long StaffCatgID)
    //{
    //    Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
    //    var t = new Thread(() =>
    //                           {
    //                               using (var serviceFactory = new ResourcesManagementServiceClient())
    //                               {
    //                                   IResourcesManagementService contract = serviceFactory.ServiceInstance;
    //                                   contract.BeginGetAllStaff(StaffCatgID,
    //                                                             Globals.DispatchCallback((asyncResult) =>
    //                                                                                          {
    //                                                                                              try
    //                                                                                              {
    //                                                                                                  List<Staff>
    //                                                                                                      results =
    //                                                                                                          contract
    //                                                                                                              .
    //                                                                                                              EndGetAllStaff
    //                                                                                                              (asyncResult);
    //                                                                                                  if (results !=
    //                                                                                                      null &&
    //                                                                                                      results.
    //                                                                                                          Count >
    //                                                                                                      0)
    //                                                                                                  {
    //                                                                                                      foreach (
    //                                                                                                          Staff
    //                                                                                                              p
    //                                                                                                              in
    //                                                                                                              results
    //                                                                                                          )
    //                                                                                                      {
    //                                                                                                          allStaff
    //                                                                                                              .
    //                                                                                                              Add
    //                                                                                                              (p);
    //                                                                                                      }
    //                                                                                                      NotifyOfPropertyChange
    //                                                                                                          (() =>
    //                                                                                                           allStaff);
    //                                                                                                  }
    //                                                                                              }
    //                                                                                              catch (Exception ex)
    //                                                                                              {
    //                                                                                                  MessageBox.Show(ex.Message);
    //                                                                                              }
    //                                                                                              finally
    //                                                                                              {
    //                                                                                                  Globals.IsBusy= false;
    //                                                                                              }
    //                                                                                          }), null);
    //                               }
    //                           });

    //    t.Start();
    //}
    //▲====== #001
}