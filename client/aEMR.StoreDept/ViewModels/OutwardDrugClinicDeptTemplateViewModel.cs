using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using System.Linq;
using System.Collections.Generic;
using aEMR.Common;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.ComponentModel;
using eHCMSLanguage;
using aEMR.Controls;
using System.Windows.Input;
using aEMR.Common.ExportExcel;
/*
* 20230508 #001 DatTB: Thêm nút xuất Excel danh sách các mẫu lĩnh
*/
namespace aEMR.StoreDept.ViewModels
{
    [Export(typeof(IOutwardDrugClinicDeptTemplate)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class OutwardDrugClinicDeptTemplateViewModel : Conductor<object>, IOutwardDrugClinicDeptTemplate
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public OutwardDrugClinicDeptTemplateViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            Globals.EventAggregator.Subscribe(this);

            MedProductTypeCollection = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_MedProductType).ToObservableCollection();
            RefreshOutwardTemplate();

            RefGenMedProductDetails = new PagedSortableCollectionView<RefGenMedProductDetails>();
            RefGenMedProductDetails.OnRefresh += RefGenMedProductDetails_OnRefresh;
            RefGenMedProductDetails.PageSize = Globals.PageSize;

            DepartmentContent = Globals.GetViewModel<IDepartmentListing>();

            (DepartmentContent as PropertyChangedBase).PropertyChanged += new PropertyChangedEventHandler(OutwardTemplate_PropertyChanged);

            //InitSelDeptCombo();
            authorization();
        }

        private IDepartmentListing _departmentContent;
        public IDepartmentListing DepartmentContent
        {
            get { return _departmentContent; }
            set
            {
                _departmentContent = value;
                NotifyOfPropertyChange(() => DepartmentContent);
            }
        }

        private bool _calByUnitUse;
        public bool CalByUnitUse
        {
            get { return _calByUnitUse; }
            set
            {
                if (_calByUnitUse != value)
                {
                    _calByUnitUse = value;
                    NotifyOfPropertyChange(() => CalByUnitUse);
                }
            }
        }

