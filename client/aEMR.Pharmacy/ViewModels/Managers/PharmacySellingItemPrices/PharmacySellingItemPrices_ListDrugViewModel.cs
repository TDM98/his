using System.ComponentModel.Composition;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using aEMR.Common;
using aEMR.Common.Collections;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
using Castle.Windsor;
using System.Threading;
using DataEntities;
using eHCMSLanguage;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IPharmacySellingItemPrices_ListDrug)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PharmacySellingItemPrices_ListDrugViewModel : Conductor<object>, IPharmacySellingItemPrices_ListDrug
         , IHandle<PharmacyCloseAddGenDrugEvent>, IHandle<PharmacyCloseEditGenDrugEvent>
         , IHandle<ReLoadDataAfterU>
    {
        public string TitleForm { get; set; }

        private long V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
        private bool _IsPopUp = false;
        public bool IsPopUp
        {
            get
            {
                return _IsPopUp;
            }
            set
            {
                if (_IsPopUp != value)
                {
                    _IsPopUp = value;
                    NotifyOfPropertyChange(() => IsPopUp);
                }
            }
        }
        private bool _IsLoadingSearch = false;
        public bool IsLoadingSearch
        {
            get { return _IsLoadingSearch; }
            set
            {
                if (_IsLoadingSearch != value)
                {
                    _IsLoadingSearch = value;
                    NotifyOfPropertyChange(() => IsLoadingSearch);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _IsLoadingDrugClass = false;
        public bool IsLoadingDrugClass
        {
            get { return _IsLoadingDrugClass; }
            set
            {
                if (_IsLoadingDrugClass != value)
                {
                    _IsLoadingDrugClass = value;
                    NotifyOfPropertyChange(() => IsLoadingDrugClass);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        public bool IsLoading
        {
            get { return _IsLoadingSearch || _IsLoadingDrugClass; }
        }
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PharmacySellingItemPrices_ListDrugViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            authorization();
            DrugsResearch = new PagedSortableCollectionView<RefGenericDrugDetail>();
            DrugsResearch.OnRefresh += DrugsResearch_OnRefresh;
            DrugsResearch.PageSize = Globals.PageSize;

            GetFamilytherapies(V_MedProductType);
            eventAggregator.Subscribe(this);

            DrugsResearch.PageIndex = 0;
            SearchDrugs(DrugsResearch.PageIndex, DrugsResearch.PageSize);
        }


        void DrugsResearch_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchDrugs(DrugsResearch.PageIndex, DrugsResearch.PageSize);
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bTim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyGiaBanThuoc,
                                               (int)oPharmacyEx.mQuanLyGiaBanThuoc_Tim, (int)ePermission.mView);
            bThem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyGiaBanThuoc,
                                               (int)oPharmacyEx.mQuanLyGiaBanThuoc_Them, (int)ePermission.mView);
            bChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyGiaBanThuoc,
                                               (int)oPharmacyEx.mQuanLyGiaBanThuoc_ChinhSua, (int)ePermission.mView);
            bThemGia = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyGiaBanThuoc,
                                               (int)oPharmacyEx.mQuanLyGiaBanThuoc_ThemGia, (int)ePermission.mView);
            bChinhSuaGia = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyGiaBanThuoc,
                                               (int)oPharmacyEx.mQuanLyGiaBanThuoc_ChinhSuaGia, (int)ePermission.mView);
            bXemGia = bThemGia || bChinhSuaGia;

        }
        #region checking account

        private bool _bTim = true;
        private bool _bThem = true;
        private bool _bChinhSua = true;
        private bool _bXemGia = true;
        private bool _bChinhSuaGia = true;
        private bool _bThemGia = true;

        public bool bXemGia
        {
            get
            {
                return _bXemGia;
            }
            set
            {
                if (_bXemGia == value)
                    return;
                _bXemGia = value;
            }
        }
        public bool bThem
        {
            get
            {
                return _bThem;
            }
            set
            {
                if (_bThem == value)
                    return;
                _bThem = value;
            }
        }
        public bool bChinhSua
        {
            get
            {
                return _bChinhSua;
            }
            set
            {
                if (_bChinhSua == value)
                    return;
                _bChinhSua = value;
            }
        }
        public bool bChinhSuaGia
        {
            get
            {
                return _bChinhSuaGia;
            }
            set
            {
                if (_bChinhSuaGia == value)
                    return;
                _bChinhSuaGia = value;
            }
        }
        public bool bThemGia
        {
            get
            {
                return _bThemGia;
            }
            set
            {
                if (_bThemGia == value)
                    return;
                _bThemGia = value;
            }
        }
        public bool bTim
        {
            get
            {
                return _bTim;
            }
            set
            {
                if (_bTim == value)
                    return;
                _bTim = value;
            }
        }

        #endregion
        #region binding visibilty

        public Button lnkDelete { get; set; }
        public Button lnkDrugID { get; set; }
        public Button lnkEdit { get; set; }
        public Button hplListPrice { get; set; }

        public void lnkDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;
            lnkDelete.Visibility = Globals.convertVisibility(bChinhSua);
        }
        public void lnkDrugID_Loaded(object sender)
        {
            lnkDrugID = sender as Button;
            lnkDrugID.Visibility = Globals.convertVisibility(bTim);
        }
        public void lnkEdit_Loaded(object sender)
        {
            lnkEdit = sender as Button;
            lnkEdit.Visibility = Globals.convertVisibility(bChinhSua);
        }
        public void hplListPrice_Loaded(object sender)
        {
            hplListPrice = sender as Button;
            hplListPrice.Visibility = Globals.convertVisibility(bXemGia);
        }

        #endregion
        #region Properties Member
        private const string ALLITEMS = "[ALL]";

        private string _BrandName;
        public string BrandName
        {
            get { return _BrandName; }
            set
            {
                _BrandName = value;
                NotifyOfPropertyChange(() => BrandName);
            }
        }

        public enum Insurance
        {
            All = 1,
            Yes = 2,
            No = 3
        }
        public enum Consult
        {
            All = 1,
            Yes = 2,
            No = 3
        }

        private byte IsInsurance = (byte)Insurance.All;
        private byte IsConsult = (byte)Consult.All;

        private DrugSearchCriteria _searchCriteria;
        public DrugSearchCriteria SearchCriteria
        {
            get
            {
                return _searchCriteria;
            }
            set
            {
                _searchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }

        private PagedSortableCollectionView<RefGenericDrugDetail> _drugsresearch;
        public PagedSortableCollectionView<RefGenericDrugDetail> DrugsResearch
        {
            get
            {
                return _drugsresearch;
            }
            set
            {
                if (_drugsresearch != value)
                {
                    _drugsresearch = value;
                    NotifyOfPropertyChange(() => DrugsResearch);
                }
            }
        }

        private RefGenericDrugDetail _CurrentDrug;
        public RefGenericDrugDetail CurrentDrug
        {
            get
            {
                return _CurrentDrug;
            }
            set
            {
                if (_CurrentDrug != value)
                {
                    _CurrentDrug = value;
                    NotifyOfPropertyChange(() => CurrentDrug);
                }
            }
        }


        private ObservableCollection<DrugClass> _familytherapies;
        public ObservableCollection<DrugClass> FamilyTherapies
        {
            get
            {
                return _familytherapies;
            }
            set
            {
                if (_familytherapies != value)
                {
                    _familytherapies = value;
                    NotifyOfPropertyChange(() => FamilyTherapies);
                }
            }
        }

        #endregion
        private void SearchDrugs(int PageIndex, int PageSize)
        {
            GetCondition();
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            int totalCount = 0;
            IsLoadingSearch = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSearchRefDrugGenericDetails_ItemPrice(SearchCriteria, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndSearchRefDrugGenericDetails_ItemPrice(out totalCount, asyncResult);
                            if (results != null)
                            {
                                DrugsResearch.Clear();
                                DrugsResearch.TotalItemCount = totalCount;
                                foreach (RefGenericDrugDetail p in results)
                                {
                                    DrugsResearch.Add(p);
                                }
                                NotifyOfPropertyChange(() => DrugsResearch);
                            }
                        }
                        catch (Exception ex)
                        {
                            IsLoadingSearch = false;
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoadingSearch = false;
                            // Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void GetCondition()
        {
            if (SearchCriteria == null)
            {
                SearchCriteria = new DrugSearchCriteria();
            }
            SearchCriteria.IsConsult = IsConsult;
            SearchCriteria.IsInsurance = IsInsurance;
        }

        private void GetFamilytherapies(long V_MedProductType)
        {
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoadingDrugClass = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetFamilyTherapies(V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetFamilyTherapies(asyncResult);
                            if (results != null)
                            {
                                if (FamilyTherapies == null)
                                {
                                    FamilyTherapies = new ObservableCollection<DrugClass>();
                                }
                                else
                                {
                                    FamilyTherapies.Clear();
                                }
                                DrugClass ite = new DrugClass();
                                ite.DrugClassID = 0;
                                ite.FaName = ALLITEMS;
                                FamilyTherapies.Add(ite);
                                foreach (DrugClass p in results)
                                {
                                    FamilyTherapies.Add(p);
                                }
                                NotifyOfPropertyChange(() => FamilyTherapies);
                            }
                        }
                        catch (Exception ex)
                        {
                            IsLoadingDrugClass = false;
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoadingDrugClass = false;
                            //  Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void Search(object sender, RoutedEventArgs e)
        {
            DrugsResearch.PageIndex = 0;
            SearchDrugs(0, DrugsResearch.PageSize);
        }

        public void btnAddNew(object sender, RoutedEventArgs e)
        {
            //var proAlloc = Globals.GetViewModel<IRefGenDrug_Add>();
            //proAlloc.IsAddFinishClosed = false;
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<IRefGenDrug_Add> onInitDLg = (proAlloc) =>
            {
                proAlloc.IsAddFinishClosed = false;
            };

            GlobalsNAV.ShowDialog<IRefGenDrug_Add>(onInitDLg);

        }

        public void lnkDrugID_Click(object sender, RoutedEventArgs e)
        {
            //var proAlloc = Globals.GetViewModel<IRefGenDrug_Show>();
            //proAlloc.SelectedDrug = CurrentDrug;
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<IRefGenDrug_Show> onInitDLg = (proAlloc) =>
            {
                proAlloc.SelectedDrug = CurrentDrug;
            };

            Action<IRefGenDrug_ShowNew> onInitDLgNew = (proAlloc) =>
            {
                proAlloc.SelectedDrug = CurrentDrug;
            };

            if (Globals.ServerConfigSection.CommonItems.EnableHIStore)
            {
                GlobalsNAV.ShowDialog<IRefGenDrug_ShowNew>(onInitDLgNew);
            }
            else
            {
                GlobalsNAV.ShowDialog<IRefGenDrug_Show>(onInitDLg);
            }
            //GlobalsNAV.ShowDialog<IRefGenDrugList>(onInitDlg);
        }

        //public void lnkEdit_Click(object sender, RoutedEventArgs e)
        //{
        //    var proAlloc = Globals.GetViewModel<IRefGenDrug_Edit>();
        //    proAlloc.NewDrug = ObjectCopier.DeepCopy(CurrentDrug);
        //    var instance = proAlloc as Conductor<object>;
        //    Globals.ShowDialog(instance, (o) => { });
        //}

        public void txt_search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.BrandName = (sender as TextBox).Text;
                }
                DrugsResearch.PageIndex = 0;
                SearchDrugs(0, DrugsResearch.PageSize);
            }
        }

        public void cbxFamilyTherapy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DrugsResearch.PageIndex = 0;
            SearchDrugs(0, DrugsResearch.PageSize);
        }

        public void griddrug_DblClick(object sender, aEMR.Common.EventArgs<object> e)
        {
            if (IsPopUp)
            {
                TryClose();
                Globals.EventAggregator.Publish(new PharmacyCloseSearchDrugEvent() { SupplierDrug = e.Value });
            }
        }

        public void IsInsurance1_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            IsInsurance = (byte)Insurance.All;
        }

        public void IsInsurance2_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            IsInsurance = (byte)Insurance.Yes;
        }

        public void IsInsurance3_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            IsInsurance = (byte)Insurance.No;
        }

        public void IsConsult1_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            IsConsult = (byte)Consult.All;
        }

        public void IsConsult2_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            IsConsult = (byte)Consult.Yes;
        }

        public void IsConsult3_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            IsConsult = (byte)Consult.No;
        }

        #region IHandle<PharmacyCloseAddGenDrugEvent> Members

        public void Handle(PharmacyCloseAddGenDrugEvent message)
        {
            DrugsResearch.PageIndex = 0;
            SearchDrugs(0, DrugsResearch.PageSize);
        }

        #endregion

        #region IHandle<PharmacyCloseEditGenDrugEvent> Members
        public void Handle(PharmacyCloseEditGenDrugEvent message)
        {
            DrugsResearch.PageIndex = 0;
            SearchDrugs(0, DrugsResearch.PageSize);
        }
        #endregion


        #region DS Giá

        public void DoubleClick(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;
            CurrentDrug = eventArgs.Value as RefGenericDrugDetail;

            if (CurrentDrug != null)
            {
                //var typeInfo = Globals.GetViewModel<IPharmacySellingItemPrices_Item>();
                //typeInfo.ObjDrug_Current = CurrentDrug;
                //typeInfo.ObjPharmacySellingItemPrices_Current = CurrentDrug.ObjPharmacySellingItemPrices;
                //typeInfo.ObjPharmacySellingItemPrices_Current.EffectiveDate = Globals.ServerDate;
                //typeInfo.ObjPharmacySellingItemPrices_Current.StaffID = (long)Globals.LoggedUserAccount.Staff.StaffID;
                //typeInfo.ObjPharmacySellingItemPrices_Current.DrugID = CurrentDrug.DrugID;

                //typeInfo.ObjPharmacySellingItemPrices_Current.ApprovedStaffID = (long)Globals.LoggedUserAccount.Staff.StaffID;

                //var instance = typeInfo as Conductor<object>;

                //Globals.ShowDialog(instance, (o) =>
                //{
                //    //làm gì đó
                //});


                Action<IPharmacySellingItemPrices_Item> onInitDLg = (typeInfo) =>
                {
                    typeInfo.ObjDrug_Current = CurrentDrug;
                    typeInfo.ObjPharmacySellingItemPrices_Current = CurrentDrug.ObjPharmacySellingItemPrices;
                    typeInfo.ObjPharmacySellingItemPrices_Current.EffectiveDate = Globals.ServerDate;
                    typeInfo.ObjPharmacySellingItemPrices_Current.StaffID = (long)Globals.LoggedUserAccount.Staff.StaffID;
                    typeInfo.ObjPharmacySellingItemPrices_Current.DrugID = CurrentDrug.DrugID;

                    typeInfo.ObjPharmacySellingItemPrices_Current.ApprovedStaffID = (long)Globals.LoggedUserAccount.Staff.StaffID;
                };

                GlobalsNAV.ShowDialog<IPharmacySellingItemPrices_Item>(onInitDLg);
            }
        }

        public void hplListPrice_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentDrug != null)
            {
                //var typeInfo = Globals.GetViewModel<IPharmacySellingItemPrices_Item>();
                //typeInfo.ObjDrug_Current = CurrentDrug;
                //typeInfo.ObjPharmacySellingItemPrices_Current = CurrentDrug.ObjPharmacySellingItemPrices;
                //typeInfo.ObjPharmacySellingItemPrices_Current.EffectiveDate = Globals.ServerDate;
                //typeInfo.ObjPharmacySellingItemPrices_Current.StaffID = (long)Globals.LoggedUserAccount.Staff.StaffID;
                //typeInfo.ObjPharmacySellingItemPrices_Current.DrugID = CurrentDrug.DrugID;

                //typeInfo.ObjPharmacySellingItemPrices_Current.ApprovedStaffID = (long)Globals.LoggedUserAccount.Staff.StaffID;

                //var instance = typeInfo as Conductor<object>;

                //Globals.ShowDialog(instance, (o) =>
                //{
                //    //làm gì đó
                //});

                Action<IPharmacySellingItemPrices_Item> onInitDLg = (typeInfo) =>
                {
                    typeInfo.ObjDrug_Current = CurrentDrug;
                    typeInfo.ObjPharmacySellingItemPrices_Current = CurrentDrug.ObjPharmacySellingItemPrices;
                    typeInfo.ObjPharmacySellingItemPrices_Current.EffectiveDate = Globals.ServerDate;
                    typeInfo.ObjPharmacySellingItemPrices_Current.StaffID = (long)Globals.LoggedUserAccount.Staff.StaffID;
                    typeInfo.ObjPharmacySellingItemPrices_Current.DrugID = CurrentDrug.DrugID;

                    typeInfo.ObjPharmacySellingItemPrices_Current.ApprovedStaffID = (long)Globals.LoggedUserAccount.Staff.StaffID;
                };

                GlobalsNAV.ShowDialog<IPharmacySellingItemPrices_Item>(onInitDLg);
            }
        }

        public void Handle(ReLoadDataAfterU message)
        {
            if (message != null)
            {
                SearchDrugs(DrugsResearch.PageIndex, DrugsResearch.PageSize);
            }
        }
        #endregion


        RefGenericDrugDetail datacontextrow = new RefGenericDrugDetail();
        public void griddrug_RowEditEnded(object sender, DataGridRowEditEndingEventArgs e)
        {
            RefGenericDrugDetail current = CurrentDrug;
            if (current != null && datacontextrow != null && current.NormalPrice != datacontextrow.NormalPrice || current.PriceForHIPatient != datacontextrow.PriceForHIPatient || current.HIAllowedPrice != datacontextrow.HIAllowedPrice)
            {
                if (MessageBox.Show(string.Format("{0} {1}?", eHCMSResources.A0134_G1_Msg_ConfCNhatGiaBanThuoc, current.BrandName), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    PharmacySellingItemPrices CurrentSelling = new PharmacySellingItemPrices();
                    CurrentSelling.StaffID = Globals.LoggedUserAccount.StaffID.GetValueOrDefault();
                    CurrentSelling.NormalPrice = current.NormalPrice;
                    CurrentSelling.PriceForHIPatient = current.PriceForHIPatient;
                    CurrentSelling.HIAllowedPrice = current.HIAllowedPrice;
                    CurrentSelling.DrugID = current.DrugID;
                    if (CheckGiaChenhLech(CurrentSelling))
                    {
                        PharmacySellingItemPrices_Save(CurrentSelling);
                    }
                }
                else
                {
                    SetObject(current, datacontextrow);
                }
            }
        }

        private void SetObject(RefGenericDrugDetail a, RefGenericDrugDetail b)
        {
            a.NormalPrice = b.NormalPrice;
            a.HIAllowedPrice = b.HIAllowedPrice;
            a.PriceForHIPatient = b.PriceForHIPatient;
        }
        private bool CheckGiaChenhLech(PharmacySellingItemPrices current)
        {
            if (current.NormalPrice >= 1)
            {
                if (current.NormalPrice < current.PriceForHIPatient)
                {
                    MessageBox.Show(eHCMSResources.Z0573_G1_GiaBNBHNhoHonDGia);
                    SetObject(CurrentDrug, datacontextrow);
                    return false;
                }
                else
                {
                    if (current.HIAllowedPrice > 0)
                    {
                        if (current.HIAllowedPrice > current.PriceForHIPatient)
                        {
                            MessageBox.Show(eHCMSResources.T1982_G1_GiaBHChoPhep2);
                            SetObject(CurrentDrug, datacontextrow);
                            return false;
                        }
                    }
                }

            }
            else
            {
                MessageBox.Show(eHCMSResources.Z0644_G1_DGiaPhaiLonHonBang1);
                SetObject(CurrentDrug, datacontextrow);
                return false;
            }
            return true;
        }

        public void PharmacySellingItemPrices_Save(PharmacySellingItemPrices current)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0645_G1_DangGhi)});
            IsLoadingDrugClass = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPharmacySellingItemPrices_SaveRow(current, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var OK = contract.EndPharmacySellingItemPrices_SaveRow(asyncResult);
                            if (OK)
                            {
                                MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK);
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail);
                                SetObject(CurrentDrug, datacontextrow);
                            }

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoadingDrugClass = false;
                            Globals.IsBusy = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }
    }
}

