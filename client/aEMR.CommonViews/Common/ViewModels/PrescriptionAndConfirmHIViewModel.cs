using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.Common.ConfigurationManager.Printer;
using aEMR.Common.Printing;
using aEMR.CommonTasks;
using aEMR.Controls;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using DataEntities;
using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using OrderDTO = DataEntities.OrderDTO;
/*
* 20191119 #001 TTM: BM 0019605: Fix lỗi load đăng ký không xóa toa thuốc cũ của đăng ký trước đó.
* 20191220 #002 TBL: BM 0019586: Fix lỗi PatientSummaryInfo không hiển thị Mã thẻ BHYT, Quyền lợi, Địa chỉ
* 20200619 #003 TTM: BM 0039267: Thêm chức năng điều trị ngoại trú (Bán thuốc).
* 20220608 #004 DatTB: Thêm in report hướng dẫn sử dụng theo toa thuốc
* 20220823 #005 DatTB: Chỉnh sửa màn hình chờ nhận thuốc
* 20220929 #006 DatTB:
* + Thêm textbox tìm bệnh nhân theo tên/mã/stt
* + Thêm đối tượng ưu tiên
* 20221007 #007 DatTB: Thêm nút in trực tiếp phiếu soạn thuốc
* 20230329 #008 DatTB: Thay đổi list soạn thuốc trước thành list phân trang
* 20230503 #009 DatTB: Thêm biến showBusy cho các function để hiện/ẩn busy theo các trường hợp
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IPrescriptionAndConfirmHI)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PrescriptionAndConfirmHIViewModel : ViewModelBase, IPrescriptionAndConfirmHI
        , IHandle<ItemSelected<PatientRegistration>>
        , IHandle<LoadDataCompleted<PatientRegistration>>
        , IHandle<DiagnosisTreatmentSelectedEvent<DiagnosisTreatment>>
        , IHandle<RegisObjAndPrescriptionLst>
    {
        [ImportingConstructor]
        public PrescriptionAndConfirmHIViewModel(IWindsorContainer aContainer, INavigationService aNavigation, IEventAggregator aEvent)
        {
            FromDate = Globals.ServerDate.Value;
            ToDate = Globals.ServerDate.Value;
            FillCondition();
            IsWaiting = 0;
            PrescriptionCollection = new ObservableCollection<Prescription>();
            ChoNhanThuocData = new ObservableCollection<Prescription>();

            //▼==== #008
            ChoSoanThuocData = new PagedSortableCollectionView<Prescription>();
            ChoSoanThuocData.PageIndex = 0;

            ChoSoanThuocData.OnRefresh += new WeakEventHandler<aEMR.Common.Collections.RefreshEventArgs>(SoanThuoc_OnRefresh).Handler;
            //▲==== #008

            IsNewConfirm = (bool)Globals.IsNewConfirm;
            IsOutPtTreatmentPrescription = Globals.IsOutPtTreatmentPrescription;
            if (Globals.ServerConfigSection.HealthInsurances.UseConfirmRecalcHIOutPt && IsNewConfirm)
            {
                UCPrescriptionView = GlobalsNAV.GetViewModel<IConfirmRecalHiOutPt>();
                if (Globals.ServerConfigSection.CommonItems.EnableHIStore && !Globals.ServerConfigSection.CommonItems.MixedHIPharmacyStores)
                {
                    ((IConfirmRecalHiOutPt)UCPrescriptionView).IsHIOutPt = true;
                }
                ((IConfirmRecalHiOutPt)UCPrescriptionView).IsConfirmHIView = true;
                ActivateItem(((IConfirmRecalHiOutPt)UCPrescriptionView));
            }
            else
            {
                if (!IsOutPtTreatmentPrescription)
                {
                    UCPrescriptionView = GlobalsNAV.GetViewModel<IPrescription>();
                    if (Globals.ServerConfigSection.CommonItems.EnableHIStore && !Globals.ServerConfigSection.CommonItems.MixedHIPharmacyStores)
                    {
                        ((IPrescription)UCPrescriptionView).IsHIOutPt = true;
                    }
                    ((IPrescription)UCPrescriptionView).IsConfirmHIView = true;
                    ActivateItem(((IPrescription)UCPrescriptionView));
                }
                else
                {
                    UCPrescriptionView = GlobalsNAV.GetViewModel<IPrescriptionOutPtTreatment>();
                    if (Globals.ServerConfigSection.CommonItems.EnableHIStore && !Globals.ServerConfigSection.CommonItems.MixedHIPharmacyStores)
                    {
                        ((IPrescriptionOutPtTreatment)UCPrescriptionView).IsHIOutPt = true;
                    }
                    ((IPrescriptionOutPtTreatment)UCPrescriptionView).IsConfirmHIView = true;
                    ActivateItem(((IPrescriptionOutPtTreatment)UCPrescriptionView));
                }
            }
            UCSearchPatientAndRegistrationView = GlobalsNAV.GetViewModel<ISearchPatientAndRegistration>();
            UCSearchPatientAndRegistrationView.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
            UCSearchPatientAndRegistrationView.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);
            UCSearchPatientAndRegistrationView.mTimBN = false;
            UCSearchPatientAndRegistrationView.mThemBN = false;
            UCSearchPatientAndRegistrationView.mTimDangKy = true;
            UCSearchPatientAndRegistrationView.PatientFindByVisibility = false;
            UCSearchPatientAndRegistrationView.PatientFindBy = AllLookupValues.PatientFindBy.NGOAITRU;
            UCSearchPatientAndRegistrationView.IsSearchGoToConfirmHI = true;

            //▼===== #003: Để lúc tìm kiếm phân biệt được đây là màn hình bán thuốc theo toa thì sẽ mở rộng thời gian tìm kiếm lên 30 ngày.
            UCSearchPatientAndRegistrationView.IsSearchOutPtTreatmentPre = IsOutPtTreatmentPrescription;

            //TTM: Thêm điều kiện nếu là màn hình điều trị ngoại trú thì khi tìm kiếm bên duyệt toa sẽ tìm cả đăng ký và toa thuốc cùng lúc
            if (Globals.ServerConfigSection.HealthInsurances.UseConfirmRecalcHIOutPt && (IsNewConfirm || IsOutPtTreatmentPrescription))
            {
                UCSearchPatientAndRegistrationView.IsSearchRegisAndGetPrescript = true;
            }
            //▲=====
            UCPatientSummaryInfoContent = GlobalsNAV.GetViewModel<IPatientSummaryInfoV2>();

            UCePrescription = GlobalsNAV.GetViewModel<IePrescriptionSimple>();
            this.ActivateItem(UCePrescription);

            UCSimplePay = Globals.GetViewModel<ISimplePay>();
            UCSimplePay.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.QUAY_XAC_NHAN;
            UCSimplePay.FormMode = PaymentFormMode.VIEW;
            UCSimplePay.RegistrationDetails = null;
            UCSimplePay.PclRequests = null;
            UCSimplePay.PayNewService = true;
            UCSimplePay.IsConfirmHIView = true;
            UCSimplePay.IsViewOnly = true;

            //20190927 TNHX: Sau Khi bỏ Global thì chỗ này chết phải chỉnh như thế này
            Registration_DataStorage = new Registration_DataStorage
            {
                CurrentPatientRegistration = null,
                CurrentPatientRegistrationDetail = null
            };
            //Registration_DataStorage.PatientServiceRecordCollection = null;
            //if (Globals.PatientFindBy_ForConsultation == null || Globals.PatientFindBy_ForConsultation.Value != AllLookupValues.PatientFindBy.NGOAITRU)
            //{
            //    Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NGOAITRU;
            //}
        }

        #region Properties
        public object UCPrescriptionView { get; set; }
        public ISearchPatientAndRegistration UCSearchPatientAndRegistrationView { get; set; }
        public IPatientSummaryInfoV2 UCPatientSummaryInfoContent { get; set; }
        public PrescriptionSearchCriteria SearchCriteria { get; set; }
        public IePrescriptionSimple UCePrescription { get; set; }
        public ISimplePay UCSimplePay { get; set; }
        private TabControl gTCMain { get; set; }
        private ObservableCollection<Prescription> _PrescriptionCollection;
        private ObservableCollection<Prescription> _ChoNhanThuocData;
        private PatientRegistration _PatientRegistrationObj;
        private Prescription _PrescriptionObj;
        private Prescription _CurrentChoNhanThuoc;
        public Prescription CurrentChoNhanThuoc
        {
            get => _CurrentChoNhanThuoc; set
            {
                _CurrentChoNhanThuoc = value;
                NotifyOfPropertyChange(() => CurrentChoNhanThuoc);
            }
        }
        public ObservableCollection<Prescription> ChoNhanThuocData
        {
            get => _PrescriptionCollection; set
            {
                _PrescriptionCollection = value;
                NotifyOfPropertyChange(() => ChoNhanThuocData);
            }
        }
        public ObservableCollection<Prescription> PrescriptionCollection
        {
            get => _ChoNhanThuocData; set
            {
                _ChoNhanThuocData = value;
                NotifyOfPropertyChange(() => PrescriptionCollection);
            }
        }

        public PatientRegistration PatientRegistrationObj
        {
            get => _PatientRegistrationObj; set
            {
                _PatientRegistrationObj = value;
                NotifyOfPropertyChange(() => PatientRegistrationObj);
            }
        }
        public bool IsConfirmPrescriptionOnly
        {
            get => UCPrescriptionView != null && Globals.ServerConfigSection.HealthInsurances.UseConfirmRecalcHIOutPt && IsNewConfirm ?
                ((IConfirmRecalHiOutPt)UCPrescriptionView).IsConfirmPrescriptionOnly : !IsOutPtTreatmentPrescription ?
                ((IPrescription)UCPrescriptionView).IsConfirmPrescriptionOnly : ((IPrescriptionOutPtTreatment)UCPrescriptionView).IsConfirmPrescriptionOnly;
            set
            {
                if (UCPrescriptionView != null)
                {
                    if (Globals.ServerConfigSection.HealthInsurances.UseConfirmRecalcHIOutPt && IsNewConfirm)
                    {
                        ((IConfirmRecalHiOutPt)UCPrescriptionView).IsConfirmPrescriptionOnly = value;
                    }
                    else
                    {

                        if (!IsOutPtTreatmentPrescription)
                        {
                            ((IPrescription)UCPrescriptionView).IsConfirmPrescriptionOnly = value;
                        }
                        else
                        {
                            ((IPrescriptionOutPtTreatment)UCPrescriptionView).IsConfirmPrescriptionOnly = value;
                        }
                    }
                }
                NotifyOfPropertyChange(() => IsConfirmPrescriptionOnly);
            }
        }
        public Prescription PrescriptionObj
        {
            get => _PrescriptionObj; set
            {
                _PrescriptionObj = value;
                NotifyOfPropertyChange(() => PrescriptionObj);
            }
        }

        public bool IsServicePatient
        {
            get
            {
                if (UCPrescriptionView != null)
                {
                    return Globals.ServerConfigSection.HealthInsurances.UseConfirmRecalcHIOutPt && IsNewConfirm ?
                        ((IConfirmRecalHiOutPt)UCPrescriptionView).IsServicePatient : !IsOutPtTreatmentPrescription ?
                        ((IPrescription)UCPrescriptionView).IsServicePatient : ((IPrescriptionOutPtTreatment)UCPrescriptionView).IsServicePatient;
                }
                return false;
            }
            set
            {
                if (UCPrescriptionView != null)
                {
                    if (Globals.ServerConfigSection.HealthInsurances.UseConfirmRecalcHIOutPt && IsNewConfirm)
                    {
                        ((IConfirmRecalHiOutPt)UCPrescriptionView).IsServicePatient = value;
                    }
                    else
                    {
                        if (!IsOutPtTreatmentPrescription)
                        {
                            ((IPrescription)UCPrescriptionView).IsServicePatient = value;
                        }
                        else
                        {
                            ((IPrescriptionOutPtTreatment)UCPrescriptionView).IsServicePatient = value;
                        }
                    }
                }
                NotifyOfPropertyChange(() => IsServicePatient);
            }
        }
        private IRegistration_DataStorage _Registration_DataStorage;
        public IRegistration_DataStorage Registration_DataStorage
        {
            get
            {
                return _Registration_DataStorage;
            }
            set
            {
                if (_Registration_DataStorage == value)
                {
                    return;
                }
                _Registration_DataStorage = value;
                NotifyOfPropertyChange(() => Registration_DataStorage);
                if (Globals.ServerConfigSection.HealthInsurances.UseConfirmRecalcHIOutPt && IsNewConfirm)
                {
                    ((IConfirmRecalHiOutPt)UCPrescriptionView).Registration_DataStorage = Registration_DataStorage;
                }
                else
                {
                    if (!IsOutPtTreatmentPrescription)
                    {
                        ((IPrescription)UCPrescriptionView).Registration_DataStorage = Registration_DataStorage;
                    }
                    else
                    {
                        ((IPrescriptionOutPtTreatment)UCPrescriptionView).Registration_DataStorage = Registration_DataStorage;
                    }
                }
                UCePrescription.Registration_DataStorage = Registration_DataStorage;
            }
        }
        #endregion
        #region Handles
        public void Handle(ItemSelected<PatientRegistration> message)
        {
            if (GetView() == null || message == null || message.Item == null || message.Item.PtRegistrationID == 0)
            {
                return;
            }
            if (message.Item.HisID.GetValueOrDefault(0) == 0)
            {
                //▼===== #003: Nếu trong màn hình điều trị ngoại trú thì cho phép thao tác với bệnh nhân dịch vụ.
                if (!IsOutPtTreatmentPrescription)
                {
                    IsServicePatient = true;
                    LoadAllTabDetails(null);
                }
                else
                {
                    LoadDetailsForOutTreatmentPrescription(message.Item);
                    Coroutine.BeginExecute(DoOpenRegistration(message.Item.PtRegistrationID), null, (o, e) => { });
                }
                //▲===== #003
            }
            else
            {

                if (!IsOutPtTreatmentPrescription)
                {
                    IsServicePatient = false;
                    LoadAllTabDetails(message.Item);
                    //▼===== #002
                    Coroutine.BeginExecute(DoOpenRegistration(message.Item.PtRegistrationID), null, (o, e) => { });
                    //▲===== #002
                }
                //▼===== #003: Nếu như tìm kiếm ở duyệt toa mà tìm được 2 giá trị trở lên thì gọi hàm LoadDetailsForOutTreatmentPrescription, còn 1 thì gọi hàm tìm kiếm của Boss.
                else
                {
                    IsServicePatient = false;
                    LoadDetailsForOutTreatmentPrescription(message.Item);
                    Coroutine.BeginExecute(DoOpenRegistration(message.Item.PtRegistrationID), null, (o, e) => { });
                }
                //▲===== #003
            }
        }

        public void Handle(RegisObjAndPrescriptionLst msg)
        {
            if (GetView() == null || msg == null || msg.RegisObj == null || msg.RegisObj.PtRegistrationID == 0)
            {
                return;
            }
            if (msg.RegisObj.HisID.GetValueOrDefault(0) == 0)
            {
                IsServicePatient = true;
            }
            else
            {
                IsServicePatient = false;
                LoadAllTabDetails_New(msg.RegisObj, msg.PrescriptLst);
            }
            // QMS Service
            //if (GlobalsNAV.IsQMSEnable())
            //{
            //    UpdateOrder(new RequestOrderDTO
            //    {
            //        orderStatus = "CALLING",
            //        patientId = msg.RegisObj.Patient.PatientID,
            //        refLocationId = 6,//(long)UCSummary.Registration_DataStorage.CurrentPatientRegistrationDetail.DeptLocID,
            //        refDeptId = 2//(long)UCSummary.Registration_DataStorage.CurrentPatientRegistration.DeptID
            //    });
            //}
        }

        public void Handle(LoadDataCompleted<PatientRegistration> message)
        {
            if (message != null && message.Obj != null)
            {
                PatientRegistrationObj = message.Obj;
                UCSimplePay.Registration = PatientRegistrationObj;
                UCSimplePay.RegistrationDetails = PatientRegistrationObj.PatientRegistrationDetails == null ? null : PatientRegistrationObj.PatientRegistrationDetails.Where(x => x.PaidTime == null && (x.RecordState == RecordState.UNCHANGED || x.RecordState == RecordState.ADDED)).ToList();
                UCSimplePay.PclRequests = PatientRegistrationObj.PCLRequests == null ? null : PatientRegistrationObj.PCLRequests.Where(x => x.PaidTime == null && x.RecordState == RecordState.UNCHANGED).ToList();
                UCSimplePay.DrugInvoices = UCSimplePay.DrugInvoices == null ? null : PatientRegistrationObj.DrugInvoices.Where(x => x.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.SAVE).ToList();
                UCSimplePay.StartCalculating();
            }
        }
        public void Handle(DiagnosisTreatmentSelectedEvent<DiagnosisTreatment> message)
        {
            if (message != null && PatientRegistrationObj != null && PrescriptionObj != null && PrescriptionObj.PrescriptionIssueHistory != null
                && PrescriptionObj.PrescriptionIssueHistory.PtRegDetailID.HasValue)
            {
                GetPtServiceRecordForKhamBenhAndLoadPrescriptionInfo(PatientRegistrationObj.PtRegistrationID, PatientRegistrationObj.V_RegistrationType, PrescriptionObj.PrescriptionIssueHistory.PtRegDetailID.Value, false);
            }
        }
        #endregion
        #region Events
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
        public void gvPrescriptionList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PrescriptionObj = null;
            if (!(sender is DataGrid) || (sender as DataGrid).SelectedItem == null || !((sender as DataGrid).SelectedItem is Prescription)
                || ((sender as DataGrid).SelectedItem as Prescription).PrescriptionIssueHistory == null
                || !((sender as DataGrid).SelectedItem as Prescription).PrescriptionIssueHistory.PtRegDetailID.HasValue)
            {
                return;
            }
            Registration_DataStorage.CurrentPatientRegistration = PatientRegistrationObj;
            Registration_DataStorage.CurrentPatientRegistration.Patient = PatientRegistrationObj.Patient;
            PrescriptionObj = ((sender as DataGrid).SelectedItem as Prescription).DeepCopy();
            Registration_DataStorage.CurrentPatientRegistrationDetail = new PatientRegistrationDetail { PtRegDetailID = PrescriptionObj.PrescriptionIssueHistory.PtRegDetailID.Value };
            //Registration_DataStorage.PatientServiceRecordCollection = new ObservableCollection<PatientServiceRecord> { new PatientServiceRecord { PrescriptionIssueHistories = new ObservableCollection<PrescriptionIssueHistory> { new PrescriptionIssueHistory { Prescription = PrescriptionObj } }
            //    , DiagnosisTreatments = new ObservableCollection<DiagnosisTreatment> { new DiagnosisTreatment { ServiceRecID = PrescriptionObj.ServiceRecID } } } };
            Globals.isConsultationStateEdit = true;
            GetPtServiceRecordForKhamBenhAndLoadPrescriptionInfo(PatientRegistrationObj.PtRegistrationID, PatientRegistrationObj.V_RegistrationType, PrescriptionObj.PrescriptionIssueHistory.PtRegDetailID.Value, true);
        }
        public void TCMain_Loaded(object sender, RoutedEventArgs e)
        {
            gTCMain = sender as TabControl;
            if (!IsConfirmPrescriptionOnly)
            {
                TabItem TIConfirmHI = gTCMain.Items.Cast<TabItem>().Where(x => x.Name == "TIConfirmHI").FirstOrDefault();
                if (TIConfirmHI != null)
                {
                    gTCMain.SelectedItem = TIConfirmHI;
                }
            }
        }
        #endregion
        #region Methods
        
        private void GetPtServiceRecordForKhamBenhAndLoadPrescriptionInfo(long aPtRegistrationID, AllLookupValues.RegistrationType aV_RegistrationType, long aPtRegDetailID, bool IsLoadPrescriptionInfoAlso)
        {
            this.ShowBusyIndicator();
            PatientServiceRecord psrSearch = new PatientServiceRecord { PtRegistrationID = aPtRegistrationID, V_RegistrationType = aV_RegistrationType, PtRegDetailID = aPtRegDetailID };
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonUtilsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPatientServiceRecordsGetForKhamBenh(psrSearch,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var psr = contract.EndPatientServiceRecordsGetForKhamBenh(asyncResult);
                                if (psr != null && psr.Count > 0)
                                {
                                    psr.FirstOrDefault().PrescriptionIssueHistories = new ObservableCollection<PrescriptionIssueHistory> { new PrescriptionIssueHistory { Prescription = PrescriptionObj } };
                                }
                                Registration_DataStorage.PatientServiceRecordCollection = new ObservableCollection<PatientServiceRecord>(psr);
                                if (IsLoadPrescriptionInfoAlso)
                                {
                                    UCePrescription.LoadPrescriptionInfo(PrescriptionObj.PrescriptID);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                ClientLoggerHelper.LogError(ex.Message);
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        public void LoadAllTabDetails_New(PatientRegistration aPatientRegistration, IList<Prescription> lstPrescriptions)
        {
            PatientRegistrationObj = aPatientRegistration;
            UCPatientSummaryInfoContent.CurrentPatient = PatientRegistrationObj.Patient;
            UCPatientSummaryInfoContent.CurrentPatientClassification = PatientRegistrationObj.PatientClassification;
            //▼===== 20200822 TTM:  Vì lý do anh Tuấn gửi dữ liệu sang để gán không có dữ liệu cho PtHiSumInfo (Do code chỉ lấy đăng ký, code cũ trước khi sửa
            //                      Lấy thông tin cả thẻ BH nên set đc giá trị cho PtHISumInfo. Nên buộc phải set bằng tay giá trị để hiển thị PatientInfo cho chính xác.
            //                      Fix luôn lỗi khi tìm bệnh nhân mới lên, bệnh nhân cũ có chọn vào toa thuốc để xem thì không clear màn hình.
            if (!string.IsNullOrEmpty(PatientRegistrationObj.HiCardNo) && PatientRegistrationObj.PtInsuranceBenefit > 0)
            {
                PatientRegistrationObj.HealthInsurance = new HealthInsurance();
                PatientRegistrationObj.HealthInsurance.HICardNo = PatientRegistrationObj.HiCardNo;
                NotifyOfPropertyChange(() => PatientRegistrationObj);
            }
            UCePrescription.LatestePrecriptions = new Prescription();
            //▲===== 20200822
            if (lstPrescriptions != null && lstPrescriptions.Count > 0)
            {
                PrescriptionCollection = lstPrescriptions.OrderByDescending(x => x.IsSold).OrderBy(x => x.IsEmptyPrescription).ToObservableCollection();
                if (Globals.ServerConfigSection.HealthInsurances.UseConfirmRecalcHIOutPt && IsNewConfirm)
                {
                    //20191123 TTM: Chuyển dữ liệu đăng ký xuống màn hình xác nhận để sử dụng thay vì load lại dữ liệu khi duyệt toa.
                    //((IConfirmRecalHiOutPt)UCPrescriptionView).PrepareDataForConfirmRegistration(PrescriptionCollection, PatientRegistrationObj);
                    ((IConfirmRecalHiOutPt)UCPrescriptionView).PrepareDataForConfirmRegistration_New(PrescriptionCollection, PatientRegistrationObj);

                }
                else
                {
                    if (IsOutPtTreatmentPrescription)
                    {
                        ((IPrescriptionOutPtTreatment)UCPrescriptionView).PrepareDataForConfirmRegistration_New(PrescriptionCollection, PatientRegistrationObj);
                    }
                    else
                    {
                        PrescriptionCollection = new ObservableCollection<Prescription>();
                        MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    }
                }
            }
            else
            {
                PrescriptionCollection = new ObservableCollection<Prescription>();
                MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }

        }

        public void LoadAllTabDetails(PatientRegistration aPatientRegistration)
        {
            if (aPatientRegistration == null)
            {
                return;
            }
            PatientRegistrationObj = aPatientRegistration;
            //▼===== #001: LoadAllTabDetails được gọi khi load bệnh nhân mới để duyệt toa - xác nhận BHYT. Nên có thể đặt clear data cho contentcontrol toa thuốc ở đây.
            UCePrescription.LatestePrecriptions = new Prescription();
            //▲===== #001
            UCPatientSummaryInfoContent.CurrentPatient = PatientRegistrationObj.Patient;
            SearchCriteria = new PrescriptionSearchCriteria { PtRegistrationID = PatientRegistrationObj.PtRegistrationID };
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSearchPrescription(SearchCriteria, 0, 100, true, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int Total;
                            var results = contract.EndSearchPrescription(out Total, asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                PrescriptionCollection = results.OrderByDescending(x => x.IsSold).OrderBy(x => x.IsEmptyPrescription).ToObservableCollection();
                                if (Globals.ServerConfigSection.HealthInsurances.UseConfirmRecalcHIOutPt && IsNewConfirm)
                                {
                                    //20191123 TTM: Chuyển dữ liệu đăng ký xuống màn hình xác nhận để sử dụng thay vì load lại dữ liệu khi duyệt toa.
                                    ((IConfirmRecalHiOutPt)UCPrescriptionView).LoadPrescriptionDetail(PrescriptionCollection.First());
                                }
                                else
                                {
                                    if (!IsOutPtTreatmentPrescription)
                                    {
                                        ((IPrescription)UCPrescriptionView).LoadPrescriptionDetail(PrescriptionCollection.First());
                                    }
                                    else
                                    {
                                        ((IPrescriptionOutPtTreatment)UCPrescriptionView).LoadPrescriptionDetail(PrescriptionCollection.First());
                                    }
                                }
                            }
                            else
                            {
                                PrescriptionCollection = new ObservableCollection<Prescription>();
                                MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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
        //▼===== #002
        public IEnumerator<IResult> DoOpenRegistration(long regID)
        {
            var loadRegTask = new LoadRegistrationInfoTask(regID, true);
            yield return loadRegTask;

            if (loadRegTask.Registration == null)
            {
                //Thong bao khong load duoc dang ky
                Globals.EventAggregator.Publish(new ShowMessageEvent { Message = eHCMSResources.A0733_G1_Msg_InfoKhTimThayDK });
                Globals.EventAggregator.Publish(new ItemLoaded<PatientRegistration, long> { Item = null, ID = regID });
            }
            else
            {
                PatientRegistrationObj = loadRegTask.Registration;
                if (UCPatientSummaryInfoContent != null)
                {
                    UCPatientSummaryInfoContent.CurrentPatient = PatientRegistrationObj.Patient;
                    UCPatientSummaryInfoContent.CurrentPatientClassification = PatientRegistrationObj.PatientClassification;

                    UCPatientSummaryInfoContent.SetPatientHISumInfo(PatientRegistrationObj.PtHISumInfo);
                }
            }
        }
        //▲===== #002
        #endregion
        private bool _IsNewConfirm = false;
        public bool IsNewConfirm
        {
            get
            {
                return _IsNewConfirm;
            }
            set
            {
                if (_IsNewConfirm != value)
                {
                    _IsNewConfirm = value;
                }
                NotifyOfPropertyChange(() => IsNewConfirm);
            }
        }
        private bool _IsOutPtTreatmentPrescription = false;
        public bool IsOutPtTreatmentPrescription
        {
            get
            {
                return _IsOutPtTreatmentPrescription;
            }
            set
            {
                if (_IsOutPtTreatmentPrescription != value)
                {
                    _IsOutPtTreatmentPrescription = value;
                }
                NotifyOfPropertyChange(() => IsOutPtTreatmentPrescription);
            }
        }
        public void LoadDetailsForOutTreatmentPrescription(PatientRegistration aPatientRegistration)
        {
            if (aPatientRegistration == null)
            {
                return;
            }
            PatientRegistrationObj = aPatientRegistration;
            UCePrescription.LatestePrecriptions = new Prescription();
            UCPatientSummaryInfoContent.CurrentPatient = PatientRegistrationObj.Patient;
            SearchCriteria = new PrescriptionSearchCriteria { PtRegistrationID = PatientRegistrationObj.PtRegistrationID };
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSearchOutPtTreatmentPrescription(SearchCriteria, Globals.GetCurServerDateTime(), 0, 100, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int Total;
                            var results = contract.EndSearchOutPtTreatmentPrescription(out Total, asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                PrescriptionCollection = results.OrderByDescending(x => x.IsSold).OrderBy(x => x.IsEmptyPrescription).ToObservableCollection();
                                ((IPrescriptionOutPtTreatment)UCPrescriptionView).LoadPrescriptionDetail(PrescriptionCollection.First());
                            }
                            else
                            {
                                //▼===== #003:  Khi không tìm thấy toa thuốc nào để bán thì không được báo lỗi và ngưng lại mà phải set CurPatient và CurRegistration
                                //              sang màn hình bán thuốc để người dùng có thể tìm toa thuốc hoặc phiếu xuất để bán lại ...
                                PrescriptionCollection = new ObservableCollection<Prescription>();
                                //MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                ((IPrescriptionOutPtTreatment)UCPrescriptionView).SetCurPatientAndRegistration(PatientRegistrationObj);
                                //▲===== #003
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

        private DateTime _FromDate;
        public DateTime FromDate
        {
            get => _FromDate; set
            {
                _FromDate = value;
                NotifyOfPropertyChange(() => FromDate);
            }
        }

        private DateTime _ToDate;
        public DateTime ToDate
        {
            get => _ToDate; set
            {
                _ToDate = value;
                NotifyOfPropertyChange(() => ToDate);
            }
        }

        private int _IsWaiting;
        public int IsWaiting
        {
            get => _IsWaiting; set
            {
                _IsWaiting = value;
                NotifyOfPropertyChange(() => IsWaiting);
            }
        }

        private ObservableCollection<Condition> _Conditions;
        public ObservableCollection<Condition> Conditions
        {
            get
            {
                return _Conditions;
            }
            set
            {
                if (_Conditions != value)
                {
                    _Conditions = value;
                    NotifyOfPropertyChange(() => Conditions);
                }
            }
        }

        public class Condition
        {
            private readonly string _Text;
            private readonly long _Value;
            public string Text { get { return _Text; } }
            public long Value { get { return _Value; } }
            public Condition(string theText, long theValue)
            {
                _Text = theText;
                _Value = theValue;
            }
        }

        private Condition _CurrentCondition;
        public Condition CurrentCondition
        {
            get
            {
                return _CurrentCondition;
            }
            set
            {
                if (_CurrentCondition != value)
                {
                    _CurrentCondition = value;
                    NotifyOfPropertyChange(() => CurrentCondition);
                }
            }
        }
        private void FillCondition()
        {
            if (Conditions == null)
            {
                Conditions = new ObservableCollection<Condition>();
            }
            else
            {
                Conditions.Clear();
            }

            Conditions.Add(new Condition("Tất cả", 2));
            Conditions.Add(new Condition("Đã soạn thuốc", 1));
            Conditions.Add(new Condition("Chờ soạn thuốc", 0));

            CurrentCondition = Conditions.LastOrDefault();
           
        }
        public void cbxCondition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentCondition != null)
            {
                switch (CurrentCondition.Value)
                {
                    case 0:
                        IsWaiting = 0;
                        break;
                    case 1:
                        IsWaiting = 1;
                        break;
                    case 2:
                        IsWaiting = 2;
                        break;
                }
            }
        }
        
        //public void XacNhanCmd(object sender, object eventArgs)
        //{
        //    CurrentChoNhanThuoc = null;
        //    var elem = sender as FrameworkElement;
        //    CurrentChoNhanThuoc =  elem.DataContext as Prescription;
        //    if (CurrentChoNhanThuoc == null)
        //    {
        //        return;
        //    }
        //    UpdateChoNhanThuoc(true, CurrentChoNhanThuoc.OutwardDrugInvoices[0].CountPrint);
        //}
        //private void InPhieuChoNhanThuoc()
        //{
        //    Action<IReportDocumentPreview> onInitDlg = delegate (IReportDocumentPreview proAlloc)
        //    {
        //        proAlloc.ID = CurrentChoNhanThuoc.OutwardDrugInvoices[0].outiID;
        //        proAlloc.eItem = ReportName.PHARMACY_PHIEUNHANTHUOC_BH;
        //    };
        //    GlobalsNAV.ShowDialog<IReportDocumentPreview>(onInitDlg, null, false, true, null);
        //    UpdateChoNhanThuoc(CurrentChoNhanThuoc.OutwardDrugInvoices[0].IsWaiting, CurrentChoNhanThuoc.OutwardDrugInvoices[0].CountPrint + 1);
        //}
        public void PreviewCmd(object sender, object eventArgs)
        {
            CurrentChoNhanThuoc = null;
            var elem = sender as FrameworkElement;
            CurrentChoNhanThuoc = elem.DataContext as Prescription;
       
            if (CurrentChoNhanThuoc == null)
            {
                return;
            }
            //▼==== #005
            //InPhieuChoNhanThuoc();
            InPhieuChoSoanThuoc();
            //▲==== #005
        }

        public void gvChoNhanThuocList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CurrentChoNhanThuoc = null;
            if (!(sender is DataGrid) || (sender as DataGrid).SelectedItem == null || !((sender as DataGrid).SelectedItem is Prescription))
            {
                return;
            }
           
            CurrentChoNhanThuoc = ((sender as DataGrid).SelectedItem as Prescription).DeepCopy();
            //▼==== #005
            //InPhieuChoNhanThuoc();
            InPhieuChoSoanThuoc();
            //▲==== #005
        }
        public void btRefresh()
        {
            ChoNhanThuocData = null;

            //▼==== #005
            GetChoSoanThuocList(FromDate, ToDate, IsWaiting, sPatientCode, sPatientName, sStoreServiceSeqNum, 0, ChoSoanThuocData.PageSize);  //==== #006, #008
            //GetChoNhanThuocList(FromDate, ToDate, IsWaiting);
            //▲==== #005
        }

        private void GetChoNhanThuocList(DateTime fromDate, DateTime toDate, int IsWaiting)
        {
            this.ShowBusyIndicator();
            //PatientServiceRecord psrSearch = new PatientServiceRecord { PtRegistrationID = aPtRegistrationID, V_RegistrationType = aV_RegistrationType, PtRegDetailID = aPtRegDetailID };
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePrescriptionsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetChoNhanThuocList(fromDate, toDate, IsWaiting,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var psr = contract.EndGetChoNhanThuocList(asyncResult);
                                if (psr != null && psr.Count > 0)
                                {
                                    ChoNhanThuocData = new ObservableCollection<Prescription>();

                                    ChoNhanThuocData = psr;

                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                ClientLoggerHelper.LogError(ex.Message);
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void UpdateChoNhanThuoc(bool isWaiting, int CountPrint)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateChoNhanThuoc(CurrentChoNhanThuoc.OutwardDrugInvoices[0].outiID, isWaiting, CountPrint
                        , Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Result = "";
                            contract.EndUpdateChoNhanThuoc(out Result,asyncResult);

                            if (Result == "1")
                            {
                                GetChoNhanThuocList(FromDate, ToDate, IsWaiting);
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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

            });
            t.Start();
        }

        //▼==== #004
        private void InHuongDanSuDungThuoc()
        {
            Action<IReportDocumentPreview> onInitDlg = delegate (IReportDocumentPreview proAlloc)
            {
                proAlloc.ID = (long)CurrentChoNhanThuoc.PtRegistrationID;
                proAlloc.IsInsurance = true;
                proAlloc.eItem = ReportName.Huong_Dan_Su_Dung_Thuoc;
            };
            GlobalsNAV.ShowDialog<IReportDocumentPreview>(onInitDlg, null, false, true, null);
        }
        public void PreviewHDSDCmd(object sender, object eventArgs)
        {
            CurrentChoNhanThuoc = null;
            var elem = sender as FrameworkElement;
            CurrentChoNhanThuoc = elem.DataContext as Prescription;

            if (CurrentChoNhanThuoc == null)
            {
                return;
            }
            //▼==== #005
            InHuongDanSuDungThuocWithoutView((long)CurrentChoNhanThuoc.PtRegistrationID, true);
            //InHuongDanSuDungThuoc();
            //▲==== #005
        }
        //▲==== #004

        //▼==== #005        
        //▼==== #008
        private void GetChoSoanThuocList(DateTime fromDate, DateTime toDate, int IsWaiting, string sPatientCode, string sPatientName, int sStoreServiceSeqNum, int pageIndex, int pageSize, bool showBusy = true) //==== #006, #009
        {
            if (showBusy)
            {
                this.ShowBusyIndicator();
            }
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePrescriptionsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetChoSoanThuocList(fromDate, toDate, IsWaiting, sPatientCode, sPatientName, sStoreServiceSeqNum, pageIndex, pageSize,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            int totalCount = 0;
                            IList<Prescription> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = contract.EndGetChoSoanThuocList(out totalCount, asyncResult);
                                bOK = true;
                                
                                ChoSoanThuocData.Clear();
                                if (bOK)
                                {
                                    if (allItems != null)
                                    {
                                        ChoSoanThuocData.TotalItemCount = totalCount;
                                        foreach (var item in allItems)
                                        {
                                            ChoSoanThuocData.Add(item);
                                        }

                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                ClientLoggerHelper.LogError(ex.Message);
                            }
                            finally
                            {
                                if (showBusy)
                                {
                                    this.HideBusyIndicator();
                                }
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    if (showBusy)
                    {
                        this.HideBusyIndicator();
                    }
                }
            });
            t.Start();
        }
        //▲==== #008

        private void UpdateChoSoanThuoc(bool isWaiting, int CountPrint, bool showBusy = true) //#009
        {
            if (CurrentChoNhanThuoc == null || CurrentChoNhanThuoc.PtRegistrationID == 0)
            {
                return;
            }
            if (showBusy)
            {
                this.ShowBusyIndicator();
            }
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateChoSoanThuoc((long)CurrentChoNhanThuoc.PtRegistrationID, isWaiting, CountPrint
                    , Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Result = "";
                            contract.EndUpdateChoSoanThuoc(out Result, asyncResult);

                            if (Result == "1")
                            {
                                GetChoSoanThuocList(FromDate, ToDate, IsWaiting, sPatientCode, sPatientName, sStoreServiceSeqNum, ChoSoanThuocData.PageIndex, ChoSoanThuocData.PageSize, showBusy); //==== #006, #009
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            if (showBusy)
                            {
                                this.ShowBusyIndicator();
                            }
                        }
                        finally
                        {
                            if (showBusy)
                            {
                                this.HideBusyIndicator();
                            }
                        }
                    }), null);

                }

            });
            t.Start();
        }

        private void InPhieuChoSoanThuoc()
        {
            Action<IReportDocumentPreview> onInitDlg = delegate (IReportDocumentPreview proAlloc)
            {
                proAlloc.ID = (long)CurrentChoNhanThuoc.PtRegistrationID;
                proAlloc.eItem = ReportName.PhieuSoanThuocPaging;
            };
            GlobalsNAV.ShowDialog<IReportDocumentPreview>(onInitDlg, null, false, true, null);
            UpdateChoSoanThuoc(true, CurrentChoNhanThuoc.CountPrint + 1);
        }

        public void InHuongDanSuDungThuocWithoutView(long PtRegistrationID, bool IsInsurance, bool showBusy = true) //#009
        {
            var printerConfigManager = new PrinterConfigurationManager();
            var allAssignedPrinterTypes = printerConfigManager.GetAllAssignedPrinterType();

            if (showBusy)
            {
                this.ShowBusyIndicator(eHCMSResources.Z1544_G1_DangIn);
            }
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ReportServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginInHuongDanSuDungThuocWithoutView(PtRegistrationID, IsInsurance, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var data = contract.EndInHuongDanSuDungThuocWithoutView(asyncResult);
                                var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_NHIET_HDDT, data, ActiveXPrintType.ByteArray);
                                Globals.EventAggregator.Publish(printEvt);
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                MessageBox.Show(eHCMSResources.Z3263_G1_Msg_InfoKhTheLayDataDeInHDSDT);
                            }
                            finally
                            {
                                if (showBusy)
                                {
                                    this.HideBusyIndicator();
                                }
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    if (showBusy)
                    {
                        this.HideBusyIndicator();
                    }
                }
            });

            t.Start();
        }
        //▲==== #005

        //▼==== #006
        private string _sPatientCode;
        public string sPatientCode
        {
            get
            {
                return _sPatientCode;
            }
            set
            {
                if (_sPatientCode != value)
                {
                    _sPatientCode = value;
                    NotifyOfPropertyChange(() => sPatientCode);
                }
            }
        }

        private string _sPatientName;
        public string sPatientName
        {
            get
            {
                return _sPatientName;
            }
            set
            {
                if (_sPatientName != value)
                {
                    _sPatientName = value;
                    NotifyOfPropertyChange(() => sPatientName);
                }
            }
        }

        private int _sStoreServiceSeqNum;
        public int sStoreServiceSeqNum
        {
            get
            {
                return _sStoreServiceSeqNum;
            }
            set
            {
                if (_sStoreServiceSeqNum != value)
                {
                    _sStoreServiceSeqNum = value;
                    NotifyOfPropertyChange(() => sStoreServiceSeqNum);
                }
            }
        }

        private AxTextBox CtrtbSoTT;

        public void tbSoTT_KeyUp(object sender, KeyEventArgs e)
        {
            string v = (sender as TextBox).Text;
            if (!string.IsNullOrEmpty(v))
            {
                int num = 0;
                int.TryParse(v, out num);

                ((System.Windows.Controls.TextBox)(sender)).Text = num.ToString();//show ra giao diện
            }

        }

        public void tbSoTT_Loaded(object sender, RoutedEventArgs e)
        {
            CtrtbSoTT = sender as AxTextBox;
            CtrtbSoTT.Focus();
        }
        //▲==== #006

        //▼==== #007

        public void PrintAllCmd(object sender, object eventArgs)
        {
            CurrentChoNhanThuoc = null;
            var elem = sender as FrameworkElement;
            CurrentChoNhanThuoc = elem.DataContext as Prescription;

            if (CurrentChoNhanThuoc == null)
            {
                return;
            }

            //▼==== #009
            InPhieuChoSoanThuocWithoutView((long)CurrentChoNhanThuoc.PtRegistrationID, false);
            InHuongDanSuDungThuocWithoutView((long)CurrentChoNhanThuoc.PtRegistrationID, true, false);
            //▲==== #009
        }

        public void InPhieuChoSoanThuocWithoutView(long PtRegistrationID, bool showBusy = true) //#009
        {
            var printerConfigManager = new PrinterConfigurationManager();
            var allAssignedPrinterTypes = printerConfigManager.GetAllAssignedPrinterType();

            if (showBusy)
            {
                this.ShowBusyIndicator(eHCMSResources.Z1544_G1_DangIn);
            }
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ReportServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginInPhieuChoSoanThuocWithoutView(PtRegistrationID, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var data = contract.EndInPhieuChoSoanThuocWithoutView(asyncResult);
                                var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_SOAN_THUOC, data, ActiveXPrintType.ByteArray, "A5");
                                Globals.EventAggregator.Publish(printEvt);
                                UpdateChoSoanThuoc(true, CurrentChoNhanThuoc.CountPrint + 1, showBusy); //#009
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                MessageBox.Show(eHCMSResources.Z3263_G1_Msg_InfoKhTheLayDataDeInHDSDT);
                            }
                            finally
                            {
                                if (showBusy)
                                {
                                    this.HideBusyIndicator();
                                }
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    if (showBusy)
                    {
                        this.HideBusyIndicator();
                    }
                }
            });

            t.Start();
        }
        //▲==== #007
        
        //▼==== #008
        private PagedSortableCollectionView<Prescription> _ChoSoanThuocData;
        public PagedSortableCollectionView<Prescription> ChoSoanThuocData
        {
            get => _ChoSoanThuocData; set
            {
                _ChoSoanThuocData = value;
                NotifyOfPropertyChange(() => ChoSoanThuocData);
            }
        }

        private Prescription _CurrentChoSoanThuoc;
        public Prescription CurrentChoSoanThuoc
        {
            get => _CurrentChoSoanThuoc; set
            {
                _CurrentChoSoanThuoc = value;
                NotifyOfPropertyChange(() => CurrentChoSoanThuoc);
            }
        }
        
        ReadOnlyDataGrid gGvSoanThuoc;

        public void gridSoanThuoc_Loaded(object sender, RoutedEventArgs e)
        {
            gGvSoanThuoc = sender as ReadOnlyDataGrid;
        }

        public void SoanThuoc_OnRefresh(object sender, aEMR.Common.Collections.RefreshEventArgs e)
        {
            GetChoSoanThuocList(FromDate, ToDate, IsWaiting, sPatientCode, sPatientName, sStoreServiceSeqNum, ChoSoanThuocData.PageIndex, ChoSoanThuocData.PageSize);  //==== #006
        }
        //▲==== #008
    }
}
