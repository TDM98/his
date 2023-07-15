using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Common;
using aEMR.Common.Printing;
using System.ComponentModel;
using DevExpress.Xpf.Printing;
using aEMR.ReportModel.ReportModels;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
using System.Collections.Generic;
using aEMR.CommonTasks;
using DevExpress.ReportServer.Printing;
/*
 * 20180830 #001 TTM:
 * 20200520 #002 TTM: BM 0038182: Cho phép ca nội trú in mẫu 12 ngoại trú của ca ngoại trú kèm theo.
 * 20220907 #003 BLQ: Thêm mẫu phiếu công khai dịch vụ
 */
namespace aEMR.TransactionManager.ViewModels
{
    [Export(typeof(ITemp02NoiTru)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class Temp02NoiTruViewModel : Conductor<object>, ITemp02NoiTru
        , IHandle<SelectedRegistrationForTemp02_1>
        , IHandle<SelectedRegistrationForTemp02_2>
        , IHandle<InPatientRegistrationSelectedForInPatientRegistration>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public Temp02NoiTruViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            //form tim kiem
            var searchPatientAndRegVm = Globals.GetViewModel<ISearchPatientAndRegistration>();
            searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_PATIENT_BTN | SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
            searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);
            searchPatientAndRegVm.PatientFindByVisibility = false;
            searchPatientAndRegVm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;
            searchPatientAndRegVm.mTimBN = false;
            searchPatientAndRegVm.LeftModule = LeftModuleActive.DANGKY_NOITRU_MAU02;
            //searchPatientAndRegVm.mThemBN = mDangKyNoiTru_Patient_ThemBN;
            //searchPatientAndRegVm.mTimDangKy = mDangKyNoiTru_Patient_TimDangKy;

            // TxD 06/12/2014: Added the following to allow searching patients of all depts to Tai Vu office can use this View 
            searchPatientAndRegVm.CanSearhRegAllDept = true;
            searchPatientAndRegVm.SearchAdmittedInPtRegOnly = true;

            SearchRegistrationContent = searchPatientAndRegVm;

            ActivateItem(searchPatientAndRegVm);

            var patientInfoVm = Globals.GetViewModel<IPatientSummaryInfoV2>();
            patientInfoVm.mInfo_CapNhatThongTinBN = false;
            patientInfoVm.mInfo_XacNhan = false;
            patientInfoVm.mInfo_XoaThe = false;
            patientInfoVm.mInfo_XemPhongKham = false;

            PatientSummaryInfoContent = patientInfoVm;
            ActivateItem(patientInfoVm);

            DepartmentContent = Globals.GetViewModel<IDepartmentListing>();
            DepartmentContent.AddSelectOneItem = false;
            DepartmentContent.AddSelectedAllItem = true;

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                DepartmentContent.LoadData();
            }
             (DepartmentContent as PropertyChangedBase).PropertyChanged += new PropertyChangedEventHandler(Temp38NoiTruViewModel_PropertyChanged);

            eventAggregator.Subscribe(this);

