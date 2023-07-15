using eHCMSLanguage;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using aEMR.DataContracts;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using System.Threading;
using DataEntities;
using System;
using aEMR.Common;
using aEMR.Common.Collections;
using aEMR.Controls;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Common.ExportExcel;
/*
* 20230509 #001 DatTB: IssueID: 3254 | Thêm nút xuất excel cho các danh mục/cấu hình
* 20230601 #002 DatTB: IssueID: 3254 | Chỉnh sửa/Gộp các function xuất excel danh mục/cấu hình (Bỏ Func cũ)
*/
namespace aEMR.Configuration.RefMedicalServiceTypes.ViewModels
{
   [Export(typeof(IRefMedicalServiceTypes)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RefMedicalServiceTypesViewModel : Conductor<object>, IRefMedicalServiceTypes
       ,IHandle<RefMedicalServiceTypes_AddEditEvent_Save>
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
        public RefMedicalServiceTypesViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            authorization();
            //Globals.EventAggregator.Subscribe(this);
           
           Load_V_RefMedicalServiceTypes();

            SearchCriteria = new RefMedicalServiceTypeSearchCriteria();
            SearchCriteria.V_RefMedicalServiceTypes = -1;
            SearchCriteria.MedicalServiceTypeCode = "";
            SearchCriteria.MedicalServiceTypeName = "";
            SearchCriteria.OrderBy = "";
            ObjRefMedicalServiceTypes_Paging = new PagedSortableCollectionView<DataEntities.RefMedicalServiceType>();
            ObjRefMedicalServiceTypes_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjRefMedicalServiceTypes_Paging_OnRefresh);

        
        }

