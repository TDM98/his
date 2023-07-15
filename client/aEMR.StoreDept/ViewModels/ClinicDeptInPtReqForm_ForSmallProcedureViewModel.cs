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
using Castle.Windsor;
/*
 * 20230510 #001 QTD:  Clone từ View ClinicDeptInPtReqForm_ForSmallProcedureViewModelModel
 */
namespace aEMR.StoreDept.ViewModels
{
    [Export(typeof(IClinicDeptInPtReqForm_ForSmallProcedure)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ClinicDeptInPtReqForm_ForSmallProcedureViewModel : Conductor<object>, IClinicDeptInPtReqForm_ForSmallProcedure
        , IHandle<PatientSelectedGoToKhamBenh_InPt<PatientRegistration>>
        , IHandle<DrugResultEvent>
        , IHandle<ReloadInPatientRequestingDrugListByReqID>
    {
        //▼====: #017
        private bool _IsCOVID = false;
        public bool IsCOVID
        {
            get
            {
                return _IsCOVID;
            }
            set
            {
                if (_IsCOVID != value)
                {
                    _IsCOVID = value;
                    NotifyOfPropertyChange(() => IsCOVID);
                }
            }
        }
        //▲====: #017
        //==== #001 ====
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
        //==== #001 ====

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
        private DateTime? _MaxDay;
        public DateTime? MaxDay
        {
            get
            {
                return _MaxDay;
            }
            set
            {
                if (_MaxDay != value)
                {
                    _MaxDay = value;
                    NotifyOfPropertyChange(() => MaxDay);
                }
            }
        }
        [ImportingConstructor]
        public ClinicDeptInPtReqForm_ForSmallProcedureViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Authorization();

            //Coroutine.BeginExecute(DoGetRefGenericDrugCategory_1List());

            SearchCriteria = new RequestSearchCriteria();

            SearchCriteria.FromDate = DateTime.Now.Date - new TimeSpan(3, 0, 0, 0);
            SearchCriteria.ToDate = DateTime.Now;

            ListRequestDrugDelete = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
            RequestDrug = new RequestDrugForTechnicalService();
            CurrentReqOutwardDrugClinicDeptPatient = new ReqOutwardDrugClinicDeptPatient();

            RefGenMedProductDetails = new ObservableCollection<RefGenMedProductDetails>();
            //RefGenMedProductDetails.OnRefresh += RefGenMedProductDetails_OnRefresh;
            //RefGenMedProductDetails.PageSize = Globals.PageSize;

            ListStaff = new ObservableCollection<Staff>();
            GetListStaffFilter();

            MedicalInstructionDateContent = Globals.GetViewModel<IMinHourDateControl>();
            MedicalInstructionDateContent.DateTime = new DateTime(Globals.GetCurServerDateTime().Year, Globals.GetCurServerDateTime().Month, Globals.GetCurServerDateTime().Day, Globals.GetCurServerDateTime().Hour, Globals.GetCurServerDateTime().Minute, 0);

            aucHoldConsultDoctor = Globals.GetViewModel<IAucHoldConsultDoctor>();
            aucHoldConsultDoctor.StaffCatType = (long)V_StaffCatType.BacSi;
            if (Globals.ServerConfigSection.ConsultationElements.FilterDoctorByDeptResponsibilitiesInPt && !Globals.IsUserAdmin)
            {
                aucHoldConsultDoctor.CurrentDeptID = Globals.DeptLocation.DeptID;
            }
            iFlag = true;
            if (Globals.ServerConfigSection.CommonItems.IsApplyTimeForAllowUpdateMedicalInstruction)
            {
                MaxDay = Globals.GetCurServerDateTime();
            }
            else
            {
                MaxDay = DateTime.MaxValue;
            }
        }

        private bool iFlag = false;
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            //if (iFlag)
            //{
            //    var homevm = Globals.GetViewModel<IHome>();
            //    ICheckStatusRequestInvoiceOutStandingTask ostvm = Globals.GetViewModel<ICheckStatusRequestInvoiceOutStandingTask>();
            //    ostvm.V_MedProductType = V_MedProductType;
            //    ostvm.LoadStore();
            //    homevm.OutstandingTaskContent = ostvm;
            //    homevm.IsExpandOST = true;
            //}
            Coroutine.BeginExecute(DoGetStore_ClinicDeptAll());
            Coroutine.BeginExecute(DoGetStore_MedDept());
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            //if (iFlag)
            //{
            //    Globals.EventAggregator.Unsubscribe(this);
            //    var homevm = Globals.GetViewModel<IHome>();
            //    homevm.OutstandingTaskContent = null;
            //    homevm.IsExpandOST = false;
            //    iFlag = false;
            //}
        }

        private long _StaffDetailID = 0;
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

        void RefGenMedProductDetails_OnRefresh(object sender, Common.Collections.RefreshEventArgs e)
        {
            //GetRefGenMedProductDetails_Auto(BrandName, false, RefGenMedProductDetails.PageIndex, RefGenMedProductDetails.PageSize);
            SearchRefGenMedProductDetails(BrandName, false);
        }

        public void Authorization()
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

        private IMinHourDateControl _MedicalInstructionDateContent;
        public IMinHourDateControl MedicalInstructionDateContent
        {
            get { return _MedicalInstructionDateContent; }
            set
            {
                _MedicalInstructionDateContent = value;
                NotifyOfPropertyChange(() => MedicalInstructionDateContent);
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

        private ObservableCollection<RefGenMedProductDetails> _RefGenMedProductDetails;
        public ObservableCollection<RefGenMedProductDetails> RefGenMedProductDetails
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

        private RequestDrugForTechnicalService _RequestDrug;
        public RequestDrugForTechnicalService RequestDrug
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

        public CollectionView CollectionView_ReqDetails { get; set; }

        private CollectionViewSource _cvs_ReqDetails = null;
        public CollectionViewSource CVS_ReqDetails
        {
            get { return _cvs_ReqDetails; }
            set
            {
                _cvs_ReqDetails = value;
                ControlStatusCoupon();
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

        private long _V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU; //11001 : thuoc, 11002 : y cu , 11003 :hoa chat
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
                    NotifyOfPropertyChange(() => IsSearchWithoutRemainingForVPPAndVTTH);
                }
            }
        }

        public bool CanSelectedRefGenDrugCatID_1
        {
            get { return V_MedProductType == (long)AllLookupValues.MedProductType.THUOC; }
        }

        private bool _IsNotCheckRemain = false;
        public bool IsNotCheckRemain
        {
            get
            {
                return _IsNotCheckRemain;
            }
            set
            {
                if (_IsNotCheckRemain == value)
                {
                    return;
                }
                _IsNotCheckRemain = value;
                NotifyOfPropertyChange(() => IsNotCheckRemain);
            }
        }
        #endregion

        #region 3. Function Member

        private void GetListStaffFilter()
        {
            if (ListStaff != null)
            {
                ListStaff.Clear();
                Staff item = new Staff
                {
                    StaffID = 0,
                    FullName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa)
                };
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

        //private IEnumerator<IResult> DoGetRefGenericDrugCategory_1List()
        //{
        //    var paymentTypeTask = new LoadRefGenericDrugCategory_1ListTask(V_MedProductType, false, false);
        //    yield return paymentTypeTask;
        //    RefGenericDrugCategory_1s = paymentTypeTask.RefGenericDrugCategory_1List;
        //    SetDefultRefGenericDrugCategory();
        //    yield break;
        //}

        //private void SetDefultRefGenericDrugCategory()
        //{
        //    if (RequestDrug != null && RefGenericDrugCategory_1s != null)
        //    {
        //        RequestDrug.RefGenDrugCatID_1 = RefGenericDrugCategory_1s.FirstOrDefault().RefGenDrugCatID_1;
        //    }
        //}

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

        private void SaveRequest()
        {
            if (RequestDrug.ReqForTechID > 0)
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
                SaveRequestDrugForTechnicalService();
            }
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
            void onInitDlg(IStoreDeptRequestSearch proAlloc)
            {
                proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                proAlloc.V_MedProductType = V_MedProductType;
                proAlloc.RequestDruglist.Clear();
                proAlloc.RequestDruglist.TotalItemCount = Totalcount;
                proAlloc.RequestDruglist.PageIndex = 0;
                proAlloc.RequestDruglist.PageSize = 20;
                proAlloc.IsCreateNewListFromSelectExisting = bCreateNewListFromOld;

                if (results != null && results.Count > 0)
                {
                    foreach (RequestDrugInwardClinicDept p in results)
                    {
                        proAlloc.RequestDruglist.Add(p);
                    }
                }
            }
            GlobalsNAV.ShowDialog<IStoreDeptRequestSearch>(onInitDlg, null, false, true, Globals.GetHalfHeightAndThreeFourthWidthDefaultDialogViewSize());
        }

        private void GetInPatientRequestingDrugListByReqID(long RequestID)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetReqOutwardDrugClinicDeptPatientByReqService(RequestID, (long)AllLookupValues.RegistrationType.NOI_TRU
                        , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetReqOutwardDrugClinicDeptPatientByReqService(asyncResult);
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
                                    RequestDrug = new RequestDrugForTechnicalService();
                                }

                                GetListStaffFilter();
                                FillPagedCollectionAndGroup();

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

        private void RefeshRequest()
        {
            RequestDrug = new RequestDrugForTechnicalService();
            CurrentPatient = null;
            RequestDrug.ReqForTechID = 0;
            RequestDrug.Comment = "";
            RequestDrug.ReqDate = DateTime.Now;
            RequestDrug.StaffID = GetStaffLogin().StaffID;
            RequestDrug.ReqOutwardDetails = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
            RequestDrug.IsLock = false;
            ListRequestDrugDelete.Clear();

            if (StoreCbxStaff != null)
            {
                RequestDrug.InDeptStoreObject = StoreCbxStaff.FirstOrDefault();
            }
            if (StoreCbx != null)
            {
                RequestDrug.OutFromStoreObject = StoreCbx.FirstOrDefault();
            }
            FillPagedCollectionAndGroup();
        }

        private void DeleteRequestDrugForTechnicalService(long ReqForTechID)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeleteRequestDrugForTechnicalService(ReqForTechID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndDeleteRequestDrugForTechnicalService(asyncResult);
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
            if (RequestDrug.ReqForTechID > 0)
            {
                DeleteRequestDrugForTechnicalService(RequestDrug.ReqForTechID);
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
            if (RequestDrug.InDeptStoreID == null || RequestDrug.InDeptStoreID <= 0)
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
                    if (RequestDrug.ReqForTechID > 0)
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
                for (int i = 0; i < RequestDrug.ReqOutwardDetails.Count; i++)
                {
                    if (RequestDrug.ReqOutwardDetails[i].GenMedProductID > 0)
                    {
                        if (RequestDrug.ReqOutwardDetails[i].PrescribedQty <= 0)
                        {
                            results = false;
                            MessageBox.Show(string.Format(eHCMSResources.Z1775_G1_SLgChiDinhLonHon0, RequestDrug.ReqOutwardDetails[i].DisplayGridRowNumber.ToString(), RequestDrug.ReqOutwardDetails[i].RefGenericDrugDetail.BrandNameAndCode));
                            break;
                        }
                        if (RequestDrug.ReqOutwardDetails[i].ReqQty < 0)
                        {
                            results = false;
                            MessageBox.Show(string.Format(eHCMSResources.Z1124_G1_SLgYCLonHonBang0, RequestDrug.ReqOutwardDetails[i].DisplayGridRowNumber.ToString(), RequestDrug.ReqOutwardDetails[i].RefGenericDrugDetail.BrandNameAndCode));
                            break;
                        }
                        if (Globals.ServerConfigSection.InRegisElements.CheckMedicalInstructDate && Globals.ServerConfigSection.InRegisElements.NumOfOverDaysForMedicalInstructDate >= 0)
                        {
                            if ((RequestDrug.ReqOutwardDetails[i].MedicalInstructionDate.GetValueOrDefault() - Globals.GetCurServerDateTime().Date).Days > Globals.ServerConfigSection.InRegisElements.NumOfOverDaysForMedicalInstructDate)
                            {
                                error += "    " + RequestDrug.ReqOutwardDetails[i].RefGenericDrugDetail.BrandNameAndCode + Environment.NewLine;
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
                if (RequestDrug.ReqOutwardDetails.Any(x => x.ReqQty == 0))
                {
                    Globals.ShowMessage(eHCMSResources.A0538_G1_Msg_InfoSLgYCLonHon0, eHCMSResources.G0442_G1_TBao);
                    return false;
                }
            }
            return results;
        }

        private IEnumerator<IResult> DoGetStore_MedDept()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT, false, null, false, true);
            yield return paymentTypeTask;
            if ((V_MedProductType == (long)AllLookupValues.MedProductType.THUOC || V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU || V_MedProductType == (long)AllLookupValues.MedProductType.NUTRITION) && Globals.ServerConfigSection.MedDeptElements.IsEnableMedSubStorage)
            {
                StoreCbx = paymentTypeTask.LookupList.Where(x => V_MedProductType != 0 && (!x.IsMain || (x.IsMain && x.IsSubStorage))
                        && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString())
                        && (V_GroupTypes == 0 || x.V_GroupTypes == V_GroupTypes)).ToObservableCollection();//-- 06/01/2021 DatTB 
            }
            else
            {
                StoreCbx = paymentTypeTask.LookupList.Where(x => V_MedProductType != 0 && x.ListV_MedProductType != null
                        && x.ListV_MedProductType.Contains(V_MedProductType.ToString())
                        && (V_GroupTypes == 0 || x.V_GroupTypes == V_GroupTypes)).OrderByDescending(x => x.StoreID).ToObservableCollection(); //-- 06/01/2021 DatTB 
            }
            if (RequestDrug != null && StoreCbx != null)
            {
                RequestDrug.OutFromStoreObject = StoreCbx.FirstOrDefault();
            }
            if (RequestDrug.InDeptStoreObject != null && RequestDrug.InDeptStoreObject.DeptID > 0)
            {
                RequestDrug.InDeptStoreObject.RefDepartment = Globals.AllRefDepartmentList.Where(x => x.DeptID == RequestDrug.InDeptStoreObject.DeptID).FirstOrDefault();
            }
            yield break;
        }

        private IEnumerator<IResult> DoGetStore_ClinicDeptAll()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_CLINIC, false, null, false, false);
            yield return paymentTypeTask;
            Globals.allRefStorageWarehouseLocation = paymentTypeTask.LookupList;
            StoreCbxStaff = Globals.checkStoreWareHouse(V_MedProductType, false, false);
            if (StoreCbxStaff == null || StoreCbxStaff.Count < 1)
            {
                MessageBox.Show(eHCMSResources.A0110_G1_Msg_InfoChuaCauHinhTNKho);
                yield break;
            }
            var selectedStore = StoreCbxStaff.FirstOrDefault();
            V_GroupTypes = selectedStore.V_GroupTypes;
            RefeshRequest();
            yield break;
        }

        #endregion

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

        #endregion

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

            if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            {
                colMDoseStr.Visibility = Visibility.Visible;
                colADoseStr.Visibility = Visibility.Visible;
                colEDoseStr.Visibility = Visibility.Visible;
                colNDoseStr.Visibility = Visibility.Visible;
            }
            else
            {
                colMDoseStr.Visibility = Visibility.Collapsed;
                colADoseStr.Visibility = Visibility.Collapsed;
                colEDoseStr.Visibility = Visibility.Collapsed;
                colNDoseStr.Visibility = Visibility.Collapsed;
            }
        }

