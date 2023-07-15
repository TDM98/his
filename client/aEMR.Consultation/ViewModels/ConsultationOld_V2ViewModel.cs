//using System;
//using System.Collections.Generic;
//using System.ComponentModel.Composition;
//using aEMR.CommonTasks;
//using aEMR.ViewContracts;
//using aEMR.ServiceClient;
//using Caliburn.Micro;
//using aEMR.Common.BaseModel;
//using aEMR.Common.Collections;
//using System.Collections.ObjectModel;
//using aEMR.Infrastructure;
//using aEMR.Infrastructure.Events;
//using System.Threading;
//using System.Windows;
//using System.Windows.Controls;
//using DataEntities;
//using aEMR.Common;
//using System.Linq;
//using aEMR.Controls;
//using eHCMSLanguage;
//using System.Windows.Media;
//using aEMR.ServiceClient.Consultation_PCLs;
//using Castle.Windsor;
//using System.Windows.Input;
//using System.ComponentModel;
///*
// * 20180913 TTM #004: Fixed cannot show information on dialog paperreferal full when user click on hyperlink
// */
//namespace aEMR.ConsultantEPrescription.ViewModels
//{
//    [Export(typeof(IConsultationOld_V2)), PartCreationPolicy(CreationPolicy.NonShared)]
//    public partial class ConsultationOld_V2ViewModel : ViewModelBase, IConsultationOld_V2
//        , IHandle<ConsultationDoubleClickEvent>
//        , IHandle<EventKhamChoVIP<PatientRegistration>>
//        , IHandle<GlobalCurPatientServiceRecordLoadComplete_Consult>
//        , IHandle<CommonClosedPhysicalForDiagnosisEvent>
//        , IHandle<ConsultationDoubleClickEvent_InPt_2>
//        , IHandle<GlobalCurPatientServiceRecordLoadComplete_Consult_InPt>
//        , IHandle<CommonClosedPhysicalForDiagnosis_InPtEvent>
//    {
//        #region Busy Indicator binding
//        public override bool IsProcessing
//        {
//            get
//            {
//                return false;
//                //return _IsWaitingGetLatestDiagnosisTreatmentByPtID
//                //    || _IsWaitingGetBlankDiagnosisTreatmentByPtID
//                //    || _IsWaitingSaveAddNew
//                //    || _IsWaitingUpdate
//                //    || _IsWaitingLoadICD10;
//            }
//        }

//        public override string StatusText
//        {
//            get
//            {
//                if (_IsWaitingGetLatestDiagnosisTreatmentByPtID)
//                {
//                    return eHCMSResources.Z0486_G1_LayChanDoanCuoi;
//                }
//                if (_IsWaitingGetBlankDiagnosisTreatmentByPtID)
//                {
//                    return eHCMSResources.K2882_G1_DangTaiDLieu;
//                }
//                if (_IsWaitingSaveAddNew)
//                {
//                    return string.Format(eHCMSResources.Z0487_G1_DangLuu, eHCMSResources.K1746_G1_CDoan);
//                }
//                if (_IsWaitingUpdate)
//                {
//                    return string.Format(eHCMSResources.Z0488_G1_DangCapNhat, eHCMSResources.K1746_G1_CDoan);
//                }
//                if (_IsWaitingLoadICD10)
//                {
//                    return string.Format(eHCMSResources.Z0489_G1_DangLoad, eHCMSResources.T1793_G1_ICD10);
//                }

//                return string.Empty;
//            }
//        }

//        private bool _IsWaitingLoadICD10;
//        public bool IsWaitingLoadICD10
//        {
//            get { return _IsWaitingLoadICD10; }
//            set
//            {
//                if (_IsWaitingLoadICD10 != value)
//                {
//                    _IsWaitingLoadICD10 = value;
//                    NotifyOfPropertyChange(() => IsWaitingLoadICD10);
//                    NotifyWhenBusy();
//                }
//            }
//        }

//        private bool _IsWaitingUpdate;
//        public bool IsWaitingUpdate
//        {
//            get { return _IsWaitingUpdate; }
//            set
//            {
//                if (_IsWaitingUpdate != value)
//                {
//                    _IsWaitingUpdate = value;
//                    NotifyOfPropertyChange(() => IsWaitingUpdate);
//                    NotifyWhenBusy();
//                }
//            }
//        }

//        private bool _IsWaitingSaveAddNew;
//        public bool IsWaitingSaveAddNew
//        {
//            get { return _IsWaitingSaveAddNew; }
//            set
//            {
//                if (_IsWaitingSaveAddNew != value)
//                {
//                    _IsWaitingSaveAddNew = value;
//                    NotifyOfPropertyChange(() => IsWaitingSaveAddNew);
//                    NotifyWhenBusy();
//                }
//            }
//        }

//        private bool _IsWaitingGetLatestDiagnosisTreatmentByPtID;
//        public bool IsWaitingGetLatestDiagnosisTreatmentByPtID
//        {
//            get { return _IsWaitingGetLatestDiagnosisTreatmentByPtID; }
//            set
//            {
//                if (_IsWaitingGetLatestDiagnosisTreatmentByPtID != value)
//                {
//                    _IsWaitingGetLatestDiagnosisTreatmentByPtID = value;
//                    NotifyOfPropertyChange(() => IsWaitingGetLatestDiagnosisTreatmentByPtID);
//                    NotifyWhenBusy();
//                }
//            }
//        }

//        private bool _IsWaitingGetBlankDiagnosisTreatmentByPtID;
//        public bool IsWaitingGetBlankDiagnosisTreatmentByPtID
//        {
//            get { return _IsWaitingGetBlankDiagnosisTreatmentByPtID; }
//            set
//            {
//                if (_IsWaitingGetBlankDiagnosisTreatmentByPtID != value)
//                {
//                    _IsWaitingGetBlankDiagnosisTreatmentByPtID = value;
//                    NotifyOfPropertyChange(() => IsWaitingGetBlankDiagnosisTreatmentByPtID);
//                    NotifyWhenBusy();
//                }
//            }
//        }
//        #endregion

//        private Visibility _VisibilyDiagnosisType = Visibility.Collapsed;
//        public Visibility VisibilyDiagnosisType
//        {
//            get { return _VisibilyDiagnosisType; }
//            set
//            {
//                _VisibilyDiagnosisType = value;
//                NotifyOfPropertyChange(() => VisibilyDiagnosisType);
//            }
//        }

//        private PhysicalExamination _pPhyExamItem;
//        public PhysicalExamination PtPhyExamItem
//        {
//            get
//            {
//                return _pPhyExamItem;
//            }
//            set
//            {
//                if (_pPhyExamItem != value)
//                {
//                    _pPhyExamItem = value;
//                    NotifyOfPropertyChange(() => PtPhyExamItem);
//                }
//            }
//        }
//        [ImportingConstructor]
//        public ConsultationOld_V2ViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
//        {
//            authorization();
//            LoadInitData();
//            GetAllLookupValuesByType();
//        }

//        protected override void OnActivate()
//        {
//            base.OnActivate();
//            Globals.EventAggregator.Subscribe(this);
//            if (IsOutPt)
//                InitPatientInfo();
//            else
//            {
//                DepartmentContent = Globals.GetViewModel<IDepartmentListing>();
//                DepartmentContent.AddSelectOneItem = true;
//                DepartmentContent.LoadData();
//                (DepartmentContent as PropertyChangedBase).PropertyChanged += new PropertyChangedEventHandler(DepartmentContent_PropertyChanged);
//            }
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
//        /*▼====: #003*/
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
//        /*▲====: #003*/

//        private IDepartmentListing _departmentContent;
//        public IDepartmentListing DepartmentContent
//        {
//            get { return _departmentContent; }
//            set
//            {
//                _departmentContent = value;
//                NotifyOfPropertyChange(() => DepartmentContent);
//            }
//        }

//        private void DepartmentContent_PropertyChanged(object sender, PropertyChangedEventArgs e)
//        {
//            if (DiagTrmtItem == null)
//            {
//                return;
//            }
//            if (e.PropertyName == "SelectedItem")
//            {
//                if (DiagTrmtItem.DTItemID > 0
//                    //==== 20161126 CMN Begin: Except Old DiagTrmt without Department
//                    && DiagTrmtItem.Department != null
//                    //==== 20161126 CMN End.
//                    && DiagTrmtItem.Department.DeptID != DepartmentContent.SelectedItem.DeptID
//                    //==== #001
//                    && !IsDiagnosisOutHospital
//                    //==== #001
//                    )
//                {
//                    /*▼====: #003*/
//                    if (IsDailyDiagnosis)
//                        MessageBox.Show(eHCMSResources.Z2159_G1_Msg_HChinhCDoanHangNgay);
//                    else
//                        /*▲====: #003*/
//                        MessageBox.Show(eHCMSResources.Z0401_G1_HChinhCDoanXK);
//                    DepartmentContent.SelectedItem = DiagTrmtItem.Department;
//                    return;
//                }
//                DiagTrmtItem.Department = DepartmentContent.SelectedItem;
//            }
//        }

//        private void GlobalCurPatientServiceRecordLoadComplete_Consult()
//        {
//            if (Globals.PatientAllDetails.PtRegistrationInfo == null)
//            {
//                MessageBox.Show(eHCMSResources.Z0412_G1_DLieuKBChuaDuocCBi, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                return;
//            }
//            InitPatientInfo();
//        }
//        public void Handle(GlobalCurPatientServiceRecordLoadComplete_Consult message)
//        {
//            GlobalCurPatientServiceRecordLoadComplete_Consult();
//        }
//        public void Handle(GlobalCurPatientServiceRecordLoadComplete_Consult_InPt message)
//        {
//            GlobalCurPatientServiceRecordLoadComplete_Consult();
//        }

//        public IEnumerator<IResult> MessageWarningShowDialogTask(string strMessage)
//        {
//            var dialog = new MessageWarningShowDialogTask(strMessage, eHCMSResources.K1576_G1_CBao, false);
//            yield return dialog;
//            yield break;
//        }

//        private bool ValidateExpiredDiagnosicTreatment(DiagnosisTreatment diagnosicTreatment)
//        {
//            DateTime curDate = Globals.GetCurServerDateTime();
//            if (diagnosicTreatment.DiagnosisDate.AddDays(Globals.ServerConfigSection.Hospitals.EditDiagDays) < curDate)
//            {
//                btEditIsEnabled = false;
//                Coroutine.BeginExecute(MessageWarningShowDialogTask(string.Format(eHCMSResources.Z2196_G1_CDoanHetHieuLucChinhSuaN, Globals.ServerConfigSection.Hospitals.EditDiagDays)));
//                return false;
//            }
//            return true;
//        }
//        public void CheckBeforeConsult()
//        {
//            if (IsOutPt)
//            {
//                //Kiem tra trang thai cua dang ky nay
//                if (Globals.curLstPatientServiceRecord != null && Globals.curLstPatientServiceRecord.Count > 0)
//                {

//                    DiagTrmtItem = ObjectCopier.DeepCopy(Globals.curLstPatientServiceRecord[0].DiagnosisTreatments[0]);
//                    DiagTrmtItemCopy = ObjectCopier.DeepCopy(DiagTrmtItem);

//                    ConsultState = ConsultationState.EditConsultationState;
//                    //dang ky dich vu nay da co chan doan
//                    //gan lai cho dang ky nay tu Globals
//                    StateEdit();

//                    DiagnosisIcd10Items_Load(DiagTrmtItem.ServiceRecID, null, false);

//                    ValidateExpiredDiagnosicTreatment(DiagTrmtItem);
//                    return;
//                }
//                else
//                {

//                    if (Globals.PatientAllDetails.PtRegistrationInfo == null)
//                    {
//                        return;
//                    }

//                    ConsultState = ConsultationState.NewConsultationState;
//                    StateNew();
//                    GetLatesDiagTrmtByPtID(Globals.PatientAllDetails.PatientInfo.PatientID);
//                    //KMx: Sau khi kiểm tra, thấy hàm này không cần thiết (01/11/2014 08:27).
//                    //GetPtRegistrationIDInDiagnosisTreatment_Latest(Globals.PatientAllDetails.PatientInfo.PatientID, Globals.PatientAllDetails.PtRegistrationInfo.PtRegistrationID);
//                    //kiem tra xem Ptregisdetail nay da co chan doan chua?neu co roi thi chi cho chinh sua thoi,khong cho tao moi j het
//                    if (Globals.PatientAllDetails.PtRegistrationDetailInfo != null && Globals.PatientAllDetails.PtRegistrationDetailInfo.PtRegDetailID > 0)
//                    {
//                        CheckDiagnosisTreatmentExists_PtRegDetailID(Globals.PatientAllDetails.PatientInfo.PatientID, Globals.PatientAllDetails.PtRegistrationDetailInfo.PtRegDetailID);
//                    }
//                }
//            }
//            else
//            {
//                if (Globals.PatientAllDetails.PtRegistrationInfo == null)
//                {
//                    return;
//                }

//                ConsultState = ConsultationState.NewAndEditConsultationState;
//                StateNewEdit();

//                if (Globals.curLstPatientServiceRecord == null
//                        || Globals.curLstPatientServiceRecord.Count <= 0
//                        || Globals.curLstPatientServiceRecord[0].DiagnosisTreatments == null
//                        || Globals.curLstPatientServiceRecord[0].DiagnosisTreatments.Count <= 0)
//                {
//                    if (IsDiagnosisOutHospital)
//                    {
//                        GetLatesDiagTrmtByPtID_InPt(Globals.PatientAllDetails.PatientInfo.PatientID, (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS);
//                    }
//                    else
//                    {
//                        GetLatesDiagTrmtByPtID_InPt(Globals.PatientAllDetails.PatientInfo.PatientID, (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTDEPT);
//                    }

//                    return;
//                }

//                //KMx: Màn hình chẩn đoán nhập/xuất viện 
//                if (IsDiagnosisOutHospital)
//                {
//                    if (Globals.curLstPatientServiceRecord[0].DiagnosisTreatments.Any(x => x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_IN)
//                        && Globals.curLstPatientServiceRecord[0].DiagnosisTreatments.Any(x => x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS))
//                    {
//                        ConsultState = ConsultationState.EditConsultationState;
//                        StateEdit();
//                    }

//                    //KMx: Nếu đăng ký hiện tại có chẩn đoán Nhập/ Xuất viện thì hiển thị chẩn đoán Nhập/ Xuất viện lên màn hình.
//                    if (Globals.curLstPatientServiceRecord[0].DiagnosisTreatments.Any(x => x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_IN)
//                        || Globals.curLstPatientServiceRecord[0].DiagnosisTreatments.Any(x => x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS))
//                    {

//                        DiagnosisTreatment item = Globals.curLstPatientServiceRecord[0].DiagnosisTreatments.Where(x => x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_IN
//                                                                                                                    || x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS).OrderByDescending(x => x.DTItemID).FirstOrDefault();

//                        DiagTrmtItem = ObjectCopier.DeepCopy(item);
//                        DiagTrmtItemCopy = ObjectCopier.DeepCopy(item);
//                        DiagnosisIcd10Items_Load_InPt(DiagTrmtItem.DTItemID);
//                        DiagnosisICD9Items_Load_InPt(DiagTrmtItem.DTItemID);

//                    }
//                    //KMx: Nếu đăng ký hiện tại không có chẩn đoán Nhập/ Xuất viện thì lấy chẩn đoán xuất viện cuối cùng của đăng ký trước.
//                    else
//                    {
//                        GetLatesDiagTrmtByPtID_InPt(Globals.PatientAllDetails.PatientInfo.PatientID, (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS);
//                    }
//                }
//                else
//                {
//                    //KMx: Nếu đăng ký hiện tại có chẩn đoán xuất khoa thì hiển thị chẩn đoán xuất khoa cuối cùng lên màn hình.
//                    /*▼====: #003*/
//                    long mDiagnosisType = (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTDEPT;
//                    if (IsDailyDiagnosis)
//                        mDiagnosisType = (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_DAILY;
//                    /*▲====: #003*/
//                    if (Globals.curLstPatientServiceRecord[0].DiagnosisTreatments.Any(x => x.V_DiagnosisType == mDiagnosisType))
//                    {
//                        DiagnosisTreatment item = Globals.curLstPatientServiceRecord[0].DiagnosisTreatments.Where(x => (x.V_DiagnosisType == mDiagnosisType || (mDiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTDEPT && x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_INDEPT))).OrderByDescending(x => x.DTItemID).FirstOrDefault();
//                        DiagTrmtItem = ObjectCopier.DeepCopy(item);
//                        DiagTrmtItemCopy = ObjectCopier.DeepCopy(item);
//                        DiagnosisIcd10Items_Load_InPt(DiagTrmtItem.DTItemID);
//                        DiagnosisICD9Items_Load_InPt(DiagTrmtItem.DTItemID);

//                    }
//                    //KMx: Nếu đăng ký hiện tại không có chẩn đoán xuất khoa thì lấy chẩn đoán xuất khoa cuối cùng của đăng ký trước.
//                    else
//                    {
//                        GetLatesDiagTrmtByPtID_InPt(Globals.PatientAllDetails.PatientInfo.PatientID, mDiagnosisType);
//                    }
//                }
//            }
//        }

//        public void InitPatientInfo()
//        {
//            if (IsOutPt)
//            {
//                refIDC10Code = new ObservableCollection<DiseasesReference>();
//                refIDC10Name = new ObservableCollection<DiseasesReference>();
//            }
//            else
//            {
//                refIDC10 = new PagedSortableCollectionView<DiseasesReference>();
//                refIDC10.OnRefresh += RefIDC10_OnRefresh;
//                refIDC10.PageSize = Globals.PageSize;
//                pageIDC9 = new PagedSortableCollectionView<RefICD9>();
//                pageIDC9.OnRefresh += PageIDC9_OnRefresh;
//                pageIDC9.PageSize = Globals.PageSize;
//                refICD9Item = new DiagnosisICD9Items();
//                refICD9List = new ObservableCollection<DiagnosisICD9Items>();
//            }
//            refIDC10Item = new DiagnosisIcd10Items();

//            refIDC10List = new ObservableCollection<DiagnosisIcd10Items>();
//            if (Globals.PatientAllDetails.PatientInfo != null && Globals.PatientAllDetails.PatientInfo.PatientID > 0)
//            {
//                InitPhyExam(Globals.PatientAllDetails.PatientInfo.PatientID);

//                if (CheckDangKyHopLe())
//                {
//                    ButtonControlsEnable = true;

//                    //Kiem tra phan quyen
//                    if (!mChanDoan_tabSuaKhamBenh_ThongTin)
//                    {
//                        Globals.ShowMessage(eHCMSResources.Z0413_G1_ChuaDuocPQuyenXemBA, "");
//                        return;
//                    }
//                    if (!Globals.isConsultationStateEdit)
//                    {
//                        MessageBox.Show(eHCMSResources.A0232_G1_Msg_InfoKhTheSua_BNTuLSBA);
//                        ButtonControlsEnable = false;
//                    }
//                    CheckBeforeConsult();
//                }
//                else
//                {
//                    this.HideBusyIndicator();
//                    FormEditorIsEnabled = false;
//                }
//            }
//            else
//            {
//                this.HideBusyIndicator();
//                FormEditorIsEnabled = false;
//            }

//        }

//        private void PageIDC9_OnRefresh(object sender, aEMR.Common.Collections.RefreshEventArgs e)
//        {
//            SearchICD9(procName, ICD9SearchType, pageIDC9.PageIndex, pageIDC9.PageSize);
//        }

//        private void RefIDC10_OnRefresh(object sender, aEMR.Common.Collections.RefreshEventArgs e)
//        {
//            LoadRefDiseases(Name, Type, refIDC10.PageIndex, refIDC10.PageSize);
//        }

//        private bool CheckDangKyHopLe()
//        {
//            return Globals.CheckValidRegistrationForConsultation(Globals.PatientAllDetails.PtRegistrationInfo, Globals.PatientAllDetails.PtRegistrationDetailInfo, Globals.PatientAllDetails.PatientInfo, Globals.curLstPatientServiceRecord, IsOutPt);
//            //if (Globals.PatientAllDetails.PtRegistrationInfo == null)
//            //{
//            //    MessageBox.Show(eHCMSResources.Z0402_G1_KgBietDKLoaiNao);
//            //    return false;
//            //}

