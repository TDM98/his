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
 * 20210109 #001 TNHX: Hoàn thiện chức năng lập suất ăn
 */
namespace aEMR.StoreDept.ViewModels
{
    [Export(typeof(IClinicDeptInPtReqForm_Food)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ClinicDeptInPtReqForm_FoodViewModel : Conductor<object>, IClinicDeptInPtReqForm_Food
        , IHandle<DrugDeptCloseSearchRequestEvent>
    {
        [ImportingConstructor]
        public ClinicDeptInPtReqForm_FoodViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Authorization();

            SearchCriteria = new RequestSearchCriteria();
            SearchCriteria.FromDate = DateTime.Now.Date - new TimeSpan(3, 0, 0, 0);
            SearchCriteria.ToDate = DateTime.Now;

            ListCurrentRequestDelete = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
            CurrentRequest = new RequestFoodClinicDept();
            CurrentReqOutwardDrugClinicDeptPatient = new ReqOutwardDrugClinicDeptPatient();

            RefGenMedProductDetails = new ObservableCollection<RefGenMedProductDetails>();

            ListStaff = new ObservableCollection<Staff>();

            MedicalInstructionDateContent = Globals.GetViewModel<IMinHourDateControl>();
            MedicalInstructionDateContent.DateTime = new DateTime(Globals.GetCurServerDateTime().Year, Globals.GetCurServerDateTime().Month, Globals.GetCurServerDateTime().Day, Globals.GetCurServerDateTime().Hour, Globals.GetCurServerDateTime().Minute, 0);

            aucHoldConsultDoctor = Globals.GetViewModel<IAucHoldConsultDoctor>();
            aucHoldConsultDoctor.StaffCatType = (long)V_StaffCatType.BacSi;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            RefeshRequest();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
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

        private ObservableCollection<ReqOutwardDrugClinicDeptPatient> ListCurrentRequestDelete;

        private RequestFoodClinicDept _CurrentRequest;
        public RequestFoodClinicDept CurrentRequest
        {
            get
            {
                return _CurrentRequest;
            }
            set
            {
                if (_CurrentRequest != value)
                {
                    _CurrentRequest = value;
                    NotifyOfPropertyChange(() => CurrentRequest);
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

        private ReqFoodClinicDeptDetail _SelectedReqOutwardDrugClinicDeptPatient;
        public ReqFoodClinicDeptDetail SelectedReqOutwardDrugClinicDeptPatient
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
        private Staff GetStaffLogin()
        {
            return Globals.LoggedUserAccount.Staff;
        }

        private void SaveRequest()
        {
            if (CurrentRequest.ReqFoodClinicDeptID == 0 && (CurrentRequest.RequestDetails == null || (CurrentRequest.RequestDetails != null && CurrentRequest.RequestDetails.Count() == 0)))
            {
                MessageBox.Show(eHCMSResources.Z2299_G1_KhongCoGiDeLuu);
                return;
            }
            if (CurrentRequest.ReqFoodClinicDeptID > 0)
            {
                //if (MessageBox.Show(eHCMSResources.A0138_G1_Msg_ConfLuuThayDoiTrenPhYC, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                //{
                return;
                //}
            }
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSaveRequestFoodClinicDept(CurrentRequest, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                RequestFoodClinicDept RequestOut;
                                contract.EndSaveRequestFoodClinicDept(out RequestOut, asyncResult);
                                CurrentRequest = RequestOut;
                                Globals.ShowMessage(eHCMSResources.Z1562_G1_DaLuu, eHCMSResources.G0442_G1_TBao);
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
            catch (Exception ex)
            {
                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                ClientLoggerHelper.LogError(ex.ToString());
                this.HideBusyIndicator();
            }
        }

        public void OpenPopUpSearchRequestInvoice(IList<RequestFoodClinicDept> results, int Totalcount, bool bCreateNewListFromOld)
        {
            void onInitDlg(IStoreDeptRequestSearch proAlloc)
            {
                proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                proAlloc.RequestDruglist.Clear();
                proAlloc.RequestDruglist.TotalItemCount = Totalcount;
                proAlloc.RequestDruglist.PageIndex = 0;
                proAlloc.RequestDruglist.PageSize = 20;
                proAlloc.IsCreateNewListFromSelectExisting = bCreateNewListFromOld;

                if (results != null && results.Count > 0)
                {
                    foreach (RequestFoodClinicDept p in results)
                    {
                        RequestDrugInwardClinicDept temp = new RequestDrugInwardClinicDept();
                        temp.ReqDrugInClinicDeptID = p.ReqFoodClinicDeptID;
                        temp.ReqDate = p.ReqDate;
                        temp.ReqStatus = p.ReqStatus;
                        temp.ReqNumCode = p.ReqNumCode;
                        temp.Comment = p.Comment;
                        temp.ToDate = p.ToDate;
                        temp.DaNhanHang = p.DaNhanHang;
                        temp.IsApproved = p.IsApproved;
                        temp.SelectedStaff = p.SelectedStaff;
                        proAlloc.RequestDruglist.Add(temp);
                    }
                }
            }
            GlobalsNAV.ShowDialog<IStoreDeptRequestSearch>(onInitDlg, null, false, true, Globals.GetHalfHeightAndThreeFourthWidthDefaultDialogViewSize());
        }

        private void SearchRequestFoodClinicDept(int PageIndex, int PageSize, bool bCreateNewListFromOld = false)
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

            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSearchRequestFoodClinicDept(SearchCriteria, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int Total = 0;
                                var results = contract.EndSearchRequestFoodClinicDept(out Total, asyncResult);
                                if (results != null && results.Count > 0)
                                {
                                    if (results.Count > 1)
                                    {
                                        //mo pop up tim
                                        this.HideBusyIndicator();
                                        OpenPopUpSearchRequestInvoice(results.ToList(), Total, bCreateNewListFromOld);
                                    }
                                    else
                                    {
                                        this.HideBusyIndicator();
                                        CurrentRequest = results.FirstOrDefault();
                                        ListCurrentRequestDelete.Clear();
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

        private void ResetingOldListToCreateNewList(bool bCreateNewListFromOld = false, bool IsLoadForInstruction = false)
        {
            CurrentRequest.ReqFoodClinicDeptID = 0;
            CurrentRequest.ReqNumCode = "";
            CurrentRequest.Comment = "";
            if (!IsLoadForInstruction)
            {
                CurrentRequest.ReqDate = DateTime.Now;
                CurrentRequest.FromDate = DateTime.Now;
                CurrentRequest.ToDate = DateTime.Now;
            }
            CurrentRequest.SelectedStaff = GetStaffLogin();
            CurrentRequest.IsApproved = false;
            ListCurrentRequestDelete.Clear();
            if (CurrentRequest.RequestDetails != null && CurrentRequest.RequestDetails.Count > 0)
            {
                foreach (ReqFoodClinicDeptDetail item in CurrentRequest.RequestDetails)
                {
                    item.EntityState = EntityState.NEW;
                }
            }
        }

        private void RefeshRequest()
        {
            CurrentRequest.ReqFoodClinicDeptID = 0;
            CurrentRequest.ReqNumCode = "";
            CurrentRequest.Comment = "";
            CurrentRequest.ReqDate = DateTime.Now;
            CurrentRequest.FromDate = DateTime.Now;
            CurrentRequest.ToDate = DateTime.Now;
            CurrentRequest.SelectedStaff = GetStaffLogin();
            CurrentRequest.RequestDetails = new ObservableCollection<ReqFoodClinicDeptDetail>();
            CurrentRequest.IsApproved = false;
            StatusCoupon = "";
            IsStatusCoupon = false;
            ListCurrentRequestDelete.Clear();
            FillPagedCollectionAndGroup();
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
        }

        public void grdReqOutwardDetails_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            ReqFoodClinicDeptDetail rowItem = e.Row.DataContext as ReqFoodClinicDeptDetail;
            if (rowItem == null)
            {
                return;
            }
            //e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        public void grdReqOutwardDetails_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
        }

        public void btnSave(object sender, RoutedEventArgs e)
        {
            if (((ClinicDeptInPtReqForm_FoodView)GetView()).grdReqOutwardDetails != null)
            {
                SaveRequest();
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
            SearchRequestFoodClinicDept(0, 20);
        }

        public void btnNew(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.A0146_G1_Msg_ConfTaoMoiPh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                RefeshRequest();
                aucHoldConsultDoctor.setDefault();
                MedicalInstructionDateContent.DateTime = new DateTime(Globals.GetCurServerDateTime().Year, Globals.GetCurServerDateTime().Month, Globals.GetCurServerDateTime().Day, Globals.GetCurServerDateTime().Hour, Globals.GetCurServerDateTime().Minute, 0);
            }
        }

        private void LoadReqFoodClinicFromInstruction()
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginLoadReqFoodClinicFromInstruction(CurrentRequest.FromDate, CurrentRequest.ToDate, 0
                            , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndLoadReqFoodClinicFromInstruction(asyncResult);
                                if (results != null)
                                {
                                    CurrentRequest.RequestDetails = results.ToObservableCollection();
                                }
                                else
                                {
                                    if (CurrentRequest.RequestDetails != null)
                                    {
                                        CurrentRequest.RequestDetails.Clear();
                                    }
                                }
                                if (CurrentRequest == null)
                                {
                                    CurrentRequest = new RequestFoodClinicDept();
                                }
                                FillPagedCollectionAndGroup();
                                ResetingOldListToCreateNewList(true, true);
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

        private void GetInPatientRequestingDrugListByReqID(long RequestID, bool bCreateNewListFromOld = false)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetReqFoodClinicDeptDetailsByID(RequestID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetReqFoodClinicDeptDetailsByID(asyncResult);
                                if (results != null)
                                {
                                    CurrentRequest.RequestDetails = results.ToObservableCollection();
                                }
                                else
                                {
                                    if (CurrentRequest.RequestDetails != null)
                                    {
                                        CurrentRequest.RequestDetails.Clear();
                                    }
                                }
                                if (CurrentRequest == null)
                                {
                                    CurrentRequest = new RequestFoodClinicDept();
                                }
                                FillPagedCollectionAndGroup();
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

        public void btnLoadInstruction()
        {
            LoadReqFoodClinicFromInstruction();
        }

        #region printing member
        public void btnPreview()
        {
            void onInitDlg(IClinicDeptReportDocumentPreview proAlloc)
            {
                proAlloc.ID = CurrentRequest.ReqFoodClinicDeptID;
                proAlloc.eItem = ReportName.XRptClinicDeptRequestFood;
            }
            GlobalsNAV.ShowDialog<IClinicDeptReportDocumentPreview>(onInitDlg);
        }
        #endregion
        #endregion

        #region IHandle<DrugDeptCloseSearchRequestEvent> Members
        public void Handle(DrugDeptCloseSearchRequestEvent message)
        {
            if (message != null && IsActive)
            {
                RequestDrugInwardClinicDept temp = message.SelectedRequest as RequestDrugInwardClinicDept;
                RequestFoodClinicDept item = new RequestFoodClinicDept();
                item.ReqFoodClinicDeptID = temp.ReqDrugInClinicDeptID;
                item.ReqDate = temp.ReqDate;
                item.ReqStatus = (long)temp.ReqStatus;
                item.ReqNumCode = temp.ReqNumCode;
                item.Comment = temp.Comment;
                item.ToDate = temp.ToDate;
                item.DaNhanHang = temp.DaNhanHang;
                item.IsApproved = (bool)temp.IsApproved;
                item.SelectedStaff = temp.SelectedStaff;

                CurrentRequest = item;
                ListCurrentRequestDelete.Clear();

                GetInPatientRequestingDrugListByReqID(CurrentRequest.ReqFoodClinicDeptID);
            }
        }
        #endregion

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
            if (CurrentRequest != null && CurrentRequest.RequestDetails != null)
            {
                CVS_ReqDetails = new CollectionViewSource { Source = CurrentRequest.RequestDetails };
                CollectionView_ReqDetails = (CollectionView)CVS_ReqDetails.View;
                //FillGroupName();
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
            //if (((ClinicDeptInPtReqForm_V2View)GetView()).grdReqOutwardDetails != null)
            //{
            //    if (CurrentRequest.CanSave)
            //    {
            //        ((ClinicDeptInPtReqForm_V2View)GetView()).grdReqOutwardDetails.Columns[(int)DataGridCol.ColDelete].Visibility = Visibility.Visible;
            //        ((ClinicDeptInPtReqForm_V2View)GetView()).grdReqOutwardDetails.Columns[(int)DataGridCol.ColMultiDelete].Visibility = Visibility.Visible;
            //    }
            //    else
            //    {
            //        ((ClinicDeptInPtReqForm_V2View)GetView()).grdReqOutwardDetails.Columns[(int)DataGridCol.ColDelete].Visibility = Visibility.Collapsed;
            //        ((ClinicDeptInPtReqForm_V2View)GetView()).grdReqOutwardDetails.Columns[(int)DataGridCol.ColMultiDelete].Visibility = Visibility.Collapsed;
            //    }
            //}
        }

        //public void btnDeleteHang()
        //{
        //    if (CurrentRequest != null && CurrentRequest.ReqOutwardDetails != null && CurrentRequest.ReqOutwardDetails.Count > 0)
        //    {
        //        var items = CurrentRequest.ReqOutwardDetails.Where(x => x.Checked == true);
        //        if (items != null && items.Count() > 0)
        //        {
        //            if (MessageBox.Show("Bạn có chắc muốn xóa những hàng đã chọn không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
        //            {
        //                foreach (ReqOutwardDrugClinicDeptPatient obj in items)
        //                {
        //                    if (obj.EntityState == EntityState.PERSITED || obj.EntityState == EntityState.MODIFIED)
        //                    {
        //                        obj.EntityState = EntityState.DETACHED;
        //                        ListCurrentRequestDelete.Add(obj);
        //                    }
        //                }
        //                CurrentRequest.ReqOutwardDetails = CurrentRequest.ReqOutwardDetails.Where(x => x.Checked == false).ToObservableCollection();
        //                FillPagedCollectionAndGroup();

        //                SetCheckAllReqOutwardDetail();
        //            }
        //        }
        //        else
        //        {
        //            MessageBox.Show(eHCMSResources.A0090_G1_Msg_InfoChuaChonHgCanXoa);
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show(eHCMSResources.A0090_G1_Msg_InfoChuaChonHgCanXoa);
        //    }
        //}
        #endregion

        //private void FillGroupName()
        //{
        //    if (CollectionView_ReqDetails != null)
        //    {
        //        CollectionView_ReqDetails.GroupDescriptions.Clear();
        //        CollectionView_ReqDetails.SortDescriptions.Clear();
        //        CollectionView_ReqDetails.GroupDescriptions.Add(new PropertyGroupDescription("CurPatientRegistration.Patient.PatientCodeAndName"));
        //        CollectionView_ReqDetails.Filter = null;
        //    }
        //}

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

        private ReqFoodClinicDeptDetail _copySelectedReqOutwardDrugClinicDeptPatient;
        public ReqFoodClinicDeptDetail copySelectedReqOutwardDrugClinicDeptPatient
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

        private string _StatusCoupon = "";
        public string StatusCoupon
        {
            get { return _StatusCoupon; }
            set
            {
                if (_StatusCoupon != value)
                {
                    _StatusCoupon = value;
                }
                NotifyOfPropertyChange(() => StatusCoupon);
            }
        }

        private bool _IsStatusCoupon = false;
        public bool IsStatusCoupon
        {
            get { return _IsStatusCoupon; }
            set
            {
                if (_IsStatusCoupon != value)
                {
                    _IsStatusCoupon = value;
                }
                NotifyOfPropertyChange(() => IsStatusCoupon);
            }
        }

        private void ControlStatusCoupon()
        {
            StatusCoupon = "";
            if (CurrentRequest.RequestDetails.Count > 0 && (CurrentRequest.IsApproved == null || CurrentRequest.IsApproved == false))
            {
                StatusCoupon = eHCMSResources.Z2425_G1_ChuaDuyetPhieuLinh;
                IsStatusCoupon = true;
            }
            else if (CurrentRequest.RequestDetails.Count > 0 && (CurrentRequest.IsApproved == true && CurrentRequest.DaNhanHang == false))
            {
                StatusCoupon = eHCMSResources.Z2423_G1_DaDuyetLinh;
                IsStatusCoupon = true;
            }
            else if (CurrentRequest.RequestDetails.Count > 0 && (CurrentRequest.IsApproved == true && CurrentRequest.DaNhanHang == true))
            {
                StatusCoupon = eHCMSResources.Z2424_G1_DaDuyetVuiLongNhap;
                IsStatusCoupon = true;
            }
        }

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
    }
}
