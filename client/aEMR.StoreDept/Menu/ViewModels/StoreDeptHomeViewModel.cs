using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using aEMR.CommonTasks;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using DataEntities;

namespace aEMR.StoreDept.Menu.ViewModels
{
    [Export(typeof(IStoreDeptHome)), PartCreationPolicy(CreationPolicy.Shared)]
    public class StoreDeptHomeViewModel : Conductor<object>, IStoreDeptHome
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public StoreDeptHomeViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            authorization();
            this.Activated += new System.EventHandler<ActivationEventArgs>(StoreDeptHomeViewModel_Activated);
        }
        //public void authorization()
        //{
        //    if(!Globals.isAccountCheck)
        //    {
        //        Coroutine.BeginExecute(DoGetStore_ClinicDeptAll());
        //    }
        //    else
        //    {
        //        var curStaffStoreDeptResponsibilities = new StaffStoreDeptResponsibilities();
        //        curStaffStoreDeptResponsibilities.StaffID = (long)Globals.LoggedUserAccount.StaffID;
        //        GetStaffStoreDeptResponsibilitiesByDeptID(curStaffStoreDeptResponsibilities, false);    
        //    }
        //}

        public void authorization()
        {
            Coroutine.BeginExecute(DoGetStore_ClinicDeptAll());
            Coroutine.BeginExecute(DoGetStore_IsMainAll());
            //if (Globals.isAccountCheck)
            //{
            //    var curStaffStoreDeptResponsibilities = new StaffStoreDeptResponsibilities();
            //    curStaffStoreDeptResponsibilities.StaffID = (long)Globals.LoggedUserAccount.StaffID;
            //    GetStaffStoreDeptResponsibilitiesByDeptID(curStaffStoreDeptResponsibilities, false);
            //}
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            MainContent = null;
        }
        void StoreDeptHomeViewModel_Activated(object sender, ActivationEventArgs e)
        {
            //Khi khoi tao module thi load menu ben trai luon.
            var shell = Globals.GetViewModel<IHome>();
            var leftMenu = Globals.GetViewModel<IStoreDeptLeftMenu>();
            var topMenu = Globals.GetViewModel<IStoreDeptTopMenu>();
            shell.TopMenuItems = topMenu;
            shell.LeftMenu = leftMenu;
            //(shell as Conductor<object>).ActivateItem(leftMenu);
            (shell as Conductor<object>).ActivateItem(topMenu);
        }

        private object _mainContent;
        public object MainContent
        {
            get { return _mainContent; }
            set
            {
                _mainContent = value;
                NotifyOfPropertyChange(() => MainContent);
            }
        }

        //private void GetStaffStoreDeptResponsibilitiesByDeptID(StaffStoreDeptResponsibilities p, bool isHis)
        //{
        //    //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
        //    this.ShowBusyIndicator();
        //    var t = new Thread(() =>
        //    {
        //        using (var serviceFactory = new UserAccountsServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;
        //            contract.BeginGetStaffStoreDeptResponsibilitiesByDeptID(p, isHis, Globals.DispatchCallback((asyncResult) =>
        //            {
        //                Globals.allStaffStoreResponsibilities = new List<long>();
        //                //List<long> allStaffStoreDeptResponsibilities = new List<long>();
        //                try
        //                {
        //                    var results = contract.EndGetStaffStoreDeptResponsibilitiesByDeptID(asyncResult);

        //                    if (results != null && results.Count > 0)
        //                    {
        //                        foreach (var item in results)
        //                        {
        //                            //allStaffStoreDeptResponsibilities.Add(item.StoreID);
        //                            Globals.allStaffStoreResponsibilities.Add(item.StoreID);
        //                        }
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                }
        //                finally
        //                {
        //                    //Coroutine.BeginExecute(DoGetStore_ClinicDept(allStaffStoreDeptResponsibilities));
        //                    this.HideBusyIndicator();
        //                }

        //            }), null);
        //        }
        //    });

        //    t.Start();
        //}

        //private IEnumerator<IResult> DoGetStore_ClinicDept(List<long> allStaffStoreDeptResponsibilities)
        //{
        //    var paymentTypeTask = new LoadListStoreForResponTask(allStaffStoreDeptResponsibilities,(long)AllLookupValues.StoreType.STORAGE_CLINIC, false, true, false);
        //    yield return paymentTypeTask;
        //    Globals.allRefStorageWarehouseLocation= paymentTypeTask.LookupList;
        //    yield break;
        //}
        private IEnumerator<IResult> DoGetStore_ClinicDeptAll()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_CLINIC, false, null, false, false);
            yield return paymentTypeTask;
            Globals.allRefStorageWarehouseLocation = paymentTypeTask.LookupList;
            yield break;
        }
        private IEnumerator<IResult> DoGetStore_IsMainAll()
        {
            var IsMainpaymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT, false, null, false, false);
            yield return IsMainpaymentTypeTask;
            Globals.IsMainStorageWarehouseLocation = IsMainpaymentTypeTask.LookupList;
            yield break;
        }
    }
}
