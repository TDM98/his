using eHCMSLanguage;
using System;
using System.Net;
using System.Threading;
using System.Windows;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using Caliburn.Micro;
using aEMR.ViewContracts;
using System.ComponentModel.Composition;
using DataEntities;
using System.Collections.ObjectModel;
using aEMR.Common.Collections;
using aEMR.Common;

namespace aEMR.UserAccountManagement.ViewModels
{
    [Export(typeof(IDoctorAutho)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class DoctorAuthoViewModel : Conductor<object>, IDoctorAutho
        , IHandle<GetAllUserNameCompletedEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public DoctorAuthoViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            Innitailize();
        }

        public void Innitailize() 
        {
            curUserSubAuthorization = new UserSubAuthorization();
            IsProcessing = true;
            aucHoldConsultDoctor = Globals.GetViewModel<IAucHoldUserAccount>();
            aucHoldConsultSecretary = Globals.GetViewModel<IAucHoldUserAccount>();
            allUserSubAuthorization = new PagedSortableCollectionView<UserSubAuthorization>();
            allUserSubAuthorization.PageSize = 10;
            allUserSubAuthorization.PageIndex = 0;
            allUserSubAuthorization.OnRefresh += new EventHandler<RefreshEventArgs>(allUserSubAuthorization_OnRefresh);
            GetSubAuthorizationPaging(new UserSubAuthorization(), allUserSubAuthorization.PageIndex
                , allUserSubAuthorization.PageSize, true);
        }
        void allUserSubAuthorization_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetSubAuthorizationPaging(new UserSubAuthorization(), allUserSubAuthorization.PageIndex
                , allUserSubAuthorization.PageSize, true);
        }
        protected override void  OnActivate()
        {
 	         base.OnActivate();
             Globals.EventAggregator.Subscribe(this);
             //==== 20161206 CMN Begin: Disable method called in onloaded
             //Innitailize();
            //==== 20161206 CMN End.
        }

        private bool _IsProcessing;
        public bool IsProcessing
        {
            get { return _IsProcessing; }
            set
            {
                _IsProcessing = value;
                NotifyOfPropertyChange(() => IsProcessing);
            }
        }

        private UserSubAuthorization _selectedUserSubAuthorization;
        public UserSubAuthorization selectedUserSubAuthorization
        {
            get
            {
                return _selectedUserSubAuthorization;
            }
            set
            {
                if (_selectedUserSubAuthorization == value)
                    return;
                _selectedUserSubAuthorization = value;
                NotifyOfPropertyChange(() => selectedUserSubAuthorization);
            }
        }

        private IAucHoldUserAccount _aucHoldConsultDoctor;
        public IAucHoldUserAccount aucHoldConsultDoctor
        {
            get
            {
                return _aucHoldConsultDoctor;
            }
            set
            {
                if (_aucHoldConsultDoctor != value)
                {
                    _aucHoldConsultDoctor = value;
                    NotifyOfPropertyChange(() => aucHoldConsultDoctor);
                }
            }
        }

        private IAucHoldUserAccount _aucHoldConsultSecretary;
        public IAucHoldUserAccount aucHoldConsultSecretary
        {
            get
            {
                return _aucHoldConsultSecretary;
            }
            set
            {
                if (_aucHoldConsultSecretary != value)
                {
                    _aucHoldConsultSecretary = value;
                    NotifyOfPropertyChange(() => aucHoldConsultSecretary);
                }
            }
        }

        private PagedSortableCollectionView<UserSubAuthorization> _allUserSubAuthorization;
        public PagedSortableCollectionView<UserSubAuthorization> allUserSubAuthorization
        {
            get
            {
                return _allUserSubAuthorization;
            }
            set
            {
                if (_allUserSubAuthorization == value)
                    return;
                _allUserSubAuthorization = value;
                NotifyOfPropertyChange(() => allUserSubAuthorization);
            }
        }

        private UserSubAuthorization _curUserSubAuthorization;
        public UserSubAuthorization curUserSubAuthorization
        {
            get
            {
                return _curUserSubAuthorization;
            }
            set
            {
                if (_curUserSubAuthorization == value)
                    return;
                _curUserSubAuthorization = value;
                NotifyOfPropertyChange(() => curUserSubAuthorization);
            }
        }

        public void butUpdate()
        {
            if(aucHoldConsultDoctor.AccountID<1)
            {
                return;
            }
            if (aucHoldConsultSecretary.AccountID < 1)
            {
                return;
            }

            ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> result = new ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult>();
            if (!Validate(out result))
            {
                return;
            }

            UpdateUserSubAuthorization();
            //Globals.EventAggregator.Publish(new GroupChangeCompletedEvent{});
            //TryClose();
        }

