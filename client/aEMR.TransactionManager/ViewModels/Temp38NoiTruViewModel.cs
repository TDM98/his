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
using aEMR.Common.Printing;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.ComponentModel;
using aEMR.ReportModel.ReportModels;
using DevExpress.ReportServer.Printing;

namespace aEMR.TransactionManager.ViewModels
{
     [Export(typeof(ITemp38NoiTru)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class Temp38NoiTruViewModel : Conductor<object>, ITemp38NoiTru
         , IHandle<PatientSelectedGoToKhamBenh_InPt<PatientRegistration>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public Temp38NoiTruViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
         {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            authorization();
             //form tim kiem
             var searchPatientAndRegVm = Globals.GetViewModel<ISearchPatientAndRegistration>();
             searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_PATIENT_BTN | SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
             searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);
             searchPatientAndRegVm.PatientFindByVisibility = false;
             searchPatientAndRegVm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;
             searchPatientAndRegVm.mTimBN = false;
             searchPatientAndRegVm.SearchAdmittedInPtRegOnly = true;

             // TxD 31/03/2015 Added to allow search all depts regardless of user
             searchPatientAndRegVm.CanSearhRegAllDept = true;

             //searchPatientAndRegVm.mThemBN = mDangKyNoiTru_Patient_ThemBN;
             //searchPatientAndRegVm.mTimDangKy = mDangKyNoiTru_Patient_TimDangKy;

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

            //if (!DesignerProperties.IsInDesignTool)
            bool designTime = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());
            if (!designTime)
            {
                 DepartmentContent.LoadData();
             }
             (DepartmentContent as PropertyChangedBase).PropertyChanged += new PropertyChangedEventHandler(Temp38NoiTruViewModel_PropertyChanged);
             //Globals.EventAggregator.Subscribe(this);

             Globals.IsAdmission = true;
         }

         public void authorization()
         {
             if (!Globals.isAccountCheck)
             {
                 return;
             }
             else
             {
                 mViewReport = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTemp02_BV_NoiTru, (int)oTransaction_ManagementEx.mViewAndPrint, (int)ePermission.mView);
             }
         }

         private bool _mViewReport = true;

         public bool mViewReport
         {
             get
             {
                 return _mViewReport;
             }
             set
             {
                 if (_mViewReport == value)
                     return;
                 _mViewReport = value;
                 NotifyOfPropertyChange(() => mViewReport);
             }
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
                 _patientSummaryInfoContent = value;
                 NotifyOfPropertyChange(() => PatientSummaryInfoContent);
             }
         }

         private DateTime? _FromDate = DateTime.Now;
         public DateTime? FromDate
         {
             get { return _FromDate; }
             set
             {
                 _FromDate = value;
                 NotifyOfPropertyChange(()=>FromDate);
             }
         }
         private DateTime? _ToDate = DateTime.Now;
         public DateTime? ToDate
         {
             get { return _ToDate; }
             set
             {
                 _ToDate = value;
                 NotifyOfPropertyChange(() => ToDate);
             }
         }
         #endregion

         public void btnPreview()
         {
             if (CurPatientRegistration == null)
             {
                 Globals.ShowMessage(eHCMSResources.K0300_G1_ChonDK,eHCMSResources.G0442_G1_TBao);
                 return;
             }
             if (FromDate == null || ToDate==null)
             {
                 Globals.ShowMessage(eHCMSResources.K0364_G1_ChonNgThCanXemBC, eHCMSResources.G0442_G1_TBao);
                 return;
             }

             if (eHCMS.Services.Core.AxHelper.CompareDate(FromDate.GetValueOrDefault(), ToDate.GetValueOrDefault()) == 1 || FromDate.GetValueOrDefault().Year <= DateTime.Now.Year - 50 || ToDate.GetValueOrDefault().Year <= DateTime.Now.Year - 50)
             {
                 MessageBox.Show(eHCMSResources.A0793_G1_Msg_InfoNgKhHopLe);
                 return;
             }
            DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
            ReportModel = new TransactionsTemplate38NoiTru().PreviewModel;
            rParams["PtRegistrationID"].Value = (int)CurPatientRegistration.PtRegistrationID;
            rParams["FromDate"].Value = FromDate.GetValueOrDefault();
            rParams["ToDate"].Value = ToDate.GetValueOrDefault();
            rParams["ViewByDate"].Value = ViewByDate;
            if (ObjDept != null && ObjDept.DeptID > 0)
            {
                rParams["DeptID"].Value = (int)ObjDept.DeptID;
                rParams["DeptName"].Value = ObjDept.DeptName;
            }
            else
            {
                rParams["DeptID"].Value = 0;
                rParams["DeptName"].Value = "";
            }

            rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
            rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
            // ReportModel.AutoShowParametersPanel = false;
            ReportModel.CreateDocument(rParams);         
         }

         private void btnPrint(long ID)
         {
             this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
             var t = new Thread(() =>
             {
                 try
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
                                  this.HideBusyIndicator();
                              }
                          }), null);
                     }
                 }
                 catch (Exception ex)
                 {
                     this.HideBusyIndicator();
                     Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
                 NotifyOfPropertyChange(()=>CurPatientRegistration);
             }
         }

         public void Handle(PatientSelectedGoToKhamBenh_InPt<PatientRegistration> message)
         {
             if (message != null && this.IsActive)
             {
                 ReportModel = new RemoteDocumentSource();

                 CurPatientRegistration = message.Item;

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
         }

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

             this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);

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

                             //var vm = Globals.GetViewModel<IInPatientBillingInvoiceListingNew>();
                             //vm.ShowEditColumn = false;
                             //vm.ShowInfoColumn = true;
                             //vm.ShowRecalcHiColumn = false;
                             //vm.bShowTotalPrice = true;
                             //vm.BillingInvoices = inv.ToObservableCollection();

                             //Globals.ShowDialog(vm as Conductor<object>);

                             Action<IInPatientBillingInvoiceListingNew> onInitDlg = (vm) =>
                             {
                                 vm.ShowEditColumn = false;
                                 vm.ShowInfoColumn = true;
                                 vm.ShowRecalcHiColumn = false;
                                 vm.ShowRecalcHiWithPriceListColumn = false;
                                 vm.bShowTotalPrice = true;
                                 vm.BillingInvoices = inv.ToObservableCollection();
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

             GetAllInPatientBillingInvoices();
         }
    }
}
