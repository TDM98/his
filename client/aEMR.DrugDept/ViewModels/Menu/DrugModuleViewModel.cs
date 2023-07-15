using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using Castle.Windsor;
using System.Collections.Generic;
using aEMR.CommonTasks;
using DataEntities;
using System.Threading;
using aEMR.ServiceClient;
using System;
using eHCMSLanguage;
/*
* 20180920 #001 TTM: 
* 20220225 #002 QTD: Function get danh sách kho theo cấu hình trách nhiệm dời ra HomeViewModel để dùng chung cho các Module
*/
namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IDrugModule))]
    public class DrugModuleViewModel : Conductor<object>, IDrugModule
    {
        [ImportingConstructor]
        public DrugModuleViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Coroutine.BeginExecute(DoGetStore_ClinicDeptAll());
            this.Activated += new System.EventHandler<ActivationEventArgs>(DrugModuleViewModel_Activated);
            //authorization();
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            MainContent = null;
        }
        void DrugModuleViewModel_Activated(object sender, ActivationEventArgs e)
        {
            //Khi khoi tao module thi load menu ben trai luon.
            //var shell = Globals.GetViewModel<IHome>();
            //var leftMenu = Globals.GetViewModel<IDrugLeftMenu>();
            //shell.LeftMenu = leftMenu;
            //(shell as Conductor<object>).ActivateItem(leftMenu);

            //Khi khoi tao module thi load menu ben trai luon.
            var shell = Globals.GetViewModel<IHome>();
            var leftMenu = Globals.GetViewModel<IDrugLeftMenu>();
            shell.LeftMenu = leftMenu;
            (shell as Conductor<object>).ActivateItem(leftMenu);

            //▼====== #001: Khi khởi tạo DrugModule thì khởi tạo Top Menu ở trên luôn.
            DrugTopMenu = Globals.GetViewModel<IDrugTopMenu>();
            shell.TopMenuItems = DrugTopMenu;
            (shell as Conductor<object>).ActivateItem(DrugTopMenu);
            //▲====== #001
        }
        protected override void OnDeactivate(bool close)
        {
            DeactivateItem(MainContent, close);
            MainContent = null;
            base.OnDeactivate(close);
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
        private IEnumerator<IResult> DoGetStore_ClinicDeptAll()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_CLINIC, false, null, false, false);
            yield return paymentTypeTask;
            Globals.allRefStorageWarehouseLocation = paymentTypeTask.LookupList;
            yield break;
        }
        private IDrugTopMenu DrugTopMenu { get; set; }
        public bool[] MenuVisibleCollection
        {
            get
            {
                return DrugTopMenu == null ? null : DrugTopMenu.MenuVisibleCollection;
            }
            set
            {
                if (DrugTopMenu == null)
                {
                    return;
                }
                DrugTopMenu.MenuVisibleCollection = value;
            }
        }

        //▼====: #002
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

        //public void authorization()
        //{
        //    if (Globals.isAccountCheck)
        //    {
        //        var curStaffStoreDeptResponsibilities = new StaffStoreDeptResponsibilities();
        //        curStaffStoreDeptResponsibilities.StaffID = (long)Globals.LoggedUserAccount.StaffID;
        //        GetStaffStoreDeptResponsibilitiesByDeptID(curStaffStoreDeptResponsibilities, false);
        //    }
        //}
        //▲====: #002
    }
}