        public void butAddNew()
        {
            if (aucHoldConsultDoctor.AccountID < 1)
            {
                return;
            }
            if (aucHoldConsultSecretary.AccountID < 1)
            {
                return;
            }
            curUserSubAuthorization.AccountIDAuth = aucHoldConsultDoctor.AccountID;
            curUserSubAuthorization.AccountIDSub = aucHoldConsultSecretary.AccountID;

            ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> result = new ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult>();
            if (!Validate(out result))
            {
                return;
            }
            curUserSubAuthorization.AuthPwd = EncryptExtension.Encrypt(curUserSubAuthorization.AuthPwd, Globals.AxonKey, Globals.AxonPass);
            AddUserSubAuthorization();
            //Globals.EventAggregator.Publish(new GroupChangeCompletedEvent{});
            //TryClose();
        }

        public void butCancel()
        {
            TryClose();
        }

        public void lnkDeleteClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Bạn có muốn xoá không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeleteUserSubAuthorization();
            }
        }
        public void lnkRefreshClick(object sender, RoutedEventArgs e)
        {
            allUserSubAuthorization.PageSize = 10;
            allUserSubAuthorization.PageIndex = 0;
            GetSubAuthorizationPaging(new UserSubAuthorization(), allUserSubAuthorization.PageIndex
                , allUserSubAuthorization.PageSize, true);
        }
        private void UpdateUserSubAuthorization()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new UserManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateUserSubAuthorization(curUserSubAuthorization, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndUpdateUserSubAuthorization(asyncResult);
                            if (results == true)
                            {
                                MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK);
                            }
                            GetSubAuthorizationPaging(new UserSubAuthorization(), allUserSubAuthorization.PageIndex
                                , allUserSubAuthorization.PageSize, true);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }
                    }), null);
                }
            });

            t.Start();
        }

        private void DeleteUserSubAuthorization()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new UserManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDeleteUserSubAuthorization(selectedUserSubAuthorization.SubUserAuthorizationID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndDeleteUserSubAuthorization(asyncResult);
                            if (results == true)
                            {
                                MessageBox.Show(eHCMSResources.K0537_G1_XoaOk);
                            }
                            GetSubAuthorizationPaging(new UserSubAuthorization(), allUserSubAuthorization.PageIndex
                                , allUserSubAuthorization.PageSize, true);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }
                    }), null);
                }
            });

            t.Start();
        }

        private void AddUserSubAuthorization()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new UserManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginAddUserSubAuthorization(curUserSubAuthorization, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndAddUserSubAuthorization(asyncResult);
                            if (results == true)
                            {
                                MessageBox.Show(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK);
                            }
                            GetSubAuthorizationPaging(new UserSubAuthorization(), allUserSubAuthorization.PageIndex
                                , allUserSubAuthorization.PageSize, true);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }
                    }), null);
                }
            });

            t.Start();
        }

        private void GetSubAuthorizationPaging(UserSubAuthorization userSubAuthorization, int PageIndex,int PageSize,bool bCountTotal)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new UserManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetUserSubAuthorizationPaging(userSubAuthorization, PageIndex,PageSize,bCountTotal
                        ,Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int total = 0;
                            var results = contract.EndGetUserSubAuthorizationPaging(out total, asyncResult);
                            allUserSubAuthorization.Clear();
                            if (results!=null)
                            {
                                foreach (var p in results)
                                {
                                    p.AccountAuth.AccountName = EncryptExtension.Decrypt(p.AccountAuth.AccountName, Globals.AxonKey, Globals.AxonPass);
                                        p.AccountSub.AccountName = EncryptExtension.Decrypt(p.AccountSub.AccountName, Globals.AxonKey, Globals.AxonPass);
                                        allUserSubAuthorization.Add(p);                                    
                                }
                            }
                            //==== 20161206 CMN Begin: Add missing variable
                            allUserSubAuthorization.TotalItemCount = total;
                            //==== 20161206 CMN End.
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }
                    }), null);
                }
            });

            t.Start();
        }

        private bool Validate(out ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> result)
        {
            result = new ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult>();
            //Se lay ngay tren server.
            DateTime today = DateTime.Now.Date;
            if (curUserSubAuthorization.AuthPwd != curUserSubAuthorization.ConfirmAuthPwd)
            {
                System.ComponentModel.DataAnnotations.ValidationResult item = new System.ComponentModel.DataAnnotations.ValidationResult("Confirm password sai!", new string[] { "Thông Báo!" });
                result.Add(item);
            }
            if (result.Count > 0)
            {
                return false;
            }
            return true;
        }

        public void Handle(GetAllUserNameCompletedEvent obj)
        {
            IsProcessing = false;
        }
    }
}

