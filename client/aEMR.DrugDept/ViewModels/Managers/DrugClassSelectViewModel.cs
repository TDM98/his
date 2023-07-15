using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System;
using aEMR.Common;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using eHCMSLanguage;
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using System.Linq;

/*
 * 20181002 #001 TBL: BM 0000070. Fix khi tu QL Hoat chat qua cac QL khac thi hien thong bao loi
 * 20191223 #002 TBL: BM 0021758: Thay đổi cách lưu trường Nhóm hàng, Code BHYT trong danh mục Y cụ
 */
namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IDrugDeptSelectClass)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DrugClassSelectViewModel : ViewModelBase, IDrugDeptSelectClass
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

        private long _V_MedProductType;
        public long V_MedProductType
        {
            get { return _V_MedProductType; }
            set {
                _V_MedProductType = value;
                NotifyOfPropertyChange(()=>V_MedProductType);
            }
        }
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public DrugClassSelectViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            myGuid = Guid.NewGuid();
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            authorization();
            TreeViewTherapies = new ObservableCollection<TherapyTree>();
            TherapiesAll = new ObservableCollection<TherapyTree>();
            SeletedFamilyTherapy = new DrugClass();

            //LoadFamilyParent(V_MedProductType);
            //GetSearchTreeView(V_MedProductType);
        }

        private Guid myGuid;

        ~DrugClassSelectViewModel()
        {

        }
        protected override void OnActivate()
        {
            Globals.EventAggregator.Subscribe(this);

            base.OnActivate();
        }
        protected override void OnDeactivate(bool close)
        {
            Globals.EventAggregator.Unsubscribe(this);
            base.OnDeactivate(close);
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

        private bool _IsEnabledTxt = false;
        public bool IsEnabledTxt
        {
            get
            {
                return _IsEnabledTxt;
            }
            set
            {
                if (_IsEnabledTxt != value)
                {
                    _IsEnabledTxt = value;
                    NotifyOfPropertyChange(() => IsEnabledTxt);
                }
            }
        }
        private bool _Visibility = false;
        public bool Visibility
        {
            get
            {
                return _Visibility;
            }
            set
            {
                if (_Visibility != value)
                {
                    _Visibility = value;
                    NotifyOfPropertyChange(() => Visibility);
                }
            }
        }

        private bool _IsDoubleClick;
        public bool IsDoubleClick
        {
            get { return _IsDoubleClick; }
            set
            {
                if(_IsDoubleClick !=value)
                {
                    _IsDoubleClick = value;
                    NotifyOfPropertyChange(() => IsDoubleClick);
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
        public void GetSearchTreeView(long V_MedProductType)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            isLoadingSearch = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetSearchTreeView_DrugDept(FaName,V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetSearchTreeView_DrugDept(asyncResult);
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
                                /*▼====: #001*/
                                LoadFamilyParent(V_MedProductType);
                                /*▲====: #001*/
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
                            this.DlgHideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void LoadFamilyParent(long V_MedProductType)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            isLoadingGetID = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDrugDeptClassesParent(V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetDrugDeptClassesParent(asyncResult);
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

                                ParentDrugClass = results.ToObservableCollection();
                                NotifyOfPropertyChange(() => ParentDrugClass);
                                //cbx.ItemsSource = ParentDrugClass;
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
                            this.DlgHideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
            //_therapyCatalog.GetFamilytherapyParent();
        }


        public void treeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null)
            {
                CurrentTherapyTree = (TherapyTree)e.NewValue;
            }
        }
        /*▼====: #002*/
        public void treeView1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!IsDoubleClick || !(sender is TreeView) || (sender as TreeView).SelectedItem == null || !((sender as TreeView).SelectedItem is TherapyTree))
            {
                return;
            }
            long parentID = ((sender as TreeView).SelectedItem as TherapyTree).ParentID.Value;
            TherapyTree therapy = TreeViewTherapies.Where(x => x.NodeID == parentID).FirstOrDefault();
            Globals.EventAggregator.Publish(new ChooseDrugClass() { ParentSelected = therapy, ChildrenSelected = (sender as TreeView).SelectedItem as TherapyTree });

            this.TryClose();
        }


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
        


    }
}
