using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.ViewContracts;
using System.Windows.Controls;
using aEMR.Common.BaseModel;
using System.Linq;
using aEMR.Common.ExportExcel;
/*
* #001 20180921 TNHX: Apply BusyIndicator, refactor code
* 20230510 #002 DatTB: IssueID: 3254 | Thêm nút xuất excel cho các danh mục/cấu hình
* 20230601 #003 DatTB: IssueID: 3254 | Chỉnh sửa/Gộp các function xuất excel danh mục/cấu hình (Bỏ Func cũ)
*/
namespace aEMR.Configuration.AdmissionCriterion_Mgnt.ViewModels
{
    [Export(typeof(IAdmissionCriterion_ListFind)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class AdmissionCriterion_ListFindViewModel : ViewModelBase, IAdmissionCriterion_ListFind
        , IHandle<SymptomCategory_Event_Save>
        , IHandle<SaveEvent<AdmissionCriterion>>
        , IHandle<SaveEvent<GroupPCLs>>
    {
        protected override void OnActivate()
        {
            authorization();
            Debug.WriteLine("OnActivate");
            base.OnActivate();
        }
        protected override void OnDeactivate(bool close)
        {
            Debug.WriteLine("OnDeActivate");
            base.OnDeactivate(close);
        }

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public AdmissionCriterion_ListFindViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            authorization();

            SearchCriteria = "";
            SearchCriteriaAC = "";
            refIDC10Code = new ObservableCollection<DiseasesReference>();
            allAdmissionCriterionAttachICD = new ObservableCollection<AdmissionCriterionAttachICD>();
            allSymptomCategory = new ObservableCollection<SymptomCategory>();
            allGroupPCLs = new ObservableCollection<GroupPCLs>();
            listGroupPCLs = new ObservableCollection<GroupPCLs>();
            allAdmissionCriterionAttachSymptom_Required = new ObservableCollection<AdmissionCriterionAttachSymptom>();
            allAdmissionCriterionAttachSymptom_Not_Required = new ObservableCollection<AdmissionCriterionAttachSymptom>();
            allAdmissionCriterionAttachGroupPCL = new ObservableCollection<AdmissionCriterionAttachGroupPCL>();

            ObjSymptomCategory_Paging = new PagedSortableCollectionView<SymptomCategory>();
            ObjSymptomCategory_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjSymptomCategory_Paging_OnRefresh);

            ObjSymptomCategory_Paging.PageIndex = 0;
            SymptomCategory_Paging(ObjSymptomCategory_Paging.PageIndex, ObjSymptomCategory_Paging.PageSize, true);

            ObjAdmissionCriterion_Paging = new PagedSortableCollectionView<AdmissionCriterion>();
            ObjAdmissionCriterion_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjAdmissionCriterion_Paging_OnRefresh);

            ObjAdmissionCriterion_Paging.PageIndex = 0;
            ObjAdmissionCriterion_Paging.PageSize = 5;
            AdmissionCriterion_Paging(ObjAdmissionCriterion_Paging.PageIndex, ObjAdmissionCriterion_Paging.PageSize, true);

            ObjGroupPCLs_Paging = new PagedSortableCollectionView<GroupPCLs>();
            ObjGroupPCLs_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjGroupPCLs_Paging_OnRefresh);

            ObjGroupPCLs_Paging.PageIndex = 0;
            GroupPCLs_Paging(ObjGroupPCLs_Paging.PageIndex, ObjGroupPCLs_Paging.PageSize, true);
            

