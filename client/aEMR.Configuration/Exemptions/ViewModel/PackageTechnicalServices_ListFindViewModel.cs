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
using DataEntities.MedicalInstruction;
/*
* #001 20180921 TNHX: Apply BusyIndicator, refactor code
*/
namespace aEMR.Configuration.Exemptions.ViewModels
{
    [Export(typeof(IPackageTechnicalServices_ListFind)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PackageTechnicalServices_ListFindViewModel : ViewModelBase, IPackageTechnicalServices_ListFind
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
        public PackageTechnicalServices_ListFindViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            authorization();

            ObjPackageTechnicalService_Paging = new PagedSortableCollectionView<PackageTechnicalService>();
            ObjPackageTechnicalService_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjPackageTechnicalService_Paging_OnRefresh);
            PackageTechnicalServices_Paging(ObjPackageTechnicalService_Paging.PageIndex,
                           ObjPackageTechnicalService_Paging.PageSize, false);
        }

        void ObjPackageTechnicalService_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            PackageTechnicalServices_Paging(ObjPackageTechnicalService_Paging.PageIndex,
                            ObjPackageTechnicalService_Paging.PageSize, false);
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

        private PagedSortableCollectionView<PackageTechnicalService> _ObjPackageTechnicalService_Paging;
        public PagedSortableCollectionView<PackageTechnicalService> ObjPackageTechnicalService_Paging
        {
            get { return _ObjPackageTechnicalService_Paging; }
            set
            {
                _ObjPackageTechnicalService_Paging = value;
                NotifyOfPropertyChange(() => ObjPackageTechnicalService_Paging);
            }
        }

        public void cboRoomTypeSelectedItemChanged(object selectedItem)
        {
            ObjPackageTechnicalService_Paging.PageIndex = 0;
            PackageTechnicalServices_Paging(0, ObjPackageTechnicalService_Paging.PageSize, true);
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
            ObjPackageTechnicalService_Paging.PageIndex = 0;
            PackageTechnicalServices_Paging(0, ObjPackageTechnicalService_Paging.PageSize, true);
        }

        private void PackageTechnicalServices_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPackageTechnicalService_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<PackageTechnicalService> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndPackageTechnicalService_Paging(out Total, asyncResult);
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

                            ObjPackageTechnicalService_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjPackageTechnicalService_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjPackageTechnicalService_Paging.Add(item);
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

        public void hplAddNew_Click()
        {
            IPackageTechnicalServices_AddEdit ThePackageTechnicalServices_AddEditDlg = Globals.GetViewModel<IPackageTechnicalServices_AddEdit>();
            void onInitDlg(IPackageTechnicalServices_AddEdit typeInfo)
            {
                typeInfo.TitleForm = "Thêm Mới Gói DVKT";
                typeInfo.InitializeNewItem();
            }
            GlobalsNAV.ShowDialog_V3(ThePackageTechnicalServices_AddEditDlg, onInitDlg, null, false, true, Globals.GetHalfHeightAndThreeFourthWidthDefaultDialogViewSize());
        }

        public void hplEdit_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                IPackageTechnicalServices_AddEdit ThePackageTechnicalServices_AddEditDlg = Globals.GetViewModel<IPackageTechnicalServices_AddEdit>();
                void onInitDlg(IPackageTechnicalServices_AddEdit typeInfo)
                {
                    typeInfo.ObjPackageTechnicalServices_Current = ObjectCopier.DeepCopy((selectedItem as PackageTechnicalService));
                    typeInfo.TitleForm = "Hiệu Chỉnh (" + (selectedItem as PackageTechnicalService).Title.Trim() + ")";
                    typeInfo.InitializeItem();
                }
                GlobalsNAV.ShowDialog_V3(ThePackageTechnicalServices_AddEditDlg, onInitDlg, null, false, true, Globals.GetHalfHeightAndThreeFourthWidthDefaultDialogViewSize());
            }
        }

        public void Handle(Location_Event_Save message)
        {
            ObjPackageTechnicalService_Paging.PageIndex = 0;
            PackageTechnicalServices_Paging(0, ObjPackageTechnicalService_Paging.PageSize, true);
        }
    }
}
