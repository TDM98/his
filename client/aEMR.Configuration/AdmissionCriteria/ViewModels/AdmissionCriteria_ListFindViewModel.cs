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
/*
 * #001 20180921 TNHX: Apply BusyIndicator, refactor code
 */
namespace aEMR.Configuration.AdmissionCriteria.ViewModels
{
    [Export(typeof(IAdmissionCriteria_ListFind)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class AdmissionCriteria_ListFindViewModel : ViewModelBase, IAdmissionCriteria_ListFind
        , IHandle<AdmissionCriteria_Event_Save>
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
        public AdmissionCriteria_ListFindViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            authorization();

            SearchCriteria = "";

            ObjAdmissionCriteria_Paging = new PagedSortableCollectionView<DataEntities.AdmissionCriteria>();
            ObjAdmissionCriteria_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjAdmissionCriteria_Paging_OnRefresh);

            ObjAdmissionCriteria_Paging.PageIndex = 0;
            AdmissionCriteria_Paging(ObjAdmissionCriteria_Paging.PageIndex,ObjAdmissionCriteria_Paging.PageSize, true);
        }

        void ObjAdmissionCriteria_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            AdmissionCriteria_Paging(ObjAdmissionCriteria_Paging.PageIndex,
                            ObjAdmissionCriteria_Paging.PageSize, false);
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

        private PagedSortableCollectionView<DataEntities.AdmissionCriteria> _ObjAdmissionCriteria_Paging;
        public PagedSortableCollectionView<DataEntities.AdmissionCriteria> ObjAdmissionCriteria_Paging
        {
            get { return _ObjAdmissionCriteria_Paging; }
            set
            {
                _ObjAdmissionCriteria_Paging = value;
                NotifyOfPropertyChange(() => ObjAdmissionCriteria_Paging);
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
            ObjAdmissionCriteria_Paging.PageIndex = 0;
            AdmissionCriteria_Paging(0, ObjAdmissionCriteria_Paging.PageSize, true);
        }

        public void AdmissionCriteria_MarkDeleted(DataEntities.AdmissionCriteria obj)
        {
            obj.IsActive = false;
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
                        contract.BeginAdmissionCriteria_Save(obj, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndAdmissionCriteria_Save(out Result, asyncResult);
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
                                    ObjAdmissionCriteria_Paging.PageIndex = 0;
                                    AdmissionCriteria_Paging(0, ObjAdmissionCriteria_Paging.PageSize, true);
                                    Globals.ShowMessage("Tạm dừng sử dụng tiêu chí" + obj.AdmissionCriteriaCode  , "Thông báo");
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

            DataEntities.AdmissionCriteria p = (selectedItem as DataEntities.AdmissionCriteria);
            if (p != null && p.AdmissionCriteriaID > 0)
            {
                if (MessageBox.Show(string.Format("Bạn có muốn dừng tiêu chí này", p.AdmissionCriteriaName), "Tạm dừng tiêu chí", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    AdmissionCriteria_MarkDeleted(p);
                }
            }
        }

        private object _AdmissionCriteria_Current;
        public object AdmissionCriteria_Current
        {
            get { return _AdmissionCriteria_Current; }
            set
            {
                _AdmissionCriteria_Current = value;
                NotifyOfPropertyChange(() => AdmissionCriteria_Current);
            }
        }
      

        private void AdmissionCriteria_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            this.DlgShowBusyIndicator("Danh sách tiêu chí");
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginAdmissionCriteria_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DataEntities.AdmissionCriteria> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndAdmissionCriteria_Paging(out Total, asyncResult);
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

                            ObjAdmissionCriteria_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjAdmissionCriteria_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjAdmissionCriteria_Paging.Add(item);
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

        public void hplAddNew_Click()
        {
            Action<IAdmissionCriteria_AddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = "THÊM MỚI TIÊU CHÍ VÀO VIỆN";
                typeInfo.InitializeNewItem();
            };
            GlobalsNAV.ShowDialog(onInitDlg);
        }

        public void hplEdit_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                Action<IAdmissionCriteria_AddEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.ObjAdmissionCriteria_Current = ObjectCopier.DeepCopy((selectedItem as DataEntities.AdmissionCriteria));
                    typeInfo.TitleForm = "Hiệu Chỉnh (" + (selectedItem as DataEntities.AdmissionCriteria).AdmissionCriteriaCode.Trim() + ")";
                };
                GlobalsNAV.ShowDialog<IAdmissionCriteria_AddEdit>(onInitDlg);
            }
        }

        public void Handle(AdmissionCriteria_Event_Save message)
        {
            ObjAdmissionCriteria_Paging.PageIndex = 0;
            AdmissionCriteria_Paging(0, ObjAdmissionCriteria_Paging.PageSize, true);
        }
    }
}
