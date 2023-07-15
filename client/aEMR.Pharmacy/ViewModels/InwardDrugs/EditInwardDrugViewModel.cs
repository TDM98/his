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
using System.Collections.Generic;
using aEMR.Common;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using eHCMSLanguage;
using aEMR.CommonTasks;
/*
 * 20200328 #001 TTM: BM 0029055: Fix lỗi không cập nhật được tiền chiết khấu khi tích vào chiết khấu theo %, và đưa % chiết khấu sai khi bỏ tích chiết khấu % để nhập tiền chiết khấu.
 * 20200411 #002 TTM: BM 0029095: Fix lỗi cập nhật giá bán DV, giá BH, giá cho BNBH giá trị < 0
 */
namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IEditInwardDrug)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class EditInwardDrugViewModel : Conductor<object>, IEditInwardDrug
    {
        #region Indicator Member

        private bool _isLoadingGoodType = false;
        public bool isLoadingGoodType
        {
            get { return _isLoadingGoodType; }
            set
            {
                if (_isLoadingGoodType != value)
                {
                    _isLoadingGoodType = value;
                    NotifyOfPropertyChange(() => isLoadingGoodType);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingFullOperater = false;
        public bool isLoadingFullOperater
        {
            get { return _isLoadingFullOperater; }
            set
            {
                if (_isLoadingFullOperater != value)
                {
                    _isLoadingFullOperater = value;
                    NotifyOfPropertyChange(() => isLoadingFullOperater);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        public bool IsLoading
        {
            get { return (isLoadingGoodType || isLoadingFullOperater); }
        }

        #endregion
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public EditInwardDrugViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            CurrentInwardDrugCopy = new InwardDrug();
            Coroutine.BeginExecute(DoGetLookup_GoodType());
        }

        #region Properties Member

        //Kiên: Biến _OrigInwardDrugCopy chứa dữ liệu gốc (chưa cập nhật) của InwardDrug, dùng để so sánh với _CurrentInwardDrugCopy.
        private InwardDrug _OrigInwardDrugCopy;

        //Biến _CurrentInwardDrugCopy chứa dữ liệu muốn cập nhật.
        private InwardDrug _CurrentInwardDrugCopy;
        public InwardDrug CurrentInwardDrugCopy
        {
            get
            {
                return _CurrentInwardDrugCopy;
            }
            set
            {
                if (_CurrentInwardDrugCopy != value)
                {
                    _CurrentInwardDrugCopy = value;
                    _OrigInwardDrugCopy = _CurrentInwardDrugCopy.DeepCopy();
                    if (_CurrentInwardDrugCopy != null)
                    {
                        _ShowIsNotVAT = false;
                        IsNotVAT = _CurrentInwardDrugCopy.IsNotVat;
                        _ShowIsNotVAT = true;
                    }
                    NotifyOfPropertyChange(() => CurrentInwardDrugCopy);
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

        private IEnumerator<IResult> DoGetLookup_GoodType()
        {
            isLoadingGoodType = true;
            var paymentTypeTask = new LoadLookupListTask(LookupValues.V_GoodsType, false, false);
            yield return paymentTypeTask;
            CbxGoodsTypes = paymentTypeTask.LookupList;
            isLoadingGoodType = false;
            yield break;
        }

        private void UpdateInwardDrug()
        {
            isLoadingFullOperater = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                //using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                using (var serviceFactory = new PharmacySaleAndOutwardClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    //contract.BeginInwardDrug_Update(CurrentInwardDrugCopy, Globals.LoggedUserAccount.Staff.StaffID, Globals.DispatchCallback((asyncResult) =>
                    contract.BeginInwardDrug_Update_Pst(CurrentInwardDrugCopy, Globals.LoggedUserAccount.Staff.StaffID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            //int results = contract.EndInwardDrug_Update(asyncResult);
                            int results = contract.EndInwardDrug_Update_Pst(asyncResult);
                            if (results == 0)
                            {
                                TryClose();
                                //phat su kien o day
                                Globals.EventAggregator.Publish(new PharmacyCloseEditInwardEvent { });
                            }
                            //Kiên comment vì không còn sử dụng nữa.
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
                            //    MessageBox.Show("Không tồn tại dòng dữ liệu này.", eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                            //}
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        }
                        finally
                        {
                            isLoadingFullOperater = false;
                            //Globals.IsBusy = false;
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
                if (CurrentInwardDrugCopy.InQuantity <= 0)
                {
                    MessageBox.Show(eHCMSResources.Z0572_G1_SLgNhapPhaiLonHon0, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }
                //Kiên thêm kiểm tra số lượng muốn cập nhật và số lượng đã bán ra.
                if (!CompareInQtyAndOutQty())
                {
                    MessageBox.Show(eHCMSResources.K0049_G1_ThuocXuatLonHonSLggChSua);
                    return;
                }
                if (CurrentInwardDrugCopy.InBuyingPrice == 0 && CurrentInwardDrugCopy.V_GoodsType == (long)AllLookupValues.V_GoodsType.HANGMUA)
                {
                    MessageBox.Show(eHCMSResources.A0556_G1_Msg_InfoGiaMuaLonHon0);
                    return;
                }
                if ((CurrentInwardDrugCopy.DiscountingByPercent < 1 && CurrentInwardDrugCopy.DiscountingByPercent > 0) || (CurrentInwardDrugCopy.DiscountingByPercent > 2))
                {
                    MessageBox.Show(string.Format("{0}.", eHCMSResources.A0072_G1_CKKhHopLe));
                    return;
                }
                if (CurrentInwardDrugCopy.SelectedDrug.NormalPrice <= 0)
                {
                    Globals.ShowMessage(eHCMSResources.A0786_G1_Msg_InfoCNhatBGiaBan, eHCMSResources.G0442_G1_TBao);
                    return;
                }
                if (CurrentInwardDrugCopy.SelectedDrug.NormalPrice < CurrentInwardDrugCopy.SelectedDrug.PriceForHIPatient)
                {
                    MessageBox.Show(eHCMSResources.Z0573_G1_GiaBNBHNhoHonDGia);
                    return;
                }
                if (CurrentInwardDrugCopy.SelectedDrug.HIAllowedPrice > CurrentInwardDrugCopy.SelectedDrug.PriceForHIPatient)
                {
                    MessageBox.Show(eHCMSResources.T1982_G1_GiaBHChoPhep2);
                    return;
                }
                if (CurrentInwardDrugCopy.VAT < 0 || CurrentInwardDrugCopy.VAT > 1)
                {
                    MessageBox.Show(eHCMSResources.Z2991_G1_VATKhongHopLe);
                    return;
                }
                //▼===== #002
                if (CurrentInwardDrugCopy.SelectedDrug.NormalPrice < 0)
                {
                    MessageBox.Show(eHCMSResources.Z3013_G1_GiaBanDVKhongDuocNhoHon0);
                    return;
                }
                if (CurrentInwardDrugCopy.SelectedDrug.HIAllowedPrice < 0)
                {
                    MessageBox.Show(eHCMSResources.Z3012_G1_GiaBHKhongDuocNhoHon0);
                    return;
                }
                if (CurrentInwardDrugCopy.SelectedDrug.PriceForHIPatient < 0)
                {
                    MessageBox.Show(eHCMSResources.Z3011_G1_GiaBNBHKhongDuocNhoHon0);
                    return;
                }
                if (CurrentInwardDrugCopy.TotalPriceNotVAT < 0)
                {
                    MessageBox.Show(eHCMSResources.Z3014_G1_ThanhTienKhongDuocNhoHon0);
                    return;
                }
                //▲===== #002
                if (Au != null)
                {
                    CurrentInwardDrugCopy.SdlDescription = Au.Text;
                }
                UpdateInwardDrug();
            }
        }

        //Kiên: Kiểm tra số lượng muốn cập nhật và số lượng ĐÃ xuất. Nếu số lượng muốn cập nhật < số lượng ĐÃ xuất là sai.
        public bool CompareInQtyAndOutQty()
        {
            double OutQuantity = _OrigInwardDrugCopy.InQuantity - _OrigInwardDrugCopy.Remaining;
            if (CurrentInwardDrugCopy.InQuantity < OutQuantity)
            {
                return false;
            }
            return true;
        }

        public bool CheckValid()
        {
            if (CurrentInwardDrugCopy == null)
            {
                return false;
            }
            return CurrentInwardDrugCopy.Validate();
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

        private double _InQuantity;
        public double InQuantity
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
            CurrentInwardDrugCopy.PackageQuantity = PackageQuantity;
            if (CurrentInwardDrugCopy != null && CurrentInwardDrugCopy.SelectedDrug != null)
            {
                CurrentInwardDrugCopy.InQuantity = CurrentInwardDrugCopy.PackageQuantity * CurrentInwardDrugCopy.SelectedDrug.UnitPackaging.GetValueOrDefault(1);
                InQuantity = CurrentInwardDrugCopy.InQuantity;
                if (CurrentInwardDrugCopy.PackageQuantity > 0)
                {
                    //Kiên: KHÔNG ĐƯỢC SỬ DỤNG "CurrentInwardDrugCopy.TotalPriceNotVAT", VÌ SAU KHI THAY ĐỔI "CurrentInwardDrugCopy.InQuantity"
                    //THÌ "CurrentInwardDrugCopy.TotalPriceNotVAT" THAY ĐỔI THEO, DẪN ĐẾN VIỆC TÍNH TOÁN SAI.
                    //CurrentInwardDrugCopy.PackagePrice = CurrentInwardDrugCopy.TotalPriceNotVAT / (decimal)CurrentInwardDrugCopy.PackageQuantity;
                    //if (CurrentInwardDrugCopy.PackagePrice == 0)
                    //{
                    //    CurrentInwardDrugCopy.PackagePrice = TotalPriceNotVAT / (decimal)CurrentInwardDrugCopy.PackageQuantity;
                    //}
                    //if (CurrentInwardDrugCopy.InBuyingPrice == 0)
                    //{
                    //    CurrentInwardDrugCopy.InBuyingPrice = CurrentInwardDrugCopy.PackagePrice / CurrentInwardDrugCopy.SelectedDrug.UnitPackaging.GetValueOrDefault(1);
                    //}
                    //PackagePrice = CurrentInwardDrugCopy.PackagePrice;
                    //InBuyingPrice = CurrentInwardDrugCopy.InBuyingPrice;
                    if (InBuyingPrice > 0)
                    {
                        CurrentInwardDrugCopy.TotalPriceNotVAT = InBuyingPrice * (decimal)InQuantity;
                        TotalPriceNotVAT = CurrentInwardDrugCopy.TotalPriceNotVAT;
                    }
                    else if (TotalPriceNotVAT > 0)
                    {
                        CurrentInwardDrugCopy.InBuyingPrice = TotalPriceNotVAT / (decimal)InQuantity;
                        InBuyingPrice = CurrentInwardDrugCopy.InBuyingPrice;
                        CurrentInwardDrugCopy.PackagePrice = TotalPriceNotVAT / (decimal)PackageQuantity;
                        PackagePrice = CurrentInwardDrugCopy.PackagePrice;
                    }
                }
            }
            //Kiên: Mục tiêu khi ra khỏi hàm thì icount PHẢI = 1.
            //Nếu như không vào được "if (CurrentInwardDrugCopy.PackageQuantity > 0)" ở trên thì icount không thể bằng 1.
            icount = 1;
        }

        private void SLLe_TextChanged()
        {
            icount = 4;
            CurrentInwardDrugCopy.InQuantity = InQuantity;
            if (CurrentInwardDrugCopy != null && CurrentInwardDrugCopy.SelectedDrug != null)
            {
                CurrentInwardDrugCopy.PackageQuantity = CurrentInwardDrugCopy.InQuantity / CurrentInwardDrugCopy.SelectedDrug.UnitPackaging.GetValueOrDefault(1);
                PackageQuantity = CurrentInwardDrugCopy.PackageQuantity;
                if (CurrentInwardDrugCopy.PackageQuantity > 0)
                {
                    //Kiên: KHÔNG ĐƯỢC SỬ DỤNG "CurrentInwardDrugCopy.TotalPriceNotVAT", VÌ SAU KHI THAY ĐỔI "CurrentInwardDrugCopy.InQuantity"
                    //THÌ "CurrentInwardDrugCopy.TotalPriceNotVAT" THAY ĐỔI THEO, DẪN ĐẾN VIỆC TÍNH TOÁN SAI.
                    //CurrentInwardDrugCopy.PackagePrice = CurrentInwardDrugCopy.TotalPriceNotVAT / (decimal)CurrentInwardDrugCopy.PackageQuantity;
                    //if (CurrentInwardDrugCopy.PackagePrice == 0)
                    //{
                    //    CurrentInwardDrugCopy.PackagePrice = TotalPriceNotVAT / (decimal)CurrentInwardDrugCopy.PackageQuantity;
                    //}
                    //if (CurrentInwardDrugCopy.InBuyingPrice == 0)
                    //{
                    //    CurrentInwardDrugCopy.InBuyingPrice = CurrentInwardDrugCopy.PackagePrice / CurrentInwardDrugCopy.SelectedDrug.UnitPackaging.GetValueOrDefault(1);
                    //}
                    //PackagePrice = CurrentInwardDrugCopy.PackagePrice;
                    //InBuyingPrice = CurrentInwardDrugCopy.InBuyingPrice;
                    if (InBuyingPrice > 0)
                    {
                        CurrentInwardDrugCopy.TotalPriceNotVAT = InBuyingPrice * (decimal)InQuantity;
                        TotalPriceNotVAT = CurrentInwardDrugCopy.TotalPriceNotVAT;
                    }
                    else if (TotalPriceNotVAT > 0)
                    {
                        CurrentInwardDrugCopy.InBuyingPrice = TotalPriceNotVAT / (decimal)InQuantity;
                        InBuyingPrice = CurrentInwardDrugCopy.InBuyingPrice;
                        CurrentInwardDrugCopy.PackagePrice = TotalPriceNotVAT / (decimal)PackageQuantity;
                        PackagePrice = CurrentInwardDrugCopy.PackagePrice;
                    }
                }
            }
            //Kiên: Mục tiêu khi ra khỏi hàm thì icount PHẢI = 1.
            //Nếu như không vào được "if (CurrentInwardDrugCopy.PackageQuantity > 0)" ở trên thì icount không thể bằng 1.
            icount = 1;
        }

        private void DGDongGoi_TextChanged()
        {
            icount = 3;
            CurrentInwardDrugCopy.PackagePrice = PackagePrice;
            if (CurrentInwardDrugCopy != null && CurrentInwardDrugCopy.SelectedDrug != null)
            {
                if (CurrentInwardDrugCopy.TotalPriceNotVAT == 0)
                {
                    CurrentInwardDrugCopy.TotalPriceNotVAT = (decimal)CurrentInwardDrugCopy.PackageQuantity * CurrentInwardDrugCopy.PackagePrice;
                }
                CurrentInwardDrugCopy.InBuyingPrice = CurrentInwardDrugCopy.PackagePrice / CurrentInwardDrugCopy.SelectedDrug.UnitPackaging.GetValueOrDefault(1);
                TotalPriceNotVAT = CurrentInwardDrugCopy.TotalPriceNotVAT;
                InBuyingPrice = CurrentInwardDrugCopy.InBuyingPrice;
                if (CurrentInwardDrugCopy.InBuyingPrice != CurrentInwardDrugCopy.SelectedDrug.InBuyingPrice && CurrentInwardDrugCopy.SelectedDrug.InBuyingPrice > 0)
                {
                    Globals.ShowMessage(eHCMSResources.Z0574_G1_GiaMuaKhacGiaDotTruoc, eHCMSResources.G0442_G1_TBao);
                }
            }
        }

        private void DGLe_TextChanged()
        {
            icount = 3;
            CurrentInwardDrugCopy.InBuyingPrice = InBuyingPrice;
            if (CurrentInwardDrugCopy != null && CurrentInwardDrugCopy.SelectedDrug != null)
            {
                if (CurrentInwardDrugCopy.TotalPriceNotVAT == 0)
                {
                    CurrentInwardDrugCopy.TotalPriceNotVAT = (decimal)CurrentInwardDrugCopy.InQuantity * CurrentInwardDrugCopy.InBuyingPrice;
                }
                CurrentInwardDrugCopy.PackagePrice = CurrentInwardDrugCopy.InBuyingPrice * CurrentInwardDrugCopy.SelectedDrug.UnitPackaging.GetValueOrDefault(1);
                TotalPriceNotVAT = CurrentInwardDrugCopy.TotalPriceNotVAT;
                PackagePrice = CurrentInwardDrugCopy.PackagePrice;

                if (CurrentInwardDrugCopy.InBuyingPrice != CurrentInwardDrugCopy.SelectedDrug.InBuyingPrice && CurrentInwardDrugCopy.SelectedDrug.InBuyingPrice > 0)
                {
                    Globals.ShowMessage(eHCMSResources.Z0574_G1_GiaMuaKhacGiaDotTruoc, eHCMSResources.G0442_G1_TBao);
                }
            }
        }

        private void ThanhTien_TextChanged()
        {
            icount = 3;
            CurrentInwardDrugCopy.TotalPriceNotVAT = TotalPriceNotVAT;
            if (CurrentInwardDrugCopy != null && CurrentInwardDrugCopy.SelectedDrug != null)
            {
                if (CurrentInwardDrugCopy.PackageQuantity > 0)
                {
                    //Kiên: KHÔNG ĐƯỢC SỬ DỤNG "CurrentInwardDrugCopy.TotalPriceNotVAT", VÌ SAU KHI THAY ĐỔI "CurrentInwardDrugCopy.InQuantity"
                    //THÌ "CurrentInwardDrugCopy.TotalPriceNotVAT" THAY ĐỔI THEO, DẪN ĐẾN VIỆC TÍNH TOÁN SAI.
                    //CurrentInwardDrugCopy.PackagePrice = CurrentInwardDrugCopy.TotalPriceNotVAT / (decimal)CurrentInwardDrugCopy.PackageQuantity;
                    if (CurrentInwardDrugCopy.PackagePrice == 0)
                    {
                        CurrentInwardDrugCopy.PackagePrice = TotalPriceNotVAT / (decimal)CurrentInwardDrugCopy.PackageQuantity;
                    }
                    if (CurrentInwardDrugCopy.InBuyingPrice == 0)
                    {
                        CurrentInwardDrugCopy.InBuyingPrice = CurrentInwardDrugCopy.PackagePrice / CurrentInwardDrugCopy.SelectedDrug.UnitPackaging.GetValueOrDefault(1);
                    }
                    PackagePrice = CurrentInwardDrugCopy.PackagePrice;
                    InBuyingPrice = CurrentInwardDrugCopy.InBuyingPrice;
                }
            }
            //Kiên: Mục tiêu khi ra khỏi hàm thì icount PHẢI = 1.
            //Nếu như không vào được "if (CurrentInwardDrugCopy.PackageQuantity > 0)" ở trên thì icount không thể bằng 1.
            icount = 1;
        }

        public void SetValueForProperty()
        {
            icount = 5;
            PackageQuantity = CurrentInwardDrugCopy.PackageQuantity;
            InQuantity = CurrentInwardDrugCopy.InQuantity;
            PackagePrice = CurrentInwardDrugCopy.PackagePrice;
            InBuyingPrice = CurrentInwardDrugCopy.InBuyingPrice;
            TotalPriceNotVAT = CurrentInwardDrugCopy.TotalPriceNotVAT;
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
                    CurrentInwardDrugCopy.IsNotVat = true;
                    CurrentInwardDrugCopy.VAT = null;
                }
                else
                {
                    CurrentInwardDrugCopy.IsNotVat = false;
                    if (_ShowIsNotVAT)
                    {
                        CurrentInwardDrugCopy.VAT = 0;
                    }
                }
                NotifyOfPropertyChange(() => IsNotVAT);
            }
        }

        //▼===== #001
        public void tbxPercent_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CurrentInwardDrugCopy != null && CurrentInwardDrugCopy.DiscountingByPercent > 1 && CurrentInwardDrugCopy.DiscountingByPercent < 2)
            {
                CurrentInwardDrugCopy.Discounting = (CurrentInwardDrugCopy.DiscountingByPercent - 1) * CurrentInwardDrugCopy.TotalPriceNotVAT;
            }
        }

        public void tbxValue_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CurrentInwardDrugCopy != null && CurrentInwardDrugCopy.Discounting > 0)
            {
                if (CurrentInwardDrugCopy.TotalPriceNotVAT > 0)
                {
                    CurrentInwardDrugCopy.DiscountingByPercent = 1 + (CurrentInwardDrugCopy.Discounting / CurrentInwardDrugCopy.TotalPriceNotVAT);
                }
                else
                {
                    return;
                }
            }
        }
        //▲===== #001
    }
}
