using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.Common.Utilities;
using aEMR.Controls;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.ViewContracts.Consultation_ePrescription;
using Caliburn.Micro;
using Castle.Windsor;
using DataEntities;
using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
/*
 * 20220308 #001 QTD:   Thêm trường Cho điều trị tại khoa, Ghi chú, Lý do vào viện
 * 20221231 #002 BLQ: Truyền thông tin từ màn hình chẩn đoán vào phiếu khám vào viện từ màn hình Hồ sơ bệnh án
 * 20230110 #003 DatTB: Thêm biến lấy tiểu sử bản thân bệnh án mãn tính
 */

namespace aEMR.Consultation.ViewModels
{
    [Export(typeof(IAdmissionExamination)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class AdmissionExaminationViewModel : ViewModelBase, IAdmissionExamination
    {
        [ImportingConstructor]
        public AdmissionExaminationViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            //LoadAdmissionExamination(InPtRegistrationID);
            refIDC10Code = new ObservableCollection<DiseasesReference>();
            refIDC10Name = new ObservableCollection<DiseasesReference>();
            //▼====: #001
            DepartmentContent = Globals.GetViewModel<IDepartmentListing>();
            DepartmentContent.AddSelectOneItem = true;
            DepartmentContent.LoadData(0, false, true);
            (DepartmentContent as PropertyChangedBase).PropertyChanged += new PropertyChangedEventHandler(DepartmentContent_PropertyChanged);
            //▲====: #001
        }

        #region Properities
        public override bool IsProcessing
        {
            get
            {
                return false;
            }
        }
        private long _InPtRegistrationID;
        public long InPtRegistrationID
        {
            get
            {
                return _InPtRegistrationID;
            }
            set
            {
                if (_InPtRegistrationID != value)
                {
                    _InPtRegistrationID = value;
                    NotifyOfPropertyChange(() => InPtRegistrationID);
                }
            }
        }
        private AdmissionExamination _CurAdmissionExamination;
        public AdmissionExamination CurAdmissionExamination
        {
            get
            {
                return _CurAdmissionExamination;
            }
            set
            {
                if (_CurAdmissionExamination != value)
                {
                    _CurAdmissionExamination = value;
                    NotifyOfPropertyChange(() => CurAdmissionExamination);
                }
            }
        }
        #endregion

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }

        public void LoadAdmissionExamination(long PtRegistrationID, string PathologicalProcessReUsed, bool pkvvrhm)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePMRsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAdmissionExamination(PtRegistrationID, V_RegistrationType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                CurAdmissionExamination = contract.EndGetAdmissionExamination(asyncResult);
                                if (CurAdmissionExamination.AdmissionExaminationID == 0)
                                {
                                    CurAdmissionExamination.PtRegistrationID = InPtRegistrationID;
                                    CurAdmissionExamination.CreatedDate = Globals.GetCurServerDateTime();
                                    CurAdmissionExamination.CreatedStaffID = Globals.LoggedUserAccount.Staff.StaffID;
                                    CurAdmissionExamination.PathologicalProcess = PathologicalProcessReUsed;
                                    //▼====: #002
                                    if (IsChronic)
                                    {
                                        CurAdmissionExamination.ReasonAdmission = DiagTrmtItem.ReasonHospitalStay;
                                        CurAdmissionExamination.PathologicalProcess = DiagTrmtItem.Diagnosis;
                                        CurAdmissionExamination.FamilyMedicalHistory = "Khoẻ";
                                        CurAdmissionExamination.FullBodyExamination = DiagTrmtItem.OrientedTreatment;
                                        CurAdmissionExamination.PartialExamination = "Chưa ghi nhận bệnh lý";
                                        //▼==== #003
                                        CurAdmissionExamination.MedicalHistory = PastMedicalHistory;
                                        //▲==== #003
                                    }
                                    //▲====: #002
                                    if (pkvvrhm)
                                    {
                                        CurAdmissionExamination.ReasonAdmission = "Đau răng";
                                        CurAdmissionExamination.MedicalHistory = "Khoẻ";
                                        CurAdmissionExamination.FamilyMedicalHistory = "Khoẻ";
                                        CurAdmissionExamination.FullBodyExamination = "Tổng trạng khỏe, tiếp xúc tốt, da niêm hồng";
                                        CurAdmissionExamination.PartialExamination = "Chưa ghi nhận bệnh lý";
                                        CurAdmissionExamination.DiagnosisResult = Diagnosis;
                                        CurAdmissionExamination.DrugTreatment = "Chưa";
                                    }
                                }
                                ICD10List = new ObservableCollection<DiagnosisIcd10Items>();
                                AddBlankRow();
                                if (pkvvrhm)
                                {
                                    SetDepartment();
                                }
                                IsPKVVRhm = pkvvrhm;
                                CurAdmissionExamination.V_RegistrationType = CurAdmissionExamination.V_RegistrationType != 0 ? CurAdmissionExamination.V_RegistrationType : V_RegistrationType;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
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
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void SaveAdmissionExamination()
        {
            //InitDataBeforeSave();
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePMRsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSaveAdmissionExamination(CurAdmissionExamination, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                long AdmissionExaminationID_New;
                                var result = contract.EndSaveAdmissionExamination(out AdmissionExaminationID_New, asyncResult);
                                if (result)
                                {
                                    CurAdmissionExamination.AdmissionExaminationID = AdmissionExaminationID_New;
                                    MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
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
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private bool CheckValidBeforeSave()
        {
            if (//string.IsNullOrEmpty(CurAdmissionExamination.ReferralDiagnosis) || 
                string.IsNullOrEmpty(CurAdmissionExamination.PathologicalProcess)
                || string.IsNullOrEmpty(CurAdmissionExamination.MedicalHistory) || string.IsNullOrEmpty(CurAdmissionExamination.FullBodyExamination)
                || string.IsNullOrEmpty(CurAdmissionExamination.PartialExamination))
            {
                return false;
            }
            return true;
        }

        private void InitDataBeforeSave()
        {
            if(CurAdmissionExamination.AdmissionExaminationID == 0)
            {
                CurAdmissionExamination.PtRegistrationID = InPtRegistrationID;
            }
            CurAdmissionExamination.CreatedDate = Globals.GetCurServerDateTime();
            CurAdmissionExamination.CreatedStaffID = Globals.LoggedUserAccount.Staff.StaffID;
        }

        public void btnSave()
        {
            if (!CheckValidBeforeSave())
            {
                MessageBox.Show("Các trường có dấu (*) phải nhập thông tin");
                return;
            }
            if (IsPKVVRhm && CurAdmissionExamination != null && CurAdmissionExamination.DeptID == 0)
            {
                MessageBox.Show("Chọn khoa điều trị");
                return;
            }
            if (ApplyCheckInPtRegistration && bCheckInPtRegistration && IsPKVVRhm)
            {
                MessageBox.Show("BN đã có đăng ký nội trú!");
                return;
            }

            SaveAdmissionExamination();
        }
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
                        Application.Current.Dispatcher.Invoke(() => AddBlankRow());
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
                DiagnosisFinalNew = DiagnosisFinalOld = ObjectCopier.DeepCopy(item.ICD10Code + ": " + item.DiseasesReference.DiseaseNameVN);
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
                DiagnosisFinalNew = diseasesReference.ICD10Code + ": " + diseasesReference.DiseaseNameVN;
                if (DiagnosisFinalOld != "")
                {
                    CurAdmissionExamination.ReferralDiagnosis = CurAdmissionExamination.ReferralDiagnosis.Replace(DiagnosisFinalOld, DiagnosisFinalNew);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(CurAdmissionExamination.ReferralDiagnosis))
                    {
                        CurAdmissionExamination.ReferralDiagnosis += DiagnosisFinalNew;
                    }
                    else
                    {
                        CurAdmissionExamination.ReferralDiagnosis += "; " + DiagnosisFinalNew;
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
                        , 0
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
                                        grdConsultation.bIcd10CodeAcbPopulated = true;
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
                                        grdConsultation.bIcd10CodeAcbPopulated = true;
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
                        Application.Current.Dispatcher.Invoke(() => AddBlankRow());
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
                    if (item.DiseasesReference != null
                        && item.DiseasesReference.DiseaseNameVN != "")
                    {
                        CurAdmissionExamination.ReferralDiagnosis = CurAdmissionExamination.ReferralDiagnosis.Replace(item.DiseasesReference.ICD10Code + ": " + item.DiseasesReference.DiseaseNameVN, "");
                    }
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
        #endregion
        //▼====: #001
        private bool _IsShowReferralDiagnosis = true;
        public bool IsShowReferralDiagnosis
        {
            get
            {
                return _IsShowReferralDiagnosis;
            }
            set
            {
                if (_IsShowReferralDiagnosis != value)
                {
                    _IsShowReferralDiagnosis = value;
                    NotifyOfPropertyChange(() => IsShowReferralDiagnosis);
                }
            }
        }

        private bool _IsShowPclResult = true;
        public bool IsShowPclResult
        {
            get
            {
                return _IsShowPclResult;
            }
            set
            {
                if (_IsShowPclResult != value)
                {
                    _IsShowPclResult = value;
                    NotifyOfPropertyChange(() => IsShowPclResult);
                }
            }
        }

        private bool _IsShowDepartment = false;
        public bool IsShowDepartment
        {
            get
            {
                return _IsShowDepartment;
            }
            set
            {
                if (_IsShowDepartment != value)
                {
                    _IsShowDepartment = value;
                    NotifyOfPropertyChange(() => IsShowDepartment);
                }
            }
        }

        private bool _IsShowNotes = false;
        public bool IsShowNotes
        {
            get
            {
                return _IsShowNotes;
            }
            set
            {
                if (_IsShowNotes != value)
                {
                    _IsShowNotes = value;
                    NotifyOfPropertyChange(() => IsShowNotes);
                }
            }
        }

        private bool _IsShowReasonAdmission = false;
        public bool IsShowReasonAdmission
        {
            get
            {
                return _IsShowReasonAdmission;
            }
            set
            {
                if (_IsShowReasonAdmission != value)
                {
                    _IsShowReasonAdmission = value;
                    NotifyOfPropertyChange(() => IsShowReasonAdmission);
                }
            }
        }

        private bool _IsShowDiagnosisResult = false;
        public bool IsShowDiagnosisResult
        {
            get
            {
                return _IsShowDiagnosisResult;
            }
            set
            {
                if (_IsShowDiagnosisResult != value)
                {
                    _IsShowDiagnosisResult = value;
                    NotifyOfPropertyChange(() => IsShowDiagnosisResult);
                }
            }
        }

        private IDepartmentListing _departmentContent;
        public IDepartmentListing DepartmentContent
        {
            get { return _departmentContent; }
            set
            {
                _departmentContent = value;
                NotifyOfPropertyChange(() => DepartmentContent);
            }
        }

        public void LoadRefDeparments()
        {
            if (Globals.isAccountCheck)
            {
                DepartmentContent.LstRefDepartment = Globals.LoggedUserAccount.DeptIDResponsibilityList;
                DepartmentContent.AddSelectOneItem = true;
            }
            else
            {
                DepartmentContent.AddSelectedAllItem = true;
            }
            bool designTime = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());
            if (!designTime)
            {
                DepartmentContent.LoadData();
            }
        }

        void DepartmentContent_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                DepartmentChange();
            }
        }

        public void DepartmentChange()
        {
            if (CurAdmissionExamination == null || DepartmentContent == null || DepartmentContent.SelectedItem == null)
            {
                return;
            }

            CurAdmissionExamination.DeptID = DepartmentContent.SelectedItem.DeptID;
        }

        private void SetDepartment()
        {
            if (CurAdmissionExamination == null || DepartmentContent == null || DepartmentContent.Departments == null)
            {
                return;
            }

            if (CurAdmissionExamination.DeptID > 0)
            {
                DepartmentContent.SetSelectedDeptItem(CurAdmissionExamination.DeptID);
            }
            else
            {
                //DepartmentContent.SelectedItem = DepartmentContent.Departments.FirstOrDefault();
                DepartmentContent.SelectedItem = DepartmentContent.Departments.Where(x => x.DeptID == Globals.ServerConfigSection.Hospitals.KhoaPhongKham).FirstOrDefault();
                DepartmentChange();
            }
        }

        private bool _IsPKVVRhm = false;
        public bool IsPKVVRhm
        {
            get
            {
                return _IsPKVVRhm;
            }
            set
            {
                if (_IsPKVVRhm != value)
                {
                    _IsPKVVRhm = value;
                    NotifyOfPropertyChange(() => IsPKVVRhm);
                }
            }
        }

        private bool _bCheckInPtRegistration = false;
        public bool bCheckInPtRegistration
        {
            get
            {
                return _bCheckInPtRegistration;
            }
            set
            {
                if (_bCheckInPtRegistration != value)
                {
                    _bCheckInPtRegistration = value;
                    NotifyOfPropertyChange(() => bCheckInPtRegistration);
                }
            }
        }

        private string _Diagnosis;
        public string Diagnosis
        {
            get
            {
                return _Diagnosis;
            }
            set
            {
                if (_Diagnosis != value)
                {
                    _Diagnosis = value;
                    NotifyOfPropertyChange(() => Diagnosis);
                }
            }
        }

        private bool _ApplyCheckInPtRegistration = Globals.ServerConfigSection.CommonItems.ApplyCheckInPtRegistration;
        public bool ApplyCheckInPtRegistration
        {
            get
            {
                return _ApplyCheckInPtRegistration;
            }
            set
            {
                if (_ApplyCheckInPtRegistration != value)
                {
                    _ApplyCheckInPtRegistration = value;
                    NotifyOfPropertyChange(() => ApplyCheckInPtRegistration);
                }
            }
        }

        private void CheckInPtRegistration()
        {
            if (!ApplyCheckInPtRegistration)
            {
                return;
            }
            else
            {
                if (InPtRegistrationID > 0)
                {
                    CheckStatusInPtRegistration_InPtRegistrationID(Convert.ToInt64(InPtRegistrationID));
                }
                else
                {
                    bCheckInPtRegistration = false;
                }
            }
        }
        private void CheckStatusInPtRegistration_InPtRegistrationID(long InPtRegistrationID)
        {
            this.ShowBusyIndicator();
            //IsWaitingGetBlankDiagnosisTreatmentByPtID = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginCheckStatusInPtRegistration_InPtRegistrationID(InPtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            bCheckInPtRegistration = contract.EndCheckStatusInPtRegistration_InPtRegistrationID(asyncResult);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //IsWaitingGetBlankDiagnosisTreatmentByPtID = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private long _V_RegistrationType;
        public long V_RegistrationType
        {
            get
            {
                return _V_RegistrationType != 0 ? _V_RegistrationType : (long)AllLookupValues.RegistrationType.NOI_TRU;
            }
            set
            {
                if (_V_RegistrationType != value)
                {
                    _V_RegistrationType = value;
                    NotifyOfPropertyChange(() => V_RegistrationType);
                }
            }
        }
        //▲====: #001
        //▼====: #002
        private bool _IsChronic;
        public bool IsChronic
        {
            get
            {
                return _IsChronic;
            }
            set
            {
                _IsChronic = value;
                NotifyOfPropertyChange(() => IsChronic);
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
        //▲====: #002
        //▼==== #003
        private string _PastMedicalHistory;
        public string PastMedicalHistory
        {
            get
            {
                return _PastMedicalHistory;
            }
            set
            {
                if (_PastMedicalHistory != value)
                {
                    _PastMedicalHistory = value;
                    NotifyOfPropertyChange(() => PastMedicalHistory);
                }
            }
        }
        //▲==== #003
    }
}
