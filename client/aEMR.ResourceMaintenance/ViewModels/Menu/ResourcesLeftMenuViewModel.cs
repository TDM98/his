using eHCMSLanguage;
using System.Windows;
using System;
using System.Windows.Controls;
using aEMR.ViewContracts.ResourcesManage.Maintenance;
using Microsoft.Win32;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure;
using Caliburn.Micro;
using aEMR.ViewContracts;
using System.ComponentModel.Composition;
using DataEntities;

namespace aEMR.ResourceMaintenance.ViewModels
{
    [Export(typeof(IResourcesLeftMenu)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ResourcesLeftMenuViewModel : Conductor<object>, IResourcesLeftMenu
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ResourcesLeftMenuViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            Globals.PageName = "";
            Globals.TitleForm = "";
            authorization();
        }
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            
            bResourcesMedListCmd =Globals.CheckFunction(Globals.listRefModule,(int)eModules.mResources
                        ,(int)eResources.mPtDashboardResource);

            bResourcesOffListCmd = Globals.CheckFunction(Globals.listRefModule,(int)eModules.mResources
                        , (int)eResources.mPtDashboardResource_Office);

            bAllocResourcesMedCmd = Globals.CheckFunction(Globals.listRefModule,(int)eModules.mResources
                        , (int)eResources.mPtDashboardNewAllocations);

            bAllocResourcesOffCmd = Globals.CheckFunction(Globals.listRefModule,(int)eModules.mResources
                        , (int)eResources.mPtDashboardNewAllocations__Office);

            bTranfResourcesCmd = Globals.CheckFunction(Globals.listRefModule,(int)eModules.mResources
                        , (int)eResources.mPtDashboardNewTranfers);

            bResourceMaintenanceLog_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mResources_Maintenance
                        , (int)eResources_Maintenance.mListRequest);