        public void grdReqOutwardDetails_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            ReqOutwardDrugClinicDeptPatient rowItem = e.Row.DataContext as ReqOutwardDrugClinicDeptPatient;

            // TxD 04/07/2018
            if (rowItem == null)
            {
                return;
            }

            rowItem.DisplayGridRowNumber = e.Row.GetIndex() + 1;

            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";

            //if (CheckUnVerifiedRows)
            //{
            //    if (!rowItem.ItemVerified)
            //    {
            //        e.Row.Background = new SolidColorBrush(Colors.Yellow);
            //    }
            //    else
            //    {
            //        e.Row.Background = new SolidColorBrush(Colors.White);
            //    }
            //}
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

        DataGridColumn prevWorkingColumn = null;

        public void grdReqOutwardDetails_CurrentCellChanged(object sender, EventArgs e)
        {
            if (sender == null)
                return;

            if (prevWorkingColumn == null)
            {
                prevWorkingColumn = ((DataGrid)sender).CurrentColumn;
                return;
            }

            int prevIdx = prevWorkingColumn.DisplayIndex;
            if (SelectedReqOutwardDrugClinicDeptPatient == null)
            {
                SelectedReqOutwardDrugClinicDeptPatient = copySelectedReqOutwardDrugClinicDeptPatient;
            }

            if ((sender as DataGrid).SelectedItem != null)
            {
                ischanged((sender as DataGrid).SelectedItem);
            }
            if (prevIdx == (int)QtyColumnIndexes.M_Dose_Idx)
            {
                Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.MDose, SelectedReqOutwardDrugClinicDeptPatient);
            }
            else if (prevIdx == (int)QtyColumnIndexes.A_Dose_Idx)
            {
                Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.ADose, SelectedReqOutwardDrugClinicDeptPatient);
            }
            else if (prevIdx == (int)QtyColumnIndexes.E_Dose_Idx)
            {
                Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.EDose, SelectedReqOutwardDrugClinicDeptPatient);
            }
            else if (prevIdx == (int)QtyColumnIndexes.N_Dose_Idx)
            {
                Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.NDose, SelectedReqOutwardDrugClinicDeptPatient);
            }
            else if (prevIdx == (int)QtyColumnIndexes.Pres_Qty_Idx)
            {
                if (RequestDrug != null && RequestDrug.ReqOutwardDetails != null && (sender as DataGrid).SelectedIndex != -1)
                {
                    ReqOutwardDrugClinicDeptPatient selItem = RequestDrug.ReqOutwardDetails[(sender as DataGrid).SelectedIndex];
                    selItem.ReqQty = selItem.PrescribedQty;
                }
            }
            else if (prevIdx == (int)QtyColumnIndexes.Req_Qty_Idx)
            {
                if (RequestDrug != null && RequestDrug.ReqOutwardDetails != null && (sender as DataGrid).SelectedIndex != -1)
                {
                    ReqOutwardDrugClinicDeptPatient selItem = RequestDrug.ReqOutwardDetails[(sender as DataGrid).SelectedIndex];
                    selItem.ReqQty = Globals.ChangeDoseStringToDecimal(selItem.ReqQtyStr);
                }
            }
            ConvertCeiling(SelectedReqOutwardDrugClinicDeptPatient);
            prevWorkingColumn = ((DataGrid)sender).CurrentColumn;

        }

        private void ConvertCeiling(ReqOutwardDrugClinicDeptPatient req)
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
            if (((ClinicDeptInPtReqForm_ForSmallProcedureView)this.GetView()).grdReqOutwardDetails != null)
            {
                if (MessageBox.Show(eHCMSResources.Z0554_G1_CoChacMuonXoaDongNayKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    RemoveMark(((ClinicDeptInPtReqForm_ForSmallProcedureView)this.GetView()).grdReqOutwardDetails.SelectedItem);

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
            if (((ClinicDeptInPtReqForm_ForSmallProcedureView)this.GetView()).grdReqOutwardDetails != null) {
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

        public void GetRemainingQtyForInPtRequestDrug()
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            List<ReqOutwardDrugClinicDeptPatient> RemainingList = null;
            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRemainingQtyForInPtRequestDrug(RequestDrug.InDeptStoreID, V_MedProductType, 0, Globals.DispatchCallback((asyncResult) =>
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

            void onInitDlg(IModify_ReqQty temp)
            {
                List<ReqOutwardDrugClinicDeptPatient> RequestDetails = new List<ReqOutwardDrugClinicDeptPatient>();
                RequestDetails = RequestDrug.ReqOutwardDetails.ToList();
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
                temp.RequestDrug.ReqOutwardDetails = RequestDrugDetails;//Lấy nội dung chi tiết phiếu yêu cầu để hiển thị trong View
            }
            GlobalsNAV.ShowDialog<IModify_ReqQty>(onInitDlg);
        }

        public void btnAutoReqQty(object sender, RoutedEventArgs e)
        {
            GetRemainingQtyForInPtRequestDrug();
        }

        public void btnNew(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.A0146_G1_Msg_ConfTaoMoiPh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                NewClick();
            }
        }

        private void NewClick()
        {
            strNote = "";
            RefeshRequest();
            hblPatientOther();
            SetCheckAllReqOutwardDetail();
            aucHoldConsultDoctor.setDefault();
            MedicalInstructionDateContent.DateTime = new DateTime(Globals.GetCurServerDateTime().Year, Globals.GetCurServerDateTime().Month, Globals.GetCurServerDateTime().Day, Globals.GetCurServerDateTime().Hour, Globals.GetCurServerDateTime().Minute, 0);
        }

        //private bool CheckValidForLoadRequest()
        //{
        //    if (RequestDrug == null)
        //    {
        //        return false;
        //    }
        //    if (RequestDrug.InDeptStoreID == null || RequestDrug.InDeptStoreID == 0)
        //    {
        //        Globals.ShowMessage(eHCMSResources.Z1129_G1_ChonKhoYC, eHCMSResources.T0074_G1_I);
        //        return false;
        //    }
        //    return true;
        //}

        //private void ReqOutwardDrugClinicFromInstruction()
        //{
        //    if (RequestDrug.RefGenDrugCatID_1 < 0 && V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
        //    {
        //        MessageBox.Show(eHCMSResources.Z1146_G1_ChonPhanNhomHg, eHCMSResources.G0442_G1_TBao);
        //        return;
        //    }

        //    this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
        //    try
        //    {
        //        var t = new Thread(() =>
        //        {
        //            using (var serviceFactory = new PharmacyClinicDeptServiceClient())
        //            {
        //                var contract = serviceFactory.ServiceInstance;
        //                contract.BeginReqOutwardDrugClinicFromInstruction(RequestDrug.FromDate, RequestDrug.ToDate, RequestDrug.RefGenDrugCatID_1, RequestDrug.InDeptStoreObject.DeptID.GetValueOrDefault(0), V_MedProductType, RequestDrug.IsInstructionFuture, Globals.DispatchCallback((asyncResult) =>
        //                {
        //                    try
        //                    {
        //                        var results = contract.EndReqOutwardDrugClinicFromInstruction(asyncResult);
        //                        if (results != null)
        //                        {
        //                            RequestDrug.ReqOutwardDetails = results.ToObservableCollection();
        //                        }
        //                        else
        //                        {
        //                            if (RequestDrug.ReqOutwardDetails != null)
        //                            {
        //                                RequestDrug.ReqOutwardDetails.Clear();
        //                            }
        //                        }
        //                        if (RequestDrug == null)
        //                        {
        //                            RequestDrug = new RequestDrugInwardClinicDept(); 
        //                        }

        //                        GetListStaffFilter();
        //                        FillPagedCollectionAndGroup();
        //                        ResetingOldListToCreateNewList(true, true);
        //                        SetCheckAllReqOutwardDetail();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                        ClientLoggerHelper.LogError(ex.ToString());
        //                    }
        //                    finally
        //                    {
        //                        this.HideBusyIndicator();
        //                    }
        //                }), null);
        //            }
        //        });

        //        t.Start();
        //    }
        //    catch (Exception ex)
        //    {
        //        Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //        ClientLoggerHelper.LogError(ex.ToString());
        //        this.HideBusyIndicator();
        //    }
        //}

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
            void onInitDlg(IClinicDeptReportDocumentPreview proAlloc)
            {
                proAlloc.ID = RequestDrug.ReqForTechID;
                proAlloc.LyDo = "PHIẾU LĨNH VẬT TƯ Y TẾT KÈM DVKT";
                proAlloc.eItem = ReportName.DRUGDEPT_REQUEST_FOR_TECHNICALSERVICE;
            }
            GlobalsNAV.ShowDialog<IClinicDeptReportDocumentPreview>(onInitDlg);
        }

        public void btnPreviewCT()
        {
            void onInitDlg(IClinicDeptReportDocumentPreview proAlloc)
            {
                switch (V_MedProductType)
                {
                    case (long)AllLookupValues.MedProductType.Y_CU:
                        proAlloc.LyDo = "PHIẾU LĨNH Y CỤ";
                        break;
                    case (long)AllLookupValues.MedProductType.HOA_CHAT:
                        proAlloc.LyDo = "PHIẾU LĨNH HÓA CHẤT";
                        break;
                    case (long)AllLookupValues.MedProductType.VTYT_TIEUHAO:
                        proAlloc.LyDo = "PHIẾU LĨNH VẬT TƯ Y TẾ TIÊU HAO";
                        break;
                    case (long)AllLookupValues.MedProductType.TIEM_NGUA:
                        proAlloc.LyDo = "PHIẾU LĨNH TIÊM NGỪA";
                        break;
                    case (long)AllLookupValues.MedProductType.MAU:
                        proAlloc.LyDo = "PHIẾU LĨNH MÁU";
                        break;
                    case (long)AllLookupValues.MedProductType.THANHTRUNG:
                        proAlloc.LyDo = "PHIẾU LĨNH THANH TRÙNG";
                        break;
                    default:
                        proAlloc.LyDo = "PHIẾU LĨNH";
                        break;
                }
                proAlloc.eItem = ReportName.DRUGDEPT_REQUEST_DETAILS;
            }
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

                        contract.BeginGetRequestPharmacyInPdfFormat(RequestDrug.ReqForTechID, Globals.DispatchCallback((asyncResult) =>
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
            ReqOutwardDrugClinicDeptPatient outItem = listObj as ReqOutwardDrugClinicDeptPatient;
            if (CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration != null && CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.Patient != null && CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.Patient.PatientID > 0)
            {
                if (outItem.CurPatientRegistration != null && outItem.CurPatientRegistration.Patient != null && outItem.CurPatientRegistration.Patient.PatientID > 0)
                {
                    return outItem.CurPatientRegistration.Patient.PatientID == CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.Patient.PatientID;
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

        //▼====: #002
        private bool _RequireDoctorAndDate = Globals.ServerConfigSection.ClinicDeptElements.RequireDoctorAndDateForMed;
        public bool RequireDoctorAndDate
        {
            get { return _RequireDoctorAndDate; }
            set
            {
                if (_RequireDoctorAndDate != value)
                {
                    _RequireDoctorAndDate = value;
                    NotifyOfPropertyChange(() => RequireDoctorAndDate);
                }
            }
        }

        private bool CheckValidMedicalInstructionDate(DateTime? aMedicalInstructionDate = null)
        {
            if (aMedicalInstructionDate == null && MedicalInstructionDateContent.DateTime != null)
            {
                MedicalInstructionDateContent.DateTime = Globals.ApplyValidMedicalInstructionDate(MedicalInstructionDateContent.DateTime.GetValueOrDefault(), CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration);
                aMedicalInstructionDate = MedicalInstructionDateContent.DateTime;
            }
            if (!RequireDoctorAndDate || aMedicalInstructionDate == null || !aMedicalInstructionDate.HasValue
                || CurrentReqOutwardDrugClinicDeptPatient == null || CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration == null)
                return true;
            if (CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.DischargeDetailRecCreatedDate.HasValue
                && CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.DischargeDetailRecCreatedDate != null
                && aMedicalInstructionDate > CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.DischargeDetailRecCreatedDate)
            {
                MessageBox.Show(eHCMSResources.Z2188_G1_NYLKhongLonHonNXV, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return false;
            }
            if ((!CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.AdmissionDate.HasValue || CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.AdmissionDate == null)
                && CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.ExamDate != null
                && aMedicalInstructionDate < CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.ExamDate)
            {
                MessageBox.Show(eHCMSResources.Z2187_G1_NYLKhongNhoHonNDK, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return false;
            }
            if (CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.AdmissionDate.HasValue
                && CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.AdmissionDate != null
                && aMedicalInstructionDate < CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.AdmissionDate)
            {
                MessageBox.Show(eHCMSResources.Z2183_G1_NgayYLKhongNhoHonNNV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            return true;
        }
        //▲====: #002

        //▼====== #009
        private bool SplitProductType(string sInput)
        {
            string[] mInputArray = sInput.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            if (mInputArray.Length >= 1)
            {
                foreach (var x in mInputArray)
                {
                    if (V_MedProductType == Convert.ToInt32(x.ToString()))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        //▲====== #009
        public void AddItem_Click(object sender, object e)
        {
            btnAddItem();
        }
        public void btnAddItem()
        {
            NotShowWhenUseCreateNewFromOld = true;
            if (CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail == null)
            {
                return;
            }
            else if (CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail.Remaining == 0)
            {
                MessageBox.Show(string.Format("{0} {1}", eHCMSResources.Z2779_G1_SoLuongKhoCCBang0, eHCMSResources.Z1418_G1_KgTheThemDuoc), eHCMSResources.G0442_G1_TBao);
                return;
            }

            if (SplitProductType(Globals.ServerConfigSection.ClinicDeptElements.ProductTypeNotDocAndDateReq))
            {
                if (RequireDoctorAndDate && (aucHoldConsultDoctor == null || aucHoldConsultDoctor.StaffID <= 0))
                {
                    MessageBox.Show(eHCMSResources.A0571_G1_Msg_InfoChonBSCDinh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    NotShowWhenUseCreateNewFromOld = false;
                    return;
                }
            }
            if (!CheckValidMedicalInstructionDate())
            {
                NotShowWhenUseCreateNewFromOld = false;
                return;
            }

            System.Diagnostics.Debug.WriteLine(" ========> btnAddItem  1 .....");
            CurrentReqOutwardDrugClinicDeptPatient.StaffID = GetStaffLogin().StaffID;
            if (CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail == null || CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail.GenMedProductID == 0 ||
                string.IsNullOrEmpty(CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail.Code))
            {
                CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = new RefGenMedProductDetails();

                Globals.ShowMessage(eHCMSResources.Z1147_G1_ChonHgCanYC, eHCMSResources.G0442_G1_TBao);
                NotShowWhenUseCreateNewFromOld = false;
                return;
            }
            if (CurrentReqOutwardDrugClinicDeptPatient.ReqQty <= 0)
            {
                Globals.ShowMessage(eHCMSResources.Z1161_G1_SLgCDinhKgHopLe, eHCMSResources.G0442_G1_TBao);
                NotShowWhenUseCreateNewFromOld = false;
                return;
            }
            if (CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration != null && CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.AdmissionDate != null
                && CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.AdmissionDate > MedicalInstructionDateContent.DateTime.GetValueOrDefault())
            {
                Globals.ShowMessage(eHCMSResources.Z2183_G1_NgayYLKhongNhoHonNNV, eHCMSResources.G0442_G1_TBao);
                NotShowWhenUseCreateNewFromOld = false;
                return;
            }
            if (CalByUnitUse)
            {
                CurrentReqOutwardDrugClinicDeptPatient.ReqQty = CurrentReqOutwardDrugClinicDeptPatient.ReqQty / (CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail.DispenseVolume == 0 ? 1 : (decimal)CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail.DispenseVolume);
                CurrentReqOutwardDrugClinicDeptPatient.ReqQty = Math.Round(CurrentReqOutwardDrugClinicDeptPatient.ReqQty, 2);
            }
            CurrentReqOutwardDrugClinicDeptPatient.DateTimeSelection = Globals.GetCurServerDateTime();
            if (RequestDrug == null)
            {
                RequestDrug = new RequestDrugForTechnicalService();
            }
            if (RequestDrug.ReqOutwardDetails == null)
            {
                RequestDrug.ReqOutwardDetails = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
            }
            var temp = RequestDrug.ReqOutwardDetails.Where(x => x.RefGenericDrugDetail != null && CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail != null && x.RefGenericDrugDetail.GenMedProductID == CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail.GenMedProductID && x.PtRegistrationID == CurrentReqOutwardDrugClinicDeptPatient.PtRegistrationID);
            {
                if (temp != null && temp.Count() > 0 && MessageBox.Show(eHCMSResources.A0774_G1_Msg_ConfThemHgDaCo, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    NotShowWhenUseCreateNewFromOld = false;
                    return;
                }
                CurrentReqOutwardDrugClinicDeptPatient.EntityState = EntityState.NEW;
                CurrentReqOutwardDrugClinicDeptPatient.PrescribedQty = CurrentReqOutwardDrugClinicDeptPatient.ReqQty;
                ConvertCeiling(CurrentReqOutwardDrugClinicDeptPatient);
                CurrentReqOutwardDrugClinicDeptPatient.DoctorStaff = new Staff
                {
                    StaffID = aucHoldConsultDoctor.StaffID,
                    FullName = aucHoldConsultDoctor.StaffName
                };
                CurrentReqOutwardDrugClinicDeptPatient.MedicalInstructionDate = MedicalInstructionDateContent.DateTime;
                CurrentReqOutwardDrugClinicDeptPatient.Notes = strNote;

                var item = CurrentReqOutwardDrugClinicDeptPatient.DeepCopy();
                RequestDrug.ReqOutwardDetails.Add(item);

                if (RequestDrug.ReqOutwardDetails.Count == 1)
                {
                    FillPagedCollectionAndGroup();
                }
            }
            CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = new RefGenMedProductDetails();
            CurrentReqOutwardDrugClinicDeptPatient.MDoseStr = "0";
            CurrentReqOutwardDrugClinicDeptPatient.ADoseStr = "0";
            CurrentReqOutwardDrugClinicDeptPatient.EDoseStr = "0";
            CurrentReqOutwardDrugClinicDeptPatient.NDoseStr = "0";
            CurrentReqOutwardDrugClinicDeptPatient.MDose = 0;
            CurrentReqOutwardDrugClinicDeptPatient.ADose = 0;
            CurrentReqOutwardDrugClinicDeptPatient.EDose = 0;
            CurrentReqOutwardDrugClinicDeptPatient.NDose = 0;
            CurrentReqOutwardDrugClinicDeptPatient.ReqQty = 0;
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
            NotShowWhenUseCreateNewFromOld = false;
        }
        AxGrid RootAxGrid;
        public void AxGrid_Loaded(object sender, RoutedEventArgs e)
        {
            RootAxGrid = sender as AxGrid;
        }
        public void AddItem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (RootAxGrid != null)
                {
                    RootAxGrid.DisableFirstNextFocus = true;
                }
            }
        }

        private void FillPagedCollectionAndGroup()
        {
            if (RequestDrug != null && RequestDrug.ReqOutwardDetails != null)
            {
                CVS_ReqDetails = new CollectionViewSource { Source = RequestDrug.ReqOutwardDetails };
                CollectionView_ReqDetails = (CollectionView)CVS_ReqDetails.View;
                FillGroupName();
                NotifyOfPropertyChange(() => CollectionView_ReqDetails);
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
            if (flagStoreSupplier && RequestDrug != null && RequestDrug.ReqOutwardDetails != null && RequestDrug.ReqOutwardDetails.Count > 0)
            {
                if (MessageBox.Show(eHCMSResources.Z2756_G1_Msg_ConfDoiKhoCC, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    RequestDrug.ReqOutwardDetails.Clear();
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
            CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = new RefGenMedProductDetails();
            CurrentReqOutwardDrugClinicDeptPatient.MDoseStr = "0";
            CurrentReqOutwardDrugClinicDeptPatient.MDose = 0;
            CurrentReqOutwardDrugClinicDeptPatient.ADoseStr = "0";
            CurrentReqOutwardDrugClinicDeptPatient.ADose = 0;
            CurrentReqOutwardDrugClinicDeptPatient.EDoseStr = "0";
            CurrentReqOutwardDrugClinicDeptPatient.EDose = 0;
            CurrentReqOutwardDrugClinicDeptPatient.NDoseStr = "0";
            CurrentReqOutwardDrugClinicDeptPatient.NDose = 0;
            CurrentReqOutwardDrugClinicDeptPatient.ReqQty = 0;
        }

        public void Handle(PatientSelectedGoToKhamBenh_InPt<PatientRegistration> message)
        {
            if (GetView() != null && message != null && message.Item != null)
            {
                CurrentPatient = message.Item.Patient;
                if (CurrentReqOutwardDrugClinicDeptPatient == null)
                {
                    CurrentReqOutwardDrugClinicDeptPatient = new ReqOutwardDrugClinicDeptPatient();
                }
                //▼====: #017
                if (message.Item.AdmissionInfo != null)
                {
                    IsCOVID = message.Item.AdmissionInfo.IsTreatmentCOVID;
                }
                else
                {
                    IsCOVID = false;
                }
                //▲====: #017
                CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration = message.Item;
                CurrentReqOutwardDrugClinicDeptPatient.PtRegistrationID = message.Item.PtRegistrationID;

                // TxD 17/11/2014: Filtering here as well when a NEW Patient has been selected to see any existing items (if any) of that Patient
                //if (ReqOutwardDrugClinicDeptPatientlst != null)
                //{
                //    if (ReqOutwardDrugClinicDeptPatientlst.Filter == null || CurrentlyViewReqByStaffIDFilter)
                //    {
                //        ReqOutwardDrugClinicDeptPatientlst.Filter = new Predicate<object>(ListOutDrugReqFilter);
                //        CurrentlyViewReqByStaffIDFilter = false;
                //    }

                //    ReqOutwardDrugClinicDeptPatientlst.Refresh();
                //}

                if (CollectionView_ReqDetails != null)
                {
                    if (CollectionView_ReqDetails.Filter == null || CurrentlyViewReqByStaffIDFilter)
                    {
                        CollectionView_ReqDetails.Filter = new Predicate<object>(ListOutDrugReqFilter);
                        CurrentlyViewReqByStaffIDFilter = false;
                    }

                    CollectionView_ReqDetails.Refresh();
                }
            }
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
            if (RequestDrug == null || RequestDrug.ReqOutwardDetails == null || RequestDrug.ReqOutwardDetails.Count <= 0)
            {
                return;
            }

            if (CheckAllOutwardDetail)
            {
                foreach (ReqOutwardDrugClinicDeptPatient item in RequestDrug.ReqOutwardDetails)
                {
                    item.Checked = true;
                }
            }
            else
            {
                foreach (ReqOutwardDrugClinicDeptPatient item in RequestDrug.ReqOutwardDetails)
                {
                    item.Checked = false;
                }
            }
        }
        //public void chkInstructionFuture_Click(object sender, RoutedEventArgs e)
        //{
        //    if (RequestDrug == null || RequestDrug.InDeptStoreObject == null || RequestDrug.InDeptStoreObject.RefDepartment == null)
        //    {
        //        return;
        //    }
        //    if (RequestDrug.ReqOutwardDetails.Count > 0 && !IsNotInstructionFuture)
        //    {
        //        if (MessageBox.Show("Thay đổi load y lệnh tương lai cần load lại y lệnh", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
        //        {
        //            RequestDrug.ReqOutwardDetails.Clear();
        //            ClearAutoComplete();
        //        }
        //        else
        //        {
        //            RequestDrug.IsInstructionFuture = !RequestDrug.IsInstructionFuture;
        //        }
        //    }

        //    SetTimeWhenReloadStore();
        //}
        //private void SetTimeWhenReloadStore()
        //{
        //    if (RequestDrug.IsInstructionFuture && !IsNotInstructionFuture)
        //    {
        //        RequestDrug.ReqDate = Globals.GetCurServerDateTime().AddDays(1);
        //        RequestDrug.FromDate = Globals.GetCurServerDateTime().AddDays(1);
        //        RequestDrug.ToDate = Globals.GetCurServerDateTime().AddDays(1);
        //    }
        //    else
        //    {
        //        RequestDrug.ReqDate = Globals.GetCurServerDateTime();
        //        RequestDrug.FromDate = Globals.GetCurServerDateTime();
        //        RequestDrug.ToDate = Globals.GetCurServerDateTime();
        //    }
        //}
        private void SetCheckAllReqOutwardDetail()
        {
            if (RequestDrug == null || RequestDrug.ReqOutwardDetails == null || RequestDrug.ReqOutwardDetails.Count <= 0)
            {
                CheckAllOutwardDetail = false;
                return;
            }

            CheckAllOutwardDetail = RequestDrug.ReqOutwardDetails.All(x => x.Checked);
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
            if (((ClinicDeptInPtReqForm_ForSmallProcedureView)GetView()).grdReqOutwardDetails != null)
            {
                if (RequestDrug.CanSave)
                {
                    ((ClinicDeptInPtReqForm_ForSmallProcedureView)GetView()).grdReqOutwardDetails.Columns[(int)DataGridCol.ColDelete].Visibility = Visibility.Visible;
                    ((ClinicDeptInPtReqForm_ForSmallProcedureView)GetView()).grdReqOutwardDetails.Columns[(int)DataGridCol.ColMultiDelete].Visibility = Visibility.Visible;
                }
                else
                {
                    ((ClinicDeptInPtReqForm_ForSmallProcedureView)GetView()).grdReqOutwardDetails.Columns[(int)DataGridCol.ColDelete].Visibility = Visibility.Collapsed;
                    ((ClinicDeptInPtReqForm_ForSmallProcedureView)GetView()).grdReqOutwardDetails.Columns[(int)DataGridCol.ColMultiDelete].Visibility = Visibility.Collapsed;
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

        //private void FillGroupName()
        //{
        //    if (ReqOutwardDrugClinicDeptPatientlst != null && ReqOutwardDrugClinicDeptPatientlst.Count > 0)
        //    {
        //        ReqOutwardDrugClinicDeptPatientlst.GroupDescriptions.Clear();
        //        ReqOutwardDrugClinicDeptPatientlst.SortDescriptions.Clear();
        //        ReqOutwardDrugClinicDeptPatientlst.GroupDescriptions.Add(new Common.PagedCollectionView.PropertyGroupDescription("CurPatientRegistration.Patient.PatientCodeAndName"));

        //        // TxD 17/11/2014: Commented out the following sort order because Filter has been applied to view and work with each Group at a time.
        //        //KMx: Sort theo thời gian thêm thuốc (15/11/2014 17:21).
        //        //ReqOutwardDrugClinicDeptPatientlst.SortDescriptions.Add(new System.ComponentModel.SortDescription("DateTimeSelection", System.ComponentModel.ListSortDirection.Descending));

        //        ReqOutwardDrugClinicDeptPatientlst.Filter = null;

        //    }
        //}

        private void FillGroupName()
        {
            if (CollectionView_ReqDetails != null)
            {
                CollectionView_ReqDetails.GroupDescriptions.Clear();
                CollectionView_ReqDetails.SortDescriptions.Clear();
                CollectionView_ReqDetails.GroupDescriptions.Add(new System.Windows.Data.PropertyGroupDescription("CurPatientRegistration.Patient.PatientCodeAndName"));

                // TxD 17/11/2014: Commented out the following sort order because Filter has been applied to view and work with each Group at a time.
                //KMx: Sort theo thời gian thêm thuốc (15/11/2014 17:21).
                //ReqOutwardDrugClinicDeptPatientlst.SortDescriptions.Add(new System.ComponentModel.SortDescription("DateTimeSelection", System.ComponentModel.ListSortDirection.Descending));

                CollectionView_ReqDetails.Filter = null;
            }
        }

        public void hblPatientOther()
        {
            CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration = null;
            CurrentReqOutwardDrugClinicDeptPatient.PtRegistrationID = 0;
            CurrentPatient = null;
            //if (ReqOutwardDrugClinicDeptPatientlst != null && ReqOutwardDrugClinicDeptPatientlst.CanFilter)
            //{
            //    ReqOutwardDrugClinicDeptPatientlst.Filter = new Predicate<object>(ListOutDrugReqFilter);
            //    CurrentlyViewReqByStaffIDFilter = false;
            //    ReqOutwardDrugClinicDeptPatientlst.Refresh();
            //}

            if (CollectionView_ReqDetails != null)
            {
                CollectionView_ReqDetails.Filter = new Predicate<object>(ListOutDrugReqFilter);
                CurrentlyViewReqByStaffIDFilter = false;
                CollectionView_ReqDetails.Refresh();
            }
        }

        public void btnViewAll()
        {
            //if (ReqOutwardDrugClinicDeptPatientlst != null && ReqOutwardDrugClinicDeptPatientlst.CanFilter)
            //{
            //    ReqOutwardDrugClinicDeptPatientlst.Filter = null;
            //    ReqOutwardDrugClinicDeptPatientlst.Refresh();
            //    FillPagedCollectionAndGroup();
            //}

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
            //if (ReqOutwardDrugClinicDeptPatientlst == null || ReqOutwardDrugClinicDeptPatientlst.Groups == null || ReqOutwardDrugClinicDeptPatientlst.Groups.Count <= 0)
            //{
            //    return;
            //}

            if (CollectionView_ReqDetails == null || CollectionView_ReqDetails.Groups == null || CollectionView_ReqDetails.Groups.Count <= 0)
            {
                return;
            }

            bAllGroupsAlreadyClosed = bClosed;

            //AxDataGridNy theGrid = ((ClinicDeptInPtReqForm_ForSmallProcedureViewModel)this.GetView()).grdReqOutwardDetails;
            ////ChangedWPF - CMN
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
            txt = (sender as TextBox).Text;

            System.Diagnostics.Debug.WriteLine(" ====> AxTextBox_LostFocus 1 .....");

            if (!string.IsNullOrEmpty(txt))
            {
                string Code = Globals.FormatCode(V_MedProductType, txt);
                SearchRefGenMedProductDetails(Code, true);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(" ====> AxTextBox_LostFocus 2 .....");
                CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = new RefGenMedProductDetails();
            }
        }

        public void AutoCompleteBox_Populating(object sender, PopulatingEventArgs e)
        {
            e.Cancel = true;
            BrandName = e.Parameter;
            SearchRefGenMedProductDetails(BrandName, false);
        }

        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (RefGenMedProductDetails == null || RefGenMedProductDetails.Count < 1)
            {
                return;
            }
            RefGenMedProductDetails obj = acbAutoDrug_Text.SelectedItem as RefGenMedProductDetails;
            if (obj != null && CurrentReqOutwardDrugClinicDeptPatient != null)
            {
                CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = obj;
            }
            AutoDrug_LostFocus(null, null);
            RefGenMedProductDetails.Clear();
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

        //public void FilterStaff_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    //FilterByStaff();
        //    //FillPagedCollectionAndGroup();
        //    //if (ReqOutwardDrugClinicDeptPatientlst != null && ReqOutwardDrugClinicDeptPatientlst.CanFilter)
        //    //{
        //    //    ReqOutwardDrugClinicDeptPatientlst.Filter = new Predicate<object>(ListRequestByStaffIDFilter);
        //    //    ReqOutwardDrugClinicDeptPatientlst.Refresh();
        //    //    CurrentlyViewReqByStaffIDFilter = true;
        //    //}

        //    if (CollectionView_ReqDetails != null && CollectionView_ReqDetails.CanFilter)
        //    {
        //        CollectionView_ReqDetails.Filter = new Predicate<object>(ListRequestByStaffIDFilter);
        //        CollectionView_ReqDetails.Refresh();
        //        CurrentlyViewReqByStaffIDFilter = true;
        //    }
        //}

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

        public void btnChangeDoctor(object sender, RoutedEventArgs e)
        {
            if (RequestDrug == null || RequestDrug.ReqOutwardDetails == null || RequestDrug.ReqOutwardDetails.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0910_G1_Msg_InfoKhTheDoiBSCDinh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            ObservableCollection<ReqOutwardDrugClinicDeptPatient> SelectReqOutwardDetails = RequestDrug.ReqOutwardDetails.Where(x => x.Checked).ToObservableCollection();

            if (SelectReqOutwardDetails == null || SelectReqOutwardDetails.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0595_G1_Msg_InfoChonDongCanDoiBS, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (SelectReqOutwardDetails.Where(x => x.IntPtDiagDrInstructionID.GetValueOrDefault() > 0).Count() > 0)
            {
                MessageBox.Show(eHCMSResources.Z2897_G1_KhongTheCapNhatYLCuaBS, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            foreach (ReqOutwardDrugClinicDeptPatient item in SelectReqOutwardDetails)
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
            if (RequestDrug == null || RequestDrug.ReqOutwardDetails == null || RequestDrug.ReqOutwardDetails.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0912_G1_Msg_InfoDoiNgYLenhPhLanh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            ObservableCollection<ReqOutwardDrugClinicDeptPatient> SelectReqOutwardDetails = RequestDrug.ReqOutwardDetails.Where(x => x.Checked).ToObservableCollection();

            if (SelectReqOutwardDetails == null || SelectReqOutwardDetails.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0597_G1_Msg_InfoChonDongCanDoiNgYL, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            //▼====: #014
            if (SelectReqOutwardDetails.Where(x => x.IntPtDiagDrInstructionID.GetValueOrDefault() > 0).Count() > 0)
            {
                MessageBox.Show(eHCMSResources.Z2897_G1_KhongTheCapNhatYLCuaBS, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration != null && CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.AdmissionDate.HasValue
                && CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.AdmissionDate != null
                && MedicalInstructionDateContent.DateTime < CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.AdmissionDate)
            {
                MessageBox.Show(eHCMSResources.Z2183_G1_NgayYLKhongNhoHonNNV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            //▲====: #014
            foreach (ReqOutwardDrugClinicDeptPatient item in SelectReqOutwardDetails)
            {
                item.MedicalInstructionDate = MedicalInstructionDateContent.DateTime;

                ischanged(item);
            }

            MessageBox.Show(eHCMSResources.Z1150_G1_DoiNgYLenhThCong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        }

        public void btnChangeNote(object sender, RoutedEventArgs e)
        {
            if (RequestDrug == null || RequestDrug.ReqOutwardDetails == null || RequestDrug.ReqOutwardDetails.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0911_G1_Msg_InfoKhTheDoiGhiChu, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            ObservableCollection<ReqOutwardDrugClinicDeptPatient> SelectReqOutwardDetails = RequestDrug.ReqOutwardDetails.Where(x => x.Checked).ToObservableCollection();

            if (SelectReqOutwardDetails == null || SelectReqOutwardDetails.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0596_G1_Msg_InfoChonDongCanDoiGhiChu, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            foreach (ReqOutwardDrugClinicDeptPatient item in SelectReqOutwardDetails)
            {
                item.Notes = strNote;

                ischanged(item);
            }

            MessageBox.Show(eHCMSResources.Z1151_G1_DoiGChuThCong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        }

        public void tbxMDoseStr_LostFocus(object sender, RoutedEventArgs e)
        {
            Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.MDose, CurrentReqOutwardDrugClinicDeptPatient);
        }

        public void tbxADoseStr_LostFocus(object sender, RoutedEventArgs e)
        {
            Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.ADose, CurrentReqOutwardDrugClinicDeptPatient);
        }

        public void tbxEDoseStr_LostFocus(object sender, RoutedEventArgs e)
        {
            Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.EDose, CurrentReqOutwardDrugClinicDeptPatient);
        }

        public void tbxNDoseStr_LostFocus(object sender, RoutedEventArgs e)
        {
            Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.NDose, CurrentReqOutwardDrugClinicDeptPatient);
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
            copySelectedReqOutwardDrugClinicDeptPatient = SelectedReqOutwardDrugClinicDeptPatient;
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
        public void Handle(ReloadInPatientRequestingDrugListByReqID message)
        {
            GetInPatientRequestingDrugListByReqID(RequestDrug.ReqForTechID);
        }

        private ReqOutwardDrugClinicDeptPatient _copySelectedReqOutwardDrugClinicDeptPatient;
        public ReqOutwardDrugClinicDeptPatient copySelectedReqOutwardDrugClinicDeptPatient
        {
            get { return _copySelectedReqOutwardDrugClinicDeptPatient; }
            set
            {
                if (_copySelectedReqOutwardDrugClinicDeptPatient != value)
                {
                    _copySelectedReqOutwardDrugClinicDeptPatient = value;
                    NotifyOfPropertyChange(() => copySelectedReqOutwardDrugClinicDeptPatient);
                }
            }
        }

        private bool _NotShowWhenUseCreateNewFromOld = false;
        public bool NotShowWhenUseCreateNewFromOld
        {
            get { return _NotShowWhenUseCreateNewFromOld; }
            set
            {
                if (_NotShowWhenUseCreateNewFromOld != value)
                {
                    _NotShowWhenUseCreateNewFromOld = value;
                }
                NotifyOfPropertyChange(() => NotShowWhenUseCreateNewFromOld);
            }
        }

        private void ControlStatusCoupon()
        {
            //StatusCoupon = "";
            //if (RequestDrug.ReqOutwardDetails.Count > 0 && (RequestDrug.IsApproved == null || RequestDrug.IsApproved == false))
            //{
            //    StatusCoupon = eHCMSResources.Z2425_G1_ChuaDuyetPhieuLinh;
            //    IsStatusCoupon = true;
            //}
            //else if (RequestDrug.ReqOutwardDetails.Count > 0 && (RequestDrug.IsApproved == true && RequestDrug.DaNhanHang == false))
            //{
            //    StatusCoupon = eHCMSResources.Z2423_G1_DaDuyetLinh;
            //    IsStatusCoupon = true;
            //}
            //else if (RequestDrug.ReqOutwardDetails.Count > 0 && (RequestDrug.IsApproved == true && RequestDrug.DaNhanHang == true))
            //{
            //    StatusCoupon = eHCMSResources.Z2424_G1_DaDuyetVuiLongNhap;
            //    IsStatusCoupon = true;
            //}
            //if (NotShowWhenUseCreateNewFromOld)
            //{
            //    StatusCoupon = "";
            //    IsStatusCoupon = false;
            //}
            NotShowWhenUseCreateNewFromOld = false;
        }
        //▲====== #005

        //▼====== #008
        private ObservableCollection<RefGenMedProductDetails> _RefGenMedProductDetailsSum;
        public ObservableCollection<RefGenMedProductDetails> RefGenMedProductDetailsListSum
        {
            get { return _RefGenMedProductDetailsSum; }
            set
            {
                if (_RefGenMedProductDetailsSum != value)
                    _RefGenMedProductDetailsSum = value;
                NotifyOfPropertyChange(() => RefGenMedProductDetailsListSum);
            }
        }

        private void GroupRemaining(IList<RefGenMedProductDetails> results)
        {
            var ListRefGMP = from RefGMP in results
                             group RefGMP by new
                             {
                                 RefGMP.GenMedProductID,
                                 RefGMP.BrandName,
                                 RefGMP.SelectedUnit.UnitName,
                                 RefGMP.RequestQty,
                                 RefGMP.Code,
                                 RefGMP.ProductCodeRefNum,
                                 RefGMP.RefGenMedDrugDetails.Content,
                                 RefGMP.GenericName
                             } into RefGMPGroup
                             select new
                             {
                                 Remaining = RefGMPGroup.Sum(groupItem => groupItem.Remaining),
                                 GenMedProductID = RefGMPGroup.Key.GenMedProductID,
                                 UnitName = RefGMPGroup.Key.UnitName,
                                 BrandName = RefGMPGroup.Key.BrandName,
                                 GenericName = RefGMPGroup.Key.GenericName,
                                 Content = RefGMPGroup.Key.Content,
                                 Code = RefGMPGroup.Key.Code,
                                 Qty = RefGMPGroup.Key.RequestQty,
                                 ProductCodeRefNum = RefGMPGroup.Key.ProductCodeRefNum
                             };
            RefGenMedProductDetailsListSum = new ObservableCollection<RefGenMedProductDetails>();
            foreach (var Details in ListRefGMP)
            {
                RefGenMedProductDetails item = new RefGenMedProductDetails();
                item.GenMedProductID = Details.GenMedProductID;
                item.BrandName = Details.BrandName;
                item.SelectedUnit = new RefUnit();
                item.SelectedUnit.UnitName = Details.UnitName;
                item.Code = Details.Code;
                item.Remaining = Details.Remaining;
                item.RequestQty = Details.Qty;
                item.ProductCodeRefNum = Details.ProductCodeRefNum;
                RefGenMedProductDetailsListSum.Add(item);
            }
        }

        AxComboBox CbxOutFromStoreObject;
        public void cbxStoreSupplier_Loaded(object sender, RoutedEventArgs e)
        {
            CbxOutFromStoreObject = sender as AxComboBox;
        }

        private void SearchRefGenMedProductDetails(string Name, bool? IsCode, long? OutwardDrugClinicDeptTemplateID = null)
        {
            long OutFromStoreObject = 1;
            if (IsCode == false && BrandName.Length < 1)
            {
                return;
            }
            long? RefGenDrugCatID_1 = null;
            if (StoreCbx != null)
            {
                OutFromStoreObject = (long)CbxOutFromStoreObject.SelectedValue;
            }
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDrugForSellVisitorAutoComplete_ForRequestDrugDept_V2(IsSearchByGenericName, null, Name
                        , OutFromStoreObject, V_MedProductType, RefGenDrugCatID_1
                        , null, IsCode, null, null, OutwardDrugClinicDeptTemplateID
                        , !IsNotCheckRemain
                        , IsCOVID
                        , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetDrugForSellVisitorAutoComplete_ForRequestDrugDept_V2(asyncResult);
                                if (results != null && results.Count > 0)
                                {
                                    GroupRemaining(results);
                                    if (OutwardDrugClinicDeptTemplateID.GetValueOrDefault(0) > 0)
                                    {
                                        foreach (var aItem in RefGenMedProductDetailsListSum)
                                        {
                                            ReqOutwardDrugClinicDeptPatient CurrentReqOutwardDrug = new ReqOutwardDrugClinicDeptPatient
                                            {
                                                RefGenericDrugDetail = aItem
                                            };
                                            CurrentReqOutwardDrugClinicDeptPatient.DateTimeSelection = Globals.GetCurServerDateTime();
                                            CurrentReqOutwardDrug.EntityState = EntityState.NEW;
                                            CurrentReqOutwardDrug.PrescribedQty = results.FirstOrDefault(x => x.GenMedProductID == aItem.GenMedProductID).RequestQty;
                                            CurrentReqOutwardDrug.ReqQty = RefGenMedProductDetailsListSum.Where(x => x.GenMedProductID == aItem.GenMedProductID).Sum(x => x.Remaining) < CurrentReqOutwardDrug.PrescribedQty ? RefGenMedProductDetailsListSum.Where(x => x.GenMedProductID == aItem.GenMedProductID).Sum(x => x.Remaining) : CurrentReqOutwardDrug.PrescribedQty;
                                            if (CalByUnitUse)
                                            {
                                                CurrentReqOutwardDrugClinicDeptPatient.ReqQty = CurrentReqOutwardDrugClinicDeptPatient.ReqQty / (CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail.DispenseVolume == 0 ? 1 : (decimal)CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail.DispenseVolume);
                                                CurrentReqOutwardDrugClinicDeptPatient.ReqQty = Math.Round(CurrentReqOutwardDrugClinicDeptPatient.ReqQty, 2);
                                            }
                                            ConvertCeiling(CurrentReqOutwardDrug);
                                            CurrentReqOutwardDrug.DoctorStaff = new Staff
                                            {
                                                StaffID = aucHoldConsultDoctor.StaffID,
                                                FullName = aucHoldConsultDoctor.StaffName
                                            };
                                            CurrentReqOutwardDrug.MedicalInstructionDate = MedicalInstructionDateContent.DateTime;
                                            CurrentReqOutwardDrug.Notes = strNote;
                                            var item = CurrentReqOutwardDrug.DeepCopy();
                                            item.CurPatientRegistration = CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration;
                                            RequestDrug.ReqOutwardDetails.Add(item);
                                        }
                                        if (RequestDrug.ReqOutwardDetails.Count == 1)
                                        {
                                            FillPagedCollectionAndGroup();
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
                                    else if (IsCode == true)
                                    {
                                        if (results != null && results.Count > 0)
                                        {
                                            if (CurrentReqOutwardDrugClinicDeptPatient == null)
                                            {
                                                CurrentReqOutwardDrugClinicDeptPatient = new ReqOutwardDrugClinicDeptPatient();
                                            }
                                            CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = results.FirstOrDefault();

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
                                        RefGenMedProductDetails = RefGenMedProductDetailsListSum;
                                        acbAutoDrug_Text.PopulateComplete();
                                    }
                                }
                                else
                                {
                                    RefGenMedProductDetails = new ObservableCollection<RefGenMedProductDetails>();
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

        AxComboBox CbxStoreRequest;
        public void cbxStoreRequest_Loaded(object sender, RoutedEventArgs e)
        {
            CbxStoreRequest = sender as AxComboBox;
        }

        public void cbxStoreRequest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbxStoreRequest = (ComboBox)sender;
            if (cbxStoreRequest == null)
            {
                return;
            }

            RefStorageWarehouseLocation temp = (RefStorageWarehouseLocation)cbxStoreRequest.SelectedItem;
            if (temp == null)
            {
                if (StoreCbxStaff != null)
                {
                    V_GroupTypes = StoreCbxStaff.FirstOrDefault().V_GroupTypes;
                }
            }
            else
            {
                V_GroupTypes = temp.V_GroupTypes;
            }
            Coroutine.BeginExecute(DoGetStore_MedDept());
        }

        public bool CheckTrung(ObservableCollection<ReqOutwardDrugClinicDeptPatient> ReqOutwardDetailsObj, ReqOutwardDrugClinicDeptPatient item)
        {
            if (ReqOutwardDetailsObj != null && item != null)
            {
                foreach (var detail in ReqOutwardDetailsObj)
                {
                    if (detail.GenMedProductID == item.GenMedProductID && detail.ReqQty == item.ReqQty
                        && string.Compare(detail.ADoseStr, item.ADoseStr) == 0
                        && string.Compare(detail.EDoseStr, item.EDoseStr) == 0
                        && string.Compare(detail.MDoseStr, item.MDoseStr) == 0
                        && string.Compare(detail.NDoseStr, item.NDoseStr) == 0)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        //▲===== #015
        private bool _isEnableLoadInstruction = true;
        public bool isEnableLoadInstruction
        {
            get
            {
                return _isEnableLoadInstruction;
            }
            set
            {
                if (_isEnableLoadInstruction != value)
                {
                    _isEnableLoadInstruction = value;
                    NotifyOfPropertyChange(() => isEnableLoadInstruction);
                }
            }
        }

        public bool IsSearchWithoutRemainingForVPPAndVTTH
        {
            get
            {
                return Globals.ServerConfigSection.CommonItems.IsApplyCreateRequestForEstimation
                    && (V_MedProductType == (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM || V_MedProductType == (long)AllLookupValues.MedProductType.VATTU_TIEUHAO);
            }
        }

        private bool _FormEditorIsEnabled = false;
        public bool FormEditorIsEnabled
        {
            get
            {
                return _FormEditorIsEnabled;
            }
            set
            {
                _FormEditorIsEnabled = value;
                NotifyOfPropertyChange(() => FormEditorIsEnabled);
            }
        }

        private long _PtRegDetailID;
        public long PtRegDetailID
        {
            get { return _PtRegDetailID; }
            set
            {
                if (_PtRegDetailID != value)
                {
                    _PtRegDetailID = value;
                }
                NotifyOfPropertyChange(() => PtRegDetailID);
            }
        }

        public void GetRequestDrugForTechnicalServicePtRegDetailID(long PtRegDetailID)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRequestDrugForTechnicalServicePtRegDetailID(PtRegDetailID, (long)AllLookupValues.RegistrationType.NOI_TRU
                            , Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var results = contract.EndGetRequestDrugForTechnicalServicePtRegDetailID(asyncResult);
                                    if (results != null)
                                    {
                                        RequestDrug = results;
                                        GetInPatientRequestingDrugListByReqID(RequestDrug.ReqForTechID);
                                    }
                                    else
                                    {
                                        RefeshRequest();
                                    }
                                    ListRequestDrugDelete.Clear();
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

        private void SaveRequestDrugForTechnicalService()
        {
            if (PtRegDetailID == 0)
            {
                return;
            }
            RequestDrug.PtRegDetailID = PtRegDetailID;
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

            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSaveRequestDrugForTechnicalService(RequestDrug, V_MedProductType, (long)AllLookupValues.RegistrationType.NOI_TRU
                            , Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    RequestDrugForTechnicalService RequestOut;
                                    contract.EndSaveRequestDrugForTechnicalService(out RequestOut, asyncResult);
                                    if (RequestOut != null)
                                    {
                                        RequestDrug = RequestOut;
                                        RequestDrug.ReqOutwardDetails = RequestOut.ReqOutwardDetails.DeepCopy();

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
    }
}
