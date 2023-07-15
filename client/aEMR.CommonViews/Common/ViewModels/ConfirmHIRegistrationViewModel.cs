using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.Common.ExportExcel;
using aEMR.Common.HotKeyManagement;
using aEMR.CommonTasks;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using DataEntities;
using eHCMS.CommonUserControls.CommonTasks;
using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
/*
 * 20190326 #001 TNHX: Add function to export excel + loaded TabControl
 * 20210610 #002 TNHX:  331: Thêm danh sách lịch sử DHST để sử dụng lại
 */
namespace aEMR.Common.ViewModels
{
    public class Registration_DataStorage : IRegistration_DataStorage
    {
        private PatientRegistration _CurrentPatientRegistration;
        private PatientRegistrationDetail _CurrentPatientRegistrationDetail;
        private ObservableCollection<PrescriptionIssueHistory> _PrescriptionIssueHistories;
        private ObservableCollection<PatientServiceRecord> _PatientServiceRecordCollection;
        private DiagnosisTreatment _CurrentDiagnosisTreatment;

        public PatientRegistration CurrentPatientRegistration
        {
            get
            {
                return _CurrentPatientRegistration;
            }
            set
            {
                _CurrentPatientRegistration = value;
            }
        }
        public PatientRegistrationDetail CurrentPatientRegistrationDetail
        {
            get
            {
                return _CurrentPatientRegistrationDetail;
            }
            set
            {
                _CurrentPatientRegistrationDetail = value;
            }
        }
        public Patient CurrentPatient
        {
            get
            {
                return CurrentPatientRegistration == null ? null : CurrentPatientRegistration.Patient;
            }
        }
        public ObservableCollection<PrescriptionIssueHistory> PrescriptionIssueHistories
        {
            get
            {
                return _PrescriptionIssueHistories;
            }
            set
            {
                _PrescriptionIssueHistories = value;
            }
        }
        public ObservableCollection<PatientServiceRecord> PatientServiceRecordCollection
        {
            get
            {
                return _PatientServiceRecordCollection;
            }
            set
            {
                _PatientServiceRecordCollection = value;
            }
        }
        public DiagnosisTreatment CurrentDiagnosisTreatment
        {
            get
            {
                return _CurrentDiagnosisTreatment;
            }
            set
            {
                _CurrentDiagnosisTreatment = value;
            }
        }

