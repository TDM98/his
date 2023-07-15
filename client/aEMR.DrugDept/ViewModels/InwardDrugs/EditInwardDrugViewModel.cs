using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
using Castle.Windsor;
using eHCMSLanguage;

/*
 * 20191224 #001 TBL: BM 0021749: Nếu thuốc không có TT thầu thì Giá BH quy định không được lớn hơn 0 khi cập nhật
 * 20200328 #002 TTM: BM 0029055: Fix lỗi không cập nhật được tiền chiết khấu khi tích vào chiết khấu theo %, và đưa % chiết khấu sai khi bỏ tích chiết khấu % để nhập tiền chiết khấu.
 * 20200411 #003 TTM: BM 0029095: Fix lỗi cập nhật giá bán DV, giá BH, giá cho BNBH giá trị < 0
 * 20200807 #004 TNHX: Thêm loại để nhập hàng cho hóa chất
 */

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IDrugDeptEditInwardDrug)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class EditInwardDrugViewModel : Conductor<object>, IDrugDeptEditInwardDrug
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public EditInwardDrugViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            CurrentInwardDrugMedDeptCopy = new InwardDrugMedDept();
        }

        IResetConverter _currentView;
        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            _currentView = view as IResetConverter;
            ResetConverterAgain(IsVND);
        }

        private bool _IsVND;
        public bool IsVND
        {
            get { return _IsVND; }
            set
            {
                _IsVND = value;
                ResetConverterAgain(_IsVND);
            }
        }
        #region Properties Member

        private InwardDrugMedDept _CurrentInwardDrugMedDeptCopy;
        public InwardDrugMedDept CurrentInwardDrugMedDeptCopy
        {
            get
            {
                return _CurrentInwardDrugMedDeptCopy;
            }
            set
            {
                if (_CurrentInwardDrugMedDeptCopy != value)
                {
                    _CurrentInwardDrugMedDeptCopy = value;
                    if (_CurrentInwardDrugMedDeptCopy != null)
                    {
                        _ShowIsNotVAT = false;
                        IsNotVAT = _CurrentInwardDrugMedDeptCopy.IsNotVat;
                        _ShowIsNotVAT = true;
                    }
                    NotifyOfPropertyChange(() => CurrentInwardDrugMedDeptCopy);
                }
            }
        }

        private ObservableCollection<Lookup> _CbxGoodsTypes;
        public ObservableCollection<Lookup> CbxGoodsTypes
        {
            get
            {
                return _CbxGoodsTypes;
            }
            set
            {
                _CbxGoodsTypes = value;
                NotifyOfPropertyChange(() => CbxGoodsTypes);
            }
        }

        #endregion

        private bool _IsLoading = false;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set
            {
                if (_IsLoading != value)
                {
                    _IsLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        #region Auto For Location

        private ObservableCollection<RefShelfDrugLocation> _Location;
        public ObservableCollection<RefShelfDrugLocation> Location
        {
            get
            {
                return _Location;
            }
            set
            {
                if (_Location != value)
                {
                    _Location = value;
                    NotifyOfPropertyChange(() => Location);
                }
            }
        }

        AutoCompleteBox Au;
        private void SearchRefShelfLocation(string Name)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetRefShelfDrugLocationAutoComplete(Name, 0, Globals.PageSize, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetRefShelfDrugLocationAutoComplete(asyncResult);
                            Location = results.ToObservableCollection();
                            Au.ItemsSource = Location;
                            Au.PopulateComplete();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void AutoLocation_Text_Populating(object sender, PopulatingEventArgs e)
        {
            Au = sender as AutoCompleteBox;
            SearchRefShelfLocation(e.Parameter);
        }

        #endregion

        private void UpdateInwardDrug()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    //▼====== #004
                    contract.BeginUpdateInwardDrugMedDept(CurrentInwardDrugMedDeptCopy, Globals.LoggedUserAccount.Staff.StaffID, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                    {
                    //▲====== #004
                        try
                        {
                            int results = contract.EndUpdateInwardDrugMedDept(asyncResult);
                            if (results == 0)
                            {
                                TryClose();
                                //phat su kien o day
                                Globals.EventAggregator.Publish(new DrugDeptCloseEditInwardEvent { });
                            }
                            //KMx: Không sử dụng cách thông báo lỗi này nữa (19/12/2014 15:28).
                            //else if (results == 1)
                            //{
                            //    MessageBox.Show("Thuốc này đã được xuất > Số Lượng bạn muốn sửa.", eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                            //}
                            //else if (results == 2)
                            //{
                            //    MessageBox.Show(eHCMSResources.K0069_G1_ThuocTheoSoLoDaTonTai, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                            //}
                            //else
                            //{
                            //    MessageBox.Show(string.Format("{0}.", eHCMSResources.A0755_G1_Msg_InfoKhTonTaiDongData), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                            //}
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void OKButton(object sender, RoutedEventArgs e)
        {
            if (CheckValid())
            {
                //▼===== #001
                if (string.IsNullOrEmpty(CurrentInwardDrugMedDeptCopy.RefGenMedProductDetails.BidCode) && CurrentInwardDrugMedDeptCopy.RefGenMedProductDetails.HIAllowedPrice > 0)
                {
                    MessageBox.Show(eHCMSResources.Z2947_G1_KhogCoTTThau, eHCMSResources.G0442_G1_TBao);
                    return;
                }
                //▲===== #001
                if (CurrentInwardDrugMedDeptCopy.InQuantity <= 0)
                {
                    MessageBox.Show(eHCMSResources.A0789_G1_Msg_InfoSLgNhapLonHon0, eHCMSResources.G0442_G1_TBao);
                    return;
                }
                if (CurrentInwardDrugMedDeptCopy.InBuyingPrice == 0 && CurrentInwardDrugMedDeptCopy.V_GoodsType == (long)AllLookupValues.V_GoodsType.HANGMUA)
                {
                    MessageBox.Show(eHCMSResources.A0556_G1_Msg_InfoGiaMuaLonHon0);
                    return;
                }
                if ((CurrentInwardDrugMedDeptCopy.DiscountingByPercent < 1 && CurrentInwardDrugMedDeptCopy.DiscountingByPercent > 0) || (CurrentInwardDrugMedDeptCopy.DiscountingByPercent > 2))
                {
                    MessageBox.Show(string.Format("{0}.", eHCMSResources.A0072_G1_CKKhHopLe));
                    return;
                }
                //KMx: Khoa Dược yêu cầu không có giá bán vẫn cho lưu (hàng tặng) (01/07/2016 10:27).
                //if (CurrentInwardDrugMedDeptCopy.RefGenMedProductDetails.NormalPrice <= 0)
                //{
                //    Globals.ShowMessage(string.Format("{0}!", eHCMSResources.A0786_G1_Msg_InfoCNhatBGiaBan), eHCMSResources.G0442_G1_TBao);
                //    return;
                //}
                if (CurrentInwardDrugMedDeptCopy.RefGenMedProductDetails.NormalPrice < CurrentInwardDrugMedDeptCopy.RefGenMedProductDetails.HIPatientPrice)
                {
                    MessageBox.Show(eHCMSResources.Z0573_G1_GiaBNBHNhoHonDGia);
                    return;
                }
                if (CurrentInwardDrugMedDeptCopy.RefGenMedProductDetails.HIAllowedPrice > CurrentInwardDrugMedDeptCopy.RefGenMedProductDetails.HIPatientPrice)
                {
                    MessageBox.Show(eHCMSResources.T1982_G1_GiaBHChoPhep2);
                    return;
                }
                if(CurrentInwardDrugMedDeptCopy.VAT < 0 || CurrentInwardDrugMedDeptCopy.VAT > 1)
                {
                    MessageBox.Show(eHCMSResources.Z2991_G1_VATKhongHopLe);
                    return;
                }
                //▼===== #003
                if (CurrentInwardDrugMedDeptCopy.RefGenMedProductDetails.NormalPrice < 0)
                {
                    MessageBox.Show(eHCMSResources.Z3013_G1_GiaBanDVKhongDuocNhoHon0);
                    return;
                }
                if (CurrentInwardDrugMedDeptCopy.RefGenMedProductDetails.HIAllowedPrice < 0)
                {
                    MessageBox.Show(eHCMSResources.Z3012_G1_GiaBHKhongDuocNhoHon0);
                    return;
                }
                if (CurrentInwardDrugMedDeptCopy.RefGenMedProductDetails.HIPatientPrice < 0)
                {
                    MessageBox.Show(eHCMSResources.Z3011_G1_GiaBNBHKhongDuocNhoHon0);
                    return;
                }
                if (CurrentInwardDrugMedDeptCopy.TotalPriceNotVAT < 0)
                {
                    MessageBox.Show(eHCMSResources.Z3014_G1_ThanhTienKhongDuocNhoHon0);
                    return;
                }
                //▲===== #003
                if (Au != null)
                {
                    CurrentInwardDrugMedDeptCopy.SdlDescription = Au.Text;
                }
                if (V_MedProductType == (long)AllLookupValues.MedProductType.TIEM_NGUA)
                {
                    UpdateInwardTiemNguaMedDept();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.HOA_CHAT)
                {
                    UpdateInwardChemicalMedDept();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.MAU)
                {
                    UpdateInwardBloodMedDept();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.THANHTRUNG)
                {
                    UpdateInwardThanhTrungMedDept();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.VTYT_TIEUHAO)
                {
                    UpdateInwardVTYTTHMedDept();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM)
                {
                    UpdateInwardVPPMedDept();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.VATTU_TIEUHAO)
                {
                    UpdateInwardVTTHMedDept();
                }
                else
                {
                    UpdateInwardDrug();
                }
            }
        }

        public bool CheckValid()
        {
            if (CurrentInwardDrugMedDeptCopy == null)
            {
                return false;
            }
            return CurrentInwardDrugMedDeptCopy.Validate();
        }

        public void CancelButton(object sender, RoutedEventArgs e)
        {
            TryClose();
        }

        private int icount = 0;

        private Double _PackageQuantity;
        public Double PackageQuantity
        {
            get { return _PackageQuantity; }
            set
            {
                _PackageQuantity = value;
                NotifyOfPropertyChange(() => PackageQuantity);
                if (icount == 0)
                {
                    SLDongGoi_TextChanged();
                }
                icount--;
            }
        }

        private decimal _InQuantity;
        public decimal InQuantity
        {
            get { return _InQuantity; }
            set
            {
                _InQuantity = value;
                NotifyOfPropertyChange(() => InQuantity);
                if (icount == 0)
                {
                    SLLe_TextChanged();
                }
                icount--;

            }
        }

        private Decimal _PackagePrice;
        public Decimal PackagePrice
        {
            get { return _PackagePrice; }
            set
            {
                _PackagePrice = value;
                NotifyOfPropertyChange(() => PackagePrice);
                if (icount == 0)
                {
                    DGDongGoi_TextChanged();
                }
                icount--;
            }
        }

        private Decimal _InBuyingPrice;
        public Decimal InBuyingPrice
        {
            get { return _InBuyingPrice; }
            set
            {

                _InBuyingPrice = value;
                NotifyOfPropertyChange(() => InBuyingPrice);
                if (icount == 0)
                {
                    DGLe_TextChanged();
                }
                icount--;

            }
        }

        private Decimal _TotalPriceNotVAT;
        public Decimal TotalPriceNotVAT
        {
            get { return _TotalPriceNotVAT; }
            set
            {
                _TotalPriceNotVAT = value;
                NotifyOfPropertyChange(() => TotalPriceNotVAT);
                if (icount == 0)
                {
                    ThanhTien_TextChanged();
                }
                icount--;

            }
        }

        private void SLDongGoi_TextChanged()
        {
            icount = 4;
            CurrentInwardDrugMedDeptCopy.PackageQuantity = PackageQuantity;
            if (CurrentInwardDrugMedDeptCopy != null && CurrentInwardDrugMedDeptCopy.RefGenMedProductDetails != null)
            {
                CurrentInwardDrugMedDeptCopy.InQuantity =(decimal)CurrentInwardDrugMedDeptCopy.PackageQuantity * CurrentInwardDrugMedDeptCopy.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1);
                InQuantity = CurrentInwardDrugMedDeptCopy.InQuantity;
                if (CurrentInwardDrugMedDeptCopy.PackageQuantity > 0)
                {
                    //if (CurrentInwardDrugMedDeptCopy.PackagePrice == 0)
                    //{
                    //    CurrentInwardDrugMedDeptCopy.PackagePrice = CurrentInwardDrugMedDeptCopy.TotalPriceNotVAT / (decimal)CurrentInwardDrugMedDeptCopy.PackageQuantity;
                    //}
                    //if (CurrentInwardDrugMedDeptCopy.InBuyingPrice == 0)
                    //{
                    //    CurrentInwardDrugMedDeptCopy.InBuyingPrice = CurrentInwardDrugMedDeptCopy.PackagePrice / CurrentInwardDrugMedDeptCopy.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1);
                    //}
                    //PackagePrice = CurrentInwardDrugMedDeptCopy.PackagePrice;
                    //InBuyingPrice = CurrentInwardDrugMedDeptCopy.InBuyingPrice;
                    if (InBuyingPrice > 0)
                    {
                        CurrentInwardDrugMedDeptCopy.TotalPriceNotVAT = InBuyingPrice * (decimal)InQuantity;
                        TotalPriceNotVAT = CurrentInwardDrugMedDeptCopy.TotalPriceNotVAT;
                    }
                    else if (TotalPriceNotVAT > 0)
                    {
                        CurrentInwardDrugMedDeptCopy.InBuyingPrice = TotalPriceNotVAT / (decimal)InQuantity;
                        InBuyingPrice = CurrentInwardDrugMedDeptCopy.InBuyingPrice;
                        CurrentInwardDrugMedDeptCopy.PackagePrice = TotalPriceNotVAT / (decimal)PackageQuantity;
                        PackagePrice = CurrentInwardDrugMedDeptCopy.PackagePrice;
                    }
                }
            }
        }

        private void SLLe_TextChanged()
        {
            icount = 4;
            CurrentInwardDrugMedDeptCopy.InQuantity = InQuantity;
            if (CurrentInwardDrugMedDeptCopy != null && CurrentInwardDrugMedDeptCopy.RefGenMedProductDetails != null)
            {
                CurrentInwardDrugMedDeptCopy.PackageQuantity = (double)CurrentInwardDrugMedDeptCopy.InQuantity / CurrentInwardDrugMedDeptCopy.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1);
                PackageQuantity = CurrentInwardDrugMedDeptCopy.PackageQuantity;
                if (CurrentInwardDrugMedDeptCopy.PackageQuantity > 0)
                {
                    //if (CurrentInwardDrugMedDeptCopy.PackagePrice == 0)
                    //{
                    //    CurrentInwardDrugMedDeptCopy.PackagePrice = CurrentInwardDrugMedDeptCopy.TotalPriceNotVAT / (decimal)CurrentInwardDrugMedDeptCopy.PackageQuantity;
                    //}
                    //if (CurrentInwardDrugMedDeptCopy.InBuyingPrice == 0)
                    //{
                    //    CurrentInwardDrugMedDeptCopy.InBuyingPrice = CurrentInwardDrugMedDeptCopy.PackagePrice / CurrentInwardDrugMedDeptCopy.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1);
                    //}
                    //PackagePrice = CurrentInwardDrugMedDeptCopy.PackagePrice;
                    //InBuyingPrice = CurrentInwardDrugMedDeptCopy.InBuyingPrice;
                    if (InBuyingPrice > 0)
                    {
                        CurrentInwardDrugMedDeptCopy.TotalPriceNotVAT = InBuyingPrice * (decimal)InQuantity;
                        TotalPriceNotVAT = CurrentInwardDrugMedDeptCopy.TotalPriceNotVAT;
                    }
                    else if (TotalPriceNotVAT > 0)
                    {
                        CurrentInwardDrugMedDeptCopy.InBuyingPrice = TotalPriceNotVAT / (decimal)InQuantity;
                        InBuyingPrice = CurrentInwardDrugMedDeptCopy.InBuyingPrice;
                        CurrentInwardDrugMedDeptCopy.PackagePrice = TotalPriceNotVAT / (decimal)PackageQuantity;
                        PackagePrice = CurrentInwardDrugMedDeptCopy.PackagePrice;
                    }
                }
            }
        }

        private void DGDongGoi_TextChanged()
        {
            icount = 3;
            CurrentInwardDrugMedDeptCopy.PackagePrice = PackagePrice;
            if (CurrentInwardDrugMedDeptCopy != null && CurrentInwardDrugMedDeptCopy.RefGenMedProductDetails != null)
            {
                if (CurrentInwardDrugMedDeptCopy.TotalPriceNotVAT == 0)
                {
                    CurrentInwardDrugMedDeptCopy.TotalPriceNotVAT = (decimal)CurrentInwardDrugMedDeptCopy.PackageQuantity * CurrentInwardDrugMedDeptCopy.PackagePrice;
                }
                CurrentInwardDrugMedDeptCopy.InBuyingPrice = CurrentInwardDrugMedDeptCopy.PackagePrice / CurrentInwardDrugMedDeptCopy.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1);
                TotalPriceNotVAT = CurrentInwardDrugMedDeptCopy.TotalPriceNotVAT;
                InBuyingPrice = CurrentInwardDrugMedDeptCopy.InBuyingPrice;
                if (CurrentInwardDrugMedDeptCopy.InBuyingPrice != CurrentInwardDrugMedDeptCopy.RefGenMedProductDetails.InBuyingPrice && CurrentInwardDrugMedDeptCopy.RefGenMedProductDetails.InBuyingPrice > 0)
                {
                    Globals.ShowMessage(eHCMSResources.Z0574_G1_GiaMuaKhacGiaDotTruoc, eHCMSResources.G0442_G1_TBao);
                }
            }
        }

        private void DGLe_TextChanged()
        {
            icount = 3;
            CurrentInwardDrugMedDeptCopy.InBuyingPrice = InBuyingPrice;
            if (CurrentInwardDrugMedDeptCopy != null && CurrentInwardDrugMedDeptCopy.RefGenMedProductDetails != null)
            {
                if (CurrentInwardDrugMedDeptCopy.TotalPriceNotVAT == 0)
                {
                    CurrentInwardDrugMedDeptCopy.TotalPriceNotVAT = (decimal)CurrentInwardDrugMedDeptCopy.InQuantity * CurrentInwardDrugMedDeptCopy.InBuyingPrice;
                }
                CurrentInwardDrugMedDeptCopy.PackagePrice = CurrentInwardDrugMedDeptCopy.InBuyingPrice * CurrentInwardDrugMedDeptCopy.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1);
                TotalPriceNotVAT = CurrentInwardDrugMedDeptCopy.TotalPriceNotVAT;
                PackagePrice = CurrentInwardDrugMedDeptCopy.PackagePrice;

                if (CurrentInwardDrugMedDeptCopy.InBuyingPrice != CurrentInwardDrugMedDeptCopy.RefGenMedProductDetails.InBuyingPrice && CurrentInwardDrugMedDeptCopy.RefGenMedProductDetails.InBuyingPrice > 0)
                {
                    Globals.ShowMessage(eHCMSResources.Z0574_G1_GiaMuaKhacGiaDotTruoc, eHCMSResources.G0442_G1_TBao);
                }
            }
        }

        private void ThanhTien_TextChanged()
        {
            icount = 3;
            CurrentInwardDrugMedDeptCopy.TotalPriceNotVAT = TotalPriceNotVAT;
            if (CurrentInwardDrugMedDeptCopy != null && CurrentInwardDrugMedDeptCopy.RefGenMedProductDetails != null)
            {
                if (CurrentInwardDrugMedDeptCopy.PackageQuantity > 0)
                {
                    if (CurrentInwardDrugMedDeptCopy.PackagePrice == 0)
                    {
                        CurrentInwardDrugMedDeptCopy.PackagePrice = CurrentInwardDrugMedDeptCopy.TotalPriceNotVAT / (decimal)CurrentInwardDrugMedDeptCopy.PackageQuantity;
                    }
                    if (CurrentInwardDrugMedDeptCopy.InBuyingPrice == 0)
                    {
                        CurrentInwardDrugMedDeptCopy.InBuyingPrice = CurrentInwardDrugMedDeptCopy.PackagePrice / CurrentInwardDrugMedDeptCopy.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1);
                    }
                    PackagePrice = CurrentInwardDrugMedDeptCopy.PackagePrice;
                    InBuyingPrice = CurrentInwardDrugMedDeptCopy.InBuyingPrice;
                }
            }
        }

        public void SetValueForProperty()
        {
            icount = 5;
            PackageQuantity = CurrentInwardDrugMedDeptCopy.PackageQuantity;
            InQuantity = CurrentInwardDrugMedDeptCopy.InQuantity;
            PackagePrice = CurrentInwardDrugMedDeptCopy.PackagePrice;
            InBuyingPrice = CurrentInwardDrugMedDeptCopy.InBuyingPrice;
            TotalPriceNotVAT = CurrentInwardDrugMedDeptCopy.TotalPriceNotVAT;
        }

        private void ResetConverterAgain(bool value)
        {
            if (_currentView != null)
            {
                _currentView.ResetConverter(value);
            }
        }
        private long _V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
        public long V_MedProductType
        {
            get
            {
                return _V_MedProductType;
            }
            set
            {
                if (_V_MedProductType != value)
                {
                    _V_MedProductType = value;
                    NotifyOfPropertyChange(() => V_MedProductType);
                }
            }
        }
        private void UpdateInwardTiemNguaMedDept()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateInwardTiemNguaMedDept(CurrentInwardDrugMedDeptCopy, Globals.LoggedUserAccount.Staff.StaffID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int results = contract.EndUpdateInwardTiemNguaMedDept(asyncResult);
                            if (results == 0)
                            {
                                TryClose();
                                Globals.EventAggregator.Publish(new DrugDeptCloseEditInwardEvent { });
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void UpdateInwardChemicalMedDept()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateInwardChemicalMedDept(CurrentInwardDrugMedDeptCopy, Globals.LoggedUserAccount.Staff.StaffID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int results = contract.EndUpdateInwardChemicalMedDept(asyncResult);
                            if (results == 0)
                            {
                                TryClose();
                                Globals.EventAggregator.Publish(new DrugDeptCloseEditInwardEvent { });
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }
        private void UpdateInwardBloodMedDept()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateInwardBloodMedDept(CurrentInwardDrugMedDeptCopy, Globals.LoggedUserAccount.Staff.StaffID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int results = contract.EndUpdateInwardBloodMedDept(asyncResult);
                            if (results == 0)
                            {
                                TryClose();
                                Globals.EventAggregator.Publish(new DrugDeptCloseEditInwardEvent { });
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }
        private void UpdateInwardThanhTrungMedDept()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateInwardThanhTrungMedDept(CurrentInwardDrugMedDeptCopy, Globals.LoggedUserAccount.Staff.StaffID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int results = contract.EndUpdateInwardThanhTrungMedDept(asyncResult);
                            if (results == 0)
                            {
                                TryClose();
                                Globals.EventAggregator.Publish(new DrugDeptCloseEditInwardEvent { });
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void UpdateInwardVTYTTHMedDept()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateInwardVTYTTHMedDept(CurrentInwardDrugMedDeptCopy, Globals.LoggedUserAccount.Staff.StaffID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int results = contract.EndUpdateInwardVTYTTHMedDept(asyncResult);
                            if (results == 0)
                            {
                                TryClose();
                                Globals.EventAggregator.Publish(new DrugDeptCloseEditInwardEvent { });
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }
        private void UpdateInwardVPPMedDept()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateInwardVPPMedDept(CurrentInwardDrugMedDeptCopy, Globals.LoggedUserAccount.Staff.StaffID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int results = contract.EndUpdateInwardVPPMedDept(asyncResult);
                            if (results == 0)
                            {
                                TryClose();
                                Globals.EventAggregator.Publish(new DrugDeptCloseEditInwardEvent { });
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }
        private void UpdateInwardVTTHMedDept()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateInwardVTTHMedDept(CurrentInwardDrugMedDeptCopy, Globals.LoggedUserAccount.Staff.StaffID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int results = contract.EndUpdateInwardVTTHMedDept(asyncResult);
                            if (results == 0)
                            {
                                TryClose();
                                Globals.EventAggregator.Publish(new DrugDeptCloseEditInwardEvent { });
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private bool _ShowIsNotVAT = true;
        private bool _IsNotVAT;
        public bool IsNotVAT
        {
            get { return _IsNotVAT; }
            set
            {
                _IsNotVAT = value;
                if (_IsNotVAT)
                {
                    if (_ShowIsNotVAT)
                    {
                        if (MessageBox.Show(eHCMSResources.Z2993_G1_ChonKhongThue, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel) == MessageBoxResult.Cancel)
                        {
                            _IsNotVAT = false;
                            return;
                        }
                    }
                    CurrentInwardDrugMedDeptCopy.IsNotVat = true;
                    CurrentInwardDrugMedDeptCopy.VAT = null;
                }
                else
                {
                    CurrentInwardDrugMedDeptCopy.IsNotVat = false;
                    if (_ShowIsNotVAT)
                    {
                        CurrentInwardDrugMedDeptCopy.VAT = 0;
                    }
                }
                NotifyOfPropertyChange(() => IsNotVAT);
            }
        }
        //▼===== #002
        public void tbxPercent_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CurrentInwardDrugMedDeptCopy != null && CurrentInwardDrugMedDeptCopy.DiscountingByPercent > 1 && CurrentInwardDrugMedDeptCopy.DiscountingByPercent < 2)
            {
                CurrentInwardDrugMedDeptCopy.Discounting = (CurrentInwardDrugMedDeptCopy.DiscountingByPercent - 1) * CurrentInwardDrugMedDeptCopy.TotalPriceNotVAT;
            }
        }

        public void tbxValue_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CurrentInwardDrugMedDeptCopy != null && CurrentInwardDrugMedDeptCopy.Discounting > 0)
            {
                if (CurrentInwardDrugMedDeptCopy.TotalPriceNotVAT > 0)
                {
                    CurrentInwardDrugMedDeptCopy.DiscountingByPercent = 1 + (CurrentInwardDrugMedDeptCopy.Discounting / CurrentInwardDrugMedDeptCopy.TotalPriceNotVAT);
                }
                else
                {
                    return;
                }
            }
        }
        //▲===== #002
    }
}