        void OutwardTemplate_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                if (DepartmentContent.SelectedItem != null)
                {
                    GetAllOutwardTemplate(DepartmentContent.SelectedItem.DeptID,IsShared);
                }
            }
        }

        void RefGenMedProductDetails_OnRefresh(object sender, aEMR.Common.Collections.RefreshEventArgs e)
        {
            GetRefGenMedProductDetails_Auto(BrandName, RefGenMedProductDetails.PageIndex, RefGenMedProductDetails.PageSize);
        }

        #region 1. Property

        private string _strHienThi;
        public string strHienThi
        {
            get
            {
                return _strHienThi;
            }
            set
            {
                _strHienThi = value;
                NotifyOfPropertyChange(() => strHienThi);
            }
        }


        private PagedSortableCollectionView<RefGenMedProductDetails> _RefGenMedProductDetails;
        public PagedSortableCollectionView<RefGenMedProductDetails> RefGenMedProductDetails
        {
            get
            {
                return _RefGenMedProductDetails;
            }
            set
            {
                if (_RefGenMedProductDetails != value)
                {
                    _RefGenMedProductDetails = value;
                    NotifyOfPropertyChange(() => RefGenMedProductDetails);
                }
            }
        }


        private OutwardDrugClinicDeptTemplate _OutwardTemplate;
        public OutwardDrugClinicDeptTemplate OutwardTemplate
        {
            get
            {
                return _OutwardTemplate;
            }
            set
            {
                if (_OutwardTemplate != value)
                {
                    _OutwardTemplate = value;
                    NotifyOfPropertyChange(() => OutwardTemplate);
                }
            }
        }


        private OutwardDrugClinicDeptTemplateItem _CurrentReqOutwardDrugClinicDeptPatient;
        public OutwardDrugClinicDeptTemplateItem CurrentOutwardTemplateItem
        {
            get { return _CurrentReqOutwardDrugClinicDeptPatient; }
            set
            {
                if (_CurrentReqOutwardDrugClinicDeptPatient != value)
                {
                    _CurrentReqOutwardDrugClinicDeptPatient = value;
                    NotifyOfPropertyChange(() => CurrentOutwardTemplateItem);
                }
            }
        }

        private ObservableCollection<OutwardDrugClinicDeptTemplate> _OutwardTemplateList;
        public ObservableCollection<OutwardDrugClinicDeptTemplate> OutwardTemplateList
        {
            get { return _OutwardTemplateList; }
            set
            {
                _OutwardTemplateList = value;
                NotifyOfPropertyChange(() => OutwardTemplateList);
            }
        }

        private OutwardDrugClinicDeptTemplate _SelectedOutwardTemplate;
        public OutwardDrugClinicDeptTemplate SelectedOutwardTemplate
        {
            get { return _SelectedOutwardTemplate; }
            set
            {
                _SelectedOutwardTemplate = value;
                NotifyOfPropertyChange(() => SelectedOutwardTemplate);
            }
        }


        private OutwardDrugClinicDeptTemplateItem _SelectedOutwardTemplateItem;
        public OutwardDrugClinicDeptTemplateItem SelectedOutwardTemplateItem
        {
            get { return _SelectedOutwardTemplateItem; }
            set
            {
                _SelectedOutwardTemplateItem = value;
                NotifyOfPropertyChange(() => SelectedOutwardTemplateItem);
            }
        }



        private long _V_MedProductType = (long)AllLookupValues.MedProductType.THUOC; //11001 : thuoc, 11002 : y cu , 11003 :hoa chat
        public long V_MedProductType
        {
            get
            {
                return _V_MedProductType;
            }
            set
            {
                if (_V_MedProductType != value)
                {
                    _V_MedProductType = value;
                    authorization();
                    NotifyOfPropertyChange(() => V_MedProductType);
                    NotifyOfPropertyChange(() => IsGeneralType);
                }
            }
        }

        private bool _VisibilityName = true;
        public bool VisibilityName
        {
            get
            {

                return _VisibilityName;
            }
            set
            {
                _VisibilityName = value;
                _VisibilityCode = !_VisibilityName;
                NotifyOfPropertyChange(() => VisibilityName);
                NotifyOfPropertyChange(() => VisibilityCode);

            }
        }

        private bool _VisibilityCode = false;
        public bool VisibilityCode
        {
            get
            {
                return _VisibilityCode;
            }
            set
            {
                if (_VisibilityCode != value)
                {
                    _VisibilityCode = value;
                    NotifyOfPropertyChange(() => VisibilityCode);
                }
            }
        }
        private ObservableCollection<Lookup> _MedProductTypeCollection;
        public ObservableCollection<Lookup> MedProductTypeCollection
        {
            get
            {
                return _MedProductTypeCollection;
            }
            set
            {
                if (_MedProductTypeCollection == value)
                {
                    return;
                }
                _MedProductTypeCollection = value;
                NotifyOfPropertyChange(() => MedProductTypeCollection);
            }
        }
        public bool IsGeneralType
        {
            get
            {
                return V_MedProductType == (long)AllLookupValues.MedProductType.Unknown;
            }
        }
        public long CurrentMedProductType { get; set; }
        private long SelectedMedProductType
        {
            get
            {
                return IsGeneralType ? CurrentMedProductType : V_MedProductType;
            }
        }

        private long _V_OutwardTemplateType = (long)AllLookupValues.V_OutwardTemplateType.OutwardTemplate;
        public long V_OutwardTemplateType
        {
            get
            {
                return _V_OutwardTemplateType;
            }
            set
            {
                if (_V_OutwardTemplateType == value)
                {
                    return;
                }
                _V_OutwardTemplateType = value;
                NotifyOfPropertyChange(() => V_OutwardTemplateType);
            }
        }
        #endregion

        #region 3. Function Member

        private Staff GetStaffLogin()
        {
            return Globals.LoggedUserAccount.Staff;
        }

        private void RefreshOutwardTemplate()
        {
            IsEnabledEditItem = true;
            SelectedOutwardTemplate = null;
            OutwardTemplate = new OutwardDrugClinicDeptTemplate();
            OutwardTemplate.CreateDate = Globals.GetCurServerDateTime();
            OutwardTemplate.CreatedStaff = GetStaffLogin();
            CurrentOutwardTemplateItem = new OutwardDrugClinicDeptTemplateItem();
        }

        public void lnkDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (OutwardTemplate == null || SelectedOutwardTemplateItem == null)
            {
                return;
            }

            if (OutwardTemplate.OutwardTemplateItems_Behind == null)
            {
                OutwardTemplate.OutwardTemplateItems_Behind = new ObservableCollection<OutwardDrugClinicDeptTemplateItem>();
            }

            SelectedOutwardTemplateItem.V_RecordState = (long)AllLookupValues.V_RecordState.DELETE;

            OutwardTemplate.OutwardTemplateItems_Behind.Add(SelectedOutwardTemplateItem);

            OutwardTemplate.OutwardTemplateItems.Remove(SelectedOutwardTemplateItem);
        }


        private bool CheckBeforeSave()
        {
            if (DepartmentContent == null || DepartmentContent.SelectedItem == null)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0493_G1_HayChonKhoa), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }

            if (OutwardTemplate == null || OutwardTemplate.OutwardTemplateItems == null || OutwardTemplate.OutwardTemplateItems.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0818_G1_Msg_InfoMauKhCoCTiet, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            if (OutwardTemplate.IsShared == false && DepartmentContent.SelectedItem.DeptID <= 0)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0493_G1_HayChonKhoa), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            if (string.IsNullOrWhiteSpace(OutwardTemplate.OutwardDrugClinicDeptTemplateName))
            {
                MessageBox.Show(eHCMSResources.A0602_G1_Msg_InfoNhapTenMau, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }

            OutwardDrugClinicDeptTemplateItem item = OutwardTemplate.OutwardTemplateItems.Where(x => x.ReqOutQuantity < 0).FirstOrDefault();

            if (item != null)
            {
                MessageBox.Show(string.Format("Loại hàng: {0} Số lượng phải >= 0!", item.RefGenericDrugDetail.BrandName), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }

            return true;
        }

        public void PrepareOutwardTemplateItemToSave()
        {
            if (OutwardTemplate == null || OutwardTemplate.OutwardTemplateItems == null || OutwardTemplate.OutwardTemplateItems.Count <= 0)
            {
                return;
            }

            if (OutwardTemplate.OutwardTemplateItems_Behind == null)
            {
                OutwardTemplate.OutwardTemplateItems_Behind = new ObservableCollection<OutwardDrugClinicDeptTemplateItem>();
            }

            foreach (OutwardDrugClinicDeptTemplateItem item in OutwardTemplate.OutwardTemplateItems)
            {
                if (item.V_RecordState == (long)AllLookupValues.V_RecordState.UNCHANGE && (item.ReqOutQuantity != item.ReqOutQuantity_Orig || item.ItemNote != item.ItemNote_Orig))
                {
                    item.V_RecordState = (long)AllLookupValues.V_RecordState.UPDATE;
                }

                if (item.V_RecordState != (long)AllLookupValues.V_RecordState.UNCHANGE)
                {
                    OutwardTemplate.OutwardTemplateItems_Behind.Add(item);
                }
            }
        }

        private void SaveOutwardTemplate()
        {
            OutwardTemplate.V_MedProductType = V_MedProductType;
            OutwardTemplate.V_OutwardTemplateType = V_OutwardTemplateType;
            OutwardTemplate.CreatedStaff = GetStaffLogin();
            OutwardTemplate.Department = DepartmentContent.SelectedItem;
            this.ShowBusyIndicator();
            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSaveOutwardDrugClinicDeptTemplate(OutwardTemplate, Globals.DispatchCallback((asyncResult) =>
                        {

                            try
                            {
                                OutwardDrugClinicDeptTemplate OutwardTemplateOut;
                                bool result = contract.EndSaveOutwardDrugClinicDeptTemplate(out OutwardTemplateOut, asyncResult);

                                if (result)
                                {
                                    OutwardTemplate = OutwardTemplateOut;

                                    if (DepartmentContent != null && DepartmentContent.SelectedItem != null)
                                    {
                                        GetAllOutwardTemplate(DepartmentContent.SelectedItem.DeptID,IsShared);
                                    }

                                    MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0701_G1_Msg_InfoLuuFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                ClientLoggerHelper.LogError(ex.ToString());
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                ClientLoggerHelper.LogError(ex.ToString());
                this.HideBusyIndicator();
            }
        }


        public void btnSave(object sender, RoutedEventArgs e)
        {
            if (!CheckBeforeSave())
            {
                return;
            }

            if (OutwardTemplate.OutwardDrugClinicDeptTemplateID > 0)
            {
                if (MessageBox.Show(eHCMSResources.A0117_G1_Msg_ConfLuuThayDoi, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            PrepareOutwardTemplateItemToSave();

            SaveOutwardTemplate();
        }

        public void btnNew(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.A0146_G1_Msg_ConfTaoMoiPh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                RefreshOutwardTemplate();
            }
        }

        private void DeleteOutwardDrugClinicDeptTemplate(long OutwardTemplateID)
        {
            this.ShowBusyIndicator();
            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeleteOutwardDrugClinicDeptTemplate(OutwardTemplateID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool result = contract.EndDeleteOutwardDrugClinicDeptTemplate(asyncResult);

                                RefreshOutwardTemplate();

                                if (DepartmentContent != null && DepartmentContent.SelectedItem != null)
                                {
                                    GetAllOutwardTemplate(DepartmentContent.SelectedItem.DeptID,IsShared);
                                }

                                if (result)
                                {
                                    MessageBox.Show(eHCMSResources.K0528_G1_XoaMauOk, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.K0529_G1_XoaMauFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                ClientLoggerHelper.LogError(ex.ToString());
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                ClientLoggerHelper.LogError(ex.ToString());
                this.HideBusyIndicator();
            }
        }


        public void btnDelete(object sender, RoutedEventArgs e)
        {
            if (OutwardTemplate == null || OutwardTemplate.OutwardDrugClinicDeptTemplateID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0914_G1_Msg_InfoKhTheXoa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (MessageBox.Show(eHCMSResources.A0120_G1_Msg_ConfXoaPh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeleteOutwardDrugClinicDeptTemplate(OutwardTemplate.OutwardDrugClinicDeptTemplateID);
            }
        }

        public void SetSelectedOutwardTemplate()
        {
            if (OutwardTemplate != null && OutwardTemplate.OutwardDrugClinicDeptTemplateID > 0
                && OutwardTemplateList != null && OutwardTemplateList.Count > 0)
            {
                SelectedOutwardTemplate = OutwardTemplateList.Where(x => x.OutwardDrugClinicDeptTemplateID == OutwardTemplate.OutwardDrugClinicDeptTemplateID).FirstOrDefault();
            }
        }


        public void GetAllOutwardTemplate(long DeptID, bool IsShared)
        {
            if (DeptID <= 0 && Globals.isAccountCheck)
            {
                OutwardTemplateList = new ObservableCollection<OutwardDrugClinicDeptTemplate>();
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllOutwardTemplate((long)V_MedProductType, DeptID, V_OutwardTemplateType, IsShared, Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<OutwardDrugClinicDeptTemplate> allItems = new ObservableCollection<OutwardDrugClinicDeptTemplate>();
                            try
                            {
                                allItems = contract.EndGetAllOutwardTemplate(asyncResult);

                                OutwardTemplateList = new ObservableCollection<OutwardDrugClinicDeptTemplate>(allItems);

                                SetSelectedOutwardTemplate();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                this.HideBusyIndicator();
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        private void GetOutwardTemplateByID(long OutwardTemplateID)
        {
            this.ShowBusyIndicator();
            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetOutwardTemplateByID(OutwardTemplateID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                OutwardTemplate = contract.EndGetOutwardTemplateByID(asyncResult);
                                CheckEnableEditItem();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                ClientLoggerHelper.LogError(ex.ToString());
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                ClientLoggerHelper.LogError(ex.ToString());
                this.HideBusyIndicator();
            }
        }

        public void grdOutwardTemplate_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        public void grdOutwardTemplate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedOutwardTemplate == null || SelectedOutwardTemplate.OutwardDrugClinicDeptTemplateID <= 0)
            {
                return;
            }
            GetOutwardTemplateByID(SelectedOutwardTemplate.OutwardDrugClinicDeptTemplateID);
        }

        #endregion

        #region AutoGenMedProduct Member
        private string BrandName;


        private void GetRefGenMedProductDetails_Auto(string BrandName, int PageIndex, int PageSize)
        {
            if (IsCode == false && BrandName.Length < 1)
            {
                return;
            }

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginRefGenMedProductDetails_SearchAutoPaging_V2(IsCode, BrandName, null, SelectedMedProductType, null, PageSize, PageIndex, null, null, true, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int Total;
                            var results = contract.EndRefGenMedProductDetails_SearchAutoPaging_V2(out Total, asyncResult);
                            if (IsCode.GetValueOrDefault())
                            {
                                if (results != null && results.Count > 0)
                                {
                                    if (CurrentOutwardTemplateItem == null)
                                    {
                                        CurrentOutwardTemplateItem = new OutwardDrugClinicDeptTemplateItem();
                                    }
                                    CurrentOutwardTemplateItem.RefGenericDrugDetail = results.FirstOrDefault();
                                }
                                else
                                {

                                    if (CurrentOutwardTemplateItem != null)
                                    {
                                        CurrentOutwardTemplateItem.RefGenericDrugDetail = null;
                                    }

                                    if (tbx != null)
                                    {
                                        txt = "";
                                        tbx.Text = "";
                                    }
                                    if (au != null)
                                    {
                                        au.Text = "";
                                    }

                                    MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                                }
                            }
                            else
                            {
                                RefGenMedProductDetails.Clear();
                                RefGenMedProductDetails.TotalItemCount = Total;
                                RefGenMedProductDetails.SourceCollection = results;
                                au.PopulateComplete();
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


        bool? IsCode = false;
        public void Code_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            IsCode = true;
            VisibilityName = false;
        }

        public void Name_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            IsCode = false;
            VisibilityName = true;
        }
        public void AddItem_Click(object sender, object e)
        {
            btnAddItem();
        }
        public void btnAddItem()
        {

            // TxD 22/12/2014: Modify the following condition to check when item is blank or Code is blank 
            if (CurrentOutwardTemplateItem.RefGenericDrugDetail == null || CurrentOutwardTemplateItem.RefGenericDrugDetail.GenMedProductID == 0
                                    || string.IsNullOrEmpty(CurrentOutwardTemplateItem.RefGenericDrugDetail.Code))
            {
                CurrentOutwardTemplateItem.RefGenericDrugDetail = new RefGenMedProductDetails();

                Globals.ShowMessage(eHCMSResources.A0579_G1_Msg_InfoChonHgCanThem, eHCMSResources.G0442_G1_TBao);
                return;
            }

            if (CurrentOutwardTemplateItem.ReqOutQuantity < 0)
            {
                MessageBox.Show("Số lượng phải >= 0", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (CalByUnitUse)
            {
                //KMx: Tính ra số lượng dựa vào DispenseVolume (15/11/2014 09:01).
                CurrentOutwardTemplateItem.ReqOutQuantity = CurrentOutwardTemplateItem.ReqOutQuantity / (CurrentOutwardTemplateItem.RefGenericDrugDetail.DispenseVolume == 0 ? 1 : (decimal)CurrentOutwardTemplateItem.RefGenericDrugDetail.DispenseVolume);
                CurrentOutwardTemplateItem.ReqOutQuantity = Math.Round(CurrentOutwardTemplateItem.ReqOutQuantity, 2);
            }

            if (OutwardTemplate == null)
            {
                OutwardTemplate = new OutwardDrugClinicDeptTemplate();
            }
            if (OutwardTemplate.OutwardTemplateItems == null)
            {
                OutwardTemplate.OutwardTemplateItems = new ObservableCollection<OutwardDrugClinicDeptTemplateItem>();
            }
            var temp = OutwardTemplate.OutwardTemplateItems.Where(x => x.RefGenericDrugDetail != null && CurrentOutwardTemplateItem.RefGenericDrugDetail != null && x.RefGenericDrugDetail.GenMedProductID == CurrentOutwardTemplateItem.RefGenericDrugDetail.GenMedProductID);
            if (temp != null && temp.Count() > 0)
            {
                temp.ToList()[0].ReqOutQuantity += CurrentOutwardTemplateItem.ReqOutQuantity;
            }
            else
            {
                var item = CurrentOutwardTemplateItem.DeepCopy();
                OutwardTemplate.OutwardTemplateItems.Add(item);
            }

            CurrentOutwardTemplateItem.RefGenericDrugDetail = new RefGenMedProductDetails();
            CurrentOutwardTemplateItem.ReqOutQuantity = 0;

            if (IsCode.GetValueOrDefault())
            {
                if (tbx != null)
                {
                    txt = "";
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
        AxGrid RootAxGrid;
        public void AxGrid_Loaded(object sender, RoutedEventArgs e)
        {
            RootAxGrid = sender as AxGrid;
        }
        public void AddItem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (RootAxGrid != null)
                {
                    RootAxGrid.DisableFirstNextFocus = true;
                }
            }
        }

        TextBox tbx = null;
        public void AxTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            tbx = sender as TextBox;
        }

        string txt = "";

        public void AxTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            txt = (sender as TextBox).Text;

            System.Diagnostics.Debug.WriteLine(" ====> AxTextBox_LostFocus 1 .....");

            if (!string.IsNullOrEmpty(txt))
            {
                string Code = Globals.FormatCode(SelectedMedProductType, txt);

                GetRefGenMedProductDetails_Auto(Code, 0, RefGenMedProductDetails.PageSize);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(" ====> AxTextBox_LostFocus 2 .....");
                // TxD 23/12/2014 If Code is blank then clear selected item
                CurrentOutwardTemplateItem.RefGenericDrugDetail = new RefGenMedProductDetails();
            }
        }

        AutoCompleteBox au;
        public void AutoCompleteBox_Populating(object sender, PopulatingEventArgs e)
        {
            if (!IsCode.GetValueOrDefault())
            {
                au = sender as AutoCompleteBox;
                BrandName = e.Parameter;
                RefGenMedProductDetails.PageIndex = 0;
                GetRefGenMedProductDetails_Auto(e.Parameter, 0, RefGenMedProductDetails.PageSize);
            }
        }

        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            RefGenMedProductDetails obj = (sender as AutoCompleteBox).SelectedItem as RefGenMedProductDetails;
            //if (CurrentOutwardTemplateItem != null)
            if (obj != null && CurrentOutwardTemplateItem != null)
            {
                CurrentOutwardTemplateItem.RefGenericDrugDetail = obj;
            }
        }

        public void AutoDrug_Text_Loaded(object sender, RoutedEventArgs e)
        {
            au = sender as AutoCompleteBox;
        }

        public void InitSelDeptCombo()
        {
            DepartmentContent.LstRefDepartment = new ObservableCollection<long>();

            if (Globals.isAccountCheck)
            {
                DepartmentContent.AddSelectOneItem = true;

                DepartmentContent.LstRefDepartment = Globals.LoggedUserAccount.DeptIDResponsibilityList;
            }
            else
            {
                DepartmentContent.AddSelectedAllItem = true;
            }

            DepartmentContent.LoadData(0, true);

            //KMx: Không cần gọi hàm GetAllOutwardTemplate vì khi set SelectedItem Khoa thì đã gọi rồi (23/01/2016 15:36).
            //if (DepartmentContent != null && DepartmentContent.SelectedItem != null)
            //{
            //    GetAllOutwardTemplate(DepartmentContent.SelectedItem.DeptID);
            //}

        }
        public bool IsShared
        {
            get
            {
                return _IsShared;
            }
            set
            {
                if (_IsShared != value)
                {
                    _IsShared = value;
                    if (DepartmentContent.SelectedItem != null)
                    {
                        GetAllOutwardTemplate(DepartmentContent.SelectedItem.DeptID, IsShared);
                    }
                    NotifyOfPropertyChange(() => IsShared);
                }
            }
        }
        private bool _IsShared;

        public bool bIsShared
        {
            get
            {
                return _bIsShared;
            }
            set
            {
                if (_bIsShared != value)
                {
                    _bIsShared = value;
                    NotifyOfPropertyChange(() => bIsShared);
                }
            }
        }
        private bool _bIsShared = false;

        public bool IsEnabledEditItem
        {
            get
            {
                return _IsEnabledEditItem;
            }
            set
            {
                if (_IsEnabledEditItem != value)
                {
                    _IsEnabledEditItem = value;
                    NotifyOfPropertyChange(() => IsEnabledEditItem);
                }
            }
        }
        private bool _IsEnabledEditItem;

        private void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                bIsShared = true;
                return;
            }

            switch(V_MedProductType)
            {
                case (long)AllLookupValues.MedProductType.THUOC:
                    bIsShared = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong, (int)eKhoPhong.mTaoMauLinh_Thuoc_DungChung);
                    break;
                case (long)AllLookupValues.MedProductType.Y_CU:
                    bIsShared = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong, (int)eKhoPhong.mTaoMauLinh_YCu_DungChung);
                    break;
                case (long)AllLookupValues.MedProductType.VATTU_TIEUHAO:
                    bIsShared = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mKhoPhong, (int)eKhoPhong.mTaoMauLinh_VTTH_DungChung);
                    break;
                default:
                    bIsShared = false;
                    break;
            }
        }
        private void CheckEnableEditItem()
        {
            if (OutwardTemplate == null || OutwardTemplate.CreatedStaff == null)
            {
                return;
            }
            if(OutwardTemplate.IsShared && OutwardTemplate.CreatedStaff.StaffID != Globals.LoggedUserAccount.StaffID)
            {
                IsEnabledEditItem = false;
            }
            else
            {
                IsEnabledEditItem = true;
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
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginExportExcelOutwardClinicDeptTemplates(V_MedProductType, V_OutwardTemplateType, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                //strNameExcel = string.Format("{0} ", eHCMSResources.Z2374_G1_ChoXacNhan);
                                var results = contract.EndExportExcelOutwardClinicDeptTemplates(asyncResult);
                                ExportToExcelFileAllData.Export(results, "Sheet1");
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