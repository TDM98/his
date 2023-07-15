using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using System.Collections.Generic;
using System.ServiceModel;
using System.Linq;
using aEMR.ViewContracts;
using Castle.Windsor;
using System.Windows.Input;
using aEMR.Common.ExportExcel;
/*
* 20230601 #001 DatTB: IssueID: 3254 | Chỉnh sửa/Gộp các function xuất excel danh mục/cấu hình (Bỏ Func cũ)
*/
namespace aEMR.Configuration.RefMedicalServiceItems.ViewModels
{
    [Export(typeof(IRefMedicalServiceItems)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RefMedicalServiceItemsViewModel : Conductor<object>, IRefMedicalServiceItems
        , IHandle<RefMedicalServiceItems_NotPCL_Add_Event>
        , IHandle<MedServiceItemPrice_AddEditViewModel_Save_Event>
        , IHandle<RefMedicalServiceItem_AddEditEvent_Save>
        , IHandle<SaveEvent<bool>>
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


        private object _leftContent;
        public object leftContent
        {
            get
            {
                return _leftContent;
            }
            set
            {
                if (_leftContent == value)
                    return;
                _leftContent = value;
                NotifyOfPropertyChange(() => leftContent);
            }
        }
        [ImportingConstructor]
        public RefMedicalServiceItemsViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            authorization();
            eventArg.Subscribe(this);

            ObjRefMedicalServiceTypeSelected = new RefMedicalServiceType();
            ObjRefMedicalServiceTypeSelected.MedicalServiceTypeID = -1;

            SearchCriteria = new RefMedicalServiceItemsSearchCriteria();
            SearchCriteria.MedicalServiceTypeID = -1;

            ObjRefMedicalServiceTypes_GetAll = new ObservableCollection<RefMedicalServiceType>();

            ObjMedServiceItems_Paging = new PagedSortableCollectionView<RefMedicalServiceItem>();
            ObjMedServiceItems_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjMedServiceItems_Paging_OnRefresh);

            GetAllMedicalServiceTypes_SubtractPCL();
        }

