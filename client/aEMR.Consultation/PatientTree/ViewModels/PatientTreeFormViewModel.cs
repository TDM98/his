using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using PCLsProxy;
using PCLsService;
using aEMR.Common;
using aEMR.Common.Collections;
using aEMR.Infrastructure.GlobalFuncs;
using Castle.Windsor;
using aEMR.Common.BaseModel;
using aEMR.CommonTasks;
using DevExpress.ReportServer.Printing;
using aEMR.ReportModel.ReportModels;
using System.Linq;
using aEMR.ConsultantEPrescription.PCL_ViewResults.ViewModels;
//using Microsoft.Web.WebView2.Wpf;
using System.Xml.Linq;
using System.Reflection;
/*
* 20181121 #001 TTM:   BM 0005257: Chỉnh sửa out standing task ngoại trú, thêm mới out standing task nội trú.	
* 20211225 #002 BLQ:  Task 857: Chỉnh lại View kết quả hình ảnh
* 20220901 #003 BLQ: Issue:2174 Chỉnh lại mẫu hình ảnh theo cách mới
* 20221129 #004 TNHX:  Thêm xem kết quả đã ký số
*/
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IPatientTreeForm)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientTreeFormViewModel : ViewModelBase, IPatientTreeForm
        , IHandle<PatientTreeChange>
        , IHandle<PatientSummaryChange>
        , IHandle<PatientInfoChange>
        , IHandle<ShowPatientInfo_KHAMBENH_LSBENHAN<Patient, PatientRegistration, PatientRegistrationDetail>>
    {
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        public Patient curPatient
        {
            get { return Registration_DataStorage.CurrentPatient; }
        }

        private string PclImageStoragePath = "";
        private string LocalFolderName = "";
        private int index = 0;
        [ImportingConstructor]
        public PatientTreeFormViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            authorization();

            CreateSubVM();

            invisibleAll();

            if (IsShowSummaryContent)
            {
                //var searchPatientAndRegVm = Globals.GetViewModel<ISearchPatientAndRegistration>();
                //searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_PATIENT_BTN);
                //searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_PATIENT);

                //SearchRegistrationContent = searchPatientAndRegVm;
                //ActivateItem(searchPatientAndRegVm);
            }

            PclImageStoragePath = aEMR.Common.EncryptExtension.Decrypt(Globals.ServerConfigSection.Pcls.PclImageStoragePath,
                                                         Globals.AxonKey, Globals.AxonPass);
            LocalFolderName = aEMR.Common.EncryptExtension.Decrypt(Globals.ServerConfigSection.Pcls.LocalFolderName,
                                                                  Globals.AxonKey, Globals.AxonPass);
            try
            {
                if (!ClientFileManager.FolderExists(LocalFolderName))
                {
                    ClientFileManager.CreateFolder(LocalFolderName);
                }
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogInfo(ex.ToString());
            }

        }

        // TxD 22/09/2018 Added the following to ALLOW child VM of each Tab to get ACCESS to the CENTRAL Data stored in the MainKB (Main KHAM BENH) Screen
        private ICS_DataStorage _CS_DS = null;
        public ICS_DataStorage CS_DS
        {
            get
            {
                return _CS_DS;
            }
            set
            {
                _CS_DS = value;
                ((IPatientInfo)UCPatientProfileInfo).CS_DS = CS_DS;
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            ActivateSubVM();
            //▼====== #001
            if (Globals.LeftModuleActive == LeftModuleActive.KHAMBENH_LSBENHAN)
            {
                var homevm = Globals.GetViewModel<IHome>();
                homevm.OutstandingTaskContent = Globals.GetViewModel<IConsultationOutstandingTask>();
                homevm.IsExpandOST = true;
            }
            //▲====== #001
        }

        public override void DeactivateItem(object item, bool close)
        {
            base.DeactivateItem(item, close);
            Globals.EventAggregator.Unsubscribe(this);
            //▼====== #001
            var homevm = Globals.GetViewModel<IHome>();
            homevm.OutstandingTaskContent = null;
            homevm.IsExpandOST = false;
            //▲====== #001
        }

        private void CreateSubVM()
        {
            UCDoctorProfileInfo = Globals.GetViewModel<ILoginInfo>();

            UCPatientProfileInfo = Globals.GetViewModel<IPatientInfo>();

            UCHeaderInfoPMR = Globals.GetViewModel<IPatientMedicalRecords_ByPatientID>();

            UCPatientTreeView = Globals.GetViewModel<IPatientTree>();


        }

        public void ActivateSubVM()
        {
            ActivateItem(UCDoctorProfileInfo);

            ActivateItem(UCPatientProfileInfo);

            ActivateItem(UCHeaderInfoPMR);

            ActivateItem(UCPatientTreeView);


        }

        public IPatientTree UCPatientTreeView { get; set; }

        public object SearchRegistrationContent
        { get; set; }

        public IPatientMedicalRecords_ByPatientID UCHeaderInfoPMR
        {
            get;
            set;
        }

        public object UCDoctorProfileInfo
        {
            get;
            set;
        }
        public object UCPatientProfileInfo
        {
            get;
            set;
        }

        public object TabKhamBenh
        {
            get;
            set;
        }
        public object TabToaThuoc
        {
            get;
            set;
        }
        public object TabPCL
        {
            get;
            set;
        }
        public object TabHinhAnhPCL
        {
            get;
            set;
        }
        public object TabPDFView
        {
            get;
            set;
        }
        public object TabHinhAnhPCL_New
        {
            get;
            set;
        }
        public object TabThuThuat
        {
            get;
            set;
        }
        public object TabNoiTru
        {
            get;
            set;
        }

        public object TabCLSHinhAnhNgoaiVien
        {
            get;
            set;
        }

        public object PCLDepartmentContent { get; set; }

        private string _PCLResultParamImpName;
        public string PCLResultParamImpName
        {
            get
            {
                return _PCLResultParamImpName;
            }
            set
            {
                if (_PCLResultParamImpName == value)
                    return;
                _PCLResultParamImpName = value;
                NotifyOfPropertyChange(() => PCLResultParamImpName);
            }
        }

        private bool _mPrescription = true;
        public bool mPrescription
        {
            get
            {
                return _mPrescription;
            }
            set
            {
                if (_mPrescription == value)
                    return;
                _mPrescription = value;
                NotifyOfPropertyChange(() => mPrescription);
            }
        }

        private bool _mTreatment = true;
        public bool mTreatment
        {
            get
            {
                return _mTreatment;
            }
            set
            {
                if (_mTreatment == value)
                    return;
                _mTreatment = value;
                NotifyOfPropertyChange(() => mTreatment);
            }
        }

        private bool _mPCL = true;
        public bool mPCL
        {
            get
            {
                return _mPCL;
            }
            set
            {
                if (_mPCL == value)
                    return;
                _mPCL = value;
                NotifyOfPropertyChange(() => mPCL);
            }
        }

        private bool _mNoiTru = true;
        public bool mNoiTru
        {
            get
            {
                return _mNoiTru;
            }
            set
            {
                if (_mNoiTru == value)
                    return;
                _mNoiTru = value;
                NotifyOfPropertyChange(() => mNoiTru);
            }
        }

        private bool _mHoiChan = true;
        public bool mHoiChan
        {
            get
            {
                return _mHoiChan;
            }
            set
            {
                if (_mHoiChan == value)
                    return;
                _mHoiChan = value;
                NotifyOfPropertyChange(() => mHoiChan);
            }
        }

        private bool _mCLSHinhAnhNgoaiVien = true;
        public bool mCLSHinhAnhNgoaiVien
        {
            get
            {
                return _mCLSHinhAnhNgoaiVien;
            }
            set
            {
                if (_mCLSHinhAnhNgoaiVien == value)
                    return;
                _mCLSHinhAnhNgoaiVien = value;
                NotifyOfPropertyChange(() => mCLSHinhAnhNgoaiVien);
            }
        }
        private bool _mPDFView = true;
        public bool mPDFView
        {
            get
            {
                return _mPDFView;
            }
            set
            {
                if (_mPDFView == value)
                    return;
                _mPDFView = value;
                NotifyOfPropertyChange(() => mPDFView);
            }
        }

        private bool _mGPTM = true;
        public bool mGPTM
        {
            get
            {
                return _mGPTM;
            }
            set
            {
                if (_mGPTM == value)
                    return;
                _mGPTM = value;
                NotifyOfPropertyChange(() => mGPTM);
            }
        }

        private bool _mPCLImage = true;
        public bool mPCLImage
        {
            get
            {
                return _mPCLImage;
            }
            set
            {
                if (_mPCLImage == value)
                    return;
                _mPCLImage = value;
                NotifyOfPropertyChange(() => mPCLImage);
            }
        }
        private bool _mPCLImage_New = true;
        public bool mPCLImage_New
        {
            get
            {
                return _mPCLImage_New;
            }
            set
            {
                if (_mPCLImage_New == value)
                    return;
                _mPCLImage_New = value;
                NotifyOfPropertyChange(() => mPCLImage_New);
            }
        }
        private bool _mSmallProcedure = true;
        public bool mSmallProcedure
        {
            get
            {
                return _mSmallProcedure;
            }
            set
            {
                if (_mSmallProcedure == value)
                    return;
                _mSmallProcedure = value;
                NotifyOfPropertyChange(() => mSmallProcedure);
            }
        }

        private bool _btPrevious = true;
        public bool btPrevious
        {
            get
            {
                return _btPrevious;
            }
            set
            {
                if (_btPrevious == value)
                    return;
                _btPrevious = value;
                NotifyOfPropertyChange(() => btPrevious);
            }
        }

        private bool _btNext = true;
        public bool btNext
        {
            get
            {
                return _btNext;
            }
            set
            {
                if (_btNext == value)
                    return;
                _btNext = value;
                NotifyOfPropertyChange(() => btNext);
            }
        }

        #region properties

        private DiagnosisTreatment _curDiagnosisTreatment;
        public DiagnosisTreatment curDiagnosisTreatment
        {
            get
            {
                return _curDiagnosisTreatment;
            }
            set
            {
                if (_curDiagnosisTreatment == value)
                    return;
                _curDiagnosisTreatment = value;
                NotifyOfPropertyChange(() => curDiagnosisTreatment);
            }
        }

        private PatientServicesTree _curPatientServicesTree;
        public PatientServicesTree curPatientServicesTree
        {
            get
            {
                return _curPatientServicesTree;
            }
            set
            {
                if (_curPatientServicesTree == value)
                    return;
                _curPatientServicesTree = value;
                NotifyOfPropertyChange(() => curPatientServicesTree);
            }
        }

        private ObservableCollection<Prescription> _allPrescription;
        public ObservableCollection<Prescription> allPrescription
        {
            get
            {
                return _allPrescription;
            }
            set
            {
                if (_allPrescription == value)
                    return;
                _allPrescription = value;
            }
        }

        private Prescription _curPrescription;
        public Prescription curPrescription
        {
            get
            {
                return _curPrescription;
            }
            set
            {
                if (_curPrescription == value)
                    return;
                _curPrescription = value;
                NotifyOfPropertyChange(() => curPrescription);
            }
        }

        private ObservableCollection<DiagnosisIcd10Items> _refIDC10List;
        public ObservableCollection<DiagnosisIcd10Items> refIDC10List
        {
            get
            {
                return _refIDC10List;
            }
            set
            {
                if (_refIDC10List != value)
                {
                    _refIDC10List = value;
                }
                NotifyOfPropertyChange(() => refIDC10List);
            }
        }

        private ObservableCollection<PatientPCLLaboratoryResultDetail> _allPatientPCLLaboratoryResultDetail;
        public ObservableCollection<PatientPCLLaboratoryResultDetail> allPatientPCLLaboratoryResultDetail
        {
            get
            {
                return _allPatientPCLLaboratoryResultDetail;

            }
            set
            {
                if (_allPatientPCLLaboratoryResultDetail == value)
                    return;
                _allPatientPCLLaboratoryResultDetail = value;
                NotifyOfPropertyChange(() => allPatientPCLLaboratoryResultDetail);
            }
        }

        private ObservableCollection<PCLExamType> _ObjPCLExamTypes_ByPatientPCLReqID;
        public ObservableCollection<PCLExamType> ObjPCLExamTypes_ByPatientPCLReqID
        {
            get
            {
                return _ObjPCLExamTypes_ByPatientPCLReqID;
            }
            set
            {
                if (_ObjPCLExamTypes_ByPatientPCLReqID == value)
                    return;
                _ObjPCLExamTypes_ByPatientPCLReqID = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypes_ByPatientPCLReqID);
            }
        }

        private ObservableCollection<PatientPCLImagingResult> _allPatientPCLImagingResult;
        public ObservableCollection<PatientPCLImagingResult> allPatientPCLImagingResult
        {
            get
            {
                return _allPatientPCLImagingResult;
            }
            set
            {
                if (_allPatientPCLImagingResult == value)
                    return;
                _allPatientPCLImagingResult = value;
                NotifyOfPropertyChange(() => allPatientPCLImagingResult);
            }
        }

        private PatientPCLImagingResult _curPatientPCLImagingResult;
        public PatientPCLImagingResult curPatientPCLImagingResult
        {
            get
            {
                return _curPatientPCLImagingResult;
            }
            set
            {
                if (_curPatientPCLImagingResult == value)
                    return;
                _curPatientPCLImagingResult = value;
                NotifyOfPropertyChange(() => curPatientPCLImagingResult);
                //▼====: #002
                //if (curPatientPCLImagingResult.PCLImgResultID > 0)
                //{
                //    GetPCLResultFileStoreDetails(curPatientPCLImagingResult.PCLImgResultID);
                //}
                //▲====: #002
            }
        }

        private ObservableCollection<PCLResultFileStorageDetail> _allPCLResultFileStorageDetail;
        public ObservableCollection<PCLResultFileStorageDetail> allPCLResultFileStorageDetail
        {
            get
            {
                return _allPCLResultFileStorageDetail;
            }
            set
            {
                if (_allPCLResultFileStorageDetail == value)
                    return;
                _allPCLResultFileStorageDetail = value;
                NotifyOfPropertyChange(() => allPCLResultFileStorageDetail);
            }
        }

        private BitmapImage _ObjBitmapImage;
        public BitmapImage ObjBitmapImage
        {
            get { return _ObjBitmapImage; }
            set
            {
                if (_ObjBitmapImage != value)
                {
                    _ObjBitmapImage = value;
                    NotifyOfPropertyChange(() => ObjBitmapImage);
                }
            }
        }

        private Stream _ObjGetVideoAndImage;
        public Stream ObjGetVideoAndImage
        {
            get { return _ObjGetVideoAndImage; }
            set
            {
                if (_ObjGetVideoAndImage != value)
                {
                    _ObjGetVideoAndImage = value;
                    NotifyOfPropertyChange(() => ObjGetVideoAndImage);
                }
            }
        }

        private InPatientAdmDisDetails _curInPatientAdmDisDetails;
        public InPatientAdmDisDetails curInPatientAdmDisDetails
        {
            get
            {
                return _curInPatientAdmDisDetails;
            }
            set
            {
                if (_curInPatientAdmDisDetails == value)
                    return;
                _curInPatientAdmDisDetails = value;
                NotifyOfPropertyChange(() => curInPatientAdmDisDetails);
            }
        }
        private ObservableCollection<DiagnosisICD9Items> _CurrentIcd9Collection;
        public ObservableCollection<DiagnosisICD9Items> CurrentIcd9Collection
        {
            get
            {
                return _CurrentIcd9Collection;
            }
            set
            {
                if (_CurrentIcd9Collection == value)
                    return;
                _CurrentIcd9Collection = value;
                NotifyOfPropertyChange(() => CurrentIcd9Collection);
                NotifyOfPropertyChange(() => IsHasIcd9);
            }
        }
        public bool IsHasIcd9
        {
            get
            {
                return CurrentIcd9Collection != null && CurrentIcd9Collection.Count > 0;
            }
        }
        #endregion

        public void lnkUpdateClick(object sender, RoutedEvent e)
        {

        }
        public void cboPMRRequest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem != null)
            {
                var Objtmp = ((sender as ComboBox).SelectedItem as PatientPCLRequest);
                if (Objtmp.PatientPCLReqID > 0)
                {
                    PCLExamTypes_ByPatientPCLReqID(Objtmp.PatientPCLReqID);
                }
                else
                {
                    //ObjPCLExamTypes_ByPatientPCLReqID.Clear();
                    //ObjPatientPCLImagingResult.PCLExamTypeID = -1;
                    //ObjGetPCLResultFileStoreDetails.Clear();
                }
            }
        }

        public void TabNoiTru_Loaded(object sender)
        {
            TabNoiTru = (TabItem)sender;
        }

        public void TabToaThuoc_Loaded(object sender)
        {
            TabToaThuoc = (TabItem)sender;
        }

        public void TabKhamBenh_Loaded(object sender)
        {
            TabKhamBenh = (TabItem)sender;
        }

        public void TabPCL_Loaded(object sender)
        {
            TabPCL = (TabItem)sender;
        }

        public void TabHinhAnhPCL_Loaded(object sender)
        {
            TabHinhAnhPCL = (TabItem)sender;
            if(PCLOldView == null)
            {
                var VM = Globals.GetViewModel<IHtmlReport>();
                PCLOldView = VM;
                this.ActivateItem(VM);
            }
        }
        public void TabPDFView_Loaded(object sender)
        {
            TabPDFView = (TabItem)sender;
            
        }
        public void PDFViewer_Loaded(object sender)
        {
            TabPDFView = (TabItem)sender;
            
        }
        public void TabHinhAnhPCL_New_Loaded(object sender)
        {
            TabHinhAnhPCL_New = (TabItem)sender;
        }
        public void TabThuThuat_Loaded(object sender)
        {
            //if (!mSmallProcedure)
            //{
            //    return;
            //}
            TabThuThuat = (TabItem)sender;
            var VM = Globals.GetViewModel<IHtmlReport>();
            ThuThuatView = VM;
            this.ActivateItem(VM);
        }

        private IPCLDeptImagingResult_Consultation _CLSHinhAnhNgoaiVien;
        public IPCLDeptImagingResult_Consultation CLSHinhAnhNgoaiVien
        {
            get { return _CLSHinhAnhNgoaiVien; }
            set
            {
                _CLSHinhAnhNgoaiVien = value;
                NotifyOfPropertyChange(() => CLSHinhAnhNgoaiVien);
            }
        }
        private IHtmlReport _ThuThuatView;
        public IHtmlReport ThuThuatView
        {
            get { return _ThuThuatView; }
            set
            {
                _ThuThuatView = value;
                NotifyOfPropertyChange(() => ThuThuatView);
            }
        }
        private IHtmlReport _PCLOldView;
        public IHtmlReport PCLOldView
        {
            get { return _PCLOldView; }
            set
            {
                _PCLOldView = value;
                NotifyOfPropertyChange(() => PCLOldView);
            }
        }

        public void TabCLSHinhAnhNgoaiVien_Loaded(object sender)
        {
            if (!mCLSHinhAnhNgoaiVien)
            {
                return;
            }
            TabCLSHinhAnhNgoaiVien = (TabItem)sender;
            var VM = Globals.GetViewModel<IPCLDeptImagingResult_Consultation>();
            VM.IsEdit = false;
            VM.Reset();
            CLSHinhAnhNgoaiVien = VM;
            this.ActivateItem(VM);
        }

        #region method

        private void GetPrescriptionByID(long PrescriptID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });

            var t = new Thread(() =>
            {
                //khong dung indicator o day vi lam mat fucus se bi an
                // IsLoading = true;

                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetPrescriptionByID(PrescriptID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetPrescriptionByID(asyncResult);
                            allPrescription = new ObservableCollection<Prescription>();
                            if (results != null)
                            {
                                foreach (Prescription p in results)
                                {
                                    p.PrescriptionDetails = ReadPrescriptionDetailFromString(p.PrescriptDetailsStr);
                                    allPrescription.Add(p);
                                }
                                NotifyOfPropertyChange(() => allPrescription);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            // IsLoading = false;
                        }

                    }), null);
                }

            });

            t.Start();
        }
        private ObservableCollection<PrescriptionDetail> ReadPrescriptionDetailFromString(string XmlString)
        {
            try
            {
                if (string.IsNullOrEmpty(XmlString))
                {
                    return null;
                }
                ObservableCollection<PrescriptionDetail> PrescriptionDetailCollection = new ObservableCollection<PrescriptionDetail>();
                XDocument PrescriptDetailDocument = XDocument.Load(new StringReader(XmlString));
                if (PrescriptDetailDocument.Element("Prescription") == null)
                {
                    return null;
                }
                foreach (var DetailItem in PrescriptDetailDocument.Element("Prescription").Elements("PrescriptionDetails"))
                {
                    PrescriptionDetail CurrentPrescriptionDetail = new PrescriptionDetail();
                    CurrentPrescriptionDetail.PrescriptDetailID = Convert.ToInt64(DetailItem.Element("PrescriptDetailID").Value);
                    CurrentPrescriptionDetail.DrugID = Convert.ToInt64(DetailItem.Element("DrugID").Value);
                    CurrentPrescriptionDetail.Qty = Convert.ToInt32(Convert.ToDecimal(DetailItem.Element("Qty").Value));
                    CurrentPrescriptionDetail.BrandName = Convert.ToString(DetailItem.Element("BrandName").Value);
                    CurrentPrescriptionDetail.Content = DetailItem.Element("Content") == null ? null : Convert.ToString(DetailItem.Element("Content").Value);
                    CurrentPrescriptionDetail.UnitName = DetailItem.Element("UnitName") == null ? null : Convert.ToString(DetailItem.Element("UnitName").Value);
                    CurrentPrescriptionDetail.UnitUse = DetailItem.Element("UnitNameUse") == null ? null : Convert.ToString(DetailItem.Element("UnitNameUse").Value);
                    CurrentPrescriptionDetail.Administration = DetailItem.Element("Administration") == null ? null : Convert.ToString(DetailItem.Element("Administration").Value);
                    CurrentPrescriptionDetail.DayRpts = DetailItem.Element("DayRpts") == null ? 0 : Convert.ToInt32(Convert.ToDecimal(DetailItem.Element("DayRpts").Value));
                    CurrentPrescriptionDetail.DrugInstructionNotes = DetailItem.Element("DrugInstructionNotes") == null ? null : Convert.ToString(DetailItem.Element("DrugInstructionNotes").Value);
                    CurrentPrescriptionDetail.V_DrugType = Convert.ToInt64(DetailItem.Element("V_DrugType").Value);
                    CurrentPrescriptionDetail.MDose = float.Parse(DetailItem.Element("MDose").Value);
                    CurrentPrescriptionDetail.ADose = float.Parse(DetailItem.Element("ADose").Value);
                    CurrentPrescriptionDetail.EDose = float.Parse(DetailItem.Element("EDose").Value);
                    CurrentPrescriptionDetail.NDose = float.Parse(DetailItem.Element("NDose").Value);
                    CurrentPrescriptionDetail.MDoseStr = Convert.ToString(DetailItem.Element("MDoseStr").Value);
                    CurrentPrescriptionDetail.ADoseStr = Convert.ToString(DetailItem.Element("ADoseStr").Value);
                    CurrentPrescriptionDetail.EDoseStr = Convert.ToString(DetailItem.Element("EDoseStr").Value);
                    CurrentPrescriptionDetail.NDoseStr = Convert.ToString(DetailItem.Element("NDoseStr").Value);
                    PrescriptionDetailCollection.Add(CurrentPrescriptionDetail);
                }
                return PrescriptionDetailCollection;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void LoadPrescriptionDetailViewByID_InPt(long ServiceRecID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetPrescriptionIssueHistoriesInPtBySerRecID(ServiceRecID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetPrescriptionIssueHistoriesInPtBySerRecID(asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                var PrescriptionIssued = results.LastOrDefault();
                                Prescription CurrentPrescription = results.LastOrDefault().Prescription;
                                GetPrescriptionDetailsByPrescriptID_InPt(CurrentPrescription);

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
        public void btnPreviewPrescription()
        {
            if (allPrescription.FirstOrDefault().IssueID <= 0)
            {
                MessageBox.Show(eHCMSResources.Z2300_G1_KhongCoToaThuocDeXemIn, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            Action<ICommonPreviewView> onInitDlg = (proAlloc) =>
            {
                proAlloc.IssueID = allPrescription.FirstOrDefault().IssueID;
                if (curPatientServicesTree.InPt)
                {
                    proAlloc.eItem = ReportName.CONSULTATION_TOATHUOC_INPT;
                }
                else
                {
                    proAlloc.eItem = ReportName.CONSULTATION_TOATHUOC;
                }
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }
        public void GetPrescriptionDetailsByPrescriptID_InPt(Prescription CurrentPrescription, bool GetRemaining = false)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetPrescriptionDetailsByPrescriptID_InPt(CurrentPrescription.PrescriptID, GetRemaining, false, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetPrescriptionDetailsByPrescriptID_InPt(asyncResult);
                            allPrescription = new ObservableCollection<Prescription>();
                            if (results != null)
                            {
                                CurrentPrescription.PrescriptionDetails = results.ToObservableCollection();
                                allPrescription.Add(CurrentPrescription);
                                NotifyOfPropertyChange(() => allPrescription);
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
        private void GetDiagnosisTreatmentByDTItemID(long DTItemID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDiagnosisTreatmentByDTItemID(DTItemID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDiagnosisTreatmentByDTItemID(asyncResult);
                            curDiagnosisTreatment = new DiagnosisTreatment();

                            if (results != null)
                            {
                                curDiagnosisTreatment = results;
                                DiagnosisIcd10Items_Load(curDiagnosisTreatment.ServiceRecID
                                    , Registration_DataStorage.CurrentPatient.PatientID, false);
                            }

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading = false;

                        }

                    }), null);

                }

            });

            t.Start();

        }
        public void LoadDiagnosisTreatmentViewByDTItemID_InPt(long PtRegistrationID, long DTItemID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var mFactory = new ePMRsServiceClient())
                {
                    var mContract = mFactory.ServiceInstance;
                    mContract.BeginGetDiagnosisTreatment_InPt_ByPtRegID(PtRegistrationID, DTItemID, null, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = mContract.EndGetDiagnosisTreatment_InPt_ByPtRegID(asyncResult);
                            curDiagnosisTreatment = new DiagnosisTreatment();
                            if (results != null && results.Count == 1)
                            {
                                curDiagnosisTreatment = results.First();
                                DiagnosisIcd10Items_Load_InPt(curDiagnosisTreatment, DTItemID);
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
        private void DiagnosisIcd10Items_Load_InPt(DiagnosisTreatment aDiagnosisTreatment, long DTItemID)
        {
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
                            refIDC10List = contract.EndGetDiagnosisIcd10Items_Load_InPt(asyncResult).ToObservableCollection();
                            if (refIDC10List == null)
                            {
                                refIDC10List = new ObservableCollection<DiagnosisIcd10Items>();
                            }
                            DiagnosisICD9Items_Load_InPt(aDiagnosisTreatment, refIDC10List, DTItemID);
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
        private void DiagnosisICD9Items_Load_InPt(DiagnosisTreatment aDiagnosisTreatment, ObservableCollection<DiagnosisIcd10Items> CurrentIcd10Collection, long DTItemID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDiagnosisICD9Items_Load_InPt(DTItemID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDiagnosisICD9Items_Load_InPt(asyncResult);
                            CurrentIcd9Collection = new ObservableCollection<DiagnosisICD9Items>();
                            if (results != null)
                            {
                                CurrentIcd9Collection = results.ToObservableCollection();
                            }
                            //IDiagnosisTreatmentHistoryDetail mPopupDialog = Globals.GetViewModel<IDiagnosisTreatmentHistoryDetail>();
                            //mPopupDialog.CurrentDiagnosisTreatment = aDiagnosisTreatment;
                            //mPopupDialog.CurrentIcd10Collection = CurrentIcd10Collection;
                            //mPopupDialog.CurrentIcd9Collection = CurrentIcd9Collection;
                            this.HideBusyIndicator();
                            //GlobalsNAV.ShowDialog_V3(mPopupDialog);
                        }
                        catch (Exception ex)
                        {
                            this.HideBusyIndicator();
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                    }), null);
                }
            });
            t.Start();
        }
        private void DiagnosisIcd10Items_Load(long? ServiceRecID, long? PatientID, bool Last)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetDiagnosisIcd10Items_Load(ServiceRecID, PatientID, Last, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDiagnosisIcd10Items_Load(asyncResult);
                            refIDC10List = results.ToObservableCollection();
                            if (refIDC10List == null)
                            {
                                refIDC10List = new ObservableCollection<DiagnosisIcd10Items>();
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }
                    }), null);
                }
            });

            t.Start();
        }

        private void PCLLaboratoryResults_With_ResultOld(long PatientID, long PatientPCLReqID, bool InPt)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                IsLoading = true;
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        IPCLs contract = serviceFactory.ServiceInstance;

                        contract.BeginPCLLaboratoryResults_With_ResultOld(PatientID, PatientPCLReqID,
                            InPt ? (long)AllLookupValues.V_PCLRequestType.NOI_TRU : (long)AllLookupValues.V_PCLRequestType.NGOAI_TRU,
                            Globals.DispatchCallback((asyncResult) =>
                            //3712, Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    IList<PatientPCLLaboratoryResultDetail> results = contract.EndPCLLaboratoryResults_With_ResultOld(asyncResult);
                                    if (results != null)
                                    {
                                        allPatientPCLLaboratoryResultDetail = new ObservableCollection<PatientPCLLaboratoryResultDetail>();

                                        foreach (PatientPCLLaboratoryResultDetail patientPclLaboratoryResultDetail in results)
                                        {
                                            allPatientPCLLaboratoryResultDetail.Add(patientPclLaboratoryResultDetail);
                                            //▼====: #004
                                            if (patientPclLaboratoryResultDetail.PatientPCLLaboratoryResult.DigitalSignatureResultPath != "" && !mPDFView)
                                            {
                                                mPDFView = true;
                                                ((TabItem)TabPDFView).IsSelected = true;
                                                Uri uri = new Uri(patientPclLaboratoryResultDetail.PatientPCLLaboratoryResult.DigitalSignatureResultPath);
                                                PDFViewerSource = uri;
                                            }
                                            //▲====: #004
                                        }
                                        NotifyOfPropertyChange(() => allPatientPCLLaboratoryResultDetail);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                }
                                finally
                                {
                                    //Globals.IsBusy = false;
                                    IsLoading = false;
                                    this.DlgHideBusyIndicator();
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }

        private void PCLExamTypes_ByPatientPCLReqID(long PatientPCLReqID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0527_G1_DSYC });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new PCLsImportClient())
                {
                    IPCLsImport contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLExamTypes_ByPatientPCLReqID(PatientPCLReqID, Globals.DispatchCallback(
                        (asyncResult) =>
                        {
                            try
                            {
                                IList<PCLExamType> items = contract.EndPCLExamTypes_ByPatientPCLReqID(asyncResult);
                                if (items != null)
                                {
                                    ObjPCLExamTypes_ByPatientPCLReqID = new ObservableCollection<PCLExamType>(items);
                                    //Item Default
                                    var ItemDefault = new PCLExamType();
                                    ItemDefault.PCLExamTypeID = -1;
                                    ItemDefault.PCLExamTypeName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2033_G1_ChonLoaiXN2);
                                    ObjPCLExamTypes_ByPatientPCLReqID.Insert(0, ItemDefault);
                                    //Item Default
                                }
                                else
                                {
                                    ObjPCLExamTypes_ByPatientPCLReqID = null;
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                //Globals.IsBusy = false;
                                IsLoading = false;
                            }
                        }), null);
                }
            });
            t.Start();
        }

        private void GetPatientPCLImagingResults_ByPatientPCLReqID(long PatientPCLReqID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0527_G1_DSYC });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new PCLsImportClient())
                {
                    IPCLsImport contract = serviceFactory.ServiceInstance;

                    contract.BeginGetPatientPCLImagingResults_ByPatientPCLReqID(PatientPCLReqID, Globals.DispatchCallback(
                        (asyncResult) =>
                        {
                            try
                            {
                                allPatientPCLImagingResult = new ObservableCollection<PatientPCLImagingResult>();

                                var result = contract.EndGetPatientPCLImagingResults_ByPatientPCLReqID(asyncResult);
                                if (result != null)
                                {
                                    if (result != null)
                                    {
                                        PatientPCLImagingResult temp = new PatientPCLImagingResult();
                                        temp.PCLExamType = new PCLExamType();
                                        temp.PCLExamType.PCLExamTypeName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0258_G1_ChonLoaiHA);
                                        temp.PCLImgResultID = -1;
                                        curPatientPCLImagingResult = temp;

                                        allPatientPCLImagingResult.Add(temp);
                                        foreach (var patientPclImagingResult in result)
                                        {
                                            allPatientPCLImagingResult.Add(patientPclImagingResult);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                //Globals.IsBusy = false;
                                IsLoading = false;
                            }
                        }), null);
                }
            });
            t.Start();
        }

        private void GetInPatientAdmDisDetails(long PtRegistrationID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetInPatientAdmDisDetails(PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            curInPatientAdmDisDetails = contract.EndGetInPatientAdmDisDetails(asyncResult);
                            NotifyOfPropertyChange(() => curInPatientAdmDisDetails);

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }
        //▼====: #002
        //private void GetPCLResultFileStoreDetails(long PCLImgResultID)
        //{
        //    //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format("{0}...", eHCMSResources.Z0528_G1_DSKQua) });

        //    var t = new Thread(() =>
        //    {
        //        IsLoading = true;

        //        using (var serviceFactory = new PCLsClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;

        //            contract.BeginGetPCLResultFileStoreDetailsByPCLImgResultID(PCLImgResultID, Globals.DispatchCallback((asyncResult) =>
        //            {
        //                try
        //                {
        //                    var items = contract.EndGetPCLResultFileStoreDetailsByPCLImgResultID(asyncResult);
        //                    if (items != null)
        //                    {
        //                        //PclImageStoragePath = PclImageStoragePath.Replace("\\\\AXSERVER01", "D:");

        //                        allPCLResultFileStorageDetail = new ObservableCollection<PCLResultFileStorageDetail>();
        //                        foreach (var pclResultFileStorageDetail in items)
        //                        {
        //                            allPCLResultFileStorageDetail.Add(pclResultFileStorageDetail);
        //                        }
        //                        index = 0;
        //                        GetVideoAndImage(allPCLResultFileStorageDetail[index]);
        //                        checkIndex();
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                }
        //                finally
        //                {
        //                    //Globals.IsBusy = false;
        //                    IsLoading = false;
        //                }
        //            }), null);
        //        }


        //    });
        //    t.Start();
        //}
        //▲====: #002
        public void checkIndex()
        {
            if (allPCLResultFileStorageDetail.Count == 1)
            {
                btPrevious = false;
                btNext = false;
            }
            else
            {
                if (index > 0
                    && index < allPCLResultFileStorageDetail.Count - 1)
                {
                    btPrevious = true;
                    btNext = true;
                }
                else
                    if (index == 0)
                {
                    btPrevious = false;
                    btNext = true;
                }
                if (index == allPCLResultFileStorageDetail.Count - 1)
                {
                    btNext = false;
                    btPrevious = true;
                }
            }

        }
        //▼====: #002
        //private void GetVideoAndImage(PCLResultFileStorageDetail pRS)
        //{
        //    if (pRS.PCLResultLocation == "Images")
        //    {
        //        string LocalFileName = LocalFolderName + "\\" + pRS.PCLResultFileName;
        //        string PCLResultFileName = PclImageStoragePath
        //             + "\\Images" + "\\"
        //            + pRS.PCLResultFileName;
        //        try
        //        {
        //            if (!ClientFileManager.FolderExists(LocalFolderName))
        //            {
        //                ClientFileManager.CreateFolder(LocalFolderName);
        //            }
        //            if (!ClientFileManager.FileExists(LocalFileName))
        //            {
        //                ClientFileManager.CopyFile(PCLResultFileName, LocalFileName);
        //            }
        //            FileStream fs = File.OpenRead(LocalFileName);
        //            byte[] buffer = new byte[fs.Length];
        //            fs.Read(buffer, 0, buffer.Length);
        //            fs.Close();
        //            ObjGetVideoAndImage = new MemoryStream(buffer);
        //            CheckKetQuaReturn(ObjGetVideoAndImage);
        //        }
        //        catch (Exception ex)
        //        {
        //            if (ex.Message.Contains("unknown user name or bad password"))
        //            {
        //                Globals.ShowMessage(eHCMSResources.Z0506_G1_I, "");
        //            }
        //            else
        //                Globals.ShowMessage(eHCMSResources.Z0507_G1_KhongTaiDuocHinh, "");
        //        }
        //    }
        //    else
        //    {
        //        pRS.PCLResultFileName = PclImageStoragePath
        //            + "\\Videos" + "\\"
        //            + pRS.PCLResultFileName;
        //    }
        //}
        //▲====: #002
        private void CheckKetQuaReturn(Stream pStream)
        {
            if (pStream != null)
            {
                //if (itype == 1) /*Images*/
                {
                    var imgsource = new BitmapImage();
                    //imgsource.SetSource(pStream);
                    imgsource.StreamSource = pStream;
                    ObjBitmapImage = imgsource;
                    //SetVisibleForVideoControls(Visibility.Collapsed);
                    //SetVisibleForImgControls(Visibility.Visible);
                }
                //else if (itype == 2) /*Video*/
                //{
                //    SetVisibleForVideoControls(Visibility.Visible);
                //    SetVisibleForImgControls(Visibility.Collapsed);

                //    if (_currentView != null)
                //    {
                //        _currentView.SetObjectSource(pStream);
                //    }
                //}
                //else /*Document*/
                //{
                //    SetVisibleForVideoControls(Visibility.Collapsed);
                //    SetVisibleForImgControls(Visibility.Collapsed);
                //}

                //btChooseFileResultForPCLIsEnabled = true;
            }
            else
            {
                //SetVisibleForVideoControls(Visibility.Collapsed);
                //SetVisibleForImgControls(Visibility.Collapsed);

                //btChooseFileResultForPCLIsEnabled = false;
            }
        }
        #endregion

        #region Handle
        public void invisibleAll()
        {
            mTreatment = false;
            mPrescription = false;
            mPCL = false;
            mNoiTru = false;
            mHoiChan = false;
            mGPTM = false;
            mPCLImage = false;
            mPCLImage_New = false;
            mSmallProcedure = false;
            mCLSHinhAnhNgoaiVien = false;
            mPDFView = false;
            NotifyOfPropertyChange(() => mTreatment);
            NotifyOfPropertyChange(() => mPrescription);
            NotifyOfPropertyChange(() => mPCL);
            NotifyOfPropertyChange(() => mNoiTru);
            NotifyOfPropertyChange(() => mHoiChan);
            NotifyOfPropertyChange(() => mGPTM);
            NotifyOfPropertyChange(() => mPCLImage);
            NotifyOfPropertyChange(() => mPCLImage_New);
            NotifyOfPropertyChange(() => mSmallProcedure);
            NotifyOfPropertyChange(() => mCLSHinhAnhNgoaiVien);
            NotifyOfPropertyChange(() => mPDFView);
        }
        public void EnableAll()
        {
            mTreatment = true;
            mPrescription = true;
            mPCL = true;
            mNoiTru = true;
            mHoiChan = true;
            mGPTM = true;
            mPCLImage = true;
            mPCLImage_New = true;
            mSmallProcedure = true;
            mCLSHinhAnhNgoaiVien = true;
            mPDFView = true;
            NotifyOfPropertyChange(() => mTreatment);
            NotifyOfPropertyChange(() => mPrescription);
            NotifyOfPropertyChange(() => mPCL);
            NotifyOfPropertyChange(() => mNoiTru);
            NotifyOfPropertyChange(() => mHoiChan);
            NotifyOfPropertyChange(() => mGPTM);
            NotifyOfPropertyChange(() => mPCLImage);
            NotifyOfPropertyChange(() => mPCLImage_New);
            NotifyOfPropertyChange(() => mSmallProcedure);
            NotifyOfPropertyChange(() => mCLSHinhAnhNgoaiVien);
            NotifyOfPropertyChange(() => mPDFView);
        }
        public void Handle(PatientTreeChange obj)
        {
            if(Registration_DataStorage == null || Registration_DataStorage.CurrentPatient == null)
            {
                return;
            }
            if (obj != null)
            {
                curPatientServicesTree = obj.curPatientServicesTree;
                if (curPatientServicesTree.Level == 2)
                {
                    switch (curPatientServicesTree.PatientSummaryType)
                    {
                        case (int)AllLookupValues.PatientSummary.KhamBenh_ChanDoan:
                            invisibleAll();
                            mTreatment = true;
                            ((TabItem)TabKhamBenh).IsSelected = true;
                            if (curPatientServicesTree.InPt)
                            {
                                LoadDiagnosisTreatmentViewByDTItemID_InPt(Convert.ToInt64(curPatientServicesTree.Description), curPatientServicesTree.NodeID);
                            }
                            else
                            {
                                GetDiagnosisTreatmentByDTItemID(curPatientServicesTree.NodeID);
                            }


                            //PrescriptID = curPatientServicesTree.PrescriptID;
                            //IsEnabled = true;

                            break;
                        case (int)AllLookupValues.PatientSummary.CanLamSang_XetNghiem:
                            invisibleAll();
                            mPCL = true;
                            ((TabItem)TabPCL).IsSelected = true;
                            PCLLaboratoryResults_With_ResultOld(Registration_DataStorage.CurrentPatient.PatientID, curPatientServicesTree.NodeID, curPatientServicesTree.InPt);
                            break;
                        case (int)AllLookupValues.PatientSummary.CanLamSang_HinhAnh:
                            invisibleAll();
                            //var VMPCL = Globals.GetViewModel<IHtmlReport>();
                            //PCLOldView = VMPCL;
                            //this.ActivateItem(VMPCL);
                            CheckTemplatePCLResultByReqID(curPatientServicesTree.NodeID, curPatientServicesTree.InPt);
                            break;
                        case (int)AllLookupValues.PatientSummary.ToaThuoc:
                            invisibleAll();
                            mPrescription = true;
                            ((TabItem)TabToaThuoc).IsSelected = true;
                            if (curPatientServicesTree.InPt)
                            {
                                LoadPrescriptionDetailViewByID_InPt((long)curPatientServicesTree.ParentID);
                            }
                            else
                            {
                                GetPrescriptionByID(curPatientServicesTree.NodeID);
                            }
                            break;
                        case (int)AllLookupValues.PatientSummary.NoiTru:
                            invisibleAll();
                            mNoiTru = true;
                            //((TabItem)TabNoiTru).IsSelected = true;
                            //GetInPatientAdmDisDetails((long)curPatientServicesTree.ParentID);
                            break;
                        case (int)AllLookupValues.PatientSummary.ThuThuat:
                            invisibleAll();
                            mSmallProcedure = true;
                            ((TabItem)TabThuThuat).IsSelected = true;
                            var VMTT = Globals.GetViewModel<IHtmlReport>();
                            ThuThuatView = VMTT;
                            this.ActivateItem(VMTT);
                            GetSmallProcedureForPrintPreview((long)curPatientServicesTree.NodeID, curPatientServicesTree.InPt);
                            break;

                        case (int)AllLookupValues.PatientSummary.CanLamSang_HinhAnh_NgoaiVien:
                            invisibleAll();
                            mCLSHinhAnhNgoaiVien = true;
                            ((TabItem)TabCLSHinhAnhNgoaiVien).IsSelected = true;
                            var VM = Globals.GetViewModel<IPCLDeptImagingResult_Consultation>();
                            VM.IsEdit = false;
                            VM.Reset();
                            CLSHinhAnhNgoaiVien = VM;
                            VM.LoadDataFromPK((long)curPatientServicesTree.NodeID);
                            this.ActivateItem(VM);
                            break;
                        case (int)AllLookupValues.PatientSummary.CanLamSang_TieuChiNV:
                            
                            
                            GetPCLResult_Criterion((long)curPatientServicesTree.ParentID);
                            break;
                        case (int)AllLookupValues.PatientSummary.HoiChan:
                            invisibleAll();
                            TitleReport = "Hội chẩn";
                            mPCLImage_New = true;
                            ((TabItem)TabHinhAnhPCL_New).IsSelected = true;
                            //PCLResultParamImpName = curPatientServicesTree.Description;
                            btnViewPrintNew_HoiChan((long)curPatientServicesTree.NodeID, curPatientServicesTree.InPt);
                            break;
                    }
                }
                else
                    if (curPatientServicesTree.Level == 3)
                {
                    mTreatment = false;
                    mPrescription = false;
                    mPCL = false;
                    mNoiTru = false;
                    mHoiChan = false;
                    mGPTM = false;
                    mPCLImage = true;
                    mPCLImage_New = false;
                    mSmallProcedure = false;

                    GetPatientPCLImagingResults_ByPatientPCLReqID(curPatientServicesTree.NodeID);
                }
            }

        }

        public void Handle(PatientSummaryChange obj)
        {
            if (obj != null)
            {
                switch ((AllLookupValues.PatientSummary)obj.curPatientSummary)
                {
                    case AllLookupValues.PatientSummary.KhamBenh_ChanDoan:
                        invisibleAll();
                        mTreatment = true;
                        ((TabItem)TabKhamBenh).IsSelected = true;

                        break;
                    case AllLookupValues.PatientSummary.CanLamSang_XetNghiem:
                        invisibleAll();
                        mPCL = true;
                        ((TabItem)TabPCL).IsSelected = true;
                        //
                        break;
                    case AllLookupValues.PatientSummary.CanLamSang_HinhAnh:
                        invisibleAll();
                        mPCLImage = true;
                        ((TabItem)TabHinhAnhPCL).IsSelected = true;
                        break;
                    case AllLookupValues.PatientSummary.ToaThuoc:
                        invisibleAll();
                        mPrescription = true;
                        ((TabItem)TabToaThuoc).IsSelected = true;
                        break;
                    case AllLookupValues.PatientSummary.NoiTru:
                        invisibleAll();
                        //mNoiTru = true;
                        //((TabItem)TabNoiTru).IsSelected = true;
                        break;
                    case AllLookupValues.PatientSummary.ThuThuat:
                        invisibleAll();
                        mSmallProcedure = true;
                        ((TabItem)TabThuThuat).IsSelected = true;
                        var VMTT = Globals.GetViewModel<IHtmlReport>();
                        ThuThuatView = VMTT;
                        this.ActivateItem(VMTT);
                        break;
                    case AllLookupValues.PatientSummary.CanLamSang_HinhAnh_NgoaiVien:
                        invisibleAll();
                        mCLSHinhAnhNgoaiVien = true;
                        ((TabItem)TabCLSHinhAnhNgoaiVien).IsSelected = true;
                        //refresh lai trang chua no

                        var VM = Globals.GetViewModel<IPCLDeptImagingResult_Consultation>();
                        VM.IsEdit = false;
                        VM.Reset();
                        CLSHinhAnhNgoaiVien = VM;
                        this.ActivateItem(VM);

                        break;
                    default:
                        //EnableAll();
                        invisibleAll();
                        break;
                }

            }

        }

        #endregion
        //▼====: #002
        //public void iNext_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        //{
        //    index++;
        //    GetVideoAndImage(allPCLResultFileStorageDetail[index]);
        //    checkIndex();
        //}

        //public void iPrevious_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        //{
        //    index--;
        //    GetVideoAndImage(allPCLResultFileStorageDetail[index]);
        //    checkIndex();
        //}
        //▲====: #002
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

            mPatientSumaryRecord_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mPtePatientSummaryRecord,
                                               (int)oConsultationEx.mPatientSumaryRecord_Xem, (int)ePermission.mView);


        }
        #region checking account

        private bool _mPatientSumaryRecord_Xem = true;

        public bool mPatientSumaryRecord_Xem
        {
            get
            {
                return _mPatientSumaryRecord_Xem;
            }
            set
            {
                if (_mPatientSumaryRecord_Xem == value)
                    return;
                _mPatientSumaryRecord_Xem = value;
                NotifyOfPropertyChange(() => mPatientSumaryRecord_Xem);
            }
        }



        #endregion

        public void Handle(PatientInfoChange message)
        {
            if (message != null)
            {
                curDiagnosisTreatment = null;
                curInPatientAdmDisDetails = null;
                curPatientPCLImagingResult = null;
                curPrescription = null;
                curPatientServicesTree = null;
                refIDC10List = null;
                allPrescription = null;
                NotifyOfPropertyChange(() => allPrescription);
            }
        }

        public void Grid_Loaded(object sender)
        {
            StackPanel PendingClientsGrid = sender as StackPanel;
            if (PendingClientsGrid.DataContext == null || PendingClientsGrid.DataContext.GetType() != typeof(Prescription)) return;
            GetPrescriptionDetailXml getfuns = new GetPrescriptionDetailXml();
            getfuns.getPendingClientGrid(PendingClientsGrid, PendingClientsGrid.DataContext as Prescription);
        }
        public void grdPrescription_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }
        public void ResetForm()
        {
            allPatientPCLLaboratoryResultDetail = new ObservableCollection<PatientPCLLaboratoryResultDetail>();
            //curDiagnosisTreatment = new DiagnosisTreatment();
            //allPrescription = new ObservableCollection<Prescription>();
            curInPatientAdmDisDetails = new InPatientAdmDisDetails();
            curDiagnosisTreatment = null;
            curInPatientAdmDisDetails = null;
            curPatientPCLImagingResult = null;
            curPrescription = null;
            curPatientServicesTree = null;
            refIDC10List = null;
            allPrescription = null;
            var VM = Globals.GetViewModel<IPCLDeptImagingResult_Consultation>();
            VM.IsEdit = false;
            VM.Reset();
            CLSHinhAnhNgoaiVien = VM;
            NotifyOfPropertyChange(() => allPrescription);
        }

        public void Handle(ShowPatientInfo_KHAMBENH_LSBENHAN<Patient, PatientRegistration, PatientRegistrationDetail> message)
        {
            if (Registration_DataStorage.CurrentPatient != null)
            {
                ResetForm();
            }
        }
        private bool _IsShowSummaryContent = true;
        public bool IsShowSummaryContent
        {
            get => _IsShowSummaryContent; set
            {
                _IsShowSummaryContent = value;
                NotifyOfPropertyChange(() => IsShowSummaryContent);
                if (UCPatientTreeView != null)
                    UCPatientTreeView.IsShowSummaryContent = IsShowSummaryContent;
            }
        }
        public void InitPatientInfo()
        {
            UCPatientTreeView.InitPatientInfo();
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
                UCHeaderInfoPMR.Registration_DataStorage = Registration_DataStorage;
                UCPatientTreeView.Registration_DataStorage = Registration_DataStorage;
            }
        }
        private PatientPCLRequest _ObjPatientPCLRequest_SearchPaging_Selected;
        public PatientPCLRequest ObjPatientPCLRequest_SearchPaging_Selected
        {
            get { return _ObjPatientPCLRequest_SearchPaging_Selected; }
            set
            {
                _ObjPatientPCLRequest_SearchPaging_Selected = value;
                NotifyOfPropertyChange(() => ObjPatientPCLRequest_SearchPaging_Selected);
            }
        }
        private ObservableCollection<PatientPCLImagingResultDetail> _allPatientPCLImagingResultDetail;
        public ObservableCollection<PatientPCLImagingResultDetail> allPatientPCLImagingResultDetail
        {
            get
            {
                return _allPatientPCLImagingResultDetail;
            }
            set
            {
                if (_allPatientPCLImagingResultDetail == value)
                    return;
                _allPatientPCLImagingResultDetail = value;
                NotifyOfPropertyChange(() => allPatientPCLImagingResultDetail);
            }
        }
        private void CheckTemplatePCLResultByReqID(long PatientPCLReqID, bool InPt)
        {

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginCheckTemplatePCLResultByReqID(PatientPCLReqID, InPt, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            bool IsNewTemplate = false;
                            long V_ReportForm = 0;
                            long PCLImgResultID = 0;
                            long V_PCLRequestType = 0;
                            string TemplateResultString = "";
                            var result = contract.EndCheckTemplatePCLResultByReqID(out IsNewTemplate, out V_ReportForm, out PCLImgResultID, out V_PCLRequestType, out TemplateResultString,asyncResult);

                            if (IsNewTemplate)
                            {
                                mPCLImage_New = true;
                                ((TabItem)TabHinhAnhPCL_New).IsSelected = true;
                                TitleReport = "Cận lâm sàng Imaging";
                                //PCLResultParamImpName = curPatientServicesTree.Description;
                                btnViewPrintNew(V_ReportForm, PCLImgResultID, V_PCLRequestType);
                            }
                            else
                            {
                                mPCLImage = true;
                                ((TabItem)TabHinhAnhPCL).IsSelected = true;
                                //PCLResultParamImpName = curPatientServicesTree.Description;
                                if(string.IsNullOrEmpty(TemplateResultString))
                                {
                                    MessageBox.Show("Không tìm thấy kết quả", eHCMSResources.T0432_G1_Error);
                                }
                                else
                                {
                                    PCLOldView.NavigateToString(TemplateResultString);
                                }
                            }
                            //GetPatientPCLRequestResultsByReqID(curPatientServicesTree.NodeID, curPatientServicesTree.InPt);

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            t.Start();
        }
        private void GetPCLResult_Criterion(long ServiceRecID)
        {

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetPCLResult_Criterion(ServiceRecID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string TemplateResultString = "";
                            long V_ResultType = 0;
                            var result = contract.EndGetPCLResult_Criterion(out V_ResultType, out TemplateResultString,  asyncResult);
                            mPCLImage = true;
                            ((TabItem)TabHinhAnhPCL).IsSelected = true;
                            //PCLResultParamImpName = curPatientServicesTree.Description;
                            if (string.IsNullOrEmpty(TemplateResultString))
                            {
                                MessageBox.Show("Không tìm thấy kết quả", eHCMSResources.T0432_G1_Error);
                            }
                            else 
                            {
                                if(V_ResultType == (long)AllLookupValues.FileStorageResultType.IMAGES)
                                {
                                    invisibleAll();
                                    mPCLImage = true;
                                    ((TabItem)TabHinhAnhPCL).IsSelected = true;
                                    PCLOldView.NavigateToString(TemplateResultString);
                                }
                                else if (V_ResultType == (long)AllLookupValues.FileStorageResultType.DOCUMENTS)
                                {
                                    invisibleAll();
                                    mPDFView = true;
                                    ((TabItem)TabPDFView).IsSelected = true;
                                    Uri uri = new Uri(TemplateResultString);
                                    PDFViewerSource = uri;
                                }
                            }
                            
                            //GetPatientPCLRequestResultsByReqID(curPatientServicesTree.NodeID, curPatientServicesTree.InPt);

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            t.Start();
        }
        static Stream GetResourceStream(string resourceName)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        }
        //private void GetPatientPCLRequestResultsByReqID(long PatientPCLReqID, bool InPt)
        //{
        //    PatientPCLRequestSearchCriteria mSearchCriteria = new PatientPCLRequestSearchCriteria();
        //    mSearchCriteria.PatientPCLReqID = PatientPCLReqID;
        //    mSearchCriteria.V_PCLMainCategory = (long)AllLookupValues.V_PCLMainCategory.Imaging;
        //    mSearchCriteria.PatientFindBy = InPt ? AllLookupValues.PatientFindBy.NOITRU : AllLookupValues.PatientFindBy.NGOAITRU;
        //    var t = new Thread(() =>
        //    {
        //        using (var serviceFactory = new PCLsClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;
        //            contract.BeginGetPatientPCLRequestResultsByReqID(mSearchCriteria, Globals.DispatchCallback((asyncResult) =>
        //            {
        //                try
        //                {
        //                    var mPCLRequest = contract.EndGetPatientPCLRequestResultsByReqID(asyncResult);
        //                    if (mPCLRequest != null && mPCLRequest.PatientPCLReqID != 0)
        //                    {
        //                        ObjPatientPCLRequest_SearchPaging_Selected = mPCLRequest;
        //                        LoadData(ObjPatientPCLRequest_SearchPaging_Selected);
        //                        //PCLImagingResults_With_ResultOld(ObjPatientPCLRequest_SearchPaging_Selected.PatientID, ObjPatientPCLRequest_SearchPaging_Selected.PatientPCLReqID, (long)ObjPatientPCLRequest_SearchPaging_Selected.V_PCLRequestType);
        //                    }
        //                    else
        //                    {
        //                        this.HideBusyIndicator();
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
        //                    this.HideBusyIndicator();
        //                }
        //            }), null);
        //        }
        //    });
        //    t.Start();
        //}
        //public void LoadData(PatientPCLRequest message)
        //{
        //    Coroutine.BeginExecute(LoadDataCoroutine(message));
        //}
        //public ObservableCollection<PCLResultFileStorageDetailClient> ObjGetPCLResultFileStoreDetails
        //{
        //    get { return _ObjGetPCLResultFileStoreDetails; }
        //    set
        //    {
        //        _ObjGetPCLResultFileStoreDetails = value;
        //        NotifyOfPropertyChange(() => ObjGetPCLResultFileStoreDetails);
        //    }
        //}
        //private ObservableCollection<PCLResultFileStorageDetailClient> _ObjGetPCLResultFileStoreDetails;
        //private IEnumerator<IResult> LoadDataCoroutine(PatientPCLRequest p)
        //{
        //    curPatientPCLImagingResult = new PatientPCLImagingResult();
        //    //isLoadingDetail = true;

        //    //if (curPatientPCLImagingResult == null)
        //    //{
        //    //    KhoiTao();
        //    //}
        //    curPatientPCLImagingResult.PatientPCLRequest = p;

        //    curPatientPCLImagingResult.UserOfficialAccountID = Globals.DoctorAccountBorrowed.StaffID;
        //    curPatientPCLImagingResult.PatientPCLReqID = p.PatientPCLReqID;
        //    curPatientPCLImagingResult.PCLExamTypeID = p.PCLExamTypeID.GetValueOrDefault();
        //    curPatientPCLImagingResult.PCLExamType = new PCLExamType
        //    {
        //        PCLExamTypeID = p.PCLExamTypeID.GetValueOrDefault(),
        //        PCLExamTypeName = p.PCLExamTypeName,
        //        V_PCLMainCategory = p.V_PCLMainCategory,
        //        V_ReportForm = p.V_ReportForm
        //    };
        //    //==== 20161007 CMN Begin: Add List for delete file
        //    //FileForDelete = new List<PCLResultFileStorageDetail>();
        //    //==== 20161007 CMN End.
        //    /*▼====: #003*/
        //    //var ResultFileStoreDetails = new LoadPCLResultFileStoreDetailsTask(p.PatientID, p.PatientPCLReqID, p.PCLExamTypeID);
        //    //var ResultFileStoreDetails = new LoadPCLResultFileStoreDetailsTask(p.PatientID, p.PatientPCLReqID, p.PCLExamTypeID, (long)p.V_PCLRequestType);
        //    ///*▲====: #003*/
        //    //yield return ResultFileStoreDetails;
        //    //if (ObjGetPCLResultFileStoreDetails == null)
        //    //{
        //    //    ObjGetPCLResultFileStoreDetails = new ObservableCollection<PCLResultFileStorageDetailClient>();
        //    //}
        //    //ObjGetPCLResultFileStoreDetails.Clear();
        //    //if (ResultFileStoreDetails.ObjGetPCLResultFileStoreDetails != null)
        //    //{
        //    //    foreach (var item in ResultFileStoreDetails.ObjGetPCLResultFileStoreDetails)
        //    //    {
        //    //        ObjGetPCLResultFileStoreDetails.Add(new PCLResultFileStorageDetailClient { IOStream = null, ObjectResult = item });
        //    //    }
        //    //}
        //    /*▼====: #003*/
        //    //var LoadPCLImagingResultsID = new LoadPatientPCLImagingResults_ByIDTask(p.PatientPCLReqID, p.PCLExamTypeID.GetValueOrDefault());
        //    var LoadPCLImagingResultsID = new LoadPatientPCLImagingResults_ByIDTask(p.PatientPCLReqID, p.PCLExamTypeID.GetValueOrDefault(), (long)p.V_PCLRequestType);
        //    /*▲====: #003*/
        //    yield return LoadPCLImagingResultsID;
        //    if (LoadPCLImagingResultsID.ObjPatientPCLImagingResult != null && LoadPCLImagingResultsID.ObjPatientPCLImagingResult.PatientPCLReqID.GetValueOrDefault() > 0)
        //    {
        //        curPatientPCLImagingResult.PCLImgResultID = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.PCLImgResultID;
        //        curPatientPCLImagingResult.TemplateResultString = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.TemplateResultString;
        //    }
        //    else
        //    {
        //        if (curPatientPCLImagingResult != null)
        //        {
        //            curPatientPCLImagingResult.PCLImgResultID = 0;
        //            curPatientPCLImagingResult.TemplateResultString = null;
        //        }
        //    }
        //    //if (mPCLImage)
        //    //{
        //    //    //NavigateAsync();
        //    //    if (File.Exists(curPatientPCLImagingResult.TemplateResultString))
        //    //    {
        //    //        string text = System.IO.File.ReadAllText(curPatientPCLImagingResult.TemplateResultString);
        //    //        PCLOldView.NavigateToString(text);  
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    btnViewPrintNew();
        //    //}
        //    yield break;

        //}
        //private async void NavigateAsync()
        //{
        //    if (webView != null)
        //    {
        //        await webView.EnsureCoreWebView2Async();
        //        if (File.Exists(curPatientPCLImagingResult.TemplateResultString))
        //        {
        //            string text = System.IO.File.ReadAllText(curPatientPCLImagingResult.TemplateResultString);
        //            webView.CoreWebView2.NavigateToString(text);
        //            webView.IsEnabled = false;
        //        }
        //        else
        //        {
        //            webView.CoreWebView2.Navigate("about:blank");
        //        }
        //    }
        //}
        private RemoteDocumentSource _reportModel;
        public RemoteDocumentSource ReportModel
        {
            get { return _reportModel; }
            set
            {
                _reportModel = value;
                NotifyOfPropertyChange(() => ReportModel);
            }
        }
        //WebView2 webView = null;
        //public void WebView_Loaded(object sender, RoutedEventArgs e)
        //{
        //    webView = (WebView2)sender;
        //}
        public void btnViewPrintLab()
        {
            if (curPatientServicesTree.NodeID <= 0)
            {
                //MessageBox.Show(eHCMSResources.Z2375_G1_ChonYCXN);
                return;
            }
            if (!curPatientServicesTree.NodeText.Contains("Đã ký số"))
            {
                MessageBox.Show("Phiếu chỉ định chưa được áp chữ ký số. Vui lòng liên hệ Khoa xét nghiệm(Nếu cần)");
                if (!curPatientServicesTree.InPt)
                {
                    return;
                }
            }
            //var proAlloc = Globals.GetViewModel<ICommonPreviewView>();
            //proAlloc.PatientPCLReqID =(int)PatientPCLReqID;
            //proAlloc.V_PCLRequestType = PCLRequestTypeID;
            //proAlloc.FindPatient = PCLRequestTypeID > 0 && PCLRequestTypeID == (long)AllLookupValues.V_PCLRequestType.NOI_TRU ? (int)AllLookupValues.PatientFindBy.NOITRU : (int)AllLookupValues.PatientFindBy.NGOAITRU;
            //proAlloc.eItem = ReportName.PCLDEPARTMENT_LABORATORY_RESULT;

            //var instance = proAlloc as Conductor<object>;s
            //Globals.ShowDialog(instance, (o) => { });

            Action<ICommonPreviewView> onInitDlg = (proAlloc) =>
            {
                proAlloc.PatientID = Registration_DataStorage.CurrentPatient.PatientID;
                proAlloc.PatientPCLReqID = (int)curPatientServicesTree.NodeID;
                proAlloc.V_PCLRequestType = curPatientServicesTree.InPt ? (long)AllLookupValues.V_PCLRequestType.NOI_TRU : (long)AllLookupValues.V_PCLRequestType.NGOAI_TRU;
                proAlloc.FindPatient = curPatientServicesTree.InPt ? (int)AllLookupValues.PatientFindBy.NOITRU : (int)AllLookupValues.PatientFindBy.NGOAITRU;
                proAlloc.StaffName = "";
                proAlloc.eItem = ReportName.PCLDEPARTMENT_LABORATORY_RESULT;
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }
        public void btnViewPrintNew(long V_ReportForm,long PCLImgResultID,long V_PCLRequestType)
        {
            //if (curPatientPCLImagingResult.PatientPCLRequest == null)
            //{
            //    return;
            //}
            //if (curPatientPCLImagingResult.PCLExamType == null || curPatientPCLImagingResult.PCLExamType.V_ReportForm == 0)
            //{
            //    return;
            //}
            DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
            ReportModel = null;
            switch (V_ReportForm)
            {
                case (long)AllLookupValues.V_ReportForm.Mau_0_Hinh:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_0_Hinh").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_1_Hinh:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_1_Hinh").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_2_Hinh:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_3_Hinh:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_3_Hinh").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_4_Hinh:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_4_Hinh").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_6_Hinh:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_6_Hinh").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_Realtime_PCR:
                    if (Globals.ServerConfigSection.CommonItems.IsApplyPCRDual)
                    {
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Realtime_PCR_Cov_Dual").PreviewModel;
                    }
                    else
                    {
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Realtime_PCR_Cov").PreviewModel;
                    }
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_Test_Nhanh:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Test_Nhanh_Cov").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_Xet_Nghiem:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Xet_Nghiem").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_Helicobacter_Pylori:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Helicobacter_Pylori").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_Sieu_Am_Tim:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Sieu_Am_Tim").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_Sieu_Am_San_4D:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Sieu_Am_San_4D").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_Dien_Tim:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Dien_Tim").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_Dien_Nao:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Dien_Nao").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_ABI:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_ABI").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_Dien_Tim_Gang_Suc:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Dien_Tim_Gang_Suc").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_CLVT_Hai_Ham:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_CLVT_Hai_Ham").PreviewModel;
                    break;
                //▼====: #003
                case (long)AllLookupValues.V_ReportForm.Mau_Sieu_Am_San_4D_New:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Sieu_Am_San_4D_New").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_6_Hinh_2_New:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_6_Hinh_2_New").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_6_Hinh_1_New:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_6_Hinh_1_New").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_Sieu_Am_Tim_New:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Sieu_Am_Tim_New").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_Noi_Soi_9_Hinh:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Noi_Soi_9_Hinh").PreviewModel;
                    break;
              
                //▲====: #003
                default:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New").PreviewModel;
                    break;
            }
            rParams["PCLImgResultID"].Value = PCLImgResultID;
            rParams["V_PCLRequestType"].Value = V_PCLRequestType;
            rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
            rParams["parHospitalCode"].Value = Globals.ServerConfigSection.Hospitals.HospitalCode;
            rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
            rParams["parHospitalAddress"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalAddress;
            ReportModel.CreateDocument(rParams);
        }
        public void btnViewPrintNew_HoiChan(long ID, bool InPt)
        {
            DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
       
            ReportModel = null;
            ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XRpt_BienBanHoiChan").PreviewModel;
            rParams["DiagConsultationSummaryID"].Value = ID;
            rParams["V_RegistrationType"].Value = InPt ? (long)AllLookupValues.RegistrationType.NOI_TRU : (long)AllLookupValues.RegistrationType.NGOAI_TRU ;
            rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
            rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;

            ReportModel.CreateDocument(rParams);
        }
        private void GetSmallProcedureForPrintPreview(long aSmallProcedureID,bool InPt)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetSmallProcedure(0, aSmallProcedureID, InPt ? (long)AllLookupValues.RegistrationType.NOI_TRU : (long)AllLookupValues.RegistrationType.NGOAI_TRU, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var mSmallProcedure = contract.EndGetSmallProcedure(asyncResult);
                            if (mSmallProcedure != null && mSmallProcedure.SmallProcedureID > 0 && Registration_DataStorage != null
                                && Registration_DataStorage.CurrentPatientRegistration != null)
                            {
                                PrintProcedureProcess(this, mSmallProcedure, Registration_DataStorage.CurrentPatientRegistration);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.G0442_G1_TBao);
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
        private void PrintProcedureProcess(ViewModelBase aView, SmallProcedure aSmallProcedure, PatientRegistration CurrentRegistration)
        {
            this.ShowBusyIndicator();
            Patient CurrentPatient = CurrentRegistration.Patient;
            if (aSmallProcedure == null || aSmallProcedure.SmallProcedureID == 0)
            {
                return;
            }

            var t = new Thread(() =>
            {
                aView.ShowBusyIndicator();
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetVideoAndImage(Path.Combine(Globals.ServerConfigSection.CommonItems.ReportTemplatesLocation, "TT-PT.html"), Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var mFileArray = contract.EndGetVideoAndImage(asyncResult);
                            if (mFileArray == null || mFileArray.Length == 0)
                            {
                                ThuThuatView.NavigateToString("");
                                return;
                            }
                            MemoryStream mMemStream = new MemoryStream(mFileArray);
                            StreamReader mReader = new StreamReader(mMemStream);
                            string mContentBody = mReader.ReadToEnd();
                            mContentBody = Globals.ReplaceStylesHref(mContentBody);
                            string DateTimeStringFormat = "{0:HH} giờ {0:mm} phút, ngày {0:dd} tháng {0:MM} năm {0:yyyy}";
                            string DateStringFormat = "Ngày {0:dd} tháng {0:MM} năm {0:yyyy}";
                            mContentBody = mContentBody.Replace("[ReportDepartmentOfHealth]", Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth);
                            mContentBody = mContentBody.Replace("[ReportHospitalName]", Globals.ServerConfigSection.CommonItems.ReportHospitalName);
                            mContentBody = mContentBody.Replace("[FullName]", CurrentPatient == null ? "" : CurrentPatient.FullName);
                            mContentBody = mContentBody.Replace("[DOB]", CurrentPatient == null || CurrentPatient.DOB == null ? "" : CurrentPatient.DOB.Value.ToString("yyyy"));
                            mContentBody = mContentBody.Replace("[Gender]", CurrentPatient == null ? "" : CurrentPatient.GenderString);
                            mContentBody = mContentBody.Replace("[PatientCode]", CurrentPatient == null ? "" : CurrentPatient.PatientCode);
                            mContentBody = mContentBody.Replace("[CompletedTime]", (aSmallProcedure == null || aSmallProcedure.CompletedDateTime == DateTime.MinValue) ? string.Format(DateTimeStringFormat, aSmallProcedure.ProcedureDateTime) : string.Format(DateTimeStringFormat, aSmallProcedure.CompletedDateTime));
                            mContentBody = mContentBody.Replace("[ProcedureTime]", aSmallProcedure == null ? "" : string.Format(DateTimeStringFormat, aSmallProcedure.ProcedureDateTime));
                            mContentBody = mContentBody.Replace("[DiagnosisFinal]", aSmallProcedure == null ? "" : aSmallProcedure.AfterICD10.DiseaseNameVN);
                            mContentBody = mContentBody.Replace("[ProcedureMethod]", aSmallProcedure == null ? "" : aSmallProcedure.ProcedureMethod);
                            //▼====== #014
                            mContentBody = mContentBody.Replace("[ProcedureType]", aSmallProcedure.V_Surgery_Tips_Type == null ? "" : string.Format(aSmallProcedure.V_Surgery_Tips_Type.ObjectValue));
                            //▼====== #015
                            //mContentBody = mContentBody.Replace("[DeptName]", aSmallProcedure == null ? "" : string.Format(aSmallProcedure.DepartmentName));
                            mContentBody = mContentBody.Replace("[DeptName]", aSmallProcedure == null || aSmallProcedure.DepartmentName == null || aSmallProcedure.DepartmentName == "" ? (Globals.DeptLocation.RefDepartment != null ? Globals.DeptLocation.RefDepartment.DeptName : "") : aSmallProcedure.DepartmentName);
                            //▲====== #015
                            if (aSmallProcedure != null && aSmallProcedure.BeforeICD10 != null)
                            {
                                mContentBody = mContentBody.Replace("[Diagnosis]", aSmallProcedure == null ? "" : aSmallProcedure.BeforeICD10.DiseaseNameVN);
                            }
                            if (aSmallProcedure != null && aSmallProcedure.V_AnesthesiaType == 0)
                            {
                                mContentBody = mContentBody.Replace("[NarcoticMethod]", aSmallProcedure == null ? "" : aSmallProcedure.NarcoticMethod == null ? "" : aSmallProcedure.NarcoticMethod);
                            }
                            else
                            {
                                Lookup temp = Globals.AllLookupValueList.Where(x => x.LookupID == aSmallProcedure.V_AnesthesiaType).ToObservableCollection().FirstOrDefault();
                                mContentBody = mContentBody.Replace("[NarcoticMethod]", temp.ObjectValue);
                            }
                            //▲====== #014
                            mContentBody = mContentBody.Replace("[DateTime]", aSmallProcedure == null ? "" : string.Format(DateStringFormat, aSmallProcedure.ProcedureDateTime));
                            mContentBody = mContentBody.Replace("[ProcedureDescription]", aSmallProcedure == null || string.IsNullOrEmpty(aSmallProcedure.TrinhTu) ? "" : aSmallProcedure.TrinhTu.Replace("\n", "<br/>"));
                            mContentBody = mContentBody.Replace("[PtRegistrationCode]", CurrentRegistration == null ? "" : CurrentRegistration.PtRegistrationCode);
                            mContentBody = mContentBody.Replace("[ProcedureType]", aSmallProcedure == null || aSmallProcedure.V_Surgery_Tips_Type == null ? "" : aSmallProcedure.V_Surgery_Tips_Type.ObjectValue);
                            List<Staff> ProcedureStaffs = new List<Staff>();
                            List<Staff> NarcoticStaffs = new List<Staff>();
                            List<Staff> NurseStaffs = new List<Staff>();
                            // 20200207 TNHX: lấy loại user lên trên xem/in
                            if (aSmallProcedure != null)
                            {
                                if (aSmallProcedure.ProcedureDoctorStaff != null)
                                {
                                    Staff TempProcedureDoctorStaff = ObjectCopier.DeepCopy(aSmallProcedure.ProcedureDoctorStaff);
                                    TempProcedureDoctorStaff.FullName = aSmallProcedure.ProcedureDoctorStaff.RefStaffCategory.StaffCatgDescription + aSmallProcedure.ProcedureDoctorStaff.FullName;
                                    ProcedureStaffs.Add(TempProcedureDoctorStaff);
                                }
                                if (aSmallProcedure.ProcedureDoctorStaff2 != null)
                                {
                                    Staff TempProcedureDoctorStaff2 = ObjectCopier.DeepCopy(aSmallProcedure.ProcedureDoctorStaff2);
                                    TempProcedureDoctorStaff2.FullName = aSmallProcedure.ProcedureDoctorStaff2.RefStaffCategory.StaffCatgDescription + aSmallProcedure.ProcedureDoctorStaff2.FullName;
                                    ProcedureStaffs.Add(TempProcedureDoctorStaff2);
                                }
                                if (aSmallProcedure.NarcoticDoctorStaff != null)
                                {
                                    Staff TempNarcoticDoctorStaff = ObjectCopier.DeepCopy(aSmallProcedure.NarcoticDoctorStaff);
                                    TempNarcoticDoctorStaff.FullName = aSmallProcedure.NarcoticDoctorStaff.RefStaffCategory.StaffCatgDescription + aSmallProcedure.NarcoticDoctorStaff.FullName;
                                    NarcoticStaffs.Add(TempNarcoticDoctorStaff);
                                }
                                if (aSmallProcedure.NarcoticDoctorStaff2 != null)
                                {
                                    Staff TempNarcoticDoctorStaff2 = ObjectCopier.DeepCopy(aSmallProcedure.NarcoticDoctorStaff2);
                                    TempNarcoticDoctorStaff2.FullName = aSmallProcedure.NarcoticDoctorStaff2.RefStaffCategory.StaffCatgDescription + aSmallProcedure.NarcoticDoctorStaff2.FullName;
                                    NarcoticStaffs.Add(TempNarcoticDoctorStaff2);
                                }
                                if (aSmallProcedure.NurseStaff != null)
                                {
                                    Staff TempNurseStaff = ObjectCopier.DeepCopy(aSmallProcedure.NurseStaff);
                                    TempNurseStaff.FullName = aSmallProcedure.NurseStaff.RefStaffCategory.StaffCatgDescription + aSmallProcedure.NurseStaff.FullName;
                                    NurseStaffs.Add(TempNurseStaff);
                                }
                                if (aSmallProcedure.NurseStaff2 != null)
                                {
                                    Staff TempNurseStaff2 = ObjectCopier.DeepCopy(aSmallProcedure.NurseStaff2);
                                    TempNurseStaff2.FullName = aSmallProcedure.NurseStaff2.RefStaffCategory.StaffCatgDescription + aSmallProcedure.NurseStaff2.FullName;
                                    NurseStaffs.Add(TempNurseStaff2);
                                }
                                if (aSmallProcedure.NurseStaff3 != null)
                                {
                                    Staff TempNurseStaff3 = ObjectCopier.DeepCopy(aSmallProcedure.NurseStaff3);
                                    TempNurseStaff3.FullName = aSmallProcedure.NurseStaff3.RefStaffCategory.StaffCatgDescription + aSmallProcedure.NurseStaff3.FullName;
                                    NurseStaffs.Add(TempNurseStaff3);
                                }
                            }
                            //▼====== #014
                            mContentBody = mContentBody.Replace("[StaffName]", ProcedureStaffs.Count == 0 ? "" : "" + string.Join(", ", ProcedureStaffs));
                            mContentBody = mContentBody.Replace("[NarcoticStaffName]", NarcoticStaffs.Count == 0 ? "" : "" + string.Join(", ", NarcoticStaffs));
                            mContentBody = mContentBody.Replace("[NurseStaffName]", NurseStaffs.Count == 0 ? "" : "" + string.Join(",  ", NurseStaffs));
                            mContentBody = mContentBody.Replace("[Drainage]", aSmallProcedure == null ? "" : aSmallProcedure.Drainage);
                            mContentBody = mContentBody.Replace("[DateOffStitches]", (aSmallProcedure == null || aSmallProcedure.DateOffStitches == null) ? "" : string.Format(DateTimeStringFormat, aSmallProcedure.DateOffStitches));
                            mContentBody = mContentBody.Replace("[Notes]", aSmallProcedure == null ? "" : aSmallProcedure.Notes);
                            //▲====== #014
                            ThuThuatView.NavigateToString(mContentBody);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            aView.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            t.Start();
        }
        private bool _IsCriterion_PCLResult = false;
        public bool IsCriterion_PCLResult
        {
            get { return _IsCriterion_PCLResult; }
            set
            {
                if (_IsCriterion_PCLResult != value)
                {
                    _IsCriterion_PCLResult = value;
                    NotifyOfPropertyChange(() => IsCriterion_PCLResult);
                    if (UCPatientTreeView != null)
                        UCPatientTreeView.IsCriterion_PCLResult = IsCriterion_PCLResult;
                }
            }
        }
        private Uri _PDFViewerSource;
        public Uri PDFViewerSource
        {
            get { return _PDFViewerSource; }
            set
            {
                if (_PDFViewerSource != value)
                {
                    _PDFViewerSource = value;
                    NotifyOfPropertyChange(() => PDFViewerSource);
                }
            }
        }
        private string _TitleReport;
        public string TitleReport
        {
            get { return _TitleReport; }
            set
            {
                if (_TitleReport != value)
                {
                    _TitleReport = value;
                    NotifyOfPropertyChange(() => TitleReport);
                }
            }
        }
    }
}