using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ServiceClient.Consultation_PCLs;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Controls;
using eHCMSLanguage;
using System.Linq;
using aEMR.Common.BaseModel;
using aEMR.Common;
/*
* 20181023 #001 TTM:   BM 0002173: Thay đổi cách lưu, cập nhật và lấy lên của tình trạng thể chất => tất cả đều dựa vào lần đăng ký.    
* 20200121 #002 TBL:   BM 0021818: Chỉnh sửa lại cách nhập dữ liệu màn hình Theo dõi sinh hiệu - Nội trúx
* 20210610 #003 TNHX:  331: Kiểm tra giá trị tối đa cho DHST
* 20210905 #004 TNHX:  Chỉnh lại kiểm tra giá trị tối đa cho DHST + cảnh báo
* 20220607 #005 BLQ: Thêm trường đánh giá ý thức và mức độ đau
* 20221213 #006 QTD  Thêm trường Ngày y lệnh cho phép chỉnh sửa
* 20230626 #007 DatTB: Thêm thông tin khoa/phòng lưu sinh hiệu
*/
namespace aEMR.ConsultantEPrescription.CommonRecs.ViewModels
{
    [Export(typeof(IPhysicalExamination)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PhysicalExaminationViewModel : ViewModelBase, IPhysicalExamination
        , IHandle<ShowPatientInfo_KHAMBENH_TONGQUAT<Patient, PatientRegistration, PatientRegistrationDetail>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PhysicalExaminationViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            //Khi khoi tao module thi load menu ben trai luon.
            //Globals.EventAggregator.Subscribe(this);
            //Thay bằng
            Globals.EventAggregator.Subscribe(this);
            authorization();
            _refSmokeStatus = new ObservableCollection<Lookup>();
            _refAlcoholDrinkingStatus = new ObservableCollection<Lookup>();
            _PtPhyExamList = new ObservableCollection<PhysicalExamination>();
            selectPhyExamList = new PhysicalExamination();
            MedicalInstructionDateContent = Globals.GetViewModel<IMinHourDateControl>();
            //20190928 TBL: Set ngày y lệnh = ngày giờ hiện tại (không lấy giây)
            MedicalInstructionDateContent.DateTime = new DateTime(Globals.GetCurServerDateTime().Year, Globals.GetCurServerDateTime().Month, Globals.GetCurServerDateTime().Day, Globals.GetCurServerDateTime().Hour, Globals.GetCurServerDateTime().Minute, 0);
            (MedicalInstructionDateContent as Conductor<object>).PropertyChanged += (s, e) =>
            {
                if (e.PropertyName.Equals("DateTime"))
                {
                    selectPhyExamList.RecordDate = MedicalInstructionDateContent.DateTime;
                }
            };
            LoadSmokeStatus();
            LoadAlcoholDrinkingStatus();
            LoadLookupValues();
        }

        public void Handle(ShowPatientInfo_KHAMBENH_TONGQUAT<Patient, PatientRegistration, PatientRegistrationDetail> message)
        {
            InitPatientInfo();
        }

        public void InitPatientInfo()
        {
            _PtPhyExamList = new ObservableCollection<PhysicalExamination>();
            if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatient != null && Registration_DataStorage.CurrentPatient.PatientID > 0)
            {
                LoadPhyExamByPtID(Registration_DataStorage.CurrentPatient.PatientID, Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID);
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            InitPatientInfo();
        }

        public void grdCommonRecordLoaded(object sender, RoutedEventArgs e)
        {
            grdCommonRecord = sender as AxDataGridEx;
            if (!mTongQuat_XemThongTin)
            {
                grdCommonRecord.IsReadOnly = true;
            }
            if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatient != null && Registration_DataStorage.CurrentPatient.PatientID > 0)
            {
                grdCommonRecord.IsEnabled = true;
            }
            else
            {
                grdCommonRecord.IsEnabled = false;
            }
        }