//            //if (IsOutPt && Globals.PatientAllDetails.PtRegistrationInfo.V_RegistrationType != AllLookupValues.RegistrationType.NGOAI_TRU)
//            //{
//            //    MessageBox.Show(eHCMSResources.A0245_G1_Msg_InfoKhongPhaiNgTru_ChiXem, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//            //    return false;
//            //}
//            //else if (!IsOutPt && Globals.PatientAllDetails.PtRegistrationInfo.V_RegistrationType != AllLookupValues.RegistrationType.NOI_TRU)
//            //{
//            //    MessageBox.Show(eHCMSResources.A0246_G1_Msg_InfoBNKhongPhaiNoiTru_ChiXem, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//            //    return false;
//            //}

//            //if (IsOutPt)
//            //{
//            //    if (Globals.PatientAllDetails.PtRegistrationDetailInfo != null && Globals.PatientAllDetails.PtRegistrationDetailInfo.PtRegDetailID > 0) //Có đăng ký và có đăng ký DV khám
//            //    {
//            //        if (Globals.PatientAllDetails.PtRegistrationDetailInfo.RefMedicalServiceItem != null && Globals.PatientAllDetails.PtRegistrationDetailInfo.RefMedicalServiceItem.IsAllowToPayAfter == 0 && Globals.PatientAllDetails.PtRegistrationDetailInfo.PaidTime == null)
//            //        {
//            //            return false;
//            //        }

//            //        // Txd 25/05/2014 Replaced ConfigList
//            //        if ((Globals.curLstPatientServiceRecord == null || Globals.curLstPatientServiceRecord.Count < 1) && (Globals.PatientAllDetails.PtRegistrationDetailInfo.RefMedicalServiceItem == null || Globals.PatientAllDetails.PtRegistrationDetailInfo.RefMedicalServiceItem.IsAllowToPayAfter == 0)
//            //            && Globals.PatientAllDetails.PtRegistrationDetailInfo.PaidTime.Value.AddHours(Globals.ServerConfigSection.Hospitals.EffectedDiagHours) < Globals.GetCurServerDateTime())
//            //        {
//            //            MessageBox.Show(string.Format(eHCMSResources.Z0414_G1_DKHetHieuLuc, Globals.ServerConfigSection.Hospitals.EffectedDiagHours.ToString()));
//            //            return false;
//            //        }
//            //        return true;
//            //    }
//            //    else
//            //    {
//            //        MessageBox.Show(Globals.PatientAllDetails.PatientInfo.FullName.Trim() + string.Format("({0})", eHCMSResources.T3719_G1_Mau20NgTru) + Environment.NewLine + eHCMSResources.T1278_G1_ChuaDKDVKBNao, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//            //        return false;
//            //    }
//            //}
//            //else
//            //{
//            //    if ((Globals.PatientAllDetails.PtRegistrationInfo.V_RegistrationStatus != (long)AllLookupValues.RegistrationStatus.COMPLETED)
//            //    && (Globals.PatientAllDetails.PtRegistrationInfo.V_RegistrationStatus != (long)AllLookupValues.RegistrationStatus.INVALID)
//            //    && (Globals.PatientAllDetails.PtRegistrationInfo.V_RegistrationStatus != (long)AllLookupValues.RegistrationStatus.PENDING))
//            //    {
//            //        return true;
//            //    }
//            //    else
//            //    {
//            //        switch (Globals.PatientAllDetails.PtRegistrationInfo.V_RegistrationStatus)
//            //        {
//            //            case (long)AllLookupValues.RegistrationStatus.COMPLETED:
//            //                {
//            //                    MessageBox.Show("'" + Globals.PatientAllDetails.PatientInfo.FullName.Trim() + "'" + eHCMSResources.A0065_G1_Msg_InfoKhongCDDc_DKDaDong);
//            //                    break;
//            //                }
//            //            case (long)AllLookupValues.RegistrationStatus.INVALID:
//            //                {
//            //                    MessageBox.Show("'" + Globals.PatientAllDetails.PatientInfo.FullName.Trim() + "'" + eHCMSResources.A0066_G1_Msg_InfoKhongCDDc_DKKhHopLe);
//            //                    break;
//            //                }
//            //            case (long)AllLookupValues.RegistrationStatus.PENDING:
//            //                {
//            //                    MessageBox.Show("'" + Globals.PatientAllDetails.PatientInfo.FullName.Trim() + "'" + eHCMSResources.A0064_G1_Msg_InfoKhongCDDc_DKChuaHTat);
//            //                    break;
//            //                }
//            //        }
//            //        return false;
//            //    }
//            //}
//        }

//        public void hpkEditPhysicalExam()
//        {
//            Action<IcwPhysiscalExam> onInitDlg = delegate (IcwPhysiscalExam proAlloc)
//            {
//                proAlloc.PatientID = Globals.PatientAllDetails.PatientInfo.PatientID;

//                if (PtPhyExamItem == null)
//                {
//                    proAlloc.IsVisibility = Visibility.Collapsed;
//                    proAlloc.isEdit = false;
//                }
//                else
//                {
//                    proAlloc.PtPhyExamItem = ObjectCopier.DeepCopy(PtPhyExamItem);
//                    proAlloc.IsVisibility = Visibility.Visible;
//                    proAlloc.isEdit = true;
//                }
//            };
//            GlobalsNAV.ShowDialog<IcwPhysiscalExam>(onInitDlg);
//        }

//        #region Properties member
//        private ObservableCollection<Lookup> _RefBehaving;
//        public ObservableCollection<Lookup> RefBehaving
//        {
//            get
//            {
//                return _RefBehaving;
//            }
//            set
//            {
//                if (_RefBehaving != value)
//                {
//                    _RefBehaving = value;
//                    NotifyOfPropertyChange(() => RefBehaving);
//                }
//            }
//        }

//        private ObservableCollection<Lookup> _RefDiagnosis;
//        public ObservableCollection<Lookup> RefDiagnosis
//        {
//            get
//            {
//                return _RefDiagnosis;
//            }
//            set
//            {
//                if (_RefDiagnosis != value)
//                {
//                    _RefDiagnosis = value;
//                    NotifyOfPropertyChange(() => RefDiagnosis);
//                }
//            }
//        }

//        private ObservableCollection<MedicalRecordTemplate> _RefMedRecTemplate;
//        public ObservableCollection<MedicalRecordTemplate> RefMedRecTemplate
//        {
//            get
//            {
//                return _RefMedRecTemplate;
//            }
//            set
//            {
//                if (_RefMedRecTemplate != value)
//                {
//                    _RefMedRecTemplate = value;
//                    NotifyOfPropertyChange(() => RefMedRecTemplate);
//                }
//            }
//        }

//        private ObservableCollection<DiagnosisIcd10Items> _refIDC10ListCopy;
//        public ObservableCollection<DiagnosisIcd10Items> refIDC10ListCopy
//        {
//            get
//            {
//                return _refIDC10ListCopy;
//            }
//            set
//            {
//                if (_refIDC10ListCopy != value)
//                {
//                    _refIDC10ListCopy = value;
//                    NotifyOfPropertyChange(() => refIDC10ListCopy);
//                }
//            }
//        }

//        private ObservableCollection<DiagnosisICD9Items> _refICD9ListCopy;
//        public ObservableCollection<DiagnosisICD9Items> refICD9ListCopy
//        {
//            get
//            {
//                return _refICD9ListCopy;
//            }
//            set
//            {
//                if (_refICD9ListCopy != value)
//                {
//                    _refICD9ListCopy = value;
//                    NotifyOfPropertyChange(() => refICD9ListCopy);
//                }
//            }
//        }

//        private DiagnosisTreatment _DiagTrmtItem;
//        public DiagnosisTreatment DiagTrmtItem
//        {
//            get
//            {
//                return _DiagTrmtItem;
//            }
//            set
//            {
//                _DiagTrmtItem = value;
//                NotifyOfPropertyChange(() => DiagTrmtItem);
//                NotifyOfPropertyChange(() => btCreateNewByOldIsEnabled);
//                NotifyOfPropertyChange(() => IsInPtDiagnosis);

//                SetDepartment();
//            }
//        }

//        public bool IsInPtDiagnosis
//        {
//            get
//            {
//                return DiagTrmtItem != null && DiagTrmtItem.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU && IsOutPt;
//            }
//        }

//        private DiagnosisTreatment _DiagTrmtItemCopy;
//        public DiagnosisTreatment DiagTrmtItemCopy
//        {
//            get
//            {
//                return _DiagTrmtItemCopy;
//            }
//            set
//            {
//                if (_DiagTrmtItemCopy != value)
//                {
//                    _DiagTrmtItemCopy = value;
//                }
//                NotifyOfPropertyChange(() => DiagTrmtItemCopy);
//            }
//        }

//        private bool _ButtonControlsEnable = false;
//        public bool ButtonControlsEnable
//        {
//            get
//            {
//                return _ButtonControlsEnable;
//            }
//            set
//            {
//                _ButtonControlsEnable = value;
//                NotifyOfPropertyChange(() => ButtonControlsEnable);
//            }
//        }

//        private bool _FormEditorIsEnabled = false;
//        public bool FormEditorIsEnabled
//        {
//            get
//            {
//                return _FormEditorIsEnabled;
//            }
//            set
//            {
//                _FormEditorIsEnabled = value;
//                NotifyOfPropertyChange(() => FormEditorIsEnabled);
//            }
//        }

//        private bool _btSaveCreateNewIsEnabled = true;
//        public bool btSaveCreateNewIsEnabled
//        {
//            get
//            {
//                return _btSaveCreateNewIsEnabled;
//            }
//            set
//            {
//                _btSaveCreateNewIsEnabled = value;
//                NotifyOfPropertyChange(() => btSaveCreateNewIsEnabled);
//            }
//        }

//        private bool _btCreateNewIsEnabled;
//        public bool btCreateNewIsEnabled
//        {
//            get { return _btCreateNewIsEnabled; }
//            set
//            {
//                _btCreateNewIsEnabled = value;
//                NotifyOfPropertyChange(() => btCreateNewIsEnabled);
//            }
//        }

//        private bool _hasDiag = true;
//        public bool hasDiag
//        {
//            get { return _hasDiag; }
//            set
//            {
//                if (_hasDiag != value)
//                {
//                    _hasDiag = value;
//                    NotifyOfPropertyChange(() => hasDiag);
//                    NotifyOfPropertyChange(() => btCreateNewByOldIsEnabled);
//                }
//            }
//        }

//        private bool _btCreateNewByOldIsEnabled;
//        public bool btCreateNewByOldIsEnabled
//        {
//            get { return _btCreateNewByOldIsEnabled; }
//            set
//            {
//                _btCreateNewByOldIsEnabled = value && hasDiag;
//                NotifyOfPropertyChange(() => btCreateNewByOldIsEnabled);
//            }
//        }
//        private bool _btEditIsEnabled;
//        public bool btEditIsEnabled
//        {
//            get
//            {
//                return _btEditIsEnabled;
//            }
//            set
//            {
//                _btEditIsEnabled = value;
//                NotifyOfPropertyChange(() => btEditIsEnabled);
//            }
//        }

//        private bool _btCancelIsEnabled = true;
//        public bool btCancelIsEnabled
//        {
//            get
//            {
//                return _btCancelIsEnabled;
//            }
//            set
//            {
//                _btCancelIsEnabled = value;
//                NotifyOfPropertyChange(() => btCancelIsEnabled);
//            }
//        }

//        private bool _btUpdateIsEnabled = false;
//        public bool btUpdateIsEnabled
//        {
//            get
//            {
//                return _btUpdateIsEnabled;
//            }
//            set
//            {
//                _btUpdateIsEnabled = value;
//                NotifyOfPropertyChange(() => btUpdateIsEnabled);
//            }
//        }


//        #endregion

//        public void authorization()
//        {
//            if (!Globals.isAccountCheck)
//            {
//                return;
//            }
//            mChanDoan_tabSuaKhamBenh_ThongTin = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
//                                               , (int)eConsultation.mPtPMRConsultationNew,
//                                               (int)oConsultationEx.mChanDoan_tabSuaKhamBenh_ThongTin, (int)ePermission.mView);
//            mChanDoan_tabSuaKhamBenh_HieuChinh = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
//                                                   , (int)eConsultation.mPtPMRConsultationNew,
//                                                   (int)oConsultationEx.mChanDoan_tabSuaKhamBenh_HieuChinh, (int)ePermission.mView);
//            mChanDoan_ChiDinhXetNghiemCLS = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
//                                                   , (int)eConsultation.mPtPMRConsultationNew,
//                                                   (int)oConsultationEx.mChanDoan_ChiDinhXetNghiemCLS, (int)ePermission.mView);
//            mChanDoan_RaToa = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
//                                                   , (int)eConsultation.mPtPMRConsultationNew,
//                                                   (int)oConsultationEx.mChanDoan_RaToa, (int)ePermission.mView);
//            mChanDoan_TaoBenhAn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
//                                                   , (int)eConsultation.mPtPMRConsultationNew,
//                                                   (int)oConsultationEx.mChanDoan_TaoBenhAn, (int)ePermission.mView);
//            mChanDoan_XemKetQuaCLS = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
//                                                   , (int)eConsultation.mPtPMRConsultationNew,
//                                                   (int)oConsultationEx.mChanDoan_XemKetQuaCLS, (int)ePermission.mView);
//            mChanDoan_XemToaThuoc_HienHanh = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
//                                                   , (int)eConsultation.mPtPMRConsultationNew,
//                                                   (int)oConsultationEx.mChanDoan_XemToaThuoc_HienHanh, (int)ePermission.mView);
//            mChanDoan_XemBenhAn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
//                                                   , (int)eConsultation.mPtPMRConsultationNew,
//                                                   (int)oConsultationEx.mChanDoan_XemBenhAn, (int)ePermission.mView);
//        }

//        #region account checking

//        private bool _mChanDoan_tabSuaKhamBenh_ThongTin = true;
//        private bool _mChanDoan_tabSuaKhamBenh_HieuChinh = true;
//        private bool _mChanDoan_ChiDinhXetNghiemCLS = true;
//        private bool _mChanDoan_RaToa = true;
//        private bool _mChanDoan_TaoBenhAn = true;
//        private bool _mChanDoan_XemKetQuaCLS = true;
//        private bool _mChanDoan_XemToaThuoc_HienHanh = true;
//        private bool _mChanDoan_XemBenhAn = true;

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
//        public bool mChanDoan_tabSuaKhamBenh_HieuChinh
//        {
//            get
//            {
//                return _mChanDoan_tabSuaKhamBenh_HieuChinh;
//            }
//            set
//            {
//                if (_mChanDoan_tabSuaKhamBenh_HieuChinh == value)
//                    return;
//                _mChanDoan_tabSuaKhamBenh_HieuChinh = value;
//            }
//        }
//        public bool mChanDoan_ChiDinhXetNghiemCLS
//        {
//            get
//            {
//                return _mChanDoan_ChiDinhXetNghiemCLS;
//            }
//            set
//            {
//                if (_mChanDoan_ChiDinhXetNghiemCLS == value)
//                    return;
//                _mChanDoan_ChiDinhXetNghiemCLS = value;
//            }
//        }
//        public bool mChanDoan_RaToa
//        {
//            get
//            {
//                return _mChanDoan_RaToa;
//            }
//            set
//            {
//                if (_mChanDoan_RaToa == value)
//                    return;
//                _mChanDoan_RaToa = value;
//            }
//        }
//        public bool mChanDoan_TaoBenhAn
//        {
//            get
//            {
//                return _mChanDoan_TaoBenhAn;
//            }
//            set
//            {
//                if (_mChanDoan_TaoBenhAn == value)
//                    return;
//                _mChanDoan_TaoBenhAn = value;
//            }
//        }
//        public bool mChanDoan_XemKetQuaCLS
//        {
//            get
//            {
//                return _mChanDoan_XemKetQuaCLS;
//            }
//            set
//            {
//                if (_mChanDoan_XemKetQuaCLS == value)
//                    return;
//                _mChanDoan_XemKetQuaCLS = value;
//            }
//        }
//        public bool mChanDoan_XemToaThuoc_HienHanh
//        {
//            get
//            {
//                return _mChanDoan_XemToaThuoc_HienHanh;
//            }
//            set
//            {
//                if (_mChanDoan_XemToaThuoc_HienHanh == value)
//                    return;
//                _mChanDoan_XemToaThuoc_HienHanh = value;
//            }
//        }
//        public bool mChanDoan_XemBenhAn
//        {
//            get
//            {
//                return _mChanDoan_XemBenhAn;
//            }
//            set
//            {
//                if (_mChanDoan_XemBenhAn == value)
//                    return;
//                _mChanDoan_XemBenhAn = value;
//            }
//        }
//        #endregion


//        private void CopyListICD10()
//        {
//            if (refIDC10List != null)
//            {
//                refIDC10ListCopy = refIDC10List.DeepCopy();
//            }
//            else
//            {
//                refIDC10ListCopy = null;
//            }
//            AddBlankRow();
//        }

//        private void CopyListICD10ForNew()
//        {
//            if (refIDC10List != null)
//            {
//                refIDC10ListCopy = refIDC10List.DeepCopy();
//            }
//            else
//            {
//                refIDC10ListCopy = null;
//            }
//            //refIDC10List = refIDC10ListLatestCopy.DeepCopy();
//            refIDC10List = new ObservableCollection<DiagnosisIcd10Items>();
//            AddBlankRow();

//        }

//        private void CopyListICD9()
//        {
//            if (refICD9List != null)
//            {
//                refICD9ListCopy = refICD9List.DeepCopy();
//            }
//            else
//            {
//                RefICD9Copy = null;
//            }
//            AddICD9BlankRow();
//        }

//        private void CopyListICD9ForNew()
//        {
//            if (refICD9List != null)
//            {
//                refICD9ListCopy = refICD9List.DeepCopy();
//            }
//            else
//            {
//                refICD9ListCopy = null;
//            }
//            refICD9List = new ObservableCollection<DiagnosisICD9Items>();
//            AddICD9BlankRow();
//        }

//        private bool NeedICD10()
//        {
//            //if (Globals.ConfigList != null && Convert.ToInt16(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.NeedICD10]) > 0)

//            // Txd 25/05/2014 Replaced ConfigList
//            if (Globals.ServerConfigSection.Hospitals.NeedICD10 > 0)
//            {
//                if (refIDC10List != null)
//                {
//                    var temp = refIDC10List.Where(x => x.DiseasesReference != null);
//                    if (temp == null || temp.Count() == 0)
//                    {
//                        MessageBox.Show(eHCMSResources.A0199_G1_Msg_YCNhapICD10);
//                        return false;
//                    }
//                    else
//                    {
//                        return true;
//                    }
//                }
//                else
//                {
//                    return true;
//                }
//            }
//            else
//            { return true; }

//        }

//        private void UpdateDoctorStaffID()
//        {
//            DiagTrmtItem.DoctorStaffID = Globals.LoggedUserAccount.StaffID.Value;
//            DiagTrmtItem.ObjDoctorStaffID.StaffID = Globals.LoggedUserAccount.StaffID.Value;
//        }

//        private bool _IsNotExistsDiagnosisTreatmentByPtRegDetailID = true;
//        public bool IsNotExistsDiagnosisTreatmentByPtRegDetailID
//        {
//            get { return _IsNotExistsDiagnosisTreatmentByPtRegDetailID; }
//            set
//            {
//                _IsNotExistsDiagnosisTreatmentByPtRegDetailID = value;
//                NotifyOfPropertyChange(() => btCreateNewIsEnabled);
//                NotifyOfPropertyChange(() => btCreateNewByOldIsEnabled);
//            }
//        }

//        private void CheckDiagnosisTreatmentExists_PtRegDetailID(long patientID, long PtRegDetailID)
//        {
//            this.ShowBusyIndicator();
//            //IsWaitingGetBlankDiagnosisTreatmentByPtID = true;

