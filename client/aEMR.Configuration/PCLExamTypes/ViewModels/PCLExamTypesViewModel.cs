using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using aEMR.Controls;
using eHCMSLanguage;
using aEMR.ViewContracts;
using Castle.Windsor;
using aEMR.Common.BaseModel;
using System.Windows.Input;
using aEMR.Common.ExportExcel;
/*
* 01062018 #001 TTM:
* 01062018 #002 TTM: 
* 01062018 #003 TTM: DeepCopy dữ liệu trong biến tmphiRepEquip (đã được load dữ liệu từ hàm GetAllResources) vào biến tmphiRepEquip của PCLExamTypesViewModel để sử dụng bên viewmodel này
* 20230509 #004 DatTB: IssueID: 3254 | Thêm nút xuất excel cho các danh mục/cấu hình
* 20230601 #005 DatTB: IssueID: 3254 | Chỉnh sửa/Gộp các function xuất excel danh mục/cấu hình (Bỏ Func cũ)
*/
namespace aEMR.Configuration.PCLExamTypes.ViewModels
{
    [Export(typeof(IPCLExamTypes)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLExamTypesViewModel : ViewModelBase, IPCLExamTypes
        , IHandle<PCLExamTypesEvent_AddEditSave>
        , IHandle<PCLExamTypePricesEvent>
    {
        protected override void OnActivate()
        {
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
        }

        [ImportingConstructor]
        public PCLExamTypesViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Globals.EventAggregator.Subscribe(this);
            authorization();

            ObjV_PCLMainCategory = new ObservableCollection<Lookup>();
            ObjV_PCLMainCategory_Selected = new Lookup
            {
                LookupID = -1
            };

            ObjPCLExamTypeSubCategory_ByV_PCLMainCategory = new ObservableCollection<PCLExamTypeSubCategory>();
            ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected = new PCLExamTypeSubCategory
            {
                PCLExamTypeSubCategoryID = -1
            };

            LoadV_PCLMainCategory();

            SearchCriteria = new PCLExamTypeSearchCriteria
            {
                IsNotInPCLItems = false,
                PCLExamTypeName = ""
            };

            ObjPCLExamTypesAndPriceIsActive_Paging = new PagedSortableCollectionView<PCLExamType>();
            ObjPCLExamTypesAndPriceIsActive_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjPCLExamTypesAndPriceIsActive_Paging_OnRefresh);

            //▼=====#002
            //Để khi bắt đầu vào màn hình quản lý danh mục CLS thì dữ liệu về trang thiết bị sẽ được load lên sẵn để khi người dùng vào PCLExamtypes_AddEditViewModel 
            //sẽ có dữ liệu trang thiết bị để load vào combobox trên trang thiết bị cần mapping.
            GetAllResources();
            //▲=====#002
        }

        //Main
        private Lookup _ObjV_PCLMainCategory_Selected;
        public Lookup ObjV_PCLMainCategory_Selected
        {
            get { return _ObjV_PCLMainCategory_Selected; }
            set
            {
                _ObjV_PCLMainCategory_Selected = value;
                NotifyOfPropertyChange(() => ObjV_PCLMainCategory_Selected);
            }
        }

        private ObservableCollection<Lookup> _ObjV_PCLMainCategory;
        public ObservableCollection<Lookup> ObjV_PCLMainCategory
        {
            get { return _ObjV_PCLMainCategory; }
            set
            {
                _ObjV_PCLMainCategory = value;
                NotifyOfPropertyChange(() => ObjV_PCLMainCategory);
            }
        }

        public void LoadV_PCLMainCategory()
        {
            var t = new Thread(() =>
            {
                this.ShowBusyIndicator(eHCMSResources.Z0340_G1_LoadDSLoaiPCL);
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllLookupValuesByType(LookupValues.V_PCLMainCategory,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);

                                    ObjV_PCLMainCategory = new ObservableCollection<Lookup>(allItems);
                                    Lookup firstItem = new Lookup
                                    {
                                        LookupID = -1,
                                        ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2034_G1_ChonLoai2)
                                    };
                                    ObjV_PCLMainCategory.Insert(0, firstItem);

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
        //Main

        //Sub
        private PCLExamTypeSubCategory _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected;
        public PCLExamTypeSubCategory ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected
        {
            get { return _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected; }
            set
            {
                _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected);
            }
        }

        private ObservableCollection<PCLExamTypeSubCategory> _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory;
        public ObservableCollection<PCLExamTypeSubCategory> ObjPCLExamTypeSubCategory_ByV_PCLMainCategory
        {
            get { return _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory; }
            set
            {
                _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypeSubCategory_ByV_PCLMainCategory);
            }
        }

        public void PCLExamTypeSubCategory_ByV_PCLMainCategory()
        {
            ObjPCLExamTypeSubCategory_ByV_PCLMainCategory.Clear();
            this.ShowBusyIndicator(eHCMSResources.Z0341_G1_LoadDSPCLForm);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPCLExamTypeSubCategory_ByV_PCLMainCategory(ObjV_PCLMainCategory_Selected.LookupID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var items = contract.EndPCLExamTypeSubCategory_ByV_PCLMainCategory(asyncResult);

                                if (items != null)
                                {
                                    ObjPCLExamTypeSubCategory_ByV_PCLMainCategory = new ObservableCollection<PCLExamTypeSubCategory>(items);
                                    PCLExamTypeSubCategory firstItem = new PCLExamTypeSubCategory
                                    {
                                        PCLExamTypeSubCategoryID = -1,
                                        PCLSubCategoryName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa)
                                    };
                                    ObjPCLExamTypeSubCategory_ByV_PCLMainCategory.Insert(0, firstItem);
                                    ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected = firstItem;
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
        //Sub

        private PCLExamTypeSearchCriteria _SearchCriteria;
        public PCLExamTypeSearchCriteria SearchCriteria
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

        public void cboV_PCLMainCategory_SelectionChanged(object selectItem)
        {
            if (selectItem != null)
            {
                Lookup Objtmp = (selectItem as Lookup);

                if (Objtmp != null)
                {
                    SearchCriteria.V_PCLMainCategory = Objtmp.LookupID;

                    if (Objtmp.LookupID == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
                    {
                        ObjPCLExamTypeSubCategory_ByV_PCLMainCategory.Clear();
                        ObjPCLExamTypesAndPriceIsActive_Paging.PageIndex = 0;
                        PCLExamTypeSubCategory_ByV_PCLMainCategory();
                        //PCLExamTypesAndPriceIsActive_Paging(0, ObjPCLExamTypesAndPriceIsActive_Paging.PageSize, true);
                    }
                    else
                    {
                        ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected.PCLExamTypeSubCategoryID = -1;
                        PCLExamTypeSubCategory_ByV_PCLMainCategory();
                    }
                }
            }
        }

        public void cboPCLExamTypeSubCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AxComboBox Ctr = sender as AxComboBox;
            if (Ctr == null)
                return;

            PCLExamTypeSubCategory Objtmp = Ctr.SelectedItemEx as PCLExamTypeSubCategory;

            if (Objtmp != null)
            {
                SearchCriteria.PCLExamTypeSubCategoryID = Objtmp.PCLExamTypeSubCategoryID;
                if (SearchCriteria.V_PCLMainCategory != (long)AllLookupValues.V_PCLMainCategory.Laboratory)
                {
                    ObjPCLExamTypesAndPriceIsActive_Paging.PageIndex = 0;
                    PCLExamTypesAndPriceIsActive_Paging(0, ObjPCLExamTypesAndPriceIsActive_Paging.PageSize, true);
                }
                else
                {
                    ObjPCLExamTypesAndPriceIsActive_Paging.PageIndex = 0;
                    PCLExamTypesAndPriceIsActive_Paging(0, ObjPCLExamTypesAndPriceIsActive_Paging.PageSize, true);
                }
            }
        }

        void ObjPCLExamTypesAndPriceIsActive_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            PCLExamTypesAndPriceIsActive_Paging(ObjPCLExamTypesAndPriceIsActive_Paging.PageIndex, ObjPCLExamTypesAndPriceIsActive_Paging.PageSize, false);
        }

        private PagedSortableCollectionView<PCLExamType> _ObjPCLExamTypesAndPriceIsActive_Paging;
        public PagedSortableCollectionView<PCLExamType> ObjPCLExamTypesAndPriceIsActive_Paging
        {
            get { return _ObjPCLExamTypesAndPriceIsActive_Paging; }
            set
            {
                _ObjPCLExamTypesAndPriceIsActive_Paging = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypesAndPriceIsActive_Paging);
            }
        }

        private void PCLExamTypesAndPriceIsActive_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            ObjPCLExamTypesAndPriceIsActive_Paging.Clear();

            if (CheckClickHeaderNotValid() == false)
                return;
            this.ShowBusyIndicator();
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách PCLExamType..." });
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLExamTypesAndPriceIsActive_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<PCLExamType> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndPCLExamTypesAndPriceIsActive_Paging(out Total, asyncResult);
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
                                this.HideBusyIndicator();
                            }
                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjPCLExamTypesAndPriceIsActive_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjPCLExamTypesAndPriceIsActive_Paging.Add(item);
                                    }
                                }
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            PCLExamType objRows = e.Row.DataContext as PCLExamType;
            if (objRows != null)
            {
                switch (objRows.ObjPCLExamTypePrice.PriceType)
                {
                    case "PriceCurrent":
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Green);
                            break;
                        }
                    case "PriceFuture-Active-1":
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Gray);
                            break;
                        }
                    default:
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Black);
                            break;
                        }
                }
            }
        }

        private bool CheckClickHeaderNotValid()
        {
            if (SearchCriteria.V_PCLMainCategory > 0)
                return true;
            return false;
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bhplEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mDanhMucPCLExamTypes,
                                               (int)oConfigurationEx.mQuanLyDMPCLExamType, (int)ePermission.mEdit);

            bhplDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mDanhMucPCLExamTypes,
                                               (int)oConfigurationEx.mQuanLyDMPCLExamType, (int)ePermission.mDelete);
            bhplListPrice = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mDanhMucPCLExamTypes,
                                               (int)oConfigurationEx.mQuanLyDMPCLExamType, (int)ePermission.mView);
            bBtSearch = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mDanhMucPCLExamTypes,
                                               (int)oConfigurationEx.mQuanLyDMPCLExamType, (int)ePermission.mView);
            bhplAddNew = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                           , (int)eConfiguration_Management.mDanhMucPCLExamTypes,
                                           (int)oConfigurationEx.mQuanLyDMPCLExamType, (int)ePermission.mAdd);
        }

        #region checking account

        private bool _bhplEdit = true;
        private bool _bhplDelete = true;
        private bool _bhplListPrice = true;
        private bool _bBtSearch = true;
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

        public bool bhplListPrice
        {
            get
            {
                return _bhplListPrice;
            }
            set
            {
                if (_bhplListPrice == value)
                    return;
                _bhplListPrice = value;
            }
        }

        public bool bBtSearch
        {
            get
            {
                return _bBtSearch;
            }
            set
            {
                if (_bBtSearch == value)
                    return;
                _bBtSearch = value;
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

        public Button hplListPrice { get; set; }

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

        public void hplListPrice_Loaded(object sender)
        {
            hplListPrice = sender as Button;
            hplListPrice.Visibility = Globals.convertVisibility(bhplListPrice);
        }
        #endregion

        public void btFind()
        {
            if (SearchCriteria.V_PCLMainCategory > 0)
            {
                ObjPCLExamTypesAndPriceIsActive_Paging.PageIndex = 0;
                PCLExamTypesAndPriceIsActive_Paging(0, ObjPCLExamTypesAndPriceIsActive_Paging.PageSize, true);
            }
            else//-1 Text yêu cầu chọn
            {
                MessageBox.Show(eHCMSResources.A0335_G1_Msg_InfoChonLoai, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
        }

        private void PCLExamTypes_MarkDelete(long PCLExamTypeID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0492_G1_DangXoa);
            string Result = "";
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPCLExamTypes_MarkDelete(PCLExamTypeID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndPCLExamTypes_MarkDelete(out Result, asyncResult);
                                if (Result == "Delete-1")
                                {
                                    ObjPCLExamTypesAndPriceIsActive_Paging.PageIndex = 0;
                                    PCLExamTypesAndPriceIsActive_Paging(0, ObjPCLExamTypesAndPriceIsActive_Paging.PageSize, true);
                                    MessageBox.Show(eHCMSResources.A0478_G1_Msg_InfoXoaOK, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.K0484_G1_XoaFail, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);
                                }
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

        public void hplDelete_Click(object datacontext)
        {
            PCLExamType p = (datacontext as PCLExamType);
            if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, p.PCLExamTypeName), eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                PCLExamTypes_MarkDelete(p.PCLExamTypeID);
            }
        }

        public void hplAddNew()
        {
            void onInitDlg(IPCLExamTypes_AddEdit typeInfo)
            {
                typeInfo.TitleForm = "Thêm Mới PCLExamType";
                typeInfo.ObjV_PCLMainCategory = ObjV_PCLMainCategory.DeepCopy();
                //▼=====#003
                typeInfo.tmphiRepEquip = ObjectCopier.DeepCopy(tmphiRepEquip);
                //▲=====#003
                typeInfo.InitializeNewItem(ObjV_PCLMainCategory_Selected.DeepCopy(), ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected.DeepCopy());

                typeInfo.FormLoad();
            }
            GlobalsNAV.ShowDialog<IPCLExamTypes_AddEdit>(onInitDlg, null, false, true, Globals.GetHalfHeightAndThreeFourthWidthDefaultDialogViewSize());
        }

        public void hplEdit_Click(object datacontext)
        {
            PCLExamType p = ObjectCopier.DeepCopy(datacontext as PCLExamType);

            void onInitDlg(IPCLExamTypes_AddEdit typeInfo)
            {
                typeInfo.TitleForm = "Hiệu Chỉnh PCLExamType: " + p.PCLExamTypeName.Trim();

                typeInfo.ObjV_PCLMainCategory = ObjV_PCLMainCategory.DeepCopy();

                typeInfo.ObjPCLExamType_Current = ObjectCopier.DeepCopy(p);
                //▼=====#003
                typeInfo.tmphiRepEquip = ObjectCopier.DeepCopy(tmphiRepEquip);
                //▲=====#003
                typeInfo.FormLoad();
            }
            GlobalsNAV.ShowDialog<IPCLExamTypes_AddEdit>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        }

        public void Handle(PCLExamTypesEvent_AddEditSave message)
        {
            if (message != null)
            {
                ObjPCLExamTypesAndPriceIsActive_Paging.PageIndex = 0;
                PCLExamTypesAndPriceIsActive_Paging(0, ObjPCLExamTypesAndPriceIsActive_Paging.PageSize, true);
            }
        }

        public void hplListPrice_Click(object datacontext)
        {
            PCLExamType p = (datacontext as PCLExamType);

            void onInitDlg(IPCLExamTypePrices typeInfo)
            {
                typeInfo.SearchCriteria = new PCLExamTypePriceSearchCriteria
                {
                    PCLExamTypeID = p.PCLExamTypeID,
                    PriceType = 1//Hiện hành
                };
                typeInfo.ObjPCLExamType_Current = p;
            }
            GlobalsNAV.ShowDialog<IPCLExamTypePrices>(onInitDlg);
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

        private Visibility _dgCellTemplate0_Visible = Visibility.Visible;
        public Visibility dgCellTemplate0_Visible
        {
            get { return _dgCellTemplate0_Visible; }
            set
            {
                _dgCellTemplate0_Visible = value;
                NotifyOfPropertyChange(() => dgCellTemplate0_Visible);
            }
        }

        public void dtgListSelectionChanged(object args)
        {
            if (((object[])(((SelectionChangedEventArgs)(args)).AddedItems)).Length > 0)
            {
                if (((object[])(((SelectionChangedEventArgs)(args)).AddedItems))[0] != null)
                {
                    Globals.EventAggregator.Publish(new dgListPCLExamTypeClickSelectionChanged_Event()
                    {
                        PCLExamType_Current = ((object[])(((SelectionChangedEventArgs)(args)).AddedItems))[0]
                    });
                }
            }
        }

        public void Handle(PCLExamTypePricesEvent message)
        {
            ObjPCLExamTypesAndPriceIsActive_Paging.PageIndex = 0;
            PCLExamTypesAndPriceIsActive_Paging(0, ObjPCLExamTypesAndPriceIsActive_Paging.PageSize, true);
        }

        //▼=====#001
        // 11062018 TTM:
        //Hiện tại, theo yêu cầu cần gán máy vào dịch vụ CLS để xác định xem CLS đó được máy thực hiện nhưng vì cần phải load máy để người sử dụng có thể thực hiện việc mapping
        //nên cần tạo một hàm GetAllResources 
        //(Đã có một hàm GetAllResource [Resource không có s] lấy toàn bộ, nhưng nó lại lấy thêm toàn bộ thông tin Supplier nên đã tạo mới một hàm
        //GetAllResources chỉ lấy danh sách Trang thiết bị y tế)
        //Sau đó tạo một biến tạm để lưu trữ thông tin này  
        #region Thông tin trang thiết bị, máy móc 
        private ObservableCollection<Resources> _tmphiRepEquip;
        public ObservableCollection<Resources> tmphiRepEquip
        {
            get { return _tmphiRepEquip; }
            set
            {
                _tmphiRepEquip = value;
                NotifyOfPropertyChange(() => tmphiRepEquip);
            }
        }

        public void GetAllResources()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Dang Load resources" });
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ResourcesManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllResources(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var mResources = contract.EndGetAllResources(asyncResult);
                                if (mResources != null)
                                {
                                    tmphiRepEquip = mResources.ToObservableCollection();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }
        #endregion
        //▲=====#001
        public void txtPCLExamTypeName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btFind();
            }
        }

        //▼==== #004
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
                            ConfigurationName = ConfigurationName.PCLExamTypes,
                            V_PCLMainCategory = ObjV_PCLMainCategory_Selected.LookupID,
                            PCLExamTypeSubCategoryID = ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected.PCLExamTypeSubCategoryID
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
        //▲==== #004
    }
}
