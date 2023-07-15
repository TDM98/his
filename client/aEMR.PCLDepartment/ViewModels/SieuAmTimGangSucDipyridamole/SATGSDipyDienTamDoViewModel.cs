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
    [Export(typeof (ISATGSDipyDienTamDo)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SATGSDipyDienTamDoViewModel : Conductor<object>, ISATGSDipyDienTamDo
         , IHandle<ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest>>
    {
        [ImportingConstructor]
        public SATGSDipyDienTamDoViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
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
        private URP_FE_StressDipyridamoleElectrocardiogram _curURP_FE_StressDipyridamoleElectrocardiogram;
        private bool _isSave;
        private bool _isUpdate;

        private URP_FE_StressDipyridamoleElectrocardiogram _tempURP_FE_StressDipyridamoleElectrocardiogram;


        private bool isHasPatient;

        public URP_FE_StressDipyridamoleElectrocardiogram curURP_FE_StressDipyridamoleElectrocardiogram
        {
            get { return _curURP_FE_StressDipyridamoleElectrocardiogram; }
            set
            {
                if (_curURP_FE_StressDipyridamoleElectrocardiogram == value)
                    return;
                _curURP_FE_StressDipyridamoleElectrocardiogram = value;
                NotifyOfPropertyChange(() => curURP_FE_StressDipyridamoleElectrocardiogram);
            }
        }

        public URP_FE_StressDipyridamoleElectrocardiogram tempURP_FE_StressDipyridamoleElectrocardiogram
        {
            get { return _tempURP_FE_StressDipyridamoleElectrocardiogram; }
            set
            {
                if (_tempURP_FE_StressDipyridamoleElectrocardiogram == value)
                    return;
                _tempURP_FE_StressDipyridamoleElectrocardiogram = value;
                NotifyOfPropertyChange(() => tempURP_FE_StressDipyridamoleElectrocardiogram);
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
            //curURP_FE_StressDipyridamoleElectrocardiogram.PCLImgResultID
            //curURP_FE_StressDipyridamoleElectrocardiogram.DoctorStaffID
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
            curURP_FE_StressDipyridamoleElectrocardiogram = new URP_FE_StressDipyridamoleElectrocardiogram();
            curURP_FE_StressDipyridamoleElectrocardiogram = tempURP_FE_StressDipyridamoleElectrocardiogram;
            NotifyOfPropertyChange(() => curURP_FE_StressDipyridamoleElectrocardiogram);
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

            if (curURP_FE_StressDipyridamoleElectrocardiogram.URP_FE_StressDipyridamoleElectrocardiogramID > 0)
            {
                UpdateURP_FE_StressDipyridamoleElectrocardiogram(curURP_FE_StressDipyridamoleElectrocardiogram);
            }
            else
            {
                curURP_FE_StressDipyridamoleElectrocardiogram.PCLImgResultID = PatientPCLReqID;
                curURP_FE_StressDipyridamoleElectrocardiogram.DoctorStaffID = Globals.LoggedUserAccount.Staff.StaffID;
                AddURP_FE_StressDipyridamoleElectrocardiogram(curURP_FE_StressDipyridamoleElectrocardiogram);
            }
        }

        //public void butUpdate()
        //{
        //    if (validationSummary.DisplayedErrors.Count > 0)
        //    {
        //        MessageBox.Show(eHCMSResources.A0559_G1_Msg_InfoGTriKhHopLe);
        //        return;
        //    }
        //    UpdateURP_FE_StressDipyridamoleElectrocardiogram(curURP_FE_StressDipyridamoleElectrocardiogram);
        //}

        public void ValidationSummary_Loaded(object sender, RoutedEventArgs e)
        {
            validationSummary = sender as ValidationSummary;
        }

        #endregion

        #region method

        private void AddURP_FE_StressDipyridamoleElectrocardiogram(URP_FE_StressDipyridamoleElectrocardiogram entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginAddURP_FE_StressDipyridamoleElectrocardiogram(entity,Globals.DispatchCallback((asyncResult)
                                                                                                            =>
                                                                                                                {
                                                                                                                    try
                                                                                                                    {
                                                                                                                        bool res=contract.EndAddURP_FE_StressDipyridamoleElectrocardiogram(asyncResult);
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

        private void UpdateURP_FE_StressDipyridamoleElectrocardiogram(URP_FE_StressDipyridamoleElectrocardiogram entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginUpdateURP_FE_StressDipyridamoleElectrocardiogram(entity,Globals.DispatchCallback((asyncResult)=>
                                                                                                                   {
                                                                                                                       try
                                                                                                                       {
                                                                                                                           bool res=contract.EndUpdateURP_FE_StressDipyridamoleElectrocardiogram(asyncResult);
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

        private void GetURP_FE_StressDipyridamoleElectrocardiogramByID(long URP_FE_StressDipyridamoleElectrocardiogramID, long PCLImgResultID)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginGetURP_FE_StressDipyridamoleElectrocardiogram(URP_FE_StressDipyridamoleElectrocardiogramID, PCLImgResultID, Globals.DispatchCallback((asyncResult) =>
                                                                                              {
                                                                                                  try
                                                                                                  {
                                                                                                      var item = contract.EndGetURP_FE_StressDipyridamoleElectrocardiogram(asyncResult);
                                                                                                      if(item!=null)
                                                                                                      {
                                                                                                          curURP_FE_StressDipyridamoleElectrocardiogram = item; 
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
            //CheckHasPCLImageID();
            //CheckSave();
            //_tempURP_FE_StressDipyridamoleElectrocardiogram = new URP_FE_StressDipyridamoleElectrocardiogram();
            //curURP_FE_StressDipyridamoleElectrocardiogram = new URP_FE_StressDipyridamoleElectrocardiogram();
            //GetURP_FE_StressDipyridamoleElectrocardiogramByID(0, PatientPCLReqID);
        }
        //==== 20161201 CMN Begin: Add button save for all pages
        public void SetResultDetails(object resDetails)
        {
            GetV_ValveOpen();
            curURP_FE_StressDipyridamoleElectrocardiogram = (URP_FE_StressDipyridamoleElectrocardiogram)resDetails;
        }
        public bool CheckValidate()
        {
            return (validationSummary != null && validationSummary.DisplayedErrors.Count == 0) || validationSummary == null;
        }
        //==== 20161201 CMN End.
    }
}