//            var t = new Thread(() =>
//            {
//                using (var serviceFactory = new ePMRsServiceClient())
//                {
//                    var contract = serviceFactory.ServiceInstance;

//                    contract.BeginCheckDiagnosisTreatmentExists_PtRegDetailID(patientID, PtRegDetailID, Globals.DispatchCallback((asyncResult) =>
//                    {

//                        try
//                        {
//                            IsNotExistsDiagnosisTreatmentByPtRegDetailID = contract.EndCheckDiagnosisTreatmentExists_PtRegDetailID(asyncResult);
//                        }
//                        catch (Exception ex)
//                        {
//                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//                        }
//                        finally
//                        {
//                            //IsWaitingGetBlankDiagnosisTreatmentByPtID = false;
//                            this.HideBusyIndicator();
//                        }

//                    }), null);

//                }

//            });

//            t.Start();
//        }


//        public void GetLatesDiagTrmtByPtID(long patientID)
//        {
//            this.ShowBusyIndicator();

//            //IsWaitingGetLatestDiagnosisTreatmentByPtID = true;

//            var t = new Thread(() =>
//            {
//                using (var serviceFactory = new ePMRsServiceClient())
//                {
//                    var contract = serviceFactory.ServiceInstance;

//                    contract.BeginGetLatestDiagnosisTreatmentByPtID(patientID, null, "", 0, true, Globals.DispatchCallback((asyncResult) =>
//                    {
//                        try
//                        {
//                            DiagTrmtItem = contract.EndGetLatestDiagnosisTreatmentByPtID(asyncResult);
//                            NotifyOfPropertyChange(() => btCreateNewByOldIsEnabled);
//                            if (DiagTrmtItem != null && DiagTrmtItem.DTItemID > 0)
//                            {
//                                //Có DiagnosisTreatment rồi
//                                //FormEditorIsEnabled = false;
//                                if (DiagTrmtItem.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU && DiagTrmtItem.DTItemID > 0)
//                                    DiagnosisIcd10Items_Load_InPt(DiagTrmtItem.DTItemID);
//                                else
//                                    DiagnosisIcd10Items_Load(DiagTrmtItem.ServiceRecID, null, false);
//                                hasDiag = true;
//                                ButtonForHasDiag();
//                            }
//                            else
//                            {

//                                hasDiag = false;
//                                //Form Trạng Thái New

//                                DiagnosisIcd10Items_Load(null, Globals.PatientAllDetails.PatientInfo.PatientID, true);

//                                //KMx: Không cần gọi về server, chỉ cần sử dụng hàm ResetDefaultForBehaving() trong ruột GetBlankDiagnosisTreatmentByPtID() (01/11/2014 10:16).
//                                //GetBlankDiagnosisTreatmentByPtID(Globals.PatientAllDetails.PatientInfo.PatientID);     
//                                ResetDefaultForBehaving();

//                                //FormEditorIsEnabled = true;
//                                //ButtonForNotDiag(false);
//                                StateNewWaiting();
//                            }

//                            DiagTrmtItemCopy = ObjectCopier.DeepCopy(DiagTrmtItem);

//                        }
//                        catch (Exception ex)
//                        {
//                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//                        }
//                        finally
//                        {
//                            //Globals.IsBusy = false;
//                            //IsWaitingGetLatestDiagnosisTreatmentByPtID = false;
//                            this.HideBusyIndicator();
//                        }

//                    }), null);

//                }

//            });

//            t.Start();
//        }

//        public void GetLatesDiagTrmtByPtID_InPt(long patientID, long? V_DiagnosisType)
//        {
//            this.ShowBusyIndicator();

//            //IsWaitingGetLatestDiagnosisTreatmentByPtID = true;

//            var t = new Thread(() =>
//            {
//                using (var serviceFactory = new ePMRsServiceClient())
//                {
//                    var contract = serviceFactory.ServiceInstance;

//                    contract.BeginGetLatestDiagnosisTreatmentByPtID_InPt(patientID, V_DiagnosisType, Globals.DispatchCallback((asyncResult) =>
//                    {
//                        try
//                        {
//                            DiagTrmtItem = contract.EndGetLatestDiagnosisTreatmentByPtID_InPt(asyncResult);
//                            NotifyOfPropertyChange(() => btCreateNewByOldIsEnabled);
//                            if (DiagTrmtItem != null && DiagTrmtItem.DTItemID > 0)
//                            {
//                                //Có DiagnosisTreatment rồi
//                                //FormEditorIsEnabled = false;

//                                //DiagnosisIcd10Items_Load_InPt(DiagTrmtItem.ServiceRecID.GetValueOrDefault());
//                                DiagnosisIcd10Items_Load_InPt(DiagTrmtItem.DTItemID);
//                                DiagnosisICD9Items_Load_InPt(DiagTrmtItem.DTItemID);
//                                hasDiag = true;
//                                ButtonForHasDiag();
//                            }
//                            else
//                            {

//                                hasDiag = false;

//                                AddBlankRow();

//                                AddICD9BlankRow();

//                                ResetDefaultForBehaving();

//                                StateNewWaiting();
//                            }

//                            DiagTrmtItemCopy = ObjectCopier.DeepCopy(DiagTrmtItem);

//                        }
//                        catch (Exception ex)
//                        {
//                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//                        }
//                        finally
//                        {
//                            this.HideBusyIndicator();
//                        }

//                    }), null);

//                }

//            });

//            t.Start();
//        }

//        private void SetDefaultDiagnosisType()
//        {
//            if (DiagTrmtItem == null)
//            {
//                return;
//            }

//            if (RefDiagnosis != null && Globals.PatientAllDetails.PtRegistrationInfo != null && Globals.PatientAllDetails.PtRegistrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
//            {
//                //KMx: Màn hình chẩn đoán chia làm 2 link. 1 link "Chẩn đoán xuất khoa" và 1 link "Chẩn đoán xuất viện", tùy theo link mà chọn loại chẩn đoán phù hợp (09/06/2015 17:38).
//                //Dời V_DiagnosisType từ PatientServiceRecord sang DiagnosisTreatment.
//                //if (RefDiagnosis.Any(x => x.LookupID == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS))
//                //{
//                //    DiagTrmtItem.PatientServiceRecord.V_DiagnosisType = (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS;
//                //}
//                //else
//                //{
//                //    DiagTrmtItem.PatientServiceRecord.V_DiagnosisType = RefDiagnosis.FirstOrDefault().LookupID;
//                //}
//                if (IsDiagnosisOutHospital)
//                {
//                    if (RefDiagnosis.Any(x => x.LookupID == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS))
//                    {
//                        DiagTrmtItem.V_DiagnosisType = (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS;
//                    }
//                    else
//                    {
//                        DiagTrmtItem.V_DiagnosisType = RefDiagnosis.FirstOrDefault().LookupID;
//                    }
//                }
//                /*▼====: #003*/
//                else if (IsDailyDiagnosis)
//                {
//                    if (RefDiagnosis.Any(x => x.LookupID == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_DAILY))
//                    {
//                        DiagTrmtItem.V_DiagnosisType = (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_DAILY;
//                    }
//                    else
//                    {
//                        DiagTrmtItem.V_DiagnosisType = RefDiagnosis.FirstOrDefault().LookupID;
//                    }
//                }
//                /*▲====: #003*/
//                else
//                {
//                    if (RefDiagnosis.Any(x => x.LookupID == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTDEPT))
//                    {
//                        DiagTrmtItem.V_DiagnosisType = (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTDEPT;
//                    }
//                    else
//                    {
//                        DiagTrmtItem.V_DiagnosisType = RefDiagnosis.FirstOrDefault().LookupID;
//                    }
//                }
//            }
//            else
//            {
//                //DiagTrmtItem.PatientServiceRecord.V_DiagnosisType = (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_NORMAL;

//                DiagTrmtItem.V_DiagnosisType = (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_NORMAL;
//            }
//        }

//        private void ResetDefaultForBehaving()
//        {
//            if (DiagTrmtItem == null)
//            {
//                return;
//            }

//            if (!IsOutPt)
//                DiagTrmtItem.DiagnosisDate = Globals.GetCurServerDateTime();

//            if (DiagTrmtItem.PatientServiceRecord == null)
//            {
//                DiagTrmtItem.PatientServiceRecord = new PatientServiceRecord();
//                DiagTrmtItem.PatientServiceRecord.ExamDate = Globals.GetCurServerDateTime();
//            }

//            if (DiagTrmtItem.PatientServiceRecord.PatientMedicalRecord == null)
//            {
//                DiagTrmtItem.PatientServiceRecord.PatientMedicalRecord = new PatientMedicalRecord();
//            }

//            if (RefBehaving != null)
//            {
//                DiagTrmtItem.PatientServiceRecord.V_Behaving = RefBehaving.FirstOrDefault().LookupID;
//            }
//            else
//            {
//                DiagTrmtItem.PatientServiceRecord.V_Behaving = (long)AllLookupValues.Behaving.KHAM_DIEU_TRI;
//            }

//            if (IsOutPt)
//            {
//                if (RefDiagnosis != null && Globals.PatientAllDetails.PtRegistrationInfo != null && Globals.PatientAllDetails.PtRegistrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
//                {
//                    if (RefDiagnosis.Any(x => x.LookupID == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS))
//                    {
//                        DiagTrmtItem.PatientServiceRecord.V_DiagnosisType = (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS;
//                    }
//                    else
//                    {
//                        DiagTrmtItem.PatientServiceRecord.V_DiagnosisType = RefDiagnosis.FirstOrDefault().LookupID;
//                    }
//                }
//                else
//                {
//                    DiagTrmtItem.PatientServiceRecord.V_DiagnosisType = (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_NORMAL;
//                }
//            }
//            else
//            {
//                SetDefaultDiagnosisType();
//            }

//            if (RefMedRecTemplate != null)
//            {
//                DiagTrmtItem.MDRptTemplateID = RefMedRecTemplate.FirstOrDefault().MDRptTemplateID;
//            }
//            else
//            {
//                DiagTrmtItem.MDRptTemplateID = 1;
//            }
//        }

//        private IEnumerator<IResult> AllCheck()
//        {
//            if (IsOutPt)
//            {
//                if (DiagTrmtItem == null)
//                {
//                    yield break;
//                }
//                if (Globals.PatientAllDetails.PtRegistrationInfo != null && Globals.IsLockRegistration(Globals.PatientAllDetails.PtRegistrationInfo.RegLockFlag, eHCMSResources.Z0407_G1_CNhatCDoan))
//                {
//                    yield break;
//                }
//            }
//            else
//            {
//                if (!CheckEditDiagnosis())
//                {
//                    yield break;
//                }

//                if (!CheckDiagnosisType())
//                {
//                    yield break;
//                }

//                if (!CheckDepartment())
//                {
//                    yield break;
//                }
//            }

//            if (!CheckEmptyFields())
//            {
//                yield break;
//            }
//            if (NeedICD10() && CheckedIsMain() && (IsOutPt || CheckedICD9IsMain()))
//            {
//                //UpdateDoctorStaffID();
//                DiagTrmtItem.ICD10List = String.Join(",", from item in refIDC10List
//                                                          where (!string.IsNullOrEmpty(item.ICD10Code) && item.DiseasesReference != null)
//                                                          select item.ICD10Code);

//                //KMx: Phải set loại đăng ký để service gọi stored update tương ứng (01/11/2014 10:30).
//                DiagTrmtItem.PatientServiceRecord.V_RegistrationType = Globals.PatientAllDetails.PtRegistrationInfo.V_RegistrationType;
//                if (IsOutPt)
//                    DiagTrmtItem.PatientServiceRecord.V_DiagnosisType = (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_NORMAL;

//                //DiagTrmtItem.PatientServiceRecord.PatientMedicalRecord.PatientID = Globals.PatientAllDetails.PatientInfo.PatientID;
//                PatientRegistration CurRegistration = Globals.PatientAllDetails.PtRegistrationInfo;

//                // Hpt 22/09/2015: Nếu bệnh nhân có quyền lợi về BHYT thì mới kiểm tra xem trong danh sách có ICD10 nào là Z10 hay không vì BHYT sẽ không chi trả cho những trường hợp này
//                if (CurRegistration.PtInsuranceBenefit != null && CurRegistration.PtInsuranceBenefit > 0 && refIDC10List != null
//                    && refIDC10List.Any(x => x.ICD10Code != null && x.DiseasesReference != null && x.DiseasesReference.ICD10Code.Contains("Z10")))
//                {
//                    warningtask = new MessageWarningShowDialogTask(content, eHCMSResources.Z0339_G1_TiepTucLuuCDoan);
//                    yield return warningtask;
//                    if (!warningtask.IsAccept)
//                    {
//                        yield break;
//                    }
//                }
//                StateEditWaiting();
//                UpdateDiagTrmt();
//            }
//        }


//        public void UpdateDiagTrmt()
//        {
//            this.ShowBusyIndicator();

//            //KMx: Lưu BS cập nhật sau cùng (28/03/2014 13:59).
//            DiagTrmtItem.DoctorStaffID = Globals.LoggedUserAccount.StaffID.Value;

//            IsWaitingUpdate = true;

//            long listID = Compare2Object();

//            if (IsOutPt)
//            {
//                var t = new Thread(() =>
//                {
//                    using (var serviceFactory = new ePMRsServiceClient())
//                    {
//                        var contract = serviceFactory.ServiceInstance;
//                        DiagTrmtItem.DeptLocationID = Globals.DeptLocation.DeptLocationID;

//                        contract.BeginUpdateDiagnosisTreatment(DiagTrmtItem, listID, refIDC10List, Globals.DispatchCallback((asyncResult) =>
//                        {
//                            try
//                            {
//                                if (contract.EndUpdateDiagnosisTreatment(asyncResult))
//                                {
//                                    FormEditorIsEnabled = false;

//                                    StateEdit();

//                                    if (refIDC10List != null)
//                                    {
//                                        refIDC10List = refIDC10List.Where(x => x.DiseasesReference != null).ToObservableCollection();
//                                    }

//                                    //phat su kien reload lai danh sach 
//                                    //Globals.EventAggregator.Publish(new ReloadDataConsultationEvent { });

//                                    //KMx: Sau khi lưu chẩn đoán, reload Service Record (22/05/2014 09:48).
//                                    IConsultationModule consultVM = Globals.GetViewModel<IConsultationModule>();

//                                    consultVM.PatientServiceRecordsGetForKhamBenh_Ext();

//                                    //KMx: Sau khi lưu chẩn đoán, xóa danh sách chẩn đoán. Vì danh sách không tự động load lại.
//                                    //Nếu không xóa thì chẩn đoán vừa lưu sẽ khác với chẩn đoán trong danh sách (22/05/2014 09:48).
//                                    Globals.EventAggregator.Publish(new ClearAllDiagnosisListAfterUpdateEvent());

//                                    //Nếu đang là Popup thì phát event lấy cđ này gán vào khám bệnh
//                                    if (Globals.ConsultationIsChildWindow)
//                                    {
//                                        Globals.EventAggregator.Publish(new DiagnosisTreatmentSelectedEvent<DiagnosisTreatment> { DiagnosisTreatment = DiagTrmtItem.DeepCopy() });
//                                    }

//                                    MessageBox.Show(eHCMSResources.K2782_G1_DaCNhat);
//                                }
//                                else
//                                {
//                                    if (DiagTrmtItem.PatientServiceRecord.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_IN)
//                                    {
//                                        MessageBox.Show(eHCMSResources.Z0403_G1_DaCoCDoanNV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                                    }
//                                    else if (DiagTrmtItem.PatientServiceRecord.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS)
//                                    {
//                                        MessageBox.Show(eHCMSResources.Z0404_G1_DaCoCDoanXV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                                    }
//                                    else
//                                    {
//                                        MessageBox.Show(eHCMSResources.A0269_G1_Msg_InfoCNhatCDFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                                    }
//                                }
//                            }
//                            catch (Exception ex)
//                            {
//                                MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                            }
//                            finally
//                            {
//                                //Globals.IsBusy = false;
//                                //IsWaitingUpdate = false;
//                                this.HideBusyIndicator();
//                            }

//                        }), null);

//                    }

//                });

//                t.Start();
//            }
//            else
//            {
//                long DiagnosisICD9ListID = Compare2ICD9List();
//                var t = new Thread(() =>
//                {
//                    using (var serviceFactory = new ePMRsServiceClient())
//                    {
//                        var contract = serviceFactory.ServiceInstance;
//                        DiagTrmtItem.DeptLocationID = Globals.DeptLocation.DeptLocationID;

//                        contract.BeginUpdateDiagnosisTreatment_InPt(DiagTrmtItem, listID, refIDC10List, DiagnosisICD9ListID, refICD9List, Globals.DispatchCallback((asyncResult) =>
//                        {
//                            try
//                            {
//                                List<InPatientDeptDetail> ReloadInPatientDeptDetails = new List<InPatientDeptDetail>();
//                                if (contract.EndUpdateDiagnosisTreatment_InPt(out ReloadInPatientDeptDetails, asyncResult))
//                                {
//                                    FormEditorIsEnabled = false;

//                                    StateNewEdit();

//                                    if (refIDC10List != null)
//                                    {
//                                        refIDC10List = refIDC10List.Where(x => x.DiseasesReference != null).ToObservableCollection();
//                                    }

//                                    if (refICD9List != null)
//                                    {
//                                        refICD9List = refICD9List.Where(x => x.RefICD9 != null).ToObservableCollection();
//                                    }

//                                    //KMx: Sau khi lưu chẩn đoán, reload Service Record (22/05/2014 09:48).
//                                    IConsultationModule consultVM = Globals.GetViewModel<IConsultationModule>();

//                                    consultVM.PatientServiceRecordsGetForKhamBenh_Ext();

//                                    //KMx: Sau khi lưu chẩn đoán, xóa danh sách chẩn đoán. Vì danh sách không tự động load lại.
//                                    //Nếu không xóa thì chẩn đoán vừa lưu sẽ khác với chẩn đoán trong danh sách (22/05/2014 09:48).
//                                    Globals.EventAggregator.Publish(new ClearAllDiagnosisListAfterUpdateEvent_InPt());

//                                    Globals.PatientAllDetails.PtRegistrationInfo.AdmissionInfo.InPatientDeptDetails = new ObservableCollection<InPatientDeptDetail>(ReloadInPatientDeptDetails);

//                                    MessageBox.Show(eHCMSResources.K2782_G1_DaCNhat);
//                                }
//                                else
//                                {
//                                    //KMx: Dời V_DiagnosisType từ PatientServiceRecord sang DiagnosisTreatment (09/06/2015 17:42).
//                                    //if (DiagTrmtItem.PatientServiceRecord.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_IN)
//                                    if (DiagTrmtItem.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_IN)
//                                    {
//                                        MessageBox.Show(eHCMSResources.Z0403_G1_DaCoCDoanNV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                                    }
//                                    //else if (DiagTrmtItem.PatientServiceRecord.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS)
//                                    else if (DiagTrmtItem.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS)
//                                    {
//                                        MessageBox.Show(eHCMSResources.Z0404_G1_DaCoCDoanXV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                                    }
//                                    else
//                                    {
//                                        MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                                    }
//                                }
//                            }
//                            catch (Exception ex)
//                            {
//                                MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                            }
//                            finally
//                            {
//                                this.HideBusyIndicator();
//                            }

//                        }), null);

//                    }

//                });

//                t.Start();
//            }
//        }

//        public void Handle(CommonClosedPhysicalForDiagnosisEvent message)
//        {
//            InitPhyExam(Globals.PatientAllDetails.PatientInfo.PatientID);
//        }
//        public void Handle(CommonClosedPhysicalForDiagnosis_InPtEvent message)
//        {
//            InitPhyExam(Globals.PatientAllDetails.PatientInfo.PatientID);
//        }
//        private void InitPhyExam(long patientID)
//        {
//            this.ShowBusyIndicator();
//            var t = new Thread(() =>
//            {
//                using (var serviceFactory = new SummaryServiceClient())
//                {
//                    var contract = serviceFactory.ServiceInstance;
//                    contract.BeginGetLastPhyExamByPtID(patientID, Globals.DispatchCallback((asyncResult) =>
//                    {