       void ObjRefMedicalServiceTypes_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            RefMedicalServiceTypes_Paging(ObjRefMedicalServiceTypes_Paging.PageIndex,ObjRefMedicalServiceTypes_Paging.PageSize, false);
        }

        private RefMedicalServiceTypeSearchCriteria _SearchCriteria;
        public RefMedicalServiceTypeSearchCriteria SearchCriteria
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

        private PagedSortableCollectionView<DataEntities.RefMedicalServiceType> _ObjRefMedicalServiceTypes_Paging;
        public PagedSortableCollectionView<DataEntities.RefMedicalServiceType> ObjRefMedicalServiceTypes_Paging
        {
            get { return _ObjRefMedicalServiceTypes_Paging; }
            set
            {
                _ObjRefMedicalServiceTypes_Paging = value;
                NotifyOfPropertyChange(() => ObjRefMedicalServiceTypes_Paging);
            }
        }


        private ObservableCollection<Lookup> _ObjV_RefMedicalServiceTypes;
        public ObservableCollection<Lookup> ObjV_RefMedicalServiceTypes
        {
            get { return _ObjV_RefMedicalServiceTypes; }
            set
            {
                _ObjV_RefMedicalServiceTypes = value;
                NotifyOfPropertyChange(() => ObjV_RefMedicalServiceTypes);
            }
        }
        public void Load_V_RefMedicalServiceTypes()
        {
            var t = new Thread(() =>
            {
                //Globals.EventAggregator.Publish(new BusyEvent{IsBusy = true,Message = "Danh Sách Thuộc Loại ..."});

                IsLoading = true;

                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllLookupValuesByType(LookupValues.V_RefMedicalServiceTypes,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);

                                    ObjV_RefMedicalServiceTypes = new ObservableCollection<Lookup>(allItems);
                                    Lookup firstItem = new Lookup();
                                    firstItem.LookupID = -1;
                                    firstItem.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                    ObjV_RefMedicalServiceTypes.Insert(0, firstItem);
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
            bhplEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mDanhMucLoaiDichVu,
                                               (int)oConfigurationEx.mQuanLyLoaiDV, (int)ePermission.mEdit);
            bhplDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mDanhMucLoaiDichVu,
                                               (int)oConfigurationEx.mQuanLyLoaiDV, (int)ePermission.mDelete);
            bhplAddNew = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mDanhMucLoaiDichVu,
                                               (int)oConfigurationEx.mQuanLyLoaiDV, (int)ePermission.mAdd);
            bBtnSearch = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mDanhMucLoaiDichVu,
                                               (int)oConfigurationEx.mQuanLyLoaiDV, (int)ePermission.mView);
        }
        #region checking account

        private bool _bhplEdit = true;
        private bool _bhplDelete = true;
        private bool _bhplAddNew = true;
        private bool _bBtnSearch = true;
        public bool bhplEdit
        {
            get
            {
                return _bhplEdit;
            }
            set
            {
                if (_bhplEdit == value)
                    return;
                _bhplEdit = value;
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
        public bool bBtnSearch
        {
            get
            {
                return _bBtnSearch;
            }
            set
            {
                if (_bBtnSearch == value)
                    return;
                _bBtnSearch = value;
            }
        }
        #endregion
        #region binding visibilty

        public Button hplEdit { get; set; }
        public Button hplDelete { get; set; }
        public void hplEdit_Loaded(object sender)
        {
            hplEdit = sender as Button;
            hplEdit.Visibility = Globals.convertVisibility(bhplEdit);
        }
        public void hplDelete_Loaded(object sender)
        {
            hplDelete = sender as Button;
            hplDelete.Visibility = Globals.convertVisibility(bhplDelete);
        }

        #endregion

        public void hplAddNewRoomType_Click()
        {
            //var typeInfo = Globals.GetViewModel<IRefMedicalServiceTypes_AddEdit>();
            //typeInfo.TitleForm = eHCMSResources.A0466_G1_Msg_ThemMoiLoaiDV;

            //typeInfo.InitializeNewItem();

            //var instance = typeInfo as Conductor<object>;

            //Globals.ShowDialog(instance, (o) =>
            //                                 {
            //                                     //làm gì đó
            //                                 });

            Action<IRefMedicalServiceTypes_AddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = eHCMSResources.A0466_G1_Msg_ThemMoiLoaiDV;
                typeInfo.InitializeNewItem();
            };
            GlobalsNAV.ShowDialog<IRefMedicalServiceTypes_AddEdit>(onInitDlg);
        }

        public void hplEdit_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                //var typeInfo = Globals.GetViewModel<IRefMedicalServiceTypes_AddEdit>();
                //typeInfo.ObjRefMedicalServiceTypes_Current = ObjectCopier.DeepCopy((selectedItem as DataEntities.RefMedicalServiceType));

                //typeInfo.TitleForm = "Hiệu Chỉnh (" + (selectedItem as DataEntities.RefMedicalServiceType).MedicalServiceTypeName.Trim() + ")";
                
                //var instance = typeInfo as Conductor<object>;

                //Globals.ShowDialog(instance, (o) =>
                //                                 {
                //                                     //làm gì đó
                //                                 });

                Action<IRefMedicalServiceTypes_AddEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.ObjRefMedicalServiceTypes_Current = ObjectCopier.DeepCopy((selectedItem as DataEntities.RefMedicalServiceType));
                    typeInfo.TitleForm = "Hiệu Chỉnh (" + (selectedItem as DataEntities.RefMedicalServiceType).MedicalServiceTypeName.Trim() + ")";
                };
                GlobalsNAV.ShowDialog<IRefMedicalServiceTypes_AddEdit>(onInitDlg);
            }
        }

        public void btFind()
        {
            ObjRefMedicalServiceTypes_Paging.PageIndex = 0;
            RefMedicalServiceTypes_Paging(0, ObjRefMedicalServiceTypes_Paging.PageSize, true);
        }

        private void RefMedicalServiceTypes_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách Loại Dịch Vụ..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginRefMedicalServiceTypes_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DataEntities.RefMedicalServiceType> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndRefMedicalServiceTypes_Paging(out Total, asyncResult);
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

                            ObjRefMedicalServiceTypes_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjRefMedicalServiceTypes_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjRefMedicalServiceTypes_Paging.Add(item);
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
       
        public void hplDelete_Click(object selectedItem)
        {
            DataEntities.RefMedicalServiceType p = (selectedItem as DataEntities.RefMedicalServiceType);

            if (p != null)
            {
                if (p.MedicalServiceTypeID > 0)
                {
                    if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, p.MedicalServiceTypeName), eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        RefMedicalServiceTypes_MarkDelete(p.MedicalServiceTypeID);
                    }
                }

                //switch (p.MedicalServiceTypeID)
                //{
                //    case 1:
                //        {
                //            MessageBox.Show("Loại Dịch Vụ Khám Chữa Bệnh Này Là Của Hệ Thống! Không Xóa Được!");
                //            break;
                //        }
                //    case 2:
                //        {
                //            MessageBox.Show("Loại Dịch Vụ Cận Lâm Sàn Này Là Của Hệ Thống! Không Xóa Được!");
                //            break;
                //        }
                //    default:
                //        {
                //            if (p.MedicalServiceTypeID > 0)
                //            {
                //                if (MessageBox.Show(string.Format("{0}: ", eHCMSResources.Z0354_G1_BanCoMuonXoa) + p.MedicalServiceTypeName + ", Này Không?",eHCMSResources.G2617_G1_Xoa,MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                //                {
                //                    RefMedicalServiceTypes_MarkDelete(p.MedicalServiceTypeID);
                //                }
                //            }
                //            break; 
                //        }
                //}
            }
        }

        private void RefMedicalServiceTypes_MarkDelete(Int64 MedicalServiceTypeID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Xóa..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginRefMedicalServiceTypes_MarkDelete(MedicalServiceTypeID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            if(contract.EndRefMedicalServiceTypes_MarkDelete(asyncResult))
                            {
                                ObjRefMedicalServiceTypes_Paging.PageIndex = 0;
                                RefMedicalServiceTypes_Paging(0, ObjRefMedicalServiceTypes_Paging.PageSize, true);
                                MessageBox.Show(eHCMSResources.A0478_G1_Msg_InfoXoaOK, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.K0484_G1_XoaFail, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);    
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


        public void Handle(RefMedicalServiceTypes_AddEditEvent_Save message)
        {
            if (message != null)
            {
                ObjRefMedicalServiceTypes_Paging.PageIndex = 0;
                RefMedicalServiceTypes_Paging(0, ObjRefMedicalServiceTypes_Paging.PageSize, true);
            }
        }

        public void cboV_RefMedicalServiceTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AxComboBox Ctr = sender as AxComboBox;
            if(Ctr!=null)
            {
                ObjRefMedicalServiceTypes_Paging.PageIndex = 0;
                RefMedicalServiceTypes_Paging(0, ObjRefMedicalServiceTypes_Paging.PageSize, true);
            }
        }

        //▼==== #002
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
                            ConfigurationName = ConfigurationName.RefMedicalServiceTypes
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
        //▲==== #002
    }
}
