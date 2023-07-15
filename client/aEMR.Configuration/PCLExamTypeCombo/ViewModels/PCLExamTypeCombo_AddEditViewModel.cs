using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
using eHCMSLanguage;
using aEMR.ViewContracts;

namespace aEMR.Configuration.PCLExamTypeCombo.ViewModels
{
    [Export(typeof(IPCLExamTypeCombo_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLExamTypeCombo_AddEditViewModel : Conductor<object>, IPCLExamTypeCombo_AddEdit
    {
        #region Properties member
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private string _TitleForm;
        public string TitleForm
        {
            get { return _TitleForm; }
            set
            {
                _TitleForm = value;
                NotifyOfPropertyChange(() => TitleForm);
            }
        }

        private List<DataEntities.PCLExamTypeComboItem> DataPCLExamTypeComboItem_Insert { get; set; }
        private List<DataEntities.PCLExamTypeComboItem> DataPCLExamTypeComboItem_Update { get; set; }
        private List<DataEntities.PCLExamTypeComboItem> DataPCLExamTypeComboItem_Delete { get; set; }

        private DataEntities.PCLExamTypeCombo _ObjPCLExamTypeCombo_Current;
        public DataEntities.PCLExamTypeCombo ObjPCLExamTypeCombo_Current
        {
            get { return _ObjPCLExamTypeCombo_Current; }
            set
            {
                _ObjPCLExamTypeCombo_Current = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypeCombo_Current);
            }
        }

        private PagedSortableCollectionView<DataEntities.PCLExamType> _ObjPCLExamTypes_Paging;
        public PagedSortableCollectionView<DataEntities.PCLExamType> ObjPCLExamTypes_Paging
        {
            get { return _ObjPCLExamTypes_Paging; }
            set
            {
                _ObjPCLExamTypes_Paging = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypes_Paging);
            }
        }

        private PCLExamTypeSearchCriteria _SearchCriteria;
        public PCLExamTypeSearchCriteria SearchCriteria
        {
            get { return _SearchCriteria; }
            set
            {
                _SearchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }

        #endregion
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PCLExamTypeCombo_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Globals.EventAggregator.Subscribe(this);

            SearchCriteria = new PCLExamTypeSearchCriteria();
            SearchCriteria.V_PCLMainCategory = (long)AllLookupValues.V_PCLMainCategory.Laboratory;

            ObjPCLExamTypes_Paging = new PagedSortableCollectionView<PCLExamType>();
            ObjPCLExamTypes_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjPCLExamTypes_Paging_OnRefresh);

            PCLExamTypes_Paging(ObjPCLExamTypes_Paging.PageIndex, ObjPCLExamTypes_Paging.PageSize, true);

            DataPCLExamTypeComboItem_Insert = new List<PCLExamTypeComboItem>();
            DataPCLExamTypeComboItem_Update = new List<PCLExamTypeComboItem>();
            DataPCLExamTypeComboItem_Delete = new List<PCLExamTypeComboItem>();
        }

        void ObjPCLExamTypes_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            PCLExamTypes_Paging(ObjPCLExamTypes_Paging.PageIndex, ObjPCLExamTypes_Paging.PageSize, true);
        }

