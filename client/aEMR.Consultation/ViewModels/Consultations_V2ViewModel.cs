//using System.Windows.Controls;
//using Caliburn.Micro;
//using System.ComponentModel.Composition;
//using aEMR.ViewContracts;
//using aEMR.Infrastructure;
//using aEMR.Infrastructure.Events;
//using System.Windows;
//using DataEntities;
//using aEMR.Common;
//using Castle.Windsor;
//using System.Collections.ObjectModel;
//using aEMR.Common.BaseModel;

//namespace aEMR.ConsultantEPrescription.ViewModels
//{
//    [Export(typeof(IConsultations_V2)), PartCreationPolicy(CreationPolicy.NonShared)]
//    public class Consultations_V2ViewModel : ViewModelBase, IConsultations_V2
//        , IHandle<ConsultationDoubleClickEvent>
//        , IHandle<DiagnosisTreatmentSelectedAndCloseEvent<DiagnosisTreatment>>
//        , IHandle<ConsultationDoubleClickEvent_InPt_1>
//    {
//        private bool _IsPopUp = false;
//        public bool IsPopUp
//        {
//            get
//            {
//                return _IsPopUp;
//            }
//            set
//            {
//                if (_IsPopUp == value)
//                    return;
//                _IsPopUp = value;
//                mChanDoan_tabSuaKhamBenh_ThongTin = mChanDoan_tabSuaKhamBenh_ThongTin && !IsPopUp;
//            }
//        }
//        [ImportingConstructor]
//        public Consultations_V2ViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
//        {
//            authorization();
//            if (IsOutPt)
//                activeControl();
//        }
//        private bool _IsDiagnosisOutHospital;
//        public bool IsDiagnosisOutHospital
//        {
//            get { return _IsDiagnosisOutHospital; }
//            set
//            {
//                if (_IsDiagnosisOutHospital != value)
//                {
//                    _IsDiagnosisOutHospital = value;
//                    NotifyOfPropertyChange(() => IsDiagnosisOutHospital);
//                }
//            }
//        }
//        /*▼====: #001*/
//        private bool _IsDailyDiagnosis;
//        public bool IsDailyDiagnosis
//        {
//            get { return _IsDailyDiagnosis; }
//            set
//            {
//                if (_IsDailyDiagnosis != value)
//                {
//                    _IsDailyDiagnosis = value;
//                    NotifyOfPropertyChange(() => IsDailyDiagnosis);
//                }
//            }
//        }
//        protected override void OnActivate()
//        {
//            base.OnActivate();
//            Globals.EventAggregator.Subscribe(this);
//            this.ActivateItem(UCDoctorProfileInfo);
//            this.ActivateItem(UCPatientProfileInfo);
//            this.ActivateItem(UCHeaderInfoPMR);
//            this.ActivateItem(ucOutPMR);
//            this.ActivateItem(ucOutPMRs);
//            this.ActivateItem(UCPtRegDetailInfo);
//            if (!IsOutPt && ucOutPMR != null)
//            {
//                ucOutPMR.IsDiagnosisOutHospital = IsDiagnosisOutHospital;
//                ucOutPMR.IsDailyDiagnosis = this.IsDailyDiagnosis;
//                ucOutPMR.InitPatientInfo();
//            }
//        }
//        protected override void OnDeactivate(bool close)
//        {
//            Globals.EventAggregator.Unsubscribe(ucOutPMR);
//            Globals.EventAggregator.Unsubscribe(this);
//            base.OnDeactivate(close);
//        }
//        public void activeControl()
//        {
//            UCDoctorProfileInfo = Globals.GetViewModel<ILoginInfo>();
//            UCPatientProfileInfo = Globals.GetViewModel<IPatientInfo>();
//            UCHeaderInfoPMR = Globals.GetViewModel<IPatientMedicalRecords_ByPatientID>();
//            ucOutPMR = Globals.GetViewModel<IConsultationOld_V2>();
//            ucOutPMRs = Globals.GetViewModel<IConsultationList_V2>();
//            UCPtRegDetailInfo = Globals.GetViewModel<IPtRegDetailInfo>();
//        }
//        public IPatientMedicalRecords_ByPatientID UCHeaderInfoPMR
//        {
//            get;
//            set;
//        }

