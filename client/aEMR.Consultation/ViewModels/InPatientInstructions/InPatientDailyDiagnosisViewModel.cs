using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using DataEntities;
using eHCMSLanguage;
using System.Linq;
using aEMR.Common.BaseModel;
using System.Threading;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.Common.Collections;
using aEMR.Common;
using aEMR.Controls;
using System.Collections.Generic;
using aEMR.CommonTasks;
using Castle.Windsor;
using System.Windows.Input;
using System.Windows.Media;
/*
 * 20190727 #001 TTM:   BM 0012992: Sửa lỗi ICD khi sử dụng Arrow Down và Enter sẽ bị mất dữ liệDiagnosisTreatmentContent. Bổ sung tự động check bệnh chính ở ICD đầu tiên chọn. Bổ sung thêm khi chọn ICD tự động nhảy chẩn đoán.
 * 20190817 #002 TTM:   BM        : Bắt buộc nhập diễn tiến bệnh mới cho lưu thông tin y lệnh.
 * 20190817 #003 TTM:   BM 0013181: Load chẩn đoán cuối cùng của bệnh nhân khi tạo mới y lệnh. Cấu hình việc tạo mới y lệnh chừa lại các trường nào.
 * 20190820 #004 TTM:   BM 0013195: Y LỆNH: lỗi không load ICD10 và Chẩn đoán lần đầu nhập viện nếu tìm đăng ký lần 1.
 * 20190929 #005 TTM:   BM 0017381: Bổ sung huyết áp dưới cho y lệnh, bổ sung chức năng cập nhật dấu hiệu sinh tồn cho y lệnh.
 * 20191005 #006 TBL:   BM 0017404: Tạo mới dựa trên cũ y lệnh (popup) 
 * 20191016 #007 TTM:   BM 0018459: [Dịch truyền] Sửa lỗi chạy đua khi lưu và đọc dữ liệu cho y lệnh. (Chuyển tất cả dữ liệu sang InPatientInstructionViewModel để thực hiện
 *                                  lưu và đọc dữ liệDiagnosisTreatmentContent.
 * 20200413 #008 TBL:   BM 0030110: Load chẩn đoán và danh sách ICD10 cùng lúc thay vì load chẩn đoán xong rồi mới load danh sách ICD10                             
 * 20210610 #009 TNHX:  331: Dựa vào mạch, huyết áp của "y lệnh theo dõi sinh hiệu" của y lệnh gần nhất để biết có cần nhập lại DHST không
 * 20210729 #010 TNHX: Thêm điều kiện bắt buộc nhập DHST trong ngày + lấy thông tin gần nhất để đỡ nhập lại
 * 20210907 #011 TNHX: Chỉnh lại kiểm tra giá trị tối đa cho DHST + cảnh báo
 * 20220105 #012 TNHX: mặc định tạo mới lấy thông tin DHST cũ cho bsi từ DHST của điều dưỡng. Kiểm tra bắt DD nhập DHST trước khi tạo mới
 * 20220725 #013 DatTB: Thêm thông tin nhịp thở RespiratoryRate vào DiagnosisTreatment_InPt.
 */
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IInPatientDailyDiagnosis)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class InPatientDailyDiagnosisViewModel : ViewModelBase, IInPatientDailyDiagnosis
    {
        #region Variables
        //▼====: #009
        public DateTime? MedicalInstructionDate { get; set; }
        private InPatientInstruction _gInPatientInstruction = null;
        public InPatientInstruction gInPatientInstruction
        {
            get
            {
                return _gInPatientInstruction;
            }
            set
            {
                _gInPatientInstruction = value;
                NotifyOfPropertyChange(() => gInPatientInstruction);
            }
        }
        //▲====: #009
        private ObservableCollection<DiagnosisIcd10Items> _ICD10List = new ObservableCollection<DiagnosisIcd10Items>();
        public ObservableCollection<DiagnosisIcd10Items> ICD10List
        {
            get
            {
                return _ICD10List;
            }
            set
            {
                if (_ICD10List != value)
                {
                    _ICD10List = value;
                }
                NotifyOfPropertyChange(() => ICD10List);
            }
        }
        private ObservableCollection<DiagnosisIcd10Items> _refIDC10ListCopy;
        public ObservableCollection<DiagnosisIcd10Items> refIDC10ListCopy
        {
            get
            {
                return _refIDC10ListCopy;
            }
            set
            {
                if (_refIDC10ListCopy != value)
                {
                    _refIDC10ListCopy = value;
                    NotifyOfPropertyChange(() => refIDC10ListCopy);
                }
            }
        }
        private PagedSortableCollectionView<DiseasesReference> _refIDC10 = new PagedSortableCollectionView<DiseasesReference>();
        public PagedSortableCollectionView<DiseasesReference> refIDC10
        {
            get
            {
                return _refIDC10;
            }
            set
            {
                if (_refIDC10 != value)
                {
                    _refIDC10 = value;
                }
                NotifyOfPropertyChange(() => refIDC10);
            }
        }
        private ObservableCollection<Lookup> _DiagIcdStatusList;
        public ObservableCollection<Lookup> DiagIcdStatusList
        {
            get
            {
                return _DiagIcdStatusList;
            }
            set
            {
                if (_DiagIcdStatusList != value)
                {
                    _DiagIcdStatusList = value;
                    NotifyOfPropertyChange(() => DiagIcdStatusList);
                }
            }
        }
        private DiagnosisIcd10Items _refIDC10Item = new DiagnosisIcd10Items();
        public DiagnosisIcd10Items refIDC10Item
        {
            get
            {
                return _refIDC10Item;
            }
            set
            {
                if (_refIDC10Item != value)
                {
                    _refIDC10Item = value;
                }
                NotifyOfPropertyChange(() => refIDC10Item);
            }
        }
        //▼===== #004
        private DiagnosisTreatment _DiagnosisTreatmentContent = new DiagnosisTreatment();
        //▲===== #004
        public DiagnosisTreatment DiagnosisTreatmentContent
        {
            get
            {
                return _DiagnosisTreatmentContent;
            }
            set
            {
                _DiagnosisTreatmentContent = value;
                NotifyOfPropertyChange(() => DiagnosisTreatmentContent);
            }
        }
        private PatientRegistration _PatientRegistrationContent;
        public PatientRegistration PatientRegistrationContent
        {
            get
            {
                return _PatientRegistrationContent;
            }
            set
            {
                if (_PatientRegistrationContent != value)
                {
                    _PatientRegistrationContent = value;
                    NotifyOfPropertyChange(() => PatientRegistrationContent);
                }
            }
        }
        //AutoCompleteBox Auto;
        //AutoCompleteBox AutoName;
        AutoCompleteBox DiseasesName;
        AxDataGridNyICD10 grdConsultation { get; set; }
        private bool _IsRequireConfirmZ10 = true;
        public bool IsRequireConfirmZ10
        {
            get
            {
                return _IsRequireConfirmZ10;
            }
            set
            {
                _IsRequireConfirmZ10 = value;
                NotifyOfPropertyChange(() => IsRequireConfirmZ10);
            }
        }
        private bool _IsWaitingSaveAddNew;
        public bool IsWaitingSaveAddNew
        {
            get { return _IsWaitingSaveAddNew; }
            set
            {
                if (_IsWaitingSaveAddNew != value)
                {
                    _IsWaitingSaveAddNew = value;
                    NotifyOfPropertyChange(() => IsWaitingSaveAddNew);
                    NotifyWhenBusy();
                }
            }
        }
        private long _IntPtDiagDrInstructionID;
        public long IntPtDiagDrInstructionID
        {
            get
            {
                return _IntPtDiagDrInstructionID;
            }
            set
            {
                _IntPtDiagDrInstructionID = value;
                ICD10List = new ObservableCollection<DiagnosisIcd10Items>();
                //GetLatesDiagTrmtByPtID_InPt(_IntPtDiagDrInstructionID, (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_DAILY);
                NotifyOfPropertyChange(() => IntPtDiagDrInstructionID);
            }
        }
        MessageWarningShowDialogTask warningtask = null;
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
        #endregion

        #region Events
        [ImportingConstructor]
        public InPatientDailyDiagnosisViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            GetAllLookupValuesByType();
            refIDC10Code = new ObservableCollection<DiseasesReference>();
            refIDC10Name = new ObservableCollection<DiseasesReference>();
            GetShortHandDictionaries((long)Globals.LoggedUserAccount.StaffID);
        }
        //public void aucICD10_Populating(object sender, PopulatingEventArgs e)
        //{
        //    Auto = (AutoCompleteBox)sender;
        //    refIDC10.PageIndex = 0;
        //    LoadRefDiseases(e.Parameter, 0, 0, refIDC10.PageSize);
        //}
        //public void AxAutoComplete_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        //{
        //    Auto = (AutoCompleteBox)sender;
        //    if (refIDC10Item != null)
        //    {
        //        refIDC10Item.DiseasesReference = Auto.SelectedItem as DiseasesReference;
        //    }
        //}
        public void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ((ComboBox)sender).ItemsSource = DiagIcdStatusList;
            if (refIDC10Item != null && DiagIcdStatusList != null)
            {
                if (refIDC10Item.LookupStatus == null)
                {
                    ((ComboBox)sender).SelectedIndex = 0;
                }
                else
                {
                    ((ComboBox)sender).SelectedItem = refIDC10Item.LookupStatus;
                }
            }
        }
        public void DiseaseName_Loaded(object sender, RoutedEventArgs e)
        {
            DiseasesName = (AutoCompleteBox)sender;
        }
        //public void aucICD10Name_Populating(object sender, PopulatingEventArgs e)
        //{
        //    AutoName = (AutoCompleteBox)sender;
        //    LoadRefDiseases(e.Parameter, 1, 0, refIDC10.PageSize);
        //}
        //public void DiseaseName_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        //{
        //    DiseasesName = (AutoCompleteBox)sender;
        //    refIDC10Item.DiseasesReference = ((AutoCompleteBox)sender).SelectedItem as DiseasesReference;
        //}
        public void grdConsultation_Loaded(object sender, RoutedEventArgs e)
        {
            grdConsultation = sender as AxDataGridNyICD10;
        }
        public void AxDataGridNy_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            DiagnosisIcd10Items item = e.Row.DataContext as DiagnosisIcd10Items;
            if (refIDC10Item != null && refIDC10Item.DiseasesReference != null)
            {
                if (CheckExists(refIDC10Item, true))
                {
                    if (e.Row.GetIndex() == (ICD10List.Count - 1) && e.EditAction == DataGridEditAction.Commit)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(() => AddBlankRow());
                    }
                }
            }
        }
        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (refIDC10Item == null || refIDC10Item.DiseasesReference == null)
            {
                MessageBox.Show(eHCMSResources.Z0405_G1_KgTheXoaDongRong);
                return;
            }
            int nSelIndex = grdConsultation.SelectedIndex;
            if (nSelIndex >= ICD10List.Count - 1)
            {
                MessageBox.Show(eHCMSResources.Z0405_G1_KgTheXoaDongRong);
                return;
            }
            var item = ICD10List[nSelIndex];
            if (item != null && item.ICD10Code != null && item.ICD10Code != "")
            {
                if (MessageBox.Show(eHCMSResources.A0202_G1_Msg_ConfXoaMaICD10, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    ICD10List.Remove(ICD10List[nSelIndex]);
                }
            }
        }
        private bool Equal(DiagnosisIcd10Items a, DiagnosisIcd10Items b)
        {
            return a.DiagIcd10ItemID == b.DiagIcd10ItemID
                && a.DiagnosisIcd10ListID == b.DiagnosisIcd10ListID
                && a.ICD10Code == b.ICD10Code
                && a.IsMain == b.IsMain
                && a.IsCongenital == b.IsCongenital
                && (a.LookupStatus != null && b.LookupStatus != null
                    && a.LookupStatus.LookupID == b.LookupStatus.LookupID);
        }
        private long Compare2Object()
        {
            long ListID = 0;
            ObservableCollection<DiagnosisIcd10Items> temp = ICD10List.Where(x => x.DiseasesReference != null).ToObservableCollection();
            if (refIDC10ListCopy != null && refIDC10ListCopy.Count > 0 && refIDC10ListCopy.Count == temp.Count)
            {
                int icount = 0;
                for (int i = 0; i < refIDC10ListCopy.Count; i++)
                {
                    for (int j = 0; j < temp.Count; j++)
                    {
                        if (Equal(refIDC10ListCopy[i], ICD10List[j]))
                        {
                            icount++;
                        }
                    }
                }
                if (icount == refIDC10ListCopy.Count)
                {
                    ListID = refIDC10ListCopy.FirstOrDefault().DiagnosisIcd10ListID;
                    return ListID;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
        private bool CheckCreateDiagnosis()
        {
            InPatientAdmDisDetails admission = Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo;
            if (admission.DischargeDate != null)
            {
                MessageBox.Show(eHCMSResources.Z0406_G1_BNDaXVKgTheCNhatCDoan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            if (Globals.IsLockRegistration(Registration_DataStorage.CurrentPatientRegistration.RegLockFlag, eHCMSResources.Z0407_G1_CNhatCDoan))
            {
                return false;
            }
            return true;
        }
        private bool CheckDepartment()
        {
            if (DiagnosisTreatmentContent == null || DiagnosisTreatmentContent.Department == null || DiagnosisTreatmentContent.Department.DeptID <= 0)
            {
                MessageBox.Show(eHCMSResources.Z0493_G1_HayChonKhoa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            InPatientAdmDisDetails admission = Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo;
            if (DiagnosisTreatmentContent.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_IN)
            {
                if (DiagnosisTreatmentContent.Department.DeptID != admission.Department.DeptID)
                {
                    MessageBox.Show(string.Format("{0} ", eHCMSResources.A0284_G1_Msg_InfoChonKhoaNpVien) + admission.Department.DeptName + string.Format(" {0}", eHCMSResources.A0285_G1_Msg_InfoViBNNpVienVaoKhoaNay), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
            }
            else if (DiagnosisTreatmentContent.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS)
            {
                int CountInDept = 0;
                if (admission.InPatientDeptDetails != null && admission.InPatientDeptDetails.Count > 0)
                {
                    CountInDept = admission.InPatientDeptDetails.Where(x => x.DeptLocation.DeptID == DiagnosisTreatmentContent.Department.DeptID).Count();
                }
                if (CountInDept <= 0)
                {
                    MessageBox.Show(eHCMSResources.A0216_G1_Msg_InfoBNChuaNhapVaoKhoaBanChon + " (" + DiagnosisTreatmentContent.Department.DeptName + ") ! ", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
            }
            else if (DiagnosisTreatmentContent.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTDEPT)
            {
                if (admission == null || admission.InPatientDeptDetails == null || admission.InPatientDeptDetails.Count() <= 0)
                {
                    return false;
                }
                if (!admission.InPatientDeptDetails.Any(x => x.DeptLocation.DeptID == DiagnosisTreatmentContent.Department.DeptID))
                {
                    MessageBox.Show(eHCMSResources.A0216_G1_Msg_InfoBNChuaNhapVaoKhoaBanChon + " (" + DiagnosisTreatmentContent.Department.DeptName + ") ! ", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
                if (DiagnosisTreatmentContent.DTItemID <= 0 && !admission.InPatientDeptDetails.Any(x => x.DeptLocation.DeptID == DiagnosisTreatmentContent.Department.DeptID && x.CompletedRequiredFromDate == null))
                {
                    MessageBox.Show(eHCMSResources.A0222_G1_Msg_InfoBNCoDuCDXK + " " + DiagnosisTreatmentContent.Department.DeptName + string.Format("\n {0}", eHCMSResources.Z0408_G1_KgTheTaoThemCDoanXK), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
            }
            return true;
        }
        private bool CheckEmptyFields()
        {
            string strWarningMsg = "";

            if (DiagnosisTreatmentContent.DiagnosisFinal == null || DiagnosisTreatmentContent.DiagnosisFinal.Trim() == "")
            {
                strWarningMsg += string.Format("{0} - ", eHCMSResources.K1775_G1_CDoanXDinh2);
            }
            if (strWarningMsg != "")
            {
                MessageBox.Show(eHCMSResources.A0201_G1_Msg_InfoYCNhapSth + ": " + strWarningMsg);
                return false;
            }
            //▼===== #002
            if (string.IsNullOrEmpty(DiagnosisTreatmentContent.OrientedTreatment) || DiagnosisTreatmentContent.OrientedTreatment.Trim() == "")
            {
                MessageBox.Show(eHCMSResources.Z2800_G1_VuiLongNhapDienTienBenh);
                return false;
            }
            //▲===== #002
            return true;
        }
        private bool CheckedIsMain()
        {
            ObservableCollection<DiagnosisIcd10Items> temp = ICD10List.Where(x => x.DiseasesReference != null).ToObservableCollection();
            if (temp != null && temp.Count > 0)
            {
                int bcount = 0;
                for (int i = 0; i < temp.Count; i++)
                {
                    if (temp[i].IsMain)
                    {
                        bcount++;
                    }
                }
                if (bcount == 0)
                {
                    Globals.ShowMessage(eHCMSResources.Z0509_G1_PhaiChonBenhChinh, eHCMSResources.G0442_G1_TBao);
                    return false;
                }
                else if (bcount == 1)
                {
                    return true;
                }
                else
                {
                    Globals.ShowMessage(eHCMSResources.Z0510_G1_I, eHCMSResources.G0442_G1_TBao);
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
        private bool NeedICD10()
        {
            if (Globals.ServerConfigSection.Hospitals.NeedICD10 > 0)
            {
                if (ICD10List != null)
                {
                    var temp = ICD10List.Where(x => x.DiseasesReference != null);
                    if (temp == null || temp.Count() == 0)
                    {
                        MessageBox.Show(eHCMSResources.A0199_G1_Msg_YCNhapICD10);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            { return true; }
        }
        public bool CheckValidNewDiagnosis(long DeptID = 0)
        {
            DiagnosisTreatmentContent.PatientServiceRecord = new PatientServiceRecord();
            DiagnosisTreatmentContent.PatientServiceRecord.PatientMedicalRecord = new PatientMedicalRecord();
            DiagnosisTreatmentContent.Department = new RefDepartment { DeptID = DeptID };
            DiagnosisTreatmentContent.V_DiagnosisType = 55004;
            if (!CheckEmptyFields())
            {
                return false;
            }
            long lBehaving = 0;
            try
            {
                lBehaving = DiagnosisTreatmentContent.PatientServiceRecord.V_Behaving.GetValueOrDefault();
            }
            catch
            {
                MessageBox.Show(eHCMSResources.A0367_G1_Msg_InfoChonTieuDe);
                return false;
            }
            long lPMRTemplateID = 0;
            try
            {
                lPMRTemplateID = DiagnosisTreatmentContent.MDRptTemplateID;
            }
            catch
            {
                MessageBox.Show(eHCMSResources.A0337_G1_Msg_InfoChonMauBAn);
                return false;
            }
            DiagnosisTreatmentContent.PatientServiceRecord.Staff = Globals.LoggedUserAccount.Staff;
            DiagnosisTreatmentContent.PatientServiceRecord.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
            
            if (CheckedIsMain() && NeedICD10())
            {
                if (Registration_DataStorage.CurrentPatientRegistrationDetail != null && Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID > 0)
                {
                    DiagnosisTreatmentContent.PtRegDetailID = Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID;
                }
                else
                {
                    DiagnosisTreatmentContent.PtRegDetailID = 0;
                }
                DiagnosisTreatmentContent.PatientServiceRecord.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
                DiagnosisTreatmentContent.PatientServiceRecord.PtRegistrationID = PatientRegistrationContent.PtRegistrationID;
                DiagnosisTreatmentContent.DeptLocationID = Globals.DeptLocation.DeptLocationID;
                DiagnosisTreatmentContent.PatientServiceRecord.V_RegistrationType = PatientRegistrationContent.V_RegistrationType;
                DiagnosisTreatmentContent.ICD10List = String.Join(",", from item in ICD10List
                                                                       where (!string.IsNullOrEmpty(item.ICD10Code) && item.DiseasesReference != null)
                                                                       select item.ICD10Code);
            }
            else return false;
            return true;
        }
        //▼===== #007
        //public IEnumerator<IResult> SaveNewDiagnosis(long aIntPtDiagDrInstructionID, long DeptID = 0, bool IsUpdate = false)
        //{
        //    DiagnosisTreatmentContent.IntPtDiagDrInstructionID = aIntPtDiagDrInstructionID;
        //    if (CheckValidNewDiagnosis(DeptID))
        //    {
        //        DiagnosisTreatmentContent.PatientServiceRecord.PatientMedicalRecord.PatientID = aIntPtDiagDrInstructionID;
        //        if (DiagnosisTreatmentContent.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTDEPT)
        //        {
        //            string confirmcontent = string.Format(eHCMSResources.K1006_G1_BanDangTaoCDXKhoaCho, DiagnosisTreatmentContent.Department.DeptName) + string.Format("\n{0}!", eHCMSResources.Z0538_G1_KgThayDoiKhoaSauKhiLuuCDoanXK);
        //            warningtask = new MessageWarningShowDialogTask(confirmcontent, eHCMSResources.Z0339_G1_TiepTucLuuCDoan);
        //            yield return warningtask;
        //            if (!warningtask.IsAccept)
        //            {
        //                yield break;
        //            }
        //        }
        //        if (!IsUpdate)
        //        {
        //            AddNewDiagTrmt();
        //        }
        //        else
        //        {
        //            UpdateDiagTrmt();
        //        }
        //    }
        //}
        //private void AddNewDiagTrmt()
        //{
        //    this.ShowBusyIndicator();
        //    IsWaitingSaveAddNew = true;
        //    long ID = Compare2Object();
        //    var t = new Thread(() =>
        //    {
        //        using (var serviceFactory = new ePMRsServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;
        //            contract.BeginAddDiagnosisTreatment_InPt(DiagnosisTreatmentContent, ID, ICD10List, 0, new List<DiagnosisICD9Items>(), Globals.DispatchCallback((asyncResult) =>
        //            {
        //                try
        //                {
        //                    List<InPatientDeptDetail> ReloadInPatientDeptDetails = new List<InPatientDeptDetail>();
        //                    if (contract.EndAddDiagnosisTreatment_InPt(out ReloadInPatientDeptDetails, asyncResult))
        //                    {
        //                        if (ICD10List != null)
        //                        {
        //                            ICD10List = ICD10List.Where(x => x.DiseasesReference != null).ToObservableCollection();
        //                        }
        //                        MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        //                    }
        //                    else
        //                    {
        //                        if (DiagnosisTreatmentContent.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_IN)
        //                        {
        //                            MessageBox.Show(eHCMSResources.Z0409_G1_DaCoCDoanNV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        //                        }
        //                        else if (DiagnosisTreatmentContent.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS)
        //                        {
        //                            MessageBox.Show(eHCMSResources.Z0410_G1_DaCoCDoanXV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        //                        }
        //                        else
        //                        {
        //                            MessageBox.Show(eHCMSResources.A0802_G1_Msg_InfoLuuCDFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        //                        }
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        //                }
        //                finally
        //                {
        //                    this.HideBusyIndicator();
        //                }
        //            }), null);
        //        }
        //    });
        //    t.Start();
        //}

        //private void UpdateDiagTrmt()
        //{
        //    this.ShowBusyIndicator();
        //    long ID = Compare2Object();
        //    var t = new Thread(() =>
        //    {
        //        using (var serviceFactory = new ePMRsServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;
        //            contract.BeginUpdateDiagnosisTreatment_InPt(DiagnosisTreatmentContent, ID, ICD10List, 0, new List<DiagnosisICD9Items>(), Globals.DispatchCallback((asyncResult) =>
        //            {
        //                try
        //                {
        //                    List<InPatientDeptDetail> ReloadInPatientDeptDetails = new List<InPatientDeptDetail>();
        //                    if (contract.EndUpdateDiagnosisTreatment_InPt(out ReloadInPatientDeptDetails, asyncResult))
        //                    {
        //                        if (ICD10List != null)
        //                        {
        //                            ICD10List = ICD10List.Where(x => x.DiseasesReference != null).ToObservableCollection();
        //                        }
        //                        MessageBox.Show(eHCMSResources.K2782_G1_DaCNhat);
        //                    }
        //                    else
        //                    {
        //                        if (DiagnosisTreatmentContent.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_IN)
        //                        {
        //                            MessageBox.Show(eHCMSResources.Z0403_G1_DaCoCDoanNV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        //                        }
        //                        else if (DiagnosisTreatmentContent.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS)
        //                        {
        //                            MessageBox.Show(eHCMSResources.Z0404_G1_DaCoCDoanXV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        //                        }
        //                        else
        //                        {
        //                            MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        //                        }
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        //                }
        //                finally
        //                {
        //                    this.HideBusyIndicator();
        //                }

        //            }), null);

        //        }

        //    });

        //    t.Start();
        //}
        //▲===== #007
        #endregion

        #region Functions
        private void GetAllLookupValuesByType()
        {
            ObservableCollection<Lookup> DiagICDSttLookupList = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_DiagIcdStatus).ToObservableCollection();
            if (DiagICDSttLookupList == null || DiagICDSttLookupList.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0750_G1_Msg_InfoKhTimThayStatusICD10, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            DiagIcdStatusList = DiagICDSttLookupList;
        }
        private void AddBlankRow()
        {
            if (ICD10List != null && ICD10List.LastOrDefault() != null && ICD10List.LastOrDefault().DiseasesReference == null)
            {
                return;
            }
            DiagnosisIcd10Items ite = new DiagnosisIcd10Items();
            ite.V_DiagIcdStatus = (long)AllLookupValues.V_DiagIcdStatus.DANGDIEUTRI;
            ite.LookupStatus = new Lookup();
            ite.LookupStatus.LookupID = (long)AllLookupValues.V_DiagIcdStatus.DANGDIEUTRI;
            ite.LookupStatus.ObjectValue = eHCMSResources.Z0540_G1_DangDTri.ToUpper();
            ICD10List.Add(ite);
        }
        //public void LoadRefDiseases(string name, byte type, int PageIndex, int PageSize)
        //{
        //    var t = new Thread(() =>
        //    {
        //        using (var serviceFactory = new CommonUtilsServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;
        //            contract.BeginSearchRefDiseases(name, PageIndex, PageSize, type, Globals.DispatchCallback((asyncResult) =>
        //            {

        //                try
        //                {
        //                    int Total = 10;
        //                    var results = contract.EndSearchRefDiseases(out Total, asyncResult);
        //                    refIDC10.Clear();
        //                    refIDC10.TotalItemCount = Total;
        //                    if (results != null)
        //                    {
        //                        foreach (DiseasesReference p in results)
        //                        {
        //                            refIDC10.Add(p);
        //                        }
        //                    }
        //                    if (type == 0)
        //                    {
        //                        Auto.ItemsSource = refIDC10;
        //                        Auto.PopulateComplete();
        //                    }
        //                    else
        //                    {
        //                        AutoName.ItemsSource = refIDC10;
        //                        AutoName.PopulateComplete();
        //                    }

        //                }
        //                catch (Exception ex)
        //                {
        //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                }
        //                finally
        //                {
        //                }
        //            }), null);
        //        }
        //    });
        //    t.Start();
        //}
        private void DiagnosisIcd10Items_Load_InPt(long DTItemID)
        {
            if (DTItemID <= 0)
            {
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDiagnosisIcd10Items_Load_InPt(DTItemID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDiagnosisIcd10Items_Load_InPt(asyncResult);
                            ICD10List = results.ToObservableCollection();
                            refIDC10ListCopy = ICD10List.DeepCopy();
                            AddBlankRow();
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
        public void GetLatesDiagTrmtByPtID_InPt(long PatientID, long IntPtDiagDrInstructionID, long? V_DiagnosisType, bool IsLoadNew = false)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetLatestDiagnosisTreatment_InPtByInstructionID(PatientID, V_DiagnosisType, IntPtDiagDrInstructionID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            DiagnosisTreatmentContent = contract.EndGetLatestDiagnosisTreatment_InPtByInstructionID(asyncResult);
                            if (DiagnosisTreatmentContent != null && DiagnosisTreatmentContent.DTItemID > 0)
                            {
                                if (IsLoadNew)
                                {
                                    DiagnosisTreatmentContent.Treatment = null;
                                    DiagnosisTreatmentContent.OrientedTreatment = null;
                                    DiagnosisTreatmentContent.DoctorComments = null;
                                    DiagnosisTreatmentContent.Pulse = 0;
                                    DiagnosisTreatmentContent.Temperature = 0;
                                    DiagnosisTreatmentContent.BloodPressure = 0;
                                    //▼===== #005
                                    DiagnosisTreatmentContent.LowerBloodPressure = 0;
                                    //▲===== #005
                                    DiagnosisTreatmentContent.SpO2 = 0;
                                }
                                DiagnosisIcd10Items_Load_InPt(DiagnosisTreatmentContent.DTItemID);
                            }
                            else
                            {
                                AddBlankRow();
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
        //▼===== #008
        public void GetLatestDiagnosisTreatment_InPtByInstructionID_V2(long PatientID, long IntPtDiagDrInstructionID, long? V_DiagnosisType, bool IsLoadNew = false)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetLatestDiagnosisTreatment_InPtByInstructionID_V2(PatientID, V_DiagnosisType, IntPtDiagDrInstructionID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            ObservableCollection<DiagnosisIcd10Items> icd10list = new ObservableCollection<DiagnosisIcd10Items>();
                            DiagnosisTreatmentContent = contract.EndGetLatestDiagnosisTreatment_InPtByInstructionID_V2(out icd10list, asyncResult);
                            if (DiagnosisTreatmentContent != null && DiagnosisTreatmentContent.DTItemID > 0)
                            {
                                if (IsLoadNew)
                                {
                                    DiagnosisTreatmentContent.Treatment = null;
                                    DiagnosisTreatmentContent.OrientedTreatment = null;
                                    DiagnosisTreatmentContent.OrientedTreatment = "Bệnh tỉnh, tiếp xúc tốt, da niêm hồng, tim đều, phổi trong, bụng mềm";
                                    DiagnosisTreatmentContent.DoctorComments = null;
                                    DiagnosisTreatmentContent.Pulse = 0;
                                    DiagnosisTreatmentContent.Temperature = 0;
                                    DiagnosisTreatmentContent.BloodPressure = 0;
                                    DiagnosisTreatmentContent.LowerBloodPressure = 0;
                                    DiagnosisTreatmentContent.SpO2 = 0;
                                }
                                if (icd10list.Count > 0)
                                {
                                    ICD10List = icd10list;
                                    refIDC10ListCopy = ICD10List.DeepCopy();
                                }
                                AddBlankRow();
                            }
                            else
                            {
                                AddBlankRow();
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
        //▲===== #008
        private bool CheckExists(DiagnosisIcd10Items Item, bool HasMessage = true)
        {
            int i = 0;
            if (Item.DiseasesReference == null)
            {
                return true;
            }
            foreach (DiagnosisIcd10Items p in ICD10List)
            {
                if (p.DiseasesReference != null)
                {
                    if (Item.DiseasesReference.ICD10Code == p.DiseasesReference.ICD10Code)
                    {
                        i++;
                    }
                }
            }
            if (i > 1)
            {
                Item.DiseasesReference = null;
                if (HasMessage)
                {
                    MessageBox.Show(eHCMSResources.A0810_G1_Msg_InfoMaICDDaTonTai);
                }
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        //▼====== #001
        #region AutoComplete mới cho ICD.
        public void grdConsultation_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (grdConsultation != null && grdConsultation.SelectedItem != null)
            {
                grdConsultation.BeginEdit();
            }
        }
        public void AxDataGridNy_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            ColumnIndex = e.Column.DisplayIndex;

            if (refIDC10Item != null)
            {
                DiseasesReferenceCopy = refIDC10Item.DiseasesReference.DeepCopy();
            }
            if (e.Column.DisplayIndex == 0)
            {
                IsCode = true;
            }
            else
            {
                IsCode = false;
            }
        }
        private DiseasesReference DiseasesReferenceCopy = null;

        bool IsCode = true;
        int ColumnIndex = 0;
        private string DiagnosisFinalOld = "";
        private string DiagnosisFinalNew = "";

        #endregion

        public void AxDataGridNyICD_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            DiagnosisIcd10Items item = e.Row.DataContext as DiagnosisIcd10Items;

            if (ColumnIndex == 0 || ColumnIndex == 1)
            {
                if (refIDC10Item.DiseasesReference == null)
                {
                    if (DiseasesReferenceCopy != null)
                    {
                        refIDC10Item.DiseasesReference = ObjectCopier.DeepCopy(DiseasesReferenceCopy);
                        if (CheckExists(refIDC10Item, false))
                        {
                            GetDiagTreatmentFinal(refIDC10Item.DiseasesReference);
                        }
                    }
                }
            }
            if (refIDC10Item != null && refIDC10Item.DiseasesReference != null)
            {
                if (CheckExists(refIDC10Item, false))
                {
                    if (e.Row.GetIndex() == (ICD10List.Count - 1) && e.EditAction == DataGridEditAction.Commit)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(() => AddBlankRow());
                    }
                }
            }
        }
        public void AxDataGridNyICD10_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DiagnosisIcd10Items item = ((DataGrid)sender).SelectedItem as DiagnosisIcd10Items;
            if (item != null && item.DiseasesReference != null)
            {
                DiseasesReferenceCopy = item.DiseasesReference;
                DiagnosisFinalNew = DiagnosisFinalOld = ObjectCopier.DeepCopy(item.DiseasesReference.DiseaseNameVN);
                DiseasesReferenceCopy = ObjectCopier.DeepCopy(item.DiseasesReference);
            }
            else
            {
                DiagnosisFinalNew = DiagnosisFinalOld = "";
                DiseasesReferenceCopy = null;
            }
        }
        public void grdConsultation_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DiagnosisIcd10Items objRows = e.Row.DataContext as DiagnosisIcd10Items;
            if (objRows != null)
            {
                switch (objRows.IsMain)
                {
                    case true:
                        e.Row.Background = new SolidColorBrush(Color.FromArgb(128, 250, 155, 232));
                        break;
                    default:
                        e.Row.Background = new SolidColorBrush(Colors.White);
                        break;
                }
                if (objRows.IsInvalid)
                {
                    e.Row.Background = new SolidColorBrush(Color.FromArgb(115, 114, 113, 30));
                }
                //20200424 TBL: BM 0030109: Khi ICD10 ngưng sử dụng thì sẽ tô màu đỏ
                if (objRows.DiseasesReference != null && !objRows.DiseasesReference.IsActive)
                {
                    e.Row.Background = new SolidColorBrush(Color.FromRgb(255, 69, 0));
                }
            }

        }
        public void GetDiagTreatmentFinal(DiseasesReference diseasesReference)
        {
            if (diseasesReference != null)
            {
                DiagnosisFinalNew = diseasesReference.DiseaseNameVN;
                if (DiagnosisFinalOld != "")
                {
                    DiagnosisTreatmentContent.DiagnosisFinal = DiagnosisTreatmentContent.DiagnosisFinal.Replace(DiagnosisFinalOld, DiagnosisFinalNew);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(DiagnosisTreatmentContent.DiagnosisFinal))
                    {
                        DiagnosisTreatmentContent.DiagnosisFinal += DiagnosisFinalNew;
                    }
                    else
                    {
                        DiagnosisTreatmentContent.DiagnosisFinal += "; " + DiagnosisFinalNew;
                    }
                }
                DiagnosisFinalOld = ObjectCopier.DeepCopy(DiagnosisFinalNew);
            }

        }
        AutoCompleteBox Acb_ICD10_Code = null;

        AutoCompleteBox Acb_ICD10_Name = null;

        private byte Type = 0;
        public void AcbICD10Code_Loaded(object sender, RoutedEventArgs e)
        {
            Acb_ICD10_Code = (AutoCompleteBox)sender;
        }
        public void AcbICD10Name_Loaded(object sender, RoutedEventArgs e)
        {
            Acb_ICD10_Name = (AutoCompleteBox)sender;
        }
        public void aucICD10_Populating(object sender, PopulatingEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("======> aucICD10_Populating OUT <<<<=======");
            if (IsCode)
            {
                System.Diagnostics.Debug.WriteLine("======> aucICD10_Populating IN <<<<=======");
                e.Cancel = true;

                Type = 0;
                LoadRefDiseases(e.Parameter, 0, 0, 100);
            }
        }
        private bool isDropDown = false;
        public void AxAutoComplete_DropDownClosing(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            isDropDown = true;
        }
        public void AxAutoComplete_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (!isDropDown)
            {
                return;
            }
            isDropDown = false;
            if (refIDC10Item != null && Acb_ICD10_Code != null)
            {
                refIDC10Item.DiseasesReference = new DiseasesReference();
                refIDC10Item.DiseasesReference = Acb_ICD10_Code.SelectedItem as DiseasesReference;
                if (CheckCountIsMain())
                {
                    refIDC10Item.IsMain = true;
                }
                if (CheckExists(refIDC10Item))
                {
                    GetDiagTreatmentFinal(refIDC10Item.DiseasesReference);
                }
            }
        }

        private bool isDiseaseDropDown = false;
        public void DiseaseName_DropDownClosing(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            System.Diagnostics.Debug.WriteLine(" <====== DiseaseName_DropDownClosing =====>");
            isDiseaseDropDown = true;
        }

        public void DiseaseName_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (!isDiseaseDropDown)
            {
                return;
            }
            System.Diagnostics.Debug.WriteLine(" <====== DiseaseName_DropDownClosed =====>");
            isDiseaseDropDown = false;

            refIDC10Item.DiseasesReference = ((AutoCompleteBox)sender).SelectedItem as DiseasesReference;
            if (CheckCountIsMain())
            {
                refIDC10Item.IsMain = true;
            }
            if (CheckExists(refIDC10Item))
            {
                GetDiagTreatmentFinal(refIDC10Item.DiseasesReference);
            }
        }
        public void aucICD10Name_Populating(object sender, PopulatingEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("======> aucICD10Name_Populating OUT <<<<=======");
            if (!IsCode && ColumnIndex == 1)
            {
                System.Diagnostics.Debug.WriteLine("======> aucICD10Name_Populating IN <<<<=======");
                e.Cancel = true;
                Type = 1;
                LoadRefDiseases(e.Parameter, 1, 0, 100);
            }
        }
        public void LoadRefDiseases(string name, byte type, int PageIndex, int PageSize)
        {
            System.Diagnostics.Debug.WriteLine("======> LoadRefDiseases <<<<=======");
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginSearchRefDiseases(name, PageIndex, PageSize, type
                        , Registration_DataStorage.CurrentPatient.PatientID
                        , Globals.GetCurServerDateTime()
                        , Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int Total = 10;
                            var results = contract.EndSearchRefDiseases(out Total, asyncResult);

                            if (type == 0)
                            {
                                refIDC10Code.Clear();
                                if (results != null)
                                {
                                    foreach (DiseasesReference p in results)
                                    {
                                        refIDC10Code.Add(p);
                                    }
                                }
                                if (refIDC10Code.Count > 0)
                                {
                                    this.grdConsultation.bIcd10CodeAcbPopulated = true;
                                }
                                Acb_ICD10_Code.ItemsSource = refIDC10Code;
                                Acb_ICD10_Code.PopulateComplete();
                            }
                            else
                            {
                                refIDC10Name.Clear();
                                if (results != null)
                                {
                                    foreach (DiseasesReference p in results)
                                    {
                                        refIDC10Name.Add(p);
                                    }
                                }
                                if (refIDC10Code.Count > 0)
                                {
                                    this.grdConsultation.bIcd10CodeAcbPopulated = true;
                                }
                                Acb_ICD10_Name.ItemsSource = refIDC10Name;
                                Acb_ICD10_Name.PopulateComplete();
                            }

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {

                        }

                    }), null);

                }

            });

            t.Start();
        }
        private ObservableCollection<DiseasesReference> _refIDC10Code;
        public ObservableCollection<DiseasesReference> refIDC10Code
        {
            get
            {
                return _refIDC10Code;
            }
            set
            {
                if (_refIDC10Code != value)
                {
                    _refIDC10Code = value;
                }
                NotifyOfPropertyChange(() => refIDC10Code);
            }
        }

        private ObservableCollection<DiseasesReference> _refIDC10Name;

        public ObservableCollection<DiseasesReference> refIDC10Name
        {
            get
            {
                return _refIDC10Name;
            }
            set
            {
                if (_refIDC10Name != value)
                {
                    _refIDC10Name = value;
                }
                NotifyOfPropertyChange(() => refIDC10Name);
            }
        }
        private bool CheckCountIsMain()
        {
            ObservableCollection<DiagnosisIcd10Items> temp = ICD10List.Where(x => x.DiseasesReference != null).ToObservableCollection();
            if (temp != null && temp.Count > 0)
            {
                int bcount = 0;
                for (int i = 0; i < temp.Count; i++)
                {
                    if (temp[i].IsMain)
                    {
                        bcount++;
                    }
                }
                if (bcount == 0)
                {
                    return true;
                }
            }
            return false;
        }
        private const int IsAllowCopyDiagnosisFinal = 1;
        private const int IsAllowCopyOrientedTreatment = 2;
        private const int IsAllowCopyDoctorComments = 4;
        public void setDefaultValueWhenReNew()
        {
            if (DiagnosisTreatmentContent != null)
            {
                //▼====: #012
                //▼====: #009
                PhysicalExamination LastPhysicalExamination = new PhysicalExamination();
                if (Registration_DataStorage != null && Registration_DataStorage.PtPhyExamList != null)
                {
                    LastPhysicalExamination = Registration_DataStorage.PtPhyExamList.FirstOrDefault();
                }
                DiagnosisTreatmentContent.Pulse = Convert.ToDecimal(LastPhysicalExamination.Pulse);
                DiagnosisTreatmentContent.Temperature = Convert.ToDecimal(LastPhysicalExamination.Temperature);
                DiagnosisTreatmentContent.BloodPressure = Convert.ToDecimal(LastPhysicalExamination.SystolicPressure);
                DiagnosisTreatmentContent.LowerBloodPressure = Convert.ToDecimal(LastPhysicalExamination.DiastolicPressure);
                DiagnosisTreatmentContent.SpO2 = Convert.ToDecimal(LastPhysicalExamination.SpO2);
                //▼==== #013
                DiagnosisTreatmentContent.RespiratoryRate = Convert.ToDecimal(LastPhysicalExamination.RespiratoryRate);
                //▲==== #013
                //▲====: #009
                //▲====: #012
                //▼===== #003: Cấu hình trường cần clear khi tạo mới
                if ((Globals.ServerConfigSection.ConsultationElements.IsAllowCopyDiagTrmtInstruction & IsAllowCopyDiagnosisFinal) == 0)
                {
                    DiagnosisTreatmentContent.DiagnosisFinal = "";
                    ICD10List = new ObservableCollection<DiagnosisIcd10Items>();
                }
                if ((Globals.ServerConfigSection.ConsultationElements.IsAllowCopyDiagTrmtInstruction & IsAllowCopyOrientedTreatment) < 2
                    || DiagnosisTreatmentContent.DoctorStaffID != Globals.LoggedUserAccount.Staff.StaffID)
                {
                    DiagnosisTreatmentContent.OrientedTreatment = "";
                }
                if ((Globals.ServerConfigSection.ConsultationElements.IsAllowCopyDiagTrmtInstruction & IsAllowCopyDoctorComments) < 4)
                {
                    DiagnosisTreatmentContent.DoctorComments = "";
                }
                //▲===== #003
            }
        }
        //▲====== #001
        //▼===== #006
        private const Int16 IsAllowCopyOrientedTrt = 1;
        private const Int16 IsAllowCopyDiagFinal = 2;
        private const Int16 IsAllowCopyDoctorCmt = 4;
        private const Int16 IsAllowCopyIDC10List = 8;
        public void CreateNewFromOld(DiagnosisTreatment diagnosisTreatment, ObservableCollection<DiagnosisIcd10Items> ICD10lst)
        {
            DiagnosisTreatmentContent = ObjectCopier.DeepCopy(diagnosisTreatment);
            ICD10List = ObjectCopier.DeepCopy(ICD10lst);
            DiagnosisTreatmentContent.Pulse = 0;
            DiagnosisTreatmentContent.BloodPressure = 0;
            DiagnosisTreatmentContent.LowerBloodPressure = 0;
            DiagnosisTreatmentContent.Temperature = 0;
            DiagnosisTreatmentContent.SpO2 = 0;
            //TBL: Tuy theo cau hinh se xoa truong nao khong muon 
            //TBL: Truong Dien tien benh khi cau hinh = 1 se dc giu lai
            if ((Globals.ServerConfigSection.ConsultationElements.IsAllowCopyInstruction & IsAllowCopyOrientedTrt) == 0)
            {
                DiagnosisTreatmentContent.OrientedTreatment = "";
            }
            //TBL: Truong Chan doan khi cau hinh = 2 se dc giu lai
            if ((Globals.ServerConfigSection.ConsultationElements.IsAllowCopyInstruction & IsAllowCopyDiagFinal) < 2)
            {
                DiagnosisTreatmentContent.DiagnosisFinal = "";
            }
            //TBL: Truong Chu thich khi cau hinh = 4 se dc giu lai
            if ((Globals.ServerConfigSection.ConsultationElements.IsAllowCopyInstruction & IsAllowCopyDoctorCmt) < 4)
            {
                DiagnosisTreatmentContent.DoctorComments = "";
            }
            //TBL: Truong ICD10List khi cau hinh = 8 se dc giu lai
            if ((Globals.ServerConfigSection.ConsultationElements.IsAllowCopyInstruction & IsAllowCopyIDC10List) < 8)
            {
                ICD10List = new ObservableCollection<DiagnosisIcd10Items>();
                AddBlankRow();
            }
        }
        //▲===== #006

        //▼===== #007
        public void SetValueWhenSaveAndUpdateDiag(long aIntPtDiagDrInstructionID, long DeptID = 0)
        {
            if (DiagnosisTreatmentContent != null)
            {
                DiagnosisTreatmentContent.IntPtDiagDrInstructionID = aIntPtDiagDrInstructionID;
                if (CheckValidNewDiagnosis(DeptID))
                {
                    DiagnosisTreatmentContent.PatientServiceRecord.PatientMedicalRecord.PatientID = aIntPtDiagDrInstructionID;
                }
                Registration_DataStorage.CurrentDiagnosisTreatment = DiagnosisTreatmentContent;
            }
        }
        //▲===== #007
        public void btDiseaseProgression()
        {
            GlobalsNAV.ShowDialog<IDiseaseProgression>((proAlloc) => {
                proAlloc.IsOpenFromInstruction = true;
            }, null, false, true, new Size(950, 950));
        }

        private Dictionary<string, string> _ShortHandDictionaryObj;
        public Dictionary<string, string> ShortHandDictionaryObj
        {
            get => _ShortHandDictionaryObj; set
            {
                _ShortHandDictionaryObj = value;
                NotifyOfPropertyChange(() => ShortHandDictionaryObj);
            }
        }

        public void GetShortHandDictionaries(long StaffID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetShortHandDictionariesByStaffID(StaffID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var mShortHandDictionaries = contract.EndGetShortHandDictionariesByStaffID(asyncResult);
                            if (mShortHandDictionaries == null)
                            {
                                ShortHandDictionaryObj = new Dictionary<string, string>();
                            }
                            else
                            {
                                ShortHandDictionaryObj = mShortHandDictionaries.ToDictionary(x => x.ShortHandDictionaryKey.ToString().ToLower(), x => x.ShortHandDictionaryValue.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobalsNAV.ShowMessagePopup(ex.Message);
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
    }
}
