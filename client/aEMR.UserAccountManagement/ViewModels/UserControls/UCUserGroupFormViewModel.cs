using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Utilities;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Collections.Generic;
using aEMR.Common.Collections;

/*
 * #001 20180921 TNHX: Apply BusyIndicator, add allModulesTree for Parent's set then it set for child, refactor code
 */

namespace aEMR.UserAccountManagement.ViewModels
{
    [Export(typeof(IUCUserGroupForm)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class UCUserGroupFormViewModel : Conductor<object>, IUCUserGroupForm
        ,IHandle<allGroupChangeEvent>
        , IHandle<DeleteUserGroupCompletedEvent>
        , IHandle<GroupChangeCompletedEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public UCUserGroupFormViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            /*▼====: #001*/
            //GetAllUserAccount(0);
            //GetAllGroupByGroupID(0);
            /*▲====: #001*/

            _allUserGroup = new ObservableCollection<UserGroup>();
            GetAllUserGroupGetByID(0,0,1000,0,"",true);
            SelectedGroup=new Group();
            SelectedUserAccount=new UserAccount();
        }
        protected override void OnActivate()
        {
            base.OnActivate();    
            Globals.EventAggregator.Subscribe(this);
        }
        
        #region property

        private ObservableCollection<UserAccount> _allUserAccount;
        public ObservableCollection<UserAccount> allUserAccount
        {
            get
            {
                return _allUserAccount;
            }
            set
            {
                if (_allUserAccount == value)
                    return;
                _allUserAccount = value;
                NotifyOfPropertyChange(() => allUserAccount);
            }
        }

        private UserAccount _SelectedUserAccount;
        public UserAccount SelectedUserAccount
        {
            get
            {
                return _SelectedUserAccount;
            }
            set
            {
                if (_SelectedUserAccount == value)
                    return;
                _SelectedUserAccount = value;
                NotifyOfPropertyChange(() => SelectedUserAccount);
                NotifyOfPropertyChange(() => CanbutAdd);
            }
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
                NotifyOfPropertyChange(() =>allGroup);
            }
        }

        private Group _SelectedGroup;
        public Group SelectedGroup
        {
            get
            {
                return _SelectedGroup;
            }
            set
            {
                if (_SelectedGroup == value)
                    return;
                _SelectedGroup = value;
                NotifyOfPropertyChange(() => SelectedGroup);
                NotifyOfPropertyChange(() => CanbutAdd);
            }
        }

        private ObservableCollection<UserGroup> _allUserGroup;
        public ObservableCollection<UserGroup> allUserGroup
        {
            get
            {
                return _allUserGroup;
            }
            set
            {
                if (_allUserGroup == value)
                    return;
                _allUserGroup = value;
                NotifyOfPropertyChange(() => allUserGroup);
                
            }
        }

        private ObservableCollection<UserGroup> _allOldUserGroup;
        public ObservableCollection<UserGroup> allOldUserGroup
        {
            get
            {
                return _allOldUserGroup;
            }
            set
            {
                if (_allOldUserGroup == value)
                    return;
                _allOldUserGroup = value;
                NotifyOfPropertyChange(() => allOldUserGroup);
            }
        }