            GetAllSymptomByType();
            GetAllGroupPCLs();
        }

        void ObjSymptomCategory_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            SymptomCategory_Paging(ObjSymptomCategory_Paging.PageIndex,
                            ObjSymptomCategory_Paging.PageSize, false);
        }
        void ObjAdmissionCriterion_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            AdmissionCriterion_Paging(ObjAdmissionCriterion_Paging.PageIndex,
                            ObjAdmissionCriterion_Paging.PageSize, false);
        }
        void ObjGroupPCLs_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            GroupPCLs_Paging(ObjGroupPCLs_Paging.PageIndex,
                            ObjGroupPCLs_Paging.PageSize, false);
        }

        private Visibility _hplAddNewVisible = Visibility.Visible;
        public Visibility hplAddNewVisible
        {
            get { return _hplAddNewVisible; }
            set
            {
                _hplAddNewVisible = value;
                NotifyOfPropertyChange(() => hplAddNewVisible);
            }
        }

        private string _SearchCriteria;
        public string SearchCriteria
        {
            get
            {
                return _SearchCriteria;
            }
            set
            {
                _SearchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }

        private string _SearchCriteriaAC;
        public string SearchCriteriaAC
        {
            get
            {
                return _SearchCriteriaAC;
            }
            set
            {
                _SearchCriteriaAC = value;
                NotifyOfPropertyChange(() => SearchCriteriaAC);
            }
        }
        private string _SearchCriteriaGroupPCL;
        public string SearchCriteriaGroupPCL
        {
            get
            {
                return _SearchCriteriaGroupPCL;
            }
            set
            {
                _SearchCriteriaGroupPCL = value;
                NotifyOfPropertyChange(() => SearchCriteriaGroupPCL);
            }
        }

        private PagedSortableCollectionView<SymptomCategory> _ObjSymptomCategory_Paging;
        public PagedSortableCollectionView<SymptomCategory> ObjSymptomCategory_Paging
        {
            get { return _ObjSymptomCategory_Paging; }
            set
            {
                _ObjSymptomCategory_Paging = value;
                NotifyOfPropertyChange(() => ObjSymptomCategory_Paging);
            }
        }
        private PagedSortableCollectionView<AdmissionCriterion> _ObjAdmissionCriterion_Paging;
        public PagedSortableCollectionView<AdmissionCriterion> ObjAdmissionCriterion_Paging
        {
            get { return _ObjAdmissionCriterion_Paging; }
            set
            {
                _ObjAdmissionCriterion_Paging = value;
                NotifyOfPropertyChange(() => ObjAdmissionCriterion_Paging);
            }
        }
        private PagedSortableCollectionView<GroupPCLs> _ObjGroupPCLs_Paging;
        public PagedSortableCollectionView<GroupPCLs> ObjGroupPCLs_Paging
        {
            get { return _ObjGroupPCLs_Paging; }
            set
            {
                _ObjGroupPCLs_Paging = value;
                NotifyOfPropertyChange(() => ObjGroupPCLs_Paging);
            }
        }


        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            //bhplEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
            //                                   , (int)eConfiguration_Management.mDanhMucPhong,
            //                                   (int)oConfigurationEx.mQuanLyDanhSachPhong, (int)ePermission.mEdit);
            //bhplDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
            //                                   , (int)eConfiguration_Management.mDanhMucPhong,
            //                                   (int)oConfigurationEx.mQuanLyDanhSachPhong, (int)ePermission.mDelete);
            //bbtSearch = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
            //                            , (int)eConfiguration_Management.mDanhMucPhong,
            //                            (int)oConfigurationEx.mQuanLyDanhSachPhong, (int)ePermission.mView);
            //bhplAddNew = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
            //                                    , (int)eConfiguration_Management.mDanhMucPhong,
            //                                    (int)oConfigurationEx.mQuanLyDanhSachPhong, (int)ePermission.mAdd);
        }

        #region checking account
        private bool _bhplEdit = true;
        private bool _bhplDelete = true;
        private bool _bbtSearch = true;
        private bool _bhplAddNew = true;
        public bool bhplEdit
        {
            get
            {
                return _bhplEdit;
            }
            set
            {
                if (_bhplEdit == value)
                    return;
                _bhplEdit = value;
            }
        }

        public bool bhplDelete
        {
            get
            {
                return _bhplDelete;
            }
            set
            {
                if (_bhplDelete == value)
                    return;
                _bhplDelete = value;
            }
        }

        public bool bbtSearch
        {
            get
            {
                return _bbtSearch;
            }
            set
            {
                if (_bbtSearch == value)
                    return;
                _bbtSearch = value;
            }
        }

        public bool bhplAddNew
        {
            get
            {
                return _bhplAddNew;
            }
            set
            {
                if (_bhplAddNew == value)
                    return;
                _bhplAddNew = value;
            }
        }
        #endregion

        #region binding visibilty
        public Button hplEdit { get; set; }
        public Button hplDelete { get; set; }

        public void hplEdit_Loaded(object sender)
        {
            hplEdit = sender as Button;
            hplEdit.Visibility = Globals.convertVisibility(bhplEdit);
        }
        public void hplDelete_Loaded(object sender)
        {
            hplDelete = sender as Button;
            hplDelete.Visibility = Globals.convertVisibility(bhplDelete);
        }
        #endregion

        public void btSearch()
        {
            ObjSymptomCategory_Paging.PageIndex = 0;
            SymptomCategory_Paging(0, ObjSymptomCategory_Paging.PageSize, true);
        }
        public void btSearchAC()
        {
            ObjAdmissionCriterion_Paging.PageIndex = 0;
            AdmissionCriterion_Paging(0, ObjAdmissionCriterion_Paging.PageSize, true);
        }
        public void btSearchGroupPCL()
        {
            ObjGroupPCLs_Paging.PageIndex = 0;
            GroupPCLs_Paging(0, ObjGroupPCLs_Paging.PageSize, true);
        }

        public void AdmissionCriteria_MarkDeleted(AdmissionCriterion obj)
        {
            obj.IsDelete = true;
            string Result = "";
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Xóa..." });
            this.DlgShowBusyIndicator(eHCMSResources.Z0492_G1_DangXoa);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginAdmissionCriterion_Save(obj, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndAdmissionCriterion_Save(out Result, asyncResult);
                                //if (Result == "InUse")
                                //{
                                //    Globals.ShowMessage(eHCMSResources.Z1318_G1_PhgDangSDungKgTheXoa, eHCMSResources.G2617_G1_Xoa);
                                //}
                                if (Result == "Update-0")
                                {
                                    Globals.ShowMessage("Thất bại", "Thông báo");
                                }
                                if (Result == "Update-1")
                                {
                                    ObjAdmissionCriterion_Paging.PageIndex = 0;
                                    AdmissionCriterion_Paging(0, ObjAdmissionCriterion_Paging.PageSize, true);
                                    Globals.ShowMessage("Đã xóa"  , "Thông báo");
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }
        public void SymptomCategory_MarkDeleted(SymptomCategory obj)
        {
            obj.IsDelete = true;
            string Result = "";
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Xóa..." });
            this.DlgShowBusyIndicator(eHCMSResources.Z0492_G1_DangXoa);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSymptomCategory_Save(obj, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndSymptomCategory_Save(out Result, asyncResult);
                                //if (Result == "InUse")
                                //{
                                //    Globals.ShowMessage(eHCMSResources.Z1318_G1_PhgDangSDungKgTheXoa, eHCMSResources.G2617_G1_Xoa);
                                //}
                                if (Result == "Update-0")
                                {
                                    Globals.ShowMessage("Thất bại", "Thông báo");
                                }
                                if (Result == "Update-1")
                                {
                                    ObjSymptomCategory_Paging.PageIndex = 0;
                                    SymptomCategory_Paging(0, ObjSymptomCategory_Paging.PageSize, true);
                                    Globals.ShowMessage("Đã xóa"  , "Thông báo");
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }
        public void GroupPCLs_MarkDeleted(GroupPCLs obj)
        {
            obj.IsDelete = true;
            string Result = "";
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Xóa..." });
            this.DlgShowBusyIndicator(eHCMSResources.Z0492_G1_DangXoa);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGroupPCLs_Save(obj, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndGroupPCLs_Save(out Result, asyncResult);
                                //if (Result == "InUse")
                                //{
                                //    Globals.ShowMessage(eHCMSResources.Z1318_G1_PhgDangSDungKgTheXoa, eHCMSResources.G2617_G1_Xoa);
                                //}
                                if (Result == "Update-0")
                                {
                                    Globals.ShowMessage("Thất bại", "Thông báo");
                                }
                                if (Result == "Update-1")
                                {
                                    ObjGroupPCLs_Paging.PageIndex = 0;
                                    GroupPCLs_Paging(0, ObjGroupPCLs_Paging.PageSize, true);
                                    Globals.ShowMessage("Đã xóa"  , "Thông báo");
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }

        public void hplDelete_Click(object selectedItem)
        {
            SymptomCategory p = (selectedItem as SymptomCategory);
            if (p != null && p.SymptomCategoryID > 0)
            {
                if (MessageBox.Show(string.Format("Bạn có muốn xóa", p.SymptomCategoryName), "Xóa triệu chứng", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    SymptomCategory_MarkDeleted(p);
                }
            }
        }
        public void hplDeleteGroupPCL_Click(object selectedItem)
        {
            GroupPCLs p = (selectedItem as GroupPCLs);
            if (p != null && p.GroupPCLID > 0)
            {
                if (MessageBox.Show(string.Format("Bạn có muốn xóa", p.GroupPCLName), "Xóa nhóm", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    GroupPCLs_MarkDeleted(p);
                }
            }
        }

        private SymptomCategory _SelectedSymptomCategory;
        public SymptomCategory SelectedSymptomCategory
        {
            get { return _SelectedSymptomCategory; }
            set
            {
                _SelectedSymptomCategory = value;
                NotifyOfPropertyChange(() => SelectedSymptomCategory);
            }
        }
        private AdmissionCriterion _SelectedAdmissionCriterion;
        public AdmissionCriterion SelectedAdmissionCriterion
        {
            get { return _SelectedAdmissionCriterion; }
            set
            {
                _SelectedAdmissionCriterion = value;
                NotifyOfPropertyChange(() => SelectedAdmissionCriterion);
            }
        }

        private ObservableCollection<AdmissionCriterionAttachICD> _allAdmissionCriterionAttachICD;
        public ObservableCollection<AdmissionCriterionAttachICD> allAdmissionCriterionAttachICD
        {
            get
            {
                return _allAdmissionCriterionAttachICD;
            }
            set
            {
                if (_allAdmissionCriterionAttachICD == value)
                    return;
                _allAdmissionCriterionAttachICD = value;
                NotifyOfPropertyChange(() => allAdmissionCriterionAttachICD);
            }
        }

        private ObservableCollection<AdmissionCriterionAttachSymptom> _allAdmissionCriterionAttachSymptom_Required;
        public ObservableCollection<AdmissionCriterionAttachSymptom> allAdmissionCriterionAttachSymptom_Required
        {
            get
            {
                return _allAdmissionCriterionAttachSymptom_Required;
            }
            set
            {
                if (_allAdmissionCriterionAttachSymptom_Required == value)
                    return;
                _allAdmissionCriterionAttachSymptom_Required = value;
                NotifyOfPropertyChange(() => allAdmissionCriterionAttachSymptom_Required);
            }
        }

        private ObservableCollection<AdmissionCriterionAttachSymptom> _allAdmissionCriterionAttachSymptom_Not_Required;
        public ObservableCollection<AdmissionCriterionAttachSymptom> allAdmissionCriterionAttachSymptom_Not_Required
        {
            get
            {
                return _allAdmissionCriterionAttachSymptom_Not_Required;
            }
            set
            {
                if (_allAdmissionCriterionAttachSymptom_Not_Required == value)
                    return;
                _allAdmissionCriterionAttachSymptom_Not_Required = value;
                NotifyOfPropertyChange(() => allAdmissionCriterionAttachSymptom_Not_Required);
            }
        }
        private ObservableCollection<AdmissionCriterionAttachGroupPCL> _allAdmissionCriterionAttachGroupPCL;
        public ObservableCollection<AdmissionCriterionAttachGroupPCL> allAdmissionCriterionAttachGroupPCL
        {
            get
            {
                return _allAdmissionCriterionAttachGroupPCL;
            }
            set
            {
                if (_allAdmissionCriterionAttachGroupPCL == value)
                    return;
                _allAdmissionCriterionAttachGroupPCL = value;
                NotifyOfPropertyChange(() => allAdmissionCriterionAttachGroupPCL);
            }
        }
        private AdmissionCriterionAttachSymptom _selectedAdmissionCriterionAttachSymptom_Required;
        public AdmissionCriterionAttachSymptom selectedAdmissionCriterionAttachSymptom_Required
        {
            get
            {
                return _selectedAdmissionCriterionAttachSymptom_Required;
            }
            set
            {
                if (_selectedAdmissionCriterionAttachSymptom_Required == value)
                    return;
                _selectedAdmissionCriterionAttachSymptom_Required = value;
                NotifyOfPropertyChange(() => selectedAdmissionCriterionAttachSymptom_Required);
            }
        }

        private AdmissionCriterionAttachSymptom _selectedAdmissionCriterionAttachSymptom_Not_Required;
        public AdmissionCriterionAttachSymptom selectedAdmissionCriterionAttachSymptom_Not_Required
        {
            get
            {
                return _selectedAdmissionCriterionAttachSymptom_Not_Required;
            }
            set
            {
                if (_selectedAdmissionCriterionAttachSymptom_Not_Required == value)
                    return;
                _selectedAdmissionCriterionAttachSymptom_Not_Required = value;
                NotifyOfPropertyChange(() => selectedAdmissionCriterionAttachSymptom_Not_Required);
            }
        }
        private AdmissionCriterionAttachICD _selectedAdmissionCriterionAttachICD;
        public AdmissionCriterionAttachICD selectedAdmissionCriterionAttachICD
        {
            get
            {
                return _selectedAdmissionCriterionAttachICD;
            }
            set
            {
                if (_selectedAdmissionCriterionAttachICD == value)
                    return;
                _selectedAdmissionCriterionAttachICD = value;
                NotifyOfPropertyChange(() => selectedAdmissionCriterionAttachICD);
            }
        }
        private AdmissionCriterionAttachGroupPCL _selectedAdmissionCriterionAttachGroupPCL;
        public AdmissionCriterionAttachGroupPCL selectedAdmissionCriterionAttachGroupPCL
        {
            get
            {
                return _selectedAdmissionCriterionAttachGroupPCL;
            }
            set
            {
                if (_selectedAdmissionCriterionAttachGroupPCL == value)
                    return;
                _selectedAdmissionCriterionAttachGroupPCL = value;
                NotifyOfPropertyChange(() => selectedAdmissionCriterionAttachGroupPCL);
            }
        }
        private ObservableCollection<SymptomCategory> _allSymptomCategory;
        public ObservableCollection<SymptomCategory> allSymptomCategory
        {
            get { return _allSymptomCategory; }
            set
            {
                _allSymptomCategory = value;
                NotifyOfPropertyChange(() => allSymptomCategory);
            }
        }
        private ObservableCollection<SymptomCategory> _SymptomCategory_Required;
        public ObservableCollection<SymptomCategory> SymptomCategory_Required
        {
            get { return _SymptomCategory_Required; }
            set
            {
                _SymptomCategory_Required = value;
                NotifyOfPropertyChange(() => SymptomCategory_Required);
            }
        }
        private ObservableCollection<SymptomCategory> _SymptomCategory_Not_Required;
        public ObservableCollection<SymptomCategory> SymptomCategory_Not_Required
        {
            get { return _SymptomCategory_Not_Required; }
            set
            {
                _SymptomCategory_Not_Required = value;
                NotifyOfPropertyChange(() => SymptomCategory_Not_Required);
            }
        }
        private ObservableCollection<GroupPCLs> _allGroupPCLs;
        public ObservableCollection<GroupPCLs> allGroupPCLs
        {
            get { return _allGroupPCLs; }
            set
            {
                _allGroupPCLs = value;
                NotifyOfPropertyChange(() => allGroupPCLs);
            }
        }
        private ObservableCollection<GroupPCLs> _listGroupPCLs;
        public ObservableCollection<GroupPCLs> listGroupPCLs
        {
            get { return _listGroupPCLs; }
            set
            {
                _listGroupPCLs = value;
                NotifyOfPropertyChange(() => listGroupPCLs);
            }
        }
        private ObservableCollection<DiseasesReference> _refIDC10Code;
        public ObservableCollection<DiseasesReference> refIDC10Code
        {
            get
            {
                return _refIDC10Code;
            }
            set
            {
                if (_refIDC10Code != value)
                {
                    _refIDC10Code = value;
                }
                NotifyOfPropertyChange(() => refIDC10Code);
            }
        }
        private string _BasicDiagTreatment;
        public string BasicDiagTreatment
        {
            get
            {
                return _BasicDiagTreatment;
            }
            set
            {
                if (_BasicDiagTreatment != value)
                {
                    _BasicDiagTreatment = value;
                }
                NotifyOfPropertyChange(() => BasicDiagTreatment);
            }
        }
        AutoCompleteBox Acb_ICD10_Code = null;
        AutoCompleteBox Acb_Symptom_Required = null;
        AutoCompleteBox Acb_Symptom_Not_Required = null;
        AutoCompleteBox Acb_GroupPCLs = null;
        public void AcbICD10Code_Loaded(object sender, RoutedEventArgs e)
        {
            Acb_ICD10_Code = (AutoCompleteBox)sender;
        }
        public void AcbSymptom_Required_Loaded(object sender, RoutedEventArgs e)
        {
            Acb_Symptom_Required = (AutoCompleteBox)sender;
        }
        public void AcbSymptom_Not_Required_Loaded(object sender, RoutedEventArgs e)
        {
            Acb_Symptom_Not_Required = (AutoCompleteBox)sender;
        }
        public void AcbGroupPCLs_Loaded(object sender, RoutedEventArgs e)
        {
            Acb_GroupPCLs = (AutoCompleteBox)sender;
        }
        public void aucICD10_Populating(object sender, PopulatingEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Parameter))
            {
                LoadRefDiseases(e.Parameter, 0, 0, 100);
            }
        }
        public void aucSymptom_Required_Populating(object sender, PopulatingEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Parameter))
            {
                SymptomCategory_Required = allSymptomCategory.Where(x => x.V_SymptomType == (long)AllLookupValues.V_SymptomType.Bat_Buoc
                                                                 && x.SymptomCategoryName.ToUpper().Contains(e.Parameter.ToUpper())).ToObservableCollection();
                Acb_Symptom_Required.ItemsSource = SymptomCategory_Required;
                Acb_Symptom_Required.PopulateComplete();
            }
        }
        public void aucSymptom_Not_Required_Populating(object sender, PopulatingEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Parameter))
            {
                SymptomCategory_Not_Required = allSymptomCategory.Where(x => x.V_SymptomType == (long)AllLookupValues.V_SymptomType.Khong_BatBuoc 
                                                                && x.SymptomCategoryName.ToUpper().Contains(e.Parameter.ToUpper())).ToObservableCollection();
                Acb_Symptom_Not_Required.ItemsSource = SymptomCategory_Not_Required;
                Acb_Symptom_Not_Required.PopulateComplete();
            }
        }
        public void aucGroupPCLs_Populating(object sender, PopulatingEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Parameter))
            {
                listGroupPCLs = allGroupPCLs.Where(x => x.GroupPCLName.ToUpper().Contains(e.Parameter.ToUpper())).ToObservableCollection();
                Acb_GroupPCLs.ItemsSource = listGroupPCLs;
                Acb_GroupPCLs.PopulateComplete();
            }
        }
        private bool isDropDown = false;
        private bool isDropDown_Symptom_Required = false;
        private bool isDropDown_Symptom_Not_Required = false;
        private bool isDropDown_GroupPCLs = false;
        public void AxAutoComplete_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (!isDropDown)
            {
                return;
            }
            isDropDown = false;
            if (Acb_ICD10_Code != null)
            {
                DiseasesReference BDiagTreatment = Acb_ICD10_Code.SelectedItem as DiseasesReference;
                if (BDiagTreatment != null)
                {
                    BasicDiagTreatment = BDiagTreatment.DiseaseNameVN;
                }
            }
        }
        public void AxAutoComplete_Symptom_Required_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (!isDropDown_Symptom_Required)
            {
                return;
            }
            isDropDown_Symptom_Required = false;
        }
        public void AxAutoComplete_Symptom_Not_Required_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (!isDropDown_Symptom_Not_Required)
            {
                return;
            }
            isDropDown_Symptom_Not_Required = false;
        }
        public void AxAutoComplete_GroupPCLs_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (!isDropDown_GroupPCLs)
            {
                return;
            }
            isDropDown_GroupPCLs = false;
        }
        public void AxAutoComplete_DropDownClosing(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            isDropDown = true;
        }
        public void AxAutoComplete_Symptom_Required_DropDownClosing(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            isDropDown_Symptom_Required = true;
        }
        public void AxAutoComplete_Symptom_Not_Required_DropDownClosing(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            isDropDown_Symptom_Not_Required = true;
        }
        public void AxAutoComplete_GroupPCLs_DropDownClosing(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            isDropDown_GroupPCLs = true;
        }
        public void LoadRefDiseases(string name, byte type, int PageIndex, int PageSize)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginSearchRefDiseases(name, PageIndex, PageSize, type
                        , 0
                        , Globals.GetCurServerDateTime()
                        , Globals.DispatchCallback((asyncResult) =>
                        {

                            try
                            {
                                int Total = 10;
                                var results = contract.EndSearchRefDiseases(out Total, asyncResult);
                                refIDC10Code.Clear();
                                if (results != null)
                                {
                                    foreach (DiseasesReference p in results)
                                    {
                                        refIDC10Code.Add(p);
                                    }
                                }
                                Acb_ICD10_Code.ItemsSource = refIDC10Code;
                                Acb_ICD10_Code.PopulateComplete();
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
        private void GetAllSymptomByType()
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetAllSymptom(Globals.DispatchCallback((asyncResult) =>
                        {

                            IList<SymptomCategory> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndGetAllSymptom(asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }

                            allSymptomCategory.Clear();

                            if (bOK)
                            {
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        allSymptomCategory.Add(item);
                                    }
                                }
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
        private void GetAllGroupPCLs()
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetAllGroupPCLs(Globals.DispatchCallback((asyncResult) =>
                        {

                            IList<GroupPCLs> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndGetAllGroupPCLs(asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }

                            allGroupPCLs.Clear();

                            if (bOK)
                            {
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        allGroupPCLs.Add(item);
                                    }
                                }
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
    
        private void SymptomCategory_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            this.DlgShowBusyIndicator("Danh sách triệu chứng");
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginSymptomCategory_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<SymptomCategory> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndSymptomCategory_Paging(out Total, asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }

                            ObjSymptomCategory_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjSymptomCategory_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjSymptomCategory_Paging.Add(item);
                                    }
                                }
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
        private void AdmissionCriterion_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            this.DlgShowBusyIndicator("Danh sách triệu chứng");
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginAdmissionCriterion_Paging(SearchCriteriaAC, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<AdmissionCriterion> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndAdmissionCriterion_Paging(out Total, asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }

                            ObjAdmissionCriterion_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjAdmissionCriterion_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjAdmissionCriterion_Paging.Add(item);
                                    }
                                }
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
        private void GroupPCLs_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            this.DlgShowBusyIndicator("Danh sách nhóm cận lâm sàng");
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGroupPCLs_Paging(SearchCriteriaGroupPCL, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<GroupPCLs> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndGroupPCLs_Paging(out Total, asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }

                            ObjGroupPCLs_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjGroupPCLs_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjGroupPCLs_Paging.Add(item);
                                    }
                                }
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
        private void GetICDListByAdmissionCriterionID(long CriterionID)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetICDListByAdmissionCriterionID(CriterionID, Globals.DispatchCallback((asyncResult) =>
                        {
                          
                            IList<AdmissionCriterionAttachICD> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndGetICDListByAdmissionCriterionID(asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }

                            allAdmissionCriterionAttachICD.Clear();

                            if (bOK)
                            {
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        allAdmissionCriterionAttachICD.Add(item);
                                    }
                                }
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
        private void GetGroupPCLListByAdmissionCriterionID(long CriterionID)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetGroupPCLListByAdmissionCriterionID(CriterionID, Globals.DispatchCallback((asyncResult) =>
                        {

                            IList<AdmissionCriterionAttachGroupPCL> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndGetGroupPCLListByAdmissionCriterionID(asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }

                            allAdmissionCriterionAttachGroupPCL.Clear();

                            if (bOK)
                            {
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        allAdmissionCriterionAttachGroupPCL.Add(item);
                                    }
                                }
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
        private void InsertAdmissionCriterionAttachICD(List<AdmissionCriterionAttachICD> listObj)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginInsertAdmissionCriterionAttachICD(listObj, Globals.LoggedUserAccount.StaffID.Value, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndInsertAdmissionCriterionAttachICD(asyncResult);
                            if (results == true)
                            {
                                GetICDListByAdmissionCriterionID(SelectedAdmissionCriterion.AdmissionCriterionID);
                                MessageBox.Show(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK);
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

            });

            t.Start();
        }
        private void InsertAdmissionCriterionAttachSymptom(List<AdmissionCriterionAttachSymptom> listObj,long V_SymptomType)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginInsertAdmissionCriterionAttachSymptom(listObj, Globals.LoggedUserAccount.StaffID.Value, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndInsertAdmissionCriterionAttachSymptom(asyncResult);
                            if (results == true)
                            {
                                GetSymptomListByAdmissionCriterionID(SelectedAdmissionCriterion.AdmissionCriterionID, V_SymptomType);
                                MessageBox.Show(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK);
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

            });

            t.Start();
        }
        private void InsertAdmissionCriterionAttachGroupPCL(List<AdmissionCriterionAttachGroupPCL> listObj)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginInsertAdmissionCriterionAttachGroupPCL(listObj, Globals.LoggedUserAccount.StaffID.Value, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndInsertAdmissionCriterionAttachGroupPCL(asyncResult);
                            if (results == true)
                            {
                                GetGroupPCLListByAdmissionCriterionID(SelectedAdmissionCriterion.AdmissionCriterionID);
                                MessageBox.Show(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK);
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

            });

            t.Start();
        }
        private void DeleteAdmissionCriterionAttachICD(long ACAI_ID)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDeleteAdmissionCriterionAttachICD(ACAI_ID,  Globals.LoggedUserAccount.StaffID.Value, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndDeleteAdmissionCriterionAttachICD(asyncResult);
                            if (results == true)
                            {
                                MessageBox.Show(eHCMSResources.K0537_G1_XoaOk);
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

            });

            t.Start();
        }
        private void DeleteAdmissionCriterionAttachSymptom(long ACAS_ID)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDeleteAdmissionCriterionAttachSymptom(ACAS_ID,  Globals.LoggedUserAccount.StaffID.Value, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndDeleteAdmissionCriterionAttachSymptom(asyncResult);
                            if (results == true)
                            {
                                MessageBox.Show(eHCMSResources.K0537_G1_XoaOk);
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

            });

            t.Start();
        }
        private void DeleteAdmissionCriterionAttachGroupPCL(long ACAG_ID)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDeleteAdmissionCriterionAttachGroupPCL(ACAG_ID, Globals.LoggedUserAccount.StaffID.Value, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndDeleteAdmissionCriterionAttachGroupPCL(asyncResult);
                            if (results == true)
                            {
                                MessageBox.Show(eHCMSResources.K0537_G1_XoaOk);
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

            });

            t.Start();
        }
        private void GetSymptomListByAdmissionCriterionID(long CriterionID,long V_SymptomType = 0)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetSymptomListByAdmissionCriterionID(CriterionID, Globals.DispatchCallback((asyncResult) =>
                        {

                            IList<AdmissionCriterionAttachSymptom> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndGetSymptomListByAdmissionCriterionID(asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                            if(V_SymptomType == (long)AllLookupValues.V_SymptomType.Bat_Buoc)
                            {
                                allAdmissionCriterionAttachSymptom_Required.Clear();
                            }
                            else if (V_SymptomType == (long)AllLookupValues.V_SymptomType.Khong_BatBuoc)
                            {
                                allAdmissionCriterionAttachSymptom_Not_Required.Clear();
                            }
                            else
                            {
                                allAdmissionCriterionAttachSymptom_Required.Clear();
                                allAdmissionCriterionAttachSymptom_Not_Required.Clear();
                            }

                            if (bOK)
                            {
                                if (allItems != null)
                                {
                                    if (V_SymptomType == (long)AllLookupValues.V_SymptomType.Bat_Buoc)
                                    {
                                        foreach (var item in allItems.Where(x => x.V_SymptomType == (long)AllLookupValues.V_SymptomType.Bat_Buoc))
                                        {
                                            allAdmissionCriterionAttachSymptom_Required.Add(item);
                                        }
                                    }
                                    else if (V_SymptomType == (long)AllLookupValues.V_SymptomType.Khong_BatBuoc)
                                    {
                                        foreach (var item in allItems.Where(x => x.V_SymptomType == (long)AllLookupValues.V_SymptomType.Khong_BatBuoc))
                                        {
                                            allAdmissionCriterionAttachSymptom_Not_Required.Add(item);
                                        }
                                    }
                                    else
                                    {
                                        foreach (var item in allItems.Where(x => x.V_SymptomType == (long)AllLookupValues.V_SymptomType.Bat_Buoc))
                                        {
                                            allAdmissionCriterionAttachSymptom_Required.Add(item);
                                        }
                                        foreach (var item in allItems.Where(x => x.V_SymptomType == (long)AllLookupValues.V_SymptomType.Khong_BatBuoc))
                                        {
                                            allAdmissionCriterionAttachSymptom_Not_Required.Add(item);
                                        }
                                    }
                                }
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
        public void AdmissionCriterionSelectionChanged()
        {
            if(SelectedAdmissionCriterion == null)
            {
                return;
            }
            GetICDListByAdmissionCriterionID(SelectedAdmissionCriterion.AdmissionCriterionID);
            GetSymptomListByAdmissionCriterionID(SelectedAdmissionCriterion.AdmissionCriterionID);
            GetGroupPCLListByAdmissionCriterionID(SelectedAdmissionCriterion.AdmissionCriterionID);
        }

        public void hplAddNew_Click()
        {
            Action<ISymptomCategory_AddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = "THÊM MỚI TRIỆU CHỨNG";
                typeInfo.InitializeNewItem();
            };
            GlobalsNAV.ShowDialog(onInitDlg);
        }
        public void hplAddNewAC_Click()
        {
            Action<IAdmissionCriterion_AddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = "THÊM MỚI TIÊU CHUẨN";
                typeInfo.InitializeNewItem();
            };
            GlobalsNAV.ShowDialog(onInitDlg);
        }
        public void hplAddNewGroupPCL_Click()
        {
            Action<IGroupPCLs_AddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = "THÊM MỚI NHÓM CẬN LÂM SÀNG";
                typeInfo.ObjGroupPCLs_Current = new GroupPCLs(); 
                typeInfo.InitializeItem();
            };
            GlobalsNAV.ShowDialog(onInitDlg);
        }

        public void hplEdit_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                Action<ISymptomCategory_AddEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.ObjSymptomCategory_Current = ObjectCopier.DeepCopy((selectedItem as SymptomCategory));
                    typeInfo.TitleForm = "HIỆU CHỈNH";
                };
                GlobalsNAV.ShowDialog<ISymptomCategory_AddEdit>(onInitDlg);
            }
        }
        public void hplEditAC_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                Action<IAdmissionCriterion_AddEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.ObjAdmissionCriterion_Current = ObjectCopier.DeepCopy((selectedItem as AdmissionCriterion));
                    typeInfo.TitleForm = "HIỆU CHỈNH";
                };
                GlobalsNAV.ShowDialog<IAdmissionCriterion_AddEdit>(onInitDlg);
            }
        }
        public void hplDeleteAC_Click(object selectedItem)
        {
            if (SelectedAdmissionCriterion != null && SelectedAdmissionCriterion.AdmissionCriterionID != 0)
            {
                if (MessageBox.Show("Bạn có muốn xóa tiêu chuẩn " + SelectedAdmissionCriterion.AdmissionCriterionName, "Thông báo", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    AdmissionCriteria_MarkDeleted(SelectedAdmissionCriterion);
                }
            }
        }
        public void hplEditGroupPCL_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                Action<IGroupPCLs_AddEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.ObjGroupPCLs_Current = ObjectCopier.DeepCopy((selectedItem as GroupPCLs));
                    typeInfo.InitializeItem();
                    typeInfo.TitleForm = "HIỆU CHỈNH";
                };
                GlobalsNAV.ShowDialog<IGroupPCLs_AddEdit>(onInitDlg);
            }
        }
        public void butAdd()
        {
            if (SelectedAdmissionCriterion == null)
            {
                MessageBox.Show("Chưa chọn tiêu chuẩn nhập viện");
                return;
            }
            if (Acb_ICD10_Code.SelectedItem != null && (Acb_ICD10_Code.SelectedItem as DiseasesReference).IDCode > 0)
            {
                foreach (var item in allAdmissionCriterionAttachICD)
                {
                    if (item.ICD10Code == (Acb_ICD10_Code.SelectedItem as DiseasesReference).ICD10Code)
                    {
                        MessageBox.Show(eHCMSResources.Z2627_G1_DaTonTaiICD10Nay);
                        return;
                    }
                }
                allAdmissionCriterionAttachICD.Add(new AdmissionCriterionAttachICD {
                    AdmissionCriterionID = SelectedAdmissionCriterion.AdmissionCriterionID,
                    IDCode = (Acb_ICD10_Code.SelectedItem as DiseasesReference).IDCode,
                    ICD10Code = (Acb_ICD10_Code.SelectedItem as DiseasesReference).ICD10Code,
                    DiseaseNameVN = (Acb_ICD10_Code.SelectedItem as DiseasesReference).DiseaseNameVN
                });
                //AddNewAdmissionCriterionAttachICD(SelectedAdmissionCriterion.AdmissionCriterionID, (Acb_ICD10_Code.SelectedItem as DiseasesReference).IDCode, (Acb_ICD10_Code.SelectedItem as DiseasesReference).ICD10Code);
                BasicDiagTreatment = "";
                Acb_ICD10_Code.Text = "";
            }
            else
            {
                MessageBox.Show(eHCMSResources.Z2623_G1_ChuaChonICD10);
            }
        }
        public void butAddSCItem_Required()
        {
            if (SelectedAdmissionCriterion == null)
            {
                MessageBox.Show("Chưa chọn tiêu chuẩn nhập viện");
                return;
            }
            if (Acb_Symptom_Required.SelectedItem != null && (Acb_Symptom_Required.SelectedItem as SymptomCategory).SymptomCategoryID > 0)
            {
                foreach (var item in allAdmissionCriterionAttachSymptom_Required)
                {
                    if (item.SymptomCategoryID == (Acb_Symptom_Required.SelectedItem as SymptomCategory).SymptomCategoryID)
                    {
                        MessageBox.Show("Đã tồn tại triệu chứng này");
                        return;
                    }
                }
                allAdmissionCriterionAttachSymptom_Required.Add(new AdmissionCriterionAttachSymptom {
                    AdmissionCriterionID = SelectedAdmissionCriterion.AdmissionCriterionID,
                    SymptomCategoryID = (Acb_Symptom_Required.SelectedItem as SymptomCategory).SymptomCategoryID,
                    SymptomCategoryName = (Acb_Symptom_Required.SelectedItem as SymptomCategory).SymptomCategoryName
                });
                //AddNewAdmissionCriterionAttachICD(SelectedAdmissionCriterion.AdmissionCriterionID, (Acb_ICD10_Code.SelectedItem as DiseasesReference).IDCode, (Acb_ICD10_Code.SelectedItem as DiseasesReference).ICD10Code);
                Acb_Symptom_Required.Text = "";
            }
            else
            {
                MessageBox.Show("Chưa chọn triệu chứng!");
            }
        }
        public void butAddSCItem_Not_Required()
        {
            if (SelectedAdmissionCriterion == null)
            {
                MessageBox.Show("Chưa chọn tiêu chuẩn nhập viện");
                return;
            }
            if (Acb_Symptom_Not_Required.SelectedItem != null && (Acb_Symptom_Not_Required.SelectedItem as SymptomCategory).SymptomCategoryID > 0)
            {
                foreach (var item in allAdmissionCriterionAttachSymptom_Not_Required)
                {
                    if (item.SymptomCategoryID == (Acb_Symptom_Not_Required.SelectedItem as SymptomCategory).SymptomCategoryID)
                    {
                        MessageBox.Show("Đã tồn tại triệu chứng này");
                        return;
                    }
                }
                allAdmissionCriterionAttachSymptom_Not_Required.Add(new AdmissionCriterionAttachSymptom {
                    AdmissionCriterionID = SelectedAdmissionCriterion.AdmissionCriterionID,
                    SymptomCategoryID = (Acb_Symptom_Not_Required.SelectedItem as SymptomCategory).SymptomCategoryID,
                    SymptomCategoryName = (Acb_Symptom_Not_Required.SelectedItem as SymptomCategory).SymptomCategoryName
                });
                //AddNewAdmissionCriterionAttachICD(SelectedAdmissionCriterion.AdmissionCriterionID, (Acb_ICD10_Code.SelectedItem as DiseasesReference).IDCode, (Acb_ICD10_Code.SelectedItem as DiseasesReference).ICD10Code);
                Acb_Symptom_Not_Required.Text = "";
            }
            else
            {
                MessageBox.Show("Chưa chọn triệu chứng!");
            }
        }
        public void butAddGroupPCL()
        {
            if (SelectedAdmissionCriterion == null)
            {
                MessageBox.Show("Chưa chọn tiêu chuẩn nhập viện");
                return;
            }
            if (Acb_GroupPCLs.SelectedItem != null && (Acb_GroupPCLs.SelectedItem as GroupPCLs).GroupPCLID > 0)
            {
                foreach (var item in allAdmissionCriterionAttachGroupPCL)
                {
                    if (item.GroupPCLID == (Acb_GroupPCLs.SelectedItem as GroupPCLs).GroupPCLID)
                    {
                        MessageBox.Show("Đã tồn tại nhóm này");
                        return;
                    }
                }
                allAdmissionCriterionAttachGroupPCL.Add(new AdmissionCriterionAttachGroupPCL
                {
                    AdmissionCriterionID = SelectedAdmissionCriterion.AdmissionCriterionID,
                    GroupPCLID = (Acb_GroupPCLs.SelectedItem as GroupPCLs).GroupPCLID,
                    GroupPCLName = (Acb_GroupPCLs.SelectedItem as GroupPCLs).GroupPCLName
                });
                //AddNewAdmissionCriterionAttachICD(SelectedAdmissionCriterion.AdmissionCriterionID, (Acb_ICD10_Code.SelectedItem as DiseasesReference).IDCode, (Acb_ICD10_Code.SelectedItem as DiseasesReference).ICD10Code);
                Acb_GroupPCLs.Text = "";
            }
            else
            {
                MessageBox.Show("Chưa chọn triệu chứng!");
            }
        }
        public void butSave()
        {
            if(allAdmissionCriterionAttachICD == null || allAdmissionCriterionAttachICD.Where(x=>x.ACAI_ID ==0).Count()==0)
            {
                MessageBox.Show(eHCMSResources.Z2299_G1_KhongCoGiDeLuu);
                return;
            }
            InsertAdmissionCriterionAttachICD(allAdmissionCriterionAttachICD.Where(x => x.ACAI_ID == 0).ToList());
        }
        public void butSaveSCItem_Required()
        {
            if(allAdmissionCriterionAttachSymptom_Required == null || allAdmissionCriterionAttachSymptom_Required.Where(x=>x.ACAS_ID ==0).Count()==0)
            {
                MessageBox.Show(eHCMSResources.Z2299_G1_KhongCoGiDeLuu);
                return;
            }
            InsertAdmissionCriterionAttachSymptom(allAdmissionCriterionAttachSymptom_Required.Where(x => x.ACAS_ID == 0).ToList(),(long)AllLookupValues.V_SymptomType.Bat_Buoc);
        }
        public void butSaveSCItem_Not_Required()
        {
            if(allAdmissionCriterionAttachSymptom_Not_Required == null || allAdmissionCriterionAttachSymptom_Not_Required.Where(x=>x.ACAS_ID == 0).Count()==0)
            {
                MessageBox.Show(eHCMSResources.Z2299_G1_KhongCoGiDeLuu);
                return;
            }
            InsertAdmissionCriterionAttachSymptom(allAdmissionCriterionAttachSymptom_Not_Required.Where(x => x.ACAS_ID == 0).ToList(),(long)AllLookupValues.V_SymptomType.Khong_BatBuoc);
        }
        public void butSaveGroupPCL()
        {
            if(allAdmissionCriterionAttachGroupPCL == null || allAdmissionCriterionAttachGroupPCL.Where(x=>x.ACAG_ID == 0).Count()==0)
            {
                MessageBox.Show(eHCMSResources.Z2299_G1_KhongCoGiDeLuu);
                return;
            }
            InsertAdmissionCriterionAttachGroupPCL(allAdmissionCriterionAttachGroupPCL.Where(x => x.ACAG_ID == 0).ToList());
        }
        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if(selectedAdmissionCriterionAttachICD == null)
            {
                return;
            }
            if(MessageBox.Show("Bạn có muốn xóa ICD "+ selectedAdmissionCriterionAttachICD.ICD10Code, "Thông báo",MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (selectedAdmissionCriterionAttachICD.ACAI_ID != 0)
                {
                    DeleteAdmissionCriterionAttachICD(selectedAdmissionCriterionAttachICD.ACAI_ID);
                }
                allAdmissionCriterionAttachICD.Remove(selectedAdmissionCriterionAttachICD);
            }
        }
        public void lnkDeleteSC_Required_Click(object sender, RoutedEventArgs e)
        {
            if(selectedAdmissionCriterionAttachSymptom_Required == null)
            {
                return;
            }
            if(MessageBox.Show("Bạn có muốn xóa triệu chứng "+ selectedAdmissionCriterionAttachSymptom_Required.SymptomCategoryName, "Thông báo",MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (selectedAdmissionCriterionAttachSymptom_Required.ACAS_ID != 0)
                {
                    DeleteAdmissionCriterionAttachSymptom(selectedAdmissionCriterionAttachSymptom_Required.ACAS_ID);
                }
                allAdmissionCriterionAttachSymptom_Required.Remove(selectedAdmissionCriterionAttachSymptom_Required);
            }
        }
        public void lnkDeleteSC_Not_Required_Click(object sender, RoutedEventArgs e)
        {
            if(selectedAdmissionCriterionAttachSymptom_Not_Required == null)
            {
                return;
            }
            if(MessageBox.Show("Bạn có muốn xóa triệu chứng "+ selectedAdmissionCriterionAttachSymptom_Not_Required.SymptomCategoryName, "Thông báo",MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (selectedAdmissionCriterionAttachSymptom_Not_Required.ACAS_ID != 0)
                {
                    DeleteAdmissionCriterionAttachSymptom(selectedAdmissionCriterionAttachSymptom_Not_Required.ACAS_ID);
                }
                allAdmissionCriterionAttachSymptom_Not_Required.Remove(selectedAdmissionCriterionAttachSymptom_Not_Required);
            }
        }
        public void lnkDeleteGroupPCLs_Click(object sender, RoutedEventArgs e)
        {
            if(selectedAdmissionCriterionAttachGroupPCL == null)
            {
                return;
            }
            if(MessageBox.Show("Bạn có muốn xóa nhóm "+ selectedAdmissionCriterionAttachGroupPCL.GroupPCLName, "Thông báo",MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (selectedAdmissionCriterionAttachGroupPCL.ACAG_ID != 0)
                {
                    DeleteAdmissionCriterionAttachGroupPCL(selectedAdmissionCriterionAttachGroupPCL.ACAG_ID);
                }
                allAdmissionCriterionAttachGroupPCL.Remove(selectedAdmissionCriterionAttachGroupPCL);
            }
        }

        private void AddNewAdmissionCriterionAttachICD(long SelectedSymptomCategory,long IDCode, string ICD10Code)
        {
            
        }
        public void Handle(SymptomCategory_Event_Save message)
        {
            ObjSymptomCategory_Paging.PageIndex = 0;
            SymptomCategory_Paging(0, ObjSymptomCategory_Paging.PageSize, true);
            GetAllSymptomByType();
        }
        public void Handle(SaveEvent<AdmissionCriterion> message)
        {
            ObjAdmissionCriterion_Paging.PageIndex = 0;
            AdmissionCriterion_Paging(0, ObjAdmissionCriterion_Paging.PageSize, true);
        }
        public void Handle(SaveEvent<GroupPCLs> message)
        {
            ObjGroupPCLs_Paging.PageIndex = 0;
            GroupPCLs_Paging(0, ObjGroupPCLs_Paging.PageSize, true);
            GetAllGroupPCLs();
        }

        //▼==== #003
        public void BtnExportExcelICD()
        {
            long AdmissionCriterionID = 0;
            if (SelectedAdmissionCriterion != null)
            {
                AdmissionCriterionID = SelectedAdmissionCriterion.AdmissionCriterionID;
            }
            this.ShowBusyIndicator(eHCMSResources.Z0669_G1_DangLayDLieu);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        ConfigurationReportParams Params = new ConfigurationReportParams()
                        {
                            ConfigurationName = ConfigurationName.AdmissionCriterionAttachICD,
                            AdmissionCriterionID = AdmissionCriterionID
                        };

                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginExportExcelConfigurationManager(Params, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var results = contract.EndExportExcelConfigurationManager(asyncResult);
                                ExportToExcelFileAllData.Export(results, "Shee1");
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
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        public void BtnExportExcelGroupPCL()
        {
            long AdmissionCriterionID = 0;
            if (SelectedAdmissionCriterion != null)
            {
                AdmissionCriterionID = SelectedAdmissionCriterion.AdmissionCriterionID;
            }
            this.ShowBusyIndicator(eHCMSResources.Z0669_G1_DangLayDLieu);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        ConfigurationReportParams Params = new ConfigurationReportParams()
                        {
                            ConfigurationName = ConfigurationName.AdmissionCriterionAttachGroupPCL,
                            AdmissionCriterionID = AdmissionCriterionID
                        };

                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginExportExcelConfigurationManager(Params, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var results = contract.EndExportExcelConfigurationManager(asyncResult);
                                ExportToExcelFileAllData.Export(results, "Shee1");
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
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        public void BtnExportExcelSymptom()
        {
            long AdmissionCriterionID = 0;
            if (SelectedAdmissionCriterion != null)
            {
                AdmissionCriterionID = SelectedAdmissionCriterion.AdmissionCriterionID;
            }
            this.ShowBusyIndicator(eHCMSResources.Z0669_G1_DangLayDLieu);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        ConfigurationReportParams Params = new ConfigurationReportParams()
                        {
                            ConfigurationName = ConfigurationName.AdmissionCriterionAttachSymptom,
                            AdmissionCriterionID = AdmissionCriterionID
                        };

                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginExportExcelConfigurationManager(Params, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var results = contract.EndExportExcelConfigurationManager(asyncResult);
                                ExportToExcelFileAllData.Export(results, "Shee1");
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
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        //▲==== #003
    }
}
