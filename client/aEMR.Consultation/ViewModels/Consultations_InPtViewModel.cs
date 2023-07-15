using System.Windows.Controls;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using System.Windows;
using DataEntities;
using Castle.Windsor;
using aEMR.Common.BaseModel;
using aEMR.Common;
using eHCMSLanguage;
using System;
using System.Linq;
using System.Collections.Generic;
using aEMR.Common.ViewModels;
using aEMR.CommonTasks;
using System.Threading;
using System.Collections.ObjectModel;
using aEMR.ServiceClient;
using aEMR.ServiceClient.Consultation_PCLs;
using System.Text;
/*
* 20171124 #001 CMN: Added Daily diagnostic
* 20181022 #002 TTM: BM0003214: Fix lỗi không hiển thị thông tin trên PatientInfo cho khám bệnh nội trú
* 20181030 #003 TTM: Fix lỗi luôn hiển thị xuất khoa mặc dù chọn chẩn đoán hàng ngày, xuất viện.
* 20181121 #004 TTM: BM 0005257: Tạo mới Out standing task nội trú.
* 20200206 #005 TTM: BM 0022883: Fix lỗi liên quan đến tường trình phẫu thuật nội trú, bổ sung nút xem in tường trình phẫu thuật nội trú.
* 20230404 #006 QTD: Thêm mã máy
* 20230510 #007 QTD: Thêm tab Vật tư kèm DVKT
*/
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IConsultations_InPt)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class Consultations_InPtViewModel : ViewModelBase, IConsultations_InPt
        , IHandle<ConsultationDoubleClickEvent_InPt_1>
        , IHandle<SetInPatientInfoAndRegistrationForConsultations_InPt>
        , IHandle<LoadDataForHtmlEditor>
    {
        private bool _IsPopUp = false;
        public bool IsPopUp
        {
            get
            {
                return _IsPopUp;
            }
            set
            {
                if (_IsPopUp == value)
                    return;
                _IsPopUp = value;
                mChanDoan_tabSuaKhamBenh_ThongTin = mChanDoan_tabSuaKhamBenh_ThongTin && !IsPopUp;
            }
        }
        private bool _IsShowSummaryContent = true;
        public bool IsShowSummaryContent
        {
            get
            {
                return _IsShowSummaryContent;
            }
            set
            {
                if (_IsShowSummaryContent == value)
                {
                    return;
                }
                _IsShowSummaryContent = value;
                NotifyOfPropertyChange(() => IsShowSummaryContent);
            }
        }
        [ImportingConstructor]
        public Consultations_InPtViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            //Globals.EventAggregator.Subscribe(this);

            authorization();
            //▼====== #002
            CreateSubVM();
            //▲====== #002
        }

        private bool _IsDiagnosisOutHospital;
        public bool IsDiagnosisOutHospital
        {
            get { return _IsDiagnosisOutHospital; }
            set
            {
                if (_IsDiagnosisOutHospital != value)
                {
                    _IsDiagnosisOutHospital = value;
                    //▼====== #003
                    if (ucOutPMR != null)
                    {
                        ucOutPMR.IsDiagnosisOutHospital = value;
                    }
                    //▲====== #003
                    NotifyOfPropertyChange(() => IsDiagnosisOutHospital);
                }
            }
        }
        /*▼====: #001*/
        private bool _IsDailyDiagnosis;
        public bool IsDailyDiagnosis
        {
            get { return _IsDailyDiagnosis; }
            set
            {
                if (_IsDailyDiagnosis != value)
                {
                    _IsDailyDiagnosis = value;
                    //▼====== #003
                    if (ucOutPMR != null)
                    {
                        ucOutPMR.IsDailyDiagnosis = value;
                    }
                    //▲====== #003
                    NotifyOfPropertyChange(() => IsDailyDiagnosis);
                }
            }
        }
        public bool IsProcedureEdit
        {
            get
            {
                return ucOutPMR != null ? ucOutPMR.IsProcedureEdit : false;
            }
            set
            {
                if (ucOutPMR != null)
                {
                    ucOutPMR.IsProcedureEdit = value;
                    NotifyOfPropertyChange(() => IsProcedureEdit);
                    NotifyOfPropertyChange(() => mChanDoan_tabLanKhamTruoc_ThongTin);
                    NotifyOfPropertyChange(() => IsVisibleSearchingContent);
                }
            }
        }
        /*▲====: #001*/
        private bool _IsPhysicalTherapy = false;
        public bool IsPhysicalTherapy
        {
            get
            {
                return _IsPhysicalTherapy;
            }
            set
            {
                if (_IsPhysicalTherapy == value)
                {
                    return;
                }
                _IsPhysicalTherapy = value;
                NotifyOfPropertyChange(() => IsPhysicalTherapy);
                NotifyOfPropertyChange(() => IsVisibleSearchingContent);
                NotifyOfPropertyChange(() => mChanDoan_tabLanKhamTruoc_ThongTin);
            }
        }
        public bool IsVisibleSearchingContent
        {
            get
            {
                return IsProcedureEdit || IsPhysicalTherapy;
            }
        }
        //▼====== #002
        protected override void OnActivate()
        {
            ActivateSubVM();
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            //▼======: 19112018 TTM: Dời từ Hàm khởi tạo xuống đây, vì nếu đặt ở hàm khởi tạo sẽ sai do sẽ chạy Init trước khi set giá trị cho 2 biến IsDiagnosisOutHospital và IsDailyDiagnosis
            //ucOutPMR.InitPatientInfo();
            //▲======:
            UCSearchRegistration.IsProcedureEdit = IsProcedureEdit;
            //▼====== #004:
            //20190404 TTM: Lý do thêm điều kiện: Vì màn hình thủ thuật đang được làm chắp vá => Lấy dịch vụ từ PatientRegistrationDetails_Inpt ra để thực hiện thủ thuật.
            //              Còn OutStandingTask lại lấy đăng ký ra để thực hiện => Sai => Màn hình thủ thuật không được phép có out standing task.
            if (!IsProcedureEdit)
            {
                if (IsShowSummaryContent)
                {
                    var homevm = Globals.GetViewModel<IHome>();
                    IInPatientOutstandingTask ostvm = Globals.GetViewModel<IInPatientOutstandingTask>();
                    ostvm.WhichVM = SetOutStandingTask.KHAMBENH;
                    homevm.OutstandingTaskContent = ostvm;
                    homevm.IsExpandOST = true;
                }
                //if (Globals.ServerConfigSection.ConsultationElements.UseOnlyDailyDiagnosis)
                //{
                //    DiagnosisTreatmentTree = Globals.GetViewModel<IDiagnosisTreatmentTree>();
                //    homevm.LeftMenu = DiagnosisTreatmentTree;
                //    ActivateItem(DiagnosisTreatmentTree);
                //    homevm.IsEnableLeftMenu = true;
                //    homevm.IsExpandLeftMenu = true;
                //    if (Globals.PatientAllDetails.PtRegistrationInfo != null
                //        && Globals.PatientAllDetails.PtRegistrationInfo.PtRegistrationID > 0)
                //    {
                //        DiagnosisTreatmentTree.LoadData(Globals.PatientAllDetails.PtRegistrationInfo.PtRegistrationID);
                //    }
                //}
            }
            else
            {
                //20190406 TTM: Bổ sung thêm code này
                //              Lý do: Việc gán PatientFindBy nằm trong hàm của OutStandingTask => Không chạy đc => Không gán đc nên màn hình nào nội trú không xài sẽ gán ở đây.
                Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NOITRU;
            }
            //▲====== #004:
        }
        //public IDiagnosisTreatmentTree DiagnosisTreatmentTree { get; set; }
        protected override void OnDeactivate(bool close)
        {
            DeActivateSubVM(close);
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
            //▼====== #004:
            var homevm = Globals.GetViewModel<IHome>();
            homevm.OutstandingTaskContent = null;
            homevm.IsEnableLeftMenu = false;
            homevm.IsExpandLeftMenu = false;
            homevm.IsExpandOST = false;
            //▲====== #004:
        }

        public void CreateSubVM()
        {
            UCDoctorProfileInfo = Globals.GetViewModel<ILoginInfo>();
            UCPatientProfileInfo = Globals.GetViewModel<IPatientInfo>();
            UCHeaderInfoPMR = Globals.GetViewModel<IPatientMedicalRecords_ByPatientID>();
            UCPtRegDetailInfo = Globals.GetViewModel<IPtRegDetailInfo>();
            ucOutPMR = Globals.GetViewModel<IConsultationOld_InPt>();
            ucOutPMRs = Globals.GetViewModel<IConsultationList_InPt>();
            UCSmallProcedureEdit = Globals.GetViewModel<ISmallProcedureEdit>();
            UCSmallProcedureEdit.bConfirmVisi = false;
            //▼===== 20200409 TTM: IsFromOutOrInDiag để phân biệt xem màn hình thủ thuật đang xuất phát từ ngoại trú hay nội trú.
            //                     True là nội trú, False là ngoại trú.
            UCSmallProcedureEdit.IsFromOutOrInDiag = true;
            //▲=====
            UCSearchRegistration = Globals.GetViewModel<ISearchPatientAndRegistration>();
            UCSearchRegistration.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;
            UCSearchRegistration.CloseRegistrationFormWhenCompleteSelection = false;
            UCSearchRegistration.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
            UCSearchRegistration.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);
            UCSearchRegistration.IsSearchGoToKhamBenh = true;
            UCSearchRegistration.PatientFindByVisibility = true;
            UCSearchRegistration.CanSearhRegAllDept = true;
            UCSearchRegistration.SearchAdmittedInPtRegOnly = true;
            if (!Globals.ServerConfigSection.ConsultationElements.IsAllowSearchingPtByName)
            {
                UCSearchRegistration.IsAllowSearchingPtByName_Visible = true;
                UCSearchRegistration.IsSearchPtByNameChecked = false;
            }
            UCSearchRegistration.IsSearchByRegistrationDetails = true;
            ucOutPMR.gICD10Changed += (aICD10Collection) =>
            {
                if (aICD10Collection == null || !aICD10Collection.Any(x => x.IsMain))
                {
                    return;
                }
                if (UCSmallProcedureEdit != null && UCSmallProcedureEdit.SmallProcedureObj != null)
                {
                    var CurrentICD = aICD10Collection.First(x => x.IsMain).DiseasesReference.DeepCopy();
                    var aBeforeDiagTreatment = CurrentICD.DiseaseNameVN;
                    CurrentICD.DiseaseNameVN = ucOutPMR.DiagTrmtItem.DiagnosisFinal;
                    UCSmallProcedureEdit.SmallProcedureObj.Diagnosis = CurrentICD.DiseaseNameVN;
                    UCSmallProcedureEdit.SmallProcedureObj.BeforeICD10 = CurrentICD;
                    UCSmallProcedureEdit.CallNotifyOfPropertyChange(aBeforeDiagTreatment);
                }
            };
            UCSmallProcedureDesc = Globals.GetViewModel<IHtmlEditor>();
            UCClinicDeptInPtReqFormSmallProcedure = Globals.GetViewModel<IClinicDeptInPtReqForm_ForSmallProcedure>();
        }
        public void ActivateSubVM()
        {
            if (UCDoctorProfileInfo != null)
            {
                ActivateItem(UCDoctorProfileInfo);
                ActivateItem(UCPatientProfileInfo);
                ActivateItem(UCHeaderInfoPMR);
                ActivateItem(UCPtRegDetailInfo);
            }
            ActivateItem(ucOutPMR);
            ActivateItem(ucOutPMRs);
            ActivateItem(UCSmallProcedureEdit);
            ActivateItem(UCSearchRegistration);
            ActivateItem(UCClinicDeptInPtReqFormSmallProcedure);
        }
        private void DeActivateSubVM(bool close)
        {
            if (UCDoctorProfileInfo != null)
            {
                DeactivateItem(UCDoctorProfileInfo, close);
                DeactivateItem(UCPatientProfileInfo, close);
                DeactivateItem(UCHeaderInfoPMR, close);
                DeactivateItem(UCPtRegDetailInfo, close);
            }
            DeactivateItem(ucOutPMR, close);
            DeactivateItem(ucOutPMRs, close);
            DeactivateItem(UCSmallProcedureEdit, close);
            DeactivateItem(UCSearchRegistration, close);
            DeactivateItem(UCClinicDeptInPtReqFormSmallProcedure, close);
        }
        //▲====== #002

        //▼====== #002: sử dụng CreateSubVM nên comment hàm này lại
        //public void activeControl()
        //{
        //    UCDoctorProfileInfo = Globals.GetViewModel<ILoginInfo>();
        //    UCPatientProfileInfo  = Globals.GetViewModel<IPatientInfo>();
        //    //UC Header PMR
        //    UCHeaderInfoPMR  = Globals.GetViewModel<IPatientMedicalRecords_ByPatientID>();
        //    //UC Header PMR
        //    ucOutPMR = Globals.GetViewModel<IConsultationOld_InPt>();
        //    ucOutPMR.IsDiagnosisOutHospital = IsDiagnosisOutHospital;
        //    /*▼====: #001*/
        //    ucOutPMR.IsDailyDiagnosis = this.IsDailyDiagnosis;
        //    /*▲====: #001*/
        //    ucOutPMR.InitPatientInfo();
        //    ucOutPMRs = Globals.GetViewModel<IConsultationList_InPt>();
        //    UCPtRegDetailInfo = Globals.GetViewModel<IPtRegDetailInfo>();
        //}
        //▲====== #002
        public IPatientMedicalRecords_ByPatientID UCHeaderInfoPMR { get; set; }
        public ILoginInfo UCDoctorProfileInfo { get; set; }
        public IPatientInfo UCPatientProfileInfo { get; set; }
        public TabControl tabCommon { get; set; }
        public void tabCommon_Loaded(object sender, RoutedEventArgs e)
        {
            tabCommon = (TabControl)sender;
            if (!mChanDoan_KhamBenhMoi
                || IsPopUp)
                tabCommon.SelectedIndex = 1;
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

            mChanDoan_KhamBenhMoi = Globals.CheckOperation(Globals.listRefModule
                                        , (int)eModules.mConsultation
                                        , (int)eConsultation.mPtPMRConsultationNew
                                        , (int)oConsultationEx.mChanDoan_KhamBenhMoi);

            mChanDoan_tabLanKhamTruoc_ThongTin = Globals.CheckOperation(Globals.listRefModule
                                        , (int)eModules.mConsultation
                                        , (int)eConsultation.mPtPMRConsultationNew
                                        , (int)oConsultationEx.mChanDoan_tabLanKhamTruoc_ThongTin);
            mChanDoan_tabSuaKhamBenh_ThongTin = Globals.CheckOperation(Globals.listRefModule
                                        , (int)eModules.mConsultation
                                        , (int)eConsultation.mPtPMRConsultationNew
                                        , (int)oConsultationEx.mChanDoan_tabSuaKhamBenh_ThongTin);
        }
        #region account checking

        private bool _mChanDoan_KhamBenhMoi = true;
        private bool _mChanDoan_tabLanKhamTruoc_ThongTin = true;
        private bool _mChanDoan_tabSuaKhamBenh_ThongTin = true;
        public bool mChanDoan_KhamBenhMoi
        {
            get
            {
                return _mChanDoan_KhamBenhMoi;
            }
            set
            {
                if (_mChanDoan_KhamBenhMoi == value)
                    return;
                _mChanDoan_KhamBenhMoi = value;
            }
        }
        public bool mChanDoan_tabLanKhamTruoc_ThongTin
        {
            get
            {
                return _mChanDoan_tabLanKhamTruoc_ThongTin && !IsProcedureEdit && !IsPhysicalTherapy;
            }
            set
            {
                if (_mChanDoan_tabLanKhamTruoc_ThongTin == value)
                    return;
                _mChanDoan_tabLanKhamTruoc_ThongTin = value;
            }
        }
        public bool mChanDoan_tabSuaKhamBenh_ThongTin
        {
            get
            {
                return _mChanDoan_tabSuaKhamBenh_ThongTin;
            }
            set
            {
                if (_mChanDoan_tabSuaKhamBenh_ThongTin == value)
                    return;
                _mChanDoan_tabSuaKhamBenh_ThongTin = value;

            }
        }

        private bool _bucOutPMR = true;
        private bool _bucOutPMREditor = true;
        private bool _bucOutPMRs = true;
        public bool bucOutPMR
        {
            get
            {
                return _bucOutPMR;
            }
            set
            {
                if (_bucOutPMR == value)
                    return;
                _bucOutPMR = value;
            }
        }
        public bool bucOutPMREditor
        {
            get
            {
                return _bucOutPMREditor;
            }
            set
            {
                if (_bucOutPMREditor == value)
                    return;
                _bucOutPMREditor = value;
            }
        }
        public bool bucOutPMRs
        {
            get
            {
                return _bucOutPMRs;
            }
            set
            {
                if (_bucOutPMRs == value)
                    return;
                _bucOutPMRs = value;
            }
        }
        #endregion
        public IConsultationOld_InPt ucOutPMR
        {
            get;
            set;
        }
        public object ucOutPMREditor
        {
            get;
            set;
        }
        public IConsultationList_InPt ucOutPMRs
        {
            get;
            set;
        }
        private IHtmlEditor _UCSmallProcedureDesc;
        public IHtmlEditor UCSmallProcedureDesc
        {
            get
            {
                return _UCSmallProcedureDesc;
            }
            set
            {
                if (_UCSmallProcedureDesc == value)
                {
                    return;
                }
                _UCSmallProcedureDesc = value;
                NotifyOfPropertyChange(() => UCSmallProcedureDesc);
            }
        }

        public ISmallProcedureEdit UCSmallProcedureEdit { get; set; }
        public ISearchPatientAndRegistration UCSearchRegistration { get; set; }
        public IPtRegDetailInfo UCPtRegDetailInfo { get; set; }
        private bool _IsUpdateDiagConfirmInPT;
        public bool IsUpdateDiagConfirmInPT
        {
            get { return _IsUpdateDiagConfirmInPT; }
            set
            {
                if (_IsUpdateDiagConfirmInPT = value)
                {
                    _IsUpdateDiagConfirmInPT = value;
                    ucOutPMR.IsUpdateDiagConfirmInPT = IsUpdateDiagConfirmInPT;
                    NotifyOfPropertyChange(() => IsUpdateDiagConfirmInPT);
                }
            }
        }
        public object TabEdit { get; set; }
        public void TabEdit_Loaded(object sender, RoutedEventArgs e)
        {
            TabEdit = sender;
        }
        public void MedicalFileInfoCmd()
        {
            if (Registration_DataStorage == null || Registration_DataStorage.CurrentPatientRegistration == null || Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID == 0 ||
                UCSmallProcedureEdit.SmallProcedureObj == null ||
                UCSmallProcedureEdit.SmallProcedureObj.PtRegDetailID == 0)
            {
                return;
            }
            ISpecialistTypeSelect DialogView = Globals.GetViewModel<ISpecialistTypeSelect>();
            GlobalsNAV.ShowDialog_V3(DialogView);
            if (DialogView.ConfirmedSpecialistType == null ||
                (DialogView.ConfirmedSpecialistType.LookupID != (long)AllLookupValues.V_SpecialistType.San && DialogView.ConfirmedSpecialistType.LookupID != (long)AllLookupValues.V_SpecialistType.Nhi))
            {
                return;
            }
            GlobalsNAV.ShowDialog<ICommonPreviewView>((aView) =>
            {
                if (DialogView.ConfirmedSpecialistType.LookupID == (long)AllLookupValues.V_SpecialistType.Nhi)
                {
                    aView.eItem = ReportName.PediatricsMedicalFile;
                }
                else
                {
                    aView.eItem = ReportName.ObstetricsMedicalFile;
                }
                aView.RegistrationDetailID = UCSmallProcedureEdit.SmallProcedureObj.PtRegDetailID;
            });
        }
        #region IHandle<DoubleClickEvent> Members
        public void Handle(ConsultationDoubleClickEvent_InPt_1 message)
        {
            if (message != null)
            {
                ((TabItem)TabEdit).IsSelected = true;
            }
        }

        //▼====== #004:
        public void Handle(SetInPatientInfoAndRegistrationForConsultations_InPt message)
        {
            if (message != null)
            {
                ucOutPMR.InitPatientInfo();
                if (ucOutPMR.DiagTrmtItem != null && ucOutPMR.DiagTrmtItem.PtRegDetailID.HasValue && ucOutPMR.DiagTrmtItem.PtRegDetailID > 0)
                {
                    UCSmallProcedureEdit.GetSmallProcedure(ucOutPMR.DiagTrmtItem.PtRegDetailID.Value, (long)AllLookupValues.RegistrationType.NOI_TRU);
                }
                else
                {
                    UCSmallProcedureEdit.GetLatesSmallProcedure(Registration_DataStorage.CurrentPatient.PatientID, message.MedServiceID, (long)AllLookupValues.RegistrationType.NOI_TRU);
                    if(Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID != 0)
                    {
                        UCSmallProcedureEdit.GetResourcesForMedicalServicesListByDeptIDAndTypeID(Globals.DeptLocation.DeptID, Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID);
                    }
                    //UCSmallProcedureEdit.ApplySmallProcedure(new SmallProcedure { V_RegistrationType = (long)AllLookupValues.RegistrationType.NOI_TRU });
                }
                //▼====== #007
                if (Registration_DataStorage.CurrentPatientRegistration != null && Registration_DataStorage.CurrentPatientRegistrationDetail != null)
                {
                    if(UCClinicDeptInPtReqFormSmallProcedure.CurrentReqOutwardDrugClinicDeptPatient == null)
                    {
                        UCClinicDeptInPtReqFormSmallProcedure.CurrentReqOutwardDrugClinicDeptPatient = new ReqOutwardDrugClinicDeptPatient();
                    }
                    UCClinicDeptInPtReqFormSmallProcedure.CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration = Registration_DataStorage.CurrentPatientRegistration;
                    UCClinicDeptInPtReqFormSmallProcedure.CurrentReqOutwardDrugClinicDeptPatient.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
                    UCClinicDeptInPtReqFormSmallProcedure.PtRegDetailID = Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID;
                    UCClinicDeptInPtReqFormSmallProcedure.GetRequestDrugForTechnicalServicePtRegDetailID(Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID);
                    UCClinicDeptInPtReqFormSmallProcedure.FormEditorIsEnabled = true;
                }
                //▲====== #007
            }
        }
        //▲====== #004:
        //▼===== #005: Phải sử dụng event ở đây vì nếu không sử dụng mà đưa vào handle của SetInPatient 
        //             sẽ bị chạy đua.
        public void Handle(LoadDataForHtmlEditor message)
        {
            if (message != null && message.SmallProcedure != null)
            {
                UCSmallProcedureDesc.LoadBaseSection(message.SmallProcedure.ProcedureDescription);
            }
        }
        //▲===== #005
        #endregion
        private bool CheckValidSmallProcedure(SmallProcedure aSmallProcedure)
        {
            StringBuilder sb = new StringBuilder();
            if (aSmallProcedure == null)
            {
                return true;
            }
            if (aSmallProcedure.CompletedDateTime < aSmallProcedure.ProcedureDateTime)
            {
                sb.AppendLine(eHCMSResources.Z3019_G1_MsgNgayBatDauNgayKetThuc);
            }
            if (aSmallProcedure == null || aSmallProcedure.ProcedureDateTime == DateTime.MinValue)
            {
                sb.AppendLine(eHCMSResources.Z2408_G1_VuiLongNhapTGPTTT);
            }
            //▼===== 20191212: Tường minh thông tin báo lỗi khi người dùng không có chẩn đoán trước thủ thuật phẫu thuật.
            if (aSmallProcedure.BeforeICD10 == null || string.IsNullOrEmpty(aSmallProcedure.BeforeICD10.DiseaseNameVN))
            {
                sb.AppendLine(eHCMSResources.Z2940_G1_ChanDoanTruocTTPT);
            }
            //▲===== 
            if (aSmallProcedure.AfterICD10 == null || string.IsNullOrEmpty(aSmallProcedure.AfterICD10.DiseaseNameVN))
            {
                sb.AppendLine(eHCMSResources.Z2915_G1_VLNhapCDSauPT);
            }
            if (string.IsNullOrEmpty(aSmallProcedure.ProcedureMethod))
            {
                sb.AppendLine(eHCMSResources.Z2408_G1_VLNhapPhuongPhapTTPT);
            }
            if (aSmallProcedure.CompletedDateTime == DateTime.MinValue)
            {
                sb.AppendLine(eHCMSResources.Z2916_G1_VLNhapNgayKetThuc);
            }
            //20191220 TBL: Ngày bắt đầu không được nhỏ hơn ngày nhập viện
            if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatientRegistration != null && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo != null &&
                Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.AdmissionDate.Value > aSmallProcedure.ProcedureDateTime)
            {
                sb.AppendLine(string.Format(eHCMSResources.Z2944_G1_NgayBatDauNhoHonNgayNhapVien, Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.AdmissionDate.Value));
            }
            //20200217 TBL: Bắt buộc phải nhập 1 trong 8 trường để lấy CCHN báo cáo BHYT
            if (aSmallProcedure.ProcedureDoctorStaffID == null && aSmallProcedure.ProcedureDoctorStaffID2 == null && aSmallProcedure.NurseStaffID == null && aSmallProcedure.NurseStaffID2 == null
                && aSmallProcedure.NurseStaffID3 == null && aSmallProcedure.NarcoticDoctorStaffID == null && aSmallProcedure.NarcoticDoctorStaffID2 == null && aSmallProcedure.CheckRecordDoctorStaffID == null)
            {
                sb.AppendLine(eHCMSResources.Z2981_G1_ChuaChonNguoiThucHien);
            }
            TimeSpan CompareTime = new TimeSpan();
            CompareTime = aSmallProcedure.CompletedDateTime.Subtract(aSmallProcedure.ProcedureDateTime);
            if (CompareTime.TotalHours > Globals.ServerConfigSection.CommonItems.MaxTimeForSmallProcedure)
            {
                sb.AppendLine(string.Format(eHCMSResources.Z2994_G1_KetThucKhongVuotQuaBatDau, Globals.ServerConfigSection.CommonItems.MaxTimeForSmallProcedure));
            }
            if (!string.IsNullOrEmpty(sb.ToString()))
            {
                MessageBox.Show(sb.ToString(), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
            else
            {
                return true;
            }
        }
        public void btnPrintProcedure()
        {
            if (UCSmallProcedureEdit.UpdatedSmallProcedure != null)
            {
                MessageBox.Show(eHCMSResources.A1037_G1_Msg_InfoTTinBiDoi);
                return;
            }
            if (UCSmallProcedureEdit.SmallProcedureObj == null)
            {
                MessageBox.Show(eHCMSResources.Z2410_G1_KhongCoTTThuThuat);
                return;
            }
            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
            {
                proAlloc.RegistrationDetailID = Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID;
                proAlloc.eItem = ReportName.The_Thu_Thuat;
                proAlloc.V_RegistrationType = (long)AllLookupValues.RegistrationType.NOI_TRU;
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }
        public void btnSave()
        {
            SmallProcedure mSmallProcedure = UCSmallProcedureEdit.UpdatedSmallProcedure;
            mSmallProcedure = UCSmallProcedureEdit.SmallProcedure_InPt;
            //20200229 TBL: Set lại PtRegDetailID của thủ thuật để lưu xuống cho đúng
            mSmallProcedure.PtRegDetailID = Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID;
            //▼===== #005: Bổ sung code đưa dữ liệu vào trong 2 biến ProcedureDescription và ProcedureDescriptionContent vì 
            //             2  biến này lưu trữ thông tin tường trình phẫu thuật mà không có thì sẽ không lưu được.
            if (UCSmallProcedureDesc != null)
            {
                mSmallProcedure.ProcedureDescription = UCSmallProcedureDesc.BodyContent;
                mSmallProcedure.ProcedureDescriptionContent = UCSmallProcedureDesc.BodyContentText;
            }
            //▲===== #005
            if (!CheckValidSmallProcedure(mSmallProcedure))
            {
                return;
            }
            //▼===== #006
            //ucOutPMR.SaveCreateNew(mSmallProcedure);
            ucOutPMR.SaveCreateNew(mSmallProcedure, UCSmallProcedureEdit.CompareResource() ? new List<Resources>() : SelectedResourceList.ToList());
            //▲===== #006
            UCSmallProcedureEdit.ApplySmallProcedure(mSmallProcedure);
            //if (mSmallProcedure != null)
            //{
            //    mSmallProcedure.SmallProcedureID = SmallProcedureID;
            //    UCSmallProcedureEdit.ApplySmallProcedure(mSmallProcedure);
            //}
        }
        public void btnPrintProcedureProcess()
        {
            var CurrentSmallProcedure = UCSmallProcedureEdit == null ? null : UCSmallProcedureEdit.SmallProcedure_InPt;
            if (CurrentSmallProcedure == null || CurrentSmallProcedure.SmallProcedureID == 0 || Registration_DataStorage.CurrentPatient == null)
            {
                return;
            }
            CommonGlobals.PrintProcedureProcess(this, CurrentSmallProcedure, Registration_DataStorage.CurrentPatientRegistration);
        }
        //▼===== #005: Bổ sung thêm chức năng xem in của tường trình phẫu thuật.
        public void btnEditProcedureDesc()
        {
            var CurrentSmallProcedure = UCSmallProcedureEdit == null ? null : UCSmallProcedureEdit.SmallProcedure_InPt;
            if (CurrentSmallProcedure == null || CurrentSmallProcedure.SmallProcedureID == 0 || Registration_DataStorage.CurrentPatient == null)
            {
                return;
            }
            CommonGlobals.PrintProcedureProcess_V2(this, CurrentSmallProcedure, Registration_DataStorage.CurrentPatientRegistration);
        }
        //▲===== #005
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
                ucOutPMR.Registration_DataStorage = Registration_DataStorage;
                UCHeaderInfoPMR.Registration_DataStorage = Registration_DataStorage;
                ucOutPMRs.Registration_DataStorage = Registration_DataStorage;
                UCPtRegDetailInfo.Registration_DataStorage = Registration_DataStorage;
                UCSmallProcedureDesc.Registration_DataStorage = Registration_DataStorage;
            }
        }
        #region Methods
        private IEnumerator<IResult> SetInPatientInfoForConsultation(PatientRegistration aRegistration, PatientRegistrationDetail aRegistrationDetail)
        {
            if (aRegistrationDetail == null)
            {
                aRegistrationDetail = new PatientRegistrationDetail();
            }
            Registration_DataStorage = new Registration_DataStorage();
            yield return GenericCoRoutineTask.StartTask(InitPhyExamAction, aRegistration);
            yield return GenericCoRoutineTask.StartTask(GetInPtServiceRecordForKhamBenhAction, aRegistration);
            yield return GenericCoRoutineTask.StartTask(GetRegistrationInPtAction, aRegistration, aRegistrationDetail);
        }
        private void InitPhyExamAction(GenericCoRoutineTask genTask, object ObjPtRegistration)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new SummaryServiceClient())
                    {
                        bool bContinue = true;

                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetPhyExam_ByPtRegID(((PatientRegistration)ObjPtRegistration).PtRegistrationID, (long)((PatientRegistration)ObjPtRegistration).V_RegistrationType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                Globals.curPhysicalExamination = contract.EndGetPhyExam_ByPtRegID(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                ClientLoggerHelper.LogError(ex.Message);
                                bContinue = false;
                            }
                            finally
                            {
                                genTask.ActionComplete(bContinue);
                                this.HideBusyIndicator();
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void GetInPtServiceRecordForKhamBenhAction(GenericCoRoutineTask genTask, object ObjPtRegistration)
        {
            this.ShowBusyIndicator();
            PatientServiceRecord psrSearch = new PatientServiceRecord();
            psrSearch.PtRegistrationID = ((PatientRegistration)ObjPtRegistration).PtRegistrationID;
            psrSearch.V_RegistrationType = ((PatientRegistration)ObjPtRegistration).V_RegistrationType;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonUtilsServiceClient())
                    {
                        bool bContinue = true;
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPatientServiceRecordsGetForKhamBenh_InPt(psrSearch,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var psr = contract.EndPatientServiceRecordsGetForKhamBenh_InPt(asyncResult);
                                Registration_DataStorage.PatientServiceRecordCollection = new ObservableCollection<PatientServiceRecord>(psr);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                ClientLoggerHelper.LogError(ex.Message);
                                bContinue = false;
                            }
                            finally
                            {
                                genTask.ActionComplete(bContinue);
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public CallSetInPatientInfoAndRegistrationForePresciption_InPt CallSetInPatientInfoAndRegistrationForePresciption_InPt { get; set; }
        private void GetRegistrationInPtAction(GenericCoRoutineTask aGenTask, object ObjPtRegistration, object aPatientRegistrationDetail)
        {
            this.ShowBusyIndicator();
            LoadRegistrationSwitch CurrentSwitch = new LoadRegistrationSwitch();
            CurrentSwitch.IsGetAdmissionInfo = true;
            CurrentSwitch.IsGetBedAllocations = true;
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var aFactory = new PatientRegistrationServiceClient())
                    {
                        bool mIsContinue = true;
                        var aContract = aFactory.ServiceInstance;
                        //KMx: Chuyển hàm, lý do cần lấy thêm AdmissionInfo để khi ra toa xuất viện lọc lại các khoa BN đã nằm (07/03/2015 10:21)
                        //contract.BeginGetRegistration(((PatientRegistration)ObjPtRegistration).PtRegistrationID, (int)Globals.PatientFindBy_ForConsultation.Value,
                        //Globals.DispatchCallback((asyncResult) =>
                        aContract.BeginGetRegistrationInfo_InPt(((PatientRegistration)ObjPtRegistration).PtRegistrationID, (int)Globals.PatientFindBy_ForConsultation.Value, CurrentSwitch, false, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                PatientRegistration mRegistration = aContract.EndGetRegistrationInfo_InPt(asyncResult);
                                mRegistration.Patient.CurrentHealthInsurance = mRegistration.HealthInsurance;
                                Registration_DataStorage.CurrentPatientRegistration = mRegistration;
                                Registration_DataStorage.CurrentPatientRegistrationDetail = (aPatientRegistrationDetail as PatientRegistrationDetail);
                                //Globals.SetInfoPatient(mRegistration.Patient, mRegistration, null);
                                //KMx: Hiển thị thông tin bệnh nhân (10/10/2014 16:45).
                                Globals.EventAggregator.Publish(new ShowInPatientInfoForConsultation() { Patient = mRegistration.Patient, PtRegistration = mRegistration });
                                Globals.EventAggregator.Publish(new SetInPatientInfoAndRegistrationForConsultations_InPt() { MedServiceID = (aPatientRegistrationDetail as PatientRegistrationDetail).MedServiceID.GetValueOrDefault(0) });
                                Globals.EventAggregator.Publish(new SetInPatientInfoAndRegistrationForePresciption_InPt() { });
                                if (CallSetInPatientInfoAndRegistrationForePresciption_InPt != null)
                                {
                                    CallSetInPatientInfoAndRegistrationForePresciption_InPt();
                                }
                                Globals.EventAggregator.Publish(new SetInPatientInfoAndRegistrationForInPatientDischarge() { });
                                Globals.EventAggregator.Publish(new SetInPatientInfoAndRegistrationForPatientPCLRequest() { });
                                Globals.EventAggregator.Publish(new SetInPatientInfoAndRegistrationForPatientPCLRequestImage() { });
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                ClientLoggerHelper.LogError(ex.Message);
                                mIsContinue = false;
                            }
                            finally
                            {
                                aGenTask.ActionComplete(mIsContinue);
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    aGenTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });
            CurrentThread.Start();
        }
        public void CallSetInPatientInfoForConsultation(PatientRegistration aRegistration, PatientRegistrationDetail aRegistrationDetail)
        {
            if (aRegistration == null)
            {
                return;
            }
            Coroutine.BeginExecute(SetInPatientInfoForConsultation(aRegistration, aRegistrationDetail));
        }

        //▼===== #006
        public ObservableCollection<Resources> SelectedResourceList
        {
            get
            {
                return UCSmallProcedureEdit.SelectedResourceList;
            }
            set
            {
                UCSmallProcedureEdit.SelectedResourceList = value;
            }
        }
        //▲===== #006

        public IClinicDeptInPtReqForm_ForSmallProcedure UCClinicDeptInPtReqFormSmallProcedure { get; set; }
        #endregion
    }
}