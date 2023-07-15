using aEMR.Common;
using aEMR.CommonTasks;
using aEMR.Controls;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using Ax.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using eHCMSLanguage;
using Microsoft.Win32;
using Service.Core.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.Common.Collections;
using aEMR.ServiceClient;
using aEMR.DataContracts;
using System.ComponentModel;
using aEMR.Common.BaseModel;
/*
* 20200801 #001 TTM: BM 0019686:   Do aEMR sử dụng DesignerProperties từ .NETFramework v4.7.1 của thư viện PresentationFramework.dll không phải từ Silverlight v5.0 của System.Windows.dll
*                                  như silverlight nên biến IsInDesignTool không có, nên phải kiểm tra khác.
*/
namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IQuotation)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class QuotationViewModel : ViewModelBase, IQuotation
        , IHandle<DoubleClick>
        , IHandle<DoubleClickAddReqLAB>
        , IHandle<ItemSelected<RefMedicalServiceItem>>
        , IHandle<ItemSelected<PCLExamType>>
        , IHandle<ResultFound<Patient>>
        , IHandle<ItemRemoved<MedRegItemBase, IInPatientBillingInvoiceDetailsListing>>
    {
        #region Properties
        private IInPatientSelectService _InPatientSelectServiceContent;
        public IInPatientSelectService InPatientSelectServiceContent
        {
            get
            {
                return _InPatientSelectServiceContent;
            }
            set
            {
                if (_InPatientSelectServiceContent == value)
                {
                    return;
                }
                _InPatientSelectServiceContent = value;
                NotifyOfPropertyChange(() => InPatientSelectServiceContent);
            }
        }
        private IInPatientSelectPcl _SelectPCLContent;
        public IInPatientSelectPcl SelectPCLContent
        {
            get
            {
                return _SelectPCLContent;
            }
            set
            {
                if (_SelectPCLContent == value)
                {
                    return;
                }
                _SelectPCLContent = value;
                NotifyOfPropertyChange(() => SelectPCLContent);
            }
        }
        private IInPatientSelectPclLAB _SelectPCLContentLAB;
        public IInPatientSelectPclLAB SelectPCLContentLAB
        {
            get
            {
                return _SelectPCLContentLAB;
            }
            set
            {
                if (_SelectPCLContentLAB == value)
                {
                    return;
                }
                _SelectPCLContentLAB = value;
                NotifyOfPropertyChange(() => SelectPCLContentLAB);
            }
        }
        private IInPatientBillingInvoiceDetailsListing _EditingInvoiceDetailsContent;
        public IInPatientBillingInvoiceDetailsListing EditingInvoiceDetailsContent
        {
            get
            {
                return _EditingInvoiceDetailsContent;
            }
            set
            {
                if (_EditingInvoiceDetailsContent == value)
                {
                    return;
                }
                _EditingInvoiceDetailsContent = value;
                NotifyOfPropertyChange(() => EditingInvoiceDetailsContent);
            }
        }
        private int? _serviceQty;
        public int? ServiceQty
        {
            get
            {
                return _serviceQty;
            }
            set
            {
                _serviceQty = value;
                NotifyOfPropertyChange(() => ServiceQty);
            }
        }
        public string EditingBillingInvoiceTitle
        {
            get
            {
                if (CurrentInPatientBillingInvoice == null)
                {
                    return string.Empty;
                }
                if (CurrentInPatientBillingInvoice.InPatientBillingInvID > 0)
                {
                    return string.Format("{0} ", eHCMSResources.Z0152_G1_CNhatBill) + CurrentInPatientBillingInvoice.BillingInvNum;
                }
                else
                {
                    return eHCMSResources.Z0014_G1_ThemBillMoi;
                }
            }
        }
        public InPatientBillingInvoice CurrentInPatientBillingInvoice
        {
            get
            {
                return EditingInvoiceDetailsContent == null ? null : EditingInvoiceDetailsContent.BillingInvoice;
            }
            set
            {
                if (EditingInvoiceDetailsContent.BillingInvoice != value)
                {
                    EditingInvoiceDetailsContent.BillingInvoice = value;
                    NotifyOfPropertyChange(() => CurrentInPatientBillingInvoice);
                    NotifyOfPropertyChange(() => EditingBillingInvoiceTitle);
                    EditingInvoiceDetailsContent.ResetView();
                }
            }
        }
        private int? _PclQty;
        public int? PclQty
        {
            get
            {
                return _PclQty;
            }
            set
            {
                _PclQty = value;
                NotifyOfPropertyChange(() => PclQty);
            }
        }
        private int? _PclQtyLAB;
        public int? PclQtyLAB
        {
            get
            {
                return _PclQtyLAB;
            }
            set
            {
                _PclQtyLAB = value;
                NotifyOfPropertyChange(() => PclQtyLAB);
            }
        }
        private PatientRegistration _CurrentPatientRegistration;
        public PatientRegistration CurrentPatientRegistration
        {
            get
            {
                return _CurrentPatientRegistration;
            }
            set
            {
                if (_CurrentPatientRegistration == value)
                {
                    return;
                }
                _CurrentPatientRegistration = value;
                NotifyOfPropertyChange(() => CurrentPatientRegistration);
            }
        }
        private double? _CurrentPtInsuranceBenefit = 0.0d;
        public double? CurrentPtInsuranceBenefit
        {
            get
            {
                return _CurrentPtInsuranceBenefit;
            }
            set
            {
                if (_CurrentPtInsuranceBenefit == value)
                {
                    return;
                }
                _CurrentPtInsuranceBenefit = value;
                if (_CurrentPtInsuranceBenefit < 0d || _CurrentPtInsuranceBenefit > 1.0d)
                {
                    _CurrentPtInsuranceBenefit = 0d;
                }
                NotifyOfPropertyChange(() => CurrentPtInsuranceBenefit);
                if (CurrentPtInsuranceBenefit < 0.8d)
                {
                    CurrentPatientRegistration.IsCrossRegion = true;
                }
                else
                {
                    CurrentPatientRegistration.IsCrossRegion = false;
                }
            }
        }
        private DateTime Now;
        private long DefaultStoreIDForQuotation;
        private int? _MedItemQty = 1;
        public int? MedItemQty
        {
            get
            {
                return _MedItemQty;
            }
            set
            {
                _MedItemQty = value;
                NotifyOfPropertyChange(() => MedItemQty);
            }
        }
        private RefGenMedProductDetails _SelectedMedProduct;
        public RefGenMedProductDetails SelectedMedProduct
        {
            get
            {
                return _SelectedMedProduct;
            }
            set
            {
                if (_SelectedMedProduct == value)
                {
                    return;
                }
                _SelectedMedProduct = value;
                NotifyOfPropertyChange(() => SelectedMedProduct);
            }
        }
        private List<RefGenMedProductDetails> RefGenMedProductDetailsList = new List<RefGenMedProductDetails>();
        private ISearchPatientAndRegistration _SearchRegistrationContent;
        public ISearchPatientAndRegistration SearchRegistrationContent
        {
            get
            {
                return _SearchRegistrationContent;
            }
            set
            {
                if (_SearchRegistrationContent == value)
                {
                    return;
                }
                _SearchRegistrationContent = value;
                NotifyOfPropertyChange(() => SearchRegistrationContent);
            }
        }

        private IPatientSummaryInfoV2 _PatientSummaryInfoContent;
        public IPatientSummaryInfoV2 PatientSummaryInfoContent
        {
            get { return _PatientSummaryInfoContent; }
            set
            {
                if (_PatientSummaryInfoContent == value)
                {
                    return;
                }
                _PatientSummaryInfoContent = value;
                NotifyOfPropertyChange(() => PatientSummaryInfoContent);
            }
        }
        private bool _IsActionCanTrigger = true;
        public bool IsActionCanTrigger
        {
            get
            {
                return _IsActionCanTrigger;
            }
            set
            {
                if (_IsActionCanTrigger == value)
                {
                    return;
                }
                _IsActionCanTrigger = value;
                NotifyOfPropertyChange(() => IsActionCanTrigger);
            }
        }
        private short _CurrentViewCase = (short)ViewCase.Template;
        public short CurrentViewCase
        {
            get
            {
                return _CurrentViewCase;
            }
            set
            {
                if (_CurrentViewCase == value)
                {
                    return;
                }
                _CurrentViewCase = value;
                NotifyOfPropertyChange(() => CurrentViewCase);
                NotifyOfPropertyChange(() => IsQuotationViewCase);
            }
        }
        public bool IsQuotationViewCase
        {
            get
            {
                return CurrentViewCase == (short)ViewCase.Quotation;
            }
        }
        #endregion
        #region Events
        public QuotationViewModel()
        {
            SearchRegistrationContent = Globals.GetViewModel<ISearchPatientAndRegistration>();
            SearchRegistrationContent.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_PATIENT_BTN);
            SearchRegistrationContent.SetDefaultButton(SearchRegistrationButtons.SEARCH_PATIENT);
            SearchRegistrationContent.PatientFindByVisibility = false;
            SearchRegistrationContent.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;
            SearchRegistrationContent.mTimBN = true;
            SearchRegistrationContent.mThemBN = true;
            SearchRegistrationContent.mTimDangKy = false;
            SearchRegistrationContent.SearchAdmittedInPtRegOnly = true;
            ActivateItem(SearchRegistrationContent);

            PatientSummaryInfoContent = Globals.GetViewModel<IPatientSummaryInfoV2>();
            PatientSummaryInfoContent.mInfo_CapNhatThongTinBN = true;
            PatientSummaryInfoContent.mInfo_XacNhan = false;
            PatientSummaryInfoContent.mInfo_XoaThe = false;
            PatientSummaryInfoContent.mInfo_XemPhongKham = false;
            PatientSummaryInfoContent.CanConfirmHi = false;
            PatientSummaryInfoContent.DisplayButtons = false;
            ActivateItem(PatientSummaryInfoContent);

            Now = Globals.GetCurServerDateTime();
            DefaultStoreIDForQuotation = Globals.ServerConfigSection.CommonItems.DefaultStoreIDForQuotation;
            Coroutine.BeginExecute(DoGetStoreToSell());
            InPatientSelectServiceContent = Globals.GetViewModel<IInPatientSelectService>();
            SelectPCLContent = Globals.GetViewModel<IInPatientSelectPcl>();
            SelectPCLContentLAB = Globals.GetViewModel<IInPatientSelectPclLAB>();
            EditingInvoiceDetailsContent = Globals.GetViewModel<IInPatientBillingInvoiceDetailsListing>();
            EditingInvoiceDetailsContent.CanEditOnGrid = true;
            EditingInvoiceDetailsContent.IsHighTechServiceBill = true;
            EditingInvoiceDetailsContent.IsNewCreateBill = true;
            EditingInvoiceDetailsContent.ShowInPackageColumn = true;
            EditingInvoiceDetailsContent.IsQuotationView = true;

            //▼===== #001
            //if (!DesignerProperties.IsInDesignTool)
            //{
            //    InPatientSelectServiceContent.GetServiceTypes();
            //}
            bool designTime = DesignerProperties.GetIsInDesignMode(new DependencyObject());
            if (!designTime)
            {
                InPatientSelectServiceContent.GetServiceTypes();
            }
            //▲===== #001
            CurrentPatientRegistration = new PatientRegistration { PtInsuranceBenefit = CurrentPtInsuranceBenefit, HealthInsurance = new HealthInsurance { HIPatientBenefit = CurrentPtInsuranceBenefit, ValidDateFrom = new DateTime(2010, 01, 01), ValidDateTo = Now.Date.AddMonths(1) } };
            CurrentInPatientBillingInvoice = new InPatientBillingInvoice { IsHighTechServiceBill = true, InvDate = Now };
            EditingInvoiceDetailsContent.ResetView();
            EditingInvoiceDetailsContent.CurentRegistration = CurrentPatientRegistration;
            EditingInvoiceDetailsContent.HIBenefit = CurrentPtInsuranceBenefit;
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            ActivateItem(InPatientSelectServiceContent);
            ActivateItem(SelectPCLContent);
            ActivateItem(SelectPCLContentLAB);
            ActivateItem(EditingInvoiceDetailsContent);
            Globals.EventAggregator.Subscribe(this);
            if (CurrentViewCase == (short)ViewCase.Template)
            {
                EditingInvoiceDetailsContent.CanEditHICount = false;
            }
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            DeactivateItem(InPatientSelectServiceContent, close);
            DeactivateItem(SelectPCLContent, close);
            DeactivateItem(SelectPCLContentLAB, close);
            DeactivateItem(EditingInvoiceDetailsContent, close);
            Globals.EventAggregator.Unsubscribe(this);
        }
        public void AddServiceCmd()
        {
            ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults;
            if (!ValidateServiceItem(out validationResults))
            {
                Globals.EventAggregator.Publish(new ValidateFailedEvent() { ValidationResults = validationResults });
                return;
            }
            AddGenMedService(InPatientSelectServiceContent.MedServiceItem, ServiceQty.Value, Now.Date);
        }
        public void AddPclExamTypeCmd()
        {
            ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults;
            if (!ValidatePclItem(out validationResults))
            {
                Globals.EventAggregator.Publish(new ValidateFailedEvent() { ValidationResults = validationResults });
                return;
            }
            bool used = true;
            CheckAndAddPCL(SelectPCLContent.SelectedPCLExamType, PclQty.Value, Now.Date, used);
        }
        public void AddPclExamTypeCmdLAB()
        {
            ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults;
            if (!ValidatePclItemLAB(out validationResults))
            {
                Globals.EventAggregator.Publish(new ValidateFailedEvent() { ValidationResults = validationResults });
                return;
            }
            bool used = true;
            CheckAndAddPCL(SelectPCLContentLAB.SelectedPCLExamType, PclQtyLAB.Value, Now.Date, used);
        }
        public void AddAllPclExamTypeCmdLAB()
        {
            ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults;
            if (!ValidatePclItemLAB(out validationResults))
            {
                Globals.EventAggregator.Publish(new ValidateFailedEvent() { ValidationResults = validationResults });
                return;
            }
            bool used = true;
            CheckAndAddAllPCL(SelectPCLContentLAB.PclExamTypes, PclQtyLAB.Value, Now.Date, used);
        }
        public void DrugControl_OnPopulating(object source, PopulatingEventArgs e)
        {
            SearchRefGenMedProductDetails(source, e.Parameter, false);
        }
        public void AddMedItemCmd()
        {
            ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults;
            if (!ValidateMedItem(out validationResults))
            {
                Globals.EventAggregator.Publish(new ValidateFailedEvent() { ValidationResults = validationResults });
                return;
            }
            if (SelectedMedProduct.MaxQtyHIAllowItem > 1 && MedItemQty.Value > 1)
            {
                for (int i = 0; i < MedItemQty.Value; i++)
                {
                    AddGenMedItem(SelectedMedProduct.DeepCopy(), 1, Now);
                }
            }
            else
            {
                AddGenMedItem(SelectedMedProduct.DeepCopy(), MedItemQty.Value, Now);
            }
        }
        public void DrugControl_OnDropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if ((sender as AxAutoComplete).SelectedItem != null)
            {
                SelectedMedProduct = (sender as AxAutoComplete).SelectedItem as RefGenMedProductDetails;
            }
        }
        public void txtInsuranceBenefit_LostFocus(object sender, RoutedEventArgs e)
        {
            CurrentPatientRegistration.PtInsuranceBenefit = CurrentPtInsuranceBenefit;
            CurrentPatientRegistration.HealthInsurance.HIPatientBenefit = CurrentPtInsuranceBenefit;
            CurrentInPatientBillingInvoice.HIBenefit = CurrentPtInsuranceBenefit;
            EditingInvoiceDetailsContent.HIBenefit = CurrentPtInsuranceBenefit;
            EditingInvoiceDetailsContent.RecacucateAllItemValues();
        }
        public void SaveQuotationCmd()
        {
            CallSaveQuotationCmd(null);
        }
        public void SearchQuotationCmd()
        {
            IQuotationCollection mDialogView = Globals.GetViewModel<IQuotationCollection>();
            mDialogView.CurrentViewCase = (short)ViewCase.Template;
            GlobalsNAV.ShowDialog_V3<IQuotationCollection>(mDialogView, null, null, false, false);
            if (mDialogView.IsHasSelected)
            {
                CallSelectedOldQuotation(mDialogView.SelectedQuotation);
            }
        }
        public void SearchPatientQuotationCmd()
        {
            IQuotationCollection mDialogView = Globals.GetViewModel<IQuotationCollection>();
            mDialogView.CurrentViewCase = (short)ViewCase.Quotation;
            GlobalsNAV.ShowDialog_V3<IQuotationCollection>(mDialogView, null, null, false, false);
            if (mDialogView.IsHasSelected)
            {
                CallSelectedOldQuotation(mDialogView.SelectedQuotation);
            }
        }
        public void CreatePatientQuotationCmd()
        {
            if (CurrentInPatientBillingInvoice == null || string.IsNullOrEmpty(CurrentInPatientBillingInvoice.QuotationTitle))
            {
                Globals.ShowMessage(eHCMSResources.Z2879_G1_NhapTieuDe, eHCMSResources.G0442_G1_TBao);
                return;
            }
            else if (PatientSummaryInfoContent.CurrentPatient == null || PatientSummaryInfoContent.CurrentPatient.PatientID == 0)
            {
                Globals.ShowMessage(eHCMSResources.K0294_G1_ChonBN, eHCMSResources.G0442_G1_TBao);
                return;
            }
            CallSaveQuotationCmd(PatientSummaryInfoContent.CurrentPatient.PatientID);
        }
        public void GenerateNewQuotationCmd()
        {
            var CurrentNotApplyMaxHIPay = CurrentInPatientBillingInvoice.NotApplyMaxHIPay;
            CurrentInPatientBillingInvoice = new InPatientBillingInvoice { IsHighTechServiceBill = true, InvDate = Now, NotApplyMaxHIPay = CurrentNotApplyMaxHIPay };
            EditingInvoiceDetailsContent.ResetView();
            EditingInvoiceDetailsContent.CurentRegistration = CurrentPatientRegistration;
            CurrentPtInsuranceBenefit = 0;
            EditingInvoiceDetailsContent.HIBenefit = CurrentPtInsuranceBenefit;
            PatientSummaryInfoContent.CurrentPatient = new Patient();
            PatientSummaryInfoContent.SetPatientHISumInfo(null);
            IsActionCanTrigger = true;
        }
        public void ExportExcelCmd()
        {
            if (CurrentInPatientBillingInvoice == null ||
                CurrentInPatientBillingInvoice.InPatientBillingInvID == 0 ||
                PatientSummaryInfoContent.CurrentPatient == null ||
                PatientSummaryInfoContent.CurrentPatient.PatientID == 0)
            {
            }
            SaveFileDialog CurrentFileDialog = new SaveFileDialog()
            {
                DefaultExt = ".csv",
                Filter = "CSV file (*.csv)|*.csv",
                FilterIndex = 1
            };
            if (CurrentFileDialog.ShowDialog() != true)
            {
                return;
            }
            ReportParameters RptParameters = new ReportParameters();
            RptParameters.ReportType = ReportType.BAOCAO_TONGHOP;
            RptParameters.reportName = ReportName.PatientQuotation;
            RptParameters.SearchID = CurrentInPatientBillingInvoice.InPatientBillingInvID;
            RptParameters.IsExportToCSV = true;
            ExportToExcelGeneric.Action(RptParameters, CurrentFileDialog, this);
        }
        public void ckbNotApplyMaxHIPay_CheckChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            txtInsuranceBenefit_LostFocus(null, null);
        }
        #endregion
        #region Methods
        private bool ValidateServiceItem(out ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> result)
        {
            result = new ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult>();
            if (InPatientSelectServiceContent.MedServiceItem == null || InPatientSelectServiceContent.MedServiceItem.MedServiceID < 1)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0162_G1_HayChonDV, new string[] { "MedServiceItem" });
                result.Add(item);
            }
            if (ServiceQty.GetValueOrDefault(0) < 1)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0163_G1_NhapGTriSLgDV, new string[] { "ServiceQty" });
                result.Add(item);
            }
            if (result.Count > 0)
            {
                return false;
            }
            return true;
        }
        public void AddGenMedService(object medicalService, int qty, DateTime createdDate)
        {
            RefMedicalServiceItem curItem = medicalService as RefMedicalServiceItem;
            if (curItem.RefMedicalServiceType.V_RefMedicalServiceTypes != (long)AllLookupValues.V_RefMedicalServiceTypes.CANLAMSANG)
            {
                PatientRegistrationDetail item;
                item = new PatientRegistrationDetail();
                item.StaffID = Globals.LoggedUserAccount.StaffID;
                item.CreatedDate = createdDate;
                item.MedProductType = AllLookupValues.MedProductType.KCB;
                item.RefMedicalServiceItem = curItem;
                item.Qty = qty;
                item.V_NewPriceType = curItem.V_NewPriceType;
                item.IsPackageService = curItem.IsPackageService;
                item.MedicalInstructionDate = Now.Date;
                item.RecordState = RecordState.ADDED;
                if (CurrentInPatientBillingInvoice.RegistrationDetails == null)
                {
                    CurrentInPatientBillingInvoice.RegistrationDetails = new ObservableCollection<PatientRegistrationDetail>();
                }
                else
                {
                    foreach (var row in CurrentInPatientBillingInvoice.RegistrationDetails)
                    {
                        if (row.MedServiceID == item.MedServiceID && item.MedServiceID > 0)
                        {
                            if (System.Windows.MessageBox.Show(item.RefMedicalServiceItem.MedServiceName + string.Format(" {0}", eHCMSResources.W0949_G1_W), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                            {
                                return;
                            }
                        }
                    }
                }
                Globals.CalcInvoiceItem(item, true, CurrentPatientRegistration);
                CurrentInPatientBillingInvoice.RegistrationDetails.Add(item);
                AddMedRegItemBaseToNewInvoice(item);
            }
            else
            {
                AddDefaultPCLRequest(curItem.MedServiceID, qty);
            }
        }
        private void AddMedRegItemBaseToNewInvoice(MedRegItemBase item)
        {
            EditingInvoiceDetailsContent.AddItemToView(item);
        }
        private void AddPCLItem(PCLExamType pclItem, int qty, DateTime createdDate, bool used)
        {
            if (pclItem == null)
            {
                return;
            }
            for (int i = 0; i < qty; i++)
            {
                PatientPCLRequestDetail item;
                item = new PatientPCLRequestDetail();
                item.StaffID = Globals.LoggedUserAccount.StaffID;
                item.CreatedDate = createdDate;
                item.MedProductType = AllLookupValues.MedProductType.CAN_LAM_SANG;
                item.PCLExamType = pclItem;
                item.Qty = 1;
                item.V_NewPriceType = pclItem.V_NewPriceType;
                item.MedicalInstructionDate = Now.Date;
                item.RecordState = RecordState.DETACHED;
                if (used)
                {
                    item.V_ExamRegStatus = (long)AllLookupValues.ExamRegStatus.HOAN_TAT;
                }
                else
                {
                    item.V_ExamRegStatus = (long)AllLookupValues.ExamRegStatus.DANG_KY_KHAM;
                }
                if (CurrentInPatientBillingInvoice.PclRequests == null)
                {
                    CurrentInPatientBillingInvoice.PclRequests = new ObservableCollection<PatientPCLRequest>();
                }
                PatientPCLRequest tempRequest = CurrentInPatientBillingInvoice.PclRequests.Where(p => p.V_PCLRequestType == AllLookupValues.V_PCLRequestType.NOI_TRU
                    && p.StaffID == item.StaffID
                    && p.CreatedDate.Date == createdDate.Date
                    && p.RecordState == RecordState.DETACHED).FirstOrDefault();
                if (tempRequest == null)
                {
                    tempRequest = new PatientPCLRequest();
                    tempRequest.PatientPCLRequestIndicators = new ObservableCollection<PatientPCLRequestDetail>();
                    tempRequest.Diagnosis = eHCMSResources.Z0159_G1_CLSBNMoi;
                    tempRequest.StaffID = item.StaffID;
                    tempRequest.CreatedDate = createdDate;
                    tempRequest.V_PCLRequestType = AllLookupValues.V_PCLRequestType.NOI_TRU;
                    if (used)
                    {
                        tempRequest.V_PCLRequestStatus = AllLookupValues.V_PCLRequestStatus.CLOSE;
                    }
                    else
                    {
                        tempRequest.V_PCLRequestStatus = AllLookupValues.V_PCLRequestStatus.OPEN;
                    }
                    tempRequest.RecordState = RecordState.DETACHED;
                    tempRequest.EntityState = EntityState.DETACHED;
                    CurrentInPatientBillingInvoice.PclRequests.Add(tempRequest);
                }
                Globals.CalcInvoiceItem(item, true, CurrentPatientRegistration);
                tempRequest.PatientPCLRequestIndicators.Add(item);
                AddMedRegItemBaseToNewInvoice(item);
            }
        }
        public void CheckAndAddAllPCL(ObservableCollection<PCLExamType> AllPCLExamType, int qty, DateTime createdDate, bool used)
        {
            if (AllPCLExamType == null)
            {
                return;
            }
            ObservableCollection<PCLExamType> NewPCLList = new ObservableCollection<PCLExamType>();
            ObservableCollection<PCLExamType> ExistsPCLList = new ObservableCollection<PCLExamType>();
            if (CurrentInPatientBillingInvoice != null && CurrentInPatientBillingInvoice.PclRequests != null)
            {
                var lstpcldetails = CurrentInPatientBillingInvoice.PclRequests.SelectMany(x => x.PatientPCLRequestIndicators);
                foreach (var item in AllPCLExamType)
                {
                    if (lstpcldetails.Any(x => x.PCLExamType != null && x.PCLExamType.PCLExamTypeID == item.PCLExamTypeID))
                    {
                        ExistsPCLList.Add(item);
                    }
                    else
                    {
                        NewPCLList.Add(item);
                    }
                }
            }
            if (ExistsPCLList != null && ExistsPCLList.Count > 0)
            {
                string strPCLName = "";
                foreach (PCLExamType existsPCL in ExistsPCLList)
                {
                    strPCLName += Environment.NewLine + existsPCL.PCLExamTypeName + ".";
                }
                if (MessageBox.Show(eHCMSResources.A0892_G1_Msg_InfoPCLDaTonTai + strPCLName + Environment.NewLine + eHCMSResources.T1986_G1_CoMuonTiepTucThemKhong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    foreach (PCLExamType item in AllPCLExamType)
                    {
                        AddPCLItem(item, qty, createdDate, used);
                    }
                }
                else
                {
                    foreach (PCLExamType item in NewPCLList)
                    {
                        AddPCLItem(item, qty, createdDate, used);
                    }
                }
            }
            else
            {
                foreach (PCLExamType item in AllPCLExamType)
                {
                    AddPCLItem(item, qty, createdDate, used);
                }
            }
        }
        public void AddDefaultPCLRequest(long medServiceID, int qty)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginCreateNewPCLRequest(medServiceID, Globals.DispatchCallback((asyncResult) =>
                        {
                            PatientPCLRequest item = null;
                            PatientPCLRequest externalRequest = null;
                            try
                            {
                                contract.EndCreateNewPCLRequest(out item, out externalRequest, asyncResult);
                                ObservableCollection<PCLExamType> PCLExamTypeList = new ObservableCollection<PCLExamType>();
                                ObservableCollection<PCLExamType> PCLExamTypeList_Ext = new ObservableCollection<PCLExamType>();
                                if (item != null && item.PatientPCLRequestIndicators != null)
                                {
                                    foreach (var requestDetails in item.PatientPCLRequestIndicators)
                                    {
                                        PCLExamTypeList.Add(requestDetails.PCLExamType);
                                    }
                                    if (PCLExamTypeList != null && PCLExamTypeList.Count > 0)
                                    {
                                        CheckAndAddAllPCL(PCLExamTypeList, qty, Now.Date, false);
                                    }
                                }

                                if (externalRequest != null && externalRequest.PatientPCLRequestIndicators != null)
                                {
                                    foreach (var requestDetails in externalRequest.PatientPCLRequestIndicators)
                                    {
                                        PCLExamTypeList_Ext.Add(requestDetails.PCLExamType);
                                    }
                                    if (PCLExamTypeList_Ext != null && PCLExamTypeList_Ext.Count > 0)
                                    {
                                        CheckAndAddAllPCL(PCLExamTypeList_Ext, qty, Now.Date, false);
                                    }
                                }
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                                this.HideBusyIndicator();
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                this.HideBusyIndicator();
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private bool ValidatePclItem(out ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> result)
        {
            result = new ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult>();
            if (SelectPCLContent.SelectedPCLExamType == null || SelectPCLContent.SelectedPCLExamType.PCLExamTypeID < 1)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0164_G1_HayChonDVCLS, new string[] { "SelectedPclExamType" });
                result.Add(item);
            }
            if (PclQty.GetValueOrDefault(0) < 1)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0163_G1_NhapGTriSLgDV, new string[] { "PclQty" });
                result.Add(item);
            }
            if (result.Count > 0)
            {
                return false;
            }
            return true;
        }
        private void CheckAndAddPCL(PCLExamType pclItem, int qty, DateTime createdDate, bool used)
        {
            if (pclItem == null)
            {
                return;
            }
            if (CurrentInPatientBillingInvoice != null && CurrentInPatientBillingInvoice.PclRequests != null)
            {
                var lstpcldetails = CurrentInPatientBillingInvoice.PclRequests.SelectMany(x => x.PatientPCLRequestIndicators).Where(x => x.RecordState != RecordState.DELETED);
                if (lstpcldetails.Any(x => x.PCLExamType != null && x.PCLExamType.PCLExamTypeID == pclItem.PCLExamTypeID))
                {
                    if (MessageBox.Show(pclItem.PCLExamTypeName + string.Format(" {0}", eHCMSResources.W0949_G1_W), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                    {
                        return;
                    }
                }
            }
            if (pclItem.V_NewPriceType == (long)AllLookupValues.V_NewPriceType.Unknown_PriceType || pclItem.V_NewPriceType == (long)AllLookupValues.V_NewPriceType.Updatable_PriceType)
            {
                PatientPCLRequestDetail item = new PatientPCLRequestDetail();
                item.PCLExamType = pclItem;
                Action<IModifyBillingInvItem> onInitDlg = delegate (IModifyBillingInvItem vm)
                {
                    vm.ModifyBillingInvItem.IsModItemOK = false;
                    vm.ModifyBillingInvItem = item;
                    vm.ModifyBillingInvItem.V_NewPriceType = item.PCLExamType.V_NewPriceType;
                    vm.PopupType = 1;
                    vm.Init();
                };
                GlobalsNAV.ShowDialog<IModifyBillingInvItem>(onInitDlg);
            }
            else
            {
                AddPCLItem(pclItem, qty, createdDate, used);
            }
        }
        private bool ValidatePclItemLAB(out ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> result)
        {
            result = new ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult>();
            if (SelectPCLContentLAB.PclExamTypes == null || SelectPCLContentLAB.PclExamTypes.Count <= 0
                || SelectPCLContentLAB.SelectedPCLExamType == null || SelectPCLContentLAB.SelectedPCLExamType.PCLExamTypeID < 1)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0171_G1_HayChonLoaiCLSCanThem, new string[] { "SelectedPclExamType" });
                result.Add(item);
            }
            if (PclQtyLAB.GetValueOrDefault(0) < 1)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0163_G1_NhapGTriSLgDV, new string[] { "PclQtyLAB" });
                result.Add(item);
            }
            if (result.Count > 0)
            {
                return false;
            }
            return true;
        }
        private void SearchRefGenMedProductDetails(object sender, string Name, bool? IsCode)
        {
            RefGenMedProductDetailsList = new List<RefGenMedProductDetails>();
            if (IsCode == false && Name.Length < 1)
            {
                return;
            }
            var CurrentThread = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetGenMedProductAndPrice(false, Name, (long)AllLookupValues.MedProductType.Y_CU, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetGenMedProductAndPrice(asyncResult);
                            (sender as AxAutoComplete).ItemsSource = results;
                            (sender as AxAutoComplete).PopulateComplete();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }), null);
                }
            });
            CurrentThread.Start();
        }
        private IEnumerator<IResult> DoGetStoreToSell()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT, false, null, false, false);
            yield return paymentTypeTask;
            var StoreCbx = paymentTypeTask.LookupList;
            if (StoreCbx != null && StoreCbx.Any(x => x.StoreTypeID == (long)AllLookupValues.StoreType.STORAGE_DRUGDEPT))
            {
                DefaultStoreIDForQuotation = StoreCbx.FirstOrDefault(x => x.StoreTypeID == (long)AllLookupValues.StoreType.STORAGE_DRUGDEPT).StoreID;
            }
            yield break;
        }
        public void AddGenMedItem(object medItem, decimal qty, DateTime createdDate)
        {
            RefGenMedProductDetails curItem = medItem as RefGenMedProductDetails;
            curItem.HIAllowedPriceNew = curItem.HIAllowedPrice;
            curItem.HIPatientPriceNew = curItem.HIPatientPrice;
            curItem.NormalPriceNew = curItem.NormalPrice;
            if (curItem != null)
            {
                var item = new OutwardDrugClinicDept();
                item.StaffID = Globals.LoggedUserAccount.StaffID;
                item.CreatedDate = createdDate;
                item.GenMedProductItem = curItem;
                item.MedProductType = AllLookupValues.MedProductType.Y_CU;
                item.OutQuantity = qty;
                item.Qty = qty;
                item.MedicalInstructionDate = Now;
                item.HIBenefit = CurrentPatientRegistration.PtInsuranceBenefit;
                item.LimQtyAndHIPrice = curItem.LimQtyAndHIPrice;
                item.GenMedProductID = curItem.GenMedProductID;
                item.outiID = 0;
                item.RecordState = RecordState.ADDED;
                if (CurrentInPatientBillingInvoice.OutwardDrugClinicDeptInvoices == null)
                {
                    CurrentInPatientBillingInvoice.OutwardDrugClinicDeptInvoices = new ObservableCollection<OutwardDrugClinicDeptInvoice>();
                }
                var lstdrugDetails = CurrentInPatientBillingInvoice.OutwardDrugClinicDeptInvoices.Where(x => x.MedProductType == AllLookupValues.MedProductType.Y_CU).SelectMany(x => x.OutwardDrugClinicDepts).Where(owd => owd.GenMedProductItem.GenMedProductID == item.GenMedProductItem.GenMedProductID);
                if (lstdrugDetails != null && lstdrugDetails.Count() > 0)
                {
                    if (MessageBox.Show(item.GenMedProductItem.BrandName + string.Format(" {0}", eHCMSResources.W0949_G1_W), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                }
                if (!CurrentInPatientBillingInvoice.OutwardDrugClinicDeptInvoices.Any(p => p.MedProductType == AllLookupValues.MedProductType.Y_CU
                    && p.outiID <= 0))
                {
                    CurrentInPatientBillingInvoice.OutwardDrugClinicDeptInvoices.Add(new OutwardDrugClinicDeptInvoice()
                    {
                        OutwardDrugClinicDepts = new ObservableCollection<OutwardDrugClinicDept>(),
                        MedProductType = AllLookupValues.MedProductType.Y_CU,
                        OutDate = createdDate,
                        StaffID = Globals.LoggedUserAccount.StaffID,
                        StoreID = curItem.StoreID,
                        SelectedStorage = curItem.Storage
                    });
                }
                var lastInvoice = CurrentInPatientBillingInvoice.OutwardDrugClinicDeptInvoices.Last(p => p.MedProductType == AllLookupValues.MedProductType.Y_CU && p.outiID <= 0);
                item.DrugInvoice = lastInvoice;
                Globals.CalcInvoiceItem(item, true, CurrentPatientRegistration);
                //item.OutID = lastInvoice.OutwardDrugClinicDepts.Count + 1;
                item.OutID = 0;
                lastInvoice.OutwardDrugClinicDepts.Add(item);
                AddMedRegItemBaseToNewInvoice(item);
            }
        }
        private bool ValidateMedItem(out ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> result)
        {
            result = new ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult>();
            if (SelectedMedProduct == null || SelectedMedProduct.GenMedProductID < 1)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0166_G1_HayChonYCu, new string[] { "SelectedInwardDrug" });
                result.Add(item);
            }
            if (MedItemQty.GetValueOrDefault(0) < 1)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0167_G1_NhapGTriSLgYCu, new string[] { "MedItemQty" });
                result.Add(item);
            }
            if (result.Count > 0)
            {
                return false;
            }
            return true;
        }
        private void CallSaveQuotationCmd(long? PatientID)
        {
            if (CurrentInPatientBillingInvoice == null)
            {
                return;
            }
            CurrentInPatientBillingInvoice.InPatientBillingInvID = 0;
            if (CurrentInPatientBillingInvoice.RegistrationDetails != null &&
                CurrentInPatientBillingInvoice.RegistrationDetails.Any(x => x.RecordState == RecordState.DELETED))
            {
                CurrentInPatientBillingInvoice.RegistrationDetails = CurrentInPatientBillingInvoice.RegistrationDetails.Where(x => x.RecordState != RecordState.DELETED).ToObservableCollection();
            }
            if (CurrentInPatientBillingInvoice.PclRequests != null &&
                CurrentInPatientBillingInvoice.PclRequests.Any(x => x.RecordState == RecordState.DELETED))
            {
                CurrentInPatientBillingInvoice.PclRequests = CurrentInPatientBillingInvoice.PclRequests.Where(x => x.RecordState != RecordState.DELETED).ToObservableCollection();
            }
            if (CurrentInPatientBillingInvoice.OutwardDrugClinicDeptInvoices != null &&
               CurrentInPatientBillingInvoice.OutwardDrugClinicDeptInvoices.Any(x => x.RecordState == RecordState.DELETED))
            {
                CurrentInPatientBillingInvoice.OutwardDrugClinicDeptInvoices = CurrentInPatientBillingInvoice.OutwardDrugClinicDeptInvoices.Where(x => x.RecordState != RecordState.DELETED).ToObservableCollection();
            }
            if (CurrentInPatientBillingInvoice.RegistrationDetails != null)
            {
                foreach (var aItem in CurrentInPatientBillingInvoice.RegistrationDetails)
                {
                    aItem.PtRegDetailID = 0;
                    aItem.PtRegistrationID = 0;
                }
            }
            if (CurrentInPatientBillingInvoice.PclRequests != null)
            {
                foreach (var aItem in CurrentInPatientBillingInvoice.PclRequests)
                {
                    aItem.PatientPCLRequestIndicators = aItem.PatientPCLRequestIndicators.Where(x => x.RecordState != RecordState.DELETED).ToObservableCollection();
                }
            }
            if (CurrentInPatientBillingInvoice.OutwardDrugClinicDeptInvoices != null)
            {
                foreach (var aItem in CurrentInPatientBillingInvoice.OutwardDrugClinicDeptInvoices)
                {
                    aItem.OutwardDrugClinicDepts = aItem.OutwardDrugClinicDepts.Where(x => x.RecordState != RecordState.DELETED).ToObservableCollection();
                }
                foreach (var aItem in CurrentInPatientBillingInvoice.OutwardDrugClinicDeptInvoices.SelectMany(x => x.OutwardDrugClinicDepts))
                {
                    aItem.OutID = 0;
                }
            }
            CurrentInPatientBillingInvoice.StaffID = Globals.LoggedUserAccount.StaffID.GetValueOrDefault(0);
            this.ShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSaveQuotation(CurrentInPatientBillingInvoice, CurrentInPatientBillingInvoice.QuotationTitle, PatientID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                long OutQuotationID;
                                contract.EndSaveQuotation(out OutQuotationID, asyncResult);
                                CurrentInPatientBillingInvoice.InPatientBillingInvID = OutQuotationID;
                                MessageBox.Show(eHCMSResources.A1053_G1_Msg_InfoTHienOK);
                                if (PatientID > 0)
                                {
                                    IsActionCanTrigger = false;
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0074_G1_I);
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0074_G1_I);
                    this.HideBusyIndicator();
                }
            });
            CurrentThread.Start();
        }
        private void CallSelectedOldQuotation(InPatientBillingInvoice SelectedQuotation)
        {
            this.ShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetQuotationAllDetail(SelectedQuotation.InPatientBillingInvID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var GettedItem = contract.EndGetQuotationAllDetail(asyncResult);
                                if (GettedItem != null)
                                {
                                    CurrentPtInsuranceBenefit = GettedItem.HIBenefit;
                                    GettedItem.BillFromDate = null;
                                    GettedItem.BillToDate = null;
                                    if (GettedItem.RegistrationDetails != null)
                                    {
                                        foreach (var aItem in GettedItem.RegistrationDetails)
                                        {
                                            aItem.MedicalInstructionDate = Now;
                                        }
                                    }
                                    if (GettedItem.PclRequests != null)
                                    {
                                        if (GettedItem.PclRequests.Any(x => x.PatientServiceRecord != null))
                                        {
                                            foreach (var aItem in GettedItem.PclRequests.Where(x => x.PatientServiceRecord != null))
                                            {
                                                aItem.PatientServiceRecord = null;
                                            }
                                        }
                                        foreach (var aItem in GettedItem.PclRequests.SelectMany(x => x.PatientPCLRequestIndicators))
                                        {
                                            aItem.MedicalInstructionDate = Now;
                                        }
                                    }
                                    if (GettedItem.OutwardDrugClinicDeptInvoices != null)
                                    {
                                        foreach (var aItem in GettedItem.OutwardDrugClinicDeptInvoices)
                                        {
                                            aItem.outiID = 0;
                                        }
                                        foreach (var aItem in GettedItem.OutwardDrugClinicDeptInvoices.SelectMany(x => x.OutwardDrugClinicDepts))
                                        {
                                            aItem.MedicalInstructionDate = Now;
                                            aItem.outiID = 0;
                                        }
                                        if (SelectedQuotation.CurrentPatient == null || SelectedQuotation.CurrentPatient.PatientID == 0)
                                        {
                                            foreach (var aItem in GettedItem.OutwardDrugClinicDeptInvoices.SelectMany(x => x.OutwardDrugClinicDepts))
                                            {
                                                aItem.ChargeableItem.HIAllowedPrice = aItem.ChargeableItem.HIAllowedPriceNew;
                                                aItem.HIAllowedPrice = aItem.ChargeableItem.HIAllowedPriceNew;
                                                aItem.ChargeableItem.HIPatientPrice = aItem.ChargeableItem.HIPatientPrice;
                                            }
                                        }
                                    }
                                    GettedItem.InvDate = Now;
                                    CurrentInPatientBillingInvoice = GettedItem;
                                    PatientSummaryInfoContent.CurrentPatient = SelectedQuotation.CurrentPatient;
                                    if (SelectedQuotation.CurrentPatient != null && SelectedQuotation.CurrentPatient.PatientID > 0)
                                    {
                                        IsActionCanTrigger = false;
                                    }
                                    else
                                    {
                                        IsActionCanTrigger = true;
                                    }
                                }
                                else
                                {
                                    CurrentInPatientBillingInvoice = new InPatientBillingInvoice { IsHighTechServiceBill = true, InvDate = Now };
                                    EditingInvoiceDetailsContent.ResetView();
                                    PatientSummaryInfoContent.CurrentPatient = null;
                                    IsActionCanTrigger = true;
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0074_G1_I);
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0074_G1_I);
                    this.HideBusyIndicator();
                }
            });
            CurrentThread.Start();
        }
        #endregion
        #region Handles
        public void Handle(DoubleClick message)
        {
            if (message.Source != SelectPCLContent)
            {
                return;
            }
            if (message.EventArgs.Value != SelectPCLContent.SelectedPCLExamType)
            {
                return;
            }
            AddPclExamTypeCmd();
        }
        public void Handle(DoubleClickAddReqLAB message)
        {
            if (message.Source != SelectPCLContentLAB)
            {
                return;
            }
            if (message.EventArgs.Value != SelectPCLContentLAB.SelectedPCLExamType)
            {
                return;
            }
            AddPclExamTypeCmdLAB();
        }
        public void Handle(ItemSelected<RefMedicalServiceItem> message)
        {
            if (this.GetView() != null)
            {
                ServiceQty = 1;
            }
        }
        public void Handle(ItemSelected<PCLExamType> message)
        {
            if (this.GetView() != null)
            {
                PclQty = 1;
                PclQtyLAB = 1;
            }
        }
        public void Handle(ResultFound<Patient> message)
        {
            if (message != null)
            {
                Globals.EventAggregator.Publish(new ItemSelected<Patient> { Item = message.Result });
                txtInsuranceBenefit_LostFocus(null, null);
            }
        }
        #endregion
        public void Handle(ItemRemoved<MedRegItemBase, IInPatientBillingInvoiceDetailsListing> message)
        {
            if (message == null || message.Item == null || CurrentInPatientBillingInvoice == null)
            {
                return;
            }
            if (message.Item is OutwardDrugClinicDept)
            {
                if (CurrentInPatientBillingInvoice.OutwardDrugClinicDeptInvoices != null && CurrentInPatientBillingInvoice.OutwardDrugClinicDeptInvoices.Count > 0)
                {
                    ObservableCollection<OutwardDrugClinicDept> ListRemoveDrug = new ObservableCollection<OutwardDrugClinicDept>();
                    foreach (var item in CurrentInPatientBillingInvoice.OutwardDrugClinicDeptInvoices)
                    {
                        foreach (var drugdetail in item.OutwardDrugClinicDepts)
                        {
                            if (drugdetail.OutID > 0 && drugdetail.OutID == (message.Item as OutwardDrugClinicDept).OutID)
                            {
                                drugdetail.RecordState = RecordState.DELETED;
                            }
                            else if ((message.Item as OutwardDrugClinicDept).OutID == 0)
                            {
                                ListRemoveDrug.Add(drugdetail);
                            }
                        }
                    }
                    if (ListRemoveDrug.Count > 0)
                    {
                        foreach (var items in ListRemoveDrug)
                        {
                            CurrentInPatientBillingInvoice.OutwardDrugClinicDeptInvoices[0].OutwardDrugClinicDepts.Remove(items);
                        }
                    }
                }
            }
            else if (message.Item is PatientPCLRequestDetail)
            {
                if (CurrentInPatientBillingInvoice.PclRequests != null && CurrentInPatientBillingInvoice.PclRequests.Count > 0)
                {
                    ObservableCollection<PatientPCLRequestDetail> ListRemovePCL = new ObservableCollection<PatientPCLRequestDetail>();
                    foreach (var item in CurrentInPatientBillingInvoice.PclRequests)
                    {
                        foreach (var detail in item.PatientPCLRequestIndicators)
                        {
                            if (detail.PCLReqItemID > 0 && detail.PCLReqItemID == (message.Item as PatientPCLRequestDetail).PCLReqItemID)
                            {
                                detail.RecordState = RecordState.DELETED;
                            }
                            else if ((message.Item as PatientPCLRequestDetail).PCLReqItemID == 0)
                            {
                                ListRemovePCL.Add(detail);
                            }
                        }
                    }
                    if (ListRemovePCL.Count > 0)
                    {
                        foreach (var items in ListRemovePCL)
                        {
                            CurrentInPatientBillingInvoice.PclRequests[0].PatientPCLRequestIndicators.Remove(items);
                        }
                    }
                }
            }
            else if (message.Item is PatientRegistrationDetail)
            {
                if (CurrentInPatientBillingInvoice.RegistrationDetails != null && CurrentInPatientBillingInvoice.RegistrationDetails.Count > 0)
                {
                    ObservableCollection<PatientRegistrationDetail> ListRemove = new ObservableCollection<PatientRegistrationDetail>();
                    foreach (var item in CurrentInPatientBillingInvoice.RegistrationDetails)
                    {
                        if (item.PtRegDetailID > 0 && item.PtRegDetailID == (message.Item as PatientRegistrationDetail).PtRegDetailID)
                        {
                            item.RecordState = RecordState.DELETED;
                        }
                        else if ((message.Item as PatientRegistrationDetail).PtRegDetailID == 0)
                        {
                            ListRemove.Add(item);
                        }
                    }
                    if (ListRemove.Count > 0)
                    {
                        foreach (var items in ListRemove)
                        {
                            CurrentInPatientBillingInvoice.RegistrationDetails.Remove(items);
                        }
                    }
                }
            }
        }
        public enum ViewCase : short
        {
            Template = 1,
            Quotation = 2
        }

        public void UpdatePatientQuotationCmd()
        {
            if (CurrentInPatientBillingInvoice == null || string.IsNullOrEmpty(CurrentInPatientBillingInvoice.QuotationTitle))
            {
                Globals.ShowMessage(eHCMSResources.Z2879_G1_NhapTieuDe, eHCMSResources.G0442_G1_TBao);
                return;
            }
            else if (PatientSummaryInfoContent.CurrentPatient == null || PatientSummaryInfoContent.CurrentPatient.PatientID == 0)
            {
                Globals.ShowMessage(eHCMSResources.K0294_G1_ChonBN, eHCMSResources.G0442_G1_TBao);
                return;
            }
            CallUpdateQuotationCmd(PatientSummaryInfoContent.CurrentPatient.PatientID);
        }
        private void CallUpdateQuotationCmd(long? PatientID)
        {
            this.ShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdateQuotation(CurrentInPatientBillingInvoice, CurrentInPatientBillingInvoice.QuotationTitle, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndUpdateQuotation(asyncResult);
                                MessageBox.Show(eHCMSResources.A1053_G1_Msg_InfoTHienOK);
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0074_G1_I);
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0074_G1_I);
                    this.HideBusyIndicator();
                }
            });
            CurrentThread.Start();
        }
    }
}