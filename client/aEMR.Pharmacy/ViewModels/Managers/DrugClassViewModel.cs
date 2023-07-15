using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
using Castle.Windsor;
using System.Collections.ObjectModel;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.Common;
using System.Threading;
using DataEntities;
using eHCMSLanguage;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IDrugClass)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DrugClassViewModel : Conductor<object>, IDrugClass
    {
        public string TitleForm { get; set; }

        #region Indicator Member
      

        private bool _isLoadingFullOperator = false;
        public bool isLoadingFullOperator
        {
            get { return _isLoadingFullOperator; }
            set
            {
                if (_isLoadingFullOperator != value)
                {
                    _isLoadingFullOperator = value;
                    NotifyOfPropertyChange(() => isLoadingFullOperator);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

    

        private bool _isLoadingGetID = false;
        public bool isLoadingGetID
        {
            get { return _isLoadingGetID; }
            set
            {
                if (_isLoadingGetID != value)
                {
                    _isLoadingGetID = value;
                    NotifyOfPropertyChange(() => isLoadingGetID);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingSearch = false;
        public bool isLoadingSearch
        {
            get { return _isLoadingSearch; }
            set
            {
                if (_isLoadingSearch != value)
                {
                    _isLoadingSearch = value;
                    NotifyOfPropertyChange(() => isLoadingSearch);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

   
      

        public bool IsLoading
        {
            get { return ( isLoadingFullOperator  || isLoadingGetID || isLoadingSearch); }
        }

        #endregion

        private long V_MedProductType =(long)AllLookupValues.MedProductType.THUOC;

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public DrugClassViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            authorization();
            _TreeViewTherapies = new ObservableCollection<TherapyTree>();
            _therapiesall = new ObservableCollection<TherapyTree>();
            _seletedfamilyTherapy = new DrugClass();

            LoadFamilyParent(V_MedProductType);
            GetSearchTreeView(V_MedProductType);
        }

        #region Properties Member
        private ObservableCollection<TherapyTree> _TreeViewTherapies;
        public ObservableCollection<TherapyTree> TreeViewTherapies
        {
            get
            {
                return _TreeViewTherapies;
            }
            set
            {
                if (_TreeViewTherapies != value)
                {
                    _TreeViewTherapies = value;
                    NotifyOfPropertyChange(() => TreeViewTherapies);
                }
            }
        }

        private ObservableCollection<TherapyTree> _therapiesall;
        public ObservableCollection<TherapyTree> TherapiesAll
        {
            get
            {
                return _therapiesall;
            }
            set
            {
                if (_therapiesall != value)
                {
                    _therapiesall = value;
                    NotifyOfPropertyChange(() => TherapiesAll);
                }
            }
        }

        private ObservableCollection<DrugClass> _parentDrugClass;
        public ObservableCollection<DrugClass> ParentDrugClass
        {
            get
            {
                return _parentDrugClass;
            }
            set
            {
                if (_parentDrugClass != value)
                {
                    _parentDrugClass = value;
                    NotifyOfPropertyChange(() => ParentDrugClass);
                }
            }
        }

        private string _faname;
        public string FaName
        {
            get
            {
                return _faname;
            }
            set
            {
                if (_faname != value)
                {
                    _faname = value;
                    NotifyOfPropertyChange(() => FaName);
                }
            }
        }

        private bool _IsEnabled = true;
        public bool IsEnabled
        {
            get
            {
                return _IsEnabled;
            }
            set
            {
                if (_IsEnabled != value)
                {
                    _IsEnabled = value;
                    NotifyOfPropertyChange(() => IsEnabled);
                }
            }
        }

        private DrugClass _seletedfamilyTherapy;
        public DrugClass SeletedFamilyTherapy
        {
            get
            {
                return _seletedfamilyTherapy;
            }
            set
            {
                if (_seletedfamilyTherapy != value)
                {
                    _seletedfamilyTherapy = value;
                    NotifyOfPropertyChange(() => SeletedFamilyTherapy);
                }
            }
        }

        private TherapyTree _CurrentTherapyTree;
        public TherapyTree CurrentTherapyTree
        {
            get
            {
                return _CurrentTherapyTree;
            }
            set
            {
                if (_CurrentTherapyTree != value)
                {
                    _CurrentTherapyTree = value;
                    NotifyOfPropertyChange(() => CurrentTherapyTree);
                }
            }
        }

        #endregion

       
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bTim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyLopThuoc,
                                               (int)oPharmacyEx.mQuanLyLopThuoc_Tim, (int)ePermission.mView);
            bThem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyLopThuoc,
                                               (int)oPharmacyEx.mQuanLyLopThuoc_Them, (int)ePermission.mView);
            bChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyLopThuoc,
                                               (int)oPharmacyEx.mQuanLyLopThuoc_ChinhSua, (int)ePermission.mView);
            

        }
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

        public void lnkDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;
            lnkDelete.Visibility = Globals.convertVisibility(bChinhSua);
        }

        #endregion
        private void GetAllChildrenNode(TherapyTree p)
        {
            foreach (TherapyTree pp in p.Children)
            {
                TherapiesAll.Add(pp);
                GetAllChildrenNode(pp);
            }
        }
        private void GetSearchTreeView(long V_MedProductType)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            isLoadingSearch = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetSearchTreeView(FaName,V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetSearchTreeView(asyncResult);
                            if (results != null)
                            {
                                TreeViewTherapies.Clear();
                                TherapiesAll.Clear();
                                foreach (TherapyTree p in results)
                                {
                                    TreeViewTherapies.Add(p);
                                    TherapiesAll.Add(p);
                                    GetAllChildrenNode(p);
                                }
                                NotifyOfPropertyChange(() => TreeViewTherapies);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingSearch = false;
                           // Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void LoadFamilyParent(long V_MedProductType)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            isLoadingGetID = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetFamilyTherapyParent(V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetFamilyTherapyParent(asyncResult);
                            if (results != null)
                            {
                                if (ParentDrugClass == null)
                                {
                                    ParentDrugClass = new ObservableCollection<DrugClass>();
                                }
                                else
                                {
                                    ParentDrugClass.Clear();
                                }
                                DrugClass all = new DrugClass();
                                all.DrugClassID = 0;
                                all.FaName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0637_G1_NoSelect);
                                ParentDrugClass.Add(all);

                                foreach (DrugClass p in results)
                                {
                                    ParentDrugClass.Add(p);
                                }
                                NotifyOfPropertyChange(() => ParentDrugClass);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingGetID = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
            //_therapyCatalog.GetFamilytherapyParent();
        }

        private void ConvertTherapyTreeToRef(TherapyTree tree)
        {
            if (tree != null)
            {
                if (tree.Parent == null)
                {
                    tree.Parent = new DrugClass();
                }
                SeletedFamilyTherapy.DrugClassID = tree.NodeID;
                SeletedFamilyTherapy.FaDescription = tree.Description;
                SeletedFamilyTherapy.FaName = tree.NodeText;
                SeletedFamilyTherapy.DrugClassCode = tree.Code;
                SeletedFamilyTherapy.ParDrugClassID = tree.Parent.DrugClassID;
                SeletedFamilyTherapy.FaActive = true;
                NotifyOfPropertyChange(() => SeletedFamilyTherapy);
            }
        }

        private void AddDrugClass()
        {
           // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            isLoadingFullOperator = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginAddNewFamilyTherapy(SeletedFamilyTherapy, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            string StrError = "";
                            contract.EndAddNewFamilyTherapy(out StrError,asyncResult);
                            if (string.IsNullOrEmpty(StrError))
                            {
                                Globals.ShowMessage(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK, eHCMSResources.G0442_G1_TBao);
                                IsEnabled = true;
                                GetSearchTreeView(V_MedProductType);
                            }
                            else
                            {
                                Globals.ShowMessage(StrError,eHCMSResources.G0442_G1_TBao);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingFullOperator = false;
                           // Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void AddDrugClass(object newFamilyTherapy)
        {
            if(!bThem)
            {
                Globals.ShowMessage(eHCMSResources.Z0638_G1_ChuaCapQuyenThemLopThuoc, "");
                return;
            }
            ConvertTherapyTreeToRef((TherapyTree)newFamilyTherapy);
            AddDrugClass();
        }

        private void UpdateDrugClass()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            isLoadingFullOperator = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginUpdateFamilyTherapy(SeletedFamilyTherapy, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            string StrError = "";
                            contract.EndUpdateFamilyTherapy(out StrError,asyncResult);
                            if (string.IsNullOrEmpty(StrError))
                            {
                                Globals.ShowMessage(eHCMSResources.A0279_G1_Msg_InfoCNhatOK, eHCMSResources.G0442_G1_TBao);
                                IsEnabled = true;
                                GetSearchTreeView(V_MedProductType);
                            }
                            else
                            {
                                Globals.ShowMessage(StrError, eHCMSResources.G0442_G1_TBao);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingFullOperator = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void UpdateDrugClass(object newFamilyTherapy)
        {
            if (!bChinhSua)
            {
                Globals.ShowMessage(eHCMSResources.Z0639_G1_ChuaCapQuyenSuaLopThuoc, "");
                return;
            }
            ConvertTherapyTreeToRef((TherapyTree)newFamilyTherapy);
            UpdateDrugClass();
        }

        private void DeleteDrugClass(long DrugClassID)
        {
           // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            isLoadingFullOperator = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginDeleteFamilyTherapy(DrugClassID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            string StrError = "";
                            contract.EndDeleteFamilyTherapy(out StrError, asyncResult);
                            if (string.IsNullOrEmpty(StrError))
                            {
                                Globals.ShowMessage(eHCMSResources.A0478_G1_Msg_InfoXoaOK, eHCMSResources.G0442_G1_TBao);
                                IsEnabled = true;
                                GetSearchTreeView(V_MedProductType);
                            }
                            else
                            {
                                Globals.ShowMessage(StrError, eHCMSResources.G0442_G1_TBao);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingFullOperator = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void DeleteDrugClass(object Current)
        {
            if (!bChinhSua)
            {
                Globals.ShowMessage(eHCMSResources.Z0640_G1_ChuaCapQuyenXoaLopThuoc, "");
                return;
            }
            DeleteDrugClass(((TherapyTree)Current).NodeID);
        }

        public void treeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null)
            {
                CurrentTherapyTree = (TherapyTree)e.NewValue;
            }
        }

        public void dataForm1_DeletingItem(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DataForm dataForm1 = sender as DataForm;
            if (MessageBox.Show(eHCMSResources.A0166_G1_Msg_ConfXoaLopThuoc, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeleteDrugClass(dataForm1.CurrentItem);
            }
            else
            {
                e.Cancel = true;
            }
        }

        int flag = 0;

        //26062018 TTM: Comment toan bo nhung gi lien quan den DataForm ra, View va viewmodel DrugClass can viet lai.

        //public void dataForm1_EditEnded(object sender, DataFormEditEndEventArgs e)
        //{
        //    DataForm dataForm1 = sender as DataForm;
        //    dataForm1.CommandButtonsVisibility = DataFormCommandButtonsVisibility.All;
        //    IsEnabled = true;

        //    if (dataForm1.CurrentIndex > -1)
        //    {
        //        if (e.EditAction == DataFormEditAction.Cancel)
        //        {
        //            flag = 0;
        //        }
        //        if (e.EditAction == DataFormEditAction.Commit)
        //        {
        //            if (flag == 1)
        //            {
        //                // if (dataForm1.IsItemChanged)
        //                {
        //                    UpdateDrugClass(dataForm1.CurrentItem);
        //                }
        //                flag = 0;
        //            }
        //            else if (flag == 2)
        //            {
        //                AddDrugClass(dataForm1.CurrentItem);
        //                flag = 0;
        //            }
        //        }
        //    }
        //}
        //public void dataForm1_AddingNewItem(object sender, DataFormAddingNewItemEventArgs e)
        //{
        //    flag = 2;
        //    IsEnabled = false;
        //}
        //public void dataForm1_BeginningEdit(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    flag = 1;
        //    DataForm dataForm1 = sender as DataForm;
        //    dataForm1.CommandButtonsVisibility = DataFormCommandButtonsVisibility.Edit | DataFormCommandButtonsVisibility.Cancel | DataFormCommandButtonsVisibility.Commit | DataFormCommandButtonsVisibility.Navigation;
        //    IsEnabled = false;

        //}
        //public void dataForm1_ValidatingItem(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    DataForm dataForm1 = sender as DataForm;
        //    if (dataForm1.ValidationSummary.Errors.Count > 0)
        //    {
        //        e.Cancel = true;
        //    }
        //}

        public void txt_search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FaName = (sender as TextBox).Text;
                GetSearchTreeView(V_MedProductType);
            }
        }
        public void Search(object sender, RoutedEventArgs e)
        {
            GetSearchTreeView(V_MedProductType);
        }

        public void cbxFamilyTherapy_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ComboBox cbx = sender as ComboBox;
            cbx.ItemsSource = ParentDrugClass;
        }

    }
}
