using aEMR.Common;
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using DataEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IShortHandDictionaryMapping)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ShortHandDictionaryMappingViewModel : ViewModelBase, IShortHandDictionaryMapping
         , IHandle<ShortHandDictionary_Event_Save>
    {
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
        }
        protected override void OnDeactivate(bool close)
        {
            Globals.EventAggregator.Unsubscribe(this);
            base.OnDeactivate(close);
        }
        [ImportingConstructor]
        public ShortHandDictionaryMappingViewModel(IWindsorContainer aContainer, INavigationService aNavigation, ISalePosCaching aCaching)
        {
            ListShortHandDictionary = new PagedSortableCollectionView<ShortHandDictionary>();
            ListShortHandDictionary.OnRefresh += new EventHandler<RefreshEventArgs>(ListShortHandDictionary_OnRefresh);
            ShortHandDictionary_Paging(0, ListShortHandDictionary.PageSize, true);
        }
        void ListShortHandDictionary_OnRefresh(object sender, RefreshEventArgs e)
        {
            ShortHandDictionary_Paging(ListShortHandDictionary.PageIndex,
                            ListShortHandDictionary.PageSize, false);
        }
        public void Handle(ShortHandDictionary_Event_Save message)
        {
            ListShortHandDictionary.PageIndex = 0;
            ShortHandDictionary_Paging(0, ListShortHandDictionary.PageSize, true);
        }
        private string _SearchValue;
        public string SearchValue
        {
            get
            {
                return _SearchValue;
            }
            set
            {
                _SearchValue = value;
                NotifyOfPropertyChange(() => SearchValue);
            }
        }
        private PagedSortableCollectionView<ShortHandDictionary> _ListShortHandDictionary;
        public PagedSortableCollectionView<ShortHandDictionary> ListShortHandDictionary
        {
            get
            {
                return _ListShortHandDictionary;
            }
            set
            {
                _ListShortHandDictionary = value;
                NotifyOfPropertyChange(() => ListShortHandDictionary);
            }
        }
        public void btSearchShortHandDictionary()
        {
            ListShortHandDictionary.PageIndex = 0;
            ShortHandDictionary_Paging(0, ListShortHandDictionary.PageSize, true);
        }
        public void hplAddNew_Click()
        {
            Action<IShortHandDictionaryAddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = "Thêm Mới Viết Tắt";
                typeInfo.InitializeNewItem();
            };
            GlobalsNAV.ShowDialog(onInitDlg);
        }
        public void hplEditChapter_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                Action<IShortHandDictionaryAddEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.ObjShortHandDictionary_Current = ObjectCopier.DeepCopy((selectedItem as ShortHandDictionary));
                    typeInfo.TitleForm = "Hiệu Chỉnh (" + (selectedItem as ShortHandDictionary).ShortHandDictionaryKey.Trim() + ")";
                };
                GlobalsNAV.ShowDialog<IShortHandDictionaryAddEdit>(onInitDlg);
            }
        }
        private void ShortHandDictionary_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách Phòng..." });
            this.DlgShowBusyIndicator("Danh sách từ điển");
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginShortHandDictionary_Paging(SearchValue,(long)Globals.LoggedUserAccount.StaffID, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<ShortHandDictionary> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndShortHandDictionary_Paging(out Total, asyncResult);
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

                            ListShortHandDictionary.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ListShortHandDictionary.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ListShortHandDictionary.Add(item);
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

    }
}
