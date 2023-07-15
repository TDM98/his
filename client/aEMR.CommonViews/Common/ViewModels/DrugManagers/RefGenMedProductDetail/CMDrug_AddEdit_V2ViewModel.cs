using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using DataEntities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using eHCMSLanguage;
using aEMR.ViewContracts;
using aEMR.Infrastructure.Events;
using aEMR.Common.BaseModel;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure;
using aEMR.Common.Collections;
using aEMR.CommonTasks;
using aEMR.ServiceClient;
using aEMR.Controls;
using aEMR.Common.Utilities;
using Castle.Core.Logging;
/*
* 20170810 #002 CMN:    Added Bid Service
* 20171019 #003 CMN:    Added button create new from old value
* 20180109 #004 CMN:    Added more properties for HI Informations.
* 20180412 #005 TTM:    Added condition for Volume
* 20181006 #006 TTM:    Thay đổi cách lấy giá trị cột từ e.Column.GetValue(FrameworkElement.NameProperty).ToString()
*                       => e.Column.Equals("Tên Grid".GetColumnByName("Tên cột") vì cách cũ bị sai => Lấy giá trị cột không đc.
* 20191006 #007 TNHX: [Bitrix 896] Insert/ Update ConIndicatorDrugsRelToMedCondXML where Insert new Drug + refactor code
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(ICMDrug_AddEdit_V2)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CMDrug_AddEdit_V2ViewModel : ViewModelBase, ICMDrug_AddEdit_V2
        , IHandle<DrugDeptCloseSearchSupplierEvent>
        , IHandle<MedDeptContraEvent>
    {
        /*▼====: #003*/
        private bool _HasOldValue = false;
        public bool HasOldValue
        {
            get { return _HasOldValue; }
            set
            {
                if (_HasOldValue != value)
                {
                    _HasOldValue = value;
                    NotifyOfPropertyChange(() => HasOldValue);
                }
            }
        }

        public void btAddNewFromOldValue()
        {
            ObjRefGenMedProductDetails_Current.GenMedProductID = 0;
            HasOldValue = false;
        }
        /*▲====: #003*/

        public override bool IsProcessing
        {
            get
            {
                return _isWaitingSave;
            }
        }

        public override string StatusText
        {
            get
            {
                if (_isWaitingSave)
                {
                    return eHCMSResources.Z0343_G1_DangLuu;
                }
                return string.Empty;
            }
        }

        private bool _isWaitingSave;
        public bool IsWaitingSave
        {
            get { return _isWaitingSave; }
            set
            {
                if (_isWaitingSave != value)
                {
                    _isWaitingSave = value;
                    NotifyOfPropertyChange(() => IsWaitingSave);
                    NotifyWhenBusy();

                    NotifyOfPropertyChange(() => CanbtSave);
                }
            }
        }

        public bool IsReadOnlyCode
        {
            get { return Globals.ServerConfigSection.MedDeptElements.AutoCreateMedCode; }
        }

        //private long? CityProvinceID = 42;//tam thoi de TP HCM la vay di

        private SupplierGenMedProduct _CurrentSupplierGenMedProduct;
        public SupplierGenMedProduct CurrentSupplierGenMedProduct
        {
            get
            {
                return _CurrentSupplierGenMedProduct;
            }
            set
            {
                if (_CurrentSupplierGenMedProduct != value)
                {
                    _CurrentSupplierGenMedProduct = value;
                    NotifyOfPropertyChange(() => CurrentSupplierGenMedProduct);
                }
            }
        }

        private long _V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
        public long V_MedProductType
        {
            get { return _V_MedProductType; }
            set
            {
                _V_MedProductType = value;
                NotifyOfPropertyChange(() => V_MedProductType);
            }

        }

        private bool _CanEdit = true;
        public bool CanEdit
        {
            get { return _CanEdit; }
            set
            {
                _CanEdit = value;
                NotifyOfPropertyChange(() => CanEdit);
            }

        }
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public CMDrug_AddEdit_V2ViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            eventArg.Subscribe(this);

            #region get reference member

            Hospitals = new ObservableCollection<Hospital>();

            if (Globals.allHospitals == null)
            {
                var vm = Globals.GetViewModel<IHome>();
                vm.LoadAllHospitalInfoAction();
            }

            Coroutine.BeginExecute(GetAllReferenceValues());

            #endregion

            ////Gán UC vào
            //var CtrUC_Left0 = Globals.GetViewModel<IDrugClasses_SearchPaging_Drug>();
            //CtrUC_Left0.criteria = new DrugClassSearchCriteria();
            //CtrUC_Left0.criteria.V_MedProductType = 11001;
            //UCDrugClassesSearchPagingDrug = CtrUC_Left0;
            //ActivateItem(UCDrugClassesSearchPagingDrug);

            //var CtrUC_Right0 = Globals.GetViewModel<IRefCountries_SearchPaging>();
            //UCRefCountriesSearchPaging = CtrUC_Right0;
            //ActivateItem(UCRefCountriesSearchPaging);

            //var CtrUC_Right01 = Globals.GetViewModel<IRefUnitsCal_SearchPaging>();
            //UCRefUnitsCalSearchPaging = CtrUC_Right01;
            //ActivateItem(UCRefUnitsCalSearchPaging);

            //var CtrUC_Right02 = Globals.GetViewModel<IRefUnitsUse_SearchPaging>();
            //UCRefUnitsUseSearchPaging = CtrUC_Right02;
            //ActivateItem(UCRefUnitsUseSearchPaging);
            ////Gán UC vào

            GroupTypeForReport20 = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_GroupTypeForReport20).ToObservableCollection();
            AllRouteOfAdministration = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_RouteOfAdministration).ToObservableCollection();

            allContraIndicatorDrugsRelToMedCond = new ObservableCollection<ContraIndicatorMedProductsRelToMedCond>();
            RefMedicalConditionType_Edit = new EntitiesEdit<RefMedContraIndicationTypes>();

            /*▼====: #004*/
            ProductScopeCollection = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_ProductScope).ToObservableCollection();
            LoadHITransactions();
            /*▲====: #004*/
            GetAllRefGeneric();
        }

        private IEnumerator<IResult> GetAllReferenceValues()
        {
            var LoadSupplierListTask = new LoadDrugDeptSupplierListTask((int)AllLookupValues.SupplierType.CUNG_CAP_THIET_BI_Y_TE, false, false);
            yield return LoadSupplierListTask;
            Suppliers = LoadSupplierListTask.DrugDeptSupplierList;

            var LoadCategory_1_Task = new LoadRefGenericDrugCategory_1ListTask(V_MedProductType, false, false);
            yield return LoadCategory_1_Task;
            RefGenericDrugCategory_1s = LoadCategory_1_Task.RefGenericDrugCategory_1List;

            yield return GenericCoRoutineTask.StartTask(LoadRefGenDrugCategory_2, (object)false, (object)false);

            //var LoadCategory_BHYT_Task = new LoadRefGenDrugBHYT_CategoryListTask(null, false, true);
            var LoadCategory_BHYT_Task = new LoadRefGenDrugBHYT_CategoryListTask(false, null, false, true);
            yield return LoadCategory_BHYT_Task;
            AllRefGenDrugBHYT_Categorys = LoadCategory_BHYT_Task.RefGenDrugBHYT_CategoryList;
            var LoadUnitListTask = new LoadDrugDeptUnitListTask(false, false);
            yield return LoadUnitListTask;
            Units = LoadUnitListTask.RefUnitList;
            var LoadDrugClassListTask = new LoadDrugDeptClassListTask(V_MedProductType, false, false);
            yield return LoadDrugClassListTask;
            AllFamilyTherapies = LoadDrugClassListTask.DrugClassList;
            var LoadCountryListTask = new LoadCountryListTask(false, false);
            yield return LoadCountryListTask;
            AllCountries = LoadCountryListTask.RefCountryList;
            var paymentTypeTask = new LoadDrugDeptPharmaceuticalCompanyListTask(false, false);
            yield return paymentTypeTask;
            AllPharmaceuticalCompanies = paymentTypeTask.DrugDeptPharmaceuticalCompanyList;
        }

        private void LoadRefGenDrugCategory_2(GenericCoRoutineTask theTask, object bAddSelectOneItem, object bAddSelectedAllItem)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginRefGenericDrugCategory_2_Get(V_MedProductType,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<RefGenericDrugCategory_2> allItems = new ObservableCollection<RefGenericDrugCategory_2>();
                                try
                                {
                                    allItems = contract.EndRefGenericDrugCategory_2_Get(asyncResult);
                                }
                                catch (Exception)
                                {

                                }
                                finally
                                {
                                    RefGenericDrugCategory_2s = new ObservableCollection<RefGenericDrugCategory_2>(allItems);
                                    if ((bool)bAddSelectOneItem)
                                    {
                                        RefGenericDrugCategory_2 firstItem = new RefGenericDrugCategory_2();
                                        firstItem.RefGenDrugCatID_2 = -1;
                                        firstItem.CategoryName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0111_G1_HayChon1GTri);
                                        RefGenericDrugCategory_2s.Insert(0, firstItem);
                                    }
                                    if ((bool)bAddSelectedAllItem)
                                    {
                                        RefGenericDrugCategory_2 firstItem = new RefGenericDrugCategory_2();
                                        firstItem.RefGenDrugCatID_2 = -2;
                                        firstItem.CategoryName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                        RefGenericDrugCategory_2s.Insert(0, firstItem);
                                    }
                                    theTask.ActionComplete(true);
                                }
                            }), null);
                    }
                }
                catch (Exception)
                {
                    theTask.ActionComplete(true);
                }
            });

            t.Start();
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

        private RefGenMedProductDetails _ObjRefGenMedProductDetails_Current;
        public RefGenMedProductDetails ObjRefGenMedProductDetails_Current
        {
            get { return _ObjRefGenMedProductDetails_Current; }
            set
            {
                _ObjRefGenMedProductDetails_Current = value;
                NotifyOfPropertyChange(() => ObjRefGenMedProductDetails_Current);
            }
        }

        AutoCompleteBox Auto_DrugClass;
        AutoCompleteBox Auto_PharmaCompany;
        AutoCompleteBox Auto_PharmaCountry;
        AutoCompleteBox Auto_Hospital;
        AutoCompleteBox Auto_HIGroup;

        public void Auto_DrugClass_Loaded(object sender, RoutedEventArgs e)
        {
            Auto_DrugClass = sender as AutoCompleteBox;
        }

        public void Auto_DrugClass_Populating(object sender, PopulatingEventArgs e)
        {
            if (AllFamilyTherapies == null || AllFamilyTherapies.Count <= 0)
            {
                return;
            }
            //20180713 TBL: Them kiem tra e.Parameter neu khong se bao loi 
            if (e.Parameter == null)
            {
                return;
            }

            FamilyTherapies = new ObservableCollection<DrugClass>();
            foreach (var x in AllFamilyTherapies)
            {
                string str = VNConvertString.ConvertString(x.FaName);
                str = str.ToLower();
                bool bres = str.Contains(VNConvertString.ConvertString(e.Parameter).ToLower());
                if (bres)
                {
                    FamilyTherapies.Add(x);
                }
            }
            Auto_DrugClass.ItemsSource = FamilyTherapies;
            Auto_DrugClass.PopulateComplete();
        }

        public void Auto_PharmaCompany_Loaded(object sender, RoutedEventArgs e)
        {
            Auto_PharmaCompany = sender as AutoCompleteBox;
        }

        public void Auto_PharmaCompany_Populating(object sender, PopulatingEventArgs e)
        {
            if (AllPharmaceuticalCompanies == null || AllPharmaceuticalCompanies.Count <= 0)
            {
                return;
            }
            //20180713 TBL: Them kiem tra e.Parameter neu khong se bao loi 
            if (e.Parameter == null)
            {
                return;
            }

            PharmaceuticalCompanies = new ObservableCollection<DrugDeptPharmaceuticalCompany>();
            foreach (var x in AllPharmaceuticalCompanies)
            {
                string str = VNConvertString.ConvertString(x.PCOName);
                str = str.ToLower();
                bool bres = str.Contains(VNConvertString.ConvertString(e.Parameter).ToLower());
                if (bres)
                {
                    PharmaceuticalCompanies.Add(x);
                }
            }
            Auto_PharmaCompany.ItemsSource = PharmaceuticalCompanies;
            Auto_PharmaCompany.PopulateComplete();
        }

        public void Auto_PharmaCountry_Loaded(object sender, RoutedEventArgs e)
        {
            Auto_PharmaCountry = sender as AutoCompleteBox;
        }

        public void Auto_PharmaCountry_Populating(object sender, PopulatingEventArgs e)
        {
            if (AllCountries == null || AllCountries.Count <= 0)
            {
                return;
            }
            //20180713 TBL: Them kiem tra e.Parameter neu khong se bao loi 
            if (e.Parameter == null)
            {
                return;
            }

            Countries = new ObservableCollection<RefCountry>();
            foreach (var x in AllCountries)
            {
                string str = VNConvertString.ConvertString(x.CountryName);
                str = str.ToLower();
                bool bres = str.Contains(VNConvertString.ConvertString(e.Parameter).ToLower());
                if (bres)
                {
                    Countries.Add(x);
                }
            }
            Auto_PharmaCountry.ItemsSource = Countries;
            Auto_PharmaCountry.PopulateComplete();
        }

        public void Auto_Hospital_Loaded(object sender, RoutedEventArgs e)
        {
            Auto_Hospital = sender as AutoCompleteBox;
        }

        public void Auto_HIGroup_Loaded(object sender, RoutedEventArgs e)
        {
            Auto_HIGroup = sender as AutoCompleteBox;
        }

        public void Auto_HIGroup_Populating(object sender, PopulatingEventArgs e)
        {
            if (AllRefGenDrugBHYT_Categorys == null || AllRefGenDrugBHYT_Categorys.Count <= 0)
            {
                return;
            }
            //20180713 TBL: Them kiem tra e.Parameter neu khong se bao loi 
            if (e.Parameter == null)
            {
                return;
            }

            RefGenDrugBHYT_Categorys = new ObservableCollection<RefGenDrugBHYT_Category>();
            foreach (var x in AllRefGenDrugBHYT_Categorys)
            {
                string str = VNConvertString.ConvertString(x.CategoryName);
                str = str.ToLower();
                bool bres = str.Contains(VNConvertString.ConvertString(e.Parameter).ToLower());
                if (bres)
                {
                    RefGenDrugBHYT_Categorys.Add(x);
                }
            }
            Auto_HIGroup.ItemsSource = RefGenDrugBHYT_Categorys;
            Auto_HIGroup.PopulateComplete();
        }

        public void InitializeNewItem(long pV_MedProductType)
        {
            ObjRefGenMedProductDetails_Current = new RefGenMedProductDetails
            {
                UnitPackaging = 1,
                NumberOfEstimatedMonths_F = 1,
                V_MedProductType = pV_MedProductType,

                IsActive = true,

                RefGenMedDrugDetails = new RefGenMedDrugDetails()
            };
            ObjRefGenMedProductDetails_Current.RefGenMedDrugDetails.AdvTimeBeforeExpire = 1;

            AddBlankRow();

            if (Auto_DrugClass != null)
            {
                Auto_DrugClass.Text = "";
            }

            if (Auto_PharmaCompany != null)
            {
                Auto_PharmaCompany.Text = "";
            }

            if (Auto_PharmaCountry != null)
            {
                Auto_PharmaCountry.Text = "";
            }

            if (Auto_Hospital != null)
            {
                Auto_Hospital.Text = "";
            }

            if (Auto_HIGroup != null)
            {
                Auto_HIGroup.Text = "";
            }
        }

        #region Properties reference member

        private ObservableCollection<DrugDeptSupplier> _Suppliers;
        public ObservableCollection<DrugDeptSupplier> Suppliers
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

        private ObservableCollection<Hospital> _Hospitals;
        public ObservableCollection<Hospital> Hospitals
        {
            get
            {
                return _Hospitals;
            }
            set
            {
                if (_Hospitals != value)
                {
                    _Hospitals = value;
                    NotifyOfPropertyChange(() => Hospitals);
                }
            }
        }

        private ObservableCollection<RefGenDrugBHYT_Category> _AllRefGenDrugBHYT_Categorys;
        public ObservableCollection<RefGenDrugBHYT_Category> AllRefGenDrugBHYT_Categorys
        {
            get
            {
                return _AllRefGenDrugBHYT_Categorys;
            }
            set
            {
                if (_AllRefGenDrugBHYT_Categorys != value)
                {
                    _AllRefGenDrugBHYT_Categorys = value;
                    NotifyOfPropertyChange(() => AllRefGenDrugBHYT_Categorys);
                }
            }
        }

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

        private ObservableCollection<RefGenericDrugCategory_1> _RefGenericDrugCategory_1s;
        public ObservableCollection<RefGenericDrugCategory_1> RefGenericDrugCategory_1s
        {
            get
            {
                return _RefGenericDrugCategory_1s;
            }
            set
            {
                if (_RefGenericDrugCategory_1s != value)
                {
                    _RefGenericDrugCategory_1s = value;
                    NotifyOfPropertyChange(() => RefGenericDrugCategory_1s);
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

        private ObservableCollection<RefCountry> _AllCountries;
        public ObservableCollection<RefCountry> AllCountries
        {
            get
            {
                return _AllCountries;
            }
            set
            {
                if (_AllCountries != value)
                {
                    _AllCountries = value;
                    NotifyOfPropertyChange(() => AllCountries);
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

        private ObservableCollection<DrugClass> _AllFamilytherapies;
        public ObservableCollection<DrugClass> AllFamilyTherapies
        {
            get
            {
                return _AllFamilytherapies;
            }
            set
            {
                if (_AllFamilytherapies != value)
                {
                    _AllFamilytherapies = value;
                    NotifyOfPropertyChange(() => AllFamilyTherapies);
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

        private ObservableCollection<DrugDeptPharmaceuticalCompany> _AllPharmaceuticalCompanies;
        public ObservableCollection<DrugDeptPharmaceuticalCompany> AllPharmaceuticalCompanies
        {
            get { return _AllPharmaceuticalCompanies; }
            set
            {
                if (_AllPharmaceuticalCompanies != value)
                    _AllPharmaceuticalCompanies = value;
                NotifyOfPropertyChange(() => AllPharmaceuticalCompanies);
            }
        }

        private ObservableCollection<DrugDeptPharmaceuticalCompany> _pharmaceuticalCompanies;
        public ObservableCollection<DrugDeptPharmaceuticalCompany> PharmaceuticalCompanies
        {
            get { return _pharmaceuticalCompanies; }
            set
            {
                if (_pharmaceuticalCompanies != value)
                    _pharmaceuticalCompanies = value;
                NotifyOfPropertyChange(() => PharmaceuticalCompanies);
            }
        }

        private ObservableCollection<Lookup> _GroupTypeForReport20;
        public ObservableCollection<Lookup> GroupTypeForReport20
        {
            get { return _GroupTypeForReport20; }
            set
            {
                if (_GroupTypeForReport20 != value)
                    _GroupTypeForReport20 = value;
                NotifyOfPropertyChange(() => GroupTypeForReport20);
            }
        }


        private ObservableCollection<Lookup> _AllRouteOfAdministration;
        public ObservableCollection<Lookup> AllRouteOfAdministration
        {
            get { return _AllRouteOfAdministration; }
            set
            {
                if (_AllRouteOfAdministration != value)
                    _AllRouteOfAdministration = value;
                NotifyOfPropertyChange(() => AllRouteOfAdministration);
            }
        }

        private ObservableCollection<Lookup> _RouteOfAdministrationSource;
        public ObservableCollection<Lookup> RouteOfAdministrationSource
        {
            get { return _RouteOfAdministrationSource; }
            set
            {
                if (_RouteOfAdministrationSource != value)
                    _RouteOfAdministrationSource = value;
                NotifyOfPropertyChange(() => RouteOfAdministrationSource);
            }
        }

        #endregion

        private bool CheckData()
        {
            //if (ObjRefGenMedProductDetails_Current.SelectedCountry != null)
            //{
            //    ObjRefGenMedProductDetails_Current.CountryID = ObjRefGenMedProductDetails_Current.SelectedCountry.CountryID;
            //}

            //if (ObjRefGenMedProductDetails_Current.SelectedUnit != null)
            //{
            //    ObjRefGenMedProductDetails_Current.UnitID = ObjRefGenMedProductDetails_Current.SelectedUnit.UnitID;

            //}

            //if (ObjRefGenMedProductDetails_Current.SelectedUnitUse != null)
            //{
            //    ObjRefGenMedProductDetails_Current.UnitUseID = ObjRefGenMedProductDetails_Current.SelectedUnitUse.UnitID;
            //}

            //if (ObjRefGenMedProductDetails_Current.PharmaceuticalCompany != null)
            //{
            //    ObjRefGenMedProductDetails_Current.PCOID = ObjRefGenMedProductDetails_Current.PharmaceuticalCompany.PCOID;
            //}

            //if (ObjRefGenMedProductDetails_Current.SelectedDrugClass != null)
            //{
            //    ObjRefGenMedProductDetails_Current.DrugClassID = ObjRefGenMedProductDetails_Current.SelectedDrugClass.DrugClassID;
            //}

            if (ObjRefGenMedProductDetails_Current.SupplierGenMedProducts == null || ObjRefGenMedProductDetails_Current.SupplierGenMedProducts.Count == 0)
            {
                MessageBox.Show(eHCMSResources.A0195_G1_Msg_InfoChonNCCChoThuoc);
                return false;
            }
            else
            {
                int icount = 0;
                for (int i = 0; i < ObjRefGenMedProductDetails_Current.SupplierGenMedProducts.Count; i++)
                {
                    if (ObjRefGenMedProductDetails_Current.SupplierGenMedProducts[i].SupplierID > 0)
                    {
                        if (ObjRefGenMedProductDetails_Current.SupplierGenMedProducts[i].UnitPrice <= 0 || ObjRefGenMedProductDetails_Current.SupplierGenMedProducts[i].PackagePrice <= 0)
                        {
                            MessageBox.Show(eHCMSResources.A0519_G1_Msg_InfoDGiaLonHon0);
                            return false;
                        }
                        if (ObjRefGenMedProductDetails_Current.SupplierGenMedProducts[i].VAT < 1)
                        {
                            MessageBox.Show(eHCMSResources.K0265_G1_VATKhongHopLe3);
                            return false;
                        }
                    }
                    if (ObjRefGenMedProductDetails_Current.SupplierGenMedProducts[i].SupplierID > 0 && ObjRefGenMedProductDetails_Current.SupplierGenMedProducts[i].IsMain == true)
                    {
                        icount++;
                        if (icount > 1)
                        {
                            MessageBox.Show(eHCMSResources.A0436_G1_Msg_InfoCoNhieuHon1NCC);
                            return false;
                        }
                    }
                }
                if (icount == 0)
                {
                    MessageBox.Show(eHCMSResources.K0028_G1_ThuocChuaCoNCCChinh);
                    return false;
                }
                if (ObjRefGenMedProductDetails_Current.TLThanhToan < 0 || ObjRefGenMedProductDetails_Current.TLThanhToan > 100)
                {
                    MessageBox.Show("Tỷ lệ thanh toán không nhỏ hơn 0% và không lớn hơn 100%");
                    return false;
                }
                /*==== #001 ====*/
                if (ObjRefGenMedProductDetails_Current.InsuranceCover == true)
                {
                    string error = "";
                    if (string.IsNullOrEmpty(ObjRefGenMedProductDetails_Current.HICode))
                    {
                        error += eHCMSResources.Z2838_G1_ChuaNhapHICode + Environment.NewLine;
                    }
                    if (string.IsNullOrEmpty(ObjRefGenMedProductDetails_Current.ReportBrandName))
                    {
                        error += eHCMSResources.Z2839_G1_ChuaNhapTenThanhPham + Environment.NewLine;
                    }
                    if (string.IsNullOrEmpty(ObjRefGenMedProductDetails_Current.Visa))
                    {
                        error += eHCMSResources.Z2840_G1_ChuaNhapSDK + Environment.NewLine;
                    }
                    if (string.IsNullOrEmpty(ObjRefGenMedProductDetails_Current.BidDecisionNumAndOrdinalNum))
                    {
                        error += eHCMSResources.Z2843_G1_ChuaNhapSoQDSTT + Environment.NewLine;
                    }
                    if (!string.IsNullOrEmpty(error))
                    {
                        MessageBox.Show(error, eHCMSResources.G0442_G1_TBao);
                        return false;
                    }
                }
                /*==== #001 ====*/
            }
            return true;
        }

        public bool CanbtSave
        {
            get
            {
                return !IsWaitingSave;
            }
        }

        public ObservableCollection<ContraIndicatorMedProductsRelToMedCond> createListDelete()
        {
            var lstNew = new ObservableCollection<ContraIndicatorMedProductsRelToMedCond>();
            foreach (var item in RefMedicalConditionType_Edit.DeleteObject)
            {
                foreach (var contra in allContraIndicatorDrugsRelToMedCond)
                {
                    if (item.MedContraTypeID == contra.MCTypeID)
                    {
                        lstNew.Add(contra);
                    }
                }
            }
            return lstNew;
        }

        public ObservableCollection<ContraIndicatorMedProductsRelToMedCond> createListNew()
        {
            var lstNew = new ObservableCollection<ContraIndicatorMedProductsRelToMedCond>();
            foreach (var item in RefMedicalConditionType_Edit.NewObject)
            {
                ContraIndicatorMedProductsRelToMedCond p = new ContraIndicatorMedProductsRelToMedCond();
                p.RefGenMedProductDetails = ObjRefGenMedProductDetails_Current;
                p.RefMedicalConditionType = item;
                lstNew.Add(p);
            }
            return lstNew;
        }

        public void btSave()
        {
            if (CheckValid(ObjRefGenMedProductDetails_Current))
            {
                /*▼====: #005*/
                //TTM 16082018: Sau đợt release ngày hôm này (16/08/2018) cần phải chỉnh sửa lại if này
                //Không thể để hardcore vào trong CT
                if (ObjRefGenMedProductDetails_Current.RefGenDrugCatID_2 != Globals.ServerConfigSection.MedDeptElements.IntravenousCatID || ObjRefGenMedProductDetails_Current.RefGenDrugCatID_2 != 47)
                {
                    ObjRefGenMedProductDetails_Current.Volume = 0;
                }
                /*▲====: #005*/
                if (CheckData())
                {
                    IsWaitingSave = true;
                    RefGenMedProductDetails_Save(ObjRefGenMedProductDetails_Current);
                }
            }
        }

        public void btClear()
        {
            if (MessageBox.Show(eHCMSResources.A0173_G1_Msg_ConfClearAll, eHCMSResources.G2363_G1_XNhan, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                TitleForm = eHCMSResources.Z0459_G1_ThemMoiThuoc;
                InitializeNewItem(V_MedProductType);
            }
        }

        private void InsertDeleteUpdateConIndicatorDrugsRelToMedCondXML(IList<ContraIndicatorMedProductsRelToMedCond> lstInsert
                                                  , IList<ContraIndicatorMedProductsRelToMedCond> lstDelete
                                                  , IList<ContraIndicatorMedProductsRelToMedCond> lstUpdate)
        {
            var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginInsertDeleteUpdateConIndicatorMedProductsRelToMedCondXML(lstInsert, lstDelete
                            , lstUpdate, Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    bool results = contract.EndInsertDeleteUpdateConIndicatorMedProductsRelToMedCondXML(asyncResult);
                                    if (results)
                                    {
                                        MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK);
                                        RefMedicalConditionType_Edit.Clear();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                }
                                finally
                                {
                                    Globals.IsBusy = false;
                                }

                            }), null);

                    }

                });
            t.Start();
        }

        private void RefGenMedProductDetails_Save(RefGenMedProductDetails Obj)
        {
            if (Obj.InsuranceCover == true && Obj.V_ProductScope != (long)AllLookupValues.V_ProductScope.InHIScope)
            {
                MessageBox.Show(eHCMSResources.Z2194_G1_PhamViBHKhongHopLe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                IsWaitingSave = false;
                return;
            }
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0343_G1_DangLuu) });

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginRefGenMedProductDetails_Save(Obj, RefMedicalConditionType_Edit.TempObject, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                string Res = "";
                                long GenMedProductID;
                                contract.EndRefGenMedProductDetails_Save(out Res, out GenMedProductID, asyncResult);
                                /*▼====: #003*/
                                if (GenMedProductID > 0)
                                    HasOldValue = true;
                                /*▲====: #003*/
                                string[] arrRes = new string[2];

                                switch (Res)
                                {
                                    case "Update-0":
                                        {
                                            MessageBox.Show(eHCMSResources.A0608_G1_Msg_InfoHChinhFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                            break;
                                        }
                                    case "Update-1":
                                        {
                                            TitleForm = string.Format("{0}: {1}", eHCMSResources.Z0723_G1_HChinhThuoc, ObjRefGenMedProductDetails_Current.BrandName.Trim());

                                            Globals.EventAggregator.Publish(new Drug_AddEditViewModel_Save_Event() { Result = true });

                                            MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                            //TryClose();
                                            InsertDeleteUpdateConIndicatorDrugsRelToMedCondXML(createListNew(), createListDelete(), null);
                                            break;
                                        }
                                    case "Insert-0":
                                        {
                                            MessageBox.Show(eHCMSResources.A1026_G1_Msg_InfoThemFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                            break;
                                        }
                                    case "Insert-1":
                                        {
                                            Obj.GenMedProductID = GenMedProductID;
                                            Globals.EventAggregator.Publish(new Drug_AddEditViewModel_Save_Event() { Result = true });

                                            MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                            //TryClose();
                                            // add update ConIndicatorDrugsRelToMedCondXML
                                            InsertDeleteUpdateConIndicatorDrugsRelToMedCondXML(createListNew(), createListDelete(), null);
                                            break;
                                        }
                                    default:
                                        {
                                            arrRes = Res.Split('@');

                                            string msg1 = "";
                                            string msg2 = "";

                                            if (arrRes[0] != "")
                                            {
                                                msg1 = eHCMSResources.Z0724_G1_TenThuongMaiDaDuocSD;
                                            }

                                            if (arrRes[1] != "")
                                            {
                                                msg2 = eHCMSResources.Z0725_G1_CodeDaDuocSD;
                                            }

                                            string msg = (msg1 == "" ? "" : "- " + msg1) + (msg2 != "" ? Environment.NewLine + "- " + msg2 : "");

                                            MessageBox.Show(msg + Environment.NewLine + eHCMSResources.I0946_G1_I, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                            break;
                                        }
                                }

                            }
                            catch (Exception ex)
                            {
                                //Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                }
            });

            t.Start();
        }

        public bool CheckValid(object temp)
        {
            RefGenMedProductDetails u = temp as RefGenMedProductDetails;
            if (u == null)
            {
                return false;
            }
            string error = "";
            //KMx: Nếu cấu hình không tự tạo code mà người dùng không nhập code thì không cho lưu (21/08/2014 11:51).
            if (!Globals.ServerConfigSection.MedDeptElements.AutoCreateMedCode && string.IsNullOrEmpty(u.Code))
            {
                error += eHCMSResources.Z2431_G1_ChuaNhapMaThuoc + Environment.NewLine;
            }
            if (string.IsNullOrEmpty(u.BrandName))
            {
                error += eHCMSResources.Z2833_G1_ChuaNhapTenThuongMai + Environment.NewLine;
            }
            if (u.SelectedGeneric == null || u.SelectedGeneric.DrugClassID <= 0)
            {
                error += eHCMSResources.Z2434_G1_ChuaNhapHoatChat + Environment.NewLine;
            }
            if (u.DispenseVolume <= 0)
            {
                error += eHCMSResources.Z0856_G1_DispenseVolume + Environment.NewLine;
            }
            if (u.SelectedUnit == null || u.SelectedUnit.UnitID <= 0)
            {
                error += eHCMSResources.Z2834_G1_ChuaChonDVT + Environment.NewLine;
            }
            if (u.SelectedUnitUse == null || u.SelectedUnitUse.UnitID <= 0)
            {
                error += eHCMSResources.Z2835_G1_ChuaChonDVD + Environment.NewLine;
            }
            if (u.RouteOfAdministration == null || u.RouteOfAdministration.LookupID <= 0)
            {
                error += eHCMSResources.Z2301_G1_BanChuaNhapDuongDung + Environment.NewLine;
            }
            if (string.IsNullOrEmpty(u.Administration))
            {
                error += eHCMSResources.Z2836_G1_ChuaNhapCachDung + Environment.NewLine;
            }
            if (u.PCOID == null || u.PCOID <= 0)
            {
                error += eHCMSResources.Z0854_G1_ChuaChonNSX + Environment.NewLine;
            }
            if (u.CountryID <= 0)
            {
                error += eHCMSResources.Z0853_G1_ChuaChonQuocGia + Environment.NewLine;
            }
            if (u.DrugClassID == null || u.DrugClassID <= 0)
            {
                error += eHCMSResources.Z0855_G1_ChuaChonNhomThuoc + Environment.NewLine;
            }
            if (u.HIPaymentPercent == null || u.HIPaymentPercent <= 0 || u.HIPaymentPercent > 1)
            {
                error += eHCMSResources.Z0857_G1_TyLeTToan + Environment.NewLine;
            }
            if (u.IsWatchOutQty)
            {
                if (u.RemainWarningLevel1 < 0 || u.RemainWarningLevel2 < 0)
                {
                    error += eHCMSResources.Z0858_G1_SLgCanhBaoLonHonBang0 + Environment.NewLine;
                }
                if (u.LimitedOutQty <= 0)
                {
                    error += eHCMSResources.Z0859_G1_SLgGioiHanLonHon0 + Environment.NewLine;
                }
                if (u.LimitedOutQty < u.RemainWarningLevel2 || u.LimitedOutQty < u.RemainWarningLevel1 || u.RemainWarningLevel2 < u.RemainWarningLevel1)
                {
                    error += eHCMSResources.Z0860_G1_I + Environment.NewLine;
                }
            }
            /*▼====: #005*/
            //TTM 16082018: Sau đợt release ngày hôm này (16/08/2018) cần phải chỉnh sửa lại if này
            //Không thể để hardcore vào trong CT
            if (u.RefGenDrugCatID_2 == Globals.ServerConfigSection.MedDeptElements.IntravenousCatID && u.Volume <= 0 || u.RefGenDrugCatID_2 == 47 && u.Volume <= 0)
            {
                error += eHCMSResources.Z2195_G1_LoiDungTich + Environment.NewLine;
            }
            /*▲====: #005*/
            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error);
                return false;
            }
            if (ValidationSummary1 != null && ValidationSummary1.HasDisplayedErrors)
            {
                return false;
            }
            return u.Validate();
        }


        public void btClose()
        {
            TryClose();
        }

        #region Supplier member

        public void SupplierGenMedProduct_LoadDrugIDNotPaging(long GenMedProductID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSupplierGenMedProduct_LoadDrugIDNotPaging(GenMedProductID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            List<RefMedicalServiceItem> ServiceList = null;
                            var results = contract.EndSupplierGenMedProduct_LoadDrugIDNotPaging(out ServiceList,asyncResult);
                            ObjRefGenMedProductDetails_Current.SupplierGenMedProducts = results.ToObservableCollection();
                            if (ObjRefGenMedProductDetails_Current.SupplierGenMedProducts == null)
                            {
                                ObjRefGenMedProductDetails_Current.SupplierGenMedProducts = new ObservableCollection<SupplierGenMedProduct>();
                            }
                            AddBlankRow();
                            /*▼====: #003*/
                            HasOldValue = true;
                            /*▲====: #003*/
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void AddBlankRow()
        {
            if (ObjRefGenMedProductDetails_Current.SupplierGenMedProducts == null)
            {
                ObjRefGenMedProductDetails_Current.SupplierGenMedProducts = new ObservableCollection<SupplierGenMedProduct>();
            }
            SupplierGenMedProduct item = new SupplierGenMedProduct();
            ObjRefGenMedProductDetails_Current.SupplierGenMedProducts.Add(item);
        }



        private bool CheckExists(SupplierGenMedProduct ite)
        {
            if (ite != null && ite.SelectedSupplier != null)
            {
                ite.SupplierID = ite.SelectedSupplier.SupplierID;
                var value = ObjRefGenMedProductDetails_Current.SupplierGenMedProducts.Where(x => x.SupplierID == ite.SupplierID);
                if (value != null && value.Count() > 1)
                {
                    MessageBox.Show(eHCMSResources.A0827_G1_Msg_InfoNCCDaChon);
                    ite.SelectedSupplier = null;
                    return false;
                }
            }
            else
            {
                //MessageBox.Show(eHCMSResources.K0347_G1_ChonNCC);
                return false;
            }
            return true;
        }

        DataGrid GridSuppliers = null;
        public void GridSuppliers_Loaded(object sender, RoutedEventArgs e)
        {
            GridSuppliers = sender as DataGrid;
        }

        string PreparingCellForEdit = "";
        public void GridSuppliers_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            AxTextBox tbl = GridSuppliers.CurrentColumn.GetCellContent(GridSuppliers.SelectedItem) as AxTextBox;
            if (tbl != null)
            {
                PreparingCellForEdit = tbl.Text;
            }
        }

        public void GridSuppliers_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            SupplierGenMedProduct item = e.Row.DataContext as SupplierGenMedProduct;

            //KMx: Không sử dụng index nữa, chuyển sang dùng column name, vì khi user đổi thứ tự column thì logic sẽ bị sai (22/01/2015 10:58).
            //if (e.Column.DisplayIndex == 1)//NCC

            //▼====== #006
            //if (e.Column.GetValue(FrameworkElement.NameProperty).ToString() == "colSupplierName")
            if (e.Column.Equals(GridSuppliers.GetColumnByName("colSupplierName")))
            {
                if (CheckExists(item))
                {
                    if (e.Row.GetIndex() == (ObjRefGenMedProductDetails_Current.SupplierGenMedProducts.Count - 1) && e.EditAction == DataGridEditAction.Commit)
                    {
                        AddBlankRow();
                    }
                }
            }
            //if (e.Column.DisplayIndex == 3)//NCC
            //else if (e.Column.GetValue(FrameworkElement.NameProperty).ToString() == "colUnitPrice")
            else if(e.Column.Equals(GridSuppliers.GetColumnByName("colUnitPrice")))
            {
                decimal value = 0;
                decimal.TryParse(PreparingCellForEdit, out value);

                if (value == item.UnitPrice)
                {
                    return;
                }
                item.PackagePrice = item.UnitPrice * ObjRefGenMedProductDetails_Current.UnitPackaging.GetValueOrDefault(1);
            }
            //if (e.Column.DisplayIndex == 4)//NCC
            //else if (e.Column.GetValue(FrameworkElement.NameProperty).ToString() == "colPackagePrice")
            else if (e.Column.Equals(GridSuppliers.GetColumnByName("colPackagePrice")))
            {
                decimal value = 0;
                decimal.TryParse(PreparingCellForEdit, out value);
                if (value == item.PackagePrice)
                {
                    return;
                }
                if (ObjRefGenMedProductDetails_Current.UnitPackaging.GetValueOrDefault() > 0)
                {
                    item.UnitPrice = item.PackagePrice / ObjRefGenMedProductDetails_Current.UnitPackaging.GetValueOrDefault(1);
                }
            }
            //▲====== #006
        }

        string txt = "";
        public void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txt != (sender as TextBox).Text)
            {
                txt = (sender as TextBox).Text;
                if (!string.IsNullOrEmpty(txt))
                {
                    if (ObjRefGenMedProductDetails_Current != null && ObjRefGenMedProductDetails_Current.SupplierGenMedProducts != null)
                    {
                        int value = 0;
                        int.TryParse(txt, out value);

                        ObjRefGenMedProductDetails_Current.UnitPackaging = value;
                        foreach (SupplierGenMedProduct p in ObjRefGenMedProductDetails_Current.SupplierGenMedProducts)
                        {
                            p.PackagePrice = p.UnitPrice * ObjRefGenMedProductDetails_Current.UnitPackaging.GetValueOrDefault(1);
                        }
                    }
                }
            }

        }



        public void btnDeleteCC_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.A0119_G1_Msg_ConfXoaNCC, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (GridSuppliers != null && GridSuppliers.SelectedItem != null)
                {
                    ObjRefGenMedProductDetails_Current.SupplierGenMedProducts.Remove(GridSuppliers.SelectedItem as SupplierGenMedProduct);
                }
                if (ObjRefGenMedProductDetails_Current.SupplierGenMedProducts.Count == 0)
                {
                    AddBlankRow();
                }
            }
        }

        #endregion

        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            AutoCompleteBox axBHYT = sender as AutoCompleteBox;
            if (ObjRefGenMedProductDetails_Current != null)
            {
                if (ObjRefGenMedProductDetails_Current.RefGenMedDrugDetails == null)
                {
                    ObjRefGenMedProductDetails_Current.RefGenMedDrugDetails = new RefGenMedDrugDetails();
                }
                if (axBHYT != null && axBHYT.SelectedItem != null)
                {
                    RefGenDrugBHYT_Category item = axBHYT.SelectedItem as RefGenDrugBHYT_Category;
                    if (item != null && item.RefGenDrugBHYT_CatID > 0)
                    {
                        ObjRefGenMedProductDetails_Current.RefGenMedDrugDetails.CurrentRefGenDrugBHYT_Category = item;
                        //KMx: Khi chọn nhóm BHYT không tự động set GenericName và ActiveIngredient (30/07/2015 16:21).
                        //ObjRefGenMedProductDetails_Current.GenericName = item.CategoryName;
                        ObjRefGenMedProductDetails_Current.RefGenMedDrugDetails.RefGenDrugBHYT_CatID = item.RefGenDrugBHYT_CatID;
                        ObjRefGenMedProductDetails_Current.HICode = item.DrugOrderNo;
                        //ObjRefGenMedProductDetails_Current.RefGenMedDrugDetails.ActiveIngredient = item.CategoryName;
                    }
                    else
                    {
                        ObjRefGenMedProductDetails_Current.RefGenMedDrugDetails.CurrentRefGenDrugBHYT_Category = null;
                        ObjRefGenMedProductDetails_Current.HICode = "";
                        //KMx: Khi chọn nhóm BHYT không tự động set GenericName và ActiveIngredient (30/07/2015 16:21).
                        //ObjRefGenMedProductDetails_Current.GenericName = "";
                        //if (ObjRefGenMedProductDetails_Current.RefGenMedDrugDetails != null)
                        //{
                        //    ObjRefGenMedProductDetails_Current.RefGenMedDrugDetails.ActiveIngredient = "";
                        //}
                    }
                }
                else
                {
                    ObjRefGenMedProductDetails_Current.RefGenMedDrugDetails.CurrentRefGenDrugBHYT_Category = null;
                    ObjRefGenMedProductDetails_Current.HICode = "";
                    //ObjRefGenMedProductDetails_Current.GenericName = "";
                    //if (ObjRefGenMedProductDetails_Current.RefGenMedDrugDetails != null)
                    //{
                    //    ObjRefGenMedProductDetails_Current.RefGenMedDrugDetails.ActiveIngredient = "";
                    //}
                }
            }
        }

        public void BvApThau_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox bvApThauBox = sender as AutoCompleteBox;
            if (e.Parameter == null || e.Parameter.Length == 0)
            {
                Hospitals = null;
                bvApThauBox.PopulateComplete();
                return;
            }

            e.Cancel = true;
            string SelHosName = e.Parameter;

            ObservableCollection<Hospital> allHos = Globals.allHospitals;
            var qryRes = (from c in allHos
                          where VNConvertString.ConvertString(c.HosName).ToLower().Contains(VNConvertString.ConvertString(bvApThauBox.SearchText).ToLower())
                          select c);

            Hospitals = new ObservableCollection<Hospital>(qryRes);

            bvApThauBox.PopulateComplete();

        }

        public void ApGiaDau_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            AutoCompleteBox axApThau = sender as AutoCompleteBox;
            if (ObjRefGenMedProductDetails_Current != null)
            {
                if (axApThau != null && axApThau.SelectedItem != null)
                {
                    Hospital item = axApThau.SelectedItem as Hospital;
                    if (item != null && item.HosID > 0)
                    {
                        ObjRefGenMedProductDetails_Current.CurrentHospital = item;
                        ObjRefGenMedProductDetails_Current.HosID = item.HosID;
                    }
                    else
                    {
                        ObjRefGenMedProductDetails_Current.CurrentHospital = null;
                        ObjRefGenMedProductDetails_Current.HosID = null;
                    }
                }
                else
                {
                    ObjRefGenMedProductDetails_Current.CurrentHospital = null;
                    ObjRefGenMedProductDetails_Current.HosID = null;
                }
            }
        }

        public void Country_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            AutoCompleteBox axCountry = sender as AutoCompleteBox;
            if (ObjRefGenMedProductDetails_Current != null)
            {
                if (axCountry != null && axCountry.SelectedItem != null)
                {
                    RefCountry item = axCountry.SelectedItem as RefCountry;
                    if (item != null && item.CountryID > 0)
                    {
                        ObjRefGenMedProductDetails_Current.SelectedCountry = item;
                        ObjRefGenMedProductDetails_Current.CountryID = item.CountryID;
                    }
                    else
                    {
                        ObjRefGenMedProductDetails_Current.SelectedCountry = null;
                        ObjRefGenMedProductDetails_Current.CountryID = 0;
                    }
                }
                else
                {
                    ObjRefGenMedProductDetails_Current.SelectedCountry = null;
                    ObjRefGenMedProductDetails_Current.CountryID = 0;
                }
            }

        }

        public void HangSX_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            AutoCompleteBox axCountry = sender as AutoCompleteBox;
            if (ObjRefGenMedProductDetails_Current != null)
            {
                if (axCountry != null && axCountry.SelectedItem != null)
                {
                    DrugDeptPharmaceuticalCompany item = axCountry.SelectedItem as DrugDeptPharmaceuticalCompany;
                    if (item != null && item.PCOID > 0)
                    {
                        ObjRefGenMedProductDetails_Current.PharmaceuticalCompany = item;
                        ObjRefGenMedProductDetails_Current.PCOID = item.PCOID;
                    }
                    else
                    {
                        ObjRefGenMedProductDetails_Current.PharmaceuticalCompany = null;
                        ObjRefGenMedProductDetails_Current.PCOID = 0;
                    }
                }
                else
                {
                    ObjRefGenMedProductDetails_Current.PharmaceuticalCompany = null;
                    ObjRefGenMedProductDetails_Current.PCOID = 0;
                }
            }
        }

        public void NhomThuoc_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            AutoCompleteBox axCountry = sender as AutoCompleteBox;
            if (ObjRefGenMedProductDetails_Current != null)
            {
                if (axCountry != null && axCountry.SelectedItem != null)
                {
                    DrugClass item = axCountry.SelectedItem as DrugClass;
                    if (item != null && item.DrugClassID > 0)
                    {
                        ObjRefGenMedProductDetails_Current.SelectedDrugClass = item;
                        ObjRefGenMedProductDetails_Current.DrugClassID = item.DrugClassID;
                    }
                    else
                    {
                        ObjRefGenMedProductDetails_Current.SelectedDrugClass = null;
                        ObjRefGenMedProductDetails_Current.DrugClassID = 0;
                    }
                }
                else
                {
                    ObjRefGenMedProductDetails_Current.SelectedDrugClass = null;
                    ObjRefGenMedProductDetails_Current.DrugClassID = 0;
                }
            }
        }



        AutoCompleteBox auRouteOfAdministration;

        public void auRouteOfAdministration_Loaded(object sender, RoutedEventArgs e)
        {
            auRouteOfAdministration = sender as AutoCompleteBox;
        }

        public void auRouteOfAdministration_Populating(object sender, PopulatingEventArgs e)
        {
            if (AllRouteOfAdministration == null || AllRouteOfAdministration.Count <= 0)
            {
                return;
            }
            //20180713 TBL: Them kiem tra e.Parameter neu khong se bao loi 
            if (e.Parameter == null)
            {
                return;
            }

            RouteOfAdministrationSource = new ObservableCollection<Lookup>();
            foreach (var x in AllRouteOfAdministration)
            {
                string str = VNConvertString.ConvertString(x.ObjectValue);
                str = str.ToLower();
                bool bres = str.Contains(VNConvertString.ConvertString(e.Parameter).ToLower());
                if (bres)
                {
                    RouteOfAdministrationSource.Add(x);
                }
            }
            //RouteOfAdministrationSource = AllRouteOfAdministration.Where(x => (VNConvertString.ConvertString(x.ObjectValue).ToLower().Contains(VNConvertString.ConvertString(e.Parameter).ToLower()))).ToObservableCollection();

            auRouteOfAdministration.ItemsSource = RouteOfAdministrationSource;
            auRouteOfAdministration.PopulateComplete();
        }

        //KMx: Sử dụng event SelectionChanged thay vì DropDownClosed. Lý do: DropDownClosed luôn luôn gọi 2 lần mà không biết lý do (31/05/2016 15:43).
        public void auRouteOfAdministration_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (ObjRefGenMedProductDetails_Current == null)
            {
                return;
            }

            if (auRouteOfAdministration != null && auRouteOfAdministration.SelectedItem != null)
            {
                Lookup item = auRouteOfAdministration.SelectedItem as Lookup;
                if (item != null && item.LookupID > 0)
                {
                    ObjRefGenMedProductDetails_Current.RouteOfAdministration = item;
                }
                else
                {
                    ObjRefGenMedProductDetails_Current.RouteOfAdministration = null;
                }
            }
            else
            {
                ObjRefGenMedProductDetails_Current.RouteOfAdministration = null;
            }
        }


        public void Supplier_Click(object sender, RoutedEventArgs e)
        {
            void onInitDlg(ISupplierProduct proAlloc)
            {
                proAlloc.IsChildWindow = true;
            }
            GlobalsNAV.ShowDialog<ISupplierProduct>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        }

        #region IHandle<DrugDeptCloseSearchSupplierEvent> Members

        public void Handle(DrugDeptCloseSearchSupplierEvent message)
        {
            if (message != null && this.IsActive)
            {
                CurrentSupplierGenMedProduct.SelectedSupplier = message.SelectedSupplier as DrugDeptSupplier;
            }
        }

        #endregion

        ValidationSummary ValidationSummary1 = null;
        public void ValidationSummary1_Loaded(object sender, RoutedEventArgs e)
        {
            ValidationSummary1 = sender as ValidationSummary;

        }

        private EntitiesEdit<RefMedContraIndicationTypes> _RefMedicalConditionType_Edit;
        public EntitiesEdit<RefMedContraIndicationTypes> RefMedicalConditionType_Edit
        {
            get
            {
                return _RefMedicalConditionType_Edit;
            }
            set
            {
                if (_RefMedicalConditionType_Edit == value)
                    return;
                _RefMedicalConditionType_Edit = value;
                NotifyOfPropertyChange(() => RefMedicalConditionType_Edit);
            }
        }

        private ObservableCollection<ContraIndicatorMedProductsRelToMedCond> _allContraIndicatorDrugsRelToMedCond;
        public ObservableCollection<ContraIndicatorMedProductsRelToMedCond> allContraIndicatorDrugsRelToMedCond
        {
            get
            {
                return _allContraIndicatorDrugsRelToMedCond;
            }
            set
            {
                if (_allContraIndicatorDrugsRelToMedCond == value)
                    return;
                _allContraIndicatorDrugsRelToMedCond = value;
                NotifyOfPropertyChange(() => allContraIndicatorDrugsRelToMedCond);
            }
        }

        public void GetContraIndicatorDrugsRelToMedCondList(int MCTypeID, long DrugID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetContraIndicatorMedProductsRelToMedCondList(MCTypeID, DrugID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetContraIndicatorMedProductsRelToMedCondList(asyncResult);
                            if (results != null)
                            {
                                var obsRMCT = new ObservableCollection<RefMedContraIndicationTypes>();
                                foreach (var p in results)
                                {
                                    allContraIndicatorDrugsRelToMedCond.Add(p);
                                    obsRMCT.Add(p.RefMedicalConditionType);
                                    RefMedicalConditionType_Edit = new EntitiesEdit<RefMedContraIndicationTypes>(obsRMCT);
                                }
                                NotifyOfPropertyChange(() => allContraIndicatorDrugsRelToMedCond);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void AddMed()
        {
            Action<IDrugDeptMedCondition> onInitDlg = delegate (IDrugDeptMedCondition MedConditionVM)
            {
                MedConditionVM.RefMedicalConditionType_Edit = RefMedicalConditionType_Edit;
                MedConditionVM.NewDrug = ObjRefGenMedProductDetails_Current;
                this.ActivateItem(MedConditionVM);
            };
            GlobalsNAV.ShowDialog<IDrugDeptMedCondition>(onInitDlg);
        }

        public void Handle(MedDeptContraEvent message)
        {
            if (message != null)
            {
                RefMedicalConditionType_Edit = message.refMedicalConditionType_Edit;
            }
        }

        public void acbDrug_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as AutoCompleteBox).ItemsSource = Suppliers;
        }

        public void Supplier_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox axApThau = sender as AutoCompleteBox;
            if (axApThau != null && Suppliers != null)
            {
                axApThau.ItemsSource = Suppliers.Where(x => StringUtil.RemoveSign4VietnameseString(x.SupplierName).ToUpper().Contains(StringUtil.RemoveSign4VietnameseString(e.Parameter.ToUpper())) || (x.SupplierCode != null && x.SupplierCode.ToUpper().Contains(e.Parameter.ToUpper())));
                axApThau.PopulateComplete();
            }
        }

        public void Supplier_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            ////AutoCompleteBox axApThau = sender as AutoCompleteBox;

            ////if (CurrentDrugDeptPurchaseOrder != null)
            ////{
            ////    if (SelectedSupplierCopy == null || !SelectedSupplierCopy.Equals(CurrentDrugDeptPurchaseOrder.SelectedSupplier))
            ////    {
            ////        long SupplierID = 0;
            ////        try
            ////        {
            ////            SupplierID = CurrentDrugDeptPurchaseOrder.SelectedSupplier.SupplierID;

            ////        }
            ////        catch { SupplierID = 0; }
            ////        DrugDeptPurchaseOrderDetail_GetFirst(CurrentDrugDeptPurchaseOrder.DrugDeptEstimatePoID, SupplierID, PCOID);
            ////        SelectedSupplierCopy = CurrentDrugDeptPurchaseOrder.SelectedSupplier.DeepCopy();
            ////    }
            ////}
        }

        #region Giới hạn số lượng xuất
        public void txtLimitedOutQty_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ObjRefGenMedProductDetails_Current == null)
            {
                return;
            }

            string txt = "";
            txt = (sender as TextBox).Text;
            if (!string.IsNullOrEmpty(txt))
            {
                int result = Globals.ConvertStringToInt(txt);
                ObjRefGenMedProductDetails_Current.LimitedOutQty = result > 0 ? result : 0;
            }
        }


        public void txtRemainWarningLevel1_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ObjRefGenMedProductDetails_Current == null)
            {
                return;
            }

            string txt = "";
            txt = (sender as TextBox).Text;
            if (!string.IsNullOrEmpty(txt))
            {
                int result = Globals.ConvertStringToInt(txt);
                ObjRefGenMedProductDetails_Current.RemainWarningLevel1 = result > 0 ? result : 0;
            }
        }


        public void txtRemainWarningLevel2_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ObjRefGenMedProductDetails_Current == null)
            {
                return;
            }

            string txt = "";
            txt = (sender as TextBox).Text;
            if (!string.IsNullOrEmpty(txt))
            {
                int result = Globals.ConvertStringToInt(txt);
                ObjRefGenMedProductDetails_Current.RemainWarningLevel2 = result > 0 ? result : 0;
            }
        }



        CheckBox chkWatchOutQty;
        public void chkWatchOutQty_Loaded(object sender, RoutedEventArgs e)
        {
            chkWatchOutQty = sender as CheckBox;
        }

        public void chkWatchOutQty_UnCheck(object sender, RoutedEventArgs e)
        {
            if (chkWatchOutQty == null || ObjRefGenMedProductDetails_Current == null || (string.IsNullOrWhiteSpace(ObjRefGenMedProductDetails_Current.LimitedOutQty.ToString()) && string.IsNullOrWhiteSpace(ObjRefGenMedProductDetails_Current.RemainWarningLevel1.ToString()) && string.IsNullOrWhiteSpace(ObjRefGenMedProductDetails_Current.RemainWarningLevel1.ToString())))
            {
                return;
            }

            if (MessageBox.Show(eHCMSResources.A0430_G1_Msg_InfoTuDongXoaSLgGHan_SLgCBao, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                ObjRefGenMedProductDetails_Current.LimitedOutQty = 0;
                ObjRefGenMedProductDetails_Current.RemainWarningLevel1 = 0;
                ObjRefGenMedProductDetails_Current.RemainWarningLevel2 = 0;
            }
            else
            {
                chkWatchOutQty.IsChecked = true;
            }
        }

        #endregion

        /*▼====: #002*/
        public void btnBidName()
        {
            if (MessageBox.Show(eHCMSResources.Z2110_G1_XoaMatHangKhoiDotThau, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                this.ShowBusyIndicator();
                var t = new Thread(() =>
                {
                    using (var mServiceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                    {
                        var contract = mServiceFactory.ServiceInstance;
                        try
                        {
                            contract.BeginRemoveBidDetails(ObjRefGenMedProductDetails_Current.BidID, ObjRefGenMedProductDetails_Current.GenMedProductID, true, Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    if (contract.EndRemoveBidDetails(asyncResult))
                                    {
                                        ObjRefGenMedProductDetails_Current.BidName = null;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                }
                            }), null);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                            this.HideBusyIndicator();
                        }
                    }
                });
                t.Start();
            }
        }
        /*▲====: #002*/
        /*▼====: #004*/
        private ObservableCollection<Lookup> _ProductScopeCollection;
        public ObservableCollection<Lookup> ProductScopeCollection
        {
            get
            {
                return _ProductScopeCollection;
            }
            set
            {
                _ProductScopeCollection = value;
                NotifyOfPropertyChange(() => ProductScopeCollection);
            }
        }
        private ObservableCollection<HITransactionType> _HITransactionTypeCollection;
        public ObservableCollection<HITransactionType> HITransactionTypeCollection
        {
            get
            {
                return _HITransactionTypeCollection;
            }
            set
            {
                if (_HITransactionTypeCollection == value)
                    return;
                _HITransactionTypeCollection = value;
                NotifyOfPropertyChange(() => HITransactionTypeCollection);
            }
        }
        private void LoadHITransactions()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginHITransactionType_GetListNoParentID(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                HITransactionTypeCollection = new ObservableCollection<HITransactionType>(contract.EndHITransactionType_GetListNoParentID(asyncResult));
                                if (HITransactionTypeCollection != null && V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                                    HITransactionTypeCollection = HITransactionTypeCollection.Where(x => x.IsShowOnDrugConfig).ToObservableCollection();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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
                    MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        /*▲====: #004*/
        AutoCompleteBox Auto_RefGeneric;
        public void Auto_RefGeneric_Loaded(object sender, RoutedEventArgs e)
        {
            Auto_RefGeneric = sender as AutoCompleteBox;
        }
        public void RefGeneric_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (ObjRefGenMedProductDetails_Current != null)
            {
                if (Auto_RefGeneric != null && Auto_RefGeneric.SelectedItem != null)
                {
                    DrugClass item = Auto_RefGeneric.SelectedItem as DrugClass;
                    if (item != null && item.DrugClassID > 0)
                    {
                        ObjRefGenMedProductDetails_Current.SelectedGeneric = item;
                        ObjRefGenMedProductDetails_Current.GenericID = item.DrugClassID;
                    }
                    else
                    {
                        ObjRefGenMedProductDetails_Current.SelectedGeneric = null;
                        ObjRefGenMedProductDetails_Current.GenericID = 0;
                    }
                }
                else
                {
                    ObjRefGenMedProductDetails_Current.SelectedGeneric = null;
                    ObjRefGenMedProductDetails_Current.GenericID = 0;
                }
            }
        }
        public void RefGeneric_Populating(object sender, PopulatingEventArgs e)
        {
            if (AllRefGeneric == null || AllRefGeneric.Count <= 0)
            {
                return;
            }
            //20180713 TBL: Them kiem tra e.Parameter neu khong se bao loi 
            if (e.Parameter == null)
            {
                return;
            }
            RefGeneric = new ObservableCollection<DrugClass>();
            foreach (var item in AllRefGeneric)
            {
                string str = VNConvertString.ConvertString(item.FaName);
                str = str.ToLower();
                bool bres = str.Contains(VNConvertString.ConvertString(e.Parameter).ToLower());
                if (bres)
                {
                    RefGeneric.Add(item);
                }
            }
            Auto_RefGeneric.ItemsSource = RefGeneric;
            Auto_RefGeneric.PopulateComplete();
        }
        private ObservableCollection<DrugClass> _AllRefGeneric;
        public ObservableCollection<DrugClass> AllRefGeneric
        {
            get
            {
                return _AllRefGeneric;
            }
            set
            {
                if (_AllRefGeneric != value)
                {
                    _AllRefGeneric = value;
                    NotifyOfPropertyChange(() => AllRefGeneric);
                }
            }
        }
        private ObservableCollection<DrugClass> _RefGeneric;
        public ObservableCollection<DrugClass> RefGeneric
        {
            get
            {
                return _RefGeneric;
            }
            set
            {
                if (_RefGeneric != value)
                {
                    _RefGeneric = value;
                    NotifyOfPropertyChange(() => RefGeneric);
                }
            }
        }
        public void GetAllRefGeneric()
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllRefGeneric(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetAllRefGeneric(asyncResult);
                            if (results != null)
                            {
                                if (AllRefGeneric == null)
                                {
                                    AllRefGeneric = new ObservableCollection<DrugClass>();
                                }
                                else
                                {
                                    AllRefGeneric.Clear();
                                }
                                AllRefGeneric = results.ToObservableCollection();
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            this.DlgHideBusyIndicator();
                        }
                    }), null);
                }
            });
            t.Start();
        }
    }
}

