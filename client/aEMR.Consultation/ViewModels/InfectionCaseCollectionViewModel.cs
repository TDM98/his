using aEMR.Common.BaseModel;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using aEMR.Common.Collections;
using System.Linq;
using System.Windows.Controls;
using System.Windows;
using aEMR.Controls;
using System.Collections.Generic;
using aEMR.CommonTasks;
using System.Windows.Input;
using aEMR.Common;
using System.ComponentModel;
using System.Windows.Data;
using aEMR.Common.ViewModels;
/*
* 20191014 #001 TTM:   BM 0018461: [Điều trị nhiễm khuẩn] Bổ sung OST cho màn hình điều trị nhiễm khuẩn.
*/
namespace aEMR.Consultation.ViewModels
{
    [Export(typeof(IInfectionCaseCollection)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InfectionCaseCollectionViewModel : ViewModelBase, IInfectionCaseCollection
        , IHandle<RegistrationSelectedForConsultation_K1>
        , IHandle<RegistrationSelectedForConsultation_K2>
        , IHandle<InPatientRegistrationSelectedForConsultation>
        , IHandle<InPatientRegistrationSelectedForInfectionCase>
    {
        #region Properties
        public ISearchPatientAndRegistration UCSearchRegistration { get; set; }
        public IPatientInfo UCPatientProfileInfo { get; set; }
        private ICollectionView _InfectionCaseView;
        public ICollectionView InfectionCaseView
        {
            get
            {
                return _InfectionCaseView;
            }
            set
            {
                if (_InfectionCaseView == value)
                {
                    return;
                }
                _InfectionCaseView = value;
                NotifyOfPropertyChange(() => InfectionCaseView);
            }
        }
        private ObservableCollection<InfectionCase> _InfectionCaseCollection;
        public ObservableCollection<InfectionCase> InfectionCaseCollection
        {
            get
            {
                return _InfectionCaseCollection;
            }
            set
            {
                if (_InfectionCaseCollection == value)
                {
                    return;
                }
                _InfectionCaseCollection = value;
                NotifyOfPropertyChange(() => InfectionCaseCollection);
                InfectionCaseView = CollectionViewSource.GetDefaultView(InfectionCaseCollection);
                InfectionCaseView.SortDescriptions.Add(new SortDescription("PtRegistrationID", ListSortDirection.Descending));
                InfectionCaseView.GroupDescriptions.Add(new PropertyGroupDescription("CurrentRegistration.RefDepartment"));
            }
        }
        private InfectionCase _CurrentInfectionCase;
        public InfectionCase CurrentInfectionCase
        {
            get
            {
                return _CurrentInfectionCase;
            }
            set
            {
                if (_CurrentInfectionCase == value)
                {
                    return;
                }
                _CurrentInfectionCase = value;
                NotifyOfPropertyChange(() => CurrentInfectionCase);
                NotifyOfPropertyChange(() => EditButtonContent);
                NotifyOfPropertyChange(() => GenerateNewButtonString);
                SelectedInfectionVirus = CurrentInfectionCase == null || InfectionVirusCollection == null ? null : InfectionVirusCollection.FirstOrDefault(x => x.InfectionVirusID == CurrentInfectionCase.InfectedByVirusID);
            }
        }
        private ObservableCollection<DiagnosisIcd10Items> _IDC10ListCollection1 = new ObservableCollection<DiagnosisIcd10Items>();
        public ObservableCollection<DiagnosisIcd10Items> IDC10ListCollection1
        {
            get
            {
                return _IDC10ListCollection1;
            }
            set
            {
                if (_IDC10ListCollection1 == value)
                {
                    return;
                }
                _IDC10ListCollection1 = value;
                NotifyOfPropertyChange(() => IDC10ListCollection1);
            }
        }
        private ObservableCollection<DiagnosisIcd10Items> _IDC10ListCollection2 = new ObservableCollection<DiagnosisIcd10Items>();
        public ObservableCollection<DiagnosisIcd10Items> IDC10ListCollection2
        {
            get
            {
                return _IDC10ListCollection2;
            }
            set
            {
                if (_IDC10ListCollection2 == value)
                {
                    return;
                }
                _IDC10ListCollection2 = value;
                NotifyOfPropertyChange(() => IDC10ListCollection2);
            }
        }
        private DiagnosisIcd10Items _SelectedICD101;
        public DiagnosisIcd10Items SelectedICD101
        {
            get
            {
                return _SelectedICD101;
            }
            set
            {
                if (_SelectedICD101 == value)
                {
                    return;
                }
                _SelectedICD101 = value;
                NotifyOfPropertyChange(() => SelectedICD101);
            }
        }
        private DiagnosisIcd10Items _SelectedICD102;
        public DiagnosisIcd10Items SelectedICD102
        {
            get
            {
                return _SelectedICD102;
            }
            set
            {
                if (_SelectedICD102 == value)
                {
                    return;
                }
                _SelectedICD102 = value;
                NotifyOfPropertyChange(() => SelectedICD102);
            }
        }
        private ObservableCollection<Lookup> _InfectionTypeCollection;
        public ObservableCollection<Lookup> InfectionTypeCollection
        {
            get
            {
                return _InfectionTypeCollection;
            }
            set
            {
                if (_InfectionTypeCollection == value)
                {
                    return;
                }
                _InfectionTypeCollection = value;
                NotifyOfPropertyChange(() => InfectionTypeCollection);
            }
        }
        private ObservableCollection<Lookup> _BloodSmearResultCollection;
        public ObservableCollection<Lookup> BloodSmearResultCollection
        {
            get
            {
                return _BloodSmearResultCollection;
            }
            set
            {
                if (_BloodSmearResultCollection == value)
                {
                    return;
                }
                _BloodSmearResultCollection = value;
                NotifyOfPropertyChange(() => BloodSmearResultCollection);
            }
        }
        private ObservableCollection<Lookup> _BloodSmearMethodCollection;
        public ObservableCollection<Lookup> BloodSmearMethodCollection
        {
            get
            {
                return _BloodSmearMethodCollection;
            }
            set
            {
                if (_BloodSmearMethodCollection == value)
                {
                    return;
                }
                _BloodSmearMethodCollection = value;
                NotifyOfPropertyChange(() => BloodSmearMethodCollection);
            }
        }
        private ObservableCollection<InfectionVirus> _InfectionVirusCollection;
        public ObservableCollection<InfectionVirus> InfectionVirusCollection
        {
            get
            {
                return _InfectionVirusCollection;
            }
            set
            {
                if (_InfectionVirusCollection == value)
                {
                    return;
                }
                _InfectionVirusCollection = value;
                NotifyOfPropertyChange(() => InfectionVirusCollection);
            }
        }
        private InfectionVirus _SelectedInfectionVirus;
        public InfectionVirus SelectedInfectionVirus
        {
            get
            {
                return _SelectedInfectionVirus;
            }
            set
            {
                if (_SelectedInfectionVirus == value)
                {
                    return;
                }
                _SelectedInfectionVirus = value;
                NotifyOfPropertyChange(() => SelectedInfectionVirus);
            }
        }
        private bool isDropDown = false;
        private int ICDIndex = 1;
        public string EditButtonContent
        {
            get
            {
                return CurrentInfectionCase != null && CurrentInfectionCase.InfectionCaseID > 0 ? eHCMSResources.K1599_G1_CNhat : eHCMSResources.T2937_G1_Luu;
            }
        }
        private PatientRegistration CurrentRegistration { get; set; }
        private bool _IsDialogViewObject = false;
        public bool IsDialogViewObject
        {
            get
            {
                return _IsDialogViewObject;
            }
            set
            {
                if (_IsDialogViewObject == value)
                {
                    return;
                }
                _IsDialogViewObject = value;
                NotifyOfPropertyChange(() => IsDialogViewObject);
            }
        }
        public bool IsUpdatedCompleted { get; set; } = false;
        public string GenerateNewButtonString
        {
            get
            {
                return CurrentInfectionCase == null || InfectionCaseCollection == null || InfectionCaseCollection.Count == 0 ||
                    CurrentRegistration == null ||
                    !InfectionCaseCollection.Any(x => x.PtRegistrationID == CurrentRegistration.PtRegistrationID) ||
                    CurrentInfectionCase.InfectionCaseID == InfectionCaseCollection.Where(x => x.PtRegistrationID == CurrentRegistration.PtRegistrationID).OrderByDescending(x => x.InfectionCaseID).First().InfectionCaseID ? eHCMSResources.Z2136_G1_TaoMoiDuaTrenCu : eHCMSResources.K1305_G1_BoQua;
            }
        }
        #endregion
        #region Events
        [ImportingConstructor]
        public InfectionCaseCollectionViewModel()
        {
            GetInfectionVirusCollection();

            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NOITRU;

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

            UCPatientProfileInfo = Globals.GetViewModel<IPatientInfo>();

            InfectionTypeCollection = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_InfectionType).ToObservableCollection();
            BloodSmearResultCollection = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_BloodSmearResult).ToObservableCollection();
            BloodSmearMethodCollection = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_BloodSmearMethod).ToObservableCollection();
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            ActivateItem(UCPatientProfileInfo);
            IsDialogViewObject = this.IsDialogView;
            //▼===== #001
            IInPatientOutstandingTask OutstandingTaskContent = Globals.GetViewModel<IInPatientOutstandingTask>();
            OutstandingTaskContent.WhichVM = SetOutStandingTask.DIEU_TRI_NHIEM_KHUAN;
            var homevm = Globals.GetViewModel<IHome>();
            homevm.OutstandingTaskContent = OutstandingTaskContent;
            homevm.IsExpandOST = true;
            //▲===== #001
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
            DeactivateItem(UCPatientProfileInfo, close);
            //▼===== #001
            var homevm = Globals.GetViewModel<IHome>();
            homevm.OutstandingTaskContent = null;
            homevm.IsExpandOST = false;
            //▲===== #001
        }
        public void Handle(RegistrationSelectedForConsultation_K1 message)
        {
            if (message != null)
            {
                CurrentRegistration = message.Source;
            }
            else
            {
                CurrentRegistration = null;
            }
            if (CurrentRegistration == null && CurrentRegistration.PtRegistrationID == 0)
            {
                UCPatientProfileInfo.CurrentPatient = null;
                GetInfectionCaseByPtID(null, null);
                return;
            }
            UCPatientProfileInfo.CurrentPatient = CurrentRegistration.Patient;
            Coroutine.BeginExecute(DoOpenRegistration(CurrentRegistration.PtRegistrationID));
        }
        public void Handle(RegistrationSelectedForConsultation_K2 message)
        {
            if (message != null)
            {
                CurrentRegistration = message.Source;
            }
            else
            {
                CurrentRegistration = null;
            }
            if (CurrentRegistration == null && CurrentRegistration.PtRegistrationID == 0)
            {
                UCPatientProfileInfo.CurrentPatient = null;
                GetInfectionCaseByPtID(null, null);
                return;
            }
            UCPatientProfileInfo.CurrentPatient = CurrentRegistration.Patient;
            Coroutine.BeginExecute(DoOpenRegistration(CurrentRegistration.PtRegistrationID));
        }
        public void Handle(InPatientRegistrationSelectedForConsultation message)
        {
            if (message != null)
            {
                CurrentRegistration = message.Source;
            }
            else
            {
                CurrentRegistration = null;
            }
            if (CurrentRegistration == null && CurrentRegistration.PtRegistrationID == 0)
            {
                UCPatientProfileInfo.CurrentPatient = null;
                GetInfectionCaseByPtID(null, null);
                return;
            }
            UCPatientProfileInfo.CurrentPatient = CurrentRegistration.Patient;
            Coroutine.BeginExecute(DoOpenRegistration(CurrentRegistration.PtRegistrationID));
        }
        //▼===== #001
        public void Handle(InPatientRegistrationSelectedForInfectionCase message)
        {
            if (message != null)
            {
                CurrentRegistration = message.Source;
            }
            else
            {
                CurrentRegistration = null;
            }
            if (CurrentRegistration == null && CurrentRegistration.PtRegistrationID == 0)
            {
                UCPatientProfileInfo.CurrentPatient = null;
                GetInfectionCaseByPtID(null, null);
                return;
            }
            UCPatientProfileInfo.CurrentPatient = CurrentRegistration.Patient;
            Coroutine.BeginExecute(DoOpenRegistration(CurrentRegistration.PtRegistrationID));
        }
        //▲===== #001
        public void ICD10_Populating(object sender, PopulatingEventArgs e)
        {
            e.Cancel = true;
            LoadRefDiseases(sender, e.Parameter, 0, 0, 100);
        }
        public void ICD10_DropDownClosing(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            isDropDown = true;
        }
        public void ICD10_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (!isDropDown)
            {
                return;
            }
            isDropDown = false;
            if ((sender as AxAutoComplete).SelectedItem == null)
            {
                return;
            }
            if (sender != null)
            {
                var CurrentDiseasesReference = new DiseasesReference();
                CurrentDiseasesReference = (sender as AxAutoComplete).SelectedItem as DiseasesReference;
                if (ICDIndex == 1)
                {
                    AddICD10Item(IDC10ListCollection1, SelectedICD101, CurrentDiseasesReference);
                }
                else
                {
                    AddICD10Item(IDC10ListCollection2, SelectedICD102, CurrentDiseasesReference);
                }
            }
        }
        private void AddICD10Item(ObservableCollection<DiagnosisIcd10Items> IDC10ListCollection, DiagnosisIcd10Items SelectedICD10, DiseasesReference CurrentDiseasesReference)
        {
            if (IDC10ListCollection.Any(x => x.ICD10Code == CurrentDiseasesReference.ICD10Code))
            {
                Globals.ShowMessage(eHCMSResources.A0810_G1_Msg_InfoMaICDDaTonTai, eHCMSResources.G0442_G1_TBao);
                return;
            }
            string mDiagnosisFinalOld = null;
            if (SelectedICD10.DiseasesReference != null
                && !string.IsNullOrEmpty(SelectedICD10.DiseasesReference.ICD10Code)
                && !string.IsNullOrEmpty(SelectedICD10.DiseasesReference.DiseaseNameVN))
            {
                mDiagnosisFinalOld = SelectedICD10.DiseasesReference.DiseaseNameVN;
            }
            SelectedICD10.DiseasesReference = CurrentDiseasesReference;
            GetDiagTreatmentFinal(CurrentDiseasesReference, mDiagnosisFinalOld);
            if (!IDC10ListCollection.Any(x => x.IsMain))
            {
                SelectedICD10.IsMain = true;
            }
            if (ICDIndex == 1)
            {
                AddBlankRow1();
            }
            else
            {
                AddBlankRow2();
            }
        }
        public void ICD10Name_Populating(object sender, PopulatingEventArgs e)
        {
            e.Cancel = true;
            LoadRefDiseases(sender, e.Parameter, 1, 0, 100);
        }
        public void ICD1_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            ICDIndex = 1;
        }
        public void ICD2_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            ICDIndex = 2;
        }
        public void DeleteICD10ItemCmd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(((sender as Button).DataContext as DiagnosisIcd10Items).ICD10Code))
            {
                MessageBox.Show(eHCMSResources.Z0405_G1_KgTheXoaDongRong);
                return;
            }
            var mItem = (sender as Button).DataContext as DiagnosisIcd10Items;
            if (mItem != null && mItem.ICD10Code != null && mItem.ICD10Code != "")
            {
                if (MessageBox.Show(eHCMSResources.A0202_G1_Msg_ConfXoaMaICD10, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    if ((sender as Button).CommandParameter.ToString() == "1")
                    {
                        if (mItem.DiseasesReference != null && mItem.DiseasesReference.DiseaseNameVN != "")
                        {
                            CurrentInfectionCase.DiagnosisFinal1 = CurrentInfectionCase.DiagnosisFinal1.Replace(mItem.DiseasesReference.DiseaseNameVN, "");
                        }
                        IDC10ListCollection1.Remove(mItem);
                    }
                    else
                    {
                        if (mItem.DiseasesReference != null && mItem.DiseasesReference.DiseaseNameVN != "")
                        {
                            CurrentInfectionCase.DiagnosisFinal2 = CurrentInfectionCase.DiagnosisFinal2.Replace(mItem.DiseasesReference.DiseaseNameVN, "");
                        }
                        IDC10ListCollection2.Remove(mItem);
                    }
                }
            }
        }
        public void AddAntibioticTreatmentCmd_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentInfectionCase == null || CurrentInfectionCase.InfectionCaseID == 0)
            {
                return;
            }
            IAntibioticTreatmentEdit mDialogView = Globals.GetViewModel<IAntibioticTreatmentEdit>();
            mDialogView.CurrentAntibioticTreatment = new AntibioticTreatment
            {
                PtRegistrationID = CurrentInfectionCase.PtRegistrationID,
                InfectionCaseID = CurrentInfectionCase.InfectionCaseID,
                StartDate = Globals.GetCurServerDateTime(),
                AntibioticTreatmentTitle = SelectedInfectionVirus == null ? "" : SelectedInfectionVirus.InfectionVirusName
            };
            mDialogView.DeptID = CurrentInfectionCase.DeptID;
            mDialogView.PtRegistrationID = CurrentInfectionCase.PtRegistrationID;
            GlobalsNAV.ShowDialog_V3(mDialogView);
            if (mDialogView.IsUpdatedCompleted)
            {
                EditAntibioticTreatment(mDialogView.CurrentAntibioticTreatment);
            }
        }
        public void AntibioticTreatment_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((sender as DataGrid).SelectedItem == null)
            {
                return;
            }
            IAntibioticTreatmentEdit mDialogView = Globals.GetViewModel<IAntibioticTreatmentEdit>();
            mDialogView.CurrentAntibioticTreatment = ((sender as DataGrid).SelectedItem as AntibioticTreatment).DeepCopy();
            mDialogView.DeptID = CurrentInfectionCase.DeptID;
            mDialogView.PtRegistrationID = CurrentInfectionCase.PtRegistrationID;
            GlobalsNAV.ShowDialog_V3(mDialogView);
            if (mDialogView.IsUpdatedCompleted)
            {
                EditAntibioticTreatment(mDialogView.CurrentAntibioticTreatment);
            }
        }
        public void InfectionCase_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((sender as DataGrid).SelectedItem == null)
            {
                return;
            }
            IInfectionCaseCollection mDialogView = Globals.GetViewModel<IInfectionCaseCollection>();
            mDialogView.LoadOldInfectionCase((sender as DataGrid).SelectedItem as InfectionCase);
            GlobalsNAV.ShowDialog_V3(mDialogView, null, null, false, true, Globals.GetDefaultDialogViewSize());
            if (mDialogView.IsUpdatedCompleted)
            {
                Coroutine.BeginExecute(DoOpenRegistration(CurrentRegistration.PtRegistrationID));
            }
        }
        public void btnCreateNew()
        {
            CreateNewCurrentInfectionCase();
        }
        public void btnGenerateNew()
        {
            if (CurrentInfectionCase != null && CurrentInfectionCase.InfectionCaseID > 0)
            {
                CurrentInfectionCase = CurrentInfectionCase.DeepCopy();
                CurrentInfectionCase.InfectionCaseID = 0;
                NotifyOfPropertyChange(() => GenerateNewButtonString);
                CurrentInfectionCase.AntibioticTreatmentCollection = new ObservableCollection<AntibioticTreatment>();
                if (CurrentRegistration == null ||
                    CurrentRegistration.AdmissionInfo == null ||
                    CurrentRegistration.AdmissionInfo.InPatientDeptDetails == null ||
                    !CurrentRegistration.AdmissionInfo.InPatientDeptDetails.Any(x => x.V_InPatientDeptStatus == AllLookupValues.InPatientDeptStatus.NHAP_KHOA_PHONG && x.DeptLocation != null && x.DeptLocation.DeptID > 0))
                {
                    CurrentInfectionCase.DeptID = 0;
                }
                else
                {
                    CurrentInfectionCase.DeptID = CurrentRegistration.AdmissionInfo.InPatientDeptDetails.Where(x => x.V_InPatientDeptStatus == AllLookupValues.InPatientDeptStatus.NHAP_KHOA_PHONG && x.DeptLocation != null && x.DeptLocation.DeptID > 0).First().DeptLocation.DeptID;
                }
                NotifyOfPropertyChange(() => EditButtonContent);
            }
            else if (CurrentInfectionCase != null && CurrentInfectionCase.InfectionCaseID == 0 &&
                InfectionCaseCollection != null && InfectionCaseCollection.Count > 0 &&
                CurrentRegistration != null &&
                InfectionCaseCollection.Any(x => x.PtRegistrationID == CurrentRegistration.PtRegistrationID))
            {
                CurrentInfectionCase = InfectionCaseCollection.Where(x => x.PtRegistrationID == CurrentRegistration.PtRegistrationID).OrderByDescending(x => x.InfectionCaseID).First();
                NotifyOfPropertyChange(() => GenerateNewButtonString);
                GetCurrentInfectionCaseDetails();
            }
        }
        public void btnSave()
        {
            if (CurrentInfectionCase == null)
            {
                return;
            }
            if (CurrentInfectionCase.InfectedByVirusID == 0)
            {
                Globals.ShowMessage(eHCMSResources.Z2861_G2_ThieuNguyenNhanNhiemKhuan, eHCMSResources.T0432_G1_Error);
                return;
            }
            if (IDC10ListCollection1 == null || !IDC10ListCollection1.Any(x => x.DiseasesReference != null && !string.IsNullOrEmpty(x.DiseasesReference.ICD10Code)))
            {
                Globals.ShowMessage(eHCMSResources.Z2861_G2_ThieuChanDoanBanDau, eHCMSResources.T0432_G1_Error);
                return;
            }
            if (CurrentInfectionCase.DeptID == 0)
            {
                Globals.ShowMessage(eHCMSResources.Z2861_G2_BenhNhanChuaDuocNhapKhoa, eHCMSResources.T0432_G1_Error);
                return;
            }
            CurrentInfectionCase.InfectionICD10ListID1Items = IDC10ListCollection1.Where(x => x.DiseasesReference != null && !string.IsNullOrEmpty(x.DiseasesReference.ICD10Code)).Select(x => new InfectionICD10Item { ICD10Code = x.DiseasesReference.ICD10Code, ICD10Name = x.DiseasesReference.DiseaseNameVN, IsMain = x.IsMain }).ToObservableCollection();
            if (IDC10ListCollection2.Any(x => x.DiseasesReference != null && !string.IsNullOrEmpty(x.DiseasesReference.ICD10Code)))
            {
                CurrentInfectionCase.InfectionICD10ListID2Items = IDC10ListCollection2.Where(x => x.DiseasesReference != null && !string.IsNullOrEmpty(x.DiseasesReference.ICD10Code)).Select(x => new InfectionICD10Item { ICD10Code = x.DiseasesReference.ICD10Code, ICD10Name = x.DiseasesReference.DiseaseNameVN, IsMain = x.IsMain }).ToObservableCollection();
            }
            else
            {
                CurrentInfectionCase.InfectionICD10ListID2Items = null;
            }
            if (CurrentInfectionCase.InfectionICD10ListID1Items.Count(x => x.IsMain) != 1 ||
                (CurrentInfectionCase.InfectionICD10ListID2Items != null && CurrentInfectionCase.InfectionICD10ListID2Items.Count(x => x.IsMain) != 1))
            {
                Globals.ShowMessage(eHCMSResources.Z0510_G1_I, eHCMSResources.T0432_G1_Error);
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginEditInfectionCase(CurrentInfectionCase, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                CurrentInfectionCase.InfectionCaseID = contract.EndEditInfectionCase(asyncResult);
                                Globals.ShowMessage(eHCMSResources.Z2715_G1_ThanhCong, eHCMSResources.G0442_G1_TBao);
                                NotifyOfPropertyChange(() => EditButtonContent);
                                if (!IsDialogViewObject)
                                {
                                    Coroutine.BeginExecute(DoOpenRegistration(CurrentRegistration.PtRegistrationID));
                                }
                                else
                                {
                                    IsUpdatedCompleted = true;
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
        public void btnGetPCLDesc()
        {
            if (CurrentRegistration == null || CurrentRegistration.PtRegistrationID == 0)
            {
                return;
            }
            IPCLResultSummary DialogView = Globals.GetViewModel<IPCLResultSummary>();
            DialogView.InitPatientInfo(new Registration_DataStorage { CurrentPatientRegistration = CurrentRegistration });
            GlobalsNAV.ShowDialog_V3(DialogView, null, null, false, true, Globals.GetDefaultDialogViewSize());
            if (DialogView.IsHasDescription && !string.IsNullOrEmpty(DialogView.CurrentDescription))
            {
                if (CurrentInfectionCase == null)
                {
                    return;
                }
                try
                {
                    CurrentInfectionCase.Notes += string.Format("{0}{1}", string.IsNullOrEmpty(CurrentInfectionCase.Notes) ? "" : "\n", DialogView.CurrentDescription);
                }
                catch (Exception ex)
                {
                    GlobalsNAV.ShowMessagePopup(ex.Message);
                }
            }
        }
        public void btnGetAntibioticFromInstruction()
        {
            if (CurrentInfectionCase == null || CurrentInfectionCase.InfectionCaseID == 0)
            {
                return;
            }
            IAntibioticTreatmentCollection DialogView = Globals.GetViewModel<IAntibioticTreatmentCollection>();
            DialogView.CurrentRegistration = CurrentRegistration;
            GlobalsNAV.ShowDialog_V3(DialogView);
            if (DialogView.SelectedAntibioticTreatment != null)
            {
                this.ShowBusyIndicator();
                var CurrentThread = new Thread(() =>
                {
                    try
                    {
                        using (var serviceFactory = new PatientRegistrationServiceClient())
                        {
                            var contract = serviceFactory.ServiceInstance;
                            contract.BeginInsertAntibioticTreatmentIntoInstruction(DialogView.SelectedAntibioticTreatment.AntibioticTreatmentID, CurrentInfectionCase.InfectionCaseID, Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    contract.EndInsertAntibioticTreatmentIntoInstruction(asyncResult);
                                    GetCurrentInfectionCaseDetails();
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
                CurrentThread.Start();
            }
        }
        #endregion
        #region Methods
        public void LoadRefDiseases(object sender, string name, byte type, int PageIndex, int PageSize)
        {
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
                            int Total;
                            var results = contract.EndSearchRefDiseases(out Total, asyncResult);
                            (sender as AxAutoComplete).ItemsSource = results;
                            (sender as AxAutoComplete).PopulateComplete();
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
        private void AddBlankRow1()
        {
            if (IDC10ListCollection1 != null
                && IDC10ListCollection1.LastOrDefault() != null
                && IDC10ListCollection1.LastOrDefault().DiseasesReference == null)
            {
                return;
            }
            DiagnosisIcd10Items mItem = new DiagnosisIcd10Items();
            mItem.V_DiagIcdStatus = (long)AllLookupValues.V_DiagIcdStatus.DANGDIEUTRI;
            mItem.LookupStatus = new Lookup();
            mItem.LookupStatus.LookupID = (long)AllLookupValues.V_DiagIcdStatus.DANGDIEUTRI;
            mItem.LookupStatus.ObjectValue = eHCMSResources.Z0540_G1_DangDTri.ToUpper();
            IDC10ListCollection1.Add(mItem);
        }
        private void AddBlankRow2()
        {
            if (IDC10ListCollection2 != null
                && IDC10ListCollection2.LastOrDefault() != null
                && IDC10ListCollection2.LastOrDefault().DiseasesReference == null)
            {
                return;
            }
            DiagnosisIcd10Items mItem = new DiagnosisIcd10Items();
            mItem.V_DiagIcdStatus = (long)AllLookupValues.V_DiagIcdStatus.DANGDIEUTRI;
            mItem.LookupStatus = new Lookup();
            mItem.LookupStatus.LookupID = (long)AllLookupValues.V_DiagIcdStatus.DANGDIEUTRI;
            mItem.LookupStatus.ObjectValue = eHCMSResources.Z0540_G1_DangDTri.ToUpper();
            IDC10ListCollection2.Add(mItem);
        }
        private void GetDiagTreatmentFinal(DiseasesReference aDiseasesReference, string aDiagnosisFinalOld)
        {
            if (aDiseasesReference != null)
            {
                var DiagnosisFinalNew = aDiseasesReference.DiseaseNameVN;
                if (ICDIndex == 1)
                {
                    if (!string.IsNullOrEmpty(aDiagnosisFinalOld))
                    {
                        CurrentInfectionCase.DiagnosisFinal1 = CurrentInfectionCase.DiagnosisFinal1.Replace(aDiagnosisFinalOld, DiagnosisFinalNew);
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(CurrentInfectionCase.DiagnosisFinal1))
                        {
                            CurrentInfectionCase.DiagnosisFinal1 = DiagnosisFinalNew;
                        }
                        else
                        {
                            CurrentInfectionCase.DiagnosisFinal1 += "- " + DiagnosisFinalNew;
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(aDiagnosisFinalOld))
                    {
                        CurrentInfectionCase.DiagnosisFinal2 = CurrentInfectionCase.DiagnosisFinal2.Replace(aDiagnosisFinalOld, DiagnosisFinalNew);
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(CurrentInfectionCase.DiagnosisFinal2))
                        {
                            CurrentInfectionCase.DiagnosisFinal2 = DiagnosisFinalNew;
                        }
                        else
                        {
                            CurrentInfectionCase.DiagnosisFinal2 += "- " + DiagnosisFinalNew;
                        }
                    }
                }
            }
        }
        private void GetInfectionCaseByPtID(long? aPtRegistrationID, long? aPatientID)
        {
            InfectionCaseCollection = new ObservableCollection<InfectionCase>();
            CurrentInfectionCase = new InfectionCase();
            IDC10ListCollection1 = new ObservableCollection<DiagnosisIcd10Items>();
            IDC10ListCollection2 = new ObservableCollection<DiagnosisIcd10Items>();
            if (aPtRegistrationID.GetValueOrDefault(0) == 0 || aPatientID.GetValueOrDefault(0) == 0)
            {
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetInfectionCaseByPtID(aPatientID.Value, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var GettedCollection = contract.EndGetInfectionCaseByPtID(asyncResult);
                                if (GettedCollection != null)
                                {
                                    InfectionCaseCollection = GettedCollection.ToObservableCollection();
                                }
                                if (InfectionCaseCollection != null && InfectionCaseCollection.Any(x => x.PtRegistrationID == aPtRegistrationID.Value))
                                {
                                    CurrentInfectionCase = InfectionCaseCollection.Where(x => x.PtRegistrationID == aPtRegistrationID.Value).OrderByDescending(x => x.InfectionCaseID).First();
                                    GetCurrentInfectionCaseDetails();
                                }
                                else
                                {
                                    CreateNewCurrentInfectionCase();
                                    GetLatestDiagTrmtByPtID_InPt(aPatientID.Value, null);
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
        private void GetCurrentInfectionCaseDetails()
        {
            if (CurrentInfectionCase == null || CurrentInfectionCase.InfectionCaseID == 0)
            {
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetInfectionCaseDetail(CurrentInfectionCase, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                CurrentInfectionCase = contract.EndGetInfectionCaseDetail(asyncResult);
                                if (CurrentInfectionCase.InfectionICD10ListID1Items != null)
                                {
                                    IDC10ListCollection1 = CurrentInfectionCase.InfectionICD10ListID1Items.Select(x => new DiagnosisIcd10Items { IsMain = x.IsMain, DiseasesReference = new DiseasesReference { ICD10Code = x.ICD10Code, DiseaseNameVN = x.ICD10Name } }).ToObservableCollection();
                                }
                                else
                                {
                                    IDC10ListCollection1 = new ObservableCollection<DiagnosisIcd10Items>();
                                }
                                if (CurrentInfectionCase.InfectionICD10ListID2Items != null)
                                {
                                    IDC10ListCollection2 = CurrentInfectionCase.InfectionICD10ListID2Items.Select(x => new DiagnosisIcd10Items { IsMain = x.IsMain, DiseasesReference = new DiseasesReference { ICD10Code = x.ICD10Code, DiseaseNameVN = x.ICD10Name } }).ToObservableCollection();
                                }
                                else
                                {
                                    IDC10ListCollection2 = new ObservableCollection<DiagnosisIcd10Items>();
                                }
                                AddBlankRow1();
                                AddBlankRow2();
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
        private void CreateNewCurrentInfectionCase()
        {
            CurrentInfectionCase = new InfectionCase();
            NotifyOfPropertyChange(() => EditButtonContent);
            IDC10ListCollection1 = new ObservableCollection<DiagnosisIcd10Items>();
            IDC10ListCollection2 = new ObservableCollection<DiagnosisIcd10Items>();
            CurrentInfectionCase.StartDate = Globals.GetCurServerDateTime();
            AddBlankRow1();
            AddBlankRow2();
            if (CurrentRegistration != null)
            {
                CurrentInfectionCase.PtRegistrationID = CurrentRegistration.PtRegistrationID;
                CurrentInfectionCase.PatientID = CurrentRegistration.PatientID.GetValueOrDefault(0);
            }
            CurrentInfectionCase.V_InfectionType = InfectionTypeCollection.First().LookupID;
            CurrentInfectionCase.V_BloodSmearMethod = BloodSmearMethodCollection.First().LookupID;
            CurrentInfectionCase.V_BloodSmearResult = BloodSmearResultCollection.First().LookupID;
            if (CurrentRegistration == null ||
                CurrentRegistration.AdmissionInfo == null ||
                CurrentRegistration.AdmissionInfo.InPatientDeptDetails == null ||
                !CurrentRegistration.AdmissionInfo.InPatientDeptDetails.Any(x => x.V_InPatientDeptStatus == AllLookupValues.InPatientDeptStatus.NHAP_KHOA_PHONG && x.DeptLocation != null && x.DeptLocation.DeptID > 0))
            {
                CurrentInfectionCase.DeptID = 0;
            }
            else
            {
                CurrentInfectionCase.DeptID = CurrentRegistration.AdmissionInfo.InPatientDeptDetails.Where(x => x.V_InPatientDeptStatus == AllLookupValues.InPatientDeptStatus.NHAP_KHOA_PHONG && x.DeptLocation != null && x.DeptLocation.DeptID > 0).First().DeptLocation.DeptID;
            }
        }
        private void GetInfectionVirusCollection()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllInfectionVirus(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var GettedCollection = contract.EndGetAllInfectionVirus(asyncResult);
                                if (GettedCollection != null)
                                {
                                    InfectionVirusCollection = GettedCollection.ToObservableCollection();
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
        public IEnumerator<IResult> DoOpenRegistration(long regID)
        {
            LoadRegistrationSwitch LoadRegisSwitch = new LoadRegistrationSwitch();
            LoadRegisSwitch.IsGetAdmissionInfo = true;
            LoadRegisSwitch.IsGetBedAllocations = true;
            var loadRegTask = new LoadRegistrationInfo_InPtTask(regID, (int)AllLookupValues.V_FindPatientType.NOI_TRU, LoadRegisSwitch);
            yield return loadRegTask;
            CurrentRegistration = loadRegTask.Registration;
            GetInfectionCaseByPtID(CurrentRegistration.PtRegistrationID, CurrentRegistration.PatientID);
        }
        private void EditAntibioticTreatment(AntibioticTreatment CurrentAntibioticTreatment)
        {
            this.ShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginEditAntibioticTreatment(CurrentAntibioticTreatment, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndEditAntibioticTreatment(asyncResult);
                                GetCurrentInfectionCaseDetails();
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
            CurrentThread.Start();
        }
        public void LoadOldInfectionCase(InfectionCase aInfectionCase)
        {
            CurrentInfectionCase = aInfectionCase;
            GetCurrentInfectionCaseDetails();
        }
        private void GetLatestDiagTrmtByPtID_InPt(long aPatientID, long? V_DiagnosisType)
        {
            this.ShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                using (var aFactory = new ePMRsServiceClient())
                {
                    var aContract = aFactory.ServiceInstance;
                    aContract.BeginGetLatestDiagnosisTreatmentByPtID_InPt(aPatientID, V_DiagnosisType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var DiagTrmtItem = aContract.EndGetLatestDiagnosisTreatmentByPtID_InPt(asyncResult);
                            if (DiagTrmtItem != null && !string.IsNullOrEmpty(DiagTrmtItem.DiagnosisFinal))
                            {
                                CurrentInfectionCase.DiagnosisFinal1 = DiagTrmtItem.DiagnosisFinal;
                                LoadIcd10Items_InPt(DiagTrmtItem.DTItemID);
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
            CurrentThread.Start();
        }
        private void LoadIcd10Items_InPt(long DTItemID)
        {
            if (DTItemID <= 0)
            {
                return;
            }
            this.ShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                using (var aFactory = new ePMRsServiceClient())
                {
                    var aContract = aFactory.ServiceInstance;
                    aContract.BeginGetDiagnosisIcd10Items_Load_InPt(DTItemID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var GettedCollection = aContract.EndGetDiagnosisIcd10Items_Load_InPt(asyncResult);
                            if (GettedCollection != null && GettedCollection.Any(x => x.DiseasesReference != null && !string.IsNullOrEmpty(x.DiseasesReference.ICD10Code)))
                            {
                                CurrentInfectionCase.InfectionICD10ListID1Items = new ObservableCollection<InfectionICD10Item>();
                                IDC10ListCollection1 = new ObservableCollection<DiagnosisIcd10Items>();
                                foreach (var aItem in GettedCollection.Where(x => x.DiseasesReference != null && !string.IsNullOrEmpty(x.DiseasesReference.ICD10Code)))
                                {
                                    CurrentInfectionCase.InfectionICD10ListID1Items.Add(new InfectionICD10Item { ICD10Code = aItem.ICD10Code, ICD10Name = aItem.DiseasesReference.DiseaseNameVN, IsMain = aItem.IsMain });
                                }
                                IDC10ListCollection1 = CurrentInfectionCase.InfectionICD10ListID1Items.Select(x => new DiagnosisIcd10Items { IsMain = x.IsMain, DiseasesReference = new DiseasesReference { ICD10Code = x.ICD10Code, DiseaseNameVN = x.ICD10Name } }).ToObservableCollection();
                                AddBlankRow1();
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
            CurrentThread.Start();
        }
        #endregion
    }
}