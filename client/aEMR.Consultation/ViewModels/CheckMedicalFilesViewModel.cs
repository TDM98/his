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
using aEMR.Infrastructure.Events;
using aEMR.ReportModel.ReportModels;
using DevExpress.ReportServer.Printing;
using System.Data;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Data;
using aEMR.CommonTasks;
using aEMR.Common.HotKeyManagement;
using System.Windows.Input;

/*
 * 20220611 #001 DatTB: Thêm biến IsUnlocked cho phép user đang thao tác trả hồ sơ
 * 20220615 #002 DatTB: Thêm điều kiện thao tác DLS duyệt/trả hồ sơ
 * 20220711 #003 DatTB: Đổi màu trạng thái khi gửi lần 2.
 * 20220730 #004 DatTB: Đổi vị trí 2 else if (Hồ sơ gửi lần 2 vẫn đổi màu khi DLS xử lí)
 * 20221029 #005 QTD:   Thêm quyền cho nút
 */
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(ICheckMedicalFiles)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CheckMedicalFilesViewModel : ViewModelBase, ICheckMedicalFiles
    {
        [ImportingConstructor]
        public CheckMedicalFilesViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            HasInputBindingCmd = true;
            LoadDepartment();
            LoadDoctorStaffCollection();
            curObjPatientRegistration = new ObservableCollection<PatientRegistration>();
            ObjPatientPCLRequestDetails = new ObservableCollection<PatientPCLRequestDetail>();
            ObjPatientRegistrationDetails = new ObservableCollection<PatientRegistrationDetail>();
            ObjPrescriptionDetails = new ObservableCollection<PrescriptionDetail>();
            UCCheckMedicalFileHistory = Globals.GetViewModel<ICheckMedicalFileHistory>();
            UCCheckMedicalFileHistory.V_RegistrationType = (long)AllLookupValues.RegistrationType.NOI_TRU;
            SearchCriteria = new CheckMedicalFilesSearchCriteria { FromDate = Globals.GetCurServerDateTime() ,ToDate = Globals.GetCurServerDateTime(), NotDischarge = false };
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            ActivateItem(UCCheckMedicalFileHistory);
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

        private string _DiagnosisFinal = "";
        public string DiagnosisFinal
        {
            get
            {
                return _DiagnosisFinal;
            }
            set
            {
                if (_DiagnosisFinal != value)
                {
                    _DiagnosisFinal = value;
                    NotifyOfPropertyChange(() => DiagnosisFinal);
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
        private ObservableCollection<RefDepartment> _AllDepartment;
        public ObservableCollection<RefDepartment> AllDepartment
        {
            get
            {
                return _AllDepartment;
            }
            set
            {
                if (_AllDepartment != value)
                {
                    _AllDepartment = value;
                    NotifyOfPropertyChange(() => AllDepartment);
                }
            }
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
        private RefDepartment _SelectedDepartments;
        public RefDepartment SelectedDepartments
        {
            get
            {
                return _SelectedDepartments;
            }
            set
            {
                _SelectedDepartments = value;
                NotifyOfPropertyChange(() => SelectedDepartments);
            }
        }

        private DateTime _SelectedDateTime = Globals.GetCurServerDateTime();
        public DateTime SelectedDateTime
        {
            get
            {
                return _SelectedDateTime;
            }
            set
            {
                if (_SelectedDateTime != value)
                {
                    _SelectedDateTime = value;
                    NotifyOfPropertyChange(() => SelectedDateTime);
                }
            }
        }
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
        private RemoteDocumentSource _reportModel_TDT;
        public RemoteDocumentSource ReportModel_TDT
        {
            get { return _reportModel_TDT; }
            set
            {
                _reportModel_TDT = value;
                NotifyOfPropertyChange(() => ReportModel_TDT);
            }
        }
        private bool _ViewByDate = false;
        public bool ViewByDate
        {
            get { return _ViewByDate; }
            set
            {
                _ViewByDate = value;
                NotifyOfPropertyChange(() => ViewByDate);
            }
        }
        private bool _btCreateNewIsEnabled;
        public bool btCreateNewIsEnabled
        {
            //get { return IsEnableButton && IsNotExistsDiagnosisTreatmentByPtRegDetailID; }
            get { return _btCreateNewIsEnabled; }
            set
            {
                _btCreateNewIsEnabled = value;
                NotifyOfPropertyChange(() => btCreateNewIsEnabled);
            }
        }
        private bool _btCreateNewForDLSIsEnabled;
        public bool btCreateNewForDLSIsEnabled
        {
            get { return _btCreateNewForDLSIsEnabled; }
            set
            {
                _btCreateNewForDLSIsEnabled = value;
                NotifyOfPropertyChange(() => btCreateNewForDLSIsEnabled);
            }
        }
        private bool _btSaveIsEnabled;
        public bool btSaveIsEnabled
        {
            //get { return IsEnableButton && IsNotExistsDiagnosisTreatmentByPtRegDetailID; }
            get { return _btSaveIsEnabled; }
            set
            {
                _btSaveIsEnabled = value;
                NotifyOfPropertyChange(() => btSaveIsEnabled);
            }
        }
        private bool _btReturnIsEnabled;
        public bool btReturnIsEnabled
        {
            //get { return IsEnableButton && IsNotExistsDiagnosisTreatmentByPtRegDetailID; }
            get { return _btReturnIsEnabled; }
            set
            {
                _btReturnIsEnabled = value;
                NotifyOfPropertyChange(() => btReturnIsEnabled);
            }
        }

        private bool _btCheckIsEnabled;
        public bool btCheckIsEnabled
        {
            //get { return IsEnableButton && IsNotExistsDiagnosisTreatmentByPtRegDetailID; }
            get { return _btCheckIsEnabled; }
            set
            {
                _btCheckIsEnabled = value;
                NotifyOfPropertyChange(() => btCheckIsEnabled);
            }
        }

        private bool _DLSIsEnabled = false;
        public bool DLSIsEnabled
        {
            get
            {
                return _DLSIsEnabled;
            }
            set
            {
                _DLSIsEnabled = value;
                NotifyOfPropertyChange(() => DLSIsEnabled);
            }
        }

        private bool _IsNew = true;
        public bool IsNew
        {
            get
            {
                return _IsNew;
            }
            set
            {
                _IsNew = value;
                NotifyOfPropertyChange(() => IsNew);
            }
        }

        private bool _btUnLockIsEnabled = (bool)Globals.IsUserAdmin;
        public bool btUnLockIsEnabled
        {
            get { return _btUnLockIsEnabled; }
            set
            {
                _btUnLockIsEnabled = value;
                NotifyOfPropertyChange(() => btUnLockIsEnabled);
            }
        }

        private bool _EditorIsEnabled;
        public bool EditorIsEnabled
        {
            get { return _EditorIsEnabled; }
            set
            {
                _EditorIsEnabled = value;
                NotifyOfPropertyChange(() => EditorIsEnabled);
            }
        }
        private DataSet _PreviewHIReportSet;
        private DataTable _DataTable1;
        private DataTable _DataTable2;
        private DataTable _DataTable3;
        private DataTable _DataTable4;
        private DataTable _DataTable5;
        private string _ErrText;
        public DataSet PreviewHIReportSet
        {
            get => _PreviewHIReportSet; set
            {
                _PreviewHIReportSet = value;
                NotifyOfPropertyChange(() => PreviewHIReportSet);
            }
        }
        public DataTable DataTable1
        {
            get => _DataTable1; set
            {
                _DataTable1 = value;
                NotifyOfPropertyChange(() => DataTable1);
            }
        }
        public DataTable DataTable2
        {
            get => _DataTable2; set
            {
                _DataTable2 = value;
                NotifyOfPropertyChange(() => DataTable2);
            }
        }
        public DataTable DataTable3
        {
            get => _DataTable3; set
            {
                _DataTable3 = value;
                NotifyOfPropertyChange(() => DataTable3);
            }
        }
        public DataTable DataTable4
        {
            get => _DataTable4; set
            {
                _DataTable4 = value;
                NotifyOfPropertyChange(() => DataTable4);
            }
        }
        public DataTable DataTable5
        {
            get => _DataTable5; set
            {
                _DataTable5 = value;
                NotifyOfPropertyChange(() => DataTable5);
            }
        }
        public string ErrText
        {
            get => _ErrText; set
            {
                _ErrText = value;
                NotifyOfPropertyChange(() => ErrText);
            }
        }


        private ObservableCollection<Lookup> _V_CheckMedicalFilesStatus;

        public ObservableCollection<Lookup> V_CheckMedicalFilesStatus
        {
            get { return _V_CheckMedicalFilesStatus; }
            set
            {
                _V_CheckMedicalFilesStatus = value;
                NotifyOfPropertyChange(() => V_CheckMedicalFilesStatus);
            }
        }
        private long _SelectedV_CheckMedicalFilesStatus;

        public long SelectedV_CheckMedicalFilesStatus
        {
            get { return _SelectedV_CheckMedicalFilesStatus; }
            set
            {
                _SelectedV_CheckMedicalFilesStatus = value;
                NotifyOfPropertyChange(() => SelectedV_CheckMedicalFilesStatus);
            }
        }
        private CheckMedicalFilesSearchCriteria _SearchCriteria;
        public CheckMedicalFilesSearchCriteria SearchCriteria
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
        AxComboBox kscV_CheckMedicalFilesStatus = null;
        public void KscV_CheckMedicalFilesStatus_Loaded(object sender, RoutedEventArgs e)
        {
            kscV_CheckMedicalFilesStatus = (AxComboBox)sender;
            GetAllV_CheckMedicalFilesStatus();
        }
        public void cboV_CheckMedicalFilesStatusItemChanged(object sender, EventArgs e)
        {

        }
        public void GetAllV_CheckMedicalFilesStatus()
        {
            //▼===== Đổi lại vị trí lấy RefTreatment với set dữ liệu cho tmpkscSelectedItem vì cần phải set lại Source trước khi đọc dữ liệu để đưa vào 
            //       tmpkscSelectedItem vì nếu không lấy từ DiagTrmtItem sẽ bị xót khi bệnh nhân 2 là cấp toa cho về, mà bệnh nhân 1 đã nhập SearchKey là NV
            //       dẫn ItemsSource không có cấp toa nữa => Không hiển thị lại đc lastest V_TreatmentType của đăng ký => Đăng ký mà không cần Hướng điều trị.
            V_CheckMedicalFilesStatus = new ObservableCollection<Lookup>();
            foreach (var tmpLookup in Globals.AllLookupValueList)
            {
                if (tmpLookup.ObjectTypeID == (long)(LookupValues.V_CheckMedicalFilesStatus))
                {
                    V_CheckMedicalFilesStatus.Add(tmpLookup);
                }
            }
            V_CheckMedicalFilesStatus.Insert(0, new Lookup { LookupID = 0, ObjectValue = "Tất cả" });

            object tmpkscSelectedItem = null;
            if (V_CheckMedicalFilesStatus != null)
            {
                if (kscV_CheckMedicalFilesStatus != null && SelectedV_CheckMedicalFilesStatus > 0)
                {
                    foreach (var tmpRefTreatment in V_CheckMedicalFilesStatus)
                    {
                        if (tmpRefTreatment.LookupID == SelectedV_CheckMedicalFilesStatus)
                        {
                            tmpkscSelectedItem = tmpRefTreatment;
                        }
                    }
                }
                else
                {
                    kscV_CheckMedicalFilesStatus.ItemsSource = V_CheckMedicalFilesStatus;
                }
            }

            if (tmpkscSelectedItem != null)
            {
                kscV_CheckMedicalFilesStatus.ItemsSource = V_CheckMedicalFilesStatus;
                kscV_CheckMedicalFilesStatus.SelectedItem = tmpkscSelectedItem;
            }
        }

        private void LoadDepartment()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllDepartments(false, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetAllDepartments(asyncResult);
                                if (results != null)
                                {
                                    AllDepartment = new ObservableCollection<RefDepartment>();
                                    AllDepartment.Add(new RefDepartment { DeptID = 0, DeptName = "Tất cả" });
                                    foreach (var dept in results)
                                    {
                                        if (dept.V_DeptTypeOperation == (long)V_DeptTypeOperation.KhoaNoi)
                                        {
                                            AllDepartment.Add(dept);
                                        }
                                    }
                                    SelectedDepartments = AllDepartment.FirstOrDefault();
                                    NotifyOfPropertyChange(() => AllDepartment);
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

        private void LoadDoctorStaffCollection()
        {
            DoctorStaffs = new ObservableCollection<Staff>(Globals.AllStaffs.Where(x => x.RefStaffCategory != null 
                                                                                    && x.RefStaffCategory.V_StaffCatType == Globals.ServerConfigSection.CommonItems.StaffCatTypeBAC_SI 
                                                                                    && x.SCertificateNumber != null && x.SCertificateNumber.Length > 9
                                                                                    && (!x.IsStopUsing)).ToList());
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
        }

        public void btnFindRegistration()
        {
            SearchCriteria.StaffID = gSelectedDoctorStaff != null ? gSelectedDoctorStaff.StaffID : 0;
            SearchCriteria.DeptID = SelectedDepartments != null ? SelectedDepartments.DeptID : 0;
            SearchCriteria.V_CheckMedicalFilesStatus = SelectedV_CheckMedicalFilesStatus;
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
                        client.BeginSearchRegistrationListForCheckMedicalFiles(SearchCriteria
                            , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                curObjPatientRegistration = new ObservableCollection<PatientRegistration>();
                                List<PatientRegistration> allItems = client.EndSearchRegistrationListForCheckMedicalFiles(asyncResult);
                                if (allItems != null && allItems.Count > 0)
                                {
                                    foreach (var items in allItems)
                                    {
                                        curObjPatientRegistration.Add(items);
                                    }
                                    CountPatientRegistration = allItems.Count();
                                    FillPagedCollectionAndGroup();
                                }
                                else
                                {
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

            //var SelectedRow = (source as DataGrid).ItemContainerGenerator.ContainerFromItem((source as DataGrid).SelectedItem) as DataGridRow;
            //SelectedRow.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFA200"); // #003

            CurrentRegistration = eventArgs.Value as PatientRegistration;
            //PatientRegistration SelectedPatientRegistation = eventArgs.Value as PatientRegistration;
            if (CurrentRegistration != null && !string.IsNullOrEmpty(CurrentRegistration.Patient.FullName)
                //&& SelectedPatientRegistation.DiagnosisTreatment != null && !string.IsNullOrEmpty(SelectedPatientRegistation.DiagnosisTreatment.DiagnosisFinal) && !string.IsNullOrEmpty(SelectedPatientRegistation.DiagnosisTreatment.ICD10List)
                )
            {
                ObjCheckMedicalFiles = CurrentRegistration.CheckMedicalFiles;
                if (ObjCheckMedicalFiles != null && ObjCheckMedicalFiles.CheckMedicalFileID != null)
                {
                    StateEdit(ObjCheckMedicalFiles.V_CheckMedicalFilesStatus, ObjCheckMedicalFiles.DLSReject);
                }
                else
                {
                    ObjCheckMedicalFiles = new CheckMedicalFiles();
                    ObjCheckMedicalFiles.PtRegistrationID = CurrentRegistration.PtRegistrationID;
                    StateNew();
                }

                if(ObjCheckMedicalFiles.IsDLSChecked == false) btCheckIsEnabled = false;

                GetAllDiagnosisTreatment();
                GetPatientRegistrationDetails(CurrentRegistration.PtRegistrationID);
                GetPatientPCLReq(CurrentRegistration.PtRegistrationID);
                GetMau12();
                GetToDieuTri();
                GetHIReport(CurrentRegistration);
                UCCheckMedicalFileHistory.InitPatientInfo(CurrentRegistration);
            }
            else
            {
                MessageBox.Show(eHCMSResources.Z1688_G1_TTinKgHopLe);
            }
        }
        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            PatientRegistration item = e.Row.DataContext as PatientRegistration;
            if (item == null)
            {
                return;
            }
            if (item.V_CheckMedicalFilesStatus == 85602)
            {
                e.Row.Foreground = new SolidColorBrush(Colors.Red);
            }
            else if (item.V_CheckMedicalFilesStatus == 85603)
            {
                e.Row.Foreground = new SolidColorBrush(Colors.Blue);
            }
            else if (item.V_CheckMedicalFilesStatus == 85604)
            {
                e.Row.Foreground = new SolidColorBrush(Colors.YellowGreen);
            }
        }

        public void ApplyPreviewHIReportSet(DataSet aPreviewHIReportSet, string aErrText)
        {
            if (aPreviewHIReportSet == null || aPreviewHIReportSet.Tables == null || aPreviewHIReportSet.Tables.Count != 3)
            {
                if (DataTable1 != null)
                {
                    DataTable1.Rows.Clear();
                    DataTable2.Rows.Clear();
                    DataTable3.Rows.Clear();
                    DataTable4.Rows.Clear();
                    DataTable5.Rows.Clear();
                }
            }
            DataTable1 = aPreviewHIReportSet.Tables[0];
            DataTable2 = aPreviewHIReportSet.Tables[1];
            DataTable3 = aPreviewHIReportSet.Tables[2];
            DataTable4 = aPreviewHIReportSet.Tables[3];
            DataTable5 = aPreviewHIReportSet.Tables[4];
            ErrText = aErrText;
        }

        public void GetHIReport(object source)
        {
            if (source == null || !(source is PatientRegistration))
            {
                return;
            }
            PatientRegistration mPatientRegistration = source as PatientRegistration;
            if (mPatientRegistration == null)
            {
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new TransactionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPreviewHIReport_ForKHTH(mPatientRegistration.PtRegistrationID, (long)mPatientRegistration.V_RegistrationType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            DataSet mPreviewHIReportSet = contract.EndPreviewHIReport_ForKHTH(out string ErrText, asyncResult);
                            ApplyPreviewHIReportSet(mPreviewHIReportSet, ErrText);
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
            });

            t.Start();
        }

        private void GetMau12()
        {
            DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer theParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
            ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_Temp12_TongHop").PreviewModel;
            theParams["parHospitalAdress"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalAddress;
            theParams["RegistrationType"].Value = (long)CurrentRegistration.V_RegistrationType;
            theParams["PtRegistrationID"].Value = (int)CurrentRegistration.PtRegistrationID;
            theParams["FromDate"].Value = Globals.GetCurServerDateTime();
            theParams["ToDate"].Value = Globals.GetCurServerDateTime();
            theParams["ViewByDate"].Value = ViewByDate;
            theParams["StaffName"].Value = Globals.LoggedUserAccount != null && Globals.LoggedUserAccount.Staff != null ? Globals.LoggedUserAccount.Staff.FullName : "";
            theParams["parHospitalName"].Value = Globals.FirstCharToUpper(Globals.ServerConfigSection.CommonItems.ReportHospitalName.ToLower());
            theParams["parDepartmentOfHealth"].Value = Globals.FirstCharToUpper(Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth.ToLower());
            theParams["IsKHTHView"].Value = true;
            if (CurrentRegistration.AdmissionInfo != null && CurrentRegistration.AdmissionInfo.IsTreatmentCOVID)
            {
                theParams["IsPatientCOVID"].Value = CurrentRegistration.AdmissionInfo.IsTreatmentCOVID;
            }
            if (SelectedDepartments != null && SelectedDepartments.DeptID > 0)
            {
                theParams["DeptID"].Value = (int)SelectedDepartments.DeptID;
                theParams["DeptName"].Value = SelectedDepartments.DeptName;
            }
            else
            {
                theParams["DeptID"].Value = 0;
                theParams["DeptName"].Value = "";
            }
            ReportModel.CreateDocument(theParams);
        }
        private void GetToDieuTri()
        {
            DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer theParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();

            ReportModel_TDT = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XRptDoctorInstruction_Summary").PreviewModel;
            theParams["PtRegistrationID"].Value = (Int64)CurrentRegistration.PtRegistrationID;
            theParams["FromDate"].Value = (DateTime)CurrentRegistration.AdmissionDate;
            if (SearchCriteria.NotDischarge)
            {
                theParams["ToDate"].Value = Globals.GetCurServerDateTime();
            }
            else
            {
                theParams["ToDate"].Value = (DateTime)CurrentRegistration.DischargeDate;
            }
            ReportModel_TDT.CreateDocument(theParams);
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
                                Weight = result != null ? result.Weight + " (kg)" : "";
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

        public void GetPatientPCLReq(long PtRegistrationID)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetPatientPCLReq(PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                ObjPatientPCLRequestDetails = new ObservableCollection<PatientPCLRequestDetail>();
                                var tmpObjPatientPCLRequestDetails = new List<PatientPCLRequestDetail>();
                                var bPCLOK = false;
                                bool bOK = client.EndGetPatientPCLReq(out tmpObjPatientPCLRequestDetails, out bPCLOK, asyncResult);
                                if (bOK)
                                {
                                    ObjPatientPCLRequestDetails = tmpObjPatientPCLRequestDetails.ToObservableCollection();
                                }
                                else
                                {
                                    if (bPCLOK)
                                    {
                                        ObjPatientPCLRequestDetails = tmpObjPatientPCLRequestDetails.ToObservableCollection();
                                    }
                                }
                                FillPagedCollectionAndGroupPCLRequest();
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

        //public void GetMedicalFilesByPtRegistrationID(long PtRegistrationID, long V_RegistrationType)
        //{
        //    this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
        //    var t = new Thread(() =>
        //    {
        //        try
        //        {
        //            using (var serviceFactory = new PatientRegistrationServiceClient())
        //            {
        //                var client = serviceFactory.ServiceInstance;
        //                client.BeginGetMedicalFilesByPtRegistrationID(PtRegistrationID, V_RegistrationType, Globals.DispatchCallback((asyncResult) =>
        //                {
        //                    try
        //                    {
        //                        ObjCheckMedicalFiles = new CheckMedicalFiles();

        //                        var result = client.EndGetMedicalFilesByPtRegistrationID(asyncResult);
        //                        if (result != null)
        //                        {
        //                            ObjCheckMedicalFiles = result;
        //                            IsNew = false;
        //                            StateEdit(ObjCheckMedicalFiles.V_CheckMedicalFilesStatus);
        //                        }
        //                        else
        //                        {
        //                            ObjCheckMedicalFiles = new CheckMedicalFiles();
        //                            ObjCheckMedicalFiles.PtRegistrationID = PtRegistrationID;
        //                            StateNew();
        //                        }
        //                        StateDLS();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        this.HideBusyIndicator();
        //                        MessageBox.Show(ex.Message);
        //                    }
        //                    finally
        //                    {
        //                        this.HideBusyIndicator();
        //                    }
        //                }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show(ex.Message);
        //            this.HideBusyIndicator();
        //        }
        //    });

        //    t.Start();
        //}

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
                        client.BeginSaveMedicalFiles(CheckMedicalFile, Is_KHTH, (long)CurrentRegistration.V_RegistrationType
                            , Globals.LoggedUserAccount.StaffID ?? 0, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var bOK = client.EndSaveMedicalFiles(out long CheckMedicalFileIDNew, asyncResult);
                                if (bOK)
                                {
                                    MessageBox.Show("Cập nhật thành công");
                                    ObjCheckMedicalFiles.CheckMedicalFileID = CheckMedicalFileIDNew;
                                    StateEdit(ObjCheckMedicalFiles.V_CheckMedicalFilesStatus, ObjCheckMedicalFiles.DLSReject);
                                }
                                else
                                {
                                    StateNew();
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

        private PatientRegistration _currentRegistration;
        public PatientRegistration CurrentRegistration
        {
            get { return _currentRegistration; }
            set
            {
                if (_currentRegistration != value)
                {
                    _currentRegistration = value;
                    NotifyOfPropertyChange(() => CurrentRegistration);
                }
            }
        }

        IScanImageCapture theScanImageCaptureDlg = null;
        private List<ScanImageFileStorageDetail> NewScanImageFilesToBeSave = null;
        private List<ScanImageFileStorageDetail> ScanImageFilesToDeleted = new List<ScanImageFileStorageDetail>();
        public void DoScanCmd()
        {
            //long PtRegistrationID = GetPtRegistrationID_ForScanningStuff();
            //if (PtRegistrationID == 0)
            //    return;
            if (CurrentRegistration == null || CurrentRegistration.Patient == null || CurrentRegistration.PtRegistrationID == 0
                || CurrentRegistration.Patient.PatientID == 0
                || string.IsNullOrEmpty(CurrentRegistration.Patient.PatientCode))
            {
                MessageBox.Show("Chưa chọn bênh nhân");
                return;
            }
            theScanImageCaptureDlg = Globals.GetViewModel<IScanImageCapture>();
            Action<IScanImageCapture> onInitDlg = delegate (IScanImageCapture vm)
            {
                vm.PatientID = (CurrentRegistration.Patient != null ? CurrentRegistration.Patient.PatientID : 0);
                vm.PatientCode = (CurrentRegistration.Patient != null ? CurrentRegistration.Patient.PatientCode : "");
                vm.PtRegistrationID = CurrentRegistration.PtRegistrationID;
            };
            GlobalsNAV.ShowDialog_V3(theScanImageCaptureDlg, onInitDlg, null);
            NewScanImageFilesToBeSave = theScanImageCaptureDlg.ScanImageFileToBeSaved;
            ScanImageFilesToDeleted = theScanImageCaptureDlg.ScanImageFileToBeDeleted;
        }
        public void btCreateNew()
        {
            if (CurrentRegistration == null || CurrentRegistration.Patient == null || CurrentRegistration.PtRegistrationID == 0
                   || CurrentRegistration.Patient.PatientID == 0
                   || string.IsNullOrEmpty(CurrentRegistration.Patient.PatientCode))
            {
                MessageBox.Show("Chưa chọn bênh nhân");
                return;
            }
            StateNewWaiting();
            ObjCheckMedicalFiles = new CheckMedicalFiles();
            ObjCheckMedicalFiles.PtRegistrationID = CurrentRegistration.PtRegistrationID;
        }
        public void btCreateNewForDLS()
        {
            if (CurrentRegistration == null || CurrentRegistration.Patient == null || CurrentRegistration.PtRegistrationID == 0
                   || CurrentRegistration.Patient.PatientID == 0
                   || string.IsNullOrEmpty(CurrentRegistration.Patient.PatientCode))
            {
                MessageBox.Show("Chưa chọn bênh nhân");
                return;
            }
            StateNewWaiting(true);
            ObjCheckMedicalFiles = new CheckMedicalFiles();
            ObjCheckMedicalFiles.PtRegistrationID = CurrentRegistration.PtRegistrationID;
        }

        public void btSave()
        {
            if (ObjCheckMedicalFiles == null && ObjCheckMedicalFiles.Check_KHTH == null)
            {
                return;
            }
            SaveMedicalFiles(ObjCheckMedicalFiles, true);
        }

        public void btSaveDiagnosis()
        {
            if (ObjCheckMedicalFiles == null)
            {
                return;
            }
            SaveMedicalFiles(ObjCheckMedicalFiles, false);
        }

        public void btCheckDLS()
        {
            if (ObjCheckMedicalFiles == null)
            {
                return;
            }
            ObjCheckMedicalFiles.IsDLSChecked = true;
            ObjCheckMedicalFiles.DLSReject = false;// #002
            SaveMedicalFiles(ObjCheckMedicalFiles, false);
            StateDLS();
        }
        public void BtnDLSReject()
        {
            if (ObjCheckMedicalFiles == null)
            {
                return;
            }
            ObjCheckMedicalFiles.IsDLSChecked = false;// #002
            ObjCheckMedicalFiles.DLSReject = true;
            SaveMedicalFiles(ObjCheckMedicalFiles, false);
            StateDLS();
        }

        public void btReturn()
        {
            if (ObjCheckMedicalFiles == null && ObjCheckMedicalFiles.Check_KHTH == null)
            {
                return;
            }
            ObjCheckMedicalFiles.V_CheckMedicalFilesStatus = 85602;
            SaveMedicalFiles(ObjCheckMedicalFiles, true);
        }

        public void btCheck()
        {
            if (ObjCheckMedicalFiles == null && ObjCheckMedicalFiles.Check_KHTH == null)
            {
                return;
            }
            ObjCheckMedicalFiles.V_CheckMedicalFilesStatus = 85603;
            SaveMedicalFiles(ObjCheckMedicalFiles, true);
        }

        public void StateDLS()
        {
            if (ObjCheckMedicalFiles == null)
            {
                return;
            }

            DLSIsEnabled = !ObjCheckMedicalFiles.IsDLSChecked;
        }

        public void StateNew()
        {
            btCreateNewIsEnabled = false;
            btCreateNewForDLSIsEnabled = false;
            btSaveIsEnabled = true;
            //▼==== #002
            btCheckIsEnabled = false;
            btReturnIsEnabled = false;
            btUnLockIsEnabled = false;
            //▲==== #002
            DLSIsEnabled = true;
            EditorIsEnabled = true;

            NotifyOfPropertyChange(() => btCreateNewIsEnabled);
            NotifyOfPropertyChange(() => btCreateNewForDLSIsEnabled);
            NotifyOfPropertyChange(() => btSaveIsEnabled);
            NotifyOfPropertyChange(() => btCheckIsEnabled);
            NotifyOfPropertyChange(() => btReturnIsEnabled);
            NotifyOfPropertyChange(() => btUnLockIsEnabled);//==== #002
            NotifyOfPropertyChange(() => DLSIsEnabled);
            NotifyOfPropertyChange(() => EditorIsEnabled);
        }

        public void StateNewWaiting(bool IsDLS = false)
        {
            if (IsDLS)
            {
                DLSIsEnabled = true;
                NotifyOfPropertyChange(() => DLSIsEnabled);
            }
            else
            {
                btCreateNewIsEnabled = false;
                btCreateNewForDLSIsEnabled = false;
                btSaveIsEnabled = true;
                //▼==== #002
                btCheckIsEnabled = false;
                btReturnIsEnabled = false;
                btUnLockIsEnabled = false;
                //▲==== #002
                EditorIsEnabled = true;

                NotifyOfPropertyChange(() => btCreateNewIsEnabled);
                NotifyOfPropertyChange(() => btCreateNewForDLSIsEnabled);
                NotifyOfPropertyChange(() => btSaveIsEnabled);
                NotifyOfPropertyChange(() => btCheckIsEnabled);
                NotifyOfPropertyChange(() => btReturnIsEnabled);
                NotifyOfPropertyChange(() => btUnLockIsEnabled);//==== #002
                NotifyOfPropertyChange(() => EditorIsEnabled);
            }           
        }

        public void StateEdit(long V_CheckMedicalFilesStatus = 0, bool DLSReject = false)
        {
            if (V_CheckMedicalFilesStatus == (long)AllLookupValues.V_CheckMedicalFilesStatus.Da_Duyet)
            {
                btCreateNewIsEnabled = false;
                btCreateNewForDLSIsEnabled = false;
                btSaveIsEnabled = false;
                btCheckIsEnabled = false;
                btReturnIsEnabled = false;
                DLSIsEnabled = false;
                EditorIsEnabled = false;
                btUnLockIsEnabled = true;//==== #002

                NotifyOfPropertyChange(() => btCreateNewIsEnabled);
                NotifyOfPropertyChange(() => btCreateNewForDLSIsEnabled);
                NotifyOfPropertyChange(() => btSaveIsEnabled);
                NotifyOfPropertyChange(() => btCheckIsEnabled);
                NotifyOfPropertyChange(() => btReturnIsEnabled);
                NotifyOfPropertyChange(() => DLSIsEnabled);
                NotifyOfPropertyChange(() => EditorIsEnabled);
                NotifyOfPropertyChange(() => btUnLockIsEnabled);//==== #002
            }
            else if (V_CheckMedicalFilesStatus == (long)AllLookupValues.V_CheckMedicalFilesStatus.Tra_Ho_So) //|| V_CheckMedicalFilesStatus == (long)AllLookupValues.V_CheckMedicalFilesStatus.Cho_Duyet_Lai)
            {
                btCreateNewIsEnabled = true;
                btCreateNewForDLSIsEnabled = true;
                btSaveIsEnabled = false;
                btCheckIsEnabled = false;
                btReturnIsEnabled = false;
                EditorIsEnabled = false;
                DLSIsEnabled = false;
                btUnLockIsEnabled = true;//==== #002

                NotifyOfPropertyChange(() => btCreateNewIsEnabled);
                NotifyOfPropertyChange(() => btCreateNewForDLSIsEnabled);
                NotifyOfPropertyChange(() => btSaveIsEnabled);
                NotifyOfPropertyChange(() => btCheckIsEnabled);
                NotifyOfPropertyChange(() => btReturnIsEnabled);
                NotifyOfPropertyChange(() => EditorIsEnabled);
                NotifyOfPropertyChange(() => DLSIsEnabled);
                NotifyOfPropertyChange(() => btUnLockIsEnabled);//==== #002
            }
            else
            {
                btCreateNewIsEnabled = false;
                btCreateNewForDLSIsEnabled = false;
                btSaveIsEnabled = true;
                btCheckIsEnabled = true && ObjCheckMedicalFiles.IsDLSChecked;
                btReturnIsEnabled = ObjCheckMedicalFiles.IsDLSChecked || ObjCheckMedicalFiles.DLSReject; //==== #002
                DLSIsEnabled = !ObjCheckMedicalFiles.IsDLSChecked && !ObjCheckMedicalFiles.DLSReject; //==== #002
                EditorIsEnabled = true;
                btUnLockIsEnabled = false;//==== #002

                NotifyOfPropertyChange(() => btCreateNewIsEnabled);
                NotifyOfPropertyChange(() => btCreateNewForDLSIsEnabled);
                NotifyOfPropertyChange(() => btSaveIsEnabled);
                NotifyOfPropertyChange(() => btCheckIsEnabled);
                NotifyOfPropertyChange(() => btReturnIsEnabled);
                NotifyOfPropertyChange(() => DLSIsEnabled);
                NotifyOfPropertyChange(() => EditorIsEnabled);
            }

            if (CurrentRegistration.IsDischarge)
            {
                btUnLockIsEnabled = false;
                NotifyOfPropertyChange(() => btUnLockIsEnabled);
            }
        }

        public void btnUnlock()
        {
            btReturnIsEnabled = true;
            CurrentRegistration.CheckMedicalFiles.IsUnlocked = true; // #001
            NotifyOfPropertyChange(() => btReturnIsEnabled);
        }
        #region List Registration Grid

        private void FillGroupName()
        {
            if (CV_CheckMedicalFilesLst != null && CV_CheckMedicalFilesLst.Count > 0)
            {
                CV_CheckMedicalFilesLst.GroupDescriptions.Clear();
                CV_CheckMedicalFilesLst.SortDescriptions.Clear();
                CV_CheckMedicalFilesLst.GroupDescriptions.Add(new PropertyGroupDescription("InDeptLocation"));
                CV_CheckMedicalFilesLst.SortDescriptions.Add(new SortDescription("Patient.PatientCode", ListSortDirection.Ascending));
                CV_CheckMedicalFilesLst.Filter = null;
            }
        }

        private CollectionViewSource _cvs_CheckMedicalFilesLst = null;
        public CollectionViewSource CVS_CheckMedicalFilesLst
        {
            get
            {
                return _cvs_CheckMedicalFilesLst;
            }
            set
            {
                _cvs_CheckMedicalFilesLst = value;
            }
        }

        public CollectionView CV_CheckMedicalFilesLst { get; set; }
        private void FillPagedCollectionAndGroup()
        {
            if (curObjPatientRegistration != null && curObjPatientRegistration.Count > 0)
            {
                CVS_CheckMedicalFilesLst = new CollectionViewSource { Source = curObjPatientRegistration };
                CV_CheckMedicalFilesLst = (CollectionView)CVS_CheckMedicalFilesLst.View;
                FillGroupName();
                NotifyOfPropertyChange(() => CV_CheckMedicalFilesLst);
            }
        }
        public void grdReqOutwardDetails_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
            PatientRegistration item = e.Row.DataContext as PatientRegistration;
            if (item == null)
            {
                return;
            }
            if (item.V_CheckMedicalFilesStatus == (long)AllLookupValues.V_CheckMedicalFilesStatus.Tra_Ho_So)
            {
                e.Row.Background = new SolidColorBrush(Colors.Red);
            }
            else if (item.V_CheckMedicalFilesStatus == (long)AllLookupValues.V_CheckMedicalFilesStatus.Da_Duyet)
            {
                //▼==== #003
                e.Row.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#0D69FF");
                e.Row.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#0D69FF");
                e.Row.Foreground = new SolidColorBrush(Colors.White);
                //▲==== #003
            }
            //▼==== #004
            else if (item.CheckMedicalFiles != null && (item.CheckMedicalFiles.IsDLSChecked || item.CheckMedicalFiles.DLSReject))
            {
                e.Row.Background = new SolidColorBrush(Colors.PaleGreen);
            }
            else if (item.V_CheckMedicalFilesStatus == (long)AllLookupValues.V_CheckMedicalFilesStatus.Cho_Duyet_Lai)
            {
                e.Row.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFA200"); // #003
            }
            //▲==== #004

        }
        #endregion

        #region new function
        private string _CurPatientClassification;
        public string CurPatientClassification
        {
            get { return _CurPatientClassification; }
            set
            {
                if (_CurPatientClassification != value)
                {
                    _CurPatientClassification = value;
                    NotifyOfPropertyChange(() => CurPatientClassification);
                }
            }
        }

        private string _ListICD10BeforeAdm;
        public string ListICD10BeforeAdm
        {
            get
            {
                return _ListICD10BeforeAdm;
            }
            set
            {
                if (_ListICD10BeforeAdm == value)
                {
                    return;
                }
                _ListICD10BeforeAdm = value;
                NotifyOfPropertyChange(() => ListICD10BeforeAdm);
            }
        }

        private string _DiagnosisTreatmentBeforeAdmOther;
        public string DiagnosisTreatmentBeforeAdmOther
        {
            get
            {
                return _DiagnosisTreatmentBeforeAdmOther;
            }
            set
            {
                if (_DiagnosisTreatmentBeforeAdmOther == value)
                {
                    return;
                }
                _DiagnosisTreatmentBeforeAdmOther = value;
                NotifyOfPropertyChange(() => DiagnosisTreatmentBeforeAdmOther);
            }
        }

        private List<DiagnosisTreatment> _DiagnosisTreatmentCollection;
        public List<DiagnosisTreatment> DiagnosisTreatmentCollection
        {
            get
            {
                return _DiagnosisTreatmentCollection;
            }
            set
            {
                if (_DiagnosisTreatmentCollection == value)
                {
                    return;
                }
                _DiagnosisTreatmentCollection = value;
                NotifyOfPropertyChange(() => DiagnosisTreatmentCollection);
            }
        }

        private ObservableCollection<PatientRegistrationDetail> _ObjPatientRegistrationDetails;
        public ObservableCollection<PatientRegistrationDetail> ObjPatientRegistrationDetails
        {
            get
            {
                return _ObjPatientRegistrationDetails;
            }
            set
            {
                if (_ObjPatientRegistrationDetails != value)
                {
                    _ObjPatientRegistrationDetails = value;
                    NotifyOfPropertyChange(() => ObjPatientRegistrationDetails);
                }
            }
        }

        private string _DeptNameInfo;
        public string DeptNameInfo
        {
            get
            {
                return _DeptNameInfo;
            }
            set
            {
                if (_DeptNameInfo != value)
                {
                    _DeptNameInfo = value;
                    NotifyOfPropertyChange(() => DeptNameInfo);
                }
            }
        }

        public void GetListDiagnosisTreatment(long DTItemID, bool IsBeforeAdm = false)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
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
                            ObservableCollection<DiagnosisIcd10Items> refIDC10List = results.ToObservableCollection();
                            if (refIDC10List == null)
                            {
                                refIDC10List = new ObservableCollection<DiagnosisIcd10Items>();
                            }
                            else
                            {
                                if (IsBeforeAdm)
                                {
                                    ListICD10BeforeAdm = "";
                                    foreach (var temp in refIDC10List)
                                    {
                                        ListICD10BeforeAdm += temp.ICD10Code + " - " + temp.DiseasesReference.DiseaseNameVN + "\n";
                                    }
                                    DeptNameInfo = CurrentRegistration.AdmissionInfo.Department.DeptName + " (" + CurrentRegistration.AdmissionInfo.AdmissionDate.Value.ToString("HH:mm dd/MM/yyyy") + ")";
                                }
                                else
                                {
                                    if (CurrentRegistration.AdmissionInfo.DiagnosisTreatmentInfo == null)
                                    {
                                        CurrentRegistration.AdmissionInfo.DiagnosisTreatmentInfo = new DiagnosisTreatment();
                                    }
                                    CurrentRegistration.AdmissionInfo.DiagnosisTreatmentInfo.ICD10List = refIDC10List.Where(x => x.IsMain).FirstOrDefault().ICD10Code + " - " + refIDC10List.Where(x => x.IsMain).FirstOrDefault().DiseasesReference.DiseaseNameVN;
                                    CurrentRegistration.AdmissionInfo.DiagnosisTreatmentInfo.ICD10SubList = "";
                                    foreach (var temp in refIDC10List)
                                    {
                                        if (!temp.IsMain)
                                        {
                                            CurrentRegistration.AdmissionInfo.DiagnosisTreatmentInfo.ICD10SubList += temp.ICD10Code + " - " + temp.DiseasesReference.DiseaseNameVN + "\n";
                                        }
                                    }
                                    DiagnosisFinal = string.Join("; ", refIDC10List.Select(x => x.DiseasesReference.DiseaseNameVN)) + CurrentRegistration.DiagnosisTreatment.DiagnosisOther;
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
            });

            t.Start();
        }

        public void GetAllDiagnosisTreatment()
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var CurrentThread = new Thread(() =>
            {
                using (var mFactory = new ePMRsServiceClient())
                {
                    var mContract = mFactory.ServiceInstance;
                    mContract.BeginGetDiagnosisTreatment_InPt_ByPtRegID(CurrentRegistration.PtRegistrationID, null, null
                        , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var ItemCollection = mContract.EndGetDiagnosisTreatment_InPt_ByPtRegID(asyncResult);
                                if (ItemCollection == null)
                                {
                                    DiagnosisTreatmentCollection = new List<DiagnosisTreatment>();
                                }
                                else
                                {
                                    DiagnosisTreatmentCollection = ItemCollection.ToList();
                                    if (CurrentRegistration.DiagnosisTreatment != null)
                                    {
                                        GetListDiagnosisTreatment(CurrentRegistration.DiagnosisTreatment.DTItemID);
                                    }
                                    GetListDiagnosisTreatment(DiagnosisTreatmentCollection.Where(x => x.IsAdmission).FirstOrDefault().DTItemID, true);
                                    DiagnosisTreatmentBeforeAdmOther = DiagnosisTreatmentCollection.Where(x => x.IsAdmission).FirstOrDefault().DiagnosisOther;
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

        public void GetPatientRegistrationDetails(long PtRegistrationID)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetPatientRegistrationDetails(PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                ObjPatientRegistrationDetails = new ObservableCollection<PatientRegistrationDetail>();
                                var tmpObjPatientRegistrationDetails = new List<PatientRegistrationDetail>();
                                var bPCLOK = false;
                                bool bOK = client.EndGetPatientRegistrationDetails(out tmpObjPatientRegistrationDetails, out bPCLOK, asyncResult);
                                if (bOK)
                                {
                                    ObjPatientRegistrationDetails = tmpObjPatientRegistrationDetails.ToObservableCollection();
                                    FillRegistrationDetailPagedCollectionAndGroup();
                                }
                                else
                                {
                                    if (bPCLOK)
                                    {
                                        ObjPatientRegistrationDetails = tmpObjPatientRegistrationDetails.ToObservableCollection();
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
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        public void grdPatientRegistrationDetail_DblClick(object sender, EventArgs<object> e)
        {
            if ((sender as DataGrid).SelectedItem != null)
            {
                PatientRegistrationDetail RegDetail = (sender as DataGrid).SelectedItem as PatientRegistrationDetail;
                if (RegDetail.RefMedicalServiceItem.ObjV_Surgery_Tips_Item != null && RegDetail.RefMedicalServiceItem.ObjV_Surgery_Tips_Item.LookupID > 0)
                {
                    this.ShowBusyIndicator();
                    var t = new Thread(() =>
                    {
                        using (var serviceFactory = new CommonUtilsServiceClient())
                        {
                            var contract = serviceFactory.ServiceInstance;
                            contract.BeginGetSmallProcedure(RegDetail.PtRegDetailID, 0, (long)CurrentRegistration.V_RegistrationType
                                , Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var mSmallProcedure = contract.EndGetSmallProcedure(asyncResult);
                                    if (mSmallProcedure == null)
                                    {
                                        return;
                                    }
                                    else
                                    {
                                        CommonGlobals.PrintProcedureProcess(this, mSmallProcedure, CurrentRegistration);
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
            }
        }
        #endregion

        #region List PCLRequest Grid
        private void FillPCLRequestGroupName()
        {
            if (CV_PatientPCLRequestDetails != null && CV_PatientPCLRequestDetails.Count > 0)
            {
                CV_PatientPCLRequestDetails.GroupDescriptions.Clear();
                CV_PatientPCLRequestDetails.SortDescriptions.Clear();
                CV_PatientPCLRequestDetails.GroupDescriptions.Add(new PropertyGroupDescription("MedicalInstructionDateStr"));
                CV_PatientPCLRequestDetails.SortDescriptions.Add(new SortDescription("MedicalInstructionDate", ListSortDirection.Ascending));
                CV_PatientPCLRequestDetails.Filter = null;
            }
        }

        private CollectionViewSource _cvs_PatientPCLRequestDetails = null;
        public CollectionViewSource CVS_PatientPCLRequestDetails
        {
            get
            {
                return _cvs_PatientPCLRequestDetails;
            }
            set
            {
                _cvs_PatientPCLRequestDetails = value;
            }
        }

        public CollectionView CV_PatientPCLRequestDetails { get; set; }
        private void FillPagedCollectionAndGroupPCLRequest()
        {
            if (ObjPatientPCLRequestDetails != null && ObjPatientPCLRequestDetails.Count > 0)
            {
                CVS_PatientPCLRequestDetails = new CollectionViewSource { Source = ObjPatientPCLRequestDetails };
                CV_PatientPCLRequestDetails = (CollectionView)CVS_PatientPCLRequestDetails.View;
                FillPCLRequestGroupName();
                NotifyOfPropertyChange(() => CV_PatientPCLRequestDetails);
            }
        }

        public void grdPCLRequestDetails_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
            PatientPCLRequestDetail item = e.Row.DataContext as PatientPCLRequestDetail;
            if (item.ResultDate < item.MedicalInstructionDate)
            {
                e.Row.Foreground = new SolidColorBrush(Colors.OrangeRed);
            }
        }
        #endregion

        #region HotKey Define
        public override void HandleHotKey_Action_New(object sender, LocalHotKeyEventArgs e)
        {
            foreach (var inputBindingCommand in ListInputBindingCmds)
            {
                if (inputBindingCommand.HotKey_Registered_Name == e.HotKey.Name)
                {
                    inputBindingCommand._executeDelegate.Invoke(this);
                    break;
                }
            }
        }

        protected override IEnumerable<InputBindingCommand> GetInputBindingCommands()
        {
            yield return new InputBindingCommand(() => BtnRefresh())
            {
                HotKey_Registered_Name = "ghkRefresh",
                GestureModifier = ModifierKeys.None,
                GestureKey = (Key)Keys.F5
            };
        }

        private void BtnRefresh()
        {
            btnFindRegistration();
        }
        #endregion

        #region List RegistrationDetails Grid

        private void FillRegistrationDetailGroupName()
        {
            if (CV_PatientRegistrationDetails != null && CV_PatientRegistrationDetails.Count > 0)
            {
                CV_PatientRegistrationDetails.GroupDescriptions.Clear();
                CV_PatientRegistrationDetails.SortDescriptions.Clear();
                //CV_PatientRegistrationDetails.GroupDescriptions.Add(new PropertyGroupDescription("MedicalInstructionDateStr"));
                CV_PatientRegistrationDetails.GroupDescriptions.Add(new PropertyGroupDescription("RefMedicalServiceItem.ObjHITTypeID.IdxTemp12Name"));
                CV_PatientRegistrationDetails.SortDescriptions.Add(new SortDescription("MedicalInstructionDate", ListSortDirection.Ascending));
                CV_PatientRegistrationDetails.SortDescriptions.Add(new SortDescription("RefMedicalServiceItem.ObjHITTypeID.HITTypeID", ListSortDirection.Ascending));
                CV_PatientRegistrationDetails.Filter = null;
            }
        }

        private CollectionViewSource _CVS_PatientRegistrationDetails = null;
        public CollectionViewSource CVS_PatientRegistrationDetails
        {
            get
            {
                return _CVS_PatientRegistrationDetails;
            }
            set
            {
                _CVS_PatientRegistrationDetails = value;
            }
        }

        public CollectionView CV_PatientRegistrationDetails { get; set; }
        private void FillRegistrationDetailPagedCollectionAndGroup()
        {
            if (curObjPatientRegistration != null && curObjPatientRegistration.Count > 0)
            {
                CVS_PatientRegistrationDetails = new CollectionViewSource { Source = ObjPatientRegistrationDetails };
                CV_PatientRegistrationDetails = (CollectionView)CVS_PatientRegistrationDetails.View;
                FillRegistrationDetailGroupName();
                NotifyOfPropertyChange(() => CV_PatientRegistrationDetails);
            }
        }
        #endregion
        public ICheckMedicalFileHistory UCCheckMedicalFileHistory { get; set; }
        public void ICDListCmd()
        {
            Action<IICD_ListFindForConsultation> onInitDlg = delegate (IICD_ListFindForConsultation listFind)
            {
                this.ActivateItem(listFind);
            };
            GlobalsNAV.ShowDialog<IICD_ListFindForConsultation>(onInitDlg);
        }
        //▼====: #005
        private bool _mCheckMedicalFile_Tim = true;
        private bool _mCheckMedicalFile_DanhMucICD = true;
        private bool _mCheckMedicalFile_Luu = true;
        private bool _mCheckMedicalFile_TraHS = true;
        private bool _mCheckMedicalFile_Duyet = true;
        private bool _mCheckMedicalFile_MoKhoa = true;
        private bool _mCheckMedicalFile_DLS_Save = true;
        private bool _mCheckMedicalFile_DLS_Duyet = true;
        private bool _mCheckMedicalFile_DLS_TraHS = true;
        public bool mCheckMedicalFile_Tim
        {
            get
            {
                return _mCheckMedicalFile_Tim;
            }
            set
            {
                if (_mCheckMedicalFile_Tim == value)
                    return;
                _mCheckMedicalFile_Tim = value;
                NotifyOfPropertyChange(() => mCheckMedicalFile_Tim);
            }
        }
        public bool mCheckMedicalFile_DanhMucICD
        {
            get
            {
                return _mCheckMedicalFile_DanhMucICD;
            }
            set
            {
                if (_mCheckMedicalFile_DanhMucICD == value)
                    return;
                _mCheckMedicalFile_DanhMucICD = value;
                NotifyOfPropertyChange(() => mCheckMedicalFile_DanhMucICD);
            }
        }
        public bool mCheckMedicalFile_Luu
        {
            get
            {
                return _mCheckMedicalFile_Luu;
            }
            set
            {
                if (_mCheckMedicalFile_Luu == value)
                    return;
                _mCheckMedicalFile_Luu = value;
                NotifyOfPropertyChange(() => mCheckMedicalFile_Luu);
            }
        }
        public bool mCheckMedicalFile_TraHS
        {
            get
            {
                return _mCheckMedicalFile_TraHS;
            }
            set
            {
                if (_mCheckMedicalFile_TraHS == value)
                    return;
                _mCheckMedicalFile_TraHS = value;
                NotifyOfPropertyChange(() => mCheckMedicalFile_TraHS);
            }
        }
        public bool mCheckMedicalFile_Duyet
        {
            get
            {
                return _mCheckMedicalFile_Duyet;
            }
            set
            {
                if (_mCheckMedicalFile_Duyet == value)
                    return;
                _mCheckMedicalFile_Duyet = value;
                NotifyOfPropertyChange(() => mCheckMedicalFile_Duyet);
            }
        }
        public bool mCheckMedicalFile_MoKhoa
        {
            get
            {
                return _mCheckMedicalFile_MoKhoa;
            }
            set
            {
                if (_mCheckMedicalFile_MoKhoa == value)
                    return;
                _mCheckMedicalFile_MoKhoa = value;
                NotifyOfPropertyChange(() => mCheckMedicalFile_MoKhoa);
            }
        }
        public bool mCheckMedicalFile_DLS_Save
        {
            get
            {
                return _mCheckMedicalFile_DLS_Save;
            }
            set
            {
                if (_mCheckMedicalFile_DLS_Save == value)
                    return;
                _mCheckMedicalFile_DLS_Save = value;
                NotifyOfPropertyChange(() => mCheckMedicalFile_DLS_Save);
            }
        }
        public bool mCheckMedicalFile_DLS_Duyet
        {
            get
            {
                return _mCheckMedicalFile_DLS_Duyet;
            }
            set
            {
                if (_mCheckMedicalFile_DLS_Duyet == value)
                    return;
                _mCheckMedicalFile_DLS_Duyet = value;
                NotifyOfPropertyChange(() => mCheckMedicalFile_DLS_Duyet);
            }
        }
        public bool mCheckMedicalFile_DLS_TraHS
        {
            get
            {
                return _mCheckMedicalFile_DLS_TraHS;
            }
            set
            {
                if (_mCheckMedicalFile_DLS_TraHS == value)
                    return;
                _mCheckMedicalFile_DLS_TraHS = value;
                NotifyOfPropertyChange(() => mCheckMedicalFile_DLS_TraHS);
            }
        }
        //▲====: #005
    }
}
