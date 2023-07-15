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
    [Export(typeof(ISieuAmTim_2D)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SieuAmTim_2DViewModel : Conductor<object>, ISieuAmTim_2D

    //, IHandle<ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SieuAmTim_2DViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            Globals.EventAggregator.Subscribe(this);
            GetV_ValveOpen();
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

        private ObservableCollection<Lookup> _allSitus;
        private UltraResParams_EchoCardiography _curUltraResParams_EchoCardiography;
        private bool _isSave;
        private bool _isUpdate;

        private UltraResParams_EchoCardiography _tempUltraResParams_EchoCardiography;


        private bool isHasPatient;

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

        public UltraResParams_EchoCardiography tempUltraResParams_EchoCardiography
        {
            get { return _tempUltraResParams_EchoCardiography; }
            set
            {
                if (_tempUltraResParams_EchoCardiography == value)
                    return;
                _tempUltraResParams_EchoCardiography = value;
                NotifyOfPropertyChange(() => tempUltraResParams_EchoCardiography);
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

        public ObservableCollection<Lookup> allSitus
        {
            get { return _allSitus; }
            set
            {
                if (_allSitus == value)
                    return;
                _allSitus = value;
                NotifyOfPropertyChange(() => allSitus);
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
            if(PatientPCLReqID<=0)
            {
                MessageBox.Show(eHCMSResources.A0301_G1_Msg_InfoChonBN);
                return;
            }

            curUltraResParams_EchoCardiography.TabIndex = 1;

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

        //public void butUpdate()
        //{
        //    if (validationSummary.DisplayedErrors.Count > 0)
        //    {
        //        MessageBox.Show(eHCMSResources.A0559_G1_Msg_InfoGTriKhHopLe);
        //        return;
        //    }

        //}

        public void ValidationSummary_Loaded(object sender, RoutedEventArgs e)
        {
            validationSummary = sender as ValidationSummary;
        }

        #endregion

        #region method

        private void AddUltraResParams_EchoCardiography(UltraResParams_EchoCardiography entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
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
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateUltraResParams_EchoCardiography(entity,null,null,null,
                                                                        Globals.DispatchCallback((asyncResult) =>
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
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
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
                            Globals.IsBusy =false;
                        }
                    }), null);
                }
            });

            t.Start();
        }

        private void GetV_ValveOpen()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new CommonService_V2Client())
                                       {
                                           var contract = serviceFactory.ServiceInstance;
                                           contract.BeginGetAllLookupValuesByType(LookupValues.V_Situs, Globals.DispatchCallback((asyncResult) =>
                                                                                      {
                                                                                          try
                                                                                          {
                                                                                              IList<Lookup> results = contract.EndGetAllLookupValuesByType(asyncResult);
                                                                                              if (results != null)
                                                                                              {
                                                                                                  if (allSitus == null)
                                                                                                  {
                                                                                                      allSitus = new ObservableCollection<Lookup>();
                                                                                                  }
                                                                                                  else
                                                                                                  {
                                                                                                      allSitus.Clear();
                                                                                                  }
                                                                                                  foreach (Lookup p in results)
                                                                                                  {
                                                                                                      allSitus.Add(p);
                                                                                                  }
                                                                                                  NotifyOfPropertyChange(() => allSitus);
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

//    //CheckHasPCLImageID();
//    //CheckSave();
//    curUltraResParams_EchoCardiography = new UltraResParams_EchoCardiography();
//    GetUltraResParams_EchoCardiographyByID(0, PatientPCLReqID);
//}