        private void PCLExamTypes_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            ObjPCLExamTypes_Paging.Clear();
            var t = new Thread(() =>
            {
                IsLoading = true;

                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLExamTypes_List_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DataEntities.PCLExamType> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndPCLExamTypes_List_Paging(out Total, asyncResult);
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


                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjPCLExamTypes_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjPCLExamTypes_Paging.Add(item);
                                    }

                                }
                            }
                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
                finally
                {
                    //Globals.IsBusy = false;
                    IsLoading = false;
                }
            });
            t.Start();
        }

        public void PCLExamTypeCombo_GetDetails(long ID)
        {
            var t = new Thread(() =>
            {
                IsLoading = true;

                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLExamTypeComboItems_ByComboID(ID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var allItems = client.EndPCLExamTypeComboItems_ByComboID(asyncResult);
                                ObjPCLExamTypeCombo_Current.PCLExamTypeComboItems = allItems.ToObservableCollection();
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
                finally
                {
                    //Globals.IsBusy = false;
                    IsLoading = false;
                }
            });
            t.Start();
        }

   

        private void CalcDataXML(string sKey, DataEntities.PCLExamTypeComboItem Obj)
        {
            switch (sKey)
            {
                case "I":
                    {
                        DataPCLExamTypeComboItem_Insert.Add(Obj);
                        break;
                    }
                case "D":
                    {
                        if (ObjPCLExamTypeCombo_Current.PCLExamTypeComboID > 0)
                        {
                            if (Obj.PCLExamTypeComboItemID > 0)
                            {
                                DataPCLExamTypeComboItem_Delete.Add(Obj);
                            }
                        }
                        break;
                    }
            }
        }

        public void btSave()
        {
            if (CheckValidPCLExamType(ObjPCLExamTypeCombo_Current) == false)
                return;
            PCLExamTypeCombo_Save();
        }

        public bool CheckValidPCLExamType(object temp)
        {
            DataEntities.PCLExamTypeCombo p = temp as DataEntities.PCLExamTypeCombo;
            if (p == null)
            {
                return false;
            }
            return p.Validate();
        }

        public void btClose()
        {
            TryClose();
        }

        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        public void btnSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.PCLExamTypeName = (sender as TextBox).Text;
                }
                ObjPCLExamTypes_Paging.PageIndex = 0;
                PCLExamTypes_Paging(ObjPCLExamTypes_Paging.PageIndex, ObjPCLExamTypes_Paging.PageSize, true);
            }
        }

        public void btSearch()
        {
            ObjPCLExamTypes_Paging.PageIndex = 0;
            PCLExamTypes_Paging(ObjPCLExamTypes_Paging.PageIndex, ObjPCLExamTypes_Paging.PageSize, true);
        }

        private bool CheckTrung(DataEntities.PCLExamType ObjCheck)
        {
            ObservableCollection<DataEntities.PCLExamTypeComboItem> ObjPCLExamTypeComboItemListTMP = new ObservableCollection<DataEntities.PCLExamTypeComboItem>();
            ObjPCLExamTypeComboItemListTMP = ObjectCopier.DeepCopy(ObjPCLExamTypeCombo_Current.PCLExamTypeComboItems);

            foreach (DataEntities.PCLExamTypeComboItem item in ObjPCLExamTypeComboItemListTMP)
            {
                if (item.PCLExamType.PCLExamTypeID == ObjCheck.PCLExamTypeID)
                {
                    return true;
                }
            }
            return false;
        }

        public void hplDelete_Click(object datacontext)
        {
            DataEntities.PCLExamTypeComboItem p = (datacontext as DataEntities.PCLExamTypeComboItem);

            if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, p.PCLExamType.PCLExamTypeName.Trim()), eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                ObjPCLExamTypeCombo_Current.PCLExamTypeComboItems.Remove(p);

                CalcDataXML("D", p);
            }
        }

        public void DoubleClick(Common.EventArgs<object> e)
        {
            PCLExamType item = e.Value as PCLExamType;
            if (item != null)
            {
                if (ObjPCLExamTypeCombo_Current == null)
                {
                    ObjPCLExamTypeCombo_Current = new DataEntities.PCLExamTypeCombo();
                    ObjPCLExamTypeCombo_Current.PCLExamTypeComboItems = new ObservableCollection<PCLExamTypeComboItem>();
                }
                if (ObjPCLExamTypeCombo_Current.PCLExamTypeComboItems == null)
                {
                    ObjPCLExamTypeCombo_Current.PCLExamTypeComboItems = new ObservableCollection<PCLExamTypeComboItem>();
                }
                //neu code do chua co trong danh sach xoa,thi lay lai tu ds xoa
                bool exists = false;
                if (DataPCLExamTypeComboItem_Delete != null && DataPCLExamTypeComboItem_Delete.Count > 0)
                {
                    foreach (PCLExamTypeComboItem p in DataPCLExamTypeComboItem_Delete)
                    {
                        if (p.PCLExamType.PCLExamTypeID == item.PCLExamTypeID)
                        {
                            exists = true;
                            DataPCLExamTypeComboItem_Delete.Remove(p);
                            ObjPCLExamTypeCombo_Current.PCLExamTypeComboItems.Add(p);
                            break;
                        }
                    }
                }
                if (!exists)
                {
                    //dat ham kiem tra trung o day ne
                    if (CheckTrung(item) == false)
                    {
                        PCLExamTypeComboItem p = new PCLExamTypeComboItem();
                        p.PCLExamType = item;
                        ObjPCLExamTypeCombo_Current.PCLExamTypeComboItems.Add(p);
                        CalcDataXML(eHCMSResources.T1779_G1_I, p);
                    }
                }
            }
        }

        private void PCLExamTypeCombo_Save()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Ghi..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLExamTypeCombo_Save(ObjPCLExamTypeCombo_Current, DataPCLExamTypeComboItem_Insert, DataPCLExamTypeComboItem_Update, DataPCLExamTypeComboItem_Delete,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                long ID = 0;
                                var bOK = contract.EndPCLExamTypeCombo_Save(out ID, asyncResult);
                                if (bOK)
                                {
                                    Globals.EventAggregator.Publish(new PCLExamTypeComboEvent_AddEditSave() { Result_AddEditSave = true });
                                    MessageBox.Show(eHCMSResources.Z0655_G1_DaGhi, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    TryClose();
                                    //phat su kien de load lai du lieu

                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0549_G1_Msg_InfoGhiDataFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                //Globals.IsBusy = false;
                                IsLoading = false;
                            }
                        }), null);
                }


            });
            t.Start();
        }

    }
}