//                        try
//                        {
//                            PtPhyExamItem = contract.EndGetLastPhyExamByPtID(asyncResult);
//                            Globals.curPhysicalExamination = PtPhyExamItem;
//                        }
//                        catch (Exception ex)
//                        {
//                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//                        }
//                        finally
//                        {
//                            this.HideBusyIndicator();
//                        }
//                    }), null);
//                }
//            });
//            t.Start();
//        }

//        private void DiagnosisIcd10Items_Load(long? ServiceRecID, long? PatientID, bool Last)
//        {
//            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
//            this.ShowBusyIndicator();
//            //IsWaitingLoadICD10 = true;
//            var t = new Thread(() =>
//            {
//                using (var serviceFactory = new ePMRsServiceClient())
//                {
//                    var contract = serviceFactory.ServiceInstance;
//                    contract.BeginGetDiagnosisIcd10Items_Load(ServiceRecID, PatientID, Last, Globals.DispatchCallback((asyncResult) =>
//                    {
//                        try
//                        {
//                            var results = contract.EndGetDiagnosisIcd10Items_Load(asyncResult);
//                            refIDC10List = results.ToObservableCollection();
//                            refIDC10ListCopy = refIDC10List.DeepCopy();
//                            //refIDC10ListLatestCopy = refIDC10List.DeepCopy();
//                            if (ServiceRecID == null)/*Chưa có chẩn đoán nào*/
//                            {
//                                CopyListICD10();
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//                        }
//                        finally
//                        {
//                            //Globals.IsBusy = false;
//                            //IsWaitingLoadICD10 = false;
//                            this.HideBusyIndicator();
//                        }
//                    }), null);
//                }
//            });
//            t.Start();
//        }
//        private void DiagnosisIcd10Items_Load_InPt(long DTItemID)
//        {
//            this.ShowBusyIndicator();
//            var t = new Thread(() =>
//            {
//                using (var serviceFactory = new ePMRsServiceClient())
//                {
//                    var contract = serviceFactory.ServiceInstance;
//                    contract.BeginGetDiagnosisIcd10Items_Load_InPt(DTItemID, Globals.DispatchCallback((asyncResult) =>
//                    {
//                        try
//                        {
//                            var results = contract.EndGetDiagnosisIcd10Items_Load_InPt(asyncResult);
//                            refIDC10List = results.ToObservableCollection();
//                            refIDC10ListCopy = refIDC10List.DeepCopy();
//                        }
//                        catch (Exception ex)
//                        {
//                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//                        }
//                        finally
//                        {
//                            this.HideBusyIndicator();
//                        }
//                    }), null);
//                }
//            });
//            t.Start();
//        }

//        private void DiagnosisICD9Items_Load_InPt(long DTItemID)
//        {
//            if (DTItemID <= 0)
//            {
//                return;
//            }

//            this.ShowBusyIndicator();

//            var t = new Thread(() =>
//            {
//                using (var serviceFactory = new ePMRsServiceClient())
//                {
//                    var contract = serviceFactory.ServiceInstance;

//                    contract.BeginGetDiagnosisICD9Items_Load_InPt(DTItemID, Globals.DispatchCallback((asyncResult) =>
//                    {
//                        try
//                        {
//                            var results = contract.EndGetDiagnosisICD9Items_Load_InPt(asyncResult);
//                            refICD9List = results.ToObservableCollection();
//                            refICD9ListCopy = refICD9List.DeepCopy();

//                        }
//                        catch (Exception ex)
//                        {
//                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//                        }
//                        finally
//                        {
//                            this.HideBusyIndicator();
//                        }

//                    }), null);

//                }

//            });

//            t.Start();
//        }

//        #region Event Double click từ ds chẩn đoán
//        public void Handle(ConsultationDoubleClickEvent obj)
//        {
//            btCancel();
//            if (obj.DiagTrmtItem != null)
//            {
//                DiagTrmtItem = ObjectCopier.DeepCopy(obj.DiagTrmtItem);
//                if (ConsultState != ConsultationState.NewConsultationState)
//                {
//                    ValidateExpiredDiagnosicTreatment(DiagTrmtItem);
//                }
//                refIDC10List = obj.refIDC10List;
//                DiagTrmtItemCopy = ObjectCopier.DeepCopy(DiagTrmtItem);
//                refIDC10ListCopy = ObjectCopier.DeepCopy(refIDC10List);
//            }
//        }
//        public void Handle(ConsultationDoubleClickEvent_InPt_2 obj)
//        {
//            hasDiag = true;
//            btCancel();
//            if (obj.DiagTrmtItem != null)
//            {
//                DiagTrmtItem = ObjectCopier.DeepCopy(obj.DiagTrmtItem);
//                refIDC10List = obj.refIDC10List;
//                refICD9List = obj.refICD9List;
//                DiagTrmtItemCopy = ObjectCopier.DeepCopy(DiagTrmtItem);
//                refIDC10ListCopy = ObjectCopier.DeepCopy(refIDC10List);
//                refICD9ListCopy = ObjectCopier.DeepCopy(refICD9List);
//            }
//        }
//        #endregion

//        #region List ICD10 member

//        private ObservableCollection<DiagnosisIcd10Items> _refIDC10List;
//        public ObservableCollection<DiagnosisIcd10Items> refIDC10List
//        {
//            get
//            {
//                return _refIDC10List;
//            }
//            set
//            {
//                if (_refIDC10List != value)
//                {
//                    _refIDC10List = value;
//                }
//                NotifyOfPropertyChange(() => refIDC10List);
//            }
//        }

//        private DiagnosisIcd10Items _refIDC10Item;
//        public DiagnosisIcd10Items refIDC10Item
//        {
//            get
//            {
//                return _refIDC10Item;
//            }
//            set
//            {
//                if (_refIDC10Item != value)
//                {
//                    _refIDC10Item = value;
//                }
//                NotifyOfPropertyChange(() => refIDC10Item);
//            }
//        }

//        private PagedSortableCollectionView<DiseasesReference> _refIDC10;
//        public PagedSortableCollectionView<DiseasesReference> refIDC10
//        {
//            get
//            {
//                return _refIDC10;
//            }
//            set
//            {
//                if (_refIDC10 != value)
//                {
//                    _refIDC10 = value;
//                }
//                NotifyOfPropertyChange(() => refIDC10);
//            }
//        }

//        private ObservableCollection<DiseasesReference> _refIDC10Code;
//        public ObservableCollection<DiseasesReference> refIDC10Code
//        {
//            get
//            {
//                return _refIDC10Code;
//            }
//            set
//            {
//                if (_refIDC10Code != value)
//                {
//                    _refIDC10Code = value;
//                }
//                NotifyOfPropertyChange(() => refIDC10Code);
//            }
//        }

//        private ObservableCollection<DiseasesReference> _refIDC10Name;

//        public ObservableCollection<DiseasesReference> refIDC10Name
//        {
//            get
//            {
//                return _refIDC10Name;
//            }
//            set
//            {
//                if (_refIDC10Name != value)
//                {
//                    _refIDC10Name = value;
//                }
//                NotifyOfPropertyChange(() => refIDC10Name);
//            }
//        }

//        public void LoadRefDiseases(string name, byte type, int PageIndex, int PageSize)
//        {
//            if (IsOutPt)
//            {
//                var t = new Thread(() =>
//                {
//                    using (var serviceFactory = new CommonUtilsServiceClient())
//                    {
//                        var contract = serviceFactory.ServiceInstance;

//                        contract.BeginSearchRefDiseases(name, PageIndex, PageSize, type, Globals.DispatchCallback((asyncResult) =>
//                        {

//                            try
//                            {
//                                int Total = 10;
//                                var results = contract.EndSearchRefDiseases(out Total, asyncResult);

//                                if (type == 0)
//                                {
//                                    refIDC10Code.Clear();
//                                    if (results != null)
//                                    {
//                                        foreach (DiseasesReference p in results)
//                                        {
//                                            refIDC10Code.Add(p);
//                                        }
//                                    }
//                                    if (refIDC10Code.Count > 0)
//                                    {
//                                        this.grdConsultation.bIcd10CodeAcbPopulated = true;
//                                    }
//                                    Acb_ICD10_Code.ItemsSource = refIDC10Code;
//                                    Acb_ICD10_Code.PopulateComplete();
//                                }
//                                else
//                                {
//                                    refIDC10Name.Clear();
//                                    if (results != null)
//                                    {
//                                        foreach (DiseasesReference p in results)
//                                        {
//                                            refIDC10Name.Add(p);
//                                        }
//                                    }
//                                    if (refIDC10Code.Count > 0)
//                                    {
//                                        this.grdConsultation.bIcd10CodeAcbPopulated = true;
//                                    }
//                                    Acb_ICD10_Name.ItemsSource = refIDC10Name;
//                                    Acb_ICD10_Name.PopulateComplete();
//                                }

//                            }
//                            catch (Exception ex)
//                            {
//                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//                            }
//                            finally
//                            {

//                            }

//                        }), null);

//                    }

//                });

//                t.Start();
//            }
//            else
//            {
//                var t = new Thread(() =>
//                {
//                    using (var serviceFactory = new CommonUtilsServiceClient())
//                    {
//                        var contract = serviceFactory.ServiceInstance;

//                        contract.BeginSearchRefDiseases(name, PageIndex, PageSize, type, Globals.DispatchCallback((asyncResult) =>
//                        {

//                            try
//                            {
//                                int Total = 10;
//                                var results = contract.EndSearchRefDiseases(out Total, asyncResult);
//                                refIDC10.Clear();
//                                refIDC10.TotalItemCount = Total;
//                                if (results != null)
//                                {
//                                    foreach (DiseasesReference p in results)
//                                    {
//                                        refIDC10.Add(p);
//                                    }
//                                }
//                                if (type == 0)
//                                {
//                                    Auto.ItemsSource = refIDC10;
//                                    Auto.PopulateComplete();
//                                }
//                                else
//                                {
//                                    AutoName.ItemsSource = refIDC10;
//                                    AutoName.PopulateComplete();
//                                }

//                            }
//                            catch (Exception ex)
//                            {
//                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//                            }
//                            finally
//                            {

//                            }

//                        }), null);

//                    }

//                });

//                t.Start();
//            }
//        }

//        AutoCompleteBox Acb_ICD10_Code = null;
//        AutoCompleteBox Acb_ICD10_Name = null;

//        AutoCompleteBox Auto;
//        AutoCompleteBox DiseasesName;
//        private string Name = "";
//        private byte Type = 0;
//        public void AcbICD10Code_Loaded(object sender, RoutedEventArgs e)
//        {
//            Acb_ICD10_Code = (AutoCompleteBox)sender;
//        }
//        public void AcbICD10Name_Loaded(object sender, RoutedEventArgs e)
//        {
//            Acb_ICD10_Name = (AutoCompleteBox)sender;
//        }
//        private string _typedText;
//        public string TypedText
//        {
//            get { return _typedText; }
//            set
//            {
//                _typedText = value.ToUpper();
//                NotifyOfPropertyChange(() => TypedText);
//            }
//        }

//        public void grdConsultation_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
//        {
//            if (grdConsultation != null && grdConsultation.SelectedItem != null)
//            {
//                grdConsultation.BeginEdit();
//            }
//        }

//        public void aucICD10_Populating(object sender, PopulatingEventArgs e)
//        {
//            if (IsCode)
//            {
//                e.Cancel = true;
//                Auto = (AutoCompleteBox)sender;
//                Name = e.Parameter;
//                Type = 0;
//                LoadRefDiseases(e.Parameter, 0, 0, IsOutPt ? 100 : refIDC10.PageSize);
//            }
//        }

//        AutoCompleteBox AutoName;
//        public void aucICD10Name_Populating(object sender, PopulatingEventArgs e)
//        {
//            if (!IsCode && ColumnIndex == 1)
//            {
//                e.Cancel = true;
//                AutoName = (AutoCompleteBox)sender;
//                Name = e.Parameter;
//                Type = 1;
//                if (refIDC10 != null)
//                    refIDC10.PageIndex = 0;
//                LoadRefDiseases(e.Parameter, 1, 0, IsOutPt ? 100 : refIDC10.PageSize);
//            }
//        }

//        public void AutoCompleteBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            if (IsCode)
//            {
//                if (refIDC10Item != null && Acb_ICD10_Code != null)
//                {
//                    refIDC10Item.DiseasesReference = new DiseasesReference();

//                    refIDC10Item.DiseasesReference = Acb_ICD10_Code.SelectedItem as DiseasesReference;
//                }
//            }
//        }

//        private bool isDropDown = false;
//        public void AxAutoComplete_DropDownClosing(object sender, RoutedPropertyChangedEventArgs<bool> e)
//        {
//            isDropDown = true;
//        }
//        public void AxAutoComplete_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
//        {
//            if (!isDropDown)
//            {
//                return;
//            }
//            isDropDown = false;

//            if (refIDC10Item != null && Acb_ICD10_Code != null)
//            {
//                refIDC10Item.DiseasesReference = new DiseasesReference();
//                refIDC10Item.DiseasesReference = Acb_ICD10_Code.SelectedItem as DiseasesReference;
//                if (CheckExists(refIDC10Item))
//                {
//                    GetDiagTreatmentFinal(refIDC10Item.DiseasesReference);
//                }
//            }
//        }

//        private bool isDiseaseDropDown = false;
//        public void DiseaseName_DropDownClosing(object sender, RoutedPropertyChangedEventArgs<bool> e)
//        {
//            isDiseaseDropDown = true;
//        }

//        public void DiseaseName_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
//        {
//            if (!isDiseaseDropDown)
//            {
//                return;
//            }
//            isDiseaseDropDown = false;

//            refIDC10Item.DiseasesReference = ((AutoCompleteBox)sender).SelectedItem as DiseasesReference;
//            if (CheckExists(refIDC10Item))
//            {
//                GetDiagTreatmentFinal(refIDC10Item.DiseasesReference);
//            }
//        }
//        public void AutoName_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            if (!IsCode)
//            {
//                if (refIDC10Item != null)
//                {
//                    refIDC10Item.DiseasesReference = Acb_ICD10_Name.SelectedItem as DiseasesReference;
//                }
//            }
//        }
//        public void AutoName_DropDownClosing(object sender, RoutedPropertyChangingEventArgs<bool> e)
//        {
//            if (!IsCode)
//            {
//                if (refIDC10Item != null)
//                {
//                    refIDC10Item.DiseasesReference = Acb_ICD10_Name.SelectedItem as DiseasesReference;
//                }
//            }
//        }


//        private void AddBlankRow()
//        {
//            if (refIDC10List != null
//                && refIDC10List.LastOrDefault() != null
//                && refIDC10List.LastOrDefault().DiseasesReference == null)
//            {
//                return;
//            }
//            DiagnosisIcd10Items ite = new DiagnosisIcd10Items();
//            ite.V_DiagIcdStatus = (long)AllLookupValues.V_DiagIcdStatus.DANGDIEUTRI;
//            ite.LookupStatus = new Lookup();
//            ite.LookupStatus.LookupID = (long)AllLookupValues.V_DiagIcdStatus.DANGDIEUTRI;
//            ite.LookupStatus.ObjectValue = eHCMSResources.Z0540_G1_DangDTri.ToUpper();
//            refIDC10List.Add(ite);
//        }

//        private bool CheckExists(DiagnosisIcd10Items Item, bool HasMessage = true)
//        {
//            int i = 0;
//            if (Item.DiseasesReference == null)
//            {
//                return true;
//            }
//            foreach (DiagnosisIcd10Items p in refIDC10List)
//            {
//                if (p.DiseasesReference != null)
//                {
//                    if (Item.DiseasesReference.ICD10Code == p.DiseasesReference.ICD10Code)
//                    {
//                        i++;
//                    }
//                }
//            }
//            if (i > 1)
//            {
//                Item.DiseasesReference = null;
//                if (HasMessage)
//                {
//                    MessageBox.Show(eHCMSResources.A0810_G1_Msg_InfoMaICDDaTonTai);
//                }
//                return false;
//            }
//            else
//            {
//                return true;
//            }
//        }

//        private DiseasesReference DiseasesReferenceCopy = null;

//        bool IsCode = true;
//        int ColumnIndex = 0;
//        public void AxDataGridNyICD10_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            DiagnosisIcd10Items item = ((DataGrid)sender).SelectedItem as DiagnosisIcd10Items;
//            if (item != null && item.DiseasesReference != null)
//            {
//                DiseasesReferenceCopy = item.DiseasesReference;
//                DiagnosisFinalNew = DiagnosisFinalOld = ObjectCopier.DeepCopy(item.DiseasesReference.DiseaseNameVN);
//                DiseasesReferenceCopy = ObjectCopier.DeepCopy(item.DiseasesReference);
//            }
//            else
//            {
//                DiagnosisFinalNew = DiagnosisFinalOld = "";
//                DiseasesReferenceCopy = null;
//            }
//        }
//        public void AxDataGridNy_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
//        {
//            ColumnIndex = e.Column.DisplayIndex;

//            if (refIDC10Item != null)
//            {
//                DiseasesReferenceCopy = refIDC10Item.DiseasesReference.DeepCopy();
//            }
//            if (e.Column.DisplayIndex == 0)
//            {
//                IsCode = true;
//            }
//            else
//            {
//                IsCode = false;
//            }
//        }

//        public void GetDiagTreatmentFinal(DiseasesReference diseasesReference)
//        {
//            if (diseasesReference != null)
//            {
//                DiagnosisFinalNew = diseasesReference.DiseaseNameVN;
//                if (DiagnosisFinalOld != "")
//                {
//                    DiagTrmtItem.DiagnosisFinal = DiagTrmtItem.DiagnosisFinal.Replace(DiagnosisFinalOld, DiagnosisFinalNew);
//                }
//                else
//                {
//                    if (string.IsNullOrWhiteSpace(DiagTrmtItem.DiagnosisFinal))
//                    {
//                        DiagTrmtItem.DiagnosisFinal += DiagnosisFinalNew;
//                    }
//                    else
//                    {
//                        DiagTrmtItem.DiagnosisFinal += "- " + DiagnosisFinalNew;
//                    }
//                }
//                DiagnosisFinalOld = ObjectCopier.DeepCopy(DiagnosisFinalNew);
//            }

//        }
//        #region get DiagnosisFinal
//        private string DiagnosisFinalOld = "";
//        private string DiagnosisFinalNew = "";

//        #endregion


//        public void AxDataGridNy_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
//        {
//            DiagnosisIcd10Items item = e.Row.DataContext as DiagnosisIcd10Items;

//            if (ColumnIndex == 0 || ColumnIndex == 1)
//            {
//                if (refIDC10Item.DiseasesReference == null)
//                {
//                    if (DiseasesReferenceCopy != null)
//                    {
//                        refIDC10Item.DiseasesReference = ObjectCopier.DeepCopy(DiseasesReferenceCopy);
//                        if (CheckExists(refIDC10Item, false))
//                        {
//                            GetDiagTreatmentFinal(refIDC10Item.DiseasesReference);
//                        }
//                    }
//                }
//            }
//            if (refIDC10Item != null && refIDC10Item.DiseasesReference != null)
//            {
//                if (CheckExists(refIDC10Item, false))
//                {
//                    if (e.Row.GetIndex() == (refIDC10List.Count - 1) && e.EditAction == DataGridEditAction.Commit)
//                    {
//                        System.Windows.Application.Current.Dispatcher.Invoke(() => AddBlankRow());
//                    }
//                }
//            }

//        }

//        #endregion

//        #region List Status Member

//        private ObservableCollection<Lookup> _DiagIcdStatusList;
//        public ObservableCollection<Lookup> DiagIcdStatusList
//        {
//            get
//            {
//                return _DiagIcdStatusList;
//            }
//            set
//            {
//                if (_DiagIcdStatusList != value)
//                {
//                    _DiagIcdStatusList = value;
//                    NotifyOfPropertyChange(() => DiagIcdStatusList);
//                }
//            }
//        }

