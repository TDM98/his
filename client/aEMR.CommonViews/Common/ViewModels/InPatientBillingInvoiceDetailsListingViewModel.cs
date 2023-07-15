using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using aEMR.Controls;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows.Controls;
using Caliburn.Micro;
using DataEntities;
using System.Windows;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using aEMR.DataContracts;
using aEMR.ServiceClient;
using aEMR.Infrastructure.Events;
using aEMR.CommonTasks;
using aEMR.Common.DataValidation;
using aEMR.Common;
using aEMR.Common.Utilities;
using aEMR.Common.Collections;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
/*
 * 20170221 #001 CMN:   Fix Recal price with HIBenefit
 * 20170522 #002 CMN:   Added variable to check InPt 5 year HI without paid enough
 * 20180102 #003 CMN:   Added properties for 4210 file
 * 20180117 #004 CMN:   Added new benefit for Stent 2nd of NotCross <(CurentRegistration.IsCrossRegion.GetValueOrDefault(false) ? (decimal)Globals.ServerConfigSection.HealthInsurances.RebatePercentageLevel1 / (outwardDrug.HIPaymentPercent != 0 ? (decimal)outwardDrug.HIPaymentPercent.GetValueOrDefault(1) : 1.0m) : 1.0m)>
 * 20180509 #005 TxD:   Make sure that Item with IsCountHI == true ALWAYS has MedicalInstructionDate within the validity of a corresponding HI Card ValidDateFrom and ValidDateTo
 * 20181006 #006 TTM:   Thay đổi cách lấy giá trị cột từ e.Column.GetValue(FrameworkElement.NameProperty).ToString()
 *                      => e.Column.Equals("Tên Grid".GetColumnByName("Tên cột") vì cách cũ bị sai => Lấy giá trị cột không đc. 
 * 20181013 #007 TTM:   Fix lỗi chỉnh sửa số lượng, giá bị thay đổi. (Đang là giá cũ => giá mới do bảng giá thay đổi trước khi quyết toán bill.)
 * 20191207 #008 TTM:   BM 0019706: [Miễn giảm] Fix lỗi không apply thông tin miễn giảm khi cập nhật bổ sung miễn giảm cho CLS trong bill
 * 20200215 #009 TTM:   BM 0023916: Cho phép xoá DV TT-PT đã thực hiện khỏi bill viện phí.
 * 20200523 #010 TTM:   BM 0038189: Fix lỗi Stent số 2 quyền lợi < 100% tính toán sai.
 * 20200713 #011 TTM:   BM 0039358: Fix lỗi không hiển thị đúng khi load các dịch vụ ngoại trú lần 2 vào bill nội trú (Sáp nhập).
 * 20201026 #012 TNHX:   BM: Ẩn nút check tính BN dưa trên cấu hình DisableBtnCheckCountPatientInPt
 * 20201117 #013 TNHX:   BM: Tăng thời gian cho phép thanh BHYT khi đã hết hạn thẻ trong nội trú dựa trên cấu hình NumOfOverDaysForMedicalInstructDate
 * 20210308 #014 TNHX:   219 Nếu bill được tạo từ khoa khám bệnh thì không cho xóa
 * 20210921 #015 TNHX:   436 Nếu DV giường tự động thì không cho xóa
 * 20210928 #016 TNHX:   681 Nếu đăng ký BN điều trị COVID thì hiển thị tích COVID trong bill
 * 20211004 #017 TNHX: Lọc danh sách bsi theo cấu hình trách nhiệm + theo tích tạm khóa
 * 20211117 #018 TNHX: Lấy giá vốn cho thuốc dịch vụ nằm trong danh mục covid. Do có thuốc tỷ lệ nên KD đã xác nhận nên chỉ lấy giá vốn
 * 20220103 #019 TNHX: Chỉnh lại tích covid + phân quyền
 * 20220728 #020 QTD:  Thêm đánh dấu tính dịch vụ Bill
 * 20230104 #021 BLQ: Thêm thời hạn được tính bảo hiểm theo cấu hình NumDayHIAgreeToPayAfterHIExpiresInPt
 * 20230515 #022 QTD:  Thêm hiển thị trần vật tư của DVKT
 */
