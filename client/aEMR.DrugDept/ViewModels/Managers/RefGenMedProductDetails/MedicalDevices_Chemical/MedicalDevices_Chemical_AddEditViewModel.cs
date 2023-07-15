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
using eHCMSLanguage;
/*
 * 20181114 #001 TTM: BM 0005259: Chuyển đổi CellEditEnded => CellEditEnding.
 */
namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IMedicalDevices_Chemical_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MedicalDevices_Chemical_AddEditViewModel : Conductor<object>, IMedicalDevices_Chemical_AddEdit
      
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public MedicalDevices_Chemical_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }
        private Int64 _V_MedProductType;
        public Int64 V_MedProductType
        {
            get { return _V_MedProductType; }
            set
            {
                _V_MedProductType = value;
                NotifyOfPropertyChange(() => V_MedProductType);
            }

        }

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

        protected override void OnActivate()
        {
            base.OnActivate();

            Globals.EventAggregator.Subscribe(this);

            #region get reference member

            Coroutine.BeginExecute(DoGetSupplierList());
            Coroutine.BeginExecute(DoGetHospitalList());
            Coroutine.BeginExecute(DoGetRefGenericDrugCategory_1List());

            Coroutine.BeginExecute(DoGetUnitList());
            Coroutine.BeginExecute(DoGetDrugClassList());
            Coroutine.BeginExecute(DoCountryListList());
            Coroutine.BeginExecute(DoPharmaceuticalCompanyListList());

            #endregion
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
                                        MessageBox.Show(string.Format("{0}!", eHCMSResources.A0608_G1_Msg_InfoHChinhFail), eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-1":
                                    {
                                        TitleForm = string.Format("{0} ", eHCMSResources.T1484_G1_HChinh)+TextType()+" " + ": " + ObjRefGenMedProductDetails_Current.BrandName.Trim();

                                        Globals.EventAggregator.Publish(new Drug_AddEditViewModel_Save_Event() { Result = true });


                                        MessageBox.Show(string.Format("{0}!", eHCMSResources.A0468_G1_Msg_InfoLuuOK), eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Insert-0":
                                    {
                                        MessageBox.Show(string.Format("{0}!", eHCMSResources.A1026_G1_Msg_InfoThemFail), eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
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

        private string TextType()
        {
            string Result = "";
            switch (V_MedProductType)
            {
                case 11001:
                    {
                        Result = eHCMSResources.G0787_G1_Thuoc;
                        break;
                    }
                case 11002:
                    {
                        Result = eHCMSResources.G2907_G1_YCu;
                        break;
                    }
                case 11003:
                    {
                        Result = eHCMSResources.T1616_G1_HC;
                        break;
                    }
            }
            return Result;
        }

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

        private IEnumerator<IResult> DoGetSupplierList()
        {
            var paymentTypeTask = new LoadDrugDeptSupplierListTask((int)AllLookupValues.SupplierType.CUNG_CAP_THIET_BI_Y_TE, false, false);
            yield return paymentTypeTask;
            Suppliers = paymentTypeTask.DrugDeptSupplierList;
            yield break;
        }

        private long? CityProvinceID = 42;//tam thoi de TP HCM la vay di

        private IEnumerator<IResult> DoGetHospitalList()
        {
            var paymentTypeTask = new LoadHospitalListTask(CityProvinceID, true, false);
            yield return paymentTypeTask;
            Hospitals = paymentTypeTask.HospitalList;
            yield break;
        }

        private IEnumerator<IResult> DoGetRefGenericDrugCategory_1List()
        {
            var paymentTypeTask = new LoadRefGenericDrugCategory_1ListTask(V_MedProductType, false, false);
            yield return paymentTypeTask;
            RefGenericDrugCategory_1s = paymentTypeTask.RefGenericDrugCategory_1List;
            yield break;
        }



        private IEnumerator<IResult> DoGetUnitList()
        {
            var paymentTypeTask = new LoadUnitListTask(false, false);
            yield return paymentTypeTask;
            Units = paymentTypeTask.RefUnitList;
            yield break;
        }

        private IEnumerator<IResult> DoGetDrugClassList()
        {
            var paymentTypeTask = new LoadDrugClassListTask(V_MedProductType, false, false);
            yield return paymentTypeTask;
            FamilyTherapies = paymentTypeTask.DrugClassList;
            yield break;
        }

        private IEnumerator<IResult> DoCountryListList()
        {
            var paymentTypeTask = new LoadCountryListTask(false, false);
            yield return paymentTypeTask;
            Countries = paymentTypeTask.RefCountryList;
            yield break;
        }

        private IEnumerator<IResult> DoPharmaceuticalCompanyListList()
        {
            var paymentTypeTask = new LoadDrugDeptPharmaceuticalCompanyListTask(false, false);
            yield return paymentTypeTask;
            PharmaceuticalCompanies = paymentTypeTask.DrugDeptPharmaceuticalCompanyList;
            yield break;
        }

        #endregion

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
    }
}
