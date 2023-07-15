using eHCMSLanguage;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using aEMR.CommonTasks;
using System.Linq;
/*
 * 20181114 #001 TTM: BM 0005259: Chuyển đổi CellEditEnded => CellEditEnding.
 */
namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IDrug_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class Drug_AddEditViewModel : Conductor<object>, IDrug_AddEdit
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        private long? CityProvinceID = 42;//tam thoi de TP HCM la vay di

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

        private Int64 _V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
        public Int64 V_MedProductType
        {
            get { return _V_MedProductType; }
            set
            {
                _V_MedProductType = value;
                NotifyOfPropertyChange(() => V_MedProductType);
            }

        }


        [ImportingConstructor]
        public Drug_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Globals.EventAggregator.Subscribe(this);

            #region get reference member

            Coroutine.BeginExecute(GetAllReferenceValues());

            #endregion
        }

        private IEnumerator<IResult> GetAllReferenceValues()
        {
            var LoadSupplierListTask = new LoadDrugDeptSupplierListTask((int)AllLookupValues.SupplierType.CUNG_CAP_THIET_BI_Y_TE, false, false);
            yield return LoadSupplierListTask;
            Suppliers = LoadSupplierListTask.DrugDeptSupplierList;
            var HosListTask = new LoadHospitalListTask(CityProvinceID, true, false);
            yield return HosListTask;
            Hospitals = HosListTask.HospitalList;
            var LoadCategory_1_Task = new LoadRefGenericDrugCategory_1ListTask(V_MedProductType, false, false);
            yield return LoadCategory_1_Task;

            yield return GenericCoRoutineTask.StartTask(LoadRefGenDrugCategory_2, (object)false, (object)false);

            RefGenericDrugCategory_1s = LoadCategory_1_Task.RefGenericDrugCategory_1List;
            var LoadCategory_BHYT_Task = new LoadRefGenDrugBHYT_CategoryListTask(false, null, false, true);
            yield return LoadCategory_BHYT_Task;
            RefGenDrugBHYT_Categorys = LoadCategory_BHYT_Task.RefGenDrugBHYT_CategoryList;
            var LoadUnitListTask = new LoadDrugDeptUnitListTask(false, false);
            yield return LoadUnitListTask;
            Units = LoadUnitListTask.RefUnitList;
            var LoadDrugClassListTask = new LoadDrugDeptClassListTask(V_MedProductType, false, false);
            yield return LoadDrugClassListTask;
            FamilyTherapies = LoadDrugClassListTask.DrugClassList;
            var LoadCountryListTask = new LoadCountryListTask(false, false);
            yield return LoadCountryListTask;
            Countries = LoadCountryListTask.RefCountryList;
            var paymentTypeTask = new LoadDrugDeptPharmaceuticalCompanyListTask(false, false);
            yield return paymentTypeTask;
            PharmaceuticalCompanies = paymentTypeTask.DrugDeptPharmaceuticalCompanyList;
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

        public void InitializeNewItem(Int64 pV_MedProductType)
        {
            ObjRefGenMedProductDetails_Current = new RefGenMedProductDetails();
            ObjRefGenMedProductDetails_Current.UnitPackaging = 1;
            ObjRefGenMedProductDetails_Current.NumberOfEstimatedMonths_F = 1;
            ObjRefGenMedProductDetails_Current.V_MedProductType = pV_MedProductType;

            ObjRefGenMedProductDetails_Current.RefGenMedDrugDetails = new RefGenMedDrugDetails();
            ObjRefGenMedProductDetails_Current.RefGenMedDrugDetails.AdvTimeBeforeExpire = 1;

            AddBlankRow();
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

        #endregion

        #region get reference member

        #endregion

        private bool CheckData()
        {
            if (ObjRefGenMedProductDetails_Current.SelectedCountry != null)
            {
                ObjRefGenMedProductDetails_Current.CountryID = ObjRefGenMedProductDetails_Current.SelectedCountry.CountryID;
            }

            if (ObjRefGenMedProductDetails_Current.SelectedUnit != null)
            {
                ObjRefGenMedProductDetails_Current.UnitID = ObjRefGenMedProductDetails_Current.SelectedUnit.UnitID;

            }

            if (ObjRefGenMedProductDetails_Current.SelectedUnitUse != null)
            {
                ObjRefGenMedProductDetails_Current.UnitUseID = ObjRefGenMedProductDetails_Current.SelectedUnitUse.UnitID;
            }

            if (ObjRefGenMedProductDetails_Current.PharmaceuticalCompany != null)
            {
                ObjRefGenMedProductDetails_Current.PCOID = ObjRefGenMedProductDetails_Current.PharmaceuticalCompany.PCOID;
            }

            if (ObjRefGenMedProductDetails_Current.SelectedDrugClass != null)
            {
                ObjRefGenMedProductDetails_Current.DrugClassID = ObjRefGenMedProductDetails_Current.SelectedDrugClass.DrugClassID;
            }

            if (ObjRefGenMedProductDetails_Current.SupplierGenMedProducts == null || ObjRefGenMedProductDetails_Current.SupplierGenMedProducts.Count == 0)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0722_G1_ChonNCCChoThuoc));
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
            }
            return true;
        }

        public void btSave()
        {
            if (CheckValid(ObjRefGenMedProductDetails_Current))
            {
                if (CheckData())
                {
                    RefGenMedProductDetails_Save(ObjRefGenMedProductDetails_Current);
                }
            }
        }

        ObservableCollection<RefMedContraIndicationTypes> temp = null;

        private void RefGenMedProductDetails_Save(RefGenMedProductDetails Obj)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0343_G1_DangLuu) });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginRefGenMedProductDetails_Save(Obj,temp, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Res = "";
                            long GenMedProductID;
                            contract.EndRefGenMedProductDetails_Save(out Res, out GenMedProductID, asyncResult);

                            string[] arrRes = new string[2];

                            switch (Res)
                            {
                                case "Update-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A0608_G1_Msg_InfoHChinhFail, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-1":
                                    {
                                        TitleForm = string.Format("{0} ", eHCMSResources.Z0723_G1_HChinhThuoc) + ": " + ObjRefGenMedProductDetails_Current.BrandName.Trim();

                                        Globals.EventAggregator.Publish(new Drug_AddEditViewModel_Save_Event() { Result = true });

                                        MessageBox.Show(string.Format("{0}!", eHCMSResources.A0468_G1_Msg_InfoLuuOK), eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Insert-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A1026_G1_Msg_InfoThemFail, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Insert-1":
                                    {
                                        Globals.EventAggregator.Publish(new Drug_AddEditViewModel_Save_Event() { Result = true });

                                        MessageBox.Show(string.Format("{0}!", eHCMSResources.A0468_G1_Msg_InfoLuuOK), eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                default:
                                    {
                                        arrRes = Res.Split('@');

                                        string msg1 = "";
                                        string msg2 = "";

                                        if (arrRes[0] != "")
                                        {
                                            msg1 = string.Format("{0}!", eHCMSResources.Z0724_G1_TenThuongMaiDaDuocSD);
                                        }

                                        if (arrRes[1] != "")
                                        {
                                            msg2 = string.Format("{0}!", eHCMSResources.Z0725_G1_CodeDaDuocSD);
                                        }

                                        string msg = (msg1 == "" ? "" : "- " + msg1) + (msg2 != "" ? Environment.NewLine + "- " + msg2 : "");

                                        MessageBox.Show(msg + Environment.NewLine + string.Format("{0}!", eHCMSResources.I0946_G1_I), eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                            }

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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

        public bool CheckValid(object temp)
        {
            RefGenMedProductDetails u = temp as RefGenMedProductDetails;
            if (u == null)
            {
                return false;
            }
            return u.Validate();
        }


        public void btClose()
        {
            TryClose();
        }

        //#region "Gán giá trị cho các UC con"
        //private DrugClass _objDrugClassesSelected;
        //public DrugClass ObjDrugClasses_Selected
        //{
        //    get
        //    {
        //        if (UCDrugClassesSearchPagingDrug == null)
        //        {
        //            return null;
        //        }
        //        return UCDrugClassesSearchPagingDrug.ObjDrugClasses_Selected;
        //    }
        //    set
        //    {
        //        if (UCDrugClassesSearchPagingDrug != null)
        //        {
        //            UCDrugClassesSearchPagingDrug.ObjDrugClasses_Selected = value;
        //        }
        //    }
        //}

        //private RefCountry _objRefCountry_Selected;
        //public RefCountry ObjRefCountry_Selected
        //{
        //    get
        //    {
        //        if(UCRefCountriesSearchPaging==null)
        //            return  null;
        //        return UCRefCountriesSearchPaging.ObjRefCountries_Selected;
        //    }
        //    set
        //    {
        //        if(UCRefCountriesSearchPaging!=null)
        //        {
        //            UCRefCountriesSearchPaging.ObjRefCountries_Selected = value;
        //        }
        //    }
        //}

        //private RefUnit _objRefUnitCal_Selected;
        //public RefUnit ObjRefUnitCal_Selected
        //{
        //    get
        //    {
        //        if (UCRefUnitsCalSearchPaging == null)
        //            return null;
        //        return UCRefUnitsCalSearchPaging.ObjRefUnits_Selected;
        //    }
        //    set
        //    {
        //        if (UCRefUnitsCalSearchPaging != null)
        //        {
        //            UCRefUnitsCalSearchPaging.ObjRefUnits_Selected = value;
        //        }
        //    }
        //}

        //private RefUnit _objRefUnitUse_Selected;
        //public RefUnit ObjRefUnitUse_Selected
        //{
        //    get
        //    {
        //        if (UCRefUnitsUseSearchPaging == null)
        //            return null;
        //        return UCRefUnitsUseSearchPaging.ObjRefUnits_Selected;
        //    }
        //    set
        //    {
        //        if (UCRefUnitsUseSearchPaging != null)
        //        {
        //            UCRefUnitsUseSearchPaging.ObjRefUnits_Selected = value;
        //        }
        //    }
        //}
        //#endregion

        //public void Handle(DrugClasses_SearchPagingDrugViewModel_Selected_Event message)
        //{
        //    if(message!=null)
        //    {
        //        if (message.ObjectSelected != null)
        //        {
        //            DrugClass tmp = message.ObjectSelected as DrugClass;

        //            if (tmp.DrugClassID > 0)
        //            {
        //                ObjRefGenMedProductDetails_Current.RefGenMedDrugDetails.SelectedDrugClass = tmp;
        //                ObjRefGenMedProductDetails_Current.RefGenMedDrugDetails.DrugClassID = tmp.DrugClassID;
        //            }
        //            else
        //            {
        //                ObjRefGenMedProductDetails_Current.RefGenMedDrugDetails.DrugClassID = null;
        //            }
        //        }
        //        else
        //        {
        //            ObjRefGenMedProductDetails_Current.RefGenMedDrugDetails.DrugClassID = null;
        //        }
        //    }
        //}

        //public void Handle(RefCountries_SearchPagingViewModel_Selected_Event message)
        //{
        //    if (message != null)
        //    {
        //        if (message.ObjectSelected != null)
        //        {
        //            RefCountry tmp = message.ObjectSelected as RefCountry;

        //            if (tmp.CountryID > 0)
        //            {
        //                ObjRefGenMedProductDetails_Current.SelectedCountry = tmp;
        //                ObjRefGenMedProductDetails_Current.CountryID = tmp.CountryID;
        //            }
        //            else
        //            {
        //                ObjRefGenMedProductDetails_Current.CountryID = 0;
        //            }
        //        }
        //        else
        //        {
        //            ObjRefGenMedProductDetails_Current.CountryID = 0;
        //        }
        //    }
        //}

        //public void Handle(RefUnitsCal_SearchPagingView_Selected_Event message)
        //{
        //    if (message != null)
        //    {
        //        if (message.ObjectSelected != null)
        //        {
        //            DataEntities.RefUnit tmp = message.ObjectSelected as DataEntities.RefUnit;

        //            if (tmp.UnitID > 0)
        //            {
        //                ObjRefGenMedProductDetails_Current.SelectedUnit = tmp;
        //                ObjRefGenMedProductDetails_Current.UnitID = tmp.UnitID;
        //            }
        //            else
        //            {
        //                ObjRefGenMedProductDetails_Current.UnitID = 0;
        //            }
        //        }
        //        else
        //        {
        //            ObjRefGenMedProductDetails_Current.UnitID = 0;
        //        }
        //    }
        //}

        //public void Handle(RefUnitsUse_SearchPagingView_Selected_Event message)
        //{
        //    if (message != null)
        //    {
        //        if (message.ObjectSelected != null)
        //        {
        //            DataEntities.RefUnit tmp = message.ObjectSelected as DataEntities.RefUnit;

        //            if (tmp.UnitID > 0)
        //            {
        //                ObjRefGenMedProductDetails_Current.SelectedUnitUse = tmp;
        //                ObjRefGenMedProductDetails_Current.UnitUseID = tmp.UnitID;
        //            }
        //            else
        //            {
        //                ObjRefGenMedProductDetails_Current.UnitUseID = 0;
        //            }
        //        }
        //        else
        //        {
        //            ObjRefGenMedProductDetails_Current.UnitUseID = 0;
        //        }
        //    }
        //}

        #region Supplier member

        public void SupplierGenMedProduct_LoadDrugIDNotPaging(long GenMedProductID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
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
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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

        private void AddBlankRow()
        {
            if (ObjRefGenMedProductDetails_Current.SupplierGenMedProducts == null)
            {
                ObjRefGenMedProductDetails_Current.SupplierGenMedProducts = new ObservableCollection<SupplierGenMedProduct>();
            }
            SupplierGenMedProduct item = new SupplierGenMedProduct();
            ObjRefGenMedProductDetails_Current.SupplierGenMedProducts.Add(item);
        }

        public void acbDrug_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as ComboBox).ItemsSource = Suppliers;
        }

        private bool CheckExists(SupplierGenMedProduct ite)
        {
            if (ite != null && ite.SelectedSupplier != null)
            {
                ite.SupplierID = ite.SelectedSupplier.SupplierID;
                var value = ObjRefGenMedProductDetails_Current.SupplierGenMedProducts.Where(x => x.SupplierID == ite.SupplierID);
                if (value != null && value.Count() > 1)
                {
                    MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0731_G1_NCCDaDuocChon));
                    ite.SelectedSupplier = null;
                    return false;
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.K0347_G1_ChonNCC);
                return false;
            }
            return true;
        }

        DataGrid GridSuppliers = null;
        public void GridSuppliers_Loaded(object sender, RoutedEventArgs e)
        {
            GridSuppliers = sender as DataGrid;
        }
        //▼====== #001
        //public void GridSuppliers_CellEditEnded(object sender, DataGridCellEditEndingEventArgs e)
        //{
        //    SupplierGenMedProduct item = e.Row.DataContext as SupplierGenMedProduct;
        //    if (CheckExists(item))
        //    {
        //        if (e.Row.GetIndex() == (ObjRefGenMedProductDetails_Current.SupplierGenMedProducts.Count - 1) && e.EditAction == DataGridEditAction.Commit)
        //        {
        //            AddBlankRow();
        //        }
        //    }
        //}
        public void GridSuppliers_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            SupplierGenMedProduct item = e.Row.DataContext as SupplierGenMedProduct;
            if (CheckExists(item))
            {
                if (e.Row.GetIndex() == (ObjRefGenMedProductDetails_Current.SupplierGenMedProducts.Count - 1) && e.EditAction == DataGridEditAction.Commit)
                {
                    AddBlankRow();
                }
            }
        }
        //▲====== #001
        public void btnDeleteCC_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(string.Format(eHCMSResources.Z0557_G1_CoChacMuonXoa, eHCMSResources.N0037_G1_NCC), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
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


     
        private bool IsChange = true;
        public void RefGenDrugBHYT_CatID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsChange)
            {
                if ((sender as ComboBox).SelectedItem != null)
                {
                    RefGenDrugBHYT_Category item = (sender as ComboBox).SelectedItem as RefGenDrugBHYT_Category;
                    if (item != null && item.RefGenDrugBHYT_CatID > 0)
                    {
                        ObjRefGenMedProductDetails_Current.GenericName = item.CategoryName;
                        if (ObjRefGenMedProductDetails_Current.RefGenMedDrugDetails == null)
                        {
                            IsChange = false;
                            ObjRefGenMedProductDetails_Current.RefGenMedDrugDetails = new RefGenMedDrugDetails();
                            ObjRefGenMedProductDetails_Current.RefGenMedDrugDetails.RefGenDrugBHYT_CatID = item.RefGenDrugBHYT_CatID;
                        }
                        ObjRefGenMedProductDetails_Current.RefGenMedDrugDetails.ActiveIngredient = item.CategoryName;
                    }
                }
                else
                {
                    ObjRefGenMedProductDetails_Current.GenericName = "";
                    if (ObjRefGenMedProductDetails_Current.RefGenMedDrugDetails != null)
                    {
                        ObjRefGenMedProductDetails_Current.RefGenMedDrugDetails.ActiveIngredient = "";
                    }
                }
                IsChange = true;
            }
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

        private void GetContraIndicatorDrugsRelToMedCondList(int MCTypeID, long DrugID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
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
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void AddMed()
        {
            //var MedConditionVM = Globals.GetViewModel<IDrugDeptMedCondition>();
            //MedConditionVM.RefMedicalConditionType_Edit = RefMedicalConditionType_Edit;
            //MedConditionVM.NewDrug = ObjRefGenMedProductDetails_Current;
            //this.ActivateItem(MedConditionVM);
            //Globals.ShowDialog(MedConditionVM as Conductor<object>);

            Action<IDrugDeptMedCondition> onInitDlg = (MedConditionVM) =>
            {
                MedConditionVM.RefMedicalConditionType_Edit = RefMedicalConditionType_Edit;
                MedConditionVM.NewDrug = ObjRefGenMedProductDetails_Current;
                this.ActivateItem(MedConditionVM);
            };
            GlobalsNAV.ShowDialog<IDrugDeptMedCondition>(onInitDlg);
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

//private IEnumerator<IResult> DoGetHospitalList()
//{
//    var paymentTypeTask = new LoadHospitalListTask(CityProvinceID, true, false);
//    yield return paymentTypeTask;
//    Hospitals = paymentTypeTask.HospitalList;
//    yield break;
//}

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