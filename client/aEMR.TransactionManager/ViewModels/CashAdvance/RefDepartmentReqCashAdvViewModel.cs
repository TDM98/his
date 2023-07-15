using eHCMSLanguage;
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts.Configuration;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using System.Threading;
using aEMR.ServiceClient;
using aEMR.Common.Collections;
using aEMR.Common;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;

namespace aEMR.Configuration.RefDepartmentReqCashAdv.ViewModels
{
    [Export(typeof(IRefDepartmentReqCashAdv)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RefDepartmentReqCashAdvViewModel : Conductor<object>, IRefDepartmentReqCashAdv
    , IHandle<RefDepartmentReqCashAdvEvent_AddEditSave>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public RefDepartmentReqCashAdvViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            authorization();
            RefDepartmentReqCashAdvLst = new PagedSortableCollectionView<DataEntities.RefDepartmentReqCashAdv>();
            RefDepartmentReqCashAdvLst.PageSize = Globals.PageSize;
            RefDepartmentReqCashAdvLst.OnRefresh += new EventHandler<RefreshEventArgs>(RefDepartmentReqCashAdvLst_OnRefresh);
            btnSearch();
        }

        void RefDepartmentReqCashAdvLst_OnRefresh(object sender, RefreshEventArgs e)
        {
            PCLRefDepartmentReqCashAdv_Search(RefDepartmentReqCashAdvLst.PageIndex, RefDepartmentReqCashAdvLst.PageSize);
        }

        #region Properties member

        private string _SearchText;
        public string SearchText
        {
            get { return _SearchText; }
            set
            {
                _SearchText = value;
                NotifyOfPropertyChange(() => SearchText);
            }
        }

        private PagedSortableCollectionView<DataEntities.RefDepartmentReqCashAdv> _RefDepartmentReqCashAdvLst;
        public PagedSortableCollectionView<DataEntities.RefDepartmentReqCashAdv> RefDepartmentReqCashAdvLst
        {
            get { return _RefDepartmentReqCashAdvLst; }
            set
            {
                _RefDepartmentReqCashAdvLst = value;
                NotifyOfPropertyChange(() => RefDepartmentReqCashAdvLst);
            }
        }

        #endregion

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bSearch = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mTransaction_Management
                                               , (int)eTransaction_Management.mGiaTamUng,
                                               (int)oTransaction_ManagementEx.mSearch, (int)ePermission.mView);
            bhplDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mTransaction_Management
                                               , (int)eTransaction_Management.mGiaTamUng,
                                               (int)oTransaction_ManagementEx.mDelete, (int)ePermission.mView);
            bhplAddNew = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mTransaction_Management
                                               , (int)eTransaction_Management.mGiaTamUng,
                                               (int)oTransaction_ManagementEx.mAddNew, (int)ePermission.mView);
        }

        #region checking account

        private bool _bSearch = true;
        private bool _bhplDelete = true;
        private bool _bhplAddNew = true;

        public bool bSearch
        {
            get
            {
                return _bSearch;
            }
            set
            {
                if (_bSearch == value)
                    return;
                _bSearch = value;
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

        #endregion
        #region binding visibilty

        public Button hplDelete { get; set; }
        public Button hplAddNew { get; set; }

        public void hplDelete_Loaded(object sender)
        {
            hplDelete = sender as Button;
            hplDelete.Visibility = Globals.convertVisibility(bhplDelete);
        }

        public void hplAddNew_Loaded(object sender)
        {
            hplAddNew = sender as Button;
            hplAddNew.Visibility = Globals.convertVisibility(bhplAddNew);
        }

        #endregion
        #region Function member

        public void hplDelete_Click(object datacontext)
        {
            DataEntities.RefDepartmentReqCashAdv p = (datacontext as DataEntities.RefDepartmentReqCashAdv);

            if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, p.DeptName), eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                PCLRefDepartmentReqCashAdv_Delete(p.RefDepartmentReqCashAdvID);
            }
        }

        public void hplAddNewExamTypeCombo_Click()
        {
            //var typeInfo = Globals.GetViewModel<IRefDepartmentReqCashAdv_AddEdit>();
            //typeInfo.TitleForm = eHCMSResources.G0276_G1_ThemMoi;
            //typeInfo.ObjRefDepartmentReqCashAdv_Current = new DataEntities.RefDepartmentReqCashAdv();

            //var instance = typeInfo as Conductor<object>;
            //Globals.ShowDialog(instance, (o) =>
            //{
            //    //làm gì đó
            //});

            Action<IRefDepartmentReqCashAdv_AddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = eHCMSResources.G0276_G1_ThemMoi;
                typeInfo.ObjRefDepartmentReqCashAdv_Current = new DataEntities.RefDepartmentReqCashAdv();
            };
            GlobalsNAV.ShowDialog<IRefDepartmentReqCashAdv_AddEdit>(onInitDlg);
        }

        public void DoubleClick(Common.EventArgs<object> e)
        {
            //DataEntities.RefDepartmentReqCashAdv item = e.Value as DataEntities.RefDepartmentReqCashAdv;
            //var typeInfo = Globals.GetViewModel<IRefDepartmentReqCashAdv_AddEdit>();
            //typeInfo.TitleForm = eHCMSResources.K1599_G1_CNhat;
            //typeInfo.IsAddNew = false;
            //typeInfo.ObjRefDepartmentReqCashAdv_Current = item;
            //var instance = typeInfo as Conductor<object>;
            //Globals.ShowDialog(instance, (o) =>
            //{
            //    //làm gì đó
            //});

            Action<IRefDepartmentReqCashAdv_AddEdit> onInitDlg = (typeInfo) =>
            {
                DataEntities.RefDepartmentReqCashAdv item = e.Value as DataEntities.RefDepartmentReqCashAdv;
                typeInfo.TitleForm = eHCMSResources.K1599_G1_CNhat;
                typeInfo.IsAddNew = false;
                typeInfo.ObjRefDepartmentReqCashAdv_Current = item;
            };
            GlobalsNAV.ShowDialog<IRefDepartmentReqCashAdv_AddEdit>(onInitDlg);

        }

        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        public void btnSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchText = (sender as TextBox).Text;
                btnSearch();
            }
        }

        public void btnSearch()
        {
            RefDepartmentReqCashAdvLst.PageIndex = 0;
            PCLRefDepartmentReqCashAdv_Search(RefDepartmentReqCashAdvLst.PageIndex, RefDepartmentReqCashAdvLst.PageSize);
        }

        private void PCLRefDepartmentReqCashAdv_Search(int PageIndex, int PageSize)
        {
            var t = new Thread(() =>
            {
                //IsLoading = true;

                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginRefDepartmentReqCashAdv_GetAll(SearchText, "", PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int Total = 0;
                                var allItems = client.EndRefDepartmentReqCashAdv_GetAll(out Total, asyncResult);
                                RefDepartmentReqCashAdvLst.Clear();
                                if (allItems != null && allItems.Count > 0)
                                {
                                    RefDepartmentReqCashAdvLst.TotalItemCount = Total;
                                    foreach (var item in allItems)
                                    {
                                        RefDepartmentReqCashAdvLst.Add(item);
                                    }
                                }
                            }
                            catch (Exception innerEx)
                            {
                                ClientLoggerHelper.LogInfo(innerEx.ToString());
                            }

                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
            });
            t.Start();
        }

        private void PCLRefDepartmentReqCashAdv_Delete(long ID)
        {
            var t = new Thread(() =>
            {
                //IsLoading = true;

                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginRefDepartmentReqCashAdv_Delete(ID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool bOK = client.EndRefDepartmentReqCashAdv_Delete(asyncResult);
                                if (bOK)
                                {
                                    MessageBox.Show(eHCMSResources.A0478_G1_Msg_InfoXoaOK);
                                    btnSearch();
                                }

                            }
                            catch (Exception innerEx)
                            {
                                ClientLoggerHelper.LogInfo(innerEx.ToString());
                            }

                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
            });
            t.Start();
        }
        #endregion

        public void Handle(RefDepartmentReqCashAdvEvent_AddEditSave message)
        {
            if (message != null)
            {
                btnSearch();
            }
        }
    }
}
