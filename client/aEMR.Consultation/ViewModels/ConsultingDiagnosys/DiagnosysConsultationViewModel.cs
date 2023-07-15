using System.Collections.Generic;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using aEMR.Common.BaseModel;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using DataEntities;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using eHCMSLanguage;
using System.Collections.ObjectModel;
using aEMR.Infrastructure;
using aEMR.Controls;
using System.Windows.Input;
using aEMR.Common;
using System.Windows.Media;
using System.Threading;
using System;
using aEMR.ServiceClient;
using aEMR.Common.Collections;
using aEMR.Common.ViewModels;
using Caliburn.Micro;
using aEMR.CommonTasks;
using aEMR.ServiceClient.Consultation_PCLs;
using aEMR.DataContracts;
using System.ServiceModel;
/*
 * 20211004 #001 TNHX: Lọc danh sách bsi theo cấu hình trách nhiệm + theo tích tạm khóa
 * 20220511 #002 DatTB: Lấy thêm bsi RMH màn hình chuẩn đoán: x.StaffCatgID == (long)StaffCatg.BsRHM
 */
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IDiagnosysConsultation)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class DiagnosysConsultationViewModel : ViewModelBase, IDiagnosysConsultation
    {
        [ImportingConstructor]
        public DiagnosysConsultationViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            ConsultationDateTime = Globals.GetViewModel<IMinHourDateControl>();
            ConsultationDateTime.DateTime = null;
            //▼====: #001, #002
            DoctorStaffs = Globals.AllStaffs.Where(x => (x.StaffCatgID == (long)StaffCatg.Bs || x.StaffCatgID == (long)StaffCatg.BsRHM) && !x.IsStopUsing).ToList();
            //▲====: #001, #002
            GetAllLookupValuesByType();
            LoadV_DiagnosysConsultation();
            CreateSubVM();
        }

        #region Init for View
        private void CreateSubVM()
        {
            UCPatientProfileInfo = Globals.GetViewModel<IPatientInfo>();
            UCDoctorProfileInfo = Globals.GetViewModel<ILoginInfo>();
            var searchPatientAndRegVm = Globals.GetViewModel<ISearchPatientAndRegistration>();
            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NGOAITRU;
            searchPatientAndRegVm.mTimBN = mNhapVien_Patient_TimBN;
            searchPatientAndRegVm.mThemBN = mNhapVien_Patient_ThemBN;
            searchPatientAndRegVm.mTimDangKy = mNhapVien_Patient_TimDangKy;
            searchPatientAndRegVm.SearchAdmittedInPtRegOnly = true;
            searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
            searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);
            searchPatientAndRegVm.PatientFindBy = AllLookupValues.PatientFindBy.NGOAITRU;
            searchPatientAndRegVm.CanSearhRegAllDept = true;
            searchPatientAndRegVm.bEnableForConsultation = true;
            searchPatientAndRegVm.PatientFindByVisibility = true;
            SearchRegistrationContent = searchPatientAndRegVm;


            refICD10Code = new ObservableCollection<DiseasesReference>();
            refICD10Name = new ObservableCollection<DiseasesReference>();
            refICD10List = new ObservableCollection<DiagnosisIcd10Items>();
            DiagTrmtItem = new DiagnosisTreatment();
            gDiagConsultation = new DiagnosysConsultationSummary();
            SurgeryDoctorCollection = new ObservableCollection<Staff>();
            Registration_DataStorage = new Registration_DataStorage();
            AddBlankRow();
        }
        private void ActivateSubVM()
        {
            ActivateItem(UCPatientProfileInfo);
            ActivateItem(SearchRegistrationContent);
            ActivateItem(UCDoctorProfileInfo);
        }
        private void DeActivateSubVM(bool close)
        {
            DeactivateItem(UCPatientProfileInfo, close);
            DeactivateItem(SearchRegistrationContent, close);
            DeactivateItem(UCDoctorProfileInfo, close);
        }
        protected override void OnActivate()
        {

            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            ActivateSubVM();
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
            DeActivateSubVM(close);
        }
        #endregion

        #region Properties
        public object SearchRegistrationContent { get; set; }
        public ILoginInfo UCDoctorProfileInfo { get; set; }
        public IPatientInfo UCPatientProfileInfo { get; set; }

        private ObservableCollection<Staff> _SurgeryDoctorCollection;
        public ObservableCollection<Staff> SurgeryDoctorCollection
        {
            get
            {
                return _SurgeryDoctorCollection;
            }
            set
            {
                if (_SurgeryDoctorCollection == value)
                {
                    return;
                }
                _SurgeryDoctorCollection = value;
                NotifyOfPropertyChange(() => SurgeryDoctorCollection);
            }
        }
        private Staff _SelectedSurgeryDoctor;
        public Staff SelectedSurgeryDoctor
        {
            get
            {
                return _SelectedSurgeryDoctor;
            }
            set
            {
                if (_SelectedSurgeryDoctor == value) return;
                _SelectedSurgeryDoctor = value;
                NotifyOfPropertyChange(() => SelectedSurgeryDoctor);
            }
        }
        private Staff _SelectedPresiderStaff;
        public Staff SelectedPresiderStaff
        {
            get
            {
                return _SelectedPresiderStaff;
            }
            set
            {
                if (_SelectedPresiderStaff == value) return;
                _SelectedPresiderStaff = value;
                NotifyOfPropertyChange(() => SelectedPresiderStaff);
            }
        }
        private Staff _SelectedSecretaryStaff;
        public Staff SelectedSecretaryStaff
        {
            get
            {
                return _SelectedSecretaryStaff;
            }
            set
            {
                if (_SelectedSecretaryStaff == value) return;
                _SelectedSecretaryStaff = value;
                NotifyOfPropertyChange(() => SelectedSecretaryStaff);
            }
        }
        private Staff _CurrentSurgeryDoctor;
        public Staff CurrentSurgeryDoctor
        {
            get
            {
                return _CurrentSurgeryDoctor;
            }
            set
            {
                if (_CurrentSurgeryDoctor == value) return;
                _CurrentSurgeryDoctor = value;
                NotifyOfPropertyChange(() => CurrentSurgeryDoctor);
            }
        }
        private List<Staff> _DoctorStaffs;
        public List<Staff> DoctorStaffs
        {
            get
            {
                return _DoctorStaffs;
            }
            set
            {
                _DoctorStaffs = value;
                NotifyOfPropertyChange(() => DoctorStaffs);
            }
        }
        private DiagnosisIcd10Items _refIDC10Item;
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
                    if (_refIDC10Item != null)
                    {
                        refIDC10Item.IsObjectBeingUsedByClient = true;
                    }
                }
                NotifyOfPropertyChange(() => refIDC10Item);
            }
        }
        private DiagnosisTreatment _NewDiagTrmtItem;
        public DiagnosisTreatment DiagTrmtItem
        {
            get
            {
                return _NewDiagTrmtItem;
            }
            set
            {
                if (_NewDiagTrmtItem != value)
                {
                    _NewDiagTrmtItem = value;
                    if (_NewDiagTrmtItem != null)
                    {
                        DiagTrmtItem.IsObjectBeingUsedByClient = true;
                    }
                    NotifyOfPropertyChange(() => DiagTrmtItem);
                }
            }
        }
        private ObservableCollection<DiagnosisIcd10Items> _NewrefIDC10List;
        public ObservableCollection<DiagnosisIcd10Items> refICD10List
        {
            get
            {
                return _NewrefIDC10List;
            }
            set
            {
                if (_NewrefIDC10List != value)
                {
                    _NewrefIDC10List = value;
                }
                NotifyOfPropertyChange(() => refICD10List);
            }
        }
        private string DiagnosisFinalOld = "";
        private string DiagnosisFinalNew = "";
        private DiagnosysConsultationSummary _gDiagConsultation;
        public DiagnosysConsultationSummary gDiagConsultation
        {
            get { return _gDiagConsultation; }
            set
            {
                if (_gDiagConsultation != value)
                {
                    _gDiagConsultation = value;
                    NotifyOfPropertyChange(() => gDiagConsultation);
                }
            }
        }
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
        private string _ConsultationResult;
        public string ConsultationResult
        {
            get { return _ConsultationResult; }
            set
            {
                if (_ConsultationResult != value)
                {
                    _ConsultationResult = value;
                    NotifyOfPropertyChange(() => ConsultationResult);
                }
            }
        }
        private string _ConsultationSummary;
        public string ConsultationSummary
        {
            get { return _ConsultationSummary; }
            set
            {
                if (_ConsultationSummary != value)
                {
                    _ConsultationSummary = value;
                    NotifyOfPropertyChange(() => ConsultationSummary);
                }
            }
        }
        private string _ConsultationTreatment;
        public string ConsultationTreatment
        {
            get { return _ConsultationTreatment; }
            set
            {
                if (_ConsultationTreatment != value)
                {
                    _ConsultationTreatment = value;
                    NotifyOfPropertyChange(() => ConsultationTreatment);
                }
            }
        }
        //private DateTime _ConsultationDate = Globals.GetCurServerDateTime();
        //public DateTime ConsultationDate
        //{
        //    get { return _ConsultationDate; }
        //    set
        //    {
        //        if (_ConsultationDate != value)
        //        {
        //            _ConsultationDate = value;
        //            NotifyOfPropertyChange(() => ConsultationDate);
        //        }
        //    }
        //}
        private bool _isUpdate = false;
        public bool isUpdate
        {
            get { return _isUpdate; }
            set
            {
                if (_isUpdate != value)
                {
                    _isUpdate = value;
                    NotifyOfPropertyChange(() => isUpdate);
                }
            }
        }
        private ObservableCollection<DiseasesReference> _refIDC10Name;

        public ObservableCollection<DiseasesReference> refICD10Name
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
                NotifyOfPropertyChange(() => refICD10Name);
            }
        }

        private bool _mNhapVien_Patient_TimBN = true;
        private bool _mNhapVien_Patient_ThemBN = true;
        private bool _mNhapVien_Patient_TimDangKy = true;

        public bool mNhapVien_Patient_TimBN
        {
            get
            {
                return _mNhapVien_Patient_TimBN;
            }
            set
            {
                if (_mNhapVien_Patient_TimBN == value)
                    return;
                _mNhapVien_Patient_TimBN = value;
                NotifyOfPropertyChange(() => mNhapVien_Patient_TimBN);
            }
        }

        public bool mNhapVien_Patient_ThemBN
        {
            get
            {
                return _mNhapVien_Patient_ThemBN;
            }
            set
            {
                if (_mNhapVien_Patient_ThemBN == value)
                    return;
                _mNhapVien_Patient_ThemBN = value;
                NotifyOfPropertyChange(() => mNhapVien_Patient_ThemBN);
            }
        }

        public bool mNhapVien_Patient_TimDangKy
        {
            get
            {
                return _mNhapVien_Patient_TimDangKy;
            }
            set
            {
                if (_mNhapVien_Patient_TimDangKy == value)
                    return;
                _mNhapVien_Patient_TimDangKy = value;
                NotifyOfPropertyChange(() => mNhapVien_Patient_TimDangKy);
            }
        }
        private Registration_DataStorage _Registration_DataStorage;
        public Registration_DataStorage Registration_DataStorage
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
                UCPatientProfileInfo.Registration_DataStorage = Registration_DataStorage;
                NotifyOfPropertyChange(() => Registration_DataStorage);
            }
        }
        AutoCompleteBox Acb_ICD10_Code = null;

        AutoCompleteBox Acb_ICD10_Name = null;
        private ObservableCollection<DiseasesReference> _refIDC10Code;
        public ObservableCollection<DiseasesReference> refICD10Code
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
                NotifyOfPropertyChange(() => refICD10Code);
            }
        }

        private bool _IsNotLockView = false;
        public bool IsNotLockView
        {
            get
            {
                return _IsNotLockView;
            }
            set
            {
                _IsNotLockView = value;
                NotifyOfPropertyChange(() => IsNotLockView);
            }
        }

        private bool _IsEdit = false;
        public bool IsEdit
        {
            get
            {
                return _IsEdit;
            }
            set
            {
                _IsEdit = value;
                NotifyOfPropertyChange(() => IsEdit);
            }
        }
        private bool _IsIgnore = false;
        public bool IsIgnore
        {
            get
            {
                return _IsIgnore;
            }
            set
            {
                _IsIgnore = value;
                NotifyOfPropertyChange(() => IsIgnore);
            }
        }
        private bool _IsCreateNew = false;
        public bool IsCreateNew
        {
            get
            {
                return _IsCreateNew;
            }
            set
            {
                _IsCreateNew = value;
                NotifyOfPropertyChange(() => IsCreateNew);
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

        private ObservableCollection<DiagnosysConsultationSummary> _DiagnosysConsultationCollection;
        public ObservableCollection<DiagnosysConsultationSummary> DiagnosysConsultationCollection
        {
            get
            {
                return _DiagnosysConsultationCollection;
            }
            set
            {
                if (_DiagnosysConsultationCollection == value)
                {
                    return;
                }
                _DiagnosysConsultationCollection = value;
                NotifyOfPropertyChange(() => DiagnosysConsultationCollection);
            }
        }
        private DiagnosysConsultationSummary _SelectedDiagnosysConsultation;
        public DiagnosysConsultationSummary SelectedDiagnosysConsultation
        {
            get
            {
                return _SelectedDiagnosysConsultation;
            }
            set
            {
                if (_SelectedDiagnosysConsultation == value)
                {
                    return;
                }
                _SelectedDiagnosysConsultation = value;
                NotifyOfPropertyChange(() => SelectedDiagnosysConsultation);
            }
        }

        private ObservableCollection<Lookup> _DiagnosysConsultationType;
        public ObservableCollection<Lookup> DiagnosysConsultationType
        {
            get { return _DiagnosysConsultationType; }
            set
            {
                _DiagnosysConsultationType = value;
                NotifyOfPropertyChange(() => DiagnosysConsultationType);
            }
        }
        #endregion

        #region Method For ICD and Doctor use AutoComplete
        public void btnAddSugeryDoctor()
        {
            if (SelectedSurgeryDoctor == null)
            {
                return;
            }
            if (SelectedSurgeryDoctor.StaffID > 0)
            {
                if (SurgeryDoctorCollection == null)
                {
                    SurgeryDoctorCollection = new ObservableCollection<Staff>();
                }
                if (!SurgeryDoctorCollection.Contains(SelectedSurgeryDoctor))
                {
                    SurgeryDoctorCollection.Add(SelectedSurgeryDoctor);
                }
                else
                {
                    MessageBox.Show(eHCMSResources.T1987_G1_DaTonTai, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                }
            }
        }
        public void cboSurgeryDoctor_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            if (string.IsNullOrEmpty(cboContext.SearchText)) return;
            var AllItemsContext = new ObservableCollection<Staff>(DoctorStaffs.Where(x => Globals.RemoveVietnameseString(x.FullName.ToLower()).Contains(cboContext.SearchText.ToLower())));
            cboContext.ItemsSource = AllItemsContext;
            cboContext.PopulateComplete();
        }
        private bool isDropDownSurgeryDoctor = false;
        public void cboSurgeryDoctor_DropDownClosing(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            isDropDownSurgeryDoctor = true;
        }
        public void cboSurgeryDoctor_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (!isDropDownSurgeryDoctor)
            {
                return;
            }
            isDropDownSurgeryDoctor = false;
            if (sender != null)
            {
                SelectedSurgeryDoctor = ((AutoCompleteBox)sender).SelectedItem as Staff;
            }

        }

        AxDataGridNyICD10 grdConsultation { get; set; }
        public void grdConsultation_Loaded(object sender, RoutedEventArgs e)
        {
            grdConsultation = sender as AxDataGridNyICD10;
        }

        public void grdConsultation_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (grdConsultation != null && grdConsultation.SelectedItem != null)
            {
                grdConsultation.BeginEdit();
            }
        }
        private DiseasesReference DiseasesReferenceCopy = null;

        bool IsCode = true;
        int ColumnIndex = 0;
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
        public void AxDataGridNy_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
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
                    if (e.Row.GetIndex() == (refICD10List.Count - 1) && e.EditAction == DataGridEditAction.Commit)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(() => AddBlankRow());
                    }
                }
            }
        }
        private void AddBlankRow()
        {
            if (refICD10List != null
                && refICD10List.LastOrDefault() != null
                && refICD10List.LastOrDefault().DiseasesReference == null)
            {
                return;
            }
            DiagnosisIcd10Items ite = new DiagnosisIcd10Items();
            ite.V_DiagIcdStatus = (long)AllLookupValues.V_DiagIcdStatus.DANGDIEUTRI;
            ite.LookupStatus = new Lookup();
            ite.LookupStatus.LookupID = (long)AllLookupValues.V_DiagIcdStatus.DANGDIEUTRI;
            ite.LookupStatus.ObjectValue = eHCMSResources.Z0540_G1_DangDTri.ToUpper();
            refICD10List.Add(ite);
        }
        public void GetDiagTreatmentFinal(DiseasesReference diseasesReference)
        {
            if (diseasesReference != null)
            {
                DiagnosisFinalNew = diseasesReference.DiseaseNameVN;
                if (DiagnosisFinalOld != "")
                {
                    DiagTrmtItem.DiagnosisFinal = DiagTrmtItem.DiagnosisFinal.Replace(DiagnosisFinalOld, DiagnosisFinalNew);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(DiagTrmtItem.DiagnosisFinal))
                    {
                        DiagTrmtItem.DiagnosisFinal += DiagnosisFinalNew;
                    }
                    else
                    {
                        DiagTrmtItem.DiagnosisFinal += "; " + DiagnosisFinalNew;
                    }
                }
                DiagnosisFinalOld = ObjectCopier.DeepCopy(DiagnosisFinalNew);
            }

        }
        private bool CheckExists(DiagnosisIcd10Items Item, bool HasMessage = true)
        {
            int i = 0;
            if (Item.DiseasesReference == null)
            {
                return true;
            }
            foreach (DiagnosisIcd10Items p in refICD10List)
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
            }
        }
        private void GetAllLookupValuesByType()
        {
            ObservableCollection<Lookup> DiagICDSttLookupList = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_DiagIcdStatus).ToObservableCollection();

            if (DiagICDSttLookupList == null || DiagICDSttLookupList.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0751_G1_Msg_InfoKhTimThayStatusICD11, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            DiagIcdStatusList = DiagICDSttLookupList;

        }
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
            if (IsCode)
            {
                e.Cancel = true;
                LoadRefDiseases(e.Parameter, 0, 0, 100);
            }
        }


        public void aucICD10Name_Populating(object sender, PopulatingEventArgs e)
        {
            if (!IsCode && ColumnIndex == 1)
            {
                e.Cancel = true;
                LoadRefDiseases(e.Parameter, 1, 0, 100);
            }
        }
        public void LoadRefDiseases(string name, byte type, int PageIndex, int PageSize)
        {
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
                                refICD10Code.Clear();
                                if (results != null)
                                {
                                    foreach (DiseasesReference p in results)
                                    {
                                        refICD10Code.Add(p);
                                    }
                                }
                                if (refICD10Code.Count > 0)
                                {
                                    this.grdConsultation.bIcd10CodeAcbPopulated = true;
                                }
                                Acb_ICD10_Code.ItemsSource = refICD10Code;
                                Acb_ICD10_Code.PopulateComplete();
                            }
                            else
                            {
                                refICD10Name.Clear();
                                if (results != null)
                                {
                                    foreach (DiseasesReference p in results)
                                    {
                                        refICD10Name.Add(p);
                                    }
                                }
                                if (refICD10Code.Count > 0)
                                {
                                    this.grdConsultation.bIcd10CodeAcbPopulated = true;
                                }
                                Acb_ICD10_Name.ItemsSource = refICD10Name;
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
            isDiseaseDropDown = true;
        }

        public void DiseaseName_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (!isDiseaseDropDown)
            {
                return;
            }
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
        private bool CheckCountIsMain()
        {
            ObservableCollection<DiagnosisIcd10Items> temp = refICD10List.Where(x => x.DiseasesReference != null).ToObservableCollection();
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
        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (refIDC10Item == null
                || refIDC10Item.DiseasesReference == null)
            {
                MessageBox.Show(eHCMSResources.Z0405_G1_KgTheXoaDongRong);
                return;
            }

            int nSelIndex = grdConsultation.SelectedIndex;
            if (nSelIndex >= refICD10List.Count - 1)
            {
                MessageBox.Show(eHCMSResources.Z0405_G1_KgTheXoaDongRong);
                return;
            }

            var item = refICD10List[nSelIndex];

            if (item != null && item.ICD10Code != null && item.ICD10Code != "")
            {
                if (MessageBox.Show(eHCMSResources.Z0419_G1_CoMuonXoaMaICD10, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    if (item.DiseasesReference != null
                        && item.DiseasesReference.DiseaseNameVN != "")
                    {
                        DiagTrmtItem.DiagnosisFinal = DiagTrmtItem.DiagnosisFinal.Replace(item.DiseasesReference.DiseaseNameVN, "");
                    }
                    refICD10List.Remove(refICD10List[nSelIndex]);
                }
            }
        }
        public void DeleteCmd_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSurgeryDoctor != null)
            {
                SurgeryDoctorCollection.Remove(CurrentSurgeryDoctor);
            }
        }
        #endregion

        #region Get All Data of Patient
        public void CallSetPatientInfoForConsultation(PatientRegistrationDetail aRegistrationDetail)
        {
            if (aRegistrationDetail == null)
            {
                return;
            }
            Coroutine.BeginExecute(SetPatientInfoForConsultation(aRegistrationDetail));
        }
        private IEnumerator<IResult> SetPatientInfoForConsultation(PatientRegistrationDetail aRegistrationDetail)
        {
            if (aRegistrationDetail == null)
            {
                aRegistrationDetail = new PatientRegistrationDetail();
            }
            Registration_DataStorage = new Registration_DataStorage();
            yield return GenericCoRoutineTask.StartTask(InitPhyExamAction, aRegistrationDetail.PatientRegistration);
            yield return GenericCoRoutineTask.StartTask(PatientServiceRecordsGetForKhamBenh, aRegistrationDetail.PatientRegistration);
            yield return GenericCoRoutineTask.StartTask(GetRegistrationAction, aRegistrationDetail);
            yield return GenericCoRoutineTask.StartTask(GetDiagnosysConsultationSummaryAction, aRegistrationDetail.PatientRegistration);
        }
        public void CallSetInPatientInfoForConsultation(PatientRegistration aRegistration, PatientRegistrationDetail aRegistrationDetail)
        {
            if (aRegistration == null)
            {
                return;
            }
            Coroutine.BeginExecute(SetInPatientInfoForConsultation(aRegistration, aRegistrationDetail));
        }
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
            yield return GenericCoRoutineTask.StartTask(GetDiagnosysConsultationSummaryAction, aRegistration);
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
        public void PatientServiceRecordsGetForKhamBenh(GenericCoRoutineTask genTask, object ObjPtRegistration)
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
                        contract.BeginPatientServiceRecordsGetForKhamBenh(psrSearch,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var psr = contract.EndPatientServiceRecordsGetForKhamBenh(asyncResult);
                                Registration_DataStorage.PatientServiceRecordCollection = new ObservableCollection<PatientServiceRecord>(psr);
                            }
                            catch (FaultException<AxException> ex)
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

        private void GetRegistrationAction(GenericCoRoutineTask aGenTask, object aPatientRegistrationDetail)
        {
            this.ShowBusyIndicator();
            LoadRegistrationSwitch CurrentSwitch = new LoadRegistrationSwitch();
            var t = new Thread(() =>
            {
                try
                {


                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        bool mIsContinue = true;
                        var aContract = serviceFactory.ServiceInstance;
                        aContract.BeginGetRegistrationInfo((long)((PatientRegistrationDetail)aPatientRegistrationDetail).PtRegistrationID,
                            (int)AllLookupValues.PatientFindBy.NGOAITRU, false, false, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                PatientRegistration mRegistration = aContract.EndGetRegistrationInfo(asyncResult);
                                mRegistration.Patient.CurrentHealthInsurance = mRegistration.HealthInsurance;
                                Registration_DataStorage.CurrentPatientRegistration = mRegistration;
                                Registration_DataStorage.CurrentPatientRegistrationDetail = (aPatientRegistrationDetail as PatientRegistrationDetail);
                                UCPatientProfileInfo.InitData();
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

            t.Start();
        }

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
                        aContract.BeginGetRegistrationInfo_InPt(((PatientRegistration)ObjPtRegistration).PtRegistrationID, (int)Globals.PatientFindBy_ForConsultation.Value, CurrentSwitch, false, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                PatientRegistration mRegistration = aContract.EndGetRegistrationInfo_InPt(asyncResult);
                                mRegistration.Patient.CurrentHealthInsurance = mRegistration.HealthInsurance;
                                Registration_DataStorage.CurrentPatientRegistration = mRegistration;
                                Registration_DataStorage.CurrentPatientRegistrationDetail = (aPatientRegistrationDetail as PatientRegistrationDetail);
                                UCPatientProfileInfo.InitData();
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

        private void GetDiagnosysConsultationSummaryAction(GenericCoRoutineTask aGenTask, object ObjPtRegistration)
        {
            this.ShowBusyIndicator();
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
                        bool mIsContinue = true;
                        var aContract = aFactory.ServiceInstance;
                        aContract.BeginGetRegistrationInfo_InPt(((PatientRegistration)ObjPtRegistration).PtRegistrationID, FindBy, CurrentSwitch, false, Globals.DispatchCallback((asyncResult) =>
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
                                DiagnosysConsultationCollection = mRegistration.DiagnosysConsultationCollection;
                                gDiagConsultationCopier = mRegistration.DiagnosysConsultation.DeepCopy();
                                setDataForViewWhenReload(mRegistration.DiagnosysConsultation, mRegistration.DiagnosysConsultation.StaffList, mRegistration.DiagnosysConsultation.ICD10List);
                                if (mRegistration.DiagnosysConsultation.StaffList.Count == 0 && mRegistration.DiagnosysConsultation.ICD10List.Count == 0)
                                {
                                    if (cboDiagnosysConsultation != null)
                                    {
                                        cboDiagnosysConsultation.SelectedIndex = 0;
                                    }
                                    setValueForEnableButton(1);
                                }
                                else
                                {
                                    setValueForEnableButton(2);
                                }
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

        #endregion

        #region Method
        public void getDataToAddorUpdateDiagConsultationSummary()
        {
            if (gDiagConsultation.DiagConsultationSummaryID > 0)
            {
                gDiagConsultation.ModifiedDate = Globals.GetCurServerDateTime();
                gDiagConsultation.ModifiedStaffID = Globals.LoggedUserAccount.Staff.StaffID;
                //gDiagConsultation.ConsultationDate = ConsultationDate;
                gDiagConsultation.ConsultationDate = ConsultationDateTime.DateTime.Value;
                isUpdate = true;
            }
            else
            {
                gDiagConsultation.RecCreateDate = Globals.GetCurServerDateTime();
                //gDiagConsultation.ConsultationDate = ConsultationDate;
                gDiagConsultation.ConsultationDate = ConsultationDateTime.DateTime.Value;
                gDiagConsultation.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
                gDiagConsultation.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
                gDiagConsultation.PatientID = Registration_DataStorage.CurrentPatient.PatientID;
                isUpdate = false;
            }

            if (Globals.PatientFindBy_ForConsultation.Value == AllLookupValues.PatientFindBy.NOITRU)
            {
                gDiagConsultation.V_RegistrationType = (long)AllLookupValues.RegistrationType.NOI_TRU;
            }
            else
            {
                gDiagConsultation.V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU;
            }
            gDiagConsultation.ConsultationDiagnosis = DiagTrmtItem.DiagnosisFinal;
            gDiagConsultation.ConsultationResult = ConsultationResult;
            gDiagConsultation.ConsultationSummary = ConsultationSummary;
            gDiagConsultation.ConsultationTreatment = ConsultationTreatment;
            gDiagConsultation.PresiderStaffID = SelectedPresiderStaff.StaffID;
            gDiagConsultation.SecretaryStaffID = SelectedSecretaryStaff.StaffID;
        }
        public void btnSaveConsultationcmd()
        {
            if (Registration_DataStorage == null)
            {
                return;
            }
            if (!CheckDataBeforeAddorUpdate())
            {
                return;
            }
            getDataToAddorUpdateDiagConsultationSummary();
            if (refICD10List != null && refICD10List.Count > 1)
            {
                ObservableCollection<DiagnosisIcd10Items> tmprefICD10List = new ObservableCollection<DiagnosisIcd10Items>();
                foreach (var tmp in refICD10List)
                {
                    if (!string.IsNullOrEmpty(tmp.ICD10Code))
                    {
                        tmprefICD10List.Add(tmp);
                    }
                }
                refICD10List = tmprefICD10List;
            }
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginAddUpdateDiagnosysConsultation(gDiagConsultation, SurgeryDoctorCollection.ToList(), refICD10List.ToList(), isUpdate, Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    long DiagConsultationSummaryID = 0;
                                    bool bOK = contract.EndAddUpdateDiagnosysConsultation(out DiagConsultationSummaryID, asyncResult);
                                    if (bOK)
                                    {
                                        MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                                        //reLoadDiagConsultation(DiagConsultationSummaryID);
                                        Coroutine.BeginExecute(reLoadDiagnosysConsultation(Registration_DataStorage.CurrentPatientRegistration));
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong);
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
            t.Start();
        }
        public void reLoadDiagConsultation(long DiagConsultationSummaryID, bool IsLastItem = true)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginLoadDiagnosysConsultationSummary(DiagConsultationSummaryID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            List<Staff> StaffList = new List<Staff>();
                            List<DiagnosisIcd10Items> ICD10List = new List<DiagnosisIcd10Items>();
                            DiagnosysConsultationSummary reDiagnosysConsultationSummary = contract.EndLoadDiagnosysConsultationSummary(out StaffList, out ICD10List, asyncResult);
                            if (reDiagnosysConsultationSummary != null)
                            {
                                gDiagConsultationCopier = reDiagnosysConsultationSummary;
                                gDiagConsultationCopier.StaffList = StaffList;
                                gDiagConsultationCopier.ICD10List = ICD10List;
                                setDataForViewWhenReload(reDiagnosysConsultationSummary, StaffList, ICD10List);
                            }
                            if (IsLastItem)
                            {
                                setValueForEnableButton(2);
                            }
                            else
                            {
                                setValueForEnableButton(5);
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
            t.Start();
        }
        public void setDataForViewWhenReload(DiagnosysConsultationSummary DiagnosysConsultationSummary, List<Staff> StaffList, List<DiagnosisIcd10Items> ICD10List)
        {
            gDiagConsultation.DiagConsultationSummaryID = DiagnosysConsultationSummary.DiagConsultationSummaryID;
            gDiagConsultation.RecCreateDate = DiagnosysConsultationSummary.RecCreateDate;
            gDiagConsultation.V_DiagnosysConsultation = DiagnosysConsultationSummary.V_DiagnosysConsultation;
            gDiagConsultation.Title = DiagnosysConsultationSummary.Title;
            gDiagConsultation.V_RegistrationType = DiagnosysConsultationSummary.V_RegistrationType;
            if (DiagnosysConsultationSummary.ConsultationDate.Year == DateTime.MinValue.Year)
            {
                //ConsultationDate = Globals.GetCurServerDateTime();
                ConsultationDateTime.DateTime = Globals.GetCurServerDateTime();
            }
            else
            {
                ConsultationDateTime.DateTime = DiagnosysConsultationSummary.ConsultationDate;
                //ConsultationDate = DiagnosysConsultationSummary.ConsultationDate;
            }
            DiagTrmtItem.DiagnosisFinal = DiagnosysConsultationSummary.ConsultationDiagnosis;
            SurgeryDoctorCollection = StaffList.ToObservableCollection();
            ConsultationResult = DiagnosysConsultationSummary.ConsultationResult;
            ConsultationTreatment = DiagnosysConsultationSummary.ConsultationTreatment;
            ConsultationSummary = DiagnosysConsultationSummary.ConsultationSummary;
            SelectedPresiderStaff = DoctorStaffs.Where(x => x.StaffID == DiagnosysConsultationSummary.PresiderStaffID).FirstOrDefault();
            SelectedSecretaryStaff = DoctorStaffs.Where(x => x.StaffID == DiagnosysConsultationSummary.SecretaryStaffID).FirstOrDefault();
          
            refICD10List = ICD10List.ToObservableCollection();
        }
        public void btnCreateNew()
        {
            refICD10Code = new ObservableCollection<DiseasesReference>();
            refICD10Name = new ObservableCollection<DiseasesReference>();
            refICD10List = new ObservableCollection<DiagnosisIcd10Items>();
            DiagTrmtItem = new DiagnosisTreatment();
            gDiagConsultation = new DiagnosysConsultationSummary();
            SurgeryDoctorCollection = new ObservableCollection<Staff>();
            //ConsultationDate = Globals.GetCurServerDateTime();
            SelectedSurgeryDoctor = new Staff();
            SelectedPresiderStaff = new Staff();
            SelectedSecretaryStaff = new Staff();
            ConsultationSummary = "";
            ConsultationTreatment = "";
            ConsultationResult = "";
            ConsultationDateTime.DateTime = Globals.GetCurServerDateTime();
            setValueForEnableButton(3);
        }
        public void btnEdit()
        {
            setValueForEnableButton(3);
        }
        public void btnIgnore()
        {
            if (gDiagConsultationCopier == null)
            {
                return;
            }
            setDataForViewWhenReload(gDiagConsultationCopier, gDiagConsultationCopier.StaffList, gDiagConsultationCopier.ICD10List);
            setValueForEnableButton(4);
        }

        /*
         * 1: Tạo mới: Nút tạo ẩn, lưu hiện, edit ẩn, bỏ qua ẩn.
         * 2: Có dữ liệu cũ: Nút tạo mới hiện, lưu ẩn, edit hiện, bỏ qua ẩn.
         * 3: Click vào chỉnh sửa: Tạo mới ẩn, lưu hiện, bỏ qua hiện.
         * 4: Click vào nút bỏ qua: Tạo mới hiện, lưu ẩn, bỏ qua ẩn, chỉnh sửa hiện.
         * 5: Không cho làm bất cứ thứ gì.
         */

        private void setValueForEnableButton(long Mode)
        {
            switch (Mode)
            {
                case 1:
                    IsNotLockView = true;
                    IsEdit = false;
                    IsIgnore = false;
                    IsCreateNew = false;
                    AddBlankRow();
                    break;
                case 2:
                    IsNotLockView = false;
                    IsEdit = true;
                    IsIgnore = false;
                    IsCreateNew = true;
                    break;
                case 3:
                    IsNotLockView = true;
                    IsEdit = false;
                    IsIgnore = true;
                    IsCreateNew = false;
                    AddBlankRow();
                    break;
                case 4:
                    IsNotLockView = false;
                    IsEdit = true;
                    IsIgnore = false;
                    IsCreateNew = true;
                    break;
                case 5:
                    IsNotLockView = false;
                    IsEdit = false;
                    IsIgnore = false;
                    IsCreateNew = false;
                    break;
                default:
                    IsNotLockView = true;
                    IsEdit = true;
                    IsIgnore = true;
                    IsCreateNew = true;
                    AddBlankRow();
                    break;
            }
        }
        //▼===== 20191012 TTM: Bổ sung thêm điều kiện trước khi lưu.
        private bool CheckDataBeforeAddorUpdate()
        {
            if (refICD10List == null)
            {
                MessageBox.Show(eHCMSResources.Z2875_G1_KhongDanhSachICD);
                return false;
            }
            if (refICD10List.Count == 0)
            {
                return false;
            }
            if (ConsultationDateTime.DateTime.Value.Year == DateTime.MinValue.Year)
            {
                MessageBox.Show(eHCMSResources.A0793_G1_Msg_InfoNgKhHopLe);
                return false;
            }
            if (SurgeryDoctorCollection == null)
            {
                MessageBox.Show(eHCMSResources.A0376_G1_Msg_InfoChuaChonBS);
                return false;
            }
            if (SurgeryDoctorCollection.Count == 0)
            {
                MessageBox.Show(eHCMSResources.A0376_G1_Msg_InfoChuaChonBS);
                return false;
            }
            if (DiagTrmtItem == null)
            {
                MessageBox.Show(eHCMSResources.K0420_G1_NhapCDoan);
                return false;
            }
            if (string.IsNullOrEmpty(DiagTrmtItem.DiagnosisFinal))
            {
                MessageBox.Show(eHCMSResources.K0420_G1_NhapCDoan);
                return false;
            }
            if (string.IsNullOrEmpty(ConsultationResult))
            {
                MessageBox.Show(eHCMSResources.Z2876_G1_KetLuanHoiChanDeTrong);
                return false;
            }
            //if (!(CheckedIsMain() && NeedICD10()))
            //{
            //    return false;
            //}
            if (!CheckConsultatingDate())
            {
                return false;
            }
            if (string.IsNullOrEmpty(gDiagConsultation.Title))
            {
                MessageBox.Show(eHCMSResources.Z2879_G1_NhapTieuDe);
                return false;
            }
            if (string.IsNullOrEmpty(ConsultationSummary))
            {
                MessageBox.Show("Chưa nhập Tóm tắt quá trình diễn biến, qtrinh điều trị và chăm sóc người bệnh");
                return false;
            }
            if (string.IsNullOrEmpty(ConsultationTreatment))
            {
                MessageBox.Show("Chưa nhập Hướng điều trị");
                return false;
            }
            if (SelectedPresiderStaff == null)
            {
                MessageBox.Show("Chưa chọn chủ tọa");
                return false;
            }
            if (SelectedSecretaryStaff == null)
            {
                MessageBox.Show("Chưa chọn thư ký");
                return false;
            }
            return true;
        }
        //▲===== 
        #endregion
        private bool NeedICD10()
        {
            //if (Globals.ConfigList != null && Convert.ToInt16(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.NeedICD10]) > 0)

            // Txd 25/05/2014 Replaced ConfigList
            if (Globals.ServerConfigSection.Hospitals.NeedICD10 > 0)
            {
                if (refICD10List != null)
                {
                    var temp = refICD10List.Where(x => x.DiseasesReference != null);
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
            {
                return true;
            }

        }
        private bool CheckedIsMain()
        {
            ObservableCollection<DiagnosisIcd10Items> temp = refICD10List.Where(x => x.DiseasesReference != null).ToObservableCollection();
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

        public bool CheckConsultatingDate()
        {
            if (Globals.PatientFindBy_ForConsultation == (long)AllLookupValues.PatientFindBy.NGOAITRU)
            {
                DateTime ExamDate = Registration_DataStorage.CurrentPatientRegistration.ExamDate;
                //if (ConsultationDate.Date < ExamDate.Date)
                if (ConsultationDateTime.DateTime.Value.Date < ExamDate.Date)
                {
                    MessageBox.Show(eHCMSResources.Z2891_G1_NgayHoiChan2);
                    return false;
                }
            }
            else
            {
                DateTime? AdmissionDate = Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.AdmissionDate;
                //if (ConsultationDate.Date < AdmissionDate.Value.Date)
                if (ConsultationDateTime.DateTime.Value.Date < AdmissionDate.Value.Date)
                {
                    MessageBox.Show(eHCMSResources.Z2890_G1_NgayHoiChan1);
                    return false;
                }
            }

            return true;
        }
        public void LoadV_DiagnosysConsultation()
        {
            var t = new Thread(() =>
            {

                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllLookupValuesByType(LookupValues.V_DiagnosysConsultationType,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);

                                    DiagnosysConsultationType = new ObservableCollection<Lookup>(allItems);
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
        ComboBox cboDiagnosysConsultation = null;
        public void cboV_DiagnosysConsultation_Loaded(object sender)
        {
            cboDiagnosysConsultation = sender as ComboBox;
        }

        DataGrid grdDiagConsul;
        public void grdDiagConsul_Loaded(object sender, RoutedEventArgs e)
        {
            grdDiagConsul = sender as DataGrid;
        }
        public void grdDiagConsul_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (grdDiagConsul != null)
            {
                if (grdDiagConsul.SelectedItem != null)
                {
                    bool IsLastItem = false;
                    if (grdDiagConsul.SelectedIndex == 0)
                    {
                        IsLastItem = true;
                    }
                    reLoadDiagConsultation((grdDiagConsul.SelectedItem as DiagnosysConsultationSummary).DiagConsultationSummaryID, IsLastItem);
                }
            }
        }
        public void grdDiagConsul_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }
        public void grdDoctor_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        private IEnumerator<IResult> reLoadDiagnosysConsultation(PatientRegistration aRegistration)
        {
            yield return GenericCoRoutineTask.StartTask(GetDiagnosysConsultationSummaryAction, aRegistration);
        }
        private IMinHourDateControl _ConsultationDateTime;
        public IMinHourDateControl ConsultationDateTime
        {
            get { return _ConsultationDateTime; }
            set
            {
                _ConsultationDateTime = value;
                NotifyOfPropertyChange(() => ConsultationDateTime);
            }
        }
        public void btnPrintcmd()
        {
            if (gDiagConsultation == null)
            {
                return;
            }
            if(gDiagConsultation.DiagConsultationSummaryID == 0)
            {
                return;
            }
            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
            {
                proAlloc.ID = gDiagConsultation.DiagConsultationSummaryID;
                proAlloc.V_RegistrationType = gDiagConsultation.V_RegistrationType;
                proAlloc.eItem = ReportName.XRpt_BienBanHoiChan;
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }

    }
}