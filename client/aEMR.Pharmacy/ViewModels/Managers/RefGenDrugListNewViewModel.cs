using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Common;
using aEMR.CommonTasks;
using System.Collections.Generic;
using aEMR.Common.ExportExcel;
using eHCMSLanguage;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Common.BaseModel;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IRefGenDrugListNew)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RefGenDrugListNewViewModel : ViewModelBase, IRefGenDrugListNew
         , IHandle<PharmacyCloseAddGenDrugEvent>, IHandle<PharmacyCloseEditGenDrugEvent>
    {
        public string TitleForm { get; set; }

        private long V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
        private bool _IsPopUp = false;
        private long? CityProvinceID = 42;//tam thoi de TP HCM la vay di

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

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public RefGenDrugListNewViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Authorization();
            DrugsResearch = new PagedSortableCollectionView<RefGenericDrugDetail>();
            DrugsResearch.OnRefresh += DrugsResearch_OnRefresh;
            DrugsResearch.PageSize = Globals.PageSize;

            DrugsResearch.PageIndex = 0;
            //TTM 23072018 TBR
            //De DlgBusyIndicator vao co dong nay thi dung im man hinh, bo ra thi ko sao, can hoi anh Tuan
            SearchDrugs(0, DrugsResearch.PageSize);

            Coroutine.BeginExecute(GetAllRefInfo());

            eventArg.Subscribe(this);
        }

        void DrugsResearch_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchDrugs(DrugsResearch.PageIndex, DrugsResearch.PageSize);
        }

        public void Authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bTim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyDanhMucThuoc,
                                               (int)oPharmacyEx.mQuanLyDanhMucThuoc_Tim, (int)ePermission.mView);
            bThem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyDanhMucThuoc,
                                               (int)oPharmacyEx.mQuanLyDanhMucThuoc_Them, (int)ePermission.mView);
            bChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyDanhMucThuoc,
                                               (int)oPharmacyEx.mQuanLyDanhMucThuoc_ChinhSua, (int)ePermission.mView);
        }

        #region properties reference member

        private ObservableCollection<Supplier> _Suppliers;
        public ObservableCollection<Supplier> Suppliers
        {
            get
            {
                return _Suppliers;
            }
            set
            {
                if (_Suppliers != value)
                {
                    _Suppliers = value;
                    NotifyOfPropertyChange(() => Suppliers);
                }
            }
        }


        //private ObservableCollection<Hospital> _Hospitals;
        //public ObservableCollection<Hospital> Hospitals
        //{
        //    get
        //    {
        //        return _Hospitals;
        //    }
        //    set
        //    {
        //        if (_Hospitals != value)
        //        {
        //            _Hospitals = value;
        //            NotifyOfPropertyChange(() => Hospitals);
        //        }
        //    }
        //}

        private ObservableCollection<RefGenDrugBHYT_Category> _RefGenDrugBHYT_Categorys;
        public ObservableCollection<RefGenDrugBHYT_Category> RefGenDrugBHYT_Categorys
        {
            get
            {
                return _RefGenDrugBHYT_Categorys;
            }
            set
            {
                if (_RefGenDrugBHYT_Categorys != value)
                {
                    _RefGenDrugBHYT_Categorys = value;
                    NotifyOfPropertyChange(() => RefGenDrugBHYT_Categorys);
                }
            }
        }

        private ObservableCollection<RefGenericDrugCategory_2> _RefGenericDrugCategory_2s;
        public ObservableCollection<RefGenericDrugCategory_2> RefGenericDrugCategory_2s
        {
            get
            {
                return _RefGenericDrugCategory_2s;
            }
            set
            {
                if (_RefGenericDrugCategory_2s != value)
                {
                    _RefGenericDrugCategory_2s = value;
                    NotifyOfPropertyChange(() => RefGenericDrugCategory_2s);
                }
            }
        }

        private ObservableCollection<RefCountry> _countries;
        public ObservableCollection<RefCountry> Countries
        {
            get
            {
                return _countries;
            }
            set
            {
                if (_countries != value)
                {
                    _countries = value;
                    NotifyOfPropertyChange(() => Countries);
                }
            }
        }

        private ObservableCollection<RefUnit> _units;
        public ObservableCollection<RefUnit> Units
        {
            get
            {
                return _units;
            }
            set
            {
                if (_units != value)
                {
                    _units = value;
                    NotifyOfPropertyChange(() => Units);
                }
            }
        }

        private ObservableCollection<PharmaceuticalCompany> _pharmaceuticalCompanies;
        public ObservableCollection<PharmaceuticalCompany> PharmaceuticalCompanies
        {
            get { return _pharmaceuticalCompanies; }
            set
            {
                if (_pharmaceuticalCompanies != value)
                    _pharmaceuticalCompanies = value;
                NotifyOfPropertyChange(() => PharmaceuticalCompanies);
            }
        }

        #endregion

        #region get reference member
        private IEnumerator<IResult> GetAllRefInfo()
        {
            if (Globals.allHospitals == null)
            {
                // TxD 2014/03/12 This Global List of Hospital for NOW only used in this View Model                
                //yield return GenericCoRoutineTask.StartTask(LoadAllHospitalInfoAction);
                //KMx: Chuyển hàm load Hospital sang HomeView để cho khoa Dược xài chung (22/07/2014 13:50)
                var vm = Globals.GetViewModel<IHome>();
                vm.LoadAllHospitalInfoAction();
            }

            if (Suppliers == null)
            {
                var loadTask1 = new LoadSupplierListTask((int)AllLookupValues.SupplierType.CUNG_CAP_THIET_BI_Y_TE, false, false);
                yield return loadTask1;
                Suppliers = loadTask1.SupplierList;
            }

            if (RefGenericDrugCategory_2s == null)
            {
                var loadTask2 = new LoadRefGenericDrugCategory_2ListTask(false, false);
                yield return loadTask2;
                RefGenericDrugCategory_2s = loadTask2.RefGenericDrugCategory_2List;
            }

            if (RefGenDrugBHYT_Categorys == null)
            {
                var loadTask3 = new LoadRefGenDrugBHYT_CategoryListTask(false, null, false, false);
                yield return loadTask3;
                RefGenDrugBHYT_Categorys = loadTask3.RefGenDrugBHYT_CategoryList;
            }

            if (Units == null)
            {
                var loadTask4 = new LoadUnitListTask(false, false);
                yield return loadTask4;
                Units = loadTask4.RefUnitList;
            }

            if (FamilyTherapies == null)
            {
                var loadTask5 = new LoadDrugClassListTask(V_MedProductType, false, true);
                yield return loadTask5;
                FamilyTherapies = loadTask5.DrugClassList;
            }

            if (Countries == null)
            {
                var loadTask6 = new LoadCountryListTask(false, false);
                yield return loadTask6;
                Countries = loadTask6.RefCountryList;
            }

            if (PharmaceuticalCompanies == null)
            {
                var loadTask7 = new LoadPharmaceuticalCompanyListTask(false, false);
                yield return loadTask7;
                PharmaceuticalCompanies = loadTask7.PharmaceuticalCompanyList;
            }
        }

        //private void LoadAllHospitalInfoAction(GenericCoRoutineTask genTask)
        //{
        //    this.ShowBusyIndicator();
        //    var t = new Thread(() =>
        //    {
        //        try
        //        {                    
        //            using (var serviceFactory = new CommonServiceClient())
        //            {
        //                bool bContinue = true;
        //                var contract = serviceFactory.ServiceInstance;
        //                long nfilterHosID = 0;
        //                contract.BeginHospital_ByCityProvinceID(nfilterHosID,
        //                    Globals.DispatchCallback((asyncResult) =>
        //                    {
        //                        IList<Hospital> allItems = new ObservableCollection<Hospital>();
        //                        try
        //                        {
        //                            allItems = contract.EndHospital_ByCityProvinceID(asyncResult);
        //                            Globals.allHospitals = new ObservableCollection<Hospital>(allItems);
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            MessageBox.Show("LoadAllHospitalInfoAction END Error: " + ex.Message);
        //                            ClientLoggerHelper.LogError(ex.Message);
        //                            bContinue = false;
        //                        }
        //                        finally
        //                        {
        //                            genTask.ActionComplete(bContinue);
        //                            this.HideBusyIndicator();
        //                        }

        //                    }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("LoadAllHospitalInfoAction BEGIN Error: " + ex.Message);
        //            ClientLoggerHelper.LogError(ex.Message);
        //            genTask.ActionComplete(false);
        //            this.HideBusyIndicator();
        //        }
        //    });
        //    t.Start();
        //}

        #endregion

        #region checking account

        private bool _bTim = true;
        private bool _bThem = true;
        private bool _bChinhSua = true;

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

        #endregion

        #region binding visibilty

        public Button lnkDelete { get; set; }
        public Button lnkDrugID { get; set; }
        public Button lnkEdit { get; set; }
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
        public enum DIsActive
        {
            All = 2,
            Yes = 0,
            No = 1
        }

        public enum Show
        {
            All = 2,
            Yes = 1,
            No = 0
        }

        public enum CatDrugType
        {
            All = 0,
            Shared = 1,
            Pharmacy = 2,
            DrugDept = 3
        }

        private byte IsCatDrugType = (byte)CatDrugType.Pharmacy;
        private byte IsInsurance = (byte)Insurance.All;
        private byte IsConsult = (byte)Consult.All;
        private new byte IsActive = (byte)DIsActive.Yes;
        private new byte IsShow = (byte)Show.Yes;

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
            this.ShowBusyIndicator(eHCMSResources.Z1125_G1_DangTimKiem);
            GetCondition();
            int totalCount = 0;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSearchRefDrugGenericDetails_Simple_New(SearchCriteria, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndSearchRefDrugGenericDetails_Simple_New(out totalCount, asyncResult);
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
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                _logger.Info(ex.Message);
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

        private string strNameExcel = "";
        private void GetCondition()
        {
            if (SearchCriteria == null)
            {
                SearchCriteria = new DrugSearchCriteria();
            }
            SearchCriteria.IsConsult = IsConsult;
            SearchCriteria.IsInsurance = IsInsurance;
            SearchCriteria.IsActive = IsActive;
            SearchCriteria.IsShow = IsShow;
            switch(IsCatDrugType)
            {
                case 0:
                    SearchCriteria.V_CatDrugType = (long)AllLookupValues.V_CatDrugType.All;
                    break;
                case 1:
                    SearchCriteria.V_CatDrugType = (long)AllLookupValues.V_CatDrugType.Shared;
                    break;
                case 2:
                    SearchCriteria.V_CatDrugType = (long)AllLookupValues.V_CatDrugType.Pharmacy;
                    break;
                case 3:
                    SearchCriteria.V_CatDrugType = (long)AllLookupValues.V_CatDrugType.DrugDept;
                    break;
            }
            strNameExcel = "";
            if (string.IsNullOrEmpty(strBH))
            {
                strNameExcel = string.Format("{0} ", eHCMSResources.Z1044_G1_DMucThuoc) + strHoiChan;
            }
            else
            {
                if (!string.IsNullOrEmpty(strHoiChan))
                {
                    strNameExcel = string.Format("{0} ", eHCMSResources.Z1044_G1_DMucThuoc) + strBH + ", " + strHoiChan;
                }
                else
                {
                    strNameExcel = string.Format("{0} ", eHCMSResources.Z1044_G1_DMucThuoc) + strBH;
                }
            }
        }

        public void Search(object sender, RoutedEventArgs e)
        {
            DrugsResearch.PageIndex = 0;
            SearchDrugs(0, DrugsResearch.PageSize);
        }

        public void btnAddNew(object sender, RoutedEventArgs e)
        {
            //KMx: Chuyển sang view mới, vì phải thêm cột để báo cáo mẫu 20 mới (01/09/2015 16:27).
            //var proAlloc = Globals.GetViewModel<IRefGenDrug_Add>();
            void onInitDlg(IRefGenDrug_Add_V2New proAlloc)
            {
                proAlloc.IsAddFinishClosed = false;
                proAlloc.IsEdit = false;
                proAlloc.IsAdd = true;

                proAlloc.AllCountries = Countries;
                //proAlloc.Hospitals = Hospitals;
                proAlloc.AllFamilyTherapies = FamilyTherapies;
                proAlloc.AllPharmaceuticalCompanies = PharmaceuticalCompanies;
                proAlloc.AllRefGenDrugBHYT_Categorys = RefGenDrugBHYT_Categorys;
                proAlloc.RefGenericDrugCategory_2s = RefGenericDrugCategory_2s;
                proAlloc.Suppliers = Suppliers;
                proAlloc.Units = Units;
                proAlloc.TitleForm = eHCMSResources.Z1626_G1_ThemThuocVaoDM;
            }
            GlobalsNAV.ShowDialog<IRefGenDrug_Add_V2New>(onInitDlg);
        }

        public void lnkDrugID_Click(object sender, RoutedEventArgs e)
        {
            void onInitDlg(IRefGenDrug_ShowNew proAlloc)
            {
                proAlloc.SelectedDrug = CurrentDrug.DeepCopy();
            }
            GlobalsNAV.ShowDialog<IRefGenDrug_ShowNew>(onInitDlg);
        }

        public void lnkEdit_Click(object sender, RoutedEventArgs e)
        {
            //var proAlloc = Globals.GetViewModel<IRefGenDrug_Edit>();
            //proAlloc.NewDrug = CurrentDrug.DeepCopy();

            //proAlloc.Countries = Countries;
            //proAlloc.Hospitals = Hospitals;
            //proAlloc.FamilyTherapies = FamilyTherapies;
            //proAlloc.PharmaceuticalCompanies = PharmaceuticalCompanies;
            //proAlloc.RefGenDrugBHYT_Categorys = RefGenDrugBHYT_Categorys;
            //proAlloc.RefGenericDrugCategory_2s = RefGenericDrugCategory_2s;
            //proAlloc.Suppliers = Suppliers;
            //proAlloc.Units = Units;

            //var instance = proAlloc as Conductor<object>;
            //instance.DisplayName = "Chỉnh Sửa Thuốc "+ CurrentDrug.BrandName;
            //Globals.ShowDialog(instance, (o) => { });

            //KMx: Chuyển sang view mới, vì phải thêm cột để báo cáo mẫu 20 mới (01/09/2015 16:27).
            //var proAlloc = Globals.GetViewModel<IRefGenDrug_Add>();
            void onInitDlg(IRefGenDrug_Add_V2New proAlloc)
            {
                proAlloc.NewDrug = CurrentDrug.DeepCopy();
                proAlloc.IsAdd = false;
                proAlloc.IsEdit = true;
                proAlloc.TitleForm = eHCMSResources.Z1628_G1_ChSuaThuoc;

                proAlloc.AllCountries = Countries;
                //proAlloc.Hospitals = Hospitals;
                proAlloc.AllFamilyTherapies = FamilyTherapies;
                proAlloc.AllPharmaceuticalCompanies = PharmaceuticalCompanies;
                proAlloc.AllRefGenDrugBHYT_Categorys = RefGenDrugBHYT_Categorys;
                proAlloc.RefGenericDrugCategory_2s = RefGenericDrugCategory_2s;
                proAlloc.Suppliers = Suppliers;
                proAlloc.Units = Units;
            }
            GlobalsNAV.ShowDialog<IRefGenDrug_Add_V2New>(onInitDlg);
        }

        private void DeleteDrugByID()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeleteDrugByID_New(CurrentDrug.DrugID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndDeleteDrugByID_New(asyncResult);
                                SearchDrugs(DrugsResearch.PageIndex, DrugsResearch.PageSize);
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

        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.A0121_G1_Msg_ConfXoaThuoc, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (CurrentDrug != null)
                {
                    DeleteDrugByID();
                }
            }
        }

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
            if (e.AddedItems.Count > 0)
            {
                SearchCriteria.FaID = Convert.ToDecimal(((DrugClass)e.AddedItems[0]).DrugClassID);
            }
            SearchDrugs(0, DrugsResearch.PageSize);
        }

        public void griddrug_DblClick(object sender, EventArgs<object> e)
        {
            if (IsPopUp)
            {
                TryClose();
                Globals.EventAggregator.Publish(new PharmacyCloseSearchDrugEvent() { SupplierDrug = e.Value });
            }
            else
            {
                //KMx: Chuyển sang view mới, vì phải thêm cột để báo cáo mẫu 20 mới (01/09/2015 16:27).
                //open popup chinh sua
                //var proAlloc = Globals.GetViewModel<IRefGenDrug_Add>();
                void onInitDlg(IRefGenDrug_Add_V2New proAlloc)
                {
                    proAlloc.NewDrug = CurrentDrug.DeepCopy();
                    proAlloc.IsAdd = false;
                    proAlloc.IsEdit = true;
                    proAlloc.TitleForm = eHCMSResources.Z1628_G1_ChSuaThuoc;

                    proAlloc.AllCountries = Countries;
                    //proAlloc.Hospitals = Hospitals;
                    proAlloc.AllFamilyTherapies = FamilyTherapies;
                    proAlloc.AllPharmaceuticalCompanies = PharmaceuticalCompanies;
                    proAlloc.AllRefGenDrugBHYT_Categorys = RefGenDrugBHYT_Categorys;
                    proAlloc.RefGenericDrugCategory_2s = RefGenericDrugCategory_2s;
                    proAlloc.Suppliers = Suppliers;
                    proAlloc.Units = Units;
                }
                GlobalsNAV.ShowDialog<IRefGenDrug_Add_V2New>(onInitDlg);
            }
        }

        private string strBH = "";
        public void IsInsurance1_Checked(object sender, RoutedEventArgs e)
        {
            IsInsurance = (byte)Insurance.All;
            strBH = "";
        }

        public void IsInsurance2_Checked(object sender, RoutedEventArgs e)
        {
            IsInsurance = (byte)Insurance.Yes;
            strBH = eHCMSResources.K0791_G1_1BHYT.ToUpper();
        }

        public void IsInsurance3_Checked(object sender, RoutedEventArgs e)
        {
            IsInsurance = (byte)Insurance.No;
            strBH = "Không BHYT";
        }

        public void CatDrugType1_Checked(object sender, RoutedEventArgs e)
        {
            IsCatDrugType = (byte)CatDrugType.All;
        }

        public void CatDrugType2_Checked(object sender, RoutedEventArgs e)
        {
            IsCatDrugType = (byte)CatDrugType.Shared;
        }

        public void CatDrugType3_Checked(object sender, RoutedEventArgs e)
        {
            IsCatDrugType = (byte)CatDrugType.Pharmacy;
        }

        public void CatDrugType4_Checked(object sender, RoutedEventArgs e)
        {
            IsCatDrugType = (byte)CatDrugType.DrugDept;
        }

        private string strHoiChan = "";
        public void IsConsult1_Checked(object sender, RoutedEventArgs e)
        {
            IsConsult = (byte)Consult.All;
            strHoiChan = "";
        }

        public void IsConsult2_Checked(object sender, RoutedEventArgs e)
        {
            IsConsult = (byte)Consult.Yes;
            strHoiChan = eHCMSResources.Z0049_G1_CanHoiChan;
        }

        public void IsConsult3_Checked(object sender, RoutedEventArgs e)
        {
            IsConsult = (byte)Consult.No;
            strHoiChan = eHCMSResources.T2457_G1_KhongCanHChan;
        }

        public void IsActive1_Checked(object sender, RoutedEventArgs e)
        {
            IsActive = (byte)DIsActive.All;
        }

        public void IsActive2_Checked(object sender, RoutedEventArgs e)
        {
            IsActive = (byte)DIsActive.Yes;
        }

        public void IsActive3_Checked(object sender, RoutedEventArgs e)
        {
            IsActive = (byte)DIsActive.No;
        }

        public void IsShow1_Checked(object sender, RoutedEventArgs e)
        {
            IsShow = (byte)Show.All;
        }

        public void IsShow2_Checked(object sender, RoutedEventArgs e)
        {
            IsShow = (byte)Show.Yes;
        }

        public void IsShow3_Checked(object sender, RoutedEventArgs e)
        {
            IsShow = (byte)Show.No;
        }

        #region IHandle<PharmacyCloseAddGenDrugEvent> Members

        public void Handle(PharmacyCloseAddGenDrugEvent message)
        {
            SearchDrugs(DrugsResearch.PageIndex, DrugsResearch.PageSize);
        }

        #endregion

        #region IHandle<PharmacyCloseEditGenDrugEvent> Members

        public void Handle(PharmacyCloseEditGenDrugEvent message)
        {
            SearchDrugs(DrugsResearch.PageIndex, DrugsResearch.PageSize);
        }

        #endregion

        public void btnExportExcel()
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            GetCondition();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginExportToExcellAll_ListRefGenericDrug_New(SearchCriteria, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndExportToExcellAll_ListRefGenericDrug_New(asyncResult);
                                ExportToExcelFileAllData.Export(results, strNameExcel);
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
    }
}
