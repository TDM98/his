using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using aEMR.Common.BaseModel;
using System.Windows.Data;
using System.Collections.ObjectModel;
using DataEntities;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using aEMR.Controls;
using System.Threading;
using aEMR.ServiceClient;
using aEMR.Infrastructure;
using System.Linq;
using System;
using eHCMSLanguage;
using aEMR.Common;
using System.Collections.Generic;
using aEMR.Common.Converters;
using aEMR.Common.Collections;
using aEMR.Infrastructure.Events;
/*
 *  20200417 #001 TTM:  BM 0032132: BM 0032132: Fix lỗi xuất y cụ hàng loạt không hiển thị nếu số lượng duyệt = 0 của kho dược và thêm cảnh báo khi số lượng bằng 0, tự động clear dòng nào có số lượng xuất bằng 0.
 *  20200728 #002 TTM:  BM: 0039419: Fix lỗi không lưu VAT khi xuất hàng loạt.
 *  20210223 #003 TNHX: 214: Cho phép xuất 0.5 trong danh sách cấu hình
 */
namespace aEMR.StoreDept.ViewModels
{
    [Export(typeof(ICreateOutwardFromReqInvoice)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CreateOutwardFromReqInvoiceViewModel : ViewModelBase, ICreateOutwardFromReqInvoice
    {
        #region Properties
        private ICollectionView _CollectionView_ReqDetails;
        private ObservableCollection<OutwardDrugClinicDept> _OutwardDrugCollection;
        private long _V_MedProductType;
        private decimal _TotalInvoicePrice;
        private decimal _TotalHIPayment;
        private decimal _TotalPatientPayment;
        private bool _CheckAllOutwardDetail;
        private bool _CheckAllCountHI;
        private bool _CheckAllCountPatient;

        public ICollectionView CollectionView_ReqDetails
        {
            get
            {
                return _CollectionView_ReqDetails;
            }
            set
            {
                if (_CollectionView_ReqDetails == value)
                {
                    return;
                }
                _CollectionView_ReqDetails = value;
                NotifyOfPropertyChange(() => CollectionView_ReqDetails);
            }
        }

        public ObservableCollection<OutwardDrugClinicDept> OutwardDrugCollection
        {
            get
            {
                return _OutwardDrugCollection;
            }
            set
            {
                if (_OutwardDrugCollection == value)
                {
                    return;
                }
                _OutwardDrugCollection = value;
                NotifyOfPropertyChange(() => OutwardDrugCollection);
                CollectionView_ReqDetails = CollectionViewSource.GetDefaultView(OutwardDrugCollection);
                CollectionView_ReqDetails.GroupDescriptions.Add(new PropertyGroupDescription("DrugInvoice.CustomerName"));
            }
        }

        public long V_MedProductType
        {
            get
            {
                return _V_MedProductType;
            }
            set
            {
                if (_V_MedProductType == value)
                {
                    return;
                }
                _V_MedProductType = value;
                NotifyOfPropertyChange(() => V_MedProductType);
            }
        }

        public long StoreID { get; set; }

        public List<InwardDrugClinicDept> RemainingInwardDrugClinicDept { get; set; }

        public decimal TotalInvoicePrice
        {
            get
            {
                return _TotalInvoicePrice;
            }
            set
            {
                if (_TotalInvoicePrice != value)
                {
                    _TotalInvoicePrice = value;
                    NotifyOfPropertyChange(() => TotalInvoicePrice);
                }
            }
        }

        public decimal TotalHIPayment
        {
            get
            {
                return _TotalHIPayment;
            }
            set
            {
                if (_TotalHIPayment != value)
                {
                    _TotalHIPayment = value;
                    NotifyOfPropertyChange(() => TotalHIPayment);
                }
            }
        }

        public decimal TotalPatientPayment
        {
            get
            {
                return _TotalPatientPayment;
            }
            set
            {
                if (_TotalPatientPayment != value)
                {
                    _TotalPatientPayment = value;
                    NotifyOfPropertyChange(() => TotalPatientPayment);
                }
            }
        }

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
                    CountHIOrNot(_CheckAllCountHI);
                    SumTotalPrice();
                }
            }
        }

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
                    CountPatientOrNot(_CheckAllCountPatient);
                    SumTotalPrice();
                }
            }
        }
        #endregion

        #region Events
        public void gvDetails_Loaded(object sender, RoutedEventArgs e)
        {
            DataGrid grdPrescription = sender as DataGrid;
            if (grdPrescription == null)
            {
                return;
            }
            var col = grdPrescription.GetColumnByName("colMedicalMaterial");
            var colMDoseStr = grdPrescription.GetColumnByName("colMDoseStr");
            var colADoseStr = grdPrescription.GetColumnByName("colADoseStr");
            var colEDoseStr = grdPrescription.GetColumnByName("colEDoseStr");
            var colNDoseStr = grdPrescription.GetColumnByName("colNDoseStr");
            if (col == null || colMDoseStr == null || colADoseStr == null || colEDoseStr == null || colNDoseStr == null)
            {
                return;
            }
            if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
            {
                col.Visibility = Visibility.Visible;
            }
            else
            {
                col.Visibility = Visibility.Collapsed;
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
            LoadInwardRemaining();
        }

        public void chkCountPatient_Click(object sender, RoutedEventArgs e)
        {
            SumTotalPrice();
        }

        public void chkCountHI_Click(object sender, RoutedEventArgs e)
        {
            CheckBox CurrentCheckBox = sender as CheckBox;
            if (CurrentCheckBox == null)
            {
                return;
            }
            OutwardDrugClinicDept CurrentOutwardDrugClinicDept = CurrentCheckBox.DataContext as OutwardDrugClinicDept;
            if (OutwardDrugCollection == null || OutwardDrugCollection.Count == 0 || CurrentOutwardDrugClinicDept == null)
            {
                return;
            }
            CountHIItem(CurrentOutwardDrugClinicDept);
        }

        public void chkAllCountPatient_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chkAllCountPatient = sender as CheckBox;
            if (chkAllCountPatient == null)
            {
                return;
            }
            CountPatientOrNot(chkAllCountPatient.IsChecked.GetValueOrDefault());
            SumTotalPrice();
        }

        public void chkAllCountHI_Click(object sender, RoutedEventArgs e)
        {
            CheckBox CurrentCheckBox = sender as CheckBox;
            if (CurrentCheckBox == null || OutwardDrugCollection == null || OutwardDrugCollection.Count == 0)
            {
                return;
            }
            CountHIOrNot(CurrentCheckBox.IsChecked.GetValueOrDefault());
            SumTotalPrice();
        }

        public void chkOutwardDetail_Click(object sender, RoutedEventArgs e)
        {
            SetCheckAllOutwardDetail();
        }

        public void lnkChooseBatchNumber_Click(object sender, RoutedEventArgs e)
        {
            Button lnkBatchNumber = sender as Button;
            long GenMedProductID = (long)lnkBatchNumber.CommandParameter;
        }

        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            Button CurrentButton = sender as Button;
            OutwardDrugClinicDept CurrentOutwardDrugClinicDept = CurrentButton.DataContext as OutwardDrugClinicDept;
            if (CurrentOutwardDrugClinicDept == null)
            {
                return;
            }
            //if (OutwardDrugCollection.Any(x => x.GenMedProductID == CurrentOutwardDrugClinicDept.GenMedProductID))
            //{
            //    OutwardDrugCollection.First(x => x.GenMedProductID == CurrentOutwardDrugClinicDept.GenMedProductID).RequestQty += CurrentOutwardDrugClinicDept.RequestQty;
            //}
            OutwardDrugCollection.Remove(CurrentOutwardDrugClinicDept);
            SumTotalPrice();
        }

        public void btnSave()
        {
            if (OutwardDrugCollection == null || OutwardDrugCollection.Count == 0)
            {
                return;
            }
            if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
            {
                SetV_MedicalMaterial();
            }
            if (!CheckData())
            {
                return;
            }
            ////20191227 TBL: Chỉ có thuốc mới kiểm tra liều dùng
            //if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC && OutwardDrugCollection != null
            //    && OutwardDrugCollection.Any(x => x.GenMedProductItem.InsuranceCover.GetValueOrDefault() && (x.MDose + x.ADose + x.EDose + x.NDose) <= 0.0 && string.IsNullOrEmpty(x.Administration)))
            //{
            //    MessageBox.Show(eHCMSResources.Z1069_G1_LieuDungKgHopLe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            //    return;
            //}
            if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC && OutwardDrugCollection != null
                    && OutwardDrugCollection.Any(x => x.GenMedProductItem.InsuranceCover.GetValueOrDefault() && (x.MDose + x.ADose + x.EDose + x.NDose) <= 0.0 && string.IsNullOrEmpty(x.Administration)))
            {
                foreach (var item in OutwardDrugCollection)
                {
                    if (item.GenMedProductItem.InsuranceCover.GetValueOrDefault() && (item.MDose + item.ADose + item.EDose + item.NDose) <= 0.0 && string.IsNullOrEmpty(item.Administration))
                    {
                        item.Administration = eHCMSResources.Z2923_G1_SuDungTheoYLenhBacSi;

                    }
                }
            }
            int temp = 0;
            foreach (var tmpOutwardDrugClinicDept in OutwardDrugCollection)
            {
                temp++;
                if (tmpOutwardDrugClinicDept.OutQuantity > tmpOutwardDrugClinicDept.GenMedProductItem.RemainingFirst)
                {
                    MessageBox.Show(string.Format(eHCMSResources.Z2308_G1_KhongXuatVuotSLTrongLo
                        , tmpOutwardDrugClinicDept.GenMedProductItem.BrandName
                        , tmpOutwardDrugClinicDept.InBatchNumber
                        , tmpOutwardDrugClinicDept.GenMedProductItem.RemainingFirst
                        , tmpOutwardDrugClinicDept.OutQuantity));
                    return;
                }
                //▼====: #003
                if (Globals.ServerConfigSection.ClinicDeptElements.ThuocDuocXuatThapPhan != "" && (tmpOutwardDrugClinicDept.OutQuantity != (int)tmpOutwardDrugClinicDept.OutQuantity)
                    && !Globals.ServerConfigSection.ClinicDeptElements.ThuocDuocXuatThapPhan.Contains(tmpOutwardDrugClinicDept.ChargeableItemCode))
                {
                    MessageBox.Show(string.Format(eHCMSResources.Z1400_G1_SLgXuatLaSoNguyen
                        , (temp + ": (" + tmpOutwardDrugClinicDept.ChargeableItemCode + ") - " + tmpOutwardDrugClinicDept.GenMedProductItem.BrandName)));
                    return;
                }
                //▲====: #003
            }
            List<OutwardDrugClinicDeptInvoice> InvoiceCollection = new List<OutwardDrugClinicDeptInvoice>();
            foreach (var item in OutwardDrugCollection)
            {
                if (InvoiceCollection.Any(x => x.PtRegistrationID == item.DrugInvoice.PtRegistrationID))
                {
                    continue;
                }
                OutwardDrugClinicDeptInvoice CurrentInvoice = item.DrugInvoice.DeepCopy();
                CurrentInvoice.OutwardDrugClinicDepts = OutwardDrugCollection.Where(x => x.DrugInvoice.PtRegistrationID == CurrentInvoice.PtRegistrationID).ToObservableCollection();
                InvoiceCollection.Add(CurrentInvoice);
            }
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginOutwardDrugClinicDeptInvoices_SaveByItemCollection(InvoiceCollection, GetStaffLogin().StaffID, Globals.GetCurServerDateTime(), V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndOutwardDrugClinicDeptInvoices_SaveByItemCollection(asyncResult);
                                //20200205 TBL: BM 0022882: Khi xuất cho BN từ phiếu lĩnh thì load lại phiếu lĩnh để tránh trường hợp tiếp tục xuất
                                Globals.EventAggregator.Publish(new ReloadInPatientRequestingDrugListByReqID());
                                Globals.ShowMessage(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao);
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        public void chkAllOutwardDetail_Click(object sender, RoutedEventArgs e)
        {
            if (OutwardDrugCollection == null || OutwardDrugCollection.Count == 0)
            {
                return;
            }
            if (CheckAllOutwardDetail)
            {
                foreach (OutwardDrugClinicDept item in OutwardDrugCollection)
                {
                    item.IsChecked = true;
                }
            }
            else
            {
                foreach (OutwardDrugClinicDept item in OutwardDrugCollection)
                {
                    item.IsChecked = false;
                }
            }
        }
        #endregion

        #region Methods
        private void LoadInwardRemaining()
        {
            if (OutwardDrugCollection == null || OutwardDrugCollection.Count == 0 || !OutwardDrugCollection.Any(x => x.GenMedProductID.GetValueOrDefault(0) > 0))
            {
                return;
            }
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllInwardDrugClinicDeptByIDList(StoreID, V_MedProductType, OutwardDrugCollection.Where(x => x.GenMedProductID.GetValueOrDefault(0) > 0).Select(x => x.GenMedProductID.Value).Distinct().ToList(), Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                RemainingInwardDrugClinicDept = contract.EndGetAllInwardDrugClinicDeptByIDList(asyncResult);
                                if (RemainingInwardDrugClinicDept != null && RemainingInwardDrugClinicDept.Count > 0)
                                {
                                    //20200422 TBL: Dưới store đã order by theo cấu hình thì ở đây không cần phải làm nữa vì đang order by sai
                                    //if (Globals.ServerConfigSection.ClinicDeptElements.DrugDeptOutDrugExpiryDateRule == 1)
                                    //{
                                    //    RemainingInwardDrugClinicDept = RemainingInwardDrugClinicDept.OrderBy(x => x.GenMedProductID).OrderByDescending(x => x.InExpiryDate).ToList();
                                    //}
                                    //else
                                    //{
                                    //    RemainingInwardDrugClinicDept = RemainingInwardDrugClinicDept.OrderBy(x => x.GenMedProductID).OrderByDescending(x => x.InID).ToList();
                                    //}
                                    ObservableCollection<OutwardDrugClinicDept> OutwardOutwardDrugCollection = new ObservableCollection<OutwardDrugClinicDept>();
                                    foreach (var item in OutwardDrugCollection)
                                    {
                                        if (!RemainingInwardDrugClinicDept.Any(x => x.GenMedProductID == item.GenMedProductID))
                                        {
                                            OutwardOutwardDrugCollection.Add(item);
                                            continue;
                                        }
                                        foreach (var InwardItem in RemainingInwardDrugClinicDept.Where(x => x.GenMedProductID == item.GenMedProductID))
                                        {
                                            if (item.RequestQty == 0)
                                            {
                                                break;
                                            }
                                            //▼===== #001: Ý anh Tuân số lượng bằng 0 cũng hiển thị.
                                            //if (InwardItem.Remaining == 0)
                                            //{
                                            //    continue;
                                            //}
                                            //▲===== 
                                            if (InwardItem.Remaining > item.RequestQty)
                                            {
                                                item.OutQuantity = item.RequestQty;
                                                InwardItem.Remaining -= item.RequestQty;
                                                item.InBatchNumber = InwardItem.InBatchNumber;
                                                item.HIAllowedPrice = InwardItem.HIAllowedPrice;
                                                item.OutPrice = item.HIBenefit.GetValueOrDefault(0) > 0 ? InwardItem.HIPatientPrice : InwardItem.NormalPrice;
                                                item.GenMedProductItem = InwardItem.RefGenMedProductDetails;
                                                item.InExpiryDate = InwardItem.RefGenMedProductDetails.InExpiryDate;
                                                item.InID = InwardItem.InID;
                                                item.GetItemTotalPrice();
                                                //▼===== #002
                                                item.VAT = (double?)InwardItem.VAT;
                                                //▲===== #002
                                                OutwardOutwardDrugCollection.Add(item);
                                                break;
                                            }
                                            else
                                            {
                                                var AddedItem = item.DeepCopy();
                                                AddedItem.OutQuantity = InwardItem.Remaining;
                                                AddedItem.InBatchNumber = InwardItem.InBatchNumber;
                                                AddedItem.HIAllowedPrice = InwardItem.HIAllowedPrice;
                                                AddedItem.OutPrice = item.HIBenefit.GetValueOrDefault(0) > 0 ? InwardItem.HIPatientPrice : InwardItem.NormalPrice;
                                                AddedItem.GenMedProductItem = InwardItem.RefGenMedProductDetails;
                                                AddedItem.InExpiryDate = InwardItem.RefGenMedProductDetails.InExpiryDate;
                                                AddedItem.InID = InwardItem.InID;
                                                AddedItem.GetItemTotalPrice();
                                                //▼===== #002
                                                AddedItem.VAT = (double?)InwardItem.VAT;
                                                //▲===== #002
                                                item.RequestQty -= InwardItem.Remaining;
                                                InwardItem.Remaining = 0;
                                                OutwardOutwardDrugCollection.Add(AddedItem);
                                            }
                                        }
                                    }
                                    OutwardDrugCollection = OutwardOutwardDrugCollection;
                                    CheckAllCountHI = true;
                                    CheckAllCountPatient = true;
                                    //▼===== #001
                                    int count = 0;
                                    string Error = "";
                                    foreach (var item in OutwardDrugCollection)
                                    {
                                        count++;
                                        if (item.OutQuantity == 0)
                                        {
                                            Error += string.Format(eHCMSResources.Z3015_G1_DongSo, (count).ToString(), "Số lượng tồn = 0. \n");
                                        }
                                    }
                                    ObservableCollection<OutwardDrugClinicDept> outwardItem = OutwardDrugCollection.Where(x => x.OutQuantity <= 0).ToObservableCollection();
                                    if (outwardItem != null && outwardItem.Count > 0)
                                    {
                                        string errornew = string.Concat(Error, eHCMSResources.A0429_G1_Msg_InfoTuDongXoaDongSLgXuat0);
                                        if (MessageBox.Show(errornew, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                        {
                                            DeleteOutwardList(outwardItem);
                                        }
                                    }
                                    //▲===== #001
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

        private void SumTotalPrice()
        {
            if (OutwardDrugCollection == null || OutwardDrugCollection.Count == 0)
            {
                return;
            }
            bool onlyRoundResultForOutward = Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward;
            foreach (OutwardDrugClinicDept item in OutwardDrugCollection)
            {
                if (item.OutID > 0)
                {
                    item.OutPrice = item.IsCountHI && item.HIBenefit.GetValueOrDefault(0) > 0 ? item.ChargeableItem.HIPatientPriceNew : item.ChargeableItem.NormalPriceNew;
                }
                else
                {
                    item.OutPrice = item.IsCountHI && item.HIBenefit.GetValueOrDefault(0) > 0 ? item.ChargeableItem.HIPatientPrice : item.ChargeableItem.NormalPrice;
                }
                if (item.HIAllowedPrice.GetValueOrDefault() > 0)
                {
                    item.HIBenefit = item.HIBenefit.GetValueOrDefault(0);
                }
                if (item.IsCountHI)
                {
                    item.TotalHIPayment = item.HIAllowedPrice.GetValueOrDefault() * item.OutQuantity * (decimal)item.HIBenefit.GetValueOrDefault(0);
                }
                else
                {
                    item.TotalHIPayment = 0;
                }
                if (!item.IsCountPatient)
                {
                    item.OutAmount = item.TotalHIPayment;
                }
                item.TotalPatientPayment = item.OutAmount.GetValueOrDefault() - item.TotalHIPayment;
                item.Qty = item.RequestQty;
                item.InvoicePrice = item.OutPrice;
                item.TotalInvoicePrice = item.OutAmount.GetValueOrDefault(0);
                item.TotalCoPayment = item.HIAllowedPrice.GetValueOrDefault() * item.OutQuantity * (decimal)(1 - item.HIBenefit.GetValueOrDefault(0));
            }
            CalcInvoicePrice();
        }

        private void CalcInvoicePrice()
        {
            TotalInvoicePrice = 0;
            TotalHIPayment = 0;
            TotalPatientPayment = 0;
            if (OutwardDrugCollection == null || OutwardDrugCollection.Count == 0)
            {
                return;
            }
            bool onlyRoundResultForOutward = Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward;
            foreach (OutwardDrugClinicDept item in OutwardDrugCollection)
            {
                TotalInvoicePrice += item.OutAmount.GetValueOrDefault();
                TotalHIPayment += item.TotalHIPayment;
            }
            if (onlyRoundResultForOutward)
            {
                TotalInvoicePrice = MathExt.Round(TotalInvoicePrice, Common.Converters.MidpointRounding.AwayFromZero);
                TotalHIPayment = MathExt.Round(TotalHIPayment, Common.Converters.MidpointRounding.AwayFromZero);
            }
            TotalPatientPayment = TotalInvoicePrice - TotalHIPayment;
        }

        private void CountPatientOrNot(bool value)
        {
            if (OutwardDrugCollection == null || OutwardDrugCollection.Count == 0)
            {
                return;
            }
            foreach (OutwardDrugClinicDept item in OutwardDrugCollection)
            {
                item.IsCountPatient = value;
            }
        }

        private void CountHIOrNot(bool value)
        {
            if (OutwardDrugCollection == null || OutwardDrugCollection.Count == 0)
            {
                return;
            }
            foreach (OutwardDrugClinicDept item in OutwardDrugCollection)
            {
                if (item.GenMedProductItem == null)
                {
                    continue;
                }
                if (value && item.HIBenefit.GetValueOrDefault() > 0 && item.GenMedProductItem.InsuranceCover.GetValueOrDefault())
                {
                    item.IsCountHI = true;
                }
                else
                {
                    item.IsCountHI = false;
                }
            }
        }

        private void SetCheckAllOutwardDetail()
        {
            if (OutwardDrugCollection == null || OutwardDrugCollection.Count == 0)
            {
                CheckAllOutwardDetail = false;
                return;
            }
            CheckAllOutwardDetail = OutwardDrugCollection.All(x => x.IsChecked);
        }

        private void SetV_MedicalMaterial()
        {
            if (OutwardDrugCollection == null || OutwardDrugCollection.Count == 0)
            {
                return;
            }
            foreach (OutwardDrugClinicDept outward in OutwardDrugCollection)
            {
                if (outward.IsReplaceMedMat)
                {
                    outward.V_MedicalMaterial = (long)AllLookupValues.V_MedicalMaterial.VTYT_THAYTHE;
                }
                else if (outward.IsDisposeMedMat)
                {
                    outward.V_MedicalMaterial = (long)AllLookupValues.V_MedicalMaterial.VTYT_TIEUHAO;
                }
                else
                {
                    outward.V_MedicalMaterial = 0;
                }
            }
        }

        private Staff GetStaffLogin()
        {
            return Globals.LoggedUserAccount.Staff;
        }

        private bool CheckData()
        {
            if (OutwardDrugCollection == null || OutwardDrugCollection.Count == 0)
            {
                return false;
            }
            string strError = "";
            //if (SelectedOutInvoice.PtRegistrationID.GetValueOrDefault(0) <= 0 && SelectedOutInvoice.OutPtRegistrationID == 0)
            //{
            //    Globals.ShowMessage(eHCMSResources.K0290_G1_ChonBN, eHCMSResources.G0442_G1_TBao);
            //    return false;
            //}
            if (OutwardDrugCollection.Any(x => x.DrugInvoice.OutDate == null || x.DrugInvoice.OutDate.GetValueOrDefault().Date > Globals.GetCurServerDateTime().Date))
            {
                MessageBox.Show(eHCMSResources.A0863_G1_Msg_InfoNgXuatKhHopLe4, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            string strNumOfRow = "";
            string strRowWithoutDoctorStaffID = "";
            string strRowWithoutMedicalInstructionDate = "";
            string strRowInvalidMedicalInstructDate = "";
            //string strInvalidProduct = "";
            for (int i = 0; i < OutwardDrugCollection.Count; i++)
            {
                if (OutwardDrugCollection[i].GenMedProductItem != null && OutwardDrugCollection[i].OutQuantity <= 0)
                {
                    Globals.ShowMessage(eHCMSResources.Z1174_G1_SLgXuatLonHon0, eHCMSResources.G0442_G1_TBao);
                    return false;
                }
                if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU && OutwardDrugCollection[i].V_MedicalMaterial <= 0)
                {
                    strNumOfRow += (i + 1).ToString() + ", ";
                }
                if ((OutwardDrugCollection[i].DoctorStaff == null || OutwardDrugCollection[i].DoctorStaff.StaffID <= 0) && OutwardDrugCollection[i].GenMedProductItem.InsuranceCover.GetValueOrDefault(false))
                {
                    strRowWithoutDoctorStaffID += (i + 1).ToString() + ", ";
                }
                if (OutwardDrugCollection[i].MedicalInstructionDate == null && OutwardDrugCollection[i].GenMedProductItem.InsuranceCover.GetValueOrDefault(false))
                {
                    strRowWithoutMedicalInstructionDate += (i + 1).ToString() + ", ";
                }
                if (Globals.ServerConfigSection.InRegisElements.CheckMedicalInstructDate
                    && Globals.ServerConfigSection.InRegisElements.NumOfOverDaysForMedicalInstructDate >= 0
                    && (OutwardDrugCollection[i].MedicalInstructionDate.GetValueOrDefault() - Globals.GetCurServerDateTime().Date).Days > Globals.ServerConfigSection.InRegisElements.NumOfOverDaysForMedicalInstructDate)
                {
                    strRowInvalidMedicalInstructDate += (i + 1).ToString() + ", ";
                }
                //neu ngay het han lon hon ngay hien tai
                if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                {
                    if (eHCMS.Services.Core.AxHelper.CompareDate(DateTime.Now, OutwardDrugCollection[i].InExpiryDate.GetValueOrDefault()) == 1)
                    {
                        strError += string.Format(eHCMSResources.Z1401_G1_DaHetHanDung, OutwardDrugCollection[i].GenMedProductItem.BrandName, (i + 1).ToString()) + Environment.NewLine;
                    }
                }
            }
            if (!string.IsNullOrEmpty(strNumOfRow))
            {
                MessageBox.Show(string.Format(eHCMSResources.Z1177_G1_ChuaChonLoaiVTYT, strNumOfRow), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            if (!string.IsNullOrEmpty(strRowWithoutDoctorStaffID))
            {
                MessageBox.Show(string.Format(eHCMSResources.Z1179_G1_ChuaChonBSiCDinh, strRowWithoutDoctorStaffID), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            if (!string.IsNullOrEmpty(strRowWithoutMedicalInstructionDate))
            {
                MessageBox.Show(string.Format(eHCMSResources.Z1182_G1_ChuaChonNgYLenh, strRowWithoutMedicalInstructionDate), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            if (!string.IsNullOrEmpty(strRowInvalidMedicalInstructDate))
            {
                string msg = Globals.ServerConfigSection.InRegisElements.NumOfOverDaysForMedicalInstructDate == 0
                    ? string.Format(eHCMSResources.Z1875_G1_NgYLenhKgLonHonNgHTai2, strRowInvalidMedicalInstructDate)
                    : string.Format(eHCMSResources.Z1874_G1_NgYLenhKgLonHonNgHTai, Globals.ServerConfigSection.InRegisElements.NumOfOverDaysForMedicalInstructDate, strRowInvalidMedicalInstructDate);
                MessageBox.Show(msg, eHCMSResources.K1576_G1_CBao, MessageBoxButton.OK);
                return false;
            }
            if (!string.IsNullOrEmpty(strError))
            {
                if (MessageBox.Show(strError + Environment.NewLine + eHCMSResources.I0942_G1_I, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private void CountHIItem(OutwardDrugClinicDept CurrentOutwardDrugClinicDept)
        {
            string error = "";
            if (CurrentOutwardDrugClinicDept.HIBenefit.GetValueOrDefault() <= 0)
            {
                error = eHCMSResources.A0244_G1_Msg_InfoBNKhongCoBHYT;
            }
            else if (!CurrentOutwardDrugClinicDept.GenMedProductItem.InsuranceCover.GetValueOrDefault())
            {
                error = eHCMSResources.Z1099_G1_LoaiDVKgThuocDMBH;
            }
            if (CurrentOutwardDrugClinicDept.IsCountHI && !string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                CurrentOutwardDrugClinicDept.IsCountHI = false;
                return;
            }
            if (CurrentOutwardDrugClinicDept.OutID > 0 && CurrentOutwardDrugClinicDept.GenMedProductItem.HIAllowedPriceNew == 0
                || CurrentOutwardDrugClinicDept.OutID <= 0 && CurrentOutwardDrugClinicDept.GenMedProductItem.HIAllowedPrice == 0)
            {
                MessageBox.Show(eHCMSResources.Z2193_G1_LoKhongCoGiaBaoHiem, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                CurrentOutwardDrugClinicDept.IsCountHI = false;
                return;
            }
            SumTotalPrice();
        }
        #endregion
        //▼===== #001: Code để đánh số thứ tự cho dòng và xoá tự động nếu người dùng chấp nhận xoá tự động.
        private void DeleteOutwardList(ObservableCollection<OutwardDrugClinicDept> deleteOutwardDrugList)
        {
            if (deleteOutwardDrugList == null || deleteOutwardDrugList.Count <= 0)
            {
                return;
            }
            foreach (OutwardDrugClinicDept outwardDelete in deleteOutwardDrugList)
            {
                DeleteOutwardItem(outwardDelete);
            }
            SumTotalPrice();
        }
        private void DeleteOutwardItem(OutwardDrugClinicDept outwardDelete)
        {
            if (outwardDelete == null)
            {
                return;
            }
            OutwardDrugCollection.Remove(outwardDelete);
        }
        public void gvDetails_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }
        //▲===== #001
    }
}
