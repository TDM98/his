using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using System.Linq;
using System.Collections.Generic;
using aEMR.Common;
using System.Windows.Media;
using Service.Core.Common;
using aEMR.StoreDept.Views;
using aEMR.Common.Printing;
using aEMR.CommonTasks;
using System.Windows.Data;
using aEMR.Controls;
using System.Text;
using aEMR.Common.PagedCollectionView;
using Castle.Windsor;

namespace aEMR.StoreDept.ViewModels
{
    [Export(typeof(IClinicDeptInPtReqForm)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ClinicDeptInPtReqFormViewModel : Conductor<object>, IClinicDeptInPtReqForm, 
         IHandle<DrugDeptCloseSearchRequestEvent>
        ,IHandle<PatientSelectedGoToKhamBenh_InPt<PatientRegistration>>
    {

        private bool _calByUnitUse;
        public bool CalByUnitUse
        {
            get { return _calByUnitUse; }
            set
            {
                if (_calByUnitUse != value)
                {
                    _calByUnitUse = value;
                    NotifyOfPropertyChange(() => CalByUnitUse);
                }
            }
        }

        private ISearchPatientAndRegistration _searchRegistrationContent;
        public ISearchPatientAndRegistration SearchRegistrationContent
        {
            get { return _searchRegistrationContent; }
            set
            {
                _searchRegistrationContent = value;
                NotifyOfPropertyChange(() => SearchRegistrationContent);
            }
        }

        private Patient _currentPatient;
        public Patient CurrentPatient
        {
            get { return _currentPatient; }
            set
            {
                if (_currentPatient != value)
                {
                    _currentPatient = value;
                    NotifyOfPropertyChange(() => CurrentPatient);
                }
            }
        }
        [ImportingConstructor]
        public ClinicDeptInPtReqFormViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            authorization();

            //form tim kiem
            var searchPatientAndRegVm = Globals.GetViewModel<ISearchPatientAndRegistration>();
            searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_PATIENT_BTN | SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
            searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);
            searchPatientAndRegVm.PatientFindByVisibility = false;
            searchPatientAndRegVm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;
            searchPatientAndRegVm.mTimBN = false;
            //searchPatientAndRegVm.mThemBN = mDangKyNoiTru_Patient_ThemBN;
            //searchPatientAndRegVm.mTimDangKy = mDangKyNoiTru_Patient_TimDangKy;

            searchPatientAndRegVm.SearchAdmittedInPtRegOnly = true;

            SearchRegistrationContent = searchPatientAndRegVm;
            ActivateItem(searchPatientAndRegVm);

            Globals.EventAggregator.Subscribe(this);

            //Coroutine.BeginExecute(DoGetStore_All());
            StoreCbxStaff = Globals.checkStoreWareHouse(V_MedProductType, false, true);
            if (StoreCbxStaff == null || StoreCbxStaff.Count < 1)
            {
                MessageBox.Show(eHCMSResources.A0110_G1_Msg_InfoChuaCauHinhTNKho);
            }

            Coroutine.BeginExecute(DoGetStore_MedDept());
            Coroutine.BeginExecute(DoGetRefGenericDrugCategory_1List());

            SearchCriteria = new RequestSearchCriteria();
            
            // TxD 18/11/2014: Set default search Date From = 3 days for Now
            SearchCriteria.FromDate = DateTime.Now.Date - new TimeSpan(3,0,0,0);
            SearchCriteria.ToDate = DateTime.Now;

            ListRequestDrugDelete = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
            RequestDrug = new RequestDrugInwardClinicDept();
            CurrentReqOutwardDrugClinicDeptPatient = new ReqOutwardDrugClinicDeptPatient();
            RefeshRequest();

            RefGenMedProductDetails = new PagedSortableCollectionView<RefGenMedProductDetails>();
            RefGenMedProductDetails.OnRefresh += RefGenMedProductDetails_OnRefresh;
            RefGenMedProductDetails.PageSize = Globals.PageSize;

            ListStaff = new ObservableCollection<Staff>();
            GetListStaffFilter();
        }

        private bool _IsLoading = false;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set
            {
                if (_IsLoading != value)
                {
                    _IsLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private long _StaffDetailID;
        public long StaffDetailID
        {
            get { return _StaffDetailID; }
            set
            {
                if (_StaffDetailID != value)
                {
                    _StaffDetailID = value;
                    NotifyOfPropertyChange(() => StaffDetailID);
                }
            }
        }
        void RefGenMedProductDetails_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetRefGenMedProductDetails_Auto(BrandName, RefGenMedProductDetails.PageIndex, RefGenMedProductDetails.PageSize);
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

        }

        #region checking account
        private bool _mPhieuYeuCau_Tim = true;
        private bool _mPhieuYeuCau_Them = true;
        private bool _mPhieuYeuCau_Xoa = true;
        private bool _mPhieuYeuCau_XemIn = true;
        private bool _mPhieuYeuCau_In = true;

        public bool mPhieuYeuCau_Tim
        {
            get
            {
                return _mPhieuYeuCau_Tim;
            }
            set
            {
                if (_mPhieuYeuCau_Tim == value)
                    return;
                _mPhieuYeuCau_Tim = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_Tim);
            }
        }

        public bool mPhieuYeuCau_Them
        {
            get
            {
                return _mPhieuYeuCau_Them;
            }
            set
            {
                if (_mPhieuYeuCau_Them == value)
                    return;
                _mPhieuYeuCau_Them = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_Them);
            }
        }