//        private void GetAllLookupValuesByType()
//        {
//            ObservableCollection<Lookup> DiagICDSttLookupList = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_DiagIcdStatus).ToObservableCollection();
//            if (DiagICDSttLookupList == null || DiagICDSttLookupList.Count <= 0)
//            {
//                if (IsOutPt)
//                    MessageBox.Show(eHCMSResources.A0751_G1_Msg_InfoKhTimThayStatusICD11, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                else
//                    MessageBox.Show(eHCMSResources.A0750_G1_Msg_InfoKhTimThayStatusICD10, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                return;
//            }
//            DiagIcdStatusList = DiagICDSttLookupList;
//        }


//        public void UserControl_Loaded(object sender, RoutedEventArgs e)
//        {
//            ((ComboBox)sender).ItemsSource = DiagIcdStatusList;
//            if (refIDC10Item != null && DiagIcdStatusList != null)
//            {
//                if (refIDC10Item.LookupStatus == null)
//                {
//                    ((ComboBox)sender).SelectedIndex = 0;
//                }
//                else
//                {
//                    ((ComboBox)sender).SelectedItem = refIDC10Item.LookupStatus;
//                }
//            }
//        }

//        #endregion

//        private bool Equal(DiagnosisIcd10Items a, DiagnosisIcd10Items b)
//        {
//            return a.DiagIcd10ItemID == b.DiagIcd10ItemID
//                && a.DiagnosisIcd10ListID == b.DiagnosisIcd10ListID
//                && a.ICD10Code == b.ICD10Code
//                && a.IsMain == b.IsMain
//                && a.IsCongenital == b.IsCongenital
//                && (a.LookupStatus != null && b.LookupStatus != null
//                    && a.LookupStatus.LookupID == b.LookupStatus.LookupID);
//        }

//        private bool ICD9Equal(DiagnosisICD9Items a, DiagnosisICD9Items b)
//        {
//            return a.DiagICD9ItemID == b.DiagICD9ItemID
//                && a.DiagnosisICD9ListID == b.DiagnosisICD9ListID
//                && a.ICD9Code == b.ICD9Code
//                && a.IsMain == b.IsMain
//                && a.IsCongenital == b.IsCongenital;
//        }

//        public long Compare2Object()
//        {
//            long ListID = 0;
//            ObservableCollection<DiagnosisIcd10Items> temp = refIDC10List.Where(x => x.DiseasesReference != null).ToObservableCollection();
//            if (refIDC10ListCopy != null && refIDC10ListCopy.Count > 0 && refIDC10ListCopy.Count == temp.Count)
//            {
//                int icount = 0;
//                for (int i = 0; i < refIDC10ListCopy.Count; i++)
//                {
//                    for (int j = 0; j < temp.Count; j++)
//                    {
//                        if (Equal(refIDC10ListCopy[i], refIDC10List[j]))
//                        {
//                            icount++;
//                        }
//                    }

//                }
//                if (icount == refIDC10ListCopy.Count)
//                {
//                    ListID = refIDC10ListCopy.FirstOrDefault().DiagnosisIcd10ListID;
//                    return ListID;
//                }
//                else
//                {
//                    return 0;
//                }
//            }
//            else
//            {
//                return 0;
//            }

//        }

//        private long Compare2ICD9List()
//        {
//            long ListID = 0;
//            ObservableCollection<DiagnosisICD9Items> temp = refICD9List.Where(x => x.RefICD9 != null).ToObservableCollection();
//            if (refICD9ListCopy != null && refICD9ListCopy.Count > 0 && refICD9ListCopy.Count == temp.Count)
//            {
//                int icount = 0;
//                for (int i = 0; i < refICD9ListCopy.Count; i++)
//                {
//                    for (int j = 0; j < temp.Count; j++)
//                    {
//                        if (ICD9Equal(refICD9ListCopy[i], refICD9List[j]))
//                        {
//                            icount++;
//                        }
//                    }

//                }
//                if (icount == refICD9ListCopy.Count)
//                {
//                    ListID = refICD9ListCopy.FirstOrDefault().DiagnosisICD9ListID;
//                    return ListID;
//                }
//                else
//                {
//                    return 0;
//                }
//            }
//            else
//            {
//                return 0;
//            }

//        }

//        private bool CheckedIsMain()
//        {
//            ObservableCollection<DiagnosisIcd10Items> temp = refIDC10List.Where(x => x.DiseasesReference != null).ToObservableCollection();
//            if (temp != null && temp.Count > 0)
//            {
//                int bcount = 0;
//                for (int i = 0; i < temp.Count; i++)
//                {
//                    if (temp[i].IsMain)
//                    {
//                        bcount++;
//                    }
//                }
//                if (bcount == 0)
//                {
//                    Globals.ShowMessage(eHCMSResources.Z0509_G1_PhaiChonBenhChinh, eHCMSResources.G0442_G1_TBao);
//                    return false;
//                }
//                else if (bcount == 1)
//                {
//                    return true;
//                }
//                else
//                {
//                    Globals.ShowMessage(eHCMSResources.Z0510_G1_I, eHCMSResources.G0442_G1_TBao);
//                    return false;
//                }
//            }
//            else
//            {
//                return true;
//            }
//        }

//        private bool CheckedICD9IsMain()
//        {
//            ObservableCollection<DiagnosisICD9Items> temp = refICD9List.Where(x => x.RefICD9 != null).ToObservableCollection();
//            if (temp != null && temp.Count > 0)
//            {
//                int bcount = temp.Where(x => x.IsMain).Count();

//                if (bcount == 0)
//                {
//                    Globals.ShowMessage(eHCMSResources.Z1907_G1_ChonCachDTriChinh, eHCMSResources.G0442_G1_TBao);
//                    return false;
//                }
//                else if (bcount == 1)
//                {
//                    return true;
//                }
//                else
//                {
//                    Globals.ShowMessage(eHCMSResources.Z1908_G1_NhieuHon1CachDTriChinh, eHCMSResources.G0442_G1_TBao);
//                    return false;
//                }
//            }
//            else
//            {
//                return true;
//            }
//        }

//        AxDataGridNyICD10 grdConsultation { get; set; }
//        public void grdConsultation_Loaded(object sender, RoutedEventArgs e)
//        {
//            grdConsultation = sender as AxDataGridNyICD10;
//        }
//        public void lnkDelete_Click(object sender, RoutedEventArgs e)
//        {
//            //var p = grdConsultation.SelectedItem as DiagnosisIcd10Items;
//            //if (p == null)
//            //{
//            //    MessageBox.Show(eHCMSResources.Z0405_G1_KgTheXoaDongRong);
//            //    return;
//            //}
//            if (refIDC10Item == null
//                || refIDC10Item.DiseasesReference == null)
//            {
//                MessageBox.Show(eHCMSResources.Z0405_G1_KgTheXoaDongRong);
//                return;
//            }

//            int nSelIndex = grdConsultation.SelectedIndex;
//            if (nSelIndex >= refIDC10List.Count - 1)
//            {
//                MessageBox.Show(eHCMSResources.Z0405_G1_KgTheXoaDongRong);
//                return;
//            }

//            var item = refIDC10List[nSelIndex];

//            if (item != null && item.ICD10Code != null && item.ICD10Code != "")
//            {
//                if (MessageBox.Show(eHCMSResources.Z0419_G1_CoMuonXoaMaICD10, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
//                {
//                    if (item.DiseasesReference != null
//                        && item.DiseasesReference.DiseaseNameVN != "")
//                    {
//                        DiagTrmtItem.DiagnosisFinal = DiagTrmtItem.DiagnosisFinal.Replace(item.DiseasesReference.DiseaseNameVN, "");
//                    }
//                    //refIDC10List.RemoveAt(nSelIndex);
//                    refIDC10List.Remove(refIDC10List[nSelIndex]);
//                }
//            }
//        }

//        #region link member

//        public void hpkCreatePrescription()
//        {
//            Globals.ConsultationIsChildWindow = true;
//            Globals.PrescriptionIsChildWindow = false;

//            var Conslt = Globals.GetViewModel<IConsultationModule>();
//            var PrescriptionVM = Globals.GetViewModel<IePrescriptions>();
//            Conslt.MainContent = PrescriptionVM;
//            (Conslt as Conductor<object>).ActivateItem(PrescriptionVM);
//        }

//        #endregion

//        private bool CheckCreateDiagnosis()
//        {
//            InPatientAdmDisDetails admission = Globals.PatientAllDetails.PtRegistrationInfo.AdmissionInfo;
//            if (admission.DischargeDate != null)
//            {
//                MessageBox.Show(eHCMSResources.Z0406_G1_BNDaXVKgTheCNhatCDoan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                return false;
//            }
//            if (Globals.IsLockRegistration(Globals.PatientAllDetails.PtRegistrationInfo.RegLockFlag, eHCMSResources.Z0407_G1_CNhatCDoan))
//            {
//                return false;
//            }
//            return true;
//        }


//        private bool CheckEditDiagnosis()
//        {
//            if (!CheckCreateDiagnosis())
//            {
//                return false;
//            }
//            else
//            {
//                if (DiagTrmtItem == null || Globals.curLstPatientServiceRecord == null || Globals.curLstPatientServiceRecord.Count <= 0)
//                {
//                    MessageBox.Show(eHCMSResources.A0731_G1_Msg_InfoKhTimThayCDHaySerRec, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                    return false;
//                }
//                if (DiagTrmtItem.DTItemID > 0 && DiagTrmtItem.ServiceRecID != Globals.curLstPatientServiceRecord[0].ServiceRecID)
//                {
//                    MessageBox.Show(eHCMSResources.A0671_G1_Msg_InfoKhDcCNhatCDCuaDKCu, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                    return false;
//                }
//            }
//            return true;
//        }

//        #region Các Button
//        public void btEdit()
//        {
//            //FormEditorIsEnabled = true;
//            //IsEnableButton = false;
//            //btSaveCreateNewIsEnabled = false;
//            //btUpdateIsEnabled = true;
//            //btCancelIsEnabled = true;

//            if (DiagTrmtItem == null || DiagTrmtItem.ServiceRecID.GetValueOrDefault() <= 0 || DiagTrmtItem.DTItemID <= 0)
//            {
//                MessageBox.Show(eHCMSResources.A0629_G1_Msg_InfoKhCoCDDeSua, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                return;
//            }
//            if (!IsOutPt && !CheckEditDiagnosis())
//            {
//                return;
//            }
//            StateEditWaiting();
//            DiagTrmtItemCopy = ObjectCopier.DeepCopy(DiagTrmtItem);
//            CopyListICD10();
//            if (!IsOutPt)
//                CopyListICD9();
//        }

//        private bool CheckDiagnosisType()
//        {
//            if (DiagTrmtItem == null)
//            {
//                return false;
//            }
//            if (IsDiagnosisOutHospital)
//            {
//                if (DiagTrmtItem.V_DiagnosisType != (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_IN && DiagTrmtItem.V_DiagnosisType != (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS)
//                {
//                    MessageBox.Show(eHCMSResources.A0176_G1_Msg_InfoChiDcChonLoaiCDoanNhapXV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                    return false;
//                }
//            }
//            /*▼====: #003*/
//            else if (IsDailyDiagnosis)
//            {
//                if (DiagTrmtItem.V_DiagnosisType != (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_DAILY)
//                {
//                    MessageBox.Show(eHCMSResources.Z2158_G1_Msg_InfoChiDcChonLoaiCDoanHangNgay, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                    return false;
//                }
//            }
//            /*▲====: #003*/
//            else
//            {
//                if (DiagTrmtItem.V_DiagnosisType != (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTDEPT && DiagTrmtItem.V_DiagnosisType != (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_INDEPT)
//                {
//                    MessageBox.Show(eHCMSResources.A0177_G1_Msg_InfoChiDcChonLoaiCDoanXuatKhoa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                    return false;
//                }
//            }
//            return true;
//        }

//        // Hpt
//        public InPatientDeptDetail GetCurDeptDetails(ObservableCollection<InPatientDeptDetail> CurDeptDetails)
//        {
//            if (!CurDeptDetails.Any(x => x.DeptLocation.RefDepartment.DeptID == DepartmentContent.SelectedItem.DeptID))
//            {
//                return null;
//            }
//            if (CurDeptDetails.Any(x => x.DeptLocation.RefDepartment.DeptID == DepartmentContent.SelectedItem.DeptID && x.DocTypeRequired == (long)AllLookupValues.V_DocTypeRequired.CD_XUAT_KHOA))
//            {
//                return CurDeptDetails.Where(x => x.DeptLocation.RefDepartment.DeptID == DepartmentContent.SelectedItem.DeptID && x.DocTypeRequired == (long)AllLookupValues.V_DocTypeRequired.CD_XUAT_KHOA).OrderBy(y => y.InPatientDeptDetailID).FirstOrDefault();
//            }
//            if (CurDeptDetails.Any(x => x.DeptLocation.RefDepartment.DeptID == DepartmentContent.SelectedItem.DeptID && x.InPtDeptGuid != null))
//            {
//                return CurDeptDetails.Where(x => x.DeptLocation.RefDepartment.DeptID == DepartmentContent.SelectedItem.DeptID && x.InPtDeptGuid != null).OrderBy(y => y.InPatientDeptDetailID).FirstOrDefault();
//            }
//            return CurDeptDetails.Where(x => x.DeptLocation.RefDepartment.DeptID == DepartmentContent.SelectedItem.DeptID).OrderBy(y => y.InPatientDeptDetailID).FirstOrDefault();

//        }

//        public bool CheckToGetGuid()
//        {
//            // Hpt 04/11/2015: Vì người tạo chẩn đoán (Bác sĩ) không quan tâm chẩn đoán đó là xuất khoa để chuyển đến khoa nào nên việc chọn Guid của đợt nhập khoa nào để lưu vào chẩn đoán được làm tự động theo thứ tự như sau:
//            // 1. Ưu tiên các dòng nhập có đòi hỏi chẩn đoán xuất khoa làm trước(tra cứu từ bảng DeptTransferDocReq anh Tuấn đã tạo - đang để dữ liệu test)
//            // 2. Nếu có nhiều dòng nhập cùng dòi hỏi chẩn đoán xuất khoa, chọn theo thứ tự thời gian phát sinh đòi hỏi đó.
//            // 3. Các dòng không có đòi hỏi chẩn đoán vẫn được phép tạo, nhưng phải sau khi tất cả các đòi hỏi chẩn đoán xuất khoa của khoa đó đã được làm đủ thì mới tới những dòng không đòi hỏi (cũng của khoa đó)
//            // 4. Mỗi đợt nhập vào khoa chỉ được tạo một chẩn đoán xuất khoa
//            ObservableCollection<InPatientDeptDetail> DeptList = Globals.PatientAllDetails.PtRegistrationInfo.AdmissionInfo.InPatientDeptDetails.DeepCopy();
//            PatientServiceRecord CurServiceRec = Globals.curLstPatientServiceRecord.Where(x => x.PtRegistrationID == Globals.PatientAllDetails.PtRegistrationInfo.PtRegistrationID).FirstOrDefault();
//            InPatientDeptDetail CurInPtDeptDetail = new InPatientDeptDetail();

//            CurInPtDeptDetail = GetCurDeptDetails(DeptList);
//            if (CurInPtDeptDetail == null)
//            {
//                MessageBox.Show(eHCMSResources.A0214_G1_Msg_InfoBNChuaNhapVaoKhoa + DepartmentContent.SelectedItem.DeptName + string.Format(". {0}", eHCMSResources.A0215_G1_Msg_InfoKhTheTaoCD), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                return false;
//            }
//            while (DeptList.Count > 0 && CurServiceRec.DiagnosisTreatments.Any(x => x.InPtDeptGuid == CurInPtDeptDetail.InPtDeptGuid && x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTDEPT)) // trừ dòng cuối, dòng cuối cho làm bao nhiêu cũng được
//            {
//                if (CurInPtDeptDetail.InPtDeptGuid == null)
//                {
//                    DiagTrmtItem.InPtDeptGuid = null;
//                    return true;
//                }
//                DeptList.Remove(CurInPtDeptDetail);
//                CurInPtDeptDetail = GetCurDeptDetails(DeptList);
//                if (CurInPtDeptDetail == null)
//                {
//                    MessageBox.Show(DepartmentContent.SelectedItem.DeptName + string.Format(" {0}", eHCMSResources.K3310_G1_I), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                    return false;
//                }
//            }
//            DiagTrmtItem.InPtDeptGuid = CurInPtDeptDetail.InPtDeptGuid;
//            return true;
//        }

//        private bool CheckDepartment()
//        {
//            if (DiagTrmtItem == null || DiagTrmtItem.Department == null || DiagTrmtItem.Department.DeptID <= 0)
//            {
//                MessageBox.Show(eHCMSResources.Z0493_G1_HayChonKhoa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                return false;
//            }

//            InPatientAdmDisDetails admission = Globals.PatientAllDetails.PtRegistrationInfo.AdmissionInfo;

//            if (DiagTrmtItem.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_IN)
//            {
//                if (DiagTrmtItem.Department.DeptID != admission.Department.DeptID)
//                {
//                    MessageBox.Show(string.Format("{0} ", eHCMSResources.A0284_G1_Msg_InfoChonKhoaNpVien) + admission.Department.DeptName + string.Format(" {0}", eHCMSResources.A0285_G1_Msg_InfoViBNNpVienVaoKhoaNay), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                    return false;
//                }
//            }
//            else if (DiagTrmtItem.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS)
//            {
//                int CountInDept = 0;

//                //Đếm số lần nhập khoa.
//                if (admission.InPatientDeptDetails != null && admission.InPatientDeptDetails.Count > 0)
//                {
//                    CountInDept = admission.InPatientDeptDetails.Where(x => x.DeptLocation.DeptID == DiagTrmtItem.Department.DeptID).Count();
//                }
//                if (CountInDept <= 0)
//                {
//                    MessageBox.Show(eHCMSResources.A0216_G1_Msg_InfoBNChuaNhapVaoKhoaBanChon + " (" + DiagTrmtItem.Department.DeptName + ") ! ", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                    return false;
//                }
//            }
//            /*▼====: #003*/
//            else if (DiagTrmtItem.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_DAILY)
//            {
//                if (admission == null || admission.InPatientDeptDetails == null || admission.InPatientDeptDetails.Count() <= 0)
//                {
//                    return false;
//                }
//                if (!admission.InPatientDeptDetails.Any(x => x.DeptLocation.DeptID == DiagTrmtItem.Department.DeptID))
//                {
//                    MessageBox.Show(eHCMSResources.A0216_G1_Msg_InfoBNChuaNhapVaoKhoaBanChon + " (" + DiagTrmtItem.Department.DeptName + ") ! ", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                    return false;
//                }
//            }
//            /*▲====: #003*/
//            else if (DiagTrmtItem.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTDEPT)
//            {
//                if (admission == null || admission.InPatientDeptDetails == null || admission.InPatientDeptDetails.Count() <= 0)
//                {
//                    return false;
//                }
//                //HPT 26/09/2016: Khi đếm những dòng nhập khoa đã có chẩn đoán xuất khoa , phải trừ dòng nhập khoa nào gắn với chẩn đoán xuất khoa đang hiệu chỉnh. Nếu không sẽ không hiệu chỉnh được
//                /*Ví dụ:
//                 * Nhập khoa A hai lần, tạo hai chẩn đoán có GUID1 và GUID2 ứng với hai lần nhập
//                 * Cập nhật chẩn đoán GUID1, đếm số dòng nhập khoa đã có chẩn đoán, nếu không trừ dòng có GUID1 ra thì sẽ thấy có hai dòng nhập khoa đều đã có chẩn đoán --> không cho lưu cập nhật
//                 */
//                if (!admission.InPatientDeptDetails.Any(x => x.DeptLocation.DeptID == DiagTrmtItem.Department.DeptID))
//                {
//                    MessageBox.Show(eHCMSResources.A0216_G1_Msg_InfoBNChuaNhapVaoKhoaBanChon + " (" + DiagTrmtItem.Department.DeptName + ") ! ", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                    return false;
//                }
//                if (DiagTrmtItem.DTItemID <= 0 && !admission.InPatientDeptDetails.Any(x => x.DeptLocation.DeptID == DiagTrmtItem.Department.DeptID && x.CompletedRequiredFromDate == null))
//                {
//                    MessageBox.Show(eHCMSResources.A0222_G1_Msg_InfoBNCoDuCDXK + " " + DiagTrmtItem.Department.DeptName + string.Format("\n {0}", eHCMSResources.Z0408_G1_KgTheTaoThemCDoanXK), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                    return false;
//                }
//            }

