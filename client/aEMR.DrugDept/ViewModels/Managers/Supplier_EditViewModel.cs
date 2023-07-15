using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using System.Windows.Controls;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Controls;
using System.Linq;
using System.Collections.Generic;
/*
 * 20190911 #001 TTM:   BM 0013236: Cho phép sửa giá lẻ nhà cung cấp tại màn hình quản lý nhà cung cấp. Để phục vụ sửa giá hàng loạt cho nhà cung cấp.
 * 20191109 #002 TTM:   BM 0018533: Bổ sung thông báo lỗi khi cập nhật nhà cung cấp => Vì ValidationSummary không còn hoạt động được trong WPF nên phải có thông báo cho người dùng biết thiếu thông tin gì.
 */
namespace aEMR.DrugDept.ViewModels
{
     [Export(typeof(ISupplierProduct_Edit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class Supplier_EditViewModel : Conductor<object>, ISupplierProduct_Edit
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public Supplier_EditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;


            _SelectedSupplier = new DrugDeptSupplier();

             ListSupplierProduct = new PagedSortableCollectionView<SupplierGenMedProduct>();
             ListSupplierProduct.OnRefresh += ListSupplierProduct_OnRefresh;
             ListSupplierProduct.PageSize = Globals.PageSize;
         }
         void ListSupplierProduct_OnRefresh(object sender, RefreshEventArgs e)
         {
             SupplierGenMedProduct_LoadSupplierID(ListSupplierProduct.PageIndex, ListSupplierProduct.PageSize);
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

         private PagedSortableCollectionView<SupplierGenMedProduct> _ListSupplierProduct;
         public PagedSortableCollectionView<SupplierGenMedProduct> ListSupplierProduct
         {
             get
             {
                 return _ListSupplierProduct;
             }
             set
             {
                 if (_ListSupplierProduct != value)
                 {
                     _ListSupplierProduct = value;
                 }
                 NotifyOfPropertyChange(() => ListSupplierProduct);
             }
         }

         private DrugDeptSupplier _SelectedSupplier;
         public DrugDeptSupplier SelectedSupplier
         {
             get
             {
                 if (_SelectedSupplier == null)
                     _SelectedSupplier = new DrugDeptSupplier();
                 return _SelectedSupplier;
            }
             set
             {
                 _SelectedSupplier = value;
                 NotifyOfPropertyChange(()=>SelectedSupplier);
             }
         }
        public bool CheckValid(object temp)
         {
             DrugDeptSupplier u = temp as DrugDeptSupplier;
             if (u == null)
             {
                 return false;
             }
             return u.Validate();
         }

         public void UpdateSupplier()
         {
             Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
             var t = new Thread(() =>
             {
                 using (var serviceFactory = new PharmacySuppliersServiceClient())
                 {
                     var contract = serviceFactory.ServiceInstance;

                     contract.BeginUpdateDrugDeptSupplier(SelectedSupplier, Globals.DispatchCallback((asyncResult) =>
                     {

                         try
                         {
                             string StrError = "";
                             contract.EndUpdateDrugDeptSupplier(out StrError, asyncResult);
                             if (string.IsNullOrEmpty(StrError))
                             {
                                 GlobalsNAV.ShowMessagePopup(eHCMSResources.A0279_G1_Msg_InfoCNhatOK);
                                 TryClose();
                                 //phat su kien de form cha load lai du lieu
                                 Globals.EventAggregator.Publish(new DrugDeptCloseEditSupplierEvent());
                             }
                             else
                             {
                                 MessageBox.Show(StrError);
                             }
                             }
                         catch (Exception ex)
                         {
                             Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                         }
                         finally
                         {
                             IsLoading = false;
                             Globals.IsBusy = false;
                         }

                     }), null);

                 }

             });

             t.Start();
         }

         public void OKButton(object sender, RoutedEventArgs e)
         {
            //▼===== #002
            string ErrorMsg = "";
            //if (CheckValid(SelectedSupplier))
            //{
            //    UpdateSupplier();
            //}
            if (!CheckValidForSupplier(out ErrorMsg, SelectedSupplier))
            {
                MessageBox.Show(ErrorMsg, eHCMSResources.G0449_G1_TBaoLoi);
                return;
            }
            else
            {
                UpdateSupplier();
            }
            //▲===== #002
        }
        private bool CheckValidForSupplier(out string Error, DrugDeptSupplier curSupplier)
        {
            Error = "";
            if (curSupplier == null)
            {
                return false;
            }
            //Supplier tmpSupplier = (Supplier)curSupplier;
            if (string.IsNullOrEmpty(curSupplier.SupplierName))
            {
                Error += string.Format("\n - {0}", eHCMSResources.Z2905_G1_NhapTenNCC);
            }
            if (string.IsNullOrEmpty(curSupplier.SupplierCode))
            {
                Error += string.Format("\n - {0}", eHCMSResources.Z2906_G1_NhapMaNCC);
            }
            if (string.IsNullOrEmpty(curSupplier.Address))
            {
                Error += string.Format("\n - {0}", eHCMSResources.Z2907_G1_NhapDiaChiNCC);
            }
            if (string.IsNullOrEmpty(curSupplier.TelephoneNumber))
            {
                Error += string.Format("\n - {0}", eHCMSResources.Z2910_G1_NhapSDT);
            }
            if (string.IsNullOrEmpty(curSupplier.CityStateZipCode))
            {
                Error += string.Format("\n - {0}", eHCMSResources.Z2909_G1_NhapMaTinhThanh);
            }
            if (!string.IsNullOrEmpty(Error))
            {
                return false;
            }
            return true;
        }
        public void CancelButton(object sender, RoutedEventArgs e)
         {
             TryClose();
             //phat su kien de form cha load lai du lieu
             Globals.EventAggregator.Publish(new DrugDeptCloseAddSupplierEvent());
         }
        #region DS Hang CC Member

         public void GridSuppliers_LoadingRow(object sender, DataGridRowEventArgs e)
         {
             e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
         }

         private void SupplierGenMedProduct_LoadSupplierID(int PageIndex, int PageSize)
         {
             Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
             int totalCount = 0;
             var t = new Thread(() =>
             {
                 using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                 {
                     var contract = serviceFactory.ServiceInstance;
                     contract.BeginSupplierGenMedProduct_LoadSupplierID(SelectedSupplier.SupplierID, PageSize, PageIndex, true, Globals.DispatchCallback((asyncResult) =>
                     {

                         try
                         {
                             var results = contract.EndSupplierGenMedProduct_LoadSupplierID(out totalCount, asyncResult);
                             ListSupplierProduct.Clear();
                             if (results != null)
                             {
                                 ListSupplierProduct.TotalItemCount = totalCount;
                                 foreach (SupplierGenMedProduct p in results)
                                 {
                                     ListSupplierProduct.Add(p);
                                 }
                                 NotifyOfPropertyChange(() => ListSupplierProduct);
                             }
                         }
                         catch (Exception ex)
                         {
                             Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                         }
                         finally
                         {
                             IsLoading = false;
                             Globals.IsBusy = false;
                         }

                     }), null);

                 }

             });

             t.Start();
         }

         public void LoadDSHangCC()
         {
             ListSupplierProduct.PageIndex = 0;
             SupplierGenMedProduct_LoadSupplierID(ListSupplierProduct.PageIndex,ListSupplierProduct.PageSize);
         }

         public void chkIsAll_Check(object sender, RoutedEventArgs e)
         {
            SelectedSupplier.SupplierDrugDeptPharmOthers = 3;
         }
         public void chkIsAll_Uncheck(object sender, RoutedEventArgs e)
         {
            SelectedSupplier.SupplierDrugDeptPharmOthers = 1;
         }
        #endregion
        //▼===== #001
        #region Sửa giá
        DataGrid GridSuppliers = null;
        public void GridSuppliers_Loaded(object sender, RoutedEventArgs e)
        {
            GridSuppliers = sender as DataGrid;
            if (GridSuppliers != null)
            {
                GridSuppliers.CurrentCellChanged += GridSuppliers_CurrentCellChanged;
            }
        }
        private SupplierGenMedProduct EditedDetailItem { get; set; }
        private string EditedColumnName { get; set; }
        private SupplierGenMedProduct _CurrentSupplierGenMedProduct;
        public SupplierGenMedProduct CurrentSupplierGenMedProduct
        {
            get
            {
                return _CurrentSupplierGenMedProduct;
            }
            set
            {
                if (_CurrentSupplierGenMedProduct != value)
                {
                    _CurrentSupplierGenMedProduct = value;
                    NotifyOfPropertyChange(() => CurrentSupplierGenMedProduct);
                }
            }
        }
        private void GridSuppliers_CurrentCellChanged(object sender, EventArgs e)
        {
            if (CurrentSupplierGenMedProduct == null || string.IsNullOrEmpty(EditedColumnName))
            {
                return;
            }
            SupplierGenMedProduct item = CurrentSupplierGenMedProduct;
            if (EditedColumnName.Equals("colUnitPrice"))
            {
                EditedColumnName = null;
                decimal value = 0;
                decimal.TryParse(PreparingCellForEdit, out value);
                if (value == item.UnitPrice)
                {
                    return;
                }
                item.PackagePrice = item.UnitPrice * (int)item.SelectedGenMedProduct.UnitPackaging;
            }
            else if (EditedColumnName.Equals("colPackagePrice"))
            {
                EditedColumnName = null;
                decimal value = 0;
                decimal.TryParse(PreparingCellForEdit, out value);
                if (value == item.PackagePrice)
                {
                    return;
                }
                if (EditedDetailItem.PackagePrice > 0)
                {
                    item.UnitPrice = item.PackagePrice / (int)item.SelectedGenMedProduct.UnitPackaging;
                }
            }
            EditedColumnName = null;
        }
        string PreparingCellForEdit = "";
        public void GridSuppliers_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            var mAxTextBox = e.EditingElement.GetChildrenByType<AxTextBox>().FirstOrDefault();
            if (mAxTextBox != null)
            {
                PreparingCellForEdit = mAxTextBox.Text;
            }
        }
        public void GridSuppliers_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            SupplierGenMedProduct item = e.Row.DataContext as SupplierGenMedProduct;
            EditedDetailItem = e.Row.DataContext as SupplierGenMedProduct;
            if (e.Column.Equals(GridSuppliers.GetColumnByName("colUnitPrice")))
            {
                EditedColumnName = "colUnitPrice";
            }
            else if (e.Column.Equals(GridSuppliers.GetColumnByName("colPackagePrice")))
            {
                EditedColumnName = "colPackagePrice";
            }
        }
        public void UpdatePriceButton(object sender, RoutedEventArgs e)
        {
            if (ListSupplierProduct != null && ListSupplierProduct.Count > 0)
            {
                UpdatePrice(ListSupplierProduct);
            }
        }
        public void UpdatePrice(PagedSortableCollectionView<SupplierGenMedProduct> UpdateListSupplierProduct)
        {
            List<SupplierGenMedProduct> tmpListSupplierProduct = new List<SupplierGenMedProduct>();
            foreach(var tmpUpdateListSupplierProductDetail in UpdateListSupplierProduct)
            {
                tmpListSupplierProduct.Add(tmpUpdateListSupplierProductDetail);
            }
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginUpdateDrugDeptSupplierPrice(tmpListSupplierProduct,(long)Globals.LoggedUserAccount.StaffID, (long)AllLookupValues.TypeChangePrice.DRUGDEPT, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            bool bOK = contract.EndUpdateDrugDeptSupplierPrice(asyncResult);
                            if (bOK)
                            {
                                LoadDSHangCC();
                                MessageBox.Show(eHCMSResources.Z2715_G1_ThanhCong);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                        }
                    }), null);
                }
            });
            t.Start();
        }
        #endregion
        //▲===== #001
    }
}