        void ObjMedServiceItems_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetMedServiceItems_Paging(ObjMedServiceItems_Paging.PageIndex, ObjMedServiceItems_Paging.PageSize, false);
        }

        private ObservableCollection<RefMedicalServiceType> _ObjRefMedicalServiceTypes_GetAll;
        public ObservableCollection<RefMedicalServiceType> ObjRefMedicalServiceTypes_GetAll
        {
            get { return _ObjRefMedicalServiceTypes_GetAll; }
            set
            {
                _ObjRefMedicalServiceTypes_GetAll = value;
                NotifyOfPropertyChange(() => ObjRefMedicalServiceTypes_GetAll);
            }
        }
        public void RefMedicalServiceItems_GroupMedicalServiceTypeIDByDeptID(Int64 DeptID, int V)
        {
            ObjRefMedicalServiceTypes_GetAll.Clear();

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách Loại Dịch Vụ..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginRefMedicalServiceItems_GroupMedicalServiceTypeIDByDeptID(DeptID, V, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndRefMedicalServiceItems_GroupMedicalServiceTypeIDByDeptID(asyncResult);

                            if (items != null)
                            {
                                ObjRefMedicalServiceTypes_GetAll = new ObservableCollection<DataEntities.RefMedicalServiceType>(items);

                                //Item Default
                                RefMedicalServiceType ItemDefault = new RefMedicalServiceType();
                                ItemDefault.MedicalServiceTypeID = -1;
                                ItemDefault.MedicalServiceTypeName = "--Chọn Loại Dịch Vụ--";
                                //Item Default
                                ObjRefMedicalServiceTypes_GetAll.Insert(0, ItemDefault);

                                if (ObjRefMedicalServiceTypes_GetAll.Count > 1)
                                {
                                    //Tất Cả Để Tìm Cho Tiện
                                    RefMedicalServiceType ItemAll = new RefMedicalServiceType();
                                    ItemAll.MedicalServiceTypeID = 0;
                                    ItemAll.MedicalServiceTypeName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                    ObjRefMedicalServiceTypes_GetAll.Insert(1, ItemAll);
                                    //Tất Cả Để Tìm Cho Tiện
                                }
                            }
                            else
                            {
                                ObjRefMedicalServiceTypes_GetAll = null;
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

        private PagedSortableCollectionView<RefMedicalServiceItem> _ObjMedServiceItems_Paging;
        public PagedSortableCollectionView<RefMedicalServiceItem> ObjMedServiceItems_Paging
        {
            get { return _ObjMedServiceItems_Paging; }
            set
            {
                _ObjMedServiceItems_Paging = value;
                NotifyOfPropertyChange(() => ObjMedServiceItems_Paging);
            }
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bhplEditService = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mQLDichVu_DonGiaCuaKhoa,
                                               (int)oConfigurationEx.mQuanLyDichVuDonGia, (int)ePermission.mEdit);
            bhplDeleteService = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mQLDichVu_DonGiaCuaKhoa,
                                               (int)oConfigurationEx.mQuanLyDichVuDonGia, (int)ePermission.mDelete);
            bhplListPrice = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mQLDichVu_DonGiaCuaKhoa,
                                               (int)oConfigurationEx.mQuanLyDichVuDonGia, (int)ePermission.mView);
            bhplAddNewRefMedicalServiceItemsView = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mQLDichVu_DonGiaCuaKhoa,
                                               (int)oConfigurationEx.mQuanLyDichVuDonGia, (int)ePermission.mAdd);
        }
        #region checking account

        private bool _bhplEditService = true;
        private bool _bhplDeleteService = true;
        private bool _bhplListPrice = true;
        private bool _bhplAddNewRefMedicalServiceItemsView = true;
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
        public bool bhplListPrice
        {
            get
            {
                return _bhplListPrice;
            }
            set
            {
                if (_bhplListPrice == value)
                    return;
                _bhplListPrice = value;
            }
        }
        public bool bhplAddNewRefMedicalServiceItemsView
        {
            get
            {
                return _bhplAddNewRefMedicalServiceItemsView;
            }
            set
            {
                if (_bhplAddNewRefMedicalServiceItemsView == value)
                    return;
                _bhplAddNewRefMedicalServiceItemsView = value;
            }
        }
        #endregion
        #region binding visibilty

        public Button hplEditService { get; set; }
        public Button hplDeleteService { get; set; }
        public Button hplListPrice { get; set; }

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
        public void hplListPrice_Loaded(object sender)
        {
            hplListPrice = sender as Button;
            hplListPrice.Visibility = Globals.convertVisibility(bhplListPrice);
        }
        #endregion

        public void btSearch()
        {
            //if (ObjTreeNodeRefDepartments_Current.NodeID > 0)
            //{
            //    if (ObjRefMedicalServiceTypeSelected.MedicalServiceTypeID > 0)
            //    {
            //        SearchCriteria.MedicalServiceTypeID = ObjRefMedicalServiceTypeSelected.MedicalServiceTypeID;
            //        ObjMedServiceItems_Paging.PageIndex = 0;
            //        GetMedServiceItems_Paging(0, ObjMedServiceItems_Paging.PageSize, true);
            //    }
            //    else
            //    {
            //        MessageBox.Show(eHCMSResources.A0327_G1_Msg_InfoChonLoaiDV, eHCMSResources.G1174_G1_TimKiem, MessageBoxButton.OK);
            //    }
            //}
            SearchCriteria.MedicalServiceTypeID = ObjRefMedicalServiceTypeSelected.MedicalServiceTypeID;
            ObjMedServiceItems_Paging.PageIndex = 0;
            GetMedServiceItems_Paging(0, ObjMedServiceItems_Paging.PageSize, true);
        }
        public void btAddNew()
        {
            Action<IDeptMedServiceItems_EditInfo> onInitDlg = delegate (IDeptMedServiceItems_EditInfo typeInfo)
            {
                typeInfo.TitleForm = eHCMSResources.G0298_G1_ThemMoiDV;
                typeInfo.isUpdate = false;
                typeInfo.ObjRefMedicalServiceTypeSelected = ObjRefMedicalServiceTypeSelected;
                typeInfo.ObjDeptMedServiceItems_Save = new DataEntities.DeptMedServiceItems();
            };
            GlobalsNAV.ShowDialog<IDeptMedServiceItems_EditInfo>(onInitDlg);
        }

        public void GetAllMedicalServiceTypes_SubtractPCL()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách Loại Dịch Vụ..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetAllMedicalServiceTypes_SubtractPCL(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndGetAllMedicalServiceTypes_SubtractPCL(asyncResult);

                            if (items != null)
                            {
                                ObjRefMedicalServiceTypes_GetAll = new ObservableCollection<DataEntities.RefMedicalServiceType>(items);

                                //Item Default
                                RefMedicalServiceType ItemDefault = new RefMedicalServiceType();
                                ItemDefault.MedicalServiceTypeID = -1;
                                ItemDefault.MedicalServiceTypeName = "--Chọn Loại Dịch Vụ--";
                                //Item Default

                                ObjRefMedicalServiceTypes_GetAll.Insert(0, ItemDefault);
                            }
                            else
                            {
                                ObjRefMedicalServiceTypes_GetAll = null;
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
       
        private void GetMedServiceItems_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách Dịch Vụ..." });
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                IsLoading = true;

                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetMedServiceItems_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<RefMedicalServiceItem> allItems = null;
                            bool bOK = false;
                            try
                            {
                                
                                allItems = client.EndGetMedServiceItems_Paging(out Total, asyncResult);
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

                            ObjMedServiceItems_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjMedServiceItems_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjMedServiceItems_Paging.Add(item);
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
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        

        #region Nút trong dataGrid


        public void hplEditService_Click(object datacontext)
        {
            RefMedicalServiceItem p = datacontext as RefMedicalServiceItem;

            //Chỉ cho phép sửa Tên thôi
            Action<IDeptMedServiceItems_EditInfo> onInitDlg = delegate (IDeptMedServiceItems_EditInfo typeInfo)
            {
                typeInfo.TitleForm = eHCMSResources.Z1586_G1_HChinhTTinDV;
                typeInfo.isUpdate = true;
                typeInfo.ObjRefMedicalServiceItem = p;
                typeInfo.tmpRefMedicalServiceItem = p.DeepCopy();
                var ID = Convert.ToInt32(p.MedicalServiceTypeID);
                typeInfo.ObjRefMedicalServiceTypeSelected = ObjRefMedicalServiceTypes_GetAll.Where(x => x.MedicalServiceTypeID == ID).FirstOrDefault();
            };
            GlobalsNAV.ShowDialog<IDeptMedServiceItems_EditInfo>(onInitDlg);

        }
        public void hplDeleteService_Click(object datacontext)
        {
            RefMedicalServiceItem p = datacontext as RefMedicalServiceItem;

            if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, p.MedServiceName), eHCMSResources.G2773_G1_XoaDV, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                p.IsDeleted = true;
                p.IsActive = false;
                MedServiceItems_Update(p);
            }
            
        }

        public void MedServiceItems_Update(RefMedicalServiceItem obj)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Xóa..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginRefMedicalServiceItems_NotPCL_Update(obj, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            if (contract.EndRefMedicalServiceItems_NotPCL_Update(asyncResult))
                            {
                                ObjMedServiceItems_Paging.PageIndex = 0;
                                GetMedServiceItems_Paging(0, ObjMedServiceItems_Paging.PageSize, true);
                                MessageBox.Show(eHCMSResources.A0478_G1_Msg_InfoXoaOK, eHCMSResources.G2773_G1_XoaDV, MessageBoxButton.OK);
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.K0484_G1_XoaFail, eHCMSResources.G2773_G1_XoaDV, MessageBoxButton.OK);
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

        public void DeptMedServiceItems_TrueDelete(Int64 DeptMedServItemID, Int64 MedServItemPriceID, Int64 MedServiceID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Xóa..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginDeptMedServiceItems_TrueDelete(DeptMedServItemID, MedServItemPriceID, MedServiceID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            if (contract.EndDeptMedServiceItems_TrueDelete(asyncResult))
                            {
                                ObjMedServiceItems_Paging.PageIndex = 0;
                                MessageBox.Show(eHCMSResources.A0478_G1_Msg_InfoXoaOK, eHCMSResources.G2773_G1_XoaDV, MessageBoxButton.OK);
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.K0484_G1_XoaFail, eHCMSResources.G2773_G1_XoaDV, MessageBoxButton.OK);
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
                                        ObjMedServiceItems_Paging.PageIndex = 0;
                                        
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

        private RefMedicalServiceItem SetValueObjPrice(RefMedicalServiceItem p)
        {
            RefMedicalServiceItem ObjPrice = new RefMedicalServiceItem();
            //ObjPrice.MedServItemPriceID = p.MedServItemPriceID;
            //ObjPrice.MedServiceID = p.MedServiceID;
            //ObjPrice.StaffID = p.StaffID;
            //ObjPrice.ApprovedStaffID = p.ApprovedStaffID;
            //ObjPrice.VATRate = p.VATRate;
            //ObjPrice.NormalPrice = p.NormalPrice;
            //ObjPrice.PriceForHIPatient = p.PriceForHIPatient;
            //ObjPrice.PriceDifference = p.PriceDifference;
            //ObjPrice.HIAllowedPrice = p.HIAllowedPrice;
            //ObjPrice.EffectiveDate = p.EffectiveDate;
            //ObjPrice.IsActive = p.IsActive;
            //ObjPrice.IsDeleted = p.IsDeleted;
            //ObjPrice.Note = p.Note;
            return ObjPrice;
        }

        #endregion


        public void Handle(RefMedicalServiceItems_NotPCL_Add_Event message)
        {
            if (message != null)
            {
                RefMedicalServiceItems_GroupMedicalServiceTypeIDByDeptID(SearchCriteria.DeptID, 1);//Subtract loại PCL
                ReLoadAfterSave();
            }
        }

        public void Handle(MedServiceItemPrice_AddEditViewModel_Save_Event message)
        {
            if (message != null)
            {
                ReLoadAfterSave();
            }
        }

        public void Handle(RefMedicalServiceItem_AddEditEvent_Save message)
        {
            if (message != null)
            {
                ReLoadAfterSave();
            }
        }

        //public void cboMedicalServiceTypesSubTractPCL_SelectionChanged(object selectItem)
        //{
        //    if(selectItem!=null)
        //    {
        //        if (ObjTreeNodeRefDepartments_Current.NodeID > 0)
        //        {
        //            if (SearchCriteria.MedicalServiceTypeID >= 0)
        //            {
        //                ObjGetDeptMedServiceItems_Paging.PageIndex = 0;
        //                GetDeptMedServiceItems_Paging(0, ObjGetDeptMedServiceItems_Paging.PageSize, true);
        //            }
        //            else//-1 Text Chọn Loại Dịch Vụ
        //            {
        //                //Xóa Lưới
        //                ObjGetDeptMedServiceItems_Paging.Clear();
        //                //Xóa Lưới
        //            }
        //        }
        //    }
        //}

        public void Handle(SaveEvent<bool> message)
        {
            if (message != null)
            {
                if (message.Result)
                {
                    ReLoadAfterSave();
                }
            }
        }

        private void ReLoadAfterSave()
        {
            //if (ObjTreeNodeRefDepartments_Current.NodeID > 0)
            {
                if (ObjRefMedicalServiceTypeSelected.MedicalServiceTypeID > 0)
                {
                    SearchCriteria.MedicalServiceTypeID = ObjRefMedicalServiceTypeSelected.MedicalServiceTypeID;
                    ObjMedServiceItems_Paging.PageIndex = 0;
                    GetMedServiceItems_Paging(0, ObjMedServiceItems_Paging.PageSize, true);
                }
            }
        }

        private RefMedicalServiceType _ObjRefMedicalServiceTypeSelected ;//= new RefMedicalServiceType();
        public RefMedicalServiceType ObjRefMedicalServiceTypeSelected
        {
            get
            {
                return _ObjRefMedicalServiceTypeSelected;
            }
            set
            {
                if (_ObjRefMedicalServiceTypeSelected != value)
                {
                    _ObjRefMedicalServiceTypeSelected = value;
                    NotifyOfPropertyChange(() => ObjRefMedicalServiceTypeSelected);

                    //if (ObjTreeNodeRefDepartments_Current.NodeID > 0)
                    {
                        if (ObjRefMedicalServiceTypeSelected.MedicalServiceTypeID > 0)
                        {
                            SearchCriteria.MedicalServiceTypeID = ObjRefMedicalServiceTypeSelected.MedicalServiceTypeID;
                            ObjMedServiceItems_Paging.PageIndex = 0;
                            GetMedServiceItems_Paging(0, ObjMedServiceItems_Paging.PageSize, true);
                        }
                        else//-1 Text Chọn Loại Dịch Vụ
                        {
                            //Xóa Lưới
                            //ObjGetDeptMedServiceItems_Paging.Clear();
                            //Xóa Lưới
                            if (SearchCriteria==null)
                            {
                                SearchCriteria=new RefMedicalServiceItemsSearchCriteria();
                            }
                            SearchCriteria.MedicalServiceTypeID = 0;
                            if (ObjMedServiceItems_Paging == null)
                            {
                                ObjMedServiceItems_Paging = new PagedSortableCollectionView<RefMedicalServiceItem>();
                            }
                            ObjMedServiceItems_Paging.PageIndex = 0;
                            GetMedServiceItems_Paging(0, ObjMedServiceItems_Paging.PageSize, true);
                        }
                    }

                }
            }
        }
        public void txtMedServiceName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btSearch();
            }
        }
        //▼==== #001
        public void BtnExportExcel()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0669_G1_DangLayDLieu);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        ConfigurationReportParams Params = new ConfigurationReportParams()
                        {
                            ConfigurationName = ConfigurationName.RefMedicalServiceItems,
                            MedicalServiceTypeID = ObjRefMedicalServiceTypeSelected.MedicalServiceTypeID
                        };

                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginExportExcelConfigurationManager(Params, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var results = contract.EndExportExcelConfigurationManager(asyncResult);
                                ExportToExcelFileAllData.Export(results, "Shee1");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        //▲==== #001
    }



}
