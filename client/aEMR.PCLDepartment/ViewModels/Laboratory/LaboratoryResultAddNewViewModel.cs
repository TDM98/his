using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using PCLsProxy;
using aEMR.CommonTasks;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Common.BaseModel;
using aEMR.Common;
using System.Windows.Input;
using aEMR.Common.HotKeyManagement;
using aEMR.Controls;
using System.Windows.Controls;
using aEMR.Common.Collections;
using System.Linq;
using aEMR.Common.Printing;
/*
* 20181024 TNHX: [BM0003200] Refactor code
* 20181216 TNHX: [BM0005430] Add PatientID for report PCLDEPARTMENT_LABORATORY_RESULT
* 20190502 #001 TTM:   BM 0006780: Thêm trường đề nghị (Suggest) cho xét nghiệm
* 20190518 #002 TTM:   BM 0006782: Thêm hot key cho xét nghiệm.
* 20210527 #003 TNHX: Thêm Chọn mã máy cho test con
* 20220215 #004 QTD:   Thêm QuickPrint cho kết quả XN
* 20220829 #005 BLQ:  Thêm phân quyền cập nhật kết quả xét nghiệm
* 20221129 #006 TNHX:  Thêm xem kết quả đã ký số
* 20230419 #007 QTD:  
    + Thêm người thực hiện
    + Chỉnh lại nút khóa
* 20230518 #008 DatTB: Thêm các trường lưu bệnh phẩm xét nghiệm
* 20230603 #009 DatTB: Thêm chức năng song ngữ mẫu kết quả xét nghiệm
*/
namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof(ILaboratoryResultAddNew)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class LaboratoryResultAddNewViewModel : ViewModelBase, ILaboratoryResultAddNew
        , IHandle<DbClickSelectedObjectEvent<PatientPCLRequest>>
        , IHandle<DbClickPatientPCLRequest<PatientPCLRequest, string>>
        , IHandle<LocationSelected>
    {
        [ImportingConstructor]
        public LaboratoryResultAddNewViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);

            //if (Globals.PatientPCLReqID_LAB > 0 && Globals.PatientAllDetails.PatientInfo != null && Globals.PatientAllDetails.PatientInfo.PatientID > 0)
            //{
            //    PatientID = Globals.PatientAllDetails.PatientInfo.PatientID;
            //    PatientPCLReqID = Globals.PatientPCLReqID_LAB;
            //    PCLLaboratoryResults_With_ResultOld(PatientID, PatientPCLReqID);
            //}
            StaffName = Globals.LoggedUserAccount.Staff.FullName;
            Content = SAVE;
            //▼====: #003
            GetResourcesForLaboratory();
            //▲====: #003
            //▼====: #007
            LoadPerformStaffCollection();
            //▲====: #007
            //▼==== #008
            GetAllSpecimen();
            //▲==== #008
            authorization();
        }
        //▼====== #002: Đặt base.HasInputBindingCmd ở onactive và ondeactive vì: Khi vào sẽ mở chức năng cho hot key của màn hình, khi thoát ra màn hình sẽ deactive hot key đi. Vì đây là hot key Globals nếu ko deactive thì khi người
        //              Khi người dùng thoát khỏi màn hình xét nghiệm vào màn hình khác mà không set false cho biết HasInputBindingCmd thì khi bấm tổ hợp phím hot key xét nghiệm vẫn đi lưu mặc dù đang ở màn hình khác, không liên quan.           
        protected override void OnActivate()
        {
            base.OnActivate();
            base.HasInputBindingCmd = true;
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            base.HasInputBindingCmd = false;
        }
        //▲====== #002
        #region Properties
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        private long PatientPCLReqID = 0;
        private long PatientID = 0;
        private string StaffName = "";
        private bool _isEnable = true;
        public bool IsEnable
        {
            get { return _isEnable; }
            set
            {
                if (_isEnable != value)
                {
                    _isEnable = value;
                    NotifyOfPropertyChange(() => IsEnable);
                }
            }
        }
        private PatientPCLRequest _CurPatientPCLRequest;
        public PatientPCLRequest CurPatientPCLRequest
        {
            get
            {
                return _CurPatientPCLRequest;
            }
            set
            {
                if (_CurPatientPCLRequest == value)
                    return;
                _CurPatientPCLRequest = value;
                NotifyOfPropertyChange(() => CurPatientPCLRequest);
            }
        }
        public long PCLRequestTypeID
        {
            get
            {
                return CurPatientPCLRequest != null && (long)CurPatientPCLRequest.V_PCLRequestType > 0 ? (long)CurPatientPCLRequest.V_PCLRequestType : 0;
            }
        }
        private PatientPCLLaboratoryResultDetail _CurPatientPCLLaboratoryResultDetail;
        public PatientPCLLaboratoryResultDetail CurPatientPCLLaboratoryResultDetail
        {
            get
            {
                return _CurPatientPCLLaboratoryResultDetail;
            }
            set
            {
                if (_CurPatientPCLLaboratoryResultDetail == value)
                    return;
                _CurPatientPCLLaboratoryResultDetail = value;
                NotifyOfPropertyChange(() => CurPatientPCLLaboratoryResultDetail);
            }
        }
        private ObservableCollection<PatientPCLLaboratoryResultDetail> _allPatientPCLLaboratoryResultDetail;
        public ObservableCollection<PatientPCLLaboratoryResultDetail> allPatientPCLLaboratoryResultDetail
        {
            get
            {
                return _allPatientPCLLaboratoryResultDetail;
            }
            set
            {
                if (_allPatientPCLLaboratoryResultDetail == value)
                    return;
                _allPatientPCLLaboratoryResultDetail = value;
                NotifyOfPropertyChange(() => allPatientPCLLaboratoryResultDetail);
            }
        }
        private PatientPCLLaboratoryResult _curPatientPCLLaboratoryResult;
        public PatientPCLLaboratoryResult curPatientPCLLaboratoryResult
        {
            get
            {
                return _curPatientPCLLaboratoryResult;
            }
            set
            {
                if (_curPatientPCLLaboratoryResult == value)
                    return;
                _curPatientPCLLaboratoryResult = value;
                NotifyOfPropertyChange(() => curPatientPCLLaboratoryResult);
            }
        }
        private string _Content;
        public string Content
        {
            get
            {
                return _Content;
            }
            set
            {
                if (_Content != value)
                {
                    _Content = value;
                    NotifyOfPropertyChange(() => Content);
                }
            }
        }
        private string SAVE = eHCMSResources.T2937_G1_Luu;
        private string UPDATE = eHCMSResources.K1599_G1_CNhat;

        //▼====: #003
        private ObservableCollection<Resources> _HIRepResourceCodeCollection;
        public ObservableCollection<Resources> HIRepResourceCodeCollection
        {
            get { return _HIRepResourceCodeCollection; }
            set
            {
                if (HIRepResourceCodeCollection != value)
                {
                    _HIRepResourceCodeCollection = value;
                }
                NotifyOfPropertyChange(() => HIRepResourceCodeCollection);
            }
        }
        //▲====: #003
        private bool _IsWaitResultEnabled = false;
        public bool IsWaitResultEnabled
        {
            get
            {
                return _IsWaitResultEnabled;
            }
            set
            {
                if (_IsWaitResultEnabled != value)
                {
                    _IsWaitResultEnabled = value;
                    NotifyOfPropertyChange(() => IsWaitResultEnabled);
                }
            }
        }
        private bool _IsWaitResultVisibility = false;
        public bool IsWaitResultVisibility
        {
            get
            {
                return _IsWaitResultVisibility;
            }
            set
            {
                if (_IsWaitResultVisibility != value)
                {
                    _IsWaitResultVisibility = value;
                    NotifyOfPropertyChange(() => IsWaitResultVisibility);
                }
            }
        }
        private bool _IsDoneVisibility = false;
        public bool IsDoneVisibility
        {
            get
            {
                return _IsDoneVisibility;
            }
            set
            {
                if (_IsDoneVisibility != value)
                {
                    _IsDoneVisibility = value;
                    NotifyOfPropertyChange(() => IsDoneVisibility);
                }
            }
        }
        #endregion

        #region Methods
        private void PCLLaboratoryResults_With_ResultOld(long PatientID, long PatientPCLReqID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        IPCLs contract = serviceFactory.ServiceInstance;
                        contract.BeginPCLLaboratoryResults_With_ResultOld(PatientID, PatientPCLReqID, PCLRequestTypeID, Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    IList<PatientPCLLaboratoryResultDetail> results = contract.EndPCLLaboratoryResults_With_ResultOld(asyncResult);
                                    if (results == null || results.Count < 1)
                                    {
                                        MessageBox.Show(eHCMSResources.A0904_G1_Msg_InfoPhPCLDaHuy);
                                        IsEnable = false;
                                    }
                                    else
                                    {
                                        allPatientPCLLaboratoryResultDetail = new ObservableCollection<PatientPCLLaboratoryResultDetail>(results);
                                        //foreach (PatientPCLLaboratoryResultDetail patientPclLaboratoryResultDetail in results)
                                        //{
                                        //    allPatientPCLLaboratoryResultDetail.Add(patientPclLaboratoryResultDetail);
                                        //}
                                        NotifyOfPropertyChange(() => allPatientPCLLaboratoryResultDetail);
                                        curPatientPCLLaboratoryResult = new PatientPCLLaboratoryResult();

                                        if (allPatientPCLLaboratoryResultDetail != null && allPatientPCLLaboratoryResultDetail.Count > 0)
                                        {
                                            curPatientPCLLaboratoryResult.LabResultID = allPatientPCLLaboratoryResultDetail[0].LabResultID;
                                            //▼===== #001 Lấy từ Index thứ 1 vì số 0 là tiêu đề không có thông tin của trường đề nghị
                                            curPatientPCLLaboratoryResult.Suggest = allPatientPCLLaboratoryResultDetail[1].PatientPCLLaboratoryResult.Suggest;
                                            curPatientPCLLaboratoryResult.DigitalSignatureResultPath = allPatientPCLLaboratoryResultDetail[1].PatientPCLLaboratoryResult.DigitalSignatureResultPath;
                                            //▲===== #001
                                            //▼===== #008
                                            if (allPatientPCLLaboratoryResultDetail[1] != null && allPatientPCLLaboratoryResultDetail[1].PatientPCLLaboratoryResult != null)
                                            {
                                                curPatientPCLLaboratoryResult.SpecimenID = allPatientPCLLaboratoryResultDetail[1].PatientPCLLaboratoryResult.SpecimenID;
                                                if (allPatientPCLLaboratoryResultDetail[1].PatientPCLLaboratoryResult.PatientPCLRequest != null)
                                                {
                                                    CurPatientPCLRequest.ReceptionTime = allPatientPCLLaboratoryResultDetail[1].PatientPCLLaboratoryResult.PatientPCLRequest.ReceptionTime;
                                                }
                                                if (allPatientPCLLaboratoryResultDetail[1].PatientPCLLaboratoryResult.SampleQuality != null)
                                                {
                                                    curPatientPCLLaboratoryResult.SampleQuality = allPatientPCLLaboratoryResultDetail[1].PatientPCLLaboratoryResult.SampleQuality;
                                                }
                                                else
                                                {
                                                    curPatientPCLLaboratoryResult.SampleQuality = "Đạt";
                                                }
                                            }
                                            //▲===== #008

                                            //▼===== #004
                                            IsCanPrint = false;
                                            foreach (var itemResult in allPatientPCLLaboratoryResultDetail)
                                            {
                                                if ((itemResult.Value != null && itemResult.Value != ""))
                                                {
                                                    IsCanPrint = true;
                                                    break;
                                                }
                                            }
                                            //▲===== #004
                                        }

                                        curPatientPCLLaboratoryResult.PatientPCLReqID = PatientPCLReqID;
                                        curPatientPCLLaboratoryResult.StaffID = Globals.LoggedUserAccount.StaffID;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
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
                    this.HideBusyIndicator();
                    MessageBox.Show(ex.Message);
                }
            });

            t.Start();
        }
        public void BtnDelete()
        {
            if (PatientPCLReqID <= 0)
            {
                MessageBox.Show(eHCMSResources.Z2375_G1_ChonYCXN);
                return;
            }
            if (MessageBox.Show(eHCMSResources.K0483_G1_BanCoChacChanMuonXoa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeletePatientPCLLaboratoryResult(PatientPCLReqID, Globals.LoggedUserAccount.Staff.StaffID);
            }
        }
        private void DeletePatientPCLLaboratoryResult(long PatientPCLReqID, long CancelStaffID, long PCLExamTypeID = 0)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        IPCLs contract = serviceFactory.ServiceInstance;
                        contract.BeginDeletePatientPCLLaboratoryResult(PatientPCLReqID, PCLRequestTypeID, CancelStaffID, PCLExamTypeID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndDeletePatientPCLLaboratoryResult(asyncResult);
                                if (results)
                                {
                                    PCLLaboratoryResults_With_ResultOld(PatientID, PatientPCLReqID);
                                    GlobalsNAV.ShowMessagePopup(eHCMSResources.K0537_G1_XoaOk);
                                    Content = SAVE;
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.K0484_G1_XoaFail);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
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
        public void BtnPrint()
        {
            if (PatientPCLReqID <= 0)
            {
                MessageBox.Show(eHCMSResources.Z2375_G1_ChonYCXN);
                return;
            }

            //var proAlloc = Globals.GetViewModel<ICommonPreviewView>();
            //proAlloc.PatientPCLReqID =(int)PatientPCLReqID;
            //proAlloc.V_PCLRequestType = PCLRequestTypeID;
            //proAlloc.FindPatient = PCLRequestTypeID > 0 && PCLRequestTypeID == (long)AllLookupValues.V_PCLRequestType.NOI_TRU ? (int)AllLookupValues.PatientFindBy.NOITRU : (int)AllLookupValues.PatientFindBy.NGOAITRU;
            //proAlloc.eItem = ReportName.PCLDEPARTMENT_LABORATORY_RESULT;

            //var instance = proAlloc as Conductor<object>;s
            //Globals.ShowDialog(instance, (o) => { });
            //▼====: #006
            if (String.IsNullOrEmpty(curPatientPCLLaboratoryResult.DigitalSignatureResultPath))
            {
                void onInitDlg(ICommonPreviewView proAlloc)
                {
                    proAlloc.PatientID = PatientID;
                    proAlloc.PatientPCLReqID = (int)PatientPCLReqID;
                    proAlloc.V_PCLRequestType = PCLRequestTypeID;
                    proAlloc.FindPatient = PCLRequestTypeID > 0 && PCLRequestTypeID == (long)AllLookupValues.V_PCLRequestType.NOI_TRU ? (int)AllLookupValues.PatientFindBy.NOITRU : (int)AllLookupValues.PatientFindBy.NGOAITRU;
                    proAlloc.StaffName = StaffName;
                    proAlloc.IsBilingual = false;
                    proAlloc.eItem = ReportName.PCLDEPARTMENT_LABORATORY_RESULT;
                }
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
            }
            else
            {
                void onInitDlg(IPDFPreviewView proAlloc)
                {
                    proAlloc.DigitalSignatureResultPath = curPatientPCLLaboratoryResult.DigitalSignatureResultPath;
                }
                GlobalsNAV.ShowDialog<IPDFPreviewView>(onInitDlg);
            }
            //▲====: #006
        }
        public IEnumerator<IResult> ShowMessage(string message)
        {
            var dialog = new MessageWarningShowDialogTask(message, "", false);
            yield return dialog;
            yield break;
        }
        private void UpdatePatientPCLLaboratoryResultDetailXml(IList<PatientPCLLaboratoryResultDetail> allPatientPCLLaboratoryResultDetailentity, PatientPCLLaboratoryResult entity)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        IPCLs contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdatePatientPCLLaboratoryResultDetailXml(allPatientPCLLaboratoryResultDetailentity, entity, PCLRequestTypeID, PatientID,CurPatientPCLRequest.IsWaitResult, CurPatientPCLRequest.IsDone, Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var results = contract.EndUpdatePatientPCLLaboratoryResultDetailXml(out string errorMessage, asyncResult);
                                    if (!string.IsNullOrEmpty(errorMessage))
                                    {
                                        switch (errorMessage)
                                        {
                                            case "Cancel":
                                            case "C":
                                                Coroutine.BeginExecute(ShowMessage(eHCMSResources.Z1766_G1_PhDaHuyCNhatLaiTTin));
                                                break;
                                            case "Delete":
                                            case "D":
                                                Coroutine.BeginExecute(ShowMessage(eHCMSResources.Z1767_G1_PhDaThayDoiCNhatTTin));
                                                break;
                                            default:
                                                Coroutine.BeginExecute(ShowMessage(errorMessage));
                                                break;
                                        }
                                    }
                                    else
                                    if (results)
                                    {
                                        PCLLaboratoryResults_With_ResultOld(PatientID, PatientPCLReqID);
                                        GlobalsNAV.ShowMessagePopup(eHCMSResources.Z1562_G1_DaLuu);
                                        Content = UPDATE;
                                        //▼====== #005
                                        CheckAuthorbtnUpdate();
                                        //▲====== #005
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                    _logger.Error(ex.Message);
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
                    _logger.Error(ex.Message);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }
        public void butUpdate()
        {
            if (PatientPCLReqID <= 0)
            {
                MessageBox.Show(eHCMSResources.Z2375_G1_ChonYCXN);
                return;
            }
            UpdatePatientPCLLaboratoryResultDetailXml(allPatientPCLLaboratoryResultDetail, curPatientPCLLaboratoryResult);
        }
        //public void butReset()
        //{
        //    GetPatientPCLLaboratoryResults_ByPatientPCLReqID(PatientPCLReqID);
        //}
        public void btnDeleteItemResult(object source)
        {
            if (source == null || !(source is PatientPCLLaboratoryResultDetail) || ((source as PatientPCLLaboratoryResultDetail).PCLExamType == null) || string.IsNullOrEmpty((source as PatientPCLLaboratoryResultDetail).PCLExamType.PCLExamTypeName) || PatientPCLReqID == 0
                || Globals.LoggedUserAccount == null
                || Globals.LoggedUserAccount.Staff == null
                || (source as PatientPCLLaboratoryResultDetail).PCLExamTypeID == 0)
            {
                return;
            }
            IErrorBold WarningDiag = Globals.GetViewModel<IErrorBold>();
            WarningDiag.isCheckBox = true;
            WarningDiag.SetMessage(string.Format(eHCMSResources.Z2590_G1_CoChacMuonXoaKQCLS, (source as PatientPCLLaboratoryResultDetail).PCLExamType.PCLExamTypeName), eHCMSResources.K3847_G1_DongY);
            WarningDiag.FireOncloseEvent = false;
            GlobalsNAV.ShowDialog_V3(WarningDiag);
            if (WarningDiag.IsAccept)
            {
                DeletePatientPCLLaboratoryResult(PatientPCLReqID, Globals.LoggedUserAccount.Staff.StaffID, (source as PatientPCLLaboratoryResultDetail).PCLExamTypeID);
            }
        }

        //▼====: #003
        private void GetResourcesForLaboratory()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        IPCLs contract = serviceFactory.ServiceInstance;
                        contract.BeginGetResourcesForLaboratory(Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    List<Resources> results = contract.EndGetResourcesForLaboratory(asyncResult);
                                    if (results != null)
                                    {
                                        if (HIRepResourceCodeCollection == null)
                                        {
                                            HIRepResourceCodeCollection = new ObservableCollection<Resources>();
                                        }
                                        else
                                        {
                                            HIRepResourceCodeCollection.Clear();
                                        }
                                        HIRepResourceCodeCollection = results.ToObservableCollection();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
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
                    MessageBox.Show(ex.Message);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        AxComboBox Cbo;
        public void cboHiRepResourceCode_Loaded(object sender, SelectionChangedEventArgs e)
        {
            Cbo = sender as AxComboBox;
            var item = Cbo.DataContext as PatientPCLLaboratoryResultDetail;
            if (item.PCLExamTypeTestItemID == 0)
            {
                Cbo.Visibility = Visibility.Hidden;
            }
        }

        public void cboHiRepResourceCode_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (sender != null)
            {
                var itemSender = sender as AxComboBox;
                var item = itemSender.DataContext as PatientPCLLaboratoryResultDetail;
                //▼====: #007
                //if (!item.IsTemporaryBlock && itemSender.SelectedValue != null && item.HIRepResourceCode != itemSender.SelectedValue.ToString())
                //{
                //    foreach (var itemResultDetail in allPatientPCLLaboratoryResultDetail)
                //    {
                //        if (itemResultDetail.PCLSectionID == item.PCLSectionID)
                //        {
                //            itemResultDetail.HIRepResourceCode = itemSender.SelectedValue.ToString();
                //        }
                //    }
                //}
                if (itemSender.SelectedValue != null && item.HIRepResourceCode != itemSender.SelectedValue.ToString())
                {
                    if (item.IsTemporaryBlock)
                    {
                        foreach (var itemResultDetail in allPatientPCLLaboratoryResultDetail.Where(x => x.IsTemporaryBlock))
                        {
                            if (itemResultDetail.PCLSectionID == item.PCLSectionID)
                            {
                                itemResultDetail.HIRepResourceCode = itemSender.SelectedValue.ToString();
                            }
                        }
                    }
                    else
                    {
                        foreach (var itemResultDetail in allPatientPCLLaboratoryResultDetail.Where(x => !x.IsTemporaryBlock))
                        {
                            if (itemResultDetail.PCLSectionID == item.PCLSectionID)
                            {
                                itemResultDetail.HIRepResourceCode = itemSender.SelectedValue.ToString();
                            }
                        }
                    }
                }
                //▲====: #007
            }
        }

        public void cboHiRepResourceCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender != null)
            {
                var itemSender = sender as AxComboBox;
                var item = itemSender.DataContext as PatientPCLLaboratoryResultDetail;
                if (!item.IsTemporaryBlock && itemSender.SelectedValue != null)
                {
                    foreach (var itemResultDetail in allPatientPCLLaboratoryResultDetail)
                    {
                        if (itemResultDetail.PCLSectionID == item.PCLSectionID)
                        {
                            itemResultDetail.HIRepResourceCode = itemSender.SelectedValue.ToString();
                        }
                    }
                }
            }
        }
        public void ckbIsChecked_Click(object source, object sender)
        {
            CheckBox ckbIsChecked = source as CheckBox;
            if (!(ckbIsChecked.DataContext is PatientPCLLaboratoryResultDetail))
            {
                return;
            }
            PatientPCLLaboratoryResultDetail item = (ckbIsChecked.DataContext as PatientPCLLaboratoryResultDetail);
            item.IsTemporaryBlock = ckbIsChecked.IsChecked.GetValueOrDefault(true);
        }
        //▲====: #003
        #endregion

        #region Handles
        public void Handle(DbClickSelectedObjectEvent<PatientPCLRequest> message)
        {
            if (message != null)
            {
                if (Globals.PCLDepartment.ObjV_PCLMainCategory.LookupID == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
                {
                    PatientPCLReqID = message.Result.PatientPCLReqID;
                    PCLLaboratoryResults_With_ResultOld(message.Result.PatientID, message.Result.PatientPCLReqID);
                    CurPatientPCLRequest = message.Result;
                    IsWaitResultVisibility = message.Result.IsHaveWaitResult;
                    if (CurPatientPCLRequest.V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.CLOSE || CurPatientPCLRequest.V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.SIGNED)
                    {
                        Content = UPDATE;
                        IsWaitResultEnabled = false;
                        if (CurPatientPCLRequest.IsWaitResult)
                        {
                            IsDoneVisibility = true;
                        }
                        else
                        {
                            IsDoneVisibility = false;
                        }
                    }
                    else
                    {
                        Content = SAVE;
                        IsWaitResultEnabled = true;
                        IsDoneVisibility = false;
                    }
                    //▼====== #005
                    CheckAuthorbtnUpdate();
                    //▲====== #005
                    PatientID = message.Result.PatientID;
                }
                IsEnable = !message.IsReadOnly;
            }
        }
        public void Handle(DbClickPatientPCLRequest<PatientPCLRequest, string> message)
        {
            if (message != null)
            {
                if (message.ObjB == eHCMSResources.Z0055_G1_Edit)
                {
                    PatientPCLReqID = message.ObjA.PatientPCLReqID;
                    PatientID = message.ObjA.PatientID;
                    PCLLaboratoryResults_With_ResultOld(message.ObjA.PatientID, message.ObjA.PatientPCLReqID);
                    CurPatientPCLRequest = message.ObjA;
                    IsWaitResultVisibility = message.ObjA.IsHaveWaitResult;
                    if (CurPatientPCLRequest.V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.CLOSE)
                    {
                        Content = UPDATE;
                        IsWaitResultEnabled = false;
                        if (CurPatientPCLRequest.IsWaitResult)
                        {
                            IsDoneVisibility = true;
                        }
                        else
                        {
                            IsDoneVisibility = false;
                        }
                    }
                    else
                    {
                        Content = SAVE;
                        IsWaitResultEnabled = true;
                        IsDoneVisibility = false;
                    }
                    //▼====== #005
                    CheckAuthorbtnUpdate();
                    //▲====== #005
                }
                IsEnable = message.IsReadOnly;
            }
        }
        public void Handle(LocationSelected message)
        {
            if (message != null && message.DeptLocation != null)
            {
                PatientPCLReqID = 0;
                allPatientPCLLaboratoryResultDetail = new ObservableCollection<PatientPCLLaboratoryResultDetail>();
            }
        }

        //▼====== #002
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
            yield return new InputBindingCommand(() => SaveCmd())
            {
                HotKey_Registered_Name = "ghkSaveLaboratory",
                GestureModifier = ModifierKeys.Control,
                GestureKey = (Key)Keys.S
            };
        }
        private void SaveCmd()
        {
            butUpdate();
        }
        //▲====== #002
        #endregion
        public void ckbIsWaitResult_Click(object source, object sender)
        {
            if(CurPatientPCLRequest == null)
            {
                return;
            }
            CheckBox ckbIsChecked = source as CheckBox;
            if (!ckbIsChecked.IsChecked.GetValueOrDefault(false))
            {
                CurPatientPCLRequest.IsDone = false;
            }
            if (ckbIsChecked != null)
            {
                foreach(var itemResult in allPatientPCLLaboratoryResultDetail)
                {
                    if (itemResult.PCLExamTestItemID != 0)
                    {
                        if ((bool)ckbIsChecked.IsChecked && (itemResult.Value == null || itemResult.Value == "") && itemResult.PCLExamType.IsAllowEditAfterDischarge)
                        {
                            itemResult.Value = "Chờ kết quả";
                        }
                        else if (!(bool)ckbIsChecked.IsChecked && itemResult.Value == "Chờ kết quả")
                        {
                            itemResult.Value = "";
                        }
                    }
                }
            }
        }

        //▼====== #004
        private bool _IsCanPrint = false;
        public bool IsCanPrint
        {
            get
            {
                return _IsCanPrint;
            }
            set
            {
                if (_IsCanPrint == value) return;
                _IsCanPrint = value;
                NotifyOfPropertyChange(() => IsCanPrint);
            }
        }

        public void btnPrintNew()
        {
            StaffPosition parHeadOfLaboratoryFullName = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.TRUONG_KHOA_XET_NGHIEM && x.IsActive).FirstOrDefault();
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ReportServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetXRptPCLDepartmentLaboratoryResultReportModel_TV3PdfFormat(Globals.ServerConfigSection.Hospitals.PrescriptionMainRightHeader
                            , Globals.ServerConfigSection.Hospitals.PrescriptionSubRightHeader
                            , parHeadOfLaboratoryFullName != null ? parHeadOfLaboratoryFullName.FullNameString : ""
                            , parHeadOfLaboratoryFullName != null ? parHeadOfLaboratoryFullName.PositionName : ""
                            , Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth
                            , Globals.ServerConfigSection.CommonItems.ReportHospitalAddress
                            , Globals.ServerConfigSection.CommonItems.ReportHospitalName
                            , Globals.ServerConfigSection.Hospitals.HospitalCode
                            , (int)PatientID
                            , (int)PatientPCLReqID
                            , (int)PCLRequestTypeID
                            , PCLRequestTypeID > 0 && PCLRequestTypeID == (long)AllLookupValues.V_PCLRequestType.NOI_TRU ? (int)AllLookupValues.PatientFindBy.NOITRU : (int)AllLookupValues.PatientFindBy.NGOAITRU
                            , StaffName
                            , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetXRptPCLDepartmentLaboratoryResultReportModel_TV3PdfFormat(asyncResult);
                                if (results == null)
                                {
                                    return;
                                }
                                var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, results, ActiveXPrintType.ByteArray, "A4");
                                Globals.EventAggregator.Publish(printEvt);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
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
                    MessageBox.Show(ex.Message);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        //▲====== #004
        //▼====== #005
        #region authorization
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            mCapNhatKetQua = Globals.CheckOperation(Globals.listRefModule, (int)eModules.mCLSLaboratory, (int)eCLSLaboratory.mXetNghiem, (int)oCLSLaboratoryEx.mXetNghiem_CapNhatKetQua);
        }
        private bool _mCapNhatKetQua = true;
        public bool mCapNhatKetQua
        {
            get
            {
                return _mCapNhatKetQua;
            }
            set
            {
                if (_mCapNhatKetQua == value)
                    return;
                _mCapNhatKetQua = value;
                NotifyOfPropertyChange(() => mCapNhatKetQua);
            }
        }
        #endregion
        private bool _btnUpdateEnable = true;
        public bool btnUpdateEnable
        {
            get
            {
                return _btnUpdateEnable;
            }
            set
            {
                if (_btnUpdateEnable == value)
                    return;
                _btnUpdateEnable = value;
                NotifyOfPropertyChange(() => btnUpdateEnable);
            }
        }
        private void CheckAuthorbtnUpdate()
        {
            if(!mCapNhatKetQua && Content == UPDATE)
            {
                btnUpdateEnable = false;
            }
            else
            {
                btnUpdateEnable = true;
            }
        }
        //▲====== #005

        //▼====== #007
        private ObservableCollection<Staff> _PerformStaffs;
        public ObservableCollection<Staff> PerformStaffs
        {
            get { return _PerformStaffs; }
            set
            {
                if (_PerformStaffs != value)
                {
                    _PerformStaffs = value;
                    NotifyOfPropertyChange(() => PerformStaffs);
                }
            }
        }

        private void LoadPerformStaffCollection()
        {
            PerformStaffs = new ObservableCollection<Staff>(Globals.AllStaffs.Where(x => x.RefStaffCategory != null
                                                        && x.RefStaffCategory.V_StaffCatType == Globals.ServerConfigSection.CommonItems.StaffCatTypeBAC_SI
                                                        && (!x.IsStopUsing)
                                                        && x.ListPCLResultParamImpID != null && x.ListPCLResultParamImpID.Contains("|45|")).ToList());
        }

        AxComboBox cboPerformStaff;
        public void cboPerformStaff_Loaded(object sender, SelectionChangedEventArgs e)
        {
            cboPerformStaff = sender as AxComboBox;
            var item = cboPerformStaff.DataContext as PatientPCLLaboratoryResultDetail;
            if (item.PCLExamTypeTestItemID == 0)
            {
                cboPerformStaff.Visibility = Visibility.Hidden;
            }
        }

        public void cboPerformStaff_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (sender != null)
            {
                var itemSender = sender as AxComboBox;
                var item = itemSender.DataContext as PatientPCLLaboratoryResultDetail;
                if (itemSender.SelectedValue != null && item.PerformStaffID != (long)itemSender.SelectedValue)
                {
                    if(item.IsTemporaryBlock)
                    {
                        foreach (var itemResultDetail in allPatientPCLLaboratoryResultDetail.Where(x => x.IsTemporaryBlock))
                        {
                            if (itemResultDetail.PCLSectionID == item.PCLSectionID)
                            {
                                itemResultDetail.PerformStaffID = (long)itemSender.SelectedValue;
                            }
                        }
                    }
                    else
                    {
                        foreach (var itemResultDetail in allPatientPCLLaboratoryResultDetail.Where(x => !x.IsTemporaryBlock))
                        {
                            if (itemResultDetail.PCLSectionID == item.PCLSectionID)
                            {
                                itemResultDetail.PerformStaffID = (long)itemSender.SelectedValue;
                            }
                        }
                    }
                }
            }
        }
        //▲====== #007
        //▼==== #008

        private ObservableCollection<Specimen> _AllSpecimen;
        public ObservableCollection<Specimen> AllSpecimen
        {
            get { return _AllSpecimen; }
            set
            {
                if (_AllSpecimen != value)
                {
                    _AllSpecimen = value;
                    NotifyOfPropertyChange(() => AllSpecimen);
                }
            }
        }

        private void GetAllSpecimen()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllSpecimen(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                List<Specimen> results = contract.EndGetAllSpecimen(asyncResult);
                                if (results != null)
                                {
                                    if (AllSpecimen == null)
                                    {
                                        AllSpecimen = new ObservableCollection<Specimen>();
                                    }
                                    else
                                    {
                                        AllSpecimen.Clear();
                                    }
                                    AllSpecimen = results.ToObservableCollection();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
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
                    MessageBox.Show(ex.Message);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }
        //▲==== #008

        //▼==== #009
        public void BtnPrintBilingual()
        {
            if (PatientPCLReqID <= 0)
            {
                MessageBox.Show(eHCMSResources.Z2375_G1_ChonYCXN);
                return;
            }
            
            if (String.IsNullOrEmpty(curPatientPCLLaboratoryResult.DigitalSignatureResultPath))
            {
                void onInitDlg(ICommonPreviewView proAlloc)
                {
                    proAlloc.PatientID = PatientID;
                    proAlloc.PatientPCLReqID = (int)PatientPCLReqID;
                    proAlloc.V_PCLRequestType = PCLRequestTypeID;
                    proAlloc.FindPatient = PCLRequestTypeID > 0 && PCLRequestTypeID == (long)AllLookupValues.V_PCLRequestType.NOI_TRU ? (int)AllLookupValues.PatientFindBy.NOITRU : (int)AllLookupValues.PatientFindBy.NGOAITRU;
                    proAlloc.StaffName = StaffName;
                    proAlloc.IsBilingual = true;
                    proAlloc.eItem = ReportName.PCLDEPARTMENT_LABORATORY_RESULT;
                }
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
            }
            else
            {
                void onInitDlg(IPDFPreviewView proAlloc)
                {
                    proAlloc.DigitalSignatureResultPath = curPatientPCLLaboratoryResult.DigitalSignatureResultPath;
                }
                GlobalsNAV.ShowDialog<IPDFPreviewView>(onInitDlg);
            }
        }
        //▲==== #009
    }
}
