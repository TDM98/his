/*
 * #001 20170303 CMN: Add Method Active sub view
 * #002 20180921 TNHX: Add method for set ModulesTree and AllGroup, apply BusyIndicator, refactor code
*/
using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows.Controls;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;

namespace aEMR.UserAccountManagement.ViewModels
{
    [Export(typeof(IModulesTab)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class ModulesTabViewModel : Conductor<object>, IModulesTab
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ModulesTabViewModel(IWindsorContainer container, INavigationService navigationService,  ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            authorization();
            initdata();
        }

        protected override void  OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            //==== 20161206 CMN Begin: Disable method called in onloaded
            //authorization();
            //==== 20161206 CMN End.
            /*▼====: #002*/
            _allModulesTree = new ObservableCollection<ModulesTree>();
            _allGroup = new ObservableCollection<Group>();
            GetModulesTreeView();
            GetAllGroupByGroupID(0);
            /*▲====: #002*/
            //==== #001
            if (bFunction)
                this.ActivateItem(FunctionForm);
            if (bOperation)
                this.ActivateItem(OperationForm);
            if (bRole)
                this.ActivateItem(RoleForm);
            if (bPermission)
                this.ActivateItem(PermissonForm);
            if (bGroup)
                this.ActivateItem(GroupForm);
            if (bGroupRole)
                this.ActivateItem(GroupRoleForm);
            if (bUserGroup)
                this.ActivateItem(UserGroupForm);
            //==== #001
        }
        //==== #001
        public void tabCommon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int Index = ((TabControl)sender).SelectedIndex;
            switch (Index)
            {
                case 0:
                    this.ActivateItem(FunctionForm);
                    break;
                case 1:
                    this.ActivateItem(OperationForm);
                    break;
                case 2:
                    this.ActivateItem(RoleForm);
                    break;
                case 3:
                    this.ActivateItem(PermissonForm);
                    break;
                case 4:
                    this.ActivateItem(GroupForm);
                    break;
                case 5:
                    this.ActivateItem(GroupRoleForm);
                    break;
                case 6:
                    this.ActivateItem(UserGroupForm);
                    break;
            }
        }
        //==== #001
        