//            return true;
//        }

//        public void btUpdate()
//        {
//            Coroutine.BeginExecute(AllCheck());
//        }

//        public void btCancel()
//        {
//            DiagTrmtItem = ObjectCopier.DeepCopy(DiagTrmtItemCopy);
//            refIDC10List = refIDC10ListCopy;
//            refICD9List = refICD9ListCopy;
            
//            switch (ConsultState)
//            {
//                case ConsultationState.NewConsultationState:
//                    StateNew(); break;
//                case ConsultationState.EditConsultationState:
//                    StateEdit(); break;
//                case ConsultationState.NewAndEditConsultationState:
//                    StateNewEdit(); break;
//            }
//        }

//        private bool CheckEmptyFields()
//        {
//            string strWarningMsg = "";

//            if (DiagTrmtItem.DiagnosisFinal == null || DiagTrmtItem.DiagnosisFinal.Trim() == "")
//            {
//                strWarningMsg += string.Format("{0} - ", eHCMSResources.K1775_G1_CDoanXDinh2);
//            }
//            if (DiagTrmtItem.Diagnosis == null || DiagTrmtItem.Diagnosis.Trim() == "")
//            {
//                strWarningMsg += string.Format("{0} - ", eHCMSResources.G1785_G1_TrieuChungDHieuLS);
//            }
//            // Hpt 20/11/2015: Giờ có yêu cầu bỏ ra nên comment lại đây, khi nào cần thì mở ra
//            //if (DiagTrmtItem.OrientedTreatment == null || DiagTrmtItem.OrientedTreatment.Trim() == "")
//            //{
//            //    strWarningMsg += string.Format("{0} - ", eHCMSResources.Z3309_G1_DienBienBenh);
//            //}
//            if (DiagTrmtItem.Treatment == null || DiagTrmtItem.Treatment.Trim() == "")
//            {
//                strWarningMsg += string.Format("{0} - ", eHCMSResources.Z0021_G1_CachDTri);
//            }
//            if (strWarningMsg != "")
//            {
//                MessageBox.Show(string.Format("{0}: ", eHCMSResources.A0201_G1_Msg_InfoYCNhapSth) + strWarningMsg);
//                return false;
//            }
//            return true;
//        }
//        MessageWarningShowDialogTask warningtask = null;
//        string content = eHCMSResources.Z0420_G1_CDoanCoICDLaZ10;
//        // Hpt 22/09/2015: Thêm hàm coroutine SaveNewDiagnosis ở đây, cắt thân hàm btSaveCreateNew bỏ vào, để có thể sử dụng được đối tượng MessageWarningShowDialogTask hiển thị cảnh báo nếu có ICD10 là Z10
//        // Thân hàm btSaveCreateNew thay bằng code gọi đến coroutine SaveNewDiagnosis

//        private bool _IsRequireConfirmZ10 = true;
//        public bool IsRequireConfirmZ10
//        {
//            get
//            {
//                return _IsRequireConfirmZ10;
//            }
//            set
//            {
//                _IsRequireConfirmZ10 = value;
//                NotifyOfPropertyChange(() => IsRequireConfirmZ10);
//            }
//        }

//        private IEnumerator<IResult> SaveNewDiagnosis()
//        {
//            if (refIDC10List != null && refIDC10List.Any(x => x.IsInvalid))
//            {
//                MessageBox.Show(string.Format(eHCMSResources.Z2205_G1_ICD10KhongHopLe, string.Join(",", refIDC10List.Where(x => x.IsInvalid).Select(x => x.ICD10Code).ToList())), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
//                yield break;
//            }

//            if (!IsOutPt)
//            {
//                if (!CheckCreateDiagnosis())
//                {
//                    yield break;
//                }

//                if (!CheckDiagnosisType())
//                {
//                    yield break;
//                }

//                if (!CheckDepartment())
//                {
//                    yield break;
//                }
//            }

//            if (!CheckEmptyFields())
//            {
//                yield break;
//            }
//            //if (!IsNotExistsDiagnosisTreatmentByPtRegDetailID)
//            //{
//            //    MessageBox.Show(eHCMSResources.A0449_G1_Msg_InfoDaCoCDChoBenh);
//            //    return;
//            //}

//            long lBehaving = 0;
//            try
//            {
//                lBehaving = DiagTrmtItem.PatientServiceRecord.V_Behaving.GetValueOrDefault();
//            }
//            catch
//            {
//                MessageBox.Show(eHCMSResources.A0367_G1_Msg_InfoChonTieuDe);
//                yield break;
//            }

//            long lPMRTemplateID = 0;
//            try
//            {
//                lPMRTemplateID = DiagTrmtItem.MDRptTemplateID;
//            }
//            catch
//            {
//                MessageBox.Show(eHCMSResources.A0337_G1_Msg_InfoChonMauBAn);
//                yield break;
//            }

//            DiagTrmtItem.PatientServiceRecord.Staff = Globals.LoggedUserAccount.Staff;
//            DiagTrmtItem.PatientServiceRecord.StaffID = Globals.LoggedUserAccount.Staff.StaffID;


//            if (CheckedIsMain() && NeedICD10() && (IsOutPt || CheckedICD9IsMain()))
//            {
//                //Khám DV cụ thể nào
//                if (Globals.PatientAllDetails.PtRegistrationDetailInfo != null && Globals.PatientAllDetails.PtRegistrationDetailInfo.PtRegDetailID > 0)
//                {
//                    DiagTrmtItem.PtRegDetailID = Globals.PatientAllDetails.PtRegistrationDetailInfo.PtRegDetailID;
//                }
//                else
//                {
//                    DiagTrmtItem.PtRegDetailID = 0;
//                }
//                //Khám DV cụ thể nào

//                DiagTrmtItem.PatientServiceRecord.PatientMedicalRecord.PatientID = Globals.PatientAllDetails.PatientInfo.PatientID;
//                DiagTrmtItem.PatientServiceRecord.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
//                DiagTrmtItem.PatientServiceRecord.PtRegistrationID = Globals.PatientAllDetails.PtRegistrationInfo.PtRegistrationID;
//                DiagTrmtItem.DeptLocationID = Globals.DeptLocation.DeptLocationID;

//                DiagTrmtItem.PatientServiceRecord.V_RegistrationType = Globals.PatientAllDetails.PtRegistrationInfo.V_RegistrationType;
//                if (IsOutPt)
//                    DiagTrmtItem.PatientServiceRecord.V_DiagnosisType = (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_NORMAL;
//                //KMx: Loại đăng ký phải dựa trên ĐK của BN, không dựa vào tiêu chí tìm kiếm đăng ký.
//                //Nếu không sẽ bị sai khi tìm kiếm và chọn 1 BN ngoại trú, sau đó tick vào NỘI TRÚ (09/10/2014 10:12).
//                //if (Globals.PatientFindBy_ForConsultation.Value == AllLookupValues.PatientFindBy.NGOAITRU)
//                //{
//                //    DiagTrmtItem.PatientServiceRecord.V_RegistrationType = AllLookupValues.RegistrationType.NGOAI_TRU;
//                //}

//                //if (Globals.PatientFindBy_ForConsultation.Value == AllLookupValues.PatientFindBy.NOITRU)
//                //{
//                //    DiagTrmtItem.PatientServiceRecord.V_RegistrationType = AllLookupValues.RegistrationType.NOI_TRU;
//                //}

//                // Hpt 22/09/2015: bắt buộc phải nhập Diễn tiến bệnh

//                DiagTrmtItem.ICD10List = String.Join(",", from item in refIDC10List
//                                                          where (!string.IsNullOrEmpty(item.ICD10Code) && item.DiseasesReference != null)
//                                                          select item.ICD10Code);
//                PatientRegistration CurRegistration = Globals.PatientAllDetails.PtRegistrationInfo;
//                // Hpt 22/09/2015: Nếu bệnh nhân có quyền lợi về BHYT thì mới kiểm tra xem trong danh sách có ICD10 nào là Z10 hay không vì BHYT sẽ không chi trả cho những trường hợp này
//                if ((IsOutPt|| IsRequireConfirmZ10) &&CurRegistration.PtInsuranceBenefit != null && CurRegistration.PtInsuranceBenefit > 0 && refIDC10List != null
//                    && refIDC10List.Any(x => x.ICD10Code != null && x.DiseasesReference != null && x.DiseasesReference.ICD10Code.Contains("Z10")))
//                {
//                    warningtask = new MessageWarningShowDialogTask(content, eHCMSResources.Z0339_G1_TiepTucLuuCDoan);
//                    yield return warningtask;
//                    if(IsOutPt)
//                    {
//                        if (warningtask.IsAccept)
//                        {
//                            AddNewDiagTrmt();
//                        }
//                        else
//                        {
//                            yield break;
//                        }
//                    }
//                    else
//                    {
//                        if (!warningtask.IsAccept)
//                        {
//                            IsRequireConfirmZ10 = true;
//                            yield break;
//                        }
//                        IsRequireConfirmZ10 = false;
//                    }
//                }
//                // Nếu là đăng ký ngoại trú không có BHYT thì cho lưu luôn không cần kiểm tra gì
//                else if (IsOutPt)
//                {
//                    AddNewDiagTrmt();
//                }
//                if (!IsOutPt)
//                {
//                    if (DiagTrmtItem.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTDEPT)
//                    {
//                        string confirmcontent = string.Format(eHCMSResources.K1006_G1_BanDangTaoCDXKhoaCho, DiagTrmtItem.Department.DeptName) + string.Format("\n{0}!", eHCMSResources.Z0538_G1_KgThayDoiKhoaSauKhiLuuCDoanXK);
//                        warningtask = new MessageWarningShowDialogTask(confirmcontent, eHCMSResources.Z0339_G1_TiepTucLuuCDoan);
//                        yield return warningtask;
//                        if (!warningtask.IsAccept)
//                        {
//                            yield break;
//                        }
//                    }
//                    AddNewDiagTrmt();
//                }
//            }
//        }
//        public void btSaveCreateNew()
//        {
//            Coroutine.BeginExecute(SaveNewDiagnosis());
//        }
//        public void btCreateNew()
//        {
//            if (IsOutPt && Globals.PatientAllDetails.PtRegistrationInfo != null && Globals.IsLockRegistration(Globals.PatientAllDetails.PtRegistrationInfo.RegLockFlag, eHCMSResources.Z0407_G1_CNhatCDoan))
//            {
//                return;
//            }
//            else if (!IsOutPt && !CheckCreateDiagnosis())
//            {
//                return;
//            }
//            //FormEditorIsEnabled = true;

//            //ButtonForNotDiag(true);
//            StateNewWaiting();

//            DiagTrmtItemCopy = ObjectCopier.DeepCopy(DiagTrmtItem);

//            //KMx: Không cần gọi về server, chỉ cần sử dụng hàm ResetDefaultForBehaving() trong ruột GetBlankDiagnosisTreatmentByPtID() (01/11/2014 10:16).
//            //GetBlankDiagnosisTreatmentByPtID(Globals.PatientAllDetails.PatientInfo.PatientID);
//            DiagTrmtItem = new DiagnosisTreatment();
//            ResetDefaultForBehaving();

//            CopyListICD10ForNew();
//            //CopyListICD10();
//            if (!IsOutPt)
//                CopyListICD9ForNew();
//        }

//        private void SetDepartment()
//        {
//            if (DiagTrmtItem == null || DepartmentContent == null || DepartmentContent.Departments == null)
//            {
//                return;
//            }

//            if (_DiagTrmtItem.Department != null && _DiagTrmtItem.Department.DeptID > 0)
//            {
//                DepartmentContent.SelectedItem = _DiagTrmtItem.Department;
//            }
//            else
//            {
//                if (Globals.ObjRefDepartment != null && Globals.ObjRefDepartment.DeptID > 0 && DepartmentContent.Departments.Any(x => x.DeptID == Globals.ObjRefDepartment.DeptID))
//                {
//                    DepartmentContent.SelectedItem = DepartmentContent.Departments.Where(x => x.DeptID == Globals.ObjRefDepartment.DeptID).FirstOrDefault();
//                }
//                else
//                {
//                    //DepartmentContent.SelectedItem = DepartmentContent.Departments != null ? DepartmentContent.Departments.FirstOrDefault() : null;
//                    DepartmentContent.SelectedItem = DepartmentContent.Departments.FirstOrDefault();
//                }

//            }
//        }

//        public void btSaveNewByOld()
//        {
//            if (IsOutPt)
//            {
//                StateNewWaiting();

//                DiagTrmtItem.DTItemID = 0;
//                DiagTrmtItemCopy = ObjectCopier.DeepCopy(DiagTrmtItem);

//                CopyListICD10();
//            }
//            else
//            {
//                if (!CheckCreateDiagnosis())
//                {
//                    return;
//                }

//                StateNewWaiting();

//                DiagTrmtItemCopy = ObjectCopier.DeepCopy(DiagTrmtItem);

//                //KMx: Khi tạo mới dựa trên chẩn đoán cũ, thì phải set DTItemID của chẩn đoán mới = 0. Để hàm CheckDepartment() biết đây là tạo mới, không phải cập nhật (10/06/2015 17:47).
//                //KMx: Khi tạo mới dựa trên chẩn đoán cũ, bắt người dùng chọn Khoa để không bị lộn (03/08/2015 16:45).
//                if (DiagTrmtItem != null)
//                {
//                    DiagTrmtItem.DTItemID = 0;
//                    DiagTrmtItem.DiagnosisDate = Globals.GetCurServerDateTime();
//                    DiagTrmtItem.Department = new RefDepartment();
//                    SetDepartment();
//                }

//                SetDefaultDiagnosisType();

//                CopyListICD10();
//                if (!IsOutPt)
//                    CopyListICD9();
//            }
//        }

//        private void AddNewDiagTrmt()
//        {
//            this.ShowBusyIndicator();

//            IsWaitingSaveAddNew = true;

//            long ID = Compare2Object();

//            if (IsOutPt)
//            {
//                var t = new Thread(() =>
//                {

//                    using (var serviceFactory = new ePMRsServiceClient())
//                    {
//                        var contract = serviceFactory.ServiceInstance;

//                        contract.BeginAddDiagnosisTreatment(DiagTrmtItem, ID, refIDC10List, Globals.DispatchCallback((asyncResult) =>
//                        {

//                            try
//                            {
//                                long ServiceID = 0;

//                                if (contract.EndAddDiagnosisTreatment(out ServiceID, asyncResult))
//                                {
//                                    StateEdit();

//                                    if (refIDC10List != null)
//                                    {
//                                        refIDC10List = refIDC10List.Where(x => x.DiseasesReference != null).ToObservableCollection();
//                                    }


//                                //phat su kien reload lai danh sach 
//                                //Globals.EventAggregator.Publish(new ReloadDataConsultationEvent { });

//                                //KMx: Sau khi lưu chẩn đoán, reload Service Record (22/05/2014 09:48).
//                                IConsultationModule consultVM = Globals.GetViewModel<IConsultationModule>();

//                                    consultVM.PatientServiceRecordsGetForKhamBenh_Ext();
//                                    var homeVm = Globals.GetViewModel<IHome>();
//                                    if (homeVm.OutstandingTaskContent != null && homeVm.OutstandingTaskContent is IConsultationOutstandingTask)
//                                    {
//                                        ((IConsultationOutstandingTask)homeVm.OutstandingTaskContent).SearchRegistrationListForOST();
//                                    }

//                                //KMx: Sau khi lưu chẩn đoán, xóa danh sách chẩn đoán. Vì danh sách không tự động load lại.
//                                //Nếu không xóa thì chẩn đoán vừa lưu sẽ khác với chẩn đoán trong danh sách (22/05/2014 09:48).
//                                Globals.EventAggregator.Publish(new ClearAllDiagnosisListAfterAddNewEvent());


//                                //Nếu đang là Popup thì phát event lấy cđ này gán vào khám bệnh
//                                if (Globals.ConsultationIsChildWindow)
//                                    {
//                                        Globals.EventAggregator.Publish(new DiagnosisTreatmentSelectedEvent<DiagnosisTreatment> { DiagnosisTreatment = DiagTrmtItem.DeepCopy() });
//                                    }


//                                    MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                                }
//                                else
//                                {
//                                    if (DiagTrmtItem.PatientServiceRecord.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_IN)
//                                    {
//                                        MessageBox.Show(eHCMSResources.Z0409_G1_DaCoCDoanNV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                                    }
//                                    else if (DiagTrmtItem.PatientServiceRecord.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS)
//                                    {
//                                        MessageBox.Show(eHCMSResources.Z0410_G1_DaCoCDoanXV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                                    }
//                                    else
//                                    {
//                                        MessageBox.Show(eHCMSResources.Z0411_G1_LuuCDoanKgThCong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                                    }
//                                }

//                            }
//                            catch (Exception ex)
//                            {
//                                MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                            }
//                            finally
//                            {
//                            //Globals.IsBusy = false;
//                            //IsWaitingSaveAddNew = false;
//                            this.HideBusyIndicator();
//                            }

//                        }), null);

//                    }

//                });

//                t.Start();
//            }
//            else
//            {
//                long DiagnosisICD9ListID = Compare2ICD9List();

//                var t = new Thread(() =>
//                {

//                    using (var serviceFactory = new ePMRsServiceClient())
//                    {
//                        var contract = serviceFactory.ServiceInstance;

//                        contract.BeginAddDiagnosisTreatment_InPt(DiagTrmtItem, ID, refIDC10List, DiagnosisICD9ListID, refICD9List, Globals.DispatchCallback((asyncResult) =>
//                        {

//                            try
//                            {
//                                List<InPatientDeptDetail> ReloadInPatientDeptDetails = new List<InPatientDeptDetail>();

//                                if (contract.EndAddDiagnosisTreatment_InPt(out ReloadInPatientDeptDetails, asyncResult))
//                                {

//                                    StateNewEdit();

//                                    if (refIDC10List != null)
//                                    {
//                                        refIDC10List = refIDC10List.Where(x => x.DiseasesReference != null).ToObservableCollection();
//                                    }

//                                    if (refICD9List != null)
//                                    {
//                                        refICD9List = refICD9List.Where(x => x.RefICD9 != null).ToObservableCollection();
//                                    }

//                                    //phat su kien reload lai danh sach 
//                                    //Globals.EventAggregator.Publish(new ReloadDataConsultationEvent { });

//                                    //KMx: Sau khi lưu chẩn đoán, reload Service Record (22/05/2014 09:48).
//                                    IConsultationModule consultVM = Globals.GetViewModel<IConsultationModule>();

//                                    consultVM.PatientServiceRecordsGetForKhamBenh_Ext();

//                                    //KMx: Sau khi lưu chẩn đoán, xóa danh sách chẩn đoán. Vì danh sách không tự động load lại.
//                                    //Nếu không xóa thì chẩn đoán vừa lưu sẽ khác với chẩn đoán trong danh sách (22/05/2014 09:48).
//                                    Globals.EventAggregator.Publish(new ClearAllDiagnosisListAfterAddNewEvent_InPt());

//                                    Globals.PatientAllDetails.PtRegistrationInfo.AdmissionInfo.InPatientDeptDetails = new ObservableCollection<InPatientDeptDetail>(ReloadInPatientDeptDetails);

//                                    MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                                }
//                                else
//                                {
//                                    //KMx: Dời V_DiagnosisType từ PatientServiceRecord sang DiagnosisTreatment (09/06/2015 17:42).
//                                    //if (DiagTrmtItem.PatientServiceRecord.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_IN)
//                                    if (DiagTrmtItem.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_IN)
//                                    {
//                                        MessageBox.Show(eHCMSResources.Z0409_G1_DaCoCDoanNV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                                    }
//                                    //else if (DiagTrmtItem.PatientServiceRecord.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS)
//                                    else if (DiagTrmtItem.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS)
//                                    {
//                                        MessageBox.Show(eHCMSResources.Z0410_G1_DaCoCDoanXV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                                    }
//                                    else
//                                    {
//                                        MessageBox.Show(eHCMSResources.A0802_G1_Msg_InfoLuuCDFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                                    }
//                                }