namespace aEMR.Common.ViewModels
{
    /// <summary>
    /// Hiển thị danh sách các item đã đăng ký của một bill trong đăng ký nội trú.
    /// Không có chi tiết của billing invoice.
    /// </summary>
    [Export(typeof(IInPatientBillingInvoiceDetailsListing)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InPatientBillingInvoiceDetailsListingViewModel : Conductor<object>, IInPatientBillingInvoiceDetailsListing
    //, IHandle<ModifyPriceToUpdate_Completed>
        , IHandle<CalcPaymentCeilingForTechServiceEvent>
    {
        [ImportingConstructor]
        public InPatientBillingInvoiceDetailsListingViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            eventArg.Subscribe(this);
            var vm = Globals.GetViewModel<IEnumListing>();
            vm.EnumType = typeof(AllLookupValues.V_BillingInvType);
            vm.AddSelectOneItem = true;
            vm.LoadData();
            BillingTypeContent = vm;
            (BillingTypeContent as INotifyPropertyChangedEx).PropertyChanged += InPatientBillingInvoiceDetailsListingViewModel_PropertyChanged;
        }

        void InPatientBillingInvoiceDetailsListingViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                BindBillingInvoiceType();
            }
        }
        private IEnumListing _billingTypeContent;
        public IEnumListing BillingTypeContent
        {
            get { return _billingTypeContent; }
            set
            {
                _billingTypeContent = value;
                NotifyOfPropertyChange(() => BillingTypeContent);
            }
        }
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            var currentview = view as IInPatientBillingInvoiceDetailsListingView;
            if (currentview != null)
            {
                currentview.ShowDeleteColumn(_showDeleteColumn);
                currentview.ShowHiAppliedColumn(_showHIAppliedColumn);
            }
        }

        private bool _canEditOnGrid;
        public bool CanEditOnGrid
        {
            get { return _canEditOnGrid; }
            set
            {
                _canEditOnGrid = value;
                NotifyOfPropertyChange(() => CanEditOnGrid);
                NotifyOfPropertyChange(() => CanEditBillingType);
                NotifyOfPropertyChange(() => CanEditDate);
            }
        }

        private bool _canDelete = true;
        public bool CanDelete
        {
            get
            {
                return _canDelete;
            }
            set
            {
                if (_canDelete != value)
                {
                    _canDelete = value;
                    NotifyOfPropertyChange(() => CanDelete);

                    if (AllRegistrationItems != null)
                    {
                        foreach (var item in AllRegistrationItems)
                        {
                            item.CanDelete = _canDelete;
                            item.CanCheck = CanDelete && (HIBenefit.GetValueOrDefault(0) > 0);
                        }
                    }
                }
            }
        }

        private bool _showDeleteColumn = true;
        public bool ShowDeleteColumn
        {
            get { return _showDeleteColumn; }
            set
            {
                if (_showDeleteColumn != value)
                {
                    _showDeleteColumn = value;
                    NotifyOfPropertyChange(() => ShowDeleteColumn);

                    var view = this.GetView() as IInPatientBillingInvoiceDetailsListingView;
                    if (view != null)
                    {
                        view.ShowDeleteColumn(_showDeleteColumn);
                    }
                }
            }
        }

        private bool _showHIAppliedColumn = false;
        public bool ShowHIAppliedColumn
        {
            get { return _showHIAppliedColumn; }
            set
            {
                if (_showHIAppliedColumn != value)
                {
                    _showHIAppliedColumn = value;
                    NotifyOfPropertyChange(() => ShowHIAppliedColumn);

                    var view = this.GetView() as IInPatientBillingInvoiceDetailsListingView;
                    if (view != null)
                    {
                        view.ShowHiAppliedColumn(_showHIAppliedColumn);
                    }
                }
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                NotifyOfPropertyChange(() => IsLoading);
            }
        }

        private InPatientBillingInvoice _billingInvoice;
        public InPatientBillingInvoice BillingInvoice
        {
            get { return _billingInvoice; }
            set
            {
                if (_billingInvoice != value)
                {
                    _billingInvoice = value;
                    NotifyOfPropertyChange(() => BillingInvoice);

                    if (_billingInvoice != null)
                    {
                        BillingTypeContent.SetSelectedID(_billingInvoice.V_BillingInvType.ToString());
                        if (_billingInvoice.Department != null && _billingInvoice.Department.IsTreatmentForCOVID)
                        {
                            IsShowPatientCOVID = Visibility.Visible;
                        }
                        else
                        {
                            IsShowPatientCOVID = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        BillingTypeContent.SetSelectedID("");
                    }
                    NotifyOfPropertyChange(() => CanEditBillingType);
                    NotifyOfPropertyChange(() => CanEditDate);
                }
            }
        }

        private InPatientBillingInvoice _OldBillingInvoice;
        public InPatientBillingInvoice OldBillingInvoice
        {
            get { return _OldBillingInvoice; }
            set
            {
                if (_OldBillingInvoice != value)
                {
                    _OldBillingInvoice = value;
                    NotifyOfPropertyChange(() => OldBillingInvoice);
                }
            }
        }

        private ObservableCollection<MedRegItemBase> _OldRegistrationItems;
        public ObservableCollection<MedRegItemBase> OldRegistrationItems
        {
            get { return _OldRegistrationItems; }
            set
            {
                _OldRegistrationItems = value;
                NotifyOfPropertyChange(() => OldRegistrationItems);
            }
        }


        private ObservableCollection<MedRegItemBase> _allRegistrationItems;
        public ObservableCollection<MedRegItemBase> AllRegistrationItems
        {
            get { return _allRegistrationItems; }
            set
            {
                _allRegistrationItems = value;
                NotifyOfPropertyChange(() => AllRegistrationItems);
            }
        }

        private MedRegItemBase _SelectedRegistrationItem;
        public MedRegItemBase SelectedRegistrationItem
        {
            get { return _SelectedRegistrationItem; }
            set
            {
                _SelectedRegistrationItem = value;
                NotifyOfPropertyChange(() => SelectedRegistrationItem);
            }
        }

        private double? _HIBenefit;
        public double? HIBenefit
        {
            get { return _HIBenefit; }
            set
            {
                _HIBenefit = value;
                NotifyOfPropertyChange(() => HIBenefit);

            }
        }

        private bool _CanShowPopupToModifyPrice = false;
        public bool CanShowPopupToModifyPrice
        {
            get
            {
                return _CanShowPopupToModifyPrice;
            }
            set
            {
                _CanShowPopupToModifyPrice = value;
                NotifyOfPropertyChange(() => CanShowPopupToModifyPrice);
            }
        }

        public void ResetView()
        {
            ConvertRegistrationInfo(true);
        }

        public bool CheckDifferentBillingInvoice()
        {
            ConvertRegistrationInfo(false);
            int NumOfOldRegItems = OldRegistrationItems != null && OldRegistrationItems.Count() > 0 ? OldRegistrationItems.Count() : 0;
            int NumOfCurRegItems = AllRegistrationItems != null && AllRegistrationItems.Count() > 0 ? AllRegistrationItems.Count() : 0;
            if (OldRegistrationItems.Count() != AllRegistrationItems.Count())
            {
                return true;
            }
            if (NumOfOldRegItems > 0)
            {
                foreach (var item in AllRegistrationItems)
                {
                    MedRegItemBase compareItem = OldRegistrationItems.FirstOrDefault(x => x.ID == item.ID && x.MedProductType == item.MedProductType && x.RecordState != RecordState.DELETED);
                    if (compareItem == null)
                    {
                        return true;
                    }
                    if (compareItem.Qty != item.Qty)
                    {
                        return true;
                    }
                    if (compareItem.TotalPatientPayment != item.TotalPatientPayment || compareItem.TotalHIPayment != item.TotalHIPayment || compareItem.TotalInvoicePrice != item.TotalInvoicePrice
                        || compareItem.TotalCoPayment != item.TotalCoPayment || compareItem.TotalPriceDifference != item.TotalPriceDifference)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public void ConvertRegistrationInfo(bool IsResetView)
        {
            InPatientBillingInvoice InputBillingInv;
            if (IsResetView)
            {
                InputBillingInv = BillingInvoice;
            }
            else
            {
                InputBillingInv = OldBillingInvoice;
            }
            if (InputBillingInv == null)
            {
                if (IsResetView)
                    AllRegistrationItems = null;
                else
                    OldRegistrationItems = null;
            }
            else
            {
                var allItems = new ObservableCollection<MedRegItemBase>();
                if (InputBillingInv.RegistrationDetails != null)
                {
                    foreach (var regDetails in InputBillingInv.RegistrationDetails)
                    {
                        //▼====: #014
                        //▼====: #015
                        if (regDetails.ReqFromDeptID == Globals.ServerConfigSection.Hospitals.KhoaPhongKham || !regDetails.IsNotBedService)
                        {
                            regDetails.CanDelete = false;
                        }
                        //▲====: #015
                        else
                        {
                            regDetails.CanDelete = CanDelete;
                        }
                        //▲====: #014
                        allItems.Add(regDetails);
                    }
                }
                if (InputBillingInv.PclRequests != null)
                {
                    foreach (var request in InputBillingInv.PclRequests)
                    {
                        if (request == null || request.PatientPCLRequestIndicators.Count <= 0)
                        {
                            continue;
                        }
                        //▼====: #014
                        bool tempCanDelete = CanDelete;
                        if (request.ReqFromDeptID == Globals.ServerConfigSection.Hospitals.KhoaPhongKham)
                        {
                            tempCanDelete = false;
                        }
                        foreach (var requestDetail in request.PatientPCLRequestIndicators)
                        {

                            requestDetail.CanDelete = tempCanDelete;
                            allItems.Add(requestDetail);
                        }
                        //▲====: #014
                    }
                }
                if (InputBillingInv.OutwardDrugClinicDeptInvoices != null)
                {
                    foreach (var inv in InputBillingInv.OutwardDrugClinicDeptInvoices)
                    {
                        if (inv == null || inv.OutwardDrugClinicDepts.Count <= 0)
                        {
                            continue;
                        }
                        int itemcount = 0;
                        foreach (var outwardDrug in inv.OutwardDrugClinicDepts)
                        {
                            outwardDrug.CanDelete = CanDelete;

                            //▼===== 20191217 Thay đổi code anh Công
                            /*==== #002 ====*/
                            if (!IsNewCreateBill)
                            {
                                if (outwardDrug.MaxQtyHIAllowItem > 1 && outwardDrug.HIPaymentPercent.GetValueOrDefault(1) < 1)
                                {
                                    outwardDrug.TotalHIPayment = Math.Min(outwardDrug.GenMedProductItem == null ? 0 : outwardDrug.GenMedProductItem.CeilingPrice2ndItem.GetValueOrDefault(0), outwardDrug.HIAllowedPrice.GetValueOrDefault(0) * (decimal)outwardDrug.HIPaymentPercent.GetValueOrDefault(1)) * (CurentRegistration != null && CurentRegistration.IsCrossRegion.GetValueOrDefault(false) ? (decimal)Globals.ServerConfigSection.HealthInsurances.RebatePercentageLevel1 / (outwardDrug.HIPaymentPercent != 0 ? (decimal)outwardDrug.HIPaymentPercent.GetValueOrDefault(1) : 1.0m) : 1.0m);
                                    outwardDrug.TotalCoPayment = outwardDrug.HIAllowedPrice.GetValueOrDefault(0) - outwardDrug.TotalHIPayment;
                                    outwardDrug.TotalPatientPayment = outwardDrug.TotalInvoicePrice - outwardDrug.TotalHIPayment;
                                }
                            }
                            else
                            {
                                if (outwardDrug.LimQtyAndHIPrice != null && outwardDrug.CountValue == 3)
                                {
                                    decimal PtHiBenefit = (decimal)CurentRegistration.PtInsuranceBenefit;
                                    if (outwardDrug.LimQtyAndHIPrice.ItemNumber3MaxBenefit > 0)
                                    {
                                        if (CurentRegistration.IsCrossRegion == false)
                                        {
                                            PtHiBenefit = outwardDrug.LimQtyAndHIPrice.ItemNumber3MaxBenefit;
                                        }
                                        else
                                        {
                                            PtHiBenefit = (decimal)Globals.ServerConfigSection.HealthInsurances.RebatePercentage2015Level1_InPt;
                                        }
                                    }
                                    //outwardDrug.TotalHIPayment = Math.Min(outwardDrug.GenMedProductItem == null ? 0 : outwardDrug.LimQtyAndHIPrice.ItemNumber1MaxPayAmt2, outwardDrug.HIAllowedPrice.GetValueOrDefault(0) * (decimal)outwardDrug.HIPaymentPercent.GetValueOrDefault(1)) * (CurentRegistration != null && CurentRegistration.IsCrossRegion.GetValueOrDefault(false) ? (decimal)Globals.ServerConfigSection.HealthInsurances.RebatePercentageLevel1 / (outwardDrug.HIPaymentPercent != 0 ? (decimal)outwardDrug.HIPaymentPercent.GetValueOrDefault(1) : 1.0m) : 1.0m);
                                    //outwardDrug.TotalHIPayment = Math.Min(outwardDrug.LimQtyAndHIPrice.ItemNumber3MaxPayAmt, outwardDrug.ChargeableItem.HIAllowedPrice.GetValueOrDefault(0) * (decimal)outwardDrug.HIPaymentPercent) * (decimal)CurentRegistration.PtInsuranceBenefit;
                                    outwardDrug.TotalHIPayment = Math.Min(outwardDrug.LimQtyAndHIPrice.ItemNumber3MaxPayAmt, outwardDrug.ChargeableItem.HIAllowedPrice.GetValueOrDefault(0) * (decimal)outwardDrug.HIPaymentPercent) * PtHiBenefit;
                                    outwardDrug.TotalCoPayment = outwardDrug.HIAllowedPrice.GetValueOrDefault(0) - outwardDrug.TotalHIPayment;
                                    outwardDrug.TotalPatientPayment = outwardDrug.TotalInvoicePrice - outwardDrug.TotalHIPayment;
                                }
                                if (outwardDrug.LimQtyAndHIPrice != null && outwardDrug.CountValue == 2)
                                {
                                    itemcount++;
                                    //▼===== #010
                                    //if (outwardDrug.GenMedProductItem != null && outwardDrug.GenMedProductItem.RefGenDrugCatID_1 == 9)
                                    //{
                                    //    outwardDrug.TotalHIPayment = Math.Min(outwardDrug.GenMedProductItem == null ? 0 : outwardDrug.LimQtyAndHIPrice.ItemNumber2MaxPayAmt, outwardDrug.HIAllowedPrice.GetValueOrDefault(0) * (decimal)outwardDrug.HIPaymentPercent.GetValueOrDefault(1)) * (CurentRegistration != null && CurentRegistration.IsCrossRegion.GetValueOrDefault(false) ? (decimal)Globals.ServerConfigSection.HealthInsurances.RebatePercentageLevel1 / (outwardDrug.HIPaymentPercent != 0 ? (decimal)outwardDrug.HIPaymentPercent.GetValueOrDefault(1) : 1.0m) : 1.0m);
                                    //}
                                    //else
                                    //{
                                    //    outwardDrug.TotalHIPayment = Math.Min(outwardDrug.LimQtyAndHIPrice.ItemNumber2MaxPayAmt, outwardDrug.ChargeableItem.HIAllowedPrice.GetValueOrDefault(0) * (decimal)outwardDrug.HIPaymentPercent) * (decimal)CurentRegistration.PtInsuranceBenefit;
                                    //}

                                    decimal PtHiBenefit = (decimal)CurentRegistration.PtInsuranceBenefit;
                                    if (outwardDrug.LimQtyAndHIPrice.ItemNumber2MaxBenefit > 0)
                                    {
                                        if (CurentRegistration.IsCrossRegion == false)
                                        {
                                            PtHiBenefit = outwardDrug.LimQtyAndHIPrice.ItemNumber2MaxBenefit;
                                        }
                                        else
                                        {
                                            PtHiBenefit = (decimal)Globals.ServerConfigSection.HealthInsurances.RebatePercentage2015Level1_InPt;
                                        }
                                    }
                                    outwardDrug.TotalHIPayment = Math.Min(outwardDrug.LimQtyAndHIPrice.ItemNumber2MaxPayAmt, outwardDrug.ChargeableItem.HIAllowedPrice.GetValueOrDefault(0) * (decimal)outwardDrug.HIPaymentPercent) * PtHiBenefit;
                                    //▲===== #010
                                    outwardDrug.TotalCoPayment = outwardDrug.HIAllowedPrice.GetValueOrDefault(0) - outwardDrug.TotalHIPayment;
                                    outwardDrug.TotalPatientPayment = outwardDrug.TotalInvoicePrice - outwardDrug.TotalHIPayment;
                                }
                                if (outwardDrug.LimQtyAndHIPrice != null && outwardDrug.CountValue == 1)
                                {
                                    decimal PtHiBenefit = (decimal)CurentRegistration.PtInsuranceBenefit;
                                    if (outwardDrug.LimQtyAndHIPrice.ItemNumber1MaxBenefit > 0)
                                    {
                                        if (CurentRegistration.IsCrossRegion == false)
                                        {
                                            PtHiBenefit = outwardDrug.LimQtyAndHIPrice.ItemNumber1MaxBenefit;
                                        }
                                        else
                                        {
                                            PtHiBenefit = (decimal)Globals.ServerConfigSection.HealthInsurances.RebatePercentage2015Level1_InPt;
                                        }
                                    }
                                    itemcount++;
                                    //outwardDrug.TotalHIPayment = Math.Min(outwardDrug.LimQtyAndHIPrice.ItemNumber1MaxPayAmt, outwardDrug.ChargeableItem.HIAllowedPrice.GetValueOrDefault(0) * (decimal)outwardDrug.HIPaymentPercent) * (decimal)CurentRegistration.PtInsuranceBenefit;
                                    outwardDrug.TotalHIPayment = Math.Min(outwardDrug.LimQtyAndHIPrice.ItemNumber1MaxPayAmt, outwardDrug.ChargeableItem.HIAllowedPrice.GetValueOrDefault(0) * (decimal)outwardDrug.HIPaymentPercent) * PtHiBenefit;
                                    outwardDrug.TotalCoPayment = outwardDrug.HIAllowedPrice.GetValueOrDefault(0) - outwardDrug.TotalHIPayment;
                                    outwardDrug.TotalPatientPayment = outwardDrug.TotalInvoicePrice - outwardDrug.TotalHIPayment;
                                }
                            }
                            /*==== #002 ====*/
                            //▲===== 
                            allItems.Add(outwardDrug);
                        }
                    }
                }
                if (IsResetView)
                    AllRegistrationItems = allItems;
                else
                    OldRegistrationItems = allItems;
            }
        }

        public void AddItemToView(MedRegItemBase item)
        {
            if (AllRegistrationItems == null || item == null)
            {
                return;
            }

            //KMx: Khi add thêm item, nếu BN không có bảo hiểm thì set giá BH = 0
            if (item.HIBenefit == null || item.HIBenefit == 0)
            {
                item.HIAllowedPrice = 0;
                item.IsCountSE = false;
            }

            item.CanDelete = CanDelete;
            //▼===== 20191217 TTM: Loại bỏ set của MaxQtyHIAllowItem vì không sử dụng nữa chuyển sang sử dụng DataEntity LimQtyHiItemMaxPaymtPerc thay thế để phân biệt và tính toán.
            /*==== #002 ====*/
            if (!IsNewCreateBill)
            {
                if (item.MaxQtyHIAllowItem > 1)
                {
                    item.IsCountHI = false;
                    item.TotalHIPayment = 0;
                }
            }
            else
            {
                if (item is OutwardDrugClinicDept && (item as OutwardDrugClinicDept).LimQtyAndHIPrice != null)
                {
                    item.IsCountHI = false;
                    item.TotalHIPayment = 0;
                }
            }
            if (Globals.ServerConfigSection.InRegisElements.NotCountHIOnPackItem && HIBenefit != null && item.IsCountHI && item.IsInPackage)
            {
                item.IsCountHI = false;
                item.TotalHIPayment = 0;
            }
            /*==== #002 ====*/
            //▲===== 
            AllRegistrationItems.Add(item);

            /*▼====: #003*/
            CalcInvoiceItem(item);
            /*▲====: #003*/
        }

        //public void AddItemToView(MedRegItemBase item)
        //{
        //    if (AllRegistrationItems != null)
        //    {
        //        item.HIAllowedPrice = hiBenefit.HasValue ? item.ChargeableItem.HIAllowedPrice : 0;

        //        item.CanDelete = CanDelete;
        //        AllRegistrationItems.Add(item);
        //    }
        //}

        public void RemoveItemFromView(MedRegItemBase item)
        {
            if (AllRegistrationItems != null)
            {
                AllRegistrationItems.Remove(item);
            }
        }
        public void GetBillingInvoiceById(long invID, Action<InPatientBillingInvoice> callback = null)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                //IsLoading = true;
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetInPatientBillingInvoice(invID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    BillingInvoice = contract.EndGetInPatientBillingInvoice(asyncResult);
                                    ResetView();
                                    if (callback != null)
                                    {
                                        callback(BillingInvoice);
                                    }

                                }
                                catch (FaultException<AxException> fault)
                                {
                                    error = new AxErrorEventArgs(fault);
                                }
                                catch (Exception ex)
                                {
                                    error = new AxErrorEventArgs(ex);
                                }
                                //finally
                                //{
                                //    if (BillingInvoice.IsHighTechServiceBill)
                                //    {
                                //        //HPT: dùng sự kiện này nhận biết đã load thông tin bill xong để tiếp tục load thông tin quỹ hỗ trợ. 
                                //        //Do khi load quỹ có tính lại tổng bill nên thông tin bill phải load đầy đủ mới tính đúng. Nếu không, sẽ bị thông báo nhắc là thông tin quỹ được cập nhật do bill thay đổi
                                //        Globals.EventAggregator.Publish(new LoadBillingDetailsCompleted());
                                //    }
                                //}
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {
                    //IsLoading = false;
                    this.HideBusyIndicator();
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                }
            });
            t.Start();
        }


        public void RemoveItemCmd(MedRegItemBase item, object eventArgs)
        {
            Coroutine.BeginExecute(CoroutineRemoveItem(item, eventArgs));
        }


        WarningWithConfirmMsgBoxTask warnOfDeleteProduct = null;

        public IEnumerator<IResult> CoroutineRemoveItem(MedRegItemBase item, object eventArgs)
        {
            bool exists = false;

            if (BillingInvoice == null || item == null)
            {
                yield break;
            }

            if (item is PatientRegistrationDetail)
            {
                var detailsItem = item as PatientRegistrationDetail;
                //▼===== #009:  Hiện tại cho phép huỷ dịch vụ đã hoàn tất ra khỏi bill
                //              1. Nếu dịch vụ không chỉ định từ tạo bill: Mark xoá, đánh dấu status là đã huỷ và gỡ khỏi bill
                //              2. Nếu dịch vụ được chỉ định từ y lệnh: chỉ gỡ ra khỏi bill.
                //20190404 TTM: Kiểm tra ở đây vì hiện tại khi gỡ dịch vụ ra khỏi bill sẽ xoá trắng dịch vụ đó khỏi bảng PatientRegistrationDetails_Inpt
                //              => Khi 1 dịch vụ thủ thuật đã thực hiện rồi => Điều dưỡng vào lỡ tay gỡ ra khỏi bill => Thông tin thực hiện thủ thuật vẫn còn lưu tại SmallProcedure nhưng thông tin dịch 
                //              vụ đó đã bị xoá khỏi bảng PatientRegistrationDetails_Inpt => Sai và sẽ làm mất tiền bệnh viện.
                //if (detailsItem.V_ExamRegStatus == (long)V_ExamRegStatus.mHoanTat)
                //{
                //    MessageBox.Show(eHCMSResources.Z2638_G1_KhongTheGoDichVuDaHoanTat);
                //    yield break;
                //}
                //▲===== #009


                if (detailsItem.RecordState == RecordState.ADDED || detailsItem.RecordState == RecordState.DETACHED)
                {
                    exists = BillingInvoice.RegistrationDetails.Remove(detailsItem);
                }
                else
                {
                    var temp = BillingInvoice.RegistrationDetails.Where(obj => obj == detailsItem).FirstOrDefault();
                    if (temp != null)
                    {
                        exists = true;
                        temp.RecordState = RecordState.DELETED;
                    }
                }
            }
            else if (item is PatientPCLRequestDetail)
            {
                //HPT 13/09/2016: Khi remove một dịch vụ cận lâm sàng từ bill, nếu dịch vụ đó thuộc một phiếu chỉ định có nhiều dịch vụ thì remove hết các dịch vụ trong phiếu đó luôn!
                if (BillingInvoice.PclRequests == null)
                {
                    yield break;
                }

                var detailsItem = item as PatientPCLRequestDetail;

                if (detailsItem.PCLReqItemID <= 0 && detailsItem.PatientPCLReqID <= 0)
                {
                    BillingInvoice.PclRequests.FirstOrDefault(x => x.PatientPCLReqID == 0).PatientPCLRequestIndicators.Remove(detailsItem);
                    RemoveItemFromView(detailsItem);
                    yield break;
                }
                PatientPCLRequest request = null;
                if (detailsItem.PatientPCLReqID >= 0)
                {
                    request = BillingInvoice.PclRequests.FirstOrDefault(x => x.PatientPCLReqID == detailsItem.PatientPCLReqID);
                    if (request.RequestCreatedFrom == (long)AllLookupValues.PCLRequestCreatedFrom.FROM_BILLINGINV)
                    {
                        detailsItem.RecordState = RecordState.DELETED;
                        RemoveItemFromView(detailsItem);
                        request.RecordState = RecordState.MODIFIED;
                        yield break;
                    }
                    if (request.RequestCreatedFrom == (long)AllLookupValues.PCLRequestCreatedFrom.FROM_PCLREQUEST)
                    {
                        if (request.PatientPCLRequestIndicators.Count() > 1)
                        {
                            warnOfDeleteProduct = new WarningWithConfirmMsgBoxTask(string.Format(eHCMSResources.Z1268_G1_XoaCTietPhChiDinh, request.PCLRequestNumID), eHCMSResources.G0442_G1_TBao);
                            yield return warnOfDeleteProduct;
                            if (!warnOfDeleteProduct.IsAccept)
                            {
                                warnOfDeleteProduct = null;
                                yield break;
                            }
                            warnOfDeleteProduct = null;
                        }
                        foreach (PatientPCLRequestDetail detail in request.PatientPCLRequestIndicators)
                        {
                            if (request.RecordState != RecordState.ADDED || request.RecordState != RecordState.DETACHED)
                            {
                                detail.RecordState = RecordState.DELETED;
                            }
                            RemoveItemFromView(detail);
                        }
                        if (request.InPatientBillingInvID == null || request.InPatientBillingInvID <= 0)
                        {
                            BillingInvoice.PclRequests.Remove(request);
                        }
                        else
                        {
                            request.RecordState = RecordState.MODIFIED;
                        }
                        yield break;
                    }
                    else
                    {
                        MessageBox.Show(eHCMSResources.A0544_G1_Msg_InfoEnglish2);
                        yield break;
                    }
                }
            }
            else
            {
                if (BillingInvoice.OutwardDrugClinicDeptInvoices == null)
                {
                    yield break;
                }

                var detailsItem = item as OutwardDrugClinicDept;
                if (detailsItem.outiID > 0)
                {
                    warnOfDeleteProduct = new WarningWithConfirmMsgBoxTask(string.Format(eHCMSResources.Z1269_G1_I, detailsItem.DrugInvoice.OutInvID), eHCMSResources.K3847_G1_DongY);
                    yield return warnOfDeleteProduct;
                    if (!warnOfDeleteProduct.IsAccept)
                    {
                        warnOfDeleteProduct = null;
                        yield break;
                    }
                    warnOfDeleteProduct = null;
                }

                foreach (var inv in BillingInvoice.OutwardDrugClinicDeptInvoices)
                {
                    if (inv.outiID != detailsItem.outiID || inv.OutwardDrugClinicDepts == null)
                    {
                        continue;
                    }

                    //KMx: Xóa những dòng xuất chung 1 phiếu xuất ra khỏi view (02/01/2016 16:14).
                    //Báo giá thì không xóa hết y cụ, chỉ xóa thằng nào đc yêu cầu xóa.
                    if (!IsQuotationView)
                    {
                        foreach (OutwardDrugClinicDept value in inv.OutwardDrugClinicDepts)
                        {
                            RemoveItemFromView(value);
                        }


                        //KMx: Nếu PX chưa nằm trong bill thì xóa phiếu xuất đó khỏi bill trên giao diện. Ngược lại thì đổi RecordState = DELETED để xóa PX đó ra khỏi bill dưới DB (02/01/2016 16:10).
                        if (inv.InPatientBillingInvID <= 0)
                        {
                            BillingInvoice.OutwardDrugClinicDeptInvoices.Remove(inv);
                        }
                        else
                        {
                            inv.RecordState = RecordState.DELETED;
                        }
                    }
                    else
                    {
                        RemoveItemFromView(detailsItem);
                    }
                    break;
                }
            }
            if (exists)
            {
                RemoveItemFromView(item);
            }

            Globals.EventAggregator.Publish(new ItemRemoved<MedRegItemBase, IInPatientBillingInvoiceDetailsListing>() { Item = item, Source = this });

        }


        //public IEnumerator<IResult> CoroutineRemoveItem(MedRegItemBase item, object eventArgs)
        //{
        //    bool exists = false;

        //    if (BillingInvoice == null || item == null)
        //    {
        //        yield break;
        //    }

        //    if (item is PatientRegistrationDetail)
        //    {
        //        var detailsItem = item as PatientRegistrationDetail;
        //        if (detailsItem.RecordState == RecordState.ADDED || detailsItem.RecordState == RecordState.DETACHED)
        //        {
        //            exists = BillingInvoice.RegistrationDetails.Remove(detailsItem);
        //        }
        //        else
        //        {
        //            var temp = BillingInvoice.RegistrationDetails.Where(obj => obj == detailsItem).FirstOrDefault();
        //            if (temp != null)
        //            {
        //                exists = true;
        //                temp.RecordState = RecordState.DELETED;
        //            }
        //        }
        //    }
        //    else if (item is PatientPCLRequestDetail)
        //    {
        //        if (BillingInvoice.PclRequests == null)
        //        {
        //            yield break;
        //        }

        //        var detailsItem = item as PatientPCLRequestDetail;

        //        foreach (var request in BillingInvoice.PclRequests)
        //        {
        //            if (request.PatientPCLRequestIndicators == null)
        //            {
        //                continue;
        //            }

        //            PatientPCLRequestDetail temp = null;
        //            PatientPCLRequest parent = null;
        //            foreach (var requestDetail in request.PatientPCLRequestIndicators)
        //            {
        //                if (requestDetail == detailsItem)
        //                {
        //                    temp = requestDetail;
        //                    parent = request;
        //                }
        //            }

        //            if (temp != null)
        //            {
        //                exists = true;
        //                if (temp.RecordState == RecordState.ADDED || temp.RecordState == RecordState.DETACHED)
        //                {
        //                    parent.PatientPCLRequestIndicators.Remove(temp);
        //                }
        //                else
        //                {
        //                    temp.RecordState = RecordState.DELETED;
        //                    request.RecordState = RecordState.MODIFIED;
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (BillingInvoice.OutwardDrugClinicDeptInvoices == null)
        //        {
        //            yield break;
        //        }

        //        var detailsItem = item as OutwardDrugClinicDept;
        //        if (detailsItem.outiID > 0)
        //        {
        //            warnOfDeleteProduct = new WarningWithConfirmMsgBoxTask(string.Format(eHCMSResources.Z1269_G1_I, detailsItem.DrugInvoice.OutInvID), eHCMSResources.G0442_G1_TBao);
        //            yield return warnOfDeleteProduct;
        //            if (!warnOfDeleteProduct.IsAccept)
        //            {
        //                warnOfDeleteProduct = null;
        //                yield break;
        //            }
        //            warnOfDeleteProduct = null;
        //        }

        //        List<OutwardDrugClinicDept> lst = new List<OutwardDrugClinicDept>();
        //        // ObservableCollection<OutwardDrugClinicDeptInvoice> lstInvoice = BillingInvoice.OutwardDrugClinicDeptInvoices.DeepCopy();
        //        foreach (var inv in BillingInvoice.OutwardDrugClinicDeptInvoices)
        //        {
        //            if (inv.OutwardDrugClinicDepts == null)
        //            {
        //                continue;
        //            }

        //            OutwardDrugClinicDeptInvoice parent = inv;
        //            //KMx: Nếu như xóa 1 dòng trong phiếu xuất thì tự dộng xóa hết tất cả các dòng trong cùng phiếu xuất. Để kho nội trú dễ chỉnh sửa phiếu xuất (23/08/2014 17:01).

        //            lst = inv.OutwardDrugClinicDepts.Where(x => x.outiID > 0 && x.outiID == detailsItem.outiID).ToList();

        //            if (lst != null && lst.Count > 0)
        //            {
        //                //KMx: Cập nhật trạng thái delete để xóa invoice khỏi bill.
        //                parent.RecordState = RecordState.DELETED;
        //            }
        //            else
        //            {
        //                //KMx: Nếu như dòng đó chưa nằm trong phiếu xuất (chưa lưu) thì xóa dòng đó thôi (20/08/2014 17:18).
        //                lst = inv.OutwardDrugClinicDepts.Where(x => x == detailsItem).ToList();
        //            }

        //            if (parent != null && (lst != null && lst.Count > 0))
        //            {
        //                foreach (OutwardDrugClinicDept value in lst)
        //                {
        //                    //KMx: Chỉ xóa trên giao diện thôi, còn list đi lưu thì vẩn giữ lại để cập nhật tất cả outward từ trong gói thành ngoài gói, để kho phòng không hiển thị số tiền âm (30/12/2015 14:46).
        //                    //parent.OutwardDrugClinicDepts.Remove(value);
        //                    RemoveItemFromView(value);
        //                }
        //            }

        //        }

        //        //KMx: Nếu như bill đã lưu rồi, thì lấy luôn Invoices có RecordState = DELETED để xuống SQL tách phiếu xuất đó ra khỏi bill (23/09/2014 15:24).
        //        if (BillingInvoice.InPatientBillingInvID > 0)
        //        {
        //            BillingInvoice.OutwardDrugClinicDeptInvoices = BillingInvoice.OutwardDrugClinicDeptInvoices.Where(x => x.outiID > 0 || x.OutwardDrugClinicDepts.Count > 0).ToObservableCollection();

        //        }
        //        //KMx: Ngược lại nếu bill chưa lưu, thì KHÔNG lấy những Invoice có RecordState = DELETED, nếu không SQL sẽ lưu phiếu xuất đó vào bill luôn (23/09/2014 15:25).
        //        else
        //        {
        //            BillingInvoice.OutwardDrugClinicDeptInvoices = BillingInvoice.OutwardDrugClinicDeptInvoices.Where(x => (x.outiID > 0 || x.OutwardDrugClinicDepts.Count > 0) && x.RecordState != RecordState.DELETED).ToObservableCollection();
        //        }
        //    }
        //    if (exists)
        //    {
        //        RemoveItemFromView(item);
        //    }

        //    Globals.EventAggregator.Publish(new ItemRemoved<MedRegItemBase, IInPatientBillingInvoiceDetailsListing>() { Item = item, Source = this });

        //}



        //public void RemoveItemCmd(MedRegItemBase item, object eventArgs)
        //{
        //    bool exists = false;

        //    if (BillingInvoice == null || item == null)
        //    {
        //        return;
        //    }

        //    if (item is PatientRegistrationDetail)
        //    {
        //        var detailsItem = item as PatientRegistrationDetail;
        //        if (detailsItem.RecordState == RecordState.ADDED || detailsItem.RecordState == RecordState.DETACHED)
        //        {
        //            exists = BillingInvoice.RegistrationDetails.Remove(detailsItem);
        //        }
        //        else
        //        {
        //            var temp = BillingInvoice.RegistrationDetails.Where(obj => obj == detailsItem).FirstOrDefault();
        //            if (temp != null)
        //            {
        //                exists = true;
        //                temp.RecordState = RecordState.DELETED;
        //            }
        //        }
        //    }
        //    else if (item is PatientPCLRequestDetail)
        //    {
        //        if (BillingInvoice.PclRequests == null)
        //        {
        //            return;
        //        }

        //        var detailsItem = item as PatientPCLRequestDetail;

        //        foreach (var request in BillingInvoice.PclRequests)
        //        {
        //            if (request.PatientPCLRequestIndicators == null)
        //            {
        //                continue;
        //            }

        //            PatientPCLRequestDetail temp = null;
        //            PatientPCLRequest parent = null;
        //            foreach (var requestDetail in request.PatientPCLRequestIndicators)
        //            {
        //                if (requestDetail == detailsItem)
        //                {
        //                    temp = requestDetail;
        //                    parent = request;
        //                }
        //            }

        //            if (temp != null)
        //            {
        //                exists = true;
        //                if (temp.RecordState == RecordState.ADDED || temp.RecordState == RecordState.DETACHED)
        //                {
        //                    parent.PatientPCLRequestIndicators.Remove(temp);
        //                }
        //                else
        //                {
        //                    temp.RecordState = RecordState.DELETED;
        //                    request.RecordState = RecordState.MODIFIED;
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (BillingInvoice.OutwardDrugClinicDeptInvoices == null)
        //        {
        //            return;
        //        }

        //        var detailsItem = item as OutwardDrugClinicDept;

        //        List<OutwardDrugClinicDept> lst = new List<OutwardDrugClinicDept>();
        //        // ObservableCollection<OutwardDrugClinicDeptInvoice> lstInvoice = BillingInvoice.OutwardDrugClinicDeptInvoices.DeepCopy();
        //        foreach (var inv in BillingInvoice.OutwardDrugClinicDeptInvoices)
        //        {
        //            if (inv.OutwardDrugClinicDepts == null)
        //            {
        //                continue;
        //            }

        //            OutwardDrugClinicDept temp = null;
        //            OutwardDrugClinicDeptInvoice parent = null;
        //          //KMx: Nếu như xóa 1 dòng trong phiếu xuất là xóa hết tất cả các dòng trong phiếu xuất (trường hợp phiếu xuất đó chưa được tạo bill).
        //            foreach (var outwardDrug in inv.OutwardDrugClinicDepts)
        //            {
        //                if (outwardDrug.outiID > 0 && outwardDrug.outiID == detailsItem.outiID && (inv.InPatientBillingInvID.GetValueOrDefault(0) == 0))
        //                {
        //                    lst.Add(outwardDrug);
        //                    parent = inv;

        //                }
        //            }

        //            if (lst == null || lst.Count <= 0)
        //            {
        //              //KMx: Trường hợp phiếu xuất đã tạo bill thì chỉ xóa 1 dòng thôi.
        //                foreach (var outwardDrug in inv.OutwardDrugClinicDepts)
        //                {
        //                    if ((outwardDrug.OutID > 0 && detailsItem.OutID == outwardDrug.OutID) || outwardDrug == detailsItem)
        //                    {
        //                        temp = outwardDrug;
        //                        parent = inv;
        //                    }
        //                }
        //            }

        //            if (parent != null && (temp != null || (lst != null && lst.Count > 0)))
        //            {
        //                if (lst != null && lst.Count() > 0)
        //                {
        //                    foreach (OutwardDrugClinicDept value in lst)
        //                    {
        //                        parent.OutwardDrugClinicDepts.Remove(value);
        //                        RemoveItemFromView(value);
        //                    }
        //                    if (parent.OutwardDrugClinicDepts == null || parent.OutwardDrugClinicDepts.Count == 0)
        //                    {
        //                        parent.RecordState = RecordState.DELETED;
        //                        // BillingInvoice.OutwardDrugClinicDeptInvoices.Remove(parent);
        //                    }
        //                }
        //                else
        //                {
        //                    if (temp.RecordState == RecordState.ADDED || temp.RecordState == RecordState.DETACHED)
        //                    {
        //                        parent.OutwardDrugClinicDepts.Remove(temp);
        //                        RemoveItemFromView(temp);
        //                        if (parent.OutwardDrugClinicDepts == null || parent.OutwardDrugClinicDepts.Count == 0)
        //                        {
        //                            parent.RecordState = RecordState.DELETED;
        //                            //BillingInvoice.OutwardDrugClinicDeptInvoices.Remove(parent);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        temp.RecordState = RecordState.DELETED;
        //                        parent.RecordState = RecordState.MODIFIED;
        //                    }

        //                }

        //            }
        //        }
        //        BillingInvoice.OutwardDrugClinicDeptInvoices = BillingInvoice.OutwardDrugClinicDeptInvoices.Where(x => x.RecordState != RecordState.DELETED).ToObservableCollection();
        //    }
        //    if (exists)
        //    {
        //        RemoveItemFromView(item);
        //    }

        //    Globals.EventAggregator.Publish(new ItemRemoved<MedRegItemBase, IInPatientBillingInvoiceDetailsListing>() { Item = item, Source = this });
        //}

        public void ModifiItemCmd(MedRegItemBase item)
        {
            if (BillingInvoice != null && item != null)
            {
                if (item is PatientRegistrationDetail)
                {
                    var detailsItem = item as PatientRegistrationDetail;
                    var temp = BillingInvoice.RegistrationDetails.Where(obj => obj.PtRegDetailID == detailsItem.PtRegDetailID && detailsItem.PtRegDetailID > 0).FirstOrDefault();
                    if (temp != null)
                    {
                        temp.RecordState = RecordState.MODIFIED;
                        detailsItem.RecordState = RecordState.MODIFIED;
                    }
                }
                //else if (item is PatientPCLRequestDetail)
                //{
                //    if (BillingInvoice.PclRequests != null)
                //    {
                //        var detailsItem = item as PatientPCLRequestDetail;
                //        foreach (var request in BillingInvoice.PclRequests)
                //        {
                //            if (request.PatientPCLRequestIndicators != null)
                //            {
                //                foreach (var requestDetail in request.PatientPCLRequestIndicators)
                //                {
                //                    if (requestDetail.PCLReqItemID == detailsItem.PCLReqItemID && detailsItem.PCLReqItemID > 0)
                //                    {
                //                        detailsItem.RecordState = RecordState.MODIFIED;
                //                        request.RecordState = RecordState.MODIFIED;
                //                        break;
                //                    }
                //                }

                //            }
                //        }
                //    }

                //}
                else if (item is PatientPCLRequestDetail)
                {
                    var detailsItem = item as PatientPCLRequestDetail;
                    if (detailsItem.PCLReqItemID > 0 && BillingInvoice.PclRequests != null)
                    {
                        foreach (var request in BillingInvoice.PclRequests)
                        {
                            if (request.PatientPCLRequestIndicators != null && request.PatientPCLRequestIndicators.Any(x => x.PCLReqItemID == detailsItem.PCLReqItemID))
                            {
                                detailsItem.RecordState = RecordState.MODIFIED;
                                request.RecordState = RecordState.MODIFIED;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (BillingInvoice.OutwardDrugClinicDeptInvoices != null)
                    {
                        var detailsItem = item as OutwardDrugClinicDept;
                        foreach (var inv in BillingInvoice.OutwardDrugClinicDeptInvoices)
                        {
                            //KMx: Phiếu xuất nào được tạo bên kho phòng và chưa cho vào bill, nếu có chỉnh sửa thì không đổi thành trạng thái MODIFIED, để khi cập nhật bill, phiếu xuất đó không nằm ở list modified mà phải nằm ở list saved (02/01/2016 15:01).
                            if (inv.InPatientBillingInvID == null || inv.InPatientBillingInvID <= 0 || inv.OutwardDrugClinicDepts == null || inv.OutwardDrugClinicDepts.Count <= 0)
                            {
                                continue;
                            }

                            foreach (var outwardDrug in inv.OutwardDrugClinicDepts)
                            {
                                if (outwardDrug.OutID > 0 && outwardDrug.OutID == detailsItem.OutID)
                                {
                                    inv.RecordState = RecordState.MODIFIED;
                                    detailsItem.RecordState = RecordState.MODIFIED;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            if (!IsQuotationView)
            {
                Globals.EventAggregator.Publish(new ItemRemoved<MedRegItemBase, IInPatientBillingInvoiceDetailsListing>() { Item = item, Source = this });
            }
        }

        public void LoadDetails()
        {
            if (BillingInvoice != null)
            {
                GetBillingInvoiceById(BillingInvoice.InPatientBillingInvID);
            }
        }
        public void LoadDetails(Action<InPatientBillingInvoice> callback)
        {
            if (BillingInvoice != null)
            {
                GetBillingInvoiceById(BillingInvoice.InPatientBillingInvID, callback);
            }
        }
        public void CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var item = e.Row.DataContext as MedRegItemBase;
            //KMx: Khi sửa số lượng, nếu dịch vụ đó đã lưu trong Database thì mới đổi trạng thái thành Modified.
            //Nếu không sẽ bị lỗi khi cập nhật bill cũ, thêm 1 dịch vụ, sửa SL DV đó thì không lưu DV đó được vì chương trình tự đổi trạng thái từ DETACHED sang MODIFIED (23/09/2014 16:45).
            if (item is PatientRegistrationDetail)
            {
                if ((item as PatientRegistrationDetail).PtRegDetailID > 0)
                {
                    item.RecordState = RecordState.MODIFIED;
                }
            }
            else if (item is PatientPCLRequestDetail)
            {
                if ((item as PatientPCLRequestDetail).PCLReqItemID > 0)
                {
                    //KMx: Từ khi thêm BSCĐ, Ngày y lệnh, ngày kết quả, phải đổi RecordState của cha luôn thì mới cập nhật được (26/04/2016 17:10).
                    //item.RecordState = RecordState.MODIFIED;
                    ModifiItemCmd(item);
                }
            }

            else if (item is OutwardDrugClinicDept)
            {
                var inv = item as OutwardDrugClinicDept;
                //inv.Qty = inv.OutQuantity - inv.QtyReturn;
                inv.OutQuantity = inv.Qty + inv.QtyReturn;

                if (inv.DrugInvoice != null && inv.DrugInvoice.outiID > 0)
                {
                    inv.DrugInvoice.RecordState = RecordState.MODIFIED;
                }
            }
            if (item != null && !item.HasErrors)
            {
                //▼====== #007
                CalcInvoiceItem(item, true);
                //▲====== #007
            }
            if (item.IsDiscounted)
            {
                CurentRegistration.ApplyDiscount(item, Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, false);
            }
            Globals.EventAggregator.Publish(new ItemChanged<MedRegItemBase, IInPatientBillingInvoiceDetailsListing>() { Item = item, Source = this });
        }

        //public void CellEditEnded(object sender, DataGridCellEditEndedEventArgs e)
        //{
        //    var item = e.Row.DataContext as MedRegItemBase;
        //    item.RecordState = RecordState.MODIFIED;

        //    if (item is OutwardDrugClinicDept)
        //    {
        //        var inv = item as OutwardDrugClinicDept;
        //        //inv.Qty = inv.OutQuantity - inv.QtyReturn;
        //        inv.OutQuantity = inv.Qty + inv.QtyReturn;

        //        if (inv.DrugInvoice != null)
        //        {
        //            inv.DrugInvoice.RecordState = RecordState.MODIFIED;
        //        }
        //    }
        //    if (item != null && e.Row.IsValid)
        //    {
        //        CalcInvoiceItem(item, item.HIBenefit);
        //    }
        //    Globals.EventAggregator.Publish(new ItemChanged<MedRegItemBase, IInPatientBillingInvoiceDetailsListing>() { Item = item, Source = this });
        //}

        public void BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            //KMx: Trước đây Dịch vụ chỉ cho sửa số lượng của Giường bệnh. Nhưng sau này A.Tuấn kiu cho sửa số lượng của tất cả dịch vụ (23/09/2014 08:37).
            object ctx = e.Row.DataContext;
            //CMN: Ngăn không cho chỉnh sửa thông tin Y lệnh các dịch vụ chỉ định từ Y lệnh tại màn hình tạo bill
            if ((ctx is PatientRegistrationDetail) && (ctx as PatientRegistrationDetail) != null && (ctx as PatientRegistrationDetail).IntPtDiagDrInstructionID.GetValueOrDefault(0) > 0 &&
                (e.Column.Equals(grid.GetColumnByName("colMedicalInstructionDate")) || e.Column.Equals(grid.GetColumnByName("colDoctorStaff"))))
            {
                e.Cancel = true;
            }
            else if (ctx is PatientPCLRequestDetail)
            {
                //▼====== #006
                //if (e.Column.GetValue(FrameworkElement.NameProperty).ToString() == "colQty")
                if (e.Column.Equals(grid.GetColumnByName("colQty")))
                {
                    MessageBox.Show(string.Format("{0}.", eHCMSResources.A0263_G1_Msg_InfoKhDcSuaSluongCLS), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    e.Cancel = true;
                }
                //else if ((e.Column.GetValue(FrameworkElement.NameProperty).ToString() == "colMedicalInstructionDate" || e.Column.GetValue(FrameworkElement.NameProperty).ToString() == "colDoctorStaff") && (ctx as PatientPCLRequestDetail).PatientPCLReqID > 0)
                else if ((e.Column.Equals(grid.GetColumnByName("colMedicalInstructionDate")) || e.Column.Equals(grid.GetColumnByName("colDoctorStaff"))) && (ctx as PatientPCLRequestDetail).PatientPCLReqID > 0)
                {
                    e.Cancel = true;
                }
                //▲====== #006
            }
            //else if (ctx is PatientRegistrationDetail)
            //{
            //    var regDetail = (PatientRegistrationDetail)ctx;
            //    if (regDetail.BedPatientRegDetail == null)
            //    {
            //        e.Cancel = true;
            //    }
            //}
            else if (ctx is OutwardDrugClinicDept)
            {
                //KMx: Nếu thuốc đó đã lưu rồi thì không cho cập nhật, muốn cập nhật thì qua kho nội trú (24/08/2014 09:56).
                OutwardDrugClinicDept owd = (OutwardDrugClinicDept)ctx;
                if (owd.outiID > 0 || owd.OutID > 0)
                {
                    MessageBox.Show(eHCMSResources.K0081_G1_I, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    e.Cancel = true;
                }
            }
        }

        //private void CalcInvoiceItem(IInvoiceItem item, double? hiBenefit)
        //▼====== #007
        //private void CalcInvoiceItem(MedRegItemBase item)
        private void CalcInvoiceItem(MedRegItemBase item, bool IsCellEditEnding = false, bool IsFromCountHI = false, bool IsFromCountPatientCovid = false)
        //▲====== #007
        {
            //▼====: #016
            bool IsCountPatientCOVID = false;
            if (CurentRegistration.AdmissionInfo != null)
            {
                IsCountPatientCOVID = CurentRegistration.AdmissionInfo.IsTreatmentCOVID;
            }
            // =====▼ #005
            item.HIAllowedPrice = item.ChargeableItem.HIAllowedPrice.GetValueOrDefault();
            //▼====== #007
            if (!IsCellEditEnding)
            {
                //▼===== #011: Vì lý do hàm này xài chung giữa thuốc, dịch vụ, CLS nên phải tách ra nếu là dịch vụ thì nhân thêm tỷ lệ thanh toán dịch vụ (nếu có).
                if (item is PatientRegistrationDetail)
                {
                    // nếu từ chỗ tích chọn/ bỏ chọn covid thì chạy thêm phần này. k thì chạy code cũ
                    if (IsFromCountPatientCovid)
                    {
                        if (item.IsCountPatientCOVID)
                        {
                            if (item.ChargeableItem.HIAllowedPrice > 0)
                            {
                                item.InvoicePrice = (decimal)item.ChargeableItem.HIAllowedPrice * (decimal)(item as PatientRegistrationDetail).PaymentPercent;
                            }
                            else
                            {
                                item.InvoicePrice = item.ChargeableItem.NormalPrice * (decimal)(item as PatientRegistrationDetail).PaymentPercent;
                            }
                            item.IsCountHI = false;
                            item.OtherAmt = item.InvoicePrice * item.Qty;
                        }
                        else
                        {
                            if (item.IsCountHI && HIBenefit != null)
                            {
                                item.InvoicePrice = item.ChargeableItem.HIPatientPrice * (decimal)(item as PatientRegistrationDetail).PaymentPercent;
                            }
                            else if (HIBenefit.GetValueOrDefault(0) > 0 && item.ChargeableItem.HIPatientPrice > 0)
                            {
                                item.InvoicePrice = item.ChargeableItem.HIPatientPrice * (decimal)(item as PatientRegistrationDetail).PaymentPercent;
                                item.IsCountHI = true;
                            }
                            else
                            {
                                item.InvoicePrice = item.ChargeableItem.NormalPrice * (decimal)(item as PatientRegistrationDetail).PaymentPercent;
                            }
                            item.OtherAmt = 0;
                        }
                    }
                    // kiểm tra đăng ký được xác nhận là bn điều trị COVID thì tính cách mới không thì đi như cũ
                    else if (IsCountPatientCOVID)
                    {
                        // nếu DVKT/ thuốc nằm trong danh mục COVID thì
                        // sau 1 hồi nhìn code + vd thì cảm thấy không cần thay đổi gì hết. giữ nguyên
                        if (item.IsCountPatientCOVID)
                        {
                            if (item.ChargeableItem.HIAllowedPrice > 0)
                            {
                                item.InvoicePrice = (decimal)item.ChargeableItem.HIAllowedPrice * (decimal)(item as PatientRegistrationDetail).PaymentPercent;
                            }
                            else
                            {
                                item.InvoicePrice = item.ChargeableItem.NormalPrice * (decimal)(item as PatientRegistrationDetail).PaymentPercent;
                            }
                            item.IsCountHI = false;
                            item.OtherAmt = item.InvoicePrice * item.Qty;
                        }
                        else
                        {
                            if (item.IsCountHI && HIBenefit != null)
                            {
                                item.InvoicePrice = item.ChargeableItem.HIPatientPrice * (decimal)(item as PatientRegistrationDetail).PaymentPercent;
                            }
                            else if (HIBenefit.GetValueOrDefault(0) > 0 && item.ChargeableItem.HIPatientPrice > 0)
                            {
                                item.InvoicePrice = item.ChargeableItem.HIPatientPrice * (decimal)(item as PatientRegistrationDetail).PaymentPercent;
                            }
                            else
                            {
                                item.InvoicePrice = item.ChargeableItem.NormalPrice * (decimal)(item as PatientRegistrationDetail).PaymentPercent;
                            }
                        }
                    }
                    else
                    {
                        if (item.IsCountHI && HIBenefit != null)
                        {
                            item.InvoicePrice = item.ChargeableItem.HIPatientPrice * (decimal)(item as PatientRegistrationDetail).PaymentPercent;
                        }
                        else if (HIBenefit.GetValueOrDefault(0) > 0 && item.ChargeableItem.HIPatientPrice > 0)
                        {
                            item.InvoicePrice = item.ChargeableItem.HIPatientPrice * (decimal)(item as PatientRegistrationDetail).PaymentPercent;
                        }
                        else
                        {
                            item.InvoicePrice = item.ChargeableItem.NormalPrice * (decimal)(item as PatientRegistrationDetail).PaymentPercent;
                        }
                    }
                }
                else if (item is PatientPCLRequestDetail)
                {
                    // nếu từ chỗ tích chọn/ bỏ chọn covid thì chạy thêm phần này. k thì chạy code cũ
                    if (IsFromCountPatientCovid)
                    {
                        if (item.IsCountPatientCOVID)
                        {
                            if (item.ChargeableItem.HIAllowedPrice > 0)
                            {
                                item.InvoicePrice = (decimal)item.ChargeableItem.HIAllowedPrice;
                            }
                            else
                            {
                                item.InvoicePrice = item.ChargeableItem.NormalPrice;
                            }
                            item.IsCountHI = false;
                            item.IsCountPatientCOVID = true;
                            item.OtherAmt = item.InvoicePrice * item.Qty;
                        }
                        else
                        {
                            if (item.IsCountHI && HIBenefit != null)
                            {
                                item.InvoicePrice = item.ChargeableItem.HIPatientPrice;
                            }
                            else if (HIBenefit.GetValueOrDefault(0) > 0 && item.ChargeableItem.HIPatientPrice > 0)
                            {
                                item.InvoicePrice = item.ChargeableItem.HIPatientPrice;
                            }
                            else
                            {
                                item.InvoicePrice = item.ChargeableItem.NormalPrice;
                            }
                            item.IsCountPatientCOVID = false;
                            item.OtherAmt = 0;
                        }
                    }
                    // kiểm tra đăng ký được xác nhận là bn điều trị COVID thì tính cách mới không thì đi như cũ
                    else if (IsCountPatientCOVID)
                    {
                        if (item.IsCountPatientCOVID)
                        {
                            if (item.ChargeableItem.HIAllowedPrice > 0)
                            {
                                item.InvoicePrice = (decimal)item.ChargeableItem.HIAllowedPrice;
                            }
                            else
                            {
                                item.InvoicePrice = item.ChargeableItem.NormalPrice;
                            }
                            item.IsCountHI = false;
                            item.OtherAmt = item.InvoicePrice * item.Qty;
                        }
                        else
                        {
                            item.OtherAmt = 0;
                            item.InvoicePrice = item.IsCountHI && HIBenefit != null ? item.ChargeableItem.HIPatientPrice : (HIBenefit.GetValueOrDefault(0) > 0 && item.ChargeableItem.HIPatientPrice > 0 ? item.ChargeableItem.HIPatientPrice : item.ChargeableItem.NormalPrice);
                        }
                    }
                    else
                    {
                        item.OtherAmt = 0;
                        item.InvoicePrice = item.IsCountHI && HIBenefit != null ? item.ChargeableItem.HIPatientPrice : (HIBenefit.GetValueOrDefault(0) > 0 && item.ChargeableItem.HIPatientPrice > 0 ? item.ChargeableItem.HIPatientPrice : item.ChargeableItem.NormalPrice);
                    }
                }
                else if (item is OutwardDrugClinicDept)
                {
                    // nếu từ chỗ tích chọn/ bỏ chọn covid thì chạy thêm phần này. k thì chạy code cũ
                    if (IsFromCountPatientCovid)
                    {
                        if (item.IsCountPatientCOVID)
                        {
                            item.InvoicePrice = ((RefGenMedProductDetails)item.ChargeableItem).InCost;
                            item.IsCountHI = false;
                            item.IsCountPatientCOVID = true;
                            item.OtherAmt = item.InvoicePrice * item.Qty;
                        }
                        else
                        {
                            if (item.IsCountHI && HIBenefit != null)
                            {
                                item.InvoicePrice = item.ChargeableItem.HIPatientPrice;
                            }
                            else if (HIBenefit.GetValueOrDefault(0) > 0 && item.ChargeableItem.HIPatientPrice > 0)
                            {
                                item.InvoicePrice = item.ChargeableItem.HIPatientPrice;
                            }
                            else
                            {
                                item.InvoicePrice = item.ChargeableItem.NormalPrice;
                            }
                            item.OtherAmt = 0;
                            item.IsCountPatientCOVID = false;
                        }
                    }
                    else if (IsCountPatientCOVID)
                    {
                        if (item.IsCountPatientCOVID)
                        {
                            //▼====: #018
                            item.InvoicePrice = ((RefGenMedProductDetails)item.ChargeableItem).InCost;
                            //▲====: #018
                            item.IsCountHI = false;
                            item.OtherAmt = item.InvoicePrice * item.Qty;
                        }
                        else
                        {
                            item.OtherAmt = 0;
                            item.InvoicePrice = item.IsCountHI && HIBenefit != null ? item.ChargeableItem.HIPatientPrice : (HIBenefit.GetValueOrDefault(0) > 0 && item.ChargeableItem.HIPatientPrice > 0 ? item.ChargeableItem.HIPatientPrice : item.ChargeableItem.NormalPrice);
                        }
                    }
                    else
                    {
                        item.OtherAmt = 0;
                        item.InvoicePrice = item.IsCountHI && HIBenefit != null ? item.ChargeableItem.HIPatientPrice : (HIBenefit.GetValueOrDefault(0) > 0 && item.ChargeableItem.HIPatientPrice > 0 ? item.ChargeableItem.HIPatientPrice : item.ChargeableItem.NormalPrice);
                    }
                }
                else
                {
                    item.OtherAmt = 0;
                    item.InvoicePrice = item.IsCountHI && HIBenefit != null ? item.ChargeableItem.HIPatientPrice : (HIBenefit.GetValueOrDefault(0) > 0 && item.ChargeableItem.HIPatientPrice > 0 ? item.ChargeableItem.HIPatientPrice : item.ChargeableItem.NormalPrice);
                }
                //▲===== #011
            }
            //▲====== #007
            //▲====: #016
            item.PriceDifference = item.InvoicePrice - item.HIAllowedPrice.GetValueOrDefault(0);
            if (item.IsCountPatientCOVID)
            {
                item.OtherAmt = item.InvoicePrice * item.Qty;
            }
            bool bItemInstDateIsValidWithHICard_1 = false;
            bool bItemInstDateIsValidWithHICard_2 = false;
            bool bItemInstDateIsValidWithHICard_3 = false;
            if (CurentRegistration.HealthInsurance != null 
                && CurentRegistration.PtInsuranceBenefit > 0 
                && (item.MedicalInstructionDate >= CurentRegistration.HealthInsurance.ValidDateFrom
                //▼====: #021
                && item.MedicalInstructionDate <= CurentRegistration.HealthInsurance.ValidDateTo.GetValueOrDefault().AddDays(Globals.ServerConfigSection.InRegisElements.NumDayHIAgreeToPayAfterHIExpiresInPt)))
                //▲====: #021
            {
                bItemInstDateIsValidWithHICard_1 = true;
            }
            else
            {
                if (CurentRegistration.HealthInsurance_3 != null 
                    && item.MedicalInstructionDate >= CurentRegistration.HealthInsurance_3.ValidDateFrom
                    //▼====: #021
                    && item.MedicalInstructionDate <= CurentRegistration.HealthInsurance_3.ValidDateTo.GetValueOrDefault().AddDays(Globals.ServerConfigSection.InRegisElements.NumDayHIAgreeToPayAfterHIExpiresInPt))
                    //▲====: #021
                {
                    bItemInstDateIsValidWithHICard_3 = true;
                }
                else if (CurentRegistration.HealthInsurance_2 != null 
                    && item.MedicalInstructionDate >= CurentRegistration.HealthInsurance_2.ValidDateFrom
                    //▼====: #021
                    && item.MedicalInstructionDate <= CurentRegistration.HealthInsurance_2.ValidDateTo.GetValueOrDefault().AddDays(Globals.ServerConfigSection.InRegisElements.NumDayHIAgreeToPayAfterHIExpiresInPt))
                    //▲====: #021

                {
                    bItemInstDateIsValidWithHICard_2 = true;
                }
            }

            //if (!onlyRoundResultForOutward)
            //{
            //    item.TotalHIPayment = MathExt.Round(item.HIAllowedPrice.GetValueOrDefault(0) * (decimal)HIBenefit.GetValueOrDefault(0.0) * item.Qty, MidpointRounding.AwayFromZero);
            //}
            //else
            //{
            //    item.TotalHIPayment = item.HIAllowedPrice.GetValueOrDefault(0) * (decimal)HIBenefit.GetValueOrDefault(0.0) * item.Qty;
            //}

            /*▼====: #003*/
            if (item.IsCountHI && bItemInstDateIsValidWithHICard_1)
            {
                //item.TotalHIPayment = item.HIAllowedPrice.GetValueOrDefault(0) * (decimal)CurentRegistration.PtInsuranceBenefit.GetValueOrDefault(0.0) * item.Qty;
                //item.HIBenefit = CurentRegistration.PtInsuranceBenefit.GetValueOrDefault(0);
                //item.HisID = CurentRegistration.HisID;
                CalcTotalHIPaymentWithInsuranceBenefit(item, CurentRegistration.PtInsuranceBenefit, CurentRegistration.HisID);
            }
            else if (item.IsCountHI && bItemInstDateIsValidWithHICard_3)
            {
                //item.TotalHIPayment = item.HIAllowedPrice.GetValueOrDefault(0) * (decimal)CurentRegistration.PtInsuranceBenefit_3.GetValueOrDefault(0.0) * item.Qty;
                //item.HIBenefit = CurentRegistration.PtInsuranceBenefit_3.GetValueOrDefault(0.0);
                //item.HisID = CurentRegistration.HisID_3;
                CalcTotalHIPaymentWithInsuranceBenefit(item, CurentRegistration.PtInsuranceBenefit_3, CurentRegistration.HisID_3);
            }
            else if (item.IsCountHI && bItemInstDateIsValidWithHICard_2)
            {
                //item.TotalHIPayment = item.HIAllowedPrice.GetValueOrDefault(0) * (decimal)CurentRegistration.PtInsuranceBenefit_2.GetValueOrDefault(0.0) * item.Qty;
                //item.HIBenefit = CurentRegistration.PtInsuranceBenefit_2.GetValueOrDefault(0.0);
                //item.HisID = CurentRegistration.HisID_2;
                CalcTotalHIPaymentWithInsuranceBenefit(item, CurentRegistration.PtInsuranceBenefit_2, CurentRegistration.HisID_2);
            }
            else
            {
                item.HisID = null;
                item.HIBenefit = null;
                // =====▼ #005
                item.IsCountHI = false;
                item.TotalHIPayment = 0;
                // =====▲ #005
            }

            //▼=====20191219 TTM: Đổi code anh Công lại thành code của hàm RecalForLimProduct
            /*==== #002 ====*/
            if (Globals.ServerConfigSection.InRegisElements.NotCountHIOnPackItem && HIBenefit != null && item.IsCountHI && item.IsInPackage)
            {
                item.IsCountHI = false;
                item.TotalHIPayment = 0;
            }
            if (!IsNewCreateBill)
            {
                if (item.MaxQtyHIAllowItem > 1 && item.IsCountHI == true && item is OutwardDrugClinicDept)
                {
                    var ModifyItem = (item as OutwardDrugClinicDept);
                    var AddedItem = AllRegistrationItems.Where(x => x.ID != item.ID && x.IsCountHI && x is OutwardDrugClinicDept && (x as OutwardDrugClinicDept).MaxQtyHIAllowItem > 1);
                    if (AddedItem.Count() >= item.MaxQtyHIAllowItem)
                    {
                        ModifyItem.IsCountHI = false;
                        ModifyItem.TotalHIPayment = 0;
                        ModifyItem.HIPaymentPercent = 0;
                    }
                    else if (AddedItem.Count() > 1 && AddedItem.Count() < item.MaxQtyHIAllowItem)
                    {
                        if (AddedItem.Where(x => (x as OutwardDrugClinicDept).HIPaymentPercent == 1).Count() == 1)
                        {
                            ModifyItem.HIPaymentPercent = item.PaymentRateOfHIAddedItem;
                            ModifyItem.TotalHIPayment = Math.Min(ModifyItem.GenMedProductItem == null ? 0 : ModifyItem.GenMedProductItem.CeilingPrice2ndItem.GetValueOrDefault(0), item.ChargeableItem.HIAllowedPrice.GetValueOrDefault(0) * (decimal)item.PaymentRateOfHIAddedItem) * (CurentRegistration.IsCrossRegion.GetValueOrDefault(false) ? (decimal)Globals.ServerConfigSection.HealthInsurances.RebatePercentageLevel1 / (item.PaymentRateOfHIAddedItem != 0 ? (decimal)item.PaymentRateOfHIAddedItem.GetValueOrDefault(1) : 1.0m) : 1.0m);
                        }
                    }
                    else if (AddedItem.Count() == 1)
                    {
                        if ((AddedItem.FirstOrDefault() as OutwardDrugClinicDept).HIPaymentPercent.GetValueOrDefault(1) == 1)
                        {
                            ModifyItem.HIPaymentPercent = item.PaymentRateOfHIAddedItem;
                            ModifyItem.TotalHIPayment = Math.Min(ModifyItem.GenMedProductItem == null ? 0 : ModifyItem.GenMedProductItem.CeilingPrice2ndItem.GetValueOrDefault(0), item.ChargeableItem.HIAllowedPrice.GetValueOrDefault(0) * (decimal)item.PaymentRateOfHIAddedItem) * (CurentRegistration.IsCrossRegion.GetValueOrDefault(false) ? (decimal)Globals.ServerConfigSection.HealthInsurances.RebatePercentageLevel1 / (item.PaymentRateOfHIAddedItem != 0 ? (decimal)item.PaymentRateOfHIAddedItem.GetValueOrDefault(1) : 1.0m) : 1.0m);
                        }
                    }
                    else if (AddedItem.Count() == 0 && item.Qty == 1)
                    {
                        ModifyItem.HIPaymentPercent = 1;
                        ModifyItem.TotalHIPayment = item.Qty > 1 ? (item.TotalHIPayment / item.Qty) + Math.Min(ModifyItem.GenMedProductItem == null ? 0 : ModifyItem.GenMedProductItem.CeilingPrice2ndItem.GetValueOrDefault(0), item.ChargeableItem.HIAllowedPrice.GetValueOrDefault(0) * (decimal)item.PaymentRateOfHIAddedItem) : item.TotalHIPayment;
                    }
                    item = ModifyItem;
                }
                else if (item.MaxQtyHIAllowItem > 1 && item.IsCountHI == false && item is OutwardDrugClinicDept)
                {
                    var ModifyItem = (item as OutwardDrugClinicDept);
                    ModifyItem.HIPaymentPercent = 1;
                    foreach (var RevertItem in AllRegistrationItems.Where(x => x.ID != item.ID && x.IsCountHI && x is OutwardDrugClinicDept && (x as OutwardDrugClinicDept).GenMedProductID == (item as OutwardDrugClinicDept).GenMedProductID))
                    {
                        CalcInvoiceItem(RevertItem);
                        ModifiItemCmd(RevertItem);
                    }
                    item = ModifyItem;
                }
                else if (item is OutwardDrugClinicDept)
                {
                    var ModifyItem = (item as OutwardDrugClinicDept);
                    ModifyItem.HIPaymentPercent = 1;
                    item = ModifyItem;
                }
            }
            else
            {
                if (item is OutwardDrugClinicDept && (item as OutwardDrugClinicDept).LimQtyAndHIPrice != null)
                {
                    bool btValOrderType = item.IsCountHI ? true : false;
                    RecalForLimProduct(btValOrderType, IsFromCountHI);
                }
            }
            /*==== #002 ====*/
            //▲===== 

            //KMx: Nếu dịch vụ đó đã có trong gói rồi thì chỉ tính BH thôi, còn "Tổng tiền VT thu = 0" và "Tổng tiền BN trả = 0"
            if (item.IsInPackage)
            {
                item.TotalInvoicePrice = 0;
                item.TotalPatientPayment = 0;
            }
            else
            {
                //KMx: Nếu có tính cho BN thì Tổng tiền = Đơn giá * Số lượng. Ngược lại thì Tổng tiền = Tổng tiền BH trả (11/12/2014).
                if (item.IsCountPatient)
                {
                    item.TotalInvoicePrice = item.InvoicePrice * item.Qty;
                }
                else
                {
                    item.TotalInvoicePrice = item.TotalHIPayment;
                }

                //▼====: #016
                item.TotalPatientPayment = item.TotalInvoicePrice - item.TotalHIPayment - item.OtherAmt;
                //▲====: #016
            }
            /*▼====: #003*/
            // =====▼ #005
            //if (!onlyRoundResultForOutward)
            //    {
            //    item.TotalHIPayment = MathExt.Round(item.TotalHIPayment, MidpointRounding.AwayFromZero);
            //}
            // =====▲ #005
            if (CurentRegistration != null && item.IsCountHI && item.TotalHIPayment > 0 && item.HisID.GetValueOrDefault(0) == 0)
                item.HisID = CurentRegistration.HisID;
            /*▲====: #003*/
        }
        //20200511 TBL: Hàm tính lại TotalHIPayment theo Benefit của thẻ theo ngày y lệnh
        private void CalcTotalHIPaymentWithInsuranceBenefit(MedRegItemBase item, double? HIBenefit, long? HisID)
        {
            if (item is PatientRegistrationDetail)
            {
                PatientRegistrationDetail PtRegDetail = item as PatientRegistrationDetail;
                if (PtRegDetail.V_EkipIndex != null && PtRegDetail.V_EkipIndex.LookupID == (long)AllLookupValues.V_EkipIndex.CungEkip)
                {
                    item.TotalHIPayment = item.HIAllowedPrice.GetValueOrDefault(0) * (decimal)HIBenefit * item.Qty * (decimal)Globals.ServerConfigSection.HealthInsurances.PercentForEkip;
                }
                else if (PtRegDetail.V_EkipIndex != null && PtRegDetail.V_EkipIndex.LookupID == (long)AllLookupValues.V_EkipIndex.KhacEkip)
                {
                    item.TotalHIPayment = item.HIAllowedPrice.GetValueOrDefault(0) * (decimal)HIBenefit * item.Qty * (decimal)Globals.ServerConfigSection.HealthInsurances.PercentForOtherEkip;
                }
                else
                {
                    //▼===== #011: Vì lý do hàm này xài chung giữa thuốc, dịch vụ, CLS nên phải tách ra nếu là dịch vụ thì nhân thêm tỷ lệ thanh toán dịch vụ (nếu có).
                    item.TotalHIPayment = item.HIAllowedPrice.GetValueOrDefault(0) * (decimal)HIBenefit * item.Qty * (decimal)(item as PatientRegistrationDetail).HIPaymentPercent;
                    //▲===== #011
                }
            }
            else
            {
                item.TotalHIPayment = item.HIAllowedPrice.GetValueOrDefault(0) * (decimal)HIBenefit * item.Qty;
            }
            item.HIBenefit = HIBenefit.GetValueOrDefault(0);
            item.HisID = HisID;
        }

        public bool ValidateInfo(out ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults)
        {
            bool bValid = true;
            validationResults = null;
            if (AllRegistrationItems != null)
            {
                foreach (var item in AllRegistrationItems)
                {
                    if (item is PatientRegistrationDetail)
                    {
                        //Neu validate binh thuong thi se doi hoi nhap DeptLocation (vi dung chung ben Ngoai tru).
                        //Chi can validate so luong thoi.
                        var detail = (PatientRegistrationDetail)item;
                        if (detail.Qty <= 0)
                        {
                            var validationResult = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z1438_G1_SLgKgHopLe, new string[] { "PatientRegistrationDetails" });
                            validationResults = new ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult>();
                            validationResults.Add(validationResult);
                            bValid = false;
                        }
                    }
                    else
                    {
                        bValid = ValidationExtensions.ValidateObject(item, out validationResults);
                    }

                    if (!bValid)
                    {
                        break;
                    }
                }
            }

            return bValid;
        }

        // Hpt: double Click vào cột phòng thì cho update Phòng dịch vụ
        // Hàm này tạm thời không dùng đến, thay bằng hàm gridRegDetails_Click để điều chỉnh giá. Comment để đây khi nào cần thì dùng (13/11/2015 14:44) 
        public void grid_DblClick(object source, object args)
        {
            var eventArgs = args as EventArgs<object>;
            if (eventArgs == null)
            {
                return;
            }

            var regDetail = eventArgs.Value as PatientRegistrationDetail;
            if (regDetail != null
                && regDetail.BedPatientRegDetail != null
                && regDetail.BedPatientRegDetail.BedPatientID > 0)
            {
                Action<IInPatientBedRegDetailListing> onInitDlg = delegate (IInPatientBedRegDetailListing vm)
                {
                    vm.BedPatientID = regDetail.BedPatientRegDetail.BedPatientID;
                };
                GlobalsNAV.ShowDialog<IInPatientBedRegDetailListing>(onInitDlg);
            }
            else/*Bằng thêm vào*/
            {
                var pclReqDetail = eventArgs.Value as PatientPCLRequestDetail;
                if (pclReqDetail != null && pclReqDetail.PCLExamType != null && pclReqDetail.PCLExamType.PCLExamTypeID > 0)
                {
                    Action<IPCLRequestDetail> onInitDlg = delegate (IPCLRequestDetail vm)
                    {
                        vm.PCLRequestDetail = eventArgs.Value as PatientPCLRequestDetail;
                    };
                    GlobalsNAV.ShowDialog<IPCLRequestDetail>(onInitDlg);
                }
            }

        }

        // Hpt 13/11/2015:
        // Lấy ra index của cột hiện hành
        //DataGridColumn currentColumn { get; set; }
        //public void gridServices_CurrentCellChanged(object sender, EventArgs e)
        //{
        //    if ((sender)!=null)
        //    {
        //        currentColumn = ((DataGrid)sender).CurrentColumn;
        //    }
        //}

        private MedRegItemBase _ModifyBillingInvItem;
        public MedRegItemBase ModifyBillingInvItem
        {
            get
            {
                return _ModifyBillingInvItem;
            }
            set
            {
                _ModifyBillingInvItem = value;
                NotifyOfPropertyChange(() => ModifyBillingInvItem);
            }
        }

        private AllLookupValues.PopupModifyPrice_Type _ModifyType;
        public AllLookupValues.PopupModifyPrice_Type ModifyType
        {
            get
            {
                return _ModifyType;
            }
            set
            {
                _ModifyType = value;
                NotifyOfPropertyChange(() => ModifyType);
            }
        }

        int index = 0;
        public void gridRegDetails_DbClick(object source, EventArgs<object> eventArgs)
        {
            //if (currentColumn == null)
            //{
            //    return;
            //}
            //if (currentColumn.DisplayIndex != 7 && currentColumn.DisplayIndex != 8) // 7 = Giá, 8= Giá BHYT: Anh tuấn nói cho sửa cả giá BHYT
            //{
            //    return;
            //}

            if (source == null)
            {
                return;
            }

            if (((DataGrid)source).CurrentColumn == null) return;
            if (((DataGrid)source).CurrentColumn != ((DataGrid)source).GetColumnByName("colInvoicePrice") && ((DataGrid)source).CurrentColumn != ((DataGrid)source).GetColumnByName("colHIAllowedPrice"))
            {
                return;
            }

            // TxD 15/07/2016 Commented the following because at the moment it caused problem to pop up the price dialog
            //if (!CanShowPopupToModifyPrice)
            //{
            //    return;
            //}

            ModifyBillingInvItem = eventArgs.Value as MedRegItemBase;

            index = AllRegistrationItems.IndexOf(ModifyBillingInvItem);

            if (ModifyBillingInvItem.V_NewPriceType != (Int32)AllLookupValues.V_NewPriceType.Unknown_PriceType && ModifyBillingInvItem.V_NewPriceType != (Int32)AllLookupValues.V_NewPriceType.Updatable_PriceType)
            {
                MessageBox.Show(ModifyBillingInvItem.ChargeableItemName + string.Format(" {0}", eHCMSResources.W0950_G1_W));
                return;
            }

            ModifyBillingInvItem.IsModItemOK = false;

            void onInitDlg(IModifyBillingInvItem vm)
            {
                vm.SetParentInPtBillingInvDetailsLst(this);
                vm.ModifyBillingInvItem = ModifyBillingInvItem;
                vm.PopupType = 2;
                vm.Init();
            }
            GlobalsNAV.ShowDialog<IModifyBillingInvItem>(onInitDlg);
        }

        private void BindBillingInvoiceType()
        {
            if (BillingInvoice != null)
            {
                if (BillingTypeContent.SelectedItem != null
                    && BillingTypeContent.SelectedItem.EnumItem != null
                    && Enum.IsDefined(typeof(AllLookupValues.V_BillingInvType), BillingTypeContent.SelectedItem.EnumItem))
                {
                    BillingInvoice.V_BillingInvType = (AllLookupValues.V_BillingInvType)BillingTypeContent.SelectedItem.EnumItem;
                    return;
                }
                BillingInvoice.V_BillingInvType = AllLookupValues.V_BillingInvType.TINH_TIEN_NOI_TRU;
            }

        }

        public bool CanEditBillingType
        {
            get
            {
                return CanEditOnGrid && BillingInvoice != null && BillingInvoice.InPatientBillingInvID <= 0;
            }
        }
        public bool CanEditDate
        {
            get
            {
                return CanEditOnGrid && BillingInvoice != null && BillingInvoice.InPatientBillingInvID <= 0;
            }
        }

        //public void HiApplied_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (HIBenefit.GetValueOrDefault(0) > 0)
        //    {
        //        MedRegItemBase item = sender as MedRegItemBase;
        //        if (item != null)
        //        {
        //            item.HIBenefit = HIBenefit;
        //            CalcInvoiceItem(item, item.HIBenefit);
        //            ModifiItemCmd(item);
        //        }
        //    }
        //}

        //public void HiApplied_UnChecked(object sender, RoutedEventArgs e)
        //{
        //    if (HIBenefit.GetValueOrDefault(0) > 0)
        //    {
        //        MedRegItemBase item = sender as MedRegItemBase;
        //        if (item != null)
        //        {
        //            item.HIBenefit = null;
        //            CalcInvoiceItem(item, item.HIBenefit);
        //            ModifiItemCmd(item);
        //        }
        //    }
        //}

        private bool _showInPackageColumn;
        public bool ShowInPackageColumn
        {
            get
            {
                return _showInPackageColumn;
            }
            set
            {
                _showInPackageColumn = value;
                NotifyOfPropertyChange(() => ShowInPackageColumn);
            }
        }

        DataGrid grid = null;
        public void grid_Loaded(object sender, RoutedEventArgs e)
        {
            grid = sender as DataGrid;

            if (grid == null)
            {
                return;
            }

            //var colIsInPackage = grid.GetColumnByName("colIsInPackage");
            var colIsInPackage = grid.Columns.FirstOrDefault(x => x.Header != null && x.Header.ToString().Equals("colIsInPackage"));

            if (colIsInPackage == null)
            {
                return;
            }

            if (ShowInPackageColumn || (BillingInvoice != null && BillingInvoice.IsHighTechServiceBill)) //20191213 TBL: Nếu xem chi tiết bill là bill KTC thì phải hiển thị cột Trong gói
            {
                colIsInPackage.Visibility = Visibility.Visible;
            }
            else
            {
                colIsInPackage.Visibility = Visibility.Collapsed;
            }
        }

        public void HideHIApplied()
        {
            if (grid != null)
            {
                grid.Columns[1].Visibility = Visibility.Collapsed;
            }
        }

        public void ShowHIApplied()
        {
            if (grid != null)
            {
                grid.Columns[1].Visibility = Visibility.Visible;
            }
        }

        private bool _CheckAllInPackage;
        public bool CheckAllInPackage
        {
            get
            {
                return _CheckAllInPackage;
            }
            set
            {
                if (_CheckAllInPackage != value)
                {
                    _CheckAllInPackage = value;
                    NotifyOfPropertyChange(() => CheckAllInPackage);
                    SetAllInPackage();
                }
            }
        }

        private bool _CheckAllCountHI;
        public bool CheckAllCountHI
        {
            get
            {
                return _CheckAllCountHI;
            }
            set
            {
                if (_CheckAllCountHI != value)
                {
                    _CheckAllCountHI = value;
                    NotifyOfPropertyChange(() => CheckAllCountHI);
                    SetAllCountHI();
                }
            }
        }

        private bool _CheckAllCountPatient;
        public bool CheckAllCountPatient
        {
            get
            {
                return _CheckAllCountPatient;
            }
            set
            {
                if (_CheckAllCountPatient != value)
                {
                    _CheckAllCountPatient = value;
                    NotifyOfPropertyChange(() => CheckAllCountPatient);
                    SetAllCountPatient();
                }
            }
        }

        //▼====: #012
        private bool _IsEnableCountPatient = !Globals.ServerConfigSection.InRegisElements.DisableBtnCheckCountPatientInPt;
        //▲====: #012
        public bool IsEnableCountPatient
        {
            get
            {
                return _IsEnableCountPatient;
            }
            set
            {
                if (_IsEnableCountPatient != value)
                {
                    _IsEnableCountPatient = value;
                    NotifyOfPropertyChange(() => IsEnableCountPatient);
                }
            }
        }

        //public void chkInPackage_Click(object source, object sender)
        //{
        //    CheckBox chkInPackage = source as CheckBox;

        //    MedRegItemBase item = sender as MedRegItemBase;

        //    if (chkInPackage == null || item == null)
        //    {
        //        return;
        //    }

        //    if (chkInPackage.IsChecked.GetValueOrDefault() && item.IsPackageService)
        //    {
        //        MessageBox.Show(string.Format("{0}!", eHCMSResources.A0509_G1_Msg_InfoKhDcChonTrongGoi), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        //        chkInPackage.IsChecked = false;
        //        return;
        //    }

        //    //KMx: Tránh trường hợp chưa bind mà đã gọi event, dẫn đến tính toán sai (09/12/2014 09:52).
        //    if (item.IsInPackage != chkInPackage.IsChecked)
        //    {
        //        item.IsInPackage = chkInPackage.IsChecked.GetValueOrDefault();
        //    }
        //    if (item.V_NewPriceType == (long)AllLookupValues.V_NewPriceType.Unknown_PriceType || item.V_NewPriceType == (long)AllLookupValues.V_NewPriceType.Updatable_PriceType)
        //    {
        //        MessageBox.Show(string.Format("{0} ", eHCMSResources.K3421_G1_DV) + item.ChargeableItemName + " thuộc loại không có giá hoặc giá thay đổi được! Vui lòng kiểm tra lại giá sau khi chọn hoặc bỏ chọn trong gói!");
        //    }
        //    CalcInvoiceItem(item);
        //    ModifiItemCmd(item);
        //}

        //public void chkCountPatient_Click(object source, object sender)
        //{
        //    CheckBox chkCountPatient = source as CheckBox;

        //    MedRegItemBase item = sender as MedRegItemBase;

        //    if (chkCountPatient == null || item == null)
        //    {
        //        return;
        //    }
        //    //KMx: Tránh trường hợp chưa bind mà đã gọi event, dẫn đến tính toán sai (09/12/2014 09:52).
        //    if (item.IsCountPatient != chkCountPatient.IsChecked)
        //    {
        //        item.IsCountPatient = chkCountPatient.IsChecked.GetValueOrDefault();
        //    }

        //    CalcInvoiceItem(item);
        //    ModifiItemCmd(item);
        //}


        //public void chkCountHI_Click(object source, object sender)
        //{
        //    CheckBox chkCountHI = source as CheckBox;

        //    MedRegItemBase item = sender as MedRegItemBase;

        //    decimal HIAllowedPriceCheck = 0;

        //    if (chkCountHI == null || item == null)
        //    {
        //        return;
        //    }

        //    if (item.ID > 0)
        //    {
        //        HIAllowedPriceCheck = item.ChargeableItem.HIAllowedPriceNew.GetValueOrDefault(0);
        //    }
        //    else
        //    {
        //        HIAllowedPriceCheck = item.ChargeableItem.HIAllowedPrice.GetValueOrDefault(0);
        //    }

        //    string error = "";

        //    if (HIBenefit.GetValueOrDefault() <= 0)
        //    {
        //        error = eHCMSResources.A0244_G1_Msg_InfoBNKhongCoBHYT;
        //    }
        //    else if (HIAllowedPriceCheck <= 0)
        //    {
        //        error = string.Format("{0}!", eHCMSResources.Z1099_G1_LoaiDVKgThuocDMBH);
        //    }

        //    if (chkCountHI.IsChecked.GetValueOrDefault() && !string.IsNullOrEmpty(error))
        //    {
        //        MessageBox.Show(error, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        //        chkCountHI.IsChecked = false;
        //        return;
        //    }


        //    //KMx: Tránh trường hợp chưa bind mà đã gọi event, dẫn đến tính toán sai (09/12/2014 09:52).
        //    //KMx: Set property IsReadOnly của column = True sẽ giải quyết được lỗi này (21/09/2016 14:07).
        //    if (item.IsCountHI != chkCountHI.IsChecked)
        //    {
        //        item.IsCountHI = chkCountHI.IsChecked.GetValueOrDefault();
        //    }

        //    CalcInvoiceItem(item);
        //    ModifiItemCmd(item);
        //}

        public void chkInPackage_Click(object source, object sender)
        {
            if (SelectedRegistrationItem == null)
            {
                return;
            }

            if (SelectedRegistrationItem.IsInPackage && SelectedRegistrationItem.IsPackageService)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.A0509_G1_Msg_InfoKhDcChonTrongGoi), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                SelectedRegistrationItem.IsInPackage = false;
                return;
            }

            if (SelectedRegistrationItem.V_NewPriceType == (long)AllLookupValues.V_NewPriceType.Unknown_PriceType || SelectedRegistrationItem.V_NewPriceType == (long)AllLookupValues.V_NewPriceType.Updatable_PriceType)
            {
                MessageBox.Show(string.Format("{0} {1} {2}", eHCMSResources.K3421_G1_DV, SelectedRegistrationItem.ChargeableItemName, eHCMSResources.A0513_G1_Msg_InfoKTraGiaDVGiaKhCoDinh));
            }
            CalcInvoiceItem(SelectedRegistrationItem);
            ModifiItemCmd(SelectedRegistrationItem);
            if (SelectedRegistrationItem.IsDiscounted)
            {
                CurentRegistration.ApplyDiscount(SelectedRegistrationItem, Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, false);
            }
        }

        public void chkCountPatient_Click(object source, object sender)
        {
            if (SelectedRegistrationItem == null)
            {
                return;
            }
            //KMx: Tránh trường hợp chưa bind mà đã gọi event, dẫn đến tính toán sai (09/12/2014 09:52).

            CalcInvoiceItem(SelectedRegistrationItem);
            ModifiItemCmd(SelectedRegistrationItem);
            if (SelectedRegistrationItem.IsDiscounted)
            {
                CurentRegistration.ApplyDiscount(SelectedRegistrationItem, Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, false);
            }
        }


        public void chkCountHI_Click(object source, object sender)
        {
            if (SelectedRegistrationItem is PatientRegistrationDetail)
            {
                PatientRegistrationDetail detail = SelectedRegistrationItem as PatientRegistrationDetail;
                if (!detail.IsCountHI && detail.V_EkipIndex != null && detail.V_EkipIndex.LookupID > 0)
                {
                    if (MessageBox.Show(eHCMSResources.Z2977_G1_XoaEkip, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        detail.V_Ekip = new Lookup();
                        detail.V_EkipIndex = new Lookup();
                    }
                    else
                    {
                        detail.IsCountHI = true;
                        return;
                    }
                }
            }
            string error = "";

            error = CheckHIError(SelectedRegistrationItem);

            //▼====: #020
            if (SelectedRegistrationItem.IsCountHI)
            {
                SelectedRegistrationItem.IsCountSE = false;
            }
            //▲====: #020
            if (SelectedRegistrationItem.IsCountHI && !string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                SelectedRegistrationItem.IsCountHI = false;
                return;
            }

            //20191217 TTM: Nếu là được tích bảo hiểm thì sẽ set thời gian tích bảo hiểm cho vật tư để tính toán số thứ tự của vật tư dựa vào thời gian tích (Tích trước xếp đầu tiên rồi lần lượt)
            if (!IsNewCreateBill)
            {
                CalcInvoiceItem(SelectedRegistrationItem);
            }
            else
            {
                //▼===== 20191230 Kiểm tra nếu là OutwardDrugClinicDept thì mới set ticktime không sẽ không làm. Vì nếu không kiểm tra mà tích BH cho dịch vụ hoặc CLS sẽ => Chết chương trình
                if (SelectedRegistrationItem is OutwardDrugClinicDept)
                {
                    if (SelectedRegistrationItem.IsCountHI)
                    {
                        //▼===== 20200817 TTM: Do kiểm tra như bên dưới thì kết quả cũng chỉ ra là SelectedRegistrationItem thôi nên bỏ kiểm tra mà sử dụng luôn biến SelectedRegistrationItem để làm việc.
                        //var tmpitem = AllRegistrationItems.Where(x => x is OutwardDrugClinicDept).Where(x => (x as OutwardDrugClinicDept).LimQtyAndHIPrice != null
                        //&& (x as OutwardDrugClinicDept).OutID == (SelectedRegistrationItem as OutwardDrugClinicDept).OutID);
                        //foreach (var tmp in tmpitem)
                        //{
                        //    if ((tmp as OutwardDrugClinicDept).OutID == (SelectedRegistrationItem as OutwardDrugClinicDept).OutID)
                        //    {
                        //        //Set tick time để bên trong Recal sắp xếp các vật tư cho chính xác.
                        //        tmp.TickTime = AllRegistrationItems.Where(x => x is OutwardDrugClinicDept).Where(x => (x as OutwardDrugClinicDept).LimQtyAndHIPrice != null && x.TickTime < 100).Max(y => y.TickTime).GetValueOrDefault(0) + 1;
                        //    }
                        //}
                        SelectedRegistrationItem.TickTime = AllRegistrationItems.Where(x => x is OutwardDrugClinicDept).Where(x => (x as OutwardDrugClinicDept).LimQtyAndHIPrice != null && x.TickTime < 100).Max(y => y.TickTime).GetValueOrDefault(0) + 1;
                        //▲=====
                    }
                    else
                    {
                        var tmpitem = AllRegistrationItems.Where(x => x is OutwardDrugClinicDept).Where(x => (x as OutwardDrugClinicDept).LimQtyAndHIPrice != null
                        && (x as OutwardDrugClinicDept).OutID == (SelectedRegistrationItem as OutwardDrugClinicDept).OutID);
                        foreach (var tmp in tmpitem)
                        {
                            if ((tmp as OutwardDrugClinicDept).OutID == (SelectedRegistrationItem as OutwardDrugClinicDept).OutID)
                            {
                                tmp.TickTime = 0;
                            }
                        }
                    }
                }
                //▲===== 
                CalcInvoiceItem(SelectedRegistrationItem, true, true);

                //▼===== 20191228 TTM:  Cần phải set modified item cho tất cả các item theo thông tư vì khi bỏ Bảo hiểm ra đã set lại countvalue và hipaymentpercent
                //                      => mặc dù không đụng vào các item đó cũng phải set modified để set lại countvalue và hipaymentpercent
                foreach (var item in AllRegistrationItems.Where(x => x is OutwardDrugClinicDept).Where(x => (x as OutwardDrugClinicDept).LimQtyAndHIPrice != null))
                {
                    ModifiItemCmd(item);
                }
                //▲===== 

            }
            ModifiItemCmd(SelectedRegistrationItem);
            if (SelectedRegistrationItem.IsDiscounted)
            {
                CurentRegistration.ApplyDiscount(SelectedRegistrationItem, Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, false);
            }
        }

        //▼====: #019
        private bool _CheckAllCountCOVID;
        public bool CheckAllCountCOVID
        {
            get
            {
                return _CheckAllCountCOVID;
            }
            set
            {
                if (_CheckAllCountCOVID != value)
                {
                    _CheckAllCountCOVID = value;
                    NotifyOfPropertyChange(() => CheckAllCountCOVID);
                    SetAllCountCOVID();
                }
            }
        }

        public void chkCountPatientCOVID_Click(object source, object sender)
        {
            string error = "";

            if (SelectedRegistrationItem.IsCountPatientCOVID)
            {
                error = CheckCOVIDError(SelectedRegistrationItem);
                if (!string.IsNullOrEmpty(error))
                {
                    SelectedRegistrationItem.IsCountPatientCOVID = false;
                    MessageBox.Show(error, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }
            }

            CalcInvoiceItem(SelectedRegistrationItem, false, false, true);
            ModifiItemCmd(SelectedRegistrationItem);
            if (SelectedRegistrationItem.IsDiscounted)
            {
                CurentRegistration.ApplyDiscount(SelectedRegistrationItem, Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, false);
            }
        }

        public void SetAllCountCOVID()
        {
            if (AllRegistrationItems == null || AllRegistrationItems.Count <= 0)
            {
                return;
            }

            if (CheckAllCountCOVID)
            {
                foreach (MedRegItemBase item in AllRegistrationItems)
                {
                    string error = "";
                    error = CheckCOVIDError(item);

                    if (string.IsNullOrEmpty(error) && !item.IsCountPatientCOVID)
                    {
                        item.IsCountPatientCOVID = true;
                    }

                    if (!IsNewCreateBill)
                    {
                        CalcInvoiceItem(item);
                    }
                    else
                    {
                        CalcInvoiceItem(item, false, false, true);
                    }
                    ModifiItemCmd(item);
                    if (item.IsDiscounted)
                    {
                        CurentRegistration.ApplyDiscount(item, Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, false);
                    }
                }
            }
            else
            {
                foreach (MedRegItemBase item in AllRegistrationItems)
                {
                    if (item.IsCountPatientCOVID)
                    {
                        item.IsCountPatientCOVID = false;
                    }
                    CalcInvoiceItem(item, false, true);
                    ModifiItemCmd(item);
                    if (item.IsDiscounted)
                    {
                        CurentRegistration.ApplyDiscount(item, Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, false);
                    }
                }
            }
        }

        private string CheckCOVIDError(MedRegItemBase item)
        {
            string error = "";
            bool IsCountPatientCOVID = false;
            if (CurentRegistration.AdmissionInfo != null)
            {
                IsCountPatientCOVID = CurentRegistration.AdmissionInfo.IsTreatmentCOVID;
            }
            if (!IsCountPatientCOVID)
            {
                error = "Đăng ký không được xác nhận điều trị COVID";
            }

            if (item is PatientRegistrationDetail)
            {
                PatientRegistrationDetail detail = item as PatientRegistrationDetail;
                if (!detail.RefMedicalServiceItem.InCategoryCOVID)
                {
                    error = "DVKT không nằm trong danh mục COVID. Vui lòng liên hệ KHTH!";
                }
            }
            if (item is PatientPCLRequestDetail)
            {
                PatientPCLRequestDetail detail = item as PatientPCLRequestDetail;
                if (!detail.PCLExamType.InCategoryCOVID)
                {
                    error = "DVKT không nằm trong danh mục COVID. Vui lòng liên hệ KHTH!";
                }
            }
            if (item is OutwardDrugClinicDept)
            {
                OutwardDrugClinicDept detail = item as OutwardDrugClinicDept;
                if (!detail.GenMedProductItem.InCategoryCOVID)
                {
                    error = "Thuốc/ vật tư không nằm trong danh mục COVID. Vui lòng liên hệ Khoa dược!";
                }
            }
            return error;
        }

        private bool _mCheckCovid = true;
        public bool mCheckCovid
        {
            get
            {
                return _mCheckCovid;
            }
            set
            {
                if (_mCheckCovid == value)
                    return;
                _mCheckCovid = value;
                NotifyOfPropertyChange(() => mCheckCovid);
            }
        }
        //▲====: #019

        public void SetAllInPackage()
        {
            if (AllRegistrationItems == null || AllRegistrationItems.Count <= 0)
            {
                return;
            }

            foreach (MedRegItemBase item in AllRegistrationItems)
            {
                if (CheckAllInPackage && !item.IsPackageService)
                {
                    item.IsInPackage = true;
                }
                else
                {
                    item.IsInPackage = false;
                }
                CalcInvoiceItem(item);
                ModifiItemCmd(item);
                if (item.IsDiscounted)
                {
                    CurentRegistration.ApplyDiscount(item, Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, false);
                }
            }
        }

        public string CheckHIError(MedRegItemBase item)
        {
            decimal HIAllowedPriceCheck = 0;

            if (item == null)
            {
                return string.Format("{0}!", eHCMSResources.Z1239_G1_ItemKgCoGTri);
            }

            if (item.ID > 0)
            {
                HIAllowedPriceCheck = item.ChargeableItem.HIAllowedPriceNew.GetValueOrDefault(0);
            }
            else
            {
                HIAllowedPriceCheck = item.ChargeableItem.HIAllowedPrice.GetValueOrDefault(0);
            }

            string error = "";

            if (HIBenefit.GetValueOrDefault() <= 0)
            {
                error = eHCMSResources.A0244_G1_Msg_InfoBNKhongCoBHYT;
            }
            else if (HIAllowedPriceCheck <= 0)
            {
                error = string.Format("{0}!", eHCMSResources.Z1099_G1_LoaiDVKgThuocDMBH);
            }
            // =====▼ #005
            else
            {
                bool bItemInstDateIsValid = false;

                if (CurentRegistration.HealthInsurance != null 
                    && CurentRegistration.PtInsuranceBenefit.GetValueOrDefault() > 0 
                    && (item.MedicalInstructionDate >= CurentRegistration.HealthInsurance.ValidDateFrom
                    //▼====: #021
                    && item.MedicalInstructionDate <= CurentRegistration.HealthInsurance.ValidDateTo.GetValueOrDefault().AddDays(Globals.ServerConfigSection.InRegisElements.NumDayHIAgreeToPayAfterHIExpiresInPt)))
                    //▲====: #021
                {
                    bItemInstDateIsValid = true;
                }
                else if (CurentRegistration.HealthInsurance_2 != null 
                    && CurentRegistration.PtInsuranceBenefit_2.GetValueOrDefault() > 0 
                    && (item.MedicalInstructionDate >= CurentRegistration.HealthInsurance_2.ValidDateFrom
                    //▼====: #021
                    && item.MedicalInstructionDate <= CurentRegistration.HealthInsurance_2.ValidDateTo.GetValueOrDefault().AddDays(Globals.ServerConfigSection.InRegisElements.NumDayHIAgreeToPayAfterHIExpiresInPt)))
                    //▲====: #021
                {
                    bItemInstDateIsValid = true;
                }
                // 2020117 #013 TNHX: Thêm số ngày BHYT đồng ý chi trả đối với BN hết hạn thẻ BHYT
                else if (CurentRegistration.HealthInsurance_3 != null
                    && CurentRegistration.PtInsuranceBenefit_3.GetValueOrDefault() > 0
                    && (item.MedicalInstructionDate >= CurentRegistration.HealthInsurance_3.ValidDateFrom
                    //▼====: #021
                    && item.MedicalInstructionDate <= CurentRegistration.HealthInsurance_3.ValidDateTo.GetValueOrDefault().AddDays(Globals.ServerConfigSection.InRegisElements.NumOfOverDaysForMedicalInstructDate)))
                    //▲====: #021
                {
                    bItemInstDateIsValid = true;
                }
                if (!bItemInstDateIsValid)
                {
                    error = string.Format("{0}!", "Item co ngay Y Lenh khong nam trong khoang thoi gian hop le cua the BHYT.");
                }
            }
            // =====▲ #005
            return error;
        }

        public void SetAllCountHI()
        {
            if (AllRegistrationItems == null || AllRegistrationItems.Count <= 0)
            {
                return;
            }

            if (CheckAllCountHI)
            {
                foreach (MedRegItemBase item in AllRegistrationItems)
                {
                    string error = "";
                    error = CheckHIError(item);

                    if (string.IsNullOrEmpty(error) && !item.IsCountPatientCOVID)
                    {
                        item.IsCountHI = true;
                    }
                    else
                    {
                        item.IsCountHI = false;
                    }

                    if (!IsNewCreateBill)
                    {
                        CalcInvoiceItem(item);
                    }
                    else
                    {
                        if (item is OutwardDrugClinicDept && (item as OutwardDrugClinicDept).LimQtyAndHIPrice != null)
                        {
                            item.TickTime = item.IsCountHI ? AllRegistrationItems.Where(x => x is OutwardDrugClinicDept).Where(x => (x as OutwardDrugClinicDept).LimQtyAndHIPrice != null && x.TickTime < 100).Max(y => y.TickTime).GetValueOrDefault(0) + 1 : 100;
                            CalcInvoiceItem(item, true, true);
                        }
                        else
                        {
                            CalcInvoiceItem(item, false, true);
                        }
                    }
                    ModifiItemCmd(item);
                    if (item.IsDiscounted)
                    {
                        CurentRegistration.ApplyDiscount(item, Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, false);
                    }
                }
            }
            else
            {
                bool bExistEkip = false;
                foreach (PatientRegistrationDetail item in AllRegistrationItems)
                {
                    if (item.V_EkipIndex != null && item.V_EkipIndex.LookupID > 0)
                    {
                        bExistEkip = true;
                        break;
                    }
                }
                if (bExistEkip && MessageBox.Show(eHCMSResources.Z2977_G1_XoaEkip, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    foreach (MedRegItemBase item in AllRegistrationItems)
                    {
                        (item as PatientRegistrationDetail).V_Ekip = new Lookup();
                        (item as PatientRegistrationDetail).V_EkipIndex = new Lookup();
                        item.IsCountHI = false;
                        CalcInvoiceItem(item, false, true);
                        ModifiItemCmd(item);
                        if (item.IsDiscounted)
                        {
                            CurentRegistration.ApplyDiscount(item, Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, false);
                        }
                    }
                }
                else if (!bExistEkip)
                {
                    foreach (MedRegItemBase item in AllRegistrationItems)
                    {
                        item.IsCountHI = false;
                        CalcInvoiceItem(item, false, true);
                        ModifiItemCmd(item);
                        if (item.IsDiscounted)
                        {
                            CurentRegistration.ApplyDiscount(item, Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, false);
                        }
                    }
                }
                else
                {
                    CheckAllCountHI = true;
                }
            }
        }

        public void SetAllCountPatient()
        {
            if (AllRegistrationItems == null || AllRegistrationItems.Count <= 0)
            {
                return;
            }

            foreach (MedRegItemBase item in AllRegistrationItems)
            {
                item.IsCountPatient = CheckAllCountPatient;
                CalcInvoiceItem(item);
                ModifiItemCmd(item);
                if (item.IsDiscounted)
                {
                    CurentRegistration.ApplyDiscount(item, Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, false);
                }
            }
        }

        //public void TinhTienCmd()
        //{
        //    if (AllRegistrationItems == null)
        //    {
        //        return;
        //    }

        //    decimal TotalPatientPayment;
        //    decimal TotalHIPayment;
        //    decimal TotalBillInvoice;
        //    TotalPatientPayment = AllRegistrationItems.Sum(x => x.TotalPatientPayment);
        //    TotalHIPayment = AllRegistrationItems.Sum(x => x.TotalHIPayment);
        //    TotalBillInvoice = TotalPatientPayment + TotalHIPayment;


        //    var vm = Globals.GetViewModel<ITotalBillInvoice>();
        //    vm.TotalPatientPayment = TotalPatientPayment;
        //    vm.TotalHIPayment = TotalHIPayment;
        //    vm.TotalBillInvoice = TotalBillInvoice;

        //    Globals.ShowDialog(vm as Conductor<object>);
        //}

        //public bool TinhTienCmd()
        //{
        //    if (AllRegistrationItems == null)
        //    {
        //        return false;
        //    }

        //    decimal TotalPatientPayment;
        //    decimal TotalHIPayment;
        //    decimal TotalRealHIPayment;
        //    decimal TotalBillInvoice;
        //    decimal MaxHIPayForHighTechServiceBill;

        //    if (IsHighTechServiceBill)
        //    {
        //        if (BillingInvoice != null && BillingInvoice.InPatientBillingInvID > 0)
        //        {
        //            MaxHIPayForHighTechServiceBill = BillingInvoice.MaxHIPay;
        //        }
        //        else
        //        {
        //            MaxHIPayForHighTechServiceBill = Globals.ServerConfigSection.InRegisElements.MaxHIPayForHighTechServiceBill;
        //        }

        //    }
        //    else
        //    {
        //        MaxHIPayForHighTechServiceBill = 0;
        //    }

        //    TotalHIPayment = AllRegistrationItems.Sum(x => x.TotalHIPayment);
        //    TotalBillInvoice = AllRegistrationItems.Sum(x => x.TotalInvoicePrice);

        //    if (MaxHIPayForHighTechServiceBill > 0 && TotalHIPayment > MaxHIPayForHighTechServiceBill)
        //    {
        //        TotalRealHIPayment = MaxHIPayForHighTechServiceBill;
        //    }
        //    else
        //    {
        //        TotalRealHIPayment = TotalHIPayment;
        //    }

        //    TotalPatientPayment = TotalBillInvoice - TotalRealHIPayment;

        //    var vm = Globals.GetViewModel<ITotalBillInvoice>();
        //    vm.TotalPatientPayment = TotalPatientPayment;
        //    vm.TotalHIPayment = TotalHIPayment;
        //    vm.TotalBillInvoice = TotalBillInvoice;
        //    vm.TotalRealHIPayment = TotalRealHIPayment;

        //    Globals.ShowDialog(vm as Conductor<object>);
        //}


        private bool _IsHighTechServiceBill = false;
        public bool IsHighTechServiceBill
        {
            get
            {
                return _IsHighTechServiceBill;
            }
            set
            {
                _IsHighTechServiceBill = value;
                NotifyOfPropertyChange(() => IsHighTechServiceBill);
            }
        }

        /*==== #002 ====*/
        private bool _IsHICard_FiveYearsCont_NoPaid = false;
        public bool IsHICard_FiveYearsCont_NoPaid
        {
            get
            {
                return _IsHICard_FiveYearsCont_NoPaid;
            }
            set
            {
                _IsHICard_FiveYearsCont_NoPaid = value;
                NotifyOfPropertyChange(() => IsHICard_FiveYearsCont_NoPaid);
            }
        }
        private bool _IsNotPermitted = false;
        public bool IsNotPermitted
        {
            get
            {
                return _IsNotPermitted;
            }
            set
            {
                _IsNotPermitted = value;
                NotifyOfPropertyChange(() => IsNotPermitted);
            }
        }
        /*==== #002 ====*/
        public decimal CalcTotalPatientPayment()
        {
            decimal TotalPatientPayment;
            decimal TotalHIPayment;
            decimal TotalRealHIPayment;
            decimal TotalBillInvoice;

            CalcTotalBillInvoice(out TotalPatientPayment, out TotalHIPayment, out TotalRealHIPayment, out TotalBillInvoice);
            return TotalPatientPayment;
        }

        public bool CheckTotalBillInvoice()
        {
            decimal TotalPatientPayment;
            decimal TotalHIPayment;
            decimal TotalRealHIPayment;
            decimal TotalBillInvoice;

            CalcTotalBillInvoice(out TotalPatientPayment, out TotalHIPayment, out TotalRealHIPayment, out TotalBillInvoice);
            if (TotalPatientPayment < 0 || TotalHIPayment < 0 || TotalRealHIPayment < 0 || TotalBillInvoice < 0)
            {
                ShowTotalBillInvoice(TotalPatientPayment, TotalHIPayment, TotalRealHIPayment, TotalBillInvoice, true);
                return false;
            }

            return true;
        }

        public decimal TotalCharitySupportFund()
        {
            if (BillingInvoice == null || CurrentSupportFunds == null || CurrentSupportFunds.Count(x => x.BillingInvID == BillingInvoice.InPatientBillingInvID) <= 0)
            {
                return 0;
            }
            return Math.Round(CurrentSupportFunds.Where(x => x.BillingInvID == BillingInvoice.InPatientBillingInvID).Sum(x => x.AmountValue));
        }

        public void TinhTienCmd()
        {
            decimal TotalPatientPayment;
            decimal TotalHIPayment;
            decimal TotalRealHIPayment;
            decimal TotalBillInvoice;

            CalcTotalBillInvoice(out TotalPatientPayment, out TotalHIPayment, out TotalRealHIPayment, out TotalBillInvoice);
            ShowTotalBillInvoice(TotalPatientPayment, TotalHIPayment, TotalRealHIPayment, TotalBillInvoice, false, TotalCharitySupportFund());
        }

        public void CalcTotalBillInvoice(out decimal totalPatientPayment, out decimal totalHIPayment, out decimal totalRealHIPayment, out decimal totalBillInvoice)
        {
            totalPatientPayment = 0;
            totalHIPayment = 0;
            totalRealHIPayment = 0;
            totalBillInvoice = 0;

            if (AllRegistrationItems == null)
            {
                return;
            }

            decimal MaxHIPayForHighTechServiceBill;
            /*==== #002 ====*/
            //if (IsHighTechServiceBill)
            if (IsHighTechServiceBill && !IsNotPermitted)
            /*==== #002 ====*/
            {
                if (BillingInvoice != null && BillingInvoice.InPatientBillingInvID > 0)
                {
                    MaxHIPayForHighTechServiceBill = BillingInvoice.MaxHIPay;
                }
                else
                {
                    /*==== #002 ====*/
                    //MaxHIPayForHighTechServiceBill = Globals.ServerConfigSection.InRegisElements.MaxHIPayForHighTechServiceBill;
                    MaxHIPayForHighTechServiceBill = BillingInvoice == null || BillingInvoice.IsHICard_FiveYearsCont_NoPaid == false ? Globals.ServerConfigSection.InRegisElements.MaxHIPayForHighTechServiceBill * (decimal)HIBenefit.GetValueOrDefault(0) : (HIBenefit.GetValueOrDefault(0) == 0.8 ? Globals.ServerConfigSection.InRegisElements.MaxHIPayForHighTechServiceBill - (6 * Globals.ServerConfigSection.HealthInsurances.HiPolicyMinSalary) : Globals.ServerConfigSection.InRegisElements.MaxHIPayForHighTechServiceBill);
                    /*==== #002 ====*/
                }

            }
            else
            {
                MaxHIPayForHighTechServiceBill = 0;
            }
            totalHIPayment = AllRegistrationItems.Sum(x => x.TotalHIPayment);
            totalBillInvoice = AllRegistrationItems.Sum(x => x.TotalInvoicePrice);

            if (MaxHIPayForHighTechServiceBill > 0 && totalHIPayment > MaxHIPayForHighTechServiceBill)
            {
                totalRealHIPayment = MaxHIPayForHighTechServiceBill;
            }
            else
            {
                totalRealHIPayment = totalHIPayment;
            }

            totalPatientPayment = totalBillInvoice - totalRealHIPayment;

        }

        public void ShowTotalBillInvoice(decimal totalPatientPayment, decimal totalHIPayment, decimal totalRealHIPayment, decimal totalBillInvoice, bool showErrorMessage, decimal totalCharitySupportFund = 0)
        {
            Action<ITotalBillInvoice> onInitDlg = delegate (ITotalBillInvoice vm)
            {
                vm.TotalPatientPayment = totalPatientPayment - totalCharitySupportFund;
                vm.TotalHIPayment = totalHIPayment;
                vm.TotalRealHIPayment = totalRealHIPayment;
                vm.TotalBillInvoice = totalBillInvoice;
                vm.ShowErrorMessage = showErrorMessage;
                vm.TotalCharitySupportFund = totalCharitySupportFund;
            };
            GlobalsNAV.ShowDialog<ITotalBillInvoice>(onInitDlg);
        }

        // TxD 16/07/2016 The following event HANDLER is NOT used anymore because the IInPatientBillingInvoiceDetailsListing view model 
        // is also used by a dialog to show the billing invoice details (click i in the list of billing invoices). So if the dialog is displayed 
        // even after it is closed the event will still be received wrongly and causing havoc.
        //public void Handle(ModifyPriceToUpdate_Completed message)
        //{
        //    if (ModifyBillingInvItem.IsModItemOK == false)
        //    {
        //        return;
        //    }
        //    AllRegistrationItems[index].ReasonChangePrice = ModifyBillingInvItem.ReasonChangePrice;
        //    if (AllRegistrationItems[index].ID > 0)
        //    {
        //        AllRegistrationItems[index].ChargeableItem.HIAllowedPriceNew = ModifyBillingInvItem.ChargeableItem.HIAllowedPrice.GetValueOrDefault();
        //        AllRegistrationItems[index].ChargeableItem.HIPatientPriceNew = AllRegistrationItems[index].ChargeableItem.NormalPriceNew = ModifyBillingInvItem.ChargeableItem.NormalPrice;
        //    }
        //    else
        //    {
        //        AllRegistrationItems[index].ChargeableItem.HIAllowedPrice = ModifyBillingInvItem.ChargeableItem.HIAllowedPrice.GetValueOrDefault();
        //        AllRegistrationItems[index].ChargeableItem.HIPatientPrice = AllRegistrationItems[index].ChargeableItem.NormalPrice = ModifyBillingInvItem.ChargeableItem.NormalPrice;
        //    }
        //    CalcInvoiceItem(AllRegistrationItems[index]);

        //    AllRegistrationItems[index].RecordState = RecordState.MODIFIED;

        //    NotifyOfPropertyChange(() => AllRegistrationItems);
        //}

        public void ModOfUpdatableItemPriceDone()
        {
            if (ModifyBillingInvItem.IsModItemOK == false)
            {
                return;
            }
            AllRegistrationItems[index].ReasonChangePrice = ModifyBillingInvItem.ReasonChangePrice;
            //if (AllRegistrationItems[index].ID > 0)
            //{
            //    AllRegistrationItems[index].ChargeableItem.HIAllowedPriceNew = ModifyBillingInvItem.ChargeableItem.HIAllowedPrice.GetValueOrDefault();
            //    AllRegistrationItems[index].ChargeableItem.HIPatientPriceNew = AllRegistrationItems[index].ChargeableItem.NormalPriceNew = ModifyBillingInvItem.ChargeableItem.NormalPrice;
            //}
            //else
            //{
            AllRegistrationItems[index].ChargeableItem.HIAllowedPrice = ModifyBillingInvItem.ChargeableItem.HIAllowedPrice.GetValueOrDefault();
            AllRegistrationItems[index].ChargeableItem.HIPatientPrice = AllRegistrationItems[index].ChargeableItem.NormalPrice = ModifyBillingInvItem.ChargeableItem.NormalPrice;
            //}
            CalcInvoiceItem(AllRegistrationItems[index]);

            AllRegistrationItems[index].RecordState = RecordState.MODIFIED;

            NotifyOfPropertyChange(() => AllRegistrationItems);
        }

        private ObservableCollection<Staff> _StaffCatgs;
        public ObservableCollection<Staff> StaffCatgs
        {
            get
            {
                return _StaffCatgs;
            }
            set
            {
                if (_StaffCatgs != value)
                {
                    _StaffCatgs = value;
                    NotifyOfPropertyChange(() => StaffCatgs);
                }
            }
        }

        AutoCompleteBox AutoDoctor;
        public void AutoDoctor_Loaded(object sender, RoutedEventArgs e)
        {
            AutoDoctor = sender as AutoCompleteBox;
        }

        public void AutoDoctor_Populating(object sender, PopulatingEventArgs e)
        {
            if (Globals.AllStaffs == null || Globals.AllStaffs.Count <= 0)
            {
                return;
            }

            StaffCatgs = Globals.AllStaffs.Where(o => o.RefStaffCategory != null && o.RefStaffCategory.V_StaffCatType == (long)V_StaffCatType.BacSi
                                    && (VNConvertString.ConvertString(o.FullName).ToLower().Contains(VNConvertString.ConvertString(e.Parameter).ToLower()))
                                    && !o.IsStopUsing).ToObservableCollection();
            NotifyOfPropertyChange(() => StaffCatgs);
            AutoDoctor.ItemsSource = StaffCatgs;
            AutoDoctor.PopulateComplete();
        }

        public void AutoDoctor_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (SelectedRegistrationItem == null)
            {
                return;
            }

            if (SelectedRegistrationItem.DoctorStaff == null)
            {
                SelectedRegistrationItem.DoctorStaff = new Staff();
            }

            if (AutoDoctor != null && AutoDoctor.SelectedItem != null)
            {
                SelectedRegistrationItem.DoctorStaff.StaffID = (AutoDoctor.SelectedItem as Staff).StaffID;
                SelectedRegistrationItem.DoctorStaff.FullName = (AutoDoctor.SelectedItem as Staff).FullName;
            }
            else
            {
                SelectedRegistrationItem.DoctorStaff.StaffID = 0;
                SelectedRegistrationItem.DoctorStaff.FullName = "";
            }
        }

        private ObservableCollection<CharitySupportFund> _CurrentSupportFunds;
        public ObservableCollection<CharitySupportFund> CurrentSupportFunds
        {
            get
            {
                return _CurrentSupportFunds;
            }
            set
            {
                _CurrentSupportFunds = value;
                NotifyOfPropertyChange(() => CurrentSupportFunds);
            }
        }
        /*▼====: #003*/
        private PatientRegistration _CurentRegistration;
        public PatientRegistration CurentRegistration
        {
            get
            {
                return _CurentRegistration;
            }
            set
            {
                if (_CurentRegistration != value)
                {
                    _CurentRegistration = value;
                    if (_CurentRegistration.AdmissionInfo != null)
                    {
                        if (_CurentRegistration.AdmissionInfo.IsTreatmentCOVID)
                        {
                            IsShowPatientCOVID = Visibility.Visible;
                        }
                        else
                        {
                            IsShowPatientCOVID = Visibility.Collapsed;
                        }
                    }
                    NotifyOfPropertyChange(() => CurentRegistration);
                }
            }
        }
        /*▲====: #003*/
        public void ckbDiscount_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckBox))
            {
                return;
            }
            CheckBox ckbDiscount = sender as CheckBox;
            if (!(ckbDiscount.DataContext is MedRegItemBase) || CurentRegistration == null)
            {
                e.Handled = true;
                ckbDiscount.IsChecked = false;
                (ckbDiscount.DataContext as MedRegItemBase).DiscountAmt = 0;
                return;
            }
            (ckbDiscount.DataContext as MedRegItemBase).PromoDiscProgID = CurentRegistration.PromoDiscountProgramObj == null ? null : (long?)CurentRegistration.PromoDiscountProgramObj.PromoDiscProgID;
            ModifiItemCmd(ckbDiscount.DataContext as MedRegItemBase);
            //(ckbDiscount.DataContext as MedRegItemBase).RecordState = RecordState.MODIFIED;
            //if (ckbDiscount.DataContext is PatientPCLRequestDetail && (ckbDiscount.DataContext as PatientPCLRequestDetail).PCLReqItemID > 0)
            //{
            //    if (BillingInvoice != null && BillingInvoice.PclRequests != null && BillingInvoice.PclRequests.Any(x => x.PatientPCLRequestIndicators != null && x.PatientPCLRequestIndicators.Any(i => i == ckbDiscount.DataContext as PatientPCLRequestDetail)))
            //    {
            //        BillingInvoice.PclRequests.First(x => x.PatientPCLRequestIndicators != null && x.PatientPCLRequestIndicators.Any(i => i == ckbDiscount.DataContext as PatientPCLRequestDetail)).RecordState = RecordState.MODIFIED;
            //    }
            //}
            if (CurentRegistration.PromoDiscountProgramObj != null && CurentRegistration.PromoDiscountProgramObj.IsOnPriceDiscount)
            {
                (ckbDiscount.DataContext as MedRegItemBase).IsDiscountChecked = ckbDiscount.IsChecked.GetValueOrDefault(false);
                return;
            }
            if ((ckbDiscount.DataContext as MedRegItemBase).DiscountAmt == 0 && CurentRegistration.PromoDiscountProgramObj != null && CurentRegistration.PromoDiscountProgramObj.DiscountPercent == 0)
            {
                e.Handled = true;
                ckbDiscount.IsChecked = false;
                (ckbDiscount.DataContext as MedRegItemBase).DiscountAmt = 0;
                return;
            }
            CurentRegistration.ApplyDiscount((ckbDiscount.DataContext as MedRegItemBase), Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, !((sender as CheckBox).IsChecked == true));
            //▼===== #008: Bổ sung code chuyển từ UNCHANGED sang MODIFIED khi chọn miễn giảm cho CLS
            if ((ckbDiscount.DataContext as MedRegItemBase) is PatientPCLRequestDetail)
            {
                ModifiItemCmd((ckbDiscount.DataContext as MedRegItemBase));
            }
            //▲===== #008
        }
        public bool CanApplyIsOnPriceDiscount
        {
            get
            {
                return CanEditOnGrid && CurentRegistration != null && CurentRegistration.PromoDiscountProgramObj != null && CurentRegistration.PromoDiscountProgramObj.IsOnPriceDiscount;
            }
        }
        public void NotifyOfCanApplyIsOnPriceDiscount()
        {
            NotifyOfPropertyChange(() => CanApplyIsOnPriceDiscount);
            NotifyOfPropertyChange(() => IsShowPrintDiscountButton);
        }
        public bool IsShowPrintDiscountButton
        {
            get
            {
                return !CanEditOnGrid || !CanApplyIsOnPriceDiscount;
            }
        }
        public void txtDiscount_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is null || !(sender is TextBox))
            {
                return;
            }
            decimal mDiscountAmount = 0m;
            if (!decimal.TryParse((sender as TextBox).Text, out mDiscountAmount))
            {
                (sender as TextBox).Text = "0";
            }
            ((sender as TextBox).DataContext as MedRegItemBase).DiscountAmt = mDiscountAmount;
            CurentRegistration.ApplyDiscount(((sender as TextBox).DataContext as MedRegItemBase), Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, false);
        }
        public void btnDiscount_Click(object sender, RoutedEventArgs e)
        {
            if (CurentRegistration == null || CurentRegistration.PtRegistrationID == 0)
            {
                return;
            }
            Button mbtnDiscount = sender as Button;
            if (!(mbtnDiscount.DataContext is MedRegItemBase) || (mbtnDiscount.DataContext as MedRegItemBase).PromoDiscProgID.GetValueOrDefault(0) == 0)
            {
                return;
            }
            IPromoDiscountProgramEdit mPromoDiscountProgramEdit = Globals.GetViewModel<IPromoDiscountProgramEdit>();
            mPromoDiscountProgramEdit.IsViewOld = true;
            mPromoDiscountProgramEdit.CanBtnViewPrint = true;
            mPromoDiscountProgramEdit.PtRegistrationID = CurentRegistration.PtRegistrationID;
            mPromoDiscountProgramEdit.PromoDiscountProgramObj = new PromoDiscountProgram { PromoDiscProgID = (mbtnDiscount.DataContext as MedRegItemBase).PromoDiscProgID.Value, V_RegistrationType = (long)AllLookupValues.RegistrationType.NOI_TRU };
            GlobalsNAV.ShowDialog_V3<IPromoDiscountProgramEdit>(mPromoDiscountProgramEdit);
        }
        public void EditMedicalInstructionDate(MedRegItemBase source, object eventArgs)
        {
            IMinHourDateControl mMinHourCtr = Globals.GetViewModel<IMinHourDateControl>();
            mMinHourCtr.DateTime = source.MedicalInstructionDate;
            mMinHourCtr.bShowButton = true;
            GlobalsNAV.ShowDialog_V3<IMinHourDateControl>(mMinHourCtr);
            if (mMinHourCtr.bOK)
            {
                source.MedicalInstructionDate = mMinHourCtr.DateTime;
            }
        }
        public void EditResultDate(MedRegItemBase source, object eventArgs)
        {
            IMinHourDateControl mMinHourCtr = Globals.GetViewModel<IMinHourDateControl>();
            mMinHourCtr.DateTime = source.ResultDate;
            mMinHourCtr.bShowButton = true;
            GlobalsNAV.ShowDialog_V3<IMinHourDateControl>(mMinHourCtr);
            if (mMinHourCtr.bOK)
            {
                source.ResultDate = mMinHourCtr.DateTime;
            }
        }

        public void RecalForLimProduct(bool OrderType = true, bool IsFromCountHI = false)
        {
            //20191219 TTM: Chỉ có tick bảo hiểm mới làm thay đổi số thứ tự của các y cụ có giới hạn tính giá BH.
            if (IsFromCountHI)
            {
                int ncounter = 1;
                int IsChangeSeqNum = 0;
                int checkcount = 0;
                List<MedRegItemBase> tmpListItemBase = new List<MedRegItemBase>();
                if (OrderType)
                {
                    tmpListItemBase = AllRegistrationItems.Where(x => x is OutwardDrugClinicDept).Where(x => (x as OutwardDrugClinicDept).LimQtyAndHIPrice != null).OrderBy(y => y.TickTime).ToList();
                }
                else
                {
                    tmpListItemBase = AllRegistrationItems.Where(x => x is OutwardDrugClinicDept).Where(x => (x as OutwardDrugClinicDept).LimQtyAndHIPrice != null).OrderBy(y => y.CountValue).ToList();
                }
                foreach (var detail in tmpListItemBase)
                {
                    checkcount++;
                    if (detail.IsCountHI && detail.CountValue <= 0)
                    {
                        detail.CountValue = ncounter;
                        ncounter++;
                    }
                    else
                    {
                        if (!detail.IsCountHI && detail.CountValue > 0)
                        {
                            detail.CountValue = 0;
                            detail.TickTime = 100;
                            IsChangeSeqNum = -1;
                        }
                        else if (IsChangeSeqNum < 0)
                        {
                            detail.CountValue += IsChangeSeqNum;
                        }
                    }
                    if (checkcount == ncounter)
                    {
                        ncounter++;
                    }
                }
            }

            //20191219 TTM: Dựa vào thứ tự đã xếp ở trên để tính bảo hiểm.
            var listItemToRecal = AllRegistrationItems.Where(x => x is OutwardDrugClinicDept).Where(x => (x as OutwardDrugClinicDept).LimQtyAndHIPrice != null);
            foreach (var itemdetail in listItemToRecal)
            {
                OutwardDrugClinicDept value = itemdetail as OutwardDrugClinicDept;
                if (value.CountValue == 1)
                {
                    value.HIPaymentPercent = Convert.ToDouble(value.LimQtyAndHIPrice.ItemNumber1MaxPayPerc);
                    if (value.GenMedProductID == null)
                    {
                        value.TotalHIPayment = 0;
                    }
                    else
                    {
                        decimal PtHiBenefit = (decimal)HIBenefit;
                        if (value.LimQtyAndHIPrice.ItemNumber1MaxBenefit > 0)
                        {
                            if (CurentRegistration.IsCrossRegion == false)
                            {
                                PtHiBenefit = value.LimQtyAndHIPrice.ItemNumber1MaxBenefit;
                            }
                            else
                            {
                                PtHiBenefit = (decimal)Globals.ServerConfigSection.HealthInsurances.RebatePercentage2015Level1_InPt;
                            }
                        }
                        value.TotalHIPayment = Math.Min(value.LimQtyAndHIPrice.ItemNumber1MaxPayAmt, value.ChargeableItem.HIAllowedPrice.GetValueOrDefault(0) * (decimal)value.HIPaymentPercent) * PtHiBenefit;
                    }
                }
                else if (value.CountValue == 2)
                {
                    value.HIPaymentPercent = Convert.ToDouble(value.LimQtyAndHIPrice.ItemNumber2MaxPayPerc);
                    if (value.GenMedProductID == null)
                    {
                        value.TotalHIPayment = 0;
                    }
                    else
                    {
                        //▼===== #010: Loại bỏ code cứng sử dụng cột MaxBenefit trong bảng Lim để tính toán cho trường hợp đặc biệt
                        //20191219 TTM: Gán cứng chỗ này stent số 2 thì tính full giá những loại khác tính theo tỷ lệ.
                        //if (value.GenMedProductItem.RefGenDrugCatID_1 == 9)
                        //{
                        //value.TotalHIPayment = Math.Min(value.LimQtyAndHIPrice.ItemNumber2MaxPayAmt, value.ChargeableItem.HIAllowedPrice.GetValueOrDefault(0)
                        //    * (decimal)value.HIPaymentPercent)

                        //    * (CurentRegistration.IsCrossRegion.GetValueOrDefault(false) ?

                        //    (decimal)Globals.ServerConfigSection.HealthInsurances.HIRebatePercentage2015Level1_InPt :

                        //    value.LimQtyAndHIPrice.ItemNumber2MaxBenefit > 0 ? value.LimQtyAndHIPrice.ItemNumber2MaxBenefit : (decimal)HIBenefit);

                        //decimal PtHiBenefit = (decimal)HIBenefit;
                        //if (CurentRegistration.IsCrossRegion == true)
                        //{
                        //    if (value.LimQtyAndHIPrice.ItemNumber2MaxBenefit > 0)
                        //    {
                        //        PtHiBenefit = (decimal)Globals.ServerConfigSection.HealthInsurances.RebatePercentage2015Level1_InPt;
                        //    }
                        //}
                        //else
                        //{
                        //    if (value.LimQtyAndHIPrice.ItemNumber2MaxBenefit > 0)
                        //    {
                        //        PtHiBenefit = value.LimQtyAndHIPrice.ItemNumber2MaxBenefit;
                        //    }
                        //}

                        decimal PtHiBenefit = (decimal)HIBenefit;
                        if (value.LimQtyAndHIPrice.ItemNumber2MaxBenefit > 0)
                        {
                            if (CurentRegistration.IsCrossRegion == false)
                            {
                                PtHiBenefit = value.LimQtyAndHIPrice.ItemNumber2MaxBenefit;
                            }
                            else
                            {
                                PtHiBenefit = (decimal)Globals.ServerConfigSection.HealthInsurances.RebatePercentage2015Level1_InPt;
                            }
                        }

                        value.TotalHIPayment = Math.Min(value.LimQtyAndHIPrice.ItemNumber2MaxPayAmt, value.ChargeableItem.HIAllowedPrice.GetValueOrDefault(0) * (decimal)value.HIPaymentPercent) * PtHiBenefit;
                        //}
                        //else
                        //{
                        //    value.TotalHIPayment = Math.Min(value.LimQtyAndHIPrice.ItemNumber2MaxPayAmt, value.ChargeableItem.HIAllowedPrice.GetValueOrDefault(0) * (decimal)value.HIPaymentPercent) * (decimal)CurentRegistration.PtInsuranceBenefit;
                        //}
                        //▲===== #010
                    }
                }
                else if (value.CountValue == 3)
                {
                    value.HIPaymentPercent = Convert.ToDouble(value.LimQtyAndHIPrice.ItemNumber3MaxPayPerc);
                    if (value.GenMedProductID == null)
                    {
                        value.TotalHIPayment = 0;
                    }
                    else
                    {
                        decimal PtHiBenefit = (decimal)HIBenefit;
                        if (value.LimQtyAndHIPrice.ItemNumber3MaxBenefit > 0)
                        {
                            if (CurentRegistration.IsCrossRegion == false)
                            {
                                PtHiBenefit = value.LimQtyAndHIPrice.ItemNumber3MaxBenefit;
                            }
                            else
                            {
                                PtHiBenefit = (decimal)Globals.ServerConfigSection.HealthInsurances.RebatePercentage2015Level1_InPt;
                            }
                        }
                        //value.TotalHIPayment = Math.Min(value.LimQtyAndHIPrice.ItemNumber1MaxPayAmt2, value.ChargeableItem.HIAllowedPrice.GetValueOrDefault(0) * (decimal)value.HIPaymentPercent) * (CurentRegistration.IsCrossRegion.GetValueOrDefault(false) ? (decimal)Globals.ServerConfigSection.HealthInsurances.RebatePercentageLevel1 / (value.HIPaymentPercent != 0 ? (decimal)value.HIPaymentPercent : 1.0m) : 1.0m);
                        value.TotalHIPayment = Math.Min(value.LimQtyAndHIPrice.ItemNumber3MaxPayAmt, value.ChargeableItem.HIAllowedPrice.GetValueOrDefault(0) * (decimal)value.HIPaymentPercent) * PtHiBenefit;
                    }
                }
                else
                {
                    value.IsCountHI = false;
                    value.HIPaymentPercent = 0;
                    value.TotalHIPayment = 0;
                    value.HIPayment = 0;
                }
                if (value.TotalHIPayment == 0)
                {
                    value.IsCountHI = false;
                }
                //20191219 TTM: Đối với những dòng trong gói rồi thì luôn mặc định bệnh nhân chi trả = 0. 
                if (value.IsInPackage)
                {
                    value.TotalPatientPayment = 0;
                }
                else
                {
                    //20191219 TTM: TotalInvoicePrice phải được kiểm tra < 0 vì TotalInvoicePrice là thành tiền <=> Tổng bảo hiểm chi + tổng bệnh nhân chi trả <=> Số lượng * đơn giá.
                    //              Nên không thể bé hơn 0 được vì thế giá trị min trong trường hợp tính cho tiền bệnh nhân chi trả phải từ tiền bảo hiểm chi trả trở lên.
                    value.TotalPatientPayment = value.TotalInvoicePrice < value.TotalHIPayment ? 0 : value.TotalInvoicePrice - value.TotalHIPayment;
                }
            }
        }
        private bool _IsNewCreateBill = false;
        public bool IsNewCreateBill
        {
            get
            {
                return _IsNewCreateBill;
            }
            set
            {
                if (_IsNewCreateBill != value)
                {
                    _IsNewCreateBill = value;
                    NotifyOfPropertyChange(() => IsNewCreateBill);
                }
            }
        }

        public void ckbDiscount_Click(object source, object sender)
        {
            if (!(source is CheckBox))
            {
                return;
            }
            CheckBox ckbDiscount = source as CheckBox;
            bool? copierChecked = ckbDiscount.IsChecked;
            if (!(ckbDiscount.DataContext is MedRegItemBase) || CurentRegistration == null || CurentRegistration.PromoDiscountProgramObj == null)
            {
                ckbDiscount.IsChecked = false;
                (ckbDiscount.DataContext as MedRegItemBase).DiscountAmt = 0;
                CommonGlobals.ChangeItemsRecordState((ckbDiscount.DataContext as MedRegItemBase));
                return;
            }
            (ckbDiscount.DataContext as MedRegItemBase).PromoDiscProgID = CurentRegistration.PromoDiscountProgramObj == null ? null : (long?)CurentRegistration.PromoDiscountProgramObj.PromoDiscProgID;
            ModifiItemCmd(ckbDiscount.DataContext as MedRegItemBase);
            if (CurentRegistration.PromoDiscountProgramObj != null && CurentRegistration.PromoDiscountProgramObj.IsOnPriceDiscount)
            {
                (ckbDiscount.DataContext as MedRegItemBase).IsDiscountChecked = ckbDiscount.IsChecked.GetValueOrDefault(false);
                if (!(ckbDiscount.DataContext as MedRegItemBase).IsDiscountChecked)
                {
                    if ((MessageBox.Show(eHCMSResources.Z2976_G1_XoaThongTinMienGiam, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No)) == MessageBoxResult.No)
                    {
                        ckbDiscount.IsChecked = !(bool)copierChecked;
                        (ckbDiscount.DataContext as MedRegItemBase).IsDiscountChecked = !(bool)copierChecked;
                        CurentRegistration.ApplyDiscount((ckbDiscount.DataContext as MedRegItemBase), Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, !((source as CheckBox).IsChecked == true));
                        CommonGlobals.ChangeItemsRecordState((ckbDiscount.DataContext as MedRegItemBase));
                        return;
                    }
                }
                CurentRegistration.ApplyDiscount((ckbDiscount.DataContext as MedRegItemBase), Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, !((source as CheckBox).IsChecked == true));
                CommonGlobals.ChangeItemsRecordState((ckbDiscount.DataContext as MedRegItemBase));
                if (copierChecked == true)
                {
                    ckbDiscount.IsChecked = true;
                }
                return;
            }
            if ((ckbDiscount.DataContext as MedRegItemBase).DiscountAmt == 0 && CurentRegistration.PromoDiscountProgramObj != null && CurentRegistration.PromoDiscountProgramObj.DiscountPercent == 0)
            {
                ckbDiscount.IsChecked = false;
                (ckbDiscount.DataContext as MedRegItemBase).DiscountAmt = 0;
                CommonGlobals.ChangeItemsRecordState((ckbDiscount.DataContext as MedRegItemBase));
                return;
            }
            if ((ckbDiscount.DataContext as MedRegItemBase).PromoDiscProgID != null && (ckbDiscount.DataContext as MedRegItemBase).PromoDiscProgID != CurentRegistration.PromoDiscountProgramObj.PromoDiscProgID)
            {
                (ckbDiscount.DataContext as MedRegItemBase).PromoDiscProgID = 0;
            }
            if (!ckbDiscount.IsChecked.GetValueOrDefault(false))
            {
                if ((MessageBox.Show(eHCMSResources.Z2976_G1_XoaThongTinMienGiam, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No)) == MessageBoxResult.No)
                {
                    ckbDiscount.IsChecked = !(bool)copierChecked;
                    (ckbDiscount.DataContext as MedRegItemBase).IsDiscountChecked = !(bool)copierChecked;
                }
            }
            if (ckbDiscount.DataContext is PatientPCLRequestDetail)
            {
                PatientPCLRequestDetail detail = ckbDiscount.DataContext as PatientPCLRequestDetail;
                //BLQ: Kiểm tra miễn giảm không có cls nào thì return
                if (CurentRegistration.PromoDiscountProgramObj != null && CurentRegistration.PromoDiscountProgramObj.PromoDiscountItems != null
                    && CurentRegistration.PromoDiscountProgramObj.PromoDiscountItems.Count() > 0
                    && CurentRegistration.PromoDiscountProgramObj.PromoDiscountItems.Where(x => x.PCLExamTypeID > 0).Count() == 0)
                {
                    ckbDiscount.IsChecked = !ckbDiscount.IsChecked;
                    MessageBox.Show(eHCMSResources.Z3231_G1_ThongBaoKhongCoMienGiam, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                //BLQ: Kiểm tra CLS đang check có trong phiếu miễn giảm không
                if (CurentRegistration.PromoDiscountProgramObj != null && CurentRegistration.PromoDiscountProgramObj.PromoDiscountItems != null
                    && CurentRegistration.PromoDiscountProgramObj.PromoDiscountItems.Count() > 0
                    && CurentRegistration.PromoDiscountProgramObj.PromoDiscountItems.Where(x => x.ObjPCLExamType != null && x.ObjPCLExamType.PCLExamTypeName == detail.PCLExamType.PCLExamTypeName).Count() == 0)
                {
                    ckbDiscount.IsChecked = !ckbDiscount.IsChecked;
                    MessageBox.Show(eHCMSResources.Z3231_G1_ThongBaoKhongCoMienGiam, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            if (ckbDiscount.DataContext is PatientRegistrationDetail)
            {
                PatientRegistrationDetail detail = ckbDiscount.DataContext as PatientRegistrationDetail;
                //BLQ: Kiểm tra không có dịch vụ thì return
                if (CurentRegistration.PromoDiscountProgramObj != null && CurentRegistration.PromoDiscountProgramObj.PromoDiscountItems != null
                    && CurentRegistration.PromoDiscountProgramObj.PromoDiscountItems.Count() > 0
                    && CurentRegistration.PromoDiscountProgramObj.PromoDiscountItems.Where(x => x.MedServiceID > 0).Count() == 0)
                {
                    ckbDiscount.IsChecked = !ckbDiscount.IsChecked;
                    MessageBox.Show(eHCMSResources.Z3231_G1_ThongBaoKhongCoMienGiam, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                //BLQ: Kiểm tra dịch vụ đang check có trong phiếu miễn giảm không
                if (CurentRegistration.PromoDiscountProgramObj != null && CurentRegistration.PromoDiscountProgramObj.PromoDiscountItems != null
                    && CurentRegistration.PromoDiscountProgramObj.PromoDiscountItems.Count() > 0
                    && CurentRegistration.PromoDiscountProgramObj.PromoDiscountItems.Where(x => x.ObjRefMedicalServiceItem != null && x.ObjRefMedicalServiceItem.MedServiceName == detail.MedServiceName).Count() == 0)
                {
                    ckbDiscount.IsChecked = !ckbDiscount.IsChecked;
                    MessageBox.Show(eHCMSResources.Z3231_G1_ThongBaoKhongCoMienGiam, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            CurentRegistration.ApplyDiscount((ckbDiscount.DataContext as MedRegItemBase), Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, !((source as CheckBox).IsChecked == true));
            if ((ckbDiscount.DataContext as MedRegItemBase) is PatientPCLRequestDetail)
            {
                ModifiItemCmd((ckbDiscount.DataContext as MedRegItemBase));
            }
        }
        public bool CanEditHICount { get; set; } = true;
        public void RecacucateAllItemValues()
        {
            if (AllRegistrationItems == null || AllRegistrationItems.Count <= 0)
            {
                return;
            }
            foreach (MedRegItemBase item in AllRegistrationItems)
            {
                CalcInvoiceItem(item);
                ModifiItemCmd(item);
            }
        }
        private bool _IsQuotationView = false;
        public bool IsQuotationView
        {
            get { return _IsQuotationView; }
            set
            {
                if (_IsQuotationView != value)
                {
                    _IsQuotationView = value;
                    NotifyOfPropertyChange(() => IsQuotationView);
                }
            }
        }

        //▼====: #015
        private Visibility _IsShowPatientCOVID = Visibility.Collapsed;
        public Visibility IsShowPatientCOVID
        {
            get
            {
                return _IsShowPatientCOVID;
            }
            set
            {
                if (_IsShowPatientCOVID != value)
                {
                    _IsShowPatientCOVID = value;
                    //▼====: #019
                    mCheckCovid = Globals.CheckOperation(Globals.listRefModule, (int)eModules.mPatient, (int)ePatient.mInPatientRegister_TV, (int)oCreateBill.mCheckCovid);
                    //▲====: #019
                    NotifyOfPropertyChange(() => IsShowPatientCOVID);
                }
            }
        }
        //▲====: #015

        //▼====: #020
        public void chkCountSE_Click(object source, object sender)
        {
            if(SelectedRegistrationItem == null)
            {
                return;
            }
            if(HIBenefit.GetValueOrDefault(0) > 0)
            {
                SelectedRegistrationItem.IsCountHI = !SelectedRegistrationItem.IsCountSE;
                chkCountHI_Click(source, sender);
            }
            else
            {
                ModifiItemCmd(SelectedRegistrationItem);
            }
        }

        private bool _IsEnableCountSE = false;
        public bool IsEnableCountSE
        {
            get
            {
                return _IsEnableCountSE;
            }
            set
            {
                _IsEnableCountSE = value;
                NotifyOfPropertyChange(() => IsEnableCountSE);
            }
        }
        //▲====: #020

        private decimal _PaymentCeilingForTechService;
        public decimal PaymentCeilingForTechService
        {
            get
            {
                return _PaymentCeilingForTechService;
            }
            set
            {
                _PaymentCeilingForTechService = value;
                NotifyOfPropertyChange(() => PaymentCeilingForTechService);
            }
        }

        public void Handle(CalcPaymentCeilingForTechServiceEvent message)
        {
            if(message != null)
            {
                PaymentCeilingForTechService = message.PaymentCeilingForTechService;
            }
        }
    }
}