        public AxDataGridEx grdCommonRecord { get; set; }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            mTongQuat_XemThongTin = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                                   , (int)eConsultation.mPtDashboardCommonRecs,
                                                   (int)oConsultationEx.mTongQuat_XemThongTin, (int)ePermission.mView);
            mTongQuat_ChinhSuaThongTin = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                                   , (int)eConsultation.mPtDashboardCommonRecs,
                                                   (int)oConsultationEx.mTongQuat_ChinhSuaThongTin, (int)ePermission.mView);
        }

        #region account checking
        private bool _mTongQuat_XemThongTin = true;
        private bool _mTongQuat_ChinhSuaThongTin = true && Globals.isConsultationStateEdit;

        public bool mTongQuat_XemThongTin
        {
            get
            {
                return _mTongQuat_XemThongTin;
            }
            set
            {
                if (_mTongQuat_XemThongTin == value)
                    return;
                _mTongQuat_XemThongTin = value;
            }
        }

        public bool mTongQuat_ChinhSuaThongTin
        {
            get
            {
                return _mTongQuat_ChinhSuaThongTin;
            }
            set
            {
                if (_mTongQuat_ChinhSuaThongTin == value)
                    return;
                _mTongQuat_ChinhSuaThongTin = value && Globals.isConsultationStateEdit;
            }
        }

        #endregion

        #region binding visibilty
        public Button lnkDelete { get; set; }
        public Button lnkEdit { get; set; }
        public Button lnkSave { get; set; }
        public Button lnkCancel { get; set; }

        public void lnkDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;
            lnkDelete.Visibility = Globals.convertVisibility(mTongQuat_ChinhSuaThongTin);
        }

        public void lnkEdit_Loaded(object sender)
        {
            lnkEdit = sender as Button;
            lnkEdit.Visibility = Globals.convertVisibility(mTongQuat_ChinhSuaThongTin);
        }

        public void lnkSave_Loaded(object sender)
        {
            lnkSave = sender as Button;
            lnkSave.Visibility = Globals.convertVisibility(mTongQuat_ChinhSuaThongTin);
        }

        public void lnkCancel_Loaded(object sender)
        {
            lnkCancel = sender as Button;
            lnkCancel.Visibility = Globals.convertVisibility(mTongQuat_ChinhSuaThongTin);
        }
        #endregion

        #region property
        private PhysicalExamination _selectPhyExamList;
        public PhysicalExamination selectPhyExamList
        {
            get
            {
                return _selectPhyExamList;
            }
            set
            {
                if (_selectPhyExamList == value)
                    return;
                _selectPhyExamList = value;
                NotifyOfPropertyChange(() => selectPhyExamList);
                GetDataComboBox();
            }
        }

        private ObservableCollection<PhysicalExamination> _PtPhyExamList;
        public ObservableCollection<PhysicalExamination> PtPhyExamList
        {
            get
            {
                return _PtPhyExamList;
            }
            set
            {
                if (_PtPhyExamList == value)
                    return;
                _PtPhyExamList = value;
                NotifyOfPropertyChange(() => PtPhyExamList);
            }
        }

        private long _V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU;
        public long V_RegistrationType
        {
            get { return _V_RegistrationType; }
            set
            {
                if (_V_RegistrationType != value)
                {
                    _V_RegistrationType = value;
                    NotifyOfPropertyChange(() => V_RegistrationType);
                }
            }
        }
        #endregion

        public object Delete { get; set; }

        public bool CheckValid(object temp)
        {
            PhysicalExamination u = temp as PhysicalExamination;
            if (u == null)
            {
                return false;
            }
            //▼====: #004
            // kiểm tra chặn trước
            //▼====: #003
            string ErrorMessage = "";
            if (u.Temperature > GlobalsNAV.BLOCK_TEMPERATURE_TOP || u.Temperature < GlobalsNAV.BLOCK_TEMPERATURE_BOTTOM)
            {
                ErrorMessage += string.Format(" - Giá trị \"Nhiệt độ\" ngoài khoảng cho phép {0} - {1}!", GlobalsNAV.BLOCK_TEMPERATURE_BOTTOM, GlobalsNAV.BLOCK_TEMPERATURE_TOP);
            }
            if (u.SystolicPressure > GlobalsNAV.BLOCK_UPPER_PRESSURE_TOP || u.SystolicPressure < GlobalsNAV.BLOCK_PRESSURE_BOTTOM)
            {
                ErrorMessage += string.Format("\n - Giá trị \"Huyết áp trên\" ngoài khoảng cho phép {0} - {1}!", GlobalsNAV.BLOCK_PRESSURE_BOTTOM, GlobalsNAV.BLOCK_UPPER_PRESSURE_TOP);
            }
            if (u.SystolicPressure.GetValueOrDefault() > 0)
            {
                if (u.DiastolicPressure.GetValueOrDefault() > GlobalsNAV.BLOCK_LOWER_PRESSURE_TOP || u.DiastolicPressure.GetValueOrDefault() < GlobalsNAV.BLOCK_PRESSURE_BOTTOM)
                {
                    ErrorMessage += string.Format("\n - Giá trị \"Huyết áp dưới\" ngoài khoảng cho phép {0} - {1}!", GlobalsNAV.BLOCK_PRESSURE_BOTTOM, GlobalsNAV.BLOCK_LOWER_PRESSURE_TOP);
                }
            }
            if (u.DiastolicPressure > GlobalsNAV.BLOCK_LOWER_PRESSURE_TOP || u.DiastolicPressure < GlobalsNAV.BLOCK_PRESSURE_BOTTOM)
            {
                ErrorMessage += string.Format("\n - Giá trị \"Huyết áp dưới\" ngoài khoảng cho phép {0} - {1}!", GlobalsNAV.BLOCK_PRESSURE_BOTTOM, GlobalsNAV.BLOCK_LOWER_PRESSURE_TOP);
            }
            if (u.DiastolicPressure.GetValueOrDefault() > 0)
            {
                if (u.SystolicPressure.GetValueOrDefault() > GlobalsNAV.BLOCK_UPPER_PRESSURE_TOP || u.SystolicPressure.GetValueOrDefault() < GlobalsNAV.BLOCK_PRESSURE_BOTTOM)
                {
                    ErrorMessage += string.Format("\n - Giá trị \"Huyết áp trên\" ngoài khoảng cho phép {0} - {1}!", GlobalsNAV.BLOCK_PRESSURE_BOTTOM, GlobalsNAV.BLOCK_UPPER_PRESSURE_TOP);
                }
            }
            if (u.SpO2 > GlobalsNAV.BLOCK_SPO2_TOP || u.SpO2 < GlobalsNAV.BLOCK_SPO2_BOTTOM)
            {
                ErrorMessage += string.Format("\n - Giá trị \"SpO2\" ngoài khoảng cho phép {0} - {1}!", GlobalsNAV.BLOCK_SPO2_BOTTOM, GlobalsNAV.BLOCK_SPO2_TOP);
            }
            if (u.Weight > GlobalsNAV.BLOCK_WEIGHT_TOP || u.Weight <= GlobalsNAV.BLOCK_WEIGHT_BOTTOM)
            {
                ErrorMessage += string.Format("\n - Giá trị \"Cân nặng\" ngoài khoảng cho phép {0} - {1}!", GlobalsNAV.BLOCK_WEIGHT_BOTTOM, GlobalsNAV.BLOCK_WEIGHT_TOP);
            }
            if (u.Pulse > GlobalsNAV.BLOCK_PULSE_TOP || u.Pulse < GlobalsNAV.BLOCK_PULSE_BOTTOM)
            {
                ErrorMessage += string.Format("\n - Giá trị \"Nhịp tim\" ngoài khoảng cho phép {0} - {1}!", GlobalsNAV.BLOCK_PULSE_BOTTOM, GlobalsNAV.BLOCK_PULSE_TOP);
            }
            if (u.RespiratoryRate > GlobalsNAV.BLOCK_RESPIRATORY_RATE_TOP || u.RespiratoryRate < GlobalsNAV.BLOCK_RESPIRATORY_RATE_BOTTOM)
            {
                ErrorMessage += string.Format("\n - Giá trị \"Nhịp thở\" ngoài khoảng cho phép {0} - {1}!", GlobalsNAV.BLOCK_RESPIRATORY_RATE_BOTTOM, GlobalsNAV.BLOCK_RESPIRATORY_RATE_TOP);
            }
            //▼====: #005
            if(u.V_PainLevel == null && Globals.ServerConfigSection.CommonItems.DeptCheckPainLevel.Contains("|" + 
                Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.InPatientDeptDetails.Where(x=>x.ToDate == null).FirstOrDefault().DeptLocation.DeptID + "|")
                && Registration_DataStorage.CurrentPatient.Age > 15)
            {
                ErrorMessage += string.Format("\n - Người bệnh chưa được đánh giá mức độ đau!");
            }
            if (PtPhyExamList.Count > 0 && PtPhyExamList.Where(x => x.RecordDate > MedicalInstructionDateContent.DateTime).FirstOrDefault() != null
                && V_RegistrationType == (long)AllLookupValues.RegistrationType.NOI_TRU && u.PhyExamID == 0)
            {
                ErrorMessage += string.Format("\n - Ngày y lệnh không được nhỏ hơn ngày y lệnh trước đó!");
            }
            //▲====: #005
            if (ErrorMessage != "")
            {
                void onInitDlg(IErrorBold confDlg)
                {
                    confDlg.ErrorTitle = eHCMSResources.K1576_G1_CBao;
                    confDlg.isCheckBox = false;
                    confDlg.SetMessage(ErrorMessage, "");
                    confDlg.FireOncloseEvent = true;
                }
                GlobalsNAV.ShowDialog<IErrorBold>(onInitDlg);
                return false;
            }
            //▲====== #003
            // kiểm tra cảnh báo
            string WarningMessage = "";
            if ((u.Temperature <= GlobalsNAV.BLOCK_TEMPERATURE_TOP && u.Temperature > GlobalsNAV.WARNING_TEMPERATURE_TOP)
                || (u.Temperature < GlobalsNAV.WARNING_TEMPERATURE_BOTTOM && u.Temperature >= GlobalsNAV.BLOCK_TEMPERATURE_BOTTOM))
            {
                WarningMessage += string.Format("\n - Giá trị \"Nhiệt độ\" ngoài khoảng {0} - {1}!", GlobalsNAV.WARNING_TEMPERATURE_BOTTOM, GlobalsNAV.WARNING_TEMPERATURE_TOP);
            }
            if ((u.Pulse < GlobalsNAV.BLOCK_PULSE_TOP && u.Pulse > GlobalsNAV.WARNING_PULSE_TOP)
                || (u.Pulse < GlobalsNAV.WARNING_PULSE_BOTTOM && u.Pulse > GlobalsNAV.BLOCK_PULSE_BOTTOM))
            {
                WarningMessage += string.Format("\n - Giá trị \"Nhịp tim\" ngoài khoảng {0} - {1}!", GlobalsNAV.WARNING_PULSE_BOTTOM, GlobalsNAV.WARNING_PULSE_TOP);
            }
            if ((u.SpO2 < GlobalsNAV.BLOCK_SPO2_TOP && u.SpO2 > GlobalsNAV.WARNING_SPO2_TOP)
                || (u.SpO2 < GlobalsNAV.WARNING_SPO2_BOTTOM && u.SpO2 > GlobalsNAV.BLOCK_SPO2_BOTTOM))
            {
                WarningMessage += string.Format("\n - Giá trị \"SpO2\" ngoài khoảng {0} - {1}!", GlobalsNAV.WARNING_SPO2_BOTTOM, GlobalsNAV.WARNING_SPO2_TOP);
            }
            if ((u.Weight < GlobalsNAV.BLOCK_WEIGHT_TOP && u.Weight > GlobalsNAV.WARNING_WEIGHT_TOP)
                || (u.Weight < GlobalsNAV.WARNING_WEIGHT_BOTTOM && u.Weight > GlobalsNAV.BLOCK_WEIGHT_BOTTOM))
            {
                WarningMessage += string.Format("\n - Giá trị \"Cân nặng\" ngoài khoảng {0} - {1}!", GlobalsNAV.WARNING_WEIGHT_BOTTOM, GlobalsNAV.WARNING_WEIGHT_TOP);
            }
            if ((u.SystolicPressure < GlobalsNAV.BLOCK_UPPER_PRESSURE_TOP && u.SystolicPressure > GlobalsNAV.WARNING_UPPER_PRESSURE_TOP)
                || (u.SystolicPressure < GlobalsNAV.WARNING_UPPER_PRESSURE_BOTTOM && u.SystolicPressure > GlobalsNAV.BLOCK_PRESSURE_BOTTOM))
            {
                WarningMessage += string.Format("\n - Giá trị \"Huyết áp trên\" ngoài khoảng {0} - {1}!", GlobalsNAV.WARNING_UPPER_PRESSURE_BOTTOM, GlobalsNAV.WARNING_UPPER_PRESSURE_TOP);
            }
            if ((u.DiastolicPressure < GlobalsNAV.BLOCK_LOWER_PRESSURE_TOP && u.DiastolicPressure > GlobalsNAV.WARNING_LOWER_PRESSURE_TOP)
                || (u.DiastolicPressure < GlobalsNAV.WARNING_LOWER_PRESSURE_BOTTOM && u.DiastolicPressure > GlobalsNAV.BLOCK_PRESSURE_BOTTOM))
            {
                WarningMessage += string.Format("\n - Giá trị \"Huyết áp dưới\" ngoài khoảng {0} - {1}!", GlobalsNAV.WARNING_LOWER_PRESSURE_BOTTOM, GlobalsNAV.WARNING_LOWER_PRESSURE_TOP);
            }
            if (u.RespiratoryRate > GlobalsNAV.WARNING_RESPIRATORY_RATE_TOP || u.RespiratoryRate < GlobalsNAV.WARNING_RESPIRATORY_RATE_BOTTOM)
            {
                WarningMessage += string.Format("\n - Giá trị \"Nhịp thở\" ngoài khoảng {0} - {1}!", GlobalsNAV.WARNING_RESPIRATORY_RATE_BOTTOM, GlobalsNAV.WARNING_RESPIRATORY_RATE_TOP);
            }
            //▼====: #005
            if (u.V_PainLevel == null && !Globals.ServerConfigSection.CommonItems.DeptCheckPainLevel.Contains("|" +
                Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.InPatientDeptDetails.Where(x => x.ToDate == null).FirstOrDefault().DeptLocation.DeptID + "|")
                && Registration_DataStorage.CurrentPatient.Age > 15)
            {
                WarningMessage += string.Format("\n - Người bệnh chưa được đánh giá mức độ đau!");
            }
            //▲====: #005
            if (WarningMessage != "")
            {
                IErrorBold MessBox = Globals.GetViewModel<IErrorBold>();
                MessBox.isCheckBox = true;
                MessBox.ErrorTitle = "Cảnh báo bệnh lý";
                MessBox.SetMessage(WarningMessage, eHCMSResources.Z0627_G1_TiepTucLuu);
                MessBox.FireOncloseEvent = true;
                GlobalsNAV.ShowDialog_V3(MessBox);
                if (MessBox.IsAccept)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            //▲====== #004
            return u.Validate();
        }

        public bool CheckNullValue(PhysicalExamination pe)
        {
            if (pe.Alcohol_CurrentHeavy == null
                && pe.Height == null
                && pe.Weight == null
                && pe.DiastolicPressure == null
                && pe.SystolicPressure == null
                && pe.Pulse == null
                && pe.Cholesterol == null
                && pe.RefAlcohol == null
                && pe.RefSmoke == null
                && pe.OxygenBreathing == null
                && pe.V_ConsciousnessLevel == null
                && pe.V_PainLevel == null
                )
            {
                Globals.ShowMessage(eHCMSResources.Z0503_G1_ChonItNhatMotTTTheChat, "");
                return false;
            }
            return true;
        }
        //▼====== #002
        public void btnCancel()
        {
            selectPhyExamList = new PhysicalExamination();
            isEdit = false;
        }

        public void btnSave()
        {
            if (!CheckValid(selectPhyExamList))
            {
                return;
            }
            if (!CheckNullValue(selectPhyExamList))//Kiem tra xem tat ca dk co bi null hay ko
            {
                return;
            }
            //▼==== #007
            if (selectPhyExamList != null && Registration_DataStorage != null && Registration_DataStorage.CurrentPatientRegistration != null 
                && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo != null && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.InPatientDeptDetails != null)
            {
                if (Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.InPatientDeptDetails.Where(x => x.IsActive).Count() > 0)
                {
                    selectPhyExamList.DeptLocID = Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.InPatientDeptDetails.Where(x => x.IsActive).FirstOrDefault().DeptLocID;
                }
                else
                {
                    selectPhyExamList.DeptLocID = 0;
                }
            }
            //▲==== #007
            if (!isEdit || selectPhyExamList.PhyExamID == 0)
            {
                selectPhyExamList.CommonMedicalRecord = new CommonMedicalRecord();
                selectPhyExamList.CommonMedicalRecord.PatientID = Registration_DataStorage.CurrentPatient.PatientID;//cai nay sua 
                //validate o day
                if (selectPhyExamList.RefAlcohol == null)
                {
                    selectPhyExamList.RefAlcohol = new Lookup();
                }
                if (selectPhyExamList.RefSmoke == null)
                {
                    selectPhyExamList.RefSmoke = new Lookup();
                }
                if (V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU)
                {
                    AddNewPhyExam_V2(selectPhyExamList);
                }
                else
                {
                    AddNewPhyExam_InPT(selectPhyExamList);
                }
            }
            else
            {
                if (V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU)
                {
                    UpdatePhyExam_V2(selectPhyExamList, Globals.LoggedUserAccount.Staff.StaffID, selectPhyExamList.PhyExamID);
                }
                else
                {
                    UpdatePhyExam_InPT(selectPhyExamList, Globals.LoggedUserAccount.Staff.StaffID, selectPhyExamList.PhyExamID);
                }
                isEdit = false;
            }
        }
        //▲====== #002

        public void lnkSaveClick(RoutedEventArgs e)
        {
            if (Globals.isAccountCheck)
            {
                if (!mTongQuat_ChinhSuaThongTin)
                {
                    Globals.ShowMessage(eHCMSResources.Z0523_G1_ChuaCapQuyenThayDoiTTin, "");
                    return;
                }
            }
            grdCommonRecord.Cancel();
            ((PhysicalExamination)grdCommonRecord.SelectedItem).isEdit = true;
            if (!CheckValid(selectPhyExamList))
            {
                return;
            }
            if (!CheckNullValue(selectPhyExamList))//Kiem tra xem tat ca dk co bi null hay ko
            {
                return;
            }

            //save o day
            if (!isEdit || selectPhyExamList.PhyExamID == 0)
            {
                selectPhyExamList.CommonMedicalRecord = new CommonMedicalRecord();
                selectPhyExamList.CommonMedicalRecord.PatientID = Registration_DataStorage.CurrentPatient.PatientID;//cai nay sua 
                //validate o day
                if (selectPhyExamList.RefAlcohol == null)
                {
                    selectPhyExamList.RefAlcohol = new Lookup();
                }
                if (selectPhyExamList.RefSmoke == null)
                {
                    selectPhyExamList.RefSmoke = new Lookup();
                }
                //▼====== #001
                //AddNewPhysicalExamination(selectPhyExamList,2);
                if (V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU)
                {
                    AddNewPhyExam_V2(selectPhyExamList);
                }
                //▲====== #001
                else
                {
                    AddNewPhyExam_InPT(selectPhyExamList);
                }
            }
            else
            {
                //▼====== #001
                //UpdatePhysicalExamination(selectPhyExamList,2,selectPhyExamList.PhyExamID);
                if (V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU)
                {
                    UpdatePhyExam_V2(selectPhyExamList, Globals.LoggedUserAccount.Staff.StaffID, selectPhyExamList.PhyExamID);
                }
                else
                {
                    UpdatePhyExam_InPT(selectPhyExamList, Globals.LoggedUserAccount.Staff.StaffID, selectPhyExamList.PhyExamID);
                }
                isEdit = false;
                //▲====== #001
            }
        }

        public void lnkCancel_Click(RoutedEventArgs e)
        {
            if (V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                GetPhyExamByPtID(Registration_DataStorage.CurrentPatient.PatientID);
            }
            else
            {
                GetPhyExamByPtID_InPT(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID);
            }
        }

        public void lnkDeleteClick(object selectItem)
        {
            //if (selectPhyExamList == null || selectPhyExamList.CommonMedRecID == null || selectPhyExamList.CommonMedRecID == 0)
            //{
            //    Globals.ShowMessage(eHCMSResources.Z0384_G1_DongTrong, "");
            //    return;
            //}
            //if (V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU && MessageBox.Show(eHCMSResources.Z0382_G1_BanCoChacXoaDongNayKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            //{
            //    DeletePhysicalExamination(2, selectPhyExamList.PhyExamID, selectPhyExamList.CommonMedRecID);
            //}
            //else if (V_RegistrationType == (long)AllLookupValues.RegistrationType.NOI_TRU && MessageBox.Show(eHCMSResources.Z0382_G1_BanCoChacXoaDongNayKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            //{
            //    DeletePhysicalExamination_InPT(selectPhyExamList.PhyExamID, selectPhyExamList.CommonMedRecID);
            //}
            //▼====== #002
            if (selectItem != null)
            {
                PhysicalExamination p = (selectItem as PhysicalExamination);
                if (V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU && MessageBox.Show(eHCMSResources.Z0382_G1_BanCoChacXoaDongNayKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    DeletePhysicalExamination(2, p.PhyExamID, p.CommonMedRecID);
                }
                else if (V_RegistrationType == (long)AllLookupValues.RegistrationType.NOI_TRU && MessageBox.Show(eHCMSResources.Z0382_G1_BanCoChacXoaDongNayKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    DeletePhysicalExamination_InPT(p.PhyExamID, p.CommonMedRecID);
                }
            }
            //▲====== #002
            isEdit = false;
        }

        private bool isEdit = false;
        public void lnkEditClick(RoutedEventArgs e)
        {
            //grdCommonRecord.IsReadOnly = false;
            //grdCommonRecord.EditRecord();
            //grdCommonRecord.BeginEdit();
            //DataTemplate row = this.grdCommonRecord.RowDetailsTemplate;
            //((PhysicalExamination)grdCommonRecord.SelectedItem).isEdit = false;
            isEdit = true;
            selectPhyExamList = ObjectCopier.DeepCopy((PhysicalExamination)grdCommonRecord.SelectedItem);
            MedicalInstructionDateContent.DateTime = selectPhyExamList.RecordDate;
        }

        public void grdCommonRecord_CurrentCellChanged(object sender, EventArgs e)
        {
            if ((PhysicalExamination)grdCommonRecord.SelectedItem != null)
            {
                if (grdCommonRecord.isLastRow())
                {
                    ((PhysicalExamination)grdCommonRecord.SelectedItem).isEdit = false;
                    ((PhysicalExamination)grdCommonRecord.SelectedItem).isDeleted = false;
                }
                else
                {
                    //((PhysicalExamination)((object[])(grdCommonRecord.ItemsSource))[grdCommonRecord.TotalItem()] as PhysicalExamination).isEdit = true;
                    ((ObservableCollection<PhysicalExamination>)grdCommonRecord.ItemsSource)[grdCommonRecord.TotalItem() - 1].isEdit = true;
                }
            }
        }

        public void grdCommonRecord_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        private ObservableCollection<Lookup> _refSmokeStatus;
        public ObservableCollection<Lookup> refSmokeStatus
        {
            get { return _refSmokeStatus; }
            set
            {
                _refSmokeStatus = value;
                NotifyOfPropertyChange(() => refSmokeStatus);
            }
        }

        private ObservableCollection<Lookup> _refAlcoholDrinkingStatus;
        public ObservableCollection<Lookup> refAlcoholDrinkingStatus
        {
            get
            {
                return _refAlcoholDrinkingStatus;
            }
            set
            {
                if (_refAlcoholDrinkingStatus == value)
                    return;
                _refAlcoholDrinkingStatus = value;
                NotifyOfPropertyChange(() => refAlcoholDrinkingStatus);
            }
        }
        #region method

        public void cboLookupAlcoholStatusLoaded(object sender)
        {
            AxComboBox cbo = sender as AxComboBox;
            if (cbo != null)
            {
                //Cai nay chua chinh xac.
                cbo.ItemsSource = refAlcoholDrinkingStatus;
            }
        }

        public void cboLookupSmokeStatusLoaded(object sender)
        {
            AxComboBox cbo = sender as AxComboBox;
            if (cbo != null)
            {
                //Cai nay chua chinh xac.
                cbo.ItemsSource = refSmokeStatus;
            }
        }

        private void LoadPhyExamByPtID(long patientID, long PtRegistrationID = 0)
        {
            if (V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                GetPhyExamByPtID(patientID);
            }
            else
            {
                GetPhyExamByPtID_InPT(PtRegistrationID);
            }
        }

        private void GetPhyExamByPtID(long patientID)
        {
            if (IsShowSummaryContent)
                this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            else
                this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {

                    using (var serviceFactory = new SummaryServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetPhyExamByPtID(patientID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                Globals.curPhysicalExamination = new PhysicalExamination();
                                PtPhyExamList = new ObservableCollection<PhysicalExamination>();
                                var items = contract.EndGetPhyExamByPtID(asyncResult);
                                if (items != null && items.Count > 0)
                                {
                                    foreach (var tp in items)
                                    {
                                        PtPhyExamList.Add(tp);
                                    }
                                    Globals.curPhysicalExamination = items.FirstOrDefault();
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                if (IsShowSummaryContent)
                                    this.HideBusyIndicator();
                                else
                                    this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    if (IsShowSummaryContent)
                        this.ShowBusyIndicator();
                    else
                        this.DlgShowBusyIndicator();
                }
            });

            t.Start();
        }

        private void GetPhyExamByPtID_InPT(long PtRegistrationID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new SummaryServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetPhyExamByPtID_InPT(PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                Globals.curPhysicalExamination = new PhysicalExamination();
                                PtPhyExamList = new ObservableCollection<PhysicalExamination>();
                                var items = contract.EndGetPhyExamByPtID_InPT(asyncResult);
                                if (items != null && items.Count > 0)
                                {
                                    PtPhyExamList = new ObservableCollection<PhysicalExamination>();
                                    foreach (var tp in items)
                                    {
                                        PtPhyExamList.Add(tp);
                                    }
                                    Globals.curPhysicalExamination = items.FirstOrDefault();
                                    selectPhyExamList = items.FirstOrDefault().DeepCopy();
                                    selectPhyExamList.PhyExamID = 0;
                                    MedicalInstructionDateContent.DateTime = new DateTime(Globals.GetCurServerDateTime().Year, Globals.GetCurServerDateTime().Month, Globals.GetCurServerDateTime().Day, Globals.GetCurServerDateTime().Hour, Globals.GetCurServerDateTime().Minute, 0);
                                    selectPhyExamList.RecordDate = MedicalInstructionDateContent.DateTime;
                                    Registration_DataStorage.PtPhyExamList = items.ToObservableCollection();
                                }
                                else
                                {
                                    selectPhyExamList = new PhysicalExamination();
                                }
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

        private void UpdatePhyExam_V2(PhysicalExamination Exam, long? StaffID, long? ExamID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new SummaryServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdatePhysicalExamination_V2(Exam, StaffID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            bool items = contract.EndUpdatePhysicalExamination_V2(asyncResult);
                            if (items)
                            {
                                Globals.ShowMessage(eHCMSResources.A0296_G1_Msg_InfoSuaOK, "");
                                LoadPhyExamByPtID(Registration_DataStorage.CurrentPatient.PatientID);
                            }
                            else
                            {
                                Globals.ShowMessage(eHCMSResources.Z0387_G1_ChSuaBiLoi, "");
                            }
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
            });

            t.Start();
        }

        private void UpdatePhyExam_InPT(PhysicalExamination Exam, long? StaffID, long? ExamID)
        {
            Exam.V_RegistrationType = V_RegistrationType;
            if (Exam.CommonMedicalRecord == null)
            {
                Exam.CommonMedicalRecord = new CommonMedicalRecord();
            }
            Exam.CommonMedicalRecord.PatientID = Registration_DataStorage.CurrentPatient.PatientID;
            Exam.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
            //▼====#006
            Exam.RecordDate = MedicalInstructionDateContent.DateTime.GetValueOrDefault(new DateTime(Globals.GetCurServerDateTime().Year, Globals.GetCurServerDateTime().Month, Globals.GetCurServerDateTime().Day, Globals.GetCurServerDateTime().Hour, Globals.GetCurServerDateTime().Minute, 0));
            //▲====#006
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new SummaryServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdatePhysicalExamination_InPT(Exam, StaffID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool items = contract.EndUpdatePhysicalExamination_InPT(asyncResult);
                                if (items)
                                {
                                    GlobalsNAV.ShowMessagePopup(eHCMSResources.A0296_G1_Msg_InfoSuaOK);
                                    LoadPhyExamByPtID(Registration_DataStorage.CurrentPatient.PatientID, Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID);
                                    selectPhyExamList = new PhysicalExamination();
                                }
                                else
                                {
                                    Globals.ShowMessage(eHCMSResources.Z0387_G1_ChSuaBiLoi, "");
                                }
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

        private void AddNewPhyExam_V2(PhysicalExamination physicalExamination)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new SummaryServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginAddNewPhysicalExamination_V2(physicalExamination, Globals.LoggedUserAccount.Staff.StaffID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool items = contract.EndAddNewPhysicalExamination_V2(asyncResult);
                                if (items)
                                {
                                    Globals.ShowMessage(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK, "");
                                    LoadPhyExamByPtID(Registration_DataStorage.CurrentPatient.PatientID);
                                }
                                else
                                {
                                    Globals.ShowMessage(eHCMSResources.A1026_G1_Msg_InfoThemFail, "");
                                }
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

        private void AddNewPhyExam_InPT(PhysicalExamination physicalExamination)
        {
            physicalExamination.V_RegistrationType = V_RegistrationType;
            if (physicalExamination.CommonMedicalRecord == null)
            {
                physicalExamination.CommonMedicalRecord = new CommonMedicalRecord();
            }
            physicalExamination.CommonMedicalRecord.PatientID = Registration_DataStorage.CurrentPatient.PatientID;
            physicalExamination.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
            //▼====#006
            physicalExamination.RecordDate = MedicalInstructionDateContent.DateTime.GetValueOrDefault(new DateTime(Globals.GetCurServerDateTime().Year, Globals.GetCurServerDateTime().Month, Globals.GetCurServerDateTime().Day, Globals.GetCurServerDateTime().Hour, Globals.GetCurServerDateTime().Minute, 0));
            //▲====#006
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new SummaryServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginAddNewPhysicalExamination_InPT(physicalExamination, Globals.LoggedUserAccount.Staff.StaffID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool items = contract.EndAddNewPhysicalExamination_InPT(asyncResult);
                                if (items)
                                {
                                    GlobalsNAV.ShowMessagePopup(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK);
                                    LoadPhyExamByPtID(Registration_DataStorage.CurrentPatient.PatientID, Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID);
                                    selectPhyExamList = new PhysicalExamination();
                                }
                                else
                                {
                                    Globals.ShowMessage(eHCMSResources.A1026_G1_Msg_InfoThemFail, "");
                                }
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
        //▲====== #001

        private void DeletePhysicalExamination(long? StaffID, long? PhyExamID, long? CommonMedRecID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new SummaryServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeletePhysicalExamination(StaffID, PhyExamID, CommonMedRecID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool items = contract.EndDeletePhysicalExamination(asyncResult);
                                if (items)
                                {
                                    Globals.ShowMessage(eHCMSResources.K0537_G1_XoaOk, "");
                                    LoadPhyExamByPtID(Registration_DataStorage.CurrentPatient.PatientID);
                                }
                                else
                                {
                                    Globals.ShowMessage(eHCMSResources.K0484_G1_XoaFail, "");
                                }
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

        private void DeletePhysicalExamination_InPT(long? PhyExamID, long? CommonMedRecID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new SummaryServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeletePhysicalExamination_InPT(PhyExamID, CommonMedRecID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool items = contract.EndDeletePhysicalExamination_InPT(asyncResult);
                                if (items)
                                {                                    
                                    GlobalsNAV.ShowMessagePopup(eHCMSResources.K0537_G1_XoaOk);
                                    LoadPhyExamByPtID(Registration_DataStorage.CurrentPatient.PatientID, Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID);
                                }
                                else
                                {
                                    Globals.ShowMessage(eHCMSResources.K0484_G1_XoaFail, "");
                                }
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

        public void LoadSmokeStatus()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0526_G1_DangLayDSTrangThaiSmoke);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllLookupValuesByType(LookupValues.SMOKE_STATUS,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);
                                    if (allItems != null)
                                    {
                                        refSmokeStatus.Clear();
                                        foreach (var tp in allItems)
                                        {
                                            refSmokeStatus.Add(tp);
                                        }
                                    }
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

        public void LoadAlcoholDrinkingStatus()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllLookupValuesByType(LookupValues.ALCOHOL_DRINKING_STATUS,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);
                                    if (allItems != null)
                                    {
                                        refAlcoholDrinkingStatus.Clear();
                                        foreach (var tp in allItems)
                                        {
                                            refAlcoholDrinkingStatus.Add(tp);
                                        }
                                    }
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
            }
        }

        public void cboCdoAn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AxComboBox cboCdoAn = sender as AxComboBox;
            if (cboCdoAn != null)
            {
                if (selectPhyExamList != null)
                {
                    if (cboCdoAn.SelectedIndex >= 0)
                    {
                        selectPhyExamList.Diet = cboCdoAn.SelectedValue.ToString();
                    }
                }
            }
        }

        public void cboCdAn_TextChanged(object sender)
        {
            AxComboBox cboCdoAn = sender as AxComboBox;
            if (cboCdoAn != null && selectPhyExamList != null)
            {
                selectPhyExamList.Diet = cboCdoAn.Text.ToString();
            }
        }
        //▼====: #005
        private ObservableCollection<Lookup> _OxygenBreathing;
        public ObservableCollection<Lookup> OxygenBreathing
        {
            get
            {
                return _OxygenBreathing;
            }
            set
            {
                if (_OxygenBreathing == value)
                    return;
                _OxygenBreathing = value;
                NotifyOfPropertyChange(() => OxygenBreathing);
            }
        }
        private ObservableCollection<Lookup> _ConsciousnessLevel;
        public ObservableCollection<Lookup> ConsciousnessLevel
        {
            get
            {
                return _ConsciousnessLevel;
            }
            set
            {
                if (_ConsciousnessLevel == value)
                    return;
                _ConsciousnessLevel = value;
                NotifyOfPropertyChange(() => ConsciousnessLevel);
            }
        }
        private ObservableCollection<Lookup> _PainLevel;
        public ObservableCollection<Lookup> PainLevel
        {
            get
            {
                return _PainLevel;
            }
            set
            {
                if (_PainLevel == value)
                    return;
                _PainLevel = value;
                NotifyOfPropertyChange(() => PainLevel);
            }
        }
        private Lookup _SelectedOxygenBreathing;
        public Lookup SelectedOxygenBreathing
        {
            get
            {
                return _SelectedOxygenBreathing;
            }
            set
            {
                if (_SelectedOxygenBreathing == value)
                    return;
                _SelectedOxygenBreathing = value;
                NotifyOfPropertyChange(() => SelectedOxygenBreathing);
            }
        }
        private Lookup _SelectedConsciousnessLevel;
        public Lookup SelectedConsciousnessLevel
        {
            get
            {
                return _SelectedConsciousnessLevel;
            }
            set
            {
                if (_SelectedConsciousnessLevel == value)
                    return;
                _SelectedConsciousnessLevel = value;
                NotifyOfPropertyChange(() => SelectedConsciousnessLevel);
            }
        }
        private Lookup _SelectedPainLevel;
        public Lookup SelectedPainLevel
        {
            get
            {
                return _SelectedPainLevel;
            }
            set
            {
                if (_SelectedPainLevel == value)
                    return;
                _SelectedPainLevel = value;
                NotifyOfPropertyChange(() => SelectedPainLevel);
            }
        }
        private void LoadLookupValues()
        {
            LoadOxygenBreathing();
            LoadConsciousnessLevel();
            LoadPainLevel();
        }
        public void LoadOxygenBreathing()
        {
            OxygenBreathing = new ObservableCollection<Lookup>();
            OxygenBreathing.Add(new Lookup { LookupID = -1, ObjectValue = "--Chọn " + eHCMSResources.Z3249_G1_ThoOxy + "--" });
            OxygenBreathing.Add(new Lookup { LookupID = 1, ObjectValue = "Có" });
            OxygenBreathing.Add(new Lookup { LookupID = 0, ObjectValue = "Không" });
            SelectedOxygenBreathing = OxygenBreathing.FirstOrDefault();
        }
        public void LoadConsciousnessLevel()
        {
            ConsciousnessLevel = new ObservableCollection<Lookup>();
            ObservableCollection<Lookup> tempLookup = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_ConsciousnessLevel).ToObservableCollection();
            ConsciousnessLevel.Add(new Lookup {LookupID = -1 ,ObjectValue = "--Chọn "+eHCMSResources.Z3250_G1_MucDoYThuc+"--" });
            if(tempLookup != null && tempLookup.Count > 0)
            {
                foreach (var item in tempLookup)
                {
                    ConsciousnessLevel.Add(item);
                }
            }
            SelectedConsciousnessLevel = ConsciousnessLevel.FirstOrDefault();
        }
        public void LoadPainLevel()
        {
            PainLevel = new ObservableCollection<Lookup>();
            ObservableCollection<Lookup> tempLookup = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_PainLevel).ToObservableCollection();
            PainLevel.Add(new Lookup { LookupID = -1, ObjectValue = "--Chọn " + eHCMSResources.Z3251_G1_MucDoDau + "--" });
            if (tempLookup != null && tempLookup.Count > 0)
            {
                foreach (var item in tempLookup)
                {
                    PainLevel.Add(item);
                }
            }
            SelectedPainLevel = PainLevel.FirstOrDefault() ;
        }
        public void cboThoOxy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedOxygenBreathing != null && selectPhyExamList != null)
            {
                switch (SelectedOxygenBreathing.LookupID)
                {
                    case -1:
                        selectPhyExamList.OxygenBreathing = null;
                        break;
                    case 0:
                        selectPhyExamList.OxygenBreathing = false;
                        break;
                    case 1:
                        selectPhyExamList.OxygenBreathing = true;
                        break;
                }
            }
        }
        public void cboMucDoYThuc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedConsciousnessLevel != null && selectPhyExamList != null)
            {
                selectPhyExamList.V_ConsciousnessLevel = SelectedConsciousnessLevel.LookupID > 0 ? (int?)SelectedConsciousnessLevel.LookupID : null;
            }
        }
        public void cboMucDoDau_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedPainLevel != null && selectPhyExamList != null)
            {
                selectPhyExamList.V_PainLevel = SelectedPainLevel.LookupID > 0 ? (int?)SelectedPainLevel.LookupID : null;
            }
        }
        private void GetDataComboBox()
        {
            GetOxygenBreathing();
            GetConsciousnessLevel();
            GetPainLevel();
        }
        private void GetOxygenBreathing()
        {
            if (selectPhyExamList != null && OxygenBreathing != null)
            {
                switch (selectPhyExamList.OxygenBreathing)
                {
                    case null:
                        SelectedOxygenBreathing = OxygenBreathing.Where(x=>x.LookupID == -1).FirstOrDefault();
                        break;
                    case false:
                        SelectedOxygenBreathing = OxygenBreathing.Where(x => x.LookupID == 0).FirstOrDefault();
                        break;
                    case true:
                        SelectedOxygenBreathing = OxygenBreathing.Where(x => x.LookupID == 1).FirstOrDefault();
                        break;
                }
            }
        }
        private void GetConsciousnessLevel()
        {
            if (selectPhyExamList != null && ConsciousnessLevel != null)
            {
                SelectedConsciousnessLevel = selectPhyExamList.V_ConsciousnessLevel != null ?
                    ConsciousnessLevel.Where(x => x.LookupID == (int)selectPhyExamList.V_ConsciousnessLevel).FirstOrDefault() :
                    ConsciousnessLevel.Where(x => x.LookupID == -1).FirstOrDefault();
            }
        }
        private void GetPainLevel()
        {
            if (selectPhyExamList != null && PainLevel != null)
            {
                SelectedPainLevel = selectPhyExamList.V_PainLevel != null ?
                    PainLevel.Where(x => x.LookupID == (int)selectPhyExamList.V_PainLevel).FirstOrDefault() :
                    PainLevel.Where(x => x.LookupID == -1).FirstOrDefault();
            }
        }
        //▲====: #005
        public Button lnkPrint { get; set; }
        public void lnkPrint_Loaded(object sender)
        {
            lnkPrint = sender as Button;
            lnkPrint.Visibility = Globals.convertVisibility(mTongQuat_ChinhSuaThongTin);
        }
        public void lnkPrintClick(RoutedEventArgs e)
        {
            PhysicalExamination physicalExamination = ObjectCopier.DeepCopy((PhysicalExamination)grdCommonRecord.SelectedItem);
            if (Registration_DataStorage != null
                && Registration_DataStorage.CurrentPatient != null
                && physicalExamination != null )
            {
                if (Registration_DataStorage.CurrentPatient.Age < 16)
                {
                    MessageBox.Show("Thang điểm News chỉ áp dụng cho đối tượng trên 15 tuổi. Liên hệ phòng Điều dưỡng để biết thêm chi tiết");
                    return;
                }
                if (physicalExamination.Pulse == null)
                {
                    MessageBox.Show("Chưa nhập (Nhịp tim) nên không xem được thang điểm News");
                    return;
                }
                if (physicalExamination.SystolicPressure == null)
                {
                    MessageBox.Show("Chưa nhập (Huyết áp tối đa) nên không xem được thang điểm News");
                    return;
                }
                if (physicalExamination.RespiratoryRate == null)
                {
                    MessageBox.Show("Chưa nhập (Nhịp thở) nên không xem được thang điểm News");
                    return;
                }
                if (physicalExamination.SpO2 == null)
                {
                    MessageBox.Show("Chưa nhập (SpO2) nên không xem được thang điểm News");
                    return;
                }
                if (physicalExamination.OxygenBreathing == null)
                {
                    MessageBox.Show("Chưa nhập (Thở oxy hỗ trợ) nên không xem được thang điểm News");
                    return;
                }
                if (physicalExamination.Temperature == null)
                {
                    MessageBox.Show("Chưa nhập (Nhiệt độ) nên không xem được thang điểm News");
                    return;
                }
                if (physicalExamination.V_ConsciousnessLevel == null)
                {
                    MessageBox.Show("Chưa nhập (Mức độ ý thức) nên không xem được thang điểm News");
                    return;
                }
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                {
                    proAlloc.eItem = ReportName.BaoCaoThangDiemCanhBaoSom;
                    proAlloc.ID = physicalExamination.PhyExamID;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
            }
        
           
        }

        //▼====: #006
        private IMinHourDateControl _MedicalInstructionDateContent;
        public IMinHourDateControl MedicalInstructionDateContent
        {
            get { return _MedicalInstructionDateContent; }
            set
            {
                _MedicalInstructionDateContent = value;
                NotifyOfPropertyChange(() => MedicalInstructionDateContent);
            }
        }
        //▲====: #006
    }
}
