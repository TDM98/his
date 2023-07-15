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
using aEMR.Pharmacy.Views;
using aEMR.Common.Printing;
using aEMR.CommonTasks;
using System.Windows.Data;
using aEMR.Controls;
using System.Text;
using Castle.Windsor;
/*
 * 20190713 #001 TNHX: BM: Cho phép khoa kho BHYT tìm thuốc bằng tên hoạt chất.
 */
namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IPharmacyHIStoreReqForm)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PharmacyHIStoreReqFormViewModel : Conductor<object>, IPharmacyHIStoreReqForm
        , IHandle<DrugDeptCloseSearchRequestForHIStoreEvent>
        , IHandle<DrugResultEvent>
    {
        private bool _IsResultView = false;
        public bool IsResultView
        {
            get { return _IsResultView; }
            set
            {
                if (_IsResultView != value)
                {
                    _IsResultView = value;
                    NotifyOfPropertyChange(() => IsResultView);
                }
            }
        }

        private bool _DoseVisibility;
        public bool DoseVisibility
        {
            get { return _DoseVisibility; }
            set
            {
                if (_DoseVisibility != value)
                {
                    _DoseVisibility = value;
                    NotifyOfPropertyChange(() => DoseVisibility);
                }
            }
        }

        private bool _IsInputDosage;
        public bool IsInputDosage
        {
            get { return _IsInputDosage; }
            set
            {
                if (_IsInputDosage != value)
                {
                    _IsInputDosage = value;
                    NotifyOfPropertyChange(() => IsInputDosage);
                }
            }
        }

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

        //--▼-- 06/01/2021 DatTB
        private long _V_GroupTypes = (long)AllLookupValues.V_GroupTypes.TINH_GTGT;
        public long V_GroupTypes
        {
            get => _V_GroupTypes; set
            {
                _V_GroupTypes = value;
                NotifyOfPropertyChange(() => V_GroupTypes);
            }
        }
        //--▲-- 06/01/2021 DatTB

        [ImportingConstructor]
        public PharmacyHIStoreReqFormViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            authorization();
            var searchPatientAndRegVm = Globals.GetViewModel<ISearchPatientAndRegistration>();
            searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_PATIENT_BTN | SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
            searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);
            searchPatientAndRegVm.PatientFindByVisibility = false;
            searchPatientAndRegVm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;
            searchPatientAndRegVm.mTimBN = false;
            searchPatientAndRegVm.SearchAdmittedInPtRegOnly = true;

            SearchRegistrationContent = searchPatientAndRegVm;
            ActivateItem(searchPatientAndRegVm);

            Globals.EventAggregator.Subscribe(this);
            Coroutine.BeginExecute(DoGetStore_MedDept());
            Coroutine.BeginExecute(DoGetStore_ClinicDeptAll());
            Coroutine.BeginExecute(DoGetRefGenericDrugCategory_1List());

            SearchCriteria = new RequestSearchCriteria
            {
                FromDate = Globals.GetCurServerDateTime() - new TimeSpan(3, 0, 0, 0),
                ToDate = Globals.GetCurServerDateTime()
            };

            ListRequestDrugDelete = new ObservableCollection<RequestDrugInwardForHiStoreDetails>();
            RequestDrugHIStore = new RequestDrugInwardForHiStore();
            CurrentReqOutwardDrugHIStoreDetails = new RequestDrugInwardForHiStoreDetails();
            RefeshRequest();

            RefGenMedProductDetails = new PagedSortableCollectionView<RefGenMedProductDetails>();
            RefGenMedProductDetails.OnRefresh += RefGenMedProductDetails_OnRefresh;
            RefGenMedProductDetails.PageSize = Globals.PageSize;

            GetListStaffFilter();

            MedicalInstructionDate = Globals.GetCurServerDateTime();

            aucHoldConsultDoctor = Globals.GetViewModel<IAucHoldConsultDoctor>();
            aucHoldConsultDoctor.StaffCatType = (long)V_StaffCatType.BacSi;
        }

        private IEnumerator<IResult> DoGetStore_ClinicDeptAll()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_HIDRUGs, false, null, false, false);
            yield return paymentTypeTask;
            StoreCbxStaff = paymentTypeTask.LookupList;
            if (RequestDrugHIStore != null && StoreCbxStaff != null)
            {
                RequestDrugHIStore.InDeptStoreObject = StoreCbxStaff.FirstOrDefault();
                //--▼-- 06/01/2021 DatTB  DatTB Gán biến mặc định 
                var selectedStore = (RefStorageWarehouseLocation)StoreCbxStaff.FirstOrDefault();
                V_GroupTypes = selectedStore.V_GroupTypes;
                //--▲-- 06/01/2021 DatTB  
            }
            yield break;
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

        void RefGenMedProductDetails_OnRefresh(object sender, aEMR.Common.Collections.RefreshEventArgs e)
        {
            GetRefGenMedProductDetails_Auto(BrandName, false, RefGenMedProductDetails.PageIndex, RefGenMedProductDetails.PageSize);
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

        private IAucHoldConsultDoctor _aucHoldConsultDoctor;
        public IAucHoldConsultDoctor aucHoldConsultDoctor
        {
            get
            {
                return _aucHoldConsultDoctor;
            }
            set
            {
                if (_aucHoldConsultDoctor != value)
                {
                    _aucHoldConsultDoctor = value;
                    NotifyOfPropertyChange(() => aucHoldConsultDoctor);
                }
            }
        }

        private DateTime? _MedicalInstructionDate;
        public DateTime? MedicalInstructionDate
        {
            get
            {
                return _MedicalInstructionDate;
            }
            set
            {
                _MedicalInstructionDate = value;
                NotifyOfPropertyChange(() => MedicalInstructionDate);
            }
        }

        private string _strNote;
        public string strNote
        {
            get
            {
                return _strNote;
            }
            set
            {
                _strNote = value;
                NotifyOfPropertyChange(() => strNote);
            }
        }

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

        private ObservableCollection<RequestDrugInwardForHiStoreDetails> ListRequestDrugDelete;

        private RequestDrugInwardForHiStore _RequestDrugHIStore;
        public RequestDrugInwardForHiStore RequestDrugHIStore
        {
            get
            {
                return _RequestDrugHIStore;
            }
            set
            {
                if (_RequestDrugHIStore != value)
                {
                    _RequestDrugHIStore = value;
                    NotifyOfPropertyChange(() => RequestDrugHIStore);
                }
            }
        }

        private RequestDrugInwardForHiStoreDetails _CurrentReqOutwardDrugHIStoreDetails;
        public RequestDrugInwardForHiStoreDetails CurrentReqOutwardDrugHIStoreDetails
        {
            get { return _CurrentReqOutwardDrugHIStoreDetails; }
            set
            {
                if (_CurrentReqOutwardDrugHIStoreDetails != value)
                {
                    _CurrentReqOutwardDrugHIStoreDetails = value;
                    NotifyOfPropertyChange(() => CurrentReqOutwardDrugHIStoreDetails);
                }
            }
        }

        private RequestDrugInwardForHiStoreDetails _SelectedReqOutwardDrugClinicDeptPatient;
        public RequestDrugInwardForHiStoreDetails SelectedReqOutwardDrugClinicDeptPatient
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

        public CollectionView CollectionView_ReqDetails { get; set; }

        public CollectionViewSource CVS_ReqDetails { get; set; } = null;

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

        private long _V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
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
        #endregion

        #region 3. Function Member

        private void GetListStaffFilter()
        {
            ListStaff = new ObservableCollection<Staff>();
            if (ListStaff != null)
            {
                ListStaff.Clear();
                Staff item = new Staff
                {
                    StaffID = -1,
                    FullName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa)
                };
                ListStaff.Add(item);
                ListStaff.Add(GetStaffLogin());
            }

            if (RequestDrugHIStore != null && RequestDrugHIStore.ReqOutwardDetails != null && RequestDrugHIStore.ReqOutwardDetails.Count > 0)
            {
                var lst = (from e in RequestDrugHIStore.ReqOutwardDetails
                           select new { e.StaffID, e.StaffName }).Distinct();
                foreach (var staffItem in lst)
                {
                    if (staffItem.StaffID != GetStaffLogin().StaffID)
                    {
                        ListStaff.Add(new Staff { StaffID = staffItem.StaffID.GetValueOrDefault(0), FullName = staffItem.StaffName });
                    }
                }
            }
            StaffDetailID = ListStaff.FirstOrDefault().StaffID;
        }

        private IEnumerator<IResult> DoGetRefGenericDrugCategory_1List()
        {
            var paymentTypeTask = new LoadRefGenericDrugCategory_1ListTask(V_MedProductType, false, false);
            yield return paymentTypeTask;
            RefGenericDrugCategory_1s = paymentTypeTask.RefGenericDrugCategory_1List;
            SetDefultRefGenericDrugCategory();
            yield break;
        }

        private void SetDefultRefGenericDrugCategory()
        {
            if (RequestDrugHIStore != null && RefGenericDrugCategory_1s != null)
            {
                RequestDrugHIStore.RefGenDrugCatID_1 = RefGenericDrugCategory_1s.FirstOrDefault().RefGenDrugCatID_1;
            }
        }

        private Staff GetStaffLogin()
        {
            return Globals.LoggedUserAccount.Staff;
        }

        private void ischanged(object item)
        {
            RequestDrugInwardForHiStoreDetails p = item as RequestDrugInwardForHiStoreDetails;
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
            if (RequestDrugHIStore != null)
            {
                if (RequestDrugHIStore.ReqOutwardDetails != null)
                {
                    int nIdx = 0;
                    foreach (RequestDrugInwardForHiStoreDetails item in RequestDrugHIStore.ReqOutwardDetails)
                    {
                        nIdx++;
                        if (item.GenMedProductID != null && item.GenMedProductID != 0)
                        {
                            if (item.ReqQty < 0)
                            {
                                string strErr = string.Format("Dữ liệu dòng số ({0}): [{1}] Số lượng yêu cầu phải >= 0!", item.DisplayGridRowNumber.ToString(), item.RefGenericDrugDetail.BrandNameAndCode);
                                st.AppendLine(strErr);
                                result = false;
                            }
                            if (item.PrescribedQty <= 0)
                            {
                                string strErr = string.Format("Dữ liệu dòng số ({0}): [{1}] Số lượng Chỉ Định phải > 0!", item.DisplayGridRowNumber.ToString(), item.RefGenericDrugDetail.BrandNameAndCode);
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

        private void RequestDrugInwardHIStore_Save()
        {
            if (RequestDrugHIStore.ReqOutwardDetails != null && RequestDrugHIStore.ReqOutwardDetails.Count > 0)
            {
                foreach (RequestDrugInwardForHiStoreDetails item in RequestDrugHIStore.ReqOutwardDetails)
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

            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginRequestDrugInwardHIStore_Save(RequestDrugHIStore, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                        {

                            try
                            {
                                RequestDrugInwardForHiStore RequestOut;
                                contract.EndRequestDrugInwardHIStore_Save(out RequestOut, asyncResult);
                                if (RequestOut != null)
                                {
                                    RequestDrugHIStore = RequestOut;
                                    RequestDrugHIStore.ReqOutwardDetails = RequestOut.ReqOutwardDetails.DeepCopy();
                                    if (RequestDrugHIStore.ReqOutwardDetails != null && RequestDrugHIStore.ReqOutwardDetails.Count > 0)
                                    {
                                        foreach (RequestDrugInwardForHiStoreDetails item in RequestDrugHIStore.ReqOutwardDetails)
                                        {
                                            item.ItemVerified = false;
                                            if (item.ItemVerfStat == 1)
                                            {
                                                item.ItemVerified = true;
                                            }
                                        }
                                    }
                                }

                                ListRequestDrugDelete.Clear();
                                FillPagedCollectionAndGroup();

                                SetCheckAllReqOutwardDetail();

                                Globals.ShowMessage(eHCMSResources.Z1562_G1_DaLuu, eHCMSResources.G0442_G1_TBao);
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

        private void SaveRequest()
        {
            if (RequestDrugHIStore.RequestDrugInwardHiStoreID > 0)
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
            if (RequestDrugHIStore.OutFromStoreObject == null || RequestDrugHIStore.OutFromStoreObject.StoreID <= 0)
            {
                MessageBox.Show(eHCMSResources.K0318_G1_ChonKhoCC);
                return false;
            }
            if (RequestDrugHIStore.ReqOutwardDetails == null || RequestDrugHIStore.ReqOutwardDetails.Count == 0)
            {
                MessageBox.Show(eHCMSResources.K0422_G1_NhapCTietPhYC);
                return false;
            }
            else
            {
                if (RequestDrugHIStore.ReqOutwardDetails.Count == 1)
                {
                    if (RequestDrugHIStore.ReqOutwardDetails[0].RefGenericDrugDetail == null)
                    {
                        MessageBox.Show(eHCMSResources.K0303_G1_ChonHgYC);
                        return false;
                    }
                }

                for (int i = 0; i < RequestDrugHIStore.ReqOutwardDetails.Count; i++)
                {
                    if (RequestDrugHIStore.ReqOutwardDetails[i].RefGenericDrugDetail != null)
                    {
                        if (RequestDrugHIStore.ReqOutwardDetails[i].EntityState != EntityState.DETACHED && RequestDrugHIStore.ReqOutwardDetails[i].PrescribedQty <= 0)
                        {
                            string strErr = "Dữ liệu dòng số (" + RequestDrugHIStore.ReqOutwardDetails[i].DisplayGridRowNumber.ToString() + ") : [" + RequestDrugHIStore.ReqOutwardDetails[i].RefGenericDrugDetail.BrandNameAndCode + "] Số lượng Chỉ Định phải > 0";
                            MessageBox.Show(strErr);
                            return false;
                        }
                        if (RequestDrugHIStore.ReqOutwardDetails[i].EntityState != EntityState.DETACHED && RequestDrugHIStore.ReqOutwardDetails[i].ReqQty < 0)
                        {
                            string strErr = "Dữ liệu dòng số (" + RequestDrugHIStore.ReqOutwardDetails[i].DisplayGridRowNumber.ToString() + ") : [" + RequestDrugHIStore.ReqOutwardDetails[i].RefGenericDrugDetail.BrandNameAndCode + "] Số lượng yêu cầu phải >= 0";
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
                if (RequestDrugHIStore.ReqOutwardDetails == null)
                {
                    RequestDrugHIStore.ReqOutwardDetails = new ObservableCollection<RequestDrugInwardForHiStoreDetails>();
                }
                foreach (RequestDrugInwardForHiStoreDetails item in ListRequestDrugDelete)
                {
                    RequestDrugHIStore.ReqOutwardDetails.Add(item);
                }
            }

            if (ValidateDataOneLastTimeBeforeSaving())
            {
                RequestDrugInwardHIStore_Save();
            }
        }

        private void RemoveRowBlank()
        {
            try
            {
                int idx = RequestDrugHIStore.ReqOutwardDetails.Count;
                if (idx > 0)
                {
                    idx--;
                    RequestDrugInwardForHiStoreDetails obj = (RequestDrugInwardForHiStoreDetails)RequestDrugHIStore.ReqOutwardDetails[idx];
                    if (obj.GenMedProductID == null || obj.GenMedProductID == 0)
                    {
                        RequestDrugHIStore.ReqOutwardDetails.RemoveAt(idx);
                    }
                }
                NotifyOfPropertyChange(() => RequestDrugHIStore);
            }
            catch
            { }
        }

        private void RemoveMark(object item)
        {
            RequestDrugInwardForHiStoreDetails obj = item as RequestDrugInwardForHiStoreDetails;
            if (obj != null)
            {
                RequestDrugHIStore.ReqOutwardDetails.Remove(obj);
                if (obj.EntityState == EntityState.PERSITED || obj.EntityState == EntityState.MODIFIED)
                {
                    obj.EntityState = EntityState.DETACHED;
                    ListRequestDrugDelete.Add(obj);
                }
            }
        }

        public void OpenPopUpSearchRequestInvoice(IList<RequestDrugInwardForHiStore> results, int Totalcount, bool bCreateNewListFromOld)
        {
            void onInitDlg(IStoreDeptRequestSearch proAlloc)
            {
                proAlloc.IsRequestFromHIStore = true;
                proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                proAlloc.V_MedProductType = V_MedProductType;
                proAlloc.SetList();
                proAlloc.RequestDruglistHIStore.Clear();
                proAlloc.RequestDruglistHIStore.TotalItemCount = Totalcount;
                proAlloc.RequestDruglistHIStore.PageIndex = 0;
                proAlloc.IsCreateNewListFromSelectExisting = bCreateNewListFromOld;

                if (results != null && results.Count > 0)
                {
                    foreach (RequestDrugInwardForHiStore p in results)
                    {
                        proAlloc.RequestDruglistHIStore.Add(p);
                    }
                }
            }
            GlobalsNAV.ShowDialog<IStoreDeptRequestSearch>(onInitDlg);
        }

        private void SearchRequestDrugInwardHIStore(int PageIndex, int PageSize, bool bCreateNewListFromOld = false)
        {
            if (SearchCriteria == null)
            {
                return;
            }

            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);

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

            if (RequestDrugHIStore != null && RequestDrugHIStore.InDeptStoreID > 0)
            {
                SearchCriteria.RequestStoreID = RequestDrugHIStore.InDeptStoreID;
            }
            else
            {
                if (StoreCbxStaff.Count() > 0)
                {
                    SearchCriteria.RequestStoreID = StoreCbxStaff[0].StoreID;
                }
            }

            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSearchRequestDrugInwardHIStore(SearchCriteria, V_MedProductType, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int Total = 0;
                                var results = contract.EndSearchRequestDrugInwardHIStore(out Total, asyncResult);
                                if (results != null && results.Count > 0)
                                {
                                    if (results.Count > 1)
                                    {
                                        this.HideBusyIndicator();
                                        OpenPopUpSearchRequestInvoice(results.ToList(), Total, bCreateNewListFromOld);

                                    }
                                    else
                                    {
                                        this.HideBusyIndicator();
                                        if (RequestDrugHIStore != null)
                                        {
                                            ChangeValue(RequestDrugHIStore.RefGenDrugCatID_1, results.FirstOrDefault().RefGenDrugCatID_1);
                                        }
                                        RequestDrugHIStore = results.FirstOrDefault();
                                        ListRequestDrugDelete.Clear();
                                        GetRequestDrugListDetailsHIStoreByReqID(RequestDrugHIStore.RequestDrugInwardHiStoreID, bCreateNewListFromOld);
                                    }
                                }
                                else
                                {
                                    if (MessageBox.Show(eHCMSResources.Z0568_G1_KgTimThay, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                    {
                                        this.HideBusyIndicator();
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

        private void GetRequestDrugListDetailsHIStoreByReqID(long RequestDrugInwardHiStoreID, bool bCreateNewListFromOld = false)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRequestDrugInwardHIStoreDetailByID(RequestDrugInwardHiStoreID, bCreateNewListFromOld, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetRequestDrugInwardHIStoreDetailByID(asyncResult);
                                if (results != null)
                                {
                                    RequestDrugHIStore.ReqOutwardDetails = results.ToObservableCollection();
                                }
                                else
                                {
                                    if (RequestDrugHIStore.ReqOutwardDetails != null)
                                    {
                                        RequestDrugHIStore.ReqOutwardDetails.Clear();
                                    }
                                }
                                if (RequestDrugHIStore == null)
                                {
                                    RequestDrugHIStore = new RequestDrugInwardForHiStore();
                                }

                                GetListStaffFilter();
                                FillPagedCollectionAndGroup();
                                if (bCreateNewListFromOld)
                                {
                                    ResetingOldListToCreateNewList(bCreateNewListFromOld);
                                }
                                else
                                {
                                    if (RequestDrugHIStore.ReqOutwardDetails != null && RequestDrugHIStore.ReqOutwardDetails.Count > 0)
                                    {
                                        foreach (RequestDrugInwardForHiStoreDetails item in RequestDrugHIStore.ReqOutwardDetails)
                                        {
                                            item.ItemVerified = false;
                                            if (item.ItemVerfStat == 1)
                                            {
                                                item.ItemVerified = true;
                                            }
                                        }
                                    }
                                }

                                SetCheckAllReqOutwardDetail();
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
            RequestDrugHIStore.RequestDrugInwardHiStoreID = 0;
            RequestDrugHIStore.ReqNumCode = "";
            RequestDrugHIStore.Comment = "";
            RequestDrugHIStore.ReqDate = DateTime.Now;
            RequestDrugHIStore.FromDate = DateTime.Now;
            RequestDrugHIStore.ToDate = DateTime.Now;
            RequestDrugHIStore.SelectedStaff = GetStaffLogin();
            RequestDrugHIStore.DaNhanHang = false;
            RequestDrugHIStore.IsApproved = false;
            ListRequestDrugDelete.Clear();
            if (!bCreateNewListFromOld && StoreCbxStaff != null)
            {
                RequestDrugHIStore.InDeptStoreObject = StoreCbxStaff.FirstOrDefault();
            }
            if (RequestDrugHIStore.ReqOutwardDetails != null && RequestDrugHIStore.ReqOutwardDetails.Count > 0)
            {
                foreach (RequestDrugInwardForHiStoreDetails item in RequestDrugHIStore.ReqOutwardDetails)
                {
                    item.EntityState = EntityState.NEW;
                    item.ItemVerfStat = 0;
                }
            }
        }

        private void RefeshRequest()
        {
            CurrentPatient = null;
            RequestDrugHIStore.RequestDrugInwardHiStoreID = 0;
            RequestDrugHIStore.ReqNumCode = "";
            RequestDrugHIStore.Comment = "";
            RequestDrugHIStore.ReqDate = DateTime.Now;
            RequestDrugHIStore.FromDate = DateTime.Now;
            RequestDrugHIStore.ToDate = DateTime.Now;
            RequestDrugHIStore.SelectedStaff = GetStaffLogin();
            RequestDrugHIStore.ReqOutwardDetails = new ObservableCollection<RequestDrugInwardForHiStoreDetails>();
            RequestDrugHIStore.DaNhanHang = false;
            RequestDrugHIStore.IsApproved = false;

            ListRequestDrugDelete.Clear();

            if (StoreCbxStaff != null)
            {
                RequestDrugHIStore.InDeptStoreObject = StoreCbxStaff.FirstOrDefault();
            }
            if (StoreCbx != null)
            {
                RequestDrugHIStore.OutFromStoreObject = StoreCbx.FirstOrDefault();
            }
            SetDefultRefGenericDrugCategory();
            FillPagedCollectionAndGroup();
        }

        private void DeleteRequestDrugInwardClinicDept(long RequestDrugInwardHiStoreID)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeleteRequestDrugInwardHIStore(RequestDrugInwardHiStoreID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndDeleteRequestDrugInwardHIStore(asyncResult);
                                RefeshRequest();
                                SetCheckAllReqOutwardDetail();
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
            if (RequestDrugHIStore.RequestDrugInwardHiStoreID > 0)
            {
                DeleteRequestDrugInwardClinicDept(RequestDrugHIStore.RequestDrugInwardHiStoreID);
            }
        }

        private bool CheckDeleted(object item)
        {
            RequestDrugInwardForHiStoreDetails temp = item as RequestDrugInwardForHiStoreDetails;
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
            if (RequestDrugHIStore.InDeptStoreID == null || RequestDrugHIStore.InDeptStoreID == 0)
            {
                Globals.ShowMessage(eHCMSResources.Z1129_G1_ChonKhoYC, eHCMSResources.T0074_G1_I);
                return false;
            }
            if (RequestDrugHIStore.OutFromStoreID == null || RequestDrugHIStore.OutFromStoreID == 0)
            {
                Globals.ShowMessage(eHCMSResources.Z1130_G1_ChonKhoCCap, eHCMSResources.T0074_G1_I);
                return false;
            }

            if (RequestDrugHIStore.InDeptStoreID == RequestDrugHIStore.OutFromStoreID)
            {
                Globals.ShowMessage(eHCMSResources.Z1131_G1_KhoYCKhoCCKgDcTrung, eHCMSResources.T0074_G1_I);
                return false;
            }

            if (RequestDrugHIStore.ReqOutwardDetails != null)
            {
                if (RequestDrugHIStore.ReqOutwardDetails.Count == 0)
                {
                    if (RequestDrugHIStore.RequestDrugInwardHiStoreID > 0)
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
                string error = "";
                for (int i = 0; i < RequestDrugHIStore.ReqOutwardDetails.Count; i++)
                {
                    if (RequestDrugHIStore.ReqOutwardDetails[i].GenMedProductID > 0)
                    {
                        if (RequestDrugHIStore.ReqOutwardDetails[i].PrescribedQty <= 0)
                        {
                            results = false;
                            MessageBox.Show(string.Format(eHCMSResources.Z1775_G1_SLgChiDinhLonHon0, RequestDrugHIStore.ReqOutwardDetails[i].DisplayGridRowNumber.ToString(), RequestDrugHIStore.ReqOutwardDetails[i].RefGenericDrugDetail.BrandNameAndCode));
                            break;
                        }
                        if (RequestDrugHIStore.ReqOutwardDetails[i].ReqQty < 0)
                        {
                            results = false;
                            MessageBox.Show(string.Format(eHCMSResources.Z1124_G1_SLgYCLonHonBang0, RequestDrugHIStore.ReqOutwardDetails[i].DisplayGridRowNumber.ToString(), RequestDrugHIStore.ReqOutwardDetails[i].RefGenericDrugDetail.BrandNameAndCode));
                            break;
                        }
                        if (Globals.ServerConfigSection.InRegisElements.CheckMedicalInstructDate && Globals.ServerConfigSection.InRegisElements.NumOfOverDaysForMedicalInstructDate >= 0)
                        {
                            if ((RequestDrugHIStore.ReqOutwardDetails[i].MedicalInstructionDate.GetValueOrDefault() - Globals.GetCurServerDateTime().Date).Days > Globals.ServerConfigSection.InRegisElements.NumOfOverDaysForMedicalInstructDate)
                            {
                                error += "    " + RequestDrugHIStore.ReqOutwardDetails[i].RefGenericDrugDetail.BrandNameAndCode + Environment.NewLine;
                            }
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(error))
                {
                    string msg = Globals.ServerConfigSection.InRegisElements.NumOfOverDaysForMedicalInstructDate == 0
                        ? string.Format(eHCMSResources.Z1875_G1_NgYLenhKgLonHonNgHTai2, error)
                        : string.Format(eHCMSResources.Z1874_G1_NgYLenhKgLonHonNgHTai, Globals.ServerConfigSection.InRegisElements.NumOfOverDaysForMedicalInstructDate, error);
                    MessageBox.Show(msg, eHCMSResources.K1576_G1_CBao, MessageBoxButton.OK);
                    return false;
                }
            }
            return results;
        }

        private IEnumerator<IResult> DoGetStore_MedDept()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT, false, null, false, true);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList.Where(x => x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString()) && x.IsMain && (V_GroupTypes == 0 || x.V_GroupTypes == V_GroupTypes)).ToObservableCollection();
            if (RequestDrugHIStore != null && StoreCbx != null)
            {
                RequestDrugHIStore.OutFromStoreObject = StoreCbx.FirstOrDefault();
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
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
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
                                    if (RequestDrugHIStore != null)
                                    {
                                        RequestDrugHIStore.InDeptStoreObject = StoreCbxStaff.FirstOrDefault();
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

        TextBox tbxQty;
        public void tbxQty_Loaded(object sender, RoutedEventArgs e)
        {
            tbxQty = sender as TextBox;
        }

        TextBox tbxMDoseStr;
        public void tbxMDoseStr_Loaded(object sender, RoutedEventArgs e)
        {
            tbxMDoseStr = sender as TextBox;
        }

        private string BrandName;

        //▼====== #001
        private void GetRefGenMedProductDetails_Auto(string BrandName, bool IsCode, int PageIndex, int PageSize)
        {
            if (IsCode == false && BrandName.Length < 1)
            {
                return;
            }

            IsFocusTextCode = IsCode;

            long? RefGenDrugCatID_1 = null;
            if (RequestDrugHIStore != null)
            {
                RefGenDrugCatID_1 = RequestDrugHIStore.RefGenDrugCatID_1;
            }
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginRefGenMedProductDetails_SearchAutoPaging_ForHIStore(IsSearchByGenericName, IsCode, BrandName, RequestDrugHIStore.OutFromStoreObject.StoreID, V_MedProductType, RefGenDrugCatID_1, PageSize, PageIndex, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int Total;
                            var results = contract.EndRefGenMedProductDetails_SearchAutoPaging_ForHIStore(out Total, asyncResult);
                            // 20200713 TNHX: Chỉnh dựa trên Lĩnh của khoa phòng để khi tìm không thấy thì không nhảy qua SL-CD
                            if (results != null && results.Count > 0)
                            {
                                if (IsCode)
                                {
                                    if (results != null && results.Count > 0)
                                    {
                                        if (CurrentReqOutwardDrugHIStoreDetails == null)
                                        {
                                            CurrentReqOutwardDrugHIStoreDetails = new RequestDrugInwardForHiStoreDetails();
                                        }
                                        CurrentReqOutwardDrugHIStoreDetails.RefGenericDrugDetail = results.FirstOrDefault();
                                    }
                                    else
                                    {
                                        if (CurrentReqOutwardDrugHIStoreDetails != null)
                                        {
                                            CurrentReqOutwardDrugHIStoreDetails.RefGenericDrugDetail = null;
                                        }

                                        if (tbx != null)
                                        {
                                            txt = "";
                                            tbx.Text = "";
                                            tbx.Focus();
                                        }
                                        if (acbAutoDrug_Text != null)
                                        {
                                            acbAutoDrug_Text.Text = "";
                                        }

                                        MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    }
                                }
                                else
                                {
                                    RefGenMedProductDetails.Clear();
                                    RefGenMedProductDetails.TotalItemCount = Total;
                                    RefGenMedProductDetails.SourceCollection = results;
                                    acbAutoDrug_Text.PopulateComplete();
                                }
                            }
                            else
                            {
                                RefGenMedProductDetails = new PagedSortableCollectionView<RefGenMedProductDetails>();
                            }
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
        //▲======= #001

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

        DataGrid grdReqOutwardDetails = null;
        public void grdReqOutwardDetails_Loaded(object sender, RoutedEventArgs e)
        {
            grdReqOutwardDetails = sender as DataGrid;

            if (grdReqOutwardDetails == null)
            {
                return;
            }

            var colMDoseStr = grdReqOutwardDetails.GetColumnByName("colMDoseStr");
            var colADoseStr = grdReqOutwardDetails.GetColumnByName("colADoseStr");
            var colEDoseStr = grdReqOutwardDetails.GetColumnByName("colEDoseStr");
            var colNDoseStr = grdReqOutwardDetails.GetColumnByName("colNDoseStr");

            if (colMDoseStr == null || colADoseStr == null || colEDoseStr == null || colNDoseStr == null)
            {
                return;
            }
            colMDoseStr.Visibility = Visibility.Collapsed;
            colADoseStr.Visibility = Visibility.Collapsed;
            colEDoseStr.Visibility = Visibility.Collapsed;
            colNDoseStr.Visibility = Visibility.Collapsed;
        }

        public void grdReqOutwardDetails_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            RequestDrugInwardForHiStoreDetails rowItem = e.Row.DataContext as RequestDrugInwardForHiStoreDetails;
            if (rowItem == null)
            {
                return;
            }

            rowItem.DisplayGridRowNumber = e.Row.GetIndex() + 1;

            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
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

        private enum QtyColumnIndexes
        {
            M_Dose_Idx = 5,
            A_Dose_Idx = 6,
            E_Dose_Idx = 7,
            N_Dose_Idx = 8,
            Pres_Qty_Idx = 9,
            Req_Qty_Idx = 10
        };

        public void grdReqOutwardDetails_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (sender == null)
            {
                return;
            }
            if (e.Column.Equals(grdReqOutwardDetails.GetColumnByName("colPrescribedQty")))
            {
                if (RequestDrugHIStore != null && RequestDrugHIStore.ReqOutwardDetails != null && (sender as DataGrid).SelectedIndex != -1)
                {
                    RequestDrugInwardForHiStoreDetails selItem = RequestDrugHIStore.ReqOutwardDetails[(sender as DataGrid).SelectedIndex];
                    selItem.ReqQty = selItem.PrescribedQty;
                }
            }
            else if (e.Column.Equals(grdReqOutwardDetails.GetColumnByName("colReqQtyStr")))
            {
                ischanged((sender as DataGrid).SelectedItem);
                if (RequestDrugHIStore != null && RequestDrugHIStore.ReqOutwardDetails != null && (sender as DataGrid).SelectedIndex != -1)
                {
                    RequestDrugInwardForHiStoreDetails selItem = RequestDrugHIStore.ReqOutwardDetails[(sender as DataGrid).SelectedIndex];
                    selItem.ReqQty = Globals.ChangeDoseStringToDecimal(selItem.ReqQtyStr);
                }
            }
            ConvertCeiling(SelectedReqOutwardDrugClinicDeptPatient);
        }

        private void ConvertCeiling(RequestDrugInwardForHiStoreDetails req)
        {
            if (req == null)
            {
                return;
            }
            if (Globals.ServerConfigSection.ClinicDeptElements.LamTronSLXuatNoiTru && V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            {
                req.ReqQty = Math.Ceiling(req.ReqQty);
            }
            req.ReqQtyStr = req.ReqQty.ToString();
        }
        public void grdReqOutwardDetails_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (!CheckValidationEditor())
            {
            }
        }

        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (((PharmacyHIStoreReqFormView)this.GetView()).grdReqOutwardDetails != null)
            {
                if (MessageBox.Show(eHCMSResources.Z0554_G1_CoChacMuonXoaDongNayKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    RemoveMark(((PharmacyHIStoreReqFormView)this.GetView()).grdReqOutwardDetails.SelectedItem);

                    SetCheckAllReqOutwardDetail();
                }
            }
        }

        public DataGridRow GetDataGridRowByDataContext(DataGrid dataGrid, object dataContext)
        {
            return null;
        }

        public void btnSave(object sender, RoutedEventArgs e)
        {
            if (((PharmacyHIStoreReqFormView)this.GetView()).grdReqOutwardDetails != null)
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
            SearchRequestDrugInwardHIStore(0, Globals.PageSize);
        }

        public void display(List<RequestDrugInwardForHiStoreDetails> RemainingList)
        {
            foreach (var a in RequestDrugHIStore.ReqOutwardDetails)
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
                List<RequestDrugInwardForHiStoreDetails> RequestDetails = new List<RequestDrugInwardForHiStoreDetails>();
                RequestDetails = RequestDrugHIStore.ReqOutwardDetails.ToList<RequestDrugInwardForHiStoreDetails>();
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
                ObservableCollection<RequestDrugInwardForHiStoreDetails> RequestDrugDetails = RequestDetails.ToObservableCollection<RequestDrugInwardForHiStoreDetails>();
                temp.RequestDrugHIStore.ReqOutwardDetails = RequestDrugDetails;
            };
            GlobalsNAV.ShowDialog<IModify_ReqQty>(onInitDlg);
        }
        public void btnNew(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.A0146_G1_Msg_ConfTaoMoiPh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                RefeshRequest();
                hblPatientOther();
                if (CVS_ReqDetails != null)
                {
                    CVS_ReqDetails = new CollectionViewSource();
                    CollectionView_ReqDetails = (CollectionView)CVS_ReqDetails.View;
                    NotifyOfPropertyChange(() => CollectionView_ReqDetails);
                }
                SetCheckAllReqOutwardDetail();
                aucHoldConsultDoctor.setDefault();
                MedicalInstructionDate = Globals.GetCurServerDateTime();
            }
        }

        public void btnCreateNewByOld(object sender, RoutedEventArgs e)
        {
            if (RequestDrugHIStore == null)
            {
                return;
            }

            if (MessageBox.Show(eHCMSResources.A0145_G1_Msg_ConfTaoMoiTuPhCu, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                return;
            }
            if (RequestDrugHIStore.InDeptStoreID == null || RequestDrugHIStore.InDeptStoreID == 0)
            {
                Globals.ShowMessage(eHCMSResources.Z1129_G1_ChonKhoYC, eHCMSResources.T0074_G1_I);
                return;
            }
            SearchCriteria = new RequestSearchCriteria();

            hblPatientOther();

            SearchRequestDrugInwardHIStore(0, Globals.PageSize, true);

        }

        private bool CheckValidForLoadRequest()
        {
            if (RequestDrugHIStore == null)
            {
                return false;
            }
            if (RequestDrugHIStore.InDeptStoreID == null || RequestDrugHIStore.InDeptStoreID == 0)
            {
                Globals.ShowMessage(eHCMSResources.Z1129_G1_ChonKhoYC, eHCMSResources.T0074_G1_I);
                return false;
            }
            return true;
        }
        public void btnDelete(object sender, RoutedEventArgs e)
        {
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
                proAlloc.ID = RequestDrugHIStore.RequestDrugInwardHiStoreID;
                if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                {
                    if (RequestDrugHIStore.RefGenDrugCatID_1 == (long)AllLookupValues.RefGenDrugCatID_1.GAYNGHIEN)
                    {
                        proAlloc.LyDo = eHCMSResources.Z1139_G1_PhLinhThuocGayNghien;
                    }
                    else if (RequestDrugHIStore.RefGenDrugCatID_1 == (long)AllLookupValues.RefGenDrugCatID_1.HUONGTHAN)
                    {
                        proAlloc.LyDo = eHCMSResources.Z1141_G1_PhLinhThuocHuongTamThan;
                    }
                    else
                    {
                        proAlloc.LyDo = eHCMSResources.Z1143_G1_PhLinhThuoc;
                    }
                }
                proAlloc.eItem = ReportName.DrugDept_Request_HIStore;
            };
            GlobalsNAV.ShowDialog<IClinicDeptReportDocumentPreview>(onInitDlg);
        }

        public void btnPreviewCT()
        {
            Action<IClinicDeptReportDocumentPreview> onInitDlg = delegate (IClinicDeptReportDocumentPreview proAlloc)
            {
                proAlloc.ID = RequestDrugHIStore.RequestDrugInwardHiStoreID;
                if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                {
                    if (RequestDrugHIStore.RefGenDrugCatID_1 == (long)AllLookupValues.RefGenDrugCatID_1.GAYNGHIEN)
                    {
                        proAlloc.LyDo = eHCMSResources.Z1139_G1_PhLinhThuocGayNghien;
                    }
                    else if (RequestDrugHIStore.RefGenDrugCatID_1 == (long)AllLookupValues.RefGenDrugCatID_1.HUONGTHAN)
                    {
                        proAlloc.LyDo = eHCMSResources.Z1141_G1_PhLinhThuocHuongTamThan;
                    }
                    else
                    {
                        proAlloc.LyDo = eHCMSResources.Z1143_G1_PhLinhThuoc;
                    }
                }
                proAlloc.eItem = ReportName.DrugDept_Request_HIStore_Details;
            };
            GlobalsNAV.ShowDialog<IClinicDeptReportDocumentPreview>(onInitDlg);
        }

        public void btnPrint()
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new ReportServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetRequestPharmacyInPdfFormat(RequestDrugHIStore.RequestDrugInwardHiStoreID, Globals.DispatchCallback((asyncResult) =>
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

        #region IHandle<DrugDeptCloseSearchRequestForHIStoreEvent> Members

        public void Handle(DrugDeptCloseSearchRequestForHIStoreEvent message)
        {
            if (message != null && this.IsActive)
            {
                RequestDrugInwardForHiStore item = message.SelectedRequest as RequestDrugInwardForHiStore;
                if (RequestDrugHIStore != null && item != null)
                {
                    ChangeValue(RequestDrugHIStore.RefGenDrugCatID_1, item.RefGenDrugCatID_1);
                }
                RequestDrugHIStore = item;
                ListRequestDrugDelete.Clear();
                GetRequestDrugListDetailsHIStoreByReqID(RequestDrugHIStore.RequestDrugInwardHiStoreID, message.IsCreateNewFromExisting);
            }
        }

        #endregion

        private bool _IsFocusTextCode;
        public bool IsFocusTextCode
        {
            get { return _IsFocusTextCode; }
            set
            {
                if (_IsFocusTextCode != value)
                {
                    _IsFocusTextCode = value;
                }
                NotifyOfPropertyChange(() => IsFocusTextCode);
            }
        }

        public bool ListOutDrugReqFilter(object listObj)
        {
            RequestDrugInwardForHiStoreDetails outItem = listObj as RequestDrugInwardForHiStoreDetails;
            if (CurrentReqOutwardDrugHIStoreDetails.CurPatientRegistration != null && CurrentReqOutwardDrugHIStoreDetails.CurPatientRegistration.Patient != null && CurrentReqOutwardDrugHIStoreDetails.CurPatientRegistration.Patient.PatientID > 0)
            {
                if (outItem.CurPatientRegistration != null && outItem.CurPatientRegistration.Patient != null && outItem.CurPatientRegistration.Patient.PatientID > 0)
                {
                    return (outItem.CurPatientRegistration.Patient.PatientID == CurrentReqOutwardDrugHIStoreDetails.CurPatientRegistration.Patient.PatientID);
                }
            }
            else
            {
                if (CurrentReqOutwardDrugHIStoreDetails.CurPatientRegistration == null && outItem.CurPatientRegistration == null)
                {
                    return true;
                }
            }
            return false;
        }

        private bool CurrentlyViewReqByStaffIDFilter = false;
        public bool ListRequestByStaffIDFilter(object listObj)
        {
            RequestDrugInwardForHiStoreDetails outItem = listObj as RequestDrugInwardForHiStoreDetails;

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

            CurrentReqOutwardDrugHIStoreDetails.StaffID = GetStaffLogin().StaffID;
            //20190723 TBL: BM 0012971. Khi so luong cua kho cung cap = 0 thi khong cho them vao phieu 
            if (CurrentReqOutwardDrugHIStoreDetails.RefGenericDrugDetail.Remaining == 0)
            {
                MessageBox.Show(string.Format("{0} {1}", eHCMSResources.Z2779_G1_SoLuongKhoCCBang0, eHCMSResources.Z1418_G1_KgTheThemDuoc), eHCMSResources.G0442_G1_TBao);
                return;
            }
            //20190723
            if (UsedForRequestingDrug && RequestDrugHIStore != null && RequestDrugHIStore.RefGenDrugCatID_1 <= 0)
            {
                Globals.ShowMessage(eHCMSResources.Z1146_G1_ChonPhanNhomHg, eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (CurrentReqOutwardDrugHIStoreDetails.RefGenericDrugDetail == null || CurrentReqOutwardDrugHIStoreDetails.RefGenericDrugDetail.GenMedProductID == 0
                                    || string.IsNullOrEmpty(CurrentReqOutwardDrugHIStoreDetails.RefGenericDrugDetail.Code))
            {
                CurrentReqOutwardDrugHIStoreDetails.RefGenericDrugDetail = new RefGenMedProductDetails();

                Globals.ShowMessage(eHCMSResources.Z1147_G1_ChonHgCanYC, eHCMSResources.G0442_G1_TBao);
                return;
            }

            if (CurrentReqOutwardDrugHIStoreDetails.ReqQty <= 0)
            {
                Globals.ShowMessage(eHCMSResources.Z1161_G1_SLgCDinhKgHopLe, eHCMSResources.G0442_G1_TBao);
                return;
            }

            if (CurrentReqOutwardDrugHIStoreDetails.RefGenericDrugDetail.Remaining < CurrentReqOutwardDrugHIStoreDetails.ReqQty)
            {
                MessageBox.Show(eHCMSResources.Z2728_G1_SLTrongKhoKhongDu, eHCMSResources.G0442_G1_TBao);
                return;
            }

            if (CalByUnitUse)
            {
                CurrentReqOutwardDrugHIStoreDetails.ReqQty = CurrentReqOutwardDrugHIStoreDetails.ReqQty / (CurrentReqOutwardDrugHIStoreDetails.RefGenericDrugDetail.DispenseVolume == 0 ? 1 : (decimal)CurrentReqOutwardDrugHIStoreDetails.RefGenericDrugDetail.DispenseVolume);
                CurrentReqOutwardDrugHIStoreDetails.ReqQty = Math.Round(CurrentReqOutwardDrugHIStoreDetails.ReqQty, 2);
            }

            CurrentReqOutwardDrugHIStoreDetails.DateTimeSelection = Globals.GetCurServerDateTime();

            if (RequestDrugHIStore == null)
            {
                RequestDrugHIStore = new RequestDrugInwardForHiStore();
            }
            if (RequestDrugHIStore.ReqOutwardDetails == null)
            {
                RequestDrugHIStore.ReqOutwardDetails = new ObservableCollection<RequestDrugInwardForHiStoreDetails>();
            }
            var temp = RequestDrugHIStore.ReqOutwardDetails.Where(x => x.RefGenericDrugDetail != null && CurrentReqOutwardDrugHIStoreDetails.RefGenericDrugDetail != null && x.RefGenericDrugDetail.GenMedProductID == CurrentReqOutwardDrugHIStoreDetails.RefGenericDrugDetail.GenMedProductID && x.PtRegistrationID == CurrentReqOutwardDrugHIStoreDetails.PtRegistrationID);
            {
                if (temp != null && temp.Count() > 0 && MessageBox.Show(eHCMSResources.A0774_G1_Msg_ConfThemHgDaCo, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return;
                }
                CurrentReqOutwardDrugHIStoreDetails.EntityState = EntityState.NEW;
                CurrentReqOutwardDrugHIStoreDetails.PrescribedQty = CurrentReqOutwardDrugHIStoreDetails.ReqQty;
                ConvertCeiling(CurrentReqOutwardDrugHIStoreDetails);
                CurrentReqOutwardDrugHIStoreDetails.DoctorStaff = new Staff
                {
                    StaffID = aucHoldConsultDoctor.StaffID,
                    FullName = aucHoldConsultDoctor.StaffName
                };
                CurrentReqOutwardDrugHIStoreDetails.MedicalInstructionDate = MedicalInstructionDate;
                CurrentReqOutwardDrugHIStoreDetails.Notes = strNote;

                var item = CurrentReqOutwardDrugHIStoreDetails.DeepCopy();
                RequestDrugHIStore.ReqOutwardDetails.Add(item);
                if (RequestDrugHIStore.ReqOutwardDetails.Count == 1)
                {
                    FillPagedCollectionAndGroup();
                }
            }

            CurrentReqOutwardDrugHIStoreDetails.RefGenericDrugDetail = new RefGenMedProductDetails();
            CurrentReqOutwardDrugHIStoreDetails.MDoseStr = "0";
            CurrentReqOutwardDrugHIStoreDetails.ADoseStr = "0";
            CurrentReqOutwardDrugHIStoreDetails.EDoseStr = "0";
            CurrentReqOutwardDrugHIStoreDetails.NDoseStr = "0";
            CurrentReqOutwardDrugHIStoreDetails.MDose = 0;
            CurrentReqOutwardDrugHIStoreDetails.ADose = 0;
            CurrentReqOutwardDrugHIStoreDetails.EDose = 0;
            CurrentReqOutwardDrugHIStoreDetails.NDose = 0;
            CurrentReqOutwardDrugHIStoreDetails.ReqQty = 0;
            if (IsFocusTextCode)
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
                if (acbAutoDrug_Text != null)
                {
                    acbAutoDrug_Text.Text = "";
                    acbAutoDrug_Text.Focus();
                }
            }

            if (CollectionView_ReqDetails != null)
            {
                CollectionView_ReqDetails.Refresh();

                if (CollectionView_ReqDetails.CanFilter && (CollectionView_ReqDetails.Filter == null || CurrentlyViewReqByStaffIDFilter))
                {
                    CollectionView_ReqDetails.Filter = new Predicate<object>(ListOutDrugReqFilter);
                    CurrentlyViewReqByStaffIDFilter = false;
                }
            }

            SetCheckAllReqOutwardDetail();
        }

        private void FillPagedCollectionAndGroup()
        {
            if (RequestDrugHIStore != null && RequestDrugHIStore.ReqOutwardDetails != null && RequestDrugHIStore.ReqOutwardDetails.Count > 0)
            {
                CVS_ReqDetails = new CollectionViewSource { Source = RequestDrugHIStore.ReqOutwardDetails };
                CollectionView_ReqDetails = (CollectionView)CVS_ReqDetails.View;
                FillGroupName();
                NotifyOfPropertyChange(() => CollectionView_ReqDetails);
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
                if (RequestDrugHIStore != null && RequestDrugHIStore.ReqOutwardDetails.Count > 0)
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
                if (RequestDrugHIStore != null && RequestDrugHIStore.ReqOutwardDetails != null)
                {
                    for (int i = 0; i < RequestDrugHIStore.ReqOutwardDetails.Count; i++)
                    {
                        if (RequestDrugHIStore.ReqOutwardDetails[i].EntityState == EntityState.PERSITED || RequestDrugHIStore.ReqOutwardDetails[i].EntityState == EntityState.MODIFIED)
                        {
                            RequestDrugHIStore.ReqOutwardDetails[i].EntityState = EntityState.DETACHED;
                            ListRequestDrugDelete.Add(RequestDrugHIStore.ReqOutwardDetails[i]);
                        }
                    }
                    RequestDrugHIStore.ReqOutwardDetails.Clear();
                }
                //20190722 TBL: BM 0012966. Khi đổi phân nhóm không xóa autocomplete
                BrandName = "";
                txt = "";
                CurrentReqOutwardDrugHIStoreDetails.RefGenericDrugDetail = new RefGenMedProductDetails();
                CurrentReqOutwardDrugHIStoreDetails.ReqQty = 0;
                //20190722
            }

            flag = true;
            if (bSel.IsDropDownOpen)
            {
                bSel.IsDropDownOpen = false;
            }
        }

        private bool flagStoreSupplier = true;
        public void cbxStoreSupplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbxStoreSupplier = (ComboBox)sender;
            if (cbxStoreSupplier == null)
            {
                return;
            }
            if (flagStoreSupplier && RequestDrugHIStore != null && RequestDrugHIStore.ReqOutwardDetails != null && RequestDrugHIStore.ReqOutwardDetails.Count > 0)
            {
                if (MessageBox.Show(eHCMSResources.Z2756_G1_Msg_ConfDoiKhoCC, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    RequestDrugHIStore.ReqOutwardDetails.Clear();
                    ClearAutoComplete();
                }
                else
                {
                    flagStoreSupplier = false;
                    cbxStoreSupplier.SelectedItem = e.RemovedItems[0];
                    return;
                }
            }
            else if (flagStoreSupplier)
            {
                ClearAutoComplete();
            }
            flagStoreSupplier = true;
        }

        private void ClearAutoComplete()
        {
            CurrentReqOutwardDrugHIStoreDetails.RefGenericDrugDetail = new RefGenMedProductDetails();
            CurrentReqOutwardDrugHIStoreDetails.ReqQty = 0;
        }

        #region Checked All Member

        private bool _CheckAllOutwardDetail;
        public bool CheckAllOutwardDetail
        {
            get
            {
                return _CheckAllOutwardDetail;
            }
            set
            {
                if (_CheckAllOutwardDetail != value)
                {
                    _CheckAllOutwardDetail = value;
                    NotifyOfPropertyChange(() => CheckAllOutwardDetail);
                }
            }
        }

        public void chkAllReqOutwardDetail_Click(object sender, RoutedEventArgs e)
        {
            if (RequestDrugHIStore == null || RequestDrugHIStore.ReqOutwardDetails == null || RequestDrugHIStore.ReqOutwardDetails.Count <= 0)
            {
                return;
            }

            if (CheckAllOutwardDetail)
            {
                foreach (RequestDrugInwardForHiStoreDetails item in RequestDrugHIStore.ReqOutwardDetails)
                {
                    item.Checked = true;
                }
            }
            else
            {
                foreach (RequestDrugInwardForHiStoreDetails item in RequestDrugHIStore.ReqOutwardDetails)
                {
                    item.Checked = false;
                }
            }
        }

        private void SetCheckAllReqOutwardDetail()
        {
            if (RequestDrugHIStore == null || RequestDrugHIStore.ReqOutwardDetails == null || RequestDrugHIStore.ReqOutwardDetails.Count <= 0)
            {
                CheckAllOutwardDetail = false;
                return;
            }

            CheckAllOutwardDetail = RequestDrugHIStore.ReqOutwardDetails.All(x => x.Checked);

        }

        public void chkReqOutwardDetail_Click(object sender, RoutedEventArgs e)
        {
            SetCheckAllReqOutwardDetail();
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
            if (((PharmacyHIStoreReqFormView)this.GetView()).grdReqOutwardDetails != null)
            {
                if (RequestDrugHIStore.CanSave)
                {
                    ((PharmacyHIStoreReqFormView)this.GetView()).grdReqOutwardDetails.Columns[(int)DataGridCol.ColDelete].Visibility = Visibility.Visible;
                    ((PharmacyHIStoreReqFormView)this.GetView()).grdReqOutwardDetails.Columns[(int)DataGridCol.ColMultiDelete].Visibility = Visibility.Visible;
                }
                else
                {
                    ((PharmacyHIStoreReqFormView)this.GetView()).grdReqOutwardDetails.Columns[(int)DataGridCol.ColDelete].Visibility = Visibility.Collapsed;
                    ((PharmacyHIStoreReqFormView)this.GetView()).grdReqOutwardDetails.Columns[(int)DataGridCol.ColMultiDelete].Visibility = Visibility.Collapsed;
                }
            }
        }

        public void btnDeleteHang()
        {
            if (RequestDrugHIStore != null && RequestDrugHIStore.ReqOutwardDetails != null && RequestDrugHIStore.ReqOutwardDetails.Count > 0)
            {
                var items = RequestDrugHIStore.ReqOutwardDetails.Where(x => x.Checked == true);
                if (items != null && items.Count() > 0)
                {
                    if (MessageBox.Show("Bạn có chắc muốn xóa những hàng đã chọn không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        foreach (RequestDrugInwardForHiStoreDetails obj in items)
                        {
                            if (obj.EntityState == EntityState.PERSITED || obj.EntityState == EntityState.MODIFIED)
                            {
                                obj.EntityState = EntityState.DETACHED;
                                ListRequestDrugDelete.Add(obj);
                            }
                        }
                        RequestDrugHIStore.ReqOutwardDetails = RequestDrugHIStore.ReqOutwardDetails.Where(x => x.Checked == false).ToObservableCollection();
                        FillPagedCollectionAndGroup();

                        SetCheckAllReqOutwardDetail();
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
            if (CollectionView_ReqDetails != null)
            {
                CollectionView_ReqDetails.GroupDescriptions.Clear();
                CollectionView_ReqDetails.SortDescriptions.Clear();
                CollectionView_ReqDetails.GroupDescriptions.Add(new System.Windows.Data.PropertyGroupDescription("CurPatientRegistration.Patient.PatientCodeAndName"));
                CollectionView_ReqDetails.Filter = null;
            }
        }

        public void hblPatientOther()
        {
            CurrentReqOutwardDrugHIStoreDetails.CurPatientRegistration = null;
            CurrentReqOutwardDrugHIStoreDetails.PtRegistrationID = 0;
            CurrentPatient = null;
            if (CollectionView_ReqDetails != null)
            {
                CollectionView_ReqDetails.Filter = new Predicate<object>(ListOutDrugReqFilter);
                CurrentlyViewReqByStaffIDFilter = false;
                CollectionView_ReqDetails.Refresh();
            }
        }

        public void btnPatientOther(object sender, RoutedEventArgs e)
        {
            hblPatientOther();
        }

        public void btnViewAll()
        {
            if (CollectionView_ReqDetails != null)
            {
                CollectionView_ReqDetails.Filter = null;
                CollectionView_ReqDetails.Refresh();
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
            if (CollectionView_ReqDetails == null || CollectionView_ReqDetails.Groups == null || CollectionView_ReqDetails.Groups.Count <= 0)
            {
                return;
            }
            bAllGroupsAlreadyClosed = bClosed;
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

                    GetRefGenMedProductDetails_Auto(Code, true, 0, RefGenMedProductDetails.PageSize);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(" ====> AxTextBox_LostFocus 2 .....");
                    CurrentReqOutwardDrugHIStoreDetails.RefGenericDrugDetail = new RefGenMedProductDetails();
                }
            }
        }

        public void AutoCompleteBox_Populating(object sender, PopulatingEventArgs e)
        {
            //20190722 TBL: BM 0012967. Chưa chọn Phân nhóm mà đã nhập vào autocomplete thì hiện thông báo
            if (RequestDrugHIStore.RefGenDrugCatID_1 < 0 && V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            {
                MessageBox.Show(eHCMSResources.Z1146_G1_ChonPhanNhomHg, eHCMSResources.G0442_G1_TBao);
                CurrentReqOutwardDrugHIStoreDetails.RefGenericDrugDetail = new RefGenMedProductDetails();
                return;
            }
            e.Cancel = true;
            BrandName = e.Parameter;
            RefGenMedProductDetails.PageIndex = 0;
            GetRefGenMedProductDetails_Auto(e.Parameter, false, 0, RefGenMedProductDetails.PageSize);
        }

        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            RefGenMedProductDetails obj = acbAutoDrug_Text.SelectedItem as RefGenMedProductDetails;
            if (obj != null && CurrentReqOutwardDrugHIStoreDetails != null)
            {
                CurrentReqOutwardDrugHIStoreDetails.RefGenericDrugDetail = obj;
            }
            AutoDrug_LostFocus(null, null);
        }

        private AutoCompleteBox acbAutoDrug_Text = null;
        public void AutoDrug_Text_Loaded(object sender, RoutedEventArgs e)
        {
            acbAutoDrug_Text = sender as AutoCompleteBox;
        }

        public void AutoDrug_LostFocus(object sender, RoutedEventArgs e)
        {
            if (acbAutoDrug_Text == null || string.IsNullOrEmpty(acbAutoDrug_Text.Text))
            {
                return;
            }
            if (IsInputDosage)
            {
                if (tbxMDoseStr == null)
                {
                    return;
                }
                tbxMDoseStr.Focus();
            }
            else
            {
                if (tbxQty == null)
                {
                    return;
                }
                tbxQty.Focus();
            }
        }

        public void FilterStaff_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CollectionView_ReqDetails != null && CollectionView_ReqDetails.CanFilter)
            {
                CollectionView_ReqDetails.Filter = new Predicate<object>(ListRequestByStaffIDFilter);
                CollectionView_ReqDetails.Refresh();
                CurrentlyViewReqByStaffIDFilter = true;
            }
        }

        public void btnChangeDoctor(object sender, RoutedEventArgs e)
        {
            if (RequestDrugHIStore == null || RequestDrugHIStore.ReqOutwardDetails == null || RequestDrugHIStore.ReqOutwardDetails.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0910_G1_Msg_InfoKhTheDoiBSCDinh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            ObservableCollection<RequestDrugInwardForHiStoreDetails> SelectReqOutwardDetails = RequestDrugHIStore.ReqOutwardDetails.Where(x => x.Checked).ToObservableCollection();

            if (SelectReqOutwardDetails == null || SelectReqOutwardDetails.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0595_G1_Msg_InfoChonDongCanDoiBS, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            foreach (RequestDrugInwardForHiStoreDetails item in SelectReqOutwardDetails)
            {
                if (item.DoctorStaff == null)
                {
                    item.DoctorStaff = new Staff();
                }
                item.DoctorStaff.StaffID = aucHoldConsultDoctor.StaffID;
                item.DoctorStaff.FullName = aucHoldConsultDoctor.StaffName;

                ischanged(item);
            }

            MessageBox.Show(eHCMSResources.Z1149_G1_DoiBSiChiDinhThCong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        }

        public void btnChangeMedicalInstructionDate(object sender, RoutedEventArgs e)
        {
            if (RequestDrugHIStore == null || RequestDrugHIStore.ReqOutwardDetails == null || RequestDrugHIStore.ReqOutwardDetails.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0912_G1_Msg_InfoDoiNgYLenhPhLanh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            ObservableCollection<RequestDrugInwardForHiStoreDetails> SelectReqOutwardDetails = RequestDrugHIStore.ReqOutwardDetails.Where(x => x.Checked).ToObservableCollection();

            if (SelectReqOutwardDetails == null || SelectReqOutwardDetails.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0597_G1_Msg_InfoChonDongCanDoiNgYL, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            foreach (RequestDrugInwardForHiStoreDetails item in SelectReqOutwardDetails)
            {
                item.MedicalInstructionDate = MedicalInstructionDate;

                ischanged(item);
            }

            MessageBox.Show(eHCMSResources.Z1150_G1_DoiNgYLenhThCong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        }

        public void btnChangeNote(object sender, RoutedEventArgs e)
        {
            if (RequestDrugHIStore == null || RequestDrugHIStore.ReqOutwardDetails == null || RequestDrugHIStore.ReqOutwardDetails.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0911_G1_Msg_InfoKhTheDoiGhiChu, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            ObservableCollection<RequestDrugInwardForHiStoreDetails> SelectReqOutwardDetails = RequestDrugHIStore.ReqOutwardDetails.Where(x => x.Checked).ToObservableCollection();

            if (SelectReqOutwardDetails == null || SelectReqOutwardDetails.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0596_G1_Msg_InfoChonDongCanDoiGhiChu, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            foreach (RequestDrugInwardForHiStoreDetails item in SelectReqOutwardDetails)
            {
                item.Notes = strNote;

                ischanged(item);
            }

            MessageBox.Show(eHCMSResources.Z1151_G1_DoiGChuThCong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        }

        public void tbxMDoseStr_LostFocus(object sender, RoutedEventArgs e)
        {
            Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.MDose, CurrentReqOutwardDrugHIStoreDetails);
        }

        public void tbxADoseStr_LostFocus(object sender, RoutedEventArgs e)
        {
            Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.ADose, CurrentReqOutwardDrugHIStoreDetails);
        }

        public void tbxEDoseStr_LostFocus(object sender, RoutedEventArgs e)
        {
            Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.EDose, CurrentReqOutwardDrugHIStoreDetails);
        }

        public void tbxNDoseStr_LostFocus(object sender, RoutedEventArgs e)
        {
            Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.NDose, CurrentReqOutwardDrugHIStoreDetails);
        }

        DataGridCell mCurrentCell;
        public void grdReqOutwardDetails_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (((e.Column.Equals(grdReqOutwardDetails.GetColumnByName("colMDoseStr")))
                || (e.Column.Equals(grdReqOutwardDetails.GetColumnByName("colADoseStr")))
                || (e.Column.Equals(grdReqOutwardDetails.GetColumnByName("colEDoseStr")))
                || (e.Column.Equals(grdReqOutwardDetails.GetColumnByName("colNDoseStr"))))
                 && IsResultView)
            {
                mCurrentCell = e.Column.GetCellContent(e.Row.DataContext).Parent as DataGridCell;
                e.Cancel = true;
                GlobalsNAV.ShowDialog<IDrugResult>();
            }
        }

        public void Handle(DrugResultEvent message)
        {
            if (message != null)
            {
                switch (message.Result)
                {
                    case 1:
                        mCurrentCell.Background = new SolidColorBrush(Colors.Blue);
                        break;
                    case 2:
                        mCurrentCell.Background = new SolidColorBrush(Colors.Green);
                        break;
                    default:
                        mCurrentCell.Background = new SolidColorBrush(Colors.White);
                        break;
                }
            }
        }

        private bool _EnableTestFunction = Globals.ServerConfigSection.CommonItems.EnableTestFunction;
        public bool EnableTestFunction
        {
            get
            {
                return _EnableTestFunction;
            }
            set
            {
                _EnableTestFunction = value;
                NotifyOfPropertyChange(() => EnableTestFunction);
            }
        }

        //▼====== #001
        private bool _isSearchByGenericName = false;
        public bool IsSearchByGenericName
        {
            get { return _isSearchByGenericName; }
            set
            {
                if (_isSearchByGenericName != value)
                {
                    _isSearchByGenericName = value;
                    NotifyOfPropertyChange(() => IsSearchByGenericName);
                }
            }
        }

        private bool _visSearchByGenericName = false;
        public bool vIsSearchByGenericName
        {
            get { return _visSearchByGenericName; }
            set
            {
                if (_visSearchByGenericName != value)
                {
                    _visSearchByGenericName = value;
                    NotifyOfPropertyChange(() => vIsSearchByGenericName);
                }
            }
        }

        public void chkSearchByGenericName_Loaded(object sender, RoutedEventArgs e)
        {
            var chkSearchByGenericName = sender as CheckBox;

            if (Globals.ServerConfigSection.ConsultationElements.DefSearchByGenericName)
            {
                chkSearchByGenericName.IsChecked = true;
            }
            else
            {
                chkSearchByGenericName.IsChecked = false;
            }
        }
        //▲======= #001
    }
}