            bResourceMaintenanceLog_AddNewMgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mResources_Maintenance
                        , (int)eResources_Maintenance.mAddNewRequest); 
        }
        #region bool properties
        private bool _bResourcesMedListCmd = true;
        private bool _bResourcesOffListCmd = true;
        private bool _bAllocResourcesMedCmd = true;
        private bool _bAllocResourcesOffCmd = true;
        private bool _bTranfResourcesCmd = true;
        private bool _bResourceMaintenanceLog_Mgnt = true;
        private bool _bResourceMaintenanceLog_AddNewMgnt = true;

        public bool bResourcesMedListCmd
        {
            get
            {
                return _bResourcesMedListCmd;
            }
            set
            {
                if (_bResourcesMedListCmd == value)
                    return;
                _bResourcesMedListCmd = value;
            }
        }
        public bool bResourcesOffListCmd
        {
            get
            {
                return _bResourcesOffListCmd;
            }
            set
            {
                if (_bResourcesOffListCmd == value)
                    return;
                _bResourcesOffListCmd = value;
            }
        }
        public bool bAllocResourcesMedCmd
        {
            get
            {
                return _bAllocResourcesMedCmd;
            }
            set
            {
                if (_bAllocResourcesMedCmd == value)
                    return;
                _bAllocResourcesMedCmd = value;
            }
        }
        public bool bAllocResourcesOffCmd
        {
            get
            {
                return _bAllocResourcesOffCmd;
            }
            set
            {
                if (_bAllocResourcesOffCmd == value)
                    return;
                _bAllocResourcesOffCmd = value;
            }
        }
        public bool bTranfResourcesCmd
        {
            get
            {
                return _bTranfResourcesCmd;
            }
            set
            {
                if (_bTranfResourcesCmd == value)
                    return;
                _bTranfResourcesCmd = value;
            }
        }
        public bool bResourceMaintenanceLog_Mgnt
        {
            get
            {
                return _bResourceMaintenanceLog_Mgnt;
            }
            set
            {
                if (_bResourceMaintenanceLog_Mgnt == value)
                    return;
                _bResourceMaintenanceLog_Mgnt = value;
            }
        }
        public bool bResourceMaintenanceLog_AddNewMgnt
        {
            get
            {
                return _bResourceMaintenanceLog_AddNewMgnt;
            }
            set
            {
                if (_bResourceMaintenanceLog_AddNewMgnt == value)
                    return;
                _bResourceMaintenanceLog_AddNewMgnt = value;
            }
        }
        #endregion
        private void ResourcesMedListCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            var regModule = Globals.GetViewModel<IResourcesHome>();
            var regVm = Globals.GetViewModel<IResourcesListGrid>();
            regVm.ResourceCategoryEnum = (int) AllLookupValues.ResGroupCategory.THIET_BI_Y_TE;
            regModule.mainContent = regVm;
            this.ActivateItem(regVm);
            var regNewGroup = Globals.GetViewModel<IResGroupNew>();
            this.ActivateItem(regNewGroup);
            (regModule as Conductor<object>).ActivateItem(regVm);
            
            //Globals.LoadDynamicModule<IResourcesListGrid>("eHCMS.ResourceMaintenance.xap");
        }

        public void ResourcesMedListCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K2916_G1_DMucVTYT;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ResourcesMedListCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ResourcesMedListCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ResourcesOffListCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IResourcesHome>();
            var regVm = Globals.GetViewModel<IResourcesListGrid>();
            regVm.ResourceCategoryEnum = (int)AllLookupValues.ResGroupCategory.THIET_BI_VAN_PHONG;
            regModule.mainContent = regVm;
            this.ActivateItem(regVm);
            var regNewGroup = Globals.GetViewModel<IResGroupNew>();
            this.ActivateItem(regNewGroup);
            (regModule as Conductor<object>).ActivateItem(regVm);
        }

        public void ResourcesOffListCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K2915_G1_DMucVTVP;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ResourcesOffListCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ResourcesOffListCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void AllocResourcesMedCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;
            var home = Globals.GetViewModel<IResourcesHome>();

            var allocPage = Globals.GetViewModel<IAllocHome>();
            allocPage.ResourceCategoryEnum = (int)AllLookupValues.ResGroupCategory.THIET_BI_Y_TE;
            home.mainContent = allocPage;

            (home as Conductor<object>).ActivateItem(allocPage);
        }

        public void AllocResourcesMedCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z1807_G1_PBoVTYT;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                AllocResourcesMedCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        AllocResourcesMedCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void AllocResourcesOffCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;
            var home = Globals.GetViewModel<IResourcesHome>();

            var allocPage = Globals.GetViewModel<IAllocHome>();
            allocPage.ResourceCategoryEnum= (int)AllLookupValues.ResGroupCategory.THIET_BI_VAN_PHONG;
            home.mainContent = allocPage;

            (home as Conductor<object>).ActivateItem(allocPage);
            //Globals.LoadDynamicModule<IAllocHome>("eHCMS.ResourceMaintenance.xap");
        }

        public void AllocResourcesOffCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z1808_G1_PBoVTVP;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                AllocResourcesOffCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        AllocResourcesOffCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void TranfResourcesCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;
            var home = Globals.GetViewModel<IResourcesHome>();

            var tranfPage = Globals.GetViewModel<ITranfHome>();
            tranfPage.IsChildWindowForChonDiBaoTri = true;

            home.mainContent = tranfPage;

            (home as Conductor<object>).ActivateItem(tranfPage);
            //Globals.LoadDynamicModule<ITranfHome>("eHCMS.ResourceMaintenance.xap");
        }

        public void TranfResourcesCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K3498_G1_DChuyenVT;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TranfResourcesCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TranfResourcesCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ResourceMaintenanceLog_Mgnt_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;
            var home = Globals.GetViewModel<IResourcesHome>();

            var tranfPage = Globals.GetViewModel<IResourceMaintenanceLog>();

            home.mainContent = tranfPage;

            (home as Conductor<object>).ActivateItem(tranfPage);
            //Globals.LoadDynamicModule<ITranfHome>("eHCMS.ResourceMaintenance.xap");
        }

        public void ResourceMaintenanceLog_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.K3098_G1_DSVTBTri;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ResourceMaintenanceLog_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ResourceMaintenanceLog_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ResourceMaintenanceLog_AddNewMgnt_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            var home = Globals.GetViewModel<IResourcesHome>();

            var tranfPage = Globals.GetViewModel<IResourceMaintenanceLog_Add>();
            tranfPage.IsShowAsChildWindow = false;
            tranfPage.btChooseResourceIsEnabled = true;

            home.mainContent = tranfPage;

            (home as Conductor<object>).ActivateItem(tranfPage);
            //Globals.LoadDynamicModule<ITranfHome>("eHCMS.ResourceMaintenance.xap");
        }
        public void ResourceMaintenanceLog_AddNewMgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.T0819_G1_TaoYeuCauBTri;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ResourceMaintenanceLog_AddNewMgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ResourceMaintenanceLog_AddNewMgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        #region menu
        protected override void OnActivate()
        {
            base.OnActivate();
            authorization();
            Globals.EventAggregator.Subscribe(this);
            if (_currentView != null)
            {
                _currentView.ResetMenuColor();
            }
        }

        ILeftMenuView _currentView;
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            _currentView = view as ILeftMenuView;
            if (_currentView != null)
            {
                _currentView.ResetMenuColor();
            }
        }
        private void SetHyperlinkSelectedStyle(Button lnk)
        {
            if (_currentView != null)
            {
                _currentView.ResetMenuColor();
            }
            if (lnk != null)
            {
                lnk.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];
            }
        }
        #endregion
    }
}