//        public ILoginInfo UCDoctorProfileInfo
//        {
//            get;
//            set;
//        }
//        public IPatientInfo UCPatientProfileInfo
//        {
//            get;
//            set;
//        }

//        public TabControl tabCommon { get; set; }
//        public void tabCommon_Loaded(object sender, RoutedEventArgs e)
//        {
//            tabCommon = (TabControl)sender;
//            if (!mChanDoan_KhamBenhMoi || IsPopUp)
//                tabCommon.SelectedIndex = 1;
//        }

//        public void authorization()
//        {
//            if (!Globals.isAccountCheck)
//            {
//                return;
//            }

//            mChanDoan_KhamBenhMoi = Globals.CheckOperation(Globals.listRefModule
//                                        , (int)eModules.mConsultation
//                                        , (int)eConsultation.mPtPMRConsultationNew
//                                        , (int)oConsultationEx.mChanDoan_KhamBenhMoi);

//            mChanDoan_tabLanKhamTruoc_ThongTin = Globals.CheckOperation(Globals.listRefModule
//                                        , (int)eModules.mConsultation
//                                        , (int)eConsultation.mPtPMRConsultationNew
//                                        , (int)oConsultationEx.mChanDoan_tabLanKhamTruoc_ThongTin);
//            mChanDoan_tabSuaKhamBenh_ThongTin = Globals.CheckOperation(Globals.listRefModule
//                                        , (int)eModules.mConsultation
//                                        , (int)eConsultation.mPtPMRConsultationNew
//                                        , (int)oConsultationEx.mChanDoan_tabSuaKhamBenh_ThongTin);
//        }
//        #region account checking

//        private bool _mChanDoan_KhamBenhMoi = true;
//        private bool _mChanDoan_tabLanKhamTruoc_ThongTin = true;
//        private bool _mChanDoan_tabSuaKhamBenh_ThongTin = true;
//        public bool mChanDoan_KhamBenhMoi
//        {
//            get
//            {
//                return _mChanDoan_KhamBenhMoi;
//            }
//            set
//            {
//                if (_mChanDoan_KhamBenhMoi == value)
//                    return;
//                _mChanDoan_KhamBenhMoi = value;
//            }
//        }
//        public bool mChanDoan_tabLanKhamTruoc_ThongTin
//        {
//            get
//            {
//                return _mChanDoan_tabLanKhamTruoc_ThongTin;
//            }
//            set
//            {
//                if (_mChanDoan_tabLanKhamTruoc_ThongTin == value)
//                    return;
//                _mChanDoan_tabLanKhamTruoc_ThongTin = value;
//            }
//        }
//        public bool mChanDoan_tabSuaKhamBenh_ThongTin
//        {
//            get
//            {
//                return _mChanDoan_tabSuaKhamBenh_ThongTin;
//            }
//            set
//            {
//                if (_mChanDoan_tabSuaKhamBenh_ThongTin == value)
//                    return;
//                _mChanDoan_tabSuaKhamBenh_ThongTin = value;

//            }
//        }

//        private bool _bucOutPMR = true;
//        private bool _bucOutPMREditor = true;
//        private bool _bucOutPMRs = true;
//        public bool bucOutPMR
//        {
//            get
//            {
//                return _bucOutPMR;
//            }
//            set
//            {
//                if (_bucOutPMR == value)
//                    return;
//                _bucOutPMR = value;
//            }
//        }
//        public bool bucOutPMREditor
//        {
//            get
//            {
//                return _bucOutPMREditor;
//            }
//            set
//            {
//                if (_bucOutPMREditor == value)
//                    return;
//                _bucOutPMREditor = value;
//            }
//        }
//        public bool bucOutPMRs
//        {
//            get
//            {
//                return _bucOutPMRs;
//            }
//            set
//            {
//                if (_bucOutPMRs == value)
//                    return;
//                _bucOutPMRs = value;
//            }
//        }
//        #endregion
//        public IConsultationOld_V2 ucOutPMR { get; set; }
//        public object ucOutPMREditor
//        {
//            get;
//            set;
//        }
//        public IConsultationList_V2 ucOutPMRs { get; set; }


