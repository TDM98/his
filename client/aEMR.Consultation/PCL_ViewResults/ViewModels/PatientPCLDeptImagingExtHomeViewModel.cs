using eHCMSLanguage;
using System;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using System.Windows;
using System.Windows.Controls;
using DataEntities;
using aEMR.ServiceClient;
using System.Collections.ObjectModel;
using System.Threading;
using aEMR.Common.Collections;
using Castle.Windsor;
using aEMR.Common.BaseModel;

namespace aEMR.ConsultantEPrescription.PCL_ViewResults.ViewModels
{
    [Export(typeof(IPatientPCLDeptImagingExtHome)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientPCLDeptImagingExtHomeViewModel : ViewModelBase, IPatientPCLDeptImagingExtHome
        , IHandle<DbClickPtPCLReqExtEdit<PatientPCLRequest_Ext, String>>
        , IHandle<DbClickPtPCLReqExtDo<PatientPCLRequest_Ext ,PatientPCLRequestDetail_Ext, String>>
        , IHandle<ReLoadListPCLRequest>
        , IHandle<ShowPatientInfo_KHAMBENH_CLS_NGOAIVIEN_HINHANH<Patient, PatientRegistration, PatientRegistrationDetail>>
    {
        [ImportingConstructor]
        public PatientPCLDeptImagingExtHomeViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Globals.EventAggregator.Subscribe(this);
            CreateSubVM();
            authorization();
        }

        private void CreateSubVM()
        {
            UCDoctorProfileInfo = Globals.GetViewModel<ILoginInfo>();
            
            UCPatientProfileInfo = Globals.GetViewModel<IPatientInfo>();
                       
            UCPatientPCLRequestExtEdit = Globals.GetViewModel<IPatientPCLDeptImagingExtResult>();
            
            UCPatientPCLRequestEdit = Globals.GetViewModel<IPatientPCLRequestEditImageExt>();            

            UCPatientPCLRequestList = Globals.GetViewModel<IListPCLRequestExt>();            
        }

        private void ActivateSubVM()
        {            
            ActivateItem(UCDoctorProfileInfo);

            ActivateItem(UCPatientProfileInfo);

            ActivateItem(UCPatientPCLRequestExtEdit);
            
            ActivateItem(UCPatientPCLRequestEdit);
            
            ActivateItem(UCPatientPCLRequestList);
        }

        // TxD 22/09/2018 Added the following to ALLOW child VM of each Tab to get ACCESS to the CENTRAL Data stored in the MainKB (Main KHAM BENH) Screen
        private ICS_DataStorage _CS_DS = null;
        public ICS_DataStorage CS_DS
        {
            get
            {
                return _CS_DS;
            }
            set
            {
                _CS_DS = value;
                ((IPatientInfo)UCPatientProfileInfo).CS_DS = CS_DS;
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            ActivateSubVM();
        }

        public object UCDoctorProfileInfo { get; set; }

        public object UCPatientProfileInfo { get; set; }

        public object UCPtRegDetailInfo
        {
            get;
            set;
        }

        //KMx: Sau khi kiểm tra, thấy biến này không được sử dụng nữa (25/05/2014 14:48).
        //public object UCHeaderInfoPMR
        //{
        //    get;
        //    set;
        //}

        public IPatientPCLDeptImagingExtResult UCPatientPCLRequestExtEdit
        {
            get;
            set;
        }
        public object UCPatientPCLRequestList
        {
            get;
            set;
        }
        public IPatientPCLRequestEditImageExt UCPatientPCLRequestEdit
        {
            get;
            set;
        }

        private int Index = 0;
        public void tabCommon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Index = ((TabControl)sender).SelectedIndex;
        }

        public TabControl tabCommon { get; set; }
        public void tabCommon_Loaded(object sender, RoutedEventArgs e)
        {
            tabCommon = (TabControl)sender;
            if (!mUCPatientPCLRequestNew)
                tabCommon.SelectedIndex = 1;
        }

        public object TabEdit
        {
            get;
            set;
        }

        public object TabWork
        {
            get;
            set;
        }
        private bool _IsLoading;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set
            {
                _IsLoading = value;
                NotifyOfPropertyChange(() => IsLoading);
            }
        }

        private ObservableCollection<PatientPCLRequest_Ext> _allPatientPCLRequest_Ext;
        public ObservableCollection<PatientPCLRequest_Ext> allPatientPCLRequest_Ext
        {
            get { return _allPatientPCLRequest_Ext; }
            set
            {
                _allPatientPCLRequest_Ext = value;
                NotifyOfPropertyChange(() => allPatientPCLRequest_Ext);
            }
        }

        public void TabEdit_Loaded(object sender, RoutedEventArgs e)
        {
            TabEdit = sender;
        }

        public void TabWork_Loaded(object sender, RoutedEventArgs e)
        {
            TabWork = sender;
        }

        //Bắt sự kiện và load lại Danh sách phiếu yêu cầu cls
        public void Handle(ReLoadListPCLRequest message)
        {
            if (message != null)
            {
                SetValueListPCLRequest_Common();
            }
        }

        private void SetValueListPCLRequest_Common()
        {
            
        }
        //Bắt sự kiện và load lại Danh sách phiếu yêu cầu cls

        public void Handle(DbClickPtPCLReqExtEdit<PatientPCLRequest_Ext, string> message)
        {
            if (this.GetView() != null)
            {
                if (message != null && message.ObjB == eHCMSResources.Z0055_G1_Edit)
                {
                    ((TabItem)TabEdit).IsSelected = true;
                }
            }
        }

        public void Handle(DbClickPtPCLReqExtDo<PatientPCLRequest_Ext,PatientPCLRequestDetail_Ext, string> message)
        {
            if (this.GetView() != null)
            {
                ((TabItem)TabWork).IsSelected = true;
            }
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            mUCPatientPCLRequestNew = Globals.CheckOperation(Globals.listRefModule
                , (int)eModules.mConsultation, (int)eConsultation.mPtPCLRequest
                , (int)oConsultationEx.mPCL_TaoPhieuMoi_Them);
            mUCPatientPCLRequestList = Globals.CheckOperation(Globals.listRefModule
                , (int)eModules.mConsultation, (int)eConsultation.mPtPCLRequest
                , (int)oConsultationEx.mPCL_DSPhieuYeuCau_ThongTin);
            mUCPatientPCLRequestEdit = Globals.CheckOperation(Globals.listRefModule
                , (int)eModules.mConsultation, (int)eConsultation.mPtPCLRequest
                , (int)oConsultationEx.mPCL_XemSuaPhieuYeuCau_ThongTin);
        }

        #region account checking
        private bool _mUCPatientPCLRequestNew = true;
        private bool _mUCPatientPCLRequestList = true;
        private bool _mUCPatientPCLRequestEdit = true;

        public bool mUCPatientPCLRequestNew
        {
            get
            {
                return _mUCPatientPCLRequestNew;
            }
            set
            {
                if (_mUCPatientPCLRequestNew == value)
                    return;
                _mUCPatientPCLRequestNew = value;
                NotifyOfPropertyChange(() => mUCPatientPCLRequestNew);
            }
        }

        public bool mUCPatientPCLRequestList
        {
            get
            {
                return _mUCPatientPCLRequestList;
            }
            set
            {
                if (_mUCPatientPCLRequestList == value)
                    return;
                _mUCPatientPCLRequestList = value;
                NotifyOfPropertyChange(() => mUCPatientPCLRequestList);
            }
        }

        public bool mUCPatientPCLRequestEdit
        {
            get
            {
                return _mUCPatientPCLRequestEdit;
            }
            set
            {
                if (_mUCPatientPCLRequestEdit == value)
                    return;
                _mUCPatientPCLRequestEdit = value;
                NotifyOfPropertyChange(() => mUCPatientPCLRequestEdit);
            }
        }

        #endregion

        public void Handle(ShowPatientInfo_KHAMBENH_CLS_NGOAIVIEN_HINHANH<Patient, PatientRegistration, PatientRegistrationDetail> message)
        {
            if (message != null)
            {
                if (Registration_DataStorage.CurrentPatient != null)
                {
                    //SearchCriteria.PatientID = Registration_DataStorage.CurrentPatient != null ? Registration_DataStorage.CurrentPatient.PatientID : 0;
                    //Coroutine.BeginExecute(DoPatientPCLRequest_SearchPaging(ObjPatientPCLRequest_SearchPaging.PageIndex, ObjPatientPCLRequest_SearchPaging.PageSize, true));
                }
            }
        }

        private void BeginGetPCLRequestListExtByRegistrationID(long PtRegistrationID)
        {
            IsLoading = true;
            Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = eHCMSResources.K3035_G1_DSPh });

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetPCLRequestListExtByRegistrationID(PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                        {
                            allPatientPCLRequest_Ext = client.EndGetPCLRequestListExtByRegistrationID(asyncResult).ToObservableCollection();

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    Globals.IsBusy = false;
                    IsLoading = false;
                }
            });
            t.Start();
        }

        private bool _IsShowSummaryContent = true;
        public bool IsShowSummaryContent
        {
            get => _IsShowSummaryContent; set
            {
                _IsShowSummaryContent = value;
                NotifyOfPropertyChange(() => IsShowSummaryContent);
            }
        }
        private IRegistration_DataStorage _Registration_DataStorage;
        public IRegistration_DataStorage Registration_DataStorage
        {
            get
            {
                return _Registration_DataStorage;
            }
            set
            {
                if (_Registration_DataStorage == value)
                {
                    return;
                }
                _Registration_DataStorage = value;
                NotifyOfPropertyChange(() => Registration_DataStorage);
                UCPatientPCLRequestExtEdit.Registration_DataStorage = Registration_DataStorage;
                UCPatientPCLRequestEdit.Registration_DataStorage = Registration_DataStorage;
                (UCPatientPCLRequestList as IListPCLRequestExt).Registration_DataStorage = Registration_DataStorage;
            }
        }
    }
}