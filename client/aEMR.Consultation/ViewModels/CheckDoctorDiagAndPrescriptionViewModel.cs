using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using Castle.Windsor;
using aEMR.Common.BaseModel;
using System.Linq;
using aEMR.Controls;
using aEMR.ServiceClient.Consultation_PCLs;
using System.Windows.Media;
using System.Windows.Data;
/*
 * 20211004 #001 TNHX: Lọc danh sách bsi theo cấu hình trách nhiệm + theo tích tạm khóa
 */
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(ICheckDoctorDiagAndPrescription)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CheckDoctorDiagAndPrescriptionViewModel : ViewModelBase, ICheckDoctorDiagAndPrescription
    {
        [ImportingConstructor]
        public CheckDoctorDiagAndPrescriptionViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            LoadDoctorStaffCollection();
            curObjPatientRegistration = new ObservableCollection<PatientRegistration>();
            ObjPatientPCLRequestDetails = new ObservableCollection<PatientPCLRequestDetail>();
            ObjPrescriptionDetails = new ObservableCollection<PrescriptionDetail>();
            SearchCriteria = new CheckDiagAndPrescriptionSearchCriteria { FromDate = Globals.GetCurServerDateTime(), ToDate = Globals.GetCurServerDateTime() };
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
        }

        public override void DeactivateItem(object item, bool close)
        {
            base.DeactivateItem(item, close);
            Globals.EventAggregator.Unsubscribe(this);
            var homevm = Globals.GetViewModel<IHome>();
            homevm.OutstandingTaskContent = null;
            homevm.IsExpandOST = false;
        }

        #region Properties
        AxAutoComplete AcbDoctorStaff { get; set; }
        AutoCompleteBox cboContext { get; set; }
        private int _CountPatientRegistration = 0;
        public int CountPatientRegistration
        {
            get
            {
                return _CountPatientRegistration;
            }
            set
            {
                if (_CountPatientRegistration != value)
                {
                    _CountPatientRegistration = value;
                    NotifyOfPropertyChange(() => CountPatientRegistration);
                }
            }
        }
        private string _FullName = "";
        public string FullName
        {
            get
            {
                return _FullName;
            }
            set
            {
                if (_FullName != value)
                {
                    _FullName = value;
                    NotifyOfPropertyChange(() => FullName);
                }
            }
        }
        private string _GenderString = "";
        public string GenderString
        {
            get
            {
                return _GenderString;
            }
            set
            {
                if (_GenderString != value)
                {
                    _GenderString = value;
                    NotifyOfPropertyChange(() => GenderString);
                }
            }
        }
        private string _Age = "";
        public string Age
        {
            get
            {
                return _Age;
            }
            set
            {
                if (_Age != value)
                {
                    _Age = value;
                    NotifyOfPropertyChange(() => Age);
                }
            }
        }

        private string _Weight = "";
        public string Weight
        {
            get
            {
                return _Weight;
            }
            set
            {
                if (_Weight != value)
                {
                    _Weight = value;
                    NotifyOfPropertyChange(() => Weight);
                }
            }
        }

        private string _Diagnosis = "";
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
        private string _DiagnosisSub = "";
        public string DiagnosisSub
        {
            get
            {
                return _DiagnosisSub;
            }
            set
            {
                if (_DiagnosisSub != value)
                {
                    _DiagnosisSub = value;
                    NotifyOfPropertyChange(() => DiagnosisSub);
                }
            }
        }
        private string _ListICD10 = "";
        public string ListICD10
        {
            get
            {
                return _ListICD10;
            }
            set
            {
                if (_ListICD10 != value)
                {
                    _ListICD10 = value;
                    NotifyOfPropertyChange(() => ListICD10);
                }
            }
        }
        private ObservableCollection<PatientRegistration> _curObjPatientRegistration;
        public ObservableCollection<PatientRegistration> curObjPatientRegistration
        {
            get
            {
                return _curObjPatientRegistration;
            }
            set
            {
                if (_curObjPatientRegistration != value)
                {
                    _curObjPatientRegistration = value;
                    NotifyOfPropertyChange(() => curObjPatientRegistration);
                }
            }
        }

        private ObservableCollection<PrescriptionDetail> _ObjPrescriptionDetails;
        public ObservableCollection<PrescriptionDetail> ObjPrescriptionDetails
        {
            get
            {
                return _ObjPrescriptionDetails;
            }
            set
            {
                if (_ObjPrescriptionDetails != value)
                {
                    _ObjPrescriptionDetails = value;
                    NotifyOfPropertyChange(() => ObjPrescriptionDetails);
                }
            }
        }

        private ObservableCollection<PatientPCLRequestDetail> _ObjPatientPCLRequestDetails;
        public ObservableCollection<PatientPCLRequestDetail> ObjPatientPCLRequestDetails
        {
            get
            {
                return _ObjPatientPCLRequestDetails;
            }
            set
            {
                if (_ObjPatientPCLRequestDetails != value)
                {
                    _ObjPatientPCLRequestDetails = value;
                    NotifyOfPropertyChange(() => ObjPatientPCLRequestDetails);
                }
            }
        }

        private ObservableCollection<Staff> _DoctorStaffs;
        public ObservableCollection<Staff> DoctorStaffs
        {
            get
            {
                return _DoctorStaffs;
            }
            set
            {
                if (_DoctorStaffs != value)
                {
                    _DoctorStaffs = value;
                    NotifyOfPropertyChange(() => DoctorStaffs);
                }
            }
        }
        private Staff _gSelectedDoctorStaff;
        public Staff gSelectedDoctorStaff
        {
            get
            {
                return _gSelectedDoctorStaff;
            }
            set
            {
                _gSelectedDoctorStaff = value;
                NotifyOfPropertyChange(() => gSelectedDoctorStaff);
            }
        }

        private CheckDiagAndPrescriptionSearchCriteria _SearchCriteria;
        public CheckDiagAndPrescriptionSearchCriteria SearchCriteria
        {
            get { return _SearchCriteria; }
            set
            {
                _SearchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }
        #endregion

        #region Method
        private void LoadDoctorStaffCollection()
        {
            //▼====: #001
            DoctorStaffs = new ObservableCollection<Staff>(Globals.AllStaffs.Where(x => x.RefStaffCategory != null 
                                                                                    && x.RefStaffCategory.V_StaffCatType == Globals.ServerConfigSection.CommonItems.StaffCatTypeBAC_SI 
                                                                                    && x.SCertificateNumber != null && x.SCertificateNumber.Length > 9
                                                                                    && (!x.IsStopUsing)).ToList());
            //▲====: #001
        }
        public void DoctorStaff_Loaded(object sender, RoutedEventArgs e)
        {
            AcbDoctorStaff = (AxAutoComplete)sender;
        }
        public void DoctorStaff_Populating(object sender, PopulatingEventArgs e)
        {
            cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            var AllItemsContext = new ObservableCollection<Staff>(DoctorStaffs.Where(x => Globals.RemoveVietnameseString(x.FullName).ToLower().Contains(Globals.RemoveVietnameseString(cboContext.SearchText).ToLower())));
            cboContext.ItemsSource = AllItemsContext;
            cboContext.PopulateComplete();
        }
        public void DoctorStaff_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            gSelectedDoctorStaff = ((AutoCompleteBox)sender).SelectedItem as Staff;
            SearchCriteria.DoctorID = gSelectedDoctorStaff != null ? gSelectedDoctorStaff.StaffID : 0;
        }

        public void btnFindRegistration()
        {
            long StaffID = gSelectedDoctorStaff != null ? gSelectedDoctorStaff.StaffID : 0;
            GetRegistationByDocAndTime();
        }
        public void GetRegistationByDocAndTime()
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginSearchRegistrationListForCheckDiagAndPrescription(SearchCriteria
                            , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                curObjPatientRegistration = new ObservableCollection<PatientRegistration>();
                                List<PatientRegistration> allItems = client.EndSearchRegistrationListForCheckDiagAndPrescription(asyncResult);
                                if (allItems != null && allItems.Count > 0)
                                {
                                    foreach (var items in allItems)
                                    {
                                        curObjPatientRegistration.Add(items);
                                    }
                                    CountPatientRegistration = allItems.Count();
                                }
                                else
                                {
                                    CountPatientRegistration = 0;
                                    MessageBox.Show(String.Format("{0}", eHCMSResources.A0638_G1_Msg_InfoKhCoData));
                                }
                            }
                            catch (Exception ex)
                            {
                                this.HideBusyIndicator();
                                MessageBox.Show(ex.Message);
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
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public void DoubleClick(object source, EventArgs<object> eventArgs)
        {
            if (source == null || eventArgs == null)
            {
                return;
            }
            PatientRegistration SelectedPatientRegistation = eventArgs.Value as PatientRegistration;
            if (SelectedPatientRegistation != null && SelectedPatientRegistation.DiagnosisTreatment != null && !string.IsNullOrEmpty(SelectedPatientRegistation.Patient.FullName)
                && !string.IsNullOrEmpty(SelectedPatientRegistation.DiagnosisTreatment.DiagnosisFinal) && !string.IsNullOrEmpty(SelectedPatientRegistation.DiagnosisTreatment.ICD10List))
            {
                CurPatientRegistation = SelectedPatientRegistation;
                FullName = SelectedPatientRegistation.Patient.FullName;
                GenderString = SelectedPatientRegistation.Patient.Gender;
                Age = SelectedPatientRegistation.Patient.AgeString.ToString();
                GetPatientPrescriptionAndPCLReq(SelectedPatientRegistation.PtRegistrationID, SelectedPatientRegistation.DiagnosisTreatment.DoctorStaffID);
                GetPhyExam_ByPtRegID(SelectedPatientRegistation.PtRegistrationID, (long)SelectedPatientRegistation.V_RegistrationType);
                GetListDiagnosisTreatment(SelectedPatientRegistation.DiagnosisTreatment.ServiceRecID, SelectedPatientRegistation.Patient.PatientID);
                GetMedicalFilesByPtRegistrationID((long)SelectedPatientRegistation.DiagnosisTreatment.PtRegDetailID, (long)SelectedPatientRegistation.V_RegistrationType);
            }
            else
            {
                MessageBox.Show(eHCMSResources.Z1688_G1_TTinKgHopLe);
            }
        }

        private void GetPhyExam_ByPtRegID(long PtRegistrationID, long V_RegistrationType)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new SummaryServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetPhyExam_ByPtRegID(PtRegistrationID, V_RegistrationType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                PhysicalExamination result = contract.EndGetPhyExam_ByPtRegID(asyncResult);
                                CurPhysicalExamination = result ?? new PhysicalExamination();
                                Weight = result != null ? result.Weight + " (kg)" : "";
                                Pressure = result != null ? result.SystolicPressure + "/" + result.DiastolicPressure : "";
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

        public string ReplaceICDString(string ICD10List, string ICD10Code)
        {
            string ListICD10AfterReplace = "";
            string tmpICD = "";
            if (!string.IsNullOrEmpty(ICD10List) && !string.IsNullOrEmpty(ICD10Code))
            {
                ICD10List = ICD10List.Trim();
                string[] ListArray = ICD10List.Split(',');
                if (ListArray.Length > 0)
                {
                    foreach (var strings in ListArray)
                    {
                        tmpICD = strings + ",";
                        if (String.Compare(strings, ICD10Code) == 0)
                        {
                            tmpICD = "";
                        }
                        ListICD10AfterReplace += tmpICD;
                    }
                }
            }
            return ListICD10AfterReplace;
        }
        #endregion

        public void GetPatientPrescriptionAndPCLReq(long PtRegistrationID, long DoctorID)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetPatientPrescriptionAndPCLReq(PtRegistrationID, DoctorID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                ObjPrescriptionDetails = new ObservableCollection<PrescriptionDetail>();
                                ObjPatientPCLRequestDetails = new ObservableCollection<PatientPCLRequestDetail>();
                                var tmpObjPrescriptionDetails = new List<PrescriptionDetail>();
                                var tmpObjPatientPCLRequestDetails = new List<PatientPCLRequestDetail>();
                                var bPreOK = false;
                                var bPCLOK = false;
                                bool bOK = client.EndGetPatientPrescriptionAndPCLReq(out tmpObjPrescriptionDetails, out tmpObjPatientPCLRequestDetails, out bPreOK, out bPCLOK, asyncResult);
                                if (bOK)
                                {
                                    ObjPrescriptionDetails = tmpObjPrescriptionDetails.ToObservableCollection();
                                    ObjPatientPCLRequestDetails = tmpObjPatientPCLRequestDetails.ToObservableCollection();
                                }
                                else
                                {
                                    if (bPreOK)
                                    {
                                        ObjPrescriptionDetails = tmpObjPrescriptionDetails.ToObservableCollection();
                                    }
                                    if (bPCLOK)
                                    {
                                        ObjPatientPCLRequestDetails = tmpObjPatientPCLRequestDetails.ToObservableCollection();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                this.HideBusyIndicator();
                                MessageBox.Show(ex.Message);
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
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        #region Handle
        //123
        #endregion

        public void grdPCLRequest_DblClick(object sender, EventArgs<object> e)
        {
            if ((sender as DataGrid).SelectedItem != null)
            {
                PatientPCLRequestDetail pcl = (sender as DataGrid).SelectedItem as PatientPCLRequestDetail;
                PatientPCLRequest request = pcl.PatientPCLRequest;

                //Action<IViewResultPCLLaboratoryByRequest_ExamHistory> onInitDlg = (typeInfo) =>
                //{
                //    //typeInfo.InitPatientInfo();
                //    typeInfo.LoadPCLRequestResult(request);
                //};
                //GlobalsNAV.ShowDialog(onInitDlg);
                if ((AllLookupValues.V_PCLMainCategory)request.V_PCLMainCategory == AllLookupValues.V_PCLMainCategory.Laboratory)
                {
                    IPatientPCLLaboratoryResult mPopupDialog = Globals.GetViewModel<IPatientPCLLaboratoryResult>();
                    mPopupDialog.IsDialogView = true;
                    mPopupDialog.IsShowSummaryContent = false;
                    mPopupDialog.LoadPCLRequestResult(//request
                        new PatientPCLRequest
                        {
                            PatientPCLReqID = request.PatientPCLReqID,
                            PatientID = request.Patient.PatientID,
                            V_PCLRequestType = request.V_PCLRequestType
                        }
                    );
                    GlobalsNAV.ShowDialog_V3(mPopupDialog, null, null, false, true, new System.Windows.Size(1000, 700));
                }
                else if ((AllLookupValues.V_PCLMainCategory)request.V_PCLMainCategory == AllLookupValues.V_PCLMainCategory.Imaging)
                {
                    IPatientPCLImagingResult mPopupDialog = Globals.GetViewModel<IPatientPCLImagingResult>();
                    mPopupDialog.IsShowSummaryContent = false;
                    mPopupDialog.IsDialogView = true;
                    mPopupDialog.LoadDataCoroutineEx( //request
                       new PatientPCLRequest
                       {
                           PatientPCLReqID = request.PatientPCLReqID,
                           PatientID = request.Patient.PatientID,
                           V_PCLRequestType = request.V_PCLRequestType,
                           PCLExamTypeID = pcl.PCLExamTypeID
                       }
                       );
                    GlobalsNAV.ShowDialog_V3(mPopupDialog, null, null, false, true, new System.Windows.Size(1000, 700));
                }

            }
        }
        //public void grdPCLRequest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if ((sender as DataGrid).SelectedItem != null)
        //    {
        //        //CurPatientPCLRequestDetails = sender.
        //    }
        //}
        #region new func
        private PatientRegistration _CurPatientRegistation;
        public PatientRegistration CurPatientRegistation
        {
            get
            {
                return _CurPatientRegistation;
            }
            set
            {
                if (_CurPatientRegistation != value)
                {
                    _CurPatientRegistation = value;
                    NotifyOfPropertyChange(() => CurPatientRegistation);
                }
            }
        }

        private PhysicalExamination _CurPhysicalExamination;
        public PhysicalExamination CurPhysicalExamination
        {
            get
            {
                return _CurPhysicalExamination;
            }
            set
            {
                if (_CurPhysicalExamination != value)
                {
                    _CurPhysicalExamination = value;
                    NotifyOfPropertyChange(() => CurPhysicalExamination);
                }
            }
        }

        private string _Pressure;
        public string Pressure
        {
            get
            {
                return _Pressure;
            }
            set
            {
                if (_Pressure != value)
                {
                    _Pressure = value;
                    NotifyOfPropertyChange(() => Pressure);
                }
            }
        }

        private string _ListICD10Sub = "";
        public string ListICD10Sub
        {
            get
            {
                return _ListICD10Sub;
            }
            set
            {
                if (_ListICD10Sub != value)
                {
                    _ListICD10Sub = value;
                    NotifyOfPropertyChange(() => ListICD10Sub);
                }
            }
        }

        public void GetListDiagnosisTreatment(long? ServiceRecID, long? PatientID)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDiagnosisIcd10Items_Load(ServiceRecID, PatientID, false, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDiagnosisIcd10Items_Load(asyncResult);
                            ObservableCollection<DiagnosisIcd10Items> refIDC10List = results.ToObservableCollection();
                            if (refIDC10List == null)
                            {
                                refIDC10List = new ObservableCollection<DiagnosisIcd10Items>();
                            }
                            else
                            {
                                ListICD10 = refIDC10List.Where(x => x.IsMain).FirstOrDefault().ICD10Code;
                                Diagnosis = refIDC10List.Where(x => x.IsMain).FirstOrDefault().DiseasesReference.DiseaseNameVN;
                                ListICD10Sub = string.Join(";", refIDC10List.Where(x => !x.IsMain).Select(x => x.ICD10Code));
                                DiagnosisSub = string.Join(";", refIDC10List.Where(x => !x.IsMain).Select(x => x.DiseasesReference.DiseaseNameVN));
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

        private CheckMedicalFiles _ObjCheckMedicalFiles;
        public CheckMedicalFiles ObjCheckMedicalFiles
        {
            get
            {
                return _ObjCheckMedicalFiles;
            }
            set
            {
                if (_ObjCheckMedicalFiles != value)
                {
                    _ObjCheckMedicalFiles = value;
                    NotifyOfPropertyChange(() => ObjCheckMedicalFiles);
                }
            }
        }

        public void btSave()
        {
            if (ObjCheckMedicalFiles == null && ObjCheckMedicalFiles.Check_KHTH == null)
            {
                return;
            }
            SaveMedicalFiles(ObjCheckMedicalFiles, false);
        }

        public void btCheckDLS()
        {
            if (ObjCheckMedicalFiles == null && ObjCheckMedicalFiles.Check_KHTH == null)
            {
                return;
            }
            ObjCheckMedicalFiles.IsDLSChecked = true;
            SaveMedicalFiles(ObjCheckMedicalFiles, false);
        }

        public void SaveMedicalFiles(CheckMedicalFiles CheckMedicalFile, bool Is_KHTH)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginSaveMedicalFiles(CheckMedicalFile, Is_KHTH, (long)CurPatientRegistation.V_RegistrationType
                            , Globals.LoggedUserAccount.StaffID ?? 0, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var bOK = client.EndSaveMedicalFiles(out long CheckMedicalFileIDNew, asyncResult);
                                if (bOK)
                                {
                                    MessageBox.Show("Cập nhật thành công");
                                    ObjCheckMedicalFiles.CheckMedicalFileID = CheckMedicalFileIDNew;
                                    btnFindRegistration();
                                }
                            }
                            catch (Exception ex)
                            {
                                this.HideBusyIndicator();
                                MessageBox.Show(ex.Message);
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
                    MessageBox.Show(ex.Message);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        public void GetMedicalFilesByPtRegistrationID(long PtRegDetailID, long V_RegistrationType)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetMedicalFilesByPtRegistrationID(PtRegDetailID, V_RegistrationType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                ObjCheckMedicalFiles = new CheckMedicalFiles();

                                var result = client.EndGetMedicalFilesByPtRegistrationID(asyncResult);
                                if (result != null)
                                {
                                    ObjCheckMedicalFiles = result;
                                }
                                else
                                {
                                    ObjCheckMedicalFiles = new CheckMedicalFiles();
                                    ObjCheckMedicalFiles.PtRegistrationID = PtRegDetailID;
                                }
                            }
                            catch (Exception ex)
                            {
                                this.HideBusyIndicator();
                                MessageBox.Show(ex.Message);
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
                    MessageBox.Show(ex.Message);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        public void grdPatientRegistration_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
            PatientRegistration item = e.Row.DataContext as PatientRegistration;
            if (item == null)
            {
                return;
            }
            if (item.IsDLSChecked)
            {
                e.Row.Background = new SolidColorBrush(Colors.LightGreen);
                e.Row.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }

        private ObservableCollection<Lookup> _V_CheckMedicalFilesStatusForDLS;

        public ObservableCollection<Lookup> V_CheckMedicalFilesStatusForDLS
        {
            get { return _V_CheckMedicalFilesStatusForDLS; }
            set
            {
                _V_CheckMedicalFilesStatusForDLS = value;
                NotifyOfPropertyChange(() => V_CheckMedicalFilesStatusForDLS);
            }
        }
        private long _SelectedV_CheckMedicalFilesStatusForDLS;

        public long SelectedV_CheckMedicalFilesStatusForDLS
        {
            get { return _SelectedV_CheckMedicalFilesStatusForDLS; }
            set
            {
                _SelectedV_CheckMedicalFilesStatusForDLS = value;
                NotifyOfPropertyChange(() => SelectedV_CheckMedicalFilesStatusForDLS);
            }
        }

        AxComboBox kscV_CheckMedicalFilesStatus = null;
        public void KscV_CheckMedicalFilesStatusForDLS_Loaded(object sender, RoutedEventArgs e)
        {
            kscV_CheckMedicalFilesStatus = (AxComboBox)sender;
            GetAllV_CheckMedicalFilesStatusForDLS();
        }

        public void GetAllV_CheckMedicalFilesStatusForDLS()
        {
            V_CheckMedicalFilesStatusForDLS = new ObservableCollection<Lookup>();
            V_CheckMedicalFilesStatusForDLS.Insert(0, new Lookup { LookupID = 0, ObjectValue = "Tất cả" });
            V_CheckMedicalFilesStatusForDLS.Insert(0, new Lookup { LookupID = 1, ObjectValue = "DLS đã xem" });
            V_CheckMedicalFilesStatusForDLS.Insert(0, new Lookup { LookupID = 2, ObjectValue = "DLS chưa xem" });

            object tmpkscSelectedItem = null;
            if (V_CheckMedicalFilesStatusForDLS != null)
            {
                if (kscV_CheckMedicalFilesStatus != null && SelectedV_CheckMedicalFilesStatusForDLS > 0)
                {
                    foreach (var tmpRefTreatment in V_CheckMedicalFilesStatusForDLS)
                    {
                        if (tmpRefTreatment.LookupID == SelectedV_CheckMedicalFilesStatusForDLS)
                        {
                            tmpkscSelectedItem = tmpRefTreatment;
                        }
                    }
                }
                else
                {
                    kscV_CheckMedicalFilesStatus.ItemsSource = V_CheckMedicalFilesStatusForDLS;
                }
            }

            if (tmpkscSelectedItem != null)
            {
                kscV_CheckMedicalFilesStatus.ItemsSource = V_CheckMedicalFilesStatusForDLS;
                kscV_CheckMedicalFilesStatus.SelectedItem = tmpkscSelectedItem;
            }
        }

        private DeptLocation _selectedLocation;
        public DeptLocation SelectedLocation
        {
            get
            {
                return _selectedLocation;
            }
            set
            {
                _selectedLocation = value;
                NotifyOfPropertyChange(() => SelectedLocation);
            }
        }

        private ObservableCollection<DeptLocation> _locations;
        public ObservableCollection<DeptLocation> Locations
        {
            get
            {
                return _locations;
            }
            set
            {
                _locations = value;
                NotifyOfPropertyChange(() => Locations);
            }
        }
        
        AxComboBox KscLocations = null;
        public void KscLocations_Loaded(object sender, RoutedEventArgs e)
        {
            KscLocations = (AxComboBox)sender;
            LoadLocations();
        }

        private void LoadLocations()
        {
            this.ShowBusyIndicator();
            var list = new List<refModule>();

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetAllLocationsByDeptIDOld(Globals.ServerConfigSection.Hospitals.KhoaPhongKham, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var allItems = contract.EndGetAllLocationsByDeptIDOld(asyncResult);

                            if (allItems != null)
                            {
                                Locations = new ObservableCollection<DeptLocation>(allItems);
                            }
                            else
                            {
                                Locations = new ObservableCollection<DeptLocation>();
                            }
                            var itemDefault = new DeptLocation();
                            itemDefault.DeptID = -1;
                            itemDefault.Location = new Location();
                            itemDefault.Location.LID = -1;
                            itemDefault.Location.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K0705_G1_TimTatCa);
                            Locations.Insert(0, itemDefault);

                            KscLocations.ItemsSource = Locations;
                            KscLocations.SelectedItem = itemDefault;
                            SelectedLocation = itemDefault;
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
        #endregion
        public void ICDListCmd()
        {
            Action<IICD_ListFindForConsultation> onInitDlg = delegate (IICD_ListFindForConsultation listFind)
            {
                this.ActivateItem(listFind);
            };
            GlobalsNAV.ShowDialog<IICD_ListFindForConsultation>(onInitDlg);
        }
    }
}