//                            }
//                            catch (Exception ex)
//                            {
//                                MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                            }
//                            finally
//                            {
//                                //Globals.IsBusy = false;
//                                //IsWaitingSaveAddNew = false;
//                                this.HideBusyIndicator();
//                            }

//                        }), null);

//                    }

//                });

//                t.Start();
//            }
//        }

//        //private IEnumerator<IResult> LoadRefBehaving_MedRecTemplate()
//        //{
//        //    var resultBehavingTask = new LoadLookupBehavingTask();
//        //    yield return resultBehavingTask;
//        //    RefBehaving = resultBehavingTask.RefBehaving;

//        //    var resultMedRecTemplateTask = new LoadMedRecTemplateTask();
//        //    yield return resultMedRecTemplateTask;
//        //    RefMedRecTemplate = resultMedRecTemplateTask.RefMedRecTemplate;


//        //    var resultDiagnosisTask = new LoadLookupListTask(LookupValues.V_DiagnosisType, false, false);
//        //    yield return resultDiagnosisTask;
//        //    RefDiagnosis = resultDiagnosisTask.LookupList;

//        //    yield break;
//        //}

//        private void LoadRefBehaving_MedRecTemplate()
//        {
//            // 1. Get Behaving.
//            ObservableCollection<Lookup> BehavingLookupList = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.BEHAVING).ToObservableCollection();

//            if (BehavingLookupList == null || BehavingLookupList.Count <= 0)
//            {
//                MessageBox.Show(eHCMSResources.A0740_G1_Msg_InfoKhTimThayLoaiKB, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                return;
//            }

//            RefBehaving = BehavingLookupList;

//            // 2. Get Medical Record Templates.
//            RefMedRecTemplate = Globals.AllMedRecTemplates;

//            // 3.Get DiagnosisType.
//            //KMx: Không lấy loại chẩn đoán Thường. Vì BN nội trú chỉ có chẩn đoán Nhập viện hoặc Xuất viện (09/10/2014 18:05).
//            ObservableCollection<Lookup> DiagnosisLookupList = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_DiagnosisType && x.LookupID != (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_NORMAL).ToObservableCollection();

//            if (DiagnosisLookupList == null || DiagnosisLookupList.Count <= 0)
//            {
//                MessageBox.Show(eHCMSResources.A0739_G1_Msg_InfoKhTimThayLoaiCDoan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                return;
//            }

//            RefDiagnosis = DiagnosisLookupList;

//        }

//        private void LoadInitData()
//        {
//            //Coroutine.BeginExecute(LoadRefBehaving_MedRecTemplate());
//            LoadRefBehaving_MedRecTemplate();
//        }
//        #endregion

//        #region button control

//        public enum ConsultationState
//        {
//            //Tao moi chan doan
//            NewConsultationState = 1,
//            //Hieu chinh chan doan
//            EditConsultationState = 2,
//            //tao moi va chinh sua chan doan cua noi tru
//            NewAndEditConsultationState = 3,

//        }


//        private ConsultationState _ConsultState = ConsultationState.NewConsultationState;
//        public ConsultationState ConsultState
//        {
//            get
//            {
//                return _ConsultState;
//            }
//            set
//            {
//                //if (_ConsultState != value)
//                {
//                    _ConsultState = value;
//                    NotifyOfPropertyChange(() => ConsultState);
//                    switch (ConsultState)
//                    {
//                        case ConsultationState.NewConsultationState:
//                            mNewConsultationState = true;
//                            mEditConsultationState = false;
//                            break;
//                        case ConsultationState.EditConsultationState:
//                            mNewConsultationState = false;
//                            mEditConsultationState = true;
//                            break;
//                        case ConsultationState.NewAndEditConsultationState:
//                            mNewConsultationState = true;
//                            mEditConsultationState = true;
//                            break;
//                    }
//                }
//            }
//        }

//        private bool _mNewConsultationState;
//        public bool mNewConsultationState
//        {
//            get
//            {
//                return _mNewConsultationState;
//            }
//            set
//            {
//                if (_mNewConsultationState != value)
//                {
//                    _mNewConsultationState = value;
//                    NotifyOfPropertyChange(() => mNewConsultationState);
//                }
//            }
//        }

//        public bool mSaveConsultationState
//        {
//            get
//            {
//                return mNewConsultationState && IsShowSummaryContent;
//            }
//        }

//        private bool _mEditConsultationState;
//        public bool mEditConsultationState
//        {
//            get
//            {
//                return _mEditConsultationState;
//            }
//            set
//            {
//                if (_mEditConsultationState != value)
//                {
//                    _mEditConsultationState = value;
//                    NotifyOfPropertyChange(() => mEditConsultationState);
//                    NotifyOfPropertyChange(() => mUpdateConsultationState);
//                }
//            }
//        }

//        public bool mUpdateConsultationState
//        {
//            get
//            {
//                return mEditConsultationState && IsShowSummaryContent;
//            }
//        }

//        public void StateNew()
//        {
//            btCreateNewIsEnabled = true;
//            btCreateNewByOldIsEnabled = true;
//            btSaveCreateNewIsEnabled = false;
//            btCancelIsEnabled = false;
//            FormEditorIsEnabled = false;

//            NotifyOfPropertyChange(() => btCreateNewIsEnabled);
//            NotifyOfPropertyChange(() => btCreateNewByOldIsEnabled);
//            NotifyOfPropertyChange(() => btSaveCreateNewIsEnabled);
//            NotifyOfPropertyChange(() => btCancelIsEnabled);
//            NotifyOfPropertyChange(() => FormEditorIsEnabled);

//        }

//        public void StateNewEdit()
//        {
//            btCreateNewIsEnabled = true;
//            btCreateNewByOldIsEnabled = true;
//            btEditIsEnabled = true;
//            btUpdateIsEnabled = false;
//            btSaveCreateNewIsEnabled = false;
//            btCancelIsEnabled = false;
//            FormEditorIsEnabled = false;

//            NotifyOfPropertyChange(() => btCreateNewIsEnabled);
//            NotifyOfPropertyChange(() => btCreateNewByOldIsEnabled);
//            NotifyOfPropertyChange(() => btSaveCreateNewIsEnabled);
//            NotifyOfPropertyChange(() => btCancelIsEnabled);
//            NotifyOfPropertyChange(() => FormEditorIsEnabled);

//        }

//        public void StateNewWaiting()
//        {
//            btCreateNewIsEnabled = false;
//            btCreateNewByOldIsEnabled = false;
//            btSaveCreateNewIsEnabled = true;
//            btCancelIsEnabled = true;
//            FormEditorIsEnabled = true;

//            btEditIsEnabled = false;

//            NotifyOfPropertyChange(() => btCreateNewIsEnabled);
//            NotifyOfPropertyChange(() => btCreateNewByOldIsEnabled);
//            NotifyOfPropertyChange(() => btSaveCreateNewIsEnabled);
//            NotifyOfPropertyChange(() => btCancelIsEnabled);
//            NotifyOfPropertyChange(() => FormEditorIsEnabled);
//        }

//        public void StateEdit()
//        {
//            btEditIsEnabled = true;
//            btUpdateIsEnabled = false;
//            btCancelIsEnabled = false;
//            FormEditorIsEnabled = false;

//            NotifyOfPropertyChange(() => btEditIsEnabled);
//            NotifyOfPropertyChange(() => btUpdateIsEnabled);
//            NotifyOfPropertyChange(() => btCancelIsEnabled);
//            NotifyOfPropertyChange(() => FormEditorIsEnabled);
//        }

//        public void StateEditWaiting()
//        {
//            btEditIsEnabled = false;
//            btUpdateIsEnabled = true;
//            btCancelIsEnabled = true;
//            FormEditorIsEnabled = true;
//            btCreateNewIsEnabled = false;
//            btCreateNewByOldIsEnabled = false;

//            NotifyOfPropertyChange(() => btEditIsEnabled);
//            NotifyOfPropertyChange(() => btUpdateIsEnabled);
//            NotifyOfPropertyChange(() => btCancelIsEnabled);
//            NotifyOfPropertyChange(() => FormEditorIsEnabled);
//        }

//        #endregion

//        #region Old Button Control

//        private bool _IsEnableButton;
//        public bool IsEnableButton
//        {
//            get { return _IsEnableButton; }
//            set
//            {
//                _IsEnableButton = value;
//                NotifyOfPropertyChange(() => IsEnableButton);
//                NotifyOfPropertyChange(() => btCreateNewIsEnabled);
//                NotifyOfPropertyChange(() => btCreateNewByOldIsEnabled);
//                NotifyOfPropertyChange(() => btEditIsEnabled);
//            }
//        }

//        private void ButtonForNotDiag(bool bCancel)
//        {
//            //ban dau neu benh nhan chua co 1 chan doan bat ky nao het va bCancel =false
//            //co the dung lai cho khi nhan nut tao moi hoac tao moi dua tren chan doan cu va bCancel=true
//            IsEnableButton = false;
//            // btCreateNewIsEnabled = false;
//            // btCreateNewByOldIsEnabled = false;
//            btSaveCreateNewIsEnabled = true;
//            //btEditIsEnabled = false;
//            btUpdateIsEnabled = false;
//            btCancelIsEnabled = bCancel;
//        }

//        private void ButtonForHasDiag()
//        {
//            IsEnableButton = true;
//            //btCreateNewIsEnabled = true && IsNotExistsDiagnosisTreatmentByPtRegDetailID;
//            //btCreateNewByOldIsEnabled = true && IsNotExistsDiagnosisTreatmentByPtRegDetailID && (DiagTrmtItem != null && Globals.PatientAllDetails.RegistrationInfo != null && DiagTrmtItem.PatientServiceRecord != null && (DiagTrmtItem.PatientServiceRecord.PtRegistrationID == Globals.PatientAllDetails.RegistrationInfo.PtRegistrationID || DiagTrmtItem.PatientServiceRecord.PtRegistrationID == PtRegistrationIDLatest.GetValueOrDefault(0)));
//            btSaveCreateNewIsEnabled = false;
//            //btEditIsEnabled = true && (DiagTrmtItem != null && (DiagTrmtItem.DoctorStaffID == Globals.LoggedUserAccount.StaffID.GetValueOrDefault(0) || PermissionManager.IsAdminUser()));
//            btUpdateIsEnabled = false;
//            btCancelIsEnabled = false;
//        }
//        #endregion

//        #region ICD9 control 

//        bool IsICD9Code = true;

//        private DiagnosisICD9Items _refICD9Item;
//        public DiagnosisICD9Items refICD9Item
//        {
//            get
//            {
//                return _refICD9Item;
//            }
//            set
//            {
//                if (_refICD9Item != value)
//                {
//                    _refICD9Item = value;
//                }
//                NotifyOfPropertyChange(() => refICD9Item);
//            }
//        }

//        private ObservableCollection<DiagnosisICD9Items> _refICD9List;
//        public ObservableCollection<DiagnosisICD9Items> refICD9List
//        {
//            get
//            {
//                return _refICD9List;
//            }
//            set
//            {
//                if (_refICD9List != value)
//                {
//                    _refICD9List = value;
//                }
//                NotifyOfPropertyChange(() => refICD9List);
//            }
//        }

//        private void AddICD9BlankRow()
//        {
//            if (refICD9List != null
//                && refICD9List.LastOrDefault() != null
//                && refICD9List.LastOrDefault().RefICD9 == null)
//            {
//                return;
//            }
//            DiagnosisICD9Items ite = new DiagnosisICD9Items();
//            refICD9List.Add(ite);
//        }

//        private RefICD9 RefICD9Copy = null;
//        private string TreatmentOld = "";
//        private string TreatmentNew = "";



//        AxDataGridNyICD10 grdICD9 { get; set; }
//        public void grdICD9_Loaded(object sender, RoutedEventArgs e)
//        {
//            grdICD9 = sender as AxDataGridNyICD10;
//        }

//        public void grdICD9_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
//        {
//            ColumnIndex = e.Column.DisplayIndex;

//            if (refICD9Item != null)
//            {
//                RefICD9Copy = refICD9Item.RefICD9.DeepCopy();
//            }
//            if (e.Column.DisplayIndex == 0)
//            {
//                IsICD9Code = true;
//            }
//            else
//            {
//                IsICD9Code = false;
//            }
//        }


//        public void grdICD9_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
//        {
//            DiagnosisICD9Items item = e.Row.DataContext as DiagnosisICD9Items;

//            if (ColumnIndex == 0 || ColumnIndex == 1)
//            {
//                if (refICD9Item.RefICD9 == null)
//                {
//                    if (RefICD9Copy != null)
//                    {
//                        refICD9Item.RefICD9 = ObjectCopier.DeepCopy(RefICD9Copy);
//                        if (CheckExistsICD9(refICD9Item, false))
//                        {
//                            GetTreatment(refICD9Item.RefICD9);
//                        }
//                    }
//                }
//            }
//            if (refICD9Item != null && refICD9Item.RefICD9 != null)
//            {
//                if (CheckExistsICD9(refICD9Item, false))
//                {
//                    if (e.Row.GetIndex() == (refICD9List.Count - 1) && e.EditAction == DataGridEditAction.Commit)
//                    {
//                        System.Windows.Application.Current.Dispatcher.Invoke(() => AddICD9BlankRow());
//                    }
//                }
//            }

//        }


//        private bool CheckExistsICD9(DiagnosisICD9Items Item, bool HasMessage = true)
//        {
//            int i = 0;
//            if (Item.RefICD9 == null)
//            {
//                return true;
//            }
//            foreach (DiagnosisICD9Items p in refICD9List)
//            {
//                if (p.RefICD9 != null)
//                {
//                    if (Item.RefICD9.ICD9Code == p.RefICD9.ICD9Code)
//                    {
//                        i++;
//                    }
//                }
//            }
//            if (i > 1)
//            {
//                Item.RefICD9 = null;
//                if (HasMessage)
//                {
//                    //Remind change the message
//                    MessageBox.Show(eHCMSResources.Z1909_G1_ICD9DaTonTai);
//                }
//                return false;
//            }
//            else
//            {
//                return true;
//            }
//        }

//        public void GetTreatment(RefICD9 refICD9)
//        {
//            if (refICD9 != null)
//            {
//                TreatmentNew = refICD9.ProcedureName;
//                if (TreatmentOld != "")
//                {
//                    DiagTrmtItem.Treatment = DiagTrmtItem.Treatment.Replace(TreatmentOld, TreatmentNew);
//                }
//                else
//                {
//                    if (string.IsNullOrWhiteSpace(DiagTrmtItem.Treatment))
//                    {
//                        DiagTrmtItem.Treatment += TreatmentNew;
//                    }
//                    else
//                    {
//                        DiagTrmtItem.Treatment += "- " + TreatmentNew;
//                    }
//                }
//                TreatmentOld = ObjectCopier.DeepCopy(TreatmentNew);
//            }

//        }

//        public void grdICD9_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            DiagnosisICD9Items item = ((DataGrid)sender).SelectedItem as DiagnosisICD9Items;
//            if (item != null && item.RefICD9 != null)
//            {
//                RefICD9Copy = item.RefICD9;
//                TreatmentNew = TreatmentOld = ObjectCopier.DeepCopy(item.RefICD9.ProcedureName);
//                RefICD9Copy = ObjectCopier.DeepCopy(item.RefICD9);
//            }
//            else
//            {
//                TreatmentNew = TreatmentOld = "";
//                RefICD9Copy = null;
//            }
//        }

//        public void lnkDeleteICD9_Click(object sender, RoutedEventArgs e)
//        {
//            if (refICD9Item == null
//                || refICD9Item.RefICD9 == null)
//            {
//                MessageBox.Show(eHCMSResources.Z0405_G1_KgTheXoaDongRong);
//                return;
//            }

//            int nSelIndex = grdICD9.SelectedIndex;
//            if (nSelIndex >= refICD9List.Count - 1)
//            {
//                MessageBox.Show(eHCMSResources.Z0405_G1_KgTheXoaDongRong);
//                return;
//            }

//            var item = refICD9List[nSelIndex];

//            if (item != null && item.ICD9Code != null && item.ICD9Code != "")
//            {
//                if (MessageBox.Show(eHCMSResources.Z1910_G1_BanMuonXoaICD9, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
//                {
//                    if (refICD9Item.RefICD9.ProcedureName != "")
//                    {
//                        DiagTrmtItem.Treatment = DiagTrmtItem.Treatment.Replace(refICD9Item.RefICD9.ProcedureName, "");
//                    }
//                    refICD9List.Remove(refICD9List[nSelIndex]);
//                }
//            }
//        }


//        #endregion

//        #region Autocomplete ICD9-Code

//        AutoCompleteBox AutoICD9Code;

//        private PagedSortableCollectionView<RefICD9> _pageIDC9;
//        public PagedSortableCollectionView<RefICD9> pageIDC9
//        {
//            get
//            {
//                return _pageIDC9;
//            }
//            set
//            {
//                if (_pageIDC9 != value)
//                {
//                    _pageIDC9 = value;
//                }
//                NotifyOfPropertyChange(() => pageIDC9);
//            }
//        }

//        public void aucICD9_Populating(object sender, PopulatingEventArgs e)
//        {
//            if (IsICD9Code)
//            {
//                e.Cancel = true;
//                AutoICD9Code = (AutoCompleteBox)sender;
//                procName = e.Parameter;
//                ICD9SearchType = 0;
//                pageIDC9.PageIndex = 0;
//                SearchICD9(e.Parameter, 0, 0, pageIDC9.PageSize);
//            }
//        }

//        public void aucICD9_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
//        {
//            if (!isDropDownICD9)
//            {
//                return;
//            }
//            isDropDownICD9 = false;
//            AutoICD9Code = (AutoCompleteBox)sender;
//            if (refICD9Item != null)
//            {
//                refICD9Item.RefICD9 = AutoICD9Code.SelectedItem as RefICD9;
//                if (CheckExistsICD9(refICD9Item))
//                {
//                    GetTreatment(refICD9Item.RefICD9);
//                }
//            }
//        }

//        private bool isDropDownICD9 = false;
//        public void aucICD9_DropDownClosing(object sender, RoutedPropertyChangedEventArgs<bool> e)
//        {
//            isDropDownICD9 = true;
//        }

//        public void SearchICD9(string name, byte type, int PageIndex, int PageSize)
//        {
//            var t = new Thread(() =>
//            {
//                using (var serviceFactory = new CommonUtilsServiceClient())
//                {
//                    var contract = serviceFactory.ServiceInstance;

//                    contract.BeginSearchRefICD9(name, PageIndex, PageSize, type, Globals.DispatchCallback((asyncResult) =>
//                    {

//                        try
//                        {
//                            int Total = 10;
//                            var results = contract.EndSearchRefICD9(out Total, asyncResult);
//                            pageIDC9.Clear();
//                            pageIDC9.TotalItemCount = Total;
//                            if (results != null)
//                            {
//                                foreach (RefICD9 p in results)
//                                {
//                                    pageIDC9.Add(p);
//                                }
//                            }
//                            if (ICD9SearchType == 0)
//                            {
//                                AutoICD9Code.ItemsSource = pageIDC9;
//                                AutoICD9Code.PopulateComplete();
//                            }
//                            else
//                            {
//                                AutoProcName.ItemsSource = pageIDC9;
//                                AutoProcName.PopulateComplete();
//                            }

//                        }
//                        catch (Exception ex)
//                        {
//                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//                        }
//                        finally
//                        {

//                        }

//                    }), null);

//                }

//            });

//            t.Start();
//        }



//        #endregion

//        #region ICD 9 - Procedure Name

//        //AutoCompleteBox auProcedureName;
//        private string procName = "";
//        private byte ICD9SearchType = 0;
//        //public void ProcedureName_Loaded(object sender, RoutedEventArgs e)
//        //{
//        //    auProcedureName = (AutoCompleteBox)sender;
//        //}