        private UserGroup _SelectedUserGroup;
        public UserGroup SelectedUserGroup
        {
            get
            {
                return _SelectedUserGroup;
            }
            set
            {
                if (_SelectedUserGroup == value)
                    return;
                _SelectedUserGroup = value;
                NotifyOfPropertyChange(()=>CanbutAdd);
            }
        }
        #endregion
        public bool CheckExist()
        {
            if (allOldUserGroup!=null)
            {
                foreach (var UG in allOldUserGroup)
                {
                    if (UG.AccountID == SelectedUserAccount.AccountID
                        && UG.GroupID == SelectedGroup.GroupID)
                    {
                        Globals.ShowMessage(eHCMSResources.Z1760_G1_ThemMoiNgDungVaoNhom, "");
                        return false;
                    }
                }    
            }
            
            return true;
        }
        public bool CheckExist(ObservableCollection<UserGroup> lstUG, UserGroup uG )
        {
            if (lstUG != null)
            {
                foreach (var UG in lstUG)
                {
                    if (UG.AccountID == uG.UserAccount.AccountID
                        && UG.GroupID == uG.Group.GroupID)
                    {
                        Globals.ShowMessage(eHCMSResources.Z1760_G1_ThemMoiNgDungVaoNhom, "");
                        return false;
                    }
                }
            }

            return true;
        }
        public bool CanbutAdd
        {
            get { return SelectedGroup.GroupID > 0 && SelectedUserAccount.AccountID > 0 ? true : false; }
        }
        public void butAdd()
        {
            if(CheckExist())
            {
                UserGroup UG = new UserGroup();
                UG.Group = new Group();
                UG.UserAccount = new UserAccount();
                UG.Group = SelectedGroup;
                UG.UserAccount = SelectedUserAccount;
                UG.GroupID = SelectedGroup.GroupID;
                UG.AccountID = SelectedUserAccount.AccountID;
                if (CheckExist(allUserGroup, UG))
                {
                    allUserGroup.Add(UG);        
                }
                
            }
            
        }
        public void butSave()
        {
            if (allUserGroup.Count > 0)
            {
                //for (int i = 0; i < allUserGroup.Count; i++)
                //{
                //    AddNewUserGroup(allUserGroup[i].UserAccount.AccountID
                //        , allUserGroup[i].Group.GroupID);
                //}
                AddNewUserGroupXML(allUserGroup);
                
            }
        }
        public void lnkDeleteClick(object sender,RoutedEvent e)
        {
            if (MessageBox.Show("Bạn có muốn xoá không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                allUserGroup.Remove(SelectedUserGroup);
            }
        }
        public void Handle(allGroupChangeEvent obj)
        {
            GetAllGroupByGroupID(0);
        }

        public void Handle(DeleteUserGroupCompletedEvent obj)
        {
            GetAllUserGroupGetByID(0, 0, 1000, 0, "", true);
        }

        public void Handle(GroupChangeCompletedEvent obj)
        {
            GetAllGroupByGroupID(0);
        }

        #region method
        public ObservableCollection<UserAccount> sortUserAccount(ObservableCollection<UserAccount> allUserAccount)
        {
            for (int i = 0; i < allUserAccount.Count - 1; i++)
            {
                for (int j = i + 1; j < allUserAccount.Count; j++)
                {
                    if (String.Compare(allUserAccount[i].AccountName, allUserAccount[j].AccountName) > 0)
                    {
                        UserAccount temp = allUserAccount[i];
                        allUserAccount[i] = allUserAccount[j];
                        allUserAccount[j] = temp;
                    }
                }
            }

            return allUserAccount;
        }

        /*▼====: #001*/
        private void GetAllUserAccount(long AccountID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllUserAccount(AccountID, Globals.DispatchCallback((asyncResult) =>
                         {
                             try
                             {
                                 var results = contract.EndGetAllUserAccount(asyncResult);
                                 if (results != null)
                                 {
                                     allUserAccount = results.ToObservableCollection();
                                     foreach (var p in results)
                                     {
                                         try
                                         {
                                                p.AccountName = EncryptExtension.Decrypt(p.AccountName, Globals.AxonKey, Globals.AxonPass);
                                                if (p.AccountName == "")
                                                {
                                                    p.AccountName = "Chưa Encrypt";
                                                    continue;
                                                }
                                         }
                                         catch
                                         {
                                             p.AccountName = "Chưa Encrypt";
                                         }
                                     }
                                     allUserAccount = sortUserAccount(allUserAccount);
                                     UserAccount usTemp = new UserAccount();
                                     usTemp.AccountID = -1;
                                     usTemp.AccountName = "-----Chọn Một User-----";
                                     SelectedUserAccount = usTemp;
                                     allUserAccount.Insert(0, usTemp);
                                     NotifyOfPropertyChange(() => allUserAccount);
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
                                     SelectedGroup = tempGroup;
                                     allGroup.Insert(0, tempGroup);
                                     NotifyOfPropertyChange(() => allGroup);
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
        
        private void AddNewUserGroup(long AccountID, int GroupID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginAddNewUserGroup(AccountID, GroupID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndAddNewUserGroup(asyncResult);
                                if (results == true)
                                {
                                    Globals.ShowMessage(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK, "");
                                    allUserGroup.Clear();
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

        private void AddNewUserGroupXML(ObservableCollection<UserGroup> lstUserGroup)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginAddNewUserGroupXML(lstUserGroup, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndAddNewUserGroupXML(asyncResult);
                                if (results == true)
                                {
                                    Globals.ShowMessage(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK, "");
                                    foreach (var userGroup in lstUserGroup)
                                    {
                                        allOldUserGroup.Add(userGroup);
                                    }
                                    allUserGroup.Clear();
                                    GetAllUserGroupGetByID(0, 0, 1000, 0, "", true);
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

        private void GetAllUserGroupGetByID(long AccountID, long GroupID
            , int PageSize, int PageIndex, string OrderBy, bool CountTotal)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllUserGroupGetByID(AccountID, GroupID
                                , PageSize, PageIndex, OrderBy, CountTotal, Globals.DispatchCallback((asyncResult) =>
                                {
                                    try
                                    {
                                        var results = contract.EndGetAllUserGroupGetByID(out int Total, asyncResult);
                                        if (results != null)
                                        {
                                            allOldUserGroup = results.ToObservableCollection();
                                            NotifyOfPropertyChange(() => allOldUserGroup);
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
        /*▲====: #001*/
        #endregion
    }
}
