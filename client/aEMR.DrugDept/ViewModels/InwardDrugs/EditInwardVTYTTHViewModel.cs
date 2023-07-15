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


namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IVTYTTHEditInward)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class EditInwardVTYTTHViewModel : Conductor<object>, IVTYTTHEditInward
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public EditInwardVTYTTHViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
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

        private void UpdateInwardVTYTTH()
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

        public void OKButton(object sender, RoutedEventArgs e)
        {
            if (CheckValid())
            {
                if (CurrentInwardDrugMedDeptCopy.InQuantity <= 0)
                {
                    Globals.ShowMessage(eHCMSResources.A0789_G1_Msg_InfoSLgNhapLonHon0, eHCMSResources.G0442_G1_TBao);
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
                if (Au != null)
                {
                    CurrentInwardDrugMedDeptCopy.SdlDescription = Au.Text;
                }
                UpdateInwardVTYTTH();
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
    }
}
