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
using aEMR.Common.Utilities;
using ObjectCopier = aEMR.Common.ObjectCopier;
using aEMR.Common.ExportExcel;
/*
* #001 20180921 TNHX: Apply BusyIndicator, refactor code
* 20230509 #002 DatTB: IssueID: 3254 | Thêm nút xuất excel cho các danh mục/cấu hình
* 20230601 #003 DatTB: IssueID: 3254 | Chỉnh sửa/Gộp các function xuất excel danh mục/cấu hình (Bỏ Func cũ)
*/
namespace aEMR.Configuration.ICDList.ViewModels
{
    [Export(typeof(IICD_ListFind)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ICD_ListFindViewModel : ViewModelBase, IICD_ListFind
        , IHandle<ICD_Event_Save>
        , IHandle<Chapter_Event_Save>
        , IHandle<Diseases_Event_Save>
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
        public ICD_ListFindViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            authorization();
            Globals.EventAggregator.Subscribe(this);

            DiseaseChapters = new ObservableCollection<DiseaseChapters>();
            DiseaseChapters_GetAll();
            ICD_GetAll();


            SearchCriteria = new ICDSearchCriteria();
            SearchCriteria.ICD10Code = "";
            SearchCriteria.DiseaseNameVN = "";
            SearchCriteria.OrderBy = "";

            SearchChapter = "";

            ObjICD_ByIDCode_Paging = new PagedSortableCollectionView<ICD>();
            ObjICD_ByIDCode_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjICD_ByIDCode_Paging_OnRefresh);
            ObjChapter_Paging = new PagedSortableCollectionView<DiseaseChapters>();
            ObjChapter_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjChapter_Paging_OnRefresh);
            ObjDiseases_Paging = new PagedSortableCollectionView<Diseases>();
            ObjDiseases_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjDiseases_Paging_OnRefresh);
            ObjICD_AutoComplete = new PagedSortableCollectionView<ICD>();
            ObjICD_ADD = new PagedSortableCollectionView<ICD>();
            ObjICD_Display = new PagedSortableCollectionView<ICD>();
            ObjICD_Display.OnRefresh += new EventHandler<RefreshEventArgs>(ObjICD_Display_OnRefresh);
        }

        void ObjICD_ByIDCode_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            ICD_ByIDCode_Paging(ObjICD_ByIDCode_Paging.PageIndex, ObjICD_ByIDCode_Paging.PageSize, false);
        }
        void ObjChapter_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            Chapter_Paging(ObjChapter_Paging.PageIndex, ObjChapter_Paging.PageSize, false);
        }
        void ObjDiseases_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            Diseases_Paging(ObjDiseases_Paging.PageIndex, ObjDiseases_Paging.PageSize, false);
        }
        void ObjICD_Display_OnRefresh(object sender, RefreshEventArgs e)
        {
            ICD_ByDiseaseID_Paging(ObjICD_Display.PageIndex, ObjICD_Display.PageSize, true);
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
        private ObservableCollection<DiseaseChapters> _DiseaseChapters;
        public ObservableCollection<DiseaseChapters> DiseaseChapters
        {
            get { return _DiseaseChapters; }
            set
            {
                _DiseaseChapters = value;
                NotifyOfPropertyChange(() => DiseaseChapters);
            }
        }
        private int _DiseaseChapters_ID = 0;
        public int DiseaseChapters_ID
        {
            get { return _DiseaseChapters_ID; }
            set
            {
                _DiseaseChapters_ID = value;
                NotifyOfPropertyChange(() => DiseaseChapters_ID);
            }
        }
        private ObservableCollection<Diseases> _Diseases;
        public ObservableCollection<Diseases> Diseases
        {
            get { return _Diseases; }
            set
            {
                _Diseases = value;
                NotifyOfPropertyChange(() => Diseases);
            }
        }
        private PagedSortableCollectionView<ICD> _ObjICD_Display;
        public PagedSortableCollectionView<ICD> ObjICD_Display
        {
            get { return _ObjICD_Display; }
            set
            {
                _ObjICD_Display = value;
                NotifyOfPropertyChange(() => ObjICD_Display);
            }
        }
        private ObservableCollection<ICD> _ObjICD_ADD;
        public ObservableCollection<ICD> ObjICD_ADD
        {
            get { return _ObjICD_ADD; }
            set
            {
                _ObjICD_ADD = value;
                NotifyOfPropertyChange(() => ObjICD_ADD);
            }
        }
        
        private long _Disease_ID = 0;
        public long Disease_ID
        {
            get { return _Disease_ID; }
            set
            {
                _Disease_ID = value;
                NotifyOfPropertyChange(() => Disease_ID);
            }
        }
        private string _ICD10Code = "";
        public string ICD10Code
        {
            get { return _ICD10Code; }
            set
            {
                _ICD10Code = value;
                NotifyOfPropertyChange(() => ICD10Code);
            }
        }
        AutoCompleteBox Acb_ICD10_Code = null;
        public Dictionary<long, ICD> _dicICD;
        public Dictionary<long, ICD> dicICD
        {
            get
            {
                return _dicICD;
            }
            set
            {
                _dicICD = value;
                NotifyOfPropertyChange(() => dicICD);

            }
        }
        public void AcbICD10Code_Loaded(object sender, RoutedEventArgs e)
        {
            Acb_ICD10_Code = (AutoCompleteBox)sender;
        }

        public void aucICD10_Populating(object sender, PopulatingEventArgs e)
        {
            ICD10Code = e.Parameter.Trim();
            ObjICD_AutoComplete.PageIndex = 0;
            ICDItems_SearchAutoComplete();
        }
        public void ICDItems_SearchAutoComplete()
        {
            if (!string.IsNullOrEmpty(ICD10Code))
            {
                var resComBo = from c in dicICD
                               where (VNConvertString.ConvertString(c.Value.ICD10Code).ToLower().Contains(VNConvertString.ConvertString(ICD10Code).ToLower())
/*#003: Điều kiện bổ sung ở dòng này*/         || VNConvertString.ConvertString(c.Value.DiseaseNameVN).ToLower().Contains(VNConvertString.ConvertString(ICD10Code).ToLower()))
                               // && (!c.Value.IsRegimenChecking || !IsRegimenChecked || CS_DS == null || CS_DS.TreatmentRegimenCollection == null
                               //     || CS_DS.TreatmentRegimenCollection.Count == 0
                               //     || !CS_DS.TreatmentRegimenCollection.Any(x => x.RefTreatmentRegimenPCLDetails != null && x.RefTreatmentRegimenPCLDetails.Count > 0)
                               //     || !CS_DS.TreatmentRegimenCollection.Where(x => x.RefTreatmentRegimenPCLDetails != null && x.RefTreatmentRegimenPCLDetails.Count > 0).Any(x => x.RefTreatmentRegimenPCLDetails.Count > 0)
                               //     || CS_DS.TreatmentRegimenCollection.Where(x => x.RefTreatmentRegimenPCLDetails != null && x.RefTreatmentRegimenPCLDetails.Count > 0).SelectMany(x => x.RefTreatmentRegimenPCLDetails).Any(x => c.Value == null || x.PCLExamTypeID == c.Value.PCLExamTypeID))
                               select c;

              
                ObjICD_AutoComplete.Clear();
               
                foreach (var item in resComBo)
                {
                    ObjICD_AutoComplete.Add(item.Value);
                }
            }
            else
            {
                ObjICD_AutoComplete.Clear();
            }
            NotifyOfPropertyChange(() => ObjICD_AutoComplete);
            //CtrcboAutoComplete.PopulateComplete();
        }

        public void SetDataForAutoComplete(IList<ICD> ListICD)
        {
            if (ListICD == null)
            {
                ListICD = new List<ICD>();
            }

            dicICD = new Dictionary<long, ICD>();


            foreach (var item in ListICD)
            {
                if (!dicICD.ContainsKey(item.IDCode))
                {
                    dicICD.Add(item.IDCode, item);
                }
            }

        }
        private ICD _selectedICD;
        public ICD SelectedICD
        {
            get { return _selectedICD; }
            set
            {
                if (_selectedICD != value)
                {
                    _selectedICD = value;
                    NotifyOfPropertyChange(() => SelectedICD);
                }
            }
        }
        int i = 0;
        public string KeyAction { get; set; }
        public void AxAutoComplete_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (SelectedICD != null && SelectedICD.IDCode > 0)
            {
                i++;
                if (i == 2)
                {
                    foreach (var item in ObjICD_Display)
                    {
                        if (item.IDCode == SelectedICD.IDCode)
                        {
                            Globals.ShowMessage("ICD đã có", "Thông báo");
                            i = 0;
                            return;
                        }
                    }
                    i = 0;
                    ObjICD_Display.Add(SelectedICD);
                    //ObjICD_ADD.Add(SelectedICD);
                    if (Acb_ICD10_Code != null)
                    {
                        Acb_ICD10_Code.Text = "";
                    }
                    //▲====== #002
                }
            }

        }
        public void btSaveICD()
        {
            if (ObjICD_Display != null && ObjICD_Display.Count > 0)
            {
                ICD_XMLInsert(Disease_ID, ObjICD_Display);
            }
            else
            {
                //Globals.ShowMessage("Chọn Phòng", "Lưu");
                MessageBox.Show("Chọn nhóm ICD", eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
            }
        }
        private void ICD_XMLInsert(Int64 Disease_ID, ObservableCollection<ICD> objCollect)
        {
            bool Result = false;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Lưu..." });
            this.ShowBusyIndicator(eHCMSResources.Z0172_G1_DangLuuDLieu);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginICD_XMLInsert(Disease_ID, objCollect, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                Result = contract.EndICD_XMLInsert(asyncResult);
                                if (Result)
                                {
                                    MessageBox.Show(eHCMSResources.Z0655_G1_DaGhi, eHCMSResources.A0464_G1_Msg_Ghi, MessageBoxButton.OK);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.Z0654_G1_GhiKgThCong, eHCMSResources.A0464_G1_Msg_Ghi, MessageBoxButton.OK);
                                }
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private ObservableCollection<ICD> _ObjICD_GetAll;
        public ObservableCollection<ICD> ObjICD_GetAll
        {
            get
            {
                return _ObjICD_GetAll;
            }
            set
            {
                _ObjICD_GetAll = value;
                NotifyOfPropertyChange(() => ObjICD_GetAll);
            }
        }
        private PagedSortableCollectionView<ICD> _ObjICD_AutoComplete;
        public PagedSortableCollectionView<ICD> ObjICD_AutoComplete
        {
            get
            {
                return _ObjICD_AutoComplete;
            }
            set
            {
                _ObjICD_AutoComplete = value;
                NotifyOfPropertyChange(() => ObjICD_AutoComplete);
            }
        }

        public void ICD_GetAll()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách Loại Phòng..." });
            this.ShowBusyIndicator("Danh sách ICD");
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginICD_GetAll(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var items = contract.EndICD_GetAll(asyncResult);
                                if (items != null)
                                {
                                    ObjICD_GetAll = new ObservableCollection<ICD>(items);
                                    //ItemDefault
                                    //ICD RoomTypeDefault = new ICD();
                                    //RoomTypeDefault.IDCode = -1;
                                    //RoomTypeDefault.DiseaseNameVN = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                    //ObjICD_GetAll.Insert(0, RoomTypeDefault);
                                    //ItemDefault
                                }
                                else
                                {
                                    ObjICD_GetAll = null;
                                }
                                SetDataForAutoComplete(ObjICD_GetAll);
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        private ICDSearchCriteria _SearchCriteria;
        public ICDSearchCriteria SearchCriteria
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

        private string _SearchChapter;
        public string SearchChapter
        {
            get
            {
                return _SearchChapter;
            }
            set
            {
                _SearchChapter = value;
                NotifyOfPropertyChange(() => SearchChapter);
            }
        }
        private string _SearchDiseases;
        public string SearchDiseases
        {
            get
            {
                return _SearchDiseases;
            }
            set
            {
                _SearchDiseases = value;
                NotifyOfPropertyChange(() => SearchDiseases);
            }
        }

        private PagedSortableCollectionView<ICD> _ObjICD_ByIDCode_Paging;
        public PagedSortableCollectionView<ICD> ObjICD_ByIDCode_Paging
        {
            get { return _ObjICD_ByIDCode_Paging; }
            set
            {
                _ObjICD_ByIDCode_Paging = value;
                NotifyOfPropertyChange(() => ObjICD_ByIDCode_Paging);
            }
        }

        private PagedSortableCollectionView<DiseaseChapters> _ObjChapter_Paging;
        public PagedSortableCollectionView<DiseaseChapters> ObjChapter_Paging
        {
            get { return _ObjChapter_Paging; }
            set
            {
                _ObjChapter_Paging = value;
                NotifyOfPropertyChange(() => ObjChapter_Paging);
            }
        }

        private PagedSortableCollectionView<Diseases> _ObjDiseases_Paging;
        public PagedSortableCollectionView<Diseases> ObjDiseases_Paging
        {
            get { return _ObjDiseases_Paging; }
            set
            {
                _ObjDiseases_Paging = value;
                NotifyOfPropertyChange(() => ObjDiseases_Paging);
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
        #region Button
        public void btSearchDiseases()
        {
            ObjDiseases_Paging.PageIndex = 0;
            Diseases_Paging(0, ObjDiseases_Paging.PageSize, true);
        }

        public void btSearchICD()
        {
            ObjICD_ByIDCode_Paging.PageIndex = 0;
            ICD_ByIDCode_Paging(0, ObjICD_ByIDCode_Paging.PageSize, true);
        }
        public void btSearchChapter()
        {
            ObjChapter_Paging.PageIndex = 0;
            Chapter_Paging(0, ObjChapter_Paging.PageSize, true);
        }
        public void hplAddNewChapter_Click()
        {
            Action<IICD_AddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = "Thêm Mới Chương";
                typeInfo.TypeForm = 1;
                typeInfo.InitializeNewItem();
            };
            GlobalsNAV.ShowDialog(onInitDlg);
        }
        public void hplAddNewDiseases_Click()
        {
            Action<IICD_AddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = "Thêm Mới Nhóm";
                typeInfo.TypeForm = 2;
                typeInfo.InitializeNewItem();
            };
            GlobalsNAV.ShowDialog(onInitDlg);
        }
        public void hplAddNewICD_Click()
        {
            Action<IICD_AddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = "Thêm Mới ICD";
                typeInfo.TypeForm = 3;
                typeInfo.InitializeNewItem();
            };
            GlobalsNAV.ShowDialog(onInitDlg);
        }
        public void hplEditChapter_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                Action<IICD_AddEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.ObjDiseaseChapters_Current = ObjectCopier.DeepCopy((selectedItem as DiseaseChapters));
                    typeInfo.TypeForm = 1;
                    typeInfo.TitleForm = "Hiệu Chỉnh (" + (selectedItem as DiseaseChapters).DiseaseChapterNameVN.Trim() + ")";
                };
                GlobalsNAV.ShowDialog<IICD_AddEdit>(onInitDlg);
            }
        }
        public void hplEditDiseases_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                Action<IICD_AddEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.ObjDiseases_Current = ObjectCopier.DeepCopy((selectedItem as Diseases));
                    typeInfo.TypeForm = 2;
                    typeInfo.TitleForm = "Hiệu Chỉnh (" + (selectedItem as Diseases).DiseaseNameVN.Trim() + ")";
                };
                GlobalsNAV.ShowDialog<IICD_AddEdit>(onInitDlg);
            }
        }
        public void hplEditICD_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                Action<IICD_AddEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.ObjICD_Current = ObjectCopier.DeepCopy((selectedItem as ICD));
                    typeInfo.TypeForm = 3;
                    typeInfo.TitleForm = "Hiệu Chỉnh (" + (selectedItem as ICD).DiseaseNameVN.Trim() + ")";
                };
                GlobalsNAV.ShowDialog<IICD_AddEdit>(onInitDlg);
            }
        }
        #endregion
        public void cboDiseaseChaptersSelectedItemChanged(object selectedItem)
        {
            DiseaseChapters_ID = (selectedItem as DiseaseChapters).DiseaseChapterID;
            ObjDiseases_Paging.PageIndex = 0;
            Diseases_Paging(0, ObjDiseases_Paging.PageSize, true);
        }
        public void cboDiseaseChaptersSelectedItemChanged2(object selectedItem)
        {
            DiseaseChapters_ID = (selectedItem as DiseaseChapters).DiseaseChapterID;
            Diseases_ByChapterID();
        }
        public void cboDiseasesSelectedItemChanged(object selectedItem)
        {
            if(selectedItem!= null)
            {
                Disease_ID = (selectedItem as Diseases).DiseaseID;
                ICD_ByDiseaseID_Paging(0, ObjICD_Display.PageSize, true);
            }
        }
        public void Diseases_ByChapterID()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách Loại Phòng..." });
            this.ShowBusyIndicator("Danh sách ICD");
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDiseases_ByChapterID(DiseaseChapters_ID, Globals.DispatchCallback((asyncResult) =>
                         {
                             try
                             {
                                 var items = contract.EndDiseases_ByChapterID(asyncResult);
                                 if (items != null)
                                 {
                                     Diseases = new ObservableCollection<Diseases>(items);
                                 }
                                 else
                                 {
                                     DiseaseChapters = null;
                                 }
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public void DiseaseChapters_GetAll()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách Loại Phòng..." });
            this.ShowBusyIndicator("Danh sách chương");
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDiseaseChapters_GetAll(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var items = contract.EndDiseaseChapters_GetAll(asyncResult);
                                if (items != null)
                                {
                                    DiseaseChapters = new ObservableCollection<DiseaseChapters>(items);
                                    //ItemDefault
                                    DiseaseChapters Default = new DiseaseChapters();
                                    Default.DiseaseChapterID = -1;
                                    Default.DiseaseChapterNameVN = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                    DiseaseChapters.Insert(0, Default);
                                    //ItemDefault
                                }
                                else
                                {
                                    DiseaseChapters = null;
                                }
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public void ICD_MarkDeleted(Int64 IDCode)
        {
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
                        contract.BeginICD_MarkDelete(IDCode, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndICD_MarkDelete(out Result, asyncResult);
                                //if (Result == "InUse")
                                //{
                                //    Globals.ShowMessage(eHCMSResources.Z1318_G1_PhgDangSDungKgTheXoa, eHCMSResources.G2617_G1_Xoa);
                                //}
                                if (Result == "0")
                                {
                                    Globals.ShowMessage("Thất bại", "Thông báo");
                                }
                                if (Result == "1")
                                {
                                    ICD_ByIDCode_Paging(ObjICD_ByIDCode_Paging.PageIndex, ObjICD_ByIDCode_Paging.PageSize, true);
                                    Globals.ShowMessage("Đã xóa ICD", "Thông báo");
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
            ICD p = (selectedItem as ICD);
            if (p != null && p.IDCode > 0)
            {
                if (MessageBox.Show(string.Format("Bạn có muốn xóa ICD này", p.ICD10Code), "Xóa ICD", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    ICD_MarkDeleted(p.IDCode);
                }
            }
        }
        public void hplDelete_ICD_Click(object selectedItem)
        {

            ICD p = (selectedItem as ICD);
            if (p != null && p.IDCode > 0)
            {
                //if (MessageBox.Show(string.Format("Bạn có muốn dừng ICD này", p.ICD10Code), "Tạm dừng ICD", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                //{
                //    ICD_MarkDeleted(p.IDCode);
                //}
                ObjICD_Display.Remove(p);
            }
        }
        private object _ICD_Current;
        public object ICD_Current
        {
            get { return _ICD_Current; }
            set
            {
                _ICD_Current = value;
                NotifyOfPropertyChange(() => ICD_Current);
            }
        }

        private void ICD_ByIDCode_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách Phòng..." });
            this.DlgShowBusyIndicator("Danh sách ICD");
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginICD_ByIDCode_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DataEntities.ICD> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndICD_ByIDCode_Paging(out Total, asyncResult);
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

                            ObjICD_ByIDCode_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjICD_ByIDCode_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjICD_ByIDCode_Paging.Add(item);
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
        private void ICD_ByDiseaseID_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách Phòng..." });
            this.DlgShowBusyIndicator("Danh sách ICD");
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginICD_ByDiseaseID_Paging(Disease_ID, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<ICD> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndICD_ByDiseaseID_Paging(out Total, asyncResult);
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

                            ObjICD_Display.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjICD_Display.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjICD_Display.Add(item);
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
        private void Chapter_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách Phòng..." });
            this.DlgShowBusyIndicator("Danh sách chương");
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginChapter_Paging(SearchChapter, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DiseaseChapters> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndChapter_Paging(out Total, asyncResult);
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

                            ObjChapter_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjChapter_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjChapter_Paging.Add(item);
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

        private void Diseases_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách Phòng..." });
            this.DlgShowBusyIndicator("Danh sách nhóm");
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginDiseases_Paging(DiseaseChapters_ID, SearchDiseases, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<Diseases> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndDiseases_Paging(out Total, asyncResult);
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

                            ObjDiseases_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjDiseases_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjDiseases_Paging.Add(item);
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
        #region Handle
        public void Handle(ICD_Event_Save message)
        {
            ObjICD_ByIDCode_Paging.PageIndex = 0;
            ICD_ByIDCode_Paging(0, ObjICD_ByIDCode_Paging.PageSize, true);
        }
        public void Handle(Chapter_Event_Save message)
        {
            ObjChapter_Paging.PageIndex = 0;
            Chapter_Paging(0, ObjChapter_Paging.PageSize, true);
        }
        public void Handle(Diseases_Event_Save message)
        {
            ObjDiseases_Paging.PageIndex = 0;
            Diseases_Paging(0, ObjDiseases_Paging.PageSize, true);
        }
        #endregion
        
        //▼==== #003
        public void BtnExportExcel()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0669_G1_DangLayDLieu);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        ConfigurationReportParams Params = new ConfigurationReportParams()
                        {
                            ConfigurationName = ConfigurationName.DiseaseChapters,
                            DiseaseChapterID = DiseaseChapters_ID,
                            DiseaseID = Disease_ID
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
