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

/*
 * #001 20180921 TNHX: Apply BusyIndicator, refactor code
 */
namespace aEMR.Configuration.InsuranceBenefitCategories.ViewModels
{
    [Export(typeof(IInsuranceBenefitCategories_ListFind)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InsuranceBenefitCategories_ListFindViewModel : ViewModelBase, IInsuranceBenefitCategories_ListFind
        , IHandle<InsuranceBenefitCategories_Event_Save>
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
        public InsuranceBenefitCategories_ListFindViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            authorization();
            //Globals.EventAggregator.Subscribe(this);


            HIPCode = "";


            ObjInsuranceBenefitPaging = new PagedSortableCollectionView<InsuranceBenefitCategories_Data>();
            ObjInsuranceBenefitPaging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjInsuranceBenefitPaging_OnRefresh);
        }

        void ObjInsuranceBenefitPaging_OnRefresh(object sender, RefreshEventArgs e)
        {
            InsuranceBenefitPaging(ObjInsuranceBenefitPaging.PageIndex,
                            ObjInsuranceBenefitPaging.PageSize, false);
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


     

        private string _HIPCode;
        public string HIPCode
        {
            get
            {
                return _HIPCode;
            }
            set
            {
                _HIPCode = value;
                NotifyOfPropertyChange(() => HIPCode);
            }
        }

        private PagedSortableCollectionView<InsuranceBenefitCategories_Data> _ObjInsuranceBenefitPaging;
        public PagedSortableCollectionView<InsuranceBenefitCategories_Data> ObjInsuranceBenefitPaging
        {
            get { return _ObjInsuranceBenefitPaging; }
            set
            {
                _ObjInsuranceBenefitPaging = value;
                NotifyOfPropertyChange(() => ObjInsuranceBenefitPaging);
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

        public void btSearch()
        {
            ObjInsuranceBenefitPaging.PageIndex = 0;
            InsuranceBenefitPaging(0, ObjInsuranceBenefitPaging.PageSize, true);
        }

        public void InsuranceBenefitCategories_MarkDeleted(Int64 IDCode)
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
                        contract.BeginICD_MarkDelete(IDCode, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndICD_MarkDelete(out Result, asyncResult);
                                //if (Result == "InUse")
                                //{
                                //    Globals.ShowMessage(eHCMSResources.Z1318_G1_PhgDangSDungKgTheXoa, eHCMSResources.G2617_G1_Xoa);
                                //}
                                if (Result == "0")
                                {
                                    Globals.ShowMessage("Thất bại", "Thông báo");
                                }
                                if (Result == "1")
                                {
                                    ObjInsuranceBenefitPaging.PageIndex = 0;
                                    InsuranceBenefitPaging(0, ObjInsuranceBenefitPaging.PageSize, true);
                                    Globals.ShowMessage("Dừng sử dụng ICD", "Thông báo");
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

        public void hplDelete_Click(object selectedItem)
        {

            ICD p = (selectedItem as ICD);
            if (p != null && p.IDCode > 0)
            {
                if (MessageBox.Show(string.Format("Bạn có muốn dừng ICD này", p.ICD10Code), "Tạm dừng ICD", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    InsuranceBenefitCategories_MarkDeleted(p.IDCode);
                }
            }
        }

        private object _InsuranceBenefitCategories_Current;
        public object InsuranceBenefitCategories_Current
        {
            get { return _InsuranceBenefitCategories_Current; }
            set
            {
                _InsuranceBenefitCategories_Current = value;
                NotifyOfPropertyChange(() => InsuranceBenefitCategories_Current);
            }
        }

        public void DoubleClick(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;
            InsuranceBenefitCategories_Current = eventArgs.Value as InsuranceBenefit;
            Globals.EventAggregator.Publish(new dgInsuranceBenefitCategoriesListClickSelectionChanged_Event() { InsuranceBenefitCategories_Current = eventArgs.Value });
        }

        public void dtgListSelectionChanged(object args)
        {
            //if (((object[])(((System.Windows.Controls.SelectionChangedEventArgs)(args)).AddedItems)).Length > 0)
            //{
            //    if (((object[])(((System.Windows.Controls.SelectionChangedEventArgs)(args)).AddedItems))[0] != null)
            //    {
            //        InsuranceBenefitCategories_Current =
            //            ((object[])(((System.Windows.Controls.SelectionChangedEventArgs)(args)).AddedItems))[0];
            //        var typeInfo = Globals.GetViewModel<IICD_ListFind>();
            //        typeInfo.ICD_Current = (ICD)InsuranceBenefitCategories_Current;

            //        Globals.EventAggregator.Publish(new dgICDListClickSelectionChanged_Event()
            //        {
            //            ICD_Current = ((object[]) (((System.Windows.Controls.SelectionChangedEventArgs) (args)).AddedItems))[0]
            //        });
            //    }
            //}
        }

        private void InsuranceBenefitPaging(int PageIndex, int PageSize, bool CountTotal)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách Phòng..." });
            this.DlgShowBusyIndicator(eHCMSResources.K3054_G1_DSPg);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginInsuranceBenefitPaging(HIPCode, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<InsuranceBenefitCategories_Data> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndInsuranceBenefitPaging(out Total, asyncResult);
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

                            ObjInsuranceBenefitPaging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjInsuranceBenefitPaging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjInsuranceBenefitPaging.Add(item);
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
            Action<IInsuranceBenefitCategories_AddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = "Thêm Mới quyền lợi bảo hiểm";
                typeInfo.InitializeNewItem();
            };
            GlobalsNAV.ShowDialog(onInitDlg);
        }

        public void hplEdit_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                Action<IInsuranceBenefitCategories_AddEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.ObjInsuranceBenefitCategories_Current = ObjectCopier.DeepCopy((selectedItem as InsuranceBenefitCategories_Data));
                    typeInfo.TitleForm = "Hiệu Chỉnh (" + (selectedItem as InsuranceBenefitCategories_Data).HIPCode.Trim() + ")";
                };
                GlobalsNAV.ShowDialog<IInsuranceBenefitCategories_AddEdit>(onInitDlg);
            }
        }

        public void Handle(InsuranceBenefitCategories_Event_Save message)
        {
            ObjInsuranceBenefitPaging.PageIndex = 0;
            InsuranceBenefitPaging(0, ObjInsuranceBenefitPaging.PageSize, true);
        }
    }
}
