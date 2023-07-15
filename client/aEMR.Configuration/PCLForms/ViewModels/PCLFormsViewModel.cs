using eHCMSLanguage;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Windows;
using aEMR.DataContracts;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using System.Threading;
using DataEntities;
using System;
using aEMR.Common;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Windows.Controls;

namespace aEMR.Configuration.PCLForms.ViewModels
{
    [Export(typeof(IPCLForms)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLFormsViewModel : Conductor<object>, IPCLForms
        ,IHandle<SaveEvent<bool>>
    {

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
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PCLFormsViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            //Globals.EventAggregator.Subscribe(this);
            authorization();
            

            ObjV_PCLMainCategory = new ObservableCollection<Lookup>();
            ObjV_PCLMainCategory_Selected = new Lookup();
            ObjV_PCLMainCategory_Selected.LookupID = -1;


            SearchCriteria = new PCLFormsSearchCriteria();
            SearchCriteria.V_PCLMainCategory = -1;
            SearchCriteria.OrderBy = "";
            
            ObjPCLForms_GetList_Paging=new PagedSortableCollectionView<DataEntities.PCLForm>();
            ObjPCLForms_GetList_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjPCLForms_GetList_Paging_OnRefresh);

            LoadV_PCLMainCategory();

            //ObjPCLForms_GetList_Paging.PageIndex = 0;
            //PCLForms_GetList_Paging(0, ObjPCLForms_GetList_Paging.PageSize, true);
           
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
                //Globals.EventAggregator.Publish(new BusyEvent{IsBusy = true,Message = "Danh Sách Loại..."});

                IsLoading = true;

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
                                    Lookup firstItem = new Lookup();
                                    firstItem.LookupID = -1;
                                    firstItem.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2034_G1_ChonLoai2);
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

                            }), null);
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
            });
            t.Start();
        }
        //Main

        void ObjPCLForms_GetList_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            PCLForms_GetList_Paging(ObjPCLForms_GetList_Paging.PageIndex,ObjPCLForms_GetList_Paging.PageSize, false);
        }

        private PCLFormsSearchCriteria _SearchCriteria;
        public PCLFormsSearchCriteria SearchCriteria
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

                    ObjPCLForms_GetList_Paging.PageIndex = 0;
                    PCLForms_GetList_Paging(0, ObjPCLForms_GetList_Paging.PageSize, true);
                }
            }
        }


        private PagedSortableCollectionView<DataEntities.PCLForm> _ObjPCLForms_GetList_Paging;
        public PagedSortableCollectionView<DataEntities.PCLForm> ObjPCLForms_GetList_Paging
        {
            get { return _ObjPCLForms_GetList_Paging; }
            set
            {
                _ObjPCLForms_GetList_Paging = value;
                NotifyOfPropertyChange(() => ObjPCLForms_GetList_Paging);
            }
        }

        public void cboV_PCLCategory_SelectionChanged(object selectedItem)
        {
            if (selectedItem != null)
            {
                SearchCriteria.V_PCLMainCategory = (selectedItem as Lookup).LookupID;
                ObjPCLForms_GetList_Paging.PageIndex = 0;
                PCLForms_GetList_Paging(0, ObjPCLForms_GetList_Paging.PageSize, true);
            }
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bhplEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mDanhMucPCLForm,
                                               (int)oConfigurationEx.mQuanLyDanhSachPCLForm, (int)ePermission.mEdit);
            bhplDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mDanhMucPCLForm,
                                               (int)oConfigurationEx.mQuanLyDanhSachPCLForm, (int)ePermission.mDelete);
            bBtSearch = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mDanhMucPCLForm,
                                               (int)oConfigurationEx.mQuanLyDanhSachPCLForm, (int)ePermission.mView);
            bhplAddNew = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mDanhMucPCLForm,
                                               (int)oConfigurationEx.mQuanLyDanhSachPCLForm, (int)ePermission.mAdd);

        }
        #region checking account

        private bool _bhplEdit = true;
        private bool _bhplDelete = true;
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

        public void hplAddNew_Click()
        {
            //var typeInfo = Globals.GetViewModel<IPCLForms_AddEdit>();
            //typeInfo.TitleForm = "Thêm Mới PCLForm";

            //typeInfo.InitializeNewItem();
            //typeInfo.FormLoad();

            //var instance = typeInfo as Conductor<object>;

            //Globals.ShowDialog(instance, (o) =>
            //                                 {
            //                                     //làm gì đó
            //                                 });

            Action<IPCLForms_AddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = "Thêm Mới PCLForm";
                typeInfo.InitializeNewItem();
                typeInfo.FormLoad();
            };
            GlobalsNAV.ShowDialog<IPCLForms_AddEdit>(onInitDlg);
        }

       
        public void hplEdit_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                //var typeInfo = Globals.GetViewModel<IPCLForms_AddEdit>();

                //typeInfo.TitleForm = "Hiệu Chỉnh (" + (selectedItem as DataEntities.PCLForm).PCLFormName.Trim() + ")";

                //typeInfo.ObjPCLForms_Current = ObjectCopier.DeepCopy((selectedItem as DataEntities.PCLForm));
                //typeInfo.FormLoad();

                //var instance = typeInfo as Conductor<object>;

                //Globals.ShowDialog(instance, (o) =>
                //                                 {
                //                                     //làm gì đó
                //                                 });

                Action<IPCLForms_AddEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.TitleForm = "Hiệu Chỉnh (" + (selectedItem as DataEntities.PCLForm).PCLFormName.Trim() + ")";
                    typeInfo.ObjPCLForms_Current = ObjectCopier.DeepCopy((selectedItem as DataEntities.PCLForm));
                    typeInfo.FormLoad();
                };
                GlobalsNAV.ShowDialog<IPCLForms_AddEdit>(onInitDlg);
            }
        }

        public void btFind()
        {
            ObjPCLForms_GetList_Paging.PageIndex = 0;
            PCLForms_GetList_Paging(0,ObjPCLForms_GetList_Paging.PageSize, true);
        }

        private void PCLForms_GetList_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách PCLForms..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLForms_GetList_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DataEntities.PCLForm> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndPCLForms_GetList_Paging(out Total, asyncResult);
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

                            ObjPCLForms_GetList_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjPCLForms_GetList_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjPCLForms_GetList_Paging.Add(item);
                                    }

                                }
                            }
                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
                finally
                {
                    //Globals.IsBusy = false;
                    IsLoading = false;
                }
            });
            t.Start();
        }
    

        public void hplDelete_Click(object selectedItem)
        {
            DataEntities.PCLForm p = (selectedItem as DataEntities.PCLForm);

            if (p!=null && p.PCLFormID>0)
            {
                if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, p.PCLFormName), eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    PCLForms_MarkDelete(p.PCLFormID);
                }
            }
        }

        private void PCLForms_MarkDelete(Int64 RmTypeID)
        {
            string Result = "";

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Xóa..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLForms_MarkDelete(RmTypeID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            contract.EndPCLForms_MarkDelete(out Result, asyncResult);
                            switch (Result)
                            {
                                case "Delete-0":
                                    {
                                        MessageBox.Show(eHCMSResources.K0484_G1_XoaFail, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);
                                        break; 
                                    }
                                case "Delete-1":
                                    {
                                        ObjPCLForms_GetList_Paging.PageIndex = 0;
                                        PCLForms_GetList_Paging(0, ObjPCLForms_GetList_Paging.PageSize, true);
                                        MessageBox.Show(eHCMSResources.A0478_G1_Msg_InfoXoaOK, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);
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
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }


        public void Handle(SaveEvent<bool> message)
        {
            if (message != null)
            {
                if (message.Result)
                {
                    ObjPCLForms_GetList_Paging.PageIndex = 0;
                    PCLForms_GetList_Paging(0, ObjPCLForms_GetList_Paging.PageSize, true);
                }
            }
        }



        public void hplPCLItems_Click(object datacontext)
        {
            PCLForm p = datacontext as PCLForm;
            IPCLItems DialogView = Globals.GetViewModel<IPCLItems>();
            DialogView.ObjPCLForm = p;
            DialogView.SearchCriteria = new PCLExamTypeSearchCriteria();
            DialogView.SearchCriteria.V_PCLMainCategory = p.V_PCLMainCategory;
            DialogView.FormLoad();
            GlobalsNAV.ShowDialog_V3(DialogView, null, null, false, true, Globals.GetDefaultDialogViewSize());
        }

    }
}

