using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Net;
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

namespace aEMR.Configuration.RefMedicalServiceItems_IsPCL.ViewModels
{
     [Export(typeof(IRefMedicalServiceItems_IsPCL))]
    public class RefMedicalServiceItems_IsPCLViewModel : Conductor<object>, IRefMedicalServiceItems_IsPCL
        , IHandle<RefMedicalServiceItems_IsPCL_AddEditViewModel_Save_Event>

     {
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
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public RefMedicalServiceItems_IsPCLViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            //Globals.EventAggregator.Subscribe(this);
            authorization();

            SearchCriteria = new RefMedicalServiceItemsSearchCriteria();

            ObjRefMedicalServiceItems_IsPCLByMedServiceID_Paging=new PagedSortableCollectionView<RefMedicalServiceItem>();
            ObjRefMedicalServiceItems_IsPCLByMedServiceID_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjRefMedicalServiceItems_IsPCLByMedServiceID_Paging_OnRefresh);

            ObjRefMedicalServiceItems_IsPCLByMedServiceID_Paging.PageIndex = 0;
            RefMedicalServiceItems_IsPCLByMedServiceID_Paging(0, ObjRefMedicalServiceItems_IsPCLByMedServiceID_Paging.PageSize, true);

        }

        void ObjRefMedicalServiceItems_IsPCLByMedServiceID_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            RefMedicalServiceItems_IsPCLByMedServiceID_Paging(ObjRefMedicalServiceItems_IsPCLByMedServiceID_Paging.PageIndex, ObjRefMedicalServiceItems_IsPCLByMedServiceID_Paging.PageSize, false);
        }

        private RefMedicalServiceItemsSearchCriteria _SearchCriteria;
        public RefMedicalServiceItemsSearchCriteria SearchCriteria
        {
            get
            {
                return _SearchCriteria;
            }
            set
            {
                _SearchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }

        private PagedSortableCollectionView<RefMedicalServiceItem> _ObjRefMedicalServiceItems_IsPCLByMedServiceID_Paging;
        public PagedSortableCollectionView<RefMedicalServiceItem> ObjRefMedicalServiceItems_IsPCLByMedServiceID_Paging
        {
            get
            {
                return _ObjRefMedicalServiceItems_IsPCLByMedServiceID_Paging;
            }
            set
            {
                _ObjRefMedicalServiceItems_IsPCLByMedServiceID_Paging = value;
                NotifyOfPropertyChange(()=>ObjRefMedicalServiceItems_IsPCLByMedServiceID_Paging);
            }
        }

        private void RefMedicalServiceItems_IsPCLByMedServiceID_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách Dịch Vụ PCL..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginRefMedicalServiceItems_IsPCLByMedServiceID_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DataEntities.RefMedicalServiceItem> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndRefMedicalServiceItems_IsPCLByMedServiceID_Paging(out Total, asyncResult);
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

                            ObjRefMedicalServiceItems_IsPCLByMedServiceID_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjRefMedicalServiceItems_IsPCLByMedServiceID_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjRefMedicalServiceItems_IsPCLByMedServiceID_Paging.Add(item);
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

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bhplEditService = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mQLGoiDichVuCLSCuaKhoa,
                                               (int)oConfigurationEx.mQuanLyGoiDV_CLSCuaKhoa, (int)ePermission.mEdit);
            bhplDeleteService=Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mQLGoiDichVuCLSCuaKhoa,
                                               (int)oConfigurationEx.mQuanLyGoiDV_CLSCuaKhoa, (int)ePermission.mDelete);
            bhplAddNew=Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                            , (int)eConfiguration_Management.mQLGoiDichVuCLSCuaKhoa,
                                            (int)oConfigurationEx.mQuanLyGoiDV_CLSCuaKhoa, (int)ePermission.mAdd);
            bbtSearch = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                            , (int)eConfiguration_Management.mQLGoiDichVuCLSCuaKhoa,
                                            (int)oConfigurationEx.mQuanLyGoiDV_CLSCuaKhoa, (int)ePermission.mView);

        }
        #region checking account

        private bool _bhplEditService = true;
         private bool _bhplDeleteService = true;
         private bool _bhplAddNew = true;
         private bool _bbtSearch = true;
         public bool bhplEditService
         {
             get
             {
                 return _bhplEditService;
             }
             set
             {
                 if (_bhplEditService == value)
                     return;
                 _bhplEditService = value;
             }
         }
         public bool bhplDeleteService
         {
             get
             {
                 return _bhplDeleteService;
             }
             set
             {
                 if (_bhplDeleteService == value)
                     return;
                 _bhplDeleteService = value;
             }
         }
         public bool bhplAddNew
         {
             get
             {
                 return _bhplAddNew;
             }
             set
             {
                 if (_bhplAddNew == value)
                     return;
                 _bhplAddNew = value;
             }
         }
         public bool bbtSearch
         {
             get
             {
                 return _bbtSearch;
             }
             set
             {
                 if (_bbtSearch == value)
                     return;
                 _bbtSearch = value;
             }
         }
        #endregion
        #region binding visibilty

        public Button hplEditService { get; set; }
        public Button hplDeleteService { get; set; }
         

        public void hplEditService_Loaded(object sender)
        {
            hplEditService = sender as Button;
            hplEditService.Visibility = Globals.convertVisibility(bhplEditService);
        }
        public void hplDeleteService_Loaded(object sender)
        {
            hplDeleteService = sender as Button;
            hplDeleteService.Visibility = Globals.convertVisibility(bhplDeleteService);
        }

        #endregion

        public void btSearch()
        {
            ObjRefMedicalServiceItems_IsPCLByMedServiceID_Paging.PageIndex = 0;
            RefMedicalServiceItems_IsPCLByMedServiceID_Paging(0,ObjRefMedicalServiceItems_IsPCLByMedServiceID_Paging.PageSize,true);
        }

        public void hplAddNewRefMedicalServiceItemsView()
        {
            //var typeInfo = Globals.GetViewModel<IRefMedicalServiceItems_IsPCL_AddEdit>();
            //typeInfo.TitleForm = " Thêm Gói Dịch Vụ Cận Lâm Sàng: ";
            //typeInfo.InitializeNewItem();

            //var instance = typeInfo as Conductor<object>;

            //Globals.ShowDialog(instance, (o) =>
            //{
            //    //làm gì đó
            //});

            Action<IRefMedicalServiceItems_IsPCL_AddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = " Thêm Gói Dịch Vụ Cận Lâm Sàng: ";
                typeInfo.InitializeNewItem();
            };
            GlobalsNAV.ShowDialog<IRefMedicalServiceItems_IsPCL_AddEdit>(onInitDlg);

        }

        #region "Nút trong dataGrid"
        public void hplEditService_Click(object datacontext)
        {
            DataEntities.RefMedicalServiceItem p =ObjectCopier.DeepCopy(datacontext as DataEntities.RefMedicalServiceItem);

            //var typeInfo = Globals.GetViewModel<IRefMedicalServiceItems_IsPCL_AddEdit>();
            //typeInfo.TitleForm = "Hiệu Chỉnh Gói Dịch Vụ PCL: " + p.MedServiceName.Trim();
            //typeInfo.ObjRefMedicalServiceItem_Current = ObjectCopier.DeepCopy(p);

            //var instance = typeInfo as Conductor<object>;

            //Globals.ShowDialog(instance, (o) =>
            //{
            //    //làm gì đó
            //});

            Action<IRefMedicalServiceItems_IsPCL_AddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = "Hiệu Chỉnh Gói Dịch Vụ PCL: " + p.MedServiceName.Trim();
                typeInfo.ObjRefMedicalServiceItem_Current = ObjectCopier.DeepCopy(p);
            };
            GlobalsNAV.ShowDialog<IRefMedicalServiceItems_IsPCL_AddEdit>(onInitDlg);
        }

        public void hplDeleteService_Click(object datacontext)
        {
            DataEntities.RefMedicalServiceItem p = datacontext as DataEntities.RefMedicalServiceItem;

            if (p != null)
            {
                if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, p.MedServiceName.Trim()), eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    RefMedicalServiceItems_MarkDeleted(p.MedServiceID);
                }
            }
        }
        #endregion

        public void RefMedicalServiceItems_MarkDeleted(Int64 MedServiceID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Xóa..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginRefMedicalServiceItems_MarkDeleted(MedServiceID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Result = "";
                            contract.EndRefMedicalServiceItems_MarkDeleted(out Result, asyncResult);
                            switch (Result)
                            {
                                case "Delete-0":
                                    {
                                        MessageBox.Show(eHCMSResources.K0484_G1_XoaFail, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);
                                        break;
                                    }  
                                case "Delete-1":
                                    {
                                        ObjRefMedicalServiceItems_IsPCLByMedServiceID_Paging.PageIndex = 0;
                                        RefMedicalServiceItems_IsPCLByMedServiceID_Paging(0, ObjRefMedicalServiceItems_IsPCLByMedServiceID_Paging.PageSize, true);
                                        MessageBox.Show(eHCMSResources.A0478_G1_Msg_InfoXoaOK, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);
                                        break;
                                    }
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

        public void hplPCLExamTypeMedServiceDefItems_Click(object datacontext)
        {
            DataEntities.RefMedicalServiceItem p = datacontext as DataEntities.RefMedicalServiceItem;
            //var typeInfo = Globals.GetViewModel<IPCLExamTypeMedServiceDefItems>();
            //typeInfo.ObjRefMedicalServiceItem_isPCL = p;
            //typeInfo.FormLoad();

            //var instance = typeInfo as Conductor<object>;

            //Globals.ShowDialog(instance, (o) =>
            //{
            //    //làm gì đó
            //});

            Action<IPCLExamTypeMedServiceDefItems> onInitDlg = (typeInfo) =>
            {
                typeInfo.ObjRefMedicalServiceItem_isPCL = p;
                typeInfo.FormLoad();
            };
            GlobalsNAV.ShowDialog<IPCLExamTypeMedServiceDefItems>(onInitDlg);

        }

        public void Handle(RefMedicalServiceItems_IsPCL_AddEditViewModel_Save_Event message)
        {
            if(message!=null)
            {
                ObjRefMedicalServiceItems_IsPCLByMedServiceID_Paging.PageIndex = 0;
                RefMedicalServiceItems_IsPCLByMedServiceID_Paging(0, ObjRefMedicalServiceItems_IsPCLByMedServiceID_Paging.PageSize, true);
            }
        }
     }
}
