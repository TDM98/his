//using System;
//using System.Windows;
//using System.ComponentModel.Composition;
//using Caliburn.Micro;
//using aEMR.ViewContracts;
//using System.Windows.Media.Imaging;
//using System.IO;
//using aEMR.Infrastructure;
//using eHCMSLanguage;
//using System.Threading;
//using aEMR.ServiceClient;
//using DataEntities;
//using aEMR.Infrastructure.Events;
//using Castle.Windsor;
//using System.Windows.Controls;
//using aEMR.Common.BaseModel;

//namespace aEMR.ConsultantEPrescription.ViewModels
//{
//    [Export(typeof(ISurgicalReport)), PartCreationPolicy(CreationPolicy.NonShared)]
//    public class SurgicalReportViewModel : ViewModelBase, ISurgicalReport
//        , IHandle<ShowPatientInfo_KHAMBENH_THONGTINCHUNG<Patient, PatientRegistration, PatientRegistrationDetail>>
//    {
//        private WriteableBitmap _ObjBitmapImage;
//        public WriteableBitmap ObjBitmapImage
//        {
//            get { return _ObjBitmapImage; }
//            set
//            {
//                if (_ObjBitmapImage != value)
//                {
//                    _ObjBitmapImage = value;
//                    NotifyOfPropertyChange(() => ObjBitmapImage);
//                }
//            }
//        }

//        private ISearchPatientAndRegistration _searchRegistrationContent;
//        public ISearchPatientAndRegistration SearchRegistrationContent
//        {
//            get { return _searchRegistrationContent; }
//            set
//            {
//                _searchRegistrationContent = value;
//                NotifyOfPropertyChange(() => SearchRegistrationContent);
//            }
//        }

//        private IPatientSummaryInfoV2 _patientSummaryInfoContent;
//        public IPatientSummaryInfoV2 PatientSummaryInfoContent
//        {
//            get { return _patientSummaryInfoContent; }
//            set
//            {
//                _patientSummaryInfoContent = value;
//                NotifyOfPropertyChange(() => PatientSummaryInfoContent);
//            }
//        }

//        private AllLookupValues.PatientFindBy _PatientFindBy;
//        public AllLookupValues.PatientFindBy PatientFindBy
//        {
//            get
//            {
//                return _PatientFindBy;
//            }
//            set
//            {
//                if (_PatientFindBy != value)
//                {
//                    _PatientFindBy = value;
//                    NotifyOfPropertyChange(() => PatientFindBy);
//                }
//            }
//        }

//        protected override void OnActivate()
//        {
//            base.OnActivate();
//            Globals.EventAggregator.Subscribe(this);
//        }
//        IEventAggregator _eventArg;
//        [ImportingConstructor]
//        public SurgicalReportViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
//        {
//            _eventArg = eventArg;
//            SearchRegistrationContent = Globals.GetViewModel<ISearchPatientAndRegistration>();
//            UCHeaderInfoPMR = Globals.GetViewModel<IPatientMedicalRecords_ByPatientID>();
//            SearchRegistrationContent.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
//            SearchRegistrationContent.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);
//            SearchRegistrationContent.CloseRegistrationFormWhenCompleteSelection = false;
//            SearchRegistrationContent.IsSearchGoToKhamBenh = true;
//            SearchRegistrationContent.PatientFindByVisibility = true;
//            SearchRegistrationContent.CanSearhRegAllDept = true;
//            SearchRegistrationContent.PatientFindBy = PatientFindBy;

//            if (PatientFindBy == AllLookupValues.PatientFindBy.NOITRU)
//            {
//                SearchRegistrationContent.SearchAdmittedInPtRegOnly = true;
//            }
//            SearchRegistrationContent.IsAllowSearchingPtByName_Visible = true;
//            SearchRegistrationContent.IsSearchPtByNameChecked = false;
//            SearchRegistrationContent.IsSearchByRegistrationDetails = true;

//            Globals.PatientFindBy_ForConsultation = PatientFindBy;
//            SearchRegistrationContent.mTimBN = true;
//            SearchRegistrationContent.mThemBN = true;
//            SearchRegistrationContent.mTimDangKy = true;
//            PatientSummaryInfoContent = Globals.GetViewModel<IPatientSummaryInfoV2>();
//            PatientSummaryInfoContent.mInfo_CapNhatThongTinBN = false;
//            PatientSummaryInfoContent.mInfo_XacNhan = false;
//            PatientSummaryInfoContent.mInfo_XoaThe = false;
//            PatientSummaryInfoContent.mInfo_XemPhongKham = false;
//            PatientSummaryInfoContent.DisplayButtons = false;

