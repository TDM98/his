using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using aEMR.DataContracts;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
using Castle.Windsor;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Common;
using System.Linq;
using aEMR.Common.BaseModel;
using System.Windows;
/*
 * 20181114 #001 TTM: BM 0005259: Chuyển đổi CellEditEnded => CellEditEnding.
 */
namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(ISupplierGenericDrugPrice_ListDrugBySupplierID)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SupplierGenericDrugPrice_ListDrugBySupplierIDViewModel : ViewModelBase, ISupplierGenericDrugPrice_ListDrugBySupplierID
        ,IHandle<SaveEvent<bool>>
        ,IHandle<DeleteEvent<bool>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SupplierGenericDrugPrice_ListDrugBySupplierIDViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
        }

        //trang nay ko can phan trang
        protected override void OnActivate()
        {
            authorization();
            Globals.EventAggregator.Subscribe(this);
            
            base.OnActivate();
            
            //ObjSupplierGenericDrugPrice_ListDrugBySupplierID_Paging=new ObservableCollection<SupplierGenericDrugPrice>();
            //ObjSupplierGenericDrugPrice_ListDrugBySupplierID_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjSupplierGenericDrugPrice_ListDrugBySupplierID_Paging_OnRefresh);

            //ObjSupplierGenericDrugPrice_ListDrugBySupplierID_Paging.PageIndex = 0;
            SupplierGenericDrugPrice_ListDrugBySupplierID_Paging(0, 0, false);
        }

        void ObjSupplierGenericDrugPrice_ListDrugBySupplierID_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            //SupplierGenericDrugPrice_ListDrugBySupplierID_Paging(ObjSupplierGenericDrugPrice_ListDrugBySupplierID_Paging.PageIndex, ObjSupplierGenericDrugPrice_ListDrugBySupplierID_Paging.PageSize, false);
        }

        private bool _IsLoading = false;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set
            {
                if (_IsLoading != value)
                {
                    _IsLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bTim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyGiaTuNCC,
                                               (int)oPharmacyEx.mQuanLyGiaTuNCC_Tim, (int)ePermission.mView);
            

        }
        #region checking account

        private bool _bTim = true;

        public bool bTim
        {
            get
            {
                return _bTim;
            }
            set
            {
                if (_bTim == value)
                    return;
                _bTim = value;
            }
        }
        
        #endregion
        #region binding visibilty

        public Button hplListPrice { get; set; }

        public void hplListPrice_Loaded(object sender)
        {
            hplListPrice = sender as Button;
            hplListPrice.Visibility = Globals.convertVisibility(bTim);
        }

        #endregion

        private Supplier _ObjSupplierCurrent;
        public Supplier ObjSupplierCurrent
        {
            get { return _ObjSupplierCurrent; }
            set
            {   
                _ObjSupplierCurrent = value;
                NotifyOfPropertyChange(()=>ObjSupplierCurrent);
            }
        }

        private SupplierGenericDrugPriceSearchCriteria _Criteria;
        public SupplierGenericDrugPriceSearchCriteria Criteria
        {
            get
            {
                return _Criteria;
            }
            set
            {
                _Criteria = value;
                NotifyOfPropertyChange(() => Criteria);

            }
        }

        private ObservableCollection<DataEntities.SupplierGenericDrugPrice> ObjSupplierGenericDrugPrice_ListDrugBySupplierID_PagingCopy;
        private ObservableCollection<DataEntities.SupplierGenericDrugPrice> ObjSupplierGenericDrugPrice_ListDrugBySupplierID_PagingRefesh;
        
        private ObservableCollection<DataEntities.SupplierGenericDrugPrice> _ObjSupplierGenericDrugPrice_ListDrugBySupplierID_Paging;
        public ObservableCollection<DataEntities.SupplierGenericDrugPrice> ObjSupplierGenericDrugPrice_ListDrugBySupplierID_Paging
        {
            get { return _ObjSupplierGenericDrugPrice_ListDrugBySupplierID_Paging; }
            set
            {
                _ObjSupplierGenericDrugPrice_ListDrugBySupplierID_Paging = value;
                NotifyOfPropertyChange(() => ObjSupplierGenericDrugPrice_ListDrugBySupplierID_Paging);
            }
        }

        private void SupplierGenericDrugPrice_ListDrugBySupplierID_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.K3080_G1_DSThuoc)});
            this.DlgShowBusyIndicator(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.K3080_G1_DSThuoc));
            IsLoading = true;
            var t = new Thread(() =>
            {
            try
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var client = serviceFactory.ServiceInstance;
                    client.BeginSupplierGenericDrugPrice_ListDrugBySupplierID_Paging(Criteria, PageIndex, PageSize, "", false, Globals.DispatchCallback((asyncResult) =>
                    {
                        int Total = 0;
                        IList<DataEntities.SupplierGenericDrugPrice> allItems = null;
                        try
                        {
                            allItems = client.EndSupplierGenericDrugPrice_ListDrugBySupplierID_Paging(out Total, asyncResult);
                            ObjSupplierGenericDrugPrice_ListDrugBySupplierID_Paging = allItems.ToObservableCollection();
                            ObjSupplierGenericDrugPrice_ListDrugBySupplierID_PagingCopy = ObjSupplierGenericDrugPrice_ListDrugBySupplierID_Paging;
                            ObjSupplierGenericDrugPrice_ListDrugBySupplierID_PagingRefesh = ObjSupplierGenericDrugPrice_ListDrugBySupplierID_Paging.DeepCopy();
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
                    }), null)
                        ;
                }
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogInfo(ex.ToString());
                this.DlgHideBusyIndicator();
            }
            finally
            {
                IsLoading = false;
                //Globals.IsBusy = false;
            }
            });
            t.Start();
        }

        private void SupplierGenericDrugPrice_Update()
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.K3080_G1_DSThuoc) });
            this.DlgShowBusyIndicator(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.K3080_G1_DSThuoc));
            IsLoading = true;
            var t = new Thread(() =>
            {
                try
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var client = serviceFactory.ServiceInstance;
                    client.BeginSupplierGenericDrugPrice_XMLSave(UpdateSupplierGenericDrugPricelst, Globals.DispatchCallback((asyncResult) =>
                    {
                          
                        try
                        {
                            var OK = client.EndSupplierGenericDrugPrice_XMLSave(asyncResult);
                            if (OK)
                            {
                                SupplierGenericDrugPrice_ListDrugBySupplierID_Paging(0,0,false);
                            }
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
                    }), null)
                        ;
                }
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogInfo(ex.ToString());
                this.DlgHideBusyIndicator();
            }
            finally
            {
                IsLoading = false;
                //Globals.IsBusy = false;
            }
        });
        t.Start();
        }

        public void btFind()
        {
            if (string.IsNullOrEmpty(Criteria.BrandName) && string.IsNullOrEmpty(Criteria.GenericName))
            {
                ObjSupplierGenericDrugPrice_ListDrugBySupplierID_Paging = ObjSupplierGenericDrugPrice_ListDrugBySupplierID_PagingCopy;
            }
            else if (string.IsNullOrEmpty(Criteria.GenericName))
            {
                ObjSupplierGenericDrugPrice_ListDrugBySupplierID_Paging = ObjSupplierGenericDrugPrice_ListDrugBySupplierID_PagingCopy.Where(x => x.ObjRefGenericDrugDetail != null && x.ObjRefGenericDrugDetail.BrandName.Contains(Criteria.BrandName)).ToObservableCollection();
            }
            else if (string.IsNullOrEmpty(Criteria.BrandName))
            {
                ObjSupplierGenericDrugPrice_ListDrugBySupplierID_Paging = ObjSupplierGenericDrugPrice_ListDrugBySupplierID_PagingCopy.Where(x => x.ObjRefGenericDrugDetail != null && x.ObjRefGenericDrugDetail.DrugCode == Criteria.GenericName).ToObservableCollection();
            }
            else
            {
                ObjSupplierGenericDrugPrice_ListDrugBySupplierID_Paging = ObjSupplierGenericDrugPrice_ListDrugBySupplierID_PagingCopy.Where(x => x.ObjRefGenericDrugDetail != null && x.ObjRefGenericDrugDetail.BrandName.Contains(Criteria.BrandName) && x.ObjRefGenericDrugDetail.DrugCode == Criteria.GenericName).ToObservableCollection();
            }
            //ObjSupplierGenericDrugPrice_ListDrugBySupplierID_Paging.PageIndex = 0;
            //SupplierGenericDrugPrice_ListDrugBySupplierID_Paging(0, ObjSupplierGenericDrugPrice_ListDrugBySupplierID_Paging.PageSize, true);
        }

        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
            DataEntities.SupplierGenericDrugPrice objRows = e.Row.DataContext as DataEntities.SupplierGenericDrugPrice;
            if (objRows != null)
            {
                switch (objRows.PriceType)
                {
                    case "PriceCurrent":
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Green);
                            break;
                        }
                    case "PriceFuture-Active-1":
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Gray);
                            break;
                        }
                    case "PriceFuture-Active-0":
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Blue);
                            break;
                        }
                    case "PriceOld":
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Orange);
                            break;
                        }
                    default:
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Black);
                            break;
                        }
                }
            }
        }

        public void hplListPrice_Click(object selectItem)
        {
            if (selectItem != null)
            {
                SupplierGenericDrugPrice p = (selectItem as SupplierGenericDrugPrice);

                //var typeInfo = Globals.GetViewModel<ISupplierGenericDrugPrice_ListPrice>();
                //typeInfo.ObjSupplierCurrent = ObjSupplierCurrent;
                //typeInfo.ObjDrugCurrent = p;

                //typeInfo.Criteria = new SupplierGenericDrugPriceSearchCriteria();
                //typeInfo.Criteria.SupplierID = ObjSupplierCurrent.SupplierID;
                //typeInfo.Criteria.DrugID = p.DrugID;
                //typeInfo.Criteria.PriceType = 1;//Giá hiện hành

                //var instance = typeInfo as Conductor<object>;

                //Globals.ShowDialog(instance, (o) =>
                //{
                //    //lam gi do
                //});

                Action<ISupplierGenericDrugPrice_ListPrice> onInitDlg = (typeInfo) =>
                {
                    
                    typeInfo.ObjSupplierCurrent = ObjSupplierCurrent;
                    typeInfo.ObjDrugCurrent = p;

                    typeInfo.Criteria = new SupplierGenericDrugPriceSearchCriteria();
                    typeInfo.Criteria.SupplierID = ObjSupplierCurrent.SupplierID;
                    typeInfo.Criteria.DrugID = p.DrugID;
                    typeInfo.Criteria.PriceType = 1;//Giá hiện hành
                };
                GlobalsNAV.ShowDialog<ISupplierGenericDrugPrice_ListPrice>(onInitDlg);

            }
        }

        public void DoubleClick(object sender, Common.EventArgs<object> e)
        {
            SupplierGenericDrugPrice p = (e.Value as SupplierGenericDrugPrice).DeepCopy();
            if (p != null)
            {
                //var typeInfo = Globals.GetViewModel<ISupplierGenericDrugPrice_ListPrice>();
                //typeInfo.ObjSupplierCurrent = ObjSupplierCurrent;
                //typeInfo.ObjDrugCurrent = p;

                //typeInfo.Criteria = new SupplierGenericDrugPriceSearchCriteria();
                //typeInfo.Criteria.SupplierID = ObjSupplierCurrent.SupplierID;
                //typeInfo.Criteria.DrugID = p.DrugID;
                //typeInfo.Criteria.PriceType = 1;//Giá hiện hành

                //var instance = typeInfo as Conductor<object>;

                //Globals.ShowDialog(instance, (o) =>
                //{
                //    //lam gi do
                //});

                Action<ISupplierGenericDrugPrice_ListPrice> onInitDlg = (typeInfo) =>
                {
                    typeInfo.ObjSupplierCurrent = ObjSupplierCurrent;
                    typeInfo.ObjDrugCurrent = p;

                    typeInfo.Criteria = new SupplierGenericDrugPriceSearchCriteria();
                    typeInfo.Criteria.SupplierID = ObjSupplierCurrent.SupplierID;
                    typeInfo.Criteria.DrugID = p.DrugID;
                    typeInfo.Criteria.PriceType = 1;//Giá hiện hành
                };
                GlobalsNAV.ShowDialog<ISupplierGenericDrugPrice_ListPrice>(onInitDlg);
            }
        }

        public void Handle(SaveEvent<bool> message)
        {
            if (message != null)
            {
                if (message.Result)
                {
                    //ObjSupplierGenericDrugPrice_ListDrugBySupplierID_Paging.PageIndex = 0;
                    SupplierGenericDrugPrice_ListDrugBySupplierID_Paging(0,0, false);

                    //Ny tao su kien nay
                    Globals.EventAggregator.Publish(new ClosePriceEvent { });
                }
            }
        }

        public void Handle(DeleteEvent<bool> message)
        {
            if (message != null)
            {
                if (message.Result)
                {
                    //ObjSupplierGenericDrugPrice_ListDrugBySupplierID_Paging.PageIndex = 0;
                    SupplierGenericDrugPrice_ListDrugBySupplierID_Paging(0, 0, false);
                    //Ny tao su kien nay
                    Globals.EventAggregator.Publish(new ClosePriceEvent { });
                }
            }
        }

        public void btnUndo()
        {
            ObjSupplierGenericDrugPrice_ListDrugBySupplierID_Paging = ObjSupplierGenericDrugPrice_ListDrugBySupplierID_PagingRefesh;
        }
        //update gia hien hanh
        private ObservableCollection<SupplierGenericDrugPrice> UpdateSupplierGenericDrugPricelst;
        public void btnUpdate()
        {
            if (UpdateSupplierGenericDrugPricelst == null)
            {
                UpdateSupplierGenericDrugPricelst = new ObservableCollection<SupplierGenericDrugPrice>();
            }
            else
            {
                UpdateSupplierGenericDrugPricelst.Clear();
            }
            foreach (var item in ObjSupplierGenericDrugPrice_ListDrugBySupplierID_PagingRefesh)
            {
                foreach (var update in ObjSupplierGenericDrugPrice_ListDrugBySupplierID_Paging)
                {
                    if (update.DrugID == item.DrugID)
                    {
                        if (update.EffectiveDate.GetValueOrDefault().ToShortDateString() != item.EffectiveDate.GetValueOrDefault().ToShortDateString())
                        {
                            //so sanh ngay dc sua co hop le khong?
                            if (eHCMS.Services.Core.AxHelper.CompareDate(update.EffectiveDate.GetValueOrDefault(Globals.ServerDate.Value), Globals.ServerDate.Value) == 1 || eHCMS.Services.Core.AxHelper.CompareDate(item.EffectiveDate.GetValueOrDefault(Globals.ServerDate.Value), update.EffectiveDate.GetValueOrDefault(Globals.ServerDate.Value)) == 1)
                            {
                                MessageBox.Show(eHCMSResources.Z1494_G1_NgApDung);
                                return;
                            }
                            update.StaffID = Globals.LoggedUserAccount.StaffID.GetValueOrDefault();
                            UpdateSupplierGenericDrugPricelst.Add(update);
                            break;
                        }
                        else if (update.UnitPrice != item.UnitPrice || update.PackagePrice != item.PackagePrice || update.VAT != item.VAT)
                        {
                            update.StaffID = Globals.LoggedUserAccount.StaffID.GetValueOrDefault();
                            UpdateSupplierGenericDrugPricelst.Add(update);
                            break;
                        }
                        break;
                    }
                }
            }
            if (UpdateSupplierGenericDrugPricelst.Count > 0)
            {
                SupplierGenericDrugPrice_Update();
            }
        }
        public void btnClose()
        {
            TryClose();
        }

        DataGrid dtgList = null;
        public void dtgList_Loaded(object sender, RoutedEventArgs e)
        {
            dtgList = sender as DataGrid;
        }

        string PreparingCellForEdit = "";
        public void dtgList_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            TextBox tbl = dtgList.CurrentColumn.GetCellContent(dtgList.SelectedItem) as TextBox;
            if (tbl != null)
            {
                PreparingCellForEdit = tbl.Text;
            }
        }

        //▼====== #001
        //public void dtgList_CellEditEnded(object sender, DataGridCellEditEndingEventArgs e)
        //{
        //    SupplierGenericDrugPrice item = e.Row.DataContext as SupplierGenericDrugPrice;
           
        //    if (e.Column.DisplayIndex == 5)//NCC
        //    {
        //        decimal value = 0;
        //        decimal.TryParse(PreparingCellForEdit, out value);

        //        if (value == item.UnitPrice)
        //        {
        //            return;
        //        }
        //        item.PackagePrice = item.UnitPrice *(item.ObjRefGenericDrugDetail !=null ? item.ObjRefGenericDrugDetail.UnitPackaging.GetValueOrDefault(1):1);
        //    }
        //    if (e.Column.DisplayIndex == 6)//NCC
        //    {
        //        decimal value = 0;
        //        decimal.TryParse(PreparingCellForEdit, out value);
        //        if (value == item.PackagePrice)
        //        {
        //            return;
        //        }
        //        if (item.ObjRefGenericDrugDetail != null && item.ObjRefGenericDrugDetail.UnitPackaging.GetValueOrDefault() > 0)
        //        {
        //            item.UnitPrice = item.PackagePrice / item.ObjRefGenericDrugDetail.UnitPackaging.GetValueOrDefault(1);
        //        }
        //    }
        //}
        public void dtgList_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            SupplierGenericDrugPrice item = e.Row.DataContext as SupplierGenericDrugPrice;

            if (e.Column.DisplayIndex == 5)//NCC
            {
                decimal value = 0;
                decimal.TryParse(PreparingCellForEdit, out value);

                if (value == item.UnitPrice)
                {
                    return;
                }
                item.PackagePrice = item.UnitPrice * (item.ObjRefGenericDrugDetail != null ? item.ObjRefGenericDrugDetail.UnitPackaging.GetValueOrDefault(1) : 1);
            }
            if (e.Column.DisplayIndex == 6)//NCC
            {
                decimal value = 0;
                decimal.TryParse(PreparingCellForEdit, out value);
                if (value == item.PackagePrice)
                {
                    return;
                }
                if (item.ObjRefGenericDrugDetail != null && item.ObjRefGenericDrugDetail.UnitPackaging.GetValueOrDefault() > 0)
                {
                    item.UnitPrice = item.PackagePrice / item.ObjRefGenericDrugDetail.UnitPackaging.GetValueOrDefault(1);
                }
            }
        }
        //▲====== #001
    }
}

