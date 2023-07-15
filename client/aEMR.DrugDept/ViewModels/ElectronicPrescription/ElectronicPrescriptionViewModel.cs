using aEMR.Common;
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.Common.HotKeyManagement;
using aEMR.CommonTasks;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
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
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
/*
* 20190918 #001 TTM:   BM 0013320: Bổ sung thêm bộ lọc tìm kiếm ca xác nhận BHYT theo loại bệnh nhân.
* 20200404 #002 TTM:   BM 0029083: Thay đổi thời gian tìm kiếm để lưu vào bảng HIReport. Trước giờ toàn lưu Datetime.Now cho cả ngày từ và đến.
* 20230213 #003 BLQ: Issue:2766 chỉnh sửa màn hình theo yêu cầu DTDT mới
*/
namespace aEMR.DrugDept.ViewModels.ElectronicPrescription
{
    [Export(typeof(IElectronicPrescription)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ElectronicPrescriptionViewModel : ViewModelBase, IElectronicPrescription
    {
        [ImportingConstructor]
        public ElectronicPrescriptionViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            base.HasInputBindingCmd = true;
            PatientRegistrationCollection = new PagedSortableCollectionView<PatientRegistration> { PageSize = 20 };
            PatientRegistrationCollection.OnRefresh += new EventHandler<RefreshEventArgs>(PatientRegistrationCollection_OnRefresh);
            OtherTypePatientRegistrationCollection = new PagedSortableCollectionView<PatientRegistration> { PageSize = 20 };
            OtherTypePatientRegistrationCollection.OnRefresh += new EventHandler<RefreshEventArgs>(OtherTypePatientRegistrationCollection_OnRefresh);
            Coroutine.BeginExecute(LoadDepartments());
        }
        void PatientRegistrationCollection_OnRefresh(object sender, RefreshEventArgs e)
        {
            GlobalsNAV.ShowMessagePopup("Bỏ các toa thuốc đã chọn");
            PatientRegistrationCollection_Paging(PatientRegistrationCollection.PageIndex, PatientRegistrationCollection.PageSize, false);
        }
        void OtherTypePatientRegistrationCollection_OnRefresh(object sender, RefreshEventArgs e)
        {
            GlobalsNAV.ShowMessagePopup("Bỏ các toa thuốc đã chọn");
            OtherTypePatientRegistrationCollection_Paging(OtherTypePatientRegistrationCollection.PageIndex, OtherTypePatientRegistrationCollection.PageSize, false);
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
        private int _StatusIndex = 2;
        private PagedSortableCollectionView<PatientRegistration> _PatientRegistrationCollection;
        private PagedSortableCollectionView<PatientRegistration> _OtherTypePatientRegistrationCollection;
      
      
        private string _OutputErrorMessage;
        private bool _AllChecked;
        private bool _IsAllDTDTReportID = false;
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
        public bool IsAllDTDTReportID
        {
            get => _IsAllDTDTReportID; set
            {
                _IsAllDTDTReportID = value;
                NotifyOfPropertyChange(() => IsAllDTDTReportID);
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
        public void btnSearch()
        {
            if (StatusIndex == 0)
            {
                SearchCriteria.IsReported = null;
                SearchCriteria.IsCancelDTDT = false;
            }
            else if (StatusIndex == 1)
            {
                SearchCriteria.IsReported = true;
                SearchCriteria.IsCancelDTDT = false;
            }
            else if (StatusIndex == 2)
            {
                SearchCriteria.IsReported = false;
                SearchCriteria.IsCancelDTDT = false;
            }
            else if (StatusIndex == 3)
            {
                SearchCriteria.IsCancelDTDT = true;
            }

            //▼===== #001
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
            //▲===== #001
            if (PatientTypeIndex != 3)
            {
                PatientRegistrationCollection_Paging(PatientRegistrationCollection.PageIndex, PatientRegistrationCollection.PageSize, true);
            }
            else
            {
                OtherTypePatientRegistrationCollection_Paging(OtherTypePatientRegistrationCollection.PageIndex, OtherTypePatientRegistrationCollection.PageSize, true);
            }
        }
        private void PatientRegistrationCollection_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSearchRegistrationsForElectronicPrescription(SearchCriteria, ViewCase, PageIndex, PageSize, "", CountTotal,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                int Total = 0;
                                IList<PatientRegistration> allItems = null;
                                bool bOK = false;
                                try
                                {
                                    allItems = contract.EndSearchRegistrationsForElectronicPrescription(out Total, asyncResult).ToObservableCollection();
                                    bOK = true;
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                }
                                PatientRegistrationCollection.Clear();
                                if (bOK)
                                {
                                    if (CountTotal)
                                    {
                                        PatientRegistrationCollection.TotalItemCount = Total;
                                    }
                                    if (allItems != null && allItems.Count > 0)
                                    {
                                        foreach (var item in allItems)
                                        {
                                            PatientRegistrationCollection.Add(item);
                                        }
                                    }

                                    if ((PatientRegistrationCollection != null && PatientRegistrationCollection.Any(x => x.DTDTReportID > 0)) || StatusIndex == 3)
                                    {
                                        IsAllDTDTReportID = true;
                                    }
                                    else
                                    {
                                        IsAllDTDTReportID = false;
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
        private void OtherTypePatientRegistrationCollection_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSearchRegistrationsForElectronicPrescription(SearchCriteria, ViewCase, PageIndex, PageSize, "", CountTotal,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                int Total = 0;
                                IList<PatientRegistration> allItems = null;
                                bool bOK = false;
                                try
                                {
                                    allItems = contract.EndSearchRegistrationsForElectronicPrescription(out Total, asyncResult).ToObservableCollection();
                                    bOK = true;
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                }
                                PatientRegistrationCollection.Clear();
                                OtherTypePatientRegistrationCollection.Clear();
                                if (bOK)
                                {
                                    if (CountTotal)
                                    {
                                        PatientRegistrationCollection.TotalItemCount = Total;
                                    }
                                    if (allItems != null && allItems.Count > 0)
                                    {
                                        foreach (var item in allItems)
                                        {
                                            if (item.IsSumTreatmentProgram)
                                            {
                                                PatientRegistrationCollection.Add(item);
                                            }
                                            else
                                            {
                                                OtherTypePatientRegistrationCollection.Add(item);
                                            }
                                        }
                                    }

                                    if (PatientRegistrationCollection != null && PatientRegistrationCollection.Any(x => x.DTDTReportID > 0))
                                    {
                                        IsAllDTDTReportID = true;
                                    }
                                    else
                                    {
                                        IsAllDTDTReportID = false;
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
            //if (!Globals.ServerConfigSection.CommonItems.NewMethodToReport4210)
            //{
            //    Coroutine.BeginExecute(CreateHIReportXml_Routine());
            //}
            //else
            //{
            //    Coroutine.BeginExecute(CreateHIReportOutInPtXml_Routine());
            //}

            Coroutine.BeginExecute(CreateDTDT_Routine());
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

        public void btnPreviewDTDTReport(object source)
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
            if (mPatientRegistration.DTDTReportID == -1)
            {
                GetPreviewElectronicPrescription_Cancel(mPatientRegistration.PtRegistrationID
                    , mPatientRegistration.ListOfPrescriptionIssueHistory.FirstOrDefault().IssueID
                    , (long)mPatientRegistration.V_RegistrationType, mPatientRegistration.OutPtTreatmentProgramID);
            }
            else
            {
                GetPreviewElectronicPrescription(mPatientRegistration.PtRegistrationID, (long)mPatientRegistration.V_RegistrationType, mPatientRegistration.OutPtTreatmentProgramID);
            }
        }
        private void GetPreviewElectronicPrescription(long PtRegistrationID, long V_RegistrationType, long? OutPtTreatmentProgramID)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPreviewElectronicPrescription(PtRegistrationID, V_RegistrationType, OutPtTreatmentProgramID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string ErrText;
                            var mPreviewElectronicPrescription = contract.EndPreviewElectronicPrescription(out ErrText, asyncResult);

                            GlobalsNAV.ShowDialog<IElectronicPrescriptionPreview>((mView) =>
                            {
                                //mView.ApplyPreviewHIReportSet(mPreviewHIReportSet, ErrText);
                                mView.ListDTDT_don_thuoc = mPreviewElectronicPrescription.ToList();
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
        private void GetPreviewElectronicPrescription_Cancel(long PtRegistrationID, long IssueID, long V_RegistrationType, long? OutPtTreatmentProgramID)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPreviewElectronicPrescription_Cancel(PtRegistrationID, IssueID, V_RegistrationType, OutPtTreatmentProgramID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string ErrText;
                            var mPreviewElectronicPrescription = contract.EndPreviewElectronicPrescription_Cancel(out ErrText, asyncResult);

                            GlobalsNAV.ShowDialog<IElectronicPrescriptionPreview>((mView) =>
                            {
                                //mView.ApplyPreviewHIReportSet(mPreviewHIReportSet, ErrText);
                                mView.ListDTDT_don_thuoc = mPreviewElectronicPrescription.ToList();
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
        public void btnCancelConfirmDTDTReport(object source)
         {
            if (source == null || !(source is PatientRegistration))
            {
                return;
            }
            PatientRegistration mPatientRegistration = source as PatientRegistration;
            if (mPatientRegistration == null || mPatientRegistration.PtRegistrationID == 0 || mPatientRegistration.DTDTReportID == 0)
            {
                return;
            }
           
            IErrorBold MessBox = Globals.GetViewModel<IErrorBold>();
            MessBox.FireOncloseEvent = true;
            MessBox.IsShowReason = true;
            MessBox.SetMessage("Bạn có chắc chắn muốn xóa tất cả toa thuốc của đăng ký này không?", "Đồng ý");
            GlobalsNAV.ShowDialog_V3(MessBox);
            if (!MessBox.IsAccept)
            {
                return;
            }
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new TransactionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginCancelConfirmDTDTReport(mPatientRegistration.PtRegistrationID, (long)mPatientRegistration.V_RegistrationType
                        , Globals.LoggedUserAccount.Staff.StaffID
                        , MessBox.Reason
                        , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                if (contract.EndCancelConfirmDTDTReport(asyncResult))
                                {
                                    MessageBox.Show("Xóa thành công");
                                    PatientRegistrationCollection.Remove(mPatientRegistration);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.T0074_G1_I, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
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
            });

            t.Start();
        }

        public void gvRegistrations_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!(sender is DataGrid) && (sender as DataGrid).SelectedItem == null && !((sender as DataGrid).SelectedItem is PatientRegistration))
            {
                return;
            }
            PatientRegistration mPatientRegistration = (sender as DataGrid).SelectedItem as PatientRegistration;
            if (mPatientRegistration == null)
            {
                return;
            }
            if(mPatientRegistration.DTDTReportID == -1)
            {
                GetPreviewElectronicPrescription_Cancel(mPatientRegistration.PtRegistrationID
                    , mPatientRegistration.ListOfPrescriptionIssueHistory.FirstOrDefault().IssueID
                    , (long)mPatientRegistration.V_RegistrationType, mPatientRegistration.OutPtTreatmentProgramID);
            }
            else
            {
                GetPreviewElectronicPrescription(mPatientRegistration.PtRegistrationID, (long)mPatientRegistration.V_RegistrationType, mPatientRegistration.OutPtTreatmentProgramID);
            }
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
        //▼===== #001
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
        //▲===== #001
        //▼===== #002
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
        //▲===== #002
        public void gvOtherRegistrations_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }
        public void hplCheck_Click(object selectedItem)
        {
            if (PatientRegistrationCollection.Where(x => x.IsSelected).Count() > Globals.ServerConfigSection.CommonItems.ElectronicPrescriptionMaxReport)
            {
                (selectedItem as PatientRegistration).IsSelected = false;
                MessageBox.Show("Số lượng phiếu vượt quá trong 1 lần xác nhận được cấu hình \"" + Globals.ServerConfigSection.CommonItems.ElectronicPrescriptionMaxReport + "\"");
            }
        }
        //▼====: #019
        private IEnumerator<IResult> CreateDTDT_Routine()
        {
            if (PatientRegistrationCollection == null || !PatientRegistrationCollection.Any(x => x.IsSelected && x.DTDTReportID == 0)) yield break;

            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);

            //string mTitle = string.Format("{0}-{1}", (byte)AllLookupValues.RegTypeID.NGOAI_TRU, PatientInfo.LatestRegistration.PtRegistrationID);
            string mTitle = string.Join(",", PatientRegistrationCollection.Where(x => x.IsSelected && x.DTDTReportID == 0).Select(x => string.Format("{0}-{1}", x.RegistrationType.RegTypeID, x.PtRegistrationID)).ToArray());
            var mCreateDTDTReportFileTask = new GenericCoRoutineTask(CreateDTDTReportOutInPtFileTask, mTitle);
            yield return mCreateDTDTReportFileTask;

            DTDTReport mDTDTReport = mCreateDTDTReportFileTask.GetResultObj(0) as DTDTReport;

            if (mDTDTReport != null)
            {
                if (mDTDTReport.DTDTReportID <= 0 && mDTDTReport.DTDTReportOutPt <= 0)
                {
                    GlobalsNAV.ShowMessagePopup("Có lỗi phát sinh trong quá trình tạo DTDT." + Environment.NewLine + "Vui lòng liên hệ phòng phần mềm để kiểm tra");
                    yield break;
                }
                if (mDTDTReport.DTDTReportID > 0)
                {
                    mDTDTReport.DTDTReportID = mDTDTReport.DTDTReportID;
                    mDTDTReport.V_RegistrationType = (long)AllLookupValues.RegistrationType.NOI_TRU;
                    var mCreateHIReportXmlTask = new GenericCoRoutineTask(CreateDTDTReport_InPtAndCalLAPITask, mDTDTReport);
                    yield return mCreateHIReportXmlTask;

                    DTDTAPIResponse DTDTAPIResponseSendDonThuocTemp = mCreateHIReportXmlTask.GetResultObj(0) as DTDTAPIResponse;

                    mDTDTReport.V_ReportStatus = (long)AllLookupValues.V_ReportStatus.Completed;
                    mDTDTReport.CheckSum = DTDTAPIResponseSendDonThuocTemp.checksum;
                    //▼====: #020
                    // ngoại lệ. đánh dấu toàn bộ DTDTReport
                    if (!DTDTAPIResponseSendDonThuocTemp.IsTransferCompleted)
                    {
                        mDTDTReport.V_ReportStatus = (long)AllLookupValues.V_ReportStatus.Errored;
                        mDTDTReport.CheckSum = "";
                        var mDeleteDTDTReportTask = new GenericCoRoutineTask(DeleteDTDTReportTask, mDTDTReport.DTDTReportID);
                        yield return mDeleteDTDTReportTask;
                    }
                    // xóa dữ liệu của toa thuốc bị lỗi để tự đẩy lại
                    else if (DTDTAPIResponseSendDonThuocTemp.ListPrescriptionErrorWhenCallAPI != null
                           && DTDTAPIResponseSendDonThuocTemp.ListPrescriptionErrorWhenCallAPI.Count > 0)
                    {
                        mDTDTReport.V_ReportStatus = (long)AllLookupValues.V_ReportStatus.Errored;
                        mDTDTReport.CheckSum = "";
                        var mDeleteRegistrationDTDTReportTask = new GenericCoRoutineTask(DeleteRegistrationDTDTReportTask, DTDTAPIResponseSendDonThuocTemp.ListPrescriptionErrorWhenCallAPI);
                        yield return mDeleteRegistrationDTDTReportTask;
                    }
                    //▲====: #020

                    var mUpdateDTDTReportTask = new GenericCoRoutineTask(UpdateDTDTReportTask, mDTDTReport);
                    yield return mUpdateDTDTReportTask;
                    foreach (var item in PatientRegistrationCollection.Where(x => x.IsSelected && x.DTDTReportID == 0))
                    {
                        item.DTDTReportID = mDTDTReport.DTDTReportID;
                    }
                    GlobalsNAV.ShowMessagePopup("Gửi dữ liệu đơn thuốc điện tử hoàn tất");
                }
                //else
                //{
                //    GlobalsNAV.ShowMessagePopup("Có lỗi phát sinh trong quá trình tạo DTDT." + Environment.NewLine + "Vui lòng liên hệ phòng phần mềm để kiểm tra");
                //}
                if (mDTDTReport.DTDTReportOutPt > 0)
                {
                    mDTDTReport.DTDTReportID = mDTDTReport.DTDTReportOutPt;
                    mDTDTReport.V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU;
                    var mCreateHIReportXmlTask = new GenericCoRoutineTask(CreateDTDTReportAndCalLAPITask, mDTDTReport);
                    yield return mCreateHIReportXmlTask;

                    DTDTAPIResponse DTDTAPIResponseSendDonThuocTemp = mCreateHIReportXmlTask.GetResultObj(0) as DTDTAPIResponse;

                    mDTDTReport.V_ReportStatus = (long)AllLookupValues.V_ReportStatus.Completed;
                    mDTDTReport.CheckSum = DTDTAPIResponseSendDonThuocTemp.checksum;
                    //▼====: #020
                    // ngoại lệ. đánh dấu toàn bộ DTDTReport
                    if (!DTDTAPIResponseSendDonThuocTemp.IsTransferCompleted)
                    {
                        mDTDTReport.V_ReportStatus = (long)AllLookupValues.V_ReportStatus.Errored;
                        mDTDTReport.CheckSum = "";
                        var mDeleteDTDTReportTask = new GenericCoRoutineTask(DeleteDTDTReportTask, mDTDTReport.DTDTReportID);
                        yield return mDeleteDTDTReportTask;
                    }
                    // xóa dữ liệu của toa thuốc bị lỗi để tự đẩy lại
                    else if (DTDTAPIResponseSendDonThuocTemp.ListPrescriptionErrorWhenCallAPI != null
                           && DTDTAPIResponseSendDonThuocTemp.ListPrescriptionErrorWhenCallAPI.Count > 0)
                    {
                        mDTDTReport.V_ReportStatus = (long)AllLookupValues.V_ReportStatus.Errored;
                        mDTDTReport.CheckSum = "";
                        var mDeleteRegistrationDTDTReportTask = new GenericCoRoutineTask(DeleteRegistrationDTDTReportTask, DTDTAPIResponseSendDonThuocTemp.ListPrescriptionErrorWhenCallAPI);
                        yield return mDeleteRegistrationDTDTReportTask;
                    }
                    //▲====: #020

                    var mUpdateDTDTReportTask = new GenericCoRoutineTask(UpdateDTDTReportTask, mDTDTReport);
                    yield return mUpdateDTDTReportTask;
                    foreach (var item in PatientRegistrationCollection.Where(x => x.IsSelected && x.DTDTReportID == 0))
                    {
                        item.DTDTReportID = mDTDTReport.DTDTReportID;
                    }
                    GlobalsNAV.ShowMessagePopup("Gửi dữ liệu đơn thuốc điện tử hoàn tất");
                }
                //else
                //{
                //    GlobalsNAV.ShowMessagePopup("Có lỗi phát sinh trong quá trình tạo DTDT." + Environment.NewLine + "Vui lòng liên hệ phòng phần mềm để kiểm tra");
                //}
            }
            else
            {
                GlobalsNAV.ShowMessagePopup("Có lỗi phát sinh trong quá trình tạo DTDT." + Environment.NewLine +  "Vui lòng liên hệ phòng phần mềm để kiểm tra");
            }

            this.HideBusyIndicator();
        }

        private void CreateDTDTReportOutInPtFileTask(GenericCoRoutineTask genTask, object aRegistrationIDList)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        DTDTReport mDTDTReport = new DTDTReport
                        {
                            RegistrationIDList = aRegistrationIDList.ToString(),
                            V_HIReportType = new Lookup { LookupID = (long)AllLookupValues.V_HIReportType.REGID },
                            V_ReportStatus = (long)AllLookupValues.V_ReportStatus.Pending,
                            Staff = new Staff
                            {
                                StaffID = Globals.LoggedUserAccount.Staff.StaffID,
                                FullName = Globals.LoggedUserAccount.Staff.FullName
                            }
                        };
                        OutputErrorMessage = "";
                        contract.BeginCreateDTDTReportOutInPt(mDTDTReport
                            , Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    long mDTDTReportID;
                                    long mDTDTReportOutPt;
                                    var mResultVal = contract.EndCreateDTDTReportOutInPt(out mDTDTReportID, out mDTDTReportOutPt, asyncResult);
                                    if (mResultVal || mDTDTReportID > 0 || mDTDTReportOutPt > 0)
                                    {
                                        if (mDTDTReportID > 0)
                                        {
                                            mDTDTReport.DTDTReportID = mDTDTReportID;
                                        }
                                        if (mDTDTReportOutPt > 0)
                                        {
                                            mDTDTReport.DTDTReportOutPt = mDTDTReportOutPt;
                                        }
                                        genTask.AddResultObj(mDTDTReport);
                                        genTask.ActionComplete(true);
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                        OutputErrorMessage += Environment.NewLine + eHCMSResources.Z2334_G1_KhongTaoDuocBaoCao;
                                        ClientLoggerHelper.LogInfo(string.Format("CreateDTDTReportFileTask Try => {0}", OutputErrorMessage));
                                        genTask.ActionComplete(false);
                                        this.HideBusyIndicator();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    OutputErrorMessage = ex.Message;
                                    ClientLoggerHelper.LogInfo(string.Format("CreateDTDTReportFileTask Catch => {0}", OutputErrorMessage));
                                    genTask.ActionComplete(false);
                                    this.HideBusyIndicator();
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private string gAPIDTDTDangNhapUserBsiParams = "{{\"ma_lien_thong_bac_si\":\"{0}\",\"password\":\"{1}\",\"ma_lien_thong_co_so_kham_chua_benh\":\"{2}\"}}";
        private DTDTAPIResponse _DTDTAPIResponse;
        public DTDTAPIResponse DTDTAPIResponse
        {
            get => _DTDTAPIResponse; set
            {
                _DTDTAPIResponse = value;
                NotifyOfPropertyChange(() => DTDTAPIResponse);
            }
        }

        private void CreateDTDTReportAndCalLAPITask(GenericCoRoutineTask genTask, object aDTDTReport)
        {
            DTDTAPIResponse DTDTAPIResponseSendDonThuoc = new DTDTAPIResponse();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetDTDTData((aDTDTReport as DTDTReport).DTDTReportID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    List<DTDT_don_thuoc> DTDTReport = contract.EndGetDTDTData(asyncResult);
                                    List<long> ListPrescriptionErrorWhenCallAPI = new List<long>();
                                    if (DTDTReport != null && DTDTReport.ToObservableCollection().Count() > 0)
                                    {
                                        foreach (DTDT_don_thuoc item in DTDTReport.ToObservableCollection())
                                        {
                                            TimeSpan timeout = new TimeSpan(0, 1, 30);
                                            string Password = EncryptExtension.Decrypt(item.Doctor.MatKhauLienThongBacSi, Globals.AxonKey, Globals.AxonPass);
                                            string mJsonDataAuthor = string.Format(gAPIDTDTDangNhapUserBsiParams
                                                , item.Doctor.MaLienThongBacSi, Password, Globals.ServerConfigSection.CommonItems.DTDTUsername);
                                            var BearerAccessTokenRes = GlobalsNAV.RequestPOST(Globals.ServerConfigSection.CommonItems.DonThuocQuocGiaAPIUrl, "/api/auth/dang-nhap-bac-si"
                                                , mJsonDataAuthor, timeout);
                                            if (BearerAccessTokenRes == null)
                                            {
                                                MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                                OutputErrorMessage += Environment.NewLine + item.ma_dinh_danh_y_te + " - " + "BadGateway";
                                            }

                                            if (BearerAccessTokenRes != null && BearerAccessTokenRes != HttpStatusCode.NotFound.ToString())
                                            {
                                                if (DTDTAPIResponse != null)
                                                {
                                                    DTDTAPIResponse = null;
                                                }
                                                DTDTAPIResponse = GlobalsNAV.ConvertJsonToObject<DTDTAPIResponse>(BearerAccessTokenRes);
                                                if (!string.IsNullOrEmpty(DTDTAPIResponse.token))
                                                {
                                                    try
                                                    {
                                                        var resultResponse = GlobalsNAV.RequestPOST(Globals.ServerConfigSection.CommonItems.DonThuocQuocGiaAPIUrl, "/api/v1/gui-don-thuoc"
                                                            , GlobalsNAV.ConvertObjectToJson(item), timeout, DTDTAPIResponse.token);
                                                        if (resultResponse == null)
                                                        {
                                                            OutputErrorMessage += Environment.NewLine + item.ma_dinh_danh_y_te + " - " + "BadGateway";
                                                            ListPrescriptionErrorWhenCallAPI.Add(item.IssueID);
                                                        }
                                                        else
                                                        {
                                                            DTDTAPIResponseSendDonThuoc = GlobalsNAV.ConvertJsonToObject<DTDTAPIResponse>(resultResponse);
                                                            if (!string.IsNullOrEmpty(DTDTAPIResponseSendDonThuoc.error))
                                                            {
                                                                OutputErrorMessage += Environment.NewLine + item.ma_dinh_danh_y_te + " - " + DTDTAPIResponseSendDonThuoc.error;
                                                                ListPrescriptionErrorWhenCallAPI.Add(item.IssueID);
                                                            }
                                                            else
                                                            {
                                                                GlobalsNAV.ShowMessagePopup(DTDTAPIResponseSendDonThuoc.success);
                                                            }
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        OutputErrorMessage += Environment.NewLine + item.ma_dinh_danh_y_te + " - " + ex.Message;
                                                        ListPrescriptionErrorWhenCallAPI.Add(item.IssueID);
                                                    }
                                                }
                                                else
                                                {
                                                    OutputErrorMessage += Environment.NewLine + item.ma_dinh_danh_y_te + " - " + "Không xác định. Liên hệ IT!";
                                                    ListPrescriptionErrorWhenCallAPI.Add(item.IssueID);
                                                }
                                            }
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(OutputErrorMessage))
                                    {
                                        try
                                        {
                                            OutputErrorMessage = System.Text.RegularExpressions.Regex.Unescape(OutputErrorMessage);
                                            MessageBox.Show("Đẩy dữ liệu đơn thuốc điện tử: " + Environment.NewLine + OutputErrorMessage, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);

                                        }
                                        catch (Exception ex)
                                        {
                                            ClientLoggerHelper.LogInfo(string.Format("CreateDTDTReportXmlTask RegularExpressions.Unescape.Catch => {0}", ex.Message));
                                            MessageBox.Show("Đẩy dữ liệu đơn thuốc điện tử: " + Environment.NewLine + OutputErrorMessage, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                        }
                                    }
                                    DTDTAPIResponseSendDonThuoc.ListPrescriptionErrorWhenCallAPI = ListPrescriptionErrorWhenCallAPI;
                                    DTDTAPIResponseSendDonThuoc.IsTransferCompleted = true;
                                    genTask.AddResultObj(DTDTAPIResponseSendDonThuoc);
                                    genTask.ActionComplete(true);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(System.Text.RegularExpressions.Regex.Unescape(ex.Message), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    ClientLoggerHelper.LogInfo(string.Format("CreateDTDTReportXmlTask Catch => {0}", ex.Message));
                                    this.HideBusyIndicator();
                                    DTDTAPIResponseSendDonThuoc.IsTransferCompleted = true;
                                    genTask.AddResultObj(DTDTAPIResponseSendDonThuoc);
                                    genTask.ActionComplete(true);
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                    DTDTAPIResponseSendDonThuoc.IsTransferCompleted = false;
                    genTask.AddResultObj(DTDTAPIResponseSendDonThuoc);
                    genTask.ActionComplete(true);
                }
            });

            t.Start();
        }
        private void CreateDTDTReport_InPtAndCalLAPITask(GenericCoRoutineTask genTask, object aDTDTReport)
        {
            DTDTAPIResponse DTDTAPIResponseSendDonThuoc = new DTDTAPIResponse();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetDTDT_InPtData((aDTDTReport as DTDTReport).DTDTReportID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    List<DTDT_don_thuoc> DTDTReport = contract.EndGetDTDT_InPtData(asyncResult);
                                    List<long> ListPrescriptionErrorWhenCallAPI = new List<long>();
                                    if (DTDTReport != null && DTDTReport.ToObservableCollection().Count() > 0)
                                    {
                                        foreach (DTDT_don_thuoc item in DTDTReport.ToObservableCollection())
                                        {
                                            TimeSpan timeout = new TimeSpan(0, 1, 30);
                                            string Password = EncryptExtension.Decrypt(item.Doctor.MatKhauLienThongBacSi, Globals.AxonKey, Globals.AxonPass);
                                            string mJsonDataAuthor = string.Format(gAPIDTDTDangNhapUserBsiParams
                                                , item.Doctor.MaLienThongBacSi, Password, Globals.ServerConfigSection.CommonItems.DTDTUsername);
                                            var BearerAccessTokenRes = GlobalsNAV.RequestPOST(Globals.ServerConfigSection.CommonItems.DonThuocQuocGiaAPIUrl, "/api/auth/dang-nhap-bac-si"
                                                , mJsonDataAuthor, timeout);
                                            if (BearerAccessTokenRes == null)
                                            {
                                                MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                                OutputErrorMessage += Environment.NewLine + item.ma_dinh_danh_y_te + " - " + "BadGateway";
                                            }

                                            if (BearerAccessTokenRes != null && BearerAccessTokenRes != HttpStatusCode.NotFound.ToString())
                                            {
                                                if (DTDTAPIResponse != null)
                                                {
                                                    DTDTAPIResponse = null;
                                                }
                                                DTDTAPIResponse = GlobalsNAV.ConvertJsonToObject<DTDTAPIResponse>(BearerAccessTokenRes);
                                                if (!string.IsNullOrEmpty(DTDTAPIResponse.token))
                                                {
                                                    try
                                                    {
                                                        var resultResponse = GlobalsNAV.RequestPOST(Globals.ServerConfigSection.CommonItems.DonThuocQuocGiaAPIUrl, "/api/v1/gui-don-thuoc"
                                                            , GlobalsNAV.ConvertObjectToJson(item), timeout, DTDTAPIResponse.token);
                                                        if (resultResponse == null)
                                                        {
                                                            OutputErrorMessage += Environment.NewLine + item.ma_dinh_danh_y_te + " - " + "BadGateway";
                                                            ListPrescriptionErrorWhenCallAPI.Add(item.IssueID);
                                                        }
                                                        else
                                                        {
                                                            DTDTAPIResponseSendDonThuoc = GlobalsNAV.ConvertJsonToObject<DTDTAPIResponse>(resultResponse);
                                                            if (!string.IsNullOrEmpty(DTDTAPIResponseSendDonThuoc.error))
                                                            {
                                                                OutputErrorMessage += Environment.NewLine + item.ma_dinh_danh_y_te + " - " + DTDTAPIResponseSendDonThuoc.error;
                                                                ListPrescriptionErrorWhenCallAPI.Add(item.IssueID);
                                                            }
                                                            else
                                                            {
                                                                GlobalsNAV.ShowMessagePopup(DTDTAPIResponseSendDonThuoc.success);
                                                            }
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        OutputErrorMessage += Environment.NewLine + item.ma_dinh_danh_y_te + " - " + ex.Message;
                                                        ListPrescriptionErrorWhenCallAPI.Add(item.IssueID);
                                                    }
                                                }
                                                else
                                                {
                                                    OutputErrorMessage += Environment.NewLine + item.ma_dinh_danh_y_te + " - " + "Không xác định. Liên hệ IT!";
                                                    ListPrescriptionErrorWhenCallAPI.Add(item.IssueID);
                                                }
                                            }
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(OutputErrorMessage))
                                    {
                                        try
                                        {
                                            OutputErrorMessage = System.Text.RegularExpressions.Regex.Unescape(OutputErrorMessage);
                                            MessageBox.Show("Đẩy dữ liệu đơn thuốc điện tử: " + Environment.NewLine + OutputErrorMessage, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);

                                        }
                                        catch (Exception ex)
                                        {
                                            ClientLoggerHelper.LogInfo(string.Format("CreateDTDTReportXmlTask RegularExpressions.Unescape.Catch => {0}", ex.Message));
                                            MessageBox.Show("Đẩy dữ liệu đơn thuốc điện tử: " + Environment.NewLine + OutputErrorMessage, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                        }
                                    }
                                    DTDTAPIResponseSendDonThuoc.ListPrescriptionErrorWhenCallAPI = ListPrescriptionErrorWhenCallAPI;
                                    DTDTAPIResponseSendDonThuoc.IsTransferCompleted = true;
                                    genTask.AddResultObj(DTDTAPIResponseSendDonThuoc);
                                    genTask.ActionComplete(true);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(System.Text.RegularExpressions.Regex.Unescape(ex.Message), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    ClientLoggerHelper.LogInfo(string.Format("CreateDTDTReportXmlTask Catch => {0}", ex.Message));
                                    this.HideBusyIndicator();
                                    DTDTAPIResponseSendDonThuoc.IsTransferCompleted = true;
                                    genTask.AddResultObj(DTDTAPIResponseSendDonThuoc);
                                    genTask.ActionComplete(true);
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                    DTDTAPIResponseSendDonThuoc.IsTransferCompleted = false;
                    genTask.AddResultObj(DTDTAPIResponseSendDonThuoc);
                    genTask.ActionComplete(true);
                }
            });

            t.Start();
        }

        private void UpdateDTDTReportTask(GenericCoRoutineTask genTask, object aDTDTReport)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdateDTDTReportStatus(aDTDTReport as DTDTReport
                            , Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    if (contract.EndUpdateDTDTReportStatus(asyncResult))
                                    {
                                        genTask.ActionComplete(true);
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                        genTask.ActionComplete(false);
                                        this.HideBusyIndicator();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    genTask.ActionComplete(false);
                                    this.HideBusyIndicator();
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }
        //▲====: #019
        //▼====: #020
        private void DeleteRegistrationDTDTReportTask(GenericCoRoutineTask genTask, object ListPrescriptionErrorWhenCallAPI)
        {
            this.ShowBusyIndicator("Xóa danh sách đăng ký bị lỗi");
            var t = new Thread(() =>
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<Root>");
                    foreach (long item in ListPrescriptionErrorWhenCallAPI as List<long>)
                    {
                        sb.Append("<Item>");
                        sb.AppendFormat("<IssueID>{0}</IssueID>", item);
                        sb.Append("</Item>");
                    }
                    sb.Append("</Root>");

                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeleteRegistrationDTDTReport(sb.ToString()
                            , Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    if (contract.EndDeleteRegistrationDTDTReport(asyncResult))
                                    {
                                        genTask.ActionComplete(true);
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                        genTask.ActionComplete(true);
                                        this.HideBusyIndicator();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    genTask.ActionComplete(true);
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
                    genTask.ActionComplete(true);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void DeleteDTDTReportTask(GenericCoRoutineTask genTask, object DTDTReportID)
        {
            this.ShowBusyIndicator("Xóa dữ liệu đợt đẩy bị lỗi");
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeleteDTDTReport((long)DTDTReportID
                            , Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    if (contract.EndDeleteDTDTReport(asyncResult))
                                    {
                                        genTask.ActionComplete(true);
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                        genTask.ActionComplete(true);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    genTask.ActionComplete(true);
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
                    genTask.ActionComplete(true);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }
        private void DeleteRegistrationDTDT_InPtReportTask(GenericCoRoutineTask genTask, object ListPrescriptionErrorWhenCallAPI)
        {
            this.ShowBusyIndicator("Xóa danh sách đăng ký bị lỗi");
            var t = new Thread(() =>
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<Root>");
                    foreach (long item in ListPrescriptionErrorWhenCallAPI as List<long>)
                    {
                        sb.Append("<Item>");
                        sb.AppendFormat("<IssueID>{0}</IssueID>", item);
                        sb.Append("</Item>");
                    }
                    sb.Append("</Root>");

                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeleteRegistrationDTDT_InPtReport(sb.ToString()
                            , Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    if (contract.EndDeleteRegistrationDTDT_InPtReport(asyncResult))
                                    {
                                        genTask.ActionComplete(true);
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                        genTask.ActionComplete(true);
                                        this.HideBusyIndicator();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    genTask.ActionComplete(true);
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
                    genTask.ActionComplete(true);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void DeleteDTDTReport_InPtTask(GenericCoRoutineTask genTask, object DTDTReportID)
        {
            this.ShowBusyIndicator("Xóa dữ liệu đợt đẩy bị lỗi");
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeleteDTDT_InPtReport((long)DTDTReportID
                            , Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    if (contract.EndDeleteDTDT_InPtReport(asyncResult))
                                    {
                                        genTask.ActionComplete(true);
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                        genTask.ActionComplete(true);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    genTask.ActionComplete(true);
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
                    genTask.ActionComplete(true);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }
    }
}
