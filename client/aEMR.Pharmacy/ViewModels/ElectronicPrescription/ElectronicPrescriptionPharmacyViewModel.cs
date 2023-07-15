using aEMR.Common;
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.Common.HotKeyManagement;
using aEMR.CommonTasks;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using DataEntities;
using eHCMS.CommonUserControls.CommonTasks;
using eHCMSLanguage;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
/*
* 20230211 #001 QTD: Clone màn hình đơn thuốc điện tử cho Nhà thuốc
*/
namespace aEMR.Pharmacy.ViewModels.ElectronicPrescription
{
    [Export(typeof(IElectronicPrescriptionPharmacy)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ElectronicPrescriptionPharmacyViewModel : ViewModelBase, IElectronicPrescriptionPharmacy, 
        IHandle<ElectronicPrescriptionPharmacyDeleteEvent>
    {
        [ImportingConstructor]
        public ElectronicPrescriptionPharmacyViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching, IEventAggregator eventArg)
        {
            eventArg.Subscribe(this);
            base.HasInputBindingCmd = true;
            gDQGReport = new DQGReport();
            gDQGReport.FromDate = Globals.GetCurServerDateTime();
            gDQGReport.ToDate = gDQGReport.FromDate;
            Coroutine.BeginExecute(LoadDepartments());
            PatientRegistrationCollection = new PagedSortableCollectionView<PatientRegistration> { PageSize = 20 };
            OtherTypePatientRegistrationCollection = new PagedSortableCollectionView<PatientRegistration> { PageSize = 20 };
            PatientRegistrationCollection.OnRefresh += Registration_OnRefresh;
            Globals.EventAggregator.Subscribe(this);
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
        private int _StatusIndex = 0;
        private PagedSortableCollectionView<PatientRegistration> _PatientRegistrationCollection;
        private PagedSortableCollectionView<PatientRegistration> _OtherTypePatientRegistrationCollection;
        //private string gIAPISendHIReportAddress = "http://egw.baohiemxahoi.gov.vn/";
        //private string gIAPISendHIReportAddressParams = "api/egw/guiHoSoGiamDinh4210?token={0}&id_token={1}&username={2}&password={3}&loaiHoSo=3&maTinh={4}&maCSKCB={5}";
        private string _OutputErrorMessage;
        private bool _AllChecked;
        private bool _IsAllDQGReportID = false;
        private bool _IsAllWaitDQGReportID = false;
        private enum UpdateCodeDQGReportCase : byte
        {
            phieu_nhap = 0,
            don_thuoc = 1,
            hoa_don = 2,
            phieu_xuat = 3,
            DQGReport = 4
        }

        private DQGReport _gDQGReport;
        public DQGReport gDQGReport
        {
            get
            {
                return _gDQGReport;
            }
            set
            {
                _gDQGReport = value;
                NotifyOfPropertyChange(() => gDQGReport);
            }
        }
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
        public int StatusIndex
        {
            get => _StatusIndex; set
            {
                _StatusIndex = value;
                NotifyOfPropertyChange(() => StatusIndex);
            }
        }
        public PagedSortableCollectionView<PatientRegistration> PatientRegistrationCollection
        {
            get => _PatientRegistrationCollection; set
            {
                _PatientRegistrationCollection = value;
                NotifyOfPropertyChange(() => PatientRegistrationCollection);
            }
        }
        public PagedSortableCollectionView<PatientRegistration> OtherTypePatientRegistrationCollection
        {
            get => _OtherTypePatientRegistrationCollection; set
            {
                _OtherTypePatientRegistrationCollection = value;
                NotifyOfPropertyChange(() => OtherTypePatientRegistrationCollection);
            }
        }
        public string OutputErrorMessage
        {
            get => _OutputErrorMessage; set
            {
                _OutputErrorMessage = value;
                NotifyOfPropertyChange(() => OutputErrorMessage);
            }
        }
        public bool AllChecked
        {
            get => _AllChecked; set
            {
                _AllChecked = value;
                NotifyOfPropertyChange(() => AllChecked);
                if (PatientRegistrationCollection != null && PatientRegistrationCollection.Count > 0)
                {
                    foreach (var item in PatientRegistrationCollection)
                    {
                        item.IsSelected = AllChecked;
                    }
                }
            }
        }
        private PatientRegistration _SelectedPatientRegistration;
        public PatientRegistration SelectedPatientRegistration
        {
            get => _SelectedPatientRegistration; set
            {
                _SelectedPatientRegistration = value;
                NotifyOfPropertyChange(() => SelectedPatientRegistration);
            }
        }
        private DataGrid gvRegistrations { get; set; }
        public bool IsAllDQGReportID
        {
            get => _IsAllDQGReportID; set
            {
                _IsAllDQGReportID = value;
                NotifyOfPropertyChange(() => IsAllDQGReportID);
            }
        }
        public bool IsAllWaitDQGReportID
        {
            get => _IsAllWaitDQGReportID; set
            {
                _IsAllWaitDQGReportID = value;
                NotifyOfPropertyChange(() => IsAllWaitDQGReportID);
            }
        }

        //private int _ViewCase;
        //public int ViewCase
        //{
        //    get
        //    {
        //        return _ViewCase;
        //    }
        //    set
        //    {
        //        if (_ViewCase == value)
        //        {
        //            return;
        //        }
        //        _ViewCase = value;
        //        NotifyOfPropertyChange(() => ViewCase);
        //    }
        //}
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

        //private void UpdateHIReportTask(GenericCoRoutineTask genTask, object aHealthInsuranceReport)
        //{
        //    var t = new Thread(() =>
        //    {
        //        try
        //        {
        //            using (var serviceFactory = new TransactionServiceClient())
        //            {
        //                var contract = serviceFactory.ServiceInstance;
        //                contract.BeginUpdateHIReportStatus(aHealthInsuranceReport as HealthInsuranceReport,
        //                    Globals.DispatchCallback((asyncResult) =>
        //                    {
        //                        try
        //                        {
        //                            if (contract.EndUpdateHIReportStatus(asyncResult))
        //                            {
        //                                genTask.ActionComplete(true);
        //                            }
        //                            else
        //                            {
        //                                MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
        //                                genTask.ActionComplete(false);
        //                                this.HideBusyIndicator();
        //                            }
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
        //                            genTask.ActionComplete(false);
        //                            this.HideBusyIndicator();
        //                        }
        //                    }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
        //            genTask.ActionComplete(false);
        //            this.HideBusyIndicator();
        //        }
        //    });
        //    t.Start();
        //}

        //private string GetErrorMessageFromErrorCode(int aErrorCode)
        //{
        //    switch (aErrorCode)
        //    {
        //        case 1001:
        //            return "Kích thước file quá lớn (20MB trong khoảng thời gian từ 8 giờ đến 11 giờ và từ 14 giờ đến 19 giờ từ thứ 2 đến thứ 6,100MB với các thời gian khác)";
        //        case 205:
        //            return "Lỗi sai định dạng tham số";
        //        case 401:
        //            return "Sai tài khoản hoặc mật khẩu";
        //        case 123:
        //            return "Sai định dạng file";
        //        case 124:
        //            return "Lỗi khi lưu dữ liệu( file sẽ được tự động gửi lại)";
        //        case 701:
        //            return "Lỗi thời gian gửi,thời gian quyết toán chỉ dc trong tháng hoặc đến mồng 5 tháng sau";
        //        case 500:
        //            return "Lỗi trong quá trình gửi dữ liệu.Vui lòng liên hệ với nhân viên hỗ trợ để được hướng dẫn cụ thể";
        //        default:
        //            return "";
        //    }
        //}
        private void FocusSelectedView()
        {
            if (PatientRegistrationCollection != null && PatientRegistrationCollection.Count > 1 && SearchCriteria != null && !string.IsNullOrWhiteSpace(SearchCriteria.PatientCode))
            {
                SelectedPatientRegistration = PatientRegistrationCollection.Skip(PatientRegistrationCollection.IndexOf(SelectedPatientRegistration)).FirstOrDefault(x => x.Patient != null && !string.IsNullOrEmpty(x.Patient.PatientCode) && x.Patient.PatientCode.ToLower() == SearchCriteria.PatientCode.ToLower());
                if (SelectedPatientRegistration == null)
                {
                    SelectedPatientRegistration = PatientRegistrationCollection.FirstOrDefault(x => x.Patient != null && !string.IsNullOrEmpty(x.Patient.PatientCode) && x.Patient.PatientCode.ToLower() == SearchCriteria.PatientCode.ToLower());
                }
                if (SelectedPatientRegistration != null)
                {
                    gvRegistrations.ScrollIntoView(SelectedPatientRegistration);
                }
            }
        }
        #endregion

        #region Events
        public void gvRegistrations_Loaded(object sender, RoutedEventArgs e)
        {
            gvRegistrations = sender as DataGrid;
        }

        public void gvRegistrations_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
            PatientRegistration item = e.Row.DataContext as PatientRegistration;
            if (item == null)
            {
                return;
            }
            if (item.HasPayOutPtDrugBill)
            {
                e.Row.Background = new SolidColorBrush(Colors.Yellow);
            }
        }

        public void cboDepartments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Coroutine.BeginExecute(LoadLocations(SearchCriteria.DeptID.GetValueOrDefault(0)));
        }
        private void btnSearch(int pageIndex, int pageSize)
        {
            SearchCriteria.ViewCase = Convert.ToByte(StatusIndex);
            if (PatientTypeIndex == 0)
            {
                SearchCriteria.PatientFindBy = AllLookupValues.PatientFindBy.NGOAITRU;
            }
            else if (PatientTypeIndex == 1)
            {
                SearchCriteria.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;
            }
            else if (PatientTypeIndex == 2)
            {
                SearchCriteria.PatientFindBy = AllLookupValues.PatientFindBy.CAHAI;
            }
            else if (PatientTypeIndex == 3)
            {
                SearchCriteria.PatientFindBy = AllLookupValues.PatientFindBy.DTNGOAITRU;
            }
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        bool vIsTotalTab = true;
                        contract.BeginSearchRegistrationsForElectronicPrescriptionPharmacy(SearchCriteria, vIsTotalTab, pageIndex, pageSize
                        , Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                int totalCount = 0;
                                IList<PatientRegistration> allItems = null;
                                PatientRegistrationCollection.Clear();
                                OtherTypePatientRegistrationCollection.Clear();
                                allItems = contract.EndSearchRegistrationsForElectronicPrescriptionPharmacy(out totalCount, asyncResult).ToObservableCollection();
                                if (vIsTotalTab)
                                {
                                    PatientRegistrationCollection.TotalItemCount = totalCount;
                                    if (allItems != null)
                                    {
                                        foreach (var item in allItems)
                                        {
                                            PatientRegistrationCollection.Add(item);
                                        }
                                    }
                                    if (PatientRegistrationCollection != null 
                                        && PatientRegistrationCollection.Any(x => x.LastestPatientRegistrationDetail.prescriptionIssueHistory.DQGReportID > 0))
                                    {
                                        IsAllDQGReportID = false;
                                    }
                                    else
                                    {
                                        IsAllDQGReportID = true;
                                    }
                                }
                                else
                                {
                                    OtherTypePatientRegistrationCollection.TotalItemCount = totalCount;
                                    if (allItems != null)
                                    {
                                        foreach (var item in allItems)
                                        {
                                            OtherTypePatientRegistrationCollection.Add(item);
                                        }
                                    }
                                    if (OtherTypePatientRegistrationCollection != null 
                                        && OtherTypePatientRegistrationCollection.Any(x => x.LastestPatientRegistrationDetail.prescriptionIssueHistory.DQGReportID > 0))
                                    {
                                        IsAllWaitDQGReportID = true;
                                    }
                                    else
                                    {
                                        IsAllWaitDQGReportID = false;
                                    }
                                }                                  

                                //▼===== #002
                                if (SearchCriteria.FromDate != null && SearchCriteria.ToDate != null)
                                {
                                    FromDate = (DateTime)SearchCriteria.FromDate.Value.Date;
                                    if (Globals.GetCurServerDateTime().Date == (DateTime)SearchCriteria.ToDate.Value.Date)
                                    {
                                        ToDate = Globals.GetCurServerDateTime();
                                    }
                                    else if (Globals.GetCurServerDateTime().Date > (DateTime)SearchCriteria.ToDate.Value.Date)
                                    {
                                        DateTime TmpToDate = SearchCriteria.ToDate.Value;
                                        TmpToDate = TmpToDate.AddDays(1);
                                        ToDate = TmpToDate.AddSeconds(-1);
                                    }
                                }
                                //▲===== #002
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

        public void btnConfirm()
        {
            //if ((GlobalsNAV.gLoggedHIAPIUser == null || GlobalsNAV.gLoggedHIAPIUser.APIKey == null || string.IsNullOrEmpty(GlobalsNAV.gLoggedHIAPIUser.APIKey.access_token))
            //    || (GlobalsNAV.gLoggedHIAPIUser != null && GlobalsNAV.gLoggedHIAPIUser.APIKey != null && GlobalsNAV.gLoggedHIAPIUser.APIKey.expires_in <= DateTime.Now))
            //{
            //    GlobalsNAV.LoginHIAPI();
            //}
            Coroutine.BeginExecute(CreateHIReportOutInPtXml_Routine());
        }

        public void btnPrint12Template(object source)
        {
            if (source == null || !(source is PatientRegistration))
            {
                return;
            }
            PatientRegistration mPatientRegistration = source as PatientRegistration;

            IReportDocumentPreview DialogView = Globals.GetViewModel<IReportDocumentPreview>();
            DialogView.ID = mPatientRegistration.PtRegistrationID;
            DialogView.eItem = ReportName.TEMP12;
            if (Globals.ServerConfigSection.CommonItems.ApplyTemp12Version6556)
            {
                if (mPatientRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
                {
                    DialogView.eItem = ReportName.TEMP12_TONGHOP;
                    if (mPatientRegistration.AdmissionInfo != null && mPatientRegistration.AdmissionInfo.IsTreatmentCOVID)
                    {
                        DialogView.IsPatientCOVID = mPatientRegistration.AdmissionInfo.IsTreatmentCOVID;
                    }
                }
                else
                {
                    DialogView.eItem = ReportName.TEMP12_6556;
                }
            }
            else
            {
                DialogView.eItem = ReportName.TEMP12;
            }

            if (Globals.ServerConfigSection.CommonItems.ShowLoginNameOnReport38 && Globals.LoggedUserAccount.Staff != null)
            {
                DialogView.StaffFullName = Globals.LoggedUserAccount.Staff.FullName;
            }
            else
            {
                DialogView.StaffFullName = "";
            }
            DialogView.V_RegistrationType = (long)mPatientRegistration.V_RegistrationType;
            GlobalsNAV.ShowDialog_V3(DialogView, null, null, false, true, Globals.GetDefaultDialogViewSize());
        }

        public void btnPreviewPrescriptionDetails(object source)
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
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPreviewElectronicPrescriptionPharmacy(mPatientRegistration.PtRegistrationID, (long)mPatientRegistration.V_RegistrationType
                        , mPatientRegistration.LastestPatientRegistrationDetail.prescriptionIssueHistory.IssueID, mPatientRegistration.OutPtTreatmentProgramID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string ErrText;
                            var mPreviewElectronicPrescription = contract.EndPreviewElectronicPrescriptionPharmacy(out ErrText, asyncResult);
                            
                            GlobalsNAV.ShowDialog<IElectronicPrescriptionPharmacyPreview>((mView) =>
                            {
                                //mView.ApplyPreviewHIReportSet(mPreviewHIReportSet, ErrText);
                                mView.ListDQG_don_thuoc = mPreviewElectronicPrescription.ToList();
                            }, null, false, true, Globals.GetDefaultDialogViewSize());
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

        public void btnDeletePrescription(object source)
        {
            if (source == null || !(source is PatientRegistration))
            {
                return;
            }
            PatientRegistration mPatientRegistration = source as PatientRegistration;
            if (mPatientRegistration == null || mPatientRegistration.PtRegistrationID == 0
                || mPatientRegistration.LastestPatientRegistrationDetail.prescriptionIssueHistory.DQGReportID == 0
                || mPatientRegistration.LastestPatientRegistrationDetail.prescriptionIssueHistory.IssueID == 0)
            {
                return;
            }
            Action<IElectronicPrescriptionPharmacyDelete> onInitDlg = (mDelete) =>
            {
                mDelete.PtRegistrationID = mPatientRegistration.PtRegistrationID;
                mDelete.DQGReportID = mPatientRegistration.LastestPatientRegistrationDetail.prescriptionIssueHistory.DQGReportID;
                mDelete.V_RegistrationType = (long)mPatientRegistration.V_RegistrationType;
                mDelete.IssueID = mPatientRegistration.LastestPatientRegistrationDetail.prescriptionIssueHistory.IssueID;
            };
            GlobalsNAV.ShowDialog(onInitDlg);
        }

        public void gvRegistrations_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!(sender is DataGrid) && (sender as DataGrid).SelectedItem == null && !((sender as DataGrid).SelectedItem is PatientRegistration))
            {
                return;
            }
            btnPreviewPrescriptionDetails((sender as DataGrid).SelectedItem as PatientRegistration);
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
            yield return new InputBindingCommand(() => FocusSelectedView())
            {
                HotKey_Registered_Name = "ghkCloseDetWin",
                GestureModifier = ModifierKeys.Control,
                GestureKey = (Key)Keys.F
            };
        }
        #endregion
        private int _PatientTypeIndex = 2;
        public int PatientTypeIndex
        {
            get => _PatientTypeIndex;
            set
            {
                _PatientTypeIndex = value;
                NotifyOfPropertyChange(() => PatientTypeIndex);
            }
        }

        private DateTime _FromDate = Globals.GetCurServerDateTime();
        public DateTime FromDate
        {
            get => _FromDate;
            set
            {
                _FromDate = value;
                NotifyOfPropertyChange(() => FromDate);
            }
        }
        private DateTime _ToDate = Globals.GetCurServerDateTime();
        public DateTime ToDate
        {
            get => _ToDate;
            set
            {
                _ToDate = value;
                NotifyOfPropertyChange(() => ToDate);
            }
        }

        public void gvOtherRegistrations_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        void Registration_OnRefresh(object sender, RefreshEventArgs e)
        {
            //int PageSize;
            //int PageIndex;
            //if (IsTotalTab)
            //{
            //    PageSize = PatientRegistrationCollection.PageSize;
            //    PageIndex = PatientRegistrationCollection.PageIndex;
            //}
            //else
            //{
            //    PageSize = OtherTypePatientRegistrationCollection.PageSize;
            //    PageIndex = OtherTypePatientRegistrationCollection.PageIndex;
            //}
            btnSearch(PatientRegistrationCollection.PageIndex, PatientRegistrationCollection.PageSize);
        }

        public void btnSearch()
        {
            //int PageSize;
            //if (IsTotalTab)
            //{
            //    PageSize = PatientRegistrationCollection.PageSize;
            //    PatientRegistrationCollection.PageIndex = 0;
            //}
            //else
            //{
            //    PageSize = OtherTypePatientRegistrationCollection.PageSize;
            //    OtherTypePatientRegistrationCollection.PageIndex = 0;
            //}
            PatientRegistrationCollection.PageIndex = 0;
            btnSearch(0, PatientRegistrationCollection.PageSize);
        }

        private IEnumerator<IResult> CreateHIReportOutInPtXml_Routine()
        {
            if (PatientRegistrationCollection == null 
                || !PatientRegistrationCollection.Any(x => x.IsSelected 
                    && x.LastestPatientRegistrationDetail.prescriptionIssueHistory.DQGReportID == 0
                    && x.DTDTReportID != 0
                    && x.LastestPatientRegistrationDetail.prescriptionIssueHistory.IsCanSelect)) yield break;

            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);

            string mTitle = string.Join(",", PatientRegistrationCollection.Where(x => x.IsSelected && x.LastestPatientRegistrationDetail.prescriptionIssueHistory.DQGReportID == 0 && x.DTDTReportID != 0 && x.LastestPatientRegistrationDetail.prescriptionIssueHistory.IsCanSelect)
                .Select(x => string.Format("{0}-{1}", x.RegistrationType.RegTypeID, x.LastestPatientRegistrationDetail.prescriptionIssueHistory.IssueID)).ToArray());

            var mCreateDQGReportFileTask = new GenericCoRoutineTask(CreateDQGReportOutInPtFileTask, mTitle);
            yield return mCreateDQGReportFileTask;

            DQGReport mDQGReport = mCreateDQGReportFileTask.GetResultObj(0) as DQGReport;
            if(mDQGReport != null)
            {
                if(mDQGReport.DQGReportID > 0)
                {
                    TransferDateToDQGByDQGReportID(mDQGReport.DQGReportID);
                }
            }

            //HealthInsuranceReport mHealthInsuranceReport = mCreateHIReportFileTask.GetResultObj(0) as HealthInsuranceReport;

            //if (mHealthInsuranceReport != null)
            //{
            //    if (mHealthInsuranceReport.HIReportID > 0)
            //    {
            //        var mCreateHIReportXmlTask = new GenericCoRoutineTask(CreateHIReportXmlTask, mHealthInsuranceReport);
            //        yield return mCreateHIReportXmlTask;

            //        HIAPIUploadHIReportXmlResult mHIAPIUploadHIReportXmlResult = mCreateHIReportXmlTask.GetResultObj(0) as HIAPIUploadHIReportXmlResult;
            //        mHealthInsuranceReport.V_ReportStatus = (long)AllLookupValues.V_ReportStatus.Completed;
            //        mHealthInsuranceReport.ReportAppliedCode = mHIAPIUploadHIReportXmlResult.maGiaoDich;
            //        mHealthInsuranceReport.ReportAppliedResultCode = mHIAPIUploadHIReportXmlResult.maKetQua;

            //        var mUpdateHIReportTask = new GenericCoRoutineTask(UpdateHIReportTask, mHealthInsuranceReport);
            //        yield return mUpdateHIReportTask;

            //        foreach (var item in PatientRegistrationCollection.Where(x => x.IsSelected && x.HIReportID == 0 && x.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU))
            //        {
            //            item.HIReportID = mHealthInsuranceReport.HIReportID;
            //            item.ReportAppliedCode = mHealthInsuranceReport.ReportAppliedCode;
            //            item.V_ReportStatus.LookupID = mHealthInsuranceReport.V_ReportStatus;
            //            item.V_ReportStatus.ObjectValue = eHCMSResources.Z2333_G1_XacNhanBCHoanTat;
            //        }
            //    }
            //    if (mHealthInsuranceReport.HIReportOutPt > 0)
            //    {
            //        mHealthInsuranceReport.HIReportID = mHealthInsuranceReport.HIReportOutPt;
            //        var mCreateHIReportXmlTask = new GenericCoRoutineTask(CreateHIReportXmlTask, mHealthInsuranceReport);
            //        yield return mCreateHIReportXmlTask;

            //        HIAPIUploadHIReportXmlResult mHIAPIUploadHIReportXmlResult = mCreateHIReportXmlTask.GetResultObj(0) as HIAPIUploadHIReportXmlResult;
            //        mHealthInsuranceReport.V_ReportStatus = (long)AllLookupValues.V_ReportStatus.Completed;
            //        mHealthInsuranceReport.ReportAppliedCode = mHIAPIUploadHIReportXmlResult.maGiaoDich;
            //        mHealthInsuranceReport.ReportAppliedResultCode = mHIAPIUploadHIReportXmlResult.maKetQua;

            //        var mUpdateHIReportTask = new GenericCoRoutineTask(UpdateHIReportTask, mHealthInsuranceReport);
            //        yield return mUpdateHIReportTask;

            //        foreach (var item in PatientRegistrationCollection.Where(x => x.IsSelected && x.HIReportID == 0 && x.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU))
            //        {
            //            item.HIReportID = mHealthInsuranceReport.HIReportID;
            //            item.ReportAppliedCode = mHealthInsuranceReport.ReportAppliedCode;
            //            item.V_ReportStatus.LookupID = mHealthInsuranceReport.V_ReportStatus;
            //            item.V_ReportStatus.ObjectValue = eHCMSResources.Z2333_G1_XacNhanBCHoanTat;
            //        }
            //    }
            //}
            //MessageBox.Show(eHCMSResources.K0461_G1_XNhanBHOk, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            btnSearch(PatientRegistrationCollection.PageIndex, PatientRegistrationCollection.PageSize);
            this.HideBusyIndicator();
        }

        public void CreateDQGReportOutInPtFileTask(GenericCoRoutineTask genTask, object aIssueIDList)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        gDQGReport.IssueIDList = aIssueIDList.ToString();
                        contract.BeginCreateDQGReportOutInpt(gDQGReport,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    long mDQGReportID;
                                    long mDQGReportIDInpt;
                                    var mDQGReport = contract.EndCreateDQGReportOutInpt(out mDQGReportID, out mDQGReportIDInpt, asyncResult);
                                    if (mDQGReport || mDQGReportID > 0 || mDQGReportIDInpt > 0)
                                    {
                                        if (mDQGReportID > 0)
                                        {
                                            gDQGReport.DQGReportID = mDQGReportID;
                                        }
                                        if (mDQGReportIDInpt > 0)
                                        {
                                            gDQGReport.DQGReportIDInpt = mDQGReportIDInpt;
                                        }
                                        genTask.AddResultObj(gDQGReport);
                                        genTask.ActionComplete(true);
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                        OutputErrorMessage += Environment.NewLine + eHCMSResources.Z2334_G1_KhongTaoDuocBaoCao;
                                        ClientLoggerHelper.LogInfo(string.Format("CreateDQGReportOutInPtFileTask Try => {0}", OutputErrorMessage));
                                        genTask.ActionComplete(false);
                                        this.HideBusyIndicator();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    OutputErrorMessage = ex.Message;
                                    ClientLoggerHelper.LogInfo(string.Format("CreateDQGReportOutInPtFileTask Catch => {0}", OutputErrorMessage));
                                    genTask.ActionComplete(false);
                                    this.HideBusyIndicator();
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        public void TransferDateToDQGByDQGReportID(long DQGReportID)
        {
            if (DQGReportID == 0)
            {
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetDQGReportWithDetails(DQGReportID,
                            new AsyncCallback((asyncResult) =>
                            {
                                try
                                {
                                    DQGReport mDQGReport = contract.EndGetDQGReportWithDetails(asyncResult);
                                    if (mDQGReport == null || mDQGReport.DQGReportID == 0)
                                    {
                                        MessageBox.Show(eHCMSResources.T0074_G1_I, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    }
                                    this.HideBusyIndicator();
                                    //Coroutine.BeginExecute(TransferAllDataToDQG_Routine(mDQGReport));
                                }
                                catch (Exception ex)
                                {
                                    this.HideBusyIndicator();
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public void Handle(ElectronicPrescriptionPharmacyDeleteEvent message)
        {
            if (message != null)
            {
                btnSearch(PatientRegistrationCollection.PageIndex, PatientRegistrationCollection.PageSize);
            }
        }

        private TabControl TabControlView { get; set; }
        public void TabControl_Loaded(object sender, RoutedEventArgs e)
        {
            TabControlView = sender as TabControl;
        }
    }
}
