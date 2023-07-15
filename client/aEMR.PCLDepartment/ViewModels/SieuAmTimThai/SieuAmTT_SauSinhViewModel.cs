/*
 * #001: 20161215 CMN Begin: Add control for choose doctor and date
 * #002: 20180904 TTM:  Thay đổi cách lấy danh sách Staff theo Category từ chạy đi chạy về database -> lấy từ Globals.
 *                      Vì danh sách Staff đã được lấy lên lúc log-in không có lý do gì phải lấy lại từng lần theo Category như vậy.
 */
using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
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
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof (ISieuAmTT_SauSinh)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SieuAmTT_SauSinhViewModel : Conductor<object>, ISieuAmTT_SauSinh
         , IHandle<ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SieuAmTT_SauSinhViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            //▼====== #002
            GetStaffCategoriesByType();
            //▲====== #002
            //GetStaffCategoriesByType((long)AllLookupValues.StaffCatType.BAC_SI);
            GetPCLExamResultTemplateListByTypeID(3, (int)AllLookupValues.PCLResultParamImpID.SIEUAM_TIMTHAI);

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

        private ObservableCollection<Staff> _allStaff;
        private PCLExamResultTemplate _curPCLExamResultTemplate;
        private UltraResParams_FetalEchocardiographyPostpartum _curURP_FEPostpartum;
        private bool _isSave;
        private bool _isUpdate;
        private ObservableCollection<PCLExamResultTemplate> _lstPCLExamResultTemplate;

        private UltraResParams_FetalEchocardiographyPostpartum _tempURP_FEPostpartum;
        private bool isHasPatient;

        public UltraResParams_FetalEchocardiographyPostpartum curURP_FEPostpartum
        {
            get { return _curURP_FEPostpartum; }
            set
            {
                if (_curURP_FEPostpartum == value)
                    return;
                _curURP_FEPostpartum = value;
                NotifyOfPropertyChange(() => curURP_FEPostpartum);
            }
        }

        public UltraResParams_FetalEchocardiographyPostpartum tempURP_FEPostpartum
        {
            get { return _tempURP_FEPostpartum; }
            set
            {
                if (_tempURP_FEPostpartum == value)
                    return;
                _tempURP_FEPostpartum = value;
                NotifyOfPropertyChange(() => tempURP_FEPostpartum);
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

        //curURP_FEPostpartum.UltraResParams_FetalEchocardiographyPostpartumID
        //curURP_FEPostpartum.PCLImgResultID
        //curURP_FEPostpartum.DoctorStaffID

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
            curURP_FEPostpartum = new UltraResParams_FetalEchocardiographyPostpartum();
            curURP_FEPostpartum = tempURP_FEPostpartum;
            NotifyOfPropertyChange(() => curURP_FEPostpartum);
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

            if(curURP_FEPostpartum.UltraResParams_FetalEchocardiographyPostpartumID>0)
            {
                UpdateUltraResParams_FetalEchocardiographyPostpartum(curURP_FEPostpartum);
            }
            else
            {
                curURP_FEPostpartum.PCLImgResultID = PatientPCLReqID;
                AddUltraResParams_FetalEchocardiographyPostpartum(curURP_FEPostpartum);
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

        public void cboVTemplate_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (curURP_FEPostpartum == null)
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

        private void AddUltraResParams_FetalEchocardiographyPostpartum(
            UltraResParams_FetalEchocardiographyPostpartum entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginAddUltraResParams_FetalEchocardiographyPostpartum(entity,Globals.DispatchCallback((asyncResult)=>
                                                                                                                    {
                                                                                                                        try
                                                                                                                        {
                                                                                                                            bool res=contract.EndAddUltraResParams_FetalEchocardiographyPostpartum(asyncResult);
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

        private void UpdateUltraResParams_FetalEchocardiographyPostpartum(
            UltraResParams_FetalEchocardiographyPostpartum entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginUpdateUltraResParams_FetalEchocardiographyPostpartum(entity,Globals.DispatchCallback((asyncResult)=>
                                                                                                                       {
                                                                                                                           try
                                                                                                                           {
                                                                                                                               bool res=contract.EndUpdateUltraResParams_FetalEchocardiographyPostpartum(asyncResult);
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

        private void GetUltraResParams_FetalEchocardiographyPostpartum(long UltraResParams_FetalEchocardiographyPostpartumID, long PCLImgResultID)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginGetUltraResParams_FetalEchocardiographyPostpartumByID(UltraResParams_FetalEchocardiographyPostpartumID, PCLImgResultID, Globals.DispatchCallback((asyncResult) =>
                                                                                              {
                                                                                                  try
                                                                                                  {
                                                                                                      var item=contract.EndGetUltraResParams_FetalEchocardiographyPostpartumByID(asyncResult);
                                                                                                      if(item!=null)
                                                                                                      {
                                                                                                          curURP_FEPostpartum=item;
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
                                                                                                  curURP_FEPostpartum.
                                                                                                      AnotherDiagnosic =
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
           
            //CheckHasPCLImageID();
            //CheckSave();
            //_tempURP_FEPostpartum = new UltraResParams_FetalEchocardiographyPostpartum();
            //==== 20161129 CMN Begin: Add button save for all pages
            //curURP_FEPostpartum = new UltraResParams_FetalEchocardiographyPostpartum();
            //GetUltraResParams_FetalEchocardiographyPostpartum(0,PatientPCLReqID);
            //==== 20161129 CMN End: Add button save for all pages
        }
        //==== 20161129 CMN Begin: Add button save for all pages
        public void SetResultDetails(object resDetails)
        {
            curURP_FEPostpartum = (UltraResParams_FetalEchocardiographyPostpartum)resDetails;
            //==== #001
            SelectedStaff = curURP_FEPostpartum.VStaff;
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
            if (cboStaff.SelectedItem != null && curURP_FEPostpartum != null)
            {
                curURP_FEPostpartum.VStaff = cboStaff.SelectedItem as Staff;
                curURP_FEPostpartum.DoctorStaffID = curURP_FEPostpartum.VStaff.StaffID;
            }
        }
        public void CreatedDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DatePicker mDatePicker = sender as DatePicker;
            if (curURP_FEPostpartum != null && mDatePicker.SelectedDate != null)
            {
                if (mDatePicker.SelectedDate.Value == DateTime.MinValue)
                    curURP_FEPostpartum.CreateDate = DateTime.Now;
                else if ((mDatePicker.SelectedDate.Value - DateTime.Now).TotalDays > 0)
                    curURP_FEPostpartum.CreateDate = DateTime.Now;
            }
        }
        //==== #001

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
    //                                                                                             Globals.IsBusy =false;
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
    //                                   contract.BeginGetAllStaff(StaffCatgID,Globals.DispatchCallback((asyncResult) =>
    //                                                                                          {
    //                                                                                              try
    //                                                                                              {
    //                                                                                                  List<Staff>results =contract.EndGetAllStaff(asyncResult);
    //                                                                                                  if (results !=null &&results.Count >0)
    //                                                                                                  {
    //                                                                                                      foreach (Staff p in results)
    //                                                                                                      {
    //                                                                                                          allStaff.Add(p);
    //                                                                                                      }
    //                                                                                                      NotifyOfPropertyChange(() =>allStaff);
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
    //▲====== #002
}