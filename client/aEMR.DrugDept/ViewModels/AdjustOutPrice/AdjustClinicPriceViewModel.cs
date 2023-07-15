using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using DataEntities;
using System.Windows;
using System.Threading;
using aEMR.ServiceClient;
using System;
using aEMR.Common.Collections;
using eHCMS.Common;
using System.Windows.Controls;
using System.Linq;
using System.Collections.Generic;
using aEMR.CommonTasks;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Service.Core.Common;
using eHCMSLanguage;

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IAdjustClinicPrice)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class AdjustClinicPriceViewModel : Conductor<object>, IAdjustClinicPrice
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public AdjustClinicPriceViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Coroutine.BeginExecute(DoGetStore_EXTERNAL());
            InwardDrugMedDeptGrid = new ObservableCollection<InwardDrugClinicDept>();
        }
        public string TitleForm { get; set; }
        private long _V_MedProductType;
        public long V_MedProductType
        {
            get { return _V_MedProductType; }
            set
            {
                _V_MedProductType = value;
                NotifyOfPropertyChange(() => V_MedProductType);
            }

        }
        #region Properties Member
        private long _StoreID;
        public long StoreID
        {
            get
            {
                return _StoreID;
            }
            set
            {
                if (_StoreID != value)
                {
                    _StoreID = value;
                    NotifyOfPropertyChange(() => StoreID);
                }
            }
        }
        private ObservableCollection<RefStorageWarehouseLocation> _StoreCbx;
        public ObservableCollection<RefStorageWarehouseLocation> StoreCbx
        {
            get
            {
                return _StoreCbx;
            }
            set
            {
                if (_StoreCbx != value)
                {
                    _StoreCbx = value;
                    NotifyOfPropertyChange(() => StoreCbx);
                }
            }
        }
        #endregion
        #region Function member
        private IEnumerator<IResult> DoGetStore_EXTERNAL()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_CLINIC, false, null, false, false);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList;
            SetDefaultForStore();
            yield break;
        }
        private void SetDefaultForStore()
        {
            if (StoreCbx != null)
            {
                StoreID = StoreCbx.FirstOrDefault().StoreID;
            }
        }
        public void btnSave()
        {
            if (!CheckData())
            {
                return;
            }

            CheckInwardDrugMedDeptModified(InwardDrugMedDeptGrid);

            ObservableCollection<InwardDrugClinicDept> inwardSave = InwardDrugMedDeptGrid.Where(x => x.EntityState == EntityState.MODIFIED).ToObservableCollection();

            if (inwardSave == null || inwardSave.Count <= 0)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0544_G1_KgCoThayDoiKgTheLuu), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            SaveMedDeptAdjustOutPrice(inwardSave);
        }
        private bool CheckData()
        {
            if (InwardDrugMedDeptGrid == null || InwardDrugMedDeptGrid.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.Z0462_G1_KgCoHangDeLuu, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }

            return true;
        }
        private void CheckInwardDrugMedDeptModified(ObservableCollection<InwardDrugClinicDept> inwardDrugs)
        {
            if (inwardDrugs == null || inwardDrugs.Count <= 0)
            {
                return;
            }

            foreach (InwardDrugClinicDept item in inwardDrugs)
            {
                if (item.NormalPrice != item.NormalPrice_Orig || item.HIPatientPrice != item.HIPatientPrice_Orig || item.HIAllowedPrice != item.HIAllowedPrice_Orig)
                {
                    item.EntityState = EntityState.MODIFIED;
                }
            }
        }
        private void SaveMedDeptAdjustOutPrice(ObservableCollection<InwardDrugClinicDept> inwardSave)
        {
            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginClinicDeptAdjustOutPrice(inwardSave, Globals.LoggedUserAccount == null ? null : Globals.LoggedUserAccount.StaffID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            bool results = contract.EndClinicDeptAdjustOutPrice(asyncResult);
                            if (results)
                            {
                                MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }
                            else
                            {
                                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0477_G1_LuuKhongThanhCong), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });

            t.Start();
        }
        public void btnNew()
        {
            if (InwardDrugMedDeptGrid != null && InwardDrugMedDeptGrid.Count > 0)
            {
                if (MessageBox.Show(eHCMSResources.A0146_G1_Msg_ConfTaoMoiPh, eHCMSResources.G2363_G1_XNhan, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
            InwardDrugMedDeptGrid = new ObservableCollection<InwardDrugClinicDept>();
        }
        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedInwardDrugInGrid != null)
            {
                InwardDrugMedDeptGrid.Remove(SelectedInwardDrugInGrid);
            }
        }
        #endregion
        #region Search Product
        private ObservableCollection<InwardDrugClinicDept> _InwardDrugMedDeptList;
        public ObservableCollection<InwardDrugClinicDept> InwardDrugMedDeptList
        {
            get { return _InwardDrugMedDeptList; }
            set
            {
                if (_InwardDrugMedDeptList != value)
                {
                    _InwardDrugMedDeptList = value;
                }
                NotifyOfPropertyChange(() => InwardDrugMedDeptList);
            }
        }
        private ObservableCollection<InwardDrugClinicDept> _InwardDrugMedDeptGroup;
        public ObservableCollection<InwardDrugClinicDept> InwardDrugMedDeptGroup
        {
            get { return _InwardDrugMedDeptGroup; }
            set
            {
                if (_InwardDrugMedDeptGroup != value)
                {
                    _InwardDrugMedDeptGroup = value;
                }
                NotifyOfPropertyChange(() => InwardDrugMedDeptGroup);
            }
        }
        private InwardDrugClinicDept _SelectedInwardDrugMedDept;
        public InwardDrugClinicDept SelectedInwardDrugMedDept
        {
            get { return _SelectedInwardDrugMedDept; }
            set
            {
                if (_SelectedInwardDrugMedDept != value)
                {
                    _SelectedInwardDrugMedDept = value;
                }
                NotifyOfPropertyChange(() => SelectedInwardDrugMedDept);
            }
        }
        private InwardDrugClinicDept _SelectedInwardDrugInGrid;
        public InwardDrugClinicDept SelectedInwardDrugInGrid
        {
            get { return _SelectedInwardDrugInGrid; }
            set
            {
                if (_SelectedInwardDrugInGrid != value)
                {
                    _SelectedInwardDrugInGrid = value;
                }
                NotifyOfPropertyChange(() => SelectedInwardDrugInGrid);
            }
        }
        private ObservableCollection<InwardDrugClinicDept> _InwardDrugMedDeptGrid;
        public ObservableCollection<InwardDrugClinicDept> InwardDrugMedDeptGrid
        {
            get { return _InwardDrugMedDeptGrid; }
            set
            {
                if (_InwardDrugMedDeptGrid != value)
                {
                    _InwardDrugMedDeptGrid = value;
                }
                NotifyOfPropertyChange(() => InwardDrugMedDeptGrid);
            }
        }
        private bool _IsFocusTextCode;
        public bool IsFocusTextCode
        {
            get { return _IsFocusTextCode; }
            set
            {
                if (_IsFocusTextCode != value)
                {
                    _IsFocusTextCode = value;
                }
                NotifyOfPropertyChange(() => IsFocusTextCode);
            }
        }
        AutoCompleteBox au;
        public void AutoDrug_Text_Loaded(object sender, RoutedEventArgs e)
        {
            au = sender as AutoCompleteBox;
        }
        public void AutoCompleteBox_Populating(object sender, PopulatingEventArgs e)
        {
            SearchInwardDrugMedDept(e.Parameter, false);
        }
        TextBox tbx = null;
        public void AxTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            tbx = sender as TextBox;
        }
        public void AxTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            string txt = "";
            if (txt != (sender as TextBox).Text)
            {
                txt = (sender as TextBox).Text;

                if (!string.IsNullOrEmpty(txt))
                {
                    string Code = Globals.FormatCode(V_MedProductType, txt);

                    SearchInwardDrugMedDept(Code, true);
                }
            }
        }
        Button btnAddItem;
        public void AddItem_Loaded(object sender, RoutedEventArgs e)
        {
            btnAddItem = sender as Button;
        }
        private void SearchInwardDrugMedDept(string Name, bool IsCode)
        {
            //KMx: Không tìm tất cả. Nếu làm như vậy thì sẽ bị đứng chương trình vì quá nhiều dữ liệu (05/07/2014 11:53).
            if (IsCode == false && Name.Length < 1)
            {
                return;
            }
            IsFocusTextCode = IsCode;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetInwardDrugClinicDeptForAdjustOutPrice(StoreID, IsCode, Name, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetInwardDrugClinicDeptForAdjustOutPrice(asyncResult);
                            InwardDrugMedDeptList = results.ToObservableCollection();
                            ListDisplayAutoComplete(IsCode);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                        }
                    }), null);
                }
            });
            t.Start();
        }
        private void ListDisplayAutoComplete(bool IsCode)
        {
            var inwardGroup = from item in InwardDrugMedDeptList
                              group item by new { item.RefGenMedProductDetails.GenMedProductID, item.RefGenMedProductDetails.BrandName, item.RefGenMedProductDetails.Code } into iGroup
                              select new
                              {
                                  GenMedProductID = iGroup.Key.GenMedProductID,
                                  BrandName = iGroup.Key.BrandName,
                                  Code = iGroup.Key.Code,
                              };
            //KMx: Phải new rồi mới add. Nếu clear rồi add thì bị chậm (05/07/2014 16:55).
            InwardDrugMedDeptGroup = new ObservableCollection<InwardDrugClinicDept>();
            foreach (var i in inwardGroup)
            {
                InwardDrugClinicDept inward = new InwardDrugClinicDept();
                inward.RefGenMedProductDetails = new RefGenMedProductDetails();
                inward.RefGenMedProductDetails.GenMedProductID = i.GenMedProductID;
                inward.RefGenMedProductDetails.BrandName = i.BrandName;
                inward.RefGenMedProductDetails.Code = i.Code;
                InwardDrugMedDeptGroup.Add(inward);
            }
            if (IsCode)
            {
                if (InwardDrugMedDeptGroup != null && InwardDrugMedDeptGroup.Count > 0)
                {
                    SelectedInwardDrugMedDept = InwardDrugMedDeptGroup.ToList()[0];
                    if (btnAddItem != null)
                    {
                        btnAddItem.Focus();
                    }
                }
                else
                {
                    SelectedInwardDrugMedDept = null;
                    if (tbx != null)
                    {
                        tbx.Text = "";
                    }
                    if (au != null)
                    {
                        au.Text = "";
                    }
                    tbx.Focus();
                    MessageBox.Show(eHCMSResources.Z0463_G1_CodeKgTonTaiHoacHangHet, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                }
            }
            else
            {
                if (au != null)
                {
                    au.ItemsSource = InwardDrugMedDeptGroup;
                    au.PopulateComplete();
                }
            }
        }
        public void AddItem_Click(object sender, RoutedEventArgs e)
        {
            
            if (SelectedInwardDrugMedDept == null || SelectedInwardDrugMedDept.RefGenMedProductDetails == null || SelectedInwardDrugMedDept.RefGenMedProductDetails.GenMedProductID <= 0)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0545_G1_ChonHangCanThem), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (InwardDrugMedDeptList == null)
            {
                return;
            }
            ObservableCollection<InwardDrugClinicDept> tempList = InwardDrugMedDeptList.Where(x => x.RefGenMedProductDetails.GenMedProductID == SelectedInwardDrugMedDept.RefGenMedProductDetails.GenMedProductID).ToObservableCollection();
            if (tempList == null || tempList.Count <= 0)
            {
                return;
            }
            bool IsExist = false;
            bool IsAdd = false;
            foreach (InwardDrugClinicDept item in tempList)
            {               
                if (InwardDrugMedDeptGrid.Any(x => x.InID == item.InID))
                {
                    IsExist = true;
                }
                else
                {
                    IsAdd = true;
                    InwardDrugMedDeptGrid.Add(item);
                }
            }
            if (IsExist)
            {
                if (IsAdd)
                {
                    MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0546_G1_HangCoSoLoTrongDS), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                }
                else
                {
                    MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0547_G1_KgTheThemSoLo), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }
            }
            //Add xong, xóa Autocomplete
            SelectedInwardDrugMedDept = null;
            if (IsFocusTextCode)
            {
                if (tbx != null)
                {
                    tbx.Text = "";
                    tbx.Focus();
                }
            }
            else
            {
                if (au != null)
                {
                    au.Text = "";
                    au.Focus();
                }
            }
        }
        #endregion
    }
}
