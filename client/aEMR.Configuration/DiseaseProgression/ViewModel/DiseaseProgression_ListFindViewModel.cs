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
using aEMR.Common.ExportExcel;
/*
* #001 20180921 TNHX: Apply BusyIndicator, refactor code
* 20230601 #002 DatTB: IssueID: 3254 | Chỉnh sửa/Gộp các function xuất excel danh mục/cấu hình (Bỏ Func cũ)
*/
namespace aEMR.Configuration.DiseaseProgression.ViewModels
{
    [Export(typeof(IDiseaseProgression_ListFind)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DiseaseProgression_ListFindViewModel : ViewModelBase, IDiseaseProgression_ListFind
        , IHandle<DiseaseProgression_Event_Save>
    {
        protected override void OnActivate()
        {
            authorization();
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
        public DiseaseProgression_ListFindViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            authorization();

            LoadDiseaseProgression();
            SearchDiseaseProgression = "";
       

            ObjDiseaseProgression_Paging = new PagedSortableCollectionView<DataEntities.DiseaseProgression>();
            ObjDiseaseProgression_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjDiseaseProgression_Paging_OnRefresh);
            DiseaseProgression_Paging(0,ObjDiseaseProgression_Paging.PageSize, false);
            ObjDiseaseProgressionDetails_Paging = new PagedSortableCollectionView<DiseaseProgressionDetails>();
            ObjDiseaseProgressionDetails_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjDiseaseProgressionDetails_Paging_OnRefresh);
            DiseaseProgressionDetails_Paging(0, ObjDiseaseProgressionDetails_Paging.PageSize, false);
        }

        void ObjDiseaseProgression_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            DiseaseProgression_Paging(ObjDiseaseProgression_Paging.PageIndex, ObjDiseaseProgression_Paging.PageSize, false);
        }
        void ObjDiseaseProgressionDetails_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            DiseaseProgressionDetails_Paging(ObjDiseaseProgressionDetails_Paging.PageIndex, ObjDiseaseProgressionDetails_Paging.PageSize, false);
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
     
        private string _SearchDiseaseProgression;
        public string SearchDiseaseProgression
        {
            get
            {
                return _SearchDiseaseProgression;
            }
            set
            {
                _SearchDiseaseProgression = value;
                NotifyOfPropertyChange(() => SearchDiseaseProgression);
            }
        }

        private string _SearchDiseaseProgressionDetails;
        public string SearchDiseaseProgressionDetails
        {
            get
            {
                return _SearchDiseaseProgressionDetails;
            }
            set
            {
                _SearchDiseaseProgressionDetails = value;
                NotifyOfPropertyChange(() => SearchDiseaseProgressionDetails);
            }
        }

        private PagedSortableCollectionView<DataEntities.DiseaseProgression> _ObjDiseaseProgression_Paging;
        public PagedSortableCollectionView<DataEntities.DiseaseProgression> ObjDiseaseProgression_Paging
        {
            get { return _ObjDiseaseProgression_Paging; }
            set
            {
                _ObjDiseaseProgression_Paging = value;
                NotifyOfPropertyChange(() => ObjDiseaseProgression_Paging);
            }
        }

