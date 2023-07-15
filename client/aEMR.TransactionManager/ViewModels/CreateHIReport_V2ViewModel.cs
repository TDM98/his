using aEMR.Common;
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
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
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using System.Xml.Serialization;
/*
* 20190918 #001 TTM:   BM 0013320: Bổ sung thêm bộ lọc tìm kiếm ca xác nhận BHYT theo loại bệnh nhân.
* 20200404 #002 TTM:   BM 0029083: Thay đổi thời gian tìm kiếm để lưu vào bảng HIReport. Trước giờ toàn lưu Datetime.Now cho cả ngày từ và đến.
* 20221210 #003 TNHX: 994 Đẩy dữ liệu đơn thuốc điện tử
* 20230314 #004 BLQ: Thêm hàm lấy dữ liệu đẩy cổng theo 130
*/
namespace aEMR.TransactionManager.ViewModels
{
    [Export(typeof(ICreateHIReport_V2)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CreateHIReport_V2ViewModel : ViewModelBase, ICreateHIReport_V2
    {
        [ImportingConstructor]
        public CreateHIReport_V2ViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            base.HasInputBindingCmd = true;
            Coroutine.BeginExecute(LoadDepartments());
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
        private ObservableCollection<PatientRegistration> _PatientRegistrationCollection;
        private ObservableCollection<PatientRegistration> _OtherTypePatientRegistrationCollection;
        private string gIAPISendHIReportAddress = "http://egw.baohiemxahoi.gov.vn/";
        private string gIAPISendHIReportAddressParams = "api/egw/guiHoSoGiamDinh4210?token={0}&id_token={1}&username={2}&password={3}&loaiHoSo=3&maTinh={4}&maCSKCB={5}";
        private string _OutputErrorMessage;
        private bool _AllChecked;
        private bool _IsAllHIReportID = false;
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
        public bool IsAllHIReportID
        {
            get => _IsAllHIReportID; set
            {
                _IsAllHIReportID = value;
                NotifyOfPropertyChange(() => IsAllHIReportID);
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
        private void CreateHIReportFileTask(GenericCoRoutineTask genTask, object aRegistrationIDList)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        HealthInsuranceReport mHealthInsuranceReport = new HealthInsuranceReport { Title = string.Format("BC-{0}", aRegistrationIDList.ToString()), RegistrationIDList = aRegistrationIDList.ToString(), V_HIReportType = new Lookup { LookupID = (long)AllLookupValues.V_HIReportType.REGID }, V_ReportStatus = (long)AllLookupValues.V_ReportStatus.Pending, FromDate = FromDate, ToDate = ToDate };
                        contract.BeginCreateHIReport_V2(mHealthInsuranceReport,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    long mHIReportID;
                                    var mResultVal = contract.EndCreateHIReport_V2(out mHIReportID, asyncResult);
                                    if (!mResultVal || mHIReportID == 0)
                                    {
                                        MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                        OutputErrorMessage += Environment.NewLine + eHCMSResources.Z2334_G1_KhongTaoDuocBaoCao;
                                        ClientLoggerHelper.LogInfo(string.Format("CreateHIReportFileTask Try => {0}", OutputErrorMessage));
                                        genTask.ActionComplete(false);
                                        this.HideBusyIndicator();
                                    }
                                    else
                                    {
                                        mHealthInsuranceReport.HIReportID = mHIReportID;
                                        genTask.AddResultObj(mHealthInsuranceReport);
                                        genTask.ActionComplete(true);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    OutputErrorMessage = ex.Message;
                                    ClientLoggerHelper.LogInfo(string.Format("CreateHIReportFileTask Catch => {0}", OutputErrorMessage));
                                    //MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
        private void CreateHIReportXmlTask(GenericCoRoutineTask genTask, object aHealthInsuranceReport)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetHIXmlReport9324_AllTab123_InOneRpt((aHealthInsuranceReport as HealthInsuranceReport).HIReportID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var ReportStream = contract.EndGetHIXmlReport9324_AllTab123_InOneRpt(asyncResult);
                                    string mHIAPICheckHICardAddress = string.Format(gIAPISendHIReportAddressParams, GlobalsNAV.gLoggedHIAPIUser.APIKey.access_token, GlobalsNAV.gLoggedHIAPIUser.APIKey.id_token, Globals.ServerConfigSection.Hospitals.HIAPILoginAccount, GlobalsNAV.gLoggedHIAPIUser.password, Globals.ServerConfigSection.Hospitals.HospitalCode.Length < 2 ? "" : Globals.ServerConfigSection.Hospitals.HospitalCode.Substring(0, 2), Globals.ServerConfigSection.Hospitals.HospitalCode);
                                    string mRestJson = GlobalsNAV.GetRESTServiceJSon(gIAPISendHIReportAddress, mHIAPICheckHICardAddress, ReportStream);
                                    HIAPIUploadHIReportXmlResult mHIAPIUploadHIReportXmlResult = GlobalsNAV.ConvertJsonToObject<HIAPIUploadHIReportXmlResult>(mRestJson);
                                    if (mHIAPIUploadHIReportXmlResult.maKetQua == 200)
                                    {
                                        genTask.AddResultObj(mHIAPIUploadHIReportXmlResult);
                                        genTask.ActionComplete(true);
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                        var mErrorMessage = string.IsNullOrEmpty(mHIAPIUploadHIReportXmlResult.maGiaoDich) ? GetErrorMessageFromErrorCode(mHIAPIUploadHIReportXmlResult.maKetQua) : mHIAPIUploadHIReportXmlResult.maGiaoDich;
                                        if (!string.IsNullOrEmpty(mErrorMessage))
                                        {
                                            mErrorMessage = string.Format(" - {0}", mErrorMessage);
                                        }
                                        OutputErrorMessage += Environment.NewLine + string.Format("{0}: {1}{2}", eHCMSResources.T0074_G1_I, mHIAPIUploadHIReportXmlResult.maKetQua, mErrorMessage);
                                        ClientLoggerHelper.LogInfo(string.Format("CreateHIReportXmlTask Else => {0}", OutputErrorMessage));
                                        genTask.ActionComplete(false);
                                        this.HideBusyIndicator();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    ClientLoggerHelper.LogInfo(string.Format("CreateHIReportXmlTask Catch => {0}", ex.Message));
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
        private void UpdateHIReportTask(GenericCoRoutineTask genTask, object aHealthInsuranceReport)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdateHIReportStatus(aHealthInsuranceReport as HealthInsuranceReport,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    if (contract.EndUpdateHIReportStatus(asyncResult))
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
        private IEnumerator<IResult> CreateHIReportXml_Routine()
        {
            if (PatientRegistrationCollection == null || !PatientRegistrationCollection.Any(x => x.IsSelected && x.HIReportID == 0)) yield break;

            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);

            string mTitle = string.Join(",", PatientRegistrationCollection.Where(x => x.IsSelected && x.HIReportID == 0).Select(x => string.Format("{0}-{1}", x.RegistrationType.RegTypeID, x.PtRegistrationID)).ToArray());

            var mCreateHIReportFileTask = new GenericCoRoutineTask(CreateHIReportFileTask, mTitle);
            yield return mCreateHIReportFileTask;

            HealthInsuranceReport mHealthInsuranceReport = mCreateHIReportFileTask.GetResultObj(0) as HealthInsuranceReport;

            var mCreateHIReportXmlTask = new GenericCoRoutineTask(CreateHIReportXmlTask, mHealthInsuranceReport);
            yield return mCreateHIReportXmlTask;

            HIAPIUploadHIReportXmlResult mHIAPIUploadHIReportXmlResult = mCreateHIReportXmlTask.GetResultObj(0) as HIAPIUploadHIReportXmlResult;
            mHealthInsuranceReport.V_ReportStatus = (long)AllLookupValues.V_ReportStatus.Completed;
            mHealthInsuranceReport.ReportAppliedCode = mHIAPIUploadHIReportXmlResult.maGiaoDich;
            mHealthInsuranceReport.ReportAppliedResultCode = mHIAPIUploadHIReportXmlResult.maKetQua;

            var mUpdateHIReportTask = new GenericCoRoutineTask(UpdateHIReportTask, mHealthInsuranceReport);
            yield return mUpdateHIReportTask;

            foreach (var item in PatientRegistrationCollection.Where(x => x.IsSelected && x.HIReportID == 0))
            {
                item.HIReportID = mHealthInsuranceReport.HIReportID;
                item.ReportAppliedCode = mHealthInsuranceReport.ReportAppliedCode;
                item.V_ReportStatus.LookupID = mHealthInsuranceReport.V_ReportStatus;
                item.V_ReportStatus.ObjectValue = eHCMSResources.Z2333_G1_XacNhanBCHoanTat;
            }

            MessageBox.Show(eHCMSResources.K0461_G1_XNhanBHOk, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);

            this.HideBusyIndicator();
        }
        private string GetErrorMessageFromErrorCode(int aErrorCode)
        {
            switch (aErrorCode)
            {
                case 1001:
                    return "Kích thước file quá lớn (20MB trong khoảng thời gian từ 8 giờ đến 11 giờ và từ 14 giờ đến 19 giờ từ thứ 2 đến thứ 6,100MB với các thời gian khác)";
                case 205:
                    return "Lỗi sai định dạng tham số";
                case 401:
                    return "Sai tài khoản hoặc mật khẩu";
                case 123:
                    return "Sai định dạng file";
                case 124:
                    return "Lỗi khi lưu dữ liệu( file sẽ được tự động gửi lại)";
                case 701:
                    return "Lỗi thời gian gửi,thời gian quyết toán chỉ dc trong tháng hoặc đến mồng 5 tháng sau";
                case 500:
                    return "Lỗi trong quá trình gửi dữ liệu.Vui lòng liên hệ với nhân viên hỗ trợ để được hướng dẫn cụ thể";
                default:
                    return "";
            }
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
            }
            else if (StatusIndex == 1)
            {
                SearchCriteria.IsReported = true;
            }
            else if (StatusIndex == 2)
            {
                SearchCriteria.IsReported = false;
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
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
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
                                    if (PatientTypeIndex != 3)
                                    {
                                        PatientRegistrationCollection = contract.EndSearchRegistrationsForHIReport(asyncResult).ToObservableCollection();
                                        OtherTypePatientRegistrationCollection = new ObservableCollection<PatientRegistration>();
                                    }
                                    else
                                    {
                                        var retval = contract.EndSearchRegistrationsForHIReport(asyncResult).ToObservableCollection();
                                        PatientRegistrationCollection = retval.Where(x => x.IsSumTreatmentProgram).ToObservableCollection();
                                        OtherTypePatientRegistrationCollection = retval.Where(x => !x.IsSumTreatmentProgram).ToObservableCollection();
                                    }

                                    if (PatientRegistrationCollection != null && PatientRegistrationCollection.Any(x => x.HIReportID > 0))
                                    {
                                        IsAllHIReportID = true;
                                    }
                                    else
                                    {
                                        IsAllHIReportID = false;
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
            if ((GlobalsNAV.gLoggedHIAPIUser == null || GlobalsNAV.gLoggedHIAPIUser.APIKey == null || string.IsNullOrEmpty(GlobalsNAV.gLoggedHIAPIUser.APIKey.access_token))
                || (GlobalsNAV.gLoggedHIAPIUser != null && GlobalsNAV.gLoggedHIAPIUser.APIKey != null && GlobalsNAV.gLoggedHIAPIUser.APIKey.expires_in <= DateTime.Now))
            {
                GlobalsNAV.LoginHIAPI();
            }
            //▼====: #004
            if (Globals.ServerConfigSection.CommonItems.ApplyReport130)
            {
                Coroutine.BeginExecute(CreateHIReport_130_OutInPtXml_Routine());
            }
            //▲====: #004
            else if (!Globals.ServerConfigSection.CommonItems.NewMethodToReport4210)
            {
                Coroutine.BeginExecute(CreateHIReportXml_Routine());
            }
            else
            {
                Coroutine.BeginExecute(CreateHIReportOutInPtXml_Routine());
            }
            //▼====: #003
            if (Globals.ServerConfigSection.CommonItems.ApplyDTDT)
            {
                Coroutine.BeginExecute(CreateDTDT_Routine());
            }
            //▲====: #003
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

        public void btnPreviewHIReport(object source)
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
                using (var serviceFactory = new TransactionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPreviewHIReport(mPatientRegistration.PtRegistrationID, (long)mPatientRegistration.V_RegistrationType, mPatientRegistration.OutPtTreatmentProgramID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string ErrText;
                            DataSet mPreviewHIReportSet = contract.EndPreviewHIReport(out ErrText, asyncResult);
                            if (Globals.ServerConfigSection.CommonItems.ApplyReport130)
                            {
                                GlobalsNAV.ShowDialog<IPreviewHIReport130>((mView) =>
                                {
                                    mView.ApplyPreviewHIReportSet(mPreviewHIReportSet, ErrText);
                                }, null, false, true, Globals.GetDefaultDialogViewSize());
                            }
                            else
                            {
                                GlobalsNAV.ShowDialog<IPreviewHIReport>((mView) =>
                                {
                                    mView.ApplyPreviewHIReportSet(mPreviewHIReportSet, ErrText);
                                }, null, false, true, Globals.GetDefaultDialogViewSize());
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

        public void btnDeleteHIReport(object source)
        {
            if (source == null || !(source is PatientRegistration))
            {
                return;
            }
            PatientRegistration mPatientRegistration = source as PatientRegistration;
            if (mPatientRegistration == null || mPatientRegistration.PtRegistrationID == 0 || mPatientRegistration.HIReportID == 0)
            {
                return;
            }
            string ma_lk = (mPatientRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU ? "1-" : "3-") + mPatientRegistration.PtRegistrationID;
            if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, string.Format("{0} {1}", eHCMSResources.K1048_G1_BC, ma_lk)), eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeleteRegistrationHIReport(ma_lk, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                if (contract.EndDeleteRegistrationHIReport(asyncResult))
                                {
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
        }

        public void gvRegistrations_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!(sender is DataGrid) && (sender as DataGrid).SelectedItem == null && !((sender as DataGrid).SelectedItem is PatientRegistration))
            {
                return;
            }
            btnPreviewHIReport((sender as DataGrid).SelectedItem as PatientRegistration);
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
        private IEnumerator<IResult> CreateHIReportOutInPtXml_Routine()
        {
            if (PatientRegistrationCollection == null || !PatientRegistrationCollection.Any(x => x.IsSelected && x.HIReportID == 0)) yield break;

            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);

            string mTitle = string.Join(",", PatientRegistrationCollection.Where(x => x.IsSelected && x.HIReportID == 0).Select(x => string.Format("{0}-{1}", x.RegistrationType.RegTypeID, x.PtRegistrationID)).ToArray());

            var mCreateHIReportFileTask = new GenericCoRoutineTask(CreateHIReportOutInPtFileTask, mTitle);
            yield return mCreateHIReportFileTask;

            HealthInsuranceReport mHealthInsuranceReport = mCreateHIReportFileTask.GetResultObj(0) as HealthInsuranceReport;

            if (mHealthInsuranceReport != null)
            {
                if (mHealthInsuranceReport.HIReportID > 0)
                {
                    var mCreateHIReportXmlTask = new GenericCoRoutineTask(CreateHIReportXmlTask, mHealthInsuranceReport);
                    yield return mCreateHIReportXmlTask;

                    HIAPIUploadHIReportXmlResult mHIAPIUploadHIReportXmlResult = mCreateHIReportXmlTask.GetResultObj(0) as HIAPIUploadHIReportXmlResult;
                    mHealthInsuranceReport.V_ReportStatus = (long)AllLookupValues.V_ReportStatus.Completed;
                    mHealthInsuranceReport.ReportAppliedCode = mHIAPIUploadHIReportXmlResult.maGiaoDich;
                    mHealthInsuranceReport.ReportAppliedResultCode = mHIAPIUploadHIReportXmlResult.maKetQua;

                    var mUpdateHIReportTask = new GenericCoRoutineTask(UpdateHIReportTask, mHealthInsuranceReport);
                    yield return mUpdateHIReportTask;

                    foreach (var item in PatientRegistrationCollection.Where(x => x.IsSelected && x.HIReportID == 0 && x.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU))
                    {
                        item.HIReportID = mHealthInsuranceReport.HIReportID;
                        item.ReportAppliedCode = mHealthInsuranceReport.ReportAppliedCode;
                        item.V_ReportStatus.LookupID = mHealthInsuranceReport.V_ReportStatus;
                        item.V_ReportStatus.ObjectValue = eHCMSResources.Z2333_G1_XacNhanBCHoanTat;
                    }
                }
                if (mHealthInsuranceReport.HIReportOutPt > 0)
                {
                    mHealthInsuranceReport.HIReportID = mHealthInsuranceReport.HIReportOutPt;
                    var mCreateHIReportXmlTask = new GenericCoRoutineTask(CreateHIReportXmlTask, mHealthInsuranceReport);
                    yield return mCreateHIReportXmlTask;

                    HIAPIUploadHIReportXmlResult mHIAPIUploadHIReportXmlResult = mCreateHIReportXmlTask.GetResultObj(0) as HIAPIUploadHIReportXmlResult;
                    mHealthInsuranceReport.V_ReportStatus = (long)AllLookupValues.V_ReportStatus.Completed;
                    mHealthInsuranceReport.ReportAppliedCode = mHIAPIUploadHIReportXmlResult.maGiaoDich;
                    mHealthInsuranceReport.ReportAppliedResultCode = mHIAPIUploadHIReportXmlResult.maKetQua;

                    var mUpdateHIReportTask = new GenericCoRoutineTask(UpdateHIReportTask, mHealthInsuranceReport);
                    yield return mUpdateHIReportTask;

                    foreach (var item in PatientRegistrationCollection.Where(x => x.IsSelected && x.HIReportID == 0 && x.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU))
                    {
                        item.HIReportID = mHealthInsuranceReport.HIReportID;
                        item.ReportAppliedCode = mHealthInsuranceReport.ReportAppliedCode;
                        item.V_ReportStatus.LookupID = mHealthInsuranceReport.V_ReportStatus;
                        item.V_ReportStatus.ObjectValue = eHCMSResources.Z2333_G1_XacNhanBCHoanTat;
                    }
                }
            }
            MessageBox.Show(eHCMSResources.K0461_G1_XNhanBHOk, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);

            this.HideBusyIndicator();
        }

        private void CreateHIReportOutInPtFileTask(GenericCoRoutineTask genTask, object aRegistrationIDList)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        HealthInsuranceReport mHealthInsuranceReport = new HealthInsuranceReport
                        {
                            Title = string.Format("BC-{0}", aRegistrationIDList.ToString()),
                            RegistrationIDList = aRegistrationIDList.ToString(),
                            V_HIReportType = new Lookup { LookupID = (long)AllLookupValues.V_HIReportType.REGID },
                            V_ReportStatus = (long)AllLookupValues.V_ReportStatus.Pending,
                            FromDate = FromDate,
                            ToDate = ToDate,
                            Staff = new Staff
                            {
                                StaffID = Globals.LoggedUserAccount.Staff.StaffID,
                                FullName = Globals.LoggedUserAccount.Staff.FullName
                            }
                        };
                        contract.BeginCreateHIReportOutInPt(mHealthInsuranceReport
                            , Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    long mHIReportID;
                                    long mHIReportOutPt;
                                    var mResultVal = contract.EndCreateHIReportOutInPt(out mHIReportID, out mHIReportOutPt, asyncResult);
                                    if (mResultVal || mHIReportID > 0 || mHIReportOutPt > 0)
                                    {
                                        if (mHIReportID > 0)
                                        {
                                            mHealthInsuranceReport.HIReportID = mHIReportID;
                                        }
                                        if (mHIReportOutPt > 0)
                                        {
                                            mHealthInsuranceReport.HIReportOutPt = mHIReportOutPt;
                                        }
                                        genTask.AddResultObj(mHealthInsuranceReport);
                                        genTask.ActionComplete(true);
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                        OutputErrorMessage += Environment.NewLine + eHCMSResources.Z2334_G1_KhongTaoDuocBaoCao;
                                        ClientLoggerHelper.LogInfo(string.Format("CreateHIReportFileTask Try => {0}", OutputErrorMessage));
                                        genTask.ActionComplete(false);
                                        this.HideBusyIndicator();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    OutputErrorMessage = ex.Message;
                                    ClientLoggerHelper.LogInfo(string.Format("CreateHIReportFileTask Catch => {0}", OutputErrorMessage));
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
        public void gvOtherRegistrations_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        //▼====: #003
        private IEnumerator<IResult> CreateDTDT_Routine()
        {
            if (PatientRegistrationCollection == null || !PatientRegistrationCollection.Any(x => x.IsSelected && x.DTDTReportID == 0)) yield break;

            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);

            string mTitle = string.Join(",", PatientRegistrationCollection.Where(x => x.IsSelected && x.DTDTReportID == 0).Select(x => string.Format("{0}-{1}", x.RegistrationType.RegTypeID, x.PtRegistrationID)).ToArray());

            var mCreateDTDTReportFileTask = new GenericCoRoutineTask(CreateDTDTReportOutInPtFileTask, mTitle);
            yield return mCreateDTDTReportFileTask;

            DTDTReport mDTDTReport = mCreateDTDTReportFileTask.GetResultObj(0) as DTDTReport;

            if (mDTDTReport != null)
            {
                //if (mDTDTReport.HIReportID > 0)
                //{
                //    var mCreateHIReportXmlTask = new GenericCoRoutineTask(CreateHIReportXmlTask, mDTDTReport);
                //    yield return mCreateHIReportXmlTask;

                //    HIAPIUploadHIReportXmlResult mHIAPIUploadHIReportXmlResult = mCreateHIReportXmlTask.GetResultObj(0) as HIAPIUploadHIReportXmlResult;
                //    mDTDTReport.V_ReportStatus = (long)AllLookupValues.V_ReportStatus.Completed;
                //    mDTDTReport.ReportAppliedCode = mHIAPIUploadHIReportXmlResult.maGiaoDich;
                //    mDTDTReport.ReportAppliedResultCode = mHIAPIUploadHIReportXmlResult.maKetQua;

                //    var mUpdateHIReportTask = new GenericCoRoutineTask(UpdateHIReportTask, mDTDTReport);
                //    yield return mUpdateHIReportTask;

                //    foreach (var item in PatientRegistrationCollection.Where(x => x.IsSelected && x.HIReportID == 0 && x.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU))
                //    {
                //        item.HIReportID = mDTDTReport.HIReportID;
                //        item.ReportAppliedCode = mDTDTReport.ReportAppliedCode;
                //        item.V_ReportStatus.LookupID = mDTDTReport.V_ReportStatus;
                //        item.V_ReportStatus.ObjectValue = eHCMSResources.Z2333_G1_XacNhanBCHoanTat;
                //    }
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
                        var mDeleteRegistrationDTDTReportTask = new GenericCoRoutineTask(DeleteRegistrationDTDTReportTask, DTDTAPIResponseSendDonThuocTemp.ListPrescriptionErrorWhenCallAPI);
                        yield return mDeleteRegistrationDTDTReportTask;
                    }

                    var mUpdateDTDTReportTask = new GenericCoRoutineTask(UpdateDTDTReportTask, mDTDTReport);
                    yield return mUpdateDTDTReportTask;

                    foreach (var item in PatientRegistrationCollection.Where(x => x.IsSelected && x.DTDTReportID == 0 && x.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU))
                    {
                        item.DTDTReportID = mDTDTReport.DTDTReportID;
                    }
                }
            }
            GlobalsNAV.ShowMessagePopup("Gửi dữ liệu đơn thuốc điện tử hoàn tất");

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

                                        }
                                        catch (Exception ex)
                                        {
                                            ClientLoggerHelper.LogInfo(string.Format("CreateDTDTReportXmlTask RegularExpressions.Unescape.Catch => {0}", ex.Message));
                                        }
                                    }

                                    DTDTAPIResponseSendDonThuoc.ListPrescriptionErrorWhenCallAPI = ListPrescriptionErrorWhenCallAPI;
                                    DTDTAPIResponseSendDonThuoc.IsTransferCompleted = true;
                                    genTask.AddResultObj(DTDTAPIResponseSendDonThuoc);
                                    genTask.ActionComplete(true);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

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
        //▲====: #003
        //▼====: #004
        private IEnumerator<IResult> CreateHIReport_130_OutInPtXml_Routine()
        {
            if (PatientRegistrationCollection == null || !PatientRegistrationCollection.Any(x => x.IsSelected && x.HIReportID == 0)) yield break;

            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);

            string mTitle = string.Join(",", PatientRegistrationCollection.Where(x => x.IsSelected && x.HIReportID == 0).Select(x => string.Format("{0}-{1}", x.RegistrationType.RegTypeID, x.PtRegistrationID)).ToArray());

            var mCreateHIReportFileTask = new GenericCoRoutineTask(CreateHIReport_130_OutInPtFileTask, mTitle);
            yield return mCreateHIReportFileTask;

            HealthInsuranceReport mHealthInsuranceReport = mCreateHIReportFileTask.GetResultObj(0) as HealthInsuranceReport;

            if (mHealthInsuranceReport != null)
            {
                if (mHealthInsuranceReport.HIReportID > 0)
                {
                    var mCreateHIReportXmlTask = new GenericCoRoutineTask(CreateHIReport_130_XmlTask, mHealthInsuranceReport);
                    yield return mCreateHIReportXmlTask;

                    HIAPIUploadHIReportXmlResult mHIAPIUploadHIReportXmlResult = mCreateHIReportXmlTask.GetResultObj(0) as HIAPIUploadHIReportXmlResult;
                    mHealthInsuranceReport.V_ReportStatus = (long)AllLookupValues.V_ReportStatus.Completed;
                    mHealthInsuranceReport.ReportAppliedCode = mHIAPIUploadHIReportXmlResult.maGiaoDich;
                    mHealthInsuranceReport.ReportAppliedResultCode = mHIAPIUploadHIReportXmlResult.maKetQua;

                    var mUpdateHIReportTask = new GenericCoRoutineTask(UpdateHIReportTask, mHealthInsuranceReport);
                    yield return mUpdateHIReportTask;

                    foreach (var item in PatientRegistrationCollection.Where(x => x.IsSelected && x.HIReportID == 0 && x.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU))
                    {
                        item.HIReportID = mHealthInsuranceReport.HIReportID;
                        item.ReportAppliedCode = mHealthInsuranceReport.ReportAppliedCode;
                        item.V_ReportStatus.LookupID = mHealthInsuranceReport.V_ReportStatus;
                        item.V_ReportStatus.ObjectValue = eHCMSResources.Z2333_G1_XacNhanBCHoanTat;
                    }
                }
                if (mHealthInsuranceReport.HIReportOutPt > 0)
                {
                    mHealthInsuranceReport.HIReportID = mHealthInsuranceReport.HIReportOutPt;
                    var mCreateHIReportXmlTask = new GenericCoRoutineTask(CreateHIReport_130_XmlTask, mHealthInsuranceReport);
                    yield return mCreateHIReportXmlTask;

                    HIAPIUploadHIReportXmlResult mHIAPIUploadHIReportXmlResult = mCreateHIReportXmlTask.GetResultObj(0) as HIAPIUploadHIReportXmlResult;
                    mHealthInsuranceReport.V_ReportStatus = (long)AllLookupValues.V_ReportStatus.Completed;
                    mHealthInsuranceReport.ReportAppliedCode = mHIAPIUploadHIReportXmlResult.maGiaoDich;
                    mHealthInsuranceReport.ReportAppliedResultCode = mHIAPIUploadHIReportXmlResult.maKetQua;

                    var mUpdateHIReportTask = new GenericCoRoutineTask(UpdateHIReportTask, mHealthInsuranceReport);
                    yield return mUpdateHIReportTask;

                    foreach (var item in PatientRegistrationCollection.Where(x => x.IsSelected && x.HIReportID == 0 && x.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU))
                    {
                        item.HIReportID = mHealthInsuranceReport.HIReportID;
                        item.ReportAppliedCode = mHealthInsuranceReport.ReportAppliedCode;
                        item.V_ReportStatus.LookupID = mHealthInsuranceReport.V_ReportStatus;
                        item.V_ReportStatus.ObjectValue = eHCMSResources.Z2333_G1_XacNhanBCHoanTat;
                    }
                }
            }
            MessageBox.Show(eHCMSResources.K0461_G1_XNhanBHOk, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);

            this.HideBusyIndicator();
        }
        private void CreateHIReport_130_OutInPtFileTask(GenericCoRoutineTask genTask, object aRegistrationIDList)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        HealthInsuranceReport mHealthInsuranceReport = new HealthInsuranceReport
                        {
                            Title = string.Format("BC-{0}", aRegistrationIDList.ToString()),
                            RegistrationIDList = aRegistrationIDList.ToString(),
                            V_HIReportType = new Lookup { LookupID = (long)AllLookupValues.V_HIReportType.REGID },
                            V_ReportStatus = (long)AllLookupValues.V_ReportStatus.Pending,
                            FromDate = FromDate,
                            ToDate = ToDate,
                            Staff = new Staff
                            {
                                StaffID = Globals.LoggedUserAccount.Staff.StaffID,
                                FullName = Globals.LoggedUserAccount.Staff.FullName
                            }
                        };
                        contract.BeginCreateHIReport_130_OutInPt(mHealthInsuranceReport
                            , Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    long mHIReportID;
                                    long mHIReportOutPt;
                                    var mResultVal = contract.EndCreateHIReport_130_OutInPt(out mHIReportID, out mHIReportOutPt, asyncResult);
                                    if (mResultVal || mHIReportID > 0 || mHIReportOutPt > 0)
                                    {
                                        if (mHIReportID > 0)
                                        {
                                            mHealthInsuranceReport.HIReportID = mHIReportID;
                                        }
                                        if (mHIReportOutPt > 0)
                                        {
                                            mHealthInsuranceReport.HIReportOutPt = mHIReportOutPt;
                                        }
                                        genTask.AddResultObj(mHealthInsuranceReport);
                                        genTask.ActionComplete(true);
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                        OutputErrorMessage += Environment.NewLine + eHCMSResources.Z2334_G1_KhongTaoDuocBaoCao;
                                        ClientLoggerHelper.LogInfo(string.Format("CreateHIReportFileTask Try => {0}", OutputErrorMessage));
                                        genTask.ActionComplete(false);
                                        this.HideBusyIndicator();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    OutputErrorMessage = ex.Message;
                                    ClientLoggerHelper.LogInfo(string.Format("CreateHIReportFileTask Catch => {0}", OutputErrorMessage));
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
        private void CreateHIReport_130_XmlTask(GenericCoRoutineTask genTask, object aHealthInsuranceReport)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetHIXmlReport_130_AllTab_InOneRpt((aHealthInsuranceReport as HealthInsuranceReport).HIReportID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var ReportStream = contract.EndGetHIXmlReport_130_AllTab_InOneRpt(asyncResult);
                                    string mHIAPICheckHICardAddress = string.Format(gIAPISendHIReportAddressParams, GlobalsNAV.gLoggedHIAPIUser.APIKey.access_token, GlobalsNAV.gLoggedHIAPIUser.APIKey.id_token, Globals.ServerConfigSection.Hospitals.HIAPILoginAccount, GlobalsNAV.gLoggedHIAPIUser.password, Globals.ServerConfigSection.Hospitals.HospitalCode.Length < 2 ? "" : Globals.ServerConfigSection.Hospitals.HospitalCode.Substring(0, 2), Globals.ServerConfigSection.Hospitals.HospitalCode);
                                    string mRestJson = GlobalsNAV.GetRESTServiceJSon(gIAPISendHIReportAddress, mHIAPICheckHICardAddress, ReportStream);
                                    HIAPIUploadHIReportXmlResult mHIAPIUploadHIReportXmlResult = GlobalsNAV.ConvertJsonToObject<HIAPIUploadHIReportXmlResult>(mRestJson);
                                    if (mHIAPIUploadHIReportXmlResult.maKetQua == 200)
                                    {
                                        genTask.AddResultObj(mHIAPIUploadHIReportXmlResult);
                                        genTask.ActionComplete(true);
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                        var mErrorMessage = string.IsNullOrEmpty(mHIAPIUploadHIReportXmlResult.maGiaoDich) ? GetErrorMessageFromErrorCode(mHIAPIUploadHIReportXmlResult.maKetQua) : mHIAPIUploadHIReportXmlResult.maGiaoDich;
                                        if (!string.IsNullOrEmpty(mErrorMessage))
                                        {
                                            mErrorMessage = string.Format(" - {0}", mErrorMessage);
                                        }
                                        OutputErrorMessage += Environment.NewLine + string.Format("{0}: {1}{2}", eHCMSResources.T0074_G1_I, mHIAPIUploadHIReportXmlResult.maKetQua, mErrorMessage);
                                        ClientLoggerHelper.LogInfo(string.Format("CreateHIReportXmlTask Else => {0}", OutputErrorMessage));
                                        genTask.ActionComplete(false);
                                        this.HideBusyIndicator();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    ClientLoggerHelper.LogInfo(string.Format("CreateHIReportXmlTask Catch => {0}", ex.Message));
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
        //▲====: #003
    }
}
