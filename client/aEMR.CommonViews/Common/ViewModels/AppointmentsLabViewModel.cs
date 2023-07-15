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
using System.Xml.Linq;
/*
* 20230619: QTD:  Gửi SMS sau khi ký số thành công
*/

namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof(IAppointmentsLab)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class AppointmentsLabViewModel : ViewModelBase, IAppointmentsLab
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public AppointmentsLabViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            SetDefaultForSearchHistory();
            GetStatusSMSLookup();

            ListAppointmentsLab = new PagedSortableCollectionView<PatientPCLRequest> { PageSize = 20 };
            ListAppointmentsLab.OnRefresh += new EventHandler<RefreshEventArgs>(ListAppointmentsLab_OnRefresh);

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
        //void ListSendTransaction_OnRefresh(object sender, RefreshEventArgs e)
        //{
        //    GlobalsNAV.ShowMessagePopup("Bỏ các phiếu đã chọn");
        //    ListSendTransaction_Paging(ListSendTransaction.PageIndex, ListSendTransaction.PageSize, false, true);
        //}

        void ListAppointmentsLab_OnRefresh(object sender, RefreshEventArgs e)
        {
            ListAppointmentsLab_Paging(ListAppointmentsLab.PageIndex, ListAppointmentsLab.PageSize, false);
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

        private PagedSortableCollectionView<PatientPCLRequest> _ListAppointmentsLab;
        public PagedSortableCollectionView<PatientPCLRequest> ListAppointmentsLab
        {
            get { return _ListAppointmentsLab; }
            set
            {
                if (_ListAppointmentsLab == value)
                    return;
                _ListAppointmentsLab = value;
                NotifyOfPropertyChange(() => ListAppointmentsLab);
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

        private bool _IsXNChisoCheck;
        public bool IsXNChisoCheck
        {
            get { return _IsXNChisoCheck; }
            set
            {
                if (_IsXNChisoCheck == value)
                    return;
                _IsXNChisoCheck = value;
                NotifyOfPropertyChange(() => IsXNChisoCheck);
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

        private void SetDefaultForSearchHistory()
        {
            SearchCriteriaHistoryTransaction = new PatientPCLRequestSearchCriteria();
            SearchCriteriaHistoryTransaction.FromDate = Globals.GetCurServerDateTime();
            SearchCriteriaHistoryTransaction.ToDate = Globals.GetCurServerDateTime();
            SearchCriteriaHistoryTransaction.PatientCode = string.Empty;
            SearchCriteriaHistoryTransaction.FullName = string.Empty;
            IsXNChisoCheck = true;
        }

        private void GetStatusSMSLookup()
        {
            TransactionStatusList = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_SendSMSStatus).ToObservableCollection();
            TransactionStatusList.Insert(0, new Lookup { LookupID = 0, ObjectValue = "Tất cả" });
        }

        //private void ListSendTransaction_Paging(int PageIndex, int PageSize, bool CountTotal, bool IsRefresh = false)
        //{
        //    this.ShowBusyIndicator(eHCMSResources.Z1125_G1_DangTimKiem);
        //    var t = new Thread(() =>
        //    {
        //        try
        //        {
        //            using (var serviceFactory = new PCLsClient())
        //            {
        //                var client = serviceFactory.ServiceInstance;
        //                client.BeginGetPatientPCLLaboratoryResultForSendTransaction(SearchCriteriaSendTransaction, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
        //                {
        //                    int Total = 0;
        //                    IList<PatientPCLRequest> allItems = null;
        //                    bool bOK = false;
        //                    try
        //                    {
        //                        allItems = client.EndGetPatientPCLLaboratoryResultForSendTransaction(out Total, asyncResult);
        //                        bOK = true;
        //                    }
        //                    catch (FaultException<AxException> fault)
        //                    {
        //                        ClientLoggerHelper.LogInfo(fault.ToString());
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        ClientLoggerHelper.LogInfo(ex.ToString());
        //                    }
        //                    finally
        //                    {
        //                        this.HideBusyIndicator();
        //                    }
        //                    ListSendTransaction.Clear();
        //                    if (bOK)
        //                    {
        //                        if (CountTotal)
        //                        {
        //                            ListSendTransaction.TotalItemCount = Total;
        //                        }
        //                        if (allItems != null && allItems.Count > 0)
        //                        {
        //                            foreach (var item in allItems)
        //                            {
        //                                ListSendTransaction.Add(item);
        //                            }
        //                        }
        //                    }
        //                }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //            this.HideBusyIndicator();
        //        }
        //    });
        //    t.Start();
        //}

        private void ListAppointmentsLab_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            this.ShowBusyIndicator(eHCMSResources.Z1125_G1_DangTimKiem);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetListAppointmentsLab_Paging(SearchCriteriaHistoryTransaction, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<PatientPCLRequest> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndGetListAppointmentsLab_Paging(out Total, asyncResult);
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
                            ListAppointmentsLab.Clear();
                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ListAppointmentsLab.TotalItemCount = Total;
                                }
                                if (allItems != null && allItems.Count > 0)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ListAppointmentsLab.Add(item);
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
        //private void DeletePCLRequestFromList(PatientPCLRequest request)
        //{
        //    this.ShowBusyIndicator(eHCMSResources.Z1125_G1_DangTimKiem);
        //    var t = new Thread(() =>
        //    {
        //        try
        //        {
        //            using (var serviceFactory = new PCLsClient())
        //            {
        //                var client = serviceFactory.ServiceInstance;
        //                client.BeginDeletePCLRequestFromList(request, (int)SearchCriteriaSendTransaction.PatientFindBy, Globals.DispatchCallback((asyncResult) =>
        //                {
        //                    try
        //                    {
        //                        bool bOK = client.EndDeletePCLRequestFromList(asyncResult);
        //                        if (bOK)
        //                        {
        //                            ListSendTransaction_Paging(ListSendTransaction.PageIndex, ListSendTransaction.PageSize, false, true);
        //                        }
        //                    }
        //                    catch (FaultException<AxException> fault)
        //                    {
        //                        ClientLoggerHelper.LogInfo(fault.ToString());
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        ClientLoggerHelper.LogInfo(ex.ToString());
        //                    }
        //                    finally
        //                    {
        //                        this.HideBusyIndicator();
        //                    }
        //                }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //            this.HideBusyIndicator();
        //        }
        //    });
        //    t.Start();
        //}

        //private void DeleteTransactionHistory(PatientPCLRequest request)
        //{
        //    this.ShowBusyIndicator(eHCMSResources.Z1125_G1_DangTimKiem);
        //    var t = new Thread(() =>
        //    {
        //        try
        //        {
        //            using (var serviceFactory = new PCLsClient())
        //            {
        //                var client = serviceFactory.ServiceInstance;
        //                client.BeginDeleteTransactionHistory(request, (int)SearchCriteriaSendTransaction.PatientFindBy, Globals.DispatchCallback((asyncResult) =>
        //                {
        //                    try
        //                    {
        //                        bool bOK = client.EndDeleteTransactionHistory(asyncResult);
        //                        if (bOK)
        //                        {
        //                            ListAppointmentsLab_Paging(ListAppointmentsLab.PageIndex, ListAppointmentsLab.PageSize, false, true);
        //                        }
        //                    }
        //                    catch (FaultException<AxException> fault)
        //                    {
        //                        ClientLoggerHelper.LogInfo(fault.ToString());
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        ClientLoggerHelper.LogInfo(ex.ToString());
        //                    }
        //                    finally
        //                    {
        //                        this.HideBusyIndicator();
        //                    }
        //                }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //            this.HideBusyIndicator();
        //        }
        //    });
        //    t.Start();
        //}
        #endregion
        #region Event
        public void rdtChiso_Checked(object sender, RoutedEventArgs e)
        {
            SearchCriteriaHistoryTransaction.V_LabType = 0;
        }

        public void rdtCDHA_Checked(object sender, RoutedEventArgs e)
        {
            SearchCriteriaHistoryTransaction.V_LabType = 1;
        }

        //public void btSearch()
        //{
        //    if (SearchCriteriaSendTransaction.FromDate == null
        //        || SearchCriteriaSendTransaction.ToDate == null)
        //    {
        //        return;
        //    }
        //    ListSendTransaction_Paging(0, ListSendTransaction.PageSize, true);
        //}

        public void btSearchHistory()
        {
            if (SearchCriteriaHistoryTransaction.FromDate == null
                || SearchCriteriaHistoryTransaction.ToDate == null)
            {
                return;
            }
            ListAppointmentsLab_Paging(0, ListAppointmentsLab.PageSize, true);
        }
        //public void hplDelete_Click(object selectedItem)
        //{
        //    PatientPCLRequest request = selectedItem as PatientPCLRequest;
        //    if (MessageBox.Show("Bạn có muốn xóa phiếu để cập nhật lại kết quả", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        //    {
        //        DeletePCLRequestFromList(request);
        //    }
        //}

        //public IEnumerator<IResult> DeleteHistoryTransaction(PatientPCLRequest request)
        //{
        //    var dialog = new MessageWarningShowDialogTask("Bạn có chắc chắn muốn xóa phiếu này không?", "Đồng ý");
        //    yield return dialog;
        //    if (dialog.IsAccept)
        //    {
        //        DeleteTransactionHistory(request);
        //    }
        //    yield break;
        //}
        //public void hplDeleteHistory_Click(object selectedItem)
        //{
        //    PatientPCLRequest request = selectedItem as PatientPCLRequest;
        //    Coroutine.BeginExecute(DeleteHistoryTransaction(request));
        //}

        //public void hplCheck_Click(object selectedItem)
        //{
        //    if (ListSendTransaction.Where(x => x.IsChecked).Count() > Globals.ServerConfigSection.CommonItems.CountSendTransaction)
        //    {
        //        (selectedItem as PatientPCLRequest).IsChecked = false;
        //        MessageBox.Show("Số lượng phiếu vượt quá trong 1 giao dịch được cấu hình \"" + Globals.ServerConfigSection.CommonItems.CountSendTransaction + "\"");
        //    }
        //}

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

        //private IEnumerator<IResult> UpdatePCLRequestStatus_Routine(object ListPCLResults)
        //{
        //    this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);

        //    var mUpdatePCLRequestStatusTask = new GenericCoRoutineTask(UpdatePCLRequestStatusTask, ListPCLResults, (long)AllLookupValues.V_PCLRequestStatus.SIGNING);
        //    yield return mUpdatePCLRequestStatusTask;

        //    var mGetPDFResultToSignTask = new GenericCoRoutineTask(GetPDFResultToSignTask, ListPCLResults);
        //    yield return mGetPDFResultToSignTask;

        //    // nếu lấy dữ liệu thất bại thì cập nhật lại trạng thái của các phiếu trên về bình thường. Khi gửi kết quả thì phiếu phải là hoàn tất
        //    bool IsGetPDFResultSuccess = (bool)mGetPDFResultToSignTask.GetResultObj(0);
        //    if (!IsGetPDFResultSuccess)
        //    {
        //        var mReUpdatePCLRequestStatusTask = new GenericCoRoutineTask(UpdatePCLRequestStatusTask, ListPCLResults, (long)AllLookupValues.V_PCLRequestStatus.CLOSE);
        //        yield return mReUpdatePCLRequestStatusTask;
        //    }
        //    else
        //    {
        //        var mReUpdatePCLRequestStatusTask = new GenericCoRoutineTask(UpdatePCLRequestStatusTask, ListPCLResults, (long)AllLookupValues.V_PCLRequestStatus.SIGNED);
        //        yield return mReUpdatePCLRequestStatusTask;
        //    }

        //    this.HideBusyIndicator();
        //}

        // đánh dấu trạng thái danh sách đang lấy kết quả + gửi giao dịch. tránh 2 màn hình
        //private void UpdatePCLRequestStatusTask(GenericCoRoutineTask genTask, object ListPCLResults, object V_PCLRequestStatus)
        //{
        //    var t = new Thread(() =>
        //    {
        //        try
        //        {
        //            using (var serviceFactory = new PCLsClient())
        //            {
        //                var contract = serviceFactory.ServiceInstance;
        //                contract.BeginUpdatePCLRequestStatusGeneratingResult(ListPCLResults.ToString(), (long)V_PCLRequestStatus
        //                    , Globals.DispatchCallback(asyncResult =>
        //                    {
        //                        try
        //                        {
        //                            var data = contract.EndUpdatePCLRequestStatusGeneratingResult(asyncResult);
        //                            this.HideBusyIndicator();
        //                            if (!data)
        //                            {
        //                                MessageBox.Show("Không tìm thấy phiếu chỉ định. Vui lòng kiểm tra lại hoặc liên hệ IT!");
        //                                genTask.ActionComplete(false);
        //                            }
        //                            else
        //                            {
        //                                if(Globals.ServerConfigSection.CommonItems.IsEnableSendSMSLab && (long)V_PCLRequestStatus == (long)AllLookupValues.V_PCLRequestStatus.SIGNED)
        //                                {
        //                                    //SendSMS(ListPCLResults);
        //                                }
        //                                genTask.ActionComplete(true);
        //                            }
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            MessageBox.Show(ex.Message);
        //                            this.HideBusyIndicator();
        //                            genTask.ActionComplete(false);
        //                        }
        //                    }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            ClientLoggerHelper.LogInfo(ex.ToString());
        //            MessageBox.Show(ex.Message);
        //            this.HideBusyIndicator();
        //            genTask.ActionComplete(false);
        //        }
        //    });

        //    t.Start();
        //}

        //private HIAPIServiceResponse _HIAPIServiceResponse;
        //public HIAPIServiceResponse HIAPIServiceResponse
        //{
        //    get => _HIAPIServiceResponse; set
        //    {
        //        _HIAPIServiceResponse = value;
        //        NotifyOfPropertyChange(() => HIAPIServiceResponse);
        //    }
        //}

        //private void GetPDFResultToSignTask(GenericCoRoutineTask genTask, object ListSendTransactionXml)
        //{
        //    try
        //    {
        //        using (var serviceFactory = new ReportServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;
        //            contract.BeginGetPDFResulstToSign(ListSendTransactionXml.ToString(), Globals.ServerConfigSection.ConsultationElements.LaboratoryResultVersion
        //                , Globals.LoggedUserAccount.Staff.FullName
        //                , Globals.ServerConfigSection.Hospitals.PrescriptionMainRightHeader, Globals.ServerConfigSection.Hospitals.PrescriptionSubRightHeader
        //                , parHeadOfLaboratoryFullName != null ? parHeadOfLaboratoryFullName.FullNameString : ""
        //                , parHeadOfLaboratoryFullName != null ? parHeadOfLaboratoryFullName.PositionName : ""
        //                , Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth
        //                , Globals.ServerConfigSection.CommonItems.ReportHospitalAddress
        //                , Globals.ServerConfigSection.CommonItems.ReportHospitalName
        //                , Globals.ServerConfigSection.Hospitals.HospitalCode
        //                , Globals.ServerConfigSection.CommonItems.ServicePool, Globals.ServerConfigSection.CommonItems.PDFFileResultToSignPath
        //                , Globals.ServerConfigSection.CommonItems.PDFFileResultSignedPath
        //                , Globals.ServerConfigSection.Hospitals.FTPAdminUserName
        //                , Globals.ServerConfigSection.Hospitals.FTPAdminPassword
        //                , Globals.ServerConfigSection.CommonItems.FTPServerSighHashUrl
        //                , Globals.DispatchCallback(asyncResult =>
        //                {
        //                    try
        //                    {
        //                        var data = contract.EndGetPDFResulstToSign(asyncResult);
        //                        if (!string.IsNullOrEmpty(data))
        //                        {
        //                            XmlTextReader txtReader = new XmlTextReader(new StringReader(data));
        //                            XmlSerializer serializer = new XmlSerializer(typeof(Root));
        //                            Root RootPatientPCLRequest = (Root)serializer.Deserialize(txtReader);
        //                            string Password = EncryptExtension.Encrypt(PwdBoxCtrl.Password, Globals.AxonKey, Globals.AxonPass);
        //                            //string Username = EncryptExtension.Encrypt(ApproveUserName, Globals.AxonKey, Globals.AxonPass);
        //                            string mJsonData = string.Format("{{\"username\":\"{0}\",\"password\":\"{1}\",\"listPDFResults\":{2},\"ftpusername\":\"{3}\",\"ftppassword\":\"{4}\",\"ftpurl\":\"{5}\"}}"
        //                            , ApproveUserName, Password, GlobalsNAV.ConvertObjectToJson(RootPatientPCLRequest.Item)
        //                            , "", ""
        //                            , Globals.ServerConfigSection.CommonItems.ServiceUrl);
        //                            //string mRestJson = GlobalsNAV.GetRESTServiceJSon(Globals.ServerConfigSection.CommonItems.HISSighHashSmartCAUrl, mJsonData);

        //                            TimeSpan timeout = new TimeSpan(0, 6, 30);
        //                            var resultResponse = GlobalsNAV.RequestPOST(Globals.ServerConfigSection.CommonItems.HISSighHashSmartCAUrl, "", mJsonData, timeout);
        //                            if (resultResponse == null)
        //                            {
        //                                genTask.AddResultObj(false);
        //                                MessageBox.Show("BadGateway");
        //                            }

        //                            if (resultResponse != null && resultResponse != HttpStatusCode.NotFound.ToString())
        //                            {
        //                                if (HIAPIServiceResponse != null)
        //                                {
        //                                    HIAPIServiceResponse = null;
        //                                }
        //                                HIAPIServiceResponse = GlobalsNAV.ConvertJsonToObject<HIAPIServiceResponse>(resultResponse);
        //                                if (HIAPIServiceResponse.success)
        //                                {
        //                                    genTask.AddResultObj(true);
        //                                    GlobalsNAV.ShowMessagePopup(HIAPIServiceResponse.message);
        //                                    ListSendTransaction_Paging(0, ListSendTransaction.PageSize, true);
        //                                }
        //                                else
        //                                {
        //                                    genTask.AddResultObj(false);
        //                                    MessageBox.Show(HIAPIServiceResponse.message);
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            MessageBox.Show("Lỗi khi xuất kết quả. Vui lòng kiểm tra kết quả của từng phiếu hoặc liên hệ IT!", eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
        //                            genTask.AddResultObj(false);
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        ClientLoggerHelper.LogInfo(ex.ToString());
        //                        MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
        //                        genTask.AddResultObj(false);
        //                    }
        //                    finally
        //                    {
        //                        this.HideBusyIndicator();
        //                        genTask.ActionComplete(true);
        //                    }
        //                }), null);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ClientLoggerHelper.LogInfo(ex.ToString());
        //        MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
        //        this.HideBusyIndicator();
        //        genTask.AddResultObj(false);
        //        genTask.ActionComplete(true);
        //    }
        //}

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
    }
}
