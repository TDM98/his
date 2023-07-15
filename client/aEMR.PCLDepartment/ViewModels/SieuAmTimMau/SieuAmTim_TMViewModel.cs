using eHCMSLanguage;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
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
    [Export(typeof (ISieuAmTim_TM)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SieuAmTim_TMViewModel : Conductor<object>, ISieuAmTim_TM

    //, IHandle<ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SieuAmTim_TMViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            Globals.EventAggregator.Subscribe(this);

            if (Globals.PatientPCLReqID_Imaging > 0)
            {
                PatientPCLReqID = Globals.PatientPCLReqID_Imaging;
                //LoadInfo();
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

        public void SetResultDetails(object resDetails)
        {
            curUltraResParams_EchoCardiography = (UltraResParams_EchoCardiography)resDetails;
        }


        #region properties

        private UltraResParams_EchoCardiography _curUltraResParams_EchoCardiography;
        
        public UltraResParams_EchoCardiography curUltraResParams_EchoCardiography
        {
            get { return _curUltraResParams_EchoCardiography; }
            set
            {
                if (_curUltraResParams_EchoCardiography == value)
                    return;
                _curUltraResParams_EchoCardiography = value;
                NotifyOfPropertyChange(() => curUltraResParams_EchoCardiography);
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
        //    //curUltraResParams_EchoCardiography.PCLImgResultID
        //    //curUltraResParams_EchoCardiography.DoctorStaffID
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
        //    curUltraResParams_EchoCardiography = new UltraResParams_EchoCardiography();
        //    curUltraResParams_EchoCardiography = tempUltraResParams_EchoCardiography;
        //    NotifyOfPropertyChange(() => curUltraResParams_EchoCardiography);
        //}

        public void butSave()
        {
            if (PatientPCLReqID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0301_G1_Msg_InfoChonBN);
                return;
            }

            curUltraResParams_EchoCardiography.TabIndex = 0;

            if (validationSummary.DisplayedErrors.Count > 0)
            {
                MessageBox.Show(eHCMSResources.A0559_G1_Msg_InfoGTriKhHopLe);
                return;
            }

            if (curUltraResParams_EchoCardiography.UltraResParams_EchoCardiographyID > 0)
            {
                UpdateUltraResParams_EchoCardiography(curUltraResParams_EchoCardiography);   
            }
            else
            {
                curUltraResParams_EchoCardiography.PCLImgResultID = PatientPCLReqID;
                curUltraResParams_EchoCardiography.DoctorStaffID = Globals.LoggedUserAccount.Staff.StaffID;
                AddUltraResParams_EchoCardiography(curUltraResParams_EchoCardiography);
            }
        }

        public void ValidationSummary_Loaded(object sender, RoutedEventArgs e)
        {
            validationSummary = sender as ValidationSummary;
        }

        #endregion

        #region method

        private void AddUltraResParams_EchoCardiography(UltraResParams_EchoCardiography entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginAddUltraResParams_EchoCardiography(entity, null, null, null,
                                                                                            Globals.DispatchCallback(
                                                                                                (asyncResult) =>
                                                                                                    {
                                                                                                        try
                                                                                                        {
                                                                                                            bool res =contract.EndAddUltraResParams_EchoCardiography(asyncResult);
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

        private void UpdateUltraResParams_EchoCardiography(UltraResParams_EchoCardiography entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginUpdateUltraResParams_EchoCardiography(entity, null, null, null,
                                                                                               Globals.DispatchCallback(
                                                                                                   (asyncResult) =>
                                                                                                       {
                                                                                                           try
                                                                                                           {
                                                                                                               bool res=contract.EndUpdateUltraResParams_EchoCardiography(asyncResult);
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
                                                                                                               Globals.IsBusy=false;
                                                                                                           }
                                                                                                       }), null);
                                       }
                                   });

            t.Start();
        }

        private void GetUltraResParams_EchoCardiographyByID(long UltraResParams_EchoCardiographyID, long PCLImgResultID)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginGetUltraResParams_EchoCardiography(UltraResParams_EchoCardiographyID, PCLImgResultID, Globals.DispatchCallback((asyncResult) =>
                                                                                              {
                                                                                                  try
                                                                                                  {
                                                                                                      var item = contract.EndGetUltraResParams_EchoCardiography(asyncResult);
                                                                                                      if (item != null)
                                                                                                      {
                                                                                                          curUltraResParams_EchoCardiography = item;
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
        //                                                                                              if (allValveOpen ==null)
        //                                                                                              {
        //                                                                                                  allValveOpen =new ObservableCollection<Lookup>();
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

        #endregion
       
    }
}


//public void Handle(DbClickSelectedObjectEvent<PatientPCLRequest> message)
//{
//    if (message != null)
//    {
//        PatientPCLReqID = message.Result.PatientPCLReqID;
//        LoadInfo();
//    }
//}

//public void Handle(ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest> message)
//{
//    if (message != null)
//    {
//        PatientPCLReqID = message.PCLRequest.PatientPCLReqID;
//        LoadInfo();
//    }
//}

//private void LoadInfo()
//{
//    return;

//    //GetV_ValveOpen();
//    //CheckHasPCLImageID();
//    //CheckSave();
//    //_tempUltraResParams_EchoCardiography = new UltraResParams_EchoCardiography();
//    curUltraResParams_EchoCardiography = new UltraResParams_EchoCardiography();
//    GetUltraResParams_EchoCardiographyByID(0, PatientPCLReqID);
//}