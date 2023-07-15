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
using aEMR.Common.Utilities;
using System.Collections.Generic;
using System.Linq;
using aEMR.Controls;
using eHCMSLanguage;
using aEMR.Common.BaseModel;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;

/*
 * 20170810 #002 CMN: Added Bid Service
 * 20180109 #003 CMN: Added more properties for HI Informations.
 * 20181002 #004 TBL: BM 0000067. Rang buoc truong duong dung.
 * 20181006 #005 TTM:   Thay đổi cách lấy giá trị cột từ e.Column.GetValue(FrameworkElement.NameProperty).ToString()
 *                      => e.Column.Equals("Tên Grid".GetColumnByName("Tên cột") vì cách cũ bị sai => Lấy giá trị cột không đc. 
 * 20191009 #006 TBL: Task #896: Thay đổi code để khi thay đổi đơn giá lẻ thì đơn giá DG cũng thay đổi và ngược lại
 * 20191012 #007 TBL: Trường hợp khi cập nhật danh mục mà xóa giá trị của DispenseVolume thì set về = 0 để khi lưu có thể kiểm tra được
 */
namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IRefGenDrug_Add_V2New)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RefGenDrug_Add_V2NewViewModel : ViewModelBase, IRefGenDrug_Add_V2New
        , IHandle<PharContraEvent>
        , IHandle<PharmacyCloseSearchSupplierEvent>
    {
        private bool _IsAddFinishClosed = false;
        public bool IsAddFinishClosed
        {
            get
            {
                return _IsAddFinishClosed;
            }
            set
            {
                if (_IsAddFinishClosed != value)
                {
                    _IsAddFinishClosed = value;
                    NotifyOfPropertyChange(() => IsAddFinishClosed);
                }
            }
        }

        private bool _IsAdd = true;
        public bool IsAdd
        {
            get { return _IsAdd; }
            set
            {
                if (_IsAdd != value)
                {
                    _IsAdd = value;
                    NotifyOfPropertyChange(() => IsAdd);
                }
            }
        }

        private bool _IsEdit;
        public bool IsEdit
        {
            get { return _IsEdit; }
            set
            {
                if (_IsEdit != value)
                {
                    _IsEdit = value;
                    NotifyOfPropertyChange(() => IsEdit);
                }
            }
        }

        public string TitleForm { get; set; }
        /*▼====: #002*/
        public void btnBidName()
        {
            if (MessageBox.Show(eHCMSResources.Z2110_G1_XoaMatHangKhoiDotThau, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
                var t = new Thread(() =>
                {
                    try
                    {
                        using (var mServiceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                        {
                            var contract = mServiceFactory.ServiceInstance;
                            contract.BeginRemoveBidDetails_New(NewDrug.BidID, NewDrug.DrugID, false, Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    if (contract.EndRemoveBidDetails_New(asyncResult))
                                    {
                                        NewDrug.BidName = null;
                                    }
                                }
                                catch (Exception ex)
                                {
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
        }

        /*▲====: #002*/
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public RefGenDrug_Add_V2NewViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            _SelectedNewRefMedicalCondition = new RefMedContraIndicationTypes();
            _allRefMedicalCondition = new ObservableCollection<RefMedContraIndicationTypes>();
            allContraIndicatorDrugsRelToMedCond = new ObservableCollection<ContraIndicatorDrugsRelToMedCond>();
            RefMedicalConditionType_Edit = new EntitiesEdit<RefMedContraIndicationTypes>();

            InitNewDrug();

            eventArg.Subscribe(this);
            Hospitals = new ObservableCollection<Hospital>();

            GroupTypeForReport20 = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_GroupTypeForReport20).ToObservableCollection();
            VENType = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_VENType).ToObservableCollection();
            AllRouteOfAdministration = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_RouteOfAdministration).ToObservableCollection();

            LoadRefPharmacyDrugCategory();

            RefGenMedProductDetailss = new PagedSortableCollectionView<RefGenMedProductSimple>();
            RefGenMedProductDetailss.OnRefresh += RefGenMedProductDetailss_OnRefresh;
            RefGenMedProductDetailss.PageSize = Globals.PageSize;

            /*▼====: #003*/
            ProductScopeCollection = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_ProductScope).ToObservableCollection();
            LoadHITransactions();
            /*▲====: #003*/
            GetAllRefGeneric();
            _logger = container.Resolve<ILogger>();
        }

        void RefGenMedProductDetailss_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchRefDrugGenericDetails_AutoPaging(null, BrandName, RefGenMedProductDetailss.PageIndex, RefGenMedProductDetailss.PageSize);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            if (NewDrug != null
                && NewDrug.DrugID > 0)
            {
                GetContraIndicatorDrugsRelToMedCondList(0, NewDrug.DrugID);
                SupplierGenericDrug_LoadDrugIDNotPaging(NewDrug.DrugID);
            }
        }

        #region Properties Member

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

        private SupplierGenericDrug _CurrentSupplierGenericDrug;
        public SupplierGenericDrug CurrentSupplierGenericDrug
        {
            get
            {
                return _CurrentSupplierGenericDrug;
            }
            set
            {
                if (_CurrentSupplierGenericDrug != value)
                {
                    _CurrentSupplierGenericDrug = value;
                    NotifyOfPropertyChange(() => CurrentSupplierGenericDrug);
                }
            }
        }

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

        private ObservableCollection<RefPharmacyDrugCategory> _RefPharmacyDrugCategorySource;
        public ObservableCollection<RefPharmacyDrugCategory> RefPharmacyDrugCategorySource
        {
            get
            {
                return _RefPharmacyDrugCategorySource;
            }
            set
            {
                if (_RefPharmacyDrugCategorySource != value)
                {
                    _RefPharmacyDrugCategorySource = value;
                    NotifyOfPropertyChange(() => RefPharmacyDrugCategorySource);
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

        private ObservableCollection<DrugClass> _Familytherapies;
        public ObservableCollection<DrugClass> FamilyTherapies
        {
            get
            {
                return _Familytherapies;
            }
            set
            {
                if (_Familytherapies != value)
                {
                    _Familytherapies = value;
                    NotifyOfPropertyChange(() => FamilyTherapies);
                }
            }
        }

        private ObservableCollection<PharmaceuticalCompany> _AllPharmaceuticalCompanies;
        public ObservableCollection<PharmaceuticalCompany> AllPharmaceuticalCompanies
        {
            get { return _AllPharmaceuticalCompanies; }
            set
            {
                if (_AllPharmaceuticalCompanies != value)
                    _AllPharmaceuticalCompanies = value;
                NotifyOfPropertyChange(() => AllPharmaceuticalCompanies);
            }
        }

        private ObservableCollection<PharmaceuticalCompany> _PharmaceuticalCompanies;
        public ObservableCollection<PharmaceuticalCompany> PharmaceuticalCompanies
        {
            get { return _PharmaceuticalCompanies; }
            set
            {
                if (_PharmaceuticalCompanies != value)
                    _PharmaceuticalCompanies = value;
                NotifyOfPropertyChange(() => PharmaceuticalCompanies);
            }
        }

        private RefGenericDrugDetail _NewDrug;
        public RefGenericDrugDetail NewDrug
        {
            get
            {
                return _NewDrug;
            }
            set
            {
                if (_NewDrug != value)
                {
                    _NewDrug = value;
                    NotifyOfPropertyChange(() => NewDrug);
                }
            }
        }

        private ObservableCollection<ContraIndicatorDrugsRelToMedCond> _allContraIndicatorDrugsRelToMedCond;
        public ObservableCollection<ContraIndicatorDrugsRelToMedCond> allContraIndicatorDrugsRelToMedCond
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

        private ContraIndicatorDrugsRelToMedCond _SelectedContraIndicatorDrugsRelToMedCond;
        public ContraIndicatorDrugsRelToMedCond SelectedContraIndicatorDrugsRelToMedCond
        {
            get
            {
                return _SelectedContraIndicatorDrugsRelToMedCond;
            }
            set
            {
                if (_SelectedContraIndicatorDrugsRelToMedCond == value)
                    return;
                _SelectedContraIndicatorDrugsRelToMedCond = value;
                NotifyOfPropertyChange(() => SelectedContraIndicatorDrugsRelToMedCond);
            }
        }

        private ObservableCollection<RefMedContraIndicationTypes> _allRefMedicalCondition;
        public ObservableCollection<RefMedContraIndicationTypes> allRefMedicalCondition
        {
            get
            {
                return _allRefMedicalCondition;
            }
            set
            {
                if (_allRefMedicalCondition != value)
                {
                    _allRefMedicalCondition = value;
                    NotifyOfPropertyChange(() => allRefMedicalCondition);
                }
            }
        }

        private ObservableCollection<string> _allContrainName;
        public ObservableCollection<string> allContrainName
        {
            get
            {
                return _allContrainName;
            }
            set
            {
                if (_allContrainName == value)
                    return;
                _allContrainName = value;
            }
        }

        private RefMedContraIndicationTypes _SelectedNewRefMedicalCondition;
        public RefMedContraIndicationTypes SelectedNewRefMedicalCondition
        {
            get
            {
                return _SelectedNewRefMedicalCondition;
            }
            set
            {
                if (_SelectedNewRefMedicalCondition == value)
                    return;
                _SelectedNewRefMedicalCondition = value;
                NotifyOfPropertyChange(() => SelectedNewRefMedicalCondition);
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

        private ObservableCollection<Lookup> _VENType;
        public ObservableCollection<Lookup> VENType
        {
            get { return _VENType; }
            set
            {
                if (_VENType != value)
                    _VENType = value;
                NotifyOfPropertyChange(() => VENType);
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

        private long DrugID = 0;
        #endregion

        private void InitNewDrug()
        {
            NewDrug = null;
            NewDrug = new RefGenericDrugDetail
            {
                UnitPackaging = 1,
                NumberOfEstimatedMonths_F = 1,
                AdvTimeBeforeExpire = 0,
                TLThanhToan = 1
            };
            SetEmptyString();
            AddBlankRow();
        }
        //▼===== 20191213 TTM:  Vì không biết lý do gì nếu như click vào Nút làm mới thì clear đc text trong các trường của AutoComplete nhưng khi tạo mới thuốc thành công gọi InitNewDrug thì không clear
        //                      được text trong các biến AutoComplete. Nên phải tạo ra hàm SetEmptyString để clear dữ liệu đi nếu không clear sẽ khiến người sử dụng lầm tưởng có thể sử dụng được 
        //                      thực tế là các ID đã mất hết chỉ còn text.
        private void SetEmptyString()
        {
            if(auCountry != null && !string.IsNullOrEmpty(auCountry.Text))
            {
                auCountry.Text = "";
            }

            if (auDrug != null && !string.IsNullOrEmpty(auDrug.Text))
            {
                auDrug.Text = "";
            }

            if (auNhomThuoc != null && !string.IsNullOrEmpty(auNhomThuoc.Text))
            {
                auNhomThuoc.Text = "";
            }

            if (auRouteOfAdministration != null && !string.IsNullOrEmpty(auRouteOfAdministration.Text))
            {
                auRouteOfAdministration.Text = "";
            }

            if (Auto_RefGeneric != null && !string.IsNullOrEmpty(Auto_RefGeneric.Text))
            {
                Auto_RefGeneric.Text = "";
            }

            if (auHangSX != null && !string.IsNullOrEmpty(auHangSX.Text))
            {
                auHangSX.Text = "";
            }
        }
        //▲===== 
        private void LoadRefPharmacyDrugCategory()
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginLoadRefPharmacyDrugCategory_New(
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<RefPharmacyDrugCategory> allItems = new ObservableCollection<RefPharmacyDrugCategory>();
                                try
                                {
                                    allItems = contract.EndLoadRefPharmacyDrugCategory_New(asyncResult);
                                    RefPharmacyDrugCategorySource = allItems.ToObservableCollection();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, "Error!(9)", MessageBoxButton.OK);
                                }

                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error!(10)", MessageBoxButton.OK);
                }
            });

            t.Start();
        }

        /*==== #001 ====*/
        private bool CheckDrugContent(RefGenericDrugDetail aDrug)
        {
            if (aDrug.InsuranceCover == true)
            {
                string error = "";
                if (string.IsNullOrEmpty(aDrug.HIDrugCode))
                {
                    error += eHCMSResources.Z2838_G1_ChuaNhapHICode + Environment.NewLine;
                }
                if (string.IsNullOrEmpty(aDrug.ReportBrandName))
                {
                    error += eHCMSResources.Z2839_G1_ChuaNhapTenThanhPham + Environment.NewLine;
                }
                if (string.IsNullOrEmpty(aDrug.Visa))
                {
                    error += eHCMSResources.Z2840_G1_ChuaNhapSDK + Environment.NewLine;
                }
                if (aDrug.V_ProductScope != (long)AllLookupValues.V_ProductScope.InHIScope)
                {
                    error += eHCMSResources.Z2194_G1_PhamViBHKhongHopLe + Environment.NewLine;
                }
                if (!string.IsNullOrEmpty(error))
                {
                    MessageBox.Show(error, eHCMSResources.G0442_G1_TBao);
                    return false;
                }
            }
            return true;
        }
        /*==== #001 ====*/
        private void AddNewDrug()
        {
            /*==== #001 ====*/
            if (!CheckDrugContent(NewDrug))
                return;
            /*==== #001 ====*/
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginAddNewDrug_New(NewDrug, RefMedicalConditionType_Edit.TempObject, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndAddNewDrug_New(out DrugID, asyncResult);
                                if (DrugID > 0)
                                {
                                    NewDrug.DrugID = DrugID;
                                    InsertDeleteUpdateConIndicatorDrugsRelToMedCondXML(createListNew(), createListDelete(), null);
                                    if (MessageBox.Show(eHCMSResources.Z1487_G1_CoMuonThem1ThuocKhac, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                    {
                                        InitNewDrug();
                                        RefMedicalConditionType_Edit.Clear();
                                    }
                                    else
                                    {
                                        NewDrug.DrugID = DrugID;
                                        Globals.EventAggregator.Publish(new PharmacyCloseFinishAddGenDrugEvent { SupplierDrug = NewDrug });
                                        if (IsAddFinishClosed)
                                        {
                                            TryClose();
                                            //add xong dog lai lien va chon thuoc moi nay de lam viec luon
                                            Globals.EventAggregator.Publish(new PharmacyCloseFinishAddGenDrugEvent { SupplierDrug = NewDrug });
                                        }
                                        else
                                        {
                                            TryClose();
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.K0053_G1_ThuocDaTonTai);
                                }
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
                    this.DlgHideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    _logger.Info(ex.Message);
                }
            });

            t.Start();
        }

        private void UpdateDrug()
        {
            if (NewDrug.V_CatDrugType == (long)AllLookupValues.V_CatDrugType.Shared) //Mã số của thuốc dùng chung
            {
                MessageBox.Show(eHCMSResources.Z2351_G1_NganCapNhatDungChung);
                return;
            }
            /*==== #001 ====*/
            if (!CheckDrugContent(NewDrug))
                return;
            /*==== #001 ====*/
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdateDrug_New(NewDrug, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndUpdateDrug_New(asyncResult);
                                //phat ra su kien de form cha bat dc va load lai du lieu
                                Globals.EventAggregator.Publish(new PharmacyCloseEditGenDrugEvent { });
                                InsertDeleteUpdateConIndicatorDrugsRelToMedCondXML(createListNew(), createListDelete(), null);
                                //Đóng Popup
                                MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK);
                                //TryClose();
                                GetContraIndicatorDrugsRelToMedCondList(0, NewDrug.DrugID);
                            }
                            catch (Exception ex)
                            {
                                //Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
                    this.DlgHideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    _logger.Info(ex.Message);
                }
            });

            t.Start();
        }

        private void GetContraIndicatorDrugsRelToMedCondList(int MCTypeID, long DrugID)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetContraIndicatorDrugsRelToMedCondList_New(MCTypeID, DrugID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetContraIndicatorDrugsRelToMedCondList_New(asyncResult);
                                if (results != null)
                                {
                                    var obsRMCT = new ObservableCollection<RefMedContraIndicationTypes>();
                                    foreach (var p in results)
                                    {
                                        allContraIndicatorDrugsRelToMedCond.Add(p);
                                        p.RefMedicalConditionType.IsWarning = p.IsWarning;
                                        obsRMCT.Add(p.RefMedicalConditionType);
                                        //RefMedicalConditionType_Edit.CurObject = allContraIndicatorDrugsRelToMedCond;
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

        private void SupplierGenericDrug_LoadDrugIDNotPaging(long DrugID)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSupplierGenericDrug_LoadDrugIDNotPaging_New(DrugID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndSupplierGenericDrug_LoadDrugIDNotPaging_New(asyncResult);
                                NewDrug.SupplierGenericDrugs = results.ToObservableCollection();
                                if (NewDrug.SupplierGenericDrugs == null)
                                {
                                    NewDrug.SupplierGenericDrugs = new ObservableCollection<SupplierGenericDrug>();
                                }
                                AddBlankRow();
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

        private void InsertConIndicatorDrugsRelToMedCond(ObservableCollection<RefMedContraIndicationTypes> lstRefMedicalCondition, long DrugID)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginInsertConIndicatorDrugsRelToMedCond_New(lstRefMedicalCondition, DrugID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool results = contract.EndInsertConIndicatorDrugsRelToMedCond_New(asyncResult);
                                if (results)
                                {
                                    allRefMedicalCondition.Clear();
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

        private void InsertDeleteUpdateConIndicatorDrugsRelToMedCondXML(IList<ContraIndicatorDrugsRelToMedCond> lstInsert
                                                    , IList<ContraIndicatorDrugsRelToMedCond> lstDelete
                                                    , IList<ContraIndicatorDrugsRelToMedCond> lstUpdate)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginInsertDeleteUpdateConIndicatorDrugsRelToMedCondXML_New(lstInsert, lstDelete
                            , lstUpdate, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool results = contract.EndInsertDeleteUpdateConIndicatorDrugsRelToMedCondXML_New(asyncResult);
                                if (results)
                                {
                                    //MessageBox.Show("Cập nhật chống chỉ định cho thuốc thành công!");
                                    //RefMedicalConditionType_Edit.Clear();
                                    //TryClose();
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

        private bool CheckData()
        {
            NewDrug.StaffID = Globals.LoggedUserAccount.StaffID.GetValueOrDefault();
            if (NewDrug.SupplierGenericDrugs == null || NewDrug.SupplierGenericDrugs.Count == 0)
            {
                MessageBox.Show(eHCMSResources.A0195_G1_Msg_InfoChonNCCChoThuoc);
                return false;
            }
            else
            {
                int icount = 0;
                for (int i = 0; i < NewDrug.SupplierGenericDrugs.Count; i++)
                {
                    if (NewDrug.SupplierGenericDrugs[i].SupplierID > 0)
                    {
                        if (NewDrug.SupplierGenericDrugs[i].UnitPrice <= 0 || NewDrug.SupplierGenericDrugs[i].PackagePrice <= 0)
                        {
                            MessageBox.Show(eHCMSResources.A0519_G1_Msg_InfoDGiaLonHon0);
                            return false;
                        }
                        if (NewDrug.SupplierGenericDrugs[i].VAT < 1)
                        {
                            MessageBox.Show(eHCMSResources.K0265_G1_VATKhongHopLe3);
                            return false;
                        }
                    }
                    if (NewDrug.SupplierGenericDrugs[i].SupplierID > 0 && NewDrug.SupplierGenericDrugs[i].IsMain == true)
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
                if (NewDrug.TLThanhToan < 0 || NewDrug.TLThanhToan > 100)
                {
                    MessageBox.Show("Tỷ lệ thanh toán không nhỏ hơn 0% và không lớn hơn 100%!");
                    return false;
                }
            }
            //▼===== 20200312 TTM: BM 0025010: Kiểm tra thông tin VAT cho danh mục
            if (NewDrug.VAT < 0 || NewDrug.VAT > 1)
            {
                MessageBox.Show(eHCMSResources.Z2991_G1_VATKhongHopLe);
                return false;
            }
            //▲===== 
            return true;
        }

        public void OKButton(object sender, RoutedEventArgs e)
        {
            if (CheckValid(NewDrug))
            {
                if (CheckData())
                {
                    if (NewDrug.PharmaceuticalCompany != null)
                    {
                        NewDrug.PCOID = NewDrug.PharmaceuticalCompany.PCOID;
                    }
                    AddNewDrug();
                }
            }
        }

        public ObservableCollection<ContraIndicatorDrugsRelToMedCond> createListNew()
        {
            var lstNew = new ObservableCollection<ContraIndicatorDrugsRelToMedCond>();
            foreach (var item in RefMedicalConditionType_Edit.NewObject)
            {
                ContraIndicatorDrugsRelToMedCond p = new ContraIndicatorDrugsRelToMedCond
                {
                    RefGenericDrugDetail = NewDrug,
                    RefMedicalConditionType = item,
                    IsWarning = item.IsWarning
                };
                lstNew.Add(p);
            }
            return lstNew;
        }

        public ObservableCollection<ContraIndicatorDrugsRelToMedCond> createListDelete()
        {
            var lstNew = new ObservableCollection<ContraIndicatorDrugsRelToMedCond>();
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

        public ObservableCollection<ContraIndicatorDrugsRelToMedCond> createListUpdate()
        {
            var lstNew = new ObservableCollection<ContraIndicatorDrugsRelToMedCond>();
            return lstNew;
        }

        public void EditButton(object sender, RoutedEventArgs e)
        {
            if (CheckValid(NewDrug))
            {
                if (CheckData())
                {
                    if (NewDrug.PharmaceuticalCompany != null)
                    {
                        NewDrug.PCOID = NewDrug.PharmaceuticalCompany.PCOID;
                    }
                    UpdateDrug();
                }
            }
        }

        public void Refesh(object sender, RoutedEventArgs e)
        {
            InitNewDrug();
        }

        public void CancelButton(object sender, RoutedEventArgs e)
        {
            TryClose();
            if (!IsAddFinishClosed)
            {
                //phat ra su kien de form cha bat dc va load lai du lieu
                Globals.EventAggregator.Publish(new PharmacyCloseAddGenDrugEvent { });
            }
        }

        ValidationSummary ValidationSummary1 = null;
        public void ValidationSummary1_Loaded(object sender, RoutedEventArgs e)
        {
            ValidationSummary1 = sender as ValidationSummary;

        }

        private bool CheckValid(object temp)
        {
            RefGenericDrugDetail u = temp as RefGenericDrugDetail;
            if (u == null)
            {
                return false;
            }
            if (ValidationSummary1 != null && ValidationSummary1.HasDisplayedErrors)
            {
                return false;
            }
            string error = "";
            if (u.DrugCode == null || u.DrugCode == "")
            {
                error += eHCMSResources.Z2431_G1_ChuaNhapMaThuoc + Environment.NewLine;
            }
            if (string.IsNullOrEmpty(u.BrandName))
            {
                error += eHCMSResources.Z2833_G1_ChuaNhapTenThuongMai + Environment.NewLine;
            }
            if (u.SelectedGeneric == null && u.RefPharmacyDrugCatID != 7)
            {
                error += eHCMSResources.Z2434_G1_ChuaNhapHoatChat + Environment.NewLine;
            }
            if (u.DispenseVolume <= 0)
            {
                error += eHCMSResources.Z0856_G1_DispenseVolume + Environment.NewLine;
            }
            if (u.SeletedUnit == null || u.SeletedUnit.UnitID <= 0)
            {
                error += eHCMSResources.Z2834_G1_ChuaChonDVT + Environment.NewLine;
            }
            if (u.SeletedUnitUse == null || u.SeletedUnitUse.UnitID <= 0)
            {
                error += eHCMSResources.Z2835_G1_ChuaChonDVD + Environment.NewLine;
            }
            /*▼====: #004*/
            if (u.RouteOfAdministration == null || u.RouteOfAdministration.LookupID <= 0)
            {
                error += eHCMSResources.Z2301_G1_BanChuaNhapDuongDung + Environment.NewLine;
            }
            /*▲====: #004*/
            if (string.IsNullOrEmpty(u.Administration))
            {
                error += eHCMSResources.Z2836_G1_ChuaNhapCachDung + Environment.NewLine;
            }
            if (string.IsNullOrEmpty(u.Packaging))
            {
                error += eHCMSResources.Z2837_G1_ChuaNhapQCDongGoi + Environment.NewLine;
            }

            if (u.PCOID == null || u.PCOID <= 0)
            {
                error += eHCMSResources.Z0854_G1_ChuaChonNSX + Environment.NewLine;
            }
            if (u.CountryID == null || u.CountryID <= 0)
            {
                error += eHCMSResources.Z0853_G1_ChuaChonQuocGia + Environment.NewLine;
            }
            if (u.DrugClassID == null || u.DrugClassID <= 0)
            {
                error += eHCMSResources.Z0855_G1_ChuaChonNhomThuoc + Environment.NewLine;
            }
            //if (u.DispenseVolume < 1)
            //{
            //    error += "DispenseVolume >=1. " + Environment.NewLine;
            //}
            if (u.MonitorOutQty)
            {
                //KMx: Phải thỏa điều kiện: LimitedOutQty >= RemainWarningLevel2 >= RemainWarningLevel1 (13/11/2014 16:32).
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
            if(!u.IsNotVat && u.VAT == null)
            {
                error += eHCMSResources.Z2996_G1_VATRong + Environment.NewLine;
            }
            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, eHCMSResources.G0442_G1_TBao);
                return false;
            }

            return u.Validate();
        }

        public void AddMed(object sender, RoutedEventArgs e)
        {
            void onInitDlg(IMedCondition MedConditionVM)
            {
                //if (allContraIndicatorDrugsRelToMedCond != null)
                //{
                //    RefMedicalConditionType_Edit = new EntitiesEdit<RefMedicalConditionType>(allRefMedicalCondition);
                //}
                MedConditionVM.RefMedicalConditionType_Edit = RefMedicalConditionType_Edit;
                MedConditionVM.NewDrug = NewDrug;
                ActivateItem(MedConditionVM);
            }
            GlobalsNAV.ShowDialog<IMedCondition>(onInitDlg);
        }

        public void DeleteContraind(object sender, RoutedEventArgs e)
        {
            allContrainName.Remove(SelectedNewRefMedicalCondition.MedContraIndicationType);
            allRefMedicalCondition.Remove(SelectedNewRefMedicalCondition);
        }

        #region event Handle
        public void Handle(PharContraEvent obj)
        {
            if (obj != null)
            {
                //allRefMedicalCondition = (ObservableCollection<RefMedicalConditionType>)obj.lstPharContra;
                //allContrainName = (ObservableCollection<string>)obj.lstPharContraName;
                RefMedicalConditionType_Edit = obj.refMedicalConditionType_Edit;
            }
        }

        #endregion

        private void AddBlankRow()
        {
            if (NewDrug.SupplierGenericDrugs == null)
            {
                NewDrug.SupplierGenericDrugs = new ObservableCollection<SupplierGenericDrug>();
            }
            SupplierGenericDrug item = new SupplierGenericDrug();
            NewDrug.SupplierGenericDrugs.Add(item);
        }

        public void acbDrug_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as ComboBox).ItemsSource = Suppliers;
        }

        private bool CheckExists(SupplierGenericDrug ite)
        {
            if (ite != null && ite.SelectedSupplier != null)
            {
                ite.SupplierID = ite.SelectedSupplier.SupplierID;
                var value = NewDrug.SupplierGenericDrugs.Where(x => x.SupplierID == ite.SupplierID);
                if (value != null && value.Count() > 1)
                {
                    MessageBox.Show(eHCMSResources.Z0731_G1_NCCDaDuocChon);
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
            //▼====== #006
            if (GridSuppliers != null)
            {
                GridSuppliers.CurrentCellChanged += GridSuppliers_CurrentCellChanged;
            }
            //▲====== #006
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
        //▼====== #006
        private string EditedColumnName { get; set; }
        private SupplierGenericDrug EditedDetailItem { get; set; }
        public void GridSuppliers_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            EditedDetailItem = (e.Row.DataContext as SupplierGenericDrug);
            EditedColumnName = null;
            if (e.Column.Equals(GridSuppliers.GetColumnByName("colSupplierName")))
            {
                if (CheckExists(EditedDetailItem))
                {
                    if (e.Row.GetIndex() == (NewDrug.SupplierGenericDrugs.Count - 1) && e.EditAction == DataGridEditAction.Commit)
                    {
                        AddBlankRow();
                    }
                }
            }
            if (e.Column.Equals(GridSuppliers.GetColumnByName("colUnitPrice")))
            {
                EditedColumnName = "colUnitPrice";
            }
            else if (e.Column.Equals(GridSuppliers.GetColumnByName("colPackagePrice")))
            {
                EditedColumnName = "colPackagePrice";
            }
        }
        private void GridSuppliers_CurrentCellChanged(object sender, EventArgs e)
        {
            if (EditedDetailItem == null || string.IsNullOrEmpty(EditedColumnName))
            {
                return;
            }
            if (EditedColumnName.Equals("colUnitPrice"))
            {
                EditedColumnName = null;
                decimal value = 0;
                Decimal.TryParse(PreparingCellForEdit, out value);
                SupplierGenericDrug item = EditedDetailItem;
                if (value == item.UnitPrice)
                {
                    return;
                }
                item.PackagePrice = item.UnitPrice * NewDrug.UnitPackaging.GetValueOrDefault(1);
            }
            else if (EditedColumnName.Equals("colPackagePrice"))
            {
                EditedColumnName = null;
                decimal value = 0;
                Decimal.TryParse(PreparingCellForEdit, out value);
                SupplierGenericDrug item = EditedDetailItem;
                if (value == item.PackagePrice)
                {
                    return;
                }
                if (NewDrug.UnitPackaging.GetValueOrDefault() > 0)
                {
                    item.UnitPrice = item.PackagePrice / NewDrug.UnitPackaging.GetValueOrDefault(1);
                }
            }
            EditedColumnName = null;
        }
        //public void GridSuppliers_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        //{
        //    SupplierGenericDrug item = e.Row.DataContext as SupplierGenericDrug;
        //    //KMx: Không sử dụng index nữa, chuyển sang dùng column name, vì khi user đổi thứ tự column thì logic sẽ bị sai (22/01/2015 10:58).
        //    //if (e.Column.DisplayIndex == 1)//NCC

        //    //TTM 13072018
        //    //thêm Foreach lấy từng cột ra vì GetValue bị lỗi nên thay bằng cách mới
        //    //if (e.Column.GetValue(FrameworkElement.NameProperty).ToString() == "colSupplierName")

        //    //▼====== #005
        //    //foreach (DataGridColumn col1 in GridSuppliers.Columns)
        //    //{
        //    //    string tmp = DataGridColNamingUtil.GetColName(col1);
        //    if (e.Column.Equals(GridSuppliers.GetColumnByName("colSupplierName")))
        //    //if (String.Compare(tmp, "colSupplierName") == 0)
        //    {
        //        if (CheckExists(item))
        //        {
        //            if (e.Row.GetIndex() == (NewDrug.SupplierGenericDrugs.Count - 1) && e.EditAction == DataGridEditAction.Commit)
        //            {
        //                AddBlankRow();
        //            }
        //        }
        //    }

        //    //if (e.Column.DisplayIndex == 3)//NCC
        //    //TTM 13072018
        //    //if (e.Column.GetValue(FrameworkElement.NameProperty).ToString() == "colUnitPrice")
        //    //else if (String.Compare(tmp, "colUnitPrice") == 0)
        //    else if (e.Column.Equals(GridSuppliers.GetColumnByName("colUnitPrice")))
        //    {
        //        decimal value = 0;
        //        decimal.TryParse(PreparingCellForEdit, out value);

        //        if (value == item.UnitPrice)
        //        {
        //            return;
        //        }
        //        item.PackagePrice = item.UnitPrice * NewDrug.UnitPackaging.GetValueOrDefault(1);
        //    }
        //    //if (e.Column.DisplayIndex == 4)//NCC

        //    //TTM 13072018
        //    //if (e.Column.GetValue(FrameworkElement.NameProperty).ToString() == "colPackagePrice")
        //    //else if (String.Compare(tmp, "colPackagePrice") == 0)
        //    else if (e.Column.Equals(GridSuppliers.GetColumnByName("colPackagePrice")))
        //    {
        //        decimal value = 0;
        //        decimal.TryParse(PreparingCellForEdit, out value);
        //        if (value == item.PackagePrice)
        //        {
        //            return;
        //        }
        //        if (NewDrug.UnitPackaging.GetValueOrDefault() > 0)
        //        {
        //            item.UnitPrice = item.PackagePrice / NewDrug.UnitPackaging.GetValueOrDefault(1);
        //        }
        //    }
        //    //▲====== #005
        //}
        //}
        //▲====== #006
        string txt = "";
        public void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txt != (sender as TextBox).Text)
            {
                txt = (sender as TextBox).Text;
                if (!string.IsNullOrEmpty(txt))
                {
                    if (NewDrug != null && NewDrug.SupplierGenericDrugs != null)
                    {
                        int value = 0;
                        int.TryParse(txt, out value);

                        NewDrug.UnitPackaging = value;
                        foreach (SupplierGenericDrug p in NewDrug.SupplierGenericDrugs)
                        {
                            p.PackagePrice = p.UnitPrice * NewDrug.UnitPackaging.GetValueOrDefault(1);
                        }
                    }
                }
            }
        }

        public void btnDeleteCC_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSupplierGenericDrug == NewDrug.SupplierGenericDrugs[NewDrug.SupplierGenericDrugs.Count - 1])
            {
                MessageBox.Show(eHCMSResources.A0181_G1_Msg_InfoXoaDongTrong);
                return;
            }

            if (MessageBox.Show(eHCMSResources.A0119_G1_Msg_ConfXoaNCC, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (CurrentSupplierGenericDrug != null)
                {
                    NewDrug.SupplierGenericDrugs.Remove(CurrentSupplierGenericDrug);
                }
                if (NewDrug.SupplierGenericDrugs.Count == 0)
                {
                    AddBlankRow();
                }
            }
        }

        AutoCompleteBox auDrug;

        public void AutoDrug_Loaded(object sender, RoutedEventArgs e)
        {
            auDrug = sender as AutoCompleteBox;
        }

        public void AutoDrug_Populating(object sender, PopulatingEventArgs e)
        {
            if (AllRefGenDrugBHYT_Categorys == null || AllRefGenDrugBHYT_Categorys.Count <= 0)
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
            auDrug.ItemsSource = RefGenDrugBHYT_Categorys;
            auDrug.PopulateComplete();
        }

        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (NewDrug != null)
            {
                if (auDrug != null && auDrug.SelectedItem != null)
                {
                    RefGenDrugBHYT_Category item = auDrug.SelectedItem as RefGenDrugBHYT_Category;
                    if (item != null && item.RefGenDrugBHYT_CatID > 0)
                    {
                        NewDrug.CurrentRefGenDrugBHYT_Category = item;
                        //KMx: Khi chọn nhóm BHYT thì không được đổi tên chung, để người dùng tự nhập (Nhi nhà thuốc yêu cầu) (23/07/2014 09:53).
                        //NewDrug.GenericName = item.CategoryName;
                        NewDrug.RefGenDrugBHYT_CatID = item.RefGenDrugBHYT_CatID;
                        NewDrug.ActiveIngredient = item.CategoryName;
                        NewDrug.HIDrugCode = item.DrugOrderNo;
                    }
                    else
                    {
                        NewDrug.CurrentRefGenDrugBHYT_Category = null;
                        //KMx: Khi chọn nhóm BHYT thì không được đổi tên chung, để người dùng tự nhập (Nhi nhà thuốc yêu cầu) (23/07/2014 09:53).
                        //NewDrug.GenericName = "";
                        NewDrug.ActiveIngredient = "";
                        NewDrug.HIDrugCode = "";
                    }
                }
                else
                {
                    NewDrug.CurrentRefGenDrugBHYT_Category = null;
                    //KMx: Khi chọn nhóm BHYT thì không được đổi tên chung, để người dùng tự nhập (Nhi nhà thuốc yêu cầu) (23/07/2014 09:53).
                    //NewDrug.GenericName = "";
                    NewDrug.ActiveIngredient = "";
                    NewDrug.HIDrugCode = "";
                }
            }
        }

        public void ApGiaDau_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            AutoCompleteBox axApThau = sender as AutoCompleteBox;
            if (NewDrug != null)
            {
                if (axApThau != null && axApThau.SelectedItem != null)
                {
                    Hospital item = axApThau.SelectedItem as Hospital;
                    if (item != null && item.HosID > 0)
                    {
                        NewDrug.CurrentHospital = item;
                        NewDrug.HosID = item.HosID;
                    }
                    else
                    {
                        NewDrug.CurrentHospital = null;
                        NewDrug.HosID = null;
                    }
                }
                else
                {
                    NewDrug.CurrentHospital = null;
                    NewDrug.HosID = null;
                }
            }
        }

        AutoCompleteBox auCountry;
        public void Country_Loaded(object sender, RoutedEventArgs e)
        {
            auCountry = sender as AutoCompleteBox;
        }

        public void Country_Populating(object sender, PopulatingEventArgs e)
        {
            if (AllCountries == null || AllCountries.Count <= 0)
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
            auCountry.ItemsSource = Countries;
            auCountry.PopulateComplete();
        }

        public void Country_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (NewDrug != null)
            {
                if (auCountry != null && auCountry.SelectedItem != null)
                {
                    RefCountry item = auCountry.SelectedItem as RefCountry;
                    if (item != null && item.CountryID > 0)
                    {
                        NewDrug.SeletedCountry = item;
                        NewDrug.CountryID = item.CountryID;
                    }
                    else
                    {
                        NewDrug.SeletedCountry = null;
                        NewDrug.CountryID = 0;
                    }
                }
                else
                {
                    NewDrug.SeletedCountry = null;
                    NewDrug.CountryID = 0;
                }
            }
        }

        AutoCompleteBox auHangSX;

        public void HangSX_Loaded(object sender, RoutedEventArgs e)
        {
            auHangSX = sender as AutoCompleteBox;
        }

        public void HangSX_Populating(object sender, PopulatingEventArgs e)
        {
            if (AllPharmaceuticalCompanies == null || AllPharmaceuticalCompanies.Count <= 0)
            {
                return;
            }

            PharmaceuticalCompanies = new ObservableCollection<PharmaceuticalCompany>();
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
            auHangSX.ItemsSource = PharmaceuticalCompanies;
            auHangSX.PopulateComplete();
        }

        public void HangSX_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (NewDrug != null)
            {
                if (auHangSX != null && auHangSX.SelectedItem != null)
                {
                    PharmaceuticalCompany item = auHangSX.SelectedItem as PharmaceuticalCompany;
                    if (item != null && item.PCOID > 0)
                    {
                        NewDrug.PharmaceuticalCompany = item;
                        NewDrug.PCOID = item.PCOID;
                    }
                    else
                    {
                        NewDrug.PharmaceuticalCompany = null;
                        NewDrug.PCOID = 0;
                    }
                }
                else
                {
                    NewDrug.PharmaceuticalCompany = null;
                    NewDrug.PCOID = 0;
                }
            }
        }

        AutoCompleteBox auNhomThuoc;

        public void NhomThuoc_Loaded(object sender, RoutedEventArgs e)
        {
            auNhomThuoc = sender as AutoCompleteBox;
        }

        public void NhomThuoc_Populating(object sender, PopulatingEventArgs e)
        {
            if (AllFamilyTherapies == null || AllFamilyTherapies.Count <= 0)
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
            auNhomThuoc.ItemsSource = FamilyTherapies;
            auNhomThuoc.PopulateComplete();
        }

        public void NhomThuoc_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (NewDrug != null)
            {
                if (auNhomThuoc != null && auNhomThuoc.SelectedItem != null)
                {
                    DrugClass item = auNhomThuoc.SelectedItem as DrugClass;
                    if (item != null && item.DrugClassID > 0)
                    {
                        NewDrug.SeletedDrugClass = item;
                        NewDrug.DrugClassID = item.DrugClassID;
                    }
                    else
                    {
                        NewDrug.SeletedDrugClass = null;
                        NewDrug.DrugClassID = 0;
                    }
                }
                else
                {
                    NewDrug.SeletedDrugClass = null;
                    NewDrug.DrugClassID = 0;
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

            RouteOfAdministrationSource = AllRouteOfAdministration.Where(x => (VNConvertString.ConvertString(x.ObjectValue).ToLower().Contains(VNConvertString.ConvertString(e.Parameter).ToLower()))).ToObservableCollection();

            auRouteOfAdministration.ItemsSource = RouteOfAdministrationSource;
            auRouteOfAdministration.PopulateComplete();
        }

        //KMx: Sử dụng event SelectionChanged thay vì DropDownClosed. Lý do: DropDownClosed luôn luôn gọi 2 lần mà không biết lý do (31/05/2016 15:43).
        public void auRouteOfAdministration_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (NewDrug == null)
            {
                return;
            }

            if (auRouteOfAdministration != null && auRouteOfAdministration.SelectedItem != null)
            {
                Lookup item = auRouteOfAdministration.SelectedItem as Lookup;
                if (item != null && item.LookupID > 0)
                {
                    NewDrug.RouteOfAdministration = item;
                }
                else
                {
                    NewDrug.RouteOfAdministration = null;
                }
            }
            else
            {
                NewDrug.RouteOfAdministration = null;
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

        public void Supplier_Click(object sender, RoutedEventArgs e)
        {
            Action<ISuppliers> onInitDlg = delegate (ISuppliers proAlloc)
            {
                proAlloc.IsChildWindow = true;
            };
            GlobalsNAV.ShowDialog<ISuppliers>(onInitDlg);
        }

        #region IHandle<PharmacyCloseSearchSupplierEvent> Members

        public void Handle(PharmacyCloseSearchSupplierEvent message)
        {
            if (message != null && this.IsActive)
            {
                CurrentSupplierGenericDrug.SelectedSupplier = message.SelectedSupplier as Supplier;
            }
        }

        #endregion

        private int ConvertStringToInt(string strNumber)
        {
            int value = 0;
            int.TryParse(strNumber, out value);
            return value;
        }

        public void txtLimitedOutQty_LostFocus(object sender, RoutedEventArgs e)
        {
            if (NewDrug == null)
            {
                return;
            }

            string txt = "";
            txt = (sender as TextBox).Text;
            if (!string.IsNullOrEmpty(txt))
            {
                int result = ConvertStringToInt(txt);
                NewDrug.LimitedOutQty = result > 0 ? result : 0;
            }
        }

        public void txtRemainWarningLevel1_LostFocus(object sender, RoutedEventArgs e)
        {
            if (NewDrug == null)
            {
                return;
            }

            string txt = "";
            txt = (sender as TextBox).Text;
            if (!string.IsNullOrEmpty(txt))
            {
                int result = ConvertStringToInt(txt);
                NewDrug.RemainWarningLevel1 = result > 0 ? result : 0;
            }
        }

        public void txtRemainWarningLevel2_LostFocus(object sender, RoutedEventArgs e)
        {
            if (NewDrug == null)
            {
                return;
            }

            string txt = "";
            txt = (sender as TextBox).Text;
            if (!string.IsNullOrEmpty(txt))
            {
                int result = ConvertStringToInt(txt);
                NewDrug.RemainWarningLevel2 = result > 0 ? result : 0;
            }
        }

        CheckBox chkMonitorOutQty;
        public void chkMonitorOutQty_Loaded(object sender, RoutedEventArgs e)
        {
            chkMonitorOutQty = sender as CheckBox;
        }

        public void chkMonitorOutQty_UnCheck(object sender, RoutedEventArgs e)
        {
            if (chkMonitorOutQty == null || NewDrug == null || (string.IsNullOrWhiteSpace(NewDrug.LimitedOutQty.ToString()) && string.IsNullOrWhiteSpace(NewDrug.RemainWarningLevel1.ToString()) && string.IsNullOrWhiteSpace(NewDrug.RemainWarningLevel1.ToString())))
            {
                return;
            }

            if (MessageBox.Show(eHCMSResources.A0430_G1_Msg_InfoTuDongXoaSLgGHan_SLgCBao, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                NewDrug.LimitedOutQty = 0;
                NewDrug.RemainWarningLevel1 = 0;
                NewDrug.RemainWarningLevel2 = 0;
            }
            else
            {
                chkMonitorOutQty.IsChecked = true;
            }
        }

        private PagedSortableCollectionView<RefGenMedProductSimple> _RefGenMedProductDetailss;
        public PagedSortableCollectionView<RefGenMedProductSimple> RefGenMedProductDetailss
        {
            get
            {
                return _RefGenMedProductDetailss;
            }
            set
            {
                if (_RefGenMedProductDetailss != value)
                {
                    _RefGenMedProductDetailss = value;
                }
                NotifyOfPropertyChange(() => RefGenMedProductDetailss);
            }
        }

        private void SearchRefDrugGenericDetails_AutoPaging(bool? IsCode, string Name, int PageIndex, int PageSize)
        {
            int totalCount = 0;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginRefGenMedProductDetails_SimpleAutoPaging_New(IsCode, Name, (long)AllLookupValues.MedProductType.THUOC, null, PageSize, PageIndex, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var ListRefGenMedProduct = contract.EndRefGenMedProductDetails_SimpleAutoPaging_New(out totalCount, asyncResult);
                            if (IsCode.GetValueOrDefault())
                            {
                                if (ListRefGenMedProduct != null && ListRefGenMedProduct.Count > 0)
                                {
                                    NewDrug.MatchRefGenMedProduct = ListRefGenMedProduct.FirstOrDefault();
                                }
                                else
                                {
                                    NewDrug.MatchRefGenMedProduct = null;

                                    if (au != null)
                                    {
                                        au.Text = "";
                                    }
                                    MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                                }
                            }
                            else
                            {
                                if (ListRefGenMedProduct != null)
                                {
                                    RefGenMedProductDetailss.Clear();
                                    RefGenMedProductDetailss.TotalItemCount = totalCount;
                                    RefGenMedProductDetailss.ItemCount = totalCount;

                                    RefGenMedProductDetailss.SourceCollection = ListRefGenMedProduct;
                                    NotifyOfPropertyChange(() => RefGenMedProductDetailss);
                                }
                                au.ItemsSource = RefGenMedProductDetailss;
                                au.PopulateComplete();
                            }
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

        public void txtRefGenMedProduct_LostFocus(object sender, RoutedEventArgs e)
        {
            AxTextBox obj = sender as AxTextBox;

            if (obj == null || string.IsNullOrWhiteSpace(obj.Text))
            {
                return;
            }

            string Code = Globals.FormatCode((long)AllLookupValues.MedProductType.THUOC, obj.Text);

            SearchRefDrugGenericDetails_AutoPaging(true, Code, 0, RefGenMedProductDetailss.PageSize);
        }

        private string BrandName;
        AutoCompleteBox au;
        public void AutoRefGenMedProduct_Populating(object sender, PopulatingEventArgs e)
        {
            au = sender as AutoCompleteBox;
            BrandName = e.Parameter;
            RefGenMedProductDetailss.PageIndex = 0;
            SearchRefDrugGenericDetails_AutoPaging(false, e.Parameter, 0, RefGenMedProductDetailss.PageSize);
        }

        //public void AutoRefGenMedProduct_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        //{
        //    RefGenMedProductSimple obj = (sender as AutoCompleteBox).SelectedItem as RefGenMedProductSimple;
        //    if (NewDrug.MatchRefGenMedProduct != null)
        //    {
        //        if (NewDrug.MatchRefGenMedProduct.BrandName != obj.BrandName)
        //        {
        //            NewDrug.MatchRefGenMedProduct = obj;
        //        }
        //    }
        //    else
        //    {
        //        NewDrug.MatchRefGenMedProduct = obj;
        //    }
        //}

        //18072018 TTM
        //Thieu ham DropDownClosed nen ko cap nhat ten khoa duoc dc, them moi vao
        public void AutoRefGenMedProduct_DropDownClosed(object sender, System.Windows.RoutedPropertyChangedEventArgs<bool> e)
        {
            RefGenMedProductSimple obj = (sender as AutoCompleteBox).SelectedItem as RefGenMedProductSimple;
            if (obj != null)
            {
                if (NewDrug.MatchRefGenMedProduct != null)
                {
                    if (NewDrug.MatchRefGenMedProduct.BrandName != obj.BrandName)
                    {
                        NewDrug.MatchRefGenMedProduct = obj;
                    }
                }
                else
                {
                    NewDrug.MatchRefGenMedProduct = obj;
                }
            }
        }

        /*▼====: #003*/
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
            this.DlgShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginHITransactionType_GetListNoParentID_New(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                HITransactionTypeCollection = new ObservableCollection<HITransactionType>(contract.EndHITransactionType_GetListNoParentID_New(asyncResult));
                                if (HITransactionTypeCollection != null)
                                    HITransactionTypeCollection = HITransactionTypeCollection.Where(x => x.IsShowOnDrugConfig).ToObservableCollection();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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
                    MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }
        /*▲====: #003*/

        AutoCompleteBox Auto_RefGeneric;
        public void Auto_RefGeneric_Loaded(object sender, RoutedEventArgs e)
        {
            Auto_RefGeneric = sender as AutoCompleteBox;
        }
        public void RefGeneric_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            AutoCompleteBox axRefGeneric = sender as AutoCompleteBox;
            if (NewDrug != null)
            {
                if (axRefGeneric != null && axRefGeneric.SelectedItem != null)
                {
                    DrugClass item = axRefGeneric.SelectedItem as DrugClass;
                    if (item != null && item.DrugClassID > 0)
                    {
                        NewDrug.SelectedGeneric = item;
                        NewDrug.GenericID = item.DrugClassID;
                    }
                    else
                    {
                        NewDrug.SelectedGeneric = null;
                        NewDrug.GenericID = 0;
                    }
                }
                else
                {
                    NewDrug.SelectedGeneric = null;
                    NewDrug.GenericID = 0;
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
        //▼===== #007
        public void DispenseVolume_LostFocus(object sender, RoutedEventArgs e)
        {
            if (NewDrug == null)
            {
                return;
            }

            string txt = "";
            txt = (sender as TextBox).Text;
            if (!string.IsNullOrEmpty(txt))
            {
                double value = 0;
                double.TryParse(txt, out value);
                NewDrug.DispenseVolume = value > 0 ? value : 0;
            }
            else //TBL: Trường hợp xóa thì set = 0 
            {
                NewDrug.DispenseVolume = 0;
            }
        }
        //▲===== #007
        public void txt_VAT_LostFocus(object sender, RoutedEventArgs e)
        {
            if (NewDrug == null)
            {
                return;
            }

            string txt = "";
            txt = (sender as TextBox).Text;
            if (!string.IsNullOrEmpty(txt))
            {
                decimal value = 0;
                decimal.TryParse(txt, out value);
                NewDrug.VAT = value > 0 ? value : 0;
            }
            else
            {
                NewDrug.VAT = null;
            }
        }
    }
}
