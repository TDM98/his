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
using System.Linq;
using aEMR.Controls;
using eHCMSLanguage;
using Castle.Windsor;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IRefGenDrug_Edit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RefGenDrug_EditViewModel : Conductor<object>, IRefGenDrug_Edit, IHandle<PharmacyCloseSearchSupplierEvent>
    {
        private long V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
        private long? CityProvinceID = 42;//tam thoi de TP HCM la vay di
        [ImportingConstructor]
        public RefGenDrug_EditViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            _SelectedNewRefMedicalCondition = new RefMedContraIndicationTypes();
            _allNewRefMedicalCondition = new ObservableCollection<RefMedContraIndicationTypes>();
            _allContraIndicatorDrugsRelToMedCond = new ObservableCollection<ContraIndicatorDrugsRelToMedCond>();
            _allContrainName = new ObservableCollection<string>();
           
            //#region get reference member

            //Coroutine.BeginExecute(DoGetSupplierList());
            //Coroutine.BeginExecute(DoGetHospitalList());
            //Coroutine.BeginExecute(DoGetRefGenericDrugCategory_2List());
            //Coroutine.BeginExecute(DoGetRefGenDrugBHYT_CategoryListList());

            //Coroutine.BeginExecute(DoGetUnitList());
            //Coroutine.BeginExecute(DoGetDrugClassList());
            //Coroutine.BeginExecute(DoCountryListList());
            //Coroutine.BeginExecute(DoPharmaceuticalCompanyListList());

            //#endregion 

         
            Globals.EventAggregator.Subscribe(this);
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            if (NewDrug != null)
            {
                GetContraIndicatorDrugsRelToMedCondList(0, NewDrug.DrugID);
                SupplierGenericDrug_LoadDrugIDNotPaging(NewDrug.DrugID);
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
        #region Properties Member

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
                    //GetContraIndicatorDrugsRelToMedCondList(0, NewDrug.DrugID);
                }
            }
        }

        private ObservableCollection<RefMedContraIndicationTypes> _allNewRefMedicalCondition;
        public ObservableCollection<RefMedContraIndicationTypes> allNewRefMedicalCondition
        {
            get
            {
                return _allNewRefMedicalCondition;
            }
            set
            {
                if (_allNewRefMedicalCondition != value)
                {
                    _allNewRefMedicalCondition = value;
                    NotifyOfPropertyChange(() => allNewRefMedicalCondition);
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
        #endregion

        #region get reference member

        //private IEnumerator<IResult> DoGetSupplierList()
        //{
        //    var paymentTypeTask = new LoadSupplierListTask((int)AllLookupValues.SupplierType.CUNG_CAP_THIET_BI_Y_TE, false, false);
        //    yield return paymentTypeTask;
        //    Suppliers = paymentTypeTask.SupplierList;
        //    yield break;
        //}

        //private IEnumerator<IResult> DoGetHospitalList()
        //{
        //    var paymentTypeTask = new LoadHospitalListTask(CityProvinceID, true, false);
        //    yield return paymentTypeTask;
        //    Hospitals = paymentTypeTask.HospitalList;
        //    yield break;
        //}

        //private IEnumerator<IResult> DoGetRefGenericDrugCategory_2List()
        //{
        //    var paymentTypeTask = new LoadRefGenericDrugCategory_2ListTask(false, false);
        //    yield return paymentTypeTask;
        //    RefGenericDrugCategory_2s = paymentTypeTask.RefGenericDrugCategory_2List;
        //    yield break;
        //}

        //private IEnumerator<IResult> DoGetRefGenDrugBHYT_CategoryListList()
        //{
        //    var paymentTypeTask = new LoadRefGenDrugBHYT_CategoryListTask(null,false, false);
        //    yield return paymentTypeTask;
        //    RefGenDrugBHYT_Categorys = paymentTypeTask.RefGenDrugBHYT_CategoryList;
        //    yield break;
        //}

        //private IEnumerator<IResult> DoGetUnitList()
        //{
        //    var paymentTypeTask = new LoadUnitListTask(false, false);
        //    yield return paymentTypeTask;
        //    Units = paymentTypeTask.RefUnitList;
        //    yield break;
        //}

        //private IEnumerator<IResult> DoGetDrugClassList()
        //{
        //    var paymentTypeTask = new LoadDrugClassListTask(V_MedProductType, false, false);
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
        //    var paymentTypeTask = new LoadPharmaceuticalCompanyListTask(false, false);
        //    yield return paymentTypeTask;
        //    PharmaceuticalCompanies = paymentTypeTask.PharmaceuticalCompanyList;
        //    yield break;
        //}
     
        #endregion
      

        private void UpdateDrug()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateDrug(NewDrug, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            contract.EndUpdateDrug(asyncResult);
                            //phat ra su kien de form cha bat dc va load lai du lieu
                            Globals.EventAggregator.Publish(new PharmacyCloseEditGenDrugEvent { });
                            //Đóng Popup
                            TryClose();
                           
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
            if (NewDrug.SupplierGenericDrugs == null || NewDrug.SupplierGenericDrugs.Count == 0)
            {
                MessageBox.Show(eHCMSResources.Z0722_G1_ChonNCCChoThuoc);
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
            }
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
                    UpdateDrug();
                    if (allNewRefMedicalCondition != null)
                    {
                        InsertConIndicatorDrugsRelToMedCond(allNewRefMedicalCondition,
                            NewDrug.DrugID);

                    }
                }
            }
        }

        public void CancelButton(object sender, RoutedEventArgs e)
        {
            TryClose();
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
            string error = "";
            if (u.CountryID ==null || u.CountryID <= 0)
            {
                error += eHCMSResources.Z0853_G1_ChuaChonQuocGia + Environment.NewLine;
            }
            if (u.PCOID ==null || u.PCOID <= 0)
            {
                error += eHCMSResources.Z0854_G1_ChuaChonNSX + Environment.NewLine;
            }
            if (u.DrugClassID == null || u.DrugClassID <= 0)
            {
                error += eHCMSResources.Z0855_G1_ChuaChonNhomThuoc + Environment.NewLine;
            }
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
        public void AddMed(object sender, RoutedEventArgs e)
        {
            //var MedConditionVM = Globals.GetViewModel<IMedCondition>();
            //this.ActivateItem(MedConditionVM);
            //Globals.ShowDialog(MedConditionVM as Conductor<object>);
        }
        public void DeleteContraind(object sender, RoutedEventArgs e)
        {
            allContrainName.Remove(SelectedNewRefMedicalCondition.MedContraIndicationType);
            allNewRefMedicalCondition.Remove(SelectedNewRefMedicalCondition);
            if (SelectedContraIndicatorDrugsRelToMedCond != null)
            {
                if (SelectedContraIndicatorDrugsRelToMedCond.RefGenericDrugDetail != null)
                {
                    DeleteConIndicatorDrugsRelToMedCond(SelectedContraIndicatorDrugsRelToMedCond.DrugsMCTypeID);
                    allContraIndicatorDrugsRelToMedCond.Remove(SelectedContraIndicatorDrugsRelToMedCond);
                }
                else
                {
                    allContrainName.Remove(SelectedContraIndicatorDrugsRelToMedCond.RefMedicalConditionType.MedContraIndicationType);
                    allNewRefMedicalCondition.Remove(SelectedContraIndicatorDrugsRelToMedCond.RefMedicalConditionType);
                    allContraIndicatorDrugsRelToMedCond.Remove(SelectedContraIndicatorDrugsRelToMedCond);
                }
            }
        }

        private void GetContraIndicatorDrugsRelToMedCondList(int MCTypeID, long DrugID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetContraIndicatorDrugsRelToMedCondList(MCTypeID, DrugID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetContraIndicatorDrugsRelToMedCondList(asyncResult);
                            if (results != null)
                            {
                                if (allContraIndicatorDrugsRelToMedCond == null)
                                {
                                    allContraIndicatorDrugsRelToMedCond = new ObservableCollection<ContraIndicatorDrugsRelToMedCond>();
                                }
                                else
                                {
                                    allContraIndicatorDrugsRelToMedCond.Clear();
                                    allContrainName.Clear();
                                }
                                foreach (var p in results)
                                {
                                    allContraIndicatorDrugsRelToMedCond.Add(p);
                                    allContrainName.Add(p.RefMedicalConditionType.MedContraIndicationType);
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

        private void InsertConIndicatorDrugsRelToMedCond(ObservableCollection<RefMedContraIndicationTypes> lstRefMedicalCondition, long DrugID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginInsertConIndicatorDrugsRelToMedCond(lstRefMedicalCondition, DrugID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            bool results = contract.EndInsertConIndicatorDrugsRelToMedCond(asyncResult);
                            if (results)
                            {
                                allNewRefMedicalCondition.Clear();
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

        private void DeleteConIndicatorDrugsRelToMedCond(long DrugsMCTypeID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDeleteConIndicatorDrugsRelToMedCond(DrugsMCTypeID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndDeleteConIndicatorDrugsRelToMedCond(asyncResult);

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
            SupplierGenericDrug item = new SupplierGenericDrug();
            NewDrug.SupplierGenericDrugs.Add(item);
        }

        private void SupplierGenericDrug_LoadDrugIDNotPaging(long DrugID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSupplierGenericDrug_LoadDrugIDNotPaging(DrugID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndSupplierGenericDrug_LoadDrugIDNotPaging(asyncResult);
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
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }
        #region event Handle
        //public void Handle(PharContraEvent obj)
        //{
        //    if (obj != null)
        //    {
        //        //allNewRefMedicalCondition = (ObservableCollection<RefMedicalConditionType>)obj.lstPharContra;
        //        //allContrainName = (ObservableCollection<string>)obj.lstPharContraName;

        //        foreach (var mc in (ObservableCollection<RefMedicalConditionType>)obj.lstPharContra)
        //        {
        //            allNewRefMedicalCondition.Add(mc);
        //            ContraIndicatorDrugsRelToMedCond cdt = new ContraIndicatorDrugsRelToMedCond();
        //            cdt.RefMedicalConditionType = new RefMedicalConditionType();
        //            cdt.RefMedicalConditionType = mc;
        //            allContraIndicatorDrugsRelToMedCond.Add(cdt);
        //        }
        //        allContrainName.Clear();
        //        foreach (string st in allContrainName)
        //        {
        //            allContrainName.Add(st);
        //        }
        //    }
        //}

        #endregion


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
               // MessageBox.Show(eHCMSResources.K0347_G1_ChonNCC);
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
            SupplierGenericDrug item = e.Row.DataContext as SupplierGenericDrug;
            if (e.Column.DisplayIndex == 1)//NCC
            {
                if (CheckExists(item))
                {
                    if (e.Row.GetIndex() == (NewDrug.SupplierGenericDrugs.Count - 1) && e.EditAction == DataGridEditAction.Commit)
                    {
                        AddBlankRow();
                    }
                }
            }

            if (e.Column.DisplayIndex == 3)//NCC
            {
                decimal value = 0;
                decimal.TryParse(PreparingCellForEdit, out value);

                if (value == item.UnitPrice)
                {
                    return;
                }
                item.PackagePrice = item.UnitPrice * NewDrug.UnitPackaging.GetValueOrDefault(1);
            }
            if (e.Column.DisplayIndex == 4)//NCC
            {
                decimal value = 0;
                decimal.TryParse(PreparingCellForEdit, out value);
                if (value == item.PackagePrice)
                {
                    return;
                }
                if (NewDrug.UnitPackaging.GetValueOrDefault() > 0)
                {
                    item.UnitPrice = item.PackagePrice / NewDrug.UnitPackaging.GetValueOrDefault(1);
                }
            }
        }

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

        //public void btnChonNCCHang()
        //{
        //    var proAlloc = Globals.GetViewModel<IAddSupplierForDrug>();
        //    if (NewDrug.DrugID > 0)
        //    {
        //        proAlloc.SupplierDrug.SelectedGenericDrug = NewDrug.DeepCopy();
        //        proAlloc.SupplierGenericDrug_LoadDrugID(0, Globals.PageSize);
        //    }
        //    var instance = proAlloc as Conductor<object>;
        //    Globals.ShowDialog(instance, (o) => { });
        //}
        public void btnDeleteCC_Click(object sender, RoutedEventArgs e)
        {
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

        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            AutoCompleteBox axBHYT = sender as AutoCompleteBox;
            if (NewDrug != null)
            {
                if (axBHYT != null && axBHYT.SelectedItem != null)
                {
                    RefGenDrugBHYT_Category item = axBHYT.SelectedItem as RefGenDrugBHYT_Category;
                    if (item != null && item.RefGenDrugBHYT_CatID > 0)
                    {
                        NewDrug.CurrentRefGenDrugBHYT_Category = item;
                        NewDrug.GenericName = item.CategoryName;
                        NewDrug.RefGenDrugBHYT_CatID = item.RefGenDrugBHYT_CatID;
                        NewDrug.ActiveIngredient = item.CategoryName;
                    }
                    else
                    {
                        NewDrug.CurrentRefGenDrugBHYT_Category = null;
                        NewDrug.GenericName = "";
                        NewDrug.ActiveIngredient = "";
                    }
                }
                else
                {
                    NewDrug.CurrentRefGenDrugBHYT_Category = null;
                    NewDrug.GenericName = "";
                    NewDrug.ActiveIngredient = "";
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

        public void Country_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            AutoCompleteBox axCountry = sender as AutoCompleteBox;
            if (NewDrug != null)
            {
                if (axCountry != null && axCountry.SelectedItem != null)
                {
                    RefCountry item = axCountry.SelectedItem as RefCountry;
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

        public void HangSX_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            AutoCompleteBox axCountry = sender as AutoCompleteBox;
            if (NewDrug != null)
            {
                if (axCountry != null && axCountry.SelectedItem != null)
                {
                    PharmaceuticalCompany item = axCountry.SelectedItem as PharmaceuticalCompany;
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

        public void NhomThuoc_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            AutoCompleteBox axCountry = sender as AutoCompleteBox;
            if (NewDrug != null)
            {
                if (axCountry != null && axCountry.SelectedItem != null)
                {
                    DrugClass item = axCountry.SelectedItem as DrugClass;
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
    }
}
