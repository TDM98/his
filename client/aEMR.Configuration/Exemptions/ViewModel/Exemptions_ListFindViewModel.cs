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
namespace aEMR.Configuration.Exemptions.ViewModels
{
    [Export(typeof(IExemptions_ListFind)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class Exemptions_ListFindViewModel : ViewModelBase, IExemptions_ListFind
        , IHandle<Location_Event_Save>
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
        public Exemptions_ListFindViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            authorization();
            //Globals.EventAggregator.Subscribe(this);

            //ObjRoomType_GetAll = new ObservableCollection<DataEntities.RoomType>();
            //SearchCriteria = new LocationSearchCriteria();
            //SearchCriteria.RmTypeID = -1;
            //SearchCriteria.LocationName = "";
            //SearchCriteria.OrderBy = "";
            //RoomType_GetAll();

            ObjExamptions_Paging = new PagedSortableCollectionView<PromoDiscountProgram>();
            ObjExamptions_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjExamptions_Paging_OnRefresh);
            Examptions_Paging(ObjExamptions_Paging.PageIndex,
                           ObjExamptions_Paging.PageSize, false);
        }

        void ObjExamptions_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            Examptions_Paging(ObjExamptions_Paging.PageIndex,
                            ObjExamptions_Paging.PageSize, false);
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

        private PagedSortableCollectionView<PromoDiscountProgram> _ObjExamptions_Paging;
        public PagedSortableCollectionView<PromoDiscountProgram> ObjExamptions_Paging
        {
            get { return _ObjExamptions_Paging; }
            set
            {
                _ObjExamptions_Paging = value;
                NotifyOfPropertyChange(() => ObjExamptions_Paging);
            }
        }

        public void cboRoomTypeSelectedItemChanged(object selectedItem)
        {
            //SearchCriteria.RmTypeID = (selectedItem as DataEntities.RoomType).RmTypeID;
            ObjExamptions_Paging.PageIndex = 0;
            Examptions_Paging(0, ObjExamptions_Paging.PageSize, true);
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bhplEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mBangGiaDichVu,
                                               (int)oConfigurationEx.mBangGiaDichVu, (int)ePermission.mEdit);
            bhplDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mBangGiaDichVu,
                                               (int)oConfigurationEx.mBangGiaDichVu, (int)ePermission.mDelete);
            bbtSearch = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                        , (int)eConfiguration_Management.mBangGiaDichVu,
                                        (int)oConfigurationEx.mBangGiaDichVu, (int)ePermission.mView);
            bhplAddNew = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                , (int)eConfiguration_Management.mBangGiaDichVu,
                                                (int)oConfigurationEx.mBangGiaDichVu, (int)ePermission.mAdd);
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
            ObjExamptions_Paging.PageIndex = 0;
            Examptions_Paging(0, ObjExamptions_Paging.PageSize, true);
        }

        //public void Exemptions_MarkDeleted(Int64 PromoDiscProgID)
        //{
        //    string Result = "";
        //    this.DlgShowBusyIndicator(eHCMSResources.Z0492_G1_DangXoa);
        //    var t = new Thread(() =>
        //    {
        //        try
        //        {
        //            using (var serviceFactory = new ConfigurationManagerServiceClient())
        //            {
        //                var contract = serviceFactory.ServiceInstance;
        //                contract.BeginExemptions_MarkDeleted(PromoDiscProgID, Globals.DispatchCallback((asyncResult) =>
        //                {
        //                    try
        //                    {
        //                        contract.EndExemptions_MarkDeleted(out Result, asyncResult);
        //                        if (Result == "InUse")
        //                        {
        //                            Globals.ShowMessage(eHCMSResources.Z1318_G1_PhgDangSDungKgTheXoa, eHCMSResources.G2617_G1_Xoa);
        //                        }
        //                        if (Result == "0")
        //                        {
        //                            Globals.ShowMessage(eHCMSResources.K0484_G1_XoaFail, eHCMSResources.G2617_G1_Xoa);
        //                        }
        //                        if (Result == "1")
        //                        {
        //                            ObjExamptions_Paging.PageIndex = 0;
        //                            Examptions_Paging(0, ObjExamptions_Paging.PageSize, true);
        //                            Globals.ShowMessage(eHCMSResources.A0478_G1_Msg_InfoXoaOK, eHCMSResources.G2617_G1_Xoa);
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                    }
        //                    finally
        //                    {
        //                        this.DlgHideBusyIndicator();
        //                    }
        //                }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //            this.DlgHideBusyIndicator();
        //        }
        //    });
        //    t.Start();
        //}

        //public void hplDelete_Click(object selectedItem)
        //{
        //    PromoDiscountProgram p = (selectedItem as PromoDiscountProgram);
        //    if (p != null && p.PromoDiscProgID > 0)
        //    {
        //        if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, p.PromoDiscName), eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
        //        {
        //            Exemptions_MarkDeleted(p.PromoDiscProgID);
        //        }
        //    }
        //}

        private void Examptions_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách Phòng..." });
            this.DlgShowBusyIndicator("Danh sách miễn giảm");
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginExemptions_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<PromoDiscountProgram> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndExemptions_Paging(out Total, asyncResult);
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

                            ObjExamptions_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjExamptions_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjExamptions_Paging.Add(item);
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
            Action<IExemptions_AddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = "Thêm Mới Miễn Giảm";
                typeInfo.InitializeNewItem();
            };
            GlobalsNAV.ShowDialog<IExemptions_AddEdit>(onInitDlg);
        }

        public void hplEdit_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                Action<IExemptions_AddEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.ObjExemptions_Current = ObjectCopier.DeepCopy((selectedItem as PromoDiscountProgram));
                    typeInfo.TitleForm = "Hiệu Chỉnh (" + (selectedItem as DataEntities.PromoDiscountProgram).PromoDiscName.Trim() + ")";
                    typeInfo.InitializeItem();
                };
                GlobalsNAV.ShowDialog<IExemptions_AddEdit>(onInitDlg);
            }
        }

        public void Handle(Location_Event_Save message)
        {
            ObjExamptions_Paging.PageIndex = 0;
            Examptions_Paging(0, ObjExamptions_Paging.PageSize, true);
        }
    }
}
