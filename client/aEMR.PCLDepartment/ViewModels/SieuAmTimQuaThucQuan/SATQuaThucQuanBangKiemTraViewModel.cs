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
using CommonServiceProxy;
using DataEntities;
using PCLsProxy;
using aEMR.Controls;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;

namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof(ISATQuaThucQuanBangKiemTra)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SATQuaThucQuanBangKiemTraViewModel : Conductor<object>, ISATQuaThucQuanBangKiemTra
         , IHandle<ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SATQuaThucQuanBangKiemTraViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
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

        //private ObservableCollection<Lookup> _allValveOpen;
        private URP_FE_OesophagienneCheck _curURP_FE_OesophagienneCheck;
        //private bool _isSave;
        //private bool _isUpdate;

        //private URP_FE_OesophagienneCheck _tempURP_FE_OesophagienneCheck;


        //private bool isHasPatient;

        public URP_FE_OesophagienneCheck curURP_FE_OesophagienneCheck
        {
            get { return _curURP_FE_OesophagienneCheck; }
            set
            {
                if (_curURP_FE_OesophagienneCheck == value)
                    return;
                _curURP_FE_OesophagienneCheck = value;
                NotifyOfPropertyChange(() => curURP_FE_OesophagienneCheck);
            }
        }

        //public URP_FE_OesophagienneCheck tempURP_FE_OesophagienneCheck
        //{
        //    get { return _tempURP_FE_OesophagienneCheck; }
        //    set
        //    {
        //        if (_tempURP_FE_OesophagienneCheck == value)
        //            return;
        //        _tempURP_FE_OesophagienneCheck = value;
        //        NotifyOfPropertyChange(() => tempURP_FE_OesophagienneCheck);
        //    }
        //}

        //public bool IsHasPatient
        //{
        //    get { return isHasPatient; }
        //    set
        //    {
        //        if (isHasPatient == value)
        //            return;
        //        isHasPatient = value;
        //        NotifyOfPropertyChange(() => isHasPatient);
        //    }
        //}

        //public bool isSave
        //{
        //    get { return _isSave; }
        //    set
        //    {
        //        if (_isSave == value)
        //            return;
        //        _isSave = value;
        //        NotifyOfPropertyChange(() => isSave);
        //    }
        //}

        //public bool isUpdate
        //{
        //    get { return _isUpdate; }
        //    set
        //    {
        //        if (_isUpdate == value)
        //            return;
        //        _isUpdate = value;
        //        NotifyOfPropertyChange(() => isUpdate);
        //    }
        //}

        //public ObservableCollection<Lookup> allValveOpen
        //{
        //    get { return _allValveOpen; }
        //    set
        //    {
        //        if (_allValveOpen == value)
        //            return;
        //        _allValveOpen = value;
        //        NotifyOfPropertyChange(() => allValveOpen);
        //    }
        //}

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
        //    //curURP_FE_OesophagienneCheck.PCLImgResultID
        //    //curURP_FE_OesophagienneCheck.DoctorStaffID
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
        //    curURP_FE_OesophagienneCheck = new URP_FE_OesophagienneCheck();
        //    curURP_FE_OesophagienneCheck = tempURP_FE_OesophagienneCheck;
        //    NotifyOfPropertyChange(() => curURP_FE_OesophagienneCheck);
        //}

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

            if (curURP_FE_OesophagienneCheck.URP_FE_OesophagienneCheckID > 0)
            {
                UpdateURP_FE_OesophagienneCheck(curURP_FE_OesophagienneCheck);
            }
            else
            {
                curURP_FE_OesophagienneCheck.PCLImgResultID = PatientPCLReqID;
                curURP_FE_OesophagienneCheck.DoctorStaffID = Globals.LoggedUserAccount.Staff.StaffID;
                AddURP_FE_OesophagienneCheck(curURP_FE_OesophagienneCheck);   
            }
        }
        
        public void ValidationSummary_Loaded(object sender, RoutedEventArgs e)
        {
            validationSummary = sender as ValidationSummary;
        }

        #endregion

        #region method

        private void AddURP_FE_OesophagienneCheck(URP_FE_OesophagienneCheck entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginAddURP_FE_OesophagienneCheck(entity,
                                                               Globals.DispatchCallback(
                                                                   (asyncResult) =>
                                                                   {
                                                                       try
                                                                       {
                                                                           bool res =contract.EndAddURP_FE_OesophagienneCheck(asyncResult);
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

        private void UpdateURP_FE_OesophagienneCheck(URP_FE_OesophagienneCheck entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateURP_FE_OesophagienneCheck(entity,
                                                                  Globals.DispatchCallback(
                                                                      (asyncResult) =>
                                                                      {
                                                                          try
                                                                          {
                                                                              bool res =contract.EndUpdateURP_FE_OesophagienneCheck(asyncResult);
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

        private void GetURP_FE_OesophagienneCheckByID(long URP_FE_OesophagienneCheckID, long PCLImgResultID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetURP_FE_OesophagienneCheck(URP_FE_OesophagienneCheckID, PCLImgResultID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var item = contract.EndGetURP_FE_OesophagienneCheck(asyncResult);
                            if (item != null)
                            {
                                curURP_FE_OesophagienneCheck = item;
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
            //_tempURP_FE_OesophagienneCheck = new URP_FE_OesophagienneCheck();
            //==== 20161129 CMN Begin: Add button save for all pages
            //curURP_FE_OesophagienneCheck = new URP_FE_OesophagienneCheck();
            //GetURP_FE_OesophagienneCheckByID(0, PatientPCLReqID);
            //==== 20161129 CMN End: Add button save for all pages
        }

        //==== 20161129 CMN Begin: Add button save for all pages
        public void SetResultDetails(object resDetails)
        {
            curURP_FE_OesophagienneCheck = (URP_FE_OesophagienneCheck)resDetails;
        }
        //==== 20161129 CMN End: Add button save for all pages
    }
}