        public bool mPhieuYeuCau_Xoa
        {
            get
            {
                return _mPhieuYeuCau_Xoa;
            }
            set
            {
                if (_mPhieuYeuCau_Xoa == value)
                    return;
                _mPhieuYeuCau_Xoa = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_Xoa);
            }
        }

        public bool mPhieuYeuCau_XemIn
        {
            get
            {
                return _mPhieuYeuCau_XemIn;
            }
            set
            {
                if (_mPhieuYeuCau_XemIn == value)
                    return;
                _mPhieuYeuCau_XemIn = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_XemIn);
            }
        }

        public bool mPhieuYeuCau_In
        {
            get
            {
                return _mPhieuYeuCau_In;
            }
            set
            {
                if (_mPhieuYeuCau_In == value)
                    return;
                _mPhieuYeuCau_In = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_In);
            }
        }

        #endregion

        #region 1. Property

        private string _strHienThi;
        public string strHienThi
        {
            get
            {
                return _strHienThi;
            }
            set
            {
                _strHienThi = value;
                NotifyOfPropertyChange(() => strHienThi);
            }
        }

        private ObservableCollection<RefGenericDrugCategory_1> _RefGenericDrugCategory_1s;
        public ObservableCollection<RefGenericDrugCategory_1> RefGenericDrugCategory_1s
        {
            get
            {
                return _RefGenericDrugCategory_1s;
            }
            set
            {
                if (_RefGenericDrugCategory_1s != value)
                {
                    _RefGenericDrugCategory_1s = value;
                    NotifyOfPropertyChange(() => RefGenericDrugCategory_1s);
                }
            }
        }

        private RefGenMedProductDetails _SelectRefGenMedProductDetail;
        public RefGenMedProductDetails SelectRefGenMedProductDetail
        {
            get
            {
                return _SelectRefGenMedProductDetail;
            }
            set
            {
                if (_SelectRefGenMedProductDetail != value)
                {
                    _SelectRefGenMedProductDetail = value;
                    NotifyOfPropertyChange(() => SelectRefGenMedProductDetail);
                }
            }
        }

        private PagedSortableCollectionView<RefGenMedProductDetails> _RefGenMedProductDetails;
        public PagedSortableCollectionView<RefGenMedProductDetails> RefGenMedProductDetails
        {
            get
            {
                return _RefGenMedProductDetails;
            }
            set
            {
                if (_RefGenMedProductDetails != value)
                {
                    _RefGenMedProductDetails = value;
                    NotifyOfPropertyChange(() => RefGenMedProductDetails);
                }
            }
        }

        private ObservableCollection<Staff> _ListStaff;
        public ObservableCollection<Staff> ListStaff
        {
            get { return _ListStaff; }
            set
            {
                if (_ListStaff != value)
                {
                    _ListStaff = value;
                    NotifyOfPropertyChange(() => ListStaff);
                }
            }
        }

        private ObservableCollection<RefStorageWarehouseLocation> _StoreCbx;
        public ObservableCollection<RefStorageWarehouseLocation> StoreCbx
        {
            get
            {
                return _StoreCbx;
            }
            set
            {
                if (_StoreCbx != value)
                {
                    _StoreCbx = value;
                    NotifyOfPropertyChange(() => StoreCbx);
                }
            }
        }

        private ObservableCollection<RefStorageWarehouseLocation> _StoreCbxStaff;
        public ObservableCollection<RefStorageWarehouseLocation> StoreCbxStaff
        {
            get
            {
                return _StoreCbxStaff;
            }
            set
            {
                if (_StoreCbxStaff != value)
                {
                    _StoreCbxStaff = value;
                    NotifyOfPropertyChange(() => StoreCbxStaff);
                }
            }
        }


        private ObservableCollection<ReqOutwardDrugClinicDeptPatient> ListRequestDrugDelete;

        private RequestDrugInwardClinicDept _RequestDrug;
        public RequestDrugInwardClinicDept RequestDrug
        {
            get
            {
                return _RequestDrug;
            }
            set
            {
                if (_RequestDrug != value)
                {
                    _RequestDrug = value;
                    NotifyOfPropertyChange(() => RequestDrug);
                }
            }
        }

        private ReqOutwardDrugClinicDeptPatient _CurrentReqOutwardDrugClinicDeptPatient;
        public ReqOutwardDrugClinicDeptPatient CurrentReqOutwardDrugClinicDeptPatient
        {
            get { return _CurrentReqOutwardDrugClinicDeptPatient; }
            set
            {
                if (_CurrentReqOutwardDrugClinicDeptPatient != value)
                {
                    _CurrentReqOutwardDrugClinicDeptPatient = value;
                    NotifyOfPropertyChange(() => CurrentReqOutwardDrugClinicDeptPatient);
                }
            }
        }

        private ReqOutwardDrugClinicDeptPatient _SelectedReqOutwardDrugClinicDeptPatient;
        public ReqOutwardDrugClinicDeptPatient SelectedReqOutwardDrugClinicDeptPatient
        {
            get { return _SelectedReqOutwardDrugClinicDeptPatient; }
            set
            {
                if (_SelectedReqOutwardDrugClinicDeptPatient != value)
                {
                    _SelectedReqOutwardDrugClinicDeptPatient = value;
                    NotifyOfPropertyChange(() => SelectedReqOutwardDrugClinicDeptPatient);
                }
            }
        }

        private PagedCollectionView _ReqOutwardDrugClinicDeptPatientlst;
        public PagedCollectionView ReqOutwardDrugClinicDeptPatientlst
        {
            get { return _ReqOutwardDrugClinicDeptPatientlst; }
            set
            {
                if (_ReqOutwardDrugClinicDeptPatientlst != value)
                {
                    _ReqOutwardDrugClinicDeptPatientlst = value;
                    NotifyOfPropertyChange(() => ReqOutwardDrugClinicDeptPatientlst);
                }
            }
        }


        private RequestSearchCriteria _SearchCriteria;
        public RequestSearchCriteria SearchCriteria
        {
            get
            {
                return _SearchCriteria;
            }
            set
            {
                _SearchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }

        private long _V_MedProductType = (long)AllLookupValues.MedProductType.THUOC; //11001 : thuoc, 11002 : y cu , 11003 :hoa chat
        public long V_MedProductType
        {
            get
            {
                return _V_MedProductType;
            }
            set
            {
                if (_V_MedProductType != value)
                {
                    _V_MedProductType = value;
                    NotifyOfPropertyChange(() => V_MedProductType);
                    NotifyOfPropertyChange(() => CanSelectedRefGenDrugCatID_1);
                }

            }
        }

        public bool CanSelectedRefGenDrugCatID_1
        {
            get { return V_MedProductType == (long)AllLookupValues.MedProductType.THUOC; }
        }

        private bool _VisibilityName = true;
        public bool VisibilityName
        {
            get
            {

                return _VisibilityName;
            }
            set
            {
                _VisibilityName = value;
                _VisibilityCode = !_VisibilityName;
                NotifyOfPropertyChange(() => VisibilityName);
                NotifyOfPropertyChange(() => VisibilityCode);

            }
        }

        private bool _VisibilityCode = false;
        public bool VisibilityCode
        {
            get
            {
                return _VisibilityCode;
            }
            set
            {
                if (_VisibilityCode != value)
                {
                    _VisibilityCode = value;
                    NotifyOfPropertyChange(() => VisibilityCode);
                }
            }
        }
        #endregion

        #region 3. Function Member

        private void GetListStaffFilter()
        {
            if (ListStaff != null)
            {
                ListStaff.Clear();
                Staff item = new Staff();
                item.StaffID = -1;
                item.FullName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                ListStaff.Add(item);
                ListStaff.Add(GetStaffLogin());
            }

            if (RequestDrug != null && RequestDrug.ReqOutwardDetails != null && RequestDrug.ReqOutwardDetails.Count > 0)
            {
                var lst = (from e in RequestDrug.ReqOutwardDetails
                           select new { e.StaffID, e.StaffName }).Distinct();
                foreach (var staffItem in lst)
                {
                    if (staffItem.StaffID != GetStaffLogin().StaffID)
                    {
                        ListStaff.Add(new Staff { StaffID = staffItem.StaffID.GetValueOrDefault(0), FullName = staffItem.StaffName });
                    }
                }
            }
            StaffDetailID = ListStaff.FirstOrDefault().StaffID; //GetStaffLogin().StaffID;
        }

        private IEnumerator<IResult> DoGetRefGenericDrugCategory_1List()
        {
            var paymentTypeTask = new LoadRefGenericDrugCategory_1ListTask(V_MedProductType, false, true);
            yield return paymentTypeTask;
            RefGenericDrugCategory_1s = paymentTypeTask.RefGenericDrugCategory_1List;
            SetDefultRefGenericDrugCategory();
            yield break;
        }
        private void SetDefultRefGenericDrugCategory()
        {
            if (RequestDrug != null && RefGenericDrugCategory_1s != null)
            {
                RequestDrug.RefGenDrugCatID_1 = RefGenericDrugCategory_1s.FirstOrDefault().RefGenDrugCatID_1;
            }
        }
        private Staff GetStaffLogin()
        {
            return Globals.LoggedUserAccount.Staff;
        }

        private void ischanged(object item)
        {
            ReqOutwardDrugClinicDeptPatient p = item as ReqOutwardDrugClinicDeptPatient;
            if (p != null)
            {
                if (p.EntityState == EntityState.PERSITED)
                {
                    p.EntityState = EntityState.MODIFIED;
                }
            }
        }

        private bool CheckValidationEditor()
        {
            bool result = true;
            StringBuilder st = new StringBuilder();
            if (RequestDrug != null)
            {
                if (RequestDrug.ReqOutwardDetails != null)
                {
                    int nIdx = 0;
                    foreach (ReqOutwardDrugClinicDeptPatient item in RequestDrug.ReqOutwardDetails)
                    {
                        nIdx++;
                        if (item.GenMedProductID != null && item.GenMedProductID != 0)
                        {
                            if (item.ReqQty < 0)
                            {
                                string strErr = "Dữ liệu dòng số (" + item.DisplayGridRowNumber.ToString() + ") : [" + item.RefGenericDrugDetail.BrandNameAndCode + "] Số lượng yêu cầu phải >= 0";
                                st.AppendLine(strErr);
                                result = false;
                            }
                            if (item.PrescribedQty <= 0)
                            {
                                string strErr = "Dữ liệu dòng số (" + item.DisplayGridRowNumber.ToString() + ") : [" + item.RefGenericDrugDetail.BrandNameAndCode + "] Số lượng Chỉ Định phải > 0";
                                st.AppendLine(strErr);
                                result = false;
                            }
                        }
                    }
                }
                if (!result)
                {
                    MessageBox.Show(st.ToString());
                }
            }
            return result;
        }

        private void FullOperatorRequestDrugInwardClinicDept()
        {
            // TxD28/04/2015: Added the following verification checkbox to aid in the process of creating a new list based on an existing one
            //                  It helps the user to identify if a row has been verified against the doctor's request document
            if (RequestDrug.ReqOutwardDetails != null && RequestDrug.ReqOutwardDetails.Count > 0)
            {
                foreach (ReqOutwardDrugClinicDeptPatient item in RequestDrug.ReqOutwardDetails)
                {
                    if (item.ItemVerified)
                    {
                        item.ItemVerfStat = 1;
                    }
                    else
                    {
                        item.ItemVerfStat = 0;
                    }
                }
            }

            this.ShowBusyIndicator();
            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginFullOperatorRequestDrugInwardClinicDeptNew(RequestDrug, V_MedProductType, (long)AllLookupValues.RegistrationType.NOI_TRU
                            , Globals.DispatchCallback((asyncResult) =>
                        {

                            try
                            {
                                RequestDrugInwardClinicDept RequestOut;
                                contract.EndFullOperatorRequestDrugInwardClinicDeptNew(out RequestOut, asyncResult);

                                RequestDrug = RequestOut;
                                RequestDrug.ReqOutwardDetails = RequestOut.ReqOutwardDetails.DeepCopy();

                                // TxD28/04/2015: Added the following verification checkbox to aid in the process of creating a new list based on an existing one
                                //                  It helps the user to identify if a row has been verified against the doctor's request document
                                if (RequestDrug.ReqOutwardDetails != null && RequestDrug.ReqOutwardDetails.Count > 0)
                                {
                                    foreach (ReqOutwardDrugClinicDeptPatient item in RequestDrug.ReqOutwardDetails)
                                    {
                                        item.ItemVerified = false;
                                        if (item.ItemVerfStat == 1)
                                        {
                                            item.ItemVerified = true;
                                        }
                                    }
                                }

                                
                                ListRequestDrugDelete.Clear();
                                //FilterByStaff();
                                FillPagedCollectionAndGroup();
                                Globals.ShowMessage(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao);
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                ClientLoggerHelper.LogError(ex.ToString());
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }

                        }), null);

                    }

                } );
            
                t.Start();

            }
            catch (Exception ex)
            {
                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                ClientLoggerHelper.LogError(ex.ToString());
                this.HideBusyIndicator();
            }
        }

        private void SaveRequest()
        {
            if (RequestDrug.ReqDrugInClinicDeptID > 0)
            {
                if (MessageBox.Show(eHCMSResources.A0138_G1_Msg_ConfLuuThayDoiTrenPhYC, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
            
            SaveFullOp();
            
        }

        private bool ValidateDataOneLastTimeBeforeSaving()
        {
            if (RequestDrug.OutFromStoreObject == null || RequestDrug.OutFromStoreObject.StoreID <= 0)
            {
                MessageBox.Show(eHCMSResources.K0318_G1_ChonKhoCC);
                return false;
            }
            if (RequestDrug.ReqOutwardDetails == null || RequestDrug.ReqOutwardDetails.Count == 0)
            {
                MessageBox.Show(eHCMSResources.K0422_G1_NhapCTietPhYC);
                return false;
            }
            else
            {
                if (RequestDrug.ReqOutwardDetails.Count == 1)
                {
                    if (RequestDrug.ReqOutwardDetails[0].RefGenericDrugDetail == null)
                    {
                        MessageBox.Show(eHCMSResources.K0303_G1_ChonHgYC);
                        return false;
                    }
                }

                for (int i = 0; i < RequestDrug.ReqOutwardDetails.Count; i++)
                {
                    if (RequestDrug.ReqOutwardDetails[i].RefGenericDrugDetail != null)
                    {
                        if (RequestDrug.ReqOutwardDetails[i].EntityState != EntityState.DETACHED && RequestDrug.ReqOutwardDetails[i].PrescribedQty <= 0)
                        {
                            string strErr = "Dữ liệu dòng số (" + RequestDrug.ReqOutwardDetails[i].DisplayGridRowNumber.ToString() + ") : [" + RequestDrug.ReqOutwardDetails[i].RefGenericDrugDetail.BrandNameAndCode + "] Số lượng Chỉ Định phải > 0";
                            MessageBox.Show(strErr);
                            return false;
                        }
                        if (RequestDrug.ReqOutwardDetails[i].EntityState != EntityState.DETACHED && RequestDrug.ReqOutwardDetails[i].ReqQty < 0)
                        {
                            string strErr = "Dữ liệu dòng số (" + RequestDrug.ReqOutwardDetails[i].DisplayGridRowNumber.ToString() + ") : [" + RequestDrug.ReqOutwardDetails[i].RefGenericDrugDetail.BrandNameAndCode + "] Số lượng yêu cầu phải >= 0";
                            MessageBox.Show(strErr);
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private void SaveFullOp()
        {
            if (ListRequestDrugDelete.Count > 0)
            {
                if (RequestDrug.ReqOutwardDetails == null)
                {
                    RequestDrug.ReqOutwardDetails = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
                }
                foreach (ReqOutwardDrugClinicDeptPatient item in ListRequestDrugDelete)
                {
                    RequestDrug.ReqOutwardDetails.Add(item);
                }
            }

            if (ValidateDataOneLastTimeBeforeSaving())
            {
                FullOperatorRequestDrugInwardClinicDept();
            }
        }

        private void RemoveRowBlank()
        {
            try
            {
                int idx = RequestDrug.ReqOutwardDetails.Count;
                if (idx > 0)
                {
                    idx--;
                    ReqOutwardDrugClinicDeptPatient obj = (ReqOutwardDrugClinicDeptPatient)RequestDrug.ReqOutwardDetails[idx];
                    if (obj.GenMedProductID == null || obj.GenMedProductID == 0)
                    {
                        RequestDrug.ReqOutwardDetails.RemoveAt(idx);
                    }
                }
                NotifyOfPropertyChange(() => RequestDrug);
            }
            catch
            { }
        }

        private void RemoveMark(object item)
        {
            ReqOutwardDrugClinicDeptPatient obj = item as ReqOutwardDrugClinicDeptPatient;
            if (obj != null)
            {
                RequestDrug.ReqOutwardDetails.Remove(obj);
                if (obj.EntityState == EntityState.PERSITED || obj.EntityState == EntityState.MODIFIED)
                {
                    obj.EntityState = EntityState.DETACHED;
                    ListRequestDrugDelete.Add(obj);
                }
            }
        }

        public void OpenPopUpSearchRequestInvoice(IList<RequestDrugInwardClinicDept> results, int Totalcount, bool bCreateNewListFromOld)
        {
            Action<IStoreDeptRequestSearch> onInitDlg = delegate (IStoreDeptRequestSearch proAlloc)
            {
                proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                proAlloc.V_MedProductType = V_MedProductType;
                proAlloc.RequestDruglist.Clear();
                proAlloc.RequestDruglist.TotalItemCount = Totalcount;
                proAlloc.RequestDruglist.PageIndex = 0;

                proAlloc.IsCreateNewListFromSelectExisting = bCreateNewListFromOld;

                if (results != null && results.Count > 0)
                {
                    foreach (RequestDrugInwardClinicDept p in results)
                    {
                        proAlloc.RequestDruglist.Add(p);
                    }
                }
            };
            GlobalsNAV.ShowDialog<IStoreDeptRequestSearch>(onInitDlg);
        }


        private void SearchRequestDrugInwardClinicDept(int PageIndex, int PageSize, bool bCreateNewListFromOld = false)
        {
            if (SearchCriteria == null)
            {
                return;
            }

            this.ShowBusyIndicator();

            if (string.IsNullOrEmpty(SearchCriteria.Code))
            {
                SearchCriteria.FromDate = Globals.GetCurServerDateTime().AddDays(-7);
                SearchCriteria.ToDate = Globals.GetCurServerDateTime();
            }
            else
            {
                SearchCriteria.FromDate = null;
                SearchCriteria.ToDate = null;
            }

            // TxD 23/12/2014: Filter search of requests by the requesting 
            if (RequestDrug != null && RequestDrug.InDeptStoreID > 0)
            {
                SearchCriteria.RequestStoreID = RequestDrug.InDeptStoreID;
            }
            else
            {
                if (StoreCbxStaff.Count() > 0)
                {
                    SearchCriteria.RequestStoreID = StoreCbxStaff[1].StoreID;
                }
            }

            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSearchRequestDrugInwardClinicDept(SearchCriteria, V_MedProductType, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int Total = 0;
                                var results = contract.EndSearchRequestDrugInwardClinicDept(out Total, asyncResult);
                                if (results != null && results.Count > 0)
                                {
                                    if (results.Count > 1)
                                    {
                                        //mo pop up tim
                                        OpenPopUpSearchRequestInvoice(results.ToList(), Total, bCreateNewListFromOld);

                                    }
                                    else
                                    {
                                        if (RequestDrug != null)
                                        {
                                            ChangeValue(RequestDrug.RefGenDrugCatID_1, results.FirstOrDefault().RefGenDrugCatID_1);
                                        }
                                        RequestDrug = results.FirstOrDefault();
                                        ListRequestDrugDelete.Clear();

                                        GetInPatientRequestingDrugListByReqID(RequestDrug.ReqDrugInClinicDeptID, bCreateNewListFromOld);

                                    }
                                }
                                else
                                {
                                    if (MessageBox.Show(eHCMSResources.Z0568_G1_KgTimThay, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                    {
                                        SearchCriteria.FromDate = Globals.GetCurServerDateTime().AddDays(-7);
                                        SearchCriteria.ToDate = Globals.GetCurServerDateTime();
                                        OpenPopUpSearchRequestInvoice(null, 0, bCreateNewListFromOld);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                ClientLoggerHelper.LogError(ex.ToString());
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
            catch (Exception ex)
            {
                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                ClientLoggerHelper.LogError(ex.ToString());
                this.HideBusyIndicator();
            }

        }

        //private ObservableCollection<ReqOutwardDrugClinicDeptPatient> ReqOutwardDrugClinicDeptPatientlstCopy;
        
        private void GetInPatientRequestingDrugListByReqID(long RequestID, bool bCreateNewListFromOld = false)
        {
            this.ShowBusyIndicator();
            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetReqOutwardDrugClinicDeptPatientByID(RequestID, bCreateNewListFromOld, (long)AllLookupValues.RegistrationType.NOI_TRU
                            , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetReqOutwardDrugClinicDeptPatientByID(asyncResult);
                                if (results != null)
                                {
                                    RequestDrug.ReqOutwardDetails = results.ToObservableCollection();
                                }
                                else
                                {
                                    if (RequestDrug.ReqOutwardDetails != null)
                                    {
                                        RequestDrug.ReqOutwardDetails.Clear();
                                    }
                                }
                                if (RequestDrug == null)
                                {
                                    RequestDrug = new RequestDrugInwardClinicDept();
                                }

                                // TxD 28/04/2015: The following line looks very bogus, so comment it out for now, if something happend then we need to REVIEW
                                //RequestDrug.ReqOutwardDetails = RequestDrug.ReqOutwardDetails.DeepCopy();

                                GetListStaffFilter();
                                FillPagedCollectionAndGroup();
                                if (bCreateNewListFromOld)
                                {
                                    ResetingOldListToCreateNewList(bCreateNewListFromOld);
                                }
                                else
                                {
                                    // TxD28/04/2015: Added the following verification checkbox to aid in the process of creating a new list based on an existing one
                                    //                  It helps the user to identify if a row has been verified against the doctor's request document
                                    if (RequestDrug.ReqOutwardDetails != null && RequestDrug.ReqOutwardDetails.Count > 0)
                                    {
                                        foreach (ReqOutwardDrugClinicDeptPatient item in RequestDrug.ReqOutwardDetails)
                                        {
                                            item.ItemVerified = false;
                                            if (item.ItemVerfStat == 1)
                                            {
                                                item.ItemVerified = true;
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                ClientLoggerHelper.LogError(ex.ToString());
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
            catch (Exception ex)
            {
                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                ClientLoggerHelper.LogError(ex.ToString());
                this.HideBusyIndicator();
            }
        }

        private void ResetingOldListToCreateNewList(bool bCreateNewListFromOld = false)
        {
            CurrentPatient = null;
            RequestDrug.ReqDrugInClinicDeptID = 0;
            RequestDrug.ReqNumCode = "";
            RequestDrug.Comment = "";
            RequestDrug.ReqDate = DateTime.Now;
            RequestDrug.FromDate = DateTime.Now;
            RequestDrug.ToDate = DateTime.Now;
            RequestDrug.SelectedStaff = GetStaffLogin();

            RequestDrug.DaNhanHang = false;
            RequestDrug.IsApproved = false;
            ListRequestDrugDelete.Clear();

            // TxD 14/01/2015: Commented out the following to allow Staff filtering to work properly
            //ReqOutwardDrugClinicDeptPatientlstCopy = null;
            
            if (!bCreateNewListFromOld && StoreCbxStaff != null)
            {
                RequestDrug.InDeptStoreObject = StoreCbxStaff.FirstOrDefault();
            }
            //KMx: Không set lại, để user khỏi phải chọn lại (06/12/2014 14:43).
            //if (StoreCbx != null)
            //{
            //    RequestDrug.OutFromStoreObject = StoreCbx.FirstOrDefault();
            //}

            // TxD28/04/2015: Added the following verification checkbox to aid in the process of creating a new list based on an existing one
            //                  It helps the user to identify if a row has been verified against the doctor's request document
            if (RequestDrug.ReqOutwardDetails != null && RequestDrug.ReqOutwardDetails.Count > 0)
            {
                foreach (ReqOutwardDrugClinicDeptPatient item in RequestDrug.ReqOutwardDetails)
                {
                    item.EntityState = EntityState.NEW;
                    item.ItemVerfStat = 0;
                }
            }

            //KMx: Không tự động set Category, vì khi set Category thì sẽ tự động gọi hàm KeyEnabledComboBox_SelectionChanged(), dẫn đến thuốc trong grid bị clear hết (06/12/2014 14:32).
            //SetDefultRefGenericDrugCategory();
        }

        private void RefeshRequest()
        {
            CurrentPatient = null;
            RequestDrug.ReqDrugInClinicDeptID = 0;
            RequestDrug.ReqNumCode = "";
            RequestDrug.Comment = "";
            RequestDrug.ReqDate = DateTime.Now;
            RequestDrug.FromDate = DateTime.Now;
            RequestDrug.ToDate = DateTime.Now;
            RequestDrug.SelectedStaff = GetStaffLogin();
            RequestDrug.ReqOutwardDetails = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
            RequestDrug.DaNhanHang = false;
            RequestDrug.IsApproved = false;

            ListRequestDrugDelete.Clear();
            
            if (StoreCbxStaff != null)
            {
                RequestDrug.InDeptStoreObject = StoreCbxStaff.FirstOrDefault();
            }
            if (StoreCbx != null)
            {
                RequestDrug.OutFromStoreObject = StoreCbx.FirstOrDefault();
            }
            SetDefultRefGenericDrugCategory();
            FillPagedCollectionAndGroup();

        }

        private void DeleteRequestDrugInwardClinicDept(long ReqDrugInClinicDeptID)
        {
            this.ShowBusyIndicator();
            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeleteRequestDrugInwardClinicDept(ReqDrugInClinicDeptID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndDeleteRequestDrugInwardClinicDept(asyncResult);
                                RefeshRequest();
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                ClientLoggerHelper.LogError(ex.ToString());
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
            catch (Exception ex)
            {
                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                ClientLoggerHelper.LogError(ex.ToString());
                this.HideBusyIndicator();
            }
        }

        private void DeleteRequest()
        {
            if (RequestDrug.ReqDrugInClinicDeptID > 0)
            {
                DeleteRequestDrugInwardClinicDept(RequestDrug.ReqDrugInClinicDeptID);
            }
        }

        private bool CheckDeleted(object item)
        {
            ReqOutwardDrugClinicDeptPatient temp = item as ReqOutwardDrugClinicDeptPatient;
            if (temp != null && temp.EntityState == EntityState.DETACHED)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckValidGrid()
        {            
            bool results = true;
            if (RequestDrug.InDeptStoreID == null || RequestDrug.InDeptStoreID == 0)
            {
                Globals.ShowMessage(eHCMSResources.Z1129_G1_ChonKhoYC, eHCMSResources.T0074_G1_I);
                return false;
            }
            if (RequestDrug.OutFromStoreID == null || RequestDrug.OutFromStoreID == 0)
            {
                Globals.ShowMessage(eHCMSResources.Z1130_G1_ChonKhoCCap, eHCMSResources.T0074_G1_I);
                return false;
            }

            if (RequestDrug.InDeptStoreID == RequestDrug.OutFromStoreID)
            {
                Globals.ShowMessage(eHCMSResources.Z1131_G1_KhoYCKhoCCKgDcTrung, eHCMSResources.T0074_G1_I);
                return false;
            }

            if (RequestDrug.ReqOutwardDetails != null)
            {
                if (RequestDrug.ReqOutwardDetails.Count == 0)
                {
                    if (RequestDrug.ReqDrugInClinicDeptID > 0)
                    {
                        if (MessageBox.Show(eHCMSResources.A0922_G1_Msg_ConfXoaPhRong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            DeleteRequest();
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show(eHCMSResources.K0443_G1_NhapSLggYC);
                        return false;
                    }
                }

                for (int i = 0; i < RequestDrug.ReqOutwardDetails.Count; i++)
                {
                    if (RequestDrug.ReqOutwardDetails[i].GenMedProductID > 0)
                    {
                        if (RequestDrug.ReqOutwardDetails[i].PrescribedQty <= 0)
                        {
                            results = false;
                            MessageBox.Show("Dữ liệu dòng số (" + RequestDrug.ReqOutwardDetails[i].DisplayGridRowNumber.ToString() + ") : [" + RequestDrug.ReqOutwardDetails[i].RefGenericDrugDetail.BrandNameAndCode + "] không hợp lệ." + Environment.NewLine + "Số lượng Chỉ Định phải > 0");
                            break;
                        }
                        if (RequestDrug.ReqOutwardDetails[i].ReqQty < 0)
                        {
                            results = false;
                            MessageBox.Show(string.Format(eHCMSResources.Z1124_G1_SLgYCLonHonBang0, RequestDrug.ReqOutwardDetails[i].DisplayGridRowNumber.ToString(), RequestDrug.ReqOutwardDetails[i].RefGenericDrugDetail.BrandNameAndCode));
                            break;
                        }
                    }
                }
            }

            return results;
        }

        private IEnumerator<IResult> DoGetStore_MedDept()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT, false, null, false, true);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList;
            if (RequestDrug != null && StoreCbx != null)
            {
                RequestDrug.OutFromStoreObject = StoreCbx.FirstOrDefault();
            }
            yield break;
        }
        private IEnumerator<IResult> DoGetStore_All()
        {
            var paymentTypeTask = new LoadStoreListTask(null, false, null, false, true);
            yield return paymentTypeTask;
            StoreCbxStaff = paymentTypeTask.LookupList;
            yield break;
        }

        private void GetStorageByStaffID(long StaffID)
        {
            this.ShowBusyIndicator();
            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyStoragesServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetStoragesByStaffID(StaffID, null, Globals.DispatchCallback((asyncResult) =>
                        {

                            try
                            {
                                var results = contract.EndGetStoragesByStaffID(asyncResult);
                                if (results != null)
                                {
                                    StoreCbxStaff = results.ToObservableCollection();
                                    if (RequestDrug != null)
                                    {
                                        RequestDrug.InDeptStoreObject = StoreCbxStaff.FirstOrDefault();
                                    }
                                }
                                else
                                {
                                    if (StoreCbxStaff != null)
                                    {
                                        StoreCbxStaff.Clear();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                ClientLoggerHelper.LogError(ex.ToString());
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
            catch (Exception ex)
            {
                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                ClientLoggerHelper.LogError(ex.ToString());
                this.HideBusyIndicator();
            }
        }

        #endregion

        #region client member

        #region AutoGenMedProduct Member
        private string BrandName;

        private void GetRefGenMedProductDetails_Auto(string BrandName, int PageIndex, int PageSize)
        {
            if (IsCode == false && BrandName.Length < 1)
            {
                return;
            }

            long? RefGenDrugCatID_1 = null;
            if (RequestDrug != null)
            {
                RefGenDrugCatID_1 = RequestDrug.RefGenDrugCatID_1;
            }
            //IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginRefGenMedProductDetails_SearchAutoPaging(IsCode, BrandName, null, V_MedProductType, RefGenDrugCatID_1, PageSize, PageIndex, null, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int Total;
                            var results = contract.EndRefGenMedProductDetails_SearchAutoPaging(out Total, asyncResult);
                            if (IsCode.GetValueOrDefault())
                            {
                                if (results != null && results.Count > 0)
                                {
                                    if (CurrentReqOutwardDrugClinicDeptPatient == null)
                                    {
                                        CurrentReqOutwardDrugClinicDeptPatient = new ReqOutwardDrugClinicDeptPatient();
                                    }
                                    CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = results.FirstOrDefault();
                                }
                                else
                                {
                                   
                                    if (CurrentReqOutwardDrugClinicDeptPatient != null)
                                    {
                                        CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = null;
                                    }

                                    if (tbx != null)
                                    {
                                        txt = "";
                                        tbx.Text = "";
                                    }
                                    if (au != null)
                                    {
                                        au.Text = "";
                                    }

                                    MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                                }
                            }
                            else
                            {
                                RefGenMedProductDetails.Clear();
                                RefGenMedProductDetails.TotalItemCount = Total;
                                RefGenMedProductDetails.SourceCollection = results;
                                //foreach (RefGenMedProductDetails p in results)
                                //{
                                //    RefGenMedProductDetails.Add(p);
                                //}
                                au.PopulateComplete();
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //IsLoading = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }
        


        #endregion

        private bool _CheckUnVerifiedRows = false;
        public bool CheckUnVerifiedRows
        {
            get { return _CheckUnVerifiedRows; }
            set
            {
                _CheckUnVerifiedRows = value;
                NotifyOfPropertyChange(() => CheckUnVerifiedRows);
            }
        }

        public void grdReqOutwardDetails_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            ReqOutwardDrugClinicDeptPatient rowItem = e.Row.DataContext as ReqOutwardDrugClinicDeptPatient;
            rowItem.DisplayGridRowNumber = e.Row.GetIndex() + 1;

            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";

            // TxD 20/04/2015: No need to do this anymore becuase when we save those deleted row will be added back into the main collection 
            //                  causing a flash of red painted row(s) on the screen of those rows that were deleted.
            //if (CheckDeleted(e.Row.DataContext))
            //{
            //    e.Row.Background = new SolidColorBrush(Colors.Red);
            //}

            if (CheckUnVerifiedRows)
            {
                if (!rowItem.ItemVerified)
                {
                    e.Row.Background = new SolidColorBrush(Colors.Yellow);
                }
                else
                {
                    e.Row.Background = new SolidColorBrush(Colors.White);
                }
            }
        }

        public void ItemVerify_Checked(object source, object dataCtx)
        {
            if (dataCtx != null)
            {
                ischanged(dataCtx);
            }
        }

        public void ItemVerify_UnChecked(object source, object dataCtx)
        {            
            if (dataCtx != null)
            {                
                ischanged(dataCtx);
            }
        }

        public void grdReqOutwardDetails_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if ((sender as DataGrid).SelectedItem != null)
            {
                ischanged((sender as DataGrid).SelectedItem);
            }
            // TxD 03/04/2015 if PrescribedQty is changed then ReqQty will be changed AS WELL (As requested by Khoa A & NTM)
            if (e.Column.DisplayIndex == 5)
            {
                if (RequestDrug != null && RequestDrug.ReqOutwardDetails != null)
                {
                    ReqOutwardDrugClinicDeptPatient selItem = RequestDrug.ReqOutwardDetails[(sender as DataGrid).SelectedIndex];
                    selItem.ReqQty = selItem.PrescribedQty;
                }
            }
        }

        public void grdReqOutwardDetails_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (!CheckValidationEditor())
            {    
                // TxD 26/03/2015: get rid of the following because if this Event is Cancel the the Row is still in Edit mode thus prevent the grid from calling Refresh
                //                  and at thie moment there is no need to cancel unless anything comes up later then review                                
                //e.Cancel = true;
            }
        }

        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (((ClinicDeptInPtReqFormView)this.GetView()).grdReqOutwardDetails != null)
            {
                if (MessageBox.Show("Bạn có chắc muốn xóa dòng dữ liệu này không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    RemoveMark(((ClinicDeptInPtReqFormView)this.GetView()).grdReqOutwardDetails.SelectedItem);
                }
            }
        }

    public DataGridRow GetDataGridRowByDataContext(DataGrid dataGrid, object dataContext)
    {
            //ChangedWPF-CMN
    //if (null != dataContext)
    //{
    //    dataGrid.ScrollIntoView(dataContext, null);

    //    System.Windows.Automation.Peers.DataGridAutomationPeer automationPeer = (System.Windows.Automation.Peers.DataGridAutomationPeer)System.Windows.Automation.Peers.DataGridAutomationPeer.CreatePeerForElement(dataGrid);

    //    // Get the DataGridRowsPresenterAutomationPeer so we can find the rows in the data grid...
    //    System.Windows.Automation.Peers.DataGridRowsPresenterAutomationPeer dataGridRowsPresenterAutomationPeer = automationPeer.GetChildren().
    //        Where(a => (a is System.Windows.Automation.Peers.DataGridRowsPresenterAutomationPeer)).
    //        Select(a => (a as System.Windows.Automation.Peers.DataGridRowsPresenterAutomationPeer)).
    //        FirstOrDefault();

    //    if (null != dataGridRowsPresenterAutomationPeer)
    //    {
    //        foreach (var item in dataGridRowsPresenterAutomationPeer.GetChildren())
    //        {
    //            // loop to find the DataGridCellAutomationPeer from which we can interrogate the owner -- which is a DataGridRow
    //            foreach (var subitem in (item as System.Windows.Automation.Peers.DataGridItemAutomationPeer).GetChildren())
    //            {
    //                if ((subitem is System.Windows.Automation.Peers.DataGridCellAutomationPeer))
    //                {
    //                    // At last -- the only public method for finding a row....
    //                    DataGridRow row = DataGridRow.GetRowContainingElement(((subitem as System.Windows.Automation.Peers.DataGridCellAutomationPeer).Owner as FrameworkElement));

    //                    // check this row to see if it is bound to the requested dataContext.
    //                    if ((row.DataContext) == dataContext)
    //                    {
    //                        return row;
    //                    }

    //                    break; // Only need to check one cell in each row
    //                }
    //            }
    //        }
    //    }
    //}

        return null;
    }

        public void btnSave(object sender, RoutedEventArgs e)
        {
            if (((ClinicDeptInPtReqFormView)this.GetView()).grdReqOutwardDetails != null && ((ClinicDeptInPtReqFormView)this.GetView()).grdReqOutwardDetails.IsValid)
            {
                if (CheckValidGrid())
                {
                    SaveRequest();
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0539_G1_Msg_InfoDataKhDung);
            }
        }

        public void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.Code = (sender as TextBox).Text;
                }
                btnFindRequest();
            }
        }

        public void btnFindRequest()
        {
            SearchRequestDrugInwardClinicDept(0, Globals.PageSize);
        }

        //Huyen_Update: 05/08/2015

        public void GetRemainingQtyForInPtRequestDrug()
        {
            this.ShowBusyIndicator();
            List<ReqOutwardDrugClinicDeptPatient> RemainingList = null;
            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRemainingQtyForInPtRequestDrug(RequestDrug.InDeptStoreID, V_MedProductType, RequestDrug.RefGenDrugCatID_1, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                RemainingList = contract.EndGetRemainingQtyForInPtRequestDrug(asyncResult);
                                display(RemainingList);
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                ClientLoggerHelper.LogError(ex.ToString());
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
            catch (Exception ex)
            {
                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                ClientLoggerHelper.LogError(ex.ToString());
                this.HideBusyIndicator();
            }
        }

        public void display(List<ReqOutwardDrugClinicDeptPatient> RemainingList)
        {
            // TxD
            //List<ReqOutwardDrugClinicDeptPatient> newList = RequestDrug.ReqOutwardDetails.GroupBy(x => x.GenMedProductID).Select
            //    (x => new { GenMedProductID = x.Key }
            //    );
            



            foreach (var a in RequestDrug.ReqOutwardDetails)
            {
                foreach (var b in RemainingList)
                {
                    if (a.GenMedProductID == b.GenMedProductID)
                    {
                        a.RemainingQty = b.RemainingQty;
                        break;
                    }
                }
            }
            
            Action<IModify_ReqQty> onInitDlg = delegate (IModify_ReqQty temp)
            {
                // Tạo list sao chép danh sách thuốc từ phiếu yêu cầu để thực hiện cộng dồn số lượng những thuốc có ID giống nhau
                //Không dùng observableCollection do không biết cách thực hiện vòng for để cộng dồn
                List<ReqOutwardDrugClinicDeptPatient> RequestDetails = new List<ReqOutwardDrugClinicDeptPatient>();






                RequestDetails = RequestDrug.ReqOutwardDetails.ToList<ReqOutwardDrugClinicDeptPatient>();
                for (int i = 0; i < RequestDetails.Count; i++)
                {
                    for (int j = 0; j < RequestDetails.Count; j++)
                    {
                        if (RequestDetails[i].GenMedProductID == RequestDetails[j].GenMedProductID && i != j)
                        {
                            RequestDetails[i].PrescribedQty += RequestDetails[j].PrescribedQty;
                            RequestDetails[i].ReqQty += RequestDetails[j].ReqQty;
                            RequestDetails.Remove(RequestDetails[j]);
                        }
                    }
                }
                ObservableCollection<ReqOutwardDrugClinicDeptPatient> RequestDrugDetails = RequestDetails.ToObservableCollection<ReqOutwardDrugClinicDeptPatient>();
                //Huyen 07/08/2015: gán giá trị cho các biến được khai báo trong Interface IModify_ReqQty
                temp.RequestDrug.ReqOutwardDetails = RequestDrugDetails;//Lấy nội dung chi tiết phiếu yêu cầu để hiển thị trong View
            };
            GlobalsNAV.ShowDialog<IModify_ReqQty>(onInitDlg);
        }
        public void btnAutoReqQty(object sender, RoutedEventArgs e)
        {    
           //Huyen: Show Drug list of current request form within remaining quantity
            GetRemainingQtyForInPtRequestDrug();        
        }

        //Huyen_End

        public void btnNew(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.A0146_G1_Msg_ConfTaoMoiPh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                RefeshRequest();
                hblPatientOther();
            }
        }

        public void btnCreateNewByOld(object sender, RoutedEventArgs e)
        {
            if (RequestDrug == null)
            {
                return;
            }

            if (MessageBox.Show(eHCMSResources.A0145_G1_Msg_ConfTaoMoiTuPhCu, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                return;
            }

            // TxD 23/12/2014: Added filtering by requesting warehouse storeID
            if (RequestDrug.InDeptStoreID == null || RequestDrug.InDeptStoreID == 0)
            {
                Globals.ShowMessage(eHCMSResources.Z1129_G1_ChonKhoYC, eHCMSResources.T0074_G1_I);
                return;
            }
            SearchCriteria = new RequestSearchCriteria();

            hblPatientOther();

            SearchRequestDrugInwardClinicDept(0, Globals.PageSize, true);

        }


        public void btnDelete(object sender, RoutedEventArgs e)
        {
            //call delete 
            if (MessageBox.Show(eHCMSResources.A0120_G1_Msg_ConfXoaPh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeleteRequest();
            }
        }

        #region printing member

        public void btnPreviewTH()
        {
            Action<IClinicDeptReportDocumentPreview> onInitDlg = delegate (IClinicDeptReportDocumentPreview proAlloc)
            {
                proAlloc.ID = RequestDrug.ReqDrugInClinicDeptID;
                if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                {
                    if (RequestDrug.RefGenDrugCatID_1 == (long)AllLookupValues.RefGenDrugCatID_1.GAYNGHIEN)
                    {
                        proAlloc.LyDo = "PHIẾU LĨNH THUỐC GÂY NGHIỆN";
                    }
                    else if (RequestDrug.RefGenDrugCatID_1 == (long)AllLookupValues.RefGenDrugCatID_1.HUONGTHAN)
                    {
                        proAlloc.LyDo = "PHIẾU LĨNH THUỐC HƯỚNG TÂM THẦN";
                    }
                    else
                    {
                        proAlloc.LyDo = "PHIẾU LĨNH THUỐC";
                    }
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                {
                    proAlloc.LyDo = "PHIẾU LĨNH Y CỤ";
                }
                else
                {
                    proAlloc.LyDo = "PHIẾU LĨNH HÓA CHẤT";
                }
                proAlloc.eItem = ReportName.DRUGDEPT_REQUEST;
            };
            GlobalsNAV.ShowDialog<IClinicDeptReportDocumentPreview>(onInitDlg);
        }

        public void btnPreviewCT()
        {
            Action<IClinicDeptReportDocumentPreview> onInitDlg = delegate (IClinicDeptReportDocumentPreview proAlloc)
            {
                proAlloc.ID = RequestDrug.ReqDrugInClinicDeptID;
                if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                {
                    if (RequestDrug.RefGenDrugCatID_1 == (long)AllLookupValues.RefGenDrugCatID_1.GAYNGHIEN)
                    {
                        proAlloc.LyDo = "PHIẾU LĨNH THUỐC GÂY NGHIỆN";
                    }
                    else if (RequestDrug.RefGenDrugCatID_1 == (long)AllLookupValues.RefGenDrugCatID_1.HUONGTHAN)
                    {
                        proAlloc.LyDo = "PHIẾU LĨNH THUỐC HƯỚNG TÂM THẦN";
                    }
                    else
                    {
                        proAlloc.LyDo = "PHIẾU LĨNH THUỐC";
                    }
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                {
                    proAlloc.LyDo = "PHIẾU LĨNH Y CỤ";
                }
                else
                {
                    proAlloc.LyDo = "PHIẾU LĨNH HÓA CHẤT";
                }
                proAlloc.eItem = ReportName.DRUGDEPT_REQUEST_DETAILS;
            };
            GlobalsNAV.ShowDialog<IClinicDeptReportDocumentPreview>(onInitDlg);
        }

        public void btnPrint()
        {
            this.ShowBusyIndicator();
            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new ReportServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetRequestPharmacyInPdfFormat(RequestDrug.ReqDrugInClinicDeptID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetRequestPharmacyInPdfFormat(asyncResult);
                                var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, results, ActiveXPrintType.ByteArray);
                                Globals.EventAggregator.Publish(results);
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                ClientLoggerHelper.LogError(ex.ToString());
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
            catch (Exception ex)
            {
                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                ClientLoggerHelper.LogError(ex.ToString());
                this.HideBusyIndicator();
            }
        }

        #endregion
        #endregion

        #region IHandle<DrugDeptCloseSearchRequestEvent> Members

        public void Handle(DrugDeptCloseSearchRequestEvent message)
        {
            if (message != null && this.IsActive)
            {
                RequestDrugInwardClinicDept item = message.SelectedRequest as RequestDrugInwardClinicDept;
                if (RequestDrug != null && item != null)
                {
                    ChangeValue(RequestDrug.RefGenDrugCatID_1, item.RefGenDrugCatID_1);
                }
                RequestDrug = item;
                ListRequestDrugDelete.Clear();

                GetInPatientRequestingDrugListByReqID(RequestDrug.ReqDrugInClinicDeptID, message.IsCreateNewFromExisting);

            }
        }

        #endregion

        bool? IsCode = false;
        public void Code_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            IsCode = true;
            VisibilityName = false;
        }

        public void Name_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            IsCode = false;
            VisibilityName = true;
        }

        public bool ListOutDrugReqFilter(object listObj)
        {
            ReqOutwardDrugClinicDeptPatient outItem = listObj as ReqOutwardDrugClinicDeptPatient;
            if (CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration != null && CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.Patient != null && CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.Patient.PatientID > 0)
            {
                if (outItem.CurPatientRegistration != null && outItem.CurPatientRegistration.Patient != null && outItem.CurPatientRegistration.Patient.PatientID > 0)
                {
                    return (outItem.CurPatientRegistration.Patient.PatientID == CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.Patient.PatientID);
                }
            }
            else
            {
                if (CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration == null && outItem.CurPatientRegistration == null)
                {
                    return true;
                }
            }
            return false;
        }

        private bool CurrentlyViewReqByStaffIDFilter = false;
        public bool ListRequestByStaffIDFilter(object listObj)
        {
            ReqOutwardDrugClinicDeptPatient outItem = listObj as ReqOutwardDrugClinicDeptPatient;

            if (StaffDetailID > 0)
            {
                if (outItem.StaffID == StaffDetailID)
                    return true;
            }
            else
            {
                return true;
            }

            return false;

        }

        public bool _usedForRequestingDrug = false;
        public bool UsedForRequestingDrug 
        {
            get { return _usedForRequestingDrug; }
            set
            {
                _usedForRequestingDrug = value;
            }
        }

        public void btnAddItem()
        {
            System.Diagnostics.Debug.WriteLine(" ========> btnAddItem  1 .....");

            CurrentReqOutwardDrugClinicDeptPatient.StaffID = GetStaffLogin().StaffID;

            if (UsedForRequestingDrug && RequestDrug != null && RequestDrug.RefGenDrugCatID_1 <= 0)
            {
                Globals.ShowMessage(eHCMSResources.Z1146_G1_ChonPhanNhomHg, eHCMSResources.G0442_G1_TBao);
                return;
            }

            // TxD 22/12/2014: Modify the following condition to check when item is blank or Code is blank 
            if (CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail == null || CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail.GenMedProductID == 0
                                    || string.IsNullOrEmpty(CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail.Code))
            {
                CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = new RefGenMedProductDetails();

                Globals.ShowMessage(eHCMSResources.Z1147_G1_ChonHgCanYC, eHCMSResources.G0442_G1_TBao);
                return;
            }

            if (CurrentReqOutwardDrugClinicDeptPatient.ReqQty <= 0)
            {
                Globals.ShowMessage(eHCMSResources.Z1161_G1_SLgCDinhKgHopLe, eHCMSResources.G0442_G1_TBao);
                return;
            }

            if (CalByUnitUse)
            {
                //KMx: Tính ra số lượng dựa vào DispenseVolume (15/11/2014 09:01).
                CurrentReqOutwardDrugClinicDeptPatient.ReqQty = CurrentReqOutwardDrugClinicDeptPatient.ReqQty / (CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail.DispenseVolume == 0 ? 1 : (decimal)CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail.DispenseVolume);
                CurrentReqOutwardDrugClinicDeptPatient.ReqQty = Math.Round(CurrentReqOutwardDrugClinicDeptPatient.ReqQty, 2);
            }

            CurrentReqOutwardDrugClinicDeptPatient.DateTimeSelection = Globals.GetCurServerDateTime();

            if (RequestDrug == null)
            {
                RequestDrug = new RequestDrugInwardClinicDept();
            }
            if (RequestDrug.ReqOutwardDetails == null)
            {
                RequestDrug.ReqOutwardDetails = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
            }
            var temp = RequestDrug.ReqOutwardDetails.Where(x => x.RefGenericDrugDetail != null && CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail != null && x.RefGenericDrugDetail.GenMedProductID == CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail.GenMedProductID && x.PtRegistrationID == CurrentReqOutwardDrugClinicDeptPatient.PtRegistrationID);
            if (temp != null && temp.Count() > 0)
            {
                temp.ToList()[0].PrescribedQty += CurrentReqOutwardDrugClinicDeptPatient.ReqQty;
                temp.ToList()[0].ReqQty += CurrentReqOutwardDrugClinicDeptPatient.ReqQty;
            }
            else
            {
                CurrentReqOutwardDrugClinicDeptPatient.EntityState = EntityState.NEW;
                // TxD 24/03/2015: Set PrescribedQty = ReqQty initially
                CurrentReqOutwardDrugClinicDeptPatient.PrescribedQty = CurrentReqOutwardDrugClinicDeptPatient.ReqQty;

                var item = CurrentReqOutwardDrugClinicDeptPatient.DeepCopy();
                RequestDrug.ReqOutwardDetails.Add(item);

                // TxD 16/11/2014: For a brand new List NOT YET SAVE we have to call the following once to define Grouping for the PagedCollectionView.
                if (RequestDrug.ReqOutwardDetails.Count == 1)
                {
                    FillPagedCollectionAndGroup();
                }
            }

            CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = new RefGenMedProductDetails();
            CurrentReqOutwardDrugClinicDeptPatient.ReqQty = 0;

            if (IsCode.GetValueOrDefault())
            {
                if (tbx != null)
                {
                    txt = "";
                    tbx.Text = "";
                    tbx.Focus();
                }
            }
            else
            {
                if (au != null)
                {
                    au.Text = "";
                    au.Focus();
                }
            }

            //KMx: Khi thêm thuốc chỉ cần refresh, không cần group lại, vì nều số lượng nhiều sẽ bị chậm (15/11/2014 17:48).
            if (ReqOutwardDrugClinicDeptPatientlst != null && ReqOutwardDrugClinicDeptPatientlst.Count > 0)
            {
                ReqOutwardDrugClinicDeptPatientlst.Refresh();
            }
            //FillPagedCollectionAndGroup();

            // TxD 19/11/2014: Create a Filter for PagedCollectionView if it does not exist yet.
            if (ReqOutwardDrugClinicDeptPatientlst != null && ReqOutwardDrugClinicDeptPatientlst.CanFilter
                           && (ReqOutwardDrugClinicDeptPatientlst.Filter == null || CurrentlyViewReqByStaffIDFilter))
            {
                ReqOutwardDrugClinicDeptPatientlst.Filter = new Predicate<object>(ListOutDrugReqFilter);
                CurrentlyViewReqByStaffIDFilter = false;
            }

        }

        private void FillPagedCollectionAndGroup()
        {
            if (RequestDrug != null && RequestDrug.ReqOutwardDetails != null)
            {
                ReqOutwardDrugClinicDeptPatientlst = new PagedCollectionView(RequestDrug.ReqOutwardDetails);
                FillGroupName();
            }
        }

        private void ChangeValue(long value1, long value2)
        {
            if (value1 != value2)
            {
                flag = false;
            }
            else
            {
                flag = true;
            }
        }

        private bool flag = true;
        
        public void KeyEnabledComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox bSel = (ComboBox)sender;
            if (bSel == null)
                return;

            if (flag)
            {
                if (RequestDrug != null && RequestDrug.ReqOutwardDetails.Count > 0)
                {
                    if (MessageBox.Show(eHCMSResources.A0993_G1_Msg_ConfDoiPhanNhomHg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    {
                        flag = false;
                        bSel.SelectedItem = e.RemovedItems[0];
                        return;
                    }
                }
            }
           
            if (flag)
            {
                if (RequestDrug != null && RequestDrug.ReqOutwardDetails != null)
                {
                    for (int i = 0; i < RequestDrug.ReqOutwardDetails.Count; i++)
                    {
                        if (RequestDrug.ReqOutwardDetails[i].EntityState == EntityState.PERSITED || RequestDrug.ReqOutwardDetails[i].EntityState == EntityState.MODIFIED)
                        {
                            RequestDrug.ReqOutwardDetails[i].EntityState = EntityState.DETACHED;
                            ListRequestDrugDelete.Add(RequestDrug.ReqOutwardDetails[i]);
                        }
                    }
                    RequestDrug.ReqOutwardDetails.Clear();                                        
                }
            }

            flag = true;
            if (bSel.IsDropDownOpen)
            {
                bSel.IsDropDownOpen = false;
            }

        }


        public void Handle(PatientSelectedGoToKhamBenh_InPt<PatientRegistration> message)
        {
            if (this.GetView() != null && message != null && message.Item != null)
            {
                CurrentPatient = message.Item.Patient;
                if (CurrentReqOutwardDrugClinicDeptPatient == null)
                {
                    CurrentReqOutwardDrugClinicDeptPatient = new ReqOutwardDrugClinicDeptPatient();
                }
                CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration = message.Item;
                CurrentReqOutwardDrugClinicDeptPatient.PtRegistrationID = message.Item.PtRegistrationID;

                // TxD 17/11/2014: Filtering here as well when a NEW Patient has been selected to see any existing items (if any) of that Patient
                if (ReqOutwardDrugClinicDeptPatientlst != null)
                {
                    if (ReqOutwardDrugClinicDeptPatientlst.Filter == null || CurrentlyViewReqByStaffIDFilter)
                    {
                        ReqOutwardDrugClinicDeptPatientlst.Filter = new Predicate<object>(ListOutDrugReqFilter);
                        CurrentlyViewReqByStaffIDFilter = false;
                    }

                    ReqOutwardDrugClinicDeptPatientlst.Refresh();
                }
            }
        }
        #region Checked All Member
        private bool _AllChecked;
        public bool AllChecked
        {
            get
            {
                return _AllChecked;
            }
            set
            {
                if (_AllChecked != value)
                {
                    _AllChecked = value;
                    NotifyOfPropertyChange(() => AllChecked);
                    if (_AllChecked)
                    {
                        AllCheckedfc();
                    }
                    else
                    {
                        UnCheckedfc();
                    }
                }
            }
        }

        private enum DataGridCol
        {
            ColMultiDelete = 0,
            ColDelete = 1,
            MaThuoc = 2,
            TenThuoc = 3
        }

        private void HideShowColumnDelete()
        {
            if (((ClinicDeptInPtReqFormView)this.GetView()).grdReqOutwardDetails != null)
            {
                if (RequestDrug.CanSave)
                {
                    ((ClinicDeptInPtReqFormView)this.GetView()).grdReqOutwardDetails.Columns[(int)DataGridCol.ColDelete].Visibility = Visibility.Visible;
                    ((ClinicDeptInPtReqFormView)this.GetView()).grdReqOutwardDetails.Columns[(int)DataGridCol.ColMultiDelete].Visibility = Visibility.Visible;
                }
                else
                {
                    ((ClinicDeptInPtReqFormView)this.GetView()).grdReqOutwardDetails.Columns[(int)DataGridCol.ColDelete].Visibility = Visibility.Collapsed;
                    ((ClinicDeptInPtReqFormView)this.GetView()).grdReqOutwardDetails.Columns[(int)DataGridCol.ColMultiDelete].Visibility = Visibility.Collapsed;
                }
            }
        }

        private void AllCheckedfc()
        {
            if (RequestDrug != null && RequestDrug.ReqOutwardDetails != null && RequestDrug.ReqOutwardDetails.Count > 0)
            {
                for (int i = 0; i < RequestDrug.ReqOutwardDetails.Count; i++)
                {
                    RequestDrug.ReqOutwardDetails[i].Checked = true;
                }
            }
        }

        private void UnCheckedfc()
        {
            if (RequestDrug != null && RequestDrug.ReqOutwardDetails != null && RequestDrug.ReqOutwardDetails.Count > 0)
            {
                for (int i = 0; i < RequestDrug.ReqOutwardDetails.Count; i++)
                {
                    RequestDrug.ReqOutwardDetails[i].Checked = false;
                }
            }
        }

        public void btnDeleteHang()
        {
            if (RequestDrug != null && RequestDrug.ReqOutwardDetails != null && RequestDrug.ReqOutwardDetails.Count > 0)
            {
                var items = RequestDrug.ReqOutwardDetails.Where(x => x.Checked == true);
                if (items != null && items.Count() > 0)
                {
                    if (MessageBox.Show("Bạn có chắc muốn xóa những hàng đã chọn không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        foreach (ReqOutwardDrugClinicDeptPatient obj in items)
                        {
                            if (obj.EntityState == EntityState.PERSITED || obj.EntityState == EntityState.MODIFIED)
                            {
                                obj.EntityState = EntityState.DETACHED;
                                ListRequestDrugDelete.Add(obj);
                            }
                        }
                        RequestDrug.ReqOutwardDetails = RequestDrug.ReqOutwardDetails.Where(x => x.Checked == false).ToObservableCollection();
                        FillPagedCollectionAndGroup();
                    }
                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0090_G1_Msg_InfoChuaChonHgCanXoa);
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0090_G1_Msg_InfoChuaChonHgCanXoa);
            }
        }
        #endregion

        private void FillGroupName()
        {
            if (ReqOutwardDrugClinicDeptPatientlst != null && ReqOutwardDrugClinicDeptPatientlst.Count > 0)
            {
                ReqOutwardDrugClinicDeptPatientlst.GroupDescriptions.Clear();
                ReqOutwardDrugClinicDeptPatientlst.SortDescriptions.Clear();
                ReqOutwardDrugClinicDeptPatientlst.GroupDescriptions.Add(new Common.PagedCollectionView.PropertyGroupDescription("CurPatientRegistration.Patient.PatientCodeAndName"));

                // TxD 17/11/2014: Commented out the following sort order because Filter has been applied to view and work with each Group at a time.
                //KMx: Sort theo thời gian thêm thuốc (15/11/2014 17:21).
                //ReqOutwardDrugClinicDeptPatientlst.SortDescriptions.Add(new System.ComponentModel.SortDescription("DateTimeSelection", System.ComponentModel.ListSortDirection.Descending));
                
                ReqOutwardDrugClinicDeptPatientlst.Filter = null;

            }
        }

        public void hblPatientOther()
        {
            CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration = null;
            CurrentReqOutwardDrugClinicDeptPatient.PtRegistrationID = 0;
            CurrentPatient = null;
            if (ReqOutwardDrugClinicDeptPatientlst != null && ReqOutwardDrugClinicDeptPatientlst.CanFilter)
            {
                ReqOutwardDrugClinicDeptPatientlst.Filter = new Predicate<object>(ListOutDrugReqFilter);
                CurrentlyViewReqByStaffIDFilter = false;
                ReqOutwardDrugClinicDeptPatientlst.Refresh();
            }
        }

        public void btnViewAll()
        {
            if (ReqOutwardDrugClinicDeptPatientlst != null && ReqOutwardDrugClinicDeptPatientlst.CanFilter)
            {
                ReqOutwardDrugClinicDeptPatientlst.Filter = null;
                ReqOutwardDrugClinicDeptPatientlst.Refresh();
                FillPagedCollectionAndGroup();
            }
        }

        private bool bAllGroupsAlreadyClosed = false;

        public void btnCloseOpenGroups()
        {
            bool bClose = false;
            if (!bAllGroupsAlreadyClosed)
            {
                bClose = true;                
            }
            
            CloseOrOpenGroups(bClose);
        }

        private void CloseOrOpenGroups(bool bClosed)
        {
            if (ReqOutwardDrugClinicDeptPatientlst == null || ReqOutwardDrugClinicDeptPatientlst.Groups == null || ReqOutwardDrugClinicDeptPatientlst.Groups.Count <= 0)
            {
                return;
            }

            bAllGroupsAlreadyClosed = bClosed;
            AxDataGridNy theGrid = ((ClinicDeptInPtReqFormView)this.GetView()).grdReqOutwardDetails;
            //ChangedWPF-CMN
            //foreach (Common.PagedCollectionView.CollectionViewGroup group in ReqOutwardDrugClinicDeptPatientlst.Groups)
            //{
            //    if (bClosed)
            //    {
            //        theGrid.CollapseRowGroup(group, true);
            //    }
            //    else
            //    {
            //        theGrid.ExpandRowGroup(group, true);
            //    }
            //}
        }

        TextBox tbx = null;
        public void AxTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            tbx = sender as TextBox;
        }

        string txt = "";

        public void AxTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txt != (sender as TextBox).Text)
            {
                txt = (sender as TextBox).Text;

                System.Diagnostics.Debug.WriteLine(" ====> AxTextBox_LostFocus 1 .....");

                if (!string.IsNullOrEmpty(txt))
                {
                    string Code = Globals.FormatCode(V_MedProductType, txt);

                    GetRefGenMedProductDetails_Auto(Code, 0, RefGenMedProductDetails.PageSize);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(" ====> AxTextBox_LostFocus 2 .....");
                    // TxD 23/12/2014 If Code is blank then clear selected item
                    CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = new RefGenMedProductDetails();
                }
            }
        }

        AutoCompleteBox au;
        public void AutoCompleteBox_Populating(object sender, PopulatingEventArgs e)
        {
            if (!IsCode.GetValueOrDefault())
            {
                au = sender as AutoCompleteBox;
                BrandName = e.Parameter;
                RefGenMedProductDetails.PageIndex = 0;
                GetRefGenMedProductDetails_Auto(e.Parameter, 0, RefGenMedProductDetails.PageSize);
            }
        }

        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            RefGenMedProductDetails obj = (sender as AutoCompleteBox).SelectedItem as RefGenMedProductDetails;
            //if (CurrentReqOutwardDrugClinicDeptPatient != null)
            if (obj != null && CurrentReqOutwardDrugClinicDeptPatient != null)
            {
                CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = obj;
            }
        }

        AutoCompleteBox acbAutoDrug_Text = null;
        public void AutoDrug_Text_Loaded(object sender, RoutedEventArgs e)
        {
            acbAutoDrug_Text = sender as AutoCompleteBox;
        }

        public void FilterStaff_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //FilterByStaff();
            //FillPagedCollectionAndGroup();
            if (ReqOutwardDrugClinicDeptPatientlst != null && ReqOutwardDrugClinicDeptPatientlst.CanFilter)
            {
                ReqOutwardDrugClinicDeptPatientlst.Filter = new Predicate<object>(ListRequestByStaffIDFilter);
                ReqOutwardDrugClinicDeptPatientlst.Refresh();
                CurrentlyViewReqByStaffIDFilter = true;
            }
        }

        //private void FilterByStaff()
        //{
        //    if (ReqOutwardDrugClinicDeptPatientlstCopy != null)
        //    {
        //        if (StaffDetailID > 0 && RequestDrug != null)
        //        {
        //            RequestDrug.ReqOutwardDetails = ReqOutwardDrugClinicDeptPatientlstCopy.Where(x => x.StaffID == StaffDetailID).ToObservableCollection();
        //        }
        //        else
        //        {
        //            RequestDrug.ReqOutwardDetails = ReqOutwardDrugClinicDeptPatientlstCopy.DeepCopy();
        //        }
        //    }
        //}
    }
}
