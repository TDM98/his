/*
 * #001: 20161215 CMN Begin: Add control for choose doctor and date
 * #002: 20180904 TTM:  Thay đổi cách lấy danh sách Staff theo Category từ chạy đi chạy về database -> lấy từ Globals.
 * Vì danh sách Staff đã được lấy lên lúc log-in không có lý do gì phải lấy lại từng lần theo Category như vậy.
 */
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
using System.Windows.Controls;
using ResourcesManagementProxy;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
/* 
 * #002: 20180904 TTM:  Thay đổi cách lấy danh sách Staff theo Category từ chạy đi chạy về database -> lấy từ Globals.
 * Vì danh sách Staff đã được lấy lên lúc log-in không có lý do gì phải lấy lại từng lần theo Category như vậy.
 */
namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof(ISATQuaThucQuanChanDoan)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SATQuaThucQuanChanDoanViewModel : Conductor<object>, ISATQuaThucQuanChanDoan
        , IHandle<ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SATQuaThucQuanChanDoanViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            //▼====== #002
            GetStaffCategoriesByType();
            //▲====== #002
            //==== #001
            //GetStaffCategoriesByType((long)AllLookupValues.StaffCatType.BAC_SI);
            //==== #001
            GetPCLExamResultTemplateListByTypeIDChiDinh(4, (int)AllLookupValues.PCLResultParamImpID.SIEUAM_THUCQUAN);
            GetPCLExamResultTemplateListByTypeIDThanhNguc(5, (int)AllLookupValues.PCLResultParamImpID.SIEUAM_THUCQUAN);

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
        private PCLExamResultTemplate _curPCLExamResultTemplateChiDinh;
        private URP_FE_Oesophagienne _curURP_FE_Oesophagienne;
        private ObservableCollection<PCLExamResultTemplate> _lstPCLExamResultTemplate;
        private ObservableCollection<PCLExamResultTemplate> _lstPCLExamResultTemplateChiDinh;


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

        public ObservableCollection<PCLExamResultTemplate> lstPCLExamResultTemplateChiDinh
        {
            get { return _lstPCLExamResultTemplateChiDinh; }
            set
            {
                if (_lstPCLExamResultTemplateChiDinh == value)
                    return;
                _lstPCLExamResultTemplateChiDinh = value;
                NotifyOfPropertyChange(() => lstPCLExamResultTemplateChiDinh);
            }
        }

        public PCLExamResultTemplate curPCLExamResultTemplateChiDinh
        {
            get { return _curPCLExamResultTemplateChiDinh; }
            set
            {
                if (_curPCLExamResultTemplateChiDinh == value)
                    return;
                _curPCLExamResultTemplateChiDinh = value;
                NotifyOfPropertyChange(() => curPCLExamResultTemplateChiDinh);
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


        public void butSave()
        {
            if (PatientPCLReqID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0301_G1_Msg_InfoChonBN);
                return;
            }

            if (curURP_FE_Oesophagienne.URP_FE_OesophagienneID > 0)
            {
                UpdateURP_FE_Oesophagienne(curURP_FE_Oesophagienne);
            }
            else
            {
                curURP_FE_Oesophagienne.PCLImgResultID = PatientPCLReqID;
                curURP_FE_Oesophagienne.DoctorStaffID = Globals.LoggedUserAccount.Staff.StaffID;
                AddURP_FE_Oesophagienne(curURP_FE_Oesophagienne);
            }

        }

        public void cboVTemplate_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (curURP_FE_Oesophagienne == null)
                return;
            if (((AxComboBox)sender).SelectedItem == null)
            {
                return;
            }
            curPCLExamResultTemplate = new PCLExamResultTemplate();
            curPCLExamResultTemplate.PCLExamResultTemplateID =
                ((PCLExamResultTemplate)((AxComboBox)sender).SelectedItem).PCLExamResultTemplateID;
            GetPCLExamResultTemplate(curPCLExamResultTemplate.PCLExamResultTemplateID);
        }

        public void cboVTempChiDinh_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (curURP_FE_Oesophagienne == null)
                return;
            if (((AxComboBox)sender).SelectedItem == null)
            {
                return;
            }
            curPCLExamResultTemplateChiDinh = new PCLExamResultTemplate();
            curPCLExamResultTemplateChiDinh.PCLExamResultTemplateID = ((PCLExamResultTemplate)((AxComboBox)sender).SelectedItem).PCLExamResultTemplateID;
            GetPCLExamResultTemplateChiDinh(curPCLExamResultTemplateChiDinh.PCLExamResultTemplateID);
        }

        #endregion

        #region method

        private void AddURP_FE_Oesophagienne(URP_FE_Oesophagienne entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginAddURP_FE_Oesophagienne(entity,
                                                                                 Globals.DispatchCallback(
                                                                                     (asyncResult) =>
                                                                                     {
                                                                                         try
                                                                                         {
                                                                                             bool res =
                                                                                                 contract.
                                                                                                     EndAddURP_FE_Oesophagienne
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
                                                                                             Globals.IsBusy = false;
                                                                                         }
                                                                                     }), null);
                                       }
                                   });

            t.Start();
        }

        private void UpdateURP_FE_Oesophagienne(URP_FE_Oesophagienne entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginUpdateURP_FE_Oesophagienne(entity, Globals.DispatchCallback((asyncResult) =>
                                           {
                                               try
                                               {
                                                   bool res = contract.EndUpdateURP_FE_Oesophagienne(asyncResult);
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
                                                   Globals.IsBusy = false;
                                               }
                                           }), null);
                                       }
                                   });

            t.Start();
        }

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

        private void GetPCLExamResultTemplateListByTypeIDChiDinh(long PCLExamGroupTemplateResultID, int ParamEnum)
        {


            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginGetPCLExamResultTemplateListByTypeID(PCLExamGroupTemplateResultID, ParamEnum, Globals.DispatchCallback((asyncResult) =>
                                                                              {
                                                                                  try
                                                                                  {
                                                                                      List<PCLExamResultTemplate> results = contract.EndGetPCLExamResultTemplateListByTypeID(asyncResult);
                                                                                      if (results != null)
                                                                                      {
                                                                                          if (lstPCLExamResultTemplateChiDinh == null)
                                                                                          {
                                                                                              lstPCLExamResultTemplateChiDinh = new ObservableCollection<PCLExamResultTemplate>();
                                                                                          }
                                                                                          else
                                                                                          {
                                                                                              lstPCLExamResultTemplateChiDinh.Clear();
                                                                                          }
                                                                                          foreach (PCLExamResultTemplate p in results)
                                                                                          {
                                                                                              lstPCLExamResultTemplateChiDinh.Add(p);
                                                                                          }

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

        private void GetPCLExamResultTemplateListByTypeIDThanhNguc(long PCLExamGroupTemplateResultID, int ParamEnum)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginGetPCLExamResultTemplateListByTypeID(PCLExamGroupTemplateResultID, ParamEnum, Globals.DispatchCallback((asyncResult) =>
                                                                              {
                                                                                  try
                                                                                  {
                                                                                      List<PCLExamResultTemplate> results = contract.EndGetPCLExamResultTemplateListByTypeID(asyncResult);
                                                                                      if (results != null)
                                                                                      {
                                                                                          if (lstPCLExamResultTemplate == null)
                                                                                          {
                                                                                              lstPCLExamResultTemplate = new ObservableCollection<PCLExamResultTemplate>();
                                                                                          }
                                                                                          else
                                                                                          {
                                                                                              lstPCLExamResultTemplate.Clear();
                                                                                          }
                                                                                          foreach (PCLExamResultTemplate p in results)
                                                                                          {
                                                                                              lstPCLExamResultTemplate.Add(p);
                                                                                          }
                                                                                          NotifyOfPropertyChange(() => lstPCLExamResultTemplate);
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
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginGetPCLExamResultTemplate(PCLExamGroupTemplateResultID, Globals.DispatchCallback((asyncResult) =>
                                                                                          {
                                                                                              try
                                                                                              {
                                                                                                  curPCLExamResultTemplate = contract.EndGetPCLExamResultTemplate(asyncResult);
                                                                                                  curURP_FE_Oesophagienne.ChanDoanThanhNguc = curPCLExamResultTemplate.ResultContent;
                                                                                                  NotifyOfPropertyChange(() => lstPCLExamResultTemplate);
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

        private void GetPCLExamResultTemplateChiDinh(long PCLExamGroupTemplateResultID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
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
                                                                                              curPCLExamResultTemplateChiDinh
                                                                                                  =
                                                                                                  contract.
                                                                                                      EndGetPCLExamResultTemplate
                                                                                                      (asyncResult);
                                                                                              curURP_FE_Oesophagienne
                                                                                                  .ChiDinh =
                                                                                                  curPCLExamResultTemplateChiDinh
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
            //CheckSave();
            //==== 20161129 CMN Begin: Add button save for all pages
            //curURP_FE_Oesophagienne = new URP_FE_Oesophagienne();
            //GetURP_FE_OesophagienneByID(0, PatientPCLReqID);
            //==== 20161129 CMN End: Add button save for all pages
        }
        //==== 20161129 CMN Begin: Add button save for all pages
        public void SetResultDetails(object resDetails)
        {
            curURP_FE_Oesophagienne = (URP_FE_Oesophagienne)resDetails;
            //==== #001
            SelectedStaff = curURP_FE_Oesophagienne.VStaff;
            //==== #001
        }
        //==== 20161129 CMN End: Add button save for all pages
        //==== #001
        private AutoCompleteBox cboStaff;
        private Staff _SelectedStaff;
        public Staff SelectedStaff
        {
            get { return _SelectedStaff; }
            set
            {
                if (_SelectedStaff != value)
                {
                    _SelectedStaff = value;
                    NotifyOfPropertyChange(() => SelectedStaff);
                }
            }
        }
        public void CreatedBy_Loaded(object sender, RoutedEventArgs e)
        {
            cboStaff = sender as AutoCompleteBox;
        }
        public void CreatedBy_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (cboStaff.SelectedItem != null && curURP_FE_Oesophagienne != null)
            {
                curURP_FE_Oesophagienne.VStaff = cboStaff.SelectedItem as Staff;
                curURP_FE_Oesophagienne.DoctorStaffID = curURP_FE_Oesophagienne.VStaff.StaffID;
            }
        }
        public void CreatedDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DatePicker mDatePicker = sender as DatePicker;
            if (curURP_FE_Oesophagienne != null && mDatePicker.SelectedDate != null)
            {
                if (mDatePicker.SelectedDate.Value == DateTime.MinValue)
                    curURP_FE_Oesophagienne.CreateDate = DateTime.Now;
                else if ((mDatePicker.SelectedDate.Value - DateTime.Now).TotalDays > 0)
                    curURP_FE_Oesophagienne.CreateDate = DateTime.Now;
            }
        }
        private ObservableCollection<Staff> _allStaff;
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
        //▼====== #002
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
        //==== #001
    }

    //private void GetStaffCategoriesByType(long V_StaffCatType)
    //{
    //    var t = new Thread(() =>
    //    {
    //        using (var serviceFactory = new ResourcesManagementServiceClient())
    //        {
    //            IResourcesManagementService contract = serviceFactory.ServiceInstance;
    //            contract.BeginGetRefStaffCategoriesByType(V_StaffCatType,
    //                Globals.DispatchCallback(
    //                    (asyncResult) =>
    //                    {
    //                        try
    //                        {
    //                            List<RefStaffCategory> results = contract.EndGetRefStaffCategoriesByType(asyncResult);
    //                            if (results != null && results.Count > 0)
    //                            {
    //                                allStaff = new ObservableCollection<Staff>();
    //                                foreach (RefStaffCategory p in results)
    //                                {
    //                                    GetAllStaff(p.StaffCatgID);
    //                                }
    //                                NotifyOfPropertyChange(() => allStaff);
    //                            }
    //                        }
    //                        catch (Exception ex)
    //                        {
    //                            MessageBox.Show(ex.Message);
    //                        }
    //                        finally
    //                        {
    //                        }
    //                    }), null);
    //        }
    //    });
    //    t.Start();
    //}
    //private void GetAllStaff(long StaffCatgID)
    //{
    //    var t = new Thread(() =>
    //    {
    //        using (var serviceFactory = new ResourcesManagementServiceClient())
    //        {
    //            IResourcesManagementService contract = serviceFactory.ServiceInstance;
    //            contract.BeginGetAllStaff(StaffCatgID, Globals.DispatchCallback((asyncResult) =>
    //            {
    //                try
    //                {
    //                    List<Staff> results = contract.EndGetAllStaff(asyncResult);
    //                    if (results != null && results.Count > 0)
    //                    {
    //                        foreach (Staff p in results)
    //                        {
    //                            allStaff.Add(p);
    //                        }
    //                        NotifyOfPropertyChange(() => allStaff);
    //                    }
    //                }
    //                catch (Exception ex)
    //                {
    //                    MessageBox.Show(ex.Message);
    //                }
    //                finally
    //                {
    //                }
    //            }), null);
    //        }
    //    });
    //    t.Start();
    //}
    //▲====== #002
}