//        AutoCompleteBox AutoProcName;
//        public void ProcedureName_Populating(object sender, PopulatingEventArgs e)
//        {
//            if (!IsICD9Code && ColumnIndex == 1)
//            {
//                e.Cancel = true;
//                AutoProcName = (AutoCompleteBox)sender;
//                procName = e.Parameter;
//                ICD9SearchType = 1;
//                pageIDC9.PageIndex = 0;
//                SearchICD9(e.Parameter, 1, 0, pageIDC9.PageSize);
//            }
//        }

//        private bool isICD9DropDown = false;
//        public void ProcedureName_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
//        {
//            isICD9DropDown = true;
//        }

//        public void ProcedureName_DropDownClosing(object sender, RoutedPropertyChangedEventArgs<bool> e)
//        {
//            if (!isICD9DropDown)
//            {
//                return;
//            }
//            isICD9DropDown = false;
//            //auProcedureName = (AutoCompleteBox)sender;
//            refICD9Item.RefICD9 = ((AutoCompleteBox)sender).SelectedItem as RefICD9;
//            if (CheckExistsICD9(refICD9Item))
//            {
//                GetTreatment(refICD9Item.RefICD9);
//            }
//        }

//        #endregion

//        /*TMA*/
//        private bool _btGCTIsEnabled = true;
//        public bool btGCTIsEnabled
//        {
//            get
//            {
//                return _btGCTIsEnabled;
//            }
//            set
//            {
//                _btGCTIsEnabled = value;
//                NotifyOfPropertyChange(() => btGCTIsEnabled);
//            }
//        }

//        private bool _btGCT_CLS_IsEnabled = true;
//        public bool btGCT_CLS_IsEnabled
//        {
//            get
//            {
//                return _btGCT_CLS_IsEnabled;
//            }
//            set
//            {
//                _btGCT_CLS_IsEnabled = value;
//                NotifyOfPropertyChange(() => btGCT_CLS_IsEnabled);
//            }
//        }
//        /*TMA*/

//        public PatientRegistration ObjPatientRegistrationVIP { get; set; }

//        public void Handle(EventKhamChoVIP<PatientRegistration> message)
//        {
//            if (message != null)
//            {
//                ObjPatientRegistrationVIP = message.PtReg;
//            }
//        }

//        //==== #001 ====
//        public void grdConsultation_LoadingRow(object sender, DataGridRowEventArgs e)
//        {
//            DiagnosisIcd10Items objRows = e.Row.DataContext as DiagnosisIcd10Items;
//            if (objRows != null)
//            {
//                switch (objRows.IsMain)
//                {
//                    case true:
//                        e.Row.Background = new SolidColorBrush(Color.FromArgb(128, 250, 155, 232));
//                        break;
//                    default:
//                        e.Row.Background = new SolidColorBrush(Colors.White);
//                        break;
//                }
//                if (objRows.IsInvalid)
//                {
//                    e.Row.Background = new SolidColorBrush(Color.FromArgb(115, 114, 113, 30));
//                }
//            }

//        }
//        //==== #001 ====

//        /*TMA*/
//        //btGCT
//        //▼====== #004
//        //TTM:  Chuyển từ publish event sang gọi thông qua Interface và đặt hàm gọi vào Action.
//        //      Thông tin bệnh nhân, thông tin thẻ bảo hiểm ... phải đc truyền vào trong lúc người dùng mở popup.
//        //      Không lý do gì đóng popup lại rồi mới truyền dữ liệu.
//        //public void GetPaPerReferalFul(int V_TransferFormType, int PatientFindBy)
//        //{
//        //    PatientRegistration CurRegistration = Globals.PatientAllDetails.PtRegistrationInfo;
//        //    Action<IPaperReferalFull> onInitDlg = delegate (IPaperReferalFull TransferFromVm)
//        //    {
//        //        TransferFromVm.V_TransferFormType = V_TransferFormType;

//        //        TransferFromVm.V_GetPaperReferalFullFromOtherView = true;
//        //        if (IsOutPt && CurRegistration != null && CurRegistration.PtRegistrationID != 0)
//        //        {
//        //            TransferFromVm.CurrentTransferForm.CurPatientRegistration = new PatientRegistration();
//        //            TransferFromVm.CurrentTransferForm.CurPatientRegistration.PtRegistrationID = (long)CurRegistration.PtRegistrationID;
//        //            if (CurRegistration.HisID != null)
//        //                TransferFromVm.CurrentTransferForm.CurPatientRegistration.HisID = (long)CurRegistration.HisID.Value;
//        //        }
//        //        else if (!IsOutPt && CurRegistration != null && CurRegistration.PtRegistrationID > 0)
//        //        {
//        //            TransferFromVm.CurrentTransferForm.CurPatientRegistration = new PatientRegistration();
//        //            TransferFromVm.CurrentTransferForm.CurPatientRegistration.PtRegistrationID = (long)CurRegistration.PtRegistrationID;
//        //            if (CurRegistration.HisID_2 != null)
//        //                TransferFromVm.CurrentTransferForm.CurPatientRegistration.HisID = (long)CurRegistration.HisID_2.Value;
//        //            else if (CurRegistration.HisID != null)
//        //                TransferFromVm.CurrentTransferForm.CurPatientRegistration.HisID = (long)CurRegistration.HisID.Value;
//        //        }
//        //        this.ActivateItem(TransferFromVm);
//        //    };
//        //    GlobalsNAV.ShowDialog<IPaperReferalFull>(onInitDlg);
//        //    var mEvent = new TransferFormEvent();
//        //    mEvent.Item = new TransferForm();

//        //    mEvent.Item.PatientFindBy = PatientFindBy;

//        //    mEvent.Item.CurPatientRegistration = new PatientRegistration();
//        //    mEvent.Item.V_TransferFormType = V_TransferFormType;

//        //    mEvent.Item.TransferFormID = (long)0;
//        //    mEvent.Item.CurPatientRegistration.PtRegistrationID = (long)CurRegistration.PtRegistrationID;
//        //    if (CurRegistration.HisID != null)
//        //        mEvent.Item.CurPatientRegistration.HisID = (long)CurRegistration.HisID.Value;

//        //    if (CurRegistration != null)
//        //    {
//        //        if (CurRegistration.HealthInsurance != null)
//        //            mEvent.Item.CurPatientRegistration.HealthInsurance = CurRegistration.HealthInsurance;
//        //        if (CurRegistration.Patient != null)
//        //        {
//        //            mEvent.Item.CurPatientRegistration.Patient = CurRegistration.Patient;
//        //        }
//        //        if (DiagTrmtItem != null)
//        //        {
//        //            if (DiagTrmtItem.Diagnosis != null)
//        //                mEvent.Item.ClinicalSign = DiagTrmtItem.Diagnosis;
//        //            if (DiagTrmtItem.Treatment != null)
//        //                mEvent.Item.UsedServicesAndItems = DiagTrmtItem.Treatment;
//        //        }

//        //        if (DiagTrmtItem.DiagnosisFinal != null && DiagTrmtItem.ICD10List != null)
//        //        {
//        //            mEvent.Item.ICD10Final = DiagTrmtItem.ICD10List;
//        //            mEvent.Item.ICD10 = DiagTrmtItem.ICD10List;
//        //            mEvent.Item.DiagnosisTreatment_Final = DiagTrmtItem.DiagnosisFinal;
//        //            mEvent.Item.DiagnosisTreatment = DiagTrmtItem.DiagnosisFinal;
//        //        }
//        //        //FromDate,
//        //        if (DiagTrmtItem.PatientServiceRecord != null && DiagTrmtItem.PatientServiceRecord.ExamDate != null)
//        //            mEvent.Item.FromDate = DiagTrmtItem.PatientServiceRecord.ExamDate;
//        //        mEvent.Item.ToDate = DateTime.Now;
//        //        mEvent.Item.TransferDate = DateTime.Now;
//        //    }
//        //    mEvent.Item.TransferFromHos = new Hospital();
//        //    /*TMA 08/11/2017 Để trống bệnh viện tuyến trước theo yêu cầu của Mr Nguyên - Viện Tim --> bỏ dòng dưới*/
//        //    //mEvent.Item.TransferFromHos.HICode = Globals.ServerConfigSection.Hospitals.HospitalCode;
//        //    mEvent.Item.TransferToHos = new Hospital();

//        //    /*TMA 08/11/2017 thay đổi giá trị khác vs chuyển đến theo yêu cầu của Mr Nguyên - Viện Tim*/
//        //    mEvent.Item.V_TransferTypeID = 62604; // defalut : chuyễn giữa các cơ sở cùng tuyế 
//        //    mEvent.Item.V_PatientStatusID = 63002;//defalut : không cấp cứu
//        //    mEvent.Item.V_TransferReasonID = 62902;//default : yêu cầu chuyên môn
//        //    mEvent.Item.V_TreatmentResultID = 62702;//defalut : ko thuyên giảm. Nặng lên - V_TreatmentResult_V2 trong bảng lookup
//        //    mEvent.Item.V_CMKTID = 62801; // default : chuyển đúng tuyến, đúng chuyên môn kỹ thuật
//        //    Globals.EventAggregator.Publish(mEvent);
//        //}
//        public void GetPaPerReferalFul(int V_TransferFormType, int PatientFindBy)
//        {
//            PatientRegistration CurRegistration = Globals.PatientAllDetails.PtRegistrationInfo;
//            var mEvent = new TransferFormEvent();
//            mEvent.Item = new TransferForm();
//            mEvent.Item.PatientFindBy = PatientFindBy;
//            mEvent.Item.CurPatientRegistration = new PatientRegistration();
//            mEvent.Item.V_TransferFormType = V_TransferFormType;
//            mEvent.Item.TransferFormID = (long)0;
//            mEvent.Item.CurPatientRegistration.PtRegistrationID = (long)CurRegistration.PtRegistrationID;
//            if (CurRegistration.HisID != null)
//                mEvent.Item.CurPatientRegistration.HisID = (long)CurRegistration.HisID.Value;
//            if (CurRegistration != null)
//            {
//                if (CurRegistration.HealthInsurance != null)
//                    mEvent.Item.CurPatientRegistration.HealthInsurance = CurRegistration.HealthInsurance;
//                if (CurRegistration.Patient != null)
//                {
//                    mEvent.Item.CurPatientRegistration.Patient = CurRegistration.Patient;
//                }
//                if (DiagTrmtItem != null)
//                {
//                    if (DiagTrmtItem.Diagnosis != null)
//                        mEvent.Item.ClinicalSign = DiagTrmtItem.Diagnosis;
//                    if (DiagTrmtItem.Treatment != null)
//                        mEvent.Item.UsedServicesAndItems = DiagTrmtItem.Treatment;
//                }
//                if (DiagTrmtItem.DiagnosisFinal != null && DiagTrmtItem.ICD10List != null)
//                {
//                    mEvent.Item.ICD10Final = DiagTrmtItem.ICD10List;
//                    mEvent.Item.ICD10 = DiagTrmtItem.ICD10List;
//                    mEvent.Item.DiagnosisTreatment_Final = DiagTrmtItem.DiagnosisFinal;
//                    mEvent.Item.DiagnosisTreatment = DiagTrmtItem.DiagnosisFinal;
//                }
//                if (DiagTrmtItem.PatientServiceRecord != null && DiagTrmtItem.PatientServiceRecord.ExamDate != null)
//                    mEvent.Item.FromDate = DiagTrmtItem.PatientServiceRecord.ExamDate;
//                mEvent.Item.ToDate = DateTime.Now;
//                mEvent.Item.TransferDate = DateTime.Now;
//            }
//            mEvent.Item.TransferFromHos = new Hospital();
//            mEvent.Item.TransferToHos = new Hospital();
//            mEvent.Item.V_TransferTypeID = 62604;           //defalut : chuyễn giữa các cơ sở cùng tuyế 
//            mEvent.Item.V_PatientStatusID = 63002;          //defalut : không cấp cứu
//            mEvent.Item.V_TransferReasonID = 62902;         //default : yêu cầu chuyên môn
//            mEvent.Item.V_TreatmentResultID = 62702;        //defalut : ko thuyên giảm. Nặng lên - V_TreatmentResult_V2 trong bảng lookup
//            mEvent.Item.V_CMKTID = 62801;                   //default : chuyển đúng tuyến, đúng chuyên môn kỹ thuật

//            Action<IPaperReferalFull> onInitDlg = delegate (IPaperReferalFull TransferFromVm)
//            {
//                TransferFromVm.IsThisViewDialog = true;
//                TransferFromVm.V_TransferFormType = V_TransferFormType;

//                TransferFromVm.V_GetPaperReferalFullFromOtherView = true;
//                if (IsOutPt && CurRegistration != null && CurRegistration.PtRegistrationID != 0)
//                {
//                    TransferFromVm.CurrentTransferForm.CurPatientRegistration = new PatientRegistration();
//                    TransferFromVm.CurrentTransferForm.CurPatientRegistration.PtRegistrationID = (long)CurRegistration.PtRegistrationID;
//                    if (CurRegistration.HisID != null)
//                        TransferFromVm.CurrentTransferForm.CurPatientRegistration.HisID = (long)CurRegistration.HisID.Value;
//                }
//                else if (!IsOutPt && CurRegistration != null && CurRegistration.PtRegistrationID > 0)
//                {
//                    TransferFromVm.CurrentTransferForm.CurPatientRegistration = new PatientRegistration();
//                    TransferFromVm.CurrentTransferForm.CurPatientRegistration.PtRegistrationID = (long)CurRegistration.PtRegistrationID;
//                    if (CurRegistration.HisID_2 != null)
//                        TransferFromVm.CurrentTransferForm.CurPatientRegistration.HisID = (long)CurRegistration.HisID_2.Value;
//                    else if (CurRegistration.HisID != null)
//                        TransferFromVm.CurrentTransferForm.CurPatientRegistration.HisID = (long)CurRegistration.HisID.Value;
//                }
//                this.ActivateItem(TransferFromVm);
//                TransferFromVm.SetCurrentInformation(mEvent);
//            };
//            GlobalsNAV.ShowDialog<IPaperReferalFull>(onInitDlg);
//        }
//        //▲====== #004
//        public void btGCT()
//        {
//            if (IsOutPt)
//                GetPaPerReferalFul((int)AllLookupValues.V_TransferFormType.CHUYEN_Di, (int)AllLookupValues.PatientFindBy.NGOAITRU);
//            else
//                GetPaPerReferalFul((int)AllLookupValues.V_TransferFormType.CHUYEN_Di, (int)AllLookupValues.PatientFindBy.NOITRU);
//        }
//        public void btGCT_CLS()
//        {
//            if (IsOutPt)
//                GetPaPerReferalFul((int)AllLookupValues.V_TransferFormType.CHUYEN_DI_CLS,(int)AllLookupValues.PatientFindBy.NGOAITRU);
//            else
//                GetPaPerReferalFul((int)AllLookupValues.V_TransferFormType.CHUYEN_DI_CLS, (int)AllLookupValues.PatientFindBy.NOITRU);
//        }

//        //btGCT_CLS
//        /*TMA*/
//        /*TMA*/
//        /*▼====: #002*/
//        public void btnCatastropheHistory()
//        {
//            Action<ICatastropheHistory> onInitDlg = delegate (ICatastropheHistory mView)
//            {
//                if (Globals.PatientAllDetails.PtRegistrationInfo != null)
//                    mView.PtRegistrationID = Globals.PatientAllDetails.PtRegistrationInfo.PtRegistrationID;
//            };
//            GlobalsNAV.ShowDialog<ICatastropheHistory>(onInitDlg);
//        }
//        /*▲====: #002*/

//        public bool CheckValidDiagnosis()
//        {
//            if (refIDC10List != null && refIDC10List.Any(x => x.IsInvalid))
//            {
//                MessageBox.Show(string.Format(eHCMSResources.Z2205_G1_ICD10KhongHopLe, string.Join(",", refIDC10List.Where(x => x.IsInvalid).Select(x => x.ICD10Code).ToList())), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
//                return false;
//            }
//            if (!CheckEmptyFields())
//            {
//                return false;
//            }

//            long lBehaving = 0;
//            try
//            {
//                lBehaving = DiagTrmtItem.PatientServiceRecord.V_Behaving.GetValueOrDefault();
//            }
//            catch
//            {
//                MessageBox.Show(eHCMSResources.A0367_G1_Msg_InfoChonTieuDe);
//                return false;
//            }

//            long lPMRTemplateID = 0;
//            try
//            {
//                lPMRTemplateID = DiagTrmtItem.MDRptTemplateID;
//            }
//            catch
//            {
//                MessageBox.Show(eHCMSResources.A0337_G1_Msg_InfoChonMauBAn);
//                return false;
//            }

//            DiagTrmtItem.PatientServiceRecord.Staff = Globals.LoggedUserAccount.Staff;
//            DiagTrmtItem.PatientServiceRecord.StaffID = Globals.LoggedUserAccount.Staff.StaffID;

//            if (CheckedIsMain() && NeedICD10())
//            {
//                //Khám DV cụ thể nào
//                if (Globals.PatientAllDetails.PtRegistrationDetailInfo != null && Globals.PatientAllDetails.PtRegistrationDetailInfo.PtRegDetailID > 0)
//                {
//                    DiagTrmtItem.PtRegDetailID = Globals.PatientAllDetails.PtRegistrationDetailInfo.PtRegDetailID;
//                }
//                else
//                {
//                    DiagTrmtItem.PtRegDetailID = 0;
//                }
//                //Khám DV cụ thể nào

//                DiagTrmtItem.PatientServiceRecord.PatientMedicalRecord.PatientID = Globals.PatientAllDetails.PatientInfo.PatientID;
//                DiagTrmtItem.PatientServiceRecord.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
//                DiagTrmtItem.PatientServiceRecord.PtRegistrationID = Globals.PatientAllDetails.PtRegistrationInfo.PtRegistrationID;
//                DiagTrmtItem.DeptLocationID = Globals.DeptLocation.DeptLocationID;

//                DiagTrmtItem.PatientServiceRecord.V_RegistrationType = Globals.PatientAllDetails.PtRegistrationInfo.V_RegistrationType;

//                DiagTrmtItem.PatientServiceRecord.V_DiagnosisType = (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_NORMAL;

//                DiagTrmtItem.ICD10List = String.Join(",", from item in refIDC10List
//                                                          where (!string.IsNullOrEmpty(item.ICD10Code) && item.DiseasesReference != null)
//                                                          select item.ICD10Code);

//                PatientRegistration CurRegistration = Globals.PatientAllDetails.PtRegistrationInfo;
//                if (CurRegistration.PtInsuranceBenefit != null && CurRegistration.PtInsuranceBenefit > 0 && refIDC10List != null && refIDC10List.Any(x => x.ICD10Code != null && x.DiseasesReference != null && x.DiseasesReference.ICD10Code.Contains("Z10")))
//                {
//                    warningtask = new MessageWarningShowDialogTask(content, eHCMSResources.Z0339_G1_TiepTucLuuCDoan);
//                    warningtask.Execute(null);
//                    if (!warningtask.IsAccept)
//                    {
//                        return false;
//                    }
//                }
//                return true;
//            }
//            return false;
//        }
//        public void ChangeStatesAfterUpdated(bool IsUpdate = false)
//        {
//            if (IsUpdate)
//                FormEditorIsEnabled = false;
//            StateEdit();
//        }
//        private bool _IsShowSummaryContent = true;
//        public bool IsShowSummaryContent
//        {
//            get => _IsShowSummaryContent; set
//            {
//                _IsShowSummaryContent = value;
//                NotifyOfPropertyChange(() => IsShowSummaryContent);
//                NotifyOfPropertyChange(() => mSaveConsultationState);
//                NotifyOfPropertyChange(() => mUpdateConsultationState);
//            }
//        }
        
//        public bool IsOutPt
//        {
//            get { return Globals.PatientAllDetails.PtRegistrationInfo == null || Globals.PatientAllDetails.PtRegistrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU; }
//        }
//    }
//}