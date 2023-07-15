using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Net;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.Common.PagedCollectionView;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using DataEntities;
using aEMR.Controls;
using System.Windows.Data;
using eHCMSLanguage;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using System.Linq;

namespace aEMR.Configuration.PCLItems.ViewModels
{
    [Export(typeof(IPCLItems)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLItemsViewModel : Conductor<object>, IPCLItems
        , IHandle<DbClickSelectedObjectEvent<PCLExamType>>
        , IHandle<SelectedObjectEvent<PCLExamType>>
    {

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PCLItemsViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }


        private object _leftContent;
        public object leftContent
        {
            get
            {
                return _leftContent;
            }
            set
            {
                if (_leftContent == value)
                    return;
                _leftContent = value;
                NotifyOfPropertyChange(() => leftContent);
            }
        }

        private PCLForm _ObjPCLForm;
        public PCLForm ObjPCLForm
        {
            get { return _ObjPCLForm;}
            set
            {
                if(_ObjPCLForm!=value)
                {
                    _ObjPCLForm = value;
                    NotifyOfPropertyChange(()=>ObjPCLForm);
                }
            }
        }



        ////Main
        //private Lookup _ObjV_PCLMainCategory_Selected;
        //public Lookup ObjV_PCLMainCategory_Selected
        //{
        //    get { return _ObjV_PCLMainCategory_Selected; }
        //    set
        //    {
        //        _ObjV_PCLMainCategory_Selected = value;
        //        NotifyOfPropertyChange(() => ObjV_PCLMainCategory_Selected);
        //    }
        //}

        //private ObservableCollection<Lookup> _ObjV_PCLMainCategory;
        //public ObservableCollection<Lookup> ObjV_PCLMainCategory
        //{
        //    get { return _ObjV_PCLMainCategory; }
        //    set
        //    {
        //        _ObjV_PCLMainCategory = value;
        //        NotifyOfPropertyChange(() => ObjV_PCLMainCategory);
        //    }
        //}

        //public void LoadV_PCLMainCategory()
        //{
        //    var t = new Thread(() =>
        //    {
        //        Globals.EventAggregator.Publish(new BusyEvent
        //        {
        //            IsBusy = true,
        //            Message = "Danh Sách Loại..."
        //        });
        //        try
        //        {
        //            using (var serviceFactory = new CommonServiceClient())
        //            {
        //                var contract = serviceFactory.ServiceInstance;

        //                contract.BeginGetAllLookupValuesByType(LookupValues.V_PCLMainCategory,
        //                    Globals.DispatchCallback((asyncResult) =>
        //                    {
        //                        IList<Lookup> allItems = new ObservableCollection<Lookup>();
        //                        try
        //                        {
        //                            allItems = contract.EndGetAllLookupValuesByType(asyncResult);

        //                            ObjV_PCLMainCategory = new ObservableCollection<Lookup>(allItems);
        //                            Lookup firstItem = new Lookup();
        //                            firstItem.LookupID = -1;
        //                            firstItem.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
        //                            ObjV_PCLMainCategory.Insert(0, firstItem);
        //                        }
        //                        catch (FaultException<AxException> fault)
        //                        {

        //                        }
        //                        catch (Exception ex)
        //                        {
        //                        }

        //                    }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //        }
        //        finally
        //        {
        //            Globals.IsBusy = false;
        //        }
        //    });
        //    t.Start();
        //}
        ////Main


        ////Sub
        //private PCLExamTypeSubCategory _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected;
        //public PCLExamTypeSubCategory ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected
        //{
        //    get { return _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected; }
        //    set
        //    {
        //        _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected = value;
        //        NotifyOfPropertyChange(() => ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected);
        //    }
        //}

        //private ObservableCollection<PCLExamTypeSubCategory> _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory;
        //public ObservableCollection<PCLExamTypeSubCategory> ObjPCLExamTypeSubCategory_ByV_PCLMainCategory
        //{
        //    get { return _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory; }
        //    set
        //    {
        //        _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory = value;
        //        NotifyOfPropertyChange(() => ObjPCLExamTypeSubCategory_ByV_PCLMainCategory);
        //    }
        //}

        //public void PCLExamTypeSubCategory_ByV_PCLMainCategory()
        //{
        //    ObjPCLExamTypeSubCategory_ByV_PCLMainCategory.Clear();

        //    Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách Nhóm..." });

        //    var t = new Thread(() =>
        //    {
        //        using (var serviceFactory = new ConfigurationManagerServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;

        //            contract.BeginPCLExamTypeSubCategory_ByV_PCLMainCategory(ObjV_PCLMainCategory_Selected.LookupID, Globals.DispatchCallback((asyncResult) =>
        //            {
        //                try
        //                {
        //                    var items = contract.EndPCLExamTypeSubCategory_ByV_PCLMainCategory(asyncResult);

        //                    if (items != null)
        //                    {
        //                        ObjPCLExamTypeSubCategory_ByV_PCLMainCategory = new ObservableCollection<PCLExamTypeSubCategory>(items);
        //                        PCLExamTypeSubCategory firstItem = new PCLExamTypeSubCategory();
        //                        firstItem.PCLExamTypeSubCategoryID = -1;
        //                        firstItem.PCLSubCategoryName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
        //                        ObjPCLExamTypeSubCategory_ByV_PCLMainCategory.Insert(0, firstItem);

        //                        ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected = firstItem;
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                }
        //                finally
        //                {
        //                    Globals.IsBusy = false;
        //                }
        //            }), null);
        //        }


        //    });
        //    t.Start();
        //}
        ////Sub

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

        public void FormLoad()
        {
            authorization();

            //Load UC
            var UCPCLExamTypes = Globals.GetViewModel<IPCLExamTypes_List_Paging>();
            leftContent = UCPCLExamTypes;
            (this as Conductor<object>).ActivateItem(leftContent);
            UCPCLExamTypes.IsEnableV_PCLMainCategory = false;
            UCPCLExamTypes.SearchCriteria = new PCLExamTypeSearchCriteria();
            UCPCLExamTypes.SearchCriteria.V_PCLMainCategory = SearchCriteria.V_PCLMainCategory;
            UCPCLExamTypes.SearchCriteria.IsNotInPCLExamTypeLocations = true;
            UCPCLExamTypes.SearchCriteria.IsNotInPCLItems = false;
            UCPCLExamTypes.IsNotInPCLItemsVisibility = Visibility.Visible;
            UCPCLExamTypes.ObjV_PCLMainCategory_Selected=new Lookup();
            UCPCLExamTypes.ObjV_PCLMainCategory_Selected.LookupID = SearchCriteria.V_PCLMainCategory;
            UCPCLExamTypes.LoadForConfigPCLExamTypesIntoPCLForm();
            //Load UC

            //ObjV_PCLMainCategory = new ObservableCollection<Lookup>();
            //LoadV_PCLMainCategory();

            //ObjV_PCLMainCategory_Selected = new Lookup();
            //ObjV_PCLMainCategory_Selected.LookupID = SearchCriteria.V_PCLMainCategory;

            //ObjPCLExamTypeSubCategory_ByV_PCLMainCategory = new ObservableCollection<PCLExamTypeSubCategory>();
            //ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected = new PCLExamTypeSubCategory();
            //ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected.PCLExamTypeSubCategoryID = -1;
            
            HasGroup = true;

            PCLItems_ByPCLFormID_HasGroup(HasGroup);
            

        }
        

        private ObservableCollection<PCLExamType> _ObjPCLItems_ByPCLFormID_TMP;
        public ObservableCollection<PCLExamType> ObjPCLItems_ByPCLFormID_TMP
        {
            get { return _ObjPCLItems_ByPCLFormID_TMP; }
            set
            {
                _ObjPCLItems_ByPCLFormID_TMP = value;
                NotifyOfPropertyChange(() => ObjPCLItems_ByPCLFormID_TMP);
            }
        }

        private PagedCollectionView _ObjPCLItems_ByPCLFormID;
        public PagedCollectionView ObjPCLItems_ByPCLFormID
        {
            get { return _ObjPCLItems_ByPCLFormID; }
            set
            {
                _ObjPCLItems_ByPCLFormID = value;
                NotifyOfPropertyChange(()=>ObjPCLItems_ByPCLFormID);
            }
        }

        private bool _HasGroup;
        public bool HasGroup
        {
            get { return _HasGroup; }
            set
            {
                if(_HasGroup!=value)
                {
                    _HasGroup = value;
                    NotifyOfPropertyChange(()=>HasGroup);
                }
            }
        }

        private void PCLItems_ByPCLFormID_HasGroup(bool isGroup)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách PCLExamType..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLItems_ByPCLFormID(SearchCriteria, ObjPCLForm.PCLFormID, null, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndPCLItems_ByPCLFormID(asyncResult);

                            if (items != null)
                            {
                                //2021-09-06 BLQ: Thêm điều kiện lấy theo PCLFormID vì khi lấy lên từ Cache sẽ lấy hết PclExamType và không kiểm tra điều kiện 
                                //                  PCLFormID nữa nên khi cập nhật sẽ cập nhật hết PCLExamType theo PCLFormID sẽ ra dòng Double của PCLExamType
                                ObjPCLItems_ByPCLFormID_TMP = new ObservableCollection<PCLExamType>(items.Where(x => x.PCLFormID == ObjPCLForm.PCLFormID));
                                
                                ObjPCLItems_ByPCLFormID = new PagedCollectionView(items.Where(x => x.PCLFormID == ObjPCLForm.PCLFormID));
                                    
                                if (b_Adding)
                                {
                                    if (b_btSearch1Click)
                                    {
                                        b_btSearch1Click = false;
                                        AddItem();
                                    }
                                }

                                if(isGroup)
                                {
                                    ObjPCLItems_ByPCLFormID.GroupDescriptions.Add(new aEMR.Common.PagedCollectionView.PropertyGroupDescription("PCLSectionName"));
                                }
                            }
                            else
                            {
                                ObjPCLItems_ByPCLFormID = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }
                    }), null);
                }


            });
            t.Start();

        }


        //public void cboV_PCLMainCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    AxComboBox Ctr = sender as AxComboBox;
        //    if (Ctr == null)
        //        return;

        //    Lookup Objtmp = Ctr.SelectedItemEx as Lookup;

        //    if (Objtmp != null)
        //    {

        //        if (Objtmp.LookupID == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
        //        {
        //            ObjPCLExamTypeSubCategory_ByV_PCLMainCategory.Clear();
        //            PCLItems_ByPCLFormID_HasGroup(HasGroup);
        //        }
        //        else
        //        {
        //            ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected.PCLExamTypeSubCategoryID = -1;
        //            SearchCriteria.V_PCLMainCategory = Objtmp.LookupID;
        //            PCLExamTypeSubCategory_ByV_PCLMainCategory();
        //        }
                
        //    }
        //}


        //public void cboPCLExamTypeSubCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    AxComboBox Ctr = sender as AxComboBox;
        //    if (Ctr == null)
        //        return;

        //    PCLExamTypeSubCategory Objtmp = Ctr.SelectedItemEx as PCLExamTypeSubCategory;

        //    if (Objtmp != null)
        //    {
        //        SearchCriteria.PCLExamTypeSubCategoryID = Objtmp.PCLExamTypeSubCategoryID;
        //        if (SearchCriteria.V_PCLMainCategory != (long)AllLookupValues.V_PCLMainCategory.Laboratory)
        //        {
        //            PCLItems_ByPCLFormID_HasGroup(HasGroup);
        //        }
        //    }
        //}

        
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bhplDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mCauHinhPCLExamTypes_Sessions,
                                               (int)oConfigurationEx.mQuanLySession_PCLExamType, (int)ePermission.mDelete);
            bBtSearch = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mCauHinhPCLExamTypes_Sessions,
                                               (int)oConfigurationEx.mQuanLySession_PCLExamType, (int)ePermission.mView);
            bBtnAdd = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mCauHinhPCLExamTypes_Sessions,
                                               (int)oConfigurationEx.mQuanLySession_PCLExamType, (int)ePermission.mAdd);
        }
      
        #region checking account

        private bool _bhplDelete = true;
        private bool _bBtSearch = true;
        private bool _bBtnAdd = true;
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
        public bool bBtnAdd
        {
            get
            {
                return _bBtnAdd;
            }
            set
            {
                if (_bBtnAdd == value)
                    return;
                _bBtnAdd = value;
            }
        }
        #endregion

        #region binding visibilty

        public Button hplDelete { get; set; }

        public void hplDelete_Loaded(object sender)
        {
            hplDelete = sender as Button;
            hplDelete.Visibility = Globals.convertVisibility(bhplDelete);
        }


        #endregion

        private bool b_btSearch1Click = false;
        public void btFind()
        {
            if(ObjPCLForm!=null)
            {
                b_btSearch1Click = true;
                PCLItems_ByPCLFormID_HasGroup(HasGroup);
            }
        }

        
        public void chkGroupBy_Click(object sender, RoutedEventArgs e)
        {
            CheckBox Ctr = sender as CheckBox;
            if(Ctr==null)
                return;
            PCLItems_ByPCLFormID_HasGroup(HasGroup);
        }


        public void hplDelete_Click(object datacontext)
        {
            ObjPCLItems_ByPCLFormID.Remove(datacontext as PCLExamType);
            ObjPCLItems_ByPCLFormID_TMP.Remove(datacontext as PCLExamType);
        }

        private DataEntities.PCLExamType _ObjPCLExamType_SelectForAdd;
        public DataEntities.PCLExamType ObjPCLExamType_SelectForAdd
        {
            get { return _ObjPCLExamType_SelectForAdd; }
            set
            {
                _ObjPCLExamType_SelectForAdd = value;
                NotifyOfPropertyChange(() => ObjPCLExamType_SelectForAdd);
            }
        }
        

        public void Handle(SelectedObjectEvent<PCLExamType> message)
        {
            if (this.GetView() != null)
            {
                if (message != null)
                {
                    ObjPCLExamType_SelectForAdd = message.Result;
                }
            }
        }

        public void Handle(DbClickSelectedObjectEvent<PCLExamType> message)
        {
            if (this.GetView() != null)
            {
                if (message != null)
                {
                    ObjPCLExamType_SelectForAdd = message.Result;
                    AddItem();
                }
            }
        }

        private bool b_Adding = false;
        public void btAddChoose()
        {
            if (ObjPCLExamType_SelectForAdd != null)
            {
                AddItem();
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0347_G1_Msg_InfoChonPCLExamType, eHCMSResources.G0156_G1_Them, MessageBoxButton.OK);
            }
        }

        private void AddItem()
        {
            b_Adding = true;

            if (this.GetView() != null)
            {
                if (b_btSearch1Click == false)
                {
                    if (ObjPCLForm != null && ObjPCLForm.PCLFormID > 0)
                    {
                        if (!ObjPCLItems_ByPCLFormID.Contains(ObjPCLExamType_SelectForAdd))
                        {
                            ObjPCLItems_ByPCLFormID_TMP.Add(ObjPCLExamType_SelectForAdd);
                            ShowDataAfterCRUD();
                            b_Adding = false;
                        }
                        else
                        {
                            MessageBox.Show(ObjPCLExamType_SelectForAdd.PCLExamTypeName.Trim() + string.Format(" {0}", eHCMSResources.A0071_G1_Msg_InfoItemIsSelected), eHCMSResources.G0156_G1_Them, MessageBoxButton.OK);
                        }
                    }
                    else
                    {
                        MessageBox.Show(eHCMSResources.K2088_G1_ChonPCLForm2, eHCMSResources.G0156_G1_Them, MessageBoxButton.OK);
                    }
                }
                else/*Có bấm nút Search ai biết list hiện tại là gì nên phải đọc lại list rồi add thêm vô*/
                {
                    SearchCriteria.PCLExamTypeName = "";
                    PCLItems_ByPCLFormID_HasGroup(HasGroup);
                }
            }
        }

        private void ShowDataAfterCRUD()
        {
            ObjPCLItems_ByPCLFormID = new PagedCollectionView(ObjPCLItems_ByPCLFormID_TMP);

            if (HasGroup)
            {
                ObjPCLItems_ByPCLFormID.GroupDescriptions.Add(new aEMR.Common.PagedCollectionView.PropertyGroupDescription("PCLSectionName"));
            }
        }
        
        public void btSaveItems()
        {
            if (ObjPCLForm!=null && ObjPCLForm.PCLFormID > 0)
            {
                if (ObjPCLItems_ByPCLFormID != null && ObjPCLItems_ByPCLFormID.Count > 0)
                {
                    PCLItems_XMLInsert();
                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0347_G1_Msg_InfoChonPCLExamType, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                }
            }
            else
            {
                MessageBox.Show("Chọn PCLForm!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
        }

        private void PCLItems_XMLInsert()
        {
            bool Result = false;

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Lưu..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLItems_XMLInsert(ObjPCLForm.PCLFormID,ObjPCLItems_ByPCLFormID_TMP, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            Result = contract.EndPCLItems_XMLInsert(asyncResult);
                            if (Result)
                            {
                                Globals.EventAggregator.Publish(new PCLItemsEvent_Save() { Result = true });

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
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }


        public void btClose()
        {
            TryClose();
        }


    }
}
