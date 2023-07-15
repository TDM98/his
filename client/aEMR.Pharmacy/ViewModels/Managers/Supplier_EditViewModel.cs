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
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Controls;
using System.Linq;
using System.Collections.Generic;
/*
 * 20190911 #001 TTM:   BM 0013236: Cho phép sửa giá lẻ nhà cung cấp tại màn hình quản lý nhà cung cấp. Để phục vụ sửa giá hàng loạt cho nhà cung cấp.
 * 20191109 #002 TTM:   BM 0018533: Bổ sung thông báo lỗi khi cập nhật nhà cung cấp => Vì ValidationSummary không còn hoạt động được trong WPF nên phải có thông báo cho người dùng biết thiếu thông tin gì.
 * 20220404 #003 DatTB: Phân loại NNC: Cho cập nhật loại "Dùng chung" ở màn hình nhà thuốc
 */
namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(ISupplier_Edit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class Supplier_EditViewModel : Conductor<object>, ISupplier_Edit
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

            _SelectedSupplier = new Supplier();

            ListSupplierDrug = new PagedSortableCollectionView<SupplierGenericDrug>();
            ListSupplierDrug.OnRefresh += ListSupplierDrug_OnRefresh;
            ListSupplierDrug.PageSize = Globals.PageSize;
        }
        void ListSupplierDrug_OnRefresh(object sender, RefreshEventArgs e)
        {
            SupplierGenericDrug_LoadSupplierID(ListSupplierDrug.PageIndex, ListSupplierDrug.PageSize);
        }

        protected override void OnDeactivate(bool close)
        {
            ListSupplierDrug = null;
            SelectedSupplier = null;
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

        private PagedSortableCollectionView<SupplierGenericDrug> _ListSupplierDrug;
        public PagedSortableCollectionView<SupplierGenericDrug> ListSupplierDrug
        {
            get
            {
                return _ListSupplierDrug;
            }
            set
            {
                if (_ListSupplierDrug != value)
                {
                    _ListSupplierDrug = value;
                }
                NotifyOfPropertyChange(() => ListSupplierDrug);
            }
        }

        private Supplier _SelectedSupplier;
        public Supplier SelectedSupplier
        {
            get
            {
                if (_SelectedSupplier == null)
                    _SelectedSupplier = new Supplier();
                //▼===== #003
                if (_SelectedSupplier.SupplierDrugDeptPharmOthers == 2)
                {
                    DrugDeptPharm = true;
                    DrugDeptPublic = false;
                }
                if (_SelectedSupplier.SupplierDrugDeptPharmOthers == 3)
                {
                    DrugDeptPharm = false;
                    DrugDeptPublic = true;
                }
                //▲===== #003
                return _SelectedSupplier;
            }
            set
            {
                _SelectedSupplier = value;
                NotifyOfPropertyChange(() => SelectedSupplier);
            }
        }
        private Visibility _bOKButton;
        public Visibility bOKButton
        {
            get
            {
                return _bOKButton;
            }
            set
            {
                _bOKButton = value;
                NotifyOfPropertyChange(() => bOKButton);
            }
        }

        //▼===== #003
        private bool _DrugDeptPharm = true;
        public bool DrugDeptPharm
        {
            get
            {
                return _DrugDeptPharm;
            }
            set
            {
                if (_DrugDeptPharm != value)
                {
                    _DrugDeptPharm = value;
                    NotifyOfPropertyChange(() => DrugDeptPharm);
                }
            }
        }

        private bool _DrugDeptPublic = true;
        public bool DrugDeptPublic
        {
            get
            {
                return _DrugDeptPublic;
            }
            set
            {
                if (_DrugDeptPublic != value)
                {
                    _DrugDeptPublic = value;
                    NotifyOfPropertyChange(() => DrugDeptPublic);
                }
            }
        }

        public void SupplierType_Checked(object sender, byte value)
        {
            SelectedSupplier.SupplierDrugDeptPharmOthers = value;
        }
        //▲===== #003

        public bool CheckValid(object temp)
        {
            Supplier u = temp as Supplier;
            if (u == null)
            {
                return false;
            }
            return u.Validate();
        }

        public void UpdateSupplier()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginUpdateSupplier(SelectedSupplier, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            string StrError = "";
                            contract.EndUpdateSupplier(out StrError, asyncResult);
                            if (string.IsNullOrEmpty(StrError))
                            {
                                GlobalsNAV.ShowMessagePopup(eHCMSResources.A0279_G1_Msg_InfoCNhatOK);
                                TryClose();
                                //phat su kien de form cha load lai du lieu
                                Globals.EventAggregator.Publish(new PharmacyCloseEditSupplierEvent());
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
            if(!CheckValidForSupplier(out ErrorMsg, SelectedSupplier))
            {
                MessageBox.Show(ErrorMsg,eHCMSResources.G0449_G1_TBaoLoi);
                return;
            }
            else
            {
                UpdateSupplier();
            }
            //▲===== #002
        }
        private bool CheckValidForSupplier(out string Error, Supplier curSupplier)
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
            Globals.EventAggregator.Publish(new PharmacyCloseAddSupplierEvent());
        }

        #region load DS Hang CC Member

        public void GridSuppliers_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        private void SupplierGenericDrug_LoadSupplierID(int PageIndex, int PageSize)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            int totalCount = 0; 
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSupplierGenericDrug_LoadSupplierID(SelectedSupplier.SupplierID, PageSize, PageIndex, true, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndSupplierGenericDrug_LoadSupplierID(out totalCount, asyncResult);
                            if (results != null)
                            {
                                ListSupplierDrug.Clear();
                                ListSupplierDrug.TotalItemCount = totalCount;
                                foreach (SupplierGenericDrug p in results)
                                {
                                    ListSupplierDrug.Add(p);
                                }
                                NotifyOfPropertyChange(() => ListSupplierDrug);
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

        #endregion

        public void LoadDsHangCC()
        {
            ListSupplierDrug.PageIndex = 0;
            SupplierGenericDrug_LoadSupplierID(ListSupplierDrug.PageIndex,ListSupplierDrug.PageSize);
        }
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
        private SupplierGenericDrug EditedDetailItem { get; set; }
        private string EditedColumnName { get; set; }
        private SupplierGenericDrug _CurrentSupplierGenericDrug;
        public SupplierGenericDrug CurrentSupplierGenericDrug
        {
            get
            {
                return _CurrentSupplierGenericDrug;
            }
            set
            {
                if (_CurrentSupplierGenericDrug != value)
                {
                    _CurrentSupplierGenericDrug = value;
                    NotifyOfPropertyChange(() => CurrentSupplierGenericDrug);
                }
            }
        }
        private void GridSuppliers_CurrentCellChanged(object sender, EventArgs e)
        {
            if (CurrentSupplierGenericDrug == null || string.IsNullOrEmpty(EditedColumnName))
            {
                return;
            }
            SupplierGenericDrug item = CurrentSupplierGenericDrug;
            if (EditedColumnName.Equals("colUnitPrice"))
            {
                EditedColumnName = null;
                decimal value = 0;
                decimal.TryParse(PreparingCellForEdit, out value);
                if (value == item.UnitPrice)
                {
                    return;
                }
                item.PackagePrice = item.UnitPrice * (int)item.SelectedGenericDrug.UnitPackaging;
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
                    item.UnitPrice = item.PackagePrice / (int)item.SelectedGenericDrug.UnitPackaging;
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
            SupplierGenericDrug item = e.Row.DataContext as SupplierGenericDrug;
            EditedDetailItem = e.Row.DataContext as SupplierGenericDrug;
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
            if (ListSupplierDrug != null && ListSupplierDrug.Count > 0)
            {
                UpdatePrice(ListSupplierDrug);
            }
        }
        public void UpdatePrice(PagedSortableCollectionView<SupplierGenericDrug> UpdateListSupplierProduct)
        {
            List<SupplierGenericDrug> tmpListSupplierProduct = new List<SupplierGenericDrug>();
            foreach (var tmpUpdateListSupplierProductDetail in UpdateListSupplierProduct)
            {
                tmpListSupplierProduct.Add(tmpUpdateListSupplierProductDetail);
            }
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginUpdateSupplierPrice(tmpListSupplierProduct, (long)Globals.LoggedUserAccount.StaffID, (long)AllLookupValues.TypeChangePrice.PHARMACY, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            bool bOK = contract.EndUpdateSupplierPrice(asyncResult);
                            if (bOK)
                            {
                                LoadDsHangCC();
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
