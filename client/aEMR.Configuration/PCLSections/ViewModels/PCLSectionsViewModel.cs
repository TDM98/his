using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Net;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts.Configuration;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Common.Collections;

namespace aEMR.Configuration.PCLSections.ViewModels
{
    [Export(typeof(IPCLSections)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLSectionsViewModel : Conductor<object>, IPCLSections
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
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PCLSectionsViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            authorization();

            //SearchCriteria = new PCLFormsSearchCriteria();
            //SearchCriteria.PCLFormName = "";

            //ObjPCLForms_GetList = new ObservableCollection<PCLForm>();
            //PCLForm_GetList(0, 99999, true);//ko phân trang

            SearchCriteriaSection = new PCLSectionsSearchCriteria();
            SearchCriteriaSection.PCLSectionName = "";

            ObjPCLSections_GetList_Paging = new PagedSortableCollectionView<PCLSection>();
            ObjPCLSections_GetList_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjPCLSections_GetList_Paging_OnRefresh);

            ObjPCLSections_GetList_Paging.PageIndex = 0;
            PCLSections_GetList_Paging(0, ObjPCLSections_GetList_Paging.PageSize, true);

        }

        void ObjPCLSections_GetList_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            PCLSections_GetList_Paging(ObjPCLSections_GetList_Paging.PageIndex, ObjPCLSections_GetList_Paging.PageSize, false);
        }

        //private ObservableCollection<Lookup> _ObjV_PCLCategory;
        //public ObservableCollection<Lookup> ObjV_PCLCategory
        //{
        //    get { return _ObjV_PCLCategory; }
        //    set
        //    {
        //        _ObjV_PCLCategory = value;
        //        NotifyOfPropertyChange(() => ObjV_PCLCategory);
        //    }
        //}

        //public void Load_V_PCLCategory()
        //{
        //    var t = new Thread(() =>
        //    {
        //        Globals.EventAggregator.Publish(new BusyEvent
        //        {
        //            IsBusy = true,
        //            Message =
        //                "Danh Sách Loại..."
        //        });
        //        try
        //        {
        //            using (var serviceFactory = new CommonServiceClient())
        //            {
        //                var contract = serviceFactory.ServiceInstance;

        //                contract.BeginGetAllLookupValuesByType(LookupValues.V_PCLCategory,
        //                    Globals.DispatchCallback((asyncResult) =>
        //                    {
        //                        IList<Lookup> allItems = new ObservableCollection<Lookup>();
        //                        try
        //                        {
        //                            allItems = contract.EndGetAllLookupValuesByType(asyncResult);

        //                            ObjV_PCLCategory = new ObservableCollection<Lookup>(allItems);
        //                            Lookup firstItem = new Lookup();
        //                            firstItem.LookupID = -1;
        //                            firstItem.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
        //                            ObjV_PCLCategory.Insert(0, firstItem);
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


        //private PCLFormsSearchCriteria _SearchCriteria;
        //public PCLFormsSearchCriteria SearchCriteria
        //{
        //    get
        //    {
        //        return _SearchCriteria;
        //    }
        //    set
        //    {
        //        _SearchCriteria = value;
        //        NotifyOfPropertyChange(() => SearchCriteria);

        //    }
        //}

        //private ObservableCollection<DataEntities.PCLForm> _ObjPCLForms_GetList;
        //public ObservableCollection<DataEntities.PCLForm> ObjPCLForms_GetList
        //{
        //    get { return _ObjPCLForms_GetList; }
        //    set
        //    {
        //        _ObjPCLForms_GetList = value;
        //        NotifyOfPropertyChange(() => ObjPCLForms_GetList);
        //    }
        //}

        //private void PCLForm_GetList(int PageIndex, int PageSize, bool CountTotal)
        //{
        //    ObjPCLForms_GetList.Clear();

        //    Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách PCLForms..." });

        //    var t = new Thread(() =>
        //    {
        //        try
        //        {
        //            using (var serviceFactory = new ConfigurationManagerServiceClient())
        //            {
        //                var client = serviceFactory.ServiceInstance;
        //                client.BeginPCLForms_GetList_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
        //                {
        //                    int Total = 0;
        //                    IList<DataEntities.PCLForm> allItems = null;

        //                    try
        //                    {
        //                        allItems = client.EndPCLForms_GetList_Paging(out Total, asyncResult);
        //                        if (allItems != null)
        //                        {
        //                            ObjPCLForms_GetList = new ObservableCollection<DataEntities.PCLForm>(allItems);

        //                            //ItemDefault
        //                            DataEntities.PCLForm ItemDefault = new DataEntities.PCLForm();
        //                            ItemDefault.PCLFormID = -1;
        //                            ItemDefault.PCLFormName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);

        //                            ObjPCLForms_GetList.Insert(0, ItemDefault);
        //                            //ItemDefault
        //                        }
        //                        else
        //                        {
        //                            ObjPCLForms_GetList = null;
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                    }

        //                }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //        }
        //        finally
        //        {
        //            Globals.IsBusy = false;
        //        }
        //    });
        //    t.Start();
        //}


        private PCLSectionsSearchCriteria _SearchCriteriaSection;
        public PCLSectionsSearchCriteria SearchCriteriaSection
        {
            get
            {
                return _SearchCriteriaSection;
            }
            set
            {
                _SearchCriteriaSection = value;
                NotifyOfPropertyChange(() => SearchCriteriaSection);

            }
        }


        private PagedSortableCollectionView<DataEntities.PCLSection> _ObjPCLSections_GetList_Paging;
        public PagedSortableCollectionView<DataEntities.PCLSection> ObjPCLSections_GetList_Paging
        {
            get { return _ObjPCLSections_GetList_Paging; }
            set
            {
                _ObjPCLSections_GetList_Paging = value;
                NotifyOfPropertyChange(() => ObjPCLSections_GetList_Paging);
            }
        }

        private void PCLSections_GetList_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            ObjPCLSections_GetList_Paging.Clear();

            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách PCLSections..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLSections_GetList_Paging(SearchCriteriaSection, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DataEntities.PCLSection> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndPCLSections_GetList_Paging(out Total, asyncResult);
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

                            

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjPCLSections_GetList_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjPCLSections_GetList_Paging.Add(item);
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
                    IsLoading = false;  
                }
            });
            t.Start();
        }

        //public void Handle(RefDepartments_BystrV_DeptTypeEvent.TreeSelectionChanged_Event message)
        //{
        //     if (message != null)
        //     {
        //         DataEntities.RefDepartmentsTree NodeTree =message.ObjRefDepartments_Current as DataEntities.RefDepartmentsTree;
        //         if (NodeTree.ParentID != null)
        //         {
        //             SearchCriteriaSection.DeptID = NodeTree.NodeID;

        //            if(SearchCriteriaSection.PCLFormID>0)
        //            {
        //                ObjPCLSections_GetList_Paging.PageIndex = 0;
        //                PCLSections_GetList_Paging(0, ObjPCLSections_GetList_Paging.PageSize, true);   
        //            }

        //         }
        //     }
        //}

        //public void cboV_PCLCategory_SelectionChanged(object selectItem)
        //{
        //    if(selectItem!=null)
        //    {
        //        SearchCriteriaSection.PCLFormID = -1;//cbo PCLForm yêu cầu Text chọn sẵn


        //        PCLForm_GetList(0, 99999, true);//ko phân trang


        //        //Rỗng List PCLSections
        //        ObjPCLSections_GetList_Paging.Clear();

        //    }
        //}

        //public void cboPCLForm_SelectionChanged(object selectItem)
        //{
        //    if (selectItem != null)
        //    {
        //        ObjPCLSections_GetList_Paging.PageIndex = 0;
        //        PCLSections_GetList_Paging(0, ObjPCLSections_GetList_Paging.PageSize, true);
        //    }
        //}

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bhplEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mDanhMucPCLSessions,
                                               (int)oConfigurationEx.mQuanLyDMPCLSessions, (int)ePermission.mEdit);
            bhplDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mDanhMucPCLSessions,
                                               (int)oConfigurationEx.mQuanLyDMPCLSessions, (int)ePermission.mDelete);
            bbtSearch = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mDanhMucPCLSessions,
                                               (int)oConfigurationEx.mQuanLyDMPCLSessions, (int)ePermission.mView);
            bhplAddNew = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mDanhMucPCLSessions,
                                               (int)oConfigurationEx.mQuanLyDMPCLSessions, (int)ePermission.mAdd);
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
            ObjPCLSections_GetList_Paging.PageIndex = 0;
            PCLSections_GetList_Paging(0, ObjPCLSections_GetList_Paging.PageSize, true);
        }

        public void hplDelete_Click(object selectedItem)
        {
            DataEntities.PCLSection p = (selectedItem as DataEntities.PCLSection);

            if (p != null && p.PCLSectionID > 0)
            {
                if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, p.PCLSectionName), eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    PCLSections_MarkDelete(p.PCLSectionID);
                }
            }

        }

        private void PCLSections_MarkDelete(Int64 PCLSectionID)
        {
            string Result = "";

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Xóa..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLSections_MarkDelete(PCLSectionID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            contract.EndPCLSections_MarkDelete(out Result, asyncResult);
                            switch (Result)
                            {
                                case "Delete-0":
                                    {
                                        MessageBox.Show(eHCMSResources.K0484_G1_XoaFail, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Delete-1":
                                    {
                                        ObjPCLSections_GetList_Paging.PageIndex = 0;
                                        PCLSections_GetList_Paging(0, ObjPCLSections_GetList_Paging.PageSize, true);

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


        public void hplAddNew_Click()
        {
            //var typeInfo = Globals.GetViewModel<IPCLSections_AddEdit>();
            //typeInfo.TitleForm = "Thêm Mới PCLSection";

            //typeInfo.InitializeNewItem();

            //var instance = typeInfo as Conductor<object>;

            //Globals.ShowDialog(instance, (o) =>
            //{
            //    //làm gì đó
            //});

            Action<IPCLSections_AddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = "Thêm Mới PCLSection";

                typeInfo.InitializeNewItem();
            };
            GlobalsNAV.ShowDialog<IPCLSections_AddEdit>(onInitDlg);
        }


        public void hplEdit_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                //var typeInfo = Globals.GetViewModel<IPCLSections_AddEdit>();

                //typeInfo.TitleForm = "Hiệu Chỉnh (" + (selectedItem as DataEntities.PCLSection).PCLSectionName.Trim() + ")";

                //typeInfo.ObjPCLSections_Current = ObjectCopier.DeepCopy((selectedItem as DataEntities.PCLSection));

                //var instance = typeInfo as Conductor<object>;

                //Globals.ShowDialog(instance, (o) =>
                //{
                //    //làm gì đó
                //});

                Action<IPCLSections_AddEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.TitleForm = "Hiệu Chỉnh (" + (selectedItem as DataEntities.PCLSection).PCLSectionName.Trim() + ")";

                    typeInfo.ObjPCLSections_Current = ObjectCopier.DeepCopy((selectedItem as DataEntities.PCLSection));
                };
                GlobalsNAV.ShowDialog<IPCLSections_AddEdit>(onInitDlg);
            }
        }



        public void Handle(SaveEvent<bool> message)
        {
            if (message != null)
            {
                if (message.Result)
                {
                    ObjPCLSections_GetList_Paging.PageIndex = 0;
                    PCLSections_GetList_Paging(0, ObjPCLSections_GetList_Paging.PageSize, true);
                }
            }
        }
    }
}
