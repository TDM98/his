using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts.ResourcesManage.Maintenance;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Common;
using eHCMSLanguage;
using aEMR.ViewContracts;
using Castle.Windsor;

namespace aEMR.ResourceMaintenance.Maintenance.ViewModels
{
    [Export(typeof (IResourceMaintenanceLog)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ResourceMaintenanceLogViewModel : Conductor<object>, IResourceMaintenanceLog
                                                   ,
                                                   IHandle
                                                       <ResourceMaintenanceLogStatus_AddViewModel_HoanTatBaoTri_Event>
                                                   , IHandle<ResourceMaintenanceLog_ConfirmViewModel_Confirm_Event>
    {
        [ImportingConstructor]
        public ResourceMaintenanceLogViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            authorization();
            Globals.EventAggregator.Subscribe(this);

            Criteria = new ResourceMaintenanceLogSearchCriteria();
            Criteria.V_StatusIssueMaintenance = -1;
            Criteria.LoggingIssue = "";
            Criteria.FromDate = DateTime.Now.AddDays(-1);
            Criteria.ToDate = DateTime.Now;

            ObjV_StatusIssueMaintenanceViewModel = new ObservableCollection<Lookup>();
            Load_V_StatusIssueMaintenance();

            ObjGetResourceMaintenanceLogSearch_Paging =
                new PagedSortableCollectionView<DataEntities.ResourceMaintenanceLog>();
            ObjGetResourceMaintenanceLogSearch_Paging.OnRefresh +=
                new EventHandler<RefreshEventArgs>(ObjGetResourceMaintenanceLogSearch_Paging_OnRefresh);
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            authorization();
        }
        private void ObjGetResourceMaintenanceLogSearch_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetResourceMaintenanceLogSearch_Paging(ObjGetResourceMaintenanceLogSearch_Paging.PageIndex,
                                                   ObjGetResourceMaintenanceLogSearch_Paging.PageSize, false);
        }

        private ObservableCollection<Lookup> _ObjV_StatusIssueMaintenanceViewModel;

        public ObservableCollection<Lookup> ObjV_StatusIssueMaintenanceViewModel
        {
            get { return _ObjV_StatusIssueMaintenanceViewModel; }
            set
            {
                _ObjV_StatusIssueMaintenanceViewModel = value;
                NotifyOfPropertyChange(() => ObjV_StatusIssueMaintenanceViewModel);
            }
        }

        private ResourceMaintenanceLogSearchCriteria _Criteria;

        public ResourceMaintenanceLogSearchCriteria Criteria
        {
            get { return _Criteria; }
            set
            {
                _Criteria = value;
                NotifyOfPropertyChange(() => Criteria);
            }
        }

        public void Load_V_StatusIssueMaintenance()
        {
            var t = new Thread(() =>
                                   {
                                       Globals.EventAggregator.Publish(new BusyEvent
                                                                           {
                                                                               IsBusy = true,
                                                                               Message =
                                                                                   "Danh Sách Tình Trạng Vấn Đề..."
                                                                           });
                                       try
                                       {
                                           using (var serviceFactory = new CommonService_V2Client())
                                           {
                                               var contract = serviceFactory.ServiceInstance;

                                               contract.BeginGetAllLookupValuesByType(
                                                   LookupValues.V_StatusIssueMaintenance,
                                                   Globals.DispatchCallback((asyncResult) =>
                                                                                {
                                                                                    IList<Lookup> allItems =
                                                                                        new ObservableCollection<Lookup>
                                                                                            ();
                                                                                    try
                                                                                    {
                                                                                        allItems =
                                                                                            contract.
                                                                                                EndGetAllLookupValuesByType
                                                                                                (asyncResult);

                                                                                        ObjV_StatusIssueMaintenanceViewModel
                                                                                            =
                                                                                            new ObservableCollection
                                                                                                <Lookup>(allItems);
                                                                                        Lookup firstItem = new Lookup();
                                                                                        firstItem.LookupID = -1;
                                                                                        firstItem.ObjectValue =
                                                                                            "-- Tất Cả --";
                                                                                        ObjV_StatusIssueMaintenanceViewModel
                                                                                            .Insert(0, firstItem);
                                                                                    }
                                                                                    catch (FaultException<AxException> fault)
                                                                                    {
                                                                                        ClientLoggerHelper.LogInfo(fault.ToString());
                                                                                    }
                                                                                    catch (Exception ex)
                                                                                    {
                                                                                        ClientLoggerHelper.LogInfo(ex.ToString());
                                                                                    }


                                                                                }), null);
                                           }
                                       }
                                       catch (Exception ex)
                                       {
                                           Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                       }
                                       finally
                                       {
                                           Globals.IsBusy = false;
                                       }
                                   });
            t.Start();
        }

        private PagedSortableCollectionView<DataEntities.ResourceMaintenanceLog>
            _ObjGetResourceMaintenanceLogSearch_Paging;

        public PagedSortableCollectionView<DataEntities.ResourceMaintenanceLog>
            ObjGetResourceMaintenanceLogSearch_Paging
        {
            get { return _ObjGetResourceMaintenanceLogSearch_Paging; }
            set
            {
                _ObjGetResourceMaintenanceLogSearch_Paging = value;
                NotifyOfPropertyChange(() => ObjGetResourceMaintenanceLogSearch_Paging);
            }
        }

        private void GetResourceMaintenanceLogSearch_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            Globals.EventAggregator.Publish(new BusyEvent() {IsBusy = true, Message = "Danh Sách Vật Tư Bảo Trì..."});

            var t = new Thread(() =>
                                   {
                                       try
                                       {
                                           using (var serviceFactory = new ResourcesManagementServiceClient())
                                           {
                                               var client = serviceFactory.ServiceInstance;
                                               client.BeginGetResourceMaintenanceLogSearch_Paging(Criteria, PageIndex,PageSize, "",CountTotal,Globals.DispatchCallback((asyncResult)
                                                        =>{
                                                                int Total=0;
                                                                IList<DataEntities.ResourceMaintenanceLog>allItems=null;
                                                                bool bOK=false;
                                                                try
                                                                {
                                                                    allItems=client.EndGetResourceMaintenanceLogSearch_Paging(out Total,asyncResult);
                                                                    bOK=true;
                                                                }
                                                                catch(FaultException<AxException> fault)
                                                                {
                                                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                                                }
                                                                catch(Exception ex)
                                                                {
                                                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                                                }

                                                                ObjGetResourceMaintenanceLogSearch_Paging.Clear();

                                                                if (bOK)
                                                                {
                                                                    if(CountTotal)
                                                                    {
                                                                        ObjGetResourceMaintenanceLogSearch_Paging.TotalItemCount=Total;
                                                                    }
                                                                    if(allItems !=null)
                                                                    {
                                                                        foreach(var item in allItems)
                                                                        {
                                                                            ObjGetResourceMaintenanceLogSearch_Paging.Add(item);
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
                                           Globals.IsBusy = false;
                                       }
                                   });
            t.Start();
        }

        public void cboV_StatusIssueMaintenance_SelectedItemChanged(object selectedItem)
        {
            if (selectedItem != null)
            {
                Criteria.V_StatusIssueMaintenance = (selectedItem as Lookup).LookupID;
                ObjGetResourceMaintenanceLogSearch_Paging.PageIndex = 0;
                GetResourceMaintenanceLogSearch_Paging(0, ObjGetResourceMaintenanceLogSearch_Paging.PageSize, true);
            }
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bbtnSearch = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mResources_Maintenance
                                                      , (int)eResources_Maintenance.mListRequest
                                                      , (int)oResources_MaintenanceEx.mList
                                                      , (int)ePermission.mView);
            bhplEditConfirm = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mResources_Maintenance
                                                      , (int)eResources_Maintenance.mListRequest
                                                      , (int)oResources_MaintenanceEx.mState
                                                      , (int)ePermission.mEdit);
            bhplListFix = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mResources_Maintenance
                                                      , (int)eResources_Maintenance.mListRequest
                                                      , (int)oResources_MaintenanceEx.mState
                                                      , (int)ePermission.mAdd);
            bhplDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mResources_Maintenance
                                                      , (int)eResources_Maintenance.mListRequest
                                                      , (int)oResources_MaintenanceEx.mList
                                                      , (int)ePermission.mDelete);
        }

#region checking account

        private bool _bbtnSearch = true;
        private bool _bhplEditConfirm = true;
        private bool _bhplListFix = true;
        private bool _bhplDelete = true;
        
        public bool bbtnSearch
        {
            get
            {
                return _bbtnSearch;
            }
            set
            {
                if (_bbtnSearch == value)
                    return;
                _bbtnSearch = value;
                NotifyOfPropertyChange(() => bbtnSearch);
            }
        }
        public bool bhplEditConfirm
        {
            get
            {
                return _bhplEditConfirm;
            }
            set
            {
                if (_bhplEditConfirm == value)
                    return;
                _bhplEditConfirm = value;
                NotifyOfPropertyChange(() => bhplEditConfirm);
            }
        }
        public bool bhplListFix
        {
            get
            {
                return _bhplListFix;
            }
            set
            {
                if (_bhplListFix == value)
                    return;
                _bhplListFix = value;
                NotifyOfPropertyChange(() => bhplListFix);
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
                NotifyOfPropertyChange(() => bhplDelete);
            }
        }
#endregion


        public void btnSearch()
        {
            if (Criteria.FromDate == null)
            {
                MessageBox.Show(eHCMSResources.A0883_G1_Msg_InfoNhapTuNg);
            }
            else if(Criteria.ToDate==null)
            {
                MessageBox.Show(eHCMSResources.A0867_G1_Msg_InfoNhapDenNg);
            }
            else
            {
                if(Criteria.FromDate<=Criteria.ToDate)
                {
                    ObjGetResourceMaintenanceLogSearch_Paging.PageIndex = 0;
                    GetResourceMaintenanceLogSearch_Paging(0,ObjGetResourceMaintenanceLogSearch_Paging.PageSize, true);
                }
                else
                {
                    MessageBox.Show(eHCMSResources.Z0467_G1_TuNgNhoHonBangDenNg);
                }
            }
            
        }

        public void hplDelete_Click(object selectItem)
        {
            if(selectItem!=null)
            {
                ResourceMaintenanceLog objDelete = selectItem as ResourceMaintenanceLog;

                if (objDelete.ObjV_CurrentStatus.LookupID == 9000)
                {
                    if (MessageBox.Show(eHCMSResources.K0483_G1_BanCoChacChanMuonXoa, "Xóa 1 Yêu Cầu Bảo Trì", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        ResourceMaintenanceLog_Delete(objDelete.RscrMaintLogID);
                    }
                }
                else
                {
                    if (objDelete.ObjV_CurrentStatus.LookupID == 9999)
                        MessageBox.Show("Thiết Bị Đã Bảo Trì Hoàn Tất! Không Thể Xóa!", eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);
                    else
                        MessageBox.Show("Thiết Bị Đang Bảo Trì! Không Thể Xóa!", eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);
                }
            }
        }


        private void ResourceMaintenanceLog_Delete(Int64 RmTypeID)
        {

            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0492_G1_DangXoa) });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginResourceMaintenanceLog_Delete(RmTypeID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            if(contract.EndResourceMaintenanceLog_Delete(asyncResult))
                            {
                                ObjGetResourceMaintenanceLogSearch_Paging.PageIndex = 0;
                                GetResourceMaintenanceLogSearch_Paging(0,ObjGetResourceMaintenanceLogSearch_Paging.PageSize, true);
                                MessageBox.Show(eHCMSResources.A0478_G1_Msg_InfoXoaOK, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);        
                                
                            }
                            else
                            {
                                MessageBox.Show("Xóa Không Thành Công!", eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);        
                            }
                            
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

        public void hplListFix_Click(object selectItem)
        {
            if (selectItem != null)
            {
                ResourceMaintenanceLog ObjResourceMaintenanceLog = selectItem as ResourceMaintenanceLog;
                Action<IResourceMaintenanceLogStatus_Add> onInitDlg = delegate (IResourceMaintenanceLogStatus_Add typeInfo)
                {
                    typeInfo.ObjResourceMaintenanceLog_Current = ObjResourceMaintenanceLog;

                    typeInfo.V_CurrentStatus_Seleted = -1;

                    typeInfo.LoadListHistoryStatus();
                };
                GlobalsNAV.ShowDialog<IResourceMaintenanceLogStatus_Add>(onInitDlg);
            }
        }



        public void Handle(ResourceMaintenanceLogStatus_AddViewModel_HoanTatBaoTri_Event message)
        {
            if(message!=null)
            {
                ObjGetResourceMaintenanceLogSearch_Paging.PageIndex = 0;
                GetResourceMaintenanceLogSearch_Paging(0,ObjGetResourceMaintenanceLogSearch_Paging.PageSize, true);
            }
        }

        public void hplEditConfirm()
        {

        }

        public void hplEditConfirm_Click(object selectItem)
        {
            if (Globals.isAccountCheck)
            {
                if (!Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mResources_Maintenance
                                                 , (int)eResources_Maintenance.mListRequest
                                                 , (int)oResources_MaintenanceEx.mState
                                                 , (int)ePermission.mEdit))
                {
                    Globals.ShowMessage(eHCMSResources.Z1323_G1_ChuaPhanQuyenChSuaBaoTri, "");
                    return;
                }
            }


            if (selectItem != null)
            {
                ResourceMaintenanceLog ObjResourceMaintenanceLog = selectItem as ResourceMaintenanceLog;
                Action<IResourceMaintenanceLog_Confirm> onInitDlg = delegate (IResourceMaintenanceLog_Confirm typeInfo)
                {
                    typeInfo.ObjResourceMaintenanceLog_Current = ObjectCopier.DeepCopy(ObjResourceMaintenanceLog);

                    typeInfo.LoadForm(ObjResourceMaintenanceLog);
                };
                GlobalsNAV.ShowDialog<IResourceMaintenanceLog_Confirm>(onInitDlg);
            }
        }

        public void Handle(ResourceMaintenanceLog_ConfirmViewModel_Confirm_Event message)
        {
            if (message != null)
            {
                ObjGetResourceMaintenanceLogSearch_Paging.PageIndex = 0;
                GetResourceMaintenanceLogSearch_Paging(0, ObjGetResourceMaintenanceLogSearch_Paging.PageSize, true);
            }
        }
    }
}