//        public IPtRegDetailInfo UCPtRegDetailInfo
//        {
//            get;
//            set;
//        }


//        public object gTabEdit
//        {
//            get;
//            set;
//        }
//        public void TabEdit_Loaded(object sender, RoutedEventArgs e)
//        {
//            gTabEdit = sender;
//        }

//        #region IHandle<DoubleClickEvent> Members
//        public void Handle(ConsultationDoubleClickEvent message)
//        {
//            //if (message != null && this.IsActive) test01
//            if (message != null)
//            {
//                ((TabItem)gTabEdit).IsSelected = true;
//            }
//        }
//        public void Handle(ConsultationDoubleClickEvent_InPt_1 message)
//        {
//            if (message != null)
//            {
//                ((TabItem)gTabEdit).IsSelected = true;
//            }
//        }
//        #endregion


//        #region IHandle<ConsultationChooseServiceRecIDEvent> Members

//        //public void Handle(ConsultationChooseServiceRecIDEvent message)
//        //{
//        //    if (message != null && this.IsActive)
//        //    {
//        //        TryClose();
//        //        Globals.EventAggregator.Publish(new ConsultationParentChooseServiceRecIDEvent { DiagTrmtItem = message.DiagTrmtItem });
//        //    }
//        //}

//        #endregion

//        #region IHandle<CloseConsultationNewEvent> Members
//        //public void Handle(CloseConsultationNewEvent message)
//        //{
//        //    if (message != null && this.IsActive)
//        //    {
//        //        TryClose();
//        //        Globals.EventAggregator.Publish(new ConsultationParentChooseServiceRecIDEvent { DiagTrmtItem = message.DiagTrmtItem });
//        //    }
//        //}
//        #endregion

//        //Chọn 1 chẩn đoán và đóng Window Chẩn Đoán lại
//        public void Handle(DiagnosisTreatmentSelectedAndCloseEvent<DiagnosisTreatment> message)
//        {
//            if (this.GetView() != null)
//            {
//                if (message != null)
//                {
//                    //Phát chọn 1 chẩn đoán và đóng window

//                    TryClose();
//                    Globals.EventAggregator.Publish(new DiagnosisTreatmentSelectedEvent<DiagnosisTreatment> { DiagnosisTreatment = message.DiagnosisTreatment.DeepCopy() });
//                    Globals.EventAggregator.Unsubscribe(this);
//                }
//            }
//        }
//        public void InitPatientInfo()
//        {
//            ucOutPMR.InitPatientInfo();
//            ucOutPMRs.InitPatientInfo();
//        }
//        public bool CheckValidDiagnosis()
//        {
//            return ucOutPMR.CheckValidDiagnosis();
//        }
//        public ObservableCollection<DiagnosisIcd10Items> refIDC10List
//        {
//            get
//            {
//                return ucOutPMR.refIDC10List;
//            }
//            set
//            {
//                ucOutPMR.refIDC10List = value;
//            }
//        }
//        public long Compare2Object()
//        {
//            return ucOutPMR.Compare2Object();
//        }
//        public DiagnosisTreatment DiagTrmtItem
//        {
//            get
//            {
//                return ucOutPMR.DiagTrmtItem;
//            }
//            set
//            {
//                ucOutPMR.DiagTrmtItem = value;
//            }
//        }
//        public void ChangeStatesAfterUpdated(bool IsUpdate = false)
//        {
//            ucOutPMR.ChangeStatesAfterUpdated(IsUpdate);
//        }

//        public bool IsShowSummaryContent
//        {
//            get
//            {
//                return ucOutPMR.IsShowSummaryContent;
//            }
//            set
//            {
//                ucOutPMR.IsShowSummaryContent = value;
//            }
//        }
        
//        public bool IsOutPt
//        {
//            get { return Globals.PatientAllDetails.PtRegistrationInfo == null || Globals.PatientAllDetails.PtRegistrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU; }
//        }

//        public bool btUpdateIsEnabled
//        {
//            get { return ucOutPMR.btUpdateIsEnabled; }
//        }
//        public bool btSaveCreateNewIsEnabled
//        {
//            get { return ucOutPMR.btSaveCreateNewIsEnabled; }
//        }
//    }
//}