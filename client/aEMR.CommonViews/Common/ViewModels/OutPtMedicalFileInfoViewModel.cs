using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.ViewContracts.Consultation_ePrescription;
using Castle.Windsor;
using DataEntities;
using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Windows;
using aEMR.Common;
using DataEntities.MedicalInstruction;
using System.Windows.Controls;
using System.ServiceModel;
using aEMR.DataContracts;
using aEMR.Infrastructure.Events;
/*
* 20220308 #001 QTD:    Phiếu khám vào viện
* 20220318 #002 QTD:    Lấy thông tin hội chẩn
* 20220509 #003 DatTB: Chỉnh sửa HSBA RHM: Bệnh chuyên khoa long -> string
* 20220511 #004 DatTB: Thêm biến ẩn trường chọn của "Tình trạng người bệnh khi ra viện", "Hướng điều trị và chế độ tiếp theo"
* 20220624 #005 BLQ: 
*   + Ràng buộc dữ liệu nhập vào 
*   + Chỉnh giá trị mặc định các trường
*   + Lấy phương pháp điều trị lên theo mẫu lời dặn
* 20220628 #006 QTD:    Chỉnh sửa phần hiển thị ngày, kiểm tra chi tiết ràng buộc
* 20220829 #007 QTD:    Chỉnh sửa màn hình Thông tin bệnh án
* 20221231 #008 BLQ: Thêm các thông tin hồ sơ bệnh án bệnh mãn tính
* 20230110 #009 DatTB: 
+ Thêm biến xác định ĐTNT bệnh mãn tính
+ Lấy thông tin bệnh án mãn tính
+ Thêm biến lấy tiểu sử bản thân bệnh án mãn tính
* 20220411 #010 BLQ: Thêm tab hồ sơ bệnh án
* 20230415 #011 QTD: Thêm Tab Giấy ra viện
* 20230601 #012 QTD: Sửa sự kiện thay đổi phương pháp điều trị
*/

