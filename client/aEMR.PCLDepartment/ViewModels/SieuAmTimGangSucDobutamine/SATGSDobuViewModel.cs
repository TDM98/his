using eHCMSLanguage;
using System;
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
    [Export(typeof (ISATGSDobu)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SATGSDobuViewModel : Conductor<object>, ISATGSDobu
        , IHandle<ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SATGSDobuViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            //Globals.EventAggregator.Subscribe(this);

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

        private URP_FE_StressDobutamine _curURP_FE_StressDobutamine;
        private URP_FE_VasculaireExam _curURP_FE_VasculaireExam;
        private bool _isSave;
        private bool _isUpdate;

        private URP_FE_StressDobutamine _tempURP_FE_StressDobutamine;
        private URP_FE_VasculaireExam _tempURP_FE_VasculaireExam;
        private bool isHasPatient;

        public URP_FE_StressDobutamine curURP_FE_StressDobutamine
        {
            get { return _curURP_FE_StressDobutamine; }
            set
            {
                if (_curURP_FE_StressDobutamine == value)
                    return;
                _curURP_FE_StressDobutamine = value;
                NotifyOfPropertyChange(() => curURP_FE_StressDobutamine);
            }
        }

        public URP_FE_StressDobutamine tempURP_FE_StressDobutamine
        {
            get { return _tempURP_FE_StressDobutamine; }
            set
            {
                if (_tempURP_FE_StressDobutamine == value)
                    return;
                _tempURP_FE_StressDobutamine = value;
                NotifyOfPropertyChange(() => tempURP_FE_StressDobutamine);
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
            //curURP_FE_StressDobutamine.PCLImgResultID
            //curURP_FE_StressDobutamine.DoctorStaffID
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
            curURP_FE_StressDobutamine = new URP_FE_StressDobutamine();
            curURP_FE_StressDobutamine = tempURP_FE_StressDobutamine;
            NotifyOfPropertyChange(() => curURP_FE_StressDobutamine);
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

            curURP_FE_StressDobutamine.DoctorStaffID = Globals.LoggedUserAccount.Staff.StaffID;

            if (curURP_FE_StressDobutamine.URP_FE_StressDobutamineID > 0)
            {
                UpdateURP_FE_StressDobutamine(curURP_FE_StressDobutamine);
            }
            else
            {
                curURP_FE_StressDobutamine.PCLImgResultID = PatientPCLReqID;
                AddURP_FE_StressDobutamine(curURP_FE_StressDobutamine);
            }
        }

        //public void butUpdate()
        //{
        //    if (validationSummary.DisplayedErrors.Count > 0)
        //    {
        //        MessageBox.Show(eHCMSResources.A0559_G1_Msg_InfoGTriKhHopLe);
        //        return;
        //    }
        //    UpdateURP_FE_StressDobutamine(curURP_FE_StressDobutamine);
        //}

        public void ValidationSummary_Loaded(object sender, RoutedEventArgs e)
        {
            validationSummary = sender as ValidationSummary;
        }

        #endregion

        #region method

        private void AddURP_FE_StressDobutamine(URP_FE_StressDobutamine entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginAddURP_FE_StressDobutamine(entity,
                                                                                    Globals.DispatchCallback(
                                                                                        (asyncResult) =>
                                                                                            {
                                                                                                try
                                                                                                {
                                                                                                    bool res =contract.EndAddURP_FE_StressDobutamine(asyncResult);
                                                                                                    if (res)
                                                                                                    {
                                                                                                        MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        MessageBox.Show("Lưu không thành công!");
                                                                                                    }
                                                                                                }
                                                                                                catch (Exception ex)
                                                                                                {
                                                                                                    MessageBox.Show(ex.Message);
                                                                                                }
                                                                                                finally
                                                                                                {
                                                                                                    Globals.IsBusy =
                                                                                                        false;
                                                                                                }
                                                                                            }), null);
                                       }
                                   });

            t.Start();
        }

        private void UpdateURP_FE_StressDobutamine(URP_FE_StressDobutamine entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginUpdateURP_FE_StressDobutamine(entity,
                                                                                       Globals.DispatchCallback(
                                                                                           (asyncResult) =>
                                                                                               {
                                                                                                   try
                                                                                                   {
                                                                                                       bool res =contract.EndUpdateURP_FE_StressDobutamine(asyncResult);
                                                                                                       if (res)
                                                                                                       {
                                                                                                           MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                                                                                                       }
                                                                                                       else
                                                                                                       {
                                                                                                           MessageBox.Show("Lưu không thành công!");
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

        private void GetURP_FE_StressDobutamineByID(long URP_FE_StressDobutamineID, long PCLImgResultID)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginGetURP_FE_StressDobutamine(URP_FE_StressDobutamineID, PCLImgResultID,Globals.DispatchCallback((asyncResult) =>
                                                                                            {
                                                                                                try
                                                                                                {
                                                                                                    var item = contract.EndGetURP_FE_StressDobutamine(asyncResult);
                                                                                                    if(item!=null)
                                                                                                    {
                                                                                                        curURP_FE_StressDobutamine = item;
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

        #endregion

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
            //_tempURP_FE_StressDobutamine = new URP_FE_StressDobutamine();
            //curURP_FE_StressDobutamine = new URP_FE_StressDobutamine();
            //GetURP_FE_StressDobutamineByID(0, PatientPCLReqID);
        }
        //==== 20161201 CMN Begin: Add button save for all pages
        public void SetResultDetails(object resDetails)
        {
            curURP_FE_StressDobutamine = (URP_FE_StressDobutamine)resDetails;
        }
        public bool CheckValidate()
        {
            return (validationSummary != null && validationSummary.DisplayedErrors.Count == 0) || validationSummary == null;
        }
        //==== 20161201 CMN End.
    }
}