        public void initdata()
        {
            if (bFunction)
            {
                var FunctionFormVM = Globals.GetViewModel<IFunctionForm>();
                FunctionForm = FunctionFormVM;
                //this.ActivateItem(FunctionForm);
            }

            if (bOperation)
            {
                var OperationFormVM = Globals.GetViewModel<IOperationForm>();
                OperationForm = OperationFormVM;
                //this.ActivateItem(OperationFormVM);
            }
            if (bRole)
            {
                var RoleFormVM = Globals.GetViewModel<IRoleForm>();
                RoleForm = RoleFormVM;
                //this.ActivateItem(RoleFormVM);
            }

            if (bPermission)
            {
                var PermissonFormVM = Globals.GetViewModel<IPermissonForm>();
                PermissonForm = PermissonFormVM;
                //this.ActivateItem(PermissonFormVM);
            }

            if (bGroup)
            {
                var GroupFormVM = Globals.GetViewModel<IGroupForm>();
                GroupForm = GroupFormVM;
                //this.ActivateItem(GroupFormVM);
            }

            if (bGroupRole)
            {
                var GroupRoleFormVM = Globals.GetViewModel<IGroupRoleTab>();
                GroupRoleForm = GroupRoleFormVM;
                //this.ActivateItem(GroupRoleFormVM);
            }

            if (bUserGroup)
            {
                var UserGroupFormVM = Globals.GetViewModel<IUserGroupTab>();
                UserGroupForm = UserGroupFormVM;
                //this.ActivateItem(UserGroupFormVM);
            }
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

            bModule = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mUserAccount
                                               , (int)eUserAccount.mPtListConfig,
                                               (int)oUserAccountEx.mModule, (int)ePermission.mView);
            bFunction = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mUserAccount
                                               , (int)eUserAccount.mPtListConfig,
                                               (int)oUserAccountEx.mFunction, (int)ePermission.mView);
            bRole = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mUserAccount
                                               , (int)eUserAccount.mPtListConfig,
                                               (int)oUserAccountEx.mRole, (int)ePermission.mView);
            bPermission = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mUserAccount
                                               , (int)eUserAccount.mPtListConfig,
                                               (int)oUserAccountEx.mPermission, (int)ePermission.mView);
            bGroup = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mUserAccount
                                               , (int)eUserAccount.mPtListConfig,
                                               (int)oUserAccountEx.mGroup, (int)ePermission.mView);
            bGroupRole = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mUserAccount
                                               , (int)eUserAccount.mPtListConfig,
                                               (int)oUserAccountEx.mGroupRole, (int)ePermission.mView);
            bUserGroup = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mUserAccount
                                               , (int)eUserAccount.mPtListConfig,
                                               (int)oUserAccountEx.mGroup, (int)ePermission.mView);

            bModuleAdd = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mUserAccount
                                               , (int)eUserAccount.mPtListConfig,
                                               (int)oUserAccountEx.mModule, (int)ePermission.mAdd);
            bFunctionAdd = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mUserAccount
                                               , (int)eUserAccount.mPtListConfig,
                                               (int)oUserAccountEx.mFunction, (int)ePermission.mAdd);
            bRoleAdd = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mUserAccount
                                               , (int)eUserAccount.mPtListConfig,
                                               (int)oUserAccountEx.mRole, (int)ePermission.mAdd);
            bPermissionAdd = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mUserAccount
                                               , (int)eUserAccount.mPtListConfig,
                                               (int)oUserAccountEx.mPermission, (int)ePermission.mAdd);
            bGroupAdd = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mUserAccount
                                               , (int)eUserAccount.mPtListConfig,
                                               (int)oUserAccountEx.mGroup, (int)ePermission.mAdd);
            bGroupRoleAdd = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mUserAccount
                                               , (int)eUserAccount.mPtListConfig,
                                               (int)oUserAccountEx.mGroupRole, (int)ePermission.mAdd);
            bUserGroupAdd = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mUserAccount
                                               , (int)eUserAccount.mPtListConfig,
                                               (int)oUserAccountEx.mUserGroup, (int)ePermission.mAdd);

            bModuleDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mUserAccount
                                               , (int)eUserAccount.mPtListConfig,
                                               (int)oUserAccountEx.mModule, (int)ePermission.mDelete);
            bFunctionDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mUserAccount
                                               , (int)eUserAccount.mPtListConfig,
                                               (int)oUserAccountEx.mFunction, (int)ePermission.mDelete);
            bRoleDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mUserAccount
                                               , (int)eUserAccount.mPtListConfig,
                                               (int)oUserAccountEx.mRole, (int)ePermission.mDelete);
            bPermissionDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mUserAccount
                                               , (int)eUserAccount.mPtListConfig,
                                               (int)oUserAccountEx.mPermission, (int)ePermission.mDelete);
            bGroupDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mUserAccount
                                               , (int)eUserAccount.mPtListConfig,
                                               (int)oUserAccountEx.mGroup, (int)ePermission.mDelete);
            bGroupRoleDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mUserAccount
                                               , (int)eUserAccount.mPtListConfig,
                                               (int)oUserAccountEx.mGroupRole, (int)ePermission.mDelete);
            bUserGroupDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mUserAccount
                                               , (int)eUserAccount.mPtListConfig,
                                               (int)oUserAccountEx.mUserGroup, (int)ePermission.mDelete);

            bModuleEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mUserAccount
                                               , (int)eUserAccount.mPtListConfig,
                                               (int)oUserAccountEx.mModule, (int)ePermission.mEdit);
            bFunctionEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mUserAccount
                                               , (int)eUserAccount.mPtListConfig,
                                               (int)oUserAccountEx.mFunction, (int)ePermission.mEdit);
            bRoleEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mUserAccount
                                               , (int)eUserAccount.mPtListConfig,
                                               (int)oUserAccountEx.mRole, (int)ePermission.mEdit);
            bPermissionEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mUserAccount
                                               , (int)eUserAccount.mPtListConfig,
                                               (int)oUserAccountEx.mPermission, (int)ePermission.mEdit);
            bGroupEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mUserAccount
                                               , (int)eUserAccount.mPtListConfig,
                                               (int)oUserAccountEx.mGroup, (int)ePermission.mEdit);
            bGroupRoleEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mUserAccount
                                               , (int)eUserAccount.mPtListConfig,
                                               (int)oUserAccountEx.mGroupRole, (int)ePermission.mEdit);
            bUserGroupEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mUserAccount
                                               , (int)eUserAccount.mPtListConfig,
                                               (int)oUserAccountEx.mUserGroup, (int)ePermission.mEdit);

            bOperation = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mUserAccount
                                               , (int)eUserAccount.mPtListConfig,
                                               (int)oUserAccountEx.mOperation, (int)ePermission.mView);
            bOperationAdd = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mUserAccount
                                               , (int)eUserAccount.mPtListConfig,
                                               (int)oUserAccountEx.mOperation, (int)ePermission.mAdd);
            bOperationDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mUserAccount
                                               , (int)eUserAccount.mPtListConfig,
                                               (int)oUserAccountEx.mOperation, (int)ePermission.mDelete);
            bOperationEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mUserAccount
                                               , (int)eUserAccount.mPtListConfig,
                                               (int)oUserAccountEx.mOperation, (int)ePermission.mEdit);

        }

        #region bool properties

        private bool _bModule = true;
        private bool _bFunction = true;
        private bool _bOperation = true;
        private bool _bRole = true;
        private bool _bPermission = true;
        private bool _bGroup = true;
        private bool _bGroupRole = true;
        private bool _bUserGroup = true;

        private bool _bModuleAdd = true;
        private bool _bFunctionAdd = true;
        private bool _bOperationAdd = true;
        private bool _bRoleAdd = true;
        private bool _bPermissionAdd = true;
        private bool _bGroupAdd = true;
        private bool _bGroupRoleAdd = true;
        private bool _bUserGroupAdd = true;

        private bool _bModuleDelete = true;
        private bool _bFunctionDelete = true;
        private bool _bOperationDelete = true;
        private bool _bRoleDelete = true;
        private bool _bPermissionDelete = true;
        private bool _bGroupDelete = true;
        private bool _bGroupRoleDelete = true;
        private bool _bUserGroupDelete = true;

        private bool _bModuleEdit = true;
        private bool _bFunctionEdit = true;
        private bool _bOperationEdit = true;
        private bool _bRoleEdit = true;
        private bool _bPermissionEdit = true;
        private bool _bGroupEdit = true;
        private bool _bGroupRoleEdit = true;
        private bool _bUserGroupEdit = true;

        public bool bOperation
        {
            get
            {
                return _bOperation;
            }
            set
            {
                if (_bOperation == value)
                    return;
                _bOperation = value;
            }
        }
        public bool bOperationAdd
        {
            get
            {
                return _bOperationAdd;
            }
            set
            {
                if (_bOperationAdd == value)
                    return;
                _bOperationAdd = value;
            }
        }
        public bool bOperationDelete
        {
            get
            {
                return _bOperationDelete;
            }
            set
            {
                if (_bOperationDelete == value)
                    return;
                _bOperationDelete = value;
            }
        }
        public bool bOperationEdit
        {
            get
            {
                return _bOperationEdit;
            }
            set
            {
                if (_bOperationEdit == value)
                    return;
                _bOperationEdit = value;
            }
        }

        public bool bModule
        {
            get
            {
                return _bModule;
            }
            set
            {
                if (_bModule == value)
                    return;
                _bModule = value;
            }
        }
        public bool bFunction
        {
            get
            {
                return _bFunction;
            }
            set
            {
                if (_bFunction == value)
                    return;
                _bFunction = value;
            }
        }
        public bool bRole
        {
            get
            {
                return _bRole;
            }
            set
            {
                if (_bRole == value)
                    return;
                _bRole = value;
            }
        }
        public bool bPermission
        {
            get
            {
                return _bPermission;
            }
            set
            {
                if (_bPermission == value)
                    return;
                _bPermission = value;
            }
        }
        public bool bGroup
        {
            get
            {
                return _bGroup;
            }
            set
            {
                if (_bGroup == value)
                    return;
                _bGroup = value;
            }
        }
        public bool bGroupRole
        {
            get
            {
                return _bGroupRole;
            }
            set
            {
                if (_bGroupRole == value)
                    return;
                _bGroupRole = value;
            }
        }
        public bool bUserGroup
        {
            get
            {
                return _bUserGroup;
            }
            set
            {
                if (_bUserGroup == value)
                    return;
                _bUserGroup = value;
            }
        }

        public bool bModuleAdd
        {
            get
            {
                return _bModuleAdd;
            }
            set
            {
                if (_bModuleAdd == value)
                    return;
                _bModuleAdd = value;
            }
        }
        public bool bFunctionAdd
        {
            get
            {
                return _bFunctionAdd;
            }
            set
            {
                if (_bFunctionAdd == value)
                    return;
                _bFunctionAdd = value;
            }
        }
        public bool bRoleAdd
        {
            get
            {
                return _bRoleAdd;
            }
            set
            {
                if (_bRoleAdd == value)
                    return;
                _bRoleAdd = value;
            }
        }
        public bool bPermissionAdd
        {
            get
            {
                return _bPermissionAdd;
            }
            set
            {
                if (_bPermissionAdd == value)
                    return;
                _bPermissionAdd = value;
            }
        }
        public bool bGroupAdd
        {
            get
            {
                return _bGroupAdd;
            }
            set
            {
                if (_bGroupAdd == value)
                    return;
                _bGroupAdd = value;
            }
        }
        public bool bGroupRoleAdd
        {
            get
            {
                return _bGroupRoleAdd;
            }
            set
            {
                if (_bGroupRoleAdd == value)
                    return;
                _bGroupRoleAdd = value;
            }
        }
        public bool bUserGroupAdd
        {
            get
            {
                return _bUserGroupAdd;
            }
            set
            {
                if (_bUserGroupAdd == value)
                    return;
                _bUserGroupAdd = value;
            }
        }

        public bool bModuleDelete
        {
            get
            {
                return _bModuleDelete;
            }
            set
            {
                if (_bModuleDelete == value)
                    return;
                _bModuleDelete = value;
            }
        }
        public bool bFunctionDelete
        {
            get
            {
                return _bFunctionDelete;
            }
            set
            {
                if (_bFunctionDelete == value)
                    return;
                _bFunctionDelete = value;
            }
        }
        public bool bRoleDelete
        {
            get
            {
                return _bRoleDelete;
            }
            set
            {
                if (_bRoleDelete == value)
                    return;
                _bRoleDelete = value;
            }
        }
        public bool bPermissionDelete
        {
            get
            {
                return _bPermissionDelete;
            }
            set
            {
                if (_bPermissionDelete == value)
                    return;
                _bPermissionDelete = value;
            }
        }
        public bool bGroupDelete
        {
            get
            {
                return _bGroupDelete;
            }
            set
            {
                if (_bGroupDelete == value)
                    return;
                _bGroupDelete = value;
            }
        }
        public bool bGroupRoleDelete
        {
            get
            {
                return _bGroupRoleDelete;
            }
            set
            {
                if (_bGroupRoleDelete == value)
                    return;
                _bGroupRoleDelete = value;
            }
        }
        public bool bUserGroupDelete
        {
            get
            {
                return _bUserGroupDelete;
            }
            set
            {
                if (_bUserGroupDelete == value)
                    return;
                _bUserGroupDelete = value;
            }
        }

        public bool bModuleEdit
        {
            get
            {
                return _bModuleEdit;
            }
            set
            {
                if (_bModuleEdit == value)
                    return;
                _bModuleEdit = value;
            }
        }
        public bool bFunctionEdit
        {
            get
            {
                return _bFunctionEdit;
            }
            set
            {
                if (_bFunctionEdit == value)
                    return;
                _bFunctionEdit = value;
            }
        }
        public bool bRoleEdit
        {
            get
            {
                return _bRoleEdit;
            }
            set
            {
                if (_bRoleEdit == value)
                    return;
                _bRoleEdit = value;
            }
        }
        public bool bPermissionEdit
        {
            get
            {
                return _bPermissionEdit;
            }
            set
            {
                if (_bPermissionEdit == value)
                    return;
                _bPermissionEdit = value;
            }
        }
        public bool bGroupEdit
        {
            get
            {
                return _bGroupEdit;
            }
            set
            {
                if (_bGroupEdit == value)
                    return;
                _bGroupEdit = value;
            }
        }
        public bool bGroupRoleEdit
        {
            get
            {
                return _bGroupRoleEdit;
            }
            set
            {
                if (_bGroupRoleEdit == value)
                    return;
                _bGroupRoleEdit = value;
            }
        }
        public bool bUserGroupEdit
        {
            get
            {
                return _bUserGroupEdit;
            }
            set
            {
                if (_bUserGroupEdit == value)
                    return;
                _bUserGroupEdit = value;
            }
        }
        #endregion
        public object FunctionForm { get; set; }
        public object OperationForm { get; set; }
        public object RoleForm { get; set; }
        public object PermissonForm { get; set; }
        public object GroupForm { get; set; }
        public object GroupRoleForm { get; set; }
        public object UserGroupForm { get; set; }

        /*▼====: #002*/
        private ObservableCollection<ModulesTree> _allModulesTree;
        public ObservableCollection<ModulesTree> allModulesTree
        {
            get
            {
                return _allModulesTree;
            }
            set
            {
                if (_allModulesTree == value)
                    return;
                _allModulesTree = value;
                NotifyOfPropertyChange(() => allModulesTree);
            }
        }

        private void GetModulesTreeView()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetModulesTreeView(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetModulesTreeView(asyncResult);
                                if (results != null)
                                {
                                    allModulesTree = results.ToObservableCollection();
                                    if (bFunction)
                                    {
                                        ((IFunctionForm)FunctionForm).allModulesTree = allModulesTree;
                                    }

                                    if (bOperation)
                                    {
                                        ((IOperationForm)OperationForm).allModulesTree = allModulesTree;
                                    }

                                    if (bPermission)
                                    {
                                        ((IPermissonForm)PermissonForm).allModulesTree = allModulesTree;
                                    }

                                    NotifyOfPropertyChange(() => allModulesTree);
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

        private ObservableCollection<Group> _allGroup;
        public ObservableCollection<Group> allGroup
        {
            get
            {
                return _allGroup;
            }
            set
            {
                if (_allGroup == value)
                    return;
                _allGroup = value;
                NotifyOfPropertyChange(() => allGroup);
            }
        }

        private void GetAllGroupByGroupID(long GroupID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllGroupByGroupID(GroupID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetAllGroupByGroupID(asyncResult);
                                if (results != null)
                                {
                                    allGroup = results.ToObservableCollection();
                                    Group tempGroup = new Group();
                                    tempGroup.GroupID = -1;
                                    tempGroup.GroupName = "-----Chọn Một Group-----";
                                    allGroup.Insert(0, tempGroup);

                                    if (bGroupRole)
                                    {
                                        ((IGroupRoleTab)GroupRoleForm).allGroup = allGroup;
                                    }

                                    if (bUserGroup)
                                    {
                                        ((IUserGroupTab)UserGroupForm).allGroup = allGroup;
                                    }
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
        /*▲====: #002*/
    }
}