        private PagedSortableCollectionView<DiseaseProgressionDetails> _ObjDiseaseProgressionDetails_Paging;
        public PagedSortableCollectionView<DiseaseProgressionDetails> ObjDiseaseProgressionDetails_Paging
        {
            get { return _ObjDiseaseProgressionDetails_Paging; }
            set
            {
                _ObjDiseaseProgressionDetails_Paging = value;
                NotifyOfPropertyChange(() => ObjDiseaseProgressionDetails_Paging);
            }
        }

     
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            //bhplEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
            //                                   , (int)eConfiguration_Management.mDanhMucPhong,
            //                                   (int)oConfigurationEx.mQuanLyDanhSachPhong, (int)ePermission.mEdit);
            //bhplDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
            //                                   , (int)eConfiguration_Management.mDanhMucPhong,
            //                                   (int)oConfigurationEx.mQuanLyDanhSachPhong, (int)ePermission.mDelete);
            //bbtSearch = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
            //                            , (int)eConfiguration_Management.mDanhMucPhong,
            //                            (int)oConfigurationEx.mQuanLyDanhSachPhong, (int)ePermission.mView);
            //bhplAddNew = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
            //                                    , (int)eConfiguration_Management.mDanhMucPhong,
            //                                    (int)oConfigurationEx.mQuanLyDanhSachPhong, (int)ePermission.mAdd);
        }

        #region checking account
        private bool _bhplEdit = true;
        private bool _bhplDelete = true;
        private bool _bbtSearch = true;
        private bool _bhplAddNew = true;
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
        #endregion

        #region binding visibilty
        public Button hplEdit { get; set; }
        public void hplEdit_Loaded(object sender)
        {
            hplEdit = sender as Button;
            hplEdit.Visibility = Globals.convertVisibility(bhplEdit);
        }
       
        #endregion

        public void btSearch()
        {
            ObjDiseaseProgression_Paging.PageIndex = 0;
            DiseaseProgression_Paging(0, ObjDiseaseProgression_Paging.PageSize, true);
        }
        public void btSearchDiseaseProgressionDetails()
        {
            ObjDiseaseProgressionDetails_Paging.PageIndex = 0;
            DiseaseProgressionDetails_Paging(0, ObjDiseaseProgressionDetails_Paging.PageSize, true);
        }

        public void DiseaseProgression_MarkDeleted(long DiseaseProgressionID)
        {
            string Result = "";
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Xóa..." });
            this.DlgShowBusyIndicator(eHCMSResources.Z0492_G1_DangXoa);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDiseaseProgression_MarkDelete(DiseaseProgressionID, (long)Globals.LoggedUserAccount.StaffID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndDiseaseProgression_MarkDelete(out Result, asyncResult);
                                if (Result == "0")
                                {
                                    Globals.ShowMessage("Xóa không thành công", "Thông báo");
                                }
                                if (Result == "1")
                                {
                                    ObjDiseaseProgression_Paging.PageIndex = 0;
                                    DiseaseProgression_Paging(0, ObjDiseaseProgression_Paging.PageSize, true);
                                    LoadDiseaseProgression();
                                    Globals.ShowMessage("Đã xóa", "Thông báo");
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }

        public void DiseaseProgressionDetail_MarkDeleted(long DiseaseProgressionDetailID)
        {
            string Result = "";
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Xóa..." });
            this.DlgShowBusyIndicator(eHCMSResources.Z0492_G1_DangXoa);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDiseaseProgressionDetail_MarkDelete(DiseaseProgressionDetailID, (long)Globals.LoggedUserAccount.StaffID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndDiseaseProgressionDetail_MarkDelete(out Result, asyncResult);
                                if (Result == "0")
                                {
                                    Globals.ShowMessage("Xóa không thành công", "Thông báo");
                                }
                                if (Result == "1")
                                {
                                    ObjDiseaseProgressionDetails_Paging.PageIndex = 0;
                                    DiseaseProgressionDetails_Paging(0, ObjDiseaseProgressionDetails_Paging.PageSize, true);
                                    Globals.ShowMessage("Đã xóa", "Thông báo");
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }

       
        public void hplDeleteDiseaseProgression_Click(object selectedItem)
        {
            DataEntities.DiseaseProgression p = (selectedItem as DataEntities.DiseaseProgression);
            if (p != null && p.DiseaseProgressionID > 0)
            {
                if (MessageBox.Show(string.Format("Bạn có muốn xóa ", p.DiseaseProgressionName), "Xóa diễn tiến", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    DiseaseProgression_MarkDeleted(p.DiseaseProgressionID);
                }
            }
        }
        public void hplDeleteDiseaseProgressionDetail_Click(object selectedItem)
        {

            DiseaseProgressionDetails p = (selectedItem as DiseaseProgressionDetails);
            if (p != null && p.DiseaseProgressionDetailID > 0)
            {
                if (MessageBox.Show(string.Format("Bạn có muốn xóa ", p.DiseaseProgressionDetailName), "Xóa chi tiết", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    DiseaseProgressionDetail_MarkDeleted(p.DiseaseProgressionDetailID);
                }
            }
        }

        private long _DiseaseProgressionID = 0;
        public long DiseaseProgressionID
        {
            get { return _DiseaseProgressionID; }
            set
            {
                _DiseaseProgressionID = value;
                NotifyOfPropertyChange(() => DiseaseProgressionID);
            }
        }

        private ObservableCollection<DataEntities.DiseaseProgression> _DiseaseProgression;
        public ObservableCollection<DataEntities.DiseaseProgression> DiseaseProgression
        {
            get { return _DiseaseProgression; }
            set
            {
                _DiseaseProgression = value;
                NotifyOfPropertyChange(() => DiseaseProgression);
            }
        }

        private ObservableCollection<SuburbNames> _SuburbNames;
        public ObservableCollection<SuburbNames> SuburbNames
        {
            get { return _SuburbNames; }
            set
            {
                _SuburbNames = value;
                NotifyOfPropertyChange(() => SuburbNames);
            }
        }

        private void DiseaseProgression_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            this.DlgShowBusyIndicator("Danh sách diễn tiến bệnh");
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginDiseaseProgression_Paging(SearchDiseaseProgression, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DataEntities.DiseaseProgression> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndDiseaseProgression_Paging(out Total, asyncResult);
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

                            ObjDiseaseProgression_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjDiseaseProgression_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjDiseaseProgression_Paging.Add(item);
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
        private void DiseaseProgressionDetails_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            this.DlgShowBusyIndicator("Danh sách chi tiết");
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginDiseaseProgressionDetails_Paging(DiseaseProgressionID, SearchDiseaseProgressionDetails, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DiseaseProgressionDetails> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndDiseaseProgressionDetails_Paging(out Total, asyncResult);
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

                            ObjDiseaseProgressionDetails_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjDiseaseProgressionDetails_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjDiseaseProgressionDetails_Paging.Add(item);
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
      
        public void hplAddNew_Click()
        {
            Action<IDiseaseProgression_AddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = "Thêm mới diễn tiến bệnh";
                typeInfo.FormType = 1;
                typeInfo.InitializeNewItem();
            };
            GlobalsNAV.ShowDialog(onInitDlg);
        }
        public void hplAddNewSuburbNames_Click()
        {
            Action<IDiseaseProgression_AddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = "Thêm mới diễn tiến bệnh chi tiết";
                typeInfo.DiseaseProgression = DiseaseProgression;
                typeInfo.FormType = 2;
                if (DiseaseProgressionID == 0)
                {
                    typeInfo.InitializeNewItem();
                }
                else
                {
                    typeInfo.ObjDiseaseProgressionDetails_Current = new DiseaseProgressionDetails();
                    typeInfo.ObjDiseaseProgressionDetails_Current.DiseaseProgressionID = DiseaseProgressionID;
                }
            };
            GlobalsNAV.ShowDialog(onInitDlg);
        }
        
        
        public void hplEdit_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                Action<IDiseaseProgression_AddEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.ObjDiseaseProgression_Current = ObjectCopier.DeepCopy((selectedItem as DataEntities.DiseaseProgression));
                    typeInfo.TitleForm = "Hiệu Chỉnh (" + (selectedItem as DataEntities.DiseaseProgression).DiseaseProgressionName.Trim() + ")";
                    typeInfo.FormType = 1;
                };
                GlobalsNAV.ShowDialog<IDiseaseProgression_AddEdit>(onInitDlg);
            }
        }
        public void hplEditSuburbNames_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                Action<IDiseaseProgression_AddEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.DiseaseProgression = DiseaseProgression;
                    typeInfo.ObjDiseaseProgressionDetails_Current = ObjectCopier.DeepCopy((selectedItem as DataEntities.DiseaseProgressionDetails));
                    typeInfo.TitleForm = "Hiệu Chỉnh (" + (selectedItem as DataEntities.DiseaseProgressionDetails).DiseaseProgressionDetailName.Trim() + ")";
                    typeInfo.FormType = 2;
                };
                GlobalsNAV.ShowDialog<IDiseaseProgression_AddEdit>(onInitDlg);
            }
        }
      
        public void LoadDiseaseProgression()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllDiseaseProgression(true,Globals.DispatchCallback(asyncResult =>
                        {
                            IList<DataEntities.DiseaseProgression> allItems = null;
                            try
                            {
                                allItems = contract.EndGetAllDiseaseProgression(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                MessageBox.Show(eHCMSResources.A0693_G1_Msg_InfoKhTheLayDSTinhThanh);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }

                            DiseaseProgression = allItems != null ? new ObservableCollection<DataEntities.DiseaseProgression>(allItems) : null;
                            DiseaseProgressionID = (DiseaseProgression!=null && DiseaseProgression.Count >0 && DiseaseProgressionID == 0) ? DiseaseProgression.FirstOrDefault().DiseaseProgressionID : DiseaseProgressionID;
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public void cboDiseaseProgressionSelectedItemChanged(object selectedItem)
        {
            if(selectedItem != null)
            {
                DiseaseProgressionID = (selectedItem as DataEntities.DiseaseProgression).DiseaseProgressionID;
            }
            ObjDiseaseProgressionDetails_Paging.PageIndex = 0;
            DiseaseProgressionDetails_Paging(0, ObjDiseaseProgressionDetails_Paging.PageSize, true);
        }
       
      
        public void Handle(DiseaseProgression_Event_Save message)
        {
            if (message.FormType == 1)
            {
                ObjDiseaseProgression_Paging.PageIndex = 0;
                DiseaseProgression_Paging(0, ObjDiseaseProgression_Paging.PageSize,true);
                LoadDiseaseProgression();
            }
            else if (message.FormType == 2)
            {
                ObjDiseaseProgressionDetails_Paging.PageIndex = 0;
                DiseaseProgressionDetails_Paging(0, ObjDiseaseProgressionDetails_Paging.PageSize, true);
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
                            ConfigurationName = ConfigurationName.DiseaseProgression
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