//            UCSmallProcedureEdit = Globals.GetViewModel<ISmallProcedureEdit>();
//            UCPatientProfileInfo = Globals.GetViewModel<IPatientInfo>();
//        }
//        private void GetVideoAndImage(string aPath)
//        {
//            this.ShowBusyIndicator();
//            var t = new Thread(() =>
//            {
//                using (var serviceFactory = new CommonService_V2Client())
//                {
//                    var contract = serviceFactory.ServiceInstance;
//                    contract.BeginGetVideoAndImage(aPath, Globals.DispatchCallback((asyncResult) =>
//                    {
//                        try
//                        {
//                            var items = contract.EndGetVideoAndImage(asyncResult);
//                            if (items != null && items.Length > 0)
//                            {
//                                Stream ObjGetVideoAndImage = new MemoryStream(items);
//                                ObjBitmapImage = Globals.GetWriteableBitmapFromStream(ObjGetVideoAndImage);
//                            }
//                            else
//                            {
//                                ObjBitmapImage = null;
//                            }
//                            this.HideBusyIndicator();
//                        }
//                        catch (Exception ex)
//                        {
//                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//                            this.HideBusyIndicator();
//                        }
//                    }), null);
//                }
//            });
//            t.Start();
//        }
        
//        public void Image_LayoutUpdated(object sender, EventArgs e)
//        {
//        }

//        public object UCHeaderInfoPMR
//        {
//            get;
//            set;
//        }

//        public object UCPatientProfileInfo
//        {
//            get;
//            set;
//        }

//        public void ActivateSubVM()
//        {
//            //ActivateItem(UCDoctorProfileInfo);
//            ActivateItem(UCPatientProfileInfo);
//            ActivateItem(UCHeaderInfoPMR);
//            ActivateItem(ucOutPMR);
//            //ActivateItem(ucOutPMRs);
//            //ActivateItem(UCPtRegDetailInfo);
//            ActivateItem(UCSmallProcedureEdit);
//            ActivateItem(SearchRegistrationContent);
//        }

//        private void DeActivateSubVM(bool close)
//        {
//            //DeactivateItem(UCDoctorProfileInfo, close);
//            DeactivateItem(UCPatientProfileInfo, close);
//            DeactivateItem(UCHeaderInfoPMR, close);
//            DeactivateItem(ucOutPMR, close);
//            //DeactivateItem(ucOutPMRs, close);
//            //DeactivateItem(UCPtRegDetailInfo, close);
//            DeactivateItem(UCSmallProcedureEdit, close);
//            DeactivateItem(SearchRegistrationContent, close);
//        }

//        public ISmallProcedureEdit UCSmallProcedureEdit { get; set; }

//        public TabControl TabCommon { get; set; }
//        public void TabCommon_Loaded(object sender, RoutedEventArgs e)
//        {
//            TabCommon = (TabControl)sender;
//            if (!mChanDoan_KhamBenhMoi
//                || IsPopUp)
//                TabCommon.SelectedIndex = 1;
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
//                return _mChanDoan_tabLanKhamTruoc_ThongTin; //&& !IsProcedureEdit;
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
//        public IConsultationOld ucOutPMR
//        {
//            get;
//            set;
//        }

//        long MedSerID;

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

//        public void Handle(ShowPatientInfo_KHAMBENH_THONGTINCHUNG<Patient, PatientRegistration, PatientRegistrationDetail> message)
//        {
//            if (message != null)
//            {
//                if (message.PtRegDetail != null)
//                {
//                    if(((IPatientInfo)UCPatientProfileInfo).CurrentPatient == null)
//                    {
//                        ((IPatientInfo)UCPatientProfileInfo).CurrentPatient = message.Pt;
//                    } else
//                    {
//                        ((IPatientInfo)UCPatientProfileInfo).CurrentPatient.GeneralInfoString = "";
//                    }
//                    ((IPatientInfo)UCPatientProfileInfo).CurrentPatient.GeneralInfoString += string.Format("                                              [{0}]       CĐ sơ bộ: [{1}]", message.PtRegDetail.MedServiceName, message.PtReg.BasicDiagTreatment);
//                    //((IPatientInfo)UCPatientProfileInfo).CurrentRegistration = message.PtReg;
//                    MedSerID = message.PtRegDetail.MedServiceID.GetValueOrDefault();
//                    UCSmallProcedureEdit.GetSmallProcedure(message.PtRegDetail.PtRegDetailID);
//                }
//            }
//        }

//        public void BtnPrintProcedure()
//        {
//            //void onInitDlg(ISurgicalResultReport proAlloc)
//            //{

//            //}
//            ////var instance = proAlloc as Conductor<object>;
//            //GlobalsNAV.ShowDialog<ISurgicalResultReport>(onInitDlg);
//        }
//    }
//}
