using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Controls;
using aEMR.Common.PagedCollectionView;
using aEMR.Common;
using Caliburn.Micro;
using DataEntities;
using Castle.Core.Logging;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using eHCMSLanguage;
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using System.Windows.Data;

/*
 * 20181019 TNHX #001: [BM0003195] Convert PagedCollectionView -> CollectionViewSource, refactor code
 * 20181114 #002 TTM: BM 0005259: Chuyển đổi CellEditEnded => CellEditEnding.
 */
namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IPharmacyReferenceItemPrice)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PharmacyReferenceItemPriceViewModel : ViewModelBase, IPharmacyReferenceItemPrice
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PharmacyReferenceItemPriceViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }

        private string _TitleForm;
        public string TitleForm
        {
            get { return _TitleForm; }
            set
            {
                _TitleForm = value;
                NotifyOfPropertyChange(() => TitleForm);
            }
        }

        private PharmacyReferencePriceList _CurrentRefPriceList;
        public PharmacyReferencePriceList CurrentRefPriceList
        {
            get { return _CurrentRefPriceList; }
            set
            {
                _CurrentRefPriceList = value;
                NotifyOfPropertyChange(() => CurrentRefPriceList);
            }
        }

        protected override void OnActivate()
        {
            Globals.EventAggregator.Subscribe(this);

            base.OnActivate();

            RefItemPriceCollection = new ObservableCollection<PharmacyReferenceItemPrice>();

            if (CurrentRefPriceList != null && CurrentRefPriceList.ReferencePriceListID > 0)/*Update*/
            {
                GetPharmacyRefItemPrice(CurrentRefPriceList.ReferencePriceListID);

                if (!CurrentRefPriceList.IsActive)
                {
                    MessageBox.Show(eHCMSResources.A0205_G1_Msg_InfoKhDcSuaBGiaCu, eHCMSResources.Z0665_G1_SuaBGia, MessageBoxButton.OK);
                    BtSave_IsEnabled = false;
                }
                else
                {
                    BtSave_IsEnabled = true;
                }
            }
            else/*AddNew*/
            {
                InitializeNewItem();
                PharmacyReferencePriceList_AutoCreate();
                BtSave_IsEnabled = true;
            }
        }

        private PharmacyReferenceItemPrice _SelectedReferenceItemPrice;
        public PharmacyReferenceItemPrice SelectedReferenceItemPrice
        {
            get { return _SelectedReferenceItemPrice; }
            set
            {
                _SelectedReferenceItemPrice = value;
                NotifyOfPropertyChange(() => SelectedReferenceItemPrice);
            }
        }

        private void PharmacyReferencePriceList_AutoCreate()
        {
            RefItemPriceCollection.Clear();
            this.DlgShowBusyIndicator(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0667_G1_DSGiaBan));
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPharmacyRefPriceList_AutoCreate(Globals.DispatchCallback((asyncResult) =>
                        {
                            List<PharmacyReferenceItemPrice> allItems = null;
                            try
                            {
                                allItems = client.EndPharmacyRefPriceList_AutoCreate(asyncResult);

                                BtSave_IsEnabled = true;
                                DtgListIsEnabled = true;

                                RefItemPriceCollection = allItems.ToObservableCollection();
                                LoadDataGrid();
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                _logger.Info(ex.Message);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }

        private void GetPharmacyRefItemPrice(Int64 ReferencePriceListID)
        {
            RefItemPriceCollection.Clear();
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0667_G1_DSGiaBan) });
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetPharmacyRefItemPrice(ReferencePriceListID, Globals.DispatchCallback((asyncResult) =>
                        {
                            List<PharmacyReferenceItemPrice> allItems = null;
                            try
                            {
                                allItems = client.EndGetPharmacyRefItemPrice(asyncResult);

                                if (allItems != null)
                                {
                                    DtgListIsEnabled = true;

                                    RefItemPriceCollection = allItems.ToObservableCollection();
                                    LoadDataGrid();
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
            });
            t.Start();
        }

        public void InitializeNewItem()
        {
            CurrentRefPriceList = new PharmacyReferencePriceList
            {
                ReferencePriceListID = 0,
                RecCreatedDate = Globals.GetCurServerDateTime(),
                CreatedStaff = Globals.LoggedUserAccount.Staff
            };
        }

        public void DtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }
        //▼====== #002
        //public void DtgList_CellEditEnded(object sender, DataGridCellEditEndingEventArgs e)
        //{
        //    if (CurrentRefPriceList.ReferencePriceListID > 0)//Update 
        //    {
        //        if (SelectedReferenceItemPrice.ContractPriceAfterVAT != SelectedReferenceItemPrice.ContractPriceAfterVAT_Old
        //            || SelectedReferenceItemPrice.HIAllowedPrice != SelectedReferenceItemPrice.HIAllowedPrice_Old)
        //        {
        //            SelectedReferenceItemPrice.RecordState = CommonRecordState.UPDATED;
        //        }
        //    }
        //}
        public void DtgList_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (CurrentRefPriceList.ReferencePriceListID > 0)//Update 
            {
                if (SelectedReferenceItemPrice.ContractPriceAfterVAT != SelectedReferenceItemPrice.ContractPriceAfterVAT_Old
                    || SelectedReferenceItemPrice.HIAllowedPrice != SelectedReferenceItemPrice.HIAllowedPrice_Old)
                {
                    SelectedReferenceItemPrice.RecordState = CommonRecordState.UPDATED;
                }
            }
        }
        //▲====== #002
        public void BtnSave()
        {
            if (String.IsNullOrEmpty(CurrentRefPriceList.Title))
            {
                MessageBox.Show(eHCMSResources.A0882_G1_Msg_InfoNhapTieuDeBGia);
                return;
            }

            if (CurrentRefPriceList.ReferencePriceListID == 0)
            {
                CurrentRefPriceList.PharmacyRefItemPriceCollection = RefItemPriceCollection.DeepCopy();
                if (CurrentRefPriceList.PharmacyRefItemPriceCollection.Count > 0)
                {
                    PharmacySellingPriceList_AddNew();
                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0204_G1_Msg_BGiaChuaCoMucNao, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                }
            }
            else//Update
            {
                if (CurrentRefPriceList.PharmacyRefItemPriceCollection == null)
                {
                    CurrentRefPriceList.PharmacyRefItemPriceCollection = new ObservableCollection<PharmacyReferenceItemPrice>();
                }

                CurrentRefPriceList.PharmacyRefItemPriceCollection.Clear();

                foreach (PharmacyReferenceItemPrice item in RefItemPriceCollection)
                {
                    if (item.RecordState == CommonRecordState.UPDATED)
                    {
                        CurrentRefPriceList.PharmacyRefItemPriceCollection.Add(item);
                    }
                }

                PharmacyRefPriceList_Update();
            }
        }

        public void BtnClose()
        {
            TryClose();
        }

        public void PharmacySellingPriceList_AddNew()
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0645_G1_DangGhi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPharmacyRefPriceList_AddNew(CurrentRefPriceList, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool res = contract.EndPharmacyRefPriceList_AddNew(asyncResult);
                                if (res)
                                {
                                    BtSave_IsEnabled = false;
                                    DtgListIsEnabled = false;

                                    Globals.EventAggregator.Publish(new EventSaveRefItemPriceSuccess());
                                    MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.T0780_G1_TaoBGiaMoi, MessageBoxButton.OK);
                                    TryClose();
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0997_G1_Msg_InfoTaoBGiaFail, eHCMSResources.T0780_G1_TaoBGiaMoi, MessageBoxButton.OK);
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    this.DlgHideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });
            t.Start();
        }
        //AddNew

        public void PharmacyRefPriceList_Update()
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0726_G1_DangCNhat);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPharmacyRefPriceList_Update(CurrentRefPriceList, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool result = contract.EndPharmacyRefPriceList_Update(asyncResult);
                                if (result)
                                {
                                    BtSave_IsEnabled = false;
                                    DtgListIsEnabled = false;

                                    Globals.EventAggregator.Publish(new EventSaveRefItemPriceSuccess());
                                    MessageBox.Show(eHCMSResources.K2782_G1_DaCNhat, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    TryClose();
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.Z1482_G1_CNhatBGiaKgThCong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    this.DlgHideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });
            t.Start();
        }

        private bool _dtgListIsEnabled;
        public bool DtgListIsEnabled
        {
            get { return _dtgListIsEnabled; }
            set
            {
                _dtgListIsEnabled = value;
                NotifyOfPropertyChange(() => DtgListIsEnabled);
            }
        }

        private bool _BtSave_IsEnabled;
        public bool BtSave_IsEnabled
        {
            get { return _BtSave_IsEnabled; }
            set
            {
                _BtSave_IsEnabled = value;
                NotifyOfPropertyChange(() => BtSave_IsEnabled);
            }
        }

        DataPager pagerSellingList = null;
        public void PagerSellingList_Loaded(object sender, RoutedEventArgs e)
        {
            pagerSellingList = sender as DataPager;
        }

        CheckBox PagingChecked;
        public void Paging_Checked(object sender, RoutedEventArgs e)
        {
            //activate datapager
            PagingChecked = sender as CheckBox;
            //TTM 25062018: Source của DataPager cần viết lại chuyển từ Observable => IEnumrable
            //pagerSellingList.Source = RefItemPriceCollection_Paging;
            VisibilityPaging = Visibility.Visible;
        }

        public void Paging_Unchecked(object sender, RoutedEventArgs e)
        {
            //deactivate datapager
            //pagerSellingList.Source = null;
            VisibilityPaging = Visibility.Collapsed;
            LoadDataGrid();
        }

        private string _SearchKey;
        public string SearchKey
        {
            get { return _SearchKey; }
            set
            {
                _SearchKey = value;
                NotifyOfPropertyChange(() => SearchKey);
            }
        }

        private void LoadDataGrid()
        {
            CVS_RefItemPriceCollection_Paging = null;
            CVS_RefItemPriceCollection_Paging = new CollectionViewSource { Source = RefItemPriceCollection };
            CV_RefItemPriceCollection_Paging = (CollectionView)CVS_RefItemPriceCollection_Paging.View;
            NotifyOfPropertyChange(() => CV_RefItemPriceCollection_Paging);

            BtnFilter();
        }

        private CollectionViewSource CVS_RefItemPriceCollection_Paging = null;
        public CollectionView CV_RefItemPriceCollection_Paging
        {
            get; set;
        }

        private ObservableCollection<PharmacyReferenceItemPrice> _RefItemPriceCollection;
        public ObservableCollection<PharmacyReferenceItemPrice> RefItemPriceCollection
        {
            get { return _RefItemPriceCollection; }
            set
            {
                _RefItemPriceCollection = value;
                NotifyOfPropertyChange(() => RefItemPriceCollection);
            }
        }

        public void BtnFilter()
        {
            CV_RefItemPriceCollection_Paging.Filter = null;
            CV_RefItemPriceCollection_Paging.Filter = new Predicate<object>(DoFilter);
        }

        //Callback method
        private bool DoFilter(object o)
        {
            //it is not a case sensitive search
            if (!(o is PharmacyReferenceItemPrice emp))
            {
                return false;
            }

            if (string.IsNullOrEmpty(SearchKey))
            {
                SearchKey = "";
            }

            if (SearchKey.Length == 1)
            {
                if (emp.Drug.BrandName.ToLower().StartsWith(SearchKey.Trim().ToLower()) || emp.Drug.DrugCode.ToLower() == SearchKey.Trim().ToLower())
                {
                    return true;
                }
            }
            else
            {
                if (emp.Drug.BrandName.ToLower().IndexOf(SearchKey.Trim().ToLower()) >= 0 || emp.Drug.DrugCode.ToLower() == SearchKey.Trim().ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        public void SearchKey_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchKey = (sender as TextBox).Text;
                BtnFilter();
            }
        }

        private Visibility _VisibilityPaging = Visibility.Collapsed;
        public Visibility VisibilityPaging
        {
            get
            {
                return _VisibilityPaging;
            }
            set
            {
                if (_VisibilityPaging != value)
                {
                    _VisibilityPaging = value;
                    NotifyOfPropertyChange(() => VisibilityPaging);
                }
            }
        }

        private int _PCVPageSize = 30;
        public int PCVPageSize
        {
            get
            {
                return _PCVPageSize;
            }
            set
            {
                if (_PCVPageSize != value)
                {
                    _PCVPageSize = value;
                    NotifyOfPropertyChange(() => PCVPageSize);
                }
            }
        }

        public void CbxPageSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox) != null && (sender as ComboBox).SelectedItem != null)
            {
                PCVPageSize = Convert.ToInt32(((sender as ComboBox).SelectedItem as ComboBoxItem).Content);
            }
        }
    }
}
