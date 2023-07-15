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
using System.Linq;
/*
* 20221224 #001 BLQ: Create New
*/
namespace aEMR.Configuration.RefApplicationConfig.ViewModels
{
    [Export(typeof(IRefApplicationConfig_ListFind)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RefApplicationConfig_ListFindViewModel : ViewModelBase, IRefApplicationConfig_ListFind
        , IHandle<CRUDEvent>
    {
        protected override void OnActivate()
        {
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
        public RefApplicationConfig_ListFindViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            SearchRefApplicationConfig = "";
       
            ObjRefApplicationConfig_Paging = new PagedSortableCollectionView<DataEntities.RefApplicationConfig>();
            ObjRefApplicationConfig_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjRefApplicationConfig_Paging_OnRefresh);
            ObjRefApplicationConfig_Paging.PageSize = 20;
            RefApplicationConfig_Paging(0,ObjRefApplicationConfig_Paging.PageSize, true);
        }

        void ObjRefApplicationConfig_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            RefApplicationConfig_Paging(ObjRefApplicationConfig_Paging.PageIndex, ObjRefApplicationConfig_Paging.PageSize, false);
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
     
        private string _SearchRefApplicationConfig;
        public string SearchRefApplicationConfig
        {
            get
            {
                return _SearchRefApplicationConfig;
            }
            set
            {
                _SearchRefApplicationConfig = value;
                NotifyOfPropertyChange(() => SearchRefApplicationConfig);
            }
        }

        private PagedSortableCollectionView<DataEntities.RefApplicationConfig> _ObjRefApplicationConfig_Paging;
        public PagedSortableCollectionView<DataEntities.RefApplicationConfig> ObjRefApplicationConfig_Paging
        {
            get { return _ObjRefApplicationConfig_Paging; }
            set
            {
                _ObjRefApplicationConfig_Paging = value;
                NotifyOfPropertyChange(() => ObjRefApplicationConfig_Paging);
            }
        }

        #region binding visibilty
        public Button hplEdit { get; set; }
        public void hplEdit_Loaded(object sender)
        {
            hplEdit = sender as Button;
        }
       
        #endregion

        public void btSearch()
        {
            ObjRefApplicationConfig_Paging.PageIndex = 0;
            RefApplicationConfig_Paging(0, ObjRefApplicationConfig_Paging.PageSize, true);
        }

        private void RefApplicationConfig_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            this.DlgShowBusyIndicator("Danh sách diễn tiến bệnh");
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginRefApplicationConfig_Paging(SearchRefApplicationConfig, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DataEntities.RefApplicationConfig> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndRefApplicationConfig_Paging(out Total, asyncResult);
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

                            ObjRefApplicationConfig_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjRefApplicationConfig_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjRefApplicationConfig_Paging.Add(item);
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
        
        public void hplEdit_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                Action<IRefApplicationConfig_AddEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.ObjRefApplicationConfig_Current = ObjectCopier.DeepCopy((selectedItem as DataEntities.RefApplicationConfig));
                    typeInfo.TitleForm = "Hiệu Chỉnh (" + (selectedItem as DataEntities.RefApplicationConfig).ConfigItemKey.Trim() + ")";
                };
                GlobalsNAV.ShowDialog<IRefApplicationConfig_AddEdit>(onInitDlg);
            }
        }


        public void Handle(CRUDEvent message)
        {
            if (message != null)
            {
                RefApplicationConfig_Paging(ObjRefApplicationConfig_Paging.PageIndex, ObjRefApplicationConfig_Paging.PageSize,true);
                ReCachingCmd();
            }
        }
        public void ReCachingCmd()
        {
            this.ShowBusyIndicator();
            var mThread = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginReCaching(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            if (contract.EndReCaching(asyncResult))
                            {
                                //GlobalsNAV.ShowMessagePopup(eHCMSResources.Z2715_G1_ThanhCong);
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobalsNAV.ShowMessagePopup(ex.Message);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            mThread.Start();
        }
    }
}