namespace aEMR.CommonViews.ViewModels
{
    [Export(typeof(IOutPtMedicalFileInfo)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class OutPtMedicalFileInfoViewModel : ViewModelBase, IOutPtMedicalFileInfo
         , IHandle<SaveDisChargePapersInfo_Event>
    {
        #region Properties
        public long PtRegDetailID { get; set; }
        public long OutPtTreatmentProgramID { get; set; }
        public string MedicalRecordNote { get; set; }
        public string SpecialistDisease { get; set; } //<====: #003
        public long PatientID { get; set; } = 0;//<====: #008
        private bool _VisibilityMaxillofacialTab = false;
        public bool VisibilityMaxillofacialTab
        {
            get
            {
                return _VisibilityMaxillofacialTab;
            }
            set
            {
                if (_VisibilityMaxillofacialTab == value)
                {
                    return;
                }
                _VisibilityMaxillofacialTab = value;
                NotifyOfPropertyChange(() => VisibilityMaxillofacialTab);
                NotifyOfPropertyChange(() => VisibilityMedicalFileTab);
                NotifyOfPropertyChange(() => VisibilitySumaryTab);
                NotifyOfPropertyChange(() => VisibilityAdmissionTab);
                NotifyOfPropertyChange(() => VisibilityTicketCareTab);
            }

        }

        //▼====: #004
        private bool _VisibilityDischarge_Treatment = true;
        public bool VisibilityDischarge_Treatment
        {
            get
            {
                return _VisibilityDischarge_Treatment;
            }
            set
            {
                if (_VisibilityDischarge_Treatment == value)
                {
                    return;
                }
                _VisibilityDischarge_Treatment = value;
                NotifyOfPropertyChange(() => VisibilityDischarge_Treatment);
            }

        }
        //▲====: #004

        private ObservableCollection<RefDepartment> _departments;
        public ObservableCollection<RefDepartment> Departments
        {
            get { return _departments; }
            set
            {
                _departments = value;
                NotifyOfPropertyChange(() => Departments);
            }
        }

        private HistoryAndPhysicalExaminationInfo _CurrentHistoryAndPhysicalExaminationInfo;
        public HistoryAndPhysicalExaminationInfo CurrentHistoryAndPhysicalExaminationInfo
        {
            get
            {
                return _CurrentHistoryAndPhysicalExaminationInfo;
            }
            set
            {
                if (_CurrentHistoryAndPhysicalExaminationInfo == value)
                {
                    return;
                }
                _CurrentHistoryAndPhysicalExaminationInfo = value;
                ProgDateFromContent.DateTime = _CurrentHistoryAndPhysicalExaminationInfo.ProgDateFrom;
                ProgDateToContent.DateTime = _CurrentHistoryAndPhysicalExaminationInfo.ProgDateTo;
                NotifyOfPropertyChange(() => CurrentHistoryAndPhysicalExaminationInfo);
            }
        }
        #endregion
        IEventAggregator _eventArg;
        #region Events
        [ImportingConstructor]
        public OutPtMedicalFileInfoViewModel(IWindsorContainer aContainer, INavigationService aNavigation, ISalePosCaching aCaching, IEventAggregator eventArg)
        {
            _eventArg = eventArg;
            eventArg.Subscribe(this);
            //▼====: #006
            ProgDateFromContent = Globals.GetViewModel<IMinHourDateControl>();
            ProgDateFromContent.DateTime = null;
            ProgDateToContent = Globals.GetViewModel<IMinHourDateControl>();
            ProgDateToContent.DateTime = null;
            //▲====: #006

            ObjPrescriptionNoteTemplates_TreatmentDirectionFollowupRegimen = new ObservableCollection<PrescriptionNoteTemplates>();
            ObjPrescriptionNoteTemplates_Treatments = new ObservableCollection<PrescriptionNoteTemplates>();
            PrescriptionNoteTemplates_GetAll();

            CreateSubVMRHM();
            //▼====: #005
            ObjNoteTemplates_GetAll = new ObservableCollection<PrescriptionNoteTemplates>();
            NoteTemplates_GetAllIsActive();
            //▲====: #005
            //▼====: #007
            GetV_OutDischargeCondition();
            GetV_OutDischargeType();
            //▲====: #007
            //▼====: #010
            GetRegistrationByOutPtTreatmentProgramID();
            //▲====: #010
        }

        protected override void OnActivate()
        {
            GetHistoryAndPhysicalExaminationInfo();
            base.OnActivate();
            ActivateItem(UCAdmissionExamination);
            //▼====: #008
            ActivateItem(UCSelfDeclaration);
            //▲====: #008
        }

        public void btnSave()
        {
            EditHistoryAndPhysicalExaminationInfo();
        }

        public void btnPrint()
        {
            GlobalsNAV.ShowDialog<ICommonPreviewView>((aView) =>
            {
                //▼==== #009
                if (IsChronic)
                {
                    aView.eItem = ReportName.ChronicOutPtMedicalFile;
                    aView.OutPtTreatmentProgramID = OutPtTreatmentProgramID;
                    aView.RegistrationID = InPtRegistrationID;
                    aView.ID = DiagConsultationSummaryID;
                    aView.V_RegistrationType = V_RegistrationType;
                }
                //▲==== #009
                else if (CurrentHistoryAndPhysicalExaminationInfo != null && CurrentHistoryAndPhysicalExaminationInfo.V_SpecialistType == 83607)
                {
                    aView.eItem = ReportName.MaxillofacialOutPtMedicalFile;
                    aView.OutPtTreatmentProgramID = OutPtTreatmentProgramID;
                    aView.RegistrationID = InPtRegistrationID;
                    aView.ID = DiagConsultationSummaryID;
                    aView.V_RegistrationType = V_RegistrationType;
                }
                else
                {
                    aView.eItem = ReportName.GeneralOutPtMedicalFile;
                    aView.OutPtTreatmentProgramID = OutPtTreatmentProgramID;
                    aView.V_RegistrationType = V_RegistrationType;
                    aView.ID = DiagConsultationSummaryID;
                }
            });
        }

        private ObservableCollection<PrescriptionNoteTemplates> _ObjPrescriptionNoteTemplates_TreatmentDirectionFollowupRegimen;
        public ObservableCollection<PrescriptionNoteTemplates> ObjPrescriptionNoteTemplates_TreatmentDirectionFollowupRegimen
        {
            get { return _ObjPrescriptionNoteTemplates_TreatmentDirectionFollowupRegimen; }
            set
            {
                _ObjPrescriptionNoteTemplates_TreatmentDirectionFollowupRegimen = value;
                NotifyOfPropertyChange(() => ObjPrescriptionNoteTemplates_TreatmentDirectionFollowupRegimen);
            }
        }

        private ObservableCollection<PrescriptionNoteTemplates> _ObjPrescriptionNoteTemplates_Treatments;
        public ObservableCollection<PrescriptionNoteTemplates> ObjPrescriptionNoteTemplates_Treatments
        {
            get { return _ObjPrescriptionNoteTemplates_Treatments; }
            set
            {
                _ObjPrescriptionNoteTemplates_Treatments = value;
                NotifyOfPropertyChange(() => ObjPrescriptionNoteTemplates_Treatments);
            }
        }

        private PrescriptionNoteTemplates _ObjPrescriptionNoteTemplates_Treatments_Selected;
        public PrescriptionNoteTemplates ObjPrescriptionNoteTemplates_Treatments_Selected
        {
            get { return _ObjPrescriptionNoteTemplates_Treatments_Selected; }
            set
            {
                if (_ObjPrescriptionNoteTemplates_Treatments_Selected == value)
                {
                    return;
                }
                _ObjPrescriptionNoteTemplates_Treatments_Selected = value;
                NotifyOfPropertyChange(() => ObjPrescriptionNoteTemplates_Treatments_Selected);

                if (_ObjPrescriptionNoteTemplates_Treatments_Selected != null && _ObjPrescriptionNoteTemplates_Treatments_Selected.PrescriptNoteTemplateID > 0)
                {
                    string str = CurrentHistoryAndPhysicalExaminationInfo.ConditionDischarge;
                    if (string.IsNullOrEmpty(str))
                    {
                        str = ObjPrescriptionNoteTemplates_Treatments_Selected.DetailsTemplate;
                    }
                    else
                    {
                        str = str + Environment.NewLine + ObjPrescriptionNoteTemplates_Treatments_Selected.DetailsTemplate;
                    }

                    CurrentHistoryAndPhysicalExaminationInfo.ConditionDischarge = str;
                }
            }
        }

        private PrescriptionNoteTemplates _ObjPrescriptionNoteTemplates_TreatmentDirectionFollowupRegimen_Selected;
        public PrescriptionNoteTemplates ObjPrescriptionNoteTemplates_TreatmentDirectionFollowupRegimen_Selected
        {
            get { return _ObjPrescriptionNoteTemplates_TreatmentDirectionFollowupRegimen_Selected; }
            set
            {
                if (_ObjPrescriptionNoteTemplates_TreatmentDirectionFollowupRegimen_Selected == value)
                {
                    return;
                }
                _ObjPrescriptionNoteTemplates_TreatmentDirectionFollowupRegimen_Selected = value;
                NotifyOfPropertyChange(() => ObjPrescriptionNoteTemplates_TreatmentDirectionFollowupRegimen_Selected);

                if (_ObjPrescriptionNoteTemplates_TreatmentDirectionFollowupRegimen_Selected != null && _ObjPrescriptionNoteTemplates_TreatmentDirectionFollowupRegimen_Selected.PrescriptNoteTemplateID > 0)
                {
                    string str = CurrentHistoryAndPhysicalExaminationInfo.DirectionOfTreatment;
                    if (string.IsNullOrEmpty(str))
                    {
                        str = ObjPrescriptionNoteTemplates_TreatmentDirectionFollowupRegimen_Selected.DetailsTemplate;
                    }
                    else
                    {
                        str = str + Environment.NewLine + ObjPrescriptionNoteTemplates_TreatmentDirectionFollowupRegimen_Selected.DetailsTemplate;
                    }

                    CurrentHistoryAndPhysicalExaminationInfo.DirectionOfTreatment = str;
                }
            }
        }
        #endregion
        
        #region Methods
        private void GetHistoryAndPhysicalExaminationInfo()
        {
            //this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            this.DlgShowBusyIndicator(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.K2887_G1_DangXuLy));
            var CurrentThread = new Thread(() =>
            {
                using (var CurrentFactory = new CommonUtilsServiceClient())
                {
                    var CurrentContract = CurrentFactory.ServiceInstance;
                    CurrentContract.BeginGetHistoryAndPhysicalExaminationInfoByOutPtTreatmentProgramID(OutPtTreatmentProgramID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            CurrentHistoryAndPhysicalExaminationInfo = CurrentContract.EndGetHistoryAndPhysicalExaminationInfoByOutPtTreatmentProgramID(asyncResult);
                            if (CurrentHistoryAndPhysicalExaminationInfo == null)
                            {
                                CurrentHistoryAndPhysicalExaminationInfo = new HistoryAndPhysicalExaminationInfo
                                {
                                    OutPtTreatmentProgramID = OutPtTreatmentProgramID,
                                    PastMedicalHistory = "Khỏe",
                                    PastMedicalHistoryOfFamily = "Khỏe",
                                    PhysicalExamination = "Bệnh tỉnh, tiếp xúc tốt",
                                    PhysicalExaminationAllParts = "Tim đều, phổi trong, bụng mềm",
                                    //▼====: #007
                                    V_OutDischargeType = SelectedOutDischargeType != null ? SelectedOutDischargeType.LookupID : 0,
                                    //DischargeStatus = SelectedOutDischargeType != null ? SelectedOutDischargeType.ObjectValue : "",
                                    ConditionDischarge = SelectedOutDischargeType != null ? SelectedOutDischargeType.ObjectValue : "",
                                    V_OutDischargeCondition = SelectedOutDischargeCondition != null ? SelectedOutDischargeCondition.LookupID : 0
                                    //▲====: #007
                                };
                            }
                            //▼====: #008
                            if (IsChronic)
                            {
                                if (InPtRegistrationID != 0)
                                {
                                    UCAdmissionExamination.InPtRegistrationID = InPtRegistrationID;
                                    UCAdmissionExamination.V_RegistrationType = V_RegistrationType;
                                    UCAdmissionExamination.DiagTrmtItem = DiagTrmtItem;
                                    UCAdmissionExamination.IsChronic = IsChronic;
                                    //▼==== #009
                                    UCAdmissionExamination.PastMedicalHistory = CurrentHistoryAndPhysicalExaminationInfo.PastMedicalHistory;
                                    //▲==== #009
                                    UCAdmissionExamination.LoadAdmissionExamination(InPtRegistrationID, CurrentHistoryAndPhysicalExaminationInfo.OrientedTreatment, false);
                                    GetDiagnosysConsultationSummaryAction(InPtRegistrationID);

                                    UCSelfDeclaration.PatientID = PatientID;
                                    UCSelfDeclaration.PtRegistrationID = InPtRegistrationID;
                                    UCSelfDeclaration.V_RegistrationType = V_RegistrationType;
                                    UCSelfDeclaration.GetSelfDeclarationByPtRegistrationID();
                                }
                                GetTicketCareByOutPtTreatmentProgramID();
                            }
                            //▲====: #008
                            if (CurrentHistoryAndPhysicalExaminationInfo != null)
                            {
                                if (CurrentHistoryAndPhysicalExaminationInfo.V_SpecialistType == 83607)
                                {
                                    VisibilityMaxillofacialTab = true;
                                    VisibilityDischarge_Treatment = false; //<====: #004
                                    if (CurrentHistoryAndPhysicalExaminationInfo.PathologicalProcessAndClinicalCourse == null)
                                        CurrentHistoryAndPhysicalExaminationInfo.PathologicalProcessAndClinicalCourse = CurrentHistoryAndPhysicalExaminationInfo.OrientedTreatment;

                                    if (CurrentHistoryAndPhysicalExaminationInfo.PCLResultsHaveDiagnosticValue == null)
                                        CurrentHistoryAndPhysicalExaminationInfo.PCLResultsHaveDiagnosticValue = CurrentHistoryAndPhysicalExaminationInfo.XQuangNote.ToString().Replace("\\n", "\n");
                                    else
                                        CurrentHistoryAndPhysicalExaminationInfo.PCLResultsHaveDiagnosticValue = CurrentHistoryAndPhysicalExaminationInfo.PCLResultsHaveDiagnosticValue.ToString().Replace("\\n", "\n");
                                    if (InPtRegistrationID != 0)
                                    {
                                        UCAdmissionExamination.InPtRegistrationID = InPtRegistrationID;
                                        UCAdmissionExamination.V_RegistrationType = V_RegistrationType;
                                        UCAdmissionExamination.Diagnosis = CurrentHistoryAndPhysicalExaminationInfo.Diagnosis;
                                        UCAdmissionExamination.LoadAdmissionExamination(InPtRegistrationID, CurrentHistoryAndPhysicalExaminationInfo.OrientedTreatment, true);
                                        GetDiagnosysConsultationSummaryAction(InPtRegistrationID);
                                    }
                                    GetTicketCareByOutPtTreatmentProgramID();
                                    MedicalRecordNote = CurrentHistoryAndPhysicalExaminationInfo.MedicalRecordNote.ToString();
                                    CurrentHistoryAndPhysicalExaminationInfo.MedicalRecordNote = MedicalRecordNote.Replace("\\n", "\n");
                                    //▼====: #003
                                    SpecialistDisease = CurrentHistoryAndPhysicalExaminationInfo.SpecialistDisease.ToString();
                                    CurrentHistoryAndPhysicalExaminationInfo.SpecialistDisease = SpecialistDisease.Replace("\\n", "\n");
                                    //▲====: #003
                                }
                            }
                            //▼====: #005
                            if (!string.IsNullOrEmpty(Diagnosis) && string.IsNullOrEmpty(CurrentHistoryAndPhysicalExaminationInfo.HistoryAndPhysicalExamination))
                            {
                                CurrentHistoryAndPhysicalExaminationInfo.HistoryAndPhysicalExamination = Diagnosis;
                            }
                            if (!string.IsNullOrEmpty(ReasonHospitalStay) && string.IsNullOrEmpty(CurrentHistoryAndPhysicalExaminationInfo.ReasonAdmission))
                            {
                                CurrentHistoryAndPhysicalExaminationInfo.ReasonAdmission = ReasonHospitalStay;
                            }
                            //▲====: #005
                            //▼====: #006
                            ProgDateFromContent.DateTime = CurrentHistoryAndPhysicalExaminationInfo.ProgDateFrom;
                            ProgDateToContent.DateTime = CurrentHistoryAndPhysicalExaminationInfo.ProgDateTo;
                            //▲====: #006
                            //▼====: #007
                            if(CurrentHistoryAndPhysicalExaminationInfo.V_OutDischargeType != 0)
                            {
                                SelectedOutDischargeType = V_OutDischargeType.Where(x => x.LookupID == CurrentHistoryAndPhysicalExaminationInfo.V_OutDischargeType).FirstOrDefault();
                            }
                            if(CurrentHistoryAndPhysicalExaminationInfo.V_OutDischargeCondition != 0)
                            {
                                SelectedOutDischargeCondition = V_OutDischargeCondition.Where(x => x.LookupID == CurrentHistoryAndPhysicalExaminationInfo.V_OutDischargeCondition).FirstOrDefault();
                            }
                            //▲====: #007
                            if(InPtRegistrationID != 0)
                            {
                                GetDiagnosysConsultationSummaryAction(InPtRegistrationID);
                            }    
                            GetSummaryMedicalRecordByOutPtTreatmentProgramID();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        }
                        finally
                        {
                            this.DlgHideBusyIndicator();
                        }
                    }), null);
                }
            });
            CurrentThread.Start();
        }
        private void EditHistoryAndPhysicalExaminationInfo(bool IsSaveSummary = false)
        {
            if (Globals.LoggedUserAccount == null || Globals.LoggedUserAccount.StaffID.GetValueOrDefault(0) == 0)
            {
                return;
            }
            if (!CheckValidMedicalFile(CurrentHistoryAndPhysicalExaminationInfo, IsSaveSummary))
            {
                // MessageBox.Show("Vui lòng nhập đầy đủ các trường có đánh dấu *","Thông báo");
                return;
            }
            //▼====: #006
            CurrentHistoryAndPhysicalExaminationInfo.ProgDateFrom = (DateTime)ProgDateFromContent.DateTime;
            CurrentHistoryAndPhysicalExaminationInfo.ProgDateTo = ProgDateToContent.DateTime;
            //▲====: #006
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var CurrentThread = new Thread(() =>
            {
                using (var CurrentFactory = new CommonUtilsServiceClient())
                {
                    var CurrentContract = CurrentFactory.ServiceInstance;
                    CurrentContract.BeginEditHistoryAndPhysicalExaminationInfo(CurrentHistoryAndPhysicalExaminationInfo, Globals.LoggedUserAccount.StaffID.Value
                    , VisibilityMaxillofacialTab , IsSaveSummary 
                    , Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            CurrentContract.EndEditHistoryAndPhysicalExaminationInfo(asyncResult);
                            GetHistoryAndPhysicalExaminationInfo();
                            MessageBox.Show(eHCMSResources.Z2715_G1_ThanhCong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            CurrentThread.Start();
        }

        public void PrescriptionNoteTemplates_GetAll()
        {
            var t = new Thread(() =>
            {
                this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
                try
                {
                    using (var serviceFactory = new ePrescriptionsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginPrescriptionNoteTemplates_GetAll(Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<PrescriptionNoteTemplates> allItems = new ObservableCollection<PrescriptionNoteTemplates>();
                            try
                            {
                                allItems = contract.EndPrescriptionNoteTemplates_GetAll(asyncResult);
                                ObservableCollection<PrescriptionNoteTemplates> ObjPrescriptionNoteTemplates_GetAll = new ObservableCollection<PrescriptionNoteTemplates>(allItems);
                                ObjPrescriptionNoteTemplates_Treatments = ObjPrescriptionNoteTemplates_GetAll.Where(x => x.V_PrescriptionNoteTempType == AllLookupValues.V_PrescriptionNoteTempType.Treatments).ToObservableCollection();
                                ObjPrescriptionNoteTemplates_TreatmentDirectionFollowupRegimen = ObjPrescriptionNoteTemplates_GetAll.Where(x => x.V_PrescriptionNoteTempType == AllLookupValues.V_PrescriptionNoteTempType.TreatmentDirectionFollowupRegimen).ToObservableCollection();
                                PrescriptionNoteTemplates firstItem = new PrescriptionNoteTemplates();
                                firstItem.PrescriptNoteTemplateID = -1;
                                firstItem.NoteDetails = "--Chọn--";
                                ObjPrescriptionNoteTemplates_Treatments.Insert(0, firstItem);
                                ObjPrescriptionNoteTemplates_TreatmentDirectionFollowupRegimen.Insert(0, firstItem);
                                SetDefaultSelected();
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }
        #endregion
        
        private bool _IsShowCheckCOVID;
        public bool IsShowCheckCOVID
        {
            get { return _IsShowCheckCOVID; }
            set
            {
                if (_IsShowCheckCOVID != value)
                {
                    _IsShowCheckCOVID = value;
                    NotifyOfPropertyChange(() => IsShowCheckCOVID);
                }
            }
        }
        
        private DiagnosisTreatment _DiagTrmtItem;
        public DiagnosisTreatment DiagTrmtItem
        {
            get
            {
                return _DiagTrmtItem;
            }
            set
            {
                _DiagTrmtItem = value;
                NotifyOfPropertyChange(() => DiagTrmtItem);
            }
        }

        //▼====== #001
        public long InPtRegistrationID { get; set; }
        public IAdmissionExamination UCAdmissionExamination { get; set; }
        //▼====: #008
        public ISelfDeclaration UCSelfDeclaration { get; set; }
        //▲====: #008

        private void CreateSubVMRHM()
        {
            UCAdmissionExamination = Globals.GetViewModel<IAdmissionExamination>();
            UCAdmissionExamination.IsShowDepartment = true;
            UCAdmissionExamination.IsShowReasonAdmission = true;
            UCAdmissionExamination.IsShowNotes = true;
            UCAdmissionExamination.IsShowDiagnosisResult = true;
            UCAdmissionExamination.IsShowReferralDiagnosis = false;
            UCAdmissionExamination.IsShowPclResult = false;
            //▼====: #008
            UCSelfDeclaration = Globals.GetViewModel<ISelfDeclaration>();
            //▲====: #008
        }

        protected override void OnDeactivate(bool close)
        {
            DeactivateItem(UCAdmissionExamination, close);
            UCAdmissionExamination = null;
            //▼====: #008
            DeactivateItem(UCSelfDeclaration, close);
            UCSelfDeclaration = null;
            //▲====: #008
            base.OnDeactivate(close);
        }

        public long DiagConsultationSummaryID { get; set; }

        public long V_RegistrationType { get; set; }

        private DiagnosysConsultationSummary _gDiagConsultationCopier;
        public DiagnosysConsultationSummary gDiagConsultationCopier
        {
            get { return _gDiagConsultationCopier; }
            set
            {
                if (_gDiagConsultationCopier != value)
                {
                    _gDiagConsultationCopier = value;
                    NotifyOfPropertyChange(() => gDiagConsultationCopier);
                }
            }
        }

        private void GetDiagnosysConsultationSummaryAction(long PtRegistrationID)
        {
            //this.ShowBusyIndicator();
            int FindBy = 0;
            LoadRegistrationSwitch CurrentSwitch = new LoadRegistrationSwitch();
            CurrentSwitch.IsGetDiagnosysConsultationSummary = true;
            if ((int)Globals.PatientFindBy_ForConsultation.Value == (int)AllLookupValues.PatientFindBy.NOITRU)
            {
                FindBy = (int)AllLookupValues.PatientFindBy.NOITRU;
            }
            else
            {
                FindBy = (int)AllLookupValues.PatientFindBy.NGOAITRU;
            }
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var aFactory = new PatientRegistrationServiceClient())
                    {
                        var aContract = aFactory.ServiceInstance;
                        aContract.BeginGetRegistrationInfo_InPt(PtRegistrationID, FindBy, CurrentSwitch, false, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                PatientRegistration mRegistration = aContract.EndGetRegistrationInfo_InPt(asyncResult);
                                if (mRegistration == null)
                                {
                                    return;
                                }
                                if (mRegistration.DiagnosysConsultation == null)
                                {
                                    return;
                                }
                                DiagConsultationSummaryID = mRegistration.DiagnosysConsultation.DiagConsultationSummaryID;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                ClientLoggerHelper.LogError(ex.Message);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    //this.HideBusyIndicator();
                }
            });
            CurrentThread.Start();
        }
        private ObservableCollection<TicketCare> _ListTicketCare;
        public ObservableCollection<TicketCare> ListTicketCare
        {
            get { return _ListTicketCare; }
            set
            {
                _ListTicketCare = value;
                NotifyOfPropertyChange(() => ListTicketCare);
            }
        }
        private string _OrientedTreatment;
        public string OrientedTreatment
        {
            get { return _OrientedTreatment; }
            set
            {
                _OrientedTreatment = value;
                NotifyOfPropertyChange(() => OrientedTreatment);
            }
        }
        private void GetTicketCareByOutPtTreatmentProgramID()
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var CurrentThread = new Thread(() =>
            {
                using (var CurrentFactory = new CommonUtilsServiceClient())
                {
                    var CurrentContract = CurrentFactory.ServiceInstance;
                    CurrentContract.BeginGetTicketCareByOutPtTreatmentProgramID(OutPtTreatmentProgramID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var Result = CurrentContract.EndGetTicketCareByOutPtTreatmentProgramID(asyncResult);
                            if (Result != null)
                            {
                                ListTicketCare = Result.ToObservableCollection();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            CurrentThread.Start();
        }
        public void btnEditTicketCare_Click(object DataContext)
        {
            if (DataContext != null && DataContext is TicketCare)
            {
                TicketCare ticket = (TicketCare)DataContext;
                ITicketCare mDialogView = Globals.GetViewModel<ITicketCare>();
                mDialogView.gTicketCare = ticket;
                mDialogView.V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU;
                mDialogView.DoctorOrientedTreatment = OrientedTreatment;
                //if (InPatientDailyDiagnosisContent != null && InPatientDailyDiagnosisContent.DiagnosisTreatmentContent != null)
                //{
                //    mDialogView.DoctorOrientedTreatment = InPatientDailyDiagnosisContent.DiagnosisTreatmentContent.OrientedTreatment;
                //}
                GlobalsNAV.ShowDialog_V3(mDialogView);
                GetTicketCareByOutPtTreatmentProgramID();
            }
        }
        //▼====: #005
        private bool CheckValidMedicalFile(HistoryAndPhysicalExaminationInfo examinationInfo, bool IsSaveSummary = false)
        {
            //▼====: #006
            // Kiểm tra trường bắt buộc tab Hồ sơ bệnh án
            //if (string.IsNullOrEmpty(examinationInfo.HistoryAndPhysicalExamination)
            //    || string.IsNullOrEmpty(examinationInfo.ReasonAdmission)
            //    || string.IsNullOrEmpty(examinationInfo.PhysicalExamination)
            //    || string.IsNullOrEmpty(examinationInfo.PhysicalExaminationAllParts)
            //    || string.IsNullOrEmpty(examinationInfo.TreatmentMethod)
            //    || string.IsNullOrEmpty(examinationInfo.TreatmentSolution))
            //{
            //    return false;
            //}
            if (!IsSaveSummary || VisibilityMaxillofacialTab)
            {
                if (string.IsNullOrEmpty(examinationInfo.HistoryAndPhysicalExamination))
                {
                    Globals.ShowMessage("Chưa nhập " + eHCMSResources.K0786_G1_1QTrinhBLy, "Thông báo");
                    return false;
                }
                if (string.IsNullOrEmpty(examinationInfo.ReasonAdmission))
                {
                    Globals.ShowMessage("Chưa nhập " + eHCMSResources.Z0263_G1_LyDoVaoVien, "Thông báo");
                    return false;
                }
                if (string.IsNullOrEmpty(examinationInfo.PhysicalExamination))
                {
                    Globals.ShowMessage("Chưa nhập " + eHCMSResources.K0788_G1_1ToanThan, "Thông báo");
                    return false;
                }
                if (string.IsNullOrEmpty(examinationInfo.PhysicalExaminationAllParts))
                {
                    Globals.ShowMessage("Chưa nhập " + eHCMSResources.Z2905_G1_CacBoPhan, "Thông báo");
                    return false;
                }
                if (string.IsNullOrEmpty(examinationInfo.TreatmentMethod))
                {
                    Globals.ShowMessage("Chưa nhập " + eHCMSResources.K0841_G1_PPhapDTri, "Thông báo");
                    return false;
                }
                if(!VisibilityMaxillofacialTab)
                {
                    if (string.IsNullOrEmpty(examinationInfo.DischargeStatus))
                    {
                        Globals.ShowMessage("Chưa nhập " + eHCMSResources.Z2905_G1_TinhTrangRaVien, "Thông báo");
                        return false;
                    }
                }
                if (string.IsNullOrEmpty(examinationInfo.TreatmentSolution))
                {
                    Globals.ShowMessage("Chưa nhập " + eHCMSResources.T1720_G1_HuongDTri, "Thông báo");
                    return false;
                }
            }
            // Kiểm tra trường bắt buộc tab Tổng kết bệnh án
            //if (IsTongKetBenhAn 
            //    && (string.IsNullOrEmpty(examinationInfo.PathologicalProcessAndClinicalCourse)
            //    || string.IsNullOrEmpty(examinationInfo.PCLResultsHaveDiagnosticValue)
            //    || string.IsNullOrEmpty(examinationInfo.DischargeDiagnostic_MainDisease)
            //    || string.IsNullOrEmpty(examinationInfo.DischargeDiagnostic_IncludingDisease)
            //    || string.IsNullOrEmpty(examinationInfo.Treatments)
            //    || string.IsNullOrEmpty(examinationInfo.ConditionDischarge)
            //    || string.IsNullOrEmpty(examinationInfo.DirectionOfTreatment)
            //    ))
            //{
            //    return false;
            //}
            //if (IsTongKetBenhAn)
            if(IsSaveSummary || VisibilityMaxillofacialTab)
            {
                if(examinationInfo.HistoryAndPhysicalExaminationInfoID == 0 && !VisibilityMaxillofacialTab)
                {
                    Globals.ShowMessage("Vui lòng lưu thông tin bệnh án trước!", "Thông báo");
                    return false;
                }
                if (string.IsNullOrEmpty(examinationInfo.PathologicalProcessAndClinicalCourse))
                {
                    Globals.ShowMessage("Chưa nhập " + eHCMSResources.K0787_G1_1QTrinhBLyVaDienBienLS, "Thông báo");
                    return false;
                }
                if (string.IsNullOrEmpty(examinationInfo.PCLResultsHaveDiagnosticValue))
                {
                    Globals.ShowMessage("Chưa nhập " + eHCMSResources.Z2905_G1_TomTatXNCoChanDoan, "Thông báo");
                    return false;
                }
                if (string.IsNullOrEmpty(examinationInfo.DischargeDiagnostic_MainDisease))
                {
                    Globals.ShowMessage("Chưa nhập " + eHCMSResources.K0759_G1_BenhChinh, "Thông báo");
                    return false;
                }
                if (string.IsNullOrEmpty(examinationInfo.DischargeDiagnostic_IncludingDisease))
                {
                    Globals.ShowMessage("Chưa nhập " + eHCMSResources.K0760_G1_BenhKemTheo, "Thông báo");
                    return false;
                }
                if (string.IsNullOrEmpty(examinationInfo.Treatments))
                {
                    Globals.ShowMessage("Chưa nhập " + eHCMSResources.K0841_G1_PPhapDTri, "Thông báo");
                    return false;
                }
                //if (string.IsNullOrEmpty(examinationInfo.ConditionDischarge))
                //{
                //    Globals.ShowMessage("Chưa nhập " + eHCMSResources.G1341_G1_TTrangNguoiBenhKhiRV, "Thông báo");
                //    return false;
                //}
                if (!VisibilityMaxillofacialTab)
                {
                    if (examinationInfo.V_OutDischargeType == 0 || string.IsNullOrEmpty(examinationInfo.ConditionDischarge))
                    {
                        Globals.ShowMessage("Chưa nhập " + eHCMSResources.Z2905_G1_TinhTrangRaVien, "Thông báo");
                        return false;
                    }
                    if (examinationInfo.V_OutDischargeCondition == 0)
                    {
                        Globals.ShowMessage("Chưa nhập " + eHCMSResources.T2071_G1_KQuaDTri, "Thông báo");
                        return false;
                    }
                }
                if (string.IsNullOrEmpty(examinationInfo.DirectionOfTreatment))
                {
                    Globals.ShowMessage("Chưa nhập " + eHCMSResources.K0867_G1_5HuongDTriVaCheDoTiepTheo, "Thông báo");
                    return false;
                }
                if (!VisibilityMaxillofacialTab)
                {
                    if(ProgDateToContent.DateTime == null)
                    {
                        Globals.ShowMessage("Chưa nhập " + eHCMSResources.N0081_G1_NgKetThuc, "Thông báo");
                        return false;
                    }
                    if(ProgDateToContent.DateTime < ProgDateFromContent.DateTime)
                    {
                        Globals.ShowMessage(eHCMSResources.Z2956_G1_Msg, "Thông báo");
                        return false;
                    }
                }
            }
            //▲====: #006
            return true;
        }
        private string _Diagnosis;
        public string Diagnosis
        {
            get { return _Diagnosis; }
            set
            {
                _Diagnosis = value;
                NotifyOfPropertyChange(() => Diagnosis);
            }
        }
        private string _ReasonHospitalStay;
        public string ReasonHospitalStay
        {
            get { return _ReasonHospitalStay; }
            set
            {
                _ReasonHospitalStay = value;
                NotifyOfPropertyChange(() => ReasonHospitalStay);
            }
        }
        private string _MainDisease;
        public string MainDisease
        {
            get { return _MainDisease; }
            set
            {
                _MainDisease = value;
                NotifyOfPropertyChange(() => MainDisease);
            }
        }
        private string _IncludingDisease;
        public string IncludingDisease
        {
            get { return _IncludingDisease; }
            set
            {
                _IncludingDisease = value;
                NotifyOfPropertyChange(() => IncludingDisease);
            }
        }
        private ObservableCollection<PrescriptionNoteTemplates> _ObjNoteTemplates_GetAll;
        public ObservableCollection<PrescriptionNoteTemplates> ObjNoteTemplates_GetAll
        {
            get { return _ObjNoteTemplates_GetAll; }
            set
            {
                _ObjNoteTemplates_GetAll = value;
                NotifyOfPropertyChange(() => ObjNoteTemplates_GetAll);
            }
        }
        private PrescriptionNoteTemplates _ObjNoteTemplates_Selected;
        public PrescriptionNoteTemplates ObjNoteTemplates_Selected
        {
            get { return _ObjNoteTemplates_Selected; }
            set
            {
                if (_ObjNoteTemplates_Selected == value)
                {
                    return;
                }
                _ObjNoteTemplates_Selected = value;
                NotifyOfPropertyChange(() => ObjNoteTemplates_Selected);
                //▼====: #012
                //if (_ObjNoteTemplates_Selected != null && _ObjNoteTemplates_Selected.PrescriptNoteTemplateID > 0 && CurrentHistoryAndPhysicalExaminationInfo != null)
                //{
                //    string str = CurrentHistoryAndPhysicalExaminationInfo.TreatmentMethod;
                //    if (string.IsNullOrEmpty(str))
                //    {
                //        str = ObjNoteTemplates_Selected.DetailsTemplate;
                //    }
                //    else
                //    {
                //        str = str + Environment.NewLine + ObjNoteTemplates_Selected.DetailsTemplate;
                //    }
                //    CurrentHistoryAndPhysicalExaminationInfo.TreatmentMethod = str;
                //}
                //▲====: #012
            }
        }
        public void NoteTemplates_GetAllIsActive()
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePrescriptionsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        var pnt = new PrescriptionNoteTemplates();
                        pnt.V_PrescriptionNoteTempType = AllLookupValues.V_PrescriptionNoteTempType.Treatments;
                        contract.BeginPrescriptionNoteTemplates_GetAllIsActive(pnt, Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<PrescriptionNoteTemplates> allItems = new ObservableCollection<PrescriptionNoteTemplates>();
                            try
                            {
                                allItems = contract.EndPrescriptionNoteTemplates_GetAllIsActive(asyncResult);
                                ObjNoteTemplates_GetAll = new ObservableCollection<PrescriptionNoteTemplates>(allItems);
                                PrescriptionNoteTemplates firstItem = new PrescriptionNoteTemplates();
                                firstItem.PrescriptNoteTemplateID = -1;
                                firstItem.NoteDetails = "--Chọn--";
                                ObjNoteTemplates_GetAll.Insert(0, firstItem);
                                SetDefaultSelected();
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                }
            });
            t.Start();
        }
       
       
        private TabControl MedicalFileTabContol { get; set; }
        public void MedicalFileTab_Loaded(object sender, RoutedEventArgs e)
        {
            MedicalFileTabContol = (TabControl)sender;
        }
        private bool IsTongKetBenhAn = false;
        public void MedicalFileTab_Changed(object source, object eventArgs)
        {
            TabControl tabCtrl = source as TabControl;
            int destTabIndex = tabCtrl.SelectedIndex;
            if (tabCtrl != null && ((TabItem)tabCtrl.SelectedItem).Name == "TongKetBenhAn")
            {
                GetDefaultValueForTongKetBenhAn();
                IsTongKetBenhAn = true;
            }
            else
            {
                IsTongKetBenhAn = false;
            }
            NotifyOfPropertyChange(() => VisibilitySumaryTab);
            NotifyOfPropertyChange(() => VisibilityMedicalFileTab);
        }
        private void GetDefaultValueForTongKetBenhAn()
        {
            if (!string.IsNullOrEmpty(Diagnosis) && string.IsNullOrEmpty(CurrentHistoryAndPhysicalExaminationInfo.PathologicalProcessAndClinicalCourse))
            {
                CurrentHistoryAndPhysicalExaminationInfo.PathologicalProcessAndClinicalCourse = Diagnosis;
            }
            if (!string.IsNullOrEmpty(CurrentHistoryAndPhysicalExaminationInfo.TreatmentMethod) && string.IsNullOrEmpty(CurrentHistoryAndPhysicalExaminationInfo.Treatments))
            {
                CurrentHistoryAndPhysicalExaminationInfo.Treatments = CurrentHistoryAndPhysicalExaminationInfo.TreatmentMethod;
            }
            if (!string.IsNullOrEmpty(MainDisease) && string.IsNullOrEmpty(CurrentHistoryAndPhysicalExaminationInfo.DischargeDiagnostic_MainDisease))
            {
                CurrentHistoryAndPhysicalExaminationInfo.DischargeDiagnostic_MainDisease = MainDisease;
            }
            if (!string.IsNullOrEmpty(IncludingDisease) && string.IsNullOrEmpty(CurrentHistoryAndPhysicalExaminationInfo.DischargeDiagnostic_IncludingDisease))
            {
                CurrentHistoryAndPhysicalExaminationInfo.DischargeDiagnostic_IncludingDisease = IncludingDisease;
            }   
        }
        //▲====: #005
        //▼====: #006
        private IMinHourDateControl _ProgDateFromContent;
        public IMinHourDateControl ProgDateFromContent
        {
            get { return _ProgDateFromContent; }
            set
            {
                _ProgDateFromContent = value;
                NotifyOfPropertyChange(() => ProgDateFromContent);
            }
        }
        
        private IMinHourDateControl _ProgDateToContent;
        public IMinHourDateControl ProgDateToContent
        {
            get { return _ProgDateToContent; }
            set
            {
                _ProgDateToContent = value;
                NotifyOfPropertyChange(() => ProgDateToContent);
            }
        }
        //▲====: #006

        private void SetDefaultSelected()
        {
            if (ObjNoteTemplates_GetAll != null)
            {
                ObjNoteTemplates_Selected = ObjNoteTemplates_GetAll.FirstOrDefault();
            }

            if (ObjPrescriptionNoteTemplates_Treatments != null)
            {
                ObjPrescriptionNoteTemplates_Treatments_Selected = ObjPrescriptionNoteTemplates_Treatments.FirstOrDefault();
            }

            if (ObjPrescriptionNoteTemplates_TreatmentDirectionFollowupRegimen != null)
            {
                ObjPrescriptionNoteTemplates_TreatmentDirectionFollowupRegimen_Selected = ObjPrescriptionNoteTemplates_TreatmentDirectionFollowupRegimen.FirstOrDefault();
            }
        }
        //▼====: #007
        private ObservableCollection<Lookup> _V_OutDischargeType;
        public ObservableCollection<Lookup> V_OutDischargeType
        {
            get
            {
                return _V_OutDischargeType;
            }
            set
            {
                if (_V_OutDischargeType != value)
                {
                    _V_OutDischargeType = value;
                    NotifyOfPropertyChange(() => V_OutDischargeType);
                }
            }
        }
        private ObservableCollection<Lookup> _V_OutDischargeCondition;
        public ObservableCollection<Lookup> V_OutDischargeCondition
        {
            get
            {
                return _V_OutDischargeCondition;
            }
            set
            {
                if (_V_OutDischargeCondition != value)
                {
                    _V_OutDischargeCondition = value;
                    NotifyOfPropertyChange(() => V_OutDischargeCondition);
                }
            }
        }
        private Lookup _SelectedOutDischargeType;
        public Lookup SelectedOutDischargeType
        {
            get
            {
                return _SelectedOutDischargeType;
            }
            set
            {
                if (_SelectedOutDischargeType != value)
                {
                    _SelectedOutDischargeType = value;
                    if(CurrentHistoryAndPhysicalExaminationInfo != null && _SelectedOutDischargeType != null)
                    {
                        CurrentHistoryAndPhysicalExaminationInfo.V_OutDischargeType = _SelectedOutDischargeType.LookupID;
                        //CurrentHistoryAndPhysicalExaminationInfo.DischargeStatus = _SelectedOutDischargeType.ObjectValue;
                        CurrentHistoryAndPhysicalExaminationInfo.ConditionDischarge = _SelectedOutDischargeType.ObjectValue;
                    }
                    NotifyOfPropertyChange(() => SelectedOutDischargeType);
                }
            }
        }
        private Lookup _SelectedOutDischargeCondition;
        public Lookup SelectedOutDischargeCondition
        {
            get
            {
                return _SelectedOutDischargeCondition;
            }
            set
            {
                if (_SelectedOutDischargeCondition != value)
                {
                    _SelectedOutDischargeCondition = value;
                    if (CurrentHistoryAndPhysicalExaminationInfo != null && _SelectedOutDischargeCondition != null)
                    {
                        CurrentHistoryAndPhysicalExaminationInfo.V_OutDischargeCondition = _SelectedOutDischargeCondition.LookupID;
                    }
                    NotifyOfPropertyChange(() => SelectedOutDischargeCondition);
                }
            }
        }
        private void GetV_OutDischargeType()
        {
            V_OutDischargeType = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_OutDischargeType).ToObservableCollection();
            Lookup firstItem = new Lookup();
            firstItem.LookupID = 0;
            firstItem.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, "Vui lòng chọn tình trạng ra viện");
            V_OutDischargeType.Insert(0, firstItem);
            SelectedOutDischargeType = V_OutDischargeType.FirstOrDefault();
        }
        private void GetV_OutDischargeCondition()
        {
            V_OutDischargeCondition = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_OutDischargeCondition).ToObservableCollection();
            Lookup firstItem = new Lookup();
            firstItem.LookupID = 0;
            firstItem.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, "Vui lòng chọn kết quả điều trị");
            V_OutDischargeCondition.Insert(0, firstItem);
            SelectedOutDischargeCondition = V_OutDischargeCondition.FirstOrDefault();
        }
        public void btnSaveSummary()
        {
            EditHistoryAndPhysicalExaminationInfo(true);
        }
        public void btnSaveInfo()
        {
            EditHistoryAndPhysicalExaminationInfo(false);
        }
        //▲====: #007
        public bool VisibilitySumaryTab
        {
            get
            {
                return IsTongKetBenhAn && !VisibilityMaxillofacialTab;
            }
        }
        public bool VisibilityMedicalFileTab
        {
            get
            {
                return !IsTongKetBenhAn && !VisibilityMaxillofacialTab;
            }
        }
        //▼====: #008
        private bool _IsChronic;
        public bool IsChronic
        {
            get { return _IsChronic; }
            set
            {
                _IsChronic = value;
                NotifyOfPropertyChange(() => IsChronic);
                NotifyOfPropertyChange(() => VisibilityAdmissionTab);
                NotifyOfPropertyChange(() => VisibilityTicketCareTab);
                NotifyOfPropertyChange(() => VisibilitySelfDeclarationTab);
            }
        }
        public bool VisibilityAdmissionTab
        {
            get
            {
                return IsChronic || VisibilityMaxillofacialTab;
            }
        }
        public bool VisibilitySelfDeclarationTab
        {
            get
            {
                return IsChronic;
            }
        }
        public bool VisibilityTicketCareTab
        {
            get
            {
                return IsChronic || VisibilityMaxillofacialTab;
            }
        }
        //▲====: #008
        //▼====: #010
        private ObservableCollection<SummaryMedicalRecords> _ListSummaryMedicalRecord;
        public ObservableCollection<SummaryMedicalRecords> ListSummaryMedicalRecord
        {
            get { return _ListSummaryMedicalRecord; }
            set
            {
                _ListSummaryMedicalRecord = value;
                NotifyOfPropertyChange(() => ListSummaryMedicalRecord);
            }
        }
        private void GetSummaryMedicalRecordByOutPtTreatmentProgramID()
        {
            this.ShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                using (var CurrentFactory = new CommonUtilsServiceClient())
                {
                    var CurrentContract = CurrentFactory.ServiceInstance;
                    CurrentContract.BeginGetSummaryMedicalRecordByOutPtTreatmentProgramID(OutPtTreatmentProgramID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var Result = CurrentContract.EndGetSummaryMedicalRecordByOutPtTreatmentProgramID(asyncResult);
                            if (Result != null)
                            {
                                ListSummaryMedicalRecord = Result.ToObservableCollection();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            this.HideBusyIndicator();
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            CurrentThread.Start();
        }
        public void btnEditSummaryMedicalRecord_Click(object DataContext)
        {
            if (DataContext != null && DataContext is SummaryMedicalRecords)
            {
                SummaryMedicalRecords CurSummaryMedicalRecords = (SummaryMedicalRecords)DataContext;
                if(CurrentHistoryAndPhysicalExaminationInfo != null && CurSummaryMedicalRecords.SummaryMedicalRecordID == 0)
                {
                    CurSummaryMedicalRecords.PathologicalProcess = CurrentHistoryAndPhysicalExaminationInfo.HistoryAndPhysicalExamination;
                    CurSummaryMedicalRecords.AdmissionDiagnosis = DiagTrmtItem.DiagnosisFinal;
                    CurSummaryMedicalRecords.DischargeDiagnosis = DiagTrmtItem.DiagnosisFinal;
                }
                ISummaryMedicalRecords_OutPt mDialogView = Globals.GetViewModel<ISummaryMedicalRecords_OutPt>();
                mDialogView.PtRegistration = CurSummaryMedicalRecords.CurPatientRegistration;
                mDialogView.SetCurrentInformation(CurSummaryMedicalRecords.DeepCopy());
                GlobalsNAV.ShowDialog_V3(mDialogView);
                GetSummaryMedicalRecordByOutPtTreatmentProgramID();
            }
        }
        public void btnPrintSummaryMedicalRecord_Click(object DataContext)
        {
            if (DataContext != null && DataContext is SummaryMedicalRecords)
            {
                SummaryMedicalRecords CurSummaryMedicalRecords = (SummaryMedicalRecords)DataContext;
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                {
                    proAlloc.ID = (long)CurSummaryMedicalRecords.SummaryMedicalRecordID;
                    proAlloc.eItem = ReportName.XRpt_TomTatHoSoBenhAn;
                    proAlloc.V_RegistrationType = CurSummaryMedicalRecords.V_RegistrationType;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
            }
        }
        //▲====: #010

        //▼====: #011
        private ObservableCollection<PatientRegistration> _PatientRegistrationCollection;
        public ObservableCollection<PatientRegistration> PatientRegistrationCollection
        {
            get
            {
                return _PatientRegistrationCollection;
            }
            set
            {
                if (_PatientRegistrationCollection != value)
                {
                    _PatientRegistrationCollection = value;
                    NotifyOfPropertyChange(() => PatientRegistrationCollection);
                }
            }
        }

        private void GetRegistrationByOutPtTreatmentProgramID()
        {
            this.DlgShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                using (var CurrentFactory = new CommonUtilsServiceClient())
                {
                    var CurrentContract = CurrentFactory.ServiceInstance;
                    CurrentContract.BeginGetRegistrationByOutPtTreatmentProgramID(OutPtTreatmentProgramID, true, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            PatientRegistrationCollection = CurrentContract.EndGetRegistrationByOutPtTreatmentProgramID(asyncResult).ToObservableCollection();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.G0442_G1_TBao);
                        }
                        finally
                        {
                            this.DlgHideBusyIndicator();
                        }
                    }), null);
                }
            });
            CurrentThread.Start();
        }

        public void btnEdit_Click(object DataContext)
        {
            if (DataContext != null && DataContext is PatientRegistration)
            {
                PatientRegistration patientRegistration = ObjectCopier.DeepCopy(DataContext as PatientRegistration);
                IDischargePapersEdit dialogView = Globals.GetViewModel<IDischargePapersEdit>();
                dialogView.CurrentRegistration = patientRegistration;
                dialogView.DischargeDiagnostic = DiagTrmtItem != null ? DiagTrmtItem.DiagnosisFinal : null ;
                dialogView.GetDischargePapersInfo(patientRegistration.PtRegistrationID, (long)AllLookupValues.RegistrationType.NGOAI_TRU);
                GlobalsNAV.ShowDialog_V5(dialogView, null, null, new Dictionary<string, object> { ["Title"] = "Thông tin giấy ra viện" }, false, new Size(600, 500));
            }
        }

        public void btnPrint_Click(object DataContext)
        {
            if (DataContext != null && DataContext is PatientRegistration)
            {
                PatientRegistration patientRegistration = ObjectCopier.DeepCopy(DataContext as PatientRegistration);
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                {
                    proAlloc.RegistrationID = patientRegistration.PtRegistrationID;
                    proAlloc.V_RegistrationType = (long)patientRegistration.V_RegistrationType;
                    proAlloc.eItem = ReportName.XRpt_DisChargePapers;
                };
                GlobalsNAV.ShowDialog(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
            }
        }
        public void Handle(SaveDisChargePapersInfo_Event message)
        {
            if(message != null && message.Result)
            {
                GetRegistrationByOutPtTreatmentProgramID();
            }
        }
        //▲====: #011

        //▼====: #012
        public void cbo_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            PrescriptionNoteTemplates obj = (sender as ComboBox).SelectedItem as PrescriptionNoteTemplates;
            if (obj != null && obj.PrescriptNoteTemplateID > 0 && CurrentHistoryAndPhysicalExaminationInfo != null)
            {
                string str = CurrentHistoryAndPhysicalExaminationInfo.TreatmentMethod;
                if (string.IsNullOrEmpty(str))
                {
                    str = obj.DetailsTemplate;
                }
                else
                {
                    str = str + Environment.NewLine + obj.DetailsTemplate;
                }
                CurrentHistoryAndPhysicalExaminationInfo.TreatmentMethod = str;
            }
        }
        //▲====: #012
    }
}