        public List<FilterPrescriptionsHasHIPay> ListFilterPrescriptionsHasHIPay { get; set; }
        public List<RequiredSubDiseasesReferences> ListRequiredSubDiseasesReferences { get; set; }
        //▼====: #002
        public ObservableCollection<PhysicalExamination> PtPhyExamList { get; set; }
        //▲====: #002
        //▼====: #003
        public List<RuleDiseasesReferences> ListRuleDiseasesReferences { get; set; }
        //▲====: #003
    }
    [Export(typeof(IConfirmHIRegistration)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ConfirmHIRegistrationViewModel : ViewModelBase, IConfirmHIRegistration
    {
        [ImportingConstructor]
        public ConfirmHIRegistrationViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            base.HasInputBindingCmd = true;
            Coroutine.BeginExecute(LoadDepartments());

            ConfirmHIView = Globals.GetViewModel<IPrescriptionAndConfirmHI>();
            ActivateItem(ConfirmHIView);
        }
        #region Properties
        private DateTime _CurrentDateTime = Globals.GetCurServerDateTime();
        public DateTime CurrentDateTime
        {
            get => _CurrentDateTime; set
            {
                _CurrentDateTime = value;
                NotifyOfPropertyChange(() => CurrentDateTime);
            }
        }
        private ObservableCollection<RefDepartment> _RefDepartmentCollection;
        private ObservableCollection<DeptLocation> _LocationCollection;
        private SeachPtRegistrationCriteria _SearchCriteria = new SeachPtRegistrationCriteria { FromDate = Globals.GetCurServerDateTime(), ToDate = Globals.GetCurServerDateTime() };
        private ObservableCollection<PatientRegistration> _PatientRegistrationCollection;
        private ObservableCollection<PatientRegistration> _OtherTypePatientRegistrationCollection;
        private int _DoMoveTabContent = 0;
        private double _gActualHeight = 1000;
        private double _gActualWidth = 1000;
        private IPrescriptionAndConfirmHI _ConfirmHIView;
        private bool _ConfirmHIViewVisible = false;
        private int _StatusIndex = 2;
        public ObservableCollection<RefDepartment> RefDepartmentCollection
        {
            get => _RefDepartmentCollection; set
            {
                _RefDepartmentCollection = value;
                NotifyOfPropertyChange(() => RefDepartmentCollection);
            }
        }
        public ObservableCollection<DeptLocation> LocationCollection
        {
            get => _LocationCollection; set
            {
                _LocationCollection = value;
                NotifyOfPropertyChange(() => LocationCollection);
            }
        }
        public SeachPtRegistrationCriteria SearchCriteria
        {
            get => _SearchCriteria; set
            {
                _SearchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }
        public ObservableCollection<PatientRegistration> PatientRegistrationCollection
        {
            get => _PatientRegistrationCollection; set
            {
                _PatientRegistrationCollection = value;
                NotifyOfPropertyChange(() => PatientRegistrationCollection);
            }
        }
        public ObservableCollection<PatientRegistration> OtherTypePatientRegistrationCollection
        {
            get => _OtherTypePatientRegistrationCollection; set
            {
                _OtherTypePatientRegistrationCollection = value;
                NotifyOfPropertyChange(() => OtherTypePatientRegistrationCollection);
            }
        }
        public int DoMoveTabContent
        {
            get { return _DoMoveTabContent; }
            set
            {
                _DoMoveTabContent = value;
                NotifyOfPropertyChange(() => DoMoveTabContent);
            }
        }
        public double gActualHeight
        {
            get => _gActualHeight; set
            {
                _gActualHeight = value;
                NotifyOfPropertyChange(() => gActualHeight);
                NotifyOfPropertyChange(() => nActualHeight);
            }
        }
        public double gActualWidth
        {
            get => _gActualWidth; set
            {
                _gActualWidth = value;
                NotifyOfPropertyChange(() => gActualWidth);
                NotifyOfPropertyChange(() => nActualWidth);
            }
        }
        public double nActualHeight
        {
            get => -1 * gActualHeight;
        }
        public double nActualWidth
        {
            get => -1 * gActualWidth;
        }
        public IPrescriptionAndConfirmHI ConfirmHIView
        {
            get => _ConfirmHIView; set
            {
                _ConfirmHIView = value;
                NotifyOfPropertyChange(() => ConfirmHIView);
            }
        }
        public bool ConfirmHIViewVisible
        {
            get => _ConfirmHIViewVisible; set
            {
                _ConfirmHIViewVisible = value;
                NotifyOfPropertyChange(() => ConfirmHIViewVisible);
            }
        }
        public int StatusIndex
        {
            get => _StatusIndex; set
            {
                _StatusIndex = value;
                NotifyOfPropertyChange(() => StatusIndex);
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
                ConfirmHIView.Registration_DataStorage = Registration_DataStorage;
            }
        }

        private int _ViewCase;
        public int ViewCase
        {
            get
            {
                return _ViewCase;
            }
            set
            {
                if (_ViewCase == value)
                {
                    return;
                }
                _ViewCase = value;
                NotifyOfPropertyChange(() => ViewCase);
            }
        }
        private int _PatientRegistrationCount;
        public int PatientRegistrationCount
        {
            get { return _PatientRegistrationCount; }
            set
            {
                _PatientRegistrationCount = value;
                NotifyOfPropertyChange(() => PatientRegistrationCount);
            }
        }
        private int _OtherTypePatientRegistrationCount;
        public int OtherTypePatientRegistrationCount
        {
            get { return _OtherTypePatientRegistrationCount; }
            set
            {
                _OtherTypePatientRegistrationCount = value;
                NotifyOfPropertyChange(() => OtherTypePatientRegistrationCount);
            }
        }
        #endregion
        #region Methods
        private IEnumerator<IResult> LoadDepartments()
        {
            ObservableCollection<RefDepartment> tempDepartments = new ObservableCollection<RefDepartment>();
            var departmentTask = new LoadDepartmentsByV_DeptTypeOperationTask(new List<long> { (long)V_DeptTypeOperation.KhoaNgoaiTru, (long)V_DeptTypeOperation.KhoaNoi });
            yield return departmentTask;
            RefDepartmentCollection = departmentTask.Departments.Where(x => x.V_DeptTypeOperation != (long)V_DeptTypeOperation.KhoaNgoaiTru || x.DeptID == Globals.ServerConfigSection.Hospitals.KhoaPhongKham).ToObservableCollection();
            if (RefDepartmentCollection == null) RefDepartmentCollection = new ObservableCollection<RefDepartment>();
            RefDepartmentCollection.Insert(0, new RefDepartment { DeptID = 0, DeptName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K1997_G1_ChonKhoa) });
            SearchCriteria.DeptID = RefDepartmentCollection.FirstOrDefault().DeptID;
            cboDepartments_SelectionChanged(null, null);
            yield break;
        }
        private IEnumerator<IResult> LoadLocations(long deptId)
        {
            if (deptId > 0)
            {
                var deptLoc = new LoadDeptLoctionByIDTask(deptId);
                yield return deptLoc;
                if (deptLoc.DeptLocations != null)
                {
                    LocationCollection = new ObservableCollection<DeptLocation>(deptLoc.DeptLocations);
                }
                else
                {
                    LocationCollection = new ObservableCollection<DeptLocation>();
                }
            }
            else
            {
                LocationCollection = new ObservableCollection<DeptLocation>();
            }
            LocationCollection.Insert(0, new DeptLocation { DeptLocationID = 0, Location = new Location { LID = 0, LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2094_G1_ChonPg) } });
            SearchCriteria.DeptLocationID = LocationCollection.FirstOrDefault().DeptLocationID;
            yield break;
        }
        private void ChangeDoMoveTabContent(int aDoMoveTabContent)
        {
            if (DoMoveTabContent == 1 && aDoMoveTabContent == 2)
            {
                DoMoveTabContent = aDoMoveTabContent;
            }
        }
        #endregion
        #region Events
        public void cboDepartments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Coroutine.BeginExecute(LoadLocations(SearchCriteria.DeptID.GetValueOrDefault(0)));
        }
        public void btnSearch()
        {
            if (StatusIndex == 0) SearchCriteria.IsReported = null;
            else if (StatusIndex == 1) SearchCriteria.IsReported = true;
            else if (StatusIndex == 2) SearchCriteria.IsReported = false;

            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSearchRegistrationsForHIReport(SearchCriteria, ViewCase,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    var retval = contract.EndSearchRegistrationsForHIReport(asyncResult).ToObservableCollection();
                                    PatientRegistrationCollection = retval.Where(x => x.ConfirmHIStaffID.GetValueOrDefault(0) == 0).ToObservableCollection();
                                    OtherTypePatientRegistrationCollection = retval.Where(x => x.ConfirmHIStaffID.GetValueOrDefault(0) != 0).ToObservableCollection();
                                    PatientRegistrationCount = PatientRegistrationCollection.Count;
                                    OtherTypePatientRegistrationCount = OtherTypePatientRegistrationCollection.Count;
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        //▼====: #001
        private string strNameExcel = "";
        public void BtnExportExcel()
        {
            if (StatusIndex == 0) SearchCriteria.IsReported = null;
            else if (StatusIndex == 1) SearchCriteria.IsReported = true;
            else if (StatusIndex == 2) SearchCriteria.IsReported = false;

            if (((System.Windows.FrameworkElement)TabControlView.SelectedItem).Name == "WaitConfirm")
            {
                this.ShowBusyIndicator(eHCMSResources.Z0669_G1_DangLayDLieu);
                var t = new Thread(() =>
                {
                    try
                    {
                        using (var serviceFactory = new TransactionServiceClient())
                        {
                            var contract = serviceFactory.ServiceInstance;
                            contract.BeginExportExcelRegistrationsForHIReportWaitConfirm(SearchCriteria,
                                Globals.DispatchCallback(asyncResult =>
                                {
                                    try
                                    {
                                        strNameExcel = string.Format("{0} ", eHCMSResources.Z2374_G1_ChoXacNhan);
                                        var results = contract.EndExportExcelRegistrationsForHIReportWaitConfirm(asyncResult);
                                        ExportToExcelFileAllData.Export(results, strNameExcel);
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
                        MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        this.HideBusyIndicator();
                    }
                });
                t.Start();
            }
            else
            {
                this.ShowBusyIndicator(eHCMSResources.Z0669_G1_DangLayDLieu);
                var t = new Thread(() =>
                {
                    try
                    {
                        using (var serviceFactory = new TransactionServiceClient())
                        {
                            var contract = serviceFactory.ServiceInstance;
                            contract.BeginExportExcelRegistrationsForHIReportOther(SearchCriteria,
                                Globals.DispatchCallback(asyncResult =>
                                {
                                    try
                                    {
                                        strNameExcel = string.Format("{0} ", eHCMSResources.K0832_G1_3Khac);
                                        var results = contract.EndExportExcelRegistrationsForHIReportOther(asyncResult);
                                        ExportToExcelFileAllData.Export(results, strNameExcel);
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
                        MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        this.HideBusyIndicator();
                    }
                });
                t.Start();
            }

        }

        TabControl TabControlView;
        public void TabControl_Loaded(object sender, RoutedEventArgs e)
        {
            TabControlView = (sender as TabControl);
        }
        //▲====: #001

        public void gvItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is DataGrid) || (sender as DataGrid).SelectedItem == null || !((sender as DataGrid).SelectedItem is PatientRegistration))
            {
                return;
            }
            ConfirmHIViewVisible = true;
            DoMoveTabContent = 1;
            ConfirmHIView.LoadAllTabDetails((sender as DataGrid).SelectedItem as PatientRegistration);
            //ConfirmHIView.ApplySearchInvoiceCriteriaValues(null, ((sender as DataGrid).SelectedItem as PatientRegistration).PtRegistrationID);
            //ConfirmHIView.SearchInvoiceOld();
        }
        public void UCConfirmHIRegistrationView_Loaded(object sender, RoutedEventArgs e)
        {
            gActualHeight = (sender as UserControl).ActualHeight;
            gActualWidth = (sender as UserControl).ActualWidth;
        }
        #endregion
        #region KeyHandles

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
            yield return new InputBindingCommand(() => ChangeDoMoveTabContent(2))
            {
                HotKey_Registered_Name = "ghkCloseDetWin",
                GestureModifier = ModifierKeys.Control,
                GestureKey = (Key)Keys.N
            };
        }
        #endregion

        public void hplPreview_Click(object datacontext)
        {
            if (datacontext == null) return;
            Action<IReportDocumentPreview> onInitDlg = delegate (IReportDocumentPreview proAlloc)
            {
                proAlloc.ID = ObjectCopier.DeepCopy((datacontext as PatientRegistration)).PtRegistrationID;
                proAlloc.eItem = ReportName.TEMP12;
                if (Globals.ServerConfigSection.CommonItems.ShowLoginNameOnReport38)
                {
                    proAlloc.StaffFullName = Globals.LoggedUserAccount.Staff.FullName;
                }
                else
                {
                    proAlloc.StaffFullName = "";
                }
            };
            GlobalsNAV.ShowDialog<IReportDocumentPreview>(onInitDlg, null, false, true, new Size(1500, 1000));
        }
    }
}