//private IEnumerator<IResult> DoGetSupplierList()
//{
//    var paymentTypeTask = new LoadDrugDeptSupplierListTask((int)AllLookupValues.SupplierType.CUNG_CAP_THIET_BI_Y_TE, false, false);
//    yield return paymentTypeTask;
//    Suppliers = paymentTypeTask.DrugDeptSupplierList;
//    yield break;
//}

////private IEnumerator<IResult> DoGetHospitalList()
////{
////    var paymentTypeTask = new LoadHospitalListTask(CityProvinceID, true, false);
////    yield return paymentTypeTask;
////    Hospitals = paymentTypeTask.HospitalList;
////    yield break;
////}

//private IEnumerator<IResult> DoGetRefGenericDrugCategory_1List()
//{
//    var paymentTypeTask = new LoadRefGenericDrugCategory_1ListTask(V_MedProductType, false, false);
//    yield return paymentTypeTask;
//    RefGenericDrugCategory_1s = paymentTypeTask.RefGenericDrugCategory_1List;
//    yield break;
//}

//private IEnumerator<IResult> DoGetRefGenDrugBHYT_CategoryListList()
//{
//    var paymentTypeTask = new LoadRefGenDrugBHYT_CategoryListTask(null, false, true);
//    yield return paymentTypeTask;
//    RefGenDrugBHYT_Categorys = paymentTypeTask.RefGenDrugBHYT_CategoryList;
//    yield break;
//}

//private IEnumerator<IResult> DoGetUnitList()
//{
//    var paymentTypeTask = new LoadDrugDeptUnitListTask(false, false);
//    yield return paymentTypeTask;
//    Units = paymentTypeTask.RefUnitList;
//    yield break;
//}

//private IEnumerator<IResult> DoGetDrugClassList()
//{
//    var paymentTypeTask = new LoadDrugDeptClassListTask(V_MedProductType, false, false);
//    yield return paymentTypeTask;
//    FamilyTherapies = paymentTypeTask.DrugClassList;
//    yield break;
//}

//private IEnumerator<IResult> DoCountryListList()
//{
//    var paymentTypeTask = new LoadCountryListTask(false, false);
//    yield return paymentTypeTask;
//    Countries = paymentTypeTask.RefCountryList;
//    yield break;
//}

//private IEnumerator<IResult> DoPharmaceuticalCompanyListList()
//{
//    var paymentTypeTask = new LoadDrugDeptPharmaceuticalCompanyListTask(false, false);
//    yield return paymentTypeTask;
//    PharmaceuticalCompanies = paymentTypeTask.DrugDeptPharmaceuticalCompanyList;
//    yield break;
//}