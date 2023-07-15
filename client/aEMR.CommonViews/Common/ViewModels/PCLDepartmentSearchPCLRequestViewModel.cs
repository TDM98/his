using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using aEMR.CommonTasks;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using Caliburn.Micro;
using aEMR.ViewContracts;
using System.ComponentModel.Composition;
using DataEntities;
using aEMR.Common.Collections;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using eHCMSLanguage;
using aEMR.DataContracts;
using aEMR.Common.BaseModel;
using System.Threading;
using aEMR.ServiceClient;
using aEMR.Controls;
using System.Collections.ObjectModel;
using System.Linq;
/*
 * 20191204 #001 TTM: BM 0019688: [Tìm kiếm] Bổ sung popup tìm kiếm đăng ký cho thực hiện CLS của bệnh nhân khám sức khoẻ có thể tìm theo tên công ty.
 * 20210709 #002 TNHX: 260 Thêm chọn user official
 * 20220725 #003 QTD: Truyền V_PCLRequestStatus lên popup là tất cả khi tìm thấy nhiều kết quả
 * 20220830 #004 TNHX: 2168 Thêm cấu hình thời gian tìm kiếm cho KSK
 */
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IPCLDepartmentSearchPCLRequest)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLDepartmentSearchPCLRequestViewModel : ViewModelBase, IPCLDepartmentSearchPCLRequest
    // , IHandle<DbClickSelectedObjectEvent<PatientPCLRequest>>
    // , IHandle<ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest>>
    {
        private long _V_PCLMainCategory;
        public long V_PCLMainCategory
        {
            get
            {
                return _V_PCLMainCategory;
            }
            set
            {
                _V_PCLMainCategory = value;
                NotifyOfPropertyChange(() => V_PCLMainCategory);
            }
        }

        public const string PATIENT_CODE_REG_EXP = "^[0-9]{8}$";

        protected override void OnActivate()
        {
            base.OnActivate();
        }
        public override void DeactivateItem(object item, bool close)
        {
            base.DeactivateItem(item, close);
            Globals.EventAggregator.Unsubscribe(this);
        }

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public PCLDepartmentSearchPCLRequestViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            SearchCriteria = new PatientPCLRequestSearchCriteria();
            PatientSearch = new PatientSearchCriteria();

            ObjPatientPCLRequest_SearchPaging = new PagedSortableCollectionView<PatientPCLRequest>();
            ObjPatientPCLRequest_SearchPaging.OnRefresh += new EventHandler<Collections.RefreshEventArgs>(ObjPatientPCLRequest_SearchPaging_OnRefresh);
            ObjPatientPCLRequest_SearchPaging_Selected = new PatientPCLRequest();
            ObjPatientPCLRequest_SearchPaging.PageIndex = 0;

            bIsNgoaiTruChecked = true;
            SetPatientFindBy();

            if (Globals.DoctorAccountBorrowed != null & Globals.DoctorAccountBorrowed.Staff != null)
            {
                UserOfficialFullName = Globals.DoctorAccountBorrowed.Staff.FullName;
            }

            // VuTTM - QMS Service
            IsQMSEnable = GlobalsNAV.IsQMSEnable() && bIsNgoaiTruChecked;
        }

        AxSearchPatientTextBox SearchPatientTextBox;
        public void SearchPatientTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            SearchPatientTextBox = (AxSearchPatientTextBox)sender;
            SearchPatientTextBox.Focus();
        }

        void ObjPatientPCLRequest_SearchPaging_OnRefresh(object sender, Collections.RefreshEventArgs e)
        {
            Coroutine.BeginExecute(DoPatientPCLRequest_SearchPaging(PatientSearch, ObjPatientPCLRequest_SearchPaging.PageIndex, ObjPatientPCLRequest_SearchPaging.PageSize, false));
        }

        private PatientPCLRequestSearchCriteria _SearchCriteria;
        public PatientPCLRequestSearchCriteria SearchCriteria
        {
            get { return _SearchCriteria; }
            set
            {
                if (_SearchCriteria != value)
                {
                    _SearchCriteria = value;
                    NotifyOfPropertyChange(() => SearchCriteria);
                }
            }
        }

        private int _FindPatient;
        public int FindPatient
        {
            get { return _FindPatient; }
            set
            {
                if (_FindPatient != value)
                {
                    _FindPatient = value;
                    NotifyOfPropertyChange(() => FindPatient);
                }
            }
        }

        private bool _bIsNgoaiTruChecked;
        public bool bIsNgoaiTruChecked
        {
            get { return _bIsNgoaiTruChecked; }
            set
            {
                _bIsNgoaiTruChecked = value;
                _IsQMSEnable = _bIsNgoaiTruChecked;
                _bIsNoiTruChecked = !_bIsNgoaiTruChecked;
                NotifyOfPropertyChange(() => bIsNgoaiTruChecked);
            }
        }

        private bool _bIsNoiTruChecked;
        public bool bIsNoiTruChecked
        {
            get { return _bIsNoiTruChecked; }
            set
            {
                _bIsNoiTruChecked = value;
                _bIsNgoaiTruChecked = !bIsNoiTruChecked;
                _IsQMSEnable = _bIsNgoaiTruChecked;
                NotifyOfPropertyChange(() => bIsNoiTruChecked);
            }
        }

        public void btSearch()
        {
            SetPatientFindBy();
            if (string.IsNullOrEmpty(PatientSearch.PatientNameString))
            {
                Action<ISearchPCLRequest> onInitDlg = (typeInfo) =>
                {
                    typeInfo.PatientFindBy = PatientFindBy;
                    typeInfo.LoadData();
                    //20181126 TBL: BM 0005352. Tim mac dinh nhung phieu co trang thai la Cho thuc hien
                    typeInfo.SearchCriteria.V_PCLRequestStatus = (long)AllLookupValues.V_PCLRequestStatus.OPEN;
                };
                GlobalsNAV.ShowDialog<ISearchPCLRequest>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSizeInput(800, 570));
            }
            else
            {
                Coroutine.BeginExecute(DoPatientPCLRequest_SearchPaging(PatientSearch, 0, ObjPatientPCLRequest_SearchPaging.PageSize, true));
            }
        }

        private PatientSearchCriteria _PatientSearch;
        public PatientSearchCriteria PatientSearch
        {
            get { return _PatientSearch; }
            set
            {
                _PatientSearch = value;
                NotifyOfPropertyChange(() => PatientSearch);
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

        private PagedSortableCollectionView<PatientPCLRequest> _ObjPatientPCLRequest_SearchPaging;
        public PagedSortableCollectionView<PatientPCLRequest> ObjPatientPCLRequest_SearchPaging
        {
            get { return _ObjPatientPCLRequest_SearchPaging; }
            set
            {
                _ObjPatientPCLRequest_SearchPaging = value;
                NotifyOfPropertyChange(() => ObjPatientPCLRequest_SearchPaging);
            }
        }

        private IEnumerator<IResult> DoPatientPCLRequest_SearchPaging(PatientSearchCriteria searchCriteria, int PageIndex, int PageSize, bool CountTotal)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            if (V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Imaging)
            {
                SearchCriteria.V_PCLMainCategory = V_PCLMainCategory;
                SearchCriteria.PCLExamTypeSubCategoryID = Globals.PCLDepartment.ObjPCLExamTypeSubCategoryID != null ? Globals.PCLDepartment.ObjPCLExamTypeSubCategoryID.PCLExamTypeSubCategoryID : 0;
                SearchCriteria.PCLResultParamImpID = Globals.PCLDepartment.ObjPCLResultParamImpID != null ? Globals.PCLDepartment.ObjPCLResultParamImpID.PCLResultParamImpID : 0;
            }
            else
            {
                //20181127 TTM: Bổ sung V_PCLMainCategory cho Xét nghiệm.
                SearchCriteria.V_PCLMainCategory = V_PCLMainCategory;
            }

            SearchCriteria.PatientFindBy = PatientFindBy;
            SearchCriteria.PCLRequestNumID = PatientSearch.PCLRequestNumID;
            SearchCriteria.FullName = PatientSearch.FullName;
            SearchCriteria.PatientCode = PatientSearch.PatientCode;
            SearchCriteria.FromDate = Globals.ServerDate;
            SearchCriteria.ToDate = Globals.ServerDate;
            SearchCriteria.PCLExamTypeLocationsDeptLocationID = Globals.DeptLocation.DeptLocationID;
            //▼====== #003
            SearchCriteria.V_PCLRequestStatus = (long)AllLookupValues.V_PCLRequestStatus.ALL;
            //▲====== #003

            var loadRegInfoTask = new PCLDepartmentSearchPCLRequestTask(SearchCriteria, PageIndex, PageSize, CountTotal);
            yield return loadRegInfoTask;

            if (loadRegInfoTask.Error == null)
            {
                if (loadRegInfoTask.PatientPclRequestList != null && loadRegInfoTask.PatientPclRequestList.Count > 0)
                {
                    if (loadRegInfoTask.PatientPclRequestList.Count == 1)
                    {
                        ObjPatientPCLRequest_SearchPaging_Selected = loadRegInfoTask.PatientPclRequestList[0];

                        Globals.PatientPCLRequest_Result = loadRegInfoTask.PatientPclRequestList[0];
                        if (SearchCriteria.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Imaging)
                        {
                            yield return GenericCoRoutineTask.StartTask(GetPatientPCLRequestResultsByReqID, SearchCriteria);
                        }
                        UpdateReceptionTime(ObjPatientPCLRequest_SearchPaging_Selected.PatientPCLReqID, Globals.GetCurServerDateTime(), (long)ObjPatientPCLRequest_SearchPaging_Selected.V_PCLRequestType);
                        // 1. Load Patient.
                        this.DlgShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
                        var loadPatients = new LoadPatientTask(ObjPatientPCLRequest_SearchPaging_Selected.PatientID);
                        yield return loadPatients;
                        this.DlgHideBusyIndicator();
                        Patient curPatient = loadPatients.CurrentPatient;
                        ObjPatientPCLRequest_SearchPaging_Selected.Patient = curPatient;

                        // 2. Load Registration
                        this.DlgShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
                        var loadRegistration = new LoadRegistrationSimpleTask(ObjPatientPCLRequest_SearchPaging_Selected.PtRegistrationID, (int)PatientFindBy);
                        yield return loadRegistration;
                        this.DlgHideBusyIndicator();
                        PatientRegistration curRegistration = loadRegistration.Registration;

                        curPatient.CurrentHealthInsurance = curRegistration.HealthInsurance;
                        ObjPatientPCLRequest_SearchPaging_Selected.ExamDate = curRegistration.ExamDate;
                        // 3. Fire event to show Patient Info.
                        Globals.EventAggregator.Publish(new ShowPatientInfoFromTextBoxSearchPCLRequest() { Patient = curPatient, PtRegistration = curRegistration });

                        if (V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Imaging)
                        {
                            Globals.PatientPCLReqID_Imaging = ObjPatientPCLRequest_SearchPaging_Selected.PatientPCLReqID;
                            Globals.PatientPCLRequest_Imaging = ObjPatientPCLRequest_SearchPaging_Selected;
                            // LoadContentByParaEnum();
                            Globals.EventAggregator.Publish(new PCLDeptImagingResultLoadEvent { PatientPCLRequest_Imaging = ObjPatientPCLRequest_SearchPaging_Selected });
                            Globals.EventAggregator.Publish(new ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest> { PCLRequest = ObjPatientPCLRequest_SearchPaging_Selected });
                            Globals.EventAggregator.Publish(new Event_SearchAbUltraRequestCompleted() { Patient = curPatient, PCLRequest = ObjPatientPCLRequest_SearchPaging_Selected });
                        }
                        else
                        {
                            Globals.PatientPCLReqID_LAB = ObjPatientPCLRequest_SearchPaging_Selected.PatientPCLReqID;
                            Globals.PatientPCLRequest_LAB = ObjPatientPCLRequest_SearchPaging_Selected;
                            Globals.EventAggregator.Publish(new DbClickSelectedObjectEvent<PatientPCLRequest> { Result = ObjPatientPCLRequest_SearchPaging_Selected });
                        }
                    }
                    else //mo popup chon phieu
                    {
                        ObjPatientPCLRequest_SearchPaging.Clear();
                        if (loadRegInfoTask.CountTotal)
                        {
                            ObjPatientPCLRequest_SearchPaging.TotalItemCount = loadRegInfoTask.TotalItemCount;
                        }
                        if (loadRegInfoTask.PatientPclRequestList != null)
                        {
                            foreach (var item in loadRegInfoTask.PatientPclRequestList)
                            {
                                ObjPatientPCLRequest_SearchPaging.Add(item);
                            }
                        }

                        //var typeInfo = Globals.GetViewModel<ISearchPCLRequest>();
                        //typeInfo.PatientFindBy = PatientFindBy;
                        //typeInfo.SearchCriteria = SearchCriteria;
                        //typeInfo.ObjPatientPCLRequest_SearchPaging = ObjPatientPCLRequest_SearchPaging;
                        //var instance = typeInfo as Conductor<object>;

                        Action<ISearchPCLRequest> onInitDlg = (typeInfo) =>
                        {
                            typeInfo.PatientFindBy = PatientFindBy;
                            typeInfo.SearchCriteria = SearchCriteria;
                            typeInfo.ObjPatientPCLRequest_SearchPaging = ObjPatientPCLRequest_SearchPaging;
                        };
                        //GlobalsNAV.ShowDialog<ISearchPCLRequest>(onInitDlg);
                        GlobalsNAV.ShowDialog<ISearchPCLRequest>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSizeInput(800, 570));
                    }
                }
                else
                {
                    MessageBox.Show(string.Format("{0}!", eHCMSResources.Z1075_G1_KgTimThayPhYC));
                    //Mở popup tìm kiếm khi không tìm thấy phiếu
                    Action<ISearchPCLRequest> onInitDlg = (typeInfo) =>
                    {
                        typeInfo.PatientFindBy = PatientFindBy;
                        typeInfo.SearchCriteria = SearchCriteria;
                        typeInfo.ObjPatientPCLRequest_SearchPaging = ObjPatientPCLRequest_SearchPaging;
                        typeInfo.SearchCriteria.V_PCLRequestStatus = (long)AllLookupValues.V_PCLRequestStatus.OPEN;
                    };
                    //GlobalsNAV.ShowDialog<ISearchPCLRequest>(onInitDlg);
                    GlobalsNAV.ShowDialog<ISearchPCLRequest>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSizeInput(800, 570));
                }
            }
            else
            {
                Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = new AxErrorEventArgs(loadRegInfoTask.Error) });
            }
            this.HideBusyIndicator();
            yield break;
        }

        public AllLookupValues.PatientFindBy PatientFindBy { get; set; }

        private void SetPatientFindBy()
        {
            if (bIsNgoaiTruChecked)
            {
                PatientFindBy = AllLookupValues.PatientFindBy.NGOAITRU;
                Globals.PatientFindBy_ForImaging = AllLookupValues.PatientFindBy.NGOAITRU;
                IsQMSEnable = true;
            }
            else
            {
                PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;
                Globals.PatientFindBy_ForImaging = AllLookupValues.PatientFindBy.NOITRU;
                IsQMSEnable = false;
            }
        }

        public void rdoNgoaiTru_Click(object sender, RoutedEventArgs e)
        {
            SetPatientFindBy();
        }

        public void rdoNoiTru_Click(object sender, RoutedEventArgs e)
        {
            SetPatientFindBy();
        }

        public void rdoCaHai_Click(object sender, RoutedEventArgs e)
        {
            SetPatientFindBy();
        }

        private void GetPatientPCLRequestResultsByReqID(GenericCoRoutineTask aGenTask, object aSearchCriteria)
        {
            if (Globals.PatientPCLRequest_Result == null || Globals.PatientPCLRequest_Result.PatientPCLReqID == 0 || !(aSearchCriteria != null && aSearchCriteria is PatientPCLRequestSearchCriteria))
            {
                aGenTask.ActionComplete(false);
                this.HideBusyIndicator();
            }
            PatientPCLRequestSearchCriteria mSearchCriteria = aSearchCriteria == null ? new PatientPCLRequestSearchCriteria() : aSearchCriteria as PatientPCLRequestSearchCriteria;
            mSearchCriteria.PatientPCLReqID = Globals.PatientPCLRequest_Result.PatientPCLReqID;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetPatientPCLRequestResultsByReqID(mSearchCriteria, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var mPCLRequest = contract.EndGetPatientPCLRequestResultsByReqID(asyncResult);
                            if (mPCLRequest != null && mPCLRequest.PatientPCLReqID != 0)
                            {
                                ObjPatientPCLRequest_SearchPaging_Selected = mPCLRequest;
                                Globals.PatientPCLRequest_Result = ObjPatientPCLRequest_SearchPaging_Selected;
                                aGenTask.ActionComplete(true);
                            }
                            else
                            {
                                aGenTask.ActionComplete(false);
                                this.HideBusyIndicator();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                            aGenTask.ActionComplete(false);
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });

            t.Start();
        }

        public void SearchPatientCmdNext()
        {
            if (!bIsNgoaiTruChecked)
            {
                return;
            }

            if (!GlobalsNAV.IsQMSEnable() || 0 == Globals.DeptLocation.DeptLocationID)
            {
                IsQMSEnable = false;
                MessageBox.Show("Thông tin phòng không hợp lệ. Vui lòng chọn lại phòng!");
                return;
            }

            OrderDTO curOrder = null;
            if (GlobalsNAV.IsQMSEnable())
            {
                curOrder = GlobalsNAV.GetNextWaitingOrder(Globals.DeptLocation.DeptLocationID);
            }

            if (GlobalsNAV.IsQMSEnable() && null != curOrder)
            {
                String msg = "Bạn đang gọi bệnh nhân\r\n";
                msg += String.Format("Số thứ tự: {0}\r\n", curOrder.orderNumber);
                msg += !String.IsNullOrEmpty(curOrder.patientName)
                    ? String.Format("Tên bệnh nhân: {0}\r\n", curOrder.patientName) : string.Empty;
                msg += !String.IsNullOrEmpty(curOrder.patientCode)
                    ? String.Format("Mã bệnh nhân: {0}\r\n", curOrder.patientCode) : string.Empty;

                MessageBox.Show(msg);
                curOrder.orderStatus = OrderDTO.CALLING_STATUS;
                curOrder.refDeptId = Globals.DeptLocation.DeptID;
                curOrder.refLocationId = Globals.DeptLocation.DeptLocationID;
                curOrder.startedServiceAt = Globals.GetCurServerDateTime()
                    .ToString(OrderDTO.DEFAULT_DATE_TIME_FORMAT); ;
                GlobalsNAV.UpdateOrder(curOrder);
                CurOrderNumber = curOrder.orderNumber.Value;
            }

            if (GlobalsNAV.IsQMSEnable() && null == curOrder)
            {
                MessageBox.Show("Hệ thống chưa ghi nhận STT tiếp theo!");
                return;
            }

            if (GlobalsNAV.IsQMSEnable()
                && !String.IsNullOrEmpty(curOrder.patientCode))
            {
                PatientSearch.PatientCode = curOrder.patientCode;
                PatientSearch.PCLRequestNumID = null;
                PatientSearch.FullName = null;

                btSearch();

                System.Console.WriteLine(ObjPatientPCLRequest_SearchPaging_Selected.Patient);
            }
        }

        //▼====: #004
        //▼===== #001
        public void btSearchExamination()
        {
            SetPatientFindBy();
            if (string.IsNullOrEmpty(PatientSearch.PatientNameString))
            {
                ISearchPCLRequestForMedicalExamination onInitDlg = Globals.GetViewModel<ISearchPCLRequestForMedicalExamination>();
                onInitDlg.PatientFindBy = PatientFindBy;
                onInitDlg.LoadData();
                onInitDlg.SearchCriteria.V_PCLRequestStatus = (long)AllLookupValues.V_PCLRequestStatus.OPEN;
                //Action<ISearchPCLRequestForMedicalExamination> onInitDlg = (typeInfo) =>
                //{
                //    typeInfo.PatientFindBy = PatientFindBy;
                //    typeInfo.LoadData();
                //    //20181126 TBL: BM 0005352. Tim mac dinh nhung phieu co trang thai la Cho thuc hien
                //    typeInfo.SearchCriteria.V_PCLRequestStatus = (long)AllLookupValues.V_PCLRequestStatus.OPEN;
                //};
                //GlobalsNAV.ShowDialog<ISearchPCLRequestForMedicalExamination>(onInitDlg);
                GlobalsNAV.ShowDialog_V3(onInitDlg, null, null, false, true, Globals.GetPCLSearchPatientKSKDialogViewSize());
            }
            else
            {
                Coroutine.BeginExecute(DoPatientPCLRequest_SearchPagingForExamination(PatientSearch, 0, ObjPatientPCLRequest_SearchPaging.PageSize, true));
            }
        }
        //▲====: #004

        private IEnumerator<IResult> DoPatientPCLRequest_SearchPagingForExamination(PatientSearchCriteria searchCriteria, int PageIndex, int PageSize, bool CountTotal)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            if (V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Imaging)
            {
                SearchCriteria.V_PCLMainCategory = V_PCLMainCategory;
                SearchCriteria.PCLExamTypeSubCategoryID = Globals.PCLDepartment.ObjPCLExamTypeSubCategoryID != null ? Globals.PCLDepartment.ObjPCLExamTypeSubCategoryID.PCLExamTypeSubCategoryID : 0;
                SearchCriteria.PCLResultParamImpID = Globals.PCLDepartment.ObjPCLResultParamImpID != null ? Globals.PCLDepartment.ObjPCLResultParamImpID.PCLResultParamImpID : 0;
            }
            else
            {
                SearchCriteria.V_PCLMainCategory = V_PCLMainCategory;
            }

            SearchCriteria.PatientFindBy = PatientFindBy;

            SearchCriteria.PCLRequestNumID = PatientSearch.PCLRequestNumID;
            SearchCriteria.FullName = PatientSearch.FullName;
            SearchCriteria.PatientCode = PatientSearch.PatientCode;
            //▼====: #004
            SearchCriteria.FromDate = Globals.GetCurServerDateTime().AddDays(-Globals.ServerConfigSection.OutRegisElements.NumDayFindOutRegistrationMedicalExamination);
            SearchCriteria.ToDate = Globals.GetCurServerDateTime();
            //▲====: #004

            SearchCriteria.PCLExamTypeLocationsDeptLocationID = Globals.DeptLocation.DeptLocationID;

            var loadRegInfoTask = new GenericCoRoutineTask(PCLDepartmentSearchPCLRequest_GenTask, SearchCriteria, PageIndex, PageSize, CountTotal);
            yield return loadRegInfoTask;

            int total = Convert.ToInt32(loadRegInfoTask.GetResultObj(0));
            bool countTotal = Convert.ToBoolean(loadRegInfoTask.GetResultObj(1));
            List<PatientPCLRequest> PatientPclRequestList = loadRegInfoTask.GetResultObj(2) as List<PatientPCLRequest>;

            if (loadRegInfoTask.Error == null)
            {
                if (PatientPclRequestList != null && PatientPclRequestList.Count > 0)
                {
                    if (PatientPclRequestList.Count == 1)
                    {
                        ObjPatientPCLRequest_SearchPaging_Selected = PatientPclRequestList[0];

                        Globals.PatientPCLRequest_Result = PatientPclRequestList[0];
                        if (SearchCriteria.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Imaging)
                        {
                            yield return GenericCoRoutineTask.StartTask(GetPatientPCLRequestResultsByReqID, SearchCriteria);
                        }

                        this.DlgShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
                        var loadPatients = new LoadPatientTask(ObjPatientPCLRequest_SearchPaging_Selected.PatientID);
                        yield return loadPatients;
                        this.DlgHideBusyIndicator();
                        Patient curPatient = loadPatients.CurrentPatient;
                        ObjPatientPCLRequest_SearchPaging_Selected.Patient = curPatient;

                        this.DlgShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
                        var loadRegistration = new LoadRegistrationSimpleTask(ObjPatientPCLRequest_SearchPaging_Selected.PtRegistrationID, (int)PatientFindBy);
                        yield return loadRegistration;
                        this.DlgHideBusyIndicator();
                        PatientRegistration curRegistration = loadRegistration.Registration;

                        curPatient.CurrentHealthInsurance = curRegistration.HealthInsurance;

                        Globals.EventAggregator.Publish(new ShowPatientInfoFromTextBoxSearchPCLRequest() { Patient = curPatient, PtRegistration = curRegistration });

                        if (V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Imaging)
                        {
                            Globals.PatientPCLReqID_Imaging = ObjPatientPCLRequest_SearchPaging_Selected.PatientPCLReqID;
                            Globals.PatientPCLRequest_Imaging = ObjPatientPCLRequest_SearchPaging_Selected;
                            Globals.EventAggregator.Publish(new PCLDeptImagingResultLoadEvent { PatientPCLRequest_Imaging = ObjPatientPCLRequest_SearchPaging_Selected });
                            Globals.EventAggregator.Publish(new ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest> { PCLRequest = ObjPatientPCLRequest_SearchPaging_Selected });
                            Globals.EventAggregator.Publish(new Event_SearchAbUltraRequestCompleted() { Patient = curPatient, PCLRequest = ObjPatientPCLRequest_SearchPaging_Selected });
                        }
                        else
                        {
                            Globals.PatientPCLReqID_LAB = ObjPatientPCLRequest_SearchPaging_Selected.PatientPCLReqID;
                            Globals.PatientPCLRequest_LAB = ObjPatientPCLRequest_SearchPaging_Selected;
                            Globals.EventAggregator.Publish(new DbClickSelectedObjectEvent<PatientPCLRequest> { Result = ObjPatientPCLRequest_SearchPaging_Selected });
                        }
                    }
                    else
                    {
                        ObjPatientPCLRequest_SearchPaging.Clear();
                        if (countTotal)
                        {
                            ObjPatientPCLRequest_SearchPaging.TotalItemCount = total;
                        }
                        if (PatientPclRequestList != null)
                        {
                            foreach (var item in PatientPclRequestList)
                            {
                                ObjPatientPCLRequest_SearchPaging.Add(item);
                            }
                        }

                        Action<ISearchPCLRequestForMedicalExamination> onInitDlg = (typeInfo) =>
                        {
                            typeInfo.PatientFindBy = PatientFindBy;
                            typeInfo.SearchCriteria = SearchCriteria;
                            typeInfo.ObjPatientPCLRequest_SearchPaging = ObjPatientPCLRequest_SearchPaging;
                        };
                        GlobalsNAV.ShowDialog<ISearchPCLRequestForMedicalExamination>(onInitDlg);
                    }
                }
                else
                {
                    MessageBox.Show(string.Format("{0}!", eHCMSResources.Z1075_G1_KgTimThayPhYC));
                }
            }
            else
            {
                Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = new AxErrorEventArgs(loadRegInfoTask.Error) });
            }
            this.HideBusyIndicator();
            yield break;
        }

        private void PCLDepartmentSearchPCLRequest_GenTask(GenericCoRoutineTask genTask, object searchCriteria, object pageIndex, object pageSize, object countTotal)
        {
            PatientPCLRequestSearchCriteria SearchCriteria = (PatientPCLRequestSearchCriteria)searchCriteria;
            int PageIndex = (int)pageIndex;
            int PageSize = (int)pageSize;
            bool CountTotal = false;

            int Total = 0;
            List<PatientPCLRequest> PatientPCLReqList = new List<PatientPCLRequest>();
            this.DlgShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        bool bActionCompleteOk = false;
                        contract.BeginPatientPCLRequest_SearchPaging_ForMedicalExamination(SearchCriteria, PageIndex, PageSize, "", (bool)countTotal
                            , Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    PatientPCLReqList = contract.EndPatientPCLRequest_SearchPaging_ForMedicalExamination(out Total, asyncResult).ToList();
                                    bActionCompleteOk = true;
                                }
                                catch (Exception ex)
                                {
                                    bActionCompleteOk = false;
                                }
                                finally
                                {
                                    genTask.AddResultObj(Total);
                                    genTask.AddResultObj(CountTotal);
                                    genTask.AddResultObj(PatientPCLReqList);
                                    genTask.AddResultObj(CountTotal);
                                    genTask.ActionComplete(bActionCompleteOk);
                                    this.DlgHideBusyIndicator();
                                    if (!bActionCompleteOk)
                                    {
                                        TryClose();
                                    }
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    this.DlgHideBusyIndicator();
                    genTask.ActionComplete(false);
                    MessageBox.Show(ex.Message, "");
                }
            });

            t.Start();
        }
        //▲===== #001

        private void UpdateReceptionTime(long PatientPCLReqID, DateTime ReceptionTime, long V_PCLRequestType)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginUpdateReceptionTime(PatientPCLReqID, ReceptionTime, V_PCLRequestType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                client.EndUpdateReceptionTime(asyncResult);
                            }
                            catch (Exception innerEx)
                            {
                                MessageBox.Show(innerEx.Message);
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
                    Globals.IsBusy = false;
                }
            });

            t.Start();
        }

        private string _UserOfficialFullName;
        public string UserOfficialFullName
        {
            get { return _UserOfficialFullName; }
            set
            {
                if (_UserOfficialFullName != value)
                {
                    _UserOfficialFullName = value;
                }
                NotifyOfPropertyChange(() => UserOfficialFullName);
            }
        }

        private bool _IsShowBtnChooseUserOfficial = false;
        public bool IsShowBtnChooseUserOfficial
        {
            get { return _IsShowBtnChooseUserOfficial; }
            set
            {
                if (_IsShowBtnChooseUserOfficial != value)
                {
                    _IsShowBtnChooseUserOfficial = value;
                }
                NotifyOfPropertyChange(() => IsShowBtnChooseUserOfficial);
            }
        }

        public void ChooseUserOfficialCmd()
        {
            void onInitDlg(IDoctorBorrowedAccount vm)
            {
                vm.IsPopupView = true;
                vm.PatientFindBy = (int)PatientFindBy;
            }
            GlobalsNAV.ShowDialog<IDoctorBorrowedAccount>(onInitDlg);
        }

        /// <summary>
        /// VuTTM - Enable control for applying QMS.
        /// </summary>
        private bool _IsQMSEnable;
        public bool IsQMSEnable
        {
            get { return _IsQMSEnable; }
            set
            {
                _IsQMSEnable = value;
                NotifyOfPropertyChange(() => IsQMSEnable);
            }
        }

        /// <summary>
        /// VuTTM - Current Order Number
        /// </summary>
        private long? _CurOrderNumber;
        public long? CurOrderNumber
        {
            get { return _CurOrderNumber; }
            set
            {
                _CurOrderNumber = value;
                NotifyOfPropertyChange(() => CurOrderNumber);
            }
        }
    }
}
