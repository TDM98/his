using eHCMSLanguage;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using aEMR.DataContracts;
using aEMR.ServiceClient;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using System.Threading;
using DataEntities;
using System;
using aEMR.Common;
using aEMR.Common.Collections;
using aEMR.ViewContracts;
using Castle.Windsor;

namespace aEMR.Configuration.PCLGroups.ViewModels
{
    [Export(typeof(IPCLGroups)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLGroupsViewModel : Conductor<object>, IPCLGroups
        , IHandle<SaveEvent<bool>>
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
        [ImportingConstructor]
        public PCLGroupsViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            authorization();
            Globals.EventAggregator.Subscribe(this);

            SearchCriteria = new PCLGroupsSearchCriteria();
            SearchCriteria.V_PCLCategory = -1;
            SearchCriteria.OrderBy = "";

            ObjPCLGroups_GetList_Paging = new PagedSortableCollectionView<DataEntities.PCLGroup>();
            ObjPCLGroups_GetList_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjPCLGroups_GetList_Paging_OnRefresh);

            Load_V_PCLCategory();

        }


        private ObservableCollection<Lookup> _ObjV_PCLCategory;
        public ObservableCollection<Lookup> ObjV_PCLCategory
        {
            get { return _ObjV_PCLCategory; }
            set
            {
                _ObjV_PCLCategory = value;
                NotifyOfPropertyChange(() => ObjV_PCLCategory);
            }
        }

        public void Load_V_PCLCategory()
        {
            var t = new Thread(() =>
            {
                //Globals.EventAggregator.Publish(new BusyEvent{IsBusy = true,Message ="Danh Sách Loại..."});

                IsLoading = true;

                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllLookupValuesByType(LookupValues.V_PCLCategory,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);

                                    ObjV_PCLCategory = new ObservableCollection<Lookup>(allItems);
                                    Lookup firstItem = new Lookup();
                                    firstItem.LookupID = -1;
                                    firstItem.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                    ObjV_PCLCategory.Insert(0, firstItem);
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

        void ObjPCLGroups_GetList_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            PCLGroups_GetList_Paging(ObjPCLGroups_GetList_Paging.PageIndex, ObjPCLGroups_GetList_Paging.PageSize, false);
        }

        private PCLGroupsSearchCriteria _SearchCriteria;
        public PCLGroupsSearchCriteria SearchCriteria
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

        private PagedSortableCollectionView<DataEntities.PCLGroup> _ObjPCLGroups_GetList_Paging;
        public PagedSortableCollectionView<DataEntities.PCLGroup> ObjPCLGroups_GetList_Paging
        {
            get { return _ObjPCLGroups_GetList_Paging; }
            set
            {
                _ObjPCLGroups_GetList_Paging = value;
                NotifyOfPropertyChange(() => ObjPCLGroups_GetList_Paging);
            }
        }

        public void cboV_PCLCategory_SelectionChanged(object selectedItem)
        {
            if (selectedItem != null)
            {
                SearchCriteria.V_PCLCategory = (selectedItem as Lookup).LookupID;
                ObjPCLGroups_GetList_Paging.PageIndex = 0;
                PCLGroups_GetList_Paging(0, ObjPCLGroups_GetList_Paging.PageSize, true);
            }
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bhplEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mDanhMucPCLGroup,
                                               (int)oConfigurationEx.mQuanLyDMPCLGroup, (int)ePermission.mEdit);

            bhplDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mDanhMucPCLGroup,
                                               (int)oConfigurationEx.mQuanLyDMPCLGroup, (int)ePermission.mDelete);
            bhplAddNew= Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                            , (int)eConfiguration_Management.mDanhMucPCLGroup,
                                            (int)oConfigurationEx.mQuanLyDMPCLGroup, (int)ePermission.mAdd);
            bBtnSearch = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                            , (int)eConfiguration_Management.mDanhMucPCLGroup,
                                            (int)oConfigurationEx.mQuanLyDMPCLGroup, (int)ePermission.mView);
        }
        #region checking account

        private bool _bhplEdit = true;
        private bool _bhplDelete = true;
        private bool _bhplAddNew = true;
        private bool _bBtnSearch = true;
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
        public bool bBtnSearch
        {
            get
            {
                return _bBtnSearch;
            }
            set
            {
                if (_bBtnSearch == value)
                    return;
                _bBtnSearch = value;
            }
        }
        #endregion
        #region binding visibilty

        public Button hplEdit{ get; set; }
        public Button hplDelete{ get; set; }

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
            Action<IPCLGroups_AddEdit> onInitDlg = delegate (IPCLGroups_AddEdit typeInfo)
            {
                typeInfo.TitleForm = "Thêm Mới Loại PCLGroup";

                typeInfo.ObjV_PCLCategory = ObjV_PCLCategory;

                typeInfo.InitializeNewItem(SearchCriteria.V_PCLCategory);
            };
            GlobalsNAV.ShowDialog<IPCLGroups_AddEdit>(onInitDlg);
        }


        public void hplEdit_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                Action<IPCLGroups_AddEdit> onInitDlg = delegate (IPCLGroups_AddEdit typeInfo)
                {
                    typeInfo.ObjV_PCLCategory = ObjV_PCLCategory;

                    typeInfo.TitleForm = "Hiệu Chỉnh (" + (selectedItem as DataEntities.PCLGroup).PCLGroupName.Trim() + ")";

                    typeInfo.ObjPCLGroups_Current = ObjectCopier.DeepCopy((selectedItem as DataEntities.PCLGroup));
                };
                GlobalsNAV.ShowDialog<IPCLGroups_AddEdit>(onInitDlg);
            }
        }

        public void btFind()
        {
            ObjPCLGroups_GetList_Paging.PageIndex = 0;
            PCLGroups_GetList_Paging(0, ObjPCLGroups_GetList_Paging.PageSize, true);
        }

        private void PCLGroups_GetList_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách PCLGroups..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLGroups_GetList_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DataEntities.PCLGroup> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndPCLGroups_GetList_Paging(out Total, asyncResult);
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

                            ObjPCLGroups_GetList_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjPCLGroups_GetList_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjPCLGroups_GetList_Paging.Add(item);
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
            DataEntities.PCLGroup p = (selectedItem as DataEntities.PCLGroup);

            if (p != null && p.PCLGroupID > 0)
            {
                if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, p.PCLGroupName), eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    PCLGroups_MarkDelete(p.PCLGroupID);
                }
            }

        }
        private void PCLGroups_MarkDelete(Int64 RmTypeID)
        {
            string Result = "";

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Xóa..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLGroups_MarkDelete(RmTypeID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            contract.EndPCLGroups_MarkDelete(out Result, asyncResult);
                            switch (Result)
                            {
                                case "Delete-0":
                                    {
                                        MessageBox.Show(eHCMSResources.K0484_G1_XoaFail, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Delete-1":
                                    {
                                        ObjPCLGroups_GetList_Paging.PageIndex = 0;
                                        PCLGroups_GetList_Paging(0, ObjPCLGroups_GetList_Paging.PageSize, true);
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
                    ObjPCLGroups_GetList_Paging.PageIndex = 0;
                    PCLGroups_GetList_Paging(0, ObjPCLGroups_GetList_Paging.PageSize, true);
                }
            }
        }
    }
}
