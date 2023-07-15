using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.BaseModel;
using System.Threading;
using aEMR.Common;
using eHCMSLanguage;
using System.Collections.Generic;
using aEMR.ServiceClient;
using aEMR.Common.Collections;
using System.ServiceModel;
using aEMR.DataContracts;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using aEMR.CommonTasks;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;
/*
* 20230619: QTD:  Gửi SMS sau khi ký số thành công
*/

namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof(IDigitalSignTransaction)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DigitalSignTransactionViewModel : ViewModelBase, IDigitalSignTransaction
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public DigitalSignTransactionViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            SetDefaultForSearch();
            SetDefaultForSearchHistory();
            GetTransactionLookup();
            ListSendTransaction = new PagedSortableCollectionView<PatientPCLRequest> { PageSize = 20 };
            ListSendTransaction.OnRefresh += new EventHandler<RefreshEventArgs>(ListSendTransaction_OnRefresh);

            ListHistoryTransaction = new PagedSortableCollectionView<PatientPCLRequest> { PageSize = 20 };
            ListHistoryTransaction.OnRefresh += new EventHandler<RefreshEventArgs>(ListHistoryTransaction_OnRefresh);

            ApproveUserName = string.IsNullOrEmpty(Globals.ApproveUserName) ? string.Empty : Globals.ApproveUserName;
            authorization();
        }
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            mXoaKetQua = Globals.CheckOperation(Globals.listRefModule, (int)eModules.mCLSLaboratory
                , (int)eCLSLaboratory.mGiaoDich_KySo, (int)oDigitalTransactionEx.mXetNghiem_XoaLichSuGiaoDich);
        }
        private bool _mXoaKetQua = true;
        public bool mXoaKetQua
        {
            get
            {
                return _mXoaKetQua;
            }
            set
            {
                if (_mXoaKetQua == value)
                    return;
                _mXoaKetQua = value;
                NotifyOfPropertyChange(() => mXoaKetQua);
            }
        }
        void ListSendTransaction_OnRefresh(object sender, RefreshEventArgs e)
        {
            GlobalsNAV.ShowMessagePopup("Bỏ các phiếu đã chọn");
            ListSendTransaction_Paging(ListSendTransaction.PageIndex, ListSendTransaction.PageSize, false, true);
        }

        void ListHistoryTransaction_OnRefresh(object sender, RefreshEventArgs e)
        {
            //GlobalsNAV.ShowMessagePopup("Bỏ các phiếu đã chọn");
            ListHistoryTransaction_Paging(ListHistoryTransaction.PageIndex, ListHistoryTransaction.PageSize, false, true);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }

        #region properties
        private PagedSortableCollectionView<PatientPCLRequest> _ListSendTransaction;
        public PagedSortableCollectionView<PatientPCLRequest> ListSendTransaction
        {
            get { return _ListSendTransaction; }
            set
            {
                if (_ListSendTransaction == value)
                    return;
                _ListSendTransaction = value;
                NotifyOfPropertyChange(() => ListSendTransaction);
            }
        }

        private PagedSortableCollectionView<PatientPCLRequest> _ListHistoryTransaction;
        public PagedSortableCollectionView<PatientPCLRequest> ListHistoryTransaction
        {
            get { return _ListHistoryTransaction; }
            set
            {
                if (_ListHistoryTransaction == value)
                    return;
                _ListHistoryTransaction = value;
                NotifyOfPropertyChange(() => ListHistoryTransaction);
            }
        }

        private PatientPCLRequestSearchCriteria _SearchCriteriaSendTransaction;
        public PatientPCLRequestSearchCriteria SearchCriteriaSendTransaction
        {
            get { return _SearchCriteriaSendTransaction; }
            set
            {
                if (_SearchCriteriaSendTransaction == value)
                    return;
                _SearchCriteriaSendTransaction = value;
                NotifyOfPropertyChange(() => SearchCriteriaSendTransaction);
            }
        }

        private PatientPCLRequestSearchCriteria _SearchCriteriaHistoryTransaction;
        public PatientPCLRequestSearchCriteria SearchCriteriaHistoryTransaction
        {
            get { return _SearchCriteriaHistoryTransaction; }
            set
            {
                if (_SearchCriteriaHistoryTransaction == value)
                    return;
                _SearchCriteriaHistoryTransaction = value;
                NotifyOfPropertyChange(() => SearchCriteriaHistoryTransaction);
            }
        }

        private bool _IsOutPatientCheck;
        public bool IsOutPatientCheck
        {
            get { return _IsOutPatientCheck; }
            set
            {
                if (_IsOutPatientCheck == value)
                    return;
                _IsOutPatientCheck = value;
                NotifyOfPropertyChange(() => IsOutPatientCheck);
            }
        }

        private bool _IsHistoryOutPatientCheck;
        public bool IsHistoryOutPatientCheck
        {
            get { return _IsHistoryOutPatientCheck; }
            set
            {
                if (_IsHistoryOutPatientCheck == value)
                    return;
                _IsHistoryOutPatientCheck = value;
                NotifyOfPropertyChange(() => IsHistoryOutPatientCheck);
            }
        }

        private string _ApproveUserName;
        public string ApproveUserName
        {
            get { return _ApproveUserName; }
            set
            {
                if (_ApproveUserName == value)
                    return;
                _ApproveUserName = value;
                NotifyOfPropertyChange(() => ApproveUserName);
            }
        }

        private ObservableCollection<Lookup> _TransactionStatusList;
        public ObservableCollection<Lookup> TransactionStatusList
        {
            get
            {
                return _TransactionStatusList;
            }
            set
            {
                if (_TransactionStatusList != value)
                {
                    _TransactionStatusList = value;
                    NotifyOfPropertyChange(() => TransactionStatusList);
                }
            }
        }
        #endregion
        #region Function
        private void SetDefaultForSearch()
        {
            SearchCriteriaSendTransaction = new PatientPCLRequestSearchCriteria();
            SearchCriteriaSendTransaction.FromDate = Globals.GetCurServerDateTime();
            SearchCriteriaSendTransaction.ToDate = Globals.GetCurServerDateTime();
            SearchCriteriaSendTransaction.PatientCode = string.Empty;
            SearchCriteriaSendTransaction.FullName = string.Empty;
            IsOutPatientCheck = true;
        }

        private void SetDefaultForSearchHistory()
        {
            SearchCriteriaHistoryTransaction = new PatientPCLRequestSearchCriteria();
            SearchCriteriaHistoryTransaction.FromDate = Globals.GetCurServerDateTime();
            SearchCriteriaHistoryTransaction.ToDate = Globals.GetCurServerDateTime();
            SearchCriteriaHistoryTransaction.PatientCode = string.Empty;
            SearchCriteriaHistoryTransaction.FullName = string.Empty;
            IsHistoryOutPatientCheck = true;
        }

        private void GetTransactionLookup()
        {
            TransactionStatusList = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_TransactionStatus
                                                                         && x.LookupID != (long)AllLookupValues.V_TransactionStatus.Khac).ToObservableCollection();
            TransactionStatusList.Insert(0, new Lookup { LookupID = 0, ObjectValue = "Tất cả" });
        }

        private void ListSendTransaction_Paging(int PageIndex, int PageSize, bool CountTotal, bool IsRefresh = false)
        {
            this.ShowBusyIndicator(eHCMSResources.Z1125_G1_DangTimKiem);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetPatientPCLLaboratoryResultForSendTransaction(SearchCriteriaSendTransaction, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<PatientPCLRequest> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndGetPatientPCLLaboratoryResultForSendTransaction(out Total, asyncResult);
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
                            ListSendTransaction.Clear();
                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ListSendTransaction.TotalItemCount = Total;
                                }
                                if (allItems != null && allItems.Count > 0)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ListSendTransaction.Add(item);
                                    }
                                }
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

        private void ListHistoryTransaction_Paging(int PageIndex, int PageSize, bool CountTotal, bool IsRefresh = false)
        {
            this.ShowBusyIndicator(eHCMSResources.Z1125_G1_DangTimKiem);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetListHistoryTransaction_Paging(SearchCriteriaHistoryTransaction, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<PatientPCLRequest> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndGetListHistoryTransaction_Paging(out Total, asyncResult);
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
                            ListHistoryTransaction.Clear();
                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ListHistoryTransaction.TotalItemCount = Total;
                                }
                                if (allItems != null && allItems.Count > 0)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ListHistoryTransaction.Add(item);
                                    }
                                }
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
        private void DeletePCLRequestFromList(PatientPCLRequest request)
        {
            this.ShowBusyIndicator(eHCMSResources.Z1125_G1_DangTimKiem);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginDeletePCLRequestFromList(request, (int)SearchCriteriaSendTransaction.PatientFindBy, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool bOK = client.EndDeletePCLRequestFromList(asyncResult);
                                if (bOK)
                                {
                                    ListSendTransaction_Paging(ListSendTransaction.PageIndex, ListSendTransaction.PageSize, false, true);
                                }
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
        private void DeleteTransactionHistory(PatientPCLRequest request)
        {
            this.ShowBusyIndicator(eHCMSResources.Z1125_G1_DangTimKiem);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginDeleteTransactionHistory(request, (int)SearchCriteriaSendTransaction.PatientFindBy, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool bOK = client.EndDeleteTransactionHistory(asyncResult);
                                if (bOK)
                                {
                                    ListHistoryTransaction_Paging(ListHistoryTransaction.PageIndex, ListHistoryTransaction.PageSize, false, true);
                                }
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
        #endregion
        #region Event
        public void rdtNgoaiTru_Checked(object sender, RoutedEventArgs e)
        {
            SearchCriteriaSendTransaction.PatientFindBy = AllLookupValues.PatientFindBy.NGOAITRU;
        }

        public void rdtNoiTru_Checked(object sender, RoutedEventArgs e)
        {
            SearchCriteriaSendTransaction.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;
        }

        public void rdtKSK_Checked(object sender, RoutedEventArgs e)
        {
            SearchCriteriaSendTransaction.PatientFindBy = AllLookupValues.PatientFindBy.KSK;
        }

        public void rdtHistoryNgoaiTru_Checked(object sender, RoutedEventArgs e)
        {
            SearchCriteriaHistoryTransaction.PatientFindBy = AllLookupValues.PatientFindBy.NGOAITRU;
        }

        public void rdtHistoryNoiTru_Checked(object sender, RoutedEventArgs e)
        {
            SearchCriteriaHistoryTransaction.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;
        }

        public void rdtHistoryKSK_Checked(object sender, RoutedEventArgs e)
        {
            SearchCriteriaHistoryTransaction.PatientFindBy = AllLookupValues.PatientFindBy.KSK;
        }

        public void btSearch()
        {
            if (SearchCriteriaSendTransaction.FromDate == null
                || SearchCriteriaSendTransaction.ToDate == null)
            {
                return;
            }
            ListSendTransaction_Paging(0, ListSendTransaction.PageSize, true);
        }

        public void btSearchHistory()
        {
            if (SearchCriteriaHistoryTransaction.FromDate == null
                || SearchCriteriaHistoryTransaction.ToDate == null)
            {
                return;
            }
            ListHistoryTransaction_Paging(0, ListHistoryTransaction.PageSize, true);
        }
        public void hplDelete_Click(object selectedItem)
        {
            PatientPCLRequest request = selectedItem as PatientPCLRequest;
            if (MessageBox.Show("Bạn có muốn xóa phiếu để cập nhật lại kết quả", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                DeletePCLRequestFromList(request);
            }
        }
        public IEnumerator<IResult> DeleteHistoryTransaction(PatientPCLRequest request)
        {
            var dialog = new MessageWarningShowDialogTask("Bạn có chắc chắn muốn xóa phiếu này không?", "Đồng ý");
            yield return dialog;
            if (dialog.IsAccept)
            {
                DeleteTransactionHistory(request);
            }
            yield break;
        }
        public void hplDeleteHistory_Click(object selectedItem)
        {
            PatientPCLRequest request = selectedItem as PatientPCLRequest;
            Coroutine.BeginExecute(DeleteHistoryTransaction(request));
        }
        public void hplCheck_Click(object selectedItem)
        {
            if (ListSendTransaction.Where(x => x.IsChecked).Count() > Globals.ServerConfigSection.CommonItems.CountSendTransaction)
            {
                (selectedItem as PatientPCLRequest).IsChecked = false;
                MessageBox.Show("Số lượng phiếu vượt quá trong 1 giao dịch được cấu hình \"" + Globals.ServerConfigSection.CommonItems.CountSendTransaction + "\"");
            }
        }
        public void hplPrint_Click(object selectedItem)
        {
            PatientPCLRequest currentItem = (selectedItem as PatientPCLRequest);
            if (currentItem.PatientPCLReqID <= 0)
            {
                MessageBox.Show(eHCMSResources.Z2375_G1_ChonYCXN);
                return;
            }

            if (currentItem.PCLResultParamImpID == null)
            {
                ICommonPreviewView mPopupDialog = Globals.GetViewModel<ICommonPreviewView>();
                mPopupDialog.PatientID = currentItem.PatientID;
                mPopupDialog.PatientPCLReqID = (int)currentItem.PatientPCLReqID;
                mPopupDialog.V_PCLRequestType = (long)currentItem.V_PCLRequestType;
                mPopupDialog.FindPatient = (long)currentItem.V_PCLRequestType > 0 && (long)currentItem.V_PCLRequestType == (long)AllLookupValues.V_PCLRequestType.NOI_TRU ? (int)AllLookupValues.PatientFindBy.NOITRU : (int)AllLookupValues.PatientFindBy.NGOAITRU;
                mPopupDialog.StaffName = Globals.LoggedUserAccount.Staff.FullName;
                mPopupDialog.eItem = ReportName.PCLDEPARTMENT_LABORATORY_RESULT;
                GlobalsNAV.ShowDialog_V3(mPopupDialog, null, null, false, true, new System.Windows.Size(1000, 700));
            }
            else
            {
                IPatientPCLImagingResult_V2 mPopupDialog = Globals.GetViewModel<IPatientPCLImagingResult_V2>();
                mPopupDialog.InitHTML();
                mPopupDialog.CheckTemplatePCLResultByReqID(currentItem.PatientPCLReqID, (long)currentItem.V_PCLRequestType == (long)AllLookupValues.V_PCLRequestType.NOI_TRU);
                GlobalsNAV.ShowDialog_V3(mPopupDialog, null, null, false, true, new System.Windows.Size(1000, 700));
            }
        }

        public void ThePasswordBox_Loaded(object sender, RoutedEventArgs e)
        {
            PwdBoxCtrl = sender as PasswordBox;
        }
        public PasswordBox PwdBoxCtrl { get; set; }
        StaffPosition parHeadOfLaboratoryFullName;

        public void btSendTransaction()
        {
            if (ListSendTransaction == null || ListSendTransaction.Count == 0)
            {
                return;
            }
            if (ListSendTransaction.Where(x => x.IsChecked).Count() == 0)
            {
                MessageBox.Show("Chưa chọn phiếu nào");
                return;
            }
            int CountSendTransaction = Globals.ServerConfigSection.CommonItems.CountSendTransaction;
            if (ListSendTransaction.Where(x => x.IsChecked).Count() > Globals.ServerConfigSection.CommonItems.CountSendTransaction)
            {
                MessageBox.Show("Chỉ cho phép ký số " + CountSendTransaction + " phiếu trên lần. Vui lòng kiểm tra lại!");
                return;
            }

            parHeadOfLaboratoryFullName = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.TRUONG_KHOA_XET_NGHIEM && x.IsActive).FirstOrDefault();

            if (string.IsNullOrEmpty(ApproveUserName) || string.IsNullOrEmpty(ApproveUserName))
            {
                MessageBox.Show("Thông tin người phê duyệt hoặc mật khẩu không được để trống. Vui lòng kiểm tra lại!");
                return;
            }
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);

            Globals.ApproveUserName = string.IsNullOrEmpty(ApproveUserName) ? string.Empty : ApproveUserName;
            string AccountPassword = EncryptExtension.Encrypt(ApproveUserName, Globals.AxonKey, Globals.AxonPass);

            // step 1: lấy kết quả pdf từ danh sách gửi qua.
            // build string dsách phiếu lấy kết quả gửi qua Report Services Do bên đó k nhận DataEntity 
            StringBuilder sb = new StringBuilder();
            sb.Append("<Root>");
            foreach (PatientPCLRequest item in ListSendTransaction.Where(x => x.IsChecked).ToObservableCollection())
            {
                sb.Append("<Item>");
                sb.AppendFormat("<patientPCLReqID>{0}</patientPCLReqID>", item.PatientPCLReqID);
                sb.AppendFormat("<PatientID>{0}</PatientID>", item.PatientID);
                sb.AppendFormat("<V_PCLRequestType>{0}</V_PCLRequestType>", (long)item.V_PCLRequestType);
                sb.AppendFormat("<V_PCLRequestStatus>{0}</V_PCLRequestStatus>", (long)item.V_PCLRequestStatus);
                sb.AppendFormat("<V_PCLMainCategory>{0}</V_PCLMainCategory>", item.V_PCLMainCategory);
                sb.AppendFormat("<FindPatient>{0}</FindPatient>", item.V_PCLRequestType > 0 && item.V_PCLRequestType == AllLookupValues.V_PCLRequestType.NOI_TRU ? (int)AllLookupValues.PatientFindBy.NOITRU : (int)AllLookupValues.PatientFindBy.NGOAITRU);
                sb.AppendFormat("<folderName>{0}</folderName>", item.MedicalInstructionDate.Value.ToString("yyyyMMdd"));
                sb.AppendFormat("<resultID>{0}</resultID>", item.ResultID);
                sb.Append("</Item>");
            }
            sb.Append("</Root>");

            Coroutine.BeginExecute(UpdatePCLRequestStatus_Routine(sb));
            // step 2: gửi danh sách file PDF qua cho service gửi SmartCA ký 
            // Step 3: sau khi ký thì service đánh dấu đã ký
            this.HideBusyIndicator();
        }

        private IEnumerator<IResult> UpdatePCLRequestStatus_Routine(object ListPCLResults)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);

            var mUpdatePCLRequestStatusTask = new GenericCoRoutineTask(UpdatePCLRequestStatusTask, ListPCLResults, (long)AllLookupValues.V_PCLRequestStatus.SIGNING);
            yield return mUpdatePCLRequestStatusTask;

            var mGetPDFResultToSignTask = new GenericCoRoutineTask(GetPDFResultToSignTask, ListPCLResults);
            yield return mGetPDFResultToSignTask;

            // nếu lấy dữ liệu thất bại thì cập nhật lại trạng thái của các phiếu trên về bình thường. Khi gửi kết quả thì phiếu phải là hoàn tất
            bool IsGetPDFResultSuccess = (bool)mGetPDFResultToSignTask.GetResultObj(0);
            if (!IsGetPDFResultSuccess)
            {
                var mReUpdatePCLRequestStatusTask = new GenericCoRoutineTask(UpdatePCLRequestStatusTask, ListPCLResults, (long)AllLookupValues.V_PCLRequestStatus.CLOSE);
                yield return mReUpdatePCLRequestStatusTask;
            }
            else
            {
                var mReUpdatePCLRequestStatusTask = new GenericCoRoutineTask(UpdatePCLRequestStatusTask, ListPCLResults, (long)AllLookupValues.V_PCLRequestStatus.SIGNED);
                yield return mReUpdatePCLRequestStatusTask;
            }

            this.HideBusyIndicator();
        }

        // đánh dấu trạng thái danh sách đang lấy kết quả + gửi giao dịch. tránh 2 màn hình
        private void UpdatePCLRequestStatusTask(GenericCoRoutineTask genTask, object ListPCLResults, object V_PCLRequestStatus)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdatePCLRequestStatusGeneratingResult(ListPCLResults.ToString(), (long)V_PCLRequestStatus
                            , Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    var data = contract.EndUpdatePCLRequestStatusGeneratingResult(asyncResult);
                                    this.HideBusyIndicator();
                                    if (!data)
                                    {
                                        MessageBox.Show("Không tìm thấy phiếu chỉ định. Vui lòng kiểm tra lại hoặc liên hệ IT!");
                                        genTask.ActionComplete(false);
                                    }
                                    else
                                    {
                                        if(Globals.ServerConfigSection.CommonItems.IsEnableSendSMSLab && (long)V_PCLRequestStatus == (long)AllLookupValues.V_PCLRequestStatus.SIGNED)
                                        {
                                            SendSMS(ListPCLResults);
                                        }
                                        genTask.ActionComplete(true);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                    this.HideBusyIndicator();
                                    genTask.ActionComplete(false);
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    MessageBox.Show(ex.Message);
                    this.HideBusyIndicator();
                    genTask.ActionComplete(false);
                }
            });

            t.Start();
        }

        private HIAPIServiceResponse _HIAPIServiceResponse;
        public HIAPIServiceResponse HIAPIServiceResponse
        {
            get => _HIAPIServiceResponse; set
            {
                _HIAPIServiceResponse = value;
                NotifyOfPropertyChange(() => HIAPIServiceResponse);
            }
        }

        private void GetPDFResultToSignTask(GenericCoRoutineTask genTask, object ListSendTransactionXml)
        {
            try
            {
                using (var serviceFactory = new ReportServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetPDFResulstToSign(ListSendTransactionXml.ToString(), Globals.ServerConfigSection.ConsultationElements.LaboratoryResultVersion
                        , Globals.LoggedUserAccount.Staff.FullName
                        , Globals.ServerConfigSection.Hospitals.PrescriptionMainRightHeader, Globals.ServerConfigSection.Hospitals.PrescriptionSubRightHeader
                        , parHeadOfLaboratoryFullName != null ? parHeadOfLaboratoryFullName.FullNameString : ""
                        , parHeadOfLaboratoryFullName != null ? parHeadOfLaboratoryFullName.PositionName : ""
                        , Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth
                        , Globals.ServerConfigSection.CommonItems.ReportHospitalAddress
                        , Globals.ServerConfigSection.CommonItems.ReportHospitalName
                        , Globals.ServerConfigSection.Hospitals.HospitalCode
                        , Globals.ServerConfigSection.CommonItems.ServicePool, Globals.ServerConfigSection.CommonItems.PDFFileResultToSignPath
                        , Globals.ServerConfigSection.CommonItems.PDFFileResultSignedPath
                        , Globals.ServerConfigSection.Hospitals.FTPAdminUserName
                        , Globals.ServerConfigSection.Hospitals.FTPAdminPassword
                        , Globals.ServerConfigSection.CommonItems.FTPServerSighHashUrl
                        , Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var data = contract.EndGetPDFResulstToSign(asyncResult);
                                if (!string.IsNullOrEmpty(data))
                                {
                                    XmlTextReader txtReader = new XmlTextReader(new StringReader(data));
                                    XmlSerializer serializer = new XmlSerializer(typeof(Root));
                                    Root RootPatientPCLRequest = (Root)serializer.Deserialize(txtReader);
                                    string Password = EncryptExtension.Encrypt(PwdBoxCtrl.Password, Globals.AxonKey, Globals.AxonPass);
                                    //string Username = EncryptExtension.Encrypt(ApproveUserName, Globals.AxonKey, Globals.AxonPass);
                                    string mJsonData = string.Format("{{\"username\":\"{0}\",\"password\":\"{1}\",\"listPDFResults\":{2},\"ftpusername\":\"{3}\",\"ftppassword\":\"{4}\",\"ftpurl\":\"{5}\"}}"
                                    , ApproveUserName, Password, GlobalsNAV.ConvertObjectToJson(RootPatientPCLRequest.Item)
                                    , "", ""
                                    , Globals.ServerConfigSection.CommonItems.ServiceUrl);
                                    //string mRestJson = GlobalsNAV.GetRESTServiceJSon(Globals.ServerConfigSection.CommonItems.HISSighHashSmartCAUrl, mJsonData);

                                    TimeSpan timeout = new TimeSpan(0, 6, 30);
                                    var resultResponse = GlobalsNAV.RequestPOST(Globals.ServerConfigSection.CommonItems.HISSighHashSmartCAUrl, "", mJsonData, timeout);
                                    if (resultResponse == null)
                                    {
                                        genTask.AddResultObj(false);
                                        MessageBox.Show("BadGateway");
                                    }

                                    if (resultResponse != null && resultResponse != HttpStatusCode.NotFound.ToString())
                                    {
                                        if (HIAPIServiceResponse != null)
                                        {
                                            HIAPIServiceResponse = null;
                                        }
                                        HIAPIServiceResponse = GlobalsNAV.ConvertJsonToObject<HIAPIServiceResponse>(resultResponse);
                                        if (HIAPIServiceResponse.success)
                                        {
                                            genTask.AddResultObj(true);
                                            GlobalsNAV.ShowMessagePopup(HIAPIServiceResponse.message);
                                            ListSendTransaction_Paging(0, ListSendTransaction.PageSize, true);
                                        }
                                        else
                                        {
                                            genTask.AddResultObj(false);
                                            MessageBox.Show(HIAPIServiceResponse.message);
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Lỗi khi xuất kết quả. Vui lòng kiểm tra kết quả của từng phiếu hoặc liên hệ IT!", eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    genTask.AddResultObj(false);
                                }
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                genTask.AddResultObj(false);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                                genTask.ActionComplete(true);
                            }
                        }), null);
                }
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogInfo(ex.ToString());
                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                this.HideBusyIndicator();
                genTask.AddResultObj(false);
                genTask.ActionComplete(true);
            }
        }

        [XmlRoot(ElementName = "Item")]
        public class Item
        {
            [XmlElement(ElementName = "patientPCLReqID")]
            public long patientPCLReqID { get; set; }
            [XmlElement(ElementName = "fullPathPDFFileToSign")]
            public string fullPathPDFFileToSign { get; set; }
            [XmlElement(ElementName = "fullPathPDFFileSigned")]
            public string fullPathPDFFileSigned { get; set; }
            [XmlElement(ElementName = "folderName")]
            public string folderName { get; set; }
            [XmlElement(ElementName = "fileNameSigned")]
            public string fileNameSigned { get; set; }
            [XmlElement(ElementName = "pageIndex")]
            public string pageIndex { get; set; }
            [XmlElement(ElementName = "fileName")]
            public string fileName { get; set; }
            [XmlElement(ElementName = "findPatient")]
            public string findPatient { get; set; }
            [XmlElement(ElementName = "resultID")]
            public long resultID { get; set; }
        }

        [Serializable()]
        [XmlRootAttribute("Root", Namespace = "", IsNullable = false)]
        public class Root
        {
            [XmlElement(ElementName = "Item")]
            public List<Item> Item { get; set; }
        }
        #endregion

        //▼====: #001
        private void SendSMS(object ListPCLResults)
        {
            XDocument xmlDoc = XDocument.Parse(ListPCLResults.ToString());
            JObject jsonObject = new JObject(); 
            var items = xmlDoc.Descendants("Item").Select(item =>
            {
                Dictionary<string, string> itemData = item.Elements()
                    .ToDictionary(element => element.Name.LocalName, element => element.Value);
                return itemData;
            });
            jsonObject["Item"] = JToken.FromObject(items);
            string jsonString = JsonConvert.SerializeObject(jsonObject);
            if (!string.IsNullOrEmpty(jsonString))
            {
                bool send = GlobalsNAV.SaveDataForSmsLab(0, 0, jsonString);
                if (send)
                {
                    MessageBox.Show("Đã gửi SMS thông báo cho tất cả bệnh nhân thành công!");
                }
                else
                {
                    MessageBox.Show("Chưa gửi được SMS thông báo cho tất cả bệnh nhân, xem báo cáo để biết chi tiết lỗi!");
                }
            }
        }
        //▲====: #001
    }
}