            Globals.IsAdmission = true;
            //▼====: #003
            DateTime curDateTime = Globals.GetCurServerDateTime();
            curDateTime = curDateTime.AddSeconds(-curDateTime.Second);
            FromDateTime = Globals.GetViewModel<IMinHourDateControl>();
            FromDateTime.DateTime = curDateTime;
            ToDateTime = Globals.GetViewModel<IMinHourDateControl>();
            ToDateTime.DateTime = curDateTime;
            //▲====: #003

        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);

            if (IsPhieuCongKhaiDV)
            {
                var homeVm = Globals.GetViewModel<IHome>();
                IInPatientOutstandingTask ostvm = Globals.GetViewModel<IInPatientOutstandingTask>();
                homeVm.OutstandingTaskContent = ostvm;
                homeVm.IsExpandOST = true;
                ostvm.WhichVM = SetOutStandingTask.TAO_BILL_VP;
            }
            //▲====== #015
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
            var homeVm = Globals.GetViewModel<IHome>();
            homeVm.OutstandingTaskContent = null;
            homeVm.IsExpandOST = false;
        }
        private RefDepartment ObjDept = null;
        void Temp38NoiTruViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                ObjDept = DepartmentContent.SelectedItem;
            }
        }

        private IDepartmentListing _departmentContent;
        public IDepartmentListing DepartmentContent
        {
            get { return _departmentContent; }
            set
            {
                _departmentContent = value;
                NotifyOfPropertyChange(() => DepartmentContent);
            }
        }


        private bool _isTemp02NoiTruNew;
        public bool IsTemp02NoiTruNew
        {
            get { return _isTemp02NoiTruNew; }
            set
            {
                _isTemp02NoiTruNew = value;
                NotifyOfPropertyChange(() => IsTemp02NoiTruNew);
            }
        }
        //▼====: #003
        private bool _IsPhieuCongKhaiDV;
        public bool IsPhieuCongKhaiDV
        {
            get { return _IsPhieuCongKhaiDV; }
            set
            {
                _IsPhieuCongKhaiDV = value;
                NotifyOfPropertyChange(() => IsPhieuCongKhaiDV);
            }
        }
        //▲====: #003
        private ReportName _ReportNameObj = ReportName.REPORT_GENERAL_TEMP02;
        public ReportName ReportNameObj
        {
            get => _ReportNameObj; set
            {
                _ReportNameObj = value;
                NotifyOfPropertyChange(() => ReportNameObj);
            }
        }

        #region propety member
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

        private IPatientSummaryInfoV2 _patientSummaryInfoContent;

        public IPatientSummaryInfoV2 PatientSummaryInfoContent
        {
            get { return _patientSummaryInfoContent; }
            set
            {
                NotifyOfPropertyChange(() => PatientSummaryInfoContent);
                _patientSummaryInfoContent = value;
            }
        }

        private DateTime? _FromDate = Globals.GetCurServerDateTime();
        public DateTime? FromDate
        {
            get { return _FromDate; }
            set
            {
                _FromDate = value;
                NotifyOfPropertyChange(() => FromDate);
            }
        }
        private DateTime? _ToDate = Globals.GetCurServerDateTime();
        public DateTime? ToDate
        {
            get { return _ToDate; }
            set
            {
                _ToDate = value;
                NotifyOfPropertyChange(() => ToDate);
            }
        }

        private string _TitlePage = Globals.PageName;
        public string TitlePage
        {
            get
            {
                return _TitlePage;
            }
            set
            {
                _TitlePage = value;
                NotifyOfPropertyChange(() => TitlePage);
            }
        }
        //▼====: #003
        private IMinHourDateControl _FromDateTime;
        public IMinHourDateControl FromDateTime
        {
            get { return _FromDateTime; }
            set
            {
                _FromDateTime = value;
                NotifyOfPropertyChange(() => FromDateTime);
            }
        }
        private IMinHourDateControl _ToDateTime;
        public IMinHourDateControl ToDateTime
        {
            get { return _ToDateTime; }
            set
            {
                _ToDateTime = value;
                NotifyOfPropertyChange(() => ToDateTime);
            }
        }
        //▲====: #003
        #endregion

        //public void btnPreview()
        //{
        //    if (CurPatientRegistration == null)
        //    {
        //        Globals.ShowMessage(eHCMSResources.K0300_G1_ChonDK,eHCMSResources.G0442_G1_TBao);
        //        return;
        //    }
        //    if (FromDate == null || ToDate==null)
        //    {
        //        Globals.ShowMessage("Vui lòng chọn ngày tháng cần xem báo cáo", eHCMSResources.G0442_G1_TBao);
        //        return;
        //    }

        //    if (AxHelper.CompareDate(FromDate.GetValueOrDefault(), ToDate.GetValueOrDefault()) == 1 || FromDate.GetValueOrDefault().Year <= DateTime.Now.Year - 50 || ToDate.GetValueOrDefault().Year <= DateTime.Now.Year - 50)
        //    {
        //        MessageBox.Show("Lỗi : Ngày không hợp lệ");
        //        return;
        //    }

        //    var proAlloc = Globals.GetViewModel<ICommonPreviewView>();
        //    proAlloc.ID = CurPatientRegistration.PtRegistrationID;
        //    proAlloc.FromDate = FromDate.GetValueOrDefault();
        //    proAlloc.ToDate = ToDate.GetValueOrDefault();
        //    proAlloc.ViewByDate = ViewByDate;
        //    if (ObjDept != null && ObjDept.DeptID > 0)
        //    {
        //        proAlloc.DeptID = ObjDept.DeptID;
        //        proAlloc.DeptName = ObjDept.DeptName;
        //    }
        //    else
        //    {
        //        proAlloc.DeptID = 0;
        //        proAlloc.DeptName = "";
        //    }
        //    proAlloc.eItem = ReportName.TEMP38aNoiTru;

        //    var instance = proAlloc as Conductor<object>;
        //    Globals.ShowDialog(instance, (o) => { });
        //}

        public void btnPreview()
        {
            DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer theParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
            if (CurPatientRegistration == null)
            {
                Globals.ShowMessage(eHCMSResources.K0300_G1_ChonDK, eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (FromDate == null || ToDate == null)
            {
                Globals.ShowMessage(eHCMSResources.K0364_G1_ChonNgThCanXemBC, eHCMSResources.G0442_G1_TBao);
                return;
            }
            
            if (eHCMS.Services.Core.AxHelper.CompareDate(FromDate.GetValueOrDefault(), ToDate.GetValueOrDefault()) == 1 || FromDate.GetValueOrDefault().Year <= DateTime.Now.Year - 50 || ToDate.GetValueOrDefault().Year <= DateTime.Now.Year - 50)
            {
                MessageBox.Show(string.Format("{0}: {1}", eHCMSResources.T0074_G1_I, eHCMSResources.A0793_G1_Msg_InfoNgKhHopLe));
                return;
            }

            if (ReportNameObj == ReportName.TEMP12_TONGHOP)
            {
                ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_Temp12_TongHop").PreviewModel;
                theParams["parHospitalAdress"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalAddress;
                theParams["RegistrationType"].Value = (long)CurPatientRegistration.V_RegistrationType;
                if (CurPatientRegistration.AdmissionInfo != null && CurPatientRegistration.AdmissionInfo.IsTreatmentCOVID)
                {
                    theParams["IsPatientCOVID"].Value = CurPatientRegistration.AdmissionInfo.IsTreatmentCOVID;
                }
            }
            else if (ReportNameObj == ReportName.TEMP12_KHOAPHONG)
            {
                ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_Temp12_KhoaPhong").PreviewModel;
                theParams["parHospitalAdress"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalAddress;
                theParams["RegistrationType"].Value = (long)CurPatientRegistration.V_RegistrationType;
            }
            else if (ReportNameObj == ReportName.TEMP12)
            {
                ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_Temp12").PreviewModel;
                theParams["parHospitalAdress"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalAddress;
            }
            else if (ReportNameObj == ReportName.Temp12_NoiBo_TV)
            {
                ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_Temp12_NonHI").PreviewModel;
                theParams["parHospitalAdress"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalAddress;
            }
            else if (IsTemp02NoiTruNew)
            {
                /*▼====: #001*/
                //ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_Temp02NoiTruNew").PreviewModel;
                if (CurPatientRegistration.Applied02Version1 == 1)
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_Temp02NoiTruNew_V1").PreviewModel;
                else
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_Temp02NoiTruNew").PreviewModel;
                /*▲====: #001*/
            }
            else
            {
                ReportModel = new TransactionsTemplate38NoiTru().PreviewModel;
            }
            theParams["PtRegistrationID"].Value = (int)CurPatientRegistration.PtRegistrationID;
            theParams["FromDate"].Value = FromDate.GetValueOrDefault();
            theParams["ToDate"].Value = ToDate.GetValueOrDefault();
            theParams["ViewByDate"].Value = ViewByDate;
            theParams["StaffName"].Value = Globals.LoggedUserAccount != null && Globals.LoggedUserAccount.Staff != null ? Globals.LoggedUserAccount.Staff.FullName : "";
            theParams["parHospitalName"].Value = Globals.FirstCharToUpper(Globals.ServerConfigSection.CommonItems.ReportHospitalName.ToLower());
            theParams["parDepartmentOfHealth"].Value = Globals.FirstCharToUpper(Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth.ToLower());
            // TxD 15/03/2015: Fixed the problem of the Department AutoComplete is showing a Dept being selected but ObjDept has not been initialized yet
            //                 thus it's as if all Depts are being selected
            if (ObjDept == null && DepartmentContent != null)
            {
                ObjDept = DepartmentContent.SelectedItem;
            }

            if (ObjDept != null && ObjDept.DeptID > 0)
            {
                theParams["DeptID"].Value = (int)ObjDept.DeptID;
                theParams["DeptName"].Value = ObjDept.DeptName;
            }
            else
            {
                theParams["DeptID"].Value = 0;
                theParams["DeptName"].Value = "";
            }

            // ReportModel.AutoShowParametersPanel = false;
            ReportModel.CreateDocument(theParams);
            //▼===== #002: 20200521: Boss nói khi người ta xem mẫu 02 thì popup luôn mẫu 01 (Nếu có) để họ không quên.
            //                       Đây là yêu cầu của Bác sĩ Vũ.
            if (CurPatientRegistration.OutPtRegistrationID > 0 && Globals.ServerConfigSection.CommonItems.ReportTwoRegistrationSameTime)
            {
                btnPreview_Temp12OutPt();
            }
            //▲===== #002
        }
        public void btnPreviewPhieuCongKhaiDV()
        {
            //Globals.ShowMessage("Chức năng đang hoàn thiện", eHCMSResources.G0442_G1_TBao);
            //return;
            DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer theParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
            if (CurPatientRegistration == null)
            {
                Globals.ShowMessage(eHCMSResources.K0300_G1_ChonDK, eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (FromDateTime.DateTime == null 
                || ToDateTime.DateTime == null 
                || FromDateTime.DateTime > ToDateTime.DateTime
                || (ToDateTime.DateTime - FromDateTime.DateTime).Value.TotalHours > 24)
            {
                Globals.ShowMessage(eHCMSResources.K0364_G1_ChonNgThCanXemBC+ Environment.NewLine+"Chỉ được chọn thời gian trong 24 giờ", eHCMSResources.G0442_G1_TBao);
                return;
            }

            //if (eHCMS.Services.Core.AxHelper.CompareDate(FromDate.GetValueOrDefault(), ToDate.GetValueOrDefault()) == 1 || FromDate.GetValueOrDefault().Year <= DateTime.Now.Year - 50 || ToDate.GetValueOrDefault().Year <= DateTime.Now.Year - 50)
            //{
            //    MessageBox.Show(string.Format("{0}: {1}", eHCMSResources.T0074_G1_I, eHCMSResources.A0793_G1_Msg_InfoNgKhHopLe));
            //    return;
            //}

            ReportModel = null;
            ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptPhieuCongKhaiDV").PreviewModel;
            
            theParams["PtRegistrationID"].Value = CurPatientRegistration.PtRegistrationID;
            theParams["FromDate"].Value = FromDateTime.DateTime;
            theParams["ToDate"].Value = ToDateTime.DateTime;
            theParams["parHospitalAdress"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalAddress;
            theParams["StaffID"].Value = Globals.LoggedUserAccount != null && Globals.LoggedUserAccount.Staff != null ? Globals.LoggedUserAccount.Staff.StaffID : Convert.ToInt64(0);
            theParams["parHospitalName"].Value = Globals.FirstCharToUpper(Globals.ServerConfigSection.CommonItems.ReportHospitalName.ToLower());
            theParams["parDepartmentOfHealth"].Value = Globals.FirstCharToUpper(Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth.ToLower());
          
            if (ObjDept == null && DepartmentContent != null)
            {
                ObjDept = DepartmentContent.SelectedItem;
            }

            if (ObjDept != null && ObjDept.DeptID > 0)
            {
                theParams["DeptID"].Value = ObjDept.DeptID;
                theParams["DeptName"].Value = ObjDept.DeptName;
            }
            else
            {
                theParams["DeptID"].Value = Convert.ToInt64(0);
                theParams["DeptName"].Value = "";
            }


            ReportModel.CreateDocument(theParams);
            ////▼===== #002: 20200521: Boss nói khi người ta xem mẫu 02 thì popup luôn mẫu 01 (Nếu có) để họ không quên.
            ////                       Đây là yêu cầu của Bác sĩ Vũ.
            //if (CurPatientRegistration.OutPtRegistrationID > 0 && Globals.ServerConfigSection.CommonItems.ReportTwoRegistrationSameTime)
            //{
            //    btnPreview_Temp12OutPt();
            //}
            ////▲===== #002
        }


        private void btnPrint(long ID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ReportServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetTemplate38aInPdfFormat(ID, 0, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetTemplate38aInPdfFormat(asyncResult);
                            var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, results, ActiveXPrintType.ByteArray);
                            Globals.EventAggregator.Publish(results);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });
            t.Start();
        }

        //public void lnkPrint_Click(object sender, RoutedEventArgs e)
        //{
        //    HyperlinkButton linkButton = e.OriginalSource as HyperlinkButton;
        //    if ((long)linkButton.CommandParameter > 0)
        //    {
        //        btnPrint((long)linkButton.CommandParameter);
        //    }
        //}


        private PatientRegistration _CurPatientRegistration;
        public PatientRegistration CurPatientRegistration
        {
            get { return _CurPatientRegistration; }
            set
            {
                _CurPatientRegistration = value;
                NotifyOfPropertyChange(() => CurPatientRegistration);
            }
        }

        private void SetCurPatientRegistration(PatientRegistration item)
        {
            ReportModel = new RemoteDocumentSource();

            CurPatientRegistration = item;

            PatientSummaryInfoContent.CurrentPatient = CurPatientRegistration.Patient;

            //PatientSummaryInfoContent.ConfirmedHiItem = CurPatientRegistration.Patient.CurrentHealthInsurance;
            //PatientSummaryInfoContent.HiBenefit = CurPatientRegistration.PtInsuranceBenefit;
            PatientSummaryInfoContent.SetPatientHISumInfo(CurPatientRegistration.PtHISumInfo);

            if (CurPatientRegistration != null && CurPatientRegistration.AdmissionDate != null)
            {
                FromDate = CurPatientRegistration.AdmissionDate;
            }
            else
            {
                FromDate = Globals.GetCurServerDateTime();
            }
        }


        public void Handle(SelectedRegistrationForTemp02_1 message)
        {
            if (message != null && this.IsActive)
            {
                SetCurPatientRegistration(message.Item);
            }
        }

        public void Handle(SelectedRegistrationForTemp02_2 message)
        {
            if (message != null && this.IsActive)
            {
                SetCurPatientRegistration(message.Item);
            }
        }
        //▼====== #001: Sau khi gọi hàm lấy lại thông tin bệnh nhân thì show popup đã có giá trị đầy đủ.
        public IEnumerator<IResult> OpenRegistration(long regID)
        {
            LoadRegistrationSwitch LoadRegisSwitch = new LoadRegistrationSwitch();
            LoadRegisSwitch.IsGetAdmissionInfo = true;
            var loadRegTask = new LoadRegistrationInfo_InPtTask(regID, (int)AllLookupValues.V_FindPatientType.NOI_TRU, LoadRegisSwitch);
            yield return loadRegTask;
            CurPatientRegistration = loadRegTask.Registration;     
            GetAllInPatientBillingInvoices();
        }
        //▲====== #001
        private bool _ViewByDate = false;
        public bool ViewByDate
        {
            get { return _ViewByDate; }
            set
            {
                _ViewByDate = value;
                NotifyOfPropertyChange(() => ViewByDate);
            }
        }

        private RemoteDocumentSource _reportModel;
        public RemoteDocumentSource ReportModel
        {
            get { return _reportModel; }
            set
            {
                _reportModel = value;
                NotifyOfPropertyChange(() => ReportModel);
            }
        }
      
        public void GetAllInPatientBillingInvoices()
        {
            if (CurPatientRegistration == null || CurPatientRegistration.PtRegistrationID <= 0)
            {
                MessageBox.Show(eHCMSResources.Z0083_G1_KhongTimThayDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            // TxD 15/03/2015: Fixed the problem of the Department AutoComplete is showing a Dept being selected but ObjDept has not been initialized yet
            //                 thus it's as if all Depts are being selected
            if (ObjDept == null && DepartmentContent != null)
            {
                ObjDept = DepartmentContent.SelectedItem;
            }

            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllInPatientBillingInvoices(CurPatientRegistration.PtRegistrationID, ObjDept != null && ObjDept.DeptID > 0 ? ObjDept.DeptID : 0, (long)AllLookupValues.RegistrationType.NOI_TRU
                       , Globals.DispatchCallback((asyncResult) =>
                       {
                           try
                           {
                               var inv = contract.EndGetAllInPatientBillingInvoices(asyncResult);

                               Action<IInPatientBillingInvoiceListingNew> onInitDlg = delegate (IInPatientBillingInvoiceListingNew vm)
                               {
                                   vm.ShowEditColumn = false;
                                   vm.ShowInfoColumn = true;
                                   vm.ShowRecalcHiColumn = false;
                                   vm.ShowRecalcHiWithPriceListColumn = false;
                                   vm.bShowTotalPrice = true;
                                   vm.BillingInvoices = inv.ToObservableCollection();
                                   //▼====== #001
                                   vm.HIBenefit = CurPatientRegistration.PtInsuranceBenefit;
                                   //▲====== #001
                                   vm.CurentRegistration = CurPatientRegistration;
                               };
                               GlobalsNAV.ShowDialog<IInPatientBillingInvoiceListingNew>(onInitDlg);
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

        public void btnBillingInvoices()
        {
            if (CurPatientRegistration == null || CurPatientRegistration.PtRegistrationID <= 0)
            {
                MessageBox.Show(eHCMSResources.K0300_G1_ChonDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            //▼====== #001 Thay đổi phương thức thực hiện việc show popup chi tiết bill lên vì hiện tại chi tiết bill này không có thông tin bhyt.
            //GetAllInPatientBillingInvoices();
            Coroutine.BeginExecute(OpenRegistration(CurPatientRegistration.PtRegistrationID));
            //▲====== #001

        }

        //▼===== #002
        public void btnPreview_Temp12OutPt()
        {
            Action<IReportDocumentPreview> onInitDlg = delegate (IReportDocumentPreview proAlloc)
            {
                proAlloc.ID = CurPatientRegistration.OutPtRegistrationID;
                proAlloc.eItem = ReportName.TEMP12;
                if (Globals.ServerConfigSection.CommonItems.ShowLoginNameOnReport38)
                {
                    proAlloc.StaffFullName = Globals.LoggedUserAccount.Staff.FullName;
                }
                else
                {
                    proAlloc.StaffFullName = "";
                }
            };
            GlobalsNAV.ShowDialog<IReportDocumentPreview>(onInitDlg, null, false, true, new Size(1500, 1000));
        }
        //▲===== #002
        public void Handle(InPatientRegistrationSelectedForInPatientRegistration message)
        {
            if (message != null && message.Source != null)
            {
                SetCurPatientRegistration(message.Source);
                //Coroutine.BeginExecute(OpenRegistration(message.Source.PtRegistrationID));
                //OpenRegistration(message.Source.PtRegistrationID);
            }
